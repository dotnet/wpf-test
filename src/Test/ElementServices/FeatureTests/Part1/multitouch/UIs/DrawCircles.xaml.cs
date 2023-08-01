// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Test.Input.MultiTouch;

namespace Microsoft.Test.Input.MultiTouch.Tests
{
    /// <summary>
    /// 



    public partial class DrawCircles : Window
    {
        #region Private Fields

        Canvas _canv;        
        // draw related segment
        bool _isDrawing;
        Ellipse _elips;
        Point _ptCenter;
                       
        bool _isDragging;
        FrameworkElement _elDragging;
        Point _ptMouseStart,_ptElementStart;

        #endregion

        #region Constructor

        public DrawCircles()
        {
            Title = "DrawCircles";
            Content = _canv = new Canvas();
        }

        #endregion

        #region Events and Helpers

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs args)
        {
            base.OnMouseLeftButtonDown(args);
            if (_isDragging)
            {
                return;
            }

            // add a new ellipse
            _ptCenter = args.GetPosition(_canv);
            _elips = new Ellipse();
            _elips.Stroke = SystemColors.WindowTextBrush;
            _elips.StrokeThickness = 1;
            _elips.Width = 0;
            _elips.Height = 0;
            _canv.Children.Add(_elips);
            Canvas.SetLeft(_elips, _ptCenter.X);
            Canvas.SetTop(_elips, _ptCenter.Y);
                        
            CaptureMouse();
            _isDrawing = true;
        }
                
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs args)
        {
            base.OnMouseRightButtonDown(args);
            if (_isDrawing)
            {
                return;
            }
          
            _ptMouseStart = args.GetPosition(_canv);
            _elDragging = _canv.InputHitTest(_ptMouseStart) as FrameworkElement;
           
            if (_elDragging != null)
            {
                _ptElementStart = new Point(Canvas.GetLeft(_elDragging), Canvas.GetTop(_elDragging));
                _isDragging = true;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs args)
        {
            base.OnMouseDown(args);

            // press mouse button, and switch filling color
            if (args.ChangedButton == MouseButton.Middle)
            {
                Shape shape = _canv.InputHitTest(args.GetPosition(_canv)) as Shape;
                if (shape != null)
                {
                    shape.Fill = (shape.Fill == Brushes.Red ? Brushes.Transparent : Brushes.Red);
                }
            }
        }
                
        protected override void OnMouseMove(MouseEventArgs args)
        {
            base.OnMouseMove(args);
            Point ptMouse = args.GetPosition(_canv);
                    
            if (_isDrawing)
            {
                double dRadius = Math.Sqrt(Math.Pow(_ptCenter.X - ptMouse.X, 2) + 
                                           Math.Pow(_ptCenter.Y - ptMouse.Y, 2));
                Canvas.SetLeft(_elips, _ptCenter.X - dRadius);
                Canvas.SetTop(_elips, _ptCenter.Y - dRadius);
                _elips.Width = 2 * dRadius;
                _elips.Height = 2 * dRadius;
            }
            else if (_isDragging)  // move it
            {
                Canvas.SetLeft(_elDragging, _ptElementStart.X + ptMouse.X - _ptMouseStart.X);
                Canvas.SetTop(_elDragging, _ptElementStart.Y + ptMouse.Y - _ptMouseStart.Y);
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs args)
        {
            base.OnMouseUp(args);
                    
            if (_isDrawing && args.ChangedButton == MouseButton.Left)
            {
                _elips.Stroke = Brushes.Blue;
                _elips.StrokeThickness = Math.Min(20, _elips.Width / 2);
                _elips.Fill = Brushes.Red;
                _isDrawing = false;
                ReleaseMouseCapture();
            }        
            else if (_isDragging && args.ChangedButton == MouseButton.Right)
            {
                _isDragging = false;
            }
        }

        protected override void OnTextInput(TextCompositionEventArgs args)
        {
            base.OnTextInput(args);
                  
            // press Escape, done with drawing or drag
            if (args.Text.IndexOf('\x1B') != -1)
            {
                if (_isDrawing)
                {
                    ReleaseMouseCapture();
                }
                else if (_isDragging)
                {
                    Canvas.SetLeft(_elDragging, _ptElementStart.X);
                    Canvas.SetTop(_elDragging, _ptElementStart.Y);
                    _isDragging = false;
                }
            }
        }

        protected override void OnLostMouseCapture(MouseEventArgs args)
        {
            base.OnLostMouseCapture(args);
            
            // remove the ellipse
            if (_isDrawing)
            {
                _canv.Children.Remove(_elips);
                _isDrawing = false;
            }
        }

        #endregion

        private void TestWindow_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            // 
        }

        private void TestWindow_PreviewStylusMove(object sender, StylusEventArgs e)
        {
            // 
        }

        private void TestWindow_PreviewStylusUp(object sender, StylusEventArgs e)
        {

        }

        private void TestWindow_StylusDown(object sender, StylusDownEventArgs e)
        {

        }

        private void TestWindow_StylusMove(object sender, StylusEventArgs e)
        {

        }

        private void TestWindow_StylusEnter(object sender, StylusEventArgs e)
        {

        }

        private void TestWindow_StylusLeave(object sender, StylusEventArgs e)
        {

        }

        private void TestWindow_GotStylusCapture(object sender, StylusEventArgs e)
        {

        }

        private void TestWindow_LostStylusCapture(object sender, StylusEventArgs e)
        {

        }
    }
}
