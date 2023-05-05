// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Markup;

[assembly: XmlnsDefinition("http://crka", "ComponentResourceKeyAssembly")]
namespace ComponentResourceKeyAssembly
{
    public static class MyClass
    {
        private static ComponentResourceKey s_myKey;
        public static ComponentResourceKey MyKey
        {
            get
            {
                if (s_myKey == null)
                {
                    s_myKey = new ComponentResourceKey(typeof(MyClass), "MyKey");
                }
                return s_myKey;
            }
        }

    }
}
