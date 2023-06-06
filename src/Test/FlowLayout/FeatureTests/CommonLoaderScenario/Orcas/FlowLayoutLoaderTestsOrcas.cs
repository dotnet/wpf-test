// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Layout
{
    /// <summary>
    /// FlowLayoutLoader - Orcas only features 
    /// Indic visual verification tests.
    /// </summary>       
    //vscan tests               
    [Test(2, "ContentElement", "Indic_Gujarati", TestParameters = "content=Indic_Gujarati.xaml", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]    
    [Test(2, "ContentElement", "Indic_Devangari", TestParameters = "content=Indic_Devangari.xaml", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    
    //Tests that specify dimensions on the master    
    [Test(2, "ContentElement", "Indic_Bengali", TestParameters = "content=Indic_Bengali.xaml, masterdimensions=os", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]   
    public class FlowLayoutLoaderOrcas : AvalonTest
    {               
        private FlowLayoutLoaderHelper _fh;
                                   
        public FlowLayoutLoaderOrcas()
            : base()
        {
            _fh = new FlowLayoutLoaderHelper(this);            
            RunSteps += new TestStep(FlowLayoutLoaderOrcas_RunTest);
            CleanUpSteps += new TestStep(CleanUp);
        }
                     
        private TestResult FlowLayoutLoaderOrcas_RunTest()
        {
            Status("Running FlowLayoutLoaderOrcas Test...");   
            _fh.RunLayoutVerificationTest();                    

            if (_fh.finalResult)
            {
                Log.LogStatus("Test Passed.");
                return TestResult.Pass;
            }
            else
            {
                Log.LogStatus("Test Failed.");
                return TestResult.Fail;
            }
        }

        private TestResult CleanUp()
        {
            _fh.CloseWindow();
            return TestResult.Pass;
        }
    }
}