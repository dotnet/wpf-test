// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Verifies the tree, its properties, and any user interaction aspects
 *          of it (animation, triggers) for StyleModel.
 * Contributors: Microsoft
 *
 
  
 * Revision:         $Revision: 7 $
 
********************************************************************/
using System;
using System.Text;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;

using Avalon.Test.CoreUI.Common;
//using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Threading;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.PropertyEngine.StyleModel
{
    /// <summary>
    /// Verification routines for StyleModel.
    /// </summary>
    static class StyleModelVerifiers
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
            s_state = new StyleModelState(CoreState.Load());

            // Get the styledItem (it may be in the template).
            DependencyObject styledItem = _GetStyledItem((FrameworkElement)root);

            // Verify untriggered properties on it
            // Perform verification based on model state.
            _VerifyUntriggeredProperties(styledItem);

            // Trigger any property triggers (mouse over)
            // verify property triggers
            _VerifyTriggerProperties(styledItem, root);

            // Trigger any event triggers (click?)
            // verify event triggers
            _VerifyEventTriggerProperties(styledItem, root);

            _VerifyDataTriggerProperties(styledItem, root);

            // Put mouse back in corner so it won't interfere with any new windows after validation.
            MouseHelper.MoveOnVirtualScreenMonitor();
            DispatcherHelper.DoEvents(200);

            return true;
        }

        /// <summary>
        /// Get styled dependency object under test root.
        /// </summary>
        static private DependencyObject _GetStyledItem(FrameworkElement root)
        {
            CoreLogger.LogStatus("Getting the styled element...");

            string name = "StyledItem_" + s_state.StyledItem;

            DependencyObject styledItem = (DependencyObject)root.FindName(name);

            if (styledItem == null)
            {
                throw new Microsoft.Test.TestValidationException("Couldn't find styled item by name '" + name + "'.");
            }

            return styledItem;
        }

        static private void _VerifyUntriggeredProperties(DependencyObject styledItem)
        {
            if (s_state.HasSetter == "None")
            {
                return;
            }

            if (s_state.RoutedEvent != "Loaded")
            {
                CoreLogger.LogStatus("Verifying the styled element has the correct initial colors");
                _VerifyProperties(styledItem, _PredictPropertyValues(_PropertyVerifications.UntriggeredProperties));
            }
            else
            {
                // Let Loaded storyboard run.
                DispatcherHelper.DoEvents(1000);
                //_VerifyProperties(styledItem, _PredictPropertyValues(_PropertyVerifications.EventTriggerProperties));
            }
        }

        static private void _VerifyTriggerProperties(DependencyObject styledItem, UIElement testRoot)
        {
            if (s_state.HasPropertyTrigger == "None")
            {
                return;
            }

            // 
            if (s_state.HasEventTrigger != "None")
            {
                return;
            }

            // Move mouse over the item.
            if (s_state.StyledItem == "FrameworkContentElement")
            {
                MouseHelper.Move((UIElement)LogicalTreeHelper.GetParent(styledItem), MouseLocation.Center);
            }
            else
            {
                MouseHelper.Move((UIElement)styledItem, MouseLocation.Center);
            }
            DispatcherHelper.DoEvents(1000);

            CoreLogger.LogStatus("Verifying the style triggers.");
            _VerifyProperties(styledItem, _PredictPropertyValues(_PropertyVerifications.TriggeredProperties));
        }

        static private void _VerifyEventTriggerProperties(DependencyObject styledItem, UIElement testRoot)
        {
            // 
            if (s_state.HasEventTrigger == "None")
            {
                return;
            }

            if (s_state.RoutedEvent != "Loaded")
            {
                // Move mouse over the item.
                if (s_state.StyledItem == "FrameworkContentElement")
                {
                    MouseHelper.Move((UIElement)LogicalTreeHelper.GetParent(styledItem), MouseLocation.Center);
                }
                else
                {
                    MouseHelper.Move((UIElement)styledItem, MouseLocation.Center);
                }
            }
            else
            {
                MouseHelper.MoveOnVirtualScreenMonitor();
            }

            DispatcherHelper.DoEvents(1000);

            CoreLogger.LogStatus("Verifying the style event triggers.");
            _VerifyProperties(styledItem, _PredictPropertyValues(_PropertyVerifications.EventTriggerProperties));
        }

        static private void _VerifyDataTriggerProperties(DependencyObject styledItem, UIElement root)
        {
            if (s_state.HasDataTrigger == "None")
            {
                return;
            }

            CoreLogger.LogStatus("Verifying the style data triggers.");
            // _VerifyProperties(styledItem, _PredictPropertyValues(_PropertyVerifications.DataTriggerProperties));
            System.Xml.XmlAttribute contentAttribute = (System.Xml.XmlAttribute)((ContentControl)styledItem).Content;
            if (contentAttribute.Value != "Goose Goose Goose")
            {
                throw new Microsoft.Test.TestValidationException("DataTrigger did not set correct value. Expected 'Goose Goose Goose'"
                    + " Actual: " + contentAttribute.Value);
            }
        }

        private enum _PropertyVerifications
        {
            UntriggeredProperties,
            TriggeredProperties,
            EventTriggerProperties,
            DataTriggerProperties,
        };

        /// <summary>
        /// Sets the expected properties and values for the template root depending on the kind of 
        /// verification that is currently in effect.
        /// </summary>
        /// <param name="verification">1 means Trigger. 2 means EventTrigger. 3 means EventSet. Anything else means nonoe.</param>
        /// <returns>A dictionary of properties and their values.</returns>
        static private IDictionary _PredictPropertyValues(_PropertyVerifications verification)
        {
            Hashtable properties = new Hashtable();

            //
            // Determine the properties to check based on model state..
            //
            DependencyProperty prop1 = null;

            if (s_state.StyledItem == "FrameworkElement")
            {
                prop1 = Control.BackgroundProperty;
                // prop2 = TextBlock.ForegroundProperty;
            }
            else if (s_state.StyledItem == "CustomFrameworkElement")
            {
                prop1 = Control.BackgroundProperty;
            }
            else if (s_state.StyledItem == "FrameworkContentElement")
            {
                prop1 = Bold.BackgroundProperty;
            }
            else
            {
                throw new NotSupportedException("The styled item type isn't supported: " + s_state.StyledItem + ".");
            }

            //
            // Set values of properties depending on 'verification'.
            //
            if (prop1 != null)
            {
                switch (verification)
                {
                    case _PropertyVerifications.UntriggeredProperties:
                        //properties[prop2] = Brushes.Green;
                        if (s_state.HasSetter == "InBaseStyle")
                        {
                            // Setter in base style only.
                            properties[prop1] = Brushes.Red;
                        }
                        else
                        {
                            // Setter in derived style only or overrides base style.
                            properties[prop1] = Brushes.Yellow;
                        }
                        if ((s_state.FreezableSetterValue == "StaticResource") || (s_state.FreezableSetterValue == "DynamicResource"))
                        {
                            if (s_state.FreezableSetter)
                                properties[prop1] = Brushes.Red;
                        }
                        break;

                    case _PropertyVerifications.TriggeredProperties:
                        if (s_state.HasPropertyTrigger == "InBaseStyle")
                        {
                            properties[prop1] = Brushes.Green;
                        }
                        else
                        {
                            // Setter in derived style only or overrides base style.
                            properties[prop1] = Brushes.Orange;
                        }
                        if ((s_state.FreezableSetterValue == "StaticResource") || (s_state.FreezableSetterValue == "DynamicResource"))
                        {
                            if (s_state.FreezableTriggerSetter)
                                properties[prop1] = Brushes.Red;
                        }
                        //else
                        //{
                        //    // goto _PropertyVerifications.UntriggeredProperties;
                        //}
                        break;

                    case _PropertyVerifications.EventTriggerProperties:
                        if (s_state.HasEventTrigger == "InBaseStyle")
                        {
                            properties[prop1] = Brushes.Maroon;
                        }
                        else
                        {
                            // Setter in derived style only or overrides base style.
                            properties[prop1] = Brushes.DarkBlue;
                        }
                        break;
                    default:
                        throw new NotSupportedException(verification.ToString());
                }
            }

            // Return collection.
            return properties;
        }

        /// <summary>
        /// Loops through the given properties and values to verify they
        /// match actual current property values.
        /// </summary>
        static private void _VerifyProperties(DependencyObject element, IDictionary properties)
        {
            string id = TreeHelper.GetNodeId(element);
            if (id == "")
            {
                id = element.GetType().Name;
            }

            foreach (DependencyProperty prop in properties.Keys)
            {
                string propstr = _ConvertToString(prop);
                string expectedVal = _ConvertToString(properties[prop]);
                string actualVal = _ConvertToString(element.GetValue(prop));

                CoreLogger.LogStatus("Verifying " + propstr + " equals " + expectedVal + "...");

                if (expectedVal != actualVal)
                {
                    throw new Microsoft.Test.TestValidationException(
                        propstr + " of '" + id + "' doesn't have the expected value. Expected: " + expectedVal + ". Actual: " + actualVal + ".");
                }
            }
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
        static private StyleModelState s_state = null;
    }

}

