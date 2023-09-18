// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides helper classes for character formatting tests.

using System;
using System.ComponentModel;
using Bitmap = System.Drawing.Bitmap;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using Microsoft.Test.Imaging;

using Test.Uis.Data;
using Test.Uis.Loggers;
using Test.Uis.Management;
using Test.Uis.TestTypes;
using Test.Uis.Utils;

namespace Test.Uis.TextEditing
{    
    /// <summary>
    /// Provides information about character formatting functionality
    /// available in the platform.
    /// </summary>
    public sealed class CharacterFormattingHelper
    {
        #region Constructors.

        /// <summary>Hide the constructor.</summary>
        private CharacterFormattingHelper() { }

        #endregion Constructors.

        #region Public properties.

        /// <summary>
        /// Gets an array of properties associated with character
        /// formatting.
        /// </summary>
        public static DependencyProperty[] FormattingProperties
        {
            get
            {
                if (s_formattingProperties == null)
                {
                    s_formattingProperties = new DependencyProperty[] {
                        // TextElement properties.
                        TextElement.FontFamilyProperty,
                        TextElement.FontFamilyProperty,
                        TextElement.FontStyleProperty,
                        TextElement.FontWeightProperty,
                        TextElement.FontStretchProperty,
                        TextElement.FontSizeProperty,
                        TextElement.ForegroundProperty,
                        TextElement.BackgroundProperty,                        
                        Paragraph.TextDecorationsProperty,
                        Block.MarginProperty,
                        Block.PaddingProperty,
                        Block.BorderThicknessProperty,
                        Block.BorderBrushProperty,

                        // Typography properties.
                        Typography.StandardLigaturesProperty,
                        Typography.ContextualLigaturesProperty,
                        Typography.DiscretionaryLigaturesProperty,
                        Typography.HistoricalLigaturesProperty,
                        Typography.ContextualAlternatesProperty,
                        Typography.HistoricalFormsProperty,
                        Typography.KerningProperty,
                        Typography.CapitalSpacingProperty,
                        Typography.CaseSensitiveFormsProperty,
                        Typography.StylisticSet1Property,
                        Typography.StylisticSet2Property,
                        Typography.StylisticSet3Property,
                        Typography.StylisticSet4Property,
                        Typography.StylisticSet5Property,
                        Typography.StylisticSet6Property,
                        Typography.StylisticSet7Property,
                        Typography.StylisticSet8Property,
                        Typography.StylisticSet9Property,
                        Typography.StylisticSet10Property,
                        Typography.StylisticSet11Property,
                        Typography.StylisticSet12Property,
                        Typography.StylisticSet13Property,
                        Typography.StylisticSet14Property,
                        Typography.StylisticSet15Property,
                        Typography.StylisticSet16Property,
                        Typography.StylisticSet17Property,
                        Typography.StylisticSet18Property,
                        Typography.StylisticSet19Property,
                        Typography.StylisticSet20Property,
                        Typography.FractionProperty,
                        Typography.SlashedZeroProperty,
                        Typography.MathematicalGreekProperty,
                        Typography.EastAsianExpertFormsProperty,
                        Typography.VariantsProperty,
                        Typography.CapitalsProperty,
                        Typography.NumeralStyleProperty,
                        Typography.NumeralAlignmentProperty,
                        Typography.EastAsianWidthsProperty,
                        Typography.EastAsianLanguageProperty,
                        Typography.StandardSwashesProperty,
                        Typography.ContextualSwashesProperty,
                        Typography.StylisticAlternatesProperty,
                        Typography.AnnotationAlternatesProperty,
                    };
                }
                return s_formattingProperties;
            }
        }
        
        #endregion Public properties.

        #region Public methods.

        /// <summary>
        /// Returns an array of interesting and valid values for the 
        /// specified formatting property.
        /// </summary>
        public static object[] ListFormattingPropertyValues(DependencyProperty formattingProperty)
        {
            if (formattingProperty == null)
            {
                throw new ArgumentNullException("formattingProperty");
            }
            if (System.Array.IndexOf(FormattingProperties, formattingProperty) == -1)
            {
                throw new ArgumentException(
                    "Property is not a formatting property: " + formattingProperty.ToString());
            }
            if (s_propertyValues == null)
            {
                object[] booleanValues = new object[] { false, true };
                Pen blackPen = new Pen(Brushes.Black, 2f);
                TextDecorationCollection tempTDC = new TextDecorationCollection();
                tempTDC.Add(new TextDecoration(TextDecorationLocation.Baseline, blackPen, 3, TextDecorationUnit.Pixel, TextDecorationUnit.Pixel));

                s_propertyValues = new PropertyValues[] {
                    new PropertyValues(TextElement.FontFamilyProperty, new object[] {
                        new FontFamily("Times New Roman"), new FontFamily("Tahoma"), new FontFamily("Arial")
                    }),
                    new PropertyValues(TextElement.FontStyleProperty, new object[] {
                        FontStyles.Italic, FontStyles.Normal, FontStyles.Oblique
                    }),
                    new PropertyValues(TextElement.FontWeightProperty, new object[] {
                        FontWeights.Black, FontWeights.Bold, FontWeights.ExtraBold, FontWeights.ExtraLight, FontWeights.Light, FontWeights.Medium, FontWeights.Normal, FontWeights.SemiBold, FontWeights.Thin,
                    }),
                    new PropertyValues(TextElement.FontStretchProperty, new object[] {
                        FontStretches.Condensed, FontStretches.Expanded, FontStretches.ExtraCondensed, FontStretches.ExtraExpanded, FontStretches.Normal, FontStretches.SemiCondensed, FontStretches.SemiExpanded, FontStretches.UltraCondensed, FontStretches.UltraExpanded,
                    }),
                    new PropertyValues(TextElement.FontSizeProperty, new object[] {
                        2, 12, 42, 
                    }),
                    new PropertyValues(TextElement.ForegroundProperty, new object[] {
                        Brushes.Black, Brushes.Transparent, Brushes.Blue
                    }),
                    new PropertyValues(Paragraph.TextDecorationsProperty, new object[] {
                        new TextDecorationCollection(), tempTDC,
                    }),
                    new PropertyValues(TextElement.BackgroundProperty, new object[] {
                        Brushes.White, Brushes.Transparent, Brushes.Red,
                    }),
                    new PropertyValues(Block.MarginProperty, new object[] {
                        new Thickness(0), new Thickness(4), new Thickness(12),
                    }),
                    new PropertyValues(Block.PaddingProperty, new object[] {
                        new Thickness(0), new Thickness(4), new Thickness(12),
                    }),
                    new PropertyValues(Block.BorderThicknessProperty, new object[] {
                        new Thickness(0), new Thickness(4), new Thickness(12),
                    }),
                    new PropertyValues(Block.BorderBrushProperty, new object[] {
                        Brushes.Yellow, Brushes.Transparent, Brushes.Green
                    }),

                    // Typography properties.
                    new PropertyValues(Typography.StandardLigaturesProperty, booleanValues),
                    new PropertyValues(Typography.ContextualLigaturesProperty, booleanValues),
                    new PropertyValues(Typography.DiscretionaryLigaturesProperty, booleanValues),
                    new PropertyValues(Typography.HistoricalLigaturesProperty, booleanValues),
                    new PropertyValues(Typography.ContextualAlternatesProperty, booleanValues),
                    new PropertyValues(Typography.HistoricalFormsProperty, booleanValues),
                    new PropertyValues(Typography.KerningProperty, booleanValues),
                    new PropertyValues(Typography.CapitalSpacingProperty, booleanValues),
                    new PropertyValues(Typography.CaseSensitiveFormsProperty, booleanValues),
                    new PropertyValues(Typography.StylisticSet1Property, booleanValues),
                    new PropertyValues(Typography.StylisticSet2Property, booleanValues),
                    new PropertyValues(Typography.StylisticSet3Property, booleanValues),
                    new PropertyValues(Typography.StylisticSet4Property, booleanValues),
                    new PropertyValues(Typography.StylisticSet5Property, booleanValues),
                    new PropertyValues(Typography.StylisticSet6Property, booleanValues),
                    new PropertyValues(Typography.StylisticSet7Property, booleanValues),
                    new PropertyValues(Typography.StylisticSet8Property, booleanValues),
                    new PropertyValues(Typography.StylisticSet9Property, booleanValues),
                    new PropertyValues(Typography.StylisticSet10Property, booleanValues),
                    new PropertyValues(Typography.StylisticSet11Property, booleanValues),
                    new PropertyValues(Typography.StylisticSet12Property, booleanValues),
                    new PropertyValues(Typography.StylisticSet13Property, booleanValues),
                    new PropertyValues(Typography.StylisticSet14Property, booleanValues),
                    new PropertyValues(Typography.StylisticSet15Property, booleanValues),
                    new PropertyValues(Typography.StylisticSet16Property, booleanValues),
                    new PropertyValues(Typography.StylisticSet17Property, booleanValues),
                    new PropertyValues(Typography.StylisticSet18Property, booleanValues),
                    new PropertyValues(Typography.StylisticSet19Property, booleanValues),
                    new PropertyValues(Typography.StylisticSet20Property, booleanValues),
                    new PropertyValues(Typography.FractionProperty, new object[] {FontFraction.Normal, FontFraction.Slashed, FontFraction.Stacked}),
                    new PropertyValues(Typography.SlashedZeroProperty, booleanValues),
                    new PropertyValues(Typography.MathematicalGreekProperty, booleanValues),
                    new PropertyValues(Typography.EastAsianExpertFormsProperty, booleanValues),
                    new PropertyValues(Typography.VariantsProperty, new object[] {
                        FontVariants.Inferior, FontVariants.Normal, FontVariants.Ordinal, FontVariants.Ruby, FontVariants.Subscript, FontVariants.Superscript,
                    }),
                    new PropertyValues(Typography.CapitalsProperty, new object[] {
                        FontCapitals.AllPetiteCaps, FontCapitals.AllSmallCaps, FontCapitals.Normal, FontCapitals.PetiteCaps, FontCapitals.SmallCaps, FontCapitals.Titling, FontCapitals.Unicase,
                    }),
                    new PropertyValues(Typography.NumeralStyleProperty, new object[] {
                        FontNumeralStyle.Lining, FontNumeralStyle.Normal, FontNumeralStyle.OldStyle
                    }),
                    new PropertyValues(Typography.NumeralAlignmentProperty, new object[] {
                        FontNumeralAlignment.Normal, FontNumeralAlignment.Proportional, FontNumeralAlignment.Tabular
                    }),
                    new PropertyValues(Typography.EastAsianWidthsProperty, new object[] {
                        FontEastAsianWidths.Full, FontEastAsianWidths.Half, FontEastAsianWidths.Normal, FontEastAsianWidths.Proportional, FontEastAsianWidths.Quarter, FontEastAsianWidths.Third,
                    }),
                    new PropertyValues(Typography.EastAsianLanguageProperty, new object[] {
                        FontEastAsianLanguage.Jis78, FontEastAsianLanguage.Jis83, FontEastAsianLanguage.Jis90, FontEastAsianLanguage.Jis04, FontEastAsianLanguage.NlcKanji, FontEastAsianLanguage.Normal, FontEastAsianLanguage.Simplified, FontEastAsianLanguage.Traditional, FontEastAsianLanguage.TraditionalNames,
                    }),
                    new PropertyValues(Typography.StandardSwashesProperty, new object[] {
                        0, 42, 3
                    }),
                    new PropertyValues(Typography.ContextualSwashesProperty, new object[] {
                        0, 42, 3
                    }),
                    new PropertyValues(Typography.StylisticAlternatesProperty, new object[] {
                        0, 42, 3
                    }),
                    new PropertyValues(Typography.AnnotationAlternatesProperty, new object[] {
                        0, 42, 3
                    }),
                };
            }
            for (int i = 0; i < s_propertyValues.Length; i++)
            {
                if (s_propertyValues[i].Property == formattingProperty)
                {
                    return s_propertyValues[i].Values;
                }
            }
            return new object[0];
        }

        /// <summary>
        /// Verifies whether the formatting attribute specified by format parameter exists
        /// at TextPointer tp
        /// </summary>
        /// <param name="tp">TextPointer at which the formatting attribute should be checked</param>
        /// <param name="format">String representation of the formatting attribute to be checked for</param>
        /// <returns>True if formatting attribute exists at TextPointer tp orelse false</returns>
        public static bool VerifyFormatting(TextPointer tp, string format)
        {
            object _testFormatValue;
            bool _condition;
            
            if (format.ToLower(System.Globalization.CultureInfo.InvariantCulture).Contains("bold"))
            {
                _testFormatValue = tp.Parent.GetValue(TextElement.FontWeightProperty);
                _condition = ((FontWeight)_testFormatValue == FontWeights.Bold);                
            }
            else if (format.ToLower(System.Globalization.CultureInfo.InvariantCulture).Contains("italic"))
            {
                _testFormatValue = tp.Parent.GetValue(TextElement.FontStyleProperty);
                _condition = ((System.Windows.FontStyle)_testFormatValue == System.Windows.FontStyles.Italic);
            }
            else if (format.ToLower(System.Globalization.CultureInfo.InvariantCulture).Contains("underline"))
            {
                _testFormatValue = tp.Parent.GetValue(Inline.TextDecorationsProperty);
                _condition = ((TextDecorationCollection)_testFormatValue == TextDecorations.Underline);
            }
            else
            {
                throw new Exception("Only Bold/Italic/Underline formats are supported");
            }

            if (_condition)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns equivalent command string for the operation specified
        /// </summary>
        /// <param name="operation">Operation for which command string is seeked.</param>
        /// <returns>Command string</returns>
        public static string GetCommandString(string operation)
        {
            if (operation.ToLower(System.Globalization.CultureInfo.InvariantCulture).Contains("bold"))
            {                
                return EditingCommandData.ToggleBold.KeyboardShortcut;
            }
            else if (operation.ToLower(System.Globalization.CultureInfo.InvariantCulture).Contains("italic"))
            {                
                return EditingCommandData.ToggleItalic.KeyboardShortcut;
            }
            else if (operation.ToLower(System.Globalization.CultureInfo.InvariantCulture).Contains("underline"))
            {
                return EditingCommandData.ToggleUnderline.KeyboardShortcut;
            }
            else
            {
                throw new Exception("Invalid operation string [" + operation + "]"); 
            }
        }

        #endregion Public methods.

        #region Private fields.

        private static DependencyProperty[] s_formattingProperties;
        private static PropertyValues[] s_propertyValues;

        #endregion Private fields.

        #region Inner types.

        class PropertyValues
        {
            public DependencyProperty Property { get { return this._property; } }
            public object[] Values { get { return this._values; } }

            private readonly DependencyProperty _property;
            private readonly object[] _values;

            public PropertyValues(DependencyProperty property, object[] values)
            {
                this._values = values;
                this._property = property;
            }
        }
        
        #endregion Inner types.
    }
}
