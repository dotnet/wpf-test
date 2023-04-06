// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // MultipleFrames

    /// <summary>
    /// Hyperlink targeting - multiple frames, navigationWindow; and JournalEntry grouping
    /// of multiple frames (all hyperlinks in frame1 targetting self, frame2 and navWin)
    /// 1) Navigate to Page5.xaml in frame2 
    /// 2) Navigate to Page6.xaml in frame2 
    /// 3) Navigate to pack://siteoforigin:,,,/test.html in frame2
    /// 4) Navigate Self to Page4.xaml
    /// 5) Go back
    /// 6) Navigate window to Page3.xaml        
    /// 7) Go back
    /// </summary>
    /// <remarks>
    /// 


    public partial class NavigationTests : Application
    {
        internal enum MultipleFrames_CurrentTest
        {
            Init,
            hlink_Page5_frame2,
            hlink_Page6_frame2,
            hlink_html_frame2,
            hlink_Page4_frame1,
            back_frame1,
            hlink_Page3_win,
            back_win,
            End
        }

        #region MultipleFrames globals

        private NavigationWindow _multipleFrames_navWin = null;
        private MultipleFrames_CurrentTest _multipleFramesTest = MultipleFrames_CurrentTest.Init;
        private String _multipleFramesCurrHlinkId = String.Empty;
        private bool _multipleFramesInVerifyMode = true;

        private NavigationStateCollection _multipleFramesActualStates = new NavigationStateCollection();
        private NavigationStateCollection _multipleFramesExpectedStates = new NavigationStateCollection();
        private JournalHelper _multipleFramesJHelper = null;

        // expected values for NavigationState
        private String[] _multipleFrames_StateDescription = null;
        private String[] _multipleFrames_WindowTitle = null;
        private bool[] _multipleFrames_BackButtonEnabled = null;
        private bool[] _multipleFrames_ForwardButtonEnabled = null;
        private String[][] _multipleFrames_BackStack = null;
        private String[][] _multipleFrames_ForwardStack = null;

        private const String MULTIPLEFRAMESRESULTS = @"MultipleFramesResults_Loose.xml";
        #endregion


        void MultipleFrames_Startup(object sender, StartupEventArgs e)
        {
            _frameTest = new FrameTestClass("MultipleFrames");
            _frameTest.FrameTestType = FrameTestClass.FrameType.Frame;
            _frameTest.RegisterNavEventHandlers = false;

            /*
             * This code no longer works because cscomp.dll is not available on test servers
             * You may use it when cscomp.dll is availabel on test servers
            if (_multipleFramesInVerifyMode)
            {
                _multipleFramesExpectedStates = NavigationStateCollection.GetResults(
                    Application.GetRemoteStream(new Uri(SITEOFORIGINPREFIX + MULTIPLEFRAMESRESULTS, UriKind.RelativeOrAbsolute)).Stream);
            }
             */ 

            if (_multipleFramesInVerifyMode)
            {
                MultipleFrames_CreateExpectedNavigationStates();

                // populate the expected navigation states
                for (int index = 0; index < _multipleFrames_StateDescription.Length; index++)
                {
                    _multipleFramesExpectedStates.states.Add(new NavigationState(_multipleFrames_StateDescription[index],
                        _multipleFrames_WindowTitle[index],
                        _multipleFrames_BackButtonEnabled[index],
                        _multipleFrames_ForwardButtonEnabled[index],
                        _multipleFrames_BackStack[index],
                        _multipleFrames_ForwardStack[index]));
                }
            }
        }

        void MultipleFrames_Navigated(object sender, NavigationEventArgs e)
        {
            // Differentiate if the Navigated event fired was from NavWin or Frame
            String source = e.Uri.ToString();
            String navigator = e.Navigator.GetType().ToString();

            if (_multipleFramesJHelper != null)
                _multipleFramesActualStates.RecordNewResult(_multipleFramesJHelper, "App Navigated Navigator = " + navigator + ": " + source + " uri = " + source);

            switch (_multipleFramesTest)
            {
                case MultipleFrames_CurrentTest.Init:
                    if ((e.Navigator is NavigationWindow) && (_frameTest.IsFirstRun))
                    {
                        _frameTest.SetupTest();

                        if (_frameTest.StdFrame == null || _frameTest.IslandFrame == null)
                            _frameTest.Fail("Could not locate Frame/IslandFrame to run MultipleFrames test case on");

                        _multipleFrames_navWin = Application.Current.MainWindow as NavigationWindow;
                        _multipleFrames_navWin.SetValue(NavigationWindow.NameProperty, "MultipleFrames_navWin");
                        _multipleFramesJHelper = new JournalHelper(_multipleFrames_navWin);

                        _frameTest.StdFrame.Source = new Uri("HyperlinkPage_MultipleFrame_Loose.xaml", UriKind.RelativeOrAbsolute);
                        _frameTest.IsFirstRun = false;
                    }
                    break;
                case MultipleFrames_CurrentTest.hlink_Page5_frame2:
                    _multipleFramesTest = MultipleFrames_CurrentTest.hlink_Page6_frame2;
                    BrowserHelper.NavigateHyperlinkViaEvent("Page6", _frameTest.StdFrame.Content as DependencyObject);
                    break;

                case MultipleFrames_CurrentTest.hlink_Page6_frame2:
                    _multipleFramesTest = MultipleFrames_CurrentTest.hlink_html_frame2;
                    BrowserHelper.NavigateHyperlinkViaEvent("WebPage1", _frameTest.StdFrame.Content as DependencyObject);
                    break;

                case MultipleFrames_CurrentTest.hlink_html_frame2:
                    _multipleFramesTest = MultipleFrames_CurrentTest.hlink_Page4_frame1;
                    // DD bugs 157786 - Targeting does not work in standalone.  Workaround: Navigate window directly here.
                    // BrowserHelper.NavigateHyperlinkViaEvent("Page4Self", _frameTest.StdFrame.Content as DependencyObject);
                    ((NavigationWindow)Application.Current.MainWindow).Navigate(new Uri("Page4.xaml", UriKind.Relative));
                    break;
            }
        }

        void MultipleFrames_ContentRendered(object sender, EventArgs e)
        {
            // Differentiate if the ContentRendered event fired was from NavWin or Frame
            String navigator = String.Empty;
            String source = String.Empty;
            if (sender is NavigationWindow)
            {
                navigator = "NavigationWindow";
                source = ((NavigationWindow)sender).CurrentSource.ToString();
            }
            else if (sender is Frame)
            {
                Frame nFrame = sender as Frame;
                navigator = "Frame" + (String)nFrame.GetValue(Frame.NameProperty);
                source = nFrame.Source.ToString();
            }

            if (_multipleFramesJHelper != null)
                _multipleFramesActualStates.RecordNewResult(_multipleFramesJHelper, navigator + "_ContentRendered Source = " + source);

            switch (_multipleFramesTest)
            {
                #region navigate NavigationWindow

                case MultipleFrames_CurrentTest.hlink_Page3_win:
                    _multipleFramesTest = MultipleFrames_CurrentTest.back_win;
                    _frameTest.GoBack(_multipleFrames_navWin, 1);
                    break;

                case MultipleFrames_CurrentTest.back_win:
                    _multipleFramesTest = MultipleFrames_CurrentTest.End;
                    bool match = NavigationStateCollection.Compare(_multipleFramesActualStates, _multipleFramesExpectedStates);
                    if (match)
                    {
                        _frameTest.Pass("All states matched");
                    }
                    else
                    {
                        _multipleFramesActualStates.WriteResults("Expected_" + MULTIPLEFRAMESRESULTS);
                        _frameTest.Fail("Not all states matched.  Expected Results correctly written to Expected_" + MULTIPLEFRAMESRESULTS);
                    }
                    break;
                #endregion

                #region navigate Frame 2
                case MultipleFrames_CurrentTest.Init:
                    _multipleFramesTest = MultipleFrames_CurrentTest.hlink_Page5_frame2;
                    BrowserHelper.NavigateHyperlinkViaEvent("Page5", _frameTest.StdFrame.Content as DependencyObject);
                    break;
                #endregion

                #region navigate Frame 1
                case MultipleFrames_CurrentTest.hlink_Page4_frame1:
                    _multipleFramesTest = MultipleFrames_CurrentTest.back_frame1;
                    _frameTest.GoBack(_multipleFrames_navWin, 1);
                    break;

                case MultipleFrames_CurrentTest.back_frame1:
                    _multipleFramesTest = MultipleFrames_CurrentTest.hlink_Page3_win;
                    // DD bugs 157786 - Targeting does not work in standalone.  Workaround: Navigate window directly here.
                    // BrowserHelper.NavigateHyperlinkViaEvent("Page3Self", _frameTest.StdFrame.Content as DependencyObject);
                    ((NavigationWindow)Application.Current.MainWindow).Navigate(new Uri("Page3.xaml", UriKind.Relative));
                    break;


                #endregion
            }
        }

        /// <summary>
        /// Create expected navigation states 
        /// </summary>
        void MultipleFrames_CreateExpectedNavigationStates()
        {
            _multipleFrames_StateDescription
                = new String[]{
                        "App Navigated Navigator = System.Windows.Controls.Frame: NavigationTests_Standalone;component/HyperlinkPage_MultipleFrame_Loose.xaml uri = NavigationTests_Standalone;component/HyperlinkPage_MultipleFrame_Loose.xaml", 
                        "NavigationWindow_ContentRendered Source = FrameTestPage.xaml", 
                        "App Navigated Navigator = System.Windows.Controls.Frame: NavigationTests_Standalone;component/Page5.xaml uri = NavigationTests_Standalone;component/Page5.xaml",
                        "App Navigated Navigator = System.Windows.Controls.Frame: NavigationTests_Standalone;component/Page6.xaml uri = NavigationTests_Standalone;component/Page6.xaml",
                        "App Navigated Navigator = System.Windows.Controls.Frame: pack://siteoforigin:,,,/WebPage1_Loose.html uri = pack://siteoforigin:,,,/WebPage1_Loose.html",
                        "App Navigated Navigator = System.Windows.Navigation.NavigationWindow: Page4.xaml uri = Page4.xaml",
                        "NavigationWindow_ContentRendered Source = Page4.xaml",
                        "App Navigated Navigator = System.Windows.Controls.Frame: NavigationTests_Standalone;component/HyperlinkPage_MultipleFrame_Loose.xaml uri = NavigationTests_Standalone;component/HyperlinkPage_MultipleFrame_Loose.xaml",
                        "App Navigated Navigator = System.Windows.Controls.Frame: pack://siteoforigin:,,,/WebPage1_Loose.html uri = pack://siteoforigin:,,,/WebPage1_Loose.html",
                        "App Navigated Navigator = System.Windows.Navigation.NavigationWindow: FrameTestPage.xaml uri = FrameTestPage.xaml",
                        "NavigationWindow_ContentRendered Source = FrameTestPage.xaml",
                        "App Navigated Navigator = System.Windows.Navigation.NavigationWindow: Page3.xaml uri = Page3.xaml",
                        "NavigationWindow_ContentRendered Source = Page3.xaml",
                        "App Navigated Navigator = System.Windows.Controls.Frame: NavigationTests_Standalone;component/HyperlinkPage_MultipleFrame_Loose.xaml uri = NavigationTests_Standalone;component/HyperlinkPage_MultipleFrame_Loose.xaml",
                        "App Navigated Navigator = System.Windows.Controls.Frame: pack://siteoforigin:,,,/WebPage1_Loose.html uri = pack://siteoforigin:,,,/WebPage1_Loose.html",
                        "App Navigated Navigator = System.Windows.Navigation.NavigationWindow: FrameTestPage.xaml uri = FrameTestPage.xaml",
                        "NavigationWindow_ContentRendered Source = FrameTestPage.xaml"
                };

            _multipleFrames_WindowTitle
                = new String[]{
                        "RedirectMitigation", 
                        "RedirectMitigation", 
                        "RedirectMitigation", 
                        "RedirectMitigation", 
                        "RedirectMitigation", 
                        "RedirectMitigation", 
                        "RedirectMitigation", 
                        "RedirectMitigation", 
                        "RedirectMitigation", 
                        "RedirectMitigation", 
                        "RedirectMitigation", 
                        "RedirectMitigation", 
                        "RedirectMitigation", 
                        "RedirectMitigation", 
                        "RedirectMitigation", 
                        "RedirectMitigation", 
                        "RedirectMitigation" 
                };

            _multipleFrames_BackButtonEnabled
                = new bool[]{
                        false, 
                        false, 
                        false, 
                        false, 
                        false, 
                        true, 
                        true, 
                        false, 
                        false, 
                        false, 
                        false, 
                        true, 
                        true, 
                        false, 
                        false, 
                        false, 
                        false 
                };

            _multipleFrames_ForwardButtonEnabled
                = new bool[]{
                        false, 
                        false, 
                        false, 
                        false, 
                        false, 
                        false, 
                        false, 
                        true, 
                        true, 
                        true, 
                        true, 
                        false, 
                        false, 
                        true, 
                        true, 
                        true, 
                        true 
                };

            _multipleFrames_BackStack = new String[17][];
            _multipleFrames_BackStack[0] = new String[0];
            _multipleFrames_BackStack[1] = new String[0];
            _multipleFrames_BackStack[2] = new String[0];
            _multipleFrames_BackStack[3] = new String[0];
            _multipleFrames_BackStack[4] = new String[0];
            _multipleFrames_BackStack[5] = new String[] { "RedirectMitigation (FrameTestPage.xaml)" };
            _multipleFrames_BackStack[6] = new String[] { "RedirectMitigation (FrameTestPage.xaml)" };
            _multipleFrames_BackStack[7] = new String[0];
            _multipleFrames_BackStack[8] = new String[0];
            _multipleFrames_BackStack[9] = new String[0];
            _multipleFrames_BackStack[10] = new String[0];
            _multipleFrames_BackStack[11] = new String[] { "RedirectMitigation (FrameTestPage.xaml)" };
            _multipleFrames_BackStack[12] = new String[] { "RedirectMitigation (FrameTestPage.xaml)" };
            _multipleFrames_BackStack[13] = new String[0];
            _multipleFrames_BackStack[14] = new String[0];
            _multipleFrames_BackStack[15] = new String[0];
            _multipleFrames_BackStack[16] = new String[0];

            _multipleFrames_ForwardStack = new String[17][];
            _multipleFrames_ForwardStack[0] = new String[0];
            _multipleFrames_ForwardStack[1] = new String[0];
            _multipleFrames_ForwardStack[2] = new String[0];
            _multipleFrames_ForwardStack[3] = new String[0];
            _multipleFrames_ForwardStack[4] = new String[0];
            _multipleFrames_ForwardStack[5] = new String[0];
            _multipleFrames_ForwardStack[6] = new String[0];
            _multipleFrames_ForwardStack[7] = new String[] { "RedirectMitigation (Page4.xaml)" };
            _multipleFrames_ForwardStack[8] = new String[] { "RedirectMitigation (Page4.xaml)" };
            _multipleFrames_ForwardStack[9] = new String[] { "RedirectMitigation (Page4.xaml)" };
            _multipleFrames_ForwardStack[10] = new String[] { "RedirectMitigation (Page4.xaml)" };
            _multipleFrames_ForwardStack[11] = new String[0];
            _multipleFrames_ForwardStack[12] = new String[0];
            _multipleFrames_ForwardStack[13] = new String[] { "RedirectMitigation (Page3.xaml)" };
            _multipleFrames_ForwardStack[14] = new String[] { "RedirectMitigation (Page3.xaml)" };
            _multipleFrames_ForwardStack[15] = new String[] { "RedirectMitigation (Page3.xaml)" };
            _multipleFrames_ForwardStack[16] = new String[] { "RedirectMitigation (Page3.xaml)" };
        }
    }
}
