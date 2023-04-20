// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Baml2006;
using System.Xaml;
using System.Xml;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.TestTypes
{
    /******************************************************************************
    * CLASS:          BamlToXamlTest
    ******************************************************************************/

    /// <summary>
    /// Class for verifying Xaml produced by XamlXmlWriter using Baml2006Reader.
    /// </summary>
    public class BamlToXamlTest : BamlTestType
    {
        #region Public and Protected Members

        /******************************************************************************
        * Function:          Run
        ******************************************************************************/

        /// <summary>
        /// Runs a BamlToXamlTest
        /// Compares the Xaml-based InfoSet to the Baml-based InfoSet.
        /// The .xaml file specified in a .xtc file is dynamically compiled.  The resulting .baml file is written
        /// to disk. XamlServices.Transform() is used to create an Xaml string from the baml, which is then verified
        /// by comparing it to the Infoset string produced from the original .xaml. 
        /// </summary>
        public override void Run()
        {
            try
            {
                bool testPassed1 = false;
                bool testPassed2 = false;

                // The base class will take care of compiling the Xaml file.
                base.Run();

                GlobalLog.LogStatus("----------- Starting BamlToXamlTest -----------");

                // Ensures that the test starts on a clean slate
                LocalTestResult = TestResult.Unknown;

                // This causes the WPF assemblies to load, this is a hack that will be removed later.
                FrameworkElement frameworkElement = new FrameworkElement();
                frameworkElement = null;

                // Generate the Xaml file.
                string xamlOutput = XamlFromBamlFactory.GenerateXamlFromBaml(BamlFileName);

                string masterXamlFileName = Path.GetFileNameWithoutExtension(XamlFileName) + ".MasterXaml";

                // First, validate the generated Xaml file.
                testPassed1 = ValidateXamlToBaml(xamlOutput, XamlFileName, masterXamlFileName);

                // Next, verify that the generated Xaml file can be loaded.
                testPassed2 = VerifyLoad(xamlOutput);

                if (testPassed1 && testPassed2)
                {
                    GlobalLog.LogEvidence("PASS: BamlToXamlTest sucessful.");
                    LocalTestResult = TestResult.Pass;
                }
                else
                {
                    throw new TestValidationException("FAIL: BamlToXamlTest failed.");
                }
            }
            finally
            {
                if (this.GetType().Equals(typeof(BamlToXamlTest)))
                {
                    CleanUp();
                }
            }
        }

        #endregion

        #region Private Members

        /******************************************************************************
        * Function:          ValidateXamlToBaml
        ******************************************************************************/

        /// <summary>
        /// Verify that the Xaml produced from the Baml2006Reader is correct by comparing it to a Master file.
        /// </summary>
        /// <param name="xamlString">The generated xaml to be tested.</param>
        /// <param name="xamlFileName">The name of the .xaml file to be tested.</param>
        /// <param name="masterXamlFileName">The name of the master file to be compared to the xamlString.</param>
        /// <returns>A boolean indicating whether or not the Xaml comparison was correct</returns>
        private bool ValidateXamlToBaml(string xamlString, string xamlFileName, string masterXamlFileName)
        {
            bool testPassed = BamlInfosetVerifier.CompareInfosetToMaster(xamlString, XamlFileName, masterXamlFileName);

            return testPassed;
        }

        /******************************************************************************
        * Function:          VerifyLoad
        ******************************************************************************/

        /// <summary>
        /// Verify that the Xaml produced from the Baml2006Reader is loaded. 
        /// We use XamlReader.Load instead of XamlServices.Load since XamlReader is 
        /// the recommented Api to use for WPF types. Since the Baml tests are mainly
        /// testing wpf baml, we need to use the XamlReader Api.
        /// </summary>
        /// <param name="xamlString">The generated xaml to be tested.</param>
        /// <returns>A boolean indicating whether or not the Xaml successfully loaded.</returns>
        private bool VerifyLoad(string xamlString)
        {
            object root = null;
            using (XmlReader xmlReader = XmlReader.Create(new StringReader(xamlString)))
            {
                root = System.Windows.Markup.XamlReader.Load(xmlReader);
            }

            if (root == null)
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
