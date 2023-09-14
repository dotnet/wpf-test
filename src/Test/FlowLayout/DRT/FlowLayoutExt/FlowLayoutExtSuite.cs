// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Common functionality for layout test suites.
//
//

using System;
using System.IO;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DRT
{
    // ----------------------------------------------------------------------
    // Common functionality for layout test suites.
    // ----------------------------------------------------------------------
    internal abstract class FlowLayoutExtSuite : LayoutSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        protected FlowLayoutExtSuite(string suiteName) : base(suiteName)
        {
        }

        // ------------------------------------------------------------------
        // Location of all DRT related files.
        // ------------------------------------------------------------------
        protected override string DrtFilesDirectory
        {
            get { return DRT.BaseDirectory + "DrtFiles\\FlowLayoutExt\\"; }
        }
    }
}
