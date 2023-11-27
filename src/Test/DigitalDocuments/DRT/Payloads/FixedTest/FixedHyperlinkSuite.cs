// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// Copyright (C) Microsoft Corporation.  All rights reserved.
// Description: Test cases for Fixed hyperlink navigation. 
//

using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Reflection;
using System.IO;

namespace DRT
{
    public class FixedHyperlinkSuite : DrtTestSuite
    {
        public FixedHyperlinkSuite() : base("FixedHyperlinkSuite")
        {
            Contact = "Microsoft";
        }

        /// <summary>
        /// Defining Test Steps
        /// </summary>
        enum TestSteps
        {
            Start, 
            HyperLink_NavTest1,
            //HyperLink_NavTest2,
            HyperLink_NavTest3,
            End,
        }
        private void OnMainWindowClosed(object sender, EventArgs e)
        {
            if (DRT.KeepAlive)
            {
                // ShutDown the application when the main window is closed.
                Dispatcher.CurrentDispatcher.InvokeShutdown();
                DRT.Dispose();
            }
        }

        public override DrtTest[] PrepareTests()
        {
            step = TestSteps.Start;

            MainTestWindow = new NavigationWindow();
            MainTestWindow.Title = "DrtFixedHyperlink";
            MainTestWindow.SandboxExternalContent = false;
            
            DrtFixedBase drtFixed = DRT as DrtFixedBase;

            if (DRT.KeepAlive == true)
            {
                MainTestWindow.Title = "Test Window";
                String extension = System.IO.Path.GetExtension(drtFixed.InFileName);
                if (String.Compare(extension, ".zip", true) == 0 ||
                    String.Compare(extension, ".xps", true) == 0 )
                {
                    MainTestWindow.Content = drtFixed.LoadPackage(drtFixed.InFileName) as System.Windows.Documents.IDocumentPaginatorSource;
                }
                else  // if (String.Compare(extension, ".xaml", true) == 0)
                {
                    MainTestWindow.Content = drtFixed.LoadXaml(drtFixed.InFileName) as System.Windows.Documents.IDocumentPaginatorSource;
                }
            }
            else
            {
                // DocumentViewer dv = new DocumentViewer();
                // dv.Content = LoadXamlFile("DRTFiles\\Payloads\\PageContent\\MSNMain.xaml") as System.Windows.Documents.IDocumentPaginatorSource;
                // MainTestWindow.Content = dv;     
                MainTestWindow.Content = drtFixed.LoadXaml("DRTFiles\\Payloads\\PageContent\\MSNMain.xaml") as System.Windows.Documents.IDocumentPaginatorSource;
            }
            MainTestWindow.Closed += new EventHandler(OnMainWindowClosed);
            MainTestWindow.Show();

            if (DRT.KeepAlive)
            {
                return new DrtTest[0];
            }
            else
            {
                return new DrtTest[]
                {
                    new DrtTest(MainTestLoop),
                    new DrtTest(Cleanup),
                };
            }
        }

        private void Cleanup()
        {
        }
        
        private void MainTestLoop()
        {
            Console.WriteLine("Test Step: {0}", step);

            switch (step)
            {
                case TestSteps.Start:
                    break;

                case TestSteps.HyperLink_NavTest1:
                    {
                        FrameworkElement fe = FindFixedPage(MainTestWindow);
                        Glyphs g = DRT.FindElementByID("Source1", fe) as Glyphs;
                        KeyDown(g, Key.Enter);
                        KeyUp(g, Key.Enter);

                        MainTestWindow.LoadCompleted += new LoadCompletedEventHandler(NextPageNavigated);
                        WaitForAsyncOperation();
                    }
                    break;

                /* to reduce drt time.
                case TestSteps.HyperLink_NavTest2:
                    if (MainTestWindow.CanGoBack)
                    {
                        MainTestWindow.GoBack();

                        FrameworkElement fe = FindFixedPage(MainTestWindow);
                        MoveMouse(fe, MainTestWindow, .05, .05);
                        Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftDown);
                        Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftUp);

                        MainTestWindow.LoadCompleted += new LoadCompletedEventHandler(NextPageNavigated);

                        WaitForAsyncOperation();
                    }

                    break;
*/
                case TestSteps.HyperLink_NavTest3:
                    if (MainTestWindow.CanGoBack)
                    {
                        MainTestWindow.LoadCompleted += new LoadCompletedEventHandler(NextPageNavigated);
                        MainTestWindow.GoBack();

                        WaitForAsyncOperation();
                    }
                    break;

                case TestSteps.End:
                    Thread.Sleep(2000);
                    MainTestWindow.Close();
                    break;

                default:
                    DRT.Fail("Unknown test step: {0}. Aborting test...", step);
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
                if (step != TestSteps.End)
                {
                    step++;
                    DRT.ResumeAt(new DrtTest(MainTestLoop));
                }
            }
        }

        // Move the mouse
        //  fe - framework element
        //  x - horizontal position:  0.0 = left edge,  1.0 = right edge
        //  y - vertical position:    0.0 = top edge,   1.0 = bottom edge
        private void MoveMouse(FrameworkElement fe, Window reference, double x, double y)
        {
            Point pt = new Point(x * fe.RenderSize.Width, y * fe.RenderSize.Height);
            GeneralTransform g = fe.TransformToAncestor(reference);
            g.TryTransform(pt, out pt);

            Input.MoveTo(PointToScreen(GetHwndSource(reference), pt));
        }

        private HwndSource  GetHwndSource(Window window)
        {
            PropertyInfo info = window.GetType().GetProperty("HwndSourceWindow", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic);

            Object obj = info.GetValue(window, null);

            return (HwndSource)obj;
        }

        private Point PointToScreen(HwndSource window, Point ptSrc)
        {
            MS.Win32.NativeMethods.POINT pt = new MS.Win32.NativeMethods.POINT(Convert.ToInt32(ptSrc.X), Convert.ToInt32(ptSrc.Y));

            MS.Win32.UnsafeNativeMethods.ClientToScreen(window.Handle, pt);
            return new Point(pt.x, pt.y);
        }

        private FrameworkElement FindFixedPage(DependencyObject node)
        {
            // see if the node itself has the right value
            FixedPage fp = node as FixedPage;

            if (fp != null)
            {
                return fp;
            }

            // if not, recursively look at the visual children
            int count = VisualTreeHelper.GetChildrenCount(node);

            for(int i = 0; i < count; i++)
            {
                FrameworkElement result = FindFixedPage(VisualTreeHelper.GetChild(node,i));

                if (result != null)
                    return result;
            }

            // not found
            return null;
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


        public void WaitForAsyncOperation()
        {
            if (IsWaiting)
            {
                DRT.Fail("TestManager is currently waiting for step {0} to finish. Cannot wait for another step. Aborting test...", step);
            }
            else
            {
                Console.WriteLine("Waiting for step {0} to finish...", step);

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

                step++;
                Console.WriteLine("EndWait: continuing MainTestLoop at step {0}", step);
                DRT.ResumeAt(new DrtTest(MainTestLoop));
            }
        }

        private void OnWaitTimeout(object arg, EventArgs e)
        {
            DRT.Fail("Wait timeout reached (wait for step {0}).", step);
        }

  

        void EndWaitAndUpdateMainWindow(TestSteps currentStep)
        {
            MainTestWindow.InvalidateMeasure();
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

        internal void KeyDown(UIElement element, Key key)
        {
            PresentationSource source = PresentationSource.FromVisual(element as Visual);

            KeyEventArgs e = new KeyEventArgs(InputManager.Current.PrimaryKeyboardDevice, source, Environment.TickCount, key);

            e.RoutedEvent= Keyboard.KeyDownEvent;
            element.RaiseEvent(e);
       }

        internal void KeyUp(UIElement element, Key key)
        {
            PresentationSource source = PresentationSource.FromVisual(element as Visual);


            KeyEventArgs e = new KeyEventArgs(InputManager.Current.PrimaryKeyboardDevice, source, Environment.TickCount, key);

            e.RoutedEvent=Keyboard.KeyUpEvent;
            element.RaiseEvent(e);
        }

        private void NextPageNavigated(object sender, EventArgs e)
        {
            MainTestWindow.Navigated -= new NavigatedEventHandler(NextPageNavigated);
            EndWaitAndUpdateMainWindow(step + 1);
        }

        //
        // Test resource
        //
        private NavigationWindow MainTestWindow = null;
        private TestSteps step;

        private int _waitTime = 60000;
        DispatcherTimer _waitOperation;

    }
}




