// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.Graphics.TestTypes;

namespace Microsoft.Test.Graphics
{
    public partial class Part1Application : Application
    {
        #region Fields

        private static Dictionary<string, string> s_driverInvokeArgs = null;

        private Logger _logger;

        #endregion

        #region Properties

        public Logger Log
        {
            get { return _logger; }
        }

        #endregion

        #region Methods

        public static void Launch()
        {
            ParseArgs(); // parse app arguments (if any)
            Part1Application.Main();
        }

        private static void ParseArgs()
        {
            s_driverInvokeArgs = new Dictionary<string, string>();
            string[] args = DriverState.DriverParameters["Args"].Split(' ');

            for (int i = 0; i < args.Length; i++)
            {
                string[] values = args[i].Split('=');
                string key = values[0].Replace("/", "").Trim();
                string value = values[1].Trim();
                if (key != null && value != null && key.Length > 0)
                {
                    s_driverInvokeArgs.Add(key, value);
                }
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            RunTest();
        }

        private Window GetTestCaseWindow(string testClassName)
        {
            Window testCaseWindow = null;

            if (testClassName != null)
            {
                if (testClassName.Equals("GT2DTo3DTo2D.xaml", StringComparison.OrdinalIgnoreCase))
                {
                    testCaseWindow = new GT2DTo3DTo2D();
                }
                else if (testClassName.Equals("VP2DV3D_NULL_VHM.xaml", StringComparison.OrdinalIgnoreCase))
                {
                    testCaseWindow = new VP2DV3D_NULL_VHM();
                }
            }

            return testCaseWindow;
        }

        private void RunTest()
        {
            // set up the app to run the particular test case
            string testCaseFileName = s_driverInvokeArgs["TestCaseFileName"];
            Window testCaseWindow = GetTestCaseWindow(testCaseFileName);

            _logger = Logger.Create();

            if (testCaseFileName == null)
            {
                _logger.AddFailure("No test case file name was provided");
            }
            else if (testCaseWindow == null)
            {
                _logger.AddFailure("Invalid test case file name was provided - " + testCaseFileName);
            }
            else
            {
                try
                {
                    testCaseWindow.WindowStyle = WindowStyle.SingleBorderWindow;
                    testCaseWindow.Topmost = true;
                    testCaseWindow.ShowDialog();
                }
                catch (Exception e)
                {
                    _logger.LogStatus("Actual: " + e.GetType() + " - " + e.Message);
                    _logger.LogStatus(e.StackTrace);
                    _logger.AddFailure(e.GetType() + " - " + e.Message);
                }
            }

            _logger.Close();
            Application.Current.Shutdown();
        }

        #endregion
    }
}