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
    /// Test for Detaching SizeChanged Event
    ///
    /// </summary>
    public partial class SizeChanged_Detach
    {                                                                                                                                                     
        AutomationHelper _AH = new AutomationHelper();
        
        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Logger.LogFail("SizeChanged Event should not have been caught!");
        }
        
        void OnContentRendered(object sender, EventArgs e)
        {
            Logger.Status("Attaching Window.SizeChanged Event Handler");
            this.SizeChanged += new SizeChangedEventHandler(OnSizeChanged);

            Logger.Status("Detaching Window.SizeChanged Event Handler");
            this.SizeChanged -= OnSizeChanged;

            Logger.Status("[SET] Window.Width - Expect HitCount");
            this.Width++;

            Logger.Status("[SET] Width.Height - Expect HitCount");
            this.Height++;

            Logger.Status("[AUTOMATION] DragDrop Upper Left Corner");
            Point fromPoint = new Point((int)(this.Left), (int)(this.Top));
            Point toPoint = new Point((int)(this.Left - 20), (int)(this.Top - 20));               
            _AH.DragDrop(fromPoint, toPoint, Cleanup);
        }

        void Cleanup()
        {
            TestHelper.Current.TestCleanup();
        }            
    }

}
