// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// File: DrtTabletBase.cs
//
// Description: Base class for Tablet DRTs
//

using System;
using System.Collections;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Automation;

using Microsoft.Win32;

namespace DRT
{
    public abstract class DrtTabletBase : DrtBase
    {
        private StylusInputSimulation.Mouse _mouse = new StylusInputSimulation.Mouse();

        private StylusInputSimulation.Hid _hid;
        
        protected DrtTabletBase() : base()
        {
        }

        public void Log(string s)
        {
            if (Verbose)
            {
                Console.Write(s);
            }
        }

        /// <summary>
        /// Takes a path to a byte[] encoded isf file and returns a StrokeCollection
        /// This API returns a StrokeCollection to shield us from upcoming Ink changes
        /// </summary>
        public StrokeCollection LoadStrokeCollection(string filePath)
        {
            FileStream fs = null;
            try
            {
                fs = File.OpenRead(filePath);

                byte[] data = new byte[fs.Length];

#pragma warning disable CA2022 // Avoid inexact read
                fs.Read(data, 0, (int)fs.Length);
#pragma warning restore CA2022

                fs.Close();

                using (MemoryStream stream = new MemoryStream(data))
                {
                    return new StrokeCollection(stream);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading an Ink ISF file from path '" + filePath + "'", ex);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        /// <summary>
        /// Plays back the specified StrokeCollection on the specified element
        /// </summary>
        public void PlayStrokeCollection(StrokeCollection strokes, InkCanvas inkCanvas)
        {
            //inkCanvas.Ink.AddStrokesAtRectangle(strokes, strokes.GetBoundingBox());
            
            //there is a 
            
            //
            // determine the transform between the UIElement and the root
            //
            GeneralTransform transform = GetTransformToRootElement(inkCanvas);

            foreach (Stroke stroke in strokes)
            {
                StylusPointCollection stylusPoints = stroke.StylusPoints;
                if (stroke.DrawingAttributes.FitToCurve)
                {
                    stylusPoints = stroke.GetBezierStylusPoints();
                }

                if (stylusPoints.Count > 0)
                {
                    Point pt = new Point();
                    for (int i = 0; i < stylusPoints.Count; i++)
                    {
                        StylusPoint stylusPoint = stylusPoints[i];
                        pt.X = stylusPoint.X;
                        pt.Y = stylusPoint.Y;
                        transform.TryTransform(pt, out pt);
                        stylusPoint.X = pt.X;
                        stylusPoint.Y = pt.Y;
                        stylusPoints[i] = stylusPoint;
                    }

                    //
                    // simulate a stylus in air
                    //
                    Hid.SimulateDrtPacket(this.MainWindow, (int)stylusPoints[0].X, (int)stylusPoints[0].Y, false);
                    foreach (StylusPoint point in stylusPoints)
                    {
                        Hid.SimulateDrtPacket(this.MainWindow, (int)point.X, (int)point.Y, true);
                    }

                    //
                    // simulate an up
                    //
                    Hid.SimulateDrtPacket(this.MainWindow, (int)stylusPoints[stylusPoints.Count - 1].X, (int)stylusPoints[stylusPoints.Count - 1].Y, false);
                }
            }
        }

        //
        // mouse methods
        //
        public void SimulateMouseMove(UIElement element, double x, double y)
        {
            SimulateMouseMove(element, new Point(x, y));
        }
        public void SimulateMouseMove(UIElement element, Point pt)
        {
            GeneralTransform transform = GetTransformToRootElement(element);
            transform.TryTransform(pt, out pt);
            _mouse.SimulateMove(this.MainWindow, pt);
        }
        public void SimulateMouseLeftDown(UIElement element, double x, double y)
        {
            SimulateMouseLeftDown(element, new Point(x, y));
        }
        public void SimulateMouseLeftDown(UIElement element, Point pt)
        {
            GeneralTransform transform = GetTransformToRootElement(element);
            transform.TryTransform(pt, out pt);
            _mouse.SimulateLeftDown(this.MainWindow, pt);
        }
        public void SimulateMouseLeftUp(UIElement element, double x, double y)
        {
            SimulateMouseLeftUp(element, new Point(x, y));
        }
        public void SimulateMouseLeftUp(UIElement element, Point pt)
        {
            GeneralTransform transform = GetTransformToRootElement(element);
            transform.TryTransform(pt, out pt);
            _mouse.SimulateLeftUp(this.MainWindow, pt);
        }
        public void SimulateMouseLeftDoubleClick(UIElement element, double x, double y)
        {
            SimulateMouseLeftDoubleClick(element, new Point(x, y));
        }
        public void SimulateMouseLeftDoubleClick(UIElement element, Point pt)
        {
            GeneralTransform transform = GetTransformToRootElement(element);
            transform.TryTransform(pt, out pt);
            _mouse.SimulateLeftDoubleClick(this.MainWindow, pt);
        }

        
        /// <summary>
        /// Saves the contents of a StrokeCollection to the location specified by filePath
        /// Saves the ink as a byte[]
        /// This API returns a StrokeCollection to shield us from upcoming Ink changes
        /// </summary>
        public void SaveStrokeCollection(StrokeCollection strokes, string filePath)
        {
            //
            // Currently saves the entire ink object
            //
            FileStream fs = null;
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    strokes.Save(stream);
                    stream.Position = 0;
                    fs = File.Create(filePath);
                    byte[] data = stream.ToArray();
                    fs.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading saving ISF file to path '" + filePath + "'", ex);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        protected StylusInputSimulation.Hid Hid
        {
            get
            {
                //
                // note - talking to wisptis_.exe is async, we need to let it warm up.
                //
                if (_hid == null)
                {
                    _hid = new StylusInputSimulation.Hid();
                    Thread.Sleep(1000);
                }
                return _hid;
            }

        }
        
        private GeneralTransform GetTransformToRootElement(UIElement element)
        {
            GeneralTransform transform = Transform.Identity;
            Visual rootVisual = this.RootElement;

            if (!Object.ReferenceEquals(element, rootVisual))
            {
                transform = element.TransformToAncestor(rootVisual);
            }
            return transform;
        }

    }

    // A "suite" of tests, typically operating on a single tree.
    public class DrtTabletTestSuite: DrtTestSuite
    {
        protected DrtTabletTestSuite(string name) : base (name)
        {
        }
        // The DrtBase that is running this suite.
        public new DrtTabletBase DRT { get { return base.DRT as DrtTabletBase; } set { base.DRT = value; } }
    }
}
