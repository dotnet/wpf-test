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
    /// Multiple HwndSource with a Single Thread and Single Context
    /// </summary>
    /// <remarks>
    ///     <ol>Scenarios steps:
    ///             <li>Creating 1 context and Enter the context</li>
    ///             <li>Create a 3 HelloElement to be rendered. Also we add an event to be called when the OnRender is executed</li>
    ///             <li>Create an 3 HwndSource and Validate that creation</li>
    ///             <li>Run the Dispatcher</li>
    ///             <li>HelloElement.OnRender will be executed and the renderEvent(testing event) will be called this will
    ///             post an item to Close the window and stop the dispatcher</li>
    ///             <li>Validate all windows are destroyed</li>
    ///     </ol>
    /// </remarks>
    [Test(0, @"Source\Simple", TestCaseSecurityLevel.FullTrust, "SimpleMultipleHwndSourceSTSC", Area = "AppModel")]
    public class SimpleMultipleHwndSourceSTSC : TestCase
    {
        
        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public SimpleMultipleHwndSourceSTSC() :base(TestCaseType.ContextSupport){}
        
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        override public void Run()
        {
            CoreLogger.BeginVariation();
            HelloElement HelloOne, HelloTwo, HelloThree;

            HwndSource SourceOne,SourceTwo,SourceThree;

           
            using (CoreLogger.AutoStatus("Creating a HelloElement For Source 1"))
            {
                HelloOne = new HelloElement();
                HelloOne.RenderedSourcedHandlerEvent +=new Avalon.Test.CoreUI.Source.HelloElement.RenderHandler(Hello_RenderdSourcedHandlerEvent);
            }

            using (CoreLogger.AutoStatus("Creating a HwndSource 1"))
            {
                SourceOne =SourceHelper.CreateHwndSource( 400, 400,0,0);
                HelloOne.Source = SourceOne;
                SourceOne.RootVisual = HelloOne;
            }
            
            using (CoreLogger.AutoStatus("Creating a HelloElement For Source 2"))
            {
                HelloTwo = new HelloElement();
                HelloTwo.RenderedSourcedHandlerEvent +=new Avalon.Test.CoreUI.Source.HelloElement.RenderHandler(Hello_RenderdSourcedHandlerEvent);
            }

            using (CoreLogger.AutoStatus("Creating a HwndSource 2"))
            {
                SourceTwo = SourceHelper.CreateHwndSource(100, 100, 25, 400);
                HelloTwo.Source = SourceTwo;
                SourceTwo.RootVisual = HelloTwo;
            }

            using (CoreLogger.AutoStatus("Creating a HelloElement For Source 3"))
            {
                HelloThree = new HelloElement();
                HelloThree.RenderedSourcedHandlerEvent +=new Avalon.Test.CoreUI.Source.HelloElement.RenderHandler(Hello_RenderdSourcedHandlerEvent);
            }

            using (CoreLogger.AutoStatus("Creating a HwndSource 3"))
            {
                SourceThree = SourceHelper.CreateHwndSource(100, 100,400, 25);
                HelloThree.Source = SourceThree;
                SourceThree.RootVisual = HelloThree;
            }


            using (CoreLogger.AutoStatus("Validating the Handle is valid Thread:" + Thread.CurrentThread.Name ))
            {            
                if (SourceOne.Handle.ToInt64() <= 0 || SourceTwo.Handle.ToInt64() <= 0 || SourceThree.Handle.ToInt64() <= 0)
                {
                    throw new Microsoft.Test.TestValidationException("Window is not created");
                }
            }
            
            using (CoreLogger.AutoStatus("Dispather.Run Thread:" + Thread.CurrentThread.Name ))
            {            

                
                Dispatcher.Run();

            }


            using (CoreLogger.AutoStatus("Validating Handle on the HwndSource Thread:" + Thread.CurrentThread.Name ))
            {            
                if (!SourceOne.IsDisposed || !SourceTwo.IsDisposed || !SourceThree.IsDisposed)
                {
                    throw new Microsoft.Test.TestValidationException("The Handler of HwndSource should be an invalid windows");
                }
            }
            CoreLogger.EndVariation();
        }
        
           
        
        /// <summary>
        /// This handler will be posted to the Context to Close the windows and Stop the dispatcher
        /// </summary>
        /// <param name="o"></param>
        object CloseWindowASyncHandler(object o)
        {
            s_counter++;
            HwndSource Source = (HwndSource)o;
            bool IsWindowDestroy = false;

            using(CoreLogger.AutoStatus("Call Win32.DestroyWindow and Stop the dispatcher Thread:" + Thread.CurrentThread.Name))
            {
                Source.RootVisual = null;
                IsWindowDestroy = Microsoft.Test.Win32.NativeMethods.DestroyWindow( new HandleRef(null,Source.Handle) )  ;

                using (CoreLogger.AutoStatus("Validating Window is destroyed Thread:" + Thread.CurrentThread.Name))
                {            
                    if (!IsWindowDestroy)
                    {
                        throw new Microsoft.Test.TestValidationException("The window should be destroyed");
                    }
                }            
                if (s_counter == 3)
                {
                    MainDispatcher.InvokeShutdown();
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="Source"></param>
        private void Hello_RenderdSourcedHandlerEvent(UIElement target, HwndSource Source)
        {
            using(CoreLogger.AutoStatus("BeginInvoke on Background Priority to Close the Window Thread:" + Thread.CurrentThread.Name))
            {
              target.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(CloseWindowASyncHandler), Source);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        static int s_counter = 0;

        
    }


}


