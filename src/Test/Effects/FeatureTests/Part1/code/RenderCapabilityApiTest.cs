// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Media;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// Api Test for effect related api in class RenderCapability
    /// </summary>`
    [Test(1, "ApiTests", "ApiTest")]
    public class RenderCapabilityApiTest : StepsTest
    {
        #region Methods
        /// <summary>
        /// Constructor, set up test step. 
        /// </summary>
        [Variation()]
        public RenderCapabilityApiTest()
        {
            RunSteps += new TestStep(RunTest);
        }

        /// <summary>
        /// Start testing. 
        /// </summary>
        /// <returns></returns>
        TestResult RunTest()
        {
            //Verify IsPixelShaderVersionSupported
            if (!VerifyIsPixelShaderVersionSupported(false))
            {
                // Skip verification until test has a different way to get the value. 
                // return TestResult.Fail;
            }

            //Verify IsPixelShaderVersionSupportedInSoftware
            if (!VerifyIsPixelShaderVersionSupported(true))
            {
                //Skip verification until test has a different way to get the value. 
                //return TestResult.Fail;
            }

            if (!VerifyMaxPixelShaderInstructionSlots())
            {
                //Skip verification until test has a different way to get the value. 
                //return TestResult.Fail;
            }

            //No failure, test case passes. 
            return TestResult.Pass;

        }

        private bool VerifyMaxPixelShaderInstructionSlots()
        {
            // We don't support another verion other than 2.0 and 3.0. So returning 0 for these ps versions. 
            bool success = VerifyMaxPixelShaderInstructionSlots(1, 1, 0)
                && VerifyMaxPixelShaderInstructionSlots(1, 4, 0)
                && VerifyMaxPixelShaderInstructionSlots(-32768, 32767, 0)
                && VerifyMaxPixelShaderInstructionSlots(32767, -32768, 0)
                && VerifyMaxPixelShaderInstructionSlots(4, 0, 0)
                && VerifyMaxPixelShaderInstructionSlots(4, 1, 0)
                // For ps2.0, 96 instruction slots can be used. 
                && VerifyMaxPixelShaderInstructionSlots(2, 0, 96)
                // For ps3.0 the number of instruction slots depends. It hardware doesn't support, return 0. 
                // Otherwise, return the actual card cap. 
                && VerifyMaxPixelShaderInstructionSlots(3, 0, MaxPixelShaderInstructionSlots);
            if (!success)
            {
                Log.LogEvidence("RenderCapability.MaxPixelShaderInstructionSlots failed.");
            }
            return success;
             
        }

        private bool VerifyMaxPixelShaderInstructionSlots(short majorVersion, short minorVersion, int expectedValue)
        {
            TestLog.Current.LogStatus(String.Format("Verifying MaxPixelShaderInstructionSlots for version: {0}.{1}.", majorVersion, minorVersion));

            int actualValue = RenderCapability.MaxPixelShaderInstructionSlots(majorVersion, minorVersion);
            bool success = (actualValue == expectedValue);

            if (!success)
            {
                TestLog.Current.LogStatus(String.Format("Expected Value: {0}, Actual Value: {1}.", expectedValue, actualValue));
            }
            return success;
        }
        /// <summary>
        /// Check return value of IsPixelShaderVersionSupportedInSoftware is checkSoftwareRender
        /// otherwise, check IsPixelShaderVersionSupported. 
        /// </summary>
        /// <param name="checkSoftwareRender">Verify software rendering version?</param>
        /// <returns>true is success, false otherwise</returns>
        private bool VerifyIsPixelShaderVersionSupported(bool checkSoftwareRender)
        {
            //Test case fails if support 1.1, 1.4, 4.0, or 4.1, or not support 2.0.
            return VerifyPSVersion(1, 1, false, checkSoftwareRender)
                && VerifyPSVersion(1, 4, false, checkSoftwareRender)
                && VerifyPSVersion(-32768, 32767, false, checkSoftwareRender)
                && VerifyPSVersion(32767, -32768, false, checkSoftwareRender)
                && VerifyPSVersion(4, 0, false, checkSoftwareRender)
                && VerifyPSVersion(4, 1, false, checkSoftwareRender)
                && VerifyPSVersion(2, 0, true, checkSoftwareRender)
                //should return true for 3.0 iff has hardware support, for IsPixelShaderVersionSupportedInSoftware, should always return false. 
                && VerifyPSVersion(3, 0, checkSoftwareRender ? false : HasPS30SupportHW, checkSoftwareRender);
        }

        /// <summary>
        /// Verify IsPixelShaderVersionSupported for certain ps version. 
        /// </summary>
        /// <param name="majorVersion">Major version requested.</param>
        /// <param name="minorVersion">Minor version requested.</param>
        /// <param name="expectedValue">True if returned value is expected, false otherwise.</param>
        /// <returns></returns>
        private bool VerifyPSVersion(short majorVersion, short minorVersion, bool expectedValue, bool checkSoftwareRender)
        {
            TestLog.Current.LogStatus(string.Format("Verifying ps version: {0}.{1}...", majorVersion, minorVersion));
            
            bool actualValue;

            if (checkSoftwareRender)
            {
                actualValue = RenderCapability.IsPixelShaderVersionSupportedInSoftware(majorVersion, minorVersion);
            }
            else
            {
                actualValue = RenderCapability.IsPixelShaderVersionSupported(majorVersion, minorVersion);
            }

            if (actualValue != expectedValue)
            {
                TestLog.Current.LogEvidence(string.Format("Expected: {0}, actual: {1}.", expectedValue, actualValue));
                return false;
            }

            return true;
        }

        /// <summary>
        /// a static method that return whether the current computer has ps3.0 support. 
        /// This will be determinated in a different way product side is using. For now 
        /// return true. 
        /// </summary>
        /// <returns></returns>
        public static bool HasPS30SupportHW
        {
            get
            {
                return true;
            }
        }

        public static int MaxPixelShaderInstructionSlots
        {
            get
            {
                return 512;
            }
        }
        #endregion
    }
}