// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
using System.Collections;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
    /// - Changing the RelativeSource's Mode is not allowed. This test verifies that throws.
    /// - This test verifies that setting the RelativeSource's Mode to an invalid integer throws.
	/// </description>
	/// <relatedBugs>


	/// </relatedBugs>
	/// </summary>
    [Test(3, "Binding", "RelativeSourceBugCoverage")]
    public class RelativeSourceBugCoverage : WindowTest
    {
        private Binding _b1;

        public RelativeSourceBugCoverage()
        {
             
            RunSteps += new TestStep(SetupChangeRelativeSourceMode);
            RunSteps += new TestStep(ChangeRelativeSourceMode);
             
            RunSteps += new TestStep(SetRelativeSourceModeToInteger);
        }

        TestResult SetupChangeRelativeSourceMode()
        {
            Status("SetupChangeRelativeSourceMode");

            // Tree: Window - StackPanel - Button1
            StackPanel sp = new StackPanel();
            sp.Background = Brushes.AliceBlue;
            Button btn1 = new Button();
            sp.Children.Add(btn1);
            this.Window.Content = sp;

            // Setup binding
            _b1 = new Binding();
            _b1.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(StackPanel), 1);
            _b1.Path = new PropertyPath("Background");
            btn1.SetBinding(Button.ForegroundProperty, _b1);

            WaitForPriority(DispatcherPriority.SystemIdle);

            // Verify that the Button's Foreground is now the same as the StackPanel's Background (binding worked)
            if (btn1.Foreground != sp.Background)
            {
                LogComment("Fail - The FindAncestor initial binding failed. Button's Foreground: " + btn1.Foreground);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        TestResult ChangeRelativeSourceMode()
        {
            Status("ChangeRelativeSourceMode");

            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
            _b1.RelativeSource.Mode = RelativeSourceMode.Self;

            return TestResult.Pass;
        }

        TestResult SetRelativeSourceModeToInteger()
        {
            Status("SetRelativeSourceModeToInteger");

            SetExpectedErrorTypeInStep(typeof(ArgumentException));
            Binding b2 = new Binding();
            b2.RelativeSource = new RelativeSource((RelativeSourceMode)10);

            return TestResult.Pass;
        }
    }
}
