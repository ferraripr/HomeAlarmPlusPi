/* 
 * SHA1.cs
 * 
 * Copyright (c) 2009, Freesc Huang (http://www.microframework.cn)
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
 * For more information about SHA algorithms
 * see FIPS 180-3 (http://csrc.nist.gov/publications/fips/fips180-3/fips180-3_final.pdf)
 * 
 * FH   09-03-26    initial version
 * 
 * 
 */
using System;
using System.IO;

namespace MFToolkit.Cryptography
{
    public class SHA1
    {
        #region Private Fields

        /// <summary>
        /// 5 digest buffer£¬used to combine as the final 160 bit digest
        /// </summary>
        private static UInt32 h0, h1, h2, h3, h4;

        /// <summary>
        /// K Constants
        /// </summary>
        private static UInt32[] K = { 0x5A827999, 0x6ED9EBA1, 0x8F1BBCDC, 0xCA62C1D6 };

        /// <summary>
        /// Hex symbols used for displaying crypto result in common way
        /// </summary>
        private static char[] hexSymbols = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        #endregion

        #region Private Methods

        /// <summary>
        /// Performs SHA-1 logical functions. 
        /// </summary>
        /// <see cref="http://csrc.nist.gov/publications/fips/fips180-3/fips180-3_final.pdf"/>
        /// <param name="t">function number</param>
        /// <param name="x">first arguement</param>
        /// <param name="y">second arguement</param>
        /// <param name="z">third arguement</param>
        /// <returns>Function results</returns>
        private static UInt32 F(int t, UInt32 x, UInt32 y, UInt32 z)
        {
            if (t < 20)
                return (z ^ (x & (y ^ z)));
            else if ((t < 60) && (t >= 40))
                return ((x & y) | (z & (x | y)));
            else
                return (x ^ y ^ z);
        }

        /// <summary>
        /// Roll left, RollLeft(x,n) = (X¡¶n)|(X¡·32-n)
        /// </summary>
        /// <param name="n">bit to move</param>
        /// <returns>result</returns>
        private static UInt32 RollLeft(UInt32 x, int n)
        {
            return ((x << n) | (x >> (32 - n)));
        }

        /// <summary>
        /// Get Hex string from the UInt32 digest buffer 
        /// </summary>
        /// <param name="buff">buffer</param>
        /// <returns>Hex value string</returns>
        private static String GetHStr(UInt32 buff)
        {
            char[] hex = new char[8];
            for (int i = 8; i > 0; i--)
            {
                if (buff == 0) break;
                hex[i - 1] = hexSymbols[buff & 0x0F];
                buff >>= 4;
            }
            return new String(hex);
        }

        /// <summary> 
        /// Gets uint32 value from array byte specialized by endian 
        /// </summary> 
        /// <remarks>
        /// now the Micro Framework CLR is little endian
        /// </remarks>
        /// <param name="byteArray">Byte array</param> 
        /// <param name="startIndex">Start index</param> 
        /// <param name="length">Number of bytes to parse</param> 
        /// <returns>UInt32 value</returns> 
        private static UInt32 GetValue(byte[] byteArray, int startIndex, int length)
        {
            UInt32 retValue = 0;
            int stopIndex = startIndex + length - 1;
            for (int i = stopIndex; i > startIndex; i--)
            {
                retValue |= byteArray[i];
                retValue <<= 8;
            }
            retValue |= byteArray[startIndex];
            return retValue;
        }

        /// <summary>
        /// The major hash transform
        /// </summary>
        /// <param name="block">data block</param>
        private static void Sha1Transform(byte[] block)
        {
            UInt32 a, b, c, d, e, temp;
            UInt32[] w = new UInt32[80];
            int j;

            //Prepare the message schedule
            for (int t = 0; t < 80; t++)
            {
                if (t < 16)
                {
                    j = 4 * t;
                    w[t] = GetValue(new byte[] { block[j + 3], block[j + 2], block[j + 1], block[j] }, 0, 4);
                }

                else
                {
                    temp = (UInt32)(w[t - 3] ^ w[t - 8] ^ w[t - 14] ^ w[t - 16]);
                    w[t] = RollLeft(temp, 1);
                }
            }

            a = h0;
            b = h1;
            c = h2;
            d = h3;
            e = h4;

            for (int t = 0; t < 80; t++)
            {
                int kt;
                if (t < 20)
                    kt = 0;
                else if (t < 40)
                    kt = 1;
                else if (t < 60)
                    kt = 2;
                else
                    kt = 3;
                temp = RollLeft(a, 5) + F(t, b, c, d) + e + K[kt] + w[t];
                e = d;
                d = c;
                c = RollLeft(b, 30);
                b = a;
                a = temp;
            }

            h0 += a;
            h1 += b;
            h2 += c;
            h3 += d;
            h4 += e;
        }

        #endregion

        /// <summary>
        /// Get the SHA-1 message digest from stream
        /// </summary>
        /// <param name="stream">data stream</param>
        /// <param name="index">begine index in the stream</param>
        /// <param name="count">count to be processed</param>
        /// <returns>message digest</returns>
        public static string Compute(Stream stream, long index, long count)
        {
            if (stream == null)
                throw new ArgumentNullException();
            byte[] block = new byte[64];
            UInt64 size = 0;
            int b;

            /* Initialize the digest buffers using SHA-1 constants */
            h0 = 0x67452301;
            h1 = 0xefcdab89;
            h2 = 0x98badcfe;
            h3 = 0x10325476;
            h4 = 0xc3d2e1f0;

            while (index-- > 0 && stream.ReadByte() >= 0)
                ;
            if (index >= 0)
                throw new ArgumentException("", "index");
            while ((b = stream.ReadByte()) != -1 && count-- > 0)
            {
                block[(Int32)(size++ % 64)] = (byte)b;

                //each block used to do the hash-transform is 64 bytes (512bit) long
                if (size % 64 == 0)
                    Sha1Transform(block);
            }
            if (count > 0)
                throw new ArgumentException("", "count");
            UInt64 bp = size % 64;//blockPoint

            /*
             * bit padding, see http://csrc.nist.gov/publications/fips/fips180-3/fips180-3_final.pdf
             */
            block[(Int32)(bp++)] = 0x80;
            if (bp > 56)
            {
                while (bp < 64)
                    block[(Int32)(bp++)] = 0;
                Sha1Transform(block);
                for (int i = 0; i < 56; i++)
                    block[i] = 0;
            }
            else
                while (bp < 56)
                    block[(Int32)(bp++)] = 0;

            //length padding
            size *= 8;
            block[56] = (byte)(size >> 56);
            block[57] = (byte)((size >> 48) & 0xFF);
            block[58] = (byte)((size >> 40) & 0xFF);
            block[59] = (byte)((size >> 32) & 0xFF);
            block[60] = (byte)((size >> 24) & 0xFF);
            block[61] = (byte)((size >> 16) & 0xFF);
            block[62] = (byte)((size >> 8) & 0xFF);
            block[63] = (byte)(size & 0xFF);
            Sha1Transform(block);

            return GetHStr(h0) + GetHStr(h1) + GetHStr(h2) + GetHStr(h3) + GetHStr(h4);
        }
    }
}