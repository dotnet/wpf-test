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

#if use_tools
using Microsoft.Windows.Test.Client.AppSec.P1;
#endif

using MTI = Microsoft.Test.Input;

namespace Microsoft.Windows.Test.Client.AppSec.P1
{
	public class CheckDialogBlockage :
#if use_tools 
	BaseTestPureApp 
#else 
Application
#endif
	{
		Window _wDlg;

		public CheckDialogBlockage ()
		{
			eventpipe = new Hashtable (5);
			eventpipe["AppStarted"] = true;
			eventpipe["MainWindowStage"] = 0;
			eventpipe["DialogStage"] = 0;

			eventpipe["wDlg_lostfocus"] = false;
			Description = "This test will check to see if the owner window frame is pushed, and it blocks";
			DialogUtilities.LogStatus (Description);
		}

		protected override void OnStartup (StartupEventArgs e)
		{
			Window wMain = new Window ();
			wMain.Left = 100;
			wMain.Top = 100;
			wMain.Show ();
			wMain.LostKeyboardFocus += new KeyboardFocusChangedEventHandler (wMain_lostfocus);
			DialogUtilities.LogStatus ("Creating Dialog");
			_wDlg = new Window ();
			DialogUtilities.LogStatus ("Main window code should still run so far");
			eventpipe["MainWindowStage"] = 1;

			_wDlg.WindowStyle = WindowStyle.ThreeDBorderWindow;
			_wDlg.Title = "Dialog";
			_wDlg.Width = 100;
			_wDlg.Height = 100;
			_wDlg.Left = 350;
			_wDlg.Top = 350;
			_wDlg.Owner = wMain;
			_wDlg.Activated += new EventHandler (wDlg_Activated);

			bool? dr = _wDlg.ShowDialog ();
			eventpipe["MainWindowStage"] = 2;
			if ((int)eventpipe["DialogStage"] < 1)
			{
				DialogUtilities.LogFail ("Main window has not blocked while the dialog is open");
			}

			DialogUtilities.LogStatus (
				"Test for blocking finished. Shutting down app");
			if ((int)eventpipe["DialogStage"] == 1 && (int)eventpipe["MainWindowStage"] == 2)
				DialogUtilities.LogPass ("Owner window was blocked while its dialog was open");
			Shutdown ();
		}


		private void wDlg_Activated (object sender, EventArgs e)
		{
			_wDlg.Activated -= new EventHandler (wDlg_Activated);
			DialogUtilities.LogStatus ("Dialog window activated");

			if (((Window)sender).Title != "Dialog")
			{
				DialogUtilities.LogFail ("Test issue: wrong window set at top of z-order.. " + "test needs to be reworked");
				goto EndTest;
			}

			DialogUtilities.LogStatus ("Now checking modality by clicking outside the window");
			MTI.Input.MoveToAndClick (new Point (150, 150));
			DialogUtilities.LogStatus ("Finished clicking");

		EndTest:
			DialogUtilities.LogStatus ("Setting return value for dialog and closing it.");
			PostTestItem (new TestStep (RetDlg));
		}

		private void RetDlg ()
		{
			DialogUtilities.LogStatus ("Setting Dialog Result");
			if ((int)eventpipe["MainWindowStage"] > 1)
				DialogUtilities.LogFail ("Main window did not block.");
			eventpipe["DialogStage"] = 1;
			_wDlg.DialogResult = false;
		}

		private void wMain_lostfocus (object sender, KeyboardFocusChangedEventArgs args)
		{
			eventpipe["mainwindow_lostfocus"] = true;
		}

		private void wDlg_lostfocus (object sender, KeyboardFocusChangedEventArgs args)
		{
			eventpipe["wDlg_lostfocus"] = true;
			// if dialog is being closed and it loses focus, it should be ok, 
			// or if the entire app loses focus, then also dialog will lose
			// focus, else a fail. 
		}


		#region protected
		protected Hashtable eventpipe;
	#endregion
	}
}
