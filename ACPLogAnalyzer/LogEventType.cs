namespace ACPLogAnalyzer
{
    /// <summary>
    /// An ACP log event (action) type
    /// </summary>
    public enum LogEventType
    {
        AllSkySolveSuccess,         // All-sky plate solve success count
        AllSkySolveFail,            // All-sky plate solve failure count
        AllSkySolveTime,            // All-sky plate solve time
        AutoFocus,                  // Auto-focus
        AutoFocusSuccess,           // Auto-focus success count
        AutoFocusFail,              // Auto-focus failed count
        Exposure,                   // An imaging exposure (not a pointing update exposure)
        FilterChange,               // Filter change
        Fwhm,                       // FWHM measurement
        GuiderFail,                 // Guider fialure where ACP continued imaging unguided
        GuiderSettle,               // Guider settle time
        GuiderStartUp,              // Guider startup time
        Hfd,                        // HFD measurement
        None,                       // No event
        PlateSolveFail,             // Plate solve failure count
        PlateSolveSuccess,          // Plate solve success count
        PointingErrorCenterSlew,    // Pointing error measurement for a center slew
        PointingErrorObjectSlew,    // Pointing error measurement for a target object slew
        PointingExpAndPlateSolve,   // Pointing update exposure (including successful plate solve)
        ScriptAbort,                // Script abort count
        ScriptError,                // Script errors count
        SlewTarget,                 // Slew time to a target
        Target,                     // A new imaging target
        Unknown,                    // An unknown event
        Wait                        // Waiting times (e.g. wait for darkness, etc.)
    }
}