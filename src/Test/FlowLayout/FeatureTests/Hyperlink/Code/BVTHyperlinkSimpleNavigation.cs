// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description : Test ContentElement derived Hyperlink simple navigation.
//               
//               1.  Create a NavigationWindow w/ a multi-line Hyperlink.
//               2.  Click on the Hyperlink.
//                                   
// Verification: Basic API validation.  Verify that the NavigationWindow.Source is the expected destination xaml.
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
using Microsoft.Test.Input;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>  
    /// Testing simple navigation.    
    /// </summary>
    [Test(0, "Hyperlink", "BVTHyperlinkSimpleNavigationTest", MethodName = "Run")]
    public class BVTHyperlinkSimpleNavigationTest : AvalonTest
    {       
        private NavigationWindow _navWin;
        private Hyperlink _hl;
        private Canvas _eRoot;
        private bool _testRun;
        private bool _result;
        private string _navFile = "SimpleNavigation.xaml";

        public BVTHyperlinkSimpleNavigationTest()
            : base()
        {
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);
            RunSteps += new TestStep(RunTests);
        }
       
        /// <summary>
        /// Initialize: Get the FlowDocument and add it to the FlowDocumentPageViewer and then set the FlowDocumentViewer viewing mode.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {
            Status("Initialize");

            _navWin = new NavigationWindow();
            _eRoot = new Canvas();
            _eRoot.Height = 160;
            _eRoot.Width = 160;
            _eRoot.Background = Brushes.Tan;

            Paragraph para = new Paragraph();

            FlowDocumentScrollViewer tp = new FlowDocumentScrollViewer();
            tp.Document = new FlowDocument(para);

            tp.Height = 160;
            tp.Width = 160;

            _hl = new Hyperlink();
            string uri = System.IO.Path.Combine(Environment.CurrentDirectory, _navFile);
            _hl.NavigateUri = new Uri(uri);
            _hl.FontSize = 20;
            _hl.FontFamily = new FontFamily("Tahoma");
            _hl.Inlines.Clear();
            _hl.Inlines.Add(new Run("This is the content.  It needs to be long enough so that it wraps to more than one line."));

            para.Inlines.Add(_hl);

            _eRoot.Children.Add(tp);

            _navWin.Content = _eRoot;
            _navWin.Top = 0;
            _navWin.Left = 0;
            _navWin.Width = 300;
            _navWin.Height = 300;
            _navWin.Show();
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _navWin.Close();
            return TestResult.Pass;
        }

        /// <summary>
        /// RunTests: Runs a set of tests based on the input variation.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTests()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            if (!_testRun)
            {
                //Move the mouse to where we will click on the link.
                UserInput.MouseButton(_eRoot, 50, 130, "Move");
                //pause for 2 seconds and then click on link.  (This makes sure content has rendered first.)
                WaitForPriority(DispatcherPriority.ApplicationIdle);
                return ClickBottomOfLink();
            }
            else
            {
                //pause for 1 seconds and verify test.  (In the event of investigation, this lets the user visually verify that navigation was successfull.)
                WaitForPriority(DispatcherPriority.ApplicationIdle);
                return Verification();
            }
        }
      
        private TestResult Verification()
        {
            Status("Checking to see if navigation was successful...");
            string source = _navWin.Source.ToString();
            string[] file = source.Split('/');
            foreach (string s in file)
            {
                if (s == _navFile)
                    _result = true;
            }

            if (_result)
            {
                LogComment("Navigation has succeeded!  Test has passed!");
                return TestResult.Pass;
            }
            else
            {
                LogComment("Test has failed!!");
                LogComment("NavigationWindow.Source after navigation = " + source);
                return TestResult.Fail;
            }
        }

        private TestResult ClickBottomOfLink()
        {
            if (!_hl.IsMouseOver)
            {
                //pause for 2 seconds and then click on link.  (This makes sure content has rendered first.)
                //Try to ove the mouse to where we will click on the link again.
                WaitFor(2000);                
                UserInput.MouseButton(_eRoot, 50, 130, "Move");
                WaitForPriority(DispatcherPriority.ApplicationIdle);               
            }
            
            if (_hl.IsMouseOver)
            {
                Status("Hyperlink.IsMouseOver is true, go ahead and click Hyperlink.");

                //Need to put this check in b/c of a bug in CommonFunctionality.DelayedBeginInvoke
                if (!_testRun)
                {
                    Status("Clicking the bottom of the Hyperlink...");
                    Microsoft.Test.Input.Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftDown);
                    Microsoft.Test.Input.Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftUp);
                }
                _testRun = true;
                return TestResult.Pass;
            }
            else
            {
                LogComment("Hyperlink.IsMouseOver is false (Is mouse over the Hyperlink?)  Test Fails.");
                return TestResult.Fail;
            }
        }
    }
}
