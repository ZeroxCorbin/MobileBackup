using MobileClasses;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Xml.Serialization;

namespace MobileBackup
{
    public partial class MobileBackup : ServiceBase
    {
        private List<MobileDebugInfoSettings> RobotConnectionData = new List<MobileDebugInfoSettings>();
        string ConfigurationFile { get; set; } = null;

        public MobileBackup() => InitializeComponent();
        protected override void OnStart(string[] args) { base.OnStart(args); Init(args, true); }
        protected override void OnStop() { base.OnStop(); }

        public void Init(string[] args, bool isService)
        {
            if(args.Length <= 1)
                Init(isService);
            else
            {
                if(!ParseArgs(args))
                    return;

                if(ConfigurationFile != null)
                {
                    Init(isService);
                    return;
                }

                RetrieveBackups();
            }
        }
        public void Init(bool isService)
        {
            if(isService && string.IsNullOrEmpty(ConfigurationFile))
                ConfigurationFile = Path.Combine("C:\\", "RobotConnectionData.xml");
            else if(string.IsNullOrEmpty(ConfigurationFile))
                ConfigurationFile = Path.Combine(Directory.GetCurrentDirectory(), "RobotConnectionData.xml");

            if(File.Exists(ConfigurationFile))
                Console.WriteLine($"Using Configuration file: {ConfigurationFile}");
            else
            {
                Console.WriteLine($"Configuration file does not exist: {ConfigurationFile}");
                return;
            }

            if(DeserializeConfiguration())
                RetrieveBackups();
        }

        private bool DeserializeConfiguration()
        {
            try
            {
                XmlSerializer serialiser = new XmlSerializer(typeof(List<MobileDebugInfoSettings>));
                using(TextReader filestream = new StreamReader(ConfigurationFile))
                    RobotConnectionData = (List<MobileDebugInfoSettings>)serialiser.Deserialize(filestream);
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

        }
        private bool SerializeConfiguration()
        {
            try
            {
                XmlSerializer serialiser = new XmlSerializer(typeof(List<MobileDebugInfoSettings>));
                using(TextWriter filestream = new StreamWriter(ConfigurationFile))
                    serialiser.Serialize(filestream, RobotConnectionData);
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public static bool SerializeConfiguration(string filePath, List<MobileDebugInfoSettings> robotConnectionData)
        {
            try
            {
                XmlSerializer serialiser = new XmlSerializer(typeof(List<MobileDebugInfoSettings>));
                using(TextWriter filestream = new StreamWriter(filePath))
                    serialiser.Serialize(filestream, robotConnectionData);
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private bool ParseArgs(string[] args)
        {
            Console.WriteLine($"Parsing input Args[]");

            bool ip = false;
            //bool port = false;
            bool user = false;
            bool pass = false;
            bool file = false;

            bool config = false;

            string ips = null;
            //string ports = null;
            string users = null;
            string passs = null;
            string files = null;

            string configs = null;

            for(int i = 1; i < args.Length; i++)
                if(args[i].Length > 1 && (args[i][0] == '-' || args[i][0] == '/'))
                {
                    switch(args[i].Substring(1).ToLower())
                    {
                        default:
                            Console.WriteLine($"ERROR: Invalid aurgument: {args[i].Substring(1).ToLower()}");
                            return false;
                        case "ip":
                        {
                            if(ip)
                            {
                                Console.WriteLine($"ERROR: You can only use one -ip command argument.");
                                return false;
                            }

                            if(i + 1 >= args.Length)
                            {
                                Console.WriteLine($"ERROR: Invalid aurgument value: {args[i].Substring(1).ToLower()}");
                                return false;
                            }

                            ips = args[i + 1].Trim();
                            if(Classes.StaticUtils.Regex.CheckValidIP(ips))
                                ip = true;
                            else
                            {
                                Console.WriteLine($"ERROR: Invalid IP address: {ips}");
                                return false;
                            }
                            break;
                        }
                        //case "port":
                        //{
                        //    if(port)
                        //    {
                        //        Console.WriteLine($"ERROR: You can only use one -/port command argument.");
                        //        return false;
                        //    }

                        //    if(i + 1 >= args.Length) return false;

                        //    ports = args[i + 1].Trim();
                        //    if(StaticUtils.Regex.CheckValidPort(ports))
                        //        port = true;
                        //    else
                        //    {
                        //        Console.WriteLine($"ERROR: Invalid port number: {ports}");
                        //        return false;
                        //    }
                        //    break;
                        //}
                        case "user":
                        {
                            if(user)
                            {
                                Console.WriteLine($"ERROR: You can only use one -user command argument.");
                                return false;
                            }

                            if(i + 1 >= args.Length) return false;

                            users = args[i + 1].Trim();
                            if(users.Length > 0)
                                user = true;
                            else
                            {
                                Console.WriteLine($"ERROR: Invalid port number: {users}");
                                return false;
                            }
                            break;
                        }
                        case "pass":
                        {
                            if(pass)
                            {
                                Console.WriteLine($"ERROR: You can only use one -pass command argument.");
                                return false;
                            }

                            if(i + 1 >= args.Length) return false;

                            passs = args[i + 1].Trim();
                            if(passs.Length > 0)
                                pass = true;
                            else
                            {
                                Console.WriteLine($"ERROR: Invalid port number: {passs}");
                                return false;
                            }
                            break;
                        }
                        case "file":
                        {
                            if(file)
                            {
                                Console.WriteLine($"ERROR: You can only use one -file command argument.");
                                return false;
                            }

                            if(i + 1 >= args.Length) return false;

                            files = args[i + 1].Trim();
                            //if(string.IsNullOrEmpty(files))
                            file = true;
                            //else
                            //{
                            //    Console.WriteLine($"ERROR: Invalid destination file path. {files}");
                            //    return false;
                            //}
                            break;
                        }
                        case "config":
                        {
                            if(config)
                            {
                                Console.WriteLine($"ERROR: You can only use one -config command argument.");
                                return false;
                            }

                            if(i + 1 >= args.Length) return false;

                            configs = args[i + 1].Trim();
                            if(configs.Length > 0)
                                config = true;
                            else
                            {
                                Console.WriteLine($"ERROR: Invalid cofig file: {configs}");
                                return false;
                            }
                            break;
                        }
                    }
                }

            if((ip | user | pass | file) & config)
            {
                Console.WriteLine($"ERROR: You can only use the -config command argument alone.");
                return false;
            }

            if(!(ip & user & pass & file) & !config)
            {
                Console.WriteLine($"ERROR: You must supply command arguments: -ip -port -user -pass -file");
                return false;
            }

            if(config)
                ConfigurationFile = configs;
            else
                RobotConnectionData.Add(new MobileDebugInfoSettings(ips, 443, users, passs, files));

            return true;
        }

        private void RetrieveBackups()
        {
            foreach(MobileDebugInfoSettings mbd in RobotConnectionData)
            {
                Console.WriteLine($"Retrieving debug file from IP: {mbd.IP}");
                if(MobileDebugInfoDownload.GetDebugFile(mbd.IP, mbd.UserName, mbd.Password, mbd.FilePath))
                {
                    Console.WriteLine($"Debug file saved.: {mbd.FilePath}");
                }
                else
                {
                    Console.WriteLine($"ERROR: Downloading debug file from IP: {mbd.IP}");
                }
            }

        }
    }
}
