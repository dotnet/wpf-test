// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


//

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class NavigationTests : Application
    {
        internal enum FrameRelativeUri_CurrentTest
        {
            InitialNav,
            Navigated
        }

        #region FrameRelativeUri globals
        FrameRelativeUri_CurrentTest _frameRelativeUriTest = FrameRelativeUri_CurrentTest.InitialNav;
        #endregion

        void FrameRelativeUri_Startup(object sender, StartupEventArgs e)
        {
            _frameTest = new FrameTestClass("FrameRelativeUri");
            _frameTest.FrameTestType = FrameTestClass.FrameType.Frame;
            _frameTest.RegisterNavEventHandlers = true;

            // Expected number of events fired and destination URI.
            NavigationHelper.NumExpectedNavigatingEvents = 1;
            NavigationHelper.NumExpectedNavigatedEvents = 1;
            NavigationHelper.NumExpectedLoadCompletedEvents = 1;

            // NumExpectedNavigationProgressEvents have to be calculated dynamically 
            // Refer Variable definition in NavigationHelper.cs
            NavigationHelper.NumExpectedNavigationProgressEvents = 1;
            NavigationHelper.ExpectedFileName = IMAGEPAGE;
        }

        void FrameRelativeUri_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (_frameTest.IsFirstRun &&
                _frameRelativeUriTest == FrameRelativeUri_CurrentTest.InitialNav)
            {
                _frameTest.SetupTest();

                if (_frameTest.StdFrame == null)
                    _frameTest.Fail("Could not locate Frame to run FrameRelativeUri test case on");
                _frameTest.StdFrame.ContentRendered += new EventHandler(FrameRelativeUri_FrameContentRendered);

                _frameTest.StdFrame.Navigate(new Uri(IMAGEPAGE, UriKind.RelativeOrAbsolute));
                _frameTest.IsFirstRun = false;
                _frameRelativeUriTest = FrameRelativeUri_CurrentTest.Navigated;
            }
        }

        void FrameRelativeUri_FrameContentRendered(object sender, EventArgs e)
        {
            Frame fSource = sender as Frame;
            FrameworkElement fe = null;

            if (fSource != null &&
                _frameRelativeUriTest == FrameRelativeUri_CurrentTest.Navigated)
            {
                _frameTest.Output("Frame's contents have been rendered");

                fe = fSource.Content as FrameworkElement;
                if (fe == null)
                {
                    NavigationHelper.Fail("FrameworkElement for Frame.Content is null");
                    return;
                }

                Image relImg1 = LogicalTreeHelper.FindLogicalNode(fe, "testImage1") as Image;
                Image relImg2 = LogicalTreeHelper.FindLogicalNode(fe, "testImage2") as Image;
                Image relImg3 = LogicalTreeHelper.FindLogicalNode(fe, "testImage3") as Image;

                if (relImg1 == null || relImg2 == null || relImg3 == null)
                {
                    NavigationHelper.Fail("At least one relImage is null");
                    return;
                }

                _frameTest.Output("Found relImages");
                if (!relImg1.IsInitialized || !relImg2.IsInitialized || !relImg3.IsInitialized)
                {
                    NavigationHelper.Fail("At least one of the relImages is not loaded");
                    return;
                }

                _frameTest.Output("relImages are loaded");
                NavigationHelper.FinishTest(
                            NavigationHelper.CompareFileNames(NavigationHelper.ExpectedFileName, _frameTest.StdFrame.Source) &&
                            _frameTest.StdFrame.Content != null);
            }
        }

    }
}
