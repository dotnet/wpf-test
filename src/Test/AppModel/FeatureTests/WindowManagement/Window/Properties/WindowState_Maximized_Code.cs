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
    /// Test for setting WindowState.Maximized property in code
    ///
    /// </summary>
    public partial class WindowState_Maximized_Code
    {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
        void OnContentRendered(object sender, EventArgs e)
        {
    	    double expectedTop, expectedLeft, expectedWidth, expectedHeight;
    	    expectedWidth=expectedHeight=expectedLeft=expectedTop=200;

            // Validating Default WindowState and Size+Location
		    WindowState ws = WindowState.Normal;
            if (this.WindowState == ws)
            {
                Logger.Status("[VALIDATION PASSED] Default Window.WindowState == " + ws.ToString());
            }
            else
            {
                Logger.LogFail("Window.WindowState != " + ws.ToString());
            }

		    // VALIDATING FOR Window.Location and Window.Size
            if (WindowValidator.ValidateRestoreBounds(this.Title, expectedLeft, expectedTop, expectedWidth, expectedHeight))
            {
                Logger.Status("[VALIDATION PASSED] Window Size and Dimension Validated");
            }
            else
            {
                Logger.LogFail("Window Size and Dimension Validation Failed");
            }

            if (WindowValidator.ValidateWindowState(this.Title, ws))
            {
                Logger.Status("[VALIDATION PASSED] Win32 Validation Passed");
            }
            else
            {
                Logger.LogFail("Win32 WindowState Validation Failed.");
            }

    	    ws = WindowState.Maximized;
    	    Logger.Status("Window.WindowState = " + ws.ToString());
            this.WindowState = ws;
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

		    // VALIDATING FOR Window.Location and Window.Size
            if (WindowValidator.ValidateRestoreBounds(this.Title, expectedLeft, expectedTop, expectedWidth, expectedHeight))
            {
                Logger.Status("[VALIDATION PASSED] Window Size and Dimension Validated");
            }
            else
            {
                Logger.LogFail("Window Size and Dimension Validation Failed");
            }


		    TestHelper.Current.TestCleanup();

        }
    }

}
