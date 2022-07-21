// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

namespace DRT
{
    public class TestTouchDevice : TouchDevice
    {
        public TestTouchDevice(int id)
            : base(id)
        {
        }

        /// <summary>
        ///     Provides the current position.
        /// </summary>
        /// <param name="relativeTo">Defines the coordinate space.</param>
        /// <returns>The current position in the coordinate space of relativeTo.</returns>
        public override TouchPoint GetTouchPoint(IInputElement relativeTo)
        {
            if (CurrentTouchPoint != null)
            {
                if (relativeTo != null)
                {
                    Visual rootVisual = ActiveSource.RootVisual;
                    UIElement element = (UIElement)relativeTo;
                    GeneralTransform transform = rootVisual.TransformToDescendant(element);

                    Point position = transform.Transform(CurrentTouchPoint.Position);
                    Rect bounds = transform.TransformBounds(CurrentTouchPoint.Bounds);

                    return new TouchPoint(this, position, bounds, _lastAction);
                }
                else
                {
                    return CurrentTouchPoint;
                }
            }

            return null;
        }

        /// <summary>
        ///     Provides all of the known points the device hit since the last reported position update.
        /// </summary>
        /// <param name="relativeTo">Defines the coordinate space.</param>
        /// <returns>A list of points in the coordinate space of relativeTo.</returns>
        public override TouchPointCollection GetIntermediateTouchPoints(IInputElement relativeTo)
        {
            TouchPointCollection points = new TouchPointCollection();

            TouchPoint point = GetTouchPoint(relativeTo);
            if (point != null)
            {
                points.Add(point);
            }

            return points;
        }

        public void UpdateActiveSource(PresentationSource activeSource)
        {
            SetActiveSource(activeSource);
        }

        public TouchPoint CurrentTouchPoint
        {
            get;
            set;
        }

        public void OnActivate()
        {
            Activate();
        }

        public void OnDeactivate()
        {
            Deactivate();
        }

        public void OnDown()
        {
            _lastAction = TouchAction.Down;
            ReportDown();
        }

        public void OnMove()
        {
            _lastAction = TouchAction.Move;
            ReportMove();
        }

        public void OnUp()
        {
            _lastAction = TouchAction.Up;
            ReportUp();
        }

        private TouchAction _lastAction = TouchAction.Move;
    }
}
