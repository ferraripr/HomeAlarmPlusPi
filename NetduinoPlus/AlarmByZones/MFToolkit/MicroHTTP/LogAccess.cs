/* 
 * LogAccess.cs
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

namespace MFToolkit.Net.Web
{
    public struct LogAccess
    {
        /// <summary>
        /// Date and time
        /// </summary>
        public DateTime Date;

        /// <summary>
        /// s-ip
        /// </summary>
        public IPAddress ServerIP;

        /// <summary>
        /// cs-method
        /// </summary>
        public string Method;

        /// <summary>
        /// cs-uri-stem
        /// </summary>
        public string RawUrl;

        /// <summary>
        /// cs-uri-query
        /// </summary>
        public string RequestString;

        /// <summary>
        /// s-port
        /// </summary>
        public int Port;

        /// <summary>
        /// cs-username
        /// </summary>
        public string Username;

        /// <summary>
        /// c-ip
        /// </summary>
        public string ClientIP;

        /// <summary>
        /// cs(User-Agent)
        /// </summary>
        public string UserAgent;

        /// <summary>
        /// cs(Referer)
        /// </summary>
        public string HttpReferer;

        /// <summary>
        /// sc-status
        /// </summary>
        public int Status;

        /// <summary>
        /// sc-substatus
        /// </summary>
        public int SubStatus;

        /// <summary>
        /// sc-bytes
        /// </summary>
        public long BytesSent;

        /// <summary>
        /// cs-bytes
        /// </summary>
        public long BytesReceived;

        /// <summary>
        /// time-taken in milli seconds
        /// </summary>
        public long Duration;
    }
}
