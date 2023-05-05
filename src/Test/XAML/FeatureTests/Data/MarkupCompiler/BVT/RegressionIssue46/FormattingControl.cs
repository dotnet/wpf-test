// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RegressionIssue46
{
    /// <summary>
    /// A control that applies a format string to a set of controls. Text in the
    /// format string turns in to TextBlocks with a specified TextStyle applied to
    /// it.
    /// 
    /// Example: <!--
    /// <FormattingControl FormatString="Some {0} Text {1} is here">
    ///     <FormattingControl.Template>
    ///         <ControlTemplate><WrapPanel x:Name="PART_Panel"/></ControlTemplate>
    ///     </FormattingControl.Template>
    ///     <ComboBox/>
    ///     <ComboBox/>
    /// </FormattingControl> -->
    /// creates a WrapPanel that contains 5 elements, TextBlocks for each range of "Some ",
    /// " Text ", and "is here" with the two ComboBoxes between them.
    /// 
    /// The format string is a simplified version of the .NET Framework format string:
    /// * The only formatting allowed is the integer specifying the child to insert here.
    /// * You can only specify any number once. E.g. "This {0} is {0}" is invalid.
    /// * The only escape code is {{, specifying an open brace. 
    /// * The integer is in the InvariantCulture.
    /// * Embedded empty braces {} are stripped out.
    /// </summary>
    [Localizability(LocalizationCategory.Text)]
    [System.Windows.Markup.ContentProperty("Items")]
    public class FormattingControl : Control
    {
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(Collection<UIElement>), typeof(FormattingControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty TextStyleProperty = DependencyProperty.Register("TextStyle", typeof(Style), typeof(FormattingControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty FormatStringProperty = DependencyProperty.Register("FormatString", typeof(string), typeof(FormattingControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
        
        private Collection<UIElement> _items;

        public Collection<UIElement> Items
        {
            get
            {
                if (this._items == null)
                {
                    this._items = new Collection<UIElement>();
                    this.OnItemsChanged();
                }
                return this._items;
            }
        }

        public Style TextStyle
        {
            get { return (Style)this.GetValue(FormattingControl.TextStyleProperty); }
            set { this.SetValue(FormattingControl.TextStyleProperty, value); }
        }

        public string FormatString
        {
            get { return (string)this.GetValue(FormattingControl.FormatStringProperty); }
            set { this.SetValue(FormattingControl.FormatStringProperty, value); }
        }

        private void OnItemsChanged()
        {
            this.UpdatePanel();
        }

        public override void OnApplyTemplate()
        {
            this.UpdatePanel();
            base.OnApplyTemplate();
        }

        private void UpdatePanel()
        {
            Panel panel = this.GetTemplateChild("PART_Panel") as Panel;
            if (panel == null)
            {
                return;
            }

            List<UIElement> formattedList = new List<UIElement>();

            Format(this.FormatString, this.Items, this.TextStyle, formattedList);

            panel.Children.Clear();

            foreach (UIElement item in formattedList)
            {
                panel.Children.Add(item);
            }
        }

        internal static void Format(string formatString, Collection<UIElement> items, Style textStyle, List<UIElement> formattedList)
        {
            StringBuilder bufferedText = new StringBuilder();

            for (int i = 0; i < formatString.Length; ++i)
            {
                if (formatString[i] != '{')
                {
                    bufferedText.Append(formatString[i]);
                }
                else
                {
                    ++i;
                    if (i < formatString.Length && formatString[i] == '{')
                    {
                        bufferedText.Append('{');
                    }
                    else if (i < formatString.Length && formatString[i] == '}')
                    {
                        // Ignore {}
                    }
                    else
                    {
                        Emit(bufferedText, textStyle, formattedList);
                        bufferedText = new StringBuilder();

                        StringBuilder tokenBuilder = new StringBuilder();
                        for (; i < formatString.Length && formatString[i] != '}'; ++i)
                        {
                            tokenBuilder.Append(formatString[i]);
                        }

                        string token = tokenBuilder.ToString();

                        int itemIndex = int.Parse(token, CultureInfo.InvariantCulture);
                        formattedList.Add(items[itemIndex]);
                    }
                }
            }
            Emit(bufferedText, textStyle, formattedList);
        }

        private static void Emit(StringBuilder text, Style textStyle, List<UIElement> formattedList)
        {
            if (text.Length != 0)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = text.ToString();
                textBlock.Style = textStyle;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                formattedList.Add(textBlock);
            }
        }
    }
}
