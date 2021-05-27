// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// <summary>

//  This class represents a single method call, and handles the execution of 

//  the call. </summary>



using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;

namespace DRT
{
    /// <summary>
    /// Call object is responsible for execution and lifetime of a call.
    /// It uses TestStepAttribute to determine delay, timeout and async behavior.
    /// </summary>
    public sealed class Call
    {
        #region Constructors
        //----------------------------------------------------------------------
        // Constructors
        //----------------------------------------------------------------------

        /// <summary>
        /// Creates a Call that will use the MethodInvoker to invoke the Method 
        /// with the specified arguments on the object.
        /// </summary>
        /// <param name="invoker">The MethodInvoker to use for the call.</param>
        /// <param name="method">The Method to call.</param>
        /// <param name="target">The Target of the call.</param>
        /// <param name="args">The Parameters use for the call.</param>
        /// <param name="dependencies">The calls this call will wait on prior to
        /// invoking.</param>
        public Call(
            MethodInvoker invoker,
            MethodInfo method,
            object target,
            object[] args,
            Call[] dependencies)
        {
            _invoker = invoker;
            Method = method;
            Target = target;
            Parameters = args;
            _dependencies.AddRange(dependencies);

            TestStepAttribute attrib = TestServices.GetFirstAttribute(
                typeof(TestStepAttribute), Method) as TestStepAttribute;

            if (attrib != null)
            {
                _timeout = attrib.Timeout;
                _delay = attrib.Delay;
                _async = attrib.Async;
                _order = attrib.Order;
            }
            else
            {
                TestServices.Assert(
                    false,
                    "Method does not have the TestStep attribute.");
            }

            if (_forceNoTimeout & (_timeout != Timeout.Infinite))
            {
                TestServices.Trace(
                    "Forcing Timeout.Infinite on {0} was {1}.",
                    method.Name,
                    _timeout);
                _timeout = Timeout.Infinite;
            }
        }
        #endregion Constructors

        #region Public Methods
        //----------------------------------------------------------------------
        // Public Methods
        //----------------------------------------------------------------------

        /// <summary>
        /// Will start the call and if not marked Async it wait for the call 
        /// to complete.
        /// </summary>
        /// <remarks>
        /// Will TestServices.Assert on timeout.
        /// </remarks>
        public void DoCall()
        {
            _started = DateTime.Now;

            if (_delay > 0)
            {
                TestServices.Trace("Delaying for {0}ms.", _delay);

                System.Threading.Thread.Sleep((int)_delay);
            }

            StartCall();

            if (!_async)
            {
                if (_timeout == Timeout.Infinite)
                {
                    TestServices.Warning(
                        "You have specified and infinite timeout for this "+
                        "call; application may hang.");
                }

                WaitForCall();
            }
        }

        /// <summary>
        /// Will wait for the call to complete.
        /// </summary>
        /// <remarks>
        /// Will TestServices.Assert on timeout.
        /// </remarks>
        public void WaitForCall()
        {
            if (!_callComplete.WaitOne(_timeout, false))
            {
                TestServices.Assert(
                    false, "Timeout after waiting {0}ms for step.", _timeout);
                CancelCall();
            }
        }

        /// <summary>
        /// Will return summary information which is available before a call
        /// executes.
        /// </summary>
        /// <remarks>
        /// Implemented because the ToString method includes current state.
        /// </remarks>
        /// <returns>Summary information.</returns>
        public string PreCallSummary()
        {
            StringBuilder waitList = new StringBuilder();

            bool doneFirst = false;
            foreach (Call c in Dependencies)
            {
                if (doneFirst)
                {
                    waitList.Append(", ");
                }
                else
                {
                    doneFirst = true;
                }

                waitList.Append(c.Method.Name);
            }

            string summary = string.Format(
                CultureInfo.CurrentUICulture,
                "{0}\t{1} {2} Delay={3} WaitFor={4} Timeout={5}",
                Order != uint.MaxValue ? Order.ToString() : "MaxValue",
                Method.Name,
                _async ? "Async" : "Sync",
                Delay,
                waitList.ToString(),
                WaitTimeout);

            return summary;
        }

        /// <summary>
        /// Will return summary information which is available before a call
        /// executes.
        /// </summary>
        /// <remarks>
        /// Implemented to diagnose slow tests.
        /// </remarks>
        /// <returns>Summary information.</returns>
        public string PostCallSummary()
        {
            TimeSpan total = _ended - _started;
            TimeSpan waited = TimeSpan.MinValue;
            TimeSpan exec = TimeSpan.MinValue;

            waited = _realStarted - _started;
            exec = _realEnded - _realStarted;

            return string.Format(
                CultureInfo.CurrentUICulture,
                "{0} Total={1}ms Wait={2}ms Execution={3}ms",
                Method.Name,
                (int)total.TotalMilliseconds,
                (int)waited.TotalMilliseconds,
                (int)exec.TotalMilliseconds);
        }

        #endregion Public Methods

        #region Public Methods - Overrides Object
        //----------------------------------------------------------------------
        // Public Methods - Overrides Object
        //----------------------------------------------------------------------

        /// <summary>
        /// Returns a text representation of the current call including names,
        /// parameters and parameter values.
        /// </summary>
        /// <returns>The string representation of the current call.</returns>
        public override string ToString()
        {
            const int MaxParameterValueSize = 128;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(
                CultureInfo.CurrentUICulture, 
                "{0}.{1}(", 
                Method.DeclaringType.Name, 
                Method.Name);

            ParameterInfo[] pis = Method.GetParameters();
            for (int i = 0; i < pis.Length; i++)
            {
                ParameterInfo p = pis[i];
                object dataValue = this.Parameters[i];
                string data = dataValue == null ? "(null)" : dataValue.ToString();
                if (data.Length > MaxParameterValueSize)
                {
                    data = data.Substring(0, MaxParameterValueSize - 3) + "...";
                }
                sb.AppendFormat(
                    CultureInfo.CurrentUICulture,
                    "{0}{1} {2}={3}{4}",
                    p.IsOut ? "out " : string.Empty,
                    p.ParameterType.Name,
                    p.Name,
                    data,
                    ((i + 1) < pis.Length) ? ", " : string.Empty);
            }

            sb.Append(")");

            return sb.ToString();
        }
        #endregion Public Methods - Overrides Object

        #region Public Properties
        //----------------------------------------------------------------------
        // Public Properties
        //----------------------------------------------------------------------

        /// <summary>
        /// True means the call will be non-blocking.
        /// </summary>
        public bool Async
        {
            get { return _async; }
        }

        /// <summary>
        /// Time in milliseconds to delay before starting call.
        /// </summary>
        public uint Delay
        {
            get { return _delay; }
        }

        /// <summary>
        /// List of calls to wait for before begining.
        /// </summary>
        public List<Call> Dependencies
        {
            get { return _dependencies; }
        }

        /// <summary>
        /// True will force all Calls to have an infinate timeout.
        /// </summary>
        public static bool ForceNoTimeout
        {
            set { _forceNoTimeout = value; }
        }

        /// <summary>
        /// The method to call.
        /// </summary>
        public MethodInfo Method
        {
            get { return _method; }
            set { _method = value; }
        }

        /// <summary>
        /// The invoker that will be used to perform the call.
        /// </summary>
        public MethodInvoker MethodInvoker
        {
            get { return _invoker; }
        }

        /// <summary>
        /// The order the call will be made in.
        /// </summary>
        public uint Order
        {
            get { return _order; }
        }

        /// <summary>
        /// The parameters passed in and out of the call.
        /// </summary>
        public object[] Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        /// <summary>
        /// The target instance of the call.
        /// </summary>
        public object Target
        {
            get { return _target; }
            set { _target = value; }
        }

        /// <summary>
        /// The number of milliseconds to wait before timeout.
        /// </summary>
        public int WaitTimeout
        {
            get { return _timeout; }
        }

        #endregion Public Properties

        #region Private Methods
        //----------------------------------------------------------------------
        // Private Methods
        //----------------------------------------------------------------------

        /// <summary>
        /// Will asyncronously start the execution of the call.
        /// </summary>
        private void StartCall()
        {
            _callComplete = new ManualResetEvent(false);
            _thread = new Thread(this.RealInvoke);
            _thread.Name = string.Format(
                CultureInfo.InvariantCulture,
                "{0}.{1}",
                _method.DeclaringType.Name,
                _method.Name);

            TestStepAttribute attrib = (TestStepAttribute)
                TestServices.GetFirstAttribute(
                typeof(TestStepAttribute), Method);

            _thread.SetApartmentState(attrib.ApartmentState);
            _thread.Start();
        }

        /// <summary>
        /// Performs the Invoke of the call once dependencies are complete.
        /// </summary>
        private void RealInvoke()
        {
            try
            {
                while (_dependencies.Count > 0)
                {
                    _dependencies[0].WaitForCall();
                    _dependencies.RemoveAt(0);
                }
                if (!_canceled)
                {
                    _realStarted = DateTime.Now;
                    _invoker.Invoke(this);
                }
            }
            finally
            {
                _realEnded = DateTime.Now;
                if (TestServices.Diagnose)
                {
                    TestServices.Log(this.ToString());
                }
                EndCall();
            }
        }

        /// <summary>
        /// Ended the call immediately; before execution is complete.
        /// </summary>
        private void EndCall()
        {
            _thread.Interrupt();
            if (_callComplete != null)
            {
                _callComplete.Set();
            }
            else
            {
                TestServices.Assert(
                    false, "We are ending a call that was never started.");
            }
            _ended = DateTime.Now;
        }

        /// <summary>
        /// Cancel the current call.
        /// </summary>
        private void CancelCall()
        {
            _canceled = true;
            EndCall();
        }
        #endregion Private Methods

        #region Private Fields
        //----------------------------------------------------------------------
        // Private Fields
        //----------------------------------------------------------------------

        private static bool _forceNoTimeout;

        private bool _async;
        private ManualResetEvent _callComplete;
        private bool _canceled;
        private uint _delay;
        private List<Call> _dependencies = new List<Call>();
        private MethodInvoker _invoker;
        private MethodInfo _method;
        private uint _order;
        private object[] _parameters;
        private object _target;
        private int _timeout;
        private Thread _thread;

        private DateTime _started;
        private DateTime _realStarted;
        private DateTime _realEnded;
        private DateTime _ended;

        #endregion Private Fields
    }
}
