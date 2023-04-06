// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Windows.Test.Client.AppSec.Navigation;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class NavigateWinToPage_Page1
    {
		void OnLoaded(object source, EventArgs e)
		{
			NavigationWindow nwSource = source as NavigationWindow;
			Window wSource = source as Window;

			NavigationHelper.Output("windowPage1.xaml is loaded");
			NavigationHelper.ActualTestCount++;

			if (nwSource !=null)
			{
				NavigationHelper.Output("loaded source is navwin");
			}
			else if (wSource != null)
			{
				NavigationHelper.Output("loaded source is window");
				NavigationHelper.ActualTestCount++;
			}
			else
			{
				NavigationHelper.Output("loaded source is neither");
			}

			NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
			if (nw != null)
			{
				NavigationHelper.Output("nw is not null");
                NavigationHelper.Output("Calling navigate on NavigateWinToPage_Page2.xaml");
                nw.Navigate(new Uri(@"NavigateWinToPage_Page2.xaml", UriKind.RelativeOrAbsolute));
			}
			else
			{
				NavigationHelper.Output("navigationwindow is null");
			}
			NavigationHelper.Output("Number of windows is: " + System.Windows.Application.Current.Windows.Count.ToString());
		}
    }
}

