// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;

namespace Microsoft.Test.Input.MultiTouch.Tests
{
    /// <summary>
    /// custom visual host
    /// </summary>
    public class MyVisualHost : FrameworkElement
    {
        #region Constructor

        public MyVisualHost()
        {
            _children = new VisualCollection(this);
            _children.Add(CreateDrawingVisualRectangle());
            ContainerVisual cv = new ContainerVisual();
            _label = new Label();
            cv.Children.Add(_label);
            _children.Add(cv);

            // Add the touch event handler
            this.TouchDown += new EventHandler<TouchEventArgs>(MyVisualHost_TouchDown);
        }

        #endregion

        #region Event handler and helpers and overrides

        void MyVisualHost_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            e.TouchDevice.Capture(_label);

            _label.Content = string.Format("TouchesCapturedWithin={0}, TouchesOver={1}", this.TouchesCapturedWithin.ToString(), this.TouchesOver.ToString());
        }

        /// <summary>
        /// create visual
        /// </summary>
        /// <returns></returns>
        private DrawingVisual CreateDrawingVisualRectangle()
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            Rect rect = new Rect(new System.Windows.Point(5, 5), new System.Windows.Size(120, 120));
            drawingContext.DrawRectangle(System.Windows.Media.Brushes.LightBlue, (System.Windows.Media.Pen)null, rect);
            drawingContext.Close();

            return drawingVisual;
        }

        protected override int VisualChildrenCount
        {
            get { return _children.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _children[index];
        }

        #endregion 

        #region Private Fields

        private VisualCollection _children;
        private Label _label;

        #endregion
    }
}
