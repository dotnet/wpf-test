// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: This is a set of helper functions that could be used by hosting
//              tests.  This has helper functions that could be used by tests that
//              launch standalone and browser hosted .deploy, .xps, .xaml.

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Automation;
using Microsoft.Win32;

// Class with helper methods for Hosting DRTs.
namespace HostingUtilities
{
    class HostingHelper
    {
        #region Public Methods

        public static void PreventIEFirstRunUI()
        {
            Version ieVersion = new Version(6, 0);
            Version.TryParse((string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Internet Explorer", "Version", null), out ieVersion);

            switch (ieVersion.Major)
            {
                case 6:
                    // Do nothing: no first run UI for local files on IE6.
                    break;
                case 7:
                    Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main", "RunOnceHasShown", 1);
                    Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main", "RunOnceComplete", 1);
                    Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main", "DisableFirstRunCustomize", 1);
                    break;
                case 8:
                    Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main", "IE8RunOnceLastShown", 1);
                    Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main", "IE8RunOnceCompleted", 1);
                    Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main", "DisableFirstRunCustomize", 1);
                    break;
                // As of Win8 build 7786, IE9 honors the same registry keys.
                case 9:
                    goto case 8;
                default:
                    // do nothing
                    break;
            }
        }

        // Run the specified test.  All hosting tests are expected to throw
        // ApplicationException on failure.  That's how failures are identified.
        public static int RunTest(RunTestDelegate rtd, string name, string owner, string[] args)
        {
            ApplicationException appex = null;
            _logFileWriter = File.CreateText(name + LOGEXT);

            Output(/* verboseonly = */ false, "Running {0} -- Owner: {1}", name, owner);

            try
            {
                // Parse the command line arguments for options generic to all hosting DRTs.
                ParseCommandLine(args);

                // Handle razzle specific stuff.
                HandleRazzle();

                // Run the test.
                rtd();
            }
            catch (ApplicationException exparam)
            {
                // Expect an ApplicationException to be thrown for all failures.
                appex = exparam;
            }

            // Write success or failure information to the console.
            if (appex != null)
            {
                Output(/* verboseonly = */ false, appex.ToString());
                Output(/* verboseonly = */ false, "{0} Test failed.", name);
            }
            else
            {
                Output(/* verboseonly = */ false, "{0} Test passed.", name);
            }

            // Return 0 on success and 1 on failure.
            return (appex == null ? 0 : 1);
        }

        // Parse the command line arguments.
        public static void ParseCommandLine(string[] args)
        {
            if (args == null)
            {
                return;
            }

            foreach (string arg in args)
            {
                string curarg = arg.ToLower();
                switch (curarg)
                {
                    case "/verbose":
                    case "-verbose":
                        _verbose = true;
                        break;
                    case "/runinrazzle":
                    case "-runinrazzle":
                        _runinrazzle = true;
                        break;
                    case "/skipproccount":
                    case "-skipproccount":
                        _skipProcCount = true;
                        break;
                    default:
                        break;
                }
            }
        }

        // This'll check if you're running in the razzle environment.  If so
        // it'll check to see if the environment variables are set for the local
        // gac to be used.  If not, it'll set the variables and re-run the exe
        // and wait for the process to exit and then exit with the return value
        // of the process.
        public static void HandleRazzle()
        {
            // Return if you're not running inside razzle.
            string raztoolpath = Environment.GetEnvironmentVariable("RazzleToolPath");
            if (raztoolpath == null)
            {
                return;
            }

            // If runinrazzle command line parameter is not passed, then exit.
            if (_runinrazzle == false)
            {
                throw new ApplicationException("This test doesn't run inside razzle.  " +
                                                "You should run this test outside razzle.  " +
                                                "You could force the test to run inside razzle by passing '/runinrazzle'.  " +
                                                "But this is not a supported option, your mileage will vary!");
            }
        }

        // Return the #of processes running with the specified name.
        public static int GetCountOfProcesses(string processName)
        {
            Process[] procArray = Process.GetProcessesByName(processName);
            return procArray.Length;
        }

        public static bool VerifyProcessCount(string processName, int numExpectedPHProc, bool throwOnFailure)
        {
            int numPHProcNow = 0;

            if (SkipProcCount)
            {
                Output(false, "Skipping process count validation for " + processName);
                return true;
            }

            Output(/* verboseonly = */ false, "Validating the process count for '{0}'.  Max of 90 seconds.", processName);
            for (int retryCount = 0; retryCount < 90; retryCount++)
            {
                Output(/* verboseonly = */ false, "Sleeping for one second waiting for process count to stabilize.");
                Thread.Sleep(1000);

                numPHProcNow = GetCountOfProcesses(processName);
                if (numPHProcNow == numExpectedPHProc)
                {
                    break;
                }
            }

            if (numPHProcNow != numExpectedPHProc && throwOnFailure)
            {
                throw new ApplicationException(String.Format("# of '{0}' processes - expected = {1}, actual = {2}.",
                                                             processName, numExpectedPHProc, numPHProcNow));
            }

            return (numPHProcNow == numExpectedPHProc);
        }

        // This function does:
        //  - shuts down process if it has been initialized.
        public static bool CloseProcess(Process process)
        {
            bool returnCode = false;

            if (process != null)
            {
                if (!process.HasExited)
                {
                    process.CloseMainWindow();

                    // verify that process did close
                    Output(false /* verboseonly */,
                           "Waiting for Process to shutdown: {0}.    Max of 90 seconds.",
                           process.Id);
                    for (int i = 0; i < 45; i++)
                    {
                        Thread.Sleep(2000);
                        if (process.HasExited) break;
                        Output(false /* verboseonly */, "Waiting for Process to shutdown");
                    }
                }
                returnCode = true;
            }

            return returnCode;
        }

// SEE Drts also include this file but those Drts have their own version of Input which
// clashes with the DrtBase version.  Don't compile this code with SEE Drts.
#if !SEEDRT
        // This function with find the window with specific title and click the specific button.
        public static void FindDialogAndClick(string title, string button)
        {
            IntPtr hwnd = IntPtr.Zero;

            Output(false /* verboseonly */, "Waiting for the dialog with title '{0}'.", title);
            for (int i = 0; i < 45; i++)
            {
                Output(false /* verboseonly */, "Waiting for the dialog with title '{0}'.", title);
                Thread.Sleep(2000);
                hwnd = FindWindow(null, title);
                if (hwnd != IntPtr.Zero) break;
            }

            if (hwnd == IntPtr.Zero)
            {
                throw new ApplicationException(String.Format("Failed to find dialog with title '{0}'.", title));
            }

            AutomationElement dialog = null;
            AutomationElement buttonToClick = null;
            dialog = AutomationElement.FromHandle(hwnd);
            buttonToClick = GetLogicalElementWithName(button, dialog);
            DRT.Input.MoveToAndClick(buttonToClick);
        }
#endif

        // This function does:
        //  - ShellExecute the specified filename.
        //  - Get the window with the specified title.
        public static AutomationElement RunTestAndGetLogicalElementForWindow(string filename,
                                                                             string arguments,
                                                                             string windowTitle,
                                                                             bool allowPartialMatch)
        {
            Process process = null;
            return RunTestAndGetLogicalElementForWindow(filename, arguments, windowTitle, allowPartialMatch, ref process, null);
        }

        // This function does:
        //  - ShellExecute the specified filename.
        //  - Get the window with the specified title.
        //  - pass out process ref if it is non null
        public static AutomationElement RunTestAndGetLogicalElementForWindow(string filename,
                                                                             string arguments,
                                                                             string windowTitle,
                                                                             bool allowPartialMatch,
                                                                             ref Process process)
        {
            return RunTestAndGetLogicalElementForWindow(filename, arguments, windowTitle, allowPartialMatch, ref process, null);
        }

        // This function does:
        //  - ShellExecute the specified filename.
        //  - Get the window with the specified title.
        //  - pass out process ref if it is non null
        public static AutomationElement RunTestAndGetLogicalElementForWindow(ProcessStartInfo startInfo,
                                                                             String windowTitle,
                                                                             bool allowPartialMatch,
                                                                             ref Process process)
        {
            return RunTestAndGetLogicalElementForWindow(String.Empty, String.Empty, windowTitle, allowPartialMatch,
                ref process, startInfo);
        }

        // This function does:
        //  - ShellExecute the specified filename.
        //  - Get the window with the specified title.
        //  - pass out process ref if it is non null
        //  - if startInfo!=null it will be used to start the process, otherwise filename and arguments will be used.
        private static AutomationElement RunTestAndGetLogicalElementForWindow(string filename,
                                                                             string arguments,
                                                                             string windowTitle,
                                                                             bool allowPartialMatch,
                                                                             ref Process process,
                                                                             ProcessStartInfo startInfo)
        {
            // If StartInfo provided start process with it, otherwise use filename / arguments.
            if (startInfo != null)
            {
                process = Process.Start(startInfo);
            }
            else
            {
                process = Process.Start(filename, arguments);
            }

            // Handle TrustManager dialog - CLR has modified ClickOnce to prompt for Local, partial-trust apps
            AutomationElement trustManagerWindow = FindFirstWithTimeout(AutomationElement.RootElement, TreeScope.Children,
                new PropertyCondition(AutomationElement.AutomationIdProperty, "TrustManagerPromptUI"), 15);

            if (trustManagerWindow == null)
            {
                Output(/* verboseonly = */ false,
                  "Couldn't find TrustManager dialog after waiting 15 seconds! Will attempt to continue but this may indicate the test is broken"); 
            }
            else
            {
                Output(/* verboseonly = */ false,  "Found TrustManager dialog, dismissing it"); 
                //try to click the run button with two identities then error out
                if (tryInvokeByAutomationId(trustManagerWindow, "btnInstall") != true)
                {
                    Output(/* verboseonly = */ false, "Couldn't invoke trust manager dialog \"Run\" button!  Attempting to continue"); 
                }
            }

            // Get the window with the specified title as part of it by searching
            // from Desktop one level deep.
            return FindElementWithPropertyValue(AutomationElement.RootElement,
                                                AutomationElement.NameProperty,
                                                windowTitle,
                                                1 /* levels */,
                                                allowPartialMatch);
        }

        // This function does:
        //  - ShellExecute the specified filename.
        //  - Get the HWND for the window with the specified title.
        public static IntPtr RunTestAndGetHwndForWindow(string filename,
                                                        string arguments,
                                                        string windowTitle)
        {
            // Run the specified process.
            Process proc = Process.Start(filename, arguments);

            // Get the window for the process.
            return GetHwndForWindowWithTitle(windowTitle);
        }


        // Finds the AutomationElement with the specified id property value.
        public static AutomationElement GetLogicalElementWithId(string idString)
        {
            return GetLogicalElementWithId(idString, AutomationElement.RootElement);
        }

        // Finds the AutomationElement with the specified id property value from specified root down.
        public static AutomationElement GetLogicalElementWithId(string idString, AutomationElement root)
        {
            // Create Condition for the property.
            PropertyCondition elementCondition =
                new PropertyCondition(AutomationElement.AutomationIdProperty, idString);

            Output(/* verboseonly = */ false,
                   "Waiting for AutomationElement with Id '{0}' to appear.  Max of 90 seconds.", idString);
            return GetLogicalElementWithCondition(elementCondition, root);
        }

        // Finds the AutomationElement with the specified Name property value.
        public static AutomationElement GetLogicalElementWithName(string nameString)
        {
            return GetLogicalElementWithName(nameString, AutomationElement.RootElement);
        }

        // Finds the AutomationElement with the specified Name property value from the specified root down.
        public static AutomationElement GetLogicalElementWithName(string nameString, AutomationElement root)
        {
            // Create MatchCondition for the property.
            PropertyCondition elementCondition =
                new PropertyCondition(AutomationElement.NameProperty, nameString);

            Output(/* verboseonly = */ false,
                   "Waiting for AutomationElement with name '{0}' to appear.  Max of 90 seconds.", nameString);
            return GetLogicalElementWithCondition(elementCondition, root);
        }

        // Get the invoke pattern and call Invoke().
        public static void ClickButton(AutomationElement leButton)
        {
            InvokePattern ip = leButton.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            if (ip == null)
            {
                throw new ApplicationException("Failed to get InvokePattern from Button element.");
            }
            ip.Invoke();
        }

        // Walk up the hierarchy till you find a window that supports the WindowPattern
        // and call the Close method on the pattern.
        public static void CloseWindow(AutomationElement leChild)
        {
            AutomationElement le = GetWindowPatternElement(leChild);
            WindowPattern wp = le.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;
            Debug.Assert(wp != null, "wp cannot be null here");
            wp.Close();
        }

        public static AutomationElement GetWindowPatternElement(AutomationElement leChild)
        {
            AutomationElement returnVal = null;

            while (leChild != null && returnVal == null)
            {
                object outobj;
                if (leChild.TryGetCurrentPattern(WindowPattern.Pattern, out outobj))
                {
                    returnVal = leChild;
                }
                leChild = TreeWalker.ControlViewWalker.GetParent(leChild);
            }

            if (returnVal == null)
            {
                throw new ApplicationException("Could not find WindowPattern in the AutomationElement hierarchy.");
            }
            return returnVal;
        }

        // Build an array from a given string.
        public static string GetStringFromArray(string[] array)
        {
            string returnString = null;

            if (array == null)
            {
                return returnString;
            }

            for (int i = 0; i < array.Length; i++)
            {
                if (i != 0)
                {
                    returnString += ", ";
                }

                returnString += array[i];
            }

            return returnString;
        }

        // Outputs the information to Console and the log file.
        //  verboseonly  - if true, write to console only in verbose mode.
        //                 if false, always write to console.
        //  format, objs - passed directly to WriteLine method.
        // This function will always write to the log file.
        public static void Output(bool verboseonly, string format, params object[] objs)
        {
            // Write to console as appropriate.
            bool write = verboseonly ? _verbose : true;
            if (write)
            {
                Console.WriteLine(format, objs);
            }

            // Always write to the log file.
            _logFileWriter.WriteLine(format, objs);
            _logFileWriter.Flush();
        }

        public static void AppendFileContentToLog(string filepath)
        {
            if (File.Exists(filepath))
            {
                string appLogContent = File.ReadAllText(filepath);
                _logFileWriter.Write(appLogContent);
                _logFileWriter.Flush();
            }
        }

        public static void CloseLog()
        {
            _logFileWriter.Close();
        }

        // Run the process and wait for it to exit.
        public static void RunProcessAndWaitForExit(string fileName, string arguments, bool throwOnFailure)
        {
            // Run the specified process and give it a generous timeout for slow
            // machines.  Five minutes should be generous enough hopefully!
            Process proc = Process.Start(fileName, arguments);

            // Process.Start() returns null for the case where a process may be reused.
            // This is the case for .deploy.  Do not wait for such cases.
            if (proc != null)
            {
                bool exited = proc.WaitForExit(300000);

                if (!exited)
                {
                    throw new ApplicationException(String.Format("'{0} {1}' timed out.", fileName, arguments));
                }

                if (throwOnFailure && (proc.ExitCode != 0))
                {
                    throw new ApplicationException(String.Format("'{0} {1}' completed with non-zero exit code.",
                                                                 fileName, arguments));
                }
            }
        }

        // Kill the deployment framework service process.
        public static void KillDeploymentService()
        {
            Process[] processes = Process.GetProcessesByName(DEPLOYMENT_FRAMEWORK_SERVICE_NAME);

            if (processes != null)
            {
                foreach (Process p in processes)
                {
                    Output(true /* verboseonly */, "Killing '{0}' process.", DEPLOYMENT_FRAMEWORK_SERVICE_NAME);
                    p.Kill();
                }
            }

            // Make sure that are no deployment service processes still around.
            VerifyProcessCount(DEPLOYMENT_FRAMEWORK_SERVICE_NAME, 0, true /* throwOnFailure */);
        }

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string className, string windowName);

        [DllImport("user32.dll", EntryPoint = "EndTask")]
        public static extern bool EndHwndTask(IntPtr hwnd, bool fShutDown, bool fForce);

        // This function will walk the tree starting at the root and try to find
        // an element that has the specified value for the specified property.
        public static AutomationElement FindElementWithPropertyValue(AutomationElement root,
                                                                      AutomationProperty property,
                                                                      object value,
                                                                      int maxDepth,
                                                                      bool allowPartialMatch)
        {
            AutomationElement outelement = null;

            for (int i = 0; i < 45; i++)
            {
                Output(false, "Waiting for element with {0}={1}.", property.ProgrammaticName, value);
                Thread.Sleep(2000);

                if (FindElementWithPropertyValueImpl(root, property, value, maxDepth,
                                                      allowPartialMatch, 0 /* currentDepth */,
                                                      ref outelement))
                {
                    break;
                }
            }

            if (outelement == null)
            {
                throw new ApplicationException(String.Format("Could not find element with {0}={1}.", property, value));
            }

            return outelement;
        }

        public static bool CheckElementWithPropertyValueExists(AutomationElement root,
                                                                            AutomationProperty property,
                                                                            object value,
                                                                            int maxDepth,
                                                                            bool allowPartialMatch)
        {
            AutomationElement outElement = null;
            FindElementWithPropertyValueImpl(root, property, value, maxDepth, allowPartialMatch, 0, ref outElement);
            return outElement != null;
        }

        public static bool tryInvokeByAutomationId(AutomationElement window, string name)
        {
            PropertyCondition itemIdCondition = new PropertyCondition(AutomationElement.AutomationIdProperty, name);
            return tryInvokeByPropertyCondition(itemIdCondition, window);
        }

        public static AutomationElement FindFirstWithTimeout(AutomationElement root, TreeScope scope, Condition condition, int timeoutSeconds)
        {
            AutomationElement whatIFound = null;
            while ((timeoutSeconds > 0) && (whatIFound == null))
            {
                whatIFound = root.FindFirst(scope, condition);
                if (whatIFound == null)
                {
                    Thread.Sleep(1000);
                    timeoutSeconds--;
                }
            }
            return whatIFound;
        }

        // Creates the specifed file in temp directory.  This could lose the content of
        // the file.  This throws an exception on failure to create a file.
        public static void CreateFileInTempDirectory(string fileName)
        {
            string filePath = GetFullPathWithTempDirectory(fileName);
            FileStream fs = File.Create(filePath);

            if (!File.Exists(filePath))
            {
                throw new ApplicationException(String.Format("Failed to create file '{0}'.", filePath));
            }

            fs.Close();
        }

        // Verify that the specified file in the temp directory is deleted.  It repeats the
        // validation in a loop to give the app time to finish on a slow machine.  Throws
        // exception if file still exists.
        public static void VerifyFileInTempDeleted(string fileName)
        {
            string filePath = GetFullPathWithTempDirectory(fileName);

            Output(/* verboseonly = */ false, "Validating that the file '{0}' is deleted.", filePath);
            for (int i = 0; i < 45; i++)
            {
                // Sleep for two seconds.
                Thread.Sleep(2000);
                Output(/* verboseonly = */ true, "Sleeping for 2 seconds waiting for file '{0}' to be deleted.", filePath);
                if (!File.Exists(filePath))
                {
                    break;
                }
            }

            if (File.Exists(filePath))
            {
                throw new ApplicationException(String.Format("File '{0}' still exists.", filePath));
            }
        }

        public static AutomationElement GetIEMenu(AutomationElement ieWindow, out AutomationElement fileMenu)
        {
            if (ieWindow == null)
            {
                HostingHelper.Output(false, "ieWindow cannot be null when checking for menus");
                fileMenu = null;
                return null;
            }

            AutomationElement ieMenu = null;
            AutomationElement tempFileMenu = null;

            /* In internet explorer the main menu is not a standard win32 MENU (even though
             * a standard menu is what we get when doing menu merging).  It is instead a
             * "ToolbarWindow32".  Calling GetMenu() on the ie frame will return null. There are
             * 3 "ToolbarWindow32"s by default on windows xp ie6.
             */
            // enumerate all ToolbarWindow32s
            AutomationElementCollection toolbarCollection = ieWindow.FindAll(
                TreeScope.Descendants | TreeScope.Element,
                new PropertyCondition(AutomationElement.ClassNameProperty, "ToolbarWindow32"));

            PropertyCondition fileMenuCondition = new PropertyCondition(
                AutomationElement.AutomationIdProperty, SYSTEM_FILE_MENU);

            if (toolbarCollection != null)
            {
                // find the ToolbarWindow32 that contains the "File" menu.  The file menu is 
                // universally (IE and explorer) defined as menu item 32768.
                foreach (AutomationElement el in toolbarCollection)
                {
                    if ( (tempFileMenu = el.FindFirst(TreeScope.Descendants | TreeScope.Element, fileMenuCondition)) != null)
                    {
                        ieMenu = el;
                        break;
                    }
                }
            }

            fileMenu = tempFileMenu;
            return ieMenu;
        }

        public delegate bool ExitConditionPredicate();

        public static void DoPollingTest(ExitConditionPredicate exitCondition, int retries, int retryDelayMs, string errMsg)
        {
            for (int i = 0; i <= retries; i++)
            {
                if(exitCondition())
                    return;
                Thread.Sleep(retryDelayMs);
            }
            throw new ApplicationException(errMsg);
        }

        #endregion Public Methods

        #region Public Properties

        public static bool Verbose
        {
            get
            {
                return _verbose;
            }
            set
            {
                _verbose = value;
            }
        }

        public static bool SkipProcCount
        {
            get
            {
                return _skipProcCount;
            }
            set
            {
                _skipProcCount = value;
            }
        }
        #endregion

        #region Public delegates

        public delegate void RunTestDelegate();

        #endregion Public delegates

        #region Private Methods

        // Returns full path with temp directory given file name.
        private static string GetFullPathWithTempDirectory(string fileName)
        {
            string tempPath = Environment.GetEnvironmentVariable(TEMP);
            if (tempPath == null || tempPath == String.Empty)
            {
                throw new ApplicationException(String.Format("Failed to get environment variable - {0}.", TEMP));
            }

            return Path.Combine(tempPath, fileName);
        }

        // Finds the AutomationElement with the specified PropertyCondition from specified root down.
        private static AutomationElement GetLogicalElementWithCondition(
                                                    PropertyCondition elementCondition,
                                                    AutomationElement root)
        {
            AutomationElement le = null;
            for (int retryCount = 0; retryCount < 45; retryCount++)
            {
                try
                {
                    // Try to find the logical element.
                    // Sleep for two seconds.
                    Output(/* verboseonly = */ false, "Sleeping for 2 seconds waiting to find AutomationElement.");
                    Thread.Sleep(2000);
                    le = root.FindFirst(TreeScope.Descendants | TreeScope.Element, elementCondition);
                    if (le != null)
                    {
                        break;
                    }
                }
                catch (ElementNotAvailableException)
                {
                    // This exception can be ignored.
                    Output(/* verboseonly = */ false, "ElementNotAvailableException in GetLogicalElementWithId.");
                }
            }

            return le;
        }

        // This is a recursive implementation for the function that walks the tree
        // to look for an element with specified value for a specified property.
        private static bool FindElementWithPropertyValueImpl(AutomationElement root,
                                                             AutomationProperty property,
                                                             object value,
                                                             int maxDepth,
                                                             bool allowPartialMatch,
                                                             int currentLevel,
                                                             ref AutomationElement outelement)
        {
            // Bail if element is null or you're too far deep.
            if (root != null && currentLevel <= maxDepth)
            {
                // See if we found the element we're looking for.
                if (IsElementWithPropertyValue(root, property, value, allowPartialMatch))
                {
                    outelement = root;
                    return true;
                }

                TreeWalker tw = TreeWalker.ControlViewWalker;

                // Don't go to siblings if you're at level zero.
                if (currentLevel != 0)
                {
                    AutomationElement nextSibling = null;

                    try
                    {
                        nextSibling = tw.GetNextSibling(root);
                    }
                    catch (ElementNotAvailableException)
                    {
                        // This exception can be ignored.
                        Output(/* verboseonly = */ false, "ElementNotAvailableException in FindElementWithPropertyValueImpl.");
                    }

                    if (FindElementWithPropertyValueImpl(nextSibling,
                                                         property, value, maxDepth,
                                                         allowPartialMatch,
                                                         currentLevel, ref outelement))
                    {
                        return true;
                    }
                }

                AutomationElement nextChild = null;
                try
                {
                    nextChild = tw.GetFirstChild(root);
                }
                catch (ElementNotAvailableException)
                {
                    // This exception can be ignored.
                    Output(/* verboseonly = */ true, "ElementNotAvailableException in FindElementWithPropertyValueImpl.");
                }

                if (FindElementWithPropertyValueImpl(nextChild,
                                                     property, value, maxDepth,
                                                     allowPartialMatch,
                                                     currentLevel + 1, ref outelement))
                {
                    return true;
                }
            }

            return false;
        }

        // See if this element has a specified property with specified value.
        private static bool IsElementWithPropertyValue(AutomationElement root,
                                                       AutomationProperty property,
                                                       object value,
                                                       bool allowPartialMatch)
        {
            // Get the value and see if you found it.
            object actualValue = root.GetCurrentPropertyValue(property);
            if (value.Equals(actualValue))
            {
                return true;
            }

            // Assume that the value is a string if allowPartialMatch is true.
            if (allowPartialMatch)
            {
                string expectedString = value as string;
                string actualString = actualValue as string;

                if (actualString == null)
                {
                    return false;
                }

                Debug.Assert(expectedString != null, "value must be string when allowPartialMatch is true.");
                if (actualString.ToLower().IndexOf(expectedString.ToLower()) != -1)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool tryInvokeByPropertyCondition(PropertyCondition cond, AutomationElement window)
        {
            AutomationElement theItem = window.FindFirst(TreeScope.Descendants, cond);
            if (theItem == null)
            {
                return false;
            }
            try
            {
                object patternObject;
                theItem.TryGetCurrentPattern(InvokePattern.Pattern, out patternObject);
                InvokePattern ip = patternObject as InvokePattern;
                ip.Invoke();
            }
            catch (System.Windows.Automation.NoClickablePointException)
            {
                Thread.Sleep(3000);
                object patternObject;
                theItem.TryGetCurrentPattern(InvokePattern.Pattern, out patternObject);
                InvokePattern ip = patternObject as InvokePattern;
                ip.Invoke();
            }
            return true;
        }

        // Get the Hwnd for window with specified title.  Waits in a loop for the
        // window for 90 seconds.
        private static IntPtr GetHwndForWindowWithTitle(string windowTitle)
        {
            IntPtr hwnd = IntPtr.Zero;

            Output(/* verboseonly = */ false, "Waiting for window with title - '{0}'.", windowTitle);
            for (int i = 0; i < 45; i++)
            {
                Output( /*verboseonly=*/ true, "Waiting for window with {0}.", windowTitle);
                Thread.Sleep(2000);
                hwnd = FindWindow(null, windowTitle);
                if (hwnd != IntPtr.Zero)
                {
                    break;
                }
            }

            if (hwnd == IntPtr.Zero)
            {
                throw new ApplicationException(String.Format("Could not find window with title {0}.", windowTitle));
            }

            return hwnd;
        }


        #endregion Private Methods

        #region Private Data

        // Command line related data.
        private static bool _verbose = false;
        private static bool _runinrazzle = false;
        private static bool _skipProcCount = false;

        // Log info.
        private static TextWriter _logFileWriter = null;

        // Readonly data
        private const string README = @"wcp\devtest\drts\hosting\readme.txt";
        public const string LOGEXT = ".log";
        private const string OPENSAVEDIALOGTITLE = "File Download";
        private const string OPENBUTTONNAME = "Open";
        private const string DEPLOYMENT_FRAMEWORK_SERVICE_NAME = "dfsvc";
        private const string TEMP = "TEMP";
        private static readonly string SYSTEM_FILE_MENU = "Item 32768";

        #endregion Private Data
    }
}
