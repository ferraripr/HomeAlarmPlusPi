/* ConfigDefault.cs
 * 
 * Copyright (c) 2014 by Gilberto García, twitter @ferraripr
 * 
 * A simple alarm monitoring system using a typical alarm panel.  This implementation
 * could be used in conjunction with the P C 5 0 1 0 - Digital Security Controls (DSC) 
 * ProwerSeries Security System Control Panel and sensors .
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
    class ConfigDefault
    {
        public class Data
        {
            /// <summary>
            /// Http server port number
            /// </summary>
            public static int Http_port = 8080;

            /// <summary>
            /// Email alert every X amount of minutes
            /// </summary>
            public static int EMAIL_FREQUENCY = 10;

            /// <summary>
            /// Stores Logs on SD Card
            /// </summary>
            public static bool STORE_LOG = true;

            /// <summary>
            /// Stores Exceptions on SD Card
            /// </summary>
            public static bool STORE_EXCEPTION = true;

            /// <summary>
            /// Option to whether or not use email.
            /// </summary>
            public static bool USE_EMAIL = true;

            /// <summary>
            /// Option to whether or not use Pachube.
            /// </summary>
            public static bool USE_PACHUBE = false;

            /// <summary>
            /// Web server address
            /// </summary>
            public static string HTTP_HOST = "127.0.0.1";

            /// <summary>
            /// Sync weather data every X amount of hours
            /// </summary>
            public static int WUNDERGROUND_SYNC_FREQUENCY = 1;
        }
    }
}
