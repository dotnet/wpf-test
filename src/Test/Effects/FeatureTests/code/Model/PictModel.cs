// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Provide a test suite by reading from pict file, generating test case variations by combination
 *          with Pict. 
 *          Usage: Inherit PictModel and implement RunVariationCollection method which do what ever you want each variation to do. 
 *                 In an xtc file, specify ModelFileName, and optionally FirstVariationIndex & LastVariationIndex.
 ********************************************************************/
using System;
using System.Windows;
using Microsoft.Test.Logging;
using Microsoft.Test.Modeling;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Discovery;
using System.Xml;
using System.Collections.Generic;
using Microsoft.Test.Markup;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Pict.MatrixOutput;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// Model base tests. Generate test case variations with PICT and 
    /// route each variation to RunTestVariations.
    /// </summary>
    public abstract class PictModel
    {
        #region Data

        private string _modelFileName = string.Empty;
        //sequence of the test.
        private int _variationIndex = 0;

        protected TestLog log;
        #endregion

        #region property

        /// <summary>
        /// Log that will be used in the test.
        /// </summary>  
        protected TestLog Log
        {
            get { return log; }
        }

        protected string ModelFileName
        {
            get { return _modelFileName; }
        }

        protected int VariationIndex
        {
            get { return _variationIndex; }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Entry point, create a Model and start to run. 
        /// </summary>
        public void LoadModel()
        {
            this.RunTestVariations();
        }

        /// <summary>
        /// For each test, create variations from a model file and call RunVariationCollection. 
        /// </summary>
        private void RunTestVariations()
        {
            // read ModelFileName from current property bag.
            PropertyBag modelParameters = DriverState.DriverParameters;
            if (!modelParameters.ContainsProperty("ModelFileName"))
            {
                GlobalLog.LogStatus("Need ModelFileName specified for Effects Model based tests.");
                return;
            }

            _modelFileName = modelParameters["ModelFileName"];

            //Generate test case with PICT
            string pictArgs = string.Empty;
            if (modelParameters.ContainsProperty("PictArgs"))
            {
                pictArgs = modelParameters["PictArgs"];
            }

            PairwiseTestCase[] variations = PairwiseHelper.GenerateTestsFromFile(_modelFileName, pictArgs);

            //Determinate range of test case to run
            int FirstVariationIndex = 0;
            int LastVariationIndex = variations.Length - 1;
            int MaxNumberOfVariationsInOnePass = 1;

            if (modelParameters.ContainsProperty("FirstVariationIndex"))
            {
                int.TryParse(modelParameters["FirstVariationIndex"], out FirstVariationIndex);
            }
            if (modelParameters.ContainsProperty("LastVariationIndex"))
            {
                int.TryParse(modelParameters["LastVariationIndex"], out LastVariationIndex);
            }
            if (modelParameters.ContainsProperty("MaxNumberOfVariationsInOnePass"))
            {
                int.TryParse(modelParameters["MaxNumberOfVariationsInOnePass"], out MaxNumberOfVariationsInOnePass);
            }

            GlobalLog.LogStatus(string.Format("Running variations from {0} to {1}.", FirstVariationIndex, LastVariationIndex));

            //For each test case, create a log and run test case. In case of an exception, mark test case as fail. 
            for (_variationIndex = FirstVariationIndex; _variationIndex <= LastVariationIndex; _variationIndex += MaxNumberOfVariationsInOnePass)
            {
                GlobalLog.LogStatus("Starting variations begin at index: #{0}...", _variationIndex);

                using (log = new TestLog(variations[_variationIndex].Signature))
                {
                    try
                    {
                        int lastTest = Math.Min(LastVariationIndex, _variationIndex + MaxNumberOfVariationsInOnePass - 1);
                        LogVariationCollection(variations, _variationIndex, lastTest);
                        RunVariationCollection(variations, _variationIndex, lastTest);

                        //No exception, mark test as pass.
                        log.Result = TestResult.Pass;
                    }
                    catch (Exception e)
                    {
                        log.LogEvidence(string.Format("Got an exception: \n{0}", e.ToString()));
                        OnTestFailed();
                        log.Result = TestResult.Fail;
                    }
                }
            }
        }

        /// <summary>
        /// This method just logs the test information. 
        /// </summary>
        /// <param name="test">Test information array</param>
        /// <param name="firstVariationIndex">Starting index in the array</param>
        /// <param name="lastVariationIndex">Ending index in the array</param>
        private void LogVariationCollection(PairwiseTestCase[] variations, int firstVariationIndex, int lastVariationIndex)
        {
            log.LogStatus("Begin RunTest, parameters:");
            for (int i = firstVariationIndex; i <= lastVariationIndex; i++)
            {
                log.LogStatus(string.Format("Test: {0}.", i.ToString()));

                foreach (PairwiseTestParameter parameter in variations[i].Parameters)
                {
                    log.LogStatus("\t" + parameter.Name + " : " + parameter.Value);
                }
            }
        }

        public abstract void RunVariationCollection(PairwiseTestCase[] variations, int firstVariationIndex, int lastVariationIndex);

        public abstract void OnTestFailed();

        #endregion
    }
}

