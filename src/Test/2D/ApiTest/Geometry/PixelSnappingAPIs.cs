// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Program:  GMC API Tests - Testing PixelSnapping APIS
//  Author:   Microsoft
//
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class PixelSnappingAPIs : ApiTest
    {
        public PixelSnappingAPIs( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            StartTest();
        }

        private void StartTest()
        {
            SetGuidelines();
            GetGuidelines();
            CommonLib.LogResult();
        }

        private void SetGuidelines()
        {
            CommonLib.LogStatus("set Empty DoubleCollection");
            _visuals.Add(new TestVisual());
            ((ContainerVisual)_visuals[0]).XSnappingGuidelines = new DoubleCollection();
            ((ContainerVisual)_visuals[0]).YSnappingGuidelines = new DoubleCollection();

            CommonLib.LogStatus("Set one value DoubleColleciton");
            _visuals.Add(new TestVisual());
            ((ContainerVisual)_visuals[1]).XSnappingGuidelines = new DoubleCollection(new Double[] { 1 });

            CommonLib.LogStatus("Set bizzare values in DoubleCollection");
            _visuals.Add(new TestVisual());
            Double[] ds1 = new double[]{Double.Epsilon, Double.MaxValue, Double.MinValue, Double.NaN, 
                Double.NegativeInfinity, Double.PositiveInfinity};
            ((ContainerVisual)_visuals[2]).YSnappingGuidelines = new DoubleCollection(ds1);

            CommonLib.LogStatus("Set normal DoubleCollection");
            _visuals.Add(new TestVisual());
            Double[] ds2 = new double[] { 32, 2, 32, 12 };
            ((ContainerVisual)_visuals[3]).XSnappingGuidelines = new DoubleCollection(ds2);

            CommonLib.LogStatus("Let XSnap and YSnap share the same DoubleCollection");
            _visuals.Add(new TestVisual());
            DoubleCollection dc = new DoubleCollection(new Double[] { 2, 2 });
            ((ContainerVisual)_visuals[4]).XSnappingGuidelines = dc;
            ((ContainerVisual)_visuals[4]).XSnappingGuidelines = dc;
            ((ContainerVisual)_visuals[4]).YSnappingGuidelines = dc;
        }

        private void GetGuidelines()
        {
            CommonLib.LogStatus("Verify the DoubleCollections set on visual0");
            DoubleCollection rds0 = VisualTreeHelper.GetXSnappingGuidelines((ContainerVisual)_visuals[0]);
            DoubleCollection rds1 = VisualTreeHelper.GetYSnappingGuidelines((ContainerVisual)_visuals[0]);
            CommonLib.DoubleCollectionVerifier(new DoubleCollection(), rds0);
            CommonLib.DoubleCollectionVerifier(new DoubleCollection(), rds1);

            CommonLib.LogStatus("Verify the DoubleCollections set on visual1");
            DoubleCollection rds2 = VisualTreeHelper.GetXSnappingGuidelines((ContainerVisual)_visuals[1]);
            CommonLib.DoubleCollectionVerifier(new Double[] { 1.0 }, rds2);

            CommonLib.LogStatus("Verify the DoubleCollections set on visual2");
            DoubleCollection rds3 = VisualTreeHelper.GetYSnappingGuidelines((ContainerVisual)_visuals[2]);
            Double[] ds1 = new double[]{Double.Epsilon, Double.MaxValue, Double.MinValue, Double.NaN, 
                Double.NegativeInfinity, Double.PositiveInfinity};
            CommonLib.DoubleCollectionVerifier(ds1, rds3);

            CommonLib.LogStatus("Verify the DoubleCollections set on visual3");
            DoubleCollection rds4 = VisualTreeHelper.GetXSnappingGuidelines((ContainerVisual)_visuals[3]);
            CommonLib.DoubleCollectionVerifier(new double[] { 32.0, 2.0, 32.0, 12.0 }, rds4);

            CommonLib.LogStatus("Verify the DoubleCollections set on visual4");
            DoubleCollection rds5 = VisualTreeHelper.GetXSnappingGuidelines((ContainerVisual)_visuals[4]);
            DoubleCollection rds6 = VisualTreeHelper.GetYSnappingGuidelines((ContainerVisual)_visuals[4]);
            CommonLib.DoubleCollectionVerifier(new Double[] { 2.0, 2.0 }, rds5);
            CommonLib.DoubleCollectionVerifier(new Double[] { 2.0, 2.0 }, rds6);
        }

        private ArrayList _visuals = new ArrayList();
    }

    internal class TestVisual : DrawingVisual
    {
        public TestVisual()
            : base()
        {
            Update();
        }

        protected void Update()
        {
            using (DrawingContext dc = RenderOpen())
            {
                OnRender(dc);
            }
        }

        protected void OnRender(DrawingContext ctx)
        {
            RectangleGeometry RG = new RectangleGeometry(new Rect(new Point(0, 0), new Size(1400, 1400)));
            ctx.DrawGeometry(Brushes.White, null, RG);
        }
    }
}