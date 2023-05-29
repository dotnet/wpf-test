// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Please see comments on class declaration below.
 *          
 * Contributor: 
 *
 
  
 * Revision:         $Revision: 4 $
 
 * Filename:         $Source: //depot/vbl_wcp_avalon_dev/windowstest/client/wcptests/Core/Framework/BVT/parser/ContentModel.cs $
********************************************************************/

using System;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Avalon.Test.CoreUI;
using System.Collections;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Avalon.Test.CoreUI.Common;
using Microsoft.Test.Threading;
using Microsoft.Test.Serialization;
using Microsoft.Test.Serialization.CustomElements;
using Avalon.Test.CoreUI.Serialization;
using Avalon.Test.CoreUI.IdTest;

namespace Avalon.Test.CoreUI.Events
{
    /// <summary>
    /// Holds verification routines for various event tests.
    /// </summary>
    public class EventsVerifier
    {
        /// <summary>
        /// Throws an exception with the given error message, if the condition is false.
        /// </summary>
        /// <param name="condition">Given condition</param>
        /// <param name="errorMesg">Error message for the exception to be thrown</param>
        private static void Assert(bool condition, String errorMesg)
        {
            if (!condition)
            {
                throw new Microsoft.Test.TestValidationException(errorMesg);
            }
        }

        /// <summary>
        /// Verification routine used by StaticLoadedModel.cs.
        /// </summary>
        public static void StaticLoadedModelVerify(UIElement uie)
        {
            // Wait 2sec before starting the verification.
            // This allows animations to finish.
            DispatcherHelper.DoEvents(2000);

            String errorMesg = "Loaded event verification failed.";

            // Get the inParams saved by the model.
            CoreModelState cms = CoreModelState.Load();

            // Read values of various inParams.
            string ElementType = cms.Dictionary["ElementType"] as string;
            string ElementUsageLocation = cms.Dictionary["ElementUsageLocation"] as string;
            string TreeHost = cms.Dictionary["TreeHost"] as string;
            string LoadedEventUsage = cms.Dictionary["LoadedEventUsage"] as string;

            SolidColorBrush background = null;

            Button Button0 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button0");
            PasswordBox PasswordBox0 = (PasswordBox)LogicalTreeHelper.FindLogicalNode(uie, "PasswordBox0");

            if ((ElementUsageLocation == "ElementTree") || (ElementUsageLocation == "Style"))
            {
                // In case of event handlers/triggers set directly on the element or thru
                // a style, they will affect the element's background directly.

                // The element on which we are testing the Loaded event is different
                // depending on whether it's a FrameworkElement or a FrameworkContentElement
                string elementName = null;
                switch (ElementType)
                {
                    case "FrameworkElement":
                        elementName = "PasswordBox0";
                        PasswordBox passwordbox = (PasswordBox)LogicalTreeHelper.FindLogicalNode(uie, elementName);
                        background = passwordbox.GetValue(Control.BackgroundProperty) as SolidColorBrush;
                        break;

                    case "FrameworkContentElement":
                        elementName = "FlowDocument0";
                        FlowDocument flowdocument = (FlowDocument)LogicalTreeHelper.FindLogicalNode(uie, elementName);
                        background = flowdocument.GetValue(FlowDocument.BackgroundProperty) as SolidColorBrush;
                        break;
                }
                
                Assert(Color.Equals(background.Color, Colors.Red),
                    errorMesg + " " + elementName + " was supposed to have Red background, but has "
                    + background + " background.");
            }
            else
            {
                // In case of event handlers/triggers set thru templates, they will affect
                // elements in the template visual tree (in our case, root of the tree).
                string templateRootName = null; // either "DataTemplateRoot" or "ControlTemplateRoot"
                object templateRoot = null;
                if ((ElementUsageLocation == "ControlTemplate") || (ElementUsageLocation == "StyleInControlTemplate"))
                {
                    templateRootName = "ControlTemplateRoot";

                    // Get the template root.
                    templateRoot = PasswordBox0.Template.FindName(templateRootName, PasswordBox0);
                }
                else // DataTemplate or StyleInDataTemplate
                {
                    templateRootName = "DataTemplateRoot";

                    // Get the template root.
                    FrameworkElement cp = getFirstContentPresenter(Button0);
                    templateRoot = Button0.ContentTemplate.FindName(templateRootName, cp);
                }

                switch (ElementType)
                {
                    case "FrameworkElement":
                        PasswordBox passwordbox = templateRoot as PasswordBox;
                        Assert(passwordbox.Name == templateRootName, errorMesg +
                            " Template's root is not as expected.");
                        background = passwordbox.GetValue(Control.BackgroundProperty) as SolidColorBrush;
                        break;

                    case "FrameworkContentElement":
                        FlowDocument flowdocument = templateRoot as FlowDocument;
                        Assert(flowdocument.Name == templateRootName, errorMesg +
                            " Template's root is not as expected.");
                        background = flowdocument.GetValue(FlowDocument.BackgroundProperty) as SolidColorBrush;
                        break;
                }
                
                Assert(Color.Equals(background.Color, Colors.Red),
                        errorMesg + " " + templateRootName + " was supposed to have Red background, but has "
                        + background + " background.");                
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
                DependencyObject child = VisualTreeHelper.GetChild(element, i);
                if (child is FrameworkElement)
                    cp = getFirstContentPresenter((FrameworkElement)child);

                if (cp != null) return cp;
            }

            return null;
        }
    }
}
