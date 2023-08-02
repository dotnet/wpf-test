// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2004
 *
 *   Program:   Test-hooked StringModifier
 
 *
 ************************************************************/

using System;


namespace                       Microsoft.Test.ElementServices.Freezables.Modifiers
{
    //--------------------------------------------------------------
    // The Freezable pattern is horribly violated by this class but I don't care
    // since I am not testing interaction with Freezable yet.

    public class                StringModifier              : System.Windows.Media.Animation.StringAnimationBase
    {
        //----------------------------------------------------------

        public                  StringModifier ( ModifierController c, string d )
        {
            _controller = c;
            _delta = d;
        }
        protected StringModifier()
        {
        }
       
        //----------------------------------------------------------
        protected override void CloneCore(System.Windows.Freezable sourceFreezable)
        {
            StringModifier stringModifier = (StringModifier)sourceFreezable;
            base.CloneCore(sourceFreezable);
            _controller = stringModifier._controller;
            _delta = stringModifier._delta;
   
        }
        protected override void GetAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            StringModifier stringModifier = (StringModifier)sourceFreezable;
            base.GetAsFrozenCore(sourceFreezable);
            _controller = stringModifier._controller;
            _delta = stringModifier._delta;

        }
        protected override void GetCurrentValueAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            StringModifier stringModifier = (StringModifier)sourceFreezable;
            base.GetCurrentValueAsFrozenCore(sourceFreezable);
            _controller = stringModifier._controller;
            _delta = stringModifier._delta;

        }
        public new StringModifier GetAsFrozen()
        {
            return (StringModifier)base.GetAsFrozen();
        }
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new StringModifier();
        }
     
        
        protected override string GetCurrentValueCore(string defaultOriginValue, string baseValue, System.Windows.Media.Animation.AnimationClock clock )
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
        private string              _delta;
    }
}
