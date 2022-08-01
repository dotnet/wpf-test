// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// DRTCompositionTargetRendering.cs

using DRT;
using System;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace DRTAnimation
{
    public sealed class DRTEasingTestSuite : DrtTestSuite
    {
        private class EasingGraphData : DependencyObject
        {
            public EasingGraphData(string title, string subtitle, EasingFunctionBase easetemplate)
            {
                Title = title;
                SubTitle = subtitle;
                EaseIn = (EasingFunctionBase)easetemplate.Clone();
                EaseOut = (EasingFunctionBase)easetemplate.Clone();
                EaseInOut = (EasingFunctionBase)easetemplate.Clone();

                EaseIn.EasingMode = EasingMode.EaseIn;
                EaseOut.EasingMode = EasingMode.EaseOut;
                EaseInOut.EasingMode = EasingMode.EaseInOut;
            }

            public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(EasingGraphData));
            public string Title { get { return (string)GetValue(TitleProperty); } set { SetValue(TitleProperty, value); } }

            public static readonly DependencyProperty SubTitleProperty = DependencyProperty.Register("SubTitle", typeof(string), typeof(EasingGraphData));
            public string SubTitle { get { return (string)GetValue(SubTitleProperty); } set { SetValue(SubTitleProperty, value); } }

            public static readonly DependencyProperty EaseInProperty = DependencyProperty.Register("EaseIn", typeof(EasingFunctionBase), typeof(EasingGraphData));
            public EasingFunctionBase EaseIn { get { return (EasingFunctionBase)GetValue(EaseInProperty); } set { SetValue(EaseInProperty, value); } }

            public static readonly DependencyProperty EaseOutProperty = DependencyProperty.Register("EaseOut", typeof(EasingFunctionBase), typeof(EasingGraphData));
            public EasingFunctionBase EaseOut { get { return (EasingFunctionBase)GetValue(EaseOutProperty); } set { SetValue(EaseOutProperty, value); } }

            public static readonly DependencyProperty EaseInOutProperty = DependencyProperty.Register("EaseInOut", typeof(EasingFunctionBase), typeof(EasingGraphData));
            public EasingFunctionBase EaseInOut { get { return (EasingFunctionBase)GetValue(EaseInOutProperty); } set { SetValue(EaseInOutProperty, value); } }
        }

        private class AnimationDataModel : Animatable
        {
            public static readonly DependencyProperty ByteProperty = DependencyProperty.Register("Byte", typeof(Byte), typeof(AnimationDataModel));
            public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(AnimationDataModel));
            public static readonly DependencyProperty DecimalProperty = DependencyProperty.Register("Decimal", typeof(Decimal), typeof(AnimationDataModel));
            public static readonly DependencyProperty DoubleProperty = DependencyProperty.Register("Double", typeof(Double), typeof(AnimationDataModel));
            public static readonly DependencyProperty Int16Property = DependencyProperty.Register("Int16", typeof(Int16), typeof(AnimationDataModel));
            public static readonly DependencyProperty Int32Property = DependencyProperty.Register("Int32", typeof(Int32), typeof(AnimationDataModel));
            public static readonly DependencyProperty Int64Property = DependencyProperty.Register("Int64", typeof(Int64), typeof(AnimationDataModel));
            public static readonly DependencyProperty PointProperty = DependencyProperty.Register("Point", typeof(Point), typeof(AnimationDataModel));
            public static readonly DependencyProperty Point3DProperty = DependencyProperty.Register("Point3D", typeof(Point3D), typeof(AnimationDataModel));
            public static readonly DependencyProperty QuaternionProperty = DependencyProperty.Register("Quaternion", typeof(Quaternion), typeof(AnimationDataModel));
            public static readonly DependencyProperty Rotation3DProperty = DependencyProperty.Register("Rotation3D", typeof(Rotation3D), typeof(AnimationDataModel));
            public static readonly DependencyProperty RectProperty = DependencyProperty.Register("Rect", typeof(Rect), typeof(AnimationDataModel));
            public static readonly DependencyProperty SingleProperty = DependencyProperty.Register("Single", typeof(Single), typeof(AnimationDataModel));
            public static readonly DependencyProperty SizeProperty = DependencyProperty.Register("Size", typeof(Size), typeof(AnimationDataModel));
            public static readonly DependencyProperty VectorProperty = DependencyProperty.Register("Vector", typeof(Vector), typeof(AnimationDataModel));
            public static readonly DependencyProperty Vector3DProperty = DependencyProperty.Register("Vector3D", typeof(Vector3D), typeof(AnimationDataModel));
            public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register("Thickness", typeof(Thickness), typeof(AnimationDataModel));

            protected override Freezable CreateInstanceCore()
            {
                return new AnimationDataModel();
            }
        }

        private AnimationDataModel dataModel = new AnimationDataModel();

        public DRTEasingTestSuite()
            : base("Easing")
        {
            this.Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            DRT.LoadXamlFile(@"DrtFiles\DrtAnimation\DRTEasing.xaml");
            var root = DRT.RootElement as FrameworkElement;
            if (root != null)
            {
                root.DataContext = dataModel;
            }

            if (!DRT.KeepAlive)
            {
                return new DrtTest[] 
                {
                    new DrtTest(TestStockEasingFunctions),
                    new DrtTest(TestToFromEasing),
                    new DrtTest(TestKeyFrameEasing),
                    new DrtTest(TestGeneratedEasingFunction),
                };
            }
            else
            {
                return new DrtTest[] { new DrtTest(TestStockEasingFunctions) };
            }

        }

        void SetupTest(double seconds)
        {
            ((ListBox)DRT.FindElementByID("easingGraphs")).ItemsSource = null;
            ((ListBox)DRT.FindElementByID("easingGraphs")).Visibility = Visibility.Collapsed;
            ((FrameworkElement)DRT.FindElementByID("animationRoot")).Visibility = Visibility.Collapsed;

            FrameworkElement progressRectangle = (FrameworkElement)DRT.FindElementByID("progressRectangle");
            if (progressRectangle != null)
            {
                DoubleAnimation progressAnimation = new DoubleAnimation(0.0, 500.0, new Duration(TimeSpan.FromSeconds(seconds)));
                progressAnimation.Completed += new EventHandler(OnProgressBarAnimationCompleted);
                progressRectangle.BeginAnimation(Rectangle.WidthProperty, progressAnimation);
            }
        }

        private void OnProgressBarAnimationCompleted(object sender, EventArgs args)
        {
            Console.WriteLine("test complete");
            if (!DRT.KeepAlive)
            {
                DRT.Resume(DispatcherPriority.Normal);
            }
        }

        void TestStockEasingFunctions()
        {
            SetupTest(6);

            var easingFuncitons = new ObservableCollection<EasingGraphData>();
            easingFuncitons.Add(new EasingGraphData("SineEase", "", new SineEase()));
            easingFuncitons.Add(new EasingGraphData("CircleEase", "", new CircleEase()));
            easingFuncitons.Add(new EasingGraphData("QuadraticEase", "", new QuadraticEase()));
            easingFuncitons.Add(new EasingGraphData("CubicEase", "", new CubicEase()));
            easingFuncitons.Add(new EasingGraphData("QuarticEase", "", new QuarticEase()));
            easingFuncitons.Add(new EasingGraphData("QuinticEase", "", new QuinticEase()));
            easingFuncitons.Add(new EasingGraphData("PowerEase", "Power = -1", new PowerEase() { Power = -1 }));
            easingFuncitons.Add(new EasingGraphData("PowerEase", "Power = 0", new PowerEase() { Power = 0 }));
            easingFuncitons.Add(new EasingGraphData("PowerEase", "Power = 50", new PowerEase() { Power = 50 }));
            easingFuncitons.Add(new EasingGraphData("PowerEase", "Power = .5", new PowerEase() { Power = .5 }));
            easingFuncitons.Add(new EasingGraphData("PowerEase", "Power = 3", new PowerEase() { Power = 3 }));
            easingFuncitons.Add(new EasingGraphData("ExponentialEase", "Factor = 5", new ExponentialEase() { Exponent = 5 }));
            easingFuncitons.Add(new EasingGraphData("ExponentialEase", "Factor = 0.1", new ExponentialEase() { Exponent = 0.1 }));
            easingFuncitons.Add(new EasingGraphData("ExponentialEase", "Factor = -5", new ExponentialEase() { Exponent = -5 }));
            easingFuncitons.Add(new EasingGraphData("BackEase", "0.75", new BackEase() { Amplitude = 0.75 }));
            easingFuncitons.Add(new EasingGraphData("BackEase", "1.0", new BackEase() { Amplitude = 1.0 }));
            easingFuncitons.Add(new EasingGraphData("BackEase", "1.5", new BackEase() { Amplitude = 1.5 }));
            easingFuncitons.Add(new EasingGraphData("BounceEase", "Bounces = 1, Bounciness = -0.5", new BounceEase() { Bounces = 1, Bounciness = -0.5 }));
            easingFuncitons.Add(new EasingGraphData("BounceEase", "Bounces = 1, Bounciness = -1", new BounceEase() { Bounces = 1, Bounciness = -1 }));
            easingFuncitons.Add(new EasingGraphData("BounceEase", "Bounces = 1, Bounciness = -2", new BounceEase() { Bounces = 1, Bounciness = -2 }));
            easingFuncitons.Add(new EasingGraphData("BounceEase", "Bounces = 1, Bounciness = 0.5", new BounceEase() { Bounces = 1, Bounciness = 0.5 }));
            easingFuncitons.Add(new EasingGraphData("BounceEase", "Bounces = 0, Bounciness = 1", new BounceEase() { Bounces = 0, Bounciness = 1 }));
            easingFuncitons.Add(new EasingGraphData("BounceEase", "Bounces = 1, Bounciness = 1", new BounceEase() { Bounces = 1, Bounciness = 1 }));
            easingFuncitons.Add(new EasingGraphData("BounceEase", "Bounces = 1, Bounciness = 1.1", new BounceEase() { Bounces = 1, Bounciness = 1.1 }));
            easingFuncitons.Add(new EasingGraphData("BounceEase", "Bounces = 3, Bounciness = 1.1", new BounceEase() { Bounces = 3, Bounciness = 1.1 }));
            easingFuncitons.Add(new EasingGraphData("BounceEase", "Bounces = 8, Bounciness = 1.1", new BounceEase() { Bounces = 8, Bounciness = 1.1 }));
            easingFuncitons.Add(new EasingGraphData("BounceEase", "Bounces = 1, Bounciness = 2", new BounceEase() { Bounces = 1, Bounciness = 2 }));
            easingFuncitons.Add(new EasingGraphData("BounceEase", "Bounces = 3, Bounciness = 2", new BounceEase() { Bounces = 3, Bounciness = 2 }));
            easingFuncitons.Add(new EasingGraphData("BounceEase", "Bounces = 8, Bounciness = 2", new BounceEase() { Bounces = 8, Bounciness = 2 }));
            easingFuncitons.Add(new EasingGraphData("BounceEase", "Bounces = 1, Bounciness = 3", new BounceEase() { Bounces = 1, Bounciness = 3 }));
            easingFuncitons.Add(new EasingGraphData("BounceEase", "Bounces = 3, Bounciness = 3", new BounceEase() { Bounces = 3, Bounciness = 3 }));
            easingFuncitons.Add(new EasingGraphData("BounceEase", "Bounces = 8, Bounciness = 3", new BounceEase() { Bounces = 8, Bounciness = 3 }));
            easingFuncitons.Add(new EasingGraphData("ElasticEase", "Oscillations = 1, Springiness = 5", new ElasticEase() { Oscillations = 1, Springiness = 5 }));
            easingFuncitons.Add(new EasingGraphData("ElasticEase", "Oscillations = 3, Springiness = 3", new ElasticEase() { Oscillations = 3, Springiness = 3 }));
            easingFuncitons.Add(new EasingGraphData("ElasticEase", "Oscillations = 5, Springiness = 1", new ElasticEase() { Oscillations = 5, Springiness = 1 }));
           
            ListBox easingGraphs = (ListBox)DRT.FindElementByID("easingGraphs");
            easingGraphs.Visibility = Visibility.Visible;
            easingGraphs.ItemsSource = easingFuncitons;

            if (!DRT.KeepAlive)
            {
                DRT.Suspend();
            }
        }

        void TestToFromEasing()
        {
            SetupTest(3.0);

            var duration = new Duration(TimeSpan.FromSeconds(3.0));
            var function = new ElasticEase() { EasingMode = EasingMode.EaseInOut };
            
            dataModel.BeginAnimation(AnimationDataModel.ByteProperty, new ByteAnimation(0, 100, duration) { EasingFunction = function, RepeatBehavior = RepeatBehavior.Forever });
            dataModel.BeginAnimation(AnimationDataModel.ColorProperty, new ColorAnimation(Colors.Black, Colors.White, duration) { EasingFunction = function, RepeatBehavior = RepeatBehavior.Forever });
            dataModel.BeginAnimation(AnimationDataModel.DecimalProperty, new DecimalAnimation(0, 100, duration) { EasingFunction = function, RepeatBehavior = RepeatBehavior.Forever });
            dataModel.BeginAnimation(AnimationDataModel.DoubleProperty, new DoubleAnimation(0, 100, duration) { EasingFunction = function, RepeatBehavior = RepeatBehavior.Forever });

            dataModel.BeginAnimation(AnimationDataModel.Int16Property, new Int16Animation(0, 100, duration) { EasingFunction = function, RepeatBehavior = RepeatBehavior.Forever });
            dataModel.BeginAnimation(AnimationDataModel.Int32Property, new Int32Animation(0, 100, duration) { EasingFunction = function, RepeatBehavior = RepeatBehavior.Forever });
            dataModel.BeginAnimation(AnimationDataModel.Int64Property, new Int64Animation(0, 100, duration) { EasingFunction = function, RepeatBehavior = RepeatBehavior.Forever });
            dataModel.BeginAnimation(AnimationDataModel.PointProperty, new PointAnimation(new Point(0,0), new Point(100, 100), duration) { EasingFunction = function, RepeatBehavior = RepeatBehavior.Forever });

            dataModel.BeginAnimation(AnimationDataModel.Point3DProperty, new Point3DAnimation(new Point3D(0, 0, 0), new Point3D(100, 100, 100), duration) { EasingFunction = function, RepeatBehavior = RepeatBehavior.Forever });
            dataModel.BeginAnimation(AnimationDataModel.QuaternionProperty, new QuaternionAnimation(Quaternion.Identity, new Quaternion(new Vector3D(1,1,1), 100), duration) { EasingFunction = function, RepeatBehavior = RepeatBehavior.Forever });
            dataModel.BeginAnimation(AnimationDataModel.Rotation3DProperty, new Rotation3DAnimation(new QuaternionRotation3D(Quaternion.Identity), new QuaternionRotation3D(new Quaternion(new Vector3D(1,1,1), 100)), duration) { EasingFunction = function, RepeatBehavior = RepeatBehavior.Forever });
            dataModel.BeginAnimation(AnimationDataModel.RectProperty, new RectAnimation(new Rect(0, 0, 100, 100), new Rect(100, 100, 200, 200), duration) { EasingFunction = function, RepeatBehavior = RepeatBehavior.Forever });

            dataModel.BeginAnimation(AnimationDataModel.SingleProperty, new SingleAnimation(0, 100, duration) { EasingFunction = function, RepeatBehavior = RepeatBehavior.Forever });
            dataModel.BeginAnimation(AnimationDataModel.SizeProperty, new SizeAnimation(new Size(0, 0), new Size(100, 100), duration) { EasingFunction = function, RepeatBehavior = RepeatBehavior.Forever });
            dataModel.BeginAnimation(AnimationDataModel.VectorProperty, new VectorAnimation(new Vector(0, 0), new Vector(100, 100), duration) { EasingFunction = function, RepeatBehavior = RepeatBehavior.Forever });
            dataModel.BeginAnimation(AnimationDataModel.Vector3DProperty, new Vector3DAnimation(new Vector3D(0, 0, 0), new Vector3D(100, 100, 100), duration) { EasingFunction = function, RepeatBehavior = RepeatBehavior.Forever });

            dataModel.BeginAnimation(AnimationDataModel.ThicknessProperty, new ThicknessAnimation(new Thickness(1), new Thickness(10), duration) { EasingFunction = function, RepeatBehavior = RepeatBehavior.Forever });

            FrameworkElement animationRoot = (FrameworkElement)DRT.FindElementByID("animationRoot");
            animationRoot.Visibility = Visibility.Visible;
            DRT.Suspend();
        }

        void TestKeyFrameEasing()
        {
            SetupTest(3.0);

            var duration = new Duration(TimeSpan.FromSeconds(3.0));
            var function = new ElasticEase() { EasingMode = EasingMode.EaseInOut };

            dataModel.BeginAnimation(AnimationDataModel.ByteProperty, new ByteAnimationUsingKeyFrames() { KeyFrames = new ByteKeyFrameCollection() { new EasingByteKeyFrame(0, KeyTime.FromPercent(0), function), new EasingByteKeyFrame(100, KeyTime.FromPercent(0.5), function), new EasingByteKeyFrame(0, KeyTime.FromPercent(1.0), function) }, Duration = duration, RepeatBehavior = RepeatBehavior.Forever });
            dataModel.BeginAnimation(AnimationDataModel.ColorProperty, new ColorAnimationUsingKeyFrames() { KeyFrames = new ColorKeyFrameCollection() { new EasingColorKeyFrame(Colors.Black, KeyTime.FromPercent(0), function), new EasingColorKeyFrame(Colors.White, KeyTime.FromPercent(0.5), function), new EasingColorKeyFrame(Colors.CornflowerBlue, KeyTime.FromPercent(1.0), function) }, Duration = duration, RepeatBehavior = RepeatBehavior.Forever });
            dataModel.BeginAnimation(AnimationDataModel.DecimalProperty, new DecimalAnimationUsingKeyFrames() { KeyFrames = new DecimalKeyFrameCollection() { new EasingDecimalKeyFrame(0, KeyTime.FromPercent(0), function), new EasingDecimalKeyFrame(100, KeyTime.FromPercent(0.5), function), new EasingDecimalKeyFrame(0, KeyTime.FromPercent(1.0), function) }, Duration = duration, RepeatBehavior = RepeatBehavior.Forever });
            dataModel.BeginAnimation(AnimationDataModel.DoubleProperty, new DoubleAnimationUsingKeyFrames() { KeyFrames = new DoubleKeyFrameCollection() { new EasingDoubleKeyFrame(0, KeyTime.FromPercent(0), function), new EasingDoubleKeyFrame(100, KeyTime.FromPercent(0.5), function), new EasingDoubleKeyFrame(0, KeyTime.FromPercent(1.0), function) }, Duration = duration, RepeatBehavior = RepeatBehavior.Forever });

            dataModel.BeginAnimation(AnimationDataModel.Int16Property, new Int16AnimationUsingKeyFrames() { KeyFrames = new Int16KeyFrameCollection() { new EasingInt16KeyFrame(0, KeyTime.FromPercent(0), function), new EasingInt16KeyFrame(100, KeyTime.FromPercent(0.5), function), new EasingInt16KeyFrame(0, KeyTime.FromPercent(1.0), function) }, Duration = duration, RepeatBehavior = RepeatBehavior.Forever });
            dataModel.BeginAnimation(AnimationDataModel.Int32Property, new Int32AnimationUsingKeyFrames() { KeyFrames = new Int32KeyFrameCollection() { new EasingInt32KeyFrame(0, KeyTime.FromPercent(0), function), new EasingInt32KeyFrame(100, KeyTime.FromPercent(0.5), function), new EasingInt32KeyFrame(0, KeyTime.FromPercent(1.0), function) }, Duration = duration, RepeatBehavior = RepeatBehavior.Forever });
            dataModel.BeginAnimation(AnimationDataModel.Int64Property, new Int64AnimationUsingKeyFrames() { KeyFrames = new Int64KeyFrameCollection() { new EasingInt64KeyFrame(0, KeyTime.FromPercent(0), function), new EasingInt64KeyFrame(100, KeyTime.FromPercent(0.5), function), new EasingInt64KeyFrame(0, KeyTime.FromPercent(1.0), function) }, Duration = duration, RepeatBehavior = RepeatBehavior.Forever });
            dataModel.BeginAnimation(AnimationDataModel.PointProperty, new PointAnimationUsingKeyFrames() { KeyFrames = new PointKeyFrameCollection() { new EasingPointKeyFrame(new Point(0, 0), KeyTime.FromPercent(0), function), new EasingPointKeyFrame(new Point(100, 100), KeyTime.FromPercent(0.5), function), new EasingPointKeyFrame(new Point(0, 0), KeyTime.FromPercent(1.0), function) }, Duration = duration, RepeatBehavior = RepeatBehavior.Forever });

            dataModel.BeginAnimation(AnimationDataModel.Point3DProperty, new Point3DAnimationUsingKeyFrames() { KeyFrames = new Point3DKeyFrameCollection() { new EasingPoint3DKeyFrame(new Point3D(0, 0, 0), KeyTime.FromPercent(0), function), new EasingPoint3DKeyFrame(new Point3D(100, 100, 100), KeyTime.FromPercent(0.5), function), new EasingPoint3DKeyFrame(new Point3D(0, 0, 0), KeyTime.FromPercent(1.0), function) }, Duration = duration, RepeatBehavior = RepeatBehavior.Forever });
            dataModel.BeginAnimation(AnimationDataModel.QuaternionProperty, new QuaternionAnimationUsingKeyFrames() { KeyFrames = new QuaternionKeyFrameCollection() { new EasingQuaternionKeyFrame(Quaternion.Identity, KeyTime.FromPercent(0), function), new EasingQuaternionKeyFrame(new Quaternion(new Vector3D(1, 1, 1), 100), KeyTime.FromPercent(0.5), function), new EasingQuaternionKeyFrame(Quaternion.Identity, KeyTime.FromPercent(1.0), function) }, Duration = duration, RepeatBehavior = RepeatBehavior.Forever });
            dataModel.BeginAnimation(AnimationDataModel.Rotation3DProperty, new Rotation3DAnimationUsingKeyFrames() { KeyFrames = new Rotation3DKeyFrameCollection() { new EasingRotation3DKeyFrame(new QuaternionRotation3D(Quaternion.Identity), KeyTime.FromPercent(0), function), new EasingRotation3DKeyFrame(new QuaternionRotation3D(new Quaternion(new Vector3D(1, 1, 1), 100)), KeyTime.FromPercent(0.5), function), new EasingRotation3DKeyFrame(new QuaternionRotation3D(Quaternion.Identity), KeyTime.FromPercent(1.0), function) }, Duration = duration, RepeatBehavior = RepeatBehavior.Forever });
            dataModel.BeginAnimation(AnimationDataModel.RectProperty, new RectAnimationUsingKeyFrames() { KeyFrames = new RectKeyFrameCollection() { new EasingRectKeyFrame(new Rect(0, 0, 100, 100), KeyTime.FromPercent(0), function), new EasingRectKeyFrame(new Rect(100, 100, 200, 200), KeyTime.FromPercent(0.5), function), new EasingRectKeyFrame(new Rect(0, 0, 100, 100), KeyTime.FromPercent(1.0), function) }, Duration = duration, RepeatBehavior = RepeatBehavior.Forever });

            dataModel.BeginAnimation(AnimationDataModel.SingleProperty, new SingleAnimationUsingKeyFrames() { KeyFrames = new SingleKeyFrameCollection() { new EasingSingleKeyFrame(0, KeyTime.FromPercent(0), function), new EasingSingleKeyFrame(100, KeyTime.FromPercent(0.5), function), new EasingSingleKeyFrame(0, KeyTime.FromPercent(1.0), function) }, Duration = duration, RepeatBehavior = RepeatBehavior.Forever });
            dataModel.BeginAnimation(AnimationDataModel.SizeProperty, new SizeAnimationUsingKeyFrames() { KeyFrames = new SizeKeyFrameCollection() { new EasingSizeKeyFrame(new Size(0, 0), KeyTime.FromPercent(0), function), new EasingSizeKeyFrame(new Size(100, 100), KeyTime.FromPercent(0.5), function), new EasingSizeKeyFrame(new Size(0, 0), KeyTime.FromPercent(1.0), function) }, Duration = duration, RepeatBehavior = RepeatBehavior.Forever });
            dataModel.BeginAnimation(AnimationDataModel.VectorProperty, new VectorAnimationUsingKeyFrames() { KeyFrames = new VectorKeyFrameCollection() { new EasingVectorKeyFrame(new Vector(0, 0), KeyTime.FromPercent(0), function), new EasingVectorKeyFrame(new Vector(100, 100), KeyTime.FromPercent(0.5), function), new EasingVectorKeyFrame(new Vector(0, 0), KeyTime.FromPercent(1.0), function) }, Duration = duration, RepeatBehavior = RepeatBehavior.Forever });
            dataModel.BeginAnimation(AnimationDataModel.Vector3DProperty, new Vector3DAnimationUsingKeyFrames() { KeyFrames = new Vector3DKeyFrameCollection() { new EasingVector3DKeyFrame(new Vector3D(0, 0, 0), KeyTime.FromPercent(0), function), new EasingVector3DKeyFrame(new Vector3D(100, 100, 100), KeyTime.FromPercent(0.5), function), new EasingVector3DKeyFrame(new Vector3D(0, 0, 0), KeyTime.FromPercent(1.0), function) }, Duration = duration, RepeatBehavior = RepeatBehavior.Forever });

            dataModel.BeginAnimation(AnimationDataModel.ThicknessProperty, new ThicknessAnimationUsingKeyFrames() { KeyFrames = new ThicknessKeyFrameCollection() { new EasingThicknessKeyFrame(new Thickness(1), KeyTime.FromPercent(0), function), new EasingThicknessKeyFrame(new Thickness(10), KeyTime.FromPercent(0.5), function), new EasingThicknessKeyFrame(new Thickness(1), KeyTime.FromPercent(1.0), function) }, Duration = duration, RepeatBehavior = RepeatBehavior.Forever });

            FrameworkElement animationRoot = (FrameworkElement)DRT.FindElementByID("animationRoot");
            animationRoot.Visibility = Visibility.Visible;
            DRT.Suspend();
        }

        void TestGeneratedEasingFunction()
        {
            SetupTest(2);

            FrameworkElement animationRoot = (FrameworkElement)DRT.FindElementByID("animationRoot");
            animationRoot.Visibility = Visibility.Visible;
            DRT.Suspend();
        }

        private void OnProgressBarAnimationCurrentStateInvalidated(object sender, EventArgs args)
        {
            Clock clock = (Clock)sender;

            if (clock.CurrentState != ClockState.Active)
            {
                if (!DRT.KeepAlive)
                {
                    DRT.Resume();
                }
            }
        }
    }
}
