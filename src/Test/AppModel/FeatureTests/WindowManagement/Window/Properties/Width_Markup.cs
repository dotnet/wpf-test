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
    /// Test for setting Window.Width property in Markup
    ///
    /// </summary>
    public partial class Width_Markup
    {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
        void OnContentRendered(object sender, EventArgs e)
        {            
            // Setting Window.Width to normal size (ie: >150) has been covered by FlowDirection tests

            // Expected Value
            double expectedValue = 112;
            Logger.Status("[EXPECTED] Window.ActualWidth >= " + expectedValue.ToString());
            if (this.ActualWidth < expectedValue)
            {
                Logger.LogFail("[ACTUAL] Window.ActualWidth = " + this.ActualWidth.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Window.ActualWidth = " + this.ActualWidth.ToString());
            }
            
            // Expected Value
            expectedValue = 300;
            Logger.Status("[SET] Window.Width = " + expectedValue.ToString());
            this.Width = expectedValue;
            Validate(expectedValue);

		    TestHelper.Current.TestCleanup();

        }

        void Validate(double expectedValue)
        {
            // Validate Property Value
            if (this.Width != expectedValue)
            {
                Logger.LogFail("this.Width==" + this.Width.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Property Value Validated");
            }
                
            // Win32 Validation
            if (WindowValidator.ValidateWidth(this.Title, expectedValue))
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
