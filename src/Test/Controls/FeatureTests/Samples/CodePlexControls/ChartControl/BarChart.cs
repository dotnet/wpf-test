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
using System.Globalization;
using System.Collections.Specialized;
using System.ComponentModel;

namespace WpfControlToolkit
{
    public class BarChartControl : ChartControl
    {
        public BarChartControl()
        {
            _startsAtZero = false;
            _barPadding = BAR_PADDING;
            _horizontalLabelPadding = HORIZ_LABEL_BUFFER;
            _verticalLabels = new List<ContentPresenter>();
            _horizontalLabels = new List<ContentPresenter>();
            _bars = new List<ContentPresenter>();
            _referenceLines = new List<Rectangle>();
            _tickMarks = new List<Rectangle>();
            _doubleHolders = new List<DoubleHolder>();
            LayoutUpdated += new EventHandler(BarChartControl_LayoutUpdated);
        }

        public static readonly DependencyProperty BarTemplateProperty = DependencyProperty.Register(
            "BarTemplate",
            typeof(DataTemplate),
            typeof(BarChartControl));

        /// <summary>
        /// Allows the user to get/set the brush that is used to fill the 
        /// bars of the char
        /// </summary>
        public DataTemplate BarTemplate
        {
            get { return (DataTemplate)GetValue(BarTemplateProperty); }
            set { SetValue(BarTemplateProperty, value); }
        }

        public static readonly DependencyProperty BarTemplateSelectorProperty = DependencyProperty.Register(
            "BarTemplateSelector",
            typeof(DataTemplateSelector),
            typeof(BarChartControl),
            new PropertyMetadata(getDefaultDataTemplateSelector()));

        /// <summary>
        /// Allows the user to provide a DataTemplateSelector
        /// </summary>
        public DataTemplateSelector BarTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(BarTemplateSelectorProperty); }
            set { SetValue(BarTemplateSelectorProperty, value); }
        }

        public static readonly DependencyProperty VerticalAxisLabelTemplateProperty = DependencyProperty.Register(
            "VerticalAxisLabelTemplate",
            typeof(DataTemplate),
            typeof(BarChartControl));

        /// <summary>
        /// Allows the user to get/set the DataTemplate of the scale
        /// labels
        /// </summary>
        public DataTemplate VerticalAxisLabelTemplate
        {
            get { return (DataTemplate)GetValue(VerticalAxisLabelTemplateProperty); }
            set { SetValue(VerticalAxisLabelTemplateProperty, value); }
        }

        public static readonly DependencyProperty HorizontalAxisLabelTemplateProperty = DependencyProperty.Register(
            "HorizontalAxisLabelTemplate",
            typeof(DataTemplate),
            typeof(BarChartControl));

        /// <summary>
        /// Allows the user to get/set the DataTemplate of the group
        /// labels
        /// </summary>
        public DataTemplate HorizontalAxisLabelTemplate
        {
            get { return (DataTemplate)GetValue(HorizontalAxisLabelTemplateProperty); }
            set { SetValue(HorizontalAxisLabelTemplateProperty, value); }
        }

        public static readonly DependencyProperty VerticalAxisLabelConverterProperty = DependencyProperty.Register(
            "VerticalAxisLabelConverter",
            typeof(IValueConverter),
            typeof(BarChartControl));

        /// <summary>
        /// This converter is used to convert from the raw value from the data to a presentable value
        /// for example, the converter can return only the 3 most significant digits. 
        /// </summary>
        public IValueConverter VerticalAxisLabelConverter
        {
            get { return (IValueConverter)GetValue(VerticalAxisLabelConverterProperty); }
            set { SetValue(VerticalAxisLabelConverterProperty, value); }
        }

        public static readonly DependencyProperty HorizontalAxisPenProperty = DependencyProperty.Register(
            "HorizontalAxisPen",
            typeof(Pen),
            typeof(BarChartControl),
            new PropertyMetadata(new Pen(Brushes.Black, BRUSH_THICKNESS)));

        public static readonly DependencyProperty VerticalLabelSizeProperty = DependencyProperty.Register(
            "VerticalLabelSize",
            typeof(Size),
            typeof(BarChartControl),
            new FrameworkPropertyMetadata(new Size(100, 20),
            FrameworkPropertyMetadataOptions.AffectsArrange));


        public Size VerticalLabelSize
        {
            get { return (Size)GetValue(VerticalLabelSizeProperty); }
            set { SetValue(VerticalLabelSizeProperty, value); }
        }


        public static readonly DependencyProperty HorizontalLabelSizeProperty = DependencyProperty.Register(
            "HorizontalLabelSize",
            typeof(Size),
            typeof(BarChartControl),
            new FrameworkPropertyMetadata(new Size(100, 30), 
            FrameworkPropertyMetadataOptions.AffectsArrange));

        public Size HorizontalLabelSize
        {
            get { return (Size)GetValue(HorizontalLabelSizeProperty); }
            set { SetValue(HorizontalLabelSizeProperty, value); }
        }

        /// <summary>
        /// Allows the user to get/set the Pen that is used to draw the 
        /// base axis lines defaults to black brush
        /// </summary>
        public Pen HorizontalAxisPen
        {
            get { return (Pen)GetValue(HorizontalAxisPenProperty); }
            set { SetValue(HorizontalAxisPenProperty, value); }
        }

        public static readonly DependencyProperty VerticalAxisPenProperty = DependencyProperty.Register(
            "VerticalAxisPen",
            typeof(Pen),
            typeof(BarChartControl),
            new PropertyMetadata(new Pen(Brushes.Black, BRUSH_THICKNESS)));

        /// <summary>
        /// Allows the user to get/set the Pen that is used to draw the 
        /// scale axis lines defaults to black brush
        /// </summary>
        public Pen VerticalAxisPen
        {
            get { return (Pen)GetValue(VerticalAxisPenProperty); }
            set { SetValue(VerticalAxisPenProperty, value); }
        }

        public static readonly DependencyProperty ReferenceLinePenProperty = DependencyProperty.Register(
            "ReferenceLinePen",
            typeof(Pen),
            typeof(BarChartControl),
            new PropertyMetadata(new Pen(Brushes.Gray, BRUSH_THICKNESS)));

        /// <summary>
        /// Allows the user to get/set the Pen that is used to draw the 
        /// reference lines defaults to black brush with 50% opacity
        /// </summary>
        public Pen ReferenceLinePen
        {
            get { return (Pen)GetValue(ReferenceLinePenProperty); }
            set { SetValue(ReferenceLinePenProperty, value); }
        }

        public static readonly DependencyProperty BackgroundProperty =
            Panel.BackgroundProperty.AddOwner(typeof(BarChartControl),
            new FrameworkPropertyMetadata(Panel.BackgroundProperty.DefaultMetadata.DefaultValue,
            FrameworkPropertyMetadataOptions.None));


        /// <summary>
        /// Allows the user to get/set the brush that is used to
        /// draw the background of the graph
        /// </summary>
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
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

            Point[] axisLinePoints = CreateAxisLinePoints(RenderSize);
            dc.DrawLine(VerticalAxisPen, axisLinePoints[0], axisLinePoints[1]);
            dc.DrawLine(HorizontalAxisPen, axisLinePoints[2], axisLinePoints[3]);
        }


        /// <summary>
        /// Called by parent UIElement.  This is the first pass of layout.
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size constraint)
        {
            Size result = new Size();

            if (constraint.Height == Double.PositiveInfinity)
            {
                result.Height = (MIN_REF_LINES * VerticalLabelSize.Height) + HorizontalLabelSize.Height;
            }
            else
            {
                if (constraint.Height < ((MIN_REF_LINES * VerticalLabelSize.Height) + HorizontalLabelSize.Height))
                {
                    result.Height = (MIN_REF_LINES * VerticalLabelSize.Height) + HorizontalLabelSize.Height;
                }
                else
                {
                    result.Height = constraint.Height;
                }
            }

            if (constraint.Width == Double.PositiveInfinity)
            {
                result.Width = ((MIN_BAR_WIDTH + BAR_PADDING) * EffectiveItemsSource.Count + BAR_PADDING) + VerticalLabelSize.Width + BRUSH_THICKNESS;
            }
            else
            {
                if (constraint.Width < (EffectiveItemsSource.Count * (HorizontalLabelSize.Width) + VerticalLabelSize.Width + BRUSH_THICKNESS))
                {
                    result.Width = EffectiveItemsSource.Count * (HorizontalLabelSize.Width) + VerticalLabelSize.Width + BRUSH_THICKNESS;
                }
                else
                {
                    result.Width = constraint.Width;
                }
            }

            if (!_visualsValid)
            {
                HandleChangesToItemsSource();
            }

            // Measure Bars
            for (int i = 0; i < _bars.Count; i++)
            {
                _bars[i].Measure(new Size(0, 0));
            }

            // Measure Tick Marks
            for (int i = 0; i < _tickMarks.Count; i++)
            {
                _tickMarks[i].Measure(new Size(0, 0));
            }

            // Measure Reference Lines
            for (int i = 0; i < _referenceLines.Count; i++)
            {
                _referenceLines[i].Measure(new Size(0, 0));
            }

            // Measure Vertical Labels
            for (int i = 0; i < _verticalLabels.Count; i++)
            {
                _verticalLabels[i].Measure(new Size(0, 0));
            }

            // Measure Horizontal Labels
            for (int i = 0; i < _horizontalLabels.Count; i++)
            {
                _horizontalLabels[i].Measure(new Size(0, 0));
            }

            return result;
        }

        /// <summary>
        /// Create all Rects for children and Arrage children
        /// </summary>
        protected override Size ArrangeOverride(Size size)
        {
            Size s = new Size();

            int refLines = Math.Max(_numReferenceLines, MIN_REF_LINES);

            if (size.Height <= ((refLines + 1) * VerticalLabelSize.Height) + HorizontalLabelSize.Height)
            {
                s.Height = (refLines + 1) * VerticalLabelSize.Height + HorizontalLabelSize.Height;
            }
            else
            {
                s.Height = size.Height;
            }
            if (size.Width < (EffectiveItemsSource.Count * (HorizontalLabelSize.Width) + VerticalLabelSize.Width))
            {
                s.Width = EffectiveItemsSource.Count * (HorizontalLabelSize.Width) + VerticalLabelSize.Width;
            }
            else
            {
                s.Width = size.Width;
            }

            // Recalculate the value increment and recreate vertical labels, and add 
            //  or remove the correct number of tick marks and reference lines
            int previousNumRefLines = _numReferenceLines;
            _valueIncrement = CalculateValueIncrement(s, EffectiveItemsSource);
            _pixelIncrement = CalculatePixelIncrements(s);

            if (previousNumRefLines != _numReferenceLines)
            {
                _referenceLinesValid = false;
            }

            // Calculate the position of the vertical Axis
            CalculateHorizontalAxisCoord(s);

            // Arrange Vertical Labels
            ArrangeVerticalLabels(s);

            // Arrange Horizontal Labels
            ArrangeHorizontalLabels(s);

            // Create Bar rects
            ArrangeBars(s);

            // Create reference line rects
            ArrangeReferenceLines(s);

            // Create tick mark rects
            ArrangeTickMarks(s);

            return s;
        }

        protected void BarChartControl_LayoutUpdated(object sender, EventArgs e)
        {
            if (_layoutCount == 3)
            {
                _layoutCount = 0;
                _referenceLinesValid = true;
            }
            // Update reference lines, tick marks, and vertical labels in the visual tree
            if (!_referenceLinesValid && _referenceLines.Count != _numReferenceLines)
            {
                int difference = _referenceLines.Count - _numReferenceLines;

                // If it was found in Arrange that the number of reference lines is invalid, then add or remove 
                //  from the visual tree as necessary
                if (difference > 0)
                {
                    for (int i = 0; i < difference; i++)
                    {
                        RemoveVisualChild(_referenceLines[0]);
                        _referenceLines.RemoveAt(0);
                        RemoveVisualChild(_tickMarks[0]);
                        _tickMarks.RemoveAt(0);
                        RemoveVisualChild(_verticalLabels[0]);
                        _verticalLabels.RemoveAt(0);
                    }
                }
                else if (difference < 0)
                {
                    for (int i = 0; i < Math.Abs(difference); i++)
                    {
                        Rectangle refLine = CreateReferenceLine();
                        _referenceLines.Add(refLine);
                        AddVisualChild(refLine);

                        Rectangle tickMark = CreateTickMark();
                        _tickMarks.Add(tickMark);
                        AddVisualChild(tickMark);

                        ContentPresenter vLabel = CreateVerticalLabel();
                        _verticalLabels.Add(vLabel);
                        AddVisualChild(vLabel);
                    }
                }
                _referenceLinesValid = true;
                _layoutCount++;
                InvalidateVisual();
            }
        }

        protected void BarTargetUpdated(object sender, DataTransferEventArgs e)
        {   
            InvalidateVisual();
        }

        /// <summary>
        /// Return visual child from list of children
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override Visual GetVisualChild(int index)
        {
            if ((index > VisualChildrenCount - 1) || index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if (index < _referenceLines.Count)
            {
                return _referenceLines[index];
            }
            index -= _referenceLines.Count;

            if (index < _tickMarks.Count)
            {
                return _tickMarks[index];
            }
            index -= _tickMarks.Count;

            if (index < _bars.Count)
            {
                return _bars[index];
            }
            index -= _bars.Count;

            if (index < _verticalLabels.Count)
            {
                return _verticalLabels[index];
            }
            else
            {
                index -= _verticalLabels.Count;
                return _horizontalLabels[index];
            }
        }

        /// <summary>
        /// Return the number of visual children
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return _referenceLines.Count +
                       _verticalLabels.Count +
                       _horizontalLabels.Count +
                       _tickMarks.Count +
                       _bars.Count;
            }
        }

        protected override void OnCollectionInvalidated()
        {
            _visualsValid = false;
            InvalidateMeasure();
        }

        private void HandleChangesToItemsSource()
        {
            int difference = EffectiveItemsSource.Count - _bars.Count;

            // If items have been added to the EffectiveItemsSource, update lists of bars and horizontal labels
            if (difference > 0)
            {
                for (int i = 0; i < Math.Abs(difference); i++)
                {
                    ContentPresenter bar = CreateBar(EffectiveItemsSource[i]);
                    _bars.Add(bar);
                    AddVisualChild(bar);

                    ContentPresenter hLabel = CreateHorizontalLabel(EffectiveItemsSource[i]);
                    _horizontalLabels.Add(hLabel);
                    AddVisualChild(hLabel);
                }
            }

            else if (difference < 0) // If an item has been removed from EffectiveItemsSource
            {
                for (int i = 0; i < Math.Abs(difference); i++)
                {
                    _bars[i].TargetUpdated -= new EventHandler<DataTransferEventArgs>(BarTargetUpdated);

                    RemoveVisualChild(_bars[i]);
                    _bars.RemoveAt(i);

                    RemoveVisualChild(_horizontalLabels[i]);
                    _horizontalLabels.RemoveAt(i);
                }
            }

            // Reset values of all the double holders
            _doubleHolders.Clear();
            for (int i = 0; i < EffectiveItemsSource.Count; i++)
            {
                Binding b = new Binding(ValuePath);
                b.Source = EffectiveItemsSource[i];
                DoubleHolder dh = new DoubleHolder();
                BindingOperations.SetBinding(dh, DoubleHolder.DoubleValueProperty, b);
                _doubleHolders.Add(dh);
            }

            // To reset the values that the bars represent, create new bindings for each bar with the
            //  correct source. No need to wire up the even handler because that has already been done
            //  for each bar. Also, create new bindings for the horizontal labels
            for (int i = 0; i < _bars.Count; i++)
            {
                Binding barBinding = new Binding(ValuePath);
                barBinding.Source = EffectiveItemsSource[i];
                barBinding.Mode = BindingMode.OneWay;
                barBinding.NotifyOnTargetUpdated = true;
                _bars[i].SetBinding(ContentPresenter.TagProperty, barBinding);
                _bars[i].TargetUpdated += new EventHandler<DataTransferEventArgs>(BarTargetUpdated);

                // Bind the text of the label to the property named by the NamePath property
                Binding myBinding = new Binding(this.NamePath);
                myBinding.Mode = BindingMode.OneWay;
                myBinding.Source = EffectiveItemsSource[i];

                if (HorizontalAxisLabelTemplate == null)
                {
                    TextBlock tb = new TextBlock();
                    tb.TextWrapping = TextWrapping.Wrap;
                    tb.TextAlignment = TextAlignment.Center;
                    tb.SetBinding(TextBlock.TextProperty, myBinding);
                    _horizontalLabels[i].Content = tb;
                }
                else
                {
                    _horizontalLabels[i].ContentTemplate = this.HorizontalAxisLabelTemplate;
                    _horizontalLabels[i].SetBinding(ContentPresenter.ContentProperty, myBinding);
                }
            }
            _visualsValid = true;
        }


        /// <summary>
        /// Create Vertical Label as ContentPresenter
        /// </summary>
        /// <returns></returns>
        private ContentPresenter CreateVerticalLabel()
        {
            ContentPresenter cp = new ContentPresenter();
            if (VerticalAxisLabelTemplate == null)
            {
                Label l = new Label();
                l.VerticalContentAlignment = VerticalAlignment.Center;
                l.HorizontalContentAlignment = HorizontalAlignment.Right;
                cp.Content = l;
            }
            else
            {
                cp.ContentTemplate = VerticalAxisLabelTemplate;
            }
            cp.MinHeight = MIN_LABEL_HEIGHT;
            return cp;
        }

        /// <summary>
        /// Create the Rects that will be passed into Arrange for the vertical label ContentPresenters
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        private void ArrangeVerticalLabels(Size constraint)
        {
            // Create the Canvas for the Vertical Scale Labels
            double rectWidth = VerticalLabelSize.Width;
            double rectHeight = VerticalLabelSize.Height;
            if (rectHeight < MIN_LABEL_HEIGHT)
                rectHeight = MIN_LABEL_HEIGHT;

            double increments = CalculatePixelIncrements(constraint);

            for (int i = 1; i <= _verticalLabels.Count; i++)
            {
                Rect r = new Rect(0, (i * increments) - (rectHeight / 2), rectWidth, rectHeight);
                _verticalLabels[i-1].Width = rectWidth;
                _verticalLabels[i-1].Height = rectHeight;
                _verticalLabels[i-1].Arrange(r);
            }

            // Create the list of numbers that will fill the vertical labels
            double[] numList = new double[_numReferenceLines];
            for (int i = 0; i < _numReferenceLines; i++)
            {
                if (_startingIncrement != 0)
                {
                    numList[i] = _startingIncrement + (i * _valueIncrement);
                }
                else
                {
                    numList[i] = _startingIncrement + ((i + 1) * _valueIncrement);
                }

                if (i == 0)
                {
                    _highValue = numList[i];
                }

                if (_highValue < numList[i])
                {
                    _highValue = numList[i];
                }
            }

            int count;
            if (_numReferenceLines < _verticalLabels.Count)
            {
                count = _numReferenceLines;
            }
            else
            {
                count = _verticalLabels.Count;
            }

            for (int i = 0; i < count; i++)
            {
                if (VerticalAxisLabelTemplate == null)
                {
                    Label l = new Label();
                    l.Content = numList[_numReferenceLines - i - 1].ToString();
                    l.VerticalContentAlignment = VerticalAlignment.Center;
                    l.HorizontalContentAlignment = HorizontalAlignment.Right;
                    _verticalLabels[i].Content = l;
                }
                else
                {
                    _verticalLabels[i].Content = numList[_numReferenceLines - i - 1].ToString();
                }
            }
        }

        /// <summary>
        /// Create Horizontal Label as ContentPresenter
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private ContentPresenter CreateHorizontalLabel(object o)
        {
            ContentPresenter cp = new ContentPresenter();

            cp.HorizontalAlignment = HorizontalAlignment.Center;

            // Bind the text of the label to the property named by the NamePath property
            Binding myBinding = new Binding(this.NamePath);
            myBinding.Mode = BindingMode.OneWay;
            myBinding.Source = o;


            cp.ContentTemplate = this.HorizontalAxisLabelTemplate;
            cp.SetBinding(ContentPresenter.ContentProperty, myBinding);

            return cp;
        }

        /// <summary>
        /// Create the Rects that will be passed into Arrange for the horizontal label ContentPresenters
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private void ArrangeHorizontalLabels(Size constraint)
        {
            double rectWidth = HorizontalLabelSize.Width;
            double rectHeight = HorizontalLabelSize.Height;
            double yCoord = constraint.Height - HorizontalLabelSize.Height;

            if (yCoord < (VerticalLabelSize.Height * (_numReferenceLines + 1)))
            {
                yCoord = (VerticalLabelSize.Height * (_numReferenceLines + 1));
            }

            _horizontalLabelPadding = ((constraint.Width - VerticalLabelSize.Width) - (EffectiveItemsSource.Count * rectWidth)) / (EffectiveItemsSource.Count + 1);
            if (_horizontalLabelPadding < HORIZ_LABEL_BUFFER)
                _horizontalLabelPadding = HORIZ_LABEL_BUFFER;

            for (int i = 0; i < _horizontalLabels.Count; i++)
            {
                Rect r = new Rect(((i + 1) * _horizontalLabelPadding) + (i * rectWidth) + (VerticalLabelSize.Width) + BRUSH_THICKNESS, yCoord, rectWidth, rectHeight);
                _horizontalLabels[i].Width = rectWidth;
                _horizontalLabels[i].Height = rectHeight;
                _horizontalLabels[i].Arrange(r);
            }
        }

        /// <summary>
        /// Crate Tick Mark as Rectangle
        /// </summary>
        /// <returns></returns>
        private Rectangle CreateTickMark()
        {
            Rectangle r = new Rectangle();
            r.Fill = VerticalAxisPen.Brush;
            return r;
        }

        /// <summary>
        /// Create tick mark Rects that will be passed into Arrange
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private void ArrangeTickMarks(Size constraint)
        {
            double increments = CalculatePixelIncrements(constraint);
            double x1 = VerticalLabelSize.Width;
            double x2 = x1 + TICK_LENGTH;

            for (int i = 0; i < _tickMarks.Count; i++)
            {
                Rect r = new Rect(x1, (_numReferenceLines - i) * _pixelIncrement - (BRUSH_THICKNESS / 2), x2 - x1, BRUSH_THICKNESS);
                _tickMarks[i].Arrange(r);
            }
        }

        /// <summary>
        /// Create Reference line as Rectangle
        /// </summary>
        /// <returns></returns>
        private Rectangle CreateReferenceLine()
        {
            Rectangle r = new Rectangle();
            r.Fill = ReferenceLinePen.Brush;
            return r;
        }

        /// <summary>
        /// Create Reference Line Rects that will be passed into Arrange
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private void ArrangeReferenceLines(Size constraint)
        {
            double increments = CalculatePixelIncrements(constraint);
            double x1 = (BRUSH_THICKNESS * 2) + VerticalLabelSize.Width;
            double x2 = constraint.Width;

            for (int i = 0; i < _referenceLines.Count; i++)
            {
                Rect r = new Rect(x1 - BRUSH_THICKNESS, (_numReferenceLines - i) * _pixelIncrement - (BRUSH_THICKNESS / 2), x2 - x1 + BRUSH_THICKNESS, BRUSH_THICKNESS);

                CalculateHorizontalAxisCoord(constraint);
                if (r.Y + (_pixelIncrement/2) > _horizAxisCoordinate &&
                    r.Y - (_pixelIncrement / 2) < _horizAxisCoordinate)
                {
                    _referenceLines[i].Visibility = Visibility.Collapsed;
                }
                else
                {
                    _referenceLines[i].Visibility = Visibility.Visible;
                }
                _referenceLines[i].Arrange(r);
            }
        }

        /// <summary>
        /// Create Axis Line Rects that will be passed into Arrange
        /// </summary>
        /// <param name="constraint"></param>
        private Point[] CreateAxisLinePoints(Size constraint)
        {
            double verticalXCoord, verticalY1Coord, verticalY2Coord, horizX1Coord, horizX2Coord, horizYCoord;

            verticalXCoord = BRUSH_THICKNESS + VerticalLabelSize.Width;
            verticalY1Coord = 0;
            verticalY2Coord = constraint.Height - HorizontalLabelSize.Height;
            if (verticalY2Coord < (VerticalLabelSize.Height * (_numReferenceLines + 1)))
            {
                verticalY2Coord = (VerticalLabelSize.Height * (_numReferenceLines + 1));
            }

            horizX1Coord = BRUSH_THICKNESS + VerticalLabelSize.Width;
            horizX2Coord = constraint.Width;
            CalculateHorizontalAxisCoord(constraint);
            horizYCoord = _horizAxisCoordinate;

            Point vertAxisStart = new Point(verticalXCoord + (BRUSH_THICKNESS / 2), verticalY1Coord + (BRUSH_THICKNESS / 2));
            Point vertAxisEnd = new Point(verticalXCoord + (BRUSH_THICKNESS / 2), verticalY1Coord + (verticalY2Coord - verticalY1Coord) + (BRUSH_THICKNESS / 2));
            Point horizAxisStart = new Point(horizX1Coord + (BRUSH_THICKNESS / 2), horizYCoord + (BRUSH_THICKNESS / 2));
            Point horizAxisEnd = new Point(horizX1Coord + (horizX2Coord - horizX1Coord) + (BRUSH_THICKNESS / 2), horizYCoord + (BRUSH_THICKNESS / 2));

            Point[] result = new Point[4];
            result[0] = vertAxisStart;
            result[1] = vertAxisEnd;
            result[2] = horizAxisStart;
            result[3] = horizAxisEnd;

            return result;
        }

        /// <summary>
        /// Calculate the Y Coordinate of the Horizontal Axis based on the overall size of the chart
        /// </summary>
        /// <param name="constraint"></param>
        private void CalculateHorizontalAxisCoord(Size constraint)
        {
            double horizYCoord = 0;

            // Calculate the y coordinate of the horizontal axis, depending on whether
            //  or not there are negative values in the dataset
            if (_startingIncrement >= 0)
            {
                horizYCoord = constraint.Height - HorizontalLabelSize.Height;

                if (horizYCoord < (VerticalLabelSize.Height * (_numReferenceLines + 1)))
                {
                    horizYCoord = (VerticalLabelSize.Height * (_numReferenceLines + 1));
                }
            }
            else
            {
                int i;
                for (i = 0; i < _numReferenceLines; i++)
                {
                    if (_startingIncrement + (i * _valueIncrement) == 0)
                    {
                        horizYCoord = (constraint.Height - HorizontalLabelSize.Height) - (_pixelIncrement * (i + 1));
                        break;
                    }
                }
            }

            if (horizYCoord != 0)
            {
                _horizAxisCoordinate = horizYCoord - (BRUSH_THICKNESS / 2);
            }
            else
            {
                _horizAxisCoordinate = horizYCoord;
            }
        }

        /// <summary>
        /// Create bar as ContentPresenter
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private ContentPresenter CreateBar(object o)
        {
            ContentPresenter cp = new ContentPresenter();
            cp.Content = o;

            Binding b = new Binding("BarTemplateSelector");
            b.Source = this;
            BindingOperations.SetBinding(cp, ContentPresenter.ContentTemplateSelectorProperty, b);

            Binding b2 = new Binding("BarTemplate");
            b2.Source = this;
            BindingOperations.SetBinding(cp, ContentPresenter.ContentTemplateProperty, b2);

            return cp;
        }

        /// <summary>
        /// Draw bars on the graph passed in as a parameter
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="content"></param>
        private void ArrangeBars(Size constraint)
        {
            double rectWidth = this.CalculateBarWidth(constraint, EffectiveItemsSource);

            for (int i = 0; i < _bars.Count; i++)
            {
                Rect r = new Rect();
                double temp = _doubleHolders[i].DoubleValue;

                r.Height = CalculateBarHeight(temp);
                r.Width = rectWidth;
                r.X = ((i + 1) * _barPadding) + (i * rectWidth) + (VerticalLabelSize.Width) + BRUSH_THICKNESS;

                if (temp >= 0)
                {
                    r.Y = _horizAxisCoordinate - r.Height + ReferenceLinePen.Thickness;
                }
                else
                {
                    r.Y = _horizAxisCoordinate + BRUSH_THICKNESS - ReferenceLinePen.Thickness;
                }

                _bars[i].Arrange(r);
            }
        }

        /// <summary>
        /// Calculate the Increment value that will correspond to the increments between
        /// the values of the labels on the scale axis
        /// </summary>
        /// <param name="size"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private double CalculateValueIncrement(Size size, IList content)
        {
            // Determine if the starting value is 0 or not
            bool startsAtZero = false;
            bool allPositiveValues = true;
            bool allNegativeValues = true;
            double increment_value = 0;
            int multiplier = 1;

            double low = 0, high = 0;

            if (EffectiveItemsSource.Count != 0)
            {
                low = _doubleHolders[0].DoubleValue;
                high = _doubleHolders[0].DoubleValue;
            }

            for (int i = 0; i < content.Count; i++)
            {
                double temp = _doubleHolders[i].DoubleValue;

                // Check for positive and negative values
                if (temp > 0)
                {
                    allNegativeValues = false;
                }
                else if (temp < 0)
                {
                    allPositiveValues = false;
                }

                // Reset low and high if necessary
                if (temp < low)
                {
                    low = temp;
                }
                else if (temp > high)
                {
                    high = temp;
                }
            }

            // Determine whether or not the increments will start at zero
            if (allPositiveValues && (low < (high / 2)) ||
                (allNegativeValues && high > (low / 2)))
            {
                _startsAtZero = true;
                startsAtZero = true;
            }

            // If all values in dataset are 0, draw one reference line and label it 0
            if (high == 0 && low == 0)
            {
                _valueIncrement = 0;
                _startingIncrement = 0;
                _numReferenceLines = 1;
                _startsAtZero = startsAtZero;
                return increment_value;
            }

            if (high == low && allNegativeValues)
            {
                _valueIncrement = Math.Abs(high);
                _startingIncrement = high;
                _numReferenceLines = 1;
                _startsAtZero = startsAtZero;
                return Math.Abs(high);
            }

            // Find an increment value that is in the set {1*10^x, 2*10^x, 5*10^x, where x is an integer 
            //  (positive, negative, or zero)}
            if (!allNegativeValues)
            {
                if (startsAtZero)
                {
                    int exp = 0;
                    while (true)
                    {
                        multiplier = IsWithinRange(high, exp, size);
                        if (multiplier != -1)
                        {
                            break;
                        }
                        multiplier = IsWithinRange(high, (-1 * exp), size);
                        if (multiplier != -1)
                        {
                            exp = -1 * exp;
                            break;
                        }
                        exp++;
                    }
                    increment_value = multiplier * Math.Pow(10, exp);
                }
                else
                {
                    int exp = 0;
                    while (true)
                    {
                        multiplier = IsWithinRange((high - low), exp, size);
                        if (multiplier != -1)
                        {
                            break;
                        }
                        multiplier = IsWithinRange((high - low), (-1 * exp), size);
                        if (multiplier != -1)
                        {
                            exp = -1 * exp;
                            break;
                        }
                        if (high == low)
                        {
                            increment_value = high;
                            _valueIncrement = increment_value;
                            _numReferenceLines = 1;
                            break;
                        }

                        exp++;
                    }
                    if (increment_value == 0)
                    {
                        increment_value = multiplier * Math.Pow(10, exp);
                    }
                }
            }
            else
            {
                if (startsAtZero)
                {
                    int exp = 0;
                    while (true)
                    {
                        multiplier = IsWithinRange(low, exp, size);
                        if (multiplier != -1)
                        {
                            break;
                        }
                        multiplier = IsWithinRange(low, (-1 * exp), size);
                        if (multiplier != -1)
                        {
                            exp = -1 * exp;
                            break;
                        }
                        exp++;
                    }
                    increment_value = multiplier * Math.Pow(10, exp);
                }
                else
                {
                    int exp = 0;
                    while (true)
                    {
                        multiplier = IsWithinRange((low - high), exp, size);
                        if (multiplier != -1)
                        {
                            break;
                        }
                        multiplier = IsWithinRange((low - high), (-1 * exp), size);
                        if (multiplier != -1)
                        {
                            exp = -1 * exp;
                            break;
                        }
                        exp++;
                    }
                    increment_value = multiplier * Math.Pow(10, exp);
                }
            }


            double starting_value = 0;

            // Determine starting value if it is nonzero
            if (!startsAtZero)
            {
                if (allPositiveValues)
                {
                    if (low % increment_value == 0)
                    {
                        starting_value = low;
                    }
                    else
                    {
                        starting_value = (int)(low / increment_value) * increment_value;
                    }
                }
                else
                {
                    if (low % increment_value == 0)
                    {
                        starting_value = low;
                    }
                    else
                    {
                        starting_value = (int)((low - increment_value) / increment_value) * increment_value;
                    }
                }
            }
            else if (startsAtZero && allNegativeValues)
            {
                if (low % increment_value == 0)
                {
                    starting_value = low;
                }
                else
                {
                    starting_value = (int)((low - increment_value) / increment_value) * increment_value;
                }
            }

            // Determine the number of reference lines
            int numRefLines = 0;

            if (allNegativeValues && !startsAtZero)
            {
                if ((high - starting_value) % increment_value == 0)
                {
                    numRefLines = (int)((high - starting_value) / increment_value) + 1;
                }
                else
                {
                    numRefLines = (int)((high - starting_value) / increment_value) + 1;
                }
            }
            else if (allNegativeValues && startsAtZero)
            {
                if ((high - starting_value) % increment_value == 0)
                {
                    numRefLines = Math.Abs((int)((starting_value) / increment_value));
                }
                else
                {
                    numRefLines = Math.Abs((int)((starting_value) / increment_value));
                }
            }
            else
            {
                if ((high - starting_value) % increment_value == 0 && _startsAtZero)
                {
                    numRefLines = (int)((high - starting_value) / increment_value);
                }
                else if (!allNegativeValues && !allPositiveValues)
                {
                    numRefLines = (int)((high - starting_value) / increment_value) + 2;
                }
                else
                {
                    numRefLines = (int)((high - starting_value) / increment_value) + 1;
                }
            }

            _valueIncrement = increment_value;
            _startingIncrement = starting_value;
            _numReferenceLines = numRefLines;
            _startsAtZero = startsAtZero;
            return increment_value;
        }

        /// <summary>
        /// Checks to see if the calculated increment value is between the low and high passed in, 
        /// then returns the multiplier used
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="exponent"></param>
        /// <param name="lowRange"></param>
        /// <param name="highRange"></param>
        /// <returns></returns>
        private int IsWithinRange(double numerator, int exponent, Size size)
        {
            double highRange, lowRange;
            if (size.Height >= ((6 * VerticalLabelSize.Height) + HorizontalLabelSize.Height))
            {
                lowRange = 4;
                highRange = 10;
            }
            else if (size.Height > ((4 * VerticalLabelSize.Height) + HorizontalLabelSize.Height))
            {
                lowRange = 3;
                highRange = 8;
            }
            else
            {
                lowRange = 1;
                highRange = 3;
            }


            if ((Math.Abs(numerator) / (1 * Math.Pow(10, exponent))) >= lowRange && (Math.Abs(numerator) / (1 * Math.Pow(10, exponent))) <= highRange)
            {
                return 1;
            }
            if ((Math.Abs(numerator) / (2 * Math.Pow(10, exponent))) >= lowRange && (Math.Abs(numerator) / (2 * Math.Pow(10, exponent))) <= highRange)
            {
                return 2;
            }
            if ((Math.Abs(numerator) / (5 * Math.Pow(10, exponent))) >= lowRange && (Math.Abs(numerator) / (5 * Math.Pow(10, exponent))) <= highRange)
            {
                return 5;
            }
            return -1;
        }

        /// <summary>
        /// Calculate the pixel distance between each tick mark on the vertical axis
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        private double CalculatePixelIncrements(Size constraint)
        {
            double result = (constraint.Height - HorizontalLabelSize.Height) / (_numReferenceLines + 1);
            if (result < VerticalLabelSize.Height)
                result = VerticalLabelSize.Height;
            return result;
        }


        /// <summary>
        /// Calculate the width of each bar according to the number of bars and the size parameter
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private double CalculateBarWidth(Size constraint, IList content)
        {
            double result = (((constraint.Width - VerticalLabelSize.Width) - BRUSH_THICKNESS - ((content.Count + 1) * BAR_PADDING)) / (content.Count));
            //double result = 0.8 * HorizontalLabelSize.Width;

            if (result < MIN_BAR_WIDTH)
            {
                _barPadding = ((constraint.Width - VerticalLabelSize.Width) - (content.Count * MIN_BAR_WIDTH)) / (content.Count + 1);
                return MIN_BAR_WIDTH;
            }
            if (result > MAX_BAR_WIDTH)
            {
                _barPadding = ((constraint.Width - VerticalLabelSize.Width) - (content.Count * MAX_BAR_WIDTH)) / (content.Count + 1);
                return MAX_BAR_WIDTH;
            }
            _barPadding = ((constraint.Width - VerticalLabelSize.Width) - (content.Count * result)) / (content.Count + 1);
            return result;
        }

        /// <summary>
        /// Calculate the height of each bar based on the both the value increment and the pixel increment
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private double CalculateBarHeight(double value)
        {
            if (_pixelIncrement == 0 || _valueIncrement == 0)
            {
                return 0.0;
            }
            if (_horizAxisCoordinate == 0 && !_startsAtZero)
            {
                return ((Math.Abs(value - _highValue) / _valueIncrement) + 1) * (_pixelIncrement);
            }
            if (_startsAtZero)
            {
                return (Math.Abs(value) / _valueIncrement) * (_pixelIncrement);
            }
            if (_startingIncrement == 0)
            {
                return (value / _valueIncrement) * (_pixelIncrement);
            }
            if (value == _startingIncrement && value >= 0)
            {
                return _pixelIncrement;
            }
            if (_startingIncrement > 0)
            {
                return (((value - _startingIncrement) / _valueIncrement) + 1) * (_pixelIncrement);
            }

            return (Math.Abs(value) / _valueIncrement) * (_pixelIncrement);

        }

        private static DataTemplateSelector getDefaultDataTemplateSelector()
        {
            if (DefaultBarTemplateSelectorInstance == null)
            {
                DefaultBarTemplateSelectorInstance = new DefaultBarTemplateSelector();
            }
            return DefaultBarTemplateSelectorInstance;
        }

        private int _numReferenceLines;
        private bool _startsAtZero;
        private bool _referenceLinesValid;
        private bool _visualsValid;
        private double _startingIncrement;
        private double _valueIncrement;
        private double _pixelIncrement;
        private double _horizAxisCoordinate;
        private double _barPadding;
        private double _horizontalLabelPadding;
        private double _highValue;
        private double _layoutCount;
        private IList<ContentPresenter> _verticalLabels;
        private IList<ContentPresenter> _horizontalLabels;
        private IList<ContentPresenter> _bars;
        private IList<Rectangle> _referenceLines;
        private IList<Rectangle> _tickMarks;
        private IList<DoubleHolder> _doubleHolders;
        private const double MAX_BAR_WIDTH = 100;
        private const double MIN_BAR_WIDTH = 3;
        private const double HORIZ_LABEL_BUFFER = 2;
        private const double VERT_LABEL_BUFFER = 2;
        private const double TICK_LENGTH = 9.0;
        private const double BAR_PADDING = 10;
        private const double MIN_LABEL_HEIGHT = 30;
        private const double BRUSH_THICKNESS = 3.0;
        private const int MIN_REF_LINES = 3;
        private static DataTemplateSelector DefaultBarTemplateSelectorInstance;
        public static readonly ComponentResourceKey DefaultBarTemplateKey =
            new ComponentResourceKey(typeof(BarChartControl), "DefaultBarTemplate");


        private class DefaultBarTemplateSelector : DataTemplateSelector
        {
            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                if (_defaultBarDataTemplate == null)
                {
                    _defaultBarDataTemplate =
                        (DataTemplate)((FrameworkElement)container).FindResource(BarChartControl.DefaultBarTemplateKey);
                }

                return _defaultBarDataTemplate;
            }

            private DataTemplate _defaultBarDataTemplate;
        }
    }

    internal class DoubleHolder : DependencyObject
    {
        public static DependencyProperty DoubleValueProperty =
        DependencyProperty.Register(
          "DoubleValue",
          typeof(double),
          typeof(DoubleHolder), new FrameworkPropertyMetadata((double)0, new PropertyChangedCallback(DoubleValueChanged)));

        public double DoubleValue
        {
            get { return (double)GetValue(DoubleValueProperty); }
            set { SetValue(DoubleValueProperty, value); }
        }

        private static void DoubleValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}
