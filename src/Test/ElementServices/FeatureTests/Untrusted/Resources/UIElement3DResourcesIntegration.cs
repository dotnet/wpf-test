// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using System.Xml;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Modeling;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;


namespace Avalon.Test.CoreUI.Resources
{

    /// <summary>
    /// Integration tests for ResourcesDictionaries and resource references in UIElement3D and Viewport2DVisual3D.
    /// </summary>
    [Test(0, "Resources.UIElement3DIntegration", TestCaseSecurityLevel.FullTrust, "UIElement3DResourcesIntegration", SupportFiles=@"FeatureTests\ElementServices\CommonUIElement3DScenario.xaml,FeatureTests\ElementServices\UIElement3DResourceDictionaryTransforms.xml")]
    public class UIElement3DResourcesIntegration : TestCase
    {
        #region
        private string _testName = null;
        private bool _testPassed = true;
        #endregion


        #region Constructor

        [Variation("ResourceDictionaryAtRootStaticReferences" ,Versions="3.0SP1,3.0SP2,AH")]
        [Variation("ResourceDictionaryAtRootDynamicReferences" ,Versions="3.0SP1,3.0SP2,AH")]
        [Variation("ResourceDictionaryOverridesStaticReferences" ,Versions="3.0SP1,3.0SP2,AH")]
        [Variation("ResourceDictionaryOverridesDynamicReferences" ,Versions="3.0SP1,3.0SP2,AH")]
        [Variation("Element3DInResourceDictionaryStaticReference" ,Versions="3.0SP1,3.0SP2,AH")]
        [Variation("Element3DInResourceDictionaryDynamicReference" ,Versions="3.0SP1,3.0SP2,AH")]

        /******************************************************************************
        * Function:          UIElement3DResourcesIntegration Constructor
        ******************************************************************************/
        public UIElement3DResourcesIntegration(string arg) : base(TestCaseType.HwndSourceSupport)
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
        /// Feed Register method with null Id.
        /// </summary>
        TestResult StartTest()
        {
            XamlTransformer resourceTransformer = new XamlTransformer("UIElement3DResourceDictionaryTransforms.xml");

            XmlDocument testDoc = new XmlDocument();
            testDoc.Load("CommonUIElement3DScenario.xaml");

            XmlDocument transformedXaml = resourceTransformer.ApplyTransform(testDoc, _testName);
           
            Page rootElement = (Page)XamlReader.Load(new XmlNodeReader(transformedXaml));
            this.Source.RootVisual = rootElement;

            // Wait for render.
            DispatcherHelper.DoEvents(DispatcherPriority.ApplicationIdle);

            // Pause to help repro debugging.
            DispatcherHelper.DoEvents(1000);

            GlobalLog.LogStatus("Running verifier...");
            
            switch(_testName)
            {
                case "ResourceDictionaryAtRootStaticReferences":
                case "ResourceDictionaryAtRootDynamicReferences":
                    ResourceDictionaryAtRootVerifier(rootElement);
                    break;

                case "ResourceDictionaryOverridesStaticReferences":
                case "ResourceDictionaryOverridesDynamicReferences":
                    ResourceDictionaryOverridesVerifier(rootElement);
                    break;

                case "Element3DInResourceDictionaryStaticReference":
                case "Element3DInResourceDictionaryDynamicReference":
                    Element3DInResourceDictionaryVerifier(rootElement);
                    break;

                default:
                    GlobalLog.LogEvidence("ERROR: Undefined test verification method for transform " + _testName + ".");
                    return TestResult.Fail;
            }

            if (_testPassed)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }
        #endregion


        #region Public Methods
        /******************************************************************************
        * Function:          ResourceDictionaryAtRootVerifier
        ******************************************************************************/
        /// <summary>
        /// Verify that each element with reference to a resource in the root dictionary has the correct value.
        /// </summary>
        public void ResourceDictionaryAtRootVerifier(FrameworkElement rootElement)
        {
            string[] elementNames = new string[] { "vp2d_second_child", "vp2d_nested_child", "containerElement_vp2d_FE", "modelVisual_vp2d_FE"};

            foreach (string elementName in elementNames)
            {
                VerifyNamedElementProperty(rootElement, elementName, Control.BackgroundProperty, Brushes.Blue);
            }
        }

        /******************************************************************************
        * Function:          ResourceDictionaryOverridesVerifier
        ******************************************************************************/
        /// <summary>
        /// Verify that the reference in the closest dictionary in the tree to the referring element is assigned.
        /// </summary>
        public void ResourceDictionaryOverridesVerifier(FrameworkElement rootElement)
        {
            string[] elementNames = new string[] { "vp2d_second_child", "containerElement_vp2d_FE", "modelVisual_vp2d_FE" };

            foreach (string elementName in elementNames)
            {
                VerifyNamedElementProperty(rootElement, elementName, Control.BackgroundProperty, Brushes.Red);
            }

            VerifyNamedElementProperty(rootElement, "vp2d_nested_child", Control.BackgroundProperty, Brushes.Yellow);
        }

        /******************************************************************************
        * Function:          Element3DInResourceDictionaryVerifier
        ******************************************************************************/
        /// <summary>
        /// Verify that ModelUIElement3D and Viewport2DVisual3D can be used as resources.
        /// </summary>
        public void Element3DInResourceDictionaryVerifier(FrameworkElement rootElement)
        {
            Viewport2DVisual3D referencedVp2d = (Viewport2DVisual3D)rootElement.FindName("viewport2dVisual3dResource"); 
            if (referencedVp2d == null)
            {
                GlobalLog.LogEvidence("FAIL: Could not find referenced Viewport2DVisual3D resource in Viewport3D");
                _testPassed = false;
            }
            else
            {
                GlobalLog.LogStatus("Found referenced Viewport2DVisual3D resource in Viewport3D");
            }

            ModelUIElement3D referencedModel = (ModelUIElement3D)rootElement.FindName("modelUIElement3dResource");
            if (referencedModel == null)
            {
                GlobalLog.LogEvidence("FAIL: Could not find referenced ModelUIElement3D resource in Viewport3D");
                _testPassed = false;
            }
            else
            {
                GlobalLog.LogStatus("Found referenced ModelUIElement3D resource in Viewport3D");
            }

            VerifyNamedElementProperty(rootElement, "elementFromResourceDictionary", Control.BackgroundProperty, Brushes.Blue);
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          VerifyNamedElementProperty
        ******************************************************************************/
        private void VerifyNamedElementProperty(FrameworkElement rootElement, string elementName, DependencyProperty testProperty, object expectedValue)
        {
            Control testControl = (Button)rootElement.FindName(elementName);

            if (testControl.GetValue(testProperty) == null && expectedValue != null)
            {
                GlobalLog.LogEvidence("FAIL: " + elementName + " " + testProperty.Name + " property does not have the expected value. Expected " + expectedValue.ToString() + ". Actual null.");
                _testPassed = false;
            }
            else
            {
                GlobalLog.LogStatus(elementName + " " + testProperty.Name + " property has the expected value. " + testControl.GetValue(testProperty).ToString() + ".");
            }

            if (String.Compare(testControl.GetValue(testProperty).ToString(), expectedValue.ToString(), true) != 0)
            {                
                GlobalLog.LogEvidence("FAIL: " + elementName + " " + testProperty.Name + " property does not have the expected value. Expected " + expectedValue.ToString() + ". Actual " + testControl.GetValue(testProperty).ToString() + ".");
                _testPassed = false;
            }
            else
            {
                GlobalLog.LogStatus(elementName + " " + testProperty.Name + " property has the expected value. " + testControl.GetValue(testProperty).ToString() + ".");
            }
        }
        #endregion
    }
}

