// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Xml;
using Microsoft.Test.Logging;

namespace RegressionIssue64
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// Verify that subclasses of Templates are usable in place of Templates
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Test.Logging.TestLog log = TestLog.Current;
            log.Result = TestResult.Pass;
            Application.Current.Shutdown(0);
        }
    }

    public class MyHierarchicalDataTemplate : HierarchicalDataTemplate
    {
    }
}
