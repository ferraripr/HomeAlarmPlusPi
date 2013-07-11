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
				"       <meta name=\"mod-date\" content=\"05/05/2013\"/>\n" +
                "       <!--jQuery, linked from a CDN-->\n"+
                "       <script src=\"http://code.jquery.com/jquery-1.9.1.js\"></script>\n" +
                "       <script src=\"http://code.jquery.com/ui/1.10.2/jquery-ui.js\"></script>\n" +
                "       <!--jQueryUI Theme -->\n" +
                "       <link rel=\"stylesheet\" href=\"http://code.jquery.com/ui/1.10.2/themes/redmond/jquery-ui.css\" />\n" +
                "       <link rel=\"stylesheet\" type=\"text/css\" href=\"http://yourRPiServer/WebResources/table_style.css\"></style>\n" +
                "	</head>\n" +
                "	<body>\n" +
                "       <div class=\"ui-widget\">\n" +
                "       <div class=\"ui-widget-header ui-corner-top\">\n" +
				"		<h2>Alarm Activity - Monitor System #1</h2></div>\n" +
                "       <div class=\"ui-widget-content ui-corner-bottom\">\n" +
				"        <br>\n" +
				"        AlarmByZones version: " + versionNumber + "\n" +
				"        <br>\n" +
				"        Build date: " + buildDate + "\n" +
				"        <br>\n" +
				"        Hardware: <a href=\"http://netduino.com/netduinoplus/specs.htm\" target=\"_blank\">Netduino Plus</a>\n" +
				"        <br>\n" +
				"        <br>\n" +
				"        <div>\n" +
				"        <ul>\n" +
				"        <li class=\"toplinks\"><a href='http://yourRPiServer/references.htm' target='_blank' title='Credits and contributors'>References</a></li>\n" +
				"        </ul>\n" +
				"        </div><br>\n" +
				"        <br>\n" +
                "        <a href=\"/\">Back to main page...</a>\n" +
				"        <br>\n" +
				"        <div style=\"border:1px solid #CCCCCC;\">" +
				"        <p><span class=\"note\">Copyright &#169; 2012, 2013 Gilberto Garc&#237;a</span></p>" +
				"        </div>" +
                "	</div></div></body>\n" +
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
                "                <li class=\"toplinks\"><a href='http://yourRPiServer/mobile/references-mobile.html' title='Credits and contributors'>References</a></li>\n" +
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
            Console.WriteLine("Creating about-mobile.html");
            FileStream file = new FileStream(mobileFileName, FileMode.Create, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(file);
            sw.WriteLine(aboutMobileWebpage);
            sw.Flush();
            sw.Close();

			Console.WriteLine("Copying new about-mobile.htm file to AlarmByZones Resources directory.");
			//Programmatically copy new about.htm into AlarmByZones Resources directory
			File.Copy(mobileFileName, destinationPathMobile, true);

			Console.WriteLine("ResourceGenerator.Htm.About mobile done!\n");
		}


	}
}
