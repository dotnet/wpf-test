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
    /// Verifies that the text for a given line can be retrieved,
    /// given its index through TextPointer and through API calls.
    /// </summary>
    [Test(0, "TextBox", "TextBoxGetTextFromLine", MethodParameters = "/TestCaseType:TextBoxGetTextFromLine", SupportFiles = @"FeatureTests\Editing\TextBox.xaml", Keywords = "TextFormattingModeTests")]
    public class TextBoxGetTextFromLine : TextBoxTestCase
    {
        #region Private data.

        struct TestCaseData
        {
            #region Internal properties.

            /// <summary>Text in lines.</summary>
            internal readonly string[] Lines;
            /// <summary>Text in control.</summary>
            internal readonly string Text;
            /// <summary>Whether text should wrap.</summary>
            internal readonly bool Wrap;
            /// <summary>Whether text should wrap.</summary>
            internal readonly bool DisplayMode;
            /// <summary>Whether text should wrap.</summary>
            internal static TestCaseData[] Cases = new TestCaseData[] {
                new TestCaseData(false, "ab\r\ncd", new string[] { "ab\r\n", "cd" },true ),
                new TestCaseData(false, "ab\r\ncd", new string[] { "ab\r\n", "cd" },false ),
                new TestCaseData(true, "ab\r\ncd", new string[] { "ab\r\n", "cd" },true ),
                new TestCaseData(true, "ab\r\ncd", new string[] { "ab\r\n", "cd" },false ),
                new TestCaseData(true, "AAAAAAAAAAAAAAAABB", new string[] { "AAAAAAAAAAAAAAAAB", "B" },true ),
                new TestCaseData(true, "AAAAAAAAAAAAAAAABB", new string[] { "AAAAAAAAAAAAAAAA", "BB" } ,false),
                new TestCaseData(false, "AAAAAAAAAAAAAAAABB", new string[] { "AAAAAAAAAAAAAAAABB" } ,true),
                new TestCaseData(false, "AAAAAAAAAAAAAAAABB", new string[] { "AAAAAAAAAAAAAAAABB" } ,false),
                new TestCaseData(true, "AAAAAAAAAAAAAAAABBBBBBBBBBBBBBBBCCCCCCCCCCCCCCCCDDDDDDDDDDDDDDDD", 
                    new string[] { "AAAAAAAAAAAAAAAAB", "BBBBBBBBBBBBBBBCC", "CCCCCCCCCCCCCCDDD", "DDDDDDDDDDDDD", },true ),
               new TestCaseData(true, "AAAAAAAAAAAAAAAABBBBBBBBBBBBBBBBCCCCCCCCCCCCCCCCDDDDDDDDDDDDDDDD", 
                    new string[] { "AAAAAAAAAAAAAAAA", "BBBBBBBBBBBBBBBB", "CCCCCCCCCCCCCCCC", "DDDDDDDDDDDDDDDD" },false )
            };

            #endregion Internal properties.

            #region Private methods.

            private TestCaseData(bool wrap, string text, string[] lines, bool displayMode)
            {
                System.Diagnostics.Debug.Assert(text != null);
                System.Diagnostics.Debug.Assert(lines != null);

                this.Wrap = wrap;
                this.Text = text;
                this.Lines = lines;
                this.DisplayMode = displayMode;
            }

            #endregion Private methods.
        }

        private const string FixedFontFamily = "Courier New";
        private const int FixedFontSize = 14;
        private const int FixedWidth = 150;

        private int _dataInstanceIndex;

        #endregion Private data.

        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _dataInstanceIndex = 0;
            SetTestCaseData();
        }

        private void SetTestCaseData()
        {
            string text;
            bool wrap;
            bool displayMode;

            text = TestCaseData.Cases[_dataInstanceIndex].Text;
            wrap = TestCaseData.Cases[_dataInstanceIndex].Wrap;
            displayMode = TestCaseData.Cases[_dataInstanceIndex].DisplayMode;

            Log("Setting TextBox.Text: [" + text + "]");
            Log("Setting TextBox.Wrap: [" + wrap + "]");


            TestTextBox.Text = text;
            TestTextBox.TextWrapping = wrap ? TextWrapping.Wrap : TextWrapping.NoWrap; ;

            TestTextBox.FontFamily = new System.Windows.Media.FontFamily(FixedFontFamily);
            TestTextBox.FontSize = FixedFontSize;
            TestTextBox.Width = FixedWidth;
            //The value of padding different between Win7 and Win8.
            //Default value is 1,1,1,1 on Win7 OS. But on Win8 is 0,0,0,0. So the textbox width on Win7 is 150, but the value is 148 on Win8.
            OperatingSystem os = Environment.OSVersion;
            Version ver = os.Version;
            if (ver.Major > 6 || ((6 == ver.Major) && ver.Minor > 1))
            {
                TestTextBox.Width = 148;
            }
            if (displayMode)
            {
                Log("Setting TextOptions.TextFormattingMode: Display");
                TextOptions.SetTextFormattingMode(TestTextBox, TextFormattingMode.Display);
            }
            else
            {
                Log("Setting TextOptions.TextFormattingMode: Ideal");
                TextOptions.SetTextFormattingMode(TestTextBox, TextFormattingMode.Ideal);
            }            
            QueueHelper.Current.QueueDelegate(CheckTestCaseData);
        }

        private void CheckTestCaseData()
        {
            TestCaseData data;      // Dataset being tested.
            TextRange[] lineRanges; // Text ranges for laid out lines.
            TextRange lineRange;    // Text range of each line. lineResults.Length == data.Lines.Length

            Log("Verifying contents of Line through TextPointer...");
            data = TestCaseData.Cases[_dataInstanceIndex];
            lineRanges = GetLineRanges();
            Verifier.Verify(lineRanges.Length == data.Lines.Length,
                "Lines expected (" + data.Lines.Length + ") Match lines found(" + lineRanges.Length + ").", true);
            for (int i = 0; i < data.Lines.Length; i++)
            {
                lineRange = lineRanges[i];
                Log("Line " + i + " has text [" + lineRange.Text + "], " + "Expected [" + data.Lines[i] + "]");
                Verifier.Verify(lineRange.Text == data.Lines[i]);
            }

            Log("Verifying contents of Line through TextBox line API calls...");
            Verifier.Verify(TestTextBox.LineCount == data.Lines.Length, "Verifying line count through LineCount property", true);
            for (int i = 0; i < TestTextBox.LineCount; i++)
            {
                Verifier.Verify(TestTextBox.GetLineText(i) == data.Lines[i],
                    "Verifying contents of line #" + i.ToString() + " through GetLineText() method", true);
            }

            // Advance to the next data instance, if available.
            _dataInstanceIndex++;
            System.Diagnostics.Debug.Assert(_dataInstanceIndex <= TestCaseData.Cases.Length);
            if (_dataInstanceIndex == TestCaseData.Cases.Length)
            {
                Logger.Current.ReportSuccess();
            }
            else
            {
                SetTestCaseData();
            }
        }

        private TextRange[] GetLineRanges()
        {
            ArrayList results;          // Results accumulated
            LogicalDirection dir;       // Direction for positions
            TextPointer loopPosition;   // Position to start looping from.
            TextPointer linePosition;   // TextPointer moved around.            

            dir = LogicalDirection.Forward;
            results = new ArrayList();
            loopPosition = TestWrapper.Start;
            do
            {
                TextPointer startPointer, endPointer;
                linePosition = loopPosition.GetPositionAtOffset(0, dir);
                startPointer = linePosition.GetLineStartPosition(0);
                endPointer = linePosition.GetLineStartPosition(1);

                if (endPointer != null)
                {
                    results.Add(new TextRange(startPointer, endPointer));
                    loopPosition = endPointer.GetPositionAtOffset(0);
                }
                else
                {
                    results.Add(new TextRange(startPointer, TestWrapper.End));
                    break;
                }
            } while (loopPosition.GetOffsetToPosition(TestWrapper.End) > 0);

            return (TextRange[])results.ToArray(typeof(TextRange));
        }

        #endregion Main flow.
    }       
}