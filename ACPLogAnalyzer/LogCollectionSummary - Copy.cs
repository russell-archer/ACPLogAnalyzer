using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACPLogAnalyzer
{
    /// <summary>
    /// This class holds summarized data (totals, etc.) about one or more logs
    /// </summary>
    public class LogCollectionSummary
    {
        private List<Log> m_logs;
        /// <summary>
        /// List of all logs to be summarized
        /// </summary>
        public List<Log> Logs
        {
            get { return m_logs; }
            set { m_logs = value; }
        }

        /// <summary>
        /// List of unique (by name) imaging targets in all logs
        /// </summary>
        public List<string> UniqueTargets
        {
            get
            {
                try
                {
                    var result =
                        from log in Logs                    // From all logs...
                        where log.LogTotalsAreConsistent
                        select log into logs                // ...select all the logs with good stats
                        from target in logs.Targets         // From all targets...
                        select target.Name;                 // ...select each target name value into a list

                    return result.Distinct<string>().ToList<string>();  // Distinct() removes duplicates
                }
                catch
                {
                }

                return null;
            }
        }

        /// <summary>
        /// Returns the total log runtime of all logs. The value returned is in seconds (not ticks)
        /// </summary>
        public double RunTime 
        { 
            get 
            {
                try
                {
                    var result =
                        (from log in Logs
                         where log.LogTotalsAreConsistent
                         select log.TotalLogRunTime).Sum();

                    return result;
                }
                catch
                {
                }

                return 0;
            } 
        }
        public TimeSpan RunTimeAsTimeSpan
        {
            get
            {
                try
                {
                    return TimeSpan.FromSeconds(RunTime);
                }
                catch
                {
                }

                return new TimeSpan(0);
            }
        }

        /// <summary>
        /// The toal number of imaging (not including pointing) exposures in all logs
        /// </summary>
        public double Exposures
        {
            get
            {
                try
                {
                    var result =
                        (from log in Logs
                         where log.LogTotalsAreConsistent
                         select log.TotalExposures).Sum();

                    return result;
                }
                catch
                {
                }

                return 0;
            }
        }

        /// <summary>
        /// Returns the total WAIT time (in secs) in all logs
        /// </summary>
        public double WaitTime
        {
            get
            {
                try
                {
                    var result =
                        (from log in Logs
                         where log.LogTotalsAreConsistent
                         select log.TotalWaitTime).Sum();

                    return result;
                }
                catch
                {
                }

                return 0;
            }
        }
        public TimeSpan WaitTimeAsTimeSpan
        {
            get
            {
                try
                {
                    return TimeSpan.FromSeconds(WaitTime);
                }
                catch
                {
                }

                return new TimeSpan(0);
            }
        }

        /// <summary>
        /// Returns the sum of all imaging exposure durations for all logs. The value returned is in seconds (not ticks)
        /// </summary>
        public double ImagingTime
        {
            get
            {
                try
                {
                    var result =
                        (from log in Logs
                         where log.LogTotalsAreConsistent
                         select log.TotalImagingTime).Sum();

                    return result;
                }
                catch
                {
                }

                return 0;
            }
        }
        public TimeSpan ImagingTimeAsTimeSpan
        {
            get
            {
                try
                {
                    return TimeSpan.FromSeconds(ImagingTime);
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
        public double OtherTime { get { return RunTime - (WaitTime + ImagingTime); } }
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
        /// Total runtime minus any waits for all logs
        /// </summary>
        public double RunTimeMinusWaits { get { return RunTime - WaitTime; } }
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
        public double ImagingTimeAsPercentageOfRunTime { get { return Math.Round((double)100 * (ImagingTime / RunTimeMinusWaits), 0); } }

        /// <summary>
        /// Average auto-focus time for all targets in all logs
        /// </summary>
        public double AverageAutoFocusTime
        {
            get
            {
                try
                {
                    var result =
                        from log in Logs                    // From all logs...
                        where log.LogTotalsAreConsistent    
                        select log into logs                // ...select all the logs with good stats
                        from target in logs.Targets         // From all targets...
                        where target.AvgAutoFocusTime != 0  // ...and there's valid data...
                        select target.AvgAutoFocusTime;     // ...select each AvgAutoFocusTime value into a list

                    return Math.Round(result.ToList<int>().Average(), 0);
                }
                catch
                {
                }

                return 0;
            }
        }

        /// <summary>
        /// Average FWHM for all targets in all logs
        /// </summary>
        public double AverageFWHM
        {
            get
            {
                try
                {
                    var result =
                        from log in Logs                    // From all logs...
                        where log.LogTotalsAreConsistent
                        select log into logs                // ...select all the logs with good stats
                        from target in logs.Targets         // From all targets...
                        where target.AvgFWHM != 0           // ...and there's valid data...
                        select target.AvgFWHM;              // ...select each AvgFWHM value into a list

                    return Math.Round(result.ToList<double>().Average(), 2);
                }
                catch
                {
                }

                return 0;
            }
        }

        /// <summary>
        /// Average HFD for all targets in all logs
        /// </summary>
        public double AverageHFD
        {
            get
            {
                try
                {
                    var result =
                        from log in Logs                    // From all logs...
                        where log.LogTotalsAreConsistent
                        select log into logs                // ...select all the logs with good stats
                        from target in logs.Targets         // From all targets...
                        where target.AvgHFD != 0            // ...and there's valid data...
                        select target.AvgHFD;               // ...select each AvgHFD value into a list

                    return Math.Round(result.ToList<double>().Average(), 2);
                }
                catch
                {
                }

                return 0;
            }
        }

        /// <summary>
        /// Average pointing error for object slews for all targets in all logs
        /// </summary>
        public double AveragePointingErrorObject
        {
            get
            {
                try
                {
                    var result =
                        from log in Logs                              // From all logs...
                        where log.LogTotalsAreConsistent
                        select log into logs                          // ...select all the logs with good stats
                        from target in logs.Targets                   // From all targets...
                        where target.AvgPointingErrorObjectSlew != 0  // ...and there's valid data...
                        select target.AvgPointingErrorObjectSlew;     // ...select each average value into a list

                    return Math.Round(result.ToList<double>().Average(), 2);
                }
                catch
                {
                }

                return 0;
            }
        }

        /// <summary>
        /// Average pointing error for center slews for all targets in all logs
        /// </summary>
        public double AveragePointingErrorCenter
        {
            get
            {
                try
                {
                    var result =
                        from log in Logs                              // From all logs...
                        where log.LogTotalsAreConsistent
                        select log into logs                          // ...select all the logs with good stats
                        from target in logs.Targets                   // From all targets...
                        where target.AvgPointingErrorCenterSlew != 0  // ...and there's valid data...
                        select target.AvgPointingErrorCenterSlew;     // ...select each value into a list

                    return Math.Round(result.ToList<double>().Average(), 2);
                }
                catch
                {
                }

                return 0;
            }
        }

        /// <summary>
        /// Average guider start-up time for all targets in all logs
        /// </summary>
        public double AverageGuiderStartUpTime
        {
            get
            {
                try
                {
                    var result =
                        from log in Logs                                    // From all logs...
                        where log.LogTotalsAreConsistent
                        select log into logs                                // ...select all the logs with good stats
                        from target in logs.Targets                         // From all targets...
                        where target.AvgGuiderStartUpTime.TotalSeconds != 0 // ...and there's valid data...
                        select target.AvgGuiderStartUpTime.TotalSeconds;    // ...select each value into a list

                    return Math.Round(result.ToList<double>().Average(), 0);
                }
                catch
                {
                }

                return 0;
            }
        }
        public TimeSpan AverageGuiderStartUpTimeAsTimeSpan
        {
            get
            {
                try
                {
                    return TimeSpan.FromSeconds(AverageGuiderStartUpTime);
                }
                catch
                {
                }

                return new TimeSpan(0);
            }
        }

        /// <summary>
        /// Average guider settle time for all targets in all logs
        /// </summary>
        public double AverageGuiderSettleTime
        {
            get
            {
                try
                {
                    var result =
                        from log in Logs                                    // From all logs...
                        where log.LogTotalsAreConsistent
                        select log into logs                                // ...select all the logs with good stats
                        from target in logs.Targets                         // From all targets...
                        where target.AvgGuiderSettleTime.TotalSeconds != 0  // ...and there's valid data...
                        select target.AvgGuiderSettleTime.TotalSeconds;     // ...select each value into a list

                    return Math.Round(result.ToList<double>().Average(), 0);
                }
                catch
                {
                }

                return 0;
            }
        }
        public TimeSpan AverageGuiderSettleTimeAsTimeSpan
        {
            get
            {
                try
                {
                    return TimeSpan.FromSeconds(AverageGuiderSettleTime);
                }
                catch
                {
                }

                return new TimeSpan(0);
            }
        }

        /// <summary>
        /// Average filter change time for all targets in all logs
        /// </summary>
        public double AverageFilterChangeTime
        {
            get
            {
                try
                {
                    var result =
                        from log in Logs                                    // From all logs...
                        where log.LogTotalsAreConsistent
                        select log into logs                                // ...select all the logs with good stats
                        from target in logs.Targets                         // From all targets...
                        where target.AvgFilterChangeTime.TotalSeconds != 0  // ...and there's valid data...
                        select target.AvgFilterChangeTime.TotalSeconds;     // ...select each value into a list

                    return Math.Round(result.ToList<double>().Average(), 0);
                }
                catch
                {
                }

                return 0;
            }
        }
        public TimeSpan AverageFilterChangeTimeAsTimeSpan
        {
            get
            {
                try
                {
                    return TimeSpan.FromSeconds(AverageFilterChangeTime);
                }
                catch
                {
                }

                return new TimeSpan(0);
            }
        }

        /// <summary>
        /// Average slew time for all targets in all logs
        /// </summary>
        public double AverageSlewTime
        {
            get
            {
                try
                {
                    var result =
                        from log in Logs                            // From all logs...
                        where log.LogTotalsAreConsistent
                        select log into logs                        // ...select all the logs with good stats
                        from target in logs.Targets                 // From all targets...
                        where target.AvgSlewTime.TotalSeconds != 0  // ...and there's valid data...
                        select target.AvgSlewTime.TotalSeconds;     // ...select each value into a list

                    return Math.Round(result.ToList<double>().Average(), 0);
                }
                catch
                {
                }

                return 0;
            }
        }
        public TimeSpan AverageSlewTimeAsTimeSpan
        {
            get
            {
                try
                {
                    return TimeSpan.FromSeconds(AverageSlewTime);
                }
                catch
                {
                }

                return new TimeSpan(0);
            }
        }

        /// <summary>
        /// Average pointing exposure/plate solve time for all targets in all logs
        /// </summary>
        public double AveragePlateSolveTime
        {
            get
            {
                try
                {
                    var result =
                        from log in Logs                                    // From all logs...
                        where log.LogTotalsAreConsistent
                        select log into logs                                // ...select all the logs with good stats
                        from target in logs.Targets                         // From all targets...
                        where target.AvgPlateSolveTime.TotalSeconds != 0    // ...and there's valid data...
                        select target.AvgPlateSolveTime.TotalSeconds;       // ...select each value into a list

                    return Math.Round(result.ToList<double>().Average(), 0);
                }
                catch
                {
                }

                return 0;
            }
        }
        public TimeSpan AveragePlateSolveTimeAsTimeSpan
        {
            get
            {
                try
                {
                    return TimeSpan.FromSeconds(AveragePlateSolveTime);
                }
                catch
                {
                }

                return new TimeSpan(0);
            }
        }

        /// <summary>
        /// Count of successful auto-focus runs for all targets in all logs
        /// </summary>
        public int SuccessfulAutoFocusCount
        {
            get
            {
                try
                {
                    var result =
                        from log in Logs                       // From all logs...
                        where log.LogTotalsAreConsistent
                        select log into logs                   // ...select all the logs with good stats
                        from target in logs.Targets            // From all targets...
                        where target.SuccessfulAutoFocus != 0  // ...and there's valid data...
                        select target.SuccessfulAutoFocus;     // ...select each value into a list

                    return result.ToList<int>().Sum();
                }
                catch
                {
                }

                return 0;
            }
        }

        /// <summary>
        /// Count of failed auto-focus runs for all targets in all logs
        /// </summary>
        public int UnsuccessfulAutoFocusCount
        {
            get
            {
                try
                {
                    var result =
                        from log in Logs                         // From all logs...
                        where log.LogTotalsAreConsistent
                        select log into logs                     // ...select all the logs with good stats
                        from target in logs.Targets              // From all targets...
                        where target.UnsuccessfulAutoFocus != 0  // ...and there's valid data...
                        select target.UnsuccessfulAutoFocus;     // ...select each value into a list

                    return result.ToList<int>().Sum();
                }
                catch
                {
                }

                return 0;
            }
        }

        /// <summary>
        /// Count of successful plate solves for all targets in all logs
        /// </summary>
        public int SuccessfulPlateSolveCount
        {
            get
            {
                try
                {
                    var result =
                        from log in Logs                         // From all logs...
                        where log.LogTotalsAreConsistent
                        select log into logs                     // ...select all the logs with good stats
                        from target in logs.Targets              // From all targets...
                        where target.SuccessfulPlateSolves != 0  // ...and there's valid data...
                        select target.SuccessfulPlateSolves;     // ...select each value into a list

                    return result.ToList<int>().Sum();
                }
                catch
                {
                }

                return 0;
            }
        }

        /// <summary>
        /// Count of failed plate solves for all targets in all logs
        /// </summary>
        public int UnsuccessfulPlateSolveCount
        {
            get
            {
                try
                {
                    var result =
                        from log in Logs                            // From all logs...
                        where log.LogTotalsAreConsistent
                        select log into logs                        // ...select all the logs with good stats
                        from target in logs.Targets                 // From all targets...
                        where target.UnsuccessfulPlateSolves != 0   // ...and there's valid data...
                        select target.UnsuccessfulPlateSolves;      // ...select each value into a list

                    return result.ToList<int>().Sum();
                }
                catch
                {
                }

                return 0;
            }
        }

        /// <summary>
        /// Count of guider failures where imaging continued unguided for all targets in all logs
        /// </summary>
        public int TotalGuiderFailureCount
        {
            get
            {
                try
                {
                    var result =
                        from log in Logs                    // From all logs...
                        where log.LogTotalsAreConsistent
                        select log into logs                // ...select all the logs with good stats
                        from target in logs.Targets         // From all targets...
                        where target.GuidingFailures != 0   // ...and there's valid data...
                        select target.GuidingFailures;      // ...select each value into a list

                    return result.ToList<int>().Sum();
                }
                catch
                {
                }

                return 0;
            }
        }

        /// <summary>
        /// Assess the consistency and reliability of log summary totals
        /// </summary>
        public bool TotalsAreConsistent
        {
            get
            {
                if (RunTime <= 0 ||
                    ImagingTime < 0 ||
                    WaitTime < 0 ||
                    OtherTime < 0 ||
                    ImagingTimeAsPercentageOfRunTime > 100 ||
                    RunTime < ImagingTime)

                    return false;  // Ignore the totals for this log 
                else
                    return true;   // Totals are OK
            }
        }

        public LogCollectionSummary()
        {
        }
    }
}
