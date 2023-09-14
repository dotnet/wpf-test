// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Windows.Threading;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;
using System.Xml;

using Avalon.Test.CoreUI.Common;

namespace Avalon.Test.CoreUI.Serialization.PerPropertySerializers
{
    #region class CustomElement
    /// <summary>Basic custom element derived from FrameworkElement.</summary>
    public class CustomElement : FrameworkElement
    {
        /// <summary>
        /// 
        /// </summary>
        public CustomElement() : base()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        static CustomElement()
        {
        }

        /// <summary>
        /// Property with TypeConverter defined on Type, not on Property.
        /// </summary>
		public ObjectWithConverter PropertyA
		{
            get { return _propertyA; }
            set { _propertyA = value; }
        }

		private ObjectWithConverter _propertyA;

		/// <summary>
        /// Property with TypeConverter defined on Type and on Property.
        /// </summary>
        [TypeConverter(typeof(TypeConverterOnProperty))]
        public ObjectWithConverter PropertyB
        {
            get { return _propertyB; }
            set { _propertyB = value; }
        }

		private ObjectWithConverter _propertyB;

    }
    #endregion class CustomElement

    #region class CustomPropAttacher
    /// <summary>
    /// 
    /// </summary>
    public class CustomPropAttacher
    {
        #region AttachedPropertyA
        // Attached property with TypeConverter defined on Type, not on Property.

        // Property storage.
        private static Hashtable s_fieldfor_AttachedPropertyA = new Hashtable();

        /// <summary>
        /// Static setter
        /// </summary>
        public static void SetAttachedPropertyA(object target, ObjectWithConverter value)
        {
            s_fieldfor_AttachedPropertyA[target] = value;
        }

        /// <summary>
        /// Static getter
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static ObjectWithConverter GetAttachedPropertyA(object target)
        {
            return (s_fieldfor_AttachedPropertyA[target] as ObjectWithConverter);
        }
        #endregion AttachedPropertyA

        #region AttachedPropertyB
        // Attached property with TypeConverter defined on Type AND on Property.
        // Type converter on the property should take precedence.

        // Property storage.
        private static Hashtable s_fieldfor_AttachedPropertyB = new Hashtable();

        /// <summary>
        /// Static setter
        /// </summary>
        public static void SetAttachedPropertyB(object target, ObjectWithConverter value)
        {
            s_fieldfor_AttachedPropertyB[target] = value;
        }

        /// <summary>
        /// Static getter
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        [TypeConverter(typeof(TypeConverterOnProperty))]
        public static ObjectWithConverter GetAttachedPropertyB(object target)
        {
            return (s_fieldfor_AttachedPropertyB[target] as ObjectWithConverter);
        }
        #endregion AttachedPropertyB
    }
    #endregion class CustomPropAttacher

    #region class ObjectWithConverter
    /// <summary>
    /// Type of properties defined in CustomElement.
    /// </summary>
    [TypeConverter(typeof(TypeConverterOnType))]
    public class ObjectWithConverter
    {
        private string _val = String.Empty;

        private string _conversionMarker = String.Empty;

        /// <summary>
        /// Accesses stored string.
        /// </summary>
        public string Value
        {
            get { return _val; }
        }

        /// <summary>
        /// Accesses stored string.
        /// </summary>
        public string ConversionMarker
        {
            get { return _conversionMarker; }
        }

        /// <summary>
        /// Initializes stored string.
        /// </summary>
        public ObjectWithConverter()
        {
        }

        /// <summary>
        /// Initializes stored string.
        /// </summary>
        public ObjectWithConverter(string val, string conversionMarker)
        {
            _val = val;
            _conversionMarker = conversionMarker;
        }
    }
    #endregion class ObjectWithConverter

    #region class TypeConverterOnType
    /// <summary>
    /// TypeConverter used for converting ObjectWithConverters. Intended to be specified
    /// directly on 'ObjectWithConverter', not on individual properties.
    /// </summary>
    public class TypeConverterOnType : TypeConverter
    {
        /// <summary>TypeConverter member.</summary>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        /// <summary>TypeConverter member.</summary>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return true;
        }

        /// <summary>TypeConverter member.</summary>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return new ObjectWithConverter(value.ToString(), "TypeConverterOnType");
        }

        /// <summary>TypeConverter member.</summary>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            ObjectWithConverter propertyType = (ObjectWithConverter)value;

            return propertyType.Value;
        }
    }
    #endregion class TypeConverterOnType

    #region class TypeConverterOnProperty
    /// <summary>
    /// TypeConverter for converting ObjectWithConverters.  Intended to be specified
    /// directly on individual properties, not the type 'ObjectWithConverter'.
    /// </summary>
    public class TypeConverterOnProperty : TypeConverter
    {
        /// <summary>TypeConverter member.</summary>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        /// <summary>TypeConverter member.</summary>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return true;
        }

        /// <summary>TypeConverter member.</summary>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return new ObjectWithConverter(value.ToString(), "TypeConverterOnProperty");
        }

        /// <summary>TypeConverter member.</summary>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            ObjectWithConverter propertyType = (ObjectWithConverter)value;

            return propertyType.Value;
        }
    }
    #endregion class TypeConverterOnProperty

    #region class Verifier
    /// <summary>
    /// Holds verification routines for per-property TypeConverters and Serializers.
    /// </summary>
    public class Verifier
    {
        /// <summary>
        /// Verifies TypeConverters on CLR properties and attached 
        /// properties are respected.
        /// </summary>
        public static void Verify1(UIElement root)
        {
            CoreLogger.LogStatus("Getting 'TargetElement1'. Should be CustomElement...");
            CustomElement customElement = (CustomElement)LogicalTreeHelper.FindLogicalNode(root, "TargetElement1");

            //
            // Verify PropertyA.
            //
            CoreLogger.LogStatus("Verifying Element's PropertyA...");
            if (customElement.PropertyA.Value != "PropA")
            {
                throw new Microsoft.Test.TestValidationException("Element's PropertyA.Value != 'PropA'.");
            }

            if (customElement.PropertyA.ConversionMarker != "TypeConverterOnType")
            {
                throw new Microsoft.Test.TestValidationException("Element's PropertyA.ConversionMarker != 'TypeConverterOnType'.");
            }

            if (CustomPropAttacher.GetAttachedPropertyA(customElement).Value != "PropA")
            {
                throw new Microsoft.Test.TestValidationException("Element's CustomPropAttacher.AttachedPropertyA.Value != 'PropA'.");
            }

            if (CustomPropAttacher.GetAttachedPropertyA(customElement).ConversionMarker != "TypeConverterOnType")
            {
                throw new Microsoft.Test.TestValidationException("Element's CustomPropAttacher.AttachedPropertyA.ConversionMarker != 'TypeConverterOnType'.");
            }

            //
            // Verify PropertyB.
            //
            CoreLogger.LogStatus("Verifying Element's PropertyB...");
            if (customElement.PropertyB.Value != "PropB")
            {
                throw new Microsoft.Test.TestValidationException("Element's PropertyB.Value != 'PropB'.");
            }

            if (customElement.PropertyB.ConversionMarker != "TypeConverterOnProperty")
            {
                throw new Microsoft.Test.TestValidationException("Element's PropertyB.ConversionMarker != 'TypeConverterOnProperty'.");
			}

            if (CustomPropAttacher.GetAttachedPropertyB(customElement).Value != "PropB")
            {
                throw new Microsoft.Test.TestValidationException("Element's CustomPropAttacher.AttachedPropertyB.Value != 'PropB'.");
            }

            if (CustomPropAttacher.GetAttachedPropertyB(customElement).ConversionMarker != "TypeConverterOnProperty")
            {
                throw new Microsoft.Test.TestValidationException("Element's CustomPropAttacher.AttachedPropertyB.ConversionMarker != 'TypeConverterOnProperty'.");
            }
		}
    }
    #endregion class Verifier
}

