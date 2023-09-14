// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Input.MultiTouch.Tests
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ModalTouchWindow
    {
        public ModalTouchWindow()
        {
            InitializeComponent();
        }

        void OnNewWindowClick(object sender, RoutedEventArgs e)
        {
            var window = new ModalTouchWindow
            {
                Width = this.Width,
                Height = this.Height,
                Left = this.Left,
                Top = this.Top,
            };
            GlobalLog.LogEvidence("Opening a new ModalTouchWindow.");
            window.ShowDialog();
            GlobalLog.LogEvidence("Opened a new ModalTouchWindow.");
        }

        void OnCloseClick(object sender, RoutedEventArgs e)
        {
            GlobalLog.LogEvidence("Closing a ModalTouchWindow.");
            this.Close();
        }
    }
}
