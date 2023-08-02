// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2004
 *
 *   Program:   Test-hooked Int64Modifier
 
 *
 ************************************************************/

using System;


namespace                       Microsoft.Test.ElementServices.Freezables.Modifiers
{
    //--------------------------------------------------------------
    // The Freezable pattern is horribly violated by this class but I don't care
    // since I am not testing interaction with Freezable yet.

    public class                Int64Modifier              : System.Windows.Media.Animation.Int64AnimationBase
    {
        //----------------------------------------------------------

        public                  Int64Modifier ( ModifierController c, long d )
        {
            _controller = c;
            _delta = d;
        }
         protected Int64Modifier()
        {
        }
       
        //----------------------------------------------------------
        protected override void CloneCore(System.Windows.Freezable sourceFreezable)
        {
            Int64Modifier int64Modifier = (Int64Modifier)sourceFreezable;
            base.CloneCore(sourceFreezable);
            _controller = int64Modifier._controller;
            _delta = int64Modifier._delta;
      
        }
        protected override void GetAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            Int64Modifier int64Modifier = (Int64Modifier)sourceFreezable;
            base.GetAsFrozenCore(sourceFreezable);
            _controller = int64Modifier._controller;
            _delta = int64Modifier._delta;

        }
        protected override void GetCurrentValueAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            Int64Modifier int64Modifier = (Int64Modifier)sourceFreezable;
            base.GetCurrentValueAsFrozenCore(sourceFreezable);
            _controller = int64Modifier._controller;
            _delta = int64Modifier._delta;

        }
        public new Int64Modifier GetAsFrozen()
        {
            return (Int64Modifier)base.GetAsFrozen();
        }
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new Int64Modifier();
        }
     
        
        protected override long  GetCurrentValueCore (long defaultOriginValue, long baseValue, System.Windows.Media.Animation.AnimationClock clock )
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
        private long                _delta;
    }
}
