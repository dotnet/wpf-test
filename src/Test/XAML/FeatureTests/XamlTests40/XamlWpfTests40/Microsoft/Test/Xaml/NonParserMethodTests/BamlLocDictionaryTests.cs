// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Markup.Localizer;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.TestTypes;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.NonParserMethodTests
{
    /// <summary>
    /// BamlLocalizationDictionary Tests 
    /// </summary>
    public class BamlLocDictionaryTests
    {
        /// <summary>A boolean indicating whether or not the entire test passed.</summary>
        private bool _testPassed = true;

        /// <summary>The name of the .baml file being tested.</summary>
        private string _bamlFileName = string.Empty;

        /******************************************************************************
        * Function:          GenerateInfosetFromBaml
        ******************************************************************************/

        /// <summary>
        /// Verify methods and properties on BamlLocalizationDictionary.
        /// </summary>
        public void VerifyBamlLocalizationDictionary()
        {
            string xamlFileName = DriverState.DriverParameters["XamlFile"];
            if (String.IsNullOrEmpty(xamlFileName))
            {
                throw new TestSetupException("XamlFileName cannot be null.");
            }

            if (!File.Exists(xamlFileName))
            {
                throw new TestSetupException("ERROR: the Xaml file specified does not exist.");
            }

            _bamlFileName = BamlFactory.CompileXamlToBaml(xamlFileName, null);

            /************************************************************************
            / Test #1
            /************************************************************************/
            GlobalLog.LogStatus("Test #1: Verify Defaults");

            Actual actual = new Actual();
            actual.Entries = new DictionaryEntry[1];
            actual.Dictionary = new BamlLocalizationDictionary();
            actual.Keys = (ICollection)actual.Dictionary.Keys;
            actual.Values  = (ICollection)actual.Dictionary.Values;
            actual.Enumerator = actual.Dictionary.GetEnumerator();
            actual.ResourceKey = new BamlLocalizableResourceKey("BN1", "System.Windows.UIElement", "Opacity");
            actual.Dictionary.CopyTo(actual.Entries, 0);

            Expected expected = new Expected();
            expected.Count = 0;
            expected.IsFixedSize = false;
            expected.IsReadOnly = false;
            expected.RootElementKeyNull = true;
            expected.EnumNull = false;
            expected.Contains = false;
            expected.EntryKeyNull = true;
            
            VerifyBasics(expected, actual);

            /************************************************************************
            / Tests #2-5
            /************************************************************************/
            using (MemoryStream memStream = XamlBamlObjectFactory.GetMemoryStream(_bamlFileName))
            {
                // Create a BamlLocalizer on the stream.
                BamlLocalizer localizer = new BamlLocalizer(memStream);
                actual.Dictionary = localizer.ExtractResources();
                actual.Keys = (ICollection)actual.Dictionary.Keys;
                actual.Values  = (ICollection)actual.Dictionary.Values;
                actual.Enumerator = actual.Dictionary.GetEnumerator();
                actual.Entries = new DictionaryEntry[15];
                actual.Dictionary.CopyTo(actual.Entries, 0);
                
                expected.Uids = new string[15] { "CV1", "CV1", "SP1", "SP1", "TB1", "TB1", "TB1", "BN1", "BN1", "SP2", "SP2", "TB2", "TB2", "BN2", "BN2" };
                expected.ClassNames = new string[15] { "System.Windows.Controls.Canvas", "System.Windows.Controls.Panel", "System.Windows.Controls.StackPanel", "System.Windows.FrameworkElement", "System.Windows.Controls.TextBox", "System.Windows.FrameworkElement", "System.Windows.UIElement", "System.Windows.Controls.Button", "System.Windows.UIElement", "System.Windows.Controls.StackPanel", "System.Windows.UIElement", "System.Windows.Controls.TextBox", "System.Windows.UIElement", "System.Windows.Controls.Button", "System.Windows.UIElement" };
                expected.PropertyNames = new string[15] { "$Content", "Background", "$Content", "Height", "$Content", "Width", "AllowDrop", "$Content", "Opacity", "$Content", "Focusable", "$Content", "Visibility", "$Content", "ClipToBounds" };
                expected.ContentValues = new string[15] { "#SP1;#SP2;", "\\#FFFFE4E1", "#TB1;#BN1;", "50", string.Empty, "45", "True", string.Empty, "0.5", "#TB2;#BN2;", "False", string.Empty, "Collapsed", string.Empty, "False" };
                expected.Count = 15;
                expected.IsFixedSize = false;
                expected.IsReadOnly = false;
                expected.RootElementKeyNull = false;
                expected.EnumNull = false;
                expected.Contains = true;
                expected.EntryKeyNull = false;

                /************************************************************************
                / Test #2
                /************************************************************************/
                GlobalLog.LogStatus("Test #2: Verify properties/methods based on specific .baml file");

                VerifyBasics(expected, actual);

                if (actual.Dictionary.Count != expected.Count)
                {
                    GlobalLog.LogEvidence("Skipping remaining tests:  BamlLocalizationDictionary count is incorrect. expected: " + expected.Count.ToString() + " / Actual: " + actual.Dictionary.Count.ToString());
                }
                else
                {
                    /************************************************************************
                    / Test #3
                    /************************************************************************/
                    GlobalLog.LogStatus("Test #3: Verify properties via BamlLocalizationDictionary");

                    int j = 0;
                    foreach (DictionaryEntry item in actual.Dictionary)
                    {
                        VerifyDictionaryProperties(j, (BamlLocalizableResourceKey)item.Key, (BamlLocalizableResource)item.Value, expected);
                        j++;
                    }

                    /************************************************************************
                    / Test #4
                    /************************************************************************/
                    GlobalLog.LogStatus("Test #4: Verify properties via BamlLocalizationDictionaryEnumerator");

                    actual.Enumerator.Reset();
                    actual.Enumerator = actual.Dictionary.GetEnumerator();
                    for (int k = 0; k < actual.Dictionary.Count; k++)
                    {
                        actual.Enumerator.MoveNext();
                        VerifyDictionaryProperties(k, (BamlLocalizableResourceKey)actual.Enumerator.Entry.Key, (BamlLocalizableResource)actual.Enumerator.Entry.Value, expected);
                    }

                    /************************************************************************
                    / Test #5
                    /************************************************************************/
                    GlobalLog.LogStatus("Test #5: Verify additional Methods");

                    BamlLocalizableResource bamlResourceABC = new BamlLocalizableResource("#ABC", "A Comment.", LocalizationCategory.None, true, true);
                    BamlLocalizableResourceKey bamlResourceKeyABC = new BamlLocalizableResourceKey("#ABC", "System.Windows.Controls.TextBox", "Text");

                    GlobalLog.LogStatus("-----#5a: Verify CopyTo()");
                    int m = 0;
                    foreach (DictionaryEntry entry in actual.Entries)
                    {
                        VerifyDictionaryProperties(m, (BamlLocalizableResourceKey)entry.Key, (BamlLocalizableResource)entry.Value, expected);
                        m++;
                    }

                    GlobalLog.LogStatus("-----#5b: Verify Add()");
                    actual.Dictionary.Add(bamlResourceKeyABC, bamlResourceABC);
                    if (!actual.Dictionary.Contains(bamlResourceKeyABC))
                    {
                        _testPassed = false;
                        GlobalLog.LogEvidence("FAIL: the Add method failed.");
                    }

                    GlobalLog.LogStatus("-----#5c: Verify BamlLocalizableResource.Equals()");
                    BamlLocalizableResource bamlResourceRetrieved = actual.Dictionary[bamlResourceKeyABC];
                    if (!bamlResourceABC.Equals(bamlResourceRetrieved))
                    {
                        _testPassed = false;
                        GlobalLog.LogEvidence("FAIL: the Equals method failed.");
                    }

                    GlobalLog.LogStatus("-----#5d: Verify Remove()");
                    actual.Dictionary.Remove(bamlResourceKeyABC);
                    if (actual.Dictionary.Contains(bamlResourceKeyABC))
                    {
                        _testPassed = false;
                        GlobalLog.LogEvidence("FAIL: the Remove method failed.");
                    }

                    GlobalLog.LogStatus("-----#5e: Verify Clear()");
                    actual.Dictionary.Clear();
                    if (actual.Dictionary.Count != 0)
                    {
                        _testPassed = false;
                        GlobalLog.LogEvidence("FAIL: the Clear method failed.");
                    }
                }
            }

            /************************************************************************
            / Final Pass/Fail
            /************************************************************************/

            if (_testPassed)
            {
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                TestLog.Current.Result = TestResult.Fail;
            }

            BamlFactory.CleanUpBamlCompilation(_bamlFileName);
        }

        /******************************************************************************
        * Function:          VerifyBasics
        ******************************************************************************/

        /// <summary>
        /// Verify basic properties and methods.
        /// </summary>
        /// <param name="expected">A struct containing expected values.</param>
        /// <param name="actual">Actual objects to be verified.</param>
        private void VerifyBasics(Expected expected, Actual actual)
        {
            if (actual.Dictionary.Count != expected.Count
                || actual.Dictionary.IsFixedSize != expected.IsFixedSize
                || actual.Dictionary.IsReadOnly != expected.IsReadOnly
                || (actual.Dictionary.RootElementKey == null) != expected.RootElementKeyNull
                || actual.Keys.Count != expected.Count
                || actual.Values.Count != expected.Count
                || (actual.Enumerator == null) != expected.EnumNull
                || actual.Dictionary.Contains(actual.ResourceKey) != expected.Contains
                || (actual.Entries[0].Key == null) != expected.EntryKeyNull)
            {
                _testPassed = false;
                GlobalLog.LogEvidence(" FAILED: One or more BamlLocalizationDictionary values is incorrect.");
            
                GlobalLog.LogStatus("****EXPECTED*********************************");
                GlobalLog.LogStatus("Count:                " + expected.Count.ToString());
                GlobalLog.LogStatus("IsFixedSize:          " + expected.IsFixedSize.ToString());
                GlobalLog.LogStatus("IsReadOnly:           " + expected.IsReadOnly.ToString());
                GlobalLog.LogStatus("RootElementKey null?: " + expected.RootElementKeyNull.ToString());
                GlobalLog.LogStatus("Keys.Count:           " + expected.Count.ToString());
                GlobalLog.LogStatus("Values.Count:         " + expected.Count.ToString());
                GlobalLog.LogStatus("Enumerator null?      " + expected.EnumNull.ToString());
                GlobalLog.LogStatus("Contains?:            " + expected.Contains.ToString());
                GlobalLog.LogStatus("entries[0].Key null?: " + expected.EntryKeyNull.ToString());
          
                GlobalLog.LogStatus("****ACTUAL***********************************");
                GlobalLog.LogStatus("Count:                " + actual.Dictionary.Count.ToString());
                GlobalLog.LogStatus("IsFixedSize:          " + actual.Dictionary.IsFixedSize.ToString());
                GlobalLog.LogStatus("IsReadOnly:           " + actual.Dictionary.IsReadOnly.ToString());
                GlobalLog.LogStatus("RootElementKey null?: " + (actual.Dictionary.RootElementKey == null).ToString());
                GlobalLog.LogStatus("Keys.Count:           " + actual.Keys.Count.ToString());
                GlobalLog.LogStatus("Values.Count:         " + actual.Values.Count.ToString());
                GlobalLog.LogStatus("Enumerator null?      " + (actual.Enumerator == null).ToString());
                GlobalLog.LogStatus("Contains?:            " + actual.Dictionary.Contains(actual.ResourceKey).ToString());
                GlobalLog.LogStatus("entries[0].Key null?: " + (actual.Entries[0].Key == null).ToString());
            }
        }

        /******************************************************************************
        * Function:          VerifyDictionaryProperties
        ******************************************************************************/

        /// <summary>
        /// Verify DictionaryEntry Key and Value properties.
        /// The expected values are based on the content of the .xaml file used to create the .baml.
        /// The BamlLocalizationDictionaryEnumerator returns a BamlLocalizableResourceKey and a BamlLocalizableResource.
        /// </summary>
        /// <param name="index">An index to the array of dictionary items.</param>
        /// <param name="key">The BamlLocalizableResourceKey Key object.</param>
        /// <param name="value">The BamlLocalizableResource Value object.</param>
        /// <param name="expected">A struct containing expected values.</param>
        private void VerifyDictionaryProperties(int index, BamlLocalizableResourceKey key, BamlLocalizableResource value, Expected expected)
        {
            bool matchFound = false;
            BamlLocalizableResourceKey[] expKeys = new BamlLocalizableResourceKey[expected.Count];

            // Create an array of BamlLocalizableResourceKeys, based on the expected propery arrays.
            for (int j = 0; j < expected.Count; j++)
            {
                expKeys[j] = new BamlLocalizableResourceKey(expected.Uids[j], expected.ClassNames[j], expected.PropertyNames[j]);
            }

            // Look for a match of each actual BamlLocalizableResourceKey with an entry within an expected list.
            for (int k = 0; k < expected.Count; k++)
            {
                if (key.Equals(expKeys[k]))
                {
                    matchFound = true;
                }
            }

            if (!matchFound)
            {
                _testPassed = false;
                GlobalLog.LogEvidence("FAIL: actual BamlLocalizableResourceKey #" + index.ToString() + " not found amongst the expected keys.");
            }

            CheckPropertyValues(index, key, value, expected);
        }

        /******************************************************************************
        * Function:          CheckPropertyValues
        ******************************************************************************/

        /// <summary>
        /// Compare expected vs. actual dictionary Key and Value properties.
        /// </summary>
        /// <param name="index">An index to the array of dictionary items.</param>
        /// <param name="key">The BamlLocalizableResourceKey Key object.</param>
        /// <param name="value">The BamlLocalizableResource Value object.</param>
        /// <param name="expected">A struct containing expected values.</param>
        private void CheckPropertyValues(int index, BamlLocalizableResourceKey key, BamlLocalizableResource value, Expected expected)
        {
            if (key.Uid != expected.Uids[index])
            {
                _testPassed = false;
                GlobalLog.LogEvidence("FAIL: Key.Uid (#" + index.ToString() + ") Expected: " + expected.Uids[index] + " / Actual: " + key.Uid);
            }

            if (key.ClassName != expected.ClassNames[index])
            {
                _testPassed = false;
                GlobalLog.LogEvidence("FAIL: Key.ClassName (#" + index.ToString() + ") Expected: " + expected.ClassNames[index] + " / Actual: " + key.ClassName);
            }

            if (key.PropertyName != expected.PropertyNames[index])
            {
                _testPassed = false;
                GlobalLog.LogEvidence("FAIL: Key.PropertyName (#" + index.ToString() + ") Expected: " + expected.PropertyNames[index] + " / Actual: " + key.PropertyName);
            }

            if (value.Content != expected.ContentValues[index])
            {
                _testPassed = false;
                GlobalLog.LogEvidence("FAIL: Value.Content (#" + index.ToString() + ") Expected: " + expected.ContentValues[index] + " / Actual: " + value.Content);
            }
        }

        /******************************************************************************
        * Struct:          Expected
        ******************************************************************************/

        /// <summary>
        /// Expected values, based on the content of the .xaml file being tested.
        /// </summary>
        private struct Expected
        {
            /// <summary>The expected count of items in the Dictionary.</summary>
            public int Count;

            /// <summary>The expected value for IsFixedSize.</summary>
            public bool IsFixedSize;

            /// <summary>The expected value for IsReadOnly.</summary>
            public bool IsReadOnly;

            /// <summary>The expected value for RootElementKeyNull.</summary>
            public bool RootElementKeyNull;

            /// <summary>The expected boolean indicating whether or not dictionary.GetEnumerator returns null.</summary>
            public bool EnumNull;

            /// <summary>The expected boolean for dictionary.Contains(resourceKey).</summary>
            public bool Contains;

            /// <summary>The expected boolean indicating whether or not entries[0].Key returns null.</summary>
            public bool EntryKeyNull;

            /// <summary>An array of expected Uids.</summary>
            public string[] Uids;

            /// <summary>An array of expected ClassNames.</summary>
            public string[] ClassNames;

            /// <summary>An array of expected PropertyNames.</summary>
            public string[] PropertyNames;

            /// <summary>An array of expected ContentValues.</summary>
            public string[] ContentValues;

            /// <summary>
            /// Initializes a new instance of the Expected struct.
            /// </summary>
            /// <param name="count">The expected dictionary count.</param>
            /// <param name="isFixedSize">The expected value of dictionary.IsFixedSize.</param>
            /// <param name="isReadOnly">The expected value of dictionary.IsReadOnly.</param>
            /// <param name="rootElementKeyNull">The expected value of dictionary.</param>
            /// <param name="enumNull">The expected value of a null check of dictionary.GetEnumerator.</param>
            /// <param name="contains">The expected value of a check for dictionary.Contains(resourceKey).</param>
            /// <param name="entryKeyNull">The expected of a null check of entries[0].Key.</param>
            /// <param name="uids">Array of expected Uid property values.</param>
            /// <param name="classNames">Array of expected ClassName property values.</param>
            /// <param name="propertyNames">Array of expected PropertyName property values.</param>
            /// <param name="contentValues">Array of expected Content property values.</param>
            public Expected(int count, bool isFixedSize, bool isReadOnly, bool rootElementKeyNull, bool enumNull, bool contains, bool entryKeyNull, string[] uids, string[] classNames, string[] propertyNames, string[] contentValues)
            {
                Count = count;
                IsFixedSize = isFixedSize;
                IsReadOnly = isReadOnly;
                RootElementKeyNull = rootElementKeyNull;
                EnumNull = enumNull;
                Contains = contains;
                EntryKeyNull = entryKeyNull;
                Uids = uids;
                ClassNames = classNames;
                PropertyNames = propertyNames;
                ContentValues = contentValues;
            }
        }

        /******************************************************************************
        * Struct:          Actual
        ******************************************************************************/

        /// <summary>
        /// Actual object to be verified.
        /// </summary>
        private struct Actual
        {
            /// <summary>A BamlLocalizationDictionary to be tested.</summary>
            public BamlLocalizationDictionary Dictionary;

            /// <summary>A BamlLocalizationDictionaryEnumerator object used in verification.</summary>
            public BamlLocalizationDictionaryEnumerator Enumerator;

            /// <summary>A BamlLocalizableResourceKey object used in verification.</summary>
            public BamlLocalizableResourceKey ResourceKey;

            /// <summary>A DictionaryEntry array used in verification.</summary>
            public DictionaryEntry[] Entries;

            /// <summary>A collection returned by dictionary.Keys.</summary>
            public ICollection Keys;

            /// <summary>A collection returned by dictionary.Values.</summary>
            public ICollection Values;

            /// <summary>
            /// Initializes a new instance of the Actual struct.
            /// </summary>
            /// <param name="dictionary">The actual BamlLocalizationDictionary.</param>
            /// <param name="enumerator">The actual BamlLocalizationDictionaryEnumerator.</param>
            /// <param name="resourceKey">The actual BamlLocalizableResourceKey.</param>
            /// <param name="entries">The actual array of DictionaryEntry objects.</param>
            /// <param name="keys">The actual Keys collection.</param>
            /// <param name="values">The actual Values collection.</param>
            public Actual(BamlLocalizationDictionary dictionary, BamlLocalizationDictionaryEnumerator enumerator, BamlLocalizableResourceKey resourceKey, DictionaryEntry[] entries, ICollection keys, ICollection values)
            {
                Dictionary = dictionary;
                Enumerator = enumerator;
                ResourceKey = resourceKey;
                Entries = entries;
                Keys = keys;
                Values = values;
            }
        }
    }
}
