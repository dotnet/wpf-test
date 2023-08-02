using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace WpfControlToolkit
{
    public class ScatterPlotPanel : Panel
    {
        protected override void OnInitialized(System.EventArgs e)
        {
            base.OnInitialized(e);
            _childrenPosition = new ObservableCollection<Point>();
        }
       
        private static void OnValuesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ScatterPlotPanel v = sender as ScatterPlotPanel;
            if (v == null)
                return;
            ItemCollection oldItems = args.OldValue as ItemCollection;
            ItemCollection newItems = args.NewValue as ItemCollection;
            if (oldItems != null)
                ((INotifyCollectionChanged)oldItems).CollectionChanged -= new NotifyCollectionChangedEventHandler(v.ScatterPlotPanel_CollectionChanged);

            if (args.Property == ScatterPlotPanel.XValuesProperty)
            {
                if (GetXValues(v) != null)
                    GetXValues(v).CollectionChanged += new NotifyCollectionChangedEventHandler(v.ScatterPlotPanel_CollectionChanged);
            }
            else if (args.Property == ScatterPlotPanel.YValuesProperty)
            {
                if (GetYValues(v) != null)
                    GetYValues(v).CollectionChanged += new NotifyCollectionChangedEventHandler(v.ScatterPlotPanel_CollectionChanged);
            }
            v.InvalidateArrange();
            v.InvalidateVisual();
        }

        private void ScatterPlotPanel_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            InvalidateArrange();
            InvalidateVisual();
        }
        
        protected override Size MeasureOverride(Size availableSize)
        {
            for (int i = 0; i < InternalChildren.Count; i++)
            {
                InternalChildren[i].Measure(availableSize);
            }
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            int count = Math.Min(XValues.Count, YValues.Count);
            _childrenPosition.Clear();
            for (int i = 0; i < count; i++)
            {
                Rect r = new Rect(XValues[i] - InternalChildren[i].DesiredSize.Width / 2,
                    YValues[i] - InternalChildren[i].DesiredSize.Height / 2
                    , InternalChildren[i].DesiredSize.Width, InternalChildren[i].DesiredSize.Height);
                InternalChildren[i].Arrange(r);
                _childrenPosition.Add(new Point(XValues[i], YValues[i]));
            }
            return finalSize;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            if (_childrenPosition.Count == 0)
                return;
            if (PlottingPen != null)
            {
                if (!IsSmoothOutline)
                {
                    for (int i = 0; i < InternalChildren.Count - 1; i++)
                    {
                        Point startPoint = GeometryOperation.ComputeIntersectionPoint((FrameworkElement)InternalChildren[i], (FrameworkElement)InternalChildren[i + 1]);
                        Point endPoint = GeometryOperation.ComputeIntersectionPoint((FrameworkElement)InternalChildren[i + 1], (FrameworkElement)InternalChildren[i]);
                        dc.DrawLine(PlottingPen, startPoint, endPoint);
                    }
                }
                else
                {
                    dc.DrawGeometry(null, PlottingPen, CreateAreaCurveGeometry());
                }
            }
        }

        private PathGeometry CreateAreaCurveGeometry()
        {
            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            pathGeometry.Figures.Add(pathFigure);

            Point[] catmullRomPoints = new Point[_childrenPosition.Count];
            _childrenPosition.CopyTo(catmullRomPoints, 0);
            Point[] bezierPoints = GeometryOperation.CatmullRom(catmullRomPoints);
            pathFigure.StartPoint = bezierPoints[0];
            PolyBezierSegment pbs = new PolyBezierSegment();
            for (int i = 1; i < bezierPoints.GetLength(0); i++)
                pbs.Points.Add(bezierPoints[i]);
            pathFigure.Segments.Add(pbs);
            return pathGeometry;
        }


        public Pen PlottingPen
        {
            get { return (Pen)GetValue(PlottingPenProperty); }
            set { SetValue(PlottingPenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlottingPen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlottingPenProperty =
            DependencyProperty.Register("PlottingPen", typeof(Pen), typeof(ScatterPlotPanel), new UIPropertyMetadata(null));


        public bool IsSmoothOutline
        {
            get { return (bool)GetValue(IsSmoothOutlineProperty); }
            set { SetValue(IsSmoothOutlineProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSmoothOutline.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSmoothOutlineProperty =
            DependencyProperty.Register("IsSmoothOutline", typeof(bool), typeof(ScatterPlotPanel), new UIPropertyMetadata(null));


        private ObservableCollection<double> XValues
        {
            get { return (ObservableCollection<double>)GetXValues(this); }
            set { SetXValues(this, value); }
        }

        public static ObservableCollection<double> GetXValues(DependencyObject obj)
        {
            return (ObservableCollection<double>)obj.GetValue(XValuesProperty);
        }

        public static void SetXValues(DependencyObject obj, ObservableCollection<double> value)
        {
            obj.SetValue(XValuesProperty, value);
        }

        // Using a DependencyProperty as the backing store for XValues.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XValuesProperty =
            DependencyProperty.RegisterAttached("XValues", typeof(ObservableCollection<double>), typeof(ScatterPlotPanel), new UIPropertyMetadata(null, new PropertyChangedCallback(OnValuesChanged)));


        private ObservableCollection<double> YValues
        {
            get { return (ObservableCollection<double>)GetYValues(this); }
            set { SetYValues(this, value); }
        }

        public static ObservableCollection<double> GetYValues(DependencyObject obj)
        {
            return (ObservableCollection<double>)obj.GetValue(YValuesProperty);
        }

        public static void SetYValues(DependencyObject obj, ObservableCollection<double> value)
        {
            obj.SetValue(YValuesProperty, value);
        }

        // Using a DependencyProperty as the backing store for YValues.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YValuesProperty =
            DependencyProperty.RegisterAttached("YValues", typeof(ObservableCollection<double>), typeof(ScatterPlotPanel), new UIPropertyMetadata(null, new PropertyChangedCallback(OnValuesChanged)));

        private ObservableCollection<Point> _childrenPosition;
    }
}
