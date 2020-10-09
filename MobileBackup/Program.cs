using MobileClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace MobileBackup
{
    static class Program
    {
        private static void Elevate(string[] args)
        {
            if((new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator))
                return;

            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach(string a in args)
            {
                sb.Append(a);
                if(i++ < args.Length - 1)
                    sb.Append(" ");
            }
            var SelfProc = new ProcessStartInfo
            {
                UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory,
                FileName = Path.Combine(Environment.CurrentDirectory, "MobileBackup.exe"),
                Arguments = sb.ToString(),
                Verb = "runas"
            };
            try
            {
                Process.Start(SelfProc);
            }
            catch
            {

            }
        }
        public static string GetResourceData(string resourceName)
        {
            string[] stt = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            var embeddedResource = Assembly.GetExecutingAssembly().GetManifestResourceNames().FirstOrDefault(s => string.Compare(s, resourceName, true) == 0);

            if(!string.IsNullOrWhiteSpace(embeddedResource))
            {
                using(var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResource))
                {
                    var buf = new byte[stream.Length];
                    int len = stream.Read(buf, 0, buf.Length);
                    return Encoding.UTF8.GetString(buf, 0, len);
                }
            }

            return null;
        }
        public static void Main(string[] args)
        {
            bool found = false;
            if(args.Count() > 0 && args[0].Length > 1 && (args[0][0] == '-' || args[0][0] == '/'))
            {
                switch(args[0].Substring(1).ToLower())
                {
                    default:
                        break;
                    case "help":
                    case "h":
                        PrintHelp();
                        found = true;
                        break;
                    case "generate":
                    {
                        Elevate(args);
                        if(args.Length >= 2)
                            GenerateConfigFile(args[1]);
                        else
                            GenerateConfigFile(null);
                        found = true;
                        break;
                    }
                    case "install":
                    case "i":
                        Elevate(args);
                        SelfInstaller.InstallMe();
                        found = true;
                        break;
                    case "uninstall":
                    case "u":
                        Elevate(args);
                        SelfInstaller.UninstallMe();
                        found = true;
                        break;
                }
            }

            if(!found)
                if(Environment.UserInteractive)
                {
                    Elevate(args);
                    Console.WriteLine("Console Mode");
                    MobileBackup s = new MobileBackup();
                    if(args.Length > 0)
                        s.Init(args, false);
                    else
                        s.Init(false);
                }
                else
                {
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[]
                    {
                        new MobileBackup()
                    };
                    ServiceBase.Run(ServicesToRun);
                }
            if(Environment.UserInteractive)
            {
                Console.WriteLine("Press any key to exit..");
                Console.ReadKey();
            }
        }

        private static void GenerateConfigFile(string filePath)
        {
            if(filePath == null)
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "RobotConnectionData.xml");

            List<MobileDebugInfoSettings> lst = new List<MobileDebugInfoSettings>
            {
                new MobileDebugInfoSettings("192.168.1.1", 443, "admin", "admin", "backup_{ip}_{MM-dd-yy_HH-mm-ss}.zip"),
                new MobileDebugInfoSettings("192.168.1.2", 443, "admin", "admin", "backup_{ip}_{MM-dd-yy_HH-mm-ss}.zip")
            };

            if(!filePath.EndsWith(".xml"))
                filePath += ".xml";

            if(MobileBackup.SerializeConfiguration(filePath, lst))
                Console.WriteLine($"The generated config file is located at: {filePath}");
            else
                Console.WriteLine($"ERROR: Could not generate config file located at: {filePath}");
        }

        private static void PrintHelp()
        {
            Console.WriteLine(GetResourceData("MobileBackup.Resources.README.txt"));
            //StringBuilder sb = new StringBuilder();

            //sb.AppendLine("This program is used to download the debug info zip file from SetNetGo.");
            //sb.AppendLine("It can be run as a system Service or as a console application.");
            //sb.AppendLine("It will start as a system Service and attempt to use a configuration file");
            //sb.AppendLine("located at C:\\RobotConnectionData.xml if no command arguments are given.");
            //sb.AppendLine("You can use the -generate command argument to create a sample configuration file.");
            //sb.AppendLine("\r\n");
            //sb.AppendLine("The first argument must be one of:");
            //sb.AppendLine("    -h -help : Display this help message.");
            //sb.AppendLine("    -c -console : Run this application in console mode.");
            //sb.AppendLine("    -generate \"Configuration File Path\" : Generate a template configuration file.");
            //sb.AppendLine("    -i -install : Install this application as a system Service.");
            //sb.AppendLine("    -u -uninstall : Uninstall this application as a system Service.");
            //sb.AppendLine("    -s -Service : Run this application as a system Service.");
            //sb.AppendLine("                  Used only when installed as a Service and you need to pass other arguments.");
            //sb.AppendLine("To download from multiple devices, you can use a XML configuration file.");
            //sb.AppendLine("    -config \"Configuration File Path\" : Use a configuration file located at the specified path.");
            //sb.AppendLine("For a single backup, you can use the following command arguments;");
            //sb.AppendLine("     -ip #.#.#.# : IP address to connect to SetNetGo.");
            //sb.AppendLine("     -user username : SetNetGo user name.");
            //sb.AppendLine("     -pass password : SetNetGo password.");
            //sb.AppendLine("     -file \"Destination File Path\" : Destination file name for the download. Relative or absolute path.");
            //sb.AppendLine("             {ip} will be replaced with the robot IP.");
            //sb.AppendLine("             {MM-dd-yy_HH-mm-ss} will be replaced with the corresponding");
            //sb.AppendLine("             date and time pattern. More information can be found at;");
            //sb.AppendLine("             https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings");
            //sb.AppendLine("Examples:");
            //sb.AppendLine("Display this help message");
            //sb.AppendLine("    MobileBackup.exe -help");
            //sb.AppendLine("Perform an immediate debug info download using the SetNetGo IP address, user name, and password.");
            //sb.AppendLine("Save the debug info to the file name supplied.");
            //sb.AppendLine("    MobileBackup.exe -c -ip 192.168.1.1 -user admin -pass admin -file backup_{ip}_{MM-dd-yy_HH-mm-ss}.zip");
            //Console.Write(sb.ToString());
        }
    }
}
