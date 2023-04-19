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

using MTI = Microsoft.Test.Input;

namespace Microsoft.Windows.Test.Client.AppSec.P1
{
	public class ManualCloseExplicit :
#if use_tools 
	BaseTestNavApp 
#else 
	Application
#endif
	{
		#region protected
		protected Hashtable eventpipe;
		Window _wMain,_wDlg;
		public bool? EXPECTED_DIALOGRESULT = null;
		#endregion
		
		public ManualCloseExplicit ()
		{
			eventpipe = new Hashtable (2);
			eventpipe["AppStarted"] = true;
			Description = "Close Dialog manually without returning a Dialog Result";
			DialogUtilities.LogStatus (Description);
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
			_wDlg.Closed += new EventHandler (wDlg_Closed);
			bool? dr;
			//DialogUtilities.LogStatus ("Before displaying the dialog, dialogresult is " + dr.ToString());
			
			dr = _wDlg.ShowDialog ();	

			if (Windows.Count > 1)
			{
				DialogUtilities.LogFail ("Wrong number of windows after dialog has been closed manually." + "\nExpected: 1" + "   \tActual: " + Windows.Count);
				return;
			}
			else
			{
				DialogUtilities.LogStatus ("Correct number of windows after closing dialog: " + Windows.Count);
			}

			DialogUtilities.LogStatus ("Return value is: " + dr.ToString() );
			if (EXPECTED_DIALOGRESULT != dr)
			{
				DialogUtilities.LogFail ("Incorrect return value when closing the " 
				+ "dialog manually: Expected: " + EXPECTED_DIALOGRESULT.ToString()
				+ "  \tActual: " + dr.ToString());
			}
			else
			{
				DialogUtilities.LogPass ("Correct return value: " + EXPECTED_DIALOGRESULT.ToString());
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

			DialogUtilities.LogStatus ("Now closing dialog manually without returning a dialogresult.");
			ManualClose (_wDlg);
		}

		private void ManualClose (Window wWinToClose)
		{
			DialogUtilities.LogStatus("Closing by clicking on close box");
			bool fRTLWindow = IsRTLWindow (wWinToClose);
			DialogUtilities.LogStatus ("Is RTL window: " + fRTLWindow.ToString());
			double left_displacement = -14, top_displacement = +12;
			double leftstartcoord = wWinToClose.Left 
					+ ((fRTLWindow) ? (double)0 : (wWinToClose.Width));
			double topstartcoord  = wWinToClose.Top;
			if (fRTLWindow)
			{
				left_displacement *= -1;
			}
			double actual_left_coord = leftstartcoord + left_displacement;
			double actual_top_coord = topstartcoord + top_displacement;
			DialogUtilities.LogStatus ("Clicking at: (" 
				+ actual_left_coord + "," + actual_top_coord + ")");
			MTI.Input.MoveToAndClick (
				new Point( actual_left_coord, actual_top_coord));
		}
		
		private bool IsRTLWindow(Window wnd) {
			WindowInteropHelper wih = new WindowInteropHelper(wnd);
			IntPtr hwnd = wih.Handle;
			if (hwnd == IntPtr.Zero) 
				throw new ArgumentException("Window must be visible before you use this function");
			long style = GetWindowLong(hwnd, GWL_EXSTYLE);	
			return (style & WS_EX_LAYOUTRTL) == WS_EX_LAYOUTRTL;
		}

		private void wDlg_Closed (object sender, EventArgs e)
		{
			DialogUtilities.LogStatus ("Dialog window has closed.");
		}

		private void RetDlg ()
		{
			DialogUtilities.LogStatus ("Setting Dialog Result");

			_wDlg.DialogResult = false;
		}
		
		#region imports
		[DllImport ("user32.dll")]
		private static extern long GetWindowLong (IntPtr hwnd, int param);
		private const int GWL_EXSTYLE = -20;
		private const long WS_EX_LAYOUTRTL = 0x00400000L;
		#endregion
} // of class

	public class DlgCloseProgrammatic :
#if use_tools 
	BaseTestNavApp 
#else 
	Application
#endif
	{
		#region protected
		protected Hashtable eventpipe;
		Window _wMain,_wDlg;
		public bool? EXPECTED_DIALOGRESULT = null;
		#endregion
		
		public DlgCloseProgrammatic ()
		{
			eventpipe = new Hashtable (5);
			eventpipe["AppStarted"] = true;
			Description = "Close Dialog programmatically without returning a Dialog Result";
			DialogUtilities.LogStatus (Description);
		}

		protected override void OnStartup (StartupEventArgs e)
		{
			_wMain = new Window ();
			_wMain.Left = 150;
			_wMain.Top = 150;
			_wMain.Show ();
			PostTestItem(new TestStep(CreateDialog));
			base.OnStartup(e);
		}
		
		private void CreateDialog() 
		{
			DialogUtilities.LogStatus ("Creating Dialog");
			_wDlg = new Window ();
			_wDlg.WindowStyle = WindowStyle.None;
			_wDlg.Title = "Dialog";
			_wDlg.Width = 200;
			_wDlg.Height = 200;
			_wDlg.Left = 300;
			_wDlg.Top = 300;
			_wDlg.Owner = _wMain;
			_wDlg.Activated += new EventHandler (wDlg_Activated);
			_wDlg.Closed += new EventHandler (wDlg_Closed);

			bool? dr = _wDlg.ShowDialog ();

			if (Windows.Count > 1)
			{
				DialogUtilities.LogFail ("Wrong number of windows after dialog has been closed manually." + "\nExpected: 1" + "   \tActual: " + Windows.Count);
				return;
			}
			else
			{
				DialogUtilities.LogStatus ("Correct number of windows after closing dialog: " + Windows.Count);
			}

			DialogUtilities.LogStatus ("Return value is: " + dr.ToString ());
			if (EXPECTED_DIALOGRESULT != dr)
			{
				DialogUtilities.LogFail ("Incorrect return value when closing the " 
					+ "dialog manually: Expected: " 
					+ EXPECTED_DIALOGRESULT.ToString () 
					+ "  \tActual: " + dr.ToString ());
			}
			else
			{
				DialogUtilities.LogPass ("Correct return value: " + EXPECTED_DIALOGRESULT.ToString ());
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

			DialogUtilities.LogStatus ("Now closing dialog programmatically without returning a dialogresult.");
			PostTestItem(new TestStep(CloseDlg));
		}

		private void CloseDlg() 
		{
			_wDlg.Close ();
		}
		
		private void wDlg_Closed (object sender, EventArgs e)
		{
			DialogUtilities.LogStatus ("Dialog window has closed.");
		}

		
	} // of class
} // of namespace
