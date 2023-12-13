// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Description: Defines a splash screen.

#if !XamlPadExpressApp
using System;
using System.Windows;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using XamlPadEdit.Properties;
using System.Windows.Media;
using System.Windows.Interop;
using System.Drawing;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Data;
using System.Windows.Media.Animation;

namespace XamlPadEdit
{
    internal class SplashScreen
    {
        Window _splash = new Window();
        ProgressBar _progressBar = new ProgressBar();
        int _timeout = 0;

        public SplashScreen(int timeout)
        {
            _timeout = timeout;
        }

        public void Open(bool FastPace)
        {
            Bitmap bitmap = Resources.xamlEditScreen;
            BitmapSource source = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, new Int32Rect(0, 0, bitmap.Width, bitmap.Height), BitmapSizeOptions.FromWidthAndHeight(bitmap.Width, bitmap.Height));

            _splash.WindowStyle = WindowStyle.None;
            _splash.SizeToContent = SizeToContent.WidthAndHeight;
            _splash.ResizeMode = ResizeMode.NoResize;
            _splash.BorderThickness = new Thickness(0);
            _splash.Topmost = true;

            System.Windows.Controls.Image i = new System.Windows.Controls.Image();
            i.Source = source;
            i.MaxHeight = 363;
            i.MaxWidth = 444;
            i.ClipToBounds = true;
            i.Margin = new Thickness(20, 20, 20, 0);

            _splash.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _splash.AllowsTransparency = true;
            _splash.Background = System.Windows.Media.Brushes.Transparent;
            _splash.Opacity = 0.95;

            _progressBar.Width = 400;
            _progressBar.Maximum = _timeout;
            _progressBar.Margin = new Thickness(20, 0, 20, 20);
            _progressBar.Foreground = System.Windows.Media.Brushes.Lime;

            System.Windows.Data.Binding _binding = new System.Windows.Data.Binding("ActualWidth");
            _binding.Source = i;
            BindingOperations.SetBinding(_progressBar, ProgressBar.WidthProperty, _binding);
            _progressBar.Height = 20;
            _progressBar.Value = (_progressBar.Maximum )/4*3;

            StackPanel _sp = new StackPanel();
            _sp.Children.Add(i);
            _sp.Children.Add(_progressBar);

            Border b = new Border();
            b.BorderThickness = new Thickness(2);
            b.CornerRadius = new CornerRadius(20);
            b.BorderBrush = System.Windows.Media.Brushes.Black;
            b.Child = _sp;

            _splash.Content = b;
            _splash.ShowInTaskbar = false;
            _splash.Loaded+=new RoutedEventHandler(splash_Loaded);
            try
            {
                if (FastPace)
                {
                    _progressBar.Value = 10.5;
                    _progressBar.Maximum = 12;
                    _timeout = 12;
                    _splash.ShowDialog();
                }
                else
                {
                    _splash.ShowDialog();
                }
            }
            catch (Exception)
            {
                _splash.Close();                

            }
        }

        public void CloseWindow()
        {
            _splash.Close();
            GC.Collect();
        }

        void splash_Loaded(object sender, RoutedEventArgs e)
        {
            int durationSecs = (XamlPadMain._resourcesExist)?1:6;
            DoubleAnimation doubleanimation = new DoubleAnimation(_progressBar.Value, _timeout - 0.5, new Duration(new TimeSpan(0, 0, durationSecs)));
            Dispatcher.CurrentDispatcher.Thread.IsBackground = true;
            _progressBar.BeginAnimation(ProgressBar.ValueProperty, doubleanimation);
        }
    }
}
#endif