// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//******************************************************************* 
//* Purpose: 
//* Owner: Microsoft
//* Contributors:
//******************************************************************* 
using System;
using System.CodeDom;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Text;
using System.Windows.Automation;
using System.Windows;
using System.Xml;

namespace Microsoft.Test.WindowsUIAutomation.Tests.Scenarios
{
    using InternalHelper.Enumerations;
    using InternalHelper.Tests;
    using Microsoft.Test.WindowsUIAutomation;
    using Microsoft.Test.WindowsUIAutomation.Core;
    using Microsoft.Test.WindowsUIAutomation.Interfaces;
    using Microsoft.Test.WindowsUIAutomation.TestManager;

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class MenuScenarioTests : ScenarioObject
    {
        const string THIS = "MenuScenarioTests";

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public const string TestSuite = NAMESPACE + "." + THIS;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public MenuScenarioTests(AutomationElement element, string testSuite, TestPriorities priority, TypeOfControl typeOfControl, TypeOfPattern typeOfPattern, string dirResults, bool testEvents, IApplicationCommands commands)
            :
            base(element, testSuite, priority, TypeOfControl.UnknownControl, TypeOfPattern.Unknown, null, testEvents, commands)
        {
        }

        #region Menu Navigation

        #region Test Menu Structure

        /// -------------------------------------------------------------------
        /// <summary>
        /// Starting point for tests (template)
        /// </summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("Test Menu Structure and Names",
            TestSummary = "Verify that the menu has a specific structure and that the names are as expected",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
           TestCaseType = TestCaseType.Scenario,
           Client = Client.ATG,
            Description = new string[] {
                "Step: Load the argument into an XmlDocument",
                "Verify: That the menu tree matches the XmlDocument tree structure by name"
            })]
        public void TestMenuStructureAndNames(TestCaseAttribute testCase, object[] arguments)
        {
            XmlDocument doc = new XmlDocument();

            if (arguments == null)
                throw new ArgumentException();

            string xml = (string)arguments[0];

            //"Step: Load the argument into an XmlDocument",
            TS_LoadMenuDefinition(xml, ref doc, CheckType.Verification);

            //"Verify: That the menu tree matches the XmlDocument tree structure by name"
            TestMenu menuBar = _appCommands.GetIWUIMenuCommands().GetMenuBar().GetFirstSubMenu();
            TS_VerifyXmlToTree(doc.DocumentElement.FirstChild, menuBar);

        }

        #endregion Test Menu Structure

        #endregion Menu Navigation

        /// -------------------------------------------------------------------
        /// <summary>
        /// This will test that the menu and it's menu names are as defined in the Xml tree
        /// </summary>
        /// -------------------------------------------------------------------
        private void TS_VerifyXmlToTree(XmlNode xmlNode, TestMenu menu)
        {
            // If they are both null, we are at the end...just return then
            if (xmlNode == null && menu == null)
                return;

            // Make sure both are either null, or something
            if ((xmlNode == null && menu != null) || (xmlNode != null && menu == null))
                ThrowMe(CheckType.Verification, "Mismatch1!");

            string curName = menu.AutomationElement.GetCurrentPropertyValue(AutomationElement.NameProperty) as string;
            string expName = new StringBuilder(xmlNode.Name).Replace("_", " ").ToString();

            // Verify that the NameProperty returned something valid
            if (String.IsNullOrEmpty(curName))
                ThrowMe(CheckType.Verification, "AutomationElement.NameProperty is empty or null");

            // Everything looks good, so tell the user what we are doing
            Comment("Comparing expected name(" + expName + ") with actual name(" + curName + ")");

            if (expName != curName)
                ThrowMe(CheckType.Verification, "Expected element with name of " + expName + ", yet it was " + curName);

            // MenuItems need to be expanded to get to the children.  Menu's don't since they are only containers that are
            // already opened.
            if (menu.Expandable)
                menu.Expand();

            // Recurse the children and siblings
            if (xmlNode.FirstChild != null)
            {
                TS_VerifyXmlToTree(xmlNode.FirstChild, menu.GetFirstSubMenu());
            }

            if (xmlNode.NextSibling != null)
            {
                TS_VerifyXmlToTree(xmlNode.NextSibling, menu.GetNextSiblingMenu());
            }

        }

        private void TS_LoadMenuDefinition(string xml, ref XmlDocument doc, CheckType checkType)
        {
            doc.LoadXml(xml);
            m_TestStep++;
        }

        private void TS_SupportsMainMenu(bool expected, CheckType checkType)
        {
            bool supports = _appCommands.SupportsIWUIMenuCommands();
            if (supports != expected)
                ThrowMe(checkType, "Supports menus != " + expected);

            Comment("Application supports menus");
            m_TestStep++;
        }
    }
}

 
