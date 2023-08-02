// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Limited Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/limitedpermissivelicense.mspx
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;

namespace WpfControlToolkit
{
    public class AnimatingTilePanel : Panel
    {

        public AnimatingTilePanel()
        {
            _compositionTarget_RenderingHandler = new EventHandler(CompositionTarget_Rendering);

            //need to make sure we only run the ticker when the control is actually loaded            
            this.Loaded += new RoutedEventHandler(AnimatingTilePanel_Loaded);
            this.Unloaded += new RoutedEventHandler(AnimatingTilePanel_Unloaded);
        }

        // Measures the children
        protected override Size MeasureOverride(Size availableSize)
        {
            double itemWidth = this.ItemWidth;
            double itemHeight = this.ItemHeight;
            Size theChildSize = new Size(itemWidth, itemHeight);

            UIElementCollection children = Children;
            int childrenCount = children.Count;
            for (int index = 0; index < childrenCount; index++)
            {
                UIElement child = children[index];
                child.Measure(theChildSize);
            }

            int childrenPerRow;

            // Figure out how many children fit on each row
            if (availableSize.Width == Double.PositiveInfinity)
                childrenPerRow = childrenCount;
            else
                childrenPerRow = Math.Max(1, (int)Math.Floor(availableSize.Width / itemWidth));

            // Calculate the width and height this results in
            double width = childrenPerRow * itemWidth;

            int rowCount = (int)Math.Ceiling((double)childrenCount / childrenPerRow);
            double height = itemHeight * rowCount;

            return new Size(width, height);
        }

        // Arrange the children
        protected override Size ArrangeOverride(Size finalSize)
        {
            double itemWidth = this.ItemWidth;
            double itemHeight = this.ItemHeight;

            // Calculate how many children fit on each row
            int childrenPerRow = Math.Max(1, (int)Math.Floor(finalSize.Width / itemWidth));

            Size theChildSize = new Size(itemWidth, itemHeight);
            UIElementCollection children = Children;
            int childrenCount = children.Count;
            for (int index = 0; index < childrenCount; index++)
            {
                UIElement child = children[index];

                // Figure out where the child goes
                Point newOffset = CalcChildOffset(index, childrenPerRow, itemWidth, itemHeight, finalSize.Width, childrenCount);

                //set the location attached DP
                SetChildTarget(child, newOffset);

                object locationLocalValue = child.ReadLocalValue(ChildLocationProperty);
                if (locationLocalValue == DependencyProperty.UnsetValue)
                {
                    SetChildLocation(child, newOffset);
                    child.Arrange(new Rect(newOffset, theChildSize));
                }
                else
                {
                    Point currentOffset = (Point)locationLocalValue;
                    // Position the child and set its size
                    child.Arrange(new Rect(currentOffset, theChildSize));
                }
            }
            return finalSize;
        }

        #region public properties
        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(AnimatingTilePanel),
            new FrameworkPropertyMetadata(50.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(AnimatingTilePanel),
            new FrameworkPropertyMetadata(50.0, FrameworkPropertyMetadataOptions.AffectsMeasure));


        public static readonly DependencyProperty DampeningProperty = DependencyProperty.Register(
            "Dampening", typeof(double), typeof(AnimatingTilePanel),
            new FrameworkPropertyMetadata(0.8, null, new CoerceValueCallback(OnCoerceDampening)));

        public double Dampening
        {
            get
            {
                return (double)GetValue(DampeningProperty);
            }
            set
            {
                SetValue(DampeningProperty, value);
            }
        }

        private static object OnCoerceDampening(DependencyObject target, object o)
        {
            double dampening = (double)o;
            if (dampening >= 1.0)
            {
                return 0.99;
            }
            else if (dampening < 0.0)
            {
                return 0.0;
            }

            return o;
        }

        public static readonly DependencyProperty AttractionProperty = DependencyProperty.Register(
            "Attraction", typeof(double), typeof(AnimatingTilePanel),
            new FrameworkPropertyMetadata(2.0, null, new CoerceValueCallback(OnCoerceAttraction)));

        public double Attraction
        {
            get
            {
                return (double)GetValue(AttractionProperty);
            }
            set
            {
                SetValue(AttractionProperty, value);
            }
        }

        private static object OnCoerceAttraction(DependencyObject target, object o)
        {
            double attraction = (double)o;
            if (attraction <= 0.0)
            {
                return 0.01;
            }

            return o;
        }

        #endregion

        #region private attached properties

        private static readonly DependencyProperty ChildLocationProperty
            = DependencyProperty.RegisterAttached("ChildLocation", typeof(Point), typeof(AnimatingTilePanel)
            , new FrameworkPropertyMetadata(new Point(), FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public static void SetChildLocation(UIElement element, Point point)
        {
            element.SetValue(ChildLocationProperty, point);
        }
        public static Point GetChildLocation(UIElement element)
        {
            return (Point)element.GetValue(ChildLocationProperty);
        }

        private static readonly DependencyProperty ChildTargetProperty
            = DependencyProperty.RegisterAttached("ChildTarget", typeof(Point), typeof(AnimatingTilePanel));

        public static void SetChildTarget(UIElement element, Point point)
        {
            element.SetValue(ChildTargetProperty, point);
        }
        public static Point GetChildTarget(UIElement element)
        {
            return (Point)element.GetValue(ChildTargetProperty);
        }

        private static readonly DependencyProperty VelocityProperty
            = DependencyProperty.RegisterAttached("Velocity", typeof(Vector), typeof(AnimatingTilePanel));

        public static void SetVelocity(UIElement element, Vector Vector)
        {
            element.SetValue(VelocityProperty, Vector);
        }
        public static Vector GetVelocity(UIElement element)
        {
            return (Vector)element.GetValue(VelocityProperty);
        }

        #endregion

        #region private methods

        private void AnimatingTilePanel_Loaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering += _compositionTarget_RenderingHandler;
        }
        private void AnimatingTilePanel_Unloaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering -= _compositionTarget_RenderingHandler;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            int nowTick = Environment.TickCount;
            int diff = nowTick - _lastTick;
            _lastTick = nowTick;

            double seconds = TimeSpan.FromMilliseconds(diff).TotalSeconds;

            double dampening = this.Dampening;
            double attractionFactor = this.Attraction;

            UIElementCollection children = Children;
            int count = children.Count;
            for (int index = 0; index < count; index++)
            {
                UIElement child = children[index];
                UpdateElement(child, seconds, dampening, attractionFactor);
            }
        }

        private static void UpdateElement(UIElement element, double seconds, double dampening, double attractionFactor)
        {
            Point current = GetChildLocation(element);
            Point target = GetChildTarget(element);
            Vector velocity = GetVelocity(element);

            Vector diff = target - current;

            if (diff.Length > Diff || velocity.Length > Diff)
            {
                velocity.X *= dampening;
                velocity.Y *= dampening;

                velocity += diff;

                Vector delta = velocity * seconds * attractionFactor;

                //velocity shouldn't be greater than...maxVelocity?
                double maxVelocity = 100;
                delta *= (delta.Length > maxVelocity) ? (maxVelocity / delta.Length) : 1;

                current += delta;

                SetChildLocation(element, current);
                SetVelocity(element, velocity);
            }
        }

        // Given a child index, child size and children per row, figure out where the child goes
        private static Point CalcChildOffset(int index, int childrenPerRow, double itemWidth, double itemHeight, double panelWidth, int totalChildren)
        {
            double fudge = 0;
            if (totalChildren > childrenPerRow)
            {
                fudge = (panelWidth - childrenPerRow * itemWidth) / childrenPerRow;
                Debug.Assert(fudge >= 0);
            }

            int row = index / childrenPerRow;
            int column = index % childrenPerRow;
            return new Point(.5 * fudge + column * (itemWidth + fudge), row * itemHeight);
        }

        #endregion

        private readonly EventHandler _compositionTarget_RenderingHandler;
        private int _lastTick = int.MinValue;
        private const double Diff = 0.1;

    }
}
