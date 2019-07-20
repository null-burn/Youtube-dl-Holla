using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Media;

namespace YoutubeDL_Holla.Helpers
{
    public class PreReq
    {
        public static readonly string youtubedlExe = @"Youtube-dl\youtube-dl.exe";
        private static readonly string youtubedlExeDir = Directory.GetCurrentDirectory() + @"\" + youtubedlExe;

        private readonly MainWindow mainWindow = Application.Current.Windows[0] as MainWindow;
        private readonly Uri youtubedlUri = new Uri(ConfigurationManager.AppSettings["youtubedl-download-package"]);

        public void CheckPreReq()
        {
            PreReq preReq = new PreReq();
            if (preReq.YoutubeDLExists())
            {
                mainWindow.btnYoutubeDLReq.Background = Brushes.LawnGreen;
                mainWindow.btnYoutubeDLReq.IsEnabled = false;
                mainWindow.btnYoutubeDLReq.Content = Util.Messages.youtubedlFound;
            }
            else
            {
                mainWindow.btnYoutubeDLReq.Background = Brushes.IndianRed;
                mainWindow.btnYoutubeDLReq.IsEnabled = true;
                mainWindow.btnYoutubeDLReq.Content = Util.Messages.youtubedlNotFound;
            }
        }

        public bool YoutubeDLExists()
        {
            return File.Exists(youtubedlExeDir);
        }

        private long totalBytesOfFile = 0;

        public void DownloadYoutubeDL()
        {
            if (MessageBox.Show(string.Format("Do you want to download {0} ? Support the youtube-dl project at http://ytdl-org.github.io .", youtubedlUri), "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var webClient = new WebClient())
                {
                    try
                    {
                        if (youtubedlUri.Scheme == "https")
                        {
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        }

                        webClient.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
                        webClient.DownloadProgressChanged += DownloadProgressChanged;
                        webClient.DownloadFileCompleted += DownloadFileCompleted;
                        webClient.DownloadFileAsync(youtubedlUri, youtubedlExeDir);
                    }
                    catch
                    {
                        MessageBox.Show("An error ocurred while trying to download file");
                    }
                }
            }
            else
            {
                mainWindow.btnYoutubeDLReq.Background = Brushes.Red;
                mainWindow.btnYoutubeDLReq.IsEnabled = true;
            }
        }

        private void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null || totalBytesOfFile != new FileInfo(youtubedlExeDir).Length)
            {
                File.Delete(youtubedlExeDir);
                MessageBox.Show("An error ocurred while trying to download file, try again maybe...");
                return;
            }
            else
            {
                mainWindow.btnYoutubeDLReq.IsEnabled = false;
                mainWindow.btnYoutubeDLReq.Background = Brushes.LawnGreen;
                mainWindow.btnYoutubeDLReq.Content = Util.Messages.youtubedlFound;
            }
        }

        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            totalBytesOfFile = e.TotalBytesToReceive;
            mainWindow.btnYoutubeDLReq.Content = e.ProgressPercentage + "% | " + String.Format("{0:.##}", Util.BytesToMBs(e.BytesReceived))  + " MB out of " + String.Format("{0:.##}", Util.BytesToMBs(e.TotalBytesToReceive)) + " MB retrieved.";
        }
    }
}
