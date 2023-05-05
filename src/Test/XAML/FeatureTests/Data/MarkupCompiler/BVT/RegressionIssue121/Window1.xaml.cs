// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;

namespace breadcrumb
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {

        private bool _eventRaised = false;
        public Window1()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));


            if (button.Style.Setters.Count == 2 && _eventRaised == true)
            {
                Application.Current.Shutdown(0);
            }
            else
            {
                Application.Current.Shutdown(-1);
            }

        }

        void OnButtonClick(object sender, RoutedEventArgs e)
        {
            this._eventRaised = true;
        }
    }
}
