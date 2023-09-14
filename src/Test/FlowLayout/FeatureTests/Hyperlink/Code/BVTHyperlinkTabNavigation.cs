// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description : Test Tab Navigation of Hyperlink.
//
//						1.  Bring focus to the first Hyperlink
//						2.  Hit Tab 3 times.
//						3.  Hit Enter key				  
//                                   
// Verification: Basic API validation.  Visual verification is not used.  We should navigate to the NavigateUri of the third Hyperlink
// Created by:  Microsoft
////////////////////////////////////////////////////////////////////////////////////////////////////////////// 

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Navigation;

using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using MTI = Microsoft.Test.Input;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>  
    /// Testing tab navigation of the hyperlink.    
    /// </summary>
    [Test(0, "Hyperlink", "BVTHyperlinkTabNavigationTest", MethodName = "Run")]
    public class BVTHyperlinkTabNavigationTest : AvalonTest
    {
        #region Test case members

        private NavigationWindow _navWin;        
        private string _navFile = "BVT_HyperlinkTabNavigation_Correct.xaml";
        private string _wrongNavFile = "BVT_HyperlinkTabNavigation_Wrong.xaml";       
        private Hyperlink _hl1;
        private Canvas _eRoot;

        #endregion

        #region Constructor

        public BVTHyperlinkTabNavigationTest()
            : base()
        {
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTabTest);
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
            Status("Initialize");
            _navWin = new NavigationWindow();

            _eRoot = new Canvas();
            _eRoot.Height = 400;
            _eRoot.Width = 400;
            _eRoot.Background = Brushes.Tan;

            Paragraph para = new Paragraph();

            FlowDocumentScrollViewer tf = new FlowDocumentScrollViewer();
            tf.Document = new FlowDocument(para);
            tf.Height = 400;
            tf.Width = 400;

            string uriCorrect = System.IO.Path.Combine(Environment.CurrentDirectory, _navFile);
            string uriWrong = System.IO.Path.Combine(Environment.CurrentDirectory, _wrongNavFile);

            LineBreak lb = new LineBreak();

            _hl1 = new Hyperlink();
            _hl1.NavigateUri = new Uri(uriWrong);
            _hl1.FontSize = 10;
            _hl1.Inlines.Clear();
            _hl1.Inlines.Add(new Run("Hyperlink1"));
            _hl1.Focusable = true;

            Hyperlink hl2 = new Hyperlink();
            hl2.NavigateUri = new Uri(uriWrong);
            hl2.FontSize = 15;
            hl2.Inlines.Clear();
            hl2.Inlines.Add(new Run("Hyperlink2"));
            hl2.Focusable = true;

            Hyperlink hl3 = new Hyperlink();
            hl3.NavigateUri = new Uri(uriCorrect);
            hl3.FontSize = 20;
            hl3.Inlines.Clear();
            hl3.Inlines.Add(new Run("Hyperlink3"));
            hl3.Focusable = true;

            Button btn = new Button();
            btn.Content = "Button";
            btn.Click += new RoutedEventHandler(btn_Click);

            //events
            _hl1.RequestNavigate += new RequestNavigateEventHandler(hl1_RequestNavigate);
            hl2.RequestNavigate += new RequestNavigateEventHandler(hl2_RequestNavigate);
            hl3.RequestNavigate += new RequestNavigateEventHandler(hl3_RequestNavigate);

            para.Inlines.Add(new Span(_hl1));
            para.Inlines.Add(new Span(lb));
            para.Inlines.Add(new Span(hl2));
            para.Inlines.Add(new Span(lb));
            para.Inlines.Add(new InlineUIContainer(btn));
            para.Inlines.Add(new Span(lb));
            para.Inlines.Add(new Span(hl3));

            _eRoot.Children.Add(tf);

            _navWin.Content = _eRoot;
            _navWin.Top = 0;
            _navWin.Left = 0;
            _navWin.Width = 800;
            _navWin.Height = 600;
            _navWin.Show();

            //When running under Test Center, need to bring focus to the test Window (from the console window of the test harness)
            MTI.Input.MoveToAndClick(_navWin);
            WaitForPriority(DispatcherPriority.ApplicationIdle);
           
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _navWin.Close();
            return TestResult.Pass;
        }
       
        private TestResult RunTabTest()
        {
            //Bring focus to the Hyperlink 
            _hl1.Focus();
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            System.Windows.Input.Key tabKey = System.Windows.Input.Key.Tab;
            System.Windows.Input.Key enterKey = System.Windows.Input.Key.Enter;

            PressKey(tabKey);
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            PressKey(tabKey);
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            PressKey(tabKey);
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            PressKey(enterKey);
            WaitForPriority(DispatcherPriority.ApplicationIdle);
           
            return TestResult.Pass;
        }

        private TestResult VerifyTest()
        {
            Status("Verify Test");

            string source;
            if (_navWin.Source != null)
            {
                source = _navWin.Source.ToString();
            }
            else
            {
                WaitFor(2000);
                source = _navWin.Source.ToString();
            }

            bool testResult = false;
            
            string[] file = source.Split('/');
            foreach (string s in file)
            {
                if (s == _navFile)
                {
                    testResult = true;
                }
            }

            if (testResult)
            {
                LogComment("Navigation has succeeded!  Test has passed!");
                return TestResult.Pass;
            }
            else
            {
                TestLog.Current.LogEvidence("Test has failed!!");
                TestLog.Current.LogEvidence("NavigationWindow.Source after navigation: " + _navWin.Source.ToString());
                TestLog.Current.LogEvidence("Expected: " + _navFile);
                return TestResult.Fail;
            }
        }
        #endregion

        private void hl1_RequestNavigate(object sender, RequestNavigateEventArgs args)
        {
            Status("RequestNavigate event hit for Hyperlink1 (Wrong!!)...");
        }

        private void hl2_RequestNavigate(object sender, RequestNavigateEventArgs args)
        {
            Status("RequestNavigate event hit for Hyperlink2 (Wrong!!)...");
        }

        private void btn_Click(object sender, RoutedEventArgs args)
        {
            Status("Button Click event hit for Button (Wrong!!)...");
        }

        private void hl3_RequestNavigate(object sender, RequestNavigateEventArgs args)
        {
            Status("RequestNavigate event hit for Hyperlink4 (Correct!!)...");
        }

        private void PressKey(System.Windows.Input.Key key)
        {
            LogComment("Pressing: " + key.ToString());
            MTI.Input.SendKeyboardInput(key, true);  //down
            MTI.Input.SendKeyboardInput(key, false); //up
        }
    }
}
