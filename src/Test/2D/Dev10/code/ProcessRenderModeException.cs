// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: ProcessRenderMode testing
 * simple simple test that ensures correct exception throwing for invalid values
 ********************************************************************/
using System;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Graphics
{
    [Test(0, "ProcessRenderModeException", "ProcessRenderModeException", 
        Area = "2D",
        Description = "Verification for ProcessRenderMode Incorrect Value Exception")]
    public class ProcessRenderModeExceptionTest : StepsTest
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ProcessRenderModeExceptionTest()
        {
            RunSteps += new TestStep(InvalidSetException);
        }

        #region test step     
        
        //try to set an invalid value and ensure that we throw the correct type of exception
        private TestResult InvalidSetException()
        {
            TestResult result = TestResult.Unknown;
            try
            {                
                RenderOptions.ProcessRenderMode = (RenderMode.SoftwareOnly+1);
            }
            catch(System.ComponentModel.InvalidEnumArgumentException)
            {
                result = TestResult.Pass;
            }
            catch(Exception e)
            {
                //any other exception and it's a failure
                Log.LogEvidence("Incorrect exception type! Expected System.ComponentModel.InvalidEnumArgumentExceptio, got {0}", e.GetType());
                result = TestResult.Fail;
            }
            return result;                    
        }

        #endregion     
    }
}

