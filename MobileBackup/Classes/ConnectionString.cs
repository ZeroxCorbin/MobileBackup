using System.Text.RegularExpressions;

namespace Classes
{
    public class ConnectionValues
    {
        public ConnectionValues() { }
        public ConnectionValues(string ip, int port, string userName, string password) => ConnectionString = $"{ip}:{port}:{userName}:{password}";

        public string ConnectionString { get; set; } = "192.168.0.20:7171:admin:admin";
        public string IP => IsValid ? Regex.Match(ConnectionString, @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)").Value : "";
        public string Port => IsValid ? Regex.Matches(ConnectionString, @"(?<=:)[a-zA-Z0-9]*")[0].Value : "";
        public string UserName => IsValid ? Regex.Matches(ConnectionString, @"(?<=:)[a-zA-Z0-9]*")[1].Value : "";
        public string Password => IsValid ? Regex.Matches(ConnectionString, @"(?<=:)[a-zA-Z0-9]*")[2].Value : "";

        public bool IsValid => Regex.IsMatch(ConnectionString, @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?):[1-9][0-9]{1,4}:.+?:.+?$");
    }
}
