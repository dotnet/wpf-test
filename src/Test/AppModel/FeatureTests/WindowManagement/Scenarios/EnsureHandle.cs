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
    /// WindowInteropHelper.EnsureHandle
    /// </summary>
    public partial class EnsureHandle
    {
        void OnContentRendered(object sender, EventArgs e)
        {
#if TESTBUILD_CLR40
            bool caught = false;
            WindowInteropHelper interopHelper = null;
            IntPtr handle = IntPtr.Zero;
            Window newWindow = new Window();
            newWindow.Owner = Application.Current.MainWindow;
            
            // Case 1: Call WindowInteropHelper.EnsureHandle
            // We expect a nonzero handle back.
            Logger.Status("Call WindowInteropHelper.EnsureHandle");
            try
            {
                interopHelper = new WindowInteropHelper(newWindow);
                handle = interopHelper.EnsureHandle();
            }
            catch (Exception ex)
            {
                Logger.LogFail("Unexpected exception caught: " + ex.ToString());
                caught = true;
            }

            if (!caught)
            {
                if ((interopHelper.Handle == handle) && (handle != IntPtr.Zero))
                {
                    Logger.LogPass("WindowInteropHelper.EnsureHandle returned the proper handle");
                }
                else
                {
                    Logger.LogFail("WindowInteropHelper.EnsureHandle failed to return the proper handle.  Got: " + handle.ToString());
                }
            }

            //this is to increase code coverage inside Window.SetRootVisualAndUpdateSTC            
            newWindow.SizeToContent = SizeToContent.WidthAndHeight;
            newWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;            

            newWindow.Show();

            // Case 2: Call EnsureHandle again.  Expect no issue calling it again.
            caught = false;
            Logger.Status("Call WindowInteropHelper.EnsureHandle again");
            try
            {
                handle = interopHelper.EnsureHandle();
            }
            catch (Exception ex)
            {
                Logger.LogFail("Unexpected exception caught: " + ex.ToString());
                caught = true;
            }

            if (!caught)
            {
                if ((interopHelper.Handle == handle) && (handle != IntPtr.Zero))
                {
                    Logger.LogPass("2nd call to WindowInteropHelper.EnsureHandle returned the proper handle");
                }
                else
                {
                    Logger.LogFail("2nd call to WindowInteropHelper.EnsureHandle failed to return the proper handle.  Got: " + handle.ToString());
                }
            }
#endif

            Logger.LogPass("test complete");

            this.Close();
        }
    }
}
