// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Build with the following command line from the devtest directory.

namespace Timer
{
    #region Namespaces.

    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Input;
    using System.Windows.Threading;

    #endregion Namespaces.

    public class Timer
    {
        #region Stock entry point.

        private Application _application;

        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                new Timer().DoMain(args);
            }
            catch(System.Exception e)
            {
                System.Console.WriteLine(e);
            }
        }

        public void DoMain(string[] args)
        {
            _application = new Application();
            _application.Startup += OnStartup;
            _application.Run();
        }

        public void OnStartup(object sender, StartupEventArgs args)
        {
            RunTestCase();
        }

        #endregion Stock entry point.

        private Window _window;
        private Label _label;
        private Viewbox _panel;
        private DispatcherTimer _timer;
        private DateTime _startTime;

        public void RunTestCase()
        {
            _window = new Window();
            _label = new Label();
            _panel = new Viewbox();
            _timer = new DispatcherTimer(
                TimeSpan.FromSeconds(1),
                DispatcherPriority.Background,
                delegate {
                    // Chop off milliseconds.
                    TimeSpan span = DateTime.Now - _startTime;
                    span = new TimeSpan(span.Days,span.Hours, span.Minutes, span.Seconds);
                    _label.Content = span;
                },
                Dispatcher.CurrentDispatcher);
            _timer.IsEnabled = false;

            _label.Content = "Click me";
            _label.MouseLeftButtonUp += delegate {
                // First time we run, make the window less conspicuous.
                // We need to use WinForms to get the right coordinates.
                if (!_timer.IsEnabled)
                {
                    _window.Width = 100;
                    _window.Height = 80;
                    _window.Left = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Right - 120;
                    _window.Top = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Bottom - 80;
                    _timer.IsEnabled = true;
                }
                _startTime = DateTime.Now;
                _label.ToolTip = "Time elapsed since " + _startTime.ToString("hh:mm:ss");
            };

            _panel.Child = _label;
            _window.Content = _panel;
            _window.Topmost = true;
            _window.Background = new LinearGradientBrush(Colors.LightBlue, Colors.SlateBlue, 90);
            _window.Text = "Timer";

            _window.Show();
        }
    }
}