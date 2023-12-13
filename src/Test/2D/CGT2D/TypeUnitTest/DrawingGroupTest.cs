// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Media.Imaging;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Graphics.Factories;

namespace Microsoft.Test.Graphics.UnitTests
{
    /// <summary/>
    public class DrawingGroupTest : CoreGraphicsTest
    {
        /// <summary/>
        public override void RunTheTest()
        {
            if (priority > 0)
            {
                RunTest2();
            }
            else
            {
                TestConstructor();


                TestChildren();
                TestClipGeometry();
                TestOpacity();
                TestTransform();
            }
        }

        private void TestConstructor()
        {
            Log("Testing Constructor DrawingGroup()...");

            DrawingGroup theirAnswer = new DrawingGroup();

            if (MathEx.NotEquals(theirAnswer.Opacity, 1) ||
                !ObjectUtils.Equals(theirAnswer.Transform, null) ||
                !ObjectUtils.Equals(theirAnswer.ClipGeometry, null) ||
                !ObjectUtils.Equals(theirAnswer.Children.Count, 0) ||
                failOnPurpose)
            {
                AddFailure("Constructor DrawingGroup() failed");
                Log("*** Expected: DrawingGroup.Opacity = {0}, DrawingGroup.Transform = {1}, DrawingGroup.ClipGeometry = {2}, DrawingGroup.Children.Count = {3}", 1, null, null, 0);
                Log("*** Actual: DrawingGroup.Opacity = {0}, DrawingGroup.Transform = {1}, DrawingGroup.ClipGeometry = {2}, DrawingGroup.Children.Count = {3}", theirAnswer.Opacity, theirAnswer.Transform, theirAnswer.ClipGeometry, theirAnswer.Children.Count);
            }
        }

        /*
        private void TestCopy()
        {
            Log( "Testing Copy()..." );

            TestCopyWith( new DrawingGroup() );
            TestCopyWith( Const2D.drawingGroup1 );
        }

        private void TestCopyWith( DrawingGroup drawingGroup )
        {
            Log( "Testing Copy()..." );

            DrawingGroup theirAnswer = drawingGroup.Clone();

            if( MathEx.NotEquals( theirAnswer.Opacity, drawingGroup.Opacity )              ||
                !ObjectUtils.Equals( theirAnswer.Transform, drawingGroup.Transform )       ||
                !ObjectUtils.Equals( theirAnswer.ClipGeometry, drawingGroup.ClipGeometry ) ||
                !ObjectUtils.Equals( theirAnswer.Children, drawingGroup.Children )         ||
                failOnPurpose )
            {
                AddFailure( "DrawingGroup.Clone() failed" );
                Log( "*** Expected: DrawingGroup.Opacity = {0}, DrawingGroup.Transform = {1}, DrawingGroup.ClipGeometry = {2}, DrawingGroup.Children = {3}", drawingGroup.Opacity, drawingGroup.Transform, drawingGroup.ClipGeometry, drawingGroup.Children );
                Log( "*** Actual: DrawingGroup.Opacity = {0}, DrawingGroup.Transform = {1}, DrawingGroup.ClipGeometry = {2}, DrawingGroup.Children = {3}", theirAnswer.Opacity, theirAnswer.Transform, theirAnswer.ClipGeometry, theirAnswer.Children );
            }
        }
        */

        private void TestChildren()
        {
            Log("Testing Children Property...");

            TestChildrenWith(null);
            TestChildrenWith(new DrawingCollection());
            TestChildrenWith(Const2D.drawingCollection1);
        }

        private void TestChildrenWith(DrawingCollection drawingCollection)
        {
            DrawingGroup theirAnswer = new DrawingGroup();

            theirAnswer.Children = drawingCollection;

            if (!ObjectUtils.Equals(theirAnswer.Children, drawingCollection) || failOnPurpose)
            {
                AddFailure("get/set Children Property failed");
                Log("*** Expected: DrawingGroup.Children = {0}", drawingCollection);
                Log("*** Actual: DrawingGroup.Children = {0}", theirAnswer.Children);
            }
        }

        private void TestClipGeometry()
        {
            Log("Testing ClipGeometry Property...");

            TestClipGeometryWith(null);
            TestClipGeometryWith(new LineGeometry(new Point(-1.5, -1.5), new Point(1.5, 1.5)));
            TestClipGeometryWith(new RectangleGeometry(new Rect(new Point(-1.5, -1.5), new Point(1.5, 1.5))));
        }

        private void TestClipGeometryWith(Geometry clipGeometry)
        {
            DrawingGroup theirAnswer = new DrawingGroup();

            theirAnswer.ClipGeometry = clipGeometry;

            if (!ObjectUtils.Equals(theirAnswer.ClipGeometry, clipGeometry) || failOnPurpose)
            {
                AddFailure("get/set ClipGeometry Property failed");
                Log("*** Expected: DrawingGroup.ClipGeometry = {0}", clipGeometry);
                Log("*** Actual: DrawingGroup.ClipGeoemtry = {0}", theirAnswer.ClipGeometry);
            }
        }

        private void TestOpacity()
        {
            Log("Testing Opacity Property...");

            TestOpacityWith(0);
            TestOpacityWith(1);
            TestOpacityWith(0.5);
            TestOpacityWith(-10.5);
            TestOpacityWith(10.5);
        }

        private void TestOpacityWith(double opacity)
        {
            DrawingGroup theirAnswer = new DrawingGroup();

            theirAnswer.Opacity = opacity;

            if (MathEx.NotEquals(theirAnswer.Opacity, opacity) || failOnPurpose)
            {
                AddFailure("get/set Opacity Property failed");
                Log("*** Expected: DrawingGroup.Opacity = {0}", opacity);
                Log("*** Actual: DrawingGroup.Opacity = {0}", theirAnswer.Opacity);
            }
        }

        private void TestTransform()
        {
            Log("Testing Transform property...");

            TestTransformWith(null);
            TestTransformWith(Transform.Identity);
            TestTransformWith(Const2D.translate10);
        }

        private void TestTransformWith(Transform transform)
        {
            DrawingGroup theirAnswer = new DrawingGroup();
            theirAnswer.Transform = transform;

            if (!ObjectUtils.Equals(theirAnswer.Transform, transform) || failOnPurpose)
            {
                AddFailure("get/set Transform Property failed");
                Log("*** Expected: DrawingGroup.Transform = {0}", transform);
                Log("*** Actual: DrawingGroup.Transform = {0}", theirAnswer.Transform);
            }
        }

        private void RunTest2()
        {
            TestAfterClose2();   // after DrawingContext.Close() is called, can't use the DrawingContext anymore
            TestAppend2();
            TestOpen2();
            TestPop2();
        }

        private void TestAfterClose2()
        {
            Log("P2 Testing AfterClose...");

            Try(DrawLineAfterClose, typeof(InvalidOperationException));
            Try(DrawAnimLineAfterClose, typeof(InvalidOperationException));
            Try(DrawRectangleAfterClose, typeof(InvalidOperationException));
            Try(DrawAnimRectangleAfterClose, typeof(InvalidOperationException));
            Try(DrawRoundedRectangleAfterClose, typeof(InvalidOperationException));
            Try(DrawAnimRoundedRectangleAfterClose, typeof(InvalidOperationException));
            Try(DrawEllipseAfterClose, typeof(InvalidOperationException));
            Try(DrawAnimEllipseAfterClose, typeof(InvalidOperationException));
            Try(DrawGeometryAfterClose, typeof(InvalidOperationException));
            Try(DrawImageAfterClose, typeof(InvalidOperationException));
            Try(DrawAnimImageAfterClose, typeof(InvalidOperationException));
            Try(DrawTextAfterClose, typeof(InvalidOperationException));
            Try(DrawDrawingAfterClose, typeof(InvalidOperationException));
            Try(DrawVideoAfterClose, typeof(InvalidOperationException));
            Try(DrawAnimVideoAfterClose, typeof(InvalidOperationException));
            Try(PushOpacityAfterClose, typeof(InvalidOperationException));
            Try(PushAnimOpacityAfterClose, typeof(InvalidOperationException));
            Try(PushClipAfterClose, typeof(InvalidOperationException));
            Try(PushTransformAfterClose, typeof(InvalidOperationException));
            Try(PopAfterClose, typeof(InvalidOperationException));
            Try(CloseAfterClose, typeof(InvalidOperationException));
        }

        #region ExceptionThrowers for AfterClose

        private void DrawLineAfterClose()
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            DrawingContext drawingContext = drawingGroup.Append();
            drawingContext.Close();
            drawingContext.DrawLine(new Pen(Brushes.Red, 5), new Point(0, 0), new Point(10, 10));
        }

        private void DrawAnimLineAfterClose()
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            DrawingContext drawingContext = drawingGroup.Append();
            drawingContext.Close();
            drawingContext.DrawLine(new Pen(Brushes.Red, 5), new Point(10, 10), Const2D.pointAnimationClock, new Point(50, 50), Const2D.pointAnimationClock);
        }

        private void DrawRectangleAfterClose()
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            DrawingContext drawingContext = drawingGroup.Append();
            drawingContext.Close();
            drawingContext.DrawRectangle(Brushes.Red, new Pen(Brushes.Red, 5), new Rect(0, 0, 10, 10));
        }

        private void DrawAnimRectangleAfterClose()
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            DrawingContext drawingContext = drawingGroup.Append();
            drawingContext.Close();
            drawingContext.DrawRectangle(Brushes.Red, new Pen(Brushes.Red, 5), new Rect(10, 10, 10, 10), Const2D.rectAnimationClock);
        }

        private void DrawRoundedRectangleAfterClose()
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            DrawingContext drawingContext = drawingGroup.Append();
            drawingContext.Close();
            drawingContext.DrawRoundedRectangle(Brushes.Red, new Pen(Brushes.Red, 5), new Rect(0, 0, 10, 10), 5, 5);
        }

        private void DrawAnimRoundedRectangleAfterClose()
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            DrawingContext drawingContext = drawingGroup.Append();
            drawingContext.Close();
            drawingContext.DrawRoundedRectangle(Brushes.Red, new Pen(Brushes.Red, 5), new Rect(10, 10, 10, 10), Const2D.rectAnimationClock, 5, Const2D.doubleAnimationClock, 6, Const2D.doubleAnimationClock);
        }

        private void DrawEllipseAfterClose()
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            DrawingContext drawingContext = drawingGroup.Append();
            drawingContext.Close();
            drawingContext.DrawEllipse(Brushes.Red, new Pen(Brushes.Red, 5), new Point(10, 10), 5, 5);
        }

        private void DrawAnimEllipseAfterClose()
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            DrawingContext drawingContext = drawingGroup.Append();
            drawingContext.Close();
            drawingContext.DrawEllipse(Brushes.Red, new Pen(Brushes.Blue, 5), new Point(10, 10), Const2D.pointAnimationClock, 5, Const2D.doubleAnimationClock, 7, Const2D.doubleAnimationClock);
        }

        private void DrawGeometryAfterClose()
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            DrawingContext drawingContext = drawingGroup.Append();
            drawingContext.Close();
            drawingContext.DrawGeometry(Brushes.Red, new Pen(Brushes.Blue, 5), new RectangleGeometry(new Rect(0, 0, 10, 10)));
        }

        private void DrawImageAfterClose()
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            DrawingContext drawingContext = drawingGroup.Append();
            drawingContext.Close();
            drawingContext.DrawImage(new BitmapImage(new Uri("wood.bmp", UriKind.RelativeOrAbsolute)), new Rect(0, 0, 100, 100));
        }

        private void DrawAnimImageAfterClose()
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            DrawingContext drawingContext = drawingGroup.Append();
            drawingContext.Close();
            drawingContext.DrawImage(new BitmapImage(new Uri("wood.bmp", UriKind.RelativeOrAbsolute)), new Rect(9, 4, 59, 35), Const2D.rectAnimationClock);
        }

        private void DrawTextAfterClose()
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            DrawingContext drawingContext = drawingGroup.Append();
            drawingContext.Close();
            drawingContext.DrawText(Const2D.formattedText1, new Point(1.5, -1.5));
        }

        private void DrawDrawingAfterClose()
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            DrawingContext drawingContext = drawingGroup.Append();
            drawingContext.Close();
            drawingContext.DrawDrawing(Const2D.geometryDrawing1);
        }

        private void DrawVideoAfterClose()
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            DrawingContext drawingContext = drawingGroup.Append();
            drawingContext.Close();
            drawingContext.DrawVideo(Const2D.MediaPlayer, new Rect(1.5, -1.5, 1.5, 1.5));
        }

        private void DrawAnimVideoAfterClose()
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            DrawingContext drawingContext = drawingGroup.Append();
            drawingContext.Close();
            drawingContext.DrawVideo(Const2D.MediaPlayer, new Rect(1.5, -1.5, 1.5, 1.5), Const2D.rectAnimationClock);
        }

        private void PushOpacityAfterClose()
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            DrawingContext drawingContext = drawingGroup.Append();
            drawingContext.Close();
            drawingContext.PushOpacity(0.5);
        }

        private void PushAnimOpacityAfterClose()
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            DrawingContext drawingContext = drawingGroup.Append();
            drawingContext.Close();
            drawingContext.PushOpacity(0.5, Const2D.doubleAnimationClock);
        }

        private void PushClipAfterClose()
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            DrawingContext drawingContext = drawingGroup.Append();
            drawingContext.Close();
            drawingContext.PushClip(new RectangleGeometry(new Rect(0, 0, 10, 10)));
        }

        private void PushTransformAfterClose()
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            DrawingContext drawingContext = drawingGroup.Append();
            drawingContext.Close();
            drawingContext.PushTransform(new TranslateTransform(10, 10));
        }

        private void PopAfterClose()
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            DrawingContext drawingContext = drawingGroup.Append();
            drawingContext.Close();
            drawingContext.Pop();
        }

        private void CloseAfterClose()
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            DrawingContext drawingContext = drawingGroup.Append();
            drawingContext.Close();
            drawingContext.Close();
        }

        #endregion

        private void TestAppend2()
        {
            Log("P2 Testing Append()...");

            Try(AppendToNullCollection, typeof(InvalidOperationException));
            Try(AppendToFrozenCollection, typeof(InvalidOperationException));
            Try(AppendToOpenedCollection, typeof(InvalidOperationException));
        }

        #region ExceptionThrowers for Append

        private void AppendToNullCollection()
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            drawingGroup.Children = null;
            DrawingContext drawingContext = drawingGroup.Append();
            drawingContext.Close();
        }

        private void AppendToFrozenCollection()
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            drawingGroup.Children.Freeze();
            DrawingContext drawingContext = drawingGroup.Append();
            drawingContext.Close();
        }

        private void AppendToOpenedCollection()
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            drawingGroup.Open();
            drawingGroup.Append();
        }

        #endregion

        private void TestOpen2()
        {
            Log("P2 Testing Open()...");

            Try(OpenOnOpenedCollection, typeof(InvalidOperationException));
        }

        #region ExceptionThrowers for Open

        private void OpenOnOpenedCollection()
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            drawingGroup.Open();
            drawingGroup.Open();
        }

        #endregion

        private void TestPop2()
        {
            Log("P2 Testing Pop()...");

            Try(PopWithoutPush, typeof(InvalidOperationException));
            Try(PopMoreThanPush, typeof(InvalidOperationException));
        }

        #region ExceptionThrowers for Pop

        private void PopWithoutPush()
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            DrawingContext drawingContext = drawingGroup.Open();
            drawingContext.Pop();
        }

        private void PopMoreThanPush()
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            DrawingContext drawingContext = drawingGroup.Open();
            drawingContext.PushOpacity(1);
            drawingContext.PushOpacity(1);
            drawingContext.Pop();
            drawingContext.Pop();
            drawingContext.Pop();
        }

        #endregion
    }
}