// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Xml;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Modeling;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;


namespace Avalon.Test.CoreUI.PropertyEngine.Template.Element3D
{
    /// <summary>
    /// Routines used by Template.Element3D tests.
    /// </summary>
    internal class TemplateElement3DHelper
    {
        #region Public Members
        /******************************************************************************
        * Function:          FindControlTemplate
        ******************************************************************************/
        /// <summary>
        /// Look for a ControlTemplate inside a NavigationWindow.
        /// </summary>
        /// <param name="navWin">NavigationWindow</param>
        /// <param name="rootName">Name of the window's Root element</param>
        /// <param name="parentName">Name of the templated Element</param>
        /// <param name="templatedElementName">Name of the templated Element</param>
        /// <returns>A ControlTemplate, which is null if not found</returns>
        public static ControlTemplate FindControlTemplate(NavigationWindow navWin, string rootName, string parentName, string templatedElementName)
        {
            GlobalLog.LogStatus("----FindControlTemplate----");

            ControlTemplate template = new ControlTemplate();

            FrameworkElement templatedElement = FindTemplatedElement(navWin, rootName, parentName, templatedElementName);

            if (templatedElement == null)
            {
                GlobalLog.LogEvidence("TemplatedElement (" + templatedElementName + ") was not found within " + parentName + ".");
            }
            else
            {
                GlobalLog.LogEvidence("TemplatedElement (" + templatedElementName + ") was found within " + parentName + ".");

                template = (ControlTemplate)((Control)templatedElement).Template;
            }

            return template;
        }

        /******************************************************************************
        * Function:          FindTemplatedElement
        ******************************************************************************/
        /// <summary
        /// Look for a ControlTemplate inside a NavigationWindow.
        /// </summary>
        /// <param name="navWin">NavigationWindow</param>
        /// <param name="rootName">Name of the window's Root element</param>
        /// <param name="parentName">Name of the Viewport3D</param>
        /// <param name="templatedElementName">Name of the templated Element</param>
        /// <returns>A FrameworkElement, which is null if not found</returns>
        public static FrameworkElement FindTemplatedElement(NavigationWindow navWin, string rootName, string parentName, string templatedElementName)
        {
            GlobalLog.LogStatus("----Finding TemplatedElement----");

            FrameworkElement templatedElement = null;

            DependencyObject parentElement = (DependencyObject)LogicalTreeHelper.FindLogicalNode((DependencyObject)navWin.Content, parentName);

            if (parentElement == null)
            {
                GlobalLog.LogEvidence("Parent Element (" + parentName + ") was not found.");
            }
            else
            {
                GlobalLog.LogEvidence("Parent Element (" + parentName + ") was found.");

                templatedElement = (FrameworkElement)VisualTreeUtils.FindElement(templatedElementName, parentElement);
            }

            return templatedElement;
        }

        /******************************************************************************
        * Function:          VerifyResourceDictionary
        ******************************************************************************/
        /// <summary>
        /// Checks whether a ResourceDictionary is found within the ControlTemplate, and in some
        /// cases whether or not it contains a Style.
        /// </summary>
        /// <param name="template">The ControlTemplate that is involved in the test result verification.</param>
        /// <param name="checkForStyle">Indicates whether or not to check for a Style inside the Template.</param>
        /// <returns>A boolean, indicating pass or fail</returns>
        public static bool VerifyResourceDictionary(ControlTemplate template, bool checkForStyle)
        {
            bool result = false;

            if (template.Resources == null)
            {
                GlobalLog.LogEvidence("VerifyResourceDictionary:  ERROR!!! Template Resources object is null.");
                result = false;
            }
            else
            {
                if (template.Resources.Count == 1)
                {
                    if (checkForStyle)
                    {
                        ResourceDictionary dictionary = template.Resources;

                        if (dictionary["StyleKey"] == null)
                        {
                            result = false;

                        }
                        else
                        {
                            result = true;
                        }
                        GlobalLog.LogEvidence("VerifyResourceDictionary: Style found in Dictionary: " + (dictionary["StyleKey"] != null).ToString());
                    }
                    else
                    {
                        result = true;
                    }
                }
                else
                {
                    result = false;
                }

                GlobalLog.LogEvidence("VerifyResourceDictionary: Template Resources.Count:  " + template.Resources.Count.ToString());
            }

            return result;
        }


        /******************************************************************************
        * Function:          VerifyPropertyValue
        ******************************************************************************/
        /// <summary>
        /// Compares the actual Width with the expected Width for the Border element within the ControlTemplate.
        /// </summary>
        /// <param name="dp">The DependencyProperty (double) to be verified</param>
        /// <param name="expectedValue">The expected value for the Width property affected by the ControlTemplate</param>
        /// <param name="template">The ControlTemplate that is involved in the test result verification</param>
        /// <param name="navWin">NavigationWindow</param>
        /// <param name="rootName">Name of the window's Root element</param>
        /// <param name="parentElementName">Name of a parent of the templated Element</param>
        /// <param name="templatedElementName">Name of the templated Element.</param>
        /// <param name="templateElementName">Name of the Element inside the Control Template</param>
        /// <returns>A boolean, indicating pass or fail</returns>
        public static bool VerifyPropertyValue(DependencyProperty dp, double expectedValue, ControlTemplate template, NavigationWindow navWin, string rootName, string parentElementName, string templatedElementName, string templateElementName)
        {
            bool result = false;

            GlobalLog.LogStatus("----VerifyPropertyValue----");

            FrameworkElement templatedElement = FindTemplatedElement(navWin, rootName, parentElementName, templatedElementName);
            
            if (templatedElement == null)
            {
                GlobalLog.LogEvidence("ERROR!!! VerifyPropertyValue: templatedElement (" + templatedElementName + ") not found within " + parentElementName + ".");
                return false;
            }
            else
            {
                GlobalLog.LogEvidence("VerifyPropertyValue: templatedElement (" + templatedElementName + ") found within " + parentElementName + ".");

                DependencyObject dependencyObject = (DependencyObject)template.FindName(templateElementName, (FrameworkElement)templatedElement);
                if (dependencyObject == null)
                {
                    GlobalLog.LogEvidence("ERROR!!! VerifyPropertyValue: element(" + templateElementName + ") was not found in the Template.");
                    return false;
                }
                else
                {
                    GlobalLog.LogEvidence("VerifyPropertyValue: element(" + templateElementName + ") was found in the Template.");

                    FrameworkElement element = (FrameworkElement)dependencyObject;
                    double actualValue = (double)element.GetValue(dp);
                    GlobalLog.LogEvidence("Property Value in Template:\n" + "Expected: " + expectedValue + "\nActual:  " + actualValue);

                    if (actualValue == expectedValue)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }

            return result;
        }
        #endregion
    }
}
