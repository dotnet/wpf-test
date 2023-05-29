// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows; 

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// Static Gestures verifier 
    /// </summary>
    public class GestureVerifier : MultiTouchVerifier
    {
        #region Private Fields

        private StaticGestures _staticGesture;
        private UIElement _uiElement;

        #endregion 

        #region Constructor

        public GestureVerifier(UIElement element)
            : base()
        {
            _uiElement = element;
        }

        #endregion

        #region Public Properties

        public UIElement TargetElement
        {
            get
            {
                return _uiElement; 
            }
        }

        public StaticGestures Gesture
        {
            get
            {
                return _staticGesture;
            }
            set
            {
                _staticGesture = value;
            }
        }

        #endregion

        #region Public Methods

        public void VerifyGesture()
        {
            switch (Gesture)
            {
                case StaticGestures.TwoFingerTap:
                    PerformTwoFingerTap();
                    break;

                case StaticGestures.RollOver:
                    PerformRollOver();
                    break;

                case StaticGestures.Drag:
                case StaticGestures.Flick:
                case StaticGestures.HoldEnter:
                case StaticGestures.HoldLeave:
                case StaticGestures.HoverEnter:
                case StaticGestures.HoverLeave:
                case StaticGestures.RightDrag:
                case StaticGestures.RightTap:
                case StaticGestures.Tap:
                    // existing ones, should be covered by regression tests w/ existing one
                    break;

                case StaticGestures.None:
                    // do nothing
                    break;

                default:
                    throw new NotImplementedException(); // not in WPF4

            }
        }

        #endregion

        #region Helpers

        void PerformTwoFingerTap()
        { 
            // 
        }

        void PerformRollOver()
        { 
            // 
        }

        #endregion
    }
}
