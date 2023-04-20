// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace RegressionIssue61_CrossRefWindowScope
{
    public class LinkedControl : ContentControl
    {
        static LinkedControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LinkedControl), new FrameworkPropertyMetadata(typeof(LinkedControl)));
        }

        public static readonly DependencyProperty NextProperty = DependencyProperty.Register("Next", typeof(ContentControl), typeof(LinkedControl));

        public ContentControl Next
        {
            get { return (ContentControl)GetValue(NextProperty); }
            set { SetValue(NextProperty, value); }
        }
    }
}
