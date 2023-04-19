// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// frame base class for test cases that uses a page with a single frame 
    /// </summary>
    public class SingleFrameTestClass : NavigationBaseClass
    {        
        #region private members
        protected Frame             frame          = null;
        private const String        frameName = "testFrame";
        private const String        framePage = @"SingleFrameTestPage.xaml";
        #endregion

        #region methods

        public SingleFrameTestClass(String userGivenTestName) : base(userGivenTestName)
        {
            Application.Current.StartupUri = new Uri(framePage, UriKind.RelativeOrAbsolute);

            // Begin the test
            NavigationHelper.SetStage(TestStage.Run);
        }

        // return the frame
        public Frame GetFrame()
        {
                Frame frame = null;
                NavigationWindow navWin = NavigationWindow;

                if (navWin != null)
                {
                    frame = LogicalTreeHelper.FindLogicalNode(navWin.Content as DependencyObject, frameName) as Frame;
                }
                else
                {
                    // the case where NavigationWindow is not available
                    frame = LogicalTreeHelper.FindLogicalNode(Application.Current.MainWindow.Content as DependencyObject, frameName) as Frame;
                }

                return frame;
        }
        #endregion

        #region properties

        public NavigationWindow NavigationWindow
        {
            get
            {
                return Application.Current.MainWindow as NavigationWindow;
            }
        }

        #endregion
    }
}
