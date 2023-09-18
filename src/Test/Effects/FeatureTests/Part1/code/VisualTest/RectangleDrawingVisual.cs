// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Microsoft.Test.Effects
{
    public class RectangleDrawingVisual : CustomDrawingVisual
    {
        public RectangleDrawingVisual(Brush fill, Pen pen, System.Windows.Rect area)
        {
            this._fill = fill;
            this._pen = pen;
            this._area = area;
        }

        internal override void Draw(System.Windows.Rect rect)
        {
            using (DrawingContext dc = RenderOpen())
            {
                dc.DrawRectangle(_fill, _pen, _area);
            }
        }

        private Brush _fill;
        private Pen _pen;
        private System.Windows.Rect _area;
    }
}
