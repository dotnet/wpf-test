// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace WFCTestLib.Util
{
    // <doc>
    // <desc>
    //  Provides useful utility methods for use with enums.
    // </desc>
    // </doc>
    public class EnumTools
    {
        // <doc>
        // <desc>
        //  Some unique value to indicate an error (distinguish from actual value)
        // </desc>
        // </doc>
        public const int ENUM_ERROR = 0xbadf00d;

        // <doc>
        // <desc>
        //  Returns the number of fields in the specified enum class.
        // <desc>
        // <param term="enumClass">
        //  The type of enum whose length we want to determine
        // </param>
        // <retvalue>
        //  number of fields.  Returns 0 if enumClass is not an enum
        // </retvalue>
        // </doc>
        public static int GetEnumLength (Type enumClass)
        {
            // If this isn't an enum class, return zero
            if (!enumClass.IsEnum)
                return 0;

            return enumClass.GetFields().Length;
        }

        // <doc>
        // <desc>
        //  Returns a string representation of the value in the specified enum
        // </desc>
        // <param term="e">
        //  The enum class
        // </param>
        // <param term="value">
        //  The value in the enum whose string value we want to return
        // </param>
        // <retvalue>
        //  representation (like what you'd see in the designer) of the e value
        // </retvalue>
        // </doc>
        public static String GetEnumStringFromValue(Type e, Int32 value)
        {
            if (!e.IsEnum)
                return e.ToString() + " not a valid enum type";
            
            TypeConverter enumEditor = TypeDescriptor.GetConverter(e);
            //ValueEditor enumEditor = TypeDescriptor.TempCreateEditorHack(e,null);    
            //ValueEditor enumEditor = ComponentDescriptor.CreateValueEditor(e);
            Object o = enumEditor.ConvertToString(value);

            if (o != null)
                return o.ToString();

            return(value.ToString() + " invalid for " + e.ToString());
        }

        // <doc>
        // <desc>
        //  Returns integer value corresponding to string representation of the
        //  value for the specified class.
        // </desc>
        // <param term="e">
        //  The enum class
        // </param>
        // <param term="s">
        //  String text to get the value for.
        // </param>
        // <retvalue>
        //  The value of the text in the enum. ENUM_ERROR is returned if "s" 
        //  is not a valid Enum name, or if e is not a valid enum type
        // </retvalue>
        // </doc>
        public static int GetEnumValueFromString (Type e, String s)
        {
            if (!e.IsEnum)
                return ENUM_ERROR;
                
            TypeConverter enumEditor = TypeDescriptor.GetConverter(e);
            
            //ValueEditor enumEditor = TypeDescriptor.TempCreateEditorHack(e,null);
            //ValueEditor enumEditor = ComponentDescriptor.CreateValueEditor (e);
            Object value = enumEditor.ConvertFromString(s);

            if ( value.GetType().IsEnum )
                return (Int32)value;

            return ENUM_ERROR;
        }

        // <doc>
        // <desc>
        //  Returns array of strings containing values for specified e
        // <desc>
        // <param term="e">
        //  The type of the enum class to get the string values for.
        // </param>
        // <retvalue>
        //  An array of strings representing the names of the values in the
        //  specified enum.
        // </retvalue>
        // </doc>
        public static String[] GetEnumStrings (Type e)
        {
            if (!e.IsEnum)
                return null;
               
            TypeConverter enumEditor = TypeDescriptor.GetConverter(e);
                       
            //ValueEditor enumEditor = TypeDescriptor.TempCreateEditorHack(e,null);    
            //ValueEditor enumEditor = ComponentDescriptor.CreateValueEditor (e);
            
            //Object[] values = enumEditor.Values;
                        
            String[] strings = new String [enumEditor.GetStandardValues().Count];
            Object[] enumObjs = new Object[enumEditor.GetStandardValues().Count];
            enumEditor.GetStandardValues().CopyTo(enumObjs,0);

            for (int n=0;  n< enumEditor.GetStandardValues().Count; n++)
            {
                strings[n] = GetEnumStringFromValue(e,(int)enumObjs[n]);
                Console.WriteLine(strings[n]);
            }
            return strings;
        }
    }
}
