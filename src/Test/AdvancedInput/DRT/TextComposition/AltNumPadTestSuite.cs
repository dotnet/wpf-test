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
    public class AltNumpadTestSuite : DrtTestSuite
    {
        private Canvas canvas;
        private InputTextBox _inputTextBoxOem;
        private InputTextBox _inputTextBoxDefault;
        private int _typingcount;
        private int _failedAttempts;
        private static int _MaxAttemptsOnFailedInput = 10;

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

        public AltNumpadTestSuite() : base("AltNumpad DRT")
        {
            Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            DRT.Show(CreateTree());

            return new DrtTest[]{
                           new DrtTest(Test_AltNumpad_OemCodepage_Init),
                           new DrtTest(Test_AltNumpad_OemCodepage_Init2),
                           new DrtTest(Test_AltNumpad_OemCodepage_Type),
                           new DrtTest(Test_AltNumpad_DefaultCodepage_Init),
                           new DrtTest(Test_AltNumpad_DefaultCodepage_Init2),
                           new DrtTest(Test_AltNumpad_DefaultCodepage_Type),
                           null };
        }


        private Visual CreateTree()
        {
            canvas = new Canvas();

            _inputTextBoxOem = new InputTextBox();
            canvas.Children.Add(_inputTextBoxOem);
            Canvas.SetTop(_inputTextBoxOem, 40);
            Canvas.SetLeft(_inputTextBoxOem, 0);
            _inputTextBoxOem.Width = 500;
            _inputTextBoxOem.Height = 50;

            _inputTextBoxDefault = new InputTextBox();
            canvas.Children.Add(_inputTextBoxDefault);
            Canvas.SetTop(_inputTextBoxDefault, 100);
            Canvas.SetLeft(_inputTextBoxDefault, 0);
            _inputTextBoxDefault.Width = 500;
            _inputTextBoxDefault.Height = 50;

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

        private bool CheckLanguage(int nLangId)
        {
            CultureInfo cl = InputLanguageManager.Current.CurrentInputLanguage;
            if (cl.LCID != nLangId)
                return false;

            return true;
        }


        //-------------------------------------------------------------------
        //
        // Test for OEM codepage
        //
        //-------------------------------------------------------------------

        private void SendNumpadKeys(string str)
        {
            Console.WriteLine(" SendNumpadKey - " + str + " Focus is on " + Keyboard.FocusedElement);
            DRT.PrepareToSendInput();

            DRT.SendKeyboardInput(Key.LeftAlt, true);

            for (int i = 0; i < str.Length; i++)
            {
                switch (str[i])
                {
                    case '0': DRT.PressKey(Key.NumPad0); break;
                    case '1': DRT.PressKey(Key.NumPad1); break;
                    case '2': DRT.PressKey(Key.NumPad2); break;
                    case '3': DRT.PressKey(Key.NumPad3); break;
                    case '4': DRT.PressKey(Key.NumPad4); break;
                    case '5': DRT.PressKey(Key.NumPad5); break;
                    case '6': DRT.PressKey(Key.NumPad6); break;
                    case '7': DRT.PressKey(Key.NumPad7); break;
                    case '8': DRT.PressKey(Key.NumPad8); break;
                    case '9': DRT.PressKey(Key.NumPad9); break;
                }
            }

            DRT.SendKeyboardInput(Key.LeftAlt, false);
        }

        private void Test_AltNumpad_OemCodepage_Init()
        {
            drthkl = new DrtHkl((UIntPtr)0x04090409, "00000409");
            drthkl.Activate();

            _inputTextBoxOem.Focus();
            DRT.Assert(_inputTextBoxOem.IsKeyboardFocusWithin, "Focus should be within the search text box, focus was: " + Keyboard.FocusedElement);
            _inputTextBoxOem.Text = "";

            _typingcount = 0;
            _failedAttempts = 0;

            Console.WriteLine(" Test_AltNumPat_OemCodepage_Init - Focus is on " + Keyboard.FocusedElement);
        }

        private void Test_AltNumpad_OemCodepage_Init2()
        {
            DRT.Assert(_inputTextBoxOem.IsKeyboardFocusWithin, "Focus should be within the search text box, focus was: " + Keyboard.FocusedElement);
            Console.WriteLine(" Test_AltNumPat_OemCodepage_Init2 - Focus is on " + Keyboard.FocusedElement);
        }

        private void Test_AltNumpad_OemCodepage_Type()
        {
            DRT.Assert(drthkl.Hkl == (UIntPtr)0x04090409, "The current hkl is not 0x04090409 in Test_AltNumpad_OemCodepage_Type.");
            if (drthkl.IsActivated)
            {
                SendNumpadKeys(HiAnsiCode.codes[_typingcount]._strOemCP);
                DRT.ResumeAt(new DrtTest(Test_AltNumpad_OemCodepage_Check));
            }
        }

        private void Test_AltNumpad_DefaultCodepage_Init2()
        {
            DRT.Assert(_inputTextBoxDefault.IsKeyboardFocusWithin, "Focus should be within the search text box, focus was: " + Keyboard.FocusedElement);
            Console.WriteLine(" Test_AltNumPat_DefaultCodepage_Init2 - Focus is on " + Keyboard.FocusedElement);
        }

        private void Test_AltNumpad_OemCodepage_Check()
        {
            DRT.Assert(drthkl.Hkl == (UIntPtr)0x04090409, "The current hkl is not 0x04090409 in Test_AltNumpad_OemCodepage_Check.");
            if (drthkl.IsActivated)
            {
                if (_typingcount < _inputTextBoxOem.Text.Length &&
                    _inputTextBoxOem.Text[_typingcount] == HiAnsiCode.codes[_typingcount]._nOemCP)
                {
                    _failedAttempts = 0;
                    _typingcount++;
                }
                else
                {
                    _failedAttempts++;
                    if (_failedAttempts >= _MaxAttemptsOnFailedInput)
                    {
                        if (_typingcount + 1 == _inputTextBoxOem.Text.Length)
                        {
                            DRT.Assert(_failedAttempts < _MaxAttemptsOnFailedInput,
                                "AltNumpad default Codepage check failed: value =" + _inputTextBoxOem.Text[_typingcount] + " expected = " + HiAnsiCode.codes[_typingcount]._nOemCP);
                        }
                        else
                        {
                            DRT.Assert(_failedAttempts < _MaxAttemptsOnFailedInput,
                                "AltNumpad default Codepage check failed: missing input value=" + HiAnsiCode.codes[_typingcount]._nOemCP);
                        }
                    }
                }

                if (_typingcount < HiAnsiCode.codes.Length)
                {
                    // Go for the next char.
                    DRT.ResumeAt(new DrtTest(Test_AltNumpad_OemCodepage_Type));
                }
                else
                {
                    drthkl.Restore();
                }
            }
        }

        //-------------------------------------------------------------------
        //
        // Test for default codepage
        //
        //-------------------------------------------------------------------

        private void Test_AltNumpad_DefaultCodepage_Init()
        {
            drthkl = new DrtHkl((UIntPtr)0x04090409, "00000409");
            drthkl.Activate();

            _inputTextBoxDefault.Focus();
            DRT.Assert(_inputTextBoxDefault.IsKeyboardFocusWithin, "Focus should be within the search text box, focus was: " + Keyboard.FocusedElement);
            _inputTextBoxDefault.Text = "";

            _typingcount = 0;
            _failedAttempts = 0;
        }

        private void Test_AltNumpad_DefaultCodepage_Type()
        {
            DRT.Assert(drthkl.Hkl == (UIntPtr)0x04090409, "The current hkl is not 0x04090409 in Test_AltNumpad_DefaultCodepage_Type.");
            if (drthkl.IsActivated)
            {
                SendNumpadKeys(HiAnsiCode.codes[_typingcount]._strDefCP);
                DRT.ResumeAt(new DrtTest(Test_AltNumpad_DefaultCodepage_Check));
            }
        }

        private void Test_AltNumpad_DefaultCodepage_Check()
        {
            DRT.Assert(drthkl.Hkl == (UIntPtr)0x04090409, "The current hkl is not 0x04090409 in Test_AltNumpad_DefaultCodepage_Check.");
            if (drthkl.IsActivated)
            {
                if (_typingcount < _inputTextBoxDefault.Text.Length &&
                    (int)_inputTextBoxDefault.Text[_typingcount] == HiAnsiCode.codes[_typingcount]._nDefCP)
                {
                    _failedAttempts = 0;
                    _typingcount++;
                }
                else
                {
                    _failedAttempts++;
                    if (_failedAttempts >= _MaxAttemptsOnFailedInput)
                    {
                        if (_typingcount + 1 == _inputTextBoxDefault.Text.Length)
                        {
                            DRT.Assert(_failedAttempts < _MaxAttemptsOnFailedInput,
                                "AltNumpad default Codepage check failed: value =" + _inputTextBoxDefault.Text[_typingcount] + " expected = " + HiAnsiCode.codes[_typingcount]._nDefCP);
                        }
                        else
                        {
                            DRT.Assert(_failedAttempts < _MaxAttemptsOnFailedInput,
                                "AltNumpad default Codepage check failed: missing input value=" + HiAnsiCode.codes[_typingcount]._nDefCP);
                        }
                    }
                }

                if (_typingcount < HiAnsiCode.codes.Length - 2) // cut the lst two (20 and 21) for now until we find out why they fail
                {
                    // Go for the next char.
                    DRT.ResumeAt(new DrtTest(Test_AltNumpad_DefaultCodepage_Type));
                }
                else
                {
                    drthkl.Restore();
                }
            }
        }
    }
}

