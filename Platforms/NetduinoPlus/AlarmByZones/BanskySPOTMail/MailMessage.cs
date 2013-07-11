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
using System.Collections;

namespace Bansky.SPOT.Mail
{
    /// <summary>
    /// Represents an e-email message that can be send using the Bansky.SPOT.Mail.SmtpClient
    /// </summary>
    public class MailMessage
    {
        /// <summary>
        /// Initializes an empty instance of the Bansky.SPOT.Mail.MailMessage.
        /// </summary>
        public MailMessage()
        {
            To = new ArrayList();
            Cc = new ArrayList();
            Bcc = new ArrayList();
            Attachments = new ArrayList();

            Subject = string.Empty;
            Body = string.Empty;
            Headers = string.Empty;

            IsBodyHtml = false;
        }        

        /// <summary>
        /// Initializes new instance of the Bansky.SPOT.Mail.MailMessage.
        /// </summary>
        /// <param name="from">Address information of the message sender.</param>
        /// <param name="to">Address that message is sent to.</param>
        /// <param name="subject">Subject line of the message.</param>
        /// <param name="body">Contains the message body.</param>
        /// <exception cref="ArgumentNullException">from or to is null reference</exception>
        /// <exception cref="ArgumentException">from or to is String.Empty ("")</exception>
        public MailMessage(string from, string to, string subject, string body)
            : this()
        {
            if (from == null || to == null)
                throw new ArgumentNullException();

            if (from == string.Empty || to == string.Empty)
                throw new ArgumentException();

            this.From = new MailAddress(from);
            this.To.Add(new MailAddress(to));
            this.Subject = subject;
            this.Body = body;
        }

        /// <summary>
        /// Address collection that contains the recipients of this e-mail message.
        /// ArrayList of Bansky.SPOT.Mail.MailAddress
        /// </summary>
        public ArrayList To;
        /// <summary>
        /// Address collection that contains the carbon copy (CC) recipients of this e-mail message.
        /// ArrayList of Bansky.SPOT.Mail.MailAddress
        /// </summary>
        public ArrayList Cc;
        /// <summary>
        /// Address collection that contains the blind carbon copy (BCC) recipients of this e-mail message.
        /// ArrayList of Bansky.SPOT.Mail.MailAddress
        /// </summary>
        public ArrayList Bcc;
        /// <summary>
        /// Attachments collection used to store data attached to this e-mail message.
        /// ArrayList of Bansky.SPOT.Mail.Attachment
        /// </summary>
        public ArrayList Attachments;

        /// <summary>
        /// From address for this e-mail message.
        /// </summary>
        public MailAddress From;
        /// <summary>
        /// Message body.
        /// </summary>
        public string Body;
        /// <summary>
        /// Subject line of this e-mail message.
        /// </summary>
        public string Subject;
        /// <summary>
        /// E-mail headers that are transmited with this e-mail message.
        /// </summary>
        /// <example>
        /// This code will set name of the client and message priority
        /// <code>
        ///	MailMessage message = new MailMessage("john@doe.com",
        ///										  "foo@bar.net",
        ///										  "Good news",
        ///										  "How are you Foo?");        
        /// 
        /// message.Headers = "X-Priority: 1\r\n";
        /// message.Headers += "X-MSMail-Priority: High\r\n";
        /// message.Headers += "X-Mailer: Micro Framework mail sender\r\n";
        /// </code>
        /// </example>
        public string Headers;
        /// <summary>
        /// Indicated whether the mail message body is in Html.
        /// </summary>
        public bool IsBodyHtml;    
    }
}
