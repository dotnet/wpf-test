// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Logging;
using System;
using System.Windows;

namespace PropertyOrderUsingNestedME
{
    /// <summary>
    /// Verifies that nested markup extesions are marked as shared when inside a template. 
    /// Verification is through the order in which the properties are set.  
    /// Shareable properties are set first.
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _validationString = "NonSharableProp, SharableProp, PropInQuestion, " // Inline Style
                                            + "SharableProp, PropInQuestion, NonSharableProp, " // Resource Template
                                            + "SharableProp, PropInQuestion, NonSharableProp, " // Resource Nested Template
                                            + "SharableProp, PropInQuestion, NonSharableProp, " // Inline Template
                                            + "SharableProp, PropInQuestion, NonSharableProp, " // Inline Nested Template
                                            + "SharableProp, PropInQuestion, NonSharableProp, " // Resource Template (4 level ME)
                                            + "SharableProp, PropInQuestion, NonSharableProp, " // Resource Nested Template (4 level ME)
                                            + "SharableProp, PropInQuestion, NonSharableProp, " // Inline Template (4 level ME)
                                            + "SharableProp, PropInQuestion, NonSharableProp,"; // Inline Nested Template (4 level ME)
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainTextBox.Text = TestButton.AllOrder;

            if (!String.Equals(TestButton.AllOrder.Trim(), _validationString.Trim()))
            {
                TestLog.Current.LogEvidence("AllOrder string did not match");
                TestLog.Current.LogEvidence("Expected: {0}", _validationString.Trim());
                TestLog.Current.LogEvidence("Actual: {0}", TestButton.AllOrder.Trim());
                TestLog.Current.Result = TestResult.Fail;
            }
            else
            {
                TestLog.Current.Result = TestResult.Pass;
            }
            Application.Current.Shutdown(0);

        }








    }
}
