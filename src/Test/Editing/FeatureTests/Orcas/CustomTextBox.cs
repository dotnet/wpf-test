// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;

using System.Windows;
using System.Windows.Controls;

namespace Test.Uis.TextEditing
{
    /// <summary>A custom TextBox implementation</summary>
    public class CustomTextBox : Control
    {
        static CustomTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomTextBox), new FrameworkPropertyMetadata(typeof(CustomTextBox)));
        }

        #region Dependency Properties

        /// <summary>Text dependency property</summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(CustomTextBox),
                new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>WrapWidth property</summary>
        public static readonly DependencyProperty WrapWidthProperty =
            DependencyProperty.Register(
                "WrapWidth",
                typeof(double),
                typeof(CustomTextBox),
                new FrameworkPropertyMetadata(250.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        #endregion

        #region Public Properties

        /// <summary>Contents of the TextBox</summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>WrapWidth of TextBox</summary>
        public double WrapWidth
        {
            get { return (double)GetValue(WrapWidthProperty); }
            set { SetValue(WrapWidthProperty, value); }
        }

        #endregion

        /// <summary>
        /// Canvas always measures us without constraint and then calls arrange with our DesiredSize 
        /// which we get from our template which is just a text box.
        /// </summary>
        protected override Size MeasureOverride(Size constraint)
        {
            constraint.Width = WrapWidth;
            Size size = base.MeasureOverride(constraint);
            return size;
        }
    }
}
