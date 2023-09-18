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
using System.Windows.Threading;


namespace FontDialogSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnFontButtonClick(object sender, RoutedEventArgs e)
        {
            ShowFontDialog();
        }

        private void ShowFontDialog()
        {
            FontChooser fontChooser = new FontChooser();
            fontChooser.Owner = this;

            fontChooser.SetPropertiesFromObject(textBox);
                fontChooser.PreviewSampleText = textBox.Selection.Text;


            if (fontChooser.ShowDialog().Value)
            {
                fontChooser.ApplyPropertiesToObject(textBox.Selection, textBox);
                fontChooser.ApplyPropertiesToObject(rtb);
                fontChooser.ApplyPropertiesToObject(tb);
            }
        }
    }
}
