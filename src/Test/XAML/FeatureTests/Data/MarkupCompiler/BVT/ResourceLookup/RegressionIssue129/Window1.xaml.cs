// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;

namespace RegressionIssue129
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private string _appRes = "ApplicationRes";
        private string _styleARes = "StyleARes";
        private string _styleBRes = "StyleBRes";
        private string _styleCRes = "StyleCRes";
        private int _exitCode = 0;

        public Window1()
        {
            InitializeComponent();
        }

        // Verifying the proper resource has been found across multiple styles and Application.Resources
        private void Window_Loaded(object sender, EventArgs e)
        {
            VerifyContent(ButtonA, _styleARes);
            VerifyContent(ButtonB, _styleBRes);
            VerifyContent(ButtonC, _styleCRes);
            VerifyContent(ButtonD, _styleBRes);
            VerifyContent(ButtonE, _styleARes);
            VerifyContent(ButtonF, _styleARes);
            VerifyContent(ButtonG, _appRes);
            VerifyContent(ButtonH, _appRes);
            VerifyContent(ButtonI, _appRes);

            Application.Current.Shutdown(_exitCode);
        }

        private void VerifyContent(Button button, string expected)
        {
            if (!String.Equals(button.Content, expected))
            {
                Console.WriteLine(String.Format("{0}'s content was: {1}, should have been {2}", button.Name, button.Content, expected));
                _exitCode = 1;
            }
        }
    }
}
