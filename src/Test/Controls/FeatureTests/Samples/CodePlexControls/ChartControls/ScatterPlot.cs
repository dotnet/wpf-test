using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
namespace WpfControlToolkit
{
    public class ScatterPlot : Chart
    {
        static ScatterPlot()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScatterPlot), new FrameworkPropertyMetadata(typeof(ScatterPlot)));
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ContentControl();
        }

        public PropertyPath XValuePath
        {
            get { return (PropertyPath)GetValue(XValuePathProperty); }
            set { SetValue(XValuePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XValuePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XValuePathProperty =
            DependencyProperty.Register("XValuePath", typeof(PropertyPath), typeof(ScatterPlot), new UIPropertyMetadata(null));


        public PropertyPath YValuePath
        {
            get { return (PropertyPath)GetValue(YValuePathProperty); }
            set { SetValue(YValuePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YValuePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YValuePathProperty =
            DependencyProperty.Register("YValuePath", typeof(PropertyPath), typeof(ScatterPlot), new UIPropertyMetadata(null));

        

        public DataTemplate YAxisItemTemplate
        {
            get { return (DataTemplate)GetValue(YAxisItemTemplateProperty); }
            set { SetValue(YAxisItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YAxisItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YAxisItemTemplateProperty =
            DependencyProperty.Register("YAxisItemTemplate", typeof(DataTemplate), typeof(ScatterPlot), new UIPropertyMetadata(null));


        public DataTemplateSelector YAxisItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(YAxisItemTemplateSelectorProperty); }
            set { SetValue(YAxisItemTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YAxisItemTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YAxisItemTemplateSelectorProperty =
            DependencyProperty.Register("YAxisItemTemplateSelector", typeof(DataTemplateSelector), typeof(ScatterPlot), new UIPropertyMetadata(null));


        public DataTemplate XAxisItemTemplate
        {
            get { return (DataTemplate)GetValue(XAxisItemTemplateProperty); }
            set { SetValue(XAxisItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XAxisItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XAxisItemTemplateProperty =
            DependencyProperty.Register("XAxisItemTemplate", typeof(DataTemplate), typeof(ScatterPlot), new UIPropertyMetadata(null));


        public DataTemplateSelector XAxisItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(XAxisItemTemplateSelectorProperty); }
            set { SetValue(XAxisItemTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XAxisItemTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XAxisItemTemplateSelectorProperty =
            DependencyProperty.Register("XAxisItemTemplateSelector", typeof(DataTemplateSelector), typeof(ScatterPlot), new UIPropertyMetadata(null));


        public object YAxisTitle
        {
            get { return (object)GetValue(YAxisTitleProperty); }
            set { SetValue(YAxisTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YAxisTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YAxisTitleProperty =
            DependencyProperty.Register("YAxisTitle", typeof(object), typeof(ScatterPlot), new UIPropertyMetadata(null));


        public DataTemplate YAxisTitleTemplate
        {
            get { return (DataTemplate)GetValue(YAxisTitleTemplateProperty); }
            set { SetValue(YAxisTitleTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YAxisTitleTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YAxisTitleTemplateProperty =
            DependencyProperty.Register("YAxisTitleTemplate", typeof(DataTemplate), typeof(ScatterPlot), new UIPropertyMetadata(null));


        public DataTemplateSelector YAxisTitleTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(YAxisTitleTemplateSelectorProperty); }
            set { SetValue(YAxisTitleTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YAxisTitleTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YAxisTitleTemplateSelectorProperty =
            DependencyProperty.Register("YAxisTitleTemplateSelector", typeof(DataTemplateSelector), typeof(ScatterPlot), new UIPropertyMetadata(null));


        public object XAxisTitle
        {
            get { return (object)GetValue(XAxisTitleProperty); }
            set { SetValue(XAxisTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XAxisTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XAxisTitleProperty =
            DependencyProperty.Register("XAxisTitle", typeof(object), typeof(ScatterPlot), new UIPropertyMetadata(null));


        public DataTemplate XAxisTitleTemplate
        {
            get { return (DataTemplate)GetValue(XAxisTitleTemplateProperty); }
            set { SetValue(XAxisTitleTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XAxisTitleTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XAxisTitleTemplateProperty =
            DependencyProperty.Register("XAxisTitleTemplate", typeof(DataTemplate), typeof(ScatterPlot), new UIPropertyMetadata(null));


        public DataTemplateSelector XAxisTitleTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(XAxisTitleTemplateSelectorProperty); }
            set { SetValue(XAxisTitleTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XAxisTitleTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XAxisTitleTemplateSelectorProperty =
            DependencyProperty.Register("XAxisTitleTemplateSelector", typeof(DataTemplateSelector), typeof(ScatterPlot), new UIPropertyMetadata(null));



        public bool ShowXAxisTicks
        {
            get { return (bool)GetValue(ShowXAxisTicksProperty); }
            set { SetValue(ShowXAxisTicksProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowXAxisTicks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowXAxisTicksProperty =
            DependencyProperty.Register("ShowXAxisTicks", typeof(bool), typeof(ScatterPlot), new UIPropertyMetadata(null));



        public bool ShowXAxisReferenceLines
        {
            get { return (bool)GetValue(ShowXAxisReferenceLinesProperty); }
            set { SetValue(ShowXAxisReferenceLinesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowXAxisReferenceLines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowXAxisReferenceLinesProperty =
            DependencyProperty.Register("ShowXAxisReferenceLines", typeof(bool), typeof(ScatterPlot), new UIPropertyMetadata(null));




        public bool ShowYAxisTicks
        {
            get { return (bool)GetValue(ShowYAxisTicksProperty); }
            set { SetValue(ShowYAxisTicksProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowYAxisTicks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowYAxisTicksProperty =
            DependencyProperty.Register("ShowYAxisTicks", typeof(bool), typeof(ScatterPlot), new UIPropertyMetadata(null));



        public bool ShowYAxisReferenceLines
        {
            get { return (bool)GetValue(ShowYAxisReferenceLinesProperty); }
            set { SetValue(ShowYAxisReferenceLinesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowYAxisReferenceLines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowYAxisReferenceLinesProperty =
            DependencyProperty.Register("ShowYAxisReferenceLines", typeof(bool), typeof(ScatterPlot), new UIPropertyMetadata(null));


        public Pen AxisLinePen
        {
            get { return (Pen)GetValue(AxisLinePenProperty); }
            set { SetValue(AxisLinePenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AxisLinePen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AxisLinePenProperty =
            DependencyProperty.Register("AxisLinePen", typeof(Pen), typeof(ScatterPlot), new UIPropertyMetadata(null));


        public Pen PlottingPen
        {
            get { return (Pen)GetValue(PlottingPenProperty); }
            set { SetValue(PlottingPenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlottingPen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlottingPenProperty =
            DependencyProperty.Register("PlottingPen", typeof(Pen), typeof(ScatterPlot), new UIPropertyMetadata(null));


        public double TickLength
        {
            get { return (double)GetValue(TickLengthProperty); }
            set { SetValue(TickLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TickLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TickLengthProperty =
            DependencyProperty.Register("TickLength", typeof(double), typeof(ScatterPlot), new UIPropertyMetadata(null));


        public bool IsSmoothOutline
        {
            get { return (bool)GetValue(IsSmoothOutlineProperty); }
            set { SetValue(IsSmoothOutlineProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSmoothOutline.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSmoothOutlineProperty =
            DependencyProperty.Register("IsSmoothOutline", typeof(bool), typeof(ScatterPlot), new UIPropertyMetadata(null));



        //public bool JoinPoints
        //{
        //    get { return (bool)GetValue(JoinPointsProperty); }
        //    set { SetValue(JoinPointsProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for JoinPoints.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty JoinPointsProperty =
        //    DependencyProperty.Register("JoinPoints", typeof(bool), typeof(ScatterPlot), new UIPropertyMetadata(null));



    }
}
