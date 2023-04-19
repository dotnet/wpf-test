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
    /// Test for Attaching and firing ContentRendered Event in Code
    ///
    /// </summary>
    public partial class ContentRendered_Attach_Code
    {                                                                                                                                                     
        int _expectedHitCount;
        
        void OnLoaded(object sender, EventArgs e)
        {
            _expectedHitCount = 1;
            Logger.Status("Attaching ContentRendered Event Handler");
            this.ContentRendered += new EventHandler(OnContentRendered);

            Logger.Status("Change Window.Content -- Expecting Hit");
            this.Content = new DockPanel();
            _expectedHitCount++;

            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (System.Windows.Threading.DispatcherOperationCallback) delegate (object o)
            {

                Logger.Status("[EXPECTED] HitCount = " + _expectedHitCount.ToString());
                int ActualHitCount = Logger.GetHitCount("OnContentRendered");
                if (ActualHitCount != _expectedHitCount)
                {
                    Logger.LogFail("[ACTUAL] HitCount = " + ActualHitCount.ToString());
                }
                else
                {
                    Logger.LogPass("[ACTUAL] HitCount = " + ActualHitCount.ToString());
                }   

                this.Close();
                return null;
            },
            null);
            
        }
        
        void OnContentRendered(object sender, EventArgs e)
        {
            Logger.RecordHit("OnContentRendered");
        }
    }

}
