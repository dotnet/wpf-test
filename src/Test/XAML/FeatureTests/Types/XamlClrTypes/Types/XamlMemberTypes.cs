// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// MemberContainer CLASS
    /// </summary>
    public class MemberContainer
    {
        /// <summary> stringProperty With NoSetter </summary>
        private string _stringPropertyWithNoSetter = null;

        /// <summary>
        /// Occurs when [public event property].
        /// </summary>
        public event EventHandler PublicEventProperty
        {
            add
            {
            }

            remove
            {
            }
        }

        /// <summary>
        /// Gets or sets the string property with getter setter.
        /// </summary>
        /// <value>The string property with getter setter.</value>
        [TypeConverter(typeof(StringTypeConverter1))]
        public string StringPropertyWithGetterSetter { get; set; }

        /// <summary>
        /// Gets the string property with no setter.
        /// </summary>
        /// <value>The string property with no setter.</value>
        public string StringPropertyWithNoSetter
        {
            get
            {
                return this._stringPropertyWithNoSetter;
            }
        }

        /// <summary>
        /// Gets or sets the protected string property.
        /// </summary>
        /// <value>The protected string property.</value>
        public string ProtectedStringProperty { get; set; }

        /// <summary>
        /// Gets the string prop.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>string value</returns>
        public static string GetStringProp(object target)
        {
            return string.Empty;
        }

        /// <summary>
        /// Sets the string prop.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="prop">The prop value.</param>
        public static void SetStringProp(object target, string prop)
        {
        }

        /// <summary>
        /// Gets the event prop.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>string value</returns>
        public static string GetEventProp(object target)
        {
            return null;
        }

        /// <summary>
        /// Sets the event prop.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="prop">The prop value.</param>
        public static void SetEventProp(object target, EventHandler prop)
        {
        }

        /// <summary>
        /// Gets the invalid prop.
        /// </summary>
        public static void GetInvalidProp()
        {
        }

        /// <summary>
        /// Sets the invalid prop.
        /// </summary>
        public static void SetInvalidProp()
        {
        }

        /// <summary>
        /// Gets the string property with getter setter prop.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>string empty </returns>
        public static string GetStringPropertyWithGetterSetterProp(object target)
        {
            return String.Empty;
        }

        /// <summary>
        /// Sets the string property with getter setter prop.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="prop">The prop value.</param>
        public static void SetStringPropertyWithGetterSetterProp(object target, string prop)
        {
        }
    }

    /// <summary>
    /// CustomType class
    /// </summary>
    [TypeConverter(typeof(CustomTypeConverter))]
    public class CustomType
    {
    }

    /// <summary>
    /// MemberContainer1 class
    /// </summary>
    [TypeConverter(typeof(StringTypeConverter1))]
    public class MemberContainer1
    {
        /// <summary>
        /// Gets or sets the type of the custom.
        /// </summary>
        /// <value>The type of the custom.</value>
        public CustomType CustomType { get; set; }

        /// <summary>
        /// Gets or sets the custom type1.
        /// </summary>
        /// <value>The custom type1.</value>
        [TypeConverter(typeof(StringTypeConverter1))]
        public CustomType CustomType1 { get; set; }
    }

    /// <summary>
    /// Custom TypeConverter
    /// </summary>
    public class CustomTypeConverter : TypeConverter
    {
    }

    /// <summary>
    /// String TypeConverter1
    /// </summary>
    public class StringTypeConverter1 : TypeConverter
    {
    }
}
