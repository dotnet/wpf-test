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
using System.Windows.Automation.Peers;

#if use_tools
using Microsoft.Windows.Test.Client.AppSec.P1;
#endif

using MTI = Microsoft.Test.Input;

namespace Microsoft.Windows.Test.Client.AppSec.P1
{
	public class DialogTestApp :
#if use_tools 
	BaseTestPureApp 
#else 
        Application
#endif
	{
		Button _dlgbutton;

		/// <summary>
		/// App to test pure dialog. Will test:
		///	<list type="unordered">
		///	<item>modality</item>
		///	<item>will create a dialog in the normal flow of the app (as opposed to on starting up)</item>
		///	<item>will not try to click outside the window</item>
		///	<item>will set the owner window. (It will not cause modality if a window is nto set as owner)</item>
		///	</list>
		///	- modality
		///	- 
		/// </summary>
		public DialogTestApp ()
		{
			Description = "Basic modality test.";
			eventpipe = new Hashtable (5);
			eventpipe["AppStarted"] = true;
			eventpipe["mainwindow_lostfocus"] = false;
			eventpipe["wDlg_lostfocus"] = false;
			Description = "Test modality of a dialog";
		}

		protected override void OnStartup (StartupEventArgs e)
		{
			Window wMain = new Window ();

			wMain.Show ();
			wMain.LostKeyboardFocus += new KeyboardFocusChangedEventHandler (wMain_lostfocus);
			_dlgbutton = new Button ();
			_dlgbutton.Click += new RoutedEventHandler (b_Click);
			((IAddChild)_dlgbutton).AddText ("Show Dialog");
			wMain.Content = _dlgbutton;
			OnTestReady += new TestStep (OpenWindowByClick);

			//OnTestReady += new TestStep (CheckModality);
			base.OnStartup (e);
		}

		private void OpenWindowByClick ()
		{
			AutomationPeer ap = UIElementAutomationPeer.CreatePeerForElement(_dlgbutton);
			IInvokeProvider iip = (IInvokeProvider)ap.GetPattern(PatternInterface.Invoke);
			iip.Invoke();
		}

		private void b_Click (object sender, RoutedEventArgs e)
		{
			DialogUtilities.LogStatus ("Clicked Dialog Button. Creating Dialog");

			Window w2 = new Window ();

			w2.Title = "Dialog";
			if (MainWindow == null)
			{
				DialogUtilities.LogStatus ("MainWindow is null");
			}

			w2.Owner = Windows[0];//MainWindow;
			w2.Activated += new EventHandler (w2_Activated);
			w2.ShowDialog ();
			Shutdown ();
		}

		private void w2_Activated (object sender, EventArgs e)
		{
			DialogUtilities.LogStatus ("Dialog window activated");

			Window[] zorderarray = new Window[2];

			zorderarray[0] = Windows[1];
			zorderarray[1] = Windows[0];
			if (MainWindow == null) 
				DialogUtilities.LogStatus ("MainWindow is null");
			else
				DialogUtilities.LogStatus ("MainWindow is not null");

			if (((Window)sender).Title != "Dialog")
			{
				DialogUtilities.LogFail ("Test issue: wrong window set at top of z-order.. " + "test needs to be reworked");
				goto EndTest;
			}

			DialogUtilities.CheckModality (zorderarray);
			DialogUtilities.LogStatus ("Modality check finished");
			if (!CheckFocusOM ())
			{
				DialogUtilities.LogFail ("Focus sequence was wrong.");
				goto EndTest;
			}

			if (!CheckActivationDeactivationRecord ())
			{
				DialogUtilities.LogFail ("Windows' activated deactivated events indicate incorrect activation seq");
				goto EndTest;
			}

			DialogUtilities.LogPass ("Passed Window test for Modality");
		EndTest:
			DialogUtilities.LogStatus ("Test completed. Now returning a dialog result");
			PostTestItem (new TestStep(RetDlg));
			//Shutdown ();
		}

		private void RetDlg ()
		{
			Windows[1].DialogResult = true;
		}


		private bool CheckFocusOM ()
		{
//			bool result = (bool)eventpipe["mainwindow_lostfocus"] == true;
//			if (!result)
//			{
//				DialogUtilities.LogStatus ("Main window had not lost focus");
//				return result;
//			}
//
//			result = result && (bool)eventpipe["wDlg_lostfocus"] == false;
//			if (!result)
//			{
//				DialogUtilities.LogStatus ("Dlg window had lost focus");
//				DialogUtilities.LogStatus ("Focus check returning false");
//			}
//
//			return result;
			return true;
		}

		private bool CheckActivationDeactivationRecord ()
		{
			return true;
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

	public class DialogModality2 :
#if use_tools 
	BaseTestPureApp 
#else 
        Application
#endif
	{
		Button _dlgbutton;
		Window _wDlg;

		/// <summary>
		/// App to test pure dialog. Will test:
		///	<list type="unordered">
		///	<item>modality</item>
		///	<item>will create a dialog in the normal flow of the app (as opposed to on starting up)</item>
		///	<item>will click outside the window to ensure modality</item>
		///	<item>will set the owner window. (It will not cause modality if a window is nto set as owner)</item>
		///	</list>
		///	- modality
		///	- 
		/// </summary>
		public DialogModality2 ()
		{
			eventpipe = new Hashtable (5);
			eventpipe["AppStarted"] = true;
			eventpipe["mainwindow_lostfocus"] = false;
			eventpipe["wDlg_lostfocus"] = false;
			Description = "This test will click outside the window to check modality of " + "a modal dialog";
		}

		protected override void OnStartup (StartupEventArgs e)
		{
			Window wMain = new Window ();

			wMain.Left = 100;
			wMain.Top = 100;
			wMain.Show ();
			wMain.LostKeyboardFocus += new KeyboardFocusChangedEventHandler (wMain_lostfocus);
			_dlgbutton = new Button ();
			_dlgbutton.Click += new RoutedEventHandler (b_Click);
			((IAddChild)_dlgbutton).AddText ("Show Dialog");
			wMain.Content = (_dlgbutton);
			OnTestReady += new TestStep (OpenWindowByClick);
			base.OnStartup (e);
		}

		private void OpenWindowByClick ()
		{
			AutomationPeer ap = UIElementAutomationPeer.CreatePeerForElement(_dlgbutton);
			IInvokeProvider iip = (IInvokeProvider)ap.GetPattern(PatternInterface.Invoke);
			iip.Invoke();
		}

		private void b_Click (object sender, RoutedEventArgs e)
		{
			DialogUtilities.LogStatus ("Clicked Dialog Button. Creating Dialog");

			_wDlg = new Window ();

			_wDlg.Title = "Dialog";
			_wDlg.Width = 100;
			_wDlg.Height = 100;
			_wDlg.Left = 350;
			_wDlg.Top = 350;
			if (MainWindow == null)
			{
				DialogUtilities.LogStatus ("MainWindow is null");
			}

			_wDlg.Owner = Windows[0];//MainWindow;
			_wDlg.Activated += new EventHandler (wDlg_Activated);
			bool? dr = _wDlg.ShowDialog ();
			Shutdown ();
		}

		private void wDlg_Activated (object sender, EventArgs e)
		{
			_wDlg.Activated -= new EventHandler (wDlg_Activated);
			DialogUtilities.LogStatus ("Dialog window activated");

			Window[] zorderarray = new Window[2];

			zorderarray[0] = Windows[1];
			zorderarray[1] = Windows[0];
			if (((Window)sender).Title != "Dialog")
			{
				DialogUtilities.LogFail ("Test issue: wrong window set at top of z-order.. " + "test needs to be reworked");
				goto EndTest;
			}

			DialogUtilities.LogStatus ("Now checking modality by clicking outside the window");
			MTI.Input.MoveToAndClick (new Point (150, 150));
			DialogUtilities.LogStatus ("Finished clicking");
			DialogUtilities.CheckModality (zorderarray);
			
			DialogUtilities.LogStatus ("Again checking modality by clicking outside the window at another point");
			MTI.Input.MoveToAndClick (new Point (550, 300));
			DialogUtilities.LogStatus ("Finished clicking");
			DialogUtilities.CheckModality (zorderarray);

			if (!CheckFocusOM ())
			{
				DialogUtilities.LogFail ("Focus sequence was wrong");
				goto EndTest;
			}

			if (!CheckActivationDeactivationRecord ())
			{
				DialogUtilities.LogFail ("Windows' activated deactivated events indicate incorrect activation seq");
				goto EndTest;
			}

			DialogUtilities.LogPass ("Passed Window test for Modality");
		EndTest:
			DialogUtilities.LogStatus ("Test completed");
			PostTestItem (new TestStep (RetDlg));
			//wDlg.DialogResult = DialogResult.OK;
			//Shutdown ();
		}

		private void RetDlg ()
		{
			DialogUtilities.LogStatus ("Setting Dialog Result");
			_wDlg.DialogResult = false;
		}

		private bool CheckFocusOM ()
		{
			return true;
		}

		private bool CheckActivationDeactivationRecord ()
		{
			return true;
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
