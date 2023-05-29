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
    /// Collector of touch events
    /// </summary>
    public class TouchEventCollector : EventCollector
    {
        #region Fields

        private readonly UIElement _root;
        private Dictionary<int, List<EventParameters>> _collectedEventsPerTouch = new Dictionary<int, List<EventParameters>>();

        [Flags]
        public enum TouchEventTypes
        {
            None = 0,
            UpDownMove = 1,
            EnterLeave = 2,
            GotLostCapture = 4,
            Gestures = 8,
            All = UpDownMove | EnterLeave | GotLostCapture | Gestures,
        }

        private static RoutedEvent[] CreateTouchEvents(TouchEventTypes touchEventTypes)
        {
            List<RoutedEvent> eventList = new List<RoutedEvent>();

            if ((touchEventTypes & TouchEventTypes.UpDownMove) != 0)
            {
                eventList.Add(UIElement.PreviewTouchDownEvent);
                eventList.Add(UIElement.PreviewTouchMoveEvent);
                eventList.Add(UIElement.PreviewTouchUpEvent);
                eventList.Add(UIElement.TouchDownEvent);
                eventList.Add(UIElement.TouchMoveEvent);
                eventList.Add(UIElement.TouchUpEvent);
            }

            if ((touchEventTypes & TouchEventTypes.EnterLeave) != 0)
            {
                eventList.Add(UIElement.TouchEnterEvent);
                eventList.Add(UIElement.TouchLeaveEvent);
            }

            if ((touchEventTypes & TouchEventTypes.Gestures) != 0)
            {
                eventList.Add(UIElement.PreviewStylusSystemGestureEvent);
                eventList.Add(UIElement.StylusSystemGestureEvent);
            }

            if ((touchEventTypes & TouchEventTypes.GotLostCapture) != 0)
            {
                eventList.Add(UIElement.GotTouchCaptureEvent);
                eventList.Add(UIElement.LostTouchCaptureEvent);
            }

            return eventList.ToArray();
        }

        #endregion

        #region Constructor

        public TouchEventCollector(UIElement root, bool verifyEnterLeave, bool verifyGestures)
            : base(TouchEventCollector.CreateTouchEvents(
                   TouchEventCollector.TouchEventTypes.UpDownMove |
                   (verifyEnterLeave ? TouchEventCollector.TouchEventTypes.EnterLeave : 0) |
                   (verifyGestures ? TouchEventCollector.TouchEventTypes.Gestures : 0)))
        {
            Debug.Assert(root != null);
            this._root = root;
        }

        public TouchEventCollector(UIElement root, TouchEventTypes touchEventTypes)
            : base(TouchEventCollector.CreateTouchEvents(touchEventTypes))
        {
            Debug.Assert(root != null);
            this._root = root;
        }

        #endregion

        #region Methods

        protected override void Collect(IList<EventParameters> list, object sender, RoutedEventArgs args)
        {
            // add event parameters to the global list
            EventParameters parameters = new EventParameters(sender, args, GetPosition(args), args.RoutedEvent);
            list.Add(parameters);

            // add event parameter to the per-touch list
            List<EventParameters> collectedEventsForThisTouch;
            int touchId;
            if (args is StylusSystemGestureEventArgs)
            {
                touchId = ((StylusSystemGestureEventArgs)args).StylusDevice.TabletDevice.Id; 
            }
            else
            {
                touchId = ((TouchEventArgs)args).TouchDevice.Id;
            }

            if (!this._collectedEventsPerTouch.TryGetValue(touchId, out collectedEventsForThisTouch))
            {
                collectedEventsForThisTouch = new List<EventParameters>();
                this._collectedEventsPerTouch.Add(touchId, collectedEventsForThisTouch);
            }
            
            collectedEventsForThisTouch.Add(parameters);
        }

        private Point GetPosition(RoutedEventArgs args)
        {
            if (args is TouchEventArgs)
            {
                TouchEventArgs touchEventArgs = args as TouchEventArgs;
                if (touchEventArgs != null)
                {
                    return touchEventArgs.TouchDevice.GetTouchPoint(this._root).Position;
                }
            }

            return new Point();
        }

        public override void Clear()
        {
            base.Clear();
            this._collectedEventsPerTouch.Clear();
        }

        public ReadOnlyCollection<EventParameters> CollectedEventsForTouch(int touchId)
        {
            List<EventParameters> collectedEventsForThisTouch;
            
            if (this._collectedEventsPerTouch.TryGetValue(touchId, out collectedEventsForThisTouch))
            {
                return new ReadOnlyCollection<EventParameters>(collectedEventsForThisTouch);
            }
            
            return new ReadOnlyCollection<EventParameters>(new EventParameters[0]);
        }

        #endregion
    }
}
