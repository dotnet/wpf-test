// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace RegressionIssue133
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// Verify that mention of an explicit collection on a read-only collection will not compile
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }
    }
}
