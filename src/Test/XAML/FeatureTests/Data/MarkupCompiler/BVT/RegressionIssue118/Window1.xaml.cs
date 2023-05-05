// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Markup;

namespace RegressionIssue118
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// Verify that there is no exception when ME.ProvideValue returns null
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if ((string)button1.Content == "hai" && button2.Content == null)
            {
                Application.Current.Shutdown(0);
            }
        }
    }

    [MarkupExtensionReturnType(typeof(String))]
    public class MyExtension : MarkupExtension
    {
        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == "null")
                return null;
            return Text;
        }
    }
}
