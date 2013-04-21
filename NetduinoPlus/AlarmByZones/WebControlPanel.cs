using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using System.Text;


namespace AlarmByZones
{
    public class WebControlPanel : MFToolkit.Net.Web.IHttpHandler
    {

        public void ProcessRequest(MFToolkit.Net.Web.HttpContext context)
        {
            string rawURL_string = string.Empty;
            string menu_Pachube = string.Empty;
            string menu_SuperUser = string.Empty;

            DateTime endTime = DateTime.Now;
            TimeSpan span = endTime.Subtract(AlarmByZones.dLastResetCycle);
            string strUptime = span.Duration().Days > 0 ? span.Days + "days, " : string.Empty;
            strUptime = strUptime + span.Hours + ":" + span.Minutes + ":" + span.Seconds + " (hh:mm:ss)</li></lu><br>";

            if (Alarm.ConfigDefault.Data.USE_PACHUBE)
            {
                menu_Pachube = "<li class=\"toplinks\"><a href='/pachube' title='Generates Pachube Graphics'>PACHUBE GRAPHICS</a></li>";
            }

            if (context.Request.RawUrl == "/su")
            {
                menu_SuperUser = "<li class=\"toplinks\"><a href='/delete-confirm' title='Deletes last event.'>DELETE LAST EVENT LOG</a></li>";
            }

            string menu_Header =
                "<div>\n" +
                "<ul>\n" +
                "<li class=\"toplinks\"><a href=\"/\" title='Home'>HOME</a></li>\n" +
                "<li class=\"toplinks\"><a href='/sdcard' title='Retrieve alarm events stored in SD Card'>SD CARD EVENT LOG</a></li>\n" +
                menu_Pachube + menu_SuperUser +
                "<li class=\"toplinks\"><a href='/diag'  title='Diagnostics'>DIAGNOSTICS</a></li>\n" +
                "<li class=\"toplinks\"><a href='" + Alarm.ConfigDefault.Data.HTTP_HOST + "' target='_blank'  title='HomeAlarmPlus Pi'>HomeAlarmPlus Pi</a></li>\n" +
                "<li class=\"toplinks\"><a href='/about' title='Credits and contributors'>ABOUT</a></li>\n" +
                "</ul>\n" +
                "</div>\n";
            string fileLink = string.Empty;
            try
            {
                rawURL_string = context.Request.RawUrl.Substring(0, 5);
            }
            catch { }

            try
            {
                if (rawURL_string == "/open")
                {
                    string[] url = context.Request.RawUrl.Split('_');
                    fileLink = context.Request.RawUrl.Substring(6, context.Request.RawUrl.Length - 6);
                    fileLink = Extension.Replace(fileLink, "/", "\\");
                    context.Request.RawUrl = "/open";
                }
            }
            catch { }

            try
            {
                if (rawURL_string == "/date")
                {
                    string[] url = context.Request.RawUrl.Split('_');
                    DateTime networkDateTime = new DateTime(Int16.Parse((url[1])), Int16.Parse(url[2]), Int16.Parse(url[3]), Int16.Parse(url[4]), 
                          Int16.Parse(url[5]), Int16.Parse(url[6]));

                    Debug.Print(networkDateTime.ToString());
                    Utility.SetLocalTime(networkDateTime);
                    context.Request.RawUrl = "/date";
                }
            }
            catch { }
           
            switch (context.Request.RawUrl)
            {
                case "/date":
                    break;
                case "/su":
                case "/":
                case "/mobile":
                    //AlarmByZones.email.SendEmail("Web Server access", "Home web server access.\nAssemblyInfo: " + System.Reflection.Assembly.GetExecutingAssembly().FullName);
                    //PushingBox Web server access notification
                    PushingBox.Notification.Connect("vPUSHINGBOX");
                    Console.DEBUG_ACTIVITY(Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0].IPAddress);
                    string HTML_tbHeader = "<table class=\"gridtable\"><tr><th><center>Time</center></th><th><center>Zone/Sensor</center></th><th><center>Description</center></th></tr>";
                    string AlarmStatus = Alarm.Common.Alarm_Info.sbActivity.Length == 0 ? "No Alarms/Sensors " : HTML_tbHeader + Alarm.Common.Alarm_Info.sbActivity.ToString();
                    context.Response.ContentType = "text/html";
                    context.Response.WriteLine("<html><head><title>Control Panel - Home</title>");
                    context.Response.WriteLine("<meta name=\"author\"   content=\"Gilberto García\"/>");
                    context.Response.WriteLine("<meta name=\"mod-date\" content=\"04/21/2013\"/>");
                    context.Response.WriteLine("<meta name=\"application-name\" content=\"HomeAlarm Plus\"/>");
                    context.Response.WriteLine("<meta charset=\"utf-8\" />");
                    context.Response.WriteLine("<meta name=\"viewport\" content=\"initial-scale=1.0, user-scalable=no\" />");
                    context.Response.WriteLine("<meta name=\"apple-mobile-web-app-capable\" content=\"yes\" />");
                    context.Response.WriteLine("<meta name=\"apple-mobile-web-app-status-bar-style\" content=\"black\" />");
                    if (context.Request.RawUrl == "/mobile")
                    {
                        context.Response.WriteLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + Alarm.ConfigDefault.Data.HTTP_HOST + "/WebResources/jquery_table_style.css\"></style>");
                        context.Response.WriteLine("<link rel=\"stylesheet\" href=\"http://code.jquery.com/mobile/1.2.0/jquery.mobile-1.2.0.min.css\" />");
                        context.Response.WriteLine("<script src=\"http://code.jquery.com/jquery-1.7.2.min.js\"></script>");
                        context.Response.WriteLine("<script src=\"http://code.jquery.com/mobile/1.2.0/jquery.mobile-1.2.0.min.js\"></script>");
                        context.Response.WriteLine("<script src=\"" + Alarm.ConfigDefault.Data.HTTP_HOST + "/WebResources/jquery_animate_collapse.js\"></script>");
                        context.Response.WriteLine("</head><body>");
                        context.Response.WriteLine("<div data-role=\"page\" id=\"mobile-page\" data-add-back-btn=\"true\" data-theme=\"b\" data-content-theme=\"b\">");
                        context.Response.WriteLine("<div data-theme=\"b\" data-role=\"header\" >");
                        context.Response.WriteLine("<h3>Alarm Activity - Monitor System #1 - Home</h3>");
                        context.Response.WriteLine("</div>");
                        context.Response.WriteLine("<div data-role=\"content\">");
                        context.Response.WriteLine("<div class=\"content-primary\">");
                        context.Response.WriteLine("<p>Current Time: <b>" + DateTime.Now.ToString("dd MMM yyyy hh:mm:ss tt") + "</b></p></br>");
                        context.Response.WriteLine("Alarm Status Log: " + AlarmStatus);
                        context.Response.WriteLine("</table>");
                        context.Response.WriteLine("</div>");
                        context.Response.WriteLine("</div>");
                        context.Response.WriteLine("<br/><br/><br/>");
                        context.Response.WriteLine("<div data-role=\"footer\" class=\"footer-docs\" data-theme=\"c\">");
                        context.Response.WriteLine("<p class=\"jqm-version\"></p>");
                        context.Response.WriteLine("<p>Copyright 2012, 2013 Gilberto Garc&#237;a</p>");
                        context.Response.WriteLine("</div>	");
                    }
                    else
                    {
                        context.Response.WriteLine("<link rel=\"stylesheet\" type=\"text/css\" href=\""+ Alarm.ConfigDefault.Data.HTTP_HOST + "/WebResources/header_style.css\"></style>");
                        context.Response.WriteLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + Alarm.ConfigDefault.Data.HTTP_HOST + "/WebResources/table_style.css\"></style>");

                        context.Response.WriteLine("</head><body>");
                        context.Response.WriteLine("<h1>Alarm Activity - Monitor System #1</h1></br>");
                        context.Response.WriteLine("<p>Current Time: <b>" + DateTime.Now.ToString("dd MMM yyyy hh:mm:ss tt") + "</b></p></br>");
                        context.Response.WriteLine(menu_Header);
                        context.Response.WriteLine("<br>");
                        context.Response.WriteLine("Alarm Status Log: " + AlarmStatus);
                        context.Response.WriteLine("</table>");
                        context.Response.WriteLine("<br><br>");
                        context.Response.WriteLine("<div style=\"border:1px solid #CCCCCC;\">");
                        context.Response.WriteLine("<p><span class=\"note\">Copyright &#169; 2012, 2013 Gilberto Garc&#237;a</span></p>");
                    }
                    context.Response.WriteLine("</div></body></html>");
                    //clear variables
                    HTML_tbHeader = null;
                    AlarmStatus = null;
                    menu_Header = null;
                    rawURL_string = null;
                    break;
                case "/about":
                    string about = Properties.Resources.GetString(Properties.Resources.StringResources.about);
                    context.Response.ContentType = "text/html";
                    context.Response.WriteLine(about);
                    //clear variables
                    about = null;
                    rawURL_string = null;
                    break;
                case "/about-mobile":
                    string aboutmobile = Properties.Resources.GetString(Properties.Resources.StringResources.about_mobile);
                    context.Response.ContentType = "text/html";
                    context.Response.WriteLine(aboutmobile);
                    //clear variables
                    aboutmobile = null;
                    rawURL_string = null;
                    break;
                case "/sdcard":
                    context.Response.ContentType = "text/html";
                    context.Response.WriteLine("<html><head><title>Control Panel - SD Card History Log</title>");
                    context.Response.WriteLine("<meta name=\"author\"   content=\"Gilberto García\"/>");
                    context.Response.WriteLine("<meta name=\"mod-date\" content=\"04/21/2013\"/>");
                    context.Response.WriteLine("<meta name=\"application-name\" content=\"HomeAlarm Plus\"/>");
                    context.Response.WriteLine("<meta charset=\"utf-8\" />");
                    context.Response.WriteLine("<meta name=\"viewport\" content=\"initial-scale=1.0, user-scalable=no\" />");
                    context.Response.WriteLine("<meta name=\"apple-mobile-web-app-capable\" content=\"yes\" />");
                    context.Response.WriteLine("<meta name=\"apple-mobile-web-app-status-bar-style\" content=\"black\" />");
                    context.Response.WriteLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + Alarm.ConfigDefault.Data.HTTP_HOST + "/WebResources/header_style.css\"></style>");    
                    context.Response.WriteLine("</head><body>");
                    context.Response.WriteLine("<h1>Alarm Activity - Monitor System #1</h1></br>");
                    context.Response.WriteLine("<p>Current Time: <b>" + DateTime.Now.ToString("dd MMM yyyy hh:mm:ss tt") + "</b></p></br>");
                    if (AlarmByZones.SdCardEventLogger.IsSDCardAvailable())
                    {
                        context.Response.WriteLine("SD Card detected and found the following files:<br><br>");
                        context.Response.WriteLine("<form name=\"sdForm\"");
                        int i = 0;
                        try
                        {
                            foreach (string file in AlarmByZones.SdCardEventLogger.FileList)
                            {
                                context.Response.WriteLine(file + "<br>");
                                Console.DEBUG_ACTIVITY(i.ToString() + " " + file);
                                i++;
                            }
                            context.Response.WriteLine("</form>");
                        }
                        catch (Exception ex)
                        {
                            Debug.Print(ex.Message);
                        }
                    }
                    else
                    {
                        context.Response.WriteLine(AlarmByZones.SdCardEventLogger.NO_SD_CARD + "<br>");
                    }
                    context.Response.WriteLine("<br>");
                    context.Response.WriteLine("<a href=\"/\">Back to main page...</a>");
                    context.Response.WriteLine("<div style=\"border:1px solid #CCCCCC;\">");
                    context.Response.WriteLine("<p><span class=\"note\">Copyright &#169; 2012, 2013 Gilberto Garc&#237;a</span></p>");
                    context.Response.WriteLine("</div></body></html>");
                    rawURL_string = null;
                    break;
                case "/open":
                    System.Collections.ArrayList alOpen = new System.Collections.ArrayList();
                    context.Response.ContentType = "text/html";
                    context.Response.WriteLine("<html><head><title>Control Panel - Open SD Card File</title>");
                    context.Response.WriteLine("<meta name=\"author\"   content=\"Gilberto García\"/>");
                    context.Response.WriteLine("<meta name=\"mod-date\" content=\"04/21/2013\"/>");
                    context.Response.WriteLine("<meta name=\"application-name\" content=\"HomeAlarm Plus\"/>");
                    context.Response.WriteLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + Alarm.ConfigDefault.Data.HTTP_HOST + "/WebResources/header_style.css\"></style>");
                    context.Response.WriteLine("</head><body>");
                    context.Response.WriteLine("<h1>Alarm Activity - Monitor System #1</h1></br>");
                    context.Response.WriteLine(menu_Header);
                    context.Response.WriteLine("<p>Current Time: <b>" + DateTime.Now.ToString("dd MMM yyyy hh:mm:ss tt") + "</b></p></br>");
                    if (AlarmByZones.SdCardEventLogger.IsSDCardAvailable())
                    {
                        AlarmByZones.SdCardEventLogger.openFileContent(fileLink, alOpen);
                        context.Response.WriteLine("File: " + fileLink + "<br>");
                        context.Response.WriteLine("File Content: " + "<br>");
                        foreach (string content in alOpen)
                        {
                            context.Response.WriteLine(content + "<br>");
                        }
                        //clear variables
                        alOpen.Clear();
                    }
                    else
                    {
                        context.Response.WriteLine(AlarmByZones.SdCardEventLogger.NO_SD_CARD);
                    }
                    context.Response.WriteLine("<br><br>");
                    context.Response.WriteLine("<a href=\"/\">Back to main page...</a>");
                    context.Response.WriteLine("<div style=\"border:1px solid #CCCCCC;\">");
                    context.Response.WriteLine("<p><span class=\"note\">Copyright &#169; 2012, 2013 Gilberto Garc&#237;a</span></p>");
                    context.Response.WriteLine("</div></body></html>");
                    //clear variables
                    alOpen.Clear();
                    alOpen = null;
                    menu_Header = null;
                    rawURL_string = null;
                    break;
                case "/pachube":
                    System.Collections.ArrayList alPachube = new System.Collections.ArrayList();
                    Pachube.EmbeddableGraphGenerator.EmbeddableHTML.GenerateHTML(alPachube);
                    context.Response.ContentType = "text/html";
                    context.Response.WriteLine("<html><head><title>Control Panel - Pachube Graphics</title>");
                    context.Response.WriteLine("<meta name=\"author\"   content=\"Gilberto García\"/>");
                    context.Response.WriteLine("<meta name=\"mod-date\" content=\"04/21/2013\"/>");					
                    context.Response.WriteLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + Alarm.ConfigDefault.Data.HTTP_HOST + "/WebResources/header_style.css\"></style>");
                    context.Response.WriteLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + Alarm.ConfigDefault.Data.HTTP_HOST + "/WebResources/table_style.css\"></style>");
                    context.Response.WriteLine("</head><body>");
                    context.Response.WriteLine("<h1>Alarm Activity - Monitor System #1</h1></br>");
                    context.Response.WriteLine(menu_Header);
                    context.Response.WriteLine("<p>Current Time: <b>" + DateTime.Now.ToString("dd MMM yyyy hh:mm:ss tt") + "</b></p></br>");
                    foreach (string content in alPachube)
                    {
                        context.Response.WriteLine(content);
                        context.Response.WriteLine("<br>");
                    }
                    context.Response.WriteLine("<a href=\"/\">Back to main page...</a>");
                    context.Response.WriteLine("<div style=\"border:1px solid #CCCCCC;\">");
                    context.Response.WriteLine("<p><span class=\"note\">Copyright &#169; 2012, 2013 Gilberto Garc&#237;a</span></p>");
                    context.Response.WriteLine("</div></body></html>");
                    //clear variables
                    alPachube.Clear();
                    alPachube = null;
                    menu_Header = null;
                    rawURL_string = null;
                    break;
                case "/delete-confirm":
                    if (AlarmByZones.SdCardEventLogger.IsSDCardAvailable())
                    {
                        string rawHref = AlarmByZones.SdCardEventLogger.FileList[AlarmByZones.SdCardEventLogger.FileList.Count - 1].ToString();
                        string[] parseHref = rawHref.Split(new Char[] { '<', '>' });
                        //We do not want to delete Config.ini 
                        string LastFile = rawHref == Alarm.User_Definitions.Constants.ALARM_CONFIG_FILE_PATH  ? rawHref : parseHref[2];
                        context.Response.ContentType = "text/html";
                        context.Response.WriteLine("<html><head><title>Control Panel - Delete SD Card confirm</title>");
                        context.Response.WriteLine("<meta name=\"author\"   content=\"Gilberto García\"/>");
                        context.Response.WriteLine("<meta name=\"mod-date\" content=\"04/21/2013\"/>");
                        context.Response.WriteLine("<meta name=\"application-name\" content=\"HomeAlarm Plus\"/>");
                        context.Response.WriteLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + Alarm.ConfigDefault.Data.HTTP_HOST + "/WebResources/header_style.css\"></style>");
                        context.Response.WriteLine("</head><body>");
                        context.Response.WriteLine("<h1>Alarm Activity - Monitor System #1</h1></br>");
                        context.Response.WriteLine(menu_Header);
                        context.Response.WriteLine("<p>Current Time: <b>" + DateTime.Now.ToString("dd MMM yyyy hh:mm:ss tt") + "</b></p></br>");
                        if (LastFile != Alarm.User_Definitions.Constants.ALARM_CONFIG_FILE_PATH &&
                            LastFile != @"\SD\Logs" && LastFile != @"\SD\Exception" )
                        {
                            context.Response.WriteLine("<A HREF=\"/delete-last\" onCLick=\"return confirm('Are you sure you want to delete " +LastFile +" file?')\">Delete " 
                                + LastFile + "</A>");                            
                        }
                        else
                        {
                            context.Response.WriteLine("No files to delete.");
                        }
                        context.Response.WriteLine("<br><br>");
                        //clear variables
                        rawHref = null;
                        parseHref = null;
                        LastFile = null;
                    }
                    context.Response.WriteLine("<a href=\"/\">Back to main page...</a>");
                    context.Response.WriteLine("<div style=\"border:1px solid #CCCCCC;\">");
                    context.Response.WriteLine("<p><span class=\"note\">Copyright &#169; 2012, 2013 Gilberto Garc&#237;a</span></p>");
                    context.Response.WriteLine("</div></body></html>");
                    menu_Header = null;
                    rawURL_string = null;
                    break;
                case "/delete":
                case "/delete-last":
                    context.Response.ContentType = "text/html";
                    context.Response.WriteLine("<html><head><title>Control Panel - Delete SD Card File</title>");
                    context.Response.WriteLine("<meta name=\"author\"   content=\"Gilberto García\"/>");
                    context.Response.WriteLine("<meta name=\"mod-date\" content=\"04/21/2013\"/>");
                    context.Response.WriteLine("<meta name=\"application-name\" content=\"HomeAlarm Plus\"/>");
                    context.Response.WriteLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + Alarm.ConfigDefault.Data.HTTP_HOST + "/WebResources/header_style.css\"></style>");
                    context.Response.WriteLine("</head><body>");
                    context.Response.WriteLine("<h1>Alarm Activity - Monitor System #1</h1></br>");
                    context.Response.WriteLine(menu_Header);
                    context.Response.WriteLine("<p>Current Time: <b>" + DateTime.Now.ToString("dd MMM yyyy hh:mm:ss tt") + "</b></p></br>");
                    if (AlarmByZones.SdCardEventLogger.IsSDCardAvailable())
                    {
                        string rawHref = AlarmByZones.SdCardEventLogger.FileList[AlarmByZones.SdCardEventLogger.FileList.Count - 1].ToString();
                        string[] parseHref = rawHref.Split(new Char[] { '<', '>' });
                        //string LastFile = parseHref[2];
                        string LastFile = rawHref == Alarm.User_Definitions.Constants.ALARM_CONFIG_FILE_PATH ? rawHref : parseHref[2];

                        if (LastFile != Alarm.User_Definitions.Constants.ALARM_CONFIG_FILE_PATH && 
                            LastFile != @"\SD\Logs" && LastFile != @"\SD\Exception")
                        {
                            AlarmByZones.SdCardEventLogger.deleteFile(LastFile);

                            context.Response.WriteLine("Deleted File: " + LastFile);                            
                        }
                        else
                        {
                            context.Response.WriteLine("No files to delete.");
                        }
                        context.Response.WriteLine("<br><br>");
                        //clear variables
                        rawHref = null;
                        parseHref = null;
                        LastFile = null;
                    }
                    context.Response.WriteLine("<a href=\"/\">Back to main page...</a>");
                    context.Response.WriteLine("<div style=\"border:1px solid #CCCCCC;\">");
                    context.Response.WriteLine("<p><span class=\"note\">Copyright &#169; 2012, 2013 Gilberto Garc&#237;a</span></p>");
                    context.Response.WriteLine("</div></body></html>");
                    menu_Header = null;
                    rawURL_string = null;
                    break;
                case "/diag":
                case "/diagnostics":
                    //This option is very useful when used with Android App "Overlook Wiz"
                    //---------------------------------------------------------------
                    //Overlook Wiz settings:
                    //Host name or IP address: your host settings.
                    //Host custom name shown on widget: Alarm System
                    //Monitored service: Web Server (HTTP)
                    //Service TCP Port: same as entered in Config.ini [NETDUINO_PLUS_HTTP_PORT], otherwise enter the default port 8080
                    //Website URL to be requested: /diag
                    context.Response.ContentType = "text/html";
                    context.Response.WriteLine("<html><head><title>Control Panel - Diagnostics</title>");
                    context.Response.WriteLine("<meta name=\"author\"   content=\"Gilberto García\"/>");
                    context.Response.WriteLine("<meta name=\"mod-date\" content=\"04/21/2013\"/>");
                    context.Response.WriteLine("<meta name=\"application-name\" content=\"HomeAlarm Plus\"/>");
                    context.Response.WriteLine("<meta charset=\"utf-8\" />");
                    context.Response.WriteLine("<meta name=\"viewport\" content=\"initial-scale=1.0, user-scalable=no\" />");
                    context.Response.WriteLine("<meta name=\"apple-mobile-web-app-capable\" content=\"yes\" />");
                    context.Response.WriteLine("<meta name=\"apple-mobile-web-app-status-bar-style\" content=\"black\" />");
                    context.Response.WriteLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + Alarm.ConfigDefault.Data.HTTP_HOST + "/WebResources/header_style.css\"></style>");
                    context.Response.WriteLine("</head><body>");
                    context.Response.WriteLine("<h1>Alarm Activity - Monitor System #1 - Diagnostics</h1><br>");
                    context.Response.WriteLine("<p>Current Time: <b>" + DateTime.Now.ToString("dd MMM yyyy hh:mm:ss tt") + "</b></p></br>");
                    context.Response.WriteLine("<p><font size=\"5\" face=\"verdana\" color=\"green\">Alarm System is up and running!</font></p><br>");
                    context.Response.WriteLine("<b>Power Cycle</b>");
                    context.Response.WriteLine("<lu><li>Last Time since reset: " + AlarmByZones.LastResetCycle + "</li>");
                    context.Response.WriteLine("<li>Uptime: " + strUptime);
                    context.Response.WriteLine("<b>Memory</b>");
                    context.Response.WriteLine("<lu>");
                    context.Response.WriteLine("<li>Available Memory: " + Debug.GC(true) + "</li>");
                    context.Response.WriteLine(AlarmByZones.SdCardEventLogger.SDCardInfo());
                    context.Response.WriteLine("<br>");
                    context.Response.WriteLine("<b>AssemblyInfo</b>");
                    context.Response.WriteLine("<li>" + System.Reflection.Assembly.GetExecutingAssembly().FullName + "</li><br>");
                    context.Response.WriteLine("</lu>");
                    context.Response.WriteLine("<br><br>");
                    context.Response.WriteLine("<a href=\"/\">Back to main page...</a>");
                    context.Response.WriteLine("<div style=\"border:1px solid #CCCCCC;\">");
                    context.Response.WriteLine("<p><span class=\"note\">Copyright &#169; 2012, 2013 Gilberto Garc&#237;a</span></p>");
                    context.Response.WriteLine("</div></body></html>");
                    rawURL_string = null;
                    break;
                case "/diag-mobile":
                    context.Response.ContentType = "text/html";
                    context.Response.WriteLine("<html><head><title>Control Panel - Diagnostics</title>");
                    context.Response.WriteLine("<meta charset=\"utf-8\" />");
                    context.Response.WriteLine("<meta name=\"viewport\" content=\"initial-scale=1.0, user-scalable=no\" />");
                    context.Response.WriteLine("<meta name=\"apple-mobile-web-app-capable\" content=\"yes\" />");
                    context.Response.WriteLine("<meta name=\"apple-mobile-web-app-status-bar-style\" content=\"black\" />");
                    context.Response.WriteLine("<meta name=\"author\"   content=\"Gilberto García\"/>");
                    context.Response.WriteLine("<meta name=\"mod-date\" content=\"03/31/2013\"/>");
                    context.Response.WriteLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + Alarm.ConfigDefault.Data.HTTP_HOST + "/WebResources/jquery_table_style.css\"></style>");
                    context.Response.WriteLine("<link rel=\"stylesheet\" href=\"http://code.jquery.com/mobile/1.2.0/jquery.mobile-1.2.0.min.css\" />");
                    context.Response.WriteLine("<script src=\"http://code.jquery.com/jquery-1.7.2.min.js\"></script>");
                    context.Response.WriteLine("<script src=\"http://code.jquery.com/mobile/1.2.0/jquery.mobile-1.2.0.min.js\"></script>");
                    context.Response.WriteLine("<script src=\"" + Alarm.ConfigDefault.Data.HTTP_HOST + "/WebResources/jquery_animate_collapse.js\"></script>");
                    context.Response.WriteLine("</head><body>");
                    context.Response.WriteLine("<div data-role=\"page\" id=\"netduino-diag-page\" data-add-back-btn=\"true\" data-theme=\"b\" data-content-theme=\"b\">");
                    context.Response.WriteLine("<div data-theme=\"b\" data-role=\"header\" >");
                    context.Response.WriteLine("<h3>Alarm Activity - Monitor System #1 - Diagnostics</h3>");
                    context.Response.WriteLine("</div>");
                    context.Response.WriteLine("<div data-role=\"content\">");
                    context.Response.WriteLine("<div class=\"content-primary\">");
                    context.Response.WriteLine("<p>Current Time: <b>" + DateTime.Now.ToString("dd MMM yyyy hh:mm:ss tt") + "</b></p></br>");
                    context.Response.WriteLine("<p><font size=\"5\" face=\"verdana\" color=\"green\">Alarm System is up and running!</font></p>");
                    context.Response.WriteLine("<b>Power Cycle</b>");
                    context.Response.WriteLine("<lu><li>Last Time since reset: " + AlarmByZones.LastResetCycle + "</li>");
                    context.Response.WriteLine("<li>Uptime: " + strUptime);
                    context.Response.WriteLine("<b>Memory</b>");
                    context.Response.WriteLine("<lu>");
                    context.Response.WriteLine("<li>Available Memory: " + Debug.GC(true) + "</li>");
                    context.Response.WriteLine(AlarmByZones.SdCardEventLogger.SDCardInfo());
                    context.Response.WriteLine("<br>");
                    context.Response.WriteLine("<b>AssemblyInfo</b>");
                    context.Response.WriteLine("<li>" + System.Reflection.Assembly.GetExecutingAssembly().FullName + "</li><br>");
                    context.Response.WriteLine("</lu>");
                    context.Response.WriteLine("</div>");
                    context.Response.WriteLine("</div>");
                    context.Response.WriteLine("<div data-role=\"footer\" class=\"footer-docs\" data-theme=\"c\">");
		            context.Response.WriteLine("<p class=\"jqm-version\"></p>");
		            context.Response.WriteLine("<p>Copyright 2012, 2013 Gilberto Garc&#237;a</p>");
	                context.Response.WriteLine("</div>	");
                    context.Response.WriteLine("</div></body></html>");
                    rawURL_string = null;
                    break;
                default:
                    rawURL_string = null;
                    context.Response.RaiseError(MFToolkit.Net.Web.HttpStatusCode.NotFound);
                    break;
            }
        }

    }
}
