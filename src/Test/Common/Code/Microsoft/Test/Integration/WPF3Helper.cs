// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace Microsoft.Test.Integration
{
    /// <summary>
    /// 
    /// </summary>
	internal static class WPF3Helper
	{
        static WPF3Helper()
        {
            // List of assemblies used by default for finding types if
            // no assembly is specified in test definition.
            _defaultAssemblies = new List<Assembly>();

            _defaultAssemblies.Add(typeof(FrameworkElement).Assembly);
            _defaultAssemblies.Add(typeof(UIElement).Assembly);
            _defaultAssemblies.Add(typeof(DispatcherObject).Assembly); 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Type FindType(string typeName)
        {
            foreach (Assembly assembly in _defaultAssemblies)
            {
                Type[] types = assembly.GetTypes();

                foreach (Type currentType in types)
                {
                    if (String.Compare(currentType.FullName, typeName, false) == 0)
                    {
                        return currentType;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public static Assembly[] Assemblies
        {
            get
            {
                return _defaultAssemblies.ToArray();
            }
        }


        static List<Assembly> _defaultAssemblies = null;

	}
}
