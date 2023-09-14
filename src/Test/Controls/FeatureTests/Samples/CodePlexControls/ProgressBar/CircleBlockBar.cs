using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Markup;
using System.ComponentModel;
using System.Diagnostics;

namespace WpfControlToolkit
{
    public class CircleBlockBar : BlockBar
    {

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            double penThickness = ThePen.Thickness;
            Size effectiveRenderSize = new Size(this.RenderSize.Width - penThickness, this.RenderSize.Height - penThickness);

            //two possible scenarios
            //1) we are driven by width
            //2) we are driven by height
            //let's run both to see which works

            double circleDiameter;
            circleDiameter = (effectiveRenderSize.Width - (BlockCount - 1) * BlockMargin) / BlockCount;
            if (circleDiameter > effectiveRenderSize.Height)
            {
                circleDiameter = effectiveRenderSize.Height;
            }

            double startLeft = penThickness/2 + effectiveRenderSize.Width - (this.BlockCount * circleDiameter + (this.BlockCount - 1) * BlockMargin);
            double startTop = penThickness/2 + (effectiveRenderSize.Height - circleDiameter) / 2;

            double circleRadius = circleDiameter / 2;
            Point center = new Point();

            int threshHold = BlockBar.GetThreshold(this.Value, this.BlockCount);

            Brush brushToUse;
            for (int i = 0; i < this.BlockCount; i++)
            {
                brushToUse = ((this.BlockCount - (i + 1)) < threshHold) ? this.EnabledBrush : this.DisabledBrush;

                center.X = startLeft + circleRadius;
                center.Y = startTop + circleRadius;
                drawingContext.DrawEllipse(brushToUse, ThePen, center, circleRadius, circleRadius);
                startLeft += circleDiameter + BlockMargin;
            }
        }
    }
}
