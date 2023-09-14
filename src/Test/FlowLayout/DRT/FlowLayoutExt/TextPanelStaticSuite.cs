// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Test suite for TextFlow with static content. 
//
//

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DRT
{
    // ----------------------------------------------------------------------
    // Test suite for TextFlow with static content.
    // ----------------------------------------------------------------------
    internal sealed class TextPanelStaticSuite : FlowLayoutExtSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal TextPanelStaticSuite() : base("TextPanel")
        {
            this.Contact = "Microsoft";
        }

        // ------------------------------------------------------------------
        // Initialize tests.
        // ------------------------------------------------------------------
        protected override DrtTest[] CreateTests()
        {
            // Return the lists of tests to run against the tree
            return new DrtTest[] {
                new DrtTest(LoadClear),             new DrtTest(VerifyLayout),
                //new DrtTest(LoadFiguresWrap),       new DrtTest(VerifyLayout),
                new DrtTest(LoadFloaters),          new DrtTest(VerifyLayout),
                //new DrtTest(LoadFloatersWrap),      new DrtTest(VerifyLayout),
                //new DrtTest(LoadTightWrapBasic),    new DrtTest(VerifyLayout),
                //new DrtTest(LoadTightWrapRot45Pos), new DrtTest(VerifyLayout),
                //new DrtTest(LoadTightWrapRot45Neg), new DrtTest(VerifyLayout),
                //new DrtTest(LoadTightWrapFigures),  new DrtTest(VerifyLayout),
                //new DrtTest(LoadFlowDocumentNesting),new DrtTest(VerifyLayout),
                new DrtTest(LoadFloaterWidthExceedsAvailableWidth),          new DrtTest(VerifyLayout),
                
            };
        }

        // ------------------------------------------------------------------
        // Load TextPanelClear.xaml
        // ------------------------------------------------------------------
        private void LoadClear()
        {
            _testName = "Clear";
            LoadContentFromXaml();
        }

        // ------------------------------------------------------------------
        // Load TextPanelFiguresWrap.xaml
        // ------------------------------------------------------------------
        private void LoadFiguresWrap()
        {
            _testName = "FiguresWrap";
            LoadContentFromXaml();
        }

        // ------------------------------------------------------------------
        // Load TextPanelFloaters.xaml
        // ------------------------------------------------------------------
        private void LoadFloaters()
        {
            _testName = "Floaters";
            LoadContentFromXaml();
        }

        // ------------------------------------------------------------------
        // Load TextPanelFloatersWrap.xaml
        // ------------------------------------------------------------------
        private void LoadFloatersWrap()
        {
            _testName = "FloatersWrap";
            LoadContentFromXaml();
        }

        // ------------------------------------------------------------------
        // Load TextPanelTightWrapBasic.xaml
        // ------------------------------------------------------------------
        private void LoadTightWrapBasic()
        {
            _testName = "TightWrapBasic";
            LoadContentFromXaml();
        }

        // ------------------------------------------------------------------
        // Load TextPanelTightWrapRotation45Pos.xaml
        // ------------------------------------------------------------------
        private void LoadTightWrapRot45Pos()
        {
            _testName = "TightWrapRotation45Pos";
            LoadContentFromXaml();
        }

        // ------------------------------------------------------------------
        // Load TightWrapRotation45Neg.xaml
        // ------------------------------------------------------------------
        private void LoadTightWrapRot45Neg()
        {
            _testName = "TightWrapRotation45Neg";
            LoadContentFromXaml();
        }

        // ------------------------------------------------------------------
        // Load TightWrapFigures.xaml
        // ------------------------------------------------------------------
        private void LoadTightWrapFigures()
        {
            _testName = "TightWrapFigures";
            LoadContentFromXaml();
        }

        // ------------------------------------------------------------------
        // Load FlowDocumentNesting.xaml
        // ------------------------------------------------------------------
        private void LoadFlowDocumentNesting()
        {
            _testName = "FlowDocumentNesting";
            //LoadContentFromXaml();
            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            fdsv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            fdsv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            fdsv.Document = new FlowDocument();
            fdsv.Document.TextAlignment = TextAlignment.Left;
            fdsv.Document.PagePadding = new Thickness(0);
            _contentRoot.Child = fdsv;
            fdsv.Document.ContentEnd.InsertTextInRun("Text paragraph [part 1].");
            FlowDocument flowDocument = new FlowDocument();
            ((System.Windows.Markup.IAddChild)fdsv.Document).AddChild(flowDocument);
            flowDocument.ContentEnd.InsertTextInRun("The first step in writing Longhorn Applications  is to equip your computer with a recent version of  the platform, ");
            flowDocument.ContentEnd.InsertTextInRun("and then install the necessary development tools. This document takes you through the necessary procedures.");
            flowDocument.ContentEnd.InsertTextInRun("The Windows Client Platform (WCP), formerly called Avalon, is part of Longhorn and builds daily in Lab06. The first step you ");
            flowDocument.ContentEnd.InsertTextInRun("need to take is thus to install a recent version of the Lab06 build of Longhorn on your system. If you can't dedicate a computer to ");
            flowDocument.ContentEnd.InsertTextInRun("Longhorn, you can have WindowsXP and Longhorn coexist on the same computer by installing Longhorn and any related ");
            flowDocument.ContentEnd.InsertTextInRun("applications on a separate partition. This ensures that you do not overwrite files that may be needed by the other system. You ");
            flowDocument.ContentEnd.InsertTextInRun("choose which system to run at boot time, and you must reboot to switch from one to the other.");
            fdsv.Document.ContentEnd.InsertTextInRun("Text paragraph [part 2].");
        }

        // ------------------------------------------------------------------
        // Load FloaterWidthExceedsAvailableWidth.xaml
        // ------------------------------------------------------------------
        private void LoadFloaterWidthExceedsAvailableWidth()
        {
            _testName = "FloaterWidthExceedsAvailableWidth";
            LoadContentFromXaml();
        }

        // ------------------------------------------------------------------
        // Verify content.
        // ------------------------------------------------------------------
        private void VerifyLayout()
        {
            DRT.Assert(_testName != string.Empty, "{0}: Test name is not initialized.", this.TestName);
            DumpLayoutTree(false, true);
            _testName = string.Empty;
        }
    }
}
