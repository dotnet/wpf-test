// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Documents;
using System.Collections;
using System.Collections.Specialized;

namespace DRT
{
    /// <summary>
    /// How to position the adorners relative to the border of the element. 
    /// </summary>
    public enum GrabHandleAnchor
    {
        /// <summary>
        ///     Adorners will be placed outside (but touching) the border
        /// </summary>
        Outside,

        /// <summary>
        ///     Adorners will be centered on the border
        /// </summary>
        Centered,

        /// <summary>
        ///     Adorners will be placed inside (but touching) the border
        /// </summary>
        Inside
    }

    /// <summary>
    /// Position of grab handles
    /// </summary>
    [Flags]
    public enum GrabHandles
    {
        /// <summary>
        /// TopLeft
        /// </summary>
        TopLeft = 0x01,

        /// <summary>
        /// TopCenter
        /// </summary>
        TopCenter = 0x02,

        /// <summary>
        /// TopRight
        /// </summary>
        TopRight = 0x04,

        /// <summary>
        /// LeftCenter
        /// </summary>
        LeftCenter = 0x08,

        /// <summary>
        /// RightCenter
        /// </summary>
        RightCenter = 0x10,

        /// <summary>
        /// BottomLeft
        /// </summary>
        BottomLeft = 0x20,

        /// <summary>
        /// BottomCenter
        /// </summary>
        BottomCenter = 0x40,
        
        /// <summary>
        /// BottomRight
        /// </summary>
        BottomRight = 0x80,

        /// <summary>
        /// Corners
        /// </summary>
        Corners = 0xa5,

        /// <summary>
        /// Centers
        /// </summary>
        Centers = 0x5a,

        /// <summary>
        /// All
        /// </summary>
        All = 0xFF,
        
        /// <summary>
        /// None
        /// </summary>
        None = 0x00
    }

    /// <summary>
    /// Information describing a grab handle adorner
    /// </summary>
    public class GrabHandleAdorner : Adorner
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">element we're adorning</param>
        public GrabHandleAdorner(UIElement element) : base(element)
        {
            _children = new VisualCollection(this);                
            CreateHandles();
            DrawHandles();
        }   

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">element we're adorning</param>
        /// <param name="anchor">specifies a way of anchoring grab handles relative to element border</param>
        /// <param name="grabHandles">flags indicating which grab handles to create</param>
        public GrabHandleAdorner(UIElement element, GrabHandleAnchor anchor, GrabHandles grabHandles) : base(element)
        {
            _children = new VisualCollection(this);                
            _anchor = anchor;
            _grabHandles = grabHandles;
            CreateHandles();
            DrawHandles();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">element we're adorning</param>
        /// <param name="anchor">specifies a way of anchoring grab handles relative to element border</param>
        /// <param name="grabHandles">flags indicating which grab handles to create</param>
        /// <param name="size">Size of grab handles</param>
        /// <param name="pen">Pen</param>
        /// <param name="brush">Brush</param>
        public GrabHandleAdorner(UIElement element, GrabHandleAnchor anchor, GrabHandles grabHandles, Size size, Pen pen, Brush brush) : base(element)
        {
            _children = new VisualCollection(this);                
            if (pen == null)
            {
                throw new ArgumentNullException("pen");
            }

            if (brush == null)
            {
                throw new ArgumentNullException("brush");
            }

            _anchor = anchor;
            _grabHandles = grabHandles;
            _adornerSize = size;
            _adornerPen = pen;
            _adornerBrush = brush;
            CreateHandles();
            DrawHandles();
        }
    
        #endregion Constructors

        //  Public Methods

        /// <summary>
        /// If the size of the adorner is larger than the adorned element, we don't want to be
        /// positioned at the element's UL corner.  Instead, position ourselves so the grab
        /// handles will be properly positioned.
        /// </summary>
        /// <param name="transform">The transform applied to the object the adorner adorns</param>
        /// <returns>Transform to apply to the adorner</returns>
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            double offsetX, offsetY;            
            GeneralTransformGroup desiredTransform = new GeneralTransformGroup();

            if (transform == null)
            {
                throw new ArgumentNullException("transform");
            }

            desiredTransform.Children.Add(transform);

            switch (_anchor)
            {
                case GrabHandleAnchor.Outside:
                    offsetX = -_adornerSize.Width;
                    offsetY = -_adornerSize.Height;
                    desiredTransform.Children.Add(new TranslateTransform(offsetX, offsetY));                    
                    break;

                case GrabHandleAnchor.Centered:
                    offsetX = -_adornerSize.Width / 2;
                    offsetY = -_adornerSize.Height / 2;
                    desiredTransform.Children.Add(new TranslateTransform(offsetX, offsetY));                    
                    break;

                case GrabHandleAnchor.Inside:
                    break;
            }

            return desiredTransform;
        }

        /// <summary>
        /// Determines whether or not the given visual is a grab handle.
        /// </summary>
        /// <param name="visual">visual to test</param>
        /// <returns>GrabHandles (enum) value if the visual is a grab handle; GrabHandle.None otherwise</returns>
        public virtual GrabHandles GetGrabHandleFromVisual(Visual visual)
        {
            foreach (GrabHandles key in _handles.Keys)
            {
                if (_handles[key] == visual)
                {
                    return key;
                }
            }
            return GrabHandles.None;
        }

        //  Public Properties

        /// <summary>
        /// Grab handle flags
        /// </summary>
        public GrabHandles GrabHandles
        {
            get { return _grabHandles; }
            set
            {
                _handles.Clear();
                _grabHandles = value;
                CreateHandles();
                _dirty = true;
                InvalidateMeasure();
            }
        }

        /// <summary>
        /// How to draw grab handles relative to the border of the element
        /// </summary>
        public GrabHandleAnchor GrabHandleAnchor
        {
            get { return _anchor; }
            set
            {
                _anchor = value;
                _dirty = true;
                InvalidateMeasure();
            }
        }

        /// <summary>
        /// Size of grab handle's bounding box; used to position grab handle
        /// </summary>
        public Size Size
        {
            get { return _adornerSize; }
            set
            {
                _adornerSize = value;
                _dirty = true;
                InvalidateMeasure();
            }
        }

        /// <summary>
        /// Pen with which to draw
        /// </summary>
        public Pen Pen
        {
            get { return _adornerPen; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _adornerPen = value;
                _dirty = true;
                InvalidateMeasure();
            }
        }

        /// <summary>
        /// Brush with which to draw
        /// </summary>
        public Brush Brush
        {
            get { return _adornerBrush; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _adornerBrush = value;
                _dirty = true;
                InvalidateArrange();
            }
        }

        //  Protected Methods
 
        #region Protected Methods

        /// <summary>
        /// Measure adorner
        /// </summary>
        protected override Size MeasureOverride(Size size)
        {
            Size desiredSize = size;
            
            desiredSize = base.MeasureOverride(size);

            switch (_anchor)
            {
                case GrabHandleAnchor.Outside:
                    desiredSize.Width += _adornerSize.Width * 2;
                    desiredSize.Height += _adornerSize.Height * 2;
                    break;

                case GrabHandleAnchor.Centered:
                    desiredSize.Width += _adornerSize.Width;
                    desiredSize.Height += _adornerSize.Height;
                    break;

                case GrabHandleAnchor.Inside:
                    break;
            }

            return desiredSize;
        }

        /// <summary>
        /// Arrange grab handles
        /// </summary>
        protected override Size ArrangeOverride(Size size)
        {
            Size finalSize;

            finalSize = base.ArrangeOverride(size);

            foreach (GrabHandles key in _handles.Keys)
            {
                PositionGrabHandle(key);
            }
            if (_dirty)
            {
                DrawHandles();
            }
            return finalSize;
        }

        /// <summary>
        /// Position a grab handle
        /// </summary>
        /// <param name="key">grab handle to position</param>
        protected virtual void PositionGrabHandle(GrabHandles key)
        {
            Point point = GetGrabHandlePosition(key, DesiredSize);
            TranslateTransform transform = new TranslateTransform(point.X, point.Y);
            ((DrawingVisual)_handles[key]).Transform =  transform;
        }

        /// <summary>
        /// Determine the anchor point at which the given Adorner should draw itself.
        /// </summary>
        /// <param name="key">flag indicating which handle to position</param>
        /// <param name="size">size of adorner</param>
        /// <returns>Point position of grab handle</returns>
        protected virtual Point GetGrabHandlePosition(GrabHandles key, Size size)
        {
            double x, y;
                  
            switch (key)
            {
                case GrabHandles.TopLeft: 
                    x = 0;
                    y = 0;
                    break;

                case GrabHandles.TopCenter :
                    x = size.Width / 2 - _adornerSize.Width / 2;
                    y = 0;
                    break;

                case GrabHandles.TopRight : 
                    x = size.Width - _adornerSize.Width;
                    y = 0;
                    break;

                case GrabHandles.LeftCenter :
                    x = 0;
                    y = size.Height / 2 - _adornerSize.Height / 2;
                    break;

                case GrabHandles.RightCenter :
                    x = size.Width - _adornerSize.Width;
                    y = size.Height / 2 - _adornerSize.Height / 2;
                    break;

                case GrabHandles.BottomLeft :
                    x = 0;
                    y = size.Height - _adornerSize.Height;
                    break;

                case GrabHandles.BottomCenter :
                    x = size.Width / 2 - _adornerSize.Width / 2;
                    y = size.Height - _adornerSize.Height;
                    break;

                case GrabHandles.BottomRight :
                    x = size.Width - _adornerSize.Width;
                    y = size.Height - _adornerSize.Height;
                    break;

                default:
                    x = 0;  // shut up the compiler
                    y = 0;  // ditto
                    throw new Exception();
            }
            return new Point(x, y);
        }

        /// <summary>
        ///   Derived class must implement to support Visual children. The method must return
        ///    the child at the specified index. Index must be between 0 and GetVisualChildrenCount-1.
        ///
        ///    By default a Visual does not have any children.
        ///
        ///  Remark: 
        ///       During this virtual call it is not valid to modify the Visual tree. 
        /// </summary>
        protected override Visual GetVisualChild(int index)
        {            
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
        ///  Derived classes override this property to enable the Visual code to enumerate 
        ///  the Visual children. Derived classes need to return the number of children
        ///  from this method.
        ///
        ///    By default a Visual does not have any children.
        ///
        ///  Remark: During this virtual method the Visual tree must not be modified.
        /// </summary>        
        protected override int VisualChildrenCount
        {           
            get 
            { 
                if(_children == null)
                {
                    throw new ArgumentOutOfRangeException("_children is null");
                }                
                return _children.Count; 
            }
        } 
        

        #endregion Protected Methods

        //  Private Methods

        #region Private Methods

        /// <summary>
        /// Create grab handles
        /// </summary>
        private void CreateHandles()
        {
            for (GrabHandles handle = GrabHandles.TopLeft; handle <= GrabHandles.BottomRight; handle = ((GrabHandles)(((int)handle) << 1)))
            {
                if ((handle & _grabHandles) != 0)
                {
                    DrawingVisual visual = new DrawingVisual();
                    _handles.Add(handle, visual);
                    _children.Add(visual);
                }
            }
        }

        /// <summary>
        /// Draw grab handles
        /// </summary>
        private void DrawHandles()
        {
            for (GrabHandles handle = GrabHandles.TopLeft; handle <= GrabHandles.BottomRight; handle = ((GrabHandles)(((int)handle) << 1)))
            {
                if ((handle & _grabHandles) != 0)
                {
                    DrawingVisual visual = (DrawingVisual)_handles[handle];

                    using (DrawingContext context = visual.RenderOpen())
                    {
                        context.DrawRectangle(_adornerBrush, _adornerPen, new Rect(0, 0, _adornerSize.Width, _adornerSize.Height));
                    }
                }
            }

            _dirty = false;
        }

        #endregion Private Methods

        //  Private Fields

        #region Private Fields
        private VisualCollection _children;        
        private GrabHandleAnchor _anchor = GrabHandleAnchor.Centered;
        private Size _adornerSize = new Size(9, 9);
        private Brush _adornerBrush = Brushes.White;
        private Pen _adornerPen = new Pen(Brushes.Black, 1);
        private ListDictionary _handles = new ListDictionary();
        private GrabHandles _grabHandles = GrabHandles.All;

        private bool _dirty = false;

        #endregion Private Fields
    }
}