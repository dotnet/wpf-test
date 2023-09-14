using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections;
using System.Windows.Markup;
using System.Windows.Media.Media3D;
using System.Collections.Specialized;

namespace WpfControlToolkit
{
    public class PieChartControl : ChartControl
    {
        public PieChartControl()
        {
            _paths = new List<Path>();
            _pathFigures = new List<PathFigure>();
            _pathGeometries = new List<PathGeometry>();
            _legendLabels = new List<ContentPresenter>();
            _swatches = new List<Rectangle>();
            _colors = new List<Brush>();
            _childrenAngles = new List<double>();
            _wedgeColor = Colors.Blue;
            _swatchHeight = 0;
            _legendHeaderLabel = new Label();
            _legendHeaderLabel.Content = "Legend";
            _legendHeaderLabel.FontSize = 16;
            _legendHeaderLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
            _legendHeaderLabel.VerticalContentAlignment = VerticalAlignment.Bottom;
            AddVisualChild(_legendHeaderLabel);
        }

        public static readonly DependencyProperty BackgroundProperty =
            Panel.BackgroundProperty.AddOwner(typeof(PieChartControl),
            new FrameworkPropertyMetadata(Panel.BackgroundProperty.DefaultMetadata.DefaultValue,
            FrameworkPropertyMetadataOptions.None));

        /// <summary>
        ///     Allows the user to get/set the brush that is used to
        ///     draw the background of the graph
        /// </summary>
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public static readonly DependencyProperty StartColorProperty = DependencyProperty.Register(
            "StartColor",
            typeof(Color),
            typeof(PieChartControl),
            new PropertyMetadata(Colors.White));

        /// <summary>
        ///     this brush defines the color of the first wedge. 
        ///     defaults to R:20  G:55  B:190
        /// </summary>
        public Color StartColor
        {
            get { return (Color)GetValue(StartColorProperty); }
            set { SetValue(StartColorProperty, value); }
        }

        public static readonly DependencyProperty EndColorProperty = DependencyProperty.Register(
            "EndColor",
            typeof(Color),
            typeof(PieChartControl),
            new PropertyMetadata(Colors.Black));

        /// <summary>
        ///     this brush defines the color of the last wedge.
        ///     defaults to R:100  G:125  B:220 if there are less than 6 wedges
        ///     defaults to R:180  G:200  B:255 if there are more than 5 wedges
        /// </summary>
        public Color EndColor
        {
            get { return (Color)GetValue(EndColorProperty); }
            set { SetValue(EndColorProperty, value); }
        }

        public BrushSelector SliceBrushSelector
        {
            get { return _sliceBrushSelector; }
            set { _sliceBrushSelector = value; }
        }

        public static readonly DependencyProperty LegendLabelTemplateProperty = DependencyProperty.Register(
            "LegendLabelTemplate",
            typeof(DataTemplate),
            typeof(PieChartControl));

        public DataTemplate LegendLabelTemplate
        {
            get { return (DataTemplate)GetValue(LegendLabelTemplateProperty); }
            set { SetValue(LegendLabelTemplateProperty, value); }
        }

        public static readonly DependencyProperty LegendLabelSizeProperty = DependencyProperty.Register(
            "LegendLabelSize",
            typeof(Size),
            typeof(PieChartControl),
            new FrameworkPropertyMetadata(new Size(100, 30),
            new PropertyChangedCallback(LegendLabelSizeChanged)));

        public Size LegendLabelSize
        {
            get { return (Size)GetValue(LegendLabelSizeProperty); }
            set { SetValue(LegendLabelSizeProperty, value); }
        }

        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
            "Radius",
            typeof(double),
            typeof(PieChartControl),
            new FrameworkPropertyMetadata((double)0,
            new PropertyChangedCallback(RadiusChanged)));

        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        /// <summary>
        /// Override from UIElement
        /// </summary>
        protected override void OnRender(DrawingContext dc)
        {
            // Draw background in rectangle inside border.
            if (Background != null)
            {
                dc.DrawRectangle(Background,
                                 null,
                                 new Rect(0, 0, RenderSize.Width, RenderSize.Height));
            }
        }

        /// <summary>
        /// Updates DesiredSize of the Canvas.  Called by parent UIElement.  This is the first pass of layout.
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size constraint)
        {
            Size size = new Size();
            double radiusHeight = 2 * (Radius + RADIUS_PADDING);
            double labelHeight = ((EffectiveItemsSource.Count + 2) * (LegendLabelSize.Height + LEGEND_LABEL_PADDING));
            double width = 0;

            if ((constraint.Height == Double.PositiveInfinity) || 
                (constraint.Height < radiusHeight && constraint.Height < labelHeight))
            {
                size.Height = Math.Max(labelHeight, radiusHeight);
            }
            else
            {
                size.Height = constraint.Height;
            }

            width = (2 * (Radius + RADIUS_PADDING)) + _swatchHeight + LegendLabelSize.Width + (3 * LEGEND_LABEL_PADDING);

            if ((constraint.Width == Double.PositiveInfinity) ||
                (constraint.Width < width))
            {
                size.Width = width;
            }
            else
            {
                size.Width = constraint.Width;
            }

            // Measure Paths
            for(int i = 0; i < _paths.Count; i++)
            {
                _paths[i].Measure(new Size(0, 0));
            }

            // Measure Legend Labels
            for(int i = 0; i < _legendLabels.Count; i++)
            {
                _legendLabels[i].Measure(LegendLabelSize);
            }

            // Measure Swatches
            for(int i = 0; i < _swatches.Count; i++)
            {
                _swatches[i].Measure(new Size(0, 0));
            }

            if (!_visualsValid)
            {
                CalculateAngles();
            }

            CalculateColors();

            if (!_visualsValid)
            {
                HandleChangesToItemsSource();
            }

            return size;
        }

        /// <summary>
        /// When ItemsSource is changed, on default, just InvalidateMeasure and allow the user to override
        /// this method if additional or different functionality is required
        /// </summary>
        protected override void OnCollectionInvalidated()
        {
            _visualsValid = false;
            InvalidateMeasure();
        }

        /// <summary>
        ///     Default control arrangement is to only arrange
        ///     the first visual child. No transforms will be applied.
        /// </summary>
        protected override Size ArrangeOverride(Size size)
        {
            Size result = new Size();
            double radius = 0;

            if (Radius == 0)
            {
                radius = Math.Min(size.Width / 2, size.Height / 2);
            }
            else
            {
                radius = Radius;
            }

            radius = radius - RADIUS_PADDING;

            double radiusHeight = 2 * (radius + RADIUS_PADDING);
            double labelHeight = ((EffectiveItemsSource.Count + 2) * (LegendLabelSize.Height + LEGEND_LABEL_PADDING)) + LEGEND_LABEL_PADDING;
            double width = (2 * (radius + RADIUS_PADDING)) + _swatchHeight + LegendLabelSize.Width + (3 * LEGEND_LABEL_PADDING) + LEGEND_LABEL_PADDING;

            if (size.Height < radiusHeight || size.Height < labelHeight)
            {
                result.Height = Math.Max(labelHeight, radiusHeight);
            }
            else
            {
                result.Height = size.Height;
            }

            result.Width = Math.Max(width, size.Width);

            double x = (result.Width - (LegendLabelSize.Width + (LEGEND_LABEL_PADDING * 3) + _swatchHeight)) / 2;
            double y = (Math.Min(result.Height, result.Width)) / 2;
            _centerPoint = new Point(x, y);

            double line1X = _centerPoint.X + radius;
            double line1Y = _centerPoint.Y;
            double angle = 0;

            // Arrange Paths
            if (EffectiveItemsSource.Count == 1)
            {
                EllipseGeometry e = new EllipseGeometry(_centerPoint, radius, radius);
                _paths[0].Data = e;
                _paths[0].Arrange(new Rect(0, 0, result.Width, result.Height));
            }
            else if ((EffectiveItemsSource.Count == 0) && (_paths != null && _paths.Count == 1))
            {
                EllipseGeometry e = (EllipseGeometry)(_paths[0].Data);
                e.Center = _centerPoint;
                e.RadiusX = radius;
                e.RadiusY = radius;
                _paths[0].Arrange(new Rect(0, 0, result.Width, result.Height));
            }
            else
            {
                for (int i = 0; i < EffectiveItemsSource.Count; i++)
                {
                    angle += _childrenAngles[i];
                    double arcX = _centerPoint.X + (radius * Math.Cos(angle));
                    double arcY = _centerPoint.Y - (radius * Math.Sin(angle));

                    _pathFigures[i].StartPoint = _centerPoint;
                    LineSegment line1 = new LineSegment(new Point(line1X, line1Y), true);
                    ArcSegment arc = null;
                    if (_childrenAngles[i] <= Math.PI)
                        arc = new ArcSegment(new Point(arcX, arcY), new Size(radius, radius), 0, false, SweepDirection.Counterclockwise, true);
                    else
                        arc = new ArcSegment(new Point(arcX, arcY), new Size(radius, radius), 0, true, SweepDirection.Counterclockwise, true);
                    LineSegment line2 = new LineSegment(_centerPoint, true);
                    _pathFigures[i].Segments.Clear();
                    _pathFigures[i].Segments.Clear();
                    _pathFigures[i].Segments.Clear();

                    _pathFigures[i].Segments.Add(line1);
                    _pathFigures[i].Segments.Add(arc);
                    _pathFigures[i].Segments.Add(line2);

                    Rect r = new Rect(0, 0, result.Width, result.Height);
                    _paths[i].Arrange(r);

                    line1X = arcX;
                    line1Y = arcY;
                }
            }

            // Arrange Legend Label Header
            double rectY = (result.Height - (EffectiveItemsSource.Count * (LegendLabelSize.Height + LEGEND_LABEL_PADDING) + (2 * LegendLabelSize.Height))) / 2;
            Rect legendHeaderRect = new Rect(result.Width - LegendLabelSize.Width, rectY, LegendLabelSize.Width, LegendLabelSize.Height * 2);
            _legendHeaderLabel.Arrange(legendHeaderRect);

            // Arrange Legend Labels
            ArrangeLegendLabelRects(result, EffectiveItemsSource);

            // Arrange Swatches
            ArrangeSwatchRects(result, EffectiveItemsSource);

            return result;
        }

        /// <summary>
        ///     Return the visual child at the specified index from the list of children
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override Visual GetVisualChild(int index)
        {
            if (index < _paths.Count)
            {
                return _paths[index];
            }
            index -= _paths.Count;

            if (index < _legendLabels.Count)
            {
                return _legendLabels[index];
            }
            index -= _legendLabels.Count;

            if (index < _swatches.Count)
            {
                return _swatches[index];
            }
            index -= _swatches.Count;

            if (index == 0)
            {
                return _legendHeaderLabel;
            }

            throw new ArgumentOutOfRangeException("index");
        }


        /// <summary>
        ///     Return the number of visual children
        /// </summary>
        protected override int VisualChildrenCount
        {
            get { return (_paths.Count + _legendLabels.Count + _swatches.Count + 1); }
        }

        protected void PathTargetUpdated(object sender, DataTransferEventArgs e)
        {
            _visualsValid = false;
            InvalidateMeasure();
        }

        private void HandleChangesToItemsSource()
        {
            if (EffectiveItemsSource.Count == 0)
            {
                _paths[0].Fill = Brushes.Transparent;
            
                for (int i = 0; i < _legendLabels.Count; i++)
                {
                    RemoveVisualChild(_legendLabels[i]);
                }
                for (int i = 0; i < _swatches.Count; i++)
                {
                    RemoveVisualChild(_swatches[i]);
                }
                _legendLabels.Clear();
                _swatches.Clear();
            }

            // Add/Remove items to/from visual tree
            int difference = EffectiveItemsSource.Count - _paths.Count;
            if (_paths.Count >= 1 && _paths[0].Fill == Brushes.Transparent && EffectiveItemsSource.Count == 1)
            {
                difference = 1;
                RemoveVisualChild(_paths[0]);
                _paths.Clear();
                _pathFigures.Clear();
                _pathGeometries.Clear();
                _visualsValid = false;
            }

            // If items have been added to the EffectiveItemsSource, update lists of bars and horizontal labels
            if (difference > 0)
            {
                for (int i = 0; i < Math.Abs(difference); i++)
                {
                    if (_paths.Count != 0 && _paths[0].Data is EllipseGeometry)
                    {
                        RemoveVisualChild(_paths[0]);
                        _paths.RemoveAt(0);
                        _pathFigures.RemoveAt(0);
                        _pathGeometries.RemoveAt(0);
                        Path newPath = CreatePath(EffectiveItemsSource[i]);
                        _paths.Add(newPath);
                        AddVisualChild(newPath);
                    }

                    Path path = CreatePath(EffectiveItemsSource[i]);
                    _paths.Add(path);
                    AddVisualChild(path);

                    ContentPresenter legendLabel = new ContentPresenter();
                    _legendLabels.Add(legendLabel);
                    AddVisualChild(legendLabel);

                    Rectangle swatch = new Rectangle();
                    _swatches.Add(swatch);
                    AddVisualChild(swatch);
                }
                ResetPathBindings();
            }

            else if (difference < 0) // If an item has been removed from EffectiveItemsSource
            {
                for (int i = 0; i < Math.Abs(difference); i++)
                {
                    if (EffectiveItemsSource.Count > 0)
                    {
                        RemoveVisualChild(_paths[i]);
                        _paths.RemoveAt(i);

                        _pathFigures.RemoveAt(i);
                        _pathGeometries.RemoveAt(i);

                        RemoveVisualChild(_legendLabels[i]);
                        _legendLabels.RemoveAt(i);

                        RemoveVisualChild(_swatches[i]);
                        _swatches.RemoveAt(i);
                    }
                }
            }

            // Reset Legend Label Bindings
            ResetLegendLabelBindings();

            // Reset Swatch and Path Fill Brushes
            ResetFillColors();

            _visualsValid = true;

        }

        private Path CreatePath(object o)
        {
            Path path = new Path();
            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure();

            pathGeometry.Figures.Add(pathFigure);
            _pathFigures.Add(pathFigure);
            _pathGeometries.Add(pathGeometry);
            
            path.Data = pathGeometry;
            path.Stroke = Brushes.Transparent;
            path.StrokeThickness = 2;

            return path;
        }

        private void ResetPathBindings()
        {
            for (int i = 0; i < EffectiveItemsSource.Count; i++)
            {
                _paths[i].TargetUpdated -= new EventHandler<DataTransferEventArgs>(PathTargetUpdated);
                Binding binding = new Binding(ValuePath);
                binding.Source = EffectiveItemsSource[i];
                binding.Mode = BindingMode.OneWay;
                binding.NotifyOnTargetUpdated = true;
                _paths[i].SetBinding(Path.TagProperty, binding);
                _paths[i].TargetUpdated += new EventHandler<DataTransferEventArgs>(PathTargetUpdated);
            }
        }

        private void ResetFillColors()
        {
            for (int i = 0; i < EffectiveItemsSource.Count; i++)
            {
                _paths[i].Fill = _colors[i];
                _paths[i].Stroke = Brushes.Black;
                _paths[i].StrokeThickness = 0.5;
                _swatches[i].Fill = _colors[i];
            }
        }

        /// <summary>
        ///     Create legen labels as ContentPresenters
        /// </summary>
        /// <param name="content"></param>
        private void ResetLegendLabelBindings()
        {
            for (int i = 0; i < EffectiveItemsSource.Count; i++)
            {
                Binding myBinding = new Binding(this.NamePath);
                myBinding.Mode = BindingMode.TwoWay;
                myBinding.Source = EffectiveItemsSource[i];
              
                if (LegendLabelTemplate == null)
                {
                    Label l = new Label();
                    l.VerticalContentAlignment = VerticalAlignment.Center;
                    l.HorizontalContentAlignment = HorizontalAlignment.Left;
                    l.SetBinding(Label.ContentProperty, myBinding);
                    _legendLabels[i].Content = l;
                }
                else
                {
                    _legendLabels[i].ContentTemplate = this.LegendLabelTemplate;
                    _legendLabels[i].SetBinding(ContentPresenter.ContentProperty, myBinding);
                }
            }
        }

        private static void LegendLabelSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PieChartControl pc = (PieChartControl)d;
            pc.InvalidateArrange();
        }

        private static void RadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PieChartControl pc = (PieChartControl)d;
            pc.InvalidateArrange();
        }

        private void CalculateAngles()
        {
            _childrenAngles.Clear();

            double angle = 0, total = 0, temp = 0;
            double[] values = new double[EffectiveItemsSource.Count];
            DoubleHolder dh = new DoubleHolder();

            for (int i = 0; i < EffectiveItemsSource.Count; i++)
            {
                Binding b = new Binding(ValuePath);
                b.Source = EffectiveItemsSource[i];
                BindingOperations.SetBinding(dh, DoubleHolder.DoubleValueProperty, b);
                temp = dh.DoubleValue;
                values[i] = temp;

                if (temp < 0)
                {
                    values[i] = 0; 
                }
                else
                {
                    total += temp;
                }
            }

            for (int i = 0; i < EffectiveItemsSource.Count; i++)
            {
                temp = values[i];

                angle = ((temp / total) * (2 * Math.PI));
                _childrenAngles.Add(angle);
            }
        }

        private void CalculateColors()
        {
            double dr = 0, dg = 0, db = 0, da = 0;
            Color startColor = StartColor, endColor = EndColor, color = StartColor;
            _colors.Clear();

            if (EffectiveItemsSource.Count == 1)
            {
                _colors.Add(new SolidColorBrush(startColor));
            }
            else
            {
                dr = (int)((endColor.R - startColor.R) / (EffectiveItemsSource.Count - 1));
                dg = (int)((endColor.G - startColor.G) / (EffectiveItemsSource.Count - 1));
                db = (int)((endColor.B - startColor.B) / (EffectiveItemsSource.Count - 1));
                da = (int)((endColor.A - startColor.A) / (EffectiveItemsSource.Count - 1));

                for (int i = 0; i < EffectiveItemsSource.Count; i++)
                {
                    if (i != 0)
                    {
                        color.R += (byte)dr;
                        color.G += (byte)dg;
                        color.B += (byte)db;
                        color.A += (byte)da;
                    }

                    if (_sliceBrushSelector != null)
                    {
                        _colors.Add(_sliceBrushSelector.SelectBrush(EffectiveItemsSource[i], _paths[i]));
                    }
                    else
                    {
                        _colors.Add(new SolidColorBrush(color));
                    }
                }
            }
        }


        /// <summary>
        ///     Create legend label Rects that will be passed into Arrange
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private void ArrangeLegendLabelRects(Size constraint, IList content)
        {
            // Create the Canvas for the Vertical Scale Labels
            double rectYPadding = (constraint.Height - (EffectiveItemsSource.Count * (LegendLabelSize.Height + LEGEND_LABEL_PADDING) + (2 * LegendLabelSize.Height))) / 2;
            double rectHeight = LegendLabelSize.Height;
            double rectWidth = LegendLabelSize.Width;
            _swatchHeight = rectHeight / 2;

            for (int i = 1; i <= content.Count; i++)
            {
                Rect r = new Rect(constraint.Width - (LegendLabelSize.Width + _swatchHeight + (3 * LEGEND_LABEL_PADDING)), rectYPadding + i * rectHeight + (2 * LegendLabelSize.Height) + ((i + 1) * LEGEND_LABEL_PADDING), rectWidth, rectHeight);
                _legendLabels[i-1].Arrange(r);
                
            }
        }

        /// <summary>
        ///     Create swatch Rects that will be passed into Arrange
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private void ArrangeSwatchRects(Size constraint, IList content)
        {
            double rectYPadding = (constraint.Height - (EffectiveItemsSource.Count * (LegendLabelSize.Height + LEGEND_LABEL_PADDING) + (2 * LegendLabelSize.Height))) / 2;

            for (int i = 1; i <= content.Count; i++)
            {
                Rect r = new Rect(constraint.Width - (_swatchHeight + LEGEND_LABEL_PADDING), rectYPadding + (i * _swatchHeight * 2) + (2 * LegendLabelSize.Height) + (i * LEGEND_LABEL_PADDING) + (_swatchHeight / 2), _swatchHeight, _swatchHeight);
                _swatches[i - 1].Arrange(r);
            }
        }


        private const double RADIUS_LENGTH = 0.9;
        private const double LEGEND_WIDTH = 0.2;
        private const double MIN_LEGEND_HEADER_HEIGHT = 20;
        private const double LEGEND_LABEL_PADDING = 2;
        private const double RADIUS_PADDING = 2;
        private IList<PathFigure> _pathFigures;
        private IList<PathGeometry> _pathGeometries;
        private IList<ContentPresenter> _legendLabels;
        private IList<Path> _paths;
        private IList<Rectangle> _swatches;
        private IList<Brush> _colors;
        private IList<double> _childrenAngles;
        private Label _legendHeaderLabel;
        private BrushSelector _sliceBrushSelector;
        private double _swatchHeight;
        private bool _visualsValid;
        private Point _centerPoint;
        private Color _wedgeColor;
    }
}
