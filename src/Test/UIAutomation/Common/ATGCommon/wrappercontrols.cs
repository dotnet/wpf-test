// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Description: Wrapper objects for driving the UI easier
// Author : Microsoft

using System;
using System.Windows.Automation;
using System.Windows;
using System.Diagnostics;

namespace WUITest.ControlWrappers
{
    /// -----------------------------------------------------------------------
    /// <summary>
    /// Creates control wrappers based on the element's ControlType  
    /// </summary>
    /// -----------------------------------------------------------------------
    class ControlClassFactory
    {
        /// -------------------------------------------------------------------
        /// <summary>Returns the control based on the ControlType</summary>
        /// -------------------------------------------------------------------
        public static object Control(AutomationElement element)
        {
            ControlType ct = element.Current.ControlType;

            if (ct == ControlType.Tree)
                return new Tree(element);

            if (ct == ControlType.TreeItem)
                return new TreeItem(element);

            if (ct == ControlType.List)
                return new List(element);

            if (ct == ControlType.ListItem)
                return new ListItem(element);

            if (ct == ControlType.ScrollBar)
                return new ScrollBar(element);

            if (ct == ControlType.Button)
                return new Button(element);

            // add more here

            return ControlType.Custom;
        }
    }

    /// -----------------------------------------------------------------------
    /// <summary>
    /// Basically helpers methods will go here if there are any needed
    /// </summary>
    /// -----------------------------------------------------------------------
    class Base
    {
        ///  ------------------------------------------------------------------
        /// <summary></summary>
        ///  ------------------------------------------------------------------
        AutomationElement _element;

        ///  ------------------------------------------------------------------
        /// <summary></summary>
        ///  ------------------------------------------------------------------
        internal Base(AutomationElement element)
        {
            _element = element;
        }
        ///  ------------------------------------------------------------------
        /// <summary></summary>
        ///  ------------------------------------------------------------------
        public AutomationElement AutomationElement { get { return _element; } }

        ///  ------------------------------------------------------------------
        /// <summary>
        /// Returns a child below the element of the 1st AutomationId found that 
        /// matches
        /// </summary>
        ///  ------------------------------------------------------------------
        static internal AutomationElement Child(AutomationElement element, string automtionId)
        {
            return element.FindFirst(TreeScope.Children | TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, automtionId));
        }

        ///  ------------------------------------------------------------------
        /// <summary></summary>
        ///  ------------------------------------------------------------------
        static internal void SetScrollPercent(AutomationElement element, double horizontalPercent, double verticalPercent)
        {
            ScrollPattern sp = element.GetCurrentPattern(ScrollPattern.Pattern) as ScrollPattern;
            Debug.Assert(sp != null);
            sp.SetScrollPercent(horizontalPercent, verticalPercent);
        }
    }

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    class Tree : Base
    {
        ///  ------------------------------------------------------------------
        /// <summary></summary>
        ///  ------------------------------------------------------------------
        public Tree(AutomationElement element): base(element) { }

        ///  ------------------------------------------------------------------
        /// <summary></summary>
        ///  ------------------------------------------------------------------
        public ScrollBar HorizontalScrollBar { get { return new ScrollBar(Base.Child(this.AutomationElement, "Horizontal ScrollBar")); } }

        ///  ------------------------------------------------------------------
        /// <summary></summary>
        ///  ------------------------------------------------------------------
        public ScrollBar VerticalScrollBar { get { return new ScrollBar(Base.Child(this.AutomationElement, "Vertical ScrollBar")); } }

        ///  ------------------------------------------------------------------
        /// <summary></summary>
        ///  ------------------------------------------------------------------
        public TreeItems TreeItems { get { return new TreeItems(this.AutomationElement); } }

        ///  ------------------------------------------------------------------
        /// <summary></summary>
        ///  ------------------------------------------------------------------
        public void SetScrollPercent(double horizontalPercent, double verticalPercent)
        {
            Base.SetScrollPercent(this.AutomationElement, horizontalPercent, verticalPercent);
        }
    }

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    class List : Base
    {
        ///  ------------------------------------------------------------------
        /// <summary></summary>
        ///  ------------------------------------------------------------------
        public List(AutomationElement element): base(element) { }

        ///  ------------------------------------------------------------------
        /// <summary></summary>
        ///  ------------------------------------------------------------------
        public ScrollBar HorizontalScrollBar { get { return new ScrollBar(Base.Child(this.AutomationElement, "Horizontal ScrollBar")); } }

        ///  ------------------------------------------------------------------
        /// <summary></summary>
        ///  ------------------------------------------------------------------
        public ScrollBar VerticalScrollBar { get { return new ScrollBar(Base.Child(this.AutomationElement, "Vertical ScrollBar")); } }

        ///  ------------------------------------------------------------------
        /// <summary></summary>
        ///  ------------------------------------------------------------------
        public ListItems ListItems { get { return new ListItems(this.AutomationElement); } }

        ///  ------------------------------------------------------------------
        /// <summary></summary>
        ///  ------------------------------------------------------------------
        public void SetScrollPercent(double horizontalPercent, double verticalPercent)
        {
            Base.SetScrollPercent(this.AutomationElement, horizontalPercent, verticalPercent);
        }

    }
    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    class ScrollBar : Base
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public ScrollBar(AutomationElement element) : base(element) { }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public Button SmallDecrement { get { return new Button(Base.Child(this.AutomationElement, "SmallDecrement")); } }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public Button SmallIncrement { get { return new Button(Base.Child(this.AutomationElement, "SmallIncrement")); } }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public Button LargeDecrement { get { return new Button(Base.Child(this.AutomationElement, "LargeDecrement")); } }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public Button LargeIncrement { get { return new Button(Base.Child(this.AutomationElement, "LargeIncrement")); } }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public Button Thumb { get { return new Button(Base.Child(this.AutomationElement, "Thumb")); } }
    }


    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    class TreeItems : Base
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public TreeItems(AutomationElement element) : base(element) { }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public TreeItem this[int index]
        {
            get
            {
                AutomationElementCollection collection = this.AutomationElement.FindAll(TreeScope.Children | TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.IsSelectionItemPatternAvailableProperty, true));
                return new TreeItem(collection[index]);
            }
        }
    }

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    class ListItems : Base
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public ListItems(AutomationElement element) : base(element) { }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public ListItem this[int index]
        {
            get
            {
                AutomationElementCollection collection = this.AutomationElement.FindAll(TreeScope.Children | TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.IsSelectionItemPatternAvailableProperty, true));
                return new ListItem(collection[index]);
            }
        }
    }

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    class Button : Base
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public Button(AutomationElement element) : base(element) { }
    }

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    class TreeItem : Base
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public TreeItem(AutomationElement element) : base(element) { }
    }

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    class ListItem : Base
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public ListItem(AutomationElement element) : base(element) { }
    }
}  
