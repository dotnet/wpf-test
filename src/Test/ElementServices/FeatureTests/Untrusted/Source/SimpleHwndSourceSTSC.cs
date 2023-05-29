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

namespace Avalon.Test.CoreUI.Source.Hwnd
{
    /// <summary>
    ///         Create a HwndSource on a Single Thread and Single Context. This will Validate that we create without
    ///         problems. Later we destroy the window and stop the dispatcher
    /// </summary>
    /// <remarks>
    ///     <ol>Scenarios steps:
    ///         <li>Creating 1 context and Enter the context</li>
    ///         <li>Create a HelloElement to be rendered. Also we add an event to be called when the OnRender is executed</li>
    ///         <li>Create an HwndSource and Validate that creation</li>
    ///         <li>Run the Dispatcher</li>
    ///         <li>HelloElement.OnRender will be executed and the renderEvent(testing event) will be called this will
    ///         post an item to Close the window and stop the dispatcher</li>
    ///         <li>Validate the window is destroyed</li>
    ///     </ol>
    ///     <Owner>Microsoft</Owner>
 
    ///     <Area>Source\SimpleThreadSingleContext</Area>
    ///     <location>SimpleHwndSourceSTSC.cs</location>
    ///</remarks>
    [Test(0, @"Source\Simple", TestCaseSecurityLevel.FullTrust, "SimpleHwndSourceSTSC", Area = "AppModel")]
    public class SimpleHwndSourceSTSC : TestCase
    {
        
        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public SimpleHwndSourceSTSC() :base(TestCaseType.ContextSupport){}
        
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        override public void Run()
        {
            CoreLogger.BeginVariation();
            HelloElement Hello;

            HwndSource Source;            

            using (CoreLogger.AutoStatus("Creating a HelloElement to render"))
            {
                Hello = new HelloElement();
                Hello.RenderedSourcedHandlerEvent+=new Avalon.Test.CoreUI.Source.HelloElement.RenderHandler(Hello_RenderdSourcedHandlerEvent);
            }

            using (CoreLogger.AutoStatus("Creating a HwndSource"))
            {
                Source = SourceHelper.CreateHwndSource( 400, 400,0,0);
                Hello.Source = Source;
                Source.RootVisual = Hello;
            }
            
            using (CoreLogger.AutoStatus("Validating the Handle is valid"))
            {            
                if (Source.Handle.ToInt64() <= 0)
                {
                    throw new Microsoft.Test.TestValidationException("Window is not created");
                }
            }
            
            using (CoreLogger.AutoStatus("Dispather.Run"))
            {                            
                Dispatcher.Run();
            }

            

            using (CoreLogger.AutoStatus("Validating Handle on the HwndSource"))
            {            
                if (!Source.IsDisposed)
                {
                    throw new Microsoft.Test.TestValidationException("The Handler of HwndSource should be an invalid window");
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
            HwndSource Source = (HwndSource)o;
            bool IsWindowDestroy = false;
            using(CoreLogger.AutoStatus("Call Win32.DestroyWindow and Stop the dispatcher"))
            {
                Source.RootVisual = null;
                IsWindowDestroy = Microsoft.Test.Win32.NativeMethods.DestroyWindow( new HandleRef(null,Source.Handle) )  ;
                using (CoreLogger.AutoStatus("Validating Window is destroyed"))
                {            
                    if (!IsWindowDestroy)
                    {
                        throw new Microsoft.Test.TestValidationException("The window should be destroyed");
                    }
                }            
                MainDispatcher.InvokeShutdown();
            }
            return null;
        }


        /// <summary>
        /// This is called when OnRender on HelloElement is called.  We post a CloseWindowAsyncHandler to be executed on the Context
        /// </summary>
        /// <param name="target"></param>
        /// <param name="Source"></param>
        private void Hello_RenderdSourcedHandlerEvent(UIElement target, HwndSource Source)
        {
            using(CoreLogger.AutoStatus("BeginInvoke on Background Priority to Close the Window"))
            {
                target.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(CloseWindowASyncHandler), Source);
            }

        }
    }
}


