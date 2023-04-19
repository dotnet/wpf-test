// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Resources;
using System.Collections;
using System.IO;
using System.Globalization;
using System.Windows;
using System.Windows.Navigation;
using System.Reflection;
using System.Windows.Threading;
using System.IO.Packaging;
using System.Runtime.InteropServices;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Windows.Test.Client.AppSec.BVT.ELCommon;

namespace Microsoft.Windows.Test.Client.AppSec.BVT.Window
{
    public partial class WindowTestApp: System.Windows.Application
    {
        [STAThread]
        static void Main(string[] args)
        {
            // Please leave this comment intact.  This makes debugging when launched by STI
            // better as it allows all the driver properties to get set and for the user to attach 
            // very early on in execution.
            // Also, PLEASE do not allow the MessageBox to be uncommented in a checkin if you are CR-ing!
            // MessageBox.Show("Attach Debugger now");
            string earlyFailureReason = string.Empty;

            TestHelper testHelper = new TestHelper(args);
            string TestName;

            try
            {
            if (TestHelper.Current.AppArgs.Length == 0)
            {
                earlyFailureReason = "No test specified";
            }

            TestName = TestHelper.Current.AppArgs[0];
            if (String.IsNullOrEmpty(TestName))
            {
                earlyFailureReason += "TestName cannot be empty or null";
            }
            // Can't log anything til we have gotten the variation's name
            // but logging occurs.  SOlution: Store any early logging in a string,
            // fail horrifically if the test never actually starts.
            if (earlyFailureReason != string.Empty)
            {
                LogManager.LogMessageDangerously("FAILURE:" + earlyFailureReason);
                Application.Current.Shutdown();
                return;
            }

            if (TestName.ToLower().Equals("all"))
            {
                Assembly exeAssembly = Assembly.GetExecutingAssembly();
                ResourceManager rm = new ResourceManager(exeAssembly.FullName, exeAssembly);
                foreach (string ResourceName in exeAssembly.GetManifestResourceNames())
                {
                    if (!ResourceName.EndsWith(".resources"))
                    {
                        continue;
                    }
                    Stream stream = exeAssembly.GetManifestResourceStream(ResourceName);
                    IResourceReader reader = new ResourceReader(stream);
                }
            }
            else
            {
                if (!TestName.EndsWith(".xaml"))
                    TestName += ".xaml";

                LoadAndRunTest(TestName);
            }

            }
            catch (Exception e)
            {
                Logger.LogFail(string.Format("Unexpected Exception: {0}", e.ToString())); 
            }
        }

        public static ExitEventHandler StandardExit = new ExitEventHandler(CloseVariationsAndCleanup);

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
            }
            else
            {
                Application app = new Application();
                app.Exit += StandardExit;
                app.StartupUri = new Uri(TestName, UriKind.Relative);
                app.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(OnDispatcherUnhandledException);
                app.Run();
            }
        }

        static void CloseVariationsAndCleanup(object sender, ExitEventArgs e)
        {
            TestHelper.CloseAndSetVariationResults();
        }

        static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.LogFail("DispatcherUnhandledException Caught!\n" + e.Exception.ToString());
        }

    }
}
