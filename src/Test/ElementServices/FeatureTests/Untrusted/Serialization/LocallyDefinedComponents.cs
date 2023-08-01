// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Tests for locally-defined components in xaml, including
 *          in Styles and Templates.
 * 
 * Contributors: 
 *
 * $Header$
********************************************************************/
using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using Avalon.Test.CoreUI.Common;
using Microsoft.Test.Win32;

using Microsoft.Test.Logging;
using Avalon.Test.CoreUI.Parser;
using Avalon.Test.CoreUI.Trusted.Utilities;
using System.Windows.Controls;
using System.Reflection;
using Microsoft.Test;


namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    /// Creates and compiles a new Avalon application project based on
    /// a single xaml file.
    /// </summary>
    public class LocallyDefinedComponents
    {
        // string _xamlFile = "TestPage.xaml";

        /// <summary>
        /// This is the equivalent of the Main method. 
        /// </summary>
        public void RunTest()
        {
            //CompilerHelper compiler = new CompilerHelper ();

            //compiler.CleanUpCompilation();
            //compiler.AddDefaults();


            //List<string> extraFiles = new List<string>();
            //extraFiles.Add("LocallyDefinedTypes.cs");
            //compiler.CompileApp(_xamlFile, "Application", null, extraFiles);

            //compiler.RunCompiledApp();
        }

        /// <summary>
        /// Verifies that the locally-defined types are present in the tree.
        /// </summary>
        public static void Verify1(UIElement root)
        {
            Hashtable idNodes = new Hashtable();
            TreeHelper.FindNodesWithIds(root, idNodes);

            //
            // MyButton1
            //
            DependencyObject myButton1 = (DependencyObject)idNodes["MyButton1"];
            _VerifyElement(myButton1, "This is locally-defined Button 1", true);


            //
            // MyButton2
            //
            DependencyObject myButton2 = (DependencyObject)idNodes["MyButton2"];
            _VerifyElement(myButton2, "This is locally-defined Button 2", true);


            //
            // TemplatedButton
            // Send 'false' for shouldTestEvent due to 

            Visual templatedButton = (Visual)idNodes["TemplatedButton"];
            DependencyObject templateRoot = VisualTreeHelper.GetChild(templatedButton,0);
            _VerifyElement(templateRoot, "This is a locally-defined Button in a template", false);

            //
            // DefaultButton
            // Verify the Content and Foreground properties (set to locally-defined markup extensions).
            Button defaultButton = (Button)idNodes["DefaultButton"];
            SolidColorBrush foreground = (SolidColorBrush)defaultButton.Foreground;
            if (!Color.Equals(foreground.Color, Colors.Maroon + Colors.Navy + Colors.Green))
            {
                throw new Microsoft.Test.TestValidationException("DefaultButton's Foreground property was not the expected value. Expected '" + (Colors.Maroon + Colors.Navy + Colors.Green) + "', Actual '" + foreground.Color + "'.");
            }
            SolidColorBrush contentBrush = (SolidColorBrush)defaultButton.Content;
            if (!Color.Equals(contentBrush.Color, Colors.Black + Colors.Blue + Colors.Yellow + Colors.Green + Colors.Orange))
            {
                throw new Microsoft.Test.TestValidationException("DefaultButton's Content property was not the expected SolidColorBrush value. Expected '" + (Colors.Black + Colors.Blue + Colors.Yellow + Colors.Green + Colors.Orange) + "', Actual '" + contentBrush.Color + "'.");
            }
        }

        
        
        /// <summary>
        /// Verifies that the locally-defined types for Locally defined model: Verify the 
        /// locally defined property value has been set and the event handler on locally 
        /// defined event has been invoked. 
        /// </summary>
        /// <param name="root"></param>
        public static void ModelVerifierEventHandler(UIElement root)
        {
            s_verifyInvoked = true;
            VerifyModel(root);
        }

        /// <summary>
        /// Verifies that the locally-defined types for Locally defined model: Verify the 
        /// locally defined property value has been set and the EventTrigger on locally 
        /// defined event has been invoked. 
        /// </summary>
        /// <param name="root"></param>
        public static void ModelVerifierEventTrigger(UIElement root)
        {
            s_verifyWidth = true;
            VerifyModel(root);
        }

        /// <summary>
        /// Verifies that the locally-defined types for Locally defined model: Verify the 
        /// locally defined property value has been set.
        /// </summary>
        /// <param name="root"></param>
        public static void ModelVerifier(UIElement root)
        {
            VerifyModel(root);
        }

        /// <summary>
        /// Verifies that the locally-defined types for Locally defined model: Verify the 
        /// locally defined property value has been set and the event handler on locally 
        /// defined event has been invoked. 
        /// </summary>
        /// <param name="root"></param>
        public static void ModelVerifierPropertyTriggerEventHandler(UIElement root)
        {
            s_verifyBackground = true;
            ModelVerifierEventHandler(root);
        }

        /// <summary>
        /// Verifies that the locally-defined types for Locally defined model: Verify the 
        /// locally defined property value has been set and the EventTrigger on locally 
        /// defined event has been invoked. 
        /// </summary>
        /// <param name="root"></param>
        public static void ModelVerifierPropertyTriggerEventTrigger(UIElement root)
        {
            s_verifyBackground = true;
            ModelVerifierEventTrigger(root);
        }

        /// <summary>
        /// Verifies that the locally-defined types for Locally defined model: Verify the 
        /// locally defined property value has been set.
        /// </summary>
        /// <param name="root"></param>
        public static void ModelVerifierPropertyTrigger(UIElement root)
        {
            s_verifyBackground = true;
            ModelVerifier(root);
        }                

        /// <summary>
        /// Verifier for locally defined model
        /// </summary>
        /// <param name="root">Root</param>
        static void VerifyModel(UIElement root)
        {
            Hashtable idNodes = new Hashtable();
            FindNodesWithIdentifierProperties(root, idNodes);

            //
            // MyButton1
            //
            DependencyObject elementInTree = (DependencyObject)idNodes["elementName"];
            DependencyObject elementHost = (DependencyObject)idNodes["elementHost"];
            DependencyObject elementWithTemplate = (DependencyObject)idNodes["elementWithTemplate"];
            DependencyObject elementToVerify = null;
            if (null != elementInTree)
            {
                elementToVerify = elementInTree;
            }
            else if (null != elementHost)
            {
                elementToVerify = ((ContentControl)elementHost).Content as DependencyObject;
            }
            else if (null != elementWithTemplate)
            {
                elementToVerify = ((Control)elementWithTemplate).Template.FindName("elementInTemplate", (FrameworkElement)elementWithTemplate) as DependencyObject;
            }
            else
            {
                throw new Microsoft.Test.TestValidationException("Not found element.");
            }
            VerifyModelElement(elementToVerify);
        }

        /// <summary>
        /// Look for nodes of type LocallyDefinedButtonBase in the tree,
        /// and gather their IDs (Identifier properties).
        /// </summary>
        /// <param name="subtreeRoot"></param>
        /// <param name="nodesWithIds"></param>
        private static void FindNodesWithIdentifierProperties(DependencyObject subtreeRoot, Hashtable nodesWithIds)
        {
            // Check given node for Identifier property.
            if (subtreeRoot is LocallyDefinedButtonBase)
            {
                string id = (subtreeRoot as LocallyDefinedButtonBase).Identifier;
                if (!(String.IsNullOrEmpty(id)) && !nodesWithIds.ContainsKey(id))
                {
                    nodesWithIds.Add(id, subtreeRoot);
                }
            }

            // Check children.
            List<DependencyObject> children = TreeHelper.GetChildren(subtreeRoot);

            for (int i = 0; i < children.Count; i++)
            {
                FindNodesWithIdentifierProperties(children[i], nodesWithIds);
            }
        }

        static void VerifyModelElement(DependencyObject element)
        {
            UIElement uiElement = element as UIElement;
            if (null == uiElement)
                throw new Microsoft.Test.TestValidationException("Element to verify is not UIElement.");
            InternalObject internalObj = null;
            RoutedEvent eventID = null;
            bool isCodeDefinedComponent = true;
            try
            {
                internalObj = new InternalObject(element);
                eventID = internalObj.GetField("LocallyDefinedEventID", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static) as RoutedEvent;
            }
            catch
            {
                isCodeDefinedComponent = false;
            }
            if (!isCodeDefinedComponent)
            {
                Type xamlbased = element.GetType();
                Type baseType = xamlbased.BaseType;

                FieldInfo field = baseType.GetField("LocallyDefinedEventID", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                eventID = field.GetValue(element) as RoutedEvent;
            }

            if (null == eventID)
            {
                throw new Microsoft.Test.TestValidationException("Not found EventID.");
            }
            uiElement.AddHandler(eventID, new RoutedEventHandler(OnLocallyDefinedEventInvoked));

            RoutedEventArgs args = new RoutedEventArgs(eventID);
            uiElement.RaiseEvent(args);
            //Verify locally defined property value has been set correctly.
            CoreLogger.LogStatus("Verifying locally defined property...");
            _VerifyProperty(internalObj, "LocallyDefinedProperty", "LocallyDefinedValue");
              
        } 
        /// <summary>
        /// Verify properties after locally defined event has been invoked. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnLocallyDefinedEventInvoked(Object sender, RoutedEventArgs e)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new DispatcherOperationCallback(RenderCallBack), sender);                
        }
        static object RenderCallBack(object sender)
        {

            UIElement uiElement = sender as UIElement;
            CoreLogger.LogStatus("Verifying Width property...");
            if (s_verifyWidth) // verify the effect of EventTrigger
            {
                double width = (double)uiElement.GetValue(FrameworkElement.WidthProperty);
                VerifyElement.VerifyDouble(width, 300);
            }

            CoreLogger.LogStatus("Verifying EventInvoked property...");
            if (s_verifyInvoked)// verify effect of EventHandler
            {
                //Verify event result
                InternalObject internalObj = new InternalObject(sender);
                bool locallyDefinedEventInvoked = (bool)internalObj.GetProperty("EventInvoked");
                if (!locallyDefinedEventInvoked)
                {
                    throw new Microsoft.Test.TestValidationException("Locally defined Event not fired.");
                }
            }
            //Verify the Background of the button.
            CoreLogger.LogStatus("Verifying Background property...");
            if (s_verifyBackground)
            {
                SolidColorBrush background = uiElement.GetValue(Control.BackgroundProperty) as SolidColorBrush;
                VerifyElement.VerifyBool(null != background, true);
                VerifyElement.VerifyColor(background.Color, Colors.Red);
            }
            return null;
        }
        
        
        private static void _VerifyElement(object obj, string initialContent, bool shouldTestEvent)
        {
            InternalObject internalObj = new InternalObject(obj);

            // Check the Content.
            _VerifyProperty(internalObj, "Content", initialContent);

            // Check  the LocallyDefinedProperty.
            _VerifyProperty(internalObj, "LocallyDefinedProperty", "LocallyDefinedValue");

            // Check the Foreground property (set to a locally-defined markup extension)
            SolidColorBrush foreground = (SolidColorBrush)internalObj.GetProperty("Foreground");
            if (!Color.Equals(foreground.Color, Colors.Red + Colors.Blue))
            {
                throw new Microsoft.Test.TestValidationException("Element's Foreground property was not the expected value. Expected '" + (Colors.Red + Colors.Blue) + "', Actual '" + foreground.Color + "'.");
            }

            if (shouldTestEvent)
            {
                // Change LocallyDefinedProperty and check the Content again.
                // Setting LocallyDefinedProperty should cause the changed event, which
                // should change the content.
                internalObj.SetProperty("LocallyDefinedProperty", "LocallyDefinedValue2");
                _VerifyProperty(internalObj, "LocallyDefinedProperty", "LocallyDefinedValue2");
                _VerifyProperty(internalObj, "Content", "ContentValue2");
            }
        }
        private static void _VerifyProperty(InternalObject internalObj, string propertyName, string expectedValue)
        {
            string actualValue = (string)internalObj.GetProperty(propertyName);

            if (actualValue != expectedValue)
            {
                throw new Microsoft.Test.TestValidationException("Element's " + propertyName + " was not the expected value. Expected '" + expectedValue + "', Actual '" + actualValue + "'.");
            }
        }
        static bool s_verifyBackground = false;
        static bool s_verifyWidth = false;    
        static bool s_verifyInvoked = false;
    }

    /// <summary>
    /// Base class for LocallyDefinedButton
    /// </summary>
    public class LocallyDefinedButtonBase : Button
    {
        /// <summary>
        /// An ID
        /// </summary>
        public string Identifier
        {
            get { return _identifier; }
            set { _identifier = value; }
        }

        private string _identifier;
    }
}

