// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DRT;
using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Security.Permissions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;

namespace DRTMil2D
{
    public class DRTMil2D : DrtBase
    {
        private static bool s_isInteractive;

        static private readonly Uri s_fontsUri = new Uri(GetSystemFontsDirectory() + "micross.ttf");

        [EnvironmentPermission(SecurityAction.Assert, Read = "windir")]
        private static string GetSystemFontsDirectory()
        {
            string s = Environment.GetEnvironmentVariable("windir") + @"\Fonts\";

            return s.ToLower(CultureInfo.InvariantCulture);
        }

        [STAThread]
        public static int Main(string[] args)
        {
            DrtBase drt = new DRTMil2D();

            return drt.Run(args);
        }

        private DRTMil2D()
        {
            WindowTitle = "DRTMil2D";
            DrtName = "DRTMil2D";
            WindowSize = new Size(DRTMil2D.WindowWidth, DRTMil2D.WindowHeight);
            TeamContact = "Wpf";
            Contact = "Microsoft";

            Suites = new DrtTestSuite[]
            {
                new DRTMil2DBrushSuite(),
                new DRTMil2DDrawingSuite()
            };
        }

        protected override bool HandleCommandLineArgument(string arg, bool option, string[] args, ref int k)
        {
            // start by giving the base class the first chance
            if (base.HandleCommandLineArgument(arg, option, args, ref k))
            {
                return true;
            }

            // process your own arguments here, using these parameters:
            //      arg     - current argument
            //      option  - true if there was a leading - or /.
            //      args    - all arguments
            //      k       - current index into args
            // Here's a typical sketch:

            if (option)
            {
                switch (arg)    // arg is lower-case, no leading - or /
                {
                    case "interactive":
                        s_isInteractive = true;
                        break;

                    default:                // unknown option.  don't handle it
                        return false;
                }
                return true;
            }

            return false;
        }

        protected override void PrintOptions()
        {
            Console.WriteLine("Options:");
            Console.WriteLine("  -interactive  don't stop tests after a set time period");
            base.PrintOptions();
        }

        public void SetDefaultPage()
        {
            Canvas canvas = new Canvas();
            canvas.Background = Brushes.White;

            RootElement = canvas;
            ShowRoot();
        }

        #region Static

        public static void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public static void Check(double expected, double actual, string message)
        {
            string checkString = "    " + message + " expected: " + expected.ToString() + " actual: " + actual.ToString();

            WriteLine(checkString);
            if (actual != expected)
            {
                throw new InvalidOperationException("DRT Failure: " + checkString);
            }
        }

        public static void Check(int expected, int actual, string message)
        {
            string checkString = "    " + message + " expected: " + expected.ToString() + " actual: " + actual.ToString();

            WriteLine(checkString);
            if (actual != expected)
            {
                throw new InvalidOperationException("DRT Failure: " + checkString);
            }
        }

        public static void Check(string expected, string actual, string message)
        {
            string checkString = "    " + message + " expected: \"" + expected + "\" actual: \"" + actual + "\"";

            WriteLine(checkString);

            if (actual != expected)
            {
                throw new InvalidOperationException("DRT Failure: " + checkString);
            }
        }

        public static void Check(bool hasPassed, string message)
        {
            if (!hasPassed)
            {
                throw new InvalidOperationException("DRT Failure: " + message);
            }
        }

        public static bool IsInteractive
        {
            get
            {
                return s_isInteractive;
            }
        }

        internal static int WindowWidth
        {
            get
            {
                return 800;
            }
        }

        internal static int WindowHeight
        {
            get
            {
                return 750;
            }
        }

        internal static Uri FontsUri
        {
            get { return s_fontsUri; }
        }

        internal static int AnimationLength
        {
            get
            {
                if (IsInteractive)
                {
                    return 5;
                }
                else
                {
                    return 1;
                }
            }
        }

        internal static Rect ResizeRectangle(Rect r, double inflateX, double inflateY)
        {
            double newWidth = inflateX*r.Width;
            double newHeight = inflateY*r.Height;
            double newLeft = Math.Floor(r.Left + ((r.Width - newWidth)/2));
            double newTop  = Math.Floor(r.Top + ((r.Height - newHeight)/2));

            return new Rect(newLeft, newTop, newWidth, newHeight);
        }

        internal static Point CalculateCenter(Rect r)
        {
            return new Point (r.Left + r.Width/2, r.Top + r.Height/2);
        }

        internal static Matrix MatrixRectangleTransform(Rect sourceRect, Rect destRect)
        {
            Matrix matrix = new Matrix();

            matrix.SetIdentity();
            matrix.Translate(-sourceRect.Left, -sourceRect.Top);
            matrix.Scale (
                destRect.Width / sourceRect.Width,
                destRect.Height / sourceRect.Height
                );
            matrix.Translate(destRect.Left, destRect.Top);

            return matrix;
        }

        #endregion
    }

    internal class AreaPartitioner
    {

        public AreaPartitioner(int windowWidth,
                                int windowHeight,
                                int cellWidth,
                                int cellHeight)
                                :
                                // Right & bottom edges of the window may be covered by the NC area
                                this (new Rect(0, 0, windowWidth - 25, windowHeight - 25), cellWidth, cellHeight)
        {
        }

        public AreaPartitioner(Rect areaToPartition, int cellWidth, int cellHeight)
        {
            if (areaToPartition.Width < cellWidth || areaToPartition.Height < cellHeight)
            {
                throw new ArgumentException();
            }

            //
            // Set private state
            //

            _nextAvailable = 0;

            // Round area down to the smallest size
            _areaWidth = (int)areaToPartition.Width - ((int)areaToPartition.Width % cellWidth);
            _areaHeight = (int)areaToPartition.Height - ((int)areaToPartition.Height % cellHeight);

            _cellWidth = cellWidth;
            _cellHeight = cellHeight;

            _numRows = _areaWidth / cellWidth;
            _numColumns = _areaHeight / cellHeight;

            _numCells = _numRows * _numColumns;
            _cells = new ArrayList(_numCells);

            //
            // Calculate cached rect array
            //

            int x = (int)areaToPartition.Left;
            int y = (int)areaToPartition.Top;

            for(int i = 0; i < _numCells; i++)
            {
                _cells.Add( new Rect( new Point(x, y), new Point(x+cellWidth, y+cellHeight) ));

                // Set next x
                if (0 != ((i+1) % _numRows))
                {
                    // Move to next row
                    x += cellWidth;
                }
                else
                {
                    // Move to next column
                    x = (int)areaToPartition.Left;
                    y += cellHeight;
                }
            }

        }

        public void Reserve(int numCellsToReserve)
        {
            if( _nextAvailable + numCellsToReserve > _numCells)
            {
                throw new InvalidOperationException();
            }

            _nextAvailable += numCellsToReserve;

        }

        public int NextIndex
        {
            get
            {
                if (_nextAvailable >= _numCells)
                    throw new InvalidOperationException();

                return _nextAvailable++;
            }
        }

        public Rect Next
        {
            get
            {
                return this[this.NextIndex];
            }
        }

        public Rect this[int index]
        {
            get
            {
                if ((index < 0) ||
                    (index >=   _numCells))
                {
                    throw new IndexOutOfRangeException();
                }

                return (Rect) _cells[index];
            }
        }

        public int CellCount
        {
            get
            {
                return _numCells;
            }
        }

        public int CellWidth
        {
            get
            {
                return _cellWidth;
            }
        }

        public int CellHeight
        {
            get
            {
                return _cellHeight;
            }
        }

        int _areaWidth;
        int _areaHeight;
        int _numRows;
        int _numColumns;

        int _cellWidth;
        int _cellHeight;

        int _numCells;
        ArrayList _cells;
        int _nextAvailable;
    }
}

