// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Verifies the tree and any user interaction aspects
 *          of it (animation, triggers) for MergedDictionariesModel.
 *
 *
 
  
 * Revision:         $Revision: 12 $
 
 * Filename:         $Source: //depot/vbl_wcp_avalon_dev/windowstest/client/wcptests/Core/Framework/BVT/Resources/MergedDictionariesVerifiers.cs $
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

namespace Avalon.Test.CoreUI.Resources.MergedDictionaries
{
    /// <summary>
    /// Verification routines for MergedDictionariesModel.
    /// </summary>
    static class MergedDictionariesModelVerifiers
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
            s_state = new MergedDictionariesModelState(CoreState.Load());

            // Get the resourcedItem (it may be in the template).
            DependencyObject resourcedItem = _GetResourcedItem((FrameworkElement)root);

            //
            // Test Lookup
            //

            // Do regular verification.
            _VerifyMergedDictionaries(resourcedItem);
    

            return true;
        }

        /// <summary>
        /// Verify if the sources are looked up from the MergedDictionaries correctly
        /// </summary>
        static private void _VerifyMergedDictionaries(DependencyObject resourcedItem)
        {
            //
            // Compare actual background color with expected color.
            //

            // Verify Behaviour for freezable types
            //if (_state.ResourceType == "Freezable")
            //{
                // If a resource in the incorrect precedence is used the color is red - otherwise it should be green.
                
                // Grab actual color.
                Button testTarget = (Button)resourcedItem;
                Brush actual = (Brush)testTarget.Background;
                
                // Compare with expected color.
                Brush expected = Brushes.Green;

                // In these cases the non-merged dictionary resource should take precedence.
                if (s_state.DuplicateKey == "InternalExternal" ||
                    (s_state.DuplicateKey == "InternalForeign" &&
                    (s_state.ResourceLocation == "PageResources" || s_state.ResourceLocation == "ApplicationResources")) ||
                    s_state.DuplicateKey == "InternalLocal")
                    
                {
                    expected = Brushes.Red;
                }

                CoreLogger.LogStatus("Comparing Backgrounds...", ConsoleColor.Green);

                string actualValue1 = _ConvertToString((Brush)actual);
                string expectedValue1 = _ConvertToString((Brush)expected);

                if (actualValue1 != expectedValue1)
                {
                    // Test Case Broken.
                    throw new Microsoft.Test.TestValidationException("FAIL: Background Colors did not get looked up properly from resource. GOT: " + actualValue1 + " EXPECTED: " + expectedValue1);
                }
            //}           
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
        static private MergedDictionariesModelState s_state = null;
    }

}
