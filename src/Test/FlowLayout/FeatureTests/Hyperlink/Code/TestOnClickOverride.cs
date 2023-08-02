// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description : Tests the ability to override protected virtual OnClick()
//               
//              By overriding the OnClick method, this test changes the Text of the Hyperlink when 
//              the link is clicked (instead of navigating).
//                                   
// Verification: Basic API validation.  Visual verification is not used.
// Created by:  Microsoft
////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
using System;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>    
    /// Testing OnClick override.   
    /// </summary>
    [Test(2, "Hyperlink", "OnClickOverride", MethodName="Run")]
    public class TestOnClickOverride : AvalonTest
    {       
        private NavigationWindow _navWin;
        private string _navFile = "SimpleNavigation.xaml";
        private string _newHyperText = "Changed the Hyperlink text";
        private MyHyperlink _hl;
        private Canvas _eRoot;
        
        public TestOnClickOverride()
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
        /// <returns>TestResult</returns>
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

            _hl = new MyHyperlink();
            _hl.NavigateUri = new Uri(uri);
            _hl.FontSize = 20;
            _hl.FontFamily = new FontFamily("Tahoma");
            _hl.Inlines.Clear();
            _hl.Inlines.Add(new Run("This is the content.  It needs to be long enough so that it wraps to more than one line."));
            _hl.RequestNavigate += new RequestNavigateEventHandler(hl_RequestNavigate);

            para.Inlines.Add(_hl);
            _eRoot.Children.Add(tp);

            _navWin.Content = _eRoot;
            _navWin.Top = 0;
            _navWin.Left = 0;
            _navWin.Width = 800;
            _navWin.Height = 600;
            _navWin.Topmost = true;
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
            LogComment("This event should not be hit b/c navigation should not happen!!");
            Log.Result = TestResult.Fail;
        }

        /// <summary>
        /// RunTests: Runs a set of tests based on the input variation.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTests()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            LogComment("Content rendered");

            //Move the mouse to where we will click on the link.
            UserInput.MouseButton(_eRoot, 50, 130, "Move");
            //pause for 2 seconds and then click on link.  (This makes sure content has rendered first.)
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            ClickBottomOfLink();

            if (Log.Result != TestResult.Fail)
                return TestResult.Pass;
            else
                return TestResult.Fail;
        }

        /// <summary>
        /// VerifyTest: Verifies the test result
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult VerifyTest()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            LogComment("Verify Test...");

            //Verify that the text of the hyperlink has changed (instead of navigating).
            if (((Run)(_hl.Inlines.FirstInline)).Text == _newHyperText && _navWin.Source == null)
            {
                LogComment("Clicking on the Hyperlink changed the Hyperlink.Text instead of navigating (as expected)");
                return TestResult.Pass;
            }
            else
            {
                LogComment("Clicking on the Hyperlink did not change the Hyperlink.Text instead of navigating!!");
                return TestResult.Fail;
            }                     
        }
        
        private void ClickBottomOfLink()
        {
            LogComment("Clicking the bottom of the Hyperlink...");
            Microsoft.Test.Input.Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftDown);
            Microsoft.Test.Input.Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftUp);
            WaitForPriority(DispatcherPriority.ApplicationIdle);
        }
    }

    //Custom Hyperlink class that overrides Hyperlink.OnClick()
    public class MyHyperlink : Hyperlink
    {
        static string s_newHyperText = "Changed the Hyperlink text";

        protected override void OnClick()
        {
            this.Inlines.Clear();
            this.Inlines.Add(new Run(s_newHyperText));
        }
    }
}
