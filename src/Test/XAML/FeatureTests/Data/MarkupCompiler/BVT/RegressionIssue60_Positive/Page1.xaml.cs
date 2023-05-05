// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfApplication3
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        public Page1()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            bool b = (bool)grid1.GetValue(AttachedDPs.AttachedDP1);
            if (b != true)
            {
                Console.WriteLine("Value of attached property is not true");
                Application.Current.Shutdown(1);
            }

            Console.WriteLine("Value of attached property is true");
            Application.Current.Shutdown(0);
        }
    }

    public static class AttachedDPs
    {
        public static readonly DependencyProperty AttachedDP1 = DependencyProperty.RegisterAttached("MyAttachedDP", typeof(bool), typeof(AttachedDPs));

        internal static void SetAttachedDP1(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(AttachedDP1, value);
        }
    };
}
