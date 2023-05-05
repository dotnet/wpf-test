// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;
using System.Xaml;

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// Custom IXamlSchemaContextProviderExtension
    /// </summary>
    public class Custom_IXamlSchemaContextProviderExtension : MarkupExtension
    {
        /// <summary>
        /// Provides the value.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>object value</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IXamlSchemaContextProvider iXamlSchemaContextProvider = (IXamlSchemaContextProvider)serviceProvider.GetService(typeof(IXamlSchemaContextProvider));
            Custom_IXSCPPropertyObject retObj = new Custom_IXSCPPropertyObject();
            retObj.SchemaContext = iXamlSchemaContextProvider.SchemaContext;
            return retObj;
        }
    }
}
