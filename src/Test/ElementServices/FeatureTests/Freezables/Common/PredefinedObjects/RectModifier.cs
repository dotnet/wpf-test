// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2003
 *
 *   Program:   Test-hooked RectModifier
 
 *
 ************************************************************/

using System;


namespace                       Microsoft.Test.ElementServices.Freezables.Modifiers
{
    //--------------------------------------------------------------
    // The Freezable pattern is horribly violated by this class but I don't care
    // since I am not testing interaction with Freezable yet.

    public class                RectModifier              : System.Windows.Media.Animation.RectAnimationBase
    {
        //----------------------------------------------------------

        public                  RectModifier ( ModifierController c, double dx, double dy, double dw, double dh )
        {
            _controller = c;
            _deltaX = dx;
            _deltaY = dy;
            _deltaW = dw;
            _deltaH = dh;
        }
        protected RectModifier()
        {
        }
       
        //----------------------------------------------------------
        protected override void CloneCore(System.Windows.Freezable sourceFreezable)
        {
            RectModifier rectModifier = (RectModifier)sourceFreezable;
            base.CloneCore(sourceFreezable);
            _controller = rectModifier._controller;
            _deltaX = rectModifier._deltaX;
            _deltaY = rectModifier._deltaY;
            _deltaW = rectModifier._deltaW;
            _deltaH = rectModifier._deltaH;

        }
        protected override void GetAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            RectModifier rectModifier = (RectModifier)sourceFreezable;
            base.GetAsFrozenCore(sourceFreezable);
            _controller = rectModifier._controller;
            _deltaX = rectModifier._deltaX;
            _deltaY = rectModifier._deltaY;
            _deltaW = rectModifier._deltaW;
            _deltaH = rectModifier._deltaH;

        }
        protected override void GetCurrentValueAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            RectModifier rectModifier = (RectModifier)sourceFreezable;
            base.GetCurrentValueAsFrozenCore(sourceFreezable);
            _controller = rectModifier._controller;
            _deltaX = rectModifier._deltaX;
            _deltaY = rectModifier._deltaY;
            _deltaW = rectModifier._deltaW;
            _deltaH = rectModifier._deltaH;

        }
        public new RectModifier GetAsFrozen()
        {
            return (RectModifier)base.GetAsFrozen();
        }
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new RectModifier();
        } 
        //----------------------------------------------------------

        protected override System.Windows.Rect
                                GetCurrentValueCore(System.Windows.Rect defaultOriginValue, System.Windows.Rect baseValue, System.Windows.Media.Animation.AnimationClock clock)
        {
            if ( !_controller.UsesBaseValue )
            {
                return new System.Windows.Rect ( _deltaX, _deltaY, _deltaW, _deltaH );
            }
            else
            {
                return new System.Windows.Rect ( baseValue.X + _deltaX, baseValue.Y + _deltaY, baseValue.Width + _deltaW, baseValue.Height + _deltaH );
            }
        }

        //----------------------------------------------------------

        private ModifierController  _controller;
        private double              _deltaX;
        private double              _deltaY;
        private double              _deltaW;
        private double              _deltaH;
    }
}
