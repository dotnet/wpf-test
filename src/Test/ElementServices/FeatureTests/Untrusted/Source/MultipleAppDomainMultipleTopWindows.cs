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
using Microsoft.Test.Discovery;
using Microsoft.Test.Threading;

namespace Avalon.Test.CoreUI.Source.Hwnd
{
    ///<summary>
    ///</summary>
    ///<remarks>
    ///     <filename>RegisterTwiceSameContext.cs</filename>
    ///</remarks>
    [TestDefaults]
    public class MultipleAppDomainMultipleTopWindows 
    {
        

        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public MultipleAppDomainMultipleTopWindows()
        {

        }
        
        /// <summary>
        /// Creating 4 Top Level HwndSource on a different AppDomans. Each AppDomain contains its own Dispatcher
        /// and its own Context.  This test creates the four threads and the main Thread waits until the 4 are done
        /// The exit criteria for the threads is that something is OnRender is called.
        /// </summary>
        [Test(0, @"Source\AppDomain\Multiple\Source\Windows", "MultipleAppDomainMultipleTopWindows", Area = "AppModel")]
        public void Run()
        {
            CoreLogger.BeginVariation();
            s_ev = new CoreAutoResetEvent(false, 4);
            
            using (CoreLogger.AutoStatus("Creating 4 Threads"))
            {
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
            }

            CoreLogger.LogStatus("Starting Threads");
            _t1.Start();
            _t2.Start();
            _t3.Start();
            _t4.Start();

            s_ev.WaitOne();


            if (s_exceptionInternalList.Count > 0)
            {
                CoreLogger.LogStatus("Exception List:");
                for (int i=0; i < s_exceptionInternalList.Count; i++)
                {
                    CoreLogger.LogStatus(((Exception)s_exceptionInternalList[i]).Message);
                    CoreLogger.LogStatus(((Exception)s_exceptionInternalList[i]).StackTrace.ToString());
                    CoreLogger.LogStatus("");
                }

                throw s_exceptionInternalList[0] as Exception;
            }
            CoreLogger.EndVariation();

        }


        ///<summary>
        ///</summary>
        public void StartTestCaseCrossAppDomain()
        {
            
            AppDomain appTest = AppDomain.CreateDomain("TestDomain",new System.Security.Policy.Evidence(AppDomain.CurrentDomain.Evidence));


            if (Thread.CurrentThread == _t1)
                appTest.SetData("position",new CrossAppPosition(10,10));

            if (Thread.CurrentThread == _t2)
                appTest.SetData("position",new CrossAppPosition(10,340));

            if (Thread.CurrentThread == _t3)
                appTest.SetData("position",new CrossAppPosition(340,340));
    
            if (Thread.CurrentThread == _t4)
                appTest.SetData("position",new CrossAppPosition(340,10));


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
                s_ev.Set();
            }
            
        }


        ///<summary>
        ///</summary>
        static public void CreateCase()
        {
            // REMOVING THIS DUE SECURITY CLEAN.. WE NEED TO ADDED BACK
            //System.Security.PermissionSet set = System.Windows.TrustManagement.TrustManager.GetDefaultPermissions().
            //set.PermitOnly().


            HwndSource source = null;

            CrossAppPosition position = (CrossAppPosition)AppDomain.CurrentDomain.GetData("position");
            int x = position.Left;
            int y = position.Top;

            try
            {
    

                source = SourceHelper.CreateHwndSource( 250, 250,x,y);
                
                Canvas canvas = new Canvas();
                Button button = new Button();
                button.Content="Button" + Thread.CurrentThread.Name;
                canvas.Children.Add(button);

                HelloElement hello = new HelloElement();
                hello.Source = source;
                canvas.Children.Add(hello);

                hello.RenderedSourcedHandlerEvent += new HelloElement.RenderHandler(_renderHandler);

                source.RootVisual = canvas;

                Dispatcher.Run();

                source.Dispose();

            }            
            finally{}
            

        }



        static void _renderHandler (UIElement target, HwndSource Source)
        {
            ThreadingHelper.DispatcherTimerHelper(DispatcherPriority.SystemIdle, new TimeSpan(0,0,3), new EventHandler (_exitHandler), Source);
            
        }

        static void  _exitHandler(object o, EventArgs args)
        {

            Microsoft.Test.Threading.DispatcherHelper.ShutDown();

        }
        
        static CoreAutoResetEvent s_ev = null;
        static ArrayList s_exceptionInternalList = ArrayList.Synchronized(new ArrayList());
        Thread _t1,_t2,_t3,_t4;


    }
        
 }

