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

The following table describes each report property in detail.

<table>
    <tr><td colspan="2" style="text-align:right; font-size:14px"><strong>Report Summary Properties</strong></td></tr>
    <tr><td width="285px"><strong>Report Property</strong></td>	<td><strong>Description</strong></td></tr>
    <tr><td>Show summary</td>									<td>Shows the report summary if checked, otherwise the summary is not displayed</td></tr>
    <tr><td>Show summary at bottom of the page</td>				<td>Shows the summary at the bottom of the report if checked, otherwise the summary will be at the top</td></tr>
    <tr><td>Total number of logs parsed</td>					<td>Shows the total number of valid ACP logs parsed. Logs are inspected when they are added to the log list to confirm they are valid ACP logs (lots of applications use the .log file extension)</td></tr>
    <tr><td>Total number of unique targets</td>					<td>The overall (for all logs) count of unique observing targets</td></tr>
    <tr><td>Total number of images taken</td>					<td>The overall count of successfully completed exposures. This number does <em>not</em> include pointing exposures, auto-focus exposures, etc.</td></tr>
    <tr><td>Overall runtime breakdown</td>						<td>Provides an overall time-based breakdown of observing activities. This includes <em>imaging time</em> (the time spent actively taking exposures), <em>wait time</em> (time spent waiting for certain conditions, e.g. plan wait statements such as #WAITFOR, #WAITUNTIL, etc.), <em>observatory overhead</em> (all other time not included in the other categories), <em>imaging as % of runtime</em> (shows the percentage of time spent usefully taking images)</td></tr>
    <tr><td>Sort report output by date</td>						<td>Sorts report output by date if checked</td></tr>
    <tr><td>Sort report output by date in ascending order</td>	<td>Sorts the output in ascending order if checked, otherwise output is sorted in descending order</td></tr>
    <tr><td>Total number of successful auto-focus runs</td>		<td>The overall count of successfully completed auto-focus runs</td></tr>
    <tr><td>Total number of unsuccessful auto-focus runs</td>	<td>The overall count of auto-focus runs which did not complete successfully</td></tr>
    <tr><td>Total number of successful plate solves</td>		<td>The overall count of successfully completed plate solve operations</td></tr>
    <tr><td>Total number of unsuccessful plate solves</td>		<td>The overall count of failed plate solve operations</td></tr>
    <tr><td>Overall guider failure/unguided imaging count</td>	<td>The overall count of failed auto-guiding operations</td></tr>
    <tr><td>Overall successful all-sky plate solve count</td>	<td>The overall count of successful all-sky plate solve operations <em>(new in 1.32, ACP 7)</em></td></tr>
    <tr><td>Overall unsuccessful all-sky plate solve count</td>	<td>The overall count of unsuccessful all-sky plate solve operations  <em>(new in 1.32, ACP 7)</em></td></tr>
    <tr>
    <td>Overall guider failure/unguided imaging brkdn</td>		<td>Show a breakdown of the log, target and time when the guiding failed (and imaging of a target (not an auto-focus target) continued unguided)</td></tr>
    <tr><td>Overall average FWHM</td>							<td>The overall average FWHM for all target images taken in all logs.  FWHM measurements only include data for successful plate-solves on imaging targets (e.g. pointing update FWHM's are ignored)</td></tr>
    <tr><td>Overall average HFD</td>							<td>The overall average HFD as reported by FocusMax for all target images taken in all logs. Only applies to successful AF runs</td></tr>
    <tr><td>Overall average pointing error (object slew)</td>	<td>The overall average pointing error following a slew to a target. This includes imaging targets, auto-focus targets, returns from auto-focusing, etc.</td></tr>
    <tr><td>Overall average pointing error (center slew)</td>	<td>The overall average pointing error following a slew to center an object in the FoV</td></tr>
    <tr><td>Overall average auto-focus time</td>				<td>The overall average time taken to successfully complete an auto-focus (failed attempts are ignored). This does not include slew time to target stars. This value is a measure of the time from when ACP hands control to FocusMax until control passes back to ACP</td></tr>
    <tr><td>Overall average guider start-up time</td>			<td>The overall average time taken for the guider to successfully start (failed starts are ignored)</td></tr>
    <tr><td>Overall average guider settle time</td>				<td>The overall average time taken for the guider to successfully settle (failed attempts are ignored)</td></tr>
    <tr><td>Overall average filter change time</td>				<td>The overall average time taken for filter changes (the results for all filters are combined)</td></tr>
    <tr><td>Overall average pointing exp/plate solve time</td>	<td>The overall average time taken to successfully complete the process to take a pointing exposure and solve the resulting image (failed attempts are ignored)</td></tr>
    <tr><td>Overall average slew time (targets)</td>			<td>The overall average time taken to slew to observing targets (all other types of slews are ignored)</td></tr>
    <tr><td>Overall average all-sky plate solve time</td>		<td>The overall average time taken to successfully complete all-sky plate solves <em>(new in 1.32, ACP 7)</em></td></tr>

    <tr><td colspan="2" style="text-align:right; font-size:14px"><strong>Per-Target Details Properties</strong></td></tr>
    <tr><td width="285px"><strong>Report Property</strong></td>	<td><strong>Description</strong></td></tr>
    <tr><td>Show target details</td>							<td>Displays target details if checked, otherwise all target details are not displayed (although the details of the target are included in other statistics and calculations)</td></tr>
    <tr><td>Ignore targets with zero completed exposures</td>	<td>If checked, targets where no images have been successfully taken are ignored (not displayed)</td></tr>
    <tr><td>Show target name</td>								<td>Shows the target's name if checked</td></tr>
    <tr><td>Completed exposure details</td>						<td>Displays a breakdown of successfully completed exposures, including imaging time, number of images for each filter and binning levels</td></tr>
    <tr><td>Total imaging time</td>								<td>Displays the total time spent imaging this target</td></tr>
    <tr><td>Average auto-focus time</td>						<td>Displays the average time spent auto-focusing on this target on this target</td></tr>
    <tr><td>Average FWHM</td>									<td>Displays the average FWHM for this target.  FWHM measurements only include data for successful plate-solves on imaging targets (e.g. pointing update FWHM's are ignored)</td></tr>
    <tr><td>Average HFD</td>									<td>Displays the average HFD as reported by FocusMax for all target images in the selected log. Only applies to successful AF runs</td></tr>
    <tr><td>Successful plate solves</td>						<td>Displays the total number of successful plate solves for this target</td></tr>
    <tr><td>Unsuccessful plate solves</td>						<td>Displays the total number of unsuccessful plate solves for this target</td></tr>
    <tr><td>Successful all-sky plate solves</td>				<td>Displays the total count of successful all-sky plate solve operations for this target <em>(new in 1.32, ACP 7)</em></td></tr>
    <tr><td>Unsuccessful all-sky plate solves</td>				<td>Displays the total count of unsuccessful all-sky plate solve operations for this target <em>(new in 1.32, ACP 7)</em></td></tr>
    <tr><td>Average pointing error (object slew)</td>			<td>Displays the average pointing error following a slew to a target. A 'target' is defined as an actual imaging target, an auto-focus star and returns to an imaging target from auto-focusing.</td></tr>
    <tr><td>Average pointing error (center slew)</td>			<td>Displays the average pointing error following a slew to center this target in the FoV</td></tr>
    <tr><td>Successful auto-focus</td>							<td>Displays a count of the number of successful auto-focus runs for this target</td></tr>
    <tr><td>Unsuccessful auto-focus</td>						<td>Displays a count of the number of unsuccessful auto-focus runs for this target</td></tr>
    <tr><td>Guider failure/unguided imaging count</td>			<td>Displays a count of the number of times guiding failed (and an exposure proceeded unguided) for this target</td></tr>
    <tr><td>Guider failure/unguided imaging breakdown</td>		<td>Shows a breakdown of when guiding failed for this target</td></tr>
    <tr><td>Average guider start-up time</td>					<td>Displays the average time taken for the guider to successfully start while imaging this target (failed starts are ignored)</td></tr>
    <tr><td>Average guider settle time</td>						<td>Displays the average time taken for the guider to settle while imaging this target (failed starts are ignored)</td></tr>
    <tr><td>Average filter change time</td>						<td>Displays the average time taken for filter changes while imaging this target (the results for all filters are combined)</td></tr>
    <tr><td>Average pointing exposure/plate solve time</td>		<td>Displays the average time taken to successfully complete the process to take a pointing exposure for this target and solve the resulting image (failed attempts are ignored)</td></tr>
    <tr><td>Average slew time (targets)</td>					<td>Displays the average time taken to slew to this target (all other types of slews are ignored)</td></tr>
    <tr><td>Average all-sky plate solve time</td>				<td>Displays the average time taken to successfully complete all-sky plate solves <em>(new in 1.32, ACP 7)</em></td></tr>

    <tr><td colspan="2" style="text-align:right; font-size:14px"><strong>Log Details Properties</strong></td></tr>
    <tr><td width="285px"><strong>Report Property</strong></td>	<td><strong>Description</strong></td></tr>
    <tr><td>Ignore logs with zero targets</td>					<td>If checked, logs with no targets are ignored (they are not displayed and all their properties are excluded from other statistics and calculations)</td></tr>
    <tr><td>Runtime breakdown</td>								<td>Shows a time-based breakdown of log activity if checked, otherwise the breakdown is not displayed</td></tr>
    <tr><td>Script errors</td>									<td>Shows a count of script errors if checked</td></tr>
    <tr><td>Script aborts</td>									<td>Shows a count of scripts that were aborted by the user if checked</td></tr>
    <tr><td>Log filename</td>									<td>Displays the log's filename (excluding the path) if checked</td></tr>
    <tr><td>Log start date</td>									<td>Displays the log's start date/time if checked</td></tr>
    <tr><td>Log end date</td>									<td>Displays the log's end date/time if checked</td></tr>
</table>                    

## Generating a graph
