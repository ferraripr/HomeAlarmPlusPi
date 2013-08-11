////
//
// General code for http://www.pushover.com for Netduino Plus v1.0
// API: https://pushover.net/api
//
////

using System;
using Microsoft.SPOT;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Notification
{
    public class Pushover
    {
        #region Constructor
        private Pushover()
        {

        }
        #endregion

        #region Declarations

        const string serverName = "rpi.server.address";
        const int serverPort = 8086;

        /// <summary>
        /// Carriage return and line feed.
        /// </summary>
        const string CRLF = "\r\n";

        /// <summary>
        /// Connection status
        /// </summary>
        public static bool bStatus = true;
        #endregion

        #region Methods
        /// <summary>
        /// Connect to Pushover
        /// </summary>
        /// <param name="time">date time stamp</param>
        /// <param name="title">Notification title</param>
        /// <param name="message">Notification message</param>
        /// <param name="alarm">True if alarm</param>
        public static void Connect(string time, string title, string message, bool alarm)
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
                    bStatus = false;
                }
            }
            try
            {
                postToRaspberryPi(connection, time, title, message, "NetduinoPlus", alarm);
                RecieveResponse(connection);
                connection.Close();
                connection = null;
            }
            catch (Exception ex)
            {
                Debug.Print("Socket Exception: " + ex.Message);
                bStatus = false;
            }
        }

        /// <summary>
        /// Establishes connection to Pushover host.
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
            IPEndPoint remoteEndPoint = new IPEndPoint(hostAddress, serverPort);
            Debug.Print("Attempting connection to RaspberryPi...");
            var connection = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            connection.Connect(remoteEndPoint);
            connection.SetSocketOption(SocketOptionLevel.Tcp,
                SocketOptionName.NoDelay, true);
            connection.SendTimeout = timeout;
            return connection;
        }

        /// <summary>
        /// Post Pushover request.
        /// </summary>
        /// <param name="s">Socket server</param>
        /// <param name="time">date time stamp</param>
        /// <param name="title">Notification title</param>
        /// <param name="message">Notification message</param>
        /// <param name="content">Pushover status update.  Data encoded as simple HTTP from encoded variables.</param>
        /// <param name="alarm">True if alarm</param>
        static void postToRaspberryPi(Socket s, string time, string title, string message, string content, bool alarm)
        {
            byte[] contentBuffer = Encoding.UTF8.GetBytes(content);
            var requestLine =
                "PUT /WebResources/alarmParse.php?alarm-description=" + message + 
                "&title=" + title + 
                "&Ntime=" + time +
                "&Alarm=" + alarm.ToString().ToLower() +
                " HTTP/1.1" + CRLF;

            byte[] requestLineBuffer = Encoding.UTF8.GetBytes(requestLine);
            var headers =
                "Accept: */*" + CRLF +
                "Host: rpi.server.address" + CRLF +
                "X-PushoverApi: " + message + CRLF +
                "Content-Type: text/csv" + CRLF +
                "User-Agent: NetduinoPlus" + CRLF +
                "Content-Length: " + contentBuffer.Length + CRLF + CRLF;
            byte[] headersBuffer = Encoding.UTF8.GetBytes(headers);
            s.Send(requestLineBuffer);
            s.Send(headersBuffer);
            s.Send(contentBuffer);
        }

        /// <summary>
        /// Receives Pushover response.
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
