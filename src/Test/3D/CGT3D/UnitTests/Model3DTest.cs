// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Media.Media3D;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Graphics.Factories;
using Microsoft.Test.Graphics.ReferenceRender;


// Subnamespace "UnitTests" is required for this case to be picked up by /RunAll
namespace Microsoft.Test.Graphics.UnitTests
{
    /// <summary/>
    public class Model3DTest : CoreGraphicsTest
    {
        /// <summary/>
        public override void RunTheTest()
        {
            TestBounds();
        }

        private void TestBounds()
        {
            // Basic testing of untransformed models

            Model3D light = LightFactory.WhiteAmbient;
            Model3D model1 = ModelFactory.MakeModel("UnitPlaneTriangle", MaterialFactory.Green, null);
            Model3D model2 = ModelFactory.MakeModel("Sphere 12 24 0.5", MaterialFactory.Yellow, null);
            Model3D model3 = ModelFactory.MakeModel("SimpleCubeMesh", MaterialFactory.White, null);
            Model3DGroup group1 = new Model3DGroup();
            Model3DGroup group2 = new Model3DGroup();
            group2.Children.Add(model1);
            group2.Children.Add(model2);
            Model3DGroup group3 = new Model3DGroup();
            group3.Children.Add(light);
            group3.Children.Add(model3);

            TestBoundsWith(light);    // Empty
            TestBoundsWith(model1);   // 0,0,0, 1,1,1
            TestBoundsWith(model2);   // -.5,-.5,-.5, 1.0,1.0,1.0
            TestBoundsWith(model3);   // -1,-1,-1, 2,2,2
            TestBoundsWith(group1);   // Empty
            TestBoundsWith(group2);   // -.5,-.5,-.5, 1.5,1.5,1.5
            TestBoundsWith(group3);   // -1,-1,-1, 2,2,2

            // Basic testing of transformed models (and bounds caching)

            light.Transform = TransformFactory.MakeTransform("Translate 1,1,1");
            model1.Transform = TransformFactory.MakeTransform("Translate 5,-5,5");
            model2.Transform = TransformFactory.MakeTransform("Scale 1.5,0.8,1.1");
            model3.Transform = TransformFactory.MakeTransform("Rotate 1,1,1 60");
            group1.Transform = TransformFactory.MakeTransform("Scale 2,2,2");
            group2.Transform = null;    // Check null handling and transform updates for children
            group3.Transform = TransformFactory.MakeTransform("Translate -1,-1,-1");

            TestBoundsWith(light);    // Empty
            TestBoundsWith(model1);   // 5,-5,5, 1,1,1
            TestBoundsWith(model2);   // -.75,-.4,-.55, 1.5,.8,1.1
            TestBoundsWith(model3);   // -1.67,-1.67,-1.67, 3.33,3.33,3.33
            TestBoundsWith(group1);   // Empty
            TestBoundsWith(group2);   // -.75,-5,-.55, 6.75,5.4,6.55
            TestBoundsWith(group3);   // -2.67,-2.67,-2.67, 3.33,3.33,3.33

            // Basic testing of transformed models (and bounds caching)

            model1.Transform = TransformFactory.MakeTransform("Rotate 0,1,0 -45");
            model2.Transform = TransformFactory.MakeTransform("Translate -1,-2,-3");
            model3.Transform = TransformFactory.MakeTransform("Scale 0.5,2.0,-3");
            group1.Children.Add(model1);
            group1.Children.Add(group2);
            group2.Transform = TransformFactory.MakeTransform("Rotate 1,0,0 45");
            group3.Transform = TransformFactory.MakeTransform("Rotate -1,-1,-1 45");

            TestBoundsWith(model1, false);    // -.707,0,0, 1.414,1,.707
            TestBoundsWith(model2);           // -1.5,-2.5,-3.5, 1,1,1
            TestBoundsWith(model3);           // -.5,-2,-3, 1,4,6
            TestBoundsWith(group1, false);    // -3,-1,-8.071, 4.414,3.414,9.485
            TestBoundsWith(group2, false);    // -1.5,-0.5,-4.036, 2.207,1.707,4.743
            TestBoundsWith(group3);           // -2.346,-3.282,-3.288, 4.692,6.565,6.577
        }

        private void TestBoundsWith(Model3D model)
        {
            TestBoundsWith(model, true);
        }

        private void TestBoundsWith(Model3D model, bool tightBox)
        {
            Rect3D theirAnswer = model.Bounds;
            Rect3D myAnswer = ModelBounder.GetBounds(model);

            if (tightBox && MathEx.NotCloseEnough(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Model3D.Bounds failed");
                Log("*** Expected: {0}", myAnswer);
                Log("***   Actual: {0}", theirAnswer);
            }

            // WPF transforms bounding boxes instead of vertices.
            // non-90 degree rotations cause bounding boxes to become larger.
            if (!tightBox && (!theirAnswer.Contains(myAnswer) || failOnPurpose))
            {
                AddFailure("Model3D.Bounds failed (loose bounds).");
                Log("Their bounds should contain my bounds.");
                Log("***  My Bounds: {0}", myAnswer);
                Log("*** WPF Bounds: {0}", theirAnswer);
            }
        }
    }
}