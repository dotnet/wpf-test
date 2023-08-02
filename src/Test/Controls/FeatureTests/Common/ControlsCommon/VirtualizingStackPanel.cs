//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
// Building and using VirtualizingStackPanel outside of WCP to make sure
// VirtualizingPanel abstract base class can be used to create a control
// like VirtualizingStackPanel by third parties
//
//---------------------------------------------------------------------------

//#define Profiling

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using Avalon.Test.ComponentModel.Utilities;
using System.Windows.Media.Media3D;

namespace Avalon.Test.ComponentModel.CustomControls
{
    /// <summary>
    /// VirtualizingStackPanel is used to arrange children into single line.
    /// </summary>
    public class VirtualizingStackPanel : VirtualizingPanel, IScrollInfo
    {
        //-------------------------------------------------------------------
        //
        //  Constructors
        //
        //-------------------------------------------------------------------

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public VirtualizingStackPanel()
        {
        }

        #endregion Constructors

        //-------------------------------------------------------------------
        //
        //  Public Methods
        //
        //-------------------------------------------------------------------

        #region Public Methods

        //-----------------------------------------------------------
        //  IScrollInfo Methods
        //-----------------------------------------------------------
        #region IScrollInfo Methods

        /// <summary>
        /// Scroll content by one line to the top.
        /// </summary>
        public void LineUp()
        {
            SetVerticalOffset(VerticalOffset - ((Orientation == Orientation.Vertical) ? 1.0 : _scrollLineDelta));
        }

        /// <summary>
        /// Scroll content by one line to the bottom.
        /// </summary>
        public void LineDown()
        {
            SetVerticalOffset(VerticalOffset + ((Orientation == Orientation.Vertical) ? 1.0 : _scrollLineDelta));
        }

        /// <summary>
        /// Scroll content by one line to the left.
        /// </summary>
        public void LineLeft()
        {
            SetHorizontalOffset(HorizontalOffset - ((Orientation == Orientation.Horizontal) ? 1.0 : _scrollLineDelta));
        }

        /// <summary>
        /// Scroll content by one line to the right.
        /// </summary>
        public void LineRight()
        {
            SetHorizontalOffset(HorizontalOffset + ((Orientation == Orientation.Horizontal) ? 1.0 : _scrollLineDelta));
        }

        /// <summary>
        /// Scroll content by one page to the top.
        /// </summary>
        public void PageUp()
        {
            SetVerticalOffset(VerticalOffset - ViewportHeight);
        }

        /// <summary>
        /// Scroll content by one page to the bottom.
        /// </summary>
        public void PageDown()
        {
            SetVerticalOffset(VerticalOffset + ViewportHeight);
        }

        /// <summary>
        /// Scroll content by one page to the left.
        /// </summary>
        public void PageLeft()
        {
            SetHorizontalOffset(HorizontalOffset - ViewportWidth);
        }

        /// <summary>
        /// Scroll content by one page to the right.
        /// </summary>
        public void PageRight()
        {
            SetHorizontalOffset(HorizontalOffset + ViewportWidth);
        }

        /// <summary>
        /// Scroll content by one page to the top.
        /// </summary>
        public void MouseWheelUp()
        {
            SetVerticalOffset(VerticalOffset - 3.0 * ((Orientation == Orientation.Vertical) ? 1.0 : _scrollLineDelta));
        }

        /// <summary>
        /// Scroll content by one page to the bottom.
        /// </summary>
        public void MouseWheelDown()
        {
            SetVerticalOffset(VerticalOffset + 3.0 * ((Orientation == Orientation.Vertical) ? 1.0 : _scrollLineDelta));
        }

        /// <summary>
        /// Scroll content by one page to the left.
        /// </summary>
        public void MouseWheelLeft()
        {
            SetHorizontalOffset(HorizontalOffset - 3.0 * ((Orientation == Orientation.Horizontal) ? 1.0 : _scrollLineDelta));
        }

        /// <summary>
        /// Scroll content by one page to the right.
        /// </summary>
        public void MouseWheelRight()
        {
            SetHorizontalOffset(HorizontalOffset + 3.0 * ((Orientation == Orientation.Horizontal) ? 1.0 : _scrollLineDelta));
        }

        /// <summary>
        /// Set the HorizontalOffset to the passed value.
        /// </summary>
        public void SetHorizontalOffset(double offset)
        {
            EnsureScrollData();
            double scrollX = ValidateInputOffset(offset, "HorizontalOffset");
            if (!DoubleUtil.AreClose(scrollX, _scrollData._offset.X))
            {
                _scrollData._offset.X = scrollX;
                InvalidateMeasure();
            }
        }

        /// <summary>
        /// Set the VerticalOffset to the passed value.
        /// </summary>
        public void SetVerticalOffset(double offset)
        {
            EnsureScrollData();
            double scrollY = ValidateInputOffset(offset, "VerticalOffset");
            if (!DoubleUtil.AreClose(scrollY, _scrollData._offset.Y))
            {
                _scrollData._offset.Y = scrollY;
                InvalidateMeasure();
            }
        }

        /// <summary>
        /// VirtualizingStackPanel implementation of <seealso cref="IScrollInfo.MakeVisible" />.
        /// </summary>
        // The goal is to change offsets to bring the child into view, and return a rectangle in our space to make visible.
        // The rectangle we return is in the physical dimension the input target rect transformed into our pace.
        // In the logical dimension, it is our immediate child's rect.
        // Note: This code presently assumes we/children are layout clean.
        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            Vector newOffset = new Vector();
            Rect newRect = new Rect();

            // We can only work on visuals that are us or children.
            // An empty rect has no size or position.  We can't meaningfully use it.
            if (    rectangle.IsEmpty
                || visual == null
                || visual == (Visual)this
                ||  !this.IsAncestorOf(visual))
            {
                return Rect.Empty;
            }

            // Compute the child's rect relative to (0,0) in our coordinate space.
            GeneralTransform childTransform = visual.TransformToAncestor(this);
            rectangle = childTransform.TransformBounds(rectangle);

            // We can't do any work unless we're scrolling.
            if (!IsScrolling)
            {
                return rectangle;
            }

            // Bring the target rect into view in the physical dimension.
            MakeVisiblePhysicalHelper(rectangle, ref newOffset, ref newRect);

            // Bring our child containing the visual into view.
            int childIndex = FindChildIndexThatParentsVisual(visual);
            MakeVisibleLogicalHelper(childIndex, rectangle, ref newOffset, ref newRect);

            // We have computed the scrolling offsets; validate and scroll to them.
            newOffset.X = CoerceOffset(newOffset.X, _scrollData._extent.Width, _scrollData._viewport.Width);
            newOffset.Y = CoerceOffset(newOffset.Y, _scrollData._extent.Height, _scrollData._viewport.Height);
            if (!DoubleUtil.AreClose(newOffset, _scrollData._offset))
            {
                _scrollData._offset = newOffset;
                InvalidateMeasure();
                OnScrollChange();
            }

            // Return the rectangle
            return newRect;
        }

        #endregion

        #endregion

        //-------------------------------------------------------------------
        //
        //  Public Properties
        //
        //-------------------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// Specifies dimension of children stacking.
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="Orientation" /> property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(VirtualizingStackPanel),
                new FrameworkPropertyMetadata(Orientation.Vertical,
                        FrameworkPropertyMetadataOptions.AffectsMeasure,
                        new PropertyChangedCallback(OnOrientationChanged)),
                new ValidateValueCallback(IsValidOrientation));
// Only needed for 3.5 test build
#if TESTBUILD_CLR20
        /// <summary>
        ///     Attached property for use on the ItemsControl that is the host for the items being
        ///     presented by this panel. Use this property to turn virtualization on/off.
        /// </summary>
        public static readonly DependencyProperty IsVirtualizingProperty =
            DependencyProperty.RegisterAttached("IsVirtualizing", typeof(bool), typeof(VirtualizingStackPanel),
                new FrameworkPropertyMetadata(true));

        /// <summary>
        ///     Retrieves the value for <see cref="IsVirtualizingProperty" />.
        /// </summary>
        /// <param name="o">The object on which to query the value.</param>
        /// <returns>True if virtualizing, false otherwise.</returns>
        public static bool GetIsVirtualizing(DependencyObject o)
        {
            if (o == null)
            {
                throw new ArgumentNullException("o");
            }

            return (bool)o.GetValue(IsVirtualizingProperty);
        }

        /// <summary>
        ///     Sets the value for <see cref="IsVirtualizingProperty" />.
        /// </summary>
        /// <param name="o">The object on which to set the value.</param>
        /// <param name="value">True if virtualizing, false otherwise.</param>
        public static void SetIsVirtualizing(DependencyObject o, bool value)
        {
            if (o == null)
            {
                throw new ArgumentNullException("o");
            }

            o.SetValue(IsVirtualizingProperty, value);
        }
#endif
        //-----------------------------------------------------------
        //  IScrollInfo Properties
        //-----------------------------------------------------------
        #region IScrollInfo Properties

        /// <summary>
        /// VirtualizingStackPanel reacts to this property by changing it's child measurement algorithm.
        /// If scrolling in a dimension, infinite space is allowed the child; otherwise, available size is preserved.
        /// </summary>
        [DefaultValue(false)]
        public bool CanHorizontallyScroll
        {
            get
            {
                if (_scrollData == null) { return false; }
                return _scrollData._allowHorizontal;
            }
            set
            {
                EnsureScrollData();
                if (_scrollData._allowHorizontal != value)
                {
                    _scrollData._allowHorizontal = value;
                    InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// VirtualizingStackPanel reacts to this property by changing it's child measurement algorithm.
        /// If scrolling in a dimension, infinite space is allowed the child; otherwise, available size is preserved.
        /// </summary>
        [DefaultValue(false)]
        public bool CanVerticallyScroll
        {
            get
            {
                if (_scrollData == null) { return false; }
                return _scrollData._allowVertical;
            }
            set
            {
                EnsureScrollData();
                if (_scrollData._allowVertical != value)
                {
                    _scrollData._allowVertical = value;
                    InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// ExtentWidth contains the horizontal size of the scrolled content element in 1/96"
        /// </summary>
        public double ExtentWidth
        {
            get
            {
                if (_scrollData == null) { return 0.0; }
                return _scrollData._extent.Width;
            }
        }

        /// <summary>
        /// ExtentHeight contains the vertical size of the scrolled content element in 1/96"
        /// </summary>
        public double ExtentHeight
        {
            get
            {
                if (_scrollData == null) { return 0.0; }
                return _scrollData._extent.Height;
            }
        }

        /// <summary>
        /// ViewportWidth contains the horizontal size of content's visible range in 1/96"
        /// </summary>
        public double ViewportWidth
        {
            get
            {
                if (_scrollData == null) { return 0.0; }
                return _scrollData._viewport.Width;
            }
        }

        /// <summary>
        /// ViewportHeight contains the vertical size of content's visible range in 1/96"
        /// </summary>
        public double ViewportHeight
        {
            get
            {
                if (_scrollData == null) { return 0.0; }
                return _scrollData._viewport.Height;
            }
        }

        /// <summary>
        /// HorizontalOffset is the horizontal offset of the scrolled content in 1/96".
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double HorizontalOffset
        {
            get
            {
                if (_scrollData == null) { return 0.0; }
                return _scrollData._computedOffset.X;
            }
        }

        /// <summary>
        /// VerticalOffset is the vertical offset of the scrolled content in 1/96".
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double VerticalOffset
        {
            get
            {
                if (_scrollData == null) { return 0.0; }
                return _scrollData._computedOffset.Y;
            }
        }

        /// <summary>
        /// ScrollOwner is the container that controls any scrollbars, headers, etc... that are dependant
        /// on this IScrollInfo's properties.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ScrollViewer ScrollOwner
        {
            get
            {
                EnsureScrollData();
                return _scrollData._scrollOwner;
            }
            set
            {
                EnsureScrollData();
                if (value != _scrollData._scrollOwner)
                {
                    ResetScrolling(this);
                    _scrollData._scrollOwner = value;
                }
            }
        }

        #endregion IScrollInfo Properties

        #endregion Public Properties

        //-------------------------------------------------------------------
        //
        //  Public Events
        //
        //-------------------------------------------------------------------


        #region Public Events

        /// <summary>
        ///     Called on the ItemsControl that owns this panel when an item is being re-virtualized.
        /// </summary>
        public static readonly RoutedEvent CleanUpVirtualizedItemEvent = EventManager.RegisterRoutedEvent("CleanUpVirtualizedItemEvent", RoutingStrategy.Direct, typeof(CleanUpVirtualizedItemEventHandler), typeof(VirtualizingStackPanel));


        /// <summary>
        ///     Adds a handler for the CleanUpVirtualizedItem attached event
        /// </summary>
        /// <param name="element">DependencyObject that listens to this event</param>
        /// <param name="handler">Event Handler to be added</param>
        public static void AddCleanUpVirtualizedItemHandler(UIElement element, CleanUpVirtualizedItemEventHandler handler)
        {
            element.AddHandler(CleanUpVirtualizedItemEvent, handler);
        }

        /// <summary>
        ///     Removes a handler for the CleanUpVirtualizedItem attached event
        /// </summary>
        /// <param name="element">DependencyObject that listens to this event</param>
        /// <param name="handler">Event Handler to be removed</param>
        public static void RemoveCleanUpVirtualizedItemHandler(UIElement element, CleanUpVirtualizedItemEventHandler handler)
        {
            element.RemoveHandler(CleanUpVirtualizedItemEvent, handler);
        }

        /// <summary>
        ///     Called when an item is being re-virtualized.
        /// </summary>
        protected virtual void OnCleanUpVirtualizedItem(Avalon.Test.ComponentModel.CustomControls.CleanUpVirtualizedItemEventArgs e)
        {
            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);

            if (itemsControl != null)
            {
                itemsControl.RaiseEvent(e);
            }
        }

        #endregion

        //-------------------------------------------------------------------
        //
        //  Protected Methods
        //
        //-------------------------------------------------------------------

        #region Protected Methods

        /// <summary>
        /// General VirtualizingStackPanel layout behavior is to grow unbounded in the "stacking" direction (Size To Content).
        /// Children in this dimension are encouraged to be as large as they like.  In the other dimension,
        /// VirtualizingStackPanel will assume the maximum size of its children.
        /// </summary>
        /// <remarks>
        /// When scrolling, VirtualizingStackPanel will not grow in layout size but effectively add the children on a z-plane which
        /// will probably be clipped by some parent (typically a ScrollContentPresenter) to Stack's size.
        /// </remarks>
        /// <param name="constraint">Constraint</param>
        /// <returns>Desired size</returns>
        protected override Size MeasureOverride(Size constraint)
        {
#if Profiling
            if (Panel.IsAboutToGenerateContent(this))
                return MeasureOverrideProfileStub(constraint);
            else
                return RealMeasureOverride(constraint);
        }

        // this is a handy place to start/stop profiling
        private Size MeasureOverrideProfileStub(Size constraint)
        {
            return RealMeasureOverride(constraint);
        }

        private Size RealMeasureOverride(Size constraint)
        {
#endif

            UIElementCollection children = InternalChildren;
            Size stackDesiredSize = new Size();
            Size layoutSlotSize = constraint;
            bool fHorizontal = (Orientation == Orientation.Horizontal);
            int firstViewport;          // First child index in the viewport.
            int lastViewport = -1;      // Last child index in the viewport.  -1 indicates we have not yet iterated through the last child.

            double logicalVisibleSpace, childLogicalSize;

            //
            // Collect information from the ItemsControl, if there is one.
            //
            IItemContainerGenerator generator = this.ItemContainerGenerator;
            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
            int itemCount = ((itemsControl != null) && itemsControl.HasItems) ? itemsControl.Items.Count : 0;
#if TESTBUILD_NET_ATLEAST_45
            bool isVirtualizing = (itemsControl != null) ? GetIsVirtualizing(itemsControl) : true;
#else
            // 


            bool isVirtualizing = true;
#endif
            //
            // Initialize child sizing and iterator data
            // Allow children as much size as they want along the stack.
            //
            if (fHorizontal)
            {
                layoutSlotSize.Width = Double.PositiveInfinity;
                if (IsScrolling && CanVerticallyScroll) { layoutSlotSize.Height = Double.PositiveInfinity; }
                firstViewport = (IsScrolling) ? CoerceOffsetToInteger(_scrollData._offset.X, itemCount) : 0;
                logicalVisibleSpace = constraint.Width;
            }
            else
            {
                layoutSlotSize.Height = Double.PositiveInfinity;
                if (IsScrolling && CanHorizontallyScroll) { layoutSlotSize.Width = Double.PositiveInfinity; }
                firstViewport = (IsScrolling) ? CoerceOffsetToInteger(_scrollData._offset.Y, itemCount) : 0;
                logicalVisibleSpace = constraint.Height;
            }

            //
            // Figure out the position of the first visible item
            //
            GeneratorPosition startPos = IndexToGeneratorPositionForStart(isVirtualizing ? firstViewport : 0, out _firstVisibleGeneratedIndex);
            int childIndex = _firstVisibleGeneratedIndex;

            _visibleCount = 0;
            if (itemCount > 0)
            {
                using (generator.StartAt(startPos, GeneratorDirection.Forward))
                {
                    for (int i = isVirtualizing ? firstViewport : 0, count = itemCount; i < count; ++i)
                    {
                        // Get next child.
                        bool newlyRealized;
                        UIElement child = generator.GenerateNext(out newlyRealized) as UIElement;
                        if (child == null)
                        {
                            Debug.Assert(!newlyRealized, "The generator realized a null value.");

                            // We reached the end of the items (because of a group)
                            break;
                        }
                        if (_generatedIndexes == null)
                        {
                            _generatedIndexes = new List<int>();
                        }
                        if ((childIndex < _generatedIndexes.Count) && (_generatedIndexes[childIndex] == i))
                        {
                            Debug.Assert(child != null, "Null child was generated");
                            Debug.Assert(!newlyRealized, "New child shouldn't have been realized");
                            child = children[childIndex];
                        }
                        else
                        {
                            Debug.Assert(child != null, "Null child was generated");
                            Debug.Assert(newlyRealized, "New child should have been realized");
                            if (childIndex >= children.Count)
                            {
                                this.AddInternalChild(child);
                                _generatedIndexes.Add(i);
                            }
                            else
                            {
                                this.InsertInternalChild(childIndex, child);
                                _generatedIndexes.Insert(childIndex, i);
                            }
                            generator.PrepareItemContainer(child);
                        }
                        childIndex++;
                        _visibleCount++;

                        // Measure the child.
                        child.Measure(layoutSlotSize);
                        Size childDesiredSize = child.DesiredSize;

                        // Accumulate child size.
                        if (fHorizontal)
                        {
                            stackDesiredSize.Width += childDesiredSize.Width;
                            stackDesiredSize.Height = Math.Max(stackDesiredSize.Height, childDesiredSize.Height);
                            childLogicalSize = childDesiredSize.Width;
                        }
                        else
                        {
                            stackDesiredSize.Width = Math.Max(stackDesiredSize.Width, childDesiredSize.Width);
                            stackDesiredSize.Height += childDesiredSize.Height;
                            childLogicalSize = childDesiredSize.Height;
                        }

                        // Adjust remaining viewport space if we are scrolling and within the viewport region.
                        // While scrolling (not virtualizing), we always measure children before and after the viewport.
                        if (IsScrolling && lastViewport == -1 && i >= firstViewport)
                        {
                            logicalVisibleSpace -= childLogicalSize;
                            if (DoubleUtil.LessThanOrClose(logicalVisibleSpace, 0.0))
                            {
                                lastViewport = i;
                            }
                        }

                        if (IsScrolling && isVirtualizing)
                        {
                            if (fHorizontal)
                            {
                                if (stackDesiredSize.Width > constraint.Width)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                if (stackDesiredSize.Height > constraint.Height)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            _visibleStart = firstViewport;

            //
            // Compute Scrolling stuff.
            //
            if (IsScrolling)
            {
                // Compute viewport and extent.
                Size viewport = constraint;
                Size extent = stackDesiredSize;
                Vector offset = _scrollData._offset;

                // If we have not yet set the last child in the viewport, set it to the last child.
                if (lastViewport == -1) { lastViewport = itemCount - 1; }

                // If we or children have resized, it's possible that we can now display more content.
                // This is true if we started at a nonzero offeset and still have space remaining.
                // In this case, we loop back through previous children until we run out of space.
                childIndex = isVirtualizing ? _firstVisibleGeneratedIndex : firstViewport;
                while (childIndex > 0)
                {
                    if ((_generatedIndexes[childIndex] - _generatedIndexes[childIndex - 1]) != 1)
                    {
                        GeneratePreviousChild(childIndex, layoutSlotSize);
                        childIndex++; // We just inserted a child, so increment the index
                    }
                    else if (childIndex <= _firstVisibleGeneratedIndex)
                    {
                        // This child has not been measured yet
                        children[childIndex - 1].Measure(layoutSlotSize);
                    }

                    double projectedLogicalVisibleSpace = logicalVisibleSpace;
                    Size childDesiredSize = children[childIndex - 1].DesiredSize;

                    if (fHorizontal)
                    {
                        projectedLogicalVisibleSpace -= childDesiredSize.Width;
                    }
                    else
                    {
                        projectedLogicalVisibleSpace -= childDesiredSize.Height;
                    }

                    // If we have run out of room, break.
                    if (DoubleUtil.LessThan(projectedLogicalVisibleSpace, 0.0)) { break; }

                    // Account for the child in the panel's desired size
                    if (fHorizontal)
                    {
                        stackDesiredSize.Width += childDesiredSize.Width;
                        stackDesiredSize.Height = Math.Max(stackDesiredSize.Height, childDesiredSize.Height);
                    }
                    else
                    {
                        stackDesiredSize.Width = Math.Max(stackDesiredSize.Width, childDesiredSize.Width);
                        stackDesiredSize.Height += childDesiredSize.Height;
                    }

                    // Adjust viewport
                    childIndex--;
                    logicalVisibleSpace = projectedLogicalVisibleSpace;
                }
                if ((childIndex < _firstVisibleGeneratedIndex) || !isVirtualizing)
                {
                    _firstVisibleGeneratedIndex = childIndex;
                }
                _visibleStart = firstViewport = ((_generatedIndexes == null) || (_generatedIndexes.Count == 0)) ? 0 : _generatedIndexes[_firstVisibleGeneratedIndex];
                EnsureCapsGenerated(layoutSlotSize);

                int logicalExtent = itemCount;
                int logicalViewport = lastViewport - firstViewport;

                // We are conservative when estimating a viewport, not including the last element in case it is only partially visible.
                // We want to count it if it is fully visible (>= 0 space remaining) or the only element in the viewport.
                if (logicalViewport == 0 || DoubleUtil.GreaterThanOrClose(logicalVisibleSpace, 0.0)) { logicalViewport++; }

                if (fHorizontal)
                {
                    _scrollData._physicalViewport = viewport.Width;
                    viewport.Width = logicalViewport;
                    extent.Width = logicalExtent;
                    offset.X = firstViewport;
                    offset.Y = Math.Max(0, Math.Min(offset.Y, extent.Height - viewport.Height));
                }
                else
                {
                    _scrollData._physicalViewport = viewport.Height;
                    viewport.Height = logicalViewport;
                    extent.Height = logicalExtent;
                    offset.Y = firstViewport;
                    offset.X = Math.Max(0, Math.Min(offset.X, extent.Width - viewport.Width));
                }

                // Since we can offset and clip our content, we never need to be larger than the parent suggestion.
                // If we returned the full size of the content, we would always be so big we didn't need to scroll.  :)
                stackDesiredSize.Width = Math.Min(stackDesiredSize.Width, constraint.Width);
                stackDesiredSize.Height = Math.Min(stackDesiredSize.Height, constraint.Height);

                // Verify Scroll Info, invalidate ScrollOwner if necessary.
                VerifyScrollingData(viewport, extent, offset);
            }

            if (isVirtualizing)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(OnCleanUp), null);
            }

            return stackDesiredSize;
        }


        private void GeneratePreviousChild(int childIndex, Size layoutSlotSize)
        {
            IItemContainerGenerator generator = this.ItemContainerGenerator;
            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
            UIElementCollection children = InternalChildren;

            int newGeneratedIndex;
            GeneratorPosition newStartPos = IndexToGeneratorPositionForStart(_generatedIndexes[childIndex] - 1, out newGeneratedIndex);
            using (generator.StartAt(newStartPos, GeneratorDirection.Forward))
            {
                bool newlyRealized;
                UIElement child = generator.GenerateNext(out newlyRealized) as UIElement;
                if (_generatedIndexes == null)
                {
                    _generatedIndexes = new List<int>();
                }

                Debug.Assert(child != null, "Null child was generated");
                Debug.Assert(newlyRealized, "New child should have been realized");

                this.InsertInternalChild(childIndex, child);
                _generatedIndexes.Insert(childIndex, _generatedIndexes[childIndex] - 1);

                generator.PrepareItemContainer(child);

                if (childIndex <= _firstVisibleGeneratedIndex)
                {
                    _firstVisibleGeneratedIndex++;
                }

                child.Measure(layoutSlotSize);
            }
        }

        /// <summary>
        ///     Called when the Items collection associated with the containing ItemsControl changes.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="args">Event arguments</param>
        protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
        {
            base.OnItemsChanged(sender, args);

            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    OnItemsAdd(args);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    OnItemsRemove(args);
                    break;
            }
        }

        /// <summary>
        ///     Called when the UI collection of children is cleared by the base Panel class.
        /// </summary>
        protected override void OnClearChildren()
        {
            base.OnClearChildren();

            if (_generatedIndexes != null)
            {
                _generatedIndexes.Clear();
            }

            _visibleStart = _firstVisibleGeneratedIndex = _visibleCount = 0;
        }


        private void OnItemsAdd(ItemsChangedEventArgs args)
        {
            UIElementCollection children = InternalChildren;
            int pos = args.Position.Index + args.Position.Offset;
            int count = args.ItemCount;

            if ((count > 0) && (_generatedIndexes != null))
            {
                for (int i = pos; i < _generatedIndexes.Count; i++)
                {
                    _generatedIndexes[i] += count;
                }
            }
        }

        private void OnItemsRemove(ItemsChangedEventArgs args)
        {
            UIElementCollection children = InternalChildren;
            int pos = args.Position.Index + args.Position.Offset;

            if ((_generatedIndexes != null) && (pos < _generatedIndexes.Count))
            {
                int count;
                int uiCount = args.ItemUICount;
                Debug.Assert((args.ItemUICount == args.ItemCount) || (args.ItemUICount == 0), "Both ItemUICount and ItemCount should be equal or ItemUICount should be 0.");
                if (uiCount > 0)
                {
                    count = uiCount;
                    children.RemoveRange(pos, uiCount);
                    _generatedIndexes.RemoveRange(pos, uiCount);
                }
                else
                {
                    count = args.ItemCount;
                }

                if (count > 0)
                {
                    int generatedCount = _generatedIndexes.Count;
                    for (int i = pos; i < generatedCount; i++)
                    {
                        _generatedIndexes[i] -= count;
                    }
                }
            }
        }

        private GeneratorPosition IndexToGeneratorPositionForStart(int index, out int childIndex)
        {
            Debug.Assert(index >= 0, "index should not be less than 0");

            childIndex = 0;

            // If the index is 0, then no need to search
            if ((index > 0) && _generatedIndexes != null)
            {
                int lastIndex = -1;
                for (int i = 0; i < _generatedIndexes.Count; i++, childIndex++)
                {
                    int generatedIndex = _generatedIndexes[i];
                    if (index < generatedIndex)
                    {
                        if (i == 0)
                        {
                            // Index is before anything that is generated
                            return new GeneratorPosition(0, index - generatedIndex);
                        }
                        else
                        {
                            // Index is between the last generated index and this one
                            return new GeneratorPosition(i - 1, index - lastIndex);
                        }
                    }
                    else if (index == generatedIndex)
                    {
                        // Index is equal to this generated index
                        if ((lastIndex >= 0) && (lastIndex == (index - 1)))
                        {
                            return new GeneratorPosition(i - 1, 0);
                        }
                        else if (lastIndex >= 0)
                        {
                            return new GeneratorPosition(i - 1, index - lastIndex);
                        }
                        else
                        {
                            return new GeneratorPosition(-1, index + 1);
                        }
                    }

                    lastIndex = generatedIndex;
                }

                if (lastIndex >= 0)
                {
                    // Index is after the last generated index
                    return new GeneratorPosition(_generatedIndexes.Count - 1, index - lastIndex);
                }
            }

            // Index is before anything that is generated
            return new GeneratorPosition(-1, index + 1);
        }

        private object OnCleanUp(object args)
        {
            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);

#if TESTBUILD_NET_ATLEAST_45
            if ((itemsControl == null) || GetIsVirtualizing(itemsControl))
            {
                CleanUp();
            }
#else
            // 


            if (itemsControl == null)
            {
                CleanUp();
            }
#endif
            return null;
        }

        private void CleanUp()
        {
            if (_generatedIndexes != null)
            {
                ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
                UIElementCollection children = InternalChildren;
                IItemContainerGenerator generator = this.ItemContainerGenerator;
                int minDesiredGenerated = MinDesiredGenerated;
                int maxDesiredGenerated = MaxDesiredGenerated;

                for (int i = 0; i < _generatedIndexes.Count; i++)
                {
                    int index = _generatedIndexes[i];
                    if ((index < minDesiredGenerated) || (index > maxDesiredGenerated))
                    {
                        if (!KeepForFocus(i, children))
                        {
                            UIElement child = children[i];
                            Avalon.Test.ComponentModel.CustomControls.CleanUpVirtualizedItemEventArgs e = new Avalon.Test.ComponentModel.CustomControls.CleanUpVirtualizedItemEventArgs(itemsControl.ItemContainerGenerator.ItemFromContainer(child), child);
                            e.Source = this;
                            OnCleanUpVirtualizedItem(e);
                            if (!e.Cancel)
                            {
                                generator.Remove(new GeneratorPosition(i, 0), 1);
                                this.RemoveInternalChildRange(i, 1);
                                _generatedIndexes.RemoveAt(i);
                                if (i < _firstVisibleGeneratedIndex)
                                {
                                    _firstVisibleGeneratedIndex--;
                                }
                                i--;
                            }
                        }
                    }
                }
            }
        }

        private bool KeepForFocus(int index, UIElementCollection children)
        {
            UIElement child = children[index];
            if (child.IsKeyboardFocusWithin)
            {
                // This one is the focused element.
                return true;
            }

            if ((index > 0) && ((_generatedIndexes[index] - _generatedIndexes[index - 1]) == 1)
                && children[index - 1].IsKeyboardFocusWithin)
            {
                // This one is after the focused element.
                return true;
            }

            if ((index < (_generatedIndexes.Count - 1)) && ((_generatedIndexes[index + 1] - _generatedIndexes[index]) == 1)
                && children[index + 1].IsKeyboardFocusWithin)
            {
                // This one is before the focused element.
                return true;
            }

            return false;
        }

        private void EnsureCapsGenerated(Size layoutSlotSize)
        {
            if (_visibleStart > 0)
            {
                if ((_firstVisibleGeneratedIndex == 0) ||
                    ((_generatedIndexes[_firstVisibleGeneratedIndex] - _generatedIndexes[_firstVisibleGeneratedIndex - 1]) > 1))
                {
                    GeneratePreviousChild(_firstVisibleGeneratedIndex, layoutSlotSize);
                }
            }
        }

        private int MinDesiredGenerated
        {
            get
            {
                return Math.Max(0, _visibleStart - 1);
            }
        }

        private int MaxDesiredGenerated
        {
            get
            {
                return Math.Min(ItemCount, _visibleStart + _visibleCount + 1);
            }
        }

        private int ItemCount
        {
            get
            {
                ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
                return ((itemsControl != null) && itemsControl.HasItems) ? itemsControl.Items.Count : 0;
            }
        }

        /// <summary>
        /// Content arrangement.
        /// </summary>
        /// <param name="arrangeSize">Arrange size</param>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
#if TESTBUILD_NET_ATLEAST_45
            bool isVirtualizing = (itemsControl != null) ? GetIsVirtualizing(itemsControl) : true;
#else
            // 


            bool isVirtualizing = true;
#endif
            int count = VisualTreeHelper.GetChildrenCount(this);
            bool fHorizontal = (Orientation == Orientation.Horizontal);
            Rect rcChild = new Rect(arrangeSize);
            double previousChildSize = 0.0;

            //
            // Compute scroll offset and seed it into rcChild.
            //
            if (IsScrolling)
            {
                if (fHorizontal)
                {
                    rcChild.X = ComputePhysicalFromLogicalOffset(isVirtualizing ? _firstVisibleGeneratedIndex : _scrollData._computedOffset.X, true);
                    rcChild.Y = -1.0 * _scrollData._computedOffset.Y;
                }
                else
                {
                    rcChild.X = -1.0 * _scrollData._computedOffset.X;
                    rcChild.Y = ComputePhysicalFromLogicalOffset(isVirtualizing ? _firstVisibleGeneratedIndex : _scrollData._computedOffset.Y, false);
                }
            }

            //
            // Arrange and Position Children.
            //
            for(int i = 0; i < count; i++)
            {
                UIElement child = (UIElement)VisualTreeHelper.GetChild(this,i);

                if (fHorizontal)
                {
                    rcChild.X += previousChildSize;
                    previousChildSize = child.DesiredSize.Width;
                    rcChild.Width = previousChildSize;
                    rcChild.Height = Math.Max(arrangeSize.Height, child.DesiredSize.Height);
                }
                else
                {
                    rcChild.Y += previousChildSize;
                    previousChildSize = child.DesiredSize.Height;
                    rcChild.Height = previousChildSize;
                    rcChild.Width = Math.Max(arrangeSize.Width, child.DesiredSize.Width);
                }

                child.Arrange(rcChild);
            }

            return arrangeSize;
        }


        #endregion Protected Methods

        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        #region Private Methods

        private void EnsureScrollData()
        {
            if (_scrollData == null) { _scrollData = new ScrollData(); }
        }

        private static void ResetScrolling(VirtualizingStackPanel element)
        {
            element.InvalidateMeasure();

            // Clear scrolling data.  Because of thrash (being disconnected & reconnected, &c...), we may
            if (element.IsScrolling)
            {
                element._scrollData.ClearLayout();
            }
        }

        // OnScrollChange is an override called whenever the IScrollInfo exposed scrolling state changes on this element.
        // At the time this method is called, scrolling state is in its new, valid state.
        private void OnScrollChange()
        {
            if (ScrollOwner != null) { ScrollOwner.InvalidateScrollInfo(); }
        }

        private void VerifyScrollingData(Size viewport, Size extent, Vector offset)
        {
            bool fValid = true;

            Debug.Assert(IsScrolling);

            fValid &= DoubleUtil.AreClose(viewport, _scrollData._viewport);
            fValid &= DoubleUtil.AreClose(extent, _scrollData._extent);
            fValid &= DoubleUtil.AreClose(offset, _scrollData._computedOffset);
            _scrollData._offset = offset;

            if (!fValid)
            {
                _scrollData._viewport = viewport;
                _scrollData._extent = extent;
                _scrollData._computedOffset = offset;
                OnScrollChange();
            }
        }

        // Translates a logical (child index) offset to a physical (1/96") when scrolling.
        // If virtualizing, it makes the assumption that the logicalOffset is always the first in the visual collection
        //   and thus returns 0.
        // If not virtualizing, it assumes that children are Measure clean; should only be called after running Measure.
        private double ComputePhysicalFromLogicalOffset(double logicalOffset, bool fHorizontal)
        {
            double physicalOffset = 0.0;

            int count = VisualTreeHelper.GetChildrenCount(this);
            Debug.Assert(logicalOffset == 0 || (logicalOffset > 0 && logicalOffset < count));

            for (int i = 0; i < logicalOffset; i++)
            {
                physicalOffset -= (fHorizontal)
                    ? ((UIElement)VisualTreeHelper.GetChild(this,i)).DesiredSize.Width
                    : ((UIElement)VisualTreeHelper.GetChild(this,i)).DesiredSize.Height;
            }

            return physicalOffset;
        }

        private int FindChildIndexThatParentsVisual(Visual v)
        {
            Visual child = v;
            Visual parent = VisualTreeHelper.GetParent(child) as Visual;
            while (parent != this)
            {
                child = parent;
                parent = VisualTreeHelper.GetParent(child) as Visual;
            }

            UIElementCollection children = InternalChildren;
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i] == child)
                {
                    return _generatedIndexes[i];
                }
            }

            return -1;
        }

        private void MakeVisiblePhysicalHelper(Rect r, ref Vector newOffset, ref Rect newRect)
        {
            double viewportOffset;
            double viewportSize;
            double targetRectOffset;
            double targetRectSize;
            double minPhysicalOffset;

            bool fHorizontal = (Orientation == Orientation.Horizontal);
            if (fHorizontal)
            {
                viewportOffset = _scrollData._computedOffset.Y;
                viewportSize = ViewportHeight;
                targetRectOffset = r.Y;
                targetRectSize = r.Height;
            }
            else
            {
                viewportOffset = _scrollData._computedOffset.X;
                viewportSize = ViewportWidth;
                targetRectOffset = r.X;
                targetRectSize = r.Width;
            }

            targetRectOffset += viewportOffset;
            minPhysicalOffset = ComputeScrollOffsetWithMinimalScroll(
                viewportOffset, viewportOffset + viewportSize, targetRectOffset, targetRectOffset + targetRectSize);

            // Compute the visible rectangle of the child relative to the viewport.
            double left = Math.Max(targetRectOffset, minPhysicalOffset);
            targetRectSize = Math.Max(Math.Min(targetRectSize + targetRectOffset, minPhysicalOffset + viewportSize) - left, 0);
            targetRectOffset = left;
            targetRectOffset -= viewportOffset;

            if (fHorizontal)
            {
                newOffset.Y = minPhysicalOffset;
                newRect.Y = targetRectOffset;
                newRect.Height = targetRectSize;
            }
            else
            {
                newOffset.X = minPhysicalOffset;
                newRect.X = targetRectOffset;
                newRect.Width = targetRectSize;
            }
        }

        private void MakeVisibleLogicalHelper(int childIndex, Rect r, ref Vector newOffset, ref Rect newRect)
        {
            bool fHorizontal = (Orientation == Orientation.Horizontal);
            int firstChildInView;
            int newFirstChild;
            int viewportSize;
            double childOffsetWithinViewport = r.Y;

            if (fHorizontal)
            {
                firstChildInView = (int)_scrollData._computedOffset.X;
                viewportSize = (int)_scrollData._viewport.Width;
            }
            else
            {
                firstChildInView = (int)_scrollData._computedOffset.Y;
                viewportSize = (int)_scrollData._viewport.Height;
            }

            newFirstChild = firstChildInView;

            // If the target child is before the current viewport, move the viewport to put the child at the top.
            if (childIndex < firstChildInView)
            {
                childOffsetWithinViewport = 0;
                newFirstChild = childIndex;
            }
            // If the target child is after the current viewport, move the viewport to put the child at the bottom.
            else if (childIndex > firstChildInView + viewportSize - 1)
            {
                newFirstChild = childIndex - viewportSize + 1;
                double pixelSize = fHorizontal ? ActualWidth : ActualHeight;
                childOffsetWithinViewport = pixelSize * (1.0 - (1.0 / viewportSize));
            }

            if (fHorizontal)
            {
                newOffset.X = newFirstChild;
                newRect.X = childOffsetWithinViewport;
                newRect.Width = r.Width;
            }
            else
            {
                newOffset.Y = newFirstChild;
                newRect.Y = childOffsetWithinViewport;
                newRect.Height = r.Height;
            }
        }

        static private int CoerceOffsetToInteger(double offset, int numberOfItems)
        {
            int iNewOffset;

            if (Double.IsNegativeInfinity(offset))
            {
                iNewOffset = 0;
            }
            else if (Double.IsPositiveInfinity(offset))
            {
                iNewOffset = numberOfItems - 1;
            }
            else
            {
                iNewOffset = (int)offset;
                iNewOffset = Math.Max(Math.Min(numberOfItems - 1, iNewOffset), 0);
            }

            return iNewOffset;
        }

        //-----------------------------------------------------------
        // Avalon Property Callbacks/Overrides
        //-----------------------------------------------------------
        #region Avalon Property Callbacks/Overrides

        /// <summary>
        /// <see cref="PropertyMetadata.PropertyChangedCallback"/>
        /// </summary>
        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Since Orientation is so essential to logical scrolling/virtualization, we synchronously check if
            // the new value is different and clear all scrolling data if so.
            ResetScrolling(d as VirtualizingStackPanel);
        }

        static internal double ValidateInputOffset(double offset, string parameterName)
        {
            if (DoubleUtil.IsNaN(offset))
            {
                throw new ArgumentOutOfRangeException(parameterName, " ");
            }
            return Math.Max(0.0, offset);
        }

        // Returns an offset coerced into the [0, Extent - Viewport] range.
        // Internal because it is also used by other Avalon ISI implementations (just to avoid code duplication).
        static internal double CoerceOffset(double offset, double extent, double viewport)
        {
            if (offset > extent - viewport) { offset = extent - viewport; }
            if (offset < 0) { offset = 0; }
            return offset;
        }

        internal static double ComputeScrollOffsetWithMinimalScroll(
            double topView,
            double bottomView,
            double topChild,
            double bottomChild)
        {
            // # CHILD POSITION       CHILD SIZE      SCROLL      REMEDY
            // 1 Above viewport       <= viewport     Down        Align top edge of child & viewport
            // 2 Above viewport       > viewport      Down        Align bottom edge of child & viewport
            // 3 Below viewport       <= viewport     Up          Align bottom edge of child & viewport
            // 4 Below viewport       > viewport      Up          Align top edge of child & viewport
            // 5 Entirely within viewport             NA          No scroll.
            // 6 Spanning viewport                    NA          No scroll.
            //
            // Note: "Above viewport" = childTop above viewportTop, childBottom above viewportBottom
            //       "Below viewport" = childTop below viewportTop, childBottom below viewportBottom
            // These child thus may overlap with the viewport, but will scroll the same direction/

            bool fAbove = DoubleUtil.LessThan(topChild, topView) && DoubleUtil.LessThan(bottomChild, bottomView);
            bool fBelow = DoubleUtil.GreaterThan(bottomChild, bottomView) && DoubleUtil.GreaterThan(topChild, topView);
            bool fLarger = (bottomChild - topChild) > (bottomView - topView);

            // Handle Cases:  1 & 4 above
            if ((fAbove && !fLarger)
               || (fBelow && fLarger))
            {
                return topChild;
            }

            // Handle Cases: 2 & 3 above
            else if (fAbove || fBelow)
            {
                return (bottomChild - (bottomView - topView));
            }

            // Handle cases: 5 & 6 above.
            return topView;
        }


        internal static bool IsValidOrientation(object o)
        {
            Orientation value = (Orientation)o;
            return value == Orientation.Horizontal
                || value == Orientation.Vertical;
        }


        #endregion

        #endregion Private Methods

        //------------------------------------------------------
        //
        //  Private Properties
        //
        //------------------------------------------------------

        #region Private Properties

        private bool IsScrolling
        {
            get { return (_scrollData != null) && (_scrollData._scrollOwner != null); }
        }

        #endregion Private Properties

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        // Logical scrolling and virtualization data.
        private ScrollData _scrollData;

        // Virtualization state
        private int _visibleStart;
        private int _visibleCount;
        private List<int> _generatedIndexes;
        private int _firstVisibleGeneratedIndex;

        // Scrolling physical "line" metrics.
        internal const double _scrollLineDelta = 16.0;   // Default physical amount to scroll with one Up/Down/Left/Right key

        #endregion Private Fields


        //------------------------------------------------------
        //
        //  Private Structures / Classes
        //
        //------------------------------------------------------

        #region Private Structures Classes

        //-----------------------------------------------------------
        // ScrollData class
        //-----------------------------------------------------------
        #region ScrollData

        // Helper class to hold scrolling data.
        // This class exists to reduce working set when VirtualizingStackPanel is used outside a scrolling situation.
        // Standard "extra pointer always for less data sometimes" cache savings model:
        //      !Scroll [1xReference]
        //      Scroll  [1xReference] + [6xDouble + 1xReference]
        private class ScrollData
        {
            // Clears layout generated data.
            // Does not clear scrollOwner, because unless resetting due to a scrollOwner change, we won't get reattached.
            internal void ClearLayout()
            {
                _offset = new Vector();
                _viewport = _extent = new Size();
                _physicalViewport = 0;
            }

            // For Stack/Flow, the two dimensions of properties are in different units:
            // 1. The "logically scrolling" dimension uses items as units.
            // 2. The other dimension physically scrolls.  Units are in Avalon pixels (1/96").
            internal bool _allowHorizontal;
            internal bool _allowVertical;
            internal Vector _offset;            // Scroll offset of content.  Positive corresponds to a visually upward offset.
            internal Vector _computedOffset = new Vector(0,0);
            internal Size _viewport;            // ViewportSize is in {pixels x items} (or vice-versa).
            internal Size _extent;              // Extent is the total number of children (logical dimension) or physical size
            internal double _physicalViewport;  // The physical size of the viewport for the items dimension above.
            internal ScrollViewer _scrollOwner; // ScrollViewer to which we're attached.
        }

        #endregion ScrollData

        #endregion Private Structures Classes
    }
}

