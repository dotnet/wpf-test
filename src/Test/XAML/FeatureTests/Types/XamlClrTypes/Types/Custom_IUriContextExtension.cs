// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// Custom IUriContextExtension
    /// </summary>
    public class Custom_IUriContextExtension : MarkupExtension
    {
        /// <summary>
        /// Provides the value.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>object value </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IUriContext iUriCxt = (IUriContext) serviceProvider.GetService(typeof(IUriContext));
            if (iUriCxt != null)
            {
                return iUriCxt.BaseUri;
            }

            return null;
        }
    }
}
