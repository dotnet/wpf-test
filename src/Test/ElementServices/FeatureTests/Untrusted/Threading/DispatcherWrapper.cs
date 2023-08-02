// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************************
*    Source Control Information
*    
 
  
*    Revision:         $Revision: 1 $
 
*
******************************************************************************/
using System;
using System.Collections;
using Microsoft.Test.Threading;
using System.Collections.Generic;
using Microsoft.Test.Modeling;
using System.Windows.Threading;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Win32;
using System.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.Threading
{
    /// <summary>
    /// This is a convinience class that wraps a Dispatcher object
    /// </summary>
    public class DispatcherWrapper
    {        

        /// <summary>
        /// Returns the a DispatcherWrapper from a Dispatcher object
        /// </summary>
        public static DispatcherWrapper FromCurrentDispatcher()
        {
            return FromDispatcher(Dispatcher.CurrentDispatcher);
        }

        /// <summary>
        /// Returns the a DispatcherWrapper from a Dispatcher object
        /// </summary>
        public static DispatcherWrapper FromDispatcher(Dispatcher d)
        {
            DispatcherWrapper dW = null;

            lock(s_globalObject)
            {
                dW = (DispatcherWrapper)s_dispatchers[d];
            }
            
            return dW;
        }
        
        /// <summary>
        /// </summary>    
        public DispatcherWrapper(Dispatcher dispatcher, DispatcherType dispatcherType)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                s_isDebuggerAttached = true;
            }

            _dispatcherType = dispatcherType;
            DispatcherPriority[] priorityList = DispatcherPriorityHelper.GetValidDispatcherPriorities();

            for (int i = 0; i < priorityList.Length; i++)
            {                
                _prioritiesItemsCount[priorityList[i]] = 0;
            }

            if (dispatcher != null)
            {
                Initialize(dispatcher);
            }
        }

        /// <summary>
        /// </summary> 
        public DispatcherType CurrentDispatcherType
        {
            get
            {
                return _dispatcherType;
            }
        }


        /// <summary>
        /// </summary> 
        public static DispatcherWrapper[] CreateRandomDispatcherWrappers(IDictionary dictionary)
        {

            string argAmountOfDispatchers = (string)dictionary["AmountOfDispatchers"];
            string argSeedForDispatcherType = (string)dictionary["SeedForDispatcherType"];

            if (argSeedForDispatcherType == null || argSeedForDispatcherType == "")
            {
                throw new ArgumentException("Missing SeedForDispatcherType value.", "dictionary");
            }

            int amountOfDispatchers = 1;            

            
            if (String.Compare(argAmountOfDispatchers,"Single", true) != 0)
            {
                string argAmountMultipleDispatchers = (string)dictionary["AmountMultipleDispatchers"];

                if (argAmountMultipleDispatchers == null || argAmountMultipleDispatchers == "")
                {
                    amountOfDispatchers = 3;
                }
                else
                {
                    amountOfDispatchers = Int32.Parse(argAmountMultipleDispatchers);
                }
            }

            int seedRandomDispatcherType = Int32.Parse(argSeedForDispatcherType);

            return CreateRandomDispatcherWrappers(amountOfDispatchers,seedRandomDispatcherType);

        }

        /// <summary>
        /// </summary> 
        public static DispatcherWrapper[] CreateRandomDispatcherWrappers(
            int amountOfDispatchers,
            int seedRandomDispatcherType)
        {


            List<DispatcherWrapper> dispatcherWrapperList = new List<DispatcherWrapper>();
           
            Random randomDispatcherType = new Random(seedRandomDispatcherType);

            for (int i = 0; i < amountOfDispatchers; i++)
            {
                DispatcherType dispatcherType = DispatcherType.Avalon;
                if (amountOfDispatchers != 1)
                {
                    int x = randomDispatcherType.Next(0,99);
                    if (x % 2 == 0)
                    {
                        dispatcherType = DispatcherType.Win32;
                    }
                }                
                DispatcherWrapper dW = new DispatcherWrapper(null,dispatcherType);
                dispatcherWrapperList.Add(dW);
            }
            return dispatcherWrapperList.ToArray();
        }



        /// <summary>
        /// </summary>
        public static void DispatcherCallbackOne(object o)
        {
            CommonValidation(o);
        }

        /// <summary>
        /// </summary>
        public static void DispatcherCallbackTwo(object o1, object o2)
        {
            CommonValidation(o1);
        }

        /// <summary>
        /// </summary>
        public static void DispatcherCallbackZero()
        {
            CommonValidation(null);
        }

        /// <summary>
        /// </summary>
        public static object DispatcherOpCallback(object o)
        {   
            CommonValidation(o);
            return null;
        }

        /// <summary>
        /// This keeps our record about the amount for expected items posted
        /// </summary>  
        public void AddPriorityCount(DispatcherPriority priority)
        {
            PriorityCountOp(priority,"+");
        }


        /// <summary>
        /// </summary>  
        public DispatcherHooksWrapper Hooks
        {
            get
            {
                return _dispatcherHooksWrapper;
            }
        }

        private DispatcherHooksWrapper _dispatcherHooksWrapper = null;

        
        /// <summary>
        /// </summary>    
        public DispatcherOperation BeginInvoke(ThreadDispatcherAction action)
        {
            DispatcherOperation operation = null;
                        
            if (action.Params.Length == 0)
            {
                operation = _dispatcher.BeginInvoke(action.Priority, action.Callback);
            }
            else if (action.Params.Length == 1)
            {
                operation = _dispatcher.BeginInvoke(action.Priority, action.Callback, action.Params[0]);
            }
            else if (action.Params.Length == 2)
            {
                operation = _dispatcher.BeginInvoke(action.Priority, action.Callback, action.Params[0], action.Params[1]);
            }

            return operation;            
        }
        
        /// <summary>
        /// </summary>  
        public void RemovePriorityCount(DispatcherPriority priority)
        {
            PriorityCountOp(priority,"-");
        }

        /// <summary>
        /// </summary>  
        public void Run()
        {
            if (_dispatcherType == DispatcherType.Avalon)
            {
                Log("Avalon Dispatcher Run. " + Thread.CurrentThread.GetHashCode());
                DispatcherHelper.RunDispatcher();
            }
            else
            {
                _win32GenericObj = Win32GenericMessagePump.Current;
                Log("Win32 Dispatcher Run. " + Thread.CurrentThread.GetHashCode());
                Win32GenericMessagePump.Run();

                // HACK! for 
                if (_forceDispatcherShutdown)
                {
                    Console.WriteLine("Invoking ShutDown");
                    DispatcherHelper.ShutDown(this.RealDispatcher);
                    Console.WriteLine("ExitInvoking ShutDown");
                }

                
            }
            Log("Exiting Dispatcher. " + Thread.CurrentThread.GetHashCode());
        }


        private Win32GenericMessagePumpObj _win32GenericObj = null;

        /// <summary>
        /// </summary>  
        public int CurrentNestingLevel
        {
            get
            {
                return _currentNestingLevel;
            }
        }

        /// <summary>
        /// </summary>  
        public int DeepestNestingLevel
        {
            get
            {
                return _deepestNestingLevel;
            }
        }

        private int _currentNestingLevel = 0; 

        private int _deepestNestingLevel = 0; 

        /// <summary>
        /// </summary>  
        public void PushNestedLoop(DispatcherType dispatcherType)
        {
            RealDispatcher.VerifyAccess();
            
            _currentNestingLevel ++;

            if (_currentNestingLevel > _deepestNestingLevel)
            {
                _deepestNestingLevel = _currentNestingLevel;
            }

            try
            {
                if (dispatcherType == DispatcherType.Avalon)
                {
                    DispatcherFrame frame = new DispatcherFrame();
                    _messageLoopStack.Push(new NestedObject(frame,dispatcherType));

                    Log("Entering Avalon Nested Pump. Thread # " + Thread.CurrentThread.GetHashCode());                
                    DispatcherHelper.PushFrame(frame);             
                    Log("Leaving Avalon Nested Pump. Thread # " + Thread.CurrentThread.GetHashCode());                
                }
                else
                {
                    Log("Entering Win32 Nested Pump. Thread # " + Thread.CurrentThread.GetHashCode());                
                    _messageLoopStack.Push(new NestedObject(dispatcherType));
                    Win32GenericMessagePump.Run();
                    Log("Leaving Win32 Nested Pump. Thread # " + Thread.CurrentThread.GetHashCode());                
                }
            }
            finally
            {
                _currentNestingLevel --;
            }
        }

        /// <summary>
        /// </summary>  
        public void PopNestedLoop()
        {
            RealDispatcher.VerifyAccess();
            
            NestedObject nestedObject = (NestedObject)_messageLoopStack.Pop();
            
            if (nestedObject.CurrentDispatcherType == DispatcherType.Avalon)
            {
                DispatcherFrame frame = nestedObject.Frame;               
                frame.Continue = false;
            }
            else
            {
                Win32GenericMessagePump.Stop();
            }
            
        }
        private Stack _messageLoopStack = new Stack();
        

        /// <summary>
        /// </summary>    
        public DispatcherPriority LastDispatchedPriority
        {
            get
            {
                return _lastDispatchedPriority;
            }
        }

        /// <summary>
        /// Access to the real Dispatcher object
        /// </summary>
        public Dispatcher RealDispatcher
        {
            get
            {
                return _dispatcher;
            }
        }

        /// <summary>
        /// Access to the real queue inside of the Dispatcher object
        /// </summary>
        public DispatcherQueueWrapper DispatcherQueue
        {
            get
            {
                return _dqw;
            }
        }


        /// <summary>
        /// Set the Dispatcher 
        /// </summary>
        public void SetDispatcher(Dispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }

            Initialize(dispatcher);
            
        }   




        public bool ForceDispatcherShutdown
        {
            get
            {
                return _forceDispatcherShutdown;
            }
            set
            {
                _forceDispatcherShutdown = value;
            }
        }

        private bool _forceDispatcherShutdown = false;


        /// <summary>
        /// </summary> 
        public void ShutDownNow()
        {
            ShutDown(true);
        }

        /// <summary>
        /// </summary>  
        public void ShutDown()
        {
            ShutDown(false);
        }

        /// <summary>
        /// </summary>  
        private void ShutDown(bool sync)
        {
            if (!sync)
            {
                this.RealDispatcher.BeginInvoke(DispatcherPriority.SystemIdle,
                    (DispatcherOperationCallback)delegate (object notUsed)
                    {
                        ShutDownPrivate();
                        return null;
                    }, null);
            }
            else
            {
                ShutDownPrivate();
            }
        }

        private void ShutDownPrivate()
        {
            _startShutdown = true; 

            if (_dispatcherType == DispatcherType.Avalon)
            {
                DispatcherHelper.ShutDown(this.RealDispatcher);
            }
            else                        
            {                        
                _win32GenericObj.Stop();
            }
        }


        /// <summary>
        /// </summary>  
        public bool ShutdownStarted
        {
            get
            {
                return _startShutdown;
            }                
        }

        private bool _startShutdown = false;

        /// <summary>
        /// </summary> 
        public void PrintCount()
        {
            for (int i = _prioritiesItemsCount.Count - 1; i >= 0; i--)
            {
                DispatcherPriority priority = (DispatcherPriority)_prioritiesItemsCount.GetKey(i);
                Log("Priority: " + priority.ToString()
                    + " Count:" + ((int)_prioritiesItemsCount[priority]).ToString());
                
            }
        }

        /// <summary>
        /// </summary>  
        public void ValidateNoHigherPriorityItemsAreOnTheQueue(DispatcherPriority priority, bool includePriority)
        {
            if (ValidateOrdering)
            {
                for (int i = _prioritiesItemsCount.Count - 1; i >= 0; i--)
                {
                    if ((DispatcherPriority)_prioritiesItemsCount.GetKey(i) == priority && !includePriority)
                    {
                        break;
                    }

                    if ((int)_prioritiesItemsCount[_prioritiesItemsCount.GetKey(i)] != 0)
                    {
                        throw new Microsoft.Test.TestValidationException("We are items with higher priority on the queue.");
                    }

                    if ((DispatcherPriority)_prioritiesItemsCount.GetKey(i) == priority && includePriority)
                    {
                        break;
                    }

                }
            }
        }


        /// <summary>
        /// </summary>  
        public void ValidateAllItemsBeenDispatched()
        {
            ValidateNoHigherPriorityItemsAreOnTheQueue(DispatcherPriority.SystemIdle, true);
        }

        /// <summary>
        /// If we want to validate that our code priorities count is validated
        /// </summary>
        public bool ValidateOrdering
        {
            get
            {
                return _validateOrdering;
            }
            set
            {       
                _validateOrdering = value;
            }
        }

        static ThreadDispatcherAction BuildThreadDispatcherAction(Delegate callback, object[] parameters)
        {
            ThreadDispatcherAction action = new ThreadDispatcherAction(DispatcherPriority.Invalid, callback,parameters);

            if (parameters != null && parameters.Length > 0)
            {
                parameters[0] = action;
            }

            return action;                
        }

        /// <summary>
        /// This method return the callback and parameters needed for the callback
        /// The input is on the form of "2_null_null" 2 parameters , null and null
        /// </summary>
        public static void GetCallback(string input, ref ThreadDispatcherAction ThreadDispatcherAction)
        {
            List<object> objectList = new List<object>();
            object[] parameters;
            Delegate callback;
            
            // If there is _ on argument it mean that it is a number the value.
            if (input.IndexOf("_") == -1)
            {
                parameters = objectList.ToArray();
                callback = new ZeroArgDelegate(DispatcherCallbackZero);
                ThreadDispatcherAction = BuildThreadDispatcherAction(callback, parameters);
                return;
            }
            
            string[] args = input.Split("_".ToCharArray());            

            if (args == null || (args.Length != 3 && args.Length != 2))
            {
                throw new ArgumentException("","input");
            }

            int amountParams = Int32.Parse(args[0]);

            objectList.Add(new Object());

            if (amountParams == 1)
            {
                parameters = objectList.ToArray();
                if (String.Compare(args[1],"docallback", true) == 0)
                {
                    callback = new DispatcherOperationCallback(DispatcherOpCallback);
                }
                else
                {   
                    callback = new OneArgDelegate(DispatcherCallbackOne);
                }
                
                ThreadDispatcherAction = BuildThreadDispatcherAction(callback, parameters);
                return;
            }

            objectList.Add(new Object());

            if (amountParams == 2)
            {
                parameters = objectList.ToArray();
                callback = new TwoArgDelegate(DispatcherCallbackTwo);
                ThreadDispatcherAction = BuildThreadDispatcherAction(callback, parameters);
                return;
            }
            
            ThreadDispatcherAction = null;
        }

    
        /// <summary>
        /// Common Validation for all the delegates that are going to be posted.
        /// </summary>        
        private static void CommonValidation(object o)
        {
            if (o != null)
            {
                ThreadDispatcherAction ThreadDispatcherAction = (ThreadDispatcherAction)o;

                if (ThreadDispatcherAction.Priority == DispatcherPriority.Inactive)
                {
                    throw new Microsoft.Test.TestValidationException("Inactive Priority should not be process");
                }
                
                ThreadDispatcherAction.DispatcherW.ValidateNoHigherPriorityItemsAreOnTheQueue(ThreadDispatcherAction.Priority, false);
                ThreadDispatcherAction.DispatcherW.RemovePriorityCount(ThreadDispatcherAction.Priority);
                LogDebuggerAttach("Dispatching Priority: " + ThreadDispatcherAction.Priority);
            }
        } 

        void Initialize(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;

            _dispatcherHooksWrapper = new DispatcherHooksWrapper(this);

            // We save this for lookup purpuses.
            lock(s_globalObject)
            {
                s_dispatchers.Add(dispatcher, this);
            }
            
            _dqw = new DispatcherQueueWrapper(_dispatcher);

            DispatcherHooks hooks = DispatcherHelper.GetHooks(_dispatcher);
            hooks.OperationCompleted += new DispatcherHookEventHandler(OperationCompleteNotification);            
        }

        /// <summary>
        /// Everytime that this dispatcher complete an operation, this is called and update our data.
        /// </summary>
        void OperationCompleteNotification(object sender, DispatcherHookEventArgs e)
        {
            _lastDispatchedPriority = e.Operation.Priority;
        }

        /// <summary>
        /// Common code for operation on our internal priority counts.
        /// </summary>
        private void PriorityCountOp(DispatcherPriority priority, string operation)
        {
            lock(_rootObject)
            {
                int count = (int)_prioritiesItemsCount[priority];  

                if (operation == "-")
                {
                    count--;
                }
                else
                {
                    count++;
                }
                  
                _prioritiesItemsCount[priority] = count;
            }
        }

        static void LogDebuggerAttach(string s)
        {
            if (s_isDebuggerAttached)
                Log(s);
        }

        static void Log(string s)
        {
            GlobalLog.LogStatus(s);
        }

        static bool s_isDebuggerAttached = false;
        bool _validateOrdering = false;
        DispatcherPriority _lastDispatchedPriority;        
        Dispatcher _dispatcher = null;
        object _rootObject = new Object();
        SortedList _prioritiesItemsCount = new SortedList();
        DispatcherType _dispatcherType;
        static Hashtable s_dispatchers = new Hashtable();
        static object s_globalObject = new object();
        DispatcherQueueWrapper _dqw = null;



        class NestedObject
        {
            public NestedObject( DispatcherType dispatcherType)
            {
                Frame = null;
                CurrentDispatcherType = dispatcherType;
            }
            
            public NestedObject(DispatcherFrame frame, DispatcherType dispatcherType)
            {
                Frame = frame;
                CurrentDispatcherType = dispatcherType;
            }
            
            public DispatcherFrame Frame;
            public DispatcherType CurrentDispatcherType;
        }
    }

}

