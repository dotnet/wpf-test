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
    public class                DoubleModifier              : System.Windows.Media.Animation.DoubleAnimationBase
    {
        //----------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        public                  DoubleModifier ( ModifierController c, double d )
        {
            _controller = c;
            _delta = d;
        }
        /// <summary>
        /// 
        /// </summary>
        protected DoubleModifier()
        {
        }
       
        //----------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        protected override void CloneCore(System.Windows.Freezable sourceFreezable)
        {
            DoubleModifier doubleModifier = (DoubleModifier)sourceFreezable;
            base.CloneCore(sourceFreezable);
            _controller = doubleModifier._controller;
            _delta = doubleModifier._delta;
    
        }
        //----------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        protected override void GetAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            DoubleModifier doubleModifier = (DoubleModifier)sourceFreezable;
            base.GetAsFrozenCore(sourceFreezable);
            _controller = doubleModifier._controller;
            _delta = doubleModifier._delta;

        }
        //----------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        protected override void GetCurrentValueAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            DoubleModifier doubleModifier = (DoubleModifier)sourceFreezable;
            base.GetCurrentValueAsFrozenCore(sourceFreezable);
            _controller = doubleModifier._controller;
            _delta = doubleModifier._delta;

        }
        /// <summary>
        /// 
        /// </summary>
        public new DoubleModifier GetAsFrozen()
        {
            return (DoubleModifier)base.GetAsFrozen();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new DoubleModifier();
        }
        
        /// <summary>
        /// 
        /// </summary>
        protected override double  GetCurrentValueCore (double defaultOriginValue, double baseValue, System.Windows.Media.Animation.AnimationClock clock )
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
        private double              _delta;
    }
}
