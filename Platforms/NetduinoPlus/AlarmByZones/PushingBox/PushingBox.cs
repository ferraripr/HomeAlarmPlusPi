////
//
// General code from http://www.pushingbox.com for Netduino Plus v1.0
//
////

using System;
using Microsoft.SPOT;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace PushingBox
{
    public class Notification
    {
        #region Constructor
        private Notification()
        {

        }
        #endregion

        #region Declarations

        //Your secret DevID from PushingBox.com. You can use multiple DevID  on multiple Pin if you want

        const string serverName = "api.pushingbox.com";

        /// <summary>
        /// Carriage return and line feed.
        /// </summary>
        const string CRLF = "\r\n";
        #endregion

        #region Methods
        public static void Connect(string DeviceID)
        {
            Socket connection = null;

            if (connection == null)
            {
                try
                {
                    connection = Connect(serverName,1000);
                }
                catch(Exception ex)
                {
                    Debug.Print("Connection Error.\nException: " + ex.Message);
                }
            }
            try
            {
                sendToPushingBox(connection, DeviceID, "NetduinoPlus");
                RecieveResponse(connection);
            }
            catch (Exception ex)
            {
                Debug.Print("Socket Exception: " + ex.Message);
            }
            connection.Close();
            connection = null;            
        }

        /// <summary>
        /// Establishes connection to PushingBox's host.
        /// </summary>
        /// <param name="host">server name or IP address</param>
        /// <param name="timeout">timeout</param>
        /// <returns></returns>
        static Socket Connect(string host, int timeout)
        {
            // look up host's domain name to find IP address        
            IPHostEntry hostEntry = Dns.GetHostEntry(host);
            // extract a returned address         
            IPAddress hostAddress = hostEntry.AddressList[0];
            IPEndPoint remoteEndPoint = new IPEndPoint(hostAddress, 80);
            Debug.Print("Attempting connection to PushingBox...");
            var connection = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            connection.Connect(remoteEndPoint);
            connection.SetSocketOption(SocketOptionLevel.Tcp,
                SocketOptionName.NoDelay, true);
            connection.SendTimeout = timeout;
            return connection;
        }

        /// <summary>
        /// Sends PushingBox request.
        /// </summary>
        /// <param name="s">Socket server</param>
        /// <param name="devId">PushingBox Device ID</param>
        /// <param name="content">PushingBox status update.  Data encoded as simple HTTP from encoded variables.</param>
        static void sendToPushingBox(Socket s, string devId, string content)
        {
            byte[] contentBuffer = Encoding.UTF8.GetBytes(content);
            var headers =
                "GET /pushingbox?devid=" + devId + " HTTP/1.1" + CRLF +
                "Host: " + serverName + CRLF +
                "User-Agent: NetduinoPlus" + CRLF +
                "Content-Length: " + contentBuffer.Length + CRLF + CRLF;
            byte[] headersBuffer = Encoding.UTF8.GetBytes(headers);
            s.Send(headersBuffer);
            s.Send(contentBuffer);
        }

        /// <summary>
        /// Receives PushingBox response.
        /// </summary>
        /// <param name="s">socket</param>
        static void RecieveResponse(Socket s)
        {
            var buffer = new byte[12];
            var i = 0;

            while (i != 12)
            {
                int read = s.Receive(buffer, i, 1, SocketFlags.None);
                i = i + 1;
            }

            string received = new string(Encoding.UTF8.GetChars(buffer));
            Debug.Print("Response: " + received);
        }

        #endregion


    }
}
