// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Markup;
using System.Xaml;

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// Custom_Type_ProtectedProp class
    /// </summary>
    [RuntimeNameProperty("Name")]
    [ContentProperty("Content")]
    public class Custom_Type_ProtectedProp
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
        /// Gets or sets the protected string.
        /// </summary>
        /// <value>The protected string.</value>
        protected string ProtectedString { get; set; }

        /// <summary>
        /// public method to get a protected property
        /// </summary>
        /// <returns>the protected string</returns>
        public string GetProtectedString()
        {
            return ProtectedString;
        }

        /// <summary>
        /// public method to set a protected property
        /// </summary>
        /// <param name="value">the protected string</param>
        public void SetProtectedString(string value)
        {
            ProtectedString = value;
        }
    }

    /// <summary>
    /// Custom_Type_ProtectedEvent class
    /// </summary>
    [RuntimeNameProperty("Name")]
    [ContentProperty("Content")]
    public class Custom_Type_ProtectedEvent
    {
        /// <summary>
        /// Sets the protected event
        /// </summary>
        /// <value>The protected event.</value>
        protected event EventHandler ProtectedEvent;

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
            return ProtectedEvent != null;
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
    /// Custom ProtectedBase
    /// </summary>
    public class Custom_ProtectedBase
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
        /// Gets or sets the protected string.
        /// </summary>
        /// <value>The protected string.</value>
        protected string ProtectedString { get; set; }

        /// <summary>
        /// uses reflection to return the desired property
        /// </summary>
        /// <param name="propertyName">name of the property to get</param>
        /// <returns>value of the property</returns>
        public object GetNonpublicProperty(string propertyName)
        {
            Type type = this.GetType();
            PropertyInfo pi = type.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);
            return pi.GetValue(this, null);
        }

        /// <summary>
        /// uses reflection to set a property
        /// </summary>
        /// <param name="propertyName">name of the property to set</param>
        /// <param name="value">new value of the property</param>
        public void SetNonpublicProperty(string propertyName, object value)
        {
            Type type = this.GetType();
            PropertyInfo pi = type.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);
            pi.SetValue(this, value, null);
        }
    }

    /// <summary>
    /// Custom Private class
    /// </summary>
    [RuntimeNameProperty("Name")]
    [ContentProperty("Content")]
    public class Custom_Private : Custom_ProtectedBase
    {
        /// <summary>
        /// Gets or sets private data
        /// </summary>
        private string PrivateString { get; set; }
    }

    /// <summary>
    /// Custom Protected class
    /// </summary>
    [RuntimeNameProperty("Name")]
    [ContentProperty("Content")]
    public class Custom_Protected : Custom_ProtectedBase
    {
    }

    /// <summary>
    /// Custom_Protected_ProtectedContent class
    /// </summary>
    [RuntimeNameProperty("Name")]
    [ContentProperty("Content")]
    public class Custom_Protected_ProtectedContent : Custom_ProtectedBase
    {
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        protected new object Content { get; set; }
    }

    /// <summary>
    /// Custom_Protected_ProtectedAmbient class
    /// </summary>
    [RuntimeNameProperty("Name")]
    [ContentProperty("Content")]
    public class Custom_Protected_ProtectedAmbient : Custom_ProtectedBase
    {
        /// <summary>
        /// Gets or sets the protected ambient int.
        /// </summary>
        /// <value>The protected ambient int.</value>
        [Ambient]
        protected int ProtectedAmbientInt { get; set; }
    }

    /// <summary>
    /// Custom_Protected_Collections class
    /// </summary>
    [RuntimeNameProperty("Name")]
    [ContentProperty("Content")]
    public class Custom_Protected_Collections : Custom_ProtectedBase
    {
        /// <summary>
        /// List of RO objects
        /// </summary>
        private readonly List<object> _listRO;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_Protected_Collections"/> class.
        /// </summary>
        public Custom_Protected_Collections()
        {
            _listRO = new List<object>();
            ListRW = new List<object>();
        }

        /// <summary>
        /// Gets the list RO.
        /// </summary>
        /// <value>The list RO.</value>
        protected List<object> ListRO
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
        protected List<object> ListRW { get; set; }
    }

    /// <summary>
    /// Custom_Protected_ProtectedTypeProp class
    /// </summary>
    [RuntimeNameProperty("Name")]
    [ContentProperty("Content")]
    public class Custom_Protected_ProtectedTypeProp : Custom_ProtectedBase
    {
        /// <summary>
        /// Gets or sets the custom_ protected property.
        /// </summary>
        /// <value>The custom_ protected property.</value>
        [TypeConverter(typeof(Custom_ProtectedConverter))]
        protected Custom_Protected Custom_ProtectedProperty { get; set; }
    }

    /// <summary>
    /// Custom_ProtectedExtension class
    /// </summary>
    public class Custom_ProtectedExtension : MarkupExtension
    {
        /// <summary>
        /// string text value
        /// </summary>
        private readonly string _text;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_ProtectedExtension"/> class.
        /// </summary>
        public Custom_ProtectedExtension()
        {
            _text = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_ProtectedExtension"/> class.
        /// </summary>
        /// <param name="text">The text value.</param>
        public Custom_ProtectedExtension(string text)
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
            IXamlSchemaContextProvider xscProvider = (IXamlSchemaContextProvider)serviceProvider.GetService(typeof(IXamlSchemaContextProvider));
            IAmbientProvider ambient = (IAmbientProvider)serviceProvider.GetService(typeof(IAmbientProvider));

            XamlType intAmbientType = xscProvider.SchemaContext.GetXamlType(typeof(Custom_Protected_ProtectedAmbient));
            XamlMember ambProp = intAmbientType.GetMember("ProtectedAmbientInt");
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
                        return _text + ((int)ambientValue.Value).ToString();
                    }
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Custom_ProtectedConverter class
    /// </summary>
    public class Custom_ProtectedConverter : TypeConverter
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

            Custom_Protected ci = new Custom_Protected();
            IXamlSchemaContextProvider xscProvider = (IXamlSchemaContextProvider)context.GetService(typeof(IXamlSchemaContextProvider));
            IAmbientProvider ambient = (IAmbientProvider)context.GetService(typeof(IAmbientProvider));

            XamlType intAmbientType = xscProvider.SchemaContext.GetXamlType(typeof(Custom_Protected_ProtectedAmbient));
            XamlMember ambProp = intAmbientType.GetMember("ProtectedAmbientInt");
            AmbientPropertyValue apv = ambient.GetFirstAmbientValue(null, ambProp);
            if (apv == null)
            {
                ci.SetNonpublicProperty("ProtectedString", s);
            }
            else
            {
                ci.SetNonpublicProperty("ProtectedString", s + ((int)apv.Value).ToString());
            }

            return ci;
        }
    }
}
