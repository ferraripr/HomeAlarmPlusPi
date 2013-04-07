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

namespace Bansky.SPOT.Mail
{
    /// <summary>
    /// Represents an attachment to an e-mail.
    /// </summary>
    public class Attachment
    {
        /// <summary>
        /// Initializes new instance of Bansky.SPOT.Mail.Attachment
        /// </summary>
        /// <param name="name">Attachment name as it appears in the e-mail message.</param>
        /// <exception cref="ArgumentNullException">name is null reference</exception>
        /// <exception cref="ArgumentException">name is String.Empty ("")</exception>
        public Attachment(string name)
        {
            if (name == null)
                throw new ArgumentNullException();

            if (name == string.Empty)
                throw new ArgumentException();

            this.Name = name;
            this.Inline = false;
            this.TransferEncoding = TransferEncoding.Unknown;
            this.ContentType = "application/octet-stream";
        }

        /// <summary>
        /// Gets or sets the encoding of this attachment.
        /// </summary>
        public TransferEncoding TransferEncoding;
        /// <summary>
        /// Content type of this attachment.
        /// Default is application/octet-stream
        /// </summary>
        public string ContentType;
        /// <summary>
        /// Gets or sets the MIME content ID for this attachment.
        /// </summary>
        public string ContentId;
        /// <summary>
        /// Name of this attachment as it appears in the e-mail message.
        /// </summary>
        public string Name;
        /// <summary>
        /// Encoded content of the attachment.
        /// </summary>
        public string Content;
        /// <summary>
        /// Gets or sets a Boolean value that determines the disposition type (Inline or Attachment) for an e-mail attachment.
        /// </summary>
        public bool Inline;
    }
}
