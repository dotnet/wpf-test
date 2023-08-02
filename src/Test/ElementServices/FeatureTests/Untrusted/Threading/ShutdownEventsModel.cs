// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Test Dispatcher Shutdown notifications
 * 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;
using System.Collections.Generic;
using Microsoft.Test.Win32;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Modeling;
using Microsoft.Test.Serialization;
using Microsoft.Test.Threading;
using System.Windows.Threading;
using System.Threading;
using Avalon.Test.CoreUI.Trusted;
using System.Reflection;
using Avalon.Test.CoreUI.Win32;

namespace Avalon.Test.CoreUI.Threading
{

    /// <summary>
    /// State based model.
    /// Adding and Removing Handlers to Dispatcher Shutdown event on a single thread.
    /// The model can be found at:
    /// %SDXROOT%\testsrc\windowstestdata\REDIST\client\wcptests\core\Base\Testcases\Threading\ShutDownEventsModel.mbt    
    /// </summary>
    [Model(@"FeatureTests\ElementServices\ShutdownEventAllTransitions.xtc", 1, 3, 1, @"Threading", TestCaseSecurityLevel.FullTrust, "ShutdownEventModel",
      Description="Adding and Removing handlers to Dispatcher Shutdown Events and validating state.", ExpandModelCases = true, Area = "ElementServices")]
    [Model(@"FeatureTests\ElementServices\ShutdownEventAllTransitions.xtc", 200, 205, 1, @"Threading", TestCaseSecurityLevel.FullTrust, "ShutdownEventModel",
      Description="Adding and Removing handlers to Dispatcher Shutdown Events and validating state.", ExpandModelCases = true, Area = "ElementServices")]
    [Model(@"FeatureTests\ElementServices\ShutdownEventAllTransitions.xtc", 200, 210, 1, @"Threading", TestCaseSecurityLevel.FullTrust, "ShutdownEventModel",
      Description="Adding and Removing handlers to Dispatcher Shutdown Events and validating state.(WorkertThread)", TestParameters = "Thread=WorkerThread", ExpandModelCases = true, Area = "ElementServices")]
    [Model(@"FeatureTests\ElementServices\ShutdownEventAllTransitions.xtc", 313, 313, 0, @"Threading", TestCaseSecurityLevel.FullTrust, "ShutdownEventModel",
      Description="Adding and Removing handlers to Dispatcher Shutdown Events and validating state.", ExpandModelCases = true, Area = "ElementServices")]
    [Model(@"FeatureTests\ElementServices\ShutdownEventAllTransitions.xtc", 313, 313, 0, @"Threading", TestCaseSecurityLevel.FullTrust, "ShutdownEventModel",
      Description="Adding and Removing handlers to Dispatcher Shutdown Events and validating state.(WorkertThread)", TestParameters = "Thread=WorkerThread", ExpandModelCases = true, Area = "ElementServices")]
    [Model(@"FeatureTests\ElementServices\ShutdownEventAllTransitions.xtc", 313, 313, 2, @"Threading", TestCaseSecurityLevel.FullTrust, "ShutdownEventModel",
      Description="Adding and Removing handlers to Dispatcher Shutdown Events and validating state.", ExpandModelCases = true, Area = "ElementServices")]
    [Model(@"FeatureTests\ElementServices\ShutdownEventAllTransitions.xtc",2, @"Threading", TestCaseSecurityLevel.FullTrust, "ShutdownEventModel",
      Description="Adding and Removing handlers to Dispatcher Shutdown Events and validating state.", ExpandModelCases = true, Area = "ElementServices")]
    public class ShutdownEventModel : CoreModel 
    {
        /// <summary>
        /// Creates a DispatcherOperationCompleteModel Model instance
        /// </summary>
        public ShutdownEventModel(): base()
        {
            Name = "ShutDownEventsModel";
            Description = "Model ShutDownEventsModel";
            ModelPath = "MODEL_PATH_TOKEN";

            //Attach Event Handlers
            OnBeginCase += new StateEventHandler(OnBeginCase_Handler);
            OnEndCase += new StateEventHandler(OnEndCase_Handler);
            OnEndCaseOnNestedPump += new EventHandler(OnEndCaseOnNestedPump_Handler);
            
            //Add Action Handlers
            AddAction("Setup", new ActionHandler(Setup));
            AddAction("RemoveHandler", new ActionHandler(RemoveHandler));
            AddAction("AddHandler", new ActionHandler(AddHandler));
            AddAction("Shutdown", new ActionHandler(Shutdown));

        }


        /// <summary>
        /// Sets the Model as necessary to begin the case with the given State
        /// </summary>
        /// <remarks>
        /// Attached to OnBeginCase event which is fired by the Traversal
        /// before a new case begins
        /// </remarks>
        /// <param name="sender">Model that fired the event</param>
        /// <param name="e">The Initial State in a StateEventArgs</param>
        private void OnBeginCase_Handler(object sender, StateEventArgs e)
        {
            _automation = new ShutdownEventModelAutomation(this.AsyncActions);
        }


        /// <summary>
        /// Sets the Model as necessary when a case ends with the given State
        /// </summary>
        /// <remarks>
        /// Attached to OnEndCase event which is fired by the Traversal
        /// after a case ends
        /// </remarks>
        /// <param name="sender">Model that fired the event</param>
        /// <param name="e">The End State in a StateEventArgs</param>
        private void OnEndCase_Handler(object sender, StateEventArgs e)
        {
            _automation.Dispose();
        }


        /// <summary>
        /// Callback that is called when alls transitions for the current test case on the 
        /// XTC are completed. For example you may want to exited the Dispatcher or
        /// or close the nested pump.
        /// </summary>
        private void OnEndCaseOnNestedPump_Handler(object o,EventArgs args)
        {
            _automation.Dispose();
        }

        /// <summary>
        /// </summary>
        private bool AddHandler( State endState, State inParameters, State outParameters )
        {
            LogComment( "Action AddHandler" );
            LogComment( inParameters.ToString() );

            bool succeed = true;

            /*
               inParameters["HandlerType"] - // 
               inParameters["WhichEvent"] - //                
            */

            _automation.AddHandler(inParameters["HandlerType"], inParameters["WhichEvent"]);

            return succeed;
        }

        /// <summary>
        /// </summary>
        private bool RemoveHandler( State endState, State inParameters, State outParameters )
        {
            LogComment( "Action RemoveHandler" );

            LogComment( inParameters.ToString() );

            bool succeed = true;

            /*
               inParameters["HandlerType"] - // 
               inParameters["WhichEvent"] - //            
            */
            _automation.RemoveHandler(inParameters["HandlerType"], inParameters["WhichEvent"]);
            return succeed;
        }

        /// <summary>
        /// </summary>
        private bool Shutdown( State endState, State inParameters, State outParameters )
        {
            LogComment( "Action Shutdown" );

            LogComment( inParameters.ToString() );

            bool succeed = true;

            /*
               inParameters["TypeofShutdown"] - // 
            */
            
            _automation.Shutdown(inParameters["TypeofShutdown"]);
            return succeed;
        }

        /// <summary>
        /// </summary>
         private bool Setup( State endState, State inParameters, State outParameters )
        {
            LogComment( "Action Setup" );

            LogComment( inParameters.ToString() );


            bool succeed = true;

            /*
               inParameters["TypeofDispatcher"] - // 
            */

            _automation.Setup(inParameters["TypeofDispatcher"]);
            return succeed;
        }


        ShutdownEventModelAutomation _automation = null;
        
    }



    /// <summary>
    /// State based model.
    /// Adding and Removing Handlers to Dispatcher Shutdown event on a single thread.
    /// The model can be found at:
    /// %SDXROOT%\testsrc\windowstestdata\REDIST\client\wcptests\core\Base\Testcases\Threading\ShutDownEventsModel.mbt  
    /// </summary>
    public class ShutdownEventModelAutomation : ModelAutomationBase
    {

        /// <summary>
        /// </summary>
        public ShutdownEventModelAutomation(AsyncActionsManager asyncManager):base(asyncManager)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            
            
            // Setting up the variables for the test case
            string workerThread = DriverState.DriverParameters["Thread"];

            if (String.Compare(workerThread, "WorkerThread", true) == 0)
            {
                CoreLogger.LogStatus("Calling Shutdown from workerThread.");
                _workerThread = true;
            }
            

            // Making sure that we stop the dispatcher (Win32 only)
            Dispatcher.CurrentDispatcher.ShutdownFinished += delegate
                {
                    if (_dispatcherWrapper != null && _dispatcherWrapper.CurrentDispatcherType == DispatcherType.Win32
                        && _shutdownCalledCount == 1)
                    {
                        GetNextAction();
                    }
                };
        }

        bool _workerThread = false;
        Dispatcher _dispatcher = null;

        /// <summary>
        /// Sets up the test case depending on the dispatcher to use.
        /// </summary>
        public void Setup(string dispatcherType)
        {
            if (String.Compare("None", dispatcherType, true) != 0)
            {
                DispatcherType dispatcherT = (DispatcherType)Enum.Parse(typeof(DispatcherType), dispatcherType);
               
                _dispatcherWrapper =  new DispatcherWrapper(Dispatcher.CurrentDispatcher, dispatcherT);                

                GetNextAction();

                _dispatcherWrapper.Run();
            }
        }



        /// <summary>
        /// Last validation.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            if (!IsDisposed)
            {
                if (Win32GenericMessagePump.Count > 1)
                {
                    Win32GenericMessagePump.Stop();
                }
            }

            DispatcherHelper.ValidHasShutdownStatus(true,true);

            // Last Validation to make sure the 
            // values match.

            int i = 0, j = 0;

            for (; i < 2; i++)
            {
                for (; j < 2; j++)
                {
                    if (_handlerActualCount[i,j] != _handlerCount[i,j])
                    {
                        CoreLogger.LogTestResult(false, "Doesn't match the count.");
                    }
                }
            }            
        }


        /// <summary>
        /// </summary>
        public void Shutdown(string typeOfShutdown)
        {           
            _shutdownCalledCount++;
            object[] arguments = {typeOfShutdown, _dispatcher};

                DoAction(
                    !_workerThread,
                    delegate(object o, EventArgs arg)
                    {
                        string shutdownType = (string)((object[])o)[0];
                        Dispatcher d = (Dispatcher)((object[])o)[1];
                        
                        if (String.Compare("Sync", shutdownType,true) == 0)
                        {                       
                            DispatcherHelper.ShutDown(d);
                        }
                        else if (String.Compare("Async", shutdownType,true) == 0)
                        {                   
                            DispatcherHelper.ShutDownPriorityBackground(d);
                        }
                    },
                    arguments);


/*            else if (String.Compare("WM_Destroy", typeOfShutdown,true) == 0)
            {
                // 


*/

            if (_dispatcherWrapper != null && _dispatcherWrapper.CurrentDispatcherType == DispatcherType.Win32 
                && _shutdownCalledCount == 2)
            {   
                Win32GenericMessagePump.Stop();
            }
        }


        /// <summary>
        /// We have this common function that depending on the speficied thread
        /// executes the specified callback with the specified args synchronously.
        /// </summary>        
        void DoAction(bool currentThread, 
            EventHandler callback, 
            object arg)
        {
            if (!currentThread)
            {
                // The callback will be executed on the threadpool.
                // The action will be synchronous.
                
                AutoResetEvent ev = new AutoResetEvent(false);
                object[] oArray = {arg, ev};
                ThreadPool.QueueUserWorkItem(
                    delegate(object o)
                    {
                        object[] oA = (object[])o;
                        AutoResetEvent ev1 = (AutoResetEvent)oA[1];
                        object wArg = oA[0];
                        ev1.Set();
                        callback(wArg, EventArgs.Empty);



                    }, oArray);
              
                ev.WaitOne();                
                Thread.Sleep(5000);
            }
            else
            {
                // Action performed by the dispatcher thread.
                callback(arg, EventArgs.Empty);
            }   
        }
   


        /// <summary>
        /// </summary>
        public void AddHandler(string handlerType, string eventName)
        {            
            AddRemoveHandler(handlerType, eventName, true);
        }


        /// <summary>
        /// </summary>
        public void RemoveHandler(string handlerType, string eventName)
        {            
            AddRemoveHandler(handlerType, eventName, false);
        }


        void AddRemoveHandler(string handlerType, string eventName, bool add)
        {
            
            DispatcherShutdownEventsNames eName = (DispatcherShutdownEventsNames)Enum.Parse(typeof(DispatcherShutdownEventsNames),eventName);
            EventHandler callback =  null;
            int callbackID = 0;
            
            if (String.Compare("One", handlerType,true) == 0)
            {
                callback = new EventHandler(ShutdownHandlerOne);
            }
            else
            {
                callbackID = 1;
                callback = new EventHandler(ShutdownHandlerTwo);                
            }

            if (add)
            {
                _handlerCount[(int)eName,callbackID]++;
                DispatcherHelper.AddHandlerShutdownEvents(eName,callback);
            }
            else
            {
                _handlerCount[(int)eName,callbackID]--;
                DispatcherHelper.RemoveHandlerShutdownEvents(eName,callback);
            }

            GetNextAction();
        }


        void GetNextAction()
        {
            if (_dispatcherWrapper != null)
            {
                GetModelNextAction(null);
            }
        }


        void ShutdownHandlerOne(object o, EventArgs args)
        {
            if (_shutdownCalledCount != 1)
            {
                CoreLogger.LogTestResult(false, "Only 1 Dispatcher.Shutdown should trigger this.");
            }
                          
            Validate(0);
              
        }

        void ShutdownHandlerTwo(object o, EventArgs args)
        {
            if (_shutdownCalledCount != 1)
            {
                CoreLogger.LogTestResult(false,"Only 1 Dispatcher.Shutdown should trigger this.");
            }          

            Validate(1);            
        }

        void Validate(int handlerIndex)
        {
            // This means that We are doing the ShutdownStarted event first.
            if (!Dispatcher.CurrentDispatcher.HasShutdownStarted)
            {                
                _handlerActualCount[0,handlerIndex]++;

                if (_handlerActualCount[1,0] != 0 || _handlerActualCount[1,1] != 0)
                {
                    CoreLogger.LogTestResult(false,"ShutdownFinished event should not be called yet.");
                }
            }
            else
            {
                // This means that ShutDownStarted already happen.
                int countForStarted = _handlerCount[0,handlerIndex];
                if (countForStarted != _handlerActualCount[0,handlerIndex])
                {
                    CoreLogger.LogTestResult(false,"The count doesn't match for ShutDownStarted.");
                }

                if (!Dispatcher.CurrentDispatcher.HasShutdownFinished)
                {
                    _handlerActualCount[1,handlerIndex]++;
                }
                else
                {
                    CoreLogger.LogTestResult(false,"ShutDownFinished should not be called.");
                }
            }
        }

    
        // The first column is for the Dispatcher Shutdown event.
        // The second column is for the handler that it is been used.
        int[,] _handlerCount = new int[2,2];

        // The first column is for the Dispatcher Shutdown event.
        // The second column is for the handler that it is been used.
        int[,] _handlerActualCount = new int[2,2];
       
        int _shutdownCalledCount = 0;
        private DispatcherWrapper _dispatcherWrapper = null;
    }


   
}


