// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using Microsoft.Test.Logging;

namespace RoutedEventDefinedInLocalAssembly
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        bool _codeWiredHandlerFired = false;
        bool _xamlWiredHandlerFired = false;

        public Window1()
        {
            InitializeComponent();
            uc1.MyRequest += new RoutedEventHandler(uc1_MyRequest);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            uc1.Button_Click(this, new RoutedEventArgs());

            if (_codeWiredHandlerFired != true)
            {
                GlobalLog.LogEvidence("Code wired handler did not fire");
                Application.Current.Shutdown(-1);
            }

            if (_xamlWiredHandlerFired != true)
            {
                GlobalLog.LogEvidence("Xaml wired handler did not fire");
                Application.Current.Shutdown(-1);
            }

            Application.Current.Shutdown(0);

        }

        void uc1_MyRequest(object sender, RoutedEventArgs e)
        {
            // MessageBox.Show("fired the code wired handler");
            _codeWiredHandlerFired = true;
        }

        private void UserControl1_MyRequestViaXaml(object sender, MyRequestEventArgs e)
        {
            // MessageBox.Show("fired the xaml wired handler");
            _xamlWiredHandlerFired = true;
        }
    }
}
