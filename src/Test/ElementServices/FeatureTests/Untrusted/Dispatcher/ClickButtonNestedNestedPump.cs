// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading; 
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Win32;


namespace Avalon.Test.Framework.Dispatchers
{
    /******************************************************************************
    * CLASS:          ClickButtonNestedNestedPump
    ******************************************************************************/

    // [DISABLE WHILE PORTING]
    //[Test(0, "Dispatcher.PushFrame", TestCaseSecurityLevel.FullTrust, "ClickButtonNestedNestedPump")]
    public class ClickButtonNestedNestedPump : TestCase
    {
        #region Private Data
        private  bool _isFirstPainted = true;
        private bool _isFirstPaintedMW = true;
        private bool _isFirstPaintedMWTwo = true;
        private SimpleModalTestWindow _mw = null,_mwTwo = null;
        private Button _b1;
        private Button _b2;
        private Button _b3;
        #endregion


        #region Constructor
        /******************************************************************************
        * Function:          ClickButtonNestedNestedPump Constructor
        ******************************************************************************/
        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.None
        /// </summary>
        public ClickButtonNestedNestedPump() :base(TestCaseType.HwndSourceSupport)
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        ///  Create a Window with a button, click on the button and create a modal dialog, later click onteh modal dialog and to close the windows
        /// </summary>
        /// <remarks>
        ///  <ol>Description Steps:
        ///  </ol>
        ///  ClickButtonNestedNestedPump.cs
        /// </remarks>
        TestResult StartTest()
        {
            TestCaseFailed = true;

            Border border = new Border();
            StackPanel sp = new StackPanel();

            border.Child = sp;

            Button b = new Button();
            _b1 = b;
            b.Content = "Click here!";
            b.Name = "Button1";
            b.Width = 100;
            b.Height = 100;
            b.Click += new RoutedEventHandler(buttonClicked);
            sp.Children.Add(b);

            HelloElement hello = new HelloElement();

            hello.Source = Source;
            hello.RenderedSourcedHandlerEvent += new HelloElement.RenderHandler(onPainted);
            sp.Children.Add(hello);
            Source.RootVisual = border;

            using (CoreLogger.AutoStatus("Main Loop Dispatcher.Run"))
            {
                Dispatcher.Run();
            }

            using (CoreLogger.AutoStatus("Last Validation"))
            {
                FinalReportFailure();
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Public Methods
        private void onPainted(UIElement target, HwndSource Source)
        {
            if (_isFirstPainted)
            {
                _isFirstPainted = false;
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(firstClick), null);
            }
        }


        private object firstClick(object o)
        {
            using (CoreLogger.AutoStatus("Click on Main Window Button"))
            {
                MouseHelper.Click(_b1);
            }

            return null;
        }


        private void buttonClicked(object sender, RoutedEventArgs e)
        {
            using (CoreLogger.AutoStatus("Creating First Modal Window"))
            {
                _mw = new SimpleModalTestWindow(Source.Handle);

                Border border = new Border();
                StackPanel sp = new StackPanel();

                border.Child = sp;
                border.Background = Brushes.Green;

                Button b = new Button();
                _b2 = b;
                sp.Children.Add(b);
                b.Content = "Modal Window Click here!";
                b.Name = "ButtonModalWindow";
                b.Width = 100;
                b.Height = 100;
                b.Click += new RoutedEventHandler(mwbuttonClicked);

                HelloElement hello = new HelloElement();

                sp.Children.Add(hello);
                hello.Source = _mw.Source;
                hello.RenderedSourcedHandlerEvent += new HelloElement.RenderHandler(onPaintedMW);
                _mw.RootVisual = border;
            }

            using (CoreLogger.AutoStatus("FirstModalWindow.Show"))
            {
                _mw.Show();
            }
        }


        private void onPaintedMW(UIElement target, HwndSource Source)
        {
            if (_isFirstPaintedMW)
            {
                _isFirstPaintedMW = false;
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(secondClick), Source);
            }
        }


        private object secondClick(object o)
        {
            HwndSource source = o as HwndSource;

            using (CoreLogger.AutoStatus("Click on First Modal Window"))
            {
                MouseHelper.Click(_b2);
            }

            return null;
        }


        private void mwbuttonClicked(object sender, RoutedEventArgs e)
        {
            using (CoreLogger.AutoStatus("Create Second ModalWindow"))
            {
                _mwTwo = new SimpleModalTestWindow(Source.Handle);

                Border border = new Border();
                StackPanel sp = new StackPanel();

                border.Child = sp;
                border.Background = Brushes.Green;

                Button b = new Button();
                _b3 =b;
                sp.Children.Add(b);
                b.Content = "Modal Window Click here!";
                b.Name = "ButtonModalWindow";
                b.Width = 100;
                b.Height = 100;
                b.Click += new RoutedEventHandler(mwTwobuttonClicked);

                HelloElement hello = new HelloElement();

                sp.Children.Add(hello);
                hello.Source = _mwTwo.Source;
                hello.RenderedSourcedHandlerEvent += new HelloElement.RenderHandler(onPaintedMWTwo);
                _mwTwo.RootVisual = border;
            }

            using (CoreLogger.AutoStatus("Second ModalWindow.Show"))
            {
                _mwTwo.Show();
            }
        }


        private void onPaintedMWTwo(UIElement target, HwndSource Source)
        {
            if (_isFirstPaintedMWTwo)
            {
                _isFirstPaintedMWTwo = false;
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(thirdClick), Source);
            }
        }


        private object thirdClick(object o)
        {
            HwndSource source = o as HwndSource;

            using (CoreLogger.AutoStatus("Click on Second Modal Window"))
            {
                MouseHelper.Click(_b3);
            }

            return null;
        }


        private void mwTwobuttonClicked(object sender, RoutedEventArgs e)
        {
            TestCaseFailed = false;
            MainDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(exitMWWindow), null);
        }


        private object exitMWWindow(object o)
        {
            _mwTwo.Source.Dispose();
            _mw.Source.Dispose();
            MainDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(exitWindow), null);
            return null;
        }


        private object exitWindow(object o)
        {
            Source.Dispose();
            Microsoft.Test.Threading.DispatcherHelper.ShutDown();
            return null;
        }
        #endregion
    }
}





