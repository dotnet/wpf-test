// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types;
using Microsoft.Test.Xaml.Types.Repro;

namespace Microsoft.Test.Xaml.Parser.Verifiers.BugRepros
{
    public static class RegressionIssue8Repro_Verify
    {
        public static bool Verify(object rootElement)
        {
            bool result = true;

            CustomRootWithCollection root = (CustomRootWithCollection) rootElement;

            InheritedContentType1 element1 = (InheritedContentType1) root.Content[0];
            InheritedContentType2 element2 = (InheritedContentType2) root.Content[1];
            InheritedContentType3 element3 = (InheritedContentType3) root.Content[2];

            if (!(element1.Content is Custom_Clr))
            {
                GlobalLog.LogEvidence("Element1's content was not a Custom_Clr");
                result = false;
            }

            if (!(element2.Content is Custom_Clr))
            {
                GlobalLog.LogEvidence("Element2's content was not a Custom_Clr");
                result = false;
            }

            if (element3.Content != "Text")
            {
                GlobalLog.LogEvidence("Element3's content was not the string Text");
                result = false;
            }

            return result;
        }
    }
}
