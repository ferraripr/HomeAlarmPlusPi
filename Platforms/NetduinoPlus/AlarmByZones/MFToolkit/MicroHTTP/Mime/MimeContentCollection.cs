/* 
 * MimeContentCollection.cs
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
    public class MimeContentCollection : ICollection
    {
        private ArrayList _mime = new ArrayList();
        private object _syncRoot = new object();

        public MimeContent this[int index]
        {
            get { return Get(index); }
        }

        public MimeContent this[string name]
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

        public void Add(string name, MimeContent value)
        {
            AddWithoutValidate(name, value);
        }

        protected void AddWithoutValidate(string name, MimeContent value)
        {
            _mime.Add(value);
        }

        public void Clear()
        {
            _mime.Clear();
        }

        public MimeContent Get(int index)
        {
            return ((MimeContent)_mime[index]);
        }

        public MimeContent Get(string name)
        {
            for (int i = 0; i < Count; i++)
            {
                MimeContent mime = (MimeContent)_mime[i];
                if (mime.Name == name)
                {
                    return mime;
                }
            }
            return null;
        }

        public string GetKey(int index)
        {
            return ((MimeContent)_mime[index]).Name;
        }

        public void Remove(string name)
        {
            for (int i = 0; i < Count; i++)
            {
                MimeContent mime = (MimeContent)_mime[i];
                if (mime.Name == name)
                {
                    _mime.RemoveAt(i);
                    break;
                }
            }
        }

        public void Set(string name, MimeContent value)
        {
            for (int i = 0; i < Count; i++)
            {
                MimeContent mime = (MimeContent)_mime[i];
                if (mime.Name == name)
                {
                    _mime[i] = mime;
                    return;
                }
            }
            Add(name, value);
        }

        private ArrayList GetKeys()
        {
            ArrayList list = new ArrayList();
            foreach (MimeContent mime in _mime)
            {
                list.Add(mime.Name);
            }
            return list;
        }

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            _mime.CopyTo(array, index);
        }

        public int Count
        {
            get { return _mime.Count; }
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
            return new MimeContentEnumerator(this);
        }

        #endregion
    }

}