using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WpfControlToolkit
{
    public class AnimatedStackPanel : Panel
    {
        /// <summary>
        /// Specifies dimension of children stacking.
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for <see cref="Orientation" /> property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = StackPanel.OrientationProperty.AddOwner(typeof(AnimatedStackPanel));

        /// <summary>
        ///     This property is always true because this panel has vertical or horizontal orientation
        /// </summary>
        protected override bool HasLogicalOrientation
        {
            get { return true; }
        }

        /// <summary>
        ///     Orientation of the panel if its layout is in one dimension.
        ///     Otherwise HasLogicalOrientation is false and LogicalOrientation should be ignored
        /// </summary>
        protected override Orientation LogicalOrientation
        {
            get { return this.Orientation; }
        }

        /// <summary>
        ///     General StackPanel layout behavior is to grow unbounded in the "stacking" direction (Size To Content).
        ///     Children in this dimension are encouraged to be as large as they like.  In the other dimension,
        ///     StackPanel will assume the maximum size of its children.
        /// </summary>
        /// <param name="constraint">Constraint</param>
        /// <returns>Desired size</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            Size desiredSize = new Size();
            UIElementCollection children = InternalChildren;

            if (children != null)
            {
                bool vertical = Orientation == Orientation.Vertical;
                Size slot = new Size(vertical ? constraint.Width : Double.PositiveInfinity,
                                     vertical ? Double.PositiveInfinity : constraint.Height);

                int removedIndex = 0;
                int visualIndex = 0;
                int totalCount = children.Count + ((_removedItems != null) ? _removedItems.Count : 0);
                for (int totalIndex = 0; totalIndex < totalCount; ++totalIndex)
                {
                    Size childDesiredSize;

                    if ((_removedItems != null) && 
                        (removedIndex < _removedItems.Count) && 
                        (_removedItems[removedIndex].Index == totalIndex))
                    {
                        childDesiredSize = _removedItems[removedIndex++].Size;
                    }
                    else
                    {
                        // Get the next child.
                        UIElement child = children[visualIndex++];
                        if (child != null)
                        {
                            // Measure the child.
                            child.Measure(slot);
                            childDesiredSize = child.DesiredSize;
                        }
                        else
                        {
                            childDesiredSize = new Size();
                        }
                    }

                    // Accumulate child size.
                    if (vertical)
                    {
                        desiredSize.Width = Math.Max(desiredSize.Width, childDesiredSize.Width);
                        desiredSize.Height += childDesiredSize.Height;
                    }
                    else
                    {
                        desiredSize.Width += childDesiredSize.Width;
                        desiredSize.Height = Math.Max(desiredSize.Height, childDesiredSize.Height);
                    }
                }
            }

            return desiredSize;
        }

        /// <summary>
        ///     Content arrangement.
        /// </summary>
        /// <param name="arrangeSize">Arrange size</param>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            UIElementCollection children = InternalChildren;

            if (children != null)
            {
                bool vertical = Orientation == Orientation.Vertical;
                Rect childRect = new Rect(arrangeSize);
                double previousChildSize = 0.0;

                // Arrange and Position Children.
                int removedIndex = 0;
                int visualIndex = 0;
                int totalCount = children.Count + ((_removedItems != null) ? _removedItems.Count : 0);
                double currentProgress = AnimationProgress;
                for (int totalIndex = 0; totalIndex < totalCount; ++totalIndex)
                {
                    if ((_removedItems != null) &&
                        (removedIndex < _removedItems.Count) &&
                        (_removedItems[removedIndex].Index == totalIndex))
                    {
                        RemovedItem item = _removedItems[removedIndex];
                        double itemProgress = currentProgress - item.AnimationStart;
                        double sizeReduction = CalculateSizeReductionPercent(itemProgress);
                        if (vertical)
                        {
                            childRect.Y += previousChildSize;
                            previousChildSize = childRect.Height = item.Size.Height * sizeReduction;
                            childRect.Width = Math.Max(arrangeSize.Width, item.Size.Width);
                        }
                        else
                        {
                            childRect.X += previousChildSize;
                            previousChildSize = childRect.Width = item.Size.Width * sizeReduction;
                            childRect.Height = Math.Max(arrangeSize.Height, item.Size.Height);
                        }
                        item.Position = childRect.TopLeft;
                        _removedItems[removedIndex++] = item;
                    }
                    else
                    {
                        UIElement child = children[visualIndex++];

                        if (child != null)
                        {
                            double sizeReduction = 1.0;
                            if (_addedItems != null)
                            {
                                double itemProgress;
                                if (_addedItems.TryGetValue(child, out itemProgress))
                                {
                                    itemProgress = currentProgress - itemProgress;
                                    sizeReduction = CalculateSizeExpansionPercent(itemProgress);
                                }
                            }
                            if (vertical)
                            {
                                childRect.Y += previousChildSize;
                                previousChildSize = childRect.Height = child.DesiredSize.Height;
                                childRect.Width = Math.Max(arrangeSize.Width, child.DesiredSize.Width);
                            }
                            else
                            {
                                childRect.X += previousChildSize;
                                previousChildSize = childRect.Width = child.DesiredSize.Width;
                                childRect.Height = Math.Max(arrangeSize.Height, child.DesiredSize.Height);
                            }
                            previousChildSize *= sizeReduction;

                            child.Arrange(childRect);
                        }
                    }
                }
            }

            return arrangeSize;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (_removedItems != null)
            {
                for (int i = 0; i < _removedItems.Count; i++)
                {
                    RemovedItem item = _removedItems[i];

                    // 


                    drawingContext.DrawRectangle(item.Brush, null, 
                        new Rect(item.Position.X, item.Position.Y, 
                        item.Bitmap.PixelWidth, item.Bitmap.PixelHeight));
                }
            }
        }

        protected override void OnIsItemsHostChanged(bool oldIsItemsHost, bool newIsItemsHost)
        {
            // The items host has changed, hook up our listener
            if ((_itemContainerGenerator == null) && newIsItemsHost)
            {
                ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
                if (itemsControl != null)
                {
                    _itemContainerGenerator = itemsControl.ItemContainerGenerator;
                    if (_itemContainerGenerator != null)
                    {
                        // It is very important that this handler be called first
                        _itemContainerGenerator.ItemsChanged += new ItemsChangedEventHandler(OnBeforeItemsChanged);
                        Dispatcher.BeginInvoke(DispatcherPriority.Normal, 
                            new DispatcherOperationCallback(delegate(object param)
                            {
                                _itemContainerGenerator.ItemsChanged += new ItemsChangedEventHandler(OnAfterItemsChanged);
                                return null;
                            }), 
                            null
                        );
                    }
                }
            }
            else if ((_itemContainerGenerator != null) && !newIsItemsHost)
            {
                _itemContainerGenerator.ItemsChanged -= new ItemsChangedEventHandler(OnBeforeItemsChanged);
                _itemContainerGenerator.ItemsChanged -= new ItemsChangedEventHandler(OnAfterItemsChanged);
                _itemContainerGenerator = null;
            }

            base.OnIsItemsHostChanged(oldIsItemsHost, newIsItemsHost);
        }

        private void OnBeforeItemsChanged(object sender, ItemsChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    OnBeforeItemsAdded(e.Position.Index, e.ItemCount);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    OnBeforeItemsRemoved(e.Position.Index, e.ItemUICount);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    // 
                    break;

                case NotifyCollectionChangedAction.Move:
                    // 
                    break;

                case NotifyCollectionChangedAction.Reset:
                    // 
                    break;
            }
        }

        private void OnAfterItemsChanged(object sender, ItemsChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    OnAfterItemsAdded(e.Position.Index, e.ItemCount);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    // 
                    break;

                case NotifyCollectionChangedAction.Move:
                    // 
                    break;

                case NotifyCollectionChangedAction.Reset:
                    // 
                    break;
            }
        }

        private void OnBeforeItemsAdded(int startIndex, int itemCount)
        {
            if (_removedItems != null)
            {
                // Adjust the indexes of the removed items to account for the newly added ones
                int endIndex = startIndex + itemCount - 1;

                int numRemoved = _removedItems.Count;
                for (int i = 0; (i < numRemoved); i++)
                {
                    RemovedItem item = _removedItems[i];

                    // Add the accumulated offset
                    if (item.Index < startIndex)
                    {
                        startIndex++;
                        endIndex++;
                    }
                    else if ((startIndex <= item.Index) && (item.Index <= endIndex))
                    {
                        item.Index = endIndex + (item.Index - startIndex) + 1;
                    }
                    else if (endIndex < item.Index)
                    {
                        item.Index += itemCount;
                    }

                    _removedItems[i] = item;
                }

                InvalidateArrange();
                InvalidateVisual();
            }
        }

        private void OnAfterItemsAdded(int startIndex, int itemCount)
        {
            if (_addedItems == null)
            {
                _addedItems = new Dictionary<UIElement, double>();
            }

            UIElementCollection children = InternalChildren;
            double animationProgress = (_animation != null) ? AnimationProgress : 0.0;
            for (int i = 0; i < itemCount; i++)
            {
                UIElement child = children[i + 1 + startIndex];
                _addedItems.Add(child, animationProgress);

                DoubleAnimationUsingKeyFrames doubleAnimation = new DoubleAnimationUsingKeyFrames();

                doubleAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.Zero)));
                doubleAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(333.3333))));
                doubleAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(child.Opacity, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(500.0))));

                child.BeginAnimation(UIElement.OpacityProperty, doubleAnimation, HandoffBehavior.SnapshotAndReplace);
            }

            StartAnimation();
        }

        private void OnBeforeItemsRemoved(int startIndex, int itemUICount)
        {
            int endIndex = startIndex + itemUICount - 1;
            for (int i = startIndex; i <= endIndex; i++)
            {
                UIElement child = InternalChildren[i];
                int expandedIndex = ConvertIndex(i);

                // 



                int width = (int)child.RenderSize.Width;
                int height = (int)child.RenderSize.Height;
                RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Default);
                child.Arrange(new Rect(child.RenderSize));
                rtb.Render(child);
                rtb.Freeze();

                ImageBrush brush = new ImageBrush(rtb);
                DoubleAnimation animation = new DoubleAnimation();
                animation.From = 1.0;
                animation.To = 0.0;
                animation.Duration = TimeSpan.FromMilliseconds(150.0);
                animation.FillBehavior = FillBehavior.HoldEnd;
                brush.BeginAnimation(ImageBrush.OpacityProperty, animation);

                double animationStart = (_animation != null) ? AnimationProgress : 0.0;

                RemovedItem removedItem = new RemovedItem(expandedIndex, rtb, brush, child.RenderSize, animationStart);
                if (_removedItems == null)
                {
                    _removedItems = new List<RemovedItem>();
                }
                int removedIndex = 0;
                for (; removedIndex < _removedItems.Count; removedIndex++)
                {
                    if (_removedItems[removedIndex].Index > expandedIndex)
                    {
                        _removedItems.Insert(removedIndex, removedItem);
                        break;
                    }
                }
                if (removedIndex == _removedItems.Count)
                {
                    _removedItems.Add(removedItem);
                }
            }

            StartAnimation();
            InvalidateVisual();
        }

        private int ConvertIndex(int index)
        {
            if (_removedItems != null)
            {
                int count = _removedItems.Count;
                for (int i = 0; i < count; i++)
                {
                    if (index >= _removedItems[i].Index)
                    {
                        index++;
                    }
                    else
                    {
                        // Since the list is sorted, index cannot be 
                        // greater or equal to anything else in the list.
                        break;
                    }
                }
            }

            return index;
        }

        /// <summary>
        ///     Value between 0 and 1 (inclusive) to move the animation along.
        /// </summary>
        private double AnimationProgress
        {
            get { return (double)GetValue(AnimationProgressProperty); }
            set { SetValue(AnimationProgressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AnimationProgress.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty AnimationProgressProperty =
            DependencyProperty.Register("AnimationProgress", typeof(double), typeof(AnimatedStackPanel), new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnAnimationProgressChanged)));

        private static void OnAnimationProgressChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ((AnimatedStackPanel)sender).OnAnimationProgressChanged(e);
        }

        private void OnAnimationProgressChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((_removedItems != null) && (_removedItems.Count > 0))
            {
                double currentProgress = AnimationProgress;
                int indexOffset = 0;
                for (int i = 0; i < _removedItems.Count; i++)
                {
                    RemovedItem item = _removedItems[i];
                    if (indexOffset != 0)
                    {
                        item.Index -= indexOffset;
                        _removedItems[i] = item;
                    }

                    double itemProgress = currentProgress - item.AnimationStart;
                    double sizeReduction = CalculateSizeReductionPercent(itemProgress);
                    if (sizeReduction < 1.0) // "time to animate smaller"
                    {
                        InvalidateArrange();
                        InvalidateVisual();

                        if (sizeReduction < 0.00001) // "zero" - remove the item
                        {
                            indexOffset++;
                            _removedItems.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }

            if ((_addedItems != null) && (_addedItems.Count > 0))
            {
                double currentProgress = AnimationProgress;
                UIElement[] keys = new UIElement[_addedItems.Keys.Count];
                _addedItems.Keys.CopyTo(keys, 0);
                for (int i = 0; i < keys.Length; i++)
                {
                    UIElement child = keys[i];
                    double itemProgress = currentProgress - _addedItems[child];
                    if (itemProgress > 0.0) // "time to animation larger"
                    {
                        InvalidateArrange();
                    }

                    if (itemProgress > 0.9999) // "one" - remove the item
                    {
                        _addedItems.Remove(child);
                    }
                }
            }

            if (((_removedItems == null) || (_removedItems.Count == 0)) &&
                ((_addedItems == null) || (_addedItems.Count == 0)))
            {
                _animation = null;
            }
        }

        private void StartAnimation()
        {
            if (_animation == null)
            {
                // Start a new animation
                _animation = new DoubleAnimation();
                _animation.From = 0.0;
                _animation.To = 1.0;
                _animation.Duration = TimeSpan.FromMilliseconds(500.0);
                _animation.FillBehavior = FillBehavior.HoldEnd;
            }
            else
            {
                // Update the animation
                double currentProgress = AnimationProgress;
                _animation.From = currentProgress;
                _animation.To = currentProgress + 1.0;
            }

            BeginAnimation(AnimationProgressProperty, _animation);
        }

        private static double CalculateSizeReductionPercent(double itemProgress)
        {
            // Second two-thirds is the slide
            // (1-progress) / (2/3)
            return Math.Max(0.0, Math.Min(1.0, (1.0 - itemProgress) * 1.5));
        }

        private static double CalculateSizeExpansionPercent(double itemProgress)
        {
            // First two-thirds is the slide
            // progress / (2/3)
            return Math.Max(0.0, Math.Min(1.0, itemProgress * 1.5));
        }

        ItemContainerGenerator _itemContainerGenerator;
        private List<RemovedItem> _removedItems;
        private Dictionary<UIElement, double> _addedItems;
        private DoubleAnimation _animation;

        private struct RemovedItem
        {
            public RemovedItem(int index, RenderTargetBitmap rtb, ImageBrush brush, Size size, double animationStart)
            {
                _index = index;
                _rtb = rtb;
                _brush = brush;
                _size = size;
                _pos = new Point();
                _animationStart = animationStart;
            }

            public int Index
            {
                get { return _index; }
                set { _index = value; }
            }

            public Size Size
            {
                get { return _size; }
            }

            public Point Position
            {
                get { return _pos; }
                set { _pos = value; }
            }

            public RenderTargetBitmap Bitmap
            {
                get { return _rtb; }
            }

            public ImageBrush Brush
            {
                get { return _brush; }
            }

            public double AnimationStart
            {
                get { return _animationStart; }
            }

            private int _index;
            private Size _size;
            private Point _pos;
            private ImageBrush _brush;
            private RenderTargetBitmap _rtb;
            private double _animationStart;
        }
    }
}
