// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Test.Logging;

namespace Avalon.Test.ComponentModel.RegressionIssue135
{
    public class Int32CollectionControl : System.Windows.Controls.Control
    {
        static Int32CollectionControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Int32CollectionControl), new FrameworkPropertyMetadata(typeof(Int32CollectionControl)));
        }

        public Int32CollectionControl()
        {
            Loaded += new RoutedEventHandler(Int32CollectionControl_Loaded);
        }

        private void Int32CollectionControl_Loaded(object sender, RoutedEventArgs e)
        {
            GlobalLog.LogStatus("Int32CollectionControl loaded");
            foreach (DependencyObject depObj
                in LogicalTreeHelper.GetChildren(this.Parent))
            {
                Button b = depObj as Button;
                if (b == null)
                {
                    continue;
                }
                Int32Collection ints = (Int32Collection)depObj.GetValue(Int32CollectionControl.IntsProperty);
                if (ints == null)
                {
                    continue;
                }
                b.Content = ints.ToString();
            }
        }

        #region Ints
        public static void SetInts(UIElement element, Int32Collection value)
        {
            element.SetValue(IntsProperty, value);
        }
        public static Int32Collection GetInts(UIElement element)
        {
            return (Int32Collection)element.GetValue(IntsProperty);
        }

        public Int32Collection Ints
        {
            get { return (Int32Collection)GetValue(IntsProperty); }
            set { SetValue(IntsProperty, value); }
        }        

        public static readonly DependencyProperty IntsProperty =
        DependencyProperty.RegisterAttached("Ints", typeof(Int32Collection), typeof(Int32CollectionControl));       

        #endregion Ints

    }
}
