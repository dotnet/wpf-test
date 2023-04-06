// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Tests fragment navigation on multiple frames; 
//  journaling of control state (checkbox/textbox) and exercises 
//  ShowsNavigationUI property on Page.
// 
//  The following is the sequence of events for this test.
//  1) Navigate to Page2.xaml#fragment2 on frame1 via Hyperlink
//  2) Navigate to pack://siteoforigin:,,,/Page3.xaml#fragment6 on frame2
//  3) While on Page3.xaml, navigate frame2 to #fragment4 using standard URIs (via Source property)
//  4) While on Page3.xaml, navigate frame2 to #fragment5 using standard URIs (via Navigate method)
//  5) While on Page2.xaml, navigate frame1 to #fragment3 via Source property
//  6) While on Page2.xaml, navigate frame1 to #fragment1 via Navigate method
//  7) Navigate to pack://siteoforigin:,,,/Page4.xaml#fragment5 on frame2
//  8) Set TextBox content to "This is some text"
//  9) Navigate to #frame3 containing image
//  10) Check the checkbox
//  11) Go back all the way
//  12) Go forward all the way
//  13) Navigate away from page to Page4.xaml
//  14) Page4.xaml sets ShowsNavigationUI to false - will add a verification step here (journal entries should be grouped)
//  15) Go back to Page1.xaml
//  16) Navigate to #frame1 - but handle the fragment navigation so that the scroll position does not change
//  Look at the results.xml file for recorded navigation state at different steps
//  This will be used for verification when actual test is run
//


using System;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Threading;
using Microsoft.Test.Logging;


namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class NavigationTests : Application
    {
        enum FragmentNavigation_State
        {
            Init,
            fragment2,
            fragment6,
            stduri_fragment4,
            stduri_fragment5,
            fragment3,
            fragment1,
            fragment5,
            textbox,
            hlinkFrame3,
            checkbox,
            GoBackAll,
            GoForwardAll,
            hlink_page4,
            GoBack,
            AttachScrollChangedHandler,
            HandleFragmentNavigation,
            End
        }

        private NavigationWindow _fragmentNavigation_navWin = null;
        private TextBox _fragmentNavigation_textBox = null;
        private CheckBox _fragmentNavigation_checkBox = null;
        private Frame _fragmentNavigation_frame1 = null;
        private Frame _fragmentNavigation_frame2 = null;
        private ScrollViewer _fragmentNavigation_frameScrollViewer = null;

        private FragmentNavigation_State _fragmentNavigation_currentState = FragmentNavigation_State.Init;
        private bool _fragmentNavigation_inVerifyMode = true;

        private JournalHelper _fragmentNavigation_helper = null;
        private NavigationStateCollection _fragmentNavigation_actualStates = new NavigationStateCollection();
        private NavigationStateCollection _fragmentNavigation_expectedStates = new NavigationStateCollection();

        private string _fragmentNavigation_activationUriString = String.Empty;
        private int _fragmentNavigation_currentStateIndex = 0;

        // expected values for NavigationState
        private String[] _fragmentNavigation_StateDescription = null;
        private String[] _fragmentNavigation_WindowTitle = null;
        private bool[] _fragmentNavigation_BackButtonEnabled = null;
        private bool[] _fragmentNavigation_ForwardButtonEnabled = null;
        private String[][] _fragmentNavigation_BackStack = null;
        private String[][] _fragmentNavigation_ForwardStack = null;

        /// <summary>
        /// FragmentNavigation test entry point
        /// </summary>
        /// <param name="sender">Object that raised startup event (not used)</param>
        /// <param name="e">Event arguments (not used)</param>
        private void FragmentNavigation_Startup(object sender, StartupEventArgs e)
        {
            _navigateUri_currentState = NavigateUri_State.Init;
            NavigationHelper.SetStage(TestStage.Initialize);

            /*
             * This code no longer works because of the error "cscomp.dll is missing" 
             * You may use it when this issue is fixed
            if (FragmentNavigation_inVerifyMode)
            {
                NavigationHelper.Output("Reading in expected results from XML file [FragmentNavigationResults_Loose.xml]");
                Stream s = Application.GetRemoteStream(new Uri("pack://siteoforigin:,,,/FragmentNavigationResults_Loose.xml", UriKind.RelativeOrAbsolute)).Stream;
                FragmentNavigation_expectedStates = NavigationStateCollection.GetResults(s);
            }
             */

            if (_fragmentNavigation_inVerifyMode)
            {
                FragmentNavigation_CreateExpectedNavigationStates();

                // populate the expected navigation states
                for (int index = 0; index < _fragmentNavigation_StateDescription.Length; index++)
                {
                    _fragmentNavigation_expectedStates.states.Add(new NavigationState(_fragmentNavigation_StateDescription[index],
                        _fragmentNavigation_WindowTitle[index],
                        _fragmentNavigation_BackButtonEnabled[index],
                        _fragmentNavigation_ForwardButtonEnabled[index],
                        _fragmentNavigation_BackStack[index],
                        _fragmentNavigation_ForwardStack[index]));
                }
            }

            // Build proper URI activation path
            Uri activationUri;
            if (BrowserInteropHelper.IsBrowserHosted)
            {
                _fragmentNavigation_activationUriString = System.Windows.Interop.BrowserInteropHelper.Source.ToString();
                if (_fragmentNavigation_activationUriString.EndsWith(".xbap"))
                {
                    int slashIndex = _fragmentNavigation_activationUriString.LastIndexOf('/');
                    _fragmentNavigation_activationUriString = _fragmentNavigation_activationUriString.Substring(0, slashIndex);
                }
            }
            else
            {
                _fragmentNavigation_activationUriString = new Uri(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), UriKind.RelativeOrAbsolute).ToString();
            }
            activationUri = new Uri(_fragmentNavigation_activationUriString, UriKind.RelativeOrAbsolute);
            NavigationHelper.Output("Detected activation URI = " + activationUri);

            FixUpStackURIs(ref _fragmentNavigation_expectedStates, "$(WORKDIR)", _fragmentNavigation_activationUriString);

            // Begin the test
            NavigationHelper.SetStage(TestStage.Run);
            this.StartupUri = new Uri("FragmentNavigation_Page1.xaml", UriKind.RelativeOrAbsolute);
        }

        private void FixUpStackURIs(ref NavigationStateCollection states, string oldString, string newString)
        {
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i].forwardStack != null)
                {
                    for (int j = 0; j < states[i].forwardStack.Length; j++)
                    {
                        string s = states[i].forwardStack[j];
                        if (s.StartsWith(oldString, StringComparison.InvariantCultureIgnoreCase))
                        {
                            string substring = s.Substring(oldString.Length);
                            states[i].forwardStack[j] = newString + substring;
                        }
                    }
                }
                if (states[i].backStack != null)
                {
                    for (int j = 0; j < states[i].backStack.Length; j++)
                    {
                        string s = states[i].backStack[j];
                        if (s.StartsWith(oldString, StringComparison.InvariantCultureIgnoreCase))
                        {
                            string substring = s.Substring(oldString.Length);
                            states[i].backStack[j] = newString + substring;
                        }
                    }
                }
            }
        }

        private static string GetActivationUri()
        {
            string deploymentPath = String.Empty;
            if (BrowserInteropHelper.IsBrowserHosted)
            {
                string src = BrowserInteropHelper.Source.ToString();
                if (src.EndsWith(".xbap"))
                {
                    int index = src.LastIndexOf('/');
                    deploymentPath = src.Substring(0, index + 1);
                }
            }
            else
            {
                deploymentPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
            return deploymentPath;
        }

        void FragmentNavigation_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (_fragmentNavigation_navWin == null)
            {
                _fragmentNavigation_navWin = Application.Current.MainWindow as NavigationWindow;
                _fragmentNavigation_navWin.ContentRendered += new EventHandler(FragmentNavigation_navWin_ContentRendered);
                _fragmentNavigation_helper = new JournalHelper(_fragmentNavigation_navWin);
            }

            if (_fragmentNavigation_currentState != FragmentNavigation_State.GoBack)
            {
                FragmentNavigation_RecordNewResult("LoadCompleted - " + e.Uri);
                FragmentNavigation_NextTest();
            }
        }

        void FragmentNavigation_navWin_ContentRendered(object sender, EventArgs e)
        {
            if (_fragmentNavigation_currentState == FragmentNavigation_State.Init)
            {
                NavigationHelper.Output("Grabbing all the controls we'll play with later");
                _fragmentNavigation_frameScrollViewer = LogicalTreeHelper.FindLogicalNode(_fragmentNavigation_navWin.Content as DependencyObject,
                                        "FragmentNavigation_frameScrollViewer") as ScrollViewer;
                _fragmentNavigation_textBox = LogicalTreeHelper.FindLogicalNode(_fragmentNavigation_navWin.Content as DependencyObject, "textBox") as TextBox;
                _fragmentNavigation_checkBox = LogicalTreeHelper.FindLogicalNode(_fragmentNavigation_navWin.Content as DependencyObject, "checkBox") as CheckBox;
                _fragmentNavigation_frame1 = LogicalTreeHelper.FindLogicalNode(_fragmentNavigation_navWin.Content as DependencyObject, "frame1") as Frame;
                _fragmentNavigation_frame2 = LogicalTreeHelper.FindLogicalNode(_fragmentNavigation_navWin.Content as DependencyObject, "frame2") as Frame;
            }

            switch (_fragmentNavigation_currentState)
            {
                case FragmentNavigation_State.Init:
                    _fragmentNavigation_currentState = FragmentNavigation_State.fragment2;
                    FragmentNavigation_NavigateHyperlink("fragment2");
                    break;

                case FragmentNavigation_State.hlink_page4:
                    _fragmentNavigation_currentState = FragmentNavigation_State.GoBack;
                    FragmentNavigation_GoBack();
                    break;

                case FragmentNavigation_State.GoBack:
                    NavigationHelper.Output("Checking state of controls.");
                    NavigationHelper.Output("TextBox: " + _fragmentNavigation_textBox.Text);
                    NavigationHelper.Output("CheckBox checked? " + _fragmentNavigation_checkBox.IsChecked);
                    String controlStateInfo = "TextBox text = " + _fragmentNavigation_textBox.Text + " checkBox checked = " + _fragmentNavigation_checkBox.IsChecked;
                    FragmentNavigation_RecordNewResult("State = " + _fragmentNavigation_currentState + " ContentRendered "
                        + " Control State = " + controlStateInfo);

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                    (DispatcherOperationCallback)delegate(object ob)
                                    {
                                        FragmentNavigation_HandleFragmentNav();
                                        return null;
                                    }, null);
                    break;
            }
        }

        void FragmentNavigation_Navigated(object sender, NavigationEventArgs e)
        {
            if (_fragmentNavigation_helper != null)
            {
                NavigationHelper.Output(_fragmentNavigation_currentState + ": Recording navigation of " + e.Navigator + " to " + e.Uri);
                FragmentNavigation_RecordNewResult("Navigated - Uri = " + e.Uri + " IsNavigationInitiator " + e.IsNavigationInitiator);
            }
        }

        void FragmentNavigation_FragmentNavigation(object sender, FragmentNavigationEventArgs e)
        {
            NavigationHelper.Output(_fragmentNavigation_currentState + ": Recording navigation of " + e.Navigator + " to " + e.Fragment);
            FragmentNavigation_RecordNewResult("FragmentNavigation " + e.Fragment);

            if (_fragmentNavigation_currentState == FragmentNavigation_State.HandleFragmentNavigation)
            {
                // FragmentNavigation_frameScrollViewer.ScrollChangedEvent shouldn't get fired
                e.Handled = true;
            }
        }

        void FragmentNavigation_frameScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_fragmentNavigation_currentState == FragmentNavigation_State.HandleFragmentNavigation)
            {
                // shouldn't get called 
                NavigationHelper.Output("FAIL!!  ScrollChanged was called in HandleFragmentNavigation state");
                NavigationHelper.Fail("FragmentNavigation test fails");
            }
        }

        #region asynchronous navigation helpers
        void FragmentNavigation_RaiseRequestNavigate(String hyperlinkId)
        {
            Hyperlink hlink = LogicalTreeHelper.FindLogicalNode(_fragmentNavigation_navWin.Content as DependencyObject, hyperlinkId) as Hyperlink;

            NavigationHelper.Output(_fragmentNavigation_currentState + ": Navigating " + hlink.TargetName + " to " + hlink.NavigateUri);
            RequestNavigateEventArgs requestNavigateEventArgs = new RequestNavigateEventArgs(hlink.NavigateUri, hlink.TargetName);
            requestNavigateEventArgs.Source = hlink;
            hlink.RaiseEvent(requestNavigateEventArgs);
        }

        void FragmentNavigation_RequestNavigation(string linkId)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                    (DispatcherOperationCallback)delegate(object ob)
                                    {
                                        FragmentNavigation_RaiseRequestNavigate(linkId);
                                        return null;
                                    }, null);
        }

        void FragmentNavigation_NavigateHyperlink(String linkId)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                    (DispatcherOperationCallback)delegate(object ob)
                                    {
                                        FragmentNavigation_RequestNavigation(linkId);
                                        return null;
                                    }, null);
        }

        void FragmentNavigation_GoBackOnce()
        {
            NavigationHelper.Output(_fragmentNavigation_currentState + ": Calling GoBack on the NavigationWindow");
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                (DispatcherOperationCallback)delegate(object ob)
                                {
                                    _fragmentNavigation_navWin.GoBack();
                                    return null;
                                }, null);
        }

        void FragmentNavigation_GoBack()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                (DispatcherOperationCallback)delegate(object ob)
                                {
                                    FragmentNavigation_GoBackOnce();
                                    return null;
                                }, null);
        }

        void FragmentNavigation_GoForwardOnce()
        {
            NavigationHelper.Output(_fragmentNavigation_currentState + ": Calling GoForward on the NavigationWindow");
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                (DispatcherOperationCallback)delegate(object ob)
                                {
                                    _fragmentNavigation_navWin.GoForward();
                                    return null;
                                }, null);
        }

        void FragmentNavigation_GoForward()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                (DispatcherOperationCallback)delegate(object ob)
                                {
                                    FragmentNavigation_GoForwardOnce();
                                    return null;
                                }, null);
        }

        void FragmentNavigation_HandleFragmentNav()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(FragmentNavigation_NavToHlinkFrame1),
                null);
        }

        Object FragmentNavigation_NavToHlinkFrame1(object arg)
        {
            _fragmentNavigation_currentState = FragmentNavigation_State.HandleFragmentNavigation;
            _fragmentNavigation_frameScrollViewer.ScrollChanged += new ScrollChangedEventHandler(FragmentNavigation_frameScrollViewer_ScrollChanged);
            FragmentNavigation_NavigateHyperlink("hlinkFrame1");
            return null;
        }

        #endregion

        void FragmentNavigation_NextTest()
        {
            switch (_fragmentNavigation_currentState)
            {
                case FragmentNavigation_State.fragment2:
                    _fragmentNavigation_currentState = FragmentNavigation_State.fragment6;
                    FragmentNavigation_NavigateHyperlink("fragment6");
                    break;

                case FragmentNavigation_State.fragment6:
                    _fragmentNavigation_currentState = FragmentNavigation_State.stduri_fragment4;
                    String deploymentPath = Path.Combine(GetActivationUri(), "LooseXaml_FragmentNavigation_Page3.xaml#fragment4");

                    NavigationHelper.Output(_fragmentNavigation_currentState + ": Navigating via Source to new deployment path:\n" + deploymentPath);
                    _fragmentNavigation_frame2.Source = new Uri(deploymentPath, UriKind.RelativeOrAbsolute);
                    break;

                case FragmentNavigation_State.stduri_fragment4:
                    _fragmentNavigation_currentState = FragmentNavigation_State.stduri_fragment5;
                    String deploymentPath2 = Path.Combine(GetActivationUri(), "LooseXaml_FragmentNavigation_Page3.xaml#fragment5");

                    NavigationHelper.Output(_fragmentNavigation_currentState + ": Navigating via API to new deployment path:\n" + deploymentPath2);
                    _fragmentNavigation_frame2.Navigate(new Uri(deploymentPath2, UriKind.RelativeOrAbsolute));
                    break;

                case FragmentNavigation_State.stduri_fragment5:
                    _fragmentNavigation_currentState = FragmentNavigation_State.fragment3;
                    _fragmentNavigation_frame1.Source = new Uri("#fragment3", UriKind.RelativeOrAbsolute);
                    break;

                case FragmentNavigation_State.fragment3:
                    _fragmentNavigation_currentState = FragmentNavigation_State.fragment1;
                    _fragmentNavigation_frame1.Navigate(new Uri("#fragment1", UriKind.RelativeOrAbsolute));
                    break;

                case FragmentNavigation_State.fragment1:
                    _fragmentNavigation_currentState = FragmentNavigation_State.fragment5;
                    FragmentNavigation_NavigateHyperlink("fragment5");
                    break;

                case FragmentNavigation_State.fragment5:
                    _fragmentNavigation_currentState = FragmentNavigation_State.textbox;
                    NavigationHelper.Output(_fragmentNavigation_currentState + ": Changing textBox Text to be 'This is a test'");
                    _fragmentNavigation_textBox = LogicalTreeHelper.FindLogicalNode(_fragmentNavigation_navWin.Content as DependencyObject, "textBox") as TextBox;
                    _fragmentNavigation_textBox.Text = "This is a test";
                    FragmentNavigation_NextTest();
                    break;

                case FragmentNavigation_State.textbox:
                    _fragmentNavigation_currentState = FragmentNavigation_State.hlinkFrame3;
                    FragmentNavigation_NavigateHyperlink("hlinkFrame3");
                    break;

                case FragmentNavigation_State.hlinkFrame3:
                    _fragmentNavigation_currentState = FragmentNavigation_State.checkbox;
                    NavigationHelper.Output(_fragmentNavigation_currentState + ": Changing checkBox to be checked");
                    _fragmentNavigation_checkBox = LogicalTreeHelper.FindLogicalNode(_fragmentNavigation_navWin.Content as DependencyObject, "checkBox") as CheckBox;
                    _fragmentNavigation_checkBox.IsChecked = true;
                    FragmentNavigation_NextTest();
                    break;

                case FragmentNavigation_State.checkbox:
                    _fragmentNavigation_currentState = FragmentNavigation_State.GoBackAll;
                    FragmentNavigation_NextTest();
                    break;

                case FragmentNavigation_State.GoBackAll:
                    NavigationHelper.Output("GoBack to the StartupUri page");
                    if (_fragmentNavigation_navWin.CanGoBack)
                        FragmentNavigation_GoBack();
                    else
                    {
                        _fragmentNavigation_currentState = FragmentNavigation_State.GoForwardAll;
                        FragmentNavigation_NextTest();
                    }
                    break;

                case FragmentNavigation_State.GoForwardAll:
                    NavigationHelper.Output("GoForward to the very last page visited");
                    if (_fragmentNavigation_navWin.CanGoForward)
                        FragmentNavigation_GoForward();
                    else
                    {
                        _fragmentNavigation_currentState = FragmentNavigation_State.hlink_page4;
                        FragmentNavigation_NavigateHyperlink("hlink_page4");
                    }
                    break;

                case FragmentNavigation_State.HandleFragmentNavigation:
                    _fragmentNavigation_currentState = FragmentNavigation_State.End;
                    if (_fragmentNavigation_inVerifyMode)
                    {
                        if (NavigationStateCollection.Compare(_fragmentNavigation_actualStates, _fragmentNavigation_expectedStates))
                        {
                            NavigationHelper.Pass("FragmentNavigation test passes");
                            NavigationHelper.SetStage(TestStage.Cleanup);
                        }
                        else
                        {
                            NavigationHelper.Output("All states did not match - test failed");

                            NavigationHelper.Fail("FragmentNavigation test fails");

                            NavigationHelper.Output("Writing output to isolated storage...");
                            try
                            {
                                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                                {
                                    using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream("actualResults.xml", FileMode.CreateNew, storage))
                                    {
                                        FixUpStackURIs(ref _fragmentNavigation_actualStates, _fragmentNavigation_activationUriString, "$(WORKDIR)");
                                        _fragmentNavigation_actualStates.WriteResults(stream);
                                    }
                                }
                                NavigationHelper.Output("Actual results written to actualResults.xml in isolated storage.");

                            }
                            catch (IOException)
                            {
                                NavigationHelper.Output("WARNING: Unable to write actual results to isolated storage.");
                            }

                        }
                    }
                    else
                    {
                        _fragmentNavigation_actualStates.WriteResults("results.xml");
                    }
                    break;
            }
        }

        void FragmentNavigation_RecordNewResult(string logString)
        {
            _fragmentNavigation_actualStates.RecordNewResult(_fragmentNavigation_helper, logString);
            NavigationHelper.Output(String.Format("[{0} - {1}] {2}",
                _fragmentNavigation_currentState,
                (_fragmentNavigation_currentStateIndex++).ToString(CultureInfo.InvariantCulture),
                logString));
        }

        /// <summary>
        /// Create expected navigation states 
        /// </summary>
        void FragmentNavigation_CreateExpectedNavigationStates()
        {
            _fragmentNavigation_StateDescription
                = new String[]{
                        "State = Init LoadCompleted - FragmentNavigation_Page2.xaml",
                        "Navigated - Uri = pack://siteoforigin:,,,/LooseXaml_FragmentNavigation_Page3.xaml IsNavigationInitiator False",
                        "State = Init LoadCompleted - pack://siteoforigin:,,,/LooseXaml_FragmentNavigation_Page3.xaml",
                        "Navigated - Uri =  IsNavigationInitiator False",
                        "State = Init LoadCompleted - ",
                        "Navigated - Uri = FragmentNavigation_Page1.xaml IsNavigationInitiator True",
                        "State = Init LoadCompleted - FragmentNavigation_Page1.xaml",
                        "FragmentNavigation fragment2",
                        "Navigated - Uri = FragmentNavigation_Page2.xaml#fragment2 IsNavigationInitiator True",
                        "State = fragment2 LoadCompleted - FragmentNavigation_Page2.xaml#fragment2",
                        "FragmentNavigation fragment6",
                        "Navigated - Uri = pack://siteoforigin:,,,/LooseXaml_FragmentNavigation_Page3.xaml#fragment6 IsNavigationInitiator True",
                        "State = fragment6 LoadCompleted - pack://siteoforigin:,,,/LooseXaml_FragmentNavigation_Page3.xaml#fragment6",
                        "Navigated - Uri = $(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4 IsNavigationInitiator True",
                        "State = stduri_fragment4 LoadCompleted - $(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4",
                        "FragmentNavigation fragment5",
                        "Navigated - Uri = $(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5 IsNavigationInitiator True",
                        "State = stduri_fragment5 LoadCompleted - $(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5",
                        "FragmentNavigation fragment3",
                        "Navigated - Uri = FragmentNavigation_Page2.xaml#fragment3 IsNavigationInitiator True",
                        "State = fragment3 LoadCompleted - FragmentNavigation_Page2.xaml#fragment3",
                        "FragmentNavigation fragment1",
                        "Navigated - Uri = FragmentNavigation_Page2.xaml#fragment1 IsNavigationInitiator True",
                        "State = fragment1 LoadCompleted - FragmentNavigation_Page2.xaml#fragment1",
                        "FragmentNavigation fragment5",
                        "Navigated - Uri = pack://siteoforigin:,,,/LooseXaml_FragmentNavigation_Page3.xaml#fragment5 IsNavigationInitiator True",
                        "State = fragment5 LoadCompleted - pack://siteoforigin:,,,/LooseXaml_FragmentNavigation_Page3.xaml#fragment5",
                        "FragmentNavigation fragment5",
                        "FragmentNavigation frame3",
                        "Navigated - Uri = FragmentNavigation_Page1.xaml#frame3 IsNavigationInitiator True",
                        "State = hlinkFrame3 LoadCompleted - FragmentNavigation_Page1.xaml#frame3",
                        "Navigated - Uri = FragmentNavigation_Page1.xaml IsNavigationInitiator True",
                        "State = GoBackAll LoadCompleted - FragmentNavigation_Page1.xaml",
                        "Navigated - Uri = $(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5 IsNavigationInitiator True",
                        "State = GoBackAll LoadCompleted - $(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5",
                        "FragmentNavigation fragment5",
                        "FragmentNavigation fragment3",
                        "Navigated - Uri = FragmentNavigation_Page2.xaml#fragment3 IsNavigationInitiator True",
                        "State = GoBackAll LoadCompleted - FragmentNavigation_Page2.xaml#fragment3",
                        "FragmentNavigation fragment2",
                        "Navigated - Uri = FragmentNavigation_Page2.xaml#fragment2 IsNavigationInitiator True",
                        "State = GoBackAll LoadCompleted - FragmentNavigation_Page2.xaml#fragment2",
                        "FragmentNavigation fragment4",
                        "Navigated - Uri = $(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4 IsNavigationInitiator True",
                        "State = GoBackAll LoadCompleted - $(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4",
                        "Navigated - Uri = pack://siteoforigin:,,,/LooseXaml_FragmentNavigation_Page3.xaml#fragment6 IsNavigationInitiator True",
                        "State = GoBackAll LoadCompleted - pack://siteoforigin:,,,/LooseXaml_FragmentNavigation_Page3.xaml#fragment6",
                        "FragmentNavigation fragment6",
                        "Navigated - Uri = pack://siteoforigin:,,,/LooseXaml_FragmentNavigation_Page3.xaml IsNavigationInitiator True",
                        "State = GoBackAll LoadCompleted - pack://siteoforigin:,,,/LooseXaml_FragmentNavigation_Page3.xaml",
                        "Navigated - Uri = FragmentNavigation_Page2.xaml IsNavigationInitiator True",
                        "State = GoBackAll LoadCompleted - FragmentNavigation_Page2.xaml",
                        "FragmentNavigation fragment2",
                        "Navigated - Uri = FragmentNavigation_Page2.xaml#fragment2 IsNavigationInitiator True",
                        "State = GoForwardAll LoadCompleted - FragmentNavigation_Page2.xaml#fragment2",
                        "FragmentNavigation fragment6",
                        "Navigated - Uri = pack://siteoforigin:,,,/LooseXaml_FragmentNavigation_Page3.xaml#fragment6 IsNavigationInitiator True",
                        "State = GoForwardAll LoadCompleted - pack://siteoforigin:,,,/LooseXaml_FragmentNavigation_Page3.xaml#fragment6",
                        "Navigated - Uri = $(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4 IsNavigationInitiator True",
                        "State = GoForwardAll LoadCompleted - $(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4",
                        "FragmentNavigation fragment4",
                        "FragmentNavigation fragment5",
                        "Navigated - Uri = $(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5 IsNavigationInitiator True",
                        "State = GoForwardAll LoadCompleted - $(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5",
                        "FragmentNavigation fragment3",
                        "Navigated - Uri = FragmentNavigation_Page2.xaml#fragment3 IsNavigationInitiator True",
                        "State = GoForwardAll LoadCompleted - FragmentNavigation_Page2.xaml#fragment3",
                        "FragmentNavigation fragment1",
                        "Navigated - Uri = FragmentNavigation_Page2.xaml#fragment1 IsNavigationInitiator True",
                        "State = GoForwardAll LoadCompleted - FragmentNavigation_Page2.xaml#fragment1",
                        "Navigated - Uri = pack://siteoforigin:,,,/LooseXaml_FragmentNavigation_Page3.xaml#fragment5 IsNavigationInitiator True",
                        "State = GoForwardAll LoadCompleted - pack://siteoforigin:,,,/LooseXaml_FragmentNavigation_Page3.xaml#fragment5",
                        "FragmentNavigation fragment5",
                        "FragmentNavigation frame3",
                        "Navigated - Uri = FragmentNavigation_Page1.xaml#frame3 IsNavigationInitiator True",
                        "State = GoForwardAll LoadCompleted - FragmentNavigation_Page1.xaml#frame3",
                        "Navigated - Uri = Page4.xaml IsNavigationInitiator True",
                        "State = hlink_page4 LoadCompleted - Page4.xaml",
                        "Navigated - Uri = FragmentNavigation_Page1.xaml#frame3 IsNavigationInitiator True",
                        "Navigated - Uri = FragmentNavigation_Page2.xaml#fragment1 IsNavigationInitiator False",
                        "Navigated - Uri = pack://siteoforigin:,,,/LooseXaml_FragmentNavigation_Page3.xaml#fragment5 IsNavigationInitiator False",
                        "Navigated - Uri =  IsNavigationInitiator False",
                        "FragmentNavigation frame3",
                        "FragmentNavigation fragment1",
                        "FragmentNavigation fragment5",
                        "State = GoBack ContentRendered  Control State = TextBox text = This is a test checkBox checked = True",
                        "FragmentNavigation frame1",
                        "Navigated - Uri = FragmentNavigation_Page1.xaml#frame1 IsNavigationInitiator True",
                        "State = HandleFragmentNavigation LoadCompleted - FragmentNavigation_Page1.xaml#frame1"
                };

            _fragmentNavigation_WindowTitle
                = new String[]{
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        ""
                };

            _fragmentNavigation_BackButtonEnabled
                = new bool[]{
                        false,
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
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        false,
                        false,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true
                };

            _fragmentNavigation_ForwardButtonEnabled
                = new bool[]{
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
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
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        false,
                        false,
                        false,
                        false,
                        false,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        false,
                        false
                };

            _fragmentNavigation_BackStack = new String[89][];
            _fragmentNavigation_BackStack[0] = new String[0];
            _fragmentNavigation_BackStack[1] = new String[0];
            _fragmentNavigation_BackStack[2] = new String[0];
            _fragmentNavigation_BackStack[3] = new String[0];
            _fragmentNavigation_BackStack[4] = new String[0];
            _fragmentNavigation_BackStack[5] = new String[0];
            _fragmentNavigation_BackStack[6] = new String[0];
            _fragmentNavigation_BackStack[7] = new String[0];
            _fragmentNavigation_BackStack[8] = new String[] { "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[9] = new String[] { "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[10] = new String[] { "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[11] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[12] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[13] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[14] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[15] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[16] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[17] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[18] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[19] = new String[] { "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[20] = new String[] { "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[21] = new String[] { "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[22] = new String[] { "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[23] = new String[] { "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[24] = new String[] { "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[25] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[26] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[27] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[28] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[29] = new String[] { "FragmentNavigation_Page1.xaml", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[30] = new String[] { "FragmentNavigation_Page1.xaml", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[31] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[32] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[33] = new String[] { "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[34] = new String[] { "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[35] = new String[] { "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[36] = new String[] { "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[37] = new String[] { "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[38] = new String[] { "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[39] = new String[] { "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[40] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[41] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[42] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[43] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[44] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[45] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[46] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[47] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[48] = new String[] { "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[49] = new String[] { "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[50] = new String[0];
            _fragmentNavigation_BackStack[51] = new String[0];
            _fragmentNavigation_BackStack[52] = new String[0];
            _fragmentNavigation_BackStack[53] = new String[] { "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[54] = new String[] { "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[55] = new String[] { "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[56] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[57] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[58] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[59] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[60] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[61] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[62] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[63] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[64] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[65] = new String[] { "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[66] = new String[] { "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[67] = new String[] { "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[68] = new String[] { "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[69] = new String[] { "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[70] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[71] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[72] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[73] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[74] = new String[] { "FragmentNavigation_Page1.xaml", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[75] = new String[] { "FragmentNavigation_Page1.xaml", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[76] = new String[] { "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_BackStack[77] = new String[] { "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_BackStack[78] = new String[] { "FragmentNavigation_Page1.xaml", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6" };
            _fragmentNavigation_BackStack[79] = new String[] { "FragmentNavigation_Page1.xaml", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[80] = new String[] { "FragmentNavigation_Page1.xaml", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[81] = new String[] { "FragmentNavigation_Page1.xaml", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[82] = new String[] { "FragmentNavigation_Page1.xaml", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[83] = new String[] { "FragmentNavigation_Page1.xaml", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[84] = new String[] { "FragmentNavigation_Page1.xaml", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[85] = new String[] { "FragmentNavigation_Page1.xaml", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[86] = new String[] { "FragmentNavigation_Page1.xaml", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[87] = new String[] { "FragmentNavigation_Page1.xaml#frame3", "FragmentNavigation_Page1.xaml", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };
            _fragmentNavigation_BackStack[88] = new String[] { "FragmentNavigation_Page1.xaml#frame3", "FragmentNavigation_Page1.xaml", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "FragmentNavigation_Page2.xaml" };

            _fragmentNavigation_ForwardStack = new String[89][];
            _fragmentNavigation_ForwardStack[0] = new String[0];
            _fragmentNavigation_ForwardStack[1] = new String[0];
            _fragmentNavigation_ForwardStack[2] = new String[0];
            _fragmentNavigation_ForwardStack[3] = new String[0];
            _fragmentNavigation_ForwardStack[4] = new String[0];
            _fragmentNavigation_ForwardStack[5] = new String[0];
            _fragmentNavigation_ForwardStack[6] = new String[0];
            _fragmentNavigation_ForwardStack[7] = new String[0];
            _fragmentNavigation_ForwardStack[8] = new String[0];
            _fragmentNavigation_ForwardStack[9] = new String[0];
            _fragmentNavigation_ForwardStack[10] = new String[0];
            _fragmentNavigation_ForwardStack[11] = new String[0];
            _fragmentNavigation_ForwardStack[12] = new String[0];
            _fragmentNavigation_ForwardStack[13] = new String[0];
            _fragmentNavigation_ForwardStack[14] = new String[0];
            _fragmentNavigation_ForwardStack[15] = new String[0];
            _fragmentNavigation_ForwardStack[16] = new String[0];
            _fragmentNavigation_ForwardStack[17] = new String[0];
            _fragmentNavigation_ForwardStack[18] = new String[0];
            _fragmentNavigation_ForwardStack[19] = new String[0];
            _fragmentNavigation_ForwardStack[20] = new String[0];
            _fragmentNavigation_ForwardStack[21] = new String[0];
            _fragmentNavigation_ForwardStack[22] = new String[0];
            _fragmentNavigation_ForwardStack[23] = new String[0];
            _fragmentNavigation_ForwardStack[24] = new String[0];
            _fragmentNavigation_ForwardStack[25] = new String[0];
            _fragmentNavigation_ForwardStack[26] = new String[0];
            _fragmentNavigation_ForwardStack[27] = new String[0];
            _fragmentNavigation_ForwardStack[28] = new String[0];
            _fragmentNavigation_ForwardStack[29] = new String[0];
            _fragmentNavigation_ForwardStack[30] = new String[0];
            _fragmentNavigation_ForwardStack[31] = new String[] { "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[32] = new String[] { "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[33] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[34] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[35] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[36] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[37] = new String[] { "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[38] = new String[] { "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[39] = new String[] { "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[40] = new String[] { "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[41] = new String[] { "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[42] = new String[] { "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[43] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[44] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[45] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[46] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[47] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[48] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[49] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[50] = new String[] { "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[51] = new String[] { "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[52] = new String[] { "FragmentNavigation_Page2.xaml#fragment2", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[53] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[54] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[55] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment6", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[56] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[57] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment4", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[58] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[59] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[60] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[61] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[62] = new String[] { "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[63] = new String[] { "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[64] = new String[] { "FragmentNavigation_Page2.xaml#fragment3", "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[65] = new String[] { "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[66] = new String[] { "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[67] = new String[] { "FragmentNavigation_Page2.xaml#fragment1", "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[68] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[69] = new String[] { "$(WORKDIR)/LooseXaml_FragmentNavigation_Page3.xaml#fragment5", "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[70] = new String[] { "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[71] = new String[] { "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[72] = new String[] { "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[73] = new String[] { "FragmentNavigation_Page1.xaml#frame3" };
            _fragmentNavigation_ForwardStack[74] = new String[0];
            _fragmentNavigation_ForwardStack[75] = new String[0];
            _fragmentNavigation_ForwardStack[76] = new String[0];
            _fragmentNavigation_ForwardStack[77] = new String[0];
            _fragmentNavigation_ForwardStack[78] = new String[] { "Page 4" };
            _fragmentNavigation_ForwardStack[79] = new String[] { "Page 4" };
            _fragmentNavigation_ForwardStack[80] = new String[] { "Page 4" };
            _fragmentNavigation_ForwardStack[81] = new String[] { "Page 4" };
            _fragmentNavigation_ForwardStack[82] = new String[] { "Page 4" };
            _fragmentNavigation_ForwardStack[83] = new String[] { "Page 4" };
            _fragmentNavigation_ForwardStack[84] = new String[] { "Page 4" };
            _fragmentNavigation_ForwardStack[85] = new String[] { "Page 4" };
            _fragmentNavigation_ForwardStack[86] = new String[] { "Page 4" };
            _fragmentNavigation_ForwardStack[87] = new String[0];
            _fragmentNavigation_ForwardStack[88] = new String[0];
        }
    }
}
