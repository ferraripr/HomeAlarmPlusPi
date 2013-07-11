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
using System.IO;
using System.Text;

namespace MFToolkit.Net.Web
{
    public class MimeParser
    {
        private byte[] buffer;
        private string _boundary;
        private byte[] _boundaryBytes;
        private int idx = 0;
        private MimeParserState state = MimeParserState.ReadBoundary;

        public MimeParser(byte[] bytes, string boundary)
        {
            buffer = bytes;
            _boundary = boundary;

            _boundaryBytes = Encoding.UTF8.GetBytes("--" + boundary);
        }

        public MimeContent GetNextContent()
        {
            string key = "";
            string value = "";

            MimeContent mime = new MimeContent();
            MemoryStream ms = new MemoryStream();

            do
            {
                switch (state)
                {
                    case MimeParserState.ReadBoundary:
                        if (buffer[idx] == '\r')
                            idx++;
                        else if (buffer[idx] != '\n')
                            value += (char)buffer[idx++];
                        else
                        {
                            idx++;
                            value = "";
                            state = MimeParserState.ReadHeaderKey;
                        }
                        break;
                    case MimeParserState.ReadHeaderKey:
                        if (buffer[idx] == '\r')
                            idx++;
                        else if (buffer[idx] == '\n')
                        {
                            idx++;
                            state = MimeParserState.ReadContent;
                        }
                        else if (buffer[idx] == ':')
                            idx++;
                        else if (buffer[idx] != ' ')
                            key += (char)buffer[idx++];
                        else
                        {
                            idx++;
                            value = "";
                            state = MimeParserState.ReadHeaderValue;
                        }
                        break;
                    case MimeParserState.ReadHeaderValue:
                        if (buffer[idx] == '\r')
                            idx++;
                        else if (buffer[idx] != '\n')
                            value += (char)buffer[idx++];
                        else
                        {
                            idx++;
                            mime.Headers.Add(key, value);
                            key = "";
                            state = MimeParserState.ReadHeaderKey;
                        }
                        break;
                    case MimeParserState.ReadContent:

                        if (buffer[idx] == '\n' && idx < buffer.Length - _boundaryBytes.Length - 2)
                        {
                            // detect if next line is boundary line or content


                            bool foundBound = true;

                            for (int i = 0; i < _boundaryBytes.Length; i++)
                            {
                                if (_boundaryBytes[i] != buffer[idx + i + 1])
                                {
                                    foundBound = false;
                                    break;
                                }
                            }

                            if (!foundBound)
                            {
                                ms.WriteByte(buffer[idx++]);
                                continue;
                            }
                            else
                            {
                                idx++;
                                state = MimeParserState.ReadBoundary;

                                if (ms.ToArray().Length > 1)
                                {
                                    mime.Content = new byte[ms.ToArray().Length - 1];
                                    Array.Copy(ms.ToArray(), mime.Content, ms.ToArray().Length - 1);
                                }

                                return mime;
                            }
                        }
                        else
                        {
                            ms.WriteByte(buffer[idx++]);
                        }

                        break;
                }
            }
            while (idx < buffer.Length);

            return null;
        }
    }

}
