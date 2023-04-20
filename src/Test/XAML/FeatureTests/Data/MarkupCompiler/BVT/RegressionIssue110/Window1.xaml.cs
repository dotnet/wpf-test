// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;

namespace RegressionIssue110
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// Verify that MouseEnter, MouseLeave etc can be set on custom control in 
    /// XAML without using attached property (UIElement.MouseEnter) syntax
    /// </summary>

    public partial class Window1 : System.Windows.Window
    {

        public Window1()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            UserInput.MouseMove(this, 0, 0);
            UserInput.MouseMove(myThumb, (int)myThumb.Width / 2, (int)myThumb.Height / 2);
        }

        void MouseHandler(object sender, EventArgs e)
        {
            Microsoft.Test.Logging.TestLog log = TestLog.Current;
            log.Result = TestResult.Pass;
            Application.Current.Shutdown(0);
        }

    }

    public class MyThumb : Thumb
    {
        public bool MyThumbProperty
        {
            get
            {
                return this.IsDragging;
            }
        }
    }
	
}
