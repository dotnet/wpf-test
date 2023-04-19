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
    /// Test for calling Window.DragMove() method
    ///
    /// </summary>
    public partial class DragMove
    {         
        AutomationHelper _AH = new AutomationHelper();
        double _expectedTop,_expectedLeft;
        
        void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        
        void OnContentRendered(object sender, EventArgs e)
        {
            double DragDropDelta = 20;
            _expectedTop = this.Top + DragDropDelta;
            _expectedLeft = this.Left + DragDropDelta;
            
            Logger.Status("Trying calling DragMove outside MouseLeftButtonDown Event Hander");
            try
            {
                this.DragMove();
                Logger.LogFail("Excepted InvalidOperationException did not fire!");
            }
            catch(System.InvalidOperationException exception)
            {
                Logger.Status("[VALIDATION PASSED] InvalidOperationException caught as expected\n" + exception.ToString());
            }
            catch(System.Exception exception)
            {
                Logger.LogFail("Unexpected Exception caught! Detail:\n"+ exception.ToString());
            }
            
            Logger.Status("Drag Window Content");
            Point InsidePoint = new Point(this.Left + this.ActualWidth / 2, this.Top + this.ActualHeight / 2);
            Point OutsidePoint = new Point(InsidePoint.X + DragDropDelta, InsidePoint.Y + DragDropDelta);

            Point insidePointDeviceUnits = TestUtil.LogicalToDeviceUnits(InsidePoint, this);
            Point outsidePointDeviceUnits = TestUtil.LogicalToDeviceUnits(OutsidePoint, this);
            
            _AH.DragDrop(insidePointDeviceUnits, outsidePointDeviceUnits, Validate);
        }

        void Validate()
        {
            Logger.Status("[EXPECTED] Location = " + _expectedLeft.ToString() + "," + _expectedTop.ToString());

            if (this.Left != _expectedLeft || this.Top != _expectedTop)
            {
                Logger.LogFail("[ACTUAL] Location = " + this.Left.ToString() + "," + this.Top.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Size = " + this.Left.ToString() + "," + this.Top.ToString());
            }
            
            TestHelper.Current.TestCleanup();
        }
    }

}
