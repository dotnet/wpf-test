// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
* Description: Custom ValueSerializers and TypeConverters.
* Owner: Microsoft
*
 
  
* Revision:         $Revision: 1 $
 
********************************************************************/

using System;
using System.ComponentModel;
using System.Windows.Markup;
using System.Collections;
using System.Globalization;

namespace Avalon.Test.CoreUI.Serialization.Converter
{
    #region Basic ValueSerializer
    /// <summary>
    /// This is very basic ValueSerializer. All methods call the
    /// same method in its base, ValueSerializer.
    /// </summary>
    public class BasicValueSerializer : ValueSerializer
    {
        /// <summary>
        /// Whether this value can be converted to a string.
        /// </summary>
        /// <param name="value">The value being serialized</param>
        /// <param name="context">Context information</param>
        /// <returns>true, it can convert, false otherwise.</returns>
        public override bool CanConvertToString(object value, IValueSerializerContext context)
        {
            return base.CanConvertToString(value, context);
        }
        /// <summary>
        /// Whether the value can be converted from a string.
        /// </summary>
        /// <param name="value">Value to be converted</param>
        /// <param name="context">Context</param>
        /// <returns>true, it can convert, false otherwise.</returns>
        public override bool CanConvertFromString(string value, IValueSerializerContext context)
        {
            return base.CanConvertToString(value, context);
        }
        /// <summary>
        /// Convert a value to a string.
        /// </summary>
        /// <param name="value">The value being serialized</param>
        /// <param name="context">Context information</param>
        /// <returns>converted string</returns>
        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            return base.ConvertToString(value, context);
        }
        /// <summary>
        /// Convert a string to an object.
        /// </summary>
        /// <param name="value">Value to be converted</param>
        /// <param name="context">Context</param>
        /// <returns>converted object</returns>
        public override object ConvertFromString(string value, IValueSerializerContext context)
        {
            return base.ConvertToString(value, context);
        }
    }
    #endregion

    #region ValueSerializer1
    /// <summary>
    /// This is a ValueSerializer that convert string with "ValueSerializer1SomeString"
    /// to customElement with value of "SomeString".
    /// </summary>
    public class ValueSerializer1 : ValueSerializer
    {
        /// <summary>
        /// Whether this value can be converted to a string.
        /// </summary>
        /// <param name="value">The value being serialized</param>
        /// <param name="context">Context information</param>
        /// <returns>true, it can convert, false otherwise.</returns>
        public override bool CanConvertToString(object value, IValueSerializerContext context)
        {
            return true;
        }
        /// <summary>
        /// Whether the value can be converted from a string.
        /// </summary>
        /// <param name="value">Value to be converted</param>
        /// <param name="context">Context</param>
        /// <returns>true, it can convert, false otherwise.</returns>
        public override bool CanConvertFromString(string value, IValueSerializerContext context)
        {
            return true;
        }
        /// <summary>
        /// Convert a value to a string.
        /// </summary>
        /// <param name="value">The value being serialized</param>
        /// <param name="context">Context information</param>
        /// <returns>converted string</returns>
        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            string converted = string.Empty;
            if (value is CustomElementBase)
            {
                converted = ((CustomElementBase)value).Value;
            }
            else if (value is CustomFrameworkElementBase)
            {
                converted = ((CustomFrameworkElementBase)value).Value;
            }
            else
            {
                throw new Exception("element is not a CustomElementBase and CustomeFrameworkElementBase.");
            }
            return _prefix + converted;
        }
        /// <summary>
        /// Convert a string to an object.
        /// </summary>
        /// <param name="value">Value to be converted</param>
        /// <param name="context">Context</param>
        /// <returns>converted object</returns>
        public override object ConvertFromString(string value, IValueSerializerContext context)
        {
            //

            throw new NotImplementedException("ValueSerializer1.ConverterFromeString has not been implemented.");
        }
        string _prefix = "ValueSerializer1";
    }
    #endregion


    #region ValueSerializer2
    /// <summary>
    /// This is a ValueSerializer that convert string with "ValueSerializer2SomeString"
    /// to customElement with value of "SomeString".
    /// </summary>
    public class ValueSerializer2 : ValueSerializer1
    {
        /// <summary>
        /// Convert a value to a string.
        /// </summary>
        /// <param name="value">The value being serialized</param>
        /// <param name="context">Context information</param>
        /// <returns>converted string</returns>
        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            string converted = string.Empty;
            if (value is CustomElementBase)
            {
                converted = ((CustomElementBase)value).Value;
            }
            else if (value is CustomFrameworkElementBase)
            {
                converted = ((CustomFrameworkElementBase)value).Value;
            }
            else
            {
                throw new Exception("element is not a CustomElementBase and CustomeFrameworkElementBase.");
            }
            return _prefix + converted;
        }

        /// <summary>
        /// Convert a string to an object.
        /// </summary>
        /// <param name="value">Value to be converted</param>
        /// <param name="context">Context</param>
        /// <returns>converted object</returns>
        public override object ConvertFromString(string value, IValueSerializerContext context)
        {
            //

            throw new NotImplementedException("ValueSerializer2.ConverterFromeString has not been implemented.");
        }
        string _prefix = "ValueSerializer2";
    }
    #endregion

    #region ValueSerializer3
    /// <summary>
    /// This is a ValueSerializer that convert string with "SomeString"
    /// to customElement with value of "SomeString".
    /// </summary>
    public class ValueSerializer3 : ValueSerializer
    {
        /// <summary>
        /// Convert a value to a string.
        /// </summary>
        /// <param name="value">The value being serialized</param>
        /// <param name="context">Context information</param>
        /// <returns>converted string</returns>
        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            string converted = string.Empty;
            if (value is CustomElementBase)
            {
                converted = ((CustomElementBase)value).Value;
            }
            else if (value is CustomFrameworkElementBase)
            {
                converted = ((CustomFrameworkElementBase)value).Value;
            }
            else
            {
                throw new Exception("element is not a CustomElementBase and CustomeFrameworkElementBase.");
            }
            return converted;
        }

        /// <summary>
        /// Convert a string to an object.
        /// </summary>
        /// <param name="value">Value to be converted</param>
        /// <param name="context">Context</param>
        /// <returns>converted object</returns>
        public override object ConvertFromString(string value, IValueSerializerContext context)
        {
            //

            throw new NotImplementedException("ValueSerializer3.ConverterFromeString has not been implemented.");
        }
    }
    #endregion

    #region InvalidTypeConverter
    /// <summary>
    /// This is a TypeConverter that cannot converter type To/From
    /// string, thus is invalid from ValueSerializer's point of view. 
    /// </summary>
    public class InvalidTypeConverter : TypeConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public object ConvertTo(object value,
                                Type destinationType)
        {
            return null;
        }

        /// <summary>
        /// CanConvertFrom
        /// </summary>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public bool CanConvertFrom(Type sourceType)
        {
            return false;
        }

        /// <summary>
        /// CanConvertTo
        /// </summary>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public bool CanConvertTo(Type destinationType)
        {
            return false;
        }
    }
    #endregion InvalidTypeConverter


    #region ValidTypeConverter
    /// <summary>
    ///     Valid TypeConverter.
    /// </summary>
    public class ValidTypeConverter : TypeConverter
    {

        /// <summary>
        /// ConvertTo - Attempt to convert a CustomPropertyType to the given type
        /// </summary>
        /// <returns>
        /// The object which was constructed.
        /// </returns>
        /// <param name="value"> The object to converter. </param>
        /// <param name="destinationType"> Destination Type</param>
        public object ConvertTo(object value,
                                Type destinationType)
        {
            if (destinationType != typeof(String))
                throw new Exception("Can only convert from string");            
            return ((CustomElementBase)value).Value;
        }

        /// <summary>
        /// ConvertFrom
        /// </summary>
        /// <param name="typeDescriptorContext"></param>
        /// <param name="cultureInfo"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext,
                                           CultureInfo cultureInfo,
                                           object value)
        {
            return new CustomElementBase(value as string);
        }
        /// <summary>
        /// CanConvertFrom
        /// </summary>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public bool CanConvertFrom(Type sourceType)
        {
            if (sourceType.Equals(typeof(String)))
                return true;
            return false;
        }

        /// <summary>
        /// CanConvertTo
        /// </summary>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public bool CanConvertTo(Type destinationType)
        {
            if (destinationType.Equals(typeof(String)))
                return true;
            return false;
        }
    }
    
    /// <summary>
    ///     Valid TypeConverter For CustomElementNoneNoneValidType.
    /// </summary>
    public class ValidTypeConverterForCustomElementNoneNoneValidType : ValidTypeConverter
    {
        /// <summary>
        /// ConvertFrom
        /// </summary>
        /// <param name="typeDescriptorContext"></param>
        /// <param name="cultureInfo"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext,
                                           CultureInfo cultureInfo,
                                           object value)
        {
            return new CustomElementNoneNoneValidType(value as string);
        }   
    }

    /// <summary>
    ///     Valid TypeConverter For CustomElementNoneNoneValidProperty.
    /// </summary>
    public class ValidTypeConverterForCustomElementNoneNoneValidProperty : ValidTypeConverter
    {
        /// <summary>
        /// ConvertFrom
        /// </summary>
        /// <param name="typeDescriptorContext"></param>
        /// <param name="cultureInfo"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext,
                                           CultureInfo cultureInfo,
                                           object value)
        {
            return new CustomElementNoneNoneValidProperty(value as string);
        }
    }

    /// <summary>
    ///     Valid TypeConverter For CustomElementNoneNoneValidType.
    /// </summary>
    public class ValidTypeConverterForCustomElementStringPropertyValidProperty : ValidTypeConverter
    {
        /// <summary>
        /// ConvertFrom
        /// </summary>
        /// <param name="typeDescriptorContext"></param>
        /// <param name="cultureInfo"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext,
                                           CultureInfo cultureInfo,
                                           object value)
        {            
            return new CustomElementStringPropertyValidProperty(value as string);
        }   
    }

    
    /// <summary>
    ///     Valid TypeConverter For CustomElementTypeBothValidProperty.
    /// </summary>
    public class ValidTypeConverterForCustomElementTypeBothValidProperty : ValidTypeConverter
    {
        /// <summary>
        /// ConvertFrom
        /// </summary>
        /// <param name="typeDescriptorContext"></param>
        /// <param name="cultureInfo"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext,
                                           CultureInfo cultureInfo,
                                           object value)
        {
            return new CustomElementTypeBothValidProperty(value as string);
        }   
    }

    /// <summary>
    ///     Valid TypeConverter For CustomElementStringBothValidType.
    /// </summary>
    public class ValidTypeConverterForCustomElementStringBothValidType : ValidTypeConverter
    {
        /// <summary>
        /// ConvertFrom
        /// </summary>
        /// <param name="typeDescriptorContext"></param>
        /// <param name="cultureInfo"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext,
                                           CultureInfo cultureInfo,
                                           object value)
        {
            return new CustomElementStringBothValidType(value as string);
        }
    }
    /// <summary>
    ///     Valid TypeConverter For CustomElementStringTypeValidType.
    /// </summary>
    public class ValidTypeConverterForCustomElementStringTypeValidType : ValidTypeConverter
    {
        /// <summary>
        /// ConvertFrom
        /// </summary>
        /// <param name="typeDescriptorContext"></param>
        /// <param name="cultureInfo"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext,
                                           CultureInfo cultureInfo,
                                           object value)
        {
            return new CustomElementStringTypeValidType(value as string);
        }
    }
    /// <summary>
    ///     Valid TypeConverter For CustomFrameworkElementTypeTypeValidType.
    /// </summary>
    public class ValidTypeConverterForCustomFrameworkElementTypeTypeValidType : ValidTypeConverter
    {
        /// <summary>
        /// ConvertFrom
        /// </summary>
        /// <param name="typeDescriptorContext"></param>
        /// <param name="cultureInfo"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext,
                                           CultureInfo cultureInfo,
                                           object value)
        {
            return new CustomFrameworkElementTypeTypeValidType(value as string);
        }
    }
    #endregion ValidTypeConverter        
    #region CustomElementPropertyConverter
    /// <summary>
    ///   CustomElementPropertyConverter.
    /// </summary>
    public class CustomElementPropertyConverter : ValidTypeConverter
    {
        /// <summary>
        /// ConvertFrom
        /// </summary>
        /// <param name="typeDescriptorContext"></param>
        /// <param name="cultureInfo"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext,
                                           CultureInfo cultureInfo,
                                           object value)
        {
            return new CustomElementProperty(value as string);
        }
    }
        #endregion         
    #region CustomElementTypeConverter
    /// <summary>
    ///     CustomElementTypeConverter.
    /// </summary>
    public class CustomElementTypeConverter : ValidTypeConverter
    {
        /// <summary>
        /// ConvertFrom
        /// </summary>
        /// <param name="typeDescriptorContext"></param>
        /// <param name="cultureInfo"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext,
                                           CultureInfo cultureInfo,
                                           object value)
        {
            return new CustomElementType(value as string);
        }
    }
    #endregion 
    #region CustomFrameworkElementPropertyConverter
    /// <summary>
    ///   CustomFrameworkElementPropertyConverter.
    /// </summary>
    public class CustomFrameworkElementPropertyConverter : ValidTypeConverter
    {
        /// <summary>
        /// ConvertFrom
        /// </summary>
        /// <param name="typeDescriptorContext"></param>
        /// <param name="cultureInfo"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext,
                                           CultureInfo cultureInfo,
                                           object value)
        {
            return new CustomFrameworkElementProperty(value as string);
        }
    }
        #endregion
    #region CustomFrameworkElementTypeConverter
    /// <summary>
    ///     CustomFrameworkElementTypeConverter.
    /// </summary>
    public class CustomFrameworkElementTypeConverter : ValidTypeConverter
    {
        /// <summary>
        /// ConvertFrom
        /// </summary>
        /// <param name="typeDescriptorContext"></param>
        /// <param name="cultureInfo"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext,
                                           CultureInfo cultureInfo,
                                           object value)
        {
            return new CustomFrameworkElementType(value as string);
        }
    }
    #endregion 
}



