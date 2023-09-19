// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Media.Media3D;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Graphics.Factories;

// Subnamespace "UnitTests" is required for this case to be picked up by /RunAll
namespace Microsoft.Test.Graphics.UnitTests
{
    /// <summary/>
    public class Transform3DAPITest : CoreGraphicsTest
    {
        /// <summary/>
        public override void RunTheTest()
        {
            if (priority > 0)
            {
                RunTheTest2();
            }
            else
            {
                TestStaticCreators();
                TestIdentity();
                TestAffinity();
                TestTransform();
                TestMatrixTransformProperties();
            }
        }

        private void TestStaticCreators()
        {
            // These all test Transform3D.Value too
            Log("Testing new TranslateTransform3D...");

            TestCreateTranslationWith(Const.v0);
            TestCreateTranslationWith(Const.v1);
            TestCreateTranslationWith(Const.vNeg1);

            Log("Testing new RotateTransform3D...");

            TestCreateRotationWith(Const.v1, 45, Const.p0);
            TestCreateRotationWith(Const.vNeg1, 90, Const.p10);

            Log("Testing new ScaleTransform3D...");

            TestCreateScaleWith(Const.v0, Const.p0);
            TestCreateScaleWith(Const.v1, Const.p0);
            TestCreateScaleWith(Const.v1, Const.p10);
            TestCreateScaleWith(Const.v10, Const.pNeg1);

            Log("Testing new MatrixTransform3D...");

            TestCreateMatrixWith(Const.mIdent);
            TestCreateMatrixWith(Const.mAffine);
            TestCreateMatrixWith(Const.mNAffine);
        }

        private void TestCreateTranslationWith(Vector3D offset)
        {
            TranslateTransform3D theirAnswer1 = new TranslateTransform3D(offset);
            TranslateTransform3D theirAnswer2 = new TranslateTransform3D();
            theirAnswer2.OffsetX = offset.X;
            theirAnswer2.OffsetY = offset.Y;
            theirAnswer2.OffsetZ = offset.Z;
            TranslateTransform3D theirAnswer3 = new TranslateTransform3D(offset.X, offset.Y, offset.Z);
            Matrix3D myAnswer = MatrixUtils.Translate(offset);

            if (MathEx.NotCloseEnough(theirAnswer1.Value, myAnswer) ||
                 failOnPurpose)
            {
                AddFailure("new TranslateTransform3D( Vector3D ) failed");
                Log("*** Expected =\r\n" + MatrixUtils.ToStr(myAnswer));
                Log("*** Actual   =\r\n" + MatrixUtils.ToStr(theirAnswer1.Value));
            }

            if (MathEx.NotCloseEnough(theirAnswer2.Value, myAnswer) ||
                 MathEx.NotCloseEnough(theirAnswer2.OffsetX, offset.X) ||
                 MathEx.NotCloseEnough(theirAnswer2.OffsetY, offset.Y) ||
                 MathEx.NotCloseEnough(theirAnswer2.OffsetZ, offset.Z) ||
                 failOnPurpose)
            {
                AddFailure("TranslateTransform3D.get/set_Offset failed");
                Log("*** Expected: {0}", offset);
                Log("*** Actual:   {0},{1},{2}", theirAnswer2.OffsetX, theirAnswer2.OffsetY, theirAnswer2.OffsetZ);
            }

            if (MathEx.NotCloseEnough(theirAnswer3.Value, myAnswer) ||
                 MathEx.NotCloseEnough(theirAnswer3.OffsetX, offset.X) ||
                 MathEx.NotCloseEnough(theirAnswer3.OffsetY, offset.Y) ||
                 MathEx.NotCloseEnough(theirAnswer3.OffsetZ, offset.Z) ||
                 failOnPurpose)
            {
                AddFailure("TranslateTransform3D.get/set_Offset failed");
                Log("*** Expected: {0}", offset);
                Log("*** Actual:   {0},{1},{2}", theirAnswer3.OffsetX, theirAnswer3.OffsetY, theirAnswer3.OffsetZ);
            }
        }

        private void TestCreateRotationWith(Vector3D axis, double angle, Point3D center)
        {
            RotateTransform3D theirAnswer1 = new RotateTransform3D(new AxisAngleRotation3D(axis, angle));
            RotateTransform3D theirAnswer2 = new RotateTransform3D(new AxisAngleRotation3D(axis, angle), center);
            AxisAngleRotation3D rotation = new AxisAngleRotation3D(axis, angle);
            RotateTransform3D theirAnswer3 = new RotateTransform3D(rotation);
            RotateTransform3D theirAnswer4 = new RotateTransform3D(rotation, center);
            RotateTransform3D theirAnswer5 = new RotateTransform3D(rotation, center.X, center.Y, center.Z);
            Matrix3D myAnswer1 = MatrixUtils.Rotate(axis, angle);
            Matrix3D myAnswer2 = MatrixUtils.Rotate(axis, angle, center);

            if (MathEx.NotCloseEnough(theirAnswer1.Value, myAnswer1) ||
                 failOnPurpose)
            {
                AddFailure("new RotateTransform3D( Vector3D, double ) failed");
                Log("*** Axis   = " + axis);
                Log("*** Angle  = " + angle);
                Log("*** Expected =\r\n" + MatrixUtils.ToStr(myAnswer1));
                Log("*** Actual   =\r\n" + MatrixUtils.ToStr(theirAnswer1.Value));
            }
            if (MathEx.NotCloseEnough(theirAnswer2.Value, myAnswer2) ||
                 failOnPurpose)
            {
                AddFailure("new RotateTransform3D( Vector3D, double, Point3D ) failed");
                Log("*** Axis   = " + axis);
                Log("*** Angle  = " + angle);
                Log("*** Center = " + center);
                Log("*** Expected =\r\n" + MatrixUtils.ToStr(myAnswer2));
                Log("*** Actual   =\r\n" + MatrixUtils.ToStr(theirAnswer2.Value));
            }

            if (MathEx.NotCloseEnough(theirAnswer3.Value, myAnswer1) || failOnPurpose)
            {
                AddFailure("new RotateTransform3D failed");
                Log("*** Expected: Rotation = {0}", rotation);
                Log("*** Actual:   Rotation = {0}", theirAnswer3.Rotation);
                Log("*** Expected =\r\n" + MatrixUtils.ToStr(myAnswer1));
                Log("*** Actual   =\r\n" + MatrixUtils.ToStr(theirAnswer3.Value));
            }

            if (MathEx.NotCloseEnough(theirAnswer4.Value, myAnswer2) ||
                 MathEx.NotCloseEnough(theirAnswer4.CenterX, center.X) ||
                 MathEx.NotCloseEnough(theirAnswer4.CenterY, center.Y) ||
                 MathEx.NotCloseEnough(theirAnswer4.CenterZ, center.Z) ||
                 failOnPurpose)
            {
                AddFailure("RotateTransform3D.get/set_Rotation/Center failed");
                Log("*** Expected: Rotation = {0}, Center = {1}", rotation, center);
                Log("*** Actual:   Rotation = {0}, Center = {1},{2},{3}", theirAnswer4.Rotation, theirAnswer4.CenterX, theirAnswer4.CenterY, theirAnswer4.CenterZ);
                Log("*** Expected =\r\n" + MatrixUtils.ToStr(myAnswer2));
                Log("*** Actual   =\r\n" + MatrixUtils.ToStr(theirAnswer4.Value));
            }

            if (MathEx.NotCloseEnough(theirAnswer5.Value, myAnswer2) ||
                 MathEx.NotCloseEnough(theirAnswer5.CenterX, center.X) ||
                 MathEx.NotCloseEnough(theirAnswer5.CenterY, center.Y) ||
                 MathEx.NotCloseEnough(theirAnswer5.CenterZ, center.Z) ||
                 failOnPurpose)
            {
                AddFailure("RotateTransform3D.get/set_Rotation/Center failed");
                Log("*** Expected: Rotation = {0}, Center = {1}", rotation, center);
                Log("*** Actual:   Rotation = {0}, Center = {1},{2},{3}", theirAnswer5.Rotation, theirAnswer5.CenterX, theirAnswer5.CenterY, theirAnswer5.CenterZ);
                Log("*** Expected =\r\n" + MatrixUtils.ToStr(myAnswer2));
                Log("*** Actual   =\r\n" + MatrixUtils.ToStr(theirAnswer5.Value));
            }
        }

        private void TestCreateScaleWith(Vector3D scale, Point3D center)
        {
            ScaleTransform3D theirAnswer1 = new ScaleTransform3D(scale);
            ScaleTransform3D theirAnswer2 = new ScaleTransform3D(scale, center);
            ScaleTransform3D theirAnswer3 = new ScaleTransform3D();
            ScaleTransform3D theirAnswer4 = new ScaleTransform3D();
            theirAnswer3.ScaleX = scale.X;
            theirAnswer3.ScaleY = scale.Y;
            theirAnswer3.ScaleZ = scale.Z;
            theirAnswer4.ScaleX = scale.X;
            theirAnswer4.ScaleY = scale.Y;
            theirAnswer4.ScaleZ = scale.Z;
            theirAnswer4.CenterX = center.X;
            theirAnswer4.CenterY = center.Y;
            theirAnswer4.CenterZ = center.Z;
            ScaleTransform3D theirAnswer5 = new ScaleTransform3D(scale.X, scale.Y, scale.Z);
            ScaleTransform3D theirAnswer6 = new ScaleTransform3D(scale.X, scale.Y, scale.Z, center.X, center.Y, center.Z);
            Matrix3D myAnswer1 = MatrixUtils.Scale(scale);
            Matrix3D myAnswer2 = MatrixUtils.Scale(scale, center);

            if (MathEx.NotCloseEnough(theirAnswer1.Value, myAnswer1) ||
                 failOnPurpose)
            {
                AddFailure("new ScaleTransform3D( Vector3D ) failed");
                Log("*** Scale = " + scale);
                Log("*** Expected =\r\n" + MatrixUtils.ToStr(myAnswer1));
                Log("*** Actual   =\r\n" + MatrixUtils.ToStr(theirAnswer1.Value));
            }

            if (MathEx.NotCloseEnough(theirAnswer2.Value, myAnswer2) ||
                 failOnPurpose)
            {
                AddFailure("new ScaleTransform3D( Vector3D, Point3D ) failed");
                Log("*** Scale  = " + scale);
                Log("*** Center = " + center);
                Log("*** Expected =\r\n" + MatrixUtils.ToStr(myAnswer2));
                Log("*** Actual   =\r\n" + MatrixUtils.ToStr(theirAnswer2.Value));
            }

            if (MathEx.NotCloseEnough(theirAnswer3.Value, myAnswer1) ||
                 MathEx.NotCloseEnough(theirAnswer3.ScaleX, scale.X) ||
                 MathEx.NotCloseEnough(theirAnswer3.ScaleY, scale.Y) ||
                 MathEx.NotCloseEnough(theirAnswer3.ScaleZ, scale.Z) ||
                 failOnPurpose)
            {
                AddFailure("ScaleTransform3D.get/set_Scale* failed");
                Log("*** Expected: {0}", scale);
                Log("*** Actual:   {0},{1},{2}", theirAnswer3.ScaleX, theirAnswer3.ScaleY, theirAnswer3.ScaleZ);
            }

            if (MathEx.NotCloseEnough(theirAnswer4.Value, myAnswer2) ||
                 MathEx.NotCloseEnough(theirAnswer4.ScaleX, scale.X) ||
                 MathEx.NotCloseEnough(theirAnswer4.ScaleY, scale.Y) ||
                 MathEx.NotCloseEnough(theirAnswer4.ScaleZ, scale.Z) ||
                 MathEx.NotCloseEnough(theirAnswer4.CenterX, center.X) ||
                 MathEx.NotCloseEnough(theirAnswer4.CenterY, center.Y) ||
                 MathEx.NotCloseEnough(theirAnswer4.CenterZ, center.Z) ||
                 failOnPurpose)
            {
                AddFailure("ScaleTransform3D.get/set_Scale*/Center* failed");
                Log("*** Expected: Scale = {0}, Center = {1}", scale, center);
                Log("*** Actual:   Scale = {0},{1},{2}, Center = {3},{4},{5}",
                        theirAnswer4.ScaleX, theirAnswer4.ScaleY, theirAnswer4.ScaleZ,
                        theirAnswer4.CenterX, theirAnswer4.CenterY, theirAnswer4.CenterZ);
            }

            if (MathEx.NotCloseEnough(theirAnswer5.Value, myAnswer1) ||
                 MathEx.NotCloseEnough(theirAnswer5.ScaleX, scale.X) ||
                 MathEx.NotCloseEnough(theirAnswer5.ScaleY, scale.Y) ||
                 MathEx.NotCloseEnough(theirAnswer5.ScaleZ, scale.Z) ||
                 failOnPurpose)
            {
                AddFailure("ScaleTransform3D.ctor( double x3 ) failed");
                Log("*** Expected: {0}", scale);
                Log("*** Actual:   {0},{1},{2}", theirAnswer5.ScaleX, theirAnswer5.ScaleY, theirAnswer5.ScaleZ);
            }

            if (MathEx.NotCloseEnough(theirAnswer6.Value, myAnswer2) ||
                 MathEx.NotCloseEnough(theirAnswer6.ScaleX, scale.X) ||
                 MathEx.NotCloseEnough(theirAnswer6.ScaleY, scale.Y) ||
                 MathEx.NotCloseEnough(theirAnswer6.ScaleZ, scale.Z) ||
                 MathEx.NotCloseEnough(theirAnswer6.CenterX, center.X) ||
                 MathEx.NotCloseEnough(theirAnswer6.CenterY, center.Y) ||
                 MathEx.NotCloseEnough(theirAnswer6.CenterZ, center.Z) ||
                 failOnPurpose)
            {
                AddFailure("ScaleTransform3D.ctor( double x6 ) failed");
                Log("*** Expected: Scale = {0}, Center = {1}", scale, center);
                Log("*** Actual:   Scale = {0},{1},{2}, Center = {3},{4},{5}",
                        theirAnswer6.ScaleX, theirAnswer6.ScaleY, theirAnswer6.ScaleZ,
                        theirAnswer6.CenterX, theirAnswer6.CenterY, theirAnswer6.CenterZ);
            }
        }

        private void TestCreateMatrixWith(Matrix3D m)
        {
            MatrixTransform3D theirAnswer = new MatrixTransform3D(m);
            Matrix3D myAnswer = m;

            if (MathEx.NotCloseEnough(theirAnswer.Value, myAnswer) ||
                 failOnPurpose)
            {
                AddFailure("new MatrixTransform3D( Matrix3D ) failed");
                Log("*** Expected =\r\n" + MatrixUtils.ToStr(myAnswer));
                Log("*** Actual   =\r\n" + MatrixUtils.ToStr(theirAnswer.Value));
            }
        }

        private void TestIdentity()
        {
            Log("Testing Transform3D.Identity...");

            Transform3D tx = Transform3D.Identity;
            Transform3D tx2 = Transform3D.Identity;     // They cache values sometimes.

            if (!MatrixUtils.IsIdentity(tx.Value))
            {
                AddFailure("Transform3D.Identity failed. The following should be the identity matrix");
                Log("*** Transform = {0}", MatrixUtils.ToStr(tx.Value));
            }

            if (!MatrixUtils.IsIdentity(tx2.Value))
            {
                AddFailure("Transform3D.Identity failed. The following should be the identity matrix");
                Log("*** Transform = {0}", MatrixUtils.ToStr(tx2.Value));
            }
        }

        private void TestAffinity()
        {
            Log("Testing IsAffine...");

            TestAffinityWith(Const.tt1);
            TestAffinityWith(Const.rtq);
            TestAffinityWith(Const.rtX45);
            TestAffinityWith(Const.st1);
            TestAffinityWith(Const.mtAffine);
            TestAffinityWith(Const.mtNAffine);

            // Transform3DCollectionTest already covers this, so I will not duplicate the effort
        }

        private void TestAffinityWith(Transform3D tx)
        {
            bool isAffine = false;

            if (tx is AffineTransform3D)
            {
                isAffine = true;
            }
            else
            {
                isAffine = MatrixUtils.IsAffine(tx.Value);
            }

            if (isAffine != tx.IsAffine || failOnPurpose)
            {
                AddFailure("Transform3D.IsAffine failed.");
                Log("*** Transform = {0}", MatrixUtils.ToStr(tx.Value));
                Log("*** Expected  = {0}", isAffine);
                Log("*** Actual    = {0}", tx.IsAffine);
            }
        }

        private void TestTransform()
        {
            Log("Testing Transform3D.Transform (6 overloads)...");

            TestTransformWith(Const.tt10, Const.p0);
            TestTransformWith(Const.rtY90, Const.pNeg1);
            TestTransformWith(Const.st10, Const.p10);
            TestTransformWith(Const.tg3, Const.p10);
            TestTransformWith(Const.mtIdent, Const.p1);

            TestTransformWith(Const.tt10, Const.v0);
            TestTransformWith(Const.rtY90, Const.vNeg1);
            TestTransformWith(Const.st10, Const.v10);
            TestTransformWith(Const.tg3, Const.v10);
            TestTransformWith(Const.mtIdent, Const.v1);

            TestTransformWith(Const.ttNeg1, Const.points0);
            TestTransformWith(Const.rtX45, Const.points1);
            TestTransformWith(Const.stNeg1, Const.points5);
            TestTransformWith(Const.tg3, Const.points5);
            TestTransformWith(Const.mtIdent, Const.points5);

            TestTransformWith(Const.ttNeg1, Const.vectors0);
            TestTransformWith(Const.rtX45, Const.vectors1);
            TestTransformWith(Const.stNeg1, Const.vectors5);
            TestTransformWith(Const.tg3, Const.vectors5);
            TestTransformWith(Const.mtIdent, Const.vectors5);

            TestTransformWith(Const.tt1, Const.p4_0);
            TestTransformWith(Const.mtAffine, Const.p4_10);
            TestTransformWith(Const.tg3, Const.p4_1);
            TestTransformWith(Const.mtIdent, Const.p4_10);

            TestTransformWith(Const.tt1, Const.points4_5);
            TestTransformWith(Const.rtZ135, Const.points4_1);
            TestTransformWith(Const.st10, Const.points4_0);
            TestTransformWith(Const.tg3, Const.points4_5);
            TestTransformWith(Const.mtIdent, Const.points4_5);

            RotateTransform3D rtx1 = Const.rtY90;
            RotateTransform3D rtx2 = Const.rtZ135;
            RotateTransform3D rtx3 = Const.rtX45;
            RotateTransform3D rtx4 = Const.rtq;
            ScaleTransform3D stx1 = new ScaleTransform3D(new Vector3D(.5, -.5, .5));
            ScaleTransform3D stx2 = new ScaleTransform3D(new Vector3D(-.5, .7, -.9));
            ScaleTransform3D stx3 = new ScaleTransform3D(new Vector3D(-1.5, 1.5, 1.5));
            ScaleTransform3D stx4 = new ScaleTransform3D(new Vector3D(2.5, 2.3, -2.1));

            Transform3D[] transforms = new Transform3D[] { rtx1, rtx2, rtx3, rtx4, stx1, stx2, stx3, stx4 };

            Point3D[] points = new Point3D[]{
                                        new Point3D( 1,2,3 ), new Point3D( -1,2,-3 ),
                                        new Point3D( .1,.2,.3 ), new Point3D( 120,-340,560 ),
                                        new Point3D( -1,-2,5 ), new Point3D( 6,3,0 )
                                        };
            Vector3D[] vectors = new Vector3D[]{
                                        new Vector3D( 1,1,1 ), new Vector3D( -1,-1,-1 ),
                                        new Vector3D( 2,2,2 ), new Vector3D( -2,2,-2 ),
                                        new Vector3D( .5,.5,.5 ), new Vector3D( -.5,.7,-.9 ),
                                        new Vector3D( 123,234,345 ), new Vector3D( -123,-234,-345 )
                                        };

            foreach (Transform3D tx in transforms)
            {
                foreach (Point3D center in points)
                {
                    ClearCenter(tx);
                    foreach (Point3D p in points)
                    {
                        TestTransformWith(tx, p);
                    }
                    foreach (Vector3D v in vectors)
                    {
                        TestTransformWith(tx, v);
                    }
                    TestTransformWith(tx, points);
                    TestTransformWith(tx, vectors);

                    SetCenter(tx, center);
                    foreach (Point3D p in points)
                    {
                        TestTransformWith(tx, p);
                    }
                    foreach (Vector3D v in vectors)
                    {
                        TestTransformWith(tx, v);
                    }
                    TestTransformWith(tx, points);
                    TestTransformWith(tx, vectors);
                }
            }
        }

        private void ClearCenter(Transform3D tx)
        {
            if (tx is RotateTransform3D)
            {
                tx.ClearValue(RotateTransform3D.CenterXProperty);
                tx.ClearValue(RotateTransform3D.CenterYProperty);
                tx.ClearValue(RotateTransform3D.CenterZProperty);
            }
            else if (tx is ScaleTransform3D)
            {
                tx.ClearValue(ScaleTransform3D.CenterXProperty);
                tx.ClearValue(ScaleTransform3D.CenterYProperty);
                tx.ClearValue(ScaleTransform3D.CenterZProperty);
            }
        }

        private void SetCenter(Transform3D tx, Point3D p)
        {
            if (tx is RotateTransform3D)
            {
                ((RotateTransform3D)tx).CenterX = p.X;
                ((RotateTransform3D)tx).CenterY = p.Y;
                ((RotateTransform3D)tx).CenterZ = p.Z;
            }
            else if (tx is ScaleTransform3D)
            {
                ((ScaleTransform3D)tx).CenterX = p.X;
                ((ScaleTransform3D)tx).CenterY = p.Y;
                ((ScaleTransform3D)tx).CenterZ = p.Z;
            }
        }

        private void TestTransformWith(Transform3D tx, Point3D p)
        {
            Transform3D before = tx.Clone();
            Point3D theirAnswer = tx.Transform(p);
            VerifyUnchanged(before, tx);
            Point3D myAnswer = MatrixUtils.Transform(p, tx.Value);

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("Transform3D.Transform( Point3D ) failed");
                Log("*** Transform =\r\n" + MatrixUtils.ToStr(tx.Value));
                Log("*** Point     = " + p);
                Log("*** Expected = " + myAnswer);
                Log("*** Actual   = " + theirAnswer);
            }
        }

        private void TestTransformWith(Transform3D tx, Vector3D v)
        {
            Transform3D before = tx.Clone();
            Vector3D theirAnswer = tx.Transform(v);
            VerifyUnchanged(before, tx);
            Vector3D myAnswer = MatrixUtils.Transform(v, tx.Value);

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("Transform3D.Transform( Vector3D ) failed");
                Log("*** Transform =\r\n" + MatrixUtils.ToStr(tx.Value));
                Log("*** Vector    = " + v);
                Log("*** Expected = " + myAnswer);
                Log("*** Actual   = " + theirAnswer);
            }
        }

        private void TestTransformWith(Transform3D tx, Point3D[] points)
        {
            Transform3D before = tx.Clone();
            Point3D[] theirAnswer = new Point3D[points.Length];
            bool failed = false;

            for (int n = 0; n < points.Length; n++)
            {
                theirAnswer[n] = points[n];
            }

            tx.Transform(theirAnswer);
            VerifyUnchanged(before, tx);

            for (int n = 0; n < points.Length; n++)
            {
                Point3D myAnswer = MatrixUtils.Transform(points[n], tx.Value);
                if (!failed && (theirAnswer[n] != myAnswer || failOnPurpose))
                {
                    AddFailure("Transform3D.Transform( Point3D[] ) failed");
                    Log("*** Transform =\r\n" + MatrixUtils.ToStr(tx.Value));
                    Log("*** Point     = " + points[n]);
                    Log("*** Expected = " + myAnswer);
                    Log("*** Actual   = " + theirAnswer[n]);
                    failed = true;
                }
            }
        }

        private void TestTransformWith(Transform3D tx, Vector3D[] vectors)
        {
            Transform3D before = tx.Clone();
            Vector3D[] theirVectors = new Vector3D[vectors.Length];
            Vector3D[] myVectors = new Vector3D[vectors.Length];
            bool failed = false;

            for (int n = 0; n < vectors.Length; n++)
            {
                theirVectors[n] = vectors[n];
                myVectors[n] = MatrixUtils.Transform(vectors[n], tx.Value);
            }

            tx.Transform(theirVectors);
            VerifyUnchanged(before, tx);

            for (int n = 0; n < vectors.Length; n++)
            {
                if (!failed && (theirVectors[n] != myVectors[n] || failOnPurpose))
                {
                    AddFailure("Transform3D.Transform( Vector3D[] ) failed");
                    Log("*** Transform =\r\n" + MatrixUtils.ToStr(tx.Value));
                    Log("*** Point     = " + vectors[n]);
                    Log("*** Expected = " + myVectors[n]);
                    Log("*** Actual   = " + theirVectors[n]);
                    failed = true;
                }
            }
        }

        private void TestTransformWith(Transform3D tx, Point4D p)
        {
            Transform3D before = tx.Clone();
            Point4D theirAnswer = tx.Transform(p);
            VerifyUnchanged(before, tx);
            Point4D myAnswer = MatrixUtils.Transform(p, tx.Value);

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("Transform3D.Transform( Point4D ) failed");
                Log("*** Transform =\r\n" + MatrixUtils.ToStr(tx.Value));
                Log("*** Point     = " + p);
                Log("*** Expected = " + myAnswer);
                Log("*** Actual   = " + theirAnswer);
            }
        }

        private void TestTransformWith(Transform3D tx, Point4D[] points)
        {
            Transform3D before = tx.Clone();
            Point4D[] theirPoints = new Point4D[points.Length];
            Point4D[] myPoints = new Point4D[points.Length];
            bool failed = false;

            for (int n = 0; n < points.Length; n++)
            {
                theirPoints[n] = points[n];
                myPoints[n] = MatrixUtils.Transform(points[n], tx.Value);
            }

            tx.Transform(theirPoints);
            VerifyUnchanged(before, tx);

            for (int n = 0; n < points.Length; n++)
            {
                if (!failed && (theirPoints[n] != myPoints[n] || failOnPurpose))
                {
                    AddFailure("Transform3D.Transform( Point4D[] ) failed");
                    Log("*** Transform =\r\n" + MatrixUtils.ToStr(tx.Value));
                    Log("*** Point     = " + points[n]);
                    Log("*** Expected = " + myPoints[n]);
                    Log("*** Actual   = " + theirPoints[n]);
                    failed = true;
                }
            }
        }

        private void VerifyUnchanged(Transform3D before, Transform3D after)
        {
            if (MathEx.NotCloseEnough(before.Value, after.Value) || failOnPurpose)
            {
                AddFailure("Call to Transform caused a change in Transform3D");
                Log("*** Expected:\n" + MatrixUtils.ToStr(before.Value));
                Log("***   Actual:\n" + MatrixUtils.ToStr(before.Value));
            }
        }

        private void TestMatrixTransformProperties()
        {
            Log("Testing MatrixTransform.Matrix...");

            TestMatrixTransformPropertiesWith(Const.mIdent);
            TestMatrixTransformPropertiesWith(Const.mAffine);
            TestMatrixTransformPropertiesWith(Const.mNAffine);
        }

        private void TestMatrixTransformPropertiesWith(Matrix3D m)
        {
            MatrixTransform3D tx = new MatrixTransform3D();
            tx.Matrix = m;

            if (tx.Matrix != m || failOnPurpose)
            {
                AddFailure("MatrixTransform.Matrix failed");
                Log("*** Expected: " + MatrixUtils.ToStr(m));
                Log("*** Actual:   " + MatrixUtils.ToStr(tx.Matrix));
            }
        }

        private void RunTheTest2()
        {
            TestTransform2();
            TestCreateRotation2();
        }

        private void TestTransform2()
        {
            Log("Testing Transform3D.Transform (6 overloads) with Bad Params...");

            SafeExecute(NonAffineTransformPoint3D);
            SafeExecute(NonAffineTransformPoint3DCollection);
            SafeExecute(NonAffineTransformVector3D);
            SafeExecute(NonAffineTransformVector3DCollection);
            SafeExecute(NonAffineTransformPoint4D);
            SafeExecute(NonAffineTransformPoint4DCollection);
        }

        private void NonAffineTransformPoint3D()
        {
            TestTransformWith(Const.mtNAffine, Const.p0);
        }

        private void NonAffineTransformPoint3DCollection()
        {
            TestTransformWith(Const.mtNAffine, Const.points5);
        }

        private void NonAffineTransformVector3D()
        {
            TestTransformWith(Const.mtNAffine, Const.v1);
        }

        private void NonAffineTransformVector3DCollection()
        {
            TestTransformWith(Const.mtNAffine, Const.vectors5);
        }

        private void NonAffineTransformPoint4D()
        {
            TestTransformWith(Const.mtNAffine, Const.p4_10);
        }

        private void NonAffineTransformPoint4DCollection()
        {
            TestTransformWith(Const.mtNAffine, Const.points4_5);
        }

        private void TestCreateRotation2()
        {
            Log("Testing new RotateTransform3D( Rotation3D ) with Bad Params...");

            // null Rotation3D means Identity
            RotateTransform3D theirAnswer1 = new RotateTransform3D(null);
            RotateTransform3D theirAnswer2 = new RotateTransform3D(null, Const.p1);

            if (!MatrixUtils.IsIdentity(theirAnswer1.Value) || failOnPurpose)
            {
                AddFailure("Creating RotateTransform3D with null Rotation3D should result in Identity transform.");
                Log("Center was not set.");
            }

            if (!MatrixUtils.IsIdentity(theirAnswer2.Value) || failOnPurpose)
            {
                AddFailure("Creating RotateTransform3D with null Rotation3D should result in Identity transform.");
                Log("Center was set.");
            }
        }
    }
}
