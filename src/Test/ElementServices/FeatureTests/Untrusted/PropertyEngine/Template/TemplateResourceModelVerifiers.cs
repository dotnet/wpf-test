// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Verifies the tree and any user interaction aspects
 *          of it (animation, triggers) for TemplateResourceModel.
 * Contributors: Microsoft
 *
 
  
 * Revision:         $Revision: 7 $
 
********************************************************************/
using System;
using System.Collections;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using Microsoft.Test.Logging;
using Microsoft.Test.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
//using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Threading;

namespace Avalon.Test.CoreUI.PropertyEngine.TemplateResources
{
    /// <summary>
    /// Verification routines for TemplateResourceModel.
    /// </summary>
    static class TemplateResourceModelVerifiers
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
            s_state = new TemplateResourceModelState(CoreState.Load());

            //
            // Get templated control.
            //
            FrameworkElement templatedElement = _GetTemplatedElement((FrameworkElement)root);

            //
            // Get the template tree root by searching the templated control's template property.
            //
            FrameworkElement templateRoot = _GetTemplateTree(templatedElement);

            if (s_state.HasBrush != "None")
            {
                _VerifyBrush(templatedElement, templateRoot);
            }

            if (s_state.HasVisualBrush != "None")
            {
                _VerifyVisualBrush(templatedElement, templateRoot);
            }

            if (s_state.HasStoryboard != "None")
            {
                _VerifyStoryboard(templatedElement, templateRoot);
            }

            if (s_state.HasStyle!= "None")
            {
                _VerifyStyle(templatedElement, templateRoot);
            }

            if (s_state.HasViewport3D != "None")
            {
                _VerifyViewport3D(templatedElement, templateRoot);
            }

            if (s_state.HasXmlDataSource != "None")
            {
                _VerifyXmlDataSource(templatedElement, templateRoot);
            }

            if (s_state.HasPropertyTrigger != "None")
            {
                _VerifyPropertyTrigger(templatedElement, templateRoot);
            }

            if (s_state.HasConflictingResourceName != "None")
            {
                _VerifyConflictingResourceName(templatedElement, templateRoot);
            }

            if (s_state.HasTemplate != "None")
            {
               _VerifyTemplate(templatedElement, templateRoot);
            }

            if (s_state.HasStyleBasedOn != "None")
            {
                _VerifyHasStyleBasedOn(templatedElement, templateRoot);
            }

            if (s_state.ContentHasResources == "true")
            {
                _VerifyContentHasResource(templatedElement, templateRoot);
            }

            // Put mouse back in corner so it won't interfere with any new windows after validation.
            MouseHelper.MoveOnVirtualScreenMonitor();

            return true;
        }

        /// <summary>
        /// Checks:
        /// 1. The templated element may be found by Id.
        /// 2. The template root cannot be found by Id outside the template.
        /// </summary>
        static private FrameworkElement _GetTemplatedElement(FrameworkElement fe)
        {
            CoreLogger.LogStatus("Verifying the templated element...");

            //
            // Get the templated control by Id.
            //
            CoreLogger.LogStatus("Verifying the templated control is found by Id outside the template...");
            string id = "TemplatedControlType_" + s_state.TemplatedControlType;

            FrameworkElement templatedParent = (FrameworkElement)fe.FindName(id);
            if (templatedParent == null)
            {
                throw new Microsoft.Test.TestValidationException("Couldn't find templated FrameworkElement by id '" + id + "'.");
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
        static private FrameworkElement _GetTemplateTree(FrameworkElement templatedControl)
        {
            FrameworkElement templateRoot = null;
            if (s_state.TemplateType == "ControlTemplate")
            {
                CoreLogger.LogStatus("Getting the template root...", ConsoleColor.Cyan);

                // Get template root as first visual child of control.
                templateRoot = (FrameworkElement)VisualTreeUtils.GetChild(templatedControl, 0);

                if (templateRoot == null)
                {
                    throw new Microsoft.Test.TestValidationException("Could not find template root in visual tree of control templated control.");
                }
                else
                {
                    CoreLogger.LogStatus("    Pass", ConsoleColor.Green);
                }
            }

            else
            {
                throw new NotSupportedException("Unsupported template type " + s_state.TemplateType);
            }

            return templateRoot;
        }

        static private void _VerifyBrush(FrameworkElement templatedControl, FrameworkElement templateRoot)
        {
            CoreLogger.LogStatus("Verifying Brush in Template resources.", ConsoleColor.Cyan);

            // Get the tree element using the brush resources.
            Button templateElement = (Button)templateRoot.FindName("TemplateTreeItem_Brush");

            _VerifyProperties(templateElement, _PredictPropertyValues("Brush"));

            CoreLogger.LogStatus("    Pass", ConsoleColor.Green);
        }

        static private void _VerifyVisualBrush(FrameworkElement templatedControl, FrameworkElement templateRoot)
        {
            CoreLogger.LogStatus("Verifying VisualBrush in Template resources.", ConsoleColor.Cyan);

            // Get the tree element using the visual brush resource.
            Button templateElement = (Button)templateRoot.FindName("TemplateTreeItem_VisualBrush");
            if (templateElement == null)
            {
                throw new Exception("Could not find TemplateTreeItem_VisualBrush");
            }

            _VerifyProperties(templateElement, _PredictPropertyValues("VisualBrush"));

            CoreLogger.LogStatus("    Pass", ConsoleColor.Green);
        }

        static private void _VerifyStoryboard(FrameworkElement templatedControl, FrameworkElement templateRoot)
        {
            CoreLogger.LogStatus("Verifying Storyboard in Template resources.", ConsoleColor.Cyan);

            // Get the tree element that has a storyboarded property.
            Button templateElement = (Button)templateRoot.FindName("TemplateTreeItem_Storyboard");

            CoreLogger.LogStatus("Verifying element with event triggered storyboard resource has correct initial colors");
            _VerifyProperties(templateElement, _PredictPropertyValues("StoryboardBefore"));

            MouseHelper.Move(templateElement, MouseLocation.Center);
            MouseHelper.Click();
            DispatcherHelper.DoEvents(1000);
            DispatcherHelper.DoEvents(1000);

            CoreLogger.LogStatus("Verifying event triggered storyboard resource has correctly changed colors");
            _VerifyProperties(templateElement, _PredictPropertyValues("StoryboardAfter"));

            CoreLogger.LogStatus("    Pass", ConsoleColor.Green);
        }

        static private void _VerifyStyle(FrameworkElement templatedControl, FrameworkElement templateRoot)
        {
        
            CoreLogger.LogStatus("Verifying Style in Template resources.", ConsoleColor.Cyan);

            // Get the tree element using the brush resources.
            Button templateElement = (Button)templateRoot.FindName("TemplateTreeItem_Style");

            _VerifyProperties(templateElement, _PredictPropertyValues("Style"));

            CoreLogger.LogStatus("    Pass", ConsoleColor.Green);
        }


        static private void _VerifyViewport3D(FrameworkElement templatedControl, FrameworkElement templateRoot)
        {

            CoreLogger.LogStatus("Verifying Viewport3D in Template resources.", ConsoleColor.Cyan);


            CoreLogger.LogStatus("    Pass", ConsoleColor.Green);
        }

        static private void _VerifyXmlDataSource(FrameworkElement templatedControl, FrameworkElement templateRoot)
        {
            CoreLogger.LogStatus("Verifying XmlDataSource in Template resources.", ConsoleColor.Cyan);

            Button templateElement = (Button)templateRoot.FindName("TemplateTreeItem_XmlDataSource");

            _VerifyProperties(templateElement, _PredictPropertyValues("XmlDataSource"));

            CoreLogger.LogStatus("    Pass", ConsoleColor.Green);
        }

        static private void _VerifyPropertyTrigger(FrameworkElement templatedControl, FrameworkElement templateRoot)
        {
            CoreLogger.LogStatus("Verifying PropertyTrigger using resource in Template resources.", ConsoleColor.Cyan);

            // Get the tree element that has the property trigger.
            Button templateElement = (Button)templateRoot.FindName("TemplateTreeItem_PropertyTrigger");

            CoreLogger.LogStatus("    Verifying correct initial properties");
            _VerifyProperties(templateElement, _PredictPropertyValues("PropertyTriggerBefore"));

            MouseHelper.Move(templateElement, MouseLocation.Center);
            DispatcherHelper.DoEvents(1000);

            CoreLogger.LogStatus("    Verifying correct triggered properties");
            _VerifyProperties(templateElement, _PredictPropertyValues("PropertyTriggerAfter"));

            CoreLogger.LogStatus("    Pass", ConsoleColor.Green);
        }

        static private void _VerifyConflictingResourceName(FrameworkElement templatedControl, FrameworkElement templateRoot)
        {
            CoreLogger.LogStatus("Verifying conflicting resource names in Template resources and page level resources.", ConsoleColor.Cyan);

            // Get the tree element using the resource.
            Button templateElement = (Button)templateRoot.FindName("TemplateTreeItem_ConflictingResourceName");

            _VerifyProperties(templateElement, _PredictPropertyValues("ConflictingResourceName"));

            CoreLogger.LogStatus("    Pass", ConsoleColor.Green);
        }

        static private void _VerifyTemplate(FrameworkElement templatedElement, FrameworkElement templateRoot)
        {
            CoreLogger.LogStatus("Verifying BasedOn Style in Template resources.", ConsoleColor.Cyan);

            // Get the tree element using the brush resources.
            Button templateElement = (Button)templateRoot.FindName("TemplateTreeItem_Template");

            // Get the templateRoot of this templateElement
            FrameworkElement templateTreeTemplateRoot = _GetTemplateTree(templateElement);

            Button templateResourceTemplateTreeItem = (Button)templateTreeTemplateRoot.FindName("TemplateResourceTemplateTreeItem");
            
            // Verify the Template.Resource template has been applied.
            _VerifyProperties(templateResourceTemplateTreeItem, _PredictPropertyValues("Template"));



            CoreLogger.LogStatus("    Pass", ConsoleColor.Green);
        }

        static private void _VerifyHasStyleBasedOn(FrameworkElement templatedElement, FrameworkElement templateRoot)
        {
            CoreLogger.LogStatus("Verifying BasedOn Style in Template resources.", ConsoleColor.Cyan);

            // Get the tree element using the brush resources.
            Button templateElement = (Button)templateRoot.FindName("TemplateTreeItem_BasedOnStyle");

            _VerifyProperties(templateElement, _PredictPropertyValues("BasedOnStyle"));

            CoreLogger.LogStatus("    Pass", ConsoleColor.Green);
        }

        static private void _VerifyContentHasResource(FrameworkElement templatedElement, FrameworkElement templateRoot)
        {
            CoreLogger.LogStatus("Verifying element with resource section in Template.", ConsoleColor.Cyan);

            Button templateElement = (Button)templateRoot.FindName("TemplateTreeItem_ContentHasResourcesButton");
            _VerifyProperties(templateElement, _PredictPropertyValues("ContentHasResources"));

            CoreLogger.LogStatus("    Pass", ConsoleColor.Green);
        }

        /// <summary>
        /// Sets the expected properties and values for the template root depending on the kind of 
        /// verification that is currently in effect.
        /// </summary>
        /// <param name="resourceItemName">1 means Trigger. 2 means EventTrigger. 3 means EventSet. Anything else means nonoe.</param>
        /// <returns>A dictionary of properties and their values.</returns>
        static private IDictionary _PredictPropertyValues(string resourceItemName)
        {
            Hashtable properties = new Hashtable();

            //
            // Determine the properties to check.
            //
            DependencyProperty prop1 = null;
            DependencyProperty prop2 = null;

            if (resourceItemName == "Brush")
            {
                prop1 = Control.BackgroundProperty;
                prop2 = Control.ForegroundProperty;
            }
            else if (resourceItemName == "VisualBrush")
            {
                prop1 = Control.BackgroundProperty;
            }
            else if ((resourceItemName == "StoryboardBefore") || (resourceItemName == "StoryboardAfter"))
            {
                prop1 = Control.BackgroundProperty;
            }
            else if (resourceItemName == "Style")
            {
                prop1 = Control.BackgroundProperty;
                prop2 = Control.ForegroundProperty;
            }
            else if (resourceItemName == "XmlDataSource")
            {
                prop1 = Control.BackgroundProperty;
            }
            else if ((resourceItemName == "PropertyTriggerBefore") || (resourceItemName == "PropertyTriggerAfter"))
            {
                prop1 = Control.BackgroundProperty;
                prop2 = Control.ForegroundProperty;
            }
            else if (resourceItemName == "ConflictingResourceName")
            {
                prop1 = Control.BackgroundProperty;
            }
            else if (resourceItemName == "Template")
            {
                prop1 = Control.BackgroundProperty;
            }
            else if (resourceItemName == "BasedOnStyle")
            {
                prop1 = Control.ForegroundProperty;
                prop2 = Control.BackgroundProperty;
            }
            else if (resourceItemName == "ContentHasResources")
            {
                prop1 = Control.BackgroundProperty;
            }
            else
            {
                throw new NotSupportedException("This resource type not supported, " + resourceItemName + ".");
            }

            //
            // Set values of properties depending on 'verification'.
            //
            if (resourceItemName == "Brush")
            {
                properties[prop1] = Brushes.Gray;
                properties[prop2] = Brushes.Crimson;
            }
            else if (resourceItemName == "VisualBrush")
            {
                properties[prop1] = "System.Windows.Media.VisualBrush";
            }
            else if (resourceItemName == "StoryboardBefore")
            {
                properties[prop1] = Brushes.Yellow;
            }
            else if (resourceItemName == "StoryboardAfter")
            {
                properties[prop1] = Brushes.Red;
            }
            else if (resourceItemName == "Style")
            {
                properties[prop1] = Brushes.Black;
                properties[prop2] = Brushes.White;
            }
            else if (resourceItemName == "XmlDataSource")
            {
                properties[prop1] = Brushes.Green;
            }
            else if (resourceItemName == "PropertyTriggerBefore")
            {
                properties[prop1] = Brushes.Orange;
                properties[prop2] = Brushes.Black;
            }
            else if (resourceItemName == "PropertyTriggerAfter")
            {
                properties[prop1] = Brushes.Black;
                properties[prop2] = Brushes.Orange;
            }
            else if (resourceItemName == "ConflictingResourceName")
            {
                properties[prop1] = Brushes.Pink;
            }
            else if (resourceItemName == "Template")
            {
                properties[prop1] = Brushes.Cyan;
            }
            else if (resourceItemName == "BasedOnStyle")
            {
                properties[prop1] = Brushes.White;
                properties[prop2] = Brushes.Brown;
            }
            else if (resourceItemName == "ContentHasResources")
            {
                properties[prop1] = Brushes.Cyan;
            }
            else
            {
                throw new NotSupportedException("This resource type not supported, " + resourceItemName + ".");
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
        static private TemplateResourceModelState s_state = null;
    }

}


