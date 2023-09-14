// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Security;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// This is to reduce the dependency on the old CoreUI code
    /// 




    public static class PointUtil
    {      
        private static Type GetPointUtilType()
        {        
            Type tsw = typeof(UIElement);
            Assembly assembly = tsw.Assembly;

            Type type = assembly.GetType("MS.Internal.PointUtil");

            return type;
        }

        private static Point InvokeStaticMethod(Type type, string name, object[] args)
        {
            return (Point)type.InvokeMember(name, 
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Static,
                null, 
                null, 
                args,
                System.Globalization.CultureInfo.InvariantCulture);
        }


        /// <summary>
        ///     Convert a point from "client" coordinate space of a window into
        ///     the coordinate space of the root element of the same window.
        /// </summary>
        public static Point ClientToRoot(Point pt, PresentationSource presentationSource)
        {
            object[] args = {pt, presentationSource};
           
            return InvokeStaticMethod(GetPointUtilType(),"ClientToRoot", args);
        }

        /// <summary>
        ///     Convert a point from the coordinate space of a root element of
        ///     a window into the "client" coordinate space of the same window.
        /// </summary>
        public static Point RootToClient(Point pt, PresentationSource presentationSource)
        {
            object[] args = {pt, presentationSource};
           
            return InvokeStaticMethod(GetPointUtilType(),"RootToClient", args);

        }
        
        /// <summary>
        ///     Convert a point from "above" the coordinate space of a
        ///     visual into the the coordinate space "below" the visual.
        /// </summary>
        public static Point ApplyVisualTransform(Point pt, Visual v, bool inverse)
        {

            object[] args = {pt, v, inverse};
           
            return InvokeStaticMethod(GetPointUtilType(),"ApplyVisualTransform", args);
        }

        /// <summary>
        ///     Convert a point from "client" coordinate space of a window into
        ///     the coordinate space of the screen.
        /// </summary>
        public static Point ClientToScreen(Point ptClient, PresentationSource presentationSource)
        {
                 
            object[] args = {ptClient, presentationSource};
           
            return InvokeStaticMethod(GetPointUtilType(),"ClientToScreen", args);
        }
        
        /// <summary>
        ///     Convert a point from the coordinate space of the screen into
        ///     the "client" coordinate space of a window.
        /// </summary>
        public static Point ScreenToClient(Point ptScreen, PresentationSource presentationSource)
        {
             object[] args = {ptScreen, presentationSource};
           
            return InvokeStaticMethod(GetPointUtilType(),"ScreenToClient", args);
        }

        /// <summary>
        ///     Gets the matrix that will convert a point from "above" the coordinate space of a visual
        ///     into the the coordinate space "below" the visual
        /// </summary>
        internal static Matrix GetVisualTransform(Visual v)
        {
            Matrix m = Matrix.Identity;

            if (v != null)
            {
                Transform transform = VisualTreeHelper.GetTransform(v);
                if (transform != null)
                {
                    Matrix cm = transform.Value;
                    m = Matrix.Multiply(m, cm);
                }

                Vector offset = VisualTreeHelper.GetOffset(v);
                m.Translate(offset.X, offset.Y);
            }

            return m; 
        }

        /// <summary>
        ///     Converts a rectangle from element co-ordinate space to that of the root visual
        /// </summary>
        /// <param name="rectElement">The rectangle to be converted</param>
        /// <param name="element">The element whose co-ordinate space you wish to convert from</param>
        /// <param name="presentationSource">The PresentationSource which hosts the specified Visual.  This is passed in for performance reasons.</param>
        /// <returns>The rectangle in the co-ordinate space of the root visual</returns>
        public static Rect ElementToRoot(Rect rectElement, Visual element, PresentationSource presentationSource)
        {
            GeneralTransform transformElementToRoot = element.TransformToAncestor(presentationSource.RootVisual);
            Rect rectRoot = transformElementToRoot.TransformBounds(rectElement);

            return rectRoot;
        }

        /// <summary>
        ///     Converts a rectangle from root visual co-ordinate space to Win32 client
        /// </summary>
        /// <remarks>
        ///     RootToClient takes into account device DPI settings to convert to/from WPF's assumed 96dpi
        ///     and any "root level" transforms applied to the root such as "right-to-left" inversions.
        /// </remarks>
        /// <param name="rectRoot">
        ///     The rectangle to be converted
        /// </param>
        /// <param name="presentationSource">
        ///     The PresentationSource which hosts the root visual.  This is passed in for performance reasons.
        /// </param>
        /// <returns>
        ///     The rectangle in Win32 client co-ordinate space
        /// </returns>
        public static Rect RootToClient(Rect rectRoot, PresentationSource presentationSource)
        {
            CompositionTarget target = presentationSource.CompositionTarget;
            Matrix matrixRootTransform = PointUtil.GetVisualTransform(target.RootVisual);
            Rect rectRootUntransformed = Rect.Transform(rectRoot, matrixRootTransform);
            Matrix matrixDPI = target.TransformToDevice;
            Rect rectClient = Rect.Transform(rectRootUntransformed, matrixDPI);

            return rectClient;
        }

    }
}

