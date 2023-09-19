// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Globalization;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Random variable driven MiniLanguage Test Tools
    /// </summary>
    public class RandomTools
    {
        /// <summary/>
        public RandomTools(int randomSeed)
        {
            _random = new Random(randomSeed);
            _ensureCleanString = true;
            paddingBugProbability = 0.1;
            _maxNumSpaces = 50;
            _maxInvalidPads = 3;
        }

        /// <summary/>
        public double GetRandomDouble(double max)
        {
            return _random.NextDouble() * max;
        }

        /// <summary/>
        public double GetIrregularDouble()
        {
            Double[] table = new double[]{
                Double.NegativeInfinity,
                Double.PositiveInfinity,
                Double.Epsilon,
                Double.NaN,
                Double.MaxValue,
                Double.MinValue,
                0,
                0/-1};
            int i = GetRandomInt(table.Length);

            return table[i];
        }

        /// <summary/>
        public int GetRandomInt(int max)
        {
            return _random.Next(max);
        }

        /// <summary/>
        public Boolean GetRandomBool()
        {
            return (GetRandomInt(2) == 1) ? true : false;
        }

        /// <summary/>
        public string PadPath(string check)
        {
            int position = 0;
            int oldPosition = 0;
            int delta;
            StringBuilder temp = new StringBuilder();
            while (((position = check.IndexOf(" ", oldPosition)) > 0 && (oldPosition != position)))
            {
                delta = position - oldPosition;
                temp.Append(check.Substring(oldPosition, delta));
                if ((_remainingInvalidPads > 0) && (paddingBugProbability > GetRandomDouble(1)))
                {
                    temp.Append(" ");
                    temp.Append(GetInvalidPad());
                    temp.Append(" ");
                }

                oldPosition = position;
            }
            temp.Append(check.Substring(oldPosition));

            return temp.ToString();
        }

        /// <summary/>
        public String PadString(String original)
        {
            String temp = CreatePad(GetRandomInt(_maxNumSpaces)) + original;
            temp = temp + CreatePad(GetRandomInt(_maxNumSpaces));
            return temp;
        }

        /// <summary/>
        public String PadScColor(Color color, bool hasAlpha)
        {
            String s = PadString("sc#");
            if (hasAlpha)
            {
                s = s + PadDouble(color.ScA) + GetSeparator();
            }
            s = s + PadDouble(color.ScR) + GetSeparator() +
            PadDouble(color.ScG) + GetSeparator() +
            PadDouble(color.ScB);

            return s;
        }

        /// <summary/>
        public string PadTupleColor(Color color, bool alpha, bool isByte)
        {
            String temp;
            if (alpha && isByte)
            {
                temp = string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", color.A, color.R, color.G, color.B);
            }
            else if (alpha && !isByte)
            {
                temp = string.Format("#{0:X1}{1:X1}{2:X1}{3:X1}", color.A, color.R, color.G, color.B);
            }
            else if (!alpha && isByte)
            {
                temp = string.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
            }
            else
            {
                temp = string.Format("#{0:X1}{1:X1}{2:X1}", color.R, color.G, color.B);
            }
            return temp;
        }

        /// <summary/>
        public String PadMatrix(Matrix m)
        {
            return PadDouble(m.M11) + GetSeparator() +
                PadDouble(m.M12) + GetSeparator() +
                PadDouble(m.M21) + GetSeparator() +
                PadDouble(m.M22) + GetSeparator() +
                PadDouble(m.OffsetX) + GetSeparator() +
                PadDouble(m.OffsetY);
        }

        /// <summary/>
        public String PadPointCollection(PointCollection points)
        {
            String temp = "";
            bool firstPass = true;
            foreach (Point p in points)
            {
                if (firstPass)
                {
                    temp = PadPoint(p);
                    firstPass = false;
                }
                else
                {
                    temp = temp + GetSeparator() + PadPoint(p);
                }
            }
            return temp;
        }

        /// <summary/>
        public String PadDouble(Double d)
        {
            return PadString(d.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary/>
        public String PadVector(Vector v)
        {
            return PadString(PadDouble(v.X) + GetSeparator() + PadDouble(v.Y));
        }

        /// <summary/>
        public String PadRect(Rect r)
        {
            return PadDouble(r.X) + GetSeparator() + PadDouble(r.Y) + GetSeparator() +
                PadDouble(r.Width) + GetSeparator() + PadDouble(r.Height);
        }

        /// <summary/>
        public String PadSize(Size s)
        {
            return PadDouble(s.Width) + GetSeparator() + PadDouble(s.Height);
        }

        /// <summary/>
        public String PadPoint(Point p)
        {
            return PadDouble(p.X) + GetSeparator() + PadDouble(p.Y);
        }

        private char GetSeparator()
        {
            char temp;
            if (GetRandomBool())
            {
                temp = ',';
            }
            else
            {
                temp = ' ';
            }
            return temp;
        }

        private String CreatePad(int length)
        {
            StringBuilder temp = new StringBuilder();

            //Prepare invalid single character inputs, surrounded by white spaces on random occasions, if enabled
            if ((_remainingInvalidPads > 0) && (GetRandomDouble(1) < paddingBugProbability))
            {
                temp.Append(" ");
                temp.Append(GetInvalidPad());
                temp.Append(" ");
            }
            //Prepare valid whitespace inputs
            else
            {

                for (int i = 0; i < length; i++)
                {
                    int j = GetRandomInt(5);

                    //legal types of whitespace characters
                    switch (j)
                    {
                        case 0:
                            temp.Append("\n");   //Newline
                            break;
                        case 1:
                            temp.Append(" ");    //Space
                            break;
                        case 2:
                            temp.Append("\t");   //Tab
                            break;
                        case 3:
                            temp.Append("\f");   //Form feed
                            break;
                        case 4:
                            temp.Append("\r");   //Carriage Return
                            break;
                    }
                }
            }
            return temp.ToString();
        }

        private String GetInvalidPad()
        {
            String pad;
            Char c = ' ';
            while (!IsGarbageToken(c))
            {
                c = Char.Parse(Char.ConvertFromUtf32(GetRandomInt(400)));
            }
            pad = c.ToString(CultureInfo.InvariantCulture);
            _remainingInvalidPads--;
            _clean = false;
            return pad;
        }

        private bool IsGarbageToken(Char c)
        {

            bool v = false;
            if (Char.IsSymbol(c))
            {
                v = true;  //Only interested in odd symbols as potential invalids here
            }

            if (Char.IsLetterOrDigit(c))
            {
                v = false;
            }
            else if (Char.IsSeparator(c))
            {
                v = false;
            }
            else if (Char.IsWhiteSpace(c))
            {
                v = false;
            }
            else if (c.Equals(',') || (c.Equals('#')))
            {
                v = false;
            }
            return v;
        }

        /// <summary/>
        public Point CreateRandomPoint(double maxBounds)
        {
            Point pt = new Point();

            pt.X = GetRandomDouble(maxBounds) * 2 - (maxBounds);
            pt.Y = GetRandomDouble(maxBounds) * 2 - (maxBounds);
            return pt;
        }

        /// <summary/>
        public Size CreateRandomSize(double maxBounds)
        {
            Size temp = new Size();
            temp.Width = GetPositiveDouble(maxBounds);
            temp.Height = GetPositiveDouble(maxBounds);
            return temp;
        }

        /// <summary/>
        public PointCollection CreateRandomPointCollection(int maxPoints, double maxBounds)
        {
            PointCollection c = new PointCollection();
            for (int i = 0; i < maxPoints; i++)
            {
                Point temp = CreateRandomPoint(maxBounds);
                c.Add(temp);
            }
            return c;
        }

        /// <summary/>
        public PathGeometry CreateRandomPath(double maxBounds)
        {
            PathGeometry temp = new PathGeometry();

            //Randomly override fillrule to become nonzero -> F1
            if (GetRandomBool())
            {
                temp.FillRule = FillRule.Nonzero;
            }

            PathFigureCollection figures = new PathFigureCollection();
            int maxFigures = GetRandomInt(3) + 1;
            for (int j = 0; j < maxFigures; j++)
            {
                PathFigure f = new PathFigure();
                f.StartPoint = CreateRandomPoint(maxBounds);
                int maxSegments = GetRandomInt(9) + 3;
                f.Segments = new PathSegmentCollection(maxSegments);
                for (int i = 0; i < maxSegments; i++)
                {
                    int maxPoints = 5;
                    PathSegment p = CreateRandomSegment(maxPoints, maxBounds);
                    f.Segments.Add(p);
                }
                figures.Add(f);
            }
            temp.Figures = figures;

            return temp;
        }

        /// <summary/>
        public PathSegment CreateRandomSegment(int maxIterations, double maxBounds)
        {
            PathSegment temp = null;

            int val = GetRandomInt(105);

            if (val < 15)
            {
                temp = new LineSegment(CreateRandomPoint(maxBounds), true);
            }
            else if (val < 30)
            {
                int maxPoints = GetRandomInt(maxIterations) + 1;
                PointCollection pc = CreateRandomPointCollection(maxPoints, maxBounds);

                temp = new PolyLineSegment(pc, true);
            }
            else if (val < 45)
            {
                //randomly override CW arc direction to go CCW
                SweepDirection sweepDirection = SweepDirection.Clockwise;
                if (GetRandomBool())
                {
                    sweepDirection = SweepDirection.Counterclockwise;
                }

                temp = new ArcSegment(CreateRandomPoint(maxBounds),
                    CreateRandomSize(maxBounds),
                    GetRandomInt(360),
                    GetRandomBool(),
                    sweepDirection,
                    true);
            }
            else if (val < 60)
            {
                temp = new BezierSegment(CreateRandomPoint(maxBounds),
                    CreateRandomPoint(maxBounds),
                    CreateRandomPoint(maxBounds),
                    true);
            }
            else if (val < 75)
            {
                //Operate w/increments of 3, as PolyBezier uses triplet
                //(next point , control point at start , control point at end)

                int maxPoints = GetRandomInt(maxIterations) * 9 + 3;
                PointCollection pc = CreateRandomPointCollection(maxPoints, maxBounds);
                temp = new PolyBezierSegment(pc, true);
            }
            else if (val < 90)
            {
                temp = new QuadraticBezierSegment(
                    CreateRandomPoint(maxBounds),
                    CreateRandomPoint(maxBounds),
                    true);
            }
            else
            {
                //Operate in increments of 2, as Quad Bezier requires pairs of points(next pt & control pt)
                int maxPoints = GetRandomInt(maxIterations) * 2 + 2;
                PointCollection pc = CreateRandomPointCollection(maxPoints, maxBounds);
                temp = new PolyQuadraticBezierSegment(pc, true);
            }
            return temp;
        }

        /// <summary/>
        public Rect CreateRandomRect(double maxBounds)
        {
            Rect temp = new Rect();
            temp.Location = CreateRandomPoint(maxBounds);
            temp.Size = CreateRandomSize(maxBounds);
            return temp;
        }

        /// <summary/>
        public Matrix CreateRandomMatrix(double maxBounds)
        {
            Matrix temp = new Matrix(GetRandomDouble(maxBounds), GetRandomDouble(maxBounds), GetRandomDouble(maxBounds), GetRandomDouble(maxBounds), GetRandomDouble(maxBounds), GetRandomDouble(maxBounds));
            return temp;
        }

        /// <summary/>
        public Vector CreateRandomVector(double maxBounds)
        {
            Vector temp = new Vector(GetRandomDouble(maxBounds), GetRandomDouble(maxBounds));
            return temp;
        }

        /// <summary/>
        public Color CreateColor(bool hasAlpha, bool isByte)
        {
            byte a = 255;
            byte r, g, b;
            if (isByte)
            {
                if (hasAlpha)
                {
                    a = (byte)GetRandomInt(256);
                }
                r = (byte)GetRandomInt(256);
                g = (byte)GetRandomInt(256);
                b = (byte)GetRandomInt(256);
            }
            else
            {
                if (hasAlpha)
                {
                    a = (byte)GetRandomInt(16);
                }
                r = (byte)GetRandomInt(16);
                g = (byte)GetRandomInt(16);
                b = (byte)GetRandomInt(16);
            }

            return Color.FromArgb(a, r, g, b);
        }


        /// <summary>
        /// This method resets the "clean" state to true and initializes the remaining pads
        /// to max if the input should be accepted on parsing.
        ///
        /// There is no situation in which an outside function outside this class
        /// should be allowed to trip the clean flag.
        /// </summary>
        public void ResetCleanFlag()
        {
            _clean = true;
            if (_ensureCleanString)
            {
                _remainingInvalidPads = 0;
            }
            else
            {
                _remainingInvalidPads = _maxInvalidPads;
            }
        }

        /// <summary/>
        public bool IsClean
        {
            get
            {
                return _clean;
            }
        }

        /// <summary/>
        public bool KeepClean
        {
            set
            {
                _ensureCleanString = value;
            }
        }

        /// <summary/>
        public double paddingBugProbability;


        private double GetPositiveDouble(double maxBounds)
        {
            return _random.NextDouble() * maxBounds;
        }

        private bool _clean;
        private bool _ensureCleanString;

        int _maxNumSpaces;
        int _maxInvalidPads;
        Random _random;
        int _remainingInvalidPads = 0;
    }
}

