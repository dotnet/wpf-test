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
    * CLASS:          BamlWriterTest
    ******************************************************************************/

    /// <summary>
    /// Class for disassembling Baml and using the resulting BamlDasm to validate the Baml2006Writer.
    /// NOTES:
    ///    Sequence of logic to produce Baml from Xaml:
    ///         1. Generate dynamically Baml from Xaml.
    ///         2. Call BamlDasm utility to produce BamlDasm from Baml.
    ///    Sequence of logic to produce Baml via the Baml2006Writer:
    ///         1. Generate dynamically Baml from Xaml.
    ///         2. Call BamlReader to produce node stream file from Baml.
    ///         3. Call BamlWriter to produce Baml from node stream.
    ///         4. Call BamlDasm utility to produce BamlDasm from Baml.
    ///            (The BamlDasm must be altered to remove variable data, e.g., strip out version numbers.)
    ///    Final step:  compare the BamlDasm strings from the two sources.
    /// </summary>
    public class BamlWriterTest : BamlTestType
    {
        #region Public and Protected Members

        /******************************************************************************
        * Function:          Run
        ******************************************************************************/

        /// <summary>
        /// Runs a BamlWriterTest.
        /// Compares the Xaml-based InfoSet to the Baml-based InfoSet.
        /// The .xaml file specified in a .xtc file is dynamically compiled.  The resulting .baml file is written
        /// to disk. XamlServices.Transform() is used to create an BamlDasm string from the baml, which is then verified
        /// by comparing it to the BamlDasm string produced from the original .xaml. 
        /// </summary>
        public override void Run()
        {
            string bamlFileName2 = string.Empty;
            string bamlDasm1 = string.Empty;
            string bamlDasm2 = string.Empty;

            try
            {
                // 1. In the base class, generate dynamically Baml from Xaml.  Result: BamlFileName.
                base.Run();

                // Ensures that the test starts on a clean slate
                LocalTestResult = TestResult.Unknown;

                Initialize();

                // 2. Call the BamlDasm utility to produce a BamlDasm string from the Xaml-based Baml.
                bamlDasm1 = BamlDasmHelper.DisassembleBamlFile(BamlFileName);

                // 3. Create a second .baml file by invoking BamlReader to produce an Infoset from the Baml, then
                //    invoking BamlWriter to produce Baml from the Infoset.  Result: bamlFileName2.
                bamlFileName2 = GetBamlFromBamlWriter(BamlFileName);

                // 4. Call the BamlDasm utility to produce a BamlDasm string from the BamlWriter-based Baml.
                bamlDasm2 = BamlDasmHelper.DisassembleBamlFile(bamlFileName2);

                bool testPassed = CompareBamlDasmStrings(bamlDasm1, bamlDasm2);

                if (testPassed)
                {
                    GlobalLog.LogEvidence("PASS: BamlWriterTest sucessful.");
                    LocalTestResult = TestResult.Pass;
                }
                else
                {
                    WriteResultsToFiles(bamlDasm1, bamlDasm2, XamlFileName);

                    // Throwing an exception so that any subclasses that carry out additional testing will not be executed.
                    throw new TestValidationException("FAIL: BamlWriterTest failed.");
                }
            }
            finally
            {
                if (this.GetType().Equals(typeof(BamlWriterTest)))
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
            GlobalLog.LogStatus("----------- Starting BamlWriterTest -----------");

            // This causes the WPF assemblies to load, this is a hack that will be removed later.
            FrameworkElement frameworkElement = new FrameworkElement();
            frameworkElement = null;

            GlobalLog.LogStatus("DisplayName: " + DriverState.TestName);
        }

        /******************************************************************************
        * Function:          GetBamlFromBamlWriter
        ******************************************************************************/

        /// <summary>
        /// Create a .baml file via a round trip: orginal .baml to Infoset back to .baml via BamlReader and
        /// BamlWriter, respectively.
        /// </summary>
        /// <param name="bamlFileNameIn">Name of the .baml file created by compiling the original .xaml.</param>
        /// <returns>The name of the .bamldasm file that was created.</returns>
        private string GetBamlFromBamlWriter(string bamlFileNameIn)
        {
            // 1. Create an Infoset from the .baml file via Baml2006Reader with ValuesMustBeString set to true.
            string diagBamlOutput = InfosetFactory.GenerateInfosetFromBaml(bamlFileNameIn, true);

            // Verify string exists.
            if (String.IsNullOrEmpty(diagBamlOutput))
            {
                GlobalLog.LogEvidence("No DiagBaml returned, BamlWriterTest failed.");
                throw new TestValidationException("FAIL: BamlWriterTest failed.");
            }

            // 2. Create a second baml file based on the Baml2006Writer.
            string bamlFileNameOut = BamlFactory.GenerateBamlFromInfoset(diagBamlOutput);

            // Verify string exists.
            if (String.IsNullOrEmpty(bamlFileNameOut))
            {
                GlobalLog.LogEvidence("No .baml file name returned from call to Baml2006Writer, BamlWriterTest failed.");
                throw new TestValidationException("FAIL: BamlWriterTest failed.");
            }

            return bamlFileNameOut;
        }

        /******************************************************************************
        * Function:          CompareBamlDasmStrings
        ******************************************************************************/

        /// <summary>
        /// Compare two BamlDasm strings.  Also, write results to file.
        /// </summary>
        /// <param name="bamlDasm1">The first BamlDasm string.</param>
        /// <param name="bamlDasm2">The second BamlDasm string.</param>
        /// <returns>A boolean indicating whether or not the two BamlDasm strings matched.</returns>
        private bool CompareBamlDasmStrings(string bamlDasm1, string bamlDasm2)
        {
            //// TO-DO: consider a line-by-line comparison, so that the point of failure can be reported.
            bool stringMatched = false;

            // Compare the the two BamlDasm outputs.
            if (bamlDasm2.Equals(bamlDasm1))
            {
                GlobalLog.LogStatus("Strings matched, test sucessful");
                stringMatched = true;
            }

            return stringMatched;
        }

        /******************************************************************************
        * Function:          WriteResultsToFiles
        ******************************************************************************/

        /// <summary>
        /// Write the two bamldasm result string to files, along with the original .xaml, for debugging purposes.
        /// </summary>
        /// <param name="bamlDasm1">The first BamlDasm string.</param>
        /// <param name="bamlDasm2">The second BamlDasm string.</param>
        /// <param name="xamlFileName">The name of the .xaml file being tested.</param>
        private void WriteResultsToFiles(string bamlDasm1, string bamlDasm2, string xamlFileName)
        {
            GlobalLog.LogEvidence("The Xaml-based BamlDasm failed to match the BamlWriter-based BamlDasm.\nGo to the 'Logs location' shown below to compare the BamlDasm files:\n\tExpected:  " + DriverState.TestName + ".bamldasm\n\tActual:    DiagOutput.bamldasm\n\tOriginal Markup:  " + DriverState.TestName + ".xaml");
            GlobalLog.LogStatus("Writing first bamlDasm output to disk...");
            StreamWriter writer1 = new StreamWriter("XamlBased.bamldasm");
            writer1.Write(bamlDasm1);
            writer1.Flush();
            writer1.Close();

            GlobalLog.LogStatus("Writing second bamlDasm output to disk...");
            StreamWriter writer2 = new StreamWriter("BamlWriterBased.bamldasm");
            writer2.Write(bamlDasm2);
            writer2.Flush();
            writer2.Close();

            GlobalLog.LogStatus("Logging files...");
            GlobalLog.LogFile(xamlFileName);
            GlobalLog.LogFile("XamlBased.bamldasm");
            GlobalLog.LogFile("BamlWriterBased.bamldasm");
            TestLog.Current.Result = TestResult.Fail;
        }

        #endregion
    }
}
