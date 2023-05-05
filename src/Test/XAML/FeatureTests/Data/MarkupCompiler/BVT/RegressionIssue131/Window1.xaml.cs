// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using Microsoft.Test.Logging;

namespace MarkupCompiler.RegressionIssue131
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// Verify if StaticExtension can access internal properties
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
            if ((String)button1.Content == InternalStaticResource.label1 && (String)button2.Content == InternalStaticResource.label2 && (InternalEnum)button1.Tag == InternalEnum.Height1)
            {
                log.Result = TestResult.Pass;
                Application.Current.Shutdown(0);
            }
            else
            {
                log.Result = TestResult.Fail;
                Application.Current.Shutdown(-1);
            }
        }
    }

    internal class InternalStaticResource
    {
        internal static string label1
        {
            get
            {
                return "HelloWorld";
            }
        }
        internal static string label2 = "HelloWorld2";
    }

    internal enum InternalEnum
    {
        Height1 = 100
    }
}
