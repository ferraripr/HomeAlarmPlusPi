using System;
using Microsoft.SPOT;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Pachube
{
    public class PachubeLibrary
    {
        #region Constructor
        public PachubeLibrary()
        {

        }
        #endregion

        #region Declarations
        public static bool IsSDCardAvailable = false;

        public static bool STORE_PACHUBE_EXCEPTION = false;

        /// <summary>
        /// Total pachube feeds
        /// <remarks>if more than one, need to declare additional feedID and apiKey</remarks>
        /// </summary>
        const int PACHUBE_FEEDS = 1;

        /// <summary>
        /// Sampling period in seconds
        /// </summary>
        const int SAMPLING_PERIOD = 20;

        /// <summary>
        /// Gets the total elapsed time measured by the current instance of each Pachube feed.
        /// </summary>
        static System.Diagnostics.Stopwatch[] stopwatchPachube = new System.Diagnostics.Stopwatch[PACHUBE_FEEDS];

        /// <summary>
        /// Suppress sampling period to update sensor event.
        /// <remarks>Force Pachube's status update.</remarks>
        /// </summary>
        public static bool forceUpdate = false;

        /// <summary>
        /// Carriage return and line feed.
        /// </summary>
        const string CRLF = "\r\n";

        /// <summary>
        /// Contains Pachube status
        /// </summary>
        public static string statusToPachube = string.Empty;
        #endregion

        #region Methods
        public static void InitPachubleLibrary(bool isSDAvailable, bool storePachubeException)
        {
            IsSDCardAvailable = isSDAvailable;
            STORE_PACHUBE_EXCEPTION = storePachubeException;
        }
        /// <summary>
        /// Pachube connect method.
        /// </summary>
        public static void PachubeConnect()
        {
            Socket connection = null;
            stopwatchPachube[0] = System.Diagnostics.Stopwatch.StartNew();

            while (true)
            {
                double eSeconds = stopwatchPachube[0].ElapsedSeconds;
                Debug.Print("stopwatchPachube[0] Elapsed time = " + eSeconds.ToString() + " seconds");

                //Time is up to update status to Pachube or alarm was trigger
                if (forceUpdate || eSeconds >= SAMPLING_PERIOD)
                {
                    Debug.Print("PachubeConnect - Memory available: " + Debug.GC(true));
                    if (connection == null)
                    {
                        try
                        {
                            connection = Connect("api.pachube.com", SAMPLING_PERIOD);
                        }
                        catch (Exception ex)
                        {
                            if (IsSDCardAvailable && STORE_PACHUBE_EXCEPTION)
                            {
                                AlarmByZones.AlarmByZones.SdCardEventLogger.saveFile(DateTime.Now.ToString("d_MMM_yyyy--HH_mm_ss") + ".log",
                                    "Connection Error. Exception trying to access pachube on PachubeConnect method." +
                                    "\nException Message: " + ex.Message +
                                    "\nStack Trace: " + ex.StackTrace +
                                    "\nInner Exception: " + ex.InnerException, "Exception");
                            }
                            Debug.Print("Connection Error.\nException: " + ex.Message);
                        }
                        //restart timer
                        stopwatchPachube[0] = System.Diagnostics.Stopwatch.StartNew();

                        //no need to force update unless another alarm/sensor is trigger
                        forceUpdate = false;
                    }
                    if (connection != null)
                    {
                        try
                        {
                            //if Pachube is active, status gets updated here. 
                            // Here is the format
                            // DATASTREAM ID = Zone Alarm #, voltage.
                            //Example: 0,1.67\n\r
                            //         2,1.67\n\r
                            //         3,1.67\n\r
                            //         4,3.37\n\r
                            statusToPachube = statusToPachube == string.Empty ? "0,No Alarms/Sensors to report" : statusToPachube;
                            SendRequest(connection, AlarmByZones.Alarm.UserData.Pachube.apiKey,
                                AlarmByZones.Alarm.UserData.Pachube.feedId, statusToPachube);
                            RecieveResponse(connection);
                        }
                        catch (SocketException ex)
                        {
                            if (IsSDCardAvailable && STORE_PACHUBE_EXCEPTION)
                            {
                                AlarmByZones.AlarmByZones.SdCardEventLogger.saveFile(DateTime.Now.ToString("d_MMM_yyyy--HH_mm_ss") + ".log",
                                    "Connection Error. Exception trying to access pachube on PachubeConnect method (connection !=null)." +
                                    "\nException Message: " + ex.Message +
                                    "\nError Code: " + ex.ErrorCode +
                                    "\nStackTrace: " + ex.StackTrace +
                                    "\nInnerException: " + ex.InnerException, "Exception");
                            }
                            Debug.Print("Socket Exception: " + ex.Message);
                            connection.Close();
                            connection = null;
                        }
                        //restart timer
                        stopwatchPachube[0] = System.Diagnostics.Stopwatch.StartNew();

                        //no need to force update unless another alarm/sensor is trigger
                        forceUpdate = false;
                    }
                }

            }
        }

        /// <summary>
        /// Establishes connection to Pachube's host.
        /// </summary>
        /// <param name="host">port number</param>
        /// <param name="timeout">timeout</param>
        /// <returns></returns>
        static Socket Connect(string host, int timeout)
        {
            // look up host's domain name to find IP address        
            IPHostEntry hostEntry = Dns.GetHostEntry(host);
            // extract a returned address         
            IPAddress hostAddress = hostEntry.AddressList[0];
            IPEndPoint remoteEndPoint = new IPEndPoint(hostAddress, 80);
            Debug.Print("Attempting connection to Pachube...");
            var connection = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            connection.Connect(remoteEndPoint);
            connection.SetSocketOption(SocketOptionLevel.Tcp,
                SocketOptionName.NoDelay, true);
            connection.SendTimeout = timeout;
            return connection;
        }

        /// <summary>
        /// Sends Pachube request.
        /// </summary>
        /// <param name="s">Socket server</param>
        /// <param name="apiKey">Pachube API key</param>
        /// <param name="feedId">Pachube Feed ID</param>
        /// <param name="content">Pachube status update.  Data encoded as simple HTTP from encoded variables.</param>
        static void SendRequest(Socket s, string apiKey, string feedId,
             string content)
        {
            byte[] contentBuffer = Encoding.UTF8.GetBytes(content);
            var requestLine =
                "PUT /v2/feeds/" + feedId + ".csv?key=" + apiKey + " HTTP/1.1" + CRLF;

            byte[] requestLineBuffer = Encoding.UTF8.GetBytes(requestLine);
            var headers =
                "Accept: */*" + CRLF +
                "Host: api.pachube.com" + CRLF +
                "X-PachubeApiKey: " + apiKey + CRLF +
                "Content-Type: text/csv" + CRLF +
                "User-Agent: NetduinoPlus" + CRLF +
                "Content-Length: " + contentBuffer.Length + CRLF + CRLF;
            byte[] headersBuffer = Encoding.UTF8.GetBytes(headers);
            s.Send(requestLineBuffer);
            s.Send(headersBuffer);
            s.Send(contentBuffer);
        }

        /// <summary>
        /// Receives Pachube response.
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
 