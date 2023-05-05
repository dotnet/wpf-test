// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Markup;

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// Custom ME that takes a string and returns it via PV
    /// </summary>
    public class CustomME_StringValue : MarkupExtension
    {
        /// <summary>
        /// String to return
        /// </summary>
        public String StringValue
        {
            get;
            set;
        }

        /// <summary>
        /// Returns StringValue
        /// </summary>
        /// <param name="serviceProvider">service provider</param>
        /// <returns>String value</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return StringValue;
        }
    }
}
