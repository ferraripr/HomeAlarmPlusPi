/* 
 * StringBuilder.cs
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
 * MS   09-02-16    added StringBuilder
 * MS   09-06-19    added string extensions
 * 
 */
using System;
using Microsoft.SPOT;
using System.Collections;

namespace System.Runtime.CompilerServices
{
    //public class ExtensionAttribute : Attribute {}

}

//namespace MFToolkit.Text
//{
//    public static class StringExtensions
//    {
//        public static bool StartsWith(this string s, string pat)
//        {
//            return true;
//        }
//    }

//    public class StringBuilder
//    {
//        private ArrayList _content;

//        #region Public Properties

//        public int Length
//        {
//            get { return _content.Count; }
//        }

//        #endregion

//        public StringBuilder()
//        {
//            _content = new ArrayList();
//        }

//        public StringBuilder(string s)
//        {
//            _content = new ArrayList();
//            Append(s);
//        }

//        public void Append(string s)
//        {
//            foreach (char c in s)
//                Append(c);
//        }

//        public void Append(char c)
//        {
//            _content.Add(c);
//        }

//        public override string ToString()
//        {
//            string res = "";

//            foreach (char c in _content)
//                res += c;

//            return res;
//        }
//    }
//}
