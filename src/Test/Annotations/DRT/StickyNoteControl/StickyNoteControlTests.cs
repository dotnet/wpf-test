// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Ink;

namespace DRT
{
    /// <summary>
    /// This is the test suite for -
    /// 1. Set/Get Properties: Text, IsExpanded, EntryMode, and Author
    /// 2. Invoke Methods: ClipboardCanPaste, ClipboardPaste, ClipboardCopy and ClipboardCut
    /// </summary>
    public sealed class StickyNoteControlBasicTests : DrtStickyNoteControlTestSuite
    {
        private StickyNoteControlProxy  _snProxy;

        public StickyNoteControlBasicTests() : base("StickyNoteControlBasicTests")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            DrtTest[] prepareStickyNoteControlTests = base.PrepareTests();

            _snProxy = new StickyNoteControlProxy();


            List<DrtTest> tests = new List<DrtTest>();

            // Tests before the visual tree being set up.
            tests.Add(new DrtTest(SetPropertiesBeforeVisualCreated));
            tests.Add(new DrtTest(VerifyPropertiesBeforeVisualCreated));

            // Tests for creating StickyNoteControl via CAF api
            foreach ( DrtTest test in prepareStickyNoteControlTests )
            {
                tests.Add(test);
            }

            // Tests after the visual tree being set up.
            tests.Add(new DrtTest(VerifyPropertiesAfterVisualCreated));
            tests.Add(new DrtTest(SetPropertiesTest));
            tests.Add(new DrtTest(VerifyPropertiesTest));

            return tests.ToArray();
        }

        #region Tests


        private void SetPropertiesBeforeVisualCreated()
        {
            _snProxy.IsExpanded = IsExpandedBeforeVisualTree;
            _snProxy.Author = Author0;
        }

        private void VerifyPropertiesBeforeVisualCreated()
        {
            DRT.AssertEqual(IsExpandedBeforeVisualTree, _snProxy.IsExpanded, "Sets/Gets IsExpanded before visual error!");
            DRT.AssertEqual(Author0, _snProxy.Author, "Sets/Gets Author before visual error!");
        }

        private void VerifyPropertiesAfterVisualCreated()
        {
            //DRT.AssertEqual(IsExpandedAfterVisualTree, StickyNoteProxy.IsExpanded, "Sets/Gets IsExpanded after visual error!");
            DRT.Assert(StickyNoteProxy.Author.Length ==0, "Gets Author after visual failed!");
        }

        //
        // test all of the properties
        //
        private void SetPropertiesTest()
        {
            StickyNoteProxy.IsExpanded = IsExpanded1;
            StickyNoteProxy.Author = Author0;
        }

        private void VerifyPropertiesTest()
        {
            DRT.AssertEqual(IsExpanded1, StickyNoteProxy.IsExpanded, "Sets/Gets IsExpanded error!");
            DRT.AssertEqual(Author0, StickyNoteProxy.Author, "Sets/Gets Author error!");
        }


        #endregion Tests

        #region Private Fields

        private bool IsExpanded1 = true;

        private bool IsExpandedBeforeVisualTree = false;

        //private bool IsExpandedAfterVisualTree = true;
        private string Author0 = "Microsoft";

        #endregion Private Fields
    }

}

