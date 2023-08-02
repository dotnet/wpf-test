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
using Microsoft.Test.Discovery;
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
    public class MultipleContextMultipleDispatcherMultiple : TestCase
    {
        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public MultipleContextMultipleDispatcherMultiple() :base(TestCaseType.None)
        {

        }
        
        /// <summary>
        /// Creating 4 Threads: each thread creates it own dispatcher , later creates a canvas and button. Validate everything run fine.
        /// </summary>
        /// <remarks>
	    ///     <filename>MultipleContextMultipleDispatcherMultiple.cs</filename>
        /// </remarks>
        [Test(0, @"Source\Multiple\Simple", TestCaseSecurityLevel.FullTrust, "MultipleContextMultipleDispatcher", Area = "AppModel")]
        public override void Run()
        {
            CoreLogger.BeginVariation();
            s_ev = new CoreAutoResetEvent(false, 4);

            Thread t1,t2,t3,t4;
            
            using (CoreLogger.AutoStatus("Creating 4 Threads"))
            {
                 t1 = new Thread(new ThreadStart(CreateCase));
                t1.SetApartmentState(ApartmentState.STA);
                t1.Name="T1";
                 t2 = new Thread(new ThreadStart(CreateCase));
                t2.SetApartmentState(ApartmentState.STA);
                t2.Name="T2";
                 t3 = new Thread(new ThreadStart(CreateCase));
                t3.SetApartmentState(ApartmentState.STA);
                t3.Name="T3";
                 t4 = new Thread(new ThreadStart(CreateCase));
                t4.SetApartmentState(ApartmentState.STA);
                t4.Name="T4";
            }

            CoreLogger.LogStatus("Starting Threads");
            t1.Start();
            t2.Start();
            t3.Start();
            t4.Start();

            s_ev.WaitOne();

            if (ExceptionList.Count != 0)
            {
                for (int i=0;i<ExceptionList.Count;i++)
                {
                    Exception e= ExceptionList[i] as Exception;
                    using (CoreLogger.AutoStatus("*** Exception Thread: " + i.ToString()))
                    {
                        CoreLogger.LogStatus(e.Message.ToString());
                        CoreLogger.LogStatus(e.StackTrace.ToString());
                    }
                }

                throw (Exception)ExceptionList[0];
            }
            CoreLogger.EndVariation();        
        }

        ///<summary>
        ///</summary>
        public void CreateCase()
        {
	
            //Dispatcher dispatcher;
            HwndSource Source = null;



            try
            {
    
                    Source = new HwndSource(0,NativeConstants.WS_VISIBLE | NativeConstants.WS_OVERLAPPEDWINDOW | NativeConstants.WS_CLIPCHILDREN, 0, 25, 25, 250, 250,"Main Window Test", IntPtr.Zero);
    

                    Canvas canvas = new Canvas();
                    Button button = new Button();
                    button.Content="Button" + Thread.CurrentThread.Name;
                    canvas.Children.Add(button);

                    HelloElement hello = new HelloElement();
                    hello.Source = Source;
                    canvas.Children.Add(hello);

                    hello.RenderedSourcedHandlerEvent += new HelloElement.RenderHandler(RenderHandler);
                    Source.RootVisual = canvas;
               
                Dispatcher.Run();

            }            
            catch(Exception e)
            {
                ExceptionList.Add(e);
            }

            try
            {
            
                if (Source != null && !Source.IsDisposed)
                    Source.Dispose();

            }
            catch(Exception e)
            {
                ExceptionList.Add(e);
            }
               
             s_ev.Set();

        }



        void RenderHandler (UIElement target, HwndSource Source)
        {

		DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.SystemIdle);
		timer.Tick += new EventHandler(ExitHandler);
		timer.Interval = TimeSpan.FromSeconds(3);
		timer.Start();

        }

        void ExitHandler(object o, EventArgs args)
        {
	    DispatcherTimer timer = (DispatcherTimer)o;
	    timer.Stop();
            Microsoft.Test.Threading.DispatcherHelper.ShutDown();
        }
        static CoreAutoResetEvent s_ev = null;

    }
        
 }








