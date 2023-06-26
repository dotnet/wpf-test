// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************************
 *
 * Description: FontChoice.cs implement the immediate window with the following
 * features:
 *      1. Font pool for font dialog
 *
 *******************************************************************************/

using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Controls; // RichTextBox


namespace EditingExaminer
{
    public class FontChoice
    {
        private FontFamily                _family;
        private FontStyle                 _style;
        private FontWeight                _weight;
        private FontStretch               _stretch;
        private double                    _size;           // In Avalon pixels (96ths of an inch)
        private TextDecorationCollection  _decoration;
        private Brush                     _brush;
        private int                       _annotationAlternates;
        private FontCapitals              _capitals;
        private bool                      _capitalSpacing;
        private bool                      _caseSensitiveForms;
        private bool                      _contextualAlternates;
        private bool                      _contextualLigatures;
        private int                       _contextualSwashes;
        private bool                      _discretionaryLigatures;
        private bool                      _eastAsianExpertForms;
        private FontEastAsianLanguage     _eastAsianLanguage;
        private FontEastAsianWidths       _eastAsianWidths;
        private FontFraction              _fraction;
        private bool                      _historicalForms;
        private bool                      _historicalLigatures;
        private bool                      _kerning;
        private bool                      _mathematicalGreek;
        private FontNumeralAlignment      _numeralAlignment;
        private FontNumeralStyle          _numeralStyle;
        private bool                      _slashedZero;
        private bool                      _standardLigatures;
        private int                       _standardSwashes;
        private int                       _stylisticAlternates;
        private bool                      _stylisticSet1;
        private bool                      _stylisticSet2;
        private bool                      _stylisticSet3;
        private bool                      _stylisticSet4;
        private bool                      _stylisticSet5;
        private bool                      _stylisticSet6;
        private bool                      _stylisticSet7;
        private bool                      _stylisticSet8;
        private bool                      _stylisticSet9;
        private bool                      _stylisticSet10;
        private bool                      _stylisticSet11;
        private bool                      _stylisticSet12;
        private bool                      _stylisticSet13;
        private bool                      _stylisticSet14;
        private bool                      _stylisticSet15;
        private bool                      _stylisticSet16;
        private bool                      _stylisticSet17;
        private bool                      _stylisticSet18;
        private bool                      _stylisticSet19;
        private bool                      _stylisticSet20;
        private FontVariants              _variants;



        public FontFamily               FontFamily              { get {return _family;}                  set  {_family = value;} }
        public FontStyle                FontStyle               { get {return _style;}                   set  {_style = value;} }
        public FontWeight               FontWeight              { get {return _weight;}                  set  {_weight = value;} }
        public FontStretch              FontStretch             { get {return _stretch;}                 set  {_stretch = value;} }
        public double                   FontSize                { get {return _size;}                    set  {_size = value;} }
        public TextDecorationCollection TextDecorationCollection{ get {return _decoration;}              set  {_decoration = value;} }
        public Brush                    Foreground              { get {return _brush;}                   set  {_brush = value;} }
        public int                      AnnotationAlternates    { get {return _annotationAlternates;}    set  {_annotationAlternates = value;} }
        public FontCapitals             Capitals                { get {return _capitals;}                set  {_capitals = value;} }
        public bool                     CapitalSpacing          { get {return _capitalSpacing;}          set  {_capitalSpacing = value;} }
        public bool                     CaseSensitiveForms      { get {return _caseSensitiveForms;}      set  {_caseSensitiveForms = value;} }
        public bool                     ContextualAlternates    { get {return _contextualAlternates;}    set  {_contextualAlternates = value;} }
        public bool                     ContextualLigatures     { get {return _contextualLigatures;}     set  {_contextualLigatures = value;} }
        public int                      ContextualSwashes       { get {return _contextualSwashes;}       set  {_contextualSwashes = value;} }
        public bool                     DiscretionaryLigatures  { get {return _discretionaryLigatures;}  set  {_discretionaryLigatures = value;} }
        public bool                     EastAsianExpertForms    { get {return _eastAsianExpertForms;}    set  {_eastAsianExpertForms = value;} }
        public FontEastAsianLanguage    EastAsianLanguage       { get {return _eastAsianLanguage;}       set  {_eastAsianLanguage = value;} }
        public FontEastAsianWidths      EastAsianWidths         { get {return _eastAsianWidths;}         set  {_eastAsianWidths = value;} }
        public FontFraction             Fraction                { get {return _fraction;}                set  {_fraction = value;} }
        public bool                     HistoricalForms         { get {return _historicalForms;}         set  {_historicalForms = value;} }
        public bool                     HistoricalLigatures     { get {return _historicalLigatures;}     set  {_historicalLigatures = value;} }
        public bool                     Kerning                 { get {return _kerning;}                 set  {_kerning = value;} }
        public bool                     MathematicalGreek       { get {return _mathematicalGreek;}       set  {_mathematicalGreek = value;} }
        public FontNumeralAlignment     NumeralAlignment        { get {return _numeralAlignment;}        set  {_numeralAlignment = value;} }
        public FontNumeralStyle         NumeralStyle            { get {return _numeralStyle;}            set  {_numeralStyle = value;} }
        public bool                     SlashedZero             { get {return _slashedZero;}             set  {_slashedZero = value;} }
        public bool                     StandardLigatures       { get {return _standardLigatures;}       set  {_standardLigatures = value;} }
        public int                      StandardSwashes         { get {return _standardSwashes;}         set  {_standardSwashes = value;} }
        public int                      StylisticAlternates     { get {return _stylisticAlternates;}     set  {_stylisticAlternates = value;} }
        public bool                     StylisticSet1           { get {return _stylisticSet1;}           set  {_stylisticSet1 = value;} }
        public bool                     StylisticSet2           { get {return _stylisticSet2;}           set  {_stylisticSet2 = value;} }
        public bool                     StylisticSet3           { get {return _stylisticSet3;}           set  {_stylisticSet3 = value;} }
        public bool                     StylisticSet4           { get {return _stylisticSet4;}           set  {_stylisticSet4 = value;} }
        public bool                     StylisticSet5           { get {return _stylisticSet5;}           set  {_stylisticSet5 = value;} }
        public bool                     StylisticSet6           { get {return _stylisticSet6;}           set  {_stylisticSet6 = value;} }
        public bool                     StylisticSet7           { get {return _stylisticSet7;}           set  {_stylisticSet7 = value;} }
        public bool                     StylisticSet8           { get {return _stylisticSet8;}           set  {_stylisticSet8 = value;} }
        public bool                     StylisticSet9           { get {return _stylisticSet9;}           set  {_stylisticSet9 = value;} }
        public bool                     StylisticSet10          { get {return _stylisticSet10;}          set  {_stylisticSet10 = value;} }
        public bool                     StylisticSet11          { get {return _stylisticSet11;}          set  {_stylisticSet11 = value;} }
        public bool                     StylisticSet12          { get {return _stylisticSet12;}          set  {_stylisticSet12 = value;} }
        public bool                     StylisticSet13          { get {return _stylisticSet13;}          set  {_stylisticSet13 = value;} }
        public bool                     StylisticSet14          { get {return _stylisticSet14;}          set  {_stylisticSet14 = value;} }
        public bool                     StylisticSet15          { get {return _stylisticSet15;}          set  {_stylisticSet15 = value;} }
        public bool                     StylisticSet16          { get {return _stylisticSet16;}          set  {_stylisticSet16 = value;} }
        public bool                     StylisticSet17          { get {return _stylisticSet17;}          set  {_stylisticSet17 = value;} }
        public bool                     StylisticSet18          { get {return _stylisticSet18;}          set  {_stylisticSet18 = value;} }
        public bool                     StylisticSet19          { get {return _stylisticSet19;}          set  {_stylisticSet19 = value;} }
        public bool                     StylisticSet20          { get {return _stylisticSet20;}          set  {_stylisticSet20 = value;} }
        public FontVariants             Variants                { get {return _variants;}                set  {_variants = value;} }



        public FontChoice()
        {
            // All fields default
        }


        public FontChoice(FontChoice choice)
        {
            _family                 = choice._family;
            _style                  = choice._style;
            _weight                 = choice._weight;
            _stretch                = choice._stretch;
            _size                   = choice._size;
            _decoration             = choice._decoration;
            _brush                  = choice._brush;
            _annotationAlternates   = choice._annotationAlternates;
            _capitals               = choice._capitals;
            _capitalSpacing         = choice._capitalSpacing;
            _caseSensitiveForms     = choice._caseSensitiveForms;
            _contextualAlternates   = choice._contextualAlternates;
            _contextualLigatures    = choice._contextualLigatures;
            _contextualSwashes      = choice._contextualSwashes;
            _discretionaryLigatures = choice._discretionaryLigatures;
            _eastAsianExpertForms   = choice._eastAsianExpertForms;
            _eastAsianLanguage      = choice._eastAsianLanguage;
            _eastAsianWidths        = choice._eastAsianWidths;
            _fraction               = choice._fraction;
            _historicalForms        = choice._historicalForms;
            _historicalLigatures    = choice._historicalLigatures;
            _kerning                = choice._kerning;
            _mathematicalGreek      = choice._mathematicalGreek;
            _numeralAlignment       = choice._numeralAlignment;
            _numeralStyle           = choice._numeralStyle;
            _slashedZero            = choice._slashedZero;
            _standardLigatures      = choice._standardLigatures;
            _standardSwashes        = choice._standardSwashes;
            _stylisticAlternates    = choice._stylisticAlternates;
            _stylisticSet1          = choice._stylisticSet1;
            _stylisticSet2          = choice._stylisticSet2;
            _stylisticSet3          = choice._stylisticSet3;
            _stylisticSet4          = choice._stylisticSet4;
            _stylisticSet5          = choice._stylisticSet5;
            _stylisticSet6          = choice._stylisticSet6;
            _stylisticSet7          = choice._stylisticSet7;
            _stylisticSet8          = choice._stylisticSet8;
            _stylisticSet9          = choice._stylisticSet9;
            _stylisticSet10         = choice._stylisticSet10;
            _stylisticSet11         = choice._stylisticSet11;
            _stylisticSet12         = choice._stylisticSet12;
            _stylisticSet13         = choice._stylisticSet13;
            _stylisticSet14         = choice._stylisticSet14;
            _stylisticSet15         = choice._stylisticSet15;
            _stylisticSet16         = choice._stylisticSet16;
            _stylisticSet17         = choice._stylisticSet17;
            _stylisticSet18         = choice._stylisticSet18;
            _stylisticSet19         = choice._stylisticSet19;
            _stylisticSet20         = choice._stylisticSet20;
            _variants               = choice._variants;
        }


        public FontChoice(DependencyObject dependencyObject)
        {
            _family                 = dependencyObject.GetValue(TextElement.FontFamilyProperty)  as FontFamily;
            _style                  = (FontStyle)                dependencyObject.GetValue(TextElement.FontStyleProperty);
            _weight                 = (FontWeight)               dependencyObject.GetValue(TextElement.FontWeightProperty);
            _stretch                = (FontStretch)              dependencyObject.GetValue(TextElement.FontStretchProperty);
            _size                   = (double)                   dependencyObject.GetValue(TextElement.FontSizeProperty);
            _decoration             = dependencyObject.GetValue(Inline.TextDecorationsProperty)  as TextDecorationCollection;
            _brush                  = dependencyObject.GetValue(TextElement.ForegroundProperty)  as Brush;
            _annotationAlternates   = (int)                      dependencyObject.GetValue(Typography.AnnotationAlternatesProperty);
            _capitals               = (FontCapitals)             dependencyObject.GetValue(Typography.CapitalsProperty);
            _capitalSpacing         = (bool)                     dependencyObject.GetValue(Typography.CapitalSpacingProperty);
            _caseSensitiveForms     = (bool)                     dependencyObject.GetValue(Typography.CaseSensitiveFormsProperty);
            _contextualAlternates   = (bool)                     dependencyObject.GetValue(Typography.ContextualAlternatesProperty);
            _contextualLigatures    = (bool)                     dependencyObject.GetValue(Typography.ContextualLigaturesProperty);
            _contextualSwashes      = (int)                      dependencyObject.GetValue(Typography.ContextualSwashesProperty);
            _discretionaryLigatures = (bool)                     dependencyObject.GetValue(Typography.DiscretionaryLigaturesProperty);
            _eastAsianExpertForms   = (bool)                     dependencyObject.GetValue(Typography.EastAsianExpertFormsProperty);
            _eastAsianLanguage      = (FontEastAsianLanguage)    dependencyObject.GetValue(Typography.EastAsianLanguageProperty);
            _eastAsianWidths        = (FontEastAsianWidths)      dependencyObject.GetValue(Typography.EastAsianWidthsProperty);
            _fraction               = (FontFraction)             dependencyObject.GetValue(Typography.FractionProperty);
            _historicalForms        = (bool)                     dependencyObject.GetValue(Typography.HistoricalFormsProperty);
            _historicalLigatures    = (bool)                     dependencyObject.GetValue(Typography.HistoricalLigaturesProperty);
            _kerning                = (bool)                     dependencyObject.GetValue(Typography.KerningProperty);
            _mathematicalGreek      = (bool)                     dependencyObject.GetValue(Typography.MathematicalGreekProperty);
            _numeralAlignment       = (FontNumeralAlignment)     dependencyObject.GetValue(Typography.NumeralAlignmentProperty);
            _numeralStyle           = (FontNumeralStyle)         dependencyObject.GetValue(Typography.NumeralStyleProperty);
            _slashedZero            = (bool)                     dependencyObject.GetValue(Typography.SlashedZeroProperty);
            _standardLigatures      = (bool)                     dependencyObject.GetValue(Typography.StandardLigaturesProperty);
            _standardSwashes        = (int)                      dependencyObject.GetValue(Typography.StandardSwashesProperty);
            _stylisticAlternates    = (int)                      dependencyObject.GetValue(Typography.StylisticAlternatesProperty);
            _stylisticSet1          = (bool)                     dependencyObject.GetValue(Typography.StylisticSet1Property);
            _stylisticSet2          = (bool)                     dependencyObject.GetValue(Typography.StylisticSet2Property);
            _stylisticSet3          = (bool)                     dependencyObject.GetValue(Typography.StylisticSet3Property);
            _stylisticSet4          = (bool)                     dependencyObject.GetValue(Typography.StylisticSet4Property);
            _stylisticSet5          = (bool)                     dependencyObject.GetValue(Typography.StylisticSet5Property);
            _stylisticSet6          = (bool)                     dependencyObject.GetValue(Typography.StylisticSet6Property);
            _stylisticSet7          = (bool)                     dependencyObject.GetValue(Typography.StylisticSet7Property);
            _stylisticSet8          = (bool)                     dependencyObject.GetValue(Typography.StylisticSet8Property);
            _stylisticSet9          = (bool)                     dependencyObject.GetValue(Typography.StylisticSet9Property);
            _stylisticSet10         = (bool)                     dependencyObject.GetValue(Typography.StylisticSet10Property);
            _stylisticSet11         = (bool)                     dependencyObject.GetValue(Typography.StylisticSet11Property);
            _stylisticSet12         = (bool)                     dependencyObject.GetValue(Typography.StylisticSet12Property);
            _stylisticSet13         = (bool)                     dependencyObject.GetValue(Typography.StylisticSet13Property);
            _stylisticSet14         = (bool)                     dependencyObject.GetValue(Typography.StylisticSet14Property);
            _stylisticSet15         = (bool)                     dependencyObject.GetValue(Typography.StylisticSet15Property);
            _stylisticSet16         = (bool)                     dependencyObject.GetValue(Typography.StylisticSet16Property);
            _stylisticSet17         = (bool)                     dependencyObject.GetValue(Typography.StylisticSet17Property);
            _stylisticSet18         = (bool)                     dependencyObject.GetValue(Typography.StylisticSet18Property);
            _stylisticSet19         = (bool)                     dependencyObject.GetValue(Typography.StylisticSet19Property);
            _stylisticSet20         = (bool)                     dependencyObject.GetValue(Typography.StylisticSet20Property);
            _variants               = (FontVariants)             dependencyObject.GetValue(Typography.VariantsProperty);
        }

        public void ApplyToRange(RichTextBox richTextBox)
        {
            TextRange range = richTextBox.Selection;

            using (richTextBox.DeclareChangeBlock())
            {
                range.ApplyPropertyValue(TextElement.FontFamilyProperty,              _family);
                range.ApplyPropertyValue(TextElement.FontStyleProperty,               _style);
                range.ApplyPropertyValue(TextElement.FontWeightProperty,              _weight);
                range.ApplyPropertyValue(TextElement.FontStretchProperty,             _stretch);
                range.ApplyPropertyValue(TextElement.FontSizeProperty,                _size);
                range.ApplyPropertyValue(Inline.TextDecorationsProperty,              _decoration);
                range.ApplyPropertyValue(TextElement.ForegroundProperty,              _brush);
                range.ApplyPropertyValue(Typography.AnnotationAlternatesProperty,     _annotationAlternates);
                range.ApplyPropertyValue(Typography.CapitalsProperty,                 _capitals);
                range.ApplyPropertyValue(Typography.CapitalSpacingProperty,           _capitalSpacing);
                range.ApplyPropertyValue(Typography.CaseSensitiveFormsProperty,       _caseSensitiveForms);
                range.ApplyPropertyValue(Typography.ContextualAlternatesProperty,     _contextualAlternates);
                range.ApplyPropertyValue(Typography.ContextualLigaturesProperty,      _contextualLigatures);
                range.ApplyPropertyValue(Typography.ContextualSwashesProperty,        _contextualSwashes);
                range.ApplyPropertyValue(Typography.DiscretionaryLigaturesProperty,   _discretionaryLigatures);
                range.ApplyPropertyValue(Typography.EastAsianExpertFormsProperty,     _eastAsianExpertForms);
                range.ApplyPropertyValue(Typography.EastAsianLanguageProperty,        _eastAsianLanguage);
                range.ApplyPropertyValue(Typography.EastAsianWidthsProperty,          _eastAsianWidths);
                range.ApplyPropertyValue(Typography.FractionProperty,                 _fraction);
                range.ApplyPropertyValue(Typography.HistoricalFormsProperty,          _historicalForms);
                range.ApplyPropertyValue(Typography.HistoricalLigaturesProperty,      _historicalLigatures);
                range.ApplyPropertyValue(Typography.KerningProperty,                  _kerning);
                range.ApplyPropertyValue(Typography.MathematicalGreekProperty,        _mathematicalGreek);
                range.ApplyPropertyValue(Typography.NumeralAlignmentProperty,         _numeralAlignment);
                range.ApplyPropertyValue(Typography.NumeralStyleProperty,             _numeralStyle);
                range.ApplyPropertyValue(Typography.SlashedZeroProperty,              _slashedZero);
                range.ApplyPropertyValue(Typography.StandardLigaturesProperty,        _standardLigatures);
                range.ApplyPropertyValue(Typography.StandardSwashesProperty,          _standardSwashes);
                range.ApplyPropertyValue(Typography.StylisticAlternatesProperty,      _stylisticAlternates);
                range.ApplyPropertyValue(Typography.StylisticSet1Property,            _stylisticSet1);
                range.ApplyPropertyValue(Typography.StylisticSet2Property,            _stylisticSet2);
                range.ApplyPropertyValue(Typography.StylisticSet3Property,            _stylisticSet3);
                range.ApplyPropertyValue(Typography.StylisticSet4Property,            _stylisticSet4);
                range.ApplyPropertyValue(Typography.StylisticSet5Property,            _stylisticSet5);
                range.ApplyPropertyValue(Typography.StylisticSet6Property,            _stylisticSet6);
                range.ApplyPropertyValue(Typography.StylisticSet7Property,            _stylisticSet7);
                range.ApplyPropertyValue(Typography.StylisticSet8Property,            _stylisticSet8);
                range.ApplyPropertyValue(Typography.StylisticSet9Property,            _stylisticSet9);
                range.ApplyPropertyValue(Typography.StylisticSet10Property,           _stylisticSet10);
                range.ApplyPropertyValue(Typography.StylisticSet11Property,           _stylisticSet11);
                range.ApplyPropertyValue(Typography.StylisticSet12Property,           _stylisticSet12);
                range.ApplyPropertyValue(Typography.StylisticSet13Property,           _stylisticSet13);
                range.ApplyPropertyValue(Typography.StylisticSet14Property,           _stylisticSet14);
                range.ApplyPropertyValue(Typography.StylisticSet15Property,           _stylisticSet15);
                range.ApplyPropertyValue(Typography.StylisticSet16Property,           _stylisticSet16);
                range.ApplyPropertyValue(Typography.StylisticSet17Property,           _stylisticSet17);
                range.ApplyPropertyValue(Typography.StylisticSet18Property,           _stylisticSet18);
                range.ApplyPropertyValue(Typography.StylisticSet19Property,           _stylisticSet19);
                range.ApplyPropertyValue(Typography.StylisticSet20Property,           _stylisticSet20);
                range.ApplyPropertyValue(Typography.VariantsProperty,                 _variants);
            }
        }

        public void ApplyToDependencyObject(DependencyObject dependencyObject)
        {
            dependencyObject.SetValue(TextElement.FontFamilyProperty,            _family);
            dependencyObject.SetValue(TextElement.FontStyleProperty,             _style);
            dependencyObject.SetValue(TextElement.FontWeightProperty,            _weight);
            dependencyObject.SetValue(TextElement.FontStretchProperty,           _stretch);
            dependencyObject.SetValue(TextElement.FontSizeProperty,              _size);
            dependencyObject.SetValue(Inline.TextDecorationsProperty,            _decoration);
            dependencyObject.SetValue(TextElement.ForegroundProperty,            _brush);
            dependencyObject.SetValue(Typography.AnnotationAlternatesProperty,   _annotationAlternates);
            dependencyObject.SetValue(Typography.CapitalsProperty,               _capitals);
            dependencyObject.SetValue(Typography.CapitalSpacingProperty,         _capitalSpacing);
            dependencyObject.SetValue(Typography.CaseSensitiveFormsProperty,     _caseSensitiveForms);
            dependencyObject.SetValue(Typography.ContextualAlternatesProperty,   _contextualAlternates);
            dependencyObject.SetValue(Typography.ContextualLigaturesProperty,    _contextualLigatures);
            dependencyObject.SetValue(Typography.ContextualSwashesProperty,      _contextualSwashes);
            dependencyObject.SetValue(Typography.DiscretionaryLigaturesProperty, _discretionaryLigatures);
            dependencyObject.SetValue(Typography.EastAsianExpertFormsProperty,   _eastAsianExpertForms);
            dependencyObject.SetValue(Typography.EastAsianLanguageProperty,      _eastAsianLanguage);
            dependencyObject.SetValue(Typography.EastAsianWidthsProperty,        _eastAsianWidths);
            dependencyObject.SetValue(Typography.FractionProperty,               _fraction);
            dependencyObject.SetValue(Typography.HistoricalFormsProperty,        _historicalForms);
            dependencyObject.SetValue(Typography.HistoricalLigaturesProperty,    _historicalLigatures);
            dependencyObject.SetValue(Typography.KerningProperty,                _kerning);
            dependencyObject.SetValue(Typography.MathematicalGreekProperty,      _mathematicalGreek);
            dependencyObject.SetValue(Typography.NumeralAlignmentProperty,       _numeralAlignment);
            dependencyObject.SetValue(Typography.NumeralStyleProperty,           _numeralStyle);
            dependencyObject.SetValue(Typography.SlashedZeroProperty,            _slashedZero);
            dependencyObject.SetValue(Typography.StandardLigaturesProperty,      _standardLigatures);
            dependencyObject.SetValue(Typography.StandardSwashesProperty,        _standardSwashes);
            dependencyObject.SetValue(Typography.StylisticAlternatesProperty,    _stylisticAlternates);
            dependencyObject.SetValue(Typography.StylisticSet1Property,          _stylisticSet1);
            dependencyObject.SetValue(Typography.StylisticSet2Property,          _stylisticSet2);
            dependencyObject.SetValue(Typography.StylisticSet3Property,          _stylisticSet3);
            dependencyObject.SetValue(Typography.StylisticSet4Property,          _stylisticSet4);
            dependencyObject.SetValue(Typography.StylisticSet5Property,          _stylisticSet5);
            dependencyObject.SetValue(Typography.StylisticSet6Property,          _stylisticSet6);
            dependencyObject.SetValue(Typography.StylisticSet7Property,          _stylisticSet7);
            dependencyObject.SetValue(Typography.StylisticSet8Property,          _stylisticSet8);
            dependencyObject.SetValue(Typography.StylisticSet9Property,          _stylisticSet9);
            dependencyObject.SetValue(Typography.StylisticSet10Property,         _stylisticSet10);
            dependencyObject.SetValue(Typography.StylisticSet11Property,         _stylisticSet11);
            dependencyObject.SetValue(Typography.StylisticSet12Property,         _stylisticSet12);
            dependencyObject.SetValue(Typography.StylisticSet13Property,         _stylisticSet13);
            dependencyObject.SetValue(Typography.StylisticSet14Property,         _stylisticSet14);
            dependencyObject.SetValue(Typography.StylisticSet15Property,         _stylisticSet15);
            dependencyObject.SetValue(Typography.StylisticSet16Property,         _stylisticSet16);
            dependencyObject.SetValue(Typography.StylisticSet17Property,         _stylisticSet17);
            dependencyObject.SetValue(Typography.StylisticSet18Property,         _stylisticSet18);
            dependencyObject.SetValue(Typography.StylisticSet19Property,         _stylisticSet19);
            dependencyObject.SetValue(Typography.StylisticSet20Property,         _stylisticSet20);
            dependencyObject.SetValue(Typography.VariantsProperty,               _variants);
        }
    }
}