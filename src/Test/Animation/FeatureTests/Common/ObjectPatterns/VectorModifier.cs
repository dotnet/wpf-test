// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

//------------------------------------------------------------------

namespace                       Microsoft.Test.Animation.ObjectPatterns
{

    /// <summary>
    /// 
    /// </summary>
    public class                VectorModifier              : System.Windows.Media.Animation.VectorAnimationBase
    {
        //----------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        public                  VectorModifier ( ModifierController c, double x, double y )
        {
            _controller = c;
            _x = x;
            _y = y;
        }

        /// <summary>
        /// 
        /// </summary>
        protected VectorModifier()
        {
        }
       

        //----------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        protected override void CloneCore(System.Windows.Freezable sourceFreezable)
        {
            VectorModifier vectorModifier = (VectorModifier)sourceFreezable;
            base.CloneCore(sourceFreezable);
            _controller = vectorModifier._controller;
            _x = vectorModifier._x;
            _y = vectorModifier._y;
    
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void GetAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            VectorModifier vectorModifier = (VectorModifier)sourceFreezable;
            base.GetAsFrozenCore(sourceFreezable);
            _controller = vectorModifier._controller;
            _x = vectorModifier._x;
            _y = vectorModifier._y;

        }
        /// <summary>
        /// 
        /// </summary>
        protected override void GetCurrentValueAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            VectorModifier vectorModifier = (VectorModifier)sourceFreezable;
            base.GetCurrentValueAsFrozenCore(sourceFreezable);
            _controller = vectorModifier._controller;
            _x = vectorModifier._x;
            _y = vectorModifier._y;

        }
        /// <summary>
        /// 
        /// </summary>
        public new VectorModifier GetAsFrozen()
        {
            return (VectorModifier)base.GetAsFrozen();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new VectorModifier();
        }
     
  
        //----------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        protected override System.Windows.Vector
                                GetCurrentValueCore(System.Windows.Vector defaultOriginValue, System.Windows.Vector baseValue, System.Windows.Media.Animation.AnimationClock clock)
        {
            if (!_controller.UsesBaseValue)
            {
                return new System.Windows.Vector ( _x, _y );
            }
            else
            {
                return new System.Windows.Vector ( baseValue.X + _x, baseValue.Y + _y );
            }
        }

        //----------------------------------------------------------

        private ModifierController  _controller;
        private double              _x;
        private double              _y;
    }
}
