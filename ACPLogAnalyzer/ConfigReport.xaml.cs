using System;
using System.Windows;

namespace ACPLogAnalyzer
{
    /// <summary>
    /// Interaction logic for ConfigReport.xaml
    /// </summary>
    public partial class ConfigReport : Window
    {
        public ConfigReport()
        {
            InitializeComponent();
        }

        private void ButtonCloseClick(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void ButtonHelpClick(object sender, RoutedEventArgs e)
        {
            var uri = new Uri(Properties.Settings.Default.Help_Text_Config_Url);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(uri.AbsoluteUri));
        }
    }
}
