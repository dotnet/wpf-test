// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Test.Input.MultiTouch.Tests
{
    /// <summary>
    /// 

    public class MultiPaintCanvas : Canvas
    {
        class CustomStrokeInfo
        {
            public TouchPoint LastPoint { get; set; }
            public Color Color { get; set; }
        }

        Dictionary<TouchDevice, CustomStrokeInfo> _StrokeInfos = new Dictionary<TouchDevice, CustomStrokeInfo>();

        public MultiPaintCanvas()
        {
            this.TouchDown += new EventHandler<TouchEventArgs>(OnTouchDown);
            this.TouchMove += new EventHandler<TouchEventArgs>(OnTouchMove);
            this.TouchUp += new EventHandler<TouchEventArgs>(OnTouchUp); 
        }

        protected void OnTouchDown(object sender, TouchEventArgs e)
        {
            var touchPoint = e.GetTouchPoint(this);
            var color = GetRandomColor();
            e.TouchDevice.Capture(this);
            _StrokeInfos[e.TouchDevice] = new CustomStrokeInfo { LastPoint = touchPoint, Color = color };
        }

        protected void OnTouchMove(object sender, TouchEventArgs e)
        {
            AddStroke(e);
            _StrokeInfos[e.TouchDevice].LastPoint = e.GetTouchPoint(this); 
        }

        protected void OnTouchUp(object sender, TouchEventArgs e)
        {
            AddStroke(e);
            e.TouchDevice.Capture(null);
            _StrokeInfos.Remove(e.TouchDevice);
        }

        private void AddStroke(TouchEventArgs e)
        {
            var touchPoint = e.GetTouchPoint(this);
            var strokeInfo = _StrokeInfos[e.TouchDevice];
            DrawLine(strokeInfo.LastPoint, touchPoint, strokeInfo.Color);
        }

        private void DrawLine(TouchPoint lastPoint, TouchPoint touchPoint, Color color)
        {
            // 
        }

        private Color GetRandomColor()
        {
            //
            return Colors.AliceBlue;
        }

    }

}
