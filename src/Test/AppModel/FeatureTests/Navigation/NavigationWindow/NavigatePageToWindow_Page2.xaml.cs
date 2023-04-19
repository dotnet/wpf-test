// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Test.Logging;
using Microsoft.Windows.Test.Client.AppSec.Navigation;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class NavigatePageToWindow_Page2
    {
		void OnLoaded(object source, EventArgs e)
		{
			NavigationWindow nwSource = source as NavigationWindow;
			Window wSource = source as Window;

			NavigationHelper.ActualTestCount++;
			Log.Current.CurrentVariation.LogMessage("secondWindowPageCB.xaml is loaded");

			if (nwSource !=null)
			{
                Log.Current.CurrentVariation.LogMessage("loaded source is navwin");
			}
			else if (wSource != null)
			{
                Log.Current.CurrentVariation.LogMessage("loaded source is window");
				NavigationHelper.ActualTestCount++;
			}
			else
			{
                Log.Current.CurrentVariation.LogMessage("loaded source is neither navwin nor window");
			}

			NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
			if (nw != null)
                Log.Current.CurrentVariation.LogMessage("nw is not null");
			else
			{
                Log.Current.CurrentVariation.LogMessage("navigationwindow is null");
				NavigationHelper.Fail("NavigationWindow is now null");
			}

            Log.Current.CurrentVariation.LogMessage("Number of windows is: " + Application.Current.Windows.Count.ToString());
			if (Application.Current.Windows.Count != 2)
				NavigationHelper.Fail("Wrong number of windows");

            NavigationHelper.FinishTest(NavigationHelper.CompareFileNames(NavigationHelper.ExpectedFileName, new Uri( @"secondWindowPageCB.xaml", UriKind.RelativeOrAbsolute)));
		}
    }
}

