// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;

namespace RegressionIssue45
{
    /// <summary>
    /// Interaction logic for UserControl2.xaml
    /// </summary>
    public partial class UserControl2 : UserControl
    {
        public UserControl2()
        {
            InitializeComponent();
        }

        public string[] Tests2
        {
            get { return (string[])GetValue(Tests2Property); }
            set { SetValue(Tests2Property, value); }
        }

        // Using a DependencyProperty as the backing store for Tests2.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Tests2Property =
            DependencyProperty.Register("Tests2", typeof(string[]), typeof(UserControl2), new UIPropertyMetadata(null));
    }
}
