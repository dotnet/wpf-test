// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Verifies speller functinality when loading custom speller dictionaries in
//  containers that specify different languages

using System;
using System.Collections;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

using Microsoft.Test.Discovery;
using Test.Uis.Data;
using Test.Uis.Loggers;
using Test.Uis.TestTypes;
using Test.Uis.Wrappers;

namespace Microsoft.Test.Editing
{
    /// <summary>
    /// This test verifies basic speller functionality in RichTextBox and Textbox using a custom speller dictionary with respect to specifying certain locales.
    /// </summary>
    [Test(0, "Speller", "CustomSpellerDictionaryLocaleTest", MethodParameters = "/TestCaseType:CustomSpellerDictionaryLocaleTest", Keywords="Localization_Suite", Timeout = 120)]
    public class CustomSpellerDictionaryLocaleTest : ManagedCombinatorialTestCase
    {
        #region Main flow

        /// <summary>Starts the combination</summary>
        protected override void DoRunCombination()
        {
            if (CombinationNumber == 1)
            {
                CleanupStaleDictionaries();
            }

            if (!DetermineIfTestShouldRun())
            {
                Logger.Current.ReportResult(true, "This test is not valid for this particular OS version and locale.", true);
                NextCombination();
                return;
            }

            _customDictionaryUris = new Uri[] {
                new Uri("pack://application:,,,/EditingTestPart1;component/CustomDictionaryEnglish.lex"),
                new Uri("pack://application:,,,/EditingTestPart1;component/CustomDictionaryFrench.lex"),
                new Uri("pack://application:,,,/EditingTestPart1;component/CustomDictionarySpanish.lex"),
                new Uri("pack://application:,,,/EditingTestPart1;component/CustomDictionaryGerman.lex"),                         
                new Uri("pack://application:,,,/EditingTestPart1;component/CustomDictionaryJapanese.lex")
            };

            _textBoxBase = (TextBoxBase)_testControlType.CreateInstance();
            _textBoxBase.FontSize = 14;
            _textBoxBase.Height = 230;
            _textBoxBase.Width = 600;
            _textBoxBase.SpellCheck.IsEnabled = true;
            _textBoxBase.Language = GetTestXmlLanguage(_testStartLocale);

            _wrapper = new UIElementWrapper(_textBoxBase);
            _wrapper.Text = _testContent;

            foreach (Uri uri in _customDictionaryUris)
            {
                _textBoxBase.SpellCheck.CustomDictionaries.Add(uri);
            }

            StackPanel panel = new StackPanel();
            panel.Children.Add(_textBoxBase);
            MainWindow.Content = panel;

            QueueDelegate(DoVerification);
        }

        /// <summary>
        /// Cleanup stale %temp%\*.dic and %temp%\*.tmp files
        /// Also cleanup keys under HKCU\Software\Microsoft\Spelling\Dictionaries 
        /// The net effect would be to ensure that ISpellChecker COM server has no 
        /// custom dictionaries registered with it. 
        /// </summary>
        private void CleanupStaleDictionaries()
        {
            Log("Starting temp file cleanup");
            try
            {
                var tempPath = System.IO.Path.GetTempPath();

                var dictionaryFiles = System.IO.Directory.EnumerateFiles(tempPath, @"*.dic");

                int count = 0;
                int deletedCount = 0;
                foreach (var file in dictionaryFiles)
                {
                    count++;
                    try
                    {
                        System.IO.File.Delete(file);
                        deletedCount++;
                    }
                    catch { }
                }

                Log(string.Format("Deleted {0} of {1} *.dic files", deletedCount, count));

                count = deletedCount = 0;
                var tmpFiles = System.IO.Directory.EnumerateFiles(tempPath, @"*.tmp");
                foreach (var file in tmpFiles)
                {
                    count++;
                    try
                    {
                        System.IO.File.Delete(file);
                        deletedCount++;
                    }
                    catch { }
                }
                Log(string.Format("Deleted {0} of {1} *.tmp files", deletedCount, count));

                var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Spelling\Dictionaries", writable:true);
                foreach (string valueName in key.GetValueNames())
                {
                    try
                    {
                        key.DeleteValue(valueName);
                        Log("Deleted " + key.Name + "\\" + valueName);
                    }
                    catch (Exception e)
                    {
                        Log("Failed to delete " + key.Name + "\\" + valueName + ": " + e.GetType().Name);
                    }
                }
            }
            catch
            {
                // do nothing
            }
        }

        private void DoVerification()
        {
            // Wrap in try-finally and ensure that custom dictionaries are explicitly 
            // released. Custom dictionaries on Win8.1 and above are persistently registered
            // to the user and remain registered until they are explicitly deregistered. 
            // Before the start of the test, we try to cleanup all previously registered
            // custom dictionaries, but we need to explicilty release them so that 
            // those registered in one test case combination (and left stale) do not interfere
            // with the next combination
            try
            {      
                // Verify that the container has the expected XMlLanguage
                VerifyContainerXmlLanguage(_testStartLocale);

                // Verify speller results based on content and XmlLanguage
                VerifySpellerResults(_testStartLocale);

                // Change the XmlLanguage of the container
                _textBoxBase.Language = GetTestXmlLanguage(_testEndLocale);
                Microsoft.Test.Threading.DispatcherHelper.DoEvents();

                // Verify that the container has the expected XMlLanguage after the Language has been changed
                VerifyContainerXmlLanguage(_testEndLocale);

                // Verify speller results after XmlLanguage has been changed
                VerifySpellerResults(_testEndLocale);
            }
            finally
            {
                _textBoxBase.SpellCheck.CustomDictionaries.Clear();
            }

            NextCombination();
        }

        private void VerifySpellerResults(string testLocale)
        {
            int expectedErrorCount = -1;
            bool expectSuggestion = false;

            // Content of the test determines what the result of the Speller check should be
            switch (_testContent)
            {
                // One string found in the dictionary matching the language 
                case "EnglishLangxx FrenchLangxx GermanLangxx SpanishLangxx JapaneseLangxx":
                    expectedErrorCount = 4;
                    expectSuggestion = false;
                    break;
                // All misspelled words with a suggestion in the dictionary matching the language
                case "EnglishLangxxw FrenchLangxxw GermanLangxxw SpanishLangxxw JapaneseLangxxw":
                    expectedErrorCount = 5;
                    expectSuggestion = true;
                    break;
            }

            // There is not a japanese default speller dictionary, so even if there is a japanese custom dictionary,
            // there will be no speller support.
            if (testLocale.ToLower() == "japanese")
            {
                expectedErrorCount = 0;
            }

            ArrayList spellingErrors = GetSpellingErrors();

            Verifier.Verify(spellingErrors.Count == expectedErrorCount, "Verifying that the container is reporting the expected number of spelling errors.", true);

            if (expectedErrorCount > 0)
            {
                // There should only be a single spelling suggestion. 
                // The spelling suggestion should contain the locale of the test.             
                if (expectSuggestion)
                {
                    bool gotExpectedSuggestion = false;
                    int suggestionCount = 0;

                    foreach (SpellingError error in spellingErrors)
                    {
                        string suggestion = GetFirstSpellingSuggestion(error);
                        if (suggestion != string.Empty)
                        {
                            suggestionCount++;
                            if (suggestion.ToLower().Contains(testLocale.ToLower()))
                            {
                                gotExpectedSuggestion = true;
                            }
                        }
                    }

                    Verifier.Verify(suggestionCount == 1, "Verifying that there was only one SpellingError with a suggestion.", true);
                    Verifier.Verify(gotExpectedSuggestion, "Verifying that there was a valid spelling suggestion for the spelling error.", true);
                }
            }
        }

        private void VerifyContainerXmlLanguage(string expectedLocale)
        {
            XmlLanguage currentXmlLanguage = _textBoxBase.Language;
            XmlLanguage expectedXmlLanguage = GetTestXmlLanguage(expectedLocale);

            Log(string.Format("Current XmlLanguage: {0}", currentXmlLanguage.ToString()));
            Log(string.Format("Expected XmlLanguage: {0}", expectedXmlLanguage.ToString()));

            Verifier.Verify(currentXmlLanguage.ToString() == expectedXmlLanguage.ToString(), "Verifying that the container has the expected XmlLanguage.", true);
        }

        private ArrayList GetSpellingErrors()
        {
            ArrayList errorList = new ArrayList();

            if (_textBoxBase is TextBox)
            {
                int errorIndex = 0;
                while ((errorIndex = ((TextBox)_textBoxBase).GetNextSpellingErrorCharacterIndex(errorIndex, LogicalDirection.Forward)) > -1)
                {
                    errorList.Add(((TextBox)_textBoxBase).GetSpellingError(errorIndex));
                    errorIndex += ((TextBox)_textBoxBase).GetSpellingErrorLength(errorIndex);
                }
            }
            else
            {
                TextPointer errorPointer = ((RichTextBox)_textBoxBase).Document.ContentStart;
                while ((errorPointer = ((RichTextBox)_textBoxBase).GetNextSpellingErrorPosition(errorPointer, LogicalDirection.Forward)) != null)
                {
                    errorList.Add(((RichTextBox)_textBoxBase).GetSpellingError(errorPointer));
                    errorPointer = ((RichTextBox)_textBoxBase).GetSpellingErrorRange(errorPointer).End.GetNextInsertionPosition(LogicalDirection.Forward);
                    if (errorPointer == null)
                    {
                        break;
                    }
                }                
            }
            return errorList;
        }

        private string GetFirstSpellingSuggestion(SpellingError spellingError)
        {
            foreach (string suggestion in spellingError.Suggestions)
            {
                return suggestion;
            }
            return string.Empty;
        }

        private XmlLanguage GetTestXmlLanguage(string testLocale)
        {
            switch (testLocale.ToLower())
            {
                case "english":
                    return XmlLanguage.GetLanguage("en-us");
                case "french":
                    return XmlLanguage.GetLanguage("fr-fr");
                case "german":
                    return XmlLanguage.GetLanguage("de-de");
                case "spanish":
                    return XmlLanguage.GetLanguage("es-es");
                case "japanese":
                    return XmlLanguage.GetLanguage("ja-jp");
                default:
                    throw new ApplicationException("Invalid value for testLocale [" + testLocale + "]");
            }
        }

        // This test needs the speller dictionary for the current XmlLanguage in order to run.
        // To satisfy this requirement the following must be met:        
        // ** The wpf language pack for both testStartLocale and testEndLocale must be installed
        private bool DetermineIfTestShouldRun()
        {            
            // If both testStartLocale and testEndLocale default dictionaries are available
            if (IsCultureSupported(_testStartLocale) && IsCultureSupported(_testEndLocale))
            {
                return true;
            }

            return false;
        }

        private bool IsCultureSupported(string language)
        {
            System.Globalization.CultureInfo currentCultureInfo = Microsoft.Test.Globalization.LanguagePackHelper.CurrentWPFUICulture();

            switch (language.ToLower())
            {
                case "english": // English is always supported
                    return true;
                case "french":
                    return (currentCultureInfo.LCID == 1036);
                case "german":
                    return (currentCultureInfo.LCID == 1031);
                case "spanish":
                    return (currentCultureInfo.LCID == 3082);
                case "japanese": // There are no Japanese speller dictionaries, so presence or absence does not matter
                    return true;
            }
            return false;
        }

        #endregion

        #region Private fields

        // Combinatorial engine variables; set to default values                
        private TextEditableType _testControlType = null;
        private string _testContent = string.Empty;
        private string _testStartLocale = string.Empty;
        private string _testEndLocale = string.Empty;

        private Uri[] _customDictionaryUris = null;
        private TextBoxBase _textBoxBase;
        private UIElementWrapper _wrapper;

        #endregion
    }
}
