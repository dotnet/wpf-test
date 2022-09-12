// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// Description: Implements the FixedGrid 
//

namespace D2Payloads
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows.Controls.Primitives;  // PageSource
    using System.Diagnostics;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Threading;

    //=====================================================================
    /// <summary>
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public sealed class FixedGrid : FrameworkElement, IServiceProvider
    {
        //--------------------------------------------------------------------
        //
        // Ctors
        //
        //---------------------------------------------------------------------

        #region Ctors
        static FixedGrid()
        {
            FocusableProperty.OverrideMetadata(typeof(FixedGrid), new FrameworkPropertyMetadata(true));
        }

        public FixedGrid(FixedDocViewer viewer)
        {
            _children = new VisualCollection(this);                
            _viewer = viewer;
            _grid = new Grid();
            _grid.ShowGridLines = true;
            _grid.SetResourceReference(Grid.BackgroundProperty, SystemColors.ControlDarkDarkBrushKey);
        }
        #endregion Ctors
        

        #region IServiceProvider Members
        /// <summary>
        /// Returns service objects associated with this control.
        /// </summary>
        /// <remarks>
        /// FixedDocument currently supports ITextView.
        /// </remarks>
        /// <param name="serviceType">
        /// Specifies the type of service object to get.
        /// </param>
        object IServiceProvider.GetService(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }

//             VerifyAccess();

#if DISABLED_BY_TOM_BREAKING_CHANGE
            if (serviceType == typeof(TextContainer))
            {
                return this.TextContainer;
            }
#endif // DISABLED_BY_TOM_BREAKING_CHANGE

            return null;
        }
        #endregion IServiceProvider Members


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
        
        /// <summary>


        #region Protected Methods
        /// <summary>
        /// Content measurement.
        /// </summary>
        /// <param name="availableSize">Available size.</param>
        /// <returns>Computed desired size.</returns>
        protected sealed override Size MeasureOverride(Size availableSize)
        {
            Trace.WriteLine(string.Format("FixedGrid:MeasureOverride {0}", availableSize));

            if (VisualTreeHelper.GetParent(_grid) == null)
            {
                _children.Add(_grid);
                //UpdateGrid();
            }

            _grid.Measure(availableSize);
            return _grid.DesiredSize;
        }


        /// <summary>
        /// Content arrangement.
        /// </summary> 
        /// <param name="finalSize">Size that element should use to arrange itself and its children.</param>
        protected sealed override Size ArrangeOverride(Size finalSize)
        {
            Trace.WriteLine(string.Format("FixedGrid:ArrangeOverride {0}", finalSize));
            _grid.Arrange(new Rect(finalSize));
            return finalSize;
        }
        #endregion Protected Methods

        //--------------------------------------------------------------------
        //
        // Internal Methods
        //
        //---------------------------------------------------------------------

        #region Internal Methods
        internal void UpdateGrid()
        {
            Debug.Assert(_grid != null);
            _grid.Children.Clear();
            _grid.RowDefinitions.Clear();
            _grid.ColumnDefinitions.Clear();

            // Setup Rows
            for (int rowDef = 0; rowDef < Rows; rowDef++)
            {
                RowDefinition rdef = new RowDefinition();
                rdef.Height = new GridLength(1.0, GridUnitType.Star);

                _grid.RowDefinitions.Add(rdef);
            }

            // Setup Columns
            for (int columnDef = 0; columnDef < Columns; columnDef++)
            {
                ColumnDefinition cdef = new ColumnDefinition();
                cdef.Width = new GridLength(1.0, GridUnitType.Star);

                _grid.ColumnDefinitions.Add(cdef);
            }

            // Setup cells
            _gridCells = new FixedCell[Rows * Columns];
            for (int cell = 0; cell < Rows * Columns; cell++)
            {
                FixedCell  pageCell;
                pageCell = new FixedCell();
                pageCell.SetValue(Grid.RowProperty, cell / Columns);
                pageCell.SetValue(Grid.ColumnProperty, cell % Columns);
                _grid.Children.Add(pageCell);
                _gridCells[cell] = pageCell;
            }
        }


        internal void UpdatePage(int pageCellNumber, DocumentPage documentPage)
        {
            Debug.Assert(pageCellNumber < _grid.Children.Count);
            FixedCell pageCell = (FixedCell)_grid.Children[pageCellNumber];
            pageCell.DocumentPage = documentPage;
        }

        internal void EnsureAttachEditor()
        {
#if DISABLED_BY_TOM_BREAKING_CHANGE
            if (!(bool)this.GetValue(TextEditor.IsEnabledProperty) && DoesContentSupportEditing)
            {
                this.SetValue(TextEditor.IsEnabledProperty, true);
                this.SetValue(TextEditor.IsReadOnlyProperty, true);
            }
#endif // DISABLED_BY_TOM_BREAKING_CHANGE
        }

        internal void EnsureDetachEditor()
        {
#if DISABLED_BY_TOM_BREAKING_CHANGE
            this.SetValue(TextEditor.IsEnabledProperty, false);
#endif // DISABLED_BY_TOM_BREAKING_CHANGE
        }
        #endregion Internal Methods

        //--------------------------------------------------------------------
        //
        // Internal Properties
        //
        //---------------------------------------------------------------------

        #region Internal Properties
        internal int Rows
        {
            get
            {
                return _viewer.Rows;
            }
        }


        internal int Columns
        {
            get
            {
                return _viewer.Columns;
            }
        }


#if DISABLED_BY_TOM_BREAKING_CHANGE
        internal TextContainer TextContainer
        {
            get
            {
                if (_textContainer == null)
                {
                    if (this.DocumentPaginator != null)
                    {
                        IServiceProvider isp = this.DocumentPaginator as IServiceProvider;
                        if (isp != null)
                        {
                            _textContainer = isp.GetService(typeof(TextContainer)) as TextContainer;
                        }
                    }
                }
                return _textContainer;
            }
        }
#endif // DISABLED_BY_TOM_BREAKING_CHANGE

        internal IDocumentPaginatorSource DocumentPaginator
        {
            get
            {
                if (_viewer.Source != null)
                {
                    return _viewer.Source.FixedDoc as IDocumentPaginatorSource;
                }
                return null;
            }
        }

        internal FixedCell[] Cells
        {
            get
            {
                return _gridCells;
            }
        }

        internal DocumentPage[] AvailablePages
        {
            get
            {
                return _viewer.AvailablePages;
            }
        }
        #endregion Internal Properties


        //--------------------------------------------------------------------
        //
        // Private Methods
        //
        //---------------------------------------------------------------------

        #region Private Methods
        #endregion Private Methods

        #region IMultiPageScope
        /// <summary>
        /// Hit-Test on this multi-page UI scope to return a DocumentPage 
        /// that contains this point
        /// </summary>
        /// <param name="point">Point in pixel unit, relative to the UI Scope's coordinates</param>
        /// <returns>A DocumentPage that is hit or null if no page is hit</returns>
        DocumentPage GetDocumentPageFromPoint(Point point)
        {
            // This HitTest results in inner-most Visual
            PointHitTestResult result = (PointHitTestResult) VisualTreeHelper.HitTest(this, point);
            Visual v = (result != null) ? result.VisualHit : null;

            FixedCell pe = null;
            // Traverse the visual parent chain until we encounter a PageElement
            while (v != null)
            {
                pe = v as FixedCell;
                if (pe != null)
                {
                    break;
                }
                v = (Visual) VisualTreeHelper.GetParent(v);
            }

            // if we hit some PageCell
            if (pe != null)
            {
                return pe.DocumentPage;
            }
            return null;
        }


        /// <summary>
        /// Return DocumentPages currently active in this scope
        /// </summary>
        /// <returns></returns>
        DocumentPage[] GetActiveDocumentPages()
        {
            FixedCell[] cells = this.Cells;
            ArrayList ar = new ArrayList(cells.Length);
            foreach (FixedCell cell in cells)
            {
                if (cell.DocumentPage != null)
                {
                    ar.Add(cell.DocumentPage);
                }
            }

            return (DocumentPage[])ar.ToArray(typeof(DocumentPage));
        }

        #endregion IMultiPageScope

        //--------------------------------------------------------------------
        //
        // private Properties
        //
        //---------------------------------------------------------------------

        #region Private Properties
        // Only support editing if the content provider
        // support TextView service.
        private bool DoesContentSupportEditing
        {
            get
            {
#if DISABLED_BY_TOM_BREAKING_CHANGE
                // Microsoft:10/8/2004: TextView and TextContainer are now interal.
                if (this.DocumentPaginator != null)
                {
                    IServiceProvider isp = this.DocumentPaginator as IServiceProvider;
                    if (isp != null)
                    {
                        TextContainer tc = (TextContainer)isp.GetService(typeof(TextContainer));
                        if (tc != null)
                        {
                            return true;
                        }
                    }
                }
#endif // DISABLED_BY_TOM_BREAKING_CHANGE
                return false;
            }
        }
        #endregion Private Properties

        //--------------------------------------------------------------------
        //
        // Private Fields
        //
        //---------------------------------------------------------------------

        #region Private Fields
        private VisualCollection _children;                        
        private readonly FixedDocViewer _viewer;
        private Grid _grid;                     // Grid to display pages
        private FixedCell[] _gridCells;
        #endregion Private Fields
    }
}


