// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ComponentModel.Design.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Tests
{
    /// <summary>
    ///
    /// </summary>
    public class CycleAnimation : AnimationTimeline
    {
        #region Constructors

        /// <summary>
        /// Creates a new CycleAnimation.
        /// </summary>
        public CycleAnimation()
            : base()
        {
        }
        
        /// <summary>
        /// Creates a new CycleAnimation.
        /// </summary>
        public CycleAnimation(Freezable[] changeables)
            : base()
        {
            _children = (Freezable[]) changeables.Clone();
            UpdateChildren();
        }
        
        #endregion
        
        #region Freezable
        
        /// <summary>
        /// Creates a copy of this CycleAnimation
        /// </summary>
        /// <returns>The copy</returns>
        public new CycleAnimation Clone()
        {
            return (CycleAnimation)base.Clone();
        }
        
        /// <summary>
        /// Implementation of <see cref="System.Windows.Freezable.CreateInstanceCore">Freezable.CreateInstanceCore</see>.
        /// </summary>
        /// <returns>The new Freezable.</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new CycleAnimation();
        }

        /// <summary>
        /// Implementation of <see cref="System.Windows.Freezable.CloneCore(Freezable)">Freezable.CopyCore</see>.
        /// </summary>
        protected override void CloneCore(Freezable sourceFreezable)
        {
            CycleAnimation sourceCycle = (CycleAnimation) sourceFreezable;
            base.CloneCore(sourceFreezable);

            if (sourceCycle._children != null)
            {
                _children = new Freezable[sourceCycle._children.Length];

                for (int i = 0; i < sourceCycle._children.Length; i++)
                {
                    _children[i] = sourceCycle._children[i].Clone();
                }
            }

            UpdateChildren();
        }

        /// <summary>
        /// Implementation of <see cref="System.Windows.Freezable.CloneCurrentValueCore(Freezable)">Freezable.CloneCurrentValueCore</see>.
        /// </summary>
        protected override void CloneCurrentValueCore(Freezable sourceFreezable)
        {
            CycleAnimation sourceCycle = (CycleAnimation) sourceFreezable;
            base.CloneCurrentValueCore(sourceFreezable);

            if (sourceCycle._children != null)
            {
                _children = new Freezable[sourceCycle._children.Length];

                for (int i = 0; i < sourceCycle._children.Length; i++)
                {
                    _children[i] = sourceCycle._children[i].CloneCurrentValue();
                }
            }

            UpdateChildren();
        }

        /// <summary>
        /// Implementation of <see cref="System.Windows.Freezable.CloneCurrentValueCore(Freezable)">Freezable.CloneCurrentValueCore</see>.
        /// </summary>
        protected override void GetAsFrozenCore(Freezable sourceFreezable)
        {
            CycleAnimation sourceCycle = (CycleAnimation)sourceFreezable;
            base.GetAsFrozenCore(sourceFreezable);

            if (sourceCycle._children != null)
            {
                _children = new Freezable[sourceCycle._children.Length];

                for (int i = 0; i < sourceCycle._children.Length; i++)
                {
                    _children[i] = sourceCycle._children[i].GetAsFrozen();
                }
            }

            UpdateChildren();
        }
        

        /// <summary>
        /// Implementation of <see cref="System.Windows.Freezable.CloneCurrentValueCore(Freezable)">Freezable.CloneCurrentValueCore</see>.
        /// </summary>
        protected override void GetCurrentValueAsFrozenCore(Freezable sourceFreezable)
        {
            CycleAnimation sourceCycle = (CycleAnimation)sourceFreezable;
            base.GetCurrentValueAsFrozenCore(sourceFreezable);

            if (sourceCycle._children != null)
            {
                _children = new Freezable[sourceCycle._children.Length];

                for (int i = 0; i < sourceCycle._children.Length; i++)
                {
                    _children[i] = sourceCycle._children[i].GetCurrentValueAsFrozen();
                }
            }

            UpdateChildren();
        }

        #endregion
        
        #region AnimationTimeline
        
        /// <summary>
        /// Returns the animation's current value.
        /// </summary>
        public override sealed object GetCurrentValue(object snapshotValue, object baseValue, AnimationClock animationClock)
        {
            if (animationClock == null)
            {
                throw new ArgumentNullException("animationClock");
            }
            
            if (animationClock.CurrentState == ClockState.Stopped)
            {
                return baseValue;
            }

            // We want progress in [0,1) not [0,1] so we'll multiply
            // by 1 - 1/Infinity and pretend nothing happened :)
            int index = (int) ((animationClock.CurrentProgress * 0.9999999) * (_children.Length));
            
            return _children[index];
        }
        
        /// <summary>
        /// Returns true if the animation can animate a value of a given type.
        /// </summary>
        public override sealed Type TargetPropertyType
        {
            get
            {
                ReadPreamble();
                
                return _type;
            }
        }
        
        #endregion

        #region Methods

        
        /// <summary>
        /// Returns the animation's current value.
        /// </summary>
        public Freezable CloneCurrentValue(Freezable snapshotValue, Freezable baseValue, AnimationClock animationClock)
        {
            return (Freezable)CloneCurrentValue(snapshotValue, baseValue, animationClock);
        }

        #endregion

        #region Private methods

        Freezable[] _children;
        Type _type = typeof(object);
        
        private void UpdateChildren()
        {
            // Handle the 0th or 1st child.
            if (_children.Length < 1)
            {
                _type = typeof(object);
            }
            else
            {
                _type = _children[0].GetType();
            }

            // Handle the rest of the children.
            for (int i = 1; i < _children.Length; ++i)
            {
                Type nextType = _children[i].GetType();
                while (!_type.IsAssignableFrom(nextType))
                {
                    _type = _type.BaseType;
                }
            }
        }
        
        #endregion Private methods
    }
}
