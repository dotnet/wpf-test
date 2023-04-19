// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Deployment;
using Microsoft.Test.Diagnostics;
using Microsoft.Test.Loaders;
using Microsoft.Test.Loaders.Steps;
using Microsoft.Test.Logging;
using Microsoft.Win32;
using MTI = Microsoft.Test.Input;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Input;

namespace Microsoft.Test.Deployment.CustomUIHandlers
{
    public class TabThroughHandler : UIHandler
    {
        public override UIHandlerAction HandleWindow(IntPtr topLevelhWnd, IntPtr hwnd, Process process, string title, UIHandlerNotification notification)
        {
            GlobalLog.LogEvidence("Verifying tabbing does not cause crashing in WPF browser content" );
            AutomationElement IEWindow = AutomationElement.FromHandle(topLevelhWnd);
            IEWindow.SetFocus();

            AutomationElement textBox = IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "refreshTestTxtBox", 20);
            if ((SystemInformation.Current.MajorVersion == 5) && (SystemInformation.Current.MinorVersion == 2))
            {
                // Work around weird clicking issue seen on Server 03, explicitly set focus...
                textBox.SetFocus();
                Thread.Sleep(100); // and sleep a little bit to allow the UIA tree to update before we check initial focus.
            }
            else
            {
                IEAutomationHelper.ClickCenterOfAutomationElement(textBox);
            }

            bool isFocused = (bool)textBox.GetCurrentPropertyValue(AutomationElement.HasKeyboardFocusProperty);
            int uiaRetryCount = 3;
            while (uiaRetryCount > 0 && !isFocused)
            {
                uiaRetryCount--;
                Thread.Sleep(1000);
                isFocused = (bool)textBox.GetCurrentPropertyValue(AutomationElement.HasKeyboardFocusProperty);
            }

            if (!isFocused)
            {
                GlobalLog.LogEvidence("Error: failed to focus the initial element");
                TestLog.Current.Result = TestResult.Fail;
                return UIHandlerAction.Abort;
            }
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Tab, true);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Tab, false);
            Thread.Sleep(1000);

            isFocused = (bool)textBox.GetCurrentPropertyValue(AutomationElement.HasKeyboardFocusProperty);
            if (!isFocused)
            {
                GlobalLog.LogEvidence("Beginning tab cycling through browser content");
            }
            else
            {
                GlobalLog.LogEvidence("Error: Tabbing did not move focus from starting element");
                TestLog.Current.Result = TestResult.Fail;
                return UIHandlerAction.Abort;
            }
            // Allow 50 tabs (way more than needed) til focus makes it back to the starting element.
            int tryCount = 50;
            while ((isFocused == false) && (tryCount > 0))
            {
                tryCount--;
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.Tab, true);
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.Tab, false);
                Thread.Sleep(100);
                isFocused = (bool)textBox.GetCurrentPropertyValue(AutomationElement.HasKeyboardFocusProperty);
            }
            // If we successfully tabbed through and nothing crashed, we should have > 1 count left.
            if (tryCount > 0)
            {
                TestLog.Current.Result = TestResult.Pass;
                GlobalLog.LogEvidence("Success: Tabbing did not cause crash in WPF browser content ");
            }
            else
            {
                GlobalLog.LogEvidence("Failure: Something went badly with regression test 6, manually verify you can tab through Xbap / Xaml content");
                TestLog.Current.Result = TestResult.Fail;
            }
            return UIHandlerAction.Abort;
        }
    }

    /// <summary>
    /// Regression tests 7- WPF HyperLink can be used to bypass IE�s subframe navigation cross-domain restriction
    /// </summary>
    public class CrossDomainFrameNavigationHandler : UIHandler
    {
        #region Public Properties

        public bool CrossDomainEnabled = false;
        public FileHostUriScheme Scheme = FileHostUriScheme.HttpInternet;

        #endregion

        #region Private members

        private static Uri s_cookieUri;
        private static bool s_crossDomainEnabled;

        private static bool InvokeElementIfPatternAvailable(AutomationElement element)
        {
            if (element != null)
            {
                InvokePattern pattern = (InvokePattern)element.GetCurrentPattern(InvokePattern.Pattern);
                if (pattern != null)
                {
                    GlobalLog.LogDebug("Nice! Got a pattern!");
                    pattern.Invoke();
                    return true;
                }
                else
                {
                    GlobalLog.LogEvidence("Couldnt get InvokePattern!");
                    return false;
                }
            }
            else
            {
                GlobalLog.LogDebug("Couldnt get InvokePattern from null element");
                return false;
            }
        }

        #endregion

        #region Handler Implementation

        public override UIHandlerAction HandleWindow(System.IntPtr topHwnd, System.IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            AutomationElement IEWindow = AutomationElement.FromHandle(topHwnd);

            // Set up the statics needed to make this work...

            // Whether this is a positive / negative test
            s_crossDomainEnabled = this.CrossDomainEnabled;

            // Figure out the Uri to ask for a cookie after testing completes.
            LoaderStep step = this.Step;
            while ((step.GetType() != typeof(FileHostStep)) && (step.ParentStep != null))
            {
                step = step.ParentStep;
            }
            if (step.GetType() != typeof(FileHostStep))
            {
                throw new Exception("Error: This test needs to check specific FileHost Uris and must be run under a FileHostStep!");
            }
            FileHost host = ((FileHostStep)step).fileHost;
            string zoneString = "";
            switch (this.Scheme)
            {
                case FileHostUriScheme.HttpInternet:
                    zoneString = "3";
                    s_cookieUri = host.GetUri("hijacked_iframe.html", FileHostUriScheme.HttpInternet);
                    break;
                case FileHostUriScheme.HttpIntranet:
                    zoneString = "1";
                    s_cookieUri = host.GetUri("hijacked_iframe.html", FileHostUriScheme.HttpIntranet);
                    break;
                default:
                    throw new Exception("Only know HTTP internet and HTTP intranet behaviors");
            }

            // Pre-clear the cookie... 
            Application.SetCookie(s_cookieUri, String.Empty);

            // Set up IE to do what we expect... tweak registry to allow or dis-allow the behavior based on CrossDomainEnabled. 
            // Note that we can do this because the property doesnt require IE to be restarted.
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Zones\" + zoneString, "1607", (s_crossDomainEnabled ? 0 : 3));
            GlobalLog.LogEvidence(@"Set HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Zones\" + zoneString + @"\1607 = " + (s_crossDomainEnabled ? 0 : 3));
            GlobalLog.LogEvidence(" Note: Zones: 1 = Values: 3 = Disabled, 0 = Enabled");

            // Do all the work on a STA thread so UIA finds hyperlinks
            ParameterizedThreadStart workerThread = new ParameterizedThreadStart(handleWindowNewThread);
            Thread thread = new Thread(workerThread);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start((object)IEWindow);
            thread.Join();

            return UIHandlerAction.Abort;
        }

        private static void handleWindowNewThread(object theWindow)
        {
            AutomationElement window = (AutomationElement)theWindow;

            WindowPattern ieWindowPattern = window.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;
            ieWindowPattern.SetWindowVisualState(WindowVisualState.Normal);
            ieWindowPattern.WaitForInputIdle(10000);

            TransformPattern transformPattern = window.GetCurrentPattern(TransformPattern.Pattern) as TransformPattern;
            transformPattern.Move(0.0, 0.0);
            transformPattern.Resize(1024.0, 768.0);

            GlobalLog.LogEvidence("Cross Domain Frame navigation is " + (s_crossDomainEnabled ? "" : "not ") + "enabled");
            GlobalLog.LogEvidence("Using cookie written to " + s_cookieUri.ToString() + " to determine outcome of test");

            window.SetFocus();

            AutomationElement popNewWindowLink = IEAutomationHelper.WaitForHTMLHyperlinkByName(AutomationElement.RootElement, "click here to open", 30);
            InvokeElementIfPatternAvailable(popNewWindowLink);

            // Not much particularly accessible about an IE Window with no WPF in it, so just spin for a while...
            Thread.Sleep(10000);

            // Return focus to original window and invoke hijack button.
            // Since UIA is utterly unreliable, I find doing it 3x with a small sleep makes it fail less.
            window.SetFocus(); Thread.Sleep(200); window.SetFocus(); Thread.Sleep(200); window.SetFocus(); Thread.Sleep(200);

            IEAutomationHelper.WaitForElementWithAutomationId(window, "FrameHijackXamlLink", 20);
            IEAutomationHelper.ClickCenterOfElementById(window, "FrameHijackXamlLink");

            int retryCount = 10;

            string receivedCookie = "";

            while ((!receivedCookie.ToLowerInvariant().Contains("hosted=")) && retryCount > 0)
            {
                Thread.Sleep(1000);
                try
                {
                    receivedCookie = Application.GetCookie(s_cookieUri);
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    // Do nothing.  This means there was no cookie.
                }
                retryCount--;
            }

            GlobalLog.LogDebug("Received Cookie = " + receivedCookie);

            bool crossDomainNavigationSucceeded = false;

            if (receivedCookie.ToLowerInvariant().Contains("hosted=true"))
            {
                crossDomainNavigationSucceeded = true;
                GlobalLog.LogEvidence("Successfully navigated a frame across domains");
            }
            else if (receivedCookie.ToLowerInvariant().Contains("hosted=false"))
            {
                crossDomainNavigationSucceeded = false;
                GlobalLog.LogEvidence("Navigating a frame across domains did not succeed");
            }
            else
            {
                GlobalLog.LogEvidence("Test case issue with this test: expected to see a cookie but did not.");
                TestLog.Current.Result = TestResult.Fail;
                return;
            }
            // re-clear the cookie... 
            Application.SetCookie(s_cookieUri, String.Empty);

            // Yes I could probably just use an XOR.  This is more legible.
            if ((s_crossDomainEnabled && crossDomainNavigationSucceeded) || (!s_crossDomainEnabled && !crossDomainNavigationSucceeded))
            {
                TestLog.Current.Result = TestResult.Pass;
                GlobalLog.LogEvidence("Success: Cross domain navigation behavior as expected for WPF browser content");
            }
            else
            {
                GlobalLog.LogEvidence("Failure: Cross domain navigation behavior was not as expected for WPF browser content");
                TestLog.Current.Result = TestResult.Fail;
            }
        }
        #endregion
    }

    public class IEFavoritesEditorHandler : UIHandler
    {
        public override UIHandlerAction HandleWindow(IntPtr topLevelhWnd, IntPtr hwnd, Process process, string title, UIHandlerNotification notification)
        {
            // If you load IEFrame.dll or BrowseLC (Depends on IE version) in Visual Studio, 
            // The accelerator tables are 256 and 7 (I use 256) and 41330 = the Ctrl B shortcut)
            KeyGesture ctrlB = IEAutomationHelper.GetAcceleratorFromWin32Table(IEAutomationHelper.GetIEResourceDLLPath(), 256, 41330);

            GlobalLog.LogEvidence("Using Ctrl B Key Gesture: " + ctrlB.ToString());
            AutomationElement IEWindow = AutomationElement.FromHandle(topLevelhWnd);

            IEWindow.SetFocus();

            Thread.Sleep(100);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftCtrl, true);
            Microsoft.Test.Input.Input.SendKeyboardInput(ctrlB.Key, true);
            Microsoft.Test.Input.Input.SendKeyboardInput(ctrlB.Key, false);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftCtrl, false);
            Thread.Sleep(1000);

            GlobalLog.LogEvidence("Pressed Ctrl-B to show Favorites organizing menu");

            PropertyCondition isOrganizeFavoritesWindow = new PropertyCondition(AutomationElement.ClassNameProperty, "Internet Explorer_TridentDlgFrame");
            AutomationElement organizeFavoritesWindow = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, isOrganizeFavoritesWindow);

            if (organizeFavoritesWindow == null)
            {
                foreach (AutomationElement element in AutomationElement.RootElement.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "IEFrame")))
                {
                    organizeFavoritesWindow = element.FindFirst(TreeScope.Descendants, isOrganizeFavoritesWindow);
                    if (organizeFavoritesWindow != null)
                    {
                        break;
                    }
                }
            }

            // Simulate moving, then closing the window.  If it's gone 2 seconds after we close it and no exceptions thrown, then this worked..
            // Can't use Translate + WindowPattern, since these both work around the bug.
            PropertyCondition isOrganizeFavoritesTitleBar = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TitleBar);
            AutomationElement titleBar = organizeFavoritesWindow.FindFirst(TreeScope.Descendants, isOrganizeFavoritesTitleBar);

            System.Windows.Rect titleBarRect = (Rect)titleBar.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty, true);
            Point pt = new Point(((titleBarRect.Left + titleBarRect.Right) / 2.0), ((titleBarRect.Top + titleBarRect.Bottom) / 2.0));

            GlobalLog.LogEvidence("Clicking and dragging \"Organize Favorites\" Window... ");

            MTI.Input.MoveToAndClick(pt);
            MTI.Input.SendMouseInput(pt.X, pt.Y, 0, Microsoft.Test.Input.SendMouseInputFlags.LeftDown | Microsoft.Test.Input.SendMouseInputFlags.Absolute);

            for (int iteration = 0; iteration < 50; iteration ++)
            {
                MTI.Input.SendMouseInput(pt.X, pt.Y, 0, Microsoft.Test.Input.SendMouseInputFlags.LeftDown | Microsoft.Test.Input.SendMouseInputFlags.Move | Microsoft.Test.Input.SendMouseInputFlags.Absolute);
                pt.X += 5.0;
                pt.Y += 5.0;
                Thread.Sleep(50);
            }
            MTI.Input.SendMouseInput(pt.X, pt.Y, 0, Microsoft.Test.Input.SendMouseInputFlags.LeftUp);

            // This works on some machines, but is too fast for others.  Sleep 2 seconds to allow UIA to catch up.
            Thread.Sleep(2000);

            System.Windows.Rect finalTitleBarRect = (Rect)titleBar.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty, true);

            bool successfulMove = (finalTitleBarRect.Top != titleBarRect.Top) && (finalTitleBarRect.Left != titleBarRect.Left);

            GlobalLog.LogEvidence("Was " + (successfulMove ? "" : "not ") + "able to move the Dialog using mouse-drag movements");


            PropertyCondition isOrganizeFavoritesCloseButton = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button);
            AutomationElement titleBarCloseButton = titleBar.FindFirst(TreeScope.Descendants, isOrganizeFavoritesCloseButton);

            GlobalLog.LogEvidence("Clicking \"Close\" button on Organize Favorites Window");

            MTI.Input.MoveToAndClick(titleBarCloseButton);

            // The window needs to close within 2 seconds of being told to... 
            Thread.Sleep(2000);

            AutomationElement searchElement = null;
            switch (ApplicationDeploymentHelper.GetIEVersion())
            {
                case 6:
                case 7:
                    {
                        searchElement = IEWindow.FindFirst(TreeScope.Descendants, isOrganizeFavoritesWindow);
                        break;
                    }
                case 8:
                    {
                        Process[] allIEs = Process.GetProcessesByName("iexplore");
                        foreach (Process proc in allIEs)
                        {
                            if (proc.MainWindowHandle != IntPtr.Zero)
                            {
                                AutomationElement ieWin = AutomationElement.FromHandle(proc.MainWindowHandle);
                                searchElement = ieWin.FindFirst(TreeScope.Descendants, isOrganizeFavoritesWindow);
                                if (searchElement != null)
                                {
                                    break;
                                }
                            }
                        }
                        break;
                    }
                // Assume LCIE is the standard going forward... 
                default: goto case 8;
            }

            if (successfulMove && (searchElement == null))
            {
                GlobalLog.LogEvidence("Success: Was able to move and dismiss IE Favorites-organizing window (Should only ever fail on IE6)");
                TestLog.Current.Result = TestResult.Pass;
            }
            else 
            {
                GlobalLog.LogEvidence("Failure: Was not able to move and dismiss IE Favorites-organizing window (Should only ever fail on IE6)");
                if (ApplicationDeploymentHelper.GetIEVersion() == 6)
                {
                    TestLog.Current.Result = TestResult.Ignore;
                    GlobalLog.LogEvidence("Since we ARE running on IE6 though, setting result to Ignore.");
                }
                else
                {
                    TestLog.Current.Result = TestResult.Fail;
                }
            }
            return UIHandlerAction.Abort;
        }
    }

    // Exercise MailTo: hyperlinks in WPF browser apps.
    // Since the behavior is radically different OS-to-OS, just validates a top-level-window was created.
    public class MailToUIHandler : UIHandler
    {
        public override UIHandlerAction HandleWindow(IntPtr topLevelhWnd, IntPtr hwnd, Process process, string title, UIHandlerNotification notification)
        {
            // Windows 7 - MailTo: Links fail to display error message in default Windows 7 setup state for WPF and other shell-executed "Mailto" links
            // So just ignore this test... the bug is not likely to ever get fixed given IE8 has RTM'ed and most people dont care if no UI displays when they have no mail client.
            if (ApplicationDeploymentHelper.GetIEVersion() >= 8)
            {
                GlobalLog.LogEvidence("Running on Internet Explorer 8+, where Win7 697861 prevents anything perceptible from happening when clicking mailto: links.  Setting result to Ignore and returning.");
                TestLog.Current.Result = TestResult.Ignore;
                return UIHandlerAction.Abort;
            }

            AutomationElement browserWindow = AutomationElement.FromHandle(topLevelhWnd);

            OrCondition isTopLevelWindow = new OrCondition(
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Pane),
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window));

            AndCondition isMyHyperlink = new AndCondition(new PropertyCondition(AutomationElement.NameProperty, "MailTo Scenario"),
                new PropertyCondition(AutomationElement.IsInvokePatternAvailableProperty, true));

            AutomationElement hyperlink = IEAutomationHelper.WaitForElementByAndCondition(browserWindow, isMyHyperlink, 15);

            AutomationElementCollection preInvoke = AutomationElement.RootElement.FindAll(TreeScope.Children, isTopLevelWindow);

            // In case UIA doesnt find a window, try to find WinMail.exe (common handler for this)
            Process[] WinMailsBefore = Process.GetProcessesByName("WinMail");

            // Can't invoke, even though that's the more robust way to do this, 
            IEAutomationHelper.ClickCenterOfAutomationElement(hyperlink);
            Thread.Sleep(6000);

            // In case UIA doesnt find a window, try to find WinMail.exe (common handler for this)
            Process[] WinMailsAfter = Process.GetProcessesByName("WinMail");
            AutomationElementCollection postInvoke = AutomationElement.RootElement.FindAll(TreeScope.Children, isTopLevelWindow);
            GlobalLog.LogEvidence("Running WinMail instances before invoking link: " + WinMailsBefore.Length + " After: " + WinMailsAfter.Length);
            GlobalLog.LogEvidence("Top level windows before invoking link: " + preInvoke.Count + " After: " + postInvoke.Count);

            if ((preInvoke.Count < postInvoke.Count) || (WinMailsBefore.Length < WinMailsAfter.Length))
            {
                GlobalLog.LogEvidence("Success: Invoking Mailto: Hyperlink results in creating a top-level window or spawning a mail client process");
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("Failure: Invoking Mailto: Hyperlink may have failed.  Ensure that this scenario works for WPF browser content.");
                TestLog.Current.Result = TestResult.Fail;
            }

            return UIHandlerAction.Abort;
        }
    }

    public class XbapHostedInHTMLVerifier : UIHandler
    {
        // Regression test case 3 
        public override UIHandlerAction HandleWindow(IntPtr topLevelhWnd, IntPtr hwnd, Process process, string title, UIHandlerNotification notification)
        {
            GlobalLog.LogEvidence("Testing that in-browser focus can cycle through HTML -> WPF (All elements) -> HTML again");
            GlobalLog.LogEvidence("Prevents regression 3");

            AutomationElement testWindow = AutomationElement.FromHandle(topLevelhWnd);

            AutomationElement securityButton = IEAutomationHelper.WaitForElementWithAutomationId(testWindow, "btnSecurityTester", 5);



            securityButton.SetFocus();

            // Button after WebOC for forward focus navigation
            AutomationElement focusTestButton = IEAutomationHelper.WaitForElementWithAutomationId(testWindow, "FocusTestButton", 5);

            // Button before WebOC for reverse navigation
            AutomationElement navigateButton = IEAutomationHelper.WaitForElementWithAutomationId(testWindow, "navigateButton", 5);

            if (navigateButton == null)
            {
                GlobalLog.LogEvidence("Couldn't get inner Xbap after a 35 second timeout, quitting...");
                TestLog.Current.Result = TestResult.Fail;
                return UIHandlerAction.Abort;
            }
            bool success = true;
            GlobalLog.LogEvidence("Forward Tab scenario:  Ensure that focus can travel out of Xbap, into WebOC, then out to WebOC's container");
            success &= DoFocusTabbingTest(securityButton, focusTestButton, true);
            GlobalLog.LogEvidence("Reverse Tab scenario:  Ensure that focus can travel into WebOC showing HTML, then into Xbap, then back out again");
            success &= DoFocusTabbingTest(focusTestButton, navigateButton, false);

            if (success)
            {
                GlobalLog.LogEvidence("Success:  Was able to tab in and out of container with Xbap hosted in HTML (Regression case 3)");
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("FAIL:  Errors seen attempting to tab in/out of WebOC hosting HTML hosting Xbap.");
                TestLog.Current.Result = TestResult.Fail;
            }
            return UIHandlerAction.Abort;
        }

        private bool DoFocusTabbingTest(AutomationElement focusOriginButton, AutomationElement focusDestinationButton, bool forwardTab)
        {
            int tabLimit = 50;
            do
            {
                tabLimit--;
                if (!forwardTab)
                {
                    MTI.Input.SendKeyboardInput(Key.LeftShift, true);
                }
                MTI.Input.SendKeyboardInput(Key.Tab, true);
                MTI.Input.SendKeyboardInput(Key.Tab, false);
                if (!forwardTab)
                {
                    MTI.Input.SendKeyboardInput(Key.LeftShift, false);
                }
                // Sleep 400 ms to guarantee UIA cache update, important for the "while" condition of this loop
                Thread.Sleep(400);
            }
            while ((AutomationElement.FocusedElement != focusOriginButton) && (AutomationElement.FocusedElement != focusDestinationButton)
                    && (tabLimit > 0));
            if (AutomationElement.FocusedElement == focusOriginButton)
            {
                GlobalLog.LogEvidence("ERROR: Focus appears to be staying within the hosted Xbap. \n Expected: Focus should leave hosted browser application and cycle through application");
                return false;
            }
            if (AutomationElement.FocusedElement == focusDestinationButton)
            {
                GlobalLog.LogEvidence("Success: " + (forwardTab ? "" : "Shift-") + "Tab Focus was able to travel from hosted Xbap -> WebOC -> Host application.");
                return true;
            }
            else
            {
                GlobalLog.LogEvidence("Error: Saw element named \"" + AutomationElement.FocusedElement.Current.Name + "\" instead of expected button on Shift-tab focus test...");
                return false;
            }
        }
    }

    // Regression test 4
    public class PresentationHostTextLeakHandler : UIHandler
    {
        public override UIHandlerAction HandleWindow(IntPtr topLevelhWnd, IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            AutomationElement IEWindow = AutomationElement.FromHandle(topLevelhWnd);
            IEWindow.SetFocus();

            ParameterizedThreadStart workerThread = new ParameterizedThreadStart(handleWindowNewThread);
            Thread thread = new Thread(workerThread);
            thread.SetApartmentState(ApartmentState.STA);

            thread.Start((object)IEWindow);
            thread.Join();
            return UIHandlerAction.Abort;
        }

        private static void handleWindowNewThread(object theWindow)
        {
            AutomationElement IEWindow = (AutomationElement)theWindow;
            Thread.Sleep(500);
            // Navigate from HTML so we have a back stack on the browser
            IEAutomationHelper.ClickHTMLHyperlinkByName(IEWindow, "HtmlRelXbap");
            // Wait til we see the .xbap...
            AutomationElement textBox = IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "refreshTestTxtBox", 10);
            // Type into TextBox
            textBox.SetFocus();
            Microsoft.Test.Input.Input.SendUnicodeString("Test string being sent to xbap... ");
            Thread.Sleep(500);
            ValuePattern vp = textBox.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;

            if (vp.Current.Value.Length == 0)
            {
                GlobalLog.LogStatus("Error: Input does not appear to have made it to the text box.  Ignoring test result...");
                TestLog.Current.Result = TestResult.Ignore;
                return;
            }

            GlobalLog.LogStatus("Input appears to have made it to the text box.  Navigating backwards...");

            IEAutomationHelper.ClickIEBackButton(IEWindow);
            // Give PHost plenty of time to exit...
            Thread.Sleep(7500);

            Process[] PHosts = Process.GetProcessesByName("PresentationHost.exe");

            if (PHosts.Length == 0)
            {
                GlobalLog.LogStatus("Success: PresentationHost.exe exited on backward navigation despite text input to app.");
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogStatus("Failure: " + PHosts.Length + " instances of PresentationHost.exe still running!");
                TestLog.Current.Result = TestResult.Fail;
            }

            return;
        }
    }

    /// <summary>
    /// UI handler
    /// </summary>
    public class FrameScrollRepaintRegressionHandler : UIHandler
    {
        /// <summary>
        /// Scroll a custom page (with xbap in frame) down and up, making sure the app paints at the end.
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(System.IntPtr topLevelhWnd, System.IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            
            if (!SystemInformation.Current.OSVersion.StartsWith("6"))
            {
                GlobalLog.LogEvidence("This test guards against regression 2, which is specific to Vista.  Setting result to \"ignore\" and returning...");
                TestLog.Current.Result = TestResult.Ignore;
                return UIHandlerAction.Abort;
            }

            AutomationElement IEWindow = AutomationElement.FromHandle(topLevelhWnd);

            WindowPattern ieWindowPattern = IEWindow.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;
            ieWindowPattern.SetWindowVisualState(WindowVisualState.Normal);
            ieWindowPattern.WaitForInputIdle(10000);

            TransformPattern ieTransformPattern = IEWindow.GetCurrentPattern(TransformPattern.Pattern) as TransformPattern;

            // Should work on anything running @ 1024x768, a test machine standard.
            ieTransformPattern.Move(0.0, 0.0);
            ieTransformPattern.Resize(775.0, 700.0);

            // Can't use "IsOffscreen" since that's always false.  Yay.
            // So, simply figure out when the screen coords of the button in the app goes off screen (i.e. negative)
            IEWindow.SetFocus();
            AutomationElement XbapTestButton = IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "btnSecurityTester", 20);
            double originalTopCoord = ((System.Windows.Rect)XbapTestButton.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty)).Top;
            double currentTop = originalTopCoord;

            // Give the IE Frame focus
            IEAutomationHelper.ClickCenterOfElementByName(IEWindow, "Sentinel Text", 20);
            IEAutomationHelper.ClickCenterOfElementByName(IEWindow, "Sentinel Text", 20);

            GlobalLog.LogEvidence("Scrolling: Position started at: " + currentTop.ToString());

            IEWindow.SetFocus();
            while (currentTop > -50.0)
            {
                MTI.Input.SendKeyboardInput(Key.Down, true);
                MTI.Input.SendKeyboardInput(Key.Down, false);
                Thread.Sleep(300);
                currentTop = ((System.Windows.Rect)XbapTestButton.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty)).Top;
            }

            GlobalLog.LogEvidence("Successfully scrolled xbap out of view, now scrolling up...");
            Thread.Sleep(5000);

            while (currentTop < originalTopCoord)
            {
                currentTop = ((System.Windows.Rect)XbapTestButton.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty)).Top;
                MTI.Input.SendKeyboardInput(Key.Up, true);
                MTI.Input.SendKeyboardInput(Key.Up, false);
                Thread.Sleep(300);
            }

            GlobalLog.LogEvidence("Scrolling completed.  Now visually verifying... ");

            VisualVerifier verifier = new VisualVerifier();
            verifier.CompareImage = Environment.CurrentDirectory + "\\deploy_contentRendered.png";
            verifier.ImageMatches = 1;
            verifier.NegativeTest = false;
            verifier.HandleWindow(topLevelhWnd, hwnd, process, title, notification);
            return UIHandlerAction.Abort;
        }
    }

    public class NestedTranslateAcceleratorRegressionHandler : UIHandler
    {
        public string TestScenario;

        public override UIHandlerAction HandleWindow(IntPtr topLevelhWnd, IntPtr hwnd, Process process, string title, UIHandlerNotification notification)
        {
            AutomationElement mainWindow = AutomationElement.FromHandle(topLevelhWnd);
            AutomationElement indicatorButton = IEAutomationHelper.WaitForElementWithAutomationId(mainWindow, "transAccelIndicator", 20);

            mainWindow.SetFocus();
            System.Windows.Rect rect = (System.Windows.Rect)mainWindow.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);

            MTI.Input.MoveTo(new System.Windows.Point(rect.Left, rect.Top));

            // Point to click (UIA cannot see these elements at ALL) and the key for which accelerator key will be used in this scenario.
            Point usablePoint;
            Key acceleratorKey;
            // Need to these separately as variations so test results will show which fails if one regresses.
            Double dpiRatio = System.Drawing.Graphics.FromHwnd(IntPtr.Zero).DpiX / 96.0;
            GlobalLog.LogEvidence("Using DPI Scale ratio of " + dpiRatio.ToString() + "x");
            switch (TestScenario.ToLowerInvariant())
            {
                case "wpftextbox":
                    usablePoint = new Point(rect.Left + (193.0 * dpiRatio), rect.Top + (202.0 * dpiRatio));
                    acceleratorKey = Key.A;
                    break;
                case "winformstextbox":
                    usablePoint = new Point(rect.Left + (243.0 * dpiRatio), rect.Top + (275.0 * dpiRatio));
                    acceleratorKey = Key.R;
                    break;
                case "win32textbox":
                    usablePoint = new Point(rect.Left + (148.0 * dpiRatio), rect.Top + (379.0 * dpiRatio));
                    acceleratorKey = Key.C;
                    break;
                default:
                    throw new InvalidOperationException("Don't know anything about test scenario \" " + TestScenario + " \"");
            }
            GlobalLog.LogDebug("Usable Point = " + usablePoint.X + ", " + usablePoint.Y);

            // Clicks inside one of the hosted text boxes and presses the desired accelerator combo
            ClickTextBoxAndHitAltKey(usablePoint, acceleratorKey);

            if (ValidationButtonHasCorrectValue(TestScenario + " Accelerator Received 1", indicatorButton))
            {
                GlobalLog.LogEvidence("Success! Scenario " + TestScenario + " succeeded (was able to listen to accelerator messages, Regression5 prevention");
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("ERROR: Scenario " + TestScenario + " failed (Regression5 regressed, investigate)");
                TestLog.Current.Result = TestResult.Fail;
            }
            return UIHandlerAction.Abort;
        }

        private static bool ValidationButtonHasCorrectValue(string expectedValue, AutomationElement element)
        {
            // Let UIA values settle...
            Thread.Sleep(250);

            if (element.GetCurrentPropertyValue(AutomationElement.NameProperty).ToString().ToLowerInvariant() == expectedValue.ToLowerInvariant())
            {
                GlobalLog.LogEvidence("Saw expected value (" + expectedValue + ") on Automation element.");
                return true;
            }
            else
            {
                GlobalLog.LogEvidence("Expected value (" + expectedValue + ") but saw " + element.GetCurrentPropertyValue(AutomationElement.NameProperty) + "on Automation element.");
                return false;
            }
        }

        private static void ClickTextBoxAndHitAltKey(Point clickablePoint, Key toPress)
        {
            MTI.Input.MoveTo(clickablePoint);
            MTI.Input.SendMouseInput(clickablePoint.X, clickablePoint.Y, 0, Microsoft.Test.Input.SendMouseInputFlags.LeftDown);
            MTI.Input.SendMouseInput(clickablePoint.X, clickablePoint.Y, 0, Microsoft.Test.Input.SendMouseInputFlags.LeftUp);
            Thread.Sleep(1500);
            MTI.Input.SendKeyboardInput(Key.LeftAlt, true);
            MTI.Input.SendKeyboardInput(toPress, true);
            MTI.Input.SendKeyboardInput(toPress, false);
            MTI.Input.SendKeyboardInput(Key.LeftAlt, false);
            // We're going to use UIA to verify this, so allow it the UIA tree to stabilize before returning.
            Thread.Sleep(1000);
        }
    }
}
