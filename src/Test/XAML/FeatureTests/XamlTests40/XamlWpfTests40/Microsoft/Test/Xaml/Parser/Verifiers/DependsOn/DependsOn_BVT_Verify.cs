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
    /// \src\wpf\Test\XAML\FeatureTests\Data\Parser\DependsOn\DependsOn_BVT.xaml
    /// </summary>
    public static class DependsOn_BVT_Verify
    {
        /// <summary>
        /// Verifies DependsOn is respected regardless of the order of property sets
        /// </summary>
        public static bool Verify(object rootElement)
        {
            bool result = true;
            CustomRootWithCollection root = (CustomRootWithCollection)rootElement;
            Custom_DependsOn targetElement1 = (Custom_DependsOn)root.Content[0];
            Custom_DependsOn targetElement2 = (Custom_DependsOn)root.Content[1];
            Custom_DependsOn targetElement3 = (Custom_DependsOn)root.Content[2];

            if (targetElement1.DependsOnStringProperty != "DependentSetBefore")
            {
                GlobalLog.LogEvidence("targetElement1.DependsOnStringProperty was not \"DependentSetBefore\"");
                GlobalLog.LogEvidence("targetElement1.DependsOnStringProperty was: " + targetElement1.DependsOnStringProperty);
                result = false;
            }

            if (targetElement2.DependsOnStringProperty != "DependentSetAfter")
            {
                GlobalLog.LogEvidence("targetElement2.DependsOnStringProperty was not \"DependentSetAfter\"");
                GlobalLog.LogEvidence("targetElement2.DependsOnStringProperty was: " + targetElement2.DependsOnStringProperty);
                result = false;
            }

            if (targetElement3.DependsOnStringProperty != "DependentOnly")
            {
                GlobalLog.LogEvidence("targetElement3.DependsOnStringProperty was not \"DependentOnly\"");
                GlobalLog.LogEvidence("targetElement3.DependsOnStringProperty was: " + targetElement3.DependsOnStringProperty);
                result = false;
            }

            return result;
        }
    }
}
