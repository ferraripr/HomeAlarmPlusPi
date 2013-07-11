/* 
 * MimeContent.cs
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
using System.Text;
using MFToolkit.Collection.Spezialized;

namespace MFToolkit.Net.Web
{
    public class MimeContent
    {
        public NameValueCollection Headers;
        public byte[] Content;

        public string Name
        {
            get
            {
                string contentDisposition = Headers["Content-Disposition"];

                if (contentDisposition != null)
                {
                    string[] parts = contentDisposition.Split(';');

                    foreach (string part in parts)
                    {
                        string[] keyvalue = part.Split('=');

                        if (keyvalue.Length == 2)
                        {
                            if (keyvalue[0].Trim() == "name")
                            {
                                string name = keyvalue[1];
                                if (name.Length > 0 && name[0] == '"')
                                    name = name.Substring(1);
                                if (name.Length > 0 && name[name.Length - 1] == '"')
                                    name = name.Substring(0, name.Length - 1);

                                return name;
                            }
                        }
                    }
                }

                return "Unknown-" + Guid.NewGuid();
            }
        }

        public MimeContent()
        {
            Headers = new NameValueCollection();
        }
    }
}
