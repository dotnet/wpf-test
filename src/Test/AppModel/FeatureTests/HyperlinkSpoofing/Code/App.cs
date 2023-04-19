// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Wpf.AppModel.HyperlinkSpoofing
{
    /// <summary>
    /// App: Main class which runs a given test case
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Called when app starts up
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            string testName = DriverState.DriverParameters["TestToRun"];
            GlobalLog.LogEvidence("test name is: " + testName);

            StartupUri = new Uri(testName + ".Start.xaml", UriKind.RelativeOrAbsolute);

            base.OnStartup(e);
        }

        /// <summary>
        /// Default DispatcherException event handler: fail on unexpected exceptions
        /// </summary>
        void OnDispatcherException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // we got an unexpected exception. show them and fail
            if (e.Exception != null)
            {
                GlobalLog.LogEvidence(e.Exception.ToString());

                if (e.Exception.InnerException != null)
                {
                    GlobalLog.LogEvidence(e.Exception.InnerException.ToString());
                }
            }
            GlobalLog.LogEvidence("Got unexpected exception => test failed");
            TestLog.Current.Result = TestResult.Fail;

            // the exception was handled
            e.Handled = true;

            // shut app down
            Shutdown();
        }
    }
}
