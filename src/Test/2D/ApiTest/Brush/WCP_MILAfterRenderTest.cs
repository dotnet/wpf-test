// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  WCP_MILlogterRenderTest - An WCP_MILlogterRenderTest class, renders a test visual, verifies the result
//  and then runs an logterRender test that changes some values dynamically, and then verifies the second result as well.  
//

using System;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WCP_MILlogterRender
{
    #region Base Classes

    internal class WCP_MILlogterRenderTest : DrawingVisual
    {
        //------------------------------------------------------
        public WCP_MILlogterRenderTest(double left, double top, double width, double height)
            : base()
        {
            m_left = left;
            m_top = top;
            m_height = width;
            m_width = height;
            Update();
        }

        //------------------------------------------------------
        protected void Update()
        {
            using (DrawingContext dc = RenderOpen())
            {
                OnRender(dc);
            }
        }

        //------------------------------------------------------
        protected virtual void OnRender(DrawingContext ctx)
        {
        }

        //------------------------------------------------------
        public new VisualCollection Children
        {
            get { return this.Children; }
        }

        public String TestDescription
        {
            get { return testDescription; }
        }

        public virtual void logterRender()
        {
        }

        //------------------------------------------------------
        protected string testDescription;
        protected double m_left;
        protected double m_top;
        protected double m_height;
        protected double m_width;
    }

    internal class WCP_MILlogterRenderDVTest : DrawingVisual
    {
        //------------------------------------------------------
        public WCP_MILlogterRenderDVTest(double left, double top, double width, double height)
            : base()
        {
            m_left = left;
            m_top = top;
            m_height = width;
            m_width = height;
        }

        public String TestDescription
        {
            get { return testDescription; }
        }

        public virtual void DoRender()
        {
        }

        public virtual void logterRender()
        {
        }

        //------------------------------------------------------
        protected string testDescription;
        protected double m_left;
        protected double m_top;
        protected double m_height;
        protected double m_width;
    }
    #endregion Base Classes

    #region Test Classes
    internal class WCP_Regression_Bug2 : WCP_MILlogterRenderTest
    {
        //------------------------------------------------------
        public WCP_Regression_Bug2(double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            testDescription = "Regression_Bug2: Dynamically make a non-empty ImageBrush empty";
        }

        //------------------------------------------------------
        protected override void OnRender(DrawingContext DC)
        {
            brush = new ImageBrush(new BitmapImage(new Uri("tulip.jpg", UriKind.RelativeOrAbsolute)));
            DC.DrawRectangle(brush, null, new Rect(new Point(m_left, m_top), new Point(m_left + m_width, m_top + m_height)));
        }

        //------------------------------------------------------
        public override void logterRender()
        {
            brush.ImageSource = null;
        }

        //------------------------------------------------------
        public ImageBrush brush;
    }

    internal class WCP_Regression_Bug3 : WCP_MILlogterRenderTest
    {
        //------------------------------------------------------
        public WCP_Regression_Bug3(double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            testDescription = "Regression_Bug3: Dynamically add a second GradientStop at 0 to a LinearGradientBrush";
        }

        //------------------------------------------------------
        protected override void OnRender(DrawingContext DC)
        {
            // GradientStopCollection
            GradientStopCollection GSC = new GradientStopCollection();
            GSC.Add(new GradientStop(Colors.Red, 0.0));
            GSC.Add(new GradientStop(Colors.Blue, 1.0));

            brush = new LinearGradientBrush(GSC);
            brush.StartPoint = new Point(0.0, 0.5);
            brush.EndPoint = new Point(1.0, 0.5);

            DC.DrawRectangle(brush, null, new Rect(new Point(m_left, m_top), new Point(m_left + m_width, m_top + m_height)));
        }

        //------------------------------------------------------
        public override void logterRender()
        {
            GradientStopCollection newGSC = brush.GradientStops;
            newGSC.Add(new GradientStop(Colors.Yellow, 0.0));
        }

        //------------------------------------------------------
        public LinearGradientBrush brush;
    }
    #endregion Test Classes
}