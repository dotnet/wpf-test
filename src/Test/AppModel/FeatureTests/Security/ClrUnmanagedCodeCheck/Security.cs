// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Test.Diagnostics;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Wpf.AppModel
{
    public partial class SecurityTestApp: System.Windows.Application
    {
        [STAThread]
        static void Main(string[] args)
        {
            TestHelper testHelper = new TestHelper(args);
            string TestName;

            if (TestHelper.Current.AppArgs.Length == 0)
            {
                LogManager.LogMessageDangerously("No test specified");
                Application.Current.Shutdown();
                return;
            }

            TestName = TestHelper.Current.AppArgs[0];
            if (String.IsNullOrEmpty(TestName))
            {
                LogManager.LogMessageDangerously("TestName cannot be empty or null");
                return;
            }
            else
            {
                LogManager.CurrentLog.CreateVariation("Security Test - " + TestName);
            }

            if ((TestHelper.Current.AppArgs.Length > 1) && (!String.IsNullOrEmpty(TestHelper.Current.AppArgs[1])))
            {
                if (TestHelper.Current.AppArgs[1] == "debug")
                {
                    MessageBox.Show("attach debugger now, then OK this message box");
                }
            }
            LoadAndRunTest(TestName);
        }

        static void CloseTheLog(object sender, ExitEventArgs e)
        {
            Logger.CommitCachedResult();
        }

        static void LoadAndRunTest(string TestName)
        {
            object TestObject = Application.LoadComponent(new Uri(TestName, UriKind.RelativeOrAbsolute));
            if (TestObject == null)
            {
                Logger.LogFail("Test could not be loaded");
                return;
            }

            if (TestObject is Application)
            {
                TestObject.GetType().GetEvent("DispatcherUnhandledException").AddEventHandler(TestObject, new DispatcherUnhandledExceptionEventHandler(OnDispatcherUnhandledException));
                TestObject.GetType().GetMethod("InitializeComponent").Invoke(TestObject, null);
                TestObject.GetType().GetMethod("Run", new Type[0]).Invoke(TestObject, null);
                ((Application)TestObject).Exit += new ExitEventHandler(CloseTheLog);
            }
            else
            {
                Application app = new Application();
                app.Exit += new ExitEventHandler(CloseTheLog);
                app.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(OnDispatcherUnhandledException);
                app.StartupUri = new Uri(TestName, UriKind.Relative);
                app.Run();
            }
        }

        static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.LogFail("DispatcherUnhandledException Caught!\n" + e.Exception.ToString());
        }
    }
}
