using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ACPLogAnalyzer
{
    #region Log class
    /// <summary>
    /// The Log class encapsulates the properties of an ACP log file, including general info and Targets
    /// </summary>
    public partial class Log
    {
        #region Private members and properties
        private string m_path = null;
        /// <summary>
        /// The physical path to the log file
        /// </summary>
        public string Path
        {
            get { return m_path; }
        }

        private Target m_currentTarget;
        /// <summary>
        /// The current target when parsing
        /// </summary>
        public Target CurrentTarget
        {
            get { return m_currentTarget; }
            set { m_currentTarget = value; }
        }

        private List<LogEvent> m_logEvents;
        /// <summary>
        /// List of events in the log (plate solves, guider failures, etc.)
        /// </summary>
        public List<LogEvent> LogEvents
        {
            get { return m_logEvents; }
            set { m_logEvents = value; }
        }

        private string m_lineLower;
        /// <summary>
        /// The current log line text converted to lower case
        /// </summary>
        public string LineLower
        {
            get { return m_lineLower; }
            set { m_lineLower = value; }
        }

        private int m_logLineIndex;
        /// <summary>
        /// The current log line index
        /// </summary>
        public int LogLineIndex
        {
            get { return m_logLineIndex; }
            set { m_logLineIndex = value; }
        }

        private List<string> m_logFileText = null;
        /// <summary>
        /// A collection of strings (one per line) that constitute the log
        /// </summary>
        public List<string> LogFileText
        {
            get { return m_logFileText; }
            set { m_logFileText = value; }
        }

        private List<Target> m_targets; 
        /// <summary>
        /// List of Targets in the log
        /// </summary>
        public List<Target> Targets
        {
            get { return m_targets; }
            set { m_targets = value; }
        }

        private bool m_isACPLog;
        /// <summary>
        /// true if the log is actually a valid ACP log, flse otherwise
        /// </summary>
        public bool IsACPLog
        {
            get { return m_isACPLog; }
            set { m_isACPLog = value; }
        }

        private DateTime? m_startDate = null;
        /// <summary>
        /// The date/time when the log run was started
        /// </summary>
        public DateTime? StartDate
        {
            get { return m_startDate; }
            set { m_startDate = value; }
        }

        private DateTime? m_endDate = null;
        /// <summary>
        /// The date/time when the log was ended
        /// </summary>
        public DateTime? EndDate
        {
            get { return m_endDate; }
            set { m_endDate = value; }
        }

        /// <summary>
        /// The number of targets in the log
        /// </summary>
        public int NumberOfTargets
        {
            get 
            {
                if (m_targets != null)
                    return m_targets.Count;
                else
                    return 0;
            }
        }

        /// <summary>
        /// List of WAIT times the log contains (e.g. for ACP #WAIT directives)
        /// </summary>
        public List<double> WaitTime
        {
            get { return GetLogList<double>(LogEventType.Wait); }
        }

        /// <summary>
        /// Returns the total WAIT time (in secs) in the log
        /// </summary>
        public double TotalWaitTime
        {
            get 
            {
                return WaitTime.Sum(); 
            }
        }
        public TimeSpan TotalWaitTimeAsTimeSpan
        {
            get
            {
                try
                {
                    return TimeSpan.FromSeconds(TotalWaitTime);
                }
                catch
                {
                }

                return new TimeSpan(0);
            }
        }

        /// <summary>
        /// Returns the total log runtime (start of observing...end). The value returned is in seconds (not ticks)
        /// </summary>
        public double TotalLogRunTime
        {
            get
            {
                double tmpTime = 0;
                try
                {
                    tmpTime = (EndDate.Value.Ticks - StartDate.Value.Ticks) / TimeSpan.TicksPerSecond;  // Convert from ticks to secs
                }
                catch
                {
                    tmpTime = 0;
                }

                return tmpTime;
            }
        }
        public TimeSpan TotalLogRunTimeAsTimeSpan
        {
            get
            {
                try
                {
                    return TimeSpan.FromSeconds(TotalLogRunTime);
                }
                catch
                {
                }

                return new TimeSpan(0);
            }
        }

        /// <summary>
        /// The toal number of imaging (not including pointing) exposures
        /// </summary>
        public double TotalExposures
        {
            get
            {
                double totalExposures = 0;
                try
                {
                    totalExposures = m_targets.Sum(tgt => tgt.ExposureSummaryList.Sum(expSummary => expSummary.Count));
                }
                catch
                {
                    totalExposures = 0;
                }

                return totalExposures;
            }
        }

        /// <summary>
        /// Returns the sum of all imaging exposure durations for the entire log. The value returned is in seconds (not ticks)
        /// </summary>
        public double TotalImagingTime
        {
            get
            {
                double totalImagingTime = 0;
                try
                {
                    // For each target, and for each ExposureSummary in the target, create a total imaging time value
                    var imagingTime =
                        from target in m_targets
                        from exposureSummary in target.ExposureSummaryList
                        select exposureSummary.Count * exposureSummary.Exposure.Duration;  // This gives us a collection of n results, one for each target

                    totalImagingTime = imagingTime.Sum();  // Sum the resuts from each target
                }
                catch
                {
                    totalImagingTime = 0;
                }

                return totalImagingTime;
            }
        }
        public TimeSpan TotalImagingTimeAsTimeSpan
        {
            get
            {
                try
                {
                    return TimeSpan.FromSeconds(TotalImagingTime);
                }
                catch
                {
                }

                return new TimeSpan(0);
            }
        }

        /// <summary>
        /// Time not spent imaging or waiting
        /// </summary>
        public double OtherTime
        {
            get
            {
                try
                {
                    return TotalLogRunTime - (TotalWaitTime + TotalImagingTime);
                }
                catch
                {
                }

                return 0;
            }
        }
        public TimeSpan OtherTimeAsTimeSpan
        {
            get
            {
                try
                {
                    return TimeSpan.FromSeconds(OtherTime);
                }
                catch
                {
                }

                return new TimeSpan(0);
            }
        }

        /// <summary>
        /// Total log runtime minus any waits
        /// </summary>
        public double RunTimeMinusWaits
        {
            get
            {
                return TotalLogRunTime - TotalWaitTime;
            }
        }
        public TimeSpan RunTimeMinusWaitsAsTimeSpan
        {
            get
            {
                try
                {
                    return TimeSpan.FromSeconds(RunTimeMinusWaits);
                }
                catch
                {
                }

                return new TimeSpan(0);
            }
        }

        /// <summary>
        /// Imaging as a percentage of runtime, excluding waits
        /// </summary>
        public double ImagingTimeAsPercentageOfRunTime
        {
            get
            {
                try
                {
                    return Math.Round((double)100 * (TotalImagingTime / RunTimeMinusWaits), 0);
                }
                catch
                {
                }

                return 0;
            }
        }

        /// <summary>
        /// Assess the consistency and reliability of log summary totals. Log errors and script aborts can cause issues when aggregating totals for all logs
        /// </summary>
        public bool LogTotalsAreConsistent
        {
            get
            {
                if (TotalLogRunTime == 0 ||
                    TotalLogRunTime < 0 ||
                    TotalImagingTime < 0 ||
                    TotalWaitTime < 0 ||
                    OtherTime < 0 ||
                    ImagingTimeAsPercentageOfRunTime > 100 ||
                    TotalLogRunTime < TotalImagingTime)

                    return false;  // Ignore the totals for this log 
                else
                    return true;  // Totals are OK
            }
        }
        #endregion

        #region Construction
        public Log() : this("", null) {}
        public Log(string path, List<string> logText)
        {
            m_path = path;
            m_currentTarget = null;
            m_logFileText = logText;
            m_logLineIndex = -1;
            m_lineLower = null;
            m_logEvents = new List<LogEvent>();
        }

        private void InitVars()
        {
            m_logLineIndex = -1;
            m_lineLower = null;
            m_isACPLog = false;
            m_startDate = null;
            m_endDate = null;
            m_logEvents = null;
        }
        #endregion

        #region Support methods
        /// <summary>
        /// Generic method that returns an event list for the specified event type
        /// </summary>
        /// <typeparam name="T">The type of data stored in the event's Data property (e.g. the list type required)</typeparam>
        /// <param name="logEventType">The event type</param>
        /// <returns>Returns an event list for the specified event type, or null if an exception occurs</returns>
        public List<T> GetLogList<T>(LogEventType logEventType)
        {
            try
            {
                var listQuery =
                    from logEvent in LogEvents
                    where ( logEvent != null &&
                            logEvent.Data != null &&
                            logEvent.EventType == logEventType)
                    select (T)logEvent.Data;

                return listQuery.ToList<T>();
            }
            catch
            {
            }

            return null;
        }
        #endregion
    }
    #endregion

    #region LogEventType enum
    /// <summary>
    /// An ACP log event (action) type
    /// </summary>
    public enum LogEventType
    {
        AutoFocus,                  // Auto-focus
        AutoFocusSuccess,           // Auto-focus success count
        AutoFocusFail,              // Auto-focus failed count
        Exposure,                   // An imaging exposure (not a pointing update exposure)
        FilterChange,               // Filter change
        FWHM,                       // FWHM measurement
        GuiderFail,                 // Guider fialure where ACP continued imaging unguided
        GuiderSettle,               // Guider settle time
        GuiderStartUp,              // Guider startup time
        HFD,                        // HFD measurement
        None,                       // No event
        PlateSolveFail,             // Plate solve failure count
        PlateSolveSuccess,          // Plate solve success count
        PointingErrorCenterSlew,    // Pointing error measurement for a center slew
        PointingErrorObjectSlew,    // Pointing error measurement for a target object slew
        PointingExpAndPlateSolve,   // Pointing update exposure (including successful plate solve)
        ScriptAbort,                // Script abort count
        ScriptError,                // Script errors count
        SlewTarget,                 // Slew time to a target
        Target,                     // A new imaging target
        Unknown,                    // An unknown event
        Wait                        // Waiting times (e.g. wait for darkness, etc.)
    }
    #endregion

    #region LogComparer class
    /// <summary>
    /// Custom comparision class to allow the sorting of Logs
    /// </summary>
    public class LogComparer : IComparer<Log>
    {
        public int Compare(Log x, Log y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    // If x is null and y is null, they're equal
                    return 0;
                }
                else
                {
                    // If x is null and y is not null, y is greater
                    return -1;
                }
            }
            else
            {
                // If x is not null...
                if (y == null)
                // ...and y is null, x is greater
                {
                    return 1;
                }
                else
                {
                    // ...and y is not null, compare the start dates of logs
                    // retval will be:  -1 if x is earlier than y
                    //                  0  if x and y are the same date
                    //                  1  if x is later than y
                    int retval = x.StartDate.Value.CompareTo(y.StartDate.Value);
                    return retval;
                }
            }

        }
    }
    #endregion

    #region LogComparerDesc class
    /// <summary>
    /// Custom comparision class to allow the sorting of Logs (descending order)
    /// </summary>
    public class LogComparerDesc : IComparer<Log>
    {
        public int Compare(Log y, Log x)
        {
            if (x == null)
            {
                if (y == null)
                {
                    // If x is null and y is null, they're equal
                    return 0;
                }
                else
                {
                    // If x is null and y is not null, y is greater
                    return -1;
                }
            }
            else
            {
                // If x is not null...
                if (y == null)
                // ...and y is null, x is greater
                {
                    return 1;
                }
                else
                {
                    // ...and y is not null, compare the start dates of logs
                    // retval will be:  -1 if x is earlier than y
                    //                  0  if x and y are the same date
                    //                  1  if x is later than y
                    int retval = x.StartDate.Value.CompareTo(y.StartDate.Value);
                    return retval;
                }
            }

        }
    }
    #endregion
}
