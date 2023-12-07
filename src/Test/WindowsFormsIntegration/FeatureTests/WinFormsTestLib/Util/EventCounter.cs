// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace WFCTestLib.Util {
    using System;
    using System.Windows.Forms;

    /// <summary>
    ///     Provides a generic way to count the number of times an event is fired.
    ///     Generally, subclasses will provide an event handler which can be hooked
    ///     up to an event, and provide a means of accessing the EventArgs.
    ///
    ///     Prior to running an event counting test, the Reset() method is called.
    ///     Whatever actions are necessary to carry out for the test are peformed,
    ///     then TimesFired returns the number of times the event was fired.
    /// </summary
    public abstract class EventCounterBase {
        private int _fired = 0;

        /// <summary>
        ///     The number of times the event has been fired.
        /// </summary
        public virtual int TimesFired {
            get { return _fired; }
            set { _fired = value; }
        }

        /// <summary>
        ///     Reset the TimesFired counter to 0.
        /// </summary
        public virtual void Reset() {
            _fired = 0;
        }
    }

    /// <summary>
    ///     An implementation of EventCounterBase to handle counting of EventArgs
    ///     events.
    /// </summary
    public class EventCounter : EventCounterBase {
        private EventHandler _handler;
        private EventArgs _eventArgs;

        public EventCounter() {
            _handler = new EventHandler(Handler);
        }

        /// <summary>
        ///     The EventArgs passed to the event handler.
        /// </summary>
        public EventArgs EventArgs {
            get { return _eventArgs; }
        }

        /// <summary>
        ///     The event handler to hook up to your tested event.  For example, if
        ///     you are testing Button1.Click, you coud say "Button1.Click += counter.EventHandler;"
        /// </summary>
        public EventHandler EventHandler {
            get { return _handler; }
        }

        /// <summary>
        ///     Resets the counter to 0 and EventArgs to null.
        /// </summary>
        public override void Reset() {
            base.Reset();
            _eventArgs = null;
        }

        /// <summary>
        ///     The actual event handler.
        /// </summary>
        private void Handler(object s, EventArgs e) {
            ++this.TimesFired;
            _eventArgs = e;
        }
    }
}
