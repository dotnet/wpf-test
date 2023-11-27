// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Test.Logging;
using Microsoft.Test.Serialization;
using System.Collections.ObjectModel;
using System.Collections;
using Microsoft.Test.Win32;
using Microsoft.Test.Input;
using System.IO;
using System.Reflection;
using Microsoft.Test;

public class Driver
{
    /// <summary>
    /// The main entry point to run tests.
    /// </summary>
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            LogManager.BeginTest(DriverState.TestName);
            try
            {
                args = new string[2];
                args[0] = DriverState.DriverParameters["Class"];
                args[1] = DriverState.TestName;
                RunExecution(args);
            }
            catch (Exception e)
            {
                GlobalLog.LogEvidence("TestDriver Exception: " + e.ToString());
                PrintHelp();
            }
        }
        finally
        {
            LogManager.EndTest();
        }
    }

    static void RunExecution(string[] args)
    {
        // Move mouse to upper-left of screen to ensure tests always
        // have the same starting position.
        ResetMousePosition();

        //The execution ElementHost RequireFullTrust runs in System.Windows.Forms.Application
        if (args[0].ToLower() == "elementhost" || args[1] == "RequireFullTrust")
        {
            Type test = TestLibraryUtility.GetTestType(args[1]);
            System.Windows.Forms.Application.Run((System.Windows.Forms.Form)Activator.CreateInstance(test, new object[] { new string[0] }));
        }
        else if (args[0].ToLower() == "windowsformshost")
        {
            string testType = "WindowsFormsHostTests." + args[1];
            Type test = TestLibraryUtility.GetTestType(testType);
            System.Windows.Application app = new System.Windows.Application();
            app.Run((System.Windows.Window)Activator.CreateInstance(test, new object[] { new string[0] }));
        }
        else
        {
            TestLog testLog = new TestLog("TestDriver");
            testLog.LogEvidence("WindowsFormsIntegration TestDriver did not run on test. args[0]: " + args[0].ToString());
            testLog.Result = TestResult.Fail;
            testLog.Close();

            PrintHelp();
        }
    }

    private static void PrintHelp()
    {
        Console.WriteLine(@"
  Usage: TestDriver [ElementHost | WindowsFormsHost] [Test Class]

  e.g. TestDriver ElementHost Async
        ");
    }

    public class TestLibraryUtility
    {
        public static Type GetTestType(string typeName)
        {
            string dir = Path.GetDirectoryName(typeof(Driver).Assembly.Location);
            string assemblyPath = Path.Combine(dir, "WindowsFormsIntegrationTests.dll");
            return System.Reflection.Assembly.LoadFrom(assemblyPath).GetType(typeName);
        }
    }

    /// <summary>
    /// Put mouse in upper-left of screen to ensure tests always have the same starting position. 
    /// </summary>
    private static void ResetMousePosition()
    {
        IntPtr hwnd = (IntPtr)NativeMethods.GetDesktopWindow();
        NativeStructs.RECT windowRect = new NativeStructs.RECT();
        NativeMethods.GetWindowRect(new System.Runtime.InteropServices.HandleRef(null, hwnd), ref windowRect);

        Input.SendMouseInput(windowRect.left, windowRect.top, 0, SendMouseInputFlags.Move | SendMouseInputFlags.Absolute);
    }
}
