// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections;
using System.Resources;
using System.Globalization;

namespace DRT
{

    public class DefSharedSuite : DrtTestSuite
    {
        public DefSharedSuite() : base("DelayLoaded")
        {
            Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            System.Uri resourceLocator = new System.Uri(@"bof.xaml", System.UriKind.RelativeOrAbsolute);
            Visual bofRoot = System.Windows.Application.LoadComponent(resourceLocator) as Visual;
            DRT.Show(bofRoot);

            // Tests

            if (!DRT.KeepAlive)
            {
                return new DrtTest[] {
                    new DrtTest(RunTest),
                };
            }
            else
            {
                return new DrtTest[] {};
            }
        }


        private void RunTest()
        {
            FrameworkElement root = DRT.FindVisualByID("RootBorder") as FrameworkElement;
            foreach (DictionaryEntry entry in root.Resources)
            {
                DRT.Assert(entry.Value is ContextMenu, "ContextMenu in Resources not inflated.");
            }
            foreach (object val in root.Resources.Values)
            {
                DRT.Assert(val is ContextMenu, "ContextMenu in Resources.Values not inflated.");
            }

            Control shared1 = DRT.FindVisualByID("shared1") as Control;
            Control shared2 = DRT.FindVisualByID("shared2") as Control;
            Control notShared1 = DRT.FindVisualByID("notShared1") as Control;
            Control notShared2 = DRT.FindVisualByID("notShared2") as Control;
            Control default1 = DRT.FindVisualByID("default1") as Control;
            Control default2 = DRT.FindVisualByID("default2") as Control;
            ContextMenu cm1;
            ContextMenu cm2;

            cm1 = shared1.ContextMenu;
            cm2 = shared2.ContextMenu;

            DRT.Assert(Object.ReferenceEquals(cm1, cm2), "Resource reference to a shared ContextMenu did not yield the same object reference (should be the same).");

            cm1 = notShared1.ContextMenu;
            cm2 = notShared2.ContextMenu;

            DRT.Assert(!Object.ReferenceEquals(cm1, cm2), "Resource reference to a non-shared ContextMenu yielded the same object reference (should be different).");

            cm1 = default1.ContextMenu;
            cm2 = default2.ContextMenu;

            DRT.Assert(Object.ReferenceEquals(cm1, cm2), "Resource reference to a default-shared ContextMenu should yield the same object reference");
        }
    }
}
