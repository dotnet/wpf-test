// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Parser;
using Avalon.Test.CoreUI.Serialization;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Serialization;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;

namespace Avalon.Test.CoreUI.Integration
{
    /******************************************************************************
    * CLASS:          XamlBasedIntegration
    ******************************************************************************/
    /// <summary>
    /// Verify ElementServices Integration tests.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for Integration.
    /// </description>
    [Test(1, "Integration.XamlBased", "Int", SecurityLevel=TestCaseSecurityLevel.PartialTrust, SupportFiles=@"FeatureTests\ElementServices\*.xaml,FeatureTests\ElementServices\FakeInfo.xml")]
    public class XamlBasedIntegration : AvalonTest
    {
        #region Private Data
        private static string s_xamlFileName = "";
        #endregion

        #region Constructor
        //TO-DO: when VariationTestAdaptor supports SubAreas on Variations, use the names in the comments below.
        //Commanding
        [Variation("CoreCommanding_MyGridPanelCommandBindings")]
        [Variation("CoreCommanding_MyGridPanelShortNameCommandBindings")]

        //Events\AddOwner
        [Variation("AddOwnerKeyDown")]

        //IdTest\Element
        [Variation("FrameworkElementWithId")]
        [Variation("FrameworkElementWithxId")]
        [Variation("FrameworkElementWithIdAndxId")]
        [Variation("FrameworkElementWithComplexName")]
        [Variation("FrameworkContentElementWithId")]
        [Variation("FrameworkContentElementWithxId")]
        [Variation("FrameworkContentElementWithIdAndxId")]
        [Variation("EventTriggerOnFrameworkElement")]

        //IdTest\Storyboard
        [Variation("StoryboardWithName")]

        // [DISABLE WHILE PORTING]
        //IdTest\DirectTargeting
        // [Variation("DirectTargeting")]

        //IdTest\Style
        [Variation("NameScopeInStyle")]
        [Variation("BindingFromStyleTrigger")]
        [Variation("StoryboardsInStyle")]

        //IdTest\ForwardReference
        [Variation("ForwardReferenceingOfIdUsingDataBinding")]
        [Variation("ForwardReferenceingOfIDUsingDataBindingInTemplate")]

        //IdTest\CustomIIdScope
        [Variation("FrameworkElementWithIDUnderCustomIIdScopeWithSameIdsInDifferentScope")]

        //IdTest\Template
        [Variation("FrameworkContentElementWithIDInControlTemplate")]
        [Variation("FrameworkContentElementWithIDInDataTemplate")]
        [Variation("FrameworkContentElementWithxIDInControlTemplate")]
        [Variation("FrameworkContentElementWithxIDInDataTemplate")]
        [Variation("FrameworkElementWithIDInControlTemplate")]
        [Variation("FrameworkElementWithIDInDataTemplate")]
        [Variation("FrameworkElementWithxIDInControlTemplate")]
        [Variation("FrameworkElementWithxIDInDataTemplate")]
        [Variation("ItemsControlWithDataTemplate")]

        //IdTest\Template\Trigger
        [Variation("TriggersInControlTemplate")]
        [Variation("TriggersInDataTemplate")]

        //IdTest\Template\DataBinding
        [Variation("DataBindingInControlTemplate")]

        //IdTest\CustomName
        [Variation("FrameworkElementWithCustomNameInDataTemplate")]
        [Variation("FrameworkElementWithCustomNameInControlTemplate")]
        [Variation("FrameworkContentElementWithCustomName")]
        [Variation("FrameworkElementWithCustomName")]
        [Variation("FrameworkElementWithCustomNameUnderCustomIIdScopeWithSameIdsInDifferentScope")]

        //PropertyEngine\ValueSource
        [Variation("ValueSourceTest")]

        //PropertyEngine\Serialization
        [Variation("DP0001")]
        [Variation("DP0002")]
        [Variation("DP0003")]

        // [DISABLE WHILE PORTING]
        //ProperyEngine\PropertyTrigger
        // [Variation("DP0101")]
        // [Variation("DP0102")]
        // [Variation("DP0103")]
        // [Variation("DP0106")]
        // [Variation("DP0107")]
        // [Variation("DP0108")]
        // [Variation("DP0111")]
        // [Variation("DP0112")]
        // [Variation("DP0113")]
        // [Variation("DP0116")]
        // [Variation("DP0117")]
        // [Variation("DP0118")]
        // [Variation("DP0121")]
        // [Variation("DP0122")]
        // [Variation("DP0123")]
        // [Variation("DP0126")]
        // [Variation("DP0127")]
        // [Variation("DP0128")]
        // [Variation("DP0131")]
        // [Variation("DP0132")]
        // [Variation("DP0133")]
        // [Variation("DP0141")]
        // [Variation("DP0142")]
        // [Variation("DP0143")]
        // [Variation("DP0151")]
        // [Variation("DP0152")]
        // [Variation("DP0153")]

        //PropertyEngine\Template\AutoAlias
        [Variation("AutoAlias")]

        //PropertyEngine\Template\ThemeChange
        [Variation("ThemeChange")]

        //PropertyEngine\Template\BugRepro
        [Variation("BugRepro3")]
        [Variation("BugRepro4")]

        //Resources\MergedDictionaryTemplates
        [Variation("MergedDictionaryTemplatesMain")]

        /******************************************************************************
        * Function:          XamlBasedIntegration Constructor
        ******************************************************************************/
        public XamlBasedIntegration(string arg)
        {
            s_xamlFileName = arg;
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        TestResult StartTest()
        {
            TestResult result = TestResult.Pass;

            s_xamlFileName = s_xamlFileName + ".xaml";
            CoreLogger.LogStatus("*******************************************");
            CoreLogger.LogStatus("Testing: " + s_xamlFileName);
            CoreLogger.LogStatus("*******************************************");

            object root = ParserUtil.ParseXamlFile(s_xamlFileName);
            if (root == null)
            {
                GlobalLog.LogEvidence("ERROR (StartTest): root was null");
                result = TestResult.Fail;
            }

            if ((root as UIElement) == null)
            {
                GlobalLog.LogEvidence("ERROR (StartTest): root was not a UIElement");
                result = TestResult.Fail;
            }
            else
            {
                //Any test failures will result in an Exception in the SerializationHelper.
                (new SerializationHelper()).DisplayTree(root as UIElement);
            }

            return result;
        }
        #endregion
    }
}
