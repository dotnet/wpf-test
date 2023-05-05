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
    /// \src\wpf\Test\XAML\FeatureTests\Data\Parser\Ambient\AmbientSiblingME.xaml
    /// </summary>
    public static class AmbientSiblingME_Verify
    {
        /// <summary>
        /// Method verifies that the Text and Name properties are set to the same value
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        /// <returns> bool value </returns>
        public static bool Verify(object rootElement)
        {
            bool result = true;

            Grid grid = (Grid) rootElement;
            ListBox lb = (ListBox) grid.Children[0];
            AmbientMultiProp amp = (AmbientMultiProp) lb.Items[0];
            if (amp.Text == amp.Name)
            {
                GlobalLog.LogStatus("Strings matched");
            }
            else
            {
                GlobalLog.LogEvidence("Strings did not match.  Text=" + amp.Text + "  Name=" + amp.Name);
                result = false;
            }

            return result;
        }
    }
}
