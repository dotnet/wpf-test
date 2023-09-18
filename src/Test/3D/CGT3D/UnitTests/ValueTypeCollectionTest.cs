// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Graphics.Factories;

// Subnamespace "UnitTests" is required for this case to be picked up by /RunAll
namespace Microsoft.Test.Graphics.UnitTests
{
    /// <summary>
    /// Test ValueType MIL Collection specific APIs
    /// </summary>
    public class ValueTypeCollectionTest : CoreGraphicsTest
    {
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
                TestPoint3DParse();
                TestVector3DParse();

                // This one's not a ValueType collection, but it acts like one because it has a Parse method
                TestGradientStopParse();
            }
        }

        private void TestPoint3DParse()
        {
            Log("Testing Point3DCollection.Parse...");

            TestPoint3DParseWith(new string[] { string.Empty });
            TestPoint3DParseWith(new string[] { "0" + _sep + "0" + _sep + "0", "1" + _sep + "1" + _sep + "1", "2" + _sep + "2" + _sep + "2" });
            TestPoint3DParseWith(new string[] { "0.0" + _sep + "0.1" + _sep + "0.2", ".22" + _sep + ".23" + _sep + ".24" });
            TestPoint3DParseWith(new string[]{
                                        "1e-12"+_sep+"1e+12"+_sep+"1e-1",
                                        "1.02e-1"+_sep+"1.99e+34"+_sep+"23",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2" }
                                        );
        }

        private void TestPoint3DParseWith(string[] points)
        {
            string global = MathEx.ToLocale(string.Join(" ", points), CultureInfo.InvariantCulture);
            Point3DCollection theirAnswer = Point3DCollection.Parse(global);

            Point3DCollection myAnswer = new Point3DCollection();
            if (points.Length > 0 && points[0] != string.Empty)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    string invariant = MathEx.ToLocale(points[i], CultureInfo.InvariantCulture);
                    myAnswer.Add(StringConverter.ToPoint3D(invariant));
                }
            }

            if (MathEx.NotEquals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Point3DCollection.Parse( string ) failed");
                Log("*** Original String = {0}", global);
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }
        }

        private void TestVector3DParse()
        {
            Log("Testing Vector3DCollection.Parse...");

            TestVector3DParseWith(new string[] { string.Empty });
            TestVector3DParseWith(new string[] { "0" + _sep + "0" + _sep + "0", "1" + _sep + "1" + _sep + "1", "2" + _sep + "2" + _sep + "2" });
            TestPoint3DParseWith(new string[] { "0.0" + _sep + "0.1" + _sep + "0.2", ".22" + _sep + ".23" + _sep + ".24" });
            TestPoint3DParseWith(new string[]{
                                        "1e-12"+_sep+"1e+12"+_sep+"1e-1",
                                        "1.02e-1"+_sep+"1.99e+34"+_sep+"23",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2",
                                        "2"+_sep+"2"+_sep+"2" }
                                        );
        }

        private void TestVector3DParseWith(string[] points)
        {
            string global = MathEx.ToLocale(string.Join(" ", points), CultureInfo.InvariantCulture);
            Vector3DCollection theirAnswer = Vector3DCollection.Parse(global);

            Vector3DCollection myAnswer = new Vector3DCollection();
            if (points.Length > 0 && points[0] != string.Empty)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    string invariant = MathEx.ToLocale(points[i], CultureInfo.InvariantCulture);
                    myAnswer.Add(StringConverter.ToVector3D(invariant));
                }
            }

            if (MathEx.NotEquals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Vector3DCollection.Parse( string ) failed");
                Log("*** Original String = {0}", global);
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }
        }

        private void TestGradientStopParse()
        {
            Log("Testing GradientStopCollection.Parse...");

            TestGradientStopParseWith("red 0.0");
            TestGradientStopParseWith("blue" + _sep + "0.25");
            TestGradientStopParseWith("#ffabcdef 1.0");
            TestGradientStopParseWith("#ffabcdef" + _sep + "1.0");
            TestGradientStopParseWith("red 0.0 blue 0.25 green 0.50 orange 0.75 black 1.0");
            TestGradientStopParseWith("red" + _sep + "0.0 blue 0.25 green" + _sep + "0.50 orange 0.75 black" + _sep + "1.0");
            TestGradientStopParseWith("red" + _sep + "0.0 blue" + _sep + "0.25 green" + _sep + "0.50 orange" + _sep + "0.75 black" + _sep + "1.0");
        }

        private void TestGradientStopParseWith(string s)
        {
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            GradientStopCollection theirAnswer = GradientStopCollection.Parse(global);

            string[] tokens = s.Split(new char[] { ' ', _sep }, StringSplitOptions.RemoveEmptyEntries);
            GradientStopCollection myAnswer = new GradientStopCollection();
            for (int i = 0; i < tokens.Length; i += 2)
            {
                string color = tokens[i];
                string offset = MathEx.ToLocale(tokens[i + 1], CultureInfo.InvariantCulture);
                GradientStop g = MakeGradientStop(color, offset);
                myAnswer.Add(g);
            }

            if (!ObjectUtils.DeepEquals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("GradientStopCollection.Parse( string ) failed");
                Log("*** Original String: {0}", global);
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual = {0}", theirAnswer);
            }
        }

        private GradientStop MakeGradientStop(string color, string offset)
        {
            Color c = (Color)ColorConverter.ConvertFromString(color);
            double d = StringConverter.ToDouble(offset);

            return new GradientStop(c, d);
        }

        private void RunTheTest2()
        {
            TestPoint3DParse2();
            TestVector3DParse2();
            TestGradientStopParse2();
        }

        private void TestPoint3DParse2()
        {
            Log("Testing Point3DCollection.Parse with Bad Params...");

            TestPoint3DParseWith(new string[] { "NaN" + _sep + "Infinity" + _sep + "-Infinity" });
        }

        private void TestVector3DParse2()
        {
            Log("Testing Vector3DCollection.Parse with Bad Params...");

            TestVector3DParseWith(new string[] { "NaN" + _sep + "Infinity" + _sep + "-Infinity" });
        }

        private void TestGradientStopParse2()
        {
            Log("Testing GradientStopCollection.Parse with Bad Params...");

            TestGradientStopParseWith("");
            TestGradientStopParseWith(string.Empty);
            TestGradientStopParseWith("violet NaN");
            TestGradientStopParseWith("lightblue Infinity");
            TestGradientStopParseWith("pink -Infinity");
        }
    }
}