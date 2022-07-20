// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Defines api for interaction between Annotations test framework
//               and an arbitrary content control.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Reflection;
using System.Collections;
using System.Windows.Annotations;
using Annotations.Test.Reflection;
using System.Windows.Automation;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Annotations.Test.Framework;
using System.Collections.Generic;
using System.Security.Permissions;
using Microsoft.Test.Diagnostics;

namespace Avalon.Test.Annotations
{
	public abstract class ATextControlWrapper
    {
        #region Constructor

        protected ATextControlWrapper(Control target)
        {
            _target = target;
        }

        #endregion        

        #region Zoom Methods

        abstract public void SetZoom(double zoomPercent);
        abstract public double GetZoom();
        abstract public void ZoomIn();
        abstract public void ZoomOut();

        /// <summary>
        /// Use ZoomIn/ZoomOut apis to reach the desired zoom percent.
        /// </summary>
        /// <param name="targetPercent">Desired viewer zoom percentage.</param>
        public void IcrementalZoomTo(double targetPercent)
        {
            double previousZoom = 0;
            
            if (GetZoom() < targetPercent) {
                TestSuite.Current.printStatus("Zooming in to " + targetPercent + "%...");
                while (GetZoom() < targetPercent) 
                {
                    if (GetZoom() == previousZoom)
                        throw new Exception("Requested zoom level outsize allowed limits.");
                    previousZoom = GetZoom();
                    ZoomIn();
                }
            }
            else
            {
                TestSuite.Current.printStatus("Zooming out to " + targetPercent + "%...");
                while (GetZoom() > targetPercent) 
                {
                    if (GetZoom() == previousZoom)
                        throw new Exception("Requested zoom level outsize allowed limits.");
                    previousZoom = GetZoom();
                    ZoomOut();
                }
            }
        }

        #endregion

        #region General Navigation

        abstract public void GoToStart();
        abstract public void GoToEnd();        

        abstract public void ScrollUp(int n);
        abstract public void ScrollDown(int n);

        /// <summary>
        /// Bring start of selection into view.
        /// </summary>
        abstract public void BringIntoView(ISelectionData selection);

        #endregion

        #region Paginated Accessors

        /// <summary>
        /// Get lowest visible page number.
        /// </summary>
        abstract public int FirstVisiblePage
        {
            get;
        }

        /// <summary>
        /// Get highest visible page page number.
        /// </summary>
        abstract public int LastVisiblePage
        {
            get;
        }

        /// <summary>
        /// Total number of pages in document.
        /// </summary>
        abstract public int PageCount
        {
            get;
        }

        public void PageUp()
        {
            PageUp(1);
        }
        public virtual void PageUp(int n)
        {
            for (int i = 0; i < n; i++) SyncCommand(NavigationCommands.PreviousPage);
        }
        public void PageDown()
        {
            PageDown(1);
        }
        public virtual void PageDown(int n)
        {
            for (int i = 0; i < n; i++) SyncCommand(NavigationCommands.NextPage);
        }

        /// <summary>
        /// Bring PageNumber into view.
        /// </summary>
        /// <param name="page">0 based page number.</param>
        public virtual void GoToPage(int pageNum)
        {
            ValidatePageNumber(pageNum);
            while (FirstVisiblePage > pageNum)
                PageUp(1);
            while (LastVisiblePage < pageNum)
                PageDown(1);
            EnsurePageIsVisible(pageNum);
        }

        /// <summary>
        /// Return whether or not page with PageNumber is currently visible.
        /// </summary>
        /// <param name="pageNum"></param>
        /// <returns>True if page number is visible.</returns>
        public bool PageIsVisible(int pageNum)
        {
            return FirstVisiblePage <= pageNum && LastVisiblePage >= pageNum;
        }

        /// <summary>
        /// Throw if given page number is not within the range of FirstVisiblePage and LastVisiblePage.
        /// </summary>
        /// <param name="page"></param>
        protected void EnsurePageIsVisible(int page)
        {
            if (!PageIsVisible(page))
                throw new ArgumentException("Expected page number '" + page + "' to be visible, but visible pages were (" + FirstVisiblePage + ", " + LastVisiblePage + ").");
        }

        /// <summary>
        /// Throw if page number is not >= 0 and less than the number of PageCount.
        /// </summary>
        protected void ValidatePageNumber(int page)
        {
            if (page < 0 || page >= PageCount)
                throw new ArgumentException("Page number '" + page + "' is invalid, must be between 0 and " + (PageCount - 1) + ".");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create MS.Internal.Annotations.TextAnchor from given TextRange.
        /// </summary>
        /// <param name="textRange"></param>
        /// <returns></returns>
        public object MakeTextAnchor(TextRange textRange)
        {

            Type type = null;

            if (SystemInformation.WpfVersion == WpfVersions.Wpf30)
            {
                type = typeof(Annotation).Assembly.GetType("MS.Internal.Annotations.TextAnchor");
            }
            else
            {
                type = typeof(Annotation).Assembly.GetType("System.Windows.Annotations.TextAnchor");
            }

            object anchor = ReflectionHelper.GetInstance(type, new Type[0], new object[0]);
            IList segments = ReflectionHelper.GetProperty(textRange, "TextSegments") as IList;
            foreach (object segment in segments)
            {
                ReflectionHelper.InvokeMethod(anchor, "AddTextSegment", new object[] { ReflectionHelper.GetProperty(segment, "Start"), ReflectionHelper.GetProperty(segment, "End") });
            }

            return anchor;
        }

        /// <summary>
        /// Convert the definition of a TextPointer into a position in screen coordinates.
        /// </summary>
        /// <param name="horizontalJustification">Determines wether we return the top, middle, or bottom of the character rect.</param>
        /// <returns>The screen coordinates that correspond to given TextPointer definition.</returns>
        abstract public Point PointerToScreenCoordinates(int page, int offset, LogicalDirection direction, HorizontalJustification horizontalJustification);

        #endregion

        #region Protected Methods

        protected void SyncCommand(int n, RoutedCommand command, IInputElement target)
        {
            for (int i = 0; i < n; i++)
            {
                command.Execute(null, target);
                DispatcherHelper.DoEvents();
            }
        }

        /// <summary>
        /// Perform a RoutedCommand synchronously N times on the DocumentViewer.
        /// </summary>
        protected void SyncCommand(int n, RoutedCommand command)
        {
            for (int i = 0; i < n; i++)
                SyncCommand(command);
        }

        /// <summary>
        /// Perform a RoutedCommand synchronously on the DocumentViewer.
        /// </summary>
        /// <param name="command"></param>
        protected void SyncCommand(RoutedCommand command)
        {
            command.Execute(null, Target);
            DispatcherHelper.DoEvents();
        }

        /// <summary>
        /// Perform a RoutedCommand synchronously on the DocumentViewer.
        /// </summary>
        /// <param name="command"></param>
        protected void SyncCommand(RoutedCommand command, object data)
        {
            command.Execute(data, Target);
            DispatcherHelper.DoEvents();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Set/Get the content of the TextControl.
        /// </summary>
        abstract public IDocumentPaginatorSource Document
        {
            get;
            set;
        }

        /// <summary>
        /// Get ScrollViewer associated with control or null if none exists.
        /// </summary>
        public ScrollViewer ScrollViewer
        {
            get
            {
                return new VisualTreeWalker<ScrollViewer>().FindChildren(Target)[0];
            }
        }

        /// <summary>
        /// Get the current text selection.
        /// </summary>
        public object Selection
        {
            get
            {
                return SelectionModule.Selection;
            }
        }

        public Proxies.System.Windows.Annotations.AnnotationService Service
        {
            get
            {
                return Proxies.System.Windows.Annotations.AnnotationService.GetService(Target);
            }
        }

        /// <summary>
        /// The text Control that is wrapped by this object.
        /// </summary>
        public Control Target
        {
            get
            {
                return _target;
            }
        }

        /// <summary>
        /// Module capable of performing selections on text in a specific Control.
        /// </summary>
        public virtual SelectionModule SelectionModule
        {
            get { return selectionModule; }
            set { selectionModule = value; }
        }

        #endregion

        #region Fields

        private Control _target;
        
        private SelectionModule selectionModule;

        #endregion

        #region Classes

        public enum HorizontalJustification
        {
            Top,
            Middle,
            Bottom
        }

        /// <summary>
        /// Comparers rects by ascending x then y position.
        /// </summary>
        protected class RectComparer : IComparer
        {
            #region IComparer Members

            public int Compare(object x, object y)
            {
                Rect A = (Rect)x;
                Rect B = (Rect)y;
                if (A.Left == B.Left && A.Top == B.Top)
                    return 0;
                // Same row.
                if (A.Top == B.Top)
                    return (A.Left <= B.Left) ? -1 : 1;
                // Different row.
                else
                    return (A.Top <= B.Top) ? -1 : 1;
            }

            #endregion
        }

        #endregion
    }
}
