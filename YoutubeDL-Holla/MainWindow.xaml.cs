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
using YoutubeDL_Holla.Helpers;

namespace YoutubeDL_Holla
{
    public partial class MainWindow : MetroWindow
    {
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            cbAudio.IsEnabled = false;
            cbVideo.IsEnabled = false;
            btnAudioOnly.IsEnabled = false;
            btnAudioOnlyMP3.IsEnabled = false;
            btnVideoOnly.IsEnabled = false;
            btnAudioPlusVideo.IsEnabled = false;

            PreReq preReq = new PreReq();
            preReq.CheckPreReq();
        }

        private void GetMedia_Click(object sender, RoutedEventArgs e)
        {
            Dir dir = new Dir();
            bool letsDoIt = true;

            string url = urlToAdd.Text;
            if (!Url.Valid(url))
            {
                letsDoIt = false;
                MessageBox.Show(Util.Messages.InvalidUrl);
            }

            if (!dir.CheckFolderPath(saveToDirectory.Text) && letsDoIt)
            {
                letsDoIt = false;
                MessageBox.Show(Util.Messages.InvalidFolderPath);
            }

            if (letsDoIt)
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
            PreReq preReq = new PreReq();
            bool letsDoIt = true;

            string url = urlToAdd.Text;

            if (!Url.Valid(url))
            {
                letsDoIt = false;
                MessageBox.Show(Util.Messages.InvalidUrl);
            }
            if (!preReq.YoutubeDLExists() && letsDoIt)
            {
                letsDoIt = false;
                MessageBox.Show(Util.Messages.MissingYoutubeDl);
            }

            if (letsDoIt)
            {
                cbAudio.ItemsSource = cbVideo.ItemsSource = null;
                consoleControl.ClearOutput();
                btnGetMediaInfo.Background = Brushes.LawnGreen;
                btnGetMediaInfo.IsEnabled = false;

                DisableSubButtons();
                YoutubeDLGetMediaInfoProcess(url);
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
                    arguments.Append("-f ").Append(formatAudioCode).Append(' ');
                }
                else if (string.IsNullOrEmpty(formatAudioCode) && !string.IsNullOrEmpty(formatVideoCode))
                {
                    arguments.Append("-f ").Append(formatVideoCode).Append(' ');
                }
                else if (!string.IsNullOrEmpty(formatVideoCode) && !string.IsNullOrEmpty(formatAudioCode))
                {
                    arguments.Append("-f ").Append(formatVideoCode).Append('+').Append(formatAudioCode).Append(' ');
                }
            }

            arguments.Append("-o ").Append(saveToDirectory.Text).Append("/%(title)s.%(ext)s ");
            arguments.Append(url);

            consoleControl.ShowDiagnostics = true;
            consoleControl.StartProcess(PreReq.youtubedlExe, arguments.ToString());
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
            consoleControl.StartProcess(PreReq.youtubedlExe, arguments.ToString());
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
                Output output = new Output();
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

                if (cbAudio.Items.Count > 0)
                {
                    cbAudio.IsEnabled = true;
                    btnAudioOnly.IsEnabled = true;
                    btnAudioOnlyMP3.IsEnabled = true;
                }
                if (cbVideo.Items.Count > 0)
                {
                    cbVideo.IsEnabled = true;
                    btnVideoOnly.IsEnabled = true;
                }
                if (cbAudio.Items.Count > 0 && cbVideo.Items.Count > 0)
                {
                    btnAudioPlusVideo.IsEnabled = true;
                }

                btnGetMediaInfo.IsEnabled = true;

                btnAudioOnly.ClearValue(Button.BackgroundProperty);
                btnAudioOnlyMP3.ClearValue(Button.BackgroundProperty);
                btnAudioPlusVideo.ClearValue(Button.BackgroundProperty);
                btnVideoOnly.ClearValue(Button.BackgroundProperty);
                btnGetMediaInfo.ClearValue(Button.BackgroundProperty);

                dispatcherTimer.Stop();

                if (cbAudio.Items.Count == 0 && cbVideo.Items.Count == 0)
                {
                    MessageBox.Show(Util.Messages.NothingFound);
                }
            }
        }

        private void SaveDirectory_Click(object sender, RoutedEventArgs e)
        {
            Dir dir = new Dir();
            string folderPath = dir.OpenDialog();

            if (folderPath == null)
            {
                MessageBox.Show("No Folder selected");
            }
            else
            {
                if (dir.HasAccessToFolder(folderPath))
                {

                    saveToDirectory.Text = folderPath;
                }
                else
                {
                    MessageBox.Show("No write access to folder");
                    saveToDirectory.Text = "";
                }
            }
        }

        private void GetYoutubeDL_Click(object sender, RoutedEventArgs e)
        {
            btnYoutubeDLReq.Background = Brushes.OrangeRed;
            btnYoutubeDLReq.IsEnabled = false;

            PreReq preReq = new PreReq();
            preReq.DownloadYoutubeDL();
        }
    }
}
