// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Test case to verify the behaviour of JournalOwnership property on a frame
//               This will also verify NavigationUIVisibility property on a frame
//               Navigate the parent and frame and verify the journal against the expected
//
//JournalOwnership is a property you set to determine which "journal" a frame will use 
//(the internal Journal type encapsulates navigation history in Windows Presentation 
//Foundation), and can be one of the JournalOwnership enumeration values: 
//
//enum JournalOwnership 
//{
//  Automatic,        // "UsesParentJournal" if hosted by Frame or 
//                    // NavigationWindow, "OwnsJournal" otherwise. 
//                    // (default)
//  OwnsJournal,      // Frame manages navigation history
//  UsesParentJournal // Frame's host manages navigation history
//}
//
//

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Windows.Media;
using Microsoft.Test;
using Microsoft.Test.Logging;           
using System.Collections;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public class JournalOwnershipTest
    {
        // state of the test
        private enum TestState
        {
            Start,            // Set the frame.Source
            ParentFirstNav,   // Navigate NavigationWindow to page3
            ParentGoBack1,    // GoBack NavigationWindow
            FrameFirstNav,    // Navigate frame to imagePage
            ParentSecondNav,  // Navigate NavigationWindow to page4
            ParentGoBack2,    // GoBack NavigationWindow
            FrameGoBack,      // GoBack on frame
            FrameGoForward,   // GoForward on frame
            ParentGoForward,  // GoForward on NavigationWindow
            End
        }

        private TestState _testState = TestState.Start;
        // navigation pages for the parent
        // Start page is SingleFrameTestPage.xaml
        private const String page3 = "page3.xaml";
        private const String page4 = "page4.xaml";
        // navigation pages for the frame
        private const String anchoredPage = "AnchoredPage_Loose.xaml"; // set as the source initially
        private const String imagePage = "ImagePage_Loose.xaml";
        private const String flowDocPage = "FlowDocument_Loose.xaml";

        private SingleFrameTestClass _frameTest = null;
        private JournalOwnership _journalOwnership; // JournalOwnership property of the current test
        private NavigationStateCollection _actualParentStates = new NavigationStateCollection();
        private NavigationStateCollection _expectedParentStates = new NavigationStateCollection();
        private NavigationStateCollection _actualFrameStates = new NavigationStateCollection();
        private NavigationStateCollection _expectedFrameStates = new NavigationStateCollection();
        private JournalHelper _journalHelper = null;

        // expected values for NavigationState Parent
        private String[] _stateDescription = null;
        private String[] _windowTitle = null;
        private bool[] _backButtonEnabled = null;
        private bool[] _forwardButtonEnabled = null;
        private String[][] _backStack = null;
        private String[][] _forwardStack = null;

        // expected values for NavigationState Frame
        private String[] _stateDescriptionFrame = null;
        private String[] _windowTitleFrame = null;
        private bool[] _backButtonEnabledFrame = null;
        private bool[] _forwardButtonEnabledFrame = null;
        private String[][] _backStackFrame = null;
        private String[][] _forwardStackFrame = null;

        public void Startup(object sender, StartupEventArgs e)
        {
            // retrive the argument and set the appropriate JournalOwnership property
            NavigationHelper.Output("Testing for the JournalOwnership property " + DriverState.DriverParameters["JournalOwnershipTest"]);
            switch (DriverState.DriverParameters["JournalOwnershipTest"])
            {
                case "Automatic":
                    _journalOwnership = JournalOwnership.Automatic;
                    CreateExpectedNavigationStatesAutomatic();
                    CreateExpectedNavigationStatesAutomaticFrame();
                    break;

                case "OwnsJournal":
                    _journalOwnership = JournalOwnership.OwnsJournal;
                    CreateExpectedNavigationStatesOwnsJournal();
                    CreateExpectedNavigationStatesOwnsJournalFrame();
                    break;

                case "UsesParentJournal":
                    _journalOwnership = JournalOwnership.UsesParentJournal;
                    CreateExpectedNavigationStatesAutomatic(); // same states as in Automatic
                    CreateExpectedNavigationStatesAutomaticFrame();
                    break;
            }

            // instantiate the frameTest object
            _frameTest = new SingleFrameTestClass("JournalOwnership");

            // populate the expected navigation states
            PopulateExpectedNavigationStatesForParentWindow();

            // navigation states for the frame
            PopulateExpectedNavigationStatesForFrame();
        }

        /// <summary>
        /// The test expects the frame hosted within the parent windo to go through a sequence of 
        /// NavigationStates. These are populated in the expectedFrameStates collection. 
        /// </summary>
        private void PopulateExpectedNavigationStatesForFrame()
        {
            for (int index = 0; index < _stateDescriptionFrame.Length; index++)
            {
                _expectedFrameStates.states.Add(new NavigationState(_stateDescriptionFrame[index],
                    _windowTitleFrame[index],
                    _backButtonEnabledFrame[index],
                    _forwardButtonEnabledFrame[index],
                    _backStackFrame[index],
                    _forwardStackFrame[index]));
            }
        }

        /// <summary>
        /// The test expects the ParentWindow to go through a sequence of NavigationStates 
        /// These states are populated in the expectedParentStates collection. they are later comp
        /// </summary>
        private void PopulateExpectedNavigationStatesForParentWindow()
        {
            for (int index = 0; index < _stateDescription.Length; index++)
            {
                _expectedParentStates.states.Add(new NavigationState(_stateDescription[index],
                    _windowTitle[index],
                    _backButtonEnabled[index],
                    _forwardButtonEnabled[index],
                    _backStack[index],
                    _forwardStack[index]));
            }
        }

        /// <summary>
        /// App Navigated Event Handler
        /// </summary>
        /// <param name="source">object</param>
        /// <param name="e">NavigationEventArgs</param>
        public void Navigated(object source, NavigationEventArgs e)
        {
            Frame frame = _frameTest.GetFrame();
            NavigationHelper.Output(string.Format("-------Enter: Navigated: {0}---------------------",(e.Uri == null) ? "NULL" : e.Uri.ToString()));
            NavigationHelper.Output(string.Format("TestState: {0}", _testState));
            DisplayNavigationWindowJournalStacks(_frameTest.NavigationWindow);
            DisplayFrameJournalStacks(frame);

            // record the journal for the NavigationWindow
            if (_journalHelper != null)
            {
                _actualParentStates.RecordNewResult(_journalHelper, "App Navigated Navigator = " + e.Navigator);
            }
            else
            {
                _journalHelper = new JournalHelper(_frameTest.NavigationWindow);
            }

            frame = _frameTest.GetFrame(); // get the frame object
            // record the journal for the frame
            if (frame != null && _journalHelper != null && _testState != TestState.Start)
            {
                _actualFrameStates.RecordNewResult(_journalHelper, frame, "Frame Navigated Navigator = " + e.Navigator);
            }

            switch (_testState)
            {
                case TestState.Start:
                    if (frame == null)
                    {
                        _frameTest.Fail("Could not locate Frame to run JournalOwnership test case on");
                    }

                    // set the JournalOwnership property for testing
                    if (_journalOwnership != JournalOwnership.Automatic)
                    {
                        frame.JournalOwnership = _journalOwnership;
                    }
                    else
                    {
                        // default is automatic and nothing to set
                        if (frame.JournalOwnership != JournalOwnership.Automatic)
                        {
                            NavigationHelper.Fail("Default JournalOwnership is not set to JournalOwnership.Automatic");
                        }
                    }

                    // check for NavigationUIVisibility property
                    if (frame.NavigationUIVisibility == NavigationUIVisibility.Visible)
                    {
                        if (_journalOwnership != JournalOwnership.OwnsJournal)
                        {
                            NavigationHelper.Fail("Found NavigationUIVisibility.Visible when journalOwnership != JournalOwnership.OwnsJournal");
                        }
                    }
                    else if (frame.NavigationUIVisibility == NavigationUIVisibility.Hidden)
                    {
                        if (_journalOwnership == JournalOwnership.OwnsJournal)
                        {
                            NavigationHelper.Fail("Found NavigationUIVisibility Hidden when journalOwnership = JournalOwnership.OwnsJournal");
                        }
                    }

                    // setting the Source property should navigate to the page
                    frame.Source = new Uri(anchoredPage, UriKind.RelativeOrAbsolute);
                    NavigationHelper.Output("Setting the frame.Source to " + anchoredPage);
                    _testState = TestState.ParentFirstNav;

                    frame = _frameTest.GetFrame();
                    if (frame == null)
                    {
                        NavigationHelper.Fail("Could not get frame element");
                    }
                    break;
                    
                case TestState.ParentSecondNav:
                    NavigationWindow nw = _frameTest.NavigationWindow;
                    _testState = TestState.ParentGoBack2;
                    NavigationHelper.Output("NavigationWindow(Parent): Navigate to : " + page4);
                    nw.Navigate(new Uri(page4, UriKind.RelativeOrAbsolute));
                    break;

                case TestState.FrameGoForward:
                    PerformNextState();
                    break;

                case TestState.ParentGoForward:
                    PerformNextState();
                    break;
            }
            NavigationHelper.Output("--------Exit: Navigated: -------------------------------------------------------------");
        }

        private void DisplayNavigationWindowJournalStacks(NavigationWindow nWin)
        {
            if (null != nWin)
            {
                NavigationHelper.Output(string.Format("~NavWin:Journal:CanGoBack   : {0}", nWin.CanGoBack));
                if (nWin.CanGoBack)
                {
                    IEnumerable entries = (IEnumerable)nWin.GetValue(NavigationWindow.BackStackProperty);
                    DisplayStack(entries);
                }

                NavigationHelper.Output(string.Format("~NavWin:Journal:CanGoForward: {0}", nWin.CanGoForward));
                if (nWin.CanGoForward)
                {
                    IEnumerable entries = (IEnumerable)nWin.GetValue(NavigationWindow.BackStackProperty);
                    DisplayStack(entries);
                }
 
            }
        }

        private void DisplayFrameJournalStacks(Frame frame)
        {
            /// Display the BackStack:
            if (null != frame)
            {
                NavigationHelper.Output(string.Format("~Frame :Journal:CanGoBack   : {0}", frame.CanGoBack));
                if (frame.CanGoBack)
                {
                    DisplayStack(frame.BackStack);
                }

                NavigationHelper.Output(string.Format("~Frame :Journal:CanGoForward: {0}", frame.CanGoForward));
                if (frame.CanGoForward)
                {
                    DisplayStack(frame.ForwardStack);
                }
            }
        }

        /// <summary>
        /// Displays JournalEntry entries from a NavigationWindow or Frame 
        /// ForwardStack or backstack.
        /// </summary>
        /// <param name="entries">IEnumerable which maps to JournalEntry</param>
        private void DisplayStack(IEnumerable entries)
        {
            NavigationHelper.Output("~DisplayStack: ~~~~~~~~~~~~~~~~~~~: Begin");
            IEnumerator entryEnumerator = entries.GetEnumerator();
            while (entryEnumerator.MoveNext())
            {
                JournalEntry entry = (JournalEntry)entryEnumerator.Current;
                NavigationHelper.Output(string.Format("~DisplayStack: ~~: {0}", entry.Name));
            }
            NavigationHelper.Output("~DisplayStack: ~~~~~~~~~~~~~~~~~~~: End");
        }


        // ContentRendered handler for the parent
        public void ContentRendered(object sender, EventArgs e)
        {
            NavigationHelper.Output("--------Enter: ContentRendered: -------------------------------------------------------------");
            NavigationHelper.Output("-----TestState = " + _testState.ToString());

            NavigationWindow nw = _frameTest.NavigationWindow;
            Frame frame = _frameTest.GetFrame();
            DisplayNavigationWindowJournalStacks(nw);
            DisplayFrameJournalStacks(frame);

            // Recored Navigation States
            if (frame != null)
            {
                _actualFrameStates.RecordNewResult(_journalHelper, frame, "OnContentRendered Source = " + frame.Source);
            }

            if (nw != null)
            {
                _actualParentStates.RecordNewResult(_journalHelper, "OnContentRendered Source = " + nw.Source);
            }

            if (nw != null)
            {
                switch (_testState)
                {
                    case TestState.ParentFirstNav:
                        _testState = TestState.ParentGoBack1;
                        NavigationHelper.Output("NavigationWindow(Parent): Navigate to: " + page3);
                        nw.Navigate(new Uri(page3, UriKind.RelativeOrAbsolute));
                        break;

                    case TestState.ParentGoBack1:
                        _testState = TestState.FrameFirstNav;
                        NavigationHelper.Output("NavigationWindow(Parent): Call GoBack");
                        nw.GoBack();
                        break;

                    case TestState.ParentGoBack2:
                        _testState = TestState.FrameGoBack;
                        NavigationHelper.Output("NavigationWindow(Parent): Call GoBack");
                        nw.GoBack();
                        break;
                }
            }

            if (frame != null)
            {
                switch (_testState)
                {
                    case TestState.FrameFirstNav:
                        _testState = TestState.ParentSecondNav;
                        NavigationHelper.Output("Frame: Navigate to: " + imagePage);
                        frame.Navigate(new Uri(imagePage, UriKind.RelativeOrAbsolute));
                        break;

                    case TestState.FrameGoBack:
                        _testState = TestState.FrameGoForward;

                        // verify the NavMenu if frame owns the journal
                        if (_journalOwnership == JournalOwnership.OwnsJournal)
                        {
                            TreeUtilities treeUtilities = new TreeUtilities();
                            if (treeUtilities.FindVisualTreeElementByID("NavMenu", (DependencyObject)frame) == null)
                            {
                                NavigationHelper.Fail("Couldn't find the NavMenu when journalOwnership = JournalOwnership.OwnsJournal");
                            }
                            else
                            {
                                NavigationHelper.Output("Found NavMenu");
                            }
                        }

                        NavigationHelper.Output("Calling GoBack on frame");
                        frame.GoBack();
                        break;
                }
            }
            else   
            {
                if (_testState == TestState.End)
                {
                    // The last state was to navigate to a page without a frame
                    // Compare the actual journal against expected
                    if (NavigationStateCollection.Compare(_actualParentStates, _expectedParentStates) == false)
                    {
                        NavigationHelper.Fail("All parent states did not match");
                    }
                    else
                    {
                        NavigationHelper.Output("All parent states matched");
                    }

                    if (NavigationStateCollection.Compare(_actualFrameStates, _expectedFrameStates))
                    {
                        NavigationHelper.Pass("All frame states matched");
                    }
                    else
                    {
                        NavigationHelper.Fail("All frame states did not match");
                    }
                }
            }
            NavigationHelper.Output("--------Exit : ContentRendered: -------------------------------------------------------------");
        }

        // perform the functions of the next state
        private void PerformNextState()
        {
            NavigationHelper.Output("In PerformNextState");
            NavigationHelper.Output("-----TestState = " + _testState.ToString());

            NavigationWindow nw = _frameTest.NavigationWindow;

            if (nw != null)
            {
                switch (_testState)
                {
                    case TestState.FrameGoForward:
                        _testState = TestState.ParentGoForward;
                        NavigationHelper.Output("Calling GoForward on frame");
                        Frame frame = _frameTest.GetFrame();
                        if (frame != null)
                        {
                            frame.GoForward();
                        }
                        else
                        {
                            NavigationHelper.Fail("Couldn't retrieve the frame object (null)");
                        }

                        break;

                    case TestState.ParentGoForward:
                        _testState = TestState.End;
                        NavigationHelper.Output("Calling GoForward on parent");
                        nw.GoForward();
                        break;
                }
            }
            else
            {
                NavigationHelper.Fail("Couldn't retrieve the NavigationWindow (null)");
            }
        }

        // when frame doesn't own it's journal (JornalOwnership.OwnsJournal), frame.GoBack() and
        // frame.GoForward() are not allowed and InvalidOperationException  is thrown
        public void DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            NavigationHelper.Output("Dispatcher exception = " + e.Exception);
            NavigationHelper.Output("State = " + _testState);
            e.Handled = true;

            // we may get the InvalidOperationException depending on the JournalOwnership property
            if (e.Exception is System.InvalidOperationException)
            {
                // previous state is FrameGoBack or FrameGoForward
                if (_testState == TestState.FrameGoForward || _testState == TestState.ParentGoForward) 
                {
                    Frame frame = _frameTest.GetFrame();
                    if (frame != null)
                    {
                        if (_journalOwnership == JournalOwnership.OwnsJournal)
                        {
                            NavigationHelper.Fail("frame.GoBack() or frame.GoForward() failed with JournalOwnership.OwnsJournal");
                        }
                        else
                        {
                            // expected exception, perform the next state
                            PerformNextState();
                        }
                    }
                    else
                    {
                        NavigationHelper.Fail("Couldn't retrieve the frame object (null)");
                    }
                }
                else
                {
                    NavigationHelper.Fail("Failed due to dispatcher exception caught in unexpected state");
                }
            }
            else
            {
                NavigationHelper.Fail("Failed due to unexpected dispatcher exception");
            }
        }

        /// <summary>
        /// Create expected navigation states 
        /// </summary>
        private void CreateExpectedNavigationStatesAutomatic()
        {
            _stateDescription
                = new String[]{
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: AnchoredPage_Loose.xaml",
                        "OnContentRendered Source = AnchoredPage_Loose.xaml",
                        "App LoadCompleted Navigator = System.Windows.Navigation.NavigationWindow",
                        "OnContentRendered Source = page3.xaml",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: AnchoredPage_Loose.xaml",
                        "App LoadCompleted Navigator = System.Windows.Navigation.NavigationWindow",
                        "OnContentRendered Source = AnchoredPage_Loose.xaml",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: ImagePage_Loose.xaml",
                        "App LoadCompleted Navigator = System.Windows.Navigation.NavigationWindow",
                        "OnContentRendered Source = page4.xaml",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: ImagePage_Loose.xaml",
                        "App LoadCompleted Navigator = System.Windows.Navigation.NavigationWindow",
                        "OnContentRendered Source = ImagePage_Loose.xaml",
                        "App LoadCompleted Navigator = System.Windows.Navigation.NavigationWindow",
                        "OnContentRendered Source = page4.xaml"
                };

            _windowTitle
                = new String[]{
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage"
                };

            _backButtonEnabled
                = new bool[]{
                        false,
                        false,
                        true,
                        true,
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
                        true
                };

            _forwardButtonEnabled
                = new bool[]{
                        false,
                        false,
                        false,
                        false,
                        true,
                        true,
                        true,
                        false,
                        false,
                        false,
                        true,
                        true,
                        true,
                        false,
                        false
                };

            _backStack = new String[15][];
            _backStack[0] = new String[0];
            _backStack[1] = new String[0];
            _backStack[2] = new String[] { "SingleFrameTestPage (SingleFrameTestPage.xaml)" };
            _backStack[3] = new String[] { "SingleFrameTestPage (SingleFrameTestPage.xaml)" };
            _backStack[4] = new String[0];
            _backStack[5] = new String[0];
            _backStack[6] = new String[0];
            _backStack[7] = new String[] { "Anchored Page" };
            _backStack[8] = new String[] { "SingleFrameTestPage (SingleFrameTestPage.xaml)" };
            _backStack[9] = new String[] { "SingleFrameTestPage (SingleFrameTestPage.xaml)" };
            _backStack[10] = new String[] { "Anchored Page" };
            _backStack[11] = new String[] { "Anchored Page" };
            _backStack[12] = new String[] { "Anchored Page" };
            _backStack[13] = new String[] { "SingleFrameTestPage (SingleFrameTestPage.xaml)" };
            _backStack[14] = new String[] { "SingleFrameTestPage (SingleFrameTestPage.xaml)" };

            _forwardStack = new String[15][];
            _forwardStack[0] = new String[0];
            _forwardStack[1] = new String[0];
            _forwardStack[2] = new String[0];
            _forwardStack[3] = new String[0];
            _forwardStack[4] = new String[] { "SingleFrameTestPage (page3.xaml)" };
            _forwardStack[5] = new String[] { "SingleFrameTestPage (page3.xaml)" };
            _forwardStack[6] = new String[] { "SingleFrameTestPage (page3.xaml)" };
            _forwardStack[7] = new String[0];
            _forwardStack[8] = new String[0];
            _forwardStack[9] = new String[0];
            _forwardStack[10] = new String[] { "SingleFrameTestPage (page4.xaml)" };
            _forwardStack[11] = new String[] { "SingleFrameTestPage (page4.xaml)" };
            _forwardStack[12] = new String[] { "SingleFrameTestPage (page4.xaml)" };
            _forwardStack[13] = new String[0];
            _forwardStack[14] = new String[0];
        }

        /// <summary>
        /// Create expected navigation states 
        /// Here we have more states due to frame.GoForward and frame.GoBack
        /// </summary>
        private void CreateExpectedNavigationStatesOwnsJournal()
        {
            _stateDescription
                = new String[]{
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: AnchoredPage_Loose.xaml",
                        "OnContentRendered Source = AnchoredPage_Loose.xaml",
                        "App LoadCompleted Navigator = System.Windows.Navigation.NavigationWindow",
                        "OnContentRendered Source = page3.xaml",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: AnchoredPage_Loose.xaml",
                        "App LoadCompleted Navigator = System.Windows.Navigation.NavigationWindow",
                        "OnContentRendered Source = AnchoredPage_Loose.xaml",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: ImagePage_Loose.xaml",
                        "App LoadCompleted Navigator = System.Windows.Navigation.NavigationWindow",
                        "OnContentRendered Source = page4.xaml",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: ImagePage_Loose.xaml",
                        "App LoadCompleted Navigator = System.Windows.Navigation.NavigationWindow",
                        "OnContentRendered Source = ImagePage_Loose.xaml",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: AnchoredPage_Loose.xaml",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: ImagePage_Loose.xaml",
                        "App LoadCompleted Navigator = System.Windows.Navigation.NavigationWindow",
                        "OnContentRendered Source = page4.xaml"
                };

            _windowTitle
                = new String[]{
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage"
                };

            _backButtonEnabled
                = new bool[]{
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
                        false,
                        false,
                        true,
                        true
                };

            _forwardButtonEnabled
                = new bool[]{
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
                        true,
                        true,
                        false,
                        false
                };

            _backStack = new String[17][];
            _backStack[0] = new String[0];
            _backStack[1] = new String[0];
            _backStack[2] = new String[] { "SingleFrameTestPage (SingleFrameTestPage.xaml)" };
            _backStack[3] = new String[] { "SingleFrameTestPage (SingleFrameTestPage.xaml)" };
            _backStack[4] = new String[0];
            _backStack[5] = new String[0];
            _backStack[6] = new String[0];
            _backStack[7] = new String[0];
            _backStack[8] = new String[] { "SingleFrameTestPage (SingleFrameTestPage.xaml)" };
            _backStack[9] = new String[] { "SingleFrameTestPage (SingleFrameTestPage.xaml)" };
            _backStack[10] = new String[0];
            _backStack[11] = new String[0];
            _backStack[12] = new String[0];
            _backStack[13] = new String[0];
            _backStack[14] = new String[0];
            _backStack[15] = new String[] { "SingleFrameTestPage (SingleFrameTestPage.xaml)" };
            _backStack[16] = new String[] { "SingleFrameTestPage (SingleFrameTestPage.xaml)" };

            _forwardStack = new String[17][];
            _forwardStack[0] = new String[0];
            _forwardStack[1] = new String[0];
            _forwardStack[2] = new String[0];
            _forwardStack[3] = new String[0];
            _forwardStack[4] = new String[] { "SingleFrameTestPage (page3.xaml)" };
            _forwardStack[5] = new String[] { "SingleFrameTestPage (page3.xaml)" };
            _forwardStack[6] = new String[] { "SingleFrameTestPage (page3.xaml)" };
            _forwardStack[7] = new String[] { "SingleFrameTestPage (page3.xaml)" };
            _forwardStack[8] = new String[0];
            _forwardStack[9] = new String[0];
            _forwardStack[10] = new String[] { "SingleFrameTestPage (page4.xaml)" };
            _forwardStack[11] = new String[] { "SingleFrameTestPage (page4.xaml)" };
            _forwardStack[12] = new String[] { "SingleFrameTestPage (page4.xaml)" };
            _forwardStack[13] = new String[] { "SingleFrameTestPage (page4.xaml)" };
            _forwardStack[14] = new String[] { "SingleFrameTestPage (page4.xaml)" };
            _forwardStack[15] = new String[0];
            _forwardStack[16] = new String[0];
        }

        /// <summary>
        /// Create expected navigation states for Frame
        /// </summary>
        private void CreateExpectedNavigationStatesAutomaticFrame()
        {
            _stateDescriptionFrame
                = new String[]{
                        "Frame LoadCompleted Navigator = System.Windows.Controls.Frame: AnchoredPage_Loose.xaml",
                        "OnContentRendered Source = AnchoredPage_Loose.xaml",
                        "Frame LoadCompleted Navigator = System.Windows.Controls.Frame: AnchoredPage_Loose.xaml",
                        "Frame LoadCompleted Navigator = System.Windows.Navigation.NavigationWindow",
                        "OnContentRendered Source = AnchoredPage_Loose.xaml",
                        "Frame LoadCompleted Navigator = System.Windows.Controls.Frame: ImagePage_Loose.xaml",
                        "Frame LoadCompleted Navigator = System.Windows.Controls.Frame: ImagePage_Loose.xaml",
                        "Frame LoadCompleted Navigator = System.Windows.Navigation.NavigationWindow",
                        "OnContentRendered Source = ImagePage_Loose.xaml"
                };

            _windowTitleFrame
                = new String[]{
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage"
                };

            _backButtonEnabledFrame
                = new bool[]{
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false
                };

            _forwardButtonEnabledFrame
                = new bool[]{
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false
                };

            _backStackFrame = new String[10][];
            _backStackFrame[0] = new String[0];
            _backStackFrame[1] = new String[0];
            _backStackFrame[2] = new String[0];
            _backStackFrame[3] = new String[0];
            _backStackFrame[4] = new String[0];
            _backStackFrame[5] = new String[0];
            _backStackFrame[6] = new String[0];
            _backStackFrame[7] = new String[0];
            _backStackFrame[8] = new String[0];

            _forwardStackFrame = new String[10][];
            _forwardStackFrame[0] = new String[0];
            _forwardStackFrame[1] = new String[0];
            _forwardStackFrame[2] = new String[0];
            _forwardStackFrame[3] = new String[0];
            _forwardStackFrame[4] = new String[0];
            _forwardStackFrame[5] = new String[0];
            _forwardStackFrame[6] = new String[0];
            _forwardStackFrame[7] = new String[0];
            _forwardStackFrame[8] = new String[0];
        }

        /// <summary>
        /// Create expected navigation states for Frame
        /// </summary>
        private void CreateExpectedNavigationStatesOwnsJournalFrame()
        {
            _stateDescriptionFrame
                = new String[]{
                        "Frame LoadCompleted Navigator = System.Windows.Controls.Frame: AnchoredPage_Loose.xaml",
                        "OnContentRendered Source = AnchoredPage_Loose.xaml",
                        "Frame LoadCompleted Navigator = System.Windows.Controls.Frame: AnchoredPage_Loose.xaml",
                        "Frame LoadCompleted Navigator = System.Windows.Navigation.NavigationWindow",
                        "OnContentRendered Source = AnchoredPage_Loose.xaml",
                        "Frame LoadCompleted Navigator = System.Windows.Controls.Frame: ImagePage_Loose.xaml",
                        "Frame LoadCompleted Navigator = System.Windows.Controls.Frame: ImagePage_Loose.xaml",
                        "Frame LoadCompleted Navigator = System.Windows.Navigation.NavigationWindow",
                        "OnContentRendered Source = ImagePage_Loose.xaml",
                        "Frame LoadCompleted Navigator = System.Windows.Controls.Frame: AnchoredPage_Loose.xaml",
                        "Frame LoadCompleted Navigator = System.Windows.Controls.Frame: ImagePage_Loose.xaml"
                };

            _windowTitleFrame
                = new String[]{
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage",
                        "SingleFrameTestPage"
                };

            _backButtonEnabledFrame
                = new bool[]{
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
                        true
                };

            _forwardButtonEnabledFrame
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
                        true,
                        false
                };

            _backStackFrame = new String[11][];
            _backStackFrame[0] = new String[0];
            _backStackFrame[1] = new String[0];
            _backStackFrame[2] = new String[0];
            _backStackFrame[3] = new String[0];
            _backStackFrame[4] = new String[0];
            _backStackFrame[5] = new String[] { "Anchored Page" };
            _backStackFrame[6] = new String[] { "Anchored Page" };
            _backStackFrame[7] = new String[] { "Anchored Page" };
            _backStackFrame[8] = new String[] { "Anchored Page" };
            _backStackFrame[9] = new String[0];
            _backStackFrame[10] = new String[] { "Anchored Page" };

            _forwardStackFrame = new String[11][];
            _forwardStackFrame[0] = new String[0];
            _forwardStackFrame[1] = new String[0];
            _forwardStackFrame[2] = new String[0];
            _forwardStackFrame[3] = new String[0];
            _forwardStackFrame[4] = new String[0];
            _forwardStackFrame[5] = new String[0];
            _forwardStackFrame[6] = new String[0];
            _forwardStackFrame[7] = new String[0];
            _forwardStackFrame[8] = new String[0];
            _forwardStackFrame[9] = new String[] { "ImagePage" };
            _forwardStackFrame[10] = new String[0];

        }
    }
}

