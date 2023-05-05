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
    /// \src\wpf\Test\XAML\FeatureTests\Data\Parser\DependsOn\DependsOn_Circular.xaml
    /// </summary>
    public static class DependsOn_Circular_Verify
    {
        /// <summary>
        /// Verifies DependsOn is respected and the parser doesn't crash in the case of a circular dependency
        /// </summary>
        public static bool Verify(object rootElement)
        {
            bool result = true;
            CustomRootWithCollection root = (CustomRootWithCollection)rootElement;
            Custom_DependsOn_Circular targetElement1 = (Custom_DependsOn_Circular)root.Content[0];

            if (targetElement1.FirstProperty != "Property1_")
            {
                GlobalLog.LogEvidence("targetElement1.FirstProperty was not \"Property1_\"");
                GlobalLog.LogEvidence("targetElement1.FirstProperty was: " + targetElement1.FirstProperty);
                result = false;
            }

            if (targetElement1.SecondProperty != "Property2_Property1_")
            {
                GlobalLog.LogEvidence("targetElement1.SecondProperty was not \"Property2_Property1_\"");
                GlobalLog.LogEvidence("targetElement1.SecondProperty was: " + targetElement1.SecondProperty);
                result = false;
            }

            return result;
        }
    }
}
