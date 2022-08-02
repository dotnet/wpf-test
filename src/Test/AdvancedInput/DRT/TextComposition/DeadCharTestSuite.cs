// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Collections;
using System.Threading;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Input;
using System.Globalization;
using System.Windows.Automation;

namespace DRT
{
    public class DeadCharTestSuite : DrtTestSuite
    {
        private Canvas canvas;
        private TextBox _inputTextBox;
        private int _typecount_dc;
        private int _typecount_ag;

        internal static IntPtr HWND_TOP = new IntPtr(0);
        internal static IntPtr HWND_BOTTOM = new IntPtr(1);
        internal static IntPtr HWND_TOPMOST = new IntPtr(-1);
        internal static IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        internal static IntPtr HWND_MESSAGE = new IntPtr( -3 );

// not used        internal static int SWP_NOSIZE = 0x0001;
// not used        internal static int SWP_NOMOVE = 0x0002;
// not used        internal static int SWP_NOZORDER = 0x0004;
// not used        internal static int SWP_NOACTIVATE = 0x0010;
// not used        internal static int SWP_SHOWWINDOW = 0x0040;
// not used        internal static int SWP_HIDEWINDOW = 0x0080;
// not used        internal static int SWP_DRAWFRAME = 0x0020;

        private DrtHkl drthkl;

        public DeadCharTestSuite() : base("DeadChar DRT")
        {
            Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            DRT.Show(CreateTree());

            return new DrtTest[]{
                           new DrtTest(Test_040c040c),
                           new DrtTest(Test_040c040c_dc_Type),
                           new DrtTest(Test_040c040c_ag_Type),
                           new DrtTest(Test_040c040c_Check),
                           new DrtTest(Test_04070407),
                           new DrtTest(Test_04070407_dc_Type),
                           new DrtTest(Test_04070407_ag_Type),
                           new DrtTest(Test_04070407_Check),
                           null };
        }


        private Visual CreateTree()
        {
            canvas = new Canvas();

            _inputTextBox = new TextBox();
            canvas.Children.Add(_inputTextBox);
            Canvas.SetTop(_inputTextBox, 40);
            Canvas.SetLeft(_inputTextBox, 0);
            _inputTextBox.Width = 500;
            _inputTextBox.Height = 50;


            return canvas;
        }


        private object CloseWindow(object obj)
        {
            Thread.Sleep(1000);
            return null;
        }

        private bool SetLanguage(int nLangId)
        {
            bool found = false;
            InputLanguageManager ilm;
            ilm = InputLanguageManager.Current;

            foreach(CultureInfo cl in ilm.AvailableInputLanguages)
            {
                if (cl.LCID == nLangId)
                {
                    ilm.CurrentInputLanguage = cl;
                    found = true;
                }
            }

            if (found)
            {
                InputMethod.Current.ImeState = InputMethodState.On;
            }
            return found;
        }

        //-------------------------------------------------------------------
        //
        // Test for 040c040c
        //
        //-------------------------------------------------------------------

        private void Test_040c040c()
        {
            _inputTextBox.Focus();
            DRT.Assert(_inputTextBox.IsKeyboardFocusWithin, "Focus should be within the search text box, focus was: " + Keyboard.FocusedElement);
            _inputTextBox.Text = "";

            drthkl = new DrtHkl((UIntPtr)0x040c040c, "0000040c");
            drthkl.Activate();
            _typecount_dc = 0;
            _typecount_ag = 0;

        }

        private void Test_040c040c_dc_Type()
        {
            DRT.Assert(drthkl.Hkl == (UIntPtr)0x040c040c, "The current hkl is not 0x040c040c in Test_040c040c_dc_Type.");
            if (drthkl.IsActivated)
            {
                DeadCharCode code = DeadCharData.code_dc_040c040c[_typecount_dc];
                code.DoKey(DRT);

                _typecount_dc++;
                if (_typecount_dc < DeadCharData.code_dc_040c040c.Length)
                {
                    DRT.RepeatTest();
                }
            }
        }

        private void Test_040c040c_ag_Type()
        {
            DRT.Assert(drthkl.Hkl == (UIntPtr)0x040c040c, "The current hkl is not 0x040c040c in Test_040c040c_ag_Type.");
            if (drthkl.IsActivated)
            {
                AltGreCharCode code = DeadCharData.code_ag_040c040c[_typecount_ag];
                code.DoKey(DRT);

                _typecount_ag++;
                if (_typecount_ag < DeadCharData.code_ag_040c040c.Length)
                {
                    DRT.RepeatTest();
                }
            }
        }

        private void Test_040c040c_Check()
        {
            DRT.Assert(drthkl.Hkl == (UIntPtr)0x040c040c, "The current hkl is not 0x040c040c in Test_040c040c_Check.");
            if (drthkl.IsActivated)
            {
                int i = 0;
                foreach (DeadCharCode code in DeadCharData.code_dc_040c040c)
                {
                    char ch = _inputTextBox.Text[i++];
                    DRT.Assert((int)ch == code._code, "code_dc_040c040c failed. value = " + ch.ToString() + " expected = " + code._code.ToString());
                }

                foreach (AltGreCharCode code in DeadCharData.code_ag_040c040c)
                {
                    char ch = _inputTextBox.Text[i++];
                    DRT.Assert((int)ch == code._code, "code_ag_040c040c failed. value = " + ch.ToString() + " expected = " + code._code.ToString());
                }
                drthkl.Restore();
            }
        }

        //-------------------------------------------------------------------
        //
        // Test for 04070407
        //
        //-------------------------------------------------------------------

        private void Test_04070407()
        {
            _inputTextBox.Focus();
            DRT.Assert(_inputTextBox.IsKeyboardFocusWithin, "Focus should be within the search text box, focus was: " + Keyboard.FocusedElement);
            _inputTextBox.Text = "";

            drthkl = new DrtHkl((UIntPtr)0x04070407, "00000407");
            drthkl.Activate();
            _typecount_dc = 0;
            _typecount_ag = 0;
        }

        private void Test_04070407_dc_Type()
        {
            DRT.Assert(drthkl.Hkl == (UIntPtr)0x04070407, "The current hkl is not 0x04070407 in Test_04070407_dc_Type.");
            if (drthkl.IsActivated)
            {
                DeadCharCode code = DeadCharData.code_dc_04070407[_typecount_dc];
                code.DoKey(DRT);

                _typecount_dc++;
                if (_typecount_dc < DeadCharData.code_dc_04070407.Length)
                {
                    DRT.RepeatTest();
                }
            }
        }

        private void Test_04070407_ag_Type()
        {
            DRT.Assert(drthkl.Hkl == (UIntPtr)0x04070407, "The current hkl is not 0x04070407 in Test_04070407_ag_Type.");
            if (drthkl.IsActivated)
            {
                AltGreCharCode code = DeadCharData.code_ag_04070407[_typecount_ag];
                code.DoKey(DRT);

                _typecount_ag++;
                if (_typecount_ag < DeadCharData.code_ag_04070407.Length)
                {
                    DRT.RepeatTest();
                }
            }
        }

        private void Test_04070407_Check()
        {
            DRT.Assert(drthkl.Hkl == (UIntPtr)0x04070407, "The current hkl is not 0x04070407 in Test_04070407_Check");
            if (drthkl.IsActivated)
            {
                int i = 0;
                foreach (DeadCharCode code in DeadCharData.code_dc_04070407)
                {
                    char ch = _inputTextBox.Text[i++];
                    DRT.Assert((int)ch == code._code, "code_dc_04070407 failed. value = " + ch.ToString() + " expected = " + code._code.ToString());
                }

                foreach (AltGreCharCode code in DeadCharData.code_ag_04070407)
                {
                    char ch = _inputTextBox.Text[i++];
                    DRT.Assert((int)ch == code._code, "code_ag_04070407 failed. value = " + ch.ToString() + " expected = " + code._code.ToString());
                }
                drthkl.Restore();
            }
        }
    }
}

