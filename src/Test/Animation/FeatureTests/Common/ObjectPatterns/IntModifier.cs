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
    public class                Int32Modifier              : System.Windows.Media.Animation.Int32AnimationBase
    {
        //----------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        public                  Int32Modifier ( ModifierController c, int d )
        {
            _controller = c;
            _delta = d;
        }
        /// <summary>
        /// 
        /// </summary>
        protected Int32Modifier()
        {
        }
        //----------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        protected override void CloneCore(System.Windows.Freezable sourceFreezable)
        {
            Int32Modifier int32Modifier = (Int32Modifier)sourceFreezable;
            base.CloneCore(sourceFreezable);
            _controller = int32Modifier._controller;
            _delta = int32Modifier._delta;
  
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void GetAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            Int32Modifier int32Modifier = (Int32Modifier)sourceFreezable;
            base.GetAsFrozenCore(sourceFreezable);
            _controller = int32Modifier._controller;
            _delta = int32Modifier._delta;

        }
        /// <summary>
        /// 
        /// </summary>
        protected override void GetCurrentValueAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            Int32Modifier int32Modifier = (Int32Modifier)sourceFreezable;
            base.GetCurrentValueAsFrozenCore(sourceFreezable);
            _controller = int32Modifier._controller;
            _delta = int32Modifier._delta;

        }
        /// <summary>
        /// 
        /// </summary>
        public new Int32Modifier GetAsFrozen()
        {
            return (Int32Modifier)base.GetAsFrozen();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new Int32Modifier();
        }
        
        /// <summary>
        /// 
        /// </summary>
        protected override int  GetCurrentValueCore (int defaultOriginValue, int baseValue, System.Windows.Media.Animation.AnimationClock clock )
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
        private int                 _delta;
    }
}
