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
    /// Test for Window.Top in code
    ///
    /// </summary>
    public partial class Top_Code
    {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        
        void OnContentRendered(object sender, EventArgs e)
        {
            // Validation Default Value
            if (this.Top < 0)
            {
                Logger.LogFail("Window should not start with negative Top. Window.Top==" + this.Top.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Default Top Value Validated to be positive.");
            }
            
            // Expected Value
            double expectedValue = 0;
            Logger.Status("[SET] Window.Top = " + expectedValue.ToString());
            this.Top = expectedValue;
            Validate(expectedValue);

            // Expected Value
            expectedValue = -50;
            Logger.Status("[SET] Window.Top = " + expectedValue.ToString());
            this.Top = expectedValue;
            Validate(expectedValue);

            // Expected Value
            expectedValue = 200;
            Logger.Status("[SET] Window.Top = " + expectedValue.ToString());
            this.Top = expectedValue;
            Validate(expectedValue);

            Logger.Status("[SET] WindowState=Maximized, Top=20");
            this.WindowState = WindowState.Maximized;
            this.Top = 20;
            Logger.Status("[SET] WindowState=Normal");
            this.WindowState = WindowState.Normal;
            expectedValue = 20;
            Validate(expectedValue);

            Logger.Status("[SET] WindowState=Minimized, Top=-1");
            this.WindowState = WindowState.Minimized;
            this.Top = -1;
            Logger.Status("[SET] WindowState=Normal");
            this.WindowState = WindowState.Normal;
            expectedValue = -1;
            Validate(expectedValue);
            
            TestHelper.Current.TestCleanup();

        }

        void Validate(double expectedValue)
        {
            Logger.Status("[EXPECTED] Top=" + expectedValue.ToString());
            
            // Validate Property Value
            if ((!(TestUtil.IsEqual(this.Top, expectedValue))) || (!TestUtil.IsEqual((double)(GetValue(Window.TopProperty)), expectedValue)))
            {
                Logger.LogFail("Window.Top==" + this.Top.ToString() + " GetValue(TopProperty)==" + GetValue(Window.TopProperty).ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Property Value Validated");
            }

            // Win32 Validation
            if (WindowValidator.ValidateTop(this.Title, expectedValue))
            {
                Logger.Status("[VALIDATION PASSED] Win32 Validation Passed");
            }
            else
            {
                Logger.LogFail("Win32 Validation Failed!");
            }
        }
    }

}
