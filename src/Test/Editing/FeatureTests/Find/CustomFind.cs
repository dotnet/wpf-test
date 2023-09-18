// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides a custom Find implementation sample.

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

    #endregion Namespaces.

    /// <summary>
    /// Provides a custom Find implementation sample.
    /// </summary>
    [TestOwner("Microsoft"), TestSample("FindEngine")]
    public class FindCustomCase: CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            // Hard-coded strings for the sample.
            // Note that Unicode character 0301 is a combining acute accent.
            // CompareOptions will be used to ignore this mark.
            const string SampleText = "Text i\u0301n TextBox.";
            const string FindText = "T in t";

            // Create a Textbox and set the sample text.
            TextBox box = new TextBox();
            box.Text = SampleText;

            // Look for the FindText constant in the TextBox.
            if (FindTextAccentInsensitive(box, FindText))
            {
                MainWindow.Text = "Text was found.";
            }
            else
            {
                MainWindow.Text = "Text was not found.";
                throw new ApplicationException("Sample text should have been found.");
            }

            MainWindow.Content = box;

            Logger.Current.ReportSuccess();
        }

        /// <summary>Finds and highlights text in the specified TextBox.</summary>
        /// <param name='box'>TextBox to search in.</param>
        /// <param name='textToFind'>Text being sought.</param>
        /// <returns>true if textToFind was found and highlighted; false otherwise.</returns>
        public bool FindTextAccentInsensitive(TextBox box, string textToFind)
        {
            // Check parameters.
            if (box == null)
            {
                throw new ArgumentNullException("box");
            }
            if (textToFind == null)
            {
                throw new ArgumentNullException("textToFind");
            }

            // Set up comparison objects.
            CompareOptions options = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace;
            CompareInfo compareInfo = CultureInfo.CurrentCulture.CompareInfo;

            // Move through the TextContainer in the TextBox searching for the text.
            bool resultFound = false;               // Whether the result has been found.
            string plainText = String.Empty;        // Current text being sought.
            TextPointer plainTextPointer = null;  // Start position for plainText.
            TextPointer navigator = box.StartPosition.CreateNavigator();
            do
            {
                //
                // If we have a character run, look into it. Note that we are not
                // taking into account matches that span elements. This is
                // fine for plain text content, but means that looking for
                // "abc" would not match <Bold>a</Bold>bc in a RichTextBox.
                //
                if (navigator.GetSymbolType(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    //
                    // Character runs can be neighbouring. A more efficient 
                    // implementation would minimize the size of plainText
                    // and adjust plainTextPointer accordingly. This implementation
                    // only resets on element boundaries such as paragraphs,
                    // formatting changes, or embedded objects.
                    //
                    if (plainTextPointer == null)
                    {
                        System.Diagnostics.Debug.Assert(plainText == String.Empty);
                        plainTextPointer = navigator.CreatePosition();
                    }
                    plainText += navigator.GetTextInRun(LogicalDirection.Forward);

                    // Search in the text.
                    int startMatch, endMatch;
                    resultFound = FindStringIndex(textToFind, plainText, options, 
                        compareInfo, out startMatch, out endMatch);
                    
                    //
                    // If a match was found, select it. Note that the offsets
                    // are from plainTextPointer.
                    if (resultFound)
                    {
                        TextPointer resultNavigator;
                        TextPointer startPosition;
                        TextPointer endPosition;
                        
                        resultNavigator = plainTextPointer.CreateNavigator();
                        resultNavigator.MoveToCharacter(startMatch);
                        startPosition = resultNavigator;
                        
                        endPosition = resultNavigator.CreateNavigator();
                        endPosition.MoveToCharacter(endMatch - startMatch);
                        
                        box.Selection.MoveToPositions(startPosition, endPosition);
                    }
                }
                else
                {
                    // Reset running text.
                    plainText = String.Empty;
                    plainTextPointer = null;
                }
            } while(!resultFound && navigator.Move(LogicalDirection.Forward));
            
            return resultFound;
        }

        /// <summary>Finds the index of a string in another.</summary>
        /// <param name='subTextToFind'>Text being looked for.</param>
        /// <param name='textToSearch'>Text being looked in.</param>
        /// <param name='compareOptions'>Options for string comparison.</param>
        /// <param name='compareInfo'>CompareInfo object for comparison.</param>
        /// <param name='startMatch'>The index of the start of the match, if found; -1 otherwise.</param>
        /// <param name='endMatch'>The index of the end of the match, if found; -1 otherwise.</param>
        /// <returns>true if subTextToFind was found in textToSearch; false otherwise.</returns>
        private bool FindStringIndex(string subTextToFind, string textToSearch,
            CompareOptions compareOptions, CompareInfo compareInfo, out int startMatch,
            out int endMatch)
        {
            // Assert for valid parameter values.
            System.Diagnostics.Debug.Assert(subTextToFind != null);
            System.Diagnostics.Debug.Assert(textToSearch != null);
            System.Diagnostics.Debug.Assert(compareInfo != null);
            
            // Search for the string.
            startMatch = compareInfo.IndexOf(textToSearch, subTextToFind, compareOptions);
            
            // If no match is found, set output start variable and return false.
            if (startMatch == -1)
            {
                endMatch = -1;
                return false;
            }
            
            // Calculate how many characters need to be selected to match
            // the comparison. Start from the end, to avoid selecting
            // only part of a combining character.
            int subTextLength = subTextToFind.Length;
            endMatch = textToSearch.Length - startMatch;
            while (compareInfo.Compare(subTextToFind, 0, subTextLength, 
                    textToSearch, startMatch, endMatch - startMatch, compareOptions) != 0)
            {
                endMatch--;
            }
            return true;
        }

        #endregion Main flow.
    }
}
