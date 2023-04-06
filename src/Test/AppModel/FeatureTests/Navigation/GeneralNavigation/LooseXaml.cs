// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: The LooseXaml BVT suite checks the functionality 
//  of various navigations to loose xaml files.  The following scenarios 
//  are covered:
//  [1] Navigate to uncompiled loose xaml Page1.xaml via absolute Uri
//  [2] Navigate to content loose xaml Page2.xaml via hyperlink in Page1.xaml ("pack://siteoforigin:,,,/Page2.xaml")
//  [3] Navigate to html containing IFrame with loose XAML source (http://wpfapps/testscratch/LooseXaml/banana.html)
//  [4] Navigate to a fragment on a content loose xaml page, and then return to it via GoForward.
//

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Test;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class NavigationTests : Application
    {
        internal enum LooseXaml_CurrentTest
        {
            UnInit,
            NavToLoosePage,
            NavToLoosePageViaHlink,
            NavToEmbeddedLoosePage,
            NavToLoosePageFrag,
            GoBackToLoosePage,
            GoBackToLoosePageFrag,
            End
        }

        internal enum LooseXaml_NavigationContainer
        {
            NavigationWindow,
            Frame,
            IslandFrame
        }

        private NavigationWindow _looseXaml_currNavWin  = null;
        private Frame            _looseXaml_currFrame   = null;
        private Frame            _looseXaml_islandFrame = null;
        private Log              _looseXaml_log = null;

        private LooseXaml_NavigationContainer _looseXaml_testContainer = LooseXaml_NavigationContainer.NavigationWindow;
        private LooseXaml_CurrentTest         _looseXaml_test          = LooseXaml_CurrentTest.UnInit;

        //bool inVerifyMode = true;
        //string resultsFile = "results.xml";

        private void LooseXaml_Startup(object sender, StartupEventArgs e)
        {
            // Initialize TestLog
            _looseXaml_log = Log.Current;
            Log.Current.CurrentVariation.LogMessage("Initializing TestLog...");

            // Begin the test
            this.StartupUri = new Uri("LooseXaml_HomePage.xaml", UriKind.RelativeOrAbsolute);
        }

        private void LooseXaml_LoadCompleted(object sender, NavigationEventArgs e)
        {
            Log.Current.CurrentVariation.LogMessage("e.Navigator is " + e.Navigator.ToString());

            if (_looseXaml_test == LooseXaml_CurrentTest.UnInit)
            {
                if (e.Navigator is NavigationWindow)
                {
                    // Snag Harness args to check which test we're running.
                    String testToRun = DriverState.DriverParameters["LooseXaml_NavigationContainer"].ToLowerInvariant();
                    Log.Current.CurrentVariation.LogMessage("LooseXaml_NavigationContainer: " + testToRun);

                    if (testToRun.Equals("navwin"))
                    {
                        Log.Current.CurrentVariation.LogMessage("Running NavigationWindow LooseXaml test");
                        Log.Current.CurrentVariation.LogMessage("Grabbing reference to the NavigationWindow.");
                        _looseXaml_testContainer = LooseXaml_NavigationContainer.NavigationWindow;
                        _looseXaml_currNavWin = Application.Current.MainWindow as NavigationWindow;
                        _looseXaml_currNavWin.ContentRendered += new EventHandler(OnContentRendered_LooseXaml_Local); 
                    }
                    else if (testToRun.Equals("frame"))
                    {
                        Log.Current.CurrentVariation.LogMessage("Running Frame/IslandFrame LooseXaml test");
                        Log.Current.CurrentVariation.LogMessage("Grabbing reference to Frame/IslandFrame.");
                        _looseXaml_currNavWin = Application.Current.MainWindow as NavigationWindow;
                        _looseXaml_currFrame = LogicalTreeHelper.FindLogicalNode(_looseXaml_currNavWin.Content as DependencyObject, "frame1") as Frame;
                        _looseXaml_currFrame.ContentRendered += new EventHandler(OnContentRendered_LooseXaml_Local);
                        _looseXaml_islandFrame = LogicalTreeHelper.FindLogicalNode(_looseXaml_currNavWin.Content as DependencyObject, "islandframe1") as Frame;
                        _looseXaml_islandFrame.ContentRendered += new EventHandler(OnContentRendered_LooseXaml_Local);

                        // Set starting page for the Frames
                        _looseXaml_testContainer = LooseXaml_NavigationContainer.Frame;
                        _looseXaml_currFrame.Source = new Uri("LooseXaml_FrameHomePage.xaml", UriKind.RelativeOrAbsolute);
                    }
                    else
                        NavigationHelper.ExitWithError("LooseXaml_NavigationContainer " + testToRun + " is not recognized");

                    _looseXaml_test = LooseXaml_CurrentTest.NavToLoosePage;
                }
            }
        }

        private void OnContentRendered_LooseXaml_Local(object sender, EventArgs e)
        {
            switch (_looseXaml_test)
            {
                // Navigate to uncompiled loose xaml Page1.xaml via absolute Uri
                case LooseXaml_CurrentTest.NavToLoosePage:
                    Log.Current.CurrentVariation.LogMessage("[1] Navigate to uncompiled loose xaml via absolute Uri");
                    if (_looseXaml_testContainer == LooseXaml_NavigationContainer.NavigationWindow)
                        _looseXaml_currNavWin.Navigate(new Uri("LooseXaml_Page1.xaml", UriKind.RelativeOrAbsolute));
                    else if (_looseXaml_testContainer == LooseXaml_NavigationContainer.Frame)
                        _looseXaml_currFrame.Navigate(new Uri("LooseXaml_Page1.xaml", UriKind.RelativeOrAbsolute));
                    else if (_looseXaml_testContainer == LooseXaml_NavigationContainer.IslandFrame)
                        _looseXaml_islandFrame.Navigate(new Uri("LooseXaml_Page1.xaml", UriKind.RelativeOrAbsolute));
                    else
                        NavigationHelper.ExitWithError("Not a NavigationWindow/Frame/IslandFrame.  Exiting test.");

                    _looseXaml_test = LooseXaml_CurrentTest.GoBackToLoosePage;
                    break;

                // Navigate back to the loose XAML page via the journal
                case LooseXaml_CurrentTest.GoBackToLoosePage:
                    Log.Current.CurrentVariation.LogMessage("[2] Return to loose XAML page via journal");
                    if (_looseXaml_testContainer == LooseXaml_NavigationContainer.NavigationWindow ||
                        _looseXaml_testContainer == LooseXaml_NavigationContainer.Frame)
                    {
                        bool isAtStart = false;
                        if (_looseXaml_currNavWin != null)
                        {
                            Log.Current.CurrentVariation.LogMessage("NavWin currently at: " + _looseXaml_currNavWin.Source);
                            isAtStart = _looseXaml_currNavWin.Source.ToString().Equals("LooseXaml_HomePage.xaml");
                        }
                        else if (_looseXaml_currFrame != null)
                        {
                            Log.Current.CurrentVariation.LogMessage("Frame currently at: " + _looseXaml_currFrame.Source);
                            isAtStart = _looseXaml_currFrame.Source.ToString().Equals("LooseXaml_FrameHomePage.xaml");
                        }

                        if (isAtStart && _looseXaml_currNavWin.CanGoForward)
                        {
                            Log.Current.CurrentVariation.LogMessage("Calling GoForward to return to loose XAML page");
                            _looseXaml_currNavWin.GoForward();
                            _looseXaml_test = LooseXaml_CurrentTest.NavToLoosePageViaHlink;
                        }
                        else
                        {
                            // GoBack to the very first page
                            Log.Current.CurrentVariation.LogMessage("LooseXaml_currNavWin.CanGoBack: " + _looseXaml_currNavWin.CanGoBack);
                            if (_looseXaml_currNavWin.CanGoBack)
                            {
                                Log.Current.CurrentVariation.LogMessage("Calling GoBack");
                                _looseXaml_currNavWin.GoBack();
                            }
                        }
                    }
                    else if (_looseXaml_testContainer == LooseXaml_NavigationContainer.IslandFrame)
                    {
                        Log.Current.CurrentVariation.LogMessage("IslandFrame currently at: " + _looseXaml_islandFrame.Source);
                        if (_looseXaml_islandFrame.Source.ToString().Equals("LooseXaml_FrameHomePage.xaml") && _looseXaml_islandFrame.CanGoForward)
                        {
                            Log.Current.CurrentVariation.LogMessage("Calling GoForward to return to loose XAML page");
                            _looseXaml_islandFrame.GoForward();
                            _looseXaml_test = LooseXaml_CurrentTest.NavToLoosePageViaHlink;
                        }
                        else
                        {
                            // GoBack to the very first page
                            if (_looseXaml_islandFrame.CanGoBack)
                            {
                                Log.Current.CurrentVariation.LogMessage("Calling GoBack");
                                _looseXaml_islandFrame.GoBack();
                            }
                        } 
                    }
                    break;

                // Navigate to content loose xaml Page2.xaml via hyperlink in Page1.xaml
                case LooseXaml_CurrentTest.NavToLoosePageViaHlink:
                    Log.Current.CurrentVariation.LogMessage("[3] Navigate to content loose xaml via Hyperlink");
                    Hyperlink page2Link = null;
                    if (_looseXaml_testContainer == LooseXaml_NavigationContainer.NavigationWindow)
                        page2Link = LogicalTreeHelper.FindLogicalNode(_looseXaml_currNavWin.Content as DependencyObject, "Page2link") as Hyperlink;
                    else if (_looseXaml_testContainer == LooseXaml_NavigationContainer.Frame)
                        page2Link = LogicalTreeHelper.FindLogicalNode(_looseXaml_currFrame.Content as DependencyObject, "Page2link") as Hyperlink;
                    else if (_looseXaml_testContainer == LooseXaml_NavigationContainer.IslandFrame)
                        page2Link = LogicalTreeHelper.FindLogicalNode(_looseXaml_islandFrame.Content as DependencyObject, "Page2link") as Hyperlink;
                    else
                        NavigationHelper.ExitWithError("Not a NavigationWindow/Frame/IslandFrame.  Exiting test.");

                    if (page2Link == null)
                        NavigationHelper.ExitWithError("Could not find Hyperlink for Page2.xaml.  Exiting test.");
                    else
                        page2Link.DoClick();

                    if (BrowserInteropHelper.IsBrowserHosted)
                        _looseXaml_test = LooseXaml_CurrentTest.NavToLoosePageFrag;
                    else
                        _looseXaml_test = LooseXaml_CurrentTest.NavToEmbeddedLoosePage;
                    break;

                // Navigate to html containing IFrame with loose XAML source (http://wpfapps/testscratch/LooseXaml/banana.html)
                case LooseXaml_CurrentTest.NavToEmbeddedLoosePage:
                    Log.Current.CurrentVariation.LogMessage("[4] Navigate to HTML containing IFrame with loose XAML source");
                    Hyperlink bananaLink = null;
                    if (_looseXaml_testContainer == LooseXaml_NavigationContainer.NavigationWindow)
                        bananaLink = LogicalTreeHelper.FindLogicalNode(_looseXaml_currNavWin.Content as DependencyObject, "BananaHTML") as Hyperlink;
                    else if (_looseXaml_testContainer == LooseXaml_NavigationContainer.Frame)
                        bananaLink = LogicalTreeHelper.FindLogicalNode(_looseXaml_currFrame.Content as DependencyObject, "BananaHTML") as Hyperlink;
                    else if (_looseXaml_testContainer == LooseXaml_NavigationContainer.IslandFrame)
                        bananaLink = LogicalTreeHelper.FindLogicalNode(_looseXaml_islandFrame.Content as DependencyObject, "BananaHTML") as Hyperlink;
                    else
                        NavigationHelper.ExitWithError("Not a NavigationWindow/Frame/IslandFrame.  Exiting test.");

                    if (bananaLink == null)
                        NavigationHelper.ExitWithError("Could not find Hyperlink for http://wpfapps/testscratch/LooseXaml/banana.html.  Exiting test.");
                    else
                        bananaLink.DoClick();
                    _looseXaml_test = LooseXaml_CurrentTest.NavToLoosePageFrag;
                    break;

                case LooseXaml_CurrentTest.NavToLoosePageFrag:
                    Log.Current.CurrentVariation.LogMessage("[5] Navigate to a fragment on a content loose xaml page");
                    // Go back to the very beginning
                    if (_looseXaml_testContainer == LooseXaml_NavigationContainer.NavigationWindow ||
                        _looseXaml_testContainer == LooseXaml_NavigationContainer.Frame)
                    {
                        bool isAtStart = false;
                        if (_looseXaml_currNavWin != null && _looseXaml_testContainer == LooseXaml_NavigationContainer.NavigationWindow)
                        {
                            Log.Current.CurrentVariation.LogMessage("NavWin currently at: " + _looseXaml_currNavWin.Source);
                            isAtStart = _looseXaml_currNavWin.Source.ToString().Equals("LooseXaml_HomePage.xaml");
                        }
                        else if (_looseXaml_currFrame != null && _looseXaml_testContainer == LooseXaml_NavigationContainer.Frame)
                        {
                            Log.Current.CurrentVariation.LogMessage("Frame currently at: " + _looseXaml_currFrame.Source);
                            isAtStart = _looseXaml_currFrame.Source.ToString().Equals("LooseXaml_FrameHomePage.xaml");
                        }

                        if (!isAtStart && _looseXaml_currNavWin.CanGoBack)
                        {
                            Log.Current.CurrentVariation.LogMessage("LooseXaml_currNavWin.CanGoBack: " + _looseXaml_currNavWin.CanGoBack);
                            Log.Current.CurrentVariation.LogMessage("Calling GoBack");
                            _looseXaml_currNavWin.GoBack();
                        }
                        else
                        {
                            // Either we are at the start page OR we can't go back any further
                            if (_looseXaml_testContainer == LooseXaml_NavigationContainer.NavigationWindow)
                                _looseXaml_currNavWin.Navigate(new Uri("pack://siteoforigin:,,,/LooseXaml_Unicorn.xaml#image", UriKind.RelativeOrAbsolute));
                            else if (_looseXaml_testContainer == LooseXaml_NavigationContainer.Frame)
                                _looseXaml_currFrame.Navigate(new Uri("pack://siteoforigin:,,,/LooseXaml_Unicorn.xaml#image", UriKind.RelativeOrAbsolute));

                            _looseXaml_test = LooseXaml_CurrentTest.GoBackToLoosePageFrag;
                        }
                    }
                    else if (_looseXaml_testContainer == LooseXaml_NavigationContainer.IslandFrame)
                    {
                        Log.Current.CurrentVariation.LogMessage("IslandFrame currently at: " + _looseXaml_islandFrame.Source);
                        if (!_looseXaml_islandFrame.Source.ToString().Equals("LooseXaml_FrameHomePage.xaml") && _looseXaml_islandFrame.CanGoBack)
                        {
                            Log.Current.CurrentVariation.LogMessage("LooseXaml_islandFrame.CanGoBack: " + _looseXaml_islandFrame.CanGoBack);
                            Log.Current.CurrentVariation.LogMessage("Calling GoBack");
                            _looseXaml_islandFrame.GoBack();
                        }
                        else
                        {
                            // Either we are at the start page OR we can't go back any further
                            _looseXaml_islandFrame.Navigate(new Uri("pack://siteoforigin:,,,/LooseXaml_Unicorn.xaml#image", UriKind.RelativeOrAbsolute));
                            _looseXaml_test = LooseXaml_CurrentTest.GoBackToLoosePageFrag;
                        }
                    } 
                    else
                        NavigationHelper.ExitWithError("Not a NavigationWindow/Frame/IslandFrame.  Exiting test."); 
                    break;

                case LooseXaml_CurrentTest.GoBackToLoosePageFrag:
                    Log.Current.CurrentVariation.LogMessage("[6] Return to fragment on loose XAML page via GoForward");
                    if (_looseXaml_testContainer == LooseXaml_NavigationContainer.NavigationWindow ||
                        _looseXaml_testContainer == LooseXaml_NavigationContainer.Frame)
                    {
                        bool isAtStart = false;
                        if (_looseXaml_currNavWin != null && _looseXaml_testContainer == LooseXaml_NavigationContainer.NavigationWindow)
                        {
                            Log.Current.CurrentVariation.LogMessage("NavWin currently at: " + _looseXaml_currNavWin.Source);
                            isAtStart = _looseXaml_currNavWin.Source.ToString().Equals("LooseXaml_HomePage.xaml");
                        }
                        else if (_looseXaml_currFrame != null && _looseXaml_testContainer == LooseXaml_NavigationContainer.Frame)
                        {
                            Log.Current.CurrentVariation.LogMessage("Frame currently at: " + _looseXaml_currFrame.Source);
                            isAtStart = _looseXaml_currFrame.Source.ToString().Equals("LooseXaml_FrameHomePage.xaml");
                        }

                        if (!isAtStart && _looseXaml_currNavWin.CanGoBack)
                        {
                            Log.Current.CurrentVariation.LogMessage("LooseXaml_currNavWin.CanGoBack: " + _looseXaml_currNavWin.CanGoBack);
                            Log.Current.CurrentVariation.LogMessage("Calling GoBack");
                            _looseXaml_currNavWin.GoBack();
                        }
                        else
                        {
                            Log.Current.CurrentVariation.LogMessage("Calling GoForward to return to fragment in loose XAML page");
                            _looseXaml_currNavWin.GoForward();
                            _looseXaml_test = LooseXaml_CurrentTest.End;
                        }
                    }
                    else if (_looseXaml_testContainer == LooseXaml_NavigationContainer.IslandFrame)
                    {
                        if (!_looseXaml_islandFrame.Source.ToString().Equals("LooseXaml_FrameHomePage.xaml") && _looseXaml_islandFrame.CanGoBack)
                        {
                            Log.Current.CurrentVariation.LogMessage("LooseXaml_islandFrame.CanGoBack: " + _looseXaml_islandFrame.CanGoBack);
                            Log.Current.CurrentVariation.LogMessage("Calling GoBack");
                            _looseXaml_islandFrame.GoBack();
                        }
                        else
                        {
                            Log.Current.CurrentVariation.LogMessage("Calling GoForward to return to fragment in loose XAML page");
                            _looseXaml_islandFrame.GoForward();
                            _looseXaml_test = LooseXaml_CurrentTest.End;
                        }
                    }
                    else
                        NavigationHelper.ExitWithError("Not a NavigationWindow/Frame/IslandFrame.  Exiting test.");
                    break;

                case LooseXaml_CurrentTest.End:
                    Log.Current.CurrentVariation.LogMessage("[7] End of test.");
                    if (_looseXaml_testContainer == LooseXaml_NavigationContainer.NavigationWindow)
                    {
                        // If we got here without failing, then pass the test
                        if (_looseXaml_currNavWin.CanGoBack && !_looseXaml_currNavWin.CanGoForward)
                            NavigationHelper.PassTest("All sub-tests passed");
                        else
                            NavigationHelper.ExitWithError("Incorrect ending state.  Test fails.");
                    }
                    else if (_looseXaml_testContainer == LooseXaml_NavigationContainer.Frame)
                    {
                        Log.Current.CurrentVariation.LogMessage("Checking Frame");
                        if (_looseXaml_currNavWin.CanGoBack && !_looseXaml_currNavWin.CanGoForward)
                        {
                            _looseXaml_testContainer = LooseXaml_NavigationContainer.IslandFrame;
                            _looseXaml_islandFrame.Source = new Uri("LooseXaml_FrameHomePage.xaml", UriKind.RelativeOrAbsolute);
                            _looseXaml_test = LooseXaml_CurrentTest.NavToLoosePage;
                        }
                        else
                            NavigationHelper.ExitWithError("Incorrect ending state. Test fails.");
                    }
                    else if (_looseXaml_testContainer == LooseXaml_NavigationContainer.IslandFrame)
                    {
                        Log.Current.CurrentVariation.LogMessage("Checking IslandFrame");
                        if (_looseXaml_islandFrame.CanGoBack && !_looseXaml_islandFrame.CanGoForward)
                            NavigationHelper.PassTest("All sub-tests passed");
                        else
                            NavigationHelper.ExitWithError("Incorrect ending state.  Test fails.");
                    }
                    else
                        NavigationHelper.ExitWithError("Not a NavigationWindow/Frame/IslandFrame.  Exiting test.");
                    break;

                default:
                    NavigationHelper.ExitWithError("Test " + _looseXaml_test + " is not recognized.  Failing LooseXaml test for " + _looseXaml_testContainer);
                    break;
            }
        }


    }
}

