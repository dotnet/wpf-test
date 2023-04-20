// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Microsoft.Test.Xaml.Utilities;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Document
{
    /// <summary/>
    public static class RegressionIssue4_Verify
    {
        /// <summary>
        /// RegressionIssue4_Verify
        /// </summary>
        /// <param name="rootElement"></param>
        /// <returns></returns>
        public static bool Verify(UIElement rootElement)
        {
            bool             result = true;
            DependencyObject root   = (DependencyObject) TreeHelper.FindLogicalRoot(rootElement);

            _VerifyLogicalRoot(root, rootElement, "MyDocumentViewer", ref result);
            _VerifyLogicalRoot(root, rootElement, "MyFixedDocument", ref result);
            _VerifyLogicalRoot(root, rootElement, "MyPageContent", ref result);
            _VerifyLogicalRoot(root, rootElement, "MyFixedPage", ref result);
            _VerifyLogicalRoot(root, rootElement, "MyTextBlock", ref result);

            return result;
        }

        private static void _VerifyLogicalRoot(DependencyObject root, DependencyObject subTreeRoot, string id, ref bool result)
        {
            object obj = TreeHelper.FindNodeById(subTreeRoot, id);

            if (root != TreeHelper.FindLogicalRoot((DependencyObject) obj))
            {
                GlobalLog.LogEvidence("The logical root is not correct.");
                result &= false;
            }
        }
    }
}
