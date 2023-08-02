// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Reflection;

namespace DRT
{
    public class DrtHyperlinkTestSuite : DrtTestSuite
    {
        public DrtHyperlinkTestSuite() : base("Hyperlink")
        {
            Contact = "Microsoft";
        }

        /// <summary>
        /// Defining Test Steps
        /// </summary>
        enum TestSteps
        {
            Start, 
            HyperLink_ContentStyleTest,
            HyperLink_NavigationTest,
            HyperLink_EnterKeyTest,
            End,
        }

        public override DrtTest[] PrepareTests()
        {
            _step = TestSteps.Start;

            _mainTestWindow = new NavigationWindow();
            _mainTestWindow.Title = "DrtHyperlink";
            _mainTestWindow.Show();

            return new DrtTest[]
            {
                new DrtTest(MainTestLoop),
                new DrtTest(Cleanup),
            };
        }

        private void Cleanup()
        {
        }

        //
        // Test resource
        //
        Hyperlink _currentHyperLink = null;
        NavigationWindow _mainTestWindow = null;

        Frame _f1 = null;
        int _waitTime = 60000;
        TestSteps _step;

        private void MainTestLoop()
        {
            Console.WriteLine("Test Step: {0}", _step);

            switch (_step)
            {
                case TestSteps.Start:
                    break;

                case TestSteps.HyperLink_ContentStyleTest:
                    Console.WriteLine("Verify that Hyperlink style expanded correctly");
                    _f1 = new Frame();
                    _mainTestWindow.ContentRendered += new EventHandler(HLTest_f1_ContentRendered);
                    _f1.LoadCompleted += new LoadCompletedEventHandler(HLTest_f1_LoadCompleted);
                    _f1.Source = new Uri(DRT.BaseDirectory + "DRTFiles\\FlowLayout\\HyperlinkPage1.xaml", UriKind.RelativeOrAbsolute);
                    
                    WaitForAsyncOperation();
                    break;

                case TestSteps.HyperLink_NavigationTest:
                    // At this point, _currentHyperLink should be 'HL1' in Page1.xaml
                    // Make sure that we're at the right state
                    if ((_currentHyperLink == null) ||
                        (_currentHyperLink.Name != "HL1"))
                    {
                        DRT.Fail("_currentHyperLink is invalid");
                    }

                    _mainTestWindow.ContentRendered -= new EventHandler(HLTest_f1_ContentRendered);
                    _f1.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(HLNavTestPage2LoadCompleted);
                    Console.WriteLine("Navigate to HyperlinkPage2.xaml");
                    ClickHyperLink(_currentHyperLink);

                    WaitForAsyncOperation();
                    break;

                case TestSteps.HyperLink_EnterKeyTest:
                    // At this point, _currentHyperLink should be 'HL2' in HyperlinkPage2.xaml
                    // Make sure that we're at the right state
                    if ((_currentHyperLink == null) ||
                        (_currentHyperLink.Name != "HL2"))
                    {
                        DRT.Fail("_currentHyperLink is invalid. It should be 'HL2' in HyperlinkPage2.xaml");
                    }

                    _f1.LoadCompleted -= new System.Windows.Navigation.LoadCompletedEventHandler(HLNavTestPage2LoadCompleted);
                    _f1.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(HLNavTestPage3LoadCompleted);
                    Console.WriteLine("Navigate to Page3.xaml");
                    KeyDown(_currentHyperLink, Key.Enter);
                    KeyUp(_currentHyperLink, Key.Enter);

                    WaitForAsyncOperation();
                    break;

                case TestSteps.End:
                    _mainTestWindow.Close();
                    break;

                default:
                    DRT.Fail("Unknown test step: {0}. Aborting test...", _step);
                    break;
            }
            
            //
            // If we're not waiting for any anychronous action, just continue with the next step.
            //
            if (IsWaiting)
            {
                WaitStep();
            }
            else
            {
                if (_step != TestSteps.End)
                {
                    _step++;
                    DRT.ResumeAt(new DrtTest(MainTestLoop));
                }
            }
        }

        private void WaitStep()
        {
            // Keep going as long as we're waiting
            if (IsWaiting)
            {
                DRT.Pause(100);
                DRT.ResumeAt(new DrtTest(WaitStep));
            }
        }

        public bool IsWaiting
        {
            get { return _waitOperation != null; }
        }

        DispatcherTimer _waitOperation;

        public void WaitForAsyncOperation()
        {
            if (IsWaiting)
            {
                DRT.Fail("TestManager is currently waiting for step {0} to finish. Cannot wait for another step. Aborting test...", _step);
            }
            else
            {
                Console.WriteLine("Waiting for step {0} to finish...", _step);

                _waitOperation = new DispatcherTimer(DispatcherPriority.Background);
                _waitOperation.Tick += new EventHandler(OnWaitTimeout);
                _waitOperation.Interval = TimeSpan.FromMilliseconds(_waitTime);
                _waitOperation.Start();
            }
        }

        private void EndWait()
        {
            if (_waitOperation != null)
            {
                _waitOperation.Stop();
                _waitOperation = null;

                _step++;
                Console.WriteLine("EndWait: continuing MainTestLoop at step {0}", _step);
                DRT.ResumeAt(new DrtTest(MainTestLoop));
            }
        }

        private void OnWaitTimeout(object arg, EventArgs e)
        {
            DRT.Fail("Wait timeout reached (wait for step {0}).", _step);
        }

        /*
         * Event Handler for HyperLink_StyleTest
         *
         *****
         * Page1.xaml content:
         *****
            <Border
                xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                Background = "VerticalGradient #666699 white">
                <DockPanel>
                    
                    <Border DockPanel.Dock = "top" >
                        <TextBlock
                            FontSize    = "14"
                            FontWeight  = "bold"
                            Foreground  = "white">Navigate to: </TextBlock>
                    </Border>
                    <Border
                        DockPanel.Dock = "top"
                    >
                    <Hyperlink ID="HL1"
                        NavigateUri = "HyperlinkPage2.xaml">
                        <TextBlock>HyperlinkPage2.xaml</TextBlock>
                    </Hyperlink>
                    </Border>
                    <Hyperlink ID="HL_WITH_STRING">string hyperlink</Hyperlink>
                    <Hyperlink ID="HL_WITH_OBJARRAY">string1<TextBlock ID="TEXT_IN_H_W_O">Text1</TextBlock></Hyperlink>
                </DockPanel>
            </Border>
        */
        
        protected void hl_test_f1_LoadCompleted(object o, System.Windows.Navigation.NavigationEventArgs args)
        {
            Border border = ((Frame)o).Content as Border;

            if (border == null)
            {
                DRT.Fail("Failed to parse 'HyperlinkPage1.xaml' (root border is not created).");
            }

            Console.WriteLine("Looking for 'HL1' in HyperlinkPage1.xaml");

            Hyperlink hl = DRT.FindElementByID("HL1", border) as Hyperlink;

            if (hl == null)
            {
                DRT.Fail("HL1 is not found, please check if HyperlinkPage1.xaml is loaded");

            }
            // Regression_Bug12 - Hyperlink content style is not expanded
            Hyperlink hl2 = DRT.FindElementByID("HL_WITH_OBJARRAY", border) as Hyperlink;

            if (hl2 == null)
            {
                Console.WriteLine("Hyperlink (ID: HL_WITH_OBJARRAY) not found!");
                DRT.Fail("This may be related to Regression_Bug12 - Hyperlink content style is not expanded");
            }

            Console.WriteLine("Now, verify that content is generated");

            TextBlock text = DRT.FindElementByID("TEXT_IN_H_W_O", hl2) as TextBlock;

            if (text == null)
            {
                DRT.Fail("TextBlock (ID: TEXT_IN_H_W_O) not found! (Please verify if Regression_Bug12 is regressed)");
            }
            // Continue the next test
            _currentHyperLink = hl;

            EndWaitAndUpdateMainWindow(TestSteps.HyperLink_ContentStyleTest);
        }

        /*
         * Event Handler for HyperLink_NavigationTest
         *
         *****
         * HyperlinkPage2.xaml content:
         *****
            <Border ID="Page2MainBorder" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" >
                <DockPanel>
	                <Hyperlink ID="HL2" NavigateUri="Page3.xaml" Width="100" Height="30"> Page3.Xaml </Hyperlink>
	                <TextBlock ID="TEXT_IN_TEST2" Foreground="Blue">We're looking for this text</TextBlock>
                </DockPanel>
            </Border>
        */
        
        protected void HLNavTestPage2LoadCompleted(object o, System.Windows.Navigation.NavigationEventArgs args)
        {
            Border border = ((Frame)o).Content as Border;
            if ((border == null) || (border.Name != "Page2MainBorder"))
            {
                DRT.Fail("Failed to parse 'HyperlinkPage2.xaml'. Root border (ID='Page2MainBorder')is not created).");
            }

            Console.WriteLine("Looking for 'HL2' in HyperlinkPage2.xaml");

            Hyperlink hl = DRT.FindElementByID("HL2", border) as Hyperlink;

            if (hl == null)
            {
                DRT.Fail("HL2 is not found, please check if HyperlinkPage2.xaml is loaded");
            }

            // Continue the next test
            _currentHyperLink = hl;

            EndWaitAndUpdateMainWindow(TestSteps.HyperLink_NavigationTest);
        }

        /*
         * Event Handler for HyperLink_EnterKeyTest
         *
         *****
         * HyperlinkPage2.xaml content:
         *****
            <Border
                xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                Background = "VerticalGradient #666699 white"
                ID = "Page3MainBorder">
            <DockPanel>
                    <Hyperlink ID="HL3" NavigateUri="test4.xaml">Go To Test4.xaml</Hyperlink>
	            <Image ID="IMAGE_IN_TEST3" Source="tulip.jpg" />
            </DockPanel>
            </Border>
        */
        
        protected void HLNavTestPage3LoadCompleted(object o, System.Windows.Navigation.NavigationEventArgs args)
        {
            Border border = ((Frame)o).Content as Border;
            if ((border == null) || (border.Name != "Page3MainBorder"))
            {
                DRT.Fail("Failed to parse 'Page3.xaml'. Root border (ID='Page3MainBorder')is not created).");
            }

            Console.WriteLine("Looking for 'HL3' in page3.xaml");

            Hyperlink hl = DRT.FindElementByID("HL3", border) as Hyperlink;

            if (hl == null)
            {
                DRT.Fail("HL3 is not found, please check if Page3.xaml is loaded");
            }

            // Continue the next test
            _currentHyperLink = hl;

            EndWaitAndUpdateMainWindow(TestSteps.HyperLink_EnterKeyTest);
        }
        
        private void SimulateMouseDownEvent(FrameworkElement elem)
        {
            MouseDevice md = System.Windows.Input.InputManager.Current.PrimaryMouseDevice;
            MouseButtonEventArgs mbea = new MouseButtonEventArgs(md, Environment.TickCount, MouseButton.Left);

            mbea.RoutedEvent=System.Windows.Input.Mouse.MouseUpEvent;
            elem.RaiseEvent(mbea);
        }

        void EndWaitAndUpdateMainWindow(TestSteps currentStep)
        {
            _mainTestWindow.InvalidateMeasure();
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(PostEndWaitProc),
                null
                );
        }

        protected object PostEndWaitProc(object obj)
        {
            EndWait();
            return null;
        }

        private void HLTest_f1_LoadCompleted(object sender, NavigationEventArgs args)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Input,
                       (DispatcherOperationCallback) delegate (object unused)
                       {
                           _mainTestWindow.Content = _f1;
                           return null;
                       },
                       this);

            _f1.LoadCompleted -= new LoadCompletedEventHandler(HLTest_f1_LoadCompleted);
        }

        private void HLTest_f1_ContentRendered(object sender, EventArgs e)
        {
            Border border = DRT.FindElementByID("Page1MainBorder", _f1) as Border;

            if (border == null)
            {
                DRT.Fail("Failed to parse 'HyperlinkPage1.xaml' (root border is not created).");
            }

            Console.WriteLine("Looking for 'HL1' in HyperlinkPage1.xaml");

            Hyperlink hl = DRT.FindElementByID("HL1", border) as Hyperlink;

            if (hl == null)
            {
                DRT.Fail("HL1 is not found, please check if HyperlinkPage1.xaml is loaded");

            }

            // Regression_Bug12 - Hyperlink content style is not expanded
            Hyperlink hl2 = DRT.FindElementByID("HL_WITH_OBJARRAY", border) as Hyperlink;

            if (hl2 == null)
            {
                Console.WriteLine("Hyperlink (ID: HL_WITH_OBJARRAY) not found!");
                DRT.Fail("This may be related to Regression_Bug12 - Hyperlink content style is not expanded");
            }

            Console.WriteLine("Now, verify that content is generated");

            Run text = DRT.FindElementByID("TEXT_IN_H_W_O", hl2) as Run;

            if (text == null)
            {
                DRT.Fail("Run (ID: TEXT_IN_H_W_O) not found! (Please verify if Regression_Bug12 is regressed)");
            }

            // Continue the next test
            _currentHyperLink = hl;

            EndWaitAndUpdateMainWindow(TestSteps.HyperLink_ContentStyleTest);
        }

        internal void ClickHyperLink(Hyperlink hl)
        {
            MethodInfo info = typeof(Hyperlink).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);

            if (info == null) throw new Exception("Could not find OnClick method on " + hl);

            info.Invoke(hl, new object[] {});
        }

        internal void KeyDown(Hyperlink hl, Key key)
        {
            PresentationSource source = PresentationSource.FromVisual(hl.Parent as Visual);

            MethodInfo info = typeof(Hyperlink).GetMethod("OnKeyDown", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);

            if (info == null) throw new Exception("Could not find OnKeyDown method on " + hl);

            KeyEventArgs e = new KeyEventArgs(InputManager.Current.PrimaryKeyboardDevice, source, Environment.TickCount, key);

            e.RoutedEvent=Keyboard.KeyDownEvent;
            hl.RaiseEvent(e);
            //info.Invoke(hl, new object[] {e});
        }

        internal void KeyUp(Hyperlink hl, Key key)
        {
            PresentationSource source = PresentationSource.FromVisual(hl.Parent as Visual);

            MethodInfo info = typeof(Hyperlink).GetMethod("OnKeyUp", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);

            if (info == null) throw new Exception("Could not find OnKeyUp method on " + hl);

            KeyEventArgs e = new KeyEventArgs(InputManager.Current.PrimaryKeyboardDevice, source, Environment.TickCount, key);

            e.RoutedEvent=Keyboard.KeyUpEvent;
            hl.RaiseEvent(e);
            //info.Invoke(hl, new object[] { e });
        }
    }
}



