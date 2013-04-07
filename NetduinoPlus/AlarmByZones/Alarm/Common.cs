/* Common.cs
 * 
 * Copyright (c) 2012 by Gilberto García, twitter @ferraripr
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
using System.Collections;
using System.Text;
using Microsoft.SPOT;

namespace AlarmByZones.Alarm
{
    public class Common
    {
        /// <summary>
        /// Common Security System data
        /// </summary>
        public class Alarm_Constants
        {
            /// <summary>
            /// Alarm monitor delay in milliseconds.
            /// </summary>
            public const int ALARM_DELAY_TIME = 500;

            /// <summary>
            /// Exit delay between 30 to 45 seconds.
            /// </summary>
            public const int EXIT_DELAY_TIME = 30000;

            /// <summary>
            /// Entry delay recommended by DSC is 10 seconds.
            /// </summary>
            public const int ENTRY_DELAY_TIME = 10000;
        }

        /// <summary>
        /// Alarm and zone information, and activity events
        /// </summary>
        public class Alarm_Info
        {
            /// <summary>
            /// Stores alarm activity events
            /// </summary>
            public static StringBuilder sbActivity = new StringBuilder();

            /// <summary>
            /// Active alarm zone description
            /// </summary>
            public static Hashtable zoneDescription = new Hashtable();

            /// <summary>
            /// Motion sensor zone Description table
            /// </summary>
            public static Hashtable sensorDescription = new Hashtable();
        }

    }
}
