// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// class for testing the behavior of a value type get/set property
    /// </summary>
    /// <remarks>
    /// This only works for value types as "owners" (the class that has the property on it); 
    /// the semantics of reference types don't work well for this model.
    /// </remarks>
    /// <typeparam name="TOwningType">The type of the object that has the property on it.</typeparam>
    /// <typeparam name="TPropertyType">The return type of the property. Must be a value type.</typeparam>
    public class ValueTypeProperty<TOwningType, TPropertyType> : Property<TOwningType, TPropertyType>
        where TPropertyType : struct
    {
        #region Private Fields

        private readonly TPropertyType _defaultValue;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="defaultValue">The default value of the property.</param>
        /// <param name="values">A function for enumerating valid and invalid values.</param>
        /// <param name="constructor">A function for constructing the owning type.</param>
        /// <param name="getter">A function to get the property value.</param>
        /// <param name="setter">An action that sets the property value.</param>
        public ValueTypeProperty(
            string name,
            TPropertyType defaultValue,
            System.Func<bool, IEnumerable<TPropertyType>> values,
            System.Func<TOwningType> constructor,
            System.Func<TOwningType, TPropertyType> getter,
            System.Action<TOwningType, TPropertyType> setter)
            : base(name, values, constructor, getter, setter)
        {
            this._defaultValue = defaultValue;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks that the property has the expected default value.
        /// </summary>
        public override void CheckDefault()
        {
            TPropertyType ownerValue = Getter(Constructor());
            bool matches = CompareValues(this._defaultValue, ownerValue);
            Utils.Assert(matches, string.Format("Property {0} has a default value of {1}. Expected: {2}", Name, ownerValue, this._defaultValue));
        }

        #endregion
    }
}
