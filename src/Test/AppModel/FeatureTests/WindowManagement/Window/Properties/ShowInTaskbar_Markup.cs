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
    /// Test for show in taskbar - markup
    ///
    /// </summary>
    public partial class ShowInTaskbar_Markup
    {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            
        void OnContentRendered(object sender, EventArgs e)
        {
            bool expectedValue = false;

            if (this.ShowInTaskbar != expectedValue)
            {
                Logger.LogFail("Default ShowInTaskbar != " + expectedValue.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Default Window.ShowInTaskBar == " + expectedValue.ToString());
            }

            expectedValue = true;
		    Logger.Status("[SET] Window.ShowInTaskBar = " + expectedValue.ToString());
            this.ShowInTaskbar = expectedValue;

            if (this.ShowInTaskbar != expectedValue)
            {
                Logger.LogFail("ShowInTaskbar != " + expectedValue.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Window.ShowInTaskBar == " + expectedValue.ToString());
            }

            if (!WindowValidator.ValidateShowInTaskbar(this.Title, expectedValue))
            {
                Logger.LogFail("Win32 Validation Failed!");
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Win32 Validation Passed");
            }

            expectedValue = false;
		    Logger.Status("[SET] Window.ShowInTaskBar = " + expectedValue.ToString());
            this.ShowInTaskbar = expectedValue;

            if (this.ShowInTaskbar != expectedValue)
            {
                Logger.LogFail("ShowInTaskbar != " + expectedValue.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Window.ShowInTaskBar == " + expectedValue.ToString());
            }

            if (!WindowValidator.ValidateShowInTaskbar(this.Title, expectedValue))
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
