// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Styles.BasedOn
{
    /// <summary/>
    public static class SerializeBasedonStyleNoTargetType_Verify
    {
        /// <summary>
        /// Verifier for SerializeBasedonStyleNoTargetType.xaml
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;
            SerializeBasedonStyleFE_Verify._noTargetType = true;
            SerializeBasedonStyleFCE_Verify._noTargetType = true;
            result &= SerializeBasedonStyleFE_Verify.Verify(rootElement);
            result &= SerializeBasedonStyleFCE_Verify.Verify(rootElement);
            return result;
        }
    }
}
