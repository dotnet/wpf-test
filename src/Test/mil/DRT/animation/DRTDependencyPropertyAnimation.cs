// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using DRT;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace DRTAnimation
{
    public sealed class DRTCSharpAnimationTestSuite : DrtTestSuite
    {
        AnimationClock _progressBarAnimation;

        public DRTCSharpAnimationTestSuite() : base("C# Animation Tests")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            Border border;

            LinearGradientBrush lgBrush;

            SolidColorBrush scBrush;

            DoubleAnimation lanim;
            DoubleAnimation danim;
            ColorAnimation canim;
            PointAnimation panim;
    
            Thickness panelMargin = new Thickness(10);

            RepeatBehavior repeatBehavior = RepeatBehavior.Forever;

            lgBrush = new LinearGradientBrush();
            lgBrush.GradientStops.Add(new GradientStop(Colors.Blue, 0.0));
            lgBrush.GradientStops.Add(new GradientStop(Colors.LightBlue, 1.0));
            lgBrush.StartPoint = new Point(0.0, 0.0);
            lgBrush.EndPoint = new Point(1.0, 1.0);
            lgBrush.MappingMode = BrushMappingMode.RelativeToBoundingBox;

            // Main Panel

            StackPanel mainPanel = new StackPanel();
            mainPanel.Background = lgBrush;

            // Progress Bar Panel

            StackPanel progressBarPanel = new StackPanel();
            progressBarPanel.Orientation = Orientation.Horizontal;
            progressBarPanel.Margin = panelMargin;
            mainPanel.Children.Add(progressBarPanel);

            Border progressBarBorder = new Border();
            progressBarBorder.BorderThickness = new Thickness(2);
            progressBarBorder.BorderBrush = Brushes.Yellow;
            progressBarBorder.Width = 500;
            progressBarPanel.Children.Add(progressBarBorder);

            Rectangle rectangle = new Rectangle();
            rectangle.HorizontalAlignment = HorizontalAlignment.Left;
            rectangle.Width = 50.0;
            rectangle.Height = 20.0;
            rectangle.Fill = Brushes.Red;
            progressBarBorder.Child = rectangle;

            lanim = new DoubleAnimation(
                0.0, 
                500.0,
                new TimeSpan(0, 0, 0, 0, 2000),
                FillBehavior.HoldEnd);

            lanim.BeginTime = null;

            if (!DRTAnimation.IsInteractive)
            {
                lanim.CurrentStateInvalidated += new EventHandler(OnCurrentStateInvalidated);
            }

            _progressBarAnimation = lanim.CreateClock();

            rectangle.ApplyAnimationClock(Rectangle.WidthProperty, _progressBarAnimation);

            // Button panel

            StackPanel buttonPanel = new StackPanel();
            buttonPanel.Orientation = Orientation.Horizontal;
            buttonPanel.Margin = panelMargin;
            mainPanel.Children.Add(buttonPanel);

            Button button = new Button();
            button.Content = "Begin";
            button.Click += new RoutedEventHandler(OnBeginClicked);
            buttonPanel.Children.Add(button);

            button = new Button();
            button.Content = "Pause";
            button.Click += new RoutedEventHandler(OnPauseClicked);
            buttonPanel.Children.Add(button);

            button = new Button();
            button.Content = "Resume";
            button.Click += new RoutedEventHandler(OnResumeClicked);
            buttonPanel.Children.Add(button);

            button = new Button();
            button.Content = "Stop";
            button.Click += new RoutedEventHandler(OnStopClicked);
            buttonPanel.Children.Add(button);

            button = new Button();
            button.Content = "Next Test";
            button.Click += new RoutedEventHandler(OnNextTestClicked);
            if (!DRTAnimation.IsInteractive) { button.IsEnabled = false; }
            buttonPanel.Children.Add(button);

            // Test Panel 1

            StackPanel testPanel1 = new StackPanel();
            testPanel1.Orientation = Orientation.Horizontal;
            testPanel1.Margin = panelMargin;
            mainPanel.Children.Add(testPanel1);

            //
            // SolidColorPaint Color Animation
            //

            canim = new ColorAnimation(Colors.LightGreen, Colors.DarkGreen, new TimeSpan(0, 0, 0, 0, 1000));
            canim.RepeatBehavior = repeatBehavior;
            canim.AutoReverse = true;

            scBrush = new SolidColorBrush(Colors.Green);
            scBrush.BeginAnimation(SolidColorBrush.ColorProperty, canim);

            border = new Border();
            border.Background = scBrush;
            border.BorderThickness = new Thickness(5.0);
            border.Width = 60.0;
            border.Height = 60.0;

            testPanel1.Children.Add(border);

            //
            // SolidColorPaint Opacity Animation
            //

            danim = new DoubleAnimation(0.0, 1.0, new TimeSpan(0, 0, 0, 0, 1000));
            danim.RepeatBehavior = repeatBehavior;
            danim.AutoReverse = true;

            scBrush = new SolidColorBrush(Colors.Yellow);
            scBrush.BeginAnimation(SolidColorBrush.OpacityProperty, danim);

            border = new Border();
            border.Background = scBrush;
            border.BorderThickness = new Thickness(5.0);
            border.Width = 60.0;
            border.Height = 60.0;

            testPanel1.Children.Add(border);

            //
            // LinearGradientPaint Color Animations
            //

            lgBrush = new LinearGradientBrush();
            lgBrush.StartPoint = new Point(0.0, 0.2);
            lgBrush.EndPoint = new Point(1.0, 0.8);
            lgBrush.MappingMode = BrushMappingMode.RelativeToBoundingBox;
            lgBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 0.0));
            lgBrush.GradientStops.Add(new GradientStop(Colors.Red, 0.5));
            lgBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 1.0));

            panim = new PointAnimation(new Point(0.0, 0.0), new Point(0.0, 1.0), new TimeSpan(0, 0, 0, 0, 1000));
            panim.RepeatBehavior = repeatBehavior;
            panim.AutoReverse = true;

            lgBrush.BeginAnimation(LinearGradientBrush.StartPointProperty, panim);

            panim = panim.Clone();
            panim.From = new Point(1.0, 1.0);
            panim.To = new Point(1.0, 0.0);

            lgBrush.BeginAnimation(LinearGradientBrush.EndPointProperty, panim);

            border = new Border();
            border.Background = lgBrush;
            border.BorderThickness = new Thickness(5.0);
            border.Width = 60.0;
            border.Height = 60.0;

            testPanel1.Children.Add(border);

            //
            // RadialGradientBrush Center Animations
            //

            RadialGradientBrush rgBrush = new RadialGradientBrush();

            rgBrush.GradientStops.Add(new GradientStop(Colors.Green, 0.0));
            rgBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 1.0));

            panim = new PointAnimation(new Point(0.2, 0.2), new Point(0.8, 0.8), new TimeSpan(0, 0, 0, 0, 1000));
            panim.RepeatBehavior = repeatBehavior;
            panim.AutoReverse = true;

            AnimationClock animationClock = panim.CreateClock();
            rgBrush.ApplyAnimationClock(RadialGradientBrush.CenterProperty, animationClock);
            rgBrush.ApplyAnimationClock(RadialGradientBrush.GradientOriginProperty, animationClock);

            border = new Border();
            border.Background = rgBrush;
            border.BorderThickness = new Thickness(5.0);
            border.Width = 60.0;
            border.Height = 60.0;

            testPanel1.Children.Add(border);

            //
            // RotateTransform
            //

            danim = new DoubleAnimation(0.0, 360.0, new TimeSpan(0, 0, 0, 0, 1000));
            danim.RepeatBehavior = repeatBehavior;
            danim.AutoReverse = true;

            RotateTransform rTransform = new RotateTransform(0.0, /* centerX = */ 30.0, /* centerY = */ 30.0);
            rTransform.BeginAnimation(RotateTransform.AngleProperty, danim);

            border = new Border();
            border.Background = Brushes.Yellow;
            border.BorderThickness = new Thickness(5.0);
            border.Width = 60.0;
            border.Height = 60.0;
            border.RenderTransform = rTransform;

            testPanel1.Children.Add(border);

            //
            // Translate Transform
            //

            danim = new DoubleAnimation(0.0, 50.0, new TimeSpan(0, 0, 0, 0, 1000));
            danim.RepeatBehavior = repeatBehavior;
            danim.AutoReverse = true;

            TranslateTransform tTransform = new TranslateTransform();
            tTransform.BeginAnimation(TranslateTransform.YProperty, danim);

            border = new Border();
            border.Background = Brushes.Red;
            border.BorderThickness = new Thickness(5.0);
            border.Width = 60.0;
            border.Height = 60.0;
            border.RenderTransform = tTransform;

            testPanel1.Children.Add(border);

            //
            // Scale Transform
            //

            danim = new DoubleAnimation(0.0, 2.0, new TimeSpan(0, 0, 0, 0, 1000));
            danim.RepeatBehavior = repeatBehavior;
            danim.AutoReverse = true;

            ScaleTransform sTransform = new ScaleTransform(1.0, 1.0, /* centerX = */ 30.0, /* centerY = */ 30.0); 
            sTransform.BeginAnimation(ScaleTransform.ScaleXProperty, danim);

            border = new Border();
            border.Background = Brushes.Green;
            border.BorderThickness = new Thickness(5.0);
            border.Width = 60.0;
            border.Height = 60.0;
            border.RenderTransform = sTransform;

            testPanel1.Children.Add(border);

            //
            // Skew Transform
            // 

            danim = new DoubleAnimation(0.0, 45.0, new TimeSpan(0, 0, 0, 0, 1000));
            danim.RepeatBehavior = repeatBehavior;
            danim.AutoReverse = true;

            SkewTransform skTransform = new SkewTransform(0.0, 0.0, /* centerX = */ 30.0, /* centerY = */ 30.0);
            skTransform.BeginAnimation(SkewTransform.AngleXProperty, danim);
            skTransform.BeginAnimation(SkewTransform.AngleYProperty, danim);

            border = new Border();
            border.Background = Brushes.Yellow;
            border.BorderThickness = new Thickness(5.0);
            border.Width = 60.0;
            border.Height = 60.0;
            border.RenderTransform = skTransform;

            testPanel1.Children.Add(border);

            //
            // GradientStop Color Animations
            //

            lgBrush = new LinearGradientBrush();
            lgBrush.StartPoint = new Point(0.0, 0.2);
            lgBrush.EndPoint = new Point(1.0, 0.8);
            lgBrush.MappingMode = BrushMappingMode.RelativeToBoundingBox;

            canim = new ColorAnimation(Colors.Red, Colors.Green, new TimeSpan(0, 0, 0, 0, 1000));
            canim.RepeatBehavior = repeatBehavior;
            canim.AutoReverse = true;

            GradientStop stop = new GradientStop(Colors.Red, 0.0);
            stop.BeginAnimation(GradientStop.ColorProperty, canim);

            lgBrush.GradientStops.Add(stop);

            stop = stop.Clone();
            canim.From = Colors.Green;
            canim.To = Colors.Yellow;

            stop.Color = Colors.Green;
            stop.Offset = 1.0;
            stop.BeginAnimation(GradientStop.ColorProperty, canim);

            lgBrush.GradientStops.Add(stop);

            border = new Border();
            border.Background = lgBrush;
            border.BorderThickness = new Thickness(5.0);
            border.Width = 60.0;
            border.Height = 60.0;

            testPanel1.Children.Add(border);

            // Test Panel 2

            StackPanel testPanel2 = new StackPanel();
            testPanel2.Orientation = Orientation.Horizontal;
            testPanel2.Margin = panelMargin;
            mainPanel.Children.Add(testPanel2);

            // 
            // DCAnimatedDrawLineTest
            //

            testPanel2.Children.Add(new DCAnimatedDrawLineTest());

            //
            // DCAnimatedDrawRectangleTest
            //

            testPanel2.Children.Add(new DCAnimatedDrawRectangleTest());

            //
            // DCAnimatedDrawRoundedRectangleTest
            //

            testPanel2.Children.Add(new DCAnimatedDrawRoundedRectangleTest());

            //
            // DCAnimatedDrawEllipseTest
            //

            testPanel2.Children.Add(new DCAnimatedDrawEllipseTest());

            // Test Panel 3

            StackPanel testPanel3 = new StackPanel();
            testPanel3.Orientation = Orientation.Horizontal;
            testPanel3.Margin = panelMargin;
            mainPanel.Children.Add(testPanel3);

            //
            // DCAnimatedDrawImageTest
            //

            testPanel3.Children.Add(new DCAnimatedDrawImageTest());

            //
            // DCAnimatedDrawDrawingTest
            //

            testPanel3.Children.Add(new DCAnimatedDrawDrawingTest());

            //
            // DCAnimatedPushOpacityTest
            //

            testPanel3.Children.Add(new DCAnimatedPushOpacityTest());

            DRT.RootElement = mainPanel;
            DRT.ShowRoot();

            return new DrtTest[] 
            {
                new DrtTest(RunTest),
            };
        }

        void RunTest()
        {
            _progressBarAnimation.Controller.Begin();

            DRT.Suspend();
        }

        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {
            Clock clock = (Clock)sender;

            if (clock.CurrentState != ClockState.Active)
            {
                DRT.Resume();

                Console.WriteLine("    DRTCSharpAnimationTestSuite.RunTest() completed.");
            }
        }

        private void OnBeginClicked(object sender, RoutedEventArgs e)
        {
            _progressBarAnimation.Controller.Begin();
        }

        private void OnPauseClicked(object sender, RoutedEventArgs e)
        {
            _progressBarAnimation.Controller.Pause();
        }

        private void OnResumeClicked(object sender, RoutedEventArgs e)
        {
            _progressBarAnimation.Controller.Resume();
        }

        private void OnStopClicked(object sender, RoutedEventArgs e)
        {
            _progressBarAnimation.Controller.Stop();
        }

        private void OnNextTestClicked(object sender, RoutedEventArgs e)
        {
            DRT.Resume();

            Console.WriteLine("    DRTCSharpAnimationTestSuite.RunTest() ended using 'Next Test' button.");
        }
    }
}
