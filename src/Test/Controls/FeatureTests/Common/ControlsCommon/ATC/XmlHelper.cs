//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------


#region Using Statements

using System;
using System.ComponentModel;
using System.Xml;

#endregion

namespace Avalon.Test.ComponentModel.Utilities
{
    /// <summary>
    /// helper class for xml operations
    /// </summary>
    public static class XmlHelper
    {
        /// <summary>
        /// get attribute value from an xml element
        /// </summary>
        /// <typeparam name="ObjectType">attribute value type</typeparam>
        /// <param name="element">xml elment</param>
        /// <param name="attributeName"></param>
        /// <param name="defaultValue"></param>
        /// <returns>attribute value or default value if not found or not be able to be converted</returns>
        public static ObjectType GetAttribute<ObjectType>(XmlElement element, string attributeName, ObjectType defaultValue)
        {
            try
            {
                return GetAttribute<ObjectType>(element, attributeName);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// get attribute value from an xml element
        /// </summary>
        /// <typeparam name="ObjectType">attribute value type</typeparam>
        /// <param name="element">xml element</param>
        /// <param name="attributeName"></param>
        /// <returns>attribute value; throw exception if not found or not be able to be converted</returns>
        public static ObjectType GetAttribute<ObjectType>(XmlElement element, string attributeName)
        {
            if (element == null)
                throw new Exception("Xml element is null");
            string stringValue = element.GetAttribute(attributeName);
            return Convert<ObjectType, string>(stringValue);
        }

        /// <summary>
        /// get attribute value from an xml element
        /// </summary>
        /// <param name="type">attribute value type</param>
        /// <param name="element">xml element</param>
        /// <param name="attributeName"></param>
        /// <returns>attribute value or null if not found or not be able to be converted</returns>
        public static object GetAttribute(Type type, XmlElement element, string attributeName)
        {
            if (element == null)
                return null;
            string stringValue = element.GetAttribute(attributeName);
            return ConvertToType(type, stringValue);
        }

        /// <summary>
        /// test whether an element has an attribute
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static bool HasAttribute(XmlElement element, string attributeName)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (string.IsNullOrEmpty(attributeName))
                throw new ArgumentNullException("attributeName");

            return element.HasAttribute(attributeName);
        }

        /// <summary>
        /// convert a value from one type to another
        /// </summary>
        /// <param name="targetType">target type</param>
        /// <param name="sourceValue">source value</param>
        /// <returns>target value or null if not able to be converted</returns>
        public static object ConvertToType(Type targetType, object sourceValue)
        {
            if (sourceValue.GetType() == targetType)
                return sourceValue;
            if (targetType.IsAssignableFrom(sourceValue.GetType()))
                return sourceValue;

            TypeConverter converter = TypeDescriptor.GetConverter(targetType);
            if (converter == null)
                return null;
            if (converter.CanConvertFrom(sourceValue.GetType()))
            {
                try
                {
                    return converter.ConvertFrom(sourceValue);
                }
                catch (FormatException)
                {
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// convert value from one type to another
        /// </summary>
        /// <typeparam name="TargetType">target type</typeparam>
        /// <typeparam name="SourceType">source type</typeparam>
        /// <param name="sourceValue">source value</param>
        /// <returns>target value or exception thrown if not able to be converted</returns>
        public static TargetType Convert<TargetType, SourceType>(SourceType sourceValue)
        {
            if (typeof(SourceType) == typeof(TargetType))
                return (TargetType)(object)sourceValue;
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TargetType));
            if (converter == null)
                throw new Exception("No converter found");
            if (!converter.CanConvertFrom(typeof(SourceType)))
                throw new Exception("Cannot convert from " + typeof(SourceType).Name + " to " + typeof(TargetType).Name);
            return (TargetType)converter.ConvertFrom(sourceValue);
        }

        /// <summary>
        /// convert value from one type to another
        /// </summary>
        /// <typeparam name="TargetType">target type</typeparam>
        /// <typeparam name="SourceType">source type</typeparam>
        /// <param name="sourceValue">source value</param>
        /// <param name="defaultValue">default value of target type</param>
        /// <returns>target value or default value if not able to be converted</returns>
        public static TargetType Convert<TargetType, SourceType>(SourceType sourceValue, TargetType defaultValue)
        {
            try
            {
                return Convert<TargetType, SourceType>(sourceValue);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
    }
}
