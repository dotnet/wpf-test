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
    public class                DecimalModifier              : System.Windows.Media.Animation.DecimalAnimationBase
    {
        //----------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        public                  DecimalModifier ( ModifierController c, System.Decimal d )
        {
            _controller = c;
            _delta = d;
        }
        /// <summary>
        /// 
        /// </summary>
        protected DecimalModifier()
        {
        }
               //----------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        protected override void CloneCore(System.Windows.Freezable sourceFreezable)
        {
            DecimalModifier decimalModifier = (DecimalModifier)sourceFreezable;
            base.CloneCore(sourceFreezable);
            _controller = decimalModifier._controller;
            _delta = decimalModifier._delta;
  
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void GetAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            DecimalModifier decimalModifier = (DecimalModifier)sourceFreezable;
            base.GetAsFrozenCore(sourceFreezable);
            _controller = decimalModifier._controller;
            _delta = decimalModifier._delta;

        }
        /// <summary>
        /// 
        /// </summary>
        protected override void GetCurrentValueAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            DecimalModifier decimalModifier = (DecimalModifier)sourceFreezable;
            base.GetCurrentValueAsFrozenCore(sourceFreezable);
            _controller = decimalModifier._controller;
            _delta = decimalModifier._delta;

        }
        /// <summary>
        /// 
        /// </summary>
        public new DecimalModifier GetAsFrozen()
        {
            return (DecimalModifier)base.GetAsFrozen();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new DecimalModifier();
        }
        
        /// <summary>
        /// 
        /// </summary>
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
