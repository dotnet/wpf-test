// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Input;
using System.Windows.Interop;
using Microsoft.Test.Loaders;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Win32;
using MTI = Microsoft.Test.Input;
using System.Globalization;


namespace Microsoft.Windows.Test.Client.AppSec.BVT
{
    /// <summary>
    /// Different methods of interacting with the dialog
    /// </summary>
    public enum DialogInteractionMethod
    {
        TypeFileNameAndEnter,
        TypeFileNameAndCancel,
        TypeFileNameAndSave,
        TypeFileNameAndOpen,
        CloseWindowWithXButton
    }

    public class CommonFileDialogHandler : UIHandler
    {
        // ***NOTE*** These button names are only used for logging purposes, not for UIAutomation.  UIAutomation is done entirely using IDs, so don't panic.

        private static String s_CANCELBUTTONNAME = "Cancel";
        private static String s_SAVEBUTTONNAME = "Save";
        private static String s_OPENBUTTONNAME = "Open";
        private static String s_CLOSEBUTTONNAME = "Close";
        private static String s_OPENFOLDERBUTTONNAME = "Select Folder";

        // Modal MessageBox button names
        private static String s_YESBUTTONNAME = "Yes";
        private static String s_OKBUTTONNAME = "OK";

        private static String s_OK_AUTOID = "1";
        private static String s_CANCEL_AUTOID = "2";
        private static String s_CLOSE_AUTOID = "Close";
        // Modal MessageBox button IDs
        private static String s_MBYES_AUTOID = "6";
        private static String s_MBOK_AUTOID = "2";

        private static string s_openTextBoxId = "1148";
        private static string s_saveTextBoxId = "1148";
        private static string s_folderTextBoxId = "1152";
        private static string s_vistaSaveTextBoxId = "1001";

        #region const class names
        /// <summary>
        /// IE class name (for automation)
        /// </summary>
        private const String IECLASSNAME = "IEFrame";

        /// <summary>
        /// Class name for the Open|SaveFileDialog window
        /// </summary>
        private const String FILEDIALOGCLASSNAME = "#32770";
        #endregion

        // Number of milliseconds to wait before next keystroke
        private const int KEYWAIT = 50;

        private bool _isBrowserHosted = false;

        // Settable attributes
        public DialogInteractionMethod DialogAction = DialogInteractionMethod.TypeFileNameAndEnter;
        public String                  Dialog       = String.Empty;
        public String                  TestType     = "TypeFileNameAndEnter";
        public String                  FileName     = String.Empty;

        #region Dialog Functionality
        private bool CloseWindowWithXButton(IntPtr hWnd)
        {
            AutomationElement fileDialog = GetWindow(hWnd, FILEDIALOGCLASSNAME);
            if (fileDialog == null)
                return false;

            Output("Clicking on the Close button in file dialog");
            ClickButtonUsingId(s_CLOSEBUTTONNAME, s_CLOSE_AUTOID, fileDialog);

            // Check that FileDialog is no longer showing...
            Wait();
            fileDialog = GetWindow(hWnd, FILEDIALOGCLASSNAME);
            if (fileDialog != null)
            {
                Output("FileDialog still shows up.");
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// VerifyIsThreadModal - Verifies ComponentDispatcher.IsThreadModal is true when a FileDialog is up.
        /// </summary>
        /// <param name="hWnd">hWnd of the app/IE window</param>
        /// <returns>true if ComponentDispatcher.IsThreadModal is true; false otherwise</returns>
        private bool VerifyIsThreadModal(IntPtr hwnd)
        {
            bool retval = true;

            AutomationElement fileDialog = GetWindow(hwnd, FILEDIALOGCLASSNAME);
            if (fileDialog == null)
            {
                Log.Current.CurrentVariation.LogMessage("FAIL - Did not find file dialog.");
                return false;
            }

            if (ComponentDispatcher.IsThreadModal)
            { 
                Output("ComponentDispatcher.IsThreadModal returned true as expected");
            }
            else
            {
                retval = false;
                Output("Error: ComponentDispatcher.IsThreadModal returned false.  Expected: true");
            }

            Output("Clicking on the Close button in file dialog");
            ClickButtonUsingId(s_CLOSEBUTTONNAME, s_CLOSE_AUTOID, fileDialog);

            return retval;
        }

        /// <summary>
        /// VerifyVistaOnVista - Will only be run on Vista.  Verifies we saw the Vista-style dialog, and closes the dialog.
        /// </summary>
        /// <param name="hWnd">hWnd of the app/IE window</param>
        /// <returns>true if the dialog is a Vista dialog; false otherwise</returns>
        private bool VerifyVistaOnVista(IntPtr hwnd)
        {
            bool retval = true;

            AutomationElement fileDialog = GetWindow(hwnd, FILEDIALOGCLASSNAME);
            if (fileDialog == null)
            {
                Log.Current.CurrentVariation.LogMessage("FAIL - Did not find file dialog.");
                return false;
            }

            if (Environment.OSVersion.Version.Major < 6)
            { 
                Log.Current.CurrentVariation.LogMessage("OS is pre-Vista, pass.");
            }
            else //we're on Vista or newer, should see Vista-style dialog
            {
                // Win7 uses a different name for the custom places pane
                bool isWin7OrGreater = Environment.OSVersion.Version >= new Version(6, 1);

                AndCondition vistaCommonPlacesHost = new AndCondition(new PropertyCondition(AutomationElement.AutomationIdProperty, isWin7OrGreater ? "ProperTreeHost" : "CommonPlacesHost"),
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Pane));

                AutomationElement commonPlacesPane = fileDialog.FindFirst(TreeScope.Descendants, vistaCommonPlacesHost);
                if (commonPlacesPane == null)
                {
                    Log.Current.CurrentVariation.LogMessage("Could not find Common Places pane - dialog is not Vista-style");
                    retval = false;
                }
            }

            Output("Clicking on the Close button in file dialog");
            ClickButtonUsingId(s_CLOSEBUTTONNAME, s_CLOSE_AUTOID, fileDialog);

            return retval;
        }

        /// <summary>
        /// OPEN:  On Vista, clicks Test, types in the given file name and then presses the Open button.  Otherwise just closes the dialog.
        /// </summary>
        /// <param name="param">String representing the file name to type into the textbox</param>
        /// <param name="hWnd">hWnd of the app/IE window</param>
        /// <returns>true if we were able to click Test, type the filename, and click Open successfully; false otherwise</returns>
        private bool VerifyOpenDialogCustomPlace(String[] param, IntPtr hwnd)
        {
            bool retval = true;

            AutomationElement fileDialog = GetWindow(hwnd, FILEDIALOGCLASSNAME);
            if (fileDialog == null)
            {
                Log.Current.CurrentVariation.LogMessage("FAIL - Did not find file dialog.");
                return false;
            }

            if (Environment.OSVersion.Version.Major < 6)
            { 
                Log.Current.CurrentVariation.LogMessage("OS is pre-Vista, pass.");
                Output("Clicking on the Close button in file dialog");
                ClickButtonUsingId(s_CLOSEBUTTONNAME, s_CLOSE_AUTOID, fileDialog);
            }
            else //we're on Vista or newer, should see Vista-style dialog
            {
                // Win7 uses a tree instead of a menu for the custom places
                bool isWin7OrGreater = Environment.OSVersion.Version >= new Version(6, 1);

                AndCondition vistaCustomPlace = new AndCondition(new PropertyCondition(AutomationElement.NameProperty, "test"),
                    new PropertyCondition(AutomationElement.ControlTypeProperty, isWin7OrGreater ? ControlType.TreeItem : ControlType.MenuItem));

                AutomationElement customPlaceItem = fileDialog.FindFirst(TreeScope.Descendants, vistaCustomPlace);
                if (customPlaceItem == null)
                {
                    Log.Current.CurrentVariation.LogMessage("Could not find custom place named Test");
                    retval = false;
                }
                MTI.Input.MoveToAndClick(customPlaceItem);
                Wait();

                return TypeFileNameAndPressButton(param, s_OPENBUTTONNAME, s_OK_AUTOID, s_openTextBoxId, hwnd);
            }

            return retval;
        }

        /// <summary>
        /// SAVE:  On Vista, clicks Pictures, types in the given file name and then presses the Save button.  Otherwise just closes the dialog.
        /// </summary>
        /// <param name="param">String representing the file name to type into the textbox</param>
        /// <param name="hWnd">hWnd of the app/IE window</param>
        /// <returns>true if we were able to click Pictures, type the filename, and click Save successfully; false otherwise</returns>
        private bool VerifySaveDialogCustomPlace(String[] param, IntPtr hwnd)
        {
            bool retval = true;

            AutomationElement fileDialog = GetWindow(hwnd, FILEDIALOGCLASSNAME);
            if (fileDialog == null)
            {
                Log.Current.CurrentVariation.LogMessage("FAIL - Did not find file dialog.");
                return false;
            }

            if (Environment.OSVersion.Version.Major < 6) 
            { 
                Log.Current.CurrentVariation.LogMessage("OS is pre-Vista, pass.");
                Output("Clicking on the Close button in file dialog");
                ClickButtonUsingId(s_CLOSEBUTTONNAME, s_CLOSE_AUTOID, fileDialog);
            }
            else if (!CultureInfo.CurrentUICulture.Name.ToLowerInvariant().Equals("en-us"))
            { 
                Output("Test cannot pass on UI Culture " + CultureInfo.CurrentUICulture.Name + " due to needing to look up the correct name for the My Pictures folder");
                // Have to do this because of the hideously implemented HandleWindow doesn't understand logging except for certain test names.  Crazy.
                TestLog.Current.Result = TestResult.Ignore;
                ClickButtonUsingId(s_CLOSEBUTTONNAME, s_CLOSE_AUTOID, fileDialog);
            }
            else //we're on Vista or newer, should see Vista-style dialog
            {
                // Win7 uses a tree instead of a menu for the custom places
                bool isWin7OrGreater = Environment.OSVersion.Version >= new Version(6, 1);

                //Get localized text for "Pictures"
                string pictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                pictures = pictures.Substring(pictures.LastIndexOf(Path.DirectorySeparatorChar) + 1);

                Output("Using Pictures = " + pictures);

                AndCondition vistaCustomPlace = new AndCondition(new PropertyCondition(AutomationElement.NameProperty, pictures),
                    new PropertyCondition(AutomationElement.ControlTypeProperty, isWin7OrGreater ? ControlType.TreeItem : ControlType.MenuItem));

                AutomationElement customPlaceItem = fileDialog.FindFirst(TreeScope.Descendants, vistaCustomPlace);
                if (customPlaceItem == null)
                {
                    Log.Current.CurrentVariation.LogMessage("Could not find place named Pictures");
                    retval = false;
                }
                MTI.Input.MoveToAndClick(customPlaceItem);
                Wait();

                return TypeFileNameAndPressButton(param, s_SAVEBUTTONNAME, s_OK_AUTOID, s_vistaSaveTextBoxId, hwnd);
            }

            return retval;
        }

        /// <summary>
        /// OPEN|SAVE:  Types in the given file name and then presses the Open/Save button
        /// </summary>
        /// <param name="param">String representing the file name to type into the textbox</param>
        /// <param name="hWnd">hWnd of the app/IE window</param>
        /// <returns>true if we were able to type and click Open/Save successfully; false otherwise</returns>
        private bool TypeFileNameAndOk(String[] param, IntPtr hWnd)
        {
            if (Dialog.ToLowerInvariant().Contains("save"))
            {
                if (Environment.OSVersion.Version.Major < 6)
                { 
                    return TypeFileNameAndPressButton(param, s_SAVEBUTTONNAME, s_OK_AUTOID, s_saveTextBoxId, hWnd);
                }
                else //we're on Vista or newer, should see Vista-style dialog
                {
                    //conditional for running against WPF4 vs 3.  V3 should use "saveTextBoxId".  V4 on Vista needs "vistaSaveTextBoxId".
#if TESTBUILD_CLR20
                    return TypeFileNameAndPressButton(param, s_SAVEBUTTONNAME, s_OK_AUTOID, s_saveTextBoxId, hWnd);
#else
                    return TypeFileNameAndPressButton(param, s_SAVEBUTTONNAME, s_OK_AUTOID, s_vistaSaveTextBoxId, hWnd);
#endif
                }
            }
            else if (Dialog.ToLowerInvariant().Contains("folder"))
            {
                return TypeFileNameAndPressButton(param, s_OPENFOLDERBUTTONNAME, s_OK_AUTOID, s_folderTextBoxId, hWnd);
            }
            else if (Dialog.ToLowerInvariant().Contains("open"))
            {
                return TypeFileNameAndPressButton(param, s_OPENBUTTONNAME, s_OK_AUTOID, s_openTextBoxId, hWnd);
            }
            else
            {
                // Invalid choice.  Return false.
                return false;
            }
        }

        /// <summary>
        /// OPEN|SAVE:  Types in the given file name and then presses the cancel button
        /// </summary>
        /// <param name="param">String representing the file name to type into the textbox</param>
        /// <param name="textBoxId">automation ID for the file name text box</param>
        /// <param name="hWnd">hWnd of the app/IE window</param>
        /// <returns>true if we were able to type and Cancel successfully; false otherwise</returns>
        private bool TypeFileNameAndCancel(String[] param, IntPtr hWnd)
        {
            if (Dialog.ToLowerInvariant().Contains("save"))
            {
                if (Environment.OSVersion.Version.Major < 6)
                { 
                    return TypeFileNameAndPressButton(param, s_CANCELBUTTONNAME, s_CANCEL_AUTOID, s_saveTextBoxId, hWnd);
                }
                else //we're on Vista or newer, should see Vista-style dialog
                {
                    //conditional for running against WPF4 vs 3.  V3 should use "saveTextBoxId".  V4 on Vista needs "vistaSaveTextBoxId".
#if TESTBUILD_CLR20
                    return TypeFileNameAndPressButton(param, s_CANCELBUTTONNAME, s_CANCEL_AUTOID, s_saveTextBoxId, hWnd);
#else
                    return TypeFileNameAndPressButton(param, s_CANCELBUTTONNAME, s_CANCEL_AUTOID, s_vistaSaveTextBoxId, hWnd);
#endif
                }
            }
            else if (Dialog.ToLowerInvariant().Contains("folder"))
            {
                return TypeFileNameAndPressButton(param, s_CANCELBUTTONNAME, s_CANCEL_AUTOID, s_folderTextBoxId, hWnd);
            }
            else if (Dialog.ToLowerInvariant().Contains("open"))
            {
                return TypeFileNameAndPressButton(param, s_CANCELBUTTONNAME, s_CANCEL_AUTOID, s_openTextBoxId, hWnd);
            }
            else
            {
                // Invalid choice.  Return false.
                return false;
            }
        }

        /// <summary>
        /// OPEN|SAVE:  Types in the given file name and presses the button with the given name
        /// </summary>
        /// <param name="param">String representing the file name to type into the textbox</param>
        /// <param name="buttonName">String representing the button name/label</param>
        /// <param name="btnAutoId">AutomationID of the button to press</param>
        /// <param name="textBoxId">automation ID for the file name text box</param>
        /// <param name="hWnd">hWnd of the app/IE window</param>
        /// <returns>true if we were able to type and press button successfully; false otherwise</returns>
        private bool TypeFileNameAndPressButton(String[] param, string buttonName, string btnAutoId, string textBoxId, IntPtr hWnd)
        {
            AutomationElement fileDialog = GetWindow(hWnd, FILEDIALOGCLASSNAME);
            if (fileDialog == null)
                return false;

            AndCondition isFileNameTextBox = new AndCondition(new PropertyCondition(AutomationElement.AutomationIdProperty, textBoxId),
                new PropertyCondition(AutomationElement.ClassNameProperty, "Edit"));

            AutomationElement tbFileName = fileDialog.FindFirst(TreeScope.Descendants, isFileNameTextBox);
            if (tbFileName == null)
            {
                Output("Couldn't find the file name text box");
                return false;
            }

            tbFileName.SetFocus();
            
            // Sleep to make sure that file name text box has focus
            Wait();

            for (int i = 0; i < param.Length; i++)
            {
                Output("Typing string '" + param[i] + "' in the file name TextBox");
                if (param.Length == 1)
                    TypeString(param[i].ToLowerInvariant());
                else
                    TypeString("\"" + param[i].ToLowerInvariant() + "\" ");
            }

            // Click on given button
            Output("Clicking on the " + buttonName + " button");
            ClickButtonUsingId(buttonName, btnAutoId, fileDialog);

            // Check if there's a MessageBox blocking the dialog from closing.  If so, deal with it.
            Wait();
            if (!_isBrowserHosted && !DetectModalMessageBox(fileDialog))
            {
                // Check that FileDialog is no longer showing...
                Wait();
                fileDialog = GetWindow(hWnd, FILEDIALOGCLASSNAME);
                if (fileDialog != null)
                {
                    Output("FileDialog still shows up.");
                    return false;
                }
                else
                    Output("FileDialog has closed");
            }

            return true;
        }

        private bool TypeFileNameAndEnter(String[] param, IntPtr hWnd)
        {
            string textBoxId = "";
            AutomationElement fileDialog = GetWindow(hWnd, FILEDIALOGCLASSNAME);
            if (fileDialog == null)
                return false;

            if (Dialog.ToLowerInvariant().Contains("save"))
            {
                if (Environment.OSVersion.Version.Major < 6)
                { 
                    textBoxId = s_saveTextBoxId;
                }
                else //we're on Vista or newer, should see Vista-style dialog
                {
#if TESTBUILD_CLR20
                    return TypeFileNameAndPressButton(param, s_SAVEBUTTONNAME, s_OK_AUTOID, s_saveTextBoxId, hWnd);
#else
                    return TypeFileNameAndPressButton(param, s_SAVEBUTTONNAME, s_OK_AUTOID, s_vistaSaveTextBoxId, hWnd);
#endif
                }
            }
            else if (Dialog.ToLowerInvariant().Contains("folder"))
            {
                textBoxId = s_folderTextBoxId;
            }
            else if (Dialog.ToLowerInvariant().Contains("open"))
            {
                textBoxId = s_openTextBoxId;
            }

            AndCondition isFileNameTextBox = new AndCondition(new PropertyCondition(AutomationElement.AutomationIdProperty, textBoxId),
                new PropertyCondition(AutomationElement.ClassNameProperty, "Edit"));
            AutomationElement tbFileName = fileDialog.FindFirst(TreeScope.Descendants, isFileNameTextBox);

            if (tbFileName == null)
            {
                Output("Couldn't find the file name text box");
                return false;
            }

            tbFileName.SetFocus();

            // Sleep to make sure that file name text box has focus
            Wait();

            for (int i = 0; i < param.Length; i++)
            {
                Output("Typing string '" + param[i] + "' in the file name TextBox");
                if (param.Length == 1)
                    TypeString(param[i].ToLowerInvariant());
                else
                    TypeString("\"" + param[i].ToLowerInvariant() + "\" ");
            } 
            
            Output("Typing ENTER in the file name TextBox");
            TypeKey(Key.Enter);

            // Check if there's a MessageBox blocking dialog from closing.  If so, deal with it.
            // If not, check if FileDialog is still showing.
            Wait();
            if (!_isBrowserHosted && !DetectModalMessageBox(fileDialog))
            {
                // Check that FileDialog is no longer showing...
                Wait();
                fileDialog = GetWindow(hWnd, FILEDIALOGCLASSNAME);
                if (fileDialog != null)
                {
                    Output("FileDialog still shows up.");
                    return false;
                }
                else
                    Output("FileDialog has closed");
            }

            return true;
        }

        private bool ClickButtonUsingId(string buttonName, string automationId, AutomationElement searchFrom)
        {

            AndCondition findButtonCondition = new AndCondition(
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button),
                new PropertyCondition(AutomationElement.IsInvokePatternAvailableProperty, true),
                new PropertyCondition(AutomationElement.AutomationIdProperty, automationId) );

            AutomationElement targetButton = searchFrom.FindFirst(TreeScope.Descendants, findButtonCondition);
            if (targetButton == null)
            {
                Output("Could not find the button " + buttonName + " using automationId " + automationId);
                Output("Trying to find a Pane with the same ID.  For some reason Vista SplitButtons are Panes.");

                findButtonCondition = new AndCondition(
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Pane),
                    new PropertyCondition(AutomationElement.AutomationIdProperty, automationId) );

                targetButton = searchFrom.FindFirst(TreeScope.Descendants, findButtonCondition);

                if (targetButton == null)
                {
                    Output("Could not find the Pane " + buttonName + " using automationId " + automationId);
                    return false;
                }
            }
            
            Output("Clicking on the button '" + buttonName + "' with id '" + automationId + "'");
            MTI.Input.MoveToAndClick(targetButton);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="expectedClassName"></param>
        /// <returns></returns>
        private AutomationElement GetWindow(IntPtr hWnd, String expectedClassName)
        {
            AutomationElement windowToFind = null;
            String windowClass = String.Empty;

            try
            {
                if (hWnd != IntPtr.Zero)
                    windowToFind = AutomationElement.FromHandle(hWnd);
            }
            catch (Exception)
            {
                GlobalLog.LogEvidence("Caught exception doing AutomationElement.FromHandle");
                windowToFind = null;
            }
            finally
            {
                if (windowToFind != null)
                    windowClass = windowToFind.GetCurrentPropertyValue(AutomationElement.ClassNameProperty) as String;

                if ( windowToFind == null || windowClass.Equals(String.Empty) || 
                     !(windowClass.ToLowerInvariant().Equals(expectedClassName.ToLowerInvariant())) )
                {
                    if(windowToFind == null) GlobalLog.LogEvidence("windowToFind = null");
                    if(windowClass.Equals(String.Empty)) GlobalLog.LogEvidence("windowClass.Equals(String.Empty)");
                    if(!windowClass.ToLowerInvariant().Equals(expectedClassName.ToLowerInvariant()))
                    {
                        GlobalLog.LogEvidence("class name '" + windowClass.ToLowerInvariant() + "' didn't match expected '" + expectedClassName.ToLowerInvariant() + "'");
                    }

                    //We are dealing with a wrong window
                    PropertyCondition isWindow = new PropertyCondition(AutomationElement.ClassNameProperty, expectedClassName);
                    windowToFind = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, isWindow);
                }
            }

            if (windowToFind != null)
            {
                windowClass = windowToFind.GetCurrentPropertyValue(AutomationElement.ClassNameProperty) as String;

                if ( windowClass == null || !(windowClass.ToLowerInvariant().Equals(expectedClassName.ToLowerInvariant())) )
                    windowToFind = null;
            }

            if (windowToFind == null)
                Log.Current.CurrentVariation.LogMessage("Couldn't find the file dialog.");

            return windowToFind;
        }

        private bool DetectModalMessageBox(AutomationElement fileDialog)
        {
            object patternObject;
            fileDialog.TryGetCurrentPattern(WindowPattern.Pattern, out patternObject);
            WindowPattern wpFileDialog = patternObject as WindowPattern;

            if (wpFileDialog == null)
                return false;

            // if the FileDialog's WindowPattern says that it's been blocked
            Output("Checking to see if FileDialog is blocked by another modal window");
            if (wpFileDialog.Current.WindowInteractionState.Equals(WindowInteractionState.BlockedByModalWindow))
            {
                // Find the dialog child of the FileDialog that is ready for UserInteraction 
                Output("FileDialog is currently blocked.");
                AndCondition isMessageBox = new AndCondition(
                    new PropertyCondition(AutomationElement.ClassNameProperty, FILEDIALOGCLASSNAME),
                    new PropertyCondition(WindowPattern.WindowInteractionStateProperty, WindowInteractionState.ReadyForUserInteraction));

                AutomationElement msgBox = fileDialog.FindFirst(TreeScope.Descendants, isMessageBox);
                if (msgBox == null)
                    return false;

                // Click on Yes | OK only
                bool yesOrOkayClicked = ClickButtonUsingId(s_YESBUTTONNAME, s_MBYES_AUTOID, msgBox);

                if (!yesOrOkayClicked)
                    yesOrOkayClicked = ClickButtonUsingId(s_OKBUTTONNAME, s_MBOK_AUTOID, msgBox);

                return yesOrOkayClicked;
            }
            else
            {
                Output("FileDialog is not blocked by another modal window");
                return false;
            }
        }

        #endregion

        #region interact with main app window
        public override UIHandlerAction HandleWindow(IntPtr topLevelhWnd, IntPtr hwnd,
            System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            // We pass by default in this handler.  Only fail in special cases.
            Log log = Log.Current;
            bool testPassed = true;
            UIHandlerAction uiHandlerAction = UIHandlerAction.Unhandled;
            Output("Starting HandleWindow.");

            String[] fileNames = FileName.Split(';');

            Output("Routing test depending on TestType: " + TestType);
            if (TestType.ToLowerInvariant().Contains("typefilenameandenter"))
                testPassed = testPassed && TypeFileNameAndEnter(fileNames, topLevelhWnd);
            else if (TestType.ToLowerInvariant().Contains("typefilenameandcancel"))
                testPassed = testPassed && TypeFileNameAndCancel(fileNames, topLevelhWnd);
            else if (TestType.ToLowerInvariant().Contains("typefilenameandsave") ||
                     TestType.ToLowerInvariant().Contains("typefilenameandopen"))
                testPassed = testPassed && TypeFileNameAndOk(fileNames, topLevelhWnd);
            else if (TestType.ToLowerInvariant().Contains("closewindowwithxbutton"))
                testPassed = testPassed && CloseWindowWithXButton(topLevelhWnd);
            else if (TestType.ToLowerInvariant().Contains("verifyopendialogvistaonvista"))
                testPassed = testPassed && VerifyVistaOnVista(topLevelhWnd);
            else if (TestType.ToLowerInvariant().Contains("verifyopendialogcustomplace"))
                testPassed = testPassed && VerifyOpenDialogCustomPlace(fileNames, topLevelhWnd);
            else if (TestType.ToLowerInvariant().Contains("verifysavedialogcustomplace"))
                testPassed = testPassed && VerifySaveDialogCustomPlace(fileNames, topLevelhWnd);
            else if (TestType.ToLowerInvariant().Contains("verifyisthreadmodal"))
                testPassed = testPassed && VerifyIsThreadModal(topLevelhWnd);
            else
            {
                Output("Can't recognize the TestType = " + TestType);
                testPassed = false;
            }

            // End the test
            if (testPassed)
            {
                Output("Passed: Expected UI Seen ...");
                Output("Process Name: " + process.ProcessName + " Window Title: " + title);
                Output("CommonFileDialogHandler - Handled Correctly");
            }
            else
                Output("Failed: An earlier subtest failed");

            // Log test result if we don't press OK|Save|Enter key
            if (TestType.ToLowerInvariant().Contains("closewindowwithxbutton") ||
                TestType.ToLowerInvariant().Contains("typefilenameandcancel") ||
                TestType.ToLowerInvariant().Contains("verifyisthreadmodal") ||
                TestType.ToLowerInvariant().Contains("verifyopendialogvistaonvista"))
                TestLog.Current.Result = (testPassed ? TestResult.Pass : TestResult.Fail);

            uiHandlerAction = UIHandlerAction.Abort;
            return uiHandlerAction;
        }

        public static bool CheckCommonFileDialogUI(bool apiResult)
        {
            // End the test
            if (apiResult)
            {
                GlobalLog.LogStatus("Passed: Expected UI Seen ...");
                GlobalLog.LogStatus("CommonFileDialogHandler - Handled Correctly");
                return true;
            }
            else
            {
                GlobalLog.LogStatus("Failed: An earlier subtest failed");
                return false;
            }
        }

        /// <summary>
        /// USED TO: set all the general FileDialog properties
        /// Now, this method just types in the file name into the dialog's text box
        /// </summary>
        /// <param name="mainWin">main window for the application (parent of common FileDialog)</param>
        /// <returns>true if we typed the filename into the TextBox successfully; false otherwise</returns>
        private bool CheckFileDialogOptions(AutomationElement mainWin)
        {
            Output("Typing in fileName TextBox");
            return TypeInTextBox(mainWin, "fileName", FileName);
        }

        /// <summary>
        /// Types the given string into the TextBox with a specified AutomationID.
        /// This clears out the current contents of the Textbox and presses Enter once
        /// the string has been typed
        /// </summary>
        /// <param name="searchFrom">Root of the AutomationElement tree to search from</param>
        /// <param name="tbName">AutomationID of the TextBox</param>
        /// <param name="tbText">String to type into the TextBox</param>
        /// <returns>true if we were able to type tbText into TextBox successfully; false otherwise</returns>
        private bool TypeInTextBox(AutomationElement searchFrom, string tbName, string tbText)
        {
            AndCondition isTextBox = new AndCondition(new PropertyCondition(AutomationElement.AutomationIdProperty, tbName),
                new PropertyCondition(AutomationElement.ClassNameProperty, "Edit"));

            AutomationElement textBox = searchFrom.FindFirst(TreeScope.Descendants, isTextBox);
            if (textBox == null)
            {
                Output("Couldn't find the TextBox named '" + tbName + "'");
                return false;
            }

            try
            {
                // Set keyboard focus on the TextBox and sleep to make sure it has focus
                textBox.SetFocus();
                Wait();

                // Select and delete current TextBox contents
                //  --> Ctrl + A to select
                Key[] ctrlA = { Key.LeftCtrl, Key.A };
                TypeKeyCombination(ctrlA);
                // --> Delete
                TypeKey(Key.Delete);

                // Type in tbText and press Enter
                TypeString(tbText);

                TypeKey(Key.Enter);
                return true;
            }
            catch (Exception exp)
            {
                Output("UNEXPECTED EXCEPTION: " + exp.ToString());
                return false;
            }
        }
        #endregion

        #region GlobalLog helpers
        private void Output(String outputMsg)
        {
            Log.Current.CurrentVariation.LogMessage(outputMsg);
        }
        #endregion

        #region Input helpers
        /// <summary>
        /// Press the given key (hold down, release)
        /// </summary>
        /// <param name="k1">Key to press</param>
        private void TypeKey(Key k1)
        {
            MTI.Input.SendKeyboardInput(k1, true);
            MTI.Input.SendKeyboardInput(k1, false);
        }

        /// <summary>
        /// Type in one character from input every KEYWAIT ms
        /// </summary>
        /// <param name="input">String to type.</param>
        private void TypeString(String input)
        {
            MTI.Input.SendUnicodeString(input, 1, KEYWAIT);
        }

        /// <summary>
        /// Hold down and release a sequence of keys
        /// </summary>
        /// <param name="keysToPress">A list of keys to hold down and release</param>
        private void TypeKeyCombination(Key[] keysToPress)
        {
            // Hold down all the keys
            for (int i = 0; i < keysToPress.Length; i++)
            {
                MTI.Input.SendKeyboardInput(keysToPress[i], true);
            }

            // Release all the keys
            for (int j = keysToPress.Length - 1; j >= 0; j--)
            {
                MTI.Input.SendKeyboardInput(keysToPress[j], false);
            }
        }

        /// <summary>
        /// Pause for 3 seconds
        /// </summary>
        private void Wait()
        {
            Thread.Sleep(3000);
        }
        #endregion
    }
}
