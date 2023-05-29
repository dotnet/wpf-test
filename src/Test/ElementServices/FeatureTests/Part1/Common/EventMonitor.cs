// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// Utility class to track the firing of events
    /// </summary>
    public class EventMonitor
    {
        #region Fields

        private readonly Stack<EventArgs> _events = new Stack<EventArgs>();

        #endregion

        #region Methods

        /// <summary>
        /// Register that an event has fired
        /// </summary>
        /// <param name="args"></param>
        public void Fire(EventArgs args)
        {
            this._events.Push(args);
        }

        /// <summary>
        /// Execute an action, expecting that no event will fire
        /// </summary>
        /// <param name="action"></param>
        public void ExpectNone(Action action)
        {
            EventArgs dummy = new EventArgs();
            this._events.Push(dummy);
            action();
            EventArgs last = this._events.Pop();
            Utils.AssertEqual(dummy, last, "Event fired when wasn't expecting one");
        }

        /// <summary>
        /// Call a function, expecting that no event will fire
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="function"></param>
        /// <returns></returns>
        public T ExpectNone<T>(Func<T> function)
        {
            T result = default(T);
            ExpectNone(() => { result = function(); });
            return result;
        }

        /// <summary>
        /// Execute an action, expecting that one event of an expected type will fire
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <param name="action"></param>
        public TEventArgs ExpectOne<TEventArgs>(Action action) where TEventArgs : EventArgs
        {
            EventArgs dummy = new EventArgs();
            this._events.Push(dummy);
            action();
            EventArgs last = this._events.Pop();
            Utils.AssertEqual(dummy, last, "Event failed to fire when expected");
            
            TEventArgs args = last as TEventArgs;
            last = this._events.Pop();
            Utils.AssertEqual(dummy, last, "Too many events fired");
            Utils.Assert(args != null, "Unexpected event type fired");
            
            return args;
        }

        /// <summary>
        /// Execute an action, expecting that at most one event will fire
        /// </summary>
        /// <param name="action"></param>
        public EventArgs ExpectAtMostOne(Action action)
        {
            EventArgs dummy = new EventArgs();
            this._events.Push(dummy);
            action();
            EventArgs last = this._events.Pop();
            if (object.ReferenceEquals(last, dummy))
            {
                // no event fired
                return null;
            }
            EventArgs result = last;
            last = this._events.Pop();
            Utils.AssertEqual(dummy, last, "Too many events fired");
            return result;
        }

        #endregion
    }
}
