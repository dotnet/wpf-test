// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Avalon.Test.CoreUI.Resources.RefreshResourceDictionaryTest
{
    /******************************************************************************
    * CLASS:          ResourceDictionaryTest
    ******************************************************************************/
    /// <summary>
    /// ResourceDictionary class used to be very simple.
    /// More APIs are added over the time so more coverage is needed
    /// </summary>
    [Test(0, "Resources.ResourceDictionaryTest", TestCaseSecurityLevel.FullTrust, "ResourceDictionaryTest")]
    public class ResourceDictionaryTest : TestCase
    {
        #region Private Data
        private string _testName = "";
        #endregion


        #region Constructor

        [Variation("TestMisc")]
        [Variation("TestResourceDictionaryBasic")]

        /******************************************************************************
        * Function:          ResourceDictionaryTest Constructor
        ******************************************************************************/
        public ResourceDictionaryTest(string arg)
        {
            _testName = arg;
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        TestResult StartTest()
        {
            switch (_testName)
            {
                case "TestMisc":
                    TestMisc();
                    break;
                case "TestResourceDictionaryBasic":
                    TestResourceDictionaryBasic();
                    break;
                default:
                    throw new Microsoft.Test.TestValidationException("ERROR!!! Test case not found.");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Public Members
        /******************************************************************************
        * Function:          TestMisc
        ******************************************************************************/
        /// <summary>
        /// Type check
        /// </summary>
        public void TestMisc()
        {
            GlobalLog.LogStatus("Type Check for ResourceDictionary");
            Type testType = typeof(ResourceDictionary);
            GlobalLog.LogStatus("Base class is Object");
            if (testType.BaseType != typeof(System.Object))
            {
                throw new Microsoft.Test.TestValidationException("FAIL: Base class failed to return Object.");
            }

            Type iDictionaryType = typeof(System.Collections.IDictionary);
            Type iSupportInitType = typeof(System.ComponentModel.ISupportInitialize);
            Type iNameScopeType = typeof(System.Windows.Markup.INameScope);
            GlobalLog.LogStatus("Check IDictionary interfaces (IDictionary inherits from IEnumerable and ICollection)");
            if (!iDictionaryType.IsAssignableFrom(testType))
            {
                throw new Microsoft.Test.TestValidationException("FAIL: iDictionaryType.IsAssignableFrom returned false.");
            }

            GlobalLog.LogStatus("Check ISupportInitialize interfaces");
            if (!iSupportInitType.IsAssignableFrom(testType))
            {
                throw new Microsoft.Test.TestValidationException("FAIL: iSupportInitType.IsAssignableFrom returned false.");
            }

            GlobalLog.LogStatus("Check INameScope interfaces");
            if (!iNameScopeType.IsAssignableFrom(testType))
            {
                throw new Microsoft.Test.TestValidationException("FAIL: iNameScopeType.IsAssignableFrom returned false.");
            }
        }

        /******************************************************************************
        * Function:          TestResourceDictionaryBasic
        ******************************************************************************/
        /// <summary>
        /// MSDotnetAvalon.Windows.ResourceDictionary is derived from HashTable
        /// So it is rather simple class indeed
        /// This test is on ResourceDictionary itself. 
        /// </summary>
        public void TestResourceDictionaryBasic()
        {
            GlobalLog.LogStatus("ResourceDictionary Basic Test");

            ResourceDictionary resourceDictionary = new ResourceDictionary(); //Only one Ctor, which is provided by compiler

            GlobalLog.LogStatus("Test indexer getter when key does not exist");
            object result = resourceDictionary["NotExist"];
            if (result == null)
                GlobalLog.LogStatus("");
            GlobalLog.LogStatus("Test indexer setter when key does not originally exist");
            resourceDictionary["NotExist"] = System.Windows.Media.Brushes.Red;
            GlobalLog.LogStatus("Test indexer getter when key already exists");
            if (resourceDictionary["NotExist"] == System.Windows.Media.Brushes.Red)
                GlobalLog.LogStatus("");
            GlobalLog.LogStatus("Test indexer setter when key already exists");
            resourceDictionary["NotExist"] = System.Windows.Media.Colors.Ivory;
            GlobalLog.LogStatus("Use Add(key, value)");
            resourceDictionary.Add("1", System.Windows.Input.Cursors.Hand);
            GlobalLog.LogStatus("Add same key again");
            try
            {
                resourceDictionary.Add("1", System.Windows.Input.Cursors.Hand);
                throw new Microsoft.Test.TestValidationException("Expected Exception not received.");
            }
            catch (ArgumentException ex)
            {
                GlobalLog.LogStatus("Expected Exception received: " + ex);
            }
            GlobalLog.LogStatus("Use Remove(key)");
            resourceDictionary.Remove("1");
            GlobalLog.LogStatus("Remove same key again");
            resourceDictionary.Remove("1");
            GlobalLog.LogStatus("Remove a key that does not exist");
            resourceDictionary.Remove(typeof(Button));
            GlobalLog.LogStatus("Add/Remove with null as Key");
            try
            {
                resourceDictionary.Add(null, "1");
                throw new Microsoft.Test.TestValidationException("Expected Exception not received.");
            }
            catch (ArgumentNullException ex)
            {
                GlobalLog.LogStatus("Expected Exception received: " + ex);
            }
            try
            {
                resourceDictionary.Remove(null);
                throw new Microsoft.Test.TestValidationException("Expected Exception not received.");
            }
            catch (ArgumentNullException ex)
            {
                GlobalLog.LogStatus("Expected Exception received: " + ex);
            }
            GlobalLog.LogStatus("Test Values");
            resourceDictionary.Add("3", System.Windows.MessageBoxImage.Information);
            resourceDictionary.Add("2", System.Windows.Media.Brushes.Violet);
            System.Collections.ICollection values = resourceDictionary.Values;
            if (values.Count == 3)
                GlobalLog.LogStatus("Values.Count verified");
            foreach (object o in values)
            {
                GlobalLog.LogStatus(o.ToString());
            }
            GlobalLog.LogStatus("Test GetEnumerator");
            System.Collections.IDictionaryEnumerator de = resourceDictionary.GetEnumerator();
            de.Reset();
            int count = 0;
            while (de.MoveNext())
            {
                count++;
            }
            if (count == 3)
                GlobalLog.LogStatus("GetEnumerator Reset and MoveNext works");
            GlobalLog.LogStatus("Test CopyTo(array, arrayIndex) when Array is null");
            try
            {
                resourceDictionary.CopyTo(null, 2);
                throw new Microsoft.Test.TestValidationException("Expected Exception not received.");
            }
            catch (ArgumentNullException ex)
            {
                GlobalLog.LogStatus("Expected Exception received: " + ex);
            }
            GlobalLog.LogStatus("Test CopyTo(array, arrayIndex) when arrayIndex is negative");
            try
            {
                DictionaryEntry[] array = new DictionaryEntry[1];
                resourceDictionary.CopyTo(array, -1);
                throw new Microsoft.Test.TestValidationException("Expected Exception not received.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                GlobalLog.LogStatus("Expected Exception received: " + ex);
            }
            GlobalLog.LogStatus("Test CopyTo(array, arrayIndex) positive test case");
            DictionaryEntry[] array1 = new DictionaryEntry[3];
            resourceDictionary.CopyTo(array1, 0);
            GlobalLog.LogStatus(array1[2].ToString());
        }
        #endregion
    }
}

