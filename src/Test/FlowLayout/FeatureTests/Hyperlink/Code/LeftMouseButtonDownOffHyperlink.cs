// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description : Tests doing a MouseButtonDown() OFF a Hyperlink and then doing a MouseButtonUp() ON the Hyperlink.
//               Navigation should not happen.               
//                                                
// Verification: Basic API validation.  Visual verification is not used.
// Created by:  Microsof
////////////////////////////////////////////////////////////////////////////////////////////////////////////// 

using System;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Input;
using Microsoft.Test.Layout;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Testing Left mouse button down off hyperlink.  
    /// </summary>
    [Test(2, "Hyperlink", "LeftMouseButtonDownOffHyperlink", MethodName = "Run")]
    public class TestLeftMouseButtonDownOffHyperlink : AvalonTest
    {
        private NavigationWindow _navWin;
        private string _navFile = "SimpleNavigation.xaml";
        private Hyperlink _hl;
        private Canvas _eRoot;
        private bool _mouseUp;
        private bool _testRun;

        public TestLeftMouseButtonDownOffHyperlink()
            : base()
        {
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
            _hl.FontFamily = new FontFamily("Tahoma");
            _hl.Inlines.Clear();
            _hl.Inlines.Add(new Run("This is the content.  It needs to be long enough so that it wraps to more than one line."));
            _hl.RequestNavigate += new RequestNavigateEventHandler(hl_RequestNavigate);
            _hl.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(hl_MouseLeftButtonDown);
            _hl.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(hl_MouseLeftButtonUp);

            para.Inlines.Add(_hl);
            _eRoot.Children.Add(tp);

            _navWin.Content = _eRoot;
            _navWin.Top = 0;
            _navWin.Left = 0;
            _navWin.Width = 600;
            _navWin.Height = 600;

            _navWin.Resources.MergedDictionaries.Add(GenericStyles.LoadAllStyles());

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

        private void hl_MouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            LogComment("MouseLeftButtonDown event hit...");
            LogComment("This event should not be hit b/c left mouse button down was done OFF the Hyperlink!!");
            Log.Result = TestResult.Fail;
        }

        private void hl_MouseLeftButtonUp(object sender, MouseButtonEventArgs args)
        {
            LogComment("MouseLeftButtonUp event hit...");
            _mouseUp = true;
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
                UserInput.MouseButton(_eRoot, 50, 220, "Move");
                WaitForPriority(DispatcherPriority.ApplicationIdle);
                MouseDownOff();
            }

            return TestResult.Pass;
        }

        private void MouseDownOff()
        {
            LogComment("MouseLeftButtonDown OFF the Hyperlink...");
            Microsoft.Test.Input.Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftDown);

            // Move Mouse onto Hyperlink
            UserInput.MouseButton(_eRoot, 50, 130, "Move");
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            MouseUpOn();
        }

        private void MouseUpOn()
        {
            LogComment("MouseLeftButtonUp ON of the Hyperlink...");
            Microsoft.Test.Input.Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftUp);

            // pause for 1 seconds and verify test.  (In the event of investigation, this lets the user visually verify that navigation was successfull.)
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            VerifyTest();
        }

        private void VerifyTest()
        {
            // Verify that navigation did not happen).
            if (_navWin.Source == null && _mouseUp)
            {
                LogComment("Navigation did not happen (as expected)");
                Log.Result = TestResult.Pass;
            }
            else
            {
                LogComment("MouseUp on Hyperlink did not happen or we navigated. (should not have)");
                Log.Result = TestResult.Fail;
            }
        }
    }
}
