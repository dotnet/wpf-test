// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using InternalTypes;
using Microsoft.Test.Logging;

namespace RegressionIssue114
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// Verify InternalsVisibleTo is honored in Xaml for signed assemblies
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            //Use internal type from another assembly in code
            SuperSecretType superSecretType = new SuperSecretType();

            Microsoft.Test.Logging.TestLog log = TestLog.Current;
            if (superSecretType.Name == "Foo" && ((SuperSecretType)listBox1.Items[0]).Name == "Foo")
            {
                log.Result = TestResult.Pass;
                Application.Current.Shutdown(0);
            }
            else
            {
                GlobalLog.LogEvidence("SuperSecretType.Name has unexpected value");
                log.Result = TestResult.Fail;
                Application.Current.Shutdown(-1);
            }
        }
    }
}
