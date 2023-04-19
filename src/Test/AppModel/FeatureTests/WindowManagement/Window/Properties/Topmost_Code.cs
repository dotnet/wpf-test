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
    /// Test for Window.Topmost in code
    ///
    /// </summary>
    public partial class Topmost_Code
    {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  
        void OnContentRendered(object sender, EventArgs e)
        {
            // check for default value
            bool expectedValue = false;
            if (this.Topmost != expectedValue)
            {
                Logger.LogFail("Default Topmost != " + expectedValue.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Default Window.Topmost == " + expectedValue.ToString());
            }
            if (!WindowValidator.ValidateTopmost(this.Title, expectedValue))
            {
                Logger.LogFail("Win32 Validation Failed!");
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Win32 Validation Passed");
            }

            expectedValue = true;
		    Logger.Status("[SET] Window.Topmost = " + expectedValue.ToString());
            this.Topmost = expectedValue;

            if (this.Topmost != expectedValue)
            {
                Logger.LogFail("Topmost != " + expectedValue.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Window.Topmost == " + expectedValue.ToString());
            }

            if (!WindowValidator.ValidateTopmost(this.Title, expectedValue))
            {
                Logger.LogFail("Win32 Validation Failed!");
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Win32 Validation Passed");
            }

		    TestHelper.Current.TestCleanup();

        }
    }

}
