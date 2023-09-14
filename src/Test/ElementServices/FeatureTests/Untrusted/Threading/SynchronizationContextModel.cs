// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Executing multiple action using multiple SynchronizationContext.
 * 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;
using System.Collections.Generic;
using Microsoft.Test.Win32;
using Microsoft.Test.Modeling;
using Microsoft.Test.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Threading;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using System.Reflection;
using Avalon.Test.CoreUI.Win32;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;

namespace Avalon.Test.CoreUI.Threading
{

    /// <summary>
    /// State based model.
    /// Creating DispatcherObject objects and Validating the its properties
    /// The model can be found at:
    /// %SDXROOT%\testsrc\windowstestdata\REDIST\client\wcptests\core\Base\Testcases\Threading\SynchronizationContextModel.mbt    
    /// </summary>
    [Model(@"FeatureTests\ElementServices\AllTransitionsSynchronizationContextModel.xtc", 1, @"Threading", TestCaseSecurityLevel.FullTrust, "SynchronizationContextModel", Description = "Executing multiple action using multiple SynchronizationContext.", ExpandModelCases = true, Area = "ElementServices")]    
    public class SynchronizationContextModel : CoreModel 
    {
        /// <summary>
        /// Creates a DispatcherOperationCompleteModel Model instance
        /// </summary>
        public SynchronizationContextModel(): base()
        {
            Name = "SynchronizationContextModel";
            Description = "Model SynchronizationContextModel";
            ModelPath = "MODEL_PATH_TOKEN";

            //Attach Event Handlers
            OnBeginCase += new StateEventHandler(OnBeginCase_Handler);
            OnEndCase += new StateEventHandler(OnEndCase_Handler);
            OnEndCaseOnNestedPump += new EventHandler(OnEndCaseOnNestedPump_Handler);
            
            //Add Action Handlers
            AddAction("EnableDispatcherProcessing", new ActionHandler(EnableDispatcherProcessing));
            AddAction("ExecuteActionToChangeSyncContext", new ActionHandler(ExecuteActionToChangeSyncContext));
            AddAction("LeaveExecuteAction", new ActionHandler(LeaveExecuteAction));
            AddAction("Setup", new ActionHandler(Setup));
            AddAction("SyncContextAction", new ActionHandler(SyncContextAction));            

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
            _automation = new SynchronizationContextModelAutomation(this.AsyncActions);
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
        private bool EnableDispatcherProcessing( State endState, State inParameters, State outParameters )
        {
            CoreLogger.LogStatus( "Action EnableDispatcherProcessing" );
            //CoreLogger.LogStatus( inParameters.ToString() );

            bool succeed = true;

            /*
               inParameters["Constructor"] - // 
            */

            _automation.EnableDispatcherProcessing();

            return succeed;
        }

        /// <summary>
        /// </summary>
        private bool ExecuteActionToChangeSyncContext( State endState, State inParameters, State outParameters )
        {
            CoreLogger.LogStatus( "Action ExecuteActionToChangeSyncContext" );

            CoreLogger.LogStatus( inParameters.ToString() );

            bool succeed = true;

            _automation.ExecuteActionToChangeSyncContext(inParameters);
            return succeed;
        }

        /// <summary>
        /// </summary>
        private bool LeaveExecuteAction(State endState, State inParameters, State outParameters)
        {
            CoreLogger.LogStatus( "Action LeaveExecutionAction" );

            //CoreLogger.LogStatus( inParameters.ToString() );

            bool succeed = true;


            _automation.LeaveExecuteAction();
            return succeed;
        }

        /// <summary>
        /// </summary>
        private bool Setup( State endState, State inParameters, State outParameters )
        {
            CoreLogger.LogStatus( "Action Setup" );
            CoreLogger.LogStatus( inParameters.ToString() );


            bool succeed = true;

            _automation.Setup(inParameters);
            return succeed;
        }


        /// <summary>
        /// </summary>
        private bool SyncContextAction( State endState, State inParameters, State outParameters )
        {
            CoreLogger.LogStatus( "Action SyncContextAction" );

            CoreLogger.LogStatus( inParameters.ToString() );


            bool succeed = true;

            _automation.SyncContextAction(inParameters);
            return succeed;
        }
        SynchronizationContextModelAutomation _automation = null;
        
    }


    /// <summary>
    /// State based model.
    /// 
    /// The model can be found at:
    /// %SDXROOT%\testsrc\windowstestdata\REDIST\client\wcptests\core\Base\Testcases\Threading\SynchronizationContextModel.mbt  
    /// </summary>
    public class SynchronizationContextModelAutomation : ModelAutomationBase
    {
        Stack<DispatcherProcessingDisabled> _dispatcherProcessingObjs = new Stack<DispatcherProcessingDisabled>();
        Stack _genericMessagePumps = new Stack();

        enum SyncContextActionNames
        {
            Post = 0,
            Send,
            Wait,
            Copy,
            BackgroundWorkerThread,
            AsyncOperationManager
        }


        enum SyncContextNames
        {
            DispatcherSynchronizationContext = 0,
            WindowsFormsSynchronizationContext,
            SynchronizationContext
        }
        
        /// <summary>
        /// </summary>
        public override void Dispose()
        {

            if (!IsDisposed)
            {
                if (_applicationType == 0)
                {
                    System.Windows.Forms.Application.Exit();                    
                }
                else if (_applicationType == 1)
                {
                    System.Windows.Forms.Application.Exit();     
                }
                else if (_applicationType == 2)
                {
                    DispatcherHelper.ShutDown();
                }
                else if (_applicationType == 3)
                {
                    System.Windows.Application.Current.Shutdown();
                }
            }
            base.Dispose();
            
        }


        /// <summary>
        /// </summary>
        public SynchronizationContextModelAutomation(AsyncActionsManager asyncManager):base(asyncManager)
        {

             DispatcherRunning = DispatcherType.Avalon;
             _dispatcher = Dispatcher.CurrentDispatcher;
        }

        /// <summary>
        /// Sets up the test case depending on the dispatcher to use.
        /// </summary>
        public void Setup(IDictionary dictionary)
        {

            _dispatcher.ShutdownStarted += delegate
            {
                NestingCall(null);
            };

            _dispatcher.ShutdownFinished += delegate
            {
                NestingCall(null);
            };

            _applicationType = ParsingSetup(dictionary);

            GetModelNextAction(null);

            if (_applicationType == 0)
            {                     
                _syncContextStack.Push(SyncContextNames.WindowsFormsSynchronizationContext);
                System.Windows.Forms.Application.Run();                    
            }
            else if (_applicationType == 1)
            {
                _syncContextStack.Push(SyncContextNames.WindowsFormsSynchronizationContext);
                WinformsApp app = new WinformsApp();
                app.Run(null);
            }
            else if (_applicationType == 2)
            {
                _syncContextStack.Push(SyncContextNames.DispatcherSynchronizationContext);                
                DispatcherHelper.RunDispatcher();
            }
            else if (_applicationType == 3)
            {
                System.Windows.Application app = new System.Windows.Application();
                _syncContextStack.Push(SyncContextNames.DispatcherSynchronizationContext);                
                app.Run();                                
            }            
            _syncContextStack.Pop();
        }


        private void Validate()
        {
            if (_syncContextStack.Count > 0)
            {
                SynchronizationContext syncContext = SynchronizationContext.Current;
                
                if (_syncContextStack.Peek() == SyncContextNames.DispatcherSynchronizationContext)
                {
                    if (!(syncContext is DispatcherSynchronizationContext))
                    {
                        CoreLogger.LogTestResult(false,"DSC failed");
                    }
                }
                
                if (_syncContextStack.Peek() == SyncContextNames.WindowsFormsSynchronizationContext)
                {
                    if (!(syncContext is WindowsFormsSynchronizationContext))
                    {
                        CoreLogger.LogTestResult(false,"WFSC failed");
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        public void SyncContextAction(IDictionary dictionary)
        {
            SyncContextActionNames actionName = ParsingName(dictionary);
            bool sameThread = ParsingOrigenThread(dictionary);
            bool disabledDispatcher = ParsingDispatcherProcessingStateDisabled(dictionary);

            SynchronizationContext sc = SynchronizationContext.Current;
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            
            Validate();
            
            if (SyncContextActionNames.Post == actionName)
            {
                DoAction(sameThread, delegate(object o, EventArgs args)
                {
                    SynchronizationContext scAux = (SynchronizationContext)o;
                    sc.Post(new SendOrPostCallback(SendOrPostHandler), null);
                }, sc);
            }
            else if (SyncContextActionNames.Send == actionName)
            {
                DoAction(sameThread, delegate(object o, EventArgs args)
                {
                    SynchronizationContext scAux = (SynchronizationContext)o;
                    sc.Send(new SendOrPostCallback(SendOrPostHandler), null);
                }, sc);
            }
            else if (SyncContextActionNames.Wait == actionName)
            {
                DispatcherProcessingDisabled dpd = new DispatcherProcessingDisabled(); 

                if(disabledDispatcher)
                {
                    dpd = dispatcher.DisableProcessing();
                }
            
                AutoResetEvent ev = new AutoResetEvent(false);
                
                ThreadPool.QueueUserWorkItem(
                     delegate(object o)
                    {
                        object[] oA = (object[])o;
                        AutoResetEvent ev1 = (AutoResetEvent)oA[0];

                        Thread.Sleep(1000);

                        ev1.Set();

                    }, new object[] {ev});
               
                ev.WaitOne();


                if(disabledDispatcher)
                {
                    dpd.Dispose();
                }

                GetNextActionOnAvalonQueue(Dispatcher.CurrentDispatcher);
                                
            }
            else if (SyncContextActionNames.Copy == actionName)
            {
                DoAction(sameThread, delegate(object o, EventArgs args)
                {
                    SynchronizationContext scAux = (SynchronizationContext)o;
                    SynchronizationContext scCopy = scAux.CreateCopy();        
                    ValidateEqualsDispatcherSyncContext(scAux, scCopy);
                }, sc);

                GetNextActionOnAvalonQueue(Dispatcher.CurrentDispatcher);

            }
            else if (SyncContextActionNames.BackgroundWorkerThread== actionName)
            {
                
                BackgroundWorker b = new BackgroundWorker();
                b.DoWork += delegate(object sender, DoWorkEventArgs e)
                    {
                        BackgroundWorker worker = sender as BackgroundWorker;

                        for (int i = 0; i<10;i++)
                        {
                            worker.ReportProgress(i);
                        }
                    };
                b.WorkerReportsProgress = true;
                b.WorkerSupportsCancellation = false;
                b.RunWorkerAsync();
                b.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
                    {
                        ValidateThread();
                        Win32GenericMessagePump.Current.Stop();
                    };
                b.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e)
                    {
                        ValidateThread();
                    };

                Win32GenericMessagePump.Run();

                GetNextActionOnAvalonQueue(Dispatcher.CurrentDispatcher);

            }

            else if (SyncContextActionNames.AsyncOperationManager == actionName)
            {
                
                AsyncOperation op = AsyncOperationManager.CreateOperation(null);
                op.Post(new SendOrPostCallback(SendOrPostHandler), null);                
            }



        }
  

        static void ValidateEqualsDispatcherSyncContext(SynchronizationContext sc1, SynchronizationContext sc2)
        {           
            if (sc1 is DispatcherSynchronizationContext && sc2 is DispatcherSynchronizationContext)
            {
                Type sc1Type = sc1.GetType();
                Type sc2Type = sc2.GetType();

                FieldInfo fieldsc1 = sc1Type.GetField("_dispatcher", BindingFlags.GetField | BindingFlags.NonPublic |  BindingFlags.Instance); 
                FieldInfo fieldsc2 = sc1Type.GetField("_dispatcher", BindingFlags.GetField | BindingFlags.NonPublic |  BindingFlags.Instance); 


                Dispatcher dispatcherObject1 = (Dispatcher)fieldsc1.GetValue(sc1);
                Dispatcher dispatcherObject2 = (Dispatcher)fieldsc2.GetValue(sc2);    

                if (dispatcherObject1 == null || dispatcherObject1 != dispatcherObject2)
                {
                    CoreLogger.LogTestResult(false,"The DispatcherSyncContext are not equals.");
                }                
            }
        }


        void ValidateThread()
        {
            CoreLogger.LogStatus("Validate Thread");
            if (Thread.CurrentThread != _dispatcher.Thread)
            {
                CoreLogger.LogTestResult(false, "The thread is not the dispatcher Thread.");
            }
        }
        
        void SendOrPostHandler(object o)
        {
            ValidateThread();
            Validate();
            GetNextActionOnAvalonQueue(Dispatcher.CurrentDispatcher);

        }



        /// <summary>
        ///
        /// </summary>
        public void ExecuteActionToChangeSyncContext(IDictionary dictionary)
        {
            int queryLocation = ParsingQueryLocation(dictionary);
            bool sameThread = ParsingOrigenThread(dictionary);
            bool nesting = ParsingNestOnLocation(dictionary);

            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;


            Validate();

            if (queryLocation == 2)
            {
                GetNextActionOnAvalonQueue(dispatcher);
                _syncContextStack.Push(SyncContextNames.DispatcherSynchronizationContext);                

                DispatcherFrame frame = new DispatcherFrame();
                _genericMessagePumps.Push(frame);                
                DispatcherHelper.EnterLoop(frame, null, DispatcherPriority.Normal);
                _syncContextStack.Pop();
            }
            else if (queryLocation == 0)
            {
                _syncContextStack.Push(SyncContextNames.DispatcherSynchronizationContext);                
                
                dispatcher.Invoke(DispatcherPriority.Normal, new DispatcherOperationCallback(NestingCall), new object[] {nesting, dispatcher});                    
                _syncContextStack.Pop();
            }
            else if (queryLocation == 1)
            {
                dispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback(NestingCall), new object[] {nesting, dispatcher});                    
            }
            
        }


        private object NestingCall(object o)
        {
            ValidateThread();

            if (!(SynchronizationContext.Current is DispatcherSynchronizationContext))
            {
                CoreLogger.LogTestResult(false,"DispatcherSyncContext should be applied.");
            }

            if (o != null)
            {
                bool nesting = (bool)((object[])o)[0];
                Dispatcher dispatcher = (Dispatcher)((object[])o)[1];
                if (nesting)
                {
                    _syncContextStack.Push(SyncContextNames.DispatcherSynchronizationContext);                

                    GetNextActionOnAvalonQueue(dispatcher);                
                    _genericMessagePumps.Push(Win32GenericMessagePump.Current);
                    Win32GenericMessagePump.Run();
                    _syncContextStack.Pop();
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void EnableDispatcherProcessing()
        {
            GetNextActionOnAvalonQueue(Dispatcher.CurrentDispatcher);
            Validate();
        }


        /// <summary>
        /// 
        /// </summary>
        public void LeaveExecuteAction()
        {
            Validate();            
            object mpObj = _genericMessagePumps.Pop();

            if (mpObj is DispatcherFrame)
            {
                ((DispatcherFrame)mpObj).Continue = false;
            }
            else
            {
                ((Win32GenericMessagePumpObj)mpObj).Stop();
            }
            GetNextActionOnAvalonQueue(Dispatcher.CurrentDispatcher);
                
        }



        static SyncContextActionNames ParsingName(IDictionary dictionary)
        {
            SyncContextActionNames actionName = (SyncContextActionNames)Enum.Parse(typeof(SyncContextActionNames),
                (string)dictionary["Name"]);

            return actionName;
        }
    

        static bool ParsingDispatcherProcessingStateDisabled(IDictionary dictionary)
        {
            if (String.Compare("Disabled",(string)dictionary["DispatcherProcessingState"], true) == 0)
            {
                return true;
            } 
            return false;


        }


        static bool ParsingNestOnLocation(IDictionary dictionary)
        {
            if (String.Compare("True",(string)dictionary["NestOnLocation"], true) == 0)
            {
                return true;
            } 
            return false;
        }

        static bool ParsingOrigenThread(IDictionary dictionary)
        {
            if (String.Compare("SameThread",(string)dictionary["OrigenThread"], true) == 0)
            {
                return true;
            } 
            return false;
        }


        static int ParsingQueryLocation(IDictionary dictionary)
        {
            string queryLocation = (string)dictionary["QueryLocation"];
            
            if (String.Compare("DispatcherInvoke",queryLocation, true) == 0)
            {
                return 0;
            }
            
            if (String.Compare("DispatcherBeginInvoke",queryLocation, true) == 0)
            {
                return 1;
            }

            if (String.Compare("DispatcherPushFrame",queryLocation, true) == 0)
            {
                return 2;
            }
            return 0;
        }

        static int ParsingSetup(IDictionary dictionary)
        {
            string applicationType = (string)dictionary["ApplicationType"];
            
            if (String.Compare("Winforms_Application",applicationType, true) == 0)
            {
                return 0;
            }
            
            if (String.Compare("Winforms_CustomApplication",applicationType, true) == 0)
            {
                return 1;
            }

            if (String.Compare("Console",applicationType, true) == 0)
            {
                return 2;
            }

            if (String.Compare("Avalon",applicationType, true) == 0)
            {
                return 3;
            }

            return 0;
        }




        /// <summary>
        /// We have this common function that depending on the speficied thread
        /// executes the specified callback with the specified args synchronously.
        /// </summary>        
        void DoAction(bool sync, 
            EventHandler callback, 
            object arg)
        {
            if (!sync)
            {
                // The callback will be executed on the threadpool.
                // The action will be synchronous.

                Win32GenericMessagePumpObj obj = Win32GenericMessagePump.Current;
                AutoResetEvent ev = new AutoResetEvent(false);
                object[] oArray = {arg, obj};
                ThreadPool.QueueUserWorkItem(
                    delegate(object o)
                    {
                        object[] oA = (object[])o;
                        //AutoResetEvent ev1 = (AutoResetEvent)oA[1];
                        object wArg = oA[0];
                        callback(wArg, EventArgs.Empty);

                        Win32GenericMessagePumpObj w = (Win32GenericMessagePumpObj)oA[1];
                        w.Stop();
                        //ev1.Set();

                    }, oArray);
              
                //ev.WaitOne();
                Win32GenericMessagePump.Run();
            }
            else
            {
                // Action performed by the dispatcher thread.
                callback(arg, EventArgs.Empty);
            }   
        }


        int _applicationType =0;
        Dispatcher _dispatcher  = null;
        Stack<SyncContextNames> _syncContextStack = new Stack<SyncContextNames>();

    }

    // 

    class WinformsApp 
    {
        // run is implemented in WindowsFormsApplicationBase
        public void Run (string[] commandLine)
        {
            throw new NotImplementedException("WinformsApp - Run");
        }
    }

}





