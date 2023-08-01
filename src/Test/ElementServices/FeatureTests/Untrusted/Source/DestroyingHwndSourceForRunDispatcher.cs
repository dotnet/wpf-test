// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Threading; 
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Win32;


namespace Avalon.Test.CoreUI.Source.SimpleThreadSingleContext
{
    /// <summary>
    ///     Destroying the HwndSource before the Dispatcher start running, and the RootVisual was se
    /// </summary>
    /// <remarks>
    ///     <ol>Scenarios steps:
    ///         <li>Creating 1 context and Enter the context</li>
    ///         <li>Creating a HelloElement to render something on the screen</li>
    ///         <li>Create a HwndSource and set the RootVisual to the HwndSource</li>
    ///         <li>Destroy the Window from the HwndSource using Win32.DestroyWindow nad Post an item to stop the dispatcher </li>
    ///         <li>Start Running the dispatcher</li>
    ///     </ol>
    ///     
    ///     <location>DestroyingHwndSourceForRunDispatcher.cs</location>
    /// </remarks>
    [TestAttribute(1, @"Source\Destroy", TestCaseSecurityLevel.FullTrust, "DestroyingHwndSourceForRunDispatcher", MethodName = "RunTest", Area = "AppModel")]
    public class DestroyingHwndSourceForRunDispatcher : TestCase
    {
        
        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public DestroyingHwndSourceForRunDispatcher() :base(TestCaseType.ContextSupport){}
        
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        override public void Run()
        {
            CoreLogger.BeginVariation();
            HwndSource Source;            

            using (CoreLogger.AutoStatus("Holding Reference Context and Entering"))
            {

                Source = (HwndSource)_enterContextOne(null);
            }

            
            using (CoreLogger.AutoStatus("Dispather.Run"))
            {            
                
                Dispatcher.Run();

            }
            CoreLogger.EndVariation();       
        }


        object _enterContextOne(object o)
        {
            HwndSource source;
            HelloElement Hello;
            
            using (CoreLogger.AutoStatus("Creating a HelloElement to render"))
            {
                Hello = new HelloElement();
             
            }

            using (CoreLogger.AutoStatus("Creating a HwndSource"))
            {
                source = SourceHelper.CreateHwndSource( 400, 400,0,0);
                Hello.Source = source;
                source.RootVisual = Hello;
            }
            
            using (CoreLogger.AutoStatus("Destroy the Window"))
            {            
                _CloseWindowASyncHandler(source);
            }
            
            using (CoreLogger.AutoStatus("Posting to Stop the Dispatcher"))
            {                        
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback(_StopDispatcher),Dispatcher.CurrentDispatcher);
            }

            return source;
           
        }
        
        /// <summary>
        /// This handler will be posted to the Context to Close the windows and Stop the dispatcher
        /// </summary>
        /// <param name="o"></param>
        object _CloseWindowASyncHandler(object o)
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
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        object _StopDispatcher(object o)
        {
            Microsoft.Test.Threading.DispatcherHelper.ShutDown();
            return null;
        }
        
    }
}


