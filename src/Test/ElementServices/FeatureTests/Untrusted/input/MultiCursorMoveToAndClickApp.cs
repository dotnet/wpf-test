// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Threading;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Microsoft.Test.Win32;
using MTI = Microsoft.Test.Input;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify multiple cursors in an application upon move to and click.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <remarks>
    /// Until UI Automation difficulties are resolved, this case is disabled.
    /// </remarks>
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class MultiCursorMoveToAndClickApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCaseContainer("All", "All", "1", @"CoreInput\Cursor", "", @"1", TestCaseSecurityLevel.FullTrust, @"Verify focus works for elements in multiple windows")]
        public void LaunchTest()
        {
            Run();
        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Loading XAML...");
            Stream stream = File.OpenRead("CoreInput_GridPanelCursors.xaml");
            object panel;

            try
            {
                ParserContext pc = new ParserContext();
                pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
                panel = System.Windows.Markup.XamlReader.Load(stream, pc);
            }
            finally
            {
                stream.Close();
            }

            CoreLogger.LogStatus("Grabbing panel...");
            _gridPanel = panel as Grid;

            CoreLogger.LogStatus("Showing window...");
            DisplayMe(_gridPanel, 10, 10, 600, 400);

            return null;

        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            Assert(_gridPanel != null, "Couldn't find grid!");

            Hashtable table = new Hashtable();

            TreeHelper.FindNodesWithIds(_gridPanel, table);

            TypeConverter ctc = new CursorConverter();

            foreach (DictionaryEntry de in table)
            {
                // Validate entries in our table
                FrameworkElement el = de.Value as FrameworkElement;

                Assert(el != null, "Expected element not found!");

                string elId = de.Key as string;

                Assert(elId != null, "Expected string not found!");
                CoreLogger.LogStatus("el='" + el.ToString() + "', Name='" + elId + "'");

                // Construct cursor from our element
                Cursor c = el.Cursor;
                string sFromCursor = ctc.ConvertTo(c, typeof(string)) as string;

                Assert(((sFromCursor != "") || (sFromCursor != null)), "no real cursor!");

                // Compare cursor to what is specified in markup.
                Assert(elId.EndsWith(sFromCursor), "Cursor value doesn't match string value!");
            }

            CoreLogger.LogStatus("Going to every element...");
            System.Windows.Automation.AutomationElement logRoot = FindLogicalElementByID("Window A");

            table = new Hashtable();
            TreeHelper.FindNodesWithIds(_gridPanel, table);

            foreach (DictionaryEntry de in table)
            {
                InstrFrameworkPanel icp = (InstrFrameworkPanel)de.Value;
                MTI.Input.MoveToAndClick(icp);
            }

            base.DoExecute(arg);

            return null;
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object arg)
        {
            CoreLogger.LogStatus("Validating...");

            this.TestPassed = true;
            CoreLogger.LogStatus("Test pass status? " + TestPassed);

            return null;
        }


        private void CheckCursors()
        {
            foreach (DictionaryEntry de in _logicalElements)
            {
                AutomationElement clickableEl = (AutomationElement)(de.Value);

                //MTI.Input.MoveToAndClick( clickableEl ).
                Rect rc = (Rect)clickableEl.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);

                MTI.Input.MoveToAndClick(new Point(rc.Left + rc.Width / 2, rc.Top + rc.Height / 2));
            }
        }

        private AutomationElement FindLogicalElementByID(string id)
        {
            System.Windows.Automation.AutomationElement p = System.Windows.Automation.AutomationElement.RootElement;
            return FindLogicalElementByID(id, p);
        }

        private AutomationElement FindLogicalElementByID(string id, AutomationElement parent)
        {
            System.Windows.Automation.Condition condition = new System.Windows.Automation.PropertyCondition(AutomationElement.AutomationIdProperty, id);
            TreeWalker customWalker = new TreeWalker(condition);

            return customWalker.GetFirstChild(parent);
        }

        private Grid _gridPanel;

        private void OnMouseButton(object sender, MouseButtonEventArgs e)
        {
            InstrFrameworkPanel btn = sender as InstrFrameworkPanel;

            _btnOutput.Content = "." + (btn.Cursor.ToString()) + ";";
        }

        private Button _btnOutput = null;

        private Hashtable _logicalElements = new Hashtable();

        private FrameworkElement FindFrameworkElement(Type type, string id, FrameworkElement root)
        {
            if (root == null)
            {
                return null;
            }

            if ((root.GetType().Equals(type)) && (root.Name == id))
            {
                return root;
            }

            int count = VisualTreeHelper.GetChildrenCount(root);
            for(int i = 0; i < count; i++)
            {
                // Common base class for Visual and Visual3D is DependencyObject
                DependencyObject child = VisualTreeHelper.GetChild(root,i);
                FrameworkElement feRet = FindFrameworkElement(type, id, child as FrameworkElement);

                if (feRet != null)
                    return feRet;
            }

            return null;
        }
    }
}


