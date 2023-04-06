// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // WindowTitle
    public partial class NavigationTests : Application
    {
        void RBWWindowTitle_Startup(object sender, StartupEventArgs e)
        {
            _rbwTest = new RootBrowserWindowTestClass("RBWWindowTitle");
        }

        void RBWWindowTitle_LoadCompleted(object sender, NavigationEventArgs e)
        {
            string NewWindowTitle = "Hello, you're delicious";

            _rbwTest.SetupTest();

            // Check the starting title of the RootBrowserWindow
            _rbwTest.Output("Starting NavigationWindow title: " + _rbwTest.NavWin.Title);

            // Validate(String.Empty);
            Page currRBWPage = LogicalTreeHelper.FindLogicalNode(_rbwTest.NavWin.Content as DependencyObject, "rbwTestPage") as Page;

            _rbwTest.Output("[SET] Page.WindowTitle = " + NewWindowTitle);
            currRBWPage.WindowTitle = NewWindowTitle;

            RBWWindowTitle_Validate(NewWindowTitle);
        }

        void RBWWindowTitle_Validate(string expectedTitle)
        {
            if (!_rbwTest.NavWin.Title.Equals(expectedTitle))
            {
                _rbwTest.Output("Expected NavigationWindow title: " + expectedTitle);
                _rbwTest.Fail("Actual NavigationWindow title: " + _rbwTest.NavWin.Title);
            }
            else
            {
                _rbwTest.Pass("[VALIDATION PASSED] Actual Title: " + _rbwTest.NavWin.Title);
            }
        }
    }
}
