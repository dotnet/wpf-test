// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.PropertyEngine;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.UtilityHelper;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;


namespace Avalon.Test.CoreUI.PropertyEngine.TestStyleResourcesApi
{
    /******************************************************************************
    * CLASS:          StyleResourcesApi
    ******************************************************************************/
    [Test(0, "PropertyEngine.StyleResourcesApi", TestCaseSecurityLevel.FullTrust, "StyleResourcesApi")]
    public class StyleResourcesApi : TestCase
    {
        #region Private Data
        private string _testName = "";
        #endregion

        #region Constructor

        [Variation("TestStyleResources")]
        [Variation("StyleResourcesInvalidationsScenarios")]
        [Variation("Bug71")]

        /******************************************************************************
        * Function:          StyleResourcesApi Constructor
        ******************************************************************************/
        public StyleResourcesApi(string arg)
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
        /// Entry Method for the test case
        /// </summary>
        TestResult StartTest()
        {
            TestStyleResourcesApi test = new TestStyleResourcesApi();

            switch (_testName)
            {
                case "TestStyleResources":
                    test.TestStyleResources();
                    break;
                case "StyleResourcesInvalidationsScenarios":
                    test.StyleResourcesInvalidationsScenarios();
                    break;
                case "Bug71":
                    test.Bug71();
                    break;
                default:
                    throw new Microsoft.Test.TestValidationException("ERROR!!! Test case not found.");
            }

            //Any test failures will be caught by Asserts or Exceptions.
            return TestResult.Pass;
        }
        #endregion
    }


    /******************************************************************************
    * CLASS:          TestStyleResourcesApi
    ******************************************************************************/
    public class TestStyleResourcesApi
    {
        /******************************************************************************
        * Function:          TestStyleResources
        ******************************************************************************/
        /// <summary>
        /// From end users� perspective, the result of task item  is an addition of
        /// a new read-only property Resources is added to Style class 
        ///     public ResourceDictionary Resources.
        /// Method TestStyleResources tests this API.
        /// </summary>
        public void TestStyleResources()
        {
            CoreLogger.LogStatus("API testing: Style.Resources Read-Only Property");

            FrameworkElement fe = new FrameworkElement();
            FrameworkContentElement fce = new FrameworkContentElement();
            
            if (fe.Style == null)
            {
                CoreLogger.LogStatus("Style property itself can be null (FE)");
            }
            if (fce.Style == null)  
            {
                CoreLogger.LogStatus("Style property itself can be null (FCE)");
            }

            Style style = new Style();
            style.Resources.Add("key1", Brushes.Gold);
            fe.Style = style;
            style = new Style();
            style.Resources.Clear(); //Access it so that Resources can be sealed. See Bug71 
            fce.Style = style;
            if (fe.Style.Resources != null)
            {
                CoreLogger.LogStatus("Style.Resources can never be null (FE)");
            }
            if (fce.Style.Resources != null)
            {
                CoreLogger.LogStatus("Style.Resources can never be null (FCE)");
            }

            CoreLogger.LogStatus("After a Style is in use (sealed), it cannot be modified.");
            try
            {
                CoreLogger.LogStatus("Cannot Add into Style.Resources");
                fce.Style.Resources.Add("key2", "value2");
                throw new Microsoft.Test.TestValidationException("Expected exception not reveived.");
            }
            catch (InvalidOperationException ex)
            {
                CoreLogger.LogStatus("Expected received exception " + ex);
            }
            try
            {
                CoreLogger.LogStatus("Cannot modify Style.Resources item");
                fe.Style.Resources["key1"] = Brushes.Goldenrod;
                throw new Microsoft.Test.TestValidationException("Expected exception not reveived.");
            }
            catch (InvalidOperationException ex)
            {
                CoreLogger.LogStatus("Expected received exception " + ex);
            }
            try
            {
                CoreLogger.LogStatus("Cannot Clear Style.Resource");
                fce.Style.Resources.Clear();
                throw new Microsoft.Test.TestValidationException("Expected exception not reveived.");
            }
            catch (InvalidOperationException ex)
            {
                CoreLogger.LogStatus("Expected received exception " + ex);
            }
            try
            {
                CoreLogger.LogStatus("Cannot Remove Style.Resources item");
                fe.Style.Resources.Remove("key1");
                throw new Microsoft.Test.TestValidationException("Expected exception not reveived.");
            }
            catch (InvalidOperationException ex)
            {
                CoreLogger.LogStatus("Expected received exception " + ex);
            }

            fe.SetResourceReference(FrameworkElement.TagProperty, "key1");
            if (fe.Tag == Brushes.Gold)
            {
                CoreLogger.LogStatus("FE: Use Style.Resources");
            }
            
            fce.SetResourceReference(FrameworkContentElement.TagProperty, "key2");
            if (fce.Tag == null)
            {
                CoreLogger.LogStatus("FCE: Use Style.Resources");
            }
        }

        /******************************************************************************
        * Function:          StyleResourcesInvalidationsScenarios
        ******************************************************************************/
        /// <summary>
        /// To support Style.Resources, StyleHelper.DoStyleResourcesInvalidations
        /// is called to propagate invalidations for Style.Resources changes
        /// 
        /// For oldStyle/newStyle, they may be (1)null (2)non-null with null Resources (3)non-null with non-null Resources
        /// for a total of 9 combination
        /// </summary>
        public void StyleResourcesInvalidationsScenarios()
        {
            CoreLogger.LogStatus("DoStyleResourcesInvalidations: More Scenarios");
            FrameworkElement fe;
            FrameworkContentElement fce;

            CoreLogger.LogStatus("(1) null -> null");
            fe = new FrameworkElement();
            fe.Style = null;
            ValidateTag(fe, null, null);

            CoreLogger.LogStatus("(2) null -> non-null with null Resources");
            fce = new FrameworkContentElement();
            fce.Style = new Style();
            ValidateTag(null, fce, null);

            CoreLogger.LogStatus("(3) null -> non-null with non-null Resources");
            fce = new FrameworkContentElement();
            fce.Style = GetAStyleWithResources("ResourceGreen", Brushes.Green, "ResourceRed", Brushes.Red);
            ValidateTag(null, fce, null);
            fce.SetResourceReference(FrameworkContentElement.TagProperty, "ResourceRed");
            ValidateTag(null, fce, Brushes.Red);
            fce.SetResourceReference(FrameworkContentElement.TagProperty, "ResourceGreen");
            ValidateTag(null, fce, Brushes.Green);
            fce.ClearValue(FrameworkContentElement.TagProperty);
            ValidateTag(null, fce, null);

            CoreLogger.LogStatus("(4) non-null with null Resources -> null");
            fe = new FrameworkElement();
            fe.Style = new Style();
            fe.Style = null;
            ValidateTag(fe, null, null);

            CoreLogger.LogStatus("(5) non-null with null Resources -> non-null with null Resources");
            fce = new FrameworkContentElement();
            fce.Style = new Style();
            fce.Style = new Style();
            ValidateTag(null, fce, null);

            CoreLogger.LogStatus("(6) non-null with null Resources -> non-null with non-null Resources");
            fe = new FrameworkElement();
            fe.Style = GetAStyleWithResources("ResourceIndigo", Brushes.Indigo, "ResourceIvory", Brushes.Ivory);
            ValidateTag(fe, null, null);
            fe.SetResourceReference(FrameworkElement.TagProperty, "ResourceIvory");
            ValidateTag(fe, null, Brushes.Ivory);
            fe.SetResourceReference(FrameworkElement.TagProperty, "ResourceIndigo");
            ValidateTag(fe, null, Brushes.Indigo);
            fe.ClearValue(FrameworkElement.TagProperty);
            ValidateTag(fe, null, null);

            CoreLogger.LogStatus("(7) non-null with non-null Resources -> null");
            fe = new FrameworkElement();
            fe.Style = GetAStyleWithResources("ResourceIndigo", Brushes.Indigo, "ResourceIvory", Brushes.Ivory);
            fe.SetResourceReference(FrameworkElement.TagProperty, "ResourceIndigo");
            fe.Style = null;
            fe.ClearValue(FrameworkElement.TagProperty);
            ValidateTag(fe, null, null);

            CoreLogger.LogStatus("(8) non-null with non-null Resources -> non-null with null Resources");
            fce = new FrameworkContentElement();
            fce.Style = GetAStyleWithResources("ResourceGreen", Brushes.Green, "ResourceRed", Brushes.Red);
            fce.SetResourceReference(FrameworkContentElement.TagProperty, "ResourceGreen");
            fce.Style = new Style();
            fce.ClearValue(FrameworkContentElement.TagProperty);
            ValidateTag(null, fce, null);

            CoreLogger.LogStatus("(9) non-null with non-null Resources -> non-null with non-null Resources");
            fce = new FrameworkContentElement();
            fce.Style = GetAStyleWithResources("ResourceGreen", Brushes.Green, "ResourceRed", Brushes.Red);
            fce.SetResourceReference(FrameworkContentElement.TagProperty, "ResourceGreen");
            fce.Style = GetAStyleWithResources("ResourceGreen", Brushes.Red, "ResourceRed", Brushes.Green);
            ValidateTag(null, fce, Brushes.Red);
        }

        private Style GetAStyleWithResources(object key1, object value1, object key2, object value2)
        {
            Style style = new Style();
            style.Resources.Add(key1, value1);
            style.Resources.Add(key2, value2);
            return style;
        }

        private void ValidateTag(FrameworkElement fe, FrameworkContentElement fce, object expectedValue)
        {
            System.Diagnostics.Debug.Assert(fe != null || fce != null);
            System.Diagnostics.Debug.Assert(fe == null || fce == null);

            if (fe != null)
            {
                if (fe.GetValue(FrameworkElement.TagProperty) == expectedValue)
                {
                    CoreLogger.LogStatus("Tag value as expected");
                }
            }
            else
            {
                if (fce.GetValue(FrameworkElement.TagProperty) == expectedValue)
                {
                    CoreLogger.LogStatus("Tag value as expected");
                }
            }
        }

        /******************************************************************************
        * Function:          Bug71
        ******************************************************************************/
        /// <summary>
        /// Regression Test for 


        public void Bug71()
        {
            CoreLogger.LogStatus("Regression Test for bug 71");

            FrameworkElement fe = new FrameworkElement();
            fe.Style = new Style();

            try
            {
                fe.Style.Resources.Add("key", "value");
                throw new Microsoft.Test.TestValidationException("Expected exception not reveived.");
            }
            catch (InvalidOperationException ex)
            {
                CoreLogger.LogStatus("Expected received exception " + ex);                
            }
        }
    }
}
