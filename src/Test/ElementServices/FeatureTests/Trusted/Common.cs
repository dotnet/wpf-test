// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Threading;
using System.Windows.Threading;
using System.Runtime.Serialization;

namespace Avalon.Test.CoreUI.Trusted
{

    /// <summary>
    /// This class work similar to AutoResetEvent but the waitOne is different. This wait for a specific number of Set() before unblocking
    /// the Thread where WaitOne was called.
    /// </summary>
    public class CoreAutoResetEvent
    {
        /// <summary>
        /// Constructor for CoreAutoResetEvent
        /// </summary>
        /// <param name="initialState">Set True/False</param>
        /// <param name="NumberSet">Number of Set calls necessary to unblock the WaitOne call</param>
        public CoreAutoResetEvent(bool initialState, int NumberSet)
        {
            _ev = new AutoResetEvent(initialState);
            _numberSet = NumberSet;
        }

        /// <summary>
        /// Similar that doing AutoResetEvent.WaitOne
        /// </summary>
        /// <returns></returns>
        public bool WaitOne()
        {
            return _ev.WaitOne();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Set()
        {
            lock (this)
            {
                _counterSet++;
            }

            if (_counterSet == _numberSet)
            {
                return _ev.Set();
            }

            return false;
        }

        /// <summary>
        /// Holds the reference for the AutoResetEvent
        /// </summary>
        public WaitHandle AutoEvent
        {
            get
            {
                return _ev;
            }
        }

        /// <summary>
        /// Holds the reference for the AutoResetEvent
        /// </summary>
        private AutoResetEvent _ev = null;

        /// <summary>
        /// Holds the reference of expected number of Set
        /// </summary>
        private int _numberSet = 1;

        /// <summary>
        /// Holds the counter of the number of Set Calls
        /// </summary>
        private int _counterSet = 0;
    }

    /// <summary>
    /// Holds the reference for the AutoResetEvent
    /// </summary>
    public class SynchronizedObject
    {
        /// <summary>
        /// Holds the reference for the AutoResetEvent
        /// </summary>
        public SynchronizedObject() { }

    }
} 
