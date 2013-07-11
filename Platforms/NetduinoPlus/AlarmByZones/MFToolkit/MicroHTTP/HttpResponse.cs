/* 
 * HttpResponse.cs
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
 * MS   09-02-10    added http headers
 *                  added http redirect
 * MS   09-02-26    fixed wrong date output
 *                  added remove http header
 * MS   09-03-09    changed that http response is written by HttpResponse
 * MS   09-04-30    fixed if socket is already closed
 * MS   09-06-19    added support for SSL (now using Stream instead of Socket)
 *                  
 * 
 */
using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Net.Sockets;

#if(MF)
using MFToolkit.Text;
#endif

namespace MFToolkit.Net.Web
{
    public class HttpResponse
    {
        private MemoryStream _content;

        private string _httpVersion = "HTTP/1.1";
        private HttpStatusCode _httpStatus = HttpStatusCode.OK;

        private HttpHeader[] _headers = null;
        private ArrayList _cookies = null;

        private DateTime _date = DateTime.Now;

        internal long totalBytes = 0;

        public HttpResponse()
        {
            _content = new MemoryStream();

            //AddHeader("Cache-Control", "no-cache");
            //AddHeader("Pragma", "no-cache");
            //AddHeader("Expires", "-1");

            AddHeader("Content-Type", "text/plain; charset=utf-8");
            AddHeader("Server", "MSchwarz HTTP Server");
        }

        #region Public Properties

        /// <summary>
        /// Sets the HTTP status code
        /// </summary>
        public HttpStatusCode HttpStatus
        {
            set { _httpStatus = value; }
        }        

        /// <summary>
        /// Sets the HTTP response version
        /// </summary>
        public string HttpVersion
        {
            set { _httpVersion = value; }
        }

        /// <summary>
        /// Sets the HTTP response content type
        /// </summary>
        public string ContentType
        {
            set { AddHeader("Content-Type", value); }
        }

        /// <summary>
        /// Sets the HTTP response Connection header value 
        /// </summary>
        public string Connection
        {
            set { AddHeader("Connection", value); }
            internal get { return GetHeader("Connection"); }
        }

        /// <summary>
        /// Sets the HTTP response Date header value
        /// </summary>
        public DateTime Date
        {
            set { _date = value; }
            internal get { return _date; }
        }

        #endregion

        #region Http header related methods

        public void RemoveHeader(string name)
        {
            if (_headers == null)
                return;

            ArrayList lis = new ArrayList();

            foreach (HttpHeader header in _headers)
                if(header.Name != name)
                    lis.Add(header);

            _headers = new HttpHeader[lis.Count];
            for (int i = 0; i < _headers.Length; i++)
                _headers[i] = lis[i] as HttpHeader;
        }

        public void AddHeader(string name, string value)
        {
            AddHeader(name, value, true);
        }

        public void AddHeader(string name, string value, bool replace)
        {
            if (replace && _headers != null)
            {
                foreach (HttpHeader header in _headers)
                {
                    if (header.Name == name)
                    {
                        header.Value = value;
                        return;
                    }
                }
            }

            ArrayList lis = new ArrayList();

            if (_headers != null)
            {
                foreach (HttpHeader header in _headers)
                    lis.Add(header);
            }

            lis.Add(new HttpHeader(name, value));

            _headers = new HttpHeader[lis.Count];
            for (int i = 0; i < _headers.Length; i++)
                _headers[i] = lis[i] as HttpHeader;
        }

        private string GetHeader(string name)
        {
            if(_headers == null)
                return null;

            for(int i=0; i<_headers.Length; i++)
                if(_headers[i].Name == name)
                    return _headers[i].Value;

            return null;
        }

        #endregion

        #region Cookie related methods

        public void SetCookie(HttpCookie cookie)
        {
            if (_cookies == null)
                _cookies = new ArrayList();

            _cookies.Add(cookie);
        }

        #endregion

        public void Redirect(string uri)
        {
            _httpStatus = HttpStatusCode.MovedPermanently;

            Clear();

            AddHeader("Location", uri);

            Write(@"<html><head><title>Object moved</title></head><body>
<h2>Object moved to <a href=""" + uri + @""">here</a>.</h2>
</body></html>");
        }

        public void RaiseError()
        {
            RaiseError(null);
        }

        public void RaiseError(HttpStatusCode status)
        {
            RaiseError(null, status);
        }

        public void RaiseError(string details)
        {
            RaiseError(details, HttpStatusCode.InternalServerError);
        }

        public void RaiseError(string details, HttpStatusCode status)
        {
            Clear();

            _httpStatus = status;

            ContentType = "text/html; charset=UTF-8";

            Write(@"<html><head><title>Error</title></head><body>
<h2>" + (int)status + " " + HttpStatusHelper.GetHttpStatusFromCode(status) + @"</h2>
" + (details != null && details.Length > 0 ? "<p>" + details + "</p>": "") + @"
</body></html>");
        }

        public void Clear()
        {
            _content = new MemoryStream();
        }

        public void Write(string s)
        {
            byte[] b = Encoding.UTF8.GetBytes(s);
            Write(b);
        }

        public void Write(byte[] bytes)
        {
            _content.Write(bytes, 0, bytes.Length);
        }

        public void WriteLine(string line)
        {
            Write(line);
            Write("\r\n");
        }

        internal string GetResponseHeader()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            string response = _httpVersion + " " + (int)_httpStatus + " " + HttpStatusHelper.GetHttpStatusFromCode(_httpStatus) + "\r\n";

            AddHeader("Date", _date.ToString("ddd, dd MMM yyyy HH':'mm':'ss 'GMT'"));

            AddHeader("Content-Length", (_content != null ? _content.Length.ToString() : "0"));

            AddHeader("X-Powered-By", "MSchwarz HTTP Server");

            if (_cookies != null)
            {
                for (int i = 0; i < _cookies.Count; i++)
                {
                    HttpCookie cookie = _cookies[i] as HttpCookie;

                    if (cookie != null)
                        AddHeader("Set-Cookie", cookie.ToString(), false);
                }
            }

            
            if (_headers != null && _headers.Length > 0)
            {
                for (int i = 0; i < _headers.Length; i++)
                {
                    response += _headers[i].Name + ": " + _headers[i].Value + "\r\n";
                }
            }

            response += "\r\n";

            return response;
        }

        public void Write(Stream stream)
        {
#if(FILELOG && !MF && !WindowsCE)
            File.AppendAllText("loghttp-" + socket.RemoteEndPoint.ToString().Replace(":", "-") + " (Response).txt", GetResponseHeader() + "\r\n");
#endif

            byte[] bytes = Encoding.UTF8.GetBytes(GetResponseHeader());
            totalBytes += bytes.Length;

            try
            {
                stream.Write(bytes, 0, bytes.Length);
                //stream.Flush();

                if (_content != null && _content.Length > 0)
                {

                    bytes = _content.ToArray();
                    totalBytes += bytes.Length;

#if(FILELOG && !MF && !WindowsCE)
                    File.AppendAllText("loghttp-" + socket.RemoteEndPoint.ToString().Replace(":", "-") + " (Response).txt", Encoding.UTF8.GetString(bytes, 0, bytes.Length) + "\r\n");
#endif

                    stream.Write(bytes, 0, bytes.Length);
                    //stream.Flush();
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
