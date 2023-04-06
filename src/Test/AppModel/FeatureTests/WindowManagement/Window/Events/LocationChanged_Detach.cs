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
    /// Test for Detaching LocationChanged Event in Code
    ///
    /// </summary>
    public partial class LocationChanged_Detach
    {                                                                                                                                                     
        AutomationHelper _AH = new AutomationHelper();
        // we set in markup to start window from origin of desktop
        
        void OnLocationChanged(object sender, EventArgs e)
        {
            Logger.LogFail("LocationEvent should not have fired!");
        }
        
        void OnContentRendered(object sender, EventArgs e)
        {
            Logger.Status("Attaching Window.LocationChanged Event Handler");
            this.LocationChanged += new EventHandler(OnLocationChanged);

            Logger.Status("Detaching Window.LocationChanged Event Handler");
            this.LocationChanged -= OnLocationChanged;

            Logger.Status("[SET] Window.Left - Expect HitCount");
            this.Left++;

            Logger.Status("[SET] Width.Top - Expect HitCount");
            this.Top++;

            Logger.Status("[AUTOMATION] DragDrop TopLeft Corner");
            Point fromPoint = new Point((int)(this.Left), (int)(this.Top));
            Point toPoint = new Point((int)(this.Left + 20), (int)(this.Top + 20));                
            _AH.DragDrop(fromPoint, toPoint, Cleanup);
        }

        void Cleanup()
        {
            TestHelper.Current.TestCleanup();
        }
    }

}
