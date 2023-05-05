// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Xaml;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types;
using Microsoft.Test.Xaml.Types.Attributes;

namespace XamlTestsDev10.Microsoft.Test.Xaml.Parser.Verifiers.Ambient
{
    /// <summary>
    /// Verification class for Xaml File
    /// \src\wpf\Test\XAML\FeatureTests\Data\Xaml\Buildable\AmbientSane.xaml
    /// </summary>
    public static class AmbientSame_Verify
    {
        /// <summary>
        /// Regression test
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        /// <returns> bool value </returns>
        public static bool Verify(object rootElement)
        {
            bool result = true;

            var foo = rootElement as TypeWithAmbientProp;
            if (foo.AmbientTC.Text != "test-Hello world")
            {
                GlobalLog.LogStatus("Ambient property value is correct");
            }
            else
            {
                GlobalLog.LogEvidence("AmbientProperty value =" + foo.AmbientTC.Text + "  Expected=test-Hello world");
                result = false;
            }

            return result;
        }
    }
}
