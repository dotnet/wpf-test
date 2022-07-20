// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Ink;

namespace DRT
{
	/// <summary>
	/// Summary description for saveload.
	/// </summary>
    [TestedSecurityLevelAttribute (SecurityLevel.PartialTrust)]
    public class SaveLoad : DrtInkTestcase
	{
        private void TestLoadSave(string fileName)
        {
            // First load the ISF
            StrokeCollection loadedStrokeCollection = DrtHelpers.LoadInk(fileName);

            for (int x = 0; x < 2; x++)
            {
                MemoryStream savedData = null;
                MemoryStream newData = null;
                try
                {
                    // Now save the ink
                    savedData = new MemoryStream();
                    loadedStrokeCollection.Save(savedData, Convert.ToBoolean(x));
                    savedData.Position = 0;

                    // Load the saved data into a different ink object
                    StrokeCollection newInk = new StrokeCollection(savedData);

                    newData = new MemoryStream();
                    newInk.Save(newData, Convert.ToBoolean(x));

                    if (newData.Length != savedData.Length)
                    {
                        throw new InvalidOperationException("Saved data length differs from Loaded data for the file :" + fileName);
                    }

                    // Now do some sanity check on the inks
                    if (newInk.Count != loadedStrokeCollection.Count)
                    {
                        throw new InvalidOperationException("Saved ink and loaded ink has different stroke count");
                    }

                    if(newInk.Count > 0)
                    {
                        if (newInk[0].DrawingAttributes.Color != loadedStrokeCollection[0].DrawingAttributes.Color)
                        {
                            throw new InvalidOperationException("1st stroke in the saved ink has different color that the 1st stroke in the original ink");
                        }

                        int lastIndex = newInk.Count - 1;
                        if (newInk[lastIndex].StylusPoints.Count != loadedStrokeCollection[lastIndex].StylusPoints.Count)
                        {
                            throw new InvalidOperationException("Last stroke in the Saved ink has different no of packets that the last stroke in the original ink");
                        }
                    }
                }
                finally
                {
                    if (savedData != null) { savedData.Close(); }
                    if (newData != null) { newData.Close(); }
                }
            }
        }

        /// <summary>
        /// save V2 ink, open it back up in V2 to make
        /// sure all new properties are saved
        /// </summary>
        private void TestV2RoundTrip()
        {
            //
            // create 2 strokes, one with defaults, one without
            //
            StrokeCollection v2strokes = new StrokeCollection();
            v2strokes.Add(new Stroke(new StylusPointCollection(new Point[] { new Point(2, 2), new Point(4, 4) })));
            v2strokes.Add(new Stroke(new StylusPointCollection(new Point[] { new Point(6, 6), new Point(8, 8) })));

            //validate V2 defaults
            if (v2strokes[0].DrawingAttributes.Color != Colors.Black ||
                v2strokes[0].DrawingAttributes.FitToCurve != false ||
                v2strokes[0].DrawingAttributes.Height != 2.0031496062992127 ||
                v2strokes[0].DrawingAttributes.Width != 2.0031496062992127 ||
                v2strokes[0].DrawingAttributes.IgnorePressure != false ||
                v2strokes[0].DrawingAttributes.StylusTip != StylusTip.Ellipse ||
                v2strokes[0].DrawingAttributes.StylusTipTransform != Matrix.Identity ||
                v2strokes[0].DrawingAttributes.IsHighlighter != false
                )
            {
                throw new InvalidOperationException("Invalid assumptions about V2 default da properties");
            }

            v2strokes[1].DrawingAttributes.Color = Colors.Blue;
            v2strokes[1].DrawingAttributes.FitToCurve = true;
            v2strokes[1].DrawingAttributes.Height = DrawingAttributes.MinHeight;
            v2strokes[1].DrawingAttributes.Width = DrawingAttributes.MaxWidth;
            v2strokes[1].DrawingAttributes.IgnorePressure = true;
            v2strokes[1].DrawingAttributes.StylusTip = StylusTip.Rectangle;
            Matrix xf = Matrix.Identity;
            xf.Rotate(45f);
            v2strokes[1].DrawingAttributes.StylusTipTransform = xf;
            v2strokes[1].DrawingAttributes.IsHighlighter = true;


            //round trip through V2
            MemoryStream stream = new MemoryStream();
            v2strokes.Save(stream, true);
            stream.Position = 0;

            //Microsoft.Ink.Ink oldInk = new Microsoft.Ink.Ink();
            //oldInk.Load(stream.ToArray());

            StrokeCollection v2strokes2 = new StrokeCollection(stream);

            //validate count
            if (v2strokes.Count != v2strokes2.Count)
            {
                throw new InvalidOperationException("Stroke count failed to roundtrip");
            }

            //validate defaults round tripping
            if (v2strokes2[0].DrawingAttributes.Color != Colors.Black)
            {
                throw new InvalidOperationException("Failed to round trip V2 default da Color");
            }

            //validate defaults round tripping
            if (v2strokes2[0].DrawingAttributes.FitToCurve != false)
            {
                throw new InvalidOperationException("Failed to round trip V2 default da FitToCurve");
            }

            //validate defaults round tripping
            if (v2strokes2[0].DrawingAttributes.Height != 2.0031496062992127)
            {
                throw new InvalidOperationException("Failed to round trip V2 default da Height");
            }

            //validate defaults round tripping
            if (v2strokes2[0].DrawingAttributes.Width != 2.0031496062992127)
            {
                throw new InvalidOperationException("Failed to round trip V2 default da Width");
            }

            //validate defaults round tripping
            if (v2strokes2[0].DrawingAttributes.IgnorePressure != false)
            {
                throw new InvalidOperationException("Failed to round trip V2 default da IgnorePressure");
            }

            //validate defaults round tripping
            if (v2strokes2[0].DrawingAttributes.StylusTip != StylusTip.Ellipse)
            {
                throw new InvalidOperationException("Failed to round trip V2 default da StylusTip");
            }

            //validate defaults round tripping
            if (v2strokes2[0].DrawingAttributes.StylusTipTransform != Matrix.Identity)
            {
                throw new InvalidOperationException("Failed to round trip V2 default da StylusTipTransform");
            }

            //validate defaults round tripping
            if (v2strokes2[0].DrawingAttributes.IsHighlighter != false)
            {
                throw new InvalidOperationException("Failed to round trip V2 default da IsHighlighter");
            }


            /////////

            //validate non-defaults round tripping
            if (v2strokes2[1].DrawingAttributes.Color != Colors.Blue)
            {
                throw new InvalidOperationException("Failed to round trip V2 non-default da Color");
            }

            //validate non-defaults round tripping
            if (v2strokes2[1].DrawingAttributes.FitToCurve != true)
            {
                throw new InvalidOperationException("Failed to round trip V2 non-default da FitToCurve");
            }

            //validate non-defaults round tripping
            if (v2strokes2[1].DrawingAttributes.Height < DrawingAttributes.MinHeight - 1f || v2strokes2[1].DrawingAttributes.Height > DrawingAttributes.MinHeight + 1f)
            {
                throw new InvalidOperationException("Failed to round trip V2 non-default da Height");
            }

            //validate non-defaults round tripping
            if (v2strokes2[1].DrawingAttributes.Width < DrawingAttributes.MaxWidth - 1f || v2strokes2[1].DrawingAttributes.Width > DrawingAttributes.MaxWidth + 1f)
            {
                throw new InvalidOperationException("Failed to round trip V2 non-default da Width");
            }

            //validate non-defaults round tripping
            if (v2strokes2[1].DrawingAttributes.IgnorePressure != true)
            {
                throw new InvalidOperationException("Failed to round trip V2 non-default da IgnorePressure");
            }

            //validate non-defaults round tripping
            if (v2strokes2[1].DrawingAttributes.StylusTip != StylusTip.Rectangle)
            {
                throw new InvalidOperationException("Failed to round trip V2 non-default da StylusTip");
            }

            //validate non-defaults round tripping
            if (v2strokes2[1].DrawingAttributes.StylusTipTransform == Matrix.Identity)
            {
                throw new InvalidOperationException("Failed to round trip V2 non-default da StylusTipTransform");
            }

            //validate non-defaults round tripping
            if (v2strokes2[1].DrawingAttributes.IsHighlighter != true)
            {
                throw new InvalidOperationException("Failed to round trip V2 non-default da IsHighlighter");
            }

        }


        public override void Run()
        {
            // First ISF format
            string[] fileNames = new string[] { "saveLoad.isf", "saveLoad.dat", "gif.isf", "base64Gif.isf" };
            foreach (string fileName in fileNames)
            {
                TestLoadSave(fileName);
            }

            TestV2RoundTrip();
            
            Success = true;
        }
	}
}
