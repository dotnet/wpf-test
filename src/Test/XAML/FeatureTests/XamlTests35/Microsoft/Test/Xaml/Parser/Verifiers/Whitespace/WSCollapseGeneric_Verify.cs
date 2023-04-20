// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Microsoft.Test.Xaml.Utilities;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Whitespace
{
    /// <summary/>
    public static class WSCollapseGeneric_Verify
    {
        /// <summary>
        /// WSCollapseCheck ensures that:
        /// 1. All pure WS strings are at most length `1.
        /// 2. All First and Last Siblings are left trimmed and right trimmed respectively if they contain text.
        /// 3. Siblings on both sides of a TrimSurroundingWhiteSpace-sibling are trimmed on the side adjacent 
        ///    to the TrimSurroundWhiteSpace-sibling
        /// 4. Text-containing siblings in a non-WhitespaceSignificantCollection are trimmed on both ends
        /// 
        /// WSCollapseCheck assumes that it is working with an xmlSpace=default setting. (NOT preserve)
        /// </summary>
        public static bool Verify(object rootObject)
        {
            bool        result                    = true;
            Boolean     sigCollectionAttrib       = XamlTestHelper.ContentPropertyHasAttribute(rootObject, "System.Windows.Markup.WhitespaceSignificantCollectionAttribute");
            IEnumerator logicalChildrenEnumerator = XamlTestHelper.GetIEnumeratorForObject(rootObject);
            if (null != logicalChildrenEnumerator)
            {
                int     count                     = 0;
                String  childString               = null;
                String  prevString                = null;
                char[]  whitespaceTrimChars       = " \t\n".ToCharArray();
                Boolean trimWSAttrib              = false;
                Boolean prevTrimWSAttrib          = false;
                int     paddingForOutputAlignment = 50;

                while (logicalChildrenEnumerator.MoveNext())
                {
                    object child = logicalChildrenEnumerator.Current;
                    prevTrimWSAttrib = trimWSAttrib;
                    trimWSAttrib = XamlTestHelper.HasAttribute(child, "System.Windows.Markup.TrimSurroundingWhitespaceAttribute");
                    prevString = childString;
                    childString = XamlTestHelper.GetStringFromControl(child);

                    if (null != childString)
                    {
                        GlobalLog.LogStatus(child.GetType().ToString().PadRight(paddingForOutputAlignment) + "|" + childString + "|");
                        if (0 == count) // more rigorous test for first sibling
                        {
                            if (!childString.Equals(childString.TrimStart(whitespaceTrimChars)))
                            {
                                GlobalLog.LogEvidence("Too much whitespace at start of first child! :" + childString);
                                result = false;
                            }
                        }
                        XamlTestHelper.EnsureWhitespaceCollapsedMiddleSiblings(childString, prevTrimWSAttrib, sigCollectionAttrib, whitespaceTrimChars, ref result);
                    }

                    if ((trimWSAttrib) && (null != prevString))
                    {
                        if (!prevString.Equals(prevString.TrimEnd(whitespaceTrimChars)))
                        {
                            GlobalLog.LogEvidence("TrimWS Attribute not enforced: " + prevString);
                            result = false;
                        }
                    }
                    result &= Verify(child);
                    count++;
                }
                // more rigorous test for last sibling
                if (null != childString)
                {
                    if (!childString.Equals(childString.TrimEnd(whitespaceTrimChars)))
                    {
                        GlobalLog.LogEvidence("Too much whitespace at end of last child! :" + childString);
                        result = false;
                    }
                }
            }
            return result;
        }
    }
}
