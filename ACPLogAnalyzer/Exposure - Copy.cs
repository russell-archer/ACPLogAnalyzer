using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACPLogAnalyzer
{
    /// <summary>
    /// The Exposure class encapsulates information and about single image exposure
    /// </summary>
    public class Exposure
    {
        #region Private members and properties
        private double m_duration;
        /// <summary>
        /// Exposure duration in seconds (not ticks)
        /// </summary>
        public double Duration
        {
            get { return m_duration; }
            set { m_duration = value; }
        }

        private string m_filter;
        /// <summary>
        /// Filter name
        /// </summary>
        public string Filter
        {
            get { return m_filter; }
            set { m_filter = value; }
        }

        private int m_bin;
        /// <summary>
        /// Exposure binning
        /// </summary>
        public int Bin
        {
            get { return m_bin; }
            set { m_bin = value; }
        }
        #endregion

        #region Construction
        public Exposure() : this(0.0, "", 0) { }
        public Exposure(double duration, string filter, int bin)
        {
            m_duration = duration;
            m_filter = filter;
            m_bin = bin;
        }
        #endregion
    }
}
