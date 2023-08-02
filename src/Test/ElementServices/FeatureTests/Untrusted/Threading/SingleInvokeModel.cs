// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Threading;
using Microsoft.Test.Modeling;
using Microsoft.Test.Threading;
using System.Windows.Threading;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test.Logging;
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
    /// Stateless Model About the a Single Dispatcher.Invoke behavior
    /// The model can be found at     
    /// %SDXROOT%\testsrc\windowstestdata\REDIST\client\wcptests\core\Base\Testcases\Threading\SingleInvokeModel.mbt
    /// </summary>
    [Model(@"FeatureTests\ElementServices\SingleInvokeModel.xtc", 1, @"Threading", TestCaseSecurityLevel.PartialTrust, "SingleDispatcherInvokeModel", Description = "Stateless Model About the a Single Dispatcher.Invoke behavior.", ExpandModelCases = true, Area = "ElementServices")]
    [Model(@"FeatureTests\ElementServices\SingleInvokeModel.xtc", 1, 5, 0, @"Threading", TestCaseSecurityLevel.PartialTrust, "SingleDispatcherInvokeModel", Description = "Stateless Model About the a Single Dispatcher.Invoke behavior.", ExpandModelCases = true, Area = "ElementServices")]    
    public class SingleInvokeModel : CoreModel 
    {

        /// <summary>
        /// Creates a SingleInvokeModel Model instance
        /// </summary>
        public SingleInvokeModel(): base()
        {
            Name = "SingleInvokeModel";
            Description = "Model SingleInvokeModel";
            ModelPath = "MODEL_PATH_TOKEN";

            //Attach Event Handlers
            OnInitialize += new EventHandler(OnInitialize_Handler);
            OnCleanUp += new EventHandler(OnCleanUp_Handler);
            
            //Add Action Handlers
            AddAction("DispatcherInvoke", new ActionHandler(DispatcherInvoke));
            AddAction("Setup", new ActionHandler(Setup));

        }


        /// <summary>
        /// Initializes the Model
        /// </summary>
        private void OnInitialize_Handler(object sender, EventArgs e){}


        /// <summary>
        /// Cleans up the Model
        /// </summary>
        private void OnCleanUp_Handler(object sender, EventArgs e) {}


        /// <summary>
        /// Handler for DispatcherInvoke
        /// </summary>
        private bool DispatcherInvoke( State endState, State inParameters, State outParameters )
        {
            LogComment( "Action DispatcherInvoke" );
            LogComment( inParameters.ToString() );


            bool succeed = true;

            _automation.DispatcherInvoke(inParameters);
            
            return succeed;
        }

        /// <summary>
        /// Handler for Setup
        /// </summary>
        private bool Setup( State endState, State inParameters, State outParameters )
        {
            LogComment( "Action Setup" );

            LogComment( inParameters.ToString() );

            bool succeed = true;

            _automation = new SingleInvokeModelAutomation(AsyncActions);
            _automation.Setup(inParameters["DispatcherType"]);

            return succeed;
        }

        SingleInvokeModelAutomation _automation;
        
    }


    ///<summary>
    /// This class implements the automation requeries for the model 
    /// %SDXROOT%\testsrc\windowstestdata\REDIST\client\wcptests\core\Base\Testcases\Threading\SingleInvokeModel.mbt
    ///</summary>     
    internal class SingleInvokeModelAutomation : ModelAutomationBase
    {
        ///<summary>
        ///</summary>     
        public SingleInvokeModelAutomation(AsyncActionsManager asyncManager):base(asyncManager){}

        ///<summary>
        /// Setting which type of dispatcher we are going to use. Avalon or Win32 Dispatcher as option.
        ///</summary>        
        public void Setup(string typeOfDispatcher)
        {
            Dispatcher.CurrentDispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(UnhandledExceptionHandler);
            DispatcherHelper.GetHooks().OperationAborted += delegate (object sender, DispatcherHookEventArgs e)
                {
                    DispatcherOperation op = e.Operation;
                    string name = DispatcherHelper.GetNameFromDispatcherOperation(op);
                    if (!String.IsNullOrEmpty(name) && name.IndexOf("InvokeWrapper") != -1)
                    {
                        _invokeAborted = true;
                        _dw.ShutDown();
                    }
                };
    
            DispatcherType dt = DispatcherType.Avalon;
            if (typeOfDispatcher == "Win32")
            {
                dt = DispatcherType.Win32;                
            }
            _dw = new DispatcherWrapper(Dispatcher.CurrentDispatcher, dt);
            _dw.ForceDispatcherShutdown = true; 
            base.GetModelNextAction(null);
            _dw.Run();


            if (_ds == DispatcherState.Shutdown)
            {
                CallingInvoke();
            }

            Validation();
            
        }

        void Validation()
        {
            Thread.Sleep(500);
            if (s_expectingExceptionType != null)
            {                
                if (!_exceptionCaught)
                {

                    if (InvokeWrapperHelper.ExceptionCaught != null)
                    {
                        bool handled = false;
                        ExceptionValidation(InvokeWrapperHelper.ExceptionCaught, ref  handled);
                    }

                    if (!_exceptionCaught)
                    {
                        CoreLogger.LogTestResult(false,"Expecting an exception");
                    }
                }
                return;
            }


            if (_ds == DispatcherState.Shutdown)
            {
                if (!(_inWrapper.Priority == DispatcherPriority.Send &&
                    _inWrapper.InvokeThread == InvokingThread.Dispatcher))
                {
                    if (_invokeCalled || _invokeAborted || _inWrapper.Result != null)
                    {
                        CoreLogger.LogTestResult(false,"The dispatcher has been shutdown. The invoke should not happen.");                    
                    }
                    return;
                }
            }


            if ((_ds == DispatcherState.ShutdownFinished || 
                _ds == DispatcherState.ShutdownStarted) &&
                _inWrapper.InvokeThread != InvokingThread.Dispatcher && 
                _inWrapper.Priority != DispatcherPriority.Send)
            {
                if (!_invokeAborted)
                {
                    CoreLogger.LogTestResult(false,"Expecting the operation will be aborted.");                    
                }
                return;
            }

            if (!_invokeAborted && _nestingInvokeCall && !_nestingCalled)
            {
                CoreLogger.LogTestResult(false,"The nesting called was not executed.");
            }

            if (_isTimeSpanSet && !_longTimeSpan)
            {
                if ((_inWrapper.InvokeThread == InvokingThread.Dispatcher && _inWrapper.Priority == DispatcherPriority.Send)
                   )
                {
                    if (!_invokeCalled)
                    {
                        CoreLogger.LogTestResult(false, "The Invoke didn't succeced even that was Dispatcher and Priority Send.");
                    }
                    return;
                }

                if (_waitForTimeout && _invokeCalled && !_invokeAborted)
                {
                    CoreLogger.LogTestResult(false, "Invoke should be aborted.");
                }
            }
            else
            {
                if (!_invokeCalled)
                {
                    CoreLogger.LogTestResult(false, "The Invoke.");                    
                }


                if (_inWrapper.InvokeCallbackKind == InvokeCallbackTypes.Zero_Param_Generic &&
                    (int)_inWrapper.Result != 1)
                {
                    CoreLogger.LogTestResult(false, "The invoke didn't return the expected value."); 
                }
            }
        }
        
        public void DispatcherInvoke(IDictionary dictionary)
        {
            /*
               dictionary["Priority"] - // 
               dictionary["Timeout"] - // 
               dictionary["Delegate"] - // 
               dictionary["CallingThread"] - // 
               dictionary["Nesting"] - // 
               dictionary["WaitForTimeout"] - // 
               dictionary["DispatcherState"] - // 
            */

            // Timeout parsing section.

            
            TimeSpan timeSpan = ParsingTimeout((string)dictionary["Timeout"], ref _isTimeSpanSet, ref _longTimeSpan);

            // Delegate parsing section.
            
            InvokeCallbackTypes callbackType = ParsingDelegate((string)dictionary["Delegate"]);

            // DispatcherPriority parsing section.

            DispatcherPriority dp = ParsingPriority((string)dictionary["Priority"]);

            // InvokingThread parsing section.

            InvokingThread it = ParsingCallingThread((string)dictionary["CallingThread"]);

            // Nesting parsing section.

            _nestingInvokeCall = ParsingYes((string)dictionary["Nesting"]);

            // waitForTimeout parsing section.
            
            _waitForTimeout = ParsingYes((string)dictionary["WaitForTimeout"]);


            // DispatcherState parsing section.
            
            _ds = ParsingDispatcherState((string)dictionary["DispatcherState"]);


            if (_ds == DispatcherState.ShutdownStarted)
            {
                System.Windows.Threading.Dispatcher.CurrentDispatcher.ShutdownStarted += delegate
                {
                    /*   Calling Dispatcher.InvokeShutdown inside of Dispatcher.ShutdownStarted 
                      event will end up in a stackoverflow */
                    CallingInvoke(); 

                };
            }
            else if (_ds == DispatcherState.ShutdownFinished)
            {
                System.Windows.Threading.Dispatcher.CurrentDispatcher.ShutdownFinished += delegate
                {
                    CallingInvoke(); 
                };
            }    


            if (_waitForTimeout)
            {
                if (timeSpan != TimeSpan.MaxValue /* We don't want to block our thread forever :) */                   
                    )
                {
                    _dw.RealDispatcher.BeginInvoke(DispatcherPriority.Send,
                        (DispatcherOperationCallback)delegate (object otimeSpan)
                        {
                            TimeSpan span = (TimeSpan)otimeSpan;
                            if (span == TimeSpan.MinValue)
                            {
                                span = TimeSpan.FromSeconds(1);
                            }
                  
                            TimeSpan newSpan = span + TimeSpan.FromSeconds(3);

                            CoreLogger.LogStatus("The Dispatcher Thread goes to sleep for " + newSpan.TotalSeconds + " milliseconds");
                            Thread.Sleep(newSpan);
                            
                            return null;
                        }, timeSpan);
                }
            }



            if (it == InvokingThread.Dispatcher && timeSpan == TimeSpan.MinValue)
            {
                /* 

*/
                _longTimeSpan = true;

                /* */
                
                s_expectingExceptionType = null;

            }

            // Depending if we use TimeSpan or Not.
            if (_isTimeSpanSet)
            {
                _inWrapper = new InvokeWrapper(Dispatcher.CurrentDispatcher,it,dp,callbackType,new EventHandler(InvokeCallback),timeSpan);
            }
            else
            {
                _inWrapper = new InvokeWrapper(Dispatcher.CurrentDispatcher,it,dp,callbackType,new EventHandler(InvokeCallback));
            }            

            if (_ds != DispatcherState.ShutdownFinished && 
                _ds != DispatcherState.ShutdownStarted &&
                _ds != DispatcherState.Shutdown
                )
            {
                CallingInvoke();
            }
            else
            {
                _dw.ShutDownNow();
            }          
        }

        void SleepDispatcherThread()
        {
            // We do this just to make just that Invoke is perform on the other thread.
            if (_inWrapper.InvokeThread != InvokingThread.Dispatcher)
            {
                while (!_inWrapper.AlmostInvokeCalled)
                {
                    Thread.Sleep(10);
                }
                Thread.Sleep(10);
            }
        }


        // This wraps the real Invoke Call.
        void CallingInvoke()
        {

            bool handledException = false;
            Exception ex = null;

            try
            {
                _inWrapper.Invoke();

                SleepDispatcherThread();
            }
            catch(Exception e)
            {
                ex = e;
                ExceptionValidation(e,ref handledException); 
            }

            if (!handledException && ex != null)
            {
                throw ex;                
            }
        }


        /// <summary>
        /// This is the actual callback that it is called.
        /// </summary>
        void InvokeCallback (object o, EventArgs args)
        {
            _invokeCalled = true;

            // This for nesting Invoke Calls
            if (_nestingInvokeCall)
            {
                int i = (int)_dw.RealDispatcher.Invoke(DispatcherPriority.Normal,
                    (DispatcherOperationCallback) delegate
                    {
                        CoreLogger.LogStatus("The nesting invoke called.");
                        _nestingCalled = true;
                        return 1900;
                    }, null);

                if (i != 1900)
                {
                    CoreLogger.LogTestResult(false,"The nesting invoke didn't return the correct value");
                }

            }
        
            // We need to shutdown the dispatcher.
            _dw.ShutDown();
        }


        void ExceptionValidation(Exception e, ref bool handled)
        {

            if (s_expectingExceptionType != null)
            {
                if (s_expectingExceptionType == e.GetType())
                {
                    handled = true;
                    _exceptionCaught = true;
                    CoreLogger.LogStatus("Expected Exception Caugth");
                    _dw.ShutDownNow();
                }

            }
        }
        
        void UnhandledExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Dispatcher != _dw.RealDispatcher)
            {
               CoreLogger.LogTestResult(false,"Dispatcher doesn't match.");
            }
            
            bool handled = false;
            ExceptionValidation(e.Exception,ref handled);       
            e.Handled = handled;
        }
        
        private static DispatcherState ParsingDispatcherState(string state)
        {
            DispatcherState ds = (DispatcherState)Enum.Parse(typeof(DispatcherState),state,true /* ignore case */ );
            return ds;
        }


        private static InvokingThread ParsingCallingThread(string invokingThread)
        {
            InvokingThread it = (InvokingThread)Enum.Parse(typeof(InvokingThread),invokingThread,true /* ignore case */ );
            return it;
        }


        private static bool ParsingYes(string yes)
        {
            bool v = (String.Compare(yes, "Yes", true /* Ignoring case */) == 0);
            return v;
        }


        private static DispatcherPriority ParsingPriority(string priority)
        {
            DispatcherPriority dp = (DispatcherPriority)Enum.Parse(typeof(DispatcherPriority),priority,true /* ignore case */ );

            if (dp == DispatcherPriority.Invalid)
            {
                s_expectingExceptionType = typeof(System.ComponentModel.InvalidEnumArgumentException);
            }
            
            if (dp == DispatcherPriority.Inactive)
            {
                s_expectingExceptionType = typeof(ArgumentException);
            }
            
            return dp;
        }


        private static InvokeCallbackTypes ParsingDelegate(string callbackType)
        {
            InvokeCallbackTypes callback = InvokeCallbackTypes.One_Param_DispatcherOperationCallback;
            
            switch(callbackType)
            {
                case "0_args":
                    callback = InvokeCallbackTypes.Zero_Param_Generic;
                    break;

                case "SendorPostCallback":
                    callback = InvokeCallbackTypes.One_Param_SendOrPost;
                    break;

                case "2_args":
                    callback = InvokeCallbackTypes.Two_Param_Generic;
                    break;

                case "3_args":
                    callback = InvokeCallbackTypes.Three_Param_Generic;
                    break;
            }

            return callback;
            
        }

        private static TimeSpan ParsingTimeout(string timeout, ref bool isTimeSpanSet, ref bool isLongTimeSpan)
        {
            TimeSpan timeSpan = new TimeSpan(0);
            isLongTimeSpan = false;
            switch(timeout)
            {
                case "None":
                    isTimeSpanSet = false;
                    break;

                case "_-1":
                    isLongTimeSpan = true;
                    timeSpan = TimeSpan.FromMilliseconds(-1);                   
                    break;


                case "_2000":
                    timeSpan = TimeSpan.FromMilliseconds(2000);                    
                    break;

                case "_60000":
                    timeSpan = TimeSpan.FromMilliseconds(6000);
                    break;                    

                case "TimeSpan_MinValue":
                    timeSpan = TimeSpan.MinValue;
                    s_expectingExceptionType = typeof(ArgumentOutOfRangeException);
                    break;

                case "TimeSpan_MaxValue":
                    timeSpan = TimeSpan.MaxValue;
                    s_expectingExceptionType = typeof(ArgumentOutOfRangeException);
                    break;

                case "TimeSpan_Zero":
                    timeSpan = TimeSpan.Zero;
                    break;                    
            }

            return timeSpan;
            
        }

        // This is for the test case where we expect an exception.
        static Type s_expectingExceptionType = null;
        
        bool _nestingInvokeCall = false;
        bool _invokeCalled = false;
        bool _waitForTimeout = false;
        DispatcherWrapper _dw = null;
        bool _nestingCalled = false;
        InvokeWrapper _inWrapper = null;
        bool _longTimeSpan = false;
        bool _isTimeSpanSet = true;
        DispatcherState _ds = DispatcherState.Running;  
        bool _exceptionCaught = false;
        bool _invokeAborted = false;

    }
}


