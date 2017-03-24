using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ACPLogAnalyzer
{
    public class Report
    {
        #region Private members and Properties
        private LogComparer m_compareLogs = null;                    // Custom comparison
        private LogComparerDesc m_compareLogsDesc = null;            // Custom comparison
        private LogReader m_logReader = null;                        // Object that deals with reading a log from disk
        private List<string> m_reportText = null;                    // Holds the text of the report which will be written to disk
        private List<LogEvent> m_allLogEvents = null;                // Holds all events from all logs
        private LogCollectionSummary m_logCollection = null;         // Holds data about the collection of logs (totals, etc.)
        private LogCollectionSummary LogCollection
        {
            get { return m_logCollection; }
            set { m_logCollection = value; }
        }                 // Calculates overall total, averages, etc. from log data

        private List<Log> m_logs = null;
        /// <summary>
        /// List of Logs (each Log encapsulates a physical ACP log file)
        /// </summary>
        public List<Log> Logs
        {
            get { return m_logs; }
            set { m_logs = value; }
        }

        private List<string> m_logFilenames = null;
        /// <summary>
        /// List of log filenames to report on
        /// </summary>
        public List<string> LogFilenames
        {
            get { return m_logFilenames; }
            set { m_logFilenames = value; }
        }

        private bool m_parsingLogs = false;
        /// <summary>
        /// Flag used to stop parsing
        /// </summary>
        public bool ParsingLogs
        {
            get { return m_parsingLogs; }
            set { m_parsingLogs = value; }
        }

        private string m_reportName = null;
        /// <summary>
        /// Filename for the report's text
        /// </summary>
        public string ReportName
        {
            get { return m_reportName; }
            set { m_reportName = value; }
        }
        #endregion

        #region Construction
        public Report() : this(null)
        {
        }

        public Report(List<string> logFiles)
        {
            m_logFilenames = logFiles;
        }
        #endregion

        /// <summary>
        /// Create a text report by parsing one or more logs and writing report details to disk
        /// </summary>
        /// <param name="path">The path to a single log, or null if all logs are to be parsed</param>
        /// <param name="forGraph">true if the data is intended for graphing, false otherwise</param>
        /// <returns>Returns the full path to the file containing the report</returns>
        public string CreateReport(string path, bool forGraph)
        {
            if (!ReadAllLogsAndParse(path))
                return null; 

            if (!SortLogs())
                return null; 

            if (forGraph)
                return null;  // Data is now setup ready for graphing - no further text processing required

            if(!OutputLogDetailReport())
                return null;

            if (!OutputLogSummaryReport())
                return null;

            if (!SaveReport())
                return null; 

            return ReportName;  // The filename of the repot
        }

        /// <summary>
        /// Parse a single log, or all logs (if path is null)
        /// </summary>
        /// <param name="path">The path to the log to be parsed, or null if all logs are to be parsed</param>
        /// <returns></returns>
        private bool ReadAllLogsAndParse(string path)
        {
            Log log = null;
            m_logs = new List<Log>();
            m_logCollection = new LogCollectionSummary();
            m_logCollection.Logs = m_logs;
            m_allLogEvents = new List<LogEvent>();
            m_compareLogs = new LogComparer();
            m_compareLogsDesc = new LogComparerDesc();

            if (!m_parsingLogs)  // User stopped the process
                return false;

            if (string.IsNullOrEmpty(path))
            {
                // Parse all logs...
                foreach (string logPath in m_logFilenames)
                {
                    if (!m_parsingLogs)  // User stopped the process
                        return false;

                    try
                    {
                        log = ReadLogAndParse(logPath);  // Get the Log object to handle the parsing of its own file

                        if (log != null && !log.IsACPLog)
                            continue;  // Not valid ACP log

                        m_logs.Add(log);
                    }
                    catch
                    {
                        continue;
                    }
                }

                return true;
            }
            else
            {
                // Parse a single log...
                try
                {
                    log = ReadLogAndParse(path);

                    if (log != null && log.IsACPLog)
                        m_logs.Add(log);

                    return true;
                }
                catch
                {
                }
            }

            return false;
        }

        /// <summary>
        /// Reads a file from disk and then parses it for log properties
        /// </summary>
        /// <param name="filename">The path to the file to be parsed</param>
        /// <returns>Returns a Log object containing properties for the log</returns>
        private Log ReadLogAndParse(string filename)
        {
            Log log = null;
            try
            {
                m_logReader = new LogReader(filename);
                if (!m_logReader.ReadLogFile())
                    return null;

                // Create a new Log object and give it the text of its underlying log file
                log = new Log(filename, m_logReader.LogFileText);

                // Parse the log's text - after this call the Log object will be populated with the various log properties
                if (!log.ParseLog())
                    return null;

                AddAllLogEvents(log);  // Add all the events from the log to our master list of all events
            }
            catch
            {
            }

            return log;
        }

        /// <summary>
        /// Sorts the list of logs into order
        /// </summary>
        /// <returns>Returns true if the sort was OK, false on an error</returns>
        private bool SortLogs()
        {
            // Do we need to sort the list of logs by date?
            try
            {
                if (Properties.Settings.Default.Sort_By_Date)
                {
                    if (Properties.Settings.Default.Sort_By_Date_Asc)
                        m_logs.Sort(m_compareLogs);
                    else
                        m_logs.Sort(m_compareLogsDesc);
                }

                return true;
            }
            catch
            {
            }

            return false;
        }

        /// <summary>
        /// Adds all log events to the master list of all events for all logs
        /// </summary>
        /// <param name="lg">The log to be enumerated for events</param>
        private void AddAllLogEvents(Log lg)
        {
            if (lg.LogEvents != null && lg.LogEvents.Count > 0 && m_allLogEvents != null)
            {
                foreach (LogEvent le in lg.LogEvents)
                    m_allLogEvents.Add(le);  // Add the event to the overall list of events
            }
        }

        /// <summary>
        /// Enumerate all log objects and output details to the report
        /// </summary>
        private bool OutputLogDetailReport()
        {
            bool ignoreCurrentLogTotals = false;
            m_reportText = new List<string>();  // Holds the text of the report

            try
            {
                foreach (Log lg in m_logs)
                {
                    if (!m_parsingLogs)  // User stopped the process
                        return false;

                    if (Properties.Settings.Default.Ignore_Logs_With_Zero_Targets)
                    {
                        if (lg.Targets.Count == 0)
                            continue;
                    }

                    // Output properties for the log as a whole...
                    if (Properties.Settings.Default.Show_Log_Name)
                        m_reportText.Add("Log: " + System.IO.Path.GetFileName(lg.Path));

                    if (Properties.Settings.Default.Show_Start_Date)
                        m_reportText.Add("Start date: " + lg.StartDate.ToString());

                    if (Properties.Settings.Default.Show_End_Date)
                        m_reportText.Add("End date: " + lg.EndDate.ToString());

                    if (Properties.Settings.Default.Runtime_Breakdown)
                    {
                        m_reportText.Add("Log runtime breakdown:");

                        if (ignoreCurrentLogTotals)
                        {
                            m_reportText.Add("  Inconsistent results");
                            m_reportText.Add("  Runtime totals for this log will be discarded");
                        }
                        else
                        {
                            m_reportText.Add("  Runtime: " +
                                lg.TotalLogRunTimeAsTimeSpan.Days.ToString() + "d " +
                                lg.TotalLogRunTimeAsTimeSpan.Hours.ToString() + "h " +
                                lg.TotalLogRunTimeAsTimeSpan.Minutes.ToString() + "m");

                            m_reportText.Add("  Imaging time: " +
                                lg.TotalImagingTimeAsTimeSpan.Days.ToString() + "d " +
                                lg.TotalImagingTimeAsTimeSpan.Hours.ToString() + "h " +
                                lg.TotalImagingTimeAsTimeSpan.Minutes.ToString() + "m");

                            m_reportText.Add("  Wait time: " +
                                lg.TotalWaitTimeAsTimeSpan.Days.ToString() + "d " +
                                lg.TotalWaitTimeAsTimeSpan.Hours.ToString() + "h " +
                                lg.TotalWaitTimeAsTimeSpan.Minutes.ToString() + "m");

                            m_reportText.Add("  Observatory overhead: " +
                                lg.OtherTimeAsTimeSpan.Days.ToString() + "d " +
                                lg.OtherTimeAsTimeSpan.Hours.ToString() + "h " +
                                lg.OtherTimeAsTimeSpan.Minutes.ToString() + "m");

                            m_reportText.Add("  Imaging as % of runtime (ex waits): " +
                                lg.ImagingTimeAsPercentageOfRunTime.ToString() + "%");
                        }
                    }

                    if (Properties.Settings.Default.Show_Target_Details)
                        m_reportText.Add("Targets: " + lg.NumberOfTargets.ToString());

                    // Output details for each target in the log...
                    foreach (Target t in lg.Targets)
                    {
                        if (Properties.Settings.Default.Ignore_Targets_With_Zero_Completed_Exposures && t.NumberOfExposures == 0)
                            continue;

                        if (Properties.Settings.Default.Show_Target_Details)
                        {
                            if (Properties.Settings.Default.Show_Target_Details && Properties.Settings.Default.Show_Target_Name)
                            {
                                m_reportText.Add("");
                                m_reportText.Add("  Target name: " + t.Name);
                            }

                            if (Properties.Settings.Default.Show_Target_Details && Properties.Settings.Default.Average_FWHM)
                            {
                                if (t.AvgFWHM > 0) m_reportText.Add("  Average FWHM: " + t.AvgFWHM.ToString() + " arcsecs");
                                else m_reportText.Add("  Average FWHM: N/A");
                            }

                            if (Properties.Settings.Default.Show_Target_Details && Properties.Settings.Default.Average_HFD)
                            {
                                if (t.AvgHFD > 0) m_reportText.Add("  Average HFD: " + t.AvgHFD.ToString());
                                else m_reportText.Add("  Average HFD: N/A");
                            }

                            if (Properties.Settings.Default.Average_Pointing_Error_Slew ||
                                Properties.Settings.Default.Average_Pointing_Error_Center)
                                m_reportText.Add("  Average Pointing Error: ");

                            if (Properties.Settings.Default.Average_Pointing_Error_Slew)
                            {
                                if (t.AvgPointingErrorObjectSlew > 0) m_reportText.Add("    Object slews: " + t.AvgPointingErrorObjectSlew.ToString() + " arcmins");
                                else m_reportText.Add("    Object slews: N/A");
                            }

                            if (Properties.Settings.Default.Average_Pointing_Error_Center)
                            {
                                if (t.AvgPointingErrorCenterSlew > 0) m_reportText.Add("    Center slews: " + t.AvgPointingErrorCenterSlew.ToString() + " arcmins");
                                else m_reportText.Add("    Center slews: N/A");
                            }

                            if (Properties.Settings.Default.Average_Auto_Focus_Time)
                            {
                                if (t.AvgAutoFocusTime > 0)
                                    m_reportText.Add("  Average auto-focus time: " + t.AvgAutoFocusTime.ToString() + " secs");
                                else
                                    m_reportText.Add("  Average auto-focus time: N/A");
                            }

                            if (Properties.Settings.Default.Average_Guider_StartUp_Time)
                            {
                                if (t.AvgGuiderStartUpTime.Ticks > 0) m_reportText.Add("  Average guider start-up time: " + Math.Round(t.AvgGuiderStartUpTime.TotalSeconds, 0).ToString() + " secs");
                                else m_reportText.Add("  Average guider start-up time: N/A");
                            }

                            if (Properties.Settings.Default.Average_Guider_Settle_Time)
                            {
                                if (t.AvgGuiderSettleTime.Ticks > 0) m_reportText.Add("  Average guider settle time: " + Math.Round(t.AvgGuiderSettleTime.TotalSeconds, 0).ToString() + " secs");
                                else m_reportText.Add("  Average guider settle time: N/A");
                            }

                            if (Properties.Settings.Default.Average_Filter_Change_Time)
                            {
                                if (t.AvgFilterChangeTime.Ticks > 0) m_reportText.Add("  Average filter change time: " + Math.Round(t.AvgFilterChangeTime.TotalSeconds, 0).ToString() + " secs");
                                else m_reportText.Add("  Average filter change time: N/A");
                            }

                            if (Properties.Settings.Default.Average_Slew_Time_Targets)
                            {
                                if (t.AvgSlewTime.Ticks > 0) m_reportText.Add("  Average slew time (targets): " + Math.Round(t.AvgSlewTime.TotalSeconds, 0).ToString() + " secs");
                                else m_reportText.Add("  Average slew time (targets): N/A");
                            }

                            if (Properties.Settings.Default.Average_Plate_Solve_Time)
                            {
                                if (t.AvgPlateSolveTime.Ticks > 0) m_reportText.Add("  Average pointing exposure/plate solve time: " + Math.Round(t.AvgPlateSolveTime.TotalSeconds, 0).ToString() + " secs");
                                else m_reportText.Add("  Average pointing exposure/plate solve time: N/A");
                            }

                            if (Properties.Settings.Default.Script_Aborts) m_reportText.Add("  Script aborts: " + t.ScriptAborts.ToString());
                            if (Properties.Settings.Default.Script_Errors) m_reportText.Add("  Script error: " + t.ScriptErrors.ToString());
                            if (Properties.Settings.Default.Successful_AutoFocus) m_reportText.Add("  Successful auto-focus runs: " + t.SuccessfulAutoFocus.ToString());
                            if (Properties.Settings.Default.UnSuccessful_AutoFocus) m_reportText.Add("  Unsuccessful auto-focus runs: " + t.UnsuccessfulAutoFocus.ToString());
                            if (Properties.Settings.Default.Successful_Plate_Solves) m_reportText.Add("  Successful plate solves: " + t.SuccessfulPlateSolves.ToString());
                            if (Properties.Settings.Default.UnSuccessful_Plate_Solves) m_reportText.Add("  Unsuccessful plate solves: " + t.UnsuccessfulPlateSolves.ToString());
                            if (Properties.Settings.Default.Show_Completed_Exposure_Details) m_reportText.Add("  " + "Completed exposures: " + t.NumberOfExposures.ToString());
                            if (Properties.Settings.Default.Show_Total_Guiding_Failures)
                            {
                                m_reportText.Add("  " + "Guider failures: " + t.GuidingFailures.ToString());
                                foreach (LogEvent le in lg.LogEvents)
                                {
                                    if (le.EventType == LogEventType.GuiderFail)
                                    {
                                        m_reportText.Add("    " + " Failed at " + le.StartDate.Value.ToLongTimeString());
                                    }
                                }
                            }

                            if (Properties.Settings.Default.Show_Total_Imaging_Time_Per_Target)
                            {
                                m_reportText.Add("  " + "Total imaging time on target: " +
                                    lg.TotalImagingTimeAsTimeSpan.Hours.ToString() + "h " +
                                    lg.TotalImagingTimeAsTimeSpan.Minutes.ToString() + "m " +
                                    lg.TotalImagingTimeAsTimeSpan.Seconds.ToString() + "s ");
                            }
                        }

                        if (t.ExposureSummaryList == null || t.ExposureSummaryList.Count == 0)
                            continue;

                        foreach (ExposureSummary summary in t.ExposureSummaryList)
                        {
                            if (Properties.Settings.Default.Show_Target_Details &&
                                Properties.Settings.Default.Show_Completed_Exposure_Details)
                            {
                                m_reportText.Add(
                                    "    " +
                                    summary.Count.ToString() +
                                    " x " +
                                    summary.Exposure.Duration.ToString() +
                                    " secs " +
                                    summary.Exposure.Filter +
                                    " @ bin " +
                                    summary.Exposure.Bin.ToString());
                            }
                        }
                    }

                    m_reportText.Add("");
                    m_reportText.Add("--------------------------------------------------------");
                    m_reportText.Add("");
                }
            }
            catch
            {
            }

            return true;
        }

        /// <summary>
        /// Output a summary of all the logs parsed
        /// </summary>
        /// <returns></returns>
        private bool OutputLogSummaryReport()
        {
            if (Properties.Settings.Default.Show_Report_Summary)
            {
                #region Output summary at bottom of report
                if (Properties.Settings.Default.Show_Report_Summary_At_Bottom)
                {
                    try
                    {
                        m_reportText.Add("Report Summary");
                        m_reportText.Add("");

                        if (Properties.Settings.Default.Show_Total_Logs_Parsed)
                            m_reportText.Add("Total number of ACP logs parsed: " + m_logs.Count.ToString());

                        if (Properties.Settings.Default.Show_Total_Targets)
                            m_reportText.Add("Total number of unique targets: " + LogCollection.UniqueTargets.Count.ToString());

                        if (Properties.Settings.Default.Show_Total_Image_Count)
                            m_reportText.Add("Total number of images taken: " + LogCollection.Exposures.ToString());

                        if (Properties.Settings.Default.Runtime_Breakdown_Overall)
                        {
                            m_reportText.Add("Overall runtime breakdown:");

                            if (!LogCollection.TotalsAreConsistent)
                                m_reportText.Add("  Inconsistent results");
                            else
                            {
                                m_reportText.Add("  Runtime: " +
                                    LogCollection.RunTimeAsTimeSpan.Days.ToString() + "d " +
                                    LogCollection.RunTimeAsTimeSpan.Hours.ToString() + "h " +
                                    LogCollection.RunTimeAsTimeSpan.Minutes.ToString() + "m");

                                m_reportText.Add("  Imaging time: " +
                                    LogCollection.ImagingTimeAsTimeSpan.Days.ToString() + "d " +
                                    LogCollection.ImagingTimeAsTimeSpan.Hours.ToString() + "h " +
                                    LogCollection.ImagingTimeAsTimeSpan.Minutes.ToString() + "m");

                                m_reportText.Add("  Wait time: " +
                                    LogCollection.WaitTimeAsTimeSpan.Days.ToString() + "d " +
                                    LogCollection.WaitTimeAsTimeSpan.Hours.ToString() + "h " +
                                    LogCollection.WaitTimeAsTimeSpan.Minutes.ToString() + "m");

                                m_reportText.Add("  Observatory overhead: " +
                                    LogCollection.OtherTimeAsTimeSpan.Days.ToString() + "d " +
                                    LogCollection.OtherTimeAsTimeSpan.Hours.ToString() + "h " +
                                    LogCollection.OtherTimeAsTimeSpan.Minutes.ToString() + "m");

                                m_reportText.Add("  Imaging as % of runtime (ex waits): " +
                                    LogCollection.ImagingTimeAsPercentageOfRunTime.ToString() + "%");
                            }
                        }

                        if (Properties.Settings.Default.Average_FWHM_Overall)
                        {
                            if (LogCollection.AverageFWHM == 0) m_reportText.Add("Average FWHM: N/A");
                            else m_reportText.Add("Average FWHM: " + LogCollection.AverageFWHM.ToString() + " arcsecs");
                        }

                        if (Properties.Settings.Default.Average_HFD_Overall)
                        {
                            if (LogCollection.AverageHFD == 0) m_reportText.Add("Average HFD: N/A");
                            else m_reportText.Add("Average HFD: " + LogCollection.AverageHFD.ToString());
                        }

                        if (Properties.Settings.Default.Average_Plate_Solve_Time_Overall)
                        {
                            if (LogCollection.AveragePlateSolveTime == 0) m_reportText.Add("Average pointing exposure/plate solve time: N/A");
                            else m_reportText.Add("Average pointing exposure/plate solve time: " + LogCollection.AveragePlateSolveTimeAsTimeSpan.TotalSeconds.ToString() + " secs");
                        }

                        if (Properties.Settings.Default.Average_Slew_Time_Targets_Overall)
                        {
                            if (LogCollection.AverageSlewTime == 0) m_reportText.Add("Average slew time (targets): N/A");
                            else m_reportText.Add("Average slew time (targets): " + LogCollection.AverageSlewTimeAsTimeSpan.TotalSeconds.ToString() + " secs");
                        }

                        if (Properties.Settings.Default.Average_Filter_Change_Time_Overall)
                        {
                            if (LogCollection.AverageFilterChangeTime == 0) m_reportText.Add("Average filter change time: N/A");
                            else m_reportText.Add("Average filter change time: " + LogCollection.AverageFilterChangeTimeAsTimeSpan.TotalSeconds.ToString() + " secs");
                        }

                        if (Properties.Settings.Default.Average_Guider_Settle_Time_Overall)
                        {
                            if (LogCollection.AverageGuiderSettleTime == 0) m_reportText.Add("Average guider settle time: N/A");
                            else m_reportText.Add("Average guider settle time: " + LogCollection.AverageGuiderSettleTimeAsTimeSpan.TotalSeconds.ToString() + " secs");
                        }

                        if (Properties.Settings.Default.Average_Guider_StartUp_Time_Overall)
                        {
                            if (LogCollection.AverageGuiderStartUpTime == 0) m_reportText.Add("Average guider start-up time: N/A");
                            else m_reportText.Add("Average guider start-up time: " + LogCollection.AverageGuiderStartUpTimeAsTimeSpan.TotalSeconds.ToString() + " secs");
                        }

                        if (Properties.Settings.Default.Average_Pointing_Error_Slew_Overall)
                        {
                            if (LogCollection.AveragePointingErrorObject == 0) m_reportText.Add("Average pointing error (object slew): N/A");
                            else m_reportText.Add("Average pointing error (object slew): " + LogCollection.AveragePointingErrorObject.ToString() + " arcsecs");
                        }

                        if (Properties.Settings.Default.Average_Pointing_Error_Center_Overall)
                        {
                            if (LogCollection.AveragePointingErrorCenter == 0) m_reportText.Add("Average pointing error (center slew): N/A");
                            else m_reportText.Add("Average pointing error (center slew): " + LogCollection.AveragePointingErrorCenter.ToString() + " arcsecs");
                        }

                        if (Properties.Settings.Default.Average_Auto_Focus_Time_Overall)
                        {
                            if (LogCollection.AverageAutoFocusTime == 0) m_reportText.Add("Average auto-focus time: N/A");
                            else m_reportText.Add("Average auto-focus time: " + LogCollection.AverageAutoFocusTime.ToString() + " secs");
                        }

                        if (Properties.Settings.Default.Successful_AutoFocus_Overall)
                            m_reportText.Add("Total successful auto-focus runs: " + LogCollection.SuccessfulAutoFocusCount.ToString());

                        if (Properties.Settings.Default.UnSuccessful_AutoFocus_Overall)
                            m_reportText.Add("Total unsuccessful auto-focus runs: " + LogCollection.UnsuccessfulAutoFocusCount.ToString());

                        if (Properties.Settings.Default.Successful_Plate_Solves_Overall)
                            m_reportText.Add("Total successful plate solves: " + LogCollection.SuccessfulPlateSolveCount.ToString());

                        if (Properties.Settings.Default.UnSuccessful_Plate_Solves_Overall)
                            m_reportText.Add("Total unsuccessful plate solves: " + LogCollection.UnsuccessfulPlateSolveCount.ToString());

                        if (Properties.Settings.Default.Show_Total_Guiding_Failures_Overall)
                        {
                            m_reportText.Add("  Total guider failures: " + LogCollection.TotalGuiderFailureCount.ToString());
                            foreach (LogEvent le in m_allLogEvents)
                            {
                                if (le.EventType == LogEventType.GuiderFail)
                                {
                                    m_reportText.Add("    " +
                                        System.IO.Path.GetFileName(le.Path) +
                                        " (" +
                                        le.Target.Name.Trim() +
                                        ") at " +
                                        le.StartDate.Value.ToLongTimeString());
                                }
                            }
                        }

                        return true;
                    }
                    catch
                    {
                    }
                }
                #endregion

                #region Output summary at top of report
                else
                {
                    try
                    {
                        m_reportText.Insert(0, "");
                        m_reportText.Insert(0, "--------------------------------------------------------");
                        m_reportText.Insert(0, "");

                        if (Properties.Settings.Default.Show_Total_Guiding_Failures_Overall)
                        {
                            foreach (LogEvent le in m_allLogEvents)
                            {
                                if (le.EventType == LogEventType.GuiderFail)
                                {
                                    m_reportText.Insert(0, "    " +
                                        System.IO.Path.GetFileName(le.Path) +
                                        " (" +
                                        le.Target.Name.Trim() +
                                        ") at " +
                                        le.StartDate.Value.ToLongTimeString());
                                }
                            }
                            m_reportText.Insert(0, "  Total guider failures: " + LogCollection.TotalGuiderFailureCount.ToString());
                        }

                        if (Properties.Settings.Default.UnSuccessful_Plate_Solves_Overall)
                            m_reportText.Insert(0, "Total unsuccessful plate solves: " + LogCollection.UnsuccessfulPlateSolveCount.ToString());

                        if (Properties.Settings.Default.Successful_Plate_Solves_Overall)
                            m_reportText.Insert(0, "Total successful plate solves: " + LogCollection.SuccessfulPlateSolveCount.ToString());

                        if (Properties.Settings.Default.UnSuccessful_AutoFocus_Overall)
                            m_reportText.Insert(0, "Total unsuccessful auto-focus runs: " + LogCollection.UnsuccessfulAutoFocusCount.ToString());

                        if (Properties.Settings.Default.Successful_AutoFocus_Overall)
                            m_reportText.Insert(0, "Total successful auto-focus runs: " + LogCollection.SuccessfulAutoFocusCount.ToString());

                        if (Properties.Settings.Default.Average_Plate_Solve_Time_Overall)
                        {
                            if (LogCollection.AveragePlateSolveTime == 0) m_reportText.Insert(0, "Average pointing exposure/plate solve time: N/A");
                            else m_reportText.Insert(0, "Average pointing exposure/plate solve time: " + LogCollection.AveragePlateSolveTimeAsTimeSpan.TotalSeconds.ToString() + " secs");
                        }

                        if (Properties.Settings.Default.Average_Slew_Time_Targets_Overall)
                        {
                            if (LogCollection.AverageSlewTime == 0) m_reportText.Insert(0, "Average slew time (targets): N/A");
                            else m_reportText.Insert(0, "Average slew time (targets): " + LogCollection.AverageSlewTimeAsTimeSpan.TotalSeconds.ToString() + " secs");
                        }

                        if (Properties.Settings.Default.Average_Filter_Change_Time_Overall)
                        {
                            if (LogCollection.AverageFilterChangeTime == 0) m_reportText.Insert(0, "Average filter change time: N/A");
                            else m_reportText.Insert(0, "Average filter change time: " + LogCollection.AverageFilterChangeTimeAsTimeSpan.TotalSeconds.ToString() + " secs");
                        }

                        if (Properties.Settings.Default.Average_Guider_Settle_Time_Overall)
                        {
                            if (LogCollection.AverageGuiderSettleTime == 0) m_reportText.Insert(0, "Average guider settle time: N/A");
                            else m_reportText.Insert(0, "Average guider settle time: " + LogCollection.AverageGuiderSettleTimeAsTimeSpan.TotalSeconds.ToString() + " secs");
                        }

                        if (Properties.Settings.Default.Average_Guider_StartUp_Time_Overall)
                        {
                            if (LogCollection.AverageGuiderStartUpTime == 0) m_reportText.Insert(0, "Average guider start-up time: N/A");
                            else m_reportText.Insert(0, "Average guider start-up time: " + LogCollection.AverageGuiderStartUpTimeAsTimeSpan.TotalSeconds.ToString() + " secs");
                        }

                        if (Properties.Settings.Default.Average_Auto_Focus_Time_Overall)
                        {
                            if (LogCollection.AverageAutoFocusTime == 0) m_reportText.Insert(0, "Average auto-focus time: N/A");
                            else m_reportText.Insert(0, "Average auto-focus time: " + LogCollection.AverageAutoFocusTime.ToString() + " secs");
                        }

                        if (Properties.Settings.Default.Average_Pointing_Error_Center_Overall)
                        {
                            if (LogCollection.AveragePointingErrorCenter == 0) m_reportText.Insert(0, "Average pointing error (center slew): N/A");
                            else m_reportText.Insert(0, "Average pointing error (center slew): " + LogCollection.AveragePointingErrorCenter.ToString() + " arcsecs");
                        }

                        if (Properties.Settings.Default.Average_Pointing_Error_Slew_Overall)
                        {
                            if (LogCollection.AveragePointingErrorObject == 0) m_reportText.Insert(0, "Average pointing error (object slew): N/A");
                            else m_reportText.Insert(0, "Average pointing error (object slew): " + LogCollection.AveragePointingErrorObject.ToString() + " arcsecs");
                        }

                        if (Properties.Settings.Default.Average_HFD_Overall)
                        {
                            if (LogCollection.AverageHFD == 0) m_reportText.Insert(0, "Average HFD: N/A");
                            else m_reportText.Insert(0, "Average HFD: " + LogCollection.AverageHFD.ToString());
                        }

                        if (Properties.Settings.Default.Average_FWHM_Overall)
                        {
                            if (LogCollection.AverageFWHM == 0) m_reportText.Insert(0, "Average FWHM: N/A");
                            else m_reportText.Insert(0, "Average FWHM: " + LogCollection.AverageFWHM.ToString() + " arcsecs");
                        }

                        if (Properties.Settings.Default.Runtime_Breakdown_Overall)
                        {
                            if (!LogCollection.TotalsAreConsistent)
                                m_reportText.Insert(0, "  Inconsistent results");
                            else
                            {
                                m_reportText.Insert(0, "  Imaging as % of runtime (ex waits): " +
                                    LogCollection.ImagingTimeAsPercentageOfRunTime.ToString() + "%");

                                m_reportText.Insert(0, "  Observatory overhead: " +
                                    LogCollection.OtherTimeAsTimeSpan.Days.ToString() + "d " +
                                    LogCollection.OtherTimeAsTimeSpan.Hours.ToString() + "h " +
                                    LogCollection.OtherTimeAsTimeSpan.Minutes.ToString() + "m");

                                m_reportText.Insert(0, "  Wait time: " +
                                    LogCollection.WaitTimeAsTimeSpan.Days.ToString() + "d " +
                                    LogCollection.WaitTimeAsTimeSpan.Hours.ToString() + "h " +
                                    LogCollection.WaitTimeAsTimeSpan.Minutes.ToString() + "m");

                                m_reportText.Insert(0, "  Imaging time: " +
                                    LogCollection.ImagingTimeAsTimeSpan.Days.ToString() + "d " +
                                    LogCollection.ImagingTimeAsTimeSpan.Hours.ToString() + "h " +
                                    LogCollection.ImagingTimeAsTimeSpan.Minutes.ToString() + "m");

                                m_reportText.Insert(0, "  Runtime: " +
                                    LogCollection.RunTimeAsTimeSpan.Days.ToString() + "d " +
                                    LogCollection.RunTimeAsTimeSpan.Hours.ToString() + "h " +
                                    LogCollection.RunTimeAsTimeSpan.Minutes.ToString() + "m");
                            }

                            m_reportText.Insert(0, "Overall runtime breakdown:");
                        }

                        if (Properties.Settings.Default.Show_Total_Image_Count)
                            m_reportText.Insert(0, "Total number of images taken: " + LogCollection.Exposures.ToString());

                        if (Properties.Settings.Default.Show_Total_Targets)
                            m_reportText.Insert(0, "Total number of unique targets: " + LogCollection.UniqueTargets.Count.ToString());

                        if (Properties.Settings.Default.Show_Total_Logs_Parsed)
                            m_reportText.Insert(0, "Total number of ACP logs parsed: " + m_logs.Count.ToString());

                        m_reportText.Insert(0, "");
                        m_reportText.Insert(0, "Report Summary");

                        return true;
                    }
                    catch 
                    { 
                    }
                }
                #endregion
            }

            return false;
        }

        /// <summary>
        /// Saves the report to disk using a unique temporary filename. The name of the file will be in the ReportName property
        /// </summary>
        /// <returns>Returns true if the file was saved OK, false otherwise</returns>
        private bool SaveReport()
        {
            try
            {
                string timestamp = DateTime.Now.ToLongTimeString();
                timestamp = timestamp.Replace(":", "");

                string path = Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\Temp\" +
                    @"\ACPLogAnalyzer-Report-" + timestamp + ".txt";

                ReportName = path;

                using (StreamWriter outfile = new StreamWriter(path))
                {
                    foreach (string line in m_reportText)
                        outfile.WriteLine(line);
                }

                return true;
            }
            catch
            {
            }

            return false;
        }
    }
}
