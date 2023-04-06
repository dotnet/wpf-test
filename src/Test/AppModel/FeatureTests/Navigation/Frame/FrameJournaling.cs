// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: 
//

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Test.Logging;           // TestLog, TestStage

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public class FrameJournaling
    {
        private enum CurrentTest
        {
            InitialNav,
            FirstNav,
            SecondNav,
            ThirdNav,
            WentBack,
            WentForward,
            WentBack2,
            FourthNav,
            WentBack3,
            WentBack4,
            WentForward2,
            EndNav
        }

        private CurrentTest _frameJournalingTest = CurrentTest.InitialNav;
        private const String anchoredPage = "AnchoredPage_Loose.xaml";
        private const String imagePage = "ImagePage_Loose.xaml";
        private const String framePage = "FramePage_Loose.xaml";
        private const String hlinkPage = "HyperlinkPage_Loose.xaml";
        private const String controlsPage = "ContentControls_Page.xaml";
        private FrameTestClass _frameTest = null;

        public void Startup(object sender, StartupEventArgs e)
        {
            _frameTest = new FrameTestClass("JournalFrame");
            _frameTest.FrameTestType = FrameTestClass.FrameType.Frame;
            _frameTest.RegisterNavEventHandlers = false;

            NavigationHelper.ExpectedFileName = imagePage;

            // Set the expected navigation counts
            int navStates = (int) CurrentTest.EndNav + 1; // number of navigation states
            NavigationHelper.NumExpectedNavigatingEvents = navStates - 1; // ignore InitialNav
            NavigationHelper.NumExpectedNavigatedEvents = navStates - 1; // ignore InitialNav
            // load completed getting fired twice for each state and once on InitialNav
            NavigationHelper.NumExpectedLoadCompletedEvents = 2 * (navStates - 1) + 1;
        }

        private void Navigating(object source, NavigatingCancelEventArgs e)
        {
            NavigationHelper.Output("Navigating event fired");
            NavigationHelper.Output("-----State = " + _frameJournalingTest.ToString());
            NavigationHelper.NumActualNavigatingEvents++;
            if (e.NavigationMode == NavigationMode.Back)
            {
                NavigationHelper.Output("GoingBack NavigationMode set");
            }
            else if (e.NavigationMode == NavigationMode.Forward)
            {
                NavigationHelper.Output("GoingForward NavigationMode set");
            }
        }

        private void Navigated(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("Navigated event fired");
            NavigationHelper.Output("-----State = " + _frameJournalingTest.ToString());
            NavigationHelper.NumActualNavigatedEvents++;
        }

        private void FrameLoadCompleted(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("LoadCompleted event fired ON FRAME");
            NavigationHelper.Output("-----State = " + _frameJournalingTest.ToString());
            NavigationHelper.NumActualLoadCompletedEvents++;
        }

        public void LoadCompleted(object source, NavigationEventArgs e)
        {
            Frame f = null;
            NavigationHelper.Output("LoadCompleted event fired ON APP");
            NavigationHelper.Output("-----State = " + _frameJournalingTest.ToString());
            NavigationHelper.NumActualLoadCompletedEvents++;

            NavigationHelper.Output("uri is: " + e.Uri.ToString());
            NavigationHelper.Output("uri is: " + NavigationHelper.getFileName(e.Uri));

            NavigationWindow nw = e.Navigator as NavigationWindow;
            if (nw != null && _frameTest.IsFirstRun && _frameJournalingTest == CurrentTest.InitialNav)
            {
                _frameTest.SetupTest();

                if (_frameTest.StdFrame == null)
                {
                    _frameTest.Fail("Could not locate Frame to run FrameJournaling test case on");
                }

                _frameTest.StdFrame.Source = new Uri(controlsPage, UriKind.RelativeOrAbsolute);
                _frameJournalingTest = CurrentTest.FirstNav;
                _frameTest.IsFirstRun = false;

                f = _frameTest.StdFrame;
                if (f != null)
                {
                    // attach events to frame
                    NavigationHelper.Output("Registering Frame-level eventhandlers");
                    f.Navigating += new NavigatingCancelEventHandler(Navigating);
                    f.Navigated += new NavigatedEventHandler(Navigated);
                    f.LoadCompleted += new LoadCompletedEventHandler(FrameLoadCompleted);

                    NavigationHelper.Output("Calling Navigate on " + controlsPage);
                    f.Navigate(new Uri(controlsPage, UriKind.RelativeOrAbsolute));
                }
                else
                {
                    NavigationHelper.Fail("Could not get frame element");
                }
            }
            else
            {
                NavigationService ns = null;
                f = e.Navigator as Frame;
                if (f != null)
                {
                    switch (_frameJournalingTest)
                    {
                        case CurrentTest.FirstNav:
                            _frameJournalingTest = CurrentTest.SecondNav;
                            NavigationHelper.Output("Navigating frame to " + anchoredPage);
                            f.Navigate(new Uri(anchoredPage, UriKind.RelativeOrAbsolute));
                            break;

                        case CurrentTest.SecondNav:
                            _frameJournalingTest = CurrentTest.ThirdNav;
                            NavigationHelper.Output("Navigating frame to " + hlinkPage);
                            f.Navigate(new Uri(hlinkPage, UriKind.RelativeOrAbsolute));
                            break;

                        case CurrentTest.ThirdNav:
                            _frameJournalingTest = CurrentTest.WentBack;
                            ns = NavigationService.GetNavigationService(f);
                            if (ns != null)
                            {
                                ns.GoBack();
                            }
                            else
                            {
                                NavigationHelper.Fail("ERROR: NavigationService.GetNavigationService(Frame) returned null");
                            }
                            break;

                        case CurrentTest.WentBack:
                            _frameJournalingTest = CurrentTest.WentForward;
                            ns = NavigationService.GetNavigationService(f);
                            if (ns != null)
                            {
                                ns.GoForward();
                            }
                            else
                            {
                                NavigationHelper.Fail("ERROR: NavigationService.GetNavigationService(Frame) returned null");
                            }
                            break;

                        case CurrentTest.WentForward:
                            _frameJournalingTest = CurrentTest.WentBack2;
                            ns = NavigationService.GetNavigationService(f);
                            if (ns != null)
                            {
                                ns.GoBack();
                            }
                            else
                            {
                                NavigationHelper.Fail("ERROR: NavigationService.GetNavigationService(Frame) returned null");
                            }
                            break;

                        case CurrentTest.WentBack2:
                            _frameJournalingTest = CurrentTest.FourthNav;
                            NavigationHelper.Output("Navigating frame to " + framePage);
                            f.Navigate(new Uri(framePage, UriKind.RelativeOrAbsolute));
                            break;

                        case CurrentTest.FourthNav:
                            _frameJournalingTest = CurrentTest.WentBack3;
                            ns = NavigationService.GetNavigationService(f);
                            if (ns != null)
                            {
                                ns.GoBack();
                            }
                            else
                            {
                                NavigationHelper.Fail("ERROR: NavigationService.GetNavigationService(Frame) returned null");
                            }
                            break;

                        case CurrentTest.WentBack3:
                            _frameJournalingTest = CurrentTest.WentBack4;
                            ns = NavigationService.GetNavigationService(f);
                            if (ns != null)
                            {
                                NavigationHelper.Output("Calling GoForward on NavigationService");
                                ns.GoBack();
                            }
                            else
                            {
                                NavigationHelper.Fail("ERROR: NavigationService.GetNavigationService(Frame)returned null");
                            }
                            break;

                        case CurrentTest.WentBack4:
                            _frameJournalingTest = CurrentTest.WentForward2;
                            ns = NavigationService.GetNavigationService(f);
                            if (ns != null)
                            {
                                NavigationHelper.Output("Calling GoForward on NavigationService");
                                ns.GoForward();
                            }
                            else
                            {
                                NavigationHelper.Fail("ERROR: NavigationService.GetNavigationService(Frame)returned null");
                            }
                            break;

                        case CurrentTest.WentForward2:
                            _frameJournalingTest = CurrentTest.EndNav;
                            NavigationHelper.Output("Navigating Frame to page5.xaml");
                            f.Navigate(new Uri(imagePage, UriKind.RelativeOrAbsolute));
                            break;

                        case CurrentTest.EndNav:
                            NavigationHelper.FinishTest((NavigationHelper.CompareFileNames(NavigationHelper.ExpectedFileName, e.Uri)) && (e.Content != null));
                            break;
                    }
                }
                else
                {
                    NavigationHelper.Fail("Could not get frame");
                }
            }
        }
    }
}

