// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.Framework.Dispatchers;

using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;


namespace Avalon.Test.Framework.Dispatchers.Registration
{
    ///<summary>
    ///</summary>
    ///<remarks>
    ///     <filename>ScheduleAbortSimple.cs</filename>
    ///</remarks>
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]    
    public class DispatchingOneDispatcherMultipleContext : TestCase
    {
        
        /// <summary>
        /// This is not used.
        /// </summary>
                public override void Run()
        {
            string Parameter = TestCaseInfo.GetCurrentInfo().Params;

             if (Parameter == "MultipleContextDispatching")
                MultipleContextDispatching();                                       
        }

        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public DispatchingOneDispatcherMultipleContext() :base(TestCaseType.None)
        {

        }
        

        /// <summary>
        /// Register 10 Dispatcher on Win32Dispatcher. Posting 20 items to each Context to be dispatched, all on BG priority,
        /// on the middl of an item dispatched we unregister the dispatched
        /// </summary>
        /// <remarks>
        ///  <ol>Description Steps:
        ///      <li>Create a Win32Dispatcher</li>
        ///      <li>Create 10 Context and Register on 1 Dispatcher</li>
        ///      <li>Post an Item to post the Items async</li>
        ///      <li>Dispatcher.Run</li>
        ///      <li>Posting 20 items on BG to each context. on 1 context post a exit on Idle</li>
        ///      <li>During the items are been dispatched, if it is the first context and it is the 10 items dispatche we unregister the context</li>
        ///      <li>During the items are been dispatched, if it is the fifth context and it is the 10 items dispatche we unregister the context</li>
        ///      <li>The dispatcher is stopped</li>
        ///      <li>Validate 180 items are dispatched.</li>
        ///  </ol>
     ///     <filename>DispatchingOneDispatcherMultipleContext.cs</filename>
        /// </remarks>
        [CoreTestsLoader(CoreTestsTestType.MethodBase)]
        [TestCasePriority("0")]
        [TestCaseArea(@"Dispatcher\Registration\Simple")]
        [TestCaseMethod("RunTest")]
        [TestCaseParams("MultipleContextDispatching")]
        public void MultipleContextDispatching()
        {
     

            using ()
            {
                _uiDispatcher = new Win32Dispatcher();
            }

        Win32Dispatcher Dispatcher = _uiDispatcher as Win32Dispatcher;

            using ()
            {
            
                RegisterContextOnDispatcher(_uiDispatcher, 10);
            }

            using ()
            {
                Dispatcher tempContext = ContextList[1] as Dispatcher;
                tempContext.BeginInvoke(new DispatcherOperationCallback(PostItems),null);
            }

            using ()
            {
                Dispatcher.Run();
            }

            if (s_counter != 180)
                throw new Microsoft.Test.TestValidationException("Not the expected items dispatched. Waiting: 180 Actual:"+ s_counter.ToString());
    
        }


        void RegisterContextOnDispatcher(UIDispatcher Dispatcher, int NoContext)
        {

            using ()
            {
                for (int i=0; i<NoContext; i++)
                    ContextList.Add(new Dispatcher());
            }        

            using ()
            {
                for (int i=0; i<ContextList.Count; i++)          
                    Dispatcher.RegisterContext((Dispatcher)ContextList[i]);
            }
        }

        object PostItems(object o)
        {
            int count=0;

            using ()
            {
        
                for (int index =0; index < ContextList.Count; index++)
                {
                    Dispatcher tempContext = ContextList[index] as Dispatcher;

                    for (int i=0;i<20;i++, count++)
                    {
                          tempContext.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(_Handler), i);
                    }
                }

                Dispatcher Context = ContextList[1] as Dispatcher;
                 Context.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback(_ExitHandler), null);

                
            }            
            
            return null;
        }

        object _Handler(object o)
        {
            s_counter++;
            
            int i = (int)o;

            Dispatcher contextRef = ContextList[5] as Dispatcher;
        
            if (contextRef == Dispatcher.CurrentDispatcher && i==9)
            {
                s_flagOne = true;
                _uiDispatcher.UnregisterContext(contextRef);
            }


            if (ContextList[0] == Dispatcher.CurrentDispatcher && i==9)
            {
                s_flagTwo = true;
         _uiDispatcher.UnregisterContext(ContextList[0] as Dispatcher);
            }
                           

            if (s_flagOne && contextRef == Dispatcher.CurrentDispatcher && i>9 && !contextRef.IsDisposed)
                throw new Microsoft.Test.TestValidationException("This items should not be handled");

            if (s_flagTwo && ContextList[5] == Dispatcher.CurrentDispatcher && i>9 && !contextRef.IsDisposed)
                throw new Microsoft.Test.TestValidationException("This items should not be handled");

            return null;
        
        }


        object _ExitHandler(object o)
        {
            using ()
            {
                Microsoft.Test.Threading.DispatcherHelper.ShutDown();
            }
            return null;
        }

        
        static bool s_flagOne = false;
        static bool s_flagTwo = false;

        static int s_counter = 0;

        private UIDispatcher _uiDispatcher = null;

    }
        
 }





