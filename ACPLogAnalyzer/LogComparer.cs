using System.Collections.Generic;

namespace ACPLogAnalyzer
{
    /// <summary>
    /// Custom comparision class to allow the sorting of Logs
    /// </summary>
    public class LogComparer : IComparer<Log>
    {
        public int Compare(Log x, Log y)
        {
            if (x == null)
            {
                if (y == null) return 0;  // If x is null and y is null, they're equal
                
                // If x is null and y is not null, y is greater
                return -1;
            }
            
            // If x is not null...
            if (y == null) return 1; // ...and y is null, x is greater
            
            // ...and y is not null, compare the start dates of logs
            // retval will be:  -1 if x is earlier than y
            //                  0  if x and y are the same date
            //                  1  if x is later than y
            if (x.StartDate != null && y.StartDate != null)
            {
                var retval = x.StartDate.Value.CompareTo(y.StartDate.Value);
                return retval;
            }

            return 0;
        }
    }

    /// <summary>
    /// Custom comparision class to allow the sorting of Logs (descending order)
    /// </summary>
    public class LogComparerDesc : IComparer<Log>
    {
        public int Compare(Log y, Log x)
        {
            if (x == null)
            {
                if (y == null) return 0;  // If x is null and y is null, they're equal

                // If x is null and y is not null, y is greater
                return -1;
            }

            // If x is not null...
            if (y == null) return 1; // ...and y is null, x is greater

            // ...and y is not null, compare the start dates of logs
            // retval will be:  -1 if x is earlier than y
            //                  0  if x and y are the same date
            //                  1  if x is later than y
            if (x.StartDate != null && y.StartDate != null)
            {
                var retval = x.StartDate.Value.CompareTo(y.StartDate.Value);
                return retval;
            }

            return 0;
        }
    }
}