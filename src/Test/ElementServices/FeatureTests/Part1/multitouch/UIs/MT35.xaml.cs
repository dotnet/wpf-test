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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Microsoft.Test.Input.MultiTouch;

namespace Microsoft.Test.Input.MultiTouch.Tests
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MT35 : Window
    {
        [DllImport("User32.dll", BestFitMapping = false, CharSet = CharSet.Auto)]
        //public static extern bool SetProp(HandleRef hWnd, string propName, HandleRef data);
        public static extern bool SetProp(IntPtr hWnd, string lpString, IntPtr hData);
 
        public MT35()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(
               delegate(object sender, RoutedEventArgs args)
               {
                   var source = new WindowInteropHelper(this);
                   SetProp(source.Handle,"MicrosoftTabletPenServiceProperty", new IntPtr(0x01000000));
               }
            );

            this.StylusDown += new StylusDownEventHandler(Window_StylusDown);
            this.StylusMove += new StylusEventHandler(Window_StylusMove);
            this.StylusUp += new StylusEventHandler(Window_StylusUp);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Results.Items.Clear();
        }

        private void Window_StylusDown(object sender, StylusDownEventArgs e)
        {
            int width = -1;
            int height = -1;
            var spc = e.GetStylusPoints(this);
            if (spc.Count > 0)
            {
                StylusPoint sPoint = spc[0];
                if (sPoint.HasProperty(StylusPointProperties.Width))
                {
                    width = sPoint.GetPropertyValue(StylusPointProperties.Width);
                }

                if (sPoint.HasProperty(StylusPointProperties.Height))
                {
                    height = sPoint.GetPropertyValue(StylusPointProperties.Height);
                }
            }

            AddResult(String.Format("StylusDown {0} {1} {2},{3}", ((StylusDevice)e.Device).TabletDevice.Type, e.StylusDevice.Id, width, height));

            e.StylusDevice.Capture(sender as FrameworkElement);
         }

        private void Window_StylusUp(object sender, StylusEventArgs e)
        {
            AddResult(String.Format("StylusUp {0} {1}", ((StylusDevice)e.Device).TabletDevice.Type, e.StylusDevice.Id));
            e.StylusDevice.Capture(null);
         }

        private void Window_StylusMove(object sender, StylusEventArgs e)
        {
            AddResult(String.Format("StylusMove {0} {1} Position=({2},{3})", ((StylusDevice)e.Device).TabletDevice.Type, e.StylusDevice.Id, e.GetPosition(this).X, e.GetPosition(this).Y));

            TextBlock tb = new TextBlock();
            tb.Text = "Stylus ID=" + e.StylusDevice.Id;
            dragCanvas.Children.Add(tb);
            e.StylusDevice.Capture(tb);

            _textBlocks[e.StylusDevice.Id] = tb;

            tb.StylusUp += new StylusEventHandler(tb_StylusUp);
            tb.StylusMove += new StylusEventHandler(tb_StylusMove);
        }

        private Dictionary<int, TextBlock> _textBlocks = new Dictionary<int, TextBlock>();

        void tb_StylusMove(object sender, StylusEventArgs e)
        {
            Point p = e.GetPosition(dragCanvas);
            var textBlock = _textBlocks[e.StylusDevice.Id];

            Canvas.SetLeft(textBlock, p.X);
            Canvas.SetTop(textBlock, p.Y);

            if (textBlock != sender)
            {
                var target = e.StylusDevice.Target as TextBlock;
                string s = target != null ? target.Text : "?";
                AddResult(String.Format("Mismatch {0} {1} {2}", e.StylusDevice.Id, ((TextBlock)sender).Text, s));
            }
        }

        void tb_StylusUp(object sender, StylusEventArgs e)
        {
            AddResult(String.Format("TB Up {0}", e.StylusDevice.Id));

            e.StylusDevice.Capture(null);

            var textBlock = _textBlocks[e.StylusDevice.Id];
            if (dragCanvas.Children.Contains(textBlock))
            {
                dragCanvas.Children.Remove(textBlock);
            }
        }

        private void AddResult(string s)
        {
            if (Results.Items.Add(s) > 25)
            {
                Results.Items.RemoveAt(0);
            }
        }

    }
}
