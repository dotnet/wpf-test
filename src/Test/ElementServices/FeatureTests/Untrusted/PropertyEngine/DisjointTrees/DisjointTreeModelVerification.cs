// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Automation for the stateless MDE model DisjointTree.
 *  
 * disjoint: v. To put out of joint; dislocate. 
 * This model verifies logical tree services in trees that are separated
 * from the primary logical tree. Particularly, trees in ContextMenus,
 * ToolTips and VisualBrushes. 
 * 
 * Contributors: Microsoft
 *
 
  
 * Revision:         $Revision: 14 $
 
 * Filename:         $Source: //depot/vbl_wcp_avalon_dev/windowstest/client/wcptests/Core/Framework/BVT/PropertyEngine/DisjointTree/DisjointTreeModel.cs $
********************************************************************/
using System;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Avalon.Test.CoreUI;
using System.IO;
using System.Collections;
using System.Windows.Threading;
using System.Windows.Markup;
using Microsoft.Test.Modeling;
using Microsoft.Test.Serialization;
using Microsoft.Test.Threading;

using Avalon.Test.CoreUI.Parser;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Threading;
using Microsoft.Test.Logging;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using System.Xml;

namespace Avalon.Test.CoreUI.PropertyEngine.DisjointTree
{
  
    /// <summary>
    /// DisjointTreeVerification class
    /// </summary>  
    internal static class DisjointTreeVerification
    {
        /// <summary>
        /// Verify disjoint tree.
        /// </summary>
        static public void Verify(FrameworkElement root)
        {
            //_state = (DisjointTreeModelState)DisjointTreeModelState.Load();
            s_state = new DisjointTreeModelState(CoreState.Load());
            
            ModelFailureOverride failureOverride = new ModelFailureOverride(s_state, @"DisjointTree_FailureOverride.xml");

            //
            // Get test tree elements by FindName. (This verifies name lookup).
            //

            FrameworkElement testRoot = (FrameworkElement)root.FindName(s_state.TestRootName);
            if (testRoot == null)
                throw new Microsoft.Test.TestValidationException("Could not find test root.");

            FrameworkElement firstMentor = (FrameworkElement)testRoot.FindName(s_state.FirstMentorName);
            if (firstMentor == null)
                throw new Microsoft.Test.TestValidationException("Could not find first mentor.");

            FrameworkElement secondMentor = (FrameworkElement)testRoot.FindName(s_state.SecondMentorName);
            if (firstMentor == null)
                throw new Microsoft.Test.TestValidationException("Could not find second mentor.");

            bool caughtValidationException = false;
            try
            {
                VerifyDisjointTree(firstMentor, s_state.FirstMenteeName);
                VerifyDisjointTree(secondMentor, s_state.SecondMenteeName);
            }
            catch (Microsoft.Test.TestValidationException e)
            {
                caughtValidationException = true;

                if (failureOverride.ShouldFail)
                {
                    //CoreLogger.LogStatus(_state.Service + " is not supported in " + _state.Link + " hybrid tree set by " + _state.HowLinked, ConsoleColor.Yellow);
                    CoreLogger.LogStatus(failureOverride.Reason, ConsoleColor.Yellow);
                    CoreLogger.LogStatus("*** THIS VERIFICATION FAILURE IS EXPECTED!", ConsoleColor.Yellow);
                    CoreLogger.LogStatus(e.Message);
                    CoreLogger.LogStatus(e.StackTrace);
                    CoreLogger.LogStatus("*** THIS VERIFICATION FAILURE IS EXPECTED!", ConsoleColor.Yellow);
                }
                else // This was not an expected exception.
                {
                    throw;
                }
            }

            if (!caughtValidationException)
            {
                if (failureOverride.ShouldFail)
                {
                    CoreLogger.LogStatus("Verification should have failed but did not, this scenario is broken", ConsoleColor.Red);
                    CoreLogger.LogStatus(failureOverride.Reason, ConsoleColor.Yellow);
                    throw new Microsoft.Test.TestValidationException(s_state.Service + " is not supported in " + s_state.Link + " hybrid tree set by " + s_state.HowLinked);
                }
            }

            // Helpful pause before closing.
            DispatcherHelper.DoEvents(1000);
        }

        static private void VerifyDisjointTree(FrameworkElement mentor, string menteeName)
        {
            ShowDisjointTree(mentor);
            FrameworkElement mentee = GetMentee(mentor, menteeName);

            if (mentee == null)
                throw new Microsoft.Test.TestValidationException("Could not find mentee " + menteeName);

            _VerifyProperties(mentee, menteeName, PredictPropertyValues(s_state.Service, menteeName));
        }

        static private void ShowDisjointTree(FrameworkElement mentor)
        {
            // Trigger display of mentee.
            switch (s_state.Link)
            {
                case "VisualBrush":
                    // Do nothing, VisualBrush is already displayed.
                    break;

                case "ContextMenu":
                    // Move mouse over button and right-click.
                    MouseHelper.Move(mentor, MouseLocation.Center);
                    MouseHelper.Click(MouseButton.Right);

                    DispatcherHelper.DoEvents(500);

                    // Close context menu
                    MouseHelper.Move(mentor, MouseLocation.CenterLeft);
                    MouseHelper.Click(MouseButton.Left);
                    break;

                case "ToolTip":
                    // Move mouse over button and wait for tooltip to appear.
                    MouseHelper.Move(mentor, MouseLocation.Center);
                    DispatcherHelper.DoEvents(2000);

                    MouseHelper.Move(mentor);
                    break;

                //default:
                //    throw new NotSupportedException(_state.Link);
                //    break;
            }

        }


        static private FrameworkElement GetMentee(FrameworkElement mentor, string menteeName)
        {
            FrameworkElement mentee = null;

            if ((s_state.HowLinked == "Locally") || (s_state.HowLinked == "Style"))
            {
                CoreLogger.LogStatus("Finding " + s_state.HowLinked + " defined name " + menteeName);
                
                mentee = (FrameworkElement)mentor.FindName(menteeName);

            }
            else if ((s_state.HowLinked == "StaticResource") || (s_state.HowLinked == "DynamicResource"))
            {
                CoreLogger.LogStatus("Getting " + menteeName + " via VisualTree.");
                // Names are not supported in resources sections so dig into the mentor's properties.
                if (s_state.Link == "VisualBrush")
                {
                    VisualBrush link = (VisualBrush)((Button)mentor).Background;
                    mentee = (FrameworkElement)link.Visual;
                }
                else if (s_state.Link == "ContextMenu")
                {
                    ContextMenu cm = ((Button)mentor).ContextMenu;
                    mentee = (FrameworkElement)cm.Items[0];
                }
                else if (s_state.Link == "ToolTip")
                {
                    ContentControl cc = (ContentControl)(((Button)mentor).ToolTip);
                    mentee = (FrameworkElement)cc.Content;
                }
                else
                {
                    throw new NotSupportedException(s_state.Link);
                }
            }
            else if (s_state.HowLinked == "ControlTemplate")
            {
                CoreLogger.LogStatus("Finding " + menteeName + " via template FindName.");
                mentee = (FrameworkElement)((Control)mentor).Template.FindName(menteeName, mentor);
            }
            else if (s_state.HowLinked == "DataTemplate")
            {
                CoreLogger.LogStatus("Finding " + menteeName + " via template FindName.");
                
                DependencyObject chrome = (DependencyObject)VisualTreeHelper.GetChild(mentor, 0);
                DependencyObject contentPresenter = (DependencyObject)VisualTreeHelper.GetChild(chrome, 0);
                Button templateButton = (Button)VisualTreeHelper.GetChild(contentPresenter, 0);
                mentee = (FrameworkElement)templateButton.FindName(menteeName);
            }
            else
            {
                throw new NotSupportedException(s_state.HowLinked);
            }

            if (mentee == null)
            {
                throw new Microsoft.Test.TestValidationException("Could not find menteeName " + menteeName);
            }

            CoreLogger.LogStatus("Pass", ConsoleColor.Green);

   
            return mentee;
        }


       
        /// <summary>
        /// Sets the expected properties and values for the mentee depending on the kind of 
        /// verification that is currently in effect and which mentee it is.
        /// </summary>
        /// <returns>A dictionary of properties and their values.</returns>
        static private IDictionary PredictPropertyValues(string service, string menteeName)
        {
            Hashtable properties = new Hashtable();

            //
            // Determine the properties to check.
            //
            DependencyProperty prop1 = null;

            bool firstMentee = menteeName.Contains("first");

            switch (service)
            {
                case "DynamicResource":
                    prop1 = Rectangle.FillProperty;
                    if (firstMentee)
                        properties[prop1] = Colors.Lime;
                    else
                        properties[prop1] = Colors.Yellow;

                    break;

                case "Inheritance":
                    prop1 = TextBlock.FontStyleProperty;
                    if (firstMentee)
                        properties[prop1] = FontStyles.Italic;
                    else
                        properties[prop1] = FontStyles.Oblique;

                    break;

                case "LoadedEvent":
                case "InitializationEvent":
                    prop1 = Button.BackgroundProperty;
                    properties[prop1] = Colors.Crimson;

                    break;

                case "BindingDataContext":
                    prop1 = Button.BackgroundProperty;
                    if (firstMentee)
                        properties[prop1] = Colors.Cyan;
                    else
                        properties[prop1] = Colors.Blue;

                    break;

                case "BindingElementName":
                    prop1 = Shape.FillProperty;
                    if (firstMentee)
                        properties[prop1] = Colors.Orange;
                    else
                        properties[prop1] = Colors.Red;

                    break;

                case "ImplicitStyle":
                    prop1 = Button.BackgroundProperty;
                        properties[prop1] = Colors.Magenta;

                    break;

                //default:
                //    throw new NotSupportedException(service);
                //    break;
            }

            // Return collection.
            return properties;
        }

        /// <summary>
        /// Loop through the given properties and values to verify they
        /// match actual current property values.
        /// </summary>
        /// <remarks>
        /// todo: refactor to CommonModelVerification
        /// </remarks>
        static private void _VerifyProperties(DependencyObject element, string name, IDictionary properties)
        {
            //string name = TreeHelper.GetNodeId(element);
            //if (name == "")
            //{
            //    name = element.GetType().Name;
            //}

            foreach (DependencyProperty prop in properties.Keys)
            {
                string propstr = _ConvertToString(prop);
                string expectedVal = _ConvertToString(properties[prop]);
                string actualVal = _ConvertToString(element.GetValue(prop));

                CoreLogger.LogStatus("Verifying " + propstr + " equals " + expectedVal + "...");

                if (expectedVal != actualVal)
                {
                    throw new Microsoft.Test.TestValidationException(
                        propstr + " of '" + name + "' doesn't have the expected value. Expected: " + expectedVal + ". Actual: " + actualVal + ".");
                }
            }
        }

        /// <summary>
        /// Verify element property has expected value.
        /// </summary>
        static private void VerifyProperty(DependencyObject element, DependencyProperty property, object value)
        {
            //string id = TreeHelper.GetNodeId(element);
            string id = ((IFrameworkInputElement)element).Name;
            if (id == "")
            {
                id = element.GetType().Name;
            }

            string propstr = _ConvertToString(property);
            string expectedVal = _ConvertToString(value);
            string actualVal = _ConvertToString(element.GetValue(property));

            CoreLogger.LogStatus("Verifying " + propstr + " equals " + expectedVal + "...");

            if (expectedVal != actualVal)
            {
                throw new Microsoft.Test.TestValidationException(
                    propstr + " of '" + id + "' doesn't have the expected value. Expected: " + expectedVal + ". Actual: " + actualVal + ".");
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
        static private DisjointTreeModelState s_state = null;
    }
}

