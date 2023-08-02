// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2004
 *
 *   Program:   Test-hooked ByteModifier
 
 *
 ************************************************************/

using System;


namespace                       Microsoft.Test.ElementServices.Freezables.Modifiers
{
    //--------------------------------------------------------------
    // The Freezable pattern is horribly violated by this class but I don't care
    // since I am not testing interaction with Freezable yet.

    public class                ByteModifier              : System.Windows.Media.Animation.ByteAnimationBase
    {
        //----------------------------------------------------------

        public                  ByteModifier ( ModifierController c, System.Byte d )
        {
            _controller = c;
            _delta = d;
        }
        protected ByteModifier()
        {
        }
        //----------------------------------------------------------
        protected override void CloneCore(System.Windows.Freezable sourceFreezable)
        {
            ByteModifier byteModifier = (ByteModifier)sourceFreezable;
            base.CloneCore(sourceFreezable);
            _controller = byteModifier._controller;
            _delta = byteModifier._delta;
        }
        //----------------------------------------------------------
        protected override void GetAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            ByteModifier byteModifier = (ByteModifier)sourceFreezable;
            base.GetAsFrozenCore(sourceFreezable);
            _controller = byteModifier._controller;
            _delta = byteModifier._delta;
        }
        //----------------------------------------------------------
        protected override void GetCurrentValueAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            ByteModifier byteModifier = (ByteModifier)sourceFreezable;
            base.GetCurrentValueAsFrozenCore(sourceFreezable);
            _controller = byteModifier._controller;
            _delta = byteModifier._delta;
        }
        public new ByteModifier GetAsFrozen()
        {
            return (ByteModifier)base.GetAsFrozen();
        }
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new ByteModifier();
        }
        
        protected override System.Byte  GetCurrentValueCore (System.Byte defaultOriginValue, System.Byte baseValue, System.Windows.Media.Animation.AnimationClock clock )
        {
            if ( !_controller.UsesBaseValue )
            {
                return _delta;
            }
            else
            {
                return (byte)(baseValue + _delta);
            }
        }

        //----------------------------------------------------------

        private ModifierController  _controller;
        private System.Byte         _delta;
    }
}
