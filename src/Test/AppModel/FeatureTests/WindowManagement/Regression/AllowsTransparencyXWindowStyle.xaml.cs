// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Windows.Test.Client.AppSec.BVT.ELCommon;

namespace WindowTest
{
    // Description: 
    // Verify that if AllowsTransparency = true, setting WindowStyle to anything but None should get an InvalidOperationException.
    public partial class AllowsTransparencyXWindowStyle
    {
        void OnContentRendered(object sender, EventArgs e)
        {
            Logger.Status("With AllowsTransparency set to True, setting WindowStyle to anything but None.  Should get InvalidOperationException.");

            SetWindowStyleAndCatch(WindowStyle.SingleBorderWindow);
            SetWindowStyleAndCatch(WindowStyle.ThreeDBorderWindow);
            SetWindowStyleAndCatch(WindowStyle.ToolWindow);

            Close();
        }

        void SetWindowStyleAndCatch(WindowStyle windowStyle)
        {
            try
            {
                WindowStyle = windowStyle;
            }
            catch (InvalidOperationException exception)
            {
                Logger.Status("Got InvalidOperationException: " + exception.Message);
                Logger.LogPass("As expected, got InvalidOperationException setting WindowStyle to " + windowStyle.ToString() + " after AllowsTransparency.");
                return;
            }

            Logger.LogFail("Did not see the expected InvalidOperationException setting WindowStyle to " + windowStyle.ToString() + " after AllowsTransparency.");
        }
    }
}
