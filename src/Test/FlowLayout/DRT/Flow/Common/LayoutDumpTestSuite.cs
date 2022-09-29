// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Common functionality for layout dump test suites.
//

using System;                   // string
using System.IO;                // Stream
using System.Xml;               // XmlTextWriter
using System.Reflection;        // MethodInfo
using System.Windows;           // UIElement

namespace DRT
{
    /// <summary>
    /// Common functionality for layout dump test suites.
    /// </summary>
    internal abstract class LayoutDumpTestSuite : DumpTestSuite
    {
        /// <summary>
        /// Static constructor.
        /// </summary>
        static LayoutDumpTestSuite()
        {
            Type typeLayoutDump = DrtFlowBase.FrameworkAssembly.GetType("MS.Internal.LayoutDump");
            _miDumpLayout = typeLayoutDump.GetMethod("DumpLayoutTree", BindingFlags.Static | BindingFlags.NonPublic);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="suiteName">Name of the suite.</param>
        protected LayoutDumpTestSuite(string suiteName) : 
            base(suiteName)
        {
        }

        /// <summary>
        /// Dump content to XmlTextWriter.
        /// </summary>
        protected override void DumpContent(XmlTextWriter writer)
        {
            _miDumpLayout.Invoke(null, new object[] { writer, "LayoutDump", this.Root.Child });
        }

        /// <summary>
        /// Method info for dump layout function.
        /// </summary>
        private static MethodInfo _miDumpLayout;
    }
}
