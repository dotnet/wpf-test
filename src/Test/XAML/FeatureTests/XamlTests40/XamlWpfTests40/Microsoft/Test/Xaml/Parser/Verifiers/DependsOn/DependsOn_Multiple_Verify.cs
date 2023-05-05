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
    /// \src\wpf\Test\XAML\FeatureTests\Data\Parser\DependsOn\DependsOn_Multiple.xaml
    /// </summary>
    public static class DependsOn_Multiple_Verify
    {
        /// <summary>
        /// Verifies DependsOn is respected for properties that depend on multiple properties
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

            if (targetElement1.DependsOnMultipleProperties != "Multiple_Dependent1_SetFirst")
            {
                GlobalLog.LogEvidence("targetElement1.DependsOnMultipleProperties was not \"Multiple_Dependent1_SetFirst\"");
                GlobalLog.LogEvidence("targetElement1.DependsOnMultipleProperties was: " + targetElement1.DependsOnMultipleProperties);
                result = false;
            }

            if (targetElement2.DependsOnMultipleProperties != "Multiple_Dependent2_SetMiddle")
            {
                GlobalLog.LogEvidence("targetElement2.DependsOnMultipleProperties was not \"Multiple_Dependent2_SetMiddle\"");
                GlobalLog.LogEvidence("targetElement2.DependsOnMultipleProperties was: " + targetElement2.DependsOnMultipleProperties);
                result = false;
            }

            if (targetElement3.DependsOnMultipleProperties != "Multiple_Dependent3_SetLast")
            {
                GlobalLog.LogEvidence("targetElement3.DependsOnMultipleProperties was not \"Multiple_Dependent3_SetLast\"");
                GlobalLog.LogEvidence("targetElement3.DependsOnMultipleProperties was: " + targetElement3.DependsOnMultipleProperties);
                result = false;
            }

            if (targetElement4.DependsOnMultipleProperties != "MultipleDependent_MissingDependency")
            {
                GlobalLog.LogEvidence("targetElement4.DependsOnMultipleProperties was not \"MultipleDependent_MissingDependency\"");
                GlobalLog.LogEvidence("targetElement4.DependsOnMultipleProperties was: " + targetElement4.DependsOnMultipleProperties);
                result = false;
            }

            if (targetElement5.DependsOnMultipleProperties != "MultipleDependentOnly__")
            {
                GlobalLog.LogEvidence("targetElement5.DependsOnMultipleProperties was not \"MultipleDependentOnly__\"");
                GlobalLog.LogEvidence("targetElement5.DependsOnMultipleProperties was: " + targetElement5.DependsOnMultipleProperties);
                result = false;
            }

            return result;
        }
    }
}
