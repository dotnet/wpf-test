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
    /// Test for Attaching, firing and Cancelling Closing Event in Markup
    ///
    /// </summary>
    public partial class Closing_Attach_Markup
    {                                                                                                                       
        bool _cancelClosing = true;
        int _expectedHitCount = 2;
        void OnClosed(object sender, EventArgs e)
        {
            Logger.Status("[EXPECTED] Closing HitCount=" + _expectedHitCount.ToString());
            if (Logger.GetHitCount("OnClosing") != _expectedHitCount)
            {
                Logger.LogFail("[ACTUAL] Closing HitCount=" + Logger.GetHitCount("OnClosing").ToString());
            }
            else
            {
                Logger.Status("[ACTUAL] Closing HitCount=" + Logger.GetHitCount("OnClosing").ToString());
            }
                
            TestHelper.Current.TestCleanup();
        }
        void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Logger.RecordHit("OnClosing");
            if (_cancelClosing)
            {
                Logger.Status("Cancelling Closing");
                e.Cancel = true;
            }
        }
        
        void OnContentRendered(object sender, EventArgs e)
        {
            _cancelClosing = true;
            Logger.Status("Closing Window --> Should be cancelled");
            this.Close();

            _cancelClosing = false;
            Logger.Status("Closing Window");
            this.Close();

        }
    }

}
