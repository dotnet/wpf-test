// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security;
using System.Windows.Forms;
using System.Windows.Controls;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;

namespace WebOCBlocked
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            _myLog = new TestLog("WebOCBlocked");

            Shown += OnShown;
        }

        TestLog _myLog;

        private void OnShown(object sender, EventArgs e)
        {
            try
            {
                //A partial-trust ClickOnce app is not allowed to host a WPF WebBrowser.  We expect a SecurityException.
                System.Windows.Controls.WebBrowser browser = new System.Windows.Controls.WebBrowser();
            }
            catch (TypeInitializationException exception)
            {
                if (exception.InnerException is SecurityException)
                {
                    //pass and close
                    GlobalLog.LogEvidence("Got expected inner SecurityException trying to instantiate a WebBrowser in a partial-trust ClickOnce app");
                    TestLog.Current.Result = TestResult.Pass;
                    ApplicationMonitor.NotifyStopMonitoring();
                    _myLog.Close();
                    return;
                }
            }

            //fail and close
            GlobalLog.LogEvidence("Failed to see expected TypeInializationException or inner SecurityException trying to instantiate a WebBrowser in a partial-trust ClickOnce app");
            TestLog.Current.Result = TestResult.Fail;
            ApplicationMonitor.NotifyStopMonitoring();
            _myLog.Close();
        }
    }
}
