// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Reflection;
using System.Xml;

namespace DRT
{
    public class DrtFrameTestSuite : DrtTestSuite
    {
        public DrtFrameTestSuite() : base("Frame")
        {
            Contact = "Microsoft";
        }

        /// <summary>
        /// Defining Test Steps
        /// </summary>
        enum TestSteps
        {
            Start, 
            //
            // Path I: Hyperlink specific tests
            //
            HyperLink_ContentStyleTest,
            HyperLink_NavigationTest,
            HyperLink_EnterKeyTest,

            //
            // Part II: Frame specific tests
            //
            // Test 0.2
            Frame_TestTemplateParts,
            // Test 0.5
            Frame_TestNullContent,
            // Test 1
            Frame_TestSourceProperty,
            // Test 2
            Frame_NavigateToElement,
            // Test 3
            Frame_NavigateUsingUri,
            // Test 4
            Frame_TestEventRoute,
            // Test 5
            Frame_NavigateUsingSourceProperty,
            // Test 6
            Frame_TestContentProperty,
            // Test 7
            Frame_TestIsTreeSeparatorFlag,

            Frame_Regress871661_ClipToBoundTest,
            Frame_Regress871661_ClipToBoundTest_ClickMouse,
            Frame_Regress871661_ClipToBoundTest_Verify,
            Frame_Regress966857_NavigateAndSourceSyncCheck,
            Frame_Regress1004762_ContentMarginTest,
            Frame_EnsureNoLogicalChild,
            NavigarionWindow_InitialFocus,
            // 

                
            End,
        }

        public override DrtTest[] PrepareTests()
        {
            _step = TestSteps.Start;
            _previousWarningLevel = ((DrtControlsBase)DRT).WindowDeactivatedWarningLevel;
            ((DrtControlsBase)DRT).WindowDeactivatedWarningLevel = WarningLevel.Ignore;
            DrtControls.NotUsingDrtWindow(DRT);

            _mainTestWindow = new NavigationWindow();
            _mainTestWindow.Title = "DrtFrame";
            _mainTestWindow.Show();

            // Load shared images
            _image1 = CreateImage(BitmapImage1);
            _image2 = CreateImage(BitmapImage2);

            return new DrtTest[]
            {
                new DrtTest(MainTestLoop),
                new DrtTest(Cleanup),
            };
        }

        private void Cleanup()
        {
            ((DrtControlsBase)DRT).WindowDeactivatedWarningLevel = _previousWarningLevel;
        }

        //
        // Test resource
        //
        Hyperlink _currentHyperLink = null;
        NavigationWindow _mainTestWindow = null;
        private WarningLevel _previousWarningLevel;


        Frame _f1 = null;
        Frame _f2 = null;
        Frame _f3 = null;
        Image _image1 = null;
        Image _image2 = null;        

        Frame _f4 = null;
        DockPanel _dp4 = null;
        Image _image4 = null;
        bool _isF4LoadCompletedFired = false;

        Frame _f5 = null;
        Frame _f6;
        Image _image6 = null;

        Frame _f8;
        Rectangle _rectOuter8;
        Rectangle _rectInner8;
        DockPanel _dp8;

        Frame _f10;
        DockPanel _dp10;
        Button _bttn10;

        Frame _frameNavigateAndSourceSyncTest = null;
        Frame _frameContentMarginTest = null;
        
        int _bttn10_click_count = 0;
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
                    _f1.LoadCompleted += new LoadCompletedEventHandler(HLTest_f1_LoadCompleted);
                    _f1.ContentRendered += new EventHandler(HLTest_f1_ContentRendered);
                    
                    _f1.Source = new Uri(DRT.BaseDirectory + "Page1.xaml", UriKind.RelativeOrAbsolute); 
                    
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

                    _f1.ContentRendered -= new EventHandler(HLTest_f1_ContentRendered);
                    _f1.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(HLNavTestPage2LoadCompleted);
                    Console.WriteLine("Navigate to Page2.xaml");
                    ClickHyperLink(_currentHyperLink);

                    WaitForAsyncOperation();
                    break;

                case TestSteps.HyperLink_EnterKeyTest:
                    // At this point, _currentHyperLink should be 'HL2' in Page2.xaml
                    // Make sure that we're at the right state
                    if ((_currentHyperLink == null) ||
                        (_currentHyperLink.Name != "HL2"))
                    {
                        DRT.Fail("_currentHyperLink is invalid. It should be 'HL2' in Page2.xaml");
                    }

                    _f1.LoadCompleted -= new System.Windows.Navigation.LoadCompletedEventHandler(HLNavTestPage2LoadCompleted);
                    _f1.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(HLNavTestPage3LoadCompleted);
                    Console.WriteLine("Navigate to Page3.xaml");
                    KeyDown(_currentHyperLink, Key.Enter);
                    KeyUp(_currentHyperLink, Key.Enter);

                    WaitForAsyncOperation();

                    break;

                case TestSteps.Frame_TestTemplateParts:
                    // Test named parts of navigation chrome. This test is also done in DrtNavigationWindow.
                    ContentPresenter cp = (ContentPresenter)_f1.Template.FindName("PART_FrameCP", _f1);
                    if (cp == null)
                        throw new ApplicationException("PART_FrameCP not found.");
                    break;

                case TestSteps.Frame_TestNullContent:
                    Console.WriteLine("Navigate to XAML page, set Content to null, then navigate to same page again");
                    _f1 = new Frame();
                    _mainTestWindow.Content = _f1;
                    _f1.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(Page1_LoadCompleted);
                    _f1.Source = new Uri(DRT.BaseDirectory + "Page1.xaml", UriKind.RelativeOrAbsolute);
                    WaitForAsyncOperation();
                    break;
                
                case TestSteps.Frame_TestSourceProperty:
                    Console.WriteLine("Dynamically add frame to main window, then navigate to DrtFrameOuter.xaml");
                    _f1 = new Frame();
                    _mainTestWindow.Content = _f1;
                    _f1.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(FrameOuter_LoadCompleted);
                    _f1.Source = new Uri(DRT.BaseDirectory + "DrtFrameOuter.xaml", UriKind.RelativeOrAbsolute);
        
                    WaitForAsyncOperation();
                    break;

                case TestSteps.Frame_NavigateToElement:
                    Console.WriteLine("Test Navigate() method. (Navigate to _image1).");
                    _f2 = new Frame();
                    _f2.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(f2_LoadCompleted);
                    _mainTestWindow.Content = _f2;
                    _f2.Navigate(_image1);

                    WaitForAsyncOperation();
                    break;

                case TestSteps.Frame_NavigateUsingUri:
                    Console.WriteLine("Test Navigate(Uri) method. (Navigate to DrtFrameInner.xaml).");
                    _f3 = new Frame();
                    _f3.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(f3_LoadCompleted);
                    _mainTestWindow.Content = _f3;
                    _f3.Navigate(new Uri(DRT.BaseDirectory + "DrtFrameInner.xaml", UriKind.RelativeOrAbsolute));

                    WaitForAsyncOperation();
                    break;

                case TestSteps.Frame_TestEventRoute:
                    Console.WriteLine("Test EventRoute. Ensure that event that raised inside the frame has adjusted-source after bubbled outside the frame.");
                    _f4 = new Frame();
                    _dp4 = new DockPanel();
                    _image4 = CreateImage(BitmapImage2);
                    _dp4.Children.Add(_image4);
                    
                    _f4.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(f4_LoadCompleted);
                    _f4.ContentRendered += new EventHandler(f4_ContentRendered);
                    _f4.MouseLeftButtonUp += new MouseButtonEventHandler(f4_MouseLeftButtonUp);
                    _image4.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(img4_MouseLeftButtonUp);
                    _dp4.MouseLeftButtonUp += new MouseButtonEventHandler(_dp4_MouseLeftButtonUp);
                    _mainTestWindow.MouseLeftButtonUp += new MouseButtonEventHandler(MainTestWindow_MouseLeftButtonUp);

                    
                    // IsLoaded property on FrameworkElement is sometimes set up so that attaching 
                    // a Loaded event handler when IsLoaded is false will not fire the Loaded event at all.
                  
                    _f4.Navigate(_dp4);
                    _mainTestWindow.Content = _f4;

                    WaitForAsyncOperation();
                    break;

                case TestSteps.Frame_NavigateUsingSourceProperty:
                    Console.WriteLine("Setting Source property");
                    _f5 = new Frame ();

                    _f5.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler (f5_LoadCompleted);
                    _mainTestWindow.Content = _f5;
                    _f5.Source = new Uri(DRT.BaseDirectory + "DrtFrameInner.xaml", UriKind.RelativeOrAbsolute);

                    WaitForAsyncOperation();
                    break;

                case TestSteps.Frame_TestContentProperty:
                    Console.WriteLine("Setting Frame Content");
                    _f6 = new Frame();
                    _image6 = CreateImage(BitmapImage2);

                    _f6.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(f6_LoadCompleted);
                    _mainTestWindow.Content = _f6;
                    _f6.Content = _image6;

                    WaitForAsyncOperation();
                    break;

                case TestSteps.Frame_TestIsTreeSeparatorFlag:
                    Console.WriteLine("Ensure that DP cannot be inherited to frame's content");
                    _f8 = new Frame();
                    _dp8 = new DockPanel();             // Frame's parent

                    _rectOuter8 = CreateRectangle(BitmapImage1);   // Rectangle outside frame
                    _rectInner8 = CreateRectangle(BitmapImage1);   // Rectangle inside frame

                    // Clear all localvalues
                    _dp8.ClearValue(FrameworkElement.FlowDirectionProperty);
                    _rectOuter8.ClearValue(FrameworkElement.FlowDirectionProperty);
                    _rectInner8.ClearValue(FrameworkElement.FlowDirectionProperty);
                    _f8.ClearValue(FrameworkElement.FlowDirectionProperty);
                    
                    _dp8.Children.Add(_rectOuter8);
                    _dp8.Children.Add(_f8);
                    DockPanel.SetDock(_rectOuter8, Dock.Top);
                    _f8.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(f8_LoadCompleted);
                    _f8.Navigate(_rectInner8);

                    WaitForAsyncOperation();
                    break;

                case TestSteps.Frame_Regress871661_ClipToBoundTest:
                    //
                    // Navigate to Button that bigger than Frame and make sure that part of the button is clipped
                    //
                    _dp10 = new DockPanel();

                    _f10 = new Frame();

                    //need to do that, otherwise attempt to click at the bottom-right corner of a
                    //big but clipped button moves mouse completely outside of the window area and DRT fails.
                    _f10.HorizontalAlignment = HorizontalAlignment.Left;
                    _f10.VerticalAlignment = VerticalAlignment.Top;
                    
                    _f10.Width = 200;
                    _f10.Height = 200;
                    _f10.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(f10_LoadCompleted);
                    _mainTestWindow.Content = _dp10;
                    _mainTestWindow.MouseLeftButtonUp -= new MouseButtonEventHandler(MainTestWindow_MouseLeftButtonUp);
                    _dp10.Children.Add(_f10);
                
                    _bttn10 = new Button();
                    _bttn10.Width = 300;
                    _bttn10.Height = 300;
                    _bttn10.Click += new RoutedEventHandler(_bttn10_clicked);
                    _bttn10.Content = "This button (400x400) is bigger than the frame (200x200). It should be clipped.";
                    _f10.Navigate(_bttn10);

                    WaitForAsyncOperation();
                    break;

                case TestSteps.Frame_Regress871661_ClipToBoundTest_ClickMouse:
                    //
                    // Click at top-left corner of the button
                    //
                    DRT.MoveMouse(_bttn10, 0.1, 0.1);
                    DRT.ClickMouse();

                    //
                    // Click at bottom_right corner of the button
                    //
                    DRT.MoveMouse(_bttn10, 0.9, 0.9);
                    DRT.ClickMouse();

                    WaitForAsyncOperation();
                    break;

                case TestSteps.Frame_Regress871661_ClipToBoundTest_Verify:
                    if (_bttn10_click_count != 1)
                    {
                        DRT.Fail("_bttn10_click_count != 1 (Test: Regress_ClipToBoundTest_Verify)");
                    }
                    break;

                case TestSteps.Frame_Regress966857_NavigateAndSourceSyncCheck:
                    _frameNavigateAndSourceSyncTest = new Frame();
                    _frameNavigateAndSourceSyncTest.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(frameNavigateAndSourceSyncTest_LoadCompleted);
                    _mainTestWindow.Content = _frameNavigateAndSourceSyncTest;
                    _frameNavigateAndSourceSyncTest.Navigate(new Uri(DRT.BaseDirectory + "Page1.xaml", UriKind.RelativeOrAbsolute));
                    WaitForAsyncOperation();
                    break;

                case TestSteps.Frame_Regress1004762_ContentMarginTest:
                    _frameContentMarginTest = new Frame();
                    _frameContentMarginTest.Background = Brushes.Green;
                    _frameContentMarginTest.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(frameContentMarginTest_LoadCompleted);
                    _frameContentMarginTest.ContentRendered += new EventHandler(ContentMarginTest_ContentRendered);

                    
                    // IsLoaded property on FrameworkElement is sometimes set up so that attaching 
                    // a Loaded event handler when IsLoaded is false will not fire the Loaded event at all.
                  
                    _frameContentMarginTest.Content = XamlReader.Load(new XmlTextReader(DRT.BaseDirectory + "TextPanelWithMargin.xaml"));
                    _mainTestWindow.Content = _frameContentMarginTest;
                    WaitForAsyncOperation();
                    break;

                case TestSteps.Frame_EnsureNoLogicalChild:
                    // Do nothing, just verify that Frame.Content doesn't have LogicalParent.
                    if (_frameContentMarginTest.Content == null)
                    {
                        DRT.Fail("Error: Frame's content is NULL. Something wrong with previous test!");
                    }

                    if (((FrameworkElement)_frameContentMarginTest.Content).Parent != null)
                    {
                        DRT.Fail("Error: Frame's content has LogicalParent. The content shouldn't have logical parent.");
                    }

                    break;

                case TestSteps.NavigarionWindow_InitialFocus:
                    _mainTestWindow.Navigate(new Uri(DRT.BaseDirectory + "Page1.xaml", UriKind.RelativeOrAbsolute));
                    _mainTestWindow.ContentRendered += new EventHandler(OnContentRendered_NavigarionWindow_InitialFocus);
                    WaitForAsyncOperation();
                    break;

                // 
                /*
                // Note: This test need to following ContentMarginTest
                case TestSteps.Frame_NavigateToNullUri:
                    _frameContentMarginTest.LoadCompleted -= new System.Windows.Navigation.LoadCompletedEventHandler(frameContentMarginTest_LoadCompleted);
                    _frameContentMarginTest.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(frameContentMarginTest_NavigateToNullUri_LoadCompleted);
                    _frameContentMarginTest.ContentRendered += new EventHandler(ContentMarginTest_ContentRendered2);
                    _frameContentMarginTest.Source = null;
                    WaitForAsyncOperation();
                    break;
                */

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
                EndWait();
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
                        NavigateUri = "page2.xaml">
                        <TextBlock>page2.xaml</TextBlock>
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
                DRT.Fail("Failed to parse 'Page1.xaml' (root border is not created).");
            }

            Console.WriteLine("Looking for 'HL1' in Page1.xaml");

            Hyperlink hl = DRT.FindElementByID("HL1", border) as Hyperlink;

            if (hl == null)
            {
                DRT.Fail("HL1 is not found, please check if Page1.xaml is loaded");

            }
            // 
            Hyperlink hl2 = DRT.FindElementByID("HL_WITH_OBJARRAY", border) as Hyperlink;

            if (hl2 == null)
            {
                Console.WriteLine("Hyperlink (ID: HL_WITH_OBJARRAY) not found!");
                DRT.Fail("This may be related to Bug#945022 - Hyperlink content style is not expanded");
            }

            Console.WriteLine("Now, verify that content is generated");

            TextBlock text = DRT.FindElementByID("TEXT_IN_H_W_O", hl2) as TextBlock;

            if (text == null)
            {
                DRT.Fail("TextBlock (ID: TEXT_IN_H_W_O) not found! (Please verify if BUG#945022 is regressed)");
            }
            // Continue the next test
            _currentHyperLink = hl;

            EndWaitAndUpdateMainWindow(TestSteps.HyperLink_ContentStyleTest);
        }

        /*
         * Event Handler for HyperLink_NavigationTest
         *
         *****
         * Page2.xaml content:
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
                DRT.Fail("Failed to parse 'Page2.xaml'. Root border (ID='Page2MainBorder')is not created).");
            }

            Console.WriteLine("Looking for 'HL2' in page2.xaml");

            Hyperlink hl = DRT.FindElementByID("HL2", border) as Hyperlink;

            if (hl == null)
            {
                DRT.Fail("HL2 is not found, please check if Page2.xaml is loaded");
            }

            // Continue the next test
            _currentHyperLink = hl;

            EndWaitAndUpdateMainWindow(TestSteps.HyperLink_NavigationTest);
        }

        /*
         * Event Handler for HyperLink_EnterKeyTest
         *
         *****
         * Page2.xaml content:
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
        
        protected void Page1_LoadCompleted(object o, System.Windows.Navigation.NavigationEventArgs args)
        {
            Console.WriteLine("Done navigating.");
            Frame f = (Frame)o;
            Border b = args.Content as Border;
            if (b == null || b.Name != "Page1MainBorder")
            {
                DRT.Fail("Cannot find Page1MainBorder inside Page1.xaml");
            }

            f.LoadCompleted -= new System.Windows.Navigation.LoadCompletedEventHandler(Page1_LoadCompleted);
            f.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(Page1_LoadCompleted_Step2);
            //
            // Setting Frame.Content to null will also clean up XamlContainer's states.
            // The will cause another Navigation event, that reset XamlContainer.Uri/Content to null
            f.Content = null;
        }

        protected void Page1_LoadCompleted_Step2(object o, System.Windows.Navigation.NavigationEventArgs args)
        {
            Console.WriteLine("Done navigating.");
            Frame f = (Frame)o;
            if (f.Content != null)
            {
                DRT.Fail("Frame's content is not null after set Frame.Content=null.");
            }

            f.LoadCompleted -= new System.Windows.Navigation.LoadCompletedEventHandler(Page1_LoadCompleted_Step2);
            f.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(Page1_LoadCompleted_Step3);
            f.Source = new Uri(DRT.BaseDirectory + "Page1.xaml", UriKind.RelativeOrAbsolute);
        }

        protected void Page1_LoadCompleted_Step3(object o, System.Windows.Navigation.NavigationEventArgs args)
        {
            Console.WriteLine("Done navigating.");
            Frame f = (Frame)o;
            
            Border b = args.Content as Border;
            if (b == null || b.Name != "Page1MainBorder")
            {
                DRT.Fail("Cannot find Page1MainBorder inside Page1.xaml");
            }

            f.LoadCompleted -= new System.Windows.Navigation.LoadCompletedEventHandler(Page1_LoadCompleted_Step3);
            EndWaitAndUpdateMainWindow(TestSteps.Frame_TestNullContent);
        }
        
        protected void FrameOuter_LoadCompleted(object o, System.Windows.Navigation.NavigationEventArgs args)
        {
            Console.WriteLine("Done navigating.");
            Frame frameInner = ((Frame)o).Content as Frame;
            if (frameInner == null)
            {
                DRT.Fail("Error: FrameOuter's content is not Frame");
            }

            if (frameInner.Content != null)
            {
                FrameInner_LoadCompleted(frameInner, null);
            }
            else
            {
                frameInner.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(FrameInner_LoadCompleted);
            }
        }
        
        protected void FrameInner_LoadCompleted(object o, System.Windows.Navigation.NavigationEventArgs args)
        {
            Console.WriteLine("Done navigating. (FrameInner)");
            DockPanel dp1 = ((Frame)o).Content as DockPanel;

            if (dp1 == null)
            {
                DRT.Fail("Error: Frame's content is not DockPanel.");
            }
            else
            {
                Image  i1L = AddImage(dp1);
                Image  i2B = AddImage(dp1);
                Image  i3R = AddImage(dp1);
                Image  i4T = AddImage(dp1);
                Image  i5T = AddImage(dp1);
                Image  i6F = AddImage(dp1);

                DockPanel.SetDock(i1L, Dock.Left);
                DockPanel.SetDock(i2B, Dock.Bottom);
                DockPanel.SetDock(i3R, Dock.Right);
                DockPanel.SetDock(i4T, Dock.Top);
                DockPanel.SetDock(i5T, Dock.Top);
            }

            EndWaitAndUpdateMainWindow(TestSteps.Frame_TestSourceProperty);
        }

        private Image AddImage(DockPanel parent)
        {
            Image image = new Image();

            parent.Children.Add(image);
            image.Source = BitmapImage1;
            image.Stretch = Stretch.Fill;
            return image;
        }

        private void SimulateMouseDownEvent(FrameworkElement elem)
        {
            MouseDevice md = System.Windows.Input.InputManager.Current.PrimaryMouseDevice;
            MouseButtonEventArgs mbea = new MouseButtonEventArgs(md, Environment.TickCount, MouseButton.Left);

            // 
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

        // *********************************************************************************
        // * Event Handler for TEST 2
        // *
        protected void f2_LoadCompleted(object o, System.Windows.Navigation.NavigationEventArgs args)
        {
            if (((Frame)o).Content != _image1)
            {
                DRT.Fail("Failed to navigate to Image.");
            }

            EndWaitAndUpdateMainWindow(TestSteps.Frame_NavigateToElement);
        }

        // *********************************************************************************
        // * Event Handler for TEST 3
        // *
        protected void f3_LoadCompleted(object o, System.Windows.Navigation.NavigationEventArgs args)
        {
            DockPanel dp1 = ((Frame)o).Content as DockPanel;
            
            if (dp1 == null)
            {
                DRT.Fail("Failed to navigate to drtframeinner.xaml.");
            }
            else
            {
                Image i6F = AddImage (dp1);
            }

            EndWaitAndUpdateMainWindow(TestSteps.Frame_NavigateUsingUri);
        }

        // *********************************************************************************
        // * Event Handler for TEST 4
        // *
        protected void f4_LoadCompleted(object o, System.Windows.Navigation.NavigationEventArgs args)
        {
            Console.WriteLine("Got LoadCompleted event");
            DockPanel dp = ((Frame)o).Content as DockPanel;
            _isF4LoadCompletedFired = true;
            if (dp == null)
            {
                DRT.Fail("Failed to navigate to DockPanel object (frame content is not a dockpanel)");
            }            
        }

        // Simulate mouse left click
        private void f4_ContentRendered(object sender, EventArgs e)
        {
            Console.WriteLine("Got ContentRendered event (this event should fired after LoadCompleted)");
            if (_isF4LoadCompletedFired == true)
            {
                Console.WriteLine("Simulating Mouse Left Click on _image4");
                SimulateMouseDownEvent(_image4);
            }
            else
            {
                DRT.Fail("Error: The order of event firing is wrong. Did LoadCompleted fire?");
            }
        }

        private void img4_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Console.WriteLine("Got MouseLeftButtonUpEvent in Image.");
            MouseButtonState mbs = e.LeftButton;
        }

        private void _dp4_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("DockPanel: Got MouseLeftButtonUpEvent in DockPanel");
            if (e.OriginalSource is Image)
            {
                Console.WriteLine("\tEvent's OriginalSource is Image. ");
            }
            else
            {
                DRT.Fail("\tError: Event's OriginalSource is not Image. ");
            }
        }

        private void f4_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Frame: Got MouseLeftButtonUpEvent in Frame");
            if (e.Source is Frame)
            {
                Console.WriteLine("\tEvent's Source is Frame. ");
            }
            else
            {
                DRT.Fail("\tError: Event's Source is not Frame. ");
            }
        }
        
        private void MainTestWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if ((e.Source is Frame) && (e.Source == _f4))
            {
                Console.WriteLine(String.Format("Main window got MouseLeftButtonUpEvent from Frame's content {0}", e.Source.GetType().FullName));
                EndWaitAndUpdateMainWindow(TestSteps.Frame_TestEventRoute);
            }
            else
            {
                DRT.Fail("Error: Main windows got MouseLeftButtonUpEvent from Frame's content {0}", e.Source.GetType().FullName);
            }
        }

        // *********************************************************************************
        // * Event Handler for TEST 5
        // *
        protected void f5_LoadCompleted (object o, System.Windows.Navigation.NavigationEventArgs args)
        {
            DockPanel dp1 = ((Frame)o).Content as DockPanel;

            if (dp1 == null)
            {
                DRT.Fail("Error: Failed to navigate to drtframeinner.xaml.");
            }
            else
            {
                Image i1L = AddImage (dp1);
                Image i2B = AddImage (dp1);
                Image i3R = AddImage (dp1);
                Image i4T = AddImage (dp1);
                Image i5T = AddImage (dp1);
                Image i6F = AddImage (dp1);

                DockPanel.SetDock (i1L, Dock.Left);
                DockPanel.SetDock (i2B, Dock.Bottom);
                DockPanel.SetDock (i3R, Dock.Right);
                DockPanel.SetDock (i4T, Dock.Top);
                DockPanel.SetDock (i5T, Dock.Top);

                EndWaitAndUpdateMainWindow(TestSteps.Frame_NavigateUsingSourceProperty);
            }
        }
        
        // *********************************************************************************
        // * Event Handler for TEST 6
        // *
        protected void f6_LoadCompleted(object o, System.Windows.Navigation.NavigationEventArgs args)
        {
            Console.WriteLine("F6 LoadCompleted. Verifying frame content.");
            if (((Frame)o).Content != _image6)
            {
                DRT.Fail("Error: Failed to set Content (Image).");
            }

            EndWaitAndUpdateMainWindow(TestSteps.Frame_TestContentProperty);
        }

        // *********************************************************************************
        // * Event Handler for TEST 8
        // *
        protected void f8_LoadCompleted(object o, System.Windows.Navigation.NavigationEventArgs args)
        {
            if (((Frame)o).Content != _rectInner8)
            {
                DRT.Fail("Error: Failed to navigate to Image.");
            }

            // Now, verify that interit property is blocked
            
            // Make sure that rectInner8.FlowDirection is set to default
            _rectInner8.ClearValue(FrameworkElement.FlowDirectionProperty);
            FlowDirection fdDefault = _rectInner8.FlowDirection;
            
            System.Windows.FlowDirection [] fds = new System.Windows.FlowDirection[2] { FlowDirection.LeftToRight,
                                                            FlowDirection.RightToLeft};
                                    
            for (int i = 0; i < fds.Length; i++)
            {
                if (fds[i].Equals (fdDefault)) continue;
               
                // Modify tree's FlowDirection
                _dp8.FlowDirection = fds[i];
                
                if (!((_dp8.FlowDirection == fds[i]) && 
                    (_rectOuter8.FlowDirection == fds[i]) &&
                    (_f8.FlowDirection == fds[i]) && 
                    (_rectInner8.FlowDirection != fds[i])))// Inheritance should be blocked on frame
                {
                    DRT.Fail("Error: Inheritance is not blocked on frame");
                }
            }

            EndWaitAndUpdateMainWindow(TestSteps.Frame_TestIsTreeSeparatorFlag);
        }

        

        // *********************************************************************************
        // * Event Handler for TEST 10
        // *
        protected void f10_LoadCompleted(object o, System.Windows.Navigation.NavigationEventArgs args)
        {
            Console.WriteLine("F10 LoadCompleted. Verifying frame content...");
            Frame f = o as Frame;

            if ((f == null) || (f != _f10))
            {
                DRT.Fail("Error: sender is not Frame");
            }

            Button bttn = f.Content as Button;

            if (bttn != _bttn10)
            {
                DRT.Fail("Error: Frame content (from xaml) is not a _bttn10.");
            }

            EndWaitAndUpdateMainWindow(TestSteps.Frame_Regress871661_ClipToBoundTest);
        }

        protected void _bttn10_clicked(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("_bttn10 is clicked.");
            _bttn10_click_count += 1;
            EndWaitAndUpdateMainWindow(TestSteps.Frame_Regress871661_ClipToBoundTest_ClickMouse);
        }

        // *********************************************************************************
        // * Event Handler for NavigateAndSourceSyncTest
        // *
        // * After the content is loaded (by calling Navigate), Frame's source property should also updated
        // *
        protected void frameNavigateAndSourceSyncTest_LoadCompleted(object o, System.Windows.Navigation.NavigationEventArgs args)
        {
            Console.WriteLine("frameUriAndSource-LoadCompleted. Verify that Source property and Uri property are the same...");
            Frame f = o as Frame;

            if ((f == null) || (f != _frameNavigateAndSourceSyncTest))
            {
                DRT.Fail("Error: sender is not Frame");
            }

            if (args.Uri != f.Source)
            {
                DRT.Fail("Error: Frame's Source property and Uri property don't match.");
            }

            EndWaitAndUpdateMainWindow(TestSteps.Frame_Regress966857_NavigateAndSourceSyncCheck);
        }

        // *********************************************************************************

        // * Event Handler for ContenMarginTest
        // *
        protected void frameContentMarginTest_LoadCompleted(object o, System.Windows.Navigation.NavigationEventArgs args)
        {
            Console.WriteLine("frameContentMarginTest_LoadCompleted. Content loaded.");
            Frame f = o as Frame;

            if ((f == null) || (f != _frameContentMarginTest))
            {
                DRT.Fail("Error: sender is not Frame");
            }
        }

        protected void ContentMarginTest_ContentRendered(object sender, EventArgs e)
        {
            Console.WriteLine("ContentMarginTest_ContentRendered. Verify content's margin");

            Visual content = _frameContentMarginTest.Content as Visual;
            Visual contentParent = VisualTreeHelper.GetParent(content) as Visual;

            if (contentParent == null)
            {
                DRT.Fail("Error: Frame's content doesn't have a visual parent");
            }

            GeneralTransform transform = content.TransformToAncestor(contentParent);
            Thickness margin = ((FrameworkElement)content).Margin;


            if (transform is Transform)
            {
                Matrix matrix = ((Transform)transform).Value;
                if (matrix.OffsetX != margin.Left ||
                    matrix.OffsetY != margin.Top)
                {
                    DRT.Fail("Error: Frame's content margin doesn't processed correctly. See Bug#1004762.");
                }
            }

            EndWaitAndUpdateMainWindow(TestSteps.Frame_Regress1004762_ContentMarginTest);
            _frameContentMarginTest.ContentRendered -= new EventHandler(ContentMarginTest_ContentRendered);
        }

        /* 




























*/
        
        #region TEST HELPER FUNCTIONS

        private BitmapSource BitmapImage1
        {
            get
            {
                if (_bitmapImage1 == null)
                {
                    _bitmapImage1 = BitmapFrame.Create(new Uri(DRT.BaseDirectory + "Tulip.jpg", UriKind.RelativeOrAbsolute), BitmapCreateOptions.None, BitmapCacheOption.Default);
                }

                return _bitmapImage1;
            }
        }

        private BitmapSource BitmapImage2
        {
            get
            {
                if (_bitmapImage2 == null)
                {
                    _bitmapImage2 = BitmapFrame.Create(new Uri(DRT.BaseDirectory + "Rose.jpg", UriKind.RelativeOrAbsolute), BitmapCreateOptions.None, BitmapCacheOption.Default);
                }

                return _bitmapImage2;
            }
        }

        private BitmapSource _bitmapImage1;
        private BitmapSource _bitmapImage2;

        internal FrameworkElement CreateDockPanelWithImages()
        {
            DockPanel root = new DockPanel();
            FillDockPanel(root);
            return root;
        }
        
        private void FillDockPanel(DockPanel dp)
        {
            if (dp == null)
                return;
            Image i1L = AddImage(dp);
            Image i2B = AddImage(dp);
            Image i3R = AddImage(dp);
            Image i4T = AddImage(dp);
            Image i5T = AddImage(dp);
            Image i6F = AddImage(dp);

            DockPanel.SetDock(i1L, Dock.Left);
            DockPanel.SetDock(i2B, Dock.Bottom);
            DockPanel.SetDock(i3R, Dock.Right);
            DockPanel.SetDock(i4T, Dock.Top);
            DockPanel.SetDock(i5T, Dock.Top);
        }
        
        internal static Image CreateImage(BitmapSource bitmapImage)
        {
            Image image = new Image();
            image.Source = bitmapImage;
            image.Stretch = Stretch.Fill;
            return image;
        }

        internal static Rectangle CreateRectangle(BitmapSource bitmapImage)
        {
            Rectangle rect = new Rectangle();
            rect.Width = bitmapImage.Width;
            rect.Height = bitmapImage.Height;
            rect.Fill = new ImageBrush(bitmapImage);
            return rect;
        }

        #endregion

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
                DRT.Fail("Failed to parse 'Page1.xaml' (root border is not created).");
            }

            Console.WriteLine("Looking for 'HL1' in Page1.xaml");

            Hyperlink hl = DRT.FindElementByID("HL1", border) as Hyperlink;

            if (hl == null)
            {
                DRT.Fail("HL1 is not found, please check if Page1.xaml is loaded");
            }
            else
            {
                if (Keyboard.FocusedElement != hl)
                    DRT.Fail("HL1 should have focus, Page1.xaml initial focus does not work - check root FocusedElement");
            }

            // 
            Hyperlink hl2 = DRT.FindElementByID("HL_WITH_OBJARRAY", border) as Hyperlink;

            if (hl2 == null)
            {
                Console.WriteLine("Hyperlink (ID: HL_WITH_OBJARRAY) not found!");
                DRT.Fail("This may be related to Bug#945022 - Hyperlink content style is not expanded");
            }

            Console.WriteLine("Now, verify that content is generated");

            TextBlock text = DRT.FindElementByID("TEXT_IN_H_W_O", hl2) as TextBlock;

            if (text == null)
            {
                DRT.Fail("TextBlock (ID: TEXT_IN_H_W_O) not found! (Please verify if BUG#945022 is regressed)");
            }

            // Continue the next test
            _currentHyperLink = hl;

            EndWaitAndUpdateMainWindow(TestSteps.HyperLink_ContentStyleTest);
        }

        private void OnContentRendered_NavigarionWindow_InitialFocus(object sender, EventArgs e)
        {
            Hyperlink hl = DRT.FindElementByID("HL1", _mainTestWindow) as Hyperlink;

            if (hl == null)
            {
                DRT.Fail("HL1 is not found, please check if Page1.xaml is loaded");
            }
            else
            {
                if (Keyboard.FocusedElement != hl)
                    DRT.Fail("HL1 should have focus, Page1.xaml initial focus does not work - check root FocusedElement");
            }

            EndWaitAndUpdateMainWindow(TestSteps.NavigarionWindow_InitialFocus);
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
            e.RoutedEvent =Keyboard.KeyDownEvent;
            hl.RaiseEvent(e);
            //info.Invoke(hl, new object[] {e});
        }

        internal void KeyUp(Hyperlink hl, Key key)
        {
            PresentationSource source = PresentationSource.FromVisual(hl.Parent as Visual);

            MethodInfo info = typeof(Hyperlink).GetMethod("OnKeyUp", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);

            if (info == null) throw new Exception("Could not find OnKeyUp method on " + hl);

            KeyEventArgs e = new KeyEventArgs(InputManager.Current.PrimaryKeyboardDevice, source, Environment.TickCount, key);

            e.RoutedEvent =Keyboard.KeyUpEvent;
            hl.RaiseEvent(e);
            //info.Invoke(hl, new object[] { e });
        }
    }
}




