using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ACPLogAnalyzer
{
    public class Report
    {
        private LogComparer _compareLogs;                           // Custom comparison
        private LogComparerDesc _compareLogsDesc;                   // Custom comparison
        private LogReader _logReader;                               // Object that deals with reading a log from disk
        private List<string> _reportText;                           // Holds the text of the report which will be written to disk
        private List<LogEvent> _allLogEvents;                       // Holds all events from all logs
        private LogCollectionSummary LogCollection { get; set; }    // Calculates overall total, averages, etc. from log data

        /// <summary>
        /// List of Logs (each Log encapsulates a physical ACP log file)
        /// </summary>
        public List<Log> Logs { get; set; }

        /// <summary>
        /// List of log filenames to report on
        /// </summary>
        public List<string> LogFilenames { get; set; }

        /// <summary>
        /// Flag used to stop parsing
        /// </summary>
        public bool ParsingLogs { get; set; }

        /// <summary>
        /// Filename for the report's text
        /// </summary>
        public string ReportName { get; set; }

        public Report() : this(null)
        {
        }

        public Report(List<string> logFiles)
        {
            ReportName = null;
            ParsingLogs = false;
            Logs = null;
            LogCollection = null;
            LogFilenames = logFiles;
        }

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
            Log log;
            Logs = new List<Log>();
            LogCollection = new LogCollectionSummary {Logs = Logs};
            _allLogEvents = new List<LogEvent>();
            _compareLogs = new LogComparer();
            _compareLogsDesc = new LogComparerDesc();

            if (!ParsingLogs)  // User stopped the process
                return false;

            if (string.IsNullOrEmpty(path))
            {
                // Parse all logs...
                foreach (var logPath in LogFilenames)
                {
                    if (!ParsingLogs)  // User stopped the process
                        return false;

                    try
                    {
                        log = ReadLogAndParse(logPath);  // Get the Log object to handle the parsing of its own file

                        if (log != null && !log.IsAcpLog)
                            continue;  // Not valid ACP log

                        Logs.Add(log);
                    }
                    catch
                    {
                    }
                }

                return true;
            }
            
            // Parse a single log...
            try
            {
                log = ReadLogAndParse(path);

                if (log != null && log.IsAcpLog)
                    Logs.Add(log);

                return true;
            }
            catch
            {
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
                _logReader = new LogReader(filename);
                if (!_logReader.ReadLogFile())
                    return null;

                // Create a new Log object and give it the text of its underlying log file
                log = new Log(filename, _logReader.LogFileText);

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
                        Logs.Sort(_compareLogs);
                    else
                        Logs.Sort(_compareLogsDesc);
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
            if (lg.LogEvents == null || lg.LogEvents.Count <= 0 || _allLogEvents == null) return;

            foreach (var le in lg.LogEvents)
                _allLogEvents.Add(le);  // Add the event to the overall list of events
        }

        /// <summary>
        /// Enumerate all log objects and output details to the report
        /// </summary>
        private bool OutputLogDetailReport()
        {
            _reportText = new List<string>();  // Holds the text of the report

            try
            {
                foreach (var lg in Logs)
                {
                    if (!ParsingLogs)  // User stopped the process
                        return false;

                    if (Properties.Settings.Default.Ignore_Logs_With_Zero_Targets)
                    {
                        if (lg.Targets.Count == 0)
                            continue;
                    }

                    // Output properties for the log as a whole...
                    if (Properties.Settings.Default.Show_Log_Name)
                        _reportText.Add("Log: " + System.IO.Path.GetFileName(lg.Path));

                    if (Properties.Settings.Default.Show_Start_Date)
                        _reportText.Add("Start date: " + lg.StartDate.ToString());

                    if (Properties.Settings.Default.Show_End_Date)
                        _reportText.Add("End date: " + lg.EndDate.ToString());

                    if (Properties.Settings.Default.Runtime_Breakdown)
                    {
                        _reportText.Add("Log runtime breakdown:");

                        _reportText.Add("  Runtime: " +
                            lg.TotalLogRunTimeAsTimeSpan.Days.ToString(CultureInfo.InvariantCulture) + "d " +
                            lg.TotalLogRunTimeAsTimeSpan.Hours.ToString(CultureInfo.InvariantCulture) + "h " +
                            lg.TotalLogRunTimeAsTimeSpan.Minutes.ToString(CultureInfo.InvariantCulture) + "m");

                        _reportText.Add("  Imaging time: " +
                            lg.TotalImagingTimeAsTimeSpan.Days.ToString(CultureInfo.InvariantCulture) + "d " +
                            lg.TotalImagingTimeAsTimeSpan.Hours.ToString(CultureInfo.InvariantCulture) + "h " +
                            lg.TotalImagingTimeAsTimeSpan.Minutes.ToString(CultureInfo.InvariantCulture) + "m");

                        _reportText.Add("  Wait time: " +
                            lg.TotalWaitTimeAsTimeSpan.Days.ToString(CultureInfo.InvariantCulture) + "d " +
                            lg.TotalWaitTimeAsTimeSpan.Hours.ToString(CultureInfo.InvariantCulture) + "h " +
                            lg.TotalWaitTimeAsTimeSpan.Minutes.ToString(CultureInfo.InvariantCulture) + "m");

                        _reportText.Add("  Observatory overhead: " +
                            lg.OtherTimeAsTimeSpan.Days.ToString(CultureInfo.InvariantCulture) + "d " +
                            lg.OtherTimeAsTimeSpan.Hours.ToString(CultureInfo.InvariantCulture) + "h " +
                            lg.OtherTimeAsTimeSpan.Minutes.ToString(CultureInfo.InvariantCulture) + "m");

                        _reportText.Add("  Imaging as % of runtime (ex waits): " +
                            lg.ImagingTimeAsPercentageOfRunTime.ToString(CultureInfo.InvariantCulture) + "%");
                        
                    }

                    if (Properties.Settings.Default.Show_Target_Details)
                        _reportText.Add("Targets: " + lg.NumberOfTargets.ToString(CultureInfo.InvariantCulture));

                    // Output details for each target in the log...
                    foreach (var t in lg.Targets)
                    {
                        if (Properties.Settings.Default.Ignore_Targets_With_Zero_Completed_Exposures && t.NumberOfExposures == 0)
                            continue;

                        if (Properties.Settings.Default.Show_Target_Details)
                        {
                            if (Properties.Settings.Default.Show_Target_Details && Properties.Settings.Default.Show_Target_Name)
                            {
                                _reportText.Add("");
                                _reportText.Add("  Target name: " + t.Name);
                            }

                            if (Properties.Settings.Default.Show_Target_Details && Properties.Settings.Default.Average_FWHM)
                            {
                                if (t.AvgFwhm > 0) _reportText.Add("  Average FWHM: " + t.AvgFwhm.ToString(CultureInfo.InvariantCulture) + " arcsecs");
                                else _reportText.Add("  Average FWHM: N/A");
                            }

                            if (Properties.Settings.Default.Show_Target_Details && Properties.Settings.Default.Average_HFD)
                            {
                                if (t.AvgHfd > 0) _reportText.Add("  Average HFD: " + t.AvgHfd.ToString(CultureInfo.InvariantCulture));
                                else _reportText.Add("  Average HFD: N/A");
                            }

                            if (Properties.Settings.Default.Average_Pointing_Error_Slew ||
                                Properties.Settings.Default.Average_Pointing_Error_Center)
                                _reportText.Add("  Average Pointing Error: ");

                            if (Properties.Settings.Default.Average_Pointing_Error_Slew)
                            {
                                if (t.AvgPointingErrorObjectSlew > 0) _reportText.Add("    Object slews: " + t.AvgPointingErrorObjectSlew.ToString(CultureInfo.InvariantCulture) + " arcmins");
                                else _reportText.Add("    Object slews: N/A");
                            }

                            if (Properties.Settings.Default.Average_Pointing_Error_Center)
                            {
                                if (t.AvgPointingErrorCenterSlew > 0) _reportText.Add("    Center slews: " + t.AvgPointingErrorCenterSlew.ToString(CultureInfo.InvariantCulture) + " arcmins");
                                else _reportText.Add("    Center slews: N/A");
                            }

                            if (Properties.Settings.Default.Average_Auto_Focus_Time)
                            {
                                if (t.AvgAutoFocusTime > 0)
                                    _reportText.Add("  Average auto-focus time: " + t.AvgAutoFocusTime.ToString(CultureInfo.InvariantCulture) + " secs");
                                else
                                    _reportText.Add("  Average auto-focus time: N/A");
                            }

                            if (Properties.Settings.Default.Average_Guider_StartUp_Time)
                            {
                                if (t.AvgGuiderStartUpTime.Ticks > 0) _reportText.Add("  Average guider start-up time: " + Math.Round(t.AvgGuiderStartUpTime.TotalSeconds, 0).ToString(CultureInfo.InvariantCulture) + " secs");
                                else _reportText.Add("  Average guider start-up time: N/A");
                            }

                            if (Properties.Settings.Default.Average_Guider_Settle_Time)
                            {
                                if (t.AvgGuiderSettleTime.Ticks > 0) _reportText.Add("  Average guider settle time: " + Math.Round(t.AvgGuiderSettleTime.TotalSeconds, 0).ToString(CultureInfo.InvariantCulture) + " secs");
                                else _reportText.Add("  Average guider settle time: N/A");
                            }

                            if (Properties.Settings.Default.Average_Filter_Change_Time)
                            {
                                if (t.AvgFilterChangeTime.Ticks > 0) _reportText.Add("  Average filter change time: " + Math.Round(t.AvgFilterChangeTime.TotalSeconds, 0).ToString(CultureInfo.InvariantCulture) + " secs");
                                else _reportText.Add("  Average filter change time: N/A");
                            }

                            if (Properties.Settings.Default.Average_Slew_Time_Targets)
                            {
                                if (t.AvgSlewTime.Ticks > 0) _reportText.Add("  Average slew time (targets): " + Math.Round(t.AvgSlewTime.TotalSeconds, 0).ToString(CultureInfo.InvariantCulture) + " secs");
                                else _reportText.Add("  Average slew time (targets): N/A");
                            }

                            if (Properties.Settings.Default.Average_Plate_Solve_Time)
                            {
                                if (t.AvgPlateSolveTime.Ticks > 0) _reportText.Add("  Average pointing exposure/plate solve time: " + Math.Round(t.AvgPlateSolveTime.TotalSeconds, 0).ToString(CultureInfo.InvariantCulture) + " secs");
                                else _reportText.Add("  Average pointing exposure/plate solve time: N/A");
                            }

                            if (Properties.Settings.Default.Show_AllSky_Time)
                            {
                                if (t.AvgPlateSolveTime.Ticks > 0) _reportText.Add("  Average all-sky plate solve time: " + Math.Round(t.AvgAllSkyPlateSolveTime.TotalSeconds, 0).ToString(CultureInfo.InvariantCulture) + " secs");
                                else _reportText.Add("  Average all-sky plate solve time: N/A");
                            }

                            if (Properties.Settings.Default.Script_Aborts) _reportText.Add("  Script aborts: " + t.ScriptAborts.ToString(CultureInfo.InvariantCulture));
                            if (Properties.Settings.Default.Script_Errors) _reportText.Add("  Script error: " + t.ScriptErrors.ToString(CultureInfo.InvariantCulture));
                            if (Properties.Settings.Default.Successful_AutoFocus) _reportText.Add("  Successful auto-focus runs: " + t.SuccessfulAutoFocus.ToString(CultureInfo.InvariantCulture));
                            if (Properties.Settings.Default.UnSuccessful_AutoFocus) _reportText.Add("  Unsuccessful auto-focus runs: " + t.UnsuccessfulAutoFocus.ToString(CultureInfo.InvariantCulture));
                            if (Properties.Settings.Default.Successful_Plate_Solves) _reportText.Add("  Successful plate solves: " + t.SuccessfulPlateSolves.ToString(CultureInfo.InvariantCulture));
                            if (Properties.Settings.Default.UnSuccessful_Plate_Solves) _reportText.Add("  Unsuccessful plate solves: " + t.UnsuccessfulPlateSolves.ToString(CultureInfo.InvariantCulture));
                            if (Properties.Settings.Default.Show_AllSky_Success_Count) _reportText.Add("  Successful all-sky plate solves: " + t.SuccessfulAllSkyPlateSolves.ToString(CultureInfo.InvariantCulture));
                            if (Properties.Settings.Default.Show_AllSky_Fail_Count) _reportText.Add("  Unsuccessful all-sky plate solves: " + t.UnsuccessfulAllSkyPlateSolves.ToString(CultureInfo.InvariantCulture));
                            if (Properties.Settings.Default.Show_Completed_Exposure_Details) _reportText.Add("  " + "Completed exposures: " + t.NumberOfExposures.ToString(CultureInfo.InvariantCulture));
                            if (Properties.Settings.Default.Show_Total_Guiding_Failures)
                            {
                                _reportText.Add("  " + "Guider failures: " + t.GuidingFailures.ToString(CultureInfo.InvariantCulture));
                                foreach (var le in lg.LogEvents)
                                {
                                    if (le.EventType != LogEventType.GuiderFail) continue;
                                    if (le.StartDate != null) _reportText.Add("    " + " Failed at " + le.StartDate.Value.ToLongTimeString());
                                }
                            }

                            if (Properties.Settings.Default.Show_Total_Imaging_Time_Per_Target && t.ExposureSummaryList != null && t.ExposureSummaryList.Count > 0)
                            {
                                _reportText.Add("  " + "Total imaging time on target: " +
                                    t.TotalTargetImagingTime.Hours.ToString(CultureInfo.InvariantCulture) + "h " +
                                    t.TotalTargetImagingTime.Minutes.ToString(CultureInfo.InvariantCulture) + "m " +
                                    t.TotalTargetImagingTime.Seconds.ToString(CultureInfo.InvariantCulture) + "s ");
                            }
                        }

                        if (t.ExposureSummaryList == null || t.ExposureSummaryList.Count == 0)
                            continue;

                        foreach (var summary in t.ExposureSummaryList)
                        {
                            if (Properties.Settings.Default.Show_Target_Details &&
                                Properties.Settings.Default.Show_Completed_Exposure_Details)
                            {
                                _reportText.Add(
                                    "    " +
                                    summary.Count.ToString(CultureInfo.InvariantCulture) +
                                    " x " +
                                    summary.Exposure.Duration.ToString(CultureInfo.InvariantCulture) +
                                    " secs " +
                                    summary.Exposure.Filter +
                                    " @ bin " +
                                    summary.Exposure.Bin.ToString(CultureInfo.InvariantCulture));
                            }
                        }
                    }

                    _reportText.Add("");
                    _reportText.Add("--------------------------------------------------------");
                    _reportText.Add("");
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
                // Output summary at bottom of report
                if (Properties.Settings.Default.Show_Report_Summary_At_Bottom)
                {
                    try
                    {
                        _reportText.Add("Report Summary");
                        _reportText.Add("");

                        if (Properties.Settings.Default.Show_Total_Logs_Parsed)
                            _reportText.Add("Total number of ACP logs parsed: " + Logs.Count.ToString(CultureInfo.InvariantCulture));

                        if (Properties.Settings.Default.Show_Total_Targets)
                            _reportText.Add("Total number of unique targets: " + LogCollection.UniqueTargets.Count.ToString(CultureInfo.InvariantCulture));

                        if (Properties.Settings.Default.Show_Total_Image_Count)
                            _reportText.Add("Total number of images taken: " + LogCollection.Exposures.ToString(CultureInfo.InvariantCulture));

                        if (Properties.Settings.Default.Runtime_Breakdown_Overall)
                        {
                            _reportText.Add("Overall runtime breakdown:");

                            if (!LogCollection.TotalsAreConsistent)
                                _reportText.Add("  Inconsistent results");
                            else
                            {
                                _reportText.Add("  Runtime: " +
                                    LogCollection.RunTimeAsTimeSpan.Days.ToString(CultureInfo.InvariantCulture) + "d " +
                                    LogCollection.RunTimeAsTimeSpan.Hours.ToString(CultureInfo.InvariantCulture) + "h " +
                                    LogCollection.RunTimeAsTimeSpan.Minutes.ToString(CultureInfo.InvariantCulture) + "m");

                                _reportText.Add("  Imaging time: " +
                                    LogCollection.ImagingTimeAsTimeSpan.Days.ToString(CultureInfo.InvariantCulture) + "d " +
                                    LogCollection.ImagingTimeAsTimeSpan.Hours.ToString(CultureInfo.InvariantCulture) + "h " +
                                    LogCollection.ImagingTimeAsTimeSpan.Minutes.ToString(CultureInfo.InvariantCulture) + "m");

                                _reportText.Add("  Wait time: " +
                                    LogCollection.WaitTimeAsTimeSpan.Days.ToString(CultureInfo.InvariantCulture) + "d " +
                                    LogCollection.WaitTimeAsTimeSpan.Hours.ToString(CultureInfo.InvariantCulture) + "h " +
                                    LogCollection.WaitTimeAsTimeSpan.Minutes.ToString(CultureInfo.InvariantCulture) + "m");

                                _reportText.Add("  Observatory overhead: " +
                                    LogCollection.OtherTimeAsTimeSpan.Days.ToString(CultureInfo.InvariantCulture) + "d " +
                                    LogCollection.OtherTimeAsTimeSpan.Hours.ToString(CultureInfo.InvariantCulture) + "h " +
                                    LogCollection.OtherTimeAsTimeSpan.Minutes.ToString(CultureInfo.InvariantCulture) + "m");

                                _reportText.Add("  Imaging as % of runtime (ex waits): " +
                                    LogCollection.ImagingTimeAsPercentageOfRunTime.ToString(CultureInfo.InvariantCulture) + "%");
                            }
                        }

                        if (Properties.Settings.Default.Average_FWHM_Overall)
                        {
                            if (LogCollection.AverageFwhm == 0) _reportText.Add("Average FWHM: N/A");
                            else _reportText.Add("Average FWHM: " + LogCollection.AverageFwhm.ToString(CultureInfo.InvariantCulture) + " arcsecs");
                        }

                        if (Properties.Settings.Default.Average_HFD_Overall)
                        {
                            if (LogCollection.AverageHfd == 0) _reportText.Add("Average HFD: N/A");
                            else _reportText.Add("Average HFD: " + LogCollection.AverageHfd.ToString(CultureInfo.InvariantCulture));
                        }

                        if (Properties.Settings.Default.Average_Plate_Solve_Time_Overall)
                        {
                            if (LogCollection.AveragePlateSolveTime == 0) _reportText.Add("Average pointing exposure/plate solve time: N/A");
                            else _reportText.Add("Average pointing exposure/plate solve time: " + LogCollection.AveragePlateSolveTimeAsTimeSpan.TotalSeconds.ToString(CultureInfo.InvariantCulture) + " secs");
                        }

                        if (Properties.Settings.Default.Show_AllSky_Overall_Time)
                        {
                            if (LogCollection.AverageAllSkyPlateSolveTime == 0) _reportText.Add("Average all-sky plate solve time: N/A");
                            else _reportText.Add("Average all-sky plate solve time: " + LogCollection.AverageAllSkyPlateSolveTimeAsTimeSpan.TotalSeconds.ToString(CultureInfo.InvariantCulture) + " secs");
                        }

                        if (Properties.Settings.Default.Average_Slew_Time_Targets_Overall)
                        {
                            if (LogCollection.AverageSlewTime == 0) _reportText.Add("Average slew time (targets): N/A");
                            else _reportText.Add("Average slew time (targets): " + LogCollection.AverageSlewTimeAsTimeSpan.TotalSeconds.ToString(CultureInfo.InvariantCulture) + " secs");
                        }

                        if (Properties.Settings.Default.Average_Filter_Change_Time_Overall)
                        {
                            if (LogCollection.AverageFilterChangeTime == 0) _reportText.Add("Average filter change time: N/A");
                            else _reportText.Add("Average filter change time: " + LogCollection.AverageFilterChangeTimeAsTimeSpan.TotalSeconds.ToString(CultureInfo.InvariantCulture) + " secs");
                        }

                        if (Properties.Settings.Default.Average_Guider_Settle_Time_Overall)
                        {
                            if (LogCollection.AverageGuiderSettleTime == 0) _reportText.Add("Average guider settle time: N/A");
                            else _reportText.Add("Average guider settle time: " + LogCollection.AverageGuiderSettleTimeAsTimeSpan.TotalSeconds.ToString(CultureInfo.InvariantCulture) + " secs");
                        }

                        if (Properties.Settings.Default.Average_Guider_StartUp_Time_Overall)
                        {
                            if (LogCollection.AverageGuiderStartUpTime == 0) _reportText.Add("Average guider start-up time: N/A");
                            else _reportText.Add("Average guider start-up time: " + LogCollection.AverageGuiderStartUpTimeAsTimeSpan.TotalSeconds.ToString(CultureInfo.InvariantCulture) + " secs");
                        }

                        if (Properties.Settings.Default.Average_Pointing_Error_Slew_Overall)
                        {
                            if (LogCollection.AveragePointingErrorObject == 0) _reportText.Add("Average pointing error (object slew): N/A");
                            else _reportText.Add("Average pointing error (object slew): " + LogCollection.AveragePointingErrorObject.ToString(CultureInfo.InvariantCulture) + " arcsecs");
                        }

                        if (Properties.Settings.Default.Average_Pointing_Error_Center_Overall)
                        {
                            if (LogCollection.AveragePointingErrorCenter == 0) _reportText.Add("Average pointing error (center slew): N/A");
                            else _reportText.Add("Average pointing error (center slew): " + LogCollection.AveragePointingErrorCenter.ToString(CultureInfo.InvariantCulture) + " arcsecs");
                        }

                        if (Properties.Settings.Default.Average_Auto_Focus_Time_Overall)
                        {
                            if (LogCollection.AverageAutoFocusTime == 0) _reportText.Add("Average auto-focus time: N/A");
                            else _reportText.Add("Average auto-focus time: " + LogCollection.AverageAutoFocusTime.ToString(CultureInfo.InvariantCulture) + " secs");
                        }

                        if (Properties.Settings.Default.Successful_AutoFocus_Overall)
                            _reportText.Add("Total successful auto-focus runs: " + LogCollection.SuccessfulAutoFocusCount.ToString(CultureInfo.InvariantCulture));

                        if (Properties.Settings.Default.UnSuccessful_AutoFocus_Overall)
                            _reportText.Add("Total unsuccessful auto-focus runs: " + LogCollection.UnsuccessfulAutoFocusCount.ToString(CultureInfo.InvariantCulture));

                        if (Properties.Settings.Default.Successful_Plate_Solves_Overall)
                            _reportText.Add("Total successful plate solves: " + LogCollection.SuccessfulPlateSolveCount.ToString(CultureInfo.InvariantCulture));

                        if (Properties.Settings.Default.UnSuccessful_Plate_Solves_Overall)
                            _reportText.Add("Total unsuccessful plate solves: " + LogCollection.UnsuccessfulPlateSolveCount.ToString(CultureInfo.InvariantCulture));

                        if (Properties.Settings.Default.Show_AllSky_Success_Count_Overall)
                            _reportText.Add("Total successful all-sky plate solves: " + LogCollection.SuccessfulAllSkyPlateSolveCount.ToString(CultureInfo.InvariantCulture));

                        if (Properties.Settings.Default.Show_AllSky_Fail_Count_Overall)
                            _reportText.Add("Total unsuccessful all-sky plate solves: " + LogCollection.UnsuccessfulAllSkyPlateSolveCount.ToString(CultureInfo.InvariantCulture));

                        if (Properties.Settings.Default.Show_Total_Guiding_Failures_Overall)
                        {
                            _reportText.Add("  Total guider failures: " + LogCollection.TotalGuiderFailureCount.ToString(CultureInfo.InvariantCulture));
                            foreach (var le in _allLogEvents)
                            {
                                if (le.EventType != LogEventType.GuiderFail) continue;
                                if (le.StartDate != null)
                                    _reportText.Add("    " + 
                                        System.IO.Path.GetFileName(le.Path) +
                                        " (" +
                                        le.Target.Name.Trim() +
                                        ") at " +
                                        le.StartDate.Value.ToLongTimeString());
                            }
                        }

                        return true;
                    }
                    catch
                    {
                    }
                }

                // Output summary at top of report
                else
                {
                    try
                    {
                        _reportText.Insert(0, "");
                        _reportText.Insert(0, "--------------------------------------------------------");
                        _reportText.Insert(0, "");

                        if (Properties.Settings.Default.Show_Total_Guiding_Failures_Overall)
                        {
                            foreach (var le in _allLogEvents)
                            {
                                if (le.EventType != LogEventType.GuiderFail) continue;
                                if (le.StartDate != null)
                                    _reportText.Insert(0, "    " +
                                        System.IO.Path.GetFileName(le.Path) +
                                        " (" +
                                        le.Target.Name.Trim() +
                                        ") at " +
                                        le.StartDate.Value.ToLongTimeString());
                            }
                            _reportText.Insert(0, "  Total guider failures: " + LogCollection.TotalGuiderFailureCount.ToString(CultureInfo.InvariantCulture));
                        }

                        if (Properties.Settings.Default.Show_AllSky_Fail_Count_Overall)
                            _reportText.Insert(0, "Total unsuccessful all-sky plate solves: " + LogCollection.UnsuccessfulAllSkyPlateSolveCount.ToString(CultureInfo.InvariantCulture));

                        if (Properties.Settings.Default.Show_AllSky_Success_Count_Overall)
                            _reportText.Insert(0, "Total successful all-sky plate solves: " + LogCollection.SuccessfulAllSkyPlateSolveCount.ToString(CultureInfo.InvariantCulture));

                        if (Properties.Settings.Default.UnSuccessful_Plate_Solves_Overall)
                            _reportText.Insert(0, "Total unsuccessful plate solves: " + LogCollection.UnsuccessfulPlateSolveCount.ToString(CultureInfo.InvariantCulture));

                        if (Properties.Settings.Default.Successful_Plate_Solves_Overall)
                            _reportText.Insert(0, "Total successful plate solves: " + LogCollection.SuccessfulPlateSolveCount.ToString(CultureInfo.InvariantCulture));

                        if (Properties.Settings.Default.UnSuccessful_AutoFocus_Overall)
                            _reportText.Insert(0, "Total unsuccessful auto-focus runs: " + LogCollection.UnsuccessfulAutoFocusCount.ToString(CultureInfo.InvariantCulture));

                        if (Properties.Settings.Default.Successful_AutoFocus_Overall)
                            _reportText.Insert(0, "Total successful auto-focus runs: " + LogCollection.SuccessfulAutoFocusCount.ToString(CultureInfo.InvariantCulture));

                        if (Properties.Settings.Default.Show_AllSky_Overall_Time)
                        {
                            if (LogCollection.AverageAllSkyPlateSolveTime == 0) _reportText.Insert(0, "Average all-sky plate solve time: N/A");
                            else _reportText.Insert(0, "Average all-sky plate solve time: " + LogCollection.AverageAllSkyPlateSolveTimeAsTimeSpan.TotalSeconds.ToString(CultureInfo.InvariantCulture) + " secs");
                        }

                        if (Properties.Settings.Default.Average_Plate_Solve_Time_Overall)
                        {
                            if (LogCollection.AveragePlateSolveTime == 0) _reportText.Insert(0, "Average pointing exposure/plate solve time: N/A");
                            else _reportText.Insert(0, "Average pointing exposure/plate solve time: " + LogCollection.AveragePlateSolveTimeAsTimeSpan.TotalSeconds.ToString(CultureInfo.InvariantCulture) + " secs");
                        }

                        if (Properties.Settings.Default.Average_Slew_Time_Targets_Overall)
                        {
                            if (LogCollection.AverageSlewTime == 0) _reportText.Insert(0, "Average slew time (targets): N/A");
                            else _reportText.Insert(0, "Average slew time (targets): " + LogCollection.AverageSlewTimeAsTimeSpan.TotalSeconds.ToString(CultureInfo.InvariantCulture) + " secs");
                        }

                        if (Properties.Settings.Default.Average_Filter_Change_Time_Overall)
                        {
                            if (LogCollection.AverageFilterChangeTime == 0) _reportText.Insert(0, "Average filter change time: N/A");
                            else _reportText.Insert(0, "Average filter change time: " + LogCollection.AverageFilterChangeTimeAsTimeSpan.TotalSeconds.ToString(CultureInfo.InvariantCulture) + " secs");
                        }

                        if (Properties.Settings.Default.Average_Guider_Settle_Time_Overall)
                        {
                            if (LogCollection.AverageGuiderSettleTime == 0) _reportText.Insert(0, "Average guider settle time: N/A");
                            else _reportText.Insert(0, "Average guider settle time: " + LogCollection.AverageGuiderSettleTimeAsTimeSpan.TotalSeconds.ToString(CultureInfo.InvariantCulture) + " secs");
                        }

                        if (Properties.Settings.Default.Average_Guider_StartUp_Time_Overall)
                        {
                            if (LogCollection.AverageGuiderStartUpTime == 0) _reportText.Insert(0, "Average guider start-up time: N/A");
                            else _reportText.Insert(0, "Average guider start-up time: " + LogCollection.AverageGuiderStartUpTimeAsTimeSpan.TotalSeconds.ToString(CultureInfo.InvariantCulture) + " secs");
                        }

                        if (Properties.Settings.Default.Average_Auto_Focus_Time_Overall)
                        {
                            if (LogCollection.AverageAutoFocusTime == 0) _reportText.Insert(0, "Average auto-focus time: N/A");
                            else _reportText.Insert(0, "Average auto-focus time: " + LogCollection.AverageAutoFocusTime.ToString(CultureInfo.InvariantCulture) + " secs");
                        }

                        if (Properties.Settings.Default.Average_Pointing_Error_Center_Overall)
                        {
                            if (LogCollection.AveragePointingErrorCenter == 0) _reportText.Insert(0, "Average pointing error (center slew): N/A");
                            else _reportText.Insert(0, "Average pointing error (center slew): " + LogCollection.AveragePointingErrorCenter.ToString(CultureInfo.InvariantCulture) + " arcsecs");
                        }

                        if (Properties.Settings.Default.Average_Pointing_Error_Slew_Overall)
                        {
                            if (LogCollection.AveragePointingErrorObject == 0) _reportText.Insert(0, "Average pointing error (object slew): N/A");
                            else _reportText.Insert(0, "Average pointing error (object slew): " + LogCollection.AveragePointingErrorObject.ToString(CultureInfo.InvariantCulture) + " arcsecs");
                        }

                        if (Properties.Settings.Default.Average_HFD_Overall)
                        {
                            if (LogCollection.AverageHfd == 0) _reportText.Insert(0, "Average HFD: N/A");
                            else _reportText.Insert(0, "Average HFD: " + LogCollection.AverageHfd.ToString(CultureInfo.InvariantCulture));
                        }

                        if (Properties.Settings.Default.Average_FWHM_Overall)
                        {
                            if (LogCollection.AverageFwhm == 0) _reportText.Insert(0, "Average FWHM: N/A");
                            else _reportText.Insert(0, "Average FWHM: " + LogCollection.AverageFwhm.ToString(CultureInfo.InvariantCulture) + " arcsecs");
                        }

                        if (Properties.Settings.Default.Runtime_Breakdown_Overall)
                        {
                            if (!LogCollection.TotalsAreConsistent)
                                _reportText.Insert(0, "  Inconsistent results");
                            else
                            {
                                _reportText.Insert(0, "  Imaging as % of runtime (ex waits): " +
                                    LogCollection.ImagingTimeAsPercentageOfRunTime.ToString(CultureInfo.InvariantCulture) + "%");

                                _reportText.Insert(0, "  Observatory overhead: " +
                                    LogCollection.OtherTimeAsTimeSpan.Days.ToString(CultureInfo.InvariantCulture) + "d " +
                                    LogCollection.OtherTimeAsTimeSpan.Hours.ToString(CultureInfo.InvariantCulture) + "h " +
                                    LogCollection.OtherTimeAsTimeSpan.Minutes.ToString(CultureInfo.InvariantCulture) + "m");

                                _reportText.Insert(0, "  Wait time: " +
                                    LogCollection.WaitTimeAsTimeSpan.Days.ToString(CultureInfo.InvariantCulture) + "d " +
                                    LogCollection.WaitTimeAsTimeSpan.Hours.ToString(CultureInfo.InvariantCulture) + "h " +
                                    LogCollection.WaitTimeAsTimeSpan.Minutes.ToString(CultureInfo.InvariantCulture) + "m");

                                _reportText.Insert(0, "  Imaging time: " +
                                    LogCollection.ImagingTimeAsTimeSpan.Days.ToString(CultureInfo.InvariantCulture) + "d " +
                                    LogCollection.ImagingTimeAsTimeSpan.Hours.ToString(CultureInfo.InvariantCulture) + "h " +
                                    LogCollection.ImagingTimeAsTimeSpan.Minutes.ToString(CultureInfo.InvariantCulture) + "m");

                                _reportText.Insert(0, "  Runtime: " +
                                    LogCollection.RunTimeAsTimeSpan.Days.ToString(CultureInfo.InvariantCulture) + "d " +
                                    LogCollection.RunTimeAsTimeSpan.Hours.ToString(CultureInfo.InvariantCulture) + "h " +
                                    LogCollection.RunTimeAsTimeSpan.Minutes.ToString(CultureInfo.InvariantCulture) + "m");
                            }

                            _reportText.Insert(0, "Overall runtime breakdown:");
                        }

                        if (Properties.Settings.Default.Show_Total_Image_Count)
                            _reportText.Insert(0, "Total number of images taken: " + LogCollection.Exposures.ToString(CultureInfo.InvariantCulture));

                        if (Properties.Settings.Default.Show_Total_Targets)
                            _reportText.Insert(0, "Total number of unique targets: " + LogCollection.UniqueTargets.Count.ToString(CultureInfo.InvariantCulture));

                        if (Properties.Settings.Default.Show_Total_Logs_Parsed)
                            _reportText.Insert(0, "Total number of ACP logs parsed: " + Logs.Count.ToString(CultureInfo.InvariantCulture));

                        _reportText.Insert(0, "");
                        _reportText.Insert(0, "Report Summary");

                        return true;
                    }
                    catch 
                    { 
                    }
                }
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
                var timestamp = DateTime.Now.ToLongTimeString();
                timestamp = timestamp.Replace(":", "");

                var path = Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\Temp\" +
                    @"\ACPLogAnalyzer-Report-" + timestamp + ".txt";

                ReportName = path;

                using (var outfile = new StreamWriter(path))
                {
                    foreach (var line in _reportText)
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
