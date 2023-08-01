// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//////////////////////////////////////////////////////////////////////////////
//                                                                          //
//                                                                          //
//  Provides helpers for Avalon widgets and windows.                        //
//                                                                          //
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using Microsoft.Test.Win32;

using LHPoint = System.Windows.Point;

namespace Avalon.Test.CoreUI.Trusted
{
    /// <summary>Provides helpers for Avalon widgets and windows.</summary>
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    public static partial class ElementUtils
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public methods;



        /// <summary>
        /// Finds the first element in the given scope with the specified id.
        /// </summary>
        /// <param name='scope'>Scope from which to begin search.</param>
        /// <param name='id'>Name of framework element.</param>
        /// <returns>The element found, null otherwise.</returns>
        public static FrameworkElement FindElement(
            FrameworkElement scope, string id)
        {
            if (scope == null)
            {
                throw new ArgumentNullException("scope");
            }
            if (scope.Name == id)
            {
                return scope;
            }

            DependencyObject node = scope;
            IEnumerator enumerator;
            enumerator = LogicalTreeHelper.GetChildren(node).GetEnumerator();
            if (enumerator == null)
            {
                // WORKAROUND: Return null when fixed if code gets here.
                return FindElementInVisuals(scope, id);
            }
            while (enumerator.MoveNext())
            {
                FrameworkElement element =
                    enumerator.Current as FrameworkElement;
                if (element != null)
                {
                    FrameworkElement result = FindElement(element, id);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            // WORKAROUND: . Return null when fixed if code gets here.
            return FindElementInVisuals(scope, id);
        }

        /// <summary>
        /// Finds the first element in the given scope with the specified id
        /// in the visual tree.
        /// </summary>
        /// <param name='scope'>Scope from which to begin search.</param>
        /// <param name='id'>Name of framework element.</param>
        /// <returns>The element found, null otherwise.</returns>
        /// <remarks>
        /// Note that this is not what the end-user would expect to use.
        /// However, it is a valid search operation, and it is required
        /// to work around a Window 

        public static FrameworkElement FindElementInVisuals(
            DependencyObject scope, string id)
        {
            if (scope == null)
            {
                throw new ArgumentNullException("scope");
            }
            FrameworkElement result = scope as FrameworkElement;
            if (result != null && result.Name == id)
            {
                return result;
            }

            // We haven't found the value yet....
            // recursively look at the visual children
            int count = VisualTreeHelper.GetChildrenCount(scope);
            for(int i = 0; i < count; i++)
            {
                // Common base class for Visual and Visual3D is DependencyObject
                DependencyObject visual = VisualTreeHelper.GetChild(scope, i);

                result = FindElementInVisuals(visual, id);
                if (result != null)
                {
                    return result;
                }
            }

            // not found
            return null;
        }

        /// <summary>
        /// Finds the first element in the given scope with the given property value
        /// in the visual tree.
        /// </summary>
        /// <param name="scope">Starting scope for the search.</param>
        /// <param name="dp">Dependency property to query.</param>
        /// <param name="value">Desired value.</param>
        /// <param name="includeScope">Do we test the starting scope for the value?</param>
        /// <returns>The element found, null otherwise.</returns>
        /// <remarks>
        /// The search is done depth-first.
        /// </remarks>
        public static DependencyObject FindVisualByPropertyValue(
            DependencyObject scope,
            DependencyProperty dp, 
            object value, 
            bool includeScope)
        {
            if (scope == null)
            {
                throw new ArgumentNullException("scope");
            }

            if (includeScope)
            {
                // see if the scope itself has the right value
                object nodeValue = scope.GetValue(dp);
                if (Object.Equals(value, nodeValue))
                {
                    return scope;
                }
            }

            // We haven't found the value yet....
            // recursively look at the visual children
            int count = VisualTreeHelper.GetChildrenCount(scope);
            for(int i = 0; i < count; i++)
            {
                // Common base class for Visual and Visual3D is DependencyObject

                DependencyObject visual = VisualTreeHelper.GetChild(scope, i);
                DependencyObject result = FindVisualByPropertyValue(visual, dp, value, true);
                if (result != null)
                {
                    return result;
                }
            }

            // not found
            return null;
        }

        /// <summary>
        /// Gets the rectangle that bounds the specified element, relative
        /// to the client area of the window the element is in.
        /// </summary>
        /// <param name='element'>Element to get rectangle for.</param>
        /// <returns>The System.Windows.Rect that bounds the element.</returns>
        public static Rect GetClientRelativeRect(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            Visual parent = GetTopMostVisual(element);
            LHPoint[] points = GetRenderSizeBoxPoints(element);

            System.Windows.Media.GeneralTransform gt = element.TransformToAncestor(parent);
            System.Windows.Media.Transform t = gt as System.Windows.Media.Transform;
            if (t == null)
            {
                // GeneralTransform returned if element is a child of Viewport2DVisual3D.
                for (int i = 0; i < points.Length; i++)
                {
                    points[i] = gt.Transform(points[i]);
                }
            }
            else
            {
                Matrix m = t.Value;
                m.Transform(points);
            }

            // Calculate the regular Rectangle that encloses all the points.
            LHPoint topLeft, bottomRight;
            CalculateBoundingPoints(points, out topLeft, out bottomRight);

            // The root visual could have an offset from the client hwnd origin,
            // so we must add the offset to the points.
            Vector vector = VisualTreeHelper.GetOffset(parent);
            topLeft.X += vector.X;
            topLeft.Y += vector.Y;
            bottomRight.X += vector.X;
            bottomRight.Y += vector.Y;

            System.Windows.Point topLeftPoint = new System.Windows.Point(topLeft.X, topLeft.Y);
            PresentationSource source = PresentationSource.FromVisual(element);
            topLeftPoint = source.CompositionTarget.TransformToDevice.Transform(topLeftPoint);


            System.Windows.Point bottomRightPoint = new System.Windows.Point(bottomRight.X, bottomRight.Y);
            bottomRightPoint = source.CompositionTarget.TransformToDevice.Transform(bottomRightPoint);


            return new Rect(
                topLeftPoint.X, topLeftPoint.Y,
                bottomRightPoint.X - topLeftPoint.X,
                bottomRightPoint.Y - topLeftPoint.Y);

        }


        /// <summary>
        /// Gets the HwndSource associated with the specified window,
        /// possibly null.
        /// </summary>
        /// <param name='window'>Window to get HwndSource for.</param>
        /// <returns>
        /// The HwndSource instance associated with window, null if there 
        /// is none.
        /// </returns>
        /// <example>The following sample shows how to use this method.<code>...
        /// private void MyMethod() {
        ///   HwndSource source = ElementUtils.GetHwndSource(
        ///     Application.Current.MainWindow).
        ///   // Do something with source.
        /// }</code></example>        
        public static HwndSource GetHwndSource(Window window)
        {
            PresentationSource source;

            if (window == null)
            {
                throw new ArgumentNullException("window");
            }

            source = PresentationSource.FromVisual(window);

            return (HwndSource)source;
        }

        /// <summary>
        /// Gets the rectangle that bounds the specified element, relative
        /// to the top-left corner of the screen.
        /// </summary>
        /// <param name='element'>Element to get rectangle for.</param>
        /// <returns>The rectangle that bounds the element.</returns>
        public static Rect GetScreenRelativeRect(UIElement element)
        {
            NativeStructs.POINT topLeft;
            Rect clientRect;

            PresentationSource source;

            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            source = PresentationSource.FromVisual(element);

            if (source == null)
            {
                throw new InvalidOperationException("element is not connected to visual tree");
            }

            clientRect = GetClientRelativeRect(element);

            topLeft = new NativeStructs.POINT((int)clientRect.Left, (int)clientRect.Top);

            NativeMethods.ClientToScreen(((HwndSource)source).Handle, ref topLeft);

            return new Rect(topLeft.x, topLeft.y, clientRect.Width, clientRect.Height);
        }

        /// <summary>
        /// Retrieves the window an element belongs to.
        /// </summary>
        /// <param name="element">Element to retrieve window for.</param>
        /// <returns>The Window instance that holds the element.</returns>
        /// <remarks>
        /// If the window cannot be found, and exception is thrown. This
        /// API never returns null.
        /// </remarks>
        public static Window GetWindowFromElement(Visual element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            object currentElement = element;
            Window window = element as Window;
            while (window == null)
            {
                Visual visual = currentElement as Visual;
                if (visual != null && VisualTreeHelper.GetParent(visual) != null)
                {
                    currentElement = VisualTreeHelper.GetParent(visual);
                }
                else
                {
                    DependencyObject node = currentElement as DependencyObject;
                    if (node == null || LogicalTreeHelper.GetParent(node) == null)
                    {
                        string msg = "Cannot find window for element " +
                            element + " - unable to go further up from " +
                            " element " + currentElement;
                        throw new Exception(msg);
                    }
                    currentElement = LogicalTreeHelper.GetParent(node);
                }
                window = currentElement as Window;
            }
            return window;
        }

        /// <summary>Retrieves a HWND from a window.</summary>
        /// <param name='window'>Window to get handle from.</param>
        /// <returns>The window handle.</returns>
        public static IntPtr WindowToHwnd(Window window)
        {
            HwndSource source;

            if (window == null)
            {
                throw new ArgumentNullException("window");
            }

            source = GetHwndSource(window);

            return source.Handle;
        }

        #endregion Public methods;

        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        #region Private methods;

        /// <summary>
        /// Calculates the rectangle with sides parallel to the
        /// axis that bounds all the given points.
        /// </summary>
        private static void CalculateBoundingPoints(LHPoint[] points,
            out LHPoint topLeft, out LHPoint bottomRight)
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            for (int i = 0; i < points.Length; i++)
            {
                LHPoint p = points[i];
                if (p.X < minX) minX = p.X;
                if (p.X > maxX) maxX = p.X;
                if (p.Y < minY) minY = p.Y;
                if (p.Y > maxY) maxY = p.Y;
            }

            topLeft = new LHPoint(minX, minY);
            bottomRight = new LHPoint(maxX, maxY);
        }

        /// <summary>
        /// Gets an array of four bounding points for the computed
        /// size of the specified element. The top-left corner
        /// is (0;0) and the bottom-right corner is (width;height).
        /// </summary>
        private static LHPoint[] GetRenderSizeBoxPoints(UIElement element)
        {
            // Get the points for the rectangle and transform them.
            double height = element.RenderSize.Height;
            double width = element.RenderSize.Width;
            LHPoint[] points = new LHPoint[4];
            points[0] = new LHPoint(0, 0);
            points[1] = new LHPoint(width, 0);
            points[2] = new LHPoint(0, height);
            points[3] = new LHPoint(width, height);
            return points;
        }

        /// <summary>
        /// Gets the top-most visual for the specified visual element.
        /// </summary>
        private static Visual GetTopMostVisual(UIElement element)
        {
            PresentationSource source;

            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            source = PresentationSource.FromVisual(element);

            if (source == null)
            {
                throw new InvalidOperationException("The specified UIElement is not connected to a rendering Visual Tree.");
            }

            return source.RootVisual;
        }

        #endregion Private methods;
    }
}

