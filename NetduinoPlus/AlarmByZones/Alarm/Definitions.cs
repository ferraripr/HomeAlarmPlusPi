/* Definitions.cs
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
using Microsoft.SPOT;

namespace AlarmByZones.Alarm
{
    class User_Definitions
    {
        public class Constants
        {
            /// <summary>
            /// Quantity of active alarm zones
            /// </summary>
            public const int ACTIVE_ZONES = 4;

            /// <summary>
            /// Quantity of motion detector sensors
            /// </summary>
            public const int MOTION_SENSORS = 1;

            /// <summary>
            /// Alarm and sensor XML config file full path
            /// </summary>
            public const string ALARM_CONFIG_FILE_PATH = @"\SD\Config.ini";
        }
    }
}
