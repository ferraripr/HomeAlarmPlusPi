//Based on: http://bansky.net/blog/2008/08/sending-e-mails-from-net-micro-framework/comments.html

using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Bansky.SPOT.Mail;
using AlarmByZones;

namespace SMTPClient
{
    public class Email
    {

        #region Constructor
        private string _host;
        private int _port;   
        private string _smtpUserName;
        private string _destinationAddress;
        private string _smtpPassword;

        /// <summary>
        /// Initializes Email
        /// </summary>
        /// <param name="host">server hostname</param>
        /// <param name="port">port to be used on the host</param>
        /// <param name="from">Address information of the message sender.</param>
        /// <param name="to">Address that message is sent to.</param>
        /// <param name="smtpPassword">SMTP Password</param>
        public Email(string host, int port, string from, string to, string smtpPassword)
        {
            _host = host;
            _port = port;
            _smtpUserName = from;
            _destinationAddress = to;
            _smtpPassword = smtpPassword;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Sends email
        /// </summary>
        /// <param name="Subject">Mail Subject</param>
        /// <param name="Body">Message Body</param>
        public void SendEmail(string Subject, string Body)
        {
            try
            {
                using (SmtpClient smtp = new SmtpClient(_host,_port))
                {
                    // Create message
                    MailMessage message = new MailMessage(_smtpUserName,
                        _destinationAddress, Subject, Body);

                    // Authenticate to server
                    smtp.Authenticate = true;
                    smtp.Username = _smtpUserName;
                    smtp.Password = _smtpPassword;

                    // Send message
                    smtp.Send(message);                   
                }
            }
            catch (SmtpException e)
            {
                // Exception handling here                 
                Console.DEBUG_ACTIVITY(e.Message);
                Console.DEBUG_ACTIVITY("Error Code: " + e.ErrorCode.ToString());

                if (AlarmByZones.AlarmByZones.SdCardEventLogger.IsSDCardAvailable() && AlarmByZones.Alarm.ConfigDefault.Data.STORE_EXCEPTION)
                {
                    AlarmByZones.AlarmByZones.SdCardEventLogger.saveFile(DateTime.Now.ToString("d_MMM_yyyy--HH_mm_ss") + ".log", 
                        "Exception trying to send an email.\nErrorCode: " + e.ErrorCode.ToString()+
                        "\nException Message: " + e.Message, "Exception");
                }
            }
        }
        #endregion

    }
}