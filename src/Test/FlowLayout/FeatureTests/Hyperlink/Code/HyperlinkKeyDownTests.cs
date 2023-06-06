// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description : Test various KeyDown elements of ContentElement derived Hyperlink.
//
//               Tests:
//                      1.  "NavigateEvents": Verifies typical Method/Event sequence in a simple Hyperlink navigation (Enter key pressed when HL has focus).
//                      2.  "NotEnterKey": Verifies that if a key other than "Enter" is pressed when Hyperlink has focus, Navigation does not happen.
//                                                  
// Verification: Basic API validation.
// Created by:  Microsoft
////////////////////////////////////////////////////////////////////////////////////////////////////////////// 

using System;
using System.Collections;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Input;

using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using MTI = Microsoft.Test.Input;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>  
    /// Testing Hyperlink keydown.   
    /// </summary>
    [Test(2, "Hyperlink", "HyperlinkKeyDown", MethodName = "Run")]
    public class TestHyperlinkKeyDown : AvalonTest
    {
        private NavigationWindow _navWin;
        private string _inputString;
        private string _navFile = "SimpleNavigation.xaml";
        private ArrayList _navEventSequence;
        private ArrayList _expectedEventSequence;
        private bool _notEnterKey;
        private bool _navEvents;
        private Hyperlink _hl;

        [Variation("EnterKey")]
        [Variation("SpaceKey")]
        public TestHyperlinkKeyDown(string testString)
            : base()
        {
            _inputString = testString;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);
        }

        /// <summary>
        /// Initialize: Get the FlowDocument and add it to the FlowDocumentPageViewer and then set the FlowDocumentViewer viewing mode.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {
            Status("Initialize");
            FindTestToRun(_inputString);

            _navWin = new NavigationWindow();
            //Need to move the navWin.Show up here because the click below was hitting the Hyperlink and essentially starting the test off too soon.
            _navWin.Show();

            //When running under Test Center, need to bring focus to the test Window (from the console window of the test harness)
            MTI.Input.MoveToAndClick(_navWin);
            //Then move the mouse out of the way.
            MTI.Input.MoveTo(new Point(0, 0));

            Canvas eRoot = new Canvas();
            eRoot.Height = 160;
            eRoot.Width = 160;
            eRoot.Background = Brushes.Tan;

            Paragraph para = new Paragraph();

            FlowDocumentScrollViewer tp = new FlowDocumentScrollViewer();
            tp.Document = new FlowDocument(para);
            tp.Height = 160;
            tp.Width = 160;

            string uri = System.IO.Path.Combine(Environment.CurrentDirectory, _navFile);

            _hl = new Hyperlink(new Span(new Run("This is the content.  It needs to be long enough so that it wraps to more than one line.")));
            _hl.NavigateUri = new Uri(uri);
            _hl.FontSize = 20;
            _hl.Focusable = true;

            //events
            _hl.RequestNavigate += new RequestNavigateEventHandler(hl_RequestNavigate);
            _hl.KeyDown += new KeyEventHandler(hl_KeyDown);
            _hl.KeyUp += new KeyEventHandler(hl_KeyUp);

            para.Inlines.Add(_hl);
            eRoot.Children.Add(tp);

            _navWin.Content = eRoot;
            _navWin.Top = 0;
            _navWin.Left = 0;
            _navWin.Width = 800;
            _navWin.Height = 600;

            _navEventSequence = new ArrayList();
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _navWin.Close();
            return TestResult.Pass;
        }

        private void hl_KeyDown(object sender, KeyEventArgs args)
        {
            LogComment("KeyDown event hit");
            _navEventSequence.Add("KeyDown");
        }

        private void hl_KeyUp(object sender, KeyEventArgs args)
        {
            LogComment("KeyUp event hit");
            _navEventSequence.Add("KeyUp");
        }

        private void hl_RequestNavigate(object sender, RequestNavigateEventArgs args)
        {
            LogComment("RequestNavigate event hit");
            _navEventSequence.Add("RequestNavigate");

            //Verify event args here
            if (_hl.NavigateUri.ToString() != args.Uri.ToString())
            {
                Log.Result = TestResult.Fail;
                Log.LogEvidence("RequestNavigate event arg validation failed!!");
                Log.LogEvidence("Args.Uri was: " + args.Uri.ToString() + " should have been: " + _hl.NavigateUri.ToString());
            }

            if (_notEnterKey)
            {
                Log.Result = TestResult.Fail;
                Log.LogEvidence("FAIL!! Should not have gotten a RequestNavigate event!!");
            }
        }

        /// <summary>
        /// RunTests: Runs a set of tests based on the input variation.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTests()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            LogComment("Content rendered");

            //Bring focus to the Hyperlink 
            _hl.Focus();
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            if (_navEvents)
            {
                PressKey(System.Windows.Input.Key.Enter);
            }
            else if (_notEnterKey)
            {
                PressKey(System.Windows.Input.Key.Space);
            }

            VerifyTest();

            return TestResult.Pass;
        }

        private void VerifyTest()
        {
            WaitFor(500);
            Status("Verify Test");

            //Make sure we have the same # of events that we expected.
            if (_navEventSequence.Count == _expectedEventSequence.Count)
            {
                //Iterate through the ArrayLists and make sure the sequence is correct 
                for (int i = 0; i < _navEventSequence.Count; i++)
                {
                    if (_navEventSequence[i] != _expectedEventSequence[i])
                    {
                        Log.Result = TestResult.Fail;
                    }
                }
            }
            else
            {
                Log.Result = TestResult.Fail;
            }

            //Spit out expected sequence if we fail
            if (Log.Result == TestResult.Fail)
            {
                PrintSequenceData();
            }

            if (_notEnterKey)
            {
                if (_navWin.Source != null)
                {
                    Log.Result = TestResult.Fail;
                    LogComment("Navigation should not have happened!!");
                }
            }
        }

        private void PressKey(System.Windows.Input.Key key)
        {
            LogComment("Pressing:" + key.ToString());
            MTI.Input.SendKeyboardInput(key, true);
            MTI.Input.SendKeyboardInput(key, false);
        }

        private void FindTestToRun(string param)
        {
            _expectedEventSequence = new ArrayList();

            switch (param.ToLowerInvariant())
            {
                case "enterkey":
                    _navEvents = true;
                    _expectedEventSequence.Add("RequestNavigate");
                    break;
                case "spacekey":
                    _notEnterKey = true;
                    _expectedEventSequence.Add("KeyDown");
                    break;
                default:
                    Log.LogEvidence("Invalid entry");
                    break;
            }

            _expectedEventSequence.Add("KeyUp");
        }

        private void PrintSequenceData()
        {
            Log.LogEvidence("There was a problem comparing expected to actual event sequence!");
            Log.LogEvidence("Expected Sequence:");
            foreach (object eventSeq in _expectedEventSequence)
            {
                Log.LogEvidence(eventSeq.ToString());
            }

            Log.LogEvidence("Actual Sequence:");
            foreach (object eventSeq in _navEventSequence)
            {
                Log.LogEvidence(eventSeq.ToString());
            }
        }
    }
}
