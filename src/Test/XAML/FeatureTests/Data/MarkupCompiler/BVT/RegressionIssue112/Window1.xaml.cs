// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using Microsoft.Test.Logging;

namespace RegressionIssue112
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// Verify Begin/EndInit() is not called twice on UserControl
    /// </summary>

    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            
            Microsoft.Test.Logging.LogManager.BeginTest(Microsoft.Test.DriverState.TestName);
            Microsoft.Test.Logging.TestLog log = TestLog.Current;
            if (userControl1.TB1.Text == "BeginInitEndInit")
            {
                log.Result = TestResult.Pass;
                Application.Current.Shutdown(0);
            }
            else
            {
                GlobalLog.LogEvidence("Text in the UserControl: [" + userControl1.TB1.Text + "]");
                log.Result = TestResult.Fail;
                Application.Current.Shutdown(-1);
            }
        }
    }
}
