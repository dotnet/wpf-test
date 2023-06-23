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
        private int fired = 0;

        /// <summary>
        ///     The number of times the event has been fired.
        /// </summary
        public virtual int TimesFired {
            get { return fired; }
            set { fired = value; }
        }

        /// <summary>
        ///     Reset the TimesFired counter to 0.
        /// </summary
        public virtual void Reset() {
            fired = 0;
        }
    }

    /// <summary>
    ///     An implementation of EventCounterBase to handle counting of EventArgs
    ///     events.
    /// </summary
    public class EventCounter : EventCounterBase {
        private EventHandler handler;
        private EventArgs eventArgs;

        public EventCounter() {
            handler = new EventHandler(Handler);
        }

        /// <summary>
        ///     The EventArgs passed to the event handler.
        /// </summary>
        public EventArgs EventArgs {
            get { return eventArgs; }
        }

        /// <summary>
        ///     The event handler to hook up to your tested event.  For example, if
        ///     you are testing Button1.Click, you coud say "Button1.Click += counter.EventHandler;"
        /// </summary>
        public EventHandler EventHandler {
            get { return handler; }
        }

        /// <summary>
        ///     Resets the counter to 0 and EventArgs to null.
        /// </summary>
        public override void Reset() {
            base.Reset();
            eventArgs = null;
        }

        /// <summary>
        ///     The actual event handler.
        /// </summary>
        private void Handler(object s, EventArgs e) {
            ++this.TimesFired;
            eventArgs = e;
        }
    }
}
