// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using RoutedEventHolder;
using System;

namespace RegressionIssue52
{
    /// <summary>
    /// Regression test
    /// Tests whether an attached event defined in an external assembly and 
    /// listened by custom controls in a data template fires properly.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private bool _eventFired = false;

        private void Control_Loaded( object sender, RoutedEventArgs e )
        {
            (sender as FrameworkElement).RaiseEvent(new RoutedEventArgs { RoutedEvent = Service.TestEvent });
            if (_eventFired == true)
            {
                Console.WriteLine("Attached event fired successfully");
                Application.Current.Shutdown(0);
            }
            else
            {
                Console.WriteLine("Attached event did not fire");
                Application.Current.Shutdown(1);
            }
        }

        private void AttachedEventHandler( object sender, RoutedEventArgs e )
        {
            _eventFired = true;
        }
    }
    
    public class CustomControl : UserControl
    {
    }
}
