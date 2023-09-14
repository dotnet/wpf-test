// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Holds verification routines for Id tests.
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Controls;
using Avalon.Test.CoreUI.Common;
using System.Windows.Media;
using System.Windows.Documents;
using Avalon.Test.CoreUI.Parser;
using System.Collections;
using System.Reflection;
using System.Windows.Media.Animation;
using Microsoft.Test.Serialization.CustomElements;
using Avalon.Test.CoreUI.Serialization;
using Avalon.Test.CoreUI.PropertyEngine.Template;
using System.Threading;
using System.Windows.Shapes;
using System.IO;
using System.Xml;
using Microsoft.Test.Discovery;
using Microsoft.Test.Windows;
using System.Windows.Controls.Primitives;

namespace Avalon.Test.CoreUI.IdTest
{
    /// <summary>
    /// Holds verification routines for IdScoping.
    /// </summary>
    public class IdTestVerifiers
    {
        /// <summary>
        /// Verification routine for FrameworkElementWithId.xaml.
        /// </summary>
        public static void FrameworkElementWithIdVerify(UIElement root)
        {
            CoreLogger.LogStatus("Inside IdTestVerifiers.FrameworkElementWithId()...");
            VerifyAnElementWithID(root, "button", typeof(Button));

            Button button = (Button)IdTestBaseCase.FindElementWithId(root, "button");
            VerifyElement.VerifyString(button.Content as string, "content");

            //Verify that the field button is correct
            CoreLogger.LogStatus("Verifying field for button...");
            IdTestBaseCase.VerifyFieldValue(root, "button", button);
        }

        /// <summary>
        /// Verification routine for FrameworkElementWithxId.xaml.
        /// </summary>
        public static void FrameworkElementWithxIdVerify(UIElement root)
        {
            CoreLogger.LogStatus("Inside IdTestVerifiers.FrameworkElementWithxIdVerify()...");

            //Verify that the field button2 is correct
            Button button2 = (Button)IdTestBaseCase.FindElementWithId(root, "button2");
            CoreLogger.LogStatus("Verifying field ...");
            IdTestBaseCase.VerifyFieldValue(root, "button2", button2);


            CoreLogger.LogStatus("Verifying element in IdScope...");
            VerifyAnElementWithID(root, "button2", typeof(Button));

            VerifyElement.VerifyString(button2.Content as string, "content2");
        }
        /// <summary>
        /// Verification routine for EventTriggerOnFrameworkElement.xaml.
        /// </summary>
        public static void EventTriggerOnFrameworkElementVerify(UIElement root)
        {
            CoreLogger.LogStatus("Inside IdTestVerifiers.EventTriggerOnFrameworkElementVerify()...");

            TreeHelper.WaitForTimeManager();

            //Verify the width or root
            CoreLogger.LogStatus("Verifying root panel width ...");
            DockPanel rootPanel = root as DockPanel;
            VerifyElement.VerifyDouble(rootPanel.Width, 500);


            //Verify the width or button
            CoreLogger.LogStatus("Verifying button width ...");
            Button button = (Button)IdTestBaseCase.FindElementWithId(root, "button");
            VerifyElement.VerifyDouble(button.Width, 80);

            //Verify the width or child button
            CoreLogger.LogStatus("Verifying child button width ...");
            button = (Button)IdTestBaseCase.FindElementWithId(root, "childButton");
            VerifyElement.VerifyDouble(button.Width, 50);

            //Verify the width or sibling button
            CoreLogger.LogStatus("Verifying sibling button width ...");
            button = (Button)IdTestBaseCase.FindElementWithId(root, "sibling");
            VerifyElement.VerifyDouble(button.Width, 80);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        public static void NameScopeInNestedTemplate(UIElement root)
        {
            CoreLogger.LogStatus("Inside IdTestVerifiers.StoryboardsInStyleVerify()...");
            TreeHelper.WaitForTimeManager();
            FrameworkElement fe = root as FrameworkElement;
            ScrollBar sb = fe.FindName("scrollbar") as ScrollBar;
            VerifyElement.VerifyBool(sb == null, false);
            ControlTemplate template = sb.Template as ControlTemplate;
            VerifyElement.VerifyBool(template == null, false);
            Button buttonInTemplate = template.FindName("buttonInTemplate", sb) as Button;
            VerifyElement.VerifyBool(buttonInTemplate == null, false);
            Border borderInTemplate = template.FindName("borderInTemplate", sb) as Border;
            //Blocked on 
            VerifyElement.VerifyBool(borderInTemplate == null, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        public static void StoryboardsInStyleVerify(UIElement root)
        {
            CoreLogger.LogStatus("Inside IdTestVerifiers.StoryboardsInStyleVerify()...");
            TreeHelper.WaitForTimeManager();

            FrameworkElement fe = root as FrameworkElement;
            Rectangle rect = fe.FindName("rect") as Rectangle;
            VerifyElement.VerifyBool(rect == null, false);

            //verify that the animation stop action once mouse move in
            CoreLogger.LogStatus("Moving mouse outside then over the rect...");
            MouseHelper.MoveOutside(rect, MouseLocation.CenterLeft);
            MouseHelper.Move(rect);
            double originalOpacity = rect.Opacity;
            MouseHelper.Move(rect, MouseLocation.CenterLeft);

            CoreLogger.LogStatus("Waiting for time manager...");
            TreeHelper.WaitForTimeManager();

            double newOpacity = rect.Opacity;
            VerifyElement.VerifyDouble(originalOpacity, newOpacity);

            //verify that the animation begin action once mouse left
            Button button = fe.FindName("button") as Button;
            MouseHelper.Move(button);
            originalOpacity = rect.Opacity;
            MouseHelper.Move(button, MouseLocation.CenterLeft);
            TreeHelper.WaitForTimeManager();
            newOpacity = rect.Opacity;
            VerifyElement.VerifyBool(originalOpacity==newOpacity, false);

            //Verify that the BeginStoryboard in not in Namescope on Style. 
            Style style = rect.Style;
            BeginStoryboard bsb = ((INameScope)style).FindName("OpacityBegin") as BeginStoryboard;
            VerifyElement.VerifyBool(null != bsb, true);
            
            //Verify that the BeginStoryboard in not in Namescope on root. 
            bsb = fe.FindName("OpacityBegin") as BeginStoryboard;
            VerifyElement.VerifyBool(null == bsb, true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        public static void FrameworkElementWithAttachedNameScope(UIElement root)
        {
            CoreLogger.LogStatus("Inside IdTestVerifiers.FrameworkElementWithAttachedNameScope()...");
            TreeHelper.WaitForTimeManager();

        }

        /// <summary>
        /// Verification routine for FrameworkElementWithIdAndxId.xaml.
        /// </summary>
        public static void FrameworkElementWithIdAndxIdVerify(UIElement root)
        {            
            CoreLogger.LogStatus("Inside IdTestVerifiers.FrameworkElementWithIdAndxIdVerify()...");            

            FrameworkElementWithIdVerify(root);
            FrameworkElementWithxIdVerify(root);
        }

        /// <summary>
        /// Verification routine for FrameworkContentElementWithIDInControlTemplate.xaml.
        /// </summary>
        public static void FrameworkContentElementWithIDInControlTemplateVerify(UIElement root)
        {
            CoreLogger.LogStatus("Inside IdTestVerifiers.FrameworkContentElementWithIDInControlTemplateVerify()...");
            FrameworkElement fe = root as FrameworkElement;

            Button button1 = fe.FindName("button1") as Button;
            Button button2 = fe.FindName("button2") as Button;
            FrameworkElement templateRoot = (FrameworkElement)VisualTreeUtils.GetChild(button1, 0);

            //Finding from template
            ControlTemplate template = button1.Template;
            Bold childBold1FoundFromTemplate = template.FindName("childBold1", button1) as Bold;
            if (null == childBold1FoundFromTemplate)
            {
                throw new Microsoft.Test.TestValidationException("childBold1 Bold not found from template.");
            }

            Bold childBold1 = templateRoot.FindName("childBold1") as Bold;
            if (null == childBold1)
            {
                throw new Microsoft.Test.TestValidationException("childBold1 Bold not found.");
            }

            if (childBold1 != childBold1FoundFromTemplate)
            {
                throw new Microsoft.Test.TestValidationException("childBold1 Bold found from Template is different from that found inside template.");
            }

            //Finding the sameName
            Bold sameName = ((FrameworkContentElement)childBold1).FindName("sameName") as Bold;
            if (null == sameName)
            {
                throw new Microsoft.Test.TestValidationException("sameName Bold not found.");
            }

            //Finding self
            Bold childBold11 = ((FrameworkContentElement)childBold1).FindName("childBold1") as Bold;
            if (null == childBold1 || childBold11 != childBold1)
            {
                throw new Microsoft.Test.TestValidationException("Brother Bold not found.");
            }

            //Finding outside template: button
            Button button =  ((FrameworkContentElement)childBold1).FindName("button1") as Button;
            if (null != button)
            {
                throw new Microsoft.Test.TestValidationException("Should not search outside Template NameScope.");
            }

            //Finding outside: page
            object pageFound = ((FrameworkContentElement)childBold1).FindName("page");
            if (null != pageFound)
            {
                throw new Microsoft.Test.TestValidationException("Should not search outside Template NameScope.");
            }
        }


        /// <summary>
        /// Verification routine for FrameworkElementWithIDInControlTemplate.xaml.
        /// </summary>
        public static void FrameworkElementWithIDInControlTemplateVerify(UIElement root)
        {
            CoreLogger.LogStatus("Inside IdTestVerifiers.FrameworkElementWithIDInControlTemplateVerify()...");
            FrameworkElement fe = root as FrameworkElement;
            Canvas canvas = fe.FindName("sameName") as Canvas;

            CoreLogger.LogStatus("Finding Template ...");
            VerifyElement.VerifyBool(null==canvas, false);
            ResourceDictionary resources = canvas.Resources;
            ControlTemplate myControlTemplate = resources["myControlTemplate"] as ControlTemplate;
            VerifyElement.VerifyBool(null == myControlTemplate, false);

            CoreLogger.LogStatus("Finding buttons ...");
            Button button2 = ((FrameworkElement)root).FindName("button2") as Button;
            VerifyElement.VerifyBool(null == button2, false);
            Button button1 = ((FrameworkElement)root).FindName("button1") as Button;
            VerifyElement.VerifyBool(null == button1, false);

            //Finding from template
            FrameworkElement templateRoot = (FrameworkElement)VisualTreeUtils.GetChild(button1, 0);
            TextBlock childBlock11FoundFromTemplate = myControlTemplate.FindName("childBlock1", (FrameworkElement)templateRoot.TemplatedParent) as TextBlock;
            if(null == childBlock11FoundFromTemplate)
                throw new Microsoft.Test.TestValidationException("Cannot find childBlock1 outside Template.");

            //Finding from element in template.
            
            TextBlock parentBlock = (TextBlock)VisualTreeUtils.GetChild(templateRoot, 0);


            TextBlock childBlock1 = (TextBlock)parentBlock.FindName("childBlock1");

            //Finding the sameName
            TextBlock sameName = ((FrameworkElement)childBlock1).FindName("sameName") as TextBlock;
            if (null == sameName)
            {
                throw new Microsoft.Test.TestValidationException("sameName textBlock not found.");
            }

            //Finding self
            TextBlock childBlock11 = ((FrameworkElement)childBlock1).FindName("childBlock1") as TextBlock;
            if (null == childBlock1 || childBlock11 != childBlock1)
            {
                throw new Microsoft.Test.TestValidationException("Brother textBlock not found.");
            }

            if (childBlock11FoundFromTemplate != childBlock11)
            {
                throw new Microsoft.Test.TestValidationException("childBlock1 found outside and inside template are different.");
            }


            //Finding outside template: button
            Button button =  ((FrameworkElement)childBlock1).FindName("button1") as Button;
            if (null != button)
            {
                throw new Microsoft.Test.TestValidationException("Should not search outside Template NameScope.");
            }

            //Finding outside: page
            object pageFound = ((FrameworkElement)childBlock1).FindName("page");
            if (null != pageFound)
            {
                throw new Microsoft.Test.TestValidationException("Should not search outside Template NameScope.");
            }
        }

        /// <summary>
        /// Verification routine for FrameworkElementWithIDInDataTemplateVerify.xaml.
        /// </summary>
        public static void FrameworkElementWithIDInDataTemplateVerify(UIElement root)
        {
            CoreLogger.LogStatus("Inside IdTestVerifiers.FrameworkElementWithIDInDataTemplateVerify()...");
            FrameworkElement fe = root as FrameworkElement;
            Button button = fe.FindName("button") as Button;

            FrameworkElement cp = TemplateModelVerifiers.getFirstContentPresenter(button);
            Button templateRoot = (FrameworkElement)VisualTreeUtils.GetChild(cp, 0) as Button;

            //Finding from template
            DataTemplate template = button.ContentTemplate;
            TextBlock childBlock11FoundFromTemplate = template.FindName("childBlock1", cp) as TextBlock;
            if (null == childBlock11FoundFromTemplate)
            {
                throw new Microsoft.Test.TestValidationException("Cannot find childBlock1 from outside template.");
            }

            //Finding inside template
            TextBlock parentBlock = templateRoot.Content as TextBlock;

            TextBlock childBlock1 = ((FrameworkElement)parentBlock).FindName("childBlock1") as TextBlock;

            //Finding the sameName
            TextBlock sameName = ((FrameworkElement)parentBlock).FindName("sameName") as TextBlock;
            if (null == sameName)
            {
                throw new Microsoft.Test.TestValidationException("sameName textBlock not found.");
            }

            //Finding self
            TextBlock childBlock11 = ((FrameworkElement)childBlock1).FindName("childBlock1") as TextBlock;
            if (null == childBlock1 || childBlock11 != childBlock1)
            {
                throw new Microsoft.Test.TestValidationException("Brother textBlock not found.");
            }

            if (childBlock11FoundFromTemplate != childBlock1)
            {
                throw new Microsoft.Test.TestValidationException("childBlock1 found inside and outside the template are different.");
            }

            //Finding outside template: button
            Button button1 =  ((FrameworkElement)childBlock1).FindName("button") as Button;
            if (null != button1)
            {
                throw new Microsoft.Test.TestValidationException("Should not search outside Template NameScope.");
            }

            //Finding outside: page
            object pageFound = ((FrameworkElement)childBlock1).FindName("page");
            if (null != pageFound)
            {
                throw new Microsoft.Test.TestValidationException("Should not search outside Template NameScope.");
            }
        }

        /// <summary>
        /// Verification routine for FrameworkContentElementWithIDInDataTemplateVerify.xaml.
        /// </summary>
        public static void FrameworkContentElementWithIDInDataTemplateVerify(UIElement root)
        {
            CoreLogger.LogStatus("Inside IdTestVerifiers.FrameworkContentElementWithIDInDataTemplateVerify()...");
            FrameworkElement fe = root as FrameworkElement;
            Button button2 = fe.FindName("button2") as Button;

            FrameworkElement cp = TemplateModelVerifiers.getFirstContentPresenter(button2);
            FrameworkElement templateRoot = (FrameworkElement)VisualTreeUtils.GetChild(cp, 0) as FrameworkElement;
            //Finding from the template
            DataTemplate template = button2.ContentTemplate;
            Bold childBold1FoundFromTemplate = template.FindName("childBold1", cp) as Bold;
            if (null == childBold1FoundFromTemplate)
                throw new Microsoft.Test.TestValidationException("Cannot find childBold1 from outside template.");

            Bold childBold1 = templateRoot.FindName("childBold1") as Bold;
            if (null == childBold1)
            {
                throw new Microsoft.Test.TestValidationException("Cannot find childBold1.");
            }
            if (childBold1FoundFromTemplate != childBold1)
            {
                throw new Microsoft.Test.TestValidationException("childBold1 found inside and outside the template are different.");
            }


            //Finding the sameName
            Bold sameName = ((FrameworkContentElement)childBold1).FindName("sameName") as Bold;
            if (null == sameName)
            {
                throw new Microsoft.Test.TestValidationException("sameName Bold not found.");
            }

            //Finding self
            Bold childBold11 = ((FrameworkContentElement)childBold1).FindName("childBold1") as Bold;
            if (null == childBold1 || childBold11 != childBold1)
            {
                throw new Microsoft.Test.TestValidationException("Brother Bold not found.");
            }

            //Finding outside template: button
            Button button =  ((FrameworkContentElement)childBold1).FindName("button1") as Button;
            if (null != button)
            {
                throw new Microsoft.Test.TestValidationException("Should not search outside Template NameScope.");
            }

            //Finding outside: page
            object pageFound = ((FrameworkContentElement)childBold1).FindName("page");
            if (null != pageFound)
            {
                throw new Microsoft.Test.TestValidationException("Should not search outside Template NameScope.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        public static void ItemsControlWithDataTemplateVerify(UIElement root)
        {
            CoreLogger.LogStatus("Inside IdTestVerifiers.ItemsControlWithDataTemplateVerify()...");
            FrameworkElement fe = root as FrameworkElement;
            TreeHelper.WaitForTimeManager();

            //Verify sameName Panel

            DockPanel sameNameOutside = fe.FindName("sameName") as DockPanel;
            VerifyElement.VerifyBool(null == sameNameOutside, false);

            ItemsControl control = ((FrameworkElement)root).FindName("itemsControl") as ItemsControl;
            VerifyElement.VerifyBool(null == control, false);

            FrameworkElement cp = TemplateModelVerifiers.getFirstContentPresenter(control);
            FrameworkElement templateRoot = VisualTreeUtils.GetChild(cp, 0) as FrameworkElement;

            //Finding from the template
            DataTemplate template = control.ItemTemplate;
            TextBlock childBlock1FoundFromTemplate = template.FindName("childBlock1", cp) as TextBlock;
            if (null == childBlock1FoundFromTemplate)
            {
                throw new Microsoft.Test.TestValidationException("Finding a TextBlock from Template failed.");
            }

            //Finding inside Template
            TextBlock childBlock1 = ((FrameworkElement)templateRoot).FindName("childBlock1") as TextBlock;
            if (null == childBlock1 || childBlock1 != childBlock1FoundFromTemplate)
            {
                throw new Microsoft.Test.TestValidationException("Finding a childBlock1 failed.");
            }
            if (null == childBlock1FoundFromTemplate)
            {
                throw new Microsoft.Test.TestValidationException("Finding a TextBlock from Template failed.");
            }

            //Finding the sameName
            TextBlock sameNameInside = templateRoot.FindName("sameName") as TextBlock;
            if (null == sameNameInside)
            {
                throw new Microsoft.Test.TestValidationException("sameName TextBlock in Template not found.");
            }

            //Finding self
            TextBlock childBlock11 = ((FrameworkElement)childBlock1).FindName("childBlock1") as TextBlock;
            if (null == childBlock11 || childBlock11 != childBlock1)
            {
                throw new Microsoft.Test.TestValidationException("self TextBlock not found.");
            }
            
            //Finding Brother in template
            TextBlock childBlock2 = ((FrameworkElement)childBlock1).FindName("childBlock2") as TextBlock;
            if (null == childBlock2)
            {
                throw new Microsoft.Test.TestValidationException("Brother TextBlock not found.");
            }

            //Finding outside template
            object pageFound = templateRoot.FindName("page");
            if (null != pageFound)
            {
                throw new Microsoft.Test.TestValidationException("Should not search outside Template NameScope.");
            }
            //Verifying databinding
            if (!String.Equals(sameNameInside.Text, childBlock1.Text, StringComparison.InvariantCulture))
            {
                throw new Microsoft.Test.TestValidationException("DataBinding in DataTemplate failed: >" + childBlock1.Text + "< vs >" + sameNameInside.Text + "<.");
            }
        }
        
        /// <summary>
        /// Do a depth-first search of the visual tree looking for a node with
        /// a given property value.
        /// </summary>
        /// <param name="dp">property to query</param>
        /// <param name="value">desired value</param>
        /// <param name="node">starting node for the search</param>
        /// <param name="includeNode">if false, do not test the node itself</param>
        /// <returns></returns>
        public static DependencyObject FindVisualByPropertyValue(DependencyProperty dp, object value, DependencyObject node, bool includeNode)
        {
            // see if the node itself has the right value
            if (includeNode)
            {
                object nodeValue = node.GetValue(dp);
                if (Object.Equals(value, nodeValue))
                {
                    return node;
                }
            }

            // if not, recursively look at the visual children
            int count = VisualTreeHelper.GetChildrenCount(node);
            for(int i = 0; i < count; i++)
            {
                // Common base class for Visual and Visual3D is DependencyObject

                DependencyObject result = FindVisualByPropertyValue(dp, value, VisualTreeHelper.GetChild(node,i), true);
                if (result != null)
                {
                    return result;
                }
            }

            // not found
            return null;
        }

        /// <summary>
        /// Do a depth-first search of the visual tree looking for a node with
        /// a given type.
        /// </summary>
        /// <param name="type">type of desired node</param>
        /// <param name="node">starting node for the search</param>
        /// <param name="includeNode">if false, do not test the node itself</param>
        public static DependencyObject FindVisualByType(Type type, DependencyObject node, bool includeNode)
        {
            // see if the node itself has the right type
            if (includeNode)
            {
                if (type == node.GetType())
                {
                    return node;
                }
            }

            // if not, recursively look at the visual children
            int count = VisualTreeHelper.GetChildrenCount(node);
            for(int i = 0; i < count; i++)
            {
                // Common base class for Visual and Visual3D is DependencyObject

                DependencyObject result = FindVisualByType(type, VisualTreeHelper.GetChild(node,i), true);
                if (result != null)
                {
                    return result;
                }
            }

            // not found
            return null;
        }

        /// <summary>
        /// Verification routine for FrameworkContentElementWithId.xaml.
        /// </summary>
        /// <param name="root"></param>
        public static void FrameworkContentElementWithIdVerify(UIElement root)
        {
            CoreLogger.LogStatus("Inside IdTestVerifiers.FrameworkContentElementWithIdVerify()...");
            VerifyAnElementWithID(root, "bold", typeof(Bold));
            Bold bold = (Bold)IdTestBaseCase.FindElementWithId(root, "bold");

            //Verify that the field bold is correct
            IdTestBaseCase.VerifyFieldValue(root, "bold", bold);
        }

        /// <summary>
        /// Verification routine for FrameworkContentElementWithxId.xaml.
        /// </summary>
        /// <param name="root"></param>
        public static void FrameworkContentElementWithxIdVerify(UIElement root)
        {
            CoreLogger.LogStatus("Inside IdTestVerifiers.FrameworkContentElementWithxIdVerify()...");
            //Verify fields
               
            Bold bold2 = (Bold)IdTestBaseCase.FindElementWithId(root, "bold2");
                
            //Verify that the field bold2 is correct

            IdTestBaseCase.VerifyFieldValue(root, "bold2", bold2);

            //Verify element with x:Name
            VerifyAnElementWithID(root, "bold2", typeof(Bold));
            VerifyElement.VerifyString(new TextRange(bold2.ContentStart, bold2.ContentEnd).Text as string, "content2");
        }

        /// <summary>
        /// Verification routine for FrameworkContentElementWithIdAndxId.xaml.
        /// </summary>
        /// <param name="root"></param>
        public static void FrameworkContentElementWithIdAndxIdVerify(UIElement root)
        {
            CoreLogger.LogStatus("Inside IdTestVerifiers.FrameworkContentElementWithIdAndxIdVerify()...");
            //Verify Id-ed element
            FrameworkContentElementWithIdVerify(root);
            //Verify xId-ed element
            FrameworkContentElementWithxIdVerify(root);
        }

        /// <summary>
        /// Verification routine for NameScopeInStyle.xaml
        /// </summary>
        /// <param name="root"></param>
        public static void NameScopeInStyleVerify(UIElement root)
        {
            CoreLogger.LogStatus("Inside IdTestVerifiers.NameScopeInStyleVerify()...");

            //Verify element with id in style visual tree
            Canvas canvasWithResources = (Canvas)IdTestBaseCase.FindElementWithId(root, "canvas");
            ResourceDictionary dictionary = canvasWithResources.Resources;
            CoreLogger.LogStatus("number of resource entries: " + dictionary.Count.ToString());
            Style style = dictionary["style"] as Style;
            CoreLogger.LogStatus("Does style implement INameScope?");
            //Verify the style has implement INameScope
            VerifyElement.VerifyBool(style is INameScope, true);

            //verify the element defined outside style cannot be found.
            Canvas canvas = ((INameScope)style).FindName("canvas") as Canvas;
            VerifyElement.VerifyBool(null == canvas, true);

            //verify the element defined in template cannot be found.
            Canvas canvasInTemplate = ((INameScope)style).FindName("id") as Canvas;
            VerifyElement.VerifyBool(null == canvasInTemplate, true);

            //Verify the timeline with Name property in storyboard of style has been added.
            BeginStoryboard storyboard = ((INameScope)style).FindName("ChangeWidth") as BeginStoryboard;
            VerifyElement.VerifyBool(null == storyboard, false);

            //Verify the timeline with x:Name property in storyboard of style has been added.
            storyboard = ((INameScope)style).FindName("ChangeHeight") as BeginStoryboard;
            VerifyElement.VerifyBool(null == storyboard, false);

            //Verify that Style can set a Name, but it has not been added to the Name Scope
            object styleFound = ((INameScope)style).FindName("button");
            VerifyElement.VerifyBool(null == styleFound, true);
        }

        /// <summary>
        /// Verification routine for NameScopeInStyle1.xaml
        /// </summary>
        /// <param name="root"></param>
        public static void NameScopeInStyle1Verify(UIElement root)
        {
            CoreLogger.LogStatus("Inside IdTestVerifiers.NameScopeInStyleVerify()...");

            //Verify element with id in style visual tree
            Button button = (Button)IdTestBaseCase.FindElementWithId(root, "buttonWithStyle");
            VerifyElement.VerifyBool(null == button, false);
            Style style = button.Style;
            VerifyElement.VerifyBool(null == style, false);
            CoreLogger.LogStatus("Finding the style from NameScope... ");
            //find style from its own namespece.
            Style style1 = ((INameScope)style).FindName("style") as Style;
            VerifyElement.VerifyBool(null == style1, true);
            //find style from otherside.
            style1 = (Style)IdTestBaseCase.FindElementWithId(root, "style");
            VerifyElement.VerifyBool(null == style1, false);
        }
        /// <summary>
        ///  Verify FrameworkElementWithIDUnderCustomINameScopeWithSameIdsInDifferentScope.xaml.
        /// </summary>
        /// <param name="root"></param>
        public static void FrameworkElementWithIDUnderCustomINameScopeWithSameIdsInDifferentScopeVerify(UIElement root)
        {
            CoreLogger.LogStatus("Inside IdTestVerifiers.FrameworkElementWithIDUnderCustomINameScopeWithSameIdsInDifferentScopeVerify()...");

            CustomINameScope CustomINameScope1 = (CustomINameScope)IdTestBaseCase.FindElementWithId(root, "CustomINameScope1");
            VerifyElement.VerifyBool(null != CustomINameScope1, true);
            VerifyElement.VerifyInt(CustomINameScope1.ElementWithIDCount, 5);

            CustomINameScope CustomINameScope2 = (CustomINameScope)IdTestBaseCase.FindElementWithId(CustomINameScope1, "CustomINameScope2");
            VerifyElement.VerifyBool(null != CustomINameScope2, true);
            VerifyElement.VerifyInt(CustomINameScope2.ElementWithIDCount, 1);

            CoreLogger.LogStatus("Verify the button found from CustomINameScope1 is correct. ");
            Button button21 = (Button)IdTestBaseCase.FindElementWithId(CustomINameScope1, "button2");
            VerifyElement.VerifyString(button21.Content.ToString(), "Outside CustomINameScope3");

            CustomINameScope CustomINameScope3 = (CustomINameScope)IdTestBaseCase.FindElementWithId(CustomINameScope1, "CustomINameScope3");
            VerifyElement.VerifyBool(null != CustomINameScope3, true);
            VerifyElement.VerifyInt(CustomINameScope3.ElementWithIDCount, 2);

            CoreLogger.LogStatus("Verify the button found from CustomINameScope3 is correct. ");
            Button button22 = (Button)IdTestBaseCase.FindElementWithId(CustomINameScope3, "button2");
            VerifyElement.VerifyString(button22.Content.ToString(), "Inside CustomINameScope3");
        }
 
        /// <summary>
        /// Verify StoryboardWithName.xaml.
        /// </summary>
        /// <param name="root"></param>
        public static void StoryboardWithNameVerify(UIElement root)
        {
            FrameworkElement fe = root as FrameworkElement;
            CoreLogger.LogStatus("Inside IdTestVerifiers.StoryboardWithNameVerify()...");
            TreeHelper.WaitForTimeManager();

            VerifyAnElementWithID(root, "button", typeof(Button));

            Button button = (Button)IdTestBaseCase.FindElementWithId(root, "button");
            
            //Verify that the field for storyboard exists            
            IdTestBaseCase.VerifyFieldExist(root, "storyboardOnElement");

            //Verify id-ed element has been added to IdScope can can be found
            CoreLogger.LogStatus("Verify pairs in IdScope ...");

            Storyboard storyboardOnElement = IdTestBaseCase.FindElementWithId(root, "storyboardOnElement") as Storyboard;
            VerifyElement.VerifyBool(storyboardOnElement != null, true);
            
            //Verify timeline in style
            ResourceDictionary rd = fe.Resources;
            VerifyElement.VerifyBool(rd != null, true);
            VerifyElement.VerifyBool(rd.Count == 1, true);
            Style style = rd["Style"] as Style;

            CoreLogger.LogStatus("Verify timeline has been added to style INameScope.");
            Storyboard storyboardInStyle = ((INameScope)style).FindName("storyboardInStyle") as Storyboard;
            VerifyElement.VerifyBool(null != storyboardInStyle, true);

            SetterBaseCollection setters = style.Setters;
            VerifyElement.VerifyBool(setters != null, true);
            VerifyElement.VerifyInt(setters.Count, 2);
            Setter templateSetter = setters[1] as Setter;
            VerifyElement.VerifyBool(templateSetter != null, true);
            ControlTemplate template = templateSetter.Value as ControlTemplate;

            VerifyElement.VerifyBool(template != null, true);

            Storyboard storyboardInTemplate = template.FindName("storyboardInTemplate", button) as Storyboard;
            //Block on 


            //Verify the effect of the storyboards
            Button button2 = (Button)IdTestBaseCase.FindElementWithId(root, "button2");
            VerifyElement.VerifyBool(null != button2, true);
            VerifyElement.VerifyDouble(button2.Width, 50);
            VerifyElement.VerifyDouble(button.Width, 150);
            VerifyElement.VerifyDouble(button.Height, 80);
        }

        /// <summary>
        /// Verify StoryboardDatabinding.xaml.
        /// </summary>
        /// <param name="root"></param>
        public static void StoryboardDatabindingVerify(UIElement root)
        {
            FrameworkElement fe = root as FrameworkElement;
            CoreLogger.LogStatus("Inside IdTestVerifiers.StoryboardDatabindingVerify()...");

            Button button = (Button)IdTestBaseCase.FindElementWithId(root, "button");
            
            //Verify the effect of the storyboards
            VerifyElement.VerifyDouble(button.Width, 200);
        }

        /// <summary>
        /// Verify DirectTargetingFreezable.xaml.
        /// </summary>
        /// <param name="root"></param>
        public static void DirectTargetingFreezableVerify(UIElement root)
        {
            FrameworkElement fe = root as FrameworkElement;
            CoreLogger.LogStatus("Inside IdTestVerifiers.DirectTargetingFreezableVerify()...");
            TreeHelper.WaitForTimeManager();

            //Verify the effect of direct targeting in logical tree
            Button targetButton = (Button)IdTestBaseCase.FindElementWithId(root, "targetButton");
            VerifyElement.VerifyBool(null != targetButton, true);
            VerifyElement.VerifyColor(((SolidColorBrush)targetButton.Background).Color, Colors.Red);
            
            //Verify the effect of direct targeting in Template
            Button buttonwithStyle = (Button)IdTestBaseCase.FindElementWithId(root, "buttonwithStyle");
            VerifyElement.VerifyBool(null != buttonwithStyle, true);
            Border border = buttonwithStyle.Template.FindName("border", buttonwithStyle) as Border;
            VerifyElement.VerifyBool(null != border, true);
            VerifyElement.VerifyColor(((SolidColorBrush)border.Background).Color, Colors.Red);
        }

        /// <summary>
        /// Verify NonDoWithxId.xaml.
        /// </summary>
        /// <param name="root"></param>
        public static void NonDoWithxIdVerify(UIElement root)
        {
            CoreLogger.LogStatus("Inside IdTestVerifiers.NonDoWithxIdVerify()...");

            //Verify fields
            Application currentApplication = Application.Current;
            Window window = currentApplication.MainWindow;

            Object contentObject = window.Content;

            CoreLogger.LogStatus("Verify a field contentObject has been created and the value is correct.");

            IdTestBaseCase.VerifyFieldValue(window, "contentObj", contentObject);


            CoreLogger.LogStatus("Verify the object can be found from IIdscope.");
            Object foundObj = (Object)IdTestBaseCase.FindElementWithId(window, "contentObj");
            VerifyElement.VerifyBool(contentObject == foundObj, true);
        }
        /// <summary>
        /// Verify ForwardReferenceingOfIdUsingDataBindingInTemplate.xaml.
        /// </summary>
        /// <param name="root"></param>
        public static void ForwardReferenceingOfIdUsingDataBindingInTemplateVerify(UIElement root)
        {
            CoreLogger.LogStatus("Inside IdTestVerifiers.ForwardReferenceingOfIdUsingDataBindingInTemplateVerify()...");
            FrameworkElement fe = root as FrameworkElement;
            Button button = fe.FindName("button") as Button;
            VerifyElement.VerifyBool(null == button, false);

            ControlTemplate template = button.Template;
            VerifyElement.VerifyBool(null == template, false);

            //Check binding to its next sibling
            CheckBox cb1 = template.FindName("cb1", button) as CheckBox;
            VerifyElement.VerifyBool(null == cb1, false);

            //Serialization of databinding in template won't work. Feature cut.
            //VerifyElement.VerifyBool((bool)(cb1.IsChecked), true);
            
            //Check binding to its child
            Button buttonInTemplate = template.FindName("buttonInTemplate", button) as Button;

            VerifyElement.VerifyBool(null == buttonInTemplate, false);

            //VerifyElement.VerifyColor(((SolidColorBrush)buttonInTemplate.Background).Color, Colors.Blue);

        }

        /// <summary>
        /// Verify ForwardReferenceingOfIdUsingDataBinding.xaml.
        /// </summary>
        /// <param name="root"></param>
        public static void ForwardReferenceingOfIdUsingDataBindingVerify(UIElement root)
        {
            CoreLogger.LogStatus("Inside IdTestVerifiers.ForwardReferenceingOfIdUsingDataBindingVerify()...");

            //Verfiy that forward reference on button1.Content
            Button button1 = (Button)IdTestBaseCase.FindElementWithId(root, "button1");
            VerifyElement.VerifyBool(null != button1, true);
            String content = button1.Content as String;
            VerifyElement.VerifyBool(null != content, true);
            VerifyElement.VerifyString(content, "bold text");

            //Verfiy that forward reference on button2.Background
            Button button2 = (Button)IdTestBaseCase.FindElementWithId(root, "button2");
            VerifyElement.VerifyBool(null != button2, true);

            object background = button2.Background;
            if (null == background)
            {
                CoreLogger.LogStatus("No background?");
            }
            
            CoreLogger.LogStatus(background.GetType().ToString());
            SolidColorBrush sbackground = button2.Background as SolidColorBrush;
            VerifyElement.VerifyBool(null != sbackground, true);
            VerifyElement.VerifyColor(sbackground.Color, Colors.Blue);
        }
        /// <summary>
        /// Verification routine for BindingFromStyleTriggerToLogicalTree.xaml.
        /// </summary>
        public static void BindingFromStyleTriggerVerify(UIElement root)
        {
            CoreLogger.LogStatus("Inside IdTestVerifiers.BindingFromStyleTriggerToLogicalTreeVerify()...");

            //Verify that the background of the button, which is a value set in Style.Trigger binding
            //to an element in the logical tree, has expected value.
            Button button = (Button)IdTestBaseCase.FindElementWithId(root, "button");
            VerifyElement.VerifyBool(null != button, true);

            CoreLogger.LogStatus(button.Background.GetType().ToString());
            SolidColorBrush sbackground = button.Background as SolidColorBrush;
            VerifyElement.VerifyBool(null != sbackground, true);

            VerifyElement.VerifyColor(sbackground.Color, Colors.Blue);
            //Use the Height of the button, which is a value bind to an element in Template
            // to verify the the binding doesn't work. 
            VerifyElement.VerifyBool(button.Height != 40, true);
        }

        /// <summary>
        /// Verification routine for DataBindingInControlTemplate.xaml.
        /// </summary>
        public static void DataBindingInControlTemplateVerify(UIElement root)
        {
            CoreLogger.LogStatus("Inside IdTestVerifiers.DataBindingInControlTemplateVerify()...");

            //Verify that the background of the button, which is a value set in Style.Trigger binding
            //to an element in the logical tree, has expected value.
            Button button = (Button)IdTestBaseCase.FindElementWithId(root, "button");
            VerifyElement.VerifyBool(null != button, true);

            ControlTemplate template = button.Template;
            VerifyElement.VerifyBool(null != template, true);
            
            Button buttonInTemplate = template.FindName("buttonInTemplate", button) as Button;
            VerifyElement.VerifyBool(null != buttonInTemplate, true);
            //Verify Width of buttonInTemplate, whose value is binding to Width 
            // of a element in the logical tree. This binding should not work.
            VerifyElement.VerifyDouble(buttonInTemplate.Width, 50);
            //Verify Height, whose value is assigned with a EventTrigger sources from 
            //another element in the Template.
            Button childButtonInTemplate = template.FindName("childButtonInTemplate", button) as Button;
            VerifyElement.VerifyBool(null != childButtonInTemplate, true);

            SolidColorBrush sbackground = childButtonInTemplate.Background as SolidColorBrush;
            VerifyElement.VerifyBool(null != sbackground, true);
            VerifyElement.VerifyColor(sbackground.Color, Colors.Blue);
        }

        /// <summary>
        /// Verification routine for DataBindingInDataTemplate.xaml.
        /// </summary>
        public static void DataBindingInDataTemplateVerify(UIElement root)
        {
            CoreLogger.LogStatus("Inside IdTestVerifiers.DataBindingInDataTemplateVerify()...");

            //Verify that the background of the button, which is a value set in Style.Trigger binding
            //to an element in the logical tree, has expected value.
            Button button = (Button)IdTestBaseCase.FindElementWithId(root, "button");
            VerifyElement.VerifyBool(null != button, true);

            DataTemplate template = button.ContentTemplate;
            VerifyElement.VerifyBool(null != template, true);

            FrameworkElement cp = TemplateModelVerifiers.getFirstContentPresenter(button);
            VerifyElement.VerifyBool(null != cp, true);

            Button buttonInTemplate = template.FindName("buttonInTemplate", cp) as Button;
            VerifyElement.VerifyBool(null != buttonInTemplate, true);
            //Verify Width of buttonInTemplate, whose value is binding to Width 
            // of a element in the logical tree. This binding should not work.
            VerifyElement.VerifyDouble(buttonInTemplate.Width, 50);
            //Verify Height, whose value is assigned with a EventTrigger sources from 
            //another element in the Template.
            Button childButtonInTemplate = template.FindName("childButtonInTemplate", cp) as Button;
            VerifyElement.VerifyBool(null != childButtonInTemplate, true);

            SolidColorBrush sbackground = childButtonInTemplate.Background as SolidColorBrush;
            VerifyElement.VerifyBool(null != sbackground, true);
            VerifyElement.VerifyColor(sbackground.Color, Colors.Blue);            
        }


        /// <summary>
        /// Verification routine for TriggersInDataTemplate.xaml.
        /// </summary>
        public static void TriggersInDataTemplateVerify(UIElement root)
        {
            CoreLogger.LogStatus("Inside IdTestVerifiers.TriggersInDataTemplateVerify()...");
            TreeHelper.WaitForTimeManager();

            //Verify that the background of the button, which is a value set in Style.Trigger binding
            //to an element in the logical tree, has expected value.
            Button button = (Button)IdTestBaseCase.FindElementWithId(root, "button");
            VerifyElement.VerifyBool(null != button, true);

            DataTemplate template = button.ContentTemplate;
            VerifyElement.VerifyBool(null != template, true);
            
            FrameworkElement cp = TemplateModelVerifiers.getFirstContentPresenter(button);
            VerifyElement.VerifyBool(null != cp, true);

            Button buttonInTemplate = template.FindName("buttonInTemplate", cp) as Button;
            VerifyElement.VerifyBool(null != buttonInTemplate, true);
        }

        /// <summary>
        /// Verification routine for TriggersInControlTemplate.xaml.
        /// </summary>
        public static void TriggersInControlTemplateVerify(UIElement root)
        {
            CoreLogger.LogStatus("Inside IdTestVerifiers.TriggersInControlTemplateVerify()...");

            TreeHelper.WaitForTimeManager();

            //Verify that the background of the button, which is a value set in Style.Trigger binding
            //to an element in the logical tree, has expected value.
            Button button = (Button)IdTestBaseCase.FindElementWithId(root, "button");
            VerifyElement.VerifyBool(null != button, true);

            ControlTemplate template = button.Template;
            VerifyElement.VerifyBool(null != template, true);

            Button buttonInTemplate = template.FindName("buttonInTemplate", button) as Button;
            VerifyElement.VerifyBool(null != buttonInTemplate, true);
            //Verify Width, whose value is assigned with a property Trigger sources from 
            //another element in the Template.
            VerifyElement.VerifyDouble(buttonInTemplate.Width, 300);
            //Verify Height, whose value is assigned with a EventTrigger sources from 
            //another element in the Template.
            VerifyElement.VerifyDouble(buttonInTemplate.Height, 500);
        }

        /// <summary>
        ///  Verify an element with a Name under a logical tree.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="id"></param>
        /// <param name="elementType"></param>
        public static void VerifyAnElementWithID(UIElement root, string id, Type elementType)
        {
            VerifyAnElementWithID(root, id, elementType, null);
        }

        /// <summary>
        /// Verify an element with a Name is under a logical tree and the element found has expected value
        /// </summary>
        /// <param name="root"></param>
        /// <param name="id"></param>
        /// <param name="elementType"></param>
        /// <param name="expectedValue"></param>
        public static void VerifyAnElementWithID(UIElement root, string id, Type elementType, object expectedValue)
        {
            CoreLogger.LogStatus("Verifying an element with an Id: " + id + ".");
            object element = IdTestBaseCase.FindElementWithId(root, id);
            if (null == element)
            {
                throw new Microsoft.Test.TestValidationException("element with Id: " + id + " not found.");
            }

            VerifyElement.VerifyBool(element.GetType().Equals(elementType) || element.GetType().IsSubclassOf(elementType), true);

            //Finding from the element

            object foundElement = null;
            if (element is FrameworkElement)
            {
                foundElement = ((FrameworkElement)element).FindName(id);
            }
            else if (element is FrameworkContentElement)
            {
                foundElement = ((FrameworkContentElement)element).FindName(id);
            }
            else
            {
                throw new Microsoft.Test.TestValidationException("Cannot call Findid on this element.");
            }
            VerifyElement.VerifyBool(foundElement.GetType().Equals(elementType) || foundElement.GetType().IsSubclassOf(elementType), true);
            VerifyElement.VerifyBool(foundElement == element, true);
            if (null != expectedValue)
            {
                VerifyElement.VerifyBool(foundElement == expectedValue, true);
            }
        }
    }
}


