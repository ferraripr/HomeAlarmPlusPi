using System;

using SecretLabs.NETMF.Hardware.NetduinoPlus;
using Microsoft.SPOT.IO;
using System.IO;
using Microsoft.SPOT;
using System.Text;

namespace AlarmByZones
{
    /// <summary>
    /// Event logger class
    /// </summary>
    public class FileManagement
    {
        #region Constructor
        public FileManagement()
        {

        }
        #endregion

        #region Declarations
        /// <summary>
        /// File arraylist
        /// </summary>
        public System.Collections.ArrayList FileList = new System.Collections.ArrayList();

        /// <summary>
        /// No SD Card Message
        /// </summary>
        public string NO_SD_CARD = "SD Card not detected or reachable.";
        #endregion

        #region Methods
        /// <summary>
        /// Checks SD Card Access on Netduino Plus.
        /// </summary>
        public void SDCardAccess()
        {
            try
            {
                Debug.GC(true);
                FileList.Clear();

                DirectoryInfo root = new DirectoryInfo(@"\SD\");
                RecurseFolders(root);

                Console.DEBUG_ACTIVITY("FileList: " + FileList.Count.ToString());
            }
            catch (Exception ex)
            {
                Console.DEBUG_ACTIVITY("SD Card Exception when trying to list file(s).\nException Message: " +
                    ex.Message);
            }
        }

        private void RecurseFolders(DirectoryInfo directory)
        {
            if (directory.Exists)
            {
                Debug.Print(directory.FullName);

                foreach (FileInfo file in directory.GetFiles())
                {
                    //linkify All File Name, except Config.ini
                    //We want to avoid any exposure of Config.ini on the web
                    string filename = file.FullName == Alarm.User_Definitions.Constants.ALARM_CONFIG_FILE_PATH ?
                        file.FullName :
                        "<a href=\"/open_" + file.FullName + "\">" + file.FullName + "</a>";

                    FileList.Add(filename);
                    Console.DEBUG_ACTIVITY(file.FullName);
                }

                foreach (DirectoryInfo subDirectory in directory.GetDirectories())
                {
                    RecurseFolders(subDirectory);
                }
            }
        }

        /// <summary>
        /// Saves file to SD card.
        /// </summary>
        /// <param name="sourceFileName">The file to save.</param>
        /// <param name="sourceContent">The content to be save.</param>
        /// <param name="type">The type of activity [Log or Exception].</param>
        public void saveFile(string sourceFileName, string sourceContent, string type)
        {
            string path = type.ToUpper() == "EXCEPTION" ? Path.Combine("SD", "Exception") : Path.Combine("SD", "Logs");
            CheckDirectory(path);
            try
            {
                using (StreamWriter sw = new StreamWriter(Path.Combine(path, sourceFileName)))
                {
                    sw.WriteLine(sourceContent);
                }
                
                UpdateFileList();
            }
            catch (Exception ex)
            {
                Console.DEBUG_ACTIVITY("SD Card Exception when trying to save a file in Full path name: " +
                    path + "\\" + sourceFileName + ".\nException Message: " + ex.Message);
            }
        }

        /// <summary>
        /// Parses config file contents.
        /// </summary>
        /// <param name="sourceFullName">The configuration file to read.</param>
        public void parseConfigFileContents(string sourceFullName)
        {
            try
            {
                StreamReader sr = new StreamReader(sourceFullName);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    switch (line.Trim())
                    {
                        case "[NETDUINO_PLUS_HTTP_PORT]":
                            Alarm.ConfigDefault.Data.Http_port = int.Parse(sr.ReadLine());
                            Console.DEBUG_ACTIVITY(Alarm.ConfigDefault.Data.Http_port.ToString());
                            break;
                        case "[ALARM_ZONES]":
                            for (int i = 0; i < Alarm.User_Definitions.Constants.ACTIVE_ZONES; i++)
                            {
                                string zone = sr.ReadLine().Trim();
                                string[] parse = zone.Split('=');
                                Alarm.Common.Alarm_Info.zoneDescription.Add(parse[0], parse[1]);
                                Console.DEBUG_ACTIVITY("Zones " + parse[0] + " ," + parse[1]);
                            }
                            break;
                        case "[SENSORS]":
                            for (int i = 0; i < Alarm.User_Definitions.Constants.MOTION_SENSORS; i++)
                            {
                                string sensor = sr.ReadLine().Trim();
                                string[] parse = sensor.Split('=');
                                Alarm.Common.Alarm_Info.sensorDescription.Add(parse[0], parse[1]);
                                Console.DEBUG_ACTIVITY("Sensors " + parse[0] + " ," + parse[1]);
                            }
                            break;
                        case "[EMAIL_FREQ]":
                            Alarm.ConfigDefault.Data.EMAIL_FREQUENCY = int.Parse(sr.ReadLine());
                            Console.DEBUG_ACTIVITY("Email Frequency: " + Alarm.ConfigDefault.Data.EMAIL_FREQUENCY.ToString());
                            break;
                        case "[STORE_LOG]":
                            bool bstoreLog = sr.ReadLine().ToUpper() == "Y" ? true : false;
                            Alarm.ConfigDefault.Data.STORE_LOG = bstoreLog;
                            Console.DEBUG_ACTIVITY("Store Log: " + Alarm.ConfigDefault.Data.STORE_LOG);
                            break;
                        case "[STORE_EXCEPTION]":
                            bool bstoreException = sr.ReadLine().ToUpper() == "Y" ? true : false;
                            Alarm.ConfigDefault.Data.STORE_EXCEPTION = bstoreException;
                            Console.DEBUG_ACTIVITY("Store Exception: " + Alarm.ConfigDefault.Data.STORE_EXCEPTION);
                            break;
                        case "[USE_EMAIL]":
                            bool bEmail = sr.ReadLine().ToUpper() == "Y" ? true : false;
                            Alarm.ConfigDefault.Data.USE_EMAIL = bEmail;
                            Console.DEBUG_ACTIVITY("Use Email Option: " + Alarm.ConfigDefault.Data.USE_EMAIL);
                            break;
                        case "[USE_PACHUBE]":
                            bool bPachube = sr.ReadLine().ToUpper() == "Y" ? true : false;
                            Alarm.ConfigDefault.Data.USE_PACHUBE = bPachube;
                            Console.DEBUG_ACTIVITY("Use Pachube Option: " + Alarm.ConfigDefault.Data.USE_PACHUBE);
                            break;
                        case "[HTTP_HOST]":
                            Alarm.ConfigDefault.Data.HTTP_HOST = sr.ReadLine();
                            Console.DEBUG_ACTIVITY("HTTP HOST: " + Alarm.ConfigDefault.Data.HTTP_HOST);
                            break;
                        default:
                            break;
                    }
                }

                sr.Close();
                sr.Dispose();
            }
            catch (Exception ex)
            {
                Console.DEBUG_ACTIVITY("SD Card Exception when trying to save a file in Full path name: " +
                    sourceFullName + ".\nException Message: " + ex.Message);
            }
        }

        /// <summary>
        /// Determines whether the given path refers to an existing directory on disk.
        /// </summary>
        /// <param name="path"></param>
        private void CheckDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception ex)
            {
                Console.DEBUG_ACTIVITY("SD Card Exception when trying to create a directory.\nException Message: " +
                    ex.Message);
            }
        }

        /// <summary>
        /// Determines whether the SD Card is accessible or not.
        /// </summary>
        /// <returns>true if SD Card is accessible; otherwise, false.</returns>
        public bool IsSDCardAvailable()
        {
            return FileList.Count != 0 ? true : false;
        }

        /// <summary>
        /// Update File ArrayList to keep current with what is stored
        /// </summary>
        private void UpdateFileList()
        {
            SDCardAccess();
        }

        /// <summary>
        /// Opens a file path with to read its content
        /// </summary>
        /// <param name="sourceFullName">The content to open.</param>
        /// <param name="alFileContent">The file content.</param>
        public void openFileContent(string sourceFullName, System.Collections.ArrayList alFileContent)
        {
            alFileContent.Clear();
            try
            {
                StreamReader sr = new StreamReader(sourceFullName);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    //alFileContent.Add("<br/>'" + line);
                    alFileContent.Add(line);
                }
                sr.Close();
                sr.Dispose();
            }
            catch (Exception ex)
            {
                Console.DEBUG_ACTIVITY("SD Card Exception when trying to open a file.\nException Message: " +
                    ex.Message);
            }
        }

        public void deleteFile(string sourceFullName)
        {
            try
            {
                if (File.Exists(sourceFullName))
                {
                    File.Delete(sourceFullName);
                    UpdateFileList();
                }
            }
            catch (Exception ex)
            {
                Console.DEBUG_ACTIVITY(ex.Message);
            }
        }

        /// <summary>
        /// Access SD Card information related to file-system volume
        /// </summary>
        /// <see cref="http://msdn.microsoft.com/en-us/library/ee437065.aspx"/>
        /// <returns>file-system volume in HTML format</returns>
        public string SDCardInfo()
        {
            string Info = "<li>SD Card not detected or installed.</li>";
            if (IsSDCardAvailable())
            {
                VolumeInfo[] volumes = VolumeInfo.GetVolumes();
                foreach (VolumeInfo vi in volumes)
                {
                    double FreeSpace = vi.TotalFreeSpace / 1048576;
                    double TotalVolume = vi.TotalSize / 1048576;
                    

                    Info = "<br/><b>SD Card Status</b>" +
                        "<li>Root Directory: " + vi.RootDirectory.ToString() + "</li>" +
                        "<li>Device Flags: " + vi.DeviceFlags.ToString() + "</li>" +
                        "<li>Attributes Read-only?: " + volumes.IsReadOnly.ToString() + "</li>" +
                        "<li>Total Volume: " + TotalVolume.ToString() + "MB</li>" +
                        "<li>Total Free Space: " + FreeSpace.ToString() + "MB</li>";
                }
            }
            else
            {
                Info = "<br/><b>SD Card Status</b>"+
                    "<li>" + NO_SD_CARD + "</li>";
            }
            return Info;
        }

        /// <summary>
        /// Load a file path to read its content
        /// </summary>
        /// <param name="sourceFullName">The content to open.</param>
        /// <returns>File content.</returns>
        public string loadFileContent(string sourceFullName)
        {
            string content = string.Empty;
            try
            {
                StreamReader sr = new StreamReader(sourceFullName);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    //alFileContent.Add("<br/>'" + line);
                    content += line;
                }
                sr.Close();
                sr.Dispose();
            }
            catch (Exception ex)
            {
                Console.DEBUG_ACTIVITY("SD Card Exception when trying to open a file.\nException Message: " +
                    ex.Message);
            }
            return content;
        }

        /// <summary>
        /// Append contents into the file.
        /// </summary>
        /// <param name="sourceFileName">The file to save.</param>
        /// <param name="sourceContent">The content to be save.</param>
        /// <param name="type">The type of activity [Log or Exception].</param>
        public void saveLog(string sourceContent, string type)
        {
            string path = type.ToUpper() == "EXCEPTION" ? Path.Combine("SD","Exception") : Path.Combine("SD","Logs");
            CheckDirectory(path);
            try
            {
                FileInfo fi = new FileInfo(Path.Combine(path,"Log.txt"));
                if (!fi.Exists)
                {
                    using (var filestream = new FileStream(Path.Combine(path, "Log.txt"), FileMode.Create))
                    {
                        StreamWriter streamWriter = new StreamWriter(filestream);
                        streamWriter.WriteLine(sourceContent);
                        streamWriter.Close();
                    }
                }
                else
                {
                    using (var filestream = new FileStream(Path.Combine(path, "Log.txt"), FileMode.Append, FileAccess.Write))
                    {
                        StreamWriter streamWriter = new StreamWriter(filestream);
                        streamWriter.WriteLine(sourceContent);
                        streamWriter.Close();
                    }
                }

                UpdateFileList();
            }
            catch (Exception ex)
            {
                Console.DEBUG_ACTIVITY("SD Card Exception when trying to save a file in Full path name: " +
                    Path.Combine(path, "Log.txt") + ".\nException Message: " + ex.Message);
            }
        }

        #endregion
    }
}