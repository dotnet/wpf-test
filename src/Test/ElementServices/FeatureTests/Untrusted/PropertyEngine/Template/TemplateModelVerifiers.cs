// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Verifies the tree and any user interaction aspects
 *          of it (animation, triggers) for TemplateModel.
 * Contributors: Microsoft
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;
using System.Collections;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using Microsoft.Test.Logging;
using Microsoft.Test.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
//using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;

namespace Avalon.Test.CoreUI.PropertyEngine.Template
{
    /// <summary>
    /// Verification routines for TemplateModel.
    /// </summary>
    static class TemplateModelVerifiers
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
            s_state = new TemplateModelState(CoreState.Load());

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

            //
            // Verify Trigger if necessary.
            //
            if (s_state.HasPropertyTrigger)
            {
                _VerifyPropertyTriggers(templatedElement, templateRoot, templateChild);
            }

            //
            // Verify EventSet if necessary.
            //
            if (s_state.HasEventSet)
            {
                _VerifyEventSet(templatedElement, templateRoot);
            }

            //
            // Verify EventTrigger if necessary.
            //
            if (s_state.HasEventTrigger == "PreviewMouseDown")
            {
                _VerifyEventTriggers(templatedElement, templateRoot);
            }

            MouseHelper.MoveOnVirtualScreenMonitor();

            return true;
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
            id = "TemplatedControlType_" + s_state.TemplatedControlType;

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

            //
            // Verify the template root cannot be found by Id from the templated parent.
            //
            CoreLogger.LogStatus("Verifying the template root is NOT found by Id outside the template...");
            id = "TemplateRootType_" + s_state.TemplateRootType;
            object foundNode = templatedParent.FindName(id);
            if (foundNode != null)
            {
                CoreLogger.LogStatus("Fail", ConsoleColor.Red);
                throw new Microsoft.Test.TestValidationException("Found the template root Id from the templated parent. Id: " + id + ".");
            }
            
            CoreLogger.LogStatus("Pass", ConsoleColor.Green);
            
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
            // Verify the templated control has logical children (when it's supposed to).
            //

            if (!s_state.HasDataBinding)
            {
                CoreLogger.LogStatus("Templated control has a data binding, verifying logical children...");

                if (templatedControl is ContentControl)
                {
                    if (((ContentControl)templatedControl).Content == null)
                    {
                        CoreLogger.LogStatus("Fail", ConsoleColor.Red);
                        throw new Microsoft.Test.TestValidationException("The templated ContentControl has no logical children but does have free time and expendable income.");
                    }                    
                }

                if (s_state.TemplateType == "DataTemplate")
                {
                    if (templatedControl is ItemsControl)
                    {
                        if (((ItemsControl)templatedControl).Items.Count == 0)
                        {
                            CoreLogger.LogStatus("Fail", ConsoleColor.Red);
                            throw new Microsoft.Test.TestValidationException("The templated ItemsControl has no logical children.");
                        }
                    }
                }
                CoreLogger.LogStatus("Pass", ConsoleColor.Green);
            }
            else
            {
                // todo: verify data bound control does not have logical children
                // except for ItemsControl which must have logical children to get templated.
            }

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

            //
            // Verify the Id searching works correctly in the 
            // template tree.  
            // 









            //
            // Verify properties of template root are correct.
            // 
            CoreLogger.LogStatus("Verifying the template root's non-triggered properties...", ConsoleColor.Cyan);

            if (s_state.HasEventTrigger == "Loaded")
            {
                // Finish Loaded storyboard.
                DispatcherHelper.DoEvents(1000);

                _VerifyProperties(
                    templateRoot,
                    _PredictPropertyValues(2)
                );
            }
            else
            {
                CoreLogger.LogStatus("Verifying the template root element has the correct initial colors");
                _VerifyProperties(
                    templateRoot,
                    _PredictPropertyValues(0)
                );
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
                    throw new Microsoft.Test.TestValidationException("Could not find template root in visual tree of control templated control.");
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
                    throw new Microsoft.Test.TestValidationException("Could not find content presenter in data templated control");
                }

                // Get its visual tree.
                templateRoot = (FrameworkElement)VisualTreeHelper.GetChild(templatedContent, 0);

                if (templateRoot == null)
                {
                    CoreLogger.LogStatus("Fail", ConsoleColor.Red);
                    throw new Microsoft.Test.TestValidationException("Could not find template root in visual tree of data templated control.");
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

            if (s_state.TemplateChildType == "None")
            {
                return null;
            }

            string childName = "TemplateChildType_";
            
            // Choose which child name to search for.
            switch (s_state.TemplateChildType)
            {
                case "FrameworkElement":
                    if (s_state.HasDataBinding == true)
                    {
                        childName = "TemplateChildType_FrameworkElement_WithBinding";
                    }
                    else
                    {
                        childName = "TemplateChildType_FrameworkElement";
                    }
                    break;
                case "FrameworkContentElement":
                    // 
                    childName = "TemplateChildType_FrameworkContentElement";
                    break;
                case "Viewport3D":
                    childName = "TemplateChildType_Viewport3D";
                    break;
                case "IList":
                    childName = "TemplateChildType_IList";
                    break;
                case "Shape":
                    childName = "TemplateChildType_Shape";
                    break;
                case "CustomControl":
                    childName = "TemplateChildType_CustomControl";
                    break;
                default:
                    throw new NotSupportedException("Template child type " + s_state.TemplateChildType + " not supported.");
                    //break;
            }

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
        /// 1. IsMouseOver on the templated control causes the correct change
        ///    in the property values.
        /// </summary>
        static private void _VerifyPropertyTriggers(FrameworkElement templatedParent, DependencyObject templateRoot, DependencyObject templateChild)
        {
            if (s_state.TriggerTarget == "TemplatedParent")
            {
                CoreLogger.LogStatus("Verifying the templated control's template property-triggered properties...", ConsoleColor.Cyan);
            }
            else
            {
                CoreLogger.LogStatus("Verifying the template root's property-triggered properties...", ConsoleColor.Cyan);
            }

            // Move mouse over control.
            if (s_state.TemplateType == "ControlTemplate")
            {
                MouseHelper.Move(templatedParent, MouseLocation.Center);
            }
            else
            {
                MouseHelper.Move((UIElement)templateRoot, MouseLocation.Center);
            }

            DispatcherHelper.DoEvents(2000);
            // Verify the property values match the declared triggered values.
            CoreLogger.LogStatus("Verifying property trigger has correctly changed element color");

            switch (s_state.TriggerTarget)
            {
                case "TemplatedParent":
                    _VerifyProperties(templatedParent, _PredictPropertyValues(1)
                    );
                    break;
                case "TemplateRoot":
                    _VerifyProperties(templateRoot, _PredictPropertyValues(1));
                    break;
                case "TemplateChild":
                    _VerifyProperties(templateChild, _PredictPropertyValues(1));
                    break;
                default:
                    throw new NotSupportedException("Trigger target " + s_state.TriggerTarget + " not supported.");
            }

        }

        /// <summary>
        /// Checks:
        /// 1. MouseUp on the templated control causes the correct change
        ///    in the property values.
        /// </summary>
        static private void _VerifyEventTriggers(FrameworkElement templatedControl, DependencyObject templateRoot)
        {
            CoreLogger.LogStatus("Verifying the template root's event-triggered properties...", ConsoleColor.Cyan);

            // Move mouse over templated parent and click once.
            // Wait 2000ms after mouseup to let animation finish.
            if (s_state.TemplateType == "DataTemplate")
            {
                MouseHelper.Move((UIElement)templateRoot, MouseLocation.Center);                
            }
            else
            {
                MouseHelper.Move(templatedControl, MouseLocation.Center);                
            }

            MouseHelper.Click(MouseButton.Right);
            DispatcherHelper.DoEvents(3000);

            // Verify the property values match the end
            // values of the triggered storyboard animation.
            CoreLogger.LogStatus("Verifying event triggered storyboard animation has correctly changed colors");
            _VerifyProperties(
                templateRoot,
                _PredictPropertyValues(2)
            );
        }

        /// <summary>
        /// Checks:
        /// The OnMouseLeave handler is called when the mouse has left the templateRoot
        /// </summary>
        static private void _VerifyEventSet(FrameworkElement templatedControl, DependencyObject templateRoot)
        {
            CoreLogger.LogStatus("Verifying the template root's event handler is called...");

            // Move mouse over then off templated parent
            if (s_state.TemplateType == "ControlTemplate")
            {
                MouseHelper.Move(templatedControl, MouseLocation.Center);
                MouseHelper.MoveOutside(templatedControl, MouseLocation.Bottom);
            }
            else
            {
                MouseHelper.Move((UIElement)templateRoot, MouseLocation.Center);
                // MouseHelper.MoveOutside((UIElement)templateRoot, MouseLocation.Bottom);
                MouseHelper.MoveOutside(templatedControl, MouseLocation.Bottom);
            }
            
            // Verify event handler has been called
            Hashtable properties = new Hashtable();
            _VerifyProperties(
                templateRoot, 
                _PredictPropertyValues(3)
                );
        }


        /// <summary>
        /// Sets the expected properties and values for the template root depending on the kind of 
        /// verification that is currently in effect.
        /// </summary>
        /// <param name="verification">1 means Trigger. 2 means EventTrigger. 3 means EventSet. Anything else means nonoe.</param>
        /// <returns>A dictionary of properties and their values.</returns>
        static private IDictionary _PredictPropertyValues(int verification)
        {
            Hashtable properties = new Hashtable();

            //
            // Determine the properties to check.
            //
            DependencyProperty prop1 = null;
            DependencyProperty prop2 = null;

            if (s_state.TemplateRootType == "Viewport3D")
            {
                // No properties to check like this on Viewport3D
            }
            else if (s_state.TriggerTarget == "TemplatedParent")
            {
                prop1 = Control.BackgroundProperty;
                prop2 = TextBlock.ForegroundProperty;
            }
            else if ((s_state.TemplateRootType == "Control") ||
                (s_state.TemplateRootType == "CustomControl"))
            {
                prop1 = Control.BackgroundProperty;
                prop2 = TextBlock.ForegroundProperty;
            }
            else if (s_state.TemplateRootType == "Panel" ||
                     s_state.TemplateRootType == "CustomPanel")
            {
                prop1 = Panel.BackgroundProperty;
                prop2 = TextBlock.ForegroundProperty;
            }
            else if (s_state.TemplateRootType == "Decorator")
            {
                prop1 = Border.BackgroundProperty;
                prop2 = TextBlock.ForegroundProperty;
            }
            else if (s_state.TemplateRootType == "FrameworkContentElement")
            {
                prop1 = Panel.BackgroundProperty;
                prop2 = TextBlock.ForegroundProperty;
            }
            else if (s_state.TemplateRootType == "Grid")
            {
                prop1 = Panel.BackgroundProperty;
                prop2 = TextBlock.ForegroundProperty;
            }
            else
            {
                throw new NotSupportedException("The template root type isn't supported. TemplateRootType: " + s_state.TemplateRootType + ".");
            }

            if (verification == 1)
            {
                if (s_state.TriggerTarget == "TemplateChild")
                {
                    switch (s_state.TemplateChildType)
                    {
                        case "Shape":
                            prop1 = Shape.FillProperty;
                            prop2 = TextBlock.ForegroundProperty;
                            break;
                        case "FrameworkElement":
                            prop1 = Control.BackgroundProperty;
                            prop2 = TextBlock.ForegroundProperty;
                            break;
                        case "FrameworkContentElement":
                            prop1 = TextElement.BackgroundProperty;
                            prop2 = TextBlock.ForegroundProperty;
                            break;
                        case "CustomControl":
                            prop1 = Control.BackgroundProperty;
                            prop2 = TextBlock.ForegroundProperty;
                            break;
                        default:
                            throw new NotSupportedException("Trigger target " + s_state.TriggerTarget + " with TemplateChildType " + s_state.TemplateChildType);
                    }
                }
            }

            //
            // Set values of properties depending on 'verification'.
            //
            if (prop1 != null)
            {
                if (verification == 1)
                {
                    properties[prop1] = Brushes.Yellow;
                    properties[prop2] = Brushes.Green;
                }
                else if (verification == 2)
                {
                    properties[prop1] = Brushes.DarkBlue;
                    properties[prop2] = Brushes.DarkGreen;
                }
                else if (verification == 3)
                {
                    properties[prop1] = Brushes.Crimson;

                    if (s_state.HasEventTrigger == "Loaded")
                    {
                        properties[prop2] = Brushes.DarkGreen;
                    }
                    else
                    {
                        properties[prop2] = Brushes.Red;
                    }
                }
                else
                {
                    //
                    // Set values depending on TemplateBinding setting.
                    //
                    if (s_state.HasTemplateBind)
                    {
                        properties[prop1] = Brushes.Orange;
                    }
                    else
                    {
                        properties[prop1] = Brushes.Pink;
                    }
                    properties[prop2] = Brushes.Red;
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

            foreach(DependencyProperty prop in properties.Keys)
            {
                string propstr = _ConvertToString(prop);
                string expectedVal = _ConvertToString(properties[prop]);
                string actualVal = _ConvertToString(element.GetValue(prop));

                CoreLogger.LogStatus("Verifying " + propstr + " equals " + expectedVal + "...");

                if (expectedVal != actualVal)
                {
                    CoreLogger.LogStatus("Fail", ConsoleColor.Red);
                    throw new Microsoft.Test.TestValidationException(
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
            
            for(int i = 0; i < count; i++)
            {
                // Common base class for Visual and Visual3D is DependencyObject

                DependencyObject child = VisualTreeHelper.GetChild(element, i);
                if (child is FrameworkElement)
                    cp = getFirstContentPresenter((FrameworkElement)child);

                if (cp != null) return cp;
            }

            return null;
        }

        static internal FrameworkElement findFirstFrameworkElement(FrameworkContentElement fce)
        {
            FrameworkElement fe = null;

            foreach (object item in LogicalTreeHelper.GetChildren(fce))
            {
                if (item is FrameworkElement) return (FrameworkElement)item;

                if (item is FrameworkContentElement)
                {
                    fe = findFirstFrameworkElement((FrameworkContentElement)item);
                }
            
                if (fe != null) return (FrameworkElement)fe;
            }

            return null;
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
        static private TemplateModelState s_state = null;
    }

}



