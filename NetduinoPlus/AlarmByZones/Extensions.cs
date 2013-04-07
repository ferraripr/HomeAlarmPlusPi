using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Net;
using System.Net.Sockets;

// This is needed for extension methods to work
namespace System.Runtime.CompilerServices
{
    public class ExtensionAttribute : Attribute { }
}


    public static class Extension
    {

        public static bool status = true;

        public static string Replace(this string stringToSearch, char charToFind, char charToSubstitute)
        {
            // Surely there must be nicer way than this?
            char[] chars = stringToSearch.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
                if (chars[i] == charToFind) chars[i] = charToSubstitute;
            return new string(chars);
        }

        public static string Replace(this string stringToSearch, string stringToFind, string stringToSubstitute)
        {
            string s = stringToSearch;
            int i = s.IndexOf(stringToFind);
            while (i >= 0)
            {
                s = s.Substring(0, i) + stringToSubstitute + s.Substring(i + stringToFind.Length);
                i = s.IndexOf(stringToFind);
            }
            return s;
        }

        /// <summary>
        /// Sets the System Time from the Internet
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="TimeZoneOffset">Offset of Time Zone from UTC time</param>
        public static void SetFromNetwork(this DateTime dateTime, TimeSpan TimeZoneOffset)
        {
            // Based on http://weblogs.asp.net/mschwarz/archive/2008/03/09/wrong-datetime-on-net-micro-framework-devices.aspx
            // And http://nickstips.wordpress.com/2010/02/12/c-get-nist-internet-time/
            // Time server list: http://tf.nist.gov/tf-cgi/servers.cgi

            //var ran = new Random(DateTime.Now.Millisecond);

            var servers = new string[] { "time-a.nist.gov", "time-b.nist.gov", "nist1-la.ustiming.org", "nist1-chi.ustiming.org", 
                "nist1-ny.ustiming.org", "time-nw.nist.gov", "nist1-atl.ustiming.org", "nist.netservicesgroup.com" };

            Debug.Print("Setting Date and Time from Network");
            
            // Try each server in random order to avoid blocked requests due to too frequent request  

            //for (int i = 0; i < servers.Length; i++)
            {
                try
                {
                    // Open a Socket to a random time server  
                    //var ep = new IPEndPoint(Dns.GetHostEntry(servers[ran.Next(servers.Length)]).AddressList[0], 123);
                    //gg: Instead of random approach, let's use a stable NIST
                    var ep = new IPEndPoint(Dns.GetHostEntry(servers[7]).AddressList[0], 123);

                    var s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    s.Connect(ep);

                    byte[] ntpData = new byte[48]; // RFC 2030
                    ntpData[0] = 0x1B;
                    for (int x = 1; x < 48; x++)
                        ntpData[x] = 0;

                    s.Send(ntpData);
                    s.Receive(ntpData);

                    byte offsetTransmitTime = 40;
                    ulong intpart = 0;
                    ulong fractpart = 0;
                    for (int x = 0; x <= 3; x++)
                        intpart = 256 * intpart + ntpData[offsetTransmitTime + x];

                    for (int x = 4; x <= 7; x++)
                        fractpart = 256 * fractpart + ntpData[offsetTransmitTime + x];

                    ulong milliseconds = (intpart * 1000 + (fractpart * 1000) / 0x100000000L);

                    s.Close();

                    //TODO: Compute the Digest and compare to that included to do a real check...
                    if (ntpData[47] != 0)
                    {
                        TimeSpan timeSpan = TimeSpan.FromTicks((long)milliseconds * TimeSpan.TicksPerMillisecond);
                        DateTime tempDateTime = new DateTime(1900, 1, 1);
                        tempDateTime += timeSpan;

                        DateTime networkDateTime = (tempDateTime + TimeZoneOffset);
                        Debug.Print(networkDateTime.ToString());
                        Utility.SetLocalTime(networkDateTime);

                        //break;
                    }

                }
                catch (Exception)
                {
                    /* Do Nothing...try the next server */
                    status = false;
                }

                // Check to see that the signature is there  
            }            
        }

        public static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1048576) ;
        }

        public static double ConvertKilobytesToMegabytes(long kilobytes)
        {
            return kilobytes / 1024;
        }
    }

