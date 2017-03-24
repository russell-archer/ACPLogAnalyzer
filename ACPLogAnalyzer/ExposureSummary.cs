namespace ACPLogAnalyzer
{
    /// <summary>
    /// The ExposureSummary class aggregates info related to a collection of exposures of the same type (same filter, duration and bin)
    /// </summary>
    public class ExposureSummary
    {
        /// <summary>
        /// Image exposure details (filter, bin, duration)
        /// </summary>
        public Exposure Exposure { get; set; }

        /// <summary>
        /// The number of exposures
        /// </summary>
        public int Count { get; set; }

        public ExposureSummary() : this(0, null) { }
        public ExposureSummary(int count, Exposure exposure)
        {
            Count = count;
            Exposure = exposure;
        }
    }
}
