// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Windows.Test.Client.AppSec.BVT.ELCommon;
using Microsoft.Test.Win32;

namespace WindowTest
{
    /// <summary>
    /// 
    /// Test for Detaching Deactivated Event in Code
    ///
    /// </summary>
    public partial class Deactivated_Detach
    {                                                                                                                                                     
        void OnDeactivated(object sender, EventArgs e)
        {
            Logger.LogFail("Deactivated Event Fired");
        }
        
        void OnContentRendered(object sender, EventArgs e)
        {               
            Logger.Status("Attaching Window.Deactivated Event Handler");
            this.Deactivated += new EventHandler(OnDeactivated);

            Logger.Status("Detaching Window.Deactivated Event Handler");
            this.Deactivated -= OnDeactivated;

            //if we show then close another window, our window is deactivated, then activated.
            Window newWindow = new Window();
            newWindow.Show();
            
            Logger.Status("Activate Window");
            newWindow.Close();	
            
            TestHelper.Current.TestCleanup();
        }
    }
}
