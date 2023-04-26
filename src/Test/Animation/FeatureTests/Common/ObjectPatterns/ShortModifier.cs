// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

//------------------------------------------------------------------

namespace                       Microsoft.Test.Animation.ObjectPatterns
{
    //--------------------------------------------------------------

    /// <summary>
    /// 
    /// </summary>
    public class                Int16Modifier              : System.Windows.Media.Animation.Int16AnimationBase
    {
        //----------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        public                  Int16Modifier ( ModifierController c, short d )
        {
            _controller = c;
            _delta = d;
        }
        /// <summary>
        /// 
        /// </summary>
        protected Int16Modifier()
        {
        }
       
        //----------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        protected override void CloneCore(System.Windows.Freezable sourceFreezable)
        {
            Int16Modifier int16Modifier = (Int16Modifier)sourceFreezable;
            base.CloneCore(sourceFreezable);
            _controller = int16Modifier._controller;
            _delta = int16Modifier._delta;
   
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void GetAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            Int16Modifier int16Modifier = (Int16Modifier)sourceFreezable;
            base.GetAsFrozenCore(sourceFreezable);
            _controller = int16Modifier._controller;
            _delta = int16Modifier._delta;

        }
        /// <summary>
        /// 
        /// </summary>
        protected override void GetCurrentValueAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            Int16Modifier int16Modifier = (Int16Modifier)sourceFreezable;
            base.GetCurrentValueAsFrozenCore(sourceFreezable);
            _controller = int16Modifier._controller;
            _delta = int16Modifier._delta;

        }
        /// <summary>
        /// 
        /// </summary>
        public new Int16Modifier GetAsFrozen()
        {
            return (Int16Modifier)base.GetAsFrozen();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new Int16Modifier();
        }
     
        
        /// <summary>
        /// 
        /// </summary>
        protected override short  GetCurrentValueCore ( short defaultOriginValue, short baseValue, System.Windows.Media.Animation.AnimationClock clock )
        {
            if ( !_controller.UsesBaseValue )
            {
                return _delta;
            }
            else
            {
                return (short)(baseValue + _delta);
            }
        }

        //----------------------------------------------------------

        private ModifierController  _controller;
        private short               _delta;
    }
}
