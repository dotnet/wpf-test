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
    public abstract class BlockBar : FrameworkElement
    {
        static BlockBar()
        {
            BlockBar.MinHeightProperty.OverrideMetadata(typeof(BlockBar), new FrameworkPropertyMetadata((double)10));
            BlockBar.MinWidthProperty.OverrideMetadata(typeof(BlockBar), new FrameworkPropertyMetadata((double)10));
            BlockBar.ClipToBoundsProperty.OverrideMetadata(typeof(BlockBar), new FrameworkPropertyMetadata(true));
        }

        public BlockBar()
        {
            _thePen = new Pen(this._rectBrush, 4);
        }

        public static readonly DependencyProperty BlockCountProperty =
            DependencyProperty.Register("BlockCount", typeof(int), typeof(BlockBar),
            new FrameworkPropertyMetadata((int)5, FrameworkPropertyMetadataOptions.AffectsRender,
                null,
                new CoerceValueCallback(CoerceBlockCount))
            );

        public int BlockCount
        {
            get { return (int)GetValue(BlockCountProperty); }
            set { SetValue(BlockCountProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(BlockBar),
            new FrameworkPropertyMetadata((double)0,
                FrameworkPropertyMetadataOptions.AffectsRender,
                null,
                new CoerceValueCallback(CoerceValue))
            );

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty BlockMarginProperty =
            DependencyProperty.Register("BlockMargin", typeof(double), typeof(BlockBar),
            new FrameworkPropertyMetadata((double)0, FrameworkPropertyMetadataOptions.AffectsRender,
                null,
                new CoerceValueCallback(CoerceBlockMargin))
            );

        public double BlockMargin
        {
            get { return (double)GetValue(BlockMarginProperty); }
            set { SetValue(BlockMarginProperty, BlockMargin); }
        }


        public Brush EnabledBrush
        {
            get
            {
                return _rectBrush;
            }
        }
        public Brush DisabledBrush
        {
            get
            {
                return _disabledBrush;
            }
        }
        public Pen ThePen
        {
            get { return _thePen; }
        }

        protected static int GetThreshold(double value, int blockCount)
        {
            if (value < 0 || value > 1)
            {
                throw new ArgumentOutOfRangeException("value");
            }
            if (blockCount < 1)
            {
                throw new ArgumentOutOfRangeException("blockCount");
            }

            int blockNumber = Math.Min((int)(value * (blockCount + 1)), blockCount);

            Debug.Assert(blockNumber <= blockCount && blockNumber >= 0);

            return blockNumber;
        }

        private static object CoerceValue(DependencyObject element, object value)
        {
            return ((BlockBar)element).CoerceValue((double)value);
        }

        private double CoerceValue(double input)
        {
            if (input < 0 || double.IsNaN(input))
            {
                return 0;
            }
            else if (input > 1)
            {
                return 1;
            }
            else
            {
                return input;
            }
        }


        private static object CoerceBlockCount(DependencyObject element, object value)
        {
            return ((BlockBar)element).CoerceBlockCount((int)value);
        }

        private int CoerceBlockCount(int input)
        {
            if (input < 1)
            {
                return 1;
            }
            else
            {
                return input;
            }
        }

        private static object CoerceBlockMargin(DependencyObject element, object value)
        {
            return ((BlockBar)element).CoerceBlockMargin((double)value);
        }

        private double CoerceBlockMargin(double input)
        {
            if (input < 0 || double.IsNaN(input) || double.IsInfinity(input))
            {
                return 0;
            }
            else
            {
                return input;
            }
        }

        private readonly Brush _rectBrush = Brushes.Navy;
        private readonly Brush _disabledBrush = Brushes.Transparent;
        private readonly Pen _thePen;
    }
}
