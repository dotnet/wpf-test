// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: ProcessRenderMode testing
 * simple simple test that ensures correct default value
 ********************************************************************/
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Graphics
{
    [Test(0, "ProcessRenderModeDefaultValue", "ProcessRenderModeDefaultValue", 
        Area = "2D",
        Description = "Verification for ProcessRenderMode Default Value")]
    public class ProcessRenderModeDefaultValueTest : StepsTest
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ProcessRenderModeDefaultValueTest()
        {
            RunSteps += new TestStep(CheckDefaultValue);
        }

        #region test step

        //make sure that the default value is correct
        private TestResult CheckDefaultValue()
        {
            RenderMode defaultValue = RenderOptions.ProcessRenderMode;
            if(defaultValue == RenderMode.Default) //admittedly not a very exciting test
            {
                return TestResult.Pass;
            }
            else
            {
                Log.LogEvidence("Incorrect default value! Expected RenderMode.Default, got {0}", defaultValue);
                return TestResult.Fail;            
            }        
        }      

        #endregion     
    }
}

