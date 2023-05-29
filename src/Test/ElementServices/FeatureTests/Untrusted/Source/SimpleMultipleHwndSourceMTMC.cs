// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Win32;


namespace Avalon.Test.CoreUI.Source.Hwnd
{
    /// <summary>
    ///     
    ///     Create 5 Threads that Each one will be create a Dispatcher, HwndDispatcher and a HwndSource
    ///     This will Destroy the window and Validate the Windows Creation and Destruction
    ///</summary>
    /// <remarks>
    ///         <ol>Scenarios steps:
    ///         <li>The main thread creates 5 thread that it will execute these:</li>
    ///             <li>Creating 1 context and Enter the context</li>
    ///             <li>Create a HelloElement to be rendered. Also we add an event to be called when the OnRender is executed</li>
    ///             <li>Create an HwndSource and Validate that creation</li>
    ///             <li>Run the Dispatcher</li>
    ///             <li>HelloElement.OnRender will be executed and the renderEvent(testing event) will be called this will
    ///             post an item to Close the window and stop the dispatcher</li>
    ///             <li>Validate the window is destroyed</li>
    ///         <li>The main thread wait until everything is done and validate that there was no exception</li>
    ///         </ol>
    ///     <Owner>Microsoft</Owner>
 
    ///     <Area>Source\MultpleThreadSingleContext</Area>
    ///     <location>SimpleMultipleHwndSourceMTMC.cs</location>
    /// </remarks>
    [Test(0, @"Source\Complex", TestCaseSecurityLevel.FullTrust, "SimpleMultipleHwndSourceMTMC", Area = "AppModel")]
    public class SimpleMultipleHwndSourceMTMC : TestCase
    {
        
        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public SimpleMultipleHwndSourceMTMC() :base(TestCaseType.None){}
        
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        override public void Run()
        {
            s_dispatchers = Hashtable.Synchronized(new Hashtable(3));

            s_evCore = new CoreAutoResetEvent(false, 5);
            
            CoreLogger.LogStatus("Creating 5 threads...");
            Thread T1 = new Thread(new ThreadStart(xRun));
            T1.SetApartmentState(ApartmentState.STA);
            T1.Name = "Thread 1";
            
            Thread T2 = new Thread(new ThreadStart(xRun));
            T2.SetApartmentState(ApartmentState.STA);
            T2.Name = "Thread 2";
            
            Thread T3 = new Thread(new ThreadStart(xRun));
            T3.SetApartmentState(ApartmentState.STA);
            T3.Name = "Thread 3";
            
            Thread T4 = new Thread(new ThreadStart(xRun));
            T4.SetApartmentState(ApartmentState.STA);
            T4.Name = "Thread 4";
            
            Thread T5 = new Thread(new ThreadStart(xRun));
            T5.SetApartmentState(ApartmentState.STA);
            T5.Name = "Thread 5";

            T1.Start();
            T3.Start();
            
            T2.Start();
            T5.Start();
            T4.Start();

            CoreLogger.LogStatus("Main Thread waiting for all the threads to finish...");
            s_evCore.WaitOne();

            using(CoreLogger.AutoStatus("Validation"))
            {
                if (ExceptionList.Count != 0)
                {
                    throw (Exception)ExceptionList[0];
                }
            }
        }

        void xRun()
        {
            HelloElement Hello;
            Dispatcher ContextRef;
            HwndSource Source;
            HwndDispatcher Dispatcher = null;
            
            try 
            {
                using (CoreLogger.AutoStatus("Holding Reference Context and Entering Thread:" + Thread.CurrentThread.Name ))
                {
                    Dispatcher = new HwndDispatcher();
                    ContextRef = new Dispatcher(); 
                    ContextList.Add(ContextRef);
                    Dispatcher.RegisterContext(ContextRef);
                    s_dispatchers.Add(ContextRef,Dispatcher);
                    ContextRef.Enter();
                }

                using (CoreLogger.AutoStatus("Creating a HelloElement to render Thread:" + Thread.CurrentThread.Name ))
                {
                    Hello = new HelloElement();
                    Hello.RenderedSourcedHandlerEvent +=new Avalon.Test.CoreUI.Source.HelloElement.RenderHandler(Hello_RenderdSourcedHandlerEvent);
                }

                using (CoreLogger.AutoStatus("Creating a HwndSource Thread:" + Thread.CurrentThread.Name ))
                {
                    Source = new HwndSource(0,NativeConstants.WS_VISIBLE, 0, 25, 25, 400, 400,"Test Window", IntPtr.Zero);
                    Hello.Source = Source;
                    Source.RootVisual = Hello;
                }
            
                using (CoreLogger.AutoStatus("Validating the Handle is valid Thread:" + Thread.CurrentThread.Name ))
                {            
                    if (Source.Handle.ToInt64() <= 0)
                    {
                        throw new Microsoft.Test.TestValidationException("Window is not created");
                    }
                }
            
                using (CoreLogger.AutoStatus("Dispather.Run Thread:" + Thread.CurrentThread.Name ))
                {            
                    ContextRef.Exit();
                    
                    try
                    {
                        Dispatcher.Run();
                    }
                    catch(Exception e)
                    {
                        Dispatcher.Quit();
                        throw e;
                    }
                }


                using (CoreLogger.AutoStatus("Validating Handle on the HwndSource Thread:" + Thread.CurrentThread.Name ))
                {            
                    if (!Source.IsDisposed)
                    {
                        throw new Microsoft.Test.TestValidationException("The Handler of HwndSource should be an invalid window");
                    }
                }
            }
            catch(Exception e)
            {
                ExceptionList.Add(e);
                
            }
            finally
            {
                s_evCore.Set();
            }
        
        }   
        
        /// <summary>
        /// This handler will be posted to the Context to Close the windows and Stop the dispatcher
        /// </summary>
        /// <param name="o"></param>
        object CloseWindowASyncHandler(object o)
        {
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
                
                //((HwndDispatcher)Dispatchers[Dispatcher.CurrentDispatcher]).Quit().
                Microsoft.Test.Threading.DispatcherHelper.ShutDown();
            }
            return null;
        }


        private void Hello_RenderdSourcedHandlerEvent(UIElement target, HwndSource Source)
        {
            using(CoreLogger.AutoStatus("BeginInvoke on Background Priority to Close the Window Thread:" + Thread.CurrentThread.Name))
            {
                target.Context.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(CloseWindowASyncHandler), Source);
            }
        }

        /// <summary>
        /// Use to sync the threads, It will wait until the 5 threas ends to continue the main thread
        /// </summary>
        static CoreAutoResetEvent s_evCore = null;

        /// <summary>
        /// Holds a referece for the Dispatcher on the different contexts
        /// </summary>
        static Hashtable s_dispatchers;
    }


}


