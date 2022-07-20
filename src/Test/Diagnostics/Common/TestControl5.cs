// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace XamlSourceInfoTest
{
    public class TestControl5 : Control
    {
        static TestControl5()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TestControl5), new FrameworkPropertyMetadata(typeof(TestControl5)));
        }
    }
}
