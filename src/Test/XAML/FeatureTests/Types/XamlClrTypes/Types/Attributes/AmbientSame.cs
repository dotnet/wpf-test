// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Markup;
using System.Xaml;

namespace Microsoft.Test.Xaml.Types.Attributes
{
    /// <summary>
    /// Type containing ambient properties 
    /// </summary>
    [UsableDuringInitialization(true)]
    public class TypeWithAmbientProp : IQueryAmbient
    {
        /// <summary>
        /// Gets or sets string text propety which is ambient
        /// </summary>
        [Ambient]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets ambient property that has a type converter
        /// </summary>
        [Ambient]
        [TypeConverter(typeof(AmbientTC2Converter))]
        public AmbientTC2 AmbientTC { get; set; }

        /// <summary>
        /// IQueryAmbient implementation
        /// </summary>
        /// <param name="propertyName">name of the property</param>
        /// <returns>true if available</returns>
        public bool IsAmbientPropertyAvailable(string propertyName)
        {
            return true;
        }
    }

    /// <summary>
    /// Custom type for the ambient property with TypeConverter scenario
    /// </summary>
    [TypeConverter(typeof(AmbientTC2Converter))]
    public class AmbientTC2
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientTC2"/> class.
        /// </summary>
        public AmbientTC2()
        {
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text  value.</value>
        public string Text { get; set; }
    }

    /// <summary>
    /// AmbientTCConverter class - This type tries to do an ambient property lookup 
    /// for the same typeconverted property.
    /// </summary>
    public class AmbientTC2Converter : TypeConverter
    {
        /// <summary>
        /// Determines whether this instance [can convert from] the specified type descriptor context.
        /// </summary>
        /// <param name="typeDescriptorContext">The type descriptor context.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <returns>
        /// <c>true</c> if this instance [can convert from] the specified type descriptor context; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type sourceType)
        {
            // XAML only converts from String to targettype.
            return sourceType == typeof(string);
        }

        /// <summary>
        /// Determines whether this instance [can convert to] the specified type descriptor context.
        /// </summary>
        /// <param name="typeDescriptorContext">The type descriptor context.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <returns>
        /// <c>true</c> if this instance [can convert to] the specified type descriptor context; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext typeDescriptorContext, Type destinationType)
        {
            // XAML only converts targettype to string.
            return destinationType == typeof(string);
        }

        /// <summary>
        /// Converts from.
        /// </summary>
        /// <param name="typeDescriptorContext">The type descriptor context.</param>
        /// <param name="cultureInfo">The culture info.</param>
        /// <param name="value">The value.</param>
        /// <returns>object value</returns>
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value)
        {
            string s = value as string;
            if (null == s)
            {
                throw new ArgumentException("XAML can only convert from Strings");
            }

            AmbientTC2 atc = new AmbientTC2();
            IXamlSchemaContextProvider xscProvider = (IXamlSchemaContextProvider)typeDescriptorContext.GetService(typeof(IXamlSchemaContextProvider));
            IAmbientProvider iAmbient = (IAmbientProvider)typeDescriptorContext.GetService(typeof(IAmbientProvider));

            XamlType ambientSP = xscProvider.SchemaContext.GetXamlType(typeof(TypeWithAmbientProp));
            XamlMember textProp = ambientSP.GetMember("AmbientTC");
            AmbientPropertyValue ambientPropVal = iAmbient.GetFirstAmbientValue(null, textProp);
            if (ambientPropVal == null)
            {
                atc.Text = s;
            }
            else
            {
                atc.Text = s + "-" + (ambientPropVal.Value as string);
            }

            return atc;
        }
    }
}
