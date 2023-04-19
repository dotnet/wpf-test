// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowsApplication2
{
    public partial class Form1 : Form
    {
 
       Microsoft.Test.Logging.TestLog _log = null;
       bool _browserhostedapp = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void newToolStripButton_Paint(object sender, PaintEventArgs e)
        {
            _log = Microsoft.Test.Logging.TestLog.Current; 
            // log.Stage = Microsoft.Test.Logging.TestStage.Initialize;

            if ( AppDomain.CurrentDomain.FriendlyName.ToString().Contains(".xbap") )        
            {
                _browserhostedapp = true;
            }

            _log.LogStatus("Run test.");
            // log.Stage = Microsoft.Test.Logging.TestStage.Run;

            _log.LogStatus("Toolstrip Paint event fired.");
            _log.Result = Microsoft.Test.Logging.TestResult.Pass;

            ShutdownApp();
        }

        private void ShutdownApp()
        {

            // log.Stage = Microsoft.Test.Logging.TestStage.Cleanup;
            _log.LogStatus("ShutdownApp - BrowserHostedApp : " + _browserhostedapp.ToString());

            if ( _browserhostedapp == false)        
            {
                this.Close();
            }
            else
            {
                _log.LogStatus("Shutting down application");
                // log.Close();
            }

            Microsoft.Test.Loaders.ApplicationMonitor.NotifyStopMonitoring();

        }

        public bool BrowserHostedApp
        {
            get
            {
                return _browserhostedapp;
            }
        }

    }
}
