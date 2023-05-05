// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Common.TestObjects.Xaml.GraphCore
{
    using System.Collections.Generic;
    using Microsoft.Test.Xaml.Common;

    /// <summary>
    /// Test dependency object interface
    /// </summary>
    public interface ITestDependencyObject
    {
        /// <summary>
        /// Gets the properties collection
        /// </summary>
        IDictionary<string, object> Properties { get; }

        /// <summary>
        /// Get a property
        /// </summary>
        /// <param name="dependencyProperty">property information</param>
        /// <returns>property value</returns>
        object GetValue(XName dependencyProperty);

        /// <summary>
        /// Set a property
        /// </summary>
        /// <param name="dependencyProperty">property information</param>
        /// <param name="value">property value</param>
        void SetValue(XName dependencyProperty, object value);
    }
}
