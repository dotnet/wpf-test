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
    /// Test for setting Window.Opacity property in Code
    ///
    /// </summary>
    public partial class Opacity_Code
    {                                                                                                                                                                                                                                                                                                                             
        void OnContentRendered(object sender, EventArgs e)
        {               
            // Expected Value
            double expectedValue = 0.2;
            Logger.Status("[SET] Window.Opacity = " + expectedValue.ToString());
            this.Opacity = expectedValue;
            Validate(expectedValue);

            // Expected Value
            expectedValue = 0;
            Logger.Status("[SET] Window.Opacity = " + expectedValue.ToString());
            this.Opacity = expectedValue;
            Validate(expectedValue);

            // Expected Value
            expectedValue = 1;
            Logger.Status("[SET] Window.Opacity = " + expectedValue.ToString());
            this.Opacity = expectedValue;
            Validate(expectedValue);


		    /* 




*/

		    TestHelper.Current.TestCleanup();

        }

        void Validate(double expectedValue)
        {
            // Validate Property Value
            if (this.Opacity != expectedValue)
            {
                Logger.LogFail("this.Opacity==" + this.Opacity.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Property Value Validated");
            }
        }
    }

}
