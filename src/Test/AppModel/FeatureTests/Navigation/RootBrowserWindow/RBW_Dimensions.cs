// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Windows.Input;
using System.Text;
using System.Windows.Threading;
using System.Windows.Controls;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // Dimensions
    public partial class NavigationTests : Application
    {
        void RBWWindowDimensions_Startup(object sender, StartupEventArgs e)
        {
            _rbwTest = new RootBrowserWindowTestClass("RBWDimensions");
            this.MainWindow.GotKeyboardFocus +=new KeyboardFocusChangedEventHandler(RBWWindowDimensions_GotFocus);
        }

        void RBWWindowDimensions_GotFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            double NewWidth = 300, NewHeight = 500;

            _rbwTest.SetupTest();

            // [1] Change dimensions via Page's WindowWidth and WindowHeight
            _rbwTest.Output("RootBrowserWindow's starting Width: " + _rbwTest.NavWin.ActualWidth + 
                "; starting Height: " + _rbwTest.NavWin.ActualHeight);

            Page currRBWPage = LogicalTreeHelper.FindLogicalNode(_rbwTest.NavWin.Content as DependencyObject, "rbwTestPage") as Page;

            _rbwTest.Output("[SET] Page.WindowWidth = " + NewWidth);
            currRBWPage.WindowWidth = NewWidth;

            _rbwTest.Output("[SET] Page.WindowHeight = " + NewHeight);
            currRBWPage.WindowHeight = NewHeight;

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                                      (DispatcherOperationCallback)delegate(object o)
                                                      {
                                                          RBWWindowDimensions_Validate(NewWidth, NewHeight);
                                                          return null;
                                                      }, null);
        }


        void RBWWindowDimensions_Validate(double expectedWidth, double expectedHeight)
        {
            if (_rbwTest.NavWin.ActualWidth != expectedWidth || _rbwTest.NavWin.ActualHeight != expectedHeight)
            {
                _rbwTest.Output("Expected Dimension: " + expectedWidth + "x" + expectedHeight);
                _rbwTest.Fail("Actual Dimensions: " + 
                    _rbwTest.NavWin.ActualWidth.ToString() + "x" + _rbwTest.NavWin.ActualHeight.ToString());
            }
            else
            {
                _rbwTest.Pass("[VALIDATION PASSED] Actual Dimension: " + 
                    _rbwTest.NavWin.ActualWidth.ToString() + "x" + _rbwTest.NavWin.ActualHeight.ToString());
            }
        }
    }
}
