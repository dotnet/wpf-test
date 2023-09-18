// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// 3D Hit Testing using VisualTreeHelper.HitTest (i.e. from a 2D Point)
    /// </summary>
    public abstract class PointHitTestingTestBase : HitTestingTest
    {
        /// <summary/>
        protected PointHitTestingTestBase()
        {
            sanityResults = new List<RayModelHitInfo>();
        }

        /// <summary/>
        protected void HitTestAllModels()
        {
            VisualTreeHelper.HitTest(
                    parameters.Content,                             // The visual we're testing against
                    filter,                                         // A description of how we want to traverse the Model3D tree
                    new HitTestResultCallback(ProcessResults),    // A function to process the results
                    new PointHitTestParameters(diPoint)           // The 2D point we're checking
                    );

            if (!variation.UseViewport3D)
            {
                parameters.Visual.HitTest(filter, ProcessSanityResults, new PointHitTestParameters(diPoint));
            }
        }

        /// <summary/>
        protected void HitTestClosestModel()
        {
            HitTestResult result = VisualTreeHelper.HitTest(parameters.Content, diPoint);

            if (result is RayHitTestResult)
            {
                theirAnswers.Add(RayModelHitInfo.Create((RayHitTestResult)result));

                if (!variation.UseViewport3D)
                {
                    HitTestResult sanity = parameters.Visual.HitTest(diPoint);

                    if (sanity is RayHitTestResult)
                    {
                        sanityResults.Add(RayModelHitInfo.Create((RayHitTestResult)sanity));
                    }
                    else
                    {
                        AddFailure("VisualTreeHelper.HitTest( Point ) and Viewport3DVisual.HitTest( Point ) returned different answers");
                    }
                }
            }
        }

        /// <summary/>
        protected void CompareHitTestResults()
        {
            VerifyHitTestResults();

            if (!variation.UseViewport3D)
            {
                Log("");
                Log("Performing sanity check for VisualTreeHelper.HitTest vs Viewport3DVisual.HitTest...");

                // Save myAnswers because it's the only one that has screen-space coords computed
                List<RayModelHitInfo> temp = myAnswers;
                myAnswers = sanityResults;
                VerifyHitTestResults();
                myAnswers = temp;
            }
        }
#if SSL
        protected override void HitTestScreenSpaceLines3D( string visualName, ScreenSpaceLines3D model, Matrix3D modelToWorld )
        {
            if ( !ddBounds.RenderBounds.Contains( ddPoint ) )
            {
                // This point is completely clipped.  Don't bother checking for hits with the lines.
                return;
            }

            // *NOTE: All verification code must happen in Device Dependent Units.

            Matrix3D worldToScreen = MatrixUtils.WorldToScreenMatrix( parameters.Camera, ddBounds.ViewportBounds );

            for ( int i = 1; i < model.Points.Count; i++ )
            {
                Point3D p1 = model.Points[ i-1 ];
                Point3D p2 = model.Points[ i ];
                Point3D p1tx = MatrixUtils.Transform( modelToWorld, p1 );
                Point3D p2tx = MatrixUtils.Transform( modelToWorld, p2 );
                bool hitThisSegment = false;

                // "Render" the line and check the 2D point to see if it's inside the triangles
                ScreenSpaceLineTriangle t1 = new ScreenSpaceLineTriangle(
                        p1tx, p2tx, model.Thickness, model.Color, worldToScreen );
                ScreenSpaceLineTriangle t2 = new ScreenSpaceLineTriangle(
                        p2tx, p1tx, model.Thickness, model.Color, worldToScreen );

                Point3D intersection = new Point3D();
                if ( t1.Bounds.Contains( ddPoint ) && t1.Contains( ddPoint.X, ddPoint.Y ) )
                {
                    intersection = t1.Interpolator.GetVertex( ddPoint.X, ddPoint.Y ).Position;
                    hitThisSegment = true;
                }
                if ( t2.Bounds.Contains( ddPoint ) && t2.Contains( ddPoint.X, ddPoint.Y ) )
                {
                    intersection = t2.Interpolator.GetVertex( ddPoint.X, ddPoint.Y ).Position;
                    hitThisSegment = true;
                }
                if ( hitThisSegment )
                {
                    // p1tx       intersection           p2tx
                    //  o-------------o-------------------o
                    //
                    //  o-------------> toIntersection
                    //  o---------------------------------> toEnd
                    //
                    Vector3D toIntersection = intersection - p1tx;
                    Vector3D toEnd = p2tx - p1tx;
                    Vector3D line = p2 - p1;

                    RayLinesHitInfo info = new RayLinesHitInfo();
                    info.modelHit = ObjectUtils.GetName( model );
                    info.visualHit = visualName;
                    info.baseVisualHit = visualName;
                    info.segmentIndex = i-1;
                    info.segmentLocation = MathEx.Length( toIntersection ) / MathEx.Length( toEnd );

                    // PointHit is in model space
                    info.pointHit =  p1 + ( line * info.segmentLocation );

                    Point3D worldPointHit = MatrixUtils.Transform( modelToWorld, info.pointHit );
                    info.distanceToRayOrigin = MathEx.Length( pickRay.Origin - worldPointHit );
                    info.screenPointHit = ddPoint;

                    myAnswer.Add( info );
                }
            }
        }
#endif
        /// <summary/>
        protected Ray PointToRay()
        {
            if (!ddBounds.RenderBounds.Contains(ddPoint))
            {
                // This point is completely clipped.  Don't bother checking for hits.
                return null;
            }

            // *NOTE: This is all happening on a left-hand coordinate system.
            // *NOTE: All verification code must happen in Device Dependent Units.

            // Transform a Ray starting at x,y,0 (screen space)
            //  and going into the screen (direction = 0,0,1)
            //  into a world Ray

            Matrix3D worldToScreen = MatrixUtils.WorldToScreenMatrix(parameters.Camera, ddBounds.ViewportBounds);
            Matrix3D screenToWorld = worldToScreen;
            screenToWorld.Invert();

            // The viewing volume goes from [0,1] in z and we enter from the front (0 is the near plane)
            Point4D o = screenToWorld.Transform(new Point4D(ddPoint.X, ddPoint.Y, 0, 1));

            // This second point is used to calculate the direction vector
            Point4D o2 = screenToWorld.Transform(new Point4D(ddPoint.X, ddPoint.Y, 1, 1));

            Point3D origin = new Point3D(o.X / o.W, o.Y / o.W, o.Z / o.W);
            Point3D end = new Point3D(o2.X / o2.W, o2.Y / o2.W, o2.Z / o2.W);

            return new Ray(origin, end - origin, RayOrigin.Visual2D);
        }

        private HitTestResultBehavior ProcessResults(HitTestResult result)
        {
            return ProcessResults(result, theirAnswers);
        }

        private HitTestResultBehavior ProcessSanityResults(HitTestResult result)
        {
            return ProcessResults(result, sanityResults);
        }

        private HitTestResultBehavior ProcessResults(HitTestResult result, List<RayModelHitInfo> list)
        {
            if (result is RayHitTestResult)
            {
                list.Add(RayModelHitInfo.Create((RayHitTestResult)result));
            }
            // else ignore all other hit test results since there may be other 2D visuals around...

            return HitTestResultBehavior.Continue;
        }

        /// <summary/>
        protected override bool OnlyVerifyClosest
        {
            get { return false; }
        }

        /// <summary/>
        protected List<RayModelHitInfo> sanityResults;
        /// <summary>Device Independent Point to HitTest</summary>
        protected Point diPoint;
        /// <summary>Device Dependent Point to HitTest</summary>
        protected Point ddPoint;
    }
}