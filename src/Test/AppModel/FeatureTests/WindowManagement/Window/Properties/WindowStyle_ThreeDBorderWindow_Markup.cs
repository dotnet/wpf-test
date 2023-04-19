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
    /// Test for setting WindowStyle_ThreeDBorderWindow property in markup
    ///
    /// </summary>
    public partial class WindowStyle_ThreeDBorderWindow_Markup
    {                                                                                     
        void OnContentRendered(object sender, EventArgs e)
        {
            WindowStyle expectedValue;
            
            // Expected Value
            expectedValue = WindowStyle.ThreeDBorderWindow;
            Validate(expectedValue);
            
            expectedValue = WindowStyle.ToolWindow;
		    Logger.Status("[SET] Window.WindowStyle = " + expectedValue.ToString());
            this.WindowStyle = expectedValue;
            Validate(expectedValue);

            expectedValue = WindowStyle.SingleBorderWindow;
		    Logger.Status("[SET] Window.WindowStyle = " + expectedValue.ToString());
            this.WindowStyle = expectedValue;
            Validate(expectedValue);
            
            expectedValue = WindowStyle.None;
		    Logger.Status("[SET] Window.WindowStyle = " + expectedValue.ToString());
            this.WindowStyle = expectedValue;
            Validate(expectedValue);              
            
		    TestHelper.Current.TestCleanup();
        }

        void Validate(WindowStyle expectedValue)
        {
            // Validate Property Value
            if (this.WindowStyle != expectedValue)
            {
                Logger.LogFail("WindowStyle != " + expectedValue.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Window.WindowStyle == " + expectedValue.ToString());
            }

            // Win32 Validation
            if (!WindowValidator.ValidateWindowStyle(this.Title, expectedValue))
            {
                Logger.LogFail("Win32 Validation Failed!");
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Win32 Validation Passed");
            }
        }
    }

}
