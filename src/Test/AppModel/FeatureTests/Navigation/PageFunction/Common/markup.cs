// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
*************************************************************************************
*                                                                                   *
*  Title:                                                                           *
*                                                                                   *
*  Description:                                                                     *
*                                                                                   *
*                                                                                   *
*                                                                                   *
*                                                                                   *
*                                                                                   * 
*************************************************************************************
*/

#define use_tools
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Markup;
using System.Collections;
using System.Windows.Input;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Automation.Provider;
using System.Windows.Automation;


namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
	public partial class PageFunctionTestApp
	{
		public void Test_MarkupPF ()
		{
            NavigationHelper.CreateLog("Basic Markup PF");
			NavigationHelper.Output("Basic Markup PageFunction test");
            Application.Current.StartupUri = new Uri("markuppf.xaml", UriKind.RelativeOrAbsolute);
            Application.Current.LoadCompleted += new LoadCompletedEventHandler(Load_MarkupPF);
		}

		private void Load_MarkupPF (object sender, NavigationEventArgs e)
		{
			NavigationHelper.Output ("Navigation Initiator property is: " + e.IsNavigationInitiator);
			if (e.Navigator is NavigationWindow)
			{
				stage++;
				switch (stage)
				{
					case 1:
						PostVerificationItem (new VerificationDelegate (Verify_MarkupPF));
						break;

					case 2:
//						PostVerificationItem (new VerificationDelegate (Verify_BackTest));
						break;

					case 3:
//						PostVerificationItem (new VerificationDelegate (Verify_BackTest2));
						break;
				}
			}
			else
			{
				NavigationHelper.Output ("Sender is: " + e.Navigator.GetType ().ToString ());
			}
		}

		private void Verify_MarkupPF ()
		{
			PageFunction<Int32> markuppf = MainNavWindow.Content as PageFunction<Int32>;
			if (markuppf == null)
			{
				NavigationHelper.Fail ("Content in Main App Window is incorrect: " 
				+ "\nExpected: PageFunction<Int32>" 
				+ "\nActual: " + MainNavWindow.Content.GetType().ToString());
				return;
			}
			if (!(markuppf.Content is TextBlock))
			{
				NavigationHelper.Fail ("Bad tree structure inside markup pagefunction. \nActual: " + markuppf.Content.GetType ().ToString ());
				return;
			}

			NavigationHelper.Pass ("Good state of markup pagefunction detected");
		}
	}
}
