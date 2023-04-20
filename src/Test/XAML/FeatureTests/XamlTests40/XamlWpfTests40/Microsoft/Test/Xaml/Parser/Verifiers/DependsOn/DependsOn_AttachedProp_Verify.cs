// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Xaml.CustomTypes.Attributes;
using Microsoft.Test.Xaml.CustomTypes;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.Verifiers.DependsOn
{
    /// <summary>
    /// Verification class for Xaml File
    /// \src\wpf\Test\XAML\FeatureTests\Data\Parser\DependsOn\DependsOn_AttachedProp.xaml
    /// </summary>
    public static class DependsOn_AttachedProp_Verify
    {
        /// <summary>
        /// Verifies DependsOn is respected by attached properties
        /// </summary>
        public static bool Verify(object rootElement)
        {
            bool result = true;
            CustomRootWithCollection root = (CustomRootWithCollection)rootElement;
            Custom_DependsOn targetElement1 = (Custom_DependsOn)root.Content[0];

            if (Custom_DependsOn.GetAttachedDependsOnAttachedProperty(targetElement1) != "AttachedProp_25")
            {
                GlobalLog.LogEvidence("Custom_DependsOn.GetAttachedDependsOnAttachedProperty(targetElement1) was not \"AttachedProp_25\"");
                GlobalLog.LogEvidence("Custom_DependsOn.GetAttachedDependsOnAttachedProperty(targetElement1) was: " + Custom_DependsOn.GetAttachedDependsOnAttachedProperty(targetElement1));
                result = false;
            }            

            return result;
        }
    }
}
