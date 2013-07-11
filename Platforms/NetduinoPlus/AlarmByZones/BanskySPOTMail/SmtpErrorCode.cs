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
    /// Specifies the errors of sending e-mail by using the Bansky.SPOT.Mail.SmtpClient class 
    /// </summary>
    public enum SmtpErrorCode
    {
        /// <summary>
        /// Server send unexpected response.
        /// </summary>
        BadResponse,
        /// <summary>
        /// Server has no support for LOGIN or PLAIN authentication method.
        /// </summary>
        AuthNotSupported,
        /// <summary>
        /// Authentication process failed.        
        /// </summary>
        AuthFailed,
        /// <summary>
        /// Server refused the sender.
        /// </summary>        
        FromFailed,
        /// <summary>
        /// Server refused the recipients.
        /// </summary>
        RcptFailed,
        /// <summary>
        /// Server refused the data.
        /// </summary>
        DataFailed,
        /// <summary>
        /// Connection closed by server.
        /// </summary>
        ConnectionClosed,
        /// <summary>
        /// Socket error while connecting to the server.
        /// </summary>
        ConnectionFailed
    }
}
