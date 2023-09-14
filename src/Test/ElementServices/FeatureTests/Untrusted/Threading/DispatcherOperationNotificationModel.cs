// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using Microsoft.Test.Modeling;
using System.Windows.Threading;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test.Discovery;
using Microsoft.Test;

namespace Avalon.Test.CoreUI.Threading
{

    /****************  DispatcherOperationCompleteModel Model *****************
     *  Description: 
     *  Area: 
      *  Dependencies: ClientTestLibrary.dll
     *  Revision History:
     **********************************************************/

    /// <summary>
    /// DispatcherOperationCompleteModel Model class
    /// State Based Model About the Notification and State behavior of a Single DispatcherOperation.
    /// The model can be found at     
    /// %SDXROOT%\testsrc\windowstestdata\REDIST\client\wcptests\core\Base\Testcases\Threading\NotificationModel.mbt
    /// </summary>
    [Model(@"FeatureTests\ElementServices\NotificationModelDirect5Depth.xtc", 3, @"Threading\Operation", TestCaseSecurityLevel.FullTrust, "DispatcherOperationNotificationModel", Description = "State Based Model About the Notification and State behavior of a Single DispatcherOperation.", ExpandModelCases = true, Area = "ElementServices")]
    [Model(@"FeatureTests\ElementServices\NotificationModelAllTransitions.xtc", 1, @"Threading\Operation", TestCaseSecurityLevel.FullTrust, "DispatcherOperationNotificationModel", Description = "State Based Model About the Notification and State behavior of a Single DispatcherOperation.", ExpandModelCases = true, Area = "ElementServices")]
    [Model(@"FeatureTests\ElementServices\NotificationModelAllTransitions.xtc", 55, 60, 0, @"Threading\Operation", TestCaseSecurityLevel.FullTrust, "DispatcherOperationNotificationModel", Description = "State Based Model About the Notification and State behavior of a Single DispatcherOperation.", ExpandModelCases = true, Area = "ElementServices")]    
    public class DispatcherOperationNotificationModel : CoreModel 
    {

        /// <summary>
        /// Creates a DispatcherOperationCompleteModel Model instance
        /// </summary>
        public DispatcherOperationNotificationModel(): base()
        {
            Name = "untitled";
            Description = "Model untitled";
            ModelPath = "MODEL_PATH_TOKEN";

            //Attach Event Handlers
            OnInitialize += new EventHandler(OnInitialize_Handler);
            OnCleanUp += new EventHandler(OnCleanUp_Handler);
            OnGetCurrentState += new StateEventHandler(OnGetCurrentState_Handler);
            OnBeginCase += new StateEventHandler(OnBeginCase_Handler);
            OnEndCase += new StateEventHandler(OnEndCase_Handler);
            OnEndCaseOnNestedPump += new EventHandler(OnEndCaseOnNestedPump_Handler);
            
            //Add Action Handlers
            AddAction("SetupEnvironment", new ActionHandler(SetupEnvironment));
            AddAction("PostOperation", new ActionHandler(PostOperation));
            AddAction("Abort", new ActionHandler(Abort));
            AddAction("ChangePriority", new ActionHandler(ChangePriority));
            AddAction("Wait", new ActionHandler(Wait));
            AddAction("DispatchOperation", new ActionHandler(DispatchOperation));

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
        /// Initializes the Model
        /// </summary>
        /// <remarks>
        /// Attached to OnInitialize event which is fired when your model is
        /// created and added to a Traversal
        /// </remarks>
        /// <param name="sender">The model that fired the event</param>
        /// <param name="e">Contains no information (EventArgs.Empty)</param>
        private void OnInitialize_Handler(object sender, EventArgs e){}


        /// <summary>
        /// Cleans up the Model
        /// </summary>
        /// <remarks>
        /// Attached to OnCleanUp event which is fired when your model is
        /// removed from the Traversal
        /// </remarks>
        /// <param name="sender">The model that fired the event</param>
        /// <param name="e">Contains no information (EventArgs.Empty)</param>
        private void OnCleanUp_Handler(object sender, EventArgs e) {}


        /// <summary>
        /// Gets the current State of the Model
        /// </summary>
        /// <remarks>
        /// Attached to OnGetCurrentState event which is fired after
        /// each action to validate
        /// </remarks>
        /// <param name="sender">Model that fired the event</param>
        /// <param name="e">The current State in a StateEventArgs</param>
        private void OnGetCurrentState_Handler(object sender, StateEventArgs e){}


        /// <summary>
        /// Sets the Model as necessary to begin the case with the given State
        /// </summary>
        private void OnBeginCase_Handler(object sender, StateEventArgs e)
        {
            _automation = new DispatcherOperationNotificationAutomation(this.AsyncActions);
        }


        /// <summary>
        /// Sets the Model as necessary when a case ends with the given State
        /// </summary>
        private void OnEndCase_Handler(object sender, StateEventArgs e)
        {
            _automation.Validate();
        }


        /// <summary>
        /// Set up the Dispatcher that will be used on test case.
        /// </summary>
        private bool SetupEnvironment( State endState, State inParameters, State outParameters )
        {
            LogComment( "Action SetupEnvironment" );
            LogComment( inParameters.ToString() );

            bool succeed = true;

            _automation.SetupEnvironment(inParameters["TypeOfDispatcher"]);


            return succeed;
        }

        /// <summary>
        /// Handler for PostOperation
        /// </summary>
        /// 
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        private bool PostOperation( State endState, State inParameters, State outParameters )
        {
            LogComment( "Action PostOperation" );
            LogComment( inParameters.ToString() );

            bool succeed = true;


            _automation.PostOperation(inParameters["Callback"],inParameters["Priority"],inParameters["Thread"]);

            return succeed;
        }

        /// <summary>
        /// Handler for Abort
        /// </summary>
        /// 
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        private bool Abort( State endState, State inParameters, State outParameters )
        {
            LogComment( "Action Abort" );

            LogComment( inParameters.ToString() );

            bool succeed = true;

            _automation.Abort(inParameters["Thread"]);

            return succeed;
        }

        /// <summary>
        /// </summary>
        /// 
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        private bool ChangePriority( State endState, State inParameters, State outParameters )
        {
            LogComment( "Action ChangePriority" );

            LogComment( inParameters.ToString() );

            bool succeed = true;

            _automation.ChangePriority(inParameters["Priority"], inParameters["Thread"]);
            return succeed;
        }

        /// <summary>
        /// Handler for Wait
        /// </summary>
        /// 
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        private bool Wait( State endState, State inParameters, State outParameters )
        {
            LogComment( "Action Wait" );

            LogComment( inParameters.ToString() );
            LogComment( outParameters.ToString() );
            LogComment( endState.ToString() );

            bool succeed = true;


            /*
               inParameters["Type"] - // 
               inParameters["Thread"] - // 
            */


            return succeed;
        }

        /// <summary>
        /// Handler for DispatchOperation
        /// </summary>
        /// 
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        private bool DispatchOperation( State endState, State inParameters, State outParameters )
        {
            LogComment( "Action DispatchOperation" );

            bool succeed = true;
            _automation.DispatchOperation();


            return succeed;
        }        

        DispatcherOperationNotificationAutomation _automation;
        
    }


    ///<summary>
    /// This class implements the automation requeries for the model 
    /// %SDXROOT%\testsrc\windowstestdata\REDIST\client\wcptests\core\Base\Testcases\Threading\NotificationModel.mbt
    ///</summary>     
    internal class DispatcherOperationNotificationAutomation : ModelAutomationBase
    {
        ///<summary>
        ///</summary>     
        public DispatcherOperationNotificationAutomation(AsyncActionsManager asyncManager):base(asyncManager){}


        ///<summary>
        /// Start the message pump and posting an item to stop it, just enough to get the 
        /// dispatcheroperation dispatched.
        ///</summary>        
        public void DispatchOperation()
        {   
            Interlocked.Increment(ref _posted);

            // This will generate another post.
            _dw.ShutDown();
            
            _dw.Run();            
        }

    
        ///<summary>
        /// Setting which type of dispatcher we are going to use. Avalon or Win32 Dispatcher as option.
        ///</summary>        
        public void SetupEnvironment(string typeOfDispatcher)
        {
            DispatcherType dt = DispatcherType.Avalon;
            if (typeOfDispatcher == "Win32")
            {
                dt = DispatcherType.Win32;                
            }

            _dw = new DispatcherWrapper(Dispatcher.CurrentDispatcher, dt);
            _dw.Hooks.AddRemoveEvents(DispatcherNotification.Posted,true);
            _dw.Hooks.AddRemoveEvents(DispatcherNotification.Completed,true);
            _dw.Hooks.AddRemoveEvents(DispatcherNotification.PriorityChanged,true);
            _dw.Hooks.AddRemoveEvents(DispatcherNotification.Aborted,true);

            _dw.Hooks.Listener += (DispatcherHooksWrapperEventHandler)delegate
                (object o, DispatcherHooksWrapperEventArgs args)
                {
                    if (args != null && args.Args != null && args.Args.Dispatcher != o)
                    {
                        CoreLogger.LogTestResult(false, "Dispatcher property should be the same.");
                    }
                    
                    if (args.Notification == DispatcherNotification.Posted)
                    {
                        Interlocked.Increment(ref _postedNotification);
                    }

                    if (args.Notification == DispatcherNotification.Completed)
                    {
                        Interlocked.Increment(ref _completedNotification);

                        if (_completedNotification == 1 && !_dw.ShutdownStarted)
                        {
                            if (_completed > 0 && (_abortedNotification > 0 || _aborted > 0 || _opWrapper.AbortedCount > 0))
                            {
                                CoreLogger.LogTestResult(false, "This operation suppose to be aborted");
                            }
                            else
                            {
                                if (_opWrapper.Priority != DispatcherPriority.Inactive && _completed != 1)
                                {
                                    CoreLogger.LogTestResult(false, "The notification arrive earlier that the execution.");
                                }
                            }
                        }
                    }

                    if (args.Notification == DispatcherNotification.Aborted)
                    {
                        Interlocked.Increment(ref _abortedNotification);

                        if (_abortedNotification == 1)
                        {
                            // This case is when the operation is Inactive
                            // and we are shutting  down. The dispatcher shutdown process
                            // will abort the operation
                            if (_dw.RealDispatcher.HasShutdownFinished)
                            {
                                Interlocked.Increment(ref _aborted);
                                if (_opWrapper.Priority != DispatcherPriority.Inactive)
                                {
                                    CoreLogger.LogTestResult(false,"The dispatcher should not be on the queue.");
                                }
                            }
                            
                            if (this._opWrapper.AbortedCount != 0)
                            {
                                CoreLogger.LogTestResult(false,"The orden for abort was not correct");
                            }
                        }                        
                    }                

                    if (args.Notification == DispatcherNotification.PriorityChanged)
                    {
                        Interlocked.Increment(ref _pcNotification);
                    }                                
                };
        }


        ///<summary>
        /// Posting the dispatcheroperation.  We have 4 different types of callbacks and different 
        /// priorities. The post happens on the specified thread.
        ///</summary>     
        public void PostOperation(string callback, string priority, string thread)
        {
            DispatcherPriority dPriority = (DispatcherPriority)Enum.Parse(typeof(DispatcherPriority), priority,true);

            // Choosing the callback
    
            InvokeCallbackTypes callbackType = InvokeCallbackTypes.One_Param_DispatcherOperationCallback;

            switch(callback)
            {
                case "TwoParam":
                    callbackType = InvokeCallbackTypes.Two_Param_Generic;
                    break;

                case "ThreeParam":
                    callbackType = InvokeCallbackTypes.Three_Param_Generic;
                    break;

                case "ZeroParam":
                    callbackType = InvokeCallbackTypes.Zero_Param_Generic;
                    break;
            }

            object[] oArray = {_dw, dPriority, callbackType};
            
            DoAction(thread, 
                delegate (object o1, EventArgs args1)
                {
                    object[] oA = (object[])o1;

                    Post((DispatcherWrapper)oA[0],
                        (DispatcherPriority)oA[1],
                        (InvokeCallbackTypes)oA[2]);

                }, oArray);       
        }


        ///<summary>
        /// Aborting the dispatcheroperation on the specified thread.
        ///</summary>     
        public void Abort(string thread)
        {
            DoAction(thread, 
                delegate
                {
                    Interlocked.Increment(ref _aborted);    

                    // Real Abort Operation.
                    bool value = _opWrapper.Abort();

                    if (_aborted == 1)
                    {
                        // Aborting for the first time.
                        if (value == true)
                        {    
                            if (_opWrapper.Status != DispatcherOperationStatus.Aborted)
                            {
                                CoreLogger.LogTestResult(false,"The operation should have aborted status.");
                            }

                            
                            if (_completed != 0 || _opWrapper.AbortedCount != 1 || _abortedNotification != 1)
                            {
                                CoreLogger.LogTestResult(false,"The abort succeed but the notification where incorrect.");
                            }
                        }
                        else
                        {
                            if (_completed != 1 && _completedNotification != 0)
                            {
                                CoreLogger.LogTestResult(false,"The abort failed, but the completed notification failed.");                    
                            }
                        }
                    }
                    else if (_aborted == 2)                        
                    {
                        if (value)
                        {
                            CoreLogger.LogTestResult(false,"The abort success but it should failed. Abortted twice.");
                        }
                    }                    
                }, null);         
        }


        ///<summary>
        /// Perform a Priority Change on the DispatcherOperation on the specified thread        
        /// at the specified priority.
        ///</summary>     
        public void ChangePriority(string priority, string thread)
        {
            DispatcherPriority dPriority = (DispatcherPriority)Enum.Parse(typeof(DispatcherPriority), priority,true);
            
            DoAction(thread, 
                delegate(object o1, EventArgs args1)
                {
                    DispatcherPriority p = (DispatcherPriority)o1;
                    
                    if ( (_aborted == 0 && _completed == 0 && _opWrapper.Priority != p))
                    {
                        Interlocked.Increment(ref _pc);
                    }

                    // Performing the Prioriry Change.                    
                    _opWrapper.Priority = p;

                    if (_aborted == 0 && _completed == 0)
                    {
                        if (_opWrapper.Priority != dPriority)
                        {
                            CoreLogger.LogTestResult(false,"The priority didn't change.");                            
                        }
                    }
                    if (_pcNotification != _pc)
                    {
                        CoreLogger.LogTestResult(false,"The priority notification was incorrect.");                            
                    }
                    
                }, dPriority);        
        }


        ///<summary>
        /// Last validations that we can performed before finishing the test cases.
        ///</summary>     
        public void Validate()
        {
            CoreLogger.LogStatus("Last Validations.");
            
            if (_postedNotification != _posted) 
            {
                CoreLogger.LogTestResult(false, "");
            }

            if (_completed > 0 && _opWrapper.Status != DispatcherOperationStatus.Completed)
            {
                CoreLogger.LogTestResult(false,"The operation should have Completed status.");
            }

            // Removing the handlers.
            _dw.Hooks.AddRemoveEvents(DispatcherNotification.Posted,false);
            _dw.Hooks.AddRemoveEvents(DispatcherNotification.Completed,false);
            _dw.Hooks.AddRemoveEvents(DispatcherNotification.PriorityChanged,false);
            _dw.Hooks.AddRemoveEvents(DispatcherNotification.Aborted,false);
            
        }


        /// <summary>
        /// We have this common function that depending on the speficied thread
        /// executes the specified callback with the specified args synchronously.
        /// </summary>        
        void DoAction(string thread, 
            EventHandler callback, 
            object arg)
        {
            if (thread == "OtherThread")
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
                        callback(wArg, EventArgs.Empty);

                        ev1.Set();

                    }, oArray);
              
                ev.WaitOne();
            }
            else
            {
                // Action performed by the dispatcher thread.
                callback(arg, EventArgs.Empty);
            }   
        }


        void Post(DispatcherWrapper dw, DispatcherPriority priority, InvokeCallbackTypes callbackType)
        {
            Interlocked.Increment(ref _posted);
            
            _opWrapper = new DispatcherOperationWrapper(dw,  priority, callbackType);
            _opWrapper.BeginInvoke(delegate
                {
                    // This will be executing for our test dispatcheroperation.
                    
                    if (_opWrapper.Status != DispatcherOperationStatus.Executing)
                    {
                        CoreLogger.LogTestResult(false,"The operation should have Executing status.");
                    }
                    
                    Interlocked.Increment(ref _completed);
                });

            if ((_posted != _postedNotification) && (_posted != 1))
            {
                CoreLogger.LogTestResult(false,"The first Posted Notification was incorrect.");                            
            }
            
        }

        // Counters for all the Notifications that happens
        // on the DispatcherHooks.
        private int _postedNotification = 0;
        private int _completedNotification = 0;
        private int _abortedNotification = 0;
        private int _pcNotification = 0;

        // Counter for when the actions are performed.
        private int _posted = 0;
        private int _completed = 0;
        private int _aborted = 0;
        private int _pc = 0;

        // Dispatcher.
        DispatcherWrapper _dw = null;

        // DispatcherOperation.
        DispatcherOperationWrapper _opWrapper = null;
        
    }
}

