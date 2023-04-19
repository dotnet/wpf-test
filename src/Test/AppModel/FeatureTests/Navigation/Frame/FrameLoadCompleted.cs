// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description:  This test deals with navigating a Frame using button
//                click handling, in standalone-deployed application.
//

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class NavigationTests : Application
    {
        internal enum FrameLoadCompleted_CurrentTest
        {
            InitialNav,
            Navigated
        }

        #region FrameLoadCompleted globals
        private FrameLoadCompleted_CurrentTest _frameLoadCompletedTest = FrameLoadCompleted_CurrentTest.InitialNav;
        #endregion

        void FrameLoadCompleted_Startup(object sender, StartupEventArgs e)
        {
            _frameTest = new FrameTestClass("FrameLoadCompleted");
            _frameTest.FrameTestType = FrameTestClass.FrameType.Frame;
            _frameTest.RegisterNavEventHandlers = true;

            // Expected number of events fired and destination URI.
            // We receive 1 Navigating/Navigated/LoadCompleted event from 
            // initially going to CONTROLSPAGE, and another from going to FLOWDOCPAGE
            NavigationHelper.NumExpectedNavigatingEvents = 2;
            NavigationHelper.NumExpectedNavigatedEvents = 2;
            NavigationHelper.NumExpectedLoadCompletedEvents = 2;

            /// NumExpectedNavigationProgressEvents have to be calculated dynamically 
            /// Refer Variable definition in NavigationHelper.cs
            NavigationHelper.NumExpectedNavigationProgressEvents = 4;
            NavigationHelper.ExpectedFileName = FLOWDOCPAGE;
        }

        void FrameLoadCompleted_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (_frameTest.IsFirstRun &&
                _frameLoadCompletedTest == FrameLoadCompleted_CurrentTest.InitialNav)
            {
                _frameTest.SetupTest();

                if (_frameTest.StdFrame == null)
                    _frameTest.Fail("Could not locate Frame to run FrameLoadCompleted test case on");

                _frameTest.StdFrame.Source = new Uri(CONTROLSPAGE, UriKind.RelativeOrAbsolute);
                _frameTest.StdFrame.ContentRendered += new EventHandler(FrameLoadCompleted_ContentRendered);
                _frameTest.IsFirstRun = false;
            }
        }

        void FrameLoadCompleted_ContentRendered(object sender, EventArgs e)
        {
            if (_frameLoadCompletedTest == FrameLoadCompleted_CurrentTest.InitialNav &&
                _frameTest.VerifySource(_frameTest.StdFrame, _RESOURCEPREFIX + CONTROLSPAGE))
            {
                _frameLoadCompletedTest = FrameLoadCompleted_CurrentTest.Navigated;

                // Get button and navigate the Frame
                _frameTest.Output("Grabbing reference to the NavigationWindow's content");
                FrameworkElement fe = _frameTest.StdFrame.Content as FrameworkElement;
                if (fe == null)
                    _frameTest.Fail("Could not get NavigationWindow.Content as FrameworkElement.  fe == null");

                _frameTest.Output("Programmatically clicking on the Navigate button");
                BrowserHelper.InvokeButton(BUTTONNAME, fe);
            }
            else if (_frameLoadCompletedTest == FrameLoadCompleted_CurrentTest.Navigated &&
                _frameTest.VerifySource(_frameTest.StdFrame, FLOWDOCPAGE))
            {
                NavigationHelper.FinishTest(_frameTest.StdFrame.Source != null &&
                    NavigationHelper.CompareFileNames(NavigationHelper.ExpectedFileName, _frameTest.StdFrame.Source));
            }
            else
            {
                _frameTest.Fail("_frameLoadCompletedTest is unknown:  " + _frameLoadCompletedTest.ToString());
            }
        }

    }
}
