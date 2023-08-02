// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2004
 *
 *   Program:   Test-hooked CharModifier
 
 *
 ************************************************************/

using System;


namespace                       Microsoft.Test.ElementServices.Freezables.Modifiers
{
    //--------------------------------------------------------------
    // The Freezable pattern is horribly violated by this class but I don't care
    // since I am not testing interaction with Freezable yet.

    public class                CharModifier              : System.Windows.Media.Animation.CharAnimationBase
    {
        //----------------------------------------------------------

        public                  CharModifier ( ModifierController c, char d )
        {
            _controller = c;
            _delta = d;
        }
        protected CharModifier()
        {
        }
        //----------------------------------------------------------
        protected override void CloneCore(System.Windows.Freezable sourceFreezable)
        {
            CharModifier charModifier = (CharModifier)sourceFreezable;
            base.CloneCore(sourceFreezable);
            _controller = charModifier._controller;
            _delta = charModifier._delta;
        }
        protected override void GetAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            CharModifier charModifier = (CharModifier)sourceFreezable;
            base.GetAsFrozenCore(sourceFreezable);
            _controller = charModifier._controller;
            _delta = charModifier._delta;
        }
        protected override void GetCurrentValueAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            CharModifier charModifier = (CharModifier)sourceFreezable;
            base.GetCurrentValueAsFrozenCore(sourceFreezable);
            _controller = charModifier._controller;
            _delta = charModifier._delta;
        }
        public new CharModifier GetAsFrozen()
        {
            return (CharModifier)base.GetAsFrozen();
        }
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new CharModifier();
        }
        
        protected override char GetCurrentValueCore(char defaultOriginValue, char baseValue, System.Windows.Media.Animation.AnimationClock clock )
        {
            if (!_controller.UsesBaseValue)
            {
                return _delta;
            }
            else
            {
                return (char)('a' + 'b');
            }
                
        }

        //----------------------------------------------------------

        private ModifierController  _controller;
        private char                _delta;
    }
}
