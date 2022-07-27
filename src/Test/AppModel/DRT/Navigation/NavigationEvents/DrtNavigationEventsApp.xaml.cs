// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Globalization;
using System.IO;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Markup;

/**************************************************************************************************
 *                                      DRT DOCUMENTATION
 * This DRT tests all the Navigation events for several scenarios - single level pages, 
 * embedded frames cases, frames with mix of error pages and proper pages,
 * Stopping navigations, refreshing toplevel/frame/error pages etc, elt tree navs, uri navs
 * bookmark navs, back/fwd state in event args etc.
 *
 * To add a new step to the DRT
 * 1. Add a step to Setup[] array in PrepareTests to start a navigation
 *
 * 2. Add a step to Verify[] array in PrepareTests to verify the navigation started in 1. above
 *
 * 3. Add the number of expected navigation events for that navigation to _appExpectedEvents list.
 *    The numbers of expected events will be checked against actual number received before
 *    calling the Verify step which should be used to perform any additional verification.
 *
 *    Include all frame level events also since this is the list of _appExpectedEvents fired on the NavApp
 *    The number of navigation progress events when a frameset page is loading is not definitive.
 *      The frame will fire on the NavApp always but will also start firing cumulative totals 
 *      at the window level once it is hooked into the tree and the latter happens at an indeterminate
 *      time. At the minimum the frame levels events MUST fire on the NavApp. At the max, the same
 *      number of events must fire in addition at the window level and hence again at the NavApp level.
 *      This is what the NavProgress min,max numbers stand for. If refreshing a frame page after the 
 *      top level page is at LoadCompleted MUST fire all NavProgress events i.e. min, max is the same
 *
 * ************************************************************************************************/

namespace DrtNavigationEvents
{
    public partial class DrtNavigationEventsApp : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Navigating          += new NavigatingCancelEventHandler(this.OnNavigating);
            NavigationProgress  += new NavigationProgressEventHandler(this.OnNavigationProgress);
            Navigated           += new NavigatedEventHandler(this.OnNavigated);
            NavigationStopped   += new NavigationStoppedEventHandler(this.OnNavigationStopped);
            LoadCompleted       += new LoadCompletedEventHandler(this.OnLoadCompleted);
            FragmentNavigation += new FragmentNavigationEventHandler(this.OnFragmentNavigation);

            PrepareTests();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_success)
                Log("Test PASSED");
            else
                Log("Test FAILED");

            base.OnExit(e);
        }

        private void OnNavigating(object source, NavigatingCancelEventArgs e)
        {
            VerifyState((++_appActualEvents[Events_Navigating] <= _appExpectedEvents[_step,Events_Navigating]),
                        "Unexpected Navigating Event");

            if (e.NavigationMode == NavigationMode.Refresh)
                ++_appActualEvents[Events_Refresh];

            if (e.NavigationMode == NavigationMode.Back)
                ++_appActualEvents[Events_Back];

            if (e.NavigationMode == NavigationMode.Forward)
                ++_appActualEvents[Events_Forward];
        }

        private void OnNavigationProgress(Object source, NavigationProgressEventArgs e)
        {
            VerifyState(++_appActualEvents[Events_NavigationProgress] <= _appExpectedEvents[_step,Events_NavigationProgressMax],
                        "Unexpected NavigationProgress Event");
        }

        private void OnNavigated(object source, NavigationEventArgs e)
        {
            VerifyState(++_appActualEvents[Events_Navigated] <= _appExpectedEvents[_step,Events_Navigated],
                        "Unexpected Navigated Event");

            /*if (_stopNavigation == true)
            {
                if (e.Uri.ToString().ToLower().EndsWith(_stopNavigationAtPage))
                {
                    INavigator nav = (_stopTopLevel == true) ? NavWindow : e.Navigator as INavigator;
                    nav.StopLoading();
                }
            }*/
        }

        private void OnNavigationStopped(object source, NavigationEventArgs e)
        {
            VerifyState(++_appActualEvents[Events_NavigationStopped] <= _appExpectedEvents[_step,Events_NavigationStopped],
                        "Unexpected NavigationStopped Event");

            if (_stopCondition == Events_NavigationStopped &&
                _appActualEvents[Events_NavigationStopped] == _appExpectedEvents[_step,Events_NavigationStopped])
            {
                VerifyCounters(e);
            }
        }
        
        private void OnLoadCompleted(object source, NavigationEventArgs e)
        {
            VerifyState(++_appActualEvents[Events_LoadCompleted] <= _appExpectedEvents[_step,Events_LoadCompleted],
                        "Unexpected LoadCompleted Event");

            if (_appActualEvents[Events_LoadCompleted] == _appExpectedEvents[_step,Events_LoadCompleted])
            {
                VerifyState(e.IsNavigationInitiator == true, "NavigationInitiator false for toplevel LoadCompleted");
                if (_stopCondition == Events_LoadCompleted)
                {
                    VerifyCounters(e);
                }
            }
            else 
            {
                VerifyState(e.IsNavigationInitiator == false, "NavigationInitiator true for non-toplevel LoadCompleted");
            }
        }

        private void OnFragmentNavigation(object source, FragmentNavigationEventArgs e)
        {
            VerifyState(++_appActualEvents[Events_FragmentNavigation] <= _appExpectedEvents[_step, Events_FragmentNavigation],
                        "Unexpected FragmentNavigation event");
        }

        internal void VerifyCounters(NavigationEventArgs e)
        {
            VerifyCount(_appActualEvents[Events_Navigating], _appExpectedEvents[_step, Events_Navigating], "Navigating events : ");
            VerifyCount(_appActualEvents[Events_Navigated], _appExpectedEvents[_step, Events_Navigated], "Navigated events : ");
            VerifyCount(_appActualEvents[Events_LoadCompleted], _appExpectedEvents[_step, Events_LoadCompleted], "LoadCompleted events : ");
            VerifyCount(_appActualEvents[Events_NavigationStopped], _appExpectedEvents[_step, Events_NavigationStopped], "NavigationStopped events : ");
            VerifyCount(_appActualEvents[Events_Refresh], _appExpectedEvents[_step, Events_Refresh], "NavigationRefresh in Navigating events : ");
            VerifyCount(_appActualEvents[Events_Back], _appExpectedEvents[_step, Events_Back], "NavigationRefresh in Navigating events : ");
            VerifyCount(_appActualEvents[Events_Forward], _appExpectedEvents[_step, Events_Forward], "NavigationRefresh in Navigating events : ");
            VerifyCount(_appActualEvents[Events_FragmentNavigation], _appExpectedEvents[_step, Events_FragmentNavigation], "FragmentNavigationEvents : ");

            VerifyState(_appActualEvents[Events_NavigationProgress] >= _appExpectedEvents[_step, Events_NavigationProgress] &&
                        _appActualEvents[Events_NavigationProgress] <= _appExpectedEvents[_step, Events_NavigationProgressMax],
                        "NavigationProgress events : ");

            //Call delegate to do extra verification
            _verifyTest[_step](e);

            //ResetCounters();
            for (int i = 0; i < _appActualEvents.Length; i++)
                _appActualEvents[i] = 0;

            _setupTest[++_step]();
        }

        private void VerifyCount(int actual, int expected, string message)
        {
            if (actual != expected)
            {
                _success = false;
                Log("Error: Step {0} - {1} - Expected: {2}; Got: {3}", _step, message, expected, actual);
                //This will print the error message and shut down the DRT
                VerifyState(false, "Navigation events count is incorrect");
            }
        }

        private void VerifyBackForwardState(bool canGoBack, bool canGoForward)
        {
            VerifyState(_navWindow.CanGoBack == canGoBack && _navWindow.CanGoForward == canGoForward,
                        "Back/Fwd state is incorrect");
        }

        private void ShutdownApp()
        {
            Shutdown(0);
        }

        private void VerifyUri(Uri uri, string page)
        {
            VerifyState(uri.ToString().ToLower().EndsWith(page), "Incorrect Uri in event");
        }

        private void VerifyState(bool checkPassed, string errorMessage)
        {
            if (!checkPassed)
            {
                Log(errorMessage);
                _success = false;
                Shutdown(-1);
                throw new ApplicationException("ERROR: Step " + ": " + _step + " - " + errorMessage);
            }
        }

        private void Log(string message, params object[] args)
        {
            _logger.Log(message, args);
        }

        private void VerboseLog(string message, params object[] args)
        {
            if (_verbose)
                Log(message, args);
        }

        private int     _step       = 0;
        private bool    _verbose    = false;
        private bool    _success    = true;

        private DRT.Logger _logger  = new DRT.Logger("DrtNavigationEvents", "Microsoft", "Testing Navigation Events");

        private NavigationWindow _navWindow = null;

        //NavigationProgress events are timing related. The ones fired on the NavApp from each container are deterministic
        //but the ones that are refired on window+navapp are not since it depends on when the containers get hooked into 
        //the tree, hence the two states for NavigationProgress
        const int Events_Navigating             = 0;
        const int Events_NavigationProgress     = 1;
        const int Events_NavigationProgressMax  = 2;
        const int Events_Navigated              = 3;
        const int Events_LoadCompleted          = 4;
        const int Events_NavigationStopped      = 5;
        const int Events_Refresh                = 6;
        const int Events_Back                   = 7;
        const int Events_Forward                = 8;
        const int Events_FragmentNavigation     = 9;

        private int     _stopCondition              = Events_LoadCompleted;

        /*private bool    _stopNavigation             = false;
        private bool    _stopTopLevel               = false;
        private string  _stopNavigationAtPage       = String.Empty;*/

        private int[] _appActualEvents = new int[11];

        private int[,] _appExpectedEvents = new int[,] {
            //The second and third values represent the minimum and maximum number of navigation progress 
            //events that can be fired for a given navigation. eg. events from navigating a frame that is 
            //already hooked into the tree is predicatable. Some are just at the 1024 boundary and will
            //fail for long directory names so increasing the max number by 1 in those cases. Will
            //find a more predicatable way to test it (based on App BaseUri length)

            //Navigating, NavProgress, NavProgressMax, Navigated, LoadCompleted, NavStopped, Refresh, Back, Forward
            {1, 0, 0, 1, 1, 0, 0, 0, 0, 0},    //StartupUri - Page10.xaml
            {1, 0, 2, 1, 1, 0, 1, 0, 0, 0},    //Refresh StartupUri - Page10.xaml
            {1, 0, 0, 0, 0, 1, 0, 0, 0, 0},    //StopLoading StartupUri - Page10.xaml
            {1, 0, 0, 1, 1, 0, 0, 0, 0, 0},    //Element tree
            {1, 0, 0, 1, 1, 0, 1, 0, 0, 0},    //Refresh Element tree
            {1, 0, 0, 0, 0, 1, 0, 0, 0, 0},    //StopLoading Element tree
            {1, 0, 2, 1, 1, 0, 0, 1, 0, 0},    //Back to StartupUri 
            {1, 0, 0, 1, 1, 0, 0, 0, 1, 0},    //Forward to Element tree
            {1, 0, 0, 1, 1, 0, 1, 0, 0, 0},    //Refresh Element tree
            {4, 21,49,4, 4, 0, 0, 0, 0, 0},   //Navigate to {11 {21 {31}} {22}}
            {1, 0, 0, 1, 1, 0, 0, 0, 0, 0},    //Navigate Frame {22} -> null uri}            
            {2, 42,44,2, 2, 0, 1, 0, 0, 0},    //Refresh {21 {31}}
            /*{4, 20,42,3, 2, 1, 1, 0, 0},    //Refresh {11 {21 {31}} {22}}, StopLoading {21}
            {1, 20,42, 1, 0, 2, 1, 0, 0},   //Refresh {21 {31}}, StopLoading TopLevel*/
            {2, 22,23,2, 2, 0, 0, 0, 0, 0},    //Navigate 21#anchorTag5
            {1, 0, 0, 1, 1, 0, 0, 0, 0, 0},    //Navigate 21
            {1, 0, 0, 1, 1, 0, 0, 1, 0, 1},    //GoBack 21#anchorTag5
            {1, 0, 0, 1, 1, 0, 0, 0, 1, 0},    //GoForward 21
            {1, 0, 0, 1, 1, 0, 0, 0, 0, 0},    //Navigate 21 again, no-op?
            {1, 0, 0, 1, 1, 0, 0, 0, 0, 1},    //Navigate #anchor10
            {1, 0, 0, 1, 1, 0, 0, 1, 0, 0},    //GoBack 21
            {1, 0, 0, 1, 1, 0, 0, 0, 1, 1},    //GoForward 21#anchorTag10
            {1, 0, 0, 1, 1, 0, 0, 0, 0, 1},    //Navigate #anchorTag10 again, no op?
            {1, 0, 0, 1, 1, 0, 0, 0, 0, 1},    //Navigate #anchorTag20
            {1, 0, 0, 1, 1, 0, 0, 1, 0, 1},    //GoBack 21#anchorTag10
            {1, 0, 0, 1, 1, 0, 0, 0, 1, 1},    //GoForward 21#anchorTag20
            {2, 22,23,2, 2, 0, 1, 0, 0, 0},    //Refresh 21#anchor20
            {1, 0, 0, 1, 1, 0, 0, 0, 0, 0},    //Navigate to element tree
            {1, 0, 0, 1, 1, 0, 0, 0, 0, 1},    //Navigate #anchorTag20
            {1, 0, 0, 1, 1, 0, 0, 1, 0, 0},    //GoBack - root of Element Tree
            //{1, 0, 0, 1, 1, 0, 0, 0, 1},    //GoForward - elementtree#anchorTag20  //Commented out until elttree#fragment journaling 
            {1, 0, 0, 1, 1, 0, 0, 0, 0, 0},    //null content navigation
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 0},    //Window null uri (re)navigation - short-circuited after Navigating
            {2, 22,23,2, 2, 0, 0, 0, 0, 0},    //Navigate 21
            {1, 0, 0, 1, 1, 0, 0, 0, 0, 1},    //Navigate 21#anchorTag5
            };

        private delegate void Setup();
        private delegate void Verify(NavigationEventArgs eventArgs);

        private Setup[] _setupTest = new Setup[0];
        private Verify[] _verifyTest = new Verify[0];

        private void PrepareTests()
        {
            //Default StopCondition is LoadCompleted, set and reset in appropriate steps if otherwise
            _setupTest = new Setup[] {
                //Startup Uri - no frames
                (Setup) delegate() { },
                //Refresh Startup Uri - no frames
                (Setup) delegate() 
                { 
                    _navWindow.Refresh();
                },
                //Navigate, StopLoading immediately - at Startup Uri
                (Setup) delegate() 
                { 
                    _stopCondition = Events_NavigationStopped;
                    _navWindow.Navigate(new Uri("Page11.xaml", UriKind.Relative), "extraData - Page11");
                    _navWindow.StopLoading();
                },
                //Simple Element tree navigation - no frames
                (Setup) delegate() 
                { 
                    _navWindow.Navigate(new DockPanel(), "extraData - DockPanel"); 
                },
                //Refresh Element tree
                (Setup) delegate() 
                { 
                    _navWindow.Refresh();
                },
                //Navigate, StopLoading immediately - at Element tree
                (Setup) delegate() 
                { 
                    _stopCondition = Events_NavigationStopped;
                    _navWindow.Navigate(new Uri("Page11.xaml", UriKind.Relative), "extraData - Page11");
                    _navWindow.StopLoading();
                },
                //Back to Startup Uri
                (Setup) delegate() 
                { 
                    _navWindow.GoBack();
                },
                //Fwd to Element tree
                (Setup) delegate() 
                { 
                    _navWindow.GoForward();
                },                
                //Refresh the error page
                (Setup) delegate() 
                { 
                    _navWindow.Refresh();
                },
                //Navigate to {11 {21 {31}} {22}}
                (Setup) delegate() 
                { 
                    _navWindow.Navigate(new Uri("Page11.xaml", UriKind.RelativeOrAbsolute), "extraData - Page11");
                },
                //Navigate Frame {22} -> null uri}
                (Setup) delegate() 
                { 
                    Frame frame = LogicalTreeHelper.FindLogicalNode((DependencyObject)_navWindow.Content, "RightFrame") as Frame;
                    frame.Source = null;
                },                
                //Refresh {21 {31}}
                (Setup) delegate() 
                { 
                    Frame frame = LogicalTreeHelper.FindLogicalNode((DependencyObject)_navWindow.Content, "LeftFrame") as Frame;
                    frame.Refresh();
                },
                /*//Refresh {11 {21 {31}} {22}}, StopLoading {21}
                (Setup) delegate() 
                { 
                    _stopNavigation         = true;
                    _stopNavigationAtPage   = "page21.xaml";
                    _stopTopLevel           = false;
                    NavWindow.Refresh();
                },
                //Refresh {21 {31}}, StopLoading TopLevel
                (Setup) delegate() 
                { 
                    _stopNavigation         = true;
                    _stopNavigationAtPage   = "page31.xaml";
                    _stopTopLevel           = true;

                    _stopCondition          = Events_NavigationStopped;

                    Frame frame = LogicalTreeHelper.FindLogicalNode(NavWindow.Content, "LeftFrame") as Frame;
                    frame.Refresh();
                    NavWindow.StopLoading();
                },*/
                //Navigate 21#anchorTag5
                (Setup) delegate() 
                { 
                    _navWindow.Navigate(new Uri("DrtFiles/NavigationEvents/Page21.xaml#anchorTag5", UriKind.Relative));
                },
                //Navigate 21
                (Setup) delegate() 
                { 
                    _navWindow.Navigate(new Uri("DrtFiles/NavigationEvents/Page21.xaml", UriKind.Relative), "extraData - Fragment");
                },
                //GoBack 21#anchorTag5
                (Setup) delegate() 
                { 
                    _navWindow.GoBack();
                    //Fwd to Element tree. See note below in the next (Setup)
                    _navWindow.GoForward();
                },
                //GoForward 21
                (Setup) delegate() 
                { 
                    //NOTE: Since NavigateToFragment is a sync call, undoJournalEntry does
                    //not get added until the GoBack call returns and since this step is called
                    //in LoadCompleted which happens before GoBack returns, GoForward will return false
                    //since its too early
                    //NavWindow.GoForward();
                },
                //Navigate 21 again, no-op?
                (Setup) delegate() 
                { 
                    _navWindow.Navigate(new Uri("DrtFiles/NavigationEvents/Page21.xaml", UriKind.Relative));
                },
                //Navigate #anchor10
                (Setup) delegate() 
                { 
                    _navWindow.Navigate(new Uri("#anchorTag10", UriKind.Relative), "extraData - Fragment");
                },
                //GoBack 21
                (Setup) delegate() 
                { 
                    _navWindow.GoBack();
                    //Fwd to #anchorTag10. See note below in the next (Setup)
                    _navWindow.GoForward();
                },
                //GoForward 21#anchorTag10
                (Setup) delegate() 
                { 
                    //NOTE: Since NavigateToFragment is a sync call, undoJournalEntry does
                    //not get added until the GoBack call returns and since this step is called
                    //in LoadCompleted which happens before GoBack returns, GoForward will return false
                    //since its too early
                    //NavWindow.GoForward();
                },
                //Navigate #anchorTag10 again, no op?
                (Setup) delegate() 
                { 
                    _navWindow.Navigate(new Uri("#anchorTag10", UriKind.Relative));
                },
                //Navigate #anchorTag20
                (Setup) delegate() 
                { 
                    _navWindow.Navigate(new Uri("#anchorTag20", UriKind.Relative));
                },
                //GoBack 21#anchorTag10
                (Setup) delegate() 
                { 
                    _navWindow.GoBack();
                    //Fwd to #anchorTag20. See note below in the next (Setup)
                    _navWindow.GoForward();
                },
                //GoForward 21#anchorTag20
                (Setup) delegate() 
                { 
                    //NOTE: Since NavigateToFragment is a sync call, undoJournalEntry does
                    //not get added until the GoBack call returns and since this step is called
                    //in LoadCompleted which happens before GoBack returns, GoForward will return false
                    //since its too early
                    //NavWindow.GoForward();
                },
                //Refresh 21#anchor20
                (Setup) delegate() 
                { 
                    _navWindow.Refresh();
                },
                //Navigate to element tree
                (Setup) delegate() 
                { 
                    Stream contentStream;
                    contentStream = System.IO.File.OpenRead(@"DrtFiles\NavigationEvents\ElementTree.xaml");
                    VerifyState(contentStream != null, "Failed to load content stream for element tree navigation");
                    UIElement content = XamlReader.Load(contentStream) as UIElement;
                    VerifyState(contentStream != null, "Failed to load content tree for element tree navigation");
                    _navWindow.Navigate(content);
                },
                //Navigate #anchorTag20
                (Setup) delegate() 
                { 
                    _navWindow.Navigate(new Uri("#anchorTag20", UriKind.Relative));
                },
                //GoBack - root of Element Tree
                (Setup) delegate() 
                { 
                    _navWindow.GoBack();
                },
                /*  Commented out until 




*/
                //null content navigation
                (Setup) delegate() 
                { 
                    _navWindow.Content = null;
                },
                //Window null uri navigation
                (Setup) delegate() 
                { 
                    _navWindow.Navigate(null);
                    // Since re-navigating to null is short-circuited, there won't be LoadCompleted
                    // raised. VerifyCounters() is called directly to continue the test sequence.
                    VerifyCounters(null);
                },
                (Setup) delegate()
                {
                   _navWindow.Source = new Uri("/DrtFiles/NavigationEvents/Page21.xaml", UriKind.Relative);
                },
                (Setup) delegate()
                {
                   _navWindow.Source = new Uri("/DrtFiles/NavigationEvents/Page21.xaml#anchorTag5", UriKind.Relative);
                },
                //****ADD NEW DRT STEPS ABOVE THIS LINE, KEEP EVENTSCOUNT AND VERIFYTEST STEPS IN SYNC****
                //Final step - Shutdown
                (Setup) delegate() { Shutdown(); }
            };

            _verifyTest = new Verify[] {
                //Startup Uri
                (Verify) delegate(NavigationEventArgs eventArgs) {
                            _navWindow = Application.Current.MainWindow as NavigationWindow; 
                        },
                //Refresh Startup Uri
                (Verify) delegate(NavigationEventArgs eventArgs) {
                            VerifyState(eventArgs.Uri.Equals(_navWindow.CurrentSource), "Uri in args does not match window's Uri"); 
                            VerifyState(_navWindow.Source.Equals(_navWindow.CurrentSource), "Window's uri and pendinguri must be the same");
                            VerifyBackForwardState(false, false);
                        },
                //StopLoading Startup Uri
                (Verify) delegate(NavigationEventArgs eventArgs) {
                            VerifyState(eventArgs.Uri.Equals(_navWindow.CurrentSource), "Uri in args does not match window's Uri"); 
                            VerifyState(_navWindow.Source.Equals(_navWindow.CurrentSource), "StopLoading should reset pending uri");
                            VerifyState(eventArgs.ExtraData.ToString() == "extraData - Page11", "Extra data was not present in eventArgs");
                            _stopCondition = Events_LoadCompleted;
                        },
                //Simple Element tree navigation
                (Verify) delegate(NavigationEventArgs eventArgs) {
                            VerifyState(_navWindow.Content as DockPanel != null, "Unexpected Content"); 
                            VerifyBackForwardState(true, false);
                            VerifyState(eventArgs.ExtraData.ToString() == "extraData - DockPanel", "Extra data was not present in eventArgs");
                        },
                //Refresh Element tree
                (Verify) delegate(NavigationEventArgs eventArgs) {
                            VerifyState(_navWindow.Content as DockPanel != null, "Unexpected Content"); 
                            VerifyBackForwardState(true, false);
                            VerifyState(eventArgs.ExtraData == null, "Stale extra data was present in eventArgs");
                        },
                //StopLoading Element tree
                (Verify) delegate(NavigationEventArgs eventArgs) {
                            VerifyState(eventArgs.Uri == null && _navWindow.Source == null, "StopLoading should clear PendingUri");
                            VerifyState(eventArgs.ExtraData.ToString() == "extraData - Page11", "Extra data was not present in eventArgs");
                            _stopCondition = Events_LoadCompleted;
                        },
                //Verify Back to Startup Uri
                (Verify) delegate(NavigationEventArgs eventArgs) 
                { 
                    VerifyBackForwardState(false, true);
                },
                //Verify Fwd to Element tree
                (Verify) delegate(NavigationEventArgs eventArgs) 
                { 
                    VerifyBackForwardState(true, false);
                },                
                //Refresh Element tree
                (Verify) delegate(NavigationEventArgs eventArgs) {
                            VerifyState(_navWindow.Content as DockPanel != null, "Unexpected Content"); 
                            VerifyBackForwardState(true, false);
                },
                //Navigate to {11 {21 {31}} {22}}
                (Verify) delegate(NavigationEventArgs eventArgs) 
                { 
                    VerifyUri(eventArgs.Uri, "page11.xaml");
                    VerifyState(eventArgs.ExtraData.ToString() == "extraData - Page11", "Extra data was not present in eventArgs");
                },
                //Navigate Frame {22} -> null uri}
                (Verify) delegate(NavigationEventArgs eventArgs) 
                { 
                    VerifyState(eventArgs.Uri == null && 
                                eventArgs.Content == null,
                                "Uri and content in eventArgs should be null for null content navigations");
                    VerifyState(eventArgs.ExtraData == null, "Stale extra data was present in eventArgs");
                    Frame frame = LogicalTreeHelper.FindLogicalNode((DependencyObject)_navWindow.Content, "RightFrame") as Frame;
                    VerifyState(frame.Source == null && frame.Content == null, 
                                "Frame's Content and Source should be null for null uri navigations");
                },                         
                //Refresh {21 {31}}
                (Verify) delegate(NavigationEventArgs eventArgs) 
                { 
                    VerifyUri(eventArgs.Uri, "page21.xaml");
                },
                /* //Refresh {11 {21 {31}} {22}}, StopLoading 21
                (Verify) delegate(NavigationEventArgs eventArgs) 
                { 
                    _stopNavigation         = false;
                    _stopTopLevel           = false;
                    _stopNavigationAtPage   = String.Empty;

                    VerifyState(eventArgs.Uri.ToString().ToLower().EndsWith("page11.xaml"), "Incorrect Uri in event");
                },
                //Refresh {21 {31}}, StopLoading TopLevel
                (Verify) delegate(NavigationEventArgs eventArgs) 
                { 
                    _stopNavigation         = false;
                    _stopTopLevel           = false;
                    _stopNavigationAtPage   = String.Empty;

                    //Toplevel won't fire StopLoading, but frame will
                    VerifyState(eventArgs.Uri.ToString().ToLower().EndsWith("page21.xaml"), "Incorrect Uri in event");

                    _stopCondition = Events_LoadCompleted;
                },*/
                //Navigate 21#anchorTag5
                (Verify) delegate(NavigationEventArgs eventArgs) 
                { 
                    VerifyUri(eventArgs.Uri, "anchortag5");
                },
                //Navigate 21
                (Verify) delegate(NavigationEventArgs eventArgs) 
                { 
                    VerifyUri(eventArgs.Uri, "page21.xaml");
                    VerifyState(eventArgs.ExtraData.ToString() == "extraData - Fragment", "Extra data was not present in eventArgs");
                },
                //GoBack 21#anchorTag5
                (Verify) delegate(NavigationEventArgs eventArgs) 
                { 
                    VerifyUri(eventArgs.Uri, "anchortag5");
                    VerifyState(eventArgs.ExtraData == null, "Stale extra data was present in eventArgs");
                },
                //GoForward 21
                (Verify) delegate(NavigationEventArgs eventArgs) 
                { 
                    VerifyUri(eventArgs.Uri, "page21.xaml");
                },
                //Navigate 21 again, no-op?
                (Verify) delegate(NavigationEventArgs eventArgs) 
                { 
                    VerifyUri(eventArgs.Uri, "page21.xaml");
                },
                //Navigate #anchor10
                (Verify) delegate(NavigationEventArgs eventArgs) 
                { 
                    VerifyUri(eventArgs.Uri, "anchortag10");
                    VerifyState(eventArgs.ExtraData.ToString() == "extraData - Fragment", "Extra data was not present in eventArgs");
                },
                //GoBack 21
                (Verify) delegate(NavigationEventArgs eventArgs) 
                { 
                    VerifyUri(eventArgs.Uri, "page21.xaml");
                    VerifyState(eventArgs.ExtraData == null, "Stale extra data was present in eventArgs");
                },
                //GoForward 21#anchorTag10
                (Verify) delegate(NavigationEventArgs eventArgs) 
                { 
                    VerifyUri(eventArgs.Uri, "anchortag10");
                },
                //Navigate #anchorTag10 again, no op?
                (Verify) delegate(NavigationEventArgs eventArgs) 
                { 
                    VerifyUri(eventArgs.Uri, "anchortag10");
                },
                //Navigate #anchorTag20
                (Verify) delegate(NavigationEventArgs eventArgs) 
                { 
                    VerifyUri(eventArgs.Uri, "anchortag20");
                },
                //GoBack 21#anchorTag10
                (Verify) delegate(NavigationEventArgs eventArgs) 
                { 
                    VerifyUri(eventArgs.Uri, "anchortag10");
                },
                //GoForward 21#anchorTag20
                (Verify) delegate(NavigationEventArgs eventArgs) 
                { 
                    VerifyUri(eventArgs.Uri, "anchortag20");
                },
                //Refresh 21#anchor20
                (Verify) delegate(NavigationEventArgs eventArgs) 
                { 
                    VerifyUri(eventArgs.Uri, "anchortag20");
                },
                //Navigate to element tree
                (Verify) delegate(NavigationEventArgs eventArgs) 
                { 
                    VerifyState(eventArgs.Uri == null, "Uri should be null for element navigations");
                },
                //Navigate #anchorTag20
                (Verify) delegate(NavigationEventArgs eventArgs) 
                { 
                    VerifyUri(eventArgs.Uri, "anchortag20");
                },
                //GoBack - root of Element Tree
                (Verify) delegate(NavigationEventArgs eventArgs) 
                { 
                    VerifyState(eventArgs.Uri == null, "Uri should be null for element navigations");
                },
                /* //GoForward - elementtree#anchorTag20
                (Verify) delegate(NavigationEventArgs eventArgs) 
                { 
                    VerifyUri(eventArgs.Uri, "anchortag20");
                }, */
                //null content navigation
                (Verify) delegate(NavigationEventArgs eventArgs) 
                { 
                    VerifyState(eventArgs.Uri == null && 
                                eventArgs.Content == null && 
                                _navWindow.Content == null ,
                                "Uri and content should be null for null content navigations");
                },
                //Window null uri navigation
                (Verify) delegate(NavigationEventArgs eventArgs) 
                { 
                    // Special case: null is passed for NavigationEventArgs here.
                    VerifyState(_navWindow.Source == null && 
                                _navWindow.Content == null ,
                                "Uri and content should be null for null content navigations");
                },
                (Verify) delegate(NavigationEventArgs eventArgs) 
                { 
                    VerifyUri(eventArgs.Uri, "page21.xaml");
                },
                (Verify) delegate(NavigationEventArgs eventArgs) 
                { 
                    VerifyUri(eventArgs.Uri, "anchortag5");
                },
            };

            //Call first delegate
            _setupTest[0]();
        }
    }
}
