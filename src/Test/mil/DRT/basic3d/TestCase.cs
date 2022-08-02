// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Test cases for DrtBasic3D
//
//
//

/*
The test cases build separate 3D scenes for a left and a right window
as determined by the variable leftSide.  In some cases this will be an
"AB-test" (by analogy with stereo A/B tests) where the left and right
sides should look the same either visually or exactly.  In other
cases, not so much.
*/

using System;
using System.Diagnostics;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Globalization;

using Microsoft.Samples;

using HitTestResult2D = System.Windows.Media.HitTestResult;

namespace Tests
{
    public abstract class TestCase
    {
        public override abstract string ToString();

        public virtual bool DoTest()
        {
            return true;
        }

        /// <summary>
        /// Do round-trip serialization of xaml for this test.  Can be
        /// skipped if it would take too long or if it's otherwise not
        /// a good idea.
        /// </summary>
        public virtual bool DoRoundtrip()
        {
            return true;
        }

        /// <summary>
        /// Clear first and then create a scene for the "left" Viewport3D
        /// in the left vs. right testing technique.  Override core
        /// functions BuildCamera, BuildLights and BuildGeometricScene to modify the
        /// testcase.
        /// </summary>
        /// <param name="visual">Visual in which to create the scene</param>
        public void BuildSceneLeft(Viewport3D visual)
        {
            _leftVisual = visual;
            double aspect = visual.ActualWidth / visual.ActualHeight;
            visual.Camera = BuildCamera(true,aspect);
            visual.Children.Clear();
            visual.Children.Add(BuildScene(/*leftSide = */ true));
        }

        /// <summary>
        /// Clear first and then create a scene for the "right" Viewport3D
        /// in the left vs. right testing technique.  Override core
        /// functions BuildCameraLights and BuildGeometricScene to modify the
        /// testcase.
        /// </summary>
        /// <param name="visual">Visual in which to create the scene</param>
        public void BuildSceneRight(Viewport3D visual)
        {
            _rightVisual = visual;
            double aspect = visual.ActualWidth / visual.ActualHeight;
            visual.Camera = BuildCamera(false,aspect);
            visual.Children.Clear();
            visual.Children.Add(BuildScene(/*leftSide = */ false));
        }

        protected virtual ModelVisual3D BuildScene(bool leftSide)
        {
            ModelVisual3D modelVisual3D = new ModelVisual3D();

            Model3DGroup group = new Model3DGroup();
            modelVisual3D.Content = group;
            BuildLights(group, /* leftSide = */ leftSide);
            BuildGeometricScene(group, /* leftSide = */ leftSide);

            return modelVisual3D;
        }

        protected virtual Camera BuildCamera(bool leftSide, double aspect)
        {
            Point3D cameraPosition = new Point3D(0.5, 0.5, 2.5);
            Vector3D cameraDirection = new Vector3D(-0.5, -0.5, -2.5);
            Vector3D cameraUp = new Vector3D(0, 1, 0);
            PerspectiveCamera camera = new PerspectiveCamera(cameraPosition, cameraDirection, cameraUp, /* fieldOfView (degrees) = */ 40);
            camera.NearPlaneDistance = 0.25;
            camera.FarPlaneDistance = 10;
            return camera;
        }

        protected virtual void BuildLights(Model3DGroup group, bool leftSide)
        {
            DirectionalLight light = new DirectionalLight(Colors.White, new Vector3D(0,0,-1));
            group.Children.Add(light);
        }

        /// <summary>
        /// Build the geometric (not lights or cameras) scene for the basic 3D testing.  Create
        /// different scenes (suitable for visual or automatic comparison) for the "left"
        /// and "right" visuals based on the bool parameter.
        /// </summary>
        /// <param name="group">Group to build the scene into</param>
        /// <param name="leftSide">Which side to build for</param>
        protected virtual void BuildGeometricScene(Model3DGroup group, bool leftSide) {}

        protected Viewport3D _leftVisual,_rightVisual;

        // static helpers
        public static LinearGradientBrush LinearGradientBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush();
                brush.GradientStops.Add(new GradientStop(Colors.Yellow, 0.0));
                brush.GradientStops.Add(new GradientStop(Colors.White, 0.25));
                brush.GradientStops.Add(new GradientStop(Colors.White, 0.75));
                brush.GradientStops.Add(new GradientStop(Colors.Blue, 1.0));
                return brush;
            }
        }

        public static DrawingBrush CheckeredBrush
        {
            get
            {
                DrawingGroup checker = new DrawingGroup();
                DrawingContext dc = checker.Open();
                dc.DrawRectangle(Brushes.Black,null,new Rect(0,0,1000,1000));
                dc.DrawRectangle(Brushes.Blue,null,new Rect(0,0,1,1));
                dc.DrawRectangle(Brushes.Blue,null,new Rect(1,1,1,1));
                dc.DrawRectangle(TestCase.LinearGradientBrush,null,new Rect(0,1,1,1));
                dc.DrawRectangle(TestCase.LinearGradientBrush,null,new Rect(1,0,1,1));
                dc.DrawText(new FormattedText("Hi", CultureInfo.GetCultureInfo("en-US"), FlowDirection.LeftToRight, new Typeface("Arial"), 1, Brushes.Black), new Point(0.5,0.5));
                dc.Close();
                DrawingBrush brush = new DrawingBrush(checker);
                brush.Opacity = 1;
                brush.Viewbox = new Rect(0,0,2,2);
                brush.ViewboxUnits = BrushMappingMode.Absolute;
                brush.Viewport = new Rect(0,0,1,1);
                brush.TileMode = TileMode.None;
                brush.Stretch = Stretch.Fill;
                return brush;
            }
        }

        public static AnimationTimeline BrushAnimation
        {
            get
            {
                Freezable[] changeables = new Freezable[]
                                           {
                                               DrtBasic3D.ChangingBrush,
                                               Brushes.Blue,
                                               LinearGradientBrush,
                                               Brushes.Green,
                                           };
                CycleAnimation animation = new CycleAnimation(changeables);
                animation.RepeatBehavior = RepeatBehavior.Forever;
                animation.AutoReverse = true;
                animation.Duration = new TimeSpan(0,0,0,0,200);
                return animation;
            }
        }

        public static AnimationTimeline BrushAnimation2
        {
            get
            {
                Freezable[] changeables = new Freezable[]
                                           {
                                               DrtBasic3D.ChangingBrush,
                                               Brushes.Blue,
                                           };
                CycleAnimation animation = new CycleAnimation(changeables);
                animation.RepeatBehavior = RepeatBehavior.Forever;
                animation.AutoReverse = true;
                animation.Duration = new TimeSpan(0,0,0,0,2000);
                return animation;
            }
        }

        public static Brush AnimatedBrush
        {
            get
            {
                BitmapSource id = BitmapFrame.Create(new Uri(@"DrtFiles\DrtBasic3D\Combined.png", UriKind.RelativeOrAbsolute), BitmapCreateOptions.None, BitmapCacheOption.Default);
                ImageBrush ib = new ImageBrush(id);
                ib.TileMode = TileMode.Tile;
                ScaleTransform scaleTransform = new ScaleTransform( 2,2 );
                DoubleAnimation scaleAnimation = new DoubleAnimation(1, 2, new TimeSpan(0,0,0,0,1000));
                scaleAnimation.RepeatBehavior = RepeatBehavior.Forever;
                scaleAnimation.AutoReverse = true;
                scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);
                ib.Transform = scaleTransform;
                return ib;
            }
        }

        // Returns an animation for materials that returns one material that has brush animations!
        // 
        public static AnimationTimeline SuperMaterialAnimation
        {
            get
            {
                Material[] materials = new Material[3];

                materials[0] = new DiffuseMaterial(AnimatedBrush);
                materials[1] = new DiffuseMaterial(LinearGradientBrush);

                Material animatedMaterial = new DiffuseMaterial(Brushes.Blue);
                animatedMaterial.BeginAnimation(DiffuseMaterial.BrushProperty, BrushAnimation);
                materials[2] = animatedMaterial;

                CycleAnimation animation = new CycleAnimation(materials);
                animation.RepeatBehavior = RepeatBehavior.Forever;
                animation.AutoReverse = true;
                animation.Duration = new TimeSpan(0,0,0,0,2222);
                return animation;
            }
        }

        // Hit testing support / helper functions.  Mostly used by
        // dedicated TestHitTesting, but moved up here so that more
        // tests can do a little hit testing tests and TestHitTesting
        // doesn't have to carry the burden of testing every type of
        // hit testing.

        private int _hit = 0;

        public HitTestResultBehavior HTResult(HitTestResult2D result2d)
        {
            RayHitTestResult rayResult = result2d as RayHitTestResult;

            if (rayResult != null)
            {
                if (rayResult.VisualHit == null)
                {
                    Console.WriteLine("FAILED: RayHitTestResult.Visual should not be null.");

                    // early exit without setting _hit to indicate failure.
                    return HitTestResultBehavior.Stop;
                }

                if (rayResult.ModelHit == null)
                {
                    Console.WriteLine("FAILED: RayHitTestResult.ModelHit should not be null.");

                    // early exit without setting _hit to indicate failure.
                    return HitTestResultBehavior.Stop;
                }

                _hit += ExtraHitTesting(rayResult) ? 1 : 0;
            }

            return HitTestResultBehavior.Continue;
        }

        public virtual bool ExtraHitTesting(RayHitTestResult rayResult)
        {
            return true;
        }

        public bool DoHitTest(Visual target, Point p)
        {
            _hit = 0;
            
            VisualTreeHelper.HitTest(
                target,
                null,
                new HitTestResultCallback(HTResult),
                new PointHitTestParameters(p));

            return _hit > 0;
        }
        
        public bool DoHitTest(Visual target, Point p, int expectedHits)
        {
            return DoHitTest(target, new Point[] { p }, expectedHits);
        }

        public bool DoHitTest(Visual target, Point[] points, int expectedHits)
        {
            _hit = 0;

            foreach (Point p in points)
            {
                VisualTreeHelper.HitTest(
                    target,
                    null,
                    new HitTestResultCallback(HTResult),
                    new PointHitTestParameters(p));
            }

            if (_hit == expectedHits)
            {
                return true;
            }
            else
            {
                string targetName = (target == _leftVisual) ? "Left" :
                                    (target == _rightVisual) ? "Right" : "Unknown";
                Console.WriteLine(String.Format("Failed DoHitTest in {0} ({1}).  Expected {2} got {3} hits.",
                                                this, targetName,
                                                expectedHits, _hit));
                return false;
            }
        }
    }

    public class TestSuite
    {
        public TestSuite()
        {
            _list = new List<TestCase>();
            _list.Add(new TestDefaults());
            _list.Add(new TestMaterialColors());
            _list.Add(new TestNormalMagic());
            _list.Add(new TestNonIndexedMeshes());
            _list.Add(new TestFlatSpheres());
            _list.Add(new TestRotationAnimationComposition());
            _list.Add(new TestWeirdMeshes());
            _list.Add(new TestVisualBrush());
            _list.Add(new TestSuperAnimation());
            _list.Add(new TestDataBinding());
            _list.Add(new TestRotate());
            _list.Add(new TestScale());
            _list.Add(new TestTranslate());
            _list.Add(new TestNonAffine());
            _list.Add(new TestTransformGroup());
            _list.Add(new TestLinearGradients());
            _list.Add(new TestLGWhite());
            _list.Add(new TestLights());
            _list.Add(new TestPerspectiveAndMatrix());
            _list.Add(new TestOrthoAndMatrix());
            _list.Add(new TestCheckeredBrush());
            _list.Add(new TestRadialGradientBrush());
            _list.Add(new TestImageBrush());
            _list.Add(new TestImageBrushSoftwareOnly());
            _list.Add(new TestImageBrushDefault());
            _list.Add(new TestEmissiveMaterial());
            _list.Add(new TestSpecularMaterial());
            _list.Add(new TestMaterialGroup());
            _list.Add(new TestBackMaterial());
            _list.Add(new TestHitTesting());
            _list.Add(new TestHitTestingClipped());
            _list.Add(new TestMaterialAnimations());
            _list.Add(new TestLightColorAnimations());
            _list.Add(new TestLightPositionAnimations());
            _list.Add(new TestLightConstantLinearAttenuationAnimations());
            _list.Add(new TestLightQuadraticAttenuationRangeAnimations());
            _list.Add(new TestLightConeAngleAnimations());
            _list.Add(new TestTranslateScaleAnimations());
            _list.Add(new TestAngleVsQuaternion());
            _list.Add(new TestQuaternionUseShortestPath());
            _list.Add(new TestRotateGroupAnimations());
            _list.Add(new TestCameraTransformAnimations());
            _list.Add(new TestCameraPositionUpAnimations());
            _list.Add(new TestCameraLookAtAnimations());
            _list.Add(new TestCameraNegativeNearPlane());
            _list.Add(new TestSmallVertexSmallIndexPrimitives());
            _list.Add(new TestSmallVertexLargeIndexPrimitives());
            _list.Add(new TestLargeVertexLargeIndexPrimitives());
            _list.Add(new TestLargeVertexNonIndexedPrimitive());
            _list.Add(new TestModelVisual3D());
            _list.Add(new TestVisualTreeHelper());
            _list.Add(new TestMath());
        }

        public TestCase this [ int index ]
        {
            get
            {
                return (TestCase) _list[index];
            }
        }

        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        List<TestCase> _list;
    }

    public class TestDefaults : TestCase
    {
        public override string ToString() { return "Defaults"; }

        protected override void BuildLights(Model3DGroup group, bool leftSide)
        {
            if (leftSide)
            {
                DirectionalLight dl = new DirectionalLight();
                group.Children.Add(dl);
            }
            else
            {
                SpotLight sl = new SpotLight();
                sl.Transform = new TranslateTransform3D(new Vector3D(0, 0, 0.5));
                group.Children.Add(sl);
            }
        }

        protected override Camera BuildCamera(bool leftSide, double aspect)
        {
            ProjectionCamera camera;

            if (leftSide)
            {
                camera = new PerspectiveCamera();
            }
            else
            {
                camera = new OrthographicCamera();
            }

            camera.Position = new Point3D(0, 0, 2.4142135623730949);

            return camera;
        }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            PlaneXY plane = new PlaneXY(1, 1);
            GeometryModel3D primitive = new GeometryModel3D(plane.CreateMesh(20,20), new DiffuseMaterial(Brushes.White));
            group.Children.Add(primitive);
        }
    }

    public class TestMaterialColors : TestCase
    {
        public override string ToString() { return "Material Colors"; }

        protected override void BuildLights(Model3DGroup group, bool leftSide)
        {
            group.Children.Add(new DirectionalLight());
            group.Children.Add(new DirectionalLight(Colors.White, new Vector3D(0,0,1)));
            group.Children.Add(new AmbientLight());

            if (!leftSide)
            {
                group.Children.Add(new DirectionalLight());
                group.Children.Add(new DirectionalLight(Colors.White, new Vector3D(0,0,1)));
                group.Children.Add(new AmbientLight());

                group.Children.Add(new DirectionalLight());
                group.Children.Add(new DirectionalLight(Colors.White, new Vector3D(0,0,1)));
                group.Children.Add(new AmbientLight());
            }
        }

        protected override Camera BuildCamera(bool leftSide, double aspect)
        {
            ProjectionCamera camera;
            camera = new PerspectiveCamera();
            camera.Position = new Point3D(0, 0, 2.4142135623730949);

            return camera;
        }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            ImageBrush ib = new ImageBrush(BitmapFrame.Create(
                new Uri(@"DrtFiles\DrtBasic3d\msnbackground.bmp", UriKind.RelativeOrAbsolute),
                BitmapCreateOptions.None, BitmapCacheOption.Default));

            PlaneXY plane = new PlaneXY(1, 1);
            MeshGeometry3D mesh = plane.CreateMesh(20,20);

            ScaleTransform3D halfScale = new ScaleTransform3D(new Vector3D(0.5, 0.5, 0.5));

            DiffuseMaterial am = new DiffuseMaterial(Brushes.White);
            am.Color = Colors.Black;
            am.AmbientColor = Color.FromRgb(0xFF, 0x44, 0x44);

            DiffuseMaterial am2 = am.Clone();
            am2.Brush = ib;

            DiffuseMaterial dm = new DiffuseMaterial(Brushes.White);
            dm.Color = Color.FromRgb(0x44, 0xFF, 0x44);
            dm.AmbientColor = Colors.Black;

            DiffuseMaterial dm2 = dm.Clone();
            dm2.Brush = ib;

            EmissiveMaterial em = new EmissiveMaterial(Brushes.White);
            em.Color = Color.FromRgb(0x44, 0x44, 0xFF);

            EmissiveMaterial em2 = em.Clone();
            em2.Brush = ib;

            SpecularMaterial sm = new SpecularMaterial(Brushes.White, 40.0);
            sm.Color = Color.FromRgb(0xFF, 0x44, 0xFF);
            
            SpecularMaterial sm2 = sm.Clone();
            sm2.Brush = ib;

            Model3DGroup plane1 = new Model3DGroup();
            plane1.Transform = new Transform3DGroup();
            ((Transform3DGroup)plane1.Transform).Children = new Transform3DCollection(
                new Transform3D[] { halfScale, new TranslateTransform3D(-0.5,0.5,0) });
            GeometryModel3D gm1 = new GeometryModel3D(mesh, am);
            gm1.BackMaterial = am2;
            plane1.Children.Add(gm1);
            group.Children.Add(plane1);

            Model3DGroup plane2 = new Model3DGroup();
            plane2.Transform = new Transform3DGroup();
            ((Transform3DGroup)plane2.Transform).Children = new Transform3DCollection(
                new Transform3D[] { halfScale, new TranslateTransform3D(0.5,0.5,0) });
            GeometryModel3D gm2 = new GeometryModel3D(mesh, dm);
            gm2.BackMaterial = dm2;
            plane2.Children.Add(gm2);
            group.Children.Add(plane2);

            Model3DGroup plane3 = new Model3DGroup();
            plane3.Transform = new Transform3DGroup();
            ((Transform3DGroup)plane3.Transform).Children = new Transform3DCollection(
                new Transform3D[] { halfScale, new TranslateTransform3D(0.5,-0.5,0) });
            GeometryModel3D gm3 = new GeometryModel3D(mesh, em);
            gm3.BackMaterial = em2;
            plane3.Children.Add(gm3);
            group.Children.Add(plane3);
            
            Model3DGroup plane4 = new Model3DGroup();
            plane4.Transform = new Transform3DGroup();
            ((Transform3DGroup)plane4.Transform).Children = new Transform3DCollection(
                new Transform3D[] { halfScale, new TranslateTransform3D(-0.5,-0.5,0) });
            GeometryModel3D gm4 = new GeometryModel3D(mesh, sm);
            gm4.BackMaterial = sm2;
            plane4.Children.Add(gm4);
            group.Children.Add(plane4);
        }
    }
    
    public class TestNormalMagic : TestCase
    {
        public override string ToString() { return "Normal Magic"; }
    
        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            ImageBrush brush = new ImageBrush(BitmapFrame.Create(
                new Uri(@"DrtFiles\DrtBasic3d\msnbackground.bmp", UriKind.RelativeOrAbsolute),
                BitmapCreateOptions.None, BitmapCacheOption.Default));

            PlaneXY plane = new PlaneXY(1, 1);
            GeometryModel3D mesh = new GeometryModel3D(plane.CreateMesh(2,2), new DiffuseMaterial(brush));
            mesh.Transform = new TranslateTransform3D(new Vector3D(0, 0.55, 0));

            Model3DGroup reflection = new Model3DGroup();
            reflection.Transform = new ScaleTransform3D(new Vector3D(1, -1, 1));
            reflection.Children.Add(mesh);

            group.Children.Add(mesh);
            group.Children.Add(reflection);
        }

        protected override Camera BuildCamera(bool leftSide, double aspect)
        {
            ProjectionCamera camera = new PerspectiveCamera();
            camera.Position = new Point3D(0, 0, 2.4142135623730949);

            if (leftSide)
            {
                camera.Transform = new ScaleTransform3D(new Vector3D(-1,1,1));
            }

            return camera;
        }    

        public override bool DoTest()
        {
            Point[] corners = {
                new Point(99,57),
                new Point(293,57),
                new Point(99,246),
                new Point(293,246),
                new Point(99,252),
                new Point(293,252),
                new Point(293,462),
                new Point(99,462)
            };

            Point[] missed = {
                new Point(97,55),
                new Point(295,55),
                new Point(97,248),
                new Point(295,248),
                new Point(97,250),
                new Point(295,250),
                new Point(295,464),
                new Point(97,464)
            };

            bool success = true;
            
            success &= DoHitTest(_leftVisual, corners, corners.Length);
            success &= DoHitTest(_leftVisual, missed, 0);
            success &= DoHitTest(_rightVisual, corners, corners.Length);
            success &= DoHitTest(_rightVisual, missed, 0);
            
            return success;
        }
    }
    
    abstract public class TestDifferentTransforms : TestCase
    {
        protected abstract Transform3D LeftTransform();
        protected abstract Transform3D RightTransform();

        protected override void BuildGeometricScene(Model3DGroup group, bool left)
        {
            //
            // Set up scene
            //

            SolidColorBrush brush = new SolidColorBrush(Colors.White);
            DiffuseMaterial material = new DiffuseMaterial(brush);
            Torus torus = new Torus();
            GeometryModel3D primitive = new GeometryModel3D(torus.MeshGeometry3D, material);

            primitive.Transform = left ? LeftTransform() : RightTransform();
            group.Children.Add(primitive);
        }
    }

    public class TestTransformGroup : TestDifferentTransforms
    {
        public override string ToString() { return "Transform Collection"; }
        static Vector3D s_scale = new Vector3D(2,3,4);
        static Vector3D s_translation = new Vector3D(0.2, 0.3, 0.4);

        protected override Transform3D LeftTransform()
        {
            Transform3DGroup tgroup = new Transform3DGroup();
            tgroup.Children.Add(new ScaleTransform3D(s_scale));
            tgroup.Children.Add(new TranslateTransform3D(s_translation));
            return tgroup;
        }

        protected override Transform3D RightTransform()
        {
            Matrix3D m = new Matrix3D();
            m.Scale(s_scale);
            m.Translate(s_translation);
            return new MatrixTransform3D(m);
        }
    }

    public class TestScale : TestDifferentTransforms
    {
        public override string ToString() { return "Scale"; }
        const double sx = 1.5;
        const double sy = 2.2;
        const double sz = 0.35;

        protected override Transform3D LeftTransform()
        {
            return new ScaleTransform3D(new Vector3D(sx, sy, sz));
        }

        protected override Transform3D RightTransform()
        {
            Matrix3D m = new Matrix3D(sx, 0, 0, 0,
                                      0,sy, 0, 0,
                                      0, 0,sz, 0,
                                      0, 0, 0, 1);
            return new MatrixTransform3D(m);
        }
    }

    public class TestTranslate : TestDifferentTransforms
    {
        public override string ToString() { return "Translate"; }
        const double tx = 0.1;
        const double ty = 0.2341;
        const double tz = 0.345;

        protected override Transform3D LeftTransform()
        {
            return new TranslateTransform3D(new Vector3D(tx, ty, tz));
        }

        protected override Transform3D RightTransform()
        {
            Matrix3D m = new Matrix3D(1, 0, 0, 0,
                                      0, 1, 0, 0,
                                      0, 0, 1, 0,
                                      tx,ty,tz,1);
            return new MatrixTransform3D(m);
        }
    }

    public class TestRotate : TestDifferentTransforms
    {
        public override string ToString() { return "Rotate"; }
        static Vector3D axis = new Vector3D(1,1,1);
        const double angle = 30;
        const double radians = Math.PI * angle / 180.0;

        protected override Transform3D LeftTransform()
        {
            return new RotateTransform3D(new AxisAngleRotation3D(axis,angle));
        }

        protected override Transform3D RightTransform()
        {
            axis.Normalize();
            double u = axis.X;
            double v = axis.Y;
            double w = axis.Z;
            double u2 = u * u;
            double v2 = v * v;
            double w2 = w * w;
            double uv = u*v;
            double uw = u*w;
            double vw = v*w;
            double cos = Math.Cos(radians);
            double sin = Math.Sin(radians);

            double r11 = u2+(v2+w2)*cos;
            double r21 = (1-cos)*uv - w * sin;
            double r31 = (1-cos)*uw + v * sin;

            double r12 = (1-cos)*uv+w*sin;
            double r22 = v2 + (u2+w2)*cos;
            double r32 = (1-cos)*vw-u*sin;

            double r13 = (1-cos)*uw-v*sin;
            double r23 = (1-cos)*vw+u*sin;
            double r33 = w2 + (u2+v2)*cos;

            Matrix3D m = new Matrix3D(r11, r12, r13, 0,
                                      r21, r22, r23, 0,
                                      r31, r32, r33, 0,
                                      0,   0,   0, 1);

            return new MatrixTransform3D(m);
        }
    }

    // Apply a non-affine MatrixTransform to a Model3D.
    public class TestNonAffine : TestDifferentTransforms
    {
        public override string ToString() { return "Non-Affine"; }
        const double tx = 0.1;
        const double ty = 0.2341;
        const double tz = 0.345;

        private static readonly Matrix3D view = new Matrix3D(
            0.98058067569092, -0.0377425678048199, 0.192450089729875, 0,
                           0,   0.981306762925316, 0.192450089729875, 0,
          -0.196116135138184,  -0.188712839024099, 0.962250448649376, 0,
          5.55111512312578E-17,5.55111512312578E-17,-2.59807621135332,1);

        private static readonly Matrix3D proj = new Matrix3D(
          2.74747741945462, 0, 0, 0,
          0,2.74747741945462, 0, 0,
          0, 0, -1.02564102564103, -1,
          0, 0, -0.256410256410256,0);

        public override bool DoTest()
        {
            return DoHitTest(_leftVisual, new Point(157, 250));
        }

        protected override Camera BuildCamera(bool leftSide, double aspect)
        {
            // We apply the camera transform to the Model3D itself so we
            // set camera to Identity.
            return new MatrixCamera();
        }

        protected override Transform3D LeftTransform()
        {
            return new MatrixTransform3D(view * proj);
        }

        protected override Transform3D RightTransform()
        {
            return new MatrixTransform3D(view * proj);
        }
    }

    public class TestLinearGradients : TestCase
    {
        public override string ToString() { return "Linear Gradient"; }
        protected override void BuildGeometricScene(Model3DGroup group, bool left)
        {
            LinearGradientBrush brush;

            if (left)
            {
                brush = new LinearGradientBrush(Colors.Red, Colors.Blue, new Point(1,1), new Point(0,0));
                brush.GradientStops.Add(new GradientStop(Colors.White, 0.5));
            }
            else
            {
                brush = new LinearGradientBrush();
                brush.GradientStops.Add(new GradientStop(Colors.Red, 0.0));
                brush.GradientStops.Add(new GradientStop(Colors.White, 0.25));
                brush.GradientStops.Add(new GradientStop(Colors.White, 0.75));
                brush.GradientStops.Add(new GradientStop(Colors.Blue, 1.0));
            }

            DiffuseMaterial material = new DiffuseMaterial(brush);
            Torus torus = new Torus();
            GeometryModel3D primitive = new GeometryModel3D(torus.MeshGeometry3D, material);
            group.Children.Add(primitive);
        }
    }

    public class TestLGWhite : TestCase
    {
        // Compares a linear gradient with only white stops to a constant white brush.
        public override string ToString() { return "Simple Gradient vs. White"; }
        protected override void BuildGeometricScene(Model3DGroup group, bool left)
        {
            Brush brush;
            if (left)
            {
                LinearGradientBrush lgb = new LinearGradientBrush();
                lgb.GradientStops.Add(new GradientStop(Colors.White, 0.0));
                lgb.GradientStops.Add(new GradientStop(Colors.White, 0.5));
                lgb.GradientStops.Add(new GradientStop(Colors.White, 1.0));
                brush = lgb;
            }
            else
            {
                brush = Brushes.White;
            }

            DiffuseMaterial material = new DiffuseMaterial(brush);
            Torus torus = new Torus();
            GeometryModel3D primitive = new GeometryModel3D(torus.MeshGeometry3D, material);
            group.Children.Add(primitive);
        }
    }

    public class TestModelCollectionTransform : TestCase
    {
        public override string ToString() { return "Model vs. Prim"; }
        protected override void BuildGeometricScene(Model3DGroup group, bool left)
        {
            Torus torus = new Torus();
            GeometryModel3D primitive = new GeometryModel3D(torus.MeshGeometry3D, new DiffuseMaterial(Brushes.White) );
            Model3DGroup collection = new Model3DGroup();
            collection.Children.Add(primitive);
            Transform3D transform = new ScaleTransform3D(new Vector3D(2, 2, 2));
            if(left)
                primitive.Transform = transform;
            else
                collection.Transform = transform;
            group.Children.Add(collection);
        }
    }

    public abstract class CameraTests : TestCase
    {
        // Base class for testing different cameras.  The derived
        // class should override BuildCamera(...)

        protected override void BuildLights(Model3DGroup group, bool leftSide)
        {
            // Lights!
            DirectionalLight light = new DirectionalLight(Colors.White, new Vector3D(0,0,-1));
            group.Children.Add(light);
        }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            Torus torus = new Torus();
            GeometryModel3D primitive = new GeometryModel3D(torus.MeshGeometry3D, new DiffuseMaterial(Brushes.White) );
            group.Children.Add(primitive);
        }

        protected Matrix3D GetViewMatrix(Point3D position, Vector3D lookDirection, Vector3D up)
        {
            Vector3D zaxis = -lookDirection;
            zaxis.Normalize();

            Vector3D xaxis = Vector3D.CrossProduct(up, zaxis);
            xaxis.Normalize();

            Vector3D yaxis = Vector3D.CrossProduct(zaxis, xaxis);

            double cx = -Vector3D.DotProduct(xaxis, (Vector3D)position);
            double cy = -Vector3D.DotProduct(yaxis, (Vector3D)position);
            double cz = -Vector3D.DotProduct(zaxis, (Vector3D)position);

            return new Matrix3D(
                xaxis.X, yaxis.X, zaxis.X, 0,
                xaxis.Y, yaxis.Y, zaxis.Y, 0,
                xaxis.Z, yaxis.Z, zaxis.Z, 0,
                cx, cy, cz, 1);
        }
    }

    public abstract class MatrixCameraTests : CameraTests
    {
        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            Torus torus = new Torus();
            GeometryModel3D primitive = new GeometryModel3D(torus.MeshGeometry3D, new DiffuseMaterial(Brushes.White) );

            group.Children.Add(primitive);
        }
    }

    public class TestPerspectiveAndMatrix : MatrixCameraTests
    {
        // This guy will compare a perspective camera with a similar
        // orthographic camera.  In the future, perhaps we should
        // actually compare each with an appropriate MatrixCamera.
        public override string ToString() { return "Perspective vs. Matrix"; }
        protected override Camera BuildCamera(bool leftSide, double aspect)
        {
            Point3D position = new Point3D(0.5, 0.5, 2.5);
            Vector3D lookDirection = new Vector3D(-0.5, -0.5, -2.5);
            Vector3D up = new Vector3D(0, 1, 0);

            double zn = 0.24;
            double zf = 10;
            double hFoV = 40;

            if (leftSide)
            {
                PerspectiveCamera camera = new PerspectiveCamera(position, lookDirection, up, hFoV);
                camera.NearPlaneDistance = 0.24;
                camera.FarPlaneDistance = 10;
                return camera;
            }
            else
            {
                double hFoVRadians = (hFoV * Math.PI) / 180;
                double fovy = 2 * Math.Atan((1 / aspect) * Math.Tan(hFoVRadians / 2));

                double h = 1 / Math.Tan(fovy/2);
                double w = h / aspect;

                double m11 = w;
                double m22 = h;
                double m33 = zf / (zn - zf);
                double m43 = m33 * zn;

                Matrix3D projection = new Matrix3D(
                    m11, 0, 0, 0,
                    0, m22, 0, 0,
                    0, 0, m33, -1,
                    0, 0, m43, 0);
                return new MatrixCamera(GetViewMatrix(position, lookDirection, up), projection);
            }
        }

        public override bool DoTest()
        {
            Point[] hits = { new Point(240,262) };
            
            Point[] misses = {
                new Point(198,283),
                new Point(183,151)
            };

            bool success = true;

            success &= DoHitTest(_leftVisual, hits, hits.Length);
            success &= DoHitTest(_rightVisual, hits, hits.Length);
            success &= DoHitTest(_leftVisual, misses, 0);
            success &= DoHitTest(_rightVisual, misses, 0);

            return success;
        }
    }

    public class TestOrthoAndMatrix : MatrixCameraTests
    {
        public override string ToString() { return "Orthographic vs. Matrix"; }

        protected override Camera BuildCamera(bool leftSide, double aspect)
        {
            Point3D position = new Point3D(0.5, 0.5, 2.5);
            Vector3D lookDirection = new Vector3D(-0.5, -0.5, -2.5);
            Vector3D up = new Vector3D(0, 1, 0);

            double zn = 0.24;
            double zf = 10;
            double w = 2;

            if (leftSide)
            {
                OrthographicCamera camera = new OrthographicCamera(position, lookDirection, up, w);
                camera.NearPlaneDistance = 0.24;
                camera.FarPlaneDistance = 10;
                return camera;
            }
            else
            {
                double h = w / aspect;

                double m11 = 2.0f / w;
                double m22 = 2.0f / h;
                double m33 = 1.0f / (zn - zf);
                double m43 = m33 * zn;

                Matrix3D projection = new Matrix3D(
                    m11, 0, 0, 0,
                    0, m22, 0, 0,
                    0, 0, m33, 0,
                    0, 0, m43, 1);
                return new MatrixCamera(GetViewMatrix(position, lookDirection, up), projection);
            }
        }

        public override bool DoTest()
        {
            Point[] hits = { new Point(210,289) };

            Point[] misses = {
                new Point(218,264),
                new Point(207,135)
            };

            bool success = true;

            success &= DoHitTest(_leftVisual, hits, hits.Length);
            success &= DoHitTest(_rightVisual, hits, hits.Length);
            success &= DoHitTest(_leftVisual, misses, 0);
            success &= DoHitTest(_rightVisual, misses, 0);

            return success;
        }

    }

    public class TestLights : TestCase
    {
        public override string ToString() { return "All lights"; }

        protected override void BuildLights(Model3DGroup group, bool leftSide)
        {
            if (leftSide)
            {
                DirectionalLight dl = new DirectionalLight(Colors.White, new Vector3D(0,0,-1));
                PointLight pl = new PointLight(Colors.Green, new Point3D(0,1,0));
                group.Children.Add(dl);
                group.Children.Add(pl);
            }
            else
            {
                AmbientLight al = new AmbientLight(Colors.Blue);
                SpotLight sl = new SpotLight(Colors.Red, new Point3D(0,0,-1), new Vector3D(0,0,1), 5, 10);
                group.Children.Add(al);
                group.Children.Add(sl);
            }
        }

        protected override Camera BuildCamera(bool leftSide, double aspect)
        {
            Point3D cameraPosition = new Point3D(0.5, 0.5, 2.5);
            Vector3D cameraDirection = new Vector3D(-0.5, -0.5, -2.5);
            Vector3D cameraUp = new Vector3D(0, 1, 0);
            PerspectiveCamera camera = new PerspectiveCamera(cameraPosition, cameraDirection, cameraUp, /* fieldOfView (degrees) = */ 40);
            return camera;
        }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            Torus torus = new Torus();
            GeometryModel3D primitive = new GeometryModel3D(torus.MeshGeometry3D, new DiffuseMaterial(Brushes.White) );
            group.Children.Add(primitive);
        }
    }


    public class TestVisualBrush : TestCase
    {
        public override string ToString() { return "Visual Brush"; }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            Image img = new Image();
            img.Source = BitmapFrame.Create(new Uri(@"DrtFiles\DrtBasic3d\msnbackground.bmp", UriKind.RelativeOrAbsolute), BitmapCreateOptions.None, BitmapCacheOption.Default);
          
            PlaneXY plane = new PlaneXY(1, 1);
            GeometryModel3D primitive = new GeometryModel3D(plane.CreateMesh(51,51), new DiffuseMaterial(new VisualBrush(img)));

            group.Children.Add(primitive);

            if (!leftSide)
            {
                RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.NearestNeighbor);  
            }
        }
    }

    public class TestEmissiveMaterial : TestCase
    {
        public override string ToString() { return "DiffuseMaterial vs. EmissiveMaterial"; }

        protected override void BuildLights(Model3DGroup group, bool leftSide)
        {
            DirectionalLight dl = new DirectionalLight(Color.FromRgb(255,255,0), new Vector3D(0, -1, 0));
            group.Children.Add(dl);

            DirectionalLight dl2 = new DirectionalLight(Color.FromRgb(0,255,255), new Vector3D(0, 1, 0));
            group.Children.Add(dl2);

            AmbientLight al = new AmbientLight(Colors.White);
            group.Children.Add(al);
        }

        protected override Camera BuildCamera(bool leftSide, double aspect)
        {
            Point3D cameraPosition = new Point3D(0.5, 0.5, 2.5);
            Vector3D cameraDirection = new Vector3D(-0.5, -0.5, -2.5);
            Vector3D cameraUp = new Vector3D(0, 1, 0);
            PerspectiveCamera camera = new PerspectiveCamera(cameraPosition, cameraDirection, cameraUp, /* fieldOfView (degrees) = */ 40);
            return camera;
        }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            Torus torus = new Torus();
            GeometryModel3D primitive = new GeometryModel3D();
            primitive.Geometry = torus.MeshGeometry3D;

            if (leftSide)
            {
                primitive.Material = CreateTexture(@"DrtFiles\DrtBasic3D\Combined.png");
            }
            else
            {
                EmissiveMaterial em = new EmissiveMaterial();

                em.Brush = new ImageBrush(BitmapFrame.Create(new Uri(@"DrtFiles\DrtBasic3D\emissive.png", UriKind.RelativeOrAbsolute), BitmapCreateOptions.None, BitmapCacheOption.Default));

                primitive.Material = em;
            }

            group.Children.Add(primitive);
        }

        private Material CreateTexture(string filename)
        {
            BitmapSource id = BitmapFrame.Create(new Uri(filename, UriKind.RelativeOrAbsolute), BitmapCreateOptions.None, BitmapCacheOption.Default);
            ImageBrush ib = new ImageBrush(id);
            return new DiffuseMaterial(ib);
        }
    }

    public class TestSpecularMaterial : TestCase
    {
        public override string ToString() { return "DiffuseMaterial vs. SpecularMaterial"; }

        protected override void BuildLights(Model3DGroup group, bool leftSide)
        {
            DirectionalLight dl = new DirectionalLight(Color.FromRgb(255,255,0), new Vector3D(0, -1, 0));
            group.Children.Add(dl);

            DirectionalLight dl2 = new DirectionalLight(Color.FromRgb(0,255,255), new Vector3D(0, 1, 0));
            group.Children.Add(dl2);

            AmbientLight al = new AmbientLight(Colors.White);
            group.Children.Add(al);
        }

        protected override Camera BuildCamera(bool leftSide, double aspect)
        {
            Point3D cameraPosition = new Point3D(0.5, 0.5, 2.5);
            Vector3D cameraDirection = new Vector3D(-0.5, -0.5, -2.5);
            Vector3D cameraUp = new Vector3D(0, 1, 0);
            PerspectiveCamera camera = new PerspectiveCamera(cameraPosition, cameraDirection, cameraUp, /* fieldOfView (degrees) = */ 40);
            return camera;
        }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            Torus torus = new Torus();
            GeometryModel3D primitive = new GeometryModel3D();
            primitive.Geometry = torus.MeshGeometry3D;

            if (leftSide)
            {
                primitive.Material = CreateTexture(@"DrtFiles\DrtBasic3D\Combined.png");
            }
            else
            {
                SpecularMaterial sm = new SpecularMaterial();

                sm.Brush = new ImageBrush(BitmapFrame.Create(new Uri(@"DrtFiles\DrtBasic3D\specular.png", UriKind.RelativeOrAbsolute), BitmapCreateOptions.None, BitmapCacheOption.Default));
                sm.SpecularPower = 15.0;

                DoubleAnimation powerAnimation = new DoubleAnimation(0, 100, new TimeSpan(0, 0, 0, 0, 2000));
                powerAnimation.RepeatBehavior = RepeatBehavior.Forever;
                powerAnimation.AutoReverse = true;

                sm.BeginAnimation(SpecularMaterial.SpecularPowerProperty, powerAnimation);

                primitive.Material = sm;
            }

            group.Children.Add(primitive);
        }

        private Material CreateTexture(string filename)
        {
            BitmapSource id = BitmapFrame.Create(new Uri(filename, UriKind.RelativeOrAbsolute), BitmapCreateOptions.None, BitmapCacheOption.Default);
            ImageBrush ib = new ImageBrush(id);
            return new DiffuseMaterial(ib);
        }
    }

    public class TestMaterialGroup : TestCase
    {
        public override string ToString() { return "DiffuseMaterial vs. MaterialGroup (d+e)"; }

        protected override void BuildLights(Model3DGroup group, bool leftSide)
        {
            DirectionalLight dl = new DirectionalLight(Color.FromRgb(255,255,0), new Vector3D(0, -1, 0));
            group.Children.Add(dl);

            DirectionalLight dl2 = new DirectionalLight(Color.FromRgb(0,255,255), new Vector3D(0, 1, 0));
            group.Children.Add(dl2);

            AmbientLight al = new AmbientLight(Colors.White);
            group.Children.Add(al);
        }

        protected override Camera BuildCamera(bool leftSide, double aspect)
        {
            Point3D cameraPosition = new Point3D(0.5, 0.5, 2.5);
            Vector3D cameraDirection = new Vector3D(-0.5, -0.5, -2.5);
            Vector3D cameraUp = new Vector3D(0, 1, 0);
            PerspectiveCamera camera = new PerspectiveCamera(cameraPosition, cameraDirection, cameraUp, /* fieldOfView (degrees) = */ 40);
            return camera;
        }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            Torus torus = new Torus();
            GeometryModel3D primitive = new GeometryModel3D();
            primitive.Geometry = torus.MeshGeometry3D;

            if (leftSide)
            {
                primitive.Material = CreateTexture(@"DrtFiles\DrtBasic3D\Combined.png");
            }
            else
            {
                MaterialGroup mg = new MaterialGroup();

                mg.Children.Add(CreateTexture(@"DrtFiles\DrtBasic3D\diffuse.png"));

                EmissiveMaterial em = new EmissiveMaterial();
                em.Brush = Brushes.Yellow;
                mg.Children.Add(em);

                primitive.Material = mg;
            }

            group.Children.Add(primitive);
        }

        private Material CreateTexture(string filename)
        {
            BitmapSource id = BitmapFrame.Create(new Uri(filename, UriKind.RelativeOrAbsolute), BitmapCreateOptions.None, BitmapCacheOption.Default);
            ImageBrush ib = new ImageBrush(id);
            return new DiffuseMaterial(ib);
        }
    }

    public class TestBackMaterial : TestCase
    {
        public override string ToString() { return "BackMaterial"; }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            // Pyramid centered on the origin
            
            Point3D py = new Point3D(0, 1, 0);  // 0
            Point3D px = new Point3D(1, 0, 0);  // 1
            Point3D pz = new Point3D(0, 0, 1);  // 2
            Point3D nz = new Point3D(0, 0, -1); // 3
            Point3D nx = new Point3D(-1, 0, 0); // 4

            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions = new Point3DCollection(new Point3D[] { py, px, pz, nz, nx });
            
            GeometryModel3D model = new GeometryModel3D();
            model.Geometry = mesh;
            
            if (leftSide)
            {
                mesh.TriangleIndices = new Int32Collection(new int[]
                    { 0, 2, 1, 0, 1, 3, 0, 3, 4, 0, 4, 2, 1, 2, 3, 4, 3, 2 }
                    );

                // transparent green
                model.Material = new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(127, 0, 255, 0)));

                // solid red
                model.BackMaterial = new DiffuseMaterial(Brushes.Red);
            }
            else
            {
                // missing a front face
                mesh.TriangleIndices = new Int32Collection(new int[]
                    { 0, 1, 3, 0, 3, 4, 0, 4, 2, 1, 2, 3, 4, 3, 2 }
                    );

                DrawingGroup dg = new DrawingGroup();
                DrawingContext dc = dg.Open();
                dc.DrawRectangle(Brushes.Red, null, new Rect(0,0,1000,1000));
                dc.DrawText(new FormattedText("BOO!", CultureInfo.GetCultureInfo("en-US"), FlowDirection.LeftToRight, new Typeface("Arial"), 1, Brushes.Black), new Point(0.7,0.5));
                dc.Close();
                DrawingBrush brush = new DrawingBrush(dg);
                brush.Viewbox = new Rect(0,0,4,4);
                brush.ViewboxUnits = BrushMappingMode.Absolute;
                brush.Viewport = new Rect(0,0,1,1);
                brush.TileMode = TileMode.None;
                brush.Stretch = Stretch.Fill;

                Point t = new Point(0,0);
                Point r = new Point(1,0);
                Point b = new Point(.5,.5 * Math.Sqrt(3));

                mesh.TextureCoordinates = new PointCollection(new Point[]
                    { t, b, t, r, t} 
                    );

                // solid green
                model.Material = new DiffuseMaterial(Brushes.Green);
                               
                // solid red with text
                model.BackMaterial = new DiffuseMaterial(brush);

                Model3D flippedModel = model.Clone();
                Transform3DGroup tg = new Transform3DGroup();
                tg.Children = new Transform3DCollection(new Transform3D[] { new ScaleTransform3D(1, -1, 1), new TranslateTransform3D(0, -1.05, 0) } );
                flippedModel.Transform = tg;
                group.Children.Add(flippedModel);
            }

            group.Children.Add(model);
        }

        protected override void BuildLights(Model3DGroup group, bool leftSide)
        {
            PointLight pl = new PointLight(Color.FromRgb(255,255,255), new Point3D(.2, 6, .1));
            pl.Range = 10000;
            pl.ConstantAttenuation = 1;
            group.Children.Add(pl);
        }

        protected override Camera BuildCamera(bool leftSide, double aspect)
        {
            Point3D cameraPosition = new Point3D(0.5, 0.5, 4.5);
            Vector3D cameraDirection = new Vector3D(-0.5, -0.5, -4.5);
            Vector3D cameraUp = new Vector3D(0, 1, 0);
            PerspectiveCamera camera = new PerspectiveCamera(cameraPosition, cameraDirection, cameraUp, /* fieldOfView (degrees) = */ 40);
            return camera;
        }

        public override bool DoTest()
        {
            try
            {
                // left pyramid: both sides
                DoHitTest(_leftVisual, new Point(172, 214), 2);

                // top right pyramid: both sides
                DoHitTest(_rightVisual, new Point(110, 243), 2);

                // top right pyramid: back face only
                DoHitTest(_rightVisual, new Point(254, 210), 1);

                // bottom right pyramid: both
                DoHitTest(_rightVisual, new Point(161, 387), 2);

                // bottom right pyramid: back only
                DoHitTest(_rightVisual, new Point(209, 442), 1);
            }
            catch(Exception e)
            {
                Console.WriteLine("BackMaterial hit test failed horribly!");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return false;
            }

            return true;
        }
        
    }

    public class TestDataBinding : TestCase
    {
        public override string ToString() { return "DataBinding & Brush animations"; }

        protected override void BuildLights(Model3DGroup group, bool leftSide)
        {
            DirectionalLight dl = new DirectionalLight(Colors.White, new Vector3D(0, -1, 0));
            group.Children.Add(dl);
            DirectionalLight dl2 = new DirectionalLight(Colors.White, new Vector3D(0, 1, 0));
            group.Children.Add(dl2);
        }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            Torus torus = new Torus();
            GeometryModel3D primitive = new GeometryModel3D();
            primitive.Geometry = torus.MeshGeometry3D;

            if (leftSide)
            {
                ComboBox comboBox = DrtBasic3D.ComboBox;
                comboBox.Items.Clear();

                ComboBoxItem item;
                item = new ComboBoxItem();
                item.Content = Transform.Identity;
                comboBox.Items.Add(item);

                item = new ComboBoxItem();
                ScaleTransform scaleTransform = new ScaleTransform( 2,2 );
                DoubleAnimation scaleAnimation = new DoubleAnimation(1, 2, new TimeSpan(0,0,0,0,1000));
                scaleAnimation.RepeatBehavior = RepeatBehavior.Forever;
                scaleAnimation.AutoReverse = true;
                scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);
                item.Content = scaleTransform;
                comboBox.Items.Add(item);

                item = new ComboBoxItem();
                TranslateTransform translateTransform = new TranslateTransform( 0,0 );
                DoubleAnimation translateAnimation = new DoubleAnimation(1, 2, new TimeSpan(0,0,0,0,1000));
                translateAnimation.RepeatBehavior = RepeatBehavior.Forever;
                translateAnimation.IsAdditive = true;
                translateAnimation.AutoReverse = true;
                translateTransform.BeginAnimation(TranslateTransform.XProperty, translateAnimation);
                translateTransform.BeginAnimation(TranslateTransform.YProperty, translateAnimation);
                item.Content = translateTransform;
                comboBox.Items.Add(item);

                comboBox.SelectedIndex = 1;

                BitmapSource id = BitmapFrame.Create(new Uri(@"DrtFiles\DrtBasic3D\Combined.png", UriKind.RelativeOrAbsolute), BitmapCreateOptions.None, BitmapCacheOption.Default);
                ImageBrush ib = new ImageBrush(id);

                Binding binding = new Binding("SelectedItem.Content");
                binding.Mode = BindingMode.OneWay;
                binding.Source = comboBox;
                BindingOperations.SetBinding(ib, Brush.TransformProperty, binding);
                DrtBasic3D.SetDPDO(ib, Brush.TransformProperty);

                DiffuseMaterial bm = new DiffuseMaterial(ib);

                primitive.Material = bm;
            }
            else
            {
                DiffuseMaterial bm = new DiffuseMaterial();
                bm.Brush = Brushes.Blue;
                primitive.Material = bm;
                primitive.BeginAnimation(GeometryModel3D.MaterialProperty, SuperMaterialAnimation);
            }

            group.Children.Add(primitive);
        }
    }

    public class TestSuperAnimation : TestCase
    {
        public override string ToString() { return "Super Animation Tests"; }

        protected override void BuildLights(Model3DGroup group, bool leftSide)
        {
            DirectionalLight dl = new DirectionalLight(Colors.White, new Vector3D(0, -1, 0));
            group.Children.Add(dl);
            DirectionalLight dl2 = new DirectionalLight(Colors.White, new Vector3D(0, 1, 0));
            group.Children.Add(dl2);
        }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            Torus torus = new Torus();
            GeometryModel3D primitive = new GeometryModel3D();
            primitive.Geometry = torus.MeshGeometry3D;

            if (leftSide)
            {
                ComboBox comboBox = DrtBasic3D.ComboBox;
                comboBox.Items.Clear();

                ComboBoxItem item;
                item = new ComboBoxItem();
                item.Content = new DiffuseMaterial(Brushes.Blue);
                comboBox.Items.Add(item);

                item = new ComboBoxItem();
                DiffuseMaterial bm = new DiffuseMaterial(Brushes.Yellow);
                bm.BeginAnimation(DiffuseMaterial.BrushProperty, BrushAnimation2);
                item.Content = bm;
                comboBox.Items.Add(item);

                comboBox.SelectedIndex = 1;

                primitive.Material = bm;

                Binding binding = new Binding("SelectedItem.Content");
                binding.Mode = BindingMode.OneWay;
                binding.Source = comboBox;
                BindingOperations.SetBinding(primitive, GeometryModel3D.MaterialProperty, binding);
                DrtBasic3D.SetDPDO(primitive, GeometryModel3D.MaterialProperty);
            }
            else
            {
                DiffuseMaterial bm = new DiffuseMaterial();
                bm.Brush = Brushes.Blue;
                primitive.Material = bm;
                primitive.BeginAnimation(GeometryModel3D.MaterialProperty, SuperMaterialAnimation);
            }

            group.Children.Add(primitive);
        }
    }

    // NOTE, this test doesn't work yet (2003/11/24) and isn't enabled.
    public class TestCheckeredBrush : TestCase
    {
        public override string ToString() { return "Drawing Brush"; }

        public override bool DoRoundtrip()
        {
            // 



            return false;
        }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            DrawingBrush brush = TestCase.CheckeredBrush;

            PlaneXY plane = new PlaneXY(1, 1);

            GeometryModel3D primitive = new GeometryModel3D(plane.CreateMesh(51,51), new DiffuseMaterial(brush));

            group.Children.Add(primitive);
        }
    }

    public class TestWeirdMeshes : TestCase
    {
        public override string ToString() { return "Weird Meshes"; }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            Brush brush = Brushes.Blue;
            DiffuseMaterial bm = new DiffuseMaterial(brush);

            foreach(MeshGeometry3D mesh in WeirdMeshes.Meshes)
            {
                GeometryModel3D primitive = new GeometryModel3D(mesh, bm);
                group.Children.Add(primitive);
            }
        }

        public override bool DoTest()
        {
            try
            {
                // Make sure hit testing doesn't crash!
                DoHitTest(_leftVisual, new Point(100,100));
            }
            catch(Exception e)
            {
                Console.WriteLine( "Hit testing weird mesh failed!" );
                Console.WriteLine( e.Message );
                Console.WriteLine( e.StackTrace );
                return false;
            }

            return true;
        }
    }

    public class TestNonIndexedMeshes : TestCase
    {
        public override string ToString() { return "Non-Indexed Meshes"; }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            // Green pyramid centered at the origin

            Brush brush = Brushes.Green;
            DiffuseMaterial bm = new DiffuseMaterial(brush);

            Point3D py = new Point3D(0, 1, 0); // positive y
            Point3D px = new Point3D(1, 0, 0); // positive x
            Point3D pz = new Point3D(0, 0, 1); // you get the idea...
            Point3D nz = new Point3D(0, 0, -1);
            Point3D nx = new Point3D(-1, 0, 0);

            MeshGeometry3D mesh = new MeshGeometry3D();

            mesh.Positions = new Point3DCollection(new Point3D[]
                { py, pz, px, py, px, nz, py, nz, nx, py, nx, pz, px, pz, nz, nx, nz, pz }
                );

            if (leftSide)
            {
                mesh.TriangleIndices = new Int32Collection();
            }
            else
            {
                mesh.TriangleIndices = null;
            }

            group.Children.Add(new GeometryModel3D(mesh, bm));
        }

        protected override void BuildLights(Model3DGroup group, bool leftSide)
        {
            PointLight pl = new PointLight(Color.FromRgb(255,255,0), new Point3D(.2, 6, .1));
            pl.Range = 10000;
            pl.ConstantAttenuation = 1;
            group.Children.Add(pl);
        }

        protected override Camera BuildCamera(bool leftSide, double aspect)
        {
            Point3D cameraPosition = new Point3D(0.5, 0.5, 4.5);
            Vector3D cameraDirection = new Vector3D(-0.5, -0.5, -4.5);
            Vector3D cameraUp = new Vector3D(0, 1, 0);
            PerspectiveCamera camera = new PerspectiveCamera(cameraPosition, cameraDirection, cameraUp, /* fieldOfView (degrees) = */ 40);
            return camera;
        }

        public override bool DoTest()
        {
            // Make sure hit testing works on non-indexed meshes

            if (!DoHitTest(_leftVisual, new Point(155,245)))
            {
                Console.WriteLine("Hit testing non-indexed mesh failed!");
                return false;
            }

            return true;
        }
    }

    public class TestRadialGradientBrush : TestCase
    {
        public override string ToString() { return "RadialGradient Brush"; }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            RadialGradientBrush gradientbrush = new RadialGradientBrush();

            gradientbrush.GradientStops.Add(new GradientStop(Colors.Yellow, 0.0));
            gradientbrush.GradientStops.Add(new GradientStop(Colors.Red, 1.0));

            PlaneXY plane = new PlaneXY(1, 1);

            GeometryModel3D primitive = new GeometryModel3D(plane.CreateMesh(51,51), new DiffuseMaterial(gradientbrush));

            group.Children.Add(primitive);
        }
    }

    public class TestImageBrush : TestCase
    {
        public override string ToString() { return "Image Brush"; }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            BitmapSource id = BitmapFrame.Create(new Uri(@"DrtFiles\DrtBasic3d\msnbackground.bmp", UriKind.RelativeOrAbsolute), BitmapCreateOptions.None, BitmapCacheOption.Default);
            ImageBrush ib = new ImageBrush(id);
            ib.TileMode = TileMode.Tile;

            PlaneXY plane = new PlaneXY(1, 1);

            GeometryModel3D primitive = new GeometryModel3D(plane.CreateMesh(51,51), new DiffuseMaterial(ib));

            group.Children.Add(primitive);
        }
    }

    public class TestImageBrushSoftwareOnly : TestImageBrush
    {
        public override string ToString() { return "Image Brush: RenderMode.SoftwareOnly"; }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            base.BuildGeometricScene(group, leftSide);

            if (leftSide)
            {
                RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            }
        }
    }

    public class TestImageBrushDefault : TestImageBrush
    {
        public override string ToString() { return "Image Brush: RenderMode.Default"; }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            base.BuildGeometricScene(group, leftSide);

            if (leftSide)
            {
                RenderOptions.ProcessRenderMode = RenderMode.Default;
            }
        }
    }


    public class TestHitTesting : TestCase
    {
        private Visual _orthographicTarget;
        private Visual _perspectiveTarget;

        private static readonly Point3D v1 = new Point3D(0, 0, 0);
        private static readonly Point3D v2 = new Point3D(0, 1, 0);
        private static readonly Point3D v3 = new Point3D(1, 0, 0);

        public override bool ExtraHitTesting(RayHitTestResult rayResult)
        {
            bool hit = false;

            GeometryModel3D gm3D = rayResult.ModelHit as GeometryModel3D;

            if (gm3D != null)
            {
                MeshGeometry3D meshHit = gm3D.Geometry as MeshGeometry3D;

                if (meshHit != null)
                {
                    // This is not a very interesting test.  The larger motivation
                    // is to touch all of the APIs.
                    if (meshHit.Positions.Count == 3
                        && meshHit.Positions[0] == v1
                        && meshHit.Positions[1] == v2
                        && meshHit.Positions[2] == v3)
                    {
                        hit = true;
                    }
                }
            }

            return hit;
        }

        public override bool DoTest()
        {
            _perspectiveTarget = _leftVisual;
            _orthographicTarget = _rightVisual;

            Point[] both = new Point[]
            {
                new Point(57, 231)
            };

            Point[] leftOnly = new Point[]
            {
                new Point(173, 125),
                new Point(140, 310)
            };

            Point[] rightOnly = new Point[]
            {
                new Point(54, 231),
                new Point(178, 153),
                new Point(150, 301)
            };

            Point[] neither = new Point[]
            {
                new Point(143, 300),
                new Point(70, 216),
                new Point(66, 242)
            };

            bool success = true;

            foreach(Point p in both)
            {
                success &= DoHitTest(_perspectiveTarget, p);
                success &= DoHitTest(_orthographicTarget, p);
            }

            foreach(Point p in leftOnly)
            {
                success &= DoHitTest(_perspectiveTarget, p);
                success &= !DoHitTest(_orthographicTarget, p);
            }

            foreach(Point p in rightOnly)
            {
                success &= !DoHitTest(_perspectiveTarget, p);
                success &= DoHitTest(_orthographicTarget, p);
            }

            foreach(Point p in neither)
            {
                success &= !DoHitTest(_perspectiveTarget, p);
                success &= !DoHitTest(_orthographicTarget, p);
            }

            return success;
        }

        public override string ToString() { return "Hit testing"; }

        protected override void BuildLights(Model3DGroup group, bool leftSide)
        {
            // Lights!
            DirectionalLight light = new DirectionalLight(Colors.White, new Vector3D(0,0,-1));
            group.Children.Add(light);
        }

        protected override Camera BuildCamera(bool leftSide, double aspect)
        {
            ProjectionCamera camera;

            Point3D cameraPosition = new Point3D(3, -5, 1);
            Vector3D cameraDirection = new Vector3D(-3, 5, -1);

            if (leftSide)
            {
                camera = new PerspectiveCamera(cameraPosition, cameraDirection, new Vector3D(0, 1, 0), 30);
            }
            else
            {
                camera = new OrthographicCamera(cameraPosition, cameraDirection, new Vector3D(0, 1, 0), 4);
            }

            return camera;
        }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            //
            //  Create the Mesh
            //

            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add(v1);
            mesh.Positions.Add(v2);
            mesh.Positions.Add(v3);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(0);
            GeometryModel3D primitive1 = new GeometryModel3D(mesh, new DiffuseMaterial(new SolidColorBrush(Colors.Red)));

            // Make a couple more back-facing meshes so that we're sure they don't get hit.
            MeshGeometry3D backwardsMesh = new MeshGeometry3D();
            backwardsMesh.Positions = mesh.Positions;
            backwardsMesh.TriangleIndices.Add(0);
            backwardsMesh.TriangleIndices.Add(1);
            backwardsMesh.TriangleIndices.Add(2);

            // Bigger & in front.  Recall that positive Z is in front of the triangle.
            GeometryModel3D primitive2 = new GeometryModel3D(backwardsMesh, new DiffuseMaterial(new SolidColorBrush(Colors.Yellow)));
            Transform3DGroup xf2 = new Transform3DGroup();
            xf2.Children.Add(new ScaleTransform3D(new Vector3D(5,5,5)));
            xf2.Children.Add(new TranslateTransform3D(new Vector3D(-2,-2,0.3)));
            primitive2.Transform = xf2;

            // Bigger & behind.  Recall that positive Z is in front of the triangle.
            GeometryModel3D primitive3 = new GeometryModel3D(backwardsMesh, new DiffuseMaterial(new SolidColorBrush(Colors.Blue)));
            Transform3DGroup xf3 = new Transform3DGroup();
            xf3.Children.Add(new ScaleTransform3D(new Vector3D(15,15,15)));
            xf3.Children.Add(new TranslateTransform3D(new Vector3D(-2,-2,-0.4)));
            primitive3.Transform = xf3;

            Model3DGroup meshGroup = new Model3DGroup();
            meshGroup.Children.Add(primitive1);
            meshGroup.Children.Add(primitive2);
            meshGroup.Children.Add(primitive3);

            Transform3DGroup meshGroupXF = new Transform3DGroup();
            Vector3D axis = new Vector3D(1, 0, 1);
            meshGroupXF.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(axis, 30), new Point3D(0.25, -1, 0.5)));
            meshGroup.Transform = meshGroupXF;

            //
            //  Create the Group
            //

            Model3DGroup group2 = new Model3DGroup();

            Transform3DGroup groupXForm = new Transform3DGroup();
            groupXForm.Children.Add(new ScaleTransform3D(new Vector3D(1.5, 2, 2.5), new Point3D(0.5, -0.25, 0.75)));
            groupXForm.Children.Add(new TranslateTransform3D(new Vector3D(0.25, -0.25, 0.5)));
            group2.Transform = groupXForm;
            group2.Children.Add(meshGroup);

            group.Children.Add(group2);
        }
    }

    // Verify that hit testing is clipped by the near and far planes.
    public class TestHitTestingClipped : TestCase
    {
        public override string ToString() { return "Hit testing clipped"; }

        public override bool DoTest()
        {
            Point[] leftMiss = new Point[]
            {
                new Point(90, 243),
                new Point(248, 237)
            };

            Point[] leftHit = new Point[]
            {
                new Point(168, 239),
            };

            Point[] rightMiss = new Point[]
            {
                new Point(142, 234),
                new Point(249, 233),
            };

            Point[] rightHit = new Point[]
            {
                new Point(198, 231),
            };

            bool success = true;

            foreach (Point p in leftMiss)
            {
                success &= !DoHitTest(_leftVisual, p);
            }

            foreach (Point p in leftHit)
            {
                success &= DoHitTest(_leftVisual, p);
            }

            foreach (Point p in rightMiss)
            {
                success &= !DoHitTest(_rightVisual, p);
            }

            foreach(Point p in rightHit)
            {
                success &= DoHitTest(_rightVisual, p);
            }

            return success;
        }

        protected override void BuildLights(Model3DGroup group, bool leftSide)
        {
            // Lights!
            DirectionalLight light = new DirectionalLight(Colors.White, new Vector3D(0,0,-1));
            group.Children.Add(light);
        }

        protected override Camera BuildCamera(bool leftSide, double aspect)
        {
            ProjectionCamera camera;

            Point3D cameraPosition = new Point3D(0,0,1);
            Vector3D cameraDirection = new Vector3D(0,0,-1);
            Vector3D upDirection = new Vector3D(0,1,0);

            if (leftSide)
            {
                camera = new PerspectiveCamera(cameraPosition, cameraDirection, upDirection, 90);
            }
            else
            {
                camera = new OrthographicCamera(cameraPosition, cameraDirection, upDirection, 4);
            }

            camera.NearPlaneDistance = 1;
            camera.FarPlaneDistance = 2;

            return camera;
        }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            //
            //  Create the Mesh
            //

            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add(new Point3D(-1, 0, 0.5));
            mesh.Positions.Add(new Point3D( 1, 0, -1.5));
            mesh.Positions.Add(new Point3D( 0, 1, -0.5));
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            GeometryModel3D primitive = new GeometryModel3D(mesh, new DiffuseMaterial(new SolidColorBrush(Colors.Red)));

            group.Children.Add(primitive);
        }
    }

    public abstract class AnimationTest : TestCase
    {
        public AnimationTest()
        {
            ZeroToTenAnimation = new DoubleAnimation(0, 10, new TimeSpan(0, 0, 0, 0, 1500));
            ZeroToTenAnimation.RepeatBehavior = RepeatBehavior.Forever;
            ZeroToTenAnimation.AutoReverse = true;

            ZeroToOneAnimation = new DoubleAnimation(0, 1, new TimeSpan(0, 0, 0, 0, 1000));
            ZeroToOneAnimation.RepeatBehavior = RepeatBehavior.Forever;
            ZeroToOneAnimation.AutoReverse = true;

            ZeroToThreeSixtyAnimation = new DoubleAnimation(0, 360, new TimeSpan(0, 0, 0, 0, 1000));
            ZeroToThreeSixtyAnimation.RepeatBehavior = RepeatBehavior.Forever;
            ZeroToThreeSixtyAnimation.AutoReverse = true;

            GreenAnimation = new ColorAnimation(Colors.LightGreen, Colors.DarkGreen, new TimeSpan(0, 0, 0, 0, 1000));
            GreenAnimation.RepeatBehavior = RepeatBehavior.Forever;
            GreenAnimation.AutoReverse = true;

            RedAnimation = new ColorAnimation(Colors.Pink, Colors.DarkRed, new TimeSpan(0, 0, 0, 0, 2000));
            RedAnimation.RepeatBehavior = RepeatBehavior.Forever;
            RedAnimation.AutoReverse = true;

            BlueAnimation = new ColorAnimation(Colors.LightBlue, Colors.DarkBlue, new TimeSpan(0, 0, 0, 0, 4000));
            BlueAnimation.RepeatBehavior = RepeatBehavior.Forever;
            BlueAnimation.AutoReverse = true;

            VectorAnimation = new Vector3DAnimation(new Vector3D(-1, -1, -1), new Vector3D(1, 1, 1), new TimeSpan(0, 0, 0, 0, 500));
            VectorAnimation.RepeatBehavior = RepeatBehavior.Forever;
            VectorAnimation.AutoReverse = true;

            PointAnimation = new Point3DAnimation(new Point3D(-1, 1, 1), new Point3D(1, -1, -1), new TimeSpan(0, 0, 0, 0, 1000));
            PointAnimation.RepeatBehavior = RepeatBehavior.Forever;
            PointAnimation.AutoReverse = true;

            NegativeOneToOneInHalfASecondAnimation = new DoubleAnimation(-1, 1, new TimeSpan(0, 0, 0, 0, 500));
            NegativeOneToOneInHalfASecondAnimation.RepeatBehavior = RepeatBehavior.Forever;
            NegativeOneToOneInHalfASecondAnimation.AutoReverse = true;

            NegativeOneToOneInASecondAnimation = new DoubleAnimation(-1, 1, new TimeSpan(0, 0, 0, 0, 1000));
            NegativeOneToOneInASecondAnimation.RepeatBehavior = RepeatBehavior.Forever;
            NegativeOneToOneInASecondAnimation.AutoReverse = true;

            OneToNegativeOneInASecondAnimation = new DoubleAnimation(1, -1, new TimeSpan(0, 0, 0, 0, 1000));
            OneToNegativeOneInASecondAnimation.RepeatBehavior = RepeatBehavior.Forever;
            OneToNegativeOneInASecondAnimation.AutoReverse = true;

            Vector3D v = new Vector3D(0, 1, 0);
            Rotation3DAnimation = new Rotation3DAnimation(new AxisAngleRotation3D(v, 0), new AxisAngleRotation3D(v, 180), new TimeSpan(0, 0, 0, 0, 2000));
            Rotation3DAnimation.RepeatBehavior = RepeatBehavior.Forever;
            Rotation3DAnimation.AutoReverse = true;
        }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            Torus torus = new Torus();
            GeometryModel3D primitive = new GeometryModel3D(torus.MeshGeometry3D, new DiffuseMaterial(Brushes.White));
            group.Children.Add(primitive);
        }

        protected override void BuildLights(Model3DGroup group, bool leftSide)
        {
            DirectionalLight light = new DirectionalLight(Colors.White, new Vector3D(0,0,-1));
            group.Children.Add(light);
        }

        protected override Camera BuildCamera(bool leftSide, double aspect)
        {
            ProjectionCamera camera = new PerspectiveCamera(
                new Point3D(0, 0, 1),
                new Vector3D(0, 0, -1),
                new Vector3D(0, 1, 0), 30);
            return camera;
        }

        public ColorAnimation GreenAnimation;
        public ColorAnimation RedAnimation;
        public ColorAnimation BlueAnimation;
        public Vector3DAnimation VectorAnimation;
        public Point3DAnimation PointAnimation;
        public DoubleAnimation NegativeOneToOneInHalfASecondAnimation;
        public DoubleAnimation NegativeOneToOneInASecondAnimation;
        public DoubleAnimation OneToNegativeOneInASecondAnimation;
        public DoubleAnimation ZeroToOneAnimation;
        public DoubleAnimation ZeroToTenAnimation;
        public DoubleAnimation ZeroToThreeSixtyAnimation;
        public Rotation3DAnimation Rotation3DAnimation;
    }

    public class TestMaterialAnimations : AnimationTest
    {
        public override string ToString() { return "Material Animations"; }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            SolidColorBrush scBrush = new SolidColorBrush(Colors.Red);
            scBrush.BeginAnimation(SolidColorBrush.ColorProperty, GreenAnimation);

            Torus torus = new Torus();
            GeometryModel3D primitive = new GeometryModel3D(torus.MeshGeometry3D, new DiffuseMaterial(scBrush));
            group.Children.Add(primitive);
        }
    }

    public class TestLightColorAnimations : AnimationTest
    {
        public override string ToString() { return "Light Color Animations"; }

        protected override void BuildLights(Model3DGroup group, bool leftSide)
        {
            if (leftSide)
            {
                DirectionalLight dl = new DirectionalLight(Colors.White, new Vector3D(1,0,0));
                dl.BeginAnimation(DirectionalLight.ColorProperty, GreenAnimation);
                group.Children.Add(dl);

                PointLight pl = new PointLight(Colors.White, new Point3D(0,1,0));
                pl.BeginAnimation(PointLight.ColorProperty, BlueAnimation);
                pl.Range = 10;
                pl.ConstantAttenuation = 0.25;
                group.Children.Add(pl);

                SpotLight sl = new SpotLight(Colors.White, new Point3D(0,-1,0), new Vector3D(0,1,0), 5, 10);
                sl.BeginAnimation(SpotLight.ColorProperty, RedAnimation);
                sl.Range = 10;
                sl.ConstantAttenuation = 0.25;
                sl.OuterConeAngle = 45;
                sl.InnerConeAngle = 30;
                group.Children.Add(sl);
            }
            else
            {
                AmbientLight al = new AmbientLight(Colors.White);
                al.BeginAnimation(AmbientLight.ColorProperty, GreenAnimation);
                group.Children.Add(al);
            }
        }
    }

    public class TestLightPositionAnimations : AnimationTest
    {
        public override string ToString() { return "Light Position & Direction Animations"; }

        protected override void BuildLights(Model3DGroup group, bool leftSide)
        {
            Point3DAnimation positionAnimation = new Point3DAnimation(new Point3D(-1, 0, 0), new Point3D(1, 0, 0), new TimeSpan(0, 0, 0, 0, 1000));
            positionAnimation.RepeatBehavior = RepeatBehavior.Forever;
            positionAnimation.AutoReverse = true;

            Vector3DAnimation directionAnimation = new Vector3DAnimation(new Vector3D(1, 0, 0), new Vector3D(-1, 0, 0), new TimeSpan(0, 0, 0, 0, 1000));
            directionAnimation.RepeatBehavior = RepeatBehavior.Forever;
            directionAnimation.AutoReverse = true;

            if (leftSide)
            {
                DirectionalLight dl = new DirectionalLight(Colors.Red, new Vector3D(1,0,0));
                dl.BeginAnimation(DirectionalLight.DirectionProperty, directionAnimation);

                // Watch this animated DP.
                DrtBasic3D.SetDPDO(dl,DirectionalLight.DirectionProperty);

                group.Children.Add(dl);

                PointLight pl = new PointLight(Colors.Blue, new Point3D(0,1,0));
                pl.BeginAnimation(PointLight.PositionProperty, positionAnimation);
                pl.Range = 10;
                pl.ConstantAttenuation = 0.25;
                group.Children.Add(pl);
            }
            else
            {
                SpotLight sl = new SpotLight(Colors.Green, new Point3D(-1, 0, 0), new Vector3D(1, 0, 0), 5, 10);
                sl.BeginAnimation(SpotLight.PositionProperty, positionAnimation);
                sl.BeginAnimation(SpotLight.DirectionProperty, directionAnimation);
                sl.Range = 10;
                sl.ConstantAttenuation = 0.25;
                sl.InnerConeAngle = 30;
                sl.OuterConeAngle = 45;
                group.Children.Add(sl);
            }
        }
    }

    public class TestLightConstantLinearAttenuationAnimations : AnimationTest
    {
        public override string ToString() { return "Light Constant & Linear Attenuation Animations "; }

        protected override void BuildLights(Model3DGroup group, bool leftSide)
        {
            if (leftSide)
            {
                PointLight pl = new PointLight(Colors.Blue, new Point3D(0,1,0));
                pl.ConstantAttenuation = 5;
                pl.BeginAnimation(PointLight.ConstantAttenuationProperty, ZeroToTenAnimation);
                pl.Range = 2;
                group.Children.Add(pl);
            }
            else
            {
                PointLight pl = new PointLight(Colors.Blue, new Point3D(0,1,0));
                pl.Range = 2;
                pl.LinearAttenuation = 5;
                pl.BeginAnimation(PointLight.LinearAttenuationProperty, ZeroToTenAnimation);
                group.Children.Add(pl);
            }
        }
    }

    public class TestLightQuadraticAttenuationRangeAnimations : AnimationTest
    {
        public override string ToString() { return "Light Quadratic Attenuation & Range Animations "; }

        protected override void BuildLights(Model3DGroup group, bool leftSide)
        {
            if (leftSide)
            {
                PointLight pl = new PointLight(Colors.Blue, new Point3D(0,1,0));
                pl.QuadraticAttenuation = 5;
                pl.BeginAnimation(PointLight.QuadraticAttenuationProperty, ZeroToTenAnimation);
                pl.Range = 2;
                group.Children.Add(pl);
            }
            else
            {
                PointLight pl = new PointLight(Colors.Blue, new Point3D(0,1,0));
                pl.Range = 0.5;
                pl.ConstantAttenuation = 0;
                pl.BeginAnimation(PointLight.RangeProperty, ZeroToOneAnimation);
                group.Children.Add(pl);
            }
        }
    }

    public class TestLightConeAngleAnimations : AnimationTest
    {
        public override string ToString() { return "Light Cone & Angle Animations "; }

        protected override void BuildLights(Model3DGroup group, bool leftSide)
        {
            if (leftSide)
            {
                DoubleAnimation outerConeAnimation = new DoubleAnimation(15, 45, new TimeSpan(0, 0, 0, 0, 1000));
                outerConeAnimation.RepeatBehavior = RepeatBehavior.Forever;
                outerConeAnimation.AutoReverse = true;

                SpotLight sl = new SpotLight(Colors.Green, new Point3D(-1, 0, 0), new Vector3D(1, 0, 0), 5, 10);
                sl.Range = 10;
                sl.ConstantAttenuation = 0.25;
                sl.InnerConeAngle = 15;
                sl.OuterConeAngle = 30;

                sl.BeginAnimation(SpotLight.OuterConeAngleProperty, outerConeAnimation);

                group.Children.Add(sl);
            }
            else
            {
                DoubleAnimation innerConeAnimation = new DoubleAnimation(1, 30, new TimeSpan(0, 0, 0, 0, 1000));
                innerConeAnimation.RepeatBehavior = RepeatBehavior.Forever;
                innerConeAnimation.AutoReverse = true;

                SpotLight sl = new SpotLight(Colors.Green, new Point3D(-1, 0, 0), new Vector3D(1, 0, 0), 5, 10);
                sl.Range = 10;
                sl.ConstantAttenuation = 0.25;
                sl.InnerConeAngle = 15;
                sl.OuterConeAngle = 45;

                sl.BeginAnimation(SpotLight.InnerConeAngleProperty, innerConeAnimation);

                group.Children.Add(sl);
            }
        }
    }

    public class TestTranslateScaleAnimations : AnimationTest
    {
        public override string ToString() { return "Translate & Scale Animations"; }

        protected override Camera BuildCamera(bool leftSide, double aspect)
        {
            ProjectionCamera camera = new PerspectiveCamera(
                new Point3D(0, 0, 10),
                new Vector3D(0, 0, -1),
                new Vector3D(0, 1, 0), 30);
            return camera;
        }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            if (leftSide)
            {
                Torus torus = new Torus();
                GeometryModel3D primitive = new GeometryModel3D(torus.MeshGeometry3D, new DiffuseMaterial(Brushes.White));

                TranslateTransform3D translation = new TranslateTransform3D();
                translation.BeginAnimation(TranslateTransform3D.OffsetXProperty, NegativeOneToOneInHalfASecondAnimation);
                translation.BeginAnimation(TranslateTransform3D.OffsetYProperty, NegativeOneToOneInHalfASecondAnimation);
                translation.BeginAnimation(TranslateTransform3D.OffsetZProperty, NegativeOneToOneInHalfASecondAnimation);
                primitive.Transform = translation;

                group.Children.Add(primitive);
            }
            else
            {
                Torus torus = new Torus();
                GeometryModel3D primitive = new GeometryModel3D(torus.MeshGeometry3D, new DiffuseMaterial(Brushes.White));

                ScaleTransform3D scale = new ScaleTransform3D();
                scale.BeginAnimation(ScaleTransform3D.ScaleXProperty, NegativeOneToOneInHalfASecondAnimation);
                scale.BeginAnimation(ScaleTransform3D.ScaleYProperty, NegativeOneToOneInHalfASecondAnimation);
                scale.BeginAnimation(ScaleTransform3D.ScaleZProperty, NegativeOneToOneInHalfASecondAnimation);
                scale.BeginAnimation(ScaleTransform3D.CenterXProperty, NegativeOneToOneInASecondAnimation);
                scale.BeginAnimation(ScaleTransform3D.CenterYProperty, OneToNegativeOneInASecondAnimation);
                scale.BeginAnimation(ScaleTransform3D.CenterZProperty, OneToNegativeOneInASecondAnimation);
                primitive.Transform = scale;

                group.Children.Add(primitive);
            }
        }
    }

    public class TestAngleVsQuaternion : AnimationTest
    {
        public override string ToString() { return "Angle vs. Quaternion Animation"; }

        protected override Camera BuildCamera(bool leftSide, double aspect)
        {
            ProjectionCamera camera = new PerspectiveCamera(
                new Point3D(0, 0, 10),
                new Vector3D(0, 0, -1),
                new Vector3D(0, 1, 0), 30);
            return camera;
        }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            Torus torus = new Torus();
            GeometryModel3D primitive = new GeometryModel3D(torus.MeshGeometry3D, new DiffuseMaterial(Brushes.White));
            RotateTransform3D rotate = new RotateTransform3D();
            primitive.Transform = rotate;
            TimeSpan span = new TimeSpan(0, 0, 0, 0, 1500);

            if (leftSide)
            {
                QuaternionAnimation animation = new QuaternionAnimation(
                    new Quaternion(new Vector3D(0,1,0), -90),
                    new Quaternion(new Vector3D(0,1,0), 90),
                    span);
                animation.RepeatBehavior = RepeatBehavior.Forever;
                animation.AutoReverse = true;

                QuaternionRotation3D rotation = new QuaternionRotation3D();
                rotation.BeginAnimation(QuaternionRotation3D.QuaternionProperty, animation);

                rotate.Rotation = rotation;
            }
            else
            {
                DoubleAnimation animation = new DoubleAnimation(-90, 90, span);
                animation.RepeatBehavior = RepeatBehavior.Forever;
                animation.AutoReverse = true;

                AxisAngleRotation3D rotation = new AxisAngleRotation3D();
                rotation.Axis = new Vector3D(0, 1, 0);
                rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);

                rotate.Rotation = rotation;
            }

            group.Children.Add(primitive);
        }
    }

    public class TestQuaternionUseShortestPath : AnimationTest
    {
        public override string ToString() { return "Quaternion UseShortestPath"; }

        protected override Camera BuildCamera(bool leftSide, double aspect)
        {
            ProjectionCamera camera = new PerspectiveCamera(
                new Point3D(0, 0, 10),
                new Vector3D(0, 0, -1),
                new Vector3D(0, 1, 0), 30);
            return camera;
        }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            Torus torus = new Torus();
            GeometryModel3D primitive = new GeometryModel3D(torus.MeshGeometry3D, new DiffuseMaterial(Brushes.White));
            RotateTransform3D rotate = new RotateTransform3D();
            primitive.Transform = rotate;
            TimeSpan span = new TimeSpan(0, 0, 0, 0, 1500);

            Quaternion q0 = new Quaternion(new Vector3D(0,1,0), -30);
            Quaternion q1 = new Quaternion(new Vector3D(0,1,0), 30);

            q1 = new Quaternion(-q1.X, -q1.Y, -q1.Z, -q1.W);

            QuaternionAnimation animation = new QuaternionAnimation(q0, q1, span);
            animation.RepeatBehavior = RepeatBehavior.Forever;
            animation.AutoReverse = true;
            
            if (!leftSide)
            {
                animation.UseShortestPath = false;
            }

            QuaternionRotation3D rotation = new QuaternionRotation3D();
            rotation.BeginAnimation(QuaternionRotation3D.QuaternionProperty, animation);
            rotate.Rotation = rotation;
            group.Children.Add(primitive);
        }
    }
    
    public class TestRotateGroupAnimations : AnimationTest
    {
        public override string ToString() { return "Rotate & Transform3DGroup Animations"; }

        protected override Camera BuildCamera(bool leftSide, double aspect)
        {
            ProjectionCamera camera = new PerspectiveCamera(
                new Point3D(0, 0, 10),
                new Vector3D(0, 0, -1),
                new Vector3D(0, 1, 0), 30);
            return camera;
        }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            if (leftSide)
            {
                Torus torus = new Torus();
                GeometryModel3D primitive = new GeometryModel3D(torus.MeshGeometry3D, new DiffuseMaterial(Brushes.White));

                Rotation3D begin = new QuaternionRotation3D(new Quaternion(new Vector3D(0,1,0), -80));
                Rotation3D end = new AxisAngleRotation3D(new Vector3D(0,1,0), 80);
                
                Rotation3DAnimation animation = new Rotation3DAnimation(begin, end, new TimeSpan(0, 0, 0, 0, 1500));
                animation.RepeatBehavior = RepeatBehavior.Forever;
                animation.AutoReverse = true;

                RotateTransform3D rotate = new RotateTransform3D();
                rotate.Rotation = Rotation3D.Identity;
                primitive.Transform = rotate;

                rotate.BeginAnimation(RotateTransform3D.RotationProperty, animation);
                rotate.BeginAnimation(RotateTransform3D.CenterXProperty, NegativeOneToOneInASecondAnimation);
                rotate.BeginAnimation(RotateTransform3D.CenterYProperty, OneToNegativeOneInASecondAnimation);
                rotate.BeginAnimation(RotateTransform3D.CenterZProperty, OneToNegativeOneInASecondAnimation);
                primitive.Transform = rotate;
                group.Children.Add(primitive);
            }
            else
            {
                Torus torus = new Torus();
                GeometryModel3D primitive = new GeometryModel3D(torus.MeshGeometry3D, new DiffuseMaterial(Brushes.White));
                Transform3DGroup transformGroup = new Transform3DGroup();

                TranslateTransform3D translation = new TranslateTransform3D();
                translation.BeginAnimation(TranslateTransform3D.OffsetXProperty, NegativeOneToOneInHalfASecondAnimation);
                translation.BeginAnimation(TranslateTransform3D.OffsetYProperty, NegativeOneToOneInHalfASecondAnimation);
                translation.BeginAnimation(TranslateTransform3D.OffsetZProperty, NegativeOneToOneInHalfASecondAnimation);
                transformGroup.Children.Add(translation);

                ScaleTransform3D scale = new ScaleTransform3D();
                scale.BeginAnimation(ScaleTransform3D.ScaleXProperty, NegativeOneToOneInHalfASecondAnimation);
                scale.BeginAnimation(ScaleTransform3D.ScaleYProperty, NegativeOneToOneInHalfASecondAnimation);
                scale.BeginAnimation(ScaleTransform3D.ScaleZProperty, NegativeOneToOneInHalfASecondAnimation);
                scale.BeginAnimation(ScaleTransform3D.CenterXProperty, NegativeOneToOneInASecondAnimation);
                scale.BeginAnimation(ScaleTransform3D.CenterYProperty, OneToNegativeOneInASecondAnimation);
                scale.BeginAnimation(ScaleTransform3D.CenterZProperty, OneToNegativeOneInASecondAnimation);
                transformGroup.Children.Add(scale);

                RotateTransform3D rotation = new RotateTransform3D();
                rotation.BeginAnimation(RotateTransform3D.CenterXProperty, NegativeOneToOneInASecondAnimation);
                rotation.BeginAnimation(RotateTransform3D.CenterYProperty, OneToNegativeOneInASecondAnimation);
                rotation.BeginAnimation(RotateTransform3D.CenterZProperty, OneToNegativeOneInASecondAnimation);
                transformGroup.Children.Add(rotation);

                primitive.Transform = transformGroup;
                group.Children.Add(primitive);
            }
        }
    }

    public class TestRotationAnimationComposition : AnimationTest
    {
        // Do same thing with quaternion animation & rotation3d animation
        public override string ToString() { return "AxisAngleRotation3D Test 2"; }

        protected override Camera BuildCamera(bool leftSide, double aspect)
        {
            ProjectionCamera camera = new PerspectiveCamera(
                new Point3D(0, 0, 10),
                new Vector3D(0, 0, -1),
                new Vector3D(0, 1, 0), 30);
            return camera;
        }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            Quaternion a = new Quaternion(1,2,3,4);
            Quaternion b = new Quaternion(4,3,2,1);
            // C is important.
            Quaternion c = new Quaternion(new Vector3D(1,1,1), 90);
            Quaternion d = new Quaternion(1,2,3,434);

            if (leftSide)
            {
                Torus torus = new Torus();
                GeometryModel3D primitive = new GeometryModel3D(torus.MeshGeometry3D, new DiffuseMaterial(Brushes.White));
                Transform3DGroup transformGroup = new Transform3DGroup();

                Quaternion q1 = a*c;
                Quaternion q2 = b*d;

                // 
                Rotation3DAnimation rAnim1 = new Rotation3DAnimation(new AxisAngleRotation3D(q1.Axis, q1.Angle), new AxisAngleRotation3D(q2.Axis, q2.Angle), new TimeSpan(0, 0, 0, 0, 6000));
                rAnim1.RepeatBehavior = RepeatBehavior.Forever;
                rAnim1.IsCumulative = false;
                rAnim1.IsAdditive = true;

                RotateTransform3D rotation = new RotateTransform3D();
                rotation.Rotation = new AxisAngleRotation3D(new Vector3D(1,1,1), 45);
                rotation.BeginAnimation(RotateTransform3D.RotationProperty, rAnim1);
                transformGroup.Children.Add(rotation);

                primitive.Transform = transformGroup;
                group.Children.Add(primitive);
            }
            else
            {
                Torus torus = new Torus();
                GeometryModel3D primitive = new GeometryModel3D(torus.MeshGeometry3D, new DiffuseMaterial(Brushes.White));
                Transform3DGroup transformGroup = new Transform3DGroup();

                // 
                Rotation3DAnimation rAnim1 = new Rotation3DAnimation(new AxisAngleRotation3D(a.Axis, a.Angle), new AxisAngleRotation3D(b.Axis, b.Angle), new TimeSpan(0, 0, 0, 0, 6000));
                rAnim1.RepeatBehavior = RepeatBehavior.Forever;
                rAnim1.IsCumulative = false;
                rAnim1.IsAdditive = true;

                // 
                Rotation3DAnimation rAnim2 = new Rotation3DAnimation(new AxisAngleRotation3D(c.Axis, c.Angle), new AxisAngleRotation3D(d.Axis, d.Angle), new TimeSpan(0, 0, 0, 0, 6000));
                rAnim2.RepeatBehavior = RepeatBehavior.Forever;
                rAnim2.IsCumulative = false;
                rAnim2.IsAdditive = true;

                RotateTransform3D rotation = new RotateTransform3D();
                rotation.Rotation = new AxisAngleRotation3D(new Vector3D(1,1,1), 45);
                rotation.BeginAnimation(RotateTransform3D.RotationProperty, rAnim1);
                rotation.BeginAnimation(RotateTransform3D.RotationProperty, rAnim2);
                transformGroup.Children.Add(rotation);

                primitive.Transform = transformGroup;
                group.Children.Add(primitive);
            }
        }
    }

    public class TestCameraTransformAnimations : AnimationTest
    {
        public override string ToString() { return "Camera Transform Animations"; }

        protected override Camera BuildCamera(bool leftSide, double aspect)
        {
            if (leftSide)
            {
                PerspectiveCamera camera = new PerspectiveCamera(
                    new Point3D(0, 0, 1),
                    new Vector3D(0, 0, -1),
                    new Vector3D(0, 1, 0), 45);

                AxisAngleRotation3D rotation = new AxisAngleRotation3D();
                rotation.Axis = new Vector3D(0,1,0);
                rotation.Angle = -720;
                rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, ZeroToThreeSixtyAnimation);

                RotateTransform3D rotationXform = new RotateTransform3D();
                rotationXform.Rotation = rotation;
                camera.Transform = rotationXform;

                return camera;
            }
            else
            {
                OrthographicCamera camera = new OrthographicCamera(
                    new Point3D(0, 0, 1),
                    new Vector3D(0, 0, -1),
                    new Vector3D(0, 1, 0),
                    1);

                AxisAngleRotation3D rotation = new AxisAngleRotation3D();
                rotation.Axis = new Vector3D(1,0,0);
                rotation.Angle = -720;
                rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, ZeroToThreeSixtyAnimation);

                RotateTransform3D rotationXform = new RotateTransform3D();
                rotationXform.Rotation = rotation;
                camera.Transform = rotationXform;

                return camera;
            }
        }
    }

    public class TestCameraPositionUpAnimations : AnimationTest
    {
        public override string ToString() { return "Camera Position & Up Animations"; }

        protected override Camera BuildCamera(bool leftSide, double aspect)
        {
            Point3DAnimation positionAnimation = new Point3DAnimation(new Point3D(-1, 0, 1), new Point3D(1, 0, 1), new TimeSpan(0, 0, 0, 0, 1000));
            positionAnimation.RepeatBehavior = RepeatBehavior.Forever;
            positionAnimation.AutoReverse = true;

            Vector3DAnimation upAnimation = new Vector3DAnimation(new Vector3D(0, 1, 0), new Vector3D(1, 0, 0), new TimeSpan(0, 0, 0, 0, 2000));
            upAnimation.RepeatBehavior = RepeatBehavior.Forever;
            upAnimation.AutoReverse = true;

            if (leftSide)
            {
                PerspectiveCamera camera = new PerspectiveCamera(
                    new Point3D(0, 0, 1),
                    new Vector3D(0, 0, -1),
                    new Vector3D(0, 1, 0), 45);

                camera.BeginAnimation(PerspectiveCamera.PositionProperty, positionAnimation);
                camera.BeginAnimation(PerspectiveCamera.UpDirectionProperty, upAnimation);

                return camera;
            }
            else
            {
                OrthographicCamera camera = new OrthographicCamera(
                    new Point3D(0, 0, 1),
                    new Vector3D(0, 0, -1),
                    new Vector3D(0, 1, 0),
                    1);

                camera.BeginAnimation(OrthographicCamera.PositionProperty, positionAnimation);
                camera.BeginAnimation(OrthographicCamera.UpDirectionProperty, upAnimation);

                return camera;
            }
        }
    }

    public class TestCameraLookAtAnimations : AnimationTest
    {
        public override string ToString() { return "Camera LookAt Animations"; }

        protected override Camera BuildCamera(bool leftSide, double aspect)
        {
            Vector3DAnimation lookDirectionAnimation = new Vector3DAnimation(new Vector3D(-0.5, 0, -1), new Vector3D(0.5, 0, -1), new TimeSpan(0, 0, 0, 0, 1000));
            lookDirectionAnimation.RepeatBehavior = RepeatBehavior.Forever;
            lookDirectionAnimation.AutoReverse = true;

            if (leftSide)
            {
                PerspectiveCamera camera = new PerspectiveCamera(
                    new Point3D(0, 0, 1),
                    // The Animation will ignore this base value but we will look for it in our test
                    _lookDirectionBase,
                    new Vector3D(0, 1, 0), 45);
                // Stash this camera so that we can run some tests on its animated property
                _perspectiveCamera = camera;

                camera.BeginAnimation(PerspectiveCamera.LookDirectionProperty, lookDirectionAnimation);


                return camera;
            }
            else
            {
                OrthographicCamera camera = new OrthographicCamera(
                    new Point3D(0, 0, 1),
                    new Vector3D(0, 0, -1),
                    new Vector3D(0, 1, 0),
                    1);

                camera.BeginAnimation(OrthographicCamera.LookDirectionProperty, lookDirectionAnimation);

                return camera;
            }
        }

        public override bool DoTest()
        {
            bool succeeded = true;

            // Grab the default value for tests.
            object defaultLookDirection = ProjectionCamera.LookDirectionProperty.GetMetadata(typeof(PerspectiveCamera)).DefaultValue;

            PerspectiveCamera currentValueCopy = _perspectiveCamera.CloneCurrentValue();
            // Current value should be neither base value nor default
            Vector3D currentLookDirection = currentValueCopy.LookDirection;
            if (currentLookDirection == _lookDirectionBase || defaultLookDirection.Equals(currentLookDirection))
            {
                Console.WriteLine( "Failure!  LookAtPoint on CurrentValue copy should be animated not default or base!" );
                Console.WriteLine( "Result was " + currentLookDirection );
                succeeded = false;
            }

            // Strip animations.  We should get base value back not default, for example.
            _perspectiveCamera.BeginAnimation(ProjectionCamera.LookDirectionProperty, null);
            if (_perspectiveCamera.LookDirection != _lookDirectionBase)
            {
                Console.WriteLine( "Failure!  LookDirection on PerspectiveCamera without animation should be base value!" );
                Console.WriteLine( "Result was " + _perspectiveCamera.LookDirection );
                Console.WriteLine( "Expected " + _lookDirectionBase );
                succeeded = false;
            }

            return succeeded;
        }

        Vector3D _lookDirectionBase = new Vector3D(0, -1000, 0); // Base value ignored by animation but used in our test.
        private PerspectiveCamera _perspectiveCamera;
    }

    public class TestCameraNegativeNearPlane : TestCase
    {
        public override string ToString() { return "Camera Negative NearPlane"; }

        protected override Camera BuildCamera(bool leftSide, double aspect)
        {
            // Set up our camera ahead of our geometry (roughly at (0,0,0)) looking
            // in the wrong direction.
            OrthographicCamera camera = new OrthographicCamera(
                new Point3D(0, 0, -1),
                new Vector3D(0, 0, -1),
                new Vector3D(0, 1, 0),
                2);

            // Allow the near plane to auto-range behind the camera to see our geometry.
            camera.NearPlaneDistance = Double.NegativeInfinity;

            return camera;
        }

        public override bool DoTest()
        {
            bool success = true;

            success &= DoHitTest(_leftVisual, new Point(240,262));

            return success;
        }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            BitmapSource id = BitmapFrame.Create(new Uri(@"DrtFiles\DrtBasic3d\msnbackground.bmp", UriKind.RelativeOrAbsolute), BitmapCreateOptions.None, BitmapCacheOption.Default);
            ImageBrush ib = new ImageBrush(id);
            ib.TileMode = TileMode.Tile;

            PlaneXY plane = new PlaneXY(1, 1);

            GeometryModel3D primitive = new GeometryModel3D(plane.CreateMesh(51,51), new DiffuseMaterial(ib));
            group.Children.Add(primitive);

            GeometryModel3D redPrimitive = new GeometryModel3D(plane.CreateMesh(51,51), new DiffuseMaterial(Brushes.Red));
            redPrimitive.Transform = new TranslateTransform3D(new Vector3D(.1, .1, -.1));
            group.Children.Add(redPrimitive);

            GeometryModel3D bluePrimitive = new GeometryModel3D(plane.CreateMesh(51,51), new DiffuseMaterial(Brushes.Blue));
            bluePrimitive.Transform = new TranslateTransform3D(new Vector3D(.2, .2, -.2));
            group.Children.Add(bluePrimitive);

            if (!leftSide)
            {
                GeometryModel3D primitive2 = primitive.Clone();
                primitive2.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 170));
                group.Children.Add(primitive2);
            }
        }
    }


    public class TestFlatSpheres : AnimationTest
    {
        public override string ToString() { return "Flattened spheres"; }
        public override bool DoRoundtrip() { return false; }

        protected override Camera BuildCamera(bool leftSide, double aspect)
        {
            ProjectionCamera camera = new PerspectiveCamera(
                new Point3D(0, 0, 10),
                new Vector3D(0, 0, -1),
                new Vector3D(0, 1, 0), 30);
            return camera;
        }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            Sphere smallSphere = new Sphere(0.3);
            MeshGeometry3D sphereMesh = smallSphere.CreateMesh(11, 11);
            DiffuseMaterial sphereMaterial = new DiffuseMaterial(Brushes.Blue);
            Model3DGroup sphereGroup = new Model3DGroup();
    
            int count = 0;

            for (double x = -1.0; x < 1.0; x += 0.6)
            {
                for (double y = -1.0; y < 1.0; y += 0.6)
                {
                    if (count % 2 != 0)
                    {
                        Transform3DGroup transformGroup = new Transform3DGroup();
                        if (count % 3 == 0)
                        {
                            transformGroup.Children.Add(new ScaleTransform3D(new Vector3D(0, 1, 1)));
                        }

                        TranslateTransform3D translateTransform = new TranslateTransform3D(new Vector3D(x, y, 0));
                        transformGroup.Children.Add(translateTransform);
                        RotateTransform3D rotateTransform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 2, 3), 33));
                        transformGroup.Children.Add(rotateTransform);

                        GeometryModel3D geoModel = new GeometryModel3D(sphereMesh, sphereMaterial);
                        geoModel.Transform = transformGroup;
                        sphereGroup.Children.Add(geoModel);
                    }
                    ++count;
                }
            }

            group.Children.Add(sphereGroup);
        }

        public override bool DoTest()
        {
            // Test some hits into a model underneath a singular transform.  We'll only
            // use the left side.

            Point[] hits = { new Point(222,179),
                             new Point(210,264),
                             new Point(112,251)
            };

            bool success = true;

            success &= DoHitTest(_leftVisual, hits, hits.Length);

            return success;
        }
    }

    public class TestSmallVertexSmallIndexPrimitives : AnimationTest
    {
        public override string ToString() { return "Small Vertex Small Index Count Limits"; }
        public override bool DoRoundtrip() { return false; }

        protected override Camera BuildCamera(bool leftSide, double aspect)
        {
            ProjectionCamera camera = new PerspectiveCamera(
                new Point3D(0, 0, 10),
                new Vector3D(0, 0, -1),
                new Vector3D(0, 1, 0), 30);
            return camera;
        }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            Sphere smallSphere = new Sphere(0.1);

            //
            // Creating the mesh at 51 x 51 precision will produce spheres that contain
            // 50 x 50 x 2 triangles, and 51 x 51 vertices.
            //
            // That means 15000 indices and 2601 vertices.  Since our vertex buffer
            // size is 20001 and our index buffer size is 60003, drawing more
            // than 10 of these will keep each one inside our vertex/index buffer
            // limits, and force at least a single flush of both the vertex and
            // index buffer.
            //
            GeometryModel3D primitive = new GeometryModel3D(smallSphere.CreateMesh(51, 51), new DiffuseMaterial(Brushes.Blue));

            for (double x = -1.0; x < 1.0; x += 0.3)
            {
                for (double y = -1.0; y < 1.0; y += 0.3)
                {
                    TranslateTransform3D transform = new TranslateTransform3D(new Vector3D(x,y,0));

                    primitive.Transform = transform;

                    group.Children.Add(primitive.Clone());
                }
            }
        }
    }

    public class TestSmallVertexLargeIndexPrimitives : AnimationTest
    {
        public override string ToString() { return "Small Vertex Large Index Count Limits"; }
        public override bool DoRoundtrip() { return false; }

        protected override Camera BuildCamera(bool leftSide, double aspect)
        {
            ProjectionCamera camera = new PerspectiveCamera(
                new Point3D(0, 0, 10),
                new Vector3D(0, 0, -1),
                new Vector3D(0, 1, 0), 30);
            return camera;
        }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            Sphere shape = new Sphere(1.0);

            //
            // Creating the mesh at 131 x 131 precision will produce spheres that contain
            // 130 x 130 x 2 triangles, and 131 x 131 vertices.
            //
            // That means 101400 indices and 17161 vertices.  Since our vertex buffer
            // size is 20001 and our index buffer size is 60003, each drawing of
            // these will stay inside our vertex buffer limit while requiring
            // several iterations through the index buffer.  Rendering more than
            // 2 will guarantee that we flusht the vertex buffer at least once.
            //
            GeometryModel3D primitive = new GeometryModel3D(shape.CreateMesh(131, 131), new DiffuseMaterial(Brushes.Blue));

            for (double x = -1.0; x <= 1.0; x += 2.0)
            {
                for (double y = -1.0; y <= 1.0; y += 2.0)
                {
                    TranslateTransform3D transform = new TranslateTransform3D(new Vector3D(x,y,0));

                    primitive.Transform = transform;

                    group.Children.Add(primitive.Clone());
                }
            }
        }
    }

    public class TestLargeVertexLargeIndexPrimitives : AnimationTest
    {
        public override string ToString() { return "Large Vertex Large Index Count Limits"; }
        public override bool DoRoundtrip() { return false; }

        protected override Camera BuildCamera(bool leftSide, double aspect)
        {
            ProjectionCamera camera = new PerspectiveCamera(
                new Point3D(0, 0, 10),
                new Vector3D(0, 0, -1),
                new Vector3D(0, 1, 0), 30);
            return camera;
        }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            Sphere shape = new Sphere(2.0);
            //
            // Creating the mesh at 201 x 201 precision will produce spheres that contain
            // 200 x 200 x 2 triangles, and 201 x 201 vertices.
            //
            // That means 240000 indices and 40401 vertices.  Since our vertex
            // buffer size is 20001 and our index buffer size is 60003, each
            // drawing of these will break our vertex buffer limit.  This draw
            // will force us to continuously fill the vertex buffer and flush
            // without even using an index buffer.
            //
            GeometryModel3D primitive = new GeometryModel3D(shape.CreateMesh(201, 201), new DiffuseMaterial(Brushes.Blue));

            group.Children.Add(primitive);
        }
    }

    public class TestLargeVertexNonIndexedPrimitive : AnimationTest
    {
        public override string ToString() { return "Large Vertex Non Indexed Limit"; }
        public override bool DoRoundtrip() { return false; }

        protected override Camera BuildCamera(bool leftSide, double aspect)
        {
            ProjectionCamera camera = new PerspectiveCamera(
                new Point3D(0, 0, 10),
                new Vector3D(0, 0, -1),
                new Vector3D(0, 1, 0), 30);
            return camera;
        }

        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide)
        {
            Sphere shape = new Sphere(2.0);
            //
            // Creating the mesh at 201 x 201 precision will produce spheres that contain
            // 200 x 200 x 2 triangles, and 201 x 201 vertices.
            //
            // That means 240000 indices which we will turn into 240000 vertices
            MeshGeometry3D sphereMesh = shape.CreateMesh(201, 201);

            // convert to non-indexed mesh
            Point3DCollection newVertices = new Point3DCollection();
            Vector3DCollection newNormals = new Vector3DCollection();
            foreach (int index in sphereMesh.TriangleIndices)
            {
                newVertices.Add(sphereMesh.Positions[index]);
                newNormals.Add(sphereMesh.Normals[index]);
            }
            sphereMesh.Positions = newVertices;
            sphereMesh.Normals = newNormals;
            sphereMesh.TriangleIndices.Clear();

            GeometryModel3D primitive = new GeometryModel3D(sphereMesh, new DiffuseMaterial(Brushes.Blue));

            group.Children.Add(primitive);
        }
    }

    public class TestMath : TestCase
    {
        public override string ToString() { return "Math"; }
        public override bool DoRoundtrip() { return false; }

        Matrix3D _prime1 = new Matrix3D(
             3,  5,  7, 11,
            13, 17, 19, 23,
            29, 31, 37, 41,
            43, 47, 53, 59);

        Matrix3D _prime2 = new Matrix3D(
             61,  67,  71,  73,
             79,  83,  97, 101,
            103, 107, 109, 113,
            127, 131, 137, 139);

        Quaternion _q = new Quaternion(new Vector3D(.1610484381, .3757796889, .912678161), 29);
        Point3D _c = new Point3D(5, 13, 19);
        Vector3D _s = new Vector3D(3, 7, 11);

        public override bool DoTest()
        {
            bool success = true;

            success &= CheckVector3D();
            success &= CheckQuaternions();

            success &= CheckMultiply(_prime1, _prime2);
            success &= CheckMultiply(new Matrix3D(), _prime2);
            success &= CheckMultiply(_prime1, new Matrix3D());

            success &= CheckAppend(_prime1, _prime2);
            success &= CheckPrepend(_prime1, _prime2);

            success &= CheckRotate(new Matrix3D(), _q, _c);
            success &= CheckRotate(_prime1, _q, _c);

            success &= CheckScale(new Matrix3D(), _s, _c);
            success &= CheckScale(_prime1, _s, _c);

            success &= CheckTranslate(new Matrix3D(), _s);
            success &= CheckTranslate(_prime1, _s);

            success &= CheckTransforms();

            success &= CheckDeterminant(_prime1);
            success &= CheckDeterminant(_prime2);

            success &= CheckInverse(_prime1);
            success &= CheckInverse(_prime2);

            return success;
        }

        private bool CheckVector3D()
        {
            bool success = true;

            for (double theta1 = 0; theta1 < 2*Math.PI; theta1 += Math.PI/12.0) 
            {
                for (double theta2 = 0; theta2 < 2*Math.PI; theta2 += Math.PI/12.0) 
                {
                    Vector3D u = new Vector3D(Math.Cos(theta1), Math.Sin(theta1), 0);
                    Vector3D v = new Vector3D(Math.Cos(theta2), Math.Sin(theta2), 0);

                    success &= CheckAngleBetween(u, v);
                }
            }

            return success;
        }

        private bool CheckAngleBetween(Vector3D u, Vector3D v)
        {
            bool success = true;

            double expected = Vector3D.AngleBetween(u, v);
            
            u.Normalize();
            v.Normalize();

            double ratio = Vector3D.DotProduct(u, v);
            
            // Clip ratio to avoid exception from Acos.
            if (ratio > 1.0)
                ratio =  1.0;
            if (ratio < -1.0)
                ratio = -1.0;

            double lowPrecision = Math.Acos(ratio) * (180.0 / Math.PI);
            success &= VerifyLowPrecision(expected, lowPrecision);

            double highPrecision = 2.0 * Math.Atan2((u - v).Length, (u + v).Length) * (180.0 / Math.PI);
            success &= Verify(expected, highPrecision);

            return success;
        }

        private bool CheckQuaternions()
        {
            bool success = true;

            Quaternion q1 = new Quaternion( 0,0,0,1 );
            Quaternion q2 = new Quaternion();

            success &= Verify(q1.Axis, q2.Axis);

            q1 = new Quaternion(new Vector3D(0,1,0), -90);
            q2 = new Quaternion();
            q2.W = Math.Sqrt(2) / 2;
            q2.Y = -q2.W;

            success &= Verify(q1.Axis, q2.Axis);
            success &= Verify(q1.Angle, q2.Angle);
            
            return success;
        }

        private bool CheckTransforms()
        {
            bool success = true;

            RotateTransform3D rotate = new RotateTransform3D(new AxisAngleRotation3D(_q.Axis, _q.Angle), _c);
            Matrix3D rotateM = GetRotationMatrix(_q, _c);
            success &= Verify(rotate.Value, rotateM);

            ScaleTransform3D scale = new ScaleTransform3D(_s, _c);
            Matrix3D scaleM = GetScaleMatrix(_s, _c);
            success &= Verify(scale.Value, scaleM);

            TranslateTransform3D translate = new TranslateTransform3D(_s);
            Matrix3D translateM = GetTranslationMatrix(_s);
            success &= Verify(translate.Value, translateM);

            Transform3DGroup group = new Transform3DGroup();
            group.Children.Add(rotate);
            group.Children.Add(scale);
            group.Children.Add(translate);
            success &= Verify(group.Value, rotateM * scaleM * translateM);

            return success;
        }

        private bool CheckRotate(Matrix3D m1, Quaternion q, Point3D c)
        {
            bool success = true;

            Matrix3D m = m1;
            m.Rotate(q);
            success &= Verify(m, m1 * GetRotationMatrix(q, new Point3D()));

            m = m1;
            m.RotateAt(q, c);
            success &= Verify(m, m1 * GetRotationMatrix(q, c));

            m = m1;
            m.RotatePrepend(q);
            success &= Verify(m, GetRotationMatrix(q, new Point3D()) * m1);

            m = m1;
            m.RotateAtPrepend(q, c);
            success &= Verify(m, GetRotationMatrix(q, c) * m1);

            return success;
        }

        private bool CheckScale(Matrix3D m1, Vector3D s, Point3D c)
        {
            bool success = true;

            Matrix3D m = m1;
            m.Scale(s);
            success &= Verify(m, m1 * GetScaleMatrix(s, new Point3D()));

            m = m1;
            m.ScaleAt(s, c);
            success &= Verify(m, m1 * GetScaleMatrix(s, c));

            m = m1;
            m.ScalePrepend(s);
            success &= Verify(m, GetScaleMatrix(s, new Point3D()) * m1);

            m = m1;
            m.ScaleAtPrepend(s, c);
            success &= Verify(m, GetScaleMatrix(s, c) * m1);

            return success;
        }

        private bool CheckTranslate(Matrix3D m1, Vector3D t)
        {
            bool success = true;

            Matrix3D m = m1;
            m.Translate(t);
            success &= Verify(m, m1 * GetTranslationMatrix(t));

            m = m1;
            m.TranslatePrepend(t);
            success &= Verify(m, GetTranslationMatrix(t) * m1);

            return success;
        }

        private bool CheckMultiply(Matrix3D m1, Matrix3D m2)
        {
           return Verify(m1 * m2, GetMultipliedMatrix(m1, m2));
        }

        private bool CheckAppend(Matrix3D m1, Matrix3D m2)
        {
            Matrix3D m = m1;
            m.Append(m2);

            return Verify(m, GetMultipliedMatrix(m1, m2));
        }

        private bool CheckPrepend(Matrix3D m1, Matrix3D m2)
        {
            Matrix3D m = m1;
            m.Prepend(m2);

            return Verify(m, GetMultipliedMatrix(m2, m1));
        }

        private bool CheckDeterminant(Matrix3D matrix)
        {
            double actual = matrix.Determinant;

            // Copy & Pasted from Maple V worksheet
            double expected =
                matrix.M11*matrix.M22*matrix.M33*matrix.M44
                - matrix.M11*matrix.M22*matrix.M34*matrix.OffsetZ
                - matrix.M11*matrix.M32*matrix.M23*matrix.M44
                + matrix.M11*matrix.M32*matrix.M24*matrix.OffsetZ
                + matrix.M11*matrix.OffsetY*matrix.M23*matrix.M34
                - matrix.M11*matrix.OffsetY*matrix.M24*matrix.M33
                - matrix.M21*matrix.M12*matrix.M33*matrix.M44
                + matrix.M21*matrix.M12*matrix.M34*matrix.OffsetZ
                + matrix.M21*matrix.M32*matrix.M13*matrix.M44
                - matrix.M21*matrix.M32*matrix.M14*matrix.OffsetZ
                - matrix.M21*matrix.OffsetY*matrix.M13*matrix.M34
                + matrix.M21*matrix.OffsetY*matrix.M14*matrix.M33
                + matrix.M31*matrix.M12*matrix.M23*matrix.M44
                - matrix.M31*matrix.M12*matrix.M24*matrix.OffsetZ
                - matrix.M31*matrix.M22*matrix.M13*matrix.M44
                + matrix.M31*matrix.M22*matrix.M14*matrix.OffsetZ
                + matrix.M31*matrix.OffsetY*matrix.M13*matrix.M24
                - matrix.M31*matrix.OffsetY*matrix.M14*matrix.M23
                - matrix.OffsetX*matrix.M12*matrix.M23*matrix.M34
                + matrix.OffsetX*matrix.M12*matrix.M24*matrix.M33
                + matrix.OffsetX*matrix.M22*matrix.M13*matrix.M34
                - matrix.OffsetX*matrix.M22*matrix.M14*matrix.M33
                - matrix.OffsetX*matrix.M32*matrix.M13*matrix.M24
                + matrix.OffsetX*matrix.M32*matrix.M14*matrix.M23;

            return Verify(expected, actual);
        }

        private bool CheckInverse(Matrix3D matrix)
        {
            Matrix3D actual = matrix;
            actual.Invert();

            // Copy & Pasted from Maple V worksheet
            Matrix3D expected = new Matrix3D(
                (-matrix.M22*matrix.M33*matrix.M44+matrix.M22*matrix.M34*matrix.OffsetZ+matrix.M32*matrix.M23*matrix.M44-matrix.M32*matrix.M24*matrix.OffsetZ-matrix.OffsetY*matrix.M23*matrix.M34+matrix.OffsetY*matrix.M24*matrix.M33)/(-matrix.M11*matrix.M22*matrix.M33*matrix.M44+matrix.M11*matrix.M22*matrix.M34*matrix.OffsetZ+matrix.M11*matrix.M32*matrix.M23*matrix.M44-matrix.M11*matrix.M32*matrix.M24*matrix.OffsetZ-matrix.M11*matrix.OffsetY*matrix.M23*matrix.M34+matrix.M11*matrix.OffsetY*matrix.M24*matrix.M33+matrix.M21*matrix.M12*matrix.M33*matrix.M44-matrix.M21*matrix.M12*matrix.M34*matrix.OffsetZ-matrix.M21*matrix.M32*matrix.M13*matrix.M44+matrix.M21*matrix.M32*matrix.M14*matrix.OffsetZ+matrix.M21*matrix.OffsetY*matrix.M13*matrix.M34-matrix.M21*matrix.OffsetY*matrix.M14*matrix.M33-matrix.M31*matrix.M12*matrix.M23*matrix.M44+matrix.M31*matrix.M12*matrix.M24*matrix.OffsetZ+matrix.M31*matrix.M22*matrix.M13*matrix.M44-matrix.M31*matrix.M22*matrix.M14*matrix.OffsetZ-matrix.M31*matrix.OffsetY*matrix.M13*matrix.M24+matrix.M31*matrix.OffsetY*matrix.M14*matrix.M23+matrix.OffsetX*matrix.M12*matrix.M23*matrix.M34-matrix.OffsetX*matrix.M12*matrix.M24*matrix.M33-matrix.OffsetX*matrix.M22*matrix.M13*matrix.M34+matrix.OffsetX*matrix.M22*matrix.M14*matrix.M33+matrix.OffsetX*matrix.M32*matrix.M13*matrix.M24-matrix.OffsetX*matrix.M32*matrix.M14*matrix.M23),
                -(-matrix.M12*matrix.M33*matrix.M44+matrix.M12*matrix.M34*matrix.OffsetZ+matrix.M32*matrix.M13*matrix.M44-matrix.M32*matrix.M14*matrix.OffsetZ-matrix.OffsetY*matrix.M13*matrix.M34+matrix.OffsetY*matrix.M14*matrix.M33)/(-matrix.M11*matrix.M22*matrix.M33*matrix.M44+matrix.M11*matrix.M22*matrix.M34*matrix.OffsetZ+matrix.M11*matrix.M32*matrix.M23*matrix.M44-matrix.M11*matrix.M32*matrix.M24*matrix.OffsetZ-matrix.M11*matrix.OffsetY*matrix.M23*matrix.M34+matrix.M11*matrix.OffsetY*matrix.M24*matrix.M33+matrix.M21*matrix.M12*matrix.M33*matrix.M44-matrix.M21*matrix.M12*matrix.M34*matrix.OffsetZ-matrix.M21*matrix.M32*matrix.M13*matrix.M44+matrix.M21*matrix.M32*matrix.M14*matrix.OffsetZ+matrix.M21*matrix.OffsetY*matrix.M13*matrix.M34-matrix.M21*matrix.OffsetY*matrix.M14*matrix.M33-matrix.M31*matrix.M12*matrix.M23*matrix.M44+matrix.M31*matrix.M12*matrix.M24*matrix.OffsetZ+matrix.M31*matrix.M22*matrix.M13*matrix.M44-matrix.M31*matrix.M22*matrix.M14*matrix.OffsetZ-matrix.M31*matrix.OffsetY*matrix.M13*matrix.M24+matrix.M31*matrix.OffsetY*matrix.M14*matrix.M23+matrix.OffsetX*matrix.M12*matrix.M23*matrix.M34-matrix.OffsetX*matrix.M12*matrix.M24*matrix.M33-matrix.OffsetX*matrix.M22*matrix.M13*matrix.M34+matrix.OffsetX*matrix.M22*matrix.M14*matrix.M33+matrix.OffsetX*matrix.M32*matrix.M13*matrix.M24-matrix.OffsetX*matrix.M32*matrix.M14*matrix.M23),
                (-matrix.M12*matrix.M23*matrix.M44+matrix.M12*matrix.M24*matrix.OffsetZ+matrix.M22*matrix.M13*matrix.M44-matrix.M22*matrix.M14*matrix.OffsetZ-matrix.OffsetY*matrix.M13*matrix.M24+matrix.OffsetY*matrix.M14*matrix.M23)/(-matrix.M11*matrix.M22*matrix.M33*matrix.M44+matrix.M11*matrix.M22*matrix.M34*matrix.OffsetZ+matrix.M11*matrix.M32*matrix.M23*matrix.M44-matrix.M11*matrix.M32*matrix.M24*matrix.OffsetZ-matrix.M11*matrix.OffsetY*matrix.M23*matrix.M34+matrix.M11*matrix.OffsetY*matrix.M24*matrix.M33+matrix.M21*matrix.M12*matrix.M33*matrix.M44-matrix.M21*matrix.M12*matrix.M34*matrix.OffsetZ-matrix.M21*matrix.M32*matrix.M13*matrix.M44+matrix.M21*matrix.M32*matrix.M14*matrix.OffsetZ+matrix.M21*matrix.OffsetY*matrix.M13*matrix.M34-matrix.M21*matrix.OffsetY*matrix.M14*matrix.M33-matrix.M31*matrix.M12*matrix.M23*matrix.M44+matrix.M31*matrix.M12*matrix.M24*matrix.OffsetZ+matrix.M31*matrix.M22*matrix.M13*matrix.M44-matrix.M31*matrix.M22*matrix.M14*matrix.OffsetZ-matrix.M31*matrix.OffsetY*matrix.M13*matrix.M24+matrix.M31*matrix.OffsetY*matrix.M14*matrix.M23+matrix.OffsetX*matrix.M12*matrix.M23*matrix.M34-matrix.OffsetX*matrix.M12*matrix.M24*matrix.M33-matrix.OffsetX*matrix.M22*matrix.M13*matrix.M34+matrix.OffsetX*matrix.M22*matrix.M14*matrix.M33+matrix.OffsetX*matrix.M32*matrix.M13*matrix.M24-matrix.OffsetX*matrix.M32*matrix.M14*matrix.M23),
                (matrix.M12*matrix.M23*matrix.M34-matrix.M12*matrix.M24*matrix.M33-matrix.M22*matrix.M13*matrix.M34+matrix.M22*matrix.M14*matrix.M33+matrix.M32*matrix.M13*matrix.M24-matrix.M32*matrix.M14*matrix.M23)/(-matrix.M11*matrix.M22*matrix.M33*matrix.M44+matrix.M11*matrix.M22*matrix.M34*matrix.OffsetZ+matrix.M11*matrix.M32*matrix.M23*matrix.M44-matrix.M11*matrix.M32*matrix.M24*matrix.OffsetZ-matrix.M11*matrix.OffsetY*matrix.M23*matrix.M34+matrix.M11*matrix.OffsetY*matrix.M24*matrix.M33+matrix.M21*matrix.M12*matrix.M33*matrix.M44-matrix.M21*matrix.M12*matrix.M34*matrix.OffsetZ-matrix.M21*matrix.M32*matrix.M13*matrix.M44+matrix.M21*matrix.M32*matrix.M14*matrix.OffsetZ+matrix.M21*matrix.OffsetY*matrix.M13*matrix.M34-matrix.M21*matrix.OffsetY*matrix.M14*matrix.M33-matrix.M31*matrix.M12*matrix.M23*matrix.M44+matrix.M31*matrix.M12*matrix.M24*matrix.OffsetZ+matrix.M31*matrix.M22*matrix.M13*matrix.M44-matrix.M31*matrix.M22*matrix.M14*matrix.OffsetZ-matrix.M31*matrix.OffsetY*matrix.M13*matrix.M24+matrix.M31*matrix.OffsetY*matrix.M14*matrix.M23+matrix.OffsetX*matrix.M12*matrix.M23*matrix.M34-matrix.OffsetX*matrix.M12*matrix.M24*matrix.M33-matrix.OffsetX*matrix.M22*matrix.M13*matrix.M34+matrix.OffsetX*matrix.M22*matrix.M14*matrix.M33+matrix.OffsetX*matrix.M32*matrix.M13*matrix.M24-matrix.OffsetX*matrix.M32*matrix.M14*matrix.M23),
                (matrix.M21*matrix.M33*matrix.M44-matrix.M21*matrix.M34*matrix.OffsetZ-matrix.M31*matrix.M23*matrix.M44+matrix.M31*matrix.M24*matrix.OffsetZ+matrix.OffsetX*matrix.M23*matrix.M34-matrix.OffsetX*matrix.M24*matrix.M33)/(-matrix.M11*matrix.M22*matrix.M33*matrix.M44+matrix.M11*matrix.M22*matrix.M34*matrix.OffsetZ+matrix.M11*matrix.M32*matrix.M23*matrix.M44-matrix.M11*matrix.M32*matrix.M24*matrix.OffsetZ-matrix.M11*matrix.OffsetY*matrix.M23*matrix.M34+matrix.M11*matrix.OffsetY*matrix.M24*matrix.M33+matrix.M21*matrix.M12*matrix.M33*matrix.M44-matrix.M21*matrix.M12*matrix.M34*matrix.OffsetZ-matrix.M21*matrix.M32*matrix.M13*matrix.M44+matrix.M21*matrix.M32*matrix.M14*matrix.OffsetZ+matrix.M21*matrix.OffsetY*matrix.M13*matrix.M34-matrix.M21*matrix.OffsetY*matrix.M14*matrix.M33-matrix.M31*matrix.M12*matrix.M23*matrix.M44+matrix.M31*matrix.M12*matrix.M24*matrix.OffsetZ+matrix.M31*matrix.M22*matrix.M13*matrix.M44-matrix.M31*matrix.M22*matrix.M14*matrix.OffsetZ-matrix.M31*matrix.OffsetY*matrix.M13*matrix.M24+matrix.M31*matrix.OffsetY*matrix.M14*matrix.M23+matrix.OffsetX*matrix.M12*matrix.M23*matrix.M34-matrix.OffsetX*matrix.M12*matrix.M24*matrix.M33-matrix.OffsetX*matrix.M22*matrix.M13*matrix.M34+matrix.OffsetX*matrix.M22*matrix.M14*matrix.M33+matrix.OffsetX*matrix.M32*matrix.M13*matrix.M24-matrix.OffsetX*matrix.M32*matrix.M14*matrix.M23),
                (-matrix.M11*matrix.M33*matrix.M44+matrix.M11*matrix.M34*matrix.OffsetZ+matrix.M31*matrix.M13*matrix.M44-matrix.M31*matrix.M14*matrix.OffsetZ-matrix.OffsetX*matrix.M13*matrix.M34+matrix.OffsetX*matrix.M14*matrix.M33)/(-matrix.M11*matrix.M22*matrix.M33*matrix.M44+matrix.M11*matrix.M22*matrix.M34*matrix.OffsetZ+matrix.M11*matrix.M32*matrix.M23*matrix.M44-matrix.M11*matrix.M32*matrix.M24*matrix.OffsetZ-matrix.M11*matrix.OffsetY*matrix.M23*matrix.M34+matrix.M11*matrix.OffsetY*matrix.M24*matrix.M33+matrix.M21*matrix.M12*matrix.M33*matrix.M44-matrix.M21*matrix.M12*matrix.M34*matrix.OffsetZ-matrix.M21*matrix.M32*matrix.M13*matrix.M44+matrix.M21*matrix.M32*matrix.M14*matrix.OffsetZ+matrix.M21*matrix.OffsetY*matrix.M13*matrix.M34-matrix.M21*matrix.OffsetY*matrix.M14*matrix.M33-matrix.M31*matrix.M12*matrix.M23*matrix.M44+matrix.M31*matrix.M12*matrix.M24*matrix.OffsetZ+matrix.M31*matrix.M22*matrix.M13*matrix.M44-matrix.M31*matrix.M22*matrix.M14*matrix.OffsetZ-matrix.M31*matrix.OffsetY*matrix.M13*matrix.M24+matrix.M31*matrix.OffsetY*matrix.M14*matrix.M23+matrix.OffsetX*matrix.M12*matrix.M23*matrix.M34-matrix.OffsetX*matrix.M12*matrix.M24*matrix.M33-matrix.OffsetX*matrix.M22*matrix.M13*matrix.M34+matrix.OffsetX*matrix.M22*matrix.M14*matrix.M33+matrix.OffsetX*matrix.M32*matrix.M13*matrix.M24-matrix.OffsetX*matrix.M32*matrix.M14*matrix.M23),
                -(-matrix.M11*matrix.M23*matrix.M44+matrix.M11*matrix.M24*matrix.OffsetZ+matrix.M21*matrix.M13*matrix.M44-matrix.M21*matrix.M14*matrix.OffsetZ-matrix.OffsetX*matrix.M13*matrix.M24+matrix.OffsetX*matrix.M14*matrix.M23)/(-matrix.M11*matrix.M22*matrix.M33*matrix.M44+matrix.M11*matrix.M22*matrix.M34*matrix.OffsetZ+matrix.M11*matrix.M32*matrix.M23*matrix.M44-matrix.M11*matrix.M32*matrix.M24*matrix.OffsetZ-matrix.M11*matrix.OffsetY*matrix.M23*matrix.M34+matrix.M11*matrix.OffsetY*matrix.M24*matrix.M33+matrix.M21*matrix.M12*matrix.M33*matrix.M44-matrix.M21*matrix.M12*matrix.M34*matrix.OffsetZ-matrix.M21*matrix.M32*matrix.M13*matrix.M44+matrix.M21*matrix.M32*matrix.M14*matrix.OffsetZ+matrix.M21*matrix.OffsetY*matrix.M13*matrix.M34-matrix.M21*matrix.OffsetY*matrix.M14*matrix.M33-matrix.M31*matrix.M12*matrix.M23*matrix.M44+matrix.M31*matrix.M12*matrix.M24*matrix.OffsetZ+matrix.M31*matrix.M22*matrix.M13*matrix.M44-matrix.M31*matrix.M22*matrix.M14*matrix.OffsetZ-matrix.M31*matrix.OffsetY*matrix.M13*matrix.M24+matrix.M31*matrix.OffsetY*matrix.M14*matrix.M23+matrix.OffsetX*matrix.M12*matrix.M23*matrix.M34-matrix.OffsetX*matrix.M12*matrix.M24*matrix.M33-matrix.OffsetX*matrix.M22*matrix.M13*matrix.M34+matrix.OffsetX*matrix.M22*matrix.M14*matrix.M33+matrix.OffsetX*matrix.M32*matrix.M13*matrix.M24-matrix.OffsetX*matrix.M32*matrix.M14*matrix.M23),
                -(matrix.M11*matrix.M23*matrix.M34-matrix.M11*matrix.M24*matrix.M33-matrix.M21*matrix.M13*matrix.M34+matrix.M21*matrix.M14*matrix.M33+matrix.M31*matrix.M13*matrix.M24-matrix.M31*matrix.M14*matrix.M23)/(-matrix.M11*matrix.M22*matrix.M33*matrix.M44+matrix.M11*matrix.M22*matrix.M34*matrix.OffsetZ+matrix.M11*matrix.M32*matrix.M23*matrix.M44-matrix.M11*matrix.M32*matrix.M24*matrix.OffsetZ-matrix.M11*matrix.OffsetY*matrix.M23*matrix.M34+matrix.M11*matrix.OffsetY*matrix.M24*matrix.M33+matrix.M21*matrix.M12*matrix.M33*matrix.M44-matrix.M21*matrix.M12*matrix.M34*matrix.OffsetZ-matrix.M21*matrix.M32*matrix.M13*matrix.M44+matrix.M21*matrix.M32*matrix.M14*matrix.OffsetZ+matrix.M21*matrix.OffsetY*matrix.M13*matrix.M34-matrix.M21*matrix.OffsetY*matrix.M14*matrix.M33-matrix.M31*matrix.M12*matrix.M23*matrix.M44+matrix.M31*matrix.M12*matrix.M24*matrix.OffsetZ+matrix.M31*matrix.M22*matrix.M13*matrix.M44-matrix.M31*matrix.M22*matrix.M14*matrix.OffsetZ-matrix.M31*matrix.OffsetY*matrix.M13*matrix.M24+matrix.M31*matrix.OffsetY*matrix.M14*matrix.M23+matrix.OffsetX*matrix.M12*matrix.M23*matrix.M34-matrix.OffsetX*matrix.M12*matrix.M24*matrix.M33-matrix.OffsetX*matrix.M22*matrix.M13*matrix.M34+matrix.OffsetX*matrix.M22*matrix.M14*matrix.M33+matrix.OffsetX*matrix.M32*matrix.M13*matrix.M24-matrix.OffsetX*matrix.M32*matrix.M14*matrix.M23),
                -(matrix.M21*matrix.M32*matrix.M44-matrix.M21*matrix.M34*matrix.OffsetY-matrix.M31*matrix.M22*matrix.M44+matrix.M31*matrix.M24*matrix.OffsetY+matrix.OffsetX*matrix.M22*matrix.M34-matrix.OffsetX*matrix.M24*matrix.M32)/(-matrix.M11*matrix.M22*matrix.M33*matrix.M44+matrix.M11*matrix.M22*matrix.M34*matrix.OffsetZ+matrix.M11*matrix.M32*matrix.M23*matrix.M44-matrix.M11*matrix.M32*matrix.M24*matrix.OffsetZ-matrix.M11*matrix.OffsetY*matrix.M23*matrix.M34+matrix.M11*matrix.OffsetY*matrix.M24*matrix.M33+matrix.M21*matrix.M12*matrix.M33*matrix.M44-matrix.M21*matrix.M12*matrix.M34*matrix.OffsetZ-matrix.M21*matrix.M32*matrix.M13*matrix.M44+matrix.M21*matrix.M32*matrix.M14*matrix.OffsetZ+matrix.M21*matrix.OffsetY*matrix.M13*matrix.M34-matrix.M21*matrix.OffsetY*matrix.M14*matrix.M33-matrix.M31*matrix.M12*matrix.M23*matrix.M44+matrix.M31*matrix.M12*matrix.M24*matrix.OffsetZ+matrix.M31*matrix.M22*matrix.M13*matrix.M44-matrix.M31*matrix.M22*matrix.M14*matrix.OffsetZ-matrix.M31*matrix.OffsetY*matrix.M13*matrix.M24+matrix.M31*matrix.OffsetY*matrix.M14*matrix.M23+matrix.OffsetX*matrix.M12*matrix.M23*matrix.M34-matrix.OffsetX*matrix.M12*matrix.M24*matrix.M33-matrix.OffsetX*matrix.M22*matrix.M13*matrix.M34+matrix.OffsetX*matrix.M22*matrix.M14*matrix.M33+matrix.OffsetX*matrix.M32*matrix.M13*matrix.M24-matrix.OffsetX*matrix.M32*matrix.M14*matrix.M23),
                -(-matrix.M11*matrix.M32*matrix.M44+matrix.M11*matrix.M34*matrix.OffsetY+matrix.M31*matrix.M12*matrix.M44-matrix.M31*matrix.M14*matrix.OffsetY-matrix.OffsetX*matrix.M12*matrix.M34+matrix.OffsetX*matrix.M14*matrix.M32)/(-matrix.M11*matrix.M22*matrix.M33*matrix.M44+matrix.M11*matrix.M22*matrix.M34*matrix.OffsetZ+matrix.M11*matrix.M32*matrix.M23*matrix.M44-matrix.M11*matrix.M32*matrix.M24*matrix.OffsetZ-matrix.M11*matrix.OffsetY*matrix.M23*matrix.M34+matrix.M11*matrix.OffsetY*matrix.M24*matrix.M33+matrix.M21*matrix.M12*matrix.M33*matrix.M44-matrix.M21*matrix.M12*matrix.M34*matrix.OffsetZ-matrix.M21*matrix.M32*matrix.M13*matrix.M44+matrix.M21*matrix.M32*matrix.M14*matrix.OffsetZ+matrix.M21*matrix.OffsetY*matrix.M13*matrix.M34-matrix.M21*matrix.OffsetY*matrix.M14*matrix.M33-matrix.M31*matrix.M12*matrix.M23*matrix.M44+matrix.M31*matrix.M12*matrix.M24*matrix.OffsetZ+matrix.M31*matrix.M22*matrix.M13*matrix.M44-matrix.M31*matrix.M22*matrix.M14*matrix.OffsetZ-matrix.M31*matrix.OffsetY*matrix.M13*matrix.M24+matrix.M31*matrix.OffsetY*matrix.M14*matrix.M23+matrix.OffsetX*matrix.M12*matrix.M23*matrix.M34-matrix.OffsetX*matrix.M12*matrix.M24*matrix.M33-matrix.OffsetX*matrix.M22*matrix.M13*matrix.M34+matrix.OffsetX*matrix.M22*matrix.M14*matrix.M33+matrix.OffsetX*matrix.M32*matrix.M13*matrix.M24-matrix.OffsetX*matrix.M32*matrix.M14*matrix.M23),
                (-matrix.M11*matrix.M22*matrix.M44+matrix.M11*matrix.M24*matrix.OffsetY+matrix.M21*matrix.M12*matrix.M44-matrix.M21*matrix.M14*matrix.OffsetY-matrix.OffsetX*matrix.M12*matrix.M24+matrix.OffsetX*matrix.M14*matrix.M22)/(-matrix.M11*matrix.M22*matrix.M33*matrix.M44+matrix.M11*matrix.M22*matrix.M34*matrix.OffsetZ+matrix.M11*matrix.M32*matrix.M23*matrix.M44-matrix.M11*matrix.M32*matrix.M24*matrix.OffsetZ-matrix.M11*matrix.OffsetY*matrix.M23*matrix.M34+matrix.M11*matrix.OffsetY*matrix.M24*matrix.M33+matrix.M21*matrix.M12*matrix.M33*matrix.M44-matrix.M21*matrix.M12*matrix.M34*matrix.OffsetZ-matrix.M21*matrix.M32*matrix.M13*matrix.M44+matrix.M21*matrix.M32*matrix.M14*matrix.OffsetZ+matrix.M21*matrix.OffsetY*matrix.M13*matrix.M34-matrix.M21*matrix.OffsetY*matrix.M14*matrix.M33-matrix.M31*matrix.M12*matrix.M23*matrix.M44+matrix.M31*matrix.M12*matrix.M24*matrix.OffsetZ+matrix.M31*matrix.M22*matrix.M13*matrix.M44-matrix.M31*matrix.M22*matrix.M14*matrix.OffsetZ-matrix.M31*matrix.OffsetY*matrix.M13*matrix.M24+matrix.M31*matrix.OffsetY*matrix.M14*matrix.M23+matrix.OffsetX*matrix.M12*matrix.M23*matrix.M34-matrix.OffsetX*matrix.M12*matrix.M24*matrix.M33-matrix.OffsetX*matrix.M22*matrix.M13*matrix.M34+matrix.OffsetX*matrix.M22*matrix.M14*matrix.M33+matrix.OffsetX*matrix.M32*matrix.M13*matrix.M24-matrix.OffsetX*matrix.M32*matrix.M14*matrix.M23),
                (matrix.M11*matrix.M22*matrix.M34-matrix.M11*matrix.M24*matrix.M32-matrix.M21*matrix.M12*matrix.M34+matrix.M21*matrix.M14*matrix.M32+matrix.M31*matrix.M12*matrix.M24-matrix.M31*matrix.M14*matrix.M22)/(-matrix.M11*matrix.M22*matrix.M33*matrix.M44+matrix.M11*matrix.M22*matrix.M34*matrix.OffsetZ+matrix.M11*matrix.M32*matrix.M23*matrix.M44-matrix.M11*matrix.M32*matrix.M24*matrix.OffsetZ-matrix.M11*matrix.OffsetY*matrix.M23*matrix.M34+matrix.M11*matrix.OffsetY*matrix.M24*matrix.M33+matrix.M21*matrix.M12*matrix.M33*matrix.M44-matrix.M21*matrix.M12*matrix.M34*matrix.OffsetZ-matrix.M21*matrix.M32*matrix.M13*matrix.M44+matrix.M21*matrix.M32*matrix.M14*matrix.OffsetZ+matrix.M21*matrix.OffsetY*matrix.M13*matrix.M34-matrix.M21*matrix.OffsetY*matrix.M14*matrix.M33-matrix.M31*matrix.M12*matrix.M23*matrix.M44+matrix.M31*matrix.M12*matrix.M24*matrix.OffsetZ+matrix.M31*matrix.M22*matrix.M13*matrix.M44-matrix.M31*matrix.M22*matrix.M14*matrix.OffsetZ-matrix.M31*matrix.OffsetY*matrix.M13*matrix.M24+matrix.M31*matrix.OffsetY*matrix.M14*matrix.M23+matrix.OffsetX*matrix.M12*matrix.M23*matrix.M34-matrix.OffsetX*matrix.M12*matrix.M24*matrix.M33-matrix.OffsetX*matrix.M22*matrix.M13*matrix.M34+matrix.OffsetX*matrix.M22*matrix.M14*matrix.M33+matrix.OffsetX*matrix.M32*matrix.M13*matrix.M24-matrix.OffsetX*matrix.M32*matrix.M14*matrix.M23),
                -(-matrix.M21*matrix.M32*matrix.OffsetZ+matrix.M21*matrix.M33*matrix.OffsetY+matrix.M31*matrix.M22*matrix.OffsetZ-matrix.M31*matrix.M23*matrix.OffsetY-matrix.OffsetX*matrix.M22*matrix.M33+matrix.OffsetX*matrix.M23*matrix.M32)/(-matrix.M11*matrix.M22*matrix.M33*matrix.M44+matrix.M11*matrix.M22*matrix.M34*matrix.OffsetZ+matrix.M11*matrix.M32*matrix.M23*matrix.M44-matrix.M11*matrix.M32*matrix.M24*matrix.OffsetZ-matrix.M11*matrix.OffsetY*matrix.M23*matrix.M34+matrix.M11*matrix.OffsetY*matrix.M24*matrix.M33+matrix.M21*matrix.M12*matrix.M33*matrix.M44-matrix.M21*matrix.M12*matrix.M34*matrix.OffsetZ-matrix.M21*matrix.M32*matrix.M13*matrix.M44+matrix.M21*matrix.M32*matrix.M14*matrix.OffsetZ+matrix.M21*matrix.OffsetY*matrix.M13*matrix.M34-matrix.M21*matrix.OffsetY*matrix.M14*matrix.M33-matrix.M31*matrix.M12*matrix.M23*matrix.M44+matrix.M31*matrix.M12*matrix.M24*matrix.OffsetZ+matrix.M31*matrix.M22*matrix.M13*matrix.M44-matrix.M31*matrix.M22*matrix.M14*matrix.OffsetZ-matrix.M31*matrix.OffsetY*matrix.M13*matrix.M24+matrix.M31*matrix.OffsetY*matrix.M14*matrix.M23+matrix.OffsetX*matrix.M12*matrix.M23*matrix.M34-matrix.OffsetX*matrix.M12*matrix.M24*matrix.M33-matrix.OffsetX*matrix.M22*matrix.M13*matrix.M34+matrix.OffsetX*matrix.M22*matrix.M14*matrix.M33+matrix.OffsetX*matrix.M32*matrix.M13*matrix.M24-matrix.OffsetX*matrix.M32*matrix.M14*matrix.M23),
                -(matrix.M11*matrix.M32*matrix.OffsetZ-matrix.M11*matrix.M33*matrix.OffsetY-matrix.M31*matrix.M12*matrix.OffsetZ+matrix.M31*matrix.M13*matrix.OffsetY+matrix.OffsetX*matrix.M12*matrix.M33-matrix.OffsetX*matrix.M13*matrix.M32)/(-matrix.M11*matrix.M22*matrix.M33*matrix.M44+matrix.M11*matrix.M22*matrix.M34*matrix.OffsetZ+matrix.M11*matrix.M32*matrix.M23*matrix.M44-matrix.M11*matrix.M32*matrix.M24*matrix.OffsetZ-matrix.M11*matrix.OffsetY*matrix.M23*matrix.M34+matrix.M11*matrix.OffsetY*matrix.M24*matrix.M33+matrix.M21*matrix.M12*matrix.M33*matrix.M44-matrix.M21*matrix.M12*matrix.M34*matrix.OffsetZ-matrix.M21*matrix.M32*matrix.M13*matrix.M44+matrix.M21*matrix.M32*matrix.M14*matrix.OffsetZ+matrix.M21*matrix.OffsetY*matrix.M13*matrix.M34-matrix.M21*matrix.OffsetY*matrix.M14*matrix.M33-matrix.M31*matrix.M12*matrix.M23*matrix.M44+matrix.M31*matrix.M12*matrix.M24*matrix.OffsetZ+matrix.M31*matrix.M22*matrix.M13*matrix.M44-matrix.M31*matrix.M22*matrix.M14*matrix.OffsetZ-matrix.M31*matrix.OffsetY*matrix.M13*matrix.M24+matrix.M31*matrix.OffsetY*matrix.M14*matrix.M23+matrix.OffsetX*matrix.M12*matrix.M23*matrix.M34-matrix.OffsetX*matrix.M12*matrix.M24*matrix.M33-matrix.OffsetX*matrix.M22*matrix.M13*matrix.M34+matrix.OffsetX*matrix.M22*matrix.M14*matrix.M33+matrix.OffsetX*matrix.M32*matrix.M13*matrix.M24-matrix.OffsetX*matrix.M32*matrix.M14*matrix.M23),
                (matrix.M11*matrix.M22*matrix.OffsetZ-matrix.M11*matrix.M23*matrix.OffsetY-matrix.M21*matrix.M12*matrix.OffsetZ+matrix.M21*matrix.M13*matrix.OffsetY+matrix.OffsetX*matrix.M12*matrix.M23-matrix.OffsetX*matrix.M13*matrix.M22)/(-matrix.M11*matrix.M22*matrix.M33*matrix.M44+matrix.M11*matrix.M22*matrix.M34*matrix.OffsetZ+matrix.M11*matrix.M32*matrix.M23*matrix.M44-matrix.M11*matrix.M32*matrix.M24*matrix.OffsetZ-matrix.M11*matrix.OffsetY*matrix.M23*matrix.M34+matrix.M11*matrix.OffsetY*matrix.M24*matrix.M33+matrix.M21*matrix.M12*matrix.M33*matrix.M44-matrix.M21*matrix.M12*matrix.M34*matrix.OffsetZ-matrix.M21*matrix.M32*matrix.M13*matrix.M44+matrix.M21*matrix.M32*matrix.M14*matrix.OffsetZ+matrix.M21*matrix.OffsetY*matrix.M13*matrix.M34-matrix.M21*matrix.OffsetY*matrix.M14*matrix.M33-matrix.M31*matrix.M12*matrix.M23*matrix.M44+matrix.M31*matrix.M12*matrix.M24*matrix.OffsetZ+matrix.M31*matrix.M22*matrix.M13*matrix.M44-matrix.M31*matrix.M22*matrix.M14*matrix.OffsetZ-matrix.M31*matrix.OffsetY*matrix.M13*matrix.M24+matrix.M31*matrix.OffsetY*matrix.M14*matrix.M23+matrix.OffsetX*matrix.M12*matrix.M23*matrix.M34-matrix.OffsetX*matrix.M12*matrix.M24*matrix.M33-matrix.OffsetX*matrix.M22*matrix.M13*matrix.M34+matrix.OffsetX*matrix.M22*matrix.M14*matrix.M33+matrix.OffsetX*matrix.M32*matrix.M13*matrix.M24-matrix.OffsetX*matrix.M32*matrix.M14*matrix.M23),
                -(matrix.M11*matrix.M22*matrix.M33-matrix.M11*matrix.M23*matrix.M32-matrix.M21*matrix.M12*matrix.M33+matrix.M21*matrix.M13*matrix.M32+matrix.M31*matrix.M12*matrix.M23-matrix.M31*matrix.M13*matrix.M22)/(-matrix.M11*matrix.M22*matrix.M33*matrix.M44+matrix.M11*matrix.M22*matrix.M34*matrix.OffsetZ+matrix.M11*matrix.M32*matrix.M23*matrix.M44-matrix.M11*matrix.M32*matrix.M24*matrix.OffsetZ-matrix.M11*matrix.OffsetY*matrix.M23*matrix.M34+matrix.M11*matrix.OffsetY*matrix.M24*matrix.M33+matrix.M21*matrix.M12*matrix.M33*matrix.M44-matrix.M21*matrix.M12*matrix.M34*matrix.OffsetZ-matrix.M21*matrix.M32*matrix.M13*matrix.M44+matrix.M21*matrix.M32*matrix.M14*matrix.OffsetZ+matrix.M21*matrix.OffsetY*matrix.M13*matrix.M34-matrix.M21*matrix.OffsetY*matrix.M14*matrix.M33-matrix.M31*matrix.M12*matrix.M23*matrix.M44+matrix.M31*matrix.M12*matrix.M24*matrix.OffsetZ+matrix.M31*matrix.M22*matrix.M13*matrix.M44-matrix.M31*matrix.M22*matrix.M14*matrix.OffsetZ-matrix.M31*matrix.OffsetY*matrix.M13*matrix.M24+matrix.M31*matrix.OffsetY*matrix.M14*matrix.M23+matrix.OffsetX*matrix.M12*matrix.M23*matrix.M34-matrix.OffsetX*matrix.M12*matrix.M24*matrix.M33-matrix.OffsetX*matrix.M22*matrix.M13*matrix.M34+matrix.OffsetX*matrix.M22*matrix.M14*matrix.M33+matrix.OffsetX*matrix.M32*matrix.M13*matrix.M24-matrix.OffsetX*matrix.M32*matrix.M14*matrix.M23));

            return Verify(expected, actual);
        }

        private Matrix3D GetMultipliedMatrix(Matrix3D m1, Matrix3D m2)
        {
            // Copy & Pasted from Maple V worksheet
            return new Matrix3D(
                m1.M11*m2.M11+m1.M12*m2.M21+m1.M13*m2.M31+m1.M14*m2.OffsetX,
                m1.M11*m2.M12+m1.M12*m2.M22+m1.M13*m2.M32+m1.M14*m2.OffsetY,
                m1.M11*m2.M13+m1.M12*m2.M23+m1.M13*m2.M33+m1.M14*m2.OffsetZ,
                m1.M11*m2.M14+m1.M12*m2.M24+m1.M13*m2.M34+m1.M14*m2.M44,
                m1.M21*m2.M11+m1.M22*m2.M21+m1.M23*m2.M31+m1.M24*m2.OffsetX,
                m1.M21*m2.M12+m1.M22*m2.M22+m1.M23*m2.M32+m1.M24*m2.OffsetY,
                m1.M21*m2.M13+m1.M22*m2.M23+m1.M23*m2.M33+m1.M24*m2.OffsetZ,
                m1.M21*m2.M14+m1.M22*m2.M24+m1.M23*m2.M34+m1.M24*m2.M44,
                m1.M31*m2.M11+m1.M32*m2.M21+m1.M33*m2.M31+m1.M34*m2.OffsetX,
                m1.M31*m2.M12+m1.M32*m2.M22+m1.M33*m2.M32+m1.M34*m2.OffsetY,
                m1.M31*m2.M13+m1.M32*m2.M23+m1.M33*m2.M33+m1.M34*m2.OffsetZ,
                m1.M31*m2.M14+m1.M32*m2.M24+m1.M33*m2.M34+m1.M34*m2.M44,
                m1.OffsetX*m2.M11+m1.OffsetY*m2.M21+m1.OffsetZ*m2.M31+m1.M44*m2.OffsetX,
                m1.OffsetX*m2.M12+m1.OffsetY*m2.M22+m1.OffsetZ*m2.M32+m1.M44*m2.OffsetY,
                m1.OffsetX*m2.M13+m1.OffsetY*m2.M23+m1.OffsetZ*m2.M33+m1.M44*m2.OffsetZ,
                m1.OffsetX*m2.M14+m1.OffsetY*m2.M24+m1.OffsetZ*m2.M34+m1.M44*m2.M44);
        }

        private Matrix3D GetRotationMatrix(Quaternion quaternion, Point3D center)
        {
            Matrix3D matrix = new Matrix3D();

            double x = quaternion.X;
            double y = quaternion.Y;
            double z = quaternion.Z;
            double w = quaternion.W;

            matrix.M11 = w*w + x*x - y*y - z*z;
            matrix.M12 = 2*x*y + 2*w*z;
            matrix.M13 = 2*x*z - 2*w*y;
            matrix.M21 = 2*x*y - 2*w*z;
            matrix.M22 = w*w - x*x + y*y - z*z;
            matrix.M23 = 2*y*z + 2*w*x;
            matrix.M31 = 2*x*z + 2*w*y;
            matrix.M32 = 2*y*z - 2*w*x;
            matrix.M33 = w*w - x*x - y*y + z*z;
            matrix.M44 = w*w + x*x + y*y + z*z;

            return GetCenteredMatrix(matrix, center);
        }

        private Matrix3D GetScaleMatrix(Vector3D scale, Point3D center)
        {
            Matrix3D matrix = new Matrix3D(
                scale.X, 0, 0, 0,
                0, scale.Y, 0, 0,
                0, 0, scale.Z, 0,
                0, 0, 0, 1);

            return GetCenteredMatrix(matrix, center);
        }

        private Matrix3D GetTranslationMatrix(Vector3D offset)
        {
            return new Matrix3D(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                offset.X, offset.Y, offset.Z, 1);
        }

        private Matrix3D GetCenteredMatrix(Matrix3D matrix, Point3D center)
        {
            return GetTranslationMatrix(-((Vector3D)center)) * matrix * GetTranslationMatrix((Vector3D)center);
        }

        private bool Verify(Matrix3D expected, Matrix3D actual)
        {
            bool success = true;

            success &= Verify(expected.M11, actual.M11);
            success &= Verify(expected.M12, actual.M12);
            success &= Verify(expected.M13, actual.M13);
            success &= Verify(expected.M14, actual.M14);

            success &= Verify(expected.M21, actual.M21);
            success &= Verify(expected.M22, actual.M22);
            success &= Verify(expected.M23, actual.M23);
            success &= Verify(expected.M24, actual.M24);

            success &= Verify(expected.M31, actual.M31);
            success &= Verify(expected.M32, actual.M32);
            success &= Verify(expected.M33, actual.M33);
            success &= Verify(expected.M34, actual.M34);

            success &= Verify(expected.OffsetX, actual.OffsetX);
            success &= Verify(expected.OffsetY, actual.OffsetY);
            success &= Verify(expected.OffsetZ, actual.OffsetZ);
            success &= Verify(expected.M44, actual.M44);

            return success;
        }

        private bool Verify(Vector3D expected, Vector3D actual)
        {
            bool success = true;

            success &= Verify(expected.X, actual.X);
            success &= Verify(expected.Y, actual.Y);
            success &= Verify(expected.Z, actual.Z);

            return success;
        }

        internal double DBL_EPSILON = 2.2204460492503131e-016; /* smallest such that 1.0+DBL_EPSILON != 1.0 */

        private bool Verify(double value1, double value2)
        {
            //in case they are Infinities (then epsilon check does not work)
            if(value1 == value2) return true;

            // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < DBL_EPSILON
            double eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * DBL_EPSILON;
            double delta = value1 - value2;

            return(-1E-10< delta) && (1E-10 > delta);
        }

        private bool VerifyLowPrecision(double value1, double value2)
        {
            return Math.Abs(value1 - value2) < 0.0000015;
        }

        protected override void BuildLights(Model3DGroup group, bool leftSide) {}
        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide) {}
    }

    public class TestVisualTreeHelper : TestCase
    {
        public override string ToString() { return "VisualTreeHelper"; }
        public override bool DoRoundtrip() { return false; }

        public override bool DoTest()
        {
            bool success = true;

            // Create a Triangle in the XY plane whose bounds are 1.0 x 1.0 at the origin.
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions = new Point3DCollection(new Point3D[] {
                new Point3D(-0.5,-0.5,0),
                new Point3D(0.5,-0.5,0),
                new Point3D(0,0.5,0) });
            mesh.TriangleIndices = new Int32Collection(new int[] { 0, 1, 2 });
            GeometryModel3D content = new GeometryModel3D(mesh, new EmissiveMaterial(Brushes.Green));

            // Construct 2 Viewport3DVisuals with cameras which will project the
            // triangle to the size of their viewports.

            Rect viewport = new Rect(100, 100, 250, 250);

            Point3D position = new Point3D(0, 0, 2.5);
            Vector3D lookDirection = new Vector3D(0, 0, -1);
            Vector3D up = new Vector3D(0, 1, 0);

            ModelVisual3D mv1 = new ModelVisual3D();
            mv1.Content = content;

            Viewport3DVisual vv1 = new Viewport3DVisual();
            vv1.Camera = new OrthographicCamera(position, lookDirection, up, 1.0);
            vv1.Viewport = viewport;
            vv1.Children.Add(mv1);

            ModelVisual3D mv2 = new ModelVisual3D();
            mv2.Content = content;

            Viewport3DVisual vv2 = new Viewport3DVisual();
            vv2.Camera = new PerspectiveCamera(position, lookDirection, up, 2 * Math.Atan(0.5 / position.Z) * (180.0 / Math.PI));
            vv2.Viewport = viewport;
            vv2.Children.Add(mv2);

            // Verify that the VisualTreeHelper can indeed walk the Viewport3DVisual.
            success &= VisualTreeHelper.GetChildrenCount(vv1) == 1;
            success &= VisualTreeHelper.GetChild(vv1, 0) == mv1;

            // Verify that the content bounds of both Viewport3DVisuals is empty
            // (that is, 3D children are children, not content.)
            success &= VisualTreeHelper.GetContentBounds(vv1) == Rect.Empty;
            success &= VisualTreeHelper.GetContentBounds(vv2) == Rect.Empty;

            // Verify that the 2D descendant bounds of both Viewport3DVisuals is the same
            // as their viewports.

            Rect db1 = VisualTreeHelper.GetDescendantBounds(vv1);
            Rect db2 = VisualTreeHelper.GetDescendantBounds(vv2);

            success &= (db1 == viewport && db2 == viewport);

            // Parent both Viewport3DVisuals to a DrawingVisual and verify that
            // FindCommondVisualAncestor correctly finds the drawing visual as the
            // common ancestor.

            DrawingVisual dv = new DrawingVisual();
            dv.Children.Add(vv1);
            dv.Children.Add(vv2);

            success &= mv1.FindCommonVisualAncestor(mv2) == dv;
            success &= mv1.FindCommonVisualAncestor(vv2) == dv;
            success &= vv1.FindCommonVisualAncestor(mv2) == dv;
            success &= vv1.FindCommonVisualAncestor(mv1) == vv1;
            success &= mv1.FindCommonVisualAncestor(vv1) == vv1;

            // Apply some transforms to our ModelVisual3Ds

            mv1.Transform = new TranslateTransform3D(new Vector3D(-1.0, 0, 0));

            Transform3DGroup tg = new Transform3DGroup();
            tg.Children.Add(new ScaleTransform3D(new Vector3D(0.5, 1, 1)));
            tg.Children.Add(new TranslateTransform3D(new Vector3D(0, -1.0, 0)));
            mv2.Transform = tg;

            success &= VisualTreeHelper.GetDescendantBounds(dv) == new Rect(-150, 100, 437.5, 500);

            //
            // Test Drawing, Bounds, & HitTest VisualTreeHelper
            //
            Model3DGroup models = new Model3DGroup();

            models.Children.Add(content);

            // Test VisualTreeHelper.GetDrawing(ViewportVisual3D)
            DrawingGroup dg = VisualTreeHelper.GetDrawing(vv1);
            // The Drawing should be empty
            success &= (dg == null);

            // Test VisualTreeHelper.GetContentBounds(ViewportVisual3D)
            Rect bounds = VisualTreeHelper.GetContentBounds(vv1);

            // Test that Visual bounds are in "inner" space, but Model3D bounds
            // are in "outer" space.
            {
                ModelVisual3D v = new ModelVisual3D();
                v.Content = content;

                Rect3D contentBefore = content.Bounds;
                Rect3D visualBefore = VisualTreeHelper.GetContentBounds(v);
                Rect3D subgraphBefore = VisualTreeHelper.GetDescendantBounds(v);

                success &= contentBefore == visualBefore;
                success &= visualBefore == subgraphBefore;

                // Changing the Model3D's transform will affect Model3D.Bounds, Visual's Content
                // bounds, and by extension the descendant bounds.
                
                Vector3D offset = new Vector3D(3, 11, 29);                

                content.Transform = new TranslateTransform3D(offset);
                Rect3D expected = new Rect3D(contentBefore.Location + offset, contentBefore.Size);

                success &= content.Bounds == expected;
                success &= VisualTreeHelper.GetContentBounds(v) == expected;
                success &= VisualTreeHelper.GetDescendantBounds(v) == expected;
                
                // However, changing the Visual's transform affects nothing.
                
                v.Transform = new ScaleTransform3D(new Vector3D(10, 20, 30));

                success &= VisualTreeHelper.GetContentBounds(v) == expected;
                success &= VisualTreeHelper.GetDescendantBounds(v) == expected;
            }

            return success;
        }

        protected override void BuildLights(Model3DGroup group, bool leftSide) {}
        protected override void BuildGeometricScene(Model3DGroup group, bool leftSide) {}
    }
  
    public class TestModelVisual3D : AnimationTest
    {
        public class Square : ModelVisual3D
        {
            public Square()
            {
                Update();
            }
            
            public static readonly DependencyProperty WidthProperty =
                DependencyProperty.Register(
                        "Width",
                        /* propertyType = */ typeof(double),
                        /* ownerType = */ typeof(Square),
                        new PropertyMetadata(1.0, WidthPropertyChanged));

            private static void WidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                ((Square)d).Update();
            }

            public double Width
            {
                get { return (double) GetValue(WidthProperty); }
                set { SetValue(WidthProperty, value); }
            }

            public static readonly DependencyProperty HeightProperty =
                DependencyProperty.Register(
                        "Height",
                        /* propertyType = */ typeof(double),
                        /* ownerType = */ typeof(Square),
                        new PropertyMetadata(1.0, HeightPropertyChanged));

            private static void HeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                ((Square)d).Update();
            }

            public double Height
            {
                get { return (double) GetValue(HeightProperty); }
                set { SetValue(HeightProperty, value); }
            }

            public static readonly DependencyProperty ColorProperty =
                DependencyProperty.Register(
                        "Color",
                        /* propertyType = */ typeof(Color),
                        /* ownerType = */ typeof(Square),
                        new PropertyMetadata(Colors.Red, ColorPropertyChanged));

            private static void ColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                ((Square)d).Update();
            }

            public Color Color
            {
                get { return (Color) GetValue(ColorProperty); }
                set { SetValue(ColorProperty, value); }
            }

            private void Update()
            {
                MeshGeometry3D mesh = new MeshGeometry3D();

                double hw = Width / 2;
                double hh = Height / 2;
                    
                mesh.Positions = new Point3DCollection(new Point3D[] {
                    new Point3D(-hw,  hh, 0),
                    new Point3D(-hw, -hh, 0),
                    new Point3D( hw, -hh, 0),
                    new Point3D( hw,  hh, 0),
                    });

                mesh.Normals = Vector3DCollection.Parse("0,1,0 0,1,0 0,1,0 0,1,0");
                mesh.TriangleIndices = Int32Collection.Parse("0 1 2 0 2 3");

                MaterialGroup material = new MaterialGroup();
                material.Children.Add(new DiffuseMaterial(Brushes.Black));
                material.Children.Add(new EmissiveMaterial(new SolidColorBrush(this.Color)));                

                GeometryModel3D model = new GeometryModel3D();
                model.Geometry = mesh;
                model.Material = material;
                model.BackMaterial = material;
                this.Content = model;
            }
        }
        
        public override string ToString()
        {
            return "TestModelVisual3D";
        }

        public override bool DoRoundtrip()
        {
            return false;
        }
        
        protected override ModelVisual3D BuildScene(bool isLeft)
        {
            Square square = new Square();

            square.BeginAnimation(Square.ColorProperty, BlueAnimation);

            if (isLeft)
            {
                square.BeginAnimation(Square.WidthProperty, NegativeOneToOneInASecondAnimation);
                square.BeginAnimation(Square.HeightProperty, OneToNegativeOneInASecondAnimation);
            }
            else
            {
                Matrix3D matrix = new Matrix3D();

                matrix.Scale(new Vector3D(.05, .05, .05));
                matrix.Translate(new Vector3D(.1, .1, 0));

                square.Transform = new MatrixTransform3D(matrix);
            }

            return square;
        }

        public override bool DoTest()
        {
            // Hit test the untransformed position. This should fail.
            bool success = !DoHitTest(_rightVisual, new Point(194, 257));

            // Hit test the transformed position. This should pass.
            success &= DoHitTest(_rightVisual, new Point(266, 185));

            return success;
        }
        
    } 
}
