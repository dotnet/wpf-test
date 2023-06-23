using System;
using System.Collections;
using System.ComponentModel;
using System.Xml;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Parameter Builder base
    /// </summary>
    public abstract class ParameterBuilder
    {
        /// <summary>
        /// Construct parameter list base on XmlAttribute information.
        /// </summary>
        /// <param name="parameterList">ArrayList</param>
        /// <param name="attribute">XmlAttribute</param>
        public abstract void Construct(ArrayList parameterList, XmlAttribute attribute);

        /// <summary>
        /// Convert attribute value to type.
        /// </summary>
        /// <param name="value">string value</param>
        /// <param name="type">type</param>
        /// <returns>type</returns>
        protected object ConvertXmlAttributeValueToTypeValue(string value, Type type)
        {
            if (value == "{null}")
            {
                return null;
            }
            // Hardcode to return DateTime.Now because we convert string to Nullable DateTime struct type.
            else if (type.Name.Contains("Nullable"))
            {
                return DateTime.Now;
            }
            else
            {
                //We use a type converter to convert the string into the requested type
                TypeConverter typeConverter = TypeDescriptor.GetConverter(type);
                if (!typeConverter.CanConvertFrom(typeof(string)))
                {
                    throw new ArgumentException("The value cannot be converted from a string");
                }
                return typeConverter.ConvertFromInvariantString(value);
            }
        }
    }
}
