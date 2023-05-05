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
    /// \src\wpf\Test\XAML\FeatureTests\Data\Parser\DependsOn\DependsOn_Directive.xaml
    /// </summary>
    public static class DependsOn_Directive_Verify
    {
        /// <summary>
        /// Verifies DependsOn is respected except when a property is set via a directive
        /// </summary>
        public static bool Verify(object rootElement)
        {
            bool result = true;
            CustomRootWithCollection root = (CustomRootWithCollection)rootElement;
            Custom_DependsOn targetElement1 = (Custom_DependsOn)root.Content[0];
            Custom_DependsOn_DependentName targetElement2 = (Custom_DependsOn_DependentName)root.Content[1];

            if (targetElement1.DependsOnNameProperty != "DependentOn_NameDirective")
            {
                GlobalLog.LogEvidence("targetElement1.DependsOnNameProperty was not \"DependentOnNameDirective\"");
                GlobalLog.LogEvidence("targetElement1.DependsOnNameProperty was: " + targetElement1.DependsOnNameProperty);
                result = false;
            }

            //in this case we expect the DependsOnAttribute to be ignored, since the name property is being set via x:Name
            if (targetElement2.Name != "NameDirectiveDependent_")
            {
                GlobalLog.LogEvidence("targetElement2.Name was not \"NameDirectiveDependent_\"");
                GlobalLog.LogEvidence("targetElement2.Name was: " + targetElement2.Name);
                result = false;
            }

            return result;
        }
    }
}
