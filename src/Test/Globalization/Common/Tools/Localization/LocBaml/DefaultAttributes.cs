// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
// Description: LocBaml command line tool. 
//

using System;
using System.Windows;
using System.Collections.Generic;

namespace BamlLocalization
{
    /// <summary>
    /// Defines all the static localizability attributes
    /// </summary>
    internal static class DefaultAttributes
    {
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
            s_definedAttributes.Add(typeof(Uri),       notModifiable);
        }   
        
        /// <summary>
        /// Get the localizability attribute for a type
        /// </summary>
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
                    // It is looking for the default value of a value type (i.e. struct and enum)
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

        internal static LocalizabilityAttribute DefaultAttribute
        {
            get 
            {
                return new LocalizabilityAttribute(LocalizationCategory.Inherit);
            }            
        }   

        private static Dictionary<object, LocalizabilityAttribute> s_definedAttributes;     // stores pre-defined attribute for types
    }
}
