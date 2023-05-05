// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Markup;
using System.Xaml;

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// Custom_Type_InternalProp class
    /// </summary>
    [RuntimeNameProperty("Name")]
    [ContentProperty("Content")]
    public class Custom_Type_InternalProp
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name prop.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public object Content { get; set; }

        /// <summary>
        /// Gets or sets the internal string.
        /// </summary>
        /// <value>The internal string.</value>
        internal string InternalString { get; set; }
    }

    /// <summary>
    /// Custom_Type_InternalEvent class
    /// </summary>
    [RuntimeNameProperty("Name")]
    [ContentProperty("Content")]
    internal class Custom_Type_InternalEvent
    {
        /// <summary>
        /// Sets the internal event
        /// </summary>
        /// <value>The internal event.</value>
        internal event EventHandler InternalEvent;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name prop.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public object Content { get; set; }

        /// <summary>
        /// Check if an event is set
        /// </summary>
        /// <returns>event added</returns>
        public bool EventAdded()
        {
            return InternalEvent != null;
        }

        /// <summary>
        /// The private event handler
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="args">Args of the event</param>
        private void PrivateEventHandler(object sender, EventArgs args)
        {
        }
    }

    /// <summary>
    /// Custom InternalBase
    /// </summary>
    internal class Custom_InternalBase
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name prop.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public object Content { get; set; }

        /// <summary>
        /// Gets or sets the internal string.
        /// </summary>
        /// <value>The internal string.</value>
        internal string InternalString { get; set; }
    }

    /// <summary>
    /// Custom Internal class
    /// </summary>
    [RuntimeNameProperty("Name")]
    [ContentProperty("Content")]
    internal class Custom_Internal : Custom_InternalBase
    {
    }

    /// <summary>
    /// Custom Internal InternalCtor class
    /// </summary>
    [RuntimeNameProperty("Name")]
    [ContentProperty("Content")]
    internal class Custom_Internal_InternalCtor : Custom_InternalBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_Internal_InternalCtor"/> class.
        /// </summary>
        internal Custom_Internal_InternalCtor()
        {
        }
    }

    /// <summary>
    /// Custom Internal InternalName class
    /// </summary>
    [RuntimeNameProperty("Name")]
    [ContentProperty("Content")]
    internal class Custom_Internal_InternalName : Custom_InternalBase
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name prop.</value>
        internal new string Name { get; set; }
    }

    /// <summary>
    /// Custom_Internal_InternalContent class
    /// </summary>
    [RuntimeNameProperty("Name")]
    [ContentProperty("Content")]
    internal class Custom_Internal_InternalContent : Custom_InternalBase
    {
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        internal new object Content { get; set; }
    }

    /// <summary>
    /// Custom_Internal_InternalAmbient class
    /// </summary>
    [RuntimeNameProperty("Name")]
    [ContentProperty("Content")] 
    internal class Custom_Internal_InternalAmbient : Custom_InternalBase
    {
        /// <summary>
        /// Gets or sets the internal ambient int.
        /// </summary>
        /// <value>The internal ambient int.</value>
        [Ambient]
        internal int InternalAmbientInt { get; set; }
    }

    /// <summary>
    /// Custom_Internal_Collections class
    /// </summary>
    [RuntimeNameProperty("Name")]
    [ContentProperty("Content")]
    internal class Custom_Internal_Collections : Custom_InternalBase
    {
        /// <summary>
        /// List of RO objects
        /// </summary>
        private readonly List<object> _listRO;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_Internal_Collections"/> class.
        /// </summary>
        public Custom_Internal_Collections()
        {
            _listRO = new List<object>();
            ListRW = new List<object>();
        }

        /// <summary>
        /// Gets the list RO.
        /// </summary>
        /// <value>The list RO.</value>
        internal List<object> ListRO
        {
            get
            {
                return _listRO;
            }
        }

        /// <summary>
        /// Gets or sets the list RW.
        /// </summary>
        /// <value>The list RW.</value>
        internal List<object> ListRW { get; set; }
    }

    /// <summary>
    /// Custom_Internal_InternalTypeProp class
    /// </summary>
    [RuntimeNameProperty("Name")]
    [ContentProperty("Content")]
    internal class Custom_Internal_InternalTypeProp : Custom_InternalBase
    {
        /// <summary>
        /// Gets or sets the custom_ internal property.
        /// </summary>
        /// <value>The custom_ internal property.</value>
        [TypeConverter(typeof(Custom_InternalConverter))]
        internal Custom_Internal Custom_InternalProperty { get; set; }
    }

    /// <summary>
    /// Custom_InternalExtension class
    /// </summary>
    internal class Custom_InternalExtension : MarkupExtension
    {
        /// <summary>
        /// string text value
        /// </summary>
        private readonly string _text;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_InternalExtension"/> class.
        /// </summary>
        public Custom_InternalExtension()
        {
            _text = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_InternalExtension"/> class.
        /// </summary>
        /// <param name="text">The text value.</param>
        public Custom_InternalExtension(string text)
        {
            this._text = text;
        }

        /// <summary>
        /// Provides the value.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>returns text or text# if AmbientInt is found</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IXamlSchemaContextProvider xscProvider = (IXamlSchemaContextProvider) serviceProvider.GetService(typeof(IXamlSchemaContextProvider));
            IAmbientProvider ambient = (IAmbientProvider) serviceProvider.GetService(typeof(IAmbientProvider));

            XamlType intAmbientType = xscProvider.SchemaContext.GetXamlType(typeof(Custom_Internal_InternalAmbient));
            XamlMember ambProp = intAmbientType.GetMember("InternalAmbientInt");
            IEnumerable<AmbientPropertyValue> ambientEnumerable = ambient.GetAllAmbientValues(null, ambProp);

            bool ambientPropsReturned = false;

            if (ambientEnumerable != null)
            {
                int count = 0;
                foreach (AmbientPropertyValue ambientValue in ambientEnumerable)
                {
                    count++;
                }

                ambientPropsReturned = count > 0;
            }
            else
            {
                ambientPropsReturned = true;
            }

            if (!ambientPropsReturned)
            {
                return _text;
            }
            else
            {
                foreach (AmbientPropertyValue ambientValue in ambientEnumerable)
                {
                    if (ambProp.Equals(ambientValue.RetrievedProperty))
                    {
                        return _text + ((int) ambientValue.Value).ToString();
                    }
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Custom_InternalConverter class
    /// </summary>
    internal class Custom_InternalConverter : TypeConverter
    {
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            string s = value as string;

            if (null == s)
            {
                throw new ArgumentException("This type converter can only convert from Strings");
            }

            Custom_Internal ci = new Custom_Internal();
            IXamlSchemaContextProvider xscProvider = (IXamlSchemaContextProvider)context.GetService(typeof(IXamlSchemaContextProvider));
            IAmbientProvider ambient = (IAmbientProvider) context.GetService(typeof(IAmbientProvider));

            XamlType intAmbientType = xscProvider.SchemaContext.GetXamlType(typeof(Custom_Internal_InternalAmbient));
            XamlMember ambProp = intAmbientType.GetMember("InternalAmbientInt");
            AmbientPropertyValue apv = ambient.GetFirstAmbientValue(null, ambProp);
            if (apv == null)
            {
                ci.InternalString = s;
            }
            else
            {
                ci.InternalString = s + ((int) apv.Value).ToString();
            }

            return ci;
        }
    }
}
