// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// The base class for all multiple points gestures
    /// </summary>
    public class MultipleContactsGesture
    {
        #region Private Fields

        private List<SingleContactGesture> _singleGestureList;
        private bool _runSequentially = false;

        #endregion

        #region Constructor

        public MultipleContactsGesture()
        {
            _singleGestureList = new List<SingleContactGesture>();
        }

        #endregion

        #region Public Properties

        public bool RunSequentially
        {
            get { return _runSequentially; }
            set { _runSequentially = value; }
        }

        public int GestureCount
        {
            get { return _singleGestureList.Count; }
        }

        public List<SingleContactGesture> GestureList
        {
            get { return _singleGestureList; }
        }

        /// <summary>
        /// Duration and Samples properties are for scenarios where all SinglePointGestures in the list are using same Duration and Samples
        /// </summary>
        public virtual int Duration
        {
            get
            {
                int duration = _singleGestureList[0].Duration;
                foreach (SingleContactGesture singlegesture in _singleGestureList)
                {
                    if (duration != singlegesture.Duration)
                    {
                        throw new NotImplementedException("SingleGestures in the list are not using the same Duration. Please use the GestureList get the Duration for specified SinglePointGesture");
                    }
                }
                return duration;
            }
            set
            {
                foreach (SingleContactGesture singlegesture in _singleGestureList)
                {
                    if (value / singlegesture.Samples < SingleContactGesture.INTERVALMIN)
                    {
                        throw new ArgumentOutOfRangeException("Interval(Duration/Samples) must be bigger than INTERVALMIN  = " + SingleContactGesture.INTERVALMIN.ToString());
                    }
                }
                foreach (SingleContactGesture singlegesture in _singleGestureList)
                {
                    singlegesture.Duration = value;
                }
            }
        }

        /// <summary>
        /// Duration and Samples properties are for scenarios where all SinglePointGestures in the list are using same Duration and Samples
        /// </summary>
        public virtual int Samples
        {
            get
            {
                int samples = _singleGestureList[0].Samples;
                foreach (SingleContactGesture singlegesture in _singleGestureList)
                {
                    if (samples != singlegesture.Samples)
                    {
                        throw new NotImplementedException("SingleGestures in the list are not using the same Samples. Please use GestureList  to get the Samples for specified SinglePointGesture");
                    }
                }
                return samples;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("Samples must be a least 1");
                }
                foreach (SingleContactGesture singlegesture in _singleGestureList)
                {
                    singlegesture.Samples = value;
                }
            }
        }

        public virtual int ContactsInterval
        {
            get
            {
                int interval = _singleGestureList[0].ContactsInterval;
                foreach (SingleContactGesture singlegesture in _singleGestureList)
                {
                    if (interval != singlegesture.ContactsInterval)
                    {
                        throw new NotImplementedException("SingleGestures in the list are not using the same interval. Please use GestureList  to get the ContactsInterval for specified SinglePointGesture");
                    }
                }
                return interval;
            }
        }

        #endregion

        #region Public Methods

        public void AddCommand(SingleContactGesture singleGesture)
        {
            _singleGestureList.Add(singleGesture);
        }

        public void AddCommand(MultipleContactsGesture multiGesture)
        {
            if (multiGesture.GestureCount > 0)
            {
                _singleGestureList.AddRange(multiGesture.GestureList.ToArray());
            }
        }

        #endregion

    }
 }    
