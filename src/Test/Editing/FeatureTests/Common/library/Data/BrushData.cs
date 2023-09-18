// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides data to be used for testing DependencyProperties of TextBox and RichTextBox.   


[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Data/BrushData.cs $")]

namespace Test.Uis.Data
{
    #region Namespaces.

    using System;
    using System.Threading;
    using System.Windows.Threading;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Animation;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// Provides information about interesting Brushes.
    /// </summary>
    public sealed class BrushData
    {

        #region Constructors.

        /// <summary>Hide the constructor.</summary>
        private BrushData() { }

        #endregion Constructors.


        #region Public methods.

        /// <summary>Checks whether two brushes are equal.</summary>
        /// <param name='brushA'>First brush to compare.</param>
        /// <param name='brushB'>Second brush to compare.</param>
        /// <remarks>
        /// Parameter values may be null. A null brush is equal to
        /// another null brush, and to nothing else.
        /// </remarks>
        public static bool AreBrushesEqual(Brush brushA, Brush brushB)
        {
            SolidColorBrush solidA;

            if (brushA == null && brushB == null)
            {
                return true;
            }
            if ((brushA == null && brushB != null) ||
                (brushA != null && brushB == null))
            {
                return false;
            }

            solidA = brushA as SolidColorBrush;
            if (solidA != null)
            {
                return solidA.Color.Equals(((SolidColorBrush)brushB).Color);
            }
            return false;
        }
        
        #endregion Public methods.


        #region Public properties.

        /// <summary>The Brush being encapsulated.</summary>
        public Brush Brush
        {
            get { return _value; }
        }

        /// <summary>Whether the brush is transparent.</summary>
        public bool IsTransparent
        {
            get { return _isTransparent; }
        }

        /// <summary>Gets some kind of GradientBrush.</summary>
        public static BrushData GradientBrush
        {
            get
            {
                foreach(BrushData result in Values)
                {
                    if (result.Brush is GradientBrush)
                    {
                        return result;
                    }
                }
                throw new Exception("Cannot find BrushData with GradientBrush value.");
            }
        }

        /// <summary>Interesting raw Brush values for testing.</summary>
        public static Brush[] BrushValues
        {
            get
            {
                Brush[] result;
                
                result = new Brush[Values.Length];
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = Values[i].Brush;
                }
                return result;
            }
        }

        /// <summary>Interesting BrushData values for testing.</summary>
        public static BrushData[] Values = new BrushData[] {
            // Simple, standard brush.
            FromBrush(new SolidColorBrush(Colors.White)),

            // Colored brush.
            FromBrush(new SolidColorBrush(Colors.Blue)),

            // Transparent brush.
            FromBrush(new SolidColorBrush(Colors.Transparent), true),

            // Diagonal gradient brush with light colors.
            FromBrush(new LinearGradientBrush(Colors.White, Colors.LightBlue, 45)),

            // Radial gradient brush with light colors.
            FromBrush(new RadialGradientBrush(Colors.White, Colors.LightGreen)),

            // Radial gradient brush with light colors.
            FromBrush(new RadialGradientBrush(Colors.White, Colors.LightGreen)),

            // DrawingBrush with an arbitrary drawing.
            FromBrush(new DrawingBrush(CreateDrawing())),

            // ImageBrush with a stretched image.
            FromBrush(CreateStretchedImageBrush()),

            // VisualBrush with tiled brush.
            FromBrush(CreateTiledVisualBrush()),

            // Animated brush.
            FromBrush(CreateAnimatedBrush()),

            // Null brush.
            FromBrush(null),
        };

        #endregion


        #region Private methods.

        /// <summary>Creates a sample animated brush.</summary>
        private static Brush CreateAnimatedBrush()
        {
            SolidColorBrush brush;
            ColorAnimation animation;

            animation = new ColorAnimation(Colors.White, Colors.Gray,
                Duration.Forever, FillBehavior.HoldEnd);
            animation.AutoReverse = true;

            brush = new SolidColorBrush();
            brush.BeginAnimation(SolidColorBrush.ColorProperty, animation);

            return brush;
        }

        /// <summary>Creates a sample Drawing instance.</summary>
        private static Drawing CreateDrawing()
        {
            Brush brush;
            Pen pen;
            Geometry geometry;

            brush = Brushes.Cyan;
            pen = new Pen(Brushes.Black, 1);
            geometry = new RectangleGeometry(new Rect(4, 4, 32, 32));

            return new GeometryDrawing(brush, pen, geometry);
        }

        /// <summary>Creates a sample stretched ImageBrush.</summary>
        /// <remarks>
        /// May return null if no image is available. For robustness,
        /// we may choose to create an image on the fly in this case.
        /// </remarks>
        private static Brush CreateStretchedImageBrush()
        {
            ImageBrush brush;
            BitmapSource bitmapSource;
            string path;

            // Return a null brush if the image is not available.
            if (!System.IO.File.Exists(ImageFileName))
            {
                return null;
            }

            path = System.IO.Path.Combine(System.Environment.CurrentDirectory, ImageFileName);
            bitmapSource = new BitmapImage(new Uri("file://" + path));
            brush = new ImageBrush(bitmapSource);
            brush.Stretch = Stretch.Fill;

            return brush;
        }

        /// <summary></summary>
        private static Brush CreateTiledVisualBrush()
        {
            VisualBrush brush;

            brush = new VisualBrush(new TextBox());
            brush.TileMode = TileMode.FlipX;
            brush.Stretch = Stretch.None;

            return brush;
        }

        /// <summary>Initializes a new BrushData instance.</summary>
        private static BrushData FromBrush(Brush brush)
        {
            return FromBrush(brush, false);
        }

        /// <summary>Initializes a new BrushData instance.</summary>
        private static BrushData FromBrush(Brush brush, bool isTransparent)
        {
            BrushData result;

            result = new BrushData();
            result._value = brush;
            result._isTransparent = isTransparent;

            return result;
        }

        #endregion


        #region Private fields.

        /// <summary>Brush encapsulated.</summary>
        private Brush _value;

        /// <summary>whether the brush is a transparent brush.</summary>
        private bool _isTransparent;

        /// <summary>Sample image file name to look for.</summary>
        private const string ImageFileName = "test.png";

        #endregion
    }
}
