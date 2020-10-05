# MobileBackup
This program is used to download the debug info zip file from SetNetGo.  
This program can be run as a system Service or as a console application.  
It will start as a system Service and attemp to use a configuration file  
located at C:\RobotConnectionData.xml if no command arguments are given.  
You can use the -generate command argument to create a sample configuration file.  
  
  
The first argument must be one of:  
    -h -help : Display this help message.  
    -c -console : Run this application in console mode.  
    -generate "Configuration File Path" : Generate a template configuration file.  
    -i -install : Install this application as a system Service.  
    -u -uninstall : Uninstall this application as a system Service.  
    -s -Service : Run this application as a system Service.  
                  Used only when installed as a Service and you need to pass other arguments.  
To download from multiple devices, you can use a XML configuration file.  
    -config "Configuration File Path" : Use a configuration file located at the specified path.  
For a single backup, you can use the following command arguments;  
     -ip #.#.#.# : IP address to connect to SetNetGo.  
     -user username : SetNetGo user name.  
     -pass password : SetNetGo password.  
     -file "Destination File Path" : Destination file name for the download. Relative or absolute path.  
             {ip} will be replaced with the robot IP.  
             {MM-dd-yy_HH-mm-ss} will be replaced with the corresponding  
             date and time pattern. More information can be found at;  
             https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings  
Examples:  
Display this help message  
    MobileBackup.exe -help  
Perform an immediate debug info download using the SetNetGo IP address, user name, and password.  
Save the debug info to the file name supplied.  
    MobileBackup.exe -c -ip 192.168.1.1 -user admin -pass admin -file backup_{ip}_{MM-dd-yy_HH-mm-ss}.zip  
