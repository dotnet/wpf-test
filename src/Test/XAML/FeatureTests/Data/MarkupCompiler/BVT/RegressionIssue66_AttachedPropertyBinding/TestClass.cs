// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace TestLib.OtherNS
{
    /// <summary>
    /// This class contains the attached property we will bind to
    /// </summary>
    public class TestClass : Button
    {
        public static bool GetOtherAttachedProp(DependencyObject obj)
        {
            return (bool)obj.GetValue(OtherAttachedPropProperty);
        }

        public static void SetOtherAttachedProp(DependencyObject obj, bool value)
        {
            obj.SetValue(OtherAttachedPropProperty, value);
        }

        // Using a DependencyProperty as the backing store for OtherAttachedProp.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OtherAttachedPropProperty =
            DependencyProperty.RegisterAttached("OtherAttachedProp", typeof(bool), typeof(TestClass), new UIPropertyMetadata(false));

    }
}
