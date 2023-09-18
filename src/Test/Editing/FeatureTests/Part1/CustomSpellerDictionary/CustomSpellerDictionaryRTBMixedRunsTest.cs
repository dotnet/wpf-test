// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Verifies speller functionality when loading custom speller dictionaries in
//  RichTextBoxes that contain Runs with different locale specifications.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;

using Microsoft.Test.Discovery;
using Microsoft.Test.Utilities;
using Test.Uis.Loggers;
using Test.Uis.TestTypes;

namespace Microsoft.Test.Editing
{
    /// <summary>
    /// This test verifies basic speller functionality in RichTextBox and Textbox using a custom speller dictionary with respect to specifying certain locales.
    /// </summary>
    [Test(0, "Speller", "CustomSpellerDictionaryRTBMixedRunsTest", MethodParameters = "/TestCaseType:CustomSpellerDictionaryRTBMixedRunsTest",  Keywords="Localization_Suite", Timeout = 120)]
    public class CustomSpellerDictionaryRTBMixedRunsTest : ManagedCombinatorialTestCase
    {
        #region Main flow

        /// <summary>Starts the combination</summary>
        protected override void DoRunCombination()
        {
            _customDictionaryUris = new Uri[] {
                new Uri("pack://application:,,,/EditingTestPart1;component/CustomDictionaryEnglish.lex"),
                new Uri("pack://application:,,,/EditingTestPart1;component/CustomDictionaryFrench.lex"),
                new Uri("pack://application:,,,/EditingTestPart1;component/CustomDictionarySpanish.lex"),
                new Uri("pack://application:,,,/EditingTestPart1;component/CustomDictionaryGerman.lex"),
                new Uri("pack://application:,,,/EditingTestPart1;component/CustomDictionaryJapanese.lex")
            };

            _richTextBox = new RichTextBox();
            _richTextBox.FontSize = 14;
            _richTextBox.Height = 230;
            _richTextBox.Width = 600;
            _richTextBox.SpellCheck.IsEnabled = true;
            _richTextBox.Language = GetTestXmlLanguage(_testRTBXmlLanguage);

            string[] contentStrings = new string[] {
                "EnglishLangxx EnglishLangxxw ",
                "FrenchLangxxw FrenchLangxx ",
                "GermanLangxx GermanLangxxw ",
                "SpanishLangxxw SpanishLangxx ",
                "JapaneseLangxx JapaneseLangxxw ",
            };

            if (CombinationNumber == 1)
            {
                CleanupStaleDicionaries();
            }

            LoadRichTextBoxContent(contentStrings);

            foreach (Uri uri in _customDictionaryUris)
            {
                _richTextBox.SpellCheck.CustomDictionaries.Add(uri);
            }

            StackPanel panel = new StackPanel();
            panel.Children.Add(_richTextBox);
            MainWindow.Content = panel;

            QueueDelegate(DoVerification);
        }

        /// <summary>
        /// Cleanup stale %temp%\*.dic and %temp%\*.tmp files
        /// Also cleanup keys under HKCU\Software\Microsoft\Spelling\Dictionaries
        /// The net effect would be to ensure that ISpellChecker COM server has no
        /// custom dictionaries registered with it.
        /// </summary>
        private void CleanupStaleDicionaries()
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

        private void LoadRichTextBoxContent(string[] runContents)
        {
            Paragraph paragraph = new Paragraph();

            foreach (string runString in runContents)
            {
                Run run = new Run(runString);
                run.Language = DetermineXmlLanguageFromString(runString);
                paragraph.Inlines.Add(run);
            }

            _richTextBox.Document.Blocks.Clear();
            _richTextBox.Document.Blocks.Add(paragraph);
        }

        // XmlLanguage determination is determined by the content of the string
        private XmlLanguage DetermineXmlLanguageFromString(string runString)
        {
            if (runString.ToLower().Contains("english"))
            {
                return XmlLanguage.GetLanguage("en-us");
            }
            else if (runString.ToLower().Contains("french"))
            {
                return XmlLanguage.GetLanguage("fr-fr");
            }
            else if (runString.ToLower().Contains("german"))
            {
                return XmlLanguage.GetLanguage("de-de");
            }
            else if (runString.ToLower().Contains("spanish"))
            {
                return XmlLanguage.GetLanguage("es-es");
            }
            else if (runString.ToLower().Contains("japanese"))
            {
                return XmlLanguage.GetLanguage("ja-jp");
            }
            else
            {
                throw new ApplicationException("Invalid value for runString [" + runString + "]");
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
                VerifyRichTextBoxXmlLanguage();

                // Verify speller results based on content and XmlLanguage
                VerifySpellerResults();
            }
            finally
            {
                _richTextBox.SpellCheck.CustomDictionaries.Clear();
            }

            NextCombination();
        }

        private void VerifyRichTextBoxXmlLanguage()
        {
            XmlLanguage currentXmlLanguage = _richTextBox.Language;
            XmlLanguage expectedXmlLanguage = GetTestXmlLanguage(_testRTBXmlLanguage);

            Log(string.Format("Current XmlLanguage: {0}", currentXmlLanguage.ToString()));
            Log(string.Format("Expected XmlLanguage: {0}", expectedXmlLanguage.ToString()));

            Verifier.Verify(currentXmlLanguage.ToString() == expectedXmlLanguage.ToString(), "Verifying that the RichTextBox has the expected XmlLanguage.", true);
        }

        private void VerifySpellerResults()
        {
            Paragraph parapraph = _richTextBox.Document.Blocks.FirstBlock as Paragraph;

            foreach (Inline inline in parapraph.Inlines)
            {
                Run currentRun = inline as Run;
                VerifyRunSpellerResults(currentRun);
            }
        }

        private void VerifyRunSpellerResults(Run run)
        {
            List<CultureInfo> inputLanguages = EnumerateInputLanguages();

            XmlLanguage currentXmlLanguage = run.Language;
            XmlLanguage expectedXmlLanguage = DetermineXmlLanguageFromString(run.Text);

            Log(string.Format("Current XmlLanguage: {0}", currentXmlLanguage.ToString()));
            Log(string.Format("Expected XmlLanguage: {0}", expectedXmlLanguage.ToString()));

            Verifier.Verify(currentXmlLanguage.ToString() == expectedXmlLanguage.ToString(), "Verifying that the Run has the expected XmlLanguage.", true);

            System.Globalization.CultureInfo currentWPFUICulture =  Microsoft.Test.Globalization.LanguagePackHelper.CurrentWPFUICulture();

            Log(string.Format("Current WPF UI Culture (EnglishName): {0}", currentWPFUICulture.EnglishName));
            Log(string.Format("Current WPF UI Culture: {0}", currentWPFUICulture.DisplayName));
            Log(string.Format("Run's XmlLanguage.GetEquivalentCulture() (EnglishName): {0}", currentXmlLanguage.GetEquivalentCulture().EnglishName));
            Log(string.Format("Run's XmlLanguage.GetEquivalentCulture(): {0}", currentXmlLanguage.GetEquivalentCulture().DisplayName));

            int expectedErrorCount = 1;

            bool spellCheckingCapability = false;

            // We should query ISpellCheckerFactory::IsSupported(langTag) here, the way
            // SpellerTest.IsSpellerAvailableForLanguage does (see Editing\FeatureTests\Speller\SpellerTest.cs).
            // But that code and the supporting COM wrappers live in a different
            // assembly.

            // Win10 Client: Languages FoD's are auto-installed as input languages are added by the user
            // Win10 Server: All languages are present in the system. This can change until server RTM's
            // Win8.1 client & server: All langs are present in the system.
            if (OSVersion.IsWindows10OrGreater() && !OSVersion.IsWindowsServer())
            {
                CultureInfo currentXmlLanguageCulture = new CultureInfo(currentXmlLanguage.IetfLanguageTag);

                // For now, just see if the InputLanguageManager knows about
                // the language.  This give the right result for the 4 main languages,
                // but not for Japanese.  Japanese spell-checking is never enabled
                // (it doesn't make sense in an ideographic language).
                if (currentXmlLanguageCulture.Equals(_japanese))
                {
                    spellCheckingCapability = false;
                }
                else
                {
                    spellCheckingCapability = (inputLanguages.Find((culture) => culture.Equals(currentXmlLanguageCulture)) != default(CultureInfo));
                }
            }
            else
            {
                // On NLG based systems, we only support 4 languges
                // Our tests haven't been expanded to explicitly test more languages, so for now we continue
                // to expclicitly use these 4 langauges as a hardcoded list.
                spellCheckingCapability =
                    currentXmlLanguage.GetEquivalentCulture().EnglishName.ToLower().Contains("english") ||
                    currentXmlLanguage.GetEquivalentCulture().EnglishName.ToLower().Contains("french")  ||
                    currentXmlLanguage.GetEquivalentCulture().EnglishName.ToLower().Contains("german")  ||
                    currentXmlLanguage.GetEquivalentCulture().EnglishName.ToLower().Contains("spanish");
            }

            expectedErrorCount = (spellCheckingCapability ? 1 : 0);

            ArrayList errorsInRun = GetSpellingErrorsInRun(run);
            foreach (SpellingError error in errorsInRun)
            {
                Log(string.Format("Found {0} errors, expected {1}", errorsInRun.Count, expectedErrorCount));
            }
            Log(string.Format("Processing Run: {0}", run.Text));
            Verifier.Verify(errorsInRun.Count == expectedErrorCount, "Verifying that there were the expected number of errors in the Run.", true);
        }

        private ArrayList GetSpellingErrorsInRun(Run run)
        {
            Log(string.Format("Processing Run: {0}", run.Text));
            ArrayList errorsInRun = new ArrayList();

            TextPointer errorPosition = _richTextBox.GetNextSpellingErrorPosition(run.ContentStart, LogicalDirection.Forward);

            if (errorPosition == null)
            {
                return errorsInRun;
            }

            TextRange runTextRange = new TextRange(run.ContentStart, run.ContentEnd);
            do
            {
                errorPosition = _richTextBox.GetNextSpellingErrorPosition(errorPosition, LogicalDirection.Forward);
                if (errorPosition != null)
                {
                    if (runTextRange.Contains(_richTextBox.GetSpellingErrorRange(errorPosition).End))
                    {
                        errorsInRun.Add(_richTextBox.GetSpellingError(errorPosition));
                    }
                    errorPosition = _richTextBox.GetSpellingErrorRange(errorPosition).End.GetNextInsertionPosition(LogicalDirection.Forward);
                }

            } while (errorPosition != null);

            return errorsInRun;
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

        List<CultureInfo> EnumerateInputLanguages()
        {
            List<CultureInfo> result = new List<CultureInfo>();
            foreach (CultureInfo culture in InputLanguageManager.Current?.AvailableInputLanguages)
            {
                result.Add(culture);
            }

            return result;
        }

        #endregion

        #region Private fields

        private CultureInfo _japanese = new CultureInfo("ja-jp");

        // Combinatorial engine variables; set to default values
        private string _testRTBXmlLanguage = null;

        private Uri[] _customDictionaryUris = null;
        private RichTextBox _richTextBox;

        #endregion
    }
}
