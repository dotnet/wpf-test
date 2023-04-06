// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Windows.Test.Client.AppSec.BVT.ELCommon;

namespace WindowTest
{
    /// <summary>
    /// New for Dev10: WindowInteropHelper.EnsureHandle negative property-related tests
    /// </summary>
    public partial class EnsureHandleNegativeProperties
    {
        void OnContentRendered(object sender, EventArgs e)
        {
#if TESTBUILD_CLR40
            bool caught = false;
            Window newWindow = new Window();
            WindowInteropHelper interopHelper = new WindowInteropHelper(newWindow);

            // Case 1: Call EnsureHandle, then set AllowsTransparency.  Should throw an InvalidOperationException.
            Logger.Status("Calling EnsureHandle, then AllowsTransparency");
            try
            {
                interopHelper.EnsureHandle();
                newWindow.AllowsTransparency = true;
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogPass("Expected exception caught: " + ex.ToString());
                caught = true;
            }
            catch (Exception ex2)
            {
                Logger.LogFail("Unxpected exception caught: " + ex2.ToString());
            }

            if (caught)
            {
                Logger.LogPass("Call to EnsureHandle, then AllowsTransparency threw exception, as expected.");
            }
            else
            {
                Logger.LogFail("Call to EnsureHandle, then AllowsTransparency either did not throw an exception, or threw an unexpected one.");
            }

            newWindow.Close();

            caught = false;
            newWindow = new Window();
            interopHelper = new WindowInteropHelper(newWindow);

            // Case 2: Call AllowsTransparency (but not WindowStyle.None) then EnsureHandle.  Should throw an InvalidOperationException.
            Logger.Status("Calling AllowsTransparency (but not WindowStyle.None), then EnsureHandle");
            try
            {
                newWindow.AllowsTransparency = true;
                interopHelper.EnsureHandle();
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogPass("Expected exception caught: " + ex.ToString());
                caught = true;
            }
            catch (Exception ex2)
            {
                Logger.LogFail("Unxpected exception caught: " + ex2.ToString());
            }

            if (caught)
            {
                Logger.LogPass("Call to AllowsTransparency w/o WindowStyle.None, then EnsureHandle, threw exception, as expected.");
            }
            else
            {
                Logger.LogFail("Call to AllowsTransparency w/o WindowStyle.None, then EnsureHandle, either did not throw an exception, or threw an unexpected one.");
            }

            newWindow.Close();
#endif

            Logger.LogPass("test complete");

            this.Close();
        }
    }
}
