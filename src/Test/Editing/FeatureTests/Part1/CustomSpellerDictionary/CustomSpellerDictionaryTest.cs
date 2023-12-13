// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Verifies basic speller functinality when loading custom speller dictionaries

using System;
using System.Collections;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Xaml;

using Microsoft.Test.Discovery;
using Microsoft.Test.Threading;

using Test.Uis.Data;
using Test.Uis.Loggers;
using Test.Uis.TestTypes;
using Test.Uis.Utils;
using Test.Uis.Wrappers;

namespace Microsoft.Test.Editing
{
    /// <summary>
    /// This test verifies basic speller functionality in RichTextBox and Textbox using a custom speller dictionary.
    /// </summary>
    [Test(0, "Speller", "CustomSpellerDictionary_Pri0", MethodParameters = "/TestCaseType:CustomSpellerDictionaryTest /Pri=0", SupportFiles = @"FeatureTests\Editing\*.lex", Keywords = "Localization_Suite,MicroSuite", Timeout = 100)]
    [Test(2, "Speller", "CustomSpellerDictionary_PartialTrust", MethodParameters = "/TestCaseType:CustomSpellerDictionaryTest /XbapName=EditingTestDeployPart1", SupportFiles = @"FeatureTests\Editing\EditingTestDeployPart1*", Timeout = 200)]
    [Test(0, "Speller", "CustomSpellerDictionary_UriTypes", MethodParameters = "/TestCaseType:CustomSpellerDictionaryTest /Pri=1", SupportFiles = @"FeatureTests\Editing\*.lex", Keywords = "Localization_Suite", Timeout = 100)]
    [Test(0, "Speller", "CustomSpellerDictionary_Xaml_Main", MethodParameters = "/TestCaseType:CustomSpellerDictionaryTest /Pri=3", SupportFiles = @"FeatureTests\Editing\CustomSpeller*xaml,FeatureTests\Editing\CustomSpellerDictionary1.lex", Keywords = "Localization_Suite", Timeout = 100)]
    [Test(0, "Speller", "CustomSpellerDictionary_Xaml_UriTypes", MethodParameters = "/TestCaseType:CustomSpellerDictionaryTest /Pri=4", SupportFiles = @"FeatureTests\Editing\CustomSpeller*xaml,FeatureTests\Editing\CustomSpellerDictionary1.lex", Keywords = "Localization_Suite", Timeout = 100)]
    [Test(2, "Speller", "CustomSpellerDictionary_Pri2", MethodParameters = "/TestCaseType:CustomSpellerDictionaryTest /Pri=2", SupportFiles = @"FeatureTests\Editing\*.lex", Keywords = "Localization_Suite", Timeout = 100)]    
    public class CustomSpellerDictionaryTest : ManagedCombinatorialTestCase
    {
        #region Main flow

        /// <summary>Starts the combination</summary>
        protected override void DoRunCombination()
        {
            if (CombinationNumber == 1)
            {
                CleanupStaleDictionaries();
            }

            BuildTextBoxBase();

            if (_testEnableSpellCheckBeforeDictionaryCreation)
            {
                _textBoxBase.SpellCheck.IsEnabled = true;
            }

            // There are separate localization tests, so for these tests we make sure we are testing in English so that the test strings work                       
            _textBoxBase.Language = XmlLanguage.GetLanguage("en-us");

            _wrapper = new UIElementWrapper(_textBoxBase);

            if (_testContentLoadedBeforeDictionaryCreation)
            {
                if (!_testXaml)
                {
                    _wrapper.Text = _testContent;
                }
            }

            if (!_testXaml)
            {
                // Hook Up CustomSpellerDictionary
                foreach (Uri uri in _testCustomDictionaryUris)
                {
                    Log(string.Format("Testing Uri: {0}", uri.ToString()));
                    _textBoxBase.SpellCheck.CustomDictionaries.Add(uri);
                }
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

        private void BuildTextBoxBase()
        {
            if (!_testXaml)
            {
                _textBoxBase = (TextBoxBase)_testControlType.CreateInstance();
                _textBoxBase.FontSize = 24;
                _textBoxBase.Height = 230;
                _textBoxBase.Width = 300;
                return;
            }

            if(TextEditableType.GetByName("TextBox") == _testControlType)
            {                
                _textBoxBase = (TextBoxBase)XamlServices.Load("CustomSpeller_TextBox_" + _testXamlUriType + ".xaml");                
            }
            else
            {                
                _textBoxBase = (TextBoxBase)XamlServices.Load("CustomSpeller_RichTextBox_" + _testXamlUriType + ".xaml");  
            }
        }
        
        private void DoVerification()
        {
            const int waitMs = 500;

            // Wrap in try-finally and ensure that custom dictionaries are explicitly 
            // released. Custom dictionaries on Win8.1 and above are persistently registered
            // to the user and remain registered until they are explicitly deregistered. 
            // Before the start of the test, we try to cleanup all previously registered
            // custom dictionaries, but we need to explicilty release them so that 
            // those registered in one test case combination (and left stale) do not interfere
            // with the next combination
            try
            {
                // Do simple property value verification
                VerifyPropertyValuesAfterSet();

                if (!_testEnableSpellCheckBeforeDictionaryCreation)
                {
                    _textBoxBase.SpellCheck.IsEnabled = true;
                }

                if (!_testContentLoadedBeforeDictionaryCreation)
                {
                    _wrapper.Text = _testContent;
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents();
                }

                if (_testDisableEnableSpellCheck)
                {
                    VerifyDisableEnableSpellCheck();
                }

                if (_testClearCustomDictionary)
                {
                    _textBoxBase.SpellCheck.CustomDictionaries.Clear();
                    // ensure that custom dictionaries can be added again after Clear()
                    if (!_testXaml)
                    {
                        foreach (Uri uri in _testCustomDictionaryUris)
                        {
                            Log(string.Format("Testing Uri: {0}", uri.ToString()));
                            _textBoxBase.SpellCheck.CustomDictionaries.Add(uri);
                        }
                    }
                    // clear again
                    _textBoxBase.SpellCheck.CustomDictionaries.Clear();
                }

                if (_testInvalidDictionaryLoad)
                {
                    LoadInvalidDictionary();
                }

                DispatcherHelper.DoEvents(waitMs);
                VerifySpellerResults();
            }
            finally
            {
                _textBoxBase.SpellCheck.CustomDictionaries.Clear();
                _textBoxBase.SpellCheck.IsEnabled = false;
            }

            DispatcherHelper.DoEvents(waitMs);
            NextCombination();
        }

        private void VerifyPropertyValuesAfterSet()
        {           
            IList customDictionaryList = _textBoxBase.SpellCheck.CustomDictionaries;            
           
            foreach (Uri uri in customDictionaryList)
            {
                Log(string.Format("Found: {0}", uri.ToString()));
            }
            
            if (!_testXaml)
            {
                Log(string.Format("customDictionaryList.Count: {0}", customDictionaryList.Count));
                Log(string.Format("testCustomDictionaryUris.Length: {0}", _testCustomDictionaryUris.Length));
                Verifier.Verify(customDictionaryList.Count == _testCustomDictionaryUris.Length, "Verifying CustomDictionary count after property is set.", true);
            }
            else
            {
                int expectedCount = 1;
                if (_testXamlUriType.ToLower() == "multiple")
                {
                    expectedCount = 4;
                }

                Verifier.Verify(customDictionaryList.Count == expectedCount, "Verifying CustomDictionary count after property is set.", true);
                return;
            }

            // Only proceed with verification if the arrays are of the same length            
            if (customDictionaryList.Count == _testCustomDictionaryUris.Length)
            {                               
                for(int i = 0; i < customDictionaryList.Count; i++)
                {
                    Verifier.Verify(Uri.Compare(customDictionaryList[i] as Uri, _testCustomDictionaryUris[i], UriComponents.SerializationInfoString, UriFormat.SafeUnescaped, StringComparison.InvariantCulture) == 0, "Verifying Individual Uris of Custom Speller Dictionary after property set", true);                   
                }
            }
        }

        private void VerifySpellerResults()
        {           
            bool expectError = false;
            bool expectSuggestion = false;

            // Content of the test determines what the result of the Speller check should be
            switch (_testContent)
            {
                // Valid string: expect no errors
                case "no error string":
                    expectError = false;
                    break;
                // String only defined in the custom dictionary
                case "xmadeupstringx":
                    if (!_testClearCustomDictionary)
                    {
                        expectError = false;
                    }
                    else
                    {
                        expectError = true;
                        expectSuggestion = false;
                    }
                    break;
                // String only defined in the custom dictionary
                case "Ymadeupstringy":
                    if (!_testClearCustomDictionary)
                    {
                        expectError = false;
                    }
                    else
                    {
                        expectError = true;
                        expectSuggestion = false;
                    }
                    break;
                // String not defined in custom dictionary, but should get a suggestion from word defined in custom dictionary
                case "madeupstringx":
                    if (!_testClearCustomDictionary)
                    {
                        expectError = true;
                        expectSuggestion = true;
                    }
                    else
                    {
                        expectError = true;
                        expectSuggestion = false;
                    }
                    break;
                // String not defined in custom dictionary, but should get a suggestion from word defined in custom dictionary
                case "Ymadeupstringtyq":
                    if (!_testClearCustomDictionary)
                    {
                        expectError = true;
                        expectSuggestion = true;
                    }
                    else
                    {
                        expectError = true;
                        expectSuggestion = false;
                    }
                    break;
                // String not defined in custom and default dictionary, but should get a suggestion from word defined in custom and default dictionary               
                case "lexicona":
                    if (!_testClearCustomDictionary)
                    {
                        expectError = true;
                        expectSuggestion = true;
                    }
                    else
                    {
                        expectError = true;
                        expectSuggestion = true;
                    }
                    break;
                // String not defined in custom dictionary, but should get a suggestion from word defined in default dictionary
                case "flowe":
                    if (!_testClearCustomDictionary)
                    {
                        expectError = true;
                        expectSuggestion = true;
                    }
                    else
                    {
                        expectError = true;
                        expectSuggestion = true;
                    }
                    break;
                // Nonsense string.  Expect error, but no suggestions
                case "nebocnor":
                    expectError = true;
                    expectSuggestion = false;
                    break;
                default:
                    throw new ApplicationException("Invalid value for testContent [" + _testContent + "]");
            }
            
            Verifier.Verify(_textBoxBase.SpellCheck.IsEnabled, "Verifying that SpellCheck is enabled.", true);

            bool hasError = ControlHasSpellingError();
            if (expectError && !hasError && System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }

            if (!expectError && hasError && System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }

            if (!expectError)
            {
                Verifier.Verify(!hasError, "Verifying that there are no errors reported from a string containing no errors", true);
            }
            else
            {
                Verifier.Verify(hasError, "Verifying that there is a spelling error reported", true);

                SpellingError spellingError = GetFirstSpellingError();

                if (_testIgnoreSpellingError)
                {
                    spellingError.IgnoreAll();

                    Microsoft.Test.Threading.DispatcherHelper.DoEvents();
                    hasError = ControlHasSpellingError();

                    Verifier.Verify(!hasError, "Verifying that there are no errors reported from a string containing no errors (after correction)", true);
                }
                else
                {
                    // If we expect a suggestion:
                    // 1. Verify that there is a suggestion
                    // 2. Correct the error with the suggestion
                    // 3. Verify that the spelling error is gone
                    if (expectSuggestion)
                    {
                        Verifier.Verify(ControlHasSpellerSuggestion(spellingError), "Verifying that there is a spelling suggestion for the current spelling error", true);

                        // Correct the spelling error with the first suggested word (should be from custom dictionary)
                        spellingError.Correct(GetFirstSpellingSuggestion(spellingError));

                        Microsoft.Test.Threading.DispatcherHelper.DoEvents();

                        // Now there should be no spelling errors after the correction
                        Verifier.Verify(!ControlHasSpellingError(), "Verifying that there are no errors reported from a string containing no errors (after correction)", true);
                    }
                }
            }
        }

        // When loading an invalid dictionary, we should be able to catch the resulting exception.
        // The dictionary state prior to the load should be maintained.
        private void LoadInvalidDictionary()
        {
            Uri invalidUri = new Uri("InvalidSpellerDictionary.lex", UriKind.Relative);

            // Load a bad dictionary.            
            try
            {
                _textBoxBase.SpellCheck.CustomDictionaries.Add(invalidUri);
                throw new ApplicationException("Loading this dictionary should have failed!");
            }
            catch (System.ArgumentException) { }                      
        }

        private bool ControlHasSpellingError()
        {
            if (_textBoxBase is TextBox)
            {
                int errorIndex = ((TextBox)_textBoxBase).GetNextSpellingErrorCharacterIndex(0, LogicalDirection.Forward);
                if (errorIndex == -1)
                {
                    return false;
                }
            }
            else
            {
                TextPointer errorPointer = ((RichTextBox)_textBoxBase).GetNextSpellingErrorPosition(((RichTextBox)_textBoxBase).Document.ContentStart, LogicalDirection.Forward);
                if (errorPointer == null)
                {
                    return false;
                }
            }
            return true;
        }

        private SpellingError GetFirstSpellingError()
        {
            if (!ControlHasSpellingError())
            {
                return null;
            }

            SpellingError spellingError = null;

            if (_textBoxBase is TextBox)
            {
                int errorIndex = ((TextBox)_textBoxBase).GetNextSpellingErrorCharacterIndex(0, LogicalDirection.Forward);
                spellingError = ((TextBox)_textBoxBase).GetSpellingError(errorIndex);
            }
            else
            {
                TextPointer errorPointer = ((RichTextBox)_textBoxBase).GetNextSpellingErrorPosition(((RichTextBox)_textBoxBase).Document.ContentStart, LogicalDirection.Forward);
                spellingError = ((RichTextBox)_textBoxBase).GetSpellingError(errorPointer);
            }

            return spellingError;
        }

        // Should be able to disable then re-enable SpellCheck and maintain the Custom Speller Dictionary
        private void VerifyDisableEnableSpellCheck()
        {
            _textBoxBase.SpellCheck.IsEnabled = false;

            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            Verifier.Verify(!ControlHasSpellingError(), "Verifying that there are no Spelling errors reported when SpellCheck has been disabled", true);

            _textBoxBase.SpellCheck.IsEnabled = true;

            Microsoft.Test.Threading.DispatcherHelper.DoEvents();
        }

        private bool ControlHasSpellerSuggestion(SpellingError spellingError)
        {
            foreach (string suggestion in spellingError.Suggestions)
            {
                return true;
            }
            return false;
        }

        private string GetFirstSpellingSuggestion(SpellingError spellingError)
        {
            foreach (string suggestion in spellingError.Suggestions)
            {
                return suggestion;
            }
            return string.Empty;
        }

        #endregion

        #region Private fields

        // Combinatorial engine variables; set to default values                
        private TextEditableType _testControlType = null;
        private string _testContent = string.Empty;
        private Uri[] _testCustomDictionaryUris = null;        
        private bool _testClearCustomDictionary = false;
        private bool _testDisableEnableSpellCheck = false;
        private bool _testIgnoreSpellingError = false;
        private bool _testEnableSpellCheckBeforeDictionaryCreation = false;
        private bool _testContentLoadedBeforeDictionaryCreation = false;
        private bool _testInvalidDictionaryLoad = false;
        private bool _testXaml = false;
        private string _testXamlUriType = string.Empty;

        private TextBoxBase _textBoxBase;
        private UIElementWrapper _wrapper;

        #endregion
    }
}
