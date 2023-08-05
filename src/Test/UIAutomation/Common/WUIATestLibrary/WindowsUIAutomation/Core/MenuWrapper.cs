// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
* Purpose: InternalHelper
* Owner: Microsoft
* Contributors:
*******************************************************************/
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Automation;
using System.Collections;
using System.Drawing;
using System.CodeDom;
using System.Threading;
using System.Diagnostics;

namespace Microsoft.Test.WindowsUIAutomation.Core
{
	using Microsoft.Test.WindowsUIAutomation.Logging;
	using InternalHelper;

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class TestMenu : IDisposable
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        AutomationElement _element;

        /// -------------------------------------------------------------------
        /// <summary>Used for debuging</summary>
        /// -------------------------------------------------------------------
        string _name;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        const int TIMEWAIT = 1;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        const int MAXWAIT = 5000;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private ManualResetEvent _ev = new ManualResetEvent(false);

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        ExpandCollapseState _desiredState;

        /// -------------------------------------------------------------------
        /// <summary>
        /// AutomationElement for the menu associated with this object
        /// </summary>
        /// -------------------------------------------------------------------
        public AutomationElement AutomationElement
        {
            get
            {
                return _element;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Constructor for the menu object
        /// </summary>
        /// -------------------------------------------------------------------
        public TestMenu(AutomationElement element)
        {
            if (element == null)
                throw new ArgumentException();

            _element = element;
            _name = _element.Current.Name;
        }

        #region IDispose

        /// -------------------------------------------------------------------
        /// <summary>
        /// </summary>
        /// -------------------------------------------------------------------
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// </summary>
        /// -------------------------------------------------------------------
        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                //_ev.Dispose(); PreSharp says we need this, but it's private
            }
        }

        #endregion IDispose

        /// -------------------------------------------------------------------
        /// <summary>
        /// Expand the menu object
        /// </summary>
        /// -------------------------------------------------------------------
        public TestMenu Expand()
        {
            expandCollapseMenu(ExpandCollapseState.Expanded);
            return this;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Collapse the menu object
        /// </summary>
        /// -------------------------------------------------------------------
        public TestMenu Collapse()
        {
            expandCollapseMenu(ExpandCollapseState.Collapsed);
            return this;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Return the sub menu for this menu that can be identified by 
        /// using FindFirst(TreeScope.Descendants, new PropertyCondition(property, indentifer));
        /// </summary>
        /// -------------------------------------------------------------------
        public TestMenu SubMenu(string indentifer, AutomationProperty property)
        {
            PropertyCondition pc = new PropertyCondition(property, indentifer);
            AutomationElement element = _element.FindFirst(TreeScope.Descendants, pc);
            Debug.Assert(element != null);
            return new TestMenu(element);
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Get the first child menu using ControlViewWalker
        /// </summary>
        /// -------------------------------------------------------------------
        public TestMenu GetFirstSubMenu()
        {
            AutomationElement element = TreeWalker.ControlViewWalker.GetFirstChild(_element);
            Debug.Assert(element != null);

            UIVerifyLogger.LogComment(Library.GetUISpyLook(_element) + "'s ControlViewWalker.GetFirstChild = " + Library.GetUISpyLook(element));
            return new TestMenu(element);
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Get the next sibling menu using ControlViewWalker
        /// </summary>
        /// -------------------------------------------------------------------
        public TestMenu GetNextSiblingMenu()
        {
            AutomationElement element = TreeWalker.ControlViewWalker.GetNextSibling(_element);
            Debug.Assert(element != null);
            UIVerifyLogger.LogComment(_element.Current.Name + "'s ControlViewWalker.GetNextSibling = " + element.Current.Name);
            return new TestMenu(element);
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Returns true if the menu's IsExpandCollapsePatternAvailableProperty is true, else false
        /// </summary>
        /// -------------------------------------------------------------------
        public bool Expandable
        {
            get
            {
                return (bool)this.AutomationElement.GetCurrentPropertyValue(AutomationElement.IsExpandCollapsePatternAvailableProperty);
            }
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Invoke the menu
        /// </summary>
        /// -------------------------------------------------------------------
        public bool Invoke()
        {
            throw new NotImplementedException();
        }

        /// ---------------------------------------------------------------
        /// <summary>
        /// Will call the approprate ExpandCollapse state to happen
        /// </summary>
        /// ---------------------------------------------------------------
        void expandCollapseMenu(ExpandCollapseState desiredState)
        {
            ExpandCollapsePattern ecp = this.AutomationElement.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;

            _desiredState = desiredState;

            if (ecp == null)
                throw new InvalidOperationException("The element does not support ExpandCallapsePattern");

            _ev.Reset();

            Automation.AddAutomationPropertyChangedEventHandler(
                _element, 
                TreeScope.Element,
                new AutomationPropertyChangedEventHandler(ExpandCollapseEventHandler),
                new AutomationProperty[] { ExpandCollapsePattern.ExpandCollapseStateProperty });

            // Cause the state to happen
            switch (desiredState)
            {
                case ExpandCollapseState.Collapsed:
                    ecp.Collapse();
                    break;

                case ExpandCollapseState.Expanded:
					Logger.LogComment("Expanding " + _element.Current.Name);
                    // There can be times when there is already another menu opened and 
                    // we throw an expection, which will dismiss it, do it again.
                    try
                    {
                        ecp.Expand();
                    }
                    catch (Exception exception)
                    {
						if (Library.IsCriticalException(exception))
                            throw;

                        ecp.Expand();
                    }
                    break;

                case ExpandCollapseState.LeafNode:
                    throw new NotImplementedException();

                case ExpandCollapseState.PartiallyExpanded:
                    throw new NotImplementedException();

                default:
                    throw new Exception("Have not implemented case for " + desiredState);

            }

            _ev.WaitOne(10000, false);

            while (ecp.Current.ExpandCollapseState != desiredState)
            {
                throw new Exception("ExpandCollapsePattern.ExpandCollapseStateProperty != " + desiredState.ToString());
            }
        }

        
        /// -------------------------------------------------------------------
        /// <summary>
        /// Event handler
        /// </summary>
        /// -------------------------------------------------------------------
        private void ExpandCollapseEventHandler(object src, AutomationPropertyChangedEventArgs arguments)
        {
            if (Automation.Compare((AutomationElement)src, _element))
            {
                if (arguments.Property == ExpandCollapsePattern.ExpandCollapseStateProperty)
                    if (_desiredState == (ExpandCollapseState)arguments.NewValue)
                        _ev.Set();

            }

        }
    }
}
