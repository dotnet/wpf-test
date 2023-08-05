// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Base class for an application.
//
//
using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using InternalHelper;
using Microsoft.Test.WindowsUIAutomation;
using Microsoft.Test.WindowsUIAutomation.Core;
using Microsoft.Test.WindowsUIAutomation.TestManager;
using Microsoft.Test.WindowsUIAutomation.Interfaces;
using WUITest.ControlWrappers;
using MS.Win32;

#pragma warning disable 1634, 1691

// PRESHARP: In order to avoid generating warnings about unkown message numbers and unknown pragmas.
#pragma warning disable 1634, 1691

namespace WUITest
{
    #region Misc
    public enum ControlId
    {
        Animation = 1065,
        Button = 1002,
        Calendar = 1026,
        Check = 1003,
        ComboBox = 1005,
        ComboBoxEdit = 1015,
        DateTimePicker1091 = 1091,
        DateTimePicker1092 = 1092,
        EditBox = 1000,
        GroupBox = 1008,
        HotKey = 1049,
        IPAddress = 1025,
        Label = 1007,
        ListBoxMultiSelect = 1011,
        ListBoxNoScroll = 1012,
        ListBoxSingleSelect = 1006,
        ListViewIcon = 1020,
        ListViewList = 1023,
        ListViewReport = 1013,
        ListViewSmallIcon = 1021,
        MenuBar = 90011,
        MenuItem1 = 90012,
        MenuItem2 = 90013,
        MenuItem3 = 90014,
        MenuItem4 = 90015,
        MenuItem5 = 90016,
        MenuItem6 = 90017,
        PopupMenu = 1919,
        Progress = 1062,
        Radio = 1004,
        RichEdit = 1035,
        RichEdit20A = 1033,
        RichEdit20W = 1032,
        ScrollBarHorizontal = 1036,
        ScrollBarVertical = 1034,
        RichEditVert = 1037,
        RichEditHoriz = 1039,
        RichEditBoth = 1041,
        EditVert = 1095,
        EditHoriz = 1096,
        EditBoth = 1097,
        RichEditVer20 = 1098,
        Slider = 1058,
        Spin1 = 1009,
        Spin2 = 1090,
        StatusBar = 59393,
        StatusBarGrip = 9004,
        StatusBarPan = 9003,
        SystemMenu = 9006,
        SystemMenuBar = 9005,
        SystemMenuItem = 9007,
        Tab = 1042,
        TitleBar = 90018,
        TitleBarClose = 90010,
        TitleBarMaximize = 9009,
        TitleBarMinimize = 9008,
        ToolBar = 59419,
        ToolBarContainer = 59392,
        ToolBarItem = 9002,
        ToolBarSeparator = 9001,
        Tree = 1016,

    }

    internal class NativeMethods
    {
        const int GA_PARENT = 1, GW_HWNDFIRST = 0, GW_HWNDLAST = 1, GW_HWNDNEXT = 2, GW_HWNDPREV = 3, GW_OWNER = 4, GW_CHILD = 5, GWL_ID = -12;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindWindow([In] string className, [In] string windowName);

        [StructLayout(LayoutKind.Sequential)]
        struct HWND
        {
            IntPtr _h;

            static HWND Cast(IntPtr h)
            {
                HWND hTemp = new HWND();

                hTemp._h = h;
                return hTemp;
            }

            public static implicit operator IntPtr(HWND h)
            {
                return h._h;
            }

            public override bool Equals(object oCompare)
            {
                HWND hr = Cast((HWND)oCompare);

                return _h == hr._h;
            }

            public override int GetHashCode()
            {
                return (int)_h;
            }
        }
    }

    #endregion Misc


    /// -----------------------------------------------------------------------
    /// Wrapper classes for the different applications.
    /// -----------------------------------------------------------------------
    namespace Applications
    {

        abstract class Base :
            IControlLookUp,
            IApplicationCommands,
            IWUIMenuCommands,
            IWUIStructureChange,
            IDisposable,
            IWUIPropertyChange
        {
            #region Variables

            /// <summary>
            /// Wait 2 seconds for events to happen
            /// </summary>
            internal const int MAXTIME = 60000;

            /// <summary>
            /// Default waiting is 1/10th second between calls
            /// </summary>
            internal const int TIMEWAIT = 100;

            /// <summary>
            /// Main Automation Element
            /// </summary>
            internal AutomationElement _element;

            /// <summary>
            /// Used in the eventing to allow the process to release and let the 
            /// event occur.
            /// </summary>
            static ManualResetEvent s_notifiedEvent = new ManualResetEvent(false);

            /// <summary>
            /// Applications menu bar
            /// </summary>
            internal TestMenu _menuBar;

            /// <summary>
            /// Applications system menu
            /// </summary>
            internal TestMenu _sysMenu;

            /// <summary>
            /// 
            /// </summary>
            internal TestMenu _logicalStructureChangeMenu;

            /// <summary>
            /// 
            /// </summary>
            private ManualResetEvent _ev = new ManualResetEvent(false);

            #endregion Variables

            #region IDisposable

            /// ---------------------------------------------------------------
            /// <summary></summary>
            /// ---------------------------------------------------------------
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// ---------------------------------------------------------------
            /// <summary></summary>
            /// ---------------------------------------------------------------
            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (_menuBar != null)
                        _menuBar.Dispose();

                    if (_sysMenu != null)
                        _sysMenu.Dispose();

                    if (_logicalStructureChangeMenu != null)
                        _logicalStructureChangeMenu.Dispose();
                }
            }

            #endregion IDisposable

            #region Methods

            /// ---------------------------------------------------------------
            /// <summary></summary>
            /// ---------------------------------------------------------------
            internal Base(AutomationElement element)
            {
                _element = element;
            }

            /// ---------------------------------------------------------------
            /// <summary>
            /// Execute some menu based on the path
            /// </summary>
            /// ---------------------------------------------------------------
            internal void ExecuteMenu(AutomationElement menuBar, string[] menuPath)
            {
                AutomationElement curMenuItem = menuBar;
                AutomationElement nextMenuItem;
                AutomationElementCollection menus = null;
                int iCurrentMenu = 0;

                AutomationEventHandler onMenuOpenedEvent = new AutomationEventHandler(OnMenuOpenedEvent);

                // start listening
                Automation.AddAutomationEventHandler(
                            AutomationElement.MenuOpenedEvent,
                            menuBar,
                            TreeScope.Children | TreeScope.Descendants,
                            onMenuOpenedEvent);


                for (int i = 0; i < menuPath.Length; i++)
                {
                    Trace.WriteLine("Menu: " + menuPath[i]);
                    iCurrentMenu = 0;
                    menus = curMenuItem.FindAll(TreeScope.Children | TreeScope.Descendants,
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.MenuItem));
                    for (; iCurrentMenu < menus.Count; iCurrentMenu++)
                    {
                        if (menus[iCurrentMenu].Current.Name == menuPath[i])
                            break;
                    }
                    Debug.Assert(iCurrentMenu != menus.Count);

                    nextMenuItem = menus[iCurrentMenu];

                    if (true == (bool)nextMenuItem.GetCurrentPropertyValue(AutomationElement.IsExpandCollapsePatternAvailableProperty))
                    {
                        Trace.WriteLine("Expanding : " + menus[iCurrentMenu].Current.Name);

                        // Even though the we are watching for events to get fired, there seems to be times when the menus are still not ready, 
                        // and calling Exapnd on it throws an InvalidOperation, such as when there is another menu that was opened
                        // and now we are trying to invoke this one.  Let the system settle down and close any other menus
                        // that might need to do it''s thing. (ie. Reset menu, and then Add Child menu)
                        Thread.Sleep(10);  
                        
                        ExpandCollapsePattern ecp = nextMenuItem.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                        Debug.Assert(ecp != null);

                        _ev.Reset();
                        
                        ecp.Expand();
                        
                        // Wait for the menu to open
                        _ev.WaitOne(5000, false);

                        curMenuItem = nextMenuItem;
                        Trace.WriteLine("Expanded : " + menus[iCurrentMenu].Current.Name);
                    }
                    else
                        Trace.WriteLine(menus[iCurrentMenu].Current.Name + " does not support expanding");
                }

                // Invoke the menu item
                curMenuItem = curMenuItem.FindFirst(TreeScope.Children | TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.NameProperty, menuPath[menuPath.Length - 1]));

                Debug.Assert(curMenuItem != null);

                InvokePattern ip = curMenuItem.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                Debug.Assert(ip != null);

                Trace.WriteLine("Invoking : " + curMenuItem.Current.Name);
                ip.Invoke();
            }

            /// ---------------------------------------------------------------
            /// <summary></summary>
            /// ---------------------------------------------------------------
            private void OnMenuOpenedEvent(object src, AutomationEventArgs e)
            {
                _ev.Set();
            }

            /// ---------------------------------------------------------------
            /// <summary>
            /// Common event handler 
            /// </summary>
            /// ---------------------------------------------------------------
            public void OnEvent(object src, AutomationEventArgs arguments)
            {
                // _log.LogComment("OnEvent fired");
                //m_EventFired = true;
                s_notifiedEvent.Set();
            }

            /// ---------------------------------------------------------------
            /// <summary>
            /// Set focus to the element.  Method will pull to see if the element
            /// actually does get the focus by calling HasKeyboardFocus
            /// </summary>
            /// ---------------------------------------------------------------
            internal static void SetFocus(AutomationElement element)
            {
                int runningtime = 0;

                element.SetFocus();

                for (; ; )
                {
                    if (element.Current.HasKeyboardFocus == true)
                        break;

                    if (runningtime > Base.MAXTIME)
                        throw new Exception("Could not setfocus to the '" + element.Current.Name + "' control");

                    Thread.Sleep(Base.TIMEWAIT);
                    runningtime += Base.TIMEWAIT;
                }
            }

            internal object Control(object automationId)
            {
                return WUITest.ControlWrappers.ControlClassFactory.Control(
                    _element.FindFirst(TreeScope.Children | TreeScope.Descendants,
                        new PropertyCondition(AutomationElement.AutomationIdProperty, automationId.ToString())));
            }

            #endregion Methods

            #region IApplicationCommands

            /// -------------------------------------------------------------------
            /// <summary>
            /// Method that the test application can call to relay any information
            /// such as status information
            /// </summary>
            /// -------------------------------------------------------------------
            public void TraceMethod(object information) { Console.WriteLine(information.ToString()); }

            /// -------------------------------------------------------------------
            /// <summary>
            /// Does this support IWUIPropertyChange
            /// </summary>
            /// -------------------------------------------------------------------
            public virtual bool SupportsIWUIPropertyChange(AutomationElement element)
            {
                return false;
            }

            /// -------------------------------------------------------------------
            /// <summary>
            /// Does this support IWUIMenu
            /// </summary>
            /// -------------------------------------------------------------------
            public virtual bool SupportsIWUIMenuCommands()
            {
                return false;
            }

            /// -------------------------------------------------------------------
            /// <summary>
            /// Does this support IWUIStructureChange
            /// </summary>
            /// -------------------------------------------------------------------
            public virtual bool SupportsIWUIStructureChange(AutomationElement element)
            {
                return false;
            }

            /// -------------------------------------------------------------------
            /// <summary>
            /// Returns IWUIMenuCommands
            /// </summary>
            /// -------------------------------------------------------------------
            public virtual IWUIMenuCommands GetIWUIMenuCommands()
            {
                throw new NotImplementedException();
            }

            /// -------------------------------------------------------------------
            /// <summary>
            /// Returns IWUIStructureChange
            /// </summary>
            /// -------------------------------------------------------------------
            public virtual IWUIStructureChange GetIWUIStructureChange()
            {
                throw new NotImplementedException();
            }

            /// -------------------------------------------------------------------
            /// <summary>
            /// Return IWUIPropertyChange
            /// </summary>
            /// -------------------------------------------------------------------
            public virtual IWUIPropertyChange GetIWUIPropertyChange()
            {
                throw new NotImplementedException();
            }

            #endregion IApplicationCommands

            #region IWUIPropertyChange

            /// ---------------------------------------------------------------
            /// <summary>
            /// Cause a property to change on the element
            /// </summary>
            /// ---------------------------------------------------------------
            public virtual void ChangeProperty(AutomationProperty property, object automationID)
            {
                throw new NotImplementedException();
            }

            #endregion IWUIPropertyChange

            #region IWUIMenuCommands
            /// ---------------------------------------------------------------
            /// <summary>
            /// Returns the applications main menu bar
            /// </summary>
            /// ---------------------------------------------------------------
            public virtual TestMenu GetMenuBar()
            {
                if (_menuBar != null)
                    return _menuBar;

                PropertyCondition condition = new PropertyCondition(AutomationElement.AutomationIdProperty, "MenuBar");
                AutomationElement menuBar = _element.FindFirst(TreeScope.Children, condition);

                if (menuBar == null)
                    throw new Exception("Could not find the menu bar");

                _menuBar = new TestMenu(menuBar);

                return _menuBar;
            }

            /// ---------------------------------------------------------------
            /// <summary>
            /// Returns the applications system menu bar
            /// </summary>
            /// ---------------------------------------------------------------
            public virtual TestMenu GetSystemMenu()
            {
                if (_sysMenu != null)
                    return _sysMenu;

                PropertyCondition condition = new PropertyCondition(AutomationElement.AutomationIdProperty, "SystemMenuBar");
                AutomationElement sysMenu = _element.FindFirst(TreeScope.Descendants, condition);

                if (sysMenu == null)
                    throw new Exception("Could not find the system menu");

                _sysMenu = new TestMenu(sysMenu);

                return _sysMenu;
            }

            #endregion IWUIMenu

            #region IWUIStructureChange

            /// ---------------------------------------------------------------
            /// <summary>
            /// The IStructureChange is passed to the test cases and used as a 
            /// call to let the class do whatever it needs to do to cause the
            /// event to happen.
            /// </summary>
            /// ---------------------------------------------------------------
            public virtual bool CauseStructureChange(AutomationElement element, StructureChangeType changeType)
            {
                throw new NotImplementedException();
            }

            /// ---------------------------------------------------------------
            /// <summary>
            /// Set the control back to it's original state
            /// </summary>
            /// ---------------------------------------------------------------
            public virtual bool ResetControl(AutomationElement element)
            {
                throw new NotImplementedException();
            }

            /// -------------------------------------------------------------------
            /// <summary>
            /// Method that will determine if the element supports tests for 
            /// structure change
            /// </summary>
            /// -------------------------------------------------------------------
            public virtual bool DoesControlSupportStructureChange(AutomationElement element, StructureChangeType structureChangeType)
            {
                return false;
            }

            #endregion IStructureChange

            #region IControlLookUp

            /// ---------------------------------------------------------------
            /// <summary>
            /// The class will use this value to identify the specific element
            /// on the client, and then return the AutomationElement based on this
            /// ID
            /// </summary>
            /// ---------------------------------------------------------------
            public virtual AutomationElement AutomationElementFromCustomId(object identifier)
            {
                AutomationElement ae = _element.FindFirst(TreeScope.Children | TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, identifier));

                /////// Logging.LogComment("Looking for control (" + identifier + ")");

                if (ae == null)
                    throw new Exception("Could not identify the element based on the AutomationIdProperty");

                return ae;
            }

            #endregion IControlLookUp

        }

        sealed class WordPad : Base
        {
            #region Methods

            /// ---------------------------------------------------------------
            /// <summary>
            /// Constructor
            /// </summary>
            /// ---------------------------------------------------------------
            internal WordPad(AutomationElement element)
                : base(element)
            {
            }

            #endregion Methods

            #region IDisposable

            /// <summary></summary>
            public new void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary></summary>
            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    base.Dispose();
                }
            }

            #endregion Disposable

            #region IControlLookUp

            /// ---------------------------------------------------------------
            /// <summary>
            /// The method will use this value to identify the specific element
            /// on the client, and then return the AutomationElement based on this
            /// ID
            /// </summary>
            /// ---------------------------------------------------------------
            public override AutomationElement AutomationElementFromCustomId(object ID)
            {
                /////// Logging.LogComment("Looking for control (" + ID + ")");

                AutomationElement ae = _element.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, ID));

                if (ae == null)
                    throw new Exception("Could not identify the element based on the AutomationIdProperty");

                return ae;
            }

            #endregion IControlLookUp

        }

        sealed class NotePad : Base
        {
            #region Methods

            /// ---------------------------------------------------------------
            /// <summary>
            /// Constructor
            /// </summary>
            /// ---------------------------------------------------------------
            internal NotePad(AutomationElement element)
                : base(element)
            {
            }

            #endregion Methods

            #region IDisposable

            /// <summary></summary>
            public new void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary></summary>
            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    base.Dispose();
                }
            }

            #endregion Disposable

            #region IApplicationCommands

            /// -------------------------------------------------------------------
            /// <summary>
            /// Does this support IWUIMenu
            /// </summary>
            /// -------------------------------------------------------------------
            public override bool SupportsIWUIMenuCommands()
            {
                return true;
            }

            /// -------------------------------------------------------------------
            /// <summary>
            /// Returns IWUIMenuCommands
            /// </summary>
            /// -------------------------------------------------------------------
            public override IWUIMenuCommands GetIWUIMenuCommands()
            {
                return (IWUIMenuCommands)this;
            }

            #endregion IApplicationCommands

        }
        sealed class ScreenReader : Base
        {

            #region Methods

            /// ---------------------------------------------------------------
            /// <summary>
            /// Constructor
            /// </summary>
            /// ---------------------------------------------------------------
            internal ScreenReader(AutomationElement element)
                : base(element)
            {
            }

            #endregion

            #region IDisposable

            /// <summary></summary>
            public new void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary></summary>
            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    base.Dispose();
                }
            }

            #endregion Disposable


        }

        sealed class ConsoleWindow : Base
        {

            #region Methods

            /// ---------------------------------------------------------------
            /// <summary>
            /// Constructor
            /// </summary>
            /// ---------------------------------------------------------------
            internal ConsoleWindow(AutomationElement element)
                : base(element)
            {
            }

            #endregion

            #region IDisposable

            /// <summary></summary>
            public new void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary></summary>
            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    base.Dispose();
                }
            }

            #endregion Disposable


        }

        sealed class GenericWindow : Base
        {

            #region Methods

            /// ---------------------------------------------------------------
            /// <summary>
            /// Constructor
            /// </summary>
            /// ---------------------------------------------------------------
            internal GenericWindow(AutomationElement element)
                : base(element)
            {
            }

            #endregion Methods

            #region IDisposable

            /// <summary></summary>
            public new void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary></summary>
            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    base.Dispose();
                }
            }

            #endregion Disposable

        }

        sealed class CtrlTest : Base
        {
            const string IDS_PROPERTYCHANGE_MENU = "Property Change";
            const string IDS_NAMEPROPERTY_MENU = "NameProperty";
            const string IDS_ENABLEDPROPERTY_MENU = "EnableProperty";
            const string IDS_LOGICALSTRUCTURECHANGE_MENU = "LogicalStructureTests";

            #region Methods

            /// ---------------------------------------------------------------
            /// <summary>
            /// Constructor
            /// </summary>
            /// ---------------------------------------------------------------
            internal CtrlTest(AutomationElement element)
                : base(element)
            {
            }

            /// ---------------------------------------------------------------
            /// <summary>
            /// Returns the test menu that does the LogicalStructureTests
            /// </summary>
            /// ---------------------------------------------------------------
            AutomationElement TestMenu
            {
                get
                {
                    if (_logicalStructureChangeMenu == null)
                        _logicalStructureChangeMenu = GetMenuBar().SubMenu("LogicalStructureTests", AutomationElement.NameProperty);
                    return _logicalStructureChangeMenu.AutomationElement;
                }
            }

            #endregion Methods

            #region Properties

            #endregion Proeprties

            #region IDisposable

            /// <summary></summary>
            public new void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary></summary>
            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    base.Dispose();
                }
            }

            #endregion Disposable

            #region IApplicationCommands

            /// -------------------------------------------------------------------
            /// <summary>
            /// Does this support IWUIPropertyChange
            /// </summary>
            /// -------------------------------------------------------------------
            public override bool SupportsIWUIPropertyChange(AutomationElement element)
            {
                // Application only knows how to change the name of these items through the menu
                // 
                string[] supported = new string[]
                {
                    "1000",
                    "1002",
                    "1003",
                    "1004",
                    "1005",
                    "1006",
                    "1007",
                    "1008",
                    "1010",
                    "1011",
                    "1012",
                    "1013",
                    "1014",
                    "1015",
                    "1016",
                    "1017",
                    "1020",
                    "1021",
                    "1023",
                    "1025",
                    "1042",
                    "1058",
                    "1062",
                    "1086",
                    "1095",
                    "1096",
                    "1097",
                    "2006",
                    "1035",
                    "1037",
                    "1039",
                    "1041"
                 };
                return (-1 != Array.IndexOf(supported,  element.Current.AutomationId));
            }

            /// -------------------------------------------------------------------
            /// <summary>
            /// Does this support IWUIMenu
            /// </summary>
            /// -------------------------------------------------------------------            
            public override bool SupportsIWUIMenuCommands()
            {
                return true;
            }

            /// -------------------------------------------------------------------
            /// <summary>
            /// Does this support IWUIStructureChange
            /// </summary>
            /// -------------------------------------------------------------------
            public override bool SupportsIWUIStructureChange(AutomationElement element)
            {
                string id = element.Current.AutomationId;
                // Combos don't support the add/delete structure change, the underlying listbox does
                return (Array.IndexOf(new string[] { /* "1005", "1015", */ "1006", "1011", "1012", "1042", "1016", "1017", "1023", "1021", "1020", "1013" }, id) != -1);
            }

            /// -------------------------------------------------------------------
            /// <summary>
            /// Returns IWUIMenuCommands
            /// </summary>
            /// -------------------------------------------------------------------
            public override IWUIMenuCommands GetIWUIMenuCommands()
            {
                return (IWUIMenuCommands)this;
            }

            /// -------------------------------------------------------------------
            /// <summary>
            /// Returns IWUIStructureChange
            /// </summary>
            /// -------------------------------------------------------------------
            public override IWUIStructureChange GetIWUIStructureChange()
            {
                return (IWUIStructureChange)this;
            }

            /// -------------------------------------------------------------------
            /// <summary>
            /// Return IWUIPropertyChange
            /// </summary>
            /// -------------------------------------------------------------------
            public override IWUIPropertyChange GetIWUIPropertyChange()
            {
                return (IWUIPropertyChange)this;
            }
            #endregion IApplicationCommands

            #region IWUIStructureChange

            /// ---------------------------------------------------------------
            /// <summary>
            /// The IStructureChange is passed to the test cases and used as a 
            /// call to let the class do whatever it needs to do to cause the
            /// event to happen.
            /// </summary>
            /// ---------------------------------------------------------------
            public override bool CauseStructureChange(AutomationElement element, StructureChangeType changeType)
            {
                string menuName = "";

                switch (changeType)
                {
                    case StructureChangeType.ChildAdded:
                        menuName = "Child Add";
                        break;

                    case StructureChangeType.ChildRemoved:
                        menuName = "Child Remove";
                        break;

                    case StructureChangeType.ChildrenBulkAdded:
                        menuName = "Bulk Add";
                        break;

                    case StructureChangeType.ChildrenBulkRemoved:
                        menuName = "Bulk Remove";
                        break;

                    case StructureChangeType.ChildrenInvalidated:
                        menuName = "Invalidate";
                        break;

                    case StructureChangeType.ChildrenReordered:
                        menuName = "Reorder";
                        break;

                    default:
                        throw new NotImplementedException();
                }

                return FireStructureChangeMenu(element, menuName);

            }

            /// ---------------------------------------------------------------
            /// <summary>
            /// Set the control back to it's original state
            /// </summary>
            /// ---------------------------------------------------------------
            public override bool ResetControl(AutomationElement element)
            {

                return FirePropertyChangeMenu(element, "Reset");
            }

            /// -------------------------------------------------------------------
            /// <summary>
            /// Method that will determine if the element supports tests for 
            /// structure change
            /// </summary>
            /// -------------------------------------------------------------------
            public override bool DoesControlSupportStructureChange(AutomationElement element, StructureChangeType structureChangeType)
            {
                ControlType ct = element.Current.ControlType;

                if (ct == ControlType.Tree || 
                    ct == ControlType.List ||
                    ct == ControlType.ComboBox ||
                    ct == ControlType.Tab)
                {
                    switch (structureChangeType)
                    {
                        case StructureChangeType.ChildAdded:
                            return true;
                        case StructureChangeType.ChildRemoved:
                            return true;
                        case StructureChangeType.ChildrenBulkAdded:
                            return false;
                        case StructureChangeType.ChildrenBulkRemoved:
                            return false;
                        case StructureChangeType.ChildrenInvalidated:
                            return false;
                        case StructureChangeType.ChildrenReordered:
                            return false;
                        default:
                            return false;
                    }
                }

                return false;
            }


            /// ---------------------------------------------------------------
            /// <summary>
            /// Fires the custom menu for the LogicalStructure change menu items
            /// </summary>
            /// ---------------------------------------------------------------
            bool FireStructureChangeMenu(AutomationElement element, string menuName)
            {
                // Get the menu item and invoke it...
                SetFocus(element);

                IWUIMenuCommands menu = this.GetIWUIMenuCommands();
                ExecuteMenu(menu.GetMenuBar().AutomationElement, new string[] { IDS_LOGICALSTRUCTURECHANGE_MENU, menuName});

                return true;

            }
            /// ---------------------------------------------------------------
            /// <summary>
            /// Fires the custom menu for the LogicalStructure change menu items
            /// </summary>
            /// ---------------------------------------------------------------
            bool FirePropertyChangeMenu(AutomationElement element, string menuName)
            {

                IWUIMenuCommands menu = this.GetIWUIMenuCommands();
                ExecuteMenu(menu.GetMenuBar().AutomationElement, new string[] { IDS_PROPERTYCHANGE_MENU, menuName, element.Current.AutomationId });

                return true;

            }

            #endregion IWUIStructureChange

            #region IControlLookUp

            /// ---------------------------------------------------------------
            /// <summary>
            /// The class will use this value to identify the specific element
            /// on the client, and then return the AutomationElement based on this
            /// ID
            /// </summary>
            /// <param name="ID">Object that identifies the element</param>
            /// <returns>The AutomationElement that the class associates
            /// with the ID paramater</returns>
            /// ---------------------------------------------------------------
            public override AutomationElement AutomationElementFromCustomId(object identifier)
            {
                string pid = string.Empty;

                if (identifier == null)
                    throw new ArgumentException();

                PropertyCondition pc;

                pid = identifier.ToString();

                /////// Logging.LogComment("Looking for control (" + pid + ")");

                switch (pid)
                {
                    // Special case the popup since it really isn't there, we need to 
                    // make it get created.  
                    // Use the button control as an offset to right click with the mouse
                    // on the clinet area to make the menu popup.
                    case "PopupMenu": //ControlId.PopupMenu.ToString():

                        AutomationElement button = AutomationElementFromCustomId("1002");

                        Rect rc = button.Current.BoundingRectangle;

                        Point pt = new Point(rc.Left - 1, rc.Top - 1);

                        ATGTestInput.Input.MoveTo(pt);
                        ATGTestInput.Input.SendMouseInput(0, 0, 0, ATGTestInput.SendMouseInputFlags.RightDown);
                        ATGTestInput.Input.SendMouseInput(0, 0, 0, ATGTestInput.SendMouseInputFlags.RightUp);

                        // 
                        Thread.Sleep(500);

                        return AutomationElement.RootElement.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Menu));


                    default:
                        {
                            pc = new PropertyCondition(AutomationElement.AutomationIdProperty, pid);
                            break;
                        }
                }
                return _element.FindFirst(TreeScope.Children | TreeScope.Descendants, pc); ;
            }

            #endregion IControlLookUp

            #region IWUIPropertyChange


            /// ---------------------------------------------------------------
            /// <summary>
            /// Cause a property to change on the element
            /// </summary>
            /// ---------------------------------------------------------------
            public override void ChangeProperty(AutomationProperty property, object automationID)
            {
                if (property == AutomationElement.NameProperty)
                {
                    IWUIMenuCommands menu = this.GetIWUIMenuCommands();
                    ExecuteMenu(menu.GetMenuBar().AutomationElement, new string[] { IDS_PROPERTYCHANGE_MENU, IDS_NAMEPROPERTY_MENU, automationID.ToString() });
                }
                else if (property == AutomationElement.IsEnabledProperty)
                {
                    IWUIMenuCommands menu = this.GetIWUIMenuCommands();
                    ExecuteMenu(menu.GetMenuBar().AutomationElement, new string[] { IDS_PROPERTYCHANGE_MENU, IDS_ENABLEDPROPERTY_MENU, automationID.ToString() });
                }

            }

            #endregion IWUIPropertyChange
        }

        sealed class CtrlTestWinForms : Base
        {
            #region Variables

            #region Menu indexes

            const int MenuChildAdded = 0;
            const int MenuStructureChangeTypeChildRemoved = 1;
            const int MenuStructureChangeTypeChildrenBulkAdded = 3;
            const int MenuStructureChangeTypeChildrenBulkRemoved = 4;
            const int MenuStructureChangeTypeChildrenArranged = 4;
            const int MenuStructureChangeTypeChildrenReordered = 6;
            const int MenuStructureChangeTypeChildrenInvalidated = 8;
            const int MenuReset = 10;

            #endregion Menu indexes

            #endregion Variables

            #region Methods

            /// ---------------------------------------------------------------
            /// <summary>
            /// Constructor
            /// </summary>
            /// ---------------------------------------------------------------
            internal CtrlTestWinForms(AutomationElement element)
                : base(element)
            {
            }

            #endregion Methods

            #region IDisposable

            /// <summary></summary>
            public new void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary></summary>
            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    base.Dispose();
                }
            }

            #endregion Disposable

            #region IControlLookUp

            /// ---------------------------------------------------------------
            /// <summary>
            /// The class will use this value to identify the specific element
            /// on the client, and then return the AutomationElement based on this
            /// ID
            /// </summary>
            /// ---------------------------------------------------------------
            public override AutomationElement AutomationElementFromCustomId(object identifier)
            {

                AutomationElement ae;

                Microsoft.Test.WindowsUIAutomation.TestManager.TestCaseAttribute tca = new Microsoft.Test.WindowsUIAutomation.TestManager.TestCaseAttribute("Idenitfy AutomationElement for tests", TestPriorities.Pri0, Microsoft.Test.WindowsUIAutomation.TestManager.TestStatus.Works, "Microsoft", new string[] { "Searching for element with AutomationIdProperty = " + identifier.ToString() });
                UIVerifyLogger.StartTest(tca, true, string.Empty, string.Empty, string.Empty);


                UIVerifyLogger.LogComment("Looking for control with AutomationId = " + identifier.ToString());
                ae = _element.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.AutomationIdProperty, identifier));
                if (ae == null)
                {
                    Exception exception = new ArgumentException("Test cannot be ran since AutomationElement with AutomationIdProperty = " + identifier.ToString() + " could not be found!!");
                    UIVerifyLogger.LogError(exception);
                    UIVerifyLogger.EndTest();
                    throw exception;
                }
                else
                {
                    UIVerifyLogger.LogComment("Found : " + identifier.ToString() + " with AutomationElementProperty = " + ae.Current.Name);
                    // If the control is a not a normal control, then the string of the label teels us how to find it.
                    if (ae.Current.Name != identifier.ToString())
                    {
                        string[] tokens = ae.Current.Name.Split('\\');
                        switch (tokens[0])
                        {
                            //Dialog/<Name of the dialog>
                            case "Dialog":
                                {
                                    // Find the control window
                                    while ((ae != null) && (ae.Current.ControlType != ControlType.Window))
                                    {
                                        ae = TreeWalker.ControlViewWalker.GetParent(ae);
                                    }
                                    if (ae == null)
                                    {
                                        Exception exception = new ArgumentException("Could not find the parent window to seach for the dialog from");
                                        UIVerifyLogger.LogError(exception);
                                        UIVerifyLogger.EndTest();
                                        throw exception;
                                    }

                                    // Find the actual dialog
                                    ae = ae.FindFirst(TreeScope.Subtree,
                                        new System.Windows.Automation.AndCondition                                        
                                        (
                                            new System.Windows.Automation.Condition[]
                                        {
                                                new System.Windows.Automation.PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window), 
                                            new System.Windows.Automation.PropertyCondition(AutomationElement.NameProperty, tokens[1]),
                                        }
                                        )
                                    );

                                    if (ae == null)
                                    {
                                        Exception exception = new ArgumentException("Could not find the dialog");
                                        UIVerifyLogger.LogError(exception);
                                        UIVerifyLogger.EndTest();
                                        throw exception;
                                    }

                                    break;
                                }
                            default:
                                {
                                    // Found control
                                    break;
                                }
                        }

                    }
                }
                UIVerifyLogger.LogPass();
                UIVerifyLogger.EndTest();
                return ae;
            }

            #endregion IControlLookUp


        }

        sealed class CtrlTestEdit : Base
        {
            #region Methods

            /// ---------------------------------------------------------------
            /// <summary>
            /// Constructor
            /// </summary>
            /// ---------------------------------------------------------------
            internal CtrlTestEdit(AutomationElement element)
                : base(element)
            {
            }

            #endregion Methods

            #region IDisposable

            /// <summary></summary>
            public new void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary></summary>
            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    base.Dispose();
                }
            }

            #endregion Disposable


        }

        sealed class CtrlTestEditWinForms : Base
        {
            #region Methods

            /// ---------------------------------------------------------------
            /// <summary>
            /// Constructor
            /// </summary>
            /// ---------------------------------------------------------------
            internal CtrlTestEditWinForms(AutomationElement element)
                : base(element)
            {
            }

            #endregion Methods

            #region IDisposable

            /// <summary></summary>
            public new void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary></summary>
            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    base.Dispose();
                }
            }

            #endregion Disposable

        }
    }

    public class WrapperApplication : IDisposable
    {
        #region Variables
        AutomationElement _element;

        object _application;

        IntPtr _mainWindowHandle;

        string _caption;

        string _exeName;

        string _exeArgs;

        string _appName;

        Process _appProcess = null;

        const string ID_CTRLTEST = "CTRLTEST.EXE";

        const string ID_CTRLTESTEDIT = "CTRLTESTEDIT.EXE";

        const string ID_CTRLTESTEDITWINFORMS = "CTRLTESTEDITWINFORMS.EXE";

        const string ID_CTRLTESTWINFORMS = "CTRLTESTWINFORMS.EXE";

        const string ID_WORDPAD = "WORDPAD.EXE";

        const string ID_CONSOLE = "CMD.EXE";

        const string ID_AVAAPP = "AVAAPP.EXE";

        const string ID_AVACTRLAPP = "AVACTRLAPP.EXE";

        const string ID_SCREENREADER = "SCREENREADER.EXE";

        const string ID_NOTEPAD = "NOTEPAD.EXE";

        #endregion Variables

        /// -------------------------------------------------------------------
        /// <summary>
        /// Used to drive the UI
        /// </summary>
        /// -------------------------------------------------------------------
        public object ApplicationUI
        {
            get
            {
                return _application;
            }
        }

        #region IDispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_appProcess != null)
                    _appProcess.Dispose();
            }
        }

        #endregion IDispose

        /// -------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// -------------------------------------------------------------------
        public WrapperApplication(string caption, string exeName, string exeArgs)
        {


            if (string.IsNullOrEmpty(caption) && string.IsNullOrEmpty(exeName) && string.IsNullOrEmpty(exeArgs))
                throw new NullReferenceException();

            // PRESharp warning: This argument is already validated above.
#pragma warning disable 6504
            if (!string.IsNullOrEmpty(exeName))
            {
#pragma warning suppress 6504
                _exeName = exeName;

                // Remove any path information
                int iLoc = _exeName.LastIndexOf(@"\");
                _appName = iLoc == -1 ? _appName = _exeName : _exeName.Substring(iLoc + 1);

                /////// Logging.LogComment("Initializing WrapperApplication(" + _appName + ")");
            }

            // PRESharp warning: This argument is already validated above. Null/Empty is allowed
#pragma warning disable 6504
            if (!string.IsNullOrEmpty(caption))
#pragma warning suppress 6504
                _caption = caption;

            // PRESharp warning: This argument is already validated above. Null/empty is allowed
#pragma warning disable 6504
            if (!string.IsNullOrEmpty(exeArgs))
#pragma warning suppress 6504
                _exeArgs = exeArgs;

        }

        /// ----------------------------------------------------------
        /// <summary>
        /// Sets up the correct Applications object
        /// </summary>
        /// ----------------------------------------------------------
        void InitializeApplication()
        {
            Debug.Assert(_element != null);
            switch (_appName.ToUpper())
            {

                case ID_NOTEPAD:
                    {
                        _application = new Applications.NotePad(_element);
                        break;
                    }

                case ID_SCREENREADER:
                    {
                        _application = new Applications.ScreenReader(_element);
                        break;
                    }

                case ID_WORDPAD:
                    {
                        _application = new Applications.WordPad(_element);
                        break;
                    }

                case ID_CTRLTESTWINFORMS:
                    {
                        _application = new Applications.CtrlTestWinForms(_element);
                        break;
                    }

                case ID_CTRLTEST:
                    {
                        _application = new Applications.CtrlTest(_element);
                        break;
                    }

                case ID_CTRLTESTEDIT:
                    {
                        _application = new Applications.CtrlTestEdit(_element);
                        break;
                    }

                case ID_CTRLTESTEDITWINFORMS:
                    {
                        _application = new Applications.CtrlTestEditWinForms(_element);
                        break;
                    }

                case ID_CONSOLE:
                    {
                        _application = new Applications.ConsoleWindow(_element);
                        break;
                    }

                default:
                    {
                        _application = new Applications.GenericWindow(_element);
                        break;
                    }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Interface wrapper to the application object.  This can be used to 
        /// obtain elements such as the menu bars
        /// </summary>
        /// <value></value>
        /// -------------------------------------------------------------------
        public IApplicationCommands IApplicationCommands
        {
            get
            {
                Debug.Assert(_application != null);
                return (IApplicationCommands)_application;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Retrieves the AutomationElement using known identifier's
        /// </summary>
        /// -------------------------------------------------------------------
        public AutomationElement AutomationElementFromCustomId(object identifier)
        {
            Debug.Assert(_application != null);

            if (identifier != null)
                return ((IControlLookUp)_application).AutomationElementFromCustomId(identifier.ToString());
            else
                throw new ArgumentException("Argument passed in cannot by null");
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// -------------------------------------------------------------------
        public void CauseStructureChange(AutomationElement element, StructureChangeType type)
        {
            Debug.Assert(_application != null);
            ((IWUIStructureChange)_application).CauseStructureChange(element, type);
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// AutomationElement of the application
        /// </summary>
        /// <value></value>
        /// -------------------------------------------------------------------
        public AutomationElement AutomationElement
        {
            get
            {
                return _element;
            }
        }

        public IntPtr MainWindowHandle
        {
            get
            {
                return _mainWindowHandle;
            }
        }

        /// -------------------------------------------------------------------------
        /// <summary>
        /// Start the application and retrieve it's AutomationElement as a member variable.
        /// </summary>
        /// -------------------------------------------------------------------------
        public void StartApplication()
        {

            Library.StartApplication(_exeName, _exeArgs, out _mainWindowHandle, out _element, out _appProcess);

            // Set up the interfaces
            InitializeApplication();
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Start the application and retrieve it's AutomationElement as a 
        /// member variable and move the application to a specific location.
        /// </summary>
        /// -------------------------------------------------------------------
        public void StartApplication(Rect location)
        {
            StartApplication();
            MoveWindow(location);
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Call the MoveTo and Resize to move a window
        /// </summary>
        /// -------------------------------------------------------------------
        public void MoveWindow(Rect rectangle)
        {
            TransformPattern wp = _element.GetCurrentPattern(TransformPattern.Pattern) as TransformPattern;

            if (wp == null)
                throw new Exception("Window does not support the TransformPattern so cannot position window");

            if (wp.Current.CanMove.Equals(true))
            {
                /////// Logging.LogComment("Calling WindowPattern.MoveTo(" + rectangle.Left + ", " + rectangle.Top + ")");

                //wp.Move(rectangle.Left, rectangle.Top);
            }

            if (wp.Current.CanResize.Equals(true))
            {
                /////// Logging.LogComment("Calling WindowPattern.Resize(" + rectangle.Width + ", " + rectangle.Top + ")");
                wp.Resize((int)rectangle.Width, (int)rectangle.Height);
            }
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Maximize the application
        /// </summary>
        /// -------------------------------------------------------------------
        public void Maximize()
        {
            TransformPattern tp = _element.GetCurrentPattern(TransformPattern.Pattern) as TransformPattern;

            if (tp == null)
                throw new Exception("Window does not support the TransformPattern so cannot maximize window");

            if (tp.Current.CanResize)
            {
                WindowPattern wp = _element.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;

                wp.SetWindowVisualState(WindowVisualState.Maximized);
            }

            /////// Logging.LogComment("Maximized the window successfully");
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Close down the application
        /// </summary>
        /// -------------------------------------------------------------------
        public void CloseApplication()
        {
            try
            {
                if (_appProcess != null)
                    this._appProcess.Kill();
            }
            catch (InvalidOperationException)
            {
                Debug.WriteLine("The process has already been killed");
            }
        }
    }
}
