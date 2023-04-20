// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Logging;
using XamlCommon.Microsoft.Test.Xaml.Utilities;
using XamlCompilerCommon;

namespace Microsoft.Test.Xaml.TestTypes
{
    /// <summary>
    ///  Test type for testing the XamlHarvester feature
    /// <para>
    /// The test type does the following
    ///  - Load the SilverlightXamlSchemaContext with the given local and 
    ///    reference assemblies
    ///  - Create a DOMRoot from the provided xaml file and the silverlight
    ///    schema context
    ///  - Pass the DOM to the harvester to get the XamlClassCodeInfo
    ///  - Serialize the XamlClassCodeInfo and compare it against a baseline
    ///  - It an exception occurred during the process, compare the exception with 
    ///    the baseline.
    ///  </para>
    ///  Please note that bulk of the test logic is implemented in the XamlCompilerCommon
    ///  assembly. This is just a wrapper on top of the logic in that assembly integrating
    ///  the test into WPF test infrastructure.
    ///  Please consult the Harvester spec for more details.
    /// </summary>
    public class XamlHarvesterTest : XamlTestType
    {
        /// <summary>
        /// Execute the test type.
        /// </summary>
        public override void Run()
        {
            string xamlFilePath = DriverState.DriverParameters["XamlFile"];
            string localAssemblyPath = null;
            string expectedClassCodeInfoPath = DriverState.DriverParameters["BaselineCodeInfo"];

            XamlCompilerCommon.XamlHarvesterTest harvesterTest = new XamlCompilerCommon.XamlHarvesterTest();

            TestLog log = new TestLog(DriverState.TestName);
            TestLogger logger = new XamlCompilerTestLogger();
            harvesterTest.RunTest(xamlFilePath, localAssemblyPath, expectedClassCodeInfoPath, logger);
            log.Close();
        }
    }
}
