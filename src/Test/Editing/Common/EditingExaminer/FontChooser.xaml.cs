// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************************
 *
 * Description: Fontchooser.xaml.cs implement the immediate window with the following
 * features:
 *      1. implement the font dialog window.
 *
 *******************************************************************************/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Collections;
using System.Globalization;
using System.Resources;
using System.Reflection;


namespace EditingExaminer
{
    /// <summary>
    /// FontChoiceWithFace
    /// </summary>
    class FontChoiceWithFace : FontChoice
    {
        private TypefaceComboBoxItem _face;

        public FontChoiceWithFace(DependencyObject dependencyObject) : base(dependencyObject)
        {
            _face = null;
        }

        public FontChoiceWithFace(FontChoice choice) : base(choice)
        {
            _face = null;
        }

        public TypefaceComboBoxItem TypefaceListItem
        {
            get
            {
                if (   _face    == null
                   || _face.Typeface.FontFamily != FontFamily
                   || _face.Typeface.Style != FontStyle
                   || _face.Typeface.Weight != FontWeight
                   || _face.Typeface.Stretch != FontStretch
                   )
                {
                    _face = new TypefaceComboBoxItem(new Typeface(FontFamily, FontStyle, FontWeight, FontStretch, null));
               }
                return _face;
            }
            set
            {
                _face = value;

                FontFamily = _face.Typeface.FontFamily;
                FontStyle = _face.Typeface.Style;
                FontWeight = _face.Typeface.Weight;
                FontStretch = _face.Typeface.Stretch;
            }
        }
    }

    /// <summary>
    /// Interaction logic for FontChooser.xaml
    /// </summary>
    public partial class FontChooser : Window
    {
        private      FontChoiceWithFace      _choice;
        private      RichTextBox             _richTextBox;
        private      TextSelection           _selection;       // Users selection we're working on, if any
        private      Control                 _controlForHelp   = null;
        public event FontChosenEventHandler  FontChosen;

        private FontFamily _sampleAnnotationFontFamily = null;
        private double     _sampleAnnotationFontSize = 0;
        private string     _sampleAnnotationPointSizeFormatString = null;

        private CultureInfo _descriptiveTextCulture = new CultureInfo("en-US");

        #region public methods
        public FontChooser() : base()
        {
        }

        public FontChooser(RichTextBox richTextBox) : this()
        {
            _richTextBox = richTextBox;
            _selection = richTextBox.Selection;
            InitializeComponent();

            _choice =  new FontChoiceWithFace(_selection.Start.Parent);

            PresetUserInterface();
        }

        public FontChooser(FontChoice choice) : this()
        {
            _selection = null;
            InitializeComponent();

            _choice = new FontChoiceWithFace(choice);

            //disable checkboxes since they cannot be used in plain text mode anyway
            UnderLineCheckBox.IsEnabled     = false;
            StrikethroughCheckBox.IsEnabled = false;
            OverLineCheckBox.IsEnabled      = false;

            PresetUserInterface();
        }
        #endregion

        #region private methods
        void WindowInitialized(object o, EventArgs args)
        {
        }

        private static double PointsToPixels(double value)
        {
            return value * 96.0d / 72.0d;
        }

        private static double PixelsToPoints(double value)
        {
            return value * 72.0d / 96.0d;
        }
        
        void PresetUserInterface()
        {
            // If initial font is a simple composite family, just pick the first real family
            int index = _choice.FontFamily.Source.IndexOf(',');
            if (index > 0)
            {
                _choice.FontFamily = new FontFamily(_choice.FontFamily.Source.Substring(0, index));
            }

            // Pick up the font to use for annotations in samples
            TypefaceSizeSamples.SelectAll();
            _sampleAnnotationPointSizeFormatString = TypefaceSizeSamples.Selection.Text.Trim(null);
            _sampleAnnotationFontFamily = TypefaceSizeSamples.FontFamily;
            _sampleAnnotationFontSize   = TypefaceSizeSamples.FontSize;


            // Copy Selected text (if any) into preview box
            if (_selection != null && !_selection.IsEmpty)
            {
                Preview.Text = _selection.Text;
            }

            PresetFontFamilyCombo(_choice.TypefaceListItem.FamilyName);
            PresetFontSizeCombo(_choice.FontSize);
            PresetFontColorCombo(_choice.Foreground);

            // Apply font formatting to samples
            GenerateFamilyTypefaceSamples();
            SetDecoration();
        }
        
        void GenerateTypefaceSizeSamples()
        {
            Paragraph paragraph;

            TypefaceSizeSamples.Document = new FlowDocument(new Paragraph(new Run()));
            TypefaceSizeSamplesLabel.Text = " " + _choice.TypefaceListItem.ToString();

            // Find the nearest font size to the current value from the list of useful sizes for this dialog
            int middleSize = 0;
            while (middleSize < s_commonlyUsedFontSizes.Length
                && _choice.FontSize > s_commonlyUsedFontSizes[middleSize])
            {
                middleSize++;
            }

            // Adjust middleSize so there are at least 5 sizes before and after
            if (middleSize < 5)
            {
                middleSize = 5;
            }
            else if (middleSize > s_commonlyUsedFontSizes.Length - 5)
            {
                middleSize = s_commonlyUsedFontSizes.Length - 5;
            }

            // Display 10 sizes around the currently selected size
            for (int i = middleSize-5; i < middleSize+5; i++)
            {
                if (i > middleSize - 5)
                {
                    // Insert a small separation before the next sample
                    paragraph = new Paragraph();
                    paragraph.Inlines.Clear();
                    paragraph.Inlines.Add(new Run(""));
                    paragraph.SetValue(Block.LineHeightProperty, _sampleAnnotationFontSize/2);
                    TypefaceSizeSamples.Document.Blocks.Add(paragraph);
                }

                paragraph = new Paragraph();
                paragraph.Margin = new Thickness(0);
                paragraph.Inlines.Clear();
                paragraph.Inlines.Add(new Run(string.Format(_sampleAnnotationPointSizeFormatString, s_commonlyUsedFontSizes[i])));
                paragraph.SetValue(TextElement.FontFamilyProperty,  _sampleAnnotationFontFamily);
                paragraph.SetValue(TextElement.FontSizeProperty,    _sampleAnnotationFontSize);
                paragraph.SetValue(TextElement.FontStyleProperty,   FontStyles.Normal);
                paragraph.SetValue(TextElement.FontWeightProperty,  FontWeights.Normal);
                paragraph.SetValue(TextElement.FontStretchProperty, FontStretches.Normal);
                paragraph.SetValue(TextElement.FontSizeProperty,    PointsToPixels(7));
                TypefaceSizeSamples.Document.Blocks.Add(paragraph);

                paragraph = new Paragraph(new Run("The quick brown fox jumps over the lazy dog"));
                paragraph.Margin = new Thickness(0);
                paragraph.Inlines.Clear();
                paragraph.Inlines.Add(new Run("The quick brown fox jumps over the lazy dog"));
                _choice.ApplyToDependencyObject(paragraph);
                paragraph.SetValue(Block.LineHeightProperty, s_commonlyUsedFontSizes[i]*1.3d); // Allow 30% line gap
                paragraph.SetValue(TextElement.FontSizeProperty, PointsToPixels(s_commonlyUsedFontSizes[i]));
                TypefaceSizeSamples.Document.Blocks.Add(paragraph);
            }

            if (FamilyTypefaceSamples.Document.Blocks.Count > 1)
            {
                TypefaceSizeSamples.Document.Blocks.Remove(TypefaceSizeSamples.Document.Blocks.FirstBlock);
            }
        }

        void GenerateFamilyTypefaceSamples()
        {
            TypefaceListBox.Items.Clear();

            FamilyTypefaceSamples.Document = new FlowDocument(new Paragraph(new Run()));
            FamilyTypefaceSamplesLabel.Text = " " + _choice.FontFamily;

            Paragraph paragraph;
            // bool firstSample = true;
            foreach (TypefaceComboBoxItem faceItem in _choice.TypefaceListItem.SortedTypefaces())
            {
                // Add the typeface to the list of available typefaces

                TypefaceListBox.Items.Add(faceItem);

                // Add an annotation identifying the size

                paragraph = new Paragraph(new Run(faceItem.ToString()));
                paragraph.Margin = new Thickness(0);
                paragraph.Inlines.Clear();
                paragraph.Inlines.Add(new Run(faceItem.ToString()));
                paragraph.SetValue(TextElement.FontFamilyProperty,  _sampleAnnotationFontFamily);
                paragraph.SetValue(TextElement.FontSizeProperty,    _sampleAnnotationFontSize);
                paragraph.SetValue(TextElement.FontStyleProperty,   FontStyles.Normal);
                paragraph.SetValue(TextElement.FontWeightProperty,  FontWeights.Normal);
                paragraph.SetValue(TextElement.FontStretchProperty, FontStretches.Normal);
                paragraph.SetValue(TextElement.FontSizeProperty,    PointsToPixels(7));
                FamilyTypefaceSamples.Document.Blocks.Add(paragraph);

                // Add the sample for this size

                paragraph = new Paragraph();
                paragraph.Margin = new Thickness(0);
                paragraph.Inlines.Clear();
                paragraph.Inlines.Add(new Run("The quick brown fox jumps over the lazy dog"));

                _choice.ApplyToDependencyObject(paragraph);

                paragraph.SetValue(TextElement.FontStyleProperty, faceItem.Typeface.Style);
                paragraph.SetValue(TextElement.FontWeightProperty, faceItem.Typeface.Weight);
                paragraph.SetValue(TextElement.FontStretchProperty, faceItem.Typeface.Stretch);
                paragraph.SetValue(TextElement.FontSizeProperty, PointsToPixels(13));
                paragraph.SetValue(Block.LineHeightProperty, 30.0d);
                FamilyTypefaceSamples.Document.Blocks.Add(paragraph);
            }

            if (FamilyTypefaceSamples.Document.Blocks.Count > 1)
            {
                FamilyTypefaceSamples.Document.Blocks.Remove(FamilyTypefaceSamples.Document.Blocks.FirstBlock);
            }

            // Initialize current face in TypefaceListBox

            if (_choice.TypefaceListItem == null)
            {
                TypefaceListBox.Items.MoveCurrentToFirst();
                _choice.TypefaceListItem = TypefaceListBox.Items.CurrentItem as TypefaceComboBoxItem;
            }
            else
            {
                TypefaceListBox.SelectedIndex = TypefaceListBox.Items.IndexOf(_choice.TypefaceListItem);
            }
            TypefaceTextBox.Text = _choice.TypefaceListItem.ToString();
        }



        private void SetTypefaceDescriptionString(string content, TextBlock label, TextBlock description)
        {
            if (content == null || content == "")
            {
                label.Visibility = Visibility.Collapsed;
                description.Visibility = Visibility.Collapsed;
            }
            else
            {
                description.Text = content;
                label.Visibility = Visibility.Visible;
                description.Visibility = Visibility.Visible;
            }
        }



        private void SetTypographySample(string sampleText, TextBlock sample, DependencyProperty property, object value)
        {
            sample.Text = sampleText;
            //_choice.ApplyToDependencyObject(paragraph);
            sample.SetValue(TextElement.FontFamilyProperty,  _choice.FontFamily);
            sample.SetValue(TextElement.FontStyleProperty,   _choice.FontStyle);
            sample.SetValue(TextElement.FontWeightProperty,  _choice.FontWeight);
            sample.SetValue(TextElement.FontStretchProperty, _choice.FontStretch);
            sample.SetValue(TextElement.FontSizeProperty,    PointsToPixels(9.5));
            // MightDo: Set color and decoration properties too.
            //paragraph.SetValue(Block.LineHeightProperty, 30.0d);
            sample.SetValue(property, value);

        }


        private void SetTypographySample(string sampleText, TextBox sample, DependencyProperty property, object value)
        {
            sample.Text = sampleText;
            //_choice.ApplyToDependencyObject(paragraph);
            sample.SetValue(TextElement.FontFamilyProperty,  _choice.FontFamily);
            sample.SetValue(TextElement.FontStyleProperty,   _choice.FontStyle);
            sample.SetValue(TextElement.FontWeightProperty,  _choice.FontWeight);
            sample.SetValue(TextElement.FontStretchProperty, _choice.FontStretch);
            sample.SetValue(TextElement.FontSizeProperty,    PointsToPixels(9.5));
            // MightDo: Set color and decoration properties too.
            //paragraph.SetValue(Block.LineHeightProperty, 30.0d);
            sample.SetValue(property, value);


            //sample.Document.Clear();
            //
            //Paragraph paragraph = new Paragraph();
            //paragraph.Text = sampleText;
            ////_choice.ApplyToDependencyObject(paragraph);
            //paragraph.SetValue(TextElement.FontFamilyProperty,  _choice.FontFamily);
            //paragraph.SetValue(TextElement.FontStyleProperty,   _choice.FontStyle);
            //paragraph.SetValue(TextElement.FontWeightProperty,  _choice.FontWeight);
            //paragraph.SetValue(TextElement.FontStretchProperty, _choice.FontStretch);
            //paragraph.SetValue(TextElement.FontSizeProperty,    PointsToPixels(9.5));
            //// MightDo: Set color and decoration properties too.
            ////paragraph.SetValue(Block.LineHeightProperty, 30.0d);
            //paragraph.SetValue(property, value);
            //sample.Document.Children.Add(paragraph);
        }


        private void GenerateTypographySamples()
        {
            string sampleText = Preview.Text; // new TextRange(Preview.ContentStart, Preview.ContentEnd).Text.Trim();

            SetTypographySample(sampleText, KerningDisabledSample,                   Typography.KerningProperty,                false);
            SetTypographySample(sampleText, KerningEnabledSample,                    Typography.KerningProperty,                true);
            SetTypographySample(sampleText, StandardLigaturesDisabledSample,         Typography.StandardLigaturesProperty,      false);
            SetTypographySample(sampleText, StandardLigaturesEnabledSample,          Typography.StandardLigaturesProperty,      true);
            SetTypographySample(sampleText, ContextualLigaturesDisabledSample,       Typography.ContextualLigaturesProperty,    false);
            SetTypographySample(sampleText, ContextualLigaturesEnabledSample,        Typography.ContextualLigaturesProperty,    true);
            SetTypographySample(sampleText, DiscretionaryLigaturesDisabledSample,    Typography.DiscretionaryLigaturesProperty, false);
            SetTypographySample(sampleText, DiscretionaryLigaturesEnabledSample,     Typography.DiscretionaryLigaturesProperty, true);
            SetTypographySample(sampleText, HistoricalLigaturesDisabledSample,       Typography.HistoricalLigaturesProperty,    false);
            SetTypographySample(sampleText, HistoricalLigaturesEnabledSample,        Typography.HistoricalLigaturesProperty,    true);
            SetTypographySample(sampleText, ContextualAlternatesDisabledSample,      Typography.ContextualAlternatesProperty,   false);
            SetTypographySample(sampleText, ContextualAlternatesEnabledSample,       Typography.ContextualAlternatesProperty,   true);
            SetTypographySample(sampleText, HistoricalFormsDisabledSample,           Typography.HistoricalFormsProperty,        false);
            SetTypographySample(sampleText, HistoricalFormsEnabledSample,            Typography.HistoricalFormsProperty,        true);
            SetTypographySample(sampleText, AnnotationAlternatesNoneSample,          Typography.AnnotationAlternatesProperty,   0);
            SetTypographySample(sampleText, AnnotationAlternatesSet1Sample,          Typography.AnnotationAlternatesProperty,   1);
            SetTypographySample(sampleText, AnnotationAlternatesSet2Sample,          Typography.AnnotationAlternatesProperty,   2);
            SetTypographySample(sampleText, AnnotationAlternatesSet3Sample,          Typography.AnnotationAlternatesProperty,   3);
            SetTypographySample(sampleText, ContextualSwashesNoneSample,             Typography.ContextualSwashesProperty,      0);
            SetTypographySample(sampleText, ContextualSwashesSet1Sample,             Typography.ContextualSwashesProperty,      1);
            SetTypographySample(sampleText, ContextualSwashesSet2Sample,             Typography.ContextualSwashesProperty,      2);
            SetTypographySample(sampleText, ContextualSwashesSet3Sample,             Typography.ContextualSwashesProperty,      3);
            SetTypographySample(sampleText, CapitalsNormalSample,                    Typography.CapitalsProperty,               FontCapitals.Normal);
            SetTypographySample(sampleText, CapitalsAllSmallSample,                  Typography.CapitalsProperty,               FontCapitals.AllSmallCaps);
            SetTypographySample(sampleText, CapitalsSmallSample,                     Typography.CapitalsProperty,               FontCapitals.SmallCaps);
            SetTypographySample(sampleText, CapitalsAllPetiteSample,                 Typography.CapitalsProperty,               FontCapitals.AllPetiteCaps);
            SetTypographySample(sampleText, CapitalsPetiteSample,                    Typography.CapitalsProperty,               FontCapitals.PetiteCaps);
            SetTypographySample(sampleText, CapitalsUnicaseSample,                   Typography.CapitalsProperty,               FontCapitals.Unicase);
            SetTypographySample(sampleText, CapitalsTitlingSample,                   Typography.CapitalsProperty,               FontCapitals.Titling);
            SetTypographySample(sampleText, FractionsNormalSample,                   Typography.FractionProperty,               FontFraction.Normal);
            SetTypographySample(sampleText, FractionsSlashedSample,                  Typography.FractionProperty,               FontFraction.Slashed);
            SetTypographySample(sampleText, FractionsStackedSample,                  Typography.FractionProperty,               FontFraction.Stacked);
            SetTypographySample(sampleText, StandardSwashesNoneSample,               Typography.StandardSwashesProperty,        0);
            SetTypographySample(sampleText, StandardSwashesSet1Sample,               Typography.StandardSwashesProperty,        1);
            SetTypographySample(sampleText, StandardSwashesSet2Sample,               Typography.StandardSwashesProperty,        2);
            SetTypographySample(sampleText, StandardSwashesSet3Sample,               Typography.StandardSwashesProperty,        3);
            SetTypographySample(sampleText, CapitalSpacingDisabledSample,            Typography.CapitalSpacingProperty,         false);
            SetTypographySample(sampleText, CapitalSpacingEnabledSample,             Typography.CapitalSpacingProperty,         true);
            SetTypographySample(sampleText, CaseSensitiveFormsDisabledSample,        Typography.CaseSensitiveFormsProperty,     false);
            SetTypographySample(sampleText, CaseSensitiveFormsEnabledSample,         Typography.CaseSensitiveFormsProperty,     true);
            SetTypographySample(sampleText, EastAsianExpertFormsDisabledSample,      Typography.EastAsianExpertFormsProperty,   false);
            SetTypographySample(sampleText, EastAsianExpertFormsEnabledSample,       Typography.EastAsianExpertFormsProperty,   true);
            SetTypographySample(sampleText, EastAsianLanguageNormalSample,           Typography.EastAsianLanguageProperty,      FontEastAsianLanguage.Normal);
            SetTypographySample(sampleText, EastAsianLanguageJis78Sample,            Typography.EastAsianLanguageProperty,      FontEastAsianLanguage.Jis78);
            SetTypographySample(sampleText, EastAsianLanguageJis83Sample,            Typography.EastAsianLanguageProperty,      FontEastAsianLanguage.Jis83);
            SetTypographySample(sampleText, EastAsianLanguageJis90Sample,            Typography.EastAsianLanguageProperty,      FontEastAsianLanguage.Jis90);
            SetTypographySample(sampleText, EastAsianLanguageJis04Sample,            Typography.EastAsianLanguageProperty,      FontEastAsianLanguage.Jis04);
            SetTypographySample(sampleText, EastAsianLanguageHojoKanjiSample,        Typography.EastAsianLanguageProperty,      FontEastAsianLanguage.HojoKanji);
            SetTypographySample(sampleText, EastAsianLanguageNlcKanjiSample,         Typography.EastAsianLanguageProperty,      FontEastAsianLanguage.NlcKanji);
            SetTypographySample(sampleText, EastAsianLanguageSimplifiedSample,       Typography.EastAsianLanguageProperty,      FontEastAsianLanguage.Simplified);
            SetTypographySample(sampleText, EastAsianLanguageTraditionalSample,      Typography.EastAsianLanguageProperty,      FontEastAsianLanguage.Traditional);
            SetTypographySample(sampleText, EastAsianLanguageTraditionalNamesSample, Typography.EastAsianLanguageProperty,      FontEastAsianLanguage.Traditional);
            SetTypographySample(sampleText, EastAsianWidthsNormalSample,             Typography.EastAsianWidthsProperty,        FontEastAsianWidths.Normal);
            SetTypographySample(sampleText, EastAsianWidthsProportionalSample,       Typography.EastAsianWidthsProperty,        FontEastAsianWidths.Proportional);
            SetTypographySample(sampleText, EastAsianWidthsFullSample,               Typography.EastAsianWidthsProperty,        FontEastAsianWidths.Full);
            SetTypographySample(sampleText, EastAsianWidthsHalfSample,               Typography.EastAsianWidthsProperty,        FontEastAsianWidths.Half);
            SetTypographySample(sampleText, EastAsianWidthsThirdSample,              Typography.EastAsianWidthsProperty,        FontEastAsianWidths.Third);
            SetTypographySample(sampleText, EastAsianWidthsQuarterSample,            Typography.EastAsianWidthsProperty,        FontEastAsianWidths.Quarter);
            SetTypographySample(sampleText, MathematicalGreekDisabledSample,         Typography.MathematicalGreekProperty,      false);
            SetTypographySample(sampleText, MathematicalGreekEnabledSample,          Typography.MathematicalGreekProperty,      true);
            SetTypographySample(sampleText, NumeralAlignmentNormalSample,            Typography.NumeralAlignmentProperty,       FontNumeralAlignment.Normal);
            SetTypographySample(sampleText, NumeralAlignmentProportionalSample,      Typography.NumeralAlignmentProperty,       FontNumeralAlignment.Proportional);
            SetTypographySample(sampleText, NumeralAlignmentTabularSample,           Typography.NumeralAlignmentProperty,       FontNumeralAlignment.Tabular);
            SetTypographySample(sampleText, NumeralStyleNormalSample,                Typography.NumeralStyleProperty,           FontNumeralStyle.Normal);
            SetTypographySample(sampleText, NumeralStyleLiningSample,                Typography.NumeralStyleProperty,           FontNumeralStyle.Lining);
            SetTypographySample(sampleText, NumeralStyleOldStyleSample,              Typography.NumeralStyleProperty,           FontNumeralStyle.OldStyle);
            SetTypographySample(sampleText, SlashedZeroDisabledSample,               Typography.SlashedZeroProperty,            false);
            SetTypographySample(sampleText, SlashedZeroEnabledSample,                Typography.SlashedZeroProperty,            true);
            SetTypographySample(sampleText, StylisticAlternatesNoneSample,           Typography.StylisticAlternatesProperty,    0);
            SetTypographySample(sampleText, StylisticAlternatesSet1Sample,           Typography.StylisticAlternatesProperty,    1);
            SetTypographySample(sampleText, StylisticAlternatesSet2Sample,           Typography.StylisticAlternatesProperty,    2);
            SetTypographySample(sampleText, StylisticAlternatesSet3Sample,           Typography.StylisticAlternatesProperty,    3);
            SetTypographySample(sampleText, StylisticSet1DisabledSample,             Typography.StylisticSet1Property,          false);
            SetTypographySample(sampleText, StylisticSet1EnabledSample,              Typography.StylisticSet1Property,          true);
            SetTypographySample(sampleText, StylisticSet2DisabledSample,             Typography.StylisticSet2Property,          false);
            SetTypographySample(sampleText, StylisticSet2EnabledSample,              Typography.StylisticSet2Property,          true);
            SetTypographySample(sampleText, StylisticSet3DisabledSample,             Typography.StylisticSet3Property,          false);
            SetTypographySample(sampleText, StylisticSet3EnabledSample,              Typography.StylisticSet3Property,          true);
            SetTypographySample(sampleText, StylisticSet4DisabledSample,             Typography.StylisticSet4Property,          false);
            SetTypographySample(sampleText, StylisticSet4EnabledSample,              Typography.StylisticSet4Property,          true);
            SetTypographySample(sampleText, StylisticSet5DisabledSample,             Typography.StylisticSet5Property,          false);
            SetTypographySample(sampleText, StylisticSet5EnabledSample,              Typography.StylisticSet5Property,          true);
            SetTypographySample(sampleText, StylisticSet6DisabledSample,             Typography.StylisticSet6Property,          false);
            SetTypographySample(sampleText, StylisticSet6EnabledSample,              Typography.StylisticSet6Property,          true);
            SetTypographySample(sampleText, StylisticSet7DisabledSample,             Typography.StylisticSet7Property,          false);
            SetTypographySample(sampleText, StylisticSet7EnabledSample,              Typography.StylisticSet7Property,          true);
            SetTypographySample(sampleText, StylisticSet8DisabledSample,             Typography.StylisticSet8Property,          false);
            SetTypographySample(sampleText, StylisticSet8EnabledSample,              Typography.StylisticSet8Property,          true);
            SetTypographySample(sampleText, StylisticSet9DisabledSample,             Typography.StylisticSet9Property,          false);
            SetTypographySample(sampleText, StylisticSet9EnabledSample,              Typography.StylisticSet9Property,          true);
            SetTypographySample(sampleText, StylisticSet10DisabledSample,            Typography.StylisticSet10Property,         false);
            SetTypographySample(sampleText, StylisticSet10EnabledSample,             Typography.StylisticSet10Property,         true);
            SetTypographySample(sampleText, StylisticSet11DisabledSample,            Typography.StylisticSet11Property,         false);
            SetTypographySample(sampleText, StylisticSet11EnabledSample,             Typography.StylisticSet11Property,         true);
            SetTypographySample(sampleText, StylisticSet12DisabledSample,            Typography.StylisticSet12Property,         false);
            SetTypographySample(sampleText, StylisticSet12EnabledSample,             Typography.StylisticSet12Property,         true);
            SetTypographySample(sampleText, StylisticSet13DisabledSample,            Typography.StylisticSet13Property,         false);
            SetTypographySample(sampleText, StylisticSet13EnabledSample,             Typography.StylisticSet13Property,         true);
            SetTypographySample(sampleText, StylisticSet14DisabledSample,            Typography.StylisticSet14Property,         false);
            SetTypographySample(sampleText, StylisticSet14EnabledSample,             Typography.StylisticSet14Property,         true);
            SetTypographySample(sampleText, StylisticSet15DisabledSample,            Typography.StylisticSet15Property,         false);
            SetTypographySample(sampleText, StylisticSet15EnabledSample,             Typography.StylisticSet15Property,         true);
            SetTypographySample(sampleText, StylisticSet16DisabledSample,            Typography.StylisticSet16Property,         false);
            SetTypographySample(sampleText, StylisticSet16EnabledSample,             Typography.StylisticSet16Property,         true);
            SetTypographySample(sampleText, StylisticSet17DisabledSample,            Typography.StylisticSet17Property,         false);
            SetTypographySample(sampleText, StylisticSet17EnabledSample,             Typography.StylisticSet17Property,         true);
            SetTypographySample(sampleText, StylisticSet18DisabledSample,            Typography.StylisticSet18Property,         false);
            SetTypographySample(sampleText, StylisticSet18EnabledSample,             Typography.StylisticSet18Property,         true);
            SetTypographySample(sampleText, StylisticSet19DisabledSample,            Typography.StylisticSet19Property,         false);
            SetTypographySample(sampleText, StylisticSet19EnabledSample,             Typography.StylisticSet19Property,         true);
            SetTypographySample(sampleText, StylisticSet20DisabledSample,            Typography.StylisticSet20Property,         false);
            SetTypographySample(sampleText, StylisticSet20EnabledSample,             Typography.StylisticSet20Property,         true);
            SetTypographySample(sampleText, VariantsNormalSample,                    Typography.VariantsProperty,               FontVariants.Normal);
            SetTypographySample(sampleText, VariantsSuperscriptSample,               Typography.VariantsProperty,               FontVariants.Superscript);
            SetTypographySample(sampleText, VariantsSubscriptSample,                 Typography.VariantsProperty,               FontVariants.Subscript);
            SetTypographySample(sampleText, VariantsOrdinalSample,                   Typography.VariantsProperty,               FontVariants.Ordinal);
            SetTypographySample(sampleText, VariantsInferiorSample,                  Typography.VariantsProperty,               FontVariants.Inferior);
            SetTypographySample(sampleText, VariantsRubySample,                      Typography.VariantsProperty,               FontVariants.Ruby);
        }




        private void GenerateTypefaceDescription()
        {
            if (_choice.TypefaceListItem.NominalGlyphTypeface == null)
            {
                DescriptiveText.Visibility = Visibility.Collapsed;
                FontViewer.SelectedIndex = 0;  // Force family and face samples into view
            }
            else
            {
                DescriptiveText.Visibility = Visibility.Visible;
                SetTypefaceDescriptionString(_choice.TypefaceListItem.NominalGlyphTypeface.FontUri.ToString(), FontFileUriLabel, FontFileUriText);
                SetTypefaceDescriptionString(_choice.TypefaceListItem.NominalGlyphTypeface.Copyrights[_descriptiveTextCulture], CopyrightLabel, CopyrightText);
                SetTypefaceDescriptionString(_choice.TypefaceListItem.NominalGlyphTypeface.Descriptions[_descriptiveTextCulture], DescriptionLabel, DescriptionText);
                SetTypefaceDescriptionString(_choice.TypefaceListItem.NominalGlyphTypeface.DesignerNames[_descriptiveTextCulture], DesignerNameLabel, DesignerNameText);
                SetTypefaceDescriptionString(_choice.TypefaceListItem.NominalGlyphTypeface.DesignerUrls[_descriptiveTextCulture], DesignerUrlLabel, DesignerUrlText);
                SetTypefaceDescriptionString(_choice.TypefaceListItem.NominalGlyphTypeface.FaceNames[_descriptiveTextCulture], FaceNameLabel, FaceNameText);
                SetTypefaceDescriptionString(_choice.TypefaceListItem.NominalGlyphTypeface.FamilyNames[_descriptiveTextCulture], FamilyNameLabel, FamilyNameText);
                SetTypefaceDescriptionString(_choice.TypefaceListItem.NominalGlyphTypeface.ManufacturerNames[_descriptiveTextCulture], ManufacturerNameLabel, ManufacturerNameText);
                SetTypefaceDescriptionString(_choice.TypefaceListItem.NominalGlyphTypeface.SampleTexts[_descriptiveTextCulture], SampleTextLabel, SampleTextText);
                SetTypefaceDescriptionString(_choice.TypefaceListItem.NominalGlyphTypeface.Trademarks[_descriptiveTextCulture], TrademarkLabel, TrademarkText);
                SetTypefaceDescriptionString(_choice.TypefaceListItem.NominalGlyphTypeface.VendorUrls[_descriptiveTextCulture], VendorUrlLabel, VendorUrlText);
                SetTypefaceDescriptionString(_choice.TypefaceListItem.NominalGlyphTypeface.VersionStrings[_descriptiveTextCulture], VersionStringLabel, VersionStringText);
                SetTypefaceDescriptionString(_choice.TypefaceListItem.NominalGlyphTypeface.Win32FaceNames[_descriptiveTextCulture], Win32FaceNameLabel, Win32FaceNameText);
                SetTypefaceDescriptionString(_choice.TypefaceListItem.NominalGlyphTypeface.Win32FamilyNames[_descriptiveTextCulture], Win32FamilyNameLabel, Win32FamilyNameText);

                LicenseDescription.Text = "";
                if (_choice.TypefaceListItem.NominalGlyphTypeface.LicenseDescriptions[_descriptiveTextCulture] != null)
                {
                    LicenseDescription.Text = _choice.TypefaceListItem.NominalGlyphTypeface.LicenseDescriptions[_descriptiveTextCulture];
                }
            }
        }


        void OnOkButtonClicked(object obj, RoutedEventArgs args)
        {
            this.DialogResult = true;
            this.Close();
            if (FontChosen != null)
            {
                FontChosen(this, new FontChooserDialogAppliedEventsArgs(_choice));
            }
            if (_selection != null)
            {
                _choice.ApplyToRange(_richTextBox);
            }
        }


        void OnCancelButtonClicked(object obj, RoutedEventArgs args)
        {
            this.DialogResult = false;
            this.Close();
        }



        void SetDecoration()
        {
            if (_choice.TextDecorationCollection != null)
            {
                bool strike = false;
                bool underline = false;
                bool overline = false;
                foreach (TextDecoration decoration in _choice.TextDecorationCollection)
                {
                    if (decoration.Equals(System.Windows.TextDecorations.Strikethrough[0] as TextDecoration))
                    {
                        strike = true;
                    }
                    else if (decoration.Equals(System.Windows.TextDecorations.Underline[0] as TextDecoration))
                    {
                        underline = true;
                    }
                    else if (decoration.Equals(System.Windows.TextDecorations.OverLine[0] as TextDecoration))
                    {
                        overline = true;
                    }
                }

                StrikethroughCheckBox.IsChecked = strike;
                UnderLineCheckBox.IsChecked = underline;
                OverLineCheckBox.IsChecked = overline;
            }
        }


        void DecorationsChanged(object o, RoutedEventArgs args)
        {
            TextDecorationCollection decorations = new TextDecorationCollection();
            if ((bool)StrikethroughCheckBox.IsChecked)
            {
                decorations.Add(System.Windows.TextDecorations.Strikethrough[0]);
            }
            if ((bool)UnderLineCheckBox.IsChecked)
            {
                decorations.Add(System.Windows.TextDecorations.Underline[0]);
            }
            if ((bool)OverLineCheckBox.IsChecked)
            {
                decorations.Add(System.Windows.TextDecorations.OverLine[0]);
            }
            _choice.TextDecorationCollection = decorations;
            GenerateFamilyTypefaceSamples();
            GenerateTypefaceSizeSamples();
            GenerateTypographySamples();
            _choice.ApplyToDependencyObject(Preview);
        }




        #region combo box handling

/*
 *      Avoiding infinite recursion in combo box handling
 *      The problem:
 *          The customer may change the value of the combo box either
 *          by typing in the textbox or scrolling the listobox.
 *          The implementation of a change in either will cause a change
 *          in the other.
 *          Clicking on an item in the list box causes the text in the
 *          textbox to be updated to the item clicked.
 *          Typing a full item name in the textbox causes the corresponding
 *          entry in the list to be selected.
 *     The solution:
 *          Changes to the textbox only cause a selection in the listbox if
 *          the listbox does not already have the correct value selected.
 *          However, clicking in the listbox always updates the textbox, and
 *          the code for propagating the changes through the font chooser is
 *          in the text box change handling
*/


        void FontFamilyChanged(object family, TextChangedEventArgs args)
        {
            FontFamilyComboBoxItem item = TextBoxChanged(FontFamilyTextBox, FontFamilyListBox) as FontFamilyComboBoxItem;
            if (item == null)
            {
                FontFamilyTextBox.Foreground = Brushes.Gray;
            }
            else
            {
                FontFamilyTextBox.Foreground = Brushes.Black;
                _choice.FontFamily = new FontFamily(FontFamilyTextBox.Text);
                GenerateFamilyTypefaceSamples();
                GenerateTypefaceSizeSamples();
                PresetDescriptionLanguageCombo();
                GenerateTypefaceDescription();
                GenerateTypographySamples();
                _choice.ApplyToDependencyObject(Preview);
            }
        }

        void TypefaceChanged(object face, TextChangedEventArgs args)
        {
            TypefaceComboBoxItem item = TextBoxChanged(TypefaceTextBox, TypefaceListBox) as TypefaceComboBoxItem;
            if (item == null)
            {
                TypefaceTextBox.Foreground = Brushes.Gray;
            }
            else
            {
                TypefaceTextBox.Foreground = Brushes.Black;
                _choice.TypefaceListItem = item;
                GenerateTypefaceSizeSamples();
                PresetDescriptionLanguageCombo();
                GenerateTypefaceDescription();
                GenerateTypographySamples();
                _choice.ApplyToDependencyObject(Preview);
            }
        }

        void FontSizeChanged(object size, TextChangedEventArgs args)
        {
            double chosenSize = -1;
            if (double.TryParse(FontSizeTextBox.Text, out chosenSize))
            {
                _choice.FontSize = PointsToPixels(chosenSize);
                GenerateTypefaceSizeSamples();
                GenerateTypographySamples();
                _choice.ApplyToDependencyObject(Preview);
                FontSizeTextBox.Foreground = Brushes.Black;
            }
            else
            {
                FontSizeTextBox.Foreground = Brushes.Gray;
            }
        }

        void TextColorChanged(object color, TextChangedEventArgs args)
        {
            ColorComboBoxItem item = TextBoxChanged(color as TextBox, TextColorListBox) as ColorComboBoxItem;
            if (item == null)
            {
                TextColorTextBox.Foreground = Brushes.Gray;
            }
            else
            {
                TextColorTextBox.Foreground = Brushes.Black;
                _choice.Foreground = item.Brush;
                GenerateFamilyTypefaceSamples();
                GenerateTypefaceSizeSamples();
                GenerateTypographySamples();
                _choice.ApplyToDependencyObject(Preview);
            }
        }

        void DescriptionLanguageChanged(object language, TextChangedEventArgs args)
        {
            DescriptionLanguageComboBoxItem item = TextBoxChanged(DescriptionLanguageTextBox, DescriptionLanguageListBox) as DescriptionLanguageComboBoxItem;
            if (item == null)
            {
                DescriptionLanguageTextBox.Foreground = Brushes.Gray;
            }
            else
            {
                DescriptionLanguageTextBox.Foreground = Brushes.Black;
                _descriptiveTextCulture = item.Culture;
                GenerateTypefaceDescription();
            }
        }


        object TextBoxChanged(TextBox textBox, ListBox listBox)
        {
            // Scroll the listbox so that the nearest listbox entry less than or
            // equal to the text box entry is in view. If the textbox entry exactly
            // matches a listbox entry, select the entry.
            // Returns selected list item or null if none matching.


            // Find the nearest string in the listbox. Note that listboxes are not
            // necessarily in alphabetic order - for example the typeface listbox
            // is sorted by stretch, weight and style, not by alphabetic value of the
            // descriptive text.
            //
            // This code finds the (first) item exactly the same as the text box, or
            // if there is no equal value, it finds the item immediately in front
            // of the first item alphabetically greater than the textbox.

            int  nearestItem  = 0;
            bool foundNearest = false;

            for (int i = 0; i < listBox.Items.Count; i++)
            {
                int comparison = (listBox.Items[i] as IFontChooserComboBoxItem).CompareWithString(textBox.Text);

                if (comparison < 0)
                {
                    if (!foundNearest)
                    {
                        nearestItem = i;
                    }
                }
                else if (comparison == 0)
                {
                    nearestItem = i;
                    foundNearest = true;
                }
                else
                {
                    foundNearest = true;
                }
            }

            listBox.Items.MoveCurrentToPosition(nearestItem);
            // Doesn't change highlight: listBox.ScrollIntoView(listBox.Items[nearestItem]);

            if ((listBox.Items[nearestItem] as IFontChooserComboBoxItem).CompareWithString(textBox.Text) == 0)
            {
                // TextBox exactly matches an entry in the list box
                // Make sure it is selected (but don't fire a selection change if it is already selected)
                if (listBox.SelectedIndex != nearestItem)
                {
                    listBox.SelectedIndex = nearestItem;
                }
                return listBox.Items[nearestItem];  // This is the item we matched
            }
            else
            {
                // Text string does not match any entry in the list
                return null;
            }
        }

        void FontFamilySelected(object o, SelectionChangedEventArgs args)
        {
            if (FontFamilyListBox.SelectedItem != null)
            {
                FontFamilyTextBox.Text = FontFamilyListBox.SelectedItem.ToString();
            }
        }

        void TypefaceSelected(object o, SelectionChangedEventArgs args)
        {
            if (TypefaceListBox.SelectedItem != null)
            {
                TypefaceTextBox.Text = TypefaceListBox.SelectedItem.ToString();
            }
        }

        void FontSizeSelected(object o, SelectionChangedEventArgs args)
        {
            if (FontSizeListBox.SelectedItem != null)
            {
                FontSizeTextBox.Text = FontSizeListBox.SelectedItem.ToString();
            }
        }

        void TextColorSelected(object o, SelectionChangedEventArgs args)
        {
            if (TextColorListBox.SelectedItem != null)
            {
                TextColorTextBox.Text = TextColorListBox.SelectedItem.ToString();
            }
        }

        void DescriptionLanguageSelected(object o, SelectionChangedEventArgs args)
        {
            if (DescriptionLanguageListBox.SelectedItem != null)
            {
                DescriptionLanguageTextBox.Text = DescriptionLanguageListBox.SelectedItem.ToString();
            }
        }

        void PresetFontFamilyCombo(string familyName)
        {
            // Preset font families list
            foreach (FontFamily family in System.Windows.Media.Fonts.SystemFontFamilies)
            {
                FontFamilyListBox.Items.Add(new FontFamilyComboBoxItem(family.Source));
            }
            FontFamilyTextBox.Text = familyName;
        }

        private static double[] s_commonlyUsedFontSizes = new double[] {
                3.0d,    4.0d,   5.0d,   6.0d,   6.5d,
                7.0d,    7.5d,   8.0d,   8.5d,   9.0d,
                9.5d,   10.0d,  10.5d,  11.0d,  11.5d,
                12.0d,  12.5d,  13.0d,  13.5d,  14.0d,
                15.0d,  16.0d,  17.0d,  18.0d,  19.0d,
                20.0d,  22.0d,  24.0d,  26.0d,  28.0d,  30.0d,  32.0d,  34.0d,  36.0d,  38.0d,
                40.0d,  44.0d,  48.0d,  52.0d,  56.0d,  60.0d,  64.0d,  68.0d,  72.0d,  76.0d,
                80.0d,  88.0d,  96.0d, 104.0d, 112.0d, 120.0d, 128.0d, 136.0d, 144.0d, 152.0d,
               160.0d, 176.0d, 192.0d, 208.0d, 224.0d, 240.0d, 256.0d, 272.0d, 288.0d, 304.0d,
               320.0d, 352.0d, 384.0d, 416.0d, 448.0d, 480.0d, 512.0d, 544.0d, 576.0d, 608.0d,
               640.0d
        };

        void PresetFontSizeCombo(double size)
        {
            // Preset font size listbox and scroll to size nearest preview text size
            for (int i = 0; i < s_commonlyUsedFontSizes.Length; i++)
            {
                FontSizeListBox.Items.Add(new FontSizeComboBoxItem(s_commonlyUsedFontSizes[i]));
            }

            FontSizeTextBox.Text = PixelsToPoints(size).ToString("0.##", CultureInfo.CurrentCulture);
        }
        
        void PresetFontColorCombo(Brush foreground)
        {
            int i;
            // Fill combobox with all known named colors

            for (i = 0; i < KnownColor.ColorNames.Length; i++)
            {
                TextColorListBox.Items.Add(new ColorComboBoxItem(
                    KnownColor.ColorNames[i],
                    (SolidColorBrush)KnownColor.ColorTable[KnownColor.ColorNames[i]]
                ));
            }

            // Look for and display incoming color

            string colorName = "Black";  // Will use black if incoming color is not a known named color

            if (foreground is SolidColorBrush)
            {
                Brush brush = foreground as SolidColorBrush;
                for (i=0; i<TextColorListBox.Items.Count; i++)
                {
                    if ((TextColorListBox.Items[i] as ColorComboBoxItem).Brush == brush)
                    {
                        colorName = (TextColorListBox.Items[i] as ColorComboBoxItem).ToString();
                    }
                }
            }

            TextColorTextBox.Text = colorName;
        }

        private int DescriptiveLanguageListBoxCultureIndex(CultureInfo culture)
        {
            for (int i = 0; i < DescriptionLanguageListBox.Items.Count; i++)
            {
                if ((DescriptionLanguageListBox.Items[i]
                     as DescriptionLanguageComboBoxItem).Culture.Equals(culture))
                {
                    return i;
                }
            }
            return -1;
        }

        private static CultureInfo s_englishCulture = new CultureInfo("en-US");

        private void PresetDescriptionLanguageCombo()
        {
            if (_choice.TypefaceListItem.NominalGlyphTypeface == null)
            {
                // No descripive text implemented for composite fonts
            }
            else
            {
                DescriptionLanguageListBox.Items.Clear();
                foreach (CultureInfo culture in _choice.TypefaceListItem.DescriptiveTextCultures)
                {
                    DescriptionLanguageListBox.Items.Add(new DescriptionLanguageComboBoxItem(culture));
                }

                // Attempt to keep current culture, if not choose English, if not use first available culture.

                int cultureIndex;
                cultureIndex = DescriptiveLanguageListBoxCultureIndex(_descriptiveTextCulture);

                if (cultureIndex < 0)
                {
                    cultureIndex = DescriptiveLanguageListBoxCultureIndex(s_englishCulture);
                }

                if (cultureIndex < 0)
                {
                    cultureIndex = 0;
                }

                DescriptionLanguageListBox.SelectedIndex = cultureIndex;

                DescriptionLanguageListBox.ScrollIntoView(DescriptionLanguageListBox.SelectedItem);
            }
        }

        #endregion

        #region Typographic feature UI

        void AnnotationAlternatesChanged(object obj, RoutedEventArgs args)
        {
        }

        void CapitalsChanged(object obj, RoutedEventArgs args)
        {
        }

        void CapitalSpacingChanged(object obj, RoutedEventArgs args)
        {
        }

        void CaseSensitiveFormsChanged(object obj, RoutedEventArgs args)
        {
        }

        void ContextualAlternatesChanged(object obj, RoutedEventArgs args)
        {
        }

        void ContextualLigaturesChanged(object obj, RoutedEventArgs args)
        {
        }

        void ContextualSwashesChanged(object obj, RoutedEventArgs args)
        {
        }

        void DiscretionaryLigaturesChanged(object obj, RoutedEventArgs args)
        {
        }

        void EastAsianExpertFormsChanged(object obj, RoutedEventArgs args)
        {
        }

        void EastAsianLanguageChanged(object obj, RoutedEventArgs args)
        {
        }

        void EastAsianWidthsChanged(object obj, RoutedEventArgs args)
        {
        }

        void FractionChanged(object obj, RoutedEventArgs args)
        {
        }

        void HistoricalFormsChanged(object obj, RoutedEventArgs args)
        {
        }

        void HistoricalLigaturesChanged(object obj, RoutedEventArgs args)
        {
        }

        void KerningChanged(object obj, RoutedEventArgs args)
        {
        }

        void MathematicalGreekChanged(object obj, RoutedEventArgs args)
        {
        }

        void NumeralAlignmentChanged(object obj, RoutedEventArgs args)
        {
        }

        void NumeralStyleChanged(object obj, RoutedEventArgs args)
        {
        }

        void SlashedZeroChanged(object obj, RoutedEventArgs args)
        {
        }

        void StandardLigaturesChanged(object obj, RoutedEventArgs args)
        {
        }

        void StandardSwashesChanged(object obj, RoutedEventArgs args)
        {
        }

        void StylisticAlternatesChanged(object obj, RoutedEventArgs args)
        {
        }

        void StylisticSet10Changed(object obj, RoutedEventArgs args)
        {
        }

        void StylisticSet11Changed(object obj, RoutedEventArgs args)
        {
        }

        void StylisticSet12Changed(object obj, RoutedEventArgs args)
        {
        }

        void StylisticSet13Changed(object obj, RoutedEventArgs args)
        {
        }

        void StylisticSet14Changed(object obj, RoutedEventArgs args)
        {
        }

        void StylisticSet15Changed(object obj, RoutedEventArgs args)
        {
        }

        void StylisticSet16Changed(object obj, RoutedEventArgs args)
        {
        }

        void StylisticSet17Changed(object obj, RoutedEventArgs args)
        {
        }

        void StylisticSet18Changed(object obj, RoutedEventArgs args)
        {
        }

        void StylisticSet19Changed(object obj, RoutedEventArgs args)
        {
        }

        void StylisticSet1Changed(object obj, RoutedEventArgs args)
        {
        }

        void StylisticSet20Changed(object obj, RoutedEventArgs args)
        {
        }

        void StylisticSet2Changed(object obj, RoutedEventArgs args)
        {
        }

        void StylisticSet3Changed(object obj, RoutedEventArgs args)
        {
        }

        void StylisticSet4Changed(object obj, RoutedEventArgs args)
        {
        }

        void StylisticSet5Changed(object obj, RoutedEventArgs args)
        {
        }

        void StylisticSet6Changed(object obj, RoutedEventArgs args)
        {
        }

        void StylisticSet7Changed(object obj, RoutedEventArgs args)
        {
        }

        void StylisticSet8Changed(object obj, RoutedEventArgs args)
        {
        }

        void StylisticSet9Changed(object obj, RoutedEventArgs args)
        {
        }

        void VariantsChanged(object obj, RoutedEventArgs args)
        {
        }

        #endregion

        void CreateContextMenu(object obj, System.Windows.Input.MouseButtonEventArgs e)
        {
            _controlForHelp = obj as Control;
            ContextMenu menu = new ContextMenu();
            MenuItem item=new MenuItem();
            item.Click +=new RoutedEventHandler(item_Click);
            item.Header = "What's this?";
            menu.Items.Add(item);
            _controlForHelp.ContextMenu = menu;
        }

        void item_Click(object o, RoutedEventArgs args)
        {
            ToolTip tp = new ToolTip();
            tp.Content = ToolTipMessage.GetToolTipMessage( _controlForHelp.Name);
            _controlForHelp.ToolTip = tp;
            tp.IsOpen = true;
            tp.Closed += new RoutedEventHandler(tp_Closed);
            tp.StaysOpen = false;
        }

        void tp_Closed(object obj, RoutedEventArgs e)
        {
            _controlForHelp.ToolTip = null;
        }
        #endregion

        #region privatefields

        ResourceManager _rm = new ResourceManager("Microsoft.Samples.AvalonNotepad.stringtable", Assembly.GetExecutingAssembly());

        #endregion privatefields
    }
}