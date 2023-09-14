// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.PropertyEngine;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Modeling;
using Microsoft.Test.Serialization;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;


namespace Avalon.Test.CoreUI.PropertyEngine.OverrideInheritance
{
    /******************************************************************************
    * CLASS:          OverrideInheritanceBehavior
    ******************************************************************************/
    [Test(0, "PropertyEngine.InheritanceBehavior", TestCaseSecurityLevel.FullTrust, "OverrideInheritanceBehavior")]
    public class OverrideInheritanceBehavior : TestCase
    {
        #region Private Data
        private string _testName = "";
        private bool _testPassed = false;
        #endregion

        #region Constructor

        [Variation("SetValuesBefore")]
        [Variation("SetValuesAfter")]
        [Variation("CheckCachedValue")]

        /******************************************************************************
        * Function:          OverrideInheritanceBehavior Constructor
        ******************************************************************************/
        public OverrideInheritanceBehavior(string arg)
        {
            _testName = arg;
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        TestResult StartTest()
        {
            switch (_testName)
            {
                case "SetValuesBefore":
                    RunTest(true, false);   // Case1: Set values before hooking up tree
                    break;
                case "SetValuesAfter":
                    RunTest(false, false);  // Case2: Set Values after hooking up tree
                    break;
                case "CheckCachedValue":
                    RunTest(true, true);    // Case3: Check Cached - value
                    break;
                default:
                    throw new Microsoft.Test.TestValidationException("ERROR!!! Test case not found.");
            }

            if (_testPassed)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        /******************************************************************************
        * Function:          RunTest
        ******************************************************************************/
        public void RunTest(bool setValBeforeTree, bool cacheNode)
        {
            // Create tree elements
            CustomFE1 cf1 = new CustomFE1();
            StackPanel sp1 = new StackPanel();
            Button b1 = new Button();

            if (setValBeforeTree == true)
            {
                cf1.Test = "GP";
                cf1.Test2 = "GP2";
            }

            // Construct the tree.
            cf1.Children.Add(sp1);
            sp1.Children.Add(b1);


            if (setValBeforeTree == false)
            {
                cf1.Test = "GP";
                cf1.Test2 = "GP2";
            }

            if (cacheNode == true)
            {
                // Set a value on the element locally so the parent value is not cached.
                sp1.SetValue(CustomFE1.TestProperty, "P");
            }

            // Read Values
            string testValue = (string)b1.GetValue(CustomFE1.TestProperty);
            string test2Value = (string)b1.GetValue(CustomFE1.Test2Property);

            // Verify Values
            if (test2Value != "GP2")
            {
                throw new Microsoft.Test.TestValidationException("OverridesInheritanceBehavior did not work correctly accross tree boundary. \n" +
                                                    "Expected: GP2  --  Got: " + test2Value);
            }

            if (cacheNode == false && testValue != "Default Value")
            {
                throw new Microsoft.Test.TestValidationException("InheritanceBehavior did not work correctly accross tree boundary. \n" +
                                                    "Expected: Default Value  --  Got: " + testValue);
            }

            if (cacheNode == true && testValue != "P")
            {
                throw new Microsoft.Test.TestValidationException("InheritanceBehavior did not work correctly accross tree boundary with cahing disabled. \n" +
                                                    "Expected: P  --  Got: " + testValue);
            }

            _testPassed = true;;
        }
        #endregion
    }


    /******************************************************************************
    * CLASS:          CustomFE1
    ******************************************************************************/
    /// <summary>
    /// A custom FrameworkElement where the inheitanceBehavior can be modified and set as desired.
    /// </summary>
    public class CustomFE1 : FrameworkElement
    {
        #region CustomFE1 Constructor
        /// <summary>
        /// Constructor for custom FrameworkElement. We can set the inheritanceBehavior property from here.
        /// </summary>
        public CustomFE1()
        {
            this.InheritanceBehavior = InheritanceBehavior.SkipToAppNow;            
        }
        #endregion


        #region CustomFE1 Members
        /// <summary>
        /// The Stretch property determines how the shape may be stretched to accommodate shape size
        /// </summary>
        public UIElementCollection Children
        {
            get
            {
                if (_Children == null)
                {
                    _Children = new UIElementCollection(this, this);
                }

                return _Children;
            }
        }

        private UIElementCollection _Children = null;

        /// <summary>
        /// The Stretch property determines how the shape may be stretched to accommodate shape size
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                // otherwise, its logical children is its visual children                
                return this.Children.GetEnumerator();
            }
        }

        /// <summary>
        /// The Stretch property determines how the shape may be stretched to accommodate shape size
        /// </summary>
        public static readonly DependencyProperty TestProperty
            = DependencyProperty.RegisterAttached(
                "Test",                  // Property name
                typeof(string),            // Property type
                typeof(CustomFE1),
                new FrameworkPropertyMetadata("Default Value", FrameworkPropertyMetadataOptions.Inherits)
            );

        /// <summary>
        /// The Stretch property determines how the shape may be stretched to accommodate shape size
        /// </summary>
        public static readonly DependencyProperty Test2Property
            = DependencyProperty.RegisterAttached(
                "Test2",                  // Property name
                typeof(string),            // Property type
                typeof(CustomFE1),
                new FrameworkPropertyMetadata("Default Value2", FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.OverridesInheritanceBehavior)
            );

        /// <summary>
        /// The Stretch property determines how the shape may be stretched to accommodate shape size
        /// </summary>
        public string Test
        {
            get { return (string)GetValue(TestProperty); }
            set { SetValue(TestProperty, value); }
        }

        /// <summary>
        /// The Stretch property determines how the shape may be stretched to accommodate shape size
        /// </summary>
        public string Test2
        {
            get { return (string)GetValue(Test2Property); }
            set { SetValue(Test2Property, value); }
        }
        #endregion
    }
}
