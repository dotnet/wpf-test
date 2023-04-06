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
    /// Test for calling Window.Activate() method
    ///
    /// </summary>
    public partial class Activate
    {         
        AutomationHelper _AH = new AutomationHelper();
        int _expectedHitCount = 0;
        
        void OnActivated(object sender, EventArgs e)
        {
            Logger.RecordHit("OnActivated");
        }
        void OnContentRendered(object sender, EventArgs e)
        {
            Logger.Status("Attaching Activated Event Handler");
            this.Activated += new EventHandler(OnActivated);

            Logger.Status("Calling Activate() to an already activated window -- NOT Expecting Hit");
            this.Activate();

            Logger.Status("Calling Activate() to hidden window");
            this.Hide();
            this.Activate();
            Logger.Status("Showing a window -- Expecting Hit");
            _expectedHitCount++;
            this.Show();
            
            Logger.Status("Calling Activate() to Minimized Window -- Expecting Hit");
            this.WindowState = WindowState.Minimized;
            _expectedHitCount++;
            this.Activate();
            this.WindowState = WindowState.Normal;

            Logger.Status("Calling Activate() when there is another window on top of it -- Expecting Hit");
            Window win = new Window();
            win.Show();
            win.Left=100;
            win.Top=100;
            _expectedHitCount++;
            this.Activate();
            
            Logger.Status("Calling Activate() when there is topmost window on top of it -- Expecting Hit");
            _expectedHitCount++;
            win.Activate();
            win.Topmost = true;
            this.Activate();

            Logger.Status("Calling Activate() to InActive Window -- Expecting Hit");
            _expectedHitCount++;
            _AH.WaitThenMoveToAndClick(new Point((int)(this.Left - 1), (int)(this.Top - 1)), Validate);
        }

        void Validate()
        {
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, (System.Windows.Threading.DispatcherOperationCallback) delegate (object o)
            {
                this.Activate();

                Logger.Status("VALIDATE HITCOUNT");
                Logger.Status("[EXPECTED] HitCount = " + _expectedHitCount.ToString() );
                int ActualHitCount = Logger.GetHitCount("OnActivated");
                if (ActualHitCount != _expectedHitCount)
                {
                    Logger.LogFail("[ACTUAL] HitCount = " + ActualHitCount.ToString());
                }
                else
                {
                    Logger.LogPass("[ACTUAL] HitCount = " + ActualHitCount.ToString());
                }

                Application.Current.Shutdown();

                return null;
            },
            null
            );
        }
    }

}
