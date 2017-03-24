using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;

namespace ACPLogAnalyzer
{
    /// <summary>
    /// The LogReader class reads a log file's text from disk
    /// </summary>
    class LogReader
    {
        #region Private member variables and properties
        private string m_path = null;  // Path/filename of log to be read
        
        private List<string> logFileText = null;  
        /// <summary>
        /// List of strings (one string per line) that make up the file
        /// </summary>
        public List<string> LogFileText
        {
            get { return logFileText; }
            set { logFileText = value; }
        }
        #endregion

        #region Construction
        private LogReader() : this("") {}
        public LogReader(string path)
		{
		    m_path = path;
		}
        #endregion

        #region Log reading methods
        public bool ReadLogFile(string path)
        {
            m_path = path;
            return ReadLogFile();
        }

        public bool ReadLogFile()
		{
			// Read log file
            logFileText = new List<string>();

			if(m_path == null || m_path.Length == 0)
				return false;

			string line;  // Holds the current line we're reading
			StreamReader sr = null;

			try
			{
				sr = new StreamReader(m_path);
				while(true)
				{
					line = sr.ReadLine();
					logFileText.Add(line);
					if(line == null)
						break;
				}
			}
			catch
			{
				return false; // Ignore the error
			}
			finally
			{
				if(sr != null)
					sr.Close();
			}

            return true;
        }
        #endregion
    }
}

