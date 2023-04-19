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
    /// Test for setting Window.Height property in Markup
    ///
    /// </summary>
    public partial class Height_Markup
    {                                                                          
        void OnContentRendered(object sender, EventArgs e)
        {            
            // Setting Window.Height to normal size (ie: >27) has been covered by FlowDirection tests

            // Expected Value
            double expectedValue = (TestUtil.IsThemeClassic && TestUtil.DPIXRatio > 1) ? 25.6 : 27;
            Logger.Status("[EXPECTED] Window.ActualHeight >= " + expectedValue.ToString());
            if (this.ActualHeight < expectedValue)
            {
                Logger.LogFail("[ACTUAL] Window.ActualHeight = " + this.ActualHeight.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Window.ActualHeight = " + this.ActualHeight.ToString());
            }
            
            // Expected Value
            expectedValue = 300;
            Logger.Status("[SET] Window.Height = " + expectedValue.ToString());
            this.Height = expectedValue;
            Validate(expectedValue);

            TestHelper.Current.TestCleanup();

        }

        void Validate(double expectedValue)
        {
            // Validate Property Value
            if (this.Height != expectedValue)
            {
                Logger.LogFail("this.Height==" + this.Height.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Property Value Validated");
            }
                
            // Win32 Validation
            if (WindowValidator.ValidateHeight(this.Title, expectedValue))
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
