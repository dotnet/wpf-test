// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.AddIn;
using System.AddIn.Pipeline;

namespace Microsoft.Test.AddIn
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    [AddIn("ClickCounter", Version = "1.0.0.0")]
    public class CountClicksAddInCode : AddInCountClicksView
    {
        private int _clicks;
        private Canvas _canvas;
        private Button _button;

        public CountClicksAddInCode()
        {
            _clicks = 0;

            _canvas = new Canvas();
            _canvas.Name = "RootCanvas";
            _canvas.Height = 300;
            _canvas.Width = 300;
            _canvas.Background = new SolidColorBrush(Colors.CornflowerBlue);

            _button = new Button();
            _button.Name = "ClickMe";
            _button.Content = "Click Me";
            _button.Height = 100;
            _button.Width = 100;

            Canvas.SetTop(_button, 1);
            Canvas.SetLeft(_button, 1);
            _button.Click += new RoutedEventHandler(button_Click);

            _canvas.Children.Add(_button);

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            _clicks++;
        }

        public override int Clicks
        {
            get
            {
                return _clicks;
            }
            set
            {
                _clicks = value;
            }
        }

        public override void Initialize(string addInParameters)
        {
            //
        }

        public override FrameworkElement GetAddInUserInterface()
        {
            return _canvas;
        }

        /// <summary>
        /// Gets the AddIn's UI child (the button)
        /// </summary>
        /// <returns>Child FrameworkElement of the AddIn's UI</returns>
        public override FrameworkElement GetAddInUserInterfaceChild()
        {
            return _button;
        }

    }
}
