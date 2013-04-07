/* 
 * HttpCookieCollection.cs
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
 * MS   09-03-09    initial version
 * 
 * 
 */
using System;
using System.Collections;

namespace MFToolkit.Net.Web
{
    public class HttpCookieCollection : ICollection
    {
        private ArrayList _cookies = new ArrayList();
        private object _syncRoot = new object();

        public HttpCookie this[int index]
        {
            get { return Get(index); }
        }

        public HttpCookie this[string name]
        {
            get { return Get(name); }
            set { Set(name, value); }
        }

        public string[] AllKeys
        {
            get { return (string[])GetKeys().ToArray(typeof(string)); }
        }

        public ArrayList Keys
        {
            get { return GetKeys(); }
        }

        public void Add(string name, HttpCookie value)
        {
            AddWithoutValidate(name, value);
        }

        protected void AddWithoutValidate(string name, HttpCookie value)
        {
            _cookies.Add(value);
        }

        public void Clear()
        {
            _cookies.Clear();
        }

        public HttpCookie Get(int index)
        {
            return ((HttpCookie)_cookies[index]);
        }

        public HttpCookie Get(string name)
        {
            for (int i = 0; i < Count; i++)
            {
                HttpCookie cookie = (HttpCookie)_cookies[i];
                if (cookie.Name == name)
                {
                    return cookie;
                }
            }
            return null;
        }

        public string GetKey(int index)
        {
            return ((HttpCookie)_cookies[index]).Name;
        }

        public void Remove(string name)
        {
            for (int i = 0; i < Count; i++)
            {
                HttpCookie cookie = (HttpCookie)_cookies[i];
                if (cookie.Name == name)
                {
                    _cookies.RemoveAt(i);
                    break;
                }
            }
        }

        public void Set(string name, HttpCookie value)
        {
            for (int i = 0; i < Count; i++)
            {
                HttpCookie cookie = (HttpCookie)_cookies[i];
                if (cookie.Name == name)
                {
                    _cookies[i] = value;
                    return;
                }
            }
            Add(name, value);
        }

        private ArrayList GetKeys()
        {
            ArrayList list = new ArrayList();
            foreach (HttpCookie cookie in _cookies)
            {
                list.Add(cookie.Name);
            }
            return list;
        }

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            _cookies.CopyTo(array, index);
        }

        public int Count
        {
            get { return _cookies.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return _syncRoot; }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return new HttpCookieEnumerator(this);
        }

        #endregion
    }

}