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
    /// Test for Visibility.Hidden in code
    ///
    /// </summary>
    public partial class Visibility_Hidden_Code
    {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
        void OnLoaded(object sender, EventArgs e)
        {
            // BVT BLOCKER: 1223920
            // Invariant Assert when calling Window.Show after setting Visibility=Hidden
            
            // Validate default visibility
            Validate(Visibility.Visible);
            
            this.Visibility = Visibility.Hidden;
            Validate(Visibility.Hidden);
            
            this.Visibility = Visibility.Collapsed;
            Validate(Visibility.Collapsed);
            
            this.Visibility = Visibility.Visible;
            Validate(Visibility.Visible);
            
		    TestHelper.Current.TestCleanup();
        }

        void Validate(Visibility expectedValue)
        {
            // Validate Default Value
            Logger.Status("[EXPECTED] Window.Visibility = " + expectedValue.ToString());
            if (this.Visibility != expectedValue)
            {
                Logger.LogFail("[ACTUAL] Window.Visibility=" + this.Visibility.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Window.Visibility=" + this.Visibility.ToString());
            }

            // Win32 Validation
		    if (WindowValidator.ValidateVisibility(this.Title, expectedValue))
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
