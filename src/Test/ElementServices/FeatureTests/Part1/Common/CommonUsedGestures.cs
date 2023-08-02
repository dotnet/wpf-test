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
    ///**************************************************************************//
    /// Commonly Used Input Gesture Commands                                     //
    /// All are of type MultiplePointGesture, while SingelPointGesture           //
    /// is a special case for MultiplePointGesture                               //
    ///**************************************************************************//
    /// </summary>

    #region "Touch And Release" gesture

    /// <summary>
    /// Represent a single point touch that presses and releases for a given number of milliseconds
    /// </summary>
    public class TouchAndReleaseGesture : MultipleContactsGesture
    {
        public TouchAndReleaseGesture(Point point)
        {
            SingleContactGesture dotgesture = new DotGesture(point);
            base.AddCommand(dotgesture);
        }
        
        public TouchAndReleaseGesture(Point point, int milliseconds)      
        {
            SingleContactGesture dotgesture = new DotGesture(point, milliseconds);
            base.AddCommand(dotgesture);
        }
    }
    
    #endregion

    #region "Drag" gesture

    /// <summary>
    /// Represents a drage gesture moves through a set of points defined by the given PointF[]
    /// </summary>
    public class DragGesture : MultipleContactsGesture
    {        
        public DragGesture(Point start, Point end)
            : this(new Point[] { start, end })
        {
        }

        public DragGesture(double x1, double y1, double x2, double y2)
            : this(new Point[] { new Point(x1, y1), new Point(x2, y2) })
        {
        }

        public DragGesture(Point[] points)
            : this(points, false)
        {
        }

        public DragGesture(Point[] points, bool bsmoothed) 
        {
            SingleContactGesture linegesture = new LineGesture(points, bsmoothed);
            base.AddCommand(linegesture);
        }    
    }

    #endregion

    #region "Pan" gesture

    /// <summary>
    /// Represents a pan gesture that moves through a set of points defined by the given PointF[]
    /// </summary>
    public class PanGesture : MultipleContactsGesture
    {
        public PanGesture(Point start, Point end)
            : this(new Point[] { start, end })
        {
        }

        public PanGesture(float x1, float y1, float x2, float y2)
            : this(new Point[] { new Point(x1, y1), new Point(x2, y2) })
        {
        }

        public PanGesture(Point[] points)
            : this(points, false)
        {
        }

        public PanGesture(Point[] points, bool bsmoothed)
        {
            SingleContactGesture linegesture = new LineGesture(points, bsmoothed);
            base.AddCommand(linegesture);
        }
    }

    #endregion
    
    #region "Zoom" gesture

    /// <summary>
    /// Represents a Zoom gesture
    /// 1. one point stays, another finger moves from begin point to end point 
    /// 2. two points move from begin point to end point respectively   
    /// 3. all point constitute a circle and its centre stays when moving
    /// 4. specific line combination
    /// </summary>
    public class ZoomGesture : MultipleContactsGesture
    {
        /// <summary>
        /// Zoom with one point stays and another point moves
        /// </summary>
        /// <param name="stayPoint"></param>
        /// <param name="movePointStart"></param>
        /// <param name="movePointEnd"></param>
        public ZoomGesture(Point stayPoint, Point movePointStart, Point movePointEnd)
            : this(stayPoint, stayPoint, movePointStart, movePointEnd)
        {
        }

        /// <summary>
        /// Zoom with two point move respectively
        /// </summary>
        /// <param name="firstStart"></param>
        /// <param name="firstEnd"></param>
        /// <param name="secondStart"></param>
        /// <param name="secondEnd"></param>
        public ZoomGesture(Point firstStart, Point firstEnd, Point secondStart, Point secondEnd)
        {
            Point[] first = new Point[2];
            Point[] second = new Point[2];

            first[0] = firstStart;
            first[1] = firstEnd;
            GenerateGesture(first);

            second[0] = secondStart;
            second[1] = secondEnd;
            GenerateGesture(second);
        }

        /// <summary>
        /// All point constitute a circle and its centre stay when moving
        /// </summary>
        /// <param name="center"></param>
        /// <param name="fingerCount"></param>
        /// <param name="startRadius"></param>
        /// <param name="endRadius"></param>
        public ZoomGesture(Point center, int fingerCount, double startRadius, double endRadius)
        {
            if (fingerCount < 1 || fingerCount > 10)
            {
                throw new ArgumentOutOfRangeException("fingerCount", "fingerCount must greater than 0 and less than or equal to 10");
            }

            if (startRadius <= 0)
            {
                throw new ArgumentOutOfRangeException("startRadius", "startRadius must greater than 0");
            }

            if (endRadius <= 0)
            {
                throw new ArgumentOutOfRangeException("endRadius", "endRadius must greater than 0");
            }

            Point[] point = null;
            for (int i = 0; i < fingerCount; i++)
            {
                point = new Point[2];

                point[0].X = (double)(center.X + startRadius * Math.Cos(Math.PI * i * 2 / fingerCount));
                point[0].Y = (double)(center.Y + startRadius * Math.Sin(Math.PI * i * 2 / fingerCount));

                point[1].X = (double)(center.X + endRadius * Math.Cos(Math.PI * i * 2 / fingerCount));
                point[1].Y = (double)(center.Y + endRadius * Math.Sin(Math.PI * i * 2 / fingerCount));

                GenerateGesture(point);
            }
        }

        /// <summary>
        /// Zoom with specific line combination
        /// </summary>
        /// <param name="lineGestures"></param>
        public ZoomGesture(LineGesture[] lineGestures)
        {
            if (lineGestures == null)
            {
                throw new ArgumentNullException("lineGestures", "lineGestures cannot be null");
            }

            foreach (LineGesture lineGesture in lineGestures)
            {
                if (lineGesture == null)
                {
                    throw new ArgumentNullException("lineGesture", "lineGesture cannot be null");
                }
                AddCommand(lineGesture);
            }
        }

        private void GenerateGesture(Point[] point)
        {
            LineGesture lineGesture = new LineGesture(point, false);
            AddCommand(lineGesture);
        }
    }

    #endregion

    #region "Rotate" gesture

    /// <summary>
    /// Represents a Rotate gesture
    /// 1. Two fingers rotate : one finger press at center, another performs rotate 
    /// 2. Multiple fingers (>=2) rotate, locating at one circle evenly
    /// 3. Rotate with customized archs
    /// </summary>
    public class RotateGesture : MultipleContactsGesture
    {
        public override int ContactsInterval
        {
            get
            {
                foreach (SingleContactGesture singlegesture in GestureList)
                {
                    if (singlegesture is ArcGesture)
                    {
                        return singlegesture.ContactsInterval;
                    }
                }
                throw new NotImplementedException("no interval value provided");
            }
        }

        public override int Samples
        {
            get
            {
                foreach (SingleContactGesture singlegesture in GestureList)
                {
                    if (singlegesture is ArcGesture)
                    {
                        return singlegesture.Samples;
                    }
                }
                throw new NotImplementedException("no samples value provided");
            }

            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("Samples should be at least 1");
                }
                foreach (SingleContactGesture singlegesture in GestureList)
                {
                    if (singlegesture is ArcGesture)
                    {
                        singlegesture.Samples = value;
                    }
                }
            }
        }

        public override int Duration
        {
            get
            {
                foreach (SingleContactGesture singlegesture in GestureList)
                {
                    if (singlegesture is ArcGesture)
                    {
                        return singlegesture.Duration;
                    }
                }
                throw new NotImplementedException("no duration value provided");
            }
            set
            {
                if (value < SingleContactGesture.INTERVALMIN)
                {
                    throw new ArgumentOutOfRangeException("Interval must be bigger than INTERVALMIN  = " + SingleContactGesture.INTERVALMIN.ToString());
                }
                foreach (SingleContactGesture singlegesture in GestureList)
                {
                    if (singlegesture is ArcGesture)
                    {
                        singlegesture.Duration = value;
                    }
                    else if (singlegesture is DotGesture)
                    {
                        //DotGesture used to use the same duration with arc gestures. However, as the time needed for 
                        //every touch operation is often much bigger than ContactInterval, this caused the total time 
                        //used for arc gesture is bigger than the duration value.
                        //This will cause the DotGesture to be finished much earlier than arc gestures as it always just has one sample.
                        //To double the time so the rotate gesture can be finished completely.
                        //This is a magic number, may have a better solution in future.
                        singlegesture.Duration = value * 2;
                    }
                }
            }
        }

        public RotateGesture(Point center, double height, double width, double startAngle, double endAngle)
            : this(center, 1, height, width, startAngle, endAngle, true)
        {
        }

        public RotateGesture(Point center, int fingerCount, double height, double width, double startAngle, double endAngle)
            : this(center, fingerCount, height, width, startAngle, endAngle, true)
        {
        }

        private RotateGesture(Point center, int fingerCount, double height, double width, double startAngle, double endAngle, bool pressCenter)
        {
            if (fingerCount <= 0 || fingerCount > 10)
            {
                throw new ArgumentOutOfRangeException("fingerCount", "fingerCount must be greater than 0 and less than or equal to 10");
            }

            int arcDuration = 1000;     //Make each arc have a 1 second duration
            for (int i = 0; i < fingerCount; i++)
            {
                ArcGesture arcGesture = new ArcGesture(center,
                                                       height,
                                                       width,
                                                       startAngle + i * 360 / fingerCount,
                                                       endAngle + i * 360 / fingerCount);
                arcGesture.Duration = arcDuration;
                AddCommand(arcGesture);
            }

            if (pressCenter)
            {
                //In this case, leave the dot gesture running a little longer than the arc gesture
                DotGesture dotGesture = new DotGesture(center, arcDuration + 3000);
                AddCommand(dotGesture);
            }
        }

        public RotateGesture(ArcGesture[] arcGestures)
        {
            foreach (ArcGesture arcGesture in arcGestures)
            {
                AddCommand(arcGesture);
            }
        }
    }

    #endregion               
    
    #region "Circle" gesture

    /// <summary>
    /// Traces out a circle of set radius from the location given
    /// </summary>
    public class CircleGesture : MultipleContactsGesture
    {
        public CircleGesture(double originX, double originY, double radiusInches)            
        {
            SingleContactGesture arcgesture = new ArcGesture(new Point(originX, originY), radiusInches, radiusInches, 0.0, 360.0);
            base.AddCommand(arcgesture);
        }    
    }

    #endregion

    #region "Arc" gesture

    /// <summary>
    /// Traces out a circle of set radius from the location given
    /// </summary>
    public class EllipseArcGesture : MultipleContactsGesture
    {
        public EllipseArcGesture(Point center, double height, double width, double startAngle, double endAngle)
        {
            SingleContactGesture archgesture = new ArcGesture(center, height, width, startAngle, endAngle);
            base.AddCommand(archgesture);
        }
    }

    #endregion
   
    #region "Wave" gesture

    /// <summary>
    /// Represents a single point touch that traces a path of a sine wave, starting at the given location
    /// and with set amplitude and frequency
    /// </summary>
    public class WaveGesture : MultipleContactsGesture
    {
        public WaveGesture(double originX, double originY, double heightInches, double widthInches, int count)
        {
            SingleContactGesture sinegesture = new SineGesture(originX, originY, heightInches, widthInches, count, false);
            base.AddCommand(sinegesture);           
        }
    }

    #endregion

    #region "Translate" gesture

    /// <summary>
    /// Represents a pan gesture that moves through 
    /// a set of points defined by the given PointF[]
    /// </summary>
    public class TranslateGesture : MultipleContactsGesture
    {
        public TranslateGesture(List<Point[]> points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                SingleContactGesture lingesture = new LineGesture(points[i][0], points[i][1]);
                base.AddCommand(lingesture);
            }
        }
    }

    #endregion

}
