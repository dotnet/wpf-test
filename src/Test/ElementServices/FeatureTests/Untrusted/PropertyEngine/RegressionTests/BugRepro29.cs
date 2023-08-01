// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows;

using Avalon.Test.CoreUI;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Avalon.Test.CoreUI.PropertyEngine.RegressionTests
{
    /******************************************************************************
    * CLASS:          DependencyObjectProviderTests
    ******************************************************************************/
    /// <summary>
    /// Regression test for 

    [Test(0, "PropertyEngine.RegressionTests", TestCaseSecurityLevel.PartialTrust, "BugRepro29")]
    public class BugRepro29 : AvalonTest
    {
        #region Constructor
        /******************************************************************************
        * Function:          BugRepro29 Constructor
        ******************************************************************************/
        public BugRepro29()
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        TestResult StartTest()
        {
            DependencyPropertyDescriptor dependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(AttachedPropertyClass.AttachedProperty, typeof(AttachedPropertyClass));
            if (dependencyPropertyDescriptor == null)
            {
                throw new TestValidationException("FAIL: DependencyPropertyDescriptor is null");
            }

            DescriptionAttribute descriptionAttribute = new DescriptionAttribute("AttributedClass");
            if (!dependencyPropertyDescriptor.Attributes.Contains(descriptionAttribute))
            {
                GlobalLog.LogEvidence("FAIL: DescriptionAttribute is not found");
                return TestResult.Fail;
            }
            else
            {
                return TestResult.Pass;
            }
        }
        #endregion
    }


    /******************************************************************************
    * CLASS:          AttributedClass
    ******************************************************************************/
    [Description("AttributedClass")]
    class AttributedClass
    {
        public AttributedClass()
        {
            //do nothing
        }
    }

    /******************************************************************************
    * CLASS:          AttachedPropertyClass
    ******************************************************************************/
    class AttachedPropertyClass : DependencyObject
    {
        public static readonly DependencyProperty AttachedProperty = DependencyProperty.RegisterAttached("Attached", typeof(AttributedClass), typeof(AttachedPropertyClass));

        public static AttributedClass GetAttached(DependencyObject element)
        {
            return (AttributedClass)element.GetValue(AttachedPropertyClass.AttachedProperty);
        }

        public static void SetAttached(DependencyObject element, AttributedClass value)
        {
            element.SetValue(AttachedPropertyClass.AttachedProperty, value);
        }
    }
}
