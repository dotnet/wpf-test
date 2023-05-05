// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace OtherAssembly
{
    public static class AttachedDPs
    {
        public static readonly DependencyProperty AttachedDP1 = DependencyProperty.RegisterAttached("MyAttachedDP", typeof(bool), typeof(AttachedDPs));

        internal static void SetAttachedDP1(DependencyObject dObj, bool value)
        {
            dObj.SetValue(AttachedDP1, value);
        }
    };
}
