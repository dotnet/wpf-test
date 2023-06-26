// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace XamlPadEdit
{
    /// <summary>
    /// Interaction logic for GoToLineDialog.xaml
    /// </summary>
    public partial class GoToLineDialog : System.Windows.Window
    {
        private TextBox _tb;

        public GoToLineDialog(TextBox MainTextBox)
        {
            _tb = MainTextBox;
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(GoToLineDialog_Loaded);
            cancel.Click += new RoutedEventHandler(cancel_Click);
            ok.Click += new RoutedEventHandler(ok_Click);
            this.PreviewKeyUp += new KeyEventHandler(GoToLineDialog_PreviewKeyUp);
        }

        void GoToLineDialog_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private delegate void SimpleDelegate();
        void ok_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int result;
                if (Int32.TryParse(lineNum.Text, out result))
                {
                    if ((result < 0) || (result > (_tb.GetLineIndexFromCharacterIndex(_tb.Text.Length)+1)))
                    {
                        throw new Exception("Input: Incorrect Line Number");
                    }
                    else
                    {
                        if (result == _tb.LineCount - 1)
                        {
                            _tb.Select(_tb.GetCharacterIndexFromLineIndex(result), _tb.Text.Length - _tb.GetCharacterIndexFromLineIndex(result));
                        }
                        else
                        {
                            _tb.Select(_tb.GetCharacterIndexFromLineIndex(result), (_tb.Text.IndexOf("\r\n", _tb.GetCharacterIndexFromLineIndex(result)) - _tb.GetCharacterIndexFromLineIndex(result)));
                        }
                        _tb.ScrollToLine(result);
                        Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new SimpleDelegate(DoFocus));
                        this.Close();
                    }
                }
                else
                {
                    throw new Exception("Only integers allowed");
                }
            }
            catch (Exception e1)
            {
                NoSelectionDialog dialog = new NoSelectionDialog();
                dialog.SetText(e1.Message);
                dialog.Width = 500;
                dialog.Height = 100;
                dialog.ShowDialog();
            }
        }

        void Text_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ok_Click(sender, null);
            }
        }

        private void DoFocus()
        {
            _tb.Focus();
        }

        public void ShowWindow()
        {
            lineNum.Focus();
            this.ShowDialog();
        }

        void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        void GoToLineDialog_Loaded(object sender, RoutedEventArgs e)
        {
            int lastLine = _tb.GetLineIndexFromCharacterIndex(_tb.Text.Length);
            gotoLineLabel.Content = "Line Number (0-" + lastLine + ")";
        }
    }
}