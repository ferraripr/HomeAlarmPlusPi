/* 
 * ByteOrder.cs
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

namespace MFToolkit.IO
{
    public enum ByteOrder
    {
        /// <summary>Use the default byte order for the computer.</summary>
        Default = 0,

        /// <summary>Use big-endian byte order, also known as Motorola byte order.</summary>
        BigEndian = 1,

        /// <summary>Use little-endian byte order, also known as Intel byte order.</summary>
        LittleEndian = 2,

        /// <summary>Use Motorola byte order. Corresponds to <see cref="BigEndian"/>.</summary>
        Motorola = BigEndian,

        /// <summary>Use Intel byte order. Corresponds to <see cref="LittleEndian"/>.</summary>
        Intel = LittleEndian,

        /// <summary>The order which multi-byte values are transmitted on a network.</summary>
        Network = BigEndian
    }
}
