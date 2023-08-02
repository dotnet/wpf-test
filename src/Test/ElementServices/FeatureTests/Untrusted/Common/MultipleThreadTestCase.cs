// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Threading;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Win32;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;


namespace Avalon.Test.CoreUI
{

    ///<summary>
    /// This is the pattern and automation for creating a test case with 1 Avalon Dispatcher Thread
    /// and 1 Worker Thread.   
    ///</summary>
    public class WorkerThreadTestCase : MultipleThreadTestCase
    {
        /// <summary>
        /// Constructor with a default sync value of Sequential.
        /// </summary>
        /// <param name="initDispatcher">True value will initialize the Avalon Dispatcher before the Worker Thread ThreadStarted
        /// event.
        /// </param>
        public WorkerThreadTestCase(bool initDispatcher)
            : this(ThreadCaseSynchronization.Sequential, initDispatcher)
        {

        }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sync">Using the specified synchronization.</param>
        /// <param name="initDispatcher">True value will initialize the Avalon Dispatcher before the Worker Thread ThreadStarted.
        /// event
        /// </param>
        public WorkerThreadTestCase(ThreadCaseSynchronization sync, bool initDispatcher)
            : base(sync)
        {
            _initDispatcher = initDispatcher;
        }

        /// <summary>
        /// Initialize all the callbacks that are going to be used on the test case.
        /// </summary>
        /// <param name="DispatcherThreadStarted">
        /// This delegate will be executed when the Dispatcher thread starts and before the Dispatcher start running.
        /// </param>
        /// <param name="DispatcherThreadFirstIdle">
        /// This delegate will be executed on the first SystemIdle that the Dispatcher achieve.
        /// </param>
        /// <param name="DispatcherThreadBeforeEnded">
        /// This delegate will be executed after the dispatcher stops and before the thread exits.
        /// </param>
        /// <param name="WorkerThreadStarted">
        /// This delegate will be executed when the Worker thread starts.
        /// </param>
        /// <param name="WorkerThreadBeforeEnded">
        /// This delegate will be executed before the Worker thread ends.
        /// </param>
        public void Initialize(ThreadTestCaseCallback DispatcherThreadStarted,
            ThreadTestCaseCallback DispatcherThreadFirstIdle,
            ThreadTestCaseCallback DispatcherThreadBeforeEnded,
            ThreadTestCaseCallback WorkerThreadStarted,
            ThreadTestCaseCallback WorkerThreadBeforeEnded)
        {
            // Setting up the Dispatcher Thread work
            
            ThreadCaseInfo dispatcherThreadInfo = new ThreadCaseInfo();
            dispatcherThreadInfo.ThreadStarted += DispatcherThreadStarted;
            dispatcherThreadInfo.DispatcherFirstIdleDispatched += DispatcherThreadFirstIdle;
            dispatcherThreadInfo.ThreadBeforeEnd += DispatcherThreadBeforeEnded;
            dispatcherThreadInfo.MessagePump = DispatcherType.Avalon;
            dispatcherThreadInfo.InitializeAvalonDispatcher = _initDispatcher;


            // Setting up the Worker Thread work

            ThreadCaseInfo workerThreadInfo = new ThreadCaseInfo();
            workerThreadInfo.ThreadStarted += WorkerThreadStarted;
            workerThreadInfo.ThreadBeforeEnd += WorkerThreadBeforeEnded;

            ThreadCaseInfo[] infoArray = {dispatcherThreadInfo, workerThreadInfo};

            Initialize(infoArray);

        }

        ///<summary>
        /// Retrieve the Avalon Dispatcher reference for this test case.
        ///</summary>
        public Dispatcher CurrentDispatcher
        {
            get
            {
                if (ThreadCaseInfoArray != null && ThreadCaseInfoArray[0].Thread != null)
                {
                    return Dispatcher.FromThread(ThreadCaseInfoArray[0].Thread);
                }

                return null;                    
            }
        }

        private bool _initDispatcher = false;
    }

    ///<summary>
    /// Specify the Synchronization for the Threads on the test case
    ///</summary> 
    public enum ThreadCaseSynchronization
    {
        ///<summary>
        /// The test case will execute the treads on the order specify on the Initialize Array.
        /// The only synchronization that we ensure is that ThreadStarted will be called in order.
        ///</summary> 
        Sequential,

        ///<summary>
        /// The test case will execute the threads without any synchronization at all.  They will
        /// run free.
        ///</summary> 
        None
    }

    ///<summary>
    /// This class encapsulates and creates a pattern for creation and execution of multi-threaded 
    /// test case.
    ///</summary>
    public class MultipleThreadTestCase
    {

        ///<summary>
        /// Creating a MultipleThreadCase with Synchronization = Sequential.
        ///</summary>
        public MultipleThreadTestCase() : this(ThreadCaseSynchronization.Sequential) { }

        /// <summary>
        /// Creating a MultipleThreadCase with the specified synchronization.
        /// </summary>
        /// <param name="sync"></param>
        public MultipleThreadTestCase(ThreadCaseSynchronization sync)
        {
            Synchronization = sync;
        }


        /// <summary>
        /// Initialize the instance with the specified data.
        /// </summary>
        public virtual void Initialize(ThreadCaseInfo[] info)
        {
            if (info == null)
            {   
                throw new ArgumentNullException("info");
            }

            if (info.Length <= 0)
            {
                throw new ArgumentOutOfRangeException("The array should not be empty");
            }

            if (ThreadCaseInfoArray == null || ThreadCaseInfoArray.Length == 0)
            {
                ThreadCaseInfoArray = info;
            }
            else
            {
                List<ThreadCaseInfo> tempCache = new List<ThreadCaseInfo>();
                tempCache.AddRange(ThreadCaseInfoArray);
                tempCache.AddRange(info);
                ThreadCaseInfoArray = tempCache.ToArray();                
            }
                

        }

        /// <summary>
        /// The event is signaled after all threads are done.
        /// </summary>
        public ManualResetEvent TestCompletedWaitHandle
        {
            get
            {
                return _testCompletedWaitHandle;
            }
        }


        ///<summary>
        /// This is the method in charge of creating and running the CLR Threads.
        ///</summary>
        private void StartThreadsExecution()
        {
            // Loop through the array to start all the threads
            for (int i = 0; i < ThreadCaseInfoArray.Length; i++)
            {                          
                ThreadCaseInfo currentInfo = ThreadCaseInfoArray[i];

                // Setting the relationship
                currentInfo.Owner = this;
                
                Thread thread = new Thread(new ThreadStart(WorkerThread));

                // Set the real thread to the ThreadCaseInfo
                currentInfo.Thread = thread;

                // This is our way to track Thread vs ThreadCaseInfo
                lock (_dataTable)
                {
                    _dataTable.Add(thread, currentInfo);
                }

                // Sets the Apartment state requested for this thread
                if (currentInfo.ThreadApartmentState == ApartmentState.STA || currentInfo.MessagePump == DispatcherType.Avalon)
                {
                    thread.SetApartmentState(ApartmentState.STA);
                }

                thread.Start();


                if (Synchronization == ThreadCaseSynchronization.Sequential && i != ThreadCaseInfoArray.Length-1)
                {
                    // We are going to wait until the ThreadStarted was
                    // happen before starting a new Thread                    
                    s_waitEvent.WaitOne();
                }             
            }            
        }

        /// <summary>
        ///  Starts the real execution of the test case. Block until all threads finish
        /// </summary>
        public void Run()
        {
            Start();
            
            // Wait for all the threads to finish
            // Blocking this thread until everything is done
            for (int i = 0; i < ThreadCaseInfoArray.Length; i++)
            {
                ThreadCaseInfo currentInfo = ThreadCaseInfoArray[i];
                currentInfo.Thread.Join();
            }
            
            // Loop through all the thread to make sure there were no
            // exceptions or errors            
            for (int i = 0; i < ThreadCaseInfoArray.Length; i++)
            {
                ThreadCaseInfo currentInfo = ThreadCaseInfoArray[i];
                
                if (currentInfo.ThreadException != null)
                {
                    throw currentInfo.ThreadException;
                }
            }            
        }

        /// <summary>
        ///  Starts the real execution of the test case. 
        /// </summary>
        public void Start()
        {
            if (ThreadCaseInfoArray == null)
            {
                throw new InvalidOperationException("You should call Initialize before calling this API.");
            }

            StartThreadsExecution();            
        }

        /// <summary>
        ///  
        /// </summary>
        public bool IsTestCompleted
        {
            get
            {
                return ThreadCaseInfoArray.Length == _completedThreads;
            }   
        }

        /// <summary>
        /// Execute the real code for each thread that was set up during
        /// initialization.
        /// </summary>
        public void WorkerThread()
        {
            Thread currentThread = Thread.CurrentThread;

            GlobalLog.LogStatus("Worker Thread Start. Thread # " + currentThread.GetHashCode().ToString());            
            ThreadCaseInfo tcInfo;
            lock(_dataTable)
            {
                tcInfo = _dataTable[Thread.CurrentThread] as ThreadCaseInfo;
            }
            try
            {

                
                // This only is you want to make sure there is
                // a dispatcher available                
                if (tcInfo.InitializeAvalonDispatcher)
                {
                    Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
                }

                GlobalLog.LogDebug("Before ThreadStarted. Thread # " + currentThread.GetHashCode().ToString()); 
                // Calling out ThreadStarted event
                tcInfo.OnThreadStarted();
                GlobalLog.LogDebug("After ThreadStarted. Thread #  " + currentThread.GetHashCode().ToString()); 

                // This is made to make sure the next thread doesn't
                // starts until the ThreadStarted is completed for the current
                // thread

                if (this.Synchronization == ThreadCaseSynchronization.Sequential)
                {
                    s_waitEvent.Set();
                }

                if (tcInfo.MessagePump != DispatcherType.None)
                {
                    DispatcherHelper.EnqueueCallback( 
                        DispatcherPriority.SystemIdle, 
                        new DispatcherOperationCallback( delegate (object notUsed)
                        {
                            GlobalLog.LogDebug("Worker Thread Before FirstIdle. Thread #  " + currentThread.GetHashCode().ToString());                             
                            tcInfo.OnDispatcherFirstIdleDispatched();
                            GlobalLog.LogDebug("Worker Thread Aftert FirstIdle. Thread #  " + currentThread.GetHashCode().ToString());                             
                            return null;
                        }), 
                        tcInfo);
                }

                RunDispatcher(tcInfo);

                // The thread is about to go away we fire   
                // ThreadBeforeEnd event
                GlobalLog.LogDebug("Worker Thread Before ThreadEnd. Thread #  " + currentThread.GetHashCode().ToString()); 
                tcInfo.OnThreadBeforeEnd();
                GlobalLog.LogDebug("Worker Thread After ThreadEnd. Thread #  " + currentThread.GetHashCode().ToString()); 


            }
            catch(Exception e)
            {
                GlobalLog.LogStatus("Exception in Thread:" + currentThread.GetHashCode().ToString()); 
                GlobalLog.LogStatus(e.Message); 
                GlobalLog.LogStatus(e.StackTrace.ToString()); 
                tcInfo.ThreadException = e;

                lock(_rootSync)
                {
                    _exceptionList.Add(e);
                }
            }
            finally
            {
                GlobalLog.LogDebug("Worker Thread End. Thread #  " + currentThread.GetHashCode().ToString());            
                lock(_rootSync)
                {
                    _completedThreads++;
                }
                if (_completedThreads == ThreadCaseInfoArray.Length)
                {
                    _testCompletedWaitHandle.Set();
                }
            }

        }

        /// <summary>
        /// Run the specified dispatcher
        /// </summary> 
        protected virtual void RunDispatcher(ThreadCaseInfo tcInfo)
        {
            // Only if we are using Avalon Dispatcher we
            // fired the DispatcherFirstIdleDispatched event
            if (tcInfo.MessagePump == DispatcherType.Avalon)
            {
                // Running Avalon Dispatcher                    
                DispatcherHelper.RunDispatcher();
            }
            else if (tcInfo.MessagePump == DispatcherType.Win32)
            {
                Win32GenericMessagePump.Run();
            }
        }


        /// <summary>
        /// Array with all the ThreadCaseInfo
        /// </summary> 
        public Exception[] ExceptionList
        {
            get
            {
                return _exceptionList.ToArray();
            }
        }

        private List<Exception> _exceptionList = new List<Exception>();

        /// <summary>
        /// Array with all the ThreadCaseInfo
        /// </summary> 
        protected ThreadCaseInfo[] ThreadCaseInfoArray = null;

        /// <summary>
        /// Holds the type of Synchronization used on the test case
        /// </summary> 
        protected ThreadCaseSynchronization Synchronization;

        private ManualResetEvent _testCompletedWaitHandle = new ManualResetEvent(false);
        private Hashtable _dataTable = new Hashtable();
        private static AutoResetEvent s_waitEvent = new AutoResetEvent(false);
        private object _rootSync = new Object();
        int _completedThreads = 0;
    }


    /// <summary>
    /// Delegate signature use on to call the test code from the MultiThread
    /// </summary>
    /// <param name="info">Information that was built by the user for this thread</param>
    /// <param name="args">Not used</param>
    public delegate void ThreadTestCaseCallback(ThreadCaseInfo info, EventArgs args);


    /// <summary>
    /// Thread description. This class sets up all the information for the thread
    /// to perform during the test case run.
    /// </summary>
    public class ThreadCaseInfo 
    {
        ///<summary>
        /// Constructor
        ///</summary>
        public ThreadCaseInfo(){}

        ///<summary>
        /// Constructor
        ///</summary>
        public ThreadCaseInfo(object argument)
        {
            _argument = argument;
        }

        /// <summary>
        /// Setting True will initialize Avalon dispatcher before the any handler is called
        /// </summary>
        public bool InitializeAvalonDispatcher 
        {
            get
            {
                return _initializeAvalonDispatcher;
            }
            set
            {                
                _initializeAvalonDispatcher = value;                    
            }
        }

        /// <summary>
        /// This event will be fired when the thread starts.
        /// </summary>
        public event ThreadTestCaseCallback ThreadStarted;


        /// <summary>
        /// Raise the ThreadStarted Event
        /// </summary>
        public void OnThreadStarted()
        {
            if (this.ThreadStarted != null)
            {
                this.ThreadStarted(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// If the MessagePump type is Avalon Dispatcher, this event will
        /// be raise during the first Idle priority dispatched item
        /// </summary>
        public event ThreadTestCaseCallback DispatcherFirstIdleDispatched;

        /// <summary>
        /// Raise the DispatcherFirstIdleDispatched event
        /// </summary>
        public void OnDispatcherFirstIdleDispatched()
        {
            if (this.DispatcherFirstIdleDispatched != null)
            {
                this.DispatcherFirstIdleDispatched(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// This event is fired before the Thread is going away.  
        /// If a dispatcher was set, this event happens when the dispatcher
        /// is already stopped.
        /// </summary>
        public event ThreadTestCaseCallback ThreadBeforeEnd;

        ///<summary>
        /// Raise the ThreadBeforeEnd event
        ///</summary>
        public void OnThreadBeforeEnd()
        {
            if (this.ThreadBeforeEnd != null)
            {
                this.ThreadBeforeEnd(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Type of message pump that will be running.
        /// </summary>
        /// <remarks>
        /// If MessagePump type is equal to Avalon, we set the apartment state
        /// of the thread to be STA
        /// </remarks>
        public DispatcherType MessagePump
        {
            get
            {
                return _messagePump;
            }
            set
            {
                if (value == DispatcherType.Avalon)
                {
                    ThreadApartmentState = ApartmentState.STA;                    
                }

                _messagePump = value;
            }
        }
        

        /// <summary>
        /// Links the this class to the real thread.
        /// </summary>
        public Thread Thread
        {
            set
            {
                if (_thread == null)
                    _thread = value;
            }
            get
            {
                return _thread;
            }
        }

        /// <summary>
        /// Requesting the Apartment state for the Thread
        /// </summary>
        public ApartmentState ThreadApartmentState = ApartmentState.MTA;

        /// <summary>
        /// Holds any exception that happens on any of the thread during the execution
        /// of the code
        /// </summary>
        public Exception ThreadException;


        /// <summary>
        /// Argument specified on the ctor.
        /// </summary>
        public object Argument
        {
            get
            {
                return _argument;
            }
        }

        /// <summary>
        /// Argument specified on the ctor.
        /// </summary>
        public MultipleThreadTestCase Owner
        {
            get
            {
                return _owner;
            }
            set
            {
                _owner = value;
            }
        }

        private MultipleThreadTestCase _owner = null;
        private object _argument = null;
        private Thread _thread = null;
        private bool _initializeAvalonDispatcher = false;
        private DispatcherType _messagePump = DispatcherType.None;

    }

}

