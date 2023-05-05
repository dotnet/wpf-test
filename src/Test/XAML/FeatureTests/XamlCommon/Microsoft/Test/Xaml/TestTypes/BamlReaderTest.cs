// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Windows;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.TestTypes
{
    /******************************************************************************
    * CLASS:          BamlReaderTest
    ******************************************************************************/

    /// <summary>
    /// Class for comparing Xaml and Baml Infosets.
    /// </summary>
    public class BamlReaderTest : BamlTestType
    {
        #region Public and Private Data

        /// <summary>
        /// Compare Baml Infoset against a master file.
        /// </summary>
        private bool _useMasterInfoset = false;

        /// <summary>
        /// If true, instructs the verification code to add a 'ConnectionId' group to the Xaml Infoset
        /// in order to compensate for its presence in the Baml Infoset.  It is needed in non-BRAT mode
        /// to connect Names and events.
        /// </summary>
        private bool _addConnectionId = true;

        /// <summary>
        /// Gets or sets a value indicating whether a fast or slow ("BRAT") Baml2006Reader is verified.
        /// </summary>
        public virtual bool ValuesMustBeString { get; set; }

        #endregion

        #region Public and Protected Members

        /******************************************************************************
        * Function:          Run
        ******************************************************************************/

        /// <summary>
        /// Runs a BamlReaderTest
        /// Compares the Xaml-based InfoSet to the Baml-based InfoSet.
        /// The .xaml file specified in a .xtc file is dynamically compiled.  The resulting .baml file is written
        /// to disk. XamlServices.Transform() is used to create an Infoset string from the baml, which is then verified
        /// by comparing it to the Infoset string produced from the original .xaml. 
        /// </summary>
        public override void Run()
        {
            try
            {
                // The base class will take care of compiling the Xaml file.
                base.Run();

                GlobalLog.LogStatus("----------- Starting BamlReaderTest -----------");

                // Ensures that the test starts on a clean slate
                LocalTestResult = TestResult.Unknown;

                Initialize();

                bool testPassed = ValidateBamlInfoset(_useMasterInfoset, XamlFileName, BamlFileName, ValuesMustBeString);

                if (testPassed)
                {
                    GlobalLog.LogEvidence("PASS: BamlReaderTest sucessful.");
                    LocalTestResult = TestResult.Pass;
                }
                else
                {
                    // Throwing an exception so that any subclasses that carry out additional testing will not be executed.
                    throw new TestValidationException("FAIL: BamlReaderTest failed.");
                }
            }
            finally
            {
                if (this.GetType().Equals(typeof(BamlReaderTest)))
                {
                    CleanUp();
                }
            }
        }

        #endregion

        #region Private Members

        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/

        /// <summary>
        /// Initialization: read test parameters from an .xtc file; set up test environment.
        /// Check that the test parameters are allowed.
        /// </summary>
        private void Initialize()
        {
            // This causes the WPF assemblies to load, this is a hack that will be removed later.
            FrameworkElement frameworkElement = new FrameworkElement();
            frameworkElement = null;

            // SET TEST PARAMETER: ValuesMustBeString.
            // When valuesMustBeString is true, the Infoset created is a BRAT ("Baml Reader as Text") version
            // which has few optimizations.  Therefore, it is close to the Infoset created from Xaml.
            string mustBeString = DriverState.DriverParameters["ValuesMustBeString"];
            if (string.IsNullOrEmpty(mustBeString))
            {
                throw new TestSetupException("ValuesMustBeString cannot be null");
            }
            else
            {
                ValuesMustBeString = bool.Parse(mustBeString);
            }

            // SET TEST PARAMETER: useMasterInfoset.
            string useMasterInfosetParam = DriverState.DriverParameters["UseMasterInfoset"];
            if (string.IsNullOrEmpty(useMasterInfosetParam))
            {
                _useMasterInfoset = false;
            }
            else
            {
                _useMasterInfoset = bool.Parse(useMasterInfosetParam);
            }

            // SET TEST PARAMETER: addConnectionId.
            // A special instruction for test case validation that may be present in the .xtc for some tests.
            string connectionIdParameter = DriverState.DriverParameters["AddConnectionId"];
            if (!string.IsNullOrEmpty(connectionIdParameter))
            {
                _addConnectionId = Convert.ToBoolean(connectionIdParameter);
            }

            GlobalLog.LogStatus("DisplayName: " + DriverState.TestName);
        }

        /******************************************************************************
        * Function:          ValidateBamlInfoset
        ******************************************************************************/

        /// <summary>
        /// Verify that the Baml Infoset is correct.
        /// </summary>
        /// <param name="useMasterInfoset">Indicates the kind of validation to be used.</param>
        /// <param name="xamlFileName">The name of the .xaml file to be tested.</param>
        /// <param name="bamlFileName">Temporary .baml file name.</param>
        /// <param name="valuesMustBeString">Indicates the value of XamlReaderSettings.ValuesMustBeString.</param>
        /// <returns>A boolean indicating whether or not the Infoset comparision was correct</returns>
        private bool ValidateBamlInfoset(bool useMasterInfoset, string xamlFileName, string bamlFileName, bool valuesMustBeString)
        {
            bool testPassed = false;

            // Compare the .xaml and .baml InfoSets.
            if (useMasterInfoset)
            {
                string masterInfosetFile = Path.GetFileNameWithoutExtension(xamlFileName) + ".DiagBaml";

                string diagBamlOutput = InfosetFactory.GenerateInfosetFromBaml(bamlFileName, valuesMustBeString);

                // Verify string exists.
                if (String.IsNullOrEmpty(diagBamlOutput))
                {
                    GlobalLog.LogEvidence("No .DiagBaml returned, test failed.");
                    return false;
                }

                testPassed = BamlInfosetVerifier.CompareInfosetToMaster(diagBamlOutput, XamlFileName, masterInfosetFile);
            }
            else
            {
                GlobalLog.LogStatus("NOTE: 'INFOSET1' is the Xaml Infoset. 'INFOSET2' is the Baml Infoset.");

                // Copy the diag result into an ArrayList.
                ArrayList xamlInfoSet = CreateXamlList(xamlFileName);
                ArrayList bamlInfoSet = CreateBamlList(bamlFileName, valuesMustBeString);

                testPassed = BamlInfosetVerifier.CompareInfosets(xamlInfoSet, bamlInfoSet, valuesMustBeString, _addConnectionId);

                GlobalLog.LogFile(xamlFileName);
            }

            string localeSpecific = DriverState.DriverParameters["LocaleSpecific"];
            bool isLocaleSpecific = false;
            if (string.IsNullOrEmpty(localeSpecific) == false)
            {
                isLocaleSpecific = bool.Parse(localeSpecific);
            }

            if (isLocaleSpecific)
            {
                CultureInfo[] exceptionCultures = 
                                                  {  
                                                      CultureInfo.GetCultureInfo("ar-SA"),
                                                      CultureInfo.GetCultureInfo("en-US"),
                                                      CultureInfo.GetCultureInfo("ja-JP")
                                                  };

                if (LangUtils.IsSystemLanguage(exceptionCultures) == false)
                {
                    GlobalLog.LogStatus("The default master file shouldnt match as in English we get [V 0.8,0.2 0,0.4 0.6,0.8 0.8,0.2 ]" +
                        " and in other Localized OSs we get [V 0,8;0,2 0;0,4 0,6;0,8 0,8;0,2]. If the files match the test should fail");

                    testPassed = true;
                }
            }

            return testPassed;
        }

        /******************************************************************************
        * Function:          CreateXamlList
        ******************************************************************************/

        /// <summary>
        /// Uses a XamlXmlReader to read a .xaml file and create an node stream (Infoset),
        /// whch is then copied to an ArrayList.
        /// </summary>
        /// <param name="xamlFileName">The name of the .xaml file being tested.</param>
        /// <returns>An ArrayList containing each line of the .xaml-based Infoset</returns>
        private ArrayList CreateXamlList(string xamlFileName)
        {
            string diagOutputXaml = InfosetFactory.GenerateInfosetFromXaml(xamlFileName);

            return CopyDiagToArray(diagOutputXaml);
        }

        /******************************************************************************
        * Function:          CreateBamlList
        ******************************************************************************/

        /// <summary>
        /// Uses a Baml2006Reader to read a .baml file and create an node stream (Infoset),
        /// which is then copied to an ArrayList.
        /// </summary>
        /// <param name="tempBamlFileName">The name of the .baml that was dynamically created.</param>
        /// <param name="valuesMustBeString">Indicates setting of XamlReaderSettings.ValuesMustBeString.</param>
        /// <returns>An ArrayList containing each line of the .baml-based Infoset</returns>
        private ArrayList CreateBamlList(string tempBamlFileName, bool valuesMustBeString)
        {
            string diagOutputBaml = InfosetFactory.GenerateInfosetFromBaml(tempBamlFileName, valuesMustBeString);

            return CopyDiagToArray(diagOutputBaml);
        }

        /******************************************************************************
        * Function:          CopyDiagToArray
        ******************************************************************************/

        /// <summary>
        /// Copies each line of the Infoset string to an ArrayList.
        /// </summary>
        /// <param name="diag">A string containing the Infoset.</param>
        /// <returns>An ArrayList containing each line of the Infoset</returns>
        private ArrayList CopyDiagToArray(string diag)
        {
            ArrayList arrayList = new ArrayList();
            StringReader reader = new StringReader(diag);
            string line = string.Empty;

            while (line != null)
            {
                line = reader.ReadLine();
                arrayList.Add(line); // Copy InfoSet into an ArrayList.
            }

            return arrayList;
        }

        #endregion
    }
}
