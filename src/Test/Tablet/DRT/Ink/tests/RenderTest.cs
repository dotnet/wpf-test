// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows.Input;

namespace DRT
{
    [TestedSecurityLevelAttribute (SecurityLevel.PartialTrust)]
    public class RenderTest : DrtInkTestcase
    {

        /// <summary>
        /// The method to run the test cases.
        /// </summary>
        public override void Run()
        {
            // Do common initialization for both static and incremental rendering
            InitializeCommon();

            // Do initialization  work for static rendering only
            InitializeForStaticRender();
            TestStaticRendering();
            TestAttachIncrementalRendering();

            // Do initialization work for incremental rendering only
            InitializeForIncrementalRender();
            TestIncrementalRendering();

            InitializeForDrawStroke();
            TestDrawStroke();

            InitializeForDrawStroke();
            TestDrawWithOverrides();

            InitializeForDrawStroke();
            TestStrokeCollectionDraw();

            // done
            Success = true;
        }


        /// <summary>
        /// TestStaticRendering
        /// </summary>
        private void TestStaticRendering()
        {
            int numOfPoints = 30;
            int yPosition = 100;

            Random rnd = new Random();
            StrokeCollection strokes = new StrokeCollection();
            DrawingAttributes da = new DrawingAttributes();
            da.FitToCurve = false;
            //
            // Test 1:  a simple case
            //
            Point[] points1 = new Point[numOfPoints];
            for (int i = 0; i < numOfPoints; i++)
            {
                points1[i].X = i;
                points1[i].Y = yPosition;
            }
            Stroke s = new Stroke(points1);
            s.DrawingAttributes = da;
            strokes.Add(s);

            // Set the strokes in the static renderer.
            Debug.Assert(_srh != null && _srh.StaticRenderer != null);
            _srh.StaticRenderer.Strokes = strokes;

            // Verify static rendering
            RenderDrtParams drtParams1 = new RenderDrtParams(points1, _srh.StaticRenderer.RootVisual, "StaticRendering");
            _drt.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                                        new DispatcherOperationCallback(this.VerifyRenderLeaveContext),
                                        drtParams1);
            WaitUntilFinish();

            //
            // Test 2: random data points
            //
            yPosition = 150;
            Point[] points2 = new Point[numOfPoints];
            for (int i = 0; i < numOfPoints; i++)
            {
                points2[i].X = rnd.Next(yPosition - 10, yPosition + 10);
                points2[i].Y = yPosition+i;
            }
            Stroke s2 = new Stroke(points2);
            s2.DrawingAttributes = da;
            _srh.StaticRenderer.Strokes.Add(s2);
            Point[] rendered = _srh.StaticRenderer.Strokes[1].GetRenderingPoints();

            RenderDrtParams drtParams2 = new RenderDrtParams(points2, _srh.StaticRenderer.RootVisual, "Static Rendering");
            _drt.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                                        new DispatcherOperationCallback(this.VerifyRenderLeaveContext),
                                        drtParams2);
            WaitUntilFinish();

        }

        /// <summary>
        /// TestAttachIncrementalRendering
        /// </summary>
        private void TestAttachIncrementalRendering()
        {
            DrawingAttributes da = new DrawingAttributes();
            da.FitToCurve = false;

            // Create an incremental renderer
            StylusPacketValueMetric[] props = new StylusPacketValueMetric[2];
            props[0] = new StylusPacketValueMetric(StylusPacketValue.X, Int32.MinValue, Int32.MaxValue, StylusPacketValueUnit.Centimeters, (float)1000.000);
            props[1] = new StylusPacketValueMetric(StylusPacketValue.Y, Int32.MinValue, Int32.MaxValue, StylusPacketValueUnit.Centimeters, (float)1000.000);
            StylusPacketDescription spd = new StylusPacketDescription(props);
            IncrementalRenderer incRenderer = new IncrementalRenderer(da, spd);

            // Attach the incremental renderer's RootVisual to the static renderer.
            Debug.Assert(_srh != null && _srh.StaticRenderer != null);
            _srh.StaticRenderer.AttachIncrementalRendering(incRenderer.RootVisual, da);

            // Create testing packets and points
            int numOfPoints = 30;
            int yPosition = 200;
            Random rnd = new Random();
            double[] packets2 = new double[numOfPoints * 2];
            Point[] points2 = new Point[numOfPoints];
            for (int i = 0, j = 0; j < numOfPoints; j++)
            {
                int y = rnd.Next(yPosition - 10, yPosition + 10);
                packets2[i++] = j;
                packets2[i++] = y;
                points2[j].X = j;
                points2[j].Y = y;
            }

            // Render the packets
            incRenderer.RenderPackets(packets2);

            // Verify the rendering results
            RenderDrtParams drtParams = new RenderDrtParams(points2, _srh.StaticRenderer.RootVisual, "AttachIncrementalRendering");
            _drt.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                                        new DispatcherOperationCallback(this.VerifyRenderLeaveContext),
                                        drtParams);

            WaitUntilFinish();
            _srh.StaticRenderer.DetachIncrementalRendering(incRenderer.RootVisual);
        }

        /// <summary>
        /// TestDynamicRendering
        /// </summary>
        private void TestIncrementalRendering()
        {
            int numOfPoints = 30;
            int yPosition = 250;

            //
            // Test 1
            //

            // Create the test packets and points
            double[] packets1 = new double[numOfPoints * 2];
            Point[] points1 = new Point[numOfPoints];
            for (int i = 0, j = 0; j < numOfPoints; j++)
            {
                packets1[i++] = j;
                packets1[i++] = yPosition;
                points1[j].X = j;
                points1[j].Y = yPosition;
            }

            // Do the rendering
            _irh.IncRenderer.RenderPackets(packets1);

            // verify the rendering results
            RenderDrtParams drtParams1 = new RenderDrtParams(points1, _irh.IncRenderer.RootVisual, "IncrementalRendering");
            _drt.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                                        new DispatcherOperationCallback(this.VerifyRenderLeaveContext),
                                        drtParams1);
            WaitUntilFinish();

            //
            // Test 2
            //

            // Create the test packets and points
            Random rnd = new Random();
            double[] packets2 = new double[numOfPoints * 2];
            Point[] points2 = new Point[numOfPoints];
            yPosition = 300;
            for (int i = 0, j = 0; j < numOfPoints; j++)
            {
                int y = rnd.Next(yPosition - 10, yPosition + 10);
                packets2[i++] = j;
                packets2[i++] = y;
                points2[j].X = j;
                points2[j].Y = y;
            }

            _irh.IncRenderer.RenderPackets(packets2);

            RenderDrtParams drtParams2 = new RenderDrtParams(points2, _irh.IncRenderer.RootVisual, "IncrementalRendering");
            _drt.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                                        new DispatcherOperationCallback(this.VerifyRenderLeaveContext),
                                        drtParams2);
            WaitUntilFinish();
        }


        private void TestDrawStroke()
        {
            int numOfPoints = 30;
            int yPosition = 350;

            Random rnd = new Random();
            DrawingAttributes da = new DrawingAttributes();
            da.Color = Colors.Red;
            da.FitToCurve = false;
            //
            // Test 1:  a simple case
            //
            Point[] points1 = new Point[numOfPoints];
            for (int i = 0; i < numOfPoints; i++)
            {
                points1[i].X = i;
                points1[i].Y = yPosition;
            }
            Stroke stroke = new Stroke(points1);
            stroke.DrawingAttributes = da;
            DrawingVisual visual = new DrawingVisual();
            DrawingContext drawingContext = visual.RenderOpen();

            try
            {
                stroke.Draw(drawingContext);
            }
            finally
            {
                drawingContext.Close();
            }
            _drawHost.RootVisual.Children.Add(visual);

            // Verify static rendering
            RenderDrtParams drtParams1 = new RenderDrtParams(points1, _drawHost.RootVisual, "DrawStroke");
            _drt.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                                        new DispatcherOperationCallback(this.VerifyRenderLeaveContext),
                                        drtParams1);
            WaitUntilFinish();

            //
            // Test 2: random data points
            //
            yPosition = 400;
            Point[] points2 = new Point[numOfPoints];
            for (int i = 0; i < numOfPoints; i++)
            {
                points2[i].X = rnd.Next(yPosition - 10, yPosition + 10);
                points2[i].Y = yPosition + i;
            }
            stroke = new Stroke(points2);
            stroke.DrawingAttributes = da;
            DrawingVisual visual2 = new DrawingVisual();
            DrawingContext drawingContext2 = visual.RenderOpen();

            try
            {
                stroke.Draw(drawingContext2);
            }
            finally
            {
                drawingContext2.Close();
            }
            _drawHost.RootVisual.Children.Add(visual2);

            RenderDrtParams drtParams2 = new RenderDrtParams(points2, _drawHost.RootVisual, "DrawStroke");
            _drt.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                                        new DispatcherOperationCallback(this.VerifyRenderLeaveContext),
                                        drtParams2);
            WaitUntilFinish();
        }

        private void TestDrawWithOverrides()
        {
            int numOfPoints = 30;
            int yPosition = 350;

            Random rnd = new Random();
            DrawingAttributes da = new DrawingAttributes();
            da.Color = Colors.Red;
            da.FitToCurve = false;
            //
            // Test 1:  a simple case
            //
            Point[] points1 = new Point[numOfPoints];
            for (int i = 0; i < numOfPoints; i++)
            {
                points1[i].X = i;
                points1[i].Y = yPosition;
            }
            Stroke stroke = new Stroke(points1);
            stroke.DrawingAttributes = da;
            DrawingVisual visual = new DrawingVisual();
            DrawingContext context = visual.RenderOpen();

            Guid[] guids = new Guid[]{KnownIds.IsHighlighter, KnownIds.Color};
            Color color = stroke.DrawingAttributes.Color;
            Object[] values = new Object[]{color};

            bool caughtException = false;
            try{
                try
                {
                    stroke.Draw(context, guids, values);
                }
                catch ( ArgumentException )
                {
                    // We expect an ArgumentException being thrown.
                    caughtException = true;
                }
                if (caughtException == false )
                {
                    throw new InvalidOperationException("Stroke.Draw should throw exception when length of two overrides does not match");
                }

                caughtException = false;
                values = null;
                try
                {
                    stroke.Draw(context, guids, values);
                }
                catch ( ArgumentNullException )
                {
                    // We expect an ArgumentException being thrown.
                    caughtException = true;
                }
                if (caughtException == false )
                {
                    throw new InvalidOperationException("Stroke.Draw should throw exception when values array is null");
                }

                caughtException = false;
                guids = new Guid[]{Guid.Empty, KnownIds.Color};
                values = new Object[]{1, 2};
                try
                {
                    stroke.Draw(context, guids, values);
                }
                catch ( ArgumentException )
                {
                    // We expect an ArgumentException being thrown.
                    caughtException = true;
                }
                if (caughtException == false )
                {
                    throw new InvalidOperationException("Stroke.Draw should throw exception when a guid is empty");
                }

                caughtException = false;
                guids = new Guid[]{KnownIds.Color};
                values = new Object[]{false};
                try
                {
                    stroke.Draw(context, guids, values);
                }
                catch ( ArgumentException )
                {
                    // We expect an ArgumentException being thrown.
                    caughtException = true;
                }
                if (caughtException == false )
                {
                    throw new InvalidOperationException("Stroke.Draw should throw exception when the type in the values array does not match that in the guid array");
                }

                caughtException = false;
                guids = new Guid[]{KnownIds.StylusTip};
                values = new Object[]{3};
                try
                {
                    stroke.Draw(context, guids, values);
                }
                catch ( ArgumentException )
                {
                    // We expect an ArgumentException being thrown.
                    caughtException = true;
                }
                if (caughtException == false )
                {
                    throw new InvalidOperationException("Stroke.Draw should throw exception when the type in the values array does not match that in the guid array");
                }

                caughtException = false;
                guids = new Guid[]{KnownIds.StylusHeight};
                values = new Object[]{0d};
                try
                {
                    stroke.Draw(context, guids, values);
                }
                catch ( ArgumentOutOfRangeException )
                {
                    // We expect an ArgumentException being thrown.
                    caughtException = true;
                }
                if (caughtException == false )
                {
                    throw new InvalidOperationException("Stroke.Draw should throw exception when the stylus height if out of range");
                }


                caughtException = false;
                Matrix matrix = new Matrix(1, -1, 1, 1, 0, 0);
                guids = new Guid[]{KnownIds.StylusTipTransform};
                values = new Object[]{matrix};
                try
                {
                    stroke.Draw(context, guids, values);
                }
                catch ( ArgumentException )
                {
                    // We expect an ArgumentException being thrown.
                    caughtException = true;
                }
                if (caughtException == false )
                {
                    throw new InvalidOperationException("Stroke.Draw should throw exception when the stylusTipTransform does not represent a rotation only");
                }

                caughtException = false;
                guids = new Guid[]{KnownIds.X};
                values = new Object[]{matrix};
                try
                {
                    stroke.Draw(context, guids, values);
                }
                catch ( ArgumentException )
                {
                    // We expect an ArgumentException being thrown.
                    caughtException = true;
                }
                if (caughtException == false )
                {
                    throw new InvalidOperationException("Stroke.Draw should throw exception when the Guid type is not allowed");
                }

                color = Colors.Red;
                StylusTip stylusTip = StylusTip.Rectangle;
                double height = 10d;
                double width = 5d;
                Matrix tipTransform = Matrix.Identity;
                tipTransform.Rotate(60f);
                bool isOutlined = true;
                bool isHighlighter = false;
                guids = new Guid[]{KnownIds.Color, KnownIds.StylusTip, KnownIds.StylusHeight,
                                 KnownIds.StylusWidth, KnownIds.StylusTipTransform,KnownIds.IsHollow,
                                 KnownIds.IsHighlighter, KnownIds.NormalPressure};
                values = new Object[]{color, stylusTip, height, width, tipTransform, isOutlined, isHighlighter, false};

                stroke.Draw(context, guids, values);

            }
            finally
            {
                context.Close();
            }

            _drawHost.RootVisual.Children.Add(visual);

            // Verify static rendering
            RenderDrtParams drtParams1 = new RenderDrtParams(points1, _drawHost.RootVisual, "TestDrawWithOverrides");
            _drt.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                                        new DispatcherOperationCallback(this.VerifyRenderLeaveContext),
                                        drtParams1);
            WaitUntilFinish();


        }


        private void TestStrokeCollectionDraw()
        {
            //
            // Test 1: random data points
            //
            int yPosition = 400;
            int numOfPoints = 10;
            StrokeCollection strokes = new StrokeCollection();

            // Normal stroke
            Point[] points1 = new Point[numOfPoints];
            for (int i = 0; i < numOfPoints; i++)
            {
                points1[i].X = i;
                points1[i].Y = yPosition;
            }
            Stroke stroke = new Stroke(points1);
            strokes.Add(stroke);

            // Highlighter yellow
            Point[] points2 = new Point[numOfPoints];
            for (int i = 0; i < numOfPoints; i++)
            {
                points2[i].X = i+numOfPoints;
                points2[i].Y = yPosition;
            }
            Stroke stroke2 = new Stroke(points2);
            DrawingAttributes da = stroke2.DrawingAttributes;
            da.Color = Colors.Yellow;
            da.IsHighlighter = true;
            strokes.Add(stroke2);

            // Highlighter red
            Point[] points3 = new Point[numOfPoints];
            for (int i = 0; i < numOfPoints; i++)
            {
                points3[i].X = i+numOfPoints;
                points3[i].Y = yPosition+4;
            }
            Stroke stroke3 = new Stroke(points3);
            DrawingAttributes da3 = stroke3.DrawingAttributes;
            da3.Color = Colors.Red;
            da3.IsHighlighter = true;
            Color c3 = da3.Color;
            c3.A = 0;
            da3.Color = c3;
            strokes.Add(stroke3);

            // Highlighter yellow
            Point[] points4 = new Point[numOfPoints];
            for (int i = 0; i < numOfPoints; i++)
            {
                points4[i].X = i+numOfPoints;
                points4[i].Y = yPosition+8;
            }
            Stroke stroke4 = new Stroke(points4);
            DrawingAttributes da4 = stroke4.DrawingAttributes;
            da4.Color = Colors.Yellow;
            da4.IsHighlighter = true;
            Color c4 = da4.Color;
            c4.A = 100;
            da4.Color = c4;
            strokes.Add(stroke4);

            DrawingVisual visual = new DrawingVisual();
            DrawingContext drawingContext = visual.RenderOpen();

            try
            {
                strokes.Draw(drawingContext);
            }
            finally
            {
                drawingContext.Close();
            }
            _drawHost.RootVisual.Children.Add(visual);


            Point[] allPoints = new Point[numOfPoints*4];
            for (int i = 0; i < numOfPoints; i++)
            {
                allPoints[i] = points1[i];
            }
            for (int i = 0; i < numOfPoints; i++)
            {
                allPoints[i+numOfPoints] = points2[i];
            }
            for (int i = 0; i < numOfPoints; i++)
            {
                allPoints[i+numOfPoints*2] = points3[i];
            }
            for (int i = 0; i < numOfPoints; i++)
            {
                allPoints[i+numOfPoints*3] = points4[i];
            }

            RenderDrtParams drtParams = new RenderDrtParams(allPoints, _drawHost.RootVisual, "TestStrokeCollectionDraw");
            _drt.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                                        new DispatcherOperationCallback(this.VerifyStrokeCollectionLeaveContext),
                                        drtParams);
            WaitUntilFinish();
        }

        /// <summary>
        /// WaitUntilFinish
        /// </summary>
        private void WaitUntilFinish()
        {
            _fExit = false;
            while (!_fExit)
            {
                DispatcherFrame frame = new DispatcherFrame();
                DispatcherOperation op = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback(Yield), frame);
                Dispatcher.PushFrame(frame);
            }
        }

        /// <summary>
        /// Yield
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private object Yield(object o)
        {
            (o as DispatcherFrame).Continue = false;
            return null;
        }


        /// <summary>
        /// VerifyDefaultRenderingLeaveContext
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private object VerifyRenderLeaveContext(Object o)
        {
            //PointHitTestResult result = null;

            RenderDrtParams inParams = o as RenderDrtParams;

            Point[] points = inParams.Points;
            int count = points.Length;
            ContainerVisual cVisual = (ContainerVisual)(inParams.RootVisual);
            Rect bounds = cVisual.DescendantBounds;
            for (int i = 0; i < count; i++)
            {
                if (!bounds.Contains(points[i]))
                {
                    //NOTE: we've changed our StrokeVisuals to report no hit test.
                    throw new InvalidOperationException(
                            String.Format("Unexpected results in {0}", inParams.Msg));
                }
            }
            _fExit = true;
            return null;
        }

        private object VerifyStrokeCollectionLeaveContext(Object o)
        {
            RenderDrtParams inParams = o as RenderDrtParams;

            Point[] points = inParams.Points;
            int count = points.Length;
            ContainerVisual cVisual = (ContainerVisual)(inParams.RootVisual);
            Rect bounds = cVisual.DescendantBounds;
            VisualCollection vc = cVisual.Children;
            DRT.Assert(vc.Count == 1, "The visual collection should have one child only");
            DrawingVisual dv = (DrawingVisual) vc[0];
            DrawingGroup drawing = dv.Drawing;
            DRT.Assert(drawing.Opacity == 1.0);
            DRT.Assert(drawing.Children.Count == 3);

            DrawingCollection dc = drawing.Children;
            DRT.Assert(dc[0] is DrawingGroup);
            DRT.Assert(dc[1] is DrawingGroup);
            DRT.Assert(!(dc[2] is DrawingGroup));

            DrawingGroup dg1 = (DrawingGroup) dc[0];
            DrawingGroup dg2 = (DrawingGroup) dc[1];

            DRT.Assert(dg1.Opacity == 0.5);
            DRT.Assert(dg1.Children.Count == 2);
            DRT.Assert(dg2.Opacity == 0.5);
            DRT.Assert(dg2.Children.Count == 1);

            for (int i = 0; i < count; i++)
            {
                if (!bounds.Contains(points[i]))
                {
                    //NOTE: we've changed our StrokeVisuals to report no hit test.
                    throw new InvalidOperationException(
                             String.Format("Unexpected results in {0}", inParams.Msg));
                }
            }
            _fExit = true;
            return null;
        }

        /// <summary>
        /// Initialize common things for rendering test
        /// </summary>
        private void InitializeCommon()
        {
            _drt = (DrtInk)DrtInk.GetDrt();
            Debug.Assert(_drt != null);
            _drt.LoadXamlFile("DrtRendering.xaml");

            _canvas = _drt.FindVisualByID("canvas1") as Canvas;
        }

        /// <summary>
        /// InitializeForStaticRender
        /// </summary>
        private void InitializeForStaticRender()
        {
            Debug.Assert(_canvas != null);
            if (_canvas.Children.Count > 0)
                _canvas.Children.Clear();
            _srh = new StaticRenderHost();
            _canvas.Children.Add(_srh);
        }

        /// <summary>
        /// InitializeForIncrementalRender
        /// </summary>
        private void InitializeForIncrementalRender()
        {
            Debug.Assert(_canvas != null);
            if (_canvas.Children.Count > 0)
                _canvas.Children.Clear();
            _irh = new IncrementalRenderHost();
            _canvas.Children.Add(_irh);
        }

        private void InitializeForDrawStroke()
        {
            Debug.Assert(_canvas != null);
            if (_canvas.Children.Count > 0)
                _canvas.Children.Clear();
            _drawHost = new DrawHost();
            _canvas.Children.Add(_drawHost);
        }


        StaticRenderHost _srh;
        IncrementalRenderHost _irh;
        DrawHost _drawHost;
        DrtInk _drt;
        Canvas _canvas;
        bool _fExit;
    }

    /// <summary>
    /// Internal class to wrap parameters in one object for the asynchrounous method call.
    /// </summary>
    internal class RenderDrtParams
    {
        internal RenderDrtParams(Point[] points, Visual rootVisual, string msg)
        {
            _points = points;
            _rootVisual = rootVisual;
            _msg = msg;
        }

        internal string Msg
        {
            get { return _msg; }
        }

        internal Visual RootVisual
        {
            get { return _rootVisual; }
        }

        internal Point[] Points
        {
            get { return _points; }
        }
        Point[] _points;
        Visual _rootVisual;
        String _msg;
    }
}
