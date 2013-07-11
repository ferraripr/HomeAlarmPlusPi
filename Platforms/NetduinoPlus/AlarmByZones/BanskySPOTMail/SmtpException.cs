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
    /// The exception that is thrown when SMTP operation failed.
    /// </summary>
    public class SmtpException : Exception
    {
        /// <summary>
        /// Initialize instance of SmtpException.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="smtpError">Code of the exception.</param>
        public SmtpException(string message, SmtpErrorCode smtpError) 
            : base(message)
        {            
            this.ErrorCode = smtpError;
        }

        /// <summary>
        /// Initialize instance of SmtpException.
        /// </summary>
        /// <param name="smtpError">Code of the exception.</param>
        public SmtpException(SmtpErrorCode smtpError)
        {
            this.ErrorCode = smtpError;
        }

        /// <summary>
        /// Initialize instance of SmtpException.
        /// </summary>
        /// <param name="smtpError">Code of the exception.</param>
        /// <param name="innerException">Inner exception wrapped by SmtpException</param>
        public SmtpException(SmtpErrorCode smtpError, Exception innerException) 
            : base(string.Empty, innerException)
        {
            this.ErrorCode = smtpError;            
        }

        /// <summary>
        /// Code of the exception.
        /// </summary>
        public SmtpErrorCode ErrorCode;
    }
}
