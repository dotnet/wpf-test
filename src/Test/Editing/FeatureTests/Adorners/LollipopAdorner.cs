// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Description: Info needed to draw decorations on objects

using System;
using System.Windows.Media;
using System.Collections;
using System.Windows;
using System.Windows.Documents;

namespace Test.Uis.TextEditing
{
    /// <summary>
    /// How to position the rotating handle relative to element border
    /// </summary>
    public enum LollipopPosition
    {
        /// <summary>
        /// TopLeft
        /// </summary>
        TopLeft,

        /// <summary>
        /// TopCenter
        /// </summary>
        TopCenter,

        /// <summary>
        /// TopRight
        /// </summary>
        TopRight,

        /// <summary>
        /// LeftCenter
        /// </summary>
        LeftCenter,

        /// <summary>
        /// RightCenter
        /// </summary>
        RightCenter,

        /// <summary>
        /// BottomLeft
        /// </summary>
        BottomLeft,

        /// <summary>
        /// BottomCenter
        /// </summary>
        BottomCenter,

        /// <summary>
        /// BottomRight
        /// </summary>
        BottomRight
    }

    /// <summary>
    /// Handle identifiers
    /// </summary>
    public enum LollipopHandle
    {
        /// <summary>
        /// Center point anchor
        /// </summary>
        Anchor,

        /// <summary>
        /// Rotation lollipop
        /// </summary>
        Lollipop,

        /// <summary>
        /// None
        /// </summary>
        None
    }

    /// <summary>
    /// Information describing a lollipop adorner
    /// </summary>
    public class LollipopAdorner : Adorner
    {

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">element we're adorning</param>
        /// <param name="lollipopPosition">LollipopPosition</param>
        public LollipopAdorner(UIElement element, LollipopPosition lollipopPosition)
            : base(element)
        {
            _children = new VisualCollection(this);          
            _lollipopPosition = lollipopPosition;
            CreateHandles();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">element we're adorning</param>
        /// <param name="lollipopPosition">LollipopPosition</param>
        /// <param name="lollipopStemLength">Length of the stem of the lollipop</param>
        /// <param name="lollipopHeadDiameter">diameter of lollipop head</param>
        /// <param name="lollipopPen">Pen</param>
        /// <param name="lollipopBrush">Brush</param>
        public LollipopAdorner(UIElement element, LollipopPosition lollipopPosition, int lollipopStemLength, int lollipopHeadDiameter, Pen lollipopPen, Brush lollipopBrush)
            : base(element)
        {
            _children = new VisualCollection(this);          
            if (lollipopPen == null)
                throw new ArgumentNullException("lollipopPen");

            if (lollipopBrush == null)
                throw new ArgumentNullException("lollipopBrush");

            _lollipopPosition = lollipopPosition;
            _lollipopStemLength = lollipopStemLength;
            _lollipopHeadDiameter = lollipopHeadDiameter;
            _lollipopPen = lollipopPen;
            _lollipopBrush = lollipopBrush;
            CreateHandles();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">element we're adorning</param>
        /// <param name="lollipopPosition">LollipopPosition</param>
        /// <param name="centerPoint">center point of the element</param>
        /// <param name="anchorSize">Size of anchor handle</param>
        /// <param name="anchorPen">Pen</param>
        /// <param name="anchorBrush">Brush</param>
        /// <param name="lollipopStemLength">Length of the stem of the lollipop</param>
        /// <param name="lollipopHeadDiameter">diameter of lollipop head</param>
        /// <param name="lollipopPen">Pen</param>
        /// <param name="lollipopBrush">Brush</param>
        public LollipopAdorner(UIElement element, LollipopPosition lollipopPosition, Point centerPoint, Size anchorSize, Pen anchorPen, Brush anchorBrush, int lollipopStemLength, int lollipopHeadDiameter, Pen lollipopPen, Brush lollipopBrush)
            : base(element)
        {
            _children = new VisualCollection(this);          
            if (anchorPen == null)
                throw new ArgumentNullException("anchorPen");

            if (anchorBrush == null)
                throw new ArgumentNullException("anchorBrush");

            if (lollipopPen == null)
                throw new ArgumentNullException("lollipopPen");

            if (lollipopBrush == null)
                throw new ArgumentNullException("lollipopBrush");


            _lollipopPosition = lollipopPosition;
            _centerPoint = centerPoint;
            _anchorSize = anchorSize;
            _anchorPen = anchorPen;
            _anchorBrush = anchorBrush;
            _lollipopStemLength = lollipopStemLength;
            _lollipopHeadDiameter = lollipopHeadDiameter;
            _lollipopPen = lollipopPen;
            _lollipopBrush = lollipopBrush;
            CreateHandles();
        }

        #endregion Constructors


        #region Public Methods

        /// <summary>
        /// If the lollipop is along the left or top edge, we don't want to position the
        /// adorner at the UL corner of the element.  Take the lollipop size into account
        /// when positioning the adorner so that it looks correct.
        /// </summary>
        /// <param name="transform">The transform applied to the object the adorner adorns</param>
        /// <returns>Transform to apply to the adorner</returns>
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup desiredTransform = new GeneralTransformGroup();

            if (transform == null)
            {
                throw new ArgumentNullException("transform");
            }

            desiredTransform.Children.Add(transform);

            if (_offsetX != 0 || _offsetY != 0)
            {
                desiredTransform.Children.Add(new TranslateTransform(-_offsetX, -_offsetY));
            }

            return desiredTransform;
        }

        /// <summary>
        /// Measure adorner
        /// </summary>
        protected override Size MeasureOverride(Size size)
        {
            Size desiredSize = base.MeasureOverride(size);
            double root2 = Math.Sqrt(2);
            double radius = _lollipopHeadDiameter / 2;

            switch (LollipopPosition)
            {
                case LollipopPosition.BottomRight:
                case LollipopPosition.TopLeft:
                case LollipopPosition.TopRight:
                case LollipopPosition.BottomLeft:
                    _offsetX = (_lollipopHeadDiameter + _lollipopStemLength / root2);
                    _offsetY = _offsetX;
                    break;

                case LollipopPosition.TopCenter:
                case LollipopPosition.BottomCenter:
                    _offsetX = 0;
                    _offsetY = (_lollipopHeadDiameter + _lollipopStemLength);
                    break;

                case LollipopPosition.RightCenter:
                case LollipopPosition.LeftCenter:
                    _offsetX = (_lollipopHeadDiameter + _lollipopStemLength);
                    _offsetY = 0;
                    break;
            }

            desiredSize.Width += _offsetX;
            desiredSize.Height += _offsetY;

            // Now that we've computed our size, we only want to offset the adorner in each direction in
            // certain cases.  Update the offsets, which we'll use for our desired transform and for
            // positioning the center point handle.
            switch (LollipopPosition)
            {
                case LollipopPosition.TopLeft:
                    break;

                case LollipopPosition.TopCenter:
                case LollipopPosition.TopRight:
                    _offsetX = 0;
                    break;

                case LollipopPosition.LeftCenter:
                case LollipopPosition.BottomLeft:
                    _offsetY = 0;
                    break;

                case LollipopPosition.BottomCenter:
                case LollipopPosition.RightCenter:
                case LollipopPosition.BottomRight:
                    _offsetX = 0;
                    _offsetY = 0;
                    break;
            }

            return desiredSize;
        }

        /// <summary>
        /// Determines whether or not the given Visual is part of this adorner.
        /// </summary>
        /// <param name="visual">Visual to test</param>
        /// <returns>LollipopHandle.Anchor if the visual is the anchor.
        /// LollipopHandle.Lollipop if the visual is the lollipop.
        /// LollipopHandle.None otherwise.</returns>
        public virtual LollipopHandle GetLollipopHandleFromVisual(Visual visual)
        {
            if (visual == _anchorVisual)
                return LollipopHandle.Anchor;
            else if (visual == _lollipopVisual)
                return LollipopHandle.Lollipop;

            return LollipopHandle.None;
        }

        /// <summary>
        /// Arrange lollipop
        /// </summary>
        protected override Size ArrangeOverride(Size size)
        {
            TranslateTransform transform;
            Size finalSize;

            finalSize = base.ArrangeOverride(size);

            if (_anchorVisual != null)
            {
                double x = _centerPoint.X + _offsetX - _anchorSize.Width / 2;
                double y = _centerPoint.Y + _offsetY - _anchorSize.Height / 2;
                transform = new TranslateTransform(x, y);

                _anchorVisual.Transform = transform;
            }

            PositionLollipop();

            // We draw inside child visuals, which isn't allowed during OnRender().  So
            // if anything has changed, draw now.
            if (_dirty)
            {
                DrawLollipop();
                DrawAnchor();
            }
            return finalSize;
        }

        #endregion Public Methods


        #region Public Properties

        /// <summary>
        /// Location of the lollipop, relative to the element to which it is attached.
        /// </summary>
        public LollipopPosition LollipopPosition
        {
            get { return _lollipopPosition; }
            set
            {
                _lollipopPosition = value;
                _dirty = true;
                InvalidateMeasure();
            }
        }

        /// <summary>
        /// Length of the lollipop's stem.  Note that this is actually the width and height of the stem, not the
        /// stem's true length (which will be sqrt(2 * (value ^ 2)))
        /// </summary>
        public int LollipopStemLength
        {
            get { return _lollipopStemLength; }
            set
            {
                _lollipopStemLength = value;
                _dirty = true;
                InvalidateMeasure();
            }
        }

        /// <summary>
        /// Diameter of the lollipop's head
        /// </summary>
        public int LollipopHeadDiameter
        {
            get { return _lollipopHeadDiameter; }
            set
            {
                _lollipopHeadDiameter = value;
                _dirty = true;
                InvalidateMeasure();
            }
        }

        /// <summary>
        /// Size of anchor's bounding box; used to position grab handle
        /// </summary>
        public Size AnchorSize
        {
            get { return _anchorSize; }
            set
            {
                _anchorSize = value;
                if (_anchorSize.Width > 0 && _anchorSize.Height > 0)
                {
                    if (_anchorVisual == null)
                    {
                        _anchorVisual = new DrawingVisual();
                        _children.Add(_anchorVisual);
                    }
                }
                else if (_anchorVisual != null)
                {
                    _children.Remove(_anchorVisual);
                    _anchorVisual = null;
                }

                _dirty = true;
                InvalidateMeasure();
            }
        }

        /// <summary>
        /// Center
        /// </summary>
        public Point CenterPoint
        {
            get { return _centerPoint; }
            set
            {
                _centerPoint = value;
                _dirty = true;
                InvalidateArrange();
            }
        }

        /// <summary>
        /// Pen with which to draw center point anchor
        /// </summary>
        public Pen AnchorPen
        {
            get { return _anchorPen; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _anchorPen = value;
                _dirty = true;
                InvalidateMeasure();
            }
        }


        /// <summary>
        /// Brush with which to draw center point anchor
        /// </summary>
        public Brush AnchorBrush
        {
            get { return _anchorBrush; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _anchorBrush = value;
                _dirty = true;
                InvalidateArrange();
            }
        }

        /// <summary>
        /// Pen with which to draw lollipop
        /// </summary>
        public Pen LollipopPen
        {
            get { return _lollipopPen; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _lollipopPen = value;
                _dirty = true;
                InvalidateMeasure();
            }
        }

        /// <summary>
        /// Brush with which to draw lollipop
        /// </summary>
        public Brush LollipopBrush
        {
            get { return _lollipopBrush; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _lollipopBrush = value;
                _dirty = true;
                InvalidateArrange();
            }
        }

        #endregion Public Properties


        #region Protected Methods

        /// <summary>
        /// Create grab handles
        /// </summary>
        private void CreateHandles()
        {
            _lollipopVisual = new DrawingVisual();
            _children.Add(_lollipopVisual);

            if (_anchorSize.Width > 0 && _anchorSize.Height > 0)
            {
                _anchorVisual = new DrawingVisual();
                _children.Add(_anchorVisual);
            }

            DrawLollipop();
            DrawAnchor();
        }

        /// <summary>
        /// Set position of lollipop visual
        /// </summary>
        protected virtual void PositionLollipop()
        {
            Point pt;
            pt = GetLollipopPosition();
            Transform transform = new TranslateTransform(pt.X, pt.Y);
            _lollipopVisual.Transform = transform;
        }

        /// <summary>
        /// Determine the point at which the lollipop should draw itself.
        /// </summary>
        /// <returns>Point</returns>
        protected virtual Point GetLollipopPosition()
        {
            double x, y;

            double root2 = Math.Sqrt(2);
            double radius = _lollipopHeadDiameter / 2;

            switch (LollipopPosition)
            {
                case LollipopPosition.TopLeft:
                    x = 0;
                    y = 0;
                    break;

                case LollipopPosition.TopCenter:
                    x = DesiredSize.Width / 2 - radius;
                    y = 0;
                    break;

                case LollipopPosition.TopRight:
                    x = DesiredSize.Width - (_lollipopHeadDiameter + _lollipopStemLength / root2);
                    y = 0;
                    break;

                case LollipopPosition.LeftCenter:
                    x = 0;
                    y = DesiredSize.Height / 2 - radius;
                    break;

                case LollipopPosition.RightCenter:
                    x = DesiredSize.Width - (_lollipopHeadDiameter + _lollipopStemLength);
                    y = DesiredSize.Height / 2 - radius;
                    break;

                case LollipopPosition.BottomLeft:
                    x = 0;
                    y = DesiredSize.Height - (_lollipopHeadDiameter + _lollipopStemLength / root2);
                    break;

                case LollipopPosition.BottomCenter:
                    x = DesiredSize.Width / 2 - radius;
                    y = DesiredSize.Height - (_lollipopHeadDiameter + _lollipopStemLength);
                    break;

                case LollipopPosition.BottomRight:
                    x = DesiredSize.Width - (_lollipopHeadDiameter + _lollipopStemLength / root2);
                    y = DesiredSize.Height - (_lollipopHeadDiameter + _lollipopStemLength / root2);
                    break;

                default:
                    x = 0;  // shut up the compiler
                    y = 0;  // ditto
                    throw new Exception();
            }
            return new Point(x, y);
        }

        /// <summary>
        /// Draw the lollipop
        /// </summary>
        protected void DrawLollipop()
        {
            using (DrawingContext context = _lollipopVisual.RenderOpen())
            {
                Point stemStart, stemEnd, head;

                // initialize to satisfy compiler
                stemStart = new Point(0, 0);
                stemEnd = new Point(0, 0);
                head = new Point(0, 0);

                GetLollipopDrawingData(ref stemStart, ref stemEnd, ref head);

                context.DrawLine(LollipopPen, stemStart, stemEnd);
                context.DrawRoundedRectangle(LollipopBrush, LollipopPen, new Rect(head, new Point(head.X + _lollipopHeadDiameter, head.Y + _lollipopHeadDiameter)), _lollipopHeadDiameter / 2, _lollipopHeadDiameter / 2);
            }

            _dirty = false;
        }

        /// <summary>
        /// Draw the anchor
        /// </summary>
        protected void DrawAnchor()
        {
            if (_anchorVisual != null)
            {
                using (DrawingContext context = _anchorVisual.RenderOpen())
                {
                    context.DrawRectangle(_anchorBrush, _anchorPen, new Rect(0, 0, _anchorSize.Width, _anchorSize.Height));
                }
            }

            _dirty = false;
        }

        /// <summary>
        /// Generate drawing data for the lollipop based on LollipopPosition.
        /// </summary>
        /// <param name="stemStart">output arg for one of the stem's endpoints</param>
        /// <param name="stemEnd">output arg for the other of the stem's endpoints</param>
        /// <param name="head">output arg for the coordinate where the head is to be drawn</param>
        protected void GetLollipopDrawingData(ref Point stemStart, ref Point stemEnd, ref Point head)
        {
            double radius = _lollipopHeadDiameter / 2;
            double root2 = Math.Sqrt(2);
            double delta = radius - radius / root2;

            switch (LollipopPosition)
            {
                case LollipopPosition.TopLeft:
                    stemStart = new Point(_lollipopHeadDiameter + _lollipopStemLength / root2, _lollipopHeadDiameter + _lollipopStemLength / root2);
                    stemEnd = new Point(_lollipopHeadDiameter, _lollipopHeadDiameter);
                    head = new Point(delta, delta);
                    break;

                case LollipopPosition.TopCenter:
                    stemStart = new Point(_lollipopHeadDiameter / 2, _lollipopHeadDiameter + _lollipopStemLength);
                    stemEnd = new Point(_lollipopHeadDiameter / 2, _lollipopHeadDiameter);
                    head = new Point(0, 0);
                    break;

                case LollipopPosition.LeftCenter:
                    stemStart = new Point(_lollipopHeadDiameter + _lollipopStemLength, _lollipopHeadDiameter / 2);
                    stemEnd = new Point(_lollipopHeadDiameter, _lollipopHeadDiameter / 2);
                    head = new Point(0, 0);
                    break;

                case LollipopPosition.BottomLeft:
                    stemStart = new Point(_lollipopHeadDiameter + _lollipopStemLength / root2, 0);
                    stemEnd = new Point(_lollipopHeadDiameter, _lollipopStemLength / root2);
                    head = new Point(delta, _lollipopStemLength / root2 - (delta));
                    break;

                case LollipopPosition.BottomCenter:
                    stemStart = new Point(_lollipopHeadDiameter / 2, 0);
                    stemEnd = new Point(_lollipopHeadDiameter / 2, _lollipopStemLength);
                    head = new Point(0, _lollipopStemLength);
                    break;

                case LollipopPosition.BottomRight:
                    stemStart = new Point(0, 0);
                    stemEnd = new Point(_lollipopStemLength / root2, _lollipopStemLength / root2);
                    head = new Point(_lollipopStemLength / root2 - (delta), _lollipopStemLength / root2 - (delta));
                    break;

                case LollipopPosition.RightCenter:
                    stemStart = new Point(0, (_lollipopHeadDiameter - 1) / 2);
                    stemEnd = new Point(_lollipopStemLength, _lollipopHeadDiameter / 2);
                    head = new Point(_lollipopStemLength, 0);
                    break;

                case LollipopPosition.TopRight:
                default:
                    stemStart = new Point(0, _lollipopHeadDiameter + _lollipopStemLength / root2);
                    stemEnd = new Point(_lollipopStemLength / root2, _lollipopHeadDiameter);
                    head = new Point(_lollipopStemLength / root2 - delta, delta);
                    break;
            }
        }

        /// <summary>
        /// Returns the child at the specified index.
        /// </summary>
        protected override Visual GetVisualChild(int index)
        {
            // if you have a template
            if(base.VisualChildrenCount != 0 && index == 0)
            {
                return base.GetVisualChild(0);
            }            
            // otherwise you can have your own children
            if(_children == null)
            {
                throw new ArgumentOutOfRangeException("index is out of range");
            }
            if(index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException("index is out of range");
            }

            return _children[index];
        }
        
        /// <summary>
        /// Returns the Visual children count.
        /// </summary>        
        protected override int VisualChildrenCount
        {           
            get 
            {
                //you can either have a Template or your own children
                if(base.VisualChildrenCount > 0) return 1;
                else return  _children.Count; 
            }            
        }  

        #endregion Protected Methods


        private VisualCollection _children;  
        private DrawingVisual _anchorVisual;
        private DrawingVisual _lollipopVisual;
        private LollipopPosition _lollipopPosition;
        private Point _centerPoint;
        private Size _anchorSize = new Size(0, 0);
        private Pen _anchorPen = new Pen(Brushes.Black, 1);
        private Brush _anchorBrush = Brushes.White;
        private int _lollipopStemLength = 10;
        private int _lollipopHeadDiameter = 8;
        private Pen _lollipopPen = new Pen(Brushes.Black, 1);
        private Brush _lollipopBrush = Brushes.White;
        private double _offsetX = 0;
        private double _offsetY = 0;
        private bool _dirty = false;
    }
}