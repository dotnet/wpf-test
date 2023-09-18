// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides a data-driven test case for the Find feature.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/Find/FindDataCase.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;

    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// Provides a data-driven test case for the Find feature.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("303"), TestBugs("491"),
     WindowlessTest(true)]
    public class FindDataCase: CustomTestCase
    {
        #region Test case data.

        /// <summary>Data drivnig test cases.</summary>
        internal class TestData
        {
            #region Private fields.

            private CultureInfo _culture;
            private Type _exceptionType;
            private string _expectedText;
            private int _expectedPosition;
            private bool _findResult;
            private bool _fromEnd;
            private FindOptions _options;
            private string _pattern;
            private string _text;
            private string _reproId;

            #endregion Private fields.

            #region Internal properties.

            /// <summary>CultureInfo to use in comparisons.</summary>
            internal CultureInfo Culture { get { return this._culture; } }

            /// <summary>
            /// Type of exception expected; null if no exception is expected.
            /// </summary>
            internal Type ExceptionType { get { return this._exceptionType; } }

            /// <summary>Text expected to be found; null if it does not matter.</summary>
            internal string ExpectedText { get { return this._expectedText; } }

            /// <summary>Position expected to be found; -1 if it does not matter.</summary>
            internal int ExpectedPosition { get { return this._expectedPosition; } }

            /// <summary>Whether the text should be found.</summary>
            internal bool FindResult { get { return this._findResult; } }

            /// <summary>Whether the search start from the end.</summary>
            internal bool FromEnd { get { return this._fromEnd; } }

            /// <summary>Search options.</summary>
            internal FindOptions Options { get { return this._options; } }

            /// <summary>Pattern being sought.</summary>
            internal string Pattern { get { return this._pattern; } }

            /// <summary>
            /// Text being searched; considered XAML if a '&lt;' character
            /// is found.
            /// </summary>
            internal string Text { get { return this._text; } }

            /// <summary>Bug number that the case reproes; may be empty.</summary>
            internal string ReproId { get { return (this._reproId != null)? _reproId : ""; } }

            #endregion Internal properties.

            internal TestData(CultureInfo culture,
                int expectedPosition, bool findResult,
                bool fromEnd, FindOptions options, string pattern, string text,
                string expectedText)
                :this(culture, expectedPosition, findResult, fromEnd, options,
                 pattern, text, expectedText, null, "")
            {
            }

            internal TestData(CultureInfo culture,
                int expectedPosition, bool findResult,
                bool fromEnd, FindOptions options, string pattern, string text,
                string expectedText, Type exceptionType, string reproId)
            {
                this._culture = culture;
                this._expectedText = expectedText;
                this._expectedPosition = expectedPosition;
                this._exceptionType = exceptionType;
                this._findResult = findResult;
                this._fromEnd = fromEnd;
                this._options = options;
                this._pattern = pattern;
                this._text = text;
                this._reproId = reproId;
            }

            private static CultureInfo s_invariantCulture = CultureInfo.InvariantCulture;
            private static CultureInfo s_englishCulture = new CultureInfo(0x0009);
            private static CultureInfo s_turkishCulture = new CultureInfo("tr");
            private static CultureInfo s_currentCulture = CultureInfo.CurrentCulture;
            private static FindOptions s_noOptions = FindOptions.None;

            internal static TestData[] TestCases = new TestData[] {
                // Verifies that the brute force search can be
                // requested (TestTactics: 304)
                new TestData(s_englishCulture, 0, true, false, s_noOptions,
                    "a", "argh", "a"),
                new TestData(s_englishCulture, 2048, true, false, s_noOptions,
                    "z", new string('-', 2048) + "z", "z"),
                // Verifies that the exact pattern match search can
                // be requested (TestTactics: 305)
                new TestData(s_invariantCulture, 3, true, false, s_noOptions,
                    "long to avoid BF", "is long to avoid BF?", null),
                // Verifies that the default culture can be requested
                // (TestTactics: 306)
                new TestData(null, 3, true, false, s_noOptions,
                    "long to avoid BF", "is long to avoid BF?", null),
                // Verifies that a specific culture can be requested
                // (TestTactics: 307)
                new TestData(new CultureInfo(0x2C0A), 0, true, false, s_noOptions,
                    "-", "--", "-"),
                // Verifies that a null pattern is rejected
                // (TestTactics: 308; TestBugs:429)
                new TestData(new CultureInfo(0x2C0A), 0, true, false, s_noOptions,
                    "-", "--", "-"),
                // Verifies that a zero-length pattern fails to match content.
                // (TestTactics: 309)
                new TestData(s_invariantCulture, 0, false, false, s_noOptions,
                    "", "", null),
                // Verifies that start is a valid position for matching.
                // (TestTactics: 310)
                new TestData(s_invariantCulture, 0, true, false, s_noOptions,
                    "sa", "sample string", "sa"),
                // Verifies that end is a valid position for matching.
                // (TestTactics: 311)
                new TestData(s_invariantCulture, -1, true, false, s_noOptions,
                    "ng", "sample string", "ng"),
                // Verifies that searches can be done from the end
                // (TestTactics: 312)
                new TestData(s_invariantCulture, -1, true, true, FindOptions.Backward,
                    "ng", "sample string", "ng", null, "406"),
                // Verifies that searches can be done from the end and match start
                // (TestTactics: 313)
                new TestData(s_invariantCulture, -1, true, true, FindOptions.Backward,
                    "sa", "sample string", "sa", null, "406"),
                // Verifies that the engine handles the case where the text
                // is too small for the pattern. (TestTactics: 314)
                new TestData(s_invariantCulture, -1, false, false, s_noOptions,
                    "sam", "s", null),
                // Verifies that the engine handles the case where the text
                // is too small for the pattern (not brute force)
                // (TestTactics: 315)
                new TestData(s_invariantCulture, -1, false, false, s_noOptions,
                    "larger pattern", "larger patter", null),
                // Verifies that the engine mismatches.
                // (TestTactics: 326)
                new TestData(s_invariantCulture, -1, false, false, s_noOptions,
                    "-", "larger text", null),
                // Verifies that the engine handles the case where there is no text.
                // (TestTactics: 327)
                new TestData(s_invariantCulture, -1, false, false, s_noOptions,
                    "--", "", null),
                // Verifies that the engine handles the case where accents are involved.
                // (TestTactics: 328)
                new TestData(s_invariantCulture, -1, true, false, s_noOptions,
                    "t", "aqu est el match", "t"),

                // Verifies that a specific culture can be requested.
                // (TestTactics: 329)
                new TestData(new CultureInfo(0x2C0A), 4, true, false, s_noOptions,
                    ".12", "0123.12", null),
                // Verifies that start is a valid position for matching (not brute force).
                // (TestTactics: 330)
                new TestData(s_invariantCulture, 0, true, false, s_noOptions,
                    "sample s", "sample string", "sample s"),
                // Verifies that end is a valid position for matching (not brute force).
                // (TestTactics: 331)
                new TestData(s_invariantCulture, -1, true, false, s_noOptions,
                    "tring", "sample string", "tring"),
                // Verifies that searches can be done from the end (not brute force).
                // (TestTactics: 332)
                new TestData(s_invariantCulture, -1, true, true, FindOptions.Backward,
                    "tring", "sample string", "tring", null, "406"),
                // Verifies that searches can be done from the end all the
                // way to the start (not brute force).
                // (TestTactics: 333)
                new TestData(s_invariantCulture, -1, true, true, FindOptions.Backward,
                    "tring", "sample string", "tring", null, "406"),
                // Verifies that the engine handles mismatches (not brute force).
                // (TestTactics: 334)
                new TestData(s_invariantCulture, -1, false, false, s_noOptions,
                    "text", "sample string", null),
                // Verifies that the engine handles the case where there is no
                // text (not brute force).
                // (TestTactics: 335)
                new TestData(s_invariantCulture, -1, false, false, s_noOptions,
                    "text", "", null),
                // Verifies that the engine handles the case where accents
                // are involved (not brute force).
                // (TestTactics: 336)
                new TestData(s_invariantCulture, -1, true, false, s_noOptions,
                    "est el ", "aqu est el match", null),
                // Verifies that Arabic text can be found.
                // (TestTactics: 337)
                new TestData(s_invariantCulture, 5, true, false, s_noOptions,
                    "a\x0688\x0689\x0690", "a\x0685\x0689\x0690\x0690a\x0688\x0689\x0690", null),

                // Verifies that text within a XAML element can be found.
                new TestData(s_invariantCulture, 1, true, false, s_noOptions,
                    "text", "<Bold>text</Bold>b", null),
                // Verifies that text within a XAML element can be found.
                new TestData(s_invariantCulture, 3, true, false, s_noOptions,
                    "old text", "a<Bold>bold text</Bold>b", null),
                // Verifies that text within a XAML atomic element can be found.
                new TestData(s_invariantCulture, 1, true, false, s_noOptions,
                    "bold text", "a<Bold>bold text</Bold>b", null),
                // Verifies that there are no partial ma-tches in atomic elements.
                new TestData(s_invariantCulture, -1, false, false, s_noOptions,
                    "text", "a<Bold>bold text</Bold>b", null),

                // Verifies that the 'Turkish I' case works as expected.
                // (TestTactics: 338)
                new TestData(s_invariantCulture, -1, false, false, s_noOptions,
                    "PARTIAL", "partial", null),
                new TestData(s_englishCulture, -1, true, false, FindOptions.IgnoreCase,
                    "PARTIAL", "partial", null),
                new TestData(s_turkishCulture, -1, false, false, s_noOptions,
                    "PARTIAL", "partial", null),
                new TestData(s_turkishCulture, -1, false, false, FindOptions.IgnoreCase,
                    "PARTIAL", "partial", null),

                // Verifies that the engine handles the case where a whole 
                // word needs to be matched (TestTactics: 339, 339)
                new TestData(s_englishCulture, 0, true, false, FindOptions.MatchWholeWords,
                    "ab", "ab cd", null),
                new TestData(s_englishCulture, 3, true, false, FindOptions.MatchWholeWords,
                    "cd", "ab cd", null),
                new TestData(s_englishCulture, 6, true, false, FindOptions.MatchWholeWords,
                    "ef", "ab cd ef", null),
                new TestData(s_englishCulture, -1, false, false, FindOptions.MatchWholeWords,
                    "no match", "blargh", null),
                new TestData(s_englishCulture, 0, true, false, FindOptions.MatchWholeWords,
                    "all", "all", null),
                new TestData(s_englishCulture, 1, true, false, FindOptions.MatchWholeWords,
                    "sep", ".sep.", null),
//                new TestData(EnglishCulture, 2, true, false, FindOptions.MatchWholeWords,
//                    "o twot two", "two", null),
            };
        }

        /// <summary>Current test data being used.</summary>
        private TestData _testData;

        #endregion Test case data.

        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            string reproId;
            string testDescription;

            reproId = Settings.GetArgument("ReproId", false);
            if (reproId != null && reproId.Length > 0)
            {
                Log("Running cases with repro-id: " + reproId);
            }

            for (int i = 0; i < TestData.TestCases.Length; i++)
            {
                _testData = TestData.TestCases[i];
                if (_testData.ReproId == reproId)
                {
                    testDescription = String.Join("\r\n",
                        ReflectionUtils.DescribeProperties(_testData));
                    Log("Running test case: " + i);
                    Log("Data:\r\n" + testDescription);
                    RunCase();
                    Log("");
                }
                else
                {
                    Log("Skipping test case: " + i);
                }
            }

            // Special cases that cannot be handled by
            // the data-driven ones.
            if (reproId == "905814")
            {
                CheckNullCultureRejected();
            }

            Logger.Current.ReportSuccess();
        }

        private void RunCase()
        {
            FindEngine engine;      // Engine to execute find with.
            bool exceptionThrown;   // Whether an exception is thrown.
            bool cultureIsDefined;  // Whether a CultureInfo is specified.

            exceptionThrown = false;
            cultureIsDefined = _testData.Culture != null;

            // Run the search.
            try
            {
                if (cultureIsDefined)
                    engine = new FindEngine(_testData.Pattern, _testData.Options, _testData.Culture);
                else
                    engine = new FindEngine(_testData.Pattern, _testData.Options);
                CheckSearch(engine);
            }
            catch(Exception e)
            {
                exceptionThrown = true;
                if (_testData.ExceptionType == null) throw;
                if (e.GetType() == _testData.ExceptionType)
                {
                    throw new Exception("Expected exception of type " +
                        _testData.ExceptionType + "\r\n. Got exception: " +
                        e.ToString());
                }
            }

            // Check for exceptions that were expected but not thrown.
            if (!exceptionThrown && _testData.ExceptionType != null)
            {
                throw new Exception("Exception of type " +
                    _testData.ExceptionType + " expected but none thrown.");
            }
        }

        private void CheckNullCultureRejected()
        {
            // TestTactics: 341.
            Log("Checking that null cultures are rejected...");
            try
            {
                new FindEngine("pattern", FindOptions.None, null);
                throw new ApplicationException("Null culture accepted.");
            }
            catch(ArgumentNullException)
            {
                Log("ArgumentNullException thrown as expected.");
            }
        }

        #endregion Main flow.

        #region Verifications.

        /// <summary>
        /// Counts the number of positions between the specified position
        /// and the start of the container.
        /// </summary>
        /// <param name="tp">Position to count from.</param>
        /// <returns>
        /// The number of positions between the specified position and the
        /// start of the container.
        /// </returns>
        private int GetPositionsFromStart(TextPointer tp)
        {
            TextPointer navigator;
            TextPointer start;
            int result;

            System.Diagnostics.Debug.Assert(tp != null);

            start = tp.TextContainer.Start;
            if (tp == start) return 0;

            navigator = tp.CreateNavigator();
            result = 0;
            while (start != navigator)
            {
                navigator.MoveByDistance(-1);
                result++;
                if (result > 1024 * 1024)
                {
                    throw new ApplicationException("More than 1MB units moved " +
                        "looking for start/position delta - aborting test...");
                }
            }
            return result;
        }

        private UIElementWrapper SetupTestControl()
        {
            string editableType;
            FrameworkElement control;
            UIElementWrapper controlWrapper;
            bool isPlainText;

            System.Diagnostics.Debug.Assert(_testData != null);

            Log("Setting up text in control to search...");
            isPlainText = _testData.Text.IndexOf('<') == -1;
            editableType = isPlainText ? "TextBox" : "RichTextBox";
            control = UIElementWrapper.CreateEditableType(editableType);
            controlWrapper = new UIElementWrapper(control);
			control.ApplyTemplate();

			// Set the text appropriately.
            if (isPlainText)
            {
                controlWrapper.Text = _testData.Text;
            }
            else
            {
                controlWrapper.XamlText = _testData.Text;
            }

            return controlWrapper;
        }

        private void GetTextPointers(TextContainer container,
            out TextPointer navigator, out TextPointer limit)
        {
            System.Diagnostics.Debug.Assert(container != null);
            if (_testData.FromEnd)
            {
                navigator = container.End.CreateNavigator();
                limit = container.Start;
            }
            else
            {
                navigator = container.Start.CreateNavigator();
                limit = container.End;
            }
        }

        private void CheckSearch(FindEngine engine)
        {
            UIElementWrapper controlWrapper;
            bool findResult;
            TextPointer limit;
            TextPointer navigator;
            int positionsFromStart;

            controlWrapper = SetupTestControl();
            GetTextPointers(controlWrapper.TextContainer, out navigator, out limit);

            Log("Searching for pattern...");
            System.Windows.Documents.FindResult result = engine.Find(navigator, limit);
            findResult = result.Matched;

            Log("Find result: expected=" + _testData.FindResult + ", actual=" + findResult);
            Verifier.Verify(_testData.FindResult == findResult);

            if (findResult)
            {
                controlWrapper.SelectionInstance.MoveToPositions(result.Start, result.End);
                if (_testData.ExpectedText != null)
                {
                    Log("Text found: expected=[" + _testData.ExpectedText +
                        "], actual=[" + controlWrapper.SelectionInstance.Text + "]");
                    Verifier.Verify(_testData.ExpectedText == controlWrapper.SelectionInstance.Text);
                }
                if (_testData.ExpectedPosition != -1)
                {
                    positionsFromStart = GetPositionsFromStart(result.Start);
                    Log("Find position: expected=" + _testData.ExpectedPosition +
                        ", actual=" + positionsFromStart);
                    Verifier.Verify(_testData.ExpectedPosition == positionsFromStart);
                }
            }
        }

        #endregion Verifications.
    }
}
