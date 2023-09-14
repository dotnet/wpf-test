// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows; 

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// The base class for all single point gestures
    /// </summary>
    public abstract class SingleContactGesture
    {
        #region Fields

        internal Point origin;
        //internal System.Drawing.Point origin;
        internal Point[] interpolated;
        //private Timer gestureTimer = null; // 
        internal int starttime = 0;
        internal double orientation = 0.0;
        internal int samples = 50;    // 
        internal int duration = 1000; // default duration = 3 secs, suitable for the Win7 input stack
        internal int? contactid = null;
        internal bool recalcOnSamplesChange = true;
        internal const int INTERVALMIN = 1; 

        #endregion

        #region Public Properties

        public Point[] Interpolated
        {
            get { return interpolated; }
            set { interpolated = value; }
        }

        public Point Location
        {
            get { return origin; }
            set { origin = value; }
        }

        public int? ContactId
        {
            get { return contactid; }
            set { contactid = value; }
        }

        public bool RecalcOnSamplesChange
        {
            get { return recalcOnSamplesChange; }
            set { recalcOnSamplesChange = value; }
        }

        /// <summary>
        /// gets the points in the animation, defaults is interpolated 
        /// user can override this properties to get a better points to animation
        /// </summary>
        internal virtual Point[] Points
        {
            get
            {
                return interpolated;
            }
        }

        public int Duration
        {
            get
            {
                return  duration;
            }
            set
            {
                if ((value/samples) < INTERVALMIN)
                {
                    throw new ArgumentOutOfRangeException("interval(duration/samples) must be bigger than INTERVALMIN = " + INTERVALMIN.ToString());
                }
                duration = value;
            }
        }

        public int ContactsInterval
        {
            get
            {
                return duration/samples;
            }           
        }

        public int StartTime
        {
            get { return starttime; }
            set { starttime = value; }
        }

        public int Samples
        {
            get
            {
                return samples;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("samples must be a least 1");
                }
                samples = value;

                if (true == recalcOnSamplesChange)
                {
                    CalculatePoints();
                }
            }
        }

        public double Orientation
        {
            get { return orientation; }
            set { orientation = value; }
        }

        #endregion

        #region Public and Internal Methods

        /// <summary>
        /// implment this to provide meaningful points track for the gesture
        /// </summary>
        public abstract void CalculatePoints();
        
        internal void Reset()
        {
            //resume to resend the same gesture
            contactid = null;
        }

        #endregion
    }  
}
