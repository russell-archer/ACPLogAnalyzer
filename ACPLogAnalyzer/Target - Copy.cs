using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ACPLogAnalyzer
{
    /// <summary>
    /// The Target class encapsulates information about a particular ACP target (e.g. summary info and a list of exposures)
    /// </summary>
    public class Target
    {
        #region Private members and properties
        private string m_name;
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        private Log m_log;
        public Log Log
        {
            get { return m_log; }
            set { m_log = value; }
        }

        public List<Exposure> Exposures
        {
            get { return GetTargetList<Exposure>(LogEventType.Exposure); }
        }
        public List<ExposureSummary> ExposureSummaryList
        {
            // Here we create a list that summarizes all the exposures for this log. For example:
            // 2, (300, "Luminance", 1)    (e.g. this target had 2 x 300secs luminance @ bin 1)
            // 5, (300, "Red", 2)          (e.g. 5 x 300secs red @ bin 2)
            // 3, (100, "Green", 1)        (e.g. 3 x 100secs green @ bin 1)

            get
            {
                if (Exposures == null || Exposures.Count == 0)
                    return null;

                var exposureSummaryList = new List<ExposureSummary>();

                try
                {
                    var query =
                    from exposure in Exposures
                    group exposure by exposure.Filter into filterGroup
                    select new
                    {
                        Filter = from fg in filterGroup
                                 group fg.Duration by fg.Bin into binGroup
                                 select new
                                 {
                                     Bin = from bg in binGroup
                                           group bg by bg into duration
                                           select new
                                           {
                                               Filter = filterGroup.Key,
                                               Bin = binGroup.Key,
                                               Duration = duration.Key,
                                               Count = duration.Count()
                                           }
                                 }
                    };

                    foreach (var groupByFilter in query)
                    {
                        foreach (var groupByBin in groupByFilter.Filter)
                        {
                            foreach (var groupByDuration in groupByBin.Bin)
                            {
                                // Create a new summary of this type
                                exposureSummaryList.Add(
                                    new ExposureSummary(groupByDuration.Count,
                                        new Exposure(
                                            groupByDuration.Duration,
                                            groupByDuration.Filter.ToString(),
                                            groupByDuration.Bin)));
                            }
                        }
                    }
                }
                catch
                {
                    return null;
                }

                return exposureSummaryList;
            }
        }
        public List<double> FWHM
        {
            get { return GetTargetList<double>(LogEventType.FWHM); }
        }
        public List<double> HFD
        {
            get { return GetTargetList<double>(LogEventType.HFD); }
        }
        public List<double> AutoFocusTime
        {
            get { return GetTargetList<double>(LogEventType.AutoFocus); }
        }
        public List<double> PointingErrorsObjectSlew
        {
            get { return GetTargetList<double>(LogEventType.PointingErrorObjectSlew); }
        }
        public List<double> PointingErrorsCenterSlew
        {
            get { return GetTargetList<double>(LogEventType.PointingErrorCenterSlew); }
        }
        public List<double> SlewTime
        {
            get { return GetTargetList<double>(LogEventType.SlewTarget); }
        }
        public List<double> GuiderStartUpTime
        {
            get { return GetTargetList<double>(LogEventType.GuiderStartUp); }
        }
        public List<double> GuiderSettleTime
        {
            get { return GetTargetList<double>(LogEventType.GuiderSettle); }
        }
        public List<double> FilterChangeTime
        {
            get { return GetTargetList<double>(LogEventType.FilterChange); }
        }
        public List<double> PointingExpAndPlateSolveTime
        {
            get { return GetTargetList<double>(LogEventType.PointingExpAndPlateSolve); }
        }
        public int GuidingFailures
        {
            get { return GetTargetCount(LogEventType.GuiderFail); }
        }
        public int SuccessfulPlateSolves
        {
            get { return GetTargetCount(LogEventType.PlateSolveSuccess); }
        }
        public int UnsuccessfulPlateSolves
        {
            get { return GetTargetCount(LogEventType.PlateSolveFail); }
        }
        public int SuccessfulAutoFocus
        {
            get { return GetTargetCount(LogEventType.AutoFocusSuccess); }
        }
        public int UnsuccessfulAutoFocus
        {
            get { return GetTargetCount(LogEventType.AutoFocusFail); }
        }
        public int ScriptErrors
        {
            get { return GetTargetCount(LogEventType.ScriptError); }
        }
        public int ScriptAborts
        {
            get { return GetTargetCount(LogEventType.ScriptAbort); }
        }
        public int NumberOfExposures
        {
            get
            {
                if (Exposures != null)
                    return Exposures.Count;
                else
                    return 0;
            }
        }
        public double AvgFWHM
        {
            get
            {
                if (FWHM == null || FWHM.Count == 0)
                    return 0;

                return Math.Round(FWHM.Average(), 2);
            }
        }
        public double AvgHFD
        {
            get
            {
                if (HFD == null || HFD.Count == 0)
                    return 0;

                return Math.Round(HFD.Average(), 2);
            }
        }
        public int AvgAutoFocusTime
        {
            get
            {
                if (AutoFocusTime == null || AutoFocusTime.Count == 0)
                    return 0;

                return (int)AutoFocusTime.Average();
            }
        }
        public double AvgPointingErrorObjectSlew
        {
            get
            {
                if (PointingErrorsObjectSlew == null || PointingErrorsObjectSlew.Count == 0)
                    return 0;

                return Math.Round(PointingErrorsObjectSlew.Average(), 2);
            }
        }
        public double AvgPointingErrorCenterSlew
        {
            get
            {
                if (PointingErrorsCenterSlew == null || PointingErrorsCenterSlew.Count == 0)
                    return 0;

                return Math.Round(PointingErrorsCenterSlew.Average(), 2);
            }
        }
        public TimeSpan AvgSlewTime
        {
            get
            {
                if (SlewTime == null || SlewTime.Count == 0)
                    return new TimeSpan(0);

                return new TimeSpan((long)SlewTime.Average(time => time * TimeSpan.TicksPerSecond));
            }
        }
        public TimeSpan AvgGuiderStartUpTime
        {
            get
            {
                if (GuiderStartUpTime == null || GuiderStartUpTime.Count == 0)
                    return new TimeSpan(0);

                return new TimeSpan((long)GuiderStartUpTime.Average(time => time * TimeSpan.TicksPerSecond));
            }
        }
        public TimeSpan AvgGuiderSettleTime
        {
            get
            {
                if (GuiderSettleTime == null || GuiderSettleTime.Count == 0)
                    return new TimeSpan(0);

                return new TimeSpan((long)GuiderSettleTime.Average(time => time * TimeSpan.TicksPerSecond));
            }
        }
        public TimeSpan AvgFilterChangeTime
        {
            get
            {
                if (FilterChangeTime == null || FilterChangeTime.Count == 0)
                    return new TimeSpan(0);

                return new TimeSpan((long)FilterChangeTime.Average(time => time * TimeSpan.TicksPerSecond));
            }
        }
        public TimeSpan AvgPlateSolveTime
        {
            get
            {
                if (PointingExpAndPlateSolveTime == null || PointingExpAndPlateSolveTime.Count == 0)
                    return new TimeSpan(0);

                return new TimeSpan((long)PointingExpAndPlateSolveTime.Average(time => time * TimeSpan.TicksPerSecond));
            }
        }
        #endregion

        #region Construction
        public Target() : this("", null) { }
        public Target(string name, Log log)
        {
            m_name = name;
            m_log = log;
        }
        #endregion

        #region Support methods
        /// <summary>
        /// Generic method that returns an event list for the specified event type
        /// </summary>
        /// <typeparam name="T">The type of data stored in the event's Data property (e.g. the list type required)</typeparam>
        /// <param name="logEventType">The event type</param>
        /// <returns>Returns an event list for the specified event type, or null if an exception occurs</returns>
        public List<T> GetTargetList<T>(LogEventType logEventType)
        {
            List<T> list = new List<T>();
            try
            {
                var listQuery =
                    from logEvent in Log.LogEvents
                    where (logEvent != null &&
                            logEvent.Data != null &&
                            logEvent.Target == this &&
                            logEvent.EventType == logEventType)
                    select (T)logEvent.Data;

                return listQuery.ToList<T>();
            }
            catch
            {
            }

            return null;
        }

        /// <summary>
        /// Generic method that returns a count for the specified event type
        /// </summary>
        /// <param name="logEventType">The event type</param>
        /// <returns>Returns a count for the specified event type, or 0 if an exception occurs</returns>
        public int GetTargetCount(LogEventType logEventType)
        {
            try
            {
                var countQuery =
                    from logEvent in Log.LogEvents
                    where (logEvent != null &&
                            logEvent.Target == this &&
                            logEvent.Data != null &&
                            logEvent.EventType == logEventType)
                    select logEvent;

                return countQuery.Count();
            }
            catch
            {
            }

            return 0;
        }
        #endregion
    }
}
