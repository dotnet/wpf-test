using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System;
using System.Globalization;
using System.Diagnostics;

namespace Microsoft.Test.Controls
{
    public partial class InheritedDPRegressionTest7 : Window
    {
        public static readonly DependencyProperty InheritedIntProperty = DependencyProperty.RegisterAttached(
            "InheritedInt",
            typeof(int),
            typeof(InheritedDPRegressionTest7),
            new FrameworkPropertyMetadata(
                0,
                FrameworkPropertyMetadataOptions.Inherits,
                new PropertyChangedCallback(InheritedIntChanged),
                new CoerceValueCallback(CoerceValue)));

        public static void InheritedIntChanged(DependencyObject changedObject, DependencyPropertyChangedEventArgs args)
        {
            Debug.WriteLine(string.Format("{0} now has value of {1}", changedObject.GetType(), args.NewValue));
        }

        public static object CoerceValue(DependencyObject dependencyObject, object val)
        {
            if (dependencyObject.GetType() == typeof(InheritedDPRegressionTest7))
            {
                return 10;
            }
            if (dependencyObject.GetType() == typeof(Grid))
            {
                return 5;
            }

            return val;
        }

        public InheritedDPRegressionTest7()
        {
            InitializeComponent();
            this.SetValue(InheritedIntProperty, 1);
        }

        private void ButtonClicked(object sender, RoutedEventArgs e)
        {
            grid.CoerceValue(InheritedDPRegressionTest7.InheritedIntProperty);
            this.InvalidateVisual();
        }
    }

    public class MyControl : ContentControl
    {
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == InheritedDPRegressionTest7.InheritedIntProperty)
            {
                propertyChangedInt = (int)e.NewValue;
                Dispatcher.BeginInvoke((Action)delegate()
                {
                    InvalidateVisual();
                });
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawText(new FormattedText(
                propertyChangedInt.ToString(),
                CultureInfo.InstalledUICulture,
                FlowDirection.LeftToRight,
                new Typeface("Veranda"),
                12,
                Brushes.DarkGreen),
                new Point(0, 0));

            drawingContext.DrawText(new FormattedText(
                this.GetValue(InheritedDPRegressionTest7.InheritedIntProperty).ToString(),
                CultureInfo.InstalledUICulture,
                FlowDirection.LeftToRight,
                new Typeface("Veranda"),
                12,
                Brushes.DarkGreen),
                new Point(0, 30));
            base.OnRender(drawingContext);
        }

        private int propertyChangedInt = 0;
    }
}
