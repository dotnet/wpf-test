// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#region Using directives
using System;
using System.Windows;
using System.Threading;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
//using Avalon.Test.CoreUI.PropertyEngine;
using Microsoft.Test.Modeling;
using System.Windows.Media.Animation;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Shapes;
#endregion

namespace Avalon.Test.CoreUI.Resources
{
    /// <summary>
    /// Verification routines for ResourcesModel
    /// </summary>
    static class ResourcesModelVerifiers
    {
        /// <summary>
        /// Verification entry point called after the test tree is rendered.
        /// The method name is set on the Verifier property, either in
        /// xaml or programmatically.
        /// </summary>
        public static bool StartVerify(UIElement root)
        {
            CoreLogger.LogStatus("ResourcesModelVerifiers.StartVerify Starts");           
            
            s_state = new ResourcesModelState(CoreState.Load());
            MouseHelper.MoveOnVirtualScreenMonitor();           

            // Get the node that contains reference to the resource.
            DependencyObject target = _GetTarget((FrameworkElement)root);
            
            //
            // Verify that the lookup worked correctly (values are set correctly).
            //

            // dazfdsadf
            _VerifyResourceLookup(root, target);            
            _VerifyTriggers(root, target);            

            CoreLogger.LogStatus("ResourcesModelVerifiers.StartVerify Ends");
            return true;
        }

        /// <summary>
        /// Get target dependency object under test root.
        /// </summary>
        static private DependencyObject _GetTarget(FrameworkElement root)
        {
            CoreLogger.LogStatus("Getting the target element...");
            
            string testTargetName = s_state.ResourceUseLocation;
            if (s_state.ResourceUseLocation == "Inline")
            {
                testTargetName = testTargetName + "_" + s_state.InlineOwnerAndResourceType;
            }

            DependencyObject target = (DependencyObject)root.FindName(testTargetName);
            
            if (target == null)
            {
                throw new Microsoft.Test.TestValidationException("Couldn't find target item by name '" + s_state.ResourceUseLocation + "'.");
            }

            return target;
        }

        /// <summary>
        /// Verify the property value by providing the actual and expected values.
        /// Legal values for comparisonType are: GradioentStops, Style, Width, Background.
        /// </summary>
        static private void _VerifyProperty(object actualValue, object expectedValue, string comparisonType)
        {
            if (comparisonType == "GradientStops")
            {
                CoreLogger.LogStatus("Comparing GradientStops...", ConsoleColor.Green);

                GradientStopCollection actualValue1 = (GradientStopCollection)actualValue;
                GradientStopCollection expectedValue1 = (GradientStopCollection)expectedValue;

                if (actualValue1[0].Color != expectedValue1[0].Color || actualValue1[0].Offset != expectedValue1[0].Offset ||
                    actualValue1[1].Color != expectedValue1[1].Color || actualValue1[1].Offset != expectedValue1[1].Offset)
                {
                    // Test Case Broken.
                    throw new Microsoft.Test.TestValidationException("FAIL: GradientStop Colors did not get looked up properly from resource");
                }
            }

            else if (comparisonType == "Style")
            {
                CoreLogger.LogStatus("Comparing Styles...", ConsoleColor.Green);

                Style actualValue1 = (Style)actualValue;
                Setter actualSetter = (Setter)actualValue1.Setters[0];

                Style expectedValue1 = (Style)expectedValue;
                Setter expectedSetter = (Setter)expectedValue1.Setters[0];

                if (_ConvertToString(actualSetter.Value) != _ConvertToString(expectedSetter.Value))
                {
                    // Test Case Broken.
                    throw new Microsoft.Test.TestValidationException("FAIL: Style Background Colors did not get looked up properly from resource. GOT: " + _ConvertToString(actualSetter.Value) + " EXPECTED: " + _ConvertToString(expectedSetter.Value));
                }
            }

            else if (comparisonType == "Background")
            {
                CoreLogger.LogStatus("Comparing Backgrounds...", ConsoleColor.Green);

                string actualValue1 = _ConvertToString((Brush)actualValue);
                string expectedValue1 = _ConvertToString((Brush)expectedValue);
                
                if (actualValue1 != expectedValue1)
                {
                    // Test Case Broken.
                    throw new Microsoft.Test.TestValidationException("FAIL: Background Colors did not get looked up properly from resource. GOT: " + actualValue1 + " EXPECTED: " + expectedValue1);
                }
            }

            else if (comparisonType == "width")
            {
                CoreLogger.LogStatus("Comparing Widths...", ConsoleColor.Green);

                string actualValue1 = _ConvertToString((double)actualValue);
                string expectedValue1 = _ConvertToString((double)expectedValue);

                if (actualValue1 != expectedValue1)
                {
                    // Test Case Broken.
                    throw new Microsoft.Test.TestValidationException("FAIL: Widths do not match-up. GOT: " + actualValue1 + " EXPECTED: " + expectedValue1);
                }
            }

        }
        /// <summary>
        /// Verify the that the resource was looked up correctly - comparing actual value to expected value
        /// </summary>
        static private void _VerifyResourceLookup (UIElement testRoot, DependencyObject target)
        {   
            string inlineName = s_state.InlineOwnerAndResourceType;

            if (inlineName == "FE_Freezable")
            {
                TextBlock testTarget = (TextBlock)target;
                Brush actual = (Brush)testTarget.Background;

                // Get Expected Value.
                Brush expected = Brushes.Green;

                // Compare and Verify.
                _VerifyProperty(actual, expected, "Background");
            }
            else if (inlineName == "FE_Style")
            {
                TextBlock testTarget = (TextBlock)target;
                Brush actual = (Brush)testTarget.Background;

                // Get Expected Value.
                Brush expected = Brushes.Green;

                // Compare and Verify.
                _VerifyProperty(actual, expected, "Background");
            }
            else if (inlineName == "FE_Control")
            {
                UserControl testTarget = (UserControl)target;
                Button testTargetButton = (Button)testTarget.Content;
                Brush actual = (Brush)testTargetButton.Background;

                // Get Expected Value.
                Brush expected = Brushes.Green;

                // Compare and Verify.
                _VerifyProperty(actual, expected, "Background");
            }
            else if (inlineName == "FCE_Freezable")
            {
                // Get Actual Value.
                FlowDocumentReader testTarget = (FlowDocumentReader)target;
                FlowDocument testTargetFlowDoc = (FlowDocument)testTarget.Document;
                Brush actual = (Brush)testTargetFlowDoc.Background;

                // Get Expected Value.
                Brush expected = Brushes.Green;

                // Compare and Verify.
                _VerifyProperty(actual, expected, "Background");
            }
            else if (inlineName == "Control_Freezable")
            {
                Button testTarget = (Button)target;
                Brush actual = (Brush)testTarget.Background;

                // Get Expected Value.
                Brush expected = Brushes.Green;

                // Compare and Verify.
                _VerifyProperty(actual, expected, "Background");
            }
            else if (inlineName == "Control_Style")
            {
                // Get Actual Value.
                Button testTarget = (Button)target;
                Brush actual = (Brush)testTarget.Background;

                // Get Expected Value.
                Brush expected = Brushes.Green;

                // Compare and Verify.
                _VerifyProperty(actual, expected, "Background");
            }
            else if (inlineName == "Control_Template")
            {
                Control testTarget = (Control)target;
                CheckBox testTargetCheckBox = (CheckBox)testTarget.Template.FindName("Up", testTarget);
                Brush actual = (Brush)testTargetCheckBox.Background;

                // Get Expected Value.
                Brush expected = Brushes.Green;

                // Compare and Verify.
                _VerifyProperty(actual, expected, "Background");
            }
            else if (inlineName == "Control_FCE")
            {
                // Get Actual Value.
                FlowDocumentReader testTarget = (FlowDocumentReader)target;
                FlowDocument testTargetFlowDoc = (FlowDocument)testTarget.Document;
                Brush actual = (Brush)testTargetFlowDoc.Background;

                // Get Expected Value.
                Brush expected = Brushes.Green;

                // Compare and Verify.
                _VerifyProperty(actual, expected, "Background");

            }
            else if (inlineName == "Freezable_CLR")
            {
                // Get Actual Value.
                Button testTarget = (Button)target;
                Brush actual = (Brush)testTarget.Background;
                
                // Get Expected Value.
                Brush expected = Brushes.Green;

                // Compare and Verify.
                _VerifyProperty(actual, expected, "Background");

            }
            else if (inlineName == "Freezable_Freezable")
            {
                // Get the actual Value.
                Shape testTarget = (Rectangle)target;
                LinearGradientBrush testTargetLGB = (LinearGradientBrush)testTarget.Fill;
                GradientStopCollection actual = testTargetLGB.GradientStops;

                // Get the expected Value.
                GradientStopCollection expected = new GradientStopCollection();
                expected.Add(new GradientStop(Colors.Green, 0));
                expected.Add(new GradientStop(Colors.Navy, 1));

                // Compare and Verify.
                _VerifyProperty(actual, expected, "GradientStops");
            }
            else if (inlineName == "N/A")
            {
                // Nothing to verify here.
            }
        }

        /// <summary>
        /// Verify the that the resource was looked up correctly - comparing actual value of trigger target to expected value
        /// </summary>
        static private void _VerifyTriggers (UIElement testRoot, DependencyObject target)
        {
            string elementName = s_state.ResourceUseLocation;

            // Stuff for trigger condition(s).
            // Move Mouse on target.
            MouseHelper.Move((UIElement)target, MouseLocation.Center);

            if (elementName == "StyleSetterValue")
            {
                // Get Actual Value.
                Button testTarget = (Button)target;
                Brush actual = (Brush)testTarget.Background;

                // Get Expected Value.
                Brush expected = Brushes.Green;

                // Compare and Verify.
                _VerifyProperty(actual, expected, "Background");
            }
            else if (elementName == "StyleTriggerConditionValue_SingleTrigger")
            {
                // Get Actual Value.
                Button testTarget = (Button)target;
                Brush actual = (Brush)testTarget.Background;

                // Get Expected Value.
                Brush expected = Brushes.Green;

                // Compare and Verify.
                _VerifyProperty(actual, expected, "Background");
            }
            else if (elementName == "StyleTriggerConditionValue_MultiTrigger")
            {
                // Get Actual Value.
                Button testTarget = (Button)target;
                double actual = (double)testTarget.Width;

                // Get Expected Value.
                double expected = 200.0;

                // Compare and Verify.
                _VerifyProperty(actual, expected, "width");
            }
            else if (elementName == "TemplateTriggerConditionValue_ControlTemplate_SingleTrigger")
            {
                // Get Actual Value.
                Control testTarget = (Control)target;
                Button testTargetButton = (Button)testTarget.Template.FindName("Up", testTarget);
                double actual = (double)testTargetButton.Width;

                // Get Expected Value.
                double expected = 200.0;

                // Compare and Verify.
                _VerifyProperty(actual, expected, "width");
            }
            else if (elementName == "TemplateTriggerConditionValue_ControlTemplate_MultiTrigger")
            {
                // Get Actual Value.
                Control testTarget = (Control)target;
                Button testTargetButton = (Button)testTarget.Template.FindName("Up", testTarget);
                double actual = (double)testTargetButton.Width;

                // Get Expected Value.
                double expected = 200.0;

                // Compare and Verify.
                _VerifyProperty(actual, expected, "width");
            }
            else if (elementName == "StyleTriggerSetterValue_SingleTrigger")
            {
                // Get Actual Value.
                Button testTarget = (Button)target;
                Brush actual = (Brush)testTarget.Background;
                
                // Get Expected Value.
                Brush expected = Brushes.Green;

                // Compare and Verify.
                _VerifyProperty(actual, expected, "Background");
            }
            else if (elementName == "StyleTriggerSetterValue_MultiTrigger")
            {
                // Get Actual Value.
                Button testTarget = (Button)target;
                Brush actual = (Brush)testTarget.Background;

                // Get Expected Value.
                Brush expected = Brushes.Green;

                // Compare and Verify.
                _VerifyProperty(actual, expected, "Background");
            }
            else if (elementName == "TemplateElementPropertyValue_ControlTemplate")
            {
                // Get Actual Value.                    
                Control testTarget = (Control)target;
                CheckBox testTargetCheckBox = (CheckBox)testTarget.Template.FindName("Up", testTarget);
                Brush actual = (Brush)testTargetCheckBox.Background;

                // Get Expected Value.
                Brush expected = Brushes.Green;

                // Compare and Verify.
                _VerifyProperty(actual, expected, "Background");
            }

            else if (elementName == "TemplateTriggerSetterValue_ControlTemplate_SingleTrigger")
            {
                // Get Actual Value.
                Control testTarget = (Control)target;
                Button testTargetButton = (Button)testTarget.Template.FindName("Up", testTarget);
                Brush actual = (Brush)testTargetButton.Background;
                
                // Get Expected Value.
                Brush expected = Brushes.Green;

                // Compare and Verify.
                _VerifyProperty(actual, expected, "Background");
            }
            else if (elementName == "TemplateTriggerSetterValue_ControlTemplate_MultiTrigger")
            {
                // Get Actual Value.
                Control testTarget = (Control)target;
                CheckBox testTargetCheckBox = (CheckBox)testTarget.Template.FindName("Up", testTarget);

                // Match Trigger Conditions.
                testTargetCheckBox.SetValue(CheckBox.IsCheckedProperty, true);

                Brush actual = (Brush)testTargetCheckBox.Background;

                // Get Expected Value.
                Brush expected = Brushes.Green;

                // Compare and Verify.
                _VerifyProperty(actual, expected, "Background");
            }
            MouseHelper.Move((UIElement)testRoot, MouseLocation.Bottom);
        }

        /// <summary>
        /// Converts various object types to a format that is helpful for
        /// comparisons in tests.
        /// </summary>
        /// <param name="obj">The object to convert to a string.</param>
        static private string _ConvertToString(object obj)
        {
            if (obj == null)
            {
                return "null";
            }
            else if (obj is DependencyProperty)
            {
                DependencyProperty dp = (DependencyProperty)obj;

                return dp.OwnerType.Name + "." + dp.Name;
            }
            else
            {
                IFormattable f = obj as IFormattable;

                if (f != null)
                {
                    return f.ToString();
                }
                else
                {
                    return obj.ToString();
                }
            }
        }

        private static ResourcesModelState s_state = null;
    }
}
