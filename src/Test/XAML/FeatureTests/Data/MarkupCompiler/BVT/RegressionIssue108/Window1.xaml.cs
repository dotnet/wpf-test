// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace MarkupCompiler.RegressionIssue108
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// Verify if x:Static can reference local types if the assembly name is included in the Clr-Namespace
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if ((String)button1.Content == PublicStaticResource.label1)
            {
                Application.Current.Shutdown(0);
            }
            else
            {
                Application.Current.Shutdown(-1);
            }
        }
    }

    internal class PublicStaticResource
    {
        public static string label1
        {
            get
            {
                return "HelloWorld";
            }
        }
    }
}
