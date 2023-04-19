// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// Description: BamlLocalizabilityByReflection class
//

using System;
using System.Windows;
using System.Windows.Markup.Localizer;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System.Security.Permissions;

namespace Microsoft.Test.Globalization
{
    /// <summary>
    /// 
    /// </summary>
    public class BamlLocalizabilityByReflection : BamlLocalizabilityResolver
    {
        /// <summary>
        /// static constructor, initialize the well know assembly names and arrays
        /// </summary>
        static BamlLocalizabilityByReflection()
        {
            s_wellKnownAssemblyNames = new string[] { 
                "presentationcore", 
                "presentationframework",
                "windowsbase",
                "globloader",
                "globtestcases"
             };

            s_wellKnownAssemblies = new Assembly[s_wellKnownAssemblyNames.Length];
        }

        /// <summary>
        /// Take in an optional list of assemblies in addition to the 
        /// default well-known avalon assemblies. The assemblies will be searched first
        /// in order before the well-known avalon assemblies.
        /// </summary>
        /// <param name="assemblies">additinal list of assemblies to search for Type information</param>
        public BamlLocalizabilityByReflection(params Assembly[] assemblies)
        {
            if (assemblies != null)
            {
                // create the table
                _assemblies = new Dictionary<string, Assembly>(assemblies.Length);

                try
                {
                    // Assert security permissions
                    FileIOPermission permobj = new FileIOPermission(PermissionState.None);
                    permobj.AllFiles = FileIOPermissionAccess.PathDiscovery;
                    //CASRemoval:permobj.Assert();

                    for (int i = 0; i < assemblies.Length; i++)
                    {
                        // skip the null ones. 
                        if (assemblies[i] != null)
                        {
                            // index it by the name;
                            _assemblies[assemblies[i].GetName().FullName] = assemblies[i];
                        }
                    }
                }
                finally
                {
                    // revert assert permission
                    //CASRemoval:FileIOPermission.RevertAssert();
                }
            }

            // create the cache for Type here
            _typeCache = new Dictionary<string, Type>(32);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public override ElementLocalizability GetElementLocalizability(
            string assembly,
            string className
            )
        {
            ElementLocalizability loc = new ElementLocalizability();

            Type type = GetType(assembly, className);
            if (type != null)
            {
                // We found the type, now try to get the localizability attribte from the type
                loc.Attribute = GetLocalizabilityFromType(type);
            }


            int index = Array.IndexOf(s_formattedElements, className);
            if (index >= 0)
            {
                loc.FormattingTag = s_formattingTag[index];
            }

            return loc;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="className"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public override LocalizabilityAttribute GetPropertyLocalizability(
            string assembly,
            string className,
            string property
            )
        {
            LocalizabilityAttribute attribute = null;

            Type type = GetType(assembly, className);

            if (type != null)
            {
                // type of the property. The type can be retrieved from CLR property, or Attached property.
                Type clrPropertyType = null, attachedPropertyType = null;

                // we found the type. try to get to the property as Clr property                    
                GetLocalizabilityForClrProperty(
                    property,
                    type,
                    out attribute,
                    out clrPropertyType
                    );

                if (attribute == null)
                {
                    // we didn't find localizability as a Clr property on the type,
                    // try to get the property as attached property
                    GetLocalizabilityForAttachedProperty(
                         property,
                         type,
                         out attribute,
                         out attachedPropertyType
                         );
                }

                if (attribute == null)
                {
                    attribute = (clrPropertyType != null) ?
                          GetLocalizabilityFromType(clrPropertyType)
                        : GetLocalizabilityFromType(attachedPropertyType);
                }
            }

            return attribute;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="formattingTag"></param>
        /// <returns></returns>
        public override string ResolveFormattingTagToClass(
            string formattingTag
            )
        {
            int index = Array.IndexOf(s_formattingTag, formattingTag);
            if (index >= 0)
                return s_formattedElements[index];
            else
                return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public override string ResolveAssemblyFromClass(
            string className
            )
        {
            for (int i = 0; i < s_wellKnownAssemblies.Length; i++)
            {
                if (s_wellKnownAssemblies[i] == null)
                {
                    LoadWellKnownAssembly(i, false, s_wellKnownAssemblyNames[i]);
                }

                if (s_wellKnownAssemblies[i] != null && s_wellKnownAssemblies[i].GetType(className) != null)
                {
                    return s_wellKnownAssemblies[i].GetName().FullName;
                }
            }

            if (_assemblies != null)
            {
                foreach (KeyValuePair<string, Assembly> pair in _assemblies)
                {
                    if (pair.Value.GetType(className) != null)
                    {
                        return pair.Value.GetName().FullName;
                    }
                }
            }

            return null;
        }
        #region Private Methods
        /// <summary>
        /// get the type in a specified assembly
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        private Type GetType(string assemblyName, string className)
        {
            System.Diagnostics.Debug.Assert(className != null, "classname can't be null");
            System.Diagnostics.Debug.Assert(assemblyName != null, "Assembly name can't be null");

            // combine assembly name and class name for unique indexing
            string fullName = assemblyName + ":" + className;
            Type type;

            if (_typeCache.ContainsKey(fullName))
            {
                // we found it in the cache, so just return
                return _typeCache[fullName];
            }

            // we didn't find it in the table. So let's get to the assembly first
            Assembly assembly = null;
            if (_assemblies != null && _assemblies.ContainsKey(assemblyName))
            {
                // find the assembly in the hash table first
                assembly = _assemblies[assemblyName];
            }

            if (assembly == null)
            {
                // we don't find the assembly in the hashtable
                // try to use the default well known assemblies
                int index;
                bool isFullName;
                if ((index = Array.BinarySearch(s_wellKnownAssemblyNames, GetAssemblyShortName(assemblyName, out isFullName).ToLower())) >= 0)
                {
                    // see if we already loaded the assembly
                    if (s_wellKnownAssemblies[index] == null)
                    {
                        // it is a well known name, load it from the gac
                        LoadWellKnownAssembly(index, isFullName, assemblyName);
                    }

                    assembly = s_wellKnownAssemblies[index];
                }
            }

            if (assembly != null)
            {
                // So, we found the assembly. Now get the type from the assembly
                type = assembly.GetType(className);
            }
            else
            {
                // Couldn't find the assembly. We will try to load it
                type = null;
            }

            // remember what we found out.
            _typeCache[fullName] = type;
            return type;    // return
        }
        /// <summary>
        /// returns the short name for the assembly
        /// </summary>
        /// <param name="assemblyFullName"></param>
        /// <param name="isFullName"></param>
        /// <returns></returns>
        private static string GetAssemblyShortName(string assemblyFullName, out bool isFullName)
        {
            int commaIndex = assemblyFullName.IndexOf(',');
            if (commaIndex > 0)
            {
                isFullName = true;
                return assemblyFullName.Substring(0, commaIndex);
            }

            isFullName = false;
            return assemblyFullName;
        }
        /// <summary>
        /// load the well known assembly
        /// </summary>
        /// <param name="index"></param>
        /// <param name="isFullName"></param>
        /// <param name="assemblyFullName"></param>
        private static void LoadWellKnownAssembly(int index, bool isFullName, string assemblyFullName)
        {
            try
            {
                // assert the necessary permissions to load an assembly 
                FileIOPermission permobj = new FileIOPermission(PermissionState.None);
                permobj.AllLocalFiles = FileIOPermissionAccess.PathDiscovery | FileIOPermissionAccess.Read;
                //CASRemoval:permobj.Assert();

                if (isFullName)
                {
                    s_wellKnownAssemblies[index] = Assembly.Load(assemblyFullName);
                }
                else
                {
                    s_wellKnownAssemblies[index] = Assembly.Load(s_wellKnownAssemblyNames[index]);
                }
            }
            finally
            {                        
            }
        }

        /// <summary>
        /// gets the localizabiity attribute from a given the type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private LocalizabilityAttribute GetLocalizabilityFromType(Type type)
        {
            if (type == null) return null;

            // let's get to its localizability attribute.
            object[] locAttributes = type.GetCustomAttributes(
                s_typeOfLocalizabilityAttribute, // type of localizability
                true                           // search for inherited value
                );

            if (locAttributes.Length == 0)
            {
                return DefaultAttributes.GetDefaultAttribute(type);
            }
            else
            {
                System.Diagnostics.Debug.Assert(locAttributes.Length == 1, "Should have only 1 localizability attribute");

                // use the one defined on the class
                return (LocalizabilityAttribute)locAttributes[0];
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="owner"></param>
        /// <param name="localizability"></param>
        /// <param name="propertyType"></param>
        private void GetLocalizabilityForClrProperty(
            string propertyName,
            Type owner,
            out LocalizabilityAttribute localizability,
            out Type propertyType
            )
        {
            localizability = null;
            propertyType = null;

            PropertyInfo info = owner.GetProperty(propertyName);
            if (info == null)
            {
                return; // couldn't find the Clr property
            }

            // we found the CLR property, set the type of the property
            propertyType = info.PropertyType;

            object[] locAttributes = info.GetCustomAttributes(
                                          s_typeOfLocalizabilityAttribute, // type of the attribute
                true                    // search in base class
                );

            if (locAttributes.Length == 0)
            {
                return;
            }
            else
            {
                System.Diagnostics.Debug.Assert(locAttributes.Length == 1, "Should have only 1 localizability attribute");

                // we found the attribute defined on the property
                localizability = (LocalizabilityAttribute)locAttributes[0];
            }
        }

        /// <summary>
        /// Get localizability for attached property
        /// </summary>
        /// <param name="propertyName">property name</param>
        /// <param name="owner">owner type</param>
        /// <param name="localizability">out: localizability attribute</param>
        /// <param name="propertyType">out: type of the property</param>
        private void GetLocalizabilityForAttachedProperty(
            string propertyName,
            Type owner,
            out LocalizabilityAttribute localizability,
            out Type propertyType
            )
        {
            localizability = null;
            propertyType = null;

            // if it is an attached property, it should have a dependency property with the name 
            // <attached proeprty's name> + "Property"
            DependencyProperty attachedDp = DependencyPropertyFromName(
                propertyName, // property name
                owner
                );       // owner type

            if (attachedDp == null)
                return;  // couldn't find the dp.

            // we found the Dp, set the type of the property
            propertyType = attachedDp.PropertyType;

            FieldInfo fieldInfo = attachedDp.OwnerType.GetField(
                attachedDp.Name + "Property",
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Static | BindingFlags.FlattenHierarchy);

            System.Diagnostics.Debug.Assert(fieldInfo != null);

            object[] attributes = fieldInfo.GetCustomAttributes(
                s_typeOfLocalizabilityAttribute, // type of localizability
                true
                );                // inherit

            if (attributes.Length == 0)
            {
                // didn't find it.
                return;
            }
            else
            {
                System.Diagnostics.Debug.Assert(attributes.Length == 1, "Should have only 1 localizability attribute");
                localizability = (LocalizabilityAttribute)attributes[0];
            }
        }

        private DependencyProperty DependencyPropertyFromName(string propertyName, Type propertyType)
        {
            FieldInfo fi = propertyType.GetField(propertyName + "Property", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            return (fi != null)?fi.GetValue(null) as DependencyProperty:null;
        }
        #endregion
        #region Private Memebers
        /// <summary>
        /// the _assemblies table
        /// </summary>
        private Dictionary<string, Assembly> _assemblies;
        /// <summary>
        /// the types cache
        /// </summary>
        private Dictionary<string, Type> _typeCache;
        /// <summary>
        /// well know assembly names, keep them sorted.
        /// </summary>
        private static string[] s_wellKnownAssemblyNames;
        /// <summary>
        /// the well known assemblies
        /// </summary>
        private static Assembly[] s_wellKnownAssemblies;
        private static Type s_typeOfLocalizabilityAttribute = typeof(LocalizabilityAttribute);
        /// <summary>
        /// supported elements that are formatted inline
        /// </summary>
        private static string[] s_formattedElements = new string[]{
            "System.Windows.Documents.Bold",
            "System.Windows.Documents.Hyperlink", 
            "System.Windows.Documents.Italic",
            "System.Windows.Documents.SmallCaps",
            "System.Windows.Documents.Subscript",
            "System.Windows.Documents.Superscript",
            "System.Windows.Documents.Underline",
            "System.Windows.Documents.Inline"
           };
        /// <summary>
        /// corresponding tag
        /// </summary>
        private static string[] s_formattingTag = new string[] {
            "b",
            "a",
            "i",
            "small",
            "sub",
            "sup",
            "u",
            "in"
           };
        #endregion
    }
}
