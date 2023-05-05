// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using Microsoft.Test.Logging;

namespace ReflectTypeForwardedTypes
{
    public static class Program
    {
        /// <summary>
        /// This test makes sure that you can reflect on the forwareded types
        /// in windows base and you should get the types automatically from System.Xaml
        /// </summary>
        public static int Main()
        {

            List<string> forwardedTypes = new List<string>()
            {
                "System.Windows.Markup.ValueSerializer",
                "System.Windows.Markup.ArrayExtension",
                "System.Windows.Markup.DateTimeValueSerializer",
                "System.Windows.Markup.IComponentConnector",
                "System.Windows.Markup.INameScope",
                "System.Windows.Markup.IProvideValueTarget",
                "System.Windows.Markup.IUriContext",
                "System.Windows.Markup.IValueSerializerContext",
                "System.Windows.Markup.IXamlTypeResolver",
                "System.Windows.Markup.MarkupExtension",
                "System.Windows.Markup.NullExtension",
                "System.Windows.Markup.StaticExtension",
                "System.Windows.Markup.TypeExtension",
                "System.Windows.Markup.AmbientAttribute",
                "System.Windows.Markup.UsableDuringInitializationAttribute",
                "System.Windows.Markup.ConstructorArgumentAttribute",
                "System.Windows.Markup.ContentPropertyAttribute",
                "System.Windows.Markup.ContentWrapperAttribute",
                "System.Windows.Markup.DependsOnAttribute",
                "System.Windows.Markup.DictionaryKeyPropertyAttribute",
                "System.Windows.Markup.MarkupExtensionReturnTypeAttribute",
                "System.Windows.Markup.NameScopePropertyAttribute",
                "System.Windows.Markup.RootNamespaceAttribute",
                "System.Windows.Markup.TrimSurroundingWhitespaceAttribute",
                "System.Windows.Markup.UidPropertyAttribute",
                // Removing this check because ValueSerializerAttribute has been moved into System.dll in .NET 4.5 
                // System.Windows.Xaml.ValueSerializerAttribute forwarded type not resolved in System.xaml.dll
                // "System.Windows.Markup.ValueSerializerAttribute",
                "System.Windows.Markup.WhitespaceSignificantCollectionAttribute",
                "System.Windows.Markup.XmlLangPropertyAttribute",
                "System.Windows.Markup.XmlnsCompatibleWithAttribute",
                "System.Windows.Markup.XmlnsDefinitionAttribute",
                "System.Windows.Markup.XmlnsPrefixAttribute",
                "System.Windows.Markup.RuntimeNamePropertyAttribute"
            };
            Assembly windowsBase = typeof(DependencyObject).Assembly;

            bool fail = false;
            foreach (string type in forwardedTypes)
            {
                Type resolvedType = windowsBase.GetType(type);
                if (resolvedType == null)
                {
                    GlobalLog.LogEvidence(type + " resolved to null");
                    fail = true;
                }
                if (!resolvedType.Assembly.FullName.Contains("System.Xaml"))
                {
                    GlobalLog.LogEvidence(type + " not resolved from System.Xaml.dll");
                    fail = true;
                }

                Console.WriteLine(resolvedType.ToString());
                
            }
             
            if(fail)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
