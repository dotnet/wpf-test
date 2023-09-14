// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Collections;

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// All atom gestures for basic tracks like dot, line, etc., used to construct commonly used 
    /// gestures like drag, rotate, etc..  Gestures in SinglePointGestures.cs and 
    /// MultiplePointGestures.cs are prefered to use directly.
    /// </summary>

   #region "DotGesture" gesture

    /// <summary>
    /// Represent a single point touch that presses and releases
    /// for a given number of milliseconds, getting a dot track.
    /// </summary>
    public class DotGesture : SingleContactGesture
    {
        public DotGesture(Point point)
        {
            origin = point;            
            samples = 1;

            CalculatePoints();
        }
        
        public DotGesture(Point point, int milliseconds)
        {
            origin = point;
            samples = 1;
            duration = milliseconds;

            CalculatePoints();
        }

        public override void CalculatePoints()
        {
            interpolated = new Point[1];
            interpolated[0] = origin;
        }
    }

    #endregion

   #region "LineGesture" gesture

    /// <summary>
    /// Represents a single-point touch that moves through a set of pre-defined points 
    /// by the given PointF[], getting a line track
    /// </summary>
    public class LineGesture : SingleContactGesture
    {
        private Point[] _points;    

        public LineGesture(Point[] inPoints, bool smoothed)
        {
            if (smoothed)
            {
                throw new NotImplementedException("Smoothed curve fit not yet implemented!");
            }
            _points = inPoints;

            CalculatePoints();
        }

        /// <summary>
        /// the original points
        /// </summary>
        internal override Point[] Points 
        {
            get
            {
                return _points;  
            }
        }

        public LineGesture(Point startpoint, Point endpoint) 
        {
            _points = new Point[] { startpoint, endpoint };  
            CalculatePoints();
        }

        public override void CalculatePoints()
        {
            int res = Samples - 1;

            interpolated = new Point[(_points.Length - 1) * Samples]; 

            for (int i = 0; i < _points.Length - 1; i++) 
            {
                Point point = _points[i];
                Point delta = new Point(_points[i + 1].X - _points[i].X, _points[i + 1].Y - _points[i].Y);

                for (int j = 0; j < Samples; j++)
                {
                    interpolated[i * Samples + j].X = point.X + j * (delta.X / res);
                    interpolated[i * Samples + j].Y = point.Y + j * (delta.Y / res);
                }
            }
        } 
    }

    #endregion

   #region "Arc" gesture

    /// <summary>
    /// Represents a arch track that will be a part of a ellipse (circle is a special case for ellipse.)
    /// </summary>
    public class ArcGesture : SingleContactGesture
    {
        private Point _center;
        private double _height;
        private double _width;
        private double _startAngle;
        private double _endAngle;
        
        /// <summary>
        /// Arc gesture
        /// </summary>
        /// <param name="center">the center of the ellipse</param>
        /// <param name="height">length of semi axis on Y direction</param>
        /// <param name="width">length of semi axis on X direction</param>
        /// <param name="startAngle">the angle from which the arch starts</param>
        /// <param name="endAngle">the angle at which the arch ends. 
        /// if startAngle > endAngle, this is clockwise, otherwise, it is anti-clockwise</param>
        public ArcGesture(Point center, double height, double width, double startAngle, double endAngle)
        {
            if (height <= 0 || width <= 0)
            {
                throw new ArgumentOutOfRangeException("width and height must be greater than 0");
            }
            if (startAngle == endAngle)
            {
                throw new ArgumentOutOfRangeException("startAngle should not equal to endAngle");
            }
            this._center = center;
            this._height = height;
            this._width = width;
            this._startAngle = startAngle;
            this._endAngle = endAngle;

            CalculatePoints();
        }

        public override void CalculatePoints()
        {
            double newx;
            double newy;
            double r;
            double pathLen = _endAngle - _startAngle;
            double perChange = pathLen / (double)(samples - 1);
            double changedAngle = 0;
            double currentAngle = 0;

            interpolated = new Point[samples];

            for (int i = 0; i < samples; i++)
            {
                currentAngle = (_startAngle + changedAngle) * Math.PI / 180;
                r = Math.Sqrt(Math.Pow(_height * _width, 2.0)
                            / (Math.Pow(_height * Math.Cos(currentAngle), 2.0)
                             + Math.Pow(_width * Math.Sin(currentAngle), 2.0)));
                newx = _center.X + r * Math.Cos(currentAngle);
                newy = _center.Y - r * Math.Sin(currentAngle);
                changedAngle += perChange;
                interpolated[i].X = (float)newx;
                interpolated[i].Y = (float)newy;
            }
        }
    }

    #endregion

   #region "SineGesture" gesture

    /// <summary>
    /// Represents a single point touch that traces a path of a sine wave, starting at 
    /// the given location and with set amplitude and frequency
    /// </summary>
    public class SineGesture : SingleContactGesture
    {
        private double _height;
        private double _width;
        private int _count;
        private bool _oscillate;

        public SineGesture(double originX, double originY, double heightInches, double widthInches, int count, bool oscillate)
        {
            this.origin = new Point(originX, originY);
            this._height = heightInches;
            this._width = widthInches;
            this._count = count;
            this._oscillate = oscillate;
            base.RecalcOnSamplesChange = false;

            CalculatePoints();
        }

        public override void CalculatePoints()
        {
            double newx = origin.X;
            double newy = origin.Y;

            List<Point> interpolatedList = new List<Point>();

            for (newx = origin.X; (newx < origin.X + _width); newx += 1.0)
            {
                newy = origin.Y + _height * Math.Sin(((newx - origin.X) * 2 * Math.PI) * _count / _width);
                interpolatedList.Add(new Point((_oscillate) ? origin.X : (double)newx, (double)newy));
            }
            interpolated = interpolatedList.ToArray();
        }

    }

    #endregion       
}
