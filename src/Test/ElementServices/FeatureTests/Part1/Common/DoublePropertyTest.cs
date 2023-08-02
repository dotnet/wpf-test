// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// Specializes Property for double values.
    /// </summary>
    /// <typeparam name="TOwningType"></typeparam>
    public class DoubleProperty<TOwningType> : ValueTypeProperty<TOwningType, double> where TOwningType : new()
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="defaultValue">The default value of the property.</param>
        /// <param name="values">A function for enumerating valid and invalid values.</param>
        /// <param name="getter">A function to get the property value.</param>
        /// <param name="setter">An action that sets the property value.</param>
        public DoubleProperty(
            string name,
            double defaultValue,
            System.Func<bool, IEnumerable<double>> values,
            System.Func<TOwningType, double> getter,
            System.Action<TOwningType, double> setter)
            : base(name, defaultValue, values, () => new TOwningType(), getter, setter)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// double.NaN doesn't evaluate as equal to itself
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        protected override bool CompareValues(double value1, double value2)
        {
            return (value1.Equals(value2)); 
        }

        #endregion
    }
}
