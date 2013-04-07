/* 
 * NetworkHelper.cs
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
 * MS   09-02-10    fixed MF support
 * 
 */
using System;
using System.Net;
using System.Net.Sockets;
using Socket = System.Net.Sockets.Socket;

namespace MFToolkit.Net
{
	public class NetworkHelper
	{
		public static byte[] GetMacFromString(string mac)
		{
			string[] macparts = mac.Split('-');

			if (macparts.Length != 6)
				throw new ArgumentException("MAC address not in correct format.");

			byte[] bytes = new byte[6];

			for (int i = 0; i < macparts.Length; i++)
			{
				int b = 0;

				for (int c = 0; c < 2; c++)
				{
					switch (macparts[i][1 - c])
					{
						case '0':
							break;

						case '1': b += 1 * (int)Math.Pow(16, c); break;
						case '2': b += 2 * (int)Math.Pow(16, c); break;
						case '3': b += 3 * (int)Math.Pow(16, c); break;
						case '4': b += 4 * (int)Math.Pow(16, c); break;
						case '5': b += 5 * (int)Math.Pow(16, c); break;
						case '6': b += 6 * (int)Math.Pow(16, c); break;
						case '7': b += 7 * (int)Math.Pow(16, c); break;
						case '8': b += 8 * (int)Math.Pow(16, c); break;
						case '9': b += 9 * (int)Math.Pow(16, c); break;
						case 'A': b += 10 * (int)Math.Pow(16, c); break;
						case 'B': b += 11 * (int)Math.Pow(16, c); break;
						case 'C': b += 12 * (int)Math.Pow(16, c); break;
						case 'D': b += 13 * (int)Math.Pow(16, c); break;
						case 'E': b += 14 * (int)Math.Pow(16, c); break;
						case 'F': b += 15 * (int)Math.Pow(16, c); break;
					}
				}

				bytes[i] = (byte)b;
			}

			return bytes;
		}

		public static void WakeUp(byte[] mac)
		{
			using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
			{
				IPEndPoint endPoint = new IPEndPoint(new IPAddress(4294967295), 40000);		// IP 255.255.255.255

				socket.Connect(endPoint);

				byte[] packet = new byte[17 * 6];

				for (int i = 0; i < 6; i++)
					packet[i] = 0xFF;

				for (int i = 1; i <= 16; i++)
					for (int j = 0; j < 6; j++)
						packet[i * 6 + j] = mac[j];

				socket.Send(packet); // , SocketFlags.Broadcast);
			}
		}
	}
}