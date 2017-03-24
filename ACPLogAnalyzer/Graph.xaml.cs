using System.Collections.Generic;
using System.Windows;

namespace ACPLogAnalyzer
{
    public enum GraphType
    {
        NoSelection,                // Place-holder
        Fwhm,                       // All FWHM measurements for the current log
        FwhmAvgAllLogs,             // Average FWHM measurements for all logs
        Hfd,                        // All HFD measurements for the current log
        HfdAvgAllLogs,              // Average HFD measurements for all logs
        FwhmvHfd,                   // Compare FWHM v HFD
        FwhmvHfdAllLogs,            // Compare FWHM v HFD for all logs
        PointErrorObj,              // All pointing errors (object slew) for the current log
        PointErrorObjAllLogs,       // Average pointing error (object slew) for all logs
        PointErrorCntr,             // All pointing errors (center slew) for the current log
        PointErrorCntrAllLogs,      // Average pointing errors (center slew) for all logs
        GuiderStartUpTime,          // All guider start-up times for current log
        GuiderStartUpTimeAllLogs,   // Average guider start-up times for all logs
        GuiderSettleTime,           // All guider settle times for current log
        GuiderSettleTimeLogAllLogs, // Average guider settle times for all logs
        FilterChangeTime,           // All filter change times for current log
        FilterChangeTimeAllLogs,    // Average filter change times for all logs
        PtExpSolveTime,             // All pointing exposure/plate solve times for the current log
        PtExpSolveTimeAllLogs,      // Average pointing exposure/plate solve times for all logs
        SlewTimeTgts,               // All slew times (targets) for the current log
        SlewTimeTgtsAllLogs,        // Average slew times (targets) for all logs
        AfTimes,                    // All auto-focus times for the current log
        AfTimesAllLogs,             // Average auto-focus times for all logs
        AllSkyTimes,                // All all-sky plate solve times for the current log
        AllSkyTimesAllLogs          // Average all-sky plate solve times for all logs
    }

    /// <summary>
    /// Interaction logic for Graph.xaml
    /// </summary>
    public partial class Graph : Window
    {
        public MainWindow MainWnd { get; set; }
        public Log Log { get; set; }

        private List<KeyValuePair<object, object>> _dataSourceSeries1;  // Set by caller to DataSource property 
        public List<KeyValuePair<object, object>> DataSourceSeries1  // string is the x-axis (normally time or date), object is the data to plot
        {
            get { return _dataSourceSeries1; }
            set { _dataSourceSeries1 = value; GraphUc.DataSourceSeries1 = _dataSourceSeries1; }
        }

        private List<KeyValuePair<object, object>> _dataSourceSeries2;  // Set by caller to DataSourceSeries2 property 
        public List<KeyValuePair<object, object>> DataSourceSeries2  
        {
            get { return _dataSourceSeries2; }
            set { _dataSourceSeries2 = value; GraphUc.DataSourceSeries2 = _dataSourceSeries2; }
        }

        private List<KeyValuePair<object, object>> _dataSourceSeries3;  // Set by caller to DataSourceSeries3 property 
        public List<KeyValuePair<object, object>> DataSourceSeries3
        {
            get { return _dataSourceSeries3; }
            set { _dataSourceSeries3 = value; GraphUc.DataSourceSeries3 = _dataSourceSeries3; }
        }

        public Graph() : this(null, null) { }
        public Graph(MainWindow wnd, Log log)
        {
            // The calling thread must be STA (the main thread) or an exception is thrown
            InitializeComponent();

            MainWnd = wnd;
            Log = log;
            GraphUc.DataPointSelectionChanged += (sender, e) =>
            {
                if (MainWnd != null && e != null)
                    MainWnd.ScrollLogToTime(e.Data, e.EventType, MapEventToLineNumber(e.Data, e.EventType));  // Scroll the main windows' log to match the datapoint selected in the graph
            }; 
        }

        private int MapEventToLineNumber(KeyValuePair<object, object> data, LogEventType logEventType)
        {
            try
            {
                foreach (var le in Log.LogEvents)
                {
                    if (le.EventType != logEventType) continue;

                    if (!le.StartDate.HasValue || le.Data == null || data.Value == null || ((double) le.Data) != ((double) data.Value)) continue;

                    var tmpEventDateTimeStr = le.StartDate.Value.ToLongTimeString();
                    if(System.String.Compare(tmpEventDateTimeStr, data.Key.ToString(), System.StringComparison.Ordinal) == 0)
                        return le.LineNumber;
                }
            }
            catch
            {
            }

            return -1;
        }
    }
}
