// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Automation;
using System.Windows.Automation.Text;
using System.Collections;

namespace Microsoft.Test.WindowsUIAutomation.Interfaces
{
	using InternalHelper;
    using InternalHelper.Enumerations;
	using Microsoft.Test.WindowsUIAutomation;
	using Microsoft.Test.WindowsUIAutomation.Core;
	using Microsoft.Test.WindowsUIAutomation.TestManager;

    /// -----------------------------------------------------------------------
    /// <summary>
    /// Interface so that we can standardize the call to SearchString given two
    /// different applications.
    /// </summary>
    /// -----------------------------------------------------------------------
    public interface IControlLookUp
    {
        /// -------------------------------------------------------------------
        /// <summary>
        /// Method returns an AutomationElement based on some defined ID
        /// </summary>
        /// -------------------------------------------------------------------
        AutomationElement AutomationElementFromCustomId(object ID);
    }

    /// -----------------------------------------------------------------------
    /// <summary>
    /// Interface that determines how to drive some abstract applications menus
    /// </summary>
    /// -----------------------------------------------------------------------
    public interface IWUIMenuCommands
    {
        /// -------------------------------------------------------------------
        /// <summary>
        /// Returns the applications main menu bar
        /// </summary>
        /// -------------------------------------------------------------------
        TestMenu GetMenuBar();

        /// -------------------------------------------------------------------
        /// <summary>
        /// Returns the applications system menu bar
        /// </summary>
        /// -------------------------------------------------------------------
        TestMenu GetSystemMenu();
    }

    /// -----------------------------------------------------------------------
    /// <summary>
    /// Interface that cause an object to change it's properties
    /// </summary>
    /// -----------------------------------------------------------------------
    public interface IWUIPropertyChange
    {
        /// -------------------------------------------------------------------
        /// <summary>
        /// Fire off a change
        /// </summary>
        /// -------------------------------------------------------------------
        void ChangeProperty(AutomationProperty property, object automationID);
    }

    /// -----------------------------------------------------------------------
    /// <summary>
    /// Interface that will cause StructureChange events to occur on some 
    /// abstract application
    /// </summary>
    /// -----------------------------------------------------------------------
    public interface IWUIStructureChange
    {
        /// -------------------------------------------------------------------
        /// <summary>
        /// Call this the wrapper application to cause a specific StructureChange 
        /// to occur.  The wrapper application will need to implement this to 
        /// successfully cause the event to occur
        /// </summary>
        /// -------------------------------------------------------------------
        bool CauseStructureChange(AutomationElement element, StructureChangeType changeType);

        /// -------------------------------------------------------------------
        /// <summary>
        /// Resets the control to it's initial state after adding or deleting 
        /// any elements
        /// </summary>
        /// -------------------------------------------------------------------
        bool ResetControl(AutomationElement element);

        /// -------------------------------------------------------------------
        /// <summary>
        /// Method that will determine if the element supports tests for 
        /// structure change
        /// </summary>
        /// -------------------------------------------------------------------
        bool DoesControlSupportStructureChange(AutomationElement element, StructureChangeType structureChangeType);
    }

    /// -----------------------------------------------------------------------
    /// <summary>
    /// Main interface holder for the different interfaces for an abstract 
    /// application
    /// </summary>
    /// -----------------------------------------------------------------------
    public interface IApplicationCommands
    {
        /// -------------------------------------------------------------------
        /// <summary>
        /// Method that the test application can call to relay any information
        /// such as status information
        /// </summary>
        /// -------------------------------------------------------------------
        void TraceMethod(object information);

        /// -------------------------------------------------------------------
        /// <summary>
        /// Does this support IWUIMenu
        /// </summary>
        /// -------------------------------------------------------------------
        bool SupportsIWUIMenuCommands();
        /// -------------------------------------------------------------------
        /// <summary>
        /// Does this support IWUIStructureChange
        /// </summary>
        /// -------------------------------------------------------------------
        bool SupportsIWUIStructureChange(AutomationElement element);
        /// -------------------------------------------------------------------
        /// <summary>
        /// Does this support IWUIPropertyChange
        /// </summary>
        /// -------------------------------------------------------------------
        bool SupportsIWUIPropertyChange(AutomationElement element);
        /// -------------------------------------------------------------------
        /// <summary>
        /// Returns IWUIMenuCommands
        /// </summary>
        /// -------------------------------------------------------------------
        IWUIMenuCommands GetIWUIMenuCommands();
        /// -------------------------------------------------------------------
        /// <summary>
        /// Returns IWUIStructureChange
        /// </summary>
        /// -------------------------------------------------------------------
        IWUIStructureChange GetIWUIStructureChange();
        /// -------------------------------------------------------------------
        /// <summary>
        /// Return IWUIPropertyChange
        /// </summary>
        /// -------------------------------------------------------------------
        IWUIPropertyChange GetIWUIPropertyChange();
    }
}





           

