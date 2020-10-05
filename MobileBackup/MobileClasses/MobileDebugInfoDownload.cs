using System;
using System.Net;

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

        public MobileDebugInfoDownload()
        {
            WebClient = new WebClient();
            ServicePointManager.ServerCertificateValidationCallback += (sender1, certificate, chain, sslPolicyErrors) => true;
        }

        private WebClient WebClient { get; set; }
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
                    wc.Credentials = new NetworkCredential(userName, password);
                    wc.DownloadFile("https://" + ip.ToString() + "/cgi-bin/debugInfo.cgi", filePath);

                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
            ServicePointManager.ServerCertificateValidationCallback -= (sender1, certificate, chain, sslPolicyErrors) => true;

            return true;
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
