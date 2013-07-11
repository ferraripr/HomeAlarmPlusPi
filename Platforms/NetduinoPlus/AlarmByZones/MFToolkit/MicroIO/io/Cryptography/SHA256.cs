/* 
 * SHA256.cs
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
    public class SHA256
    {
        #region Private Fields

        //SHA-256 digests
        private static uint h0;
        private static uint h1;
        private static uint h2;
        private static uint h3;
        private static uint h4;
        private static uint h5;
        private static uint h6;
        private static uint h7;

        /// <summary>
        /// K Constants
        /// </summary>
        private static UInt32[] K = { 0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5, 
						   0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
						   0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3,
						   0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
						   0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc,
						   0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
						   0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7,
						   0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
						   0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13,
						   0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
						   0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3,
						   0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
						   0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5,
						   0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
						   0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208,
						   0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2 };

        /// <summary>
        /// Hex symbols used for displaying crypto result in common way
        /// </summary>
        private static char[] hexSymbols = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        #endregion

        #region Private Methods

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
        /// Performs SHA-256 logical functions.  See FIPS 180-3 for
        /// complete description.
        /// </summary>
        /// <param name="b">
        /// Boolean value indicating whether the desired function is Ch
        /// </param>
        /// <param name="x">First arguement</param>
        /// <param name="y">Second arguement</param>
        /// <param name="z">Third arguement</param>
        /// <returns>Function results</returns>
        private static uint F(bool b, uint x, uint y, uint z)
        {
            if (b)
                return (uint)((x & y) ^ ((~x) & z));
            return (uint)((x & y) ^ (x & z) ^ (y & z));
        }

        /// <summary>
        /// Performs the SHA-256 shifting and rotating functions.
        /// See FIPS 180-2 for complete details.  For internal
        /// use only!
        /// </summary>
        /// <param name="i">Integer indicating which function to perform</param>
        /// <param name="x">Operand</param>
        /// <returns>Result of rotation/shift</returns>
        private static uint Sigma(int i, uint x)
        {
            uint temp;

            switch (i)
            {
                case 0:
                    temp = RollLeft(x, 30);
                    temp ^= RollLeft(x, 19);
                    temp ^= RollLeft(x, 10);
                    break;
                case 1:
                    temp = RollLeft(x, 26);
                    temp ^= RollLeft(x, 21);
                    temp ^= RollLeft(x, 7);
                    break;
                case 2:
                    temp = RollLeft(x, 25);
                    temp ^= RollLeft(x, 14);
                    temp ^= (uint)(x >> 3);
                    break;
                case 3:
                    temp = RollLeft(x, 15);
                    temp ^= RollLeft(x, 13);
                    temp ^= (uint)(x >> 10);
                    break;
                default:
                    temp = x;
                    break;
            }
            return temp;
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
            // Calculation variables
            UInt32 a, b, c, d, e, f, g, h, temp1, temp2;
            UInt32[] w = new UInt32[64];
            int j;

            // Prepare the message schedule
            for (int t = 0; t < 64; t++)
            {
                if (t < 16)
                {
                    j = 4 * t;
                    w[t] = GetValue(new byte[] { block[j + 3], block[j + 2], block[j + 1], block[j] }, 0, 4);
                }
                else
                {
                    w[t] = (uint)(Sigma(3, w[t - 2]) + w[t - 7]
                        + Sigma(2, w[t - 15]) + w[t - 16]);
                }
            }

            a = h0;
            b = h1;
            c = h2;
            d = h3;
            e = h4;
            f = h5;
            g = h6;
            h = h7;

            //Perform main hash loop
            for (int t = 0; t < 64; t++)
            {
                temp1 = (uint)(h + Sigma(1, e) + F(true, e, f, g)
                    + K[t] + w[t]);
                temp2 = (uint)(Sigma(0, a) + F(false, a, b, c));
                h = g;
                g = f;
                f = e;
                e = (uint)(d + temp1);
                d = c;
                c = b;
                b = a;
                a = (uint)(temp1 + temp2);
            }

            h0 += a;
            h1 += b;
            h2 += c;
            h3 += d;
            h4 += e;
            h5 += f;
            h6 += g;
            h7 += h;
        }

        #endregion

        /// <summary>
        /// Get the SHA-256 message digest from stream
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

            /* Initialize the digest buffers using SHA-256 constants */
            h0 = 0x6a09e667;
            h1 = 0xbb67ae85;
            h2 = 0x3c6ef372;
            h3 = 0xa54ff53a;
            h4 = 0x510e527f;
            h5 = 0x9b05688c;
            h6 = 0x1f83d9ab;
            h7 = 0x5be0cd19;

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

            return GetHStr(h0) + GetHStr(h1) + GetHStr(h2) + GetHStr(h3) + GetHStr(h4) + GetHStr(h5) + GetHStr(h6) + GetHStr(h7);
        }
    }
}