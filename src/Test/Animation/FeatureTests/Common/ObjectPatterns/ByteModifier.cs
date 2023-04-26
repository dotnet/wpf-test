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
    public class                ByteModifier              : System.Windows.Media.Animation.ByteAnimationBase
    {
        //----------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        public                  ByteModifier ( ModifierController c, System.Byte d )
        {
            _controller = c;
            _delta = d;
        }
        /// <summary>
        /// 
        /// </summary>
        protected ByteModifier()
        {
        }
        //----------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        protected override void CloneCore(System.Windows.Freezable sourceFreezable)
        {
            ByteModifier byteModifier = (ByteModifier)sourceFreezable;
            base.CloneCore(sourceFreezable);
            _controller = byteModifier._controller;
            _delta = byteModifier._delta;
        }
        //----------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        protected override void GetAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            ByteModifier byteModifier = (ByteModifier)sourceFreezable;
            base.GetAsFrozenCore(sourceFreezable);
            _controller = byteModifier._controller;
            _delta = byteModifier._delta;
        }
        //----------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        protected override void GetCurrentValueAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            ByteModifier byteModifier = (ByteModifier)sourceFreezable;
            base.GetCurrentValueAsFrozenCore(sourceFreezable);
            _controller = byteModifier._controller;
            _delta = byteModifier._delta;
        }
        /// <summary>
        /// 
        /// </summary>
        public new ByteModifier GetAsFrozen()
        {
            return (ByteModifier)base.GetAsFrozen();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new ByteModifier();
        }
        
        /// <summary>
        /// 
        /// </summary>
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
