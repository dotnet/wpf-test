// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;

namespace DRT
{
    public class InputLanguageTestSuite : DrtTestSuite
    {
        private Canvas _canvas;
        private TextBox _tb1;

        public InputLanguageTestSuite() : base("InputLanguage state DRT")
        {
            Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            DRT.Show(CreateTree());

            return new DrtTest[]{
                                 new DrtTest( ChangeInputLanguage ),
                                 null };
        }

        private Visual CreateTree()
        {
            _canvas = new Canvas();

            _tb1 = new TextBox();
            _canvas.Children.Add(_tb1);
            Canvas.SetTop(_tb1, 0);
            Canvas.SetLeft(_tb1, 20);
            _tb1.Width = 300;
            _tb1.Height = 40;

            return _canvas;
        }

        private void ChangeInputLanguage()
        {
            DRT.PrepareToSendInput();

            Keyboard.Focus(_tb1);

            InputLanguageManager ilm;
            ilm = InputLanguageManager.Current;

            foreach(CultureInfo cl in ilm.AvailableInputLanguages)
            {
                ilm.CurrentInputLanguage = cl;
            }
        }
    }
}

