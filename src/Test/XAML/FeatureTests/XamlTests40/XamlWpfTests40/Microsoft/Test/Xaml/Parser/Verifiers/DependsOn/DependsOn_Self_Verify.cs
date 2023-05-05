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
    /// \src\wpf\Test\XAML\FeatureTests\Data\Parser\DependsOn\DependsOn_Self.xaml
    /// </summary>
    public static class DependsOn_Self_Verify
    {
        /// <summary>
        /// Verifies that the parser does not crash when encountering a self-dependency
        /// </summary>
        public static bool Verify(object rootElement)
        {
            bool result = true;
            CustomRootWithCollection root = (CustomRootWithCollection)rootElement;
            Custom_DependsOn_Self targetElement1 = (Custom_DependsOn_Self)root.Content[0];

            if (targetElement1.StringProperty != "_StringPropertySelfDependency")
            {
                GlobalLog.LogEvidence("targetElement1.StringProperty was not \"_StringPropertySelfDependency\"");
                GlobalLog.LogEvidence("targetElement1.StringProperty was: " + targetElement1.StringProperty);
                result = false;
            }
            return result;
        }
    }
}
