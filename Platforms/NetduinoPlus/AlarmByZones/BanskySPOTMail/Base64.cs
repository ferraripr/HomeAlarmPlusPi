//------------------------------------------------------------------------------
// Bansky.SPOT.Mail
//
// Bansky.SPOT.Mail allows .NET Micro Framework application to send e-mail
// by using Simple Mail Transfer Protocol (SMTP). Goal of this library is
// to provide similar functionality as System.Net.Mail in full .NET Framework.
//
// http://bansky.net/blog
//
// This code was written by Pavel Bansky. It is released under the terms of 
// the Creative Commons "Attribution 3.0 Unported" license.
// http://creativecommons.org/licenses/by/3.0/
//
//------------------------------------------------------------------------------
using System;
using System.Text;

namespace Bansky.SPOT.Mail
{
    /// <summary>
    /// Represents Base64 encoding / decoding functionality.
    /// 
    /// Heavily based on Base64Encoder by Timm Martin.
    /// http://www.csharp411.com/convert-binary-to-base64-string/
    /// </summary>
    public static class Base64
    {
        /// <summary>
        /// Encodes byte array into Base64 digits.
        /// </summary>
        /// <param name="data">Data to encode.</param>
        /// <returns>Base64 encoded char array.</returns>
        public static char[] Encode(byte[] data)
        {
            int length = data == null ? 0 : data.Length;
            if (length == 0)
                return null;

            int padding = length % 3;
            if (padding > 0)
                padding = 3 - padding;
            int blocks = (length - 1) / 3 + 1;

            char[] encodedData = new char[blocks * 4];

            for (int i = 0; i < blocks; i++)
            {
                bool finalBlock = i == blocks - 1;
                bool pad2 = false;
                bool pad1 = false;
                if (finalBlock)
                {
                    pad2 = padding == 2;
                    pad1 = padding > 0;
                }

                int index = i * 3;
                byte b1 = data[index];
                byte b2 = pad2 ? (byte)0 : data[index + 1];
                byte b3 = pad1 ? (byte)0 : data[index + 2];

                byte temp1 = (byte)((b1 & 0xFC) >> 2);

                byte temp = (byte)((b1 & 0x03) << 4);
                byte temp2 = (byte)((b2 & 0xF0) >> 4);
                temp2 += temp;

                temp = (byte)((b2 & 0x0F) << 2);
                byte temp3 = (byte)((b3 & 0xC0) >> 6);
                temp3 += temp;

                byte temp4 = (byte)(b3 & 0x3F);

                index = i * 4;
                encodedData[index] = SixBitToChar(temp1);
                encodedData[index + 1] = SixBitToChar(temp2);
                encodedData[index + 2] = pad2 ? '=' : SixBitToChar(temp3);
                encodedData[index + 3] = pad1 ? '=' : SixBitToChar(temp4);
            }

            return encodedData;
        }

        /// <summary>
        /// Encodes byte array into Base64 digits.
        /// </summary>
        /// <param name="data">Data to encode.</param>
        /// <param name="insertLineBreaks">Determines whether the output will be MIME friendly.</param>
        /// <returns>Base64 encoded string.</returns>
        public static string Encode(byte[] data, bool insertLineBreaks)
        {            
            char[] encodedData = Encode(data);

            // Joins chars into string and process line-breaks
            string result = string.Empty;
            for (int i = 1; i <= encodedData.Length; i++)
            {
                result += encodedData[i - 1];
                if (insertLineBreaks && (i % MIME_LINE_LENGTH) == 0)
                    result += "\r\n";
            }

            return result;
        }

        /// <summary>
        /// Encodes string into Base64 digits.
        /// </summary>
        /// <param name="data">Data to encode.</param>
        /// <param name="insertLineBreaks">Determines whether the output will be MIME friendly.</param>
        /// <returns>Base64 encoded string.</returns>
        public static string Encode(string data, bool insertLineBreaks)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(data);
            return Encode(inputBytes, insertLineBreaks);
        }

        /// <summary>
        /// Decodes data in Base64 encoding.
        /// </summary>
        /// <param name="data">Data to decode.</param>
        /// <returns>Decoded data.</returns>
        public static byte[] Decode(char[] data)
        {
            int length = data.Length;
            if (length == 0)
                return new byte[0];

            int padding = 0;
            if (length > 2 && data[length - 2] == '=')
                padding = 2;
            else if (length > 1 && data[length - 1] == '=')
                padding = 1;

            int blocks = (length - 1) / 4 + 1;
            int bytes = blocks * 3;

            byte[] decodedData = new byte[bytes - padding];

            for (int i = 0; i < blocks; i++)
            {
                bool finalBlock = i == blocks - 1;
                bool pad2 = false;
                bool pad1 = false;
                if (finalBlock)
                {
                    pad2 = padding == 2;
                    pad1 = padding > 0;
                }

                int index = i * 4;
                byte temp1 = CharToSixBit(data[index]);
                byte temp2 = CharToSixBit(data[index + 1]);
                byte temp3 = CharToSixBit(data[index + 2]);
                byte temp4 = CharToSixBit(data[index + 3]);

                byte b = (byte)(temp1 << 2);
                byte b1 = (byte)((temp2 & 0x30) >> 4);
                b1 += b;

                b = (byte)((temp2 & 0x0F) << 4);
                byte b2 = (byte)((temp3 & 0x3C) >> 2);
                b2 += b;

                b = (byte)((temp3 & 0x03) << 6);
                byte b3 = temp4;
                b3 += b;

                index = i * 3;
                decodedData[index] = b1;
                if (!pad2)
                    decodedData[index + 1] = b2;
                if (!pad1)
                    decodedData[index + 2] = b3;
            }

            return decodedData;
        }

        /// <summary>
        /// Decodes string in Base64 encoding.
        /// </summary>
        /// <param name="data">Data to decode.</param>
        /// <returns>Decoded data.</returns>
        public static byte[] Decode(string data)
        {
            // Remove all line-breaks and white spaces
            string[] lines = data.Split('\n');
            data = string.Empty;
            foreach (string line in lines)
            {
                data += line.Trim();
            }

            return Decode(Encoding.UTF8.GetChars(Encoding.UTF8.GetBytes(data)));
        }

        private static byte CharToSixBit(char c)
        {
            byte b;
            if (c >= 'A' && c <= 'Z')
            {
                b = (byte)((int)c - (int)'A');
            }
            else if (c >= 'a' && c <= 'z')
            {
                b = (byte)((int)c - (int)'a' + 26);
            }
            else if (c >= '0' && c <= '9')
            {
                b = (byte)((int)c - (int)'0' + 52);
            }
            else if (c == CHAR_PLUS_SIGN)
            {
                b = (byte)62;
            }
            else
            {
                b = (byte)63;
            }
            return b;
        }

        private static char SixBitToChar(byte b)
        {
            char c;
            if (b < 26)
            {
                c = (char)((int)b + (int)'A');
            }
            else if (b < 52)
            {
                c = (char)((int)b - 26 + (int)'a');
            }
            else if (b < 62)
            {
                c = (char)((int)b - 52 + (int)'0');
            }
            else if (b == 62)
            {
                c = CHAR_PLUS_SIGN;
            }
            else
            {
                c = CHAR_SLASH;
            }
            return c;
        }

        private const int MIME_LINE_LENGTH = 76;
        private const char CHAR_PLUS_SIGN = '+';
        private const char CHAR_SLASH = '/';
    }
}
