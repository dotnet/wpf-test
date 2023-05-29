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
    /// Baseline event builder. Uses the Mouse to perform hit testing.
    /// </summary>
    public class BaselineEventBuilder
    {
        #region Fields

        private enum CollectionState
        {
            Skip, MatchToTouchDown, MatchToTouchMove, MatchToTouchUp, MatchToTouchHasGone
        };

        // mouse event to collect
        private static readonly RoutedEvent s_simulationEvent = Mouse.PreviewMouseMoveEvent;

        // mouse events to collect
        private static readonly RoutedEvent[] s_mouseEvents = new RoutedEvent[]
        {
            s_simulationEvent,
        };

        // mouse events to collect
        private static readonly RoutedEvent[] s_mouseEventsWithEnterLeave = new RoutedEvent[]
        {
            s_simulationEvent,
            Mouse.MouseEnterEvent,
            Mouse.MouseLeaveEvent,
        };

        #endregion


        /// <summary>
        /// Event collector that collects simulated mouse events
        /// </summary>
        private class SimulationEventCollector : EventCollector
        {
            // collection state, the caller is responsible to set this property that is used to generate matched touch event
            private CollectionState _state;

            // a dictionary: visual element -> bool to filter out duplicated events
            private Dictionary<object, bool> _handledEvents = new Dictionary<object, bool>();

            // insert position to gnerate a new event
            private int _insertPosition;

            // root of the visual tree
            private readonly UIElement _root;

            public SimulationEventCollector(UIElement root, bool collectEnterLeave)
                : base(collectEnterLeave ? s_mouseEventsWithEnterLeave : s_mouseEvents)
            {
                Debug.Assert(root != null);
                this._root = root;
                Init();
            }

            protected override void Collect(IList<EventParameters> list, object sender, RoutedEventArgs args)
            {
                RoutedEvent[] matchedEvents = GetExpectedTouchEvents(sender, args);
                if (matchedEvents != null)
                {
                    Point position = GetPosition(args);
                    for (int i = 0; i < matchedEvents.Length; i++)
                    {
                        RoutedEvent matchedEvent = matchedEvents[i];
                        EventParameters parameters = new EventParameters(sender, args, position, matchedEvent);
                        list.Insert(this._insertPosition, parameters);
                        if (i == 0)
                        {
                            // increase insertPosition only once, we need to make sure that all tunnel (Preview) events
                            // go first follow by all bubble events.
                            this._insertPosition++;
                        }
                    }
                }
            }

            /// <summary>
            /// Returns a collection of expected events based on the matching specified by the 'this.state'
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            private RoutedEvent[] GetExpectedTouchEvents(object sender, RoutedEventArgs args)
            {
                RoutedEvent[] expectedEvents = null;

                if (args.RoutedEvent == s_simulationEvent)
                {
                    if (!this._handledEvents.ContainsKey(sender))
                    {
                        this._handledEvents.Add(sender, true);
                
                        switch (this._state)  // 
                        {
                            // handle the event as TouchDown
                            case CollectionState.MatchToTouchDown:
                                expectedEvents = new RoutedEvent[]
                                {
                                    UIElement.PreviewTouchDownEvent,
                                    UIElement.TouchDownEvent
                                };
                                break;

                            // handle the event as TouchMove
                            case CollectionState.MatchToTouchMove:
                                expectedEvents = new RoutedEvent[]
                                {
                                    UIElement.PreviewTouchMoveEvent,
                                    UIElement.TouchMoveEvent
                                };
                                break;

                            // handle the event as Touchup
                            case CollectionState.MatchToTouchUp:
                                expectedEvents = new RoutedEvent[]
                                {
                                    UIElement.PreviewTouchUpEvent,
                                    UIElement.TouchUpEvent
                                };
                                break;

                            // skip the event
                            case CollectionState.Skip:
                                break;

                            // unexpected state
                            default:
                                Debug.Assert(false);
                                break;
                        }
                    }
                }
                else if (args.RoutedEvent == Mouse.MouseEnterEvent &&
                        (this._state == CollectionState.MatchToTouchDown || this._state == CollectionState.MatchToTouchMove))
                {
                    expectedEvents = new RoutedEvent[] { UIElement.TouchEnterEvent };
                }
                else if (args.RoutedEvent == Mouse.MouseLeaveEvent &&
                        (this._state == CollectionState.MatchToTouchMove || this._state == CollectionState.MatchToTouchHasGone))
                {
                    expectedEvents = new RoutedEvent[] { UIElement.TouchLeaveEvent };
                }

                return expectedEvents;
            }

            /// <summary>
            /// Reads position from the mouse event
            /// </summary>
            /// <param name="args"></param>
            /// <returns></returns>
            private Point GetPosition(RoutedEventArgs args)
            {
                MouseEventArgs mouseEventArgs = args as MouseEventArgs;
                if (mouseEventArgs != null)
                {
                    if (this._state == CollectionState.MatchToTouchHasGone)
                    {
                        // take position from the previous event
                        return LastEvent != null ? LastEvent.Position : new Point(double.NaN, double.NaN);
                    }
                    else
                    {
                        // get the current position
                        return mouseEventArgs.GetPosition(this._root);
                    }
                }
                return new Point();
            }

            public override void Clear()
            {
                base.Clear();
                Init();
            }

            private void Init()
            {
                this._state = CollectionState.Skip;
                this._handledEvents.Clear();
                this._insertPosition = 0;
            }

            public CollectionState State
            {
                get
                {
                    return this._state;
                }
                set
                {
                    this._state = value;
                    this._handledEvents.Clear();
                    this._insertPosition = CollectedEventsCount;
                }
            }
        }


        #region Methods

        private static void MoveMouseOutsideWindow(Window window)
        {
            // move the mouse outside the window
            UserInput.MouseMove((int)1000000, (int)1000000);
        }

        /// <summary>
        /// Generates baseline, returns true if operation succeeds, otherwise - false with the 'error' parameter containing the error string.
        /// </summary>
        /// <param name="window">Window the simulated events will be sent to.</param>
        /// <param name="root">Root element to retrieve the event position relative to.</param>
        /// <param name="objectToAddHandlers">The object to attach events handlers.</param>
        /// <param name="addHandlersToChildren">Add event handlers to the children.</param>
        /// <param name="collectEnterLeave">Collect Enter and Leave events.</param>
        /// <param name="touchSnapshots">List of touch snapshots to build the baseline against.</param>
        /// <param name="previewTouchProcessing">Optional delegate that will be called before processing a touch snapshot.</param>
        /// <param name="collectedEvents">The result collection of collected events.</param>
        /// <param name="error">The result error string if the method fails.</param>
        /// <returns></returns>
        public static bool Generate(Window window, UIElement root, 
            DependencyObject objectToAddHandlers, bool addHandlersToChildren, bool collectEnterLeave,
            IList<TouchDevice> touchSnapshots, Action<TouchDevice> previewTouchProcessing,
            out ReadOnlyCollection<EventParameters> collectedEvents, out string error)
        {
            error = null;
            collectedEvents = null;
            SimulationEventCollector collector = null;

            try
            {
                // 

                // move mouse outside the window
                MoveMouseOutsideWindow(window);

                // create event collector and add handlers
                collector = new SimulationEventCollector(root, collectEnterLeave);
                collector.AddHandlers(objectToAddHandlers, addHandlersToChildren);

                // simulate mouse events
                Debug.Assert(touchSnapshots.Count >= 2);
                Point previousPosition = new Point(double.NaN, double.NaN);

                for (int i = 0; i < touchSnapshots.Count; i++)
                {
                    // current touch snapshot
                    TouchDevice snapshot = touchSnapshots[i];

                    // get snapshot position and simulate a mouse event
                    Point position = snapshot.GetTouchPoint(root).Position;

                    // move mouse somewhere else if position is the same as the previous one
                    if (position == previousPosition)
                    {
                        // if there are more than one consequent snapshot with the same position then we cannot do anything.
                        // We can try to move the mouse somewhere else but then we can get spurious MouseEnter event.
                        // So fail beseline generation unless this is the last snapshot that
                        // corresponds to the touchUp.
                        if (i != touchSnapshots.Count - 1)
                        {
                            error = "Interrupted. Could not generate baseline, several snapshots have the same position.";
                            return false;
                        }
                        collector.State = CollectionState.Skip;
                        
                        Point anyPosition = new Point(position.X > 0 ? position.X - 1 : position.X + 1, position.Y);

                        // simulate mouse move
                        UserInput.MouseMove(window, (int)anyPosition.X, (int)anyPosition.Y);
                    }
                    previousPosition = position;

                    // invoke the optional previewTouchProcessing delegate
                    if (previewTouchProcessing != null)
                    {
                        previewTouchProcessing(snapshot);
                    }

                    if (i == 0)  // first touch snapshot, generate TouchDown event
                    {
                        collector.State = CollectionState.MatchToTouchDown;
                    }

                    else if (i == touchSnapshots.Count - 1)  // last touch snapshot, generate TouchUp event
                    {
                        collector.State = CollectionState.MatchToTouchUp;
                    }
                    else  // a touch snapshot somewhere in the middle, generate TouchMove event
                    {
                        collector.State = CollectionState.MatchToTouchMove;
                    }

                    int collectedEventCountBefore = collector.CollectedEventsCount;

                    UserInput.MouseMove(window, (int)position.X, (int)position.Y);

                    // make sure that the event is processed
                    EventCollector.DoEvents();

                    int collectedEventCountAfter = collector.CollectedEventsCount;

                    if (collectedEventCountBefore == collectedEventCountAfter)
                    {
                        // mouse event is missing, can't do anything
                        error = "Interrupted. Could not generate baseline, mouse event is missing.";
                        return false;
                    }
                }

                // move the mouse outside the window
                collector.State = CollectionState.MatchToTouchHasGone;
                MoveMouseOutsideWindow(window);
            }
            finally
            {
                if (collector != null)
                {
                    // remove event handlers
                    collector.RemoveHandlers(objectToAddHandlers, addHandlersToChildren);
                }

                // 
            }

            collectedEvents = collector.CollectedEvents;
            return true;
        }

        #endregion
    }
}
