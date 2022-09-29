// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using System.IO; // FileStream
using System.Xml; // XmlTextReader
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Documents;
using System.Windows.Threading;

using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Microsoft.Win32;

namespace DrtAnnotations
{
    /// <summary>
    /// Class that exposes DPs that represent the formatting state of the 
    /// current selection.  These properties make it easy to detect and alter
    /// the state of certain formatting properties for the current selection.
    /// This code was borrowed almost completely from LexiconEditor.cs in the
    /// HLS/Lexicon folder.
    /// </summary>
    public class StickyNoteRichTextBox : RichTextBox
    {
        public StickyNoteRichTextBox()
            : base()
        {
            this.TextChanged += new TextChangedEventHandler(OnTextChanged);
        }

        // Selection we are registered on
        private TextSelection _selection = null;

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_selection == this.Selection)
                return;

            if (_selection != null)
                _selection.Changed -= new EventHandler(OnSelectionChanged);

            if (this.Selection != null)
            {
                _selection = this.Selection;
                this.Selection.Changed += new EventHandler(OnSelectionChanged);
                this.OnSelectionChanged(null, null);
            }
        }

        // Handler for selection change notification - schedules a job for controls updating
        private void OnSelectionChanged(object sender, EventArgs e)
        {
            // Schedule a job for controls updating
            if (!_updatePropertiesPending)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new DispatcherOperationCallback(UpdateSelectionProperties),
                    null);
                _updatePropertiesPending = true;
            }
        }

        // Worker for the job updating conntrols
        private object UpdateSelectionProperties(object arg)
        {
            object value;

            value = this.Selection.GetPropertyValue(TextElement.FontWeightProperty);
            this.SelectionIsBold = value == DependencyProperty.UnsetValue ? (bool?)null : ((FontWeight)value == FontWeights.Bold);

            value = this.Selection.GetPropertyValue(TextElement.FontStyleProperty);
            this.SelectionIsItalic = value == DependencyProperty.UnsetValue ? (bool?)null : ((FontStyle)value == FontStyles.Italic);

            value = this.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            this.SelectionIsUnderline = value == DependencyProperty.UnsetValue ? (bool?)null : value != null && System.Windows.TextDecorations.Underline.Equals(value);

            value = this.Selection.GetPropertyValue(TextElement.FontFamilyProperty);
            this.SelectionFontFamily = value == DependencyProperty.UnsetValue ? null : value.ToString();

            value = this.Selection.GetPropertyValue(TextElement.FontSizeProperty);
            this.SelectionFontSize = value == DependencyProperty.UnsetValue ? null : value.ToString();

            // Bullets and Numbering
            Paragraph startParagraph = this.Selection.Start.Paragraph;
            Paragraph endParagraph = this.Selection.End.Paragraph;
            if (startParagraph != null && endParagraph != null &&
                (startParagraph.Parent is ListItem) && (endParagraph.Parent is ListItem) &&
                ((ListItem)startParagraph.Parent).List == ((ListItem)endParagraph.Parent).List)
            {
                TextMarkerStyle markerStyle = ((ListItem)startParagraph.Parent).List.MarkerStyle;
                this.SelectionIsBullets = (
                    markerStyle == TextMarkerStyle.Disc ||
                    markerStyle == TextMarkerStyle.Circle ||
                    markerStyle == TextMarkerStyle.Square ||
                    markerStyle == TextMarkerStyle.Box);
                this.SelectionIsNumbering = (
                    markerStyle == TextMarkerStyle.LowerRoman ||
                    markerStyle == TextMarkerStyle.UpperRoman ||
                    markerStyle == TextMarkerStyle.LowerLatin ||
                    markerStyle == TextMarkerStyle.UpperLatin ||
                    markerStyle == TextMarkerStyle.Decimal);
            }
            else
            {
                this.SelectionIsBullets = EditingCommands.ToggleBullets.CanExecute(null, this) ? (bool?)false : (bool?)null;
                this.SelectionIsNumbering = EditingCommands.ToggleNumbering.CanExecute(null, this) ? (bool?)false : (bool?)null;
            }

            _updatePropertiesPending = false;
            return null;
        }

        private bool _updatePropertiesPending;

        // SelectionIsBold
        // ---------------
        public static readonly DependencyProperty SelectionIsBoldProperty = DependencyProperty.Register(
            "SelectionIsBold", typeof(bool?), typeof(StickyNoteRichTextBox),
            new FrameworkPropertyMetadata(
                (bool?)false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(OnSelectionIsBoldPropertyChanged), null));

        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool? SelectionIsBold { get { return (bool?)GetValue(SelectionIsBoldProperty); } set { SetValue(SelectionIsBoldProperty, value); } }

        private static void OnSelectionIsBoldPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StickyNoteRichTextBox StickyNoteRichTextBox = (StickyNoteRichTextBox)d;
            if (StickyNoteRichTextBox._updatePropertiesPending) return;
            bool? value = (bool?)e.NewValue;
            if (value != null)
            {
                StickyNoteRichTextBox.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, (value == true) ? FontWeights.Bold : FontWeights.Normal);
                StickyNoteRichTextBox.OnSelectionChanged(null, null);
            }
        }

        // SelectionIsItalic
        // -----------------
        public static readonly DependencyProperty SelectionIsItalicProperty = DependencyProperty.Register(
            "SelectionIsItalic", typeof(bool?), typeof(StickyNoteRichTextBox),
            new FrameworkPropertyMetadata(
                (bool?)false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(OnSelectionIsItalicPropertyChanged), null));

        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool? SelectionIsItalic { get { return (bool?)GetValue(SelectionIsItalicProperty); } set { SetValue(SelectionIsItalicProperty, value); } }

        private static void OnSelectionIsItalicPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StickyNoteRichTextBox StickyNoteRichTextBox = (StickyNoteRichTextBox)d;
            if (StickyNoteRichTextBox._updatePropertiesPending) return;
            bool? value = (bool?)e.NewValue;
            if (value != null)
            {
                StickyNoteRichTextBox.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, (value == true) ? FontStyles.Italic : FontStyles.Normal);
                StickyNoteRichTextBox.OnSelectionChanged(null, null);
            }
        }

        // SelectionIsUnderline
        // --------------------
        public static readonly DependencyProperty SelectionIsUnderlineProperty = DependencyProperty.Register(
            "SelectionIsUnderline", typeof(bool?), typeof(StickyNoteRichTextBox),
            new FrameworkPropertyMetadata(
                (bool?)false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(OnSelectionIsUnderlinePropertyChanged), null));

        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool? SelectionIsUnderline { get { return (bool?)GetValue(SelectionIsUnderlineProperty); } set { SetValue(SelectionIsUnderlineProperty, value); } }

        private static void OnSelectionIsUnderlinePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StickyNoteRichTextBox StickyNoteRichTextBox = (StickyNoteRichTextBox)d;
            if (StickyNoteRichTextBox._updatePropertiesPending) return;
            bool? value = (bool?)e.NewValue;
            if (value != null)
            {
                StickyNoteRichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, (value == true) ? System.Windows.TextDecorations.Underline : null);
                StickyNoteRichTextBox.OnSelectionChanged(null, null);
            }
        }

        // SelectionFontFamily
        // -------------------
        public static readonly DependencyProperty SelectionFontFamilyProperty = DependencyProperty.Register(
            "SelectionFontFamily", typeof(string), typeof(StickyNoteRichTextBox),
            new FrameworkPropertyMetadata(
                null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(OnSelectionFontFamilyPropertyChanged), null));

        [Bindable(true), Browsable(false), Category("Appearance")]
        public string SelectionFontFamily { get { return (string)GetValue(SelectionFontFamilyProperty); } set { SetValue(SelectionFontFamilyProperty, value); } }

        private static void OnSelectionFontFamilyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StickyNoteRichTextBox StickyNoteRichTextBox = (StickyNoteRichTextBox)d;
            if (StickyNoteRichTextBox._updatePropertiesPending) return;
            string value = (string)e.NewValue;
            if (value != null)
            {
                StickyNoteRichTextBox.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, value);
                StickyNoteRichTextBox.OnSelectionChanged(null, null);
            }
        }

        // SelectionFontSize
        // -----------------
        public static readonly DependencyProperty SelectionFontSizeProperty = DependencyProperty.Register(
            "SelectionFontSize", typeof(string), typeof(StickyNoteRichTextBox),
            new FrameworkPropertyMetadata(
                null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(OnSelectionFontSizePropertyChanged), null));

        [Bindable(true), Browsable(false), Category("Appearance")]
        public string SelectionFontSize { get { return (string)GetValue(SelectionFontSizeProperty); } set { SetValue(SelectionFontSizeProperty, value); } }

        private static void OnSelectionFontSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StickyNoteRichTextBox StickyNoteRichTextBox = (StickyNoteRichTextBox)d;
            if (StickyNoteRichTextBox._updatePropertiesPending) return;
            string value = (string)e.NewValue;
            if (value != null)
            {
                StickyNoteRichTextBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, value);
                StickyNoteRichTextBox.OnSelectionChanged(null, null);
            }
        }


        // SelectionIsBullets
        // ------------------
        public static readonly DependencyProperty SelectionIsBulletsProperty = DependencyProperty.Register(
            "SelectionIsBullets", typeof(bool?), typeof(StickyNoteRichTextBox),
            new FrameworkPropertyMetadata(
                (bool?)false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(OnSelectionIsBulletsPropertyChanged), null));

        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool? SelectionIsBullets { get { return (bool?)GetValue(SelectionIsBulletsProperty); } set { SetValue(SelectionIsBulletsProperty, value); } }

        private static void OnSelectionIsBulletsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StickyNoteRichTextBox StickyNoteRichTextBox = (StickyNoteRichTextBox)d;
            if (StickyNoteRichTextBox._updatePropertiesPending) return;
            bool? value = (bool?)e.NewValue;
            if (value != null)
            {
                EditingCommands.ToggleBullets.Execute(null, StickyNoteRichTextBox);
                StickyNoteRichTextBox.OnSelectionChanged(null, null);
            }
        }

        // SelectionIsNumbering
        // --------------------
        public static readonly DependencyProperty SelectionIsNumberingProperty = DependencyProperty.Register(
            "SelectionIsNumbering", typeof(bool?), typeof(StickyNoteRichTextBox),
            new FrameworkPropertyMetadata(
                (bool?)false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(OnSelectionIsNumberingPropertyChanged), null));

        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool? SelectionIsNumbering { get { return (bool?)GetValue(SelectionIsNumberingProperty); } set { SetValue(SelectionIsNumberingProperty, value); } }

        private static void OnSelectionIsNumberingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StickyNoteRichTextBox StickyNoteRichTextBox = (StickyNoteRichTextBox)d;
            if (StickyNoteRichTextBox._updatePropertiesPending) return;
            bool? value = (bool?)e.NewValue;
            if (value != null)
            {
                EditingCommands.ToggleNumbering.Execute(null, StickyNoteRichTextBox);
                StickyNoteRichTextBox.OnSelectionChanged(null, null);
            }
        }

        // Helper used in all boolean SelectionProperties
        private void OnBoolSelectionPropertyInvalidated(DependencyProperty selectionProperty, DependencyProperty formattingProperty)
        {
            if (_updatePropertiesPending)
            {
                return;
            }
            bool? value = (bool?)this.GetValue(selectionProperty);
            if (value != null)
            {
                this.Selection.ApplyPropertyValue(formattingProperty, (bool)value);
                this.OnSelectionChanged(null, null);
            }
        }
    }
}
