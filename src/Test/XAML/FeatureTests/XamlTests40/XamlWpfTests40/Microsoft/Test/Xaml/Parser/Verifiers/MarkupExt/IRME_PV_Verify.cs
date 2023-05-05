// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types;

namespace Microsoft.Test.Xaml.Parser.Verifiers.MarkupExt
{
    /// <summary>
    /// Base verifier class for IRME case where ProvideValue is called
    /// </summary>
    public sealed class IRME_PV_Verify
    {
        /// <summary>
        /// Method verifier that ProvideValue() was called by ObjectWriter.
        /// The expected values of 30 and 60 can only be returned by ProvideValue()
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        /// <returns>bool value</returns>
        public static bool Verify(object rootElement)
        {
            bool result = true;

            CustomRoot root = (CustomRoot) rootElement;
            Box box1 = (Box) root.Content;
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
