// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2004
 *
 *   Program:   Test-hooked SingleModifier
 
 *
 ************************************************************/

using System;


namespace                       Microsoft.Test.ElementServices.Freezables.Modifiers
{
    //--------------------------------------------------------------
    // The Freezable pattern is horribly violated by this class but I don't care
    // since I am not testing interaction with Freezable yet.

    public class                SingleModifier              : System.Windows.Media.Animation.SingleAnimationBase
    {
        //----------------------------------------------------------

        public                  SingleModifier ( ModifierController c, float d )
        {
            _controller = c;
            _delta = d;
        }
        protected SingleModifier()
        {
        }
       
        //----------------------------------------------------------
        protected override void CloneCore(System.Windows.Freezable sourceFreezable)
        {
            SingleModifier singleModifier = (SingleModifier)sourceFreezable;
            base.CloneCore(sourceFreezable);
            _controller = singleModifier._controller;
            _delta = singleModifier._delta;
      
        }
        protected override void GetAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            SingleModifier singleModifier = (SingleModifier)sourceFreezable;
            base.GetAsFrozenCore(sourceFreezable);
            _controller = singleModifier._controller;
            _delta = singleModifier._delta;

        }
        protected override void GetCurrentValueAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            SingleModifier singleModifier = (SingleModifier)sourceFreezable;
            base.GetCurrentValueAsFrozenCore(sourceFreezable);
            _controller = singleModifier._controller;
            _delta = singleModifier._delta;

        }
        public new SingleModifier GetAsFrozen()
        {
            return (SingleModifier)base.GetAsFrozen();
        }
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new SingleModifier();
        }
        
        protected override float  GetCurrentValueCore (float defaultOriginValue, float baseValue, System.Windows.Media.Animation.AnimationClock clock )
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
        private float               _delta;
    }
}
