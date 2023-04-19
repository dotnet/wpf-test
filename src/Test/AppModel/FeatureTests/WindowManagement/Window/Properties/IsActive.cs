// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Windows.Test.Client.AppSec.BVT.ELCommon;

namespace WindowTest
{
    /// <summary>
    /// 
    /// Test for IsActive() property
    ///
    /// </summary>
    public partial class IsActive
    {                                                                                          
        void OnContentRendered(object sender, EventArgs e)
        {            
            Logger.Status("[EXPECTED] Window.IsActive = True");
            if (!this.IsActive)
            {
                Logger.LogFail("[ACTUAL] Window.IsActive = " + this.IsActive.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Window.IsActive = " + this.IsActive.ToString());
            }

            //if we show another window, our window is deactivated.
            Window newWindow = new Window();
            newWindow.Show();

            Logger.Status("[EXPECTED] Window.IsActive = False");
            if (this.IsActive)
            {
                Logger.LogFail("[ACTUAL] Window.IsActive = " + this.IsActive.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Window.IsActive = " + this.IsActive.ToString());
            }
            
            newWindow.Close();

            TestHelper.Current.TestCleanup();
        }
    }
}
