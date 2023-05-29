// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Avalon.Test.CoreUI.CoreInput
{
    /******************************************************************************
    * CLASS:          UIElement3DSequenceTests
    ******************************************************************************/
    [Test(1, "Input.Element3D", TestCaseSecurityLevel.FullTrust, "UIElement3DSequence", SupportFiles=@"FeatureTests\ElementServices\UIElement3DKeyboardScenarios.xtc,FeatureTests\ElementServices\UIElement3DMouseScenarios.xtc,FeatureTests\ElementServices\CommonUIElement3DScenario.xaml")]
    public class UIElement3DSequenceTests : AvalonTest
    {
        #region Private Data
        private string _testName = "";
        #endregion

        #region Constructor

        [Variation("Keyboard", Versions="3.0SP1,3.0SP2,AH", Disabled=true)]
        [Variation("Mouse", Versions="3.0SP1,3.0SP2,AH", Disabled=true)]

        /******************************************************************************
        * Function:          UIElement3DSequenceTests Constructor
        ******************************************************************************/
        public UIElement3DSequenceTests(string arg)
        {
            _testName = arg;
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Stub for launching UIElement3D, ModelUIElement3D and Viewport2DVisual3D sequence tests.
        /// </summary>
        TestResult StartTest()
        {
            string actionSequenceFileName = "";

            GlobalLog.LogStatus("Running UIElement3D Input Sequence Test...");

            if (_testName == "Keyboard")
            {
                actionSequenceFileName = "UIElement3DKeyboardScenarios.xtc";
            }
            else if (_testName == "Mouse")
            {
                actionSequenceFileName = "UIElement3DMouseScenarios.xtc";
            }
            else
            {
                throw new Microsoft.Test.TestSetupException("ERROR: An incorrect test name was specified.");
            }

            //
            // Load scenarios and get Xml Test node.
            //
            XmlDocument doc = new XmlDocument();            
            doc.Load(actionSequenceFileName);
            XmlNode testNode = doc.DocumentElement;

            ActionSequenceTestEngine sequenceEngine = new ActionSequenceTestEngine();

            // Create an HwndSource for hosting the test tree.
            sequenceEngine.TestContainer = new ExeStubContainerCore();

            sequenceEngine.RunVariation(testNode, "ActionSequence");

            return TestResult.Pass;
        }
        #endregion
    }
}
