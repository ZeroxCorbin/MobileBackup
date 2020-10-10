using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace MobileClasses
{
    public class MobileDebugInfoDownload : IDisposable
    {
        public delegate void DownloadFileCompletedDel(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
        public event DownloadFileCompletedDel DownloadFileCompleted;

        public delegate void DownloadProgressChangedDel(object sender, DownloadProgressChangedEventArgs e);
        public event DownloadProgressChangedDel DownloadProgressChanged;

        public delegate void DownloadDataCompletedDel(object sender, DownloadDataCompletedEventArgs e);
        public event DownloadDataCompletedDel DownloadDataCompleted;

        private WebClient WebClient { get; set; }
        public MobileDebugInfoDownload()
        {
            WebClient = new WebClient();
            ServicePointManager.ServerCertificateValidationCallback += (sender1, certificate, chain, sslPolicyErrors) => true;
        }

        public static string GetPage(string url, string userName, string password)
        {
            using(var client = new WebClient())
            {
                client.Credentials = new NetworkCredential(userName, password);

                using(var stream = client.OpenRead(url))
                using(var textReader = new StreamReader(stream, Encoding.UTF8, true))
                {
                    return textReader.ReadToEnd();
                }
            }


        }

        public static bool GetDebugFile(string ip, string userName, string password, string filePath)
        {
            Classes.ConnectionValues cv = new Classes.ConnectionValues(ip, 443, userName, password);
            if(!cv.IsValid)
            {
                Console.WriteLine($"ERROR: Could not validate connection values: {cv.ConnectionString}");
                return false;
            }

            ServicePointManager.ServerCertificateValidationCallback += (sender1, certificate, chain, sslPolicyErrors) => true;
            using(WebClient wc = new WebClient())
            {
                try
                {
                    string version = null;
                    string s = GetPage("https://" + ip.ToString() + "/?s=0&ss=0", userName, password);
                    Match m = Regex.Match(s, @"SetNetGo[ A-Za-z0-9.-]*<BR>");
                    if(m.Success)
                    {
                        Console.WriteLine($"VERSION: {m.Value.Replace("<BR>", "")}");
                        Match m1 = Regex.Match(m.Value, @"[0-9]");
                        if(m1.Success)
                            version = m1.Value;

                    }
                    if(version == null)
                    {
                        Console.WriteLine($"ERROR: Could not determine SetNetGo version.");
                        return false;
                    }

                    wc.Credentials = new NetworkCredential(userName, password);

                    if(version == "4")
                        wc.DownloadFile("https://" + ip.ToString() + "/cgi-bin/debugInfo.cgi", filePath);
                    else
                        wc.DownloadFile("https://" + ip.ToString() + "/cgi-bin/createDebugInfo.cgi", filePath);

                    return true;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
                finally
                {
                    ServicePointManager.ServerCertificateValidationCallback -= (sender1, certificate, chain, sslPolicyErrors) => true;
                }
            }
        }
        public bool StartGetDebugFile(string ip, string userName, string password, string filePath)
        {
            Classes.ConnectionValues cv = new Classes.ConnectionValues(ip, 443, userName, password);
            if(!cv.IsValid)
            {
                Console.WriteLine($"ERROR: Could not validate connection values: {cv.ConnectionString}");
                return false;
            }

            try
            {
                WebClient.Credentials = new NetworkCredential(userName, password);

                WebClient.DownloadDataCompleted += WebClient_DownloadDataCompleted;
                WebClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                WebClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;

                WebClient.DownloadFileAsync(new Uri("https://" + ip.ToString() + "/cgi-bin/debugInfo.cgi"), filePath);

                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }

        private void WebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e) => DownloadFileCompleted?.Invoke(sender, e);
        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) => DownloadProgressChanged?.Invoke(sender, e);
        private void WebClient_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e) => DownloadDataCompleted?.Invoke(sender, e);

        public void Dispose()
        {
            ServicePointManager.ServerCertificateValidationCallback -= (sender1, certificate, chain, sslPolicyErrors) => true;
            WebClient?.Dispose();
        }
    }
}
