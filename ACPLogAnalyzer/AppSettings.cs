namespace ACPLogAnalyzer
{
    /// <summary>
    /// Class to read and write all the properties used by the application to/from the registry
    /// </summary>
    class AppSettings
    {
        public string DocRoot { get; set; }
        public string LogDir { get; set; }

        public AppSettings()
        {
            this.DocRoot = "";
            this.LogDir = "";
        }

        public void ReadRegistry()
        {
            try
            {
                // Get -> HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Denny\ACP\WebServer
                var key = Microsoft.Win32.Registry.LocalMachine.
                             OpenSubKey("Software", false).
                             OpenSubKey("Denny", false).
                             OpenSubKey("ACP", false).
                             OpenSubKey("WebServer", false);

                if (key != null)
                {
                    DocRoot = key.GetValue("DocRoot") as string;
                    key.Close();
                }

                // Get -> HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Denny\ACP\UserData\local_user
                key = Microsoft.Win32.Registry.LocalMachine.
                    OpenSubKey("Software", false).
                    OpenSubKey("Denny", false).
                    OpenSubKey("ACP", false).
                    OpenSubKey("UserData", false).
                    OpenSubKey("local_user", false);

                if (key == null) return;
                LogDir = key.GetValue("LogDir") as string;
                key.Close();
            }
            catch
            {
            }
        }
    }
}