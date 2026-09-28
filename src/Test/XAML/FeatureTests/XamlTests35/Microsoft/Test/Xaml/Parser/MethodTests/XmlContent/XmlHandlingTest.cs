// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Markup;
using System.Windows.Threading;
using Microsoft.Test.Serialization;
using System.Windows.Media;
using Microsoft.Test.Logging;
using Microsoft.Test.Discovery;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.MethodTests.XmlContent
{
    /// <summary>
    /// </summary>
    public class XmlHandlingTest
    {
        /// <summary> xaml file name </summary>
        private static string s_xamlFile = "XmlHandlingTest.xaml";

        /// <summary> oracle dictionary </summary>
        private static Dictionary<string, string> s_oracle = new Dictionary<string, string>();

        /// <summary>
        /// XmlHandling Test constructor
        /// </summary>
        static XmlHandlingTest()
        {
            // Create oracle dictionary.
            s_oracle.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            s_oracle.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
            s_oracle.Add("cc", "clr-namespace:;assembly=CoretestsUntrusted");
            s_oracle.Add("cmn", "clr-namespace:Microsoft.Test.Serialization.CustomElements;assembly=TestRuntime");
        }

        /// <summary>
        /// Verify basic XmlnsDictionary behavior.
        /// </summary>
        public void VerifyXmlnsDictionary()
        {
            UIElement root = (UIElement)SerializationHelper.ParseXamlFile(s_xamlFile);
            Exception ex = null;

            // Verify that dictionary entries are correct from the loaded xaml.
            _VerifyXmlnsDictionary(s_oracle, XmlAttributeProperties.GetXmlnsDictionary(root));

            // Verify that dictionary is sealed from loaded xaml.
            XmlnsDictionary dictionary = XmlAttributeProperties.GetXmlnsDictionary(root);
            try { dictionary[""] = "foo"; }
            catch (Exception e) { ex = e; }

            if (ex == null) throw new Microsoft.Test.TestValidationException("FAILED");

            _VerifyXmlnsDictionary(s_oracle, XmlAttributeProperties.GetXmlnsDictionary(root));

            //
            // Add
            // set_Item(prefix, val)
            //
            dictionary = new XmlnsDictionary();
            foreach (string key in s_oracle.Keys) dictionary.Add(key, s_oracle[key]);
            XmlAttributeProperties.SetXmlnsDictionary(root, dictionary);

            s_oracle[""] = "foo";
            dictionary[""] = "foo";

            _VerifyXmlnsDictionary(s_oracle, XmlAttributeProperties.GetXmlnsDictionary(root));

            //
            // CopyTo
            //
            DictionaryEntry[] entries = new DictionaryEntry[s_oracle.Count];
            dictionary.CopyTo(entries, 0);
            for(int i=0; i < entries.Length; i++)
            {
                string key = (string)entries[i].Key;

                object value = entries[i].Value;
                object expected_value = s_oracle[(string)key];

                if (!_IsEqual(expected_value, value))
                {
                    throw new Microsoft.Test.TestValidationException("Dictionary item with key '" + key + "' is not the expected value.");
                }
            }

            //
            // Remove
            //
            dictionary.Remove("");
            s_oracle.Remove("");

            _VerifyXmlnsDictionary(s_oracle, XmlAttributeProperties.GetXmlnsDictionary(root));

        }

        // Checks the entire contents of the actual dictionary against the oracle dictionary.
        private static void _VerifyXmlnsDictionary(Dictionary<string, string> _oracle, XmlnsDictionary dictionary)
        {
            int count = dictionary.Count;

            // Count property
            // Check that the dictionary's count is the same as the _oracle's count.
            if (count != _oracle.Count)
            {
                throw new Microsoft.Test.TestValidationException("Dictionary Count is not the expected value. Expected: " + _oracle.Count + ", Actual: " + count);
            }

            // get this[int] indexer
            // Contains
            // Check that each entry in the dictionary has the same value as the oracle's entries.
            foreach (string prefix in _oracle.Keys)
            {
                if (!dictionary.Contains(prefix))
                {
                    throw new Microsoft.Test.TestValidationException("Key (prefix) '" + prefix + "' doesn't exist in dictionary.");
                }

                object value = dictionary[prefix];
                object expected_value = _oracle[prefix];

                if (!_IsEqual(expected_value, value))
                {
                    throw new Microsoft.Test.TestValidationException("Dictionary item with key (prefix) '" + prefix + "' is not the expected value.");
                }

                _IsStringEqual(prefix, dictionary.LookupPrefix((string)expected_value));

                _IsStringEqual((string)expected_value, dictionary.LookupNamespace(prefix));
            }
        }

        // Checks if two values are the same.
        private static bool _IsEqual(object expectedValue, object actualValue)
        {
            bool same = false;

            if (actualValue == null)
            {
                same = expectedValue == null;
            }
            else
            {
                same = actualValue.Equals(expectedValue);
            }

            return same;
        }
        /// <summary>
        /// Verify basic XmlAttributeProperties behavior.
        /// </summary>
        public void VerifyXmlAttributeProperties()
        {
            UIElement root = (UIElement)SerializationHelper.ParseXamlFile(s_xamlFile);

            //
            // GetXmlSpace(DependencyObject dependencyObject), 
            // SetXmlSpace(DependencyObject dependencyObject, string value)
            //
            DependencyObject dobj = (DependencyObject)TreeHelper.FindNodeById(root, "xmlspace_default");
            _IsStringEqual("default", XmlAttributeProperties.GetXmlSpace(dobj));

            dobj = (DependencyObject)TreeHelper.FindNodeById(root, "xmlspace_explicitdefault");
            _IsStringEqual("default", XmlAttributeProperties.GetXmlSpace(dobj));

            dobj = (DependencyObject)TreeHelper.FindNodeById(root, "xmlspace_preserve");
            _IsStringEqual("preserve", XmlAttributeProperties.GetXmlSpace(dobj));

            XmlAttributeProperties.SetXmlSpace(dobj, "default");
            _IsStringEqual("default", XmlAttributeProperties.GetXmlSpace(dobj));

            //
            // GetXmlnsDictionary(DependencyObject dependencyObject)
            // SetXmlnsDictionary(DependencyObject dependencyObject, XmlnsDictionary value)
            //

            // Verify that dictionary entries exist from the loaded xaml.
            XmlnsDictionary xmldict = XmlAttributeProperties.GetXmlnsDictionary(dobj);
            if (xmldict.Count <= 0) throw new Microsoft.Test.TestValidationException("FAILED");

            // Set empty XmlnsDictionary on root - verify new value comes from child.
            xmldict = new XmlnsDictionary();
            XmlAttributeProperties.SetXmlnsDictionary(root, xmldict);

            xmldict = XmlAttributeProperties.GetXmlnsDictionary(dobj);

            if (xmldict.Count > 0) throw new Microsoft.Test.TestValidationException("FAILED");

            // 
            // GetXmlnsDefinition(DependencyObject dependencyObject)
            // SetXmlnsDefinition(DependencyObject dependencyObject, string value)
            // 
            // First, verify that value at child is correct.
            // Then, set new value on root and recheck value at child.
            // New value should inherit.
            //
            _IsStringEqual(s_oracle["x"], XmlAttributeProperties.GetXmlnsDefinition(dobj));
            XmlAttributeProperties.SetXmlnsDefinition(root, "foo");
            _IsStringEqual("foo", XmlAttributeProperties.GetXmlnsDefinition(dobj));

            //
            // GetXmlNamespaceMaps(DependencyObject dependencyObject)
            // SetXmlNamespaceMaps(DependencyObject dependencyObject, string value)
            //

            try
            {
                //Pass null to both dependencyObject and value in SetXmlNamespaceMaps, this will throw exception
                XmlAttributeProperties.SetXmlNamespaceMaps(null, null);

            }
            catch (Exception e)
            {
                GlobalLog.LogStatus("Exception is expected. Exception in SetXmlNamespaceMaps : " + e.ToString());
            }
            
            try
            {
                //Pass null to dependencyObject in GetXmlNamespaceMaps, this will throw exception
                XmlAttributeProperties.GetXmlNamespaceMaps(null);
            }
            catch (Exception e)
            {
                GlobalLog.LogStatus("Exception is expected. Exception in GetXmlNamespaceMaps : " + e.ToString());
            }

            //Pass value in SetXmlNamespaceMaps, then verify if the value is set correctly by GetXmlNamespaceMaps

            DependencyObject dObj = new DependencyObject();
            Hashtable hashTable = new Hashtable();
            var hashTableKey = "dummyKey";
            var hashTableValue = "dummyValue";
            hashTable.Add(hashTableKey, hashTableValue);
            XmlAttributeProperties.SetXmlNamespaceMaps(dObj, hashTable);
            var xmlNamespaceMapsValue = XmlAttributeProperties.GetXmlNamespaceMaps(dObj);

            if (xmlNamespaceMapsValue.GetType() == typeof(Hashtable))
            {
                foreach (DictionaryEntry entry in xmlNamespaceMapsValue)
                {
                    _IsStringEqual(entry.Key.ToString(), hashTableKey);
                    if (entry.Value != null)
                    {
                        _IsStringEqual(entry.Value.ToString(), hashTableValue);
                    }
                    else
                    {
                        throw new Microsoft.Test.TestValidationException("Value is unexpected. Expected:" + hashTableValue + ", Actual:null");
                    }
                }
            }
            else
            {
                throw new Microsoft.Test.TestValidationException("Value is unexpected. Expected:" + typeof(Hashtable) + ", Actual:" + xmlNamespaceMapsValue.GetType());
            }
        }

        // Checks that 2 string values are equivalent.
        private static void _IsStringEqual(string expected, string actual)
        {
            if (actual == null)
            {
                throw new Microsoft.Test.TestValidationException("Value is unexpected. Expected:" + expected + ", Actual:null");
            }

            if (!String.Equals(expected, actual, StringComparison.InvariantCulture))
            {
                throw new Microsoft.Test.TestValidationException("Value is unexpected. Expected:" + expected + ", Actual:" + actual);
            }
        }
    }
}
