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
using System.Windows.GlobalMouseHook;
using System.Runtime.InteropServices;

namespace XamlPadEdit
{
    /// <summary>
    /// Interaction logic for ColorPallette.xaml
    /// </summary>

    public partial class ColorPallette : System.Windows.Window
    {

        RenderTargetBitmap _render = null;
        RenderTargetBitmap _initialRender = null;
        private bool _firstChange = true;
        private byte[] _pixels;
        MouseHook _mouseHook = null;

        public ColorPallette()
        {
            InitializeComponent();

            rec1.MouseEnter += new MouseEventHandler(rec1_MouseEnter);
            rec1.PreviewMouseDown += new MouseButtonEventHandler(rec1_PreviewMouseDown);
            rec1.MouseLeave += new MouseEventHandler(rec1_MouseLeave);

            tb.PreviewMouseDown += new MouseButtonEventHandler(tb_PreviewMouseDown);

            wheel.MouseEnter += new MouseEventHandler(wheel_MouseEnter);
            wheel.MouseLeave += new MouseEventHandler(wheel_MouseLeave);
            wheel.MouseDown += new MouseButtonEventHandler(wheel_MouseDown);
            this.Loaded += new RoutedEventHandler(Window1_Loaded);
        }

        void rec1_MouseLeave(object sender, MouseEventArgs e)
        {
            rec1.MouseMove -= new MouseEventHandler(rec1_MouseMove);
        }

        private delegate void SimpleDelegate();
        void tb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new SimpleDelegate(DoFocus));
        }

        private void DoFocus()
        {
            tb.SelectAll();
        }

        void rec1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            rec1.MouseMove -= new MouseEventHandler(rec1_MouseMove);
        }

        void rec1_MouseEnter(object sender, MouseEventArgs e)
        {

            if (ColorPicker.IsChecked == false)
            {
                _firstChange = true;
                _render = null;
                rec1.MouseMove += new MouseEventHandler(rec1_MouseMove);
            }
        }

        void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            this.Height = sp.ActualHeight + 30;
            _initialRender = new RenderTargetBitmap((int)175, (int)150, 0, 0, PixelFormats.Default);
            _initialRender.Render(wheel);
        }


        void rec1_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (_firstChange)
                {
                    _render = new RenderTargetBitmap((int)175, (int)200, 0, 0, PixelFormats.Default);
                    _render.Render(rec1);
                    _firstChange = false;
                }

                CroppedBitmap cb = new CroppedBitmap(_render as BitmapSource, new Int32Rect((int)Mouse.GetPosition(this).X, (int)Mouse.GetPosition(this).Y, 1, 1));
                _pixels = new byte[4];

                try
                {
                    cb.CopyPixels(_pixels, 4, 0);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                Console.WriteLine(_pixels[0] + ":" + _pixels[1] + ":" + _pixels[2] + ":" + _pixels[3]);

                rec.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(_pixels[2], _pixels[1], _pixels[0]));
                tb.Text = System.Windows.Media.Color.FromRgb(_pixels[2], _pixels[1], _pixels[0]).ToString();

            }
            catch (Exception)
            {
            }
        }

        void wheel_MouseLeave(object sender, MouseEventArgs e)
        {
            wheel.MouseMove -= new MouseEventHandler(wheel_MouseMove);
        }

        void wheel_MouseEnter(object sender, MouseEventArgs e)
        {
            if (ColorPicker.IsChecked == false)
            {
                _firstChange = true;
                _render = null;
                wheel.MouseMove += new MouseEventHandler(wheel_MouseMove);
            }
        }

        void wheel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            wheel.MouseMove -= new MouseEventHandler(wheel_MouseMove);
        }


        void wheel_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                CroppedBitmap cb = new CroppedBitmap(_initialRender as BitmapSource, new Int32Rect((int)Mouse.GetPosition(this).X, (int)Mouse.GetPosition(this).Y, 1, 1));
                _pixels = new byte[4];
                try
                {
                    cb.CopyPixels(_pixels, 4, 0);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                Console.WriteLine(_pixels[0] + ":" + _pixels[1] + ":" + _pixels[2] + ":" + _pixels[3]);

                LinearGradientBrush myHorizontalGradient =
    new LinearGradientBrush();
                myHorizontalGradient.StartPoint = new Point(0, 0.5);
                myHorizontalGradient.EndPoint = new Point(1, 0.5);
                myHorizontalGradient.GradientStops.Add(
                    new GradientStop(Colors.Black, 0.05));
                myHorizontalGradient.GradientStops.Add(
                    new GradientStop(System.Windows.Media.Color.FromRgb(_pixels[2], _pixels[1], _pixels[0]), 0.5));
                myHorizontalGradient.GradientStops.Add(
                    new GradientStop(Colors.White, 0.9));

                rec.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(_pixels[2], _pixels[1], _pixels[0]));
                rec1.Fill = myHorizontalGradient;
                tb.Text = System.Windows.Media.Color.FromRgb(_pixels[2], _pixels[1], _pixels[0]).ToString();
            }
            catch (Exception)
            {
            }
        }

        void ColorPicker_Checked(object sender, RoutedEventArgs e)
        {
            if (_mouseHook == null)
            {
                _mouseHook = new MouseHook();
            }
            _mouseHook.OnMouseActivity += new System.Windows.Forms.MouseEventHandler(mouseHook_OnMouseActivity);
        }

        void mouseHook_OnMouseActivity(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Console.WriteLine(e.Clicks);
            if (e.Clicks == 100)
            {
                ColorPicker.IsChecked = false;
                tb.Focus();
            }
            else
            {
                System.Drawing.Color C = GetColorUnderCursor();
                rec.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(C.R, C.G, C.B));
                LinearGradientBrush myHorizontalGradient = new LinearGradientBrush();
                myHorizontalGradient.StartPoint = new Point(0, 0.5);
                myHorizontalGradient.EndPoint = new Point(1, 0.5);
                myHorizontalGradient.GradientStops.Add(new GradientStop(Colors.Black, 0.05));
                myHorizontalGradient.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(C.R, C.G, C.B), 0.5));
                myHorizontalGradient.GradientStops.Add(new GradientStop(Colors.White, 0.9));

                rec1.Fill = myHorizontalGradient;
                tb.Text = rec.Fill.ToString();
            }
        }

        void ColorPicker_Unchecked(object sender, RoutedEventArgs e)
        {
            _mouseHook.OnMouseActivity -= new System.Windows.Forms.MouseEventHandler(mouseHook_OnMouseActivity);
        }

        private static System.Drawing.Color GetColorUnderCursor()
        {
            uint abgr = 0;

            POINT mousePoint;
            if (GetCursorPos(out mousePoint))
            {
                IntPtr desktopDC = GetDC(IntPtr.Zero);

                abgr = GetPixel(desktopDC, mousePoint.X, mousePoint.Y);

                ReleaseDC(IntPtr.Zero, desktopDC);
            }
            int r = (int)(abgr & 0x0000FF) >> 00;
            int g = (int)(abgr & 0x00FF00) >> 08;
            int b = (int)(abgr & 0xFF0000) >> 16;
            return System.Drawing.Color.FromArgb(r, g, b);

        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr windowHandle);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("gdi32.dll")]
        private static extern uint GetPixel(IntPtr hDC, int x, int y);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
    }
}