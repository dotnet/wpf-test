using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Markup;
using System.ComponentModel;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Automation;
using System.Diagnostics;

namespace WpfControlToolkit
{
    public class RectBlockBar : BlockBar
    {
        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            Rect rect;
            int blockCount = BlockCount;
            Size renderSize = this.RenderSize;
            double blockMargin = this.BlockMargin;
            double value = Value;
            for (int i = 0; i < blockCount; i++)
            {
                rect = GetRect(renderSize, blockCount, blockMargin, i, ThePen.Thickness);

                if (!rect.IsEmpty)
                {
                    int threshold = GetThreshold(value, blockCount);
                    drawingContext.DrawRectangle((i < threshold) ? EnabledBrush : DisabledBrush, ThePen, rect);
                }
            }
        }


        private static Rect GetRect(Size targetSize, int blockCount, double blockMargin, int blockNumber, double penThickness)
        {
            if (targetSize.IsEmpty)
            {
                throw new ArgumentNullException();
            }
            if (blockCount < 1)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (blockNumber >= blockCount)
            {
                throw new ArgumentOutOfRangeException();
            }

            targetSize.Width -= penThickness;

            double width = (targetSize.Width - (blockCount - 1) * blockMargin) / blockCount;
            double left = penThickness / 2 + (width + blockMargin) * blockNumber;
            double height = targetSize.Height - penThickness;

            if (width > 0 && height > 0)
            {
                return new Rect(left, penThickness / 2, width, height);
            }
            else
            {
                return Rect.Empty;
            }
        }
    }
}
