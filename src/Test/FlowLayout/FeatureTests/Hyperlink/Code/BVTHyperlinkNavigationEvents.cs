// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Navigation;

using Microsoft.Test.Input;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Testing hyperlink navigation events    
    /// </summary>
    [Test(0, "Hyperlink", "HyperlinkNavigationEventsTest", MethodName = "Run")]
    public class HyperlinkNavigationEventsTest : AvalonTest
    {
        #region Test case members

        private NavigationWindow _navWin;      
        private ArrayList _navEventSequence;
        private Hyperlink _hl;
        private Canvas _eRoot;
        private string _test;
        private bool _flowContent;

        #endregion

        #region Constructor

        [Variation("EventSequence", true)]
        [Variation("RemoveEvents", true)]
        [Variation("EventSequence", false)]
        [Variation("RemoveEvents", false)]
        public HyperlinkNavigationEventsTest(string test, bool flowContent)
            : base()
        {
            this._test = test;
            this._flowContent = flowContent;

            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(ClickLink);
            if (test == "RemoveEvents")
            {
                RunSteps += new TestStep(RemoveEventHandlers);
                RunSteps += new TestStep(ClickLink);               
            }            
            RunSteps += new TestStep(VerifyTest);
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initialize: Get the FlowDocument and add it to the FlowDocumentPageViewer and then set the FlowDocumentViewer viewing mode.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {
            _navEventSequence = new ArrayList();            
                                    
            _eRoot = new Canvas();
            _eRoot.Height = 300;
            _eRoot.Width = 500;            
            
            _hl = new Hyperlink();
            string uri = System.IO.Path.Combine(Environment.CurrentDirectory, "SimpleNavigation.xaml");
            _hl.NavigateUri = new Uri(uri);
            _hl.FontSize = 20;
            _hl.Inlines.Clear();
            _hl.Inlines.Add(new Run("This is the content for the Hyperlink.  It needs to be long enough so that it wraps to more than one line."));

            //events            
            _hl.Click += new RoutedEventHandler(hl_Click);
            _hl.RequestNavigate += new RequestNavigateEventHandler(hl_RequestNavigate);
            _hl.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(hl_PreviewMouseLeftButtonDown);
            _hl.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(hl_PreviewMouseLeftButtonUp);
            _hl.AddHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(hl_MouseLeftButtonDown), true);
            _hl.AddHandler(Mouse.MouseUpEvent, new MouseButtonEventHandler(hl_MouseLeftButtonUp), true);
                        
            if(_flowContent)
            {
                Paragraph para = new Paragraph();
                para.Inlines.Add(_hl);
                FlowDocument fd = new FlowDocument(para);
                fd.PagePadding = new Thickness(0);
                FlowDocumentReader fdr = new FlowDocumentReader();
                fdr.Width = 500;
                fdr.Height = 300;
                fdr.Document = fd;
                _eRoot.Children.Add(fdr);
            }
            else
            {
                TextBlock tb = new TextBlock(_hl);
                tb.TextWrapping = TextWrapping.Wrap;
                tb.Width = 500;
                _eRoot.Children.Add(tb);
            }

            _navWin = new NavigationWindow();
            _navWin.Content = _eRoot;
            _navWin.Top = 0;
            _navWin.Left = 0;
            _navWin.Width = 500;
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

        private TestResult ClickLink()
        {
            //Move the mouse to where we will click on the link.
            WaitFor(1000);
            UserInput.MouseButton(_eRoot, 50, 30, "Move");
            
            if (!_hl.IsMouseOver)
            {
                //Wait and try to move the mouse over the hyperlink again.
                WaitFor(1500);
                UserInput.MouseButton(_eRoot, 50, 30, "Move");
                WaitForPriority(DispatcherPriority.ApplicationIdle);
            }
            
            if (_hl.IsMouseOver)
            {
                LogComment("Hyperlink.IsMouseOver is true, clicking the Hyperlink...");
                Microsoft.Test.Input.Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftDown);
                Microsoft.Test.Input.Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftUp);
                WaitForPriority(DispatcherPriority.ApplicationIdle);
            }
            else //ok, something is wrong
            {
                TestLog.Current.LogEvidence("Hyperlink.IsMouseOver is false. Test Fails.");
                TestLog.Current.Result = TestResult.Fail;
            }
            return TestResult.Pass;
        }

        /// <summary>
        /// RemoveEvents: Remove event handlers from the Hyperlink
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RemoveEventHandlers()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            Status("Removing event handlers...");

            _hl.Click -= new RoutedEventHandler(hl_Click);
            _hl.RequestNavigate -= new RequestNavigateEventHandler(hl_RequestNavigate);
            _hl.RemoveHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(hl_MouseLeftButtonDown));
            _hl.RemoveHandler(Mouse.MouseUpEvent, new MouseButtonEventHandler(hl_MouseLeftButtonUp));
            _hl.PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(hl_PreviewMouseLeftButtonDown);
            _hl.PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(hl_PreviewMouseLeftButtonUp);

            //reset event tracker
            _navEventSequence.Clear();

            // Wait a little for Navigaiton to complete.
            WaitFor(500);

            //Go back to the previous page
            if (NavigationCommands.BrowseBack.CanExecute(null, _navWin))
            {
                Status("Navigating back to the previous page...");
                NavigationCommands.BrowseBack.Execute(null, _navWin);
                WaitForPriority(DispatcherPriority.ApplicationIdle);
            }
            else
            {
                TestLog.Current.LogEvidence("Something has gone wrong.  Should have been able to execute a BrowseBack command!");
                TestLog.Current.Result = TestResult.Fail;
            }

            return TestResult.Pass;
        }
        
        /// <summary>
        /// VerifyTest: Verifies the test
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult VerifyTest()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            LogComment("Verify Test...");

            ArrayList expectedEventSequence = new ArrayList();

            if (_test == "EventSequence")
            {
                expectedEventSequence.Add("PreviewMouseLeftButtonDown");
                expectedEventSequence.Add("MouseLeftButtonDown");
                expectedEventSequence.Add("PreviewMouseLeftButtonUp");
                expectedEventSequence.Add("RequestNavigate");
                expectedEventSequence.Add("Click");
                expectedEventSequence.Add("MouseLeftButtonUp");
            }

            bool resultEventSequence = true;

            //Make event count is expected.
            if (_navEventSequence.Count == expectedEventSequence.Count)
            {
                //Iterate through the ArrayLists and make sure the sequence is correct 
                for (int i = 0; i < _navEventSequence.Count; i++)
                {
                    if (_navEventSequence[i] != expectedEventSequence[i])
                    {
                        resultEventSequence = false;                        
                    }
                }
            }
            else
            {               
                resultEventSequence = false;
            }

            if (!resultEventSequence)
            {
                TestLog.Current.Result = TestResult.Fail;

                TestLog.Current.LogEvidence("Some problem with the expected events. The entire sequence of events should have been:");
                foreach (object expected in expectedEventSequence)
                {
                    TestLog.Current.LogEvidence("" + expected.ToString());
                }

                TestLog.Current.LogEvidence("The sequence returned was:");
                foreach (object actual in _navEventSequence)
                {
                    TestLog.Current.LogEvidence("" + actual.ToString());
                }
            }
            return TestResult.Pass;
        }
                
        #endregion

        #region Events

        private void hl_Click(object sender, RoutedEventArgs args)
        {
            LogComment("Click event hit...");
            _navEventSequence.Add("Click");
        }

        private void hl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            LogComment("PreviewMouseLeftButtonDown event hit...");
            _navEventSequence.Add("PreviewMouseLeftButtonDown");
        }

        private void hl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs args)
        {
            LogComment("PreviewMouseLeftButtonUp event hit...");
            _navEventSequence.Add("PreviewMouseLeftButtonUp");
        }

        private void hl_MouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            LogComment("MouseLeftButtonDown event hit...");
            _navEventSequence.Add("MouseLeftButtonDown");

            if (args.ChangedButton != MouseButton.Left || args.LeftButton != MouseButtonState.Pressed)
            {
                TestLog.Current.Result = TestResult.Fail;
                TestLog.Current.LogEvidence("Unexpected MouseButtonDown state.");
                TestLog.Current.LogEvidence("Expected ChangedButton: MouseButton.Left Actual: " + args.ChangedButton.ToString());
                TestLog.Current.LogEvidence("Expected LeftButton: MouseButtonState.Pressed Actual: " + args.LeftButton.ToString());
            }
        }

        private void hl_MouseLeftButtonUp(object sender, MouseButtonEventArgs args)
        {
            LogComment("MouseLeftButtonUp event hit...");
            _navEventSequence.Add("MouseLeftButtonUp");

            if (args.ChangedButton != MouseButton.Left || args.LeftButton != MouseButtonState.Released)
            {
                TestLog.Current.Result = TestResult.Fail;
                TestLog.Current.LogEvidence("Unexpected MouseButtonUp state.");
                TestLog.Current.LogEvidence("Expected ChangedButton: MouseButton.Left Actual: " + args.ChangedButton.ToString());
                TestLog.Current.LogEvidence("Expected LeftButton: MouseButtonState.Released Actual: " + args.LeftButton.ToString());
            }
        }

        private void hl_RequestNavigate(object sender, RequestNavigateEventArgs args)
        {
            LogComment("RequestNavigate event hit...");
            _navEventSequence.Add("RequestNavigate");

            if (_hl.NavigateUri.ToString() != args.Uri.ToString())
            {
                TestLog.Current.Result = TestResult.Fail;
                TestLog.Current.LogEvidence("Unexpected RequestNavigate state.");
                LogComment("Expected Args.Uri: " + _hl.NavigateUri.ToString() + " Actual: " + args.Uri.ToString());
            }
        }

        #endregion
    }
}
