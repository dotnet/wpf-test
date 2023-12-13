// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Graphics.ReferenceRender;
using Microsoft.Test.Graphics.Factories;

// Subnamespace "UnitTests" is required for this case to be picked up by /RunAll
namespace Microsoft.Test.Graphics.UnitTests
{
    /// <summary>
    /// Test Visual3D APIs (and some from Viewport3DVisual)
    /// </summary>
    public class Visual3DTest : CoreGraphicsTest
    {
        /// <summary/>
        public override void RunTheTest()
        {
            if (priority == 0)
            {
                TestGetParent();
                TestIsAncestorOf();
                TestIsDescendantOf();
                TestFindCommonVisualAncestor();
                TestGetContentBounds();
                TestGetDescendantBounds();
                TestGetChild();
                TestGetChildrenCount();
            }
            else // priority > 0
            {
                TestIsAncestorOf2();
                TestIsDescendantOf2();
                TestFindCommonVisualAncestor2();
                TestHitTest2();
                TestAnimation2();
            }
        }

        private void TestGetParent()
        {
            Log("Testing VisualTreeHelper.GetParent...");

            Visual3D parent = ModelVisual3DFactory.OneKid;
            Visual3D child = VisualUtils.GetChild(parent, 0);
            Viewport3DVisual viewport = new Viewport3DVisual();
            ContainerVisual visual = new ContainerVisual();
            visual.Children.Add(viewport);
            viewport.Children.Add(parent);

            TestGetParentWith(child, parent);                 // Visual3D with Visual3D parent
            TestGetParentWith(parent, viewport);              // Visual3D with Visual parent
            TestGetParentWith(viewport, visual);              // Visual with Visual parent
            TestGetParentWith(visual, null);                  // Visual with no parent
            TestGetParentWith(new Viewport3DVisual(), null);  // Viewport3DVisual with no parent
            TestGetParentWith(ModelVisual3DFactory.NoKids, null);  // Visual3D with no parent
        }

        private void TestGetParentWith(DependencyObject child, DependencyObject actualParent)
        {
            DependencyObject theirAnswer = VisualTreeHelper.GetParent(child);

            if (!ObjectUtils.Equals(theirAnswer, actualParent) || failOnPurpose)
            {
                AddFailure("VisualTreeHelper.GetParent failed. Results for remaining tests may be wrong.");
            }

            if (child is Viewport3DVisual)
            {
                // Even if the above check fails, at least GetParent and Parent should return the same thing.
                DependencyObject localParent = ((Viewport3DVisual)child).Parent;
                if (!ObjectUtils.Equals(localParent, theirAnswer))
                {
                    AddFailure("Viewport3DVisual.Parent returned something different than VisualTreeHelper.GetParent");
                }
            }
        }

        private void TestIsAncestorOf()
        {
            Log("Testing Visual3D.IsAncestorOf...");

            Visual3D v = ModelVisual3DFactory.NoKids;

            TestIsAncestorOfWith(v, v);

            v = ModelVisual3DFactory.OneKid;
            TestIsAncestorOfWith(v, VisualUtils.GetChild(v, 0));
            TestIsAncestorOfWith(VisualUtils.GetChild(v, 0), v);

            v = ModelVisual3DFactory.FourKids;
            TestIsAncestorOfWith(VisualUtils.GetChild(v, 0), VisualUtils.GetChild(v, 1));
            TestIsAncestorOfWith(v, VisualUtils.GetChild(v, 3));

            v = ModelVisual3DFactory.SixteenDeep;
            TestIsAncestorOfWith(v, VisualUtils.GetChild(v, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
            TestIsAncestorOfWith(VisualUtils.GetChild(v, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0), v);

            ContainerVisual visual = new ContainerVisual();
            Viewport3DVisual viewport = new Viewport3DVisual();
            v = ModelVisual3DFactory.OneKid;
            viewport.Children.Add(v);
            visual.Children.Add(viewport);

            TestIsAncestorOfWith(v, viewport);
            TestIsAncestorOfWith(VisualUtils.GetChild(v, 0), viewport);

            TestIsAncestorOfWith(v, visual);
            TestIsAncestorOfWith(VisualUtils.GetChild(v, 0), visual);
        }

        private void TestIsAncestorOfWith(Visual3D ancestor, DependencyObject child)
        {
            bool theirAnswer = ancestor.IsAncestorOf(child);
            bool myAnswer = VisualUtils.IsAncestorOf(ancestor, child);

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("IsAncestorOf failed");
            }
        }

        private void TestIsDescendantOf()
        {
            Log("Testing Visual3D.IsDescendantOf...");

            Visual3D v = ModelVisual3DFactory.NoKids;

            TestIsDescendantOfWith(v, v);

            v = ModelVisual3DFactory.OneKid;
            TestIsDescendantOfWith(v, VisualUtils.GetChild(v, 0));
            TestIsDescendantOfWith(VisualUtils.GetChild(v, 0), v);

            v = ModelVisual3DFactory.FourKids;
            TestIsDescendantOfWith(VisualUtils.GetChild(v, 0), VisualUtils.GetChild(v, 1));
            TestIsDescendantOfWith(VisualUtils.GetChild(v, 3), v);

            v = ModelVisual3DFactory.SixteenDeep;
            TestIsDescendantOfWith(v, VisualUtils.GetChild(v, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
            TestIsDescendantOfWith(VisualUtils.GetChild(v, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0), v);

            ContainerVisual visual = new ContainerVisual();
            Viewport3DVisual viewport = new Viewport3DVisual();
            v = ModelVisual3DFactory.OneKid;
            viewport.Children.Add(v);
            visual.Children.Add(viewport);

            TestIsDescendantOfWith(v, viewport);
            TestIsDescendantOfWith(VisualUtils.GetChild(v, 0), viewport);

            TestIsDescendantOfWith(v, visual);
            TestIsDescendantOfWith(VisualUtils.GetChild(v, 0), visual);
        }

        private void TestIsDescendantOfWith(Visual3D child, DependencyObject ancestor)
        {
            bool theirAnswer = child.IsDescendantOf(ancestor);
            bool myAnswer = VisualUtils.IsAncestorOf(ancestor, child);

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("IsDescendantOf failed");
            }
        }

        private void TestFindCommonVisualAncestor()
        {
            Log("Testing Visual3D.FindCommonVisualAncestor...");

            Visual3D v = ModelVisual3DFactory.NoKids;
            TestFindCommonVisualAncestorWith(v, v);
            TestFindCommonVisualAncestorWith(v, ModelVisual3DFactory.NoKids);

            v = ModelVisual3DFactory.OneKid;
            TestFindCommonVisualAncestorWith(v, VisualUtils.GetChild(v, 0));
            TestFindCommonVisualAncestorWith(VisualUtils.GetChild(v, 0), v);

            v = ModelVisual3DFactory.TwoKidsTwoGrandkids;
            TestFindCommonVisualAncestorWith(VisualUtils.GetChild(v, 0, 0), VisualUtils.GetChild(v, 1, 0));
            TestFindCommonVisualAncestorWith(VisualUtils.GetChild(v, 1, 0), VisualUtils.GetChild(v, 0, 0));
            TestFindCommonVisualAncestorWith(VisualUtils.GetChild(v, 0), VisualUtils.GetChild(v, 1, 0));
            TestFindCommonVisualAncestorWith(VisualUtils.GetChild(v, 0, 0), VisualUtils.GetChild(v, 1));

            v = ModelVisual3DFactory.SixteenDeep;
            TestFindCommonVisualAncestorWith(VisualUtils.GetChild(v, 0), VisualUtils.GetChild(v, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
            TestFindCommonVisualAncestorWith(VisualUtils.GetChild(v, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0), VisualUtils.GetChild(v, 0));

            v = ModelVisual3DFactory.Unbalanced;
            TestFindCommonVisualAncestorWith(VisualUtils.GetChild(v, 0), VisualUtils.GetChild(v, 2, 3, 3));

            // Cross over into the 2D world
            ContainerVisual root = new ContainerVisual();
            Viewport3DVisual viewport1 = new Viewport3DVisual();
            Viewport3DVisual viewport2 = new Viewport3DVisual();
            root.Children.Add(viewport1);
            root.Children.Add(viewport2);
            Visual3D v1 = ModelVisual3DFactory.OneKid;
            Visual3D v2 = ModelVisual3DFactory.OneKid;
            viewport1.Children.Add(v1);
            viewport2.Children.Add(v2);
            TestFindCommonVisualAncestorWith(v1, root);
            TestFindCommonVisualAncestorWith(v1, viewport1);
            TestFindCommonVisualAncestorWith(v1, viewport2);
            TestFindCommonVisualAncestorWith(v1, v2);
            TestFindCommonVisualAncestorWith(v1, VisualUtils.GetChild(v2, 0));
            TestFindCommonVisualAncestorWith(v2, root);
            TestFindCommonVisualAncestorWith(v2, viewport1);
            TestFindCommonVisualAncestorWith(v2, v1);
            TestFindCommonVisualAncestorWith(v2, VisualUtils.GetChild(v1, 0));
        }

        private void TestFindCommonVisualAncestorWith(Visual3D visual1, DependencyObject visual2)
        {
            DependencyObject theirAnswer = visual1.FindCommonVisualAncestor(visual2);
            DependencyObject myAnswer = VisualUtils.GetLeastCommonAncestor(visual1, visual2);

            if (!ObjectUtils.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("FindCommonVisualAncestor failed");
            }
        }

        private void TestGetContentBounds()
        {
            Log("Testing VisualTreeHelper.GetContentBounds...");

            // Visual3D with no transforms (on model or visual)
            ModelVisual3D visual = ModelVisual3DFactory.NoKids;
            visual.Content = ModelFactory.MakeModel("Sphere 24 24 1.0");      // Model3D.Bounds = -1,-1,-1, 2,2,2
            TestGetContentBoundsWith(visual, true);                           // Visual3D.Bounds = [same as above]

            // Visual3D with transform on model (not visual)
            visual.Content.Transform = new TranslateTransform3D(1, -1, 0.5);    // This should affect the Visual3d's bounds
            TestGetContentBoundsWith(visual, true);                           // Visual3D.Bounds = 0,-2,-0.5 2,2,2

            // Visual3D with transforms on model and visual
            visual.Transform = new TranslateTransform3D(-1, 1, -0.5);           // This should NOT affect the Visual3D's bounds
            TestGetContentBoundsWith(visual, true);                           // Visual3D.Bounds = [same as last case]

            // Visual3D with null Content (make sure children's bounds are not included)
            ModelVisual3D parent = ModelVisual3DFactory.NoKids;
            parent.Children.Add(visual);
            TestGetContentBoundsWith(parent, true);                           // Visual3D.Bounds = Empty

            // Their bounding box will be bigger.
            // Bounds are not always tight in the case of transforms
            parent.Content = ModelFactory.MakeModel("UnitPlaneTriangle");     // Model3D.Bounds = 0,0,0, 1,1,1
            RotateTransform3D tx = new RotateTransform3D();
            tx.Rotation = new AxisAngleRotation3D(new Vector3D(-1, 0, 1), 54.7356);
            tx.CenterX = .5;
            tx.CenterZ = .5;
            parent.Content.Transform = tx;
            TestGetContentBoundsWith(parent, false);                          // Tight bounds = -0.366,0,-0.366 1.366,0,1.366
        }

        private void TestGetContentBoundsWith(Visual3D visual, bool tightBox)
        {
            Rect3D theirAnswer = VisualTreeHelper.GetContentBounds(visual);
            Rect3D myAnswer = VisualBounder.GetContentBounds(visual);

            if (tightBox && (MathEx.NotCloseEnough(theirAnswer, myAnswer) || failOnPurpose))
            {
                AddFailure("GetContentBounds failed (tight bounds)");
                Log("*** Expected: {0}", myAnswer);
                Log("***   Actual: {0}", theirAnswer);
            }

            // Rect3D.Contains is tested by Rect3DTest.  It is trustworthy.
            if (!tightBox && (!theirAnswer.Contains(myAnswer) || failOnPurpose))
            {
                AddFailure("GetContentBounds failed (loose bounds).");
                Log("Their bounds should contain my bounds.");
                Log("***  My Bounds: {0}", myAnswer);
                Log("*** WPF Bounds: {0}", theirAnswer);
            }
        }

        private void TestGetDescendantBounds()
        {
            Log("Testing VisualTreeHelper.GetDescendantBounds...");

            ModelVisual3D parent = new ModelVisual3D();

            // Need to add test cases for Content/Transforms on parent.

            ModelVisual3D triangle = new ModelVisual3D();
            triangle.Content = ModelFactory.MakeModel("UnitPlaneTriangle");   // Model3D.Bounds = 0,0,0 1,1,1
            ModelVisual3D cube = new ModelVisual3D();
            cube.Content = ModelFactory.MakeModel("SimpleCubeMesh");      // Model3D.Bounds = -1,-1,-1 2,2,2

            // Visual3D with no children
            TestGetDescendantBoundsWith(parent, true);                    // Visual3D.DescBounds = Empty

            // Visual3D with one child
            parent.Children.Add(triangle);
            TestGetDescendantBoundsWith(parent, true);                    // Visual3D.DescBounds = 0,0,0 1,1,1

            // Transform on child's Content
            triangle.Content.Transform = new TranslateTransform3D(1, 1, 1);
            TestGetDescendantBoundsWith(parent, true);                    // Visual3D.DescBounds = 1,1,1 1,1,1

            // Transform on child
            triangle.Transform = new TranslateTransform3D(1, 1, 1);
            TestGetDescendantBoundsWith(parent, true);                    // Visual3D.DescBounds = 2,2,2 1,1,1

            // Transform on parent should have no effect
            parent.Transform = new TranslateTransform3D(-4, -4, -4);
            TestGetDescendantBoundsWith(parent, true);                    // Visual3D.DescBounds = 2,2,2 1,1,1

            // Add a child
            parent.Children.Add(cube);
            TestGetDescendantBoundsWith(parent, true);                    // Visual3D.DescBounds = -1,-1,-1 4,4,4

            // Transform the other child's Content
            cube.Content.Transform = new TranslateTransform3D(-1, -1, -1);
            TestGetDescendantBoundsWith(parent, true);                    // Visual3D.DescBounds = -2,-2,-2 5,5,5

            // Transform the other child
            cube.Transform = new TranslateTransform3D(-1, -1, -1);
            TestGetDescendantBoundsWith(parent, true);                    // Visual3D.DescBounds = -3,-3,-3 6,6,6

            // Add one more child with content really deep
            parent.Children.Add(ModelVisual3DFactory.SixteenDeep);
            Model3D sphere = ModelFactory.MakeModel("Sphere 24 24 4.0");  // Model3D.Bounds = -4,-4,-4 8,8,8
            VisualUtils.SetChildContent(parent, sphere, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            TestGetDescendantBoundsWith(parent, true);                    // Visual3D.DescBounds = -4,-4,-4 8,8,8

            // Transform it to increase the bounds
            VisualUtils.SetChildTransform(parent, TransformFactory.MakeTransform("Rotate 0,1,0 45"), 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            TestGetDescendantBoundsWith(parent, false);                   // Tight bounds = -4,-4,-4 8,8,8

            // Make the bounds even bigger
            VisualUtils.SetChildTransform(parent, TransformFactory.MakeTransform("Rotate 0,1,0 45"), 2, 0, 0, 0, 0, 0, 0, 0, 0);
            TestGetDescendantBoundsWith(parent, false);                   // Tight bounds = -4,-4,-4 8,8,8
        }

        private void TestGetDescendantBoundsWith(Visual3D visual, bool tightBox)
        {
            Rect3D theirAnswer = VisualTreeHelper.GetDescendantBounds(visual);
            Rect3D myAnswer = VisualBounder.GetChildrenBounds(visual);

            if (tightBox && (MathEx.NotCloseEnough(theirAnswer, myAnswer) || failOnPurpose))
            {
                AddFailure("GetDescendantBounds failed (tight bounds)");
                Log("*** Expected: {0}", myAnswer);
                Log("***   Actual: {0}", theirAnswer);
            }

            // Rect3D.Contains is tested by Rect3DTest.  It is trustworthy.
            if (!tightBox && (!theirAnswer.Contains(myAnswer) || failOnPurpose))
            {
                AddFailure("GetDescendantBounds failed (loose bounds).");
                Log("Their bounds should contain my bounds.");
                Log("***  My Bounds: {0}", myAnswer);
                Log("*** WPF Bounds: {0}", theirAnswer);
            }
        }

        private void TestGetChild()
        {
            Log("Testing VisualTreeHelper.GetChild...");

            Viewport3DVisual visual = new Viewport3DVisual();
            visual.Children.Add(ModelVisual3DFactory.NoKids);

            TestGetChildWith(visual, 0);

            visual.Children.Add(ModelVisual3DFactory.FourKids);
            TestGetChildWith(visual, 0);
            TestGetChildWith(visual, 1);

            visual.Children.Add(ModelVisual3DFactory.OneKid);
            TestGetChildWith(visual, 0);
            TestGetChildWith(visual, 1);
            TestGetChildWith(visual, 2);
        }

        private void TestGetChildWith(Viewport3DVisual visual, int index)
        {
            DependencyObject theirAnswer = VisualTreeHelper.GetChild(visual, index);
            DependencyObject myAnswer = visual.Children[index];

            if (theirAnswer != myAnswer)
            {
                AddFailure("VisualTreeHelper.GetChild failed");
            }
        }

        private void TestGetChildrenCount()
        {
            Log("Testing VisualTreeHelper.GetChildrenCount...");

            Viewport3DVisual visual = new Viewport3DVisual();
            TestGetChildrenCountWith(visual, 0);
            visual.Children.Add(ModelVisual3DFactory.NoKids);
            TestGetChildrenCountWith(visual, 1);
            visual.Children.Add(ModelVisual3DFactory.OneKid);
            TestGetChildrenCountWith(visual, 2);
            visual.Children.Add(ModelVisual3DFactory.FourKids);
            TestGetChildrenCountWith(visual, 3);
        }

        private void TestGetChildrenCountWith(Viewport3DVisual visual, int myAnswer)
        {
            int theirAnswer = VisualTreeHelper.GetChildrenCount(visual);

            if (theirAnswer != myAnswer)
            {
                AddFailure("VisualTreeHelper.GetChildrenCount failed");
                Log("*** Expected: {0}", myAnswer);
                Log("***   Actual: {0}", theirAnswer);
            }
        }

        private void TestIsAncestorOf2()
        {
            Log("Testing Visual3D.IsAncestorOf with bad parameters...");

            Try(IsAncestorOfNull, typeof(ArgumentNullException));
        }

        #region ExceptionThrowers for IsAncestorOf

        private void IsAncestorOfNull()
        {
            Visual3D v = ModelVisual3DFactory.OneKid;
            TestIsAncestorOfWith(v, null);
        }

        #endregion

        private void TestIsDescendantOf2()
        {
            Log("Testing Visual3D.IsDescendantOf with bad parameters...");

            Try(IsDescendantOfNull, typeof(ArgumentNullException));
        }

        #region ExceptionThrowers for IsDescendantOf

        private void IsDescendantOfNull()
        {
            Visual3D v = ModelVisual3DFactory.OneKid;
            TestIsDescendantOfWith(v, null);
        }

        #endregion

        private void TestFindCommonVisualAncestor2()
        {
            Log("Testing Visual3D.FindCommonVisualAncestor with bad parameters...");

            Try(FindCommonAncestorOfNull, typeof(ArgumentNullException));
        }

        #region ExceptionThrowers for FindCommonVisualAncestor

        private void FindCommonAncestorOfNull()
        {
            Visual3D v = ModelVisual3DFactory.OneKid;
            TestFindCommonVisualAncestorWith(v, null);
        }

        #endregion

        private void TestHitTest2()
        {
            Log("Testing Visual3D/Viewport3DVisual.HitTest with bad parameters...");

            Try(NullCallback, typeof(ArgumentNullException));
            Try(NullParameters, typeof(ArgumentNullException));
            Try(GeometryParameters, typeof(NotSupportedException));
            Try(NonInvertibleMatrixCamera, typeof(NotSupportedException));
            SafeExecute(NoChildren);
        }

        #region ExceptionThrowers for HitTest

        private void NullCallback()
        {
            ModelVisual3D v = new ModelVisual3D();
            VisualTreeHelper.HitTest(v, null, null, new RayHitTestParameters(new Point3D(), new Vector3D()));
        }

        private void NullParameters()
        {
            ModelVisual3D v = new ModelVisual3D();
            VisualTreeHelper.HitTest(v, null, Callback, null);
        }

        private void GeometryParameters()
        {
            Viewport3DVisual v = new Viewport3DVisual();
            v.Viewport = new Rect(0, 0, 100, 100);
            v.Camera = CameraFactory.OrthographicDefault;
            ModelVisual3D m = new ModelVisual3D();
            m.Content = SceneFactory.UnitPlane;
            v.Children.Add(m);
            v.HitTest(null, Callback, new GeometryHitTestParameters(new RectangleGeometry(v.Viewport)));
        }

        private void NonInvertibleMatrixCamera()
        {
            Viewport3DVisual v = new Viewport3DVisual();
            v.Viewport = new Rect(0, 0, 100, 100);
            v.Camera = CameraFactory.MatrixNonInvertible;
            ModelVisual3D m = new ModelVisual3D();
            m.Content = SceneFactory.LayeredMeshes;
            v.Children.Add(m);
            v.HitTest(new Point(50, 50));
        }

        private HitTestResultBehavior Callback(HitTestResult result)
        {
            return HitTestResultBehavior.Continue;
        }

        #endregion

        #region SafeExecute blocks for HitTest

        private void NoChildren()
        {
            Viewport3DVisual v = new Viewport3DVisual();
            HitTestResult r = v.HitTest(new Point());
            if (r != null)
            {
                AddFailure("Hit Testing an empty Viewport3DVisual should return null");
            }
        }

        #endregion

        private void TestAnimation2()
        {
            Log("Testing BeginAnimation with bad parameters...");

            Try(BeginAnimationNullDP, typeof(ArgumentNullException));
            Try(BeginAnimationNonAnimatableProperty, typeof(ArgumentException));
            Try(BeginAnimationWrongAnimationType, typeof(ArgumentException));
            Try(BeginAnimationInvalidHandoffBehavior, typeof(ArgumentException));

            Log("Testing ApplyAnimationClock with bad parameters...");

            Try(ApplyAnimationClockNullDP, typeof(ArgumentNullException));
            Try(ApplyAnimationClockNonAnimatableProperty, typeof(ArgumentException));
            Try(ApplyAnimationClockWrongAnimationType, typeof(ArgumentException));
            Try(ApplyAnimationClockInvalidHandoffBehavior, typeof(ArgumentException));
        }

        #region ExceptionThrowers for TestAnimation

        private void BeginAnimationNullDP()
        {
            CustomV3D visual = new CustomV3D();
            visual.BeginAnimation(null, new Vector3DAnimation());
        }
        private void BeginAnimationNonAnimatableProperty()
        {
            CustomV3D visual = new CustomV3D();
            visual.BeginAnimation(CustomV3D.ReadOnlyPropertyKey.DependencyProperty, new BooleanAnimationUsingKeyFrames());
        }
        private void BeginAnimationWrongAnimationType()
        {
            CustomV3D visual = new CustomV3D();
            visual.BeginAnimation(CustomV3D.IntProperty, new DoubleAnimation());
        }
        private void BeginAnimationInvalidHandoffBehavior()
        {
            CustomV3D visual = new CustomV3D();
            visual.BeginAnimation(CustomV3D.IntProperty, null, (HandoffBehavior)100);
        }
        private void ApplyAnimationClockNullDP()
        {
            CustomV3D visual = new CustomV3D();
            visual.ApplyAnimationClock(null, new Vector3DAnimation().CreateClock());
        }
        private void ApplyAnimationClockNonAnimatableProperty()
        {
            CustomV3D visual = new CustomV3D();
            visual.ApplyAnimationClock(CustomV3D.ReadOnlyPropertyKey.DependencyProperty, new BooleanAnimationUsingKeyFrames().CreateClock());
        }
        private void ApplyAnimationClockWrongAnimationType()
        {
            CustomV3D visual = new CustomV3D();
            visual.ApplyAnimationClock(CustomV3D.IntProperty, new DoubleAnimation().CreateClock());
        }
        private void ApplyAnimationClockInvalidHandoffBehavior()
        {
            CustomV3D visual = new CustomV3D();
            visual.ApplyAnimationClock(CustomV3D.IntProperty, null, (HandoffBehavior)100);
        }

        #endregion
    }
    /// <summary/>
    public class CustomV3D : ModelVisual3D
    {
        /// <summary/>
        public CustomV3D()
        {
        }

        /// <summary/>
        public int Int
        {
            get { return (int)GetValue(IntProperty); }
            set { SetValue(IntProperty, value); }
        }

        /// <summary/>
        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyPropertyKey.DependencyProperty); }
        }

        /// <summary/>
        public static DependencyProperty IntProperty =
                DependencyProperty.Register("Int", typeof(int), typeof(CustomV3D), new PropertyMetadata(0));

        /// <summary/>
        public static DependencyPropertyKey ReadOnlyPropertyKey =
                DependencyProperty.RegisterReadOnly("ReadOnly", typeof(bool), typeof(CustomV3D), new PropertyMetadata(true));
    }
}
