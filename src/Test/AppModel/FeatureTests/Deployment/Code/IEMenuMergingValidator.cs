// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Deployment;
using Microsoft.Test.Diagnostics;
using Microsoft.Test.Logging;
using Microsoft.Test.Loaders;
using MTI = Microsoft.Test.Input;
using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Input;

namespace Microsoft.Windows.Test.Client.AppSec.Deployment.CustomUIHandlers
{
    /// <summary>
    /// UI handler
    /// </summary>
    public class IEMenuMergingValidator: UIHandler
    {
        /// <summary>
        /// constructor
        /// </summary>
        public IEMenuMergingValidator() { }
        
        private static AutomationElement s_ieWindow;

        /// <summary>
        /// Bool representing whether the handler should check for file/save to be enabled
        /// For the .xbap version of the test, it will be enabled since we use the commandbinding
        /// </summary>
        public bool FileSaveEnabled = false;
        private static bool s_fileSaveEnabled = false;

        /// <summary>
        /// Checks for existence of "Expected" menus when displaying .xaml / .xbap
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(System.IntPtr topHwnd, System.IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            s_fileSaveEnabled = this.FileSaveEnabled;
            s_ieWindow = AutomationElement.FromHandle(topHwnd);
            s_ieWindow.SetFocus();

            // Sleep to wait the xbap file to finish loading
            Thread.Sleep(1000);

            bool succeeded = true;

            GlobalLog.LogEvidence(" *** Top Level test: *** ");
            succeeded &= InternetExplorerClassicMenu(topHwnd);
            s_ieWindow.SetFocus();

            GlobalLog.LogEvidence(" *** Edit Menu Test: *** ");
            succeeded &= InternetExplorerEditMenu();
            s_ieWindow.SetFocus();

            GlobalLog.LogEvidence(" *** File Menu Test: *** ");
            succeeded &= InternetExplorerFileMenu();
            s_ieWindow.SetFocus();

            GlobalLog.LogEvidence(" *** View Menu Test: *** ");
            succeeded &= InternetExplorerViewMenu();
            s_ieWindow.SetFocus();

            GlobalLog.LogEvidence(" *** 'Go To' Menu Test: *** ");
            succeeded &= InternetExplorerGoTo();
            s_ieWindow.SetFocus();

            GlobalLog.LogEvidence(" *** Tools Menu Test: (IE6 only) *** ");
            succeeded &= InternetExplorerTools(topHwnd);

            GlobalLog.LogEvidence(" *** Page Menu Test: (IE7 only) *** ");
            succeeded &= InternetExplorerPage(topHwnd);
            GlobalLog.LogEvidence("Finished!");

            if (succeeded)
            {
                TestLog.Current.Result = TestResult.Pass;
                GlobalLog.LogEvidence("Passed : Browser menu merging working correctly with all menus accounted for and correctly enabled/disabled");
            }
            else
            {
                TestLog.Current.Result = TestResult.Fail;
                GlobalLog.LogEvidence("Failed : One or more menus was incorrect.  See above log for more info.");
            }
            return UIHandlerAction.Abort;
        }

        /// <summary>
        /// Test IE Top level menus.
        /// </summary>
        public static bool InternetExplorerClassicMenu(IntPtr topHwnd)
        {
            bool success = true;

            IEAutomationHelper.ClickIEAddrBarTwice(s_ieWindow);
            MTI.Input.SendKeyboardInput(Key.LeftAlt, true);
            Thread.Sleep(250);
            MTI.Input.SendKeyboardInput(Key.LeftAlt, false);
            Thread.Sleep(250);

            // IE File should be enabled
            // ValidateIsEnabled(topHwnd, AutomationID.IExplorer.Bar.File, "File", true);
            success &= ValidateIsEnabled(topHwnd, "Item 32768", "File", true);

            // Load the next 2 from PresentationHostDll.Dll (since these are the "merged" menus)
            string pHostDllPath = Microsoft.Test.Diagnostics.SystemInformation.Current.FrameworkWpfPath + CultureInfo.CurrentUICulture.Name + ApplicationDeploymentHelper.PresentationHostDllMui;
            if (!File.Exists(pHostDllPath))
            {
                pHostDllPath = Microsoft.Test.Diagnostics.SystemInformation.Current.FrameworkWpfPath + @"\en-us" + ApplicationDeploymentHelper.PresentationHostDllMui;
            }
            // IE Edit should be enabled.
            success &= ValidateIsEnabled(topHwnd, null, IEAutomationHelper.GetUnmanagedTopMenuResourceString(pHostDllPath, 2000, 0, 0).Replace("&", ""), true);

            // IE View should be enabled.
            success &= ValidateIsEnabled(topHwnd, null, IEAutomationHelper.GetUnmanagedTopMenuResourceString(pHostDllPath, 2000, 1, 1).Replace("&","") , true);

            // IE GoTo should be enabled.
            // ValidateIsEnabled(topHwnd, AutomationID.IExplorer.Bar.GoTo, "Go To", true);
            success &= ValidateIsEnabled(topHwnd, "Item 33104", "Go To", true);
            
            // IE Favorites should be enabled.
            success &= ValidateIsEnabled(topHwnd, "Item 33136", "Favorites", true);

            // IE Help should be enabled.
            success &= ValidateIsEnabled(topHwnd, "Item 33024", "Help", true);

            return success;
        }

        /// <summary>
        /// Test IE Edit menu.  
        /// Test fails if the PresentationHost specific IE menu items are not present.
        /// Not checking enabled/disabled status due to weird behavior seen randomly on downlevel platforms.
        /// When/if that issue is fixed this can be switched back to check enabled.
        /// </summary>
        public static bool InternetExplorerEditMenu()
        {
            bool success = true;
            // Invoke the IE Edit menu.
            AutomationElement IEEditMenu = IEAutomationHelper.ShowIEEditMenu(s_ieWindow);

            // IE Edit Cut should be disabled.
            success &= ValidateExists(IEEditMenu, "Item 8001", "Cut");

            // IE Edit Copy should be disabled.
            success &= ValidateExists(IEEditMenu, "Item 8002", "Copy");

            // IE Edit Paste should be disabled.
            success &= ValidateExists(IEEditMenu, "Item 8003", "Paste");

            // IE Edit Select All should be enabled.
            success &= ValidateExists(IEEditMenu, "Item 8004", "Select All");

            // IE Edit Find (on this page) should be enabled.
            success &= ValidateExists(IEEditMenu, "Item 8005", "Find (on This Page)...");

            return success;

        }

        /// <summary>
        /// Test IE Edit menu.  
        /// Test fails if the PresentationHost specific IE menu items are correctly enabled, disabled and removed.
        /// The failed status is logged automatically by DRT.Logger. Test continues even when an test area fails.      
        /// </summary>
        public static bool InternetExplorerGoTo()
        {
            bool success = true;
            // Invoke the IE Edit menu.
            AutomationElement IEGoToMenu = IEAutomationHelper.ShowIEGoToMenu(s_ieWindow);

            // IE Go To/Back should be disabled.
            success &= ValidateIsEnabled(IEGoToMenu, "Item 41249", "Back", false);

            // IE Go To/Fwd should be disabled.
            success &= ValidateIsEnabled(IEGoToMenu, "Item 41250", "Forward", false);

            // IE Go To/Home should be enabled.
            success &= ValidateIsEnabled(IEGoToMenu, "Item 41253", "Home Page", true);

            return success;
        }
                
        /// <summary>
        /// Test IE File menu.
        /// Test fails if the PresentationHost specific IE menu items are correctly enabled, disabled and removed.
        /// The failed status is logged automatically by DRT.Logger. Test continues even when an test area fails.
        /// </summary>
        public static bool InternetExplorerFileMenu()
        {
            bool success = true;
            AutomationElement IEFileMenu = IEAutomationHelper.ShowIEFileMenu(s_ieWindow);                       
            
            // IE File Save should be enabled
            success &= ValidateIsEnabled(IEFileMenu, "Item 257", "Save", s_fileSaveEnabled);

            // IE File SaveAs should be enabled, but not over Http
            success &= ValidateIsEnabled(IEFileMenu, "Item 258", "Save As...", false);

            //IE File Edit should be removed.
            string fileEdit = "";
            switch (ApplicationDeploymentHelper.GetIEVersion())
            {
                case 6:
                    {
                        fileEdit = IEAutomationHelper.GetUnmanagedSubMenuResourceString(Environment.SystemDirectory + @"\browselc.dll", 267, 0, 2);
                        break;
                    }
                case 7:
                    {
                        fileEdit = IEAutomationHelper.GetUnmanagedSubMenuResourceString(Environment.SystemDirectory + @"\" + CultureInfo.CurrentUICulture.Name + @"\IEFRAME.dll.mui", 267, 0, 4);
                        // Second chance... try again w/ English
                        if (fileEdit == null)
                            fileEdit = IEAutomationHelper.GetUnmanagedSubMenuResourceString(Environment.SystemDirectory + @"\en-us\IEFRAME.dll.mui", 267, 0, 3);
                        break;
                    }
                case 8:
                    {
                        fileEdit = IEAutomationHelper.GetUnmanagedSubMenuResourceString(Environment.SystemDirectory + @"\" + CultureInfo.CurrentUICulture.Name + @"\IEFRAME.dll.mui", 333, 0, 5);
                        // Second chance... try again w/ English
                        if (fileEdit == null)
                            fileEdit = IEAutomationHelper.GetUnmanagedSubMenuResourceString(Environment.SystemDirectory + @"\en-us\IEFRAME.dll.mui", 333, 0, 5);
                        break;
                    }                
                default:
                    goto case 8;
            }
            if (fileEdit == null) // STILL failed... log it and try english hard coding
            {
                fileEdit = "Edit";
                GlobalLog.LogEvidence("ERROR: had to fall back to english string!");
            }
            fileEdit = fileEdit.Replace("&", "");
            success &= ValidateElementRemoved(IEFileMenu, fileEdit);            

            // IE File Page Setup should be disabled.
            success &= ValidateIsEnabled(IEFileMenu, "Item 259", "Page Setup...", false);

            //IE File Print should be enabled.
            success &= ValidateIsEnabled(IEFileMenu, "Item 260", "Print...", true);

            // IE File Properties should be enabled.
            success &= ValidateIsEnabled(IEFileMenu, "Item 262", "Properties", false);

            // IE File Print Preview... should be removed.
            string printPreview = "";
            switch (ApplicationDeploymentHelper.GetIEVersion())
            {
                case 6:
                    {
                        printPreview = IEAutomationHelper.GetUnmanagedSubMenuResourceString(Environment.SystemDirectory + @"\browselc.dll", 267, 0, 8);
                        break;
                    }
                case 7:
                    {
                        printPreview = IEAutomationHelper.GetUnmanagedSubMenuResourceString(Environment.SystemDirectory + @"\" + CultureInfo.CurrentUICulture.Name + @"\IEFRAME.dll.mui", 267, 0, 11);
                        // Second chance... try again w/ English
                        if (printPreview == null)
                            printPreview = IEAutomationHelper.GetUnmanagedSubMenuResourceString(Environment.SystemDirectory + @"\en-us\IEFRAME.dll.mui", 267, 0, 10);
                        break;
                    }
                // 8 + 9 behave like 7 for now... 
                default:
                    goto case 7;
            }
            if (printPreview == null) // STILL failed... log it and try english hard coding
            {
                printPreview = "Print Preview";
                GlobalLog.LogEvidence("ERROR: had to fall back to english string!");
            }
            printPreview = printPreview.Replace("&", "");
            success &= ValidateElementRemoved(IEFileMenu, printPreview);

            return success;
        }        
        
        /// <summary>
        /// Test IE View menu.
        /// Test fails if the PresentationHost specific IE menu items are correctly enabled, disabled and removed.
        /// The failed status is logged automatically by DRT.Logger. Test continues even when an test area fails.
        /// </summary>
        public static bool InternetExplorerViewMenu()
        {
            bool success = true;
            AutomationElement IEViewMenu = IEAutomationHelper.ShowIEViewMenu(s_ieWindow); 

            // IE View "Status Bar" should be enabled.
            success &= ValidateIsEnabled(IEViewMenu, "Item 8014", "Status Bar", true);

            // IE View "Status Bar" should be enabled.
            success &= ValidateIsEnabled(IEViewMenu, "Item 8015", "Stop", true);
            
            // IE View "Status Bar" should be enabled.
            success &= ValidateIsEnabled(IEViewMenu, "Item 8016", "Refresh", true);

            // IE View "Status Bar" should be enabled.
            success &= ValidateIsEnabled(IEViewMenu, "Item 8017", "Full Screen", true);
            return success;
        } 
        
        /// <summary>
        /// Test IE Tools menu.
        /// Test fails if the PresentationHost specific IE menu items are correctly enabled, disabled and removed.
        /// The failed status is logged automatically by DRT.Logger. Test continues even when an test area fails.
        /// </summary>
        public static bool InternetExplorerPage(IntPtr topHwnd) 
        {
            if ((ApplicationDeploymentHelper.GetIEVersion() >= 7) && (ApplicationDeploymentHelper.GetIEVersion() < 9))
            {
                bool success = true;
                                
                AutomationElement IEWindow = AutomationElement.FromHandle(topHwnd);

                string pageString = UnmanagedStringHelper.LoadUnmanagedResourceString(@"@%SystemRoot%\system32\" + CultureInfo.CurrentUICulture.Name + @"\ieframe.dll.mui", 1124, 0);
                GlobalLog.LogDebug("Detected " + pageString + " as menu name for IE Page Menu");
                pageString = pageString.Replace("|", "").Replace("&", "").Trim();

                AutomationElement theMenu = IEWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, pageString));

                MTI.Input.MoveToAndClick(theMenu);
                Thread.Sleep(500);
                AutomationElement pageMenu = AutomationElement.RootElement.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Menu));

                int countDown = 20;

                while ((pageMenu == null) && (countDown-- > 0))
                {
                    GlobalLog.LogDebug("Retrying Page Click...");
                    MTI.Input.MoveToAndClick(theMenu);
                    Thread.Sleep(1000);                    
                    pageMenu = AutomationElement.RootElement.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Menu));
                }

                string SaveAsString = UnmanagedStringHelper.LoadUnmanagedResourceString(@"@%SystemRoot%\system32\" + CultureInfo.CurrentUICulture.Name + @"\ieframe.dll.mui", 1124, 7);
                SaveAsString = SaveAsString.Replace("&", "");

                string ViewSrcString = UnmanagedStringHelper.LoadUnmanagedResourceString(@"@%SystemRoot%\system32\" + CultureInfo.CurrentUICulture.Name + @"\ieframe.dll.mui", 1124, 11);
                ViewSrcString = ViewSrcString.Replace("&", "");

                switch (ApplicationDeploymentHelper.GetIEVersion())
                {
                    // For best automate-ability, the IE best practice is "Change Automation Identifier every release"
                    case 7:
                        {
                            // IE Page "Save As" should not be enabled.
                            success &= ValidateIsEnabled(pageMenu, "Item 524", SaveAsString, false);

                            // IE Page "View Source" should be enabled (but it doesnt do anything)
                            success &= ValidateIsEnabled(pageMenu, "Item 528", ViewSrcString, true);
                            break;
                        }
                    case 8:
                        {
                            // IE Page "Save As" should not be enabled.
                            success &= ValidateIsEnabled(pageMenu, "Item 283", SaveAsString, false);

                            // IE Page "View Source" should be enabled (but it doesnt do anything)
                            success &= ValidateIsEnabled(pageMenu, "Item 286", ViewSrcString, true);
                            break;
                        }
                    // IE9's Automation identifiers were the same as IE8 as of 7/29/2010...
                    default:
                        goto case 8;
                }
                return success;
            }
            else
            {
                GlobalLog.LogDebug("Not checking for \"Page\" menu removal since it doesn't exist in IE6 or IE9+...");
                return true;
            }
        }

        /// <summary>
        /// Test IE Tools menu.
        /// Test fails if the PresentationHost specific IE menu items are correctly enabled, disabled and removed.
        /// The failed status is logged automatically by DRT.Logger. Test continues even when an test area fails.
        /// </summary>
        private static bool InternetExplorerTools(IntPtr topHwnd)
        {
            // IE Tool is should be removed -- Item 32960
            if (ApplicationDeploymentHelper.GetIEVersion() == 6)
                return ValidateElementRemoved(AutomationElement.FromHandle(topHwnd),
                   new PropertyCondition(AutomationElement.AutomationIdProperty, "Item 32960"));
            else
            {
                GlobalLog.LogDebug("Not checking for \"Tools\" menu removal since it isnt removed in IE7+...");
                return true;
            }
        }
        
        /// <summary>
        /// Validates the state of an Automation Element for IsEnabled.
        /// Test fails if the state is unexpected.
        /// The failed status is logged automatically by DRT.Logger. Test continues even when an test area fails.
        /// </summary>
        private static bool ValidateIsEnabled(IntPtr topHwnd, string id, string name, bool expectedIsEnabled)
        {
            AutomationElement element = AutomationElement.FromHandle(topHwnd);
            return ValidateIsEnabled(element, id, name, expectedIsEnabled);
        }

        /// <summary>
        /// Validates the state of an Automation Element for IsEnabled.
        /// Test fails if the state is unexpected.
        /// The failed status is logged automatically by DRT.Logger. Test continues even when an test area fails.
        /// </summary>
        private static bool ValidateIsEnabled(AutomationElement element, string id, string name, bool expectedIsEnabled)
        {
            AutomationElement ieElement = null;
            if (id != null)
            {
                // Locate the IE Automation element.
                ieElement = IEAutomationHelper.WaitForElementWithAutomationId(element, id, 25);
            }
            else
            {
                // Locate the IE Automation element.
                ieElement = IEAutomationHelper.WaitForElementWithName(element, name, 25);
            }
            // Log a Pass or Fail
            return LogValidate(ieElement, id, name, expectedIsEnabled);
        }

        /// <summary>
        /// Validates that an element exists but not any other info about its state.
        /// </summary>
        private static bool ValidateExists(IntPtr topHwnd, string id, string name)
        {
            AutomationElement element = AutomationElement.FromHandle(topHwnd);
            return ValidateExists(element, id, name);
        }

        /// <summary>
        /// Validates the state of an Automation Element for IsEnabled.
        /// Test fails if the state is unexpected.
        /// The failed status is logged automatically by DRT.Logger. Test continues even when an test area fails.
        /// </summary>
        private static bool ValidateExists(AutomationElement element, string id, string name)
        {
            AutomationElement ieElement = null;
            if (id != null)
            {
                // Locate the IE Automation element.
                ieElement = IEAutomationHelper.WaitForElementWithAutomationId(element, id, 25);
            }
            else
            {
                // Locate the IE Automation element.
                ieElement = IEAutomationHelper.WaitForElementWithName(element, name, 25);
            }
            if (ieElement != null)
            {
                GlobalLog.LogEvidence("IE Menu: " + name + " existed (Not validating enabled-ness) ");
                return true;
            }
            else
            {
                GlobalLog.LogEvidence("IE Menu: " + name + " did not exist! (Bad) ");
                return false;
            }            
        }

        private static bool LogValidate(AutomationElement ieElement, string id, string name, bool expectedIsEnabled)
        {
            if (ieElement == null)
            {
                GlobalLog.LogDebug("Oops, element found by " + name + " was null!");
                return false;
            }

            // Test Expects IsEnabled property to be True and Name value to match.
            if (expectedIsEnabled)
            {
                // Validate IsEnabled value (true or false)
                // TestServices.Trace("IE Menu: {0}", name, "Expected: {1}", expectedIsEnabled, "Actual: {2}", ieElement.Current.IsEnabled);
                // TestServices.Assert(ieElement.Current.IsEnabled, "IE Menu: " + name + " Expeced to be true (enabled).");
                if (ieElement.Current.IsEnabled)
                {
                    if (id == null)
                    {
                        GlobalLog.LogEvidence("IE Menu: " + name + " was enabled (Correct) ");
                    }
                    else
                    {
                        GlobalLog.LogEvidence("IE Menu: " + id + " was enabled (Correct) ");
                    }
                    return true;
                }
                else
                {
                    GlobalLog.LogEvidence("IE Menu: " + id + " was not enabled (Bad) ");
                    return false;
                }
            }

            // Test Expects IsEnabled property to be False
            else
            {
                if (!ieElement.Current.IsEnabled)
                {
                    if (id == null)
                    {
                        GlobalLog.LogEvidence("IE Menu: " + name + " was not enabled (Correct) ");
                    }
                    else
                    {
                        GlobalLog.LogEvidence("IE Menu: " + id + " was not enabled (Correct) ");
                    }
                    return true;
                }
                else
                {
                    if (id == null)
                    {
                        GlobalLog.LogEvidence("IE Menu: " + name + " was enabled but wasn't supposed to be (Bad) ");
                    }
                    else
                    {
                        GlobalLog.LogEvidence("IE Menu: AutoID: " + id + " was enabled but wasn't supposed to be (Bad) ");
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// Validates an Automation Element by Name or ID has been removed.
        /// </summary>
        private static bool ValidateElementRemoved(AutomationElement parentElement, string name)
        {
            GlobalLog.LogDebug("Validating element " + name + " is removed ...");
            return ValidateElementRemoved(parentElement, new PropertyCondition(AutomationElement.NameProperty, name));
        }

        /// <summary>
        /// Validates an Automation Element by Name or ID has been removed.
        /// </summary>
        private static bool ValidateElementRemoved(AutomationElement parentElement, PropertyCondition cond)
        {
            if (parentElement == null)
            {
                GlobalLog.LogDebug("Error: ValidateElementRemoved() called with null parent element... defaulting to root element");
                parentElement = AutomationElement.RootElement;
            }

            // TestServices.Trace("IE Menu: {0}", name, "Expected: NullReferenceException.  Menu should be removed.");
            AutomationElement result = null;
            int i = 0;

            while (i <= 25)
            {
                result = parentElement.FindFirst(TreeScope.Descendants, cond);
                System.Threading.Thread.Sleep(15);
                i++;
            }
            // Validation Fails if we find the element.  PresentationHost disables these menus in IE for security reasons.
            if (result != null)
            {
                // Logger.failedCount++;
                // TestServices.Trace("IE Menu: {0} ", name, "Expected: NullReferenceException.  Actual: ITEM WAS FOUND.");
                GlobalLog.LogDebug("Found element when it should not have been... (Bad)");
                return false;
            }
            else
            {
                GlobalLog.LogDebug("Didn't find element (Correct)");
                return true;
            }
        }        
    }

    /// <summary>
    /// UI handler
    /// </summary>
    public class CtrlSHandler : UIHandler
    {
        public override UIHandlerAction HandleWindow(System.IntPtr topHwnd, System.IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            AutomationElement IEWindow = AutomationElement.FromHandle(topHwnd);
            IEWindow.SetFocus();
            Thread.Sleep(5000);

            IEAutomationHelper.ClickIEAddrBarTwice(IEWindow); 

            GlobalLog.LogEvidence("Opening IE File Menu...");
            AutomationElement FileMenu = IEAutomationHelper.ShowIEFileMenu(IEWindow);
            GlobalLog.LogEvidence("Pressing \"Save\"... ");

            // Removed Invoke() call 
            // This is unlikely to get fixed in RTM
            // Direct clicking works but is a bit less reliable since it depends on visibility.
            // IEAutomationHelper.InvokeElementViaAutomationId(FileMenu, "Item 257", 15);
            IEAutomationHelper.ClickCenterOfElementById(FileMenu, "Item 257");
            GlobalLog.LogEvidence("Clicked \"Save\"... if App's title does not change there may be an issue with the feature or test");

            return UIHandlerAction.Handled;
        }
    }

    /// <summary>
    /// Exercises select/cut/copy/paste from IE menus
    /// </summary>
    public class BrowserMenuCommandHandler : UIHandler
    {
        private static UIHandlerAction s_result = UIHandlerAction.Abort;

        public override UIHandlerAction HandleWindow(IntPtr topLevelhWnd, IntPtr hwnd, Process process, string title, UIHandlerNotification notification)
        {
             if ((SystemInformation.Current.MajorVersion == 5) && (SystemInformation.Current.MinorVersion == 2))
             {
                 TestLog.Current.Result = TestResult.Ignore;
                 GlobalLog.LogEvidence("Setting result to Ignore as SendInput and Invoke() do not work on IE on Server 03 OSes.");
                 return UIHandlerAction.Abort;
             }
             else
             {
                 AutomationElement IEWindow = AutomationElement.FromHandle(topLevelhWnd);
                 IEWindow.SetFocus();

                 ParameterizedThreadStart workerThread = new ParameterizedThreadStart(handleWindowNewThread);
                 Thread thread = new Thread(workerThread);
                 thread.SetApartmentState(ApartmentState.STA);
                 thread.Start((object)IEWindow);
                 thread.Join();

                 return s_result;
             }
        }

        // Do this to access Clipboard data.
        private static void handleWindowNewThread(object theWindow)
        {
            bool testPasses = true;

            // We're going to make sure copying occurred, so empty clipboard...
            Clipboard.Clear();

            // Load all the strings (for loc runs) for edit menu items
            string pHostDllPath = Microsoft.Test.Diagnostics.SystemInformation.Current.FrameworkWpfPath + CultureInfo.CurrentUICulture.Name + ApplicationDeploymentHelper.PresentationHostDllMui;
            if (!File.Exists(pHostDllPath))
            {
                pHostDllPath = Microsoft.Test.Diagnostics.SystemInformation.Current.FrameworkWpfPath + @"\en-us" + ApplicationDeploymentHelper.PresentationHostDllMui;
            }
            string selectAll = IEAutomationHelper.GetUnmanagedSubMenuResourceString(pHostDllPath, 2000, 0, 4);
            selectAll = selectAll.Replace("&", "");
            selectAll = selectAll.Substring(0, selectAll.IndexOf('\t'));

            string paste = IEAutomationHelper.GetUnmanagedSubMenuResourceString(pHostDllPath, 2000, 0, 2);
            paste = paste.Replace("&", "");
            paste = paste.Substring(0, paste.IndexOf('\t'));

            string copy = IEAutomationHelper.GetUnmanagedSubMenuResourceString(pHostDllPath, 2000, 0, 1);
            copy = copy.Replace("&", "");
            copy = copy.Substring(0, copy.IndexOf('\t'));

            string cut = IEAutomationHelper.GetUnmanagedSubMenuResourceString(pHostDllPath, 2000, 0, 0);
            cut = cut.Replace("&", "");
            cut = cut.Substring(0, cut.IndexOf('\t'));

            GlobalLog.LogEvidence("Using Select All = " + selectAll + ", Paste=" + paste + ", Copy=" + copy + "Cut=" + cut);

            AutomationElement IEWindow = (AutomationElement) theWindow;
            IEWindow.SetFocus();

            PropertyCondition condition = new PropertyCondition(AutomationElement.AutomationIdProperty, "refreshTestTxtBox");

            AutomationElement testTextBox = IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "refreshTestTxtBox", 10);

            // Type some stuff into the textbox:
            testTextBox.SetFocus();

            // One last check that it's empty... 
            testPasses &= checkClipboardHasText(false);

            GlobalLog.LogEvidence("Typing into text box...");
            MTI.Input.SendUnicodeString("copy test", 1, 15);
            
            testPasses &= correctTextBoxContent(testTextBox, "copy test");

            //Click address bar twice for workaround
            IEAutomationHelper.ClickIEAddrBarTwice(IEWindow); 

            AutomationElement editMenu = IEAutomationHelper.ShowIEEditMenu(IEWindow);

            GlobalLog.LogEvidence("Pressing \"Select All\"...");
            AutomationElement selectAllMenuItem = IEAutomationHelper.WaitForElementWithName(editMenu, selectAll, 10);
            // Using Win32 APIs exclusively to ensure the right codepath gets hit...
            MTI.Input.MoveToAndClick(selectAllMenuItem);

            if ((SystemInformation.Current.MajorVersion == 6) && (SystemInformation.Current.MinorVersion == 1))
            {
                editMenu = IEAutomationHelper.ShowIEEditMenu(IEWindow);

                GlobalLog.LogEvidence("Pressing \"Select All\"  (IE Menu Merging - IE edit menu commands disabled on first use for Xbaps on Win7) ...");
                selectAllMenuItem = IEAutomationHelper.WaitForElementWithName(editMenu, selectAll, 10);
                // Using Win32 APIs exclusively to ensure the right codepath gets hit...
                MTI.Input.MoveToAndClick(selectAllMenuItem);
            }

            GlobalLog.LogEvidence("Pressing \"Copy\"...");
            // Has to happen anew every time, since menus are technically ephemeral windows.
            editMenu = IEAutomationHelper.ShowIEEditMenu(IEWindow);

            AutomationElement copyMenuItem = IEAutomationHelper.WaitForElementWithName(editMenu, copy, 10);
            // Using Win32 APIs exclusively to ensure the right codepath gets hit...
            MTI.Input.MoveToAndClick(copyMenuItem);

            testPasses &= correctTextBoxContent(testTextBox, "copy test");
            testPasses &= checkClipboardHasText(true);

            editMenu = IEAutomationHelper.ShowIEEditMenu(IEWindow);

            GlobalLog.LogEvidence("Pressing \"Select All\" (For Cut operation)...");
            selectAllMenuItem = IEAutomationHelper.WaitForElementWithName(editMenu, selectAll, 10);
            // Using Win32 APIs exclusively to ensure the right codepath gets hit...
            MTI.Input.MoveToAndClick(selectAllMenuItem);

            // Has to happen anew every time, since menus are technically ephemeral windows.
            editMenu = IEAutomationHelper.ShowIEEditMenu(IEWindow);

            GlobalLog.LogEvidence("Pressing \"Cut\" ...");
            AutomationElement cutMenuItem = IEAutomationHelper.WaitForElementWithName(editMenu, cut, 10);
            // Using Win32 APIs exclusively to ensure the right codepath gets hit...
            MTI.Input.MoveToAndClick(cutMenuItem);

            testPasses &= correctTextBoxContent(testTextBox, "");
            testPasses &= checkClipboardHasText(true);

            // Has to happen anew every time, since menus are technically ephemeral windows.
            editMenu = IEAutomationHelper.ShowIEEditMenu(IEWindow);

            GlobalLog.LogEvidence("Pressing \"Paste\" ...");
            AutomationElement pasteMenuItem = IEAutomationHelper.WaitForElementWithName(editMenu, paste, 10);
            // Using Win32 APIs exclusively to ensure the right codepath gets hit...
            MTI.Input.MoveToAndClick(pasteMenuItem);

            testPasses &= correctTextBoxContent(testTextBox, "copy test");

            if (testPasses)
            {
                TestLog.Current.Result = TestResult.Pass;
                GlobalLog.LogEvidence("Passed : Successfully exercised all Edit cut/copy/paste/select functions through the browser.");
            }
            else
            {
                TestLog.Current.Result = TestResult.Fail;
                GlobalLog.LogEvidence("Failed : Issue hit performing IE Edit menu behaviors.  See log.");
            }
        }

        private static bool checkClipboardHasText(bool itShould)
        {
            // Allows value to settle
            Thread.Sleep(2000);

            if (itShould)
            {
                if (Clipboard.ContainsText())
                {
                    GlobalLog.LogEvidence("Clipboard had text in it as expected.");
                    return true;
                }
                else
                {
                    GlobalLog.LogEvidence("Clipboard had no text in it when expected!");
                    return false;
                }
            }
            else
            {
                if (Clipboard.ContainsText())
                {
                    GlobalLog.LogEvidence("Clipboard had text in it when not expected to!");
                    return false;
                }
                else
                {
                    GlobalLog.LogEvidence("Clipboard had no text in it as expected.");
                    return true;
                }
            }
        }

        private static bool correctTextBoxContent(AutomationElement testTextBox, string content)
        {
            // Allows value to settle
            Thread.Sleep(2000);
            ValuePattern valuePattern = testTextBox.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
            GlobalLog.LogEvidence("Text Box contains: " + valuePattern.Current.Value + ", Expected: " + content);
            return (valuePattern.Current.Value.ToLowerInvariant() == content.ToLowerInvariant());
        }
    }

}
