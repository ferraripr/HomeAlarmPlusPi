/* 
 * HttpCookie.cs
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
 * MS   09-03-09    fixed MF support
 * 
 * 
 */
using System;
using System.Collections;

namespace MFToolkit.Net.Web
{
    public class HttpCookie
    {
        private string _name;
        private string _value;
        private string _path = "/";
        private string _domain;
        
        private DateTime _expires;
        private bool _expiresSet = false;

        private bool _secure;
        private bool _httpOnly;

        public HttpCookie()
        {

        }

        public HttpCookie(string name, string value)
            : base()
        {
            _name = name;
            _value = value;
        }

        #region Public Properties

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        public string Domain
        {
            get { return _domain; }
            set { _domain = value; }
        }

        public DateTime Expires
        {
            get { return _expires; }
            set
            {
                _expires = value;
                _expiresSet = true;
            }
        }

        public bool Secure
        {
            get { return _secure; }
            set { _secure = value; }
        }

        public bool HttpOnly
        {
            get { return _httpOnly; }
            set { _httpOnly = value; }
        }

        #endregion

        public static HttpCookie[] CookiesFromHeader(string httpHeader)
        {
            if (httpHeader == null)
                return null;

            ArrayList cookies = new ArrayList();

            foreach (string cookieStr in httpHeader.Split(';'))
            {
                string[] cookieParts = cookieStr.Split('=');

                if(cookieParts.Length != 2)
                    continue;

                HttpCookie cookie = new HttpCookie();
                cookie.Name = cookieParts[0];
                cookie.Value = cookieParts[1];

                cookies.Add(cookie);
            }

            HttpCookie[] res = new HttpCookie[cookies.Count];

            for(int i=0; i<cookies.Count; i++) {
                res[i] = cookies[i] as HttpCookie;
            }

            return res;
        }

        public override string ToString()
        {
            // Set-Cookie: RMID=732423sdfs73242; expires=Fri, 31-Dec-2010 23:59:59 GMT; path=/; domain=.example.net; HttpOnly

            return _name + "=" + _value + "; "
                + (_expiresSet ? "expires=" + 
                
#if(MF)
                _expires.ToString("ddd, dd'-'MMM'-'yyyy HH':'mm':'ss 'GMT'")
#else
                _expires.ToString("r")
#endif
                
                + "; " : "")
                + (_path != null ? "path=" + _path + "; " : "")
                + (_domain != null ? "domain=" + _domain + "; " : "")
                + (_httpOnly ? "HttpOnly" : "");
        }
    }
}
