using System;
using System.Globalization;
using System.Windows;

namespace ACPLogAnalyzer
{
    /// <summary>
    /// Interaction logic for Help.xaml
    /// </summary>
    public partial class Help : Window
    {
        public Help()
        {
            InitializeComponent();

            // Format: "v.mb" (e.g. "1.21"), where v = version, m = minor version, b = build
            LabelVersion.Content = "Version: " +
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major.ToString(CultureInfo.InvariantCulture) +
                "." +
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString(CultureInfo.InvariantCulture) +
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Build.ToString(CultureInfo.InvariantCulture);

            // Release notes...
            WebBrowserRelNotes.Source = new Uri(Properties.Settings.Default.Release_Notes_Url);
        }

        private void HyperlinkRequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void WebBrowserRelNotesLoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            WebBrowserRelNotes.Refresh(true);
        }

        private void ButtonOkClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
