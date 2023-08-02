// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Avalon.Test.CoreUI;
using System.Threading;
using System.Windows.Threading;

using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;
using Avalon.Test.CoreUI.Common;

using System.Runtime.InteropServices;
using Avalon.Test.CoreUI.Source;
using Microsoft.Test.Win32;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Avalon.Test.Framework.Dispatchers.Nested
{
    /******************************************************************************
    * CLASS:          SimpleNestedMessageLoop
    ******************************************************************************/
    ///<summary>
    ///         Testing a simple Modal Window. This will create a modal window on mouse up on the main window. Single Thread Single Context
    ///</summary>
    ///<remarks>
    ///     <ol>Scenarios steps:
    ///         <li>Creating 1 context and Enter the context, mouse is on 0,0 coordinates</li>
    ///         <li>Create an HwndSource and source a HelloElement</li>
    ///         <li>On the render of the HelloEleement, we inject a MouseDown and mouse up on the HelloElement</li>
    ///         <li>On the MouseUp event we create a SimpleModalTestWindow and we source a HelloElement (II) and we call the show() that it will push a frame to the dispatcher</li>
    ///         <li>On the render of HelloElement (II),  we inject a Mouse Down and Up </li>
    ///         <li>On the MouseUp on the HelloElement(II) we listen and we set a global varaible that the test passes and we close the window</li>
    ///         <li>We validate that the PushFrame works correctly.</li>
    ///     </ol>
    ///     <filename>SimpleNestedMessageLoop.cs</filename>
    ///</remarks>
    [Test(0, @"Dispatcher.PushFrame", TestCaseSecurityLevel.FullTrust, "SimpleNestedMessageLoop")]
    public class SimpleNestedMessageLoop : TestCase
    {
        #region Private Data
        private SimpleModalTestWindow _modalWindow = null;
        private bool _isTestPassed = false;
        #endregion

        
        #region Constructor
        /******************************************************************************
        * Function:          SimpleNestedMessageLoop Constructor
        ******************************************************************************/
        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public SimpleNestedMessageLoop() :base(TestCaseType.HwndSourceSupport)
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        TestResult StartTest()
        {
            HelloElement Hello;

            using (CoreLogger.AutoStatus("Hold on to the Context and the HwndDispatcher"))
            { 
                MainDispatcher = Dispatcher.CurrentDispatcher;  
            }

            using (CoreLogger.AutoStatus("Creates an element to render and set to the source. Adding a lister for MouseUp Event"))
            { 
            
                Hello = new HelloElement();
                Hello.MouseLeftButtonUp +=new System.Windows.Input.MouseButtonEventHandler(Hello_MouseUp);   
                Hello.RenderedSourcedHandlerEvent += new HelloElement.RenderHandler(OnRenderFirstElement);
                Hello.Source = Source;
                Source.RootVisual = Hello;
            }

            using (CoreLogger.AutoStatus("Run the dispatcher"))
            { 
                this.OnDestroyMainWindowEvent +=new EventHandler(SimpleNestedMessageLoop_OnDestroyMainWindowEvent);
                Dispatcher.Run();
            }

            using (CoreLogger.AutoStatus("Last Validation"))
            {
                if (!this._isTestPassed)
                {
                    throw new Microsoft.Test.TestValidationException("No correct clicks or something wrong");
                }
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion

        #region Public Members
        /// <summary>
        /// Inject mouse to the main window to Open the Modal dialog. the dialog will be open on MOuseUp
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public object InjectFirstInput(object Source)
        {
            HwndSource source = Source as HwndSource;
            using (CoreLogger.AutoStatus("Inject Mouse Down and Up on Main Window"))
            {
                //MouseUtility.MouseLeftButtonClick(source.Handle,100,100,true);
                MouseHelper.Click((UIElement)source.RootVisual);            
            }
            return null;
        }

        /// <summary>
        /// Inject Mouse Down and Up
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public object InjectSecondInput(object Source)
        {
            // This is the Modal Window
            HwndSource source = Source as HwndSource;
            
            using (CoreLogger.AutoStatus("Mouse Down and Up on the Modal Window"))
            {            
                MouseHelper.Click((UIElement)_modalWindow.Source.RootVisual);                  
            }
            return null;
        }
        #endregion


        #region Private Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="Source"></param>
        private void OnRenderFirstElement(UIElement target, HwndSource Source)
        {            
            target.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(InjectFirstInput), Source);
        }



        /// <summary>
        /// When the Main window is close this is called to stop the dispatcher
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SimpleNestedMessageLoop_OnDestroyMainWindowEvent(object sender, EventArgs e)
        {
            MainDispatcher.InvokeShutdown();
        }

        /// <summary>
        /// During MouseUp event on the MainWindow it will create a ModalWindow that it will be 
        /// host an WPP window and it call Show, that it will push a frame
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hello_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            using (CoreLogger.AutoStatus("Creating the Modal Dialog"))
            {
                _modalWindow = new SimpleModalTestWindow(Source.Handle);
            }
            using (CoreLogger.AutoStatus("Creating the Source for the Modal Window"))
            {
                HelloElement hello = new HelloElement();
                hello.Source = _modalWindow.Source;
                hello.RenderedSourcedHandlerEvent +=new Avalon.Test.CoreUI.Source.HelloElement.RenderHandler(hello_RenderedSourcedHandlerEvent);
                hello.FontColor = Brushes.Red;
                hello.MouseLeftButtonUp +=new System.Windows.Input.MouseButtonEventHandler(hello_MouseUp);
                _modalWindow.RootVisual = hello;
            }
            using (CoreLogger.AutoStatus("ModalWindow.Show"))
            {
                _modalWindow.Show();
            }
            
            using (CoreLogger.AutoStatus("Validating the Nested Frame works"))
            {
                if (!this._isTestPassed)
                {
                    throw new Microsoft.Test.TestValidationException("Incorrect Nested Pump");
            
                }
            }
        }

        /// <summary>
        /// When the modal window with the Visual is rendered we inject Input over the modal window
        /// to validate that works. MouseDown and Up
        /// </summary>
        /// <param name="target"></param>
        /// <param name="Source"></param>
        private void hello_RenderedSourcedHandlerEvent(UIElement target, HwndSource Source)
        {
            target.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(InjectSecondInput), Source);
        }

        /// <summary>
        /// We set a Global varible as true to mark that the test pass and we close the windows. End of the test case
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hello_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isTestPassed = true;
            using (CoreLogger.AutoStatus("Closing the Modal Window"))
            {
                NativeMethods.PostMessage(new HandleRef(null,_modalWindow.Source.Handle),NativeConstants.WM_CLOSE,IntPtr.Zero,IntPtr.Zero);
            }
            using (CoreLogger.AutoStatus("Closing the Main Window"))
            {
                NativeMethods.PostMessage(new HandleRef(null,Source.Handle),NativeConstants.WM_CLOSE,IntPtr.Zero,IntPtr.Zero);
            }
            
        }
        #endregion
    }
}


