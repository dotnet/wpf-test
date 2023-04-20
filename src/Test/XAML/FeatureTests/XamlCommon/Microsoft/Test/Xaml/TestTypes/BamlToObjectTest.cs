// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.Test.Logging;
using Microsoft.Test.Markup;
using Microsoft.Test.Windows;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.TestTypes
{
    /******************************************************************************
    * CLASS:          BamlToObjectTest
    ******************************************************************************/

    /// <summary>
    /// Class for verifying the object tree produced from Xaml matches the object tree
    /// produced from Baml (using Baml2006Reader).
    /// </summary>
    public class BamlToObjectTest : BamlReaderTest
    {
        #region Public and Protected Members

        /******************************************************************************
        * Function:          Run
        ******************************************************************************/

        /// <summary>
        /// Runs a BamlToObjectTest
        /// Compares the object tree from Xaml to the object tree from Baml.
        /// The .xaml file specified in a .xtc file is dynamically compiled.  The resulting .baml file is written
        /// to disk. 
        /// </summary>
        public override void Run()
        {
            try
            {
                bool testPassed = false;

                // The base class will take care of compiling the Xaml file.
                base.Run();

                GlobalLog.LogStatus("----------- Starting BamlToObjectTest -----------");

                // Ensures that the test starts on a clean slate
                LocalTestResult = TestResult.Unknown;

                // Generate the Xaml-based, baml-based object trees.
                object xamlObjectTree = null;
                object bamlObjectTree = null;
                
                // Choose v3 XamlReader or v4 XamlServices based on the XamlParser parameter
                string parser = DriverState.DriverParameters["XamlParser"];
                bool useXamlServices = string.IsNullOrEmpty(parser) || !parser.Equals("WPF");
                if (useXamlServices)
                {
                    // use XamlServices.Parse
                    xamlObjectTree = XamlBamlObjectFactory.GenerateObjectFromXaml(XamlFileName);
                    bamlObjectTree = XamlBamlObjectFactory.GenerateObjectFromBaml(BamlFileName, ValuesMustBeString);
                }
                else
                {
                    // use v3 XamlReader.Load which internally uses XamlServices
                    xamlObjectTree = XamlBamlObjectFactory.GenerateObjectFromWpfXaml(XamlFileName);
                    bamlObjectTree = XamlBamlObjectFactory.GenerateObjectFromWpfBaml(BamlFileName, ValuesMustBeString);
                }

                testPassed = ValidateBamlToObject(xamlObjectTree, bamlObjectTree);

                if (testPassed)
                {
                    GlobalLog.LogEvidence("PASS: BamlToObjectTest sucessful.");
                    LocalTestResult = TestResult.Pass;
                }
                else
                {
                    // Throwing an exception so that any subclasses that carry out additional testing will not be executed.
                    throw new TestValidationException("FAIL: BamlToObjectTest failed.");
                }
            }
            finally
            {
                if (this.GetType().Equals(typeof(BamlToObjectTest)))
                {
                    CleanUp();
                }
            }
        }

        #endregion

        #region Private Members

        /******************************************************************************
        * Function:          ValidateBamlToObject
        ******************************************************************************/

        /// <summary>
        /// Compare a xaml-based object tree to a baml-based object treee.
        /// </summary>
        /// <param name="xamlObjectTree">An object tree generated from xaml.</param>
        /// <param name="bamlObjectTree">An object tree generated from baml.</param>
        /// <returns>Pass or fail.</returns>
        private bool ValidateBamlToObject(object xamlObjectTree, object bamlObjectTree)
        {
            Dictionary<string, PropertyToIgnore> ignores = new Dictionary<string, PropertyToIgnore>();

            // Since file name is different for baml and xaml, ignore the base uri property 
            PropertyToIgnore ignoreProperty = new PropertyToIgnore()
                                                    {
                                                        WhatToIgnore = IgnoreProperty.IgnoreNameAndValue
                                                    };
            ignores.Add("BaseUriHelper.BaseUri", ignoreProperty);
            ignores.Add("BaseUri", ignoreProperty);

            TreeCompareResult treeCompareResult = TreeComparer.CompareLogical(xamlObjectTree, bamlObjectTree, ignores);

            if (treeCompareResult.Result == CompareResult.Different)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion
    }
}
