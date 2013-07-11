/* UserData.cs
 * 
 * Copyright (c) 2012 by Gilberto García, twitter @ferraripr
 *
 * A simple alarm monitoring system using a typical alarm panel.  This implementation
 * could be used in conjunction with the P C 5 0 1 0 - Digital Security Controls (DSC) 
 * ProwerSeries Security System Control Panel and sensors .
 * 
 * Implementation based on Pavel Bánský BanskySPOTMail (http://bansky.net/blog/2008/08/e-mail-class-library-update-1/). 
 *
 * This code was written by Gilberto García. It is released under the terms of 
 * the Creative Commons "Attribution 3.0 Unported" license.
 * http://creativecommons.org/licenses/by/3.0/
 * 
 * WARNING: This code contains information related to typical home alarm panels.  Please, be aware that
 * this procedure may void any warranty.
 * Any alarm system of any type may be compromised deliberately or may fail to
 * operate as expected for a variety of reasons.
 * The author, G. García, is not liable for any System Failures such as: inadequate installation, 
 * criminal knowledge, access by intruders, power failure, failure of replaceable batteries,
 * compromise of Radio Frequency (Wireless) devices, system users, smoke detectors, motion
 * detectors, warning devices (sirens, bells, horns), telephone lines, insufficient time, component
 * failure, inadequate testing, security and insurance (property or life insurance).
 * 
 * *** DISCONNECT AC POWER AND TELEPHONE LINES PRIOR TO DOING ANYTHING.
 */

using System;
using Microsoft.SPOT;

namespace AlarmByZones.Alarm
{
    public class UserData
    {
        /// <summary>
        /// Email user data class
        /// </summary>
        public class Email
        {
            /// <summary>
            /// server hostname
            /// </summary>
            public const string host = "mail.host";

            /// <summary>
            /// port to be used on the host
            /// </summary>
            public const int port = 587;

            /// <summary>
            /// Address information of the message sender.
            /// </summary>
            public const string From = "johndoe@gmail.com";

            /// <summary>
            /// Address that message is sent to
            /// </summary>
            public const string To = "janedoe@gmail.com";

            /// <summary>
            /// SMTP Password
            /// </summary>
            public const string smtpPassword = "thePassword";
        }

        /// <summary>
        /// Pachube user data class
        /// </summary>
        public class Pachube
        {
            //public const string apiKey = "your Pachube API key";         
            //public const string feedId = "your Pachube feed id";

            /// <summary>
            /// Pachube API key
            /// </summary>
            public const string apiKey = "nXXXXXXXXXXXXXXXXXXXXXXXXXX";

            /// <summary>
            /// Pachube Feed ID
            /// </summary>
            public const string feedId = "12345";
        }
    }
}
