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
    public class                Int64Modifier              : System.Windows.Media.Animation.Int64AnimationBase
    {
        //----------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        public                  Int64Modifier ( ModifierController c, long d )
        {
            _controller = c;
            _delta = d;
        }
        /// <summary>
        /// 
        /// </summary>
        protected Int64Modifier()
        {
        }
       
        //----------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        protected override void CloneCore(System.Windows.Freezable sourceFreezable)
        {
            Int64Modifier int64Modifier = (Int64Modifier)sourceFreezable;
            base.CloneCore(sourceFreezable);
            _controller = int64Modifier._controller;
            _delta = int64Modifier._delta;
      
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void GetAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            Int64Modifier int64Modifier = (Int64Modifier)sourceFreezable;
            base.GetAsFrozenCore(sourceFreezable);
            _controller = int64Modifier._controller;
            _delta = int64Modifier._delta;

        }
        /// <summary>
        /// 
        /// </summary>
        protected override void GetCurrentValueAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            Int64Modifier int64Modifier = (Int64Modifier)sourceFreezable;
            base.GetCurrentValueAsFrozenCore(sourceFreezable);
            _controller = int64Modifier._controller;
            _delta = int64Modifier._delta;

        }
        /// <summary>
        /// 
        /// </summary>
        public new Int64Modifier GetAsFrozen()
        {
            return (Int64Modifier)base.GetAsFrozen();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new Int64Modifier();
        }
     
        
        /// <summary>
        /// 
        /// </summary>
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
