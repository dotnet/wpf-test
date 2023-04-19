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
    /// Test for setting ResizeMode.CanResize property in code
    ///
    /// </summary>
    public partial class ResizeMode_CanResize_Code
    {                                                                                                                                                                                                                                                                                                                                                                                                                                       
        void OnContentRendered(object sender, EventArgs e)
        {
            // Expected Value
            ResizeMode expectedValue = ResizeMode.CanResize;
            Logger.Status("Expected ResizeMode = " + expectedValue.ToString());

            this.ResizeMode = expectedValue;
            Logger.Status("[SET] Window.ResizeMode = " + expectedValue.ToString());

            // Validate Value
            if (this.ResizeMode == expectedValue)
            {
                Logger.Status("[VALIDATION PASSED] ResizeMode validated");
            }
            else
            {
                Logger.LogFail("Actual ResizeMode = " + expectedValue.ToString());
            }

            // Win32 Validation
            if (WindowValidator.ValidateResizeMode(this.Title, expectedValue))
            {
                Logger.Status("[VALIDATION PASSED] Win32 Validation Passed");
            }
            else
            {
                Logger.LogFail("Win32 Validation Failed!");
            }

            // Set expectedValue to ResizeMode.NoResize
            expectedValue = ResizeMode.NoResize;
            Logger.Status("Expected ResizeMode = " + expectedValue.ToString());

            this.ResizeMode = expectedValue;
            Logger.Status("[SET] Window.ResizeMode = " + expectedValue.ToString());
            
            // Validate Default Value
            if (this.ResizeMode == expectedValue)
            {
                Logger.Status("[VALIDATION PASSED] ResizeMode validated");
            }
            else
            {
                Logger.LogFail("Actual ResizeMode = " + expectedValue.ToString());
            }

            // Win32 Validation
            if (WindowValidator.ValidateResizeMode(this.Title, expectedValue))
            {
                Logger.Status("[VALIDATION PASSED] Win32 Validation Passed");
            }
            else
            {
                Logger.LogFail("Win32 Validation Failed!");
            }

		    TestHelper.Current.TestCleanup();

        }
    }

}
