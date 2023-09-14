// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Test suite for TextElements.
//

using System.Windows;               // Style, Font*
using System.Windows.Media;         // Brushes
using System.Windows.Documents;     // Bold, Inline, ...

namespace DRT
{
    // ----------------------------------------------------------------------
    // Test suite for TextElements.
    // ----------------------------------------------------------------------
    internal sealed class TextElementsSuite : DrtTestSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal TextElementsSuite() : base("TextElements")
        {
             this.Contact = "Microsoft";
        }

        // ------------------------------------------------------------------
        // Initialize tests.
        // ------------------------------------------------------------------
        public override DrtTest[] PrepareTests()
        {
            // Return the lists of tests to run against the tree
            return new DrtTest[] {
                new DrtTest(BoldStyling),
                new DrtTest(ItalicStyling),
                new DrtTest(UnderlineStyling),
            };
        }

        // ------------------------------------------------------------------
        // BoldStyling
        // ------------------------------------------------------------------
        private void BoldStyling()
        {
            Bold element = new Bold();
            Initialize(element);
            DRT.Assert(element.FontWeight == FontWeights.Bold, "BoldStyling 1: Expecting FontWeight={0}, got {1}.", FontWeights.Bold, element.FontWeight);

            Style style1 = new Style(typeof(Bold));
            style1.Setters.Add(new Setter(Bold.ForegroundProperty, Brushes.DarkRed));
            element.Style = style1;
            DRT.Assert(element.FontWeight == FontWeights.Bold, "BoldStyling 2: Expecting FontWeight={0}, got {1}.", FontWeights.Bold, element.FontWeight);

            Style style2 = new Style(typeof(Bold));
            style2.Setters.Add(new Setter(Bold.ForegroundProperty, Brushes.DarkRed));
            style2.Setters.Add(new Setter(Bold.FontWeightProperty, FontWeights.Normal));
            element.Style = style2;
            DRT.Assert(element.FontWeight == FontWeights.Normal, "BoldStyling 3: Expecting FontWeight={0}, got {1}.", FontWeights.Normal, element.FontWeight);

            Style style3 = new Style(typeof(TextElement));
            style3.Setters.Add(new Setter(Bold.ForegroundProperty, Brushes.DarkRed));
            element.Style = style3;
            DRT.Assert(element.FontWeight == FontWeights.Bold, "BoldStyling 4: Expecting FontWeight={0}, got {1}.", FontWeights.Bold, element.FontWeight);
        }

        // ------------------------------------------------------------------
        // ItalicStyling
        // ------------------------------------------------------------------
        private void ItalicStyling()
        {
            Italic element = new Italic();
            Initialize(element);
            DRT.Assert(element.FontStyle == FontStyles.Italic, "ItalicStyling 1: Expecting FontStyle={0}, got {1}.", FontStyles.Italic, element.FontStyle);

            Style style1 = new Style(typeof(Italic));
            style1.Setters.Add(new Setter(Italic.ForegroundProperty, Brushes.DarkRed));
            element.Style = style1;
            DRT.Assert(element.FontStyle == FontStyles.Italic, "ItalicStyling 2: Expecting FontStyle={0}, got {1}.", FontStyles.Italic, element.FontStyle);

            Style style2 = new Style(typeof(Italic));
            style2.Setters.Add(new Setter(Italic.ForegroundProperty, Brushes.DarkRed));
            style2.Setters.Add(new Setter(Italic.FontStyleProperty, FontStyles.Normal));
            element.Style = style2;
            DRT.Assert(element.FontStyle == FontStyles.Normal, "ItalicStyling 3: Expecting FontStyle={0}, got {1}.", FontStyles.Normal, element.FontStyle);

            Style style3 = new Style(typeof(TextElement));
            style3.Setters.Add(new Setter(Italic.ForegroundProperty, Brushes.DarkRed));
            element.Style = style3;
            DRT.Assert(element.FontStyle == FontStyles.Italic, "ItalicStyling 4: Expecting FontStyle={0}, got {1}.", FontStyles.Italic, element.FontStyle);
        }

        // ------------------------------------------------------------------
        // UnderlineStyling
        // ------------------------------------------------------------------
        private void UnderlineStyling()
        {
            TextDecorationCollection tdNone = new TextDecorationCollection();
            Underline element = new Underline();
            Initialize(element);
            DRT.Assert(IsUnderline(element.TextDecorations), "UnderlineStyling 1: Expecting TextDecorations.Underline.");

            Style style1 = new Style(typeof(Underline));
            style1.Setters.Add(new Setter(Underline.ForegroundProperty, Brushes.DarkRed));
            element.Style = style1;
            DRT.Assert(IsUnderline(element.TextDecorations), "UnderlineStyling 2: Expecting TextDecorations.Underline.");

            Style style2 = new Style(typeof(Underline));
            style2.Setters.Add(new Setter(Underline.ForegroundProperty, Brushes.DarkRed));
            style2.Setters.Add(new Setter(Underline.TextDecorationsProperty, tdNone));
            element.Style = style2;
            DRT.Assert(element.TextDecorations.Count == 0, "UnderlineStyling 3: Expecting TextDecorations.None.");

            Style style3 = new Style(typeof(TextElement));
            style3.Setters.Add(new Setter(Underline.ForegroundProperty, Brushes.DarkRed));
            element.Style = style3;
            DRT.Assert(IsUnderline(element.TextDecorations), "UnderlineStyling 4: Expecting TextDecorations.Underline.");
        }

        private bool IsUnderline(TextDecorationCollection textDecorations)
        {
            return textDecorations != null 
                && textDecorations.Count == 1
                && textDecorations[0].Location == TextDecorations.Underline[0].Location;
        }

        // ------------------------------------------------------------------
        // Initialize FCE
        // ------------------------------------------------------------------
        private void Initialize(FrameworkContentElement fce)
        {
            fce.BeginInit();
            fce.EndInit();
        }

    }
}
