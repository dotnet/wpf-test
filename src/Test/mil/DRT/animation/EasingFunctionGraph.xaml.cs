// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

namespace DRTAnimation
{
    /// <summary>
    /// Interaction logic for EasingFunctionGraph.xaml
    /// </summary>
    public partial class EasingFunctionGraph : UserControl
    {
        /// <summary>
        /// EasingFunctionProperty
        /// </summary>                                 
        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register(
                "EasingFunction",
                typeof(IEasingFunction),
                typeof(EasingFunctionGraph),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, OnEasingFunctionChanged));

        private static void OnEasingFunctionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((EasingFunctionGraph)sender).UpdateEasingFunction();
        }

        private void UpdateEasingFunction()
        {
            var function = EasingFunction;
            var path = functionPath;
            
            if (function == null)
            {
                path.Data = null;
            }
            else
            {
                var data = new StreamGeometry();
                var context = data.Open();
                context.BeginFigure(new Point(0, 0), false, false);
                for(int i=0; i<100; i++)
                {
                    context.LineTo(new Point((double)i, -50.0 * function.Ease(((double)i) / 99.0)), true, true);
                }
                context.Close();

                path.Data = data;

                var duration = new Duration(TimeSpan.FromSeconds(5.0));
                var da = new DoubleAnimation(0.0, -50.0, duration);
                da.RepeatBehavior = RepeatBehavior.Forever;
                da.EasingFunction = function;

                functionCircle.BeginAnimation(TranslateTransform.YProperty, da);
                animationCircle.BeginAnimation(TranslateTransform.YProperty, da);

                da = new DoubleAnimation(0.0, 100.0, duration);
                da.RepeatBehavior = RepeatBehavior.Forever;
                functionCircle.BeginAnimation(TranslateTransform.XProperty, da);
            }
        }

        /// <summary>
        /// EasingFunction
        /// </summary>
        public IEasingFunction EasingFunction                
        {
            get
            {
                return (IEasingFunction)GetValue(EasingFunctionProperty);
            }
            set
            {
                SetValue(EasingFunctionProperty, value);
            }
        }

        public EasingFunctionGraph()
        {
            InitializeComponent();
        }
    }
}
