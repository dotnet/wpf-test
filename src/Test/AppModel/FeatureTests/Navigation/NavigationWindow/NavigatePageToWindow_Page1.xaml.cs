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
    public partial class NavigatePageToWindow_Page1
    {
		void OnLoaded(object source, EventArgs e)
		{
			NavigationWindow nwSource = source as NavigationWindow;
			Window wSource = source as Window;

			Log.Current.CurrentVariation.LogMessage("panePage1.xaml is loaded");
			NavigationHelper.ActualTestCount++;

			if (nwSource !=null)
			{
				Log.Current.CurrentVariation.LogMessage("loaded source is navwin");
			}
			else if (wSource != null)
			{
				Log.Current.CurrentVariation.LogMessage("loaded source is window");
			}
			else
			{
				Log.Current.CurrentVariation.LogMessage("loaded source is neither window nor navigationwindow");
				NavigationHelper.ActualTestCount++;
			}
		}
    }
}

