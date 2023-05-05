// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Windows.Markup;

namespace Microsoft.Test.Xaml.Types.ClrClasses
{
    /// <summary>
    /// A custom class used for testing NameReferenceConverter.
    /// </summary>
    public class CustomType1
    {
        /// <summary>Gets or sets a custom property with the type converter.</summary>
        [TypeConverter(typeof(NameReferenceConverter))]
        public object Property1 { get; set; }

        /// <summary>Gets or sets a  custom property without the type converter.</summary>
        public object Property2 { get; set; }
    }
}
