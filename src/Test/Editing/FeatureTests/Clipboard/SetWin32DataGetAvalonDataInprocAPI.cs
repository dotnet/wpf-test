// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*****************************************
 *  This file test CF_TEXT and CF_UNICODETEXT:
 *  1. Win32.SetClipboardData()            - Set data to win32 clipboard
 *    1. Clipboard.GetDataObject()        - GetData from clipboard
 *    2. Clipboard.IsCurrent(dataObject)    - flush the clipboard
 *    3. dataObject.GetData()                - GetData from clipboard and compare to string that was set into clipboard
 *  Command Line: exe.exe /TestCaseType=SetWin32DataGetAvalonDataInprocAPI /TestText1=abc /StringType=Ansi
 *    TestText1:    "abc", "a 1 & [", !AD:index=-1;length=20 OR any string text
 *  StringType:    Ansi, Auto or Uni
 *  index=0        :default string from autodata - is equal to AutoData.Extract.GetTestString(0)
 *  index=-1    :random length and text from common library code - configuration.cs
 *  length=20    :(option)how long the string is set - can be any length
******************************************/

namespace DataTransfer
{
    #region Namespaces.
    using System;
    using System.Drawing;
    using System.Threading; using System.Windows.Threading;
    using System.Windows;
    using System.Windows.Controls;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using Test.Uis.TestTypes;
    using Microsoft.Test.Imaging;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    #endregion Namespaces.

    /// <summary>
    /// Verifies that data set on the clipboard through the Win32 APIs can
    /// be retrieved through the Avalon clipboard.
    /// </summary>
    [Test(0, "Clipboard", "SetWin32DataGetAvalonDataInprocAPI1", MethodParameters = "/TestCaseType=SetWin32DataGetAvalonDataInprocAPI /TestText1='a 1 & Z' /StringType=Ansi")]
    [Test(2, "Clipboard", "SetWin32DataGetAvalonDataInprocAPI2", MethodParameters = "/TestCaseType=SetWin32DataGetAvalonDataInprocAPI /TestText1='a 1 & Z' /StringType=Uni")]
    [Test(2, "Clipboard", "SetWin32DataGetAvalonDataInprocAPI3", MethodParameters = "/TestCaseType=SetWin32DataGetAvalonDataInprocAPI /TestText1='a 1 & Z' /StringType=Auto")]
    public class SetWin32DataGetAvalonDataInprocAPI : CustomTestCase
    {
        private UIElementWrapper _editBox1;
        private UIElementWrapper _editBox2;

        private string TestText1
        {
            get { return ConfigurationSettings.Current.GetArgument("TestText1", true); }
        }
        private void BuildWindow()
        {
            Panel topPanel;
            FrameworkElement box1;
            FrameworkElement box2;
            string panel;
            string editbox;

            panel = Settings.GetArgument("ContainerType1");
            if (panel == "")
                panel="Canvas";
            topPanel = (Panel)ReflectionUtils.CreateInstanceOfType(panel, new object[] {});

            editbox = Settings.GetArgument("EditableType");
            if (editbox == "")
                editbox = "TextBox";

            box1 = TextEditableType.GetByName(editbox).CreateInstance();
            box2 = TextEditableType.GetByName(editbox).CreateInstance();
            box1.Height = box2.Height = 100;
            box1.Width = box2.Width = 250;
            box2.SetValue(Canvas.TopProperty, 120d);
            box1.Name = "tb1";
            box2.Name = "tb2";
            topPanel.Children.Add(box1);
            topPanel.Children.Add(box2);
            MainWindow.Content = topPanel;
        }

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            BuildWindow();
            QueueHelper.Current.QueueDelegate(new SimpleHandler(StartTest));
        }
        private void StartTest()
        {
            _editBox1 = new UIElementWrapper(ElementUtils.FindElement(MainWindow, "tb1"));
            _editBox2 = new UIElementWrapper(ElementUtils.FindElement(MainWindow, "tb2"));
            _editBox1.Element.Focus();

            IntPtr handle;
            IntPtr setData;
            Win32.SafeOpenClipboard(Win32.HWND.NULL); //Open clipboard so data can be set to
            Win32.SafeEmptyClipboard();
            switch (ConfigurationSettings.Current.GetArgument("StringType", true))
            {
                case "Ansi":
                    handle = Avalon.Test.Win32.Interop.hMemStringToHGlobalAnsi(TestText1);
                    setData = Win32.SafeSetClipboardData(Win32.CF_TEXT, handle);
                    break;
                case "Auto":  //Add regrssion for Regression_Bug20
                    handle = Avalon.Test.Win32.Interop.hMemStringToHGlobalAuto(TestText1);
                    setData = Win32.SafeSetClipboardData(Win32.CF_UNICODETEXT, handle);
                    IDataObject dataObject;
                    handle = Avalon.Test.Win32.Interop.hMemStringToHGlobalAnsi(TestText1);
                    setData = Win32.SafeSetClipboardData(Win32.CF_TEXT, handle);
                    try
                    {
                        dataObject = Clipboard.GetDataObject();
                    }
                    catch
                    {
                        Logger.Current.Log("ExternalException is catched.");
                    }
                    KeyboardInput.TypeString("^v");
                    QueueHelper.Current.QueueDelegate(new SimpleHandler(VerifyPaste));
                    break;
                case "Uni":
                    handle = Avalon.Test.Win32.Interop.hMemStringToHGlobalUni(TestText1);
                    setData = Win32.SafeSetClipboardData(Win32.CF_UNICODETEXT, handle);
                    break;
            }

            QueueHelper.Current.QueueDelegate(new SimpleHandler(GetDataByAvalonClipboard));
        }
        private void VerifyPaste()
        {
            Verifier.Verify(_editBox1.Text == "", "Application should not crash.  editBox1 should be empty. [Regression_Bug20]", true);
            Win32.SafeCloseClipboard();
            KeyboardInput.TypeString("^v");
        }
        //Get clipboard from win32 clipboard
        private void GetDataByAvalonClipboard()
        {
            Win32.SafeCloseClipboard();
            IDataObject dataObject = Clipboard.GetDataObject();
            if (dataObject == null)
                throw new Exception("Unable to create DataObject with Clipboard.GetDataObject()");
            Logger.Current.Log("Input TestText1 is           : " + TestText1);
            Logger.Current.Log("Clipboard.GetDataObject() is : " + dataObject.GetData("System.String", true).ToString());
            Verifier.Verify(TestText1 == dataObject.GetData("System.String", true).ToString(), "GetData is correct.");
            Verifier.Verify(dataObject.GetDataPresent(DataFormats.Text), "GetDataPresent is correct for Text.");
            Verifier.Verify(dataObject.GetDataPresent(DataFormats.UnicodeText), "GetDataPresent is correct for UnicodeText.");
            Verifier.Verify(dataObject.GetDataPresent(DataFormats.StringFormat), "GetDataPresent is correct for StringFormat.");
            Logger.Current.ReportSuccess();
        }
    }
}
