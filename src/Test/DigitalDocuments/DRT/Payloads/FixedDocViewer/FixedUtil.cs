// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// Description:  Implements the FixedUtil 
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
    public static class FixedUtil
    {
        //--------------------------------------------------------------------
        //
        // Ctors
        //
        //---------------------------------------------------------------------
        
        //--------------------------------------------------------------------
        //
        // Public Methods
        //
        //---------------------------------------------------------------------
        #region Public methods

        // Calc rect based on alignments and stretch setting. Respect original
        // aspect ratio. 
        public static Rect SizingHelper(
                Size containingSize,
                Size naturalSize,
                TextAlignment textAlignment,
                AlignmentY verticalAlignment,
                Stretch stretch
                )
        {
            Rect rect = new Rect(new Point(), naturalSize);

            // Degenerate case: We need a natural size > 0 to do scaling/comparisons.
            // If we need to scale with aspect ratio (stretching uniformly), we return here to avoid div by zero.
            if (
                (DoubleUtil.LessThanOrClose(naturalSize.Width, 0)
                  || DoubleUtil.LessThanOrClose(naturalSize.Height, 0))
                && (stretch == Stretch.Uniform
                    || stretch == Stretch.UniformToFill)
                )
            {
                return rect;
            }

            // STEP 1: Determine the size of the content given the size of its container, its Stretch
            //         property, and its natural size / aspect ratio.
            switch (stretch)
            {
                // Fill:     Object assumes the size of the container.  Aspect ratio is not respected.
                case Stretch.Fill:
                    rect.Size = containingSize;
                    break;

                // Uniform:  Object scales with aspect ratio as large as possible while still fitting in the containing size.
                case Stretch.Uniform:
                    if (containingSize.Width / naturalSize.Width < containingSize.Height / naturalSize.Height)
                    {
                        rect.Width *= (containingSize.Width / naturalSize.Width);
                        rect.Height *= (containingSize.Width / naturalSize.Width);
                    }
                    else
                    {
                        rect.Width *= (containingSize.Height / naturalSize.Height);
                        rect.Height *= (containingSize.Height / naturalSize.Height);
                    }
                    break;

                // UniformToFill:  Object scales with aspect ratio as small as possible while still filling the containing size.
                case Stretch.UniformToFill:
                    if (containingSize.Width / naturalSize.Width > containingSize.Height / naturalSize.Height)
                    {
                        rect.Width *= (containingSize.Width / naturalSize.Width);
                        rect.Height *= (containingSize.Width / naturalSize.Width);
                    }
                    else
                    {
                        rect.Width *= (containingSize.Height / naturalSize.Height);
                        rect.Height *= (containingSize.Height / naturalSize.Height);
                    }

                    break;

                case Stretch.None:
                    // Always assume the natural size of the object.  contentSize already contains this size.
                    break;

                default:
                    // 
                    break;
            }

            // STEP 2: Align the content within the container.
            switch (textAlignment)
            {
                case TextAlignment.Center:
                    rect.X = (containingSize.Width - rect.Width) * 0.5;
                    break;

                case TextAlignment.Right:
                    rect.X = containingSize.Width - rect.Width;
                    break;

                // Justify and unrecognized types should be treated as the default Left alignment.
                case TextAlignment.Left:
                case TextAlignment.Justify:
                default:
                    break;
            }
            switch (verticalAlignment)
            {
                case AlignmentY.Center:
                    rect.Y = (containingSize.Height - rect.Height) * 0.5;
                    break;

                case AlignmentY.Bottom:
                    rect.Y = containingSize.Height - rect.Height;
                    break;

                // Unrecognized types should be treated as the default Top alignment.
                case AlignmentY.Top:
                default:
                    break;
            }

            return rect;
        }


        // Depth First Search of Part by traversing VisualTree. 
        public static FrameworkElement FindPartByID(DependencyObject root, string ID)
        {
            FrameworkElement fe = root as FrameworkElement;
            if (fe != null && fe.Name == ID)
            {
                return fe;
            }

            // Depth first traversing
            int count = VisualTreeHelper.GetChildrenCount(root);
            for(int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(root,i);
                FrameworkElement part = FindPartByID(child, ID);
                if (part != null)
                {
                    return part;
                }
            }
            return null;
        }
        #endregion Public Methods

        //--------------------------------------------------------------------
        //
        // Public Properties
        //
        //---------------------------------------------------------------------


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


        //--------------------------------------------------------------------
        //
        // Internal Methods
        //
        //---------------------------------------------------------------------


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
    }
}

