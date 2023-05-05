// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types;

namespace Microsoft.Test.Xaml.Parser.Verifiers.MarkupExt
{
    /// <summary>
    /// Verifier class for Xaml file:
    /// \src\wpf\Test\XAML\FeatureTests\Data\Parser\MarkupExt\IRME_MissingIRME.xaml
    /// </summary>
    public sealed class IRME_MissingIRME_Verify
    {
        /// <summary>
        /// Verifies the specified root element.
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        /// <returns>bool value</returns>
        public static bool Verify(object rootElement)
        {
            bool result = true;

            CustomRoot root = (CustomRoot) rootElement;
            BoxNoIRME box1 = (BoxNoIRME) root.Content;
            if (box1.Footprint != 30)
            {
                GlobalLog.LogEvidence("Footprint was: " + box1.Footprint + "Expected: 30");
                result = false;
            }

            if (box1.Volume != 60)
            {
                GlobalLog.LogEvidence("Volume was: " + box1.Volume + "Expected: 60");
                result = false;
            }

            return result;
        }
    }
}
