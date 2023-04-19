// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Automation;
using Microsoft.Windows.Test.Client.AppSec.BVT.ELCommon;
using Microsoft.Test.Win32;
using System.Windows.Controls;
using Microsoft.Test.Input;

namespace WindowTest
{
    /// <summary>
    /// 
    /// Test for setting WindowState.Maximized property in markup
    ///
    /// </summary>
    public partial class WindowState_Maximized_Markup
    {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
        void OnContentRendered(object sender, EventArgs e)
        {

        	
    	    WindowState ws = WindowState.Maximized;
    	    Logger.Status("Window.WindowState = " + ws.ToString());
            if (this.WindowState == ws)
            {
                Logger.Status("[VALIDATION PASSED] Window.WindowState == " + ws.ToString());
            }
            else
            {
                Logger.LogFail("Window.WindowState != " + ws.ToString());
            }

            if (WindowValidator.ValidateWindowState(this.Title, ws))
            {
                Logger.Status("[VALIDATION PASSED] Win32 Validation Passed");
            }
            else
            {
                Logger.LogFail("Win32 WindowState Validation Failed.");
            }

            // VALIDATION: Maximized->Minimized
		    ws = WindowState.Minimized;
		    this.WindowState = ws;
    	    Logger.Status("Window.WindowState = " + ws.ToString());
            if (this.WindowState == ws)
            {
                Logger.Status("[VALIDATION PASSED] Window.WindowState == " + ws.ToString());
            }
            else
            {
                Logger.LogFail("Window.WindowState != " + ws.ToString());
            }

            if (WindowValidator.ValidateWindowState(this.Title, ws))
            {
                Logger.Status("[VALIDATION PASSED] Win32 Validation Passed");
            }
            else
            {
                Logger.LogFail("Win32 WindowState Validation Failed.");
            }

            // Reset WindowState back to Maximized;
            Logger.Status("Reset WindowState to Maximized");
            this.WindowState = WindowState.Maximized;				

            // VALIDATION: Maximized->Normal
		    ws = WindowState.Normal;
		    this.WindowState = ws;
    	    Logger.Status("Window.WindowState = " + ws.ToString());
            if (this.WindowState == ws)
            {
                Logger.Status("[VALIDATION PASSED] Window.WindowState == " + ws.ToString());
            }
            else
            {
                Logger.LogFail("Window.WindowState != " + ws.ToString());
            }

            if (WindowValidator.ValidateWindowState(this.Title, ws))
            {
                Logger.Status("[VALIDATION PASSED] Win32 Validation Passed");
            }
            else
            {
                Logger.LogFail("Win32 WindowState Validation Failed.");
            }

		    TestHelper.Current.TestCleanup();

        }
    }

}
