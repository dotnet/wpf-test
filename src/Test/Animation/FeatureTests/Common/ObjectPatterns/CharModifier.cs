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
    public class                CharModifier              : System.Windows.Media.Animation.CharAnimationBase
    {
        //----------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        public                  CharModifier ( ModifierController c, char d )
        {
            _controller = c;
            _delta = d;
        }
        /// <summary>
        /// 
        /// </summary>
        protected CharModifier()
        {
        }
        //----------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        protected override void CloneCore(System.Windows.Freezable sourceFreezable)
        {
            CharModifier charModifier = (CharModifier)sourceFreezable;
            base.CloneCore(sourceFreezable);
            _controller = charModifier._controller;
            _delta = charModifier._delta;
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void GetAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            CharModifier charModifier = (CharModifier)sourceFreezable;
            base.GetAsFrozenCore(sourceFreezable);
            _controller = charModifier._controller;
            _delta = charModifier._delta;
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void GetCurrentValueAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            CharModifier charModifier = (CharModifier)sourceFreezable;
            base.GetCurrentValueAsFrozenCore(sourceFreezable);
            _controller = charModifier._controller;
            _delta = charModifier._delta;
        }
        /// <summary>
        /// 
        /// </summary>
        public new CharModifier GetAsFrozen()
        {
            return (CharModifier)base.GetAsFrozen();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new CharModifier();
        }
        
        /// <summary>
        /// 
        /// </summary>
        protected override char GetCurrentValueCore(char defaultOriginValue, char baseValue, System.Windows.Media.Animation.AnimationClock clock )
        {
            if (!_controller.UsesBaseValue)
            {
                return _delta;
            }
            else
            {
                return (char)('a' + 'b');
            }
                
        }

        //----------------------------------------------------------

        private ModifierController  _controller;
        private char                _delta;
    }
}
