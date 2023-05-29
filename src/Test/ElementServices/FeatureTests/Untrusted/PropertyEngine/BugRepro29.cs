// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows;
using Avalon.Test.CoreUI;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Microsoft.Test.ElementServices.Untrusted.PropertyEngine
{
    /// <summary>
    /// Regression test for 

    [TestDefaults]
    public class BugRepro29 : TestCase
    {
        /// <summary>
        /// Regression test for 

        [Test(1, @"PropertyEngine\RegressionTests", TestCaseSecurityLevel.PartialTrust, "BugRepro29", MethodName="TestBugRepro29")]    
        public void TestBugRepro29()
        {
            DependencyPropertyDescriptor dependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(AttachedPropertyClass.AttachedProperty, typeof(AttachedPropertyClass));
            if (dependencyPropertyDescriptor == null)
            {
                throw new TestValidationException("dependencyPropertyDescriptor is null");
            }

            DescriptionAttribute descriptionAttribute = new DescriptionAttribute("AttributedClass");
            if (!dependencyPropertyDescriptor.Attributes.Contains(descriptionAttribute))
            {
                GlobalLog.LogEvidence("descriptionAttribute is not found");
                TestLog.Current.Result = TestResult.Fail;
            }
            else
            {
                TestLog.Current.Result = TestResult.Pass;
            }
        }
    }

    [Description("AttributedClass")]
    class AttributedClass
    {
        public AttributedClass()
        {
            //do nothing
        }
    }

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
