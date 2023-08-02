// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading; 
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.Framework.Dispatchers;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;
using Microsoft.Test.Win32;


namespace Avalon.Test.Framework.Dispatchers.Registration
{
    ///<summary>
    ///</summary>
    ///<remarks>
    ///     <filename>DispatchingMultipleDispatchersMultipleContext.cs</filename>
    ///</remarks>
    [Test(0, "Threading", TestCaseSecurityLevel.FullTrust, "DispatchingMultipleDispatchersMultipleContext")]
    public class DispatchingMultipleDispatchersMultipleContext : TestCase
    {
        #region Private Data
        [ThreadStatic]
        static int t_counter = 0;
        [ThreadStatic]
        static private Dispatcher t_dispatcher = null;
        private static ManualResetEvent s_ev = null;
        #endregion


        #region Constructor
        /******************************************************************************
        * Function:          DispatchingMultipleDispatchersMultipleContext Constructor
        ******************************************************************************/
        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public DispatchingMultipleDispatchersMultipleContext() :base(TestCaseType.None)
        {
            RunSteps += new TestStep(Dispatching);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          Dispatching
        ******************************************************************************/
        /// <summary>
        ///  Create a Window with a button, click on the button and create a modal dialog, later click onteh modal dialog and to close the windows
        /// </summary>
        /// <remarks>
        ///  <ol>Description Steps:
        ///  </ol>
        /// </remarks>
        /// <summary>
        /// Spwan 5 threads, each thread Register 10 Dispatcher on Win32Dispatcher. Posting 20 items to each Context to be dispatched, all on BG priority,
        /// on the middle of an item dispatched we unregister the dispatched
        /// </summary>
        /// <remarks>
        ///  <ol>Description Steps:
        ///  <li>Create 5 Threads</li>
        ///     <li>on each thread</li>
        ///          <li>Create 10 Context and Register on 1 Dispatcher</li>
        ///          <li>Post an Item to post the Items async</li>
        ///          <li>Dispatcher.Run</li>
        ///          <li>Posting 20 items on BG to each context. on 1 context post a exit on Idle</li>
        ///          <li>During the items are been dispatched, if it is the first context and it is the 10 items dispatche we unregister the context</li>
        ///          <li>During the items are been dispatched, if it is the fifth context and it is the 10 items dispatche we unregister the context</li>
        ///          <li>The dispatcher is stopped</li>
        ///          <li>Validate 180 items are dispatched.</li>
        ///      <li>Validate  just that there is no expection on any thread</li>        
        ///  </ol>
        ///     <filename>DispatchingMultipleDispatchersMultipleContext.cs</filename>
        /// </remarks>
        TestResult Dispatching()
        {
            s_ev = new ManualResetEvent(false);

            Thread t1;
            Thread t2;
            Thread t3;
            Thread t4;
            Thread t5;

            using (CoreLogger.AutoStatus("Creating 5 STA Threads"))
            {
            
                t1 = new Thread(new ThreadStart(MultipleContextDispatching));
                t1.SetApartmentState(ApartmentState.STA);


                t2 = new Thread(new ThreadStart(MultipleContextDispatching));
                t2.SetApartmentState(ApartmentState.STA);


                t3 = new Thread(new ThreadStart(MultipleContextDispatching));
                t3.SetApartmentState(ApartmentState.STA);


                t4 = new Thread(new ThreadStart(MultipleContextDispatching));
                t4.SetApartmentState(ApartmentState.STA);

                t5 = new Thread(new ThreadStart(MultipleContextDispatching));
                t5.SetApartmentState(ApartmentState.STA);
            }


            using (CoreLogger.AutoStatus("Starting Threads"))
            {
                t1.Start();
                t2.Start();
                t3.Start();
                t4.Start();
                t5.Start();
            }


            CoreLogger.LogStatus("Signal the Threads to Start");
            s_ev.Set();

            t1.Join();
            t2.Join();
            t3.Join();
            t4.Join();
            t5.Join();


            FinalReportFailure();

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Private Members
        private void MultipleContextDispatching()
        {
            s_ev.WaitOne();

            try
            {        
                using (CoreLogger.AutoStatus("Creating an Dispatcher"))
                {
                    t_dispatcher = Dispatcher.CurrentDispatcher;
                }

                using (CoreLogger.AutoStatus("Validate all the context belong to that dispatcher"))
                {
                    t_dispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback(PostItems),null);
                }

                using (CoreLogger.AutoStatus("Dispatcher.Run"))
                {
                    DispatcherHelper.RunDispatcher();
                }

                if (t_counter != 20)
                    throw new Microsoft.Test.TestValidationException("Not the expected items dispatched. Waiting: 20 Actual:"+ t_counter.ToString());

            }
            catch(Exception e)
            {
                TestCase.ExceptionList.Add(e);
            }
            finally
            {

            }
        }


        private object PostItems(object o)
        {
            int count=0;

            using (CoreLogger.AutoStatus("Posting Working items to all the context"))
            {
                for (int i=0;i<20;i++, count++)
                {
                      t_dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(_Handler), i);
                }


                 t_dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback(_ExitHandler), null);
            }            
            
            return null;
        }

        private object _Handler(object o)
        {
            t_counter++;
                                       
            return null;
        }


        private object _ExitHandler(object o)
        {
            using (CoreLogger.AutoStatus("Quit Dispatcher"))
            {
                Microsoft.Test.Threading.DispatcherHelper.ShutDown();
            }
            return null;
        }
        #endregion
    }
 }






