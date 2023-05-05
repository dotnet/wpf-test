// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MS.Internal;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Diagnostics;
using System.Reflection;
using System.Globalization;
using System.Threading;
using System.Windows.Media;
using System;
using System.Collections;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// Summary description for PiePanel.
    /// </summary>
    public class PiePanel : Canvas
    {


        public PiePanel() : base()
        {

        }


        #region static mathods for attached properties

        public static void SetValue(DependencyObject d, double point)
        {
            if (d == null) { throw new ArgumentNullException("d"); }
            d.SetValue(ValueProperty, point);
        }

        public static double GetValue(DependencyObject d)
        {
            if (d == null) { throw new ArgumentNullException("d"); }
            return (double)d.GetValue(ValueProperty);
        }

        public static void SetArc1(DependencyObject d, double point)
        {
            if (d == null) { throw new ArgumentNullException("d"); }
            d.SetValue(Arc1Property, point);
        }

        public static double GetArc1(DependencyObject d)
        {
            if (d == null) { throw new ArgumentNullException("d"); }
            return (double)d.GetValue(Arc1Property);
        }

        public static void SetArc2(DependencyObject d, double point)
        {
            if (d == null) { throw new ArgumentNullException("d"); }
            d.SetValue(Arc2Property, point);
        }

        public static double GetArc2(DependencyObject d)
        {
            if (d == null) { throw new ArgumentNullException("d"); }
            return (double)d.GetValue(Arc2Property);
        }

        public static void SetWedgeName(DependencyObject d, string name)
        {
            if (d == null) { throw new ArgumentNullException("d"); }
            d.SetValue(WedgeNameProperty, name);
        }

        public static string GetWedgeName(DependencyObject d)
        {
            if (d == null) { throw new ArgumentNullException("d"); }
            return (string)d.GetValue(WedgeNameProperty);
        }



#endregion

        #region private static Enum
        static Brush[] s_wedgeColors = new Brush[] {
            Brushes.Red,
            Brushes.Green,
            Brushes.Yellow,
            Brushes.Blue,
            Brushes.Violet,
            Brushes.White,
            Brushes.SteelBlue,
            Brushes.Silver,
            Brushes.Salmon,
        };

        #endregion

        #region Public Properties

        public static readonly DependencyProperty ValueProperty
            = DependencyProperty.RegisterAttached("Value", typeof(double), typeof(PiePanel),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        public static readonly DependencyProperty Arc1Property
            = DependencyProperty.RegisterAttached("Arc1", typeof(double), typeof(PiePanel),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty Arc2Property
            = DependencyProperty.RegisterAttached("Arc2", typeof(double), typeof(PiePanel),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty WedgeNameProperty
            = DependencyProperty.RegisterAttached("WedgeName", typeof(string), typeof(PiePanel),
                new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender));


        #endregion

        #region Protected Methods

        protected override Size ArrangeOverride(Size arrangeSize)
        {

            double total = 0;
            foreach (UIElement child in InternalChildren)
                total += GetValue(child);

            double prevArc = 0;
            foreach (UIElement child in InternalChildren)
            {
                for (int i = 0; i < InternalChildren.Count; i++)
                {
                    if (total > 0)
                    {
                        SetArc1(InternalChildren[i], prevArc);
                        double arc2 = (GetValue(InternalChildren[i]) / total * 2d * Math.PI);
                        SetArc2(InternalChildren[i], arc2);
                        prevArc += arc2;
                    }
                    int _colorChoice;
                    Math.DivRem(i, 8, out _colorChoice);
                    InternalChildren[i].SetValue(Control.BackgroundProperty, s_wedgeColors[_colorChoice]);
                    InternalChildren[i].Arrange(new Rect(0, 0, arrangeSize.Width, arrangeSize.Height));
                }

            }
            return arrangeSize;
        }
        #endregion
    }


    public class PieChart : System.Windows.Controls.Primitives.Selector
    {

        static  PieChart()
        {
            ItemsPanelProperty.OverrideMetadata(typeof(PieChart), new FrameworkPropertyMetadata(new ItemsPanelTemplate(new FrameworkElementFactory(typeof(PiePanel)))));
            ItemTemplateProperty.OverrideMetadata(typeof(PieChart), new FrameworkPropertyMetadata(new DataTemplate()));

        }
        protected override void OnItemTemplateChanged(DataTemplate oldItemTemplate, DataTemplate newItemTemplate)
        {
            throw new InvalidOperationException("PieChart set's it own ItemTemplate! ");
        }

        #region Public Properties

        public static readonly DependencyProperty TitleProperty
    = DependencyProperty.RegisterAttached("Title", typeof(string), typeof(PieChart),
        new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion
        #region static mathods for attached properties
        public static void SetTitle(DependencyObject d, string name)
        {
            if (d == null) { throw new ArgumentNullException("d"); }
            d.SetValue(TitleProperty, name);
        }

        public static string GetTitle(DependencyObject d)
        {
            if (d == null) { throw new ArgumentNullException("d"); }
            return (string)d.GetValue(TitleProperty);
        }

        #endregion
        #region Methods
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is Wedge);
        }
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new Wedge();
        }
        private int ElementIndex(Wedge listItem)
        {
            return ItemContainerGenerator.IndexFromContainer(listItem);
        }
        private Wedge ElementAt(int index)
        {
            return ItemContainerGenerator.ContainerFromIndex(index) as Wedge;
        }
        #endregion
    }

    public class Wedge : ContentControl
    {
        public Wedge() : base()
        {
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            int count = VisualTreeHelper.GetChildrenCount(this);
            UIElement ue = (UIElement)VisualTreeHelper.GetChild(this,0);

            for(int i = 0; i < count; i++)
            {
                double wedgeheight = ue.RenderSize.Height;
                double wedgewidth = ue.RenderSize.Width;
                double radiusx = arrangeBounds.Width / 2;
                double radiusy = arrangeBounds.Height / 2;
                Point center = new Point(radiusx, radiusy);
                double arc1 = PiePanel.GetArc1(this);
                double arc2 = PiePanel.GetArc2(this);
                Point pnt1 = new Point(Math.Cos(arc1 + (arc2 / 2)) * (radiusx / 2), Math.Sin(arc1 + (arc2 / 2)) * (radiusy / 2));
                pnt1.Offset(center.X - (wedgeheight / 2), center.Y - (wedgewidth / 2));

                Rect r = new Rect();
                r.X = pnt1.X ;
                r.Y = pnt1.Y ;
                r.Height = wedgeheight;
                r.Width = wedgewidth;
                ue.Arrange(r);
            }

            return arrangeBounds;
        }
        protected override void OnRender(DrawingContext drawingContext)
        {

            string _title = PieChart.GetTitle(this);
            if (_title.Length>0)
                _title="foo";
            double radiusx = ActualWidth / 2;
            double radiusy = ActualHeight/2;
            double arc1 = PiePanel.GetArc1(this);
            double arc2 = PiePanel.GetArc2(this);

            Point center = new Point(radiusx, radiusy);
            Point pnt1 = new Point(Math.Cos(arc1) * radiusx, Math.Sin(arc1) * radiusy);
            Point pnt2 = new Point(Math.Cos(arc1 + arc2) * radiusx, Math.Sin(arc1 + arc2) * radiusy);
            pnt1.Offset(center.X, center.Y);
            pnt2.Offset(center.X, center.Y);

            PathFigure fig = new PathFigure();
            fig.StartPoint = center;
            fig.Segments.Add(new LineSegment(pnt1, true));
            if(arc2>0)
                fig.Segments.Add(new ArcSegment(pnt2, new Size(radiusx, radiusy), 0, arc2 > Math.PI, SweepDirection.Clockwise, true));
            else
                fig.Segments.Add(new ArcSegment(pnt2, new Size(radiusx, radiusy), 0, arc2 > Math.PI, SweepDirection.Counterclockwise, true));


            fig.IsClosed = true;


            fig.IsFilled = true;

            PathGeometry pie = new PathGeometry();
            pie.Figures.Add(fig);
            pie.FillRule = FillRule.Nonzero;

            drawingContext.DrawGeometry(Background, new Pen(Brushes.Black, 2), pie);

            FormattedText ft = new FormattedText(PiePanel.GetWedgeName(this), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 12.22, Brushes.Black);
            Point pnt3 = new Point(Math.Cos(arc1 + (arc2 / 2)) * (radiusx / .9), Math.Sin(arc1 + (arc2 / 2)) * (radiusy / .9));
            pnt3.Offset(center.X - 15, center.Y - 5 );
            drawingContext.DrawText(ft, pnt3);

        }
    }


}

