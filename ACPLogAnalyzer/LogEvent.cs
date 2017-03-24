using System;

namespace ACPLogAnalyzer
{
    /// <summary>
    /// This class encapsulates an event (action) in an ACP log. A Log object normally contains a generic List of LogEvent objects
    /// </summary>
    public class LogEvent
    {
        /// <summary>
        /// The physical path to the log file
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// The date/time when the event started
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// The date/time when the event ended
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// The type of log event this object encasulates
        /// </summary>
        public LogEventType EventType { get; set; }

        /// <summary>
        /// The line number in the og log file for the event
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// True if the event was successful, false otherwise
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// The name of the target (if any) associated with the event
        /// </summary>
        public Target Target { get; set; }

        /// <summary>
        /// Event-specific data
        /// </summary>
        public object Data { get; set; }

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
            StartDate = start;
            EndDate = end;
            EventType = type;
            LineNumber = lineNumber;
            Success = success;
            Target = target;
            Data = data;
            Path = path;
        }
    }
}
