// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace RegressionIssue112
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>

    public partial class UserControl1 : UserControl, ISupportInitialize
    {
        private string _text;

        public UserControl1()
        {
            _text = "";
            InitializeComponent();
        }

        public void OnLoaded(object sender, EventArgs e)
        {
            TB1.Text = _text;
        }

        #region ISupportInitialize Members

        void ISupportInitialize.BeginInit()
        {
            _text += "BeginInit";
        }

        void ISupportInitialize.EndInit()
        {
            _text += "EndInit";
        }

        #endregion
    }
}
