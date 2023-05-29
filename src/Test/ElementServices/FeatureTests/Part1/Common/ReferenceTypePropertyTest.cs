// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// Utility class for testing the behavior of a get/set property 
    /// that's a reference type.
    /// </summary>
    /// <typeparam name="TOwningType">The type of the object that has the property on it.</typeparam>
    /// <typeparam name="TPropertyType">The return type of the property. Must be a reference type.</typeparam>
    public class ReferenceTypeProperty<TOwningType, TPropertyType> : Property<TOwningType, TPropertyType>
        where TPropertyType : class, new()
    {
        #region Private fields

        private readonly bool _isDefaultValueNull;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="isDefaultValueNull">Whether the default value of the property is null.</param>
        /// <param name="values">A function for enumerating valid and invalid values.</param>
        /// <param name="constructor">A function for constructing the owning type.</param>
        /// <param name="getter">A function to get the property value.</param>
        /// <param name="setter">An action that sets the property value.</param>
        public ReferenceTypeProperty(
            string name,
            bool isDefaultValueNull,
            System.Func<bool, IEnumerable<TPropertyType>> values,
            System.Func<TOwningType> constructor,
            System.Func<TOwningType, TPropertyType> getter,
            System.Action<TOwningType, TPropertyType> setter)
            : base(name, values, constructor, getter, setter)
        {
            this._isDefaultValueNull = isDefaultValueNull;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks that the property has the expected default value.
        /// </summary>
        public override void CheckDefault()
        {
            TPropertyType ownerValue = Getter(Constructor());
            if (this._isDefaultValueNull)
            {
                Utils.AssertEqual(
                    ownerValue == null,
                    "Property {0} has a non-null default value. Expected null.",
                    Name);
            }
            else
            {
                Utils.AssertEqual(
                    ownerValue != null,
                    "Property {0} has a null default value. Expected non-null.",
                    Name);
            }
        }

        /// <summary>
        /// Checks two values to see whether they're the same.
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        protected override bool CompareValues(TPropertyType value1, TPropertyType value2)
        {
            return object.ReferenceEquals(value1, value2);
        }

        #endregion
    }    
}
