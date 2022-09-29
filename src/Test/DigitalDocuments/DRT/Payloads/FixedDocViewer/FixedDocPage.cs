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
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Threading;


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
            CreateDefaultStyle();
        }

        private static void CreateDefaultStyle()
        {
            /*
                Default Style is a grid
                <DockPanel>
                    <!-- ToolBar Here -->
                    <Grid ID="ContentGrid" Background=" "> 
                        <PageView />
                        <PageView />
                        <PageView />
                    </Grid>
                    <!-- Statusbar Here -->
                    <!-- Scrollbar somewhere -->
                </DockPanel>
            */

            // Create the main content area
            Brush backgroundBrush = new LinearGradientBrush(Color.FromScRgb(1.0F, .95F, .96F, .95F), Color.FromScRgb(1.0F, .94F, .92F, .85F), 0);

            FrameworkElementFactory contentFactory = new FrameworkElementFactory(typeof(Grid), "SIFixedDocViewer");
            contentFactory.SetValue(DockPanel.DockProperty, Dock.Bottom);
            contentFactory.SetValue(Grid.IDProperty, "ContentGrid");
            contentFactory.SetValue(Grid.Background, backgroundBrush);

            // Setup the entire VisualTree
            FrameworkElementFactory dpFactory = new FrameworkElementFactory(typeof(DockPanel), "SIFixedDocViewer");
            dpFactory.AppendChild(contentFactory);

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
        ///     Automatic determination of current UIContext. Use alternative constructor
        ///     that accepts a UIContext for best performance.
        /// </remarks>
        public FixedDocViewer() : base()
        {
            Init();
        }

        /// <summary>
        ///     FixedDocViewer construction
        /// </summary>
        /// <remarks>
        ///     Best performance constructor
        /// </remarks>
        /// <param name="context">UIContext to place this instance within</param>
        public FixedDocViewer(UIContext context) : base(context)
        {
            Init();
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
        public static readonly DependencyProperty SourceProperty
              = DependencyProperty.Register("Source",
                                            typeof(FixedDocSource),
                                            typeof(FixedDocViewer),
                                            new FrameworkPropertyMetadata((FixedDocSource)null, FrameworkPropertyMetadataOptions.AffectsMeasure)
                                          );


        /// <summary>
        /// Get/Set Source property that references an external Doc stream. 
        /// </summary>
        public FixedDocSource Source
        {
            get
            {
                if (!_sourceValid)
                {
                    _source = (FixedDocSource)GetValue(SourceProperty);
                    _sourceValid = true;
                }
                return _source;
            }

            set
            {
                // Cleanup
                FixedDocSource src = this.Source;
                if (src != null)
                {
                    FixedDocData fixedDocData = src as FixedDocData;
                    if (fixedDocData != null)
                    {
                        PageSource.Content = null;
                    }
                }

                SetValue(SourceProperty, value);

                // 
                if (value != null)
                {
                    FixedDocData fixedDocData = value as FixedDocData;
                    if (fixedDocData != null)
                    {
                        PageSource.Content = fixedDocData.FixedDoc;
                        //OnContentUpdated();
                    }
                }
            }
        }
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
        protected sealed override Size MeasureCore(Size availableSize)
        {
            if (double.IsInfinity(constraint.Width) || double.IsInfinity(constraint.Height))
            {
                return new Size(0.0, 0.0);
            }
            return constraint;
        }

        /// <summary>
        /// Content arrangement.
        /// </summary>
        /// <param name="arrangeSize">Size that element should use to arrange itself and its children.</param>
        protected sealed override void ArrangeCore(Size arrangeSize)
        {
            _children.Clear();
            for (int i = 0, nCount = Pages.Count; i < nCount; i++)
            {
                DocumentPage 
                FixedPage fp = SyncGetPage(i, false /*forceReload*/);

                if (h >= arrangeSize.Height)
                    break;

                ArrangeChildHelper(fp, fp.DesiredSize, arrangeSize);

                Thickness computedMargin = ComputeMargin(fp);

                _children.Add(fp);
                fp.SetLayoutOffset(new Vector(computedMargin.Left, h + computedMargin.Top));
                h += (fp.ComputedSize.Height + computedMargin.Top + computedMargin.Bottom);
            }
        }
        #endregion Protected Methods

        //--------------------------------------------------------------------
        //
        // Internal Methods
        //
        //---------------------------------------------------------------------

        internal PageSource PageSource
        {
            get
            {
                if (_pageSource == null)
                {
                    _pageSource = new PageSource();
                }
                return _pageSource;
            }
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


        //--------------------------------------------------------------------
        //
        // private Properties
        //
        //---------------------------------------------------------------------
        private void _LocateParts()
        {
            _grid = _FindPart(this, "ContentGrid");
        }


        private FrameworkElement _FindPart(Visual root, string ID)
        {
            FrameworkElement fe = root as FrameworkElement;

            //Check to see if this node has our property set to true.
            if (fe != null && fe.ID == ID)
            {
                return fe;
            }

            //Now check our children.
            int count = VisualTreeHelper.GetChildrenCount(root);            
            for(int i = 0; i < count; i++)
            {
                Visual child = VisualTreeHelper.GetChild(root, i);
                FrameworkElement r = FindMarkedElement(dp, child);
                if (r != null)
                {
                    return r;
                }
            }

            return null;
        }
        //--------------------------------------------------------------------
        //
        // Private Methods
        //
        //---------------------------------------------------------------------

        #region Private Methods
        private void Init()
        {
            _children = new VisualCollection(this);                
        }
        #endregion Private Methods

        //--------------------------------------------------------------------
        //
        // Private Fields
        //
        //---------------------------------------------------------------------
        #region Private Fields
        private VisualCollection _children;                
        private List<DocumentPage> _pages;      // list of pages currently to be displayed
        private Grid _grid;                     // Grid to display pages
        private PageSource _pageSource;
        private FixedDocSource _source;
        private bool _sourceValid;
        #endregion Private Fields
    }
}


