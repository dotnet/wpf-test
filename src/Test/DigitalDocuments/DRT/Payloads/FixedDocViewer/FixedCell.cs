// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// Description: Implements the FixedCell 
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
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public sealed class FixedCell : Panel
    {
        //--------------------------------------------------------------------
        //
        // Ctors
        //
        //---------------------------------------------------------------------

        #region Ctors
        public FixedCell() : base()
        {
            Margin = new Thickness(10.0, 5.0, 10.0, 5.0);
            this.SetResourceReference(Panel.BackgroundProperty, SystemColors.ControlDarkDarkBrushKey);
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
            Trace.WriteLine(string.Format("FixedCell.MeasureOverride {0}", availableSize));
            if (_pageRect != null)
            {
                _pageRect.Measure(availableSize);
                return _pageRect.DesiredSize;
            }
            return new Size();
        }


        /// <summary>
        /// Content arrangement.
        /// </summary>
        /// <param name="finalSize">Size that element should use to arrange itself and its children.</param>
        protected sealed override Size ArrangeOverride(Size finalSize)
        {
            Trace.WriteLine(string.Format("FixedCell.ArrangeOverride {0}", finalSize));
            int count = VisualTreeHelper.GetChildrenCount(this);
            if (count > 0)
            {
                Debug.Assert(VisualTreeHelper.GetChild(this,0) == _pageRect);
                _pageRect.Arrange(new Rect(finalSize));
            }
            return finalSize;
        }
        #endregion Protected Methods

        //--------------------------------------------------------------------
        //
        // Internal Methods
        //
        //---------------------------------------------------------------------

        #region Internal Methods
        internal DocumentPage  DocumentPage
        {
            get
            {
                return _docPage;
            }
            set
            {
                DocumentPage oldPage = _docPage;
                _docPage = value;
                if (oldPage != _docPage)
                {
                    if (oldPage != null)
                    {
                        this.Children.Clear();
                    }
                    if (_docPage != null && _docPage != DocumentPage.Missing)
                    {
                        _pageRect = new FixedPageRect(_docPage);
                        this.Children.Add(_pageRect);
                    }
                }
            }
        }
        #endregion Internal Methods

        //--------------------------------------------------------------------
        //
        // Internal Properties
        //
        //---------------------------------------------------------------------

        //--------------------------------------------------------------------
        //
        // Private Methods
        //
        //---------------------------------------------------------------------


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
        private FixedPageRect _pageRect;
        private DocumentPage _docPage;
        #endregion Private Fields
    }
}

