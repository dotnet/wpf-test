// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Xaml.Types;
using Microsoft.Test.Logging;
using System.Windows;

namespace Microsoft.Test.Xaml.Parser.Verifiers.XmlContent
{
    /// <summary/>
    public static class XmlContent_Verify
    {
        /// <summary>
        /// Verification routine for XmlContent.xaml.
        /// See comments in XmlContent.xaml to learn what this is about.
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool              result             = true;
            XmlContentControl xmlcontentcontrol0 = (XmlContentControl) LogicalTreeHelper.FindLogicalNode(rootElement, "XmlContentControl0");
            string            content            = xmlcontentcontrol0.Content.ToString();
            string            expectedContent    = "<Foo xmlns=\"\">\n      <Bar />\n    </Foo>";
            if (content != expectedContent)
            {
                GlobalLog.LogEvidence(@" Expected value of XmlContentControl0.Content.ToString(): " + expectedContent +
                    "\r\n  Actual value: " + content);
                result = false;
            }
            return result;
        }
    }
}
