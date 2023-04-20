// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using Microsoft.Test.Logging;

namespace RegressionIssue116
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// Verify that resource dictionary files can have file extensions other than .xaml
    /// </summary>

    public partial class Window1 : Window
    {
        public Window1()
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
