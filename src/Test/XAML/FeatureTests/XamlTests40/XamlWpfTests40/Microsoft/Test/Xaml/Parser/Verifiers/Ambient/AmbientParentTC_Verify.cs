// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types.Attributes;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Ambient
{
    /// <summary>
    /// Verification class for Xaml File
    /// \src\wpf\Test\XAML\FeatureTests\Data\Parser\Ambient\AmbientParentTC.xaml
    /// </summary>
    public static class AmbientParentTC_Verify
    {
        /// <summary>
        /// Method verifies that the Text properties are set to the same value by the TypeConverter
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        /// <returns> bool value </returns>
        public static bool Verify(object rootElement)
        {
            bool result = true;

            Grid grid = (Grid) rootElement;
            ListBox lb = (ListBox) grid.Children[0];
            AmbientWithContent element1 = (AmbientWithContent) lb.Items[0];
            NonAmbientWithAmbientTCProp element2 = (NonAmbientWithAmbientTCProp) element1.Content;

            if (element2.ATC.Text == "TCText-" + element1.Text)
            {
                GlobalLog.LogStatus("Strings matched");
            }
            else
            {
                GlobalLog.LogEvidence("Strings did not match.  Child Text=" + element2.ATC.Text + "  Parent Text=" + element1.Text);
                result = false;
            }

            return result;
        }
    }
}
