// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Xaml.CustomTypes;
using Microsoft.Test.Xaml.CustomTypes.Attributes;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.Verifiers.DependsOn
{
    /// <summary>
    /// Verification class for Xaml File
    /// \src\wpf\Test\XAML\FeatureTests\Data\Parser\DependsOn\DependsOn_Nested.xaml
    /// </summary>
    public static class DependsOn_Nested_Verify
    {
        /// <summary>
        /// Verifies DependsOn is respected for properties that depend on properties with their own dependencies
        /// </summary>
        public static bool Verify(object rootElement)
        {
            bool result = true;
            CustomRootWithCollection root = (CustomRootWithCollection)rootElement;
            Custom_DependsOn targetElement1 = (Custom_DependsOn)root.Content[0];
            Custom_DependsOn targetElement2 = (Custom_DependsOn)root.Content[1];
            Custom_DependsOn targetElement3 = (Custom_DependsOn)root.Content[2];
            Custom_DependsOn targetElement4 = (Custom_DependsOn)root.Content[3];
            Custom_DependsOn targetElement5 = (Custom_DependsOn)root.Content[4];

            if (targetElement1.DependsOnDependsOnStringProperty != "Nested_DependentSetFirst")
            {
                GlobalLog.LogEvidence("targetElement1.DependsOnDependsOnStringProperty was not \"Nested_DependentSetFirst\"");
                GlobalLog.LogEvidence("targetElement1.DependsOnDependsOnStringProperty was: " + targetElement1.DependsOnDependsOnStringProperty);
                result = false;
            }

            if (targetElement2.DependsOnDependsOnStringProperty != "Nested_DependentSetMiddle")
            {
                GlobalLog.LogEvidence("targetElement2.DependsOnDependsOnStringProperty was not \"Nested_DependentSetMiddle\"");
                GlobalLog.LogEvidence("targetElement2.DependsOnDependsOnStringProperty was: " + targetElement2.DependsOnDependsOnStringProperty);
                result = false;
            }

            if (targetElement3.DependsOnDependsOnStringProperty != "Nested_DependentSetLast")
            {
                GlobalLog.LogEvidence("targetElement3.DependsOnDependsOnStringProperty was not \"Nested_DependentSetLast\"");
                GlobalLog.LogEvidence("targetElement3.DependsOnDependsOnStringProperty was: " + targetElement3.DependsOnDependsOnStringProperty);
                result = false;
            }

            if (targetElement4.DependsOnDependsOnStringProperty != "NestedDependent_MissingDependency")
            {
                GlobalLog.LogEvidence("targetElement4.DependsOnDependsOnStringProperty was not \"NestedDependent_MissingDependency\"");
                GlobalLog.LogEvidence("targetElement4.DependsOnDependsOnStringProperty was: " + targetElement4.DependsOnDependsOnStringProperty);
                result = false;
            }

            if (targetElement5.DependsOnDependsOnStringProperty != "NestedDependentOnly_")
            {
                GlobalLog.LogEvidence("targetElement5.DependsOnDependsOnStringProperty was not \"NestedDependentOnly_\"");
                GlobalLog.LogEvidence("targetElement5.DependsOnDependsOnStringProperty was: " + targetElement5.DependsOnDependsOnStringProperty);
                result = false;
            }

            return result;
        }
    }
}
