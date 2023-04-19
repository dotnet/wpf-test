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
    /// Test for Left property in markup
    ///
    /// </summary>
    public partial class Left_Markup
    {                                                                                                                            
        void OnContentRendered(object sender, EventArgs e)
        {
            // Expected Value
            double expectedValue = 0;
            Logger.Status("[SET] Window.Left = " + expectedValue.ToString());

            // Validate Property Value
            if (!TestUtil.IsEqual(this.Left, expectedValue))
            {
                Logger.LogFail("this.Left==" + this.Left.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Property Value Validated");
            }
                
            // Win32 Validation
            if (WindowValidator.ValidateLeft(this.Title, expectedValue))
            {
                Logger.Status("[VALIDATION PASSED] Win32 Validation Passed");
            }
            else
            {
                Logger.LogFail("Win32 Validation Failed!");
            }

            // Expected Value
            expectedValue = -50;
            Logger.Status("[SET] Window.Left = " + expectedValue.ToString());
            this.Left = expectedValue;

            // Validate Property Value
            if (!TestUtil.IsEqual(this.Left, expectedValue))
            {
                Logger.LogFail("this.Left==" + this.Left.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Property Value Validated");
            }

            // Win32 Validation
            if (WindowValidator.ValidateLeft(this.Title, expectedValue))
            {
                Logger.Status("[VALIDATION PASSED] Win32 Validation Passed");
            }
            else
            {
                Logger.LogFail("Win32 Validation Failed!");
            }

            // Expected Value
            expectedValue = 200;
            Logger.Status("[SET] Window.Left = " + expectedValue.ToString());
            this.Left = expectedValue;

            // Validate Property Value
            if (!TestUtil.IsEqual(this.Left, expectedValue))
            {
                Logger.LogFail("this.Left==" + this.Left.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Property Value Validated");
            }

            // Win32 Validation
            if (WindowValidator.ValidateLeft(this.Title, expectedValue))
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
