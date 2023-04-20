// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xaml;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Test.Serialization;

namespace Microsoft.Test.Xaml.Utilities
{
    /// <summary>
    /// Generic XML serializer that follows the same convention as the System.Xml.Serialization
    /// The value of this is that it is interporable with our own serialization and we can customize
    /// it as needed
    /// </summary>
    public class XamlObjectSerializer
    {
        #region Private Members

        /// <summary>
        /// Invariant SerializationCulture
        /// </summary>
        internal static readonly CultureInfo InvariantSerializationCulture;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes static members of the <see cref="XamlObjectSerializer"/> class.
        /// </summary>
        static XamlObjectSerializer()
        {
            InvariantSerializationCulture = CultureInfo.InvariantCulture.Clone() as CultureInfo;
            InvariantSerializationCulture.DateTimeFormat.ShortTimePattern = "HH:mm:ss.fffffffK";
        }

        #endregion

        #region IObjectSerializer Implementation

        /// <summary>
        /// Serializes the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="obj">The object.</param>
        /// <param name="writeElement">if set to <c>true</c> [write element].</param>
        public void Serialize(XmlTextWriter writer, object obj, bool writeElement)
        {
            // Use the IXmlSerializable implementation if needed
            IXmlSerializable serializableObject = obj as IXmlSerializable;
            if (serializableObject != null)
            {
                serializableObject.WriteXml(writer);
                return;
            }

            // If the type can be converted directly to a string then serialize it
            // as an element with text content (int, string, etc)
            TypeConverter conv = TypeDescriptor.GetConverter(obj);
            if (CanSerialize(conv))
            {
                writer.WriteElementString(obj.GetType().Name.ToLowerInvariant(), conv.ConvertToInvariantString(obj));
                return;
            }

            // Serialize the object and its public members
            if (writeElement)
            {
                writer.WriteStartElement(obj.GetType().Name);
            }

            // Get the list of properties with the ones that want to be serialized as attributed listed first
            List<PropertyDescriptor> props = GetSortedProperties(obj);

            // Serialize the Properties
            int i = 0;
            foreach (PropertyDescriptor prop in props)
            {
                i++;
                //// Get the property value
                object value;
                value = prop.GetValue(obj);
                if (value == null)
                {
                    writer.WriteStartElement(prop.Name);
                    writer.WriteString("null");
                    writer.WriteEndElement();
                    continue;
                }

                if (prop.IsReadOnly)
                {
                    // if the property is read-only and is a list then serialize the content
                    IList list = value as IList;
                    if (list != null && list.Count > 0)
                    {
                        writer.WriteStartElement(prop.Name);

                        XamlListSerializer serializer = new XamlListSerializer();
                        serializer.Serialize(writer, list, false);

                        writer.WriteEndElement();
                        continue;
                    }
                }

                if (value is XamlType)
                {
                    writer.WriteStartElement(prop.Name);
                    writer.WriteString(((XamlType) value).Name);
                    writer.WriteEndElement();
                    continue;
                }
                else if (value is XamlMember)
                {
                    writer.WriteStartElement(prop.Name);
                    Serialize(writer, value, true);
                    writer.WriteEndElement();
                }
                else if (value is System.Reflection.MemberInfo)
                {
                    writer.WriteStartElement(prop.Name);
                    writer.WriteString(value.ToString());
                    writer.WriteEndElement();
                    continue;
                }
                else
                {
                    // Get a converter and Serialize it to a string
                    TypeConverter converter = prop.Converter;
                    if (converter == null || !converter.CanConvertTo(typeof(string)) || !converter.CanConvertFrom(typeof(string)))
                    {
                        writer.WriteStartElement(prop.Name);
                        ObjectSerializer.Serialize(writer, value, false);
                        writer.WriteEndElement();
                        continue;
                    }

                    string strValue = converter.ConvertToString(null, InvariantSerializationCulture, value);

                    if (prop.Attributes.Contains(new XmlAttributeAttribute()))
                    {
                        writer.WriteAttributeString(prop.Name, strValue);
                    }
                    else
                    {
                        writer.WriteElementString(prop.Name, strValue);
                    }
                }
            }

            if (writeElement)
            {
                writer.WriteEndElement(); // Object element
            }
        }

        /// <summary>
        /// Deserializes the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="type">The type .</param>
        /// <param name="context">The context.</param>
        /// <returns>object value</returns>
        public object Deserialize(XmlTextReader reader, Type type, object context)
        {
            // If the object has a type converter then just read the attribute string
            TypeConverter conv = TypeDescriptor.GetConverter(type);
            if (CanSerialize(conv))
            {
                string strValue = reader.ReadElementString();
                object value = conv.ConvertFromInvariantString(strValue);
                return value;
            }

            // Create an instance of the type
            object obj = Activator.CreateInstance(type);

            // Use the IXmlSerializable implementation if needed
            IXmlSerializable serializableObject = obj as IXmlSerializable;
            if (serializableObject != null)
            {
                serializableObject.ReadXml(reader);
                return obj;
            }

            // parse all the attributes as properties
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(type);
            bool hasChildren = !reader.IsEmptyElement;
            if (reader.MoveToFirstAttribute())
            {
                do
                {
                    PropertyDescriptor prop = properties[reader.Name];
                    if (prop == null)
                    {
                        throw new InvalidOperationException("the property " + reader.Name + " does not exist on the object " + type.Name);
                    }

                    reader.ReadAttributeValue();
                    TypeConverter converter = prop.Converter;
                    object value = converter.ConvertFromInvariantString(reader.Value);
                    prop.SetValue(obj, value);
                } 
                while (reader.MoveToNextAttribute());
            }

            reader.Read();

            if (hasChildren)
            {
                // Parse the children as properties
                while (true)
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        // If the property is read-only and a list then add the children to the list members
                        PropertyDescriptor prop = properties[reader.Name];
                        if (prop == null)
                        {
                            throw new InvalidOperationException("the property " + reader.Name + " does not exist on the object " + type.Name);
                        }

                        if (prop.IsReadOnly)
                        {
                            IList list = prop.GetValue(obj) as IList;
                            if (list != null)
                            {
                                XamlListSerializer serializer = new XamlListSerializer();
                                serializer.Deserialize(reader, list.GetType(), list);
                            }
                        }
                        else
                        {
                            // parse the value and set the property
                            object value = ObjectSerializer.Deserialize(reader, prop.PropertyType, null);
                            prop.SetValue(obj, value);
                        }
                    }

                    // If end element then we are done parsing children.
                    else if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        reader.Read(); // end element
                        break;
                    }
                    else if (reader.EOF)
                    {
                        break; // end of document
                    }
                    else // If we don't explicitly handle this kind of node then ignore it.
                    {
                        reader.Read();
                    }
                }
            }

            return obj;
        }

        #endregion

        #region Static Members

        /// <summary>
        /// Determines whether this instance can serialize the specified converter1.
        /// a converter can serialize if it supports convertion to and from a string
        /// </summary>
        /// <param name="converter1">The converter1.</param>
        /// <returns>
        /// <c>true</c> if this instance can serialize the specified converter1; otherwise, <c>false</c>.
        /// </returns>
        internal static bool CanSerialize(TypeConverter converter1)
        {
            return converter1 != null && converter1.CanConvertTo(typeof(string)) && converter1.CanConvertFrom(typeof(string));
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Gets the sorted properties.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Listof PropertyDescriptor </returns>
        private static List<PropertyDescriptor> GetSortedProperties(object obj)
        {
            // Gets all the properties for an object and sorts them by
            // which properties want to be serialized as an XMLAttribute
            // and then alphabetically by name

            // HACK: we have to use a List<T> because the Sort method on the
            // PropertyDescriptorCollection doesn't respect the comparer
            List<PropertyDescriptor> props = new List<PropertyDescriptor>();
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(obj))
            {
                props.Add(prop);
            }

            props.Sort(delegate(PropertyDescriptor x, PropertyDescriptor y)
                           {
                               return StringComparer.InvariantCulture.Compare(x.Name, y.Name); // sort by name                
                           });

            return props;
        }

        #endregion
    }

    /// <summary>
    /// Serializes a list of objects
    /// </summary>
    internal class XamlListSerializer
    {
        #region Private Data

        /// <summary>
        /// item ElementName
        /// </summary>
        private readonly string _itemElementName;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XamlListSerializer"/> class.
        /// </summary>
        public XamlListSerializer() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XamlListSerializer"/> class.
        /// itemElementName: overrides the name of the item elements
        /// </summary>
        /// <param name="itemElementName">Name of the item element.</param>
        public XamlListSerializer(string itemElementName)
        {
            this._itemElementName = itemElementName;
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Gets the type of the generic list.
        /// determines the type of a generic list
        /// </summary>
        /// <param name="type">The type .</param>
        /// <returns>Type value</returns>
        public static Type GetGenericListType(Type type)
        {
            Type interfaceType = type.GetInterface("IList`1");
            if (interfaceType != null)
            {
                return interfaceType.GetGenericArguments()[0];
            }

            return null;
        }

        #endregion

        #region IObjectSerializer Implementation

        /// <summary>
        /// Serializes the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="obj">The object.</param>
        /// <param name="writeElement">if set to <c>true</c> [write element].</param>
        public void Serialize(XmlTextWriter writer, object obj, bool writeElement)
        {
            IList list = (IList) obj;

            // Serialize the object and its public members
            // Generic types have a ` in the name so we have to strip it out
            if (writeElement)
            {
                writer.WriteStartElement(obj.GetType().Name.Replace("`", string.Empty));
            }

            if (list.Count > 0)
            {
                // determine the converter for the type of the list if it is a generic
                // if it is not a generic then it will use a converter per type
                Type listType = GetGenericListType(list.GetType());
                TypeConverter listTypeConverter = null;
                if (listType != null)
                {
                    listTypeConverter = TypeDescriptor.GetConverter(listType);
                }

                if (!XamlObjectSerializer.CanSerialize(listTypeConverter))
                {
                    listTypeConverter = null;
                }

                foreach (object listEntry in list)
                {
                    // If we have a item element name specified then use it otherwise use the name of the type
                    string elementName = string.Empty;
                    XamlType xamlType = listEntry as XamlType;
                    if (xamlType != null)
                    {
                        elementName = xamlType.Name;
                    }
                    else
                    {
                        elementName = (_itemElementName != null) ? _itemElementName : listEntry.GetType().Name;
                    }

                    if (listTypeConverter != null)
                    {
                        writer.WriteElementString(elementName, listTypeConverter.ConvertToString(null, XamlObjectSerializer.InvariantSerializationCulture, listEntry));
                    }
                    else
                    {
                        writer.WriteStartElement(elementName);
                        writer.WriteEndElement();
                    }
                }
            }

            if (writeElement)
            {
                writer.WriteEndElement(); // Object element
            }
        }

        /// <summary>
        /// Deserializes the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="type">The type .</param>
        /// <param name="context">The context.</param>
        /// <returns>object value</returns>
        public object Deserialize(XmlTextReader reader, Type type, object context)
        {
            // see if we need to add items to the context
            IList list = context as IList;

            // otherwise create the object
            if (list == null)
            {
                list = (IList) Activator.CreateInstance(type);
            }

            if (!reader.IsEmptyElement)
            {
                // If the list is a generic then predermine the type
                // otherwise it will try to use the element-type mapping
                Type listType = GetGenericListType(list.GetType());
                reader.Read(); // first child element
                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        object listEntry = ObjectSerializer.Deserialize(reader, listType, null);
                        list.Add(listEntry);
                    }
                    else
                    {
                        reader.Read(); // Ignore non-Element nodes.
                    }
                }
            }

            reader.Read(); // end element

            return list;
        }

        #endregion
    }
}
