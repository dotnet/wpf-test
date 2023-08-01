// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.PropertyEngine;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.UtilityHelper;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Avalon.Test.CoreUI.PropertyEngine.RefreshFrameworkElementTest
{
    [Test(0, "PropertyEngine.FrameworkElement", TestCaseSecurityLevel.FullTrust, "FrameworkElementTest")]
    public class FrameworkElementTest : TestCase
    {
        #region Constructor
        /******************************************************************************
        * Function:          FrameworkElementTest Constructor
        ******************************************************************************/
        public FrameworkElementTest()
        {
            RunSteps += new TestStep(TestMisc);
            RunSteps += new TestStep(TestInheritanceBehavior);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          TestMisc
        ******************************************************************************/
        /// <summary>
        /// Type Check
        /// </summary>
        TestResult TestMisc()
        {
            Utilities.PrintTitle("FrameworkElement Type Check");
            Type typeFrameworkElement = typeof(FrameworkElement);
            Utilities.Assert(typeFrameworkElement.BaseType == typeof(UIElement), "Base class is UIElement");
            Utilities.Assert(typeof(IFrameworkInputElement).IsAssignableFrom(typeFrameworkElement), "Implements IFrameworkInputElement");
            Utilities.Assert(typeof(System.ComponentModel.ISupportInitialize).IsAssignableFrom(typeFrameworkElement), "Implements ISupportInitalize");

            //Any test failures will be caught by Utilities.Assert.
            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          TestInheritanceBehavior
        ******************************************************************************/
        /// <summary>
        /// Test InheritanceBehavior Property
        /// </summary>
        /// <value></value>
        TestResult TestInheritanceBehavior()
        {
            Utilities.PrintTitle("Test InheritanceBehavior Property");
            InheritanceBehaviorFrameworkElement basicElement = new InheritanceBehaviorFrameworkElement();
            Utilities.Assert(basicElement.InheritanceBehavior == InheritanceBehavior.Default, "Default is InheritanceBehavior.Default");
            Utilities.PrintStatus("Set to True");
            basicElement.InheritanceBehavior = InheritanceBehavior.SkipToAppNow;
            Utilities.Assert(basicElement.InheritanceBehavior == InheritanceBehavior.SkipToAppNow, "Now it is set to InheritanceBehavior.SkipToAppNow");
            Utilities.PrintStatus("Set to False");
            basicElement.InheritanceBehavior = InheritanceBehavior.Default;
            Utilities.Assert(basicElement.InheritanceBehavior == InheritanceBehavior.Default, "Now it is set to InheritanceBehavior.Default");
            StackPanel panel = new StackPanel();
            panel.Children.Add(basicElement);
            Utilities.PrintStatus("After having a child, changing InheritanceBehavior is invalid operation.");
            try
            {
                basicElement.InheritanceBehavior = InheritanceBehavior.Default;
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (InvalidOperationException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
            Utilities.PrintStatus("We can still Get");
            Utilities.Assert(basicElement.InheritanceBehavior == InheritanceBehavior.Default, "We can still call get");

            //Any test failures will be caught by Utilities.Assert.
            return TestResult.Pass;
        }
        #endregion
    }
}

