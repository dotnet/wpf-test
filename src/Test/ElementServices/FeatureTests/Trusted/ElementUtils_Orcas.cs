// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//////////////////////////////////////////////////////////////////////////////
//                                                                          //
//  ElementUtils.cs                                                         //
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
        #region Public methods

        /// <summary>
        /// Gets the rectangle that bounds the specified element3d, relative
        /// to the client area of the window the element3d is in.
        /// </summary>
        /// <param name='element3d'>UIElement3D to get rectangle for.</param>
        /// <returns>The System.Windows.Rect that bounds the element.</returns>
        public static Rect GetClientRelativeRect(UIElement3D element3d)
        {
            if (element3d == null)
            {
                throw new ArgumentNullException("element3d");
            }

            Visual parent = GetTopMostVisual(element3d);
            GeneralTransform3DTo2D uiElement3dToParentTransform = element3d.TransformToAncestor(parent);

            Rect elementBounds = uiElement3dToParentTransform.TransformBounds(VisualTreeHelper.GetContentBounds(element3d));

            // Add the root visual's offset from the client Hwnd origin.
            Vector vector = VisualTreeHelper.GetOffset(parent);
            elementBounds.Offset(vector);

            PresentationSource source = PresentationSource.FromDependencyObject(element3d);

            //Convert from element to device coordinates.
            System.Windows.Point topLeftPoint = source.CompositionTarget.TransformToDevice.Transform(elementBounds.TopLeft);
            System.Windows.Point bottomRightPoint = source.CompositionTarget.TransformToDevice.Transform(elementBounds.BottomRight);


            return new Rect(
                topLeftPoint.X, topLeftPoint.Y,
                bottomRightPoint.X - topLeftPoint.X,
                bottomRightPoint.Y - topLeftPoint.Y);
        }

        /// <summary>
        /// Gets the rectangle that bounds the specified 3d element, relative
        /// to the top-left corner of the screen.
        /// </summary>
        /// <param name='element3d'>UIElement3D to get rectangle for.</param>
        /// <returns>The rectangle that bounds the element.</returns>
        public static Rect GetScreenRelativeRect(UIElement3D element3d)
        {
            NativeStructs.POINT topLeft;
            Rect clientRect;

            PresentationSource source;

            if (element3d == null)
            {
                throw new ArgumentNullException("element3d");
            }

            source = PresentationSource.FromDependencyObject(element3d);

            if (source == null)
            {
                throw new InvalidOperationException("UIElement3D is not connected to visual tree");
            }

            clientRect = GetClientRelativeRect(element3d);

            topLeft = new NativeStructs.POINT((int)clientRect.Left, (int)clientRect.Top);

            NativeMethods.ClientToScreen(((HwndSource)source).Handle, ref topLeft);

            return new Rect(topLeft.x, topLeft.y, clientRect.Width, clientRect.Height);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Gets the top-most visual for the specified visual element.
        /// </summary>
        private static Visual GetTopMostVisual(UIElement3D element3d)
        {
            PresentationSource source;

            if (element3d == null)
            {
                throw new ArgumentNullException("element3d");
            }

            source = PresentationSource.FromDependencyObject(element3d);

            if (source == null)
            {
                throw new InvalidOperationException("The specified UIElement3D is not connected to a rendering Visual Tree.");
            }

            return source.RootVisual;
        }
        
        #endregion 
    }
}

