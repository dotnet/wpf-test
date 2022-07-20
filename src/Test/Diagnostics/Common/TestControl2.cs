// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Markup;

namespace XamlSourceInfoTest.Debug
{
    /// <summary>
    /// Interaction logic for TestControl2.xaml
    /// </summary>
    [ContentProperty("TestContent")]
    public class TestControl2 : UIElement
    {
        public static readonly DependencyProperty TestContentProperty = DependencyProperty.Register(
            "TestContent", typeof(DependencyObject), typeof(TestControl2));

        public TestControl2()
        {
        }

        public DependencyObject TestContent
        {
            get
            {
                return (DependencyObject)this.GetValue(TestContentProperty);
            }
            set
            {
                this.SetValue(TestContentProperty, value);
            }
        }
    }
}
