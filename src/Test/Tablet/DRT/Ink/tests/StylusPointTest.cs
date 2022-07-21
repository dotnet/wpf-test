// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Ink;

namespace DRT
{
	/// <summary>
	/// Summary description for saveload.
	/// </summary>
    [TestedSecurityLevelAttribute (SecurityLevel.PartialTrust)]
    public class StylusPointTest : DrtInkTestcase
	{
        public void TestStylusPoint()
        {
            //
            // start simple
            //
            double x = 234.23423d;
            double y = 23353245.42342d;
            StylusPoint sp = new StylusPoint(x, y);
            if (sp.X != x || sp.Y != y)
            {
                throw new InvalidOperationException("Invalid StylusPoint X, Y");
            }

            float pressure = 0.7894f;
            sp = new StylusPoint(x, y, pressure);
            if (sp.X != x || sp.Y != y || sp.PressureFactor != pressure)
            {
                throw new InvalidOperationException("Invalid StylusPoint X, Y, P");
            }

            x = -455623.23;
            y = -11334234.2344;
            pressure = 0.01f;

            sp.X = x;
            sp.Y = y;
            sp.PressureFactor = pressure;
            if (sp.X != x || sp.Y != y || sp.PressureFactor != pressure)
            {
                throw new InvalidOperationException("Invalid StylusPoint X, Y, P");
            }

            //
            // test boundaries
            //
            x = StylusPoint.MinXY;
            y = StylusPoint.MinXY;
            pressure = 0.00f;
            sp.X = x;
            sp.Y = y;
            sp.PressureFactor = pressure;
            if (sp.X != x || sp.Y != y || sp.PressureFactor != pressure)
            {
                throw new InvalidOperationException("Invalid StylusPoint X, Y, P");
            }

            x = StylusPoint.MaxXY;
            y = StylusPoint.MaxXY;
            pressure = 1.0f;
            sp.X = x;
            sp.Y = y;
            sp.PressureFactor = pressure;
            if (sp.X != x || sp.Y != y || sp.PressureFactor != pressure)
            {
                throw new InvalidOperationException("Invalid StylusPoint X, Y, P");
            }

            //
            // now create an interesting styluspoint
            //
            List<StylusPointPropertyInfo> infos = new List<StylusPointPropertyInfo>();
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.X));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.Y));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.NormalPressure));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.XTiltOrientation));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.YTiltOrientation));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.PacketStatus));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.TipButton));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.BarrelButton));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.SecondaryTipButton));

            StylusPointDescription description =
                new StylusPointDescription(infos);

            x = 10.0;
            y = -10.0;
            pressure = 0.5f;
            int xTilt = 10;
            int yTilt = 20;
            int packStatus = 30;
            int tipButton = 1;
            int barButton = 0;
            int secButton = 1;
            int[] additionalData = 
                new int[] { 
                            xTilt,
                            yTilt,
                            packStatus,
                            tipButton,
                            barButton,
                            secButton,
                          };
            sp = new StylusPoint(x, y, pressure, description, additionalData);

            //
            // test getvalue / setvalue
            //
            if (sp.X != x || 
                sp.Y != y || 
                sp.PressureFactor != pressure ||
                sp.GetPropertyValue(StylusPointProperties.X) != (int)x ||
                sp.GetPropertyValue(StylusPointProperties.Y) != (int)y ||
                sp.GetPropertyValue(StylusPointProperties.NormalPressure) != 511 ||
                sp.GetPropertyValue(StylusPointProperties.XTiltOrientation) != xTilt ||
                sp.GetPropertyValue(StylusPointProperties.YTiltOrientation) != yTilt ||
                sp.GetPropertyValue(StylusPointProperties.PacketStatus) != packStatus ||
                sp.GetPropertyValue(StylusPointProperties.TipButton) != tipButton ||
                sp.GetPropertyValue(StylusPointProperties.BarrelButton) != barButton ||
                sp.GetPropertyValue(StylusPointProperties.SecondaryTipButton) != secButton
                )
            {
                throw new InvalidOperationException("Invalid StylusPoint data!");
            }

            x = -10.0;
            y = 10.0;
            pressure = 0.0f;
            xTilt = -10;
            yTilt = -20;
            packStatus = -30;
            tipButton = 0;
            barButton = 1;
            secButton = 0;

            sp.SetPropertyValue(StylusPointProperties.X, (int)x);
            sp.SetPropertyValue(StylusPointProperties.Y, (int)y);
            sp.SetPropertyValue(StylusPointProperties.NormalPressure, 0);
            sp.SetPropertyValue(StylusPointProperties.XTiltOrientation, xTilt);
            sp.SetPropertyValue(StylusPointProperties.YTiltOrientation, yTilt);
            sp.SetPropertyValue(StylusPointProperties.PacketStatus, packStatus);
            sp.SetPropertyValue(StylusPointProperties.TipButton, tipButton);
            sp.SetPropertyValue(StylusPointProperties.BarrelButton, barButton);
            sp.SetPropertyValue(StylusPointProperties.SecondaryTipButton, secButton);

            if (sp.X != x ||
                sp.Y != y ||
                sp.PressureFactor != pressure ||
                sp.GetPropertyValue(StylusPointProperties.X) != (int)x ||
                sp.GetPropertyValue(StylusPointProperties.Y) != (int)y ||
                sp.GetPropertyValue(StylusPointProperties.NormalPressure) != 0 ||
                sp.GetPropertyValue(StylusPointProperties.XTiltOrientation) != xTilt ||
                sp.GetPropertyValue(StylusPointProperties.YTiltOrientation) != yTilt ||
                sp.GetPropertyValue(StylusPointProperties.PacketStatus) != packStatus ||
                sp.GetPropertyValue(StylusPointProperties.TipButton) != tipButton ||
                sp.GetPropertyValue(StylusPointProperties.BarrelButton) != barButton ||
                sp.GetPropertyValue(StylusPointProperties.SecondaryTipButton) != secButton
                )
            {
                throw new InvalidOperationException("Invalid StylusPoint data!");
            }

            //
            // test serialization
            //
            StylusPointCollection spc = new StylusPointCollection(description);
            sp.X = x = 0.0f;
            sp.Y = y = 0.0f;
            spc.Add(sp);
            spc.Add(sp);
            Stroke s = new Stroke(spc);
            StrokeCollection sc = new StrokeCollection();
            sc.Add(s);
            using (MemoryStream stream = new MemoryStream())
            {
                sc.Save(stream, true);
                stream.Position = 0;
                StrokeCollection sc2 = new StrokeCollection(stream);
                if (sc2.Count != 1 ||
                    sc.Count != 1 ||
                    sc2[0].StylusPoints.Count != 2 ||
                    sc[0].StylusPoints.Count != 2)
                {
                    throw new InvalidOperationException("Invalid save - load of data!");
                }

                StylusPoint spS1 = sc2[0].StylusPoints[0];
                StylusPoint spS2 = sc[0].StylusPoints[0];
                if (spS1 == spS2)
                {
                    throw new InvalidOperationException("Button data wasn't stripped during ISF encoding!");
                }

                if (spS1.X != x ||
                    spS1.Y != y ||
                    spS1.PressureFactor != pressure ||
                    spS1.GetPropertyValue(StylusPointProperties.X) != (int)x ||
                    spS1.GetPropertyValue(StylusPointProperties.Y) != (int)y ||
                    spS1.GetPropertyValue(StylusPointProperties.NormalPressure) != 0 ||
                    spS1.GetPropertyValue(StylusPointProperties.XTiltOrientation) != xTilt ||
                    spS1.GetPropertyValue(StylusPointProperties.YTiltOrientation) != yTilt ||
                    spS1.GetPropertyValue(StylusPointProperties.PacketStatus) != packStatus ||
                    spS1.Description.HasProperty(StylusPointProperties.TipButton) ||
                    spS1.Description.HasProperty(StylusPointProperties.BarrelButton) ||
                    spS1.Description.HasProperty(StylusPointProperties.SecondaryTipButton)
                )
                {
                    throw new InvalidOperationException("Invalid StylusPoint data across serialization!");
                }

                spS1 = sc2[0].StylusPoints[1];
                spS2 = sc[0].StylusPoints[1];
                if (spS1 == spS2)
                {
                    throw new InvalidOperationException("Button data wasn't stripped during ISF encoding!");
                }

                if (spS1.X != x ||
                    spS1.Y != y ||
                    spS1.PressureFactor != pressure ||
                    spS1.GetPropertyValue(StylusPointProperties.X) != (int)x ||
                    spS1.GetPropertyValue(StylusPointProperties.Y) != (int)y ||
                    spS1.GetPropertyValue(StylusPointProperties.NormalPressure) != 0 ||
                    spS1.GetPropertyValue(StylusPointProperties.XTiltOrientation) != xTilt ||
                    spS1.GetPropertyValue(StylusPointProperties.YTiltOrientation) != yTilt ||
                    spS1.GetPropertyValue(StylusPointProperties.PacketStatus) != packStatus ||
                    spS1.Description.HasProperty(StylusPointProperties.TipButton) ||
                    spS1.Description.HasProperty(StylusPointProperties.BarrelButton) ||
                    spS1.Description.HasProperty(StylusPointProperties.SecondaryTipButton)
                )
                {
                    throw new InvalidOperationException("Invalid StylusPoint data across serialization!");
                }
            }

            //test serialization without buttons

            List<StylusPointPropertyInfo> infos3 = new List<StylusPointPropertyInfo>();
            infos3.Add(new StylusPointPropertyInfo(StylusPointProperties.X));
            infos3.Add(new StylusPointPropertyInfo(StylusPointProperties.Y));
            infos3.Add(new StylusPointPropertyInfo(StylusPointProperties.NormalPressure));
            infos3.Add(new StylusPointPropertyInfo(StylusPointProperties.XTiltOrientation));

            StylusPointDescription description3 =
                new StylusPointDescription(infos3);

            StylusPoint spNB1 = new StylusPoint(0, 0, 0.5f, description3, new int[] { 10 });
            StylusPointCollection stylusPointsNoButtons = new StylusPointCollection(description3);
            stylusPointsNoButtons.Add(spNB1);
            stylusPointsNoButtons.Add(spNB1);
            stylusPointsNoButtons.Add(spNB1);

            Stroke strokeNoButtons = new Stroke(stylusPointsNoButtons);
            StrokeCollection scNB = new StrokeCollection(new Stroke[]{strokeNoButtons});
            using (MemoryStream stream = new MemoryStream())
            {
                scNB.Save(stream, true);
                stream.Position = 0;
                StrokeCollection scNB2 = new StrokeCollection(stream);
                if (scNB.Count != 1 ||
                    scNB2.Count != 1 ||
                    scNB[0].StylusPoints.Count != 3 ||
                    scNB2[0].StylusPoints.Count != 3 ||
                    scNB[0].StylusPoints[0] != scNB2[0].StylusPoints[0] ||
                    scNB[0].StylusPoints[1] != scNB2[0].StylusPoints[1] ||
                    scNB[0].StylusPoints[2] != scNB2[0].StylusPoints[2])
                {
                    throw new InvalidOperationException("Invalid StylusPoint data across serialization!");
                }
            }


            //
            // test copy on assignment
            //
            StylusPoint sp2 = sp;
            sp.SetPropertyValue(StylusPointProperties.SecondaryTipButton, secButton + 1);
            sp.SetPropertyValue(StylusPointProperties.XTiltOrientation, xTilt + 1);

            if ( sp.GetPropertyValue(StylusPointProperties.XTiltOrientation) ==
                 sp2.GetPropertyValue(StylusPointProperties.XTiltOrientation)
                 ||
                 sp.GetPropertyValue(StylusPointProperties.SecondaryTipButton) ==
                 sp2.GetPropertyValue(StylusPointProperties.SecondaryTipButton)
                 )
            {
                throw new InvalidOperationException("We're not copying on assignment in StylusPoint!");
            }

            //
            // test default ctor
            //
            StylusPoint def = new StylusPoint();
            if (def.Description.PropertyCount != 3)
            {
                throw new InvalidOperationException("Something is wrong with StylusPoint default ctor");
            }

            //
            // test our .Equals and .GetHashCode
            //
            if (sp2 == sp || !(sp2 != sp))
            {
                throw new InvalidOperationException("StylusPoint.Equals is incorrect!");
            }
            if (sp2.GetHashCode() == sp.GetHashCode())
            {
                throw new InvalidOperationException("StylusPoint.GetHashCode is incorrect!");
            }
            

            //
            // test copy on assignment again
            //
            StylusPoint sp3 = sp;
            sp3.SetPropertyValue(StylusPointProperties.SecondaryTipButton, 0);
            sp3.SetPropertyValue(StylusPointProperties.XTiltOrientation, xTilt + 2);

            if (sp.GetPropertyValue(StylusPointProperties.XTiltOrientation) ==
                 sp3.GetPropertyValue(StylusPointProperties.XTiltOrientation)
                 ||
                 sp.GetPropertyValue(StylusPointProperties.SecondaryTipButton) ==
                 sp3.GetPropertyValue(StylusPointProperties.SecondaryTipButton)
                 )
            {
                throw new InvalidOperationException("We're not copying on assignment in StylusPoint!");
            }

            sp.SetPropertyValue(StylusPointProperties.SecondaryTipButton, 0);
            sp.SetPropertyValue(StylusPointProperties.XTiltOrientation, xTilt + 2);


            if (sp3 != sp || !(sp3 == sp))
            {
                throw new InvalidOperationException("StylusPoint.Equals is incorrect!");
            }

            //
            // now test our exceptions
            //
            bool exceptionCaught = false;
            try
            {
                StylusPoint testPoint =
                    new StylusPoint(0f, 0f, -0.000000000001f);
            }
            catch (ArgumentOutOfRangeException)
            {
                exceptionCaught = true;
            }

            if (!exceptionCaught)
            {
                throw new InvalidOperationException("No expected exception");
            }

            
            try
            {
                exceptionCaught = false;
                StylusPoint testPoint =
                    new StylusPoint(0f, 0f, 1.1f); //invalid pressure
            }
            catch (ArgumentOutOfRangeException)
            {
                exceptionCaught = true;
            }

            if (!exceptionCaught)
            {
                throw new InvalidOperationException("No expected exception");
            }

            int[] mismatchData = new int[] { 1 };
            try
            {
                exceptionCaught = false;
                StylusPoint testPoint =
                    new StylusPoint(0f, 0f, 0f, null, mismatchData);
            }
            catch (ArgumentNullException)
            {
                exceptionCaught = true;
            }

            if (!exceptionCaught)
            {
                throw new InvalidOperationException("No expected exception");
            }

            try
            {
                exceptionCaught = false;
                StylusPoint testPoint =
                    new StylusPoint(0f, 0f, 0f, description, null);
            }
            catch (ArgumentNullException)
            {
                exceptionCaught = true;
            }

            if (!exceptionCaught)
            {
                throw new InvalidOperationException("No expected exception");
            }

            try
            {
                exceptionCaught = false;
                StylusPoint testPoint =
                    new StylusPoint(0f, 0f, 0f, description, mismatchData);
            }
            catch (ArgumentException)
            {
                exceptionCaught = true;
            }

            if (!exceptionCaught)
            {
                throw new InvalidOperationException("No expected exception");
            }
            


            //
            // now make sure we can serialize to himetric at our boundaries
            //
            StylusPoint boundary = new StylusPoint(StylusPoint.MinXY, StylusPoint.MaxXY);
            StylusPointCollection boundaryCollection = new StylusPointCollection();
            boundaryCollection.Add(boundary);

            int[] isfData = boundaryCollection.ToHiMetricArray();

            if (isfData[0] != Int32.MinValue ||
                isfData[1] != Int32.MaxValue)
            {
                throw new InvalidOperationException("Invalid Min / Max on StylusPoint");
            }

            //even when we go over
            StylusPoint boundary2 = new StylusPoint(Double.MinValue, Double.MaxValue);
            StylusPointCollection boundaryCollection2 = new StylusPointCollection();
            boundaryCollection2.Add(boundary2);

            int[] isfData2 = boundaryCollection2.ToHiMetricArray();

            if (isfData2[0] != Int32.MinValue ||
                isfData2[1] != Int32.MaxValue)
            {
                throw new InvalidOperationException("Invalid Min / Max on StylusPoint");
            }
        }


        public void TestStylusPointCollection()
        {
            int eventCount = 0;
            int expectedEventCount = 0;
            //
            // StylusPointCollection.Changed anon delegate
            //
            EventHandler spcChangedAnonymousDelegate =
                delegate(object sender, EventArgs e)
                {
                    eventCount++;
                };

            StylusPointCollection collection
                = new StylusPointCollection();
            collection.Changed += spcChangedAnonymousDelegate;

            StylusPoint p1 = new StylusPoint(0.0d, 0.0d, 0.0f);
            StylusPoint p2 = new StylusPoint(0.5d, 0.5d, 0.5f);
            StylusPoint p3 = new StylusPoint(1.0d, 1.0d, 1.0f);

            //
            // simple IList tests
            //
            expectedEventCount++;
            collection.Add(p1);
            if (collection.Count != 1)
            {
                throw new InvalidOperationException("Unexpected count");
            }
            if (eventCount != expectedEventCount)
            {
                throw new InvalidOperationException("Unexpected event count");
            }

            expectedEventCount += 2;
            collection.Add(p2);
            collection.Add(p3);
            if (collection.Count != 3)
            {
                throw new InvalidOperationException("Unexpected count");
            }
            if (eventCount != expectedEventCount)
            {
                throw new InvalidOperationException("Unexpected event count");
            }
            if (collection.IndexOf(p1) != 0 ||
                collection.IndexOf(p2) != 1 ||
                collection.IndexOf(p3) != 2 ||
                collection.IndexOf(new StylusPoint(2.0d, 2.0d)) != -1)
            {
                throw new InvalidOperationException("StylusPointCollection.IndexOf returned bad results");
            }

            expectedEventCount++;
            collection.Remove(p2);
            if (collection.Count != 2)
            {
                throw new InvalidOperationException("Unexpected count");
            }
            if (eventCount != expectedEventCount)
            {
                throw new InvalidOperationException("Unexpected event count");
            }
            if (collection.IndexOf(p1) != 0 ||
                collection.IndexOf(p3) != 1)
            {
                throw new InvalidOperationException("StylusPointCollection.IndexOf returned bad results");
            }

            expectedEventCount++;
            collection.Insert(1, p2);
            if (collection.Count != 3)
            {
                throw new InvalidOperationException("Unexpected count");
            }
            if (eventCount != expectedEventCount)
            {
                throw new InvalidOperationException("Unexpected event count");
            }
            if (collection.IndexOf(p1) != 0 ||
                collection.IndexOf(p2) != 1 ||
                collection.IndexOf(p3) != 2)
            {
                throw new InvalidOperationException("StylusPointCollection.IndexOf returned bad results");
            }

            expectedEventCount++;
            collection.RemoveAt(0);
            if (collection.Count != 2)
            {
                throw new InvalidOperationException("Unexpected count");
            }
            if (eventCount != expectedEventCount)
            {
                throw new InvalidOperationException("Unexpected event count");
            }
            if (collection.IndexOf(p2) != 0 || 
                collection.IndexOf(p3) != 1)
            {
                throw new InvalidOperationException("StylusPointCollection.IndexOf returned bad results");
            }

            expectedEventCount++;
            StylusPointCollection collection2 = new StylusPointCollection();
            collection2.Add(p1);
            collection.Add(collection2);
            if (collection.Count != 3)
            {
                throw new InvalidOperationException("Unexpected count");
            }
            if (eventCount != expectedEventCount)
            {
                throw new InvalidOperationException("Unexpected event count");
            }
            if (collection.IndexOf(p2) != 0 ||
                collection.IndexOf(p3) != 1 ||
                collection.IndexOf(p1) != 2)
            {
                throw new InvalidOperationException("StylusPointCollection.IndexOf returned bad results");
            }


            //
            // test exceptions
            //
            Stroke stroke = new Stroke(collection);
            int preCount = collection.Count;
            bool exceptionCaught = false;
            try
            {
                collection.Clear();
            }
            catch (InvalidOperationException)
            {
                exceptionCaught = true;
            }

            if (!exceptionCaught)
            {
                throw new InvalidOperationException("No expected exception");
            }

            if (collection.Count != preCount)
            {
                throw new InvalidOperationException("StylusPointCollection was left in an invalid state!");
            }

            while (collection.Count > 1)
            {
                collection.RemoveAt(collection.Count - 1);
            }
            preCount = collection.Count;
            try
            {
                exceptionCaught = false;
                collection.RemoveAt(0);
            }
            catch (InvalidOperationException)
            {
                exceptionCaught = true;
            }

            if (!exceptionCaught)
            {
                throw new InvalidOperationException("No expected exception");
            }

            if (collection.Count != preCount)
            {
                throw new InvalidOperationException("StylusPointCollection was left in an invalid state!");
            }

            stroke.StylusPoints = new StylusPointCollection(new StylusPoint[] { new StylusPoint(1, 1) });

            //
            // now that collection is disconnected, we should be able to go to zero
            //
            try
            {
                exceptionCaught = false;
                collection.RemoveAt(0);
            }
            catch (InvalidOperationException)
            {
                exceptionCaught = true;
            }

            if (exceptionCaught)
            {
                throw new InvalidOperationException("No expected exception");
            }

            if (collection.Count != 0)
            {
                throw new InvalidOperationException("StylusPointCollection was not allowed to go to zero count when not connected to a stroke!");
            }


            //
            // test reformat
            //
            List<StylusPointPropertyInfo> infos = new List<StylusPointPropertyInfo>();
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.X));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.Y));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.NormalPressure, 10, 100, StylusPointPropertyUnit.Radians, 2.0f));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.YTiltOrientation));
            StylusPointDescription descriptionToReformatTo = new StylusPointDescription(infos);

            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.XTiltOrientation));
            StylusPointDescription description = new StylusPointDescription(infos);

            StylusPoint sp1 = new StylusPoint(10, 10, 1.0f, description, new int[] { 10, 10 });
            StylusPoint sp2 = new StylusPoint(7.5, 7.5, .75f, description, new int[] { 7, 7 });
            StylusPointCollection original = new StylusPointCollection(description);
            original.Add(sp1);
            original.Add(sp2);

            StylusPointCollection newCollection = original.Reformat(descriptionToReformatTo);

            if (newCollection[0].X != original[0].X ||
                newCollection[0].Y != original[0].X ||
                newCollection[0].PressureFactor != original[0].PressureFactor ||
                newCollection[1].X != original[1].X ||
                newCollection[1].Y != original[1].X ||
                newCollection[1].PressureFactor != original[1].PressureFactor ||
                newCollection[1].HasProperty(StylusPointProperties.XTiltOrientation) || 
                newCollection[1].Description.GetPropertyInfo(StylusPointProperties.NormalPressure).Minimum != 10 ||
                newCollection[1].Description.GetPropertyInfo(StylusPointProperties.NormalPressure).Maximum != 100 ||
                newCollection[1].Description.GetPropertyInfo(StylusPointProperties.NormalPressure).Unit != StylusPointPropertyUnit.Radians ||
                newCollection[1].Description.GetPropertyInfo(StylusPointProperties.NormalPressure).Resolution != 2.0f)
            {
                throw new InvalidOperationException("Unexpected StylusPointCollection.Reformat result");
            }

            //
            // last, test adding a collection to itself
            //
            newCollection.Add(newCollection);
            if (newCollection.Count != 4 ||
                newCollection[0].X != 10 ||
                newCollection[1].X != 7.5 ||
                newCollection[2].X != 10 ||
                newCollection[3].X != 7.5)
            {
                throw new InvalidOperationException("Error adding a StylusPointCollection to itself");
            }

            List<StylusPointPropertyInfo> infos2 = new List<StylusPointPropertyInfo>();
            infos2.Add(new StylusPointPropertyInfo(StylusPointProperties.X));
            infos2.Add(new StylusPointPropertyInfo(StylusPointProperties.Y));
            infos2.Add(new StylusPointPropertyInfo(StylusPointProperties.NormalPressure));


            StylusPointCollection newCollection2 = original.Reformat(new StylusPointDescription(infos2));
            if (newCollection2[0].X != original[0].X ||
                newCollection2[0].Y != original[0].X ||
                newCollection2[0].PressureFactor != original[0].PressureFactor ||
                newCollection2[1].X != original[1].X ||
                newCollection2[1].Y != original[1].X ||
                newCollection2[1].PressureFactor != original[1].PressureFactor ||
                newCollection2[1].HasProperty(StylusPointProperties.XTiltOrientation) ||
                newCollection2[1].HasProperty(StylusPointProperties.YTiltOrientation) ||
                newCollection2[1].Description.GetPropertyInfo(StylusPointProperties.NormalPressure).Minimum != 10 ||
                newCollection2[1].Description.GetPropertyInfo(StylusPointProperties.NormalPressure).Maximum != 100 ||
                newCollection2[1].Description.GetPropertyInfo(StylusPointProperties.NormalPressure).Unit != StylusPointPropertyUnit.Radians ||
                newCollection2[1].Description.GetPropertyInfo(StylusPointProperties.NormalPressure).Resolution != 2.0f)
            {
                throw new InvalidOperationException("Unexpected StylusPointCollection.Reformat result");
            }
        }

        public void TestStylusPointDescription()
        {
            //
            // first, test a simple spd
            //
            List<StylusPointPropertyInfo> infos = new List<StylusPointPropertyInfo>();
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.X));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.Y));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.NormalPressure));
            StylusPointDescription description = new StylusPointDescription(infos);

            if (description.PropertyCount != 3 ||
                !description.HasProperty(StylusPointProperties.X) ||
                description.GetPropertyInfo(StylusPointProperties.X).Id != StylusPointProperties.X.Id ||
                !description.HasProperty(StylusPointProperties.Y) ||
                description.GetPropertyInfo(StylusPointProperties.Y).Id != StylusPointProperties.Y.Id ||
                !description.HasProperty(StylusPointProperties.NormalPressure) ||
                description.GetPropertyInfo(StylusPointProperties.NormalPressure).Id != StylusPointProperties.NormalPressure.Id ||
                description.GetStylusPointProperties().Count != 3)
            {
                throw new InvalidOperationException("Unexpected StylusPointDescription values");
            }


            //
            // then one with a button
            //
            infos.Clear();
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.X));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.Y));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.NormalPressure));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.XTiltOrientation));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.BarrelButton));
            description = new StylusPointDescription(infos);

            if (description.PropertyCount != 5 ||
                !description.HasProperty(StylusPointProperties.X) ||
                description.GetPropertyInfo(StylusPointProperties.X).Id != StylusPointProperties.X.Id||
                !description.HasProperty(StylusPointProperties.Y) ||
                description.GetPropertyInfo(StylusPointProperties.Y).Id != StylusPointProperties.Y.Id ||
                !description.HasProperty(StylusPointProperties.NormalPressure) ||
                description.GetPropertyInfo(StylusPointProperties.NormalPressure).Id != StylusPointProperties.NormalPressure.Id ||
                !description.HasProperty(StylusPointProperties.XTiltOrientation) ||
                description.GetPropertyInfo(StylusPointProperties.XTiltOrientation).Id != StylusPointProperties.XTiltOrientation.Id ||
                !description.HasProperty(StylusPointProperties.BarrelButton) ||
                description.GetPropertyInfo(StylusPointProperties.BarrelButton).Id != StylusPointProperties.BarrelButton.Id ||

                description.GetStylusPointProperties().Count != 5)
            {
                throw new InvalidOperationException("Unexpected StylusPointDescription values");
            }


            //
            // test exceptions
            //
            infos.Clear();
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.X));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.Y));
            //infos.Add(new StylusPointPropertyInfo(StylusPointProperties.NormalPressure));
            bool exceptionCaught = false;
            try
            {
                description = new StylusPointDescription(infos);
            }
            catch (ArgumentException)
            {
                exceptionCaught = true;
            }

            if (!exceptionCaught)
            {
                throw new InvalidOperationException("No expected exception");
            }


            infos.Clear();
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.X));
            //infos.Add(new StylusPointPropertyInfo(StylusPointProperties.Y));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.NormalPressure));
            exceptionCaught = false;
            try
            {
                description = new StylusPointDescription(infos);
            }
            catch (ArgumentException)
            {
                exceptionCaught = true;
            }

            if (!exceptionCaught)
            {
                throw new InvalidOperationException("No expected exception");
            }

            infos.Clear();
            //infos.Add(new StylusPointPropertyInfo(StylusPointProperties.X));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.Y));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.NormalPressure));
            exceptionCaught = false;
            try
            {
                description = new StylusPointDescription(infos);
            }
            catch (ArgumentException)
            {
                exceptionCaught = true;
            }

            if (!exceptionCaught)
            {
                throw new InvalidOperationException("No expected exception");
            }

            //
            // test duplicates
            //
            infos.Clear();
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.X));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.Y));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.NormalPressure));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.X));
            exceptionCaught = false;
            try
            {
                description = new StylusPointDescription(infos);
            }
            catch (ArgumentException)
            {
                exceptionCaught = true;
            }

            if (!exceptionCaught)
            {
                throw new InvalidOperationException("No expected exception");
            }


            //
            // test buttons not coming last
            //
            infos.Clear();
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.X));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.Y));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.NormalPressure));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.BarrelButton));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.XTiltOrientation));
            exceptionCaught = false;
            try
            {
                description = new StylusPointDescription(infos);
            }
            catch (ArgumentException)
            {
                exceptionCaught = true;
            }

            if (!exceptionCaught)
            {
                throw new InvalidOperationException("No expected exception");
            }


            //
            // test too many buttons
            //
            infos.Clear();
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.X));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.Y));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.NormalPressure));
            for (int i = 0; i < 32; i++)
            {
                infos.Add(new StylusPointPropertyInfo(new StylusPointProperty(Guid.NewGuid(), true)));
            }
            exceptionCaught = false;
            try
            {
                description = new StylusPointDescription(infos);
            }
            catch (ArgumentException)
            {
                exceptionCaught = true;
            }

            if (!exceptionCaught)
            {
                throw new InvalidOperationException("No expected exception");
            }
        }
        public void TestCompatibility()
        {

            List<StylusPointPropertyInfo> infos = new List<StylusPointPropertyInfo>();
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.X));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.Y));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.NormalPressure));
            StylusPointDescription description = new StylusPointDescription(infos);



            //
            // then create a spd with different value metrics
            //
            infos.Clear();
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.X, 10, 100, StylusPointPropertyUnit.None, 1.0f));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.Y, 10, 100, StylusPointPropertyUnit.None, 1.0f));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.NormalPressure, 10, 100, StylusPointPropertyUnit.None, 1.0f));
            StylusPointDescription description2 = new StylusPointDescription(infos);

            if (!StylusPointDescription.AreCompatible(description, description2))
            {
                throw new InvalidOperationException("Bogus results from StylusPointDescription.AreCompatible");
            }


            //
            // now add another metric to the second, they should not be compatible
            //
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.XTiltOrientation));
            description2 = new StylusPointDescription(infos);
            if (StylusPointDescription.AreCompatible(description, description2))
            {
                throw new InvalidOperationException("Bogus results from StylusPointDescription.AreCompatible");
            }


            //
            // now change the first 
            //
            description = new StylusPointDescription(infos);
            if (!StylusPointDescription.AreCompatible(description, description2))
            {
                throw new InvalidOperationException("Bogus results from StylusPointDescription.AreCompatible");
            }

            //
            // test compat if a metric is off on a nonX / Y
            //
            infos.Clear();
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.X));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.Y));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.NormalPressure));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.XTiltOrientation, 10, 100, StylusPointPropertyUnit.None, 1.0f));
            description = new StylusPointDescription(infos);


            infos.Clear();
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.X));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.Y));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.NormalPressure));
            infos.Add(new StylusPointPropertyInfo(StylusPointProperties.XTiltOrientation, 11, 100, StylusPointPropertyUnit.None, 1.0f));
            description2 = new StylusPointDescription(infos);

            if (!StylusPointDescription.AreCompatible(description, description2))
            {
                throw new InvalidOperationException("Bogus results from StylusPointDescription.AreCompatible");
            }

        }

        public void TestStylusPointsWithStrokes()
        {
            StylusPointCollection spc = new StylusPointCollection();
            StylusPoint sp = new StylusPoint(10d, 10d);
            spc.Add(sp);

            Stroke stroke = new Stroke(spc);
            if (stroke.StylusPoints != spc)
            {
                throw new InvalidOperationException("StylusPointCollections's don't match!");
            }

            //test exceptions

            bool exceptionCaught = false;
            try
            {
                stroke.StylusPoints.Clear();
            }
            catch (InvalidOperationException)
            {
                exceptionCaught = true;
            }

            if (!exceptionCaught)
            {
                throw new InvalidOperationException("No expected exception");
            }
            if (stroke.StylusPoints.Count != 1)
            {
                throw new InvalidOperationException("Unexpected StylusPoints count");
            }


            exceptionCaught = false;
            try
            {
                stroke.StylusPoints.RemoveAt(0);
            }
            catch (InvalidOperationException)
            {
                exceptionCaught = true;
            }

            if (!exceptionCaught)
            {
                throw new InvalidOperationException("No expected exception");
            }
            if (stroke.StylusPoints.Count != 1)
            {
                throw new InvalidOperationException("Unexpected StylusPoints count");
            }

            exceptionCaught = false;
            try
            {
                stroke.StylusPoints.Remove(sp);
            }
            catch (InvalidOperationException)
            {
                exceptionCaught = true;
            }

            if (!exceptionCaught)
            {
                throw new InvalidOperationException("No expected exception");
            }
            if (stroke.StylusPoints.Count != 1)
            {
                throw new InvalidOperationException("Unexpected StylusPoints count");
            }


            exceptionCaught = false;
            try
            {
                stroke.StylusPoints = new StylusPointCollection();
            }
            catch (ArgumentException)
            {
                exceptionCaught = true;
            }

            if (!exceptionCaught)
            {
                throw new InvalidOperationException("No expected exception");
            }
            if (stroke.StylusPoints.Count != 1)
            {
                throw new InvalidOperationException("Unexpected StylusPoints count");
            }

        }

        public override void Run()
        {
            TestStylusPoint();
            TestStylusPointCollection();
            TestStylusPointDescription();
            TestStylusPointsWithStrokes();
            TestCompatibility();
            
            Success = true;
        }
	}
}
