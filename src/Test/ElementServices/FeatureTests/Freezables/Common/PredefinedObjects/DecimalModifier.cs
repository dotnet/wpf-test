// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2004
 *
 *   Program:   Test-hooked DecimalModifier
 
 *
 ************************************************************/

using System;


namespace                       Microsoft.Test.ElementServices.Freezables.Modifiers
{
    //--------------------------------------------------------------
    // The Freezable pattern is horribly violated by this class but I don't care
    // since I am not testing interaction with Freezable yet.

    public class                DecimalModifier              : System.Windows.Media.Animation.DecimalAnimationBase
    {
        //----------------------------------------------------------

        public                  DecimalModifier ( ModifierController c, System.Decimal d )
        {
            _controller = c;
            _delta = d;
        }
        protected DecimalModifier()
        {
        }
               //----------------------------------------------------------
        protected override void CloneCore(System.Windows.Freezable sourceFreezable)
        {
            DecimalModifier decimalModifier = (DecimalModifier)sourceFreezable;
            base.CloneCore(sourceFreezable);
            _controller = decimalModifier._controller;
            _delta = decimalModifier._delta;
  
        }
        protected override void GetAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            DecimalModifier decimalModifier = (DecimalModifier)sourceFreezable;
            base.GetAsFrozenCore(sourceFreezable);
            _controller = decimalModifier._controller;
            _delta = decimalModifier._delta;

        }
        protected override void GetCurrentValueAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            DecimalModifier decimalModifier = (DecimalModifier)sourceFreezable;
            base.GetCurrentValueAsFrozenCore(sourceFreezable);
            _controller = decimalModifier._controller;
            _delta = decimalModifier._delta;

        }
        public new DecimalModifier GetAsFrozen()
        {
            return (DecimalModifier)base.GetAsFrozen();
        }
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new DecimalModifier();
        }
        
        protected override System.Decimal GetCurrentValueCore(System.Decimal defaultOriginValue, System.Decimal baseValue, System.Windows.Media.Animation.AnimationClock clock )
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
        private System.Decimal      _delta;
    }
}
