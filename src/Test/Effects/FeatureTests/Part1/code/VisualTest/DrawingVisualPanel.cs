// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// A panel that hold DrawingVisuals. 
    /// </summary>
    public class DrawingVisualPanel : Panel
    {
        /// <summary>
        /// Constructor, create visuals collection. 
        /// </summary>
        public DrawingVisualPanel()
        {
            _visuals = new VisualCollection(this);
        }

        internal void AddVisual(DrawingVisual visual)
        {
            _visuals.Add(visual);
        }

        internal void RemoveVisual(DrawingVisual visual)
        {
            _visuals.Remove(visual);
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return _visuals.Count;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return _visuals[index];
        }

        /// <summary>
        /// Draw all the visuals. 
        /// </summary>
        /// <param name="drawingContext"></param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (null == _visuals)
            {
                return;
            }

            Rect rect = new Rect(new Point(0, 0), RenderSize);

            if (rect.IsEmpty)
            {
                return;
            }

            foreach (DrawingVisual v in _visuals)
            {
                CustomDrawingVisual customDrawingVisual = v as CustomDrawingVisual;
                if (customDrawingVisual != null)
                {
                    customDrawingVisual.Draw(rect);
                }
            }
        }

        private VisualCollection _visuals;
    }
}
