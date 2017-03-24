using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ACPLogAnalyzer
{
    public partial class Log
    {
        #region ParseLog()
        /// <summary>
        /// Enumerate the contents of a log and parse various properties, stats, timings, etc.
        /// </summary>
        /// <returns>Returns true if the log parsed OK, false otherwise</returns>
        public bool ParseLog()
        {
            CurrentTarget = null;
            m_targets = new List<Target>();
            m_isACPLog = false;     
            
            // Is this an ACP log?
            if (!ParseValidACPLog())
            {
                InitVars();
                return false;  // Can't parse, it's not an ACP log
            }

            // Start parsing the log until we reach the EOF or the ending statement
            m_logLineIndex = -1;
            foreach (string line in this.LogFileText)
            {
                m_logLineIndex++;
                if (line == null || line.Length == 0) 
                    continue;
                
                m_lineLower = line.ToLower();

                    ParseAutoFocusTime();                          // Auto-focus time (intentional fall-through)
                    ParseAutoFocusCount();                         // Auto-focus count (intentional fall-through)
                    ParseFWHM();                                   // FWHM value (intentional fall-through)
                    ParsePointingError();                          // Pointing error (object slew) (intentional fall-through)
                if (ParseEndOfLog())                    break;     // End of log
                if (ParseComment())                     continue;  // Comment line
                if (ParseLogPreamble())                 continue;  // Start of log preamble
                if (ParseImagingTargetStart())          continue;  // Start of new imaging target
                if (ParseImagingExposure())             continue;  // New imaging exposure
                if (ParseFWHM())                        continue;  // FWHM value
                if (ParseSlewToTargetTime())            continue;  // Slew to target time
                if (ParsePointingErrorCenterSlew())     continue;  // Pointing error (center slew)
                if (ParsePlateSolveCount())             continue;  // Plate solve count
                if (ParsePlateSolveErrorCount())        continue;  // Plate solve error count
                if (ParseHFD())                         continue;  // HFD value
                if (ParseAutoFocusErrorCount())         continue;  // Auto-focus error count
                if (ParseScriptError())                 continue;  // Script error count
                if (ParseScriptAbort())                 continue;  // Script abort count
                if (ParseGuiderStartUpTime())           continue;  // Guider start-up time
                if (ParseGuiderSettleTime())            continue;  // Guider settle time
                if (ParseFilterChangeTime())            continue;  // Filter change time
                if (ParseWaitTime())                    continue;  // Wait time
                if (ParsePointingExpPlateSolveTime())   continue;  // Pointing exposure/plate solve time
                if (ParseGuiderFailure())               continue;  // Guider failure (with carry on) count
            }

            return m_isACPLog;
        }
        #endregion

        #region Log parsing methods
        /// <summary>
        /// Parse comment line ("#")
        /// </summary>
        /// <returns>Returns true if the event was successfully parsed, false otherwise</returns>
        public bool ParseComment()
        {
            if (m_lineLower.Trim()[0].CompareTo('#') == 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Parse valid ACP log ("acp console log opened")
        /// </summary>
        /// <returns>Returns true if the event was successfully parsed, false otherwise</returns>
        public bool ParseValidACPLog()
        {
            m_isACPLog = false;
            DateTime tmpDate;
            string lineLower;

            if (LogFileText == null || LogFileText.Count == 0)
                return false;

            foreach (string line in this.LogFileText)
            {
                lineLower = line.ToLower();

                if (lineLower == null || lineLower.Length == 0)
                    continue;

                if (lineLower.IndexOf("acp console log opened") != -1)
                {
                    try
                    {
                        m_isACPLog = true;
                        if (!DateTime.TryParse(
                            lineLower.Substring(lineLower.IndexOf("opened") + "opened".Length + 1, "dd-mmm-yyyy hh:mm:ss".Length),
                                out tmpDate))
                        {
                            this.m_startDate = null;
                        }
                        else
                            this.m_startDate = tmpDate;

                        break;
                    }
                    catch
                    {
                        this.m_startDate = null;
                    }
                }
            }

            return m_isACPLog;
        }

        /// <summary>
        /// Parse log start\preamble ("acp console log opened", "this is acp version", "licensed to")
        /// </summary>
        /// <returns>Returns true if the event was successfully parsed, false otherwise</returns>
        public bool ParseLogPreamble()
        {
            if (LogFileText == null || LogFileText.Count == 0)
                return false;

            if (m_lineLower.IndexOf("acp console log opened") != -1 ||
                m_lineLower.IndexOf("this is acp version") != -1 ||
                m_lineLower.IndexOf("licensed to") != -1)
                return true;
            else
                return false;
        }

        /// <summary> 
        /// Parse end of log ("acp console log closed")
        /// </summary>
        /// <returns>Returns true if the event was successfully parsed, false otherwise</returns>
        public bool ParseEndOfLog()
        {
            DateTime tmpDate;
            if (m_lineLower.IndexOf("acp console log closed") != -1)
            {
                try
                {
                    if (!DateTime.TryParse(
                        m_lineLower.Substring(m_lineLower.IndexOf("closed") + "closed".Length + 1,
                        "dd-mmm-yyyy hh:mm:ss".Length), out tmpDate))
                    {
                        this.m_endDate = null;
                    }
                    else
                        this.m_endDate = tmpDate;
                }
                catch
                {
                    this.m_endDate = null;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Parse start of new imaging target ("starting target")
        /// </summary>
        /// <returns>Returns true if the event was successfully parsed, false otherwise</returns>
        public bool ParseImagingTargetStart()
        {
            string tmpStr;
            if (m_lineLower.IndexOf("starting target") != -1)
            {
                try
                {
                    // We found a target
                    tmpStr = m_lineLower.Substring(m_lineLower.IndexOf("target") + "target".Length + 1);
                    tmpStr = tmpStr.Substring(0, tmpStr.IndexOf("="));
                    CurrentTarget = new Target(tmpStr.Trim(), this);
                    m_targets.Add(CurrentTarget);

                    // Add a log event for the new target...
                    DateTime? dtOpTime = GetOperationTime(m_lineLower);
                    LogEvents.Add(new LogEvent(
                        dtOpTime,
                        dtOpTime,
                        LogEventType.Target,
                        m_logLineIndex,
                        true,
                        CurrentTarget,
                        CurrentTarget.Name,
                        Path));

                    return true;
                }
                catch
                {
                }
            }

            return false;
        }

        /// <summary>
        /// Parse start of new exposure ("imaging to")
        /// </summary>
        /// <returns>Returns true if the event was successfully parsed, false otherwise</returns>
        public bool ParseImagingExposure()
        {
            if (m_lineLower.IndexOf("imaging to") != -1)
            {
                DateTime? dtOpTime = null;
                Exposure newExposure = FindExposure(m_logLineIndex, -1, out dtOpTime);
                if (newExposure != null)
                {
                    // Add a new exposure event
                    LogEvents.Add(new LogEvent(
                        dtOpTime,
                        dtOpTime,
                        LogEventType.Exposure,
                        m_logLineIndex,
                        true,
                        CurrentTarget,
                        newExposure,
                        Path));

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Parse FWHM value ("image fwhm is")
        /// </summary>
        /// <returns>Returns true if the event was successfully parsed, false otherwise</returns>
        public bool ParseFWHM()
        {
            if (m_lineLower.IndexOf("image fwhm is") != -1)
            {
                try
                {
                    string tmpStr;

                    // Changes re work item #129:
                    // Change FWHM measurements to work only on imaging exposures (i.e. exclude pointing FWHM's)
                    // Rule: Scan back until we encounter "imaging to" (this indicates an imaging FWHM)
                    //       If we find "updating pointing" first, we ignore this FWHM (it's a pointing update FWHM)
                    // Error:If we find end of log OR "image fwhm is" before either of the above, ignore the FWHM

                    if (!IsImagingFWHM(GetPreviousLogLineIndex(m_logLineIndex), -1))
                        return false;  // It was a pointing update FWHM - ignore

                    // Get the FWHM...
                    tmpStr = m_lineLower.Substring(m_lineLower.IndexOf("is") + "is".Length + 1);
                    tmpStr = tmpStr.Substring(0, tmpStr.IndexOf("arcsec"));

                    double tmpFWHM;
                    if (double.TryParse(tmpStr, out tmpFWHM))
                    {
                        // Add a log event for the FWHM measurement...
                        DateTime? dtStart = GetOperationTime(m_lineLower);
                        LogEvents.Add(new LogEvent(
                            dtStart,
                            dtStart,
                            LogEventType.FWHM,
                            m_logLineIndex,
                            true,
                            CurrentTarget,
                            tmpFWHM,
                            Path));

                        return true;
                    }
                }
                catch
                {
                }
            }

            return false;
        }

        /// <summary>
        /// Parse auto-focus time ("start slew to autofocus")
        /// </summary>
        /// <returns>Returns true if the event was successfully parsed, false otherwise</returns>
        public bool ParseAutoFocusTime()
        {
             //       -----------------------------------------------------------------------------------------
             // NOTE: When processing (successful or otherwise) of this directive is complete, we need to allow
             //       fall-through to the rest of the parsing rules (for the same line) so that "start to slew"
             //       can be parsed as part of pointing error (target slew) processing
             //       -----------------------------------------------------------------------------------------
             //   
             // The rule for true AF time (versions prior to 1.2 simply used the time reported by FocusMax) includes:
             //    
             //   The time to slew to the AF target
             //   Acquiring the AF star (includes a possible plate solve by FocusMax)
             //   The actual focusing by FM
             //   Re-slew back to the original target
             //   Pointing update/slew
             //    
             //   Begin:      "start slew to autofocus"
             //   End:        "autofocus finished", then find plate solve pointing error value 
             //   Exclusions: "plate solve error!" OR "no matching stars found" OR "solution is suspect" OR "**autofocus failed"
             //   Data:       Time span from Begin to End  

            if (m_lineLower.IndexOf("start slew to autofocus") != -1)
            {
                try
                {
                    // Preceded by "re-slew to target"? (only process if "re-slew to target" is not found prev line)
                    if (GetPreviousLogLine(m_logLineIndex).ToLower().IndexOf("re-slew to target") == -1)
                    {
                        // Only process the "start slew to autofocus" if it was NOT part of a center slew (part of an on-going AF op)

                        DateTime? dtStart = GetOperationTime(m_lineLower);
                        if (dtStart != null)  // null if we can't find the start time (unlikely)
                        {
                            int endIndex = FindAutoFocusOpEnd(GetNextLogLineIndex(m_logLineIndex), -1);   // OK: "autofocus finished"
                            // Err: "**autofocus failed"
                            if (endIndex != -1)  // -1 => AF failed (so we discard the mesurement)
                            {
                                DateTime? dtEnd = new DateTime?();
                                int lineNum;
                                FindPointingError(GetNextLogLineIndex(endIndex), -1, out dtEnd, out lineNum);

                                if (dtEnd != null)  // null => an error condition (e.g. a plate solve failure) so we ignore the measure
                                {
                                    TimeSpan opTSTmp = new TimeSpan(dtEnd.Value.Ticks);
                                    TimeSpan opTimeSpan = opTSTmp.Subtract(new TimeSpan(dtStart.Value.Ticks));

                                    // Add a log event for the AF time...
                                    if (opTimeSpan.TotalSeconds > 0)
                                    {
                                        LogEvents.Add(new LogEvent(
                                            dtStart,
                                            dtEnd,
                                            LogEventType.AutoFocus,
                                            m_logLineIndex,
                                            true,
                                            CurrentTarget,
                                            opTimeSpan.TotalSeconds,
                                            Path));

                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }

            return false;
        }

        /// <summary>
        /// Parse pointing error (object slew) ("start slew to")
        /// </summary>
        /// <returns>Returns true if the event was successfully parsed, false otherwise</returns>
        public bool ParsePointingError()
        {
             // The rule for a pointing error measurement after an object slew (slews to imaging targets, auto-focus targets and
             // return slews from AF targets is:
             //
             // Begin:      "Start slew to"
             // End:        "Pointing error is" (prior to "(slew complete)")
             // Exclusions: Anytime "Start slew to" is immediately *preceded* by "Re-slew to target"
             //             Rationale is this is a centering slew so you don’t want it
             // Exclusions: Any plate solve errors; "plate solve error!" OR "no matching stars found" OR "solution is suspect"
             // Data:       Next "Pointing error is" in the log

            if (m_lineLower.IndexOf("start slew to") != -1)
            {
                try
                {
                    // *** Pointing error measurement (object slew) ***
                    // Preceded by "re-slew to target"?
                    if (GetPreviousLogLine(m_logLineIndex).ToLower().IndexOf("re-slew to target") != -1)
                        return false;  // Ignore, it was a center slew

                    DateTime? dtPtErrorTime = new DateTime();
                    int peLineNum = -1;
                    double tmpPtErr = FindPointingError(GetNextLogLineIndex(m_logLineIndex), -1, out dtPtErrorTime, out peLineNum);

                    // FindPointingError():
                    // OK:       "pointing error is"
                    // Err (-1): "plate solve error!"
                    //           "no matching stars found"
                    //           "solution is suspect"
                    //           "start slew to {Target}" [where {Target} != "autofocus"]
                    //           "re-slew to target"

                    if (tmpPtErr != -1)  // return of negative pointing error signals an error - discard the measurement
                    {
                        // Have we capture this event before as part of an objec center slew event?
                        if (!HasLogEventBeenCaptured(dtPtErrorTime, LogEventType.PointingErrorCenterSlew))
                        {
                            // Add a log event for the measurement...
                            LogEvents.Add(new LogEvent(
                                dtPtErrorTime,
                                dtPtErrorTime,
                                LogEventType.PointingErrorObjectSlew,
                                peLineNum,
                                true,
                                CurrentTarget,
                                tmpPtErr,
                                Path));

                            return true;
                        }
                    }
                }
                catch
                {
                }
            }

            return false;
        }

        /// <summary>
        /// Parse slew to target time ("start slew to")
        /// </summary>
        /// <returns>Returns true if the event was successfully parsed, false otherwise</returns>
        public bool ParseSlewToTargetTime()
        {
            if (m_lineLower.IndexOf("start slew to") != -1)
            {
                try
                {
                    // *** Slew (to target) time (not centering slews)***

                    // Preceded by "start slew to autofocus"?
                    if (GetPreviousLogLine(m_logLineIndex).ToLower().IndexOf("re-slew to target") != -1)
                        return false;  // Ignore, it was an center slew

                    // Now get the start/end time for slew
                    DateTime? dtStart = GetOperationTime(m_lineLower);
                    DateTime? dtEnd = FindSlewOpEndTime(GetNextLogLineIndex(m_logLineIndex), -1);  // OK: "slew complete"
                    // Error: end of log found before "slew complete"

                    if (dtStart == null || dtEnd == null)
                        return false;  // Can't find the start/end time of the current op 

                    TimeSpan opTSTmp = new TimeSpan(dtEnd.Value.Ticks);
                    TimeSpan opTimeSpan = opTSTmp.Subtract(new TimeSpan(dtStart.Value.Ticks));

                    // Add a log event for the slew time...
                    if (opTimeSpan.TotalSeconds > 0)
                    {
                        LogEvents.Add(new LogEvent(
                            dtStart,
                            dtEnd,
                            LogEventType.SlewTarget,
                            m_logLineIndex,
                            true,
                            CurrentTarget,
                            opTimeSpan.TotalSeconds,
                            Path));

                        return true;
                    }
                }
                catch
                {
                }
            }
            return false;
        }

        /// <summary>
        /// Parse pointing error (center slew) ("re-slew to target")
        /// </summary>
        /// <returns>Returns true if the event was successfully parsed, false otherwise</returns>
        public bool ParsePointingErrorCenterSlew()
        {
            // The rule for a pointing error measurement after a center slew is:
            //  
            // Begin:      "Re-slew to target"
            // End:        "(slew complete)"
            // Exclusions: Anytime "Re-slew to target" is immediately followed by "Start slew to autofocus"
            //             Rationale is there is no plate-solve after the re-slew to a focus star
            // Exclusions: Any plate solve errors; "plate solve error!" OR "no matching stars found" OR "solution is suspect"
            // Data:       "Pointing error is".  
            //             I'm pretty sure this will always be the next plate-solve, but for sure it will the next 
            //             "Pointing error is" following "Plate-solve final image".  
            //             Rationale is the only time you measure the quality of the centering slew is in the final 
            //             image plate-solve.
            //             There will be cases where there is a re-slew after a "Plate-solve final image" and others 
            //             where there isn't ("Within max error tolerance, no recenter needed").  
            //             I think the rules already account for both of these situations

            if (m_lineLower.IndexOf("re-slew to target") != -1)
            {
                try
                {
                    double tmpPtErr;

                    // Followed by "Start slew to autofocus"?
                    if (GetNextLogLine(m_logLineIndex).ToLower().IndexOf("start slew to autofocus") != -1)
                        return false;  // Ignore, it was an object slew

                    DateTime? dtPtErrorTime = new DateTime();
                    int lineNum;
                    if (GetNextLogLine(m_logLineIndex).ToLower().IndexOf("start slew to") != -1)
                        tmpPtErr = FindPointingError(GetNextLogLineIndex(GetNextLogLineIndex(m_logLineIndex)), -1, out dtPtErrorTime, out lineNum);  // Skip (2 lines) over the line "Start slew to {imaging target}"
                    else
                        tmpPtErr = FindPointingError(GetNextLogLineIndex(m_logLineIndex), -1, out dtPtErrorTime, out lineNum);  // Start searching on the next line

                    // FindPointingError():
                    // OK:       "pointing error is"
                    // Err (-1): "plate solve error!"
                    //           "no matching stars found"
                    //           "solution is suspect"
                    //           "start slew to {Target}" [where {Target} != "autofocus"]
                    //           "re-slew to target"

                    if (tmpPtErr != -1)  // return of negative pointing error signals an error - discard the measurement
                    {
                        // Add a log event for the measurement...
                        LogEvents.Add(new LogEvent(
                            dtPtErrorTime,
                            dtPtErrorTime,
                            LogEventType.PointingErrorCenterSlew,
                            lineNum,
                            true,
                            CurrentTarget,
                            tmpPtErr,
                            Path));

                        return true;
                    }
                }
                catch
                {
                }
            }

            return false;
        }

        /// <summary>
        /// Parse plate solve count ("solved!")
        /// </summary>
        /// <returns>Returns true if the event was successfully parsed, false otherwise</returns>
        public bool ParsePlateSolveCount()
        {
            if (m_lineLower.IndexOf("solved!") != -1)
            {
                try
                {
                    // Get the start/end time for the error
                    DateTime? dtStart = GetOperationTime(m_lineLower);
                    if (dtStart == null)
                        return false;  // Can't find the start/end time of the current op

                    LogEvents.Add(new LogEvent(
                        dtStart,
                        dtStart,
                        LogEventType.PlateSolveSuccess,
                        m_logLineIndex,
                        true,
                        CurrentTarget,
                        "Plate solve ok",
                        Path));

                    return true;
                }
                catch
                {
                }
            }

            return false;
        }

        /// <summary>
        /// Plate plate solve error count ("plate solve error!", "no matching stars found", "solution is suspect")
        /// </summary>
        /// <returns>Returns true if the event was successfully parsed, false otherwise</returns>
        public bool ParsePlateSolveErrorCount()
        {
            if (m_lineLower.IndexOf("plate solve error!") != -1 ||
                m_lineLower.IndexOf("no matching stars found") != -1 ||
                m_lineLower.IndexOf("solution is suspect") != -1)
            {
                try
                {
                    // Get the start/end time for the error
                    DateTime? dtStart = GetOperationTime(m_lineLower);
                    if (dtStart == null)
                        return false;  // Can't find the start/end time of the current op

                    LogEvents.Add(new LogEvent(
                        dtStart,
                        dtStart,
                        LogEventType.PlateSolveFail,
                        m_logLineIndex,
                        false,
                        CurrentTarget,
                        "Plate solve fail",
                        Path));

                    return true;
                }
                catch
                {
                }
            }

            return false;
        }

        /// <summary>
        /// Parse auto-focus count ("auto-focus successful!")
        /// </summary>
        /// <returns>Returns true if the event was successfully parsed, false otherwise</returns>
        public bool ParseAutoFocusCount()
        {
            if (m_lineLower.IndexOf("auto-focus successful!") != -1)
            {
                try
                {
                    // Get the start/end time for the error
                    DateTime? dtStart = GetOperationTime(m_lineLower);
                    if (dtStart == null)
                        return false;  // Can't find the start/end time of the current op

                    LogEvents.Add(new LogEvent(
                        dtStart,
                        dtStart,
                        LogEventType.AutoFocusSuccess,
                        m_logLineIndex,
                        true,
                        CurrentTarget,
                        "AF ok",
                        Path));

                    return true;
                }
                catch
                {
                }
            }

            return false;
        }

        /// <summary>
        /// Parse auto-focus error count ("**autofocus failed")
        /// </summary>
        /// <returns>Returns true if the event was successfully parsed, false otherwise</returns>
        public bool ParseAutoFocusErrorCount()
        {
            if (m_lineLower.IndexOf("**autofocus failed") != -1)
            {
                try
                {
                    // Get the start/end time for the error
                    DateTime? dtStart = GetOperationTime(m_lineLower);
                    if (dtStart == null)
                        return false;  // Can't find the start/end time of the current op

                    LogEvents.Add(new LogEvent(
                        dtStart,
                        dtStart,
                        LogEventType.AutoFocusFail,
                        m_logLineIndex,
                        false,
                        CurrentTarget,
                        "AF fail",
                        Path));

                    return true;
                }
                catch
                {
                }
            }

            return false;
        }

        /// <summary>
        /// Parse HFD value ("HFD =")
        /// </summary>
        /// <returns>Returns true if the event was successfully parsed, false otherwise</returns>
        public bool ParseHFD()
        {
            if (m_lineLower.IndexOf("auto-focus successful!") != -1)
            {
                // 01:48:36   FocusMax auto-focus successful!
                // 01:48:36     HFD = 3.26

                // Get the HFD...
                DateTime? dtOpTime = new DateTime();
                double tmpHFD = FindHFDOpEndTime(GetNextLogLineIndex(m_logLineIndex), 10, out dtOpTime);
                if (tmpHFD != -1)
                {
                    // Add a log event for the HFD measurement...
                    LogEvents.Add(new LogEvent(
                        dtOpTime,
                        dtOpTime,
                        LogEventType.HFD,
                        m_logLineIndex + 1,
                        true,
                        CurrentTarget,
                        tmpHFD,
                        Path));

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Parse script error count ("script error")
        /// </summary>
        /// <returns>Returns true if the event was successfully parsed, false otherwise</returns>
        public bool ParseScriptError()
        {
            if (m_lineLower.IndexOf("script error") != -1)
            {
                try
                {
                    // Get the start/end time for the error
                    DateTime? dtStart = GetOperationTime(m_lineLower);
                    if (dtStart == null)
                        return false;  // Can't find the start/end time of the current op

                    LogEvents.Add(new LogEvent(
                        dtStart,
                        dtStart,
                        LogEventType.ScriptError,
                        m_logLineIndex,
                        false,
                        CurrentTarget,
                        "Script error",
                        Path));

                    return true;
                }
                catch
                {
                }
            }

            return false;
        }

        /// <summary>
        /// Parse script abort count ("script was aborted")
        /// </summary>
        /// <returns>Returns true if the event was successfully parsed, false otherwise</returns>
        public bool ParseScriptAbort()
        {
            if (m_lineLower.IndexOf("script was aborted") != -1)
            {
                try
                {
                    // Get the start/end time for the abort
                    DateTime? dtStart = GetOperationTime(m_lineLower);
                    if (dtStart == null)
                        return false;  // Can't find the start/end time of the current op

                    LogEvents.Add(new LogEvent(
                        dtStart,
                        dtStart,
                        LogEventType.ScriptAbort,
                        m_logLineIndex,
                        false,
                        CurrentTarget,
                        "Script abort",
                        Path));

                    return true;
                }
                catch
                {
                }
            }

            return false;
        }

        /// <summary>
        /// Parse guider start-up time ("trying to autoguide")
        /// </summary>
        /// <returns>Returns true if the event was successfully parsed, false otherwise</returns>
        public bool ParseGuiderStartUpTime()
        {
            if (m_lineLower.IndexOf("trying to autoguide") != -1)
            {
                try
                {
                    // Get the start/end time for guider start-up
                    int lineNum;
                    DateTime? dtStart = GetOperationTime(m_lineLower);
                    DateTime? dtEnd = FindGuiderStartUpOpEndTime(GetNextLogLineIndex(m_logLineIndex), -1, out lineNum);  // Look for "autoguiding at nnn"
                    if (dtStart == null || dtEnd == null)
                        return false;  // Can't find the start/end time of the current op

                    TimeSpan opTSTmp = new TimeSpan(dtEnd.Value.Ticks);
                    TimeSpan opTimeSpan = opTSTmp.Subtract(new TimeSpan(dtStart.Value.Ticks));

                    // Add a log event for the guider start-up time...
                    if (opTimeSpan.TotalSeconds > 0)
                    {
                        LogEvents.Add(new LogEvent(
                            dtStart,
                            dtEnd,
                            LogEventType.GuiderStartUp,
                            m_logLineIndex,
                            true,
                            CurrentTarget,
                            opTimeSpan.TotalSeconds,
                            Path));

                        return true;
                    }
                }
                catch
                {
                }
            }

            return false;
        }

        /// <summary>
        /// Parse guider settle time ("guider check ok")
        /// </summary>
        /// <returns>Returns true if the event was successfully parsed, false otherwise</returns>
        public bool ParseGuiderSettleTime()
        {
            if (m_lineLower.IndexOf("guider check ok") != -1)
            {
                try
                {
                    // Get the start/end time for guider settle
                    int lineNum;
                    DateTime? dtEnd = GetOperationTime(m_lineLower);  // Note we have the END time for when the guider settled
                    DateTime? dtStart = FindGuiderSettleOpStartTime(GetPreviousLogLineIndex(m_logLineIndex), -1, out lineNum);  // Now find the start time (scan back up the log)
                    if (dtStart == null || dtEnd == null)
                        return false;  // Can't find the start/end time of the current op 

                    TimeSpan opTSTmp = new TimeSpan(dtEnd.Value.Ticks);
                    TimeSpan opTimeSpan = opTSTmp.Subtract(new TimeSpan(dtStart.Value.Ticks));

                    // Add a log event for the guider settle time...
                    if (opTimeSpan.TotalSeconds > 0)
                    {
                        LogEvents.Add(new LogEvent(
                            dtStart,
                            dtEnd,
                            LogEventType.GuiderSettle,
                            lineNum,
                            true,
                            CurrentTarget,
                            opTimeSpan.TotalSeconds,
                            Path));

                        return true;
                    }
                }
                catch
                {
                }
            }

            return false;
        }

        /// <summary>
        /// Parse filter change time ("switching from")
        /// </summary>
        /// <returns>Returns true if the event was successfully parsed, false otherwise</returns>
        public bool ParseFilterChangeTime()
        {
            // The rule for filter change time (versions prior to 1.2 were not correctly calulating the time) is:
            //
            // Begin:      "switching from"
            // End:        either the next line or the one after that includes "(taking"
            // Exclusions: If the next line (or one after) includes "(guide star"
            // Data:       Time span from Begin to End 
            // 
            // If not doing a pointing update there's *no way of working out the filter change time*. This is because
            // the change time is included as part of the guider start-up time. For example:
            //  
            // 00:14:29   Switching from Clear to Green filter for imaging
            // 00:14:29   Focus change of -40 steps required
            // 00:14:54   (Guide star SNR=231.5; X=74.2, Y=90.7; Aggressiveness=5)
            //
            // As part of a pointing exposure we can see the actual time taken:
            //
            // 00:10:55   (doing post-flip pointing update...)
            // 00:10:55   Updating pointing...
            // >00:10:55   Switching from Green to Clear filter for pointing exposure
            // 00:10:55   Focus change of 40 steps required  [this line is only present in systems where filters are not parfocal]
            // >00:11:07   (taking 15 sec. exposure, Clear filter, binning = 3)

            if (m_lineLower.IndexOf("switching from") != -1)
            {
                try
                {
                    // Get the start/end time for filter change
                    DateTime? dtStart = GetOperationTime(m_lineLower);
                    DateTime? dtEnd = FindFilterChangeOpEndTime(GetNextLogLineIndex(m_logLineIndex), 5);
                    if (dtStart == null || dtEnd == null)
                        return false;  // Can't find the start/end time of the current op, or unable to determine real filter change time

                    TimeSpan opTSTmp = new TimeSpan(dtEnd.Value.Ticks);
                    TimeSpan opTimeSpan = opTSTmp.Subtract(new TimeSpan(dtStart.Value.Ticks));

                    // Add a log event for the filter change time...
                    if (opTimeSpan.TotalSeconds > 0)
                    {
                        LogEvents.Add(new LogEvent(
                            dtStart,
                            dtEnd,
                            LogEventType.FilterChange,
                            m_logLineIndex,
                            true,
                            CurrentTarget,
                            opTimeSpan.TotalSeconds,
                            Path));

                        return true;
                    }
                }
                catch
                {
                }
            }

            return false;
        }

        /// <summary>
        /// Parse wait time ("wait until")
        /// </summary>
        /// <returns>Returns true if the event was successfully parsed, false otherwise</returns>
        public bool ParseWaitTime()
        {
            if (m_lineLower.IndexOf("wait until") != -1)
            {
                try
                {
                    // Get the start/end time for the wait
                    DateTime? dtStart = GetOperationTime(m_lineLower);
                    DateTime? dtEnd = FindWaitOpEndTime(GetNextLogLineIndex(m_logLineIndex), -1);
                    if (dtStart == null || dtEnd == null)
                        return false;  // Can't find the start/end time of the current op

                    TimeSpan opTSTmp = new TimeSpan(dtEnd.Value.Ticks);
                    TimeSpan opTimeSpan = opTSTmp.Subtract(new TimeSpan(dtStart.Value.Ticks));

                    if (opTimeSpan.TotalSeconds > 0)
                    {
                        LogEvents.Add(new LogEvent(
                            dtStart,
                            dtEnd,
                            LogEventType.Wait,
                            m_logLineIndex,
                            true,
                            CurrentTarget,
                            opTimeSpan.TotalSeconds,
                            Path));

                        return true;
                    }
                }
                catch
                {
                }
            }

            return false;
        }

        /// <summary>
        /// Parse pointing exposure/plate solve time ("updating pointing")
        /// </summary>
        /// <returns>Returns true if the event was successfully parsed, false otherwise</returns>
        public bool ParsePointingExpPlateSolveTime()
        {
            if (m_lineLower.IndexOf("updating pointing") != -1)
            {
                try
                {
                    // Get the start/end time for the plate solve
                    DateTime? dtStart = GetOperationTime(m_lineLower);
                    DateTime? dtEnd = FindPlateSolveOpEndTime(GetNextLogLineIndex(m_logLineIndex), -1);  // Returns null if the op failed
                    if (dtStart == null || dtEnd == null)
                        return false;  // Can't find the start/end time of the current op so ignore the results

                    TimeSpan opTSTmp = new TimeSpan(dtEnd.Value.Ticks);
                    TimeSpan opTimeSpan = opTSTmp.Subtract(new TimeSpan(dtStart.Value.Ticks));

                    // Add a log event...
                    if (opTimeSpan.TotalSeconds > 0)
                    {
                        LogEvents.Add(new LogEvent(
                            dtStart,
                            dtEnd,
                            LogEventType.PointingExpAndPlateSolve,
                            m_logLineIndex,
                            true,
                            CurrentTarget,
                            opTimeSpan.TotalSeconds,
                            Path));

                        return true;
                    }
                }
                catch
                {
                }
            }

            return false;
        }

        /// <summary>
        /// Guider failure ("**autoguiding failed", "excessive guiding errors", "guider stopped or lost star")
        /// </summary>
        /// <returns>Returns true if the event was successfully parsed, false otherwise</returns>
        public bool ParseGuiderFailure()
        {
            if (m_lineLower.IndexOf("**autoguiding failed") != -1 ||
                m_lineLower.IndexOf("excessive guiding errors") != -1 ||
                m_lineLower.IndexOf("guider stopped or lost star") != -1)
            {
                // We found a guiding failure - see if ACP carried on imaging anyway (this is the condition 
                // [failure then continue] that we count)
                if (FindGuiderFailureRecovery(GetNextLogLineIndex(m_logLineIndex), 5))
                {
                    DateTime? dtStart = GetOperationTime(m_lineLower);
                    LogEvents.Add(new LogEvent(
                            dtStart,
                            dtStart,
                            LogEventType.GuiderFail,
                            m_logLineIndex,
                            false,
                            CurrentTarget,
                            true,  // true signals that imaging continued unguided
                            Path));

                    return true;
                }
            }

            return false;
        }
        #endregion

        #region Log parsing support methods
        /// <summary>
        /// Returns the line that logically immediately precedes the line indicated by lineIndex (the current line)
        /// This method ignores any comment lines
        /// </summary>
        /// <param name="lineIndex">Log line index to start searching from</param>
        /// <returns>Returns the line that logically immediately precedes the line indicated by lineIndex (the current line), or
        /// an empty string if no suitable line is found</returns>
        private string GetPreviousLogLine(int lineIndex)
        {
            try
            {
                for (int tmpLineIndex = lineIndex - 1 /* Start BEFORE current line */; tmpLineIndex > 0; tmpLineIndex--)
                {
                    if (this.LogFileText[tmpLineIndex][0].CompareTo('#') != 0)
                        return this.LogFileText[tmpLineIndex];
                }
            }
            catch
            {
            }
            return "";  // Start of log found before suitable line
        }

        /// <summary>
        /// Returns the line that logically immediately follows the line indicated by lineIndex (the current line)
        /// This method ignores any comment lines
        /// </summary>
        /// <param name="lineIndex">Log line index to start searching from</param>
        /// <returns>Returns the line that logically immediately follows the line indicated by lineIndex (the current line), or
        /// an empty string if no suitable line is found</returns>
        private string GetNextLogLine(int lineIndex)
        {
            try
            {
                for (int tmpLineIndex = lineIndex + 1 /* Start AFTER the current line */; tmpLineIndex < LogFileText.Count; tmpLineIndex++)
                {
                    if (this.LogFileText[tmpLineIndex][0].CompareTo('#') != 0)
                        return this.LogFileText[tmpLineIndex];
                }
            }
            catch
            {
            }
            return "";  // End of log found before suitable line
        }

        /// <summary>
        /// Returns the index of the line that logically immediately precedes the line indicated by lineIndex (the current line)
        /// This method ignores (skips over) any comment lines
        /// </summary>
        /// <param name="lineIndex">Log line index to start searching from</param>
        /// <returns>Returns the index of the line that logically immediately precedes the line indicated by lineIndex (the current line),
        /// or 0 if no suitable line is found</returns>
        private int GetPreviousLogLineIndex(int lineIndex)
        {
            try
            {
                for (int tmpLineIndex = lineIndex - 1 /* Start BEFORE current line */; tmpLineIndex > 0; tmpLineIndex--)
                {
                    if (this.LogFileText[tmpLineIndex][0].CompareTo('#') != 0)
                        return tmpLineIndex;
                }
            }
            catch
            {
            }
            return 0;  // Start of log found before suitable line
        }

        /// <summary>
        /// Returns the index of the line that logically immediately follows the line indicated by lineIndex (the current line)
        /// This method ignores (skips over) any comment lines
        /// </summary>
        /// <param name="lineIndex">Log line index to start searching from</param>
        /// <returns>Returns the index of the line that logically immediately follows the line indicated by lineIndex (the current line),
        /// or the highlest line number in the log if no suitable line is found</returns>
        private int GetNextLogLineIndex(int lineIndex)
        {
            try
            {
                for (int tmpLineIndex = lineIndex + 1 /* Start AFTER the current line */; tmpLineIndex < LogFileText.Count; tmpLineIndex++)
                {
                    if (this.LogFileText[tmpLineIndex][0].CompareTo('#') != 0)
                        return tmpLineIndex;
                }
            }
            catch
            {
            }
            return LogFileText.Count;  // End of log found before suitable line
        }

        /// <summary>
        /// Returns true if the current line is a comment line (starts with '#')
        /// </summary>
        /// <param name="lineIndex">Log line index</param>
        /// <returns>Returns true if the current line is a comment line (starts with '#'), false otherwise</returns>
        private bool IsCommentLine(int lineIndex)
        {
            if (this.LogFileText[lineIndex][0].CompareTo('#') == 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Scan up the log until we encounter "imaging to" (this indicates an imaging FWHM)
        /// If we find "updating pointing" first, we return false (it's a pointing update FWHM)
        /// If we find start-of-log OR "image fwhm is" before either of the above, ignore the FWHM (returns false)
        /// </summary>
        /// <param name="lineIndex">Log line index</param>
        /// <returns>Returns true if the FWHM is part of an imaging exposure, false otherwise</returns>
        private bool IsImagingFWHM(int lineIndex, int maxLinesToScan)
        {
            try
            {
                int lineScanCount = 0;
                if (maxLinesToScan == -1)
                    maxLinesToScan = int.MaxValue;  // Unlimited

                for (int tmpLineIndex = lineIndex; tmpLineIndex > 0 && lineScanCount < maxLinesToScan; tmpLineIndex--)
                {
                    if (IsCommentLine(tmpLineIndex))
                        continue;

                    lineScanCount++;
                    if (this.LogFileText[tmpLineIndex].ToLower().IndexOf("imaging to") != -1)
                        return true;
                    else if (this.LogFileText[tmpLineIndex].ToLower().IndexOf("updating pointing") != -1 ||
                            (this.LogFileText[tmpLineIndex].ToLower().IndexOf("image fwhm is") != -1))
                        return false;
                }
            }
            catch
            {
            }
            return false;
        }

        /// <summary>
        /// Start scanning down the log until we discover an imaging exposure
        /// </summary>
        /// <param name="lineIndex">Log line index to start searching from</param>
        /// <param name="maxLinesToScan">Number of lines to scan before reporting an error</param>
        /// <returns>Returns a valid Exposure object if the exposure is found, null otherwise</returns>
        private Exposure FindExposure(int lineIndex, int maxLinesToScan, out DateTime? opDateTime)
        {
            Exposure tmpExposure = null;
            string tmpStr;
            string lineLower;
            int lineScanCount = 0;
            if (maxLinesToScan == -1)
                maxLinesToScan = int.MaxValue;  // Unlimited

            try
            {
                for (int tmpLineIndex = lineIndex; tmpLineIndex < LogFileText.Count && lineScanCount < maxLinesToScan; tmpLineIndex++)
                {
                    if (IsCommentLine(tmpLineIndex))
                        continue;

                    lineScanCount++;
                    lineLower = this.LogFileText[tmpLineIndex].ToLower();

                    if (lineLower.IndexOf("taking") != -1)
                    {
                        // The line here will take the format:
                        // (taking {duration} sec. exposure, {filterName} filter, binning = {bin})
                        tmpExposure = new Exposure();

                        // Get the exposure duration...
                        tmpStr = lineLower.Substring(lineLower.IndexOf("taking") + "taking".Length + 1);
                        tmpStr = tmpStr.Substring(0, tmpStr.IndexOf("s"));
                        double tmpDuration;
                        if (!double.TryParse(tmpStr, out tmpDuration))
                            tmpExposure.Duration = 0;
                        else
                            tmpExposure.Duration = tmpDuration;

                        // Get the exposure filter name...
                        tmpStr = lineLower.Substring(lineLower.IndexOf("exposure") + "exposure".Length + 1);
                        tmpStr = tmpStr.Substring(0, tmpStr.IndexOf("filter"));
                        tmpExposure.Filter = tmpStr.Trim();

                        // Get the binning value...
                        tmpStr = lineLower.Substring(lineLower.IndexOf("binning") + "binning".Length + 1);
                        tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(")"));
                        tmpStr = tmpStr.Substring(tmpStr.IndexOf("=") + 1);

                        int tmpBin;
                        if (!int.TryParse(tmpStr, out tmpBin))
                            tmpExposure.Bin = 0;
                        else
                            tmpExposure.Bin = tmpBin;

                        //TODO: Remove: CurrentTarget.Exposures.Add(tmpExposure);

                        opDateTime = GetOperationTime(lineLower);
                        return tmpExposure;
                    }
                }
            }
            catch
            {
            }

            opDateTime = null;
            return null;
        }

        /// <summary>
        /// Start scanning down the log until we find the next pointing error.
        /// If we encounter any of the following, an error is returned and the pointing error measurement should be discarded:
        /// "plate solve error!" OR "no matching stars found" OR "solution is suspect" OR "start slew to" OR "re-slew to target"
        /// </summary>
        /// <param name="lineIndex">Log line index to start searching from</param>
        /// <param name="maxLinesToScan">Number of lines to scan before reporting an error</param>
        /// <param name="opDateTime">A nullable DateTime that will contain the time of the pointing error</param>
        /// <returns>Returns the pointing error value, -1 otherwise (or an error was encountered)</returns>
        private double FindPointingError(int lineIndex, int maxLinesToScan, out DateTime? opDateTime, out int lineNum)
        {
            string tmpStr;
            string tmpLine;
            double tmpPtErr;
            int lineScanCount = 0;
            if (maxLinesToScan == -1)
                maxLinesToScan = int.MaxValue;  // Unlimited

            try
            {
                for (int tmpLineIndex = lineIndex; tmpLineIndex < LogFileText.Count && lineScanCount < maxLinesToScan; tmpLineIndex++)
                {
                    if (IsCommentLine(tmpLineIndex))
                        continue;

                    lineScanCount++;
                    tmpLine = this.LogFileText[tmpLineIndex].ToLower();
                    if (tmpLine.IndexOf("plate solve error!") != -1 ||
                        tmpLine.IndexOf("no matching stars found") != -1 ||
                        tmpLine.IndexOf("solution is suspect") != -1 ||
                        tmpLine.IndexOf("re-slew to target") != -1)
                    {
                        // Finding any of the above before we find the point error value is an error condition
                        break;
                    }
                    else if (tmpLine.IndexOf("start slew to") != -1)
                    {
                        if (tmpLine.IndexOf("start slew to autofocus") != -1)
                            continue;  // That's OK - continue scanning for the the pointing error measurement
                        else
                            break;  // Slew to a target other than AF is an error at this point
                    }
                    else if (tmpLine.IndexOf("pointing error is") != -1)
                    {
                        // Get the pointing error...
                        tmpStr = tmpLine.Substring(tmpLine.IndexOf("is") + "is".Length + 1);
                        tmpStr = tmpStr.Substring(0, tmpStr.IndexOf("arcmin"));

                        if (double.TryParse(tmpStr, out tmpPtErr))
                        {
                            opDateTime = GetOperationTime(tmpLine);
                            lineNum = tmpLineIndex;
                            return tmpPtErr;
                        }
                    }
                }
            }
            catch
            {
            }

            opDateTime = null;
            lineNum = -1;
            return -1;
        }

        /// <summary>
        /// Start scanning down the log until we find the next pointing error.
        /// If we encounter any of the following, an error is returned and the pointing error measurement should be discarded:
        /// "plate solve error!" OR "no matching stars found" OR "solution is suspect" OR "start slew to" OR "re-slew to target"
        /// </summary>
        /// <param name="lineIndex">Log line index to start searching from</param>
        /// <param name="maxLinesToScan">Number of lines to scan before reporting an error</param>
        /// <returns>Returns the pointing error value, -1 otherwise (or an error was encountered)</returns>
        private double FindPointingError(int lineIndex, int maxLinesToScan)
        {
            DateTime? tmpDate = new DateTime?();
            int lineNum;
            return FindPointingError(lineIndex, maxLinesToScan, out tmpDate, out lineNum);
        }

        /// <summary>
        /// Start scanning down the log until we discover the time when AF has completed
        /// </summary>
        /// <param name="lineIndex">Log line index to start searching from</param>/// 
        /// <param name="maxLinesToScan">Number of lines to scan before reporting an error</param>
        /// <returns>Returns the index of the the ending statement</returns>
        private int FindAutoFocusOpEnd(int lineIndex, int maxLinesToScan)
        {
            try
            {
                int lineScanCount = 0;
                if (maxLinesToScan == -1)
                    maxLinesToScan = int.MaxValue;  // Unlimited

                for (int tmpLineIndex = lineIndex; tmpLineIndex < LogFileText.Count && lineScanCount < maxLinesToScan; tmpLineIndex++)
                {
                    if (IsCommentLine(tmpLineIndex))
                        continue;

                    lineScanCount++;
                    if (this.LogFileText[tmpLineIndex].ToLower().IndexOf("**autofocus failed") != -1)
                        return -1;  // AF failed
                    else if (this.LogFileText[tmpLineIndex].ToLower().IndexOf("autofocus finished") != -1)
                        return tmpLineIndex;
                }
            }
            catch
            {
            }
            return -1;  // Error - couldn't find the end of AF
        }

        /// <summary>
        /// Start scanning up the log until we discover the start of guider settle time
        /// </summary>
        /// <param name="lineIndex">Log line index to start searching from</param>
        /// <param name="maxLinesToScan">Number of lines to scan before reporting an error</param>
        /// <returns>Return the DateTime of the event</returns>
        private DateTime? FindGuiderSettleOpStartTime(int lineIndex, int maxLinesToScan, out int lineNum)
        {
            try
            {
                int lineScanCount = 0;
                if (maxLinesToScan == -1)
                    maxLinesToScan = int.MaxValue;  // Unlimited

                for (int tmpLineIndex = lineIndex; tmpLineIndex > 0 && lineScanCount < maxLinesToScan; tmpLineIndex--)
                {
                    if (IsCommentLine(tmpLineIndex))
                        continue;

                    lineScanCount++;
                    if (this.LogFileText[tmpLineIndex].ToLower().IndexOf("imaging to") != -1)
                    {
                        lineNum = tmpLineIndex;
                        return GetOperationTime(this.LogFileText[tmpLineIndex]);
                    }
                }
            }
            catch
            {
            }

            lineNum = -1;
            return null;
        }

        /// <summary>
        /// Start scanning down the log until we when the filter change completed
        /// </summary>
        /// <param name="lineIndex">Log line index to start searching from</param>
        /// <param name="maxLinesToScan">Number of lines to scan before reporting an error</param>
        /// <returns>Return the DateTime of the event</returns>
        private DateTime? FindFilterChangeOpEndTime(int lineIndex, int maxLinesToScan)
        {
            // End:        "(taking"
            // Exclusions: "(guide star"
            // Data:       Time span from Begin to End 
            try
            {
                int lineScanCount = 0;
                if (maxLinesToScan == -1)
                    maxLinesToScan = int.MaxValue;  // Unlimited

                for (int tmpLineIndex = lineIndex; tmpLineIndex < LogFileText.Count && lineScanCount < maxLinesToScan; tmpLineIndex++)
                {
                    if (IsCommentLine(tmpLineIndex))
                        continue;

                    lineScanCount++;
                    if (this.LogFileText[tmpLineIndex].ToLower().IndexOf("(taking") != -1)
                        return GetOperationTime(this.LogFileText[tmpLineIndex]);
                    else if (this.LogFileText[tmpLineIndex].ToLower().IndexOf("(guide star") != -1)
                        return null;
                }
            }
            catch
            {
            }
            return null;
        }

        /// <summary>
        /// Start scanning down the log until we discover the time when the autoguider start-up has completed
        /// </summary>
        /// <param name="lineIndex">Log line index to start searching from</param>
        /// <param name="maxLinesToScan">Number of lines to scan before reporting an error</param>
        /// <returns>Return the DateTime of the event</returns>
        private DateTime? FindGuiderStartUpOpEndTime(int lineIndex, int maxLinesToScan, out int lineNum)
        {
            try
            {
                int lineScanCount = 0;
                if (maxLinesToScan == -1)
                    maxLinesToScan = int.MaxValue;  // Unlimited

                for (int tmpLineIndex = lineIndex; tmpLineIndex < LogFileText.Count && lineScanCount < maxLinesToScan; tmpLineIndex++)
                {
                    if (IsCommentLine(tmpLineIndex))
                        continue;

                    lineScanCount++;
                    if (this.LogFileText[tmpLineIndex].ToLower().IndexOf("autoguiding at") != -1)
                    {
                        lineNum = tmpLineIndex;
                        return GetOperationTime(this.LogFileText[tmpLineIndex]);
                    }
                }
            }
            catch
            {
            }

            lineNum = -1;
            return null;
        }

        /// <summary>
        /// Start scanning down the log until we discover the time when the slew has completed
        /// </summary>
        /// <param name="lineIndex">Log line index to start searching from</param>
        /// <param name="maxLinesToScan">Number of lines to scan before reporting an error</param>
        /// <returns>Return the DateTime of the event</returns>
        private DateTime? FindSlewOpEndTime(int lineIndex, int maxLinesToScan)
        {
            try
            {
                int lineScanCount = 0;
                if (maxLinesToScan == -1)
                    maxLinesToScan = int.MaxValue;  // Unlimited

                for (int tmpLineIndex = lineIndex; tmpLineIndex < LogFileText.Count && lineScanCount < maxLinesToScan; tmpLineIndex++)
                {
                    if (IsCommentLine(tmpLineIndex))
                        continue;

                    lineScanCount++;
                    if (this.LogFileText[tmpLineIndex].ToLower().IndexOf("slew complete") != -1)
                        return GetOperationTime(this.LogFileText[tmpLineIndex]);
                    else if (this.LogFileText[tmpLineIndex].ToLower().IndexOf("updating pointing") != -1 ||
                             this.LogFileText[tmpLineIndex].ToLower().IndexOf("re-slew to target") != -1 ||
                             this.LogFileText[tmpLineIndex].ToLower().IndexOf("start slew to") != -1)
                        return null;
                }
            }
            catch
            {
            }
            return null;
        }

        /// <summary>
        /// Start scanning down the log until we discover the time when the wait has completed
        /// </summary>
        /// <param name="lineIndex">Log line index to start searching from</param>
        /// <param name="maxLinesToScan">Number of lines to scan before reporting an error</param>
        /// <returns>Return the DateTime of the event</returns>
        private DateTime? FindWaitOpEndTime(int lineIndex, int maxLinesToScan)
        {
            try
            {
                int lineScanCount = 0;
                if (maxLinesToScan == -1)
                    maxLinesToScan = int.MaxValue;  // Unlimited

                for (int tmpLineIndex = lineIndex; tmpLineIndex < LogFileText.Count && lineScanCount < maxLinesToScan; tmpLineIndex++)
                {
                    if (IsCommentLine(tmpLineIndex))
                        continue;

                    lineScanCount++;
                    if (this.LogFileText[tmpLineIndex].ToLower().IndexOf("wait finished") != -1)
                        return GetOperationTime(this.LogFileText[tmpLineIndex]);
                }
            }
            catch
            {
            }
            return null;
        }

        /// <summary>
        /// Start scanning down the log until we discover the time when the plate solve failed/succeeded
        /// </summary>
        /// <param name="lineIndex">Log line index to start searching from</param>
        /// <param name="maxLinesToScan">Number of lines to scan before reporting an error</param>
        /// <returns>Returns the datetime for the end of the op, or null if the time could not be determined (or the op failed)</returns>
        private DateTime? FindPlateSolveOpEndTime(int lineIndex, int maxLinesToScan)
        {
            try
            {
                int lineScanCount = 0;
                if (maxLinesToScan == -1)
                    maxLinesToScan = int.MaxValue;  // Unlimited

                for (int tmpLineIndex = lineIndex; tmpLineIndex < LogFileText.Count && lineScanCount < maxLinesToScan; tmpLineIndex++)
                {
                    if (IsCommentLine(tmpLineIndex))
                        continue;

                    lineScanCount++;
                    if (this.LogFileText[tmpLineIndex].ToLower().IndexOf("target is now centered") != -1)
                        return GetOperationTime(this.LogFileText[tmpLineIndex]);
                    else if (this.LogFileText[tmpLineIndex].ToLower().IndexOf("**aiming failed") != -1)
                        return null;  // Tell the caller the op failed
                }
            }
            catch
            {
            }
            return null;
        }

        /// <summary>
        /// Start scanning down the log until we discover the time when ACP reports the HFD from a successful AF run
        /// </summary>
        /// <param name="lineIndex">Log line index to start searching from</param>
        /// <param name="maxLinesToScan">Number of lines to scan before reporting an error</param>
        /// <returns>Returns the datetime for the end of the op, or null if the time could not be determined (or the op failed)</returns>
        private double FindHFDOpEndTime(int lineIndex, int maxLinesToScan, out DateTime? opDateTime)
        {
            opDateTime = null;

            try
            {
                // Example:
                //
                // 01:48:36   FocusMax auto-focus successful!
                // 01:48:36     HFD = 3.26
                //

                string tmpStr;
                int lineScanCount = 0;
                if (maxLinesToScan == -1)
                    maxLinesToScan = int.MaxValue;  // Unlimited

                for (int tmpLineIndex = lineIndex; tmpLineIndex < LogFileText.Count && lineScanCount < maxLinesToScan; tmpLineIndex++)
                {
                    if (IsCommentLine(tmpLineIndex))
                        continue;

                    lineScanCount++;
                    tmpStr = this.LogFileText[tmpLineIndex].ToLower();
                    if (tmpStr.IndexOf("hfd =") != -1)
                    {
                        tmpStr = tmpStr.Substring(tmpStr.IndexOf("=") + 1);
                        double tmpHFD;
                        if (double.TryParse(tmpStr, out tmpHFD))
                        {
                            opDateTime = GetOperationTime(this.LogFileText[tmpLineIndex]);
                            return tmpHFD;
                        }
                    }
                    else if (this.LogFileText[tmpLineIndex].ToLower().IndexOf("auto-focus successful!") != -1)
                        return -1;  // Tell the caller the op failed
                }
            }
            catch
            {
            }
            return -1;
        }

        /// <summary>
        /// Start scanning down the log until we discover if ACP recovered from a guiding failure
        /// </summary>
        /// <param name="lineIndex">The index of the line from which we start scanning</param>
        /// <param name="maxLinesToScan">The max number of lines to look for the op end condition before giving up</param>
        /// <returns>Returns true if ACP carried on imaging unguided after a guider failure, or otherwise</returns>
        private bool FindGuiderFailureRecovery(int lineIndex, int maxLinesToScan)
        {
            try
            {
                int lineScanCount = 0;
                if (maxLinesToScan == -1)
                    maxLinesToScan = int.MaxValue;  // Unlimited

                for (int tmpLineIndex = lineIndex; tmpLineIndex < LogFileText.Count && lineScanCount < maxLinesToScan; tmpLineIndex++)
                {
                    if (IsCommentLine(tmpLineIndex))
                        continue;

                    lineScanCount++;
                    if (this.LogFileText[tmpLineIndex].ToLower().IndexOf("will try image again, this time unguided") != -1 ||
                        this.LogFileText[tmpLineIndex].ToLower().IndexOf("**guiding failed, continuing unguided") != -1)
                        return true;
                }
            }
            catch
            {
            }
            return false;
        }

        /// <summary>
        /// Extract the DateTime from a string. Assumes the datetime stamp starts at position zero in the string
        /// </summary>
        /// <param name="text">The string containing the datetime</param>
        /// <returns>A nullable DateTime object containing the datetime of the operation (which could be null)</returns>
        public DateTime? GetOperationTime(string text)
        {
            DateTime? dt = null;
            try
            {
                string tmpStr = text.Trim().Substring(0, "xx:xx:xx".Length);
                int hrs = int.Parse(tmpStr.Substring(0, 2));
                int mins = int.Parse(tmpStr.Substring(3, 2));
                int secs = int.Parse(tmpStr.Substring(6, 2));

                if (this.StartDate != null)
                    dt = new DateTime(this.StartDate.Value.Year, this.StartDate.Value.Month, this.StartDate.Value.Day, hrs, mins, secs, 0);
                else
                    dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hrs, mins, secs, 0);
            }
            catch
            {
            }

            return dt;
        }

        /// <summary>
        /// Extract the DateTime (as a string) from a string. Assumes the datetime stamp starts at position zero in the string
        /// </summary>
        /// <param name="text">The string containing the datetime</param>
        /// <returns>A string representation of a DateTime object containing the datetime of the operation (which could be null)</returns>
        public string GetOperationTimeText(string text)
        {
            try
            {
                return text.Trim().Substring(0, "xx:xx:xx".Length);
            }
            catch
            {
            }

            return "";
        }

        /// <summary>
        /// Scans the list of log events to see if the event has previously been recorded
        /// </summary>
        /// <param name="startDate">The date of the event</param>
        /// <param name="eventType">The type of event</param>
        /// <returns></returns>
        private bool HasLogEventBeenCaptured(DateTime? startDate, LogEventType eventType)
        {
            if (startDate == null)
                return false;

            var queryResults =
                from le in this.LogEvents
                where (le.EventType == eventType && 
                       le.StartDate != null && 
                       le.StartDate.Value.CompareTo(startDate.Value) == 0)
                select le;

            if (queryResults.Count() > 0)
                return true;
            else
                return false;

            // Replaced the following with the above LINQ 
            // ------------------------------------------
            //foreach (LogEvent le in this.LogEvents)
            //{
            //    if (le.EventType == eventType && le.StartDate != null && le.StartDate.Value.CompareTo(startDate.Value) == 0)
            //        return true;
            //}
            //return false;
            // ------------------------------------------
        }
        #endregion
    }
}
