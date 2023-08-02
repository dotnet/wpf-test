// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2003
 *
 *   Program:   Test-hooked ColorModifier
 
 *
 ************************************************************/

using System;


namespace                       Microsoft.Test.ElementServices.Freezables.Modifiers
{
    //--------------------------------------------------------------
    // The Freezable pattern is horribly violated by this class but I don't care
    // since I am not testing interaction with Freezable yet.

    public class                ColorModifier              : System.Windows.Media.Animation.ColorAnimationBase
    {
        //----------------------------------------------------------

        public                  ColorModifier ( ModifierController c, float da, float dr, float dg, float db )
        {
            _controller = c;
            _deltaA = da;
            _deltaR = dr;
            _deltaG = dg;
            _deltaB = db;
        }
        protected ColorModifier()
        {
        }
        //----------------------------------------------------------
        protected override void CloneCore(System.Windows.Freezable sourceFreezable)
        {
            ColorModifier colorModifier = (ColorModifier)sourceFreezable;
            base.CloneCore(sourceFreezable);
            _controller = colorModifier._controller;
            _deltaA = colorModifier._deltaA;
            _deltaR = colorModifier._deltaR;
            _deltaG = colorModifier._deltaG;
            _deltaB = colorModifier._deltaB;

        }
        protected override void GetAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            ColorModifier colorModifier = (ColorModifier)sourceFreezable;
            base.GetAsFrozenCore(sourceFreezable);
            _controller = colorModifier._controller;
            _deltaA = colorModifier._deltaA;
            _deltaR = colorModifier._deltaR;
            _deltaG = colorModifier._deltaG;
            _deltaB = colorModifier._deltaB;

        }
        protected override void GetCurrentValueAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            ColorModifier colorModifier = (ColorModifier)sourceFreezable;
            base.GetCurrentValueAsFrozenCore(sourceFreezable);
            _controller = colorModifier._controller;
            _deltaA = colorModifier._deltaA;
            _deltaR = colorModifier._deltaR;
            _deltaG = colorModifier._deltaG;
            _deltaB = colorModifier._deltaB;

        }
        public new ColorModifier GetAsFrozen()
        {
            return (ColorModifier)base.GetAsFrozen();
        }
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new ColorModifier();
        }
        protected override System.Windows.Media.Color
                                GetCurrentValueCore(System.Windows.Media.Color defaultOriginValue, System.Windows.Media.Color baseValue, System.Windows.Media.Animation.AnimationClock clock)
        {
            if ( !_controller.UsesBaseValue )
            {
                return System.Windows.Media.Color.FromScRgb ( _deltaA, _deltaR, _deltaG, _deltaB );
            }
            else
            {
                return System.Windows.Media.Color.FromScRgb ( baseValue.ScA + _deltaA, baseValue.ScR + _deltaR, baseValue.ScG + _deltaG, baseValue.ScB + _deltaB );
            }
        }

        //----------------------------------------------------------

        private ModifierController  _controller;
        private float               _deltaA;
        private float               _deltaR;
        private float               _deltaG;
        private float               _deltaB;
    }
}
