// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// Description: Implements the FixedDocViewer 
//

[assembly: System.Security.AllowPartiallyTrustedCallers()]

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
    using System.Windows.Input;
    using System.Windows.Markup;         // IAddChild
    using System.Windows.Shapes;
    using System.Windows.Threading;

    //=====================================================================
    /// <summary>
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public sealed class FixedDocViewer : Control
    {
        //--------------------------------------------------------------------
        //
        // Ctors
        //
        //---------------------------------------------------------------------

        #region Ctors
        static FixedDocViewer()
        {
            Style defaultStyle = GetDefaultStyle();
            StyleProperty.OverrideMetadata(typeof(FixedDocViewer), new FrameworkPropertyMetadata(defaultStyle));
            FocusableProperty.OverrideMetadata(typeof(FixedDocViewer), new FrameworkPropertyMetadata(true));
        }

        private static Style GetDefaultStyle()
        {
            /*
                Default Style is a grid
                <Grid Width="100%" Height="100%">
                    <RowDefinition />
                    <RowDefinition Height="Auto" />
                    <!-- ToolBar Here -->
                    <Canvas ID="Content" Grid.Row="0" Grid.Column="0">
                        <FixedGrid > 
                            <FixedCell />
                            <FixedCell />
                            <FixedCell />
                            <FixedCell />
                        </FixedGrid>
                    </Canvas>
                    <Button ID="Usage" Grid.Row="1" Grid.Column="0" >
                            "*String(Usage)"
                    </Button>
                    <!-- Scrollbar somewhere -->
                </Grid>
            */

            // Create the main content area
            FrameworkElementFactory contentFactory = new FrameworkElementFactory(typeof(Canvas), "SIFixedDocViewerContent");
            contentFactory.SetValue(DockPanel.DockProperty, Dock.Top);
            contentFactory.SetValue(FixedGrid.NameProperty, "Content");
            contentFactory.SetValue(Grid.RowProperty, 0);
            contentFactory.SetValue(Grid.ColumnProperty, 1);

            // Create the Usage Bar
            FrameworkElementFactory usageFactory = new FrameworkElementFactory(typeof(Button), "SIFixedDocViewerUsage");
            usageFactory.SetValue(Button.NameProperty, "Usage");
            usageFactory.SetValue(Button.ContentProperty, Usage);
            usageFactory.SetValue(Grid.RowProperty, 1);
            usageFactory.SetValue(Grid.ColumnProperty, 0);

            // Setup the entire VisualTree
            FrameworkElementFactory dpFactory = new FrameworkElementFactory(typeof(Grid), "SIFixedDocViewerGrid");

            FrameworkElementFactory row1Factory = new FrameworkElementFactory(typeof(RowDefinition), "SIFixedDocViewerRow1");
            row1Factory.SetValue(RowDefinition.HeightProperty, new GridLength(1, GridUnitType.Star));

            FrameworkElementFactory row2Factory = new FrameworkElementFactory(typeof(RowDefinition), "SIFixedDocViewerRow2");
            row2Factory.SetValue(RowDefinition.HeightProperty, new GridLength(1, GridUnitType.Auto));

            dpFactory.AppendChild(row1Factory);
            dpFactory.AppendChild(row2Factory);
            dpFactory.AppendChild(contentFactory);
            dpFactory.AppendChild(usageFactory);

            Style ds = new Style(typeof(FixedDocViewer));
            ControlTemplate template = new ControlTemplate(typeof(FixedDocViewer));
            template.VisualTree = dpFactory;
            ds.Setters.Add(new Setter(Control.TemplateProperty, template));
            return ds;
        }


        /// <summary>
        ///     Default FixedDocViewer constructor
        /// </summary>
        /// <remarks>
        ///     Automatic determination of current Dispatcher. Use alternative constructor
        ///     that accepts a Dispatcher for best performance.
        /// </remarks>
        public FixedDocViewer() : base()
        {
            _Init();
        }

        #endregion Ctors
        
        //--------------------------------------------------------------------
        //
        // Public Methods
        //
        //---------------------------------------------------------------------
        #region Public methods

        /// <summary>
        /// Called when our Template's Tree is created.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _LocateParts();
        }

        // PageNumber starting from 1
        public void MoveToPage(int proposedPageNumber)
        {
            Trace.WriteLine(string.Format("MoveToPage {0} CurrentTotalPage {1}", proposedPageNumber, _source.FixedDoc.DocumentPaginator.PageCount));
            if (proposedPageNumber > _source.FixedDoc.DocumentPaginator.PageCount)
            {
                proposedPageNumber = _source.FixedDoc.DocumentPaginator.PageCount;
            }

            if (proposedPageNumber < 1)
            {
                proposedPageNumber = 1;
            }

            this.LayoutUpdated += new EventHandler(OnLayoutUpdated);
            _topLeftPage = proposedPageNumber - 1;
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(_GetPages), null);
        }
        #endregion Public Methods

        //--------------------------------------------------------------------
        //
        // Public Properties
        //
        //---------------------------------------------------------------------

        #region Public Properties
        /// <summary>
        /// Dynamic Property to reference an external document stream.
        /// </summary>
        public static readonly DependencyProperty SourceProperty = 
                DependencyProperty.Register(
                        "Source",
                        typeof(FixedDocSource),
                        typeof(FixedDocViewer),
                        new FrameworkPropertyMetadata(
                                 (FixedDocSource)null,
                                 FrameworkPropertyMetadataOptions.AffectsMeasure, //MetaData flags
                                 new PropertyChangedCallback(Source_Changed))); //changed callback


        private static void Source_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FixedDocViewer v = (FixedDocViewer) d;
            v._OnSourceChanged((FixedDocSource) e.OldValue, (FixedDocSource) e.NewValue);
        }

        public IDocumentPaginatorSource DocumentPaginator
        {
            get
            {
                if (_source != null)
                {
                    return _source.FixedDoc;
                }
                return null;
            }
        }

        /// <summary>
        /// Get/Set Source property that references an external Doc stream. 
        /// </summary>
        public FixedDocSource Source
        {
            get { return _source; }
            set { SetValue(SourceProperty, value); }
        }


        public int Rows
        {
            get
            {
                return _rows;
            }
        }


        public int Columns
        {
            get
            {
                return _columns;
            }
        }


        public FixedGrid FixedGrid
        {
            get
            {
                Debug.Assert(_fixedGrid != null);
                return _fixedGrid;
            }
        }

        public DocumentPage[] AvailablePages
        {
            get
            {
                Debug.Assert(_loadedPages != null);
                return _loadedPages;
            }
        }

        public int PageNumber
        {
            get
            {
                return _topLeftPage + 1;
            }
        }
        #endregion Public Properties

        //--------------------------------------------------------------------
        //
        // Public Events
        //
        //---------------------------------------------------------------------

        #region Public Event
        public event EventHandler PaginationCompleted;
        public event GetPageCompletedEventHandler GetPageCompleted;
        public event EventHandler PagesDisplayed;
        #endregion Public Event

        //--------------------------------------------------------------------
        //
        // Protected Methods
        //
        //---------------------------------------------------------------------

        #region Protected Methods
        /// <summary>
        /// This method responds to KeyUp events and is used
        /// to implement the Number + Enter method of specifying a page to
        /// jump to.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            Trace.WriteLine(string.Format("OnKeyUp {0}", e.Key));
            //If this event has been handled elsewhere
            //we should just return.
            if (e.Handled)
            {
                return;
            }

            //Store the value of the number key that was pressed.
            //We use this to build the Page number that the user types,
            //one digit at a time.
            int keyValue = -1;

            //Determine the key that was pressed and do something with it:
            //- If it's a number key (on the numpad or elsewhere), we update our keyValue to correspond with it.
            //- If it's the Enter key and a non-zero page number has been entered
            //  we attempt to jump to the page typed in
            //  (or the last page if it's greater than PageCount) and we reset the
            //  entered Page number to 0.
            //- If it's the delete or Backspace key we delete the first digit of the entered number.
            switch (e.Key)
            {
                case Key.F1:
                case Key.F2:
                case Key.F4:
                case Key.F6:
                case Key.F9:
                    {
                        if (e.Key == Key.F1)
                        {
                            _rows = 1;
                            _columns = 1;
                        }
                        else if (e.Key == Key.F2)
                        {
                            _rows = 1;
                            _columns = 2;
                        }
                        else if (e.Key == Key.F4)
                        {
                            _rows = 2;
                            _columns = 2;
                        }
                        else if (e.Key == Key.F6)
                        {
                            _rows = 2;
                            _columns = 3;
                        }
                        else if (e.Key == Key.F9)
                        {
                            _rows = 3;
                            _columns = 3;
                        }
                        _UpatePageArray();
                        MoveToPage(_GetExternalPageNumber(_topLeftPage));
                    }
                    break;

                case Key.H:     // Left 
                case Key.L:     // Right
                case Key.J:     // Next Row
                case Key.K:     // Previous Row
                case Key.Prior:     // PageUp
                case Key.Next:      // PageDown
                //case Key.Home:        // Home
                //case Key.End:
                    {
                        int delta = 0;
                        _enteredPageNumber = 0;
                        switch (e.Key)
                        {
                            case Key.H:
                            case Key.Prior:
                                delta = -1;
                                break;

                            case Key.L:
                            case Key.Next:
                                delta = 1;
                                break;

                            case Key.J:
                                delta = _columns;
                                break;

                            case Key.K:
                                delta = -1 * _columns;
                                break;

                                /*
                            case Key.Home:
                                delta = -1 * _topLeftPage;
                                break;

                            case Key.End:
                                delta = PageSource.PageCount - _topLeftPage - 1;
                                break;
                                */
                        }
                        MoveToPage(_GetExternalPageNumber(_topLeftPage + delta));
                    }
                    break;

                case Key.D0:
                case Key.NumPad0:
                    keyValue = 0;
                    break;

                case Key.D1:
                case Key.NumPad1:
                    keyValue = 1;
                    break;

                case Key.D2:
                case Key.NumPad2:
                    keyValue = 2;
                    break;

                case Key.D3:
                case Key.NumPad3:
                    keyValue = 3;
                    break;

                case Key.D4:
                case Key.NumPad4:
                    keyValue = 4;
                    break;

                case Key.D5:
                case Key.NumPad5:
                    keyValue = 5;
                    break;

                case Key.D6:
                case Key.NumPad6:
                    keyValue = 6;
                    break;

                case Key.D7:
                case Key.NumPad7:
                    keyValue = 7;
                    break;

                case Key.D8:
                case Key.NumPad8:
                    keyValue = 8;
                    break;

                case Key.D9:
                case Key.NumPad9:
                    keyValue = 9;
                    break;

                //Handle the Backspace and Del key here:
                case Key.Back:
                case Key.Delete:
                    _enteredPageNumber /= 10;  //Since _enteredPageNumber is an int, dividing by 10 effectively removes the Least Significant Difference.
                    e.Handled = true;
                    break;

                //Handle the Return/Enter key here:
                //(NOTE: Enter & Return have their own enumerations, but they
                //happen to be the same value, so we just check for Key.Return here)
                case Key.Return:
                    //We only jump to the page if a non-zero page number has been entered.
                    if (_enteredPageNumber > 0)
                    {
                        MoveToPage(_enteredPageNumber);
                    }
                    _enteredPageNumber = 0;

                    e.Handled = true;
                    break;
            }

            //If a number was entered, we tack it on to the end of the currently entered number.
            //If the number overflows, we set it to the value of the last entered key.
            if (keyValue != -1)
            {
                if (_enteredPageNumber * 10 + keyValue > MaxEnteredPageNumber)
                {
                    _enteredPageNumber = keyValue;
                }
                else
                {
                    _enteredPageNumber = _enteredPageNumber * 10 + keyValue;
                }

                //Since we got a valid keyValue, we need to mark that we've handled this event.
                e.Handled = true;
            }

            base.OnKeyUp(e);
        }

        /// <summary>
        /// This is the method that responds to the MouseWheel event.
        /// It pages forward or backward depending on the wheel direction.
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            if (e.Handled)
            {
                return;
            }

            // Go one page forward or back, depending on which way the wheel turned.
            if (e.Delta > 0)
            {
                MoveToPage(_GetExternalPageNumber(_topLeftPage - 1));
            }
            else
            {
                MoveToPage(_GetExternalPageNumber(_topLeftPage + 1));
            }

            e.Handled = true;
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

        #region Internal Properties
        #endregion Internal Properties     


        //--------------------------------------------------------------------
        //
        // Private Methods
        //
        //---------------------------------------------------------------------

        #region Private Methods
        private void _OnSourceChanged(FixedDocSource oldDoc, FixedDocSource newDoc)
        {
            if (oldDoc != null)
            {
                oldDoc.FixedDoc.DocumentPaginator.GetPageCompleted -= new GetPageCompletedEventHandler(_OnGetPageCompleted);
                _loadedPages = null;
                _topLeftPage = 0;
                _fixedGrid.EnsureDetachEditor();
            }

            _source = newDoc;
            
            if (newDoc != null)
            {
                FixedDocData fixedDocData = newDoc as FixedDocData;
                if (fixedDocData != null)
                {
                    // We now setup the grid
                    _UpatePageArray();
                    fixedDocData.FixedDoc.DocumentPaginator.GetPageCompleted += new GetPageCompletedEventHandler(_OnGetPageCompleted);
                    ((DynamicDocumentPaginator)fixedDocData.FixedDoc.DocumentPaginator).PaginationCompleted += new EventHandler(_OnPaginationCompleted);
                    MoveToPage(_GetExternalPageNumber(0));
                    _fixedGrid.EnsureAttachEditor();
                }
            }
        }

        /// <summary>
        /// Our content is done loading, we need to inform PageSource.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _OnPaginationCompleted(Object sender, EventArgs e)
        {
            Trace.WriteLine("_OnPaginationCompleted: FixedDoc now loaded");
            // Tell PageSource the async loading is over
            MoveToPage(_GetExternalPageNumber(_topLeftPage));
            if (PaginationCompleted != null)
            {
                PaginationCompleted(this, new EventArgs());
            }
        }


        private void _LocateParts()
        {
            FrameworkElement contentHost = FixedUtil.FindPartByID(this, "Content");
            Debug.Assert(contentHost != null);
            AdornerDecorator adorner = new AdornerDecorator();
            adorner.Child = FixedGrid;
            ((IAddChild)contentHost).AddChild(adorner);
        }


        // This needs to be called when row and column changes
        private void _UpatePageArray()
        {
            // Clear the loaded page array
            _loadedPages = new DocumentPage[_rows * _columns];
            _fixedGrid.UpdateGrid();
        }


        // Getting _rows * _columns Pages starting from _topLeftPage
        private object _GetPages(object args)
        {
            Trace.WriteLine(string.Format("GetPages {0} {1}x{2}", _topLeftPage, _rows, _columns));
            if (_source != null && _source.FixedDoc != null)
            {
                for (int pageNumber = _topLeftPage; pageNumber < _topLeftPage + _rows * _columns; pageNumber++)
                {
                    _source.FixedDoc.DocumentPaginator.GetPageAsync(pageNumber, pageNumber);
                }
            }
            return null;
        }

        private object _GetPage(object arg)
        {
            Trace.WriteLine(string.Format("GetPage {0}", (int)arg));
            int pageNumber = (int)arg;
            Debug.Assert(pageNumber >= _topLeftPage && pageNumber < _topLeftPage + _rows * _columns);
            if (_source != null && _source.FixedDoc != null)
            {
                _source.FixedDoc.DocumentPaginator.GetPageAsync(pageNumber, pageNumber);
            }
            return null;
        }


        private void _OnGetPageCompleted(object sender, GetPageCompletedEventArgs e)
        {
            Trace.WriteLine(string.Format("_OnGetPageCompleted {0} TopLeft {1} {2}x{3}", e.PageNumber, _topLeftPage, _rows, _columns));
            if (e.PageNumber < _topLeftPage || e.PageNumber >= _topLeftPage + _rows * _columns)
            {
                Trace.WriteLine(string.Format("_OnGetPageCompleted page out of bound {0}", e.PageNumber));
                return;
            }

            if (GetPageCompleted != null)
            {
                GetPageCompleted(this, e);
            }

            if (!e.Cancelled && e.Error == null)
            {
                int relativePage = e.PageNumber - _topLeftPage;
                _loadedPages[relativePage] = e.DocumentPage;
                _fixedGrid.UpdatePage(relativePage, e.DocumentPage);
            }
        }


        private int _GetExternalPageNumber(int internalPageNumber)
        {
            // return internalPageNumber + 1;
            return internalPageNumber;
        }


        private void _Init()
        {
            _loadedPages = new DocumentPage[0];
            _fixedGrid = new FixedGrid(this);
            _topLeftPage  = 0;
            _rows    = 1;
            _columns = 1;
        }


        private void OnLayoutUpdated(object sender, EventArgs args)
        {
            Trace.WriteLine(string.Format("LayoutUpdated"));
            if (_source != null)
            {
                for (int i = 0; i < _loadedPages.Length; i++)
                {
                    if (_topLeftPage + i < _source.FixedDoc.DocumentPaginator.PageCount && _loadedPages[i] == null)
                    {
                        Trace.WriteLine(string.Format("LayoutUpdated: still waiting for page {0}", _topLeftPage + i));
                        return;
                    }
                }

                // if all valid pages are here. Fire pages displayed event
                if (PagesDisplayed != null)
                {
                    PagesDisplayed(this, new EventArgs());
                }
                this.LayoutUpdated -= new EventHandler(OnLayoutUpdated);
            }
        }
        #endregion Private Methods

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
        private int _topLeftPage;
        private int _rows;
        private int _columns;
        private DocumentPage[] _loadedPages;      // list of pages currently available to be displayed
        private FixedGrid _fixedGrid;                     // Grid to display pages
        private FixedDocSource _source;

        // Input Handling 
        private int _enteredPageNumber;         // Input-on-progress
        private const int MaxEnteredPageNumber = 2 ^ 32 - 1;
        private const string Usage =
@"Page Navigation ( h: PreviousPage l: NextPage  k: PreviousRow  j: NextRow   PAGEUP: PreviousPage  PAGEDOWN: NextPage    Mouse Wheel Scrolling is supported. ) 
Change Grid ( F1: 1x1  F2: 1x2  F4: 2x2  F6: 2x3  F9 3x3 )";
        #endregion Private Fields
    }
}
