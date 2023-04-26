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
    public class                SingleModifier              : System.Windows.Media.Animation.SingleAnimationBase
    {
        //----------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        public                  SingleModifier ( ModifierController c, float d )
        {
            _controller = c;
            _delta = d;
        }
        /// <summary>
        /// 
        /// </summary>
        protected SingleModifier()
        {
        }
       
        //----------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        protected override void CloneCore(System.Windows.Freezable sourceFreezable)
        {
            SingleModifier singleModifier = (SingleModifier)sourceFreezable;
            base.CloneCore(sourceFreezable);
            _controller = singleModifier._controller;
            _delta = singleModifier._delta;
      
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void GetAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            SingleModifier singleModifier = (SingleModifier)sourceFreezable;
            base.GetAsFrozenCore(sourceFreezable);
            _controller = singleModifier._controller;
            _delta = singleModifier._delta;

        }
        /// <summary>
        /// 
        /// </summary>
        protected override void GetCurrentValueAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            SingleModifier singleModifier = (SingleModifier)sourceFreezable;
            base.GetCurrentValueAsFrozenCore(sourceFreezable);
            _controller = singleModifier._controller;
            _delta = singleModifier._delta;

        }
        /// <summary>
        /// 
        /// </summary>
        public new SingleModifier GetAsFrozen()
        {
            return (SingleModifier)base.GetAsFrozen();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new SingleModifier();
        }
        
        /// <summary>
        /// 
        /// </summary>
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
