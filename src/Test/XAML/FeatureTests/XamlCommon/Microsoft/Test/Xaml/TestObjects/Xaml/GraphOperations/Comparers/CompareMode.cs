// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Common.TestObjects.Xaml.GraphOperations.Comparers
{
    /// <summary>
    /// Comparison mode 
    /// </summary>
    public enum CompareMode
    {
        /// <summary>
        /// Compare name and value
        /// </summary>
        PropertyNameAndValue,

        /// <summary>
        /// Compare just the name
        /// </summary>
        PropertyName,

        /// <summary>
        /// Compare the property value
        /// </summary>
        PropertyValue
    }
}
