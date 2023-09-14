// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Avalon.Test.CoreUI.Commanding
{
    /******************************************************************************
    * CLASS:          CommandingPartialTrustApp
    ******************************************************************************/
    //Note: this class is identical to CommandingApp, except for the Attribute in this case specifies PartialTrust.
    //The reason: the VariationTestAdaptor does not support setting TestCaseSecurityLevel on individual Variations.
    // and there's little evidence that they ever did.
    // However, since they are all Xbap_Browser variants, and since the Xbap that runs them IS partial trust, they are still partial trust scenarios
    // Despite the rather counterintuitive "FullTrust" below.  This AppDomain will be full trust; the other one (created for the Xbap) is definitely partial.
    [Test(0, "Commanding.PartialTrust", TestCaseSecurityLevel.FullTrust, "Var", SupportFiles=@"FeatureTests\ElementServices\CoreCommanding_*.xaml,FeatureTests\ElementServices\Controller*.*")]
    public class CommandingPartialTrustApp : AvalonTest
    {
        #region Private Data
        private string              _testName        = "";
        private string              _applicationType = "";
        private string              _testHost        = "";
        #endregion


        #region Constructor

        //[Variation("SecureInputBindingPartialTrustApp_Xbap_Browser")]
        // [Variation("InputBindingPartialTrustApp_Xbap_Browser")] // [DISABLE WHILE PORTING]
        //[Variation("SecureCommandExecutePartialTrustApp_Xbap_Browser")]
        // [Variation("SecureCommandPartialTrustApp_Xbap_Browser")] // [DISABLE WHILE PORTING]
        //[Variation("SecureCommandBindingPartialTrustApp_Xbap_Browser")]
        // [Variation("SecureCommandBindingExecutePartialTrustApp_Xbap_Browser")] // [DISABLE WHILE PORTING]
        //[Variation("UICommandExecutePartialTrustApp_Xbap_Browser")]
        //[Variation("UICommandPartialTrustApp_Xbap_Browser")]
        // [Variation("UICommandBindingPartialTrustApp_Xbap_Browser")] // [DISABLE WHILE PORTING]
        // [Variation("UICommandBindingExecutePartialTrustApp_Xbap_Browser")] // [DISABLE WHILE PORTING]


        /******************************************************************************
        * Function:          CommandingPartialTrustApp Constructor
        ******************************************************************************/
        public CommandingPartialTrustApp(string arg)
        {
            char[] delimiters = new char[] { '_' };
            String[] argArray = arg.Split(delimiters);
            if (argArray.Length != 3)
            {
                throw new Microsoft.Test.TestSetupException("Three parameters delimited by underscores must be specified.");
            }

            _testName = argArray[0];
            _applicationType = argArray[1];
            _testHost = argArray[2];

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
            GlobalLog.LogStatus("In CommandingPartialTrustApp.RunTest...");

            GlobalLog.LogStatus("*******************************************");
            GlobalLog.LogStatus("ApplicationType: " + _applicationType);
            GlobalLog.LogStatus("Host:            " + _testHost);
            GlobalLog.LogStatus("Running:         " + _testName);
            GlobalLog.LogStatus("*******************************************");

            ApplicationType appType = (ApplicationType)Enum.Parse(typeof(ApplicationType), _applicationType);

            CommonStorage.CleanAll();
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), _testHost);

            ContainerVariationItem cvi = new ContainerVariationItem();
            cvi.Execute(appType, hostType, "Commanding", _testName);

            //A test failure will be handled by an Exception thrown during Verification.
            return TestResult.Pass;
        }
        #endregion
    }
}
