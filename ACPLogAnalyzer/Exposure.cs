namespace ACPLogAnalyzer
{
    /// <summary>
    /// The Exposure class encapsulates information and about single image exposure
    /// </summary>
    public class Exposure
    {
        /// <summary>
        /// Exposure duration in seconds (not ticks)
        /// </summary>
        public double Duration { get; set; }

        /// <summary>
        /// Filter name
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// Exposure binning
        /// </summary>
        public int Bin { get; set; }

        public Exposure() : this(0.0, "", 0) { }
        public Exposure(double duration, string filter, int bin)
        {
            Duration = duration;
            Filter = filter;
            Bin = bin;
        }
    }
}
