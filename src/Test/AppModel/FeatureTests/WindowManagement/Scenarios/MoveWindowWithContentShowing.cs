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
    /// Test for move Avalon Window with content showing
    ///
    /// </summary>
    public partial class MoveWindowWithContentShowing
    {                   
        System.Timers.Timer _timer;
        int _LocationChangedCounter = 0;
        int _expectedLocationChangedHitCount = 200;
        double _expectedTop = 100;
        double _expectedLeft = 100;
        
        void OnLocationChanged(object sender, EventArgs e)
        {
            _LocationChangedCounter++;
            this.Title = _LocationChangedCounter.ToString();
        }
        
        void OnActivated(object sender, EventArgs e)
        {
             _expectedTop = 100;
             _expectedLeft = 100;
             Logger.Status("Validate Initial Location");
             _expectedLocationChangedHitCount = 0;
             Validate();

             _timer = new System.Timers.Timer(7000);
             _timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
             _timer.Start();
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _timer.Stop();
            _expectedTop = 600;
            _expectedLeft = 600;
            _expectedLocationChangedHitCount = 200;
            Logger.Status("Validate Final Location");
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback) delegate (object o)
            {
                Validate();
                Logger.LogPass();
                this.Close();
                return null;
            }, null);
        }

        private void Validate()
        {
            Logger.Status("[EXPECTED] Expected LocationChanged HitCount > " + _expectedLocationChangedHitCount.ToString());
            if (_LocationChangedCounter < _expectedLocationChangedHitCount)
            {
                Logger.LogFail("[ACTUAL] LocationChanged HitCount = " + _LocationChangedCounter.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] LocationChanged HitCount = " + _LocationChangedCounter.ToString());
            }

            Logger.Status("[EXPECTED] Window.Top  = " + _expectedTop.ToString());
            Logger.Status("[EXPECTED] Window.Left = " + _expectedLeft.ToString());
            if ((this.Left != _expectedLeft) || (this.Top != _expectedTop))
            {
                Logger.Status("[ACTUAL] Window.Top  = " + this.Top.ToString());
                Logger.Status("[ACTUAL] Window.Left = " + this.Left.ToString());
                Logger.LogFail("Property Validation Failed!");
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Window.Top  = " + this.Top.ToString());
                Logger.Status("[VALIDATION PASSED] Window.Left = " + this.Left.ToString());
            }
        }
    }

}
