// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Animation
{
    using System;
    using System.Collections;
    using System.Windows; 
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data; 
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Navigation;

    using Microsoft.Test.Logging;


    /******************************************************************************
    *******************************************************************************
    * CLASS:          AnimationUtilities
    *******************************************************************************
    ******************************************************************************/
    /// <summary>
    /// Contains utility functions for testing Animation.
    /// </summary>
    public static class AnimationUtilities
    {
        /******************************************************************************
        * Function:          CreateBeginStoryboard
        ******************************************************************************/
        /// <summary>
        /// Create a BeginStoryboard, registering it on a FrameworkElement.
        /// </summary>
        /// <returns>A new BeginStoryboard object</returns>
        public static BeginStoryboard CreateBeginStoryboard(FrameworkElement element, string name)
        {
            BeginStoryboard beginStory = new BeginStoryboard();
            beginStory.Name = name;
            element.RegisterName(beginStory.Name, beginStory);
            
            return beginStory;
        }
        
        /******************************************************************************
        * Function:          CreateBeginStoryboard
        ******************************************************************************/
        /// <summary>
        /// Create a BeginStoryboard with a HandoffBehavior, registering it on a FrameworkElement.
        /// </summary>
        /// <returns>A new BeginStoryboard object</returns>
        public static BeginStoryboard CreateBeginStoryboard(FrameworkElement element, string name, HandoffBehavior handoffBehavior)
        {
            BeginStoryboard beginStory = new BeginStoryboard();
            beginStory.Name = name;
            beginStory.HandoffBehavior = handoffBehavior;
            element.RegisterName(beginStory.Name, beginStory);
            
            return beginStory;
        }
        
        /******************************************************************************
        * Function:          CreateBeginStoryboard
        ******************************************************************************/
        /// <summary>
        /// Create a BeginStoryboard, registering it on a FrameworkContentElement.
        /// </summary>
        /// <returns>A new BeginStoryboard object</returns>
        public static BeginStoryboard CreateBeginStoryboard(FrameworkContentElement element, string name)
        {
            BeginStoryboard beginStory = new BeginStoryboard();
            beginStory.Name = name;
            element.RegisterName(beginStory.Name, beginStory);
            
            return beginStory;
        }
        
        /******************************************************************************
        * Function:          CreateBeginStoryboard
        ******************************************************************************/
        /// <summary>
        /// Create a BeginStoryboard with a HandoffBehavior, registering it on a FrameworkContentElement.
        /// </summary>
        /// <returns>A new BeginStoryboard object</returns>
        public static BeginStoryboard CreateBeginStoryboard(FrameworkContentElement element, string name, HandoffBehavior handoffBehavior)
        {
            BeginStoryboard beginStory = new BeginStoryboard();
            beginStory.Name = name;
            beginStory.HandoffBehavior = handoffBehavior;
            element.RegisterName(beginStory.Name, beginStory);
            
            return beginStory;
        }
        
        /******************************************************************************
        * Function:          CreateEventTrigger
        ******************************************************************************/
        /// <summary>
        /// Create a new EventTrigger, set its RoutedEvent, and add a BeginStoryboard.
        /// </summary>
        /// <returns>A new EventTrigger, with a BeginStoryboard added</returns>
        public static EventTrigger CreateEventTrigger(RoutedEvent routedEvent, BeginStoryboard beginStory)
        {
            EventTrigger eventTrigger = new EventTrigger();
            eventTrigger.RoutedEvent = routedEvent;
            eventTrigger.Actions.Add(beginStory);
            
            return eventTrigger;
        }
        
        /******************************************************************************
        * Function:          CreateEventTrigger
        ******************************************************************************/
        /// <summary>
        /// Create a new EventTrigger, set its RoutedEvent and SourceName, and add a BeginStoryboard.
        /// </summary>
        /// <returns>A new EventTrigger, with a BeginStoryboard added</returns>
        public static EventTrigger CreateEventTrigger(RoutedEvent routedEvent, BeginStoryboard beginStory, string sourceName)
        {
            EventTrigger eventTrigger = new EventTrigger();
            eventTrigger.RoutedEvent = routedEvent;
            eventTrigger.SourceName = sourceName;
            eventTrigger.Actions.Add(beginStory);
            
            return eventTrigger;
        }
        
        /******************************************************************************
        * Function:          CreatePropertyTrigger
        ******************************************************************************/
        /// <summary>
        /// Create a new Property Trigger, set its Property and Value, and add a BeginStoryboard.
        /// </summary>
        /// <returns>A new Property Trigger, with DP/Value and BeginStoryboard added</returns>
        public static Trigger CreatePropertyTrigger(DependencyProperty dp, object value, BeginStoryboard beginStory)
        {
            Trigger propertyTrigger = new Trigger();
            propertyTrigger.Property    = dp;
            propertyTrigger.Value       = value;
            propertyTrigger.EnterActions.Add(beginStory);
            
            return propertyTrigger;
        }
        
        /******************************************************************************
        * Function:          CreateDataTrigger
        ******************************************************************************/
        /// <summary>
        /// Create a new DataTrigger, set its Binding (with PropertyPath) and Value, and 
        ///add a BeginStoryboard.
        /// </summary>
        /// <returns>A new DataTrigger, with PropertyPath/Value and BeginStoryboard added</returns>
        public static DataTrigger CreateDataTrigger(PropertyPath path, object value, BeginStoryboard beginStory)
        {
            Binding binding = CreateBinding(path, RelativeSource.Self);

            DataTrigger dataTrigger = new DataTrigger();
            dataTrigger.Binding  = binding;
            dataTrigger.Value    = value;
            dataTrigger.EnterActions.Add(beginStory);
            
            return dataTrigger;
        }
        
        /******************************************************************************
        * Function:          CreateBinding
        ******************************************************************************/
        /// <summary>
        /// Create a new Binding.
        /// </summary>
        /// <returns>A new Binding, with PropertyPath and RelativeSource added</returns>
        public static Binding CreateBinding(PropertyPath path, RelativeSource relativeSource)
        {
            Binding binding = new Binding();
            binding.RelativeSource    = relativeSource;
            binding.Path              = path;
            
            return binding;
        }
        
        /******************************************************************************
        * Function:          CreateMultiDataTrigger
        ******************************************************************************/
        /// <summary>
        /// Create a new MultiDataTrigger and add a BeginStoryboard as an EnterAction.
        /// </summary>
        /// <returns>A new MultiDataTrigger, with BeginStoryboard added</returns>
        public static MultiDataTrigger CreateMultiDataTrigger(BeginStoryboard beginStory)
        {
            MultiDataTrigger mdTrigger = new MultiDataTrigger();
            mdTrigger.EnterActions.Add(beginStory);

            return mdTrigger;
        }
        
        /******************************************************************************
        * Function:          CreateMultiTrigger
        ******************************************************************************/
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static MultiTrigger CreateMultiTrigger(BeginStoryboard beginStory)
        {
            MultiTrigger multiTrigger = new MultiTrigger();
            multiTrigger.EnterActions.Add(beginStory);

            return multiTrigger;
        }
        
        /******************************************************************************
        * Function:          CreateCondition
        ******************************************************************************/
        /// <summary>
        /// CreateCondition: creates a Condition object for a MultiDataTrigger.
        /// A binding is passed in, with RelativeSource and Path already set.
        /// </summary>
        /// <returns></returns>
        public static Condition CreateCondition(Binding binding, object value)
        {
            Condition condition = new Condition();
            condition.Binding = binding;
            condition.Value = value;
            
            return condition;
        }
        
        /******************************************************************************
        * Function:          CreateCondition
        ******************************************************************************/
        /// <summary>
        /// CreateCondition: creates a Condition object for a MultiTrigger.
        /// A DependencyProperty is passed in.
        /// </summary>
        /// <returns></returns>
        public static Condition CreateCondition(DependencyProperty dp, object value)
        {
            Condition condition = new Condition();
            condition.Property = dp;
            condition.Value = value;
            
            return condition;
        }
        
        /******************************************************************************
           * Function:          GetBeginStoryboard
           ******************************************************************************/
        /// <summary>
        /// GetBeginStoryboard: retrieve a BeginStoryboard, which can reside in different
        /// places in Markup:  (a) on the root element, (b) on the animated element, (c) inside
        /// a Style, or (d) inside a (Property) Trigger in a Style.
        ///     rootElement: the root element in Markup
        ///     animatedElement: the DO being animated in Markup
        /// </summary>
        /// <returns>The BeginStoryboard object from Markup</returns>
        public static BeginStoryboard GetBeginStoryboard(FrameworkElement rootElement, FrameworkElement animatedElement)
        {
            Style               style               = null;
            EventTrigger        eventTrigger        = null;
            Trigger             trigger             = null;
            BeginStoryboard     beginStoryboard     = null;
            bool                triggerInStyle      = false;

            
            //Look for a Style.  If found and has an Trigger, get the Trigger from the Style
            //NOTE:  Markup containing a Property Trigger will also have an additional animation
            //used to invoke the Property Trigger.  Only the Property Trigger will be verified.
            if (rootElement.Style != null)
            {
                style = (Style)rootElement.Style;
            }
            else if (animatedElement.Style != null)
            {
                style = (Style)animatedElement.Style;
            }

            if (style != null)
            {
                if (style.Triggers.Count != 0)
                {
                    triggerInStyle = true;
                    
                    //Look for an EventTrigger or Trigger in Style.Triggers.
                    object obj = style.Triggers[0];
                    Type t = obj.GetType();
                    
                    Trigger tr = new Trigger();
                    EventTrigger et = new EventTrigger();
                    
                    if ( t.Equals( tr.GetType() ) )
                    {
                        trigger = (Trigger)(obj);
                    }
                    else if (t.Equals( et.GetType() ) )
                    {
                        eventTrigger = (EventTrigger)(obj);
                    }
                    else
                    {
                        GlobalLog.LogEvidence("ERROR!! The Trigger must be an Property Trigger or an EventTrigger.");
                        return null;
                    }
                }
            }
            
            //If there is no style, or there is a style but it has no Triggers, look
            //for the EventTrigger in a ControlTemplate or on an Element.
            if (!triggerInStyle)
            {
                if (rootElement.Triggers.Count != 0)
                {
                    eventTrigger = (EventTrigger)rootElement.Triggers[0];
                }
                else if (animatedElement.Triggers.Count != 0)
                {
                    eventTrigger = (EventTrigger)animatedElement.Triggers[0];
                }
            }
            
            //Second, use the Event or Property Trigger to retrieve the BeginStoryboard.
            //NOTE: the Property Trigger has precedence for the purposes of verification: if
            //one is found, any EventTriggers present will be ignored.
            if (trigger != null)
            {
                if (trigger.EnterActions.Count == 0)
                {
                    GlobalLog.LogEvidence("ERROR!! No Actions were found on the Trigger.");
                    return null;
                }
                else
                {
                    beginStoryboard = (BeginStoryboard)trigger.EnterActions[0];
                }
            }
            else if (eventTrigger != null)
            {
                if (eventTrigger.Actions.Count == 0)
                {
                    GlobalLog.LogEvidence("ERROR!! No Actions were found on the EventTrigger.");
                    return null;
                }
                else
                {
                    beginStoryboard = (BeginStoryboard)eventTrigger.Actions[0];
                }
            }
            else
            {
                GlobalLog.LogEvidence("ERROR!! No Trigger was found.");
                return null;
            }
            
            GlobalLog.LogEvidence("A BeginStoryboard was found.");
            return beginStoryboard;
        }

        /******************************************************************************
        * Function:          RemoveNavigationBar
        ******************************************************************************/
        /// <summary>
        /// Removes the NavigationBar from a NavigationWindow.
        /// </summary>
        public static void RemoveNavigationBar(NavigationWindow navWin)
        {
            //create new style and apply, in order to remove the default Navigation bar.
            navWin.Resources = new System.Windows.ResourceDictionary();
            navWin.Resources.BeginInit();
            navWin.Resources.Add("Testing", ApplyNewNavWinStyle());
            navWin.Resources.EndInit();
            navWin.SetResourceReference(System.Windows.FrameworkElement.StyleProperty, "Testing");
        }

        /******************************************************************************
        * Function:          ApplyNewNavWinStyle
        ******************************************************************************/
        /// <summary>
        /// Creates a Style removing the Navigation bar for the NavigationWindow.
        /// </summary>
        /// <returns>Style</returns>
        private static Style ApplyNewNavWinStyle()
        {
            Style ds = new Style(typeof(NavigationWindow));
            //dockpanel
            FrameworkElementFactory dockPanel = new FrameworkElementFactory(typeof(DockPanel), "DockPanel");
            
            // content presenter
            FrameworkElementFactory cp = new FrameworkElementFactory(typeof(ContentPresenter), "ContentSite");
            cp.SetValue(DockPanel.LastChildFillProperty, true);
            cp.SetValue(DockPanel.BackgroundProperty, System.Windows.Media.Brushes.White);
            cp.SetValue(ContentControl.ContentProperty, new TemplateBindingExtension(ContentControl.ContentProperty));

            dockPanel.AppendChild(cp);

            ControlTemplate template = new ControlTemplate(typeof(NavigationWindow));
            template.VisualTree = dockPanel;
            ds.Setters.Add(new Setter(NavigationWindow.TemplateProperty, template));

            return ds;
        }

        /******************************************************************************
        * Function:          CheckInputString
        ******************************************************************************/
        /// <summary>
        /// CheckInputString: checks for a valid string, by determining whether or not it is
        /// in the list of strings passed in.
        /// </summary>
        /// <returns>A boolean, indicating whether or not the requested string was found</returns>
        public static bool CheckInputString(string actString, string[] expStringList, ref string returnMessage)
        {
            bool stringFound = false;

            for (int i = 0; i < expStringList.Length; i++)
            {
                if (actString == expStringList[i])
                {
                    stringFound = true;
                    break;
                }
            }

            if (!stringFound)
            {
                string animList = "";
                for (int j = 0; j < expStringList.Length; j++)
                {
                    animList += "\n\t" + expStringList[j];
                }
                returnMessage = "Incorrect string found.\nChoose one from the following list: " + animList;
            }
            
            return stringFound;
        }

        /******************************************************************************
           * Function:          CompareColors
           ******************************************************************************/
        /// <summary>
        /// CompareColors: compares expected vs. actual color, using a tolerance
        /// </summary>
        /// <returns>A boolean, indicating whether or not the colors match</returns>
        public static bool CompareColors(Color expColor, Color actColor, float tolerance)
        {
            bool colorMatched   = true;

            if (Math.Abs(expColor.ScR - actColor.ScR) >= tolerance)
            { 
                colorMatched = false; 
            }
            if (Math.Abs(expColor.ScG - actColor.ScG) >= tolerance) 
            { 
                colorMatched = false; 
            }
            if (Math.Abs(expColor.ScB - actColor.ScB) >= tolerance) 
            { 
                colorMatched = false; 
            }

            return colorMatched;
        }

        /// <summary>
        /// Finds the first FrameworkElement in the VisualTree that has the specified Name
        /// </summary>
        /// <param name="element">Visual Element to search from</param>
        /// <param name="id">Name of the element to look for</param>
        /// <returns>The found element</returns>
        public static FrameworkElement FindElement(FrameworkElement element, string id)
        {
            FrameworkElement[] elements = FindElements(element, id);

            if (elements.Length > 0)
                return elements[0];

            return null;
        }

        /// <summary>
        /// Finds all the FrameworkElements in the VisualTree that has the specified Name
        /// </summary>
        /// <param name="element">Visual Element to search from</param>
        /// <param name="id">Name of the elements to look for</param>
        /// <returns>array of the elements found</returns>
        public static FrameworkElement[] FindElements(FrameworkElement element, string id)
        {
            ArrayList list = new ArrayList();
            FindElements(element, id, list);
            return (FrameworkElement[])list.ToArray(typeof(FrameworkElement));
        }

        /// <summary>
        /// Finds all the FrameworkElements in the VisualTree that has the specified Name
        /// </summary>
        /// <param name="element">Visual Element to search from</param>
        /// <param name="id">Name of the elements to look for</param>
        /// <param name="list">List of elements matching the id</param>
        /// <returns></returns>
        static void FindElements(FrameworkElement element, string id, ArrayList list)
        {
            if (element.Name == id)
                list.Add(element);

            //make sure that the visual tree is avalible
            element.ApplyTemplate();

            //search all the children
            int count = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(element, i);
                if (child is FrameworkElement)
                    FindElements((FrameworkElement)child, id, list);
            }
            //Connect the Popup visual tree
            if (element is Popup)
                FindElements(((Popup)element).Child as FrameworkElement, id, list);
        }
    }
}

