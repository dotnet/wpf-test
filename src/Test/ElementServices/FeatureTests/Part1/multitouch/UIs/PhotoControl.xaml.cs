// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Test.Input.MultiTouch;

namespace Microsoft.Test.Input.MultiTouch.Tests
{
    /// <summary>
    /// Interaction logic for PhotoControl.xaml
    /// </summary>
    public partial class PhotoControl : UserControl
    {
        Matrix _orgMatrix;

        public PhotoControl()
        {
            InitializeComponent();

            Manipulation.SetManipulationMode(this, ManipulationModes.All); 
            this.RenderTransform = new MatrixTransform();
        }

        public void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            e.Handled = true;
            _orgMatrix = ((MatrixTransform)this.RenderTransform).Matrix;
            BringToFront();
        }

        public void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            var manipulation = e.CumulativeManipulation; 
            this.RenderTransform = CalculateMatrixTransform(manipulation, _orgMatrix);
            e.Handled = true;
        }

        public void OnManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
        }

        public void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
        }

        #region Methods

        public Uri ImagePath
        {
            get
            {
                BitmapImage bitmap = Photo.Source as BitmapImage;

                if (bitmap == null)
                {
                    return new Uri("");
                }

                return bitmap.UriSource;
            }
            set
            {
                Photo.Source = new BitmapImage(value);
            }
        }
        
        /// <summary>
        /// bring the current photo to the front most
        /// </summary>
        private void BringToFront()
        {
            Canvas parent = LogicalTreeHelper.GetParent(this) as Canvas;
            if (parent != null)
            {
                int zIndex = Canvas.GetZIndex(this);
                foreach (UIElement child in parent.Children)
                {
                    int childIndex = Canvas.GetZIndex(child);
                    if (childIndex > zIndex)
                    {
                        Canvas.SetZIndex(child, childIndex - 1);
                    }
                }

                Canvas.SetZIndex(this, parent.Children.Count - 1);
            }
        }

        private MatrixTransform CalculateMatrixTransform(ManipulationDelta e, Matrix matrix)
        {
            matrix.Translate(e.Translation.X, e.Translation.Y);
            var orgCenter = new Point(this.RenderSize.Width * 0.5, this.RenderSize.Height * 0.5);
            var center = matrix.Transform(orgCenter);

            matrix.RotateAt(e.Rotation, center.X, center.Y);
            center = matrix.Transform(orgCenter);

            matrix.ScaleAt(e.Scale.X, e.Scale.Y, center.X, center.Y);
            return new MatrixTransform(matrix);
        }
        
        #endregion
    }
    
}
