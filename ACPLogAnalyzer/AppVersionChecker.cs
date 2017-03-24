using System;
using System.Globalization;
using System.IO;
using System.Net;

namespace ACPLogAnalyzer
{
    public class AppVersionChecker
    {
        public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgress;
        public event EventHandler DownloadComplete;

        public bool IsUpdateAvailable { get { return AvailableVersion > RunningVersion; } }
        public bool UpdateDownloaded { get; set; }
        public double RunningVersion { get; private set; }
        public double AvailableVersion { get; private set; }

        public AppVersionChecker()
        {
            UpdateDownloaded = false;
            RunningVersion = 0;
            AvailableVersion = 0;
        }

        public void CheckForUpdate()
        {
            try
            {
                // We expect a string like: "CurrentVersion=n.nn" (where n = 0..9)
                var request = WebRequest.Create(Properties.Settings.Default.Version_Check_Url);
                var response = request.GetResponse();
                var stream = response.GetResponseStream();

                if (stream == null) return;

                var sr = new StreamReader(stream);
                var verInfo = sr.ReadToEnd();
                if (string.IsNullOrEmpty(verInfo)) return;

                var version = verInfo.Substring(verInfo.IndexOf("=", System.StringComparison.Ordinal) + 1);

                // This version is available from the web:
                AvailableVersion = double.Parse(version);

                // What's the running app's version?
                version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major.ToString(CultureInfo.InvariantCulture) + "." +
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString(CultureInfo.InvariantCulture) +
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Build.ToString(CultureInfo.InvariantCulture);

                RunningVersion = double.Parse(version);
            }
            catch
            {
            }
        }

        public void StartUpdateDownload()
        {
            try
            {
                UpdateDownloaded = false;

                // Save the installer to the desktop...
                var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (!path.EndsWith("\\")) path += "\\";
                path += System.IO.Path.GetFileName(Properties.Settings.Default.Update_Download_Url);

                using (var webClient = new WebClient())
                { 
                    webClient.DownloadFileCompleted += DownloadFileCompleted;
                    webClient.DownloadProgressChanged += DownloadProgressChanged;
                    webClient.DownloadFileAsync(new Uri(Properties.Settings.Default.Update_Download_Url), path);
                }
            }
            catch
            {
            }
        }

        private void DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            // Bubble up the event for the Update window that created us
            if (DownloadComplete != null) DownloadComplete(this, System.EventArgs.Empty);

            UpdateDownloaded = true;
        }

        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // Bubble up the event for the Update window that created us
            if(DownloadProgress != null) DownloadProgress(this, e);
        }

        public void RemoveInstaller()
        {
            try
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (!path.EndsWith("\\")) path += "\\";
                path += System.IO.Path.GetFileName(Properties.Settings.Default.Update_Download_Url);

                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }
            catch
            {
                // Fail silently 
            }
        }
    }
}
