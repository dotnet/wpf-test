// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// Description:  Implements the FixedPageRect 
//

namespace D2Payloads
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows.Controls.Primitives;  // PageSource
    using System.Diagnostics;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Threading;

    //=====================================================================
    /// <summary>
    /// Rect to present the page. Zoom the page if necessary. 
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public sealed class FixedPageRect : Panel
    {
        //--------------------------------------------------------------------
        //
        // Ctors
        //
        //---------------------------------------------------------------------

        #region Ctors
        public FixedPageRect(DocumentPage documentPage) : base()
        {
            Debug.Assert(documentPage != null);
            _documentPage = documentPage;
            _pageFrame = new MyBorder();
            _pageFrame.SetValue(Border.BorderBrushProperty, Brushes.Black);
            _pageFrame.SetValue(Border.BorderThicknessProperty, new Thickness(2.0));
            this.Children.Add(_pageFrame);
            
            Debug.Assert(_documentPage.Visual != null);
            // There might be a case where a visual associated with a page was 
            // inserted to a visual tree before. It got removed later, but GC did not
            // destroy its parent yet. To workaround this case always check for the parent
            // of page visual and disconnect it, when necessary.
            Visual currentParent = (Visual) VisualTreeHelper.GetParent(_documentPage.Visual);
            if (currentParent != null)
            {
                Debug.Assert(currentParent is Border, "The parent should be a Border");
                Border borderParent = (Border)currentParent;
                borderParent.Child = null;
            }

            // Now add the page visual to this rect. 
            _pageFrame.Child = (UIElement) _documentPage.Visual;
        }
        #endregion Ctors
        
        //--------------------------------------------------------------------
        //
        // Public Properties
        //
        //---------------------------------------------------------------------

        #region Public Properties
        #endregion Public Properties

        //--------------------------------------------------------------------
        //
        // Public Events
        //
        //---------------------------------------------------------------------

        //--------------------------------------------------------------------
        //
        // Protected Methods
        //
        //---------------------------------------------------------------------

        #region Protected Methods
        /// <summary>
        /// Content measurement.
        /// </summary>
        /// <param name="availableSize">Available size.</param>
        /// <returns>Computed desired size.</returns>
        protected sealed override Size MeasureOverride(Size availableSize)
        {
            Trace.WriteLine(string.Format("FixedPageRect.MeasureOverride {0}", availableSize));
            _pageFrame.Measure(PageFrameSize);
            return _documentPage.Size;
        }


        /// <summary>
        /// Content arrangement.
        /// </summary>
        /// <param name="finalSize">Size that element should use to arrange itself and its children.</param>
        protected sealed override Size ArrangeOverride(Size finalSize)
        {
            Trace.WriteLine(string.Format("FixedpageRect.ArrangeOverride {0}", finalSize));
            UIElementCollection vc = this.Children;
            if (_documentPage.Visual == null)
            {
                // Remove existing visiual children.
                vc.Clear();
            }
            else
            {
                // Add visual representing the page contents. For performance reasons
                // first check if it is already insered there.
                if (!(vc.Count == 1 && vc[0] == _pageFrame))
                {
                    vc.Clear();
                    vc.Add(_pageFrame);
                }
                Size pageSize = PageFrameSize;
                // Build some scale transform here.
                Size contentSize = new Size(
                    Math.Max(finalSize.Width, 0.0),
                    Math.Max(finalSize.Height, 0.0));

                Rect pageArea = FixedUtil.SizingHelper(contentSize, pageSize, 
                    TextAlignment.Center,
                    AlignmentY.Center,
                    Stretch.Uniform
                    );

                Transform scaleTransform = new ScaleTransform(pageArea.Width / pageSize.Width, pageArea.Height / pageSize.Height);
                Transform translateTransform = new TranslateTransform(pageArea.Left, pageArea.Top);

                TransformGroup tg = new TransformGroup();
                tg.Children.Add(scaleTransform);
                tg.Children.Add(translateTransform);

                _pageFrame.Arrange(new Rect(pageSize));
                _pageFrame.MyOffset = new Vector();
                _pageFrame.MyTransform = tg;
            }
            base.ArrangeOverride(finalSize);
            return finalSize;
        }
        #endregion Protected Methods

        //--------------------------------------------------------------------
        //
        // Internal Methods
        //
        //---------------------------------------------------------------------

        #region Internal Methods
        #endregion Internal Methods

        //--------------------------------------------------------------------
        //
        // Internal Properties
        //
        //---------------------------------------------------------------------
        internal Size PageFrameSize
        {
            get
            {
                Size childConstraint = new Size(_documentPage.Size.Width, _documentPage.Size.Height);
                childConstraint.Width = Math.Max(0.0, childConstraint.Width + 4);
                childConstraint.Height = Math.Max(0.0, childConstraint.Height + 4);
                return childConstraint; 
            }
        }
        //--------------------------------------------------------------------
        //
        // Private Class
        //
        //---------------------------------------------------------------------
        private class MyBorder : Border
        {
            private UIElement _child;

            /// <summary>
            /// Adds the child as Visual Child only.
            /// </summary>
            public override UIElement Child
            {
                get
                {
                    return _child;
                }
                
                set
                {
                    if(_child != value)
                    {
                        // notify the visual layer that the old child has been removed.
                        RemoveVisualChild(_child);                    

                        _child = value;

                        // notify the visual layer about the new child.
                        AddVisualChild(value);

                        InvalidateMeasure();
                        InvalidateVisual(); //ensure re-rendering
                    }
                }
            }
        
            /// <summary>
            /// Gets and sets the offset.
            /// </summary>
            public Vector MyOffset
            {
                get
                {
                    return this.VisualOffset;
                }                
                set
                {
                    this.VisualOffset = value;
                }
            }

        /// <summary>
        /// Gets or sets the transform of this Visual.
        /// </summary>
            public Transform MyTransform
            {
                get
                {
                    return this.VisualTransform;
                }                
                set
                {
                    this.VisualTransform = value;
                }
            }            

        }


        //--------------------------------------------------------------------
        //
        // private Properties
        //
        //---------------------------------------------------------------------

        //--------------------------------------------------------------------
        //
        // Private Fields
        //
        //---------------------------------------------------------------------

        #region Private Fields
        private readonly DocumentPage _documentPage;
        private readonly MyBorder _pageFrame;
        #endregion Private Fields
    }
}

