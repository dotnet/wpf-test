// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System;

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// Custom TouchDevice for TouchDevice related captures
    /// </summary>
    public class CustomTouchDevice : TouchDevice
    {
        #region Constructor

        public CustomTouchDevice(int id)
            : base(id)
        {
        }

        #endregion

        #region Public Methods, Properties, and Event Handlers

        /// <summary>
        ///     Provides the current position.
        /// </summary>
        /// <param name="relativeTo">Defines the coordinate space.</param>
        /// <returns>The current position in the coordinate space of relativeTo.</returns>
        public override TouchPoint GetTouchPoint(IInputElement relativeTo)
        {
            if (relativeTo == null)
            {
                throw new ArgumentNullException("relativeTo");
            }

            if (CurrentTouchPoint != null)
            {
                if (relativeTo != null)
                {
                    Visual rootVisual = ActiveSource.RootVisual;
                    UIElement element = relativeTo as UIElement;
                    if (element != null)
                    {
                        GeneralTransform transform = rootVisual.TransformToDescendant(element);

                        Point position = transform.Transform(CurrentTouchPoint.Position);
                        Rect bounds = transform.TransformBounds(CurrentTouchPoint.Bounds);

                        return new TouchPoint(this, position, bounds, _lastAction);
                    }
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

        public Point GetPosition(IInputElement relativeTo)
        {
            if (this._positionRelativeTo == null || relativeTo == null)
            {
                return new Point(0, 0);
            }

            return this._positionRelativeTo.TranslatePoint(this._position, (UIElement)relativeTo);
        }

        public new event EventHandler Updated;

        public void SetPosition(UIElement positionRelativeTo, Point position)
        {
            this._position = position;
            this._positionRelativeTo = positionRelativeTo;

            if (Updated != null)
            {
                Updated(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Private Fields

        private TouchAction _lastAction = TouchAction.Move;
        private UIElement _positionRelativeTo;
        private Point _position;

        #endregion
    }
}
