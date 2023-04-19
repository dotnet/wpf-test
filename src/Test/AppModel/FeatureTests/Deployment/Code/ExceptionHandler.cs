// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Resources;
using System.Threading;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;
using System.Windows.Automation;
using MTI = Microsoft.Test.Input;

namespace Microsoft.Test.Deployment
{
    /// <summary>
    /// UIHandler to fail if its HandleWindow method is ever invoked. 
    /// Only use this if the app in question will change its title or process, as it will always fail otherwise.
    /// </summary>
    public class FailIfSeenHandler : UIHandler
    {
        #region Private Members
        /// <summary>
        /// Testlog instance
        /// </summary>
        protected TestLog TestLog;
        #endregion

        #region Public Members
        /// <summary>
        /// Specify this if you want to check for a particular value in the IE StatusBar (and FAIL if seen)
        /// Only tested in IE6/7, so only use for verifying IE UI.
        /// </summary>
        public string StatusBarValue = "";

        #endregion

        /// <summary>
        /// Fails if certain UI is seen.
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(IntPtr topLevelhWnd, IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            // We fail by default in this handler.  Only pass in special cases.
            bool testFailed = true;

            this.TestLog = TestLog.Current;
            if (this.TestLog == null)
            {
                throw new InvalidOperationException("Must be run in the context of a test log");
            }

            if (StatusBarValue != "")
            {
                testFailed = testFailed && VerifyStatusBarContent(topLevelhWnd, StatusBarValue);
            }

            if (testFailed)
            {
                GlobalLog.LogEvidence("Failed: Saw UI of app that should not have displayed.");
                GlobalLog.LogEvidence("Process Name: " + process.ProcessName + " Window Title: " + title);
                this.TestLog.Result = TestResult.Fail;
            }
            else
            {
                GlobalLog.LogEvidence("Passed: Never saw UI of app that should not have displayed.");
                if (StatusBarValue != "")
                {
                    GlobalLog.LogEvidence("Never saw value: " + StatusBarValue + " in the status bar");
                }
                GlobalLog.LogEvidence("Process Name: " + process.ProcessName + " Window Title: " + title);
                this.TestLog.Result = TestResult.Pass;
            }

            // Get the highest level window (may be same as current) and close it
            AutomationElement windowToClose = AutomationElement.FromHandle(topLevelhWnd);
            object patternObject;
            windowToClose.TryGetCurrentPattern(WindowPattern.Pattern, out patternObject);
            WindowPattern wp = patternObject as WindowPattern;
            wp.Close();
            GlobalLog.LogDebug("Closed main window");

            return UIHandlerAction.Abort;
        }

        private bool VerifyStatusBarContent(IntPtr IEWindow, string expectedStatusContent)
        {
            AutomationElement theWindow;
            try
            {
                theWindow = AutomationElement.FromHandle(IEWindow);
            }
            catch (System.Windows.Automation.ElementNotAvailableException)
            {
                // Sub-optimal for performance... but if we can't get the existing element we may as well start from the root.
                theWindow = AutomationElement.RootElement;
            }

            AutomationElement statusBar = theWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "StatusBar.Pane0"));
            if (statusBar == null)
            {
                GlobalLog.LogEvidence("Failed finding the status bar");
                return false;
            }

            // Special Case for users who don't want to load their string from likely WPF-related status bar content.  Just uses whatever is after this.
            // NOT RECOMMENDED but left here in case someone needs it.  This will only work with loc builds if the app itself is setting the status bar.
            if (expectedStatusContent.StartsWith("literal:"))
            {
                expectedStatusContent = expectedStatusContent.Substring(8);
            }
            // Normal case: Load the string from the PresentationFramework ExceptionStringTable, given the string's identifier
            else
            {
                // Get the localized text we expect in the status bar...
                // Should only need values from PresentationFramework here, this can be expanded later if needed.
                ResourceManager resMan = new ResourceManager("ExceptionStringTable", typeof(System.Windows.Controls.Control).Assembly);
                expectedStatusContent = resMan.GetString(expectedStatusContent);
                GlobalLog.LogDebug("Waiting to fail if status bar value is " + expectedStatusContent + " ... ");
            }

            object patternObject;
            statusBar.TryGetCurrentPattern(ValuePattern.Pattern, out patternObject);
            ValuePattern vp = (ValuePattern)patternObject;
            int totalTries = 0;
            string statusBarValue = TryGetValueString(vp).ToLowerInvariant();
            while (!(statusBarValue.Equals(expectedStatusContent.ToLowerInvariant())) && (totalTries < 1500))
            {
                string statusBarPrevValue = statusBarValue;
                System.Threading.Thread.Sleep(10);
                object _patternObject;
                statusBar.TryGetCurrentPattern(ValuePattern.Pattern, out _patternObject);
                vp = (ValuePattern)_patternObject;
                statusBarValue = TryGetValueString(vp).ToLowerInvariant();
                totalTries++;
                if (statusBarValue != statusBarPrevValue)
                {
                    GlobalLog.LogDebug("Saw " + statusBarValue + " appear in the status bar...");
                }
            }
            if (TryGetValueString(vp).ToLowerInvariant().Equals(expectedStatusContent.ToLowerInvariant()))
            {
                GlobalLog.LogEvidence("Saw unwanted Status Bar value " + expectedStatusContent + " within 15 seconds");
                return true;
            }
            else
            {
                GlobalLog.LogEvidence("Waited to see " + expectedStatusContent + " within 15 seconds, saw " + TryGetValueString(vp) + " in the IE Status bar.");
                return false;
            }
        }

        private static string TryGetValueString(ValuePattern pattern)
        {
            try
            {
                return pattern.Current.Value;
            }
            catch
            {
                return "";
            }
        }
    }

    /// <summary>
    /// UIHandler to click a button, either by name or AutomationID. 
    /// Preface the button name with AutoID: to use automationID instead of Name.
    /// </summary>
    public class GenericDialogHandler : UIHandler
    {
        #region Private Members
        /// <summary>
        /// Testlog instance
        /// </summary>
        protected TestLog TestLog;
        #endregion

        #region Public Members
        /// <summary>
        /// Button that should be clicked when this dialog is encountered.
        /// Preface with "AutoID:" to use the rest of the string to search based on automationId, not name
        /// </summary>
        public string ButtonToClick = "";
        
        #endregion


        /// <summary>
        /// Handle any generic dialog, as long as this is registered in the UIHandler for the correct process/windowtitle.
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(IntPtr topLevelhWnd, IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            GlobalLog.LogDebug("Handling " + title + " window, clicking on button:  " + ButtonToClick);
            this.TestLog = TestLog.Current;
            if (this.TestLog == null)
            {
                throw new InvalidOperationException("Must be run in the context of a test log");
            }
            AutomationElement dialogWindow = AutomationElement.FromHandle(topLevelhWnd);
            PropertyCondition isButtonToInvoke;

            if ((ButtonToClick.Length > 7) &&(ButtonToClick.ToLowerInvariant().Substring(0, 7) == "autoid:"))
            {
                isButtonToInvoke = new PropertyCondition(AutomationElement.AutomationIdProperty, ButtonToClick.Substring(7));
            }
            else if ((ButtonToClick.Length > 6) &&(ButtonToClick.ToLowerInvariant().Substring(0, 6) == "image:"))
            {
                // Sleep for a little while since we are just blindly clicking the element... no way to know when it's ready.
                Thread.Sleep(5000);
                IEAutomationHelper.ClickCenterOfElementById(ButtonToClick.Substring(6));
                return UIHandlerAction.Handled;
            }
            else
            {
                isButtonToInvoke = new PropertyCondition(AutomationElement.NameProperty, ButtonToClick);
            }
            AutomationElement buttonToInvoke = dialogWindow.FindFirst(TreeScope.Descendants, isButtonToInvoke);
            if (buttonToInvoke != null)
            {
                try
                {
                    object patternObject;
                    buttonToInvoke.TryGetCurrentPattern(InvokePattern.Pattern, out patternObject);
                    InvokePattern ip = patternObject as InvokePattern;
                    ip.Invoke();
                    GlobalLog.LogEvidence("Successfully invoked button '" + ButtonToClick + "'");
                    return UIHandlerAction.Handled;
                }
                catch (Exception e)
                {
                    if (e is NullReferenceException || e is System.Runtime.InteropServices.SEHException)
                    {
                        throw;
                    }
                    GlobalLog.LogEvidence("Failed trying to invoke button " + ButtonToClick);
                    return UIHandlerAction.Abort;
                }
            }
            else
            {
                GlobalLog.LogEvidence("Failed to invoke button " + ButtonToClick);
                return UIHandlerAction.Abort;
            }
        }
    }

    /// <summary>
    /// UIHandler to pass if its HandleWindow method is ever invoked. 
    /// Only use this if the app in question will change its title or process, as it will always fail otherwise.
    /// </summary>
    public class PassIfSeenHandler : UIHandler
    {
        #region Private Members
        /// <summary>
        /// Testlog instance
        /// </summary>
        protected TestLog TestLog;
        #endregion

        #region Public Members

        /// <summary>
        /// Specify this if you want to check for a particular value in the IE StatusBar
        /// Only tested in IE6/7, so only use for verifying IE UI.
        /// </summary>
        public string StatusBarValue = "";

        /// <summary>
        /// Specify this to find an element by it's AutomationId and pass based on its value.
        /// Format: AutomationId;Value
        /// </summary>
        public string AutomationElementAndValue = "";

        #endregion

        /// <summary>
        /// Passes if the appropriate UI is seen.
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(IntPtr topLevelhWnd, IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            // We pass by default in this handler.  Only fail in special cases.
            bool testPassed = true;

            this.TestLog = TestLog.Current;
            if (this.TestLog == null)
            {
                throw new InvalidOperationException("Must be run in the context of a test log");
            }
            
            // For tests that involve failed deployment.  Pass only if the status bar has settled
            // on a particular value, such as "Installation Failed"
            if (StatusBarValue != "")
            {
                testPassed = testPassed && VerifyStatusBarContent(AutomationElement.FromHandle(topLevelhWnd), StatusBarValue);
            }

            // For tests that involve verifying some text from an automation element.
            // Will fail (return false) if incorrectly formatted, element does not exist, or
            // element does not support valuepatterns.
            if (AutomationElementAndValue != "")
            {
                testPassed = testPassed && VerifyAutomationElementValue(topLevelhWnd, AutomationElementAndValue);
            }

            if (testPassed)
            {
                GlobalLog.LogEvidence("Passed: Expected UI Seen ...");
                if (StatusBarValue != "")
                {
                    GlobalLog.LogEvidence("Saw expected IE StatusBar value. (" + StatusBarValue + ")");                        
                }
                GlobalLog.LogEvidence("Process Name: " + process.ProcessName + " Window Title: " + title);
                this.TestLog.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("Failed: Expected UI Seen but status bar value (" + StatusBarValue + ") not seen.");
                this.TestLog.Result = TestResult.Fail;
            }

            // Get the highest level window (may be same as current) and close it
            AutomationElement windowToClose = AutomationElement.FromHandle(topLevelhWnd);
            object patternObject;
            windowToClose.TryGetCurrentPattern(WindowPattern.Pattern, out patternObject);
            WindowPattern wp = patternObject as WindowPattern;
            wp.Close();
            GlobalLog.LogDebug("Closed main window");
            return UIHandlerAction.Abort;
        }

        private bool VerifyAutomationElementValue(IntPtr hWnd, string elementAndValue)
        {
            AutomationElement baseElement;
            try
            {
                baseElement = AutomationElement.FromHandle(hWnd);
            }
            catch (System.Windows.Automation.ElementNotAvailableException)
            {
                // Sub-optimal for performance... but if we can't get the existing element we may as well start from the root.
                baseElement = AutomationElement.RootElement;
            }

            // Only check if the element exists if a value is not passed in.
            // For scenarios where the value is unimportant
            bool onlyExists = false;
            // If not, keep the inputted element/value in here
            string[] inputs = new string[2];

            if (!elementAndValue.Contains(";"))
            {
                // Only check that the element we're looking at exists
                onlyExists = true;
            }
            else
            {
                inputs = elementAndValue.Split(';');
                if (inputs.Length != 2)
                {
                    return false;
                }
            }
            string value = "";
            try
            {
                if (!onlyExists)
                {
                    if (inputs[1].Contains("%%"))
                    {
                        string[] typeNameAndResourceId = inputs[1].Split(new string[] { "%%" }, StringSplitOptions.RemoveEmptyEntries);
                        if (typeNameAndResourceId.Length != 2)
                        {
                            return false;
                        }

                        Type toLoadResource = null;
                        switch (typeNameAndResourceId[1].ToLowerInvariant())
                        {
                            case "presentationframework":
                            default:
                                toLoadResource = typeof(System.Windows.Controls.Control);
                                break;
                        }
                        ResourceManager resMan = new ResourceManager("ExceptionStringTable", toLoadResource.Assembly);
                        value = resMan.GetString(typeNameAndResourceId[1]);
                    }
                    else
                    {
                        value = inputs[1];
                    }
                }
                int tries = 60;
                AndCondition isTheElement;
                if (onlyExists)
                {
                    isTheElement = new AndCondition(new PropertyCondition(AutomationElement.AutomationIdProperty, elementAndValue), new PropertyCondition(AutomationElement.AutomationIdProperty, elementAndValue));
                }
                else
                {
                    isTheElement = new AndCondition(new PropertyCondition(AutomationElement.NameProperty, value), new PropertyCondition(AutomationElement.AutomationIdProperty, inputs[0]));
                }
                AutomationElement elementToGet = baseElement.FindFirst(TreeScope.Descendants, isTheElement);
                while ((elementToGet == null) && (tries > 0))
                {
                    if (onlyExists)
                    {
                        GlobalLog.LogDebug("Waiting to see element with AutoId(" + elementAndValue + ")");
                    }
                    else
                    {
                        GlobalLog.LogDebug("Waiting to see element with AutoId(" + inputs[0] + ") With value (" + value + ")");
                    }
                    Thread.Sleep(2000);
                    elementToGet = baseElement.FindFirst(TreeScope.Descendants, isTheElement);
                    tries--;
                }
                
                if (elementToGet != null)
                {
                    if (onlyExists)
                    {
                        GlobalLog.LogEvidence("SUCCESS: Found element with Automation Id(" + elementAndValue + ")");
                    }
                    else
                    {
                        GlobalLog.LogEvidence("SUCCESS: Found element " + inputs[0] + " with value " + value);
                    }
                    return true;
                }
                else
                {
                    if (onlyExists)
                    {
                        GlobalLog.LogEvidence("FAILURE: Failed to find element with Automation Id(" + elementAndValue + ")");
                    }
                    else
                    {
                        GlobalLog.LogEvidence("FAILURE: Failed to find element with automation Id:" + inputs[0] + ", name:" + value);
                    }
                    return false;
                }
            }
            catch
            {
                GlobalLog.LogEvidence("Failed while trying to get value " + value + " from element with AutomationId " + inputs[0] + "\n Element may not exist");
                return false;
            }
        }

        private bool VerifyStatusBarContent(AutomationElement parentIEWindow, string expectedStatusContent)
        {
            int count = 0;
            AutomationElement statusBar = parentIEWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "StatusBar.Pane0"));
            while ((statusBar == null) && count < 1000)
            {
                Thread.Sleep(10);
                statusBar = parentIEWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "StatusBar.Pane0"));
            }

            if (statusBar == null)
            {
                GlobalLog.LogEvidence("Failed finding the status bar");
                return false;
            }
            
            // Special Case for users who don't want to load their string from likely WPF-related status bar content.  Just uses whatever is after this.
            // NOT RECOMMENDED but left here in case someone needs it.  This will only work with loc builds if the app itself is setting the status bar.
            if (expectedStatusContent.StartsWith("literal:"))
            {
                expectedStatusContent = expectedStatusContent.Substring(8);
            }
            // Normal case: Load the string from the PresentationFramework ExceptionStringTable, given the string's identifier
            else
            {
                // Get the localized text we expect in the status bar...
                // Should only need values from PresentationFramework here, this can be expanded later if needed.
                ResourceManager resMan = new ResourceManager("ExceptionStringTable", typeof(System.Windows.Controls.Control).Assembly);
                expectedStatusContent = resMan.GetString(expectedStatusContent);
                GlobalLog.LogDebug("Waiting for status bar value to be " + expectedStatusContent + " ... ");
            }

            object patternObject;
            statusBar.TryGetCurrentPattern(ValuePattern.Pattern, out patternObject);
            ValuePattern vp = (ValuePattern)patternObject;
            int totalTries = 0;
            // Poll as fast as possible, and give up to 45 seconds.  
            // lots of time is needed for certain HTTP scenarios due to strange proxy slowness observed in the lab.
            while (!(TryGetValueString(vp).ToLowerInvariant().Contains(expectedStatusContent.ToLowerInvariant())) && (totalTries < 4500))
            {
                System.Threading.Thread.Sleep(10);
                object _patternObject;
                statusBar.TryGetCurrentPattern(ValuePattern.Pattern, out _patternObject);
                vp = (ValuePattern)_patternObject;
                totalTries++;
            }
            if (TryGetValueString(vp).ToLowerInvariant().Contains(expectedStatusContent.ToLowerInvariant()))
            {
                return true;
            }
            else
            {
                GlobalLog.LogEvidence("Expected to see " + expectedStatusContent + ", Saw " + TryGetValueString(vp) + " in the IE Status bar.");
                return false;
            }
        }

        private static string TryGetValueString(ValuePattern pattern)
        {
            try
            {
                return pattern.Current.Value;
            }
            catch
            {
                return "";
            }
        }

    }




    /// <summary>
	/// UIHandler for Application Context Exceptions
	/// </summary>
	public class ExceptionHandler : UIHandler
	{
        #region Private Members

        string _expectedExceptionString;
        /// <summary>
        /// Testlog instance
        /// </summary>
        protected TestLog TestLog;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
		public ExceptionHandler()
		{
            GlobalLog.LogDebug("Entering constructor for  " + this.ToString());
		}

        #region Public Members
        /// <summary>
        /// Image file to compare to when doing VScan verification
        /// Needs to be in same directory as AppMonitor.exe is.
        /// </summary>
        /// <value></value>
        public string ExpectedExceptionString
        {
            get
            {
                return this._expectedExceptionString;
            }
            set
            {
                this._expectedExceptionString = value;
            }
        }
        #endregion


        /// <summary>
        /// Passes and notes to the log if a particular type of exception is seen.
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(IntPtr topLevelhWnd, IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            this.TestLog = TestLog.Current;
            if (this.TestLog == null)
            {
                throw new InvalidOperationException("Must be run in the context of a test log");
            }

            GlobalLog.LogEvidence("Context Exception Dialog seen...");

            //Get the Automation Element for the window and find the "Details" button
            AutomationElement window = AutomationElement.FromHandle(topLevelhWnd);
            AutomationElement btn = window.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Details"));

            if (btn == null)
            {
                GlobalLog.LogEvidence("A Context exception has occurred but we were unable to get the exception text from the UI.");
                return UIHandlerAction.Abort;
            }

            //Click on the Button since the Invoke pattern does not work
            MTI.Input.MoveToAndClick(btn);

            //Wait for the window to process the click and show the exception dialog
            object patternObject;
            window.TryGetCurrentPattern(WindowPattern.Pattern, out patternObject);
            WindowPattern winPattern = patternObject as WindowPattern;
            winPattern.WaitForInputIdle(5000);

            //find the Text box that has the exception text in it
            AutomationElement txt = window.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, "TextBox"));
            if (txt == null)
            {
                GlobalLog.LogEvidence("A Context exception has occurred but we were unable to get the exception text from the UI.");
                this.TestLog.Result = TestResult.Fail;
            }
            else
            {
                string exceptionText = txt.GetCurrentPropertyValue(ValuePattern.ValueProperty) as string;
                if (exceptionText.ToLowerInvariant().Contains(_expectedExceptionString.ToLowerInvariant()))
                {
                    GlobalLog.LogEvidence("Found exception specified in Application Context Exception Message (" + _expectedExceptionString + ")");
                    this.TestLog.Result = TestResult.Pass;
                }
                else
                {
                    GlobalLog.LogEvidence("Exception specified in UIHandler ExpectedExceptionString property not encountered:");
                    GlobalLog.LogEvidence("Actual Exception Message: ");
                    GlobalLog.LogEvidence(exceptionText);
                    this.TestLog.Result = TestResult.Fail;
                }
            }

            return UIHandlerAction.Abort;
        }

    }
}
