// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2003
 *
 *   Program:   Test-hooked PointModifier
 
 *
 ************************************************************/

using System;


namespace                       Microsoft.Test.ElementServices.Freezables.Modifiers
{
    //--------------------------------------------------------------
    // The Freezable pattern is horribly violated by this class but I don't care
    // since I am not testing interaction with Freezable yet.

    public class                PointModifier              : System.Windows.Media.Animation.PointAnimationBase
    {
        //----------------------------------------------------------

        public                  PointModifier ( ModifierController c, double dx, double dy )
        {
            _controller = c;
            _deltaX = dx;
            _deltaY = dy;
        }
        protected PointModifier()
        {
        }
       
        //----------------------------------------------------------
        protected override void CloneCore(System.Windows.Freezable sourceFreezable)
        {
            PointModifier pointModifier = (PointModifier)sourceFreezable;
            base.CloneCore(sourceFreezable);
            _controller = pointModifier._controller;
            _deltaX = pointModifier._deltaX;
            _deltaY = pointModifier._deltaY;
        }
        protected override void GetAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            PointModifier pointModifier = (PointModifier)sourceFreezable;
            base.GetAsFrozenCore(sourceFreezable);
            _controller = pointModifier._controller;
            _deltaX = pointModifier._deltaX;
            _deltaY = pointModifier._deltaY;
        }
        protected override void GetCurrentValueAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            PointModifier pointModifier = (PointModifier)sourceFreezable;
            base.GetCurrentValueAsFrozenCore(sourceFreezable);
            _controller = pointModifier._controller;
            _deltaX = pointModifier._deltaX;
            _deltaY = pointModifier._deltaY;
        }
        public new PointModifier GetAsFrozen()
        {
            return (PointModifier)base.GetAsFrozen();
        }
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new PointModifier();
        }
        protected override System.Windows.Point
                                GetCurrentValueCore(System.Windows.Point defaultOriginValue, System.Windows.Point baseValue, System.Windows.Media.Animation.AnimationClock clock)
        {
            if ( !_controller.UsesBaseValue )
            {
                return new System.Windows.Point ( _deltaX, _deltaY );
            }
            else
            {
                return new System.Windows.Point ( baseValue.X + _deltaX, baseValue.Y + _deltaY );
            }
        }

        //----------------------------------------------------------

        private ModifierController  _controller;
        private double              _deltaX;
        private double              _deltaY;
    }
}
