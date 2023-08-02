// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: DRT elements specific dumping functions.
//
//

using System;
using System.Xml;
using System.Diagnostics;
using System.Collections;
using System.Windows;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Documents;

namespace DRT
{
    // ----------------------------------------------------------------------
    // DRT elements specific dumping functions.
    // ----------------------------------------------------------------------
    internal sealed class DrtElements
    {
        // ------------------------------------------------------------------
        // Static constructor.
        // ------------------------------------------------------------------
        static DrtElements()
        {
            System.Reflection.Assembly frameworkAssembly = System.Reflection.Assembly.GetAssembly(typeof(System.Windows.FrameworkElement));
            Type layoutDumpType = frameworkAssembly.GetType("MS.Internal.LayoutDump");

            Type typeDelegate = frameworkAssembly.GetType("MS.Internal.LayoutDump+DumpCustomUIElement");
            System.Reflection.MethodInfo miDumpSplitPage = typeof(DrtElements).GetMethod("DumpSplitPage", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            Delegate d = Delegate.CreateDelegate(typeDelegate, miDumpSplitPage);
            System.Reflection.MethodInfo miDumpLayout = layoutDumpType.GetMethod("AddUIElementDumpHandler", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            miDumpLayout.Invoke(null, new object[] { typeof(SplitPage), d });

            s_miDumpDocumentPage = layoutDumpType.GetMethod("DumpDocumentPage", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        }

        // ------------------------------------------------------------------
        // Initialize DRT elements dumpers.
        // ------------------------------------------------------------------
        internal static void Init()
        {
            // All work is done in the constructor, but Init call is required
            // to invoke it.
        }

        // ------------------------------------------------------------------
        // Dump SplitPage specific data.
        // ------------------------------------------------------------------
        private static bool DumpSplitPage(XmlTextWriter writer, UIElement element, bool uiElementsOnly)
        {
            SplitPage splitPage = element as SplitPage;
            Debug.Assert(splitPage != null, "Dump function has to match element type.");

            // Dump document pages
            DocumentPage[] pages = splitPage.GetPages();
            if (pages != null)
            {
                writer.WriteStartElement("Pages");
                writer.WriteAttributeString("Count", pages.Length.ToString(CultureInfo.InvariantCulture));

                for (int i = 0; i < pages.Length; i++)
                {
                    s_miDumpDocumentPage.Invoke(null, new object[] { writer, pages[i], splitPage });
                }

                writer.WriteEndElement();
            }
            return false;
        }

        private static System.Reflection.MethodInfo s_miDumpDocumentPage;
    }
}
