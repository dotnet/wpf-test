// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// Specializes Property for vectors
    /// </summary>
    /// <typeparam name="TOwningType"></typeparam>
    public class VectorProperty<TOwningType> : ValueTypeProperty<TOwningType, Vector> where TOwningType : new()
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
        public VectorProperty(
            string name,
            Vector defaultValue,
            System.Func<bool, IEnumerable<Vector>> values,
            System.Func<TOwningType, Vector> getter,
            System.Action<TOwningType, Vector> setter)
            : base(name, defaultValue, values, () => new TOwningType(), getter, setter)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// compare two vectors
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        protected override bool CompareValues(Vector value1, Vector value2)
        {
            return Vector.Equals(value1, value2); 
        }

        #endregion
    }
}
