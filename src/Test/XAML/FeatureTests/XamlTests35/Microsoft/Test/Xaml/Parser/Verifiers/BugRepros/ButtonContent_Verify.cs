// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.Verifiers.BugRepros
{
    /// <summary/>
    public static class ButtonContent_Verify
    {
        /// <summary>
        ///  Verify that Button content does not include a '\' character
        /// </summary>
        public static bool Verify(object rootElement)
        {
            bool result = true;

            Button root = (Button)rootElement;

            if ((string)root.Content != " Hello")
            {
                GlobalLog.LogEvidence("Button's content was not the string Hello");
                result = false;
            }
            return result;
        }
    }
}
