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
    /// WindowInteropHelper.EnsureHandle negative event-related tests
    /// </summary>
    public partial class EnsureHandleNegativeEvents
    {
        Window _newWindow;
        void OnContentRendered(object sender, EventArgs e)
        {
            _newWindow = new Window();            
            _newWindow.Closing += newWindow_Closing;
            _newWindow.Closed += newWindow_Closed;

            Logger.Status("Calling Window.Close");
            _newWindow.Close();
        }

        void newWindow_Closing(object sender, CancelEventArgs e)
        {
#if TESTBUILD_CLR40
            // Case 1: In response to Window.Closing, call WindowInteropHelper.EnsureHandle.  It should throw an InvalidOperationException.
            bool caught = false;
            try
            {
                Logger.Status("Calling WindowInteropHelper.EnsureHandle in Window.Closing event");
                WindowInteropHelper interopHelper = new WindowInteropHelper(newWindow);
                interopHelper.EnsureHandle();
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogPass("Expected exception caught: " + ex.ToString());
                caught = true;
            }
            catch (Exception ex2)
            {
                Logger.LogFail("Unexpected exception caught: " + ex2.ToString());
            }        

            if (caught)
            {
                Logger.LogPass("As expected, calling WindowInteropHelper.EnsureHandle in Window.Closing threw an InvalidOperationException.");
            }
            else
            {
                Logger.LogFail("Did not catch expected exception when calling EnsureHandle during Closing.");
            }
#endif
        }

        void newWindow_Closed(object sender, EventArgs e)
        {
#if TESTBUILD_CLR40
            // Case 2: In response to Window.Closed, call WindowInteropHelper.EnsureHandle.  It should throw an InvalidOperationException.
            bool caught = false;
            try
            {
                Logger.Status("Calling WindowInteropHelper.EnsureHandle in Window.Closed event");
                WindowInteropHelper interopHelper = new WindowInteropHelper(newWindow);
                interopHelper.EnsureHandle();
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogPass("Expected exception caught: " + ex.ToString());
                caught = true;
            }
            catch (Exception ex2)
            {
                Logger.LogFail("Unexpected exception caught: " + ex2.ToString());
            }        

            if (caught)
            {
                Logger.LogPass("As expected, calling WindowInteropHelper.EnsureHandle in Window.Closed threw an InvalidOperationException.");
            }
            else
            {
                Logger.LogFail("Did not catch expected exception when calling EnsureHandle during Closed.");
            }
#endif
            Logger.LogPass("test complete");

            this.Close();
        }
    }
}
