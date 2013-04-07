/* 
 * NameValueCollection.cs
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
 * MS   09-03-09    initial version of a simple NameValue collection
 * 
 *
 */
using System;
using System.Collections;

namespace MFToolkit.Collection.Spezialized
{
    public class NameValueCollection : ICollection
    {
        private ArrayList _pairs = new ArrayList();
        private object _syncRoot = new object();

        public string this[int index]
        {
            get { return Get(index); }
        }

        public string this[string name]
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

        public void Add(string name, string value)
        {
            AddWithoutValidate(name, value);
        }

        protected void AddWithoutValidate(string name, string value)
        {
            _pairs.Add(new NameValuePair(name, value));
        }

        public void Clear()
        {
            _pairs.Clear();
        }

        public string Get(int index)
        {
            return ((NameValuePair)_pairs[index]).Value;
        }

        public string Get(string name)
        {
            for (int i = 0; i < Count; i++)
            {
                NameValuePair nvp = (NameValuePair)_pairs[i];
                if (nvp.Name == name)
                {
                    return nvp.Value;
                }
            }
            return null;
        }

        public string GetKey(int index)
        {
            return ((NameValuePair)_pairs[index]).Name;
        }

        public void Remove(string name)
        {
            for (int i = 0; i < Count; i++)
            {
                NameValuePair nvp = (NameValuePair)_pairs[i];
                if (nvp.Name == name)
                {
                    _pairs.RemoveAt(i);
                    break;
                }
            }
        }

        public void Set(string name, string value)
        {
            for (int i = 0; i < Count; i++)
            {
                NameValuePair nvp = (NameValuePair)_pairs[i];
                if (nvp.Name == name)
                {
                    nvp.Value = value;
                    return;
                }
            }
            Add(name, value);
        }

        private ArrayList GetKeys()
        {
            ArrayList list = new ArrayList();
            foreach (NameValuePair nvp in _pairs)
            {
                list.Add(nvp.Name);
            }
            return list;
        }

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            _pairs.CopyTo(array, index);
        }

        public int Count
        {
            get { return _pairs.Count; }
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
            return new NameValueEnumerator(this);
        }

        #endregion
    }

}