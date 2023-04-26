// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//  Summary: this file creates sample modifiers that can be used to
//  set animated properties.
//  Current implementation has a discrete animation timed modifier.
//
// $Id:$ $Change:$
using System;
using System.IO;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Threading;


using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;


namespace Microsoft.Test.Animation
{


    /// <Summary>
    /// Discrete Animation object.
    /// Takes a timeline and set of objects to be alternated
    /// during that timeline.
    /// </Summary>
    [ContentProperty("Children")]
    public class DiscreteAnimation : AnimationTimeline, IAddChild
    {
        /// <Summary>
        /// internal collection of discrete objects
        /// </Summary>
        internal object[] _values;

        /// <summary>
        /// value indicating the number of times the animation has been queried
        /// for the current value of the animation - we use that to get the new 
        /// value from the _values array
        /// </summary>
        int _animationQueryCount = 0;

        /// <Summary>
        /// Constructor requires an object array.
        /// </Summary>
        public DiscreteAnimation( object[] values )
        {
            _values = values;
        }

        /// <Summary>
        /// Constructor requires an object array.
        /// </Summary>
        public DiscreteAnimation( )
        {
            _values = new object[ 0 ];
        }

        public override System.Type TargetPropertyType
        {
            get
            {
                if (_values != null && _values.Length > 0 )
                {
                    return _values[0].GetType();
                }
                else
                {
                    object myobject = new Object();
                    return myobject.GetType();
                }
            }
        }

        /// <summary>
        /// Implementation of <see cref="System.Windows.Freezable.CloneCore">Freezable.CloneCore</see>.
        /// </summary>
        protected override void CloneCore(Freezable sourceFreezable)
        {
            DiscreteAnimation discreteAnimation = (DiscreteAnimation)sourceFreezable;
            base.CloneCore(sourceFreezable);

            _values = new object[discreteAnimation._values.Length];
            discreteAnimation._values.CopyTo(_values, 0);
   
        }
        /// <summary>
        /// Implementation of <see cref="System.Windows.Freezable.GetAsFrozen">Freezable.GetAsFrozen</see>.
        /// </summary>
        public new DiscreteAnimation GetAsFrozen()
        {
            return (DiscreteAnimation)base.GetAsFrozen();

        }
        /// <summary>
        /// Implementation of <see cref="System.Windows.Freezable.GetAsFrozenCore">Freezable.GetAsFrozenCore</see>.
        /// </summary>
        protected override void GetAsFrozenCore(Freezable sourceFreezable)
        {
            DiscreteAnimation discreteAnimation = (DiscreteAnimation)sourceFreezable;

            base.GetAsFrozenCore(sourceFreezable);

            _values = new object[discreteAnimation._values.Length];
            discreteAnimation._values.CopyTo(_values, 0);
        }
        /// <summary>
        /// Implementation of <see cref="System.Windows.Freezable.GetCurrentValueAsFrozenCore">Freezable.GetAsFrozenCore</see>.
        /// </summary>
        protected override void GetCurrentValueAsFrozenCore(Freezable sourceFreezable)
        {
            DiscreteAnimation discreteAnimation = (DiscreteAnimation)sourceFreezable;

            base.GetCurrentValueAsFrozenCore(sourceFreezable);

            _values = new object[discreteAnimation._values.Length];
            discreteAnimation._values.CopyTo(_values, 0);
        }
 
        /// <summary>
        /// Implementation of <see cref="System.Windows.Freezable.CreateInstanceCore">Freezable.CreateInstanceCore</see>.
        /// </summary>
        protected override Freezable CreateInstanceCore()
        {
            return new DiscreteAnimation();

        }
        /// <summary>
        /// Calculates the value of the animation at the current time.
        /// </summary>
        /// <param name="baseValue">base value</param>
        /// <returns>new value</returns>
        public override object GetCurrentValue(object defaultOriginValue, object baseValue, AnimationClock animClock)
        {
            // NOTE: There is a potential issue here that we need to keep in
            // mind while testing. Be very careful about where you sample the animation values. 
            // If the default for this property is the same value as _value[0], then you could have 
            // something like:
            //      read value -> property == B
            //      apply animation
            //      read value -> property == B
            // if you then compared the starting point to the first sampled value, and found that
            // they were identical, then the case might fail because we assumed that *some* sort of 
            // change would have taken place. 
            // This is not just a hypothetical possibility
			
            // return the next discrete value
            int index = _animationQueryCount++;
            index %= _values.Length;

            // notify any listeners that the property has been animated
            // we do this because simply querying the property via GetValue() would lead right
            // back to a call to GetCurrentValueCore(), where we would supply a different value. 
            PropertyAnimatedEventArgs args = new PropertyAnimatedEventArgs(_animationQueryCount - 1, _values[index]);
            OnPropertyAnimated(args);
            
            return _values[index];

        }

        /// <summary>
        /// delegate signature for listeners to implement
        /// </summary>
        public delegate void PropertyAnimatedEventHandler(object sender, PropertyAnimatedEventArgs args);


        /// <summary>
        /// event for notifying a listener that the discrete property has changed
        /// </summary>
        public event PropertyAnimatedEventHandler PropertyAnimated;


        /// <summary>
        /// function for firing the PropertyAnimated event
        /// </summary>
        protected virtual void OnPropertyAnimated(PropertyAnimatedEventArgs args)
        {
            if (PropertyAnimated != null)
            {
                PropertyAnimated(this, args);
            }
        }

        private Collection<object> _children;

        public Collection<object> Children
        {
            get {
                  if (_children == null)
                      _children = new Collection<object>();
                  return _children;
                }
        }    

        #region IAddChild
        /// <summary>
        /// Implementation of <see cref="System.Windows.Markup.IAddChild.AddChild">IAddChild.AddChild</see>.
        /// Adds a Modifier to this AnimationCollection from Markup.
        /// </summary>
        public void AddChild(object o)
        {
            Children.Add(o);
        }
        
        /// <summary>
        /// Implementation of <see cref="System.Windows.Markup.IAddChild.AddText">IAddChild.AddText</see>.
        /// This is not implemented on this class.
        /// </summary>
        public void AddText(string s)
        {
            // throw new NotImplementedException();
        }
        #endregion

    }


    /// <summary>
    /// event arguments for any listeners of the PropertyAnimated event
    /// </summary>
    public class PropertyAnimatedEventArgs : EventArgs
    {
        public readonly int TickCount;          // # of times GetCurrentValueCore has been called
        public readonly object PropertyValue;   // value of the property being returned to the caller

        /// <summary>
        /// constructor
        /// </summary>
        public PropertyAnimatedEventArgs(int tickCount, object propertyValue)
        {
            TickCount = tickCount;
            PropertyValue = propertyValue;
        }
    }



}




