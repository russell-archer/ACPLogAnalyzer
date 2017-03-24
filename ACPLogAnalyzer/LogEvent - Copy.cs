using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACPLogAnalyzer
{
    /// <summary>
    /// This class encapsulates an event (action) in an ACP log. A Log object normally contains a generic List of LogEvent objects
    /// </summary>
    public class LogEvent
    {
        private string m_path = null;
        /// <summary>
        /// The physical path to the log file
        /// </summary>
        public string Path
        {
            get { return m_path; }
        }

        private DateTime? m_startDate;
        /// <summary>
        /// The date/time when the event started
        /// </summary>
        public DateTime? StartDate
        {
            get { return m_startDate; }
            set { m_startDate = value; }
        }

        private DateTime? m_endDate;
        /// <summary>
        /// The date/time when the event ended
        /// </summary>
        public DateTime? EndDate
        {
            get { return m_endDate; }
            set { m_endDate = value; }
        }

        private LogEventType m_eventType;
        /// <summary>
        /// The type of log event this object encasulates
        /// </summary>
        public LogEventType EventType
        {
            get { return m_eventType; }
            set { m_eventType = value; }
        }

        private int m_lineNumber;
        /// <summary>
        /// The line number in the og log file for the event
        /// </summary>
        public int LineNumber
        {
            get { return m_lineNumber; }
            set { m_lineNumber = value; }
        }

        private bool m_success;
        /// <summary>
        /// True if the event was successful, false otherwise
        /// </summary>
        public bool Success
        {
            get { return m_success; }
            set { m_success = value; }
        }

        private Target m_target;
        /// <summary>
        /// The name of the target (if any) associated with the event
        /// </summary>
        public Target Target
        {
            get { return m_target; }
            set { m_target = value; }
        }

        private object m_data;
        /// <summary>
        /// Event-specific data
        /// </summary>
        public object Data
        {
            get { return m_data; }
            set { m_data = value; }
        }

        #region Construction
        public LogEvent() : this(null, null, LogEventType.Unknown, -1, false, null, null, null) { }
        public LogEvent(
            DateTime? start,
            DateTime? end,
            LogEventType type,
            int lineNumber,
            bool success,
            Target target,
            object data,
            string path)
        {
            m_startDate = start;
            m_endDate = end;
            m_eventType = type;
            m_lineNumber = lineNumber;
            m_success = success;
            m_target = target;
            m_data = data;
            m_path = path;
        }
        #endregion
    }
}
