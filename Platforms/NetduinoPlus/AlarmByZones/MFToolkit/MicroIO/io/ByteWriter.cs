/* 
 * ByteWriter.cs
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
 */
using System;
using System.Text;
using System.IO;

namespace MFToolkit.IO
{
    public class ByteWriter : IDisposable
    {
        protected Stream _stream;
        protected Encoding _encoding = Encoding.UTF8;
        protected ByteOrder _byteOrder = ByteOrder.Default;
        private static ByteOrder _defaultByteOrder;

        #region Constructor

        /// <summary>
        /// Creates a new instance of ByteWriter using UTF-8 encoding.
        /// </summary>
        public ByteWriter()
        {
            _byteOrder = ByteOrder.Default;
            _stream = new MemoryStream();
        }

        public ByteWriter(ByteOrder byteOrder)
        {
            _byteOrder = byteOrder;
            _stream = new MemoryStream();
        }

        public ByteWriter(int capacity)
        {
            _byteOrder = ByteOrder.Default;

#if(MF)
			_stream = new MemoryStream();
#else
			//_stream = new MemoryStream(capacity);
#endif
        }

        public ByteWriter(int capacity, ByteOrder byteOrder)
        {
            _byteOrder = byteOrder;

			_stream = new MemoryStream();

        }

		public ByteWriter(ByteOrder byteOrder, Encoding encoding)
            : this()
        {
            _byteOrder = byteOrder;
            _encoding = encoding;

            if (_byteOrder == ByteOrder.Default)
                _byteOrder = _defaultByteOrder;
        }

        public ByteWriter(Stream s)
            : this()
        {
            if (s != null && !s.CanWrite)
                throw new ArgumentException("The Stream must be writable.");

            _stream = s;
        }

        public ByteWriter(Stream s, ByteOrder byteOrder, Encoding encoding) : this(s)
        {
            _byteOrder = byteOrder;
            _encoding = encoding;

            if (_byteOrder == ByteOrder.Default)
                _byteOrder = _defaultByteOrder;
        }

        static ByteWriter()
        {
            _defaultByteOrder = ByteOrder.LittleEndian;
        }

        #endregion

        #region Public Properties

        public Encoding Encoding
        {
            get { return _encoding; }
            set { _encoding = value; }
        }

        public Stream BaseStream
        {
            get { return _stream; }
        }

        #endregion

        /// <summary>
        /// Writes one byte to the stream.
        /// </summary>
        /// <param name="b">The byte to write to the stream.</param>
        public virtual void Write(byte b)
        {
            if(_stream != null && _stream.CanWrite)
                _stream.WriteByte(b);
        }

        /// <summary>
        /// Writes one or more bytes to the stream.
        /// </summary>
        /// <param name="bytes">The bytes to write to the stream.</param>
        public virtual void Write(byte[] bytes)
        {
            if (_stream != null && _stream.CanWrite)
                _stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Writes a boolean value to the stream. If true write 0; otherwise 1.
        /// </summary>
        /// <param name="b">The boolean to write.</param>
        public virtual void Write(bool b)
        {
            if (b)
                Write(1);
            else
                Write(0);
        }

        /// <summary>
        /// Writes a 2-byte signed integer to the stream.
        /// </summary>
        /// <param name="s">The 2-byte signed integer to write.</param>
        public virtual void Write(short s)
        {
            switch (_byteOrder)
            {
                case ByteOrder.BigEndian:
                    Write((byte)(s >> 8));
                    Write((byte)s);
                    break;
                case ByteOrder.LittleEndian:
                    Write((byte)s);
                    Write((byte)(s >> 8));
                    break;
                default:
                    throw new Exception("Could not handle ByteOrder.");
            }
        }

        /// <summary>
        /// Writes a 4-byte signed integer to the stream.
        /// </summary>
        /// <param name="i">The 4-byte signed integer to write</param>
        public virtual void Write(int i)
        {
            switch (_byteOrder)
            {
                case ByteOrder.BigEndian:
                    Write((short)(i >> 16));
                    Write((short)i);
                    break;
                case ByteOrder.LittleEndian:
                    Write((short)i);
                    Write((short)(i >> 16));
                    break;
                default:
                    throw new Exception("Could not handle ByteOrder.");
            }
        }

        /// <summary>
        /// Writes a 8-byte signed integer to the stream.
        /// </summary>
        /// <param name="i">The 8-byte signed integer to write</param>
        public virtual void Write(long l)
        {
            switch(_byteOrder)
            {
                case ByteOrder.BigEndian:
                    Write((int)(l >> 32));
                    Write((int)l);
                    break;
                case ByteOrder.LittleEndian:
                    Write((int)l);
                    Write((int)(l >> 32));
                    break;
                default:
                    throw new Exception("Could not handle ByteOrder.");
            }
        }

        /// <summary>
        /// Writes a 2-byte unsigned integer to the stream.
        /// </summary>
        /// <param name="i">The 2-byte unsigned integer to write</param>
        public virtual void Write(ushort s)
        {
            switch (_byteOrder)
            {
                case ByteOrder.BigEndian:
                    Write((byte)(s >> 8));
                    Write((byte)s);
                    break;
                case ByteOrder.LittleEndian:
                    Write((byte)s);
                    Write((byte)(s >> 8));
                    break;
                default:
                    throw new Exception("Could not handle ByteOrder.");
            }
        }

        /// <summary>
        /// Writes a 4-byte unsigned integer to the stream.
        /// </summary>
        /// <param name="i">The 4-byte unsigned integer to write</param>
        public virtual void Write(uint i)
        {
            switch (_byteOrder)
            {
                case ByteOrder.BigEndian:
                    Write((byte)(i >> 24));
                    Write((byte)(i >> 16));
                    Write((byte)(i >> 8));
                    Write((byte)(i));
                    break;
                case ByteOrder.LittleEndian:
                    Write((ushort)i);
                    Write((ushort)(i >> 16));
                    break;
                default:
                    throw new Exception("Could not handle ByteOrder.");
            }
        }

        /// <summary>
        /// Writes a 8-byte unsigned integer to the stream.
        /// </summary>
        /// <param name="i">The 8-byte unsigned integer to write</param>
        public virtual void Write(ulong l)
        {
            switch (_byteOrder)
            {
                case ByteOrder.BigEndian:
                    Write((uint)(l >> 32));
                    Write((uint)l);
                    break;
                case ByteOrder.LittleEndian:
                    Write((uint)l);
                    Write((uint)(l >> 32));
                    break;
                default:
                    throw new Exception("Could not handle ByteOrder.");
            }
        }

        /// <summary>
        /// Writes a character to the stream.
        /// </summary>
        /// <param name="c">The character to write.</param>
        public virtual void Write(char c)
        {
            byte[] bytes = _encoding.GetBytes(c.ToString());

            Write(bytes);
        }

        /// <summary>
        /// Writes a string to the stream.
        /// </summary>
        /// <param name="s">The string to write.</param>
        public virtual void Write(string s)
        {
            byte[] bytes = _encoding.GetBytes(s);
            Write(bytes);
        }

        /// <summary>
        /// Writes a string to the current stream. The string is prefixed with the length, encoded as 32-bit integer.
        /// </summary>
        /// <param name="s">The string to write.</param>
        public virtual void WriteString(string s)
        {
            byte[] bytes = _encoding.GetBytes(s);
            Write((int)bytes.Length);
            Write(bytes);
        }

        #region IDisposable Members

        public void Dispose(bool disposing)
        {
            // Check if we have created a new instance of MemoryStream;
            // external Streams are not closed.

            if (_stream != null)
            {
                _stream.Close();

#if (!WindowsCE)
                _stream.Dispose();
#endif
                _stream = null;
            }
            
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        /// <summary>
        /// Gets the bytes written to the stream.
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            MemoryStream ms = _stream as MemoryStream;

            if (_stream == null || ms == null)
                return null;

            return ms.ToArray();
        }
    }
}