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
using System.Collections;

namespace Avalon.Test.CoreUI.Source.Hwnd
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    [Test(0, @"Source\Complex", TestCaseSecurityLevel.FullTrust, "MultipleHwndSourceSingleDispatcher", Area = "AppModel")]
    public class MultipleHwndSourceMultipleClickSingleDispatcherSingleContext : TestCase
    {
        
        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public MultipleHwndSourceMultipleClickSingleDispatcherSingleContext() :base(TestCaseType.HwndSourceSupport){}
        
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        override public void Run()
        {
            CoreLogger.BeginVariation();
            _childrenSource = new HwndSource[2];           
            this.OnDestroyMainWindowEvent +=new EventHandler(MultipleHwndSourceMultipleClickSingleDispatcherSingleContext_OnDestroyMainWindowEvent);

            CreateHost(0);
            CreateHost(1);

            NativeMethods.SetForegroundWindow(new HandleRef(null,Source.Handle));
            using (CoreLogger.AutoStatus("Dispather.Run"))
            {                                
                try
                {
                    Dispatcher.Run();
                }
                catch(Exception e)
                {
                    throw e;
                }
            }

            if (!_gotClickControl1 || !_gotClickControl2)
            {
                throw new Microsoft.Test.TestValidationException("");
            }
            CoreLogger.EndVariation();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        void CreateHost(int Name)
        {
            HelloElement Hello;
            
            if (Name == 1)
            {
                _childPositionX = 170;
            }

            using (CoreLogger.AutoStatus("Creating a HelloElement to render Host: " + Name.ToString()  ))
            {
                Hello = new HelloElement();
                Hello.MouseLeftButtonUp +=new System.Windows.Input.MouseButtonEventHandler(Hello_MouseUp);
                Hello.MouseLeftButtonDown +=new System.Windows.Input.MouseButtonEventHandler(Hello_MouseDown);
                if (Name == 1)
                    Hello.FontColor = Brushes.Green;
                Hello.RenderedSourcedHandlerEvent +=new Avalon.Test.CoreUI.Source.HelloElement.RenderHandler(Hello_RenderdSourcedHandlerEvent);
            }
            using (CoreLogger.AutoStatus("Creating a HwndSource Host:" + Name.ToString()))
            {
                _childrenSource[Name] = SourceHelper.CreateHwndSource( _childSizeX, _childSizeY, _childPositionX, _childPositionY,Source.Handle);
                Hello.Source = _childrenSource[Name];
                _childrenSource[Name].RootVisual = Hello;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="Source"></param>
        private void Hello_RenderdSourcedHandlerEvent(UIElement target, HwndSource Source)
        {

            if (Source == _childrenSource[0])
            {
                _isSource1Render = true;
            }
            if (Source == _childrenSource[1])
            {
                _isSource2Render = true;
            }
            if (_isSource2Render && _isSource1Render && _firstRender)
            {
                _firstRender = false;
                MainDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(InputFirstControl), _childrenSource[0]);

            }
        }


        /// <summary>
        /// Click on the First Control
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        object InputFirstControl(object o)
        {
            MouseHelper.Click((UIElement)_childrenSource[0].RootVisual);
            return null;
        }


        /// <summary>
        /// Closing the dispatcher
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MultipleHwndSourceMultipleClickSingleDispatcherSingleContext_OnDestroyMainWindowEvent(object sender, EventArgs e)
        {

                MainDispatcher.InvokeShutdown();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hello_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender == _childrenSource[0].RootVisual)
            {
                _gotClickControl1=true;
                MainDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(InputSecondControl), _childrenSource[1]);
            }
            if (sender == _childrenSource[1].RootVisual)
            {
                _gotClickControl2=true;
                using(CoreLogger.AutoStatus("BeginInvoke on Background Priority to Close the Window Thread:" + Thread.CurrentThread.Name))
                {
                    MainDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(CloseWindowASyncHandler), Source);
                }
            }
        }

        /// <summary>
        /// Click on the First Control
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        object InputSecondControl(object o)
        {
            HwndSource source = o as HwndSource;
            MouseHelper.Click((UIElement)_childrenSource[1].RootVisual);
            return null;
        }

        /// <summary>
        /// Destroying the main window
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        object CloseWindowASyncHandler(object o)
        {
            Source.Dispose();
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hello_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender == _childrenSource[0].RootVisual)
            {
             Console.WriteLine("Mouse Down on Ctrl 1");
            }
            if (sender == _childrenSource[1].RootVisual)
            {
             Console.WriteLine("Mouse Down on Ctrl 2");
            }
        }

        int _childPositionX = 10, _childPositionY = 10;
        int _childSizeX = 150, _childSizeY = 150;
        HwndSource[] _childrenSource;
        bool _isSource1Render = false, _isSource2Render = false, _firstRender = true;
        bool _gotClickControl1 = false,_gotClickControl2 = false;
    }
}


