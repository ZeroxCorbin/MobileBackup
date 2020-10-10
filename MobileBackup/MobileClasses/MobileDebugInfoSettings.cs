using System;
using System.Text.RegularExpressions;

namespace MobileClasses
{
    public class MobileDebugInfoSettings : Classes.ConnectionValues
    {
        public string FilePath
        {
            get
            {
                string @return = DestinationPath;
                foreach(Match match in Regex.Matches(@return, @"(?<={).*?(?=})"))
                {
                    if(match.Value.Equals("ip"))
                        @return = @return.Replace($"{{ip}}", IP);
                    else
                    {
                        string dt;
                        try
                        {
                            dt = DateTime.Now.ToString(match.Value);
                            @return = @return.Replace($"{{{match.Value}}}", DateTime.Now.ToString(match.Value));
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            @return = @return.Replace($"{{{match.Value}}}", DateTime.Now.ToString("MM-dd-yy_HH-mm-ss"));
                        }
                    }
                }
                if(!@return.EndsWith(".zip"))
                    @return += ".zip";

                return @return;
            }
        }
        public string DestinationPath { get; set; }

        public MobileDebugInfoSettings() { }
        public MobileDebugInfoSettings(string ip, int port, string userName, string password, string destPath) : base(ip, port, userName, password) => DestinationPath = destPath;
    }
}
