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
using System.ComponentModel;
using System.Globalization;
#if use_tools
using Microsoft.Windows.Test.Client.AppSec.P1;
#endif

namespace Microsoft.Windows.Test.Client.AppSec.P1
{
	public class DlgRetValTest :
#if use_tools 
	BaseTestPureApp 
#else 
	Application
#endif
	{
		#region protected
		protected Hashtable eventpipe;
		protected Window wMain, wDlg;
		#endregion
		
		public DlgRetValTest ()
		{
			eventpipe = new Hashtable (2);
			eventpipe["AppStarted"] = true;
			Description = "Dialog Result Test ";
			DialogUtilities.LogStatus (Description);
		}

		protected override void OnStartup (StartupEventArgs e)
		{
			wMain = new Window ();
			wMain.Left = 150;
			wMain.Top = 150;
			wMain.Show ();
			OnTestReady += new TestStep (CreateDialog);
			if (e.Args.Length < 1 || e.Args[0] == null)
			{
				DialogUtilities.LogFail("Incorrect Usage");
				DialogUtilities.LogStatus("Usage: param 1 is on of true, false etc.");
				Shutdown();
			}
			
			Description += ": " + e.Args[0];

			switch (e.Args[0].ToLower(CultureInfo.InvariantCulture)) {
				case "true":
					eventpipe["testtype"] = "true";
					break;
				case "false":
					eventpipe["testtype"] = "false";
					break;
				default:
					DialogUtilities.LogFail("Unknown test: " + e.Args[0]);
					break;
			}
			
			base.OnStartup (e);
		}

		private void CreateDialog ()
		{
			DialogUtilities.LogStatus ("Creating Dialog");
			wDlg = new Window ();
			wDlg.Title = "Dialog";
			wDlg.Width = 200;
			wDlg.Height = 200;
			wDlg.Left = 300;
			wDlg.Top = 300;
			wDlg.Owner = wMain;
			wDlg.Activated += new EventHandler (wDlg_Activated);
			wDlg.Closed += new EventHandler (wDlg_Closed);
			Button btn = new Button();
			btn.Content = "Done";
			btn.Click += new RoutedEventHandler(btn_Click);
			wDlg.Content = (btn);

			bool? dr = wDlg.ShowDialog ();

			if (Windows.Count > 1)
			{
				DialogUtilities.LogFail (
					"Wrong number of windows after dialog result was set." 
					+ "\nExpected: 1" + "   \tActual: " + Windows.Count);
				return;
			}
			else
			{
				DialogUtilities.LogStatus ("Correct number of windows after setting dialog result: " 
					+ Windows.Count);
			}

			DialogUtilities.LogStatus ("Return value is: " + dr.ToString ());
			CheckResult(dr);

			Shutdown ();
		}
		
		private void CheckResult(bool? dr) 
		{
			bool? EXPECTED_DIALOGRESULT = null;
			switch ((string)eventpipe["testtype"])
			{
				case "true":
					EXPECTED_DIALOGRESULT = true;
					break;
				case "false":
					EXPECTED_DIALOGRESULT = false;
					break;
				default:
					DialogUtilities.LogFail("Unknown value for test type.");
					return;
			}
			if (EXPECTED_DIALOGRESULT != dr)
			{
				DialogUtilities.LogFail (
				"Incorrect return value from the dialog : Expected: " 
				+ EXPECTED_DIALOGRESULT.ToString () 
				+ "  \tActual: " + dr.ToString ());
			}
			else
			{
				DialogUtilities.LogPass ("Correct return value: " 
					+ EXPECTED_DIALOGRESULT.ToString ());
			}
		}
	
		private void wDlg_Activated (object sender, EventArgs e)
		{
			wDlg.Activated -= new EventHandler (wDlg_Activated);
			DialogUtilities.LogStatus ("Dialog window activated");
			DialogUtilities.LogStatus ("Now closing dialog programmatically without returning a dialogresult.");
			PostTestItem (new TestStep (ClickDlgCloseBtn));
		}

		private void btn_Click(object sender, RoutedEventArgs e) 
		{
			switch ((string)eventpipe["testtype"])
			{
				case "true":
					wDlg.DialogResult = true;
					break;
				case "false":
					wDlg.DialogResult = false;
					break;
			}
		}

		private void wDlg_Closed (object sender, EventArgs e)
		{
			DialogUtilities.LogStatus ("Dialog window has closed.");
		}
		
		private void ClickDlgCloseBtn() 
		{
			DialogUtilities.LogStatus("Clicking Done button on dialog");
			Button b = wDlg.Content as Button;
			if (b == null) 
			{
				DialogUtilities.LogFail("Could not find the Dialog's button");
				return;
			}
			AutomationPeer ap = UIElementAutomationPeer.CreatePeerForElement(b);
			IInvokeProvider iip = (IInvokeProvider)ap.GetPattern(PatternInterface.Invoke);
			iip.Invoke();
		}
	} // of class
}
