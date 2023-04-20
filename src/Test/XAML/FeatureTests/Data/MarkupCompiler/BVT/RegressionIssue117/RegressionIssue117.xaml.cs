// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using Microsoft.Test.Logging;

namespace RegressionIssue117
{
    /// <summary>
    /// Interaction logic for WpfApplication2.xaml
    /// Verify that markup compilation succeeds when there is a class with same name as the root namespace
    /// </summary>
    public partial class RegressionIssue117 : Window
    {
        public RegressionIssue117()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            TestLog log = TestLog.Current;
            log.Result = TestResult.Pass;
            Application.Current.Shutdown(0);
        }
    }
}
