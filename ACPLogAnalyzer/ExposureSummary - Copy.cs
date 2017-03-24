using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACPLogAnalyzer
{
    /// <summary>
    /// The ExposureSummary class aggregates info related to a collection of exposures of the same type (same filter, duration and bin)
    /// </summary>
    public class ExposureSummary
    {
        #region Private members and properties
        private Exposure m_exposure;
        /// <summary>
        /// Image exposure details (filter, bin, duration)
        /// </summary>
        public Exposure Exposure
        {
            get { return m_exposure; }
            set { m_exposure = value; }
        }

        private int m_count;
        /// <summary>
        /// The number of exposures
        /// </summary>
        public int Count
        {
            get { return m_count; }
            set { m_count = value; }
        }
        #endregion

        #region Construction
        public ExposureSummary() : this(0, null) { }
        public ExposureSummary(int count, Exposure exposure)
        {
            m_count = count;
            m_exposure = exposure;
        }
        #endregion
    }
}
