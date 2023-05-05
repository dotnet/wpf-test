// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types;

namespace Microsoft.Test.Xaml.Parser.Verifiers.MarkupExt
{
    /// <summary>
    /// Verifier class for Xaml file:
    /// \src\wpf\Test\XAML\FeatureTests\Data\Parser\MarkupExt\IRME_Simple.xaml
    /// </summary>
    public sealed class IRME_Simple_Verify
    {
        /// <summary>
        /// Method verifies the ReceiveMarkupExtension() was called by ObjectWriter
        /// The expected values of 200 and 6000 can only be returned by ReceiveMarkupExtension()
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        /// <returns>bool value</returns>
        public static bool Verify(object rootElement)
        {
            bool result = true;

            CustomRoot root = (CustomRoot) rootElement;
            Box box1 = (Box) root.Content;
            if (box1.Footprint != 200)
            {
                GlobalLog.LogEvidence("Footprint was: " + box1.Footprint + "Expected: 200");
                result = false;
            }

            if (box1.Volume != 6000)
            {
                GlobalLog.LogEvidence("Volume was: " + box1.Volume + "Expected: 6000");
                result = false;
            }

            return result;
        }
    }
}
