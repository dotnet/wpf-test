// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Verifies the tree and any user interaction aspects
 *          of it (animation, triggers) for PropertyTriggerModel.
 * Contributors: Microsoft
 *
 
  
 * Revision:         $Revision: 12 $
 
 * Filename:         $Source: //depot/vbl_wcp_avalon_dev/windowstest/client/wcptests/Core/Framework/BVT/PropertyEngine/Template/PropertyTriggerModelVerifiers.cs $
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

namespace Avalon.Test.CoreUI.PropertyEngine.PropertyTrigger
{
    /// <summary>
    /// Verification routines for PropertyTriggerModel.
    /// </summary>
    static class PropertyTriggerModelVerifiers
    {
        /// <summary>
        /// Verification entry point called after the test tree is rendered.
        /// The method name is set on the Verifier property, either in
        /// xaml or programmatically.
        /// </summary>
        public static bool StartVerify(UIElement root)
        {
            CoreLogger.LogStatus("In StartVerify()...");

            //// Load the model state.            
            s_state = new PropertyTriggerModelState(CoreState.Load());

            if (s_state.StyleLocation != "None")
            {
                // Get the styledItem (it may be in the template).
                DependencyObject styledItem = _GetStyledItem((FrameworkElement)root);

                // verify property triggers - storyBoard actions
                _VerifyStoryBoard(styledItem, root);

                // verify property triggers
                _VerifyTriggerProperties(styledItem, root);
            }
            else
            {
                ////
                //// Verify and get templated control.
                ////
                FrameworkElement templatedElement = _VerifyTemplatedElement((FrameworkElement)root);

                ////
                //// Verify the template tree can be found by searching the templated control's template property.
                ////
                FrameworkElement templateRoot = _VerifyTemplateTree(templatedElement);

                ////
                //// Verify the template child can be found from the template root.
                ////
                DependencyObject templateChild = _VerifyTemplateChild(templateRoot);

                // verify property triggers - storyBoard actions
                _VerifyTemplateStoryBoard(templatedElement, templateRoot, templateChild, root);

                //
                // Verify Triggers
                //
                _VerifyPropertyTriggers(templatedElement, templateRoot, templateChild, root);                

                MouseHelper.MoveOnVirtualScreenMonitor();
            }

            return true;
        }

        /// <summary>
        /// Get styled dependency object under test root.
        /// </summary>
        static private DependencyObject _GetStyledItem(FrameworkElement root)
        {
            CoreLogger.LogStatus("Getting the styled element...");

            string name = "StyledItem_FrameworkElement";

            DependencyObject styledItem = (DependencyObject)root.FindName(name);

            if (styledItem == null)
            {
                throw new  Microsoft.Test.TestValidationException("Couldn't find styled item by name '" + name + "'.");
            }

            return styledItem;
        }

        static private void _VerifyTriggerProperties(DependencyObject styledItem, UIElement testRoot)
        {
            // Move mouse over the item.
            MouseHelper.Move((UIElement)styledItem, MouseLocation.Center);
            _MatchConditions(styledItem);
            DispatcherHelper.DoEvents(1000);

            CoreLogger.LogStatus("Verifying the style triggers.");
            _VerifyProperties(styledItem, _PredictPropertyValues());
            MouseHelper.Move((UIElement)testRoot, MouseLocation.Bottom);
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
            id = "TemplatedControlType_ContentControl";

            FrameworkElement templatedParent = (FrameworkElement)fe.FindName(id);
            if (templatedParent == null)
            {
                CoreLogger.LogStatus("Fail", ConsoleColor.Red);
                throw new  Microsoft.Test.TestValidationException("Couldn't find FrameworkElement with id '" + id + "'.");
            }
            else
            {
                CoreLogger.LogStatus("Pass", ConsoleColor.Green);
            }

            return templatedParent;
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
                throw new  Microsoft.Test.TestValidationException("The template root element could not be found through the templated control.");
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
            if (s_state.TemplateType == "ControlTemplate")
            {
                CoreLogger.LogStatus("Verifying the templated control's visual child is the template root...");

                // Get template root as first visual child of control.
                templateRoot = (FrameworkElement)VisualTreeHelper.GetChild(templatedControl, 0);

                if (templateRoot == null)
                {
                    CoreLogger.LogStatus("Fail", ConsoleColor.Red);
                    throw new  Microsoft.Test.TestValidationException("Could not find template root in visual tree of control templated control.");
                }
            }
            else if (s_state.TemplateType == "DataTemplate")
            {
                CoreLogger.LogStatus("     Searching Data template for first content presenter.");
                // Find content presenter in visual tree.
                FrameworkElement templatedContent = getFirstContentPresenter(templatedControl);

                if (templatedContent == null)
                {
                    CoreLogger.LogStatus("Fail", ConsoleColor.Red);
                    throw new  Microsoft.Test.TestValidationException("Could not find content presenter in data templated control");
                }

                // Get its visual tree.
                templateRoot = (FrameworkElement)VisualTreeHelper.GetChild(templatedContent, 0);

                if (templateRoot == null)
                {
                    CoreLogger.LogStatus("Fail", ConsoleColor.Red);
                    throw new  Microsoft.Test.TestValidationException("Could not find template root in visual tree of data templated control.");
                }
            }
            else
            {
                throw new NotSupportedException("Unsupported template type " + s_state.TemplateType);
            }

            return templateRoot;
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
                throw new  Microsoft.Test.TestValidationException("Could not find template tree child with name " + childName);
            }
            else
            {
                CoreLogger.LogStatus("    Found.");
            }
            return child;

        }

        /// <summary>
        /// Checks:
        /// 1. IsMouseOver on the templated control causes the correct change
        ///    in the property values.
        /// </summary>
        static private void _VerifyPropertyTriggers(FrameworkElement templatedParent, DependencyObject templateRoot, DependencyObject templateChild, UIElement testRoot)
        {
            DependencyObject src = null;

            if (s_state.TriggerSource == "TemplatedParent")
            {
                CoreLogger.LogStatus("Verifying the templated control's template property-triggered properties...", ConsoleColor.Cyan);
                src = templatedParent;
            }
            else if (s_state.TriggerSource == "TemplateRoot")
            {
                CoreLogger.LogStatus("Verifying the template root's property-triggered properties...", ConsoleColor.Cyan);
                src = templateRoot;
            }
            else
            {
                src = templateChild;
            }
            
            //Meet the Required Conditions
            // Move mouse over control.
            if (s_state.TemplateType == "ControlTemplate")
            {
                MouseHelper.Move(templatedParent, MouseLocation.Center);
            }
            else
            {
                MouseHelper.Move((UIElement)templateRoot, MouseLocation.Center);
            }
            _MatchConditions(src);            
            DispatcherHelper.DoEvents(1000);

            // Verify the property values match the declared triggered values.
            CoreLogger.LogStatus("Verifying property trigger has correctly changed element color");

            switch (s_state.TriggerTarget)
            {
                case "TemplatedParent":
                    _VerifyProperties(templatedParent, _PredictPropertyValues());
                    break;
                case "TemplateRoot":
                    _VerifyProperties(templateRoot, _PredictPropertyValues());
                    break;
                case "TemplateChild":
                    _VerifyProperties(templateChild, _PredictPropertyValues());
                    break;
                default:
                    throw new NotSupportedException("Trigger target " + s_state.TriggerTarget + " not supported.");
            }
            //MouseHelper.Move((UIElement)templateRoot, MouseLocation.Bottom);
            MouseHelper.Move((UIElement)testRoot, MouseLocation.Bottom);
        }
        /// <summary>
        /// Sets the expected properties and values for the template root depending on the kind of 
        /// verification that is currently in effect.
        /// </summary>
        /// <param name="src">The Item whose property values need updating.</param>
        static private void _MatchConditions(DependencyObject src)
        {
            if (Convert.ToInt32(s_state.NumConditions) >= 2)
            {
                src.SetValue(Control.FontFamilyProperty, new FontFamily("Symbol"));
            }
            if (Convert.ToInt32(s_state.NumConditions) == 3)
            {
                src.SetValue(Control.FontSizeProperty, (Double)20.0);
            }

        }

        /// <summary>
        /// Verifies whether the storyboard actions were triggered off correctly
        /// </summary>
        static private void _VerifyTemplateStoryBoard(FrameworkElement templatedParent, DependencyObject templateRoot, DependencyObject templateChild, UIElement testRoot)
        {
            if (s_state.HasStoryBoardActions == "False") return;


            DependencyObject src = null;

            if (s_state.TriggerSource == "TemplatedParent")
            {
                CoreLogger.LogStatus("Verifying the templated control's template property-triggered properties...", ConsoleColor.Cyan);
                src = templatedParent;
            }
            else if (s_state.TriggerSource == "TemplateRoot")
            {
                CoreLogger.LogStatus("Verifying the template root's property-triggered properties...", ConsoleColor.Cyan);
                src = templateRoot;
            }
            else
            {
                src = templateChild;
            }

            //Meet the Required Conditions
            // Move mouse over control.
            if (s_state.TemplateType == "ControlTemplate")
            {
                MouseHelper.Move(templatedParent, MouseLocation.Center);
            }
            else
            {
                MouseHelper.Move((UIElement)templateRoot, MouseLocation.Center);
            }
            DispatcherHelper.DoEvents(1000);

            // Verify the property values match the declared triggered values.
            CoreLogger.LogStatus("Verifying property trigger has correctly changed element color");

            switch (s_state.TriggerTarget)
            {
                case "TemplatedParent":
                    _VerifyProperties(templatedParent, _PredictStoryBoardValues(true));
                    break;
                case "TemplateRoot":
                    _VerifyProperties(templateRoot, _PredictStoryBoardValues(true));
                    break;
                case "TemplateChild":
                    _VerifyProperties(templateChild, _PredictStoryBoardValues(true));
                    break;
                default:
                    throw new NotSupportedException("Trigger target " + s_state.TriggerTarget + " not supported.");
            }

            //Move mouse-out
            MouseHelper.Move((UIElement)testRoot, MouseLocation.Bottom);

            //Verify StoryBoard - ExitActions
            CoreLogger.LogStatus("Verifying the StoryBoard - EnterActions.");
            switch (s_state.TriggerTarget)
            {
                case "TemplatedParent":
                    _VerifyProperties(templatedParent, _PredictStoryBoardValues(false));
                    break;
                case "TemplateRoot":
                    _VerifyProperties(templateRoot, _PredictStoryBoardValues(false));
                    break;
                case "TemplateChild":
                    _VerifyProperties(templateChild, _PredictStoryBoardValues(false));
                    break;
                default:
                    throw new NotSupportedException("Trigger target " + s_state.TriggerTarget + " not supported.");
            }
        }

        /// <summary>
        /// Verifies whether the storyboard actions were triggered off correctly
        /// </summary>
        static private void _VerifyStoryBoard(DependencyObject styledItem, UIElement testRoot)
        {
            if (s_state.HasStoryBoardActions == "False") return;

            // Move mouse over the item.
            MouseHelper.Move((UIElement)styledItem, MouseLocation.Center);
            DispatcherHelper.DoEvents(1000);
            
            //Verify StoryBoard - EnterActions
            CoreLogger.LogStatus("Verifying the StoryBoard - EnterActions.");
            _VerifyProperties(styledItem, _PredictStoryBoardValues(true));
            
            //Move mouse-out
            MouseHelper.Move((UIElement)testRoot, MouseLocation.Bottom);

            //Verify StoryBoard - ExitActions
            CoreLogger.LogStatus("Verifying the StoryBoard - EnterActions.");
            _VerifyProperties(styledItem, _PredictStoryBoardValues(false));
        }

        /// <summary>
        /// Sets the expected properties and values for the template root depending on the kind of 
        /// verification that is currently in effect.
        /// </summary>
        /// <parameter> If the mouse is on the source element </parameter>
        /// <returns>A dictionary of properties and their values.</returns>
        static private IDictionary _PredictStoryBoardValues(bool isEnter)
        {
            Hashtable properties = new Hashtable();

            //
            // Determine the properties to check.
            //
            DependencyProperty prop1 = Control.OpacityProperty;

            if (isEnter)
            {
                properties[prop1] = (Double)0.0;
            }
            else
            {
                properties[prop1] = (Double)0.5;
            }

            // Return collection.
            return properties;
        }


        /// <summary>
        /// Sets the expected properties and values for the template root depending on the kind of 
        /// verification that is currently in effect.
        /// </summary>
        /// <returns>A dictionary of properties and their values.</returns>
        static private IDictionary _PredictPropertyValues()
        {
            Hashtable properties = new Hashtable();

            //
            // Determine the properties to check.
            //
            DependencyProperty prop1 = null;
            DependencyProperty prop2 = null;
            DependencyProperty prop3 = null;
            DependencyProperty prop4 = null;
            DependencyProperty prop5 = null;
            DependencyProperty prop6 = null;
            DependencyProperty prop7 = null;
            DependencyProperty prop8 = null;
            DependencyProperty prop9 = null;
            DependencyProperty prop10 = null;
            DependencyProperty prop11 = null;
            DependencyProperty prop12 = null;

            int numSetters = Convert.ToInt32(s_state.NumSetters);
            int numSingleTrigs = Convert.ToInt32(s_state.NumSingleTrigs);
            int numMultiTrigs = Convert.ToInt32(s_state.NumMultiTrigs);
                                   
            // Case One Single Trigger Only
            if (numSingleTrigs >= 1)
            {
                if (numSetters >= 1)
                {
                    prop1 = Control.BackgroundProperty;
                    properties[prop1] = Brushes.Yellow;
                }
                if (numSetters >= 2)
                {                    
                    prop2 = TextBlock.ForegroundProperty;
                    properties[prop2] = Brushes.Green;
                }
                if (numSetters == 3)
                {
                    prop3 = Control.WidthProperty;
                    properties[prop3] = (Double)40.0;
                }
            }
            if (numSingleTrigs >= 2)
            {
                if (numSetters >= 1)
                {
                    prop4 = Control.HeightProperty;
                    properties[prop4] = (Double) 60.0;
                }
                if (numSetters >= 2)
                {                    
                    prop5 = TextBlock.FontStretchProperty;
                    properties[prop5] = FontStretches.UltraCondensed;
                }
                if (numSetters == 3)
                {
                    prop6 = Control.BorderBrushProperty;
                    properties[prop6] = Brushes.Blue;
                }
            }

            if (numMultiTrigs >= 1)
            {
                if (numSetters >= 1)
                {
                    prop7 = Control.BorderBrushProperty;
                    properties[prop7] = Brushes.Blue;
                }
                if (numSetters >= 2)
                {
                    prop8 = TextBlock.FontStyleProperty;
                    properties[prop8] = FontStyles.Italic;
                }
                if (numSetters == 3)
                {
                    prop9 = Control.FontWeightProperty;
                    properties[prop9] = FontWeights.Bold;
                }
            }

            if (numMultiTrigs == 2)
            {
                if (numSetters >= 1)
                {
                    prop10 = Control.PaddingProperty;
                    properties[prop10] = new Thickness(0);
                }
                if (numSetters >= 2)
                {
                    prop11 = TextBlock.TextProperty;
                    properties[prop11] = "Hello";
                }
                if (numSetters == 3)
                {
                    prop12 = Control.FontStretchProperty;
                    properties[prop12] = FontStretches.UltraCondensed;
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
                    CoreLogger.LogStatus("Fail", ConsoleColor.Red);
                    throw new  Microsoft.Test.TestValidationException(
                        propstr + " of '" + id + "' doesn't have the expected value. Expected: " + expectedVal + ". Actual: " + actualVal + ".");
                }
                else
                {
                    CoreLogger.LogStatus("Pass", ConsoleColor.Green);
                }
            }
        }

        /// <summary>
        /// Find ContentPresenter in visual tree.
        /// Adapted from FindDataVisuals in ConnectedData\Common\Util.cs
        /// </summary>
        static internal FrameworkElement getFirstContentPresenter(FrameworkElement element)
        {
            if ((element is ContentPresenter) && !(element is ScrollContentPresenter)) return element;

            FrameworkElement cp = null;

            int count = VisualTreeHelper.GetChildrenCount(element);

            for (int i = 0; i < count; i++)
            {
                Visual child = (Visual)VisualTreeHelper.GetChild(element, i);
                if (child is FrameworkElement)
                    cp = getFirstContentPresenter((FrameworkElement)child);

                if (cp != null) return cp;
            }

            return null;
        }

        //static internal FrameworkElement findFirstFrameworkElement(FrameworkContentElement fce)
        //{
        //    FrameworkElement fe = null;

        //    foreach (object item in LogicalTreeHelper.GetChildren(fce))
        //    {
        //        if (item is FrameworkElement) return (FrameworkElement)item;

        //        if (item is FrameworkContentElement)
        //        {
        //            fe = findFirstFrameworkElement((FrameworkContentElement)item);
        //        }
            
        //        if (fe != null) return (FrameworkElement)fe;
        //    }

        //    return null;
        //}


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

        //// Holds the model instance that works with these verifier routines.
        static private PropertyTriggerModelState s_state = null;
    }

}
