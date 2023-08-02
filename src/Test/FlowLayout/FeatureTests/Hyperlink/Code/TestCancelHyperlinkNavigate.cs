// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description : Test ContentElement derived Hyperlink.
//               
//               Verifies that a Hyperlink navigation can be cancelled.
//                                   
// Verification: Basic API validation.  Visual verification is not used.
// Created by:  Microsoft
////////////////////////////////////////////////////////////////////////////////////////////////////////////// 

using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Navigation;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>  
    /// Testing Cancel hyperlink navigate.   
    /// </summary>
    [Test(2, "Hyperlink", "CancelHyperlinkNavigateTest", MethodName = "Run")]
    public class TestCancelHyperlinkNavigateTest : AvalonTest
    {
        private NavigationWindow _navWin;
        private bool _testRun;
        private bool _resultCancelNav = true;
        private bool _resultRequestNavigateValidation;
        private string _navFile = "SimpleNavigation.xaml";
        private Hyperlink _hl;
        private Canvas _eRoot;

        public TestCancelHyperlinkNavigateTest()
            : base()
        {
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);
            RunSteps += new TestStep(VerifyTest);
        }

        /// <summary>
        /// Initialize: Get the FlowDocument and add it to the FlowDocumentPageViewer and then set the FlowDocumentViewer viewing mode.
        /// </summary>
        /// <returns>TestResult.Pass;</returns>
        private TestResult Initialize()
        {
            _navWin = new NavigationWindow();

            Status("Initialize");

            _eRoot = new Canvas();
            _eRoot.Height = 160;
            _eRoot.Width = 160;
            _eRoot.Background = Brushes.Tan;

            Paragraph para = new Paragraph();

            FlowDocumentScrollViewer tp = new FlowDocumentScrollViewer();
            tp.Document = new FlowDocument(para);
            tp.Height = 160;
            tp.Width = 160;

            string uri = System.IO.Path.Combine(Environment.CurrentDirectory, _navFile);

            _hl = new Hyperlink();
            _hl.NavigateUri = new Uri(uri);
            _hl.FontSize = 20;
            _hl.Inlines.Clear();
            _hl.Inlines.Add(new Run("Click me!"));

            _hl.RequestNavigate += new RequestNavigateEventHandler(hl_RequestNavigate);

            para.Inlines.Add(_hl);
            _eRoot.Children.Add(tp);

            _navWin.Content = _eRoot;
            _navWin.Top = 0;
            _navWin.Left = 0;
            _navWin.Width = 800;
            _navWin.Height = 600;
            _navWin.Show();
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _navWin.Close();
            return TestResult.Pass;
        }

        private void hl_RequestNavigate(object sender, RequestNavigateEventArgs args)
        {
            LogComment("RequestNavigate event hit...");

            //Verify event args here
            if (_hl.NavigateUri.ToString() == args.Uri.ToString())
            {
                _resultRequestNavigateValidation = true;
            }
            else
            {
                LogComment("RequestNavigate event arg validation failed!!");
                LogComment("Args.Uri was: " + args.Uri.ToString() + " should have been: " + _hl.NavigateUri.ToString());
                Log.Result = TestResult.Fail;
            }

            //This should cancel the navigate
            args.Handled = true;

            //pause for 3 seconds and verify navigation does not happen.
            WaitForPriority(DispatcherPriority.ApplicationIdle);
        }

        /// <summary>
        /// RunTests: Runs a set of tests based on the input variation.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTests()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            LogComment("Content rendered");
            if (!_testRun)
            {
                _testRun = true;
                _hl.DoClick();
            }

            return TestResult.Pass;
        }

        /// <summary>
        /// VerifyTest: Verify the test
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult VerifyTest()
        {
            LogComment("Verify Test...");
            LogComment("Checking to see if navigation was cancelled...");
            if (_navWin.Source != null)
            {
                string source = _navWin.Source.ToString();
                string[] file = source.Split('/');
                foreach (string s in file)
                {
                    if (s == _navFile)
                    {
                        _resultCancelNav = false;
                        LogComment("Navigate failed to cancel!!");
                        return TestResult.Fail;
                    }
                }
            }

            if (_resultCancelNav && _resultRequestNavigateValidation)
            {
                LogComment("Navigation has been cancelled!  Test has passed!");
                LogComment("Cancel Hyperlink Navigate Test passed!");
                return TestResult.Pass;
            }
            else
            {
                LogComment("Test has failed!!");
                LogComment("Cancel Hyperlink Navigate Test failed");
                return TestResult.Fail;
            }
        }
    }
}
