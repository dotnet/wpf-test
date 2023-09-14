// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;
//using Avalon.Test.Framework.Dispatchers;

using Microsoft.Test;
using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.Source.Hwnd
{
    ///<summary>
    ///</summary>
    ///<remarks>
    ///     <filename>RegisterTwiceSameContext.cs</filename>
    ///</remarks>
    //[CoreTestsLoader(CoreTestsTestType.MethodBase)]    
    [TestDefaults]
    public class MultipleWindowMultipleContextOneDispatcher : TestCase
    {
        

        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public MultipleWindowMultipleContextOneDispatcher() :base(TestCaseType.None)
        {

        }
        


        /// <summary>
        /// Creating 4 Threads: each thread creates it own dispatcher and context, later creates a canvas and button. Validate everything run fine.
        /// </summary>
        /// <remarks>
        ///  <ol>Description Steps:
        ///     <li>Spwan 4 Threads and wait for all the thread finished their work.</li>
        ///     <li>Each thread: Create a context and a Win32Dispatcher .</li>
        ///     <li>Each Thread: Create a HwndSource and a CAnvas, Button and Hello Element</li>
        ///     <li>When HelloElement is rendered, we post a message to exit a dispatcher en 2 seconds</li>
        ///     <li>Validating that there is no expecption on any thread. and exit the app</li>
        ///  </ol>
	    ///     <filename>MultipleContextMultipleDispatcherMultiple.cs</filename>
        /// </remarks>
        [Test(0, @"Source\Multiple\SimpleTwo", "MultipleWindowMultipleContextOneDispatcher", Area = "AppModel")]
        public override void Run()
        {

            CoreAutoResetEvent ev = new CoreAutoResetEvent(false, 4);

            Thread t1,t2,t3,t4;
            
            using (CoreLogger.AutoStatus("Creating 4 Threads"))
            {
                 t1 = new Thread(new ThreadStart(CreateCase));
                t1.SetApartmentState(ApartmentState.STA);
                 t2 = new Thread(new ThreadStart(CreateCase));
                t2.SetApartmentState(ApartmentState.STA);
                 t3 = new Thread(new ThreadStart(CreateCase));
                t3.SetApartmentState(ApartmentState.STA);
                 t4 = new Thread(new ThreadStart(CreateCase));
                t4.SetApartmentState(ApartmentState.STA);
            }

            MainDispatcher = Dispatcher.CurrentDispatcher;


            CoreLogger.LogStatus("Starting Threads");
            t1.Start();
            t2.Start();
            t3.Start();
            t4.Start();


            ExitDispatcheronTimeout(TimeSpan.FromSeconds(20), false);
                      


            Dispatcher.Run();

            FinalReportFailure();
                        
        }

       
        ///<summary>
        ///</summary>
        public void CreateCase()
        {

            Thread.Sleep(0);
            
            HwndSource Source = null;
            
            try
            {
    

                MainDispatcher.Invoke(DispatcherPriority.Normal,
                    (DispatcherOperationCallback)delegate(object arg)
                    {
                        Source = new HwndSource(0,NativeConstants.WS_VISIBLE | NativeConstants.WS_OVERLAPPEDWINDOW | NativeConstants.WS_CLIPCHILDREN, 0, 25, 25, 500, 500,"Main Window Test", IntPtr.Zero);
                        return null;
                    }, null);


                MainDispatcher.Invoke(DispatcherPriority.Normal,
                    (DispatcherOperationCallback)delegate(object arg)
                    {
                        Canvas canvas = new Canvas();
                        Button button = new Button();
                        canvas.Children.Add(button);

                        HelloElement hello = new HelloElement();
                        hello.Source = Source;
                        canvas.Children.Add(hello);

                        hello.RenderedSourcedHandlerEvent += new HelloElement.RenderHandler(renderHandler);
                        return null;
                    }, null);

                
                MainDispatcher.Invoke(DispatcherPriority.Normal,
                    (DispatcherOperationCallback)delegate(object arg)
                    {
                        Source.RootVisual = canvas;
                        return null;
                    }, null);

            }            
            catch(Exception e)
            {
                ExceptionList.Add(e);
            }

            try
            {

                MainDispatcher.Invoke(DispatcherPriority.Normal,
                    (DispatcherOperationCallback)delegate(object arg)
                    {

                        if (Source != null && !Source.IsDisposed)
                            Source.Dispose();
                        return null;
                    }, null);

            }
            catch(Exception e)
            {
                ExceptionList.Add(e);
            }

        }


        void renderHandler (UIElement target, HwndSource Source)
        {

            DispatcherTimer dTimer = new DispatcherTimer(DispatcherPriority.Normal);
            dTimer.Interval = TimeSpan.FromSeconds(2);
            dTimer.Tick += new EventHandler(exiting);
            dTimer.Start();            
        }

        void exiting(object o, EventArgs args)
        {
            lock(s_rootSync)
            {               
                s_count++; 
                if (s_count == 4)
                    MainDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
            }

        }

        static object s_rootSync = new Object();
        static int s_count = 0;

    }
        
 }









