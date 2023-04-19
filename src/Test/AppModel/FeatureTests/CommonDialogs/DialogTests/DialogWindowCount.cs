// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
using System.ComponentModel;
#if use_tools
using Microsoft.Windows.Test.Client.AppSec.P1;
#endif

namespace Microsoft.Windows.Test.Client.AppSec.P1
{
	public class DialogWindowCountVisibility :
#if use_tools 
	BaseTestNavApp 
#else 
	Application
#endif
	{
		#region protected
		protected Hashtable eventpipe;
		Window _wMain,_wDlg;
		#endregion
		
		public DialogWindowCountVisibility ()
		{
			eventpipe = new Hashtable (5);
			eventpipe["AppStarted"] = true;
			Description = "Check window count and Visibility after dialog is dismissed";
		}

		protected override void OnStartup (StartupEventArgs e)
		{
			_wMain = new Window ();
			_wMain.Left = 600;
			_wMain.Width = 200;
			_wMain.Top = 400;
			_wMain.Show ();
			DialogUtilities.LogStatus ("Creating Dialog");
			_wDlg = new Window ();
			_wDlg.WindowStyle = WindowStyle.SingleBorderWindow;
			_wDlg.Title = "Dialog";
			_wDlg.Width = 200;
			_wDlg.Height = 200;
			_wDlg.Left = 300;
			_wDlg.Top = 300;
			_wDlg.Owner = _wMain;
			_wDlg.Activated += new EventHandler (wDlg_Activated);
			_wDlg.Closing += new CancelEventHandler (wDlg_Closing);

			bool? dr = _wDlg.ShowDialog ();

			if (Windows.Count > 1)
			{
				DialogUtilities.LogFail ("Wrong number of windows after dialog has been closed." + "\nExpected: 1" + "   \tActual: " + Windows.Count);
				return;
			}
			else
			{
				DialogUtilities.LogStatus ("Correct number of windows after closing dialog: " + Windows.Count);
			}

			Shutdown ();
		}

		private void wDlg_Activated (object sender, EventArgs e)
		{
			_wDlg.Activated -= new EventHandler (wDlg_Activated);
			DialogUtilities.LogStatus ("Dialog window activated");
			if (((Window)sender).Title != "Dialog")
			{
				DialogUtilities.LogFail ("Test issue: wrong window set at top of z-order.. " + "test needs to be reworked");
			}

			DialogUtilities.LogStatus ("Setting return value for dialog.");
			PostTestItem (new TestStep (RetDlg));
		}

		private void wDlg_Closing (object sender, CancelEventArgs e)
		{
			DialogUtilities.LogPass ("Dialog Window closed on setting DialogResult");
		}

		private void RetDlg ()
		{
			DialogUtilities.LogStatus ("Setting Dialog Result");

			_wDlg.DialogResult = false;
		}
	}

	// dialog window cannot be reused. Since setting dialog result currently closes the window
	// this is spec issue that Hamid is following up on for M7
	public class DialogWindowReuse :
#if use_tools 
	BaseTestNavApp 
#else 
	Application
#endif
	{
	}
}
