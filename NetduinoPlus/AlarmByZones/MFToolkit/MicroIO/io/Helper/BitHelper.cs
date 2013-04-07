/* 
 * BitHelper.cs
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
 * MS   09-03-13    initial version
 * 
 */
using System;

namespace MFToolkit.IO
{
    /// <summary>
    /// Static class providing utility methods for manipulating bits in a bit field (8 bit).
    /// </summary>
    public class BitHelper
    {
        public static ushort SetBit(ref ushort value, int position, bool flag)
        {
            return SetBits(ref value, position, 1, (flag ? (ushort)1 : (ushort)0));
        }

        public static ushort SetBits(ref ushort value, int position, int length, ushort bits)
        {
            if (length <= 0 || position >= 16)
                return value;

            int mask = (2 << (length - 1)) - 1;

            value &= (ushort)~(mask << position);
            value |= (ushort)((bits & mask) << position);

            return value;
        }

        public static ushort SetBit(ref byte value, int position, bool flag)
        {
            if (position >= 8)
                return value;

            int mask = (2 << (1 - 1)) - 1;

            value &= (byte)~(mask << position);
            value |= (byte)(((flag ? (byte)1 : (byte)0) & mask) << position);

            return value;
        }

        public static bool GetBit(ushort value, int position)
        {
            return (GetBits(value, position, 1) == 1);
        }

        public static ushort GetBits(ushort value, int position, int length)
        {
            if (length <= 0 || position >= 16)
                return 0;

            int mask = (2 << (length - 1)) - 1;

            return (ushort)((value >> position) & mask);
        }

        public static bool GetBit(byte value, int position)
        {
            if (position >= 8)
                return false;

            int mask = (2 << (1 - 1)) - 1;

            return ((byte)((value >> position) & mask)) == 1;
        }
    }
}
