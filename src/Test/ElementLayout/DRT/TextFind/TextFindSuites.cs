// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// Description: Collection of Text Find related tests.
//
//

using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.IO;
using System.Resources;
using System.Windows.Markup;

using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Documents;

namespace DRT
{
    [Flags]
    internal enum FindFlags
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0x00000000,
        /// <summary>
        /// Match case.
        /// </summary>
        MatchCase = 0x00000001,
        /// <summary>
        /// Searches for last occurance.
        /// </summary>
        FindInReverse = 0x00000002,
        /// <summary>
        /// Matches the entire word.
        /// </summary>
        FindWholeWordsOnly = 0x00000004,
        /// <summary>
        /// Matches Bidi diacritics.
        /// </summary>
        MatchDiacritics = 0x00000008,
        /// <summary>
        /// Matches Arabic kashida.
        /// </summary>
        MatchKashida = 0x00000010,
        /// <summary>
        /// Matches Arabic AlefHamza.
        /// </summary>
        MatchAlefHamza = 0x00000020,
    }

    internal static class TextFindProxy
    {
        static TextFindProxy()
        {
            Assembly assembly = (typeof(FlowDocument)).Assembly;
            Type[] types = assembly.GetTypes();
            Type findEngineType = null;

            for (int i = 0; i < types.Length && findEngineType == null; ++i)
            {
                if (types[i].Name == "TextFindEngine")
                {
                    s_findMethodInfo = types[i].GetMethod(
                        "Find",
                        BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod);
                        break;
                }
            }
        }
    
        public static TextRange Find(TextRange findContainer, string findPattern, FindFlags flags, CultureInfo cultureInfo)
        {
            TextRange findResult = (TextRange)s_findMethodInfo.Invoke(
                null,
                new object[] {
                    findContainer.Start, 
                    findContainer.End, 
                    findPattern,
                    flags,
                    CultureInfo.CurrentCulture }
                    );
                    
            return (findResult);
        }

        private static readonly System.Reflection.MethodInfo s_findMethodInfo = null;
    }
    
    internal class Selection
    {
        internal Selection(TextRange selectionRange)
        {
            if (selectionRange != null)
            {
                _range = new TextRange(selectionRange.Start, selectionRange.End);
                _background = (Brush)selectionRange.Start.Parent.GetValue(TextElement.BackgroundProperty);
                _foreground = (Brush)selectionRange.Start.Parent.GetValue(TextElement.ForegroundProperty);
                _fontSize = (double)selectionRange.Start.Parent.GetValue(TextElement.FontSizeProperty);

                selectionRange.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Gold);
                selectionRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
                selectionRange.ApplyPropertyValue(TextElement.FontSizeProperty, 16.0 / 72.0 * 96.0);
            }
        }
        
        internal void Dispose()
        {
            if (_range != null)
            {
                _range.ApplyPropertyValue(TextElement.BackgroundProperty, _background);
                _range.ApplyPropertyValue(TextElement.ForegroundProperty, _foreground);
                _range.ApplyPropertyValue(TextElement.FontSizeProperty, _fontSize);

                _range = null;
            }
        }

        private TextRange _range;
        private Brush _background;
        private Brush _foreground;
        private double _fontSize;
    }
    
    internal abstract class TextFindSuite : DrtTextFindSuite
    {
        internal TextFindSuite(string xamlFileName, string findString, FindFlags findFlags, int findsCount, string suiteName, string ownerAlias)
            : base(suiteName, ownerAlias)
        {
            Debug.Assert(xamlFileName != null);
            Debug.Assert(findsCount > 0);

            _xamlFileName = xamlFileName;
            _findString = findString;
            _findFlags = findFlags;
            _findsCount = findsCount;
        }

        /// <summary>
        /// Creates array of callbacks
        /// </summary>
        /// <returns>Array of callbacks</returns>
        protected override DrtTest[] CreateTests()
        {
            DrtTest[] tests = new DrtTest[_findsCount + 1];
            tests[0] = new DrtTest(LoadXaml);
            tests[1] = new DrtTest(FindStart);

            DrtTest findContinue = new DrtTest(FindContinue);

            for (int i = 2; i < tests.Length; ++i)
            {
                tests[i] = findContinue;
            }

            return (tests);
        }

        internal void LoadXaml()
        {
            LoadContentFromXaml(_xamlFileName);
        }
        
        internal void FindStart()
        {
            ResetFindContainer();
            FindContinue();
        }
        
        internal void FindContinue()
        {
            if (_highlight != null) _highlight.Dispose();
            TextRange findResult = TextFindProxy.Find(_findContainer, _findString, _findFlags, CultureInfo.InvariantCulture);

            if ((_findFlags & FindFlags.FindInReverse) != 0)
            {
                _findContainer = new TextRange(_findContainer.Start, findResult.Start);
            }
            else
            {
                _findContainer = new TextRange(findResult.End, _findContainer.End);
            }
            _highlight = new Selection(findResult);
        }
        
        private void ResetFindContainer()
        {
            _fdsv = (FlowDocumentScrollViewer)LogicalTreeHelper.FindLogicalNode(ContentRoot.Child, "FindContainer");
            _findContainer = new TextRange(_fdsv.Document.ContentStart, _fdsv.Document.ContentEnd);
        }

        private readonly string _xamlFileName;
        private readonly string _findString;
        private readonly FindFlags _findFlags;
        private readonly int _findsCount;
        private FlowDocumentScrollViewer _fdsv;
        private TextRange _findContainer;
        private Selection _highlight;
    }

    //
    //  Forward, Case Insensitive
    //
    internal sealed class TextFind0x00000000 : TextFindSuite
    {
        internal TextFind0x00000000()
            : base(c_xamlFileName, c_findString, c_findFlags, c_findsCount, c_suiteId, "Microsoft")
        {
            Debug.Assert(this.GetType().Name == c_suiteId);
        }

        private const string c_xamlFileName = "TextFindSampleContent01";
        private const string c_findString = "findTarget";
        private const FindFlags c_findFlags = FindFlags.None;
        private const int c_findsCount = 9;
        private static readonly string c_suiteId = "TextFind0x" + ((int)(c_findFlags)).ToString("X8");
    }

    //
    //  Forward, Case Sensitive
    //
    internal sealed class TextFind0x00000001 : TextFindSuite
    {
        internal TextFind0x00000001()
            : base(c_xamlFileName, c_findString, c_findFlags, c_findsCount, c_suiteId, "Microsoft")
        {
            Debug.Assert(this.GetType().Name == c_suiteId);
        }

        private const string c_xamlFileName = "TextFindSampleContent01";
        private const string c_findString = "findTarget";
        private const FindFlags c_findFlags = FindFlags.MatchCase;
        private const int c_findsCount = 5;
        private static readonly string c_suiteId = "TextFind0x" + ((int)(c_findFlags)).ToString("X8");
    }

    //
    //  Reverse, Case Insensitive
    //
    internal sealed class TextFind0x00000002 : TextFindSuite
    {
        internal TextFind0x00000002()
            : base(c_xamlFileName, c_findString, c_findFlags, c_findsCount, c_suiteId, "Microsoft")
        {
            Debug.Assert(this.GetType().Name == c_suiteId);
        }

        private const string c_xamlFileName = "TextFindSampleContent01";
        private const string c_findString = "findTarget";
        private const FindFlags c_findFlags = FindFlags.FindInReverse;
        private const int c_findsCount = 9;
        private static readonly string c_suiteId = "TextFind0x" + ((int)(c_findFlags)).ToString("X8");
    }

    //
    //  Reverse, Case Sensitive
    //
    internal sealed class TextFind0x00000003 : TextFindSuite
    {
        internal TextFind0x00000003()
            : base(c_xamlFileName, c_findString, c_findFlags, c_findsCount, c_suiteId, "Microsoft")
        {
            Debug.Assert(this.GetType().Name == c_suiteId);
        }

        private const string c_xamlFileName = "TextFindSampleContent01";
        private const string c_findString = "findTarget";
        private const FindFlags c_findFlags = FindFlags.MatchCase | FindFlags.FindInReverse;
        private const int c_findsCount = 5;
        private static readonly string c_suiteId = "TextFind0x" + ((int)(c_findFlags)).ToString("X8");
    }

    //
    //  Forward, Find whole word only
    //
    internal sealed class TextFind0x00000004 : TextFindSuite
    {
        internal TextFind0x00000004()
            : base(c_xamlFileName, c_findString, c_findFlags, c_findsCount, c_suiteId, "Microsoft")
        {
            Debug.Assert(this.GetType().Name == c_suiteId);
        }

        private const string c_xamlFileName = "TextFindSampleContent02";
        private const string c_findString = "find";
        private const FindFlags c_findFlags = FindFlags.FindWholeWordsOnly;
        private const int c_findsCount = 2;
        private static readonly string c_suiteId = "TextFind0x" + ((int)(c_findFlags)).ToString("X8");
    }

    internal class ProgrammaticFindSuite : DrtTestSuite
    {
        internal ProgrammaticFindSuite()
            : base("ProgrammaticFind")
        {
        }

                // Initialize tests.
        public override DrtTest[] PrepareTests()
        {
            return new DrtTest[] { new DrtTest(KashidaTests), 
                                   new DrtTest(AlefHamzaTests),
                                   //new DrtTest(DiacriticTests),
                                   //new DrtTest(ArabicSymbolTests),
                                   new DrtTest(WholeWordOnlyTests),
                                 };
        }

        // Test FindFlags.MatchKashida.
        private void KashidaTests()
        {
            const char KashidaChar = '\u0640';

            char[] kashidaChars = { '\u062c', KashidaChar, '\u062d', '\u062e', };
            string kashidaText = new string(kashidaChars);

            char[] nonKashidaChars = { '\u062c', '\u062d', '\u062e', };
            string nonKashidaText = new string(nonKashidaChars);

            CultureInfo cultureInfo = new CultureInfo("ar");

            FlowDocument document = new FlowDocument(new Paragraph(new Run(kashidaText)));
            TextRange range = new TextRange(document.ContentStart, document.ContentEnd);

            TextRange findRange;

            findRange = TextFindProxy.Find(range, kashidaText, FindFlags.MatchKashida, cultureInfo);
            DRT.Assert(findRange.Text == kashidaText);

            findRange = TextFindProxy.Find(range, kashidaText, 0 /* FindFlags */, cultureInfo);
            DRT.Assert(findRange.Text == kashidaText);

            findRange = TextFindProxy.Find(range, nonKashidaText, FindFlags.MatchKashida, cultureInfo);
            DRT.Assert(findRange == null);

            findRange = TextFindProxy.Find(range, nonKashidaText, 0 /* FindFlags */, cultureInfo);
            DRT.Assert(findRange.Text == kashidaText);
        }

        // Test FindFlags.MatchAlefHamza.
        private void AlefHamzaTests()
        {
            const char AlefMaddaAbove = '\u0622';
            const char AlefHamzaAbove = '\u0623';
            const char AlefHamzaBelow = '\u0625';
            const char Alef = '\u0627';

            char[] alefHamzaChars = { 'x', AlefMaddaAbove, AlefHamzaAbove, AlefHamzaBelow, Alef, };
            string alefHamzaText = new string(alefHamzaChars);

            char[] nonAlefHamzaChars = { 'x', Alef, Alef, Alef, Alef, };
            string nonAlefHamzaText = new string(nonAlefHamzaChars);

            CultureInfo cultureInfo = new CultureInfo("ar");

            FlowDocument document = new FlowDocument(new Paragraph(new Run(alefHamzaText)));
            TextRange range = new TextRange(document.ContentStart, document.ContentEnd);

            TextRange findRange;

            findRange = TextFindProxy.Find(range, alefHamzaText, FindFlags.MatchAlefHamza, cultureInfo);
            DRT.Assert(findRange.Text == alefHamzaText);

            findRange = TextFindProxy.Find(range, alefHamzaText, 0 /* FindFlags */, cultureInfo);
            DRT.Assert(findRange.Text == alefHamzaText);

            findRange = TextFindProxy.Find(range, nonAlefHamzaText, FindFlags.MatchAlefHamza, cultureInfo);
            DRT.Assert(findRange == null);

            findRange = TextFindProxy.Find(range, nonAlefHamzaText, 0 /* FindFlags */, cultureInfo);
            DRT.Assert(findRange.Text == alefHamzaText);
        }

        // Test FindFlags.MatchDiacritics.
        private void DiacriticTests()
        {
            char []diacriticChars = { '\u0627', '\u0644', '\u0652', '\u0643', '\u0650', '\u062a', '\u064e', '\u0627', '\u0628' };
            string diacriticText = new string(diacriticChars);

            char[] nonDiacriticChars = { '\u0627', '\u0644', '\u0643', '\u062a', '\u0627', '\u0628' };
            string nonDiacriticText = new string(nonDiacriticChars);

            CultureInfo cultureInfo = new CultureInfo("ar");

            FlowDocument document = new FlowDocument(new Paragraph(new Run(nonDiacriticText)));
            TextRange range = new TextRange(document.ContentStart, document.ContentEnd);

            TextRange findRange;

            findRange = TextFindProxy.Find(range, diacriticText, FindFlags.MatchDiacritics, cultureInfo);
            DRT.Assert(findRange == null);

            findRange = TextFindProxy.Find(range, diacriticText, 0 /* FindFlags */, cultureInfo);
            DRT.Assert(findRange.Text == nonDiacriticText);

            document = new FlowDocument(new Paragraph(new Run(diacriticText)));
            range = new TextRange(document.ContentStart, document.ContentEnd);

            findRange = TextFindProxy.Find(range, nonDiacriticText, FindFlags.MatchDiacritics, cultureInfo);
            DRT.Assert(findRange == null);

            findRange = TextFindProxy.Find(range, nonDiacriticText, 0 /* FindFlags */, cultureInfo);
            DRT.Assert(findRange.Text == diacriticText);
        }

        // Test find engine behavior around mixed bidi/symbolic characters.
        // Because of a dependency on the CLR's string compare methods, our
        // find engine cannot distinguish between arabic diacritic and "symbol"
        // character such as ',' or ' '.
        //
        // This test verifies that in the case where the search string contains
        // no characters in the unicode bidi range, the ignore diacritic code
        // path is skipped and symbols are considered.
        //
        // 
        private void ArabicSymbolTests()
        {
            char[] arabicChars = { '\u0627', '\u0644', '\u0652', '\u0643', '\u0650', '\u062a', '\u064e', '\u0627', '\u0628', ' ', '\u0627', '\u0644', '\u0652', '\u0643', '\u0650', '\u062a', '\u064e', '\u0627', '\u0628' };
            string arabicText = new string(arabicChars);

            char[] symbolChars = { '\u0627', '\u002e' };
            string symbolText = new string(symbolChars);

            CultureInfo cultureInfo = new CultureInfo("ar");

            FlowDocument document = new FlowDocument(new Paragraph(new Run(arabicText)));
            TextRange range = new TextRange(document.ContentStart, document.ContentEnd);

            TextRange findRange;

            findRange = TextFindProxy.Find(range, " ", 0, cultureInfo);
            DRT.Assert(findRange.Text == " ");

            // Test 0: search text has bidi chars, u002e ignored.
            findRange = TextFindProxy.Find(range, symbolText, 0, cultureInfo);
            DRT.Assert(findRange.Text.Length == 1 && findRange.Text[0] == symbolChars[0]);

            // Test 1: search text does not contain bidi chars, u002e respected.
            findRange = TextFindProxy.Find(range, symbolText, FindFlags.MatchDiacritics, cultureInfo);
            DRT.Assert(findRange == null);
        }

        private void WholeWordOnlyTests()
        {
            string findPattern = "justice";

            TextRange findRange;

            CultureInfo cultureInfo = new CultureInfo("en");

            // Find "Justice" word from the whole word option before the punctuation.
            string contentString = "Justice.";
            FlowDocument document = new FlowDocument(new Paragraph(new Run(contentString)));
            TextRange range = new TextRange(document.ContentStart, document.ContentEnd);
            findRange = TextFindProxy.Find(range, findPattern, FindFlags.FindWholeWordsOnly, cultureInfo);
            DRT.Assert(findRange != null && findRange.Text == "Justice");

            // Find "Justice" word from the whole word option after the punctuation.
            contentString = "?justice";
            document = new FlowDocument(new Paragraph(new Run(contentString)));
            range = new TextRange(document.ContentStart, document.ContentEnd);
            findRange = TextFindProxy.Find(range, findPattern, FindFlags.FindWholeWordsOnly, cultureInfo);
            DRT.Assert(findRange != null && findRange.Text == "justice");

            // Find "Justice" word from the whole word option before/after the punctuation.
            contentString = ",juStice.";
            document = new FlowDocument(new Paragraph(new Run(contentString)));
            range = new TextRange(document.ContentStart, document.ContentEnd);
            findRange = TextFindProxy.Find(range, findPattern, FindFlags.FindWholeWordsOnly, cultureInfo);
            DRT.Assert(findRange != null && findRange.Text == "juStice");

            // Must fail to find the whole word option after the digits.
            contentString = "justice4";
            document = new FlowDocument(new Paragraph(new Run(contentString)));
            range = new TextRange(document.ContentStart, document.ContentEnd);
            findRange = TextFindProxy.Find(range, findPattern, FindFlags.FindWholeWordsOnly, cultureInfo);
            DRT.Assert(findRange == null);
        }
    }
}
