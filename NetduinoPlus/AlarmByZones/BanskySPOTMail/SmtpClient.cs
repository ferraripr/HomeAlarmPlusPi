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
using System.Net;
using System.Net.Sockets;
using System.Collections;
using Microsoft.SPOT.Hardware;

namespace Bansky.SPOT.Mail
{
    /// <summary>
    /// Allows application to send e-mail by using Simple Mail Transfer Protocol (SMTP)
    /// </summary>
    public class SmtpClient : IDisposable
    {
        /// <summary>
        /// Initializes new instance of Bansky.SPOT.Mail.SmtpClient.
        /// </summary>
        /// <param name="host">server hostname.</param>
        /// <param name="port">port to be used on the host.</param>
        public SmtpClient(string host, int port)
        {
            this.Domain = "localhost";
            this.Host = host;
            this.Port = port;
        }        

        /// <summary>
        /// Dispose the SmtpClient
        /// </summary>
        public void Dispose()
        {            
            socket = null;
        }        

        /// <summary>
        /// Sends the specified message to an SMTP server for delivery.
        /// </summary>
        /// <param name="message">Bansky.SPOT.Mail.MailMessage that contains message to send.</param>
        /// <exception cref="ArgumentNullException">Message or From is null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">There are no recipients in To, CC, and Bcc.</exception>
        /// <exception cref="SmtpException">The connection or session failed.</exception>
        /// <example>
        /// This code sends e-mail message and use authentication to server
        /// <code>
        ///	using (SmtpClient smtp = new SmtpClient("smtp.hostname.net", 25))
        ///	{
        ///		// Create message
        ///		MailMessage message = new MailMessage("john@doe.com",
        ///											  "foo@bar.net",
        ///											  "Good news",
        ///											  "How are you Foo?");
        ///
        ///		// Authenicate to server
        ///		smtp.Authenticate = true;
        ///		smtp.Username = "userlogin";
        ///		smtp.Password = "userpassword";
        ///
        ///		// Send message
        ///		smtp.Send(message);
        ///	}
        /// </code>
        /// This code sends HTML formated e-mail message with attachment to more recipients.
        /// <code>
        ///	MailMessage message = new MailMessage();
        ///	// Set sender name and address
        ///	message.From = new MailAddress("foobar@contoso.com", "Foo Bar");
        ///	
        ///	// Set recipients
        ///	message.To.Add(new MailAddress("john.doe@customer.com", "John Doe"));
        ///	message.Cc.Add(new MailAddress("manager@contoso.com"));
        ///	
        ///	message.Subject = "Hello World";
        ///	message.Body = "Good news,<br />from now on you can send e-mails from <b>.NET Micro Framework</b>.";
        ///	// Format body as HTML
        ///	message.IsBodyHtml = true;
        ///	
        ///	// Create new attachment and define it's name
        ///	Attachment attachment = new Attachment("Snwoflake.gif");        
        ///	attachment.ContentType = "image/gif";
        ///	attachment.TransferEncoding = TransferEncoding.Base64;
        ///	// Attachment content
        ///	attachment.Content = Base64.Encode( Resources.GetBytes(
        ///	                                    Resources.BinaryResources.Snowflake_gif),
        ///	                                    true);
        ///	
        ///	// Add attachment to message
        ///	message.Attachments.Add(attachment);
        ///	
        ///	// Create new SMTP instance
        ///	SmtpClient smtp = new SmtpClient("smtp.contoso.com", 25);
        ///	try
        ///	{
        ///		// Authenicate to server
        ///		smtp.Authenticate = true;
        ///		smtp.Username = "userlogin";
        ///		smtp.Password = "userpassword";
        ///	
        ///		// Send message
        ///		smtp.Send(message);
        ///	}
        ///	catch (SmtpException e)
        ///	{
        ///		// Exception handling here 
        ///		Debug.Print(e.Message);
        ///		Debug.Print("Error Code: " + e.ErrorCode.ToString());
        ///	}
        ///	finally
        ///	{
        ///		smtp.Dispose();
        ///	}
        /// </code>
        /// </example>
        public void Send(MailMessage message)
        {
            if (message == null)
                throw new ArgumentNullException();

            if (message.To.Count == 0
                && message.Cc.Count == 0 
                && message.Bcc.Count == 0)
                    throw new ArgumentOutOfRangeException();

            try
            {
                // Get server's IP address.            
                IPHostEntry hostEntry = Dns.GetHostEntry(this.Host);
                // Create socket and connect to the server's IP address and port
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(new IPEndPoint(hostEntry.AddressList[0], this.Port));
            }
            catch (Exception e)
            {
                throw new SmtpException(SmtpErrorCode.ConnectionFailed, e);
            }

            GetResponse("220", SmtpErrorCode.BadResponse);
            
            if (Authenticate)
                DoAuthentication(Username, Password);
            else
                SendCommand("HELO " + Domain, "250", SmtpErrorCode.BadResponse);

            SendCommand("MAIL From:<" + message.From.Address +">", "250", SmtpErrorCode.FromFailed);

            if (message.To.Count > 0)
                DoRcpt("To", message.To);

            if (message.Cc.Count > 0)
                DoRcpt("To", message.Cc);

            if (message.Bcc.Count > 0)
                DoRcpt("To", message.Bcc);

            SendCommand("DATA", "354", SmtpErrorCode.DataFailed);
            SendCommand(ProcessMessage(message) + "\r\n.\r\n", "250", SmtpErrorCode.DataFailed);

            SendCommand("QUIT", "221", SmtpErrorCode.DataFailed);

            socket.Close();
        }

        /// <summary>
        /// Sends the specified message to an SMTP server for delivery.
        /// </summary>
        /// <param name="from">Address information of the message sender.</param>
        /// <param name="to">Address that message is sent to.</param>
        /// <param name="subject">Subject line of the message.</param>
        /// <param name="body">Contains the message body.</param>
        /// <exception cref="ArgumentNullException">from or to is null reference</exception>
        /// <exception cref="ArgumentException">from or to is String.Empty ("")</exception>        
        /// <exception cref="SmtpException">connection or session errors</exception>
        /// <example>
        /// This code send simple e-mail message.
        /// No authentication to server is used.
        /// <code>
        /// using (SmtpClient smtp = new SmtpClient("smtp.hostname.net", 25))
        ///	{
        ///		// Send message
        ///		smtp.Send("john@doe.com",
        ///				  "foo@bar.net",
        ///				  "Good news",
        ///				  "How are you Foo?");
        ///	}
        /// </code>
        /// </example>
        public void Send(string from, string to, string subject, string body)
        {
            MailMessage msg = new MailMessage(from, to, subject, body);
            Send(msg);
        }

        /// <summary>
        /// Sends SMTP command or data through the socket.
        /// </summary>
        /// <param name="command">Data to be send.</param>
        private void SendCommand(string command)
        {            
            if (!StrEndWith(command, "\r\n"))
                command += "\r\n";

            byte[] buffer = Encoding.UTF8.GetBytes(command);

            socket.Send(buffer);            
        }

        /// <summary>
        /// Sends SMTP command or data through the socket.
        /// Waits for server response and evaluate it.
        /// </summary>
        /// <param name="command">Data to be send.</param>
        /// <param name="expectedReply">Expected SMTP reply code.</param>
        /// <param name="errorCode">Error code to be used in exception, when reply code do not match.</param>
        /// <returns>Complete response from server.</returns>
        private string SendCommand(string command, string expectedReply, SmtpErrorCode errorCode)
        {
            SendCommand(command);
            return GetResponse(expectedReply, errorCode);
        }

        /// <summary>
        /// Reads SMTP responses through socket.
        /// </summary>
        /// <returns>Server response.</returns>
        private string GetResponse()
        {
            //AlarmByZones modification
            //const int c_microsecondsPerMilisecond = 1000;
            const int c_microsecondsPerMilisecond = 1000000;

            // According to RFC the maximum response length must be 512 bytes
            // http://www.freesoft.org/CIE/RFC/821/24.htm
            byte[] buffer = new byte[512];
            string response = string.Empty;            

            int timeOut = this.Timeout * c_microsecondsPerMilisecond;
            // Poll for data until time out
            while (socket.Poll(timeOut, SelectMode.SelectRead))
            {            
                int bytesRead = socket.Receive(buffer);
                if (bytesRead == 0)
                    throw new SmtpException(SmtpErrorCode.ConnectionClosed);
                
                char[] chars = Encoding.UTF8.GetChars(buffer);
                
                // this is for performance tuning of the loop
                int len = chars.Length;
                for (int i = 0; i < len; i++)
                    response += chars[i];

                if (StrEndWith(response, "\r\n"))
                    break;
            }            

            return response;
        }

        /// <summary>
        /// Reads SMTP responses through the socket and evaluate it.
        /// </summary>
        /// <param name="expectedReply">Expected SMTP reply code.</param>
        /// <param name="errorCode">Error code to be used in exception, when reply code do not match.</param>
        /// <returns>Server resposne.</returns>
        private string GetResponse(string expectedReply, SmtpErrorCode errorCode)
        {
            string response = GetResponse();            

            if (GetReplyCode(response) != expectedReply)
                throw new SmtpException(response, errorCode);

            return response;
        }

        /// <summary>
        /// Parses reply code from response.
        /// </summary>
        /// <param name="response">Response</param>
        /// <returns>Reply code.</returns>
        private string GetReplyCode(string response)
        {
            response = response.Trim();
            if (response.Length >= 3)
                response = response.Substring(0, 3);

            return response;
        }
        
        /// <summary>
        /// Sends recipient command and data to the server.
        /// </summary>
        /// <param name="rcptType">Type of RCPT: To, Cc, Bcc.</param>
        /// <param name="recipients">ArrayList of Bansky.SPOT.Mail.MailAddress with recipients.</param>
        private void DoRcpt(string rcptType, ArrayList recipients)
        {
            int len = recipients.Count;
            for(int i=0; i < len; i++)
            {
                MailAddress address = (MailAddress)recipients[i];
                SendCommand("RCPT " + rcptType + ":<" + address.Address + ">");
                
                string replyCode = GetReplyCode(GetResponse());
                if (replyCode != "250" && replyCode != "251")
                    throw new SmtpException(replyCode, SmtpErrorCode.RcptFailed);
            }
        }

        /// <summary>
        /// Does authentication to server.
        /// Gets available authentication methods and use on of them.        
        /// </summary>
        /// <exception cref="NotSupportedException">When LOGIN or PLAIN method are not supported.</exception>
        /// <param name="userName">Username</param>
        /// <param name="passWord">Password</param>
        private void DoAuthentication(string userName, string passWord)
        {
            string response = SendCommand("EHLO " + Domain, "250", SmtpErrorCode.BadResponse);

            // Parse supported authentication methods
            string[] lines = response.Split('\n');
            for(int i=0; i < lines.Length; i++)
            {
                // Find the line with AUTH infomation
                string line = lines[i].ToUpper();
                if (line.Substring(4, 4) == "AUTH")
                {
                    // Authetnicate with one of the methods
                    if (line.IndexOf("LOGIN") > -1)
                    {
                        DoAuthLogin(userName, passWord);
                        break;
                    }
                    else if (line.IndexOf("PLAIN") > -1)
                    {
                        DoAuthPlain(userName, passWord);
                        break;
                    }
                    else
                        throw new SmtpException(SmtpErrorCode.AuthNotSupported);
                }
            }
        }

        /// <summary>
        /// Does AUTH PLAIN authentication.
        /// </summary>
        /// <param name="userName">Username</param>
        /// <param name="passWord">Password</param>
        private void DoAuthPlain(string userName, string passWord)
        {
            /* This is awful workaround. 
             * .NET Micro Framework 2.5 has bug in string interpretation.
             * \0 char is intrepreted as end of the string instead of \0 char
             * 
             * That's why the \0 chars must be included directly into the byte array.
             * 
             * One day the authString will be someting like this
             * authString = "AUTH PLAIN "+Base64.Encode("\0" + userName + "\0" + passWord, false);
             */

            byte[] userBytes = UTF8Encoding.UTF8.GetBytes(userName);
            int userLen = userBytes.Length;

            byte[] passBytes = UTF8Encoding.UTF8.GetBytes(passWord);
            byte[] login = new byte[userLen + passBytes.Length + 2];            

            login[0] = 0;
            login[userLen] = 0;
            Array.Copy(userBytes, 0, login, 1, userLen);
            Array.Copy(passBytes, 0, login, userLen + 2, passBytes.Length);
                        
            SendCommand("AUTH PLAIN " + Base64.Encode(login, false),
                        "235",
                        SmtpErrorCode.AuthFailed);
        }

        /// <summary>
        /// Does AUTH LOGIN authentication.
        /// </summary>
        /// <param name="userName">Username</param>
        /// <param name="passWord">Password</param>
        private void DoAuthLogin(string userName, string passWord)
        {
            SendCommand("AUTH LOGIN", "334", SmtpErrorCode.BadResponse);
            SendCommand(Base64.Encode(userName, false), "334", SmtpErrorCode.BadResponse);
            SendCommand(Base64.Encode(passWord, false), "235", SmtpErrorCode.AuthFailed);
        }

        /// <summary>
        /// Creates message body from MailMessage.
        /// </summary>
        /// <param name="message">MailMessage to be processed.</param>
        /// <returns>String suitable for SMTP transaction.</returns>
        private string ProcessMessage(MailMessage message)
        {
            string result = "Subject: " + message.Subject + "\r\n";
            result += "From: ";
            if (message.From.DisplayName != null)
                result += message.From.DisplayName + " <" + message.From.Address + ">";
            else
                result += message.From.Address;

            result += "\r\n";

            result += ProcessRcpt("To", message.To);
            result += ProcessRcpt("Cc", message.Cc);
            result += ProcessRcpt("Bcc", message.Bcc);

            DateTime now = DateTime.Now;
            result += "Date: " + now.ToString("ddd, d MMM yyyy HH:mm:ss \r\n");

            result += message.Headers;            
            if (message.Headers.Length > 0 && !StrEndWith(message.Headers, "\r\n"))
                result += "\r\n";

            // We need special header when attachments are present
            if (message.Attachments.Count > 0)
            {
                result += "MIME-Version: 1.0\r\n";
                result += "Content-Type: multipart/mixed; boundary=\"frontier\"\r\n";
                result += "\r\n--frontier\r\n";
                result += ProcessBody(message);

                // Add all attachements
                foreach (Attachment attachment in message.Attachments)
                {
                    result += "\r\n--frontier\r\n";
                    result += "Content-type: " + attachment.ContentType;
                    result += ";\r\n\tname=\"" + attachment.Name + "\"\r\n";
                    
                    result += "Content-Transfer-Encoding: ";
                    switch (attachment.TransferEncoding)
                    {
                        case TransferEncoding.QuotedPrintable:
                            result += "quoted-printable";
                            break;
                        case TransferEncoding.Base64:
                            result += "base64";
                            break;
                        case TransferEncoding.SevenBit:
                            result += "7bit";
                            break;
                        default:
                            result += "unknown";
                            break;
                    }
                    result += "\r\n";

                    if (attachment.ContentId != null)
                        result += "Content-ID: <" + attachment.ContentId + ">\r\n"; 

                    result += "Content-Disposition: ";
                    result += (attachment.Inline) ? "inline" : "attachment";
                    result += ";\r\n\tfilename=\"" + attachment.Name + "\"\r\n\r\n";
                
                    result += attachment.Content;
                }

                result += "\r\n--frontier--\r\n";
            }
            else            
                result += ProcessBody(message);            

            // Double all dots after line-breaks
            int i = 0;
            while ((i = result.IndexOf("\r\n.", i)) > -1)
            {
                result = result.Substring(0, i) + "." + result.Substring(i);
                i += 3;
            }
            
            return result;
        }

        /// <summary>
        /// Creates list of recipients for message header.
        /// </summary>
        /// <param name="rcptType">Type of RCPT: To, Cc, Bcc.</param>
        /// <param name="recipients">ArrayList of Bansky.SPOT.Mail.MailAddress with recipients.</param>
        /// <returns></returns>
        private string ProcessRcpt(string rcptType, ArrayList recipients)
        {
            string result = string.Empty;
            int len = recipients.Count;
            if (len > 0)
            {
                result += rcptType + ": ";
                for (int i = 0; i < len; i++)
                {
                    MailAddress address = (MailAddress)recipients[i];
                    string rcpt;
                    if (address.DisplayName != null)
                        rcpt = address.DisplayName + " <" + address.Address + ">";
                    else
                        rcpt = address.Address;
                    
                    result += rcpt;
                    result += (i < len - 1) ? "," : "";
                }
                result += "\r\n";
            }

            return result;
        }

        /// <summary>
        /// Creates message body wrapped in the appropriate content type header.
        /// </summary>
        /// <param name="message">MailMessage to process.</param>
        /// <returns>Message body.</returns>
        private string ProcessBody(MailMessage message)
        {
            string result = string.Empty;
            if (message.IsBodyHtml)
                result += "Content-Type: text/html";
            else
                result += "Content-Type: text/plain";

            return result + ";\r\n\tcharset=\"UTF-8\"\r\n\r\n" + message.Body;
        }

        /// <summary>
        /// Test whether the string ends with the given pattern.
        /// </summary>
        /// <param name="data">String to be tested.</param>
        /// <param name="pattern">Matching pattern.</param>
        /// <returns>True when data ends with the pattern</returns>
        private bool StrEndWith(string data, string pattern)
        {
            int dataLen = data.Length;
            int patLen = pattern.Length;

            if (dataLen < patLen || data.Substring(dataLen - patLen) != pattern)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Socket timeout in miliseconds
        /// </summary>
        public int Timeout = 15000;
        /// <summary>
        /// Domain to be send in HELO / EHLO command
        /// </summary>
        public string Domain;
        /// <summary>
        /// Smtp server host name
        /// </summary>
        public string Host;
        /// <summary>
        /// Auth username
        /// </summary>
        public string Username;
        /// <summary>
        /// Auth password
        /// </summary>
        public string Password;
        /// <summary>
        /// Smtp server port
        /// </summary>
        public int Port;
        /// <summary>
        /// Determines whether the authenication inromation is required
        /// </summary>
        public bool Authenticate;

        private Socket socket;
    }
}
