// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Xaml.Types;
using System.Windows;
using Microsoft.Test.Logging;
using System.Windows.Documents;

namespace Microsoft.Test.Xaml.Parser.Verifiers.ContentWrapperAttribute
{
    /// <summary/>
    public static class ContentWrapper_Verify
    {
        /// <summary>
        /// Verifier for correct parsing/serialization of ContentWrapperAttribute.
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;

            Custom_DO_With_GenericCollection_Properties customElement = LogicalTreeHelper.FindLogicalNode(rootElement, "customElement") as Custom_DO_With_GenericCollection_Properties;

            if (customElement == null)
            {
                GlobalLog.LogStatus("Could not find custom element.");
                result = false;
            }

            if (customElement.CustomGenericCollection.Count != 1)
            {
                GlobalLog.LogStatus("Custom element does not contain exactly 1 child.");
                result = false;
            }

            if (!(customElement.CustomGenericCollection[0] is Run))
            {
                GlobalLog.LogStatus("Custom element does not contain a Run child.");
                result = false;
            }

            return result;
        }
    }
}
