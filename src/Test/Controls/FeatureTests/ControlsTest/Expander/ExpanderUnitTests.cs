
//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
// All classes in this file are test cases for Expander, all of them are related 
// to Expader; include functionality such as Automation, Serialization, Visual,DataBing, 
// basic Property,Event and Method and so on.
//---------------------------------------------------------------------------



#region Using directives

using System;
using System.Windows.Controls;
using System.Xml;
using Avalon.Test.ComponentModel.Validations;
using Avalon.Test.ComponentModel.Actions;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Logging;
using System.Windows;
using System.Windows.Data;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Automation.Peers;
using System.Windows.Markup;
using System.Threading;
using System.Collections;
using System.Windows.Documents;
using Avalon.Test.ComponentModel.UnitTests;
using Microsoft.Test.Input;

#endregion



namespace Avalon.Test.ComponentModel.UnitTests
{
    /// <summary>
    /// P1 class to test invoking Expander by automation; To run this test case, one parameters should be provided: XTC file name
    /// for example : for example : /ExpanderTest.xtc 
    /// </summary>
    [TargetType(typeof(Expander))]
    class ExpanderAutomationInvoke : IUnitTest
    {
        /// <summary>
        /// Test Invoking Expaner by automation
        /// </summary>
        /// <param name="testElement"></param>
        /// <param name="variation"></param>
        /// <returns></returns>
        public TestResult Perform(object testElement, XmlElement variation)
        {
            expander = testElement as Expander;
            bool preIsExpanded = expander.IsExpanded;

            //hook event handlers
            expander.Expanded += new RoutedEventHandler(ep_Expanded);
            expander.Collapsed += new RoutedEventHandler(ep_Collapsed);
            ControlTestHelper.ExecuteActions(expander,variation);

            //restore status
            QueueHelper.WaitTillQueueItemsProcessed();
            expander.Expanded -= new RoutedEventHandler(ep_Expanded);
            expander.Collapsed -= new RoutedEventHandler(ep_Collapsed);
            if (isExpanded && isCollapsed)
            {
                expander.IsExpanded = preIsExpanded;
                return TestResult.Pass;
            }
            GlobalLog.LogEvidence("Fail: ExpanderAutomationInvoke: Error bahavior by Automation !");
            return TestResult.Fail;
        }


        /// <summary>
        /// Handler for Expanded event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ep_Expanded(object sender, RoutedEventArgs e)
        {
            isExpanded = true;
        }


        /// <summary>
        /// Handler for Collapsed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ep_Collapsed(object sender, RoutedEventArgs e)
        {
            isCollapsed = true;
        }



        /// <summary>
        /// variables
        /// </summary>
        private Expander expander = null;
        private bool isExpanded = false;
        private bool isCollapsed = false;
    }



    /// <summary>
    /// P1 class to test invoking by Interface methods; To run this test case, one parameters should be provided: XTC file name
    /// for example : for example : /ExpanderTest.xtc 
    /// </summary>
    [TargetType(typeof(Expander))]
    class ExpanderAutomationInterface : IUnitTest
    {
        /// <summary>
        /// test invoking by Interface IExpandCollapseProvider
        /// </summary>
        /// <param name="testElement"></param>
        /// <param name="variation"></param>
        /// <returns></returns>
        public TestResult Perform(object testElement, XmlElement variation)
        {
            // invoke by interface
            AutomationPeer ap = UIElementAutomationPeer.CreatePeerForElement(expander);
            IExpandCollapseProvider ip = (IExpandCollapseProvider)ap.GetPattern(PatternInterface.ExpandCollapse);
            try
            {
                if (ip == null)
                    throw new NotSupportedException("Expander does not support IExpandCollapseProvider automation");
            }
            catch (Exception)
            {
                GlobalLog.LogEvidence("Fail: ExpanderAutomationInterface: Error status after invoking by Inteface. !");
                return TestResult.Fail;
            }
         
            //Change status and validate .
            //Change status and validate .
             if (ip.ExpandCollapseState == ExpandCollapseState.Collapsed)
            {
                ip.Expand();
                if (ip.ExpandCollapseState == ExpandCollapseState.Expanded)
                {
                    return TestResult.Pass;
                }
            }
            else if (ip.ExpandCollapseState == ExpandCollapseState.Expanded)
            {
                ip.Collapse();
                if (ip.ExpandCollapseState == ExpandCollapseState.Collapsed)
                {
                    return TestResult.Pass;
                }
            }
            GlobalLog.LogEvidence("Fail: ExpanderAutomationInterface: Error status after invoking by Inteface. !");
            return TestResult.Fail;
        }



        //variables
        private Expander expander = null;
    }



    /// <summary>
    /// P1 class to test MouseOver property; To run this test case, one parameters should be provided: XTC file name
    /// for example : for example : /ExpanderTest.xtc 
    /// </summary>
    [TargetType(typeof(Expander))]
    class ExpanderMouseoverProperty : IUnitTest
    {
        /// <summary>
        /// test triggering mouse over event and checking its properties
        /// </summary>
        /// <param name="testElement"></param>
        /// <param name="variation"></param>
        /// <returns></returns>
        public TestResult Perform(object testElement, XmlElement variation)
        {
            expander = testElement as Expander;
            
            //Set Header and Content as an button respectively
            FrameworkElement head = expander.Header as FrameworkElement;
            FrameworkElement content = expander.Content as FrameworkElement;

            // Mouse over every Control in Expander and validate status
            UserInput.MouseMove(head, 5, 5);
            QueueHelper.WaitTillQueueItemsProcessed();
            if (!expander.IsMouseOver && head.IsMouseOver)
            {
                GlobalLog.LogEvidence("Fail: ExpanderMouseoverProperty: Error Mouse Over behavior !");
                return TestResult.Fail;
            }

            UserInput.MouseMove(content, 5, 5);
            QueueHelper.WaitTillQueueItemsProcessed();
            if (!expander.IsMouseOver && content.IsMouseOver)
            {
                GlobalLog.LogEvidence("Fail: ExpanderMouseoverProperty: Error Mouse Over behavior !");
                return TestResult.Fail;
            }

            UserInput.MouseMove(expander, 15, 15);
            QueueHelper.WaitTillQueueItemsProcessed();
            if (!expander.IsMouseOver)
            {
                GlobalLog.LogEvidence("Fail: ExpanderMouseoverProperty: Error Mouse Over behavior !");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }



        //Expander instance variable
        private Expander expander = null;
    }

}
