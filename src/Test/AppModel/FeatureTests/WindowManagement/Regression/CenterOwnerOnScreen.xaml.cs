// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using Microsoft.Windows.Test.Client.AppSec.BVT.ELCommon;

namespace WindowTest
{
    // Description:
	// Verify that if a parent window is partly off-screen, a child window created with CenterOwner set should display fully on-screen.
    public partial class CenterOwnerOnScreen
    {
        Window _newWindow;

        void OnContentRendered(object sender, EventArgs e)
        {
            Logger.Status("Creating child window with CenterOwner set.  Verifying that despite the parent being partly offscreen, the child is fully on screen.");
            _newWindow = new Window();
            _newWindow.Height = 300;
            _newWindow.Width = 400;
            _newWindow.Owner = this;
            _newWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            _newWindow.Loaded += VerifyWindowPosition;
            _newWindow.Show();
        }

        void VerifyWindowPosition(object sender, EventArgs e)
        {
            if((_newWindow.Top < 0) || (_newWindow.Left < 0))
            {
                Logger.LogFail("A child window created with CenterOwner set did not display fully on-screen.  Top: " + _newWindow.Top + " Left: " + _newWindow.Left);
            }
            else
            {
                Logger.LogPass("As expected, a child window created with CenterOwner set was displayed fully on-screen.");
            }
            _newWindow.Close();

            //now move the parent off the bottom right and test again.
            Top = 2000; Left = 3000;

            Logger.Status("Creating 2nd child window with CenterOwner set.  Verifying that despite the parent being offscreen, the child is fully on screen.");
            _newWindow = new Window();
            _newWindow.Height = 300;
            _newWindow.Width = 400;
            _newWindow.Owner = this;
            _newWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            _newWindow.Loaded += VerifySecondWindowPosition;
            _newWindow.Show();
        }

        void VerifySecondWindowPosition(object sender, EventArgs e)
        {
            double right = _newWindow.Left + _newWindow.Width;
            double bottom = _newWindow.Top + _newWindow.Height;

            double screenRight = 0.0;
            // Height wouldnt matter unless screen res could drop below X by 300
            double screenBottom = SystemParameters.PrimaryScreenHeight;
            foreach (Screen aScreen in Screen.AllScreens)
            {
                screenRight += aScreen.Bounds.Width;
            }
            Logger.Status(Screen.AllScreens.Length + " Screens detected. Desktop right: " + screenRight + " bottom: " + screenBottom);

            if((bottom > screenBottom) || (right > screenRight))
            {
                Logger.LogFail("The 2nd child window created with CenterOwner set did not display fully on-screen.  Bottom: " + bottom + " Right: " + right);
            }
            else
            {
                Logger.LogPass("As expected, the 2nd child window created with CenterOwner set was displayed fully on-screen.");
            }

            _newWindow.Close();

            Close();
        }
    }
}
