// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;


namespace Avalon.Test.ComponentModel.RegressionIssue135
{
    /// <summary>
    /// Regression test
    /// Problem Description: The values of custom (attached) dependency properties of type Int32Collection used in XAML in the 
    /// integrated XAML-Editor/Designer do not end up in the final instanciated CLR-object if less than 3 values have been defined 
    /// for the property in xaml.
    /// </summary>

    public partial class TestPanel : StackPanel
    {        

        public void OnLoaded(object sender, EventArgs args)
        {
            //use Dispatcher.BeginInvoke with SystemIdle priority to allow the Int32CollectionControl to load first
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.SystemIdle, new d1(Verify));            
        }

        delegate void d1();

        public void Verify()
        {
            TestLog.Current.Result = TestResult.Pass;
            CheckButtonContent(btn1, "2 3 5");
            CheckButtonContent(btn2, "2 3 5");
            CheckButtonContent(btn3, "2 3");
            CheckButtonContent(btn4, "2 3");
            CheckButtonContent(btn5, "2");
            CheckButtonContent(btn6, "2");

            System.Windows.Application.Current.Shutdown();
        }

        private void CheckButtonContent(Button b, string expectedContent)
        {
            if (b.Content.ToString() != expectedContent)
            {
                GlobalLog.LogEvidence("Button " + b.Name + "'s content did not match. Expected: " + expectedContent + " Found: " + b.Content.ToString());
                TestLog.Current.Result = TestResult.Fail;
            }
            TestLog.Current.Result = TestResult.Pass;
        }

    }
}
