# ACPLogAnalyzer
## Log analyzer tool for ACP

![](./Help/_images/acplaHead.jpg)

The ACP Log Analyzer app provides a quick and easy mechanism for generating an informational report on your
ACP-based observing activities. This can be for the previous evening, the last month, or as long as you've been using ACP
(and have the log files related to your observing sessions).

ACP Log Analyzer makes it easy to find out, for example, what your average pointing error is when slewing to objects,
what your average FWHM is, how many plate solves failed, how many times an auto-focus run succeeded/failed, etc. This can
be done for a single log or for many hundreds, and you can view summarized information about each log and/or overall
results for the entire set of log files. And ACP Log Analyzer works quickly too, normally completing in a few seconds or
less, even when parsing several hundred logs.

<h3>Will it damage my logs?!</h3>
<div>
    <p>
        No, ACP Log Analyzer simply <em>reads</em> the contents of one or more of your ACP logs (which are just text files with a .log extension),
        analyzes the text and outputs a summary of the information found. At no time is the contents of any log file altered.
    </p>
    <p></p>
</div>
<hr />

## How to install ACP Log Analyzer
ACP Log Analyzer is a Microsoft Windows application and uses a standard Windows installer to install the application.
Simply download the latest version of the installer (ACPLogAnalyzerSetup.msi) from **Help/\_files** and save it to a temporary
location on your hard disk. Double-click the installer and follow the on-screen instructions.

![](./Help/_images/la03.png)

When installation is complete, a new folder containing a link to launch the ACP Log Analyzer will have been created in your
**Start | All Programs</em> menu**:

![](./Help/_images/la04.png)

## How to remove ACP Log Analyzer
To remove the application, use the Windows Control Panel (e.g. Programs & Features in Windows 7) to uninstall it.

![](./Help/_images/la02.png)

## How to use ACP Log Analyzer
When you first run the application you'll see the following:

![](./Help/_images/la01.png)    

The first thing to do is tell ACP Log Analyzer which logs to work on. This can be done in a number of ways.
The first way is to **Drag & Drop** one or more files and/or folders from Windows Explorer into the **Logs** list:

![](./Help/_images/la05.png)

Another alternative is to click the **Add Single Log** button. This allows you to select a single log file.

You may also click the **Add All Logs In Path** button. In this case, ACP Log Analyzer will search the folder
structure specified in the path bar for ACP logs.

![](./Help/_images/la06.png)

Note that whatever method you choose, all logs are briefly examined to determine if they are valid ACP logs (lots of applications
produce logs with a .log extension). Non-ACP logs are rejected.

The contents of a log may be viewed in your default text editor (e.g. Notepad.exe) by double-clicking either the log contents
window, or, by double-clicking any item in the list of logs.

If required, you may remove a log from the list by right-clicking it, and then selecting **Remove log from list**
from the popup menu:

![](./Help/_images/la24.png)

## Generating a report
You are now ready to create a report. Simply select (click on) a log in the list or click the **Generate Report On All Logs**
button. Whatever method you choose, the report appears in the right-most panel in ACP Log Analyzer:

![](./Help/_images/la07.png)

The contents of a report may be saved to disk by clicking the **Save Report** button. You may also view it in your default
text editor by double-clicking on the report.

## Report configuration details
The content of a report may be modified by clicking the report **Configure** button:

![](./Help/_images/la23.png)

The **Configure Report** window allows you to control which items appear in report output:

![](./Help/_images/la08.png)

The following describes each report property in detail.

### Report Summary Properties

| Report Property                                   | Description                                                                                           |
| Show summary                                      | Shows the report summary if checked, otherwise the summary is not displayed                           |
| Show summary at bottom of the page                | Shows the summary at the bottom of the report if checked, otherwise the summary will be at the top    |
| Total number of logs parsed                       | Shows the total number of valid ACP logs parsed. Logs are inspected when they are added to the log
                                                      list to confirm they are valid ACP logs (lots of applications use the .log file extension)            |
| Total number of unique targets                    | The overall (for all logs) count of unique observing targets                                          |
| Total number of images taken                      | The overall count of successfully completed exposures. This number does not include pointing
                                                      exposures, auto-focus exposures, etc.                                                                 |
| Overall runtime breakdown                         | Provides an overall time-based breakdown of observing activities. This includes **imaging time**
                                                      (the time spent actively taking exposures), **wait time** (time spent waiting for certain conditions
                                                      , e.g. plan wait statements such as #WAITFOR, #WAITUNTIL, etc.), **observatory overhead** (all other
                                                      time not included in the other categories), **imaging as % of runtime** (shows the percentage of time
                                                      spent usefully taking images)                                                                         |
| Sort report output by date                        | Sorts report output by date if checked                                                                |
| Sort report output by date in ascending order     | Sorts the output in ascending order if checked, otherwise output is sorted in descending order        |
| Total number of successful auto-focus runs        | The overall count of successfully completed auto-focus runs                                           |
| Total number of unsuccessful auto-focus runs      | The overall count of auto-focus runs which did not complete successfully                              |
| Total number of successful plate solves           | The overall count of successfully completed plate solve operations                                    |
| Total number of unsuccessful plate solves         | The overall count of failed plate solve operations                                                    |
| Overall guider failure/unguided imaging count     | The overall count of failed auto-guiding operations                                                   |
| Overall successful all-sky plate solve count      | The overall count of successful all-sky plate solve operations (new in 1.32, ACP 7)                   |
| Overall unsuccessful all-sky plate solve count    | The overall count of unsuccessful all-sky plate solve operations (new in 1.32, ACP 7)                 |
| Overall guider failure/unguided imaging brkdn     | Show a breakdown of the log, target and time when the guiding failed (and imaging of a target
                                                      (not an auto-focus target) continued unguided)                                                        |
| Overall average FWHM                              | The overall average FWHM for all target images taken in all logs. FWHM measurements only include
                                                      data for successful plate-solves on imaging targets (e.g. pointing update FWHM's are ignored)         |
| Overall average HFD                               | The overall average HFD as reported by FocusMax for all target images taken in all logs.
                                                      Only applies to successful AF runs                                                                    |
| Overall average pointing error (object slew)      | The overall average pointing error following a slew to a target. This includes imaging targets,
                                                      auto-focus targets, returns from auto-focusing, etc.                                                  |
| Overall average pointing error (center slew)      | The overall average pointing error following a slew to center an object in the FoV                    |
| Overall average auto-focus time                   | The overall average time taken to successfully complete an auto-focus (failed attempts are ignored).
                                                      This does not include slew time to target stars. This value is a measure of the time from when ACP
                                                      hands control to FocusMax until control passes back to ACP                                            |
| Overall average guider start-up time              | The overall average time taken for the guider to successfully start (failed starts are ignored)       |
| Overall average guider settle time                | The overall average time taken for the guider to successfully settle (failed attempts are ignored)    |
| Overall average filter change time                | The overall average time taken for filter changes (the results for all filters are combined)          |
| Overall average pointing exp/plate solve time     | The overall average time taken to successfully complete the process to take a pointing exposure and
                                                      solve the resulting image (failed attempts are ignored)                                               |
| Overall average slew time (targets)               | The overall average time taken to slew to observing targets (all other types of slews are ignored)    |
| Overall average all-sky plate solve time          | The overall average time taken to successfully complete all-sky plate solves <em>(new in 1.32, ACP 7) |

### Per-Target Details Properties

| Report Property                                   | Description                                                                                           |
| Show target details                               | Displays target details if checked, otherwise all target details are not displayed (although the
                                                      details of the target are included in other statistics and calculations)                              |
| Ignore targets with zero completed exposures      | If checked, targets where no images have been successfully taken are ignored (not displayed)          |
| Show target name                                  | Shows the target's name if checked                                                                    |
| Completed exposure details                        | Displays a breakdown of successfully completed exposures, including imaging time, number of images
                                                      for each filter and binning levels                                                                    |
| Total imaging time                                | Displays the total time spent imaging this target                                                     |
| Average auto-focus time                           | Displays the average time spent auto-focusing on this target on this target                           |
| Average FWHM                                      | Displays the average FWHM for this target.  FWHM measurements only include data for successful
                                                      plate-solves on imaging targets (e.g. pointing update FWHM's are ignored)                             |
| Average HFD                                       | Displays the average HFD as reported by FocusMax for all target images in the selected log.
                                                      Only applies to successful AF runs                                                                    |
| Successful plate solves                           | Displays the total number of successful plate solves for this target                                  |
| Unsuccessful plate solves                         | Displays the total number of unsuccessful plate solves for this target                                |
| Successful all-sky plate solves                   | Displays the total count of successful all-sky plate solve operations for this target
                                                      (new in 1.32, ACP 7)                                                                                  |
| Unsuccessful all-sky plate solves                 | Displays the total count of unsuccessful all-sky plate solve operations for this target
                                                      (new in 1.32, ACP 7)                                                                                  |
| Average pointing error (object slew)              | Displays the average pointing error following a slew to a target. A 'target' is defined as an actual
                                                      imaging target, an auto-focus star and returns to an imaging target from auto-focusing                |
| Average pointing error (center slew)              | Displays the average pointing error following a slew to center this target in the FoV                 |
| Successful auto-focus	                            | Displays a count of the number of successful auto-focus runs for this target                          |
| Unsuccessful auto-focus                           | Displays a count of the number of unsuccessful auto-focus runs for this target                        |
| Guider failure/unguided imaging count	            | Displays a count of the number of times guiding failed (and an exposure proceeded unguided) for
                                                      this target                                                                                           |
| Guider failure/unguided imaging breakdown	        | Shows a breakdown of when guiding failed for this target                                              |
| Average guider start-up time                      | Displays the average time taken for the guider to successfully start while imaging this target
                                                      (failed starts are ignored)                                                                           |
| Average guider settle time                        | Displays the average time taken for the guider to settle while imaging this target
                                                      (failed starts are ignored)                                                                           |
| Average filter change time                        | Displays the average time taken for filter changes while imaging this target (the results for all
                                                      filters are combined)                                                                                 |
| Average pointing exposure/plate solve time        | Displays the average time taken to successfully complete the process to take a pointing exposure
                                                      for this target and solve the resulting image (failed attempts are ignored)                           |
| Average slew time (targets)                       | Displays the average time taken to slew to this target (all other types of slews are ignored)         |
| Average all-sky plate solve time                  | Displays the average time taken to successfully complete all-sky plate solves (new in 1.32, ACP 7)    |

### Log Details Properties

| Report Property                                   | Description                                                                                           |
| Ignore logs with zero targets                     | If checked, logs with no targets are ignored (they are not displayed and all their properties
                                                      are excluded from other statistics and calculations)                                                  |
| Runtime breakdown                                 | Shows a time-based breakdown of log activity if checked, otherwise the breakdown is not displayed     |
| Script errors                                     | Shows a count of script errors if checked                                                             |
| Script aborts                                     | Shows a count of scripts that were aborted by the user if checked                                     |
| Log filename                                      | Displays the log's filename (excluding the path) if checked                                           |
| Log start date                                    | Displays the log's start date/time if checked                                                         |
| Log end date                                      | Displays the log's end date/time if checked                                                           |

## Generating a graph
Log Analyzer allows you to generate 22 different types of graph that plot data points for key observing activities.
Simply select the required graph type and click **Plot**:
