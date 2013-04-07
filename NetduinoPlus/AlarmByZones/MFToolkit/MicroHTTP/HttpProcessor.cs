/* 
 * HttpProcessor.cs
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
 * MS	08-03-24	initial version
 * MS   09-02-10    fixed keep-alive support
 * MS   09-03-09    changed how the request and response is handled
 * MS   09-06-19    added support for SSL (now using Stream instead of Socket)
 * 
 * 
 */
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Threading;
using System.Text;
using Socket = System.Net.Sockets.Socket;
using System.Diagnostics;
using System.IO;

#if(SSL)
#if(!MF)
using System.Net.Security;
using System.Security.Authentication;
#else
using Microsoft.SPOT.Net.Security;
#endif
#endif

namespace MFToolkit.Net.Web
{
    internal sealed class HttpProcessor
    {
        private Socket _client;
        private IHttpHandler _handler;
        private HttpServer _server;

        public HttpProcessor(ref Socket Client, IHttpHandler Handler, HttpServer Server)
        {
            _client = Client;
            _handler = Handler;
            _server = Server;
        }

        private void Close()
        {
            if (_client != null)
            {
                _client.Close();
                _client = null;
            }
        }

        internal void ProcessRequest()
        {
#if(LOG && !MF && !WindowsCE)
            Console.WriteLine((_client.RemoteEndPoint as IPEndPoint).ToString());
#endif

            using (_client)
            {
                while (true)
                {

                    #region Wait for first byte (used for keep-alive, too)

                    int avail = 0;

                    DateTime maxWait = DateTime.Now.AddMilliseconds(2000);
                    do
                    {
                        try
                        {
                            avail = _client.Available;

                            if (avail == 0)
                                Thread.Sleep(10);
                        }
                        catch
                        {
                            break;
                        }
                    }
                    while (avail == 0 && DateTime.Now <= maxWait);

                    #endregion

                    if (avail == 0)
                        break;

                    DateTime begin = DateTime.Now;

                    HttpRequest httpRequest = new HttpRequest();
                    HttpResponse httpResponse = null;

                    Stream stream;

#if(SSL)
                    if (_server.IsSecure && _server.Certificate != null)
                    {
                        SslStream ssl = null;

                        try
                        {
#if(!MF)
                            ssl = new SslStream(new NetworkStream(_client));
                            ssl.AuthenticateAsServer(_server.Certificate, false, SslProtocols.Default, false);
#else
                            ssl = new SslStream(_client);
                            ssl.AuthenticateAsServer(_server.Certificate, SslVerification.NoVerification, SslProtocols.Default);
#endif
                            stream = ssl;
                        }
                        catch (Exception)
                        {
                            Close();
                            return;
                        }
                    }
                    else
#endif
                    {
                        stream = new NetworkStream(_client);
                    }

                    stream.ReadTimeout = 200;
                    stream.WriteTimeout = 1000;

                    try
                    {
                        if (!httpRequest.Read(stream, (_client.RemoteEndPoint as IPEndPoint)))
                        {
                            httpResponse = new HttpResponse();
                            httpResponse.RaiseError(HttpStatusCode.ServiceUnavailable);
                            httpResponse.AddHeader("Connection", "close");
                        }
                    }
                    catch (HttpException ex)
                    {
                        httpResponse = new HttpResponse();
                        httpResponse.RaiseError(ex.Message, ex.Code);
                        httpResponse.AddHeader("Connection", "close");
                    }
                    catch (Exception)
                    {
                        httpResponse = new HttpResponse();
                        httpResponse.RaiseError();
                        httpResponse.AddHeader("Connection", "close");
                    }

                    if (httpResponse == null)
                    {
                        httpResponse = new HttpResponse();
                        httpResponse.HttpVersion = httpRequest.HttpVersion;

                        HttpContext ctx = new HttpContext();
                        ctx.Request = httpRequest;
                        ctx.Response = httpResponse;

                        try
                        {
                            _handler.ProcessRequest(ctx);
                        }
                        catch (HttpException ex)
                        {
                            httpResponse = new HttpResponse();
                            httpResponse.RaiseError(ex.Message, ex.Code);
                            httpResponse.AddHeader("Connection", "close");
                        }
                        catch (Exception)
                        {
                            httpResponse = new HttpResponse();
                            httpResponse.RaiseError();
                            httpResponse.AddHeader("Connection", "close");
                        }
                    }

                    httpResponse.Write(stream);

                    stream.Flush();

                    LogAccess log = new LogAccess();
                    log.ClientIP = httpRequest.UserHostAddress;
                    log.BytesReceived = httpRequest.totalBytes;
                    log.BytesSent = httpResponse.totalBytes;
                    log.Date = begin;
                    log.Method = httpRequest.HttpMethod;
                    log.RawUrl = httpRequest.RawUrl;
                    log.UserAgent = httpRequest.UserAgent;
                    log.HttpReferer = httpRequest.Referer;

                    log.Duration = (DateTime.Now.Ticks - begin.Ticks) / TimeSpan.TicksPerMillisecond;


                    _server.OnLogAccess(log);

                    if (httpResponse.Connection == null || httpResponse.Connection != "Keep-Alive")
                        break;

                    Thread.Sleep(15);
                }

                Close();
            }
        }
    }
}
