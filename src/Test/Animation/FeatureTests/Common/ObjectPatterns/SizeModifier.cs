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
    public class                SizeModifier              : System.Windows.Media.Animation.SizeAnimationBase
    {
        //----------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        public                  SizeModifier ( ModifierController c, double dx, double dy )
        {
            _controller = c;
            _deltaX = dx;
            _deltaY = dy;
        }
   
        /// <summary>
        /// 
        /// </summary>
        protected SizeModifier()
        {
        }
       
        //----------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        protected override void CloneCore(System.Windows.Freezable sourceFreezable)
        {
            SizeModifier sizeModifier = (SizeModifier)sourceFreezable;
            base.CloneCore(sourceFreezable);
            _controller = sizeModifier._controller;
            _deltaX = sizeModifier._deltaX;
            _deltaY = sizeModifier._deltaY;
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void GetAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            SizeModifier sizeModifier = (SizeModifier)sourceFreezable;
            base.GetAsFrozenCore(sourceFreezable);
            _controller = sizeModifier._controller;
            _deltaX = sizeModifier._deltaX;
            _deltaY = sizeModifier._deltaY;
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void GetCurrentValueAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            SizeModifier sizeModifier = (SizeModifier)sourceFreezable;
            base.GetCurrentValueAsFrozenCore(sourceFreezable);
            _controller = sizeModifier._controller;
            _deltaX = sizeModifier._deltaX;
            _deltaY = sizeModifier._deltaY;
        }
        /// <summary>
        /// 
        /// </summary>
        public new SizeModifier GetAsFrozen()
        {
            return (SizeModifier)base.GetAsFrozen();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new SizeModifier();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override System.Windows.Size
                                GetCurrentValueCore ( System.Windows.Size defaultOriginValue, System.Windows.Size baseValue, System.Windows.Media.Animation.AnimationClock clock )
        {
            if ( !_controller.UsesBaseValue )
            {
                return new System.Windows.Size ( _deltaX, _deltaY );
            }
            else
            {
                return new System.Windows.Size ( baseValue.Width + _deltaX, baseValue.Height + _deltaY );
            }
        }

  
        //----------------------------------------------------------

        private ModifierController  _controller;
        private double              _deltaX;
        private double              _deltaY;
    }
}
