// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // NavWinDimensions
    public partial class NavigationTests : Application
    {

        void RBWWindowNavWinDimensions_Startup(object sender, StartupEventArgs e)
        {
            _rbwTest = new RootBrowserWindowTestClass("RBWNavWinDimensions");
            this.MainWindow.GotKeyboardFocus +=new KeyboardFocusChangedEventHandler(RBWWindowNavWinDimensions_GotFocus);
        }

        void RBWWindowNavWinDimensions_GotFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            double NewWidth = 400, NewHeight = 400;

            _rbwTest.SetupTest();

            // [1] Change dimensions via NavigationWindow's Width and Height
            _rbwTest.Output("RootBrowserWindow's starting Width: " + _rbwTest.NavWin.ActualWidth + "; starting Height: " + _rbwTest.NavWin.ActualHeight);

            _rbwTest.Output("[SET] NavigationWindow.Width = " + NewWidth);
            _rbwTest.NavWin.Width = NewWidth;

            _rbwTest.Output("[SET] NavigationWindow.Height = " + NewHeight);
            _rbwTest.NavWin.Height = NewHeight;

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                                      (DispatcherOperationCallback)delegate(object o)
                                                      {
                                                          RBWWindowNavWinDimensions_Validate(NewWidth, NewHeight);
                                                          return null;
                                                      }, null);
        }

        void RBWWindowNavWinDimensions_Validate(double expectedWidth, double expectedHeight)
        {
            if (_rbwTest.NavWin.ActualWidth != expectedWidth || _rbwTest.NavWin.ActualHeight != expectedHeight)
            {
                _rbwTest.Output("Expected Dimension: " + expectedWidth + "x" + expectedHeight);
                _rbwTest.Fail("Actual Dimensions: " + _rbwTest.NavWin.ActualWidth + "x" + _rbwTest.NavWin.ActualHeight);
            }
            else
            {
                _rbwTest.Output("[VALIDATION PASSED] Actual Dimension: " + _rbwTest.NavWin.ActualWidth + "x" + _rbwTest.NavWin.ActualHeight);
            }

            _rbwTest.Pass("Dimensions changed vis NavigationWindow's width and height properties");
        }
    }
}
