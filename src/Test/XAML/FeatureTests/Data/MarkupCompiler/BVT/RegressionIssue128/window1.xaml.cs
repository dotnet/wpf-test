// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: System.Windows.Markup.XmlnsDefinition("http://myprojectnamespace", "RegressionIssue128")]
namespace RegressionIssue128
{
    using System.Windows.Media;
    using System;
    using System.Windows;
    using Microsoft.Test.Logging;

    public class StaticHolder
    {
        public static readonly Brush RedBrush =
        new SolidColorBrush(Color.FromRgb(byte.MaxValue, 0, 0));
    }

    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }
    }
}
