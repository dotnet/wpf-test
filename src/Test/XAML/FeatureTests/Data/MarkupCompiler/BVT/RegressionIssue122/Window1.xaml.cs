// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;

namespace RegressionIssue122
{
    /// <summary>
    /// Test case verifies attached events are fired
    /// </summary>
    public partial class Window1 : Window
    {
        private bool _eventRaised = false;
        public Window1()
        {
            InitializeComponent();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            button.RaiseEvent(new RoutedEventArgs(AttachedEventSource.AttachEvent));


            if (_eventRaised)
            {
                Application.Current.Shutdown(0);
            }
            else
            {
                Application.Current.Shutdown(-1);
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button1_Attach(object sender, RoutedEventArgs e)
        {
            _eventRaised = true;
        }
    }
}
