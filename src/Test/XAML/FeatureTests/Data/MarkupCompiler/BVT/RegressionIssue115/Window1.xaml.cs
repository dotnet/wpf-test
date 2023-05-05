// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;

namespace RegressionIssue115
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// Verify if CommandManager.CanExecute/CommandManager.Executed can be used in XAML
    /// </summary>
    public partial class Window1 : Window
    {
        private bool _hasCanExecuteFired = false;
        private bool _hasExecutedFired = false;

        public Window1()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, DependencyPropertyChangedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            if (menuItem.IsEnabled == true)
            {
                CommandManager.InvalidateRequerySuggested();
                UserInput.MouseLeftClickCenter(NewTask);
            }
        }

        private void NewTask_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            _hasCanExecuteFired = true;
            NewTask.IsEnabled = true;
            e.CanExecute = true;
        }

        private void NewTask_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _hasExecutedFired = true;
            if (_hasCanExecuteFired == true && _hasExecutedFired == true)
            {
                Application.Current.Shutdown(0);
            }
            else
            {
                GlobalLog.LogEvidence("fail: hasCanExecuteFired:" + _hasCanExecuteFired.ToString() + " hasExecutedFired:" + _hasExecutedFired.ToString());
                Application.Current.Shutdown(-1);
            }
        }
    }
}
