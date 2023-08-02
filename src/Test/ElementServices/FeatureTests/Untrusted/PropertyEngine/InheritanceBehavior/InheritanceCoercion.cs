// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.ComponentModel;
using System.Globalization;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Markup;

namespace Avalon.Test.CoreUI.PropertyEngine
{
    /// <summary>
    /// <description>
    ///  Regression coverage for 





    [Test(1, "PropertyEngine", TestCaseSecurityLevel.PartialTrust, "InheritanceCoercion", Versions = "4.0+,4.0Client+")]  // 
    [Test(0, "PropertyEngine.DependencyObject", TestCaseSecurityLevel.FullTrust, "TestDependencyObjectType", Versions = "4.0+,4.0Client+")]  // 
    public class InheritanceCoercion : AvalonTest
    {
        #region Constructors

        public InheritanceCoercion()
        {
            InitializeSteps += new TestStep(TreeWalk);
            RunSteps += new TestStep(TreeChange);
            RunSteps += new TestStep(TreeChangeMultipleDecsendants);
        }

        #endregion

        #region Private Members

        private TestResult TreeWalk()
        {
            WaitForPriority(DispatcherPriority.Render);

            // Create Element Tree
            MyGrid grandParent = new MyGrid();
            MyGrid parent = new MyGrid();
            MyGrid child = new MyGrid();
            MyGrid grandChild = new MyGrid();

            child.Children.Add(grandChild);
            parent.Children.Add(child);
            grandParent.Children.Add(parent);

            // Define a coercion value on the child.
            child.StateCoercionValue = "child";

            // Set grandParent State.
            grandParent.State = "grand";

            // Property should be inherited down the tree.
            if (grandParent.State != "grand" || parent.State != "grand")
            {
                LogComment("Initial States were not inherited correctly.");
                return TestResult.Fail;
            }

            // Coercion Value should be set and inherited down the tree.
            if (child.State != "child" || grandChild.State != "child")
            {
                LogComment("Coerced States were not inherited correctly.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult TreeChange()
        {
            WaitForPriority(DispatcherPriority.Render);

            // Create Element Tree
            MyGrid grandParent = new MyGrid();
            MyGrid parent = new MyGrid();
            MyGrid child = new MyGrid();
            MyGrid grandChild = new MyGrid();

            child.Children.Add(grandChild);
            parent.Children.Add(child);

            // Define a coercion value on the child.
            child.StateCoercionValue = "child";

            // Set grandParent State and add the rest of the tree.
            grandParent.State = "grand";
            grandParent.Children.Add(parent);

            // Property should be inherited down the tree.
            if (grandParent.State != "grand" || parent.State != "grand")
            {
                LogComment("Initial States were not inherited correctly.");
                return TestResult.Fail;
            }

            // Coercion Value should be set and inherited down the tree.
            if (child.State != "child" || grandChild.State != "child")
            {
                LogComment("Coerced States were not inherited correctly.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult TreeChangeMultipleDecsendants()
        {
            WaitForPriority(DispatcherPriority.Render);

            // Create 2 Element Trees with varying lengths. We will set coercion at different depths to see if it still works.
            MyGrid grandParent = new MyGrid();

            MyGrid parentChainOne = new MyGrid();
            MyGrid childChainOne = new MyGrid();
            MyGrid grandChildChainOne = new MyGrid();

            MyGrid parentChainTwo = new MyGrid();
            MyGrid childChainTwo = new MyGrid();
            MyGrid grandChildChainTwo = new MyGrid();
            MyGrid granderChildChainTwo = new MyGrid();

            childChainOne.Children.Add(grandChildChainOne);
            parentChainOne.Children.Add(childChainOne);

            grandChildChainTwo.Children.Add(granderChildChainTwo);
            childChainTwo.Children.Add(grandChildChainTwo);
            parentChainTwo.Children.Add(childChainTwo);

            // Define a coercion value on the child.
            childChainOne.StateCoercionValue = "child chain one";
            grandChildChainTwo.StateCoercionValue = "grand child chain two";

            // Set grandParent State.
            grandParent.State = "grand";
            grandParent.Children.Add(parentChainOne);
            grandParent.Children.Add(parentChainTwo);

            // Property should be inherited down the tree.
            if (grandParent.State != "grand" || parentChainOne.State != "grand")
            {
                LogComment("Initial States were not inherited correctly in chain one.");
                return TestResult.Fail;
            }

            if (grandParent.State != "grand" || parentChainTwo.State != "grand" || childChainTwo.State != "grand")
            {
                LogComment("Initial States were not inherited correctly in chain two.");
                return TestResult.Fail;
            }

            // Coercion Value should be set and inherited down the tree.
            if (childChainOne.State != "child chain one" || grandChildChainOne.State != "child chain one")
            {
                LogComment("Coerced States were not inherited correctly in chain one.");
                return TestResult.Fail;
            }

            if (grandChildChainTwo.State != "grand child chain two" || granderChildChainTwo.State != "grand child chain two")
            {
                LogComment("Coerced States were not inherited correctly in chain two.");
                return TestResult.Fail;
            }

            // Remove coercion
            childChainOne.StateCoercionValue = null;
            grandChildChainTwo.StateCoercionValue = null;
            grandParent.Children.Remove(parentChainOne);
            grandParent.Children.Remove(parentChainTwo);

            // Property values should be default now.
            if (parentChainOne.State != "Default" || childChainOne.State != "Default" || grandChildChainOne.State != "Default" ||
                parentChainTwo.State != "Default" || childChainTwo.State != "Default" || grandChildChainTwo.State != "Default" ||
                granderChildChainTwo.State != "Default")
            {
                LogComment("Removal from tree did not reset to default values.");
                return TestResult.Fail;
            }


            return TestResult.Pass;
        }

        #endregion

    }

	#region Helper Classes

    public class MyGrid : Grid
    {
        public MyGrid()
            : base()
        {
        }

        public static readonly DependencyProperty StateProperty
        = DependencyProperty.RegisterAttached("State", typeof(string), typeof(MyGrid),
                                      new FrameworkPropertyMetadata("Default",
                                        FrameworkPropertyMetadataOptions.Inherits,
                                        null,
                                        new CoerceValueCallback(CoerceState)));

        public string State
        {
            get { return (string)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        internal string StateCoercionValue { get; set; }

        private static object CoerceState(DependencyObject dependencyObject, object baseValue)
        {
            MyGrid myDo = (MyGrid)dependencyObject;
            return (myDo.StateCoercionValue == null) ? baseValue : myDo.StateCoercionValue;
        }


        #region IAddChild Members

        public void AddChild(object value)
        {
            throw new NotImplementedException();
        }

        public void AddText(string text)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

	#endregion
}
