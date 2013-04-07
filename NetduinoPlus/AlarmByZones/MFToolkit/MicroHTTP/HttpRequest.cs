/* 
 * HttpRequest.cs
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
 * MS   09-02-10    added GetHeaderValue
 * MS   09-03-09    changed that HttpRequest is reading the request instead of ProcessClientRequest
 * MS   09-06-19    added support for SSL (now using Stream instead of Socket)
 *                  changed to speed up http request reading (when finished return)
 * 
 */
using System;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Net;
#if(MF)
using MFToolkit.Collection.Spezialized;
#else
//using System.Collections.Specialized;
using System.Security.Cryptography.X509Certificates;
//using System.Net.Security;
//using System.Security.Authentication;
#endif

namespace MFToolkit.Net.Web
{
    /// <summary>
    /// Class that represents a http request.
    /// </summary>
    public class HttpRequest
    {
        private string _httpMethod;
        private string _rawUrl;
        private string _httpVersion;

        private string _path;
        private string _queryString;

        private string _userHostAddress;

        private MFToolkit.Collection.Spezialized.NameValueCollection _headers = null;
        private HttpCookieCollection _cookies = null;
        private MFToolkit.Collection.Spezialized.NameValueCollection _params = null;
        private MFToolkit.Collection.Spezialized.NameValueCollection _form = null;
        private MimeContentCollection _mime = null;

        private byte[] _body = null;

        internal long totalBytes = 0;

        private const long MAX_CONTENT_LENGTH = 100 * 1024;
        private const long MAX_REQUEST_TIMEOUT = 20 * 1000;     // 20 seconds
        private const long MAX_REQUEST_SLIDING_TIMEOUT = 2 * 1000;     // 1 seconds

        public HttpRequest()
        {
            _headers = new MFToolkit.Collection.Spezialized.NameValueCollection();
        }

        #region Public Properties

        /// <summary>
        /// Gets the value of Form, Params or Headers key.
        /// </summary>
        /// <param name="name">The key to get.</param>
        /// <returns>Returns the value of Form, Params or Headers key; if not found it returns null.</returns>
        public string this[string name]
        {
            get
            {
                if (_form != null && _form[name] != null)
                    return _form[name];

                if (_params != null && _params[name] != null)
                    return _params[name];

                if (_headers != null && _headers[name] != null)
                    return _headers[name];

                return null;
            }
        }

        /// <summary>
        /// Gets the http query key/value pairs.
        /// </summary>
        public MFToolkit.Collection.Spezialized.NameValueCollection Params
        {
            get { return _params; }
            internal set { _params = value; }
        }

        /// <summary>
        /// Gets the http header key/value pairs.
        /// </summary>
        public MFToolkit.Collection.Spezialized.NameValueCollection Headers
        {
            get { return _headers; }
            internal set { _headers = value; }
        }

        /// <summary>
        /// Gets the http form key/value pairs.
        /// </summary>
        public MFToolkit.Collection.Spezialized.NameValueCollection Form
        {
            get { return _form; }
            internal set { _form = value; }
        }

        /// <summary>
        /// Gets the http mime content if any.
        /// </summary>
        public MimeContentCollection MimeContent
        {
            get { return _mime; }
            internal set { _mime = value; }
        }

        /// <summary>
        /// Gets the http cookie key/value pairs.
        /// </summary>
        public HttpCookieCollection Cookies
        {
            get
            {
                if (_cookies == null)
                {
                    _cookies = new HttpCookieCollection();

                    string cookies = Headers["Cookie"];

                    if (cookies == null || cookies.Length == 0)
                        return _cookies;


                    foreach (string cookieStr in cookies.Split(';'))
                    {
                        string[] cookieParts = cookieStr.Split('=');

                        if (cookieParts.Length != 2)
                            continue;

                        _cookies.Add(cookieParts[0], new HttpCookie(cookieParts[0], cookieParts[1]));
                    }
                }

                return _cookies;
            }
            internal set
            {
                _cookies = value;
            }
        }

        /// <summary>
        /// Gets the http body if using POST.
        /// </summary>
        public byte[] Body
        {
            get
            {
                return _body;
            }
        }

        /// <summary>
        /// Gets the http header Host value; otherwise returns blank.
        /// </summary>
        public string Host
        {
            get
            {
                return (Headers["Host"] != null ? Headers["Host"] : "");
            }
        }

        /// <summary>
        /// Gets the http header Connection value; otherwise returns blank.
        /// </summary>
        public string Connection
        {
            get
            {
                    return (Headers["Connection"] != null ? Headers["Connection"] : "");
            }
        }

        /// <summary>
        /// Gets the http header UserAgent value; otherwise returns blank.
        /// </summary>
        public string UserAgent
        {
            get
            {
                return (Headers["User-Agent"] != null ? Headers["User-Agent"] : "");
            }
        }

        /// <summary>
        /// Gets the http header Referer value; otherwise returns blank.
        /// </summary>
        public string Referer
        {
            get
            {
                return (Headers["Referer"] != null ? Headers["Referer"] : "");
            }
        }

        /// <summary>
        /// Gets the http header Accept value; otherwise returns null.
        /// </summary>
        public string[] AcceptTypes
        {
            get
            {
                string accept = Headers["Accept"];

                if (accept == null)
                    return null;

                string[] acceptTypes = accept.Split(',');

                for (int i = 0; i < acceptTypes.Length; i++)
                {
                    acceptTypes[i].Trim();
                }

                return acceptTypes;
            }
        }

        /// <summary>
        /// Gets the content length if using POST; otherwise 0.
        /// </summary>
        public int ContentLength
        {
            get
            {
                if (Headers["Content-Length"] != null)
                    return int.Parse(Headers["Content-Length"]);

                return 0;
            }
        }

        /// <summary>
        /// Gets the http content type value; otherwise returns blank.
        /// </summary>
        public string ContentType
        {
            get
            {
                if (Headers["Content-Type"] != null)
                    return Headers["Content-Type"];

                if (Headers["Content-type"] != null)
                    return Headers["Content-type"];

                if (Headers["content-type"] != null)
                    return Headers["content-type"];

                return "";
            }
        }

        /// <summary>
        /// Gets the http method value.
        /// </summary>
        public string HttpMethod
        {
            get
            {
                return _httpMethod;
            }
            internal set
            {
                _httpMethod = value;
            }
        }

        /// <summary>
        /// Gets the http raw url.
        /// </summary>
        public string RawUrl
        {
            get
            {
                return _rawUrl;
            }
            internal set
            {
                _rawUrl = value;
            }
        }

        /// <summary>
        /// Gets the http version.
        /// </summary>
        public string HttpVersion
        {
            get
            {
                return _httpVersion;
            }
            internal set
            {
                _httpVersion = value;
            }
        }

        public string Path
        {
            get
            {
                if (_path == null)
                    return RawUrl;

                return _path;
            }
            internal set
            {
                _path = value;
            }
        }

        /// <summary>
        /// Gets the http query string.
        /// </summary>
        public string QueryString
        {
            get
            {
                return _queryString;
            }
            internal set
            {
                _queryString = value;
            }
        }

        /// <summary>
        /// Gets the users client address.
        /// </summary>
        public string UserHostAddress
        {
            get
            {
                return _userHostAddress;
            }
            internal set
            {
                _userHostAddress = value;
            }
        }

        #endregion

        internal bool RaiseError()
        {
            return false;
        }
        
        internal bool Read(Stream stream, IPEndPoint endPoint)
        {
            byte[] buffer = new byte[1024];
            RequestParserState state = RequestParserState.ReadMethod;
            string key = "";
            string value = "";
            MemoryStream ms = null;

            DateTime begin = DateTime.Now;
            DateTime lastByteReceived = begin;

            if (endPoint != null)
                UserHostAddress = endPoint.Address.ToString();

            while (true)
            {
                if (state == RequestParserState.ReadDone)
                    return true;

                int bytesRead = 0;
                int idx = 0;

#if(MF)
                // set all bytes to null byte (strings are ending with null byte in MF)
                Array.Clear(buffer, 0, buffer.Length);
#endif

                try
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);

                    if (bytesRead > 0)
                        lastByteReceived = DateTime.Now;
                }
                catch (IOException)
                {
                    break;
                }
                catch (Exception)
                {
                    DateTime nd = DateTime.Now;
#if(MF)
                    if((nd.Ticks - lastByteReceived.Ticks) / TimeSpan.TicksPerMillisecond > MAX_REQUEST_SLIDING_TIMEOUT)
                        break;
#else
                 //   if ((nd - lastByteReceived).TotalMilliseconds > MAX_REQUEST_SLIDING_TIMEOUT)
                 //       break;
#endif

                    if (HttpMethod == "POST" && (_body == null || _body.Length < ContentLength))
                        continue;

#if(MF)
                    if((nd.Ticks - begin.Ticks) / TimeSpan.TicksPerMillisecond < MAX_REQUEST_TIMEOUT)
                        continue;
#else
                   // if ((nd - begin).TotalMilliseconds < MAX_REQUEST_TIMEOUT)
                   //     continue;
#endif

                    break;
                }

                if (bytesRead == 0)     // should never happen
                    break;

                totalBytes += bytesRead;

#if(FILELOG && !MF && !WindowsCE)
                File.AppendAllText("loghttp-" + socket.RemoteEndPoint.ToString().Replace(":", "-") + ".txt", Encoding.UTF8.GetString(buffer, 0, bytesRead) + "\r\n");
#endif

                if (totalBytes <= idx)
                    continue;

                do
                {
                    switch (state)
                    {
                        case RequestParserState.ReadMethod:
                            if (buffer[idx] != ' ')
                                HttpMethod += (char)buffer[idx++];
                            else
                            {
                                // TODO: add a allowed methods list

                                //if (HttpMethod != "POST" && HttpMethod != "GET" && HttpMethod != "OPTIONS")
                                //    throw new HttpException(HttpStatusCode.MethodNotAllowed);

                                idx++;
                                state = RequestParserState.ReadUrl;
                            }
                            break;

                        case RequestParserState.ReadUrl:
                            if (buffer[idx] == '?')
                            {
                                idx++;
                                key = "";
                                _params = new MFToolkit.Collection.Spezialized.NameValueCollection();
                                state = RequestParserState.ReadParamKey;
                            }
                            else if (buffer[idx] != ' ')
                            {
                                RawUrl += (char)buffer[idx++];
                            }
                            else
                            {
                                idx++;
                                RawUrl = HttpServerUtility.UrlDecode(RawUrl);
                                state = RequestParserState.ReadVersion;
                            }
                            break;

                        case RequestParserState.ReadParamKey:
                            if (buffer[idx] == '=')
                            {
                                idx++;
                                value = "";
                                state = RequestParserState.ReadParamValue;
                            }
                            else if (buffer[idx] == ' ')
                            {
                                idx++;
                                RawUrl = HttpServerUtility.UrlDecode(RawUrl);
                                state = RequestParserState.ReadVersion;
                            }
                            else
                            {
                                key += (char)buffer[idx++];
                            }
                            break;

                        case RequestParserState.ReadParamValue:
                            if (buffer[idx] == '&')
                            {
                                idx++;
                                key = HttpServerUtility.UrlDecode(key);
                                value = HttpServerUtility.UrlDecode(value);

                                Params[key] = (Params[key] != null ? Params[key] + ", " + value : value);

                                key = "";
                                value = "";

                                state = RequestParserState.ReadParamKey;
                            }
                            else if (buffer[idx] == ' ')
                            {
                                idx++;
                                key = HttpServerUtility.UrlDecode(key);
                                value = HttpServerUtility.UrlDecode(value);

                                Params[key] = (Params[key] != null ? Params[key] + ", " + value : value);

                                RawUrl = HttpServerUtility.UrlDecode(RawUrl);

                                state = RequestParserState.ReadVersion;
                            }
                            else
                            {
                                value += (char)buffer[idx++];
                            }
                            break;

                        case RequestParserState.ReadVersion:
                            if (buffer[idx] == '\r')
                                idx++;
                            else if (buffer[idx] != '\n')
                                HttpVersion += (char)buffer[idx++];
                            else
                            {
                                if (HttpVersion != "HTTP/1.1" && HttpVersion != "HTTP/1.0")
                                    throw new HttpException(HttpStatusCode.HttpVersionNotSupported);

                                idx++;
                                key = "";
                                Headers = new MFToolkit.Collection.Spezialized.NameValueCollection();
                                state = RequestParserState.ReadHeaderKey;
                            }
                            break;

                        case RequestParserState.ReadHeaderKey:
                            if (buffer[idx] == '\r')
                                idx++;
                            else if (buffer[idx] == '\n')
                            {
                                idx++;
                                if (HttpMethod == "POST")
                                    state = RequestParserState.ReadBody;
                                else
                                {
                                    state = RequestParserState.ReadDone;    // well, we don't really need this
                                    return true;
                                }
                            }
                            else if (buffer[idx] == ':')
                                idx++;
                            else if (buffer[idx] != ' ')
                                key += (char)buffer[idx++];
                            else
                            {
                                idx++;
                                value = "";
                                state = RequestParserState.ReadHeaderValue;
                            }
                            break;

                        case RequestParserState.ReadHeaderValue:
                            if (buffer[idx] == '\r')
                                idx++;
                            else if (buffer[idx] != '\n')
                                value += (char)buffer[idx++];
                            else
                            {
                                idx++;
                                Headers.Add(key, value);
                                key = "";
                                state = RequestParserState.ReadHeaderKey;
                            }
                            break;

                        case RequestParserState.ReadBody:

                            if (ContentLength > MAX_CONTENT_LENGTH)
                            {
                                // TODO: how can I stop the client to cancel http request
                                //throw new HttpException(HttpStatusCode.RequestEntitiyTooLarge);
                            }

                            if (ms == null)
                                ms = new MemoryStream();

                            ms.Write(buffer, idx, bytesRead - idx);
                            idx = bytesRead;
                            
                            if (ms.Length >= ContentLength)
                            {
                                _body = ms.ToArray();

                                // if using a <form/> tag with POST check if it is urlencoded or multipart boundary

                                if (ContentType.IndexOf("application/x-www-form-urlencoded") != -1)
                                {
                                    _form = new MFToolkit.Collection.Spezialized.NameValueCollection();
                                    key = "";
                                    value = null;

                                    for (int i = 0; i < _body.Length; i++)
                                    {
                                        if (_body[i] == '=')
                                            value = "";
                                        else if (_body[i] == '&')
                                        {
                                            _form.Add(key, value != null ? HttpServerUtility.UrlDecode(value) : "");
                                            key = "";
                                            value = null;
                                        }
                                        else if (value == null)
                                            key += (char)_body[i];
                                        else if (value != null)
                                            value += (char)_body[i];
                                    }

                                    if (key != null && key.Length > 0)
                                    {
                                        _form.Add(key, value != null ? HttpServerUtility.UrlDecode(value) : "");
                                    }
                                }
                                else if (ContentType != null && ContentType.Length > "multipart/form-data; boundary=".Length && ContentType.Substring(0, "multipart/form-data; boundary=".Length) == "multipart/form-data; boundary=")
                                {
                                    string boundary = ContentType.Substring("multipart/form-data; boundary=".Length);

                                    _mime = new MimeContentCollection();

                                    MimeParser mp = new MimeParser(_body, boundary);

                                    MimeContent mime = mp.GetNextContent();
                                    while (mime != null)
                                    {
                                        _mime.Add(mime.Name, mime);

                                        if (mime.Headers["Content-Disposition"] != null && mime.Headers["Content-Disposition"].IndexOf("form-data") >= 0)
                                        {
                                            if (_form == null)
                                                _form = new MFToolkit.Collection.Spezialized.NameValueCollection();

                                            _form.Add(mime.Name, (mime.Content != null && mime.Content.Length > 0 ? new string(Encoding.UTF8.GetChars(mime.Content)) : ""));
                                        }

                                        mime = mp.GetNextContent();
                                    }
                                }
                                state = RequestParserState.ReadDone;        // well, we don't really need this
                                return true;
                            }
                            break;

                        case RequestParserState.ReadDone:                   // well, we don't really need this
                            return true;

                        default:
                            //idx++;
                            break;

                    }
                }
                while (idx < bytesRead);
            }

            return false;
        }
    }
}