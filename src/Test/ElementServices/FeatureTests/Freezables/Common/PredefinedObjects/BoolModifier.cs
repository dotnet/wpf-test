// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2004
 *
 *   Program:   Test-hooked BooleanModifier
 
 *
 ************************************************************/

using System;


namespace                       Microsoft.Test.ElementServices.Freezables.Modifiers
{
    //--------------------------------------------------------------
    // The Freezable pattern is horribly violated by this class but I don't care
    // since I am not testing interaction with Freezable yet.

    public class                BooleanModifier              : System.Windows.Media.Animation.BooleanAnimationBase
    {
        //----------------------------------------------------------

        public                  BooleanModifier ( ModifierController c, bool b )
        {
            _controller = c;
            _delta = b;
        }
        public BooleanModifier()
        {
        }
        //----------------------------------------------------------
        public new BooleanModifier GetAsFrozen()
        {
            return (BooleanModifier)base.GetAsFrozen();
        }
        //----------------------------------------------------------
        protected override void CloneCore(System.Windows.Freezable sourceFreezable)
        {
            BooleanModifier booleanModifier = (BooleanModifier)sourceFreezable;
      
            base.CloneCore(sourceFreezable);
            _controller = booleanModifier._controller;
            _delta = booleanModifier._delta;
        }
        //----------------------------------------------------------
        protected override void GetAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            BooleanModifier booleanModifier = (BooleanModifier)sourceFreezable;
     
            base.GetAsFrozenCore(sourceFreezable);
            _controller = booleanModifier._controller;
            _delta = booleanModifier._delta;
        }
        //----------------------------------------------------------
        protected override void GetCurrentValueAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            BooleanModifier booleanModifier = (BooleanModifier)sourceFreezable;

            base.GetCurrentValueAsFrozenCore(sourceFreezable);
            _controller = booleanModifier._controller;
            _delta = booleanModifier._delta;
        }
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new BooleanModifier();
        }
     
        
        protected override bool  GetCurrentValueCore ( bool defaultOriginValue, bool baseValue, System.Windows.Media.Animation.AnimationClock clock )
        {
            if (!_controller.UsesBaseValue)
            {
                return _delta;
            }
            else
            {
                return (baseValue && _delta);
            }
        }

        //----------------------------------------------------------

        private ModifierController  _controller;
        private bool                _delta;
    }
}
