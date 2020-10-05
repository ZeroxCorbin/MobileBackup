using gotnet.biz.Utilities.WindowsServices;
using MobileClasses;
using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace MobileBackup
{
    static class Program
    {
        public static void Main(string[] args)
        {
            if(args != null && args[0].Length > 1
                && (args[0][0] == '-' || args[0][0] == '/'))
            {
                switch(args[0].Substring(1).ToLower())
                {
                    default:
                        Console.WriteLine($"Invalid aurgument: {args[0].Substring(1).ToLower()}");
                        break;
                    case "help":
                    case "h":
                        PrintHelp();
                        break;
                    case "generate":
                    {
                        if(args.Length >= 2 && !string.IsNullOrEmpty(args[1]))
                            GenerateConfigFile(args[1]);
                        else
                            Console.WriteLine("No file name or file path was supplied.");
                        break;
                    }
                    case "install":
                    case "i":
                        SelfInstaller.InstallMe();
                        break;
                    case "uninstall":
                    case "u":
                        SelfInstaller.UninstallMe();
                        break;
                    case "console":
                    case "c":
                    {
                        Console.WriteLine("Console Mode");
                        MobileBackup s = new MobileBackup();
                        s.Init(args, false); // Init() is pretty much any code you would have in OnStart().
                        break;
                    }
                    case "service":
                    case "s":
                    {
                        ServiceBase[] ServicesToRun;
                        ServicesToRun = new ServiceBase[]
                        {
                            new MobileBackup()
                        };
                        ServiceBase.Run(ServicesToRun);
                        break;
                    }

                }
            }
            else
            {
#if DEBUG
                Console.WriteLine("Console Mode no Args");
                MobileBackup s = new MobileBackup();
                s.Init(false); // Init() is pretty much any code you would have in OnStart().
                
#else
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new MobileBackup()
                };
                ServiceBase.Run(ServicesToRun);
#endif
            }
#if DEBUG
            Thread.Sleep(10000);
#endif
        }

        private static void GenerateConfigFile(string filePath)
        {

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
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("This program is used to download the debug info zip file from SetNetGo.");
            sb.AppendLine("It can be run as a system Service or as a console application.");
            sb.AppendLine("It will start as a system Service and attempt to use a configuration file");
            sb.AppendLine("located at C:\\RobotConnectionData.xml if no command arguments are given.");
            sb.AppendLine("You can use the -generate command argument to create a sample configuration file.");
            sb.AppendLine("\r\n");
            sb.AppendLine("The first argument must be one of:");
            sb.AppendLine("    -h -help : Display this help message.");
            sb.AppendLine("    -c -console : Run this application in console mode.");
            sb.AppendLine("    -generate \"Configuration File Path\" : Generate a template configuration file.");
            sb.AppendLine("    -i -install : Install this application as a system Service.");
            sb.AppendLine("    -u -uninstall : Uninstall this application as a system Service.");
            sb.AppendLine("    -s -Service : Run this application as a system Service.");
            sb.AppendLine("                  Used only when installed as a Service and you need to pass other arguments.");
            sb.AppendLine("To download from multiple devices, you can use a XML configuration file.");
            sb.AppendLine("    -config \"Configuration File Path\" : Use a configuration file located at the specified path.");
            sb.AppendLine("For a single backup, you can use the following command arguments;");
            sb.AppendLine("     -ip #.#.#.# : IP address to connect to SetNetGo.");
            sb.AppendLine("     -user username : SetNetGo user name.");
            sb.AppendLine("     -pass password : SetNetGo password.");
            sb.AppendLine("     -file \"Destination File Path\" : Destination file name for the download. Relative or absolute path.");
            sb.AppendLine("             {ip} will be replaced with the robot IP.");
            sb.AppendLine("             {MM-dd-yy_HH-mm-ss} will be replaced with the corresponding");
            sb.AppendLine("             date and time pattern. More information can be found at;");
            sb.AppendLine("             https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings");
            sb.AppendLine("Examples:");
            sb.AppendLine("Display this help message");    
            sb.AppendLine("    MobileBackup.exe -help");
            sb.AppendLine("Perform an immediate debug info download using the SetNetGo IP address, user name, and password.");
            sb.AppendLine("Save the debug info to the file name supplied.");
            sb.AppendLine("    MobileBackup.exe -c -ip 192.168.1.1 -user admin -pass admin -file backup_{ip}_{MM-dd-yy_HH-mm-ss}.zip");
            Console.Write(sb.ToString());
        }
    }
}
