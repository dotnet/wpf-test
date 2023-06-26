// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*****************************************

 *  This file test CF_TEXT and CF_UNICODETEXT:
 *  1. Clipboard.SetDataObject()        - SetData to clipboard bu avalon api
 *  2. Win32.GetClipboardData           - GetData by Win32.GetClipboardData
 *  Command Line: exe.exe /TestCaseType=SetAvalonDataGetWin32DataInprocAPI /TestText1=abc /CF=1 or 13
 *  TestText1:  "abc", "a 1 & [", !AD:index=-1;length=20 OR any string text
 *  CF: 1 for CF_TEXT, 13 for CF_UNICODE
******************************************/

namespace DataTransfer
{
    #region Namespaces.

    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Threading;
    using System.Windows.Threading;

    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using Test.Uis.TestTypes;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    #endregion Namespaces.

    /// <summary>
    /// Verifies that data set through the Avalon clipboard can be retrieved
    /// with the Win32 APIs.
    /// </summary>
    [Test(2, "Clipboard", "SetAvalonDataGetWin32DataInprocAPI1", MethodParameters = "/TestCaseType=SetAvalonDataGetWin32DataInprocAPI /TestText1='1 m Q (' /CF=1")]
    [Test(2, "Clipboard", "SetAvalonDataGetWin32DataInprocAPI2", MethodParameters = "/TestCaseType=SetAvalonDataGetWin32DataInprocAPI /TestText1='( = a ?' /CF=13")]
    public class SetAvalonDataGetWin32DataInprocAPI : CustomTestCase
    {
        bool _pass = true;
        string _str;

        private string TestText1
        {
            get { return ConfigurationSettings.Current.GetArgument("TestText1", true); }
        }

        //Set clipboard from win32 clipboard
        private void SetDataToAvalonClipboard()
        {
            Clipboard.SetDataObject(TestText1, true);
        }

        //Get text from Avalon clipboard
        private void GetDataByWin32Clipboard()
        {
            if (!Win32.OpenClipboard(Win32.HWND.NULL))
                Logger.Current.Log("OpenClipboard was not oppened. Bad!");
            int arg = ConfigurationSettings.Current.GetArgumentAsInt("CF", true);
            IntPtr p = Win32.GlobalLock(Win32.GetClipboardData((uint)arg));

            if (arg == 1)
                _str = Avalon.Test.Win32.Interop.StringFromSBytePtr(p); //for CF_TEXT
            else if (arg == 13)
                _str = Avalon.Test.Win32.Interop.StringFromCharPtr(p);  //for CF_UNICODETEXT
            if (_str != TestText1)
            {
                Logger.Current.Log("Clipboard text is incorrect: " + _str);
                _pass = false;
            }
            Win32.CloseClipboard();
        }

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            SetDataToAvalonClipboard();
            GetDataByWin32Clipboard();
            Logger.Current.ReportResult(_pass, "Test case is : " + _pass);
        }
    }
}
