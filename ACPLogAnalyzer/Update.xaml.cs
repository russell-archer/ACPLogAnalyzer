using System;
using System.Globalization;
using System.Windows;

namespace ACPLogAnalyzer
{
    /// <summary>
    /// Interaction logic for Help.xaml
    /// </summary>
    public partial class Update : Window
    {
        public bool ReadToInstall { get; set; }
        public bool Downloading { get; set; }
        public AppVersionChecker AppVersionChecker { get; set; }

        public Update(AppVersionChecker avc)
        {
            InitializeComponent();

            Downloading = false;
            ReadToInstall = false;
            AppVersionChecker = avc;
        }

        private void ButtonCloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void WindowActivated(object sender, EventArgs e)
        {
            ProgressBarDownload.Visibility = System.Windows.Visibility.Hidden;

            AppVersionChecker.CheckForUpdate();
            TextBoxRunning.Text = AppVersionChecker.RunningVersion.ToString(CultureInfo.InvariantCulture);
            TextBoxAvailable.Text = AppVersionChecker.AvailableVersion.ToString(CultureInfo.InvariantCulture);

            if (AppVersionChecker.IsUpdateAvailable) LabelTitle.Content = "An update is available";
            else
            {
                ButtonClose.Content = "Close";
                ButtonDownloadUpdate.Visibility = System.Windows.Visibility.Hidden;
                LabelTitle.Content = "You are running the latest version";
            }
        }

        private void ButtonDownloadUpdateClick(object sender, RoutedEventArgs e)
        {
            if (!Downloading)
            {
                Downloading = true;
                ButtonClose.Visibility = System.Windows.Visibility.Hidden;
                ButtonDownloadUpdate.Content = "Stop Download";
                LabelTitle.Visibility = System.Windows.Visibility.Visible;
                ProgressBarDownload.Visibility = System.Windows.Visibility.Visible;

                AppVersionChecker.DownloadProgress += (o, args) =>
                {
                    LabelTitle.Content = "Downloading (" + args.ProgressPercentage.ToString(CultureInfo.InvariantCulture) + "%)...";
                    ProgressBarDownload.Value = args.ProgressPercentage;
                };

                AppVersionChecker.DownloadComplete += DownloadComplete;

                AppVersionChecker.StartUpdateDownload();
            }
            else if (ReadToInstall)
            {
                // Quit and do the install
                var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (!path.EndsWith("\\")) path += "\\";
                path += System.IO.Path.GetFileName(Properties.Settings.Default.Update_Download_Url);

                if (System.IO.File.Exists(path))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(path));
                    Application.Current.Shutdown();
                }
            }
            else
            {
                this.Close();
            }
        }

        private void DownloadComplete(object sender, EventArgs e)
        {
            ReadToInstall = true;
            ButtonDownloadUpdate.Content = "Quit and Install Update";
            ButtonClose.Content = "Don't Install Update";
            ButtonClose.Visibility = System.Windows.Visibility.Visible;
            LabelTitle.Content = "Ready to install update (downloaded)";
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Release notes...
            WebBrowserRelNotes.Source = new Uri(Properties.Settings.Default.Release_Notes_Url);
        }

        private void WebBrowserRelNotesLoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            WebBrowserRelNotes.Refresh(true);
        }
    }
}
