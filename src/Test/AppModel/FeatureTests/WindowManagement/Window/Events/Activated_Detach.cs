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
    /// Test for Detaching Activated Event in Code
    ///
    /// </summary>
    public partial class Activated_Detach
    {                                       
        void OnActivated(object sender, EventArgs e)
        {
            Logger.LogFail("OnActivated should not have been caught!");
        }
        
        void OnContentRendered(object sender, EventArgs e)
        {
            Logger.Status("Attaching Window.Activated Event Handler");
            this.Activated += new EventHandler(OnActivated);

            Logger.Status("Detaching Window.Activated Event Handler");
            this.Activated -= OnActivated;

            //if we show then close another window, our window is deactivated, then activated.
            Window newWindow = new Window();
            newWindow.Show();
            newWindow.Close();

            TestHelper.Current.TestCleanup();
        }
    }
}
