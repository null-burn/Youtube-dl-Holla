using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace YoutubeDL_Holla
{
    public partial class MainWindow : MetroWindow
    {
        string msgInvalidUrl = "Invalid URL, try again!";
        string youtubeDlFileName = @"Youtube-dl\youtube-dl.exe";
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            cbAudio.IsEnabled = false;
            cbVideo.IsEnabled = false;
            btnAudioOnly.IsEnabled = false;
            btnAudioOnlyMP3.IsEnabled = false;
            btnVideoOnly.IsEnabled = false;
            btnAudioPlusVideo.IsEnabled = false;

            //DEBUG
            urlToAdd.Text = "https://youtu.be/FQLGhPHzxjc";
        }

        private void GetMedia_Click(object sender, RoutedEventArgs e)
        {
            string url = urlToAdd.Text;
            if (Helpers.Url.Valid(url))
            {
                string btncontent = (sender as Button).Name.ToString();

                consoleControl.ClearOutput();
                DisableSubButtons();
                DisableMediaButton();

                if (btncontent == "btnAudioOnly")
                {
                    btnAudioOnly.Background = Brushes.LawnGreen;
                    YoutubeDLProcess(url, cbAudio.SelectedValue.ToString(), null);
                }
                else if (btncontent == "btnAudioOnlyMP3")
                {
                    btnAudioOnlyMP3.Background = Brushes.LawnGreen;
                    YoutubeDLProcess(url, cbAudio.SelectedValue.ToString(), null, true);
                }
                else if (btncontent == "btnVideoOnly")
                {
                    btnVideoOnly.Background = Brushes.LawnGreen;
                    YoutubeDLProcess(url, null, cbVideo.SelectedValue.ToString());
                }
                else if (btncontent == "btnAudioPlusVideo")
                {
                    btnAudioPlusVideo.Background = Brushes.LawnGreen;
                    YoutubeDLProcess(url, cbAudio.SelectedValue.ToString(), cbVideo.SelectedValue.ToString());
                }
            }
            else
            {
                MessageBox.Show(msgInvalidUrl);
            }
        }

        private void DisableSubButtons()
        {
            btnAudioOnly.IsEnabled = false;
            btnAudioOnlyMP3.IsEnabled = false;
            btnAudioPlusVideo.IsEnabled = false;
            btnVideoOnly.IsEnabled = false;

            btnAudioOnly.Background = Brushes.Red;
            btnAudioOnlyMP3.Background = Brushes.Red;
            btnAudioPlusVideo.Background = Brushes.Red;
            btnVideoOnly.Background = Brushes.Red;
        }

        private void DisableMediaButton()
        {
            btnGetMediaInfo.IsEnabled = false;
            btnGetMediaInfo.Background = Brushes.Red;
        }

        private void GetMediaInfo_Click(object sender, RoutedEventArgs e)
        {
            string url = urlToAdd.Text;
            if (Helpers.Url.Valid(url))
            {
                consoleControl.ClearOutput();
                btnGetMediaInfo.Background = Brushes.LawnGreen;
                btnGetMediaInfo.IsEnabled = false;

                DisableSubButtons();
                YoutubeDLGetMediaInfoProcess(url);
            }
            else
            {
                MessageBox.Show(msgInvalidUrl);
            }
        }

        private void YoutubeDLProcess(string url, string formatAudioCode, string formatVideoCode, bool mp3 = false)
        {
            StringBuilder arguments = new StringBuilder();
            if (chkVerbose.IsChecked ?? false)
            {
                arguments.Append("-v ");
            }
            if (chkKeep.IsChecked ?? false)
            {
                arguments.Append("-k ");
            }

            if (mp3)
            {
                arguments.Append("--extract-audio --audio-format mp3 --audio-quality 0 -f " + formatAudioCode + " ");
            }
            else
            {
                if (!string.IsNullOrEmpty(formatAudioCode) && string.IsNullOrEmpty(formatVideoCode) && !mp3)
                {
                    arguments.Append("-f " + formatAudioCode + " ");
                }
                else if (string.IsNullOrEmpty(formatAudioCode) && !string.IsNullOrEmpty(formatVideoCode))
                {
                    arguments.Append("-f " + formatVideoCode + " ");
                }
                else if (!string.IsNullOrEmpty(formatVideoCode) && !string.IsNullOrEmpty(formatAudioCode))
                {
                    arguments.Append("-f " + formatVideoCode + "+" + formatAudioCode + " ");
                }
            }
            
            arguments.Append(url);

            consoleControl.ShowDiagnostics = true;
            consoleControl.StartProcess(youtubeDlFileName, arguments.ToString());
            consoleControlSV.ScrollToBottom();

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(Processing);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void YoutubeDLGetMediaInfoProcess(string url)
        {
            StringBuilder arguments = new StringBuilder();
            if (chkVerbose.IsChecked ?? false)
            {
                arguments.Append("-v ");
            }
            if (chkKeep.IsChecked ?? false)
            {
                arguments.Append("-k ");
            }
            arguments.Append("-F ");
            arguments.Append(url);

            consoleControl.ShowDiagnostics = true;
            consoleControl.StartProcess(youtubeDlFileName, arguments.ToString());
            consoleControlSV.ScrollToBottom();

            dispatcherTimer.Tick += new EventHandler(ProcessingGetMediaInfo);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void Processing(object sender, EventArgs e)
        {
            if (!consoleControl.IsProcessRunning)
            {
                btnAudioOnly.IsEnabled = true;
                btnAudioOnlyMP3.IsEnabled = true;
                btnVideoOnly.IsEnabled = true;
                btnAudioPlusVideo.IsEnabled = true;
                btnGetMediaInfo.IsEnabled = true;

                btnAudioOnly.ClearValue(Button.BackgroundProperty);
                btnAudioOnlyMP3.ClearValue(Button.BackgroundProperty);
                btnAudioPlusVideo.ClearValue(Button.BackgroundProperty);
                btnVideoOnly.ClearValue(Button.BackgroundProperty);
                btnGetMediaInfo.ClearValue(Button.BackgroundProperty);

                dispatcherTimer.Stop();
            }
        }

        private void ProcessingGetMediaInfo(object sender, EventArgs e)
        {
            if (!consoleControl.IsProcessRunning)
            {
                Helpers.Output output = new Helpers.Output();
                RichTextBox rtbConsole = (RichTextBox)consoleControl.Content;
                TextRange txtRange = new TextRange(rtbConsole.Document.ContentStart, rtbConsole.Document.ContentEnd);
                txtDebug.Text += txtRange.Text;
                string[] lines = txtRange.Text.Split('\n');

                List<Helpers.Output.AvailableMediaOutput> availableMediaOutputList = new List<Helpers.Output.AvailableMediaOutput>();
                availableMediaOutputList = output.CreateAvailableMediaList(lines);

                cbAudio.ItemsSource = availableMediaOutputList.Where(x => x.Resolution == "audio only").Reverse().ToList();
                cbAudio.SelectedIndex = 0;

                cbVideo.ItemsSource = availableMediaOutputList.Where(x => x.Resolution != "audio only").Reverse().ToList();
                cbVideo.SelectedIndex = 0;

                cbAudio.IsEnabled = true;
                cbVideo.IsEnabled = true;

                btnAudioOnly.IsEnabled = true;
                btnAudioOnlyMP3.IsEnabled = true;
                btnVideoOnly.IsEnabled = true;
                btnAudioPlusVideo.IsEnabled = true;
                btnGetMediaInfo.IsEnabled = true;

                btnAudioOnly.ClearValue(Button.BackgroundProperty);
                btnAudioOnlyMP3.ClearValue(Button.BackgroundProperty);
                btnAudioPlusVideo.ClearValue(Button.BackgroundProperty);
                btnVideoOnly.ClearValue(Button.BackgroundProperty);
                btnGetMediaInfo.ClearValue(Button.BackgroundProperty);

                dispatcherTimer.Stop();   
            }
        }
    }
}
