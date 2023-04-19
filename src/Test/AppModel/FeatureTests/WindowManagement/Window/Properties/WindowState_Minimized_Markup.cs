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
    /// Test for setting WindowState.Minimized property in markup
    ///
    /// </summary>
    public partial class WindowState_Minimized_Markup
    {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
        void OnContentRendered(object sender, EventArgs e)
        {
    	    double expectedTop, expectedLeft, expectedWidth, expectedHeight;

        	
    	    this.Width=this.Height=expectedWidth=expectedHeight=this.Left=this.Top=expectedLeft=expectedTop=200;

    	    // VALIDATING FOR WindowState.Minimized
    	    WindowState ws = WindowState.Minimized;
            if (this.WindowState == ws)
            {
                Logger.Status("[VALIDATION PASSED] Window.WindowState == " + ws.ToString());
            }
            else
            {
                Logger.LogFail("Window.WindowState != Minimized");
            }

            if (WindowValidator.ValidateWindowState(this.Title, ws))
            {
                Logger.Status("[VALIDATION PASSED] Win32 Validation Passed");
            }
            else
            {
                Logger.LogFail("Win32 WindowState Validation Failed.");
            }

    	    // VALIDATING FOR WindowState.Maximized
		    ws = WindowState.Maximized;
		    this.WindowState = WindowState.Maximized;

            if (this.WindowState == ws)
            {
                Logger.Status("[VALIDATION PASSED] Window.WindowState == " + ws.ToString());
            }
            else
            {
                Logger.LogFail("Window.WindowState != Minimized");
            }

            if (WindowValidator.ValidateWindowState(this.Title, ws))
            {
                Logger.Status("[VALIDATION PASSED] Win32 Validation Passed");
            }
            else
            {
                Logger.LogFail("Win32 WindowState Validation Failed.");
            }
    		

    	    // VALIDATING FOR WindowState.Normal
		    ws = WindowState.Normal;
		    this.WindowState = WindowState.Normal;

            if (this.WindowState == ws)
            {
                Logger.Status("[VALIDATION PASSED] Window.WindowState == " + ws.ToString());
            }
            else
            {
                Logger.LogFail("Window.WindowState != Minimized");
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
