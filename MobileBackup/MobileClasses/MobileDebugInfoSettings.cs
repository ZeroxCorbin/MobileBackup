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
                        @return = @return.Replace($"{{{match.Value}}}", DateTime.Now.ToString(match.Value));
                }
                if(!@return.EndsWith(".zip"))
                    @return += ".zip";

                return @return;
            }
        }
        public string DestinationPath { get; set; }
        //public double FrequencyDays { get; set; }

        public MobileDebugInfoSettings() { }
        public MobileDebugInfoSettings(string ip, int port, string userName, string password, string destPath) : base(ip, port, userName, password) => DestinationPath = destPath;
    }
}
