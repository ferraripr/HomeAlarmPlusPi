/* 
 * HttpServer.cs
 * 
 * Copyright (c) 2009, Michael Schwarz (http://www.schwarz-interactive.de)
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or
 * sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR
 * ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
 * CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 * MS   09-02-10    added MT support
 * MS   09-03-09    changed stop http server when there is any exception while starting (i.e. when port is not available)
 * MS   09-04-30    fixed closing threads
 * MS   09-06-19    added support for SSL
 * MS   10-11-08    fixed non-thread safe usage (work item 4318)
 * 
 */
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Threading;
using Socket = System.Net.Sockets.Socket;

#if(SSL)
#if(!MF)
using System.Security.Cryptography.X509Certificates;
#else
using Microsoft.SPOT.Net.Security;
#endif
#endif

namespace MFToolkit.Net.Web
{
	public class HttpServer : IDisposable
	{
        private IHttpHandler _httpHandler;
        private int _port = 80;
        private IPAddress _address = IPAddress.Any;
        private Socket _listenSocket;
        private ArrayList _workerThreads = new ArrayList();
        private Thread _thdListener;
        private Thread _thdWorker;
        private bool _stopThreads = true;
        private const int _maxWorkers = 256;        // for AJAX enabled web sites we need a higher max worker process count

#if(SSL)
        private bool _isSecure = false;
        private X509Certificate _certificate;
#endif

        #region Events

        public event LogAccessEventHandler LogAccess;
        public event ClientConnectedEventHandler ClientConnected;

        #endregion

        #region Constructors

        public HttpServer(IHttpHandler Handler)
        {
            _httpHandler = Handler;
        }

        public HttpServer(int Port, IHttpHandler Handler)
            : this(Handler)
        {
            _port = Port;
        }

        public HttpServer(int Port, IPAddress Address, IHttpHandler Handler)
            : this(Port, Handler)
        {
            _address = Address;
        }

#if(!MF)
        public HttpServer(IPAddress Address, IHttpHandler Handler)
            : this(Handler)
        {
            _address = Address;
        }
#endif

        #endregion

        #region Public Properties

        public int Port
        {
            get { return _port; }
        }

        public IPAddress Address
        {
            get { return _address; }
        }

#if(SSL)
        public bool IsSecure
        {
            get { return _isSecure; }
            set { _isSecure = value; }
        }

        public X509Certificate Certificate
        {
            get { return _certificate; }
            set { _certificate = value; }
        }
#endif

        #endregion

        public bool Start()
        {
            try
            {
                if (_stopThreads)
                {
                    _stopThreads = false;

                    _thdWorker = new Thread(new ThreadStart(RemoveWorkerThreads));

                    _thdWorker.Start();

                    _listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    // TODO: check if there is already a binding for the IPEndPoint
                    _listenSocket.Bind(new IPEndPoint(_address, _port));
                    _listenSocket.Listen(30);

                    _thdListener = new Thread(new ThreadStart(ListenerThread));

                    _thdListener.Start();
                }
            }
            catch (Exception)
            {
                Stop();
                return false;
            }

            return true;
        }

        public bool Stop()
        {
            _stopThreads = true;

            int count = 0;

            if (_thdListener != null && _thdListener.ThreadState != ThreadState.Stopped)
            {
                try
                {
                    _listenSocket.Close();
                    _thdListener.Abort();
                }
                finally
                {
                    _thdListener = null;
                }
            }

            while (++count < 30)
            {
                if (_thdWorker != null && _thdWorker.ThreadState == ThreadState.Stopped)
                {
                    _thdWorker = null;

                    return true;
                }

                Thread.Sleep(10);
            }

            return false;
        }

        private void ListenerThread()
        {
            while (!_stopThreads)
            {
                Socket client = null;

                try
                {
                    client = _listenSocket.Accept();
                }
                catch (Exception)
                {
                    break;
                }

                if (client == null)
                    continue;

                if (!OnClientConnected((client.RemoteEndPoint as IPEndPoint).Address))
                {
                    client.Close();
                    continue;
                }

                CreateWorkerProcess(client);
            }
        }

        private bool OnClientConnected(IPAddress address)
        {
            ClientConnectedEventHandler handler = ClientConnected;
            
            bool res = true;
            
            if (handler != null)
                res = handler(this, new ClientConnectedEventArgs(address));

            return res;
        }

        internal void OnLogAccess(LogAccess data)
        {
            LogAccessEventHandler handler = LogAccess;

            if (handler != null)
                handler(this, new LogAccessEventArgs(data));
        }

        private void CreateWorkerProcess(Socket client)
        {
            int workerCount;

            while (!_stopThreads)        // TODO: add timeout
            {
                lock (_workerThreads)
                {
                    workerCount = _workerThreads.Count;
                }

                if (workerCount < _maxWorkers)
                    break;

                Thread.Sleep(10);
            }

            HttpProcessor pcr = new HttpProcessor(ref client, _httpHandler, this);

            Thread thd = new Thread(new ThreadStart(pcr.ProcessRequest));

            thd.Start();

            lock (_workerThreads)
            {
                _workerThreads.Add(thd);
            }
        }

        private void RemoveWorkerThreads()
        {
            while (!_stopThreads)
            {
                lock (_workerThreads)
                {
                    if (_workerThreads.Count > 0)
                    {
                        for (int i = _workerThreads.Count - 1; i >= 0; i--)
                        {
                            if (((Thread)_workerThreads[i]).ThreadState == ThreadState.Stopped)
                            {
                                 _workerThreads.RemoveAt(i);
                            }
                        }
                    }
#if(LOG && !MF && !WindowsCE)
                    if (_workerThreads.Count > 0)
                        Console.WriteLine(_workerThreads.Count + " worker threads (" + Address + ":" + Port + ")");
#elif(LOG && MF)
                    if (_workerThreads.Count > 0)
                        Microsoft.SPOT.Debug.Print(_workerThreads.Count + " worker threads (" + Address + ":" + Port + ")");
#endif
                }

                Thread.Sleep(300);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Stop();
        }

        #endregion
    }
}
