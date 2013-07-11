/* 
 * HttpServerUtility.cs
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
 * 
 */
using System;
using System.IO;
using MFToolkit.IO;
using System.Text;
using System.Collections;
#if(MF)
using MFToolkit.Text;
#endif

namespace MFToolkit.Net.Web
{
    public class HttpServerUtility
    {
        private static bool IsSafe(char c)
        {

            switch (c)
            {
                case '\'':
                case '(':
                case ')':
                case '[':
                case ']':
                case '*':
                case '-':
                case '.':
                case '!':
                case '_':
                    return true;
            }

            if (c > 255)
                return true;

            return false;
        }

        public static string UrlEncode(string s)
        {
            if(s == null)
                return null;

            string res = "";

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == ' ')
                    res += '+';
                else if (!IsSafe(s[i]))
                    res += '%' + ByteUtil.BytesToHex(Encoding.UTF8.GetBytes(s[i].ToString()));
                else
                    res += s[i];
            }

            return res;
        }

        public static string UrlDecode(string s)
        {
            if (s == null)
                return null;

            ArrayList bytes = new ArrayList();

            for(int i=0; i<s.Length; i++)
            {
                char c = s[i];

                switch (c)
                {
                    case '+':
                        bytes.Add(Encoding.UTF8.GetBytes(" ")[0]);
                        break;
                    case '%':
                        foreach (byte _b in ByteUtil.HexToByte(new string(new char[] { s[++i], s[++i] })))
                            bytes.Add(_b);
                        break;
                    default:
                        bytes.Add(Encoding.UTF8.GetBytes(c.ToString())[0]);
                        break;
                }
            }

            byte[] b = new byte[bytes.Count];
            for(int i=0; i<bytes.Count; i++)
                b[i] = (byte)bytes[i];

            return new string(Encoding.UTF8.GetChars(b));

        }

        public static string HtmlEncode(string s)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            foreach (char c in s)
            {
                switch(c)
                {
                    case '&':
                        sb.Append("&amp;");
                        break;
                    case '<':
                        sb.Append("&lt;");
                        break;
                    case '>':
                        sb.Append("&gt;");
                        break;
                    default:
                        if ((int)(c) > 127)
                            sb.Append("&#" + (int)(c) + ";");
                        else
                            sb.Append(c);
                        break;
                }
            }
		
            return sb.ToString();
        }
    }
}
