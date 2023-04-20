// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.MethodTests.Mapper
{
    public class MarkupExtensionReturnTypeTest
    {
        private Dictionary<Type, Type> _returnTypeMap = new Dictionary<Type,Type>();

        /// <summary>
        /// Initializes a map from MarkupExtension to ReturnType.
        /// </summary>
        public MarkupExtensionReturnTypeTest()
        {
            _returnTypeMap.Add(typeof(ResourceKey), typeof(ResourceKey));
            _returnTypeMap.Add(typeof(BindingBase), typeof(Object));
            _returnTypeMap.Add(typeof(Binding), typeof(Object));
            _returnTypeMap.Add(typeof(MultiBinding), typeof(Object));
            _returnTypeMap.Add(typeof(PriorityBinding), typeof(Object));
            _returnTypeMap.Add(typeof(RelativeSource), typeof(RelativeSource));
            _returnTypeMap.Add(typeof(TemplateKey), typeof(ResourceKey));
            _returnTypeMap.Add(typeof(DataTemplateKey), typeof(ResourceKey));
            _returnTypeMap.Add(typeof(DynamicResourceExtension), typeof(Object));
            _returnTypeMap.Add(typeof(ColorConvertedBitmapExtension), typeof(System.Windows.Media.Imaging.ColorConvertedBitmap));
            _returnTypeMap.Add(typeof(StaticResourceExtension), typeof(Object));
            _returnTypeMap.Add(typeof(ArrayExtension), typeof(Array));
            _returnTypeMap.Add(typeof(NullExtension), typeof(Object));
            _returnTypeMap.Add(typeof(StaticExtension), typeof(Object));
            _returnTypeMap.Add(typeof(TypeExtension), typeof(Type));
            _returnTypeMap.Add(typeof(ThemeDictionaryExtension), typeof(Uri));
        }

        public void RunTest() 
        {
            List<Type> types = GetAssemblyTypes(typeof(FrameworkElement).Assembly);

            foreach (Type type in types)
            {
                foreach(System.Attribute attrib in TypeDescriptor.GetAttributes(type))
                {
                    MarkupExtensionReturnTypeAttribute markupAttrib = 
                        attrib as MarkupExtensionReturnTypeAttribute;

                    if (markupAttrib != null)
                    {
                        TestLog.Current.LogStatus(type.Name + " -- ReturnType: " + markupAttrib.ReturnType);

                        if (_returnTypeMap.ContainsKey(type))
                        {
                            Type returnType = _returnTypeMap[type];

                            if (returnType != markupAttrib.ReturnType) throw new Microsoft.Test.TestValidationException("Failed");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the given assembly's types. 
        /// Catches ReflectionTypeLoadException's, and returns any types that didn't cause an exception.
        /// That is necessary because an InheritanceDemand on some Avalon types which are overridden on 
        /// our test types causes a security exception for the types that are in partial trust assemblies.
        /// </summary>
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
        public static List<Type> GetAssemblyTypes(Assembly assembly)
        {
            Type[] types = null;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types;
            }

            List<Type> typeList = new List<Type>();
            typeList.AddRange(types);

            return typeList;
        }
    }

}
