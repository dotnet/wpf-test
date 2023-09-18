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
    /// Interaction logic for FindAndReplace.xaml
    /// </summary>

    public partial class FindAndReplace : System.Windows.Window
    {
        TextBox _tb;
        int _index = 0;

        public FindAndReplace(TextBox MainTextBox)
        {
            _tb = MainTextBox;
            _index = 0;
            InitializeComponent();
            ok.Click += new RoutedEventHandler(ok_Click);
            cancel.Click += new RoutedEventHandler(cancel_Click);
            this.PreviewKeyUp += new KeyEventHandler(Window2_PreviewKeyUp);

        }

        void Text_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ok_Click(sender, null);
            }
        }

        void Window2_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void ShowWindow()
        {
            searchText.Focus();
            this.Show();
        }

        public void ok_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (searchText.Text == "")
                {
                    NoSelectionDialog dialog = new NoSelectionDialog();
                    dialog.SetText("SearchText cannot be empty");
                    dialog.Width = 500;
                    dialog.ShowDialog();
                    return;
                }
                if (replaceAllText.Text != "")
                {
                    _tb.Text = _tb.Text.Replace(searchText.Text, replaceAllText.Text);
                    this.Close();
                    return;
                }
                if ((_tb.SelectedText == searchText.Text) && (replaceText.Text != ""))
                {
                    _tb.SelectedText = replaceText.Text;
                }
                _index = (_tb.CaretIndex == _tb.Text.Length) ? 0 : (_tb.CaretIndex + 1);
                _index = _tb.Text.IndexOf(searchText.Text, _index);
                if (_index == -1)
                {
                    _index = _tb.Text.IndexOf(searchText.Text, 0);
                }
                if (_index >= 0)
                {
                    _tb.Select(_index, searchText.Text.Length);
                    _tb.ScrollToLine(_tb.GetLineIndexFromCharacterIndex(_index));
                    _tb.Focus();
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                _index = 0;
                NoSelectionDialog dialog = new NoSelectionDialog();
                dialog.SetText("Cannot Find \"" + searchText.Text + "\"");
                dialog.Width = 500;
                dialog.ShowDialog();
            }
        }
    }
}