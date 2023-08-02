// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Microsoft.Test.Input;

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// Event parameters
    /// </summary>
    public class EventParameters
    {
        #region Fields

        private readonly object _originalSender;
        private readonly RoutedEventArgs _originalRoutedEventArgs;
        private readonly Point _position;
        private readonly RoutedEvent _matchedRoutedEvent;
        private readonly int _receivedTimestamp;

        #endregion

        #region Contructor

        public EventParameters(object sender, RoutedEventArgs args, Point snapshotPosition, RoutedEvent matchedRoutedEvent)
        {
            this._originalSender = sender;
            this._originalRoutedEventArgs = args;
            this._position = snapshotPosition;
            this._matchedRoutedEvent = matchedRoutedEvent;
            this._receivedTimestamp = Environment.TickCount;
        }

        #endregion

        #region Properties

        public object OriginalSender
        {
            get
            {
                return this._originalSender;
            }
        }

        public RoutedEventArgs OriginalRoutedEventArgs
        {
            get
            {
                return this._originalRoutedEventArgs;
            }
        }

        public int ReceviedTimestamp
        {
            get
            {
                return this._receivedTimestamp;
            }
        }

        public Point Position
        {
            get
            {
                return this._position;
            }
        }

        public RoutedEvent MatchedRoutedEvent
        {
            get
            {
                return this._matchedRoutedEvent;
            }
        }

        #endregion
    }

    /// <summary>
    /// A general purpose collector of events
    ///
    /// Usage:
    /// 1. Create an instance of this class and specify what events to collect.
    /// 2. Call the AttachHandlers() method to subscribe event listerens
    /// 3. Execute a test that is suppose to trigger events
    /// 4. Call the CollectedEvents property to get a readonly collection of the collected events
    /// 5. Call the RemoveHandlers() method to unsubscribe listeners before releasing the collector instance
    /// </summary>
    public class EventCollector
    {
        #region Fields

        private List<EventParameters> _collectedEventParameters = new List<EventParameters>();
        private RoutedEvent[] _eventsToCollect;
        private readonly bool _collectAllEvents;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct an instance that can listern to the given events
        /// </summary>
        /// <param name="root">UIElement to retrive</param>
        /// <param name="eventsToCollect"></param>
        public EventCollector(params RoutedEvent[] eventsToCollect)
        {
            this._eventsToCollect = eventsToCollect;
            this._collectAllEvents = false;
        }

        public EventCollector(bool collectAllEvents)
        {
            this._eventsToCollect = new RoutedEvent[0];
            this._collectAllEvents = collectAllEvents;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns a readonly collection of the collected events
        /// </summary>
        public ReadOnlyCollection<EventParameters> CollectedEvents
        {
            get
            {
                return new ReadOnlyCollection<EventParameters>(this._collectedEventParameters);
            }
        }

        /// <summary>
        /// Returns the number of collected events
        /// </summary>
        public int CollectedEventsCount
        {
            get
            {
                return this._collectedEventParameters.Count;
            }
        }

        /// <summary>
        /// The last collected event or null if no events have been collected
        /// </summary>
        public EventParameters LastEvent
        {
            get
            {
                return this._collectedEventParameters.Count > 0 ?
                    this._collectedEventParameters[this._collectedEventParameters.Count - 1] :
                    null;
            }
        }

        #endregion

        #region Various Methods

        /// <summary>
        /// Attaches event handlers
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="addToChildren"></param>
        public void AddHandlers(DependencyObject obj, bool addToChildren)
        {
            Debug.Assert(obj != null);
            foreach (RoutedEvent e in this._eventsToCollect)
            {
                MultiTouchVerifier.AddHandler(obj, e, new RoutedEventHandler(GenericEventHandler), false/*addToParents*/, addToChildren);
            }
        }

        /// <summary>
        /// Detaches event handlers
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="addToChildren"></param>
        public void RemoveHandlers(DependencyObject obj, bool addToChildren)
        {
            Debug.Assert(obj != null);
            foreach (RoutedEvent e in this._eventsToCollect)
            {
                MultiTouchVerifier.RemoveHandler(obj, e, new RoutedEventHandler(GenericEventHandler), false/*addToParents*/, addToChildren);
            }
        }

        /// <summary>
        /// Clears the list of collected events
        /// </summary>
        public virtual void Clear()
        {
            this._collectedEventParameters.Clear();
        }

        /// <summary>
        /// Base implementation of the Collect() method, derived class can override it.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void Collect(IList<EventParameters> list, object sender, RoutedEventArgs args)
        {
            list.Add(new EventParameters(sender, args, new Point(), args.RoutedEvent));
        }

        /// <summary>
        /// Collects the event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void Collect(object sender, RoutedEventArgs args)
        {
            if (this._collectAllEvents || Array.Exists(this._eventsToCollect, delegate(RoutedEvent r) { return r == args.RoutedEvent; }))
            {
                Collect(_collectedEventParameters, sender, args);
            }
        }

        /// <summary>
        /// Dumps the list of the collected events
        /// </summary>
        public void Dump()
        {
            Dump(this._collectedEventParameters);
        }

        /// <summary>
        /// Dumps the list of the collected events
        /// </summary>
        public static void Dump(IList<EventParameters> collectedEvents)
        {
            for (int i = 0; i < collectedEvents.Count; i++)
            {
                EventParameters p = collectedEvents[i];
                Debug.WriteLine("  " + i + ": " + p.MatchedRoutedEvent + " sender=" + p.OriginalSender + "[" + GetHashCode(p.OriginalSender) + "]" +
                    " (" + p.OriginalRoutedEventArgs.RoutedEvent + ")" +
                    " position={" + p.Position.X + " " + p.Position.Y + "}" +
                    " source=" + p.OriginalRoutedEventArgs.Source + "[" + GetHashCode(p.OriginalRoutedEventArgs.Source) + "]");
            }
        }

        /// <summary>
        /// Compares events. - 



        public static bool CompareEvents(IList<EventParameters> expectedEvents, IList<EventParameters> touchEvents)
        {
            Debug.WriteLine("");
            Debug.WriteLine("ACTUAL:");
            EventCollector.Dump(touchEvents);

            Debug.WriteLine("");
            Debug.WriteLine("EXPECTED:");
            EventCollector.Dump(expectedEvents);

            bool isMatch = true;
            int mi = 0;
            int ci = 0;

            Debug.WriteLine("");
            Debug.WriteLine("VERIFICATION:");
            for (; mi < expectedEvents.Count && ci < touchEvents.Count; mi++, ci++)
            {
                EventParameters baselineEvent = expectedEvents[mi];
                EventParameters touchEvent = touchEvents[ci];
                Debug.WriteLine(mi.ToString() + " EVENT, exp=" + baselineEvent.MatchedRoutedEvent + " act=" + touchEvent.MatchedRoutedEvent);
                if (baselineEvent.MatchedRoutedEvent != touchEvent.MatchedRoutedEvent)
                {
                    Debug.WriteLine("FAILURE: Unexpected event.");
                    isMatch = false;
                }

                // check position
                Point baselinePosition = baselineEvent.Position;
                Point touchPosition = touchEvent.Position;
                Debug.WriteLine("Position, exp={" + baselinePosition.X + " " + baselinePosition.Y + "} act={" +
                        touchPosition.X + " " + touchPosition.Y + "}");
                if (!IsTheSame(baselinePosition, touchPosition, 0.0001))
                {
                    Debug.WriteLine("FAILURE: Invalid Position.");
                    isMatch = false;
                }

                // check source
                RoutedEventArgs baselineEventArgs = baselineEvent.OriginalRoutedEventArgs;
                if (touchEvent.OriginalRoutedEventArgs is StylusSystemGestureEventArgs)
                {
                    StylusSystemGestureEventArgs gestureEventArgs = (StylusSystemGestureEventArgs)touchEvent.OriginalRoutedEventArgs;

                    Debug.WriteLine("Source, exp=" + baselineEventArgs.Source + "[" + GetHashCode(baselineEventArgs.Source) + "]" +
                        " act=" + gestureEventArgs.Source + "[" + GetHashCode(gestureEventArgs.Source) + "]");
                    if (baselineEventArgs.Source != gestureEventArgs.Source)
                    {
                        Debug.WriteLine("FAILURE: Invalid Source.");
                        isMatch = false;
                    }
                }
                else
                {
                    TouchEventArgs touchEventArgs = (TouchEventArgs)touchEvent.OriginalRoutedEventArgs;

                    Debug.WriteLine("Source, exp=" + baselineEventArgs.Source + "[" + GetHashCode(baselineEventArgs.Source) + "]" +
                        " act=" + touchEventArgs.Source + "[" + GetHashCode(touchEventArgs.Source) + "]");
                    if (baselineEventArgs.Source != touchEventArgs.Source)
                    {
                        Debug.WriteLine("FAILURE: Invalid Source.");
                        isMatch = false;
                    }
                }
                
                // check sender
                Debug.WriteLine("Sender, exp=" + baselineEvent.OriginalSender + "[" + GetHashCode(baselineEvent.OriginalSender) + "]" +
                    " act=" + touchEvent.OriginalSender + "[" + GetHashCode(touchEvent.OriginalSender) + "]");
                if (baselineEvent.OriginalSender != touchEvent.OriginalSender)
                {
                    Debug.WriteLine("FAILURE: Invalid Sender.");
                    isMatch = false;
                }
            }

            // missing events
            for (; mi < expectedEvents.Count; mi++)
            {
                EventParameters baselineEvent = expectedEvents[mi];
                Debug.WriteLine("FAILURE, missing event: " + baselineEvent.MatchedRoutedEvent);
                Debug.WriteLine("Position, {" + baselineEvent.Position.X + " " + baselineEvent.Position.Y + "}");
                Debug.WriteLine("Source, " + baselineEvent.OriginalRoutedEventArgs.Source + "[" + GetHashCode(baselineEvent.OriginalRoutedEventArgs.Source) + "]");
                Debug.WriteLine("Sender, " + baselineEvent.OriginalSender + "[" + GetHashCode(baselineEvent.OriginalSender) + "]");
                isMatch = false;
            }

            // unexpected events
            for (; ci < touchEvents.Count; ci++)
            {
                EventParameters touchEvent = touchEvents[ci];
                Debug.WriteLine("FAILURE, Unexpected event: " + touchEvent.MatchedRoutedEvent);
                Debug.WriteLine("Position, {" + touchEvent.Position.X + " " + touchEvent.Position.Y + "}");
                Debug.WriteLine("Source, " + touchEvent.OriginalRoutedEventArgs.Source + "[" + GetHashCode(touchEvent.OriginalRoutedEventArgs.Source) + "]");
                Debug.WriteLine("Sender, " + touchEvent.OriginalSender + "[" + GetHashCode(touchEvent.OriginalSender) + "]");
                isMatch = false;
            }

            return isMatch;
        }

        /// <summary>
        /// the good old helper
        /// </summary>
        public static void DoEvents()
        {
            // To keep this thread busy, we'll have to push a frame.
            System.Windows.Threading.DispatcherFrame frame = new System.Windows.Threading.DispatcherFrame();

            System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle,
                new System.Windows.Threading.DispatcherOperationCallback(
                    delegate(object arg)
                    {
                        frame.Continue = false;
                        return null;
                    }), null);

            // Keep the thread busy processing events until the timeout has expired.
            System.Windows.Threading.Dispatcher.PushFrame(frame);
        }

        /// <summary>
        /// General event handler for all events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void GenericEventHandler(object sender, RoutedEventArgs args)
        {
            Collect(sender, args);
        }

        private static string GetHashCode(object obj)
        {
            return obj == null ? "null" : obj.GetHashCode().ToString();
        }

        private static bool IsTheSame(Point pt1, Point pt2, double tolerance)
        {
            return Math.Abs(pt1.X - pt2.X) < tolerance && Math.Abs(pt1.Y - pt2.Y) < tolerance;
        }

        #endregion
    }

}
