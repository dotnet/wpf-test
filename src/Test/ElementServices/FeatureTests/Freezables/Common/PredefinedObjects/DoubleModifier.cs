// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2003
 *
 *   Program:   Test-hooked DoubleModifier
 
 *
 ************************************************************/

using System;


namespace                       Microsoft.Test.ElementServices.Freezables.Modifiers
{
    //--------------------------------------------------------------
    // The Freezable pattern is horribly violated by this class but I don't care
    // since I am not testing interaction with Freezable yet.

    public class                DoubleModifier              : System.Windows.Media.Animation.DoubleAnimationBase
    {
        //----------------------------------------------------------

        public                  DoubleModifier ( ModifierController c, double d )
        {
            _controller = c;
            _delta = d;
        }
        protected DoubleModifier()
        {
        }
       
        //----------------------------------------------------------
        protected override void CloneCore(System.Windows.Freezable sourceFreezable)
        {
            DoubleModifier doubleModifier = (DoubleModifier)sourceFreezable;
            base.CloneCore(sourceFreezable);
            _controller = doubleModifier._controller;
            _delta = doubleModifier._delta;
    
        }
        //----------------------------------------------------------
        protected override void GetAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            DoubleModifier doubleModifier = (DoubleModifier)sourceFreezable;
            base.GetAsFrozenCore(sourceFreezable);
            _controller = doubleModifier._controller;
            _delta = doubleModifier._delta;

        }
        //----------------------------------------------------------
        protected override void GetCurrentValueAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            DoubleModifier doubleModifier = (DoubleModifier)sourceFreezable;
            base.GetCurrentValueAsFrozenCore(sourceFreezable);
            _controller = doubleModifier._controller;
            _delta = doubleModifier._delta;

        }
        public new DoubleModifier GetAsFrozen()
        {
            return (DoubleModifier)base.GetAsFrozen();
        }
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new DoubleModifier();
        }
        
        protected override double  GetCurrentValueCore (double defaultOriginValue, double baseValue, System.Windows.Media.Animation.AnimationClock clock )
        {
            if ( !_controller.UsesBaseValue )
            {
                return _delta;
            }
            else
            {
                return baseValue + _delta;
            }
        }

        //----------------------------------------------------------

        private ModifierController  _controller;
        private double              _delta;
    }
}
