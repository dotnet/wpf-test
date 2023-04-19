// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Collections.Generic;

namespace Microsoft.Test.Globalization
{
    /// <summary>
    /// define all the default attributes here for various classes.
    /// </summary>
    internal sealed class DefaultAttributes
    {
        /// <summary>
        /// 
        /// </summary>
        static DefaultAttributes()
        {
            // predefined localizability attributes
            s_definedAttributes = new Dictionary<object, LocalizabilityAttribute>(32);

            // nonlocalizable attribute
            LocalizabilityAttribute notReadable = new LocalizabilityAttribute(LocalizationCategory.None);
            notReadable.Readability = Readability.Unreadable;

            LocalizabilityAttribute notModifiable = new LocalizabilityAttribute(LocalizationCategory.None);
            notModifiable.Modifiability = Modifiability.Unmodifiable;
            
            // not localizable CLR types
            s_definedAttributes.Add(typeof(Boolean),   notReadable);
            s_definedAttributes.Add(typeof(Byte),      notReadable);
            s_definedAttributes.Add(typeof(SByte),     notReadable);
            s_definedAttributes.Add(typeof(Char),      notReadable);
            s_definedAttributes.Add(typeof(Decimal),   notReadable);
            s_definedAttributes.Add(typeof(Double),    notReadable);            
            s_definedAttributes.Add(typeof(Single),    notReadable);            
            s_definedAttributes.Add(typeof(Int32),     notReadable);            
            s_definedAttributes.Add(typeof(UInt32),    notReadable);            
            s_definedAttributes.Add(typeof(Int64),     notReadable);
            s_definedAttributes.Add(typeof(UInt64),    notReadable);            
            s_definedAttributes.Add(typeof(Int16),     notReadable);            
            s_definedAttributes.Add(typeof(UInt16),    notReadable);    
            s_definedAttributes.Add("PresentationFramework:Set", notReadable);
            s_definedAttributes.Add(typeof(Uri),       notModifiable);
        }
        /// <summary>
        /// 
        /// </summary>
        private DefaultAttributes(){}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static LocalizabilityAttribute GetDefaultAttribute(object type)
        {
            if (s_definedAttributes.ContainsKey(type))
            {
               LocalizabilityAttribute predefinedAttribute = s_definedAttributes[type];

               // create a copy of the predefined attribute and return the copy
               LocalizabilityAttribute result = new LocalizabilityAttribute(predefinedAttribute.Category);
               result.Readability   = predefinedAttribute.Readability;
               result.Modifiability = predefinedAttribute.Modifiability;
               return result;
            }
            else
            {            
                Type targetType = type as Type;
                if ( targetType != null && targetType.IsValueType)
                {
                    // it is looking for the default value of a value type (i.e. struct and enum)
                    // we use this default.
                    LocalizabilityAttribute attribute = new LocalizabilityAttribute(LocalizationCategory.Inherit);
                    attribute.Modifiability           = Modifiability.Unmodifiable;
                    return attribute;                    
                }
                else
                {    
                    return DefaultAttribute;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal static LocalizabilityAttribute DefaultAttribute
        {
            get 
            {
                return new LocalizabilityAttribute(LocalizationCategory.Inherit);
            }            
        }   
        /// <summary>
        /// stores pre-defined attribute for types
        /// </summary>
        private static Dictionary<object, LocalizabilityAttribute> s_definedAttributes;
    }
}
