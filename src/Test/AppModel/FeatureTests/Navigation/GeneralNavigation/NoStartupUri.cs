// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Navigation;
using System.Data;
using System.Xml;
using System.Configuration;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// BVT that ensures that we can create and set a Page as RBW content
    /// in the OnStartup Override (without using a StartupUri)
    /// </summary>

    public partial class NavigationTests : Application
    {
        NavigationWindow _noStartupUri_navWin = null;
        String _noStartupUri_pageTitle = "Maroon Out !";
        String _noStartupUri_pageName = "StartPage";

        void NoStartupUri_Startup(object sender, StartupEventArgs e)
        {
            _noStartupUri_navWin = Application.Current.MainWindow as NavigationWindow;

            Page p = new Page();
            p.Name = _noStartupUri_pageName;
            p.WindowTitle = _noStartupUri_pageTitle;
            p.Background = Brushes.Maroon;
            _noStartupUri_navWin.Content = p;
            _noStartupUri_navWin.ContentRendered += new EventHandler(OnContentRendered_NoStartupUri_NW);
        }

        void OnContentRendered_NoStartupUri_NW(object sender, EventArgs e)
        {
            Page p = LogicalTreeHelper.FindLogicalNode(_noStartupUri_navWin.Content as DependencyObject, _noStartupUri_pageName) as Page;
            if (p == null || !_noStartupUri_navWin.Title.Equals(_noStartupUri_pageTitle))
            {
                NavigationHelper.Fail("Page p is null or windowTitle didn't match (actual title = " + _noStartupUri_navWin.Title);
            }
            else
            {
                NavigationHelper.Pass("Page p is non-null or windowTitle matches: " + _noStartupUri_navWin.Title);
            }
        }
    }
}
