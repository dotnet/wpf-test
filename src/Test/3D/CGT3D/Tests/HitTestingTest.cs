// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Graphics.ReferenceRender;
using Microsoft.Test.Graphics.Factories;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Base class for all 3D Hit Testing verification
    /// </summary>
    public abstract class HitTestingTest : RenderingTest
    {
        static HitTestingTest()
        {
            //     *****
            //       *
            //      ***
            //       *
            // *           *
            // * *       * *
            // ****  *  ****
            // * *       * *
            // *           *
            //       *
            //      ***
            //       *
            //     *****
            // This is the cursor we draw in the framebuffer for each hit intersection
            //  (it looks like garbage in ASCII, but great in a PNG!)

            s_hitCursor = new byte[,]
                {
                    {0,0,0,0,1,1,1,1,1,0,0,0,0},
                    {0,0,0,0,0,0,1,0,0,0,0,0,0},
                    {0,0,0,0,0,1,1,1,0,0,0,0,0},
                    {0,0,0,0,0,0,1,0,0,0,0,0,0},
                    {1,0,0,0,0,0,0,0,0,0,0,0,1},
                    {1,0,1,0,0,0,0,0,0,0,1,0,1},
                    {1,1,1,1,0,0,1,0,0,1,1,1,1},
                    {1,0,1,0,0,0,0,0,0,0,1,0,1},
                    {1,0,0,0,0,0,0,0,0,0,0,0,1},
                    {0,0,0,0,0,0,1,0,0,0,0,0,0},
                    {0,0,0,0,0,1,1,1,0,0,0,0,0},
                    {0,0,0,0,0,0,1,0,0,0,0,0,0},
                    {0,0,0,0,1,1,1,1,1,0,0,0,0},
                };
        }

        /// <summary/>
        public override void Init(Variation v)
        {
            base.Init(v);
            parameters = new SceneTestObjects(v);
            string tol = v["NumericalPrecisionTolerance"];
            MathEx.NumericalPrecisionTolerance = (tol == null)
                                                    ? MathEx.DefaultNumericalPrecisionTolerance
                                                    : StringConverter.ToDouble(tol);

            filter = FactoryParser.MakeFilter(v);
            myAnswers = new List<RayModelHitInfo>();
            theirAnswers = new List<RayModelHitInfo>();

            bool clipToBounds = v["ClipToBounds"] == null ? false : StringConverter.ToBool(v["ClipToBounds"]);

            ddBounds = new Bounds(v.WindowSize, ViewportRect, clipToBounds);
            ddBounds.ConvertToAbsolutePixels();

            // Give unique identifiers to all the objects I care to hit test
            ObjectUtils.NameObjects(parameters.Visual3Ds);
        }

        /// <summary/>
        public override Visual GetWindowContent()
        {
            return parameters.Content;
        }

        /// <summary/>
        public override void Verify()
        {
            // We have the visual, we can be assured that it has rendered by now.
            // This must be done or else Point Hit Testing won't work correctly.

            VerifyHitTesting();

            if (Failures != 0 || variation["ForceSave"] == "true")
            {
                LogFailedHitTestImage();
            }
        }

        #region Log failed hit tests into the screen capture

        private void LogFailedHitTestImage()
        {
            try
            {
                SceneRenderer renderer = parameters.SceneRenderer;
                RenderBuffer expected = renderer.Render();
                RenderBuffer report = expected.Clone();

                // We're going to draw everything at z=0.  Set this parameter so that RenderBuffer doesn't freak out.
                double oldTolerance = RenderTolerance.NearPlaneTolerance;
                RenderTolerance.NearPlaneTolerance = 0;
                report.DepthTestFunction = DepthTestFunction.LessThanOrEqualTo;

                foreach (RayModelHitInfo info in myAnswers)
                {
                    if (info.HasValidScreenPointHit)
                    {
                        AddHitToFrameBuffer(report, info.screenPointHit);
                    }
                }
                RenderTolerance.NearPlaneTolerance = oldTolerance;

                Log("");
                Log("Saving failed images as " + logPrefix + "_*.png");
                PhotoConverter.SaveImageAs(expected.FrameBuffer, logPrefix + "_Rendered.png");
                PhotoConverter.SaveImageAs(expected.ToleranceBuffer, logPrefix + "_Expected_tb.png", false);
                PhotoConverter.SaveImageAs(report.FrameBuffer, logPrefix + "_Expected_fb.png");
                PhotoConverter.SaveImageAs(report.ToleranceBuffer, logPrefix + "_Diff_tb.png", false);
            }
            catch (InvalidOperationException)
            {
                // We can't handle scenes with non-invertible transforms yet.
                Log("");
                Log("Log output is not currently possible. Please re-run this variation under a debugger.");
            }
        }



        private void AddHitToFrameBuffer(RenderBuffer buffer, Point point)
        {
            // "point" is the pixel in the center of the cursor.
            double x = Math.Floor(point.X);
            double y = Math.Floor(point.Y);
            int cursorWidth = s_hitCursor.GetLength(0);
            int cursorHeight = s_hitCursor.GetLength(1);

            x -= cursorWidth / 2;
            y -= cursorHeight / 2;

            for (int j = 0; j < cursorHeight; j++)
            {
                for (int i = 0; i < cursorWidth; i++)
                {
                    if (s_hitCursor[i, j] != 0)
                    {
                        DrawCrossHairsPoint(buffer, (int)x + i, (int)y + j);
                    }
                }
            }
        }

        private void DrawCrossHairsPoint(RenderBuffer buffer, int x, int y)
        {
            // Avoid IndexOutOfRange exceptions with RenderBuffer
            if (0 <= x && x < DpiScaledWindowWidth && 0 <= y && y < DpiScaledWindowHeight)
            {
                // Draw the crosshairs into both the frame buffer and the tolerance buffer
                //      Premultiplied 50% transparent red:
                Color hitColor = ColorOperations.ColorFromArgb(0.5, 0.5, 0.0, 0.0);
                Color tolerance = Colors.White;

                buffer.FrameBuffer[x, y] = ColorOperations.PreMultipliedAlphaBlend(hitColor, buffer.FrameBuffer[x, y]);
                buffer.ToleranceBuffer[x, y] = tolerance;
            }
        }

        #endregion

        /// <summary/>
        protected abstract void VerifyHitTesting();

        /// <summary/>
        protected void DoRayHitTesting(Ray ray)
        {
            if (ray == null)
            {
                // If the ray is null, it means that the point used to create it was outisde the render bounds.
                return;
            }

            pickRay = ray;

            if (pickRay.RayOrigin == RayOrigin.Visual3D)
            {
                foreach (Visual3D v in parameters.Visual3Ds)
                {
                    _keepGoing = true;
                    HitTest(v);
                }
            }
            else
            {
                _keepGoing = true;
                HitTest(parameters.Visual3Ds, Const.mIdent);
            }
        }

        private void HitTest(Visual3D visual)
        {
            if (pickRay.RayOrigin != RayOrigin.Visual3D)
            {
                throw new ApplicationException("HitTest( Visual3D ) must only be invoked during a Ray Hit Test");
            }

            // The reason it is so important to distinguish between Ray hit tests and Point hit tests
            //  is because the ray hit test ignores the transform on the Visual3D it starts with.
            //
            // This is consistent with VisualTreeHelper.GetContentBounds( Visual3D ) which also ignores
            //  the transform of the Visual3D passed in.

            if (visual is ModelVisual3D)
            {
                ModelVisual3D modelVisual = (ModelVisual3D)visual;
                HitTest(ObjectUtils.GetName(visual), modelVisual.Content, Const.mIdent, Const.mIdent);
                HitTest(modelVisual.Children, Const.mIdent);
            }
        }

        private void HitTest(IEnumerable<Visual3D> visuals, Matrix3D parentTransform)
        {
            foreach (Visual3D visual3D in visuals)
            {
                if (visual3D is ModelVisual3D)
                {
                    ModelVisual3D modelVisual = (ModelVisual3D)visual3D;
                    Matrix3D currentTransform = MatrixUtils.Value(modelVisual.Transform);
                    Matrix3D childTransform = MatrixUtils.Multiply(currentTransform, parentTransform);
                    HitTest(ObjectUtils.GetName(visual3D),
                             modelVisual.Content,
                             childTransform,
                             Const.mIdent);
                    HitTest(modelVisual.Children, childTransform);
                }
            }
        }

        private void HitTest(string visualName, Model3D model,
                                         Matrix3D visualToWorld, Matrix3D modelToVisual)
        {
            if (model == null)
            {
                return;
            }

            bool skipSelf = false;
            bool skipChildren = false;

            switch ((string)model.GetValue(Const.SkipProperty))
            {
                case "SkipSelf":
                    skipSelf = true;
                    break;

                case "SkipChildren":
                    skipChildren = true;
                    break;

                case "SkipSelfAndChildren":
                    skipSelf = true;
                    skipChildren = true;
                    break;

                case "Stop":
                    _keepGoing = false;
                    break;
            }

            if (!_keepGoing)
            {
                // We don't want to return these results anyway, so let's pretend they don't exist
                return;
            }

            Matrix3D newModelToVisual = MatrixUtils.Multiply(MatrixUtils.Value(model.Transform), modelToVisual);

            if (model is Model3DGroup)
            {
                Model3DGroup group = model as Model3DGroup;

                if (!skipChildren && group.Children != null)
                {
                    for (int i = 0; i < group.Children.Count && _keepGoing; i++)
                    {
                        HitTest(visualName, group.Children[i], visualToWorld, newModelToVisual);
                    }
                }
            }
            else if (model is GeometryModel3D && !skipSelf)
            {
                GeometryModel3D gm = (GeometryModel3D)model;
                VisibleFaces visibleFaces = VisibleFaces.None;

                if (gm.Material != null)
                {
                    visibleFaces |= VisibleFaces.Front;
                }
                if (gm.BackMaterial != null)
                {
                    visibleFaces |= VisibleFaces.Back;
                }

                if (visibleFaces != VisibleFaces.None)
                {
                    HitTestGeometry3D(
                            visualName,
                            ObjectUtils.GetName(model),
                            gm.Geometry,
                            visibleFaces,
                            visualToWorld,
                            newModelToVisual);
                }
                // else do nothing because "hit" is already false.
            }
#if SSL
            else if ( model is ScreenSpaceLines3D && !skipSelf )
            {
                HitTestScreenSpaceLines3D( visualName, ((ScreenSpaceLines3D)model), newTransform );
            }
#endif
            else if (model is Light)
            {
                // Do nothing because "hit" is already false.
            }
            else
            {
                throw new ApplicationException("Unexpected Model type in the group");
            }
        }

        private void HitTestGeometry3D(string visualName, string modelName, Geometry3D geometry, VisibleFaces visibleFaces, Matrix3D visualToWorld, Matrix3D modelToVisual)
        {
            Matrix3D modelToWorld = MatrixUtils.Multiply(modelToVisual, visualToWorld);

            MeshGeometry3D mesh = geometry as MeshGeometry3D;

            if (mesh == null)
            {
                throw new NotSupportedException("MeshGeometry3D is the only GeometryModel supported for hit testing verification");
            }

            int positionCount = (mesh.Positions == null) ? 0 : mesh.Positions.Count;
            bool isIndexedMesh = mesh.TriangleIndices != null && mesh.TriangleIndices.Count != 0;
            int indexCount = (isIndexedMesh) ? mesh.TriangleIndices.Count : positionCount;
            bool reverseWinding = MatrixUtils.Determinant(modelToWorld) < 0;

            for (int i = 2; i < indexCount; i += 3)
            {
                int i1, i2, i3;

                if (isIndexedMesh)
                {
                    i1 = mesh.TriangleIndices[i - 2];
                    i2 = mesh.TriangleIndices[i - 1];
                    i3 = mesh.TriangleIndices[i];
                }
                else
                {
                    i1 = i - 2;
                    i2 = i - 1;
                    i3 = i;
                }
                if (reverseWinding)
                {
                    int temp = i2;
                    i2 = i3;
                    i3 = temp;
                }

                if (i1 < 0 || positionCount <= i1 ||
                     i2 < 0 || positionCount <= i2 ||
                     i3 < 0 || positionCount <= i3)
                {
                    // A bad triangle signifies the end of a rendered mesh. Stop hit testing.
                    break;
                }

                // The pick ray is in world space so we need to transform the geometry to match it.
                Point3D p1 = MatrixUtils.Transform(mesh.Positions[i1], modelToWorld);
                Point3D p2 = MatrixUtils.Transform(mesh.Positions[i2], modelToWorld);
                Point3D p3 = MatrixUtils.Transform(mesh.Positions[i3], modelToWorld);

                RayMeshHitInfo hitInfo = new RayMeshHitInfo();

                // It's impossible to hit both sides of a triangle with the same ray,
                //  so nothing special needs to happen here to account for visible faces.
                if (pickRay.DoesIntersect(p1, p2, p3, visibleFaces, hitInfo))
                {
                    // The ray intersection code can't find the intersection in model space
                    //  because it is working with a transformed triangle.
                    // We will compute the intersection in model space now.
                    hitInfo.modelPointHit = MathEx.WeightedSum(
                                                   mesh.Positions[i1],
                                                   hitInfo.vertexWeight1,
                                                   mesh.Positions[i2],
                                                   hitInfo.vertexWeight2,
                                                   mesh.Positions[i3],
                                                   hitInfo.vertexWeight3);

                    // Now we transform the intersection to Visual space
                    hitInfo.visualPointHit = MatrixUtils.Transform(hitInfo.modelPointHit, modelToVisual);

                    // Only add this intersection if it's not clipped by the camera
                    // (we're treating world space and screen space as the same here)
                    Point screenPointHit;
                    if (IsClipped(MatrixUtils.Transform(hitInfo.modelPointHit, modelToWorld), out screenPointHit))
                    {
                        continue;
                    }

                    hitInfo.screenPointHit = screenPointHit;
                    hitInfo.meshHit = ObjectUtils.GetName(mesh);
                    hitInfo.modelHit = modelName;
                    hitInfo.visualHit = visualName;
                    hitInfo.baseVisualHit = visualName;
                    hitInfo.vertexIndex1 = i1;
                    if (reverseWinding)
                    {
                        // Everything was done with a reversed triangle... Switch indices and weights back to normal.
                        hitInfo.vertexIndex2 = i3;
                        hitInfo.vertexIndex3 = i2;
                        double temp = hitInfo.vertexWeight2;
                        hitInfo.vertexWeight2 = hitInfo.vertexWeight3;
                        hitInfo.vertexWeight3 = temp;
                    }
                    else
                    {
                        hitInfo.vertexIndex2 = i2;
                        hitInfo.vertexIndex3 = i3;
                    }

                    myAnswers.Add(hitInfo);
                }
            }
        }

        private bool IsClipped(Point3D intersection, out Point screenPointHit)
        {
            // *NOTE: All verification code must happen in Device Dependent Units.

            Matrix3D worldToScreen = MatrixUtils.WorldToScreenMatrix(parameters.Camera, ddBounds.ViewportBounds);

            // It's possible that the matrix represents a perspective transform.
            // This means that we need to perform the transform in 4-space and
            //  then convert back to 2-space before we have the correct point.
            Point4D point = (Point4D)intersection;
            point = MatrixUtils.Transform(point, worldToScreen);

            // Clipping planes only exist on 2D visuals
            if (pickRay.RayOrigin == RayOrigin.Visual2D)
            {
                double z = point.Z / point.W;

                // If z is outside of the [0,1] cannonical viewing volume,
                //  then the hit point is clipped and should be ignored.
                if (z < -Const.eps || 1 + Const.eps < z)
                {
                    screenPointHit = new Point(Const.nan, Const.nan);
                    return true;
                }
            }

            screenPointHit = new Point(point.X / point.W, point.Y / point.W);
            return false;
        }
#if SSL
        protected virtual void  HitTestScreenSpaceLines3D( string visualName, ScreenSpaceLines3D model, Matrix3D modelToWorld )
        {
            // Can't hit a screen space line if we're just doing 3D Ray hit testing
            // This is overridden in PointHitTestingTestBase because it starts from 2D.
        }
#endif
        #region Verify the results

        /// <summary/>
        protected void VerifyHitTestResults()
        {
            if (MathEx.NumericalPrecisionTolerance == 1.0)
            {
                // We completely ignore the results of hit testing for meshes
                //  with Non-Affine transforms
                Log("Hit Test results ignored");
                return;
            }

            if (myAnswers.Count > 0)
            {
                Log("This is what I got:");
                foreach (RayModelHitInfo info in myAnswers)
                {
                    Log(" - {0} - {1} @ {2}", info.visualHit, ModelIdentifier(info), info.visualPointHit);
                }
            }
            else
            {
                Log("Nothing was hit");
            }

            if (!OnlyVerifyClosest && theirAnswers.Count != myAnswers.Count)
            {
                AddFailure("Avalon and I did not hit the same number of models");
                Log("*** Expected: {0} models hit", myAnswers.Count);
                Log("*** Actual:   {0} models hit", theirAnswers.Count);
            }

            double lastDistance = double.NegativeInfinity;

            for (int i = 0; i < theirAnswers.Count; i++)
            {
                RayModelHitInfo current = FindMyMatchingAnswer(theirAnswers[i]);
                if (current == null)
                {
                    // This answer was not found in my answer list.
                    continue;
                }

                if ((theirAnswers[i].distanceToRayOrigin < lastDistance &&
                       MathEx.NotCloseEnough(theirAnswers[i].distanceToRayOrigin, lastDistance)) ||
                     failOnPurpose)
                {
                    AddFailure("Hit testing results should always be sorted");
                    Log("*** Current Distance: {0}", theirAnswers[i].distanceToRayOrigin);
                    Log("***    Last Distance: {0}", lastDistance);
                }

                lastDistance = theirAnswers[i].distanceToRayOrigin;

                if (MathEx.NotCloseEnough(current.distanceToRayOrigin, theirAnswers[i].distanceToRayOrigin) ||
                     failOnPurpose)
                {
                    AddFailure("ModelIntersection.DistanceToRayOrigin is incorrect for {0}", ModelIdentifier(current));
                    Log("*** Expected: {0}", current.distanceToRayOrigin);
                    Log("***   Actual: {0}", theirAnswers[i].distanceToRayOrigin);
                }
                if (MathEx.NotCloseEnough(current.visualPointHit, theirAnswers[i].visualPointHit) ||
                     failOnPurpose)
                {
                    AddFailure("ModelIntersection.PointHit returned the wrong value for {0}", ModelIdentifier(current));
                    Log("*** Expected: {0}", current.visualPointHit);
                    Log("***   Actual: {0}", theirAnswers[i].visualPointHit);
                }

                // We already know that modelHit is the same
                //  otherwise we wouldn't be comparing these right now.
                //  (see method: "FindMyMatchingAnswer")

                if (current.visualHit != theirAnswers[i].visualHit || failOnPurpose)
                {
                    AddFailure("ModelIntersection.VisualHit returned the wrong value for {0}", ModelIdentifier(current));
                    Log("*** Expected: {0}", current.visualHit);
                    Log("***   Actual: {0}", theirAnswers[i].visualHit);
                }

                if (current.baseVisualHit != theirAnswers[i].baseVisualHit || failOnPurpose)
                {
                    AddFailure("ModelIntersection.BaseVisualHit returned the wrong value for {0}", ModelIdentifier(current));
                    Log("*** Expected: {0}", current.baseVisualHit);
                    Log("***   Actual: {0}", theirAnswers[i].baseVisualHit);
                }

                if (theirAnswers[i] is RayMeshHitInfo)
                {
                    VerifyMeshIntersection((RayMeshHitInfo)theirAnswers[i], (RayMeshHitInfo)current);
                }
#if SSL
                else if ( theirAnswer[ i ] is RayLinesHitInfo )
                {
                    VerifyLinesIntersection( (RayLinesHitInfo)theirAnswer[ i ], (RayLinesHitInfo)current );
                }
#endif
                current.alreadyVerified = true;
            }

            if (!OnlyVerifyClosest)
            {
                foreach (RayModelHitInfo info in myAnswers)
                {
                    if (!info.alreadyVerified)
                    {
                        AddFailure("I hit something that Avalon did not!");
                        Log("*** Visual hit: {0}", info.visualHit);
                        Log("***  Model hit: {0}", ModelIdentifier(info));
                        Log("***  Point hit: {0}", info.visualPointHit);
                    }
                }
            }
        }



        private RayModelHitInfo FindMyMatchingAnswer(RayModelHitInfo nextHitInfo)
        {
            for (int i = 0; i < myAnswers.Count; i++)
            {
                if (myAnswers[i].alreadyVerified)
                {
                    continue;
                }
                if (nextHitInfo.modelHit == myAnswers[i].modelHit)
                {
                    if (nextHitInfo is RayMeshHitInfo)
                    {
                        // It's possible to hit a single mesh more than once
                        //  so the name comparison isn't good enough.
                        // But we do know that these casts are safe since model names are unique.
                        RayMeshHitInfo info1 = (RayMeshHitInfo)nextHitInfo;
                        RayMeshHitInfo info2 = (RayMeshHitInfo)myAnswers[i];
                        if (info1.vertexIndex1 != info2.vertexIndex1 ||
                             info1.vertexIndex2 != info2.vertexIndex2 ||
                             info1.vertexIndex3 != info2.vertexIndex3)
                        {
                            continue;
                        }
                        // It's not possible to hit the same triangle more than once.
                        // We've got the right one.
                        // Fall through to the return statement.
                    }
#if SSL
                    else if ( nextHitInfo is RayLinesHitInfo )
                    {
                        // It's possible to hit a single SSL3D more than once
                        //  so the name comparison isn't good enough.
                        // But we do know that these casts are safe since model names are unique.
                        RayLinesHitInfo info1 = (RayLinesHitInfo)nextHitInfo;
                        RayLinesHitInfo info2 = (RayLinesHitInfo)myAnswer[ i ];
                        if ( info1.segmentIndex != info2.segmentIndex )
                        {
                            continue;
                        }
                        // It's not possible to hit the same segment more than once.
                        // We've got the right one.
                        // Fall through to the return statement.
                    }
#endif
                    return myAnswers[i];
                }
            }
            AddFailure("Avalon reported an unexpected hit");
            Log("*** Visual hit: {0}", nextHitInfo.visualHit);
            Log("***  Model hit: {0}", ModelIdentifier(nextHitInfo));
            Log("***  Point hit: {0}", nextHitInfo.visualPointHit);
            return null;
        }



        private string ModelIdentifier(RayModelHitInfo hitInfo)
        {
            if (hitInfo is RayMeshHitInfo)
            {
                RayMeshHitInfo i = (RayMeshHitInfo)hitInfo;
                return string.Format(
                                "{0} triangle:({1},{2},{3})",
                                i.modelHit,
                                i.vertexIndex1,
                                i.vertexIndex2,
                                i.vertexIndex3);
            }
#if SSL
            else if ( hitInfo is RayLinesHitInfo )
            {
                RayLinesHitInfo i = (RayLinesHitInfo)hitInfo;
                return string.Format( "{0} segment:({1})", i.modelHit, i.segmentIndex );
            }
#endif
            return hitInfo.modelHit;
        }



        private void VerifyMeshIntersection(RayMeshHitInfo theirAnswer, RayMeshHitInfo myAnswer)
        {
            if (theirAnswer.meshHit != myAnswer.meshHit || failOnPurpose)
            {
                AddFailure("MeshIntersection.MeshHit returned the wrong mesh for {0}", ModelIdentifier(myAnswer));
                Log("*** Expected: {0}", myAnswer.meshHit);
                Log("*** Actual:   {0}", theirAnswer.meshHit);
            }

            if (MathEx.NotCloseEnough(myAnswer.modelPointHit, theirAnswer.modelPointHit) ||
                 failOnPurpose)
            {
                AddFailure("ModelIntersection.ModelPointHit returned the wrong value for {0}", ModelIdentifier(myAnswer));
                Log("*** Expected: {0}", myAnswer.modelPointHit);
                Log("***   Actual: {0}", theirAnswer.modelPointHit);
            }

            if (MathEx.NotCloseEnough(theirAnswer.vertexWeight1, myAnswer.vertexWeight1) ||
                 MathEx.NotCloseEnough(theirAnswer.vertexWeight2, myAnswer.vertexWeight2) ||
                 MathEx.NotCloseEnough(theirAnswer.vertexWeight3, myAnswer.vertexWeight3) ||
                 failOnPurpose)
            {
                AddFailure("MeshIntersection.VertexWeight failed for {0}", ModelIdentifier(myAnswer));
                Log("*** Expected: Weight1 = {0}, Weight2 = {1}, Weight3 = {2}", myAnswer.vertexWeight1, myAnswer.vertexWeight2, myAnswer.vertexWeight3);
                Log("*** Actual:   Weight1 = {0}, Weight2 = {1}, Weight3 = {2}", theirAnswer.vertexWeight1, theirAnswer.vertexWeight2, theirAnswer.vertexWeight3);
            }

            // We already know that vertexIndex* are the same
            //  otherwise we wouldn't be comparing these right now.
            //  (see method: "FindMyMatchingAnswer")
        }


#if SSL
        private void            VerifyLinesIntersection( RayLinesHitInfo theirAnswer, RayLinesHitInfo myAnswer )
        {
            if ( MathEx.NotCloseEnough( theirAnswer.segmentLocation, myAnswer.segmentLocation ) ||
                 failOnPurpose )
            {
                AddFailure( "LinesIntersection.SegmentLocation failed for {0}", ModelIdentifier( myAnswer ) );
                Log( "*** Expected: Location = {0}", myAnswer.segmentLocation );
                Log( "*** Actual:   Location = {0}", theirAnswer.segmentLocation );
            }

            // We already know that segmentIndex is the same
            //  otherwise we wouldn't be comparing these right now.
            //  (see method: "FindMyMatchingAnswer")
        }
#endif
        /// <summary/>
        protected abstract bool OnlyVerifyClosest { get; }

        #endregion

        /// <summary>Device Dependent Bounds</summary>
        protected Bounds ddBounds;
        /// <summary/>
        protected HitTestFilterCallback filter;
        /// <summary/>
        protected List<RayModelHitInfo> myAnswers;
        /// <summary/>
        protected List<RayModelHitInfo> theirAnswers;
        /// <summary/>
        protected Ray pickRay;
        /// <summary/>
        protected SceneTestObjects parameters;

        private bool _keepGoing;
        private static byte[,] s_hitCursor;
    }
}