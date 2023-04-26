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
    public class                StringModifier              : System.Windows.Media.Animation.StringAnimationBase
    {
        //----------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        public                  StringModifier ( ModifierController c, string d )
        {
            _controller = c;
            _delta = d;
        }
        /// <summary>
        /// 
        /// </summary>
        protected StringModifier()
        {
        }
       
        //----------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        protected override void CloneCore(System.Windows.Freezable sourceFreezable)
        {
            StringModifier stringModifier = (StringModifier)sourceFreezable;
            base.CloneCore(sourceFreezable);
            _controller = stringModifier._controller;
            _delta = stringModifier._delta;
   
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void GetAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            StringModifier stringModifier = (StringModifier)sourceFreezable;
            base.GetAsFrozenCore(sourceFreezable);
            _controller = stringModifier._controller;
            _delta = stringModifier._delta;

        }
        /// <summary>
        /// 
        /// </summary>
        protected override void GetCurrentValueAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            StringModifier stringModifier = (StringModifier)sourceFreezable;
            base.GetCurrentValueAsFrozenCore(sourceFreezable);
            _controller = stringModifier._controller;
            _delta = stringModifier._delta;

        }
        /// <summary>
        /// 
        /// </summary>
        public new StringModifier GetAsFrozen()
        {
            return (StringModifier)base.GetAsFrozen();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new StringModifier();
        }
     
        
        /// <summary>
        /// 
        /// </summary>
        protected override string GetCurrentValueCore(string defaultOriginValue, string baseValue, System.Windows.Media.Animation.AnimationClock clock )
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
        private string              _delta;
    }
}
