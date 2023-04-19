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
    /// Test for restoring bounds
    ///
    /// </summary>
    public partial class RestoreBounds
    {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
        AutomationHelper _AH = new AutomationHelper();
        bool _exit = false;
        
        void OnContentRendered(object sender, EventArgs e)
        {               
            Validate();
            
            this.Width = 200;
            this.Height = 200;
            
            Validate();

            _exit = true;

            Point fromPoint = new Point(this.ActualWidth + this.Left - 1, this.ActualHeight + this.Top - 1);
            Point toPoint = new Point(fromPoint.X + 50, fromPoint.Y + 50);
            _AH.DragDrop(fromPoint, toPoint, Validate);


        }

        void Validate()
        {
            Logger.Status("[EXPECTED] Window.Rect (L,T,R,B)= " + this.Left.ToString() + 
                                                    "," + this.Top.ToString() +
                                                    "," + (this.ActualWidth + this.Left).ToString() +
                                                    "," + (this.ActualHeight + this.Top).ToString());
            if (!WindowValidator.ValidateRestoreBounds(this.Title, this.RestoreBounds))
            {
                Logger.LogFail("[ACTUAL] Window.Rect = " + this.RestoreBounds.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Window.Rect = " + this.RestoreBounds.ToString());
            }

            if (_exit)
            {
                TestHelper.Current.TestCleanup();
            }

        }
    }

}
