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
    /// Interaction logic for NoSelectionDialog.xaml
    /// </summary>

    public partial class NoSelectionDialog : Window
    {

        public NoSelectionDialog()
        {
            InitializeComponent();
            this.PreviewKeyUp += new KeyEventHandler(NoSelectionDialog_PreviewKeyUp);
        }

        void NoSelectionDialog_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        public void SetText(string text)
        {
            textBlock.Text = text;
        }

        public void RemoveButton()
        {
            button.Visibility = Visibility.Hidden;
        }

        void b_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}