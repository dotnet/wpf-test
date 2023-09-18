// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Few tests with TextOptions.TextFormattingMode set to Ideal and Display

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Collections.Generic;     
    using System.Globalization;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Threading;

    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;
    using Microsoft.Test.Logging;
    using Microsoft.Test.Input;
    using Microsoft.Test.Threading;

    using Test.Uis.Data;    
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;   
    
    #endregion Namespaces.

    /// <summary>
    /// Verifies that Font formatting works in RichTextBox control in a more complex scenario.
    /// It first selects some text, insert an element, change the fontsize and fontfamily properties,
    /// overwrites the text in that selection and verifies FontSize and FontFamily properties are retained.
    /// </summary>
    [Test(0, "RichEditing", "ComplexFontFormattingTestsInDisplayMode", MethodParameters = "/TestCaseType:ComplexFontFormattingTests /TextFormattingmode=Display", Keywords = "TextFormattingModeTests")]
    [Test(0, "RichEditing", "ComplexFontFormattingTestsInIdealMode", MethodParameters = "/TestCaseType:ComplexFontFormattingTests /TextFormattingmode=Ideal", Keywords = "TextFormattingModeTests")]
    public class ComplexFontFormattingTests : CustomTestCase
    {
        #region Settings
        /// <summary>TextBlock control which is being tested.</summary>
        private RichTextBox TextControl
        {
            get { return _textControl; }
            set { _textControl = value; }
        }

        /// <summary>FontSize to be used in the test.</summary>
        private int FontSize
        {
            get
            {
                if (Settings.GetArgumentAsInt("FontSize") != 0)
                    return Settings.GetArgumentAsInt("FontSize");
                else
                    return (48); //default value
            }
        }

        /// <summary>FontFamily to be used in the test.</summary>
        private FontFamily FontFamily
        {
            get
            {
                if (Settings.GetArgument("FontFamily") != String.Empty)
                    return new FontFamily(Settings.GetArgument("FontFamily"));
                else
                    return new FontFamily("Castellar"); //default value
            }
        }
        #endregion Settings

        #region members
        RichTextBox _textControl = null;
        private UIElementWrapper _textControlWrapper;

        private FontFamily _systemFontFamily = SystemFonts.MessageFontFamily;
        private double _systemFontSize = SystemFonts.MessageFontSize;

        private delegate void FormatDelegate(bool testBold, bool testItalic, bool testUnderline);
        #endregion members

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            #region Setup
            TextControl = new RichTextBox();

            switch (ConfigurationSettings.Current.GetArgument("TextFormattingmode"))
            {
                case "Display": TextOptions.SetTextFormattingMode(TextControl, TextFormattingMode.Display);
                    break;
                case "Ideal": TextOptions.SetTextFormattingMode(TextControl, TextFormattingMode.Ideal);
                    break;
            }

            _textControlWrapper = new UIElementWrapper(TextControl);
            _textControlWrapper.Text = "This is a test";

            MainWindow.Content = (UIElement)TextControl;
            #endregion Setup

            QueueHelper.Current.QueueDelegate(new SimpleHandler(SelectText));
        }

        /// <summary> Selects some text in the text control </summary>
        private void SelectText()
        {
            TextControl.Focus();
            TextSelection textSelection = _textControlWrapper.SelectionInstance;
            textSelection.Select(
                _textControl.Document.Blocks.FirstBlock.ContentStart,
                _textControl.Document.Blocks.FirstBlock.ContentStart.GetPositionAtOffset(7));

            textSelection.ApplyPropertyValue(TextElement.FontSizeProperty, (double)FontSize);
            textSelection.ApplyPropertyValue(TextElement.FontFamilyProperty, FontFamily);

            QueueHelper.Current.QueueDelegate(new SimpleHandler(FontFormat));
        }

        private void FontFormat()
        {
            KeyboardInput.TypeString("This is"); //overwrite the selected text to check if properties are retained.
            QueueHelper.Current.QueueDelegate(new SimpleHandler(VerifyFontFormat));
        }

        private void VerifyFontFormat()
        {
            object testFontFamilyValue = null;
            object testFontSizeValue = null;

            TextPointer tpPart1 = _textControl.Document.Blocks.FirstBlock.ContentStart;
            TextPointer tpPart2 = _textControl.Document.Blocks.FirstBlock.ContentStart;
            tpPart1 = tpPart1.GetPositionAtOffset(2);
            tpPart2 = tpPart2.GetPositionAtOffset(10);

            testFontFamilyValue = tpPart1.Parent.GetValue(RichTextBox.FontFamilyProperty);
            testFontSizeValue = tpPart1.Parent.GetValue(RichTextBox.FontSizeProperty);

            Log("Actual FontFamily [" + ((FontFamily)testFontFamilyValue).Source + "] Expected FontFamily [" + FontFamily + "]");
            Verifier.Verify((((FontFamily)testFontFamilyValue).Source == FontFamily.Source), "Verify FontFamily value in the selected text...", true);
            Verifier.Verify(((double)testFontSizeValue == FontSize), "Verifying FontSize value in the selected text...", true);

            testFontFamilyValue = tpPart2.Parent.GetValue(RichTextBox.FontFamilyProperty);
            testFontSizeValue = tpPart2.Parent.GetValue(RichTextBox.FontSizeProperty);

            Verifier.Verify((((FontFamily)testFontFamilyValue).Source == _systemFontFamily.Source), "Verify FontFamily value in the non selected text...", true);
            Verifier.Verify(((double)testFontSizeValue == _systemFontSize), "Verifying FontSize value in the non selected text...", true);

            Logger.Current.ReportSuccess();
        }
    }   
}