// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Media.Media3D;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Graphics.Factories;

// Subnamespace "UnitTests" is required for this case to be picked up by /RunAll
namespace Microsoft.Test.Graphics.UnitTests
{
    /// <summary/>
    public class QuaternionTest : CoreGraphicsTest
    {
        private double _eps = Const.eps;
        private double _min = Const.min;
        private double _max = Const.max;
        private double _inf = Const.inf;
        private double _nan = Const.nan;
        private char _sep = Const.valueSeparator;

        /// <summary/>
        public override void RunTheTest()
        {
            if (priority > 0)
            {
                RunTheTest2();
            }
            else
            {
                TestCtor();
                TestIdentity();
                TestIsIdentity();
                TestProperties();
                TestConjugate();
                TestInvert();
                TestNormalize();
                TestAdd();
                TestSubtract();
                TestMultiply();
                TestSlerp();
                TestEquality();
                TestParse();
                TestToString();
                TestGetHashCode();
            }
        }

        private void TestCtor()
        {
            Log("Testing constructors...");

            Quaternion q = new Quaternion();

            if (q.X != 0 || q.Y != 0 || q.Z != 0 || q.W != 1 || failOnPurpose)
            {
                AddFailure("Default ctor for Quaternion failed.");
                Log("*** Expected: 0,0,0,1");
                Log("***   Actual: {0}", q);
            }

            TestCtorWith(0, 0, 0, 0);
            TestCtorWith(-25, 100, 2, 15);
            TestCtorWith(0, 0, _min, -25);
            TestCtorWith(Const.v10, 0);
            TestCtorWith(Const.vNeg1, 90);
            TestCtorWith(Const.xAxis, 520);
        }

        private void TestCtorWith(double x, double y, double z, double w)
        {
            Quaternion q = new Quaternion(x, y, z, w);

            if (MathEx.NotEquals(q.X, x) || MathEx.NotEquals(q.Y, y) ||
                 MathEx.NotEquals(q.Z, z) || MathEx.NotEquals(q.W, w) ||
                 failOnPurpose)
            {
                AddFailure("ctor for Quaternion with x, y, z, w failed.");
                Log("*** Expected: {0},{1},{2},{3}", x, y, z, w);
                Log("***   Actual: {0}", q);
            }
        }

        private void TestCtorWith(Vector3D v, double a)
        {
            Quaternion theirAnswer = new Quaternion(v, a);
            Quaternion myAnswer = MathEx.ToQuaternion(v, a);

            if (MathEx.NotCloseEnough(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("ctor for Quaternion with axis, angle failed.");
                Log("*** Axis  = {0}", v);
                Log("*** Angle = {0}", a);
                Log("*** Expected: {0}", myAnswer);
                Log("***   Actual: {0}", theirAnswer);
            }
        }

        private void TestIdentity()
        {
            Log("Testing Identity...");

            Quaternion q = Quaternion.Identity;

            if (q.X != 0 || q.Y != 0 || q.Z != 0 || q.W != 1 || failOnPurpose)
            {
                AddFailure("Quaternion.Identity failed.");
                Log("*** Expected: 0,0,0,1");
                Log("***   Actual: {0}", q);
            }

            // New quaternion should also be the identity (but is tested by constructor test)
        }

        private void TestIsIdentity()
        {
            Log("Testing IsIdentity...");

            TestIsIdentityWith(Quaternion.Identity);
            TestIsIdentityWith(new Quaternion());
            TestIsIdentityWith(Const.q0);
            TestIsIdentityWith(Const.q1);
        }

        private void TestIsIdentityWith(Quaternion q)
        {
            bool theirAnswer = q.IsIdentity;
            bool myAnswer = MathEx.IsIdentity(q);

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("IsIdentity failed");
                Log("*** Quaternion: {0}", q);
                Log("***   Expected: {0}", myAnswer);
                Log("***     Actual: {0}", theirAnswer);
            }
        }

        private void TestProperties()
        {
            Log("Testing XYZW get/set...");

            TestXYZWWith(0, 0, 0, 1);
            TestXYZWWith(10.23457, -10.83549, 100.2348590, 1109);
            TestXYZWWith(_max, _min, _eps, -_eps);

            Log("Testing Axis/Angle get...");

            TestAxisAngleWith(new Quaternion(0, 1, 0, 0));
            TestAxisAngleWith(new Quaternion(1, 0, 0, 0));
            TestAxisAngleWith(new Quaternion(0, 0, 1, 0));
            TestAxisAngleWith(Const.qX45);
            TestAxisAngleWith(Const.qX540);
            TestAxisAngleWith(new Quaternion(Const.v10, -89));
            TestAxisAngleWith(Const.qX360);
            TestAxisAngleWith(Quaternion.Identity);
            TestAxisAngleWith(new Quaternion());
            TestAxisAngleWith(Const.q1);
            TestAxisAngleWith(Const.q0);

            Log("Testing IsNormalized get...");

            TestIsNormalizedWith(Const.q0);
            TestIsNormalizedWith(Const.q1);
            TestIsNormalizedWith(Const.qNeg1);
            TestIsNormalizedWith(Const.qX45);
            TestIsNormalizedWith(Const.qX360);
            TestIsNormalizedWith(Quaternion.Identity);
            TestIsNormalizedWith(new Quaternion());
        }

        private void TestXYZWWith(double x, double y, double z, double w)
        {
            Quaternion q = new Quaternion();
            q.X = x;
            q.Y = y;
            q.Z = z;
            q.W = w;

            if (MathEx.NotEquals(q, new Quaternion(x, y, z, w)) || failOnPurpose)
            {
                AddFailure("At least one Property for Quaternion failed to set.");
                Log("*** Expected = ( {0}, {1}, {2}, {3} )", x, y, z, w);
                Log("*** Actual   = ( {0}, {1}, {2}, {3} )", q.X, q.Y, q.Z, q.W);
            }
        }

        private void TestAxisAngleWith(Quaternion q)
        {
            Vector3D theirAxis = q.Axis;
            double theirAngle = q.Angle;

            AxisAngleRotation3D rotation = MathEx.ToAxisAngle(q);
            Vector3D myAxis = rotation.Axis;
            double myAngle = rotation.Angle;

            if (MathEx.NotCloseEnough(theirAxis, myAxis) ||
                 MathEx.NotCloseEnough(theirAngle, myAngle) ||
                 failOnPurpose)
            {
                AddFailure("Axis/Angle get failed.");
                Log("*** Quaternion: {0}", q);
                Log("***   Expected: axis = {0}, angle = {1}", myAxis, myAngle);
                Log("***     Actual: axis = {0}, angle = {1}", theirAxis, theirAngle);
            }
        }

        private void TestIsNormalizedWith(Quaternion q)
        {
            double lengthSq = MathEx.LengthSquared(q.X, q.Y, q.Z, q.W);
            bool isNormalized = MathEx.AreCloseEnough(lengthSq, 1.0);

            if (q.IsNormalized != isNormalized || failOnPurpose)
            {
                AddFailure("IsNormalized failed.");
                Log("***     Quaternion: {0}", q);
                Log("*** Length Squared: {0}", lengthSq);
            }
        }

        private void TestConjugate()
        {
            Log("Testing Conjugate...");

            TestConjugateWith(Const.q0);
            TestConjugateWith(Const.q1);
            TestConjugateWith(Const.qNeg1);
            TestConjugateWith(Const.qX135);
            TestConjugateWith(Const.qX540);
            TestConjugateWith(Const.qX360);
            TestConjugateWith(Quaternion.Identity);
            TestConjugateWith(new Quaternion());
        }

        private void TestConjugateWith(Quaternion q)
        {
            Quaternion theirAnswer = q;
            theirAnswer.Conjugate();

            // For those unfamiliar with Conjugation, the W parameter is not negated
            Quaternion myAnswer = new Quaternion(-q.X, -q.Y, -q.Z, q.W);

            if (MathEx.NotEquals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Conjugate failed");
                Log("*** Expected: {0}", myAnswer);
                Log("***   Actual: {0}", theirAnswer);
            }
        }

        private void TestInvert()
        {
            Log("Testing Invert...");

            TestInvertWith(Const.q0);
            TestInvertWith(Const.q1);
            TestInvertWith(Const.qX135);
            TestInvertWith(Const.qX540);
            TestInvertWith(Const.qZ45);
            TestInvertWith(Quaternion.Identity);
            TestInvertWith(new Quaternion());
        }

        private void TestInvertWith(Quaternion q)
        {
            Quaternion theirAnswer = q;
            try
            {
                theirAnswer.Invert();
            }
            catch (InvalidOperationException ex)
            {
                if (MathEx.NotCloseEnough(q, Const.q0) || failOnPurpose)
                {
                    AddFailure("Invert failed on non-zero quaternion\n" + ex);
                    Log("*** Quaternion: {0}", q);
                }
                return;
            }

            //                                                                         __________________
            //           -1   _        2          _                                _  / 2    2    2    2
            // Inverse: q   = q / ||q||     where q = conjugate of q,   and ||q|| = \/ x  + y  + z  + w

            double x = -q.X;
            double y = -q.Y;
            double z = -q.Z;
            double w = q.W;
            double norm = MathEx.LengthSquared(x, y, z, w);
            Quaternion myAnswer = new Quaternion(x / norm, y / norm, z / norm, w / norm);

            if (MathEx.NotCloseEnough(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Invert failed");
                Log("*** Quaternion: {0}", q);
                Log("***   Expected: {0}", myAnswer);
                Log("***     Actual: {0}", theirAnswer);
            }
        }

        private void TestNormalize()
        {
            Log("Testing Normalize...");

            TestNormalizeWith(Const.q0);
            TestNormalizeWith(Const.q1);
            TestNormalizeWith(Const.qNeg1);
            TestNormalizeWith(Const.qX135);
            TestNormalizeWith(new Quaternion(-10, 100, 23, 200));
            TestNormalizeWith(Quaternion.Identity);
            TestNormalizeWith(new Quaternion());
        }

        private void TestNormalizeWith(Quaternion q)
        {
            Quaternion theirAnswer = q;
            try
            {
                theirAnswer.Normalize();
            }
            catch (InvalidOperationException ex)
            {
                if (MathEx.NotCloseEnough(q, Const.q0) || failOnPurpose)
                {
                    AddFailure("Normalize failed with non-zero quaternion\n" + ex);
                    Log("*** Quaternion: {0}", q);
                }
                return;
            }

            Quaternion myAnswer = MathEx.Normalize(q);

            if (MathEx.NotCloseEnough(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Normalize failed");
                Log("*** Original: {0}", q);
                Log("*** Expected: {0}", myAnswer);
                Log("***   Actual: {0}", theirAnswer);
            }
        }

        private void TestAdd()
        {
            Log("Testing Addition...");

            TestAddWith(Const.q0, Const.q0);
            TestAddWith(Const.q1, Const.qNeg1);
            TestAddWith(Const.qX45, Const.qX90);
            TestAddWith(Const.qX135, Const.qX540);
            TestAddWith(Quaternion.Identity, Const.qY90);
            TestAddWith(Quaternion.Identity, Quaternion.Identity);
            TestAddWith(Const.qZ135, Quaternion.Identity);
            TestAddWith(new Quaternion(), Const.qY90);
            TestAddWith(new Quaternion(), new Quaternion());
            TestAddWith(Const.qZ135, new Quaternion());
        }

        private void TestAddWith(Quaternion q1, Quaternion q2)
        {
            Quaternion theirAnswer1 = q1 + q2;
            Quaternion theirAnswer2 = Quaternion.Add(q1, q2);
            Quaternion myAnswer = new Quaternion(q1.X + q2.X, q1.Y + q2.Y, q1.Z + q2.Z, q1.W + q2.W);

            if (MathEx.NotCloseEnough(theirAnswer1, myAnswer) || failOnPurpose)
            {
                AddFailure("operator + failed");
                Log("*** Expected: {0}", myAnswer);
                Log("***   Actual: {0}", theirAnswer1);
            }
            if (MathEx.NotCloseEnough(theirAnswer2, myAnswer) || failOnPurpose)
            {
                AddFailure("Quaternion.Add failed");
                Log("*** Expected: {0}", myAnswer);
                Log("***   Actual: {0}", theirAnswer2);
            }
        }

        private void TestSubtract()
        {
            Log("Testing Subtraction...");

            TestSubtractWith(Const.q0, Const.q0);
            TestSubtractWith(Const.q1, Const.qNeg1);
            TestSubtractWith(Const.qX45, Const.qX90);
            TestSubtractWith(Const.qX135, Const.qX540);
            TestSubtractWith(Quaternion.Identity, Const.qY90);
            TestSubtractWith(Quaternion.Identity, Quaternion.Identity);
            TestSubtractWith(Const.qZ135, Quaternion.Identity);
            TestSubtractWith(new Quaternion(), Const.qY90);
            TestSubtractWith(new Quaternion(), new Quaternion());
            TestSubtractWith(Const.qZ135, new Quaternion());
        }

        private void TestSubtractWith(Quaternion q1, Quaternion q2)
        {
            Quaternion theirAnswer1 = q1 - q2;
            Quaternion theirAnswer2 = Quaternion.Subtract(q1, q2);
            Quaternion myAnswer = new Quaternion(q1.X - q2.X, q1.Y - q2.Y, q1.Z - q2.Z, q1.W - q2.W);

            if (MathEx.NotCloseEnough(theirAnswer1, myAnswer) || failOnPurpose)
            {
                AddFailure("operator - failed");
                Log("*** Expected: {0}", myAnswer);
                Log("***   Actual: {0}", theirAnswer1);
            }
            if (MathEx.NotCloseEnough(theirAnswer2, myAnswer) || failOnPurpose)
            {
                AddFailure("Quaternion.Subtract failed");
                Log("*** Expected: {0}", myAnswer);
                Log("***   Actual: {0}", theirAnswer2);
            }
        }

        private void TestMultiply()
        {
            Log("Testing Multiplication...");

            TestMultiplyWith(Const.q0, Const.q0);
            TestMultiplyWith(Const.q1, Const.qNeg1);
            TestMultiplyWith(Const.qX45, Const.qX90);
            TestMultiplyWith(Const.qX135, Const.qX540);
            TestMultiplyWith(Quaternion.Identity, Const.qY90);
            TestMultiplyWith(Quaternion.Identity, Quaternion.Identity);
            TestMultiplyWith(Const.qZ135, Quaternion.Identity);
            TestMultiplyWith(new Quaternion(), Const.qY90);
            TestMultiplyWith(new Quaternion(), new Quaternion());
            TestMultiplyWith(Const.qZ135, new Quaternion());
        }

        private void TestMultiplyWith(Quaternion q1, Quaternion q2)
        {
            Quaternion theirAnswer1 = q1 * q2;
            Quaternion theirAnswer2 = Quaternion.Multiply(q1, q2);
            Quaternion myAnswer = MathEx.Multiply(q1, q2);

            if (MathEx.NotCloseEnough(theirAnswer1, myAnswer) || failOnPurpose)
            {
                AddFailure("operator * failed");
                Log("*** Expected: {0}", myAnswer);
                Log("***   Actual: {0}", theirAnswer1);
            }
            if (MathEx.NotCloseEnough(theirAnswer2, myAnswer) || failOnPurpose)
            {
                AddFailure("Quaternion.Multiply failed");
                Log("*** Expected: {0}", myAnswer);
                Log("***   Actual: {0}", theirAnswer2);
            }
        }

        private void TestSlerp()
        {
            Log("Testing Slerp...");

            TestSlerpWith(Const.qX45, Const.qX90, 0.0);       // t = 0
            TestSlerpWith(Const.qX45, Const.qX90, 1.0);       // t = 1
            TestSlerpWith(Const.qX90, Const.qX90, .5);        // same Q
            TestSlerpWith(Const.qX45, Const.qX90, .5);        // acute angle, t = non-integer
            TestSlerpWith(Const.qX180, Const.qX45, .5);       // obtuse angle, t = non-integer
            TestSlerpWith(Const.qX90, Const.qY90, .33);       // different rotation axes
            TestSlerpWith(Const.qY90, Const.qZ45, .80);       // different rotation axes
            TestSlerpWith(Const.qX45, new Quaternion(-Const.xAxis, 180), .5);   // Take the shortcut instead of long road
            TestSlerpWith(Quaternion.Identity, Const.qX45, 0.0);
            TestSlerpWith(Quaternion.Identity, Const.qX45, 0.5);
            TestSlerpWith(Quaternion.Identity, Const.qX45, 1.0);
            TestSlerpWith(Const.qX45, Quaternion.Identity, 0.0);
            TestSlerpWith(Const.qX45, Quaternion.Identity, 0.5);
            TestSlerpWith(Const.qX45, Quaternion.Identity, 1.0);
            TestSlerpWith(new Quaternion(), Const.qX45, 0.0);
            TestSlerpWith(new Quaternion(), Const.qX45, 0.5);
            TestSlerpWith(new Quaternion(), Const.qX45, 1.0);
            TestSlerpWith(Const.qX45, new Quaternion(), 0.0);
            TestSlerpWith(Const.qX45, new Quaternion(), 0.5);
            TestSlerpWith(Const.qX45, new Quaternion(), 1.0);
            TestSlerpWith(Quaternion.Identity, Const.qX180, 0.45);                        // 180 degree rotation (from identity)
            TestSlerpWith(Const.qX45, new Quaternion(Const.xAxis, 225), 0.5);           // 180 degree rotation
            TestSlerpWith(Const.qX45, new Quaternion(Const.xAxis, 224.999999), 0.5);    // ~180 degree rotation
            TestSlerpWith(Const.qX45, new Quaternion(Const.xAxis, 225.000001), 0.5);    // ~180 degree rotation
            TestSlerpWith(new Quaternion(Const.yAxis, 0), new Quaternion(Const.yAxis, 270), 0.33);

            TestSlerpWith(new Quaternion(Const.yAxis, 0), new Quaternion(Const.yAxis, 359.75), 0.33);
            TestSlerpWith(new Quaternion(Const.yAxis, 0), new Quaternion(Const.yAxis, 359.999), 0.5);

            TestSlerpWith(new Quaternion(0, 0, 0, 1), new Quaternion(0, 0, 0, -1), 0.4);
            TestSlerpWith(new Quaternion(1, 0, 0, 0), new Quaternion(-1, 0, 0, 0), 0.9);
            TestSlerpWith(new Quaternion(Const.RootTwo, 0, Const.RootTwo, 0), new Quaternion(-Const.RootTwo, 0, -Const.RootTwo, 0), 0.2);
            TestSlerpWith(new Quaternion(.5, .5, .5, .5), new Quaternion(-.5, -.5, -.5, -.5), 0.8333);
        }

        private void TestSlerpWith(Quaternion q1, Quaternion q2, double t)
        {
            Quaternion theirAnswer1 = Quaternion.Slerp(q1, q2, t, false);
            Quaternion theirAnswer2 = Quaternion.Slerp(q1, q2, t, true);
            Quaternion theirAnswer3 = Quaternion.Slerp(q1, q2, t);

            Quaternion myAnswer1 = MathEx.Slerp(q1, q2, t, false);
            Quaternion myAnswer2 = MathEx.Slerp(q1, q2, t, true);

            if (MathEx.NotCloseEnough(theirAnswer1, myAnswer1) || failOnPurpose)
            {
                AddFailure("Quaternion.Slerp failed for useShortestPath == false");
                Log("*** From Quaternion: {0}", q1);
                Log("***   To Quaternion: {0}", q2);
                Log("***        Interval: {0}", t);
                Log("*** Expected: {0}", myAnswer1);
                Log("***   Actual: {0}", theirAnswer1);
            }

            if (MathEx.NotCloseEnough(theirAnswer2, myAnswer2) || failOnPurpose)
            {
                AddFailure("Quaternion.Slerp failed for useShortestPath == true");
                Log("*** From Quaternion: {0}", q1);
                Log("***   To Quaternion: {0}", q2);
                Log("***        Interval: {0}", t);
                Log("*** Expected: {0}", myAnswer2);
                Log("***   Actual: {0}", theirAnswer2);
            }

            if (MathEx.NotCloseEnough(theirAnswer2, theirAnswer3) || failOnPurpose)
            {
                AddFailure("Quaternion.Slerp failed.  useShortestPath should default to true.");
                Log("*** From Quaternion: {0}", q1);
                Log("***   To Quaternion: {0}", q2);
                Log("***        Interval: {0}", t);
                Log("*** Expected: {0}", theirAnswer2);
                Log("***   Actual: {0}", theirAnswer3);
            }
        }

        private void TestEquality()
        {
            Log("Testing Equality...");

            TestEqualityWith(Const.q0, Const.q0);
            TestEqualityWith(Const.q1, Const.q1);
            TestEqualityWith(Const.qX180, Const.qX180);
            TestEqualityWith(new Quaternion(1, 2, 3, 4), new Quaternion(2, 2, 3, 4));
            TestEqualityWith(new Quaternion(1, 2, 3, 4), new Quaternion(1, 3, 3, 4));
            TestEqualityWith(new Quaternion(1, 2, 3, 4), new Quaternion(1, 2, 4, 4));
            TestEqualityWith(new Quaternion(1, 2, 3, 4), new Quaternion(1, 2, 3, 5));
            TestEqualityWith(Quaternion.Identity, Const.q1);
            TestEqualityWith(Quaternion.Identity, Quaternion.Identity);
            TestEqualityWith(Const.qX135, Quaternion.Identity);
            TestEqualityWith(new Quaternion(), Const.q1);
            TestEqualityWith(new Quaternion(), new Quaternion());
            TestEqualityWith(Const.qX135, new Quaternion());
        }

        private void TestEqualityWith(Quaternion q1, Quaternion q2)
        {
            bool theirAnswer1 = Quaternion.Equals(q1, q2);
            bool theirAnswer2 = q1.Equals(q2);
            bool theirAnswer3 = q1 == q2;
            bool theirNotAnswer3 = q1 != q2;
            bool myAnswer12 = !MathEx.NotEquals(q1, q2);
            bool myAnswer3 = !MathEx.ClrOperatorNotEquals(q1, q2);

            if (theirAnswer1 != myAnswer12 || failOnPurpose)
            {
                AddFailure("Quaternion.Equals failed.");
                Log("*** Quaternion 1 = {0}", q1);
                Log("*** Quaternion 2 = {0}", q2);
                Log("*** Expected = {0}", myAnswer12);
                Log("*** Actual   = {0}", theirAnswer1);
            }

            if (theirAnswer2 != myAnswer12 || failOnPurpose)
            {
                AddFailure("Equals( object ) failed.");
                Log("*** Quaternion 1 = {0}", q1);
                Log("*** Quaternion 2 = {0}", q2);
                Log("*** Expected = {0}", myAnswer12);
                Log("*** Actual   = {0}", theirAnswer2);
            }

            if (theirAnswer3 != myAnswer3 || failOnPurpose)
            {
                AddFailure("operator == failed.");
                Log("*** Quaternion 1 = {0}", q1);
                Log("*** Quaternion 2 = {0}", q2);
                Log("*** Expected = {0}", myAnswer3);
                Log("*** Actual   = {0}", theirAnswer3);
            }

            if (theirNotAnswer3 == myAnswer3 || failOnPurpose)
            {
                AddFailure("operator != failed.");
                Log("*** Quaternion 1 = {0}", q1);
                Log("*** Quaternion 2 = {0}", q2);
                Log("*** Expected = {0}", myAnswer3);
                Log("*** Actual   = {0}", theirNotAnswer3);
            }

            if (priority > 0)
            {
                bool theirAnswer4 = q1.Equals(null);
                bool theirAnswer5 = q1.Equals(new Point4D(q2.X, q2.Y, q2.Z, q2.W));

                if (theirAnswer4 != false || failOnPurpose)
                {
                    AddFailure("Equals( object ) failed.");
                    Log("*** Quaternion = {0}", q1);
                    Log("*** object     = null");
                    Log("*** Expected = false");
                    Log("*** Actual   = {0}", theirAnswer4);
                }

                if (theirAnswer5 != false || failOnPurpose)
                {
                    AddFailure("Equals( object ) failed.");
                    Log("*** Quaternion = {0}", q1);
                    Log("*** Point4D    = {0}", q2);
                    Log("*** Expected = false");
                    Log("*** Actual   = {0}", theirAnswer5);
                }
            }
        }

        private void TestParse()
        {
            Log("Testing Parse...");

            TestParseWith("0" + _sep + "0" + _sep + "0" + _sep + "0");
            TestParseWith("0.0 " + _sep + " -1.0	" + _sep + " 90" + _sep + " 2");
            TestParseWith("1. " + _sep + "	.0 " + _sep + " 90." + _sep + " 89");
            TestParseWith("Identity");
        }

        private void TestParseWith(string s)
        {
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Quaternion q1 = Quaternion.Parse(global);

            string invariant = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Quaternion q2 = StringConverter.ToQuaternion(invariant);

            if (MathEx.NotEquals(q1, q2) || failOnPurpose)
            {
                AddFailure("Quaternion.Parse( string ) failed");
                Log("*** Original String = {0}", global);
                Log("*** Expected = {0}", q2);
                Log("*** Actual   = {0}", q1);
            }
        }

        private void TestToString()
        {
            Log("Testing ToString...");

            TestToStringWith(Const.q0);
            TestToStringWith(Const.q1);
            TestToStringWith(Const.qX540);
            TestToStringWith(Const.q1Norm);
            TestToStringWith(new Quaternion(1, 2, 3, 4));
            TestToStringWith(Quaternion.Identity);
            TestToStringWith(new Quaternion());
        }

        private void TestToStringWith(Quaternion q)
        {
            string theirAnswer = q.ToString();

            // Don't want these to be affected by locale yet
            string myX = q.X.ToString(CultureInfo.InvariantCulture);
            string myY = q.Y.ToString(CultureInfo.InvariantCulture);
            string myZ = q.Z.ToString(CultureInfo.InvariantCulture);
            string myW = q.W.ToString(CultureInfo.InvariantCulture);

            // ... Because of this
            string myAnswer = MathEx.ToLocale(myX + _sep + myY + _sep + myZ + _sep + myW, CultureInfo.CurrentCulture);
            if (MathEx.IsIdentity(q))
            {
                myAnswer = "Identity";
            }

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("Quaternion.ToString() failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }

            theirAnswer = q.ToString(CultureInfo.CurrentCulture);

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("Quaternion.ToString( IFormatProvider ) failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }

            theirAnswer = ((IFormattable)q).ToString("N", CultureInfo.CurrentCulture.NumberFormat);

            // Don't want these to be affected by locale yet
            NumberFormatInfo numberFormat = CultureInfo.InvariantCulture.NumberFormat;
            myX = q.X.ToString("N", numberFormat);
            myY = q.Y.ToString("N", numberFormat);
            myZ = q.Z.ToString("N", numberFormat);
            myW = q.W.ToString("N", numberFormat);

            // ... Because of this
            myAnswer = MathEx.ToLocale(myX + _sep + myY + _sep + myZ + _sep + myW, CultureInfo.CurrentCulture);
            if (MathEx.IsIdentity(q))
            {
                myAnswer = "Identity";
            }

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("Quaternion.ToString( string,IFormatProvider ) failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }
        }

        private void TestGetHashCode()
        {
            Log("Testing GetHashCode...");

            int hash1 = Const.q0.GetHashCode();
            int hash2 = Quaternion.Identity.GetHashCode();
            int hash3 = Const.qX45.GetHashCode();
            int hash4 = Const.q1.GetHashCode();

            if ((hash1 == hash2) && (hash2 == hash3) && (hash3 == hash4) || failOnPurpose)
            {
                AddFailure("GetHashCode failed");
                Log("*** Expected hash function to generate unique hashes.");
            }
        }

        private void RunTheTest2()
        {
            TestCtor2();
            TestProperties2();
            TestConjugate2();
            TestInvert2();
            TestNormalize2();
            TestAdd2();
            TestSubtract2();
            TestMultiply2();
            TestSlerp2();
            TestEquality2();
            TestParse2();
            TestToString2();
        }

        private void TestCtor2()
        {
            Log("Testing constructor with Bad Params...");

            TestCtorWith(Const.vNaN, 0);
            TestCtorWith(Const.zAxis, _nan);
            TestCtorWith(Const.vInf, 0);
            TestCtorWith(Const.xAxis, _inf);
            TestCtorWith(Const.vNegInf, 0);
            TestCtorWith(Const.xAxis, -_inf);
            Try(MakeQuaternionWithZeroAxis, typeof(InvalidOperationException));

            TestCtorWith(_inf, _inf, _inf, _inf);
            TestCtorWith(-_inf, -_inf, -_inf, -_inf);
            TestCtorWith(_nan, _nan, _nan, _nan);
        }

        #region ExceptionThrowers

        private void MakeQuaternionWithZeroAxis() { Quaternion q = new Quaternion(Const.v0, 0); }

        #endregion

        private void TestProperties2()
        {
            Log("Testing XYZW get/set with Bad Params...");

            TestXYZWWith(_nan, _inf, -_inf, _nan);

            Log("Testing Axis/Angle with Bad Params...");

            TestAxisAngleWith(new Quaternion(_nan, 0, 0, 0));
            TestAxisAngleWith(new Quaternion(0, _nan, 0, 0));
            TestAxisAngleWith(new Quaternion(0, 0, _nan, 0));
            TestAxisAngleWith(new Quaternion(_inf, 0, 0, 0));
            TestAxisAngleWith(new Quaternion(0, _inf, 0, 0));
            TestAxisAngleWith(new Quaternion(0, 0, _inf, 0));

            Log("Testing IsNormalized with Bad Params...");

            TestIsNormalizedWith(Const.qNaN);
            TestIsNormalizedWith(Const.qInf);
            TestIsNormalizedWith(Const.qNegInf);
        }

        private void TestConjugate2()
        {
            Log("Testing Conjugate with Bad Params...");

            TestConjugateWith(Const.qNaN);
            TestConjugateWith(Const.qInf);
            TestConjugateWith(Const.qNegInf);
        }

        private void TestInvert2()
        {
            Log("Testing Invert with Bad Params...");

            TestInvertWith(Const.qNaN);
            TestInvertWith(Const.qInf);
            TestInvertWith(Const.qNegInf);
        }

        private void TestNormalize2()
        {
            Log("Testing Normalize with Bad Params...");

            TestNormalizeWith(Const.qNaN);
            TestNormalizeWith(Const.qInf);
            TestNormalizeWith(Const.qNegInf);
            TestNormalizeWith(Const.qMax);
            TestNormalizeWith(Const.qMin);
            TestNormalizeWith(new Quaternion(Math.Sqrt(_max) * 2, 0, 0, 0));

            // Coverage for internal Quaternion function "Max" which is utilized during "Normalize"
            TestNormalizeWith(new Quaternion(_max, 0, 0, 0));
            TestNormalizeWith(new Quaternion(0, _max, 0, 0));
            TestNormalizeWith(new Quaternion(0, 0, _max, 0));
            TestNormalizeWith(new Quaternion(0, 0, 0, _max));
            TestNormalizeWith(new Quaternion(1, 2, 3, _max));
            TestNormalizeWith(new Quaternion(2, 1, 3, _max));
        }

        private void TestAdd2()
        {
            Log("Testing Add with Bad Params...");

            TestAddWith(Const.qNaN, Const.qNaN);
            TestAddWith(Const.qNaN, Const.q1);
            TestAddWith(Const.qInf, Const.qInf);
            TestAddWith(Const.qInf, Const.qNaN);
            TestAddWith(Const.qNegInf, Const.qInf);
            TestAddWith(Const.qNegInf, Const.q1);
        }

        private void TestSubtract2()
        {
            Log("Testing Subtract with Bad Params...");

            TestSubtractWith(Const.qNaN, Const.qNaN);
            TestSubtractWith(Const.qNaN, Const.q1);
            TestSubtractWith(Const.qInf, Const.qInf);
            TestSubtractWith(Const.qInf, Const.qNaN);
            TestSubtractWith(Const.qNegInf, Const.qInf);
            TestSubtractWith(Const.qNegInf, Const.q1);
        }

        private void TestMultiply2()
        {
            Log("Testing Multiply with Bad Params...");

            TestMultiplyWith(Const.qNaN, Const.qNaN);
            TestMultiplyWith(Const.qNaN, Const.q1);
            TestMultiplyWith(Const.qInf, Const.qInf);
            TestMultiplyWith(Const.qInf, Const.qNaN);
            TestMultiplyWith(Const.qNegInf, Const.qInf);
            TestMultiplyWith(Const.qNegInf, Const.q1);
        }

        private void TestSlerp2()
        {
            Log("Testing Slerp with Bad Params...");

            TestSlerpWith(Const.qX45, Const.qY90, -1.0);
            TestSlerpWith(Const.qX45, Const.qY90, 2.0);
            TestSlerpWith(Const.qX45, Const.qY90, 132.345345);
            TestSlerpWith(Const.qX45, Const.qY90, _inf);
            TestSlerpWith(Const.qX45, Const.qY90, -_inf);
            TestSlerpWith(Const.qX45, Const.qY90, _nan);
            TestSlerpWith(Const.qNaN, Const.qY90, 0);
            TestSlerpWith(Const.qNaN, Const.qY90, 0.33);
            TestSlerpWith(Const.qY90, Const.qNaN, 0);
            TestSlerpWith(Const.qY90, Const.qNaN, 0.33);
        }

        private void TestEquality2()
        {
            Log("Testing Equality with Bad Params...");

            TestEqualityWith(Const.qNaN, Const.qNaN);
            TestEqualityWith(Const.qNaN, Const.qInf);
            TestEqualityWith(Const.qInf, Const.qInf);
            TestEqualityWith(Const.qInf, Const.qNegInf);
            TestEqualityWith(Const.qNegInf, Const.qNegInf);
        }

        private void TestParse2()
        {
            Log("Testing Parse with Bad Params...");

            TestParseWith("NaN" + _sep + "Infinity" + _sep + "-Infinity" + _sep + "NaN");
            TestParseWith("-Infinity" + _sep + "NaN" + _sep + "Infinity" + _sep + "-Infinity");
            TestParseWith("Infinity" + _sep + "-Infinity" + _sep + "NaN" + _sep + "Infinity");
        }

        private void TestToString2()
        {
            Log("Testing ToString with Bad Params...");

            TestToStringWith(Const.qNaN);
            TestToStringWith(Const.qInf);
            TestToStringWith(Const.qNegInf);
        }
    }
}