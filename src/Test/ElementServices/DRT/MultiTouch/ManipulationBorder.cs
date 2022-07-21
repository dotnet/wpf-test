// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DRT
{
    public class ManipulationBorder : Control
    {
        static ManipulationBorder()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ManipulationBorder), new FrameworkPropertyMetadata(typeof(ManipulationBorder)));
        }

        public ManipulationModes ManipulationMode
        {
            get { return (ManipulationModes)GetValue(ManipulationModeProperty); }
            set { SetValue(ManipulationModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ManipulationMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ManipulationModeProperty =
            DependencyProperty.Register("ManipulationMode", typeof(ManipulationModes), typeof(ManipulationBorder), new UIPropertyMetadata(ManipulationModes.All));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _centerPoint = (Ellipse)GetTemplateChild("CenterPoint");
        }

        protected override void OnManipulationStarting(ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = GetParent(this);
            e.Mode = ManipulationMode;
        }

        protected override void OnManipulationStarted(ManipulationStartedEventArgs e)
        {
            _nonRestrictedMatrix = new MatrixTransform(EnsureMatrixTransform().Matrix);
            UpdateCenterPoint(e.ManipulationOrigin);
        }

        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            var origin = e.ManipulationOrigin;
            var manipulation = e.DeltaManipulation;

            Vector pastEndVector;
            if (UpdateManipulationRect(origin, manipulation.Translation, manipulation.Rotation, manipulation.Scale, out pastEndVector))
            {
                e.ReportBoundaryFeedback(new ManipulationDelta(pastEndVector, 0.0, new Vector(1.0, 1.0), new Vector()));
            }
        }

        protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e)
        {
        }

        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
#if false
            var origin = e.ManipulationOrigin;
            var manipulation = e.TotalManipulation;

            UpdateManipulationRect(origin, manipulation.Translation, manipulation.Rotation, manipulation.Scale);
#endif

        }

        private MatrixTransform EnsureMatrixTransform()
        {
            var group = RenderTransform as TransformGroup;
            if (group == null)
            {
                group = new TransformGroup();
                var matrixTransform = new MatrixTransform();
                group.Children.Add(matrixTransform);
                RenderTransform = group;
                return matrixTransform;
            }
            else
            {
                return (MatrixTransform)group.Children[0];
            }
        }

        private static UIElement GetParent(object element)
        {
            return (UIElement)VisualTreeHelper.GetParent((UIElement)element);
        }

        private bool UpdateManipulationRect(Point origin, Vector translation, double rotation, Vector scale)
        {
            Vector pastEndVector;
            return UpdateManipulationRect(origin, translation, rotation, scale, out pastEndVector);
        }

        private bool UpdateManipulationRect(Point origin, Vector translation, double rotation, Vector scale, out Vector pastEndVector)
        {
            var matrixTransform = EnsureMatrixTransform();
            var newMatrix = matrixTransform.Matrix;
            var nonRestrictedMatrix = _nonRestrictedMatrix.Matrix;

            newMatrix.Translate(translation.X, translation.Y);
            nonRestrictedMatrix.Translate(translation.X, translation.Y);

            //var originalCenterPt = new Point(ActualWidth * 0.5, ActualWidth * 0.5);
            //var centerPt = newMatrix.Transform(originalCenterPt);
            newMatrix.RotateAt(rotation, origin.X, origin.Y);
            nonRestrictedMatrix.RotateAt(rotation, origin.X, origin.Y);

            //centerPt = newMatrix.Transform(originalCenterPt);
            newMatrix.ScaleAt(scale.X, scale.Y, origin.X, origin.Y);
            nonRestrictedMatrix.ScaleAt(scale.X, scale.Y, origin.X, origin.Y);

            _nonRestrictedMatrix.Matrix = nonRestrictedMatrix;
            matrixTransform.Matrix = newMatrix;

            pastEndVector = new Vector();

            bool pastEnd = false;
            double offsetX = 0.0;
            double offsetY = 0.0;
            UIElement parent = GetParent(this);
            if (parent != null)
            {
                Rect bounds = matrixTransform.TransformBounds(new Rect(new Point(), RenderSize));
                if (bounds.Left < 0.0)
                {
                    newMatrix.OffsetX -= bounds.Left;
                }
                else if (bounds.Right > parent.RenderSize.Width)
                {
                    newMatrix.OffsetX -= bounds.Right - parent.RenderSize.Width;
                }
                if (bounds.Top < 0.0)
                {
                    newMatrix.OffsetY -= bounds.Top;
                }
                else if (bounds.Bottom > parent.RenderSize.Height)
                {
                    newMatrix.OffsetY -= bounds.Bottom - parent.RenderSize.Height;
                }

                bounds = _nonRestrictedMatrix.TransformBounds(new Rect(new Point(), RenderSize));
                if (bounds.Left < 0.0)
                {
                    pastEnd = true;
                    offsetX = bounds.Left;
                }
                else if (bounds.Right > parent.RenderSize.Width)
                {
                    pastEnd = true;
                    offsetX = bounds.Right - parent.RenderSize.Width;
                }
                if (bounds.Top < 0.0)
                {
                    pastEnd = true;
                    offsetY = bounds.Top;
                }
                else if (bounds.Bottom > parent.RenderSize.Height)
                {
                    pastEnd = true;
                    offsetY = bounds.Bottom - parent.RenderSize.Height;
                }

                if (pastEnd)
                {
                    pastEndVector = new Vector(offsetX, offsetY);
                }
            }

            matrixTransform.Matrix = newMatrix;
            UpdateCenterPoint(origin);

            return pastEnd;
        }

        private void UpdateCenterPoint(Point origin)
        {
            if (_centerPoint != null)
            {
                UIElement parent = GetParent(this);
                if (parent != null)
                {
                    GeneralTransform transform = parent.TransformToDescendant(this);
                    if (transform != null)
                    {
                        origin = transform.Transform(origin);
                    }
                }

                _centerPoint.Margin = new Thickness(origin.X - 4.0, origin.Y - 4.0, 0.0, 0.0);
            }
        }

        public void Reset()
        {
            RenderTransform = null;
        }

        public MatrixTransform CurrentMatrixTransform
        {
            get
            {
                var group = RenderTransform as TransformGroup;
                if (group != null)
                {
                    return (MatrixTransform)group.Children[0];
                }

                return null;
            }
        }

        public void SetMatrix(Matrix matrix)
        {
            EnsureMatrixTransform().Matrix = matrix;
        }

        private Ellipse _centerPoint;
        private MatrixTransform _nonRestrictedMatrix;
    }
}
