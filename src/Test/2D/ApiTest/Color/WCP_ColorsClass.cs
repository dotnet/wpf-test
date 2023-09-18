// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Test all public API's in the Colors class
//
using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class WCP_ColorsClass : ApiTest
    {
        //--------------------------------------------------------------------

        public WCP_ColorsClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _objectType = typeof(System.Windows.Media.Colors);
            _helper = new HelperClass();
            Update();
        }

        //--------------------------------------------------------------------

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus(_objectType.ToString());

            // Create objects for comparison and drawing
            Point P1 = new Point(10, 10);
            Point P2 = new Point(30, 30);
            Vector V1 = new Vector(30, 0);
            Vector V2 = new Vector(0, 30);

            #region SECTION I - STATIC PROPERTIES
            CommonLib.LogStatus("***** SECTION I - STATIC PROPERTIES *****");

            #region Test #1 - The AliceBlue Property
            // Usage: Color = Colors.AliceBlue (Read only)
            // Notes: Returns a new Color object with Color = 4293982463
            CommonLib.LogStatus("Test #1 - The AliceBlue Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.AliceBlue), null, new Rect(P1, P2));

            // Confirm that the Color has the expected value
            _helper.CompareColorPropUint("AliceBlue", Colors.AliceBlue, 4293982463);
            #endregion

            #region Test #2 - The AntiqueWhite Property
            // Usage: Color = Colors.AntiqueWhite (Read only)
            // Notes: Returns a new Color object who's Color has the Argb value 4294634455
            CommonLib.LogStatus("Test #2 - The AntiqueWhite Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.AntiqueWhite), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("AntiqueWhite", Colors.AntiqueWhite, 4294634455);
            #endregion

            #region Test #3 - The Aqua Property
            // Usage: Color = Colors.Aqua (Read only)
            // Notes: Returns a new Color object with Color = 4278255615
            CommonLib.LogStatus("Test #3 - The Aqua Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Aqua), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Aqua", Colors.Aqua, 4278255615);
            #endregion

            #region Test #4 - The Aquamarine Property
            // Usage: Color = Colors.Aquamarine (Read only)
            // Notes: Returns a new Color object with Color = 4286578644
            CommonLib.LogStatus("Test #4 - The Aquamarine Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Aquamarine), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Aquamarine", Colors.Aquamarine, 4286578644);
            #endregion

            #region Test #5 - The Azure Property
            // Usage: Color = Colors.Azure (Read only)
            // Notes: Returns a new Color object with Color = 4293984255
            CommonLib.LogStatus("Test #5 - The Azure Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Azure), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Azure", Colors.Azure, 4293984255);
            #endregion

            #region Test #6 - The Beige Property
            // Usage: Color = Colors.Beige (Read only)
            // Notes: Returns a new Color object with Color = 4294309340
            CommonLib.LogStatus("Test #6 - The Beige Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Beige), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Beige", Colors.Beige, 4294309340);
            #endregion

            #region Test #7 - The Bisque Property
            // Usage: Color = Colors.Bisque (Read only)
            // Notes: Returns a new Color object with Color = 4294960324
            CommonLib.LogStatus("Test #7 - The Bisque Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Bisque), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Bisque", Colors.Bisque, 4294960324);
            #endregion

            #region Test #8 - The Black Property
            // Usage: Color = Colors.Black (Read only)
            // Notes: Returns a new Color object with Color = 4278190080
            CommonLib.LogStatus("Test #8 - The Black Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Black), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Black", Colors.Black, 4278190080);
            #endregion

            #region Test #9 - The BlanchedAlmond Property
            // Usage: Color = Colors.BlanchedAlmond (Read only)
            // Notes: Returns a new Color object with Color = 4294962125
            CommonLib.LogStatus("Test #9 - The BlanchedAlmond Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.BlanchedAlmond), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("BlanchedAlmond", Colors.BlanchedAlmond, 4294962125);
            #endregion

            #region Test #10 - The Blue Property
            // Usage: Color = Colors.Blue (Read only)
            // Notes: Returns a new Color object with Color = 4278190335
            CommonLib.LogStatus("Test #10 - The Blue Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Blue), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Blue", Colors.Blue, 4278190335);
            #endregion

            #region Test #11 - The BlueViolet Property
            // Usage: Color = Colors.BlueViolet (Read only)
            // Notes: Returns a new Color object with Color = 4287245282
            CommonLib.LogStatus("Test #11 - The BlueViolet Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.BlueViolet), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("BlueViolet", Colors.BlueViolet, 4287245282);
            #endregion

            #region Test #12 - The Brown Property
            // Usage: Color = Colors.Brown (Read only)
            // Notes: Returns a new Color object with Color = 4289014314
            CommonLib.LogStatus("Test #12 - The Brown Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Brown), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Brown", Colors.Brown, 4289014314);
            #endregion

            #region Test #13 - The BurlyWood Property
            // Usage: Color = Colors.BurlyWood (Read only)
            // Notes: Returns a new Color object with Color = 4292786311
            CommonLib.LogStatus("Test #13 - The BurlyWood Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.BurlyWood), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("BurlyWood", Colors.BurlyWood, 4292786311);
            #endregion

            #region Test #14 - The CadetBlue Property
            // Usage: Color = Colors.CadetBlue (Read only)
            // Notes: Returns a new Color object with Color = 4284456608
            CommonLib.LogStatus("Test #14 - The CadetBlue Property");

            // Fill a Rectangle with the Color.
            P1.X = -20;
            P2.X = 0;
            P1 += V2;
            P2 += V2;
            DC.DrawRectangle(new SolidColorBrush(Colors.CadetBlue), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("CadetBlue", Colors.CadetBlue, 4284456608);
            #endregion

            #region Test #15 - The Chartreuse Property
            // Usage: Color = Colors.Chartreuse (Read only)
            // Notes: Returns a new Color object with Color = 4286578432
            CommonLib.LogStatus("Test #15 - The Chartreuse Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Chartreuse), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Chartreuse", Colors.Chartreuse, 4286578432);
            #endregion

            #region Test #16 - The Chocolate Property
            // Usage: Color = Colors.Chocolate (Read only)
            // Notes: Returns a new Color object with Color = 4291979550
            CommonLib.LogStatus("Test #16 - The Chocolate Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Chocolate), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Chocolate", Colors.Chocolate, 4291979550);
            #endregion

            #region Test #17 - The Coral Property
            // Usage: Color = Colors.Coral (Read only)
            // Notes: Returns a new Color object with Color = 4294934352
            CommonLib.LogStatus("Test #17 - The Coral Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Coral), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Coral", Colors.Coral, 4294934352);
            #endregion

            #region Test #18 - The CornflowerBlue Property
            // Usage: Color = Colors.CornflowerBlue (Read only)
            // Notes: Returns a new Color object with Color = 4284782061
            CommonLib.LogStatus("Test #18 - The CornflowerBlue Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.CornflowerBlue), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("CornflowerBlue", Colors.CornflowerBlue, 4284782061);
            #endregion

            #region Test #19 - The Cornsilk Property
            // Usage: Color = Colors.Cornsilk (Read only)
            // Notes: Returns a new Color object with Color = 4294965468
            CommonLib.LogStatus("Test #19 - The Cornsilk Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Cornsilk), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Cornsilk", Colors.Cornsilk, 4294965468);
            #endregion

            #region Test #20 - The Crimson Property
            // Usage: Color = Colors.Crimson (Read only)
            // Notes: Returns a new Color object with Color = 4292613180
            CommonLib.LogStatus("Test #20 - The Crimson Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Crimson), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Crimson", Colors.Crimson, 4292613180);
            #endregion

            #region Test #21 - The Cyan Property
            // Usage: Color = Colors.Cyan (Read only)
            // Notes: Returns a new Color object with Color = 4278255615
            CommonLib.LogStatus("Test #21 - The Cyan Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Cyan), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Cyan", Colors.Cyan, 4278255615);
            #endregion

            #region Test #22 - The DarkBlue Property
            // Usage: Color = Colors.DarkBlue (Read only)
            // Notes: Returns a new Color object with Color = 4278190219
            CommonLib.LogStatus("Test #22 - The DarkBlue Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.DarkBlue), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("DarkBlue", Colors.DarkBlue, 4278190219);
            #endregion

            #region Test #23 - The DarkCyan Property
            // Usage: Color = Colors.DarkCyan (Read only)
            // Notes: Returns a new Color object with Color = 4278225803
            CommonLib.LogStatus("Test #23 - The DarkCyan Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.DarkCyan), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("DarkCyan", Colors.DarkCyan, 4278225803);
            #endregion

            #region Test #24 - The DarkGoldenrod Property
            // Usage: Color = Colors.DarkGoldenrod (Read only)
            // Notes: Returns a new Color object with Color = 4290283019
            CommonLib.LogStatus("Test #24 - The DarkGoldenrod Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.DarkGoldenrod), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("DarkGoldenrod", Colors.DarkGoldenrod, 4290283019);
            #endregion

            #region Test #25 - The DarkGray Property
            // Usage: Color = Colors.DarkGray (Read only)
            // Notes: Returns a new Color object with Color = 4289309097
            CommonLib.LogStatus("Test #25 - The DarkGray Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.DarkGray), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("DarkGray", Colors.DarkGray, 4289309097);
            #endregion

            #region Test #26 - The DarkGreen Property
            // Usage: Color = Colors.DarkGreen (Read only)
            // Notes: Returns a new Color object with Color = 4278215680
            CommonLib.LogStatus("Test #26 - The DarkGreen Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.DarkGreen), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("DarkGreen", Colors.DarkGreen, 4278215680);
            #endregion

            #region Test #27 - The DarkKhaki Property
            // Usage: Color = Colors.DarkKhaki (Read only)
            // Notes: Returns a new Color object with Color = 4290623339
            CommonLib.LogStatus("Test #27 - The DarkKhaki Property");

            // Fill a Rectangle with the Color.
            P1.X = -20;
            P2.X = 0;
            P1 += V2;
            P2 += V2;
            DC.DrawRectangle(new SolidColorBrush(Colors.DarkKhaki), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("DarkKhaki", Colors.DarkKhaki, 4290623339);
            #endregion

            #region Test #28 - The DarkMagenta Property
            // Usage: Color = Colors.DarkMagenta (Read only)
            // Notes: Returns a new Color object with Color = 4287299723
            CommonLib.LogStatus("Test #28 - The DarkMagenta Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.DarkMagenta), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("DarkMagenta", Colors.DarkMagenta, 4287299723);
            #endregion

            #region Test #29 - The DarkOliveGreen Property
            // Usage: Color = Colors.DarkOliveGreen (Read only)
            // Notes: Returns a new Color object with Color = 4283788079
            CommonLib.LogStatus("Test #29 - The DarkOliveGreen Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.DarkOliveGreen), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("DarkOliveGreen", Colors.DarkOliveGreen, 4283788079);
            #endregion

            #region Test #30 - The DarkOrange Property
            // Usage: Color = Colors.DarkOrange (Read only)
            // Notes: Returns a new Color object with Color = 4294937600
            CommonLib.LogStatus("Test #30 - The DarkOrange Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.DarkOrange), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value
            _helper.CompareColorPropUint("DarkOrange", Colors.DarkOrange, 4294937600);
            #endregion

            #region Test #31 - The DarkOrchid Property
            // Usage: Color = Colors.DarkOrchid (Read only)
            // Notes: Returns a new Color object with Color = 4288230092
            CommonLib.LogStatus("Test #31 - The DarkOrchid Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.DarkOrchid), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value
            _helper.CompareColorPropUint("DarkOrchid", Colors.DarkOrchid, 4288230092);
            #endregion

            #region Test #32 - The DarkRed Property
            // Usage: Color = Colors.DarkRed (Read only)
            // Notes: Returns a new Color object with Color = 4287299584
            CommonLib.LogStatus("Test #32 - The DarkRed Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.DarkRed), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("DarkRed", Colors.DarkRed, 4287299584);
            #endregion

            #region Test #33 - The DarkSalmon Property
            // Usage: Color = Colors.DarkSalmon (Read only)
            // Notes: Returns a new Color object with Color = 4293498490
            CommonLib.LogStatus("Test #33 - The DarkSalmon Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.DarkSalmon), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("DarkSalmon", Colors.DarkSalmon, 4293498490);
            #endregion

            #region Test #34 - The DarkSeaGreen Property
            // Usage: Color = Colors.DarkSeaGreen (Read only)
            // Notes: Returns a new Color object with Color = 4287609995
            CommonLib.LogStatus("Test #34 - The DarkSeaGreen Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.DarkSeaGreen), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("DarkSeaGreen", Colors.DarkSeaGreen, 4287609999);
            #endregion

            #region Test #35 - The DarkSlateBlue Property
            // Usage: Color = Colors.DarkSlateBlue (Read only)
            // Notes: Returns a new Color object with Color = 4282924427
            CommonLib.LogStatus("Test #35 - The DarkSlateBlue Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.DarkSlateBlue), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("DarkSlateBlue", Colors.DarkSlateBlue, 4282924427);
            #endregion

            #region Test #36 - The DarkSlateGray Property
            // Usage: Color = Colors.DarkSlateGray (Read only)
            // Notes: Returns a new Color object with Color = 4281290575
            CommonLib.LogStatus("Test #36 - The DarkSlateGray Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.DarkSlateGray), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("DarkSlateGray", Colors.DarkSlateGray, 4281290575);
            #endregion

            #region Test #37 - The DarkTurquoise Property
            // Usage: Color = Colors.DarkTurquoise (Read only)
            // Notes: Returns a new Color object with Color = 4278243025
            CommonLib.LogStatus("Test #37 - The DarkTurquoise Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.DarkTurquoise), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value
            _helper.CompareColorPropUint("DarkTurquoise", Colors.DarkTurquoise, 4278243025);
            #endregion

            #region Test #38 - The DarkViolet Property
            // Usage: Color = Colors.DarkViolet (Read only)
            // Notes: Returns a new Color object with Color = 4287889619
            CommonLib.LogStatus("Test #38 - The DarkViolet Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.DarkViolet), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value
            _helper.CompareColorPropUint("DarkViolet", Colors.DarkViolet, 4287889619);
            #endregion

            #region Test #39 - The DeepPink Property
            // Usage: Color = Colors.DeepPink (Read only)
            // Notes: Returns a new Color object with Color = 4294907027
            CommonLib.LogStatus("Test #39 - The DeepPink Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.DeepPink), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("DeepPink", Colors.DeepPink, 4294907027);
            #endregion

            #region Test #40 - The DeepSkyBlue Property
            // Usage: Color = Colors.DeepSkyBlue (Read only)
            // Notes: Returns a new Color object with Color = 4278239231
            CommonLib.LogStatus("Test #40 - The DeepSkyBlue Property");

            // Fill a Rectangle with the Color.
            P1.X = -20;
            P2.X = 0;
            P1 += V2;
            P2 += V2;
            DC.DrawRectangle(new SolidColorBrush(Colors.DeepSkyBlue), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("DeepSkyBlue", Colors.DeepSkyBlue, 4278239231);
            #endregion

            #region Test #41 - The DimGray Property
            // Usage: Color = Colors.DimGray (Read only)
            // Notes: Returns a new Color object with Color = 4285098345
            CommonLib.LogStatus("Test #41 - The DimGray Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.DimGray), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("DimGray", Colors.DimGray, 4285098345);
            #endregion

            #region Test #42 - The DodgerBlue Property
            // Usage: Color = Colors.DodgerBlue (Read only)
            // Notes: Returns a new Color object with Color = 4280193279
            CommonLib.LogStatus("Test #42 - The DodgerBlue Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.DodgerBlue), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("DodgerBlue", Colors.DodgerBlue, 4280193279);
            #endregion

            #region Test #43 - The Firebrick Property
            // Usage: Color = Colors.Firebrick (Read only)
            // Notes: Returns a new Color object with Color = 4289864226
            CommonLib.LogStatus("Test #43 - The Firebrick Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Firebrick), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Firebrick", Colors.Firebrick, 4289864226);
            #endregion

            #region Test #44 - The FloralWhite Property
            // Usage: Color = Colors.FloralWhite (Read only)
            // Notes: Returns a new Color object with Color = 4294966000
            CommonLib.LogStatus("Test #44 - The FloralWhite Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.FloralWhite), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value
            _helper.CompareColorPropUint("FloralWhite", Colors.FloralWhite, 4294966000);
            #endregion

            #region Test #45 - The ForestGreen Property
            // Usage: Color = Colors.ForestGreen (Read only)
            // Notes: Returns a new Color object with Color = 4280453922
            CommonLib.LogStatus("Test #45 - The ForestGreen Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.ForestGreen), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value
            _helper.CompareColorPropUint("ForestGreen", Colors.ForestGreen, 4280453922);
            #endregion

            #region Test #46 - The Fuchsia Property
            // Usage: Color = Colors.Fuchsia (Read only)
            // Notes: Returns a new Color object with Color = 4294902015
            CommonLib.LogStatus("Test #46 - The Fuchsia Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Fuchsia), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Fuchsia", Colors.Fuchsia, 4294902015);
            #endregion

            #region Test #47 - The Gainsboro Property
            // Usage: Color = Colors.Gainsboro (Read only)
            // Notes: Returns a new Color object with Color = 4292664540
            CommonLib.LogStatus("Test #47 - The Gainsboro Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Gainsboro), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Gainsboro", Colors.Gainsboro, 4292664540);
            #endregion

            #region Test #48 - The GhostWhite Property
            // Usage: Color = Colors.GhostWhite (Read only)
            // Notes: Returns a new Color object with Color = 4294506751
            CommonLib.LogStatus("Test #48 - The GhostWhite Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.GhostWhite), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("GhostWhite", Colors.GhostWhite, 4294506751);
            #endregion

            #region Test #49 - The Gold Property
            // Usage: Color = Colors.Gold (Read only)
            // Notes: Returns a new Color object with Color = 4294956800
            CommonLib.LogStatus("Test #49 - The Gold Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Gold), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Gold", Colors.Gold, 4294956800);
            #endregion

            #region Test #50 - The Goldenrod Property
            // Usage: Color = Colors.Goldenrod (Read only)
            // Notes: Returns a new Color object with Color = 4292519200
            CommonLib.LogStatus("Test #50 - The Goldenrod Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Goldenrod), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Goldenrod", Colors.Goldenrod, 4292519200);
            #endregion

            #region Test #51 - The Gray Property
            // Usage: Color = Colors.Gray (Read only)
            // Notes: Returns a new Color object with Color = 4286611584
            CommonLib.LogStatus("Test #51 - The Gray Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Gray), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value
            _helper.CompareColorPropUint("Gray", Colors.Gray, 4286611584);
            #endregion

            #region Test #52 - The Green Property
            // Usage: Color = Colors.Green (Read only)
            // Notes: Returns a new Color object with Color = 4278222848
            CommonLib.LogStatus("Test #52 - The Green Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Green), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Green", Colors.Green, 4278222848);
            #endregion

            #region Test #53 - The GreenYellow Property
            // Usage: Color = Colors.GreenYellow (Read only)
            // Notes: Returns a new Color object with Color = 4289593135
            CommonLib.LogStatus("Test #53 - The GreenYellow Property");

            // Fill a Rectangle with the Color.
            P1.X = -20;
            P2.X = 0;
            P1 += V2;
            P2 += V2;
            DC.DrawRectangle(new SolidColorBrush(Colors.GreenYellow), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("GreenYellow", Colors.GreenYellow, 4289593135);
            #endregion

            #region Test #54 - The Honeydew Property
            // Usage: Color = Colors.Honeydew (Read only)
            // Notes: Returns a new Color object with Color = 4293984240
            CommonLib.LogStatus("Test #54 - The Honeydew Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Honeydew), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Honeydew", Colors.Honeydew, 4293984240);
            #endregion

            #region Test #55 - The HotPink Property
            // Usage: Color = Colors.HotPink (Read only)
            // Notes: Returns a new Color object with Color = 4294928820
            CommonLib.LogStatus("Test #55 - The HotPink Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.HotPink), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("HotPink", Colors.HotPink, 4294928820);
            #endregion

            #region Test #56 - The IndianRed Property
            // Usage: Color = Colors.IndianRed (Read only)
            // Notes: Returns a new Color object with Color = 4291648604
            CommonLib.LogStatus("Test #56 - The IndianRed Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.IndianRed), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("IndianRed", Colors.IndianRed, 4291648604);
            #endregion

            #region Test #57 - The Indigo Property
            // Usage: Color = Colors.Indigo (Read only)
            // Notes: Returns a new Color object with Color = 4283105410
            CommonLib.LogStatus("Test #57 - The Indigo Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Indigo), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Indigo", Colors.Indigo, 4283105410);
            #endregion

            #region Test #58 - The Ivory Property
            // Usage: Color = Colors.Ivory (Read only)
            // Notes: Returns a new Color object with Color = 4294967280
            CommonLib.LogStatus("Test #58 - The Ivory Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Ivory), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Ivory", Colors.Ivory, 4294967280);
            #endregion

            #region Test #59 - The Khaki Property
            // Usage: Color = Colors.Khaki (Read only)
            // Notes: Returns a new Color object with Color = 4293977740
            CommonLib.LogStatus("Test #59 - The Khaki Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Khaki), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value
            _helper.CompareColorPropUint("Khaki", Colors.Khaki, 4293977740);
            #endregion

            #region Test #60 - The Lavender Property
            // Usage: Color = Colors.Lavender (Read only)
            // Notes: Returns a new Color object with Color = 4293322490
            CommonLib.LogStatus("Test #60 - The Lavender Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Lavender), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Lavender", Colors.Lavender, 4293322490);
            #endregion

            #region Test #61 - The LavenderBlush Property
            // Usage: Color = Colors.LavenderBlush (Read only)
            // Notes: Returns a new Color object with Color = 4294963445
            CommonLib.LogStatus("Test #61 - The LavenderBlush Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.LavenderBlush), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("LavenderBlush", Colors.LavenderBlush, 4294963445);
            #endregion

            #region Test #62 - The LawnGreen Property
            // Usage: Color = Colors.LawnGreen (Read only)
            // Notes: Returns a new Color object with Color = 4286381056
            CommonLib.LogStatus("Test #62 - The LawnGreen Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.LawnGreen), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("LawnGreen", Colors.LawnGreen, 4286381056);
            #endregion

            #region Test #63 - The LemonChiffon Property
            // Usage: Color = Colors.LemonChiffon (Read only)
            // Notes: Returns a new Color object with Color = 4294965965
            CommonLib.LogStatus("Test #63 - The LemonChiffon Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.LemonChiffon), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("LemonChiffon", Colors.LemonChiffon, 4294965965);
            #endregion

            #region Test #64 - The LightBlue Property
            // Usage: Color = Colors.LightBlue (Read only)
            // Notes: Returns a new Color object with Color = 4289583334
            CommonLib.LogStatus("Test #64 - The LightBlue Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.LightBlue), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("LightBlue", Colors.LightBlue, 4289583334);
            #endregion

            #region Test #65 - The LightCoral Property
            // Usage: Color = Colors.LightCoral (Read only)
            // Notes: Returns a new Color object with Color = 4293951616
            CommonLib.LogStatus("Test #65 - The LightCoral Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.LightCoral), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value
            _helper.CompareColorPropUint("LightCoral", Colors.LightCoral, 4293951616);
            #endregion

            #region Test #66 - The LightCyan Property
            // Usage: Color = Colors.LightCyan (Read only)
            // Notes: Returns a new Color object with Color = 4292935679
            CommonLib.LogStatus("Test #66 - The LightCyan Property");

            // Fill a Rectangle with the Color.
            P1.X = -20;
            P2.X = 0;
            P1 += V2;
            P2 += V2;
            DC.DrawRectangle(new SolidColorBrush(Colors.LightCyan), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("LightCyan", Colors.LightCyan, 4292935679);
            #endregion

            #region Test #67 - The LightGoldenrodYellow Property
            // Usage: Color = Colors.LightGoldenrodYellow (Read only)
            // Notes: Returns a new Color object with Color = 4294638290
            CommonLib.LogStatus("Test #67 - The LightGoldenrodYellow Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.LightGoldenrodYellow), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("LightGoldenrodYellow", Colors.LightGoldenrodYellow, 4294638290);
            #endregion

            #region Test #68 - The LightGray Property
            // Usage: Color = Colors.LightGray (Read only)
            // Notes: Returns a new Color object with Color = 4292072403
            CommonLib.LogStatus("Test #68 - The LightGray Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.LightGray), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("LightGray", Colors.LightGray, 4292072403);
            #endregion

            #region Test #69 - The LightGreen Property
            // Usage: Color = Colors.LightGreen (Read only)
            // Notes: Returns a new Color object with Color = 4287688336
            CommonLib.LogStatus("Test #69 - The LightGreen Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.LightGreen), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("LightGreen", Colors.LightGreen, 4287688336);
            #endregion

            #region Test #70 - The LightPink Property
            // Usage: Color = Colors.LightPink (Read only)
            // Notes: Returns a new Color object with Color = 4294948545
            CommonLib.LogStatus("Test #70 - The LightPink Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.LightPink), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("LightPink", Colors.LightPink, 4294948545);
            #endregion

            #region Test #71 - The LightSalmon Property
            // Usage: Color = Colors.LightSalmon (Read only)
            // Notes: Returns a new Color object with Color = 4294942842
            CommonLib.LogStatus("Test #71 - The LightSalmon Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.LightSalmon), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value
            _helper.CompareColorPropUint("LightSalmon", Colors.LightSalmon, 4294942842);
            #endregion

            #region Test #72 - The LightSeaGreen Property
            // Usage: Color = Colors.LightSeaGreen (Read only)
            // Notes: Returns a new Color object with Color = 4280332970
            CommonLib.LogStatus("Test #72 - The LightSeaGreen Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.LightSeaGreen), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("LightSeaGreen", Colors.LightSeaGreen, 4280332970);
            #endregion

            #region Test #73 - The LightSkyBlue Property
            // Usage: Color = Colors.LightSkyBlue (Read only)
            // Notes: Returns a new Color object with Color = 4287090426
            CommonLib.LogStatus("Test #73 - The LightSkyBlue Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.LightSkyBlue), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("LightSkyBlue", Colors.LightSkyBlue, 4287090426);
            #endregion

            #region Test #74 - The LightSlateGray Property
            // Usage: Color = Colors.LightSlateGray (Read only)
            // Notes: Returns a new Color object with Color = 4286023833
            CommonLib.LogStatus("Test #74 - The LightSlateGray Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.LightSlateGray), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("LightSlateGray", Colors.LightSlateGray, 4286023833);
            #endregion

            #region Test #75 - The LightSteelBlue Property
            // Usage: Color = Colors.LightSteelBlue (Read only)
            // Notes: Returns a new Color object with Color = 4289774814
            CommonLib.LogStatus("Test #75 - The LightSteelBlue Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.LightSteelBlue), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("LightSteelBlue", Colors.LightSteelBlue, 4289774814);
            #endregion

            #region Test #76 - The LightYellow Property
            // Usage: Color = Colors.LightYellow (Read only)
            // Notes: Returns a new Color object with Color = 4294967264
            CommonLib.LogStatus("Test #76 - The LightYellow Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.LightYellow), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("LightYellow", Colors.LightYellow, 4294967264);
            #endregion

            #region Test #77 - The Lime Property
            // Usage: Color = Colors.Lime (Read only)
            // Notes: Returns a new Color object with Color = 4278255360
            CommonLib.LogStatus("Test #77 - The Lime Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Lime), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value
            _helper.CompareColorPropUint("Lime", Colors.Lime, 4278255360);
            #endregion

            #region Test #78 - The LimeGreen Property
            // Usage: Color = Colors.LimeGreen (Read only)
            // Notes: Returns a new Color object with Color = 4281519410
            CommonLib.LogStatus("Test #78 - The LimeGreen Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.LimeGreen), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("LimeGreen", Colors.LimeGreen, 4281519410);
            #endregion

            #region Test #79 - The Linen Property
            // Usage: Color = Colors.Linen (Read only)
            // Notes: Returns a new Color object with Color = 4294635750
            CommonLib.LogStatus("Test #79 - The Linen Property");

            // Fill a Rectangle with the Color.
            P1.X = -20;
            P2.X = 0;
            P1 += V2;
            P2 += V2;
            DC.DrawRectangle(new SolidColorBrush(Colors.Linen), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Linen", Colors.Linen, 4294635750);
            #endregion

            #region Test #80 - The Magenta Property
            // Usage: Color = Colors.Magenta (Read only)
            // Notes: Returns a new Color object with Color = 4294902015
            CommonLib.LogStatus("Test #80 - The Magenta Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Magenta), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Magenta", Colors.Magenta, 4294902015);
            #endregion

            #region Test #81 - The Maroon Property
            // Usage: Color = Colors.Maroon (Read only)
            // Notes: Returns a new Color object with Color = 4286578688
            CommonLib.LogStatus("Test #81 - The Maroon Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Maroon), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Maroon", Colors.Maroon, 4286578688);
            #endregion

            #region Test #82 - The MediumAquamarine Property
            // Usage: Color = Colors.MediumAquamarine (Read only)
            // Notes: Returns a new Color object with Color = 4284927402
            CommonLib.LogStatus("Test #82 - The MediumAquamarine Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.MediumAquamarine), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("MediumAquamarine", Colors.MediumAquamarine, 4284927402);
            #endregion

            #region Test #83 - The MediumBlue Property
            // Usage: Color = Colors.MediumBlue (Read only)
            // Notes: Returns a new Color object with Color = 4278190285
            CommonLib.LogStatus("Test #83 - The MediumBlue Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.MediumBlue), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("MediumBlue", Colors.MediumBlue, 4278190285);
            #endregion

            #region Test #84 - The MediumOrchid Property
            // Usage: Color = Colors.MediumOrchid (Read only)
            // Notes: Returns a new Color object with Color = 4290401747
            CommonLib.LogStatus("Test #84 - The MediumOrchid Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.MediumOrchid), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("MediumOrchid", Colors.MediumOrchid, 4290401747);
            #endregion

            #region Test #85 - The MediumPurple Property
            // Usage: Color = Colors.MediumPurple (Read only)
            // Notes: Returns a new Color object with Color = 4287852763
            CommonLib.LogStatus("Test #85 - The MediumPurple Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.MediumPurple), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("MediumPurple", Colors.MediumPurple, 4287852763);
            #endregion

            #region Test #86 - The MediumSeaGreen Property
            // Usage: Color = Colors.MediumSeaGreen (Read only)
            // Notes: Returns a new Color object with Color = 4282168177
            CommonLib.LogStatus("Test #86 - The MediumSeaGreen Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.MediumSeaGreen), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value
            _helper.CompareColorPropUint("MediumSeaGreen", Colors.MediumSeaGreen, 4282168177);
            #endregion

            #region Test #87 - The MediumSlateBlue Property
            // Usage: Color = Colors.MediumSlateBlue (Read only)
            // Notes: Returns a new Color object with Color = 4286277870
            CommonLib.LogStatus("Test #87 - The MediumSlateBlue Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.MediumSlateBlue), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("MediumSlateBlue", Colors.MediumSlateBlue, 4286277870);
            #endregion

            #region Test #88 - The MediumSpringGreen Property
            // Usage: Color = Colors.MediumSpringGreen (Read only)
            // Notes: Returns a new Color object with Color = 4278254234
            CommonLib.LogStatus("Test #88 - The MediumSpringGreen Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.MediumSpringGreen), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("MediumSpringGreen", Colors.MediumSpringGreen, 4278254234);
            #endregion

            #region Test #89 - The MediumTurquoise Property
            // Usage: Color = Colors.MediumTurquoise (Read only)
            // Notes: Returns a new Color object with Color = 4282962380
            CommonLib.LogStatus("Test #89 - The MediumTurquoise Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.MediumTurquoise), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("MediumTurquoise", Colors.MediumTurquoise, 4282962380);
            #endregion

            #region Test #90 - The MediumVioletRed Property
            // Usage: Color = Colors.MediumVioletRed (Read only)
            // Notes: Returns a new Color object with Color = 4291237253
            CommonLib.LogStatus("Test #90 - The MediumVioletRed Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.MediumVioletRed), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("MediumVioletRed", Colors.MediumVioletRed, 4291237253);
            #endregion

            #region Test #91 - The MidnightBlue Property
            // Usage: Color = Colors.MidnightBlue (Read only)
            // Notes: Returns a new Color object with Color = 4279834992
            CommonLib.LogStatus("Test #91 - The MidnightBlue Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.MidnightBlue), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("MidnightBlue", Colors.MidnightBlue, 4279834992);
            #endregion

            #region Test #92 - The MintCream Property
            // Usage: Color = Colors.MintCream (Read only)
            // Notes: Returns a new Color object with Color = 4294311930
            CommonLib.LogStatus("Test #92 - The MintCream Property");

            // Fill a Rectangle with the Color.
            P1.X = -20;
            P2.X = 0;
            P1 += V2;
            P2 += V2;
            DC.DrawRectangle(new SolidColorBrush(Colors.MintCream), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("MintCream", Colors.MintCream, 4294311930);
            #endregion

            #region Test #93 - The MistyRose Property
            // Usage: Color = Colors.MistyRose (Read only)
            // Notes: Returns a new Color object with Color = 4294960353
            CommonLib.LogStatus("Test #93 - The MistyRose Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.MistyRose), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("MistyRose", Colors.MistyRose, 4294960353);
            #endregion

            #region Test #94 - The Moccasin Property
            // Usage: Color = Colors.Moccasin (Read only)
            // Notes: Returns a new Color object with Color = 4294960309
            CommonLib.LogStatus("Test #94 - The Moccasin Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Moccasin), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Moccasin", Colors.Moccasin, 4294960309);
            #endregion

            #region Test #95 - The NavajoWhite Property
            // Usage: Color = Colors.NavajoWhite (Read only)
            // Notes: Returns a new Color object with Color = 4294958765
            CommonLib.LogStatus("Test #95 - The NavajoWhite Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.NavajoWhite), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("NavajoWhite", Colors.NavajoWhite, 4294958765);
            #endregion

            #region Test #96 - The Navy Property
            // Usage: Color = Colors.Navy (Read only)
            // Notes: Returns a new Color object with Color = 4278190208
            CommonLib.LogStatus("Test #96 - The Navy Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Navy), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Navy", Colors.Navy, 4278190208);
            #endregion

            #region Test #97 - The OldLace Property
            // Usage: Color = Colors.OldLace (Read only)
            // Notes: Returns a new Color object with Color = 4294833638
            CommonLib.LogStatus("Test #97 - The OldLace Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.OldLace), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("OldLace", Colors.OldLace, 4294833638);
            #endregion

            #region Test #98 - The Olive Property
            // Usage: Color = Colors.Olive (Read only)
            // Notes: Returns a new Color object with Color = 4286611456
            CommonLib.LogStatus("Test #98 - The Olive Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Olive), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Olive", Colors.Olive, 4286611456);
            #endregion

            #region Test #99 - The OliveDrab Property
            // Usage: Color = Colors.OliveDrab (Read only)
            // Notes: Returns a new Color object with Color = 4285238819
            CommonLib.LogStatus("Test #99 - The OliveDrab Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.OliveDrab), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("OliveDrab", Colors.OliveDrab, 4285238819);
            #endregion

            #region Test #100 - The Orange Property
            // Usage: Color = Colors.Orange (Read only)
            // Notes: Returns a new Color object with Color = 4294944000
            CommonLib.LogStatus("Test #100 - The Orange Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Orange), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Orange", Colors.Orange, 4294944000);
            #endregion

            #region Test #101 - The OrangeRed Property
            // Usage: Color = Colors.OrangeRed (Read only)
            // Notes: Returns a new Color object with Color = 4294919424
            CommonLib.LogStatus("Test #101 - The OrangeRed Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.OrangeRed), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("OrangeRed", Colors.OrangeRed, 4294919424);
            #endregion

            #region Test #102 - The Orchid Property
            // Usage: Color = Colors.Orchid (Read only)
            // Notes: Returns a new Color object with Color = 4292505814
            CommonLib.LogStatus("Test #102 - The Orchid Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Orchid), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Orchid", Colors.Orchid, 4292505814);
            #endregion

            #region Test #103 - The PaleGoldenrod Property
            // Usage: Color = Colors.PaleGoldenrod (Read only)
            // Notes: Returns a new Color object with Color = 4293847210
            CommonLib.LogStatus("Test #103 - The PaleGoldenrod Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.PaleGoldenrod), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value
            _helper.CompareColorPropUint("PaleGoldenrod", Colors.PaleGoldenrod, 4293847210);
            #endregion

            #region Test #104 - The PaleGreen Property
            // Usage: Color = Colors.PaleGreen (Read only)
            // Notes: Returns a new Color object with Color = 4288215960
            CommonLib.LogStatus("Test #104 - The PaleGreen Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.PaleGreen), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("PaleGreen", Colors.PaleGreen, 4288215960);
            #endregion

            #region Test #105 - The PaleTurquoise Property
            // Usage: Color = Colors.PaleTurquoise (Read only)
            // Notes: Returns a new Color object with Color = 4289720046
            CommonLib.LogStatus("Test #105 - The PaleTurquoise Property");

            // Fill a Rectangle with the Color.
            P1.X = -20;
            P2.X = 0;
            P1 += V2;
            P2 += V2;
            DC.DrawRectangle(new SolidColorBrush(Colors.PaleTurquoise), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("PaleTurquoise", Colors.PaleTurquoise, 4289720046);
            #endregion

            #region Test #106 - The PaleVioletRed Property
            // Usage: Color = Colors.PaleVioletRed (Read only)
            // Notes: Returns a new Color object with Color = 4292571283
            CommonLib.LogStatus("Test #106 - The PaleVioletRed Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.PaleVioletRed), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("PaleVioletRed", Colors.PaleVioletRed, 4292571283);
            #endregion

            #region Test #107 - The PapayaWhip Property
            // Usage: Color = Colors.PapayaWhip (Read only)
            // Notes: Returns a new Color object with Color = 4294963157
            CommonLib.LogStatus("Test #107 - The PapayaWhip Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.PapayaWhip), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("PapayaWhip", Colors.PapayaWhip, 4294963157);
            #endregion

            #region Test #108 - The PeachPuff Property
            // Usage: Color = Colors.PeachPuff (Read only)
            // Notes: Returns a new Color object with Color = 4294957753
            CommonLib.LogStatus("Test #108 - The PeachPuff Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.PeachPuff), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("PeachPuff", Colors.PeachPuff, 4294957753);
            #endregion

            #region Test #109 - The Peru Property
            // Usage: Color = Colors.Peru (Read only)
            // Notes: Returns a new Color object with Color = 4291659071
            CommonLib.LogStatus("Test #109 - The Peru Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Peru), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Peru", Colors.Peru, 4291659071);
            #endregion

            #region Test #110 - The Pink Property
            // Usage: Color = Colors.Pink (Read only)
            // Notes: Returns a new Color object with Color = 4294951115
            CommonLib.LogStatus("Test #110 - The Pink Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Pink), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Pink", Colors.Pink, 4294951115);
            #endregion

            #region Test #111 - The Plum Property
            // Usage: Color = Colors.Plum (Read only)
            // Notes: Returns a new Color object with Color = 4292714717
            CommonLib.LogStatus("Test #111 - The Plum Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Plum), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Plum", Colors.Plum, 4292714717);
            #endregion

            #region Test #112 - The PowderBlue Property
            // Usage: Color = Colors.PowderBlue (Read only)
            // Notes: Returns a new Color object with Color = 4289781990
            CommonLib.LogStatus("Test #112 - The PowderBlue Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.PowderBlue), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("PowderBlue", Colors.PowderBlue, 4289781990);
            #endregion

            #region Test #113 - The Purple Property
            // Usage: Color = Colors.Purple (Read only)
            // Notes: Returns a new Color object with Color = 4286578816
            CommonLib.LogStatus("Test #113 - The Purple Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Purple), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Purple", Colors.Purple, 4286578816);
            #endregion

            #region Test #114 - The Red Property
            // Usage: Color = Colors.Red (Read only)
            // Notes: Returns a new Color object with Color = 4294901760
            CommonLib.LogStatus("Test #114 - The Red Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Red), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Red", Colors.Red, 4294901760);
            #endregion

            #region Test #115 - The RosyBrown Property
            // Usage: Color = Colors.RosyBrown (Read only)
            // Notes: Returns a new Color object with Color = 4290547599
            CommonLib.LogStatus("Test #115 - The RosyBrown Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.RosyBrown), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("RosyBrown", Colors.RosyBrown, 4290547599);
            #endregion

            #region Test #116 - The RoyalBlue Property
            // Usage: Color = Colors.RoyalBlue (Read only)
            // Notes: Returns a new Color object with Color = 4282477025
            CommonLib.LogStatus("Test #116 - The RoyalBlue Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.RoyalBlue), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("RoyalBlue", Colors.RoyalBlue, 4282477025);
            #endregion

            #region Test #117 - The SaddleBrown Property
            // Usage: Color = Colors.SaddleBrown (Read only)
            // Notes: Returns a new Color object with Color = 4287317267
            CommonLib.LogStatus("Test #117 - The SaddleBrown Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.SaddleBrown), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("SaddleBrown", Colors.SaddleBrown, 4287317267);
            #endregion

            #region Test #118 - The Salmon Property
            // Usage: Color = Colors.Salmon (Read only)
            // Notes: Returns a new Color object with Color = 4294606962
            CommonLib.LogStatus("Test #118 - The Salmon Property");

            // Fill a Rectangle with the Color.
            P1.X = -20;
            P2.X = 0;
            P1 += V2;
            P2 += V2;
            DC.DrawRectangle(new SolidColorBrush(Colors.Salmon), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Salmon", Colors.Salmon, 4294606962);
            #endregion

            #region Test #119 - The SandyBrown Property
            // Usage: Color = Colors.SandyBrown (Read only)
            // Notes: Returns a new Color object with Color = 4294222944
            CommonLib.LogStatus("Test #119 - The SandyBrown Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.SandyBrown), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("SandyBrown", Colors.SandyBrown, 4294222944);
            #endregion

            #region Test #120 - The SeaGreen Property
            // Usage: Color = Colors.SeaGreen (Read only)
            // Notes: Returns a new Color object with Color = 4281240407
            CommonLib.LogStatus("Test #120 - The SeaGreen Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.SeaGreen), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("SeaGreen", Colors.SeaGreen, 4281240407);
            #endregion

            #region Test #121 - The SeaShell Property
            // Usage: Color = Colors.SeaShell (Read only)
            // Notes: Returns a new Color object with Color = 4294964718
            CommonLib.LogStatus("Test #121 - The SeaShell Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.SeaShell), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("SeaShell", Colors.SeaShell, 4294964718);
            #endregion

            #region Test #122 - The Sienna Property
            // Usage: Color = Colors.Sienna (Read only)
            // Notes: Returns a new Color object with Color = 4288696877
            CommonLib.LogStatus("Test #122 - The Sienna Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Sienna), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Sienna", Colors.Sienna, 4288696877);
            #endregion

            #region Test #123 - The Silver Property
            // Usage: Color = Colors.Silver (Read only)
            // Notes: Returns a new Color object with Color = 4290822336
            CommonLib.LogStatus("Test #123 - The Silver Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Silver), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Silver", Colors.Silver, 4290822336);
            #endregion

            #region Test #124 - The SkyBlue Property
            // Usage: Color = Colors.SkyBlue (Read only)
            // Notes: Returns a new Color object with Color = 4287090411
            CommonLib.LogStatus("Test #124 - The SkyBlue Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.SkyBlue), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("SkyBlue", Colors.SkyBlue, 4287090411);
            #endregion

            #region Test #125 - The SlateBlue Property
            // Usage: Color = Colors.SlateBlue (Read only)
            // Notes: Returns a new Color object with Color = 4285160141
            CommonLib.LogStatus("Test #125 - The SlateBlue Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.SlateBlue), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("SlateBlue", Colors.SlateBlue, 4285160141);
            #endregion

            #region Test #126 - The SlateGray Property
            // Usage: Color = Colors.SlateGray (Read only)
            // Notes: Returns a new Color object with Color = 4285563024
            CommonLib.LogStatus("Test #126 - The SlateGray Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.SlateGray), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("SlateGray", Colors.SlateGray, 4285563024);
            #endregion

            #region Test #127 - The Snow Property
            // Usage: Color = Colors.Snow (Read only)
            // Notes: Returns a new Color object with Color = 4294966010
            CommonLib.LogStatus("Test #127 - The Snow Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Snow), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Snow", Colors.Snow, 4294966010);
            #endregion

            #region Test #128 - The SpringGreen Property
            // Usage: Color = Colors.SpringGreen (Read only)
            // Notes: Returns a new Color object with Color = 4278255487
            CommonLib.LogStatus("Test #128 - The SpringGreen Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.SpringGreen), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("SpringGreen", Colors.SpringGreen, 4278255487);
            #endregion

            #region Test #129 - The SteelBlue Property
            // Usage: Color = Colors.SteelBlue (Read only)
            // Notes: Returns a new Color object with Color = 4282811060
            CommonLib.LogStatus("Test #129 - The SteelBlue Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.SteelBlue), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value
            _helper.CompareColorPropUint("SteelBlue", Colors.SteelBlue, 4282811060);
            #endregion

            #region Test #130 - The Tan Property
            // Usage: Color = Colors.Tan (Read only)
            // Notes: Returns a new Color object with Color = 4291998860
            CommonLib.LogStatus("Test #130 - The Tan Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Tan), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Tan", Colors.Tan, 4291998860);
            #endregion

            #region Test #131 - The Teal Property
            // Usage: Color = Colors.Teal (Read only)
            // Notes: Returns a new Color object with Color = 4278222976
            CommonLib.LogStatus("Test #131 - The Teal Property");

            // Fill a Rectangle with the Color.
            P1.X = -20;
            P2.X = 0;
            P1 += V2;
            P2 += V2;
            DC.DrawRectangle(new SolidColorBrush(Colors.Teal), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Teal", Colors.Teal, 4278222976);
            #endregion

            #region Test #132 - The Thistle Property
            // Usage: Color = Colors.Thistle (Read only)
            // Notes: Returns a new Color object with Color = 4292394968
            CommonLib.LogStatus("Test #132 - The Thistle Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Thistle), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Thistle", Colors.Thistle, 4292394968);
            #endregion

            #region Test #133 - The Tomato Property
            // Usage: Color = Colors.Tomato (Read only)
            // Notes: Returns a new Color object with Color = 4294927175
            CommonLib.LogStatus("Test #133 - The Tomato Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Tomato), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Tomato", Colors.Tomato, 4294927175);
            #endregion

            #region Test #134 - The Transparent Property
            // Usage: Color = Colors.Transparent (Read only)
            // Notes: Returns a new Color object with Color = 16777215
            CommonLib.LogStatus("Test #134 - The Transparent Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Transparent), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Transparent", Colors.Transparent, 16777215);
            #endregion

            #region Test #135 - The Turquoise Property
            // Usage: Color = Colors.Turquoise (Read only)
            // Notes: Returns a new Color object with Color = 4282441936
            CommonLib.LogStatus("Test #135 - The Turquoise Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Turquoise), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Turquoise", Colors.Turquoise, 4282441936);
            #endregion

            #region Test #136 - The Violet Property
            // Usage: Color = Colors.Violet (Read only)
            // Notes: Returns a new Color object with Color = 4293821166
            CommonLib.LogStatus("Test #136 - The Violet Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Violet), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Violet", Colors.Violet, 4293821166);
            #endregion

            #region Test #137 - The Wheat Property
            // Usage: Color = Colors.Wheat (Read only)
            // Notes: Returns a new Color object with Color = 4294303411
            CommonLib.LogStatus("Test #137 - The Wheat Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Wheat), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Wheat", Colors.Wheat, 4294303411);
            #endregion

            #region Test #138 - The White Property
            // Usage: Color = Colors.White (Read only)
            // Notes: Returns a new Color object with Color = 4294967295
            CommonLib.LogStatus("Test #138 - The White Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.White), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("White", Colors.White, 4294967295);
            #endregion

            #region Test #139 - The WhiteSmoke Property
            // Usage: Color = Colors.WhiteSmoke (Read only)
            // Notes: Returns a new Color object with Color = 4294309365
            CommonLib.LogStatus("Test #139 - The WhiteSmoke Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.WhiteSmoke), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("WhiteSmoke", Colors.WhiteSmoke, 4294309365);
            #endregion

            #region Test #140 - The Yellow Property
            // Usage: Color = Colors.Yellow (Read only)
            // Notes: Returns a new Color object with Color = 4294967040
            CommonLib.LogStatus("Test #140 - The Yellow Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.Yellow), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("Yellow", Colors.Yellow, 4294967040);
            #endregion

            #region Test #141 - The YellowGreen Property
            // Usage: Color = Colors.YellowGreen (Read only)
            // Notes: Returns a new Color object with Color = 4288335154
            CommonLib.LogStatus("Test #141 - The YellowGreen Property");

            // Fill a Rectangle with the Color.
            DC.DrawRectangle(new SolidColorBrush(Colors.YellowGreen), null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _helper.CompareColorPropUint("YellowGreen", Colors.YellowGreen, 4288335154);
            #endregion
            #endregion


            #region TEST LOGGING            
            CommonLib.LogTest("Result for:"+_objectType );
            #endregion End of TEST LOGGING
        }

        //--------------------------------------------------------------------

        private Type _objectType;
        private HelperClass _helper;
    }
}
