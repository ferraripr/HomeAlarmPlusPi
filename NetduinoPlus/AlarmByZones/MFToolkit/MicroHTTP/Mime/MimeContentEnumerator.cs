/* 
 * MimeContentEnumerator.cs
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
    public class MimeContentEnumerator : IEnumerator
    {
        private MimeContentCollection _collection;
        private int _idx = -1;

        public MimeContentEnumerator(MimeContentCollection collection)
        {
            _collection = collection;
        }

        #region IEnumerator Members

        public object Current
        {
            get
            {
                return _collection[_idx];
            }
        }

        public bool MoveNext()
        {
            _idx++;
            if (_idx < _collection.Count)
            {
                return true;
            }
            else
            {
                _idx = -1;
                return false;
            }

        }

        public void Reset()
        {
            _idx = -1;
        }

        #endregion
    }
}
