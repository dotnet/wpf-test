// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Wpf.AppModel.HyperlinkSpoofing
{
    /// <summary>
    /// Interaction logic for HyperlinkSpoofing.xaml
    /// </summary>
    public partial class Start_HyperlinkSpoofing : Page
    {
        #region Private Data

        string _eventName = "";

        #endregion

        #region Private Methods

        void OnLoaded(object sender, EventArgs e)
        {
            _eventName = DriverState.DriverParameters["TargetEvent"];
            GlobalLog.LogEvidence("target event is: " + _eventName);
            Application.Current.MainWindow.Title = "Loaded";
        }

        private void hl_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            GlobalLog.LogEvidence("in PreviewMouseUp");
            if (_eventName == "PreviewMouseUp")
            {
                hl.NavigateUri = new Uri("http://www.microsoft.com");
            }
        }

        private void hl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            GlobalLog.LogEvidence("in PreviewMouseLeftButtonUp");
            if (_eventName == "PreviewMouseLeftButtonUp")
            {
                hl.NavigateUri = new Uri("http://www.microsoft.com");
            }
        }

        private void hl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            GlobalLog.LogEvidence("in PreviewKeyDown");
            if (_eventName == "PreviewKeyDown")
            {
                if ((hl.IsKeyboardFocused) && (e.Key == Key.Enter))
                {
                    hl.NavigateUri = new Uri("http://www.microsoft.com");
                }
            }
        }

        #endregion
    }
}
