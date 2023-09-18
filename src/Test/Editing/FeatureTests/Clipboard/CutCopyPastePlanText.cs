// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*************************************************
 *  In first app do:
 *  1. SetDataObject(object)
 *  2. SetDataObject(object, true)
 *  3. SetClipboardData(Ansi)     -  win32 clipboard
 *  4. SetClipboardData(Auto)     -  win32 clipboard
 *  5. SetClipboardData(UniCode)  -  win32 clipboard
 *  6. Ctrl+x for cut
 *  7. Ctrl+c for copy
 *  In first app verify:
 *  1. correct avalon clipboard data
 *  2. correct text selection
 *  In Second app verify:
 *  1. GetDataObject()
 *  2. GetClipboardData(anis, auto, unicode) - win32 clipboard
 *  3. Ctrl+v for paste
 *  4. IsCurrent() for both false and true
 *  Command Line: exe.exe /TestCaseType=CutCopyPastePlanText /Xaml=CutCopyPastePlanText3.xaml /InputText="abc def" /Action=SetClipboardDataAnsi /SelectText="abc" /CrossApp=true
 *  Where:
 *  1. Action= SetDataObject, SetDataObjectTrue, SetClipboardDataAnsi, SetClipboardDataAuto, SetClipboardDataUniCode, Copy, Cut
 *  2. CrossApp = true, false
 * ************************************************/
 
namespace DataTransfer
{
    #region Namespaces.

    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;

    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using Test.Uis.TestTypes;        

    #endregion Namespaces.

    // command: /TestCaseType=CutCopyPastePlanText /Xaml=CutCopyPastePlanText3.xaml /InputText="abc def" /Action=SetClipboardDataAnsi /SelectText="abc" /CrossApp=true

    /// <summary>
    /// Verifies that the cut, copy and paste commands work with plain text.
    /// </summary>
    [Test(0, "Clipboard", "CutCopyPastePlanText1", MethodParameters = "/TestCaseType=CutCopyPastePlanText /Xaml=CutCopyPastePlanText3.xaml /InputText=abc_def /Action=SetDataObject /SelectText=abc /CrossApp=true")]
    [Test(0, "Clipboard", "CutCopyPastePlanText2", MethodParameters = "/TestCaseType=CutCopyPastePlanText /Xaml=CutCopyPastePlanText3.xaml /InputText=abc_def /Action=SetDataObjectTrue /SelectText=abc /CrossApp=true")]
    [Test(0, "Clipboard", "CutCopyPastePlanText3", MethodParameters = "/TestCaseType=CutCopyPastePlanText /Xaml=CutCopyPastePlanText3.xaml /InputText=abc_def /Action=SetClipboardDataUniCode /SelectText=abc /CrossApp=true")]
    [Test(2, "Clipboard", "CutCopyPastePlanText4", MethodParameters = "/TestCaseType=CutCopyPastePlanText /Xaml=CutCopyPastePlanText3.xaml /InputText=abc_def /Action=Copy /SelectText=abc /CrossApp=true")]
    [Test(2, "Clipboard", "CutCopyPastePlanText5", MethodParameters = "/TestCaseType=CutCopyPastePlanText /Xaml=CutCopyPastePlanText3.xaml /InputText=abc_def /Action=Cut /SelectText=abc /CrossApp=true")]
    [Test(2, "Clipboard", "CutCopyPastePlanText6", MethodParameters = "/TestCaseType=CutCopyPastePlanText /Xaml=CutCopyPastePlanText3.xaml /InputText=abc_def /Action=SetClipboardDataAnsi /SelectText=abc /CrossApp=true")]
    [Test(2, "Clipboard", "CutCopyPastePlanText7", MethodParameters = "/TestCaseType=CutCopyPastePlanText /Xaml=CutCopyPastePlanText3.xaml /InputText=abc_def /Action=SetClipboardDataAuto /SelectText=abc /CrossApp=true")]
    [TestOwner("Microsoft"), TestTactics("54,55,56,57,58,59,60")]
    public class CutCopyPastePlanText : CustomTestCase
    {
        private UIElement _textBox = null;        //first TextBox
        private Rect _rc;     //find rectangle of a control
        private System.Windows.Point _p1;       //first point for mouse click action
        private bool _pass = false;               //for logging
        private string _arg1 = null;              //for loading xaml
        private string _arg2 = null;              //for adding text to TextBox1
        private string _arg3 = null;              //for specifying action
        private string _arg4 = null;              //for verifying selected text
        IntPtr _handle;
        IntPtr _setData;

        private const int VK_C = 0x43;
        private const int VK_V = 0x56;
        private const int VK_X = 0x58;

        /// <summary>Creates a new CutCopyPastePlanText instance.</summary>
        public CutCopyPastePlanText()
        {                                    
        }        

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _arg1 = ConfigurationSettings.Current.GetArgument("Xaml", true);
            string xamlContents = XamlUtils.GetXamlFileContents(_arg1);            
            MainWindow.Content = XamlUtils.ParseToObject(xamlContents);

            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(FindElements));       
        }

        private void FindElements()
        {
            _textBox = ElementUtils.FindElement(MainWindow, "textbox1"); //find TextBox by Name
            _rc = ElementUtils.GetScreenRelativeRect(_textBox);               //find position of TextBox
            _p1 = new System.Windows.Point(_rc.Left + 14, _rc.Top + 14);                     //find a point in TextBox to mouseclick on "a"

            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(StartTest));
        }

        private void StartTest()
        {            
            MouseInput.MouseClick(_p1);
            _arg2 = ConfigurationSettings.Current.GetArgument("InputText", true);

            Log("Assigning [" + _arg2 + "] to the textbox");
            ((TextBox)_textBox).Text = _arg2;         //Add text into TextBox1
            
            _arg3 = ConfigurationSettings.Current.GetArgument("Action", true);
            switch (_arg3)
            {
                case "SetDataObject":
                    Clipboard.SetDataObject("abc");
                    QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(CheckClipboardText));
                    break;
                case "SetDataObjectTrue":
                    Clipboard.SetDataObject("abc", true);
                    QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(CheckClipboardText));
                    break;
                case "SetClipboardDataAnsi":
                    Win32.OpenClipboard(Win32.HWND.NULL);   //Open clipboard so data can be set to
                    Win32.EmptyClipboard();
                    _handle = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi("abc");
                    _setData = Win32.SetClipboardData(1, _handle);            //Set text string to win32 clipboard. 1 for CF_TEXT
                    Win32.CloseClipboard();
                    QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(CheckClipboardText));
                    break;
                case "SetClipboardDataAuto":
                    Win32.OpenClipboard(Win32.HWND.NULL);   //Open clipboard so data can be set to
                    Win32.EmptyClipboard();
                    _handle = System.Runtime.InteropServices.Marshal.StringToHGlobalAuto("abc");
                    _setData = Win32.SetClipboardData(13, _handle);
                    Win32.CloseClipboard();
                    QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(CheckClipboardText));
                    break;
                case "SetClipboardDataUniCode":
                    Win32.OpenClipboard(Win32.HWND.NULL);   //Open clipboard so data can be set to
                    Win32.EmptyClipboard();
                    _handle = System.Runtime.InteropServices.Marshal.StringToHGlobalUni("abc");
                    _setData = Win32.SetClipboardData(13, _handle);           //Set text string to win32 clipboard. 1 for CF_UNICODETEXT
                    Win32.CloseClipboard();
                    QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(CheckClipboardText));
                    break;
                case "Copy":
                    QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(DoSelectText));
                    QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(DoCopyText));
                    break;
                case "Cut":
                    QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(DoSelectText));
                    QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(DoCutText));
                    break;
                default:
                    Logger.Current.ReportResult(false, "invalid comamnd. " + _arg3);
                    break;
            }
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(LaunchSecondApp));
        }

        private void DoSelectText()
        {
            _arg4 = ConfigurationSettings.Current.GetArgument("SelectText", true);
            ((TextBox)_textBox).Select(0, 3);
            if (((TextBox)_textBox).SelectedText != _arg4)
                Logger.Current.ReportResult(false, "Fail to select text. Expected[" + _arg4 + "] Actual[" + ((TextBox)_textBox).SelectedText + "]");
            else
                Logger.Current.Log("Text selection is corrected. " + ((TextBox)_textBox).SelectedText);
        }

        private void DoCopyText()
        {
            KeyboardInput.TypeString("^c");
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(VerifyDoCopyText));
        }

        private void VerifyDoCopyText()
        {
            if (((TextBox)_textBox).Text != _arg2)
                Logger.Current.ReportResult(false, "Failed to copy text");
            else
                QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(CheckClipboardText));
        }

        private void DoCutText()
        {
            KeyboardInput.TypeString("^x");
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(VerifyDoCutText));
        }

        private void VerifyDoCutText()
        {
            if (((TextBox)_textBox).Text == _arg2)
                Logger.Current.ReportResult(false, "Failed to cut text");
            else
                QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(CheckClipboardText));
        }

        private void CheckClipboardText()
        {
            IDataObject dataObj;
            object data;

            dataObj = Clipboard.GetDataObject();
            if (dataObj != null)
            {
                data = dataObj.GetData(DataFormats.Text);
                Verifier.Verify((string)data == "abc", "Clipboard data is notvalid.");
            }
        }

        private void LaunchSecondApp()
        {
            if ((Settings.GetArgumentAsBool("CrossApp", true)))
            {
                Logger.Current.Log("Test is completed in cross app.");
                const int milliseconds = 1000 * 15;
                System.Diagnostics.Process process;
                process = System.Diagnostics.Process.Start("EditingTest.exe", "/TestCaseType=CutCopyPastePlanTextFor2ndApp /Xaml=cutcopypasteplantext4.xaml");
                process.WaitForExit(milliseconds);
                if (process.HasExited)
                {
                    _pass = Logger.Current.ProcessLog("CutCopyPastePlanTextLog.txt");
                    Logger.Current.ReportResult(_pass, "Test is " + _pass);
                }
            }
            else
            {
                Logger.Current.Log("Test is completed in Single app.");
                _pass = true;
                Logger.Current.ReportResult(_pass, "Test is " + _pass);
            }
        }
    }

    //Launch second app
    // command: /TestCaseType=CutCopyPastePlanTextFor2ndApp /Xaml2=cutcopypasteplantext4.xaml /InputText=any string

    /// <summary>
    /// Runs the part of the CutCopyPastePlanText test executing in 
    /// a different process.
    /// </summary>
    public class CutCopyPastePlanTextFor2ndApp : CustomTestCase
    {
        private UIElement _textBox2 = null;
        private bool _pass = false;

        /// <summary>Creates a new CutCopyPastePlanTextFor2ndApp instance.</summary>
        public CutCopyPastePlanTextFor2ndApp()
        {
            string arg = ConfigurationSettings.Current.GetArgument("Xaml", true);
            this.StartupPage = arg;
        }

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            //Comment 3 lines below when debug if case fail so you can see the log
            //            if (Test.Uis.IO.TextFileUtils.Exists("CutCopyPastePlanTextLog.txt"))
            //                Test.Uis.IO.TextFileUtils.Delete("CutCopyPastePlanTextLog.txt");

            Logger.Current.LogToFile("CutCopyPastePlanTextLog.txt");
            _textBox2 = ElementUtils.FindElement(MainWindow, "textbox2");        //Find TextBox2 by Name
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(StartTest));
        }

        private void StartTest()
        {
            TextSelection textSelection = Test.Uis.Utils.TextUtils.GetTextBoxSelection((TextBox)_textBox2);
            TextPointer textNavigatorStart = Test.Uis.Utils.TextUtils.GetTextBoxStart((TextBox)_textBox2);
            textNavigatorStart = textNavigatorStart.GetPositionAtOffset(1);
            textSelection.Select(textNavigatorStart, textNavigatorStart);

            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(DoGetDataObject));
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(DoGetClipboardData));    //win32 clipboard
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(DoPaste));
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(DoIsCurrent));
        }

        private void DoGetDataObject()
        {
            IDataObject dataObj;
            object data;

            dataObj = Clipboard.GetDataObject();
            if (dataObj != null)
            {
                data = dataObj.GetData(DataFormats.Text);
                if ((string)data != "abc")
                    Logger.Current.ReportResult(false, "Text in Clipboard is in corrected. " + (string)data);
                else
                    Logger.Current.Log("Clipboard data is valid in second app. " + (string)data);
            }
        }

        private void DoGetClipboardData()
        {
            string str = null;

            if (!Win32.OpenClipboard(Win32.HWND.NULL))
                Logger.Current.Log("OpenClipboard was not oppened. Bad!");
            IntPtr p1 = Win32.GlobalLock(Win32.GetClipboardData((uint)1));

            str = Avalon.Test.Win32.Interop.StringFromSBytePtr(p1);   //for CF_TEXT
            if (str != "abc")
                Logger.Current.ReportResult(false, "Clipboard text is incorrect for CF_TEXT: " + str);

            IntPtr p2 = Win32.GlobalLock(Win32.GetClipboardData((uint)13));

            str = Avalon.Test.Win32.Interop.StringFromCharPtr(p2);    //for CF_UNICODETEXT
            if (str != "abc")
                Logger.Current.ReportResult(false, "Clipboard text is incorrect for CF_UNICODETEXT: " + str);

            Logger.Current.Log("End of Get win32 clipboard data");
            Win32.CloseClipboard();
        }

        private void DoPaste()
        {
            RoutedCommand PasteCommand = ApplicationCommands.Paste;
            PasteCommand.Execute(null, (TextBox)_textBox2);
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(VerifyResult));
        }

        private void DoIsCurrent()
        {
            IDataObject dataObj;
            DataObject dataObj2;

            dataObj = Clipboard.GetDataObject(); //Get clipboard data

            if (Clipboard.IsCurrent(dataObj))   //return false
                Logger.Current.ReportResult(false, "Clipboard.IsCurrent(dataObj)should be false: " + Clipboard.IsCurrent(dataObj));
            else
            {
                dataObj2 = new DataObject(dataObj.ToString());
                Clipboard.SetDataObject(dataObj2);          //SetDataObject copy=false
                if (!Clipboard.IsCurrent(dataObj2)) //return true
                    Logger.Current.ReportResult(false, "Clipboard.IsCurrent(dataObj2)should be true: " + Clipboard.IsCurrent(dataObj2));
                else
                {
                    Log("Clipboard.IsCurrent for dataObj2 is true.");
                    Clipboard.SetDataObject(dataObj2, true);    //SetDataObject copy=true
                    if (Clipboard.IsCurrent(dataObj2))
                        Logger.Current.ReportResult(false, "Clipboard.IsCurrent(dataObj2)should be false: " + Clipboard.IsCurrent(dataObj2));
                    else
                        Logger.Current.Log("IsCurrent() is working. Regression_Bug281 is fixed.");
                }
            }
        }

        private void VerifyResult()
        {
            if (((TextBox)_textBox2).Text != "1abc23 456")
                _pass = false;
            else
                _pass = true;
            Logger.Current.ReportResult(_pass, "Test is " + ((TextBox)_textBox2).Text, false);
        }
    }
}
