// -----------------------------------------------------------------------
// <copyright file="About.cs" company="Gilberto García">
//  This code was written by Gilberto García. It is released under the terms of 
//  the Creative Commons "Attribution 3.0 Unported" license.
//  http://creativecommons.org/licenses/by/3.0/
//  Copyright (c) 2012 by Gilberto García, twitter @ferraripr
// </copyright>
// -----------------------------------------------------------------------

namespace ResourceGenerator.Htm
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// About.htm HTML file generator
    /// </summary>
    public class About
    {

        /// <summary>
        /// AlarmByZones AssemblyInfo.cs file path
        /// </summary>
        private string assemblyInfoPath = @"..\..\..\Properties\AssemblyInfo.cs";        
        
        /// <summary>
        /// Source file name
        /// </summary>
        private static string fileName = "about.htm";
        private static string mobileFileName = "about-mobile.html";
        
        /// <summary>
        /// Destination path
        /// </summary>
        private string destinationPath = @"..\..\..\Resources\" + fileName;
        private string destinationPathMobile = @"..\..\..\Resources\" + mobileFileName;

        public void CreateAboutHtml()
        {
            string pattern = @"AssemblyFileVersion\(""(?<Version>.*?"")";
            string versionNumber = string.Empty;
            string buildDate = DateTime.Now.ToLongDateString();

            if (File.Exists(assemblyInfoPath))
            {
                StreamReader srFromLocalFile = new StreamReader(assemblyInfoPath);
                string OnMemory = srFromLocalFile.ReadToEnd();

                if (Regex.Match(OnMemory, pattern).Success)
                {
                    versionNumber = Regex.Match(OnMemory, pattern).Groups["Version"].ToString().Trim('"');
                }
            }

            string aboutWebpage = "" +
                "<!DOCTYPE HTML>\n" +
                "<html>\n" +
                "	<head>\n" +
                "	    <title>Control Panel - About</title>\n" +
                "       <meta name=\"author\"   content=\"Gilberto García\"/>\n" +
                "       <meta name=\"mod-date\" content=\"03/09/2013\"/>\n" +
                "       <link rel=\"stylesheet\" type=\"text/css\" href=\"http://yourRPiServer/WebResources/header_style.css\"></style>\n" +
                "       <link rel=\"stylesheet\" type=\"text/css\" href=\"http://yourRPiServer/WebResources/table_style.css\"></style>\n" +
                "	</head>\n" +
                "	<body>\n" +
                "		<h1>Alarm Activity - Monitor System #1</h1>\n" +
                "        <br>\n" +
                "        AlarmByZones version: " + versionNumber + "\n" +
                "        <br>\n" +
                "        Build date: " + buildDate + "\n" +
                "        <br>\n" +
                "        Hardware: <a href=\"http://netduino.com/netduinoplus/specs.htm\" target=\"_blank\">Netduino Plus</a>\n" +
                "        <br>\n" +
                "        <br>\n" +
                "            <ul>\n" +
                "                <li>\n" +
                "                    AlarmByZones original implementation by: Gilberto Garc&#237;a (<i><a href=\"mailto:ferraripr@gmail.com\">ferraripr@gmail.com</a></i>).\n" +
                "                </li>\n" +
                "                <br>\n" +
                "                <li>\n" +
                "                    SMTP based on BanskySPOTMail by: <a href=\"http://bansky.net/blog/2008/08/sending-e-mails-from-net-micro-framework/\" target=\"_blank\">Pavel B&#225;nsk&#253;</a>.\n" +
                "                </li>\n" +
                "                <br>\n" +
                "                <li>\n" +
                "                    Web Server based on MFToolkit library by: <a href=\"http://mftoolkit.codeplex.com/\" target=\"_blank\">Michael Schwarz</a>.\n" +
                "                </li>\n" +
                "                <br>\n" +
                "                <li>\n" +
                "                    Pachube Embeddable Graph Generator (Beta) by: <a href=\"http://pachube.github.com/pachube_graph_library/\" target=\"_blank\">Pachube</a>, adapted by Gilberto Garc&#237;a.\n" +
                "                </li>\n" +
                "                <br>\n" +
                "                <li>\n" +
                "                    NTP Server and Extensions class based on a post/implementation by: <a href=\"http://forums.netduino.com/index.php?/topic/475-still-learning-internet-way-to-grab-date-and-time-on-startup/\" target=\"_blank\">Valkyrie-MT</a>.\n" +
                "                </li>\n" +
                "                <br>\n" +

                "                <li>\n" +
                "                    StopWatch class based on a post/implementation by: <a href=\"http://forums.netduino.com/index.php?/topic/97-systemdiagnosticsstopwatch-class/\" target=\"_blank\">Chris Walker</a>.\n" +
                "                </li>\n" +
                "                <br>\n" +

                "                <li>\n" +
                "                    LCD display using <a href=\"http://microliquidcrystal.codeplex.com/\" target=\"_blank\">uLiquidCrystal</a> library.\n" +
                "                </li>\n" +
                "                <br>\n" +

                "                <li>\n" +
                "                    Notification service using <a href=\"http://www.pushingbox.com/index.php\" target=\"_blank\">PushingBox</a>.\n" +
                "                </li>\n" +
                "                <br>\n" +

                "            </ul>\n" +
                "        <br>\n" +
                "        <a href=\"/\">Back to main page...</a>\n" +
                "        <br>\n" +
                "        <div style=\"border:1px solid #CCCCCC;\">" +
                "        <p><span class=\"note\">Copyright &#169; "+DateTime.Now.ToString("yyyy")+" Gilberto Garc&#237;a</span></p>" +
                "        </div>" +
                "	</body>\n" +
                "</html>\n";
            Console.WriteLine("Creating about.htm");
            FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(file);
            sw.WriteLine(aboutWebpage);
            sw.Flush();
            sw.Close();

            Console.WriteLine("Copying new about.htm file to AlarmByZones Resources directory.");
            //Programmatically copy new about.htm into AlarmByZones Resources directory
            File.Copy(fileName, destinationPath,true);

            Console.WriteLine("ResourceGenerator.Htm.About done!\n");
        }

        public void CreateAboutHtmlMobile()
        {
            string pattern = @"AssemblyFileVersion\(""(?<Version>.*?"")";
            string versionNumber = string.Empty;
            string buildDate = DateTime.Now.ToLongDateString();

            if (File.Exists(assemblyInfoPath))
            {
                StreamReader srFromLocalFile = new StreamReader(assemblyInfoPath);
                string OnMemory = srFromLocalFile.ReadToEnd();

                if (Regex.Match(OnMemory, pattern).Success)
                {
                    versionNumber = Regex.Match(OnMemory, pattern).Groups["Version"].ToString().Trim('"');
                }
            }

            string aboutMobileWebpage = "" +
                "<!DOCTYPE HTML>\n" +
                "<html>\n" +
                "	<head>\n" +
                "	    <title>Control Panel - About</title>\n" +
                "       <meta charset=\"utf-8\" />\n" +
                "       <meta name=\"viewport\" content=\"initial-scale=1.0, user-scalable=no\" />\n" +
                "       <meta name=\"apple-mobile-web-app-capable\" content=\"yes\" />\n" +
                "       <meta name=\"apple-mobile-web-app-status-bar-style\" content=\"black\" />\n" +
                "       <meta name=\"mod-date\" content=\"03/24/2013\"/>\n" +
                "       <!--http://jsfiddle.net/frankdenouter/Lp9P2/-->\n" +
                "       <link rel=\"stylesheet\" type=\"text/css\" href=\"http://yourRPiServer/WebResources/jquery_table_style.css\"></style>\n" +
                "       <link rel=\"stylesheet\" href=\"http://code.jquery.com/mobile/1.2.0/jquery.mobile-1.2.0.min.css\" />\n" +
                "       <script src=\"http://code.jquery.com/jquery-1.7.2.min.js\"></script>\n" +
                "       <script src=\"http://code.jquery.com/mobile/1.2.0/jquery.mobile-1.2.0.min.js\"></script>\n" +
                "       <!-- http://jsfiddle.net/jerone/gsNzT/ -->\n" +
                "       <script src=\"http://yourRPiServer/WebResources/jquery_animate_collapse.js\"></script>\n" +
                "	</head>\n" +
                "	<body>\n" +
                "		<div data-role=\"page\" id=\"about\" data-add-back-btn=\"true\" data-theme=\"b\" data-content-theme=\"b\">\n" +
                "          <div data-theme=\"b\" data-role=\"header\" >\n" +
                "          <h3>About</h3>\n" +
                "          <div data-role=\"content\">\n" +
                "             <div class=\"content-primary\">\n" +
                "                <h2>HomeAlarmPlus</h2>\n" +
                "                <p>Programmed by Gilberto Garc&#237;a</p>\n" +
                "                <p>For latest source code visit: <a href=\"https://github.com/ferraripr/HomeAlarmPlus\" rel=\"external\">Repository</a></p>\n" +
                "                <p><b>AlarmByZones version:</b> " + versionNumber + "</p>\n" +
                "                <p><b>Build date:</b> " + buildDate + "</p>\n" +
                "                <p><b>Hardware:</b> <a href=\"http://netduino.com/netduinoplus/specs.htm\" target=\"_blank\">Netduino Plus</a></p>\n" +
                "            <ul>\n" +
                "                <li>\n" +
                "                    AlarmByZones original implementation by: Gilberto Garc&#237;a (<i><a href=\"mailto:ferraripr@gmail.com\">ferraripr@gmail.com</a></i>).\n" +
                "                </li>\n" +
                "                <br>\n" +
                "                <li>\n" +
                "                    SMTP based on BanskySPOTMail by: <a href=\"http://bansky.net/blog/2008/08/sending-e-mails-from-net-micro-framework/\" target=\"_blank\">Pavel B&#225;nsk&#253;</a>.\n" +
                "                </li>\n" +
                "                <br>\n" +
                "                <li>\n" +
                "                    Web Server based on MFToolkit library by: <a href=\"http://mftoolkit.codeplex.com/\" target=\"_blank\">Michael Schwarz</a>.\n" +
                "                </li>\n" +
                "                <br>\n" +
                "                <li>\n" +
                "                    Pachube Embeddable Graph Generator (Beta) by: <a href=\"http://pachube.github.com/pachube_graph_library/\" target=\"_blank\">Pachube</a>, adapted by Gilberto Garc&#237;a.\n" +
                "                </li>\n" +
                "                <br>\n" +
                "                <li>\n" +
                "                    NTP Server and Extensions class based on a post/implementation by: <a href=\"http://forums.netduino.com/index.php?/topic/475-still-learning-internet-way-to-grab-date-and-time-on-startup/\" target=\"_blank\">Valkyrie-MT</a>.\n" +
                "                </li>\n" +
                "                <br>\n" +

                "                <li>\n" +
                "                    StopWatch class based on a post/implementation by: <a href=\"http://forums.netduino.com/index.php?/topic/97-systemdiagnosticsstopwatch-class/\" target=\"_blank\">Chris Walker</a>.\n" +
                "                </li>\n" +
                "                <br>\n" +

                "                <li>\n" +
                "                    LCD display using <a href=\"http://microliquidcrystal.codeplex.com/\" target=\"_blank\">uLiquidCrystal</a> library.\n" +
                "                </li>\n" +
                "                <br>\n" +
                "                <li>\n" +
                "                    Notification service using <a href=\"http://www.pushingbox.com/index.php\" target=\"_blank\">PushingBox</a>.\n" +
                "                </li>\n" +
                "                <br>\n" +
                "            </ul>\n" +
                "        </div>" +
                "        </div>" +
                "        </div>" +
                "        <div data-role=\"footer\" class=\"footer-docs\" data-theme=\"c\">\n" +
		        "            <p class=\"jqm-version\"></p>\n" +
		        "            <p>Copyright 2012, 2013 Gilberto Garc&#237;a</p>\n" +
	            "        </div>\n" +
                "        </div>" +
                "	</body>\n" +
                "</html>\n";
            Console.WriteLine("Creating about.htm");
            FileStream file = new FileStream(mobileFileName, FileMode.Create, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(file);
            sw.WriteLine(aboutMobileWebpage);
            sw.Flush();
            sw.Close();

            Console.WriteLine("Copying new about.htm file to AlarmByZones Resources directory.");
            //Programmatically copy new about.htm into AlarmByZones Resources directory
            File.Copy(mobileFileName, destinationPathMobile, true);

            Console.WriteLine("ResourceGenerator.Htm.About mobile done!\n");
        }


    }
}
