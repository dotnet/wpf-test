// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Markup;
using System.Xaml;
using System.Windows.Media;

namespace Microsoft.Test.Xaml.Types.Attributes
{
    /// <summary>
    /// Custom type for the ambient property with TypeConverter scenario
    /// </summary>
    [TypeConverter(typeof(AmbientTCConverter))]
    public class AmbientTC
    {
        /// <summary>
        /// Brush property
        /// </summary>
        private Brush _brush;

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientTC"/> class.
        /// </summary>
        public AmbientTC()
        {
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text  value.</value>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets my brush.
        /// </summary>
        /// <value>My brush value.</value>
        [Ambient]
        public Brush MyBrush
        {
            get
            {
                return _brush;
            }

            set
            {
                // Checking value's type rather than calling IsValid() on the Brush's TypeConverter, which no
                // longer returns true, due to a clr fix which now rejects a value that is already a SolidColorBrush 
                // rather than a string.
                if (value.GetType() != typeof(SolidColorBrush))
                {
                    throw new ArgumentException();
                }

                _brush = value;
            }
        }
    }

    /// <summary>
    /// AmbientTCConverter class
    /// </summary>
    public class AmbientTCConverter : TypeConverter
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

            AmbientTC atc = new AmbientTC();
            IXamlSchemaContextProvider xscProvider = (IXamlSchemaContextProvider) typeDescriptorContext.GetService(typeof(IXamlSchemaContextProvider));
            IAmbientProvider iAmbient = (IAmbientProvider) typeDescriptorContext.GetService(typeof(IAmbientProvider));

            XamlType ambientSP = xscProvider.SchemaContext.GetXamlType(typeof(AmbientSingleProp));
            XamlMember textProp = ambientSP.GetMember("Text");
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
