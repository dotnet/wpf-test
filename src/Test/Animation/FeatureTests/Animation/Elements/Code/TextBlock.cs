// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/****************  TextBlock Integration Tests *****************
*     This file supports testing of animations in a particular way:  a Markup file is dynamically
*     loaded and an Animation associated with an element in the Markup, in this case a TextBlock
*     Element.  The Animation is then verified.
*     Major Actions:
*          (a) Create a new Navigation window and load the .xaml file via OnStartup.
*          (b) Create an Animation on the Element specified in the Markup.  Start it.
*          (c) During the Animation, call VScan routines for verification.
*     Pass Conditions:
*          The test passes if (a) the animation's GetValue() API returns the correct value, and
*          (b) the actual rendering matches the expected rendering during the course of the animation.
*     How verified:
*          The SideBySideVerifier routine is called to (a) verify values during the animation, and
*          (b) verify rendering as well using VScan to compare the expected vs. actual bitmaps at
*          specified intervals during the animation (controlled by ClockManager).
*
*     Framework:          A CLR executable is created.
*     Area:               Animation & Timing
*     Dependencies:       TestRuntime.dll
*     Support Files:      .xaml files [must be available at run time]
**********************************************************/
using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
     
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.Elements.Controls</area>
    /// <priority>2</priority>
    /// <description>
    /// Full verification of animation of TextBlock Dependency Properties
    /// </description>
    /// </summary>
    [Test(2, "Animation.Elements.Controls", "TextBlockTest", SupportFiles=@"FeatureTests\Animation\avalon.png")]

    public class TextBlockTest : XamlTest
    {
        #region Test case members
        
        private ClockManager                 _clockManager;
        private SideBySideVerifier           _sideBySide;
        private string                       _inputString = "";

        private TextBlock                    _textBlock1      = null;
        private SolidColorBrush              _SCB             = null;
        private AnimationClock               _clock1          = null;
        private AnimationClock               _clock2          = null;
        private string                       _windowTitle     = "TextBlock Animation";
        private TimeSpan                     _BEGIN_TIME      = new TimeSpan(0,0,0,0,3000);
        private TimeSpan                     _DURATION_TIME   = new TimeSpan(0,0,0,0,3000);
        private bool                         _testPassed      = true;

        AnimationValidator _myValidator = new AnimationValidator();

        #endregion


        #region Constructor
        
        [Variation(@"BaselineOffset.xaml", Priority=0)]
        [Variation(@"ClipCenter.xaml", Priority=0)]
        [Variation(@"ClipRect.xaml")]
        // [DISABLE WHILE PORTING]
        // [Variation(@"ColorFore.xaml", Priority=0)]
        // [Variation(@"ColorBack.xaml", Priority=0)]
        [Variation(@"FontSize.xaml")]
        // [DISABLE WHILE PORTING]
        // [Variation(@"Gradients2.xaml", Priority=0)]
        // [Variation(@"GradientStopColor.xaml")]
        [Variation(@"GradientStopOffset.xaml")]
        [Variation(@"Height.xaml")]
        [Variation(@"IsEnabled.xaml", Disabled=true)]
        [Variation(@"Left.xaml")]
        [Variation(@"LineHeight.xaml")]
        [Variation(@"Opacity.xaml", Priority=0)]
        [Variation(@"Opacity2.xaml")]
        [Variation(@"OpacitySCB.xaml", Priority=0)]
        [Variation(@"RadialGradientOrigin.xaml")]
        [Variation(@"RadialGradientRadiusX.xaml")]
        [Variation(@"Rotate.xaml")]
        [Variation(@"RotateRelative.xaml")]
        [Variation(@"Scale.xaml", Priority=1)]
        [Variation(@"Skew.xaml", Priority=0)]
        [Variation(@"Text.xaml")]
        [Variation(@"Thickness.xaml")]
        [Variation(@"Top.xaml")]
        [Variation(@"Translate.xaml")]
        [Variation(@"TranslateLeft.xaml", Priority=0)]
        [Variation(@"VisualRect.xaml")]
        [Variation(@"Width.xaml")]

        /******************************************************************************
        * Function:          TextBlockTest Constructor
        ******************************************************************************/
        public TextBlockTest(string testValue): base(@"TextBlock.xaml")
        {
            int indx = testValue.LastIndexOf(".");
            if (indx >= 0)
                testValue = testValue.Remove(indx);         //Remove the .xaml suffix.

            _inputString = testValue;

            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(Animate);
            RunSteps += new TestStep(Verify);
        }
        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Initialize.
        /// </summary>
        /// <returns></returns>
        TestResult Initialize()
        {
            GlobalLog.LogEvidence("---Initializing---");
            
            WaitForPriority(DispatcherPriority.SystemIdle);

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          Animate
        ******************************************************************************/
        /// <summary>
        /// Starting the Animation here.
        /// </summary>
        /// <returns></returns>
        TestResult Animate()
        {
            GlobalLog.LogEvidence("---Animate---");
            
            Window.Title        = _windowTitle;
            Window.Height       = 350d;
            Window.Width        = 500d;
            _textBlock1 = (TextBlock)LogicalTreeHelper.FindLogicalNode((DependencyObject)Window.Content,"Animate");

            //Set up the Clock Manager to control the Clock. This pauses the Root timeline.
            int[] times = new int[]{2875,3525,4000,5000,5875,6525};
            _clockManager = new ClockManager(times);
            ClockManager.Ticked += new TickEvent(OnTimeTicked);
            GlobalLog.LogEvidence("---Clock Manager set up---");

            if (_textBlock1 == null)
            {
                GlobalLog.LogEvidence("ERROR!!! Animate: Element not found in Markup.");
                return TestResult.Fail;
            }
            else
            {
                //Create an instance of the SideBySideVerifier, passing the window.
                _sideBySide = new SideBySideVerifier(Window);

                CreateAnimation();
                
                GlobalLog.LogEvidence("---Animated Added---");
                _clockManager.hostManager.Resume();
                GlobalLog.LogEvidence("---Root Timeline Resumed---");
                
                return TestResult.Pass;
            }
        }
          
        /******************************************************************************
        * Function:          CreateAnimation
        ******************************************************************************/
        /// <summary>
        /// Establish an Animation.
        /// </summary>
        /// <returns></returns>
        public void CreateAnimation()
        {
            try
            {
                switch (_inputString.ToLower(CultureInfo.InvariantCulture))
                {
                    case "baselineoffset":
                        _textBlock1.Background     = Brushes.MistyRose;
                        _textBlock1.BaselineOffset = 0;

                        DoubleAnimation animBaseline = new DoubleAnimation();
                        animBaseline.To             = 50;
                        animBaseline.BeginTime      = _BEGIN_TIME;
                        animBaseline.Duration       = new Duration(_DURATION_TIME);
                        _clock1 = animBaseline.CreateClock();
                        _textBlock1.ApplyAnimationClock(TextBlock.BaselineOffsetProperty, _clock1);

                        //Register an animation for verification, passing the animated DO and DP.
                        _sideBySide.RegisterAnimation(_textBlock1, TextBlock.BaselineOffsetProperty, _clock1);
                        break;
                    case "clipcenter":
                        _textBlock1.Background   = Brushes.MistyRose;

                        PointAnimation animClipCenter = new PointAnimation();
                        animClipCenter.To             = new Point(150,150);
                        animClipCenter.BeginTime      = _BEGIN_TIME;
                        animClipCenter.Duration       = new Duration(_DURATION_TIME);

                        EllipseGeometry ellipseGeo = new EllipseGeometry();
                        ellipseGeo.RadiusX               = 50d;
                        ellipseGeo.RadiusY               = 50d;
                        ellipseGeo.Center                = new Point(5,5);

                        _clock1 = animClipCenter.CreateClock();
                        ellipseGeo.ApplyAnimationClock(EllipseGeometry.CenterProperty, _clock1);

                        _textBlock1.Clip = ellipseGeo;

                        //Register an animation for verification, passing the animated DO and DP.
                        _sideBySide.RegisterAnimation(ellipseGeo, EllipseGeometry.CenterProperty, _clock1);
                        break;
                    case "cliprect":
                        _textBlock1.Background   = Brushes.Navy;

                        RectAnimation animClipRect = new RectAnimation();
                        animClipRect.To             = new Rect(0,0,80,80);
                        animClipRect.BeginTime      = _BEGIN_TIME;
                        animClipRect.Duration       = new Duration(_DURATION_TIME);

                        RectangleGeometry rectGeometry = new RectangleGeometry();
                        rectGeometry.Rect       = new Rect(0,0,0,0);

                        _clock1 = animClipRect.CreateClock();
                        rectGeometry.ApplyAnimationClock(RectangleGeometry.RectProperty, _clock1);

                        _textBlock1.Clip = rectGeometry;

                        //Register an animation for verification, passing the animated DO and DP.
                        _sideBySide.RegisterAnimation(rectGeometry, RectangleGeometry.RectProperty, _clock1);
                        break;
                    case "colorfore":
                        //ColorAnimation is carried out on the Foreground property of the TextBlock element.
                        ColorAnimation animColor1 = new ColorAnimation();
                        animColor1.From          = Colors.Orange;
                        animColor1.To            = Colors.HotPink;
                        animColor1.BeginTime     = _BEGIN_TIME;
                        animColor1.Duration      = new Duration(_DURATION_TIME);

                        _SCB = new SolidColorBrush();
                        _SCB.Color = Colors.Orange;

                        _clock1 = animColor1.CreateClock();
                        _SCB.ApplyAnimationClock(SolidColorBrush.ColorProperty, _clock1);

                        _textBlock1.Foreground = _SCB;

                        //Register an animation for verification, passing the animated DO and DP.
                        _sideBySide.RegisterAnimation(_SCB, SolidColorBrush.ColorProperty, _clock1);
                        break;
                    case "colorback":
                        //ColorAnimation is carried out on the Background property of the TextBlock element.
                        ColorAnimation animColor2 = new ColorAnimation();
                        animColor2.From          = Colors.White;
                        animColor2.To            = Colors.Black;
                        animColor2.BeginTime     = _BEGIN_TIME;
                        animColor2.Duration      = new Duration(_DURATION_TIME);

                        _SCB = new SolidColorBrush();
                        _SCB.Color = Colors.White;

                        _clock1 = animColor2.CreateClock();
                        _SCB.ApplyAnimationClock(SolidColorBrush.ColorProperty, _clock1);

                        _textBlock1.Background = _SCB;

                        //Register an animation for verification, passing the animated DO and DP.
                        _sideBySide.RegisterAnimation(_SCB, SolidColorBrush.ColorProperty, _clock1);
                        break;
                    case "fontsize":
                        DoubleAnimation animFontSize = new DoubleAnimation();
                        animFontSize.By             = 58d;
                        animFontSize.BeginTime      = _BEGIN_TIME;
                        animFontSize.Duration       = new Duration(_DURATION_TIME);
                        _clock1 = animFontSize.CreateClock();
                        _textBlock1.ApplyAnimationClock(TextBlock.FontSizeProperty, _clock1);

                        //Register an animation for verification, passing the animated DO and DP.
                        _sideBySide.RegisterAnimation(_textBlock1, TextBlock.FontSizeProperty, _clock1);
                        break;
                    case "gradients2":
                        ColorAnimation animG2Color = new ColorAnimation();
                        animG2Color.From          = Colors.LightGreen;
                        animG2Color.To            = Colors.Moccasin;
                        animG2Color.BeginTime     = _BEGIN_TIME;
                        animG2Color.Duration      = new Duration(_DURATION_TIME);

                        DoubleAnimation animG2Offset = new DoubleAnimation();                                             
                        animG2Offset.BeginTime          = _BEGIN_TIME;
                        animG2Offset.Duration           = new Duration(_DURATION_TIME);
                        animG2Offset.From               = 0.9d;
                        animG2Offset.To                 = 0.2d;

                        LinearGradientBrush LGB = new LinearGradientBrush();
                        LGB.StartPoint     = new Point(0.0, 0.0);
                        LGB.EndPoint       = new Point(1.0, 1.0);
                        LGB.MappingMode    = BrushMappingMode.RelativeToBoundingBox;

                        GradientStop gs1 = new GradientStop(Colors.LightBlue, 0.0);
                        GradientStop gs2 = new GradientStop(Colors.LightGreen, 0.4);
                        GradientStop gs3 = new GradientStop(Colors.Green, 0.9);
                        GradientStopCollection GSC = new GradientStopCollection();
                        GSC.Add(gs1);
                        GSC.Add(gs2);
                        GSC.Add(gs3);

                        _clock1 = animG2Color.CreateClock();
                        GSC[1].ApplyAnimationClock(GradientStop.ColorProperty, _clock1);
                        _clock2 = animG2Offset.CreateClock();
                        GSC[1].ApplyAnimationClock(GradientStop.OffsetProperty, _clock2);

                        LGB.GradientStops = GSC;
                        _textBlock1.Background = LGB;

                        //Register an animation for verification, passing the animated DO and DP.
                        _sideBySide.RegisterAnimation(GSC[1], GradientStop.ColorProperty, _clock1);
                        _sideBySide.RegisterAnimation(GSC[1], GradientStop.OffsetProperty, _clock2);

                        break;
                    case "gradientstopcolor":
                        ColorAnimation animGSColor = new ColorAnimation();
                        animGSColor.From          = Colors.LightGreen;
                        animGSColor.To            = Colors.Moccasin;
                        animGSColor.BeginTime     = _BEGIN_TIME;
                        animGSColor.Duration      = new Duration(_DURATION_TIME);

                        LinearGradientBrush colorLGB = new LinearGradientBrush();
                        colorLGB.StartPoint     = new Point(0.0, 0.0);
                        colorLGB.EndPoint       = new Point(1.0, 1.0);
                        colorLGB.MappingMode    = BrushMappingMode.RelativeToBoundingBox;

                        GradientStop colorGS1 = new GradientStop(Colors.LightBlue, 0.0);
                        GradientStop colorGS2 = new GradientStop(Colors.LightGreen, 0.4);
                        GradientStop colorGS3 = new GradientStop(Colors.Green, 0.9);
                        GradientStopCollection colorGSC = new GradientStopCollection();
                        colorGSC.Add(colorGS1);
                        colorGSC.Add(colorGS2);
                        colorGSC.Add(colorGS3);

                        _clock1 = animGSColor.CreateClock();
                        colorGSC[1].ApplyAnimationClock(GradientStop.ColorProperty, _clock1);

                        colorLGB.GradientStops = colorGSC;
                        _textBlock1.Background = colorLGB;

                        //Register an animation for verification, passing the animated DO and DP.
                        _sideBySide.RegisterAnimation(colorGSC[1], GradientStop.ColorProperty, _clock1);

                        break;
                    case "gradientstopoffset":
                        DoubleAnimation animGSOffset = new DoubleAnimation();                                             
                        animGSOffset.BeginTime          = _BEGIN_TIME;
                        animGSOffset.Duration           = new Duration(_DURATION_TIME);
                        animGSOffset.From               = 0.9d;
                        animGSOffset.To                 = 0.2d;

                        LinearGradientBrush offsetLGB = new LinearGradientBrush();
                        offsetLGB.StartPoint = new Point(0.0, 0.0);
                        offsetLGB.EndPoint   = new Point(1.0, 1.0);
                        offsetLGB.MappingMode = BrushMappingMode.RelativeToBoundingBox;

                        GradientStop offsetGS1 = new GradientStop(Colors.RoyalBlue, 0.0);
                        GradientStop offsetGS2 = new GradientStop(Colors.SteelBlue, 0.2);
                        GradientStop offsetGS3 = new GradientStop(Colors.LightBlue, 1.0);
                        GradientStopCollection offsetGSC = new GradientStopCollection();
                        offsetGSC.Add(offsetGS1);
                        offsetGSC.Add(offsetGS2);
                        offsetGSC.Add(offsetGS3);

                        _clock1 = animGSOffset.CreateClock();
                        offsetGSC[1].ApplyAnimationClock(GradientStop.OffsetProperty, _clock1);

                        offsetLGB.GradientStops = offsetGSC;
                        _textBlock1.Background = offsetLGB;

                        //Register an animation for verification, passing the animated DO and DP.
                        _sideBySide.RegisterAnimation(offsetGSC[1], GradientStop.OffsetProperty, _clock1);
                        break;
                    case "height":
                        DoubleAnimation animHeight = new DoubleAnimation();
                        animHeight.By             = 120d;
                        animHeight.BeginTime      = _BEGIN_TIME;
                        animHeight.Duration       = new Duration(_DURATION_TIME);
                        _clock1 = animHeight.CreateClock();
                        _textBlock1.ApplyAnimationClock(FrameworkElement.HeightProperty, _clock1);

                        //Register an animation for verification, passing the animated DO and DP.
                        _sideBySide.RegisterAnimation(_textBlock1, FrameworkElement.HeightProperty, _clock1);
                        break;
                    case "isenabled":
                        BooleanAnimationUsingKeyFrames animIsEnabled = new BooleanAnimationUsingKeyFrames();

                        BooleanKeyFrameCollection BKFC2 = new BooleanKeyFrameCollection();
                        BKFC2.Add(new DiscreteBooleanKeyFrame(false,KeyTime.FromPercent(0f)));
                        BKFC2.Add(new DiscreteBooleanKeyFrame(true,KeyTime.FromPercent(0.5f)));
                        BKFC2.Add(new DiscreteBooleanKeyFrame(false, KeyTime.FromPercent(1.0f)));
                        animIsEnabled.KeyFrames = BKFC2;

                        animIsEnabled.BeginTime     = _BEGIN_TIME;
                        animIsEnabled.Duration      = new Duration(_DURATION_TIME);

                        _clock1 = animIsEnabled.CreateClock();
                        _textBlock1.IsEnabled = true;
                        _textBlock1.ApplyAnimationClock(UIElement.IsEnabledProperty, _clock1);

                        //Register an animation for verification, passing the animated DO and DP.
                        _sideBySide.RegisterAnimation(_textBlock1, UIElement.IsEnabledProperty, _clock1);
                        break;
                    case "left":
                        DoubleAnimation animLeft = new DoubleAnimation();
                        animLeft.From           = 50d;
                        animLeft.To             = 0d;
                        animLeft.BeginTime      = _BEGIN_TIME;
                        animLeft.Duration       = new Duration(_DURATION_TIME);
                        _clock1 = animLeft.CreateClock();
                        _textBlock1.ApplyAnimationClock(Canvas.LeftProperty, _clock1);

                        //Register an animation for verification, passing the animated DO and DP.
                        _sideBySide.RegisterAnimation(_textBlock1, Canvas.LeftProperty, _clock1);
                        break;
                    case "lineheight":
                        _textBlock1.Text             = "LineHeight\nLineHeight";
                        _textBlock1.FontSize         = 24;
                        _textBlock1.LineHeight       = 100;

                        DoubleAnimation animLineHeight = new DoubleAnimation();
                        animLineHeight.From         = 100d;
                        animLineHeight.To           = 1d;
                        animLineHeight.BeginTime    = _BEGIN_TIME;
                        animLineHeight.Duration     = new Duration(_DURATION_TIME);
                        animLineHeight.FillBehavior = FillBehavior.Stop;

                        _clock1 = animLineHeight.CreateClock();
                        _textBlock1.ApplyAnimationClock(TextBlock.LineHeightProperty, _clock1);

                        //Register an animation for verification, passing the animated DO and DP.
                        _sideBySide.RegisterAnimation(_textBlock1, TextBlock.LineHeightProperty, _clock1);
                        break;
                    case "opacity":
                        DoubleAnimation animOpacity = new DoubleAnimation();
                        animOpacity.From        = 0;
                        animOpacity.To          = .5;
                        animOpacity.BeginTime   = _BEGIN_TIME;
                        animOpacity.Duration    = new Duration(_DURATION_TIME);

                        _clock1 = animOpacity.CreateClock();
                        _textBlock1.ApplyAnimationClock(UIElement.OpacityProperty, _clock1);

                        //Register an animation for verification, passing the animated DO and DP.
                        _sideBySide.RegisterAnimation(_textBlock1, UIElement.OpacityProperty, _clock1);
                        break;
                    case "opacity2":

                        _textBlock1.FontSize     = 96d;
                        _textBlock1.Background   = Brushes.White;
                        _textBlock1.Foreground   = Brushes.Red;
                        _textBlock1.FontFamily   = new FontFamily("Trebuchet MS");
                        _textBlock1.Text         = "*******";

                        DoubleAnimation animOpacity2 = new DoubleAnimation();
                        animOpacity2.From        = 0;
                        animOpacity2.To          = 1;
                        animOpacity2.BeginTime   = _BEGIN_TIME;
                        animOpacity2.Duration    = new Duration(_DURATION_TIME);

                        _clock1 = animOpacity2.CreateClock();
                        _textBlock1.ApplyAnimationClock(UIElement.OpacityProperty, _clock1);

                        //Register an animation for verification, passing the animated DO and DP.
                        _sideBySide.RegisterAnimation(_textBlock1, UIElement.OpacityProperty, _clock1);
                        break;
                    case "opacityscb":
                        DoubleAnimation animOpacitySCB  = new DoubleAnimation();
                        animOpacitySCB.From             = 1;
                        animOpacitySCB.To               = 0;
                        animOpacitySCB.BeginTime        = _BEGIN_TIME;
                        animOpacitySCB.Duration         = new Duration(_DURATION_TIME);

                        _SCB = new SolidColorBrush();
                        _SCB.Color = Colors.DarkOrchid;

                        _clock1 = animOpacitySCB.CreateClock();
                        _SCB.ApplyAnimationClock(SolidColorBrush.OpacityProperty, _clock1);

                        _textBlock1.Foreground = _SCB;

                        //Register an animation for verification, passing the animated DO and DP.
                        _sideBySide.RegisterAnimation(_SCB, SolidColorBrush.OpacityProperty, _clock1);
                        break;
                    case "radialgradientorigin":
                        _textBlock1.Background = Brushes.White; 
                        RadialGradientBrush RGB1 = new RadialGradientBrush();
                        RGB1.GradientStops.Add(new GradientStop(Colors.LightBlue, 0.0));
                        RGB1.GradientStops.Add(new GradientStop(Colors.Blue, 0.4));
                        RGB1.GradientStops.Add(new GradientStop(Colors.LightGreen, 0.8));
                        //RGB1.Center          = new Point(0.5, 0.5);
                        //RGB1.RadiusX         = 0.3;
                        //RGB1.RadiusY         = 0.4;
                        RGB1.GradientOrigin  = new Point(0.0, 0.0);

                        PointAnimation animGradientOrigin = new PointAnimation();
                        animGradientOrigin.To               = new Point(0.8, 0.8);
                        animGradientOrigin.BeginTime        = _BEGIN_TIME;
                        animGradientOrigin.Duration         = new Duration(_DURATION_TIME);

                        _clock1 = animGradientOrigin.CreateClock();
                        RGB1.ApplyAnimationClock(RadialGradientBrush.GradientOriginProperty, _clock1);

                        _textBlock1.Background = RGB1; 

                        //Register an animation for verification, passing the animated DO and DP.
                        _sideBySide.RegisterAnimation(RGB1, RadialGradientBrush.GradientOriginProperty, _clock1);
                        break;
                    case "radialgradientradiusx":
                        RadialGradientBrush RGB2 = new RadialGradientBrush();
                        RGB2.GradientStops.Add(new GradientStop(Colors.MistyRose, 0.0));
                        RGB2.GradientStops.Add(new GradientStop(Colors.HotPink, 1.0));
                        RGB2.Center          = new Point(0.5, 0.5);
                        RGB2.RadiusX         = 1d;
                        RGB2.RadiusY         = 1d;
                        RGB2.GradientOrigin  = new Point(0.0, 0.0);

                        DoubleAnimation animGradientRadiusX = new DoubleAnimation();
                        animGradientRadiusX.To               = 5d;
                        animGradientRadiusX.BeginTime        = _BEGIN_TIME;
                        animGradientRadiusX.Duration         = new Duration(_DURATION_TIME);

                        _clock1 = animGradientRadiusX.CreateClock();
                        RGB2.ApplyAnimationClock(RadialGradientBrush.RadiusXProperty, _clock1);

                        _textBlock1.Background = RGB2; 

                        //Register an animation for verification, passing the animated DO and DP.
                        _sideBySide.RegisterAnimation(RGB2, RadialGradientBrush.RadiusXProperty, _clock1);
                        break;
                    case "rotate":
                        DoubleAnimation animRotate1  = new DoubleAnimation();                                             
                        animRotate1.BeginTime           = _BEGIN_TIME;
                        animRotate1.Duration            = new Duration(_DURATION_TIME);
                        animRotate1.From                = 0;
                        animRotate1.To                  = 170;

                        RotateTransform rotateTransform1 = new RotateTransform();
                        rotateTransform1.Angle   = 0.0f;
                        rotateTransform1.CenterX  = 0;
                        rotateTransform1.CenterY  = 0;

                        _clock1 = animRotate1.CreateClock();
                        rotateTransform1.ApplyAnimationClock(RotateTransform.AngleProperty, _clock1);

                        _textBlock1.RenderTransform = rotateTransform1;

                        //Register an animation for verification, passing the animated DO and DP.
                        _sideBySide.RegisterAnimation(rotateTransform1, RotateTransform.AngleProperty, _clock1);
                        break;
                    case "rotaterelative":
                        Uri bitmapUri = new Uri(@"Avalon.png", UriKind.RelativeOrAbsolute);
                        BitmapImage image = null;
                        image = new BitmapImage(bitmapUri);
                        ImageBrush imageBrush = new ImageBrush(image);
                        imageBrush.Stretch = Stretch.Fill;

                        DoubleAnimation animRotate2  = new DoubleAnimation();                                             
                        animRotate2.BeginTime           = _BEGIN_TIME;
                        animRotate2.Duration            = new Duration(_DURATION_TIME);
                        animRotate2.From                = 0;
                        animRotate2.To                  = 360;

                        RotateTransform rotateTransform2 = new RotateTransform();
                        rotateTransform2.Angle   = 0.0f;
                        rotateTransform2.CenterX  = 20;
                        rotateTransform2.CenterY  = 25;

                        _clock1 = animRotate2.CreateClock();
                        rotateTransform2.ApplyAnimationClock(RotateTransform.AngleProperty, _clock1);

                        imageBrush.RelativeTransform = rotateTransform2;
                        _textBlock1.Foreground = imageBrush;

                        //Register an animation for verification, passing the animated DO and DP.
                        _sideBySide.RegisterAnimation(rotateTransform2, RotateTransform.AngleProperty, _clock1);
                        break;
                    case "scale":
                        DoubleAnimation animScale  = new DoubleAnimation();                                             
                        animScale.BeginTime           = _BEGIN_TIME;
                        animScale.Duration            = new Duration(_DURATION_TIME);
                        animScale.By                  = 3;

                        ScaleTransform scaleTransform = new ScaleTransform();
                        scaleTransform.ScaleX   = 1d;
                        scaleTransform.ScaleY   = 1d;

                        _clock1 = animScale.CreateClock();
                        scaleTransform.ApplyAnimationClock(ScaleTransform.ScaleYProperty, _clock1);

                        _textBlock1.RenderTransform = scaleTransform;

                        //Register an animation for verification, passing the animated DO and DP.
                        _sideBySide.RegisterAnimation(scaleTransform, ScaleTransform.ScaleYProperty, _clock1);
                        break;
                    case "skew":
                        DoubleAnimation animSkew = new DoubleAnimation();                                             
                        animSkew.BeginTime           = _BEGIN_TIME;
                        animSkew.Duration            = new Duration(_DURATION_TIME);
                        animSkew.By                  = 30;

                        SkewTransform skewTransform = new SkewTransform();
                        skewTransform.CenterX   = 60;
                        skewTransform.CenterY   = 60;

                        _clock1 = animSkew.CreateClock();
                        skewTransform.ApplyAnimationClock(SkewTransform.AngleXProperty, _clock1);

                        _textBlock1.RenderTransform = skewTransform;

                        //Register an animation for verification, passing the animated DO and DP.
                        _sideBySide.RegisterAnimation(skewTransform, SkewTransform.AngleXProperty, _clock1);
                        break;
                    case "text":
                        StringAnimationUsingKeyFrames animString = new StringAnimationUsingKeyFrames();

                        StringKeyFrameCollection SKFC = new StringKeyFrameCollection();
                        SKFC.Add(new DiscreteStringKeyFrame("Y",KeyTime.FromPercent(0f)));
                        SKFC.Add(new DiscreteStringKeyFrame("X",KeyTime.FromPercent(0.5f)));
                        SKFC.Add(new DiscreteStringKeyFrame("W", KeyTime.FromPercent(0.9f)));
                        animString.KeyFrames = SKFC;

                        animString.BeginTime            = _BEGIN_TIME;
                        animString.Duration             = new Duration(_DURATION_TIME);

                        _clock1 = animString.CreateClock();
                        _textBlock1.ApplyAnimationClock(TextBlock.TextProperty, _clock1);

                        //Register an animation for verification, passing the animated DO and DP.
                        _sideBySide.RegisterAnimation(_textBlock1, TextBlock.TextProperty, _clock1);
                        break;
                     case "thickness":
                        ThicknessAnimation animThickness = new ThicknessAnimation();

                        animThickness.BeginTime         = _BEGIN_TIME;
                        animThickness.Duration          = new Duration(_DURATION_TIME);
                        animThickness.By                = new Thickness(9, 9, 9, 9);

                        _clock1 = animThickness.CreateClock();
                        _textBlock1.ApplyAnimationClock(TextBlock.MarginProperty, _clock1);

                        //Register an animation for verification, passing the animated DO and DP.
                        _sideBySide.RegisterAnimation(_textBlock1, TextBlock.MarginProperty, _clock1);
                        break;
                    case "top":
                        DoubleAnimation animTop = new DoubleAnimation();
                        animTop.From           = 50d;
                        animTop.To             = 0d;
                        animTop.BeginTime      = _BEGIN_TIME;
                        animTop.Duration       = new Duration(_DURATION_TIME);
                        _clock1 = animTop.CreateClock();
                        _textBlock1.ApplyAnimationClock(Canvas.TopProperty, _clock1);

                        //Register an animation for verification, passing the animated DO and DP.
                        _sideBySide.RegisterAnimation(_textBlock1, Canvas.TopProperty, _clock1);
                        break;
                    case "translate":
                        DoubleAnimation animTrans = new DoubleAnimation();                                             
                        animTrans.BeginTime            = _BEGIN_TIME;
                        animTrans.Duration             = new Duration(_DURATION_TIME);
                        animTrans.By                   = -25d;

                        TranslateTransform translateTransform = new TranslateTransform();
                        translateTransform.X     = 0;
                        translateTransform.Y     = 0;
                        _clock1 = animTrans.CreateClock();
                        translateTransform.ApplyAnimationClock(TranslateTransform.YProperty, _clock1);

                        _textBlock1.RenderTransform = translateTransform;

                        //Register an animation for verification, passing the animated DO and DP.
                        _sideBySide.RegisterAnimation(translateTransform, TranslateTransform.YProperty, _clock1);
                        break;
                    case "translateleft":
                        DoubleAnimation animTransLeft1 = new DoubleAnimation();                                             
                        animTransLeft1.BeginTime            = _BEGIN_TIME;
                        animTransLeft1.Duration             = new Duration(_DURATION_TIME);
                        animTransLeft1.By                   = 100d;

                        DoubleAnimation animTransLeft2 = new DoubleAnimation();
                        animTransLeft2.From           = 50d;
                        animTransLeft2.To             = 0d;
                        animTransLeft2.BeginTime      = _BEGIN_TIME;
                        animTransLeft2.Duration       = new Duration(_DURATION_TIME);

                        TranslateTransform trans = new TranslateTransform();
                        trans.X     = 0;
                        trans.Y     = 0;

                        _clock1 = animTransLeft1.CreateClock();
                        trans.ApplyAnimationClock(TranslateTransform.XProperty, _clock1);
                        _clock2 = animTransLeft2.CreateClock();
                        _textBlock1.ApplyAnimationClock(Canvas.LeftProperty, _clock2);

                        _textBlock1.RenderTransform = trans;

                        //Register an animation for verification, passing the animated DO and DP.
                        _sideBySide.RegisterAnimation(trans, TranslateTransform.XProperty, _clock1);
                        _sideBySide.RegisterAnimation(_textBlock1, Canvas.LeftProperty, _clock2);
                        break;
                    case "visualrect":
                        RectAnimation animVisualRect = new RectAnimation();
                        animVisualRect.To             = new Rect(0,0,100,100);
                        animVisualRect.BeginTime      = _BEGIN_TIME;
                        animVisualRect.Duration       = new Duration(_DURATION_TIME);

                        VisualBrush VB = new VisualBrush();
                        VB.Viewbox          = new Rect(0,0,0,0);
                        VB.Viewport         = new Rect(0,0,.5,.5);
                        VB.ViewportUnits    = BrushMappingMode.RelativeToBoundingBox;
                        VB.TileMode         = TileMode.Tile;
                        VB.Stretch          = Stretch.UniformToFill;

                        _clock1 = animVisualRect.CreateClock();
                        VB.ApplyAnimationClock(TileBrush.ViewboxProperty, _clock1);

                        Canvas canvas = new Canvas();
                        canvas.Height       = 100d;
                        canvas.Width        = 100d;
                        canvas.Background   = Brushes.Orange;

                        VB.Visual = canvas;

                        _textBlock1.Background   = VB;

                        //Register an animation for verification, passing the animated DO and DP.
                        _sideBySide.RegisterAnimation(VB, TileBrush.ViewboxProperty, _clock1);
                        break;
                    case "width":
                        DoubleAnimation animWidth = new DoubleAnimation();
                        animWidth.To             = 0d;
                        animWidth.BeginTime      = _BEGIN_TIME;
                        animWidth.Duration       = new Duration(_DURATION_TIME);
                        _clock1 = animWidth.CreateClock();
                        _textBlock1.ApplyAnimationClock(FrameworkElement.WidthProperty, _clock1);

                        //Register an animation for verification, passing the animated DO and DP.
                        _sideBySide.RegisterAnimation(_textBlock1, FrameworkElement.WidthProperty, _clock1);
                        break;
                    default:
                        string strMessage = "ERROR!!! CreateAnimation: Incorrect argument.";
                        GlobalLog.LogEvidence(strMessage);
                        _testPassed = false;
                        break;
                 }
            }
            catch (Exception e1)
            {
                GlobalLog.LogEvidence("Exception 1 caught: " + e1.ToString());
                _testPassed = false;
                throw;
            }
        }
              
        /******************************************************************************
        * Function:          OnTimeTicked
        ******************************************************************************/
        private void OnTimeTicked(object sender, TimeControlArgs e)
        {
            GlobalLog.LogEvidence("---------------------------------------------");

            _sideBySide.toleranceInPercent = .25;

            //Verify Values.
            GlobalLog.LogEvidence("VERIFICATION RESULTS");
            bool valueResult = _sideBySide.ValuesOnlyVerify(e.curTime);
            GlobalLog.LogEvidence("  VALUES:    " + valueResult);

            //Verify Rendering.
            bool renderResult = _sideBySide.Verify(e.curTime);
            GlobalLog.LogEvidence("  RENDERING: " + renderResult);
            GlobalLog.LogEvidence(_sideBySide.verboseLog);

            _testPassed = (_testPassed && (valueResult && renderResult));

            if (e.lastTick)
            {
                GlobalLog.LogEvidence("-------------Last Tick-----------------------");

                if (_testPassed)
                {
                    Signal("TestFinished", TestResult.Pass);
                }
                else
                {
                    Signal("TestFinished", TestResult.Fail);
                }
            }
        }

        /******************************************************************************
        * Function:          Verify
        ******************************************************************************/
        /// <summary>
        /// Verifies the result of the test case, and returns a Pass/Fail result.
        /// </summary>
        /// <returns>A TestResult, indicating pass or fail</returns>
        TestResult Verify()
        {
            TestResult result = WaitForSignal("TestFinished");

            return result;
        }
        #endregion
    }          
}
