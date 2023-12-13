// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Test all public API's in the Brushes class
//
using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class WCP_BrushesClass : ApiTest
    {

        public WCP_BrushesClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _objectType = typeof(System.Windows.Media.Brushes);
            _helper = new HelperClass();
            Update();
        }

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
            // Usage: SolidColorBrush = Brushes.AliceBlue (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4293982463
            CommonLib.LogStatus("Test #1 - The AliceBlue Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.AliceBlue, null, new Rect(P1, P2));

            // Confirm that the Color has the expected value
            _class_testresult &= _helper.CompareColorPropUint("AliceBlue", Brushes.AliceBlue.Color, 4293982463);
            #endregion

            #region Test #2 - The AntiqueWhite Property
            // Usage: SolidColorBrush = Brushes.AntiqueWhite (Read only)
            // Notes: Returns a new SolidColorBrush object who's Color has the Argb value 4294634455
            CommonLib.LogStatus("Test #2 - The AntiqueWhite Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.AntiqueWhite, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("AntiqueWhite", Brushes.AntiqueWhite.Color, 4294634455);
            #endregion

            #region Test #3 - The Aqua Property
            // Usage: SolidColorBrush = Brushes.Aqua (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4278255615
            CommonLib.LogStatus("Test #3 - The Aqua Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Aqua, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Aqua", Brushes.Aqua.Color, 4278255615);
            #endregion

            #region Test #4 - The Aquamarine Property
            // Usage: SolidColorBrush = Brushes.Aquamarine (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4286578644
            CommonLib.LogStatus("Test #4 - The Aquamarine Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Aquamarine, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Aquamarine", Brushes.Aquamarine.Color, 4286578644);
            #endregion

            #region Test #5 - The Azure Property
            // Usage: SolidColorBrush = Brushes.Azure (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4293984255
            CommonLib.LogStatus("Test #5 - The Azure Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Azure, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Azure", Brushes.Azure.Color, 4293984255);
            #endregion

            #region Test #6 - The Beige Property
            // Usage: SolidColorBrush = Brushes.Beige (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294309340
            CommonLib.LogStatus("Test #6 - The Beige Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Beige, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Beige", Brushes.Beige.Color, 4294309340);
            #endregion

            #region Test #7 - The Bisque Property
            // Usage: SolidColorBrush = Brushes.Bisque (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294960324
            CommonLib.LogStatus("Test #7 - The Bisque Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Bisque, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Bisque", Brushes.Bisque.Color, 4294960324);
            #endregion

            #region Test #8 - The Black Property
            // Usage: SolidColorBrush = Brushes.Black (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4278190080
            CommonLib.LogStatus("Test #8 - The Black Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Black, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Black", Brushes.Black.Color, 4278190080);
            #endregion

            #region Test #9 - The BlanchedAlmond Property
            // Usage: SolidColorBrush = Brushes.BlanchedAlmond (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294962125
            CommonLib.LogStatus("Test #9 - The BlanchedAlmond Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.BlanchedAlmond, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("BlanchedAlmond", Brushes.BlanchedAlmond.Color, 4294962125);
            #endregion

            #region Test #10 - The Blue Property
            // Usage: SolidColorBrush = Brushes.Blue (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4278190335
            CommonLib.LogStatus("Test #10 - The Blue Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Blue, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Blue", Brushes.Blue.Color, 4278190335);
            #endregion

            #region Test #11 - The BlueViolet Property
            // Usage: SolidColorBrush = Brushes.BlueViolet (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4287245282
            CommonLib.LogStatus("Test #11 - The BlueViolet Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.BlueViolet, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("BlueViolet", Brushes.BlueViolet.Color, 4287245282);
            #endregion

            #region Test #12 - The Brown Property
            // Usage: SolidColorBrush = Brushes.Brown (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4289014314
            CommonLib.LogStatus("Test #12 - The Brown Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Brown, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Brown", Brushes.Brown.Color, 4289014314);
            #endregion

            #region Test #13 - The BurlyWood Property
            // Usage: SolidColorBrush = Brushes.BurlyWood (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4292786311
            CommonLib.LogStatus("Test #13 - The BurlyWood Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.BurlyWood, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("BurlyWood", Brushes.BurlyWood.Color, 4292786311);
            #endregion

            #region Test #14 - The CadetBlue Property
            // Usage: SolidColorBrush = Brushes.CadetBlue (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4284456608
            CommonLib.LogStatus("Test #14 - The CadetBlue Property");

            // Fill a Rectangle with the SolidColorBrush.
            P1.X = -20;
            P2.X = 0;
            P1 += V2;
            P2 += V2;
            DC.DrawRectangle(Brushes.CadetBlue, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("CadetBlue", Brushes.CadetBlue.Color, 4284456608);
            #endregion

            #region Test #15 - The Chartreuse Property
            // Usage: SolidColorBrush = Brushes.Chartreuse (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4286578432
            CommonLib.LogStatus("Test #15 - The Chartreuse Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Chartreuse, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Chartreuse", Brushes.Chartreuse.Color, 4286578432);
            #endregion

            #region Test #16 - The Chocolate Property
            // Usage: SolidColorBrush = Brushes.Chocolate (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4291979550
            CommonLib.LogStatus("Test #16 - The Chocolate Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Chocolate, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Chocolate", Brushes.Chocolate.Color, 4291979550);
            #endregion

            #region Test #17 - The Coral Property
            // Usage: SolidColorBrush = Brushes.Coral (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294934352
            CommonLib.LogStatus("Test #17 - The Coral Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Coral, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Coral", Brushes.Coral.Color, 4294934352);
            #endregion

            #region Test #18 - The CornflowerBlue Property
            // Usage: SolidColorBrush = Brushes.CornflowerBlue (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4284782061
            CommonLib.LogStatus("Test #18 - The CornflowerBlue Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.CornflowerBlue, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("CornflowerBlue", Brushes.CornflowerBlue.Color, 4284782061);
            #endregion

            #region Test #19 - The Cornsilk Property
            // Usage: SolidColorBrush = Brushes.Cornsilk (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294965468
            CommonLib.LogStatus("Test #19 - The Cornsilk Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Cornsilk, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Cornsilk", Brushes.Cornsilk.Color, 4294965468);
            #endregion

            #region Test #20 - The Crimson Property
            // Usage: SolidColorBrush = Brushes.Crimson (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4292613180
            CommonLib.LogStatus("Test #20 - The Crimson Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Crimson, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Crimson", Brushes.Crimson.Color, 4292613180);
            #endregion

            #region Test #21 - The Cyan Property
            // Usage: SolidColorBrush = Brushes.Cyan (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4278255615
            CommonLib.LogStatus("Test #21 - The Cyan Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Cyan, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Cyan", Brushes.Cyan.Color, 4278255615);
            #endregion

            #region Test #22 - The DarkBlue Property
            // Usage: SolidColorBrush = Brushes.DarkBlue (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4278190219
            CommonLib.LogStatus("Test #22 - The DarkBlue Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.DarkBlue, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("DarkBlue", Brushes.DarkBlue.Color, 4278190219);
            #endregion

            #region Test #23 - The DarkCyan Property
            // Usage: SolidColorBrush = Brushes.DarkCyan (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4278225803
            CommonLib.LogStatus("Test #23 - The DarkCyan Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.DarkCyan, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("DarkCyan", Brushes.DarkCyan.Color, 4278225803);
            #endregion

            #region Test #24 - The DarkGoldenrod Property
            // Usage: SolidColorBrush = Brushes.DarkGoldenrod (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4290283019
            CommonLib.LogStatus("Test #24 - The DarkGoldenrod Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.DarkGoldenrod, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("DarkGoldenrod", Brushes.DarkGoldenrod.Color, 4290283019);
            #endregion

            #region Test #25 - The DarkGray Property
            // Usage: SolidColorBrush = Brushes.DarkGray (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4289309097
            CommonLib.LogStatus("Test #25 - The DarkGray Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.DarkGray, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("DarkGray", Brushes.DarkGray.Color, 4289309097);
            #endregion

            #region Test #26 - The DarkGreen Property
            // Usage: SolidColorBrush = Brushes.DarkGreen (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4278215680
            CommonLib.LogStatus("Test #26 - The DarkGreen Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.DarkGreen, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("DarkGreen", Brushes.DarkGreen.Color, 4278215680);
            #endregion

            #region Test #27 - The DarkKhaki Property
            // Usage: SolidColorBrush = Brushes.DarkKhaki (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4290623339
            CommonLib.LogStatus("Test #27 - The DarkKhaki Property");

            // Fill a Rectangle with the SolidColorBrush.
            P1.X = -20;
            P2.X = 0;
            P1 += V2;
            P2 += V2;
            DC.DrawRectangle(Brushes.DarkKhaki, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("DarkKhaki", Brushes.DarkKhaki.Color, 4290623339);
            #endregion

            #region Test #28 - The DarkMagenta Property
            // Usage: SolidColorBrush = Brushes.DarkMagenta (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4287299723
            CommonLib.LogStatus("Test #28 - The DarkMagenta Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.DarkMagenta, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("DarkMagenta", Brushes.DarkMagenta.Color, 4287299723);
            #endregion

            #region Test #29 - The DarkOliveGreen Property
            // Usage: SolidColorBrush = Brushes.DarkOliveGreen (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4283788079
            CommonLib.LogStatus("Test #29 - The DarkOliveGreen Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.DarkOliveGreen, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("DarkOliveGreen", Brushes.DarkOliveGreen.Color, 4283788079);
            #endregion

            #region Test #30 - The DarkOrange Property
            // Usage: SolidColorBrush = Brushes.DarkOrange (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294937600
            CommonLib.LogStatus("Test #30 - The DarkOrange Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.DarkOrange, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value
            _class_testresult &= _helper.CompareColorPropUint("DarkOrange", Brushes.DarkOrange.Color, 4294937600);
            #endregion

            #region Test #31 - The DarkOrchid Property
            // Usage: SolidColorBrush = Brushes.DarkOrchid (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4288230092
            CommonLib.LogStatus("Test #31 - The DarkOrchid Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.DarkOrchid, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value
            _class_testresult &= _helper.CompareColorPropUint("DarkOrchid", Brushes.DarkOrchid.Color, 4288230092);
            #endregion

            #region Test #32 - The DarkRed Property
            // Usage: SolidColorBrush = Brushes.DarkRed (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4287299584
            CommonLib.LogStatus("Test #32 - The DarkRed Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.DarkRed, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("DarkRed", Brushes.DarkRed.Color, 4287299584);
            #endregion

            #region Test #33 - The DarkSalmon Property
            // Usage: SolidColorBrush = Brushes.DarkSalmon (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4293498490
            CommonLib.LogStatus("Test #33 - The DarkSalmon Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.DarkSalmon, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("DarkSalmon", Brushes.DarkSalmon.Color, 4293498490);
            #endregion

            #region Test #34 - The DarkSeaGreen Property
            // Usage: SolidColorBrush = Brushes.DarkSeaGreen (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4287609995
            CommonLib.LogStatus("Test #34 - The DarkSeaGreen Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.DarkSeaGreen, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("DarkSeaGreen", Brushes.DarkSeaGreen.Color, 4287609999);
            #endregion

            #region Test #35 - The DarkSlateBlue Property
            // Usage: SolidColorBrush = Brushes.DarkSlateBlue (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4282924427
            CommonLib.LogStatus("Test #35 - The DarkSlateBlue Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.DarkSlateBlue, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("DarkSlateBlue", Brushes.DarkSlateBlue.Color, 4282924427);
            #endregion

            #region Test #36 - The DarkSlateGray Property
            // Usage: SolidColorBrush = Brushes.DarkSlateGray (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4281290575
            CommonLib.LogStatus("Test #36 - The DarkSlateGray Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.DarkSlateGray, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("DarkSlateGray", Brushes.DarkSlateGray.Color, 4281290575);
            #endregion

            #region Test #37 - The DarkTurquoise Property
            // Usage: SolidColorBrush = Brushes.DarkTurquoise (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4278243025
            CommonLib.LogStatus("Test #37 - The DarkTurquoise Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.DarkTurquoise, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value
            _class_testresult &= _helper.CompareColorPropUint("DarkTurquoise", Brushes.DarkTurquoise.Color, 4278243025);
            #endregion

            #region Test #38 - The DarkViolet Property
            // Usage: SolidColorBrush = Brushes.DarkViolet (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4287889619
            CommonLib.LogStatus("Test #38 - The DarkViolet Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.DarkViolet, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value
            _class_testresult &= _helper.CompareColorPropUint("DarkViolet", Brushes.DarkViolet.Color, 4287889619);
            #endregion

            #region Test #39 - The DeepPink Property
            // Usage: SolidColorBrush = Brushes.DeepPink (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294907027
            CommonLib.LogStatus("Test #39 - The DeepPink Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.DeepPink, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("DeepPink", Brushes.DeepPink.Color, 4294907027);
            #endregion

            #region Test #40 - The DeepSkyBlue Property
            // Usage: SolidColorBrush = Brushes.DeepSkyBlue (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4278239231
            CommonLib.LogStatus("Test #40 - The DeepSkyBlue Property");

            // Fill a Rectangle with the SolidColorBrush.
            P1.X = -20;
            P2.X = 0;
            P1 += V2;
            P2 += V2;
            DC.DrawRectangle(Brushes.DeepSkyBlue, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("DeepSkyBlue", Brushes.DeepSkyBlue.Color, 4278239231);
            #endregion

            #region Test #41 - The DimGray Property
            // Usage: SolidColorBrush = Brushes.DimGray (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4285098345
            CommonLib.LogStatus("Test #41 - The DimGray Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.DimGray, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("DimGray", Brushes.DimGray.Color, 4285098345);
            #endregion

            #region Test #42 - The DodgerBlue Property
            // Usage: SolidColorBrush = Brushes.DodgerBlue (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4280193279
            CommonLib.LogStatus("Test #42 - The DodgerBlue Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.DodgerBlue, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("DodgerBlue", Brushes.DodgerBlue.Color, 4280193279);
            #endregion

            #region Test #43 - The Firebrick Property
            // Usage: SolidColorBrush = Brushes.Firebrick (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4289864226
            CommonLib.LogStatus("Test #43 - The Firebrick Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Firebrick, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Firebrick", Brushes.Firebrick.Color, 4289864226);
            #endregion

            #region Test #44 - The FloralWhite Property
            // Usage: SolidColorBrush = Brushes.FloralWhite (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294966000
            CommonLib.LogStatus("Test #44 - The FloralWhite Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.FloralWhite, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value
            _class_testresult &= _helper.CompareColorPropUint("FloralWhite", Brushes.FloralWhite.Color, 4294966000);
            #endregion

            #region Test #45 - The ForestGreen Property
            // Usage: SolidColorBrush = Brushes.ForestGreen (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4280453922
            CommonLib.LogStatus("Test #45 - The ForestGreen Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.ForestGreen, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value
            _class_testresult &= _helper.CompareColorPropUint("ForestGreen", Brushes.ForestGreen.Color, 4280453922);
            #endregion

            #region Test #46 - The Fuchsia Property
            // Usage: SolidColorBrush = Brushes.Fuchsia (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294902015
            CommonLib.LogStatus("Test #46 - The Fuchsia Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Fuchsia, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Fuchsia", Brushes.Fuchsia.Color, 4294902015);
            #endregion

            #region Test #47 - The Gainsboro Property
            // Usage: SolidColorBrush = Brushes.Gainsboro (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4292664540
            CommonLib.LogStatus("Test #47 - The Gainsboro Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Gainsboro, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Gainsboro", Brushes.Gainsboro.Color, 4292664540);
            #endregion

            #region Test #48 - The GhostWhite Property
            // Usage: SolidColorBrush = Brushes.GhostWhite (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294506751
            CommonLib.LogStatus("Test #48 - The GhostWhite Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.GhostWhite, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("GhostWhite", Brushes.GhostWhite.Color, 4294506751);
            #endregion

            #region Test #49 - The Gold Property
            // Usage: SolidColorBrush = Brushes.Gold (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294956800
            CommonLib.LogStatus("Test #49 - The Gold Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Gold, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Gold", Brushes.Gold.Color, 4294956800);
            #endregion

            #region Test #50 - The Goldenrod Property
            // Usage: SolidColorBrush = Brushes.Goldenrod (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4292519200
            CommonLib.LogStatus("Test #50 - The Goldenrod Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Goldenrod, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Goldenrod", Brushes.Goldenrod.Color, 4292519200);
            #endregion

            #region Test #51 - The Gray Property
            // Usage: SolidColorBrush = Brushes.Gray (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4286611584
            CommonLib.LogStatus("Test #51 - The Gray Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Gray, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value
            _class_testresult &= _helper.CompareColorPropUint("Gray", Brushes.Gray.Color, 4286611584);
            #endregion

            #region Test #52 - The Green Property
            // Usage: SolidColorBrush = Brushes.Green (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4278222848
            CommonLib.LogStatus("Test #52 - The Green Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Green, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Green", Brushes.Green.Color, 4278222848);
            #endregion

            #region Test #53 - The GreenYellow Property
            // Usage: SolidColorBrush = Brushes.GreenYellow (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4289593135
            CommonLib.LogStatus("Test #53 - The GreenYellow Property");

            // Fill a Rectangle with the SolidColorBrush.
            P1.X = -20;
            P2.X = 0;
            P1 += V2;
            P2 += V2;
            DC.DrawRectangle(Brushes.GreenYellow, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("GreenYellow", Brushes.GreenYellow.Color, 4289593135);
            #endregion

            #region Test #54 - The Honeydew Property
            // Usage: SolidColorBrush = Brushes.Honeydew (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4293984240
            CommonLib.LogStatus("Test #54 - The Honeydew Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Honeydew, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Honeydew", Brushes.Honeydew.Color, 4293984240);
            #endregion

            #region Test #55 - The HotPink Property
            // Usage: SolidColorBrush = Brushes.HotPink (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294928820
            CommonLib.LogStatus("Test #55 - The HotPink Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.HotPink, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("HotPink", Brushes.HotPink.Color, 4294928820);
            #endregion

            #region Test #56 - The IndianRed Property
            // Usage: SolidColorBrush = Brushes.IndianRed (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4291648604
            CommonLib.LogStatus("Test #56 - The IndianRed Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.IndianRed, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("IndianRed", Brushes.IndianRed.Color, 4291648604);
            #endregion

            #region Test #57 - The Indigo Property
            // Usage: SolidColorBrush = Brushes.Indigo (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4283105410
            CommonLib.LogStatus("Test #57 - The Indigo Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Indigo, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Indigo", Brushes.Indigo.Color, 4283105410);
            #endregion

            #region Test #58 - The Ivory Property
            // Usage: SolidColorBrush = Brushes.Ivory (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294967280
            CommonLib.LogStatus("Test #58 - The Ivory Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Ivory, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Ivory", Brushes.Ivory.Color, 4294967280);
            #endregion

            #region Test #59 - The Khaki Property
            // Usage: SolidColorBrush = Brushes.Khaki (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4293977740
            CommonLib.LogStatus("Test #59 - The Khaki Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Khaki, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value
            _class_testresult &= _helper.CompareColorPropUint("Khaki", Brushes.Khaki.Color, 4293977740);
            #endregion

            #region Test #60 - The Lavender Property
            // Usage: SolidColorBrush = Brushes.Lavender (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4293322490
            CommonLib.LogStatus("Test #60 - The Lavender Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Lavender, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Lavender", Brushes.Lavender.Color, 4293322490);
            #endregion

            #region Test #61 - The LavenderBlush Property
            // Usage: SolidColorBrush = Brushes.LavenderBlush (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294963445
            CommonLib.LogStatus("Test #61 - The LavenderBlush Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.LavenderBlush, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("LavenderBlush", Brushes.LavenderBlush.Color, 4294963445);
            #endregion

            #region Test #62 - The LawnGreen Property
            // Usage: SolidColorBrush = Brushes.LawnGreen (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4286381056
            CommonLib.LogStatus("Test #62 - The LawnGreen Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.LawnGreen, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("LawnGreen", Brushes.LawnGreen.Color, 4286381056);
            #endregion

            #region Test #63 - The LemonChiffon Property
            // Usage: SolidColorBrush = Brushes.LemonChiffon (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294965965
            CommonLib.LogStatus("Test #63 - The LemonChiffon Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.LemonChiffon, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("LemonChiffon", Brushes.LemonChiffon.Color, 4294965965);
            #endregion

            #region Test #64 - The LightBlue Property
            // Usage: SolidColorBrush = Brushes.LightBlue (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4289583334
            CommonLib.LogStatus("Test #64 - The LightBlue Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.LightBlue, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("LightBlue", Brushes.LightBlue.Color, 4289583334);
            #endregion

            #region Test #65 - The LightCoral Property
            // Usage: SolidColorBrush = Brushes.LightCoral (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4293951616
            CommonLib.LogStatus("Test #65 - The LightCoral Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.LightCoral, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value
            _class_testresult &= _helper.CompareColorPropUint("LightCoral", Brushes.LightCoral.Color, 4293951616);
            #endregion

            #region Test #66 - The LightCyan Property
            // Usage: SolidColorBrush = Brushes.LightCyan (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4292935679
            CommonLib.LogStatus("Test #66 - The LightCyan Property");

            // Fill a Rectangle with the SolidColorBrush.
            P1.X = -20;
            P2.X = 0;
            P1 += V2;
            P2 += V2;
            DC.DrawRectangle(Brushes.LightCyan, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("LightCyan", Brushes.LightCyan.Color, 4292935679);
            #endregion

            #region Test #67 - The LightGoldenrodYellow Property
            // Usage: SolidColorBrush = Brushes.LightGoldenrodYellow (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294638290
            CommonLib.LogStatus("Test #67 - The LightGoldenrodYellow Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.LightGoldenrodYellow, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("LightGoldenrodYellow", Brushes.LightGoldenrodYellow.Color, 4294638290);
            #endregion

            #region Test #68 - The LightGray Property
            // Usage: SolidColorBrush = Brushes.LightGray (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4292072403
            CommonLib.LogStatus("Test #68 - The LightGray Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.LightGray, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("LightGray", Brushes.LightGray.Color, 4292072403);
            #endregion

            #region Test #69 - The LightGreen Property
            // Usage: SolidColorBrush = Brushes.LightGreen (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4287688336
            CommonLib.LogStatus("Test #69 - The LightGreen Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.LightGreen, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("LightGreen", Brushes.LightGreen.Color, 4287688336);
            #endregion

            #region Test #70 - The LightPink Property
            // Usage: SolidColorBrush = Brushes.LightPink (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294948545
            CommonLib.LogStatus("Test #70 - The LightPink Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.LightPink, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("LightPink", Brushes.LightPink.Color, 4294948545);
            #endregion

            #region Test #71 - The LightSalmon Property
            // Usage: SolidColorBrush = Brushes.LightSalmon (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294942842
            CommonLib.LogStatus("Test #71 - The LightSalmon Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.LightSalmon, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value
            _class_testresult &= _helper.CompareColorPropUint("LightSalmon", Brushes.LightSalmon.Color, 4294942842);
            #endregion

            #region Test #72 - The LightSeaGreen Property
            // Usage: SolidColorBrush = Brushes.LightSeaGreen (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4280332970
            CommonLib.LogStatus("Test #72 - The LightSeaGreen Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.LightSeaGreen, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("LightSeaGreen", Brushes.LightSeaGreen.Color, 4280332970);
            #endregion

            #region Test #73 - The LightSkyBlue Property
            // Usage: SolidColorBrush = Brushes.LightSkyBlue (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4287090426
            CommonLib.LogStatus("Test #73 - The LightSkyBlue Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.LightSkyBlue, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("LightSkyBlue", Brushes.LightSkyBlue.Color, 4287090426);
            #endregion

            #region Test #74 - The LightSlateGray Property
            // Usage: SolidColorBrush = Brushes.LightSlateGray (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4286023833
            CommonLib.LogStatus("Test #74 - The LightSlateGray Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.LightSlateGray, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("LightSlateGray", Brushes.LightSlateGray.Color, 4286023833);
            #endregion

            #region Test #75 - The LightSteelBlue Property
            // Usage: SolidColorBrush = Brushes.LightSteelBlue (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4289774814
            CommonLib.LogStatus("Test #75 - The LightSteelBlue Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.LightSteelBlue, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("LightSteelBlue", Brushes.LightSteelBlue.Color, 4289774814);
            #endregion

            #region Test #76 - The LightYellow Property
            // Usage: SolidColorBrush = Brushes.LightYellow (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294967264
            CommonLib.LogStatus("Test #76 - The LightYellow Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.LightYellow, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("LightYellow", Brushes.LightYellow.Color, 4294967264);
            #endregion

            #region Test #77 - The Lime Property
            // Usage: SolidColorBrush = Brushes.Lime (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4278255360
            CommonLib.LogStatus("Test #77 - The Lime Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Lime, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value
            _class_testresult &= _helper.CompareColorPropUint("Lime", Brushes.Lime.Color, 4278255360);
            #endregion

            #region Test #78 - The LimeGreen Property
            // Usage: SolidColorBrush = Brushes.LimeGreen (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4281519410
            CommonLib.LogStatus("Test #78 - The LimeGreen Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.LimeGreen, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("LimeGreen", Brushes.LimeGreen.Color, 4281519410);
            #endregion

            #region Test #79 - The Linen Property
            // Usage: SolidColorBrush = Brushes.Linen (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294635750
            CommonLib.LogStatus("Test #79 - The Linen Property");

            // Fill a Rectangle with the SolidColorBrush.
            P1.X = -20;
            P2.X = 0;
            P1 += V2;
            P2 += V2;
            DC.DrawRectangle(Brushes.Linen, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Linen", Brushes.Linen.Color, 4294635750);
            #endregion

            #region Test #80 - The Magenta Property
            // Usage: SolidColorBrush = Brushes.Magenta (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294902015
            CommonLib.LogStatus("Test #80 - The Magenta Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Magenta, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Magenta", Brushes.Magenta.Color, 4294902015);
            #endregion

            #region Test #81 - The Maroon Property
            // Usage: SolidColorBrush = Brushes.Maroon (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4286578688
            CommonLib.LogStatus("Test #81 - The Maroon Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Maroon, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Maroon", Brushes.Maroon.Color, 4286578688);
            #endregion

            #region Test #82 - The MediumAquamarine Property
            // Usage: SolidColorBrush = Brushes.MediumAquamarine (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4284927402
            CommonLib.LogStatus("Test #82 - The MediumAquamarine Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.MediumAquamarine, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("MediumAquamarine", Brushes.MediumAquamarine.Color, 4284927402);
            #endregion

            #region Test #83 - The MediumBlue Property
            // Usage: SolidColorBrush = Brushes.MediumBlue (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4278190285
            CommonLib.LogStatus("Test #83 - The MediumBlue Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.MediumBlue, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("MediumBlue", Brushes.MediumBlue.Color, 4278190285);
            #endregion

            #region Test #84 - The MediumOrchid Property
            // Usage: SolidColorBrush = Brushes.MediumOrchid (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4290401747
            CommonLib.LogStatus("Test #84 - The MediumOrchid Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.MediumOrchid, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("MediumOrchid", Brushes.MediumOrchid.Color, 4290401747);
            #endregion

            #region Test #85 - The MediumPurple Property
            // Usage: SolidColorBrush = Brushes.MediumPurple (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4287852763
            CommonLib.LogStatus("Test #85 - The MediumPurple Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.MediumPurple, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("MediumPurple", Brushes.MediumPurple.Color, 4287852763);
            #endregion

            #region Test #86 - The MediumSeaGreen Property
            // Usage: SolidColorBrush = Brushes.MediumSeaGreen (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4282168177
            CommonLib.LogStatus("Test #86 - The MediumSeaGreen Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.MediumSeaGreen, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value
            _class_testresult &= _helper.CompareColorPropUint("MediumSeaGreen", Brushes.MediumSeaGreen.Color, 4282168177);
            #endregion

            #region Test #87 - The MediumSlateBlue Property
            // Usage: SolidColorBrush = Brushes.MediumSlateBlue (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4286277870
            CommonLib.LogStatus("Test #87 - The MediumSlateBlue Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.MediumSlateBlue, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("MediumSlateBlue", Brushes.MediumSlateBlue.Color, 4286277870);
            #endregion

            #region Test #88 - The MediumSpringGreen Property
            // Usage: SolidColorBrush = Brushes.MediumSpringGreen (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4278254234
            CommonLib.LogStatus("Test #88 - The MediumSpringGreen Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.MediumSpringGreen, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("MediumSpringGreen", Brushes.MediumSpringGreen.Color, 4278254234);
            #endregion

            #region Test #89 - The MediumTurquoise Property
            // Usage: SolidColorBrush = Brushes.MediumTurquoise (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4282962380
            CommonLib.LogStatus("Test #89 - The MediumTurquoise Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.MediumTurquoise, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("MediumTurquoise", Brushes.MediumTurquoise.Color, 4282962380);
            #endregion

            #region Test #90 - The MediumVioletRed Property
            // Usage: SolidColorBrush = Brushes.MediumVioletRed (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4291237253
            CommonLib.LogStatus("Test #90 - The MediumVioletRed Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.MediumVioletRed, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("MediumVioletRed", Brushes.MediumVioletRed.Color, 4291237253);
            #endregion

            #region Test #91 - The MidnightBlue Property
            // Usage: SolidColorBrush = Brushes.MidnightBlue (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4279834992
            CommonLib.LogStatus("Test #91 - The MidnightBlue Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.MidnightBlue, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("MidnightBlue", Brushes.MidnightBlue.Color, 4279834992);
            #endregion

            #region Test #92 - The MintCream Property
            // Usage: SolidColorBrush = Brushes.MintCream (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294311930
            CommonLib.LogStatus("Test #92 - The MintCream Property");

            // Fill a Rectangle with the SolidColorBrush.
            P1.X = -20;
            P2.X = 0;
            P1 += V2;
            P2 += V2;
            DC.DrawRectangle(Brushes.MintCream, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("MintCream", Brushes.MintCream.Color, 4294311930);
            #endregion

            #region Test #93 - The MistyRose Property
            // Usage: SolidColorBrush = Brushes.MistyRose (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294960353
            CommonLib.LogStatus("Test #93 - The MistyRose Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.MistyRose, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("MistyRose", Brushes.MistyRose.Color, 4294960353);
            #endregion

            #region Test #94 - The Moccasin Property
            // Usage: SolidColorBrush = Brushes.Moccasin (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294960309
            CommonLib.LogStatus("Test #94 - The Moccasin Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Moccasin, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Moccasin", Brushes.Moccasin.Color, 4294960309);
            #endregion

            #region Test #95 - The NavajoWhite Property
            // Usage: SolidColorBrush = Brushes.NavajoWhite (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294958765
            CommonLib.LogStatus("Test #95 - The NavajoWhite Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.NavajoWhite, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("NavajoWhite", Brushes.NavajoWhite.Color, 4294958765);
            #endregion

            #region Test #96 - The Navy Property
            // Usage: SolidColorBrush = Brushes.Navy (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4278190208
            CommonLib.LogStatus("Test #96 - The Navy Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Navy, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Navy", Brushes.Navy.Color, 4278190208);
            #endregion

            #region Test #97 - The OldLace Property
            // Usage: SolidColorBrush = Brushes.OldLace (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294833638
            CommonLib.LogStatus("Test #97 - The OldLace Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.OldLace, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("OldLace", Brushes.OldLace.Color, 4294833638);
            #endregion

            #region Test #98 - The Olive Property
            // Usage: SolidColorBrush = Brushes.Olive (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4286611456
            CommonLib.LogStatus("Test #98 - The Olive Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Olive, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Olive", Brushes.Olive.Color, 4286611456);
            #endregion

            #region Test #99 - The OliveDrab Property
            // Usage: SolidColorBrush = Brushes.OliveDrab (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4285238819
            CommonLib.LogStatus("Test #99 - The OliveDrab Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.OliveDrab, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("OliveDrab", Brushes.OliveDrab.Color, 4285238819);
            #endregion

            #region Test #100 - The Orange Property
            // Usage: SolidColorBrush = Brushes.Orange (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294944000
            CommonLib.LogStatus("Test #100 - The Orange Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Orange, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Orange", Brushes.Orange.Color, 4294944000);
            #endregion

            #region Test #101 - The OrangeRed Property
            // Usage: SolidColorBrush = Brushes.OrangeRed (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294919424
            CommonLib.LogStatus("Test #101 - The OrangeRed Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.OrangeRed, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("OrangeRed", Brushes.OrangeRed.Color, 4294919424);
            #endregion

            #region Test #102 - The Orchid Property
            // Usage: SolidColorBrush = Brushes.Orchid (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4292505814
            CommonLib.LogStatus("Test #102 - The Orchid Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Orchid, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Orchid", Brushes.Orchid.Color, 4292505814);
            #endregion

            #region Test #103 - The PaleGoldenrod Property
            // Usage: SolidColorBrush = Brushes.PaleGoldenrod (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4293847210
            CommonLib.LogStatus("Test #103 - The PaleGoldenrod Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.PaleGoldenrod, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value
            _class_testresult &= _helper.CompareColorPropUint("PaleGoldenrod", Brushes.PaleGoldenrod.Color, 4293847210);
            #endregion

            #region Test #104 - The PaleGreen Property
            // Usage: SolidColorBrush = Brushes.PaleGreen (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4288215960
            CommonLib.LogStatus("Test #104 - The PaleGreen Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.PaleGreen, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("PaleGreen", Brushes.PaleGreen.Color, 4288215960);
            #endregion

            #region Test #105 - The PaleTurquoise Property
            // Usage: SolidColorBrush = Brushes.PaleTurquoise (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4289720046
            CommonLib.LogStatus("Test #105 - The PaleTurquoise Property");

            // Fill a Rectangle with the SolidColorBrush.
            P1.X = -20;
            P2.X = 0;
            P1 += V2;
            P2 += V2;
            DC.DrawRectangle(Brushes.PaleTurquoise, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("PaleTurquoise", Brushes.PaleTurquoise.Color, 4289720046);
            #endregion

            #region Test #106 - The PaleVioletRed Property
            // Usage: SolidColorBrush = Brushes.PaleVioletRed (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4292571283
            CommonLib.LogStatus("Test #106 - The PaleVioletRed Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.PaleVioletRed, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("PaleVioletRed", Brushes.PaleVioletRed.Color, 4292571283);
            #endregion

            #region Test #107 - The PapayaWhip Property
            // Usage: SolidColorBrush = Brushes.PapayaWhip (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294963157
            CommonLib.LogStatus("Test #107 - The PapayaWhip Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.PapayaWhip, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("PapayaWhip", Brushes.PapayaWhip.Color, 4294963157);
            #endregion

            #region Test #108 - The PeachPuff Property
            // Usage: SolidColorBrush = Brushes.PeachPuff (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294957753
            CommonLib.LogStatus("Test #108 - The PeachPuff Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.PeachPuff, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("PeachPuff", Brushes.PeachPuff.Color, 4294957753);
            #endregion

            #region Test #109 - The Peru Property
            // Usage: SolidColorBrush = Brushes.Peru (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4291659071
            CommonLib.LogStatus("Test #109 - The Peru Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Peru, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Peru", Brushes.Peru.Color, 4291659071);
            #endregion

            #region Test #110 - The Pink Property
            // Usage: SolidColorBrush = Brushes.Pink (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294951115
            CommonLib.LogStatus("Test #110 - The Pink Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Pink, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Pink", Brushes.Pink.Color, 4294951115);
            #endregion

            #region Test #111 - The Plum Property
            // Usage: SolidColorBrush = Brushes.Plum (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4292714717
            CommonLib.LogStatus("Test #111 - The Plum Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Plum, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Plum", Brushes.Plum.Color, 4292714717);
            #endregion

            #region Test #112 - The PowderBlue Property
            // Usage: SolidColorBrush = Brushes.PowderBlue (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4289781990
            CommonLib.LogStatus("Test #112 - The PowderBlue Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.PowderBlue, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("PowderBlue", Brushes.PowderBlue.Color, 4289781990);
            #endregion

            #region Test #113 - The Purple Property
            // Usage: SolidColorBrush = Brushes.Purple (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4286578816
            CommonLib.LogStatus("Test #113 - The Purple Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Purple, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Purple", Brushes.Purple.Color, 4286578816);
            #endregion

            #region Test #114 - The Red Property
            // Usage: SolidColorBrush = Brushes.Red (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294901760
            CommonLib.LogStatus("Test #114 - The Red Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Red, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Red", Brushes.Red.Color, 4294901760);
            #endregion

            #region Test #115 - The RosyBrown Property
            // Usage: SolidColorBrush = Brushes.RosyBrown (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4290547599
            CommonLib.LogStatus("Test #115 - The RosyBrown Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.RosyBrown, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("RosyBrown", Brushes.RosyBrown.Color, 4290547599);
            #endregion

            #region Test #116 - The RoyalBlue Property
            // Usage: SolidColorBrush = Brushes.RoyalBlue (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4282477025
            CommonLib.LogStatus("Test #116 - The RoyalBlue Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.RoyalBlue, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("RoyalBlue", Brushes.RoyalBlue.Color, 4282477025);
            #endregion

            #region Test #117 - The SaddleBrown Property
            // Usage: SolidColorBrush = Brushes.SaddleBrown (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4287317267
            CommonLib.LogStatus("Test #117 - The SaddleBrown Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.SaddleBrown, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("SaddleBrown", Brushes.SaddleBrown.Color, 4287317267);
            #endregion

            #region Test #118 - The Salmon Property
            // Usage: SolidColorBrush = Brushes.Salmon (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294606962
            CommonLib.LogStatus("Test #118 - The Salmon Property");

            // Fill a Rectangle with the SolidColorBrush.
            P1.X = -20;
            P2.X = 0;
            P1 += V2;
            P2 += V2;
            DC.DrawRectangle(Brushes.Salmon, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Salmon", Brushes.Salmon.Color, 4294606962);
            #endregion

            #region Test #119 - The SandyBrown Property
            // Usage: SolidColorBrush = Brushes.SandyBrown (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294222944
            CommonLib.LogStatus("Test #119 - The SandyBrown Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.SandyBrown, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("SandyBrown", Brushes.SandyBrown.Color, 4294222944);
            #endregion

            #region Test #120 - The SeaGreen Property
            // Usage: SolidColorBrush = Brushes.SeaGreen (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4281240407
            CommonLib.LogStatus("Test #120 - The SeaGreen Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.SeaGreen, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("SeaGreen", Brushes.SeaGreen.Color, 4281240407);
            #endregion

            #region Test #121 - The SeaShell Property
            // Usage: SolidColorBrush = Brushes.SeaShell (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294964718
            CommonLib.LogStatus("Test #121 - The SeaShell Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.SeaShell, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("SeaShell", Brushes.SeaShell.Color, 4294964718);
            #endregion

            #region Test #122 - The Sienna Property
            // Usage: SolidColorBrush = Brushes.Sienna (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4288696877
            CommonLib.LogStatus("Test #122 - The Sienna Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Sienna, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Sienna", Brushes.Sienna.Color, 4288696877);
            #endregion

            #region Test #123 - The Silver Property
            // Usage: SolidColorBrush = Brushes.Silver (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4290822336
            CommonLib.LogStatus("Test #123 - The Silver Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Silver, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Silver", Brushes.Silver.Color, 4290822336);
            #endregion

            #region Test #124 - The SkyBlue Property
            // Usage: SolidColorBrush = Brushes.SkyBlue (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4287090411
            CommonLib.LogStatus("Test #124 - The SkyBlue Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.SkyBlue, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("SkyBlue", Brushes.SkyBlue.Color, 4287090411);
            #endregion

            #region Test #125 - The SlateBlue Property
            // Usage: SolidColorBrush = Brushes.SlateBlue (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4285160141
            CommonLib.LogStatus("Test #125 - The SlateBlue Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.SlateBlue, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("SlateBlue", Brushes.SlateBlue.Color, 4285160141);
            #endregion

            #region Test #126 - The SlateGray Property
            // Usage: SolidColorBrush = Brushes.SlateGray (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4285563024
            CommonLib.LogStatus("Test #126 - The SlateGray Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.SlateGray, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("SlateGray", Brushes.SlateGray.Color, 4285563024);
            #endregion

            #region Test #127 - The Snow Property
            // Usage: SolidColorBrush = Brushes.Snow (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294966010
            CommonLib.LogStatus("Test #127 - The Snow Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Snow, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Snow", Brushes.Snow.Color, 4294966010);
            #endregion

            #region Test #128 - The SpringGreen Property
            // Usage: SolidColorBrush = Brushes.SpringGreen (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4278255487
            CommonLib.LogStatus("Test #128 - The SpringGreen Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.SpringGreen, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("SpringGreen", Brushes.SpringGreen.Color, 4278255487);
            #endregion

            #region Test #129 - The SteelBlue Property
            // Usage: SolidColorBrush = Brushes.SteelBlue (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4282811060
            CommonLib.LogStatus("Test #129 - The SteelBlue Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.SteelBlue, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value
            _class_testresult &= _helper.CompareColorPropUint("SteelBlue", Brushes.SteelBlue.Color, 4282811060);
            #endregion

            #region Test #130 - The Tan Property
            // Usage: SolidColorBrush = Brushes.Tan (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4291998860
            CommonLib.LogStatus("Test #130 - The Tan Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Tan, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Tan", Brushes.Tan.Color, 4291998860);
            #endregion

            #region Test #131 - The Teal Property
            // Usage: SolidColorBrush = Brushes.Teal (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4278222976
            CommonLib.LogStatus("Test #131 - The Teal Property");

            // Fill a Rectangle with the SolidColorBrush.
            P1.X = -20;
            P2.X = 0;
            P1 += V2;
            P2 += V2;
            DC.DrawRectangle(Brushes.Teal, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Teal", Brushes.Teal.Color, 4278222976);
            #endregion

            #region Test #132 - The Thistle Property
            // Usage: SolidColorBrush = Brushes.Thistle (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4292394968
            CommonLib.LogStatus("Test #132 - The Thistle Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Thistle, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Thistle", Brushes.Thistle.Color, 4292394968);
            #endregion

            #region Test #133 - The Tomato Property
            // Usage: SolidColorBrush = Brushes.Tomato (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294927175
            CommonLib.LogStatus("Test #133 - The Tomato Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Tomato, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Tomato", Brushes.Tomato.Color, 4294927175);
            #endregion

            #region Test #134 - The Transparent Property
            // Usage: SolidColorBrush = Brushes.Transparent (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 16777215
            CommonLib.LogStatus("Test #134 - The Transparent Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Transparent, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Transparent", Brushes.Transparent.Color, 16777215);
            #endregion

            #region Test #135 - The Turquoise Property
            // Usage: SolidColorBrush = Brushes.Turquoise (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4282441936
            CommonLib.LogStatus("Test #135 - The Turquoise Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Turquoise, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Turquoise", Brushes.Turquoise.Color, 4282441936);
            #endregion

            #region Test #136 - The Violet Property
            // Usage: SolidColorBrush = Brushes.Violet (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4293821166
            CommonLib.LogStatus("Test #136 - The Violet Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Violet, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Violet", Brushes.Violet.Color, 4293821166);
            #endregion

            #region Test #137 - The Wheat Property
            // Usage: SolidColorBrush = Brushes.Wheat (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294303411
            CommonLib.LogStatus("Test #137 - The Wheat Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Wheat, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Wheat", Brushes.Wheat.Color, 4294303411);
            #endregion

            #region Test #138 - The White Property
            // Usage: SolidColorBrush = Brushes.White (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294967295
            CommonLib.LogStatus("Test #138 - The White Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.White, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("White", Brushes.White.Color, 4294967295);
            #endregion

            #region Test #139 - The WhiteSmoke Property
            // Usage: SolidColorBrush = Brushes.WhiteSmoke (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294309365
            CommonLib.LogStatus("Test #139 - The WhiteSmoke Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.WhiteSmoke, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("WhiteSmoke", Brushes.WhiteSmoke.Color, 4294309365);
            #endregion

            #region Test #140 - The Yellow Property
            // Usage: SolidColorBrush = Brushes.Yellow (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4294967040
            CommonLib.LogStatus("Test #140 - The Yellow Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.Yellow, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("Yellow", Brushes.Yellow.Color, 4294967040);
            #endregion

            #region Test #141 - The YellowGreen Property
            // Usage: SolidColorBrush = Brushes.YellowGreen (Read only)
            // Notes: Returns a new SolidColorBrush object with Color = 4288335154
            CommonLib.LogStatus("Test #141 - The YellowGreen Property");

            // Fill a Rectangle with the SolidColorBrush.
            DC.DrawRectangle(Brushes.YellowGreen, null, new Rect((P1 += V1), (P2 += V1)));

            // Confirm that the Color has the expected value.
            _class_testresult &= _helper.CompareColorPropUint("YellowGreen", Brushes.YellowGreen.Color, 4288335154);
            #endregion
            #endregion

            #region TEST LOGGING

            // Log the programmatic result for this API test using the
            // Automation Framework LogTest method.  If This result is False,
            // it will override the result of a Visual Comparator.  Conversely,
            // if a Visual Comparator is False it will override a True result
            // from this test
            CommonLib.LogTest("Result for: "+_objectType );
            #endregion End of TEST LOGGING
        }

        private bool _class_testresult;
        private Type _objectType;
        private HelperClass _helper;
    }
}
