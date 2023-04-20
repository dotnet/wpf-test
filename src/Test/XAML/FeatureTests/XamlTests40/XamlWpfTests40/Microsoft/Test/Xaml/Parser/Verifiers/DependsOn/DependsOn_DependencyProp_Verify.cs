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
    /// \src\wpf\Test\XAML\FeatureTests\Data\Parser\DependsOn\DependsOn_DependencyProp.xaml
    /// </summary>
    public static class DependsOn_DependencyProp_Verify
    {
        /// <summary>
        /// Verifies DependsOn is respected for DependencyProperties
        /// </summary>
        public static bool Verify(object rootElement)
        {
            bool result = true;
            CustomRootWithCollection root = (CustomRootWithCollection)rootElement;
            Custom_DependsOn targetElement1 = (Custom_DependsOn)root.Content[0];
            Custom_DependsOn targetElement2 = (Custom_DependsOn)root.Content[1];
            Custom_DependsOn targetElement3 = (Custom_DependsOn)root.Content[2];

            if (targetElement1.DependsOnDependencyProp != "ClrProp_25")
            {
                GlobalLog.LogEvidence("targetElement1.DependsOnDependencyProp was not \"ClrProp_25\"");
                GlobalLog.LogEvidence("targetElement1.DependsOnDependencyProp was: " + targetElement1.DependsOnDependencyProp);
                result = false;
            }

            if (targetElement2.DependencyDependsOnStringProperty != "DependencyDependent_OnClrProp")
            {
                GlobalLog.LogEvidence("targetElement2.DependencyDependsOnStringProperty was not \"DependencyDependent_OnClrProp\"");
                GlobalLog.LogEvidence("targetElement2.DependencyDependsOnStringProperty was: " + targetElement2.DependencyDependsOnStringProperty);
                result = false;
            }

            if (targetElement3.DependencyDependsOnDependencyProp != 2525)
            {
                GlobalLog.LogEvidence("targetElement3.DependencyDependsOnDependencyProp was not 2525");
                GlobalLog.LogEvidence("targetElement3.DependencyDependsOnDependencyProp was: " + targetElement3.DependencyDependsOnDependencyProp.ToString());
                result = false;
            }

            return result;
        }
    }
}
