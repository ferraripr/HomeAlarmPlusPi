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
    /// Represents the address of an electronic mail sender or recipient.
    /// </summary>
    public class MailAddress
    {
        /// <summary>
        /// Initializes a new instance of the MailAddress class using the specified address. 
        /// </summary>
        /// <param name="address">A String that contains an e-mail address.</param>
        public MailAddress(string address)
        {
            this.Address = address;
        }

        /// <summary>
        /// Initializes a new instance of the MailAddress class using the specified address and display name.
        /// </summary>
        /// <param name="address">A String that contains an e-mail address.</param>
        /// <param name="displayName">A String that contains the display name associated with address. This parameter can be null reference</param>
        public MailAddress(string address, string displayName) 
            : this(address)
        {            
            this.DisplayName = displayName;
        }

        /// <summary>
        /// Gets the e-mail address specified when this instance was created.
        /// </summary>
        public readonly string Address;
        /// <summary>
        /// Gets the display name composed from the display name and address information specified when this instance was created.
        /// </summary>
        public readonly string DisplayName;
    }
}
