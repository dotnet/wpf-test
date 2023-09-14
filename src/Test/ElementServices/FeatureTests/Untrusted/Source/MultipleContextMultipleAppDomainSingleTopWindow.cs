// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Threading;

using Avalon.Test.CoreUI.Trusted;
//using Avalon.Test.Framework.Dispatchers;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Threading;
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
    public class MultipleContextMultipleAppDomainSingleTopWindow 
    {
        

        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public MultipleContextMultipleAppDomainSingleTopWindow() 
        {

        }
        
        static CrossAppData s_data;

        
        /// <summary>
        /// Create a top level HWND, that HWND will contain 4 child HwndSource. Each HwndSource is created on a different
        /// Thread and on a different AppDomain. For that reason we create for each a new HwndDispatcher and a Dispatcher
        /// After each HwndSource start rendering it post a Quit to the dispatcher an later signel the main thread to say
        /// that that thread is done.
        /// </summary>
        [Test(0, @"Source\AppDomain\Multiple\Source\Windows", TestCaseSecurityLevel.FullTrust, "MultipleAppDomainMultipleHwndSource", Area = "AppModel")]
        public void Run()
        {
            CoreLogger.BeginVariation();
            HwndWrapper hwnd = new HwndWrapper(
                0,
                NativeConstants.WS_VISIBLE | NativeConstants.WS_OVERLAPPEDWINDOW | NativeConstants.WS_CLIPCHILDREN,
                0,
                0,
                0,
                600,
                600,
                "Main Window",
                IntPtr.Zero,
                null);
                
            s_data = new CrossAppData(hwnd.Handle);
                
            s_ev = new CoreAutoResetEvent(false, 4);

            ThreadingHelper.BeginInvokeOnSignalHandle(s_ev.AutoEvent, Dispatcher.CurrentDispatcher, DispatcherPriority.SystemIdle, new DispatcherOperationCallback(_exitItem) , null);
            s_dispatcher = Dispatcher.CurrentDispatcher;
       
            _t1 = new Thread(new ThreadStart(StartTestCaseCrossAppDomain));
            _t1.SetApartmentState(ApartmentState.STA);
            _t1.Name="T1";
            _t2 = new Thread(new ThreadStart(StartTestCaseCrossAppDomain));
            _t2.SetApartmentState(ApartmentState.STA);
            _t2.Name="T2";
            _t3 = new Thread(new ThreadStart(StartTestCaseCrossAppDomain));
            _t3.SetApartmentState(ApartmentState.STA);
            _t3.Name="T3";
            _t4 = new Thread(new ThreadStart(StartTestCaseCrossAppDomain));
            _t4.SetApartmentState(ApartmentState.STA);
            _t4.Name="T4";
        

            CoreLogger.LogStatus("Starting Threads");
            _t1.Start();
            _t2.Start();
            _t3.Start();
            _t4.Start();

            Dispatcher.Run();


            hwnd.Dispose();

            if (s_exceptionInternalList.Count > 0)
            {
                CoreLogger.LogStatus("Exception List:");
                for (int i=0; i < s_exceptionInternalList.Count; i++)
                {
                    CoreLogger.LogStatus(((Exception)s_exceptionInternalList[i]).Message);
                    CoreLogger.LogStatus(((Exception)s_exceptionInternalList[i]).StackTrace.ToString());
                    CoreLogger.LogStatus("");
                }

                throw  s_exceptionInternalList[0] as Exception;
            }
            CoreLogger.EndVariation();
        }

        static Dispatcher s_dispatcher = null;

        object _exitItem(object o)
        {
            Microsoft.Test.Threading.DispatcherHelper.ShutDown();
            return null;
        }


        Thread _t1,_t2,_t3,_t4;
        
        ///<summary>
        ///</summary>
        public void StartTestCaseCrossAppDomain()
        {
            
            AppDomain appTest = AppDomain.CreateDomain("TestDomain");
            CrossAppData data = new CrossAppData(s_data);

            appTest.SetData("handler", data);

            if (Thread.CurrentThread == _t1)
                appTest.SetData("position",new CrossAppPosition(10,10));

            if (Thread.CurrentThread == _t2)
                appTest.SetData("position",new CrossAppPosition(10,240));

            if (Thread.CurrentThread == _t3)
                appTest.SetData("position",new CrossAppPosition(240,240));
    
            if (Thread.CurrentThread == _t4)
                appTest.SetData("position",new CrossAppPosition(240,10));
                        
            try
            {
                appTest.DoCallBack(new CrossAppDomainDelegate(CreateCase));
            }
            catch(Exception e)
            {
                s_exceptionInternalList.Add(e);
            }
            finally
            {
                s_dispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback(_signalEvent), null);
            }
        }

        object _signalEvent (object o)
        {
            s_ev.Set();
            return null;
        }




        ///<summary>
        ///</summary>
        static public void CreateCase()
        {
           
            // REMOVING THIS DUE SECURITY CLEAN.. WE NEED TO ADDED BACK
            //System.Security.PermissionSet set = System.Windows.TrustManagement.TrustManager.GetDefaultPermissions().
            //set.PermitOnly().



            HwndSource source = null;

            try
            {
                CrossAppData data = (CrossAppData)AppDomain.CurrentDomain.GetData("handler");
                CrossAppPosition position = (CrossAppPosition)AppDomain.CurrentDomain.GetData("position");
                int x = position.Left;
                int y = position.Top;
                IntPtr pointer = data.Handler;



                source = SourceHelper.CreateHwndSource( 200, 200, x, y, pointer);
                


                Canvas canvas = new Canvas();
                Button button = new Button();

                button.Content = "Button" + Thread.CurrentThread.Name;
                canvas.Children.Add(button);

                HelloElement hello = new HelloElement();

                hello.Source = source;
                canvas.Children.Add(hello);

                hello.RenderedSourcedHandlerEvent += new HelloElement.RenderHandler(_renderHandler);
                
                source.RootVisual = canvas;

                Dispatcher.Run();               


                source.Dispose();

                
                
            }
            finally
            {

            }
        }



        static void _renderHandler (UIElement target, HwndSource Source)
        {
            ThreadingHelper.DispatcherTimerHelper(DispatcherPriority.SystemIdle, target.Dispatcher,new TimeSpan(0,0,3), new EventHandler (_exitHandler),Source);
            
        }

        static void _exitHandler(object o, EventArgs args)
        {
            Microsoft.Test.Threading.DispatcherHelper.ShutDown();

        }
        
        static CoreAutoResetEvent s_ev = null;
        static ArrayList s_exceptionInternalList = ArrayList.Synchronized(new ArrayList());

    }
        
    ///<summary>
    ///</summary>
    public class CrossAppData : MarshalByRefObject
    {

        ///<summary>
        ///</summary>
        public CrossAppData(IntPtr pointer)
        {
            this.Handler = pointer;
        }


        ///<summary>
        ///</summary>
        public CrossAppData(CrossAppData data)
        {
            this.Handler = data.Handler;
        }


        ///<summary>
        ///</summary>
        public IntPtr Handler;
    }


    ///<summary>
    ///</summary>
    public class CrossAppPosition : MarshalByRefObject
    {

        ///<summary>
        ///</summary>
        public CrossAppPosition(int top, int left)
        {
            Top = top;
            Left = left;
        }


        ///<summary>
        ///</summary>
        public CrossAppPosition(CrossAppPosition data)
        {
            this.Top = data.Top ;
            this.Left = data.Left;
        }


        ///<summary>
        ///</summary>
        public int Top;

        ///<summary>
        ///</summary>
        public int Left;

    }



 }






