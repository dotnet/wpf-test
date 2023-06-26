// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides a data-driven test case for the Find feature as exposed
//  through the TextBox API. 

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Xml;

    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;

    #endregion Namespaces.

    /// <summary>
    /// Provides a data-driven test case for the Find feature as
    /// exposed through the TextBox API.
    /// </summary>
    /// <remarks>
    /// This test case reads arguments by walking the XML block
    /// and running a test for each Find element found.
    /// </remarks>
    [TestOwner("Microsoft"), TestTactics("103"), TestBugs("406"),
     TestArgument("Description", "Optional description of intent."),
     TestArgument("ExpectedResult", "Expected result."),
     TestArgument("Options", "FindOptions to use in searching."),
     TestArgument("Pattern", "Pattern to look for."),
     TestArgument("SearchFrom", "Value for the SearchFrom argument."),
     TestArgument("SearchTo", "Value for the SearchTo argument."),
     TestArgument("Text", "Text in which to seek.")]
    public class TextBoxFindDataCase: TextBoxTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            XmlReader reader = Settings.GetXmlBlockReader();
            while (reader.Read())
            {
                if (reader.Name == "Find")
                {
                    RunFilterCase(reader);
                }
            }
            Logger.Current.ReportSuccess();
        }

        private void RunFilterCase(XmlReader reader)
        {
            Log("----------------");
            string description = reader["Description"];
            if (description != null) Log(description);

            int searchTo;
            string searchToString;
            bool useSearchTo;
            searchToString = reader["SearchTo"];
            useSearchTo = searchToString != null;
            searchTo = (useSearchTo)? Int32.Parse(searchToString,
                System.Globalization.CultureInfo.InvariantCulture): -1;

            int searchFrom;
            string searchFromString;
            bool useSearchFrom;
            searchFromString = reader["SearchFrom"];
            useSearchFrom = searchFromString != null;
            searchFrom = (useSearchFrom)? Int32.Parse(searchFromString,
                System.Globalization.CultureInfo.InvariantCulture): 0;

            FindOptions options;
            string optionsString;
            bool useOptions;
            optionsString = reader["Options"];
            useOptions = optionsString != null;
            options = (useOptions)? (FindOptions) Enum.Parse(
                    typeof(FindOptions), optionsString) : FindOptions.None;

            string text = reader["Text"];
            if (text == null)
            {
                throw new Exception("Invalid test case configuration " +
                    "- Text attribute is required.");
            }

            string pattern = reader["Pattern"];
            if (pattern == null)
            {
                throw new Exception("Invalid test case configuration " +
                    "- Pattern attribute is required.");
            }

            string expectedResultString = reader["ExpectedResult"];
            if (expectedResultString == null)
            {
                throw new Exception("Invalid test case configuration " +
                    "- ExpectedResult attribute is required.");
            }
            int expectedResult = Int32.Parse(expectedResultString,
                System.Globalization.CultureInfo.InvariantCulture);

            Log("Use options:     " + useOptions);
            Log("Use search from: " + useSearchFrom);
            Log("Use search to:   " + useSearchTo);
            if (useOptions)
                Log("Options:         " + options);
            if (useSearchFrom)
                Log("Search from:     " + searchFrom);
            if (useSearchTo)
                Log("Search to:       " + searchTo);
            Log("Text:           [" + text + "]");
            Log("Pattern:        [" + pattern + "]");
            Log("Expected result: " + expectedResult);

            TestTextBox.Text = text;
            int result;
            if (useSearchTo)
                result = TestTextBox.Find(pattern, options, searchFrom, searchTo );
            else if (useSearchFrom)
                result = TestTextBox.Find(pattern, options, searchFrom);
            else if (useOptions)
                result = TestTextBox.Find(pattern, options);
            else
                result = TestTextBox.Find(pattern);

            Log("Find performed.");
            Log("Result: " + result);
            Verifier.Verify(result == expectedResult,
                "Result is as expected", true);

            Log("Successful find operation.");
        }

        #endregion Main flow.
    }
}
