// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Collections;
using Avalon.Test.CoreUI.Threading;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI
{
    /// <summary>
    /// </summary>
    public class ThreadTest
    {
        /// <summary>
        /// </summary>
        public ThreadTest(string friendlyName, ApartmentState state)
        {
            _ev = new AutoResetEvent(false);
            _queue = new Queue();
            _thread = new Thread(new ThreadStart(WorkerCallback));
            _thread.Name = friendlyName;
            _thread.SetApartmentState(state);
            _thread.Start();
        }

        /// <summary>
        /// </summary>
        public event EventHandler ExceptionThrown;

        /// <summary>
        /// </summary>
        public Exception Exception;


        /// <summary>
        /// </summary>
        public TestThreadOperation BeginInvoke(TestThreadOperationCallback callback, object arg)
        {
            TestThreadOperation operation = new TestThreadOperation(callback, arg);
            
            lock(_syncObject)
            {                
                _queue.Enqueue(operation);            
                _ev.Set();
            }
            return operation;
        }

        /// <summary>
        /// </summary>
        public void Stop()
        {
            lock(_syncObject)
            {
                _exitRequested = true;
                _ev.Set();
            }
        }

        private void WorkerCallback()
        {
            while (!_exitRequested)
            {
                if (_queue.Count == 0)
                {
                    _ev.WaitOne();
                }
                else
                {
               
                    TestThreadOperation operation = null;            
                    
                    lock(_syncObject)
                    {
                        operation = (TestThreadOperation)_queue.Dequeue();
                    }
                    try
                    {
                        operation.Result = operation.Invoke();
                    }
                    catch(Exception e)
                    {
                        Exception = e;

                        if (ExceptionThrown != null)
                        {
                            ExceptionThrown(e, EventArgs.Empty);
                        }
                    }                                    
                }                
            }            
        }

        object _syncObject = new Object();
        bool _exitRequested = false;
        AutoResetEvent _ev = null;        
        Thread _thread = null;
        Queue _queue = null;
    }


    /// <summary>
    /// </summary>
    public class TestThreadOperation
    {
        /// <summary>
        /// </summary>
        internal TestThreadOperation(TestThreadOperationCallback callback, object args)
        {
            _callback = callback;
            _args = args;
        }

        /// <summary>
        /// </summary>
        internal object Invoke()
        {
            return _callback(_args);
        }

        /// <summary>
        /// </summary>
        public object Result = null;

        TestThreadOperationCallback _callback;
        object _args = null;
    }

    /// <summary>
    /// </summary>
    public delegate object TestThreadOperationCallback(object o);

}
