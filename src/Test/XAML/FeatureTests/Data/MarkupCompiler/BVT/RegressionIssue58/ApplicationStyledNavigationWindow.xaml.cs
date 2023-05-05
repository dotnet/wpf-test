// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XamlExperiment
{
    /// <summary>
    /// Interaction logic for ApplicationStyledNavigationWindow.xaml
    /// </summary>
    public partial class ApplicationStyledNavigationWindow : Application
    {
        NavigationWindow _win;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            _win = new NavigationWindow();
            _win.ContentRendered += new EventHandler(win_ContentRendered);
            _win.Show(); 
            Application.Current.Shutdown(0);
        }

        void win_ContentRendered(object sender, EventArgs e)
        {
            /// Some cleanup 
        }
    }
}
