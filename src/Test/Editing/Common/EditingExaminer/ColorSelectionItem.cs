// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************************
 *
 * Description: ColorSelectionItem.cs implement the immediate window with the following
 * features:
 *      1. Define the ColorComboBoxItem.
 *
 *******************************************************************************/
#region Using directives

using System;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Collections;
using System.Globalization;

#endregion

namespace EditingExaminer
{
    /// <summary>
    /// Define a item for chosing text color
    /// </summary>
    public class ColorComboBoxItem : System.Windows.Controls.ListBoxItem, IFontChooserComboBoxItem
    {
        private SolidColorBrush _brush;
        private string          _colorName;

        public ColorComboBoxItem(string ColorName, SolidColorBrush brush)
        {
            _brush = brush;

            StackPanel panel = new StackPanel();
            panel.Height       = double.NaN; // _height;
            panel.Orientation  = Orientation.Horizontal;

            Rectangle colorSwatch       = new Rectangle();
            colorSwatch.Height          = double.NaN; // _height;
            colorSwatch.Width           = 25;
            colorSwatch.Fill            = brush;
            colorSwatch.StrokeThickness = 1;
            colorSwatch.Stroke          = Brushes.DarkGray;
            colorSwatch.Margin          = new Thickness(1, 1, 1, 1);

            TextBlock colorName = new TextBlock();
            colorName.Text      = "  " + ColorName.Trim();
            colorName.Height    = double.NaN;       // auto height
            colorName.Width     = double.NaN;       // auto width
            colorName.Margin    = new Thickness(1, 1, 1, 1);

            panel.Children.Add(colorSwatch);
            panel.Children.Add(colorName);

            Content = panel;
            _colorName = ColorName;
        }

        /// <summary>
        /// return the name of a color
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _colorName;
        }

        /// <summary>
        /// Return the color as a solid colorBrush
        /// </summary>
        public SolidColorBrush Brush
        {
            get
            {
                return _brush;
            }
        }

        public int CompareWithString(string value)
        {
            return String.Compare(_colorName, value, true, CultureInfo.InvariantCulture);
        }

        public bool RestrictToListedValues()
        {
            return true;     // Only available fonts may be selected
        }
    }
}
