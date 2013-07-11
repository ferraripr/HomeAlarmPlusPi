/* 
 * ClientConnectedEvent.cs
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
 * 
 * 
 */
using System;
using System.Net;
#if(MF)
using Microsoft.SPOT;
#endif

namespace MFToolkit.Net.Web
{
    public class ClientConnectedEventArgs : Microsoft.SPOT.EventArgs
    {
        private IPAddress _address;

        #region Public Properties

        /// <summary>
        /// The client IP address
        /// </summary>
        public IPAddress RemoteHost
        {
            get { return _address; }
            internal set { _address = value; }
        }

        #endregion

        public ClientConnectedEventArgs(IPAddress address)
        {
            RemoteHost = address;
        }
    }
    public delegate void LogEventHandler(LogEventType ev, string text);
    public delegate void LogAccessHandler(LogAccess data);


    public delegate bool ClientConnectedEventHandler(object sender, ClientConnectedEventArgs e);
}
