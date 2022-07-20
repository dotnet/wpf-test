// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Ink;
using System.Runtime.InteropServices;
using MS.Win32;

namespace DRT
{
    /// <summary>
    /// This is a test suite to test the visual style supports in StickyNoteControl
    /// 1. Style a StickyNoteControl in xaml.
    /// </summary>
    public sealed class StickyNoteControlStyleTests : DrtTabletTestSuite
    {
        // The values for typing test
        private string              TextString2 = "Hello World!";
        private StickyNoteControl   _snc;

        public StickyNoteControlStyleTests() : base("StickyNoteControlStyleTests")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            _snc = null;

            //
            // load our test xaml
            //
            DRT.LoadXamlFile(@"StyledSNC.xaml");

            return new DrtTest[]{
                                new DrtTest(IdentifyControls),
                                new DrtTest(PreTypeTest),
                                new DrtTest(VerifyTypeTest),
                                };

        }

        #region Tests
        private void IdentifyControls()
        {
            _snc = (StickyNoteControl)DRT.FindVisualByType(typeof(StickyNoteControl), DRT.RootElement);
        }

        //
        // test for typing text in the TextBox
        //
        private void PreTypeTest()
        {
            _snc.Text = string.Empty;
            DRT.MoveMouse(_snc, 0.5, 0.5);
            DRT.ClickMouse();
            DRT.ResumeAt(new DrtTest(TypeText));
        }

        private void TypeText()
        {
            DrtInput.KeyboardType("+hello +world+1");
        }

        private void VerifyTypeTest()
        {
            DRT.AssertEqual(TextString2, _snc.Text, "VerifyTypeTest: Types Text error!");
        }

        #endregion Tests

    }
}
