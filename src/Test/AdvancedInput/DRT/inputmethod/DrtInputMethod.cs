// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading;
using System.Windows.Media;
using System.Windows.Automation;

namespace DRT
{

    public class DrtInputMethod : DrtBase
    {
        [STAThread]
        public static int Main(string[] args)
        {
            DrtBase drt = new DrtInputMethod();

            ((DrtInputMethod)drt).WindowSize = new Size(800,600);
            drt.Run(args);

            Console.WriteLine( "Passed" );
            return 0;
        }

        //
        // constructor
        //
        DrtInputMethod()
        {
            WindowTitle = "InputMethod Drt";
            TeamContact = "WPF";
            Contact= "Microsoft";
            DrtName= "DRTInputMethod";

            WarningMismatchedForeground = WarningLevel.Error;

            Suites = new DrtTestSuite[]
                        {
                            new InputMethodStateTestSuite(),
                            new InputScopeTestSuite(),
                            new InputLanguageTestSuite(),
                            null
                        };
        }

    }

    public class InputMethodStateTestSuite : DrtTestSuite
    {
        private Canvas _canvas;
        private TextBox _tb1;
        private TextBox _tb2;
        private MyBoxElement _mb1;
        private MyBoxElement _mb2;

        public InputMethodStateTestSuite() : base("InputMethod state DRT")
        {
            Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            DRT.Show(CreateTree());

            return new DrtTest[]{
                       new DrtTest( ChangeImeState ),
                       new DrtTest( CheckPreferredImeState ),
                       new DrtTest( ChangeInputMethodEnabled ),
                            null };
        }

        private Visual CreateTree()
        {
            _canvas = new Canvas();

//  -- DRT.WindowSize is not accessible as it's protected. Find replacement --
//            _canvas.Width = new Length(DRT.WindowSize.Width);
//           _canvas.Height = new Length(DRT.WindowSize.Hight);

            _tb1 = new TextBox();
            _tb2 = new TextBox();
            _mb1 = new MyBoxElement();
            _mb2 = new MyBoxElement();
            UIElementCollection children = _canvas.Children;
            children.Add(_tb1);
            children.Add(_tb2);
            children.Add(_mb1);
            children.Add(_mb2);

            Canvas.SetTop(_tb1, 0);
            Canvas.SetLeft(_tb1, 20);
            _tb1.Width = 300;
            _tb1.Height = 40;
            _tb1.Text = "tb1";
            Canvas.SetTop(_tb2, 50);
            Canvas.SetLeft(_tb2, 20);
            _tb2.Width = 300;
            _tb2.Height = 40;
            _tb2.Text = "tb2";

            Canvas.SetTop(_mb1, 100);
            Canvas.SetLeft(_mb1, 20);
            _mb1.Width = 300;
            _mb1.Height = 40;
            _mb1.Background = new SolidColorBrush(Color.FromRgb(0xff, 0, 0));

            Canvas.SetTop(_mb2, 150);
            Canvas.SetLeft(_mb2, 20);
            _mb2.Width = 300;
            _mb2.Height = 40;
            _mb2.Background = new SolidColorBrush(Color.FromRgb(0xff, 0xff, 0));

            _tb2.SetValue(InputMethod.PreferredImeStateProperty, InputMethodState.On);

            return _canvas;

        }

        private void ChangeImeState()
        {
            InputLanguageManager ilm = InputLanguageManager.Current;
            InputMethod im = InputMethod.Current;

            DRT.PrepareToSendInput();
            Keyboard.Focus(_tb1);

            foreach(CultureInfo cl in ilm.AvailableInputLanguages)
            {
                ilm.CurrentInputLanguage = cl;

                if ((cl.LCID == 0x0404) ||
                    (cl.LCID == 0x0804) ||
                    (cl.LCID == 0x0411) ||
                    (cl.LCID == 0x0412))
                {
                    im.ImeState = InputMethodState.On;
                    im.ImeState = InputMethodState.Off;

                    ImeConversionModeValues convmode = im.ImeConversionMode;
                    im.ImeConversionMode = ImeConversionModeValues.Alphanumeric;
                    im.ImeConversionMode = ImeConversionModeValues.Native;
                    im.ImeConversionMode = ImeConversionModeValues.Native | ImeConversionModeValues.Katakana;
                    im.ImeConversionMode = ImeConversionModeValues.Native | ImeConversionModeValues.FullShape;
                    im.ImeConversionMode = ImeConversionModeValues.Native | ImeConversionModeValues.Katakana | ImeConversionModeValues.FullShape;
                    im.ImeConversionMode = ImeConversionModeValues.Native | ImeConversionModeValues.FullShape| ImeConversionModeValues.Roman;
                    im.ImeConversionMode = convmode;
                }
            }


        }

        private void CheckPreferredImeState()
        {
            // set and get ImeState any way.

            DRT.PrepareToSendInput();
            Keyboard.Focus(_tb1);
            InputMethod.Current.ImeState = InputMethodState.Off;

            Keyboard.Focus(_tb2);
            if (InputMethod.Current.ImeState != InputMethodState.On)
            {
                // This is temporary solution.
                // Cicero Compartment is not be available if TIP is not installed.
                // Then the value could not be set.
                CultureInfo cl = InputLanguageManager.Current.CurrentInputLanguage;
                if ((cl.LCID == 0x0404) ||
                    (cl.LCID == 0x0804) ||
                    (cl.LCID == 0x0411) ||
                    (cl.LCID == 0x0412))
                    throw new ApplicationException("ImeState is not changed");
            }

        }

        private void ChangeInputMethodEnabled()
        {

            InputLanguageManager ilm = InputLanguageManager.Current;
            InputMethod im = InputMethod.Current;

            _mb2.SetValue(InputMethod.IsInputMethodEnabledProperty, false);

            foreach(CultureInfo cl in ilm.AvailableInputLanguages)
            {
                DRT.PrepareToSendInput();

                Keyboard.Focus(_mb1);

                ilm.CurrentInputLanguage = cl;

                Keyboard.Focus(_mb2);

                _mb2.SetValue(InputMethod.IsInputMethodEnabledProperty, true);
                _mb2.SetValue(InputMethod.IsInputMethodEnabledProperty, false);

            }
        }
    }
}
