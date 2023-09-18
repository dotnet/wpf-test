// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Graphics.ReferenceRender;

namespace Microsoft.Test.Graphics
{
    /// <summary/>
    public class PathMiniLanguageTest : RenderingTest
    {
        /// <summary/>
        public override void Init(Variation v)
        {
            base.Init(v);

            _randomGenerator = new RandomTools(0);
            _maxRange = 5000;
            if (v["logAllTests"] == null)
            {
                _logEnabled = false;
            }
            else
            {
                _logEnabled = true;
            }
        }

        /// <summary/>
        public override void RunTheTest()
        {
            int maxIterations = 30;

            if (priority == 0)
            {
                TestBoundaryCases();
                ValidInputTest(maxIterations);
            }
            else
            {
                TestNonRenderableRoundTrip();
                TestInvalidBoundaryCases();
                InvalidInputTest(maxIterations);
            }
        }

        /// <summary/>
        public override Visual GetWindowContent()
        {
            if (!_boundaryCase)
            {
                _randomGenerator.KeepClean = true;
                _randomGenerator.ResetCleanFlag();
                if (_isControlTest)
                {
                    _testPath = _randomGenerator.CreateRandomPath(_maxRange);
                    _controlPath = _testPath.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    String check = _testPath.ToString(CultureInfo.InvariantCulture);
                    check = _randomGenerator.PadPath(check);

                    _testPath = (StreamGeometry)_converter.ConvertFromString(check);
                    _actualPath = check;
                }
            }
            else
            {
                String check;

                if (_isControlTest)
                {
                    check = _pathStringData[_testId, 2];
                    _controlPath = check;
                }
                else
                {
                    check = _pathStringData[_testId, 1];
                    _actualPath = check;
                }
                _testPath = (StreamGeometry)_converter.ConvertFromString(check);
            }

            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext c = visual.RenderOpen())
            {
                c.DrawGeometry(Brushes.Green, new Pen(Brushes.Black, 1.0), PreparePathToDisplay(_testPath));
            }
            return visual;
        }

        /// <summary/>
        public override void Verify()
        {

            RenderBuffer renderBuffer = new RenderBuffer(_expectedCapture, BackgroundColor);
            Color toleranceClearColor = RenderTolerance.DefaultColorTolerance;
            toleranceClearColor.A = 0xff;
            renderBuffer.ClearToleranceBuffer(toleranceClearColor);

            renderBuffer.EnsureCorrectBitDepth();

            Color[,] screenCapture = GetScreenCapture();
            int differences = RenderVerifier.VerifyRender(screenCapture, renderBuffer);

            if (differences > 0)
            {
                AddFailure("{0} pixels did not meet the tolerance criteria.", differences);
                RenderBuffer diff = RenderVerifier.ComputeDifference(screenCapture, renderBuffer);
                PhotoConverter.SaveImageAs(screenCapture, logPrefix + _serialId + "_Rendered.png");
                PhotoConverter.SaveImageAs(renderBuffer.FrameBuffer, logPrefix + _serialId + "_Expected_fb.png");
                PhotoConverter.SaveImageAs(renderBuffer.ToleranceBuffer, logPrefix + _serialId + "_Expected_tb.png");
                PhotoConverter.SaveImageAs(diff.ToleranceBuffer, logPrefix + _serialId + "_Diff_tb.png");
                Log("Actual {0}", _actualPath);
                Log("Expected {0}", _controlPath);
            }
            else if (_logEnabled)
            {
                Log("Actual {0}", _actualPath);
                Log("Expected {0}", _controlPath);
            }
        }

        private void TestBoundaryCases()
        {
            _boundaryCase = true;
            Log("Running Visual Path Boundary Case Tests.");

            for (_testId = 0; _testId < _pathStringData.GetLength(0); _testId++)
            {
                _serialId = "_B" + _testId;
                Log(_pathStringData[_testId, 0]);

                _isControlTest = true;
                RenderWindowContent();

                _expectedCapture = GetScreenCapture();

                //render and capture duplicate object
                _isControlTest = false;
                RenderWindowContent();

                VerifyWithinContext();
            }
        }

        private void ValidInputTest(int maxIterations)
        {
            for (int i = 0; i < maxIterations; i++)
            {
                Log("Running Visual Path Automatic Test #{0} of #{1}.", i + 1, maxIterations);
                _serialId = "_A" + _testId;

                _boundaryCase = false;
                _isControlTest = true;
                RenderWindowContent();

                _expectedCapture = GetScreenCapture();

                //render and capture duplicate object
                _isControlTest = false;
                RenderWindowContent();

                VerifyWithinContext();
            }
        }

        private void TestNonRenderableRoundTrip()
        {
            for (int i = 0; i < _roundTripSet.GetLength(0); i++)
            {
                String result;
                try
                {
                    StreamGeometry testPath = (StreamGeometry)_converter.ConvertFromString(_roundTripSet[i, 1]);
                    result = testPath.ToString(CultureInfo.InvariantCulture);
                }
                catch (Exception e)
                {
                    result = e.ToString();
                }
                if (!result.Equals(_roundTripSet[i, 1]))
                {
                    AddFailure("Round-Trip mismatch!");
                    Log("Expected: " + _roundTripSet[i, 1]);
                    Log("Result: " + result);
                }
            }
        }

        private void InvalidInputTest(int maxIterations)
        {
            Log("Running Non-Visual invalid Path parsing tests.");
            for (int i = 0; i < maxIterations; i++)
            {
                //Pared down non-visual test for invalid inputs.
                _testPath = _randomGenerator.CreateRandomPath(_maxRange);

                _boundaryCase = false;
                _randomGenerator.KeepClean = false;
                _randomGenerator.paddingBugProbability = 0.3;

                String check = _testPath.ToString(CultureInfo.InvariantCulture);
                _randomGenerator.ResetCleanFlag();

                check = _randomGenerator.PadPath(check);

                try
                {
                    _testPath = (StreamGeometry)_converter.ConvertFromString(check);
                    if (!_randomGenerator.IsClean)
                    {
                        AddFailure("No failure logged on parsing invalid path.");
                        Log("Using string: {0}", check);
                    }
                    else if (_logEnabled)
                    {
                        Log("Using string: {0}", check);
                    }

                }
                catch (FormatException)
                {
                    if (_randomGenerator.IsClean)
                    {
                        AddFailure("Unexpected FormatException on parse.");
                        Log("Using string: {0}", check);
                        throw;
                    }
                    else if (_logEnabled)
                    {
                        Log("Using string: {0}", check);
                    }
                }
            }
        }

        private void TestInvalidBoundaryCases()
        {
            _boundaryCase = true;
            Log("Running Invalid Path Boundary Case Tests.");

            for (_testId = 0; _testId < _invalidPathStringData.GetLength(0); _testId++)
            {
                Log(_invalidPathStringData[_testId, 0]);
                try
                {
                    _testPath = (StreamGeometry)_converter.ConvertFromString(_invalidPathStringData[_testId, 1]);
                    AddFailure("No exception reported on parsing invalid path.");
                    Log("Using string: {0}", _invalidPathStringData[_testId, 1]);
                }
                catch (Exception e)
                {
                    if ((e.GetType() != typeof(FormatException)) && (e.GetType() != typeof(System.ArgumentException)))
                    {
                        AddFailure("Exception is not of expected exception type:{0}", e.ToString());
                        Log("Using string: {0}", _invalidPathStringData[_testId, 1]);
                    }
                }
            }
        }

        PathGeometry PreparePathToDisplay(Geometry path)
        {

            PathGeometry newPath = new PathGeometry();
            newPath.AddGeometry(path);

            //Translate to 0,0 and resize to window extents
            TransformGroup group = new TransformGroup();
            TransformCollection collection = new TransformCollection();

            collection.Add(new TranslateTransform(-path.Bounds.X, -path.Bounds.Y));
            collection.Add(new ScaleTransform(WindowWidth / ((double)path.Bounds.Width), WindowHeight / (double)path.Bounds.Height));

            group.Children = collection;
            newPath.Transform = group;
            newPath.FillRule = GetFillRule(path);

            return newPath;
        }

        FillRule GetFillRule(Geometry path)
        {
            if (path is PathGeometry)
            {
                return ((PathGeometry)path).FillRule;
            }
            else if (path is StreamGeometry)
            {
                return ((StreamGeometry)path).FillRule;
            }
            else
            {
                throw new Exception("A Geometry of an unexpected type was passed into GetFillRule().");
            }
        }

        /// <summary>
        /// Visual Comparison Test Format:
        /// Description, test path, control path
        /// Data may move to WindowsTestData, once test usage patterns are more certain
        /// </summary>
        private String[,] _pathStringData = new String[,] {

            {"Moderately large box (Renders normally)","M 3.402E8 , 3.402E8 L 3.402E8,-3.402E8 -3.402E8,-3.402E8 -3.402E8,3.402E8 Z","M 3.402E8 , 3.402E8 L -3.402E8,3.402E8 -3.402E8,-3.402E8 3.402E8,-3.402E8 Z"},
            {"largest renderable box. (Renders normally)","M 3.402E38 , 3.402E38 L 3.402E38,-3.402E38 -3.402E38,-3.402E38 -3.402E38,3.402E38 Z","M 3.402E8 , 3.402E8 L -3.402E8,3.402E8 -3.402E8,-3.402E8 3.402E8,-3.402E8 Z"},
            {"Largest box +1 (Will not render)","M 3.403E38 , 3.402E38 L 3.402E38,-3.402E38 -3.402E38,-3.402E38 -3.402E38,3.402E38 Z"," "},

            {"Diagonal Line & Reverse", " M 450,0 l -450,450 Z", "M 0,450 l 450,-450 Z"},

            {"Relative Positioning line", "M 200,200 l 100,100", "M 200,200 L 300,300"},
            {"Sign Separated Coordinates", "M 200,200 l-5-190-180+5+20+170 Z",
                "M 200,200 l-5,-190 -180,5 20,170Z"},
            {"Equivalence of polyline w/line",
                "M 00, 0 l 00, 200 200,-200 200, 200 200,-200","M 00, 0 l 00, 200 l 200,-200 l 200, 200 l 200,-200"},


            {"Testing of dual decimal coordinate pairs ie(0.0.0) with H&V lines ",
                "M100,60 V100.0 H200.0.0 V200.0.0",
                "M100,60 V100.0 L200,100 0,100 0,200 0,0"},

            {"Exponential Representation",
                "M 2e2,2E2 l 0.1E3,0.025E4Z M 450,0 l -450,450 Z",
                "M 200,200 l 100,250Z M 450,0 l -450,450 Z"},
            {"Negative Exponents",
                "m 0,100 l 50,50E-1 100,-100E-1 100,100E-1 100,-100E-1 50,50E-1",
                "m 0,100 l 50,5 100,-10 100,10 100,-10 50,5"},

            {"Horizontal Line with Absolute positioning",
                "M 200,200 H 300, 400 Z M 450,0 l -450,450 Z",
                "M 200,200 L 300,200, 400,200 Z M 450,0 l -450,450 Z"},
            {"Horizontal Line with Relative positioning",
                "m 200,200 l 300,00, 400,00 Z M 450,0 l -450,450 Z",
                "m 200,200 h 300, 400 Z M 450,0 l -450,450 Z"},
            {"Vertical Line with Absolute positioning",
                "M 200,200 L 200,400, 200,50Z M 450,0 l -450,450 Z",
                "M 200,200 V 400, 50Z M 450,0 l -450,450 Z"},
            {"Vertical Line with Relative positioning",
                "m 200,200 l 0,100, 0,50Z M 450,0 l -450,450 Z",
                "m 200,200 v 100, 50Z M 450,0 l -450,450 Z"},

            {"Triangle, using close loop terminator",
                "M 200,200 l 300,0, 0,200 z M 450,0 l -450,450 Z",
                "M 200,200 h 300 v 200 z M 450,0 l -450,450 Z"},
            {"Moveto w/no drawto operations","m 50 50","M 50,50"},

            {"Moveto w/implicit absolute lineto operations","M 50 50 10,10","M 50 50L 10,10"},
            {"Moveto w/implicit relative lineto operations","m 50 50 10,10","M 50 50l 10,10"},
            {"Equivalence of implicit lineto operations","m 100, 100, 200, 200 200,-200","M 100, 100, 300, 300 500,100"},

            {"Relative Arcs with closure",
                "m 200,200 l 50,25 a 25,25 -30 0,0 50,-25 l 50,25 a25,50 -30 0,1 50,-25 l 50,25 a 25,75 -30 1,0 50,-25 l 50,25 a 25,100 -30 1,1 50,-25 l 50,25 Z",
                "m 200,200  L 250,225  A 25,25 -30 0,0 300,200 l 50,25  A 25,50 -30 0,1 400,200 l 50,25  A 25,75 -30 1,0 500,200 l 50,25  A 25,100 -30 1,1 600,200 l 50,25 Z" },
            {"4 elliptical Arcs toggling sweep & large arc flags.",
                "M 125,75 A100,50 0 0,0 225,125 M 125,75 A100,50 0 0,1 225,125 M 125,75 A100,50 0 1,0 225,125 M 125,75 A100,50 0 1,1 225,125",
                "m 125,75 a100,50 0 0,0 100, 50 M 125,75 a100,50 0 0,1 100, 50 M 125,75 a100,50 0 1,0 100, 50 M 125,75 a100,50 0 1,1 100, 50"},
            
            {"Arcs with positive sign parameters. We accept this, unlike the W3C SVG spec, as covered in Regression_Bug24",
                "M 100,20 9.8.9 a +25,25 -30 0 ,1 50, -25",
                "M 100,20 9.8.9 a 25,+25 -30 0 ,1 50, -25"},

            {"Relative Curveto",
                "m 100 200 c  0,-100 150,-100 150,0 0,100 150,100 150,0",
                "M 100 200 C  100,100 250,100 250,200 250,300 400,300 400,200"},
            {"Shorthand Relative Curveto",
                "m 100 200 s  150,-100 150,0 150,100 150,0",
                "M 100 200 S  250,100 250,200 400,300 400,200"},

            {"Relative Quadratic Bezier Curveto",
                "m 0,100 q 100,-100 200,0, 100,100 200,0",
                "m 0,100 Q 100,0 200,100, 300,200 400,100"},
            {"Quadratic Bezier with curveTo & closure",
                "M200,300 q200,-250 400,000 t400,0 Z M200,300 L400,50 L600,300 L800,550 L1000, 300",
                "M200,300 Q400,50 600,300 T1000,300 Z M200,300 L400,50 L600,300 L800,550 L1000, 300"},
            {"Shorthand Quadratic Bezier Curveto","m 0,100 t 100,-100 100,100","m 0,100 T 100,0 200,100 "},

            {"Shorthand Bezier Immediately following Closepath",
                "M200,200 Q600,50 500,150 T300,150 Z T 200,0 Z",
                "m200,200 q400,-150 300,-50 T300,150 Z T 200,0 Z"},

            {"Twice drawn clockwise box with fill rule 0",
                "F0 m 0,0 L 0,100 100,100 100,0 0,0 m 0,0 L 0,100 100,100 100,0 0,0",
                "F0 m 0,0 L 0,100 M0,100 L 100,100M 100,100 L 100,0 M100,0 L 0,0"},
            {"Twice drawn clockwise box with fill rule 1",
                "F1 m 0,0 L 0,100 100,100 100,0 0,0 m 0,0 L 0,100 100,100 100,0 0,0",
                "F1 m 0,0 L 0,100 100,100 100,0 0,0"},

            {"Fill rule 0 test cw vs Hollow-wound",
                "F0 m 0,0 L 0,100 100,100 100,0 Z  M 40,40 L 40,60 60,60 L60,40 Z",
                "F0 m 0,0 L 0,100 100,100 100,0 Z  M 40,40 L 60,40 60,60  40,60 Z"},
            {"Fill rule 1 test vs Solid",
                "F1 m 0,0 L 0,100 100,100 100,0 0,0  M 40,40 L 40,60 60,60 60,40 40,40 Z",
                "F1 m 0,0 L 0,100 100,100 100,0 0,0  M40,40 L 40,60 60,60 M40,60 L 60,60 60,40 M 60,60 L60,40 40,40 M 60,40 L40,40 40,60"},
        };

        //Non-Visual Round Trip test format for unusual behavior which can't be visually verified, but is valid.
        // This is introduced to test for  behavior introduced in fix of 1492108
        //NaN, Infinity and -Infinity are valid inputs in WPF Paths
        //but their presence in the string will prevent any of the path from being rendered.
        private String[,] _roundTripSet = new String[,]{
                {"Infinity","M0,InfinityLInfinity,0"},
                {"-Infinity","MInfinity,0LInfinity,0"},
                {"NaN","MNaN,NaNLNaN,NaN"},
                {"Line","M-Infinity,NaNLInfinity,-Infinity"},
                {"Arc","MInfinity,NaNLInfinity,-InfinityAInfinity,NaN,1,0,1,50,90"},
                {"Curves","M100,InfinityC100,200 250,NaN 250,Infinity 250,300 400,300 400,200"},
                {"Curves","M100,-InfinityC-Infinity,200 250,Infinity 250,200 250,-Infinity 400,300 400,200"}};

        //Invalid Comparison test format:
        //Description, invalid path
        private String[,] _invalidPathStringData = new String[,] {
            {"Non 0/1 Fill-rule"," F 9 M200,200 Q600,50 500,150 Z"},
            {"Non 0/1 Fill-rule"," F true M200,200 Q600,50 500,150 Z"},

            {"Exponent w/Floating point value","m 0,100 l100,10e1.1"},

            {"Extra sign character","m 0,100 l100,++10"},

            {"Numerical value immediately following closepath","M200,200 Q600,50 500,150  Z 50 T 200,0 Z"},
            {"Numerical pair immediately following closepath","M 125,75 L 4,4 Z T Z Z 4,5  Z Z Z Z Z Z Z"},
            {"Negative Signed Arc","M 100,20 9.8.9 A -25,25 -30 0 ,1 50, 25"},
            {"Negative Signed Arc","M 100,20 9.8.9 A 25,-25 -30 0 ,1 50, 25"},
            {"Negative Signed Arc","M 100,20 9.8.9 a -25,25 -30 0 ,1 50, 25"},
            {"Negative Signed Arc","M 100,20 9.8.9 a 25,-25 -30 0 ,1 50, 25"},

        };

        private TypeConverter _converter = TypeDescriptor.GetConverter(typeof(StreamGeometry));
        private RandomTools _randomGenerator;
        private Geometry _testPath;
        private Boolean _isControlTest;
        private Boolean _boundaryCase;
        private Boolean _logEnabled = false;
        private Color[,] _expectedCapture;
        private double _maxRange;
        private int _testId;
        private String _serialId;
        private String _controlPath,_actualPath;
    }
}

