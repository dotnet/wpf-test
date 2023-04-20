// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Test;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
    /// This provides coverage for the DCR that allows ElementName in a template to bind to elements outside
    /// of the template.
    /// Scenario 1 - Bind a DP in a template of ItemsControl to an item in the logical tree.
    /// Scenario 2 - Bind a DP in a template of ContentControl to an item in the logical tree and in another 
    /// template up in the chain of templates.
    /// Scenario 3 - Bind a DP in a template of ItemsControl to a DP in the generated container.
	/// </description>
	/// <relatedBugs>

	/// </relatedBugs>
    /// <relatedTasks>

    /// </relatedTasks>
	/// </summary>
    [Test(1, "Binding", "ElementNameScope")]
    public class ElementNameScope : XamlTest
    {
        private ListBox _lb1;
        private Button _btn1;
        private ListBox _lb2;

        public ElementNameScope()
            : base(@"ElementNameScope.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            // Scenario 1 - Bind a DP in a template of ItemsControl to an item in the logical tree 
            RunSteps += new TestStep(ItemsControlLogicalTree);
            // Scenario 2 - Bind a DP in a template of ContentControl to an item in the logical tree 
            // and in another template up in the chain of templates 
            RunSteps += new TestStep(ContentControlLogicalVisualTree);
            // Scenario 3 - Bind a DP in a template of ItemsControl to a DP in the generated container
       }

        private TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.Render);

            _lb1 = (ListBox)LogicalTreeHelper.FindLogicalNode(this.RootElement, "lb1");
            _btn1 = (Button)LogicalTreeHelper.FindLogicalNode(this.RootElement, "btn1");
            _lb2 = (ListBox)LogicalTreeHelper.FindLogicalNode(this.RootElement, "lb2");

            return TestResult.Pass;
        }

        private TestResult ItemsControlLogicalTree()
        {
            Status("ItemsControlLogicalTree");
            return TestItemsForeground(_lb1, "tb", Brushes.IndianRed);
        }

        private TestResult ContentControlLogicalVisualTree()
        {
            Status("ContentControlLogicalVisualTree");

            Border border1 = (Border)Util.FindElement(_btn1, "border1");
            Border border2 = (Border)Util.FindElement(_btn1, "border2");

            if (!TestBrush(border1.BorderBrush, Brushes.IndianRed))
            {
                return TestResult.Fail;
            }
            if (!TestBrush(border2.BorderBrush, Brushes.Orange))
            {
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult TestItemsForeground(ListBox lb, string nameElement, Brush color)
        {
            FrameworkElement[] elementList = Util.FindElements(lb, nameElement);
            foreach (TextBlock tb in elementList)
            {
                if (!TestBrush(tb.Foreground, color))
                {
                    return TestResult.Fail;
                }
            }
            return TestResult.Pass;
        }

        private bool TestBrush(Brush actual, Brush expected)
        {
            if (actual != expected)
            {
                LogComment("Fail - Actual brush: " + actual + ". Expected: " + expected);
                return false;
            }
            return true;
        }
    }
}
