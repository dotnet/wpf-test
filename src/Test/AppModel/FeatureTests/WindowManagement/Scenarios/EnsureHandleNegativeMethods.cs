// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Windows.Test.Client.AppSec.BVT.ELCommon;

namespace WindowTest
{
    /// <summary>
    ///  WindowInteropHelper.EnsureHandle
    /// </summary>
    public partial class EnsureHandleNegativeMethods
    {
        void OnContentRendered(object sender, EventArgs e)
        {
#if TESTBUILD_CLR40
            bool caught = false;
            WindowInteropHelper interopHelper = null;
            IntPtr handle = IntPtr.Zero;
            Window newWindow = new Window();
            newWindow.Owner = Application.Current.MainWindow;
            
            // Case 1: After calling WindowInteropHelper.EnsureHandle, call Hide
            // We expect a no-op.
            Logger.Status("Calling WindowInteropHelper.EnsureHandle, then Hide");
            try
            {
                interopHelper = new WindowInteropHelper(newWindow);
                handle = interopHelper.EnsureHandle();
                Logger.Status("Window.Visibility before hide: " + newWindow.Visibility.ToString());
                newWindow.Hide();
            }
            catch (Exception ex)
            {
                Logger.LogFail("Unexpected exception caught: " + ex.ToString());
                caught = true;
            }

            if (!caught)
            {
                Logger.Status("Window.Visibility after hide: " + newWindow.Visibility.ToString());
                Logger.LogPass("As expected, calling WindowInteropHelper.EnsureHandle then Hide was a no-op, and didn't throw an exception.");
            }

            // Case 2: Call DragMove.  Should throw an InvalidOperationException.
            caught = false;
            Logger.Status("Calling DragMove");
            try
            {
                newWindow.DragMove();
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogPass("Expected exception caught: " + ex.ToString());
                caught = true;
            }
            catch (Exception ex)
            {
                Logger.LogFail("Unexpected exception caught: " + ex.ToString());
            }

            if (caught)
            {
                Logger.LogPass("Call to DragMove threw InvalidOperationException as expected.");
            }
            else
            {
                Logger.LogFail("Call to DragMove silently returned, or threw unexpected exception.");
            }

            // Case 3: Call Activate.  Should throw an InvalidOperationException.
            caught = false;
            bool retval = true;
            Logger.Status("Calling Activate");
            try
            {
                retval = newWindow.Activate();
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogPass("Expected exception caught: " + ex.ToString());
                caught = true;
            }
            catch (Exception ex)
            {
                Logger.LogFail("Unexpected exception caught: " + ex.ToString());
            }

            if (caught)
            {
                Logger.LogPass("Call to Activate threw InvalidOperationException as expected.");
            }
            else
            {
                Logger.LogFail("Call to Activate silently returned, or threw unexpected exception.");
            }

            // Case 4: Call Close, then Show.  Should throw an InvalidOperationException.
            caught = false;
            retval = true;
            Logger.Status("Calling Close, then Show");
            try
            {
                newWindow.Close();
                newWindow.Show();
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogPass("Expected exception caught: " + ex.ToString());
                caught = true;
            }
            catch (Exception ex2)
            {
                Logger.LogPass("Unxpected exception caught: " + ex2.ToString());
            }

            if (caught)
            {
                Logger.LogPass("Call to Show after Close threw exception, as expected.");
            }
            else
            {
                Logger.LogFail("Call to Show after Close either did not throw an exception, or threw an unexpected one.");
            }
            

#endif

            Logger.LogPass("test complete");

            this.Close();
        }
    }
}
