// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;
using System.Xaml;
using Microsoft.Test.Xaml.Types.Attributes;

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// Custom MarkupExtension for testing IAmbientProvider
    /// </summary>
    public class CustomME_AmbientValue : MarkupExtension
    {
        /// <summary>
        /// Property Name
        /// </summary>
        private readonly string _propName;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomME_AmbientValue"/> class.
        /// </summary>
        /// <param name="propName">Name of the prop.</param>
        public CustomME_AmbientValue(string propName)
        {
            this._propName = propName;
        }

        /// <summary>
        /// Provides the value.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>object value</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (String.IsNullOrEmpty(_propName))
            {
                return null;
            }

            IXamlSchemaContextProvider xscProvider;
            IAmbientProvider ambient;
            xscProvider = (IXamlSchemaContextProvider)serviceProvider.GetService(typeof(IXamlSchemaContextProvider));
            ambient = (IAmbientProvider) serviceProvider.GetService(typeof(IAmbientProvider));
            XamlType ambSingleType = xscProvider.SchemaContext.GetXamlType(typeof(AmbientSingleProp));
            XamlMember textProperty = ambSingleType.GetMember(_propName);

            IEnumerable<AmbientPropertyValue> ambientEnumerable = ambient.GetAllAmbientValues(null, textProperty);
            if (ambientEnumerable == null)
            {
                return null;
            }

            foreach (AmbientPropertyValue ambientValue in ambientEnumerable)
            {
                if (textProperty.Equals(ambientValue.RetrievedProperty))
                {
                    return ambientValue.Value;
                }
            }

            return null;
        }
    }
}
