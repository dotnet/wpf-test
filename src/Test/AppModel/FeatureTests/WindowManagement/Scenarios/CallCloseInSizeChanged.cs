// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Windows;
using System.Windows.Automation;
using Microsoft.Windows.Test.Client.AppSec.BVT.ELCommon;
using Microsoft.Test.Win32;

namespace WindowTest
{

    /// <summary>
    /// Test for calling Window.Close() method in StateChanged Event
    /// </summary>
    public partial class CallCloseInSizeChanged
    {
        void OnContentRendered(object sender, EventArgs e)
        {
            this.SizeChanged += OnSizeChanged;
        }

        void OnSizeChanged(object sender, EventArgs e)
        {
            Logger.Status("[OnSizeChanged] Calling Close()");
            this.Close();
        }

        void OnClosed(object Sender, EventArgs e)
        {
            Logger.Status("Closed Event Fired - Test case passes if Window was not forced to close due to Timeout.");
            TestHelper.Current.TestCleanup();
        }
    }

}
