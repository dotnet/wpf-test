// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Verifies the tree and any user interaction aspects
 *          of it (animation, triggers) for ChainedTriggersModel.
 *
 *
 
  
 * Revision:         $Revision: 12 $
 
 * Filename:         $Source: //depot/vbl_wcp_avalon_dev/windowstest/client/wcptests/Core/Framework/BVT/Resources/ChainedTriggersVerifiers.cs $
********************************************************************/
using System;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Avalon.Test.CoreUI;
using System.Text;
using System.Collections;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Test.Threading;
using Avalon.Test.CoreUI.Common;
//using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.Threading;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.PropertyEngine.ChainedTriggers
{
    /// <summary>
    /// Verification routines for ChainedTriggersModel.
    /// </summary>
    static class ChainedTriggersModelVerifiers
    {
        /// <summary>
        /// Verification entry point called after the test tree is rendered.
        /// The method name is set on the Verifier property, either in
        /// xaml or programmatically.
        /// </summary>
        public static bool StartVerify(UIElement root)
        {
            CoreLogger.LogStatus("In StartVerify()...");

            // Load the model state.            
            s_state = new ChainedTriggersModelState(CoreState.Load());
            if (s_state.Context == "Style")
            {
                // Get the resourcedItem (it may be in the template).
                DependencyObject resourcedItem = _GetResourcedItem((FrameworkElement)root);

                //
                // Test Lookup
                //

                // Do regular verification.
                _VerifyChainedTriggers(resourcedItem);
            }

            else
            {
                //
                // Verify and get templated control.
                //
                FrameworkElement templatedElement = _VerifyTemplatedElement((FrameworkElement)root);

                //
                // Verify the template tree can be found by searching the templated control's template property.
                //
                FrameworkElement templateRoot = _VerifyTemplateTree(templatedElement);

                //
                // Verify the template child can be found from the template root.
                //
                DependencyObject templateChild = _VerifyTemplateChild(templateRoot);

                // verify property triggers - storyBoard actions
                _VerifyChainedTriggers(templatedElement);

            }

            MouseHelper.Move((UIElement)root, MouseLocation.Bottom);

            return true;
        }

        /// <summary>
        /// Checks:
        /// That the template tree child can be found by name from the template root.
        /// </summary>
        static private DependencyObject _VerifyTemplateChild(FrameworkElement templateRoot)
        {

            string childName = "TemplateChildType_";

            // Choose which child name to search for.
            childName = "TemplateChildType_FrameworkElement";

            // Search for the child.
            CoreLogger.LogStatus("Verifying a template child " + childName + " can be found from the template root by name...");

            DependencyObject child = (DependencyObject)templateRoot.FindName(childName);


            // Verify child was found and return.

            if (child == null)
            {
                CoreLogger.LogStatus("Fail", ConsoleColor.Red);
                throw new Microsoft.Test.TestValidationException("Could not find template tree child with name " + childName);
            }
            else
            {
                CoreLogger.LogStatus("    Found.");
            }
            return child;

        }

        /// <summary>
        /// Checks:
        /// 1. The templated control has logical children if it's supposed to.
        /// 2. The template root is the first visual child of the templated control.
        /// 3. Id lookup works correctly in the template tree.
        /// 4. The stable (non-triggered) property values are correct.
        /// </summary>
        static private FrameworkElement _VerifyTemplateTree(FrameworkElement templatedControl)
        {
            FrameworkElement templateRoot = null;

            CoreLogger.LogStatus("Verifying the template tree...", ConsoleColor.Cyan);

            //
            // Get the template root.
            //            
            CoreLogger.LogStatus("    Finding template root.");
            templateRoot = _GetTemplateRoot(templatedControl);
            if (templateRoot == null)
            {
                CoreLogger.LogStatus("Fail", ConsoleColor.Red);
                throw new Microsoft.Test.TestValidationException("The template root element could not be found through the templated control.");
            }
            else
            {
                CoreLogger.LogStatus("Pass", ConsoleColor.Green);
            }

            // Return template root.
            return templateRoot;
        }

        /// <summary>
        /// Get the root element of the template.
        /// </summary>
        static private FrameworkElement _GetTemplateRoot(FrameworkElement templatedControl)
        {
            FrameworkElement templateRoot = null;
            
            CoreLogger.LogStatus("Verifying the templated control's visual child is the template root...");

            // Get template root as first visual child of control.
            templateRoot = (FrameworkElement)VisualTreeHelper.GetChild(templatedControl, 0);

            if (templateRoot == null)
            {
                CoreLogger.LogStatus("Fail", ConsoleColor.Red);
                throw new Microsoft.Test.TestValidationException("Could not find template root in visual tree of control templated control.");
            }
                  
            return templateRoot;
        }

        /// <summary>
        /// Checks:
        /// 1. The templated element may be found by Id.
        /// 2. The template root cannot be found by Id outside the template.
        /// </summary>
        static private FrameworkElement _VerifyTemplatedElement(FrameworkElement fe)
        {
            CoreLogger.LogStatus("Verifying the templated element...");

            //
            // Get the templated control by Id.
            //
            CoreLogger.LogStatus("Checking the templated control is found by Id outside the template...");
            string id = String.Empty;
            id = "FrameworkElement";

            FrameworkElement templatedParent = (FrameworkElement)fe.FindName(id);
            if (templatedParent == null)
            {
                CoreLogger.LogStatus("Fail", ConsoleColor.Red);
                throw new Microsoft.Test.TestValidationException("Couldn't find FrameworkElement with id '" + id + "'.");
            }
            else
            {
                CoreLogger.LogStatus("Pass", ConsoleColor.Green);
            }

            return templatedParent;
        }

        /// <summary>
        /// Verify if the sources are looked up from the ChainedTriggers correctly
        /// </summary>
        static private void _VerifyChainedTriggers(DependencyObject resourcedItem)
        {            
            MouseHelper.Move((UIElement)resourcedItem, MouseLocation.Center);
            
            //
            // Compare actual background color with expected color.
            //
            
            // Grab actual color.
            Button testTarget = (Button)resourcedItem;
            Brush actual = (Brush)testTarget.Background;
            
            //// Compare with expected color.
            Brush expected = Brushes.Green;

            CoreLogger.LogStatus("Comparing Backgrounds...", ConsoleColor.Green);

            string actualValue1 = _ConvertToString((Brush)actual);
            string expectedValue1 = _ConvertToString((Brush)expected);
                        
            if (actualValue1 != expectedValue1)
            {
                // Test Case Broken.
                throw new Microsoft.Test.TestValidationException("FAIL: Background Colors did not get looked up properly from resource. GOT: " + actualValue1 + " EXPECTED: " + expectedValue1);
            }            
        }


        /// <summary>
        /// Get resourced dependency object under test root.
        /// </summary>
        static private DependencyObject _GetResourcedItem(FrameworkElement root)
        {
            CoreLogger.LogStatus("Getting the resourced element...");

            string name = "FrameworkElement";

            DependencyObject resourcedItem = (DependencyObject)root.FindName(name);

            if (resourcedItem == null)
            {
                throw new Microsoft.Test.TestValidationException("Couldn't find resourced item by name '" + name + "'.");
            }

            return resourcedItem;
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

        // Holds the model instance that works with these verifier routines.
        static private ChainedTriggersModelState s_state = null;
    }

}
