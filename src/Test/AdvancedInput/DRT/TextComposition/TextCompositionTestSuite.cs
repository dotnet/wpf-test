// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// DRTTextComposition.cs
//

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
    public class TextCompositionTestSuite : DrtTestSuite
    {
        private Canvas canvas;
        private InputElement _inputElement;
        private InputTextBox _inputTextBox;

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

        public TextCompositionTestSuite() : base("TextComposition DRT")
        {
            Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            DRT.Show(CreateTree());

            return new DrtTest[]{
                           new DrtTest(Test_InputElement_0409_001),
                           new DrtTest(Test_InputElement_0409_002),
                           new DrtTest(Test_InputTextBox_0409_001),
                           new DrtTest(Test_InputTextBox_0409_002),
                           new DrtTest(Test_InputElement_0411_001),
                           new DrtTest(Test_InputElement_0411_002),
                           new DrtTest(Test_InputTextBox_0411_001),
                           new DrtTest(Test_InputTextBox_0411_002),
                           new DrtTest(Test_InputElement_0804_001),
                           new DrtTest(Test_InputElement_0804_002),
                           new DrtTest(Test_InputTextBox_0804_001),
                           new DrtTest(Test_InputTextBox_0804_002),
                           new DrtTest(Test_InputElement_0412_001),
                           new DrtTest(Test_InputElement_0412_002),
                           new DrtTest(Test_InputTextBox_0412_001),
                           new DrtTest(Test_InputTextBox_0412_002),
                           new DrtTest(Test_InputElement_0404_001),
                           new DrtTest(Test_InputElement_0404_002),
                           new DrtTest(Test_InputTextBox_0404_001),
                           new DrtTest(Test_InputTextBox_0404_002),
                           null };
        }


        private Visual CreateTree()
        {
            canvas = new Canvas();

            _inputElement = new InputElement();
            canvas.Children.Add(_inputElement);
            Canvas.SetTop(_inputElement, 40);
            Canvas.SetLeft(_inputElement, 0);
            _inputElement.Width = 300;
            _inputElement.Height = 50;

            _inputTextBox = new InputTextBox();
            canvas.Children.Add(_inputTextBox);
            Canvas.SetTop(_inputTextBox, 100);
            Canvas.SetLeft(_inputTextBox, 0);
            _inputTextBox.Width = 300;
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

        private void CloseIme()
        {
            InputMethod.Current.ImeState = InputMethodState.Off;
        }

        //-------------------------------------------------------------------
        //
        // Test for 0409
        //
        //-------------------------------------------------------------------

        private void Test_InputElement_0409_001()
        {
            _inputElement.Focus();

            if (SetLanguage(0x0409))
            {
                DRT.PrepareToSendInput();

                DRT.PressKey(Key.C);
                DRT.PressKey(Key.I);
                DRT.PressKey(Key.C);
                DRT.PressKey(Key.E);
                DRT.PressKey(Key.R);
                DRT.PressKey(Key.O);
                DRT.PressKey(Key.T);
                DRT.PressKey(Key.E);
                DRT.PressKey(Key.S);
                DRT.PressKey(Key.T);
                DRT.PressKey(Key.Space);
                DRT.PressKey(Key.Space);
                DRT.PressKey(Key.Enter);
                DRT.PressKey(Key.Enter);

                Console.WriteLine("ENG Input Test typed for InputElement.");
            }
            else
            {
                Console.WriteLine("ENG Input Test is skipped for InputElement. Not installed.");
            }

        }

        private void Test_InputElement_0409_002()
        {

            TextComposition textComposition = _inputElement.TextComposition;
            if (textComposition != null)
                textComposition.Complete();

            CloseIme();
        }

        private void Test_InputTextBox_0409_001()
        {
            _inputTextBox.Focus();

            if (SetLanguage(0x0409))
            {
                DRT.PrepareToSendInput();

                DRT.PressKey(Key.C);
                DRT.PressKey(Key.I);
                DRT.PressKey(Key.C);
                DRT.PressKey(Key.E);
                DRT.PressKey(Key.R);
                DRT.PressKey(Key.O);
                DRT.PressKey(Key.T);
                DRT.PressKey(Key.E);
                DRT.PressKey(Key.S);
                DRT.PressKey(Key.T);
                DRT.PressKey(Key.Space);
                DRT.PressKey(Key.Space);
                DRT.PressKey(Key.Enter);
                DRT.PressKey(Key.Enter);

                Console.WriteLine("ENG Input Test typed for InputTextBox.");
            }
            else
            {
                Console.WriteLine("ENG Input Test is skipped for InputTextBox. Not installed.");
            }

        }

        private void Test_InputTextBox_0409_002()
        {


            TextComposition textComposition = _inputTextBox.TextComposition;
            if (textComposition != null)
                textComposition.Complete();

            CloseIme();
        }

        //-------------------------------------------------------------------
        //
        // Test for 0411
        //
        //-------------------------------------------------------------------

        private void Test_InputElement_0411_001()
        {
            _inputElement.Focus();

            if (SetLanguage(0x0411))
            {
                DRT.PrepareToSendInput();

                DRT.PressKey(Key.K);
                DRT.PressKey(Key.Y);
                DRT.PressKey(Key.O);
                DRT.PressKey(Key.U);
                DRT.PressKey(Key.Space);
                DRT.PressKey(Key.Space);
                DRT.PressKey(Key.Enter);
                DRT.PressKey(Key.Enter);
            }
            else
            {
                Console.WriteLine("JPN Input Test is skipped for InputElement. Not installed.");
            }

        }

        private void Test_InputElement_0411_002()
        {

            TextComposition textComposition = _inputElement.TextComposition;
            if (textComposition != null)
                textComposition.Complete();

            CloseIme();
        }

        private void Test_InputTextBox_0411_001()
        {
            _inputTextBox.Focus();

            if (SetLanguage(0x0411))
            {
                DRT.PrepareToSendInput();

                DRT.PressKey(Key.K);
                DRT.PressKey(Key.Y);
                DRT.PressKey(Key.O);
                DRT.PressKey(Key.U);
                DRT.PressKey(Key.Space);
                DRT.PressKey(Key.Space);
                DRT.PressKey(Key.Enter);
                DRT.PressKey(Key.Enter);
            }
            else
            {
                Console.WriteLine("JPN Input Test is skipped for InputTextBox. Not installed.");
            }

        }

        private void Test_InputTextBox_0411_002()
        {
            TextComposition textComposition = _inputTextBox.TextComposition;
            if (textComposition != null)
                textComposition.Complete();

            CloseIme();
        }

        //-------------------------------------------------------------------
        //
        // Test for 0804
        //
        //-------------------------------------------------------------------

        private void Test_InputElement_0804_001()
        {
            _inputElement.Focus();

            if (SetLanguage(0x0804))
            {
                DRT.PrepareToSendInput();

                DRT.PressKey(Key.W);
                DRT.PressKey(Key.O);
                DRT.PressKey(Key.Space);
            }
            else
            {
                Console.WriteLine("CHS Input Test is skipped for InputElement. Not installed.");
            }

        }

        private void Test_InputElement_0804_002()
        {

            TextComposition textComposition = _inputElement.TextComposition;
            if (textComposition != null)
                textComposition.Complete();

            CloseIme();
        }

        private void Test_InputTextBox_0804_001()
        {
            _inputTextBox.Focus();

            if (SetLanguage(0x0804))
            {
                DRT.PrepareToSendInput();

                DRT.PressKey(Key.W);
                DRT.PressKey(Key.O);
                DRT.PressKey(Key.Space);
            }
            else
            {
                Console.WriteLine("CHS Input Test is skipped for InputTextBox. Not installed.");
            }

        }

        private void Test_InputTextBox_0804_002()
        {
            TextComposition textComposition = _inputTextBox.TextComposition;
            if (textComposition != null)
                textComposition.Complete();

            CloseIme();
        }

        //-------------------------------------------------------------------
        //
        // Test for 0412
        //
        //-------------------------------------------------------------------

        private void Test_InputElement_0412_001()
        {
            _inputElement.Focus();

            if (SetLanguage(0x0412))
            {
                DRT.PrepareToSendInput();

                DRT.PressKey(Key.R);
                DRT.PressKey(Key.K);
                DRT.PressKey(Key.RightCtrl);
            }
            else
            {
                Console.WriteLine("KOR Input Test is skipped for InputElement. Not installed.");
            }

        }

        private void Test_InputElement_0412_002()
        {

            TextComposition textComposition = _inputElement.TextComposition;
            if (textComposition != null)
                textComposition.Complete();

            CloseIme();
        }

        private void Test_InputTextBox_0412_001()
        {
            _inputTextBox.Focus();

            if (SetLanguage(0x0412))
            {
                DRT.PrepareToSendInput();

                DRT.PressKey(Key.R);
                DRT.PressKey(Key.K);
                DRT.PressKey(Key.RightCtrl);
            }
            else
            {
                Console.WriteLine("KOR Input Test is skipped for InputTextBox. Not installed.");
            }

        }

        private void Test_InputTextBox_0412_002()
        {
            TextComposition textComposition = _inputTextBox.TextComposition;
            if (textComposition != null)
                textComposition.Complete();

            CloseIme();
        }

        //-------------------------------------------------------------------
        //
        // Test for 0404
        //
        //-------------------------------------------------------------------

        private void Test_InputElement_0404_001()
        {
            _inputElement.Focus();

            if (SetLanguage(0x0404))
            {
                DRT.PrepareToSendInput();

                DRT.PressKey(Key.J);
                DRT.PressKey(Key.Space);
            }
            else
            {
                Console.WriteLine("CHT Input Test is skipped for InputElement. Not installed.");
            }
        }

        private void Test_InputElement_0404_002()
        {

            TextComposition textComposition = _inputElement.TextComposition;
            if (textComposition != null)
                textComposition.Complete();

            CloseIme();
        }

        private void Test_InputTextBox_0404_001()
        {
            _inputTextBox.Focus();

            if (SetLanguage(0x0404))
            {
                DRT.PrepareToSendInput();

                DRT.PressKey(Key.J);
                DRT.PressKey(Key.Space);
            }
            else
            {
                Console.WriteLine("CHT Input Test is skipped for InputTextBox. Not installed.");
            }
        }

        private void Test_InputTextBox_0404_002()
        {
            TextComposition textComposition = _inputTextBox.TextComposition;
            if (textComposition != null)
                textComposition.Complete();

            CloseIme();
        }
    }
}

