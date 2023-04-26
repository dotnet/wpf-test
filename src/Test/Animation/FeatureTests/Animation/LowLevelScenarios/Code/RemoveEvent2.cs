// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* ***************  Animation Event Test ****************************************************
*     Description:
*          Verify that events attached to a Clock can be removed.
*          This version removes the events via the Begun event, so that the Begun and Changed
*          events fire once.
*          NOTE: many of the Animations affect Ellipse properties, but others involve Path or
*          Button properties, or are attached to a ParallelTimeline and do not affect UI.
*     Pass Conditions:
*          The test case will Pass if all events do not fire after removal.
*     How verified:
*          The result of the comparisons between actual and expected values is passed to
*          frmwk.LogStatus().
*
*     Framework:          A CLR executable is created.
*     Area:               Animation/Timing
*     Dependencies:
*     Support Files:
* ******************************************************************************************************/
using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
     
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.LowLevelScenarios.Events</area>
    /// <description>
    /// Verify Animation event firing when the events are dynamically removed.


    [Test(3, "Animation.LowLevelScenarios.Events", "RemoveEvent2Test")]

    class RemoveEvent2Test : WindowTest
    {
        #region Test case members

        private string                      _inputString     = "";
        private Canvas                      _body;
        private Ellipse                     _EL;
        private AnimationClock              _AC;
        private Clock                       _TLC;
        private ParallelTimeline            _root;
        private int                         _SEEK_TIME        = 200;
        private int                         _DURATION         = 1000;

        #endregion


        #region Constructor

        [Variation("Byte")]
        [Variation("Color")]
        [Variation("Double")]
        [Variation("Double2")]
        [Variation("Point")]
        [Variation("Rect")]
        [Variation("Size")]
        [Variation("Vector")]
        [Variation("Point3D")]
        [Variation("Vector3D")]
        [Variation("PathPoint")]
        [Variation("Bool")]
        [Variation("Char")]
        [Variation("Decimal")]
        [Variation("Int16")]
        [Variation("Int32")]
        [Variation("Int64")]
        [Variation("PathDouble")]
        [Variation("PathDouble2")]
        [Variation("Single")]
        [Variation("String", Priority=1)]
        [Variation("Thickness")]
        [Variation("Double3")]

        /******************************************************************************
        * Function:          AddEvent2Test Constructor
        ******************************************************************************/
        public RemoveEvent2Test(string testValue)
        {
            _inputString = testValue;
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(CreateTimelines);
            RunSteps += new TestStep(CreateAnimation);
            RunSteps += new TestStep(Animate);
            RunSteps += new TestStep(Verify);
        }

        #endregion

        #region Test Steps

        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Initialize: create a new Window and add content to it.
        /// </summary>
        /// <returns>TestResult.Pass</returns>
        TestResult Initialize()
        {
            GlobalLog.LogStatus("---Initialize---");

            Window.Width           = 200;
            Window.Height          = 200;
            Window.Title           = "RemoveEvent2 Test";

            _EL  = new Ellipse();
            _EL.Height       = 60;
            _EL.Width        = 60;
            _EL.Fill         = Brushes.Lavender;        
            Canvas.SetTop  (_EL, 70d);
            Canvas.SetLeft (_EL, 70d);

            _body = new Canvas();
            _body.Children.Add(_EL);
            Window.Content = _body;
            
            return TestResult.Pass;
        }
        
        /******************************************************************************
        * Function:          CreateTimelines
        ******************************************************************************/
        /// <summary>
        /// CreateTimelines: create additional Timelines for control and verification.
        /// </summary>
        /// <returns>TestResult.Pass</returns>
        TestResult CreateTimelines()
        {
            GlobalLog.LogStatus("---Creating Verification ParallelTimeline---");

            _root = new ParallelTimeline();
            _root.BeginTime           = null;
            _root.Duration            = Duration.Forever;

            //This is a separate Timeline, used for verification.
            ParallelTimeline TL1 = new ParallelTimeline();
            TL1.BeginTime                 = TimeSpan.FromMilliseconds(0);
            TL1.Duration                  = new Duration(TimeSpan.FromMilliseconds(6000));
            TL1.CurrentStateInvalidated  += new EventHandler(OnCurrentStateTL1);
            _root.Children.Add(TL1);

            //This is a separate timeline, used only to control invocation of the
            //Pause/Resume/Seek methods for the Animation.
            ParallelTimeline TL2 = new ParallelTimeline();
            TL2.BeginTime                   = TimeSpan.FromMilliseconds(0);
            TL2.Duration                    = new Duration(TimeSpan.FromMilliseconds(750));
            TL2.CurrentStateInvalidated    += new EventHandler(OnCurrentStateTL2);
            _root.Children.Add(TL2);
            
            return TestResult.Pass;
        }

        
        /******************************************************************************
        * Function:          CreateAnimation
        ******************************************************************************/
        /// <summary>
        /// CreateAnimation: create an Animation, but do not start it.
        /// </summary>
        /// <returns>TestResult.Pass</returns>
        TestResult CreateAnimation()
        {
            GlobalLog.LogStatus("---CreateAnimation---");
                    
            bool animationCreated = true;

            switch (_inputString)
            {
                 case "Byte":
                      //SCENARIO 1: Byte Animation
                      ByteAnimation animByte = new ByteAnimation();
                      animByte.From                   = 0;
                      animByte.To                     = 3;
                      animByte.BeginTime              = null;
                      animByte.Duration               = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                      animByte.RepeatBehavior         = new RepeatBehavior(2);
                      animByte.AutoReverse            = true;

                      //Currently, no DPs or other objects support ByteAnimation, so 
                      //hooking it up to the Root.
                      _root.Children.Add(animByte);
                      _TLC = _root.CreateClock();
                      _AC = (AnimationClock)animByte.CreateClock();
                      break;

                 case "Color":
                      //SCENARIO 2: Color Animation
                      ColorAnimation animColor = new ColorAnimation();
                      animColor.From                  = Colors.Black;
                      animColor.To                    = Colors.White;
                      animColor.BeginTime             = null;
                      animColor.Duration              = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                      animColor.RepeatBehavior        = new RepeatBehavior(2);
                      animColor.AutoReverse           = true;

                      SolidColorBrush SCB1 = new SolidColorBrush();
                      SCB1.Color = Colors.Red;
                      _AC = animColor.CreateClock();
                      SCB1.ApplyAnimationClock(SolidColorBrush.ColorProperty, _AC);
                      _EL.Fill = SCB1;
                      _TLC = _root.CreateClock();
                      break;

                 case "Double":
                      //SCENARIO 3: Double Animation
                      DoubleAnimation animDouble = new DoubleAnimation();
                      animDouble.From                 = 1;
                      animDouble.To                   = 0;
                      animDouble.BeginTime            = null;
                      animDouble.Duration             = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                      animDouble.RepeatBehavior       = new RepeatBehavior(2);
                      animDouble.AutoReverse          = true;

                      RadialGradientBrush RGB = new RadialGradientBrush();
                      RGB.GradientStops.Add(new GradientStop(Colors.Blue, 0.0));
                      RGB.GradientStops.Add(new GradientStop(Colors.LightGreen, 1.0));
                      RGB.Center        = new Point(0.5, 0.5);
                      RGB.RadiusX       = 1;
                      RGB.RadiusY       = 1;
                      RGB.GradientOrigin = new Point(0.0, 0.0);
                      _AC = animDouble.CreateClock();
                      RGB.ApplyAnimationClock(RadialGradientBrush.OpacityProperty, _AC);
                      _EL.Fill = RGB;
                      _TLC = _root.CreateClock();
                      break;                                   

                 case "Double2":
                      //SCENARIO 4: Double Animation
                      DoubleAnimation animDouble2 = new DoubleAnimation();
                      animDouble2.From                 = 160;
                      animDouble2.To                   = 20;
                      animDouble2.BeginTime            = null;
                      animDouble2.Duration             = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                      animDouble2.RepeatBehavior       = new RepeatBehavior(2);
                      animDouble2.AutoReverse          = true;

                      _AC = animDouble2.CreateClock();
                      _EL.ApplyAnimationClock(Ellipse.WidthProperty, _AC);
                      _TLC = _root.CreateClock();
                      break;

                 case "Point":
                      //SCENARIO 5: Point Animation
                      PointAnimation animPoint = new PointAnimation();
                      animPoint.From                  = new Point(1.0, 0.0);
                      animPoint.To                    = new Point(0.0, 1.0);
                      animPoint.BeginTime             = null;
                      animPoint.Duration              = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                      animPoint.RepeatBehavior        = new RepeatBehavior(2);
                      animPoint.AutoReverse           = true;

                      LinearGradientBrush LGB = new LinearGradientBrush();
                      LGB.StartPoint = new Point(0.0, 0.0);
                      LGB.EndPoint = new Point(1.0, 1.0);
                      LGB.MappingMode = BrushMappingMode.RelativeToBoundingBox;
                      GradientStopCollection GSC = new GradientStopCollection();
                      GSC.Add(new GradientStop(Colors.Blue, 0.0));
                      GSC.Add(new GradientStop(Colors.LightBlue, 1.0));
                      LGB.GradientStops = GSC;

                      _EL.Fill = LGB;

                      _AC = animPoint.CreateClock();
                      LGB.ApplyAnimationClock(LinearGradientBrush.EndPointProperty, _AC);
                      _TLC = _root.CreateClock();

                      break;

                 case "Rect":
                      //SCENARIO 6: Rect Animation                                                  
                      RectAnimation animRect = new RectAnimation();
                      animRect.From                   = new Rect(0,0,0,0);
                      animRect.To                     = new Rect(100,100,100,100);
                      animRect.BeginTime              = null;
                      animRect.Duration               = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                      animRect.RepeatBehavior         = new RepeatBehavior(2);
                      animRect.AutoReverse            = true;

                      Path PT = new Path();
                      PT.Fill           = Brushes.SlateBlue;

                      RectangleGeometry RG = new RectangleGeometry();
                      RG.Rect = new Rect(0,0,0,0);
                      _AC = animRect.CreateClock();
                      RG.ApplyAnimationClock(RectangleGeometry.RectProperty, _AC);
                      PT.Data = RG;
                      _body.Children.Add(PT);

                      _TLC = _root.CreateClock();
                      break;

                 case "Size":
                      //SCENARIO 7: Size Animation
                      SizeAnimation animSize = new SizeAnimation();
                      animSize.By                     = new Size(20,20);
                      animSize.BeginTime              = null;
                      animSize.Duration               = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                      animSize.RepeatBehavior         = new RepeatBehavior(2);
                      animSize.AutoReverse            = true;

                      Path path1 = new Path();
                      path1.Fill                = Brushes.Red;
                      path1.StrokeThickness     = 5;
                      path1.Stroke              = Brushes.Blue;

                      ArcSegment AS = new ArcSegment ();     
                      AS.IsLargeArc      = true;
                      AS.Size          = new Size(50, 50);
                      AS.SweepDirection     = SweepDirection.Counterclockwise;
                      AS.RotationAngle     = 50;

                      _AC = animSize.CreateClock();
                      AS.ApplyAnimationClock(ArcSegment.SizeProperty, _AC);

                      PathFigure PF1 = new PathFigure ();
                      PF1.StartPoint = new Point(30, 30);
                      PF1.Segments.Add(AS);

                      PathGeometry PG = new PathGeometry ();
                      PG.Figures.Add(PF1);

                      path1.Data = PG;
                      _body.Children.Add(path1);

                      _TLC = _root.CreateClock();
                      break;

                 case "Vector":
                      //SCENARIO 8: Vector Animation
                      VectorAnimation animVector = new VectorAnimation();
                      animVector.From                 = new Vector(100,100);
                      animVector.To                   = new Vector(0,0);
                      animVector.BeginTime            = null;
                      animVector.Duration             = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                      animVector.RepeatBehavior       = new RepeatBehavior(2);
                      animVector.AutoReverse          = true;

                      //Currently, no DPs or other objects support VectorAnimation, so 
                      //hooking it up to the Root.
                      _root.Children.Add(animVector);
                      _TLC = _root.CreateClock();
                      _AC = (AnimationClock)animVector.CreateClock();
                      break;

                 case "Point3D":
                      //SCENARIO 9: Point3D Animation
                      Point3DAnimation animPoint3D = new Point3DAnimation();
                      animPoint3D.From                = new Point3D( 0, 0, 0 );
                      animPoint3D.To                  = new Point3D( 0, 1, 0 );
                      animPoint3D.BeginTime           = null;
                      animPoint3D.Duration            = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                      animPoint3D.RepeatBehavior      = new RepeatBehavior(2);
                      animPoint3D.AutoReverse         = true;

                      //hooking up the Point3DAnimation to the Root.
                      _root.Children.Add(animPoint3D);
                      _TLC = _root.CreateClock();
                      _AC = (AnimationClock)animPoint3D.CreateClock();
                      break; 

                 case "Vector3D":
                      //SCENARIO 12: Vector3D Animation
                      Vector3DAnimation animVector3D = new Vector3DAnimation();
                      animVector3D.From               = new Vector3D( 2, 4, 6 );
                      animVector3D.To                 = new Vector3D( 3, 5, 7 );
                      animVector3D.BeginTime          = null;
                      animVector3D.Duration           = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                      animVector3D.RepeatBehavior     = new RepeatBehavior(2);
                      animVector3D.AutoReverse        = true;

                      //hooking up the Vector3DAnimation to the Root.
                      _root.Children.Add(animVector3D);
                      _TLC = _root.CreateClock();
                      _AC = (AnimationClock)animVector3D.CreateClock();
                      break;

                 case "PathPoint":
                      //SCENARIO 13: Path Animation
                      PointAnimationUsingPath animPathPoint = new PointAnimationUsingPath();
                      animPathPoint.BeginTime = null;
                      animPathPoint.Duration               = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                      animPathPoint.RepeatBehavior         = new RepeatBehavior(2);
                      animPathPoint.AutoReverse            = true;

                      //hooking up the PointAnimationUsingPath to the Root.
                      _root.Children.Add(animPathPoint);
                      _TLC = _root.CreateClock();
                      _AC = (AnimationClock)animPathPoint.CreateClock();
                      break;

                 case "Bool":
                      //SCENARIO 14: Bool Animation
                      BooleanAnimationUsingKeyFrames animBool = new BooleanAnimationUsingKeyFrames();

                      BooleanKeyFrameCollection BKFC = new BooleanKeyFrameCollection();
                      BKFC.Add(new DiscreteBooleanKeyFrame(false,KeyTime.FromPercent(0f)));
                      BKFC.Add(new DiscreteBooleanKeyFrame(true,KeyTime.FromPercent(0.5f)));
                      BKFC.Add(new DiscreteBooleanKeyFrame(false, KeyTime.FromPercent(1.0f)));
                      animBool.KeyFrames = BKFC;

                      animBool.BeginTime              = null;
                      animBool.Duration               = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                      animBool.RepeatBehavior         = new RepeatBehavior(2);
                      animBool.AutoReverse            = true;

                      _AC = animBool.CreateClock();
                      _EL.ApplyAnimationClock(Ellipse.FocusableProperty, _AC);
                      _TLC = _root.CreateClock();
                      break;

                 case "Char":
                      //SCENARIO 15: Char Animation
                      CharAnimationUsingKeyFrames animChar = new CharAnimationUsingKeyFrames();

                      CharKeyFrameCollection CKFC = new CharKeyFrameCollection();
                      CKFC.Add(new DiscreteCharKeyFrame('a',KeyTime.FromPercent(0.5f)));
                      CKFC.Add(new DiscreteCharKeyFrame('b',KeyTime.FromPercent(1.0f)));
                      animChar.KeyFrames = CKFC;

                      animChar.BeginTime              = null;
                      animChar.Duration               = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                      animChar.RepeatBehavior         = new RepeatBehavior(2);
                      animChar.AutoReverse            = true;

                      //hooking up the CharAnimationUsingKeyFrames to the Root.
                      _root.Children.Add(animChar);
                      _TLC = _root.CreateClock();
                      _AC = (AnimationClock)animChar.CreateClock();
                      break;

                 case "Decimal":
                      //SCENARIO 16: Decimal Animation
                      DecimalAnimation animDecimal = new DecimalAnimation();
                      animDecimal.From                = 0;
                      animDecimal.To                  = 200.5M;
                      animDecimal.BeginTime           = null;
                      animDecimal.Duration            = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                      animDecimal.RepeatBehavior      = new RepeatBehavior(2);
                      animDecimal.AutoReverse         = true;

                      //Currently, no DPs or other objects support DecimalAnimation, so 
                      //hooking it up to the Root.
                      _root.Children.Add(animDecimal);
                      _TLC = _root.CreateClock();
                      _AC = (AnimationClock)animDecimal.CreateClock();
                      break;

                 case "Int16":
                      //SCENARIO 17: Int16 Animation
                      Int16Animation animInt16 = new Int16Animation();
                      animInt16.From                  = 0;
                      animInt16.To                    = 15;
                      animInt16.BeginTime             = null;
                      animInt16.Duration              = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                      animInt16.RepeatBehavior        = new RepeatBehavior(2);
                      animInt16.AutoReverse           = true;

                      //Currently, no DPs or other objects support Int16Animation, so 
                      //hooking it up to the Root.
                      _root.Children.Add(animInt16);
                      _TLC = _root.CreateClock();
                      _AC = (AnimationClock)animInt16.CreateClock();
                      break;

                 case "Int32":
                      //SCENARIO 18: Int32 Animation
                      Int32Animation animInt32 = new Int32Animation();
                      animInt32.From                  = 5;
                      animInt32.To                    = 0;
                      animInt32.BeginTime             = null;
                      animInt32.Duration              = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                      animInt32.RepeatBehavior        = new RepeatBehavior(2);
                      animInt32.AutoReverse           = true;

                      _root.Children.Add(animInt32);
                      _TLC = _root.CreateClock();
                      _AC = (AnimationClock)animInt32.CreateClock();
                      break;  

                 case "Int64":
                      //SCENARIO 19: In64 Animation
                      System.Int64 fromValue = 5;
                      System.Int64 toValue   = 9999;

                      Int64Animation animInt64 = new Int64Animation();
                      animInt64.From                 = fromValue;
                      animInt64.To                   = toValue;
                      animInt64.BeginTime            = null;
                      animInt64.Duration             = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                      animInt64.RepeatBehavior       = new RepeatBehavior(2);
                      animInt64.AutoReverse          = true;

                      //Currently, no DPs or other objects support In64Animation, so 
                      //hooking it up to the Root.
                      _root.Children.Add(animInt64);
                      _TLC = _root.CreateClock();
                      _AC = (AnimationClock)animInt64.CreateClock();
                      break; 

                 case "PathDouble":
                      //SCENARIO 20: PathDouble Animation

                      //Create a PathFigureCollection.
                      PathFigureCollection PFC1 = SpecialObjects.CreatePathFigureCollection();

                      //Create a PathGeometry: animation.
                      //Assign it a ScaleTransform and PathFigureCollection.
                      ScaleTransform scaleTransform1 = new ScaleTransform();
                      scaleTransform1.ScaleX     = 1.5;
                      scaleTransform1.ScaleY     = 1.5;
                      PathGeometry pathGeometry = new PathGeometry();
                      pathGeometry.Transform    = scaleTransform1;
                      pathGeometry.Figures      = PFC1;

                      //Specify the DoubleAnimationUsingPath; add the PathGeometry to it.
                      DoubleAnimationUsingPath animPathDouble = new DoubleAnimationUsingPath();
                      animPathDouble.IsAdditive       = true;
                      animPathDouble.BeginTime        = null;
                      animPathDouble.Duration         = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                      animPathDouble.RepeatBehavior   = new RepeatBehavior(2);
                      animPathDouble.AutoReverse      = true;

                      animPathDouble.Source           = PathAnimationSource.X;
                      animPathDouble.PathGeometry     = pathGeometry; //Takes the PathGeometry.

                      //Create a TranslateTransform and add the Animation to it.
                      TranslateTransform TT = new TranslateTransform();
                      TT.X     = 5.0;

                      //Create a TransformDecorator and add the animating TranslateTransform
                      //and a new Ellipse to it.
                      Decorator TD2 = new Decorator();

                      _AC = animPathDouble.CreateClock();
                      TT.ApplyAnimationClock(TranslateTransform.XProperty, _AC);

                      TD2.LayoutTransform = TT;

                      Ellipse EL2 = new Ellipse();
                      EL2.Height = 20;
                      EL2.Width  = 120;

                      TD2.Child = EL2;
                      _body.Children.Add(TD2);

                      _TLC = _root.CreateClock();
                      break;                                   
                 case "PathDouble2":
                      //SCENARIO 21: PathDouble Animation               

                      //Create a PathFigureCollection.
                      PathFigureCollection PFC2 = SpecialObjects.CreatePathFigureCollection();

                      //Create a PathGeometry: animation.
                      //Assign it the ScaleTransform and PathFigureCollection.
                      PathGeometry pathGeometry2 = new PathGeometry();
                      pathGeometry2.Figures   = PFC2;

                      //Specify the DoubleAnimationUsingPath; add the PathGeometry to it.
                      DoubleAnimationUsingPath animPathDouble2 = new DoubleAnimationUsingPath();
                      animPathDouble2.IsAdditive       = true;
                      animPathDouble2.BeginTime        = null;
                      animPathDouble2.Duration         = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                      animPathDouble2.RepeatBehavior   = new RepeatBehavior(2);
                      animPathDouble2.AutoReverse      = true;

                      animPathDouble2.Source           = PathAnimationSource.Y;
                      animPathDouble2.PathGeometry     = pathGeometry2; //Takes the PathGeometry.

                      _AC = animPathDouble2.CreateClock();
                      _EL.ApplyAnimationClock(Canvas.LeftProperty, _AC);
                      _TLC = _root.CreateClock();
                      break;

                 case "Single":
                      //SCENARIO 22: Single Animation
                      SingleAnimation animSingle = new SingleAnimation();
                      animSingle.BeginTime            = null;
                      animSingle.Duration             = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                      animSingle.RepeatBehavior       = new RepeatBehavior(2);
                      animSingle.AutoReverse          = true;

                      //hooking up the SingleAnimation to the Root.
                      _root.Children.Add(animSingle);
                      _TLC = _root.CreateClock();
                      _AC = (AnimationClock)animSingle.CreateClock();
                      break;

                 case "String":
                      //SCENARIO 23: String Animation

                      Button BN = new Button();
                      //BN.Height         = 60;
                      //BN.Width          = 120;
                      BN.FontSize       = 48;
                      BN.Background     = Brushes.MediumAquamarine;        
                      Canvas.SetTop  (BN, 70d);
                      Canvas.SetLeft (BN, 70d);
                      _body.Children.Add(BN);
                      StringAnimationUsingKeyFrames animString = new StringAnimationUsingKeyFrames();

                      StringKeyFrameCollection SKFC = new StringKeyFrameCollection();
                      SKFC.Add(new DiscreteStringKeyFrame("I'll",KeyTime.FromPercent(0f)));
                      SKFC.Add(new DiscreteStringKeyFrame("be",KeyTime.FromPercent(0.5f)));
                      SKFC.Add(new DiscreteStringKeyFrame("back.", KeyTime.FromPercent(0.9f)));
                      animString.KeyFrames = SKFC;

                      animString.BeginTime            = null;
                      animString.Duration             = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                      animString.RepeatBehavior       = new RepeatBehavior(2);
                      animString.AutoReverse          = true;

                      _AC = animString.CreateClock();
                      BN.ApplyAnimationClock(Button.ContentProperty, _AC);
                      _TLC = _root.CreateClock();
                      break;                                   
                 case "Thickness":
                      //SCENARIO 24: Thickness Animation
                      ThicknessAnimation animThickness = new ThicknessAnimation();

                      animThickness.BeginTime         = null;
                      animThickness.Duration          = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                      animThickness.By                = new Thickness(9, 9, 9, 9);
                      animThickness.RepeatBehavior    = new RepeatBehavior(2);
                      animThickness.AutoReverse       = true;

                      _EL.Margin = new Thickness(1, 1, 1, 1);
                      _AC = animThickness.CreateClock();
                      _EL.ApplyAnimationClock(Ellipse.MarginProperty, _AC);
                      _TLC = _root.CreateClock();
                      break;
                 case "Double3":
                      //SCENARIO 25: Double Animation [RotateTransform]
                      DoubleAnimation animAngle = new DoubleAnimation();                                             
                      animAngle.FillBehavior        = FillBehavior.HoldEnd;
                      animAngle.BeginTime           = null;
                      animAngle.Duration            = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                      animAngle.From                = 0d;
                      animAngle.To                  = 45d;

                      RotateTransform rotateTransform = new RotateTransform();
                      rotateTransform.Angle         = 0d;
                      rotateTransform.CenterX       = 30;
                      rotateTransform.CenterY       = 60;

                      _EL.RenderTransform = rotateTransform;
                      _EL.Height = 120;

                      _AC = animAngle.CreateClock();
                      rotateTransform.ApplyAnimationClock(RotateTransform.AngleProperty, _AC);
                      _TLC = _root.CreateClock();
                      break;

                 default:
                    GlobalLog.LogEvidence("ERROR!!! CreateAnimation: Unexpected failure to match argument.");
                    animationCreated = false;
                    break;
            }

            if (animationCreated)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        /******************************************************************************
        * Function:          Animate
        ******************************************************************************/
        /// <summary>
        /// Animate: Start the Animation.
        /// </summary>
        /// <returns>TestResult.Pass</returns>
        TestResult Animate()
        {
            AttachHandlersClock(ref _AC);

            _TLC.Controller.Begin();  //root (also begins its children).
            _AC.Controller.Begin();   //animation (sibling of root).

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          AttachHandlersClock
        ******************************************************************************/
        private void AttachHandlersClock(ref AnimationClock clock)
        {
            clock.CurrentStateInvalidated       += new EventHandler(OnCurrentStateInvalidated);
            clock.CurrentGlobalSpeedInvalidated += new EventHandler(OnCurrentGlobalSpeedInvalidated);
            clock.CurrentTimeInvalidated        += new EventHandler(OnCurrentTimeInvalidated);
        }
          
        /******************************************************************************
        * Function:          DetachHandlersClock
        ******************************************************************************/
        private void DetachHandlersClock(ref AnimationClock clock)
        {
            clock.CurrentStateInvalidated       -= new EventHandler(OnCurrentStateInvalidated);
            clock.CurrentGlobalSpeedInvalidated -= new EventHandler(OnCurrentGlobalSpeedInvalidated);
            clock.CurrentTimeInvalidated        -= new EventHandler(OnCurrentTimeInvalidated);
        }
          
        /******************************************************************************
        * Function:          OnCurrentStateInvalidated
        ******************************************************************************/
        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {
            //GlobalLog.LogStatus("---OnCurrentStateInvalidated--AC-" + ((Clock)sender).CurrentState);
            CommonEvents.CurrentStateInvalidatedHandler(sender);
        }

        /******************************************************************************
        * Function:          OnCurrentTimeInvalidated
        ******************************************************************************/
        private void OnCurrentTimeInvalidated(object sender, EventArgs e)
        {
            //GlobalLog.LogStatus("---OnCurrentTimeInvalidated--AC-");
            CommonEvents.CurrentTimeInvalidatedHandler(sender);
        }
          
        /******************************************************************************
        * Function:          OnCurrentGlobalSpeedInvalidated
        ******************************************************************************/
        /// <summary>
        /// Used to remove the Events.
        /// </summary>
        /// <returns></returns>
        private void OnCurrentGlobalSpeedInvalidated(object sender, EventArgs e)
        {               
            //GlobalLog.LogStatus("---OnCurrentGlobalSpeedInvalidated--AC-");
            CommonEvents.CurrentGlobalSpeedInvalidatedHandler(sender);

            //REMOVE EVENTS WHEN THE ANIMATION IS PAUSED.
            if (((Clock)sender).IsPaused)
            {
                DetachHandlersClock(ref _AC);
            }
        }

        /******************************************************************************
        * Function:          OnCurrentTimeInvalidatedRepeat
        ******************************************************************************/
        public void OnCurrentTimeInvalidatedRepeat(object sender, EventArgs args)
        {                             
            CommonEvents.CurrentTimeInvalidatedRepeatHandler(sender);
        }
               
        /******************************************************************************
        * Function:          OnCurrentStateTL2
        ******************************************************************************/
        /// <summary>
        /// Used to Pause/Seek the Animation.
        /// </summary>
        /// <returns></returns>
        private void OnCurrentStateTL2(object sender, EventArgs e)
        {     
            //GlobalLog.LogStatus("---OnCurrentStateTL2---" + ((Clock)sender).CurrentState);
            if (((Clock)sender).CurrentState == ClockState.Active)
            {
                GlobalLog.LogStatus("---Begun TL2---");
                //Using this event to start the Pause/Resume/Seek sequence for the Animaton.
                _AC.Controller.Pause();
            }
            else
            {
                GlobalLog.LogStatus("---Ended TL2---");
                //Using this event to Resume the Animation, then perform a Seek().
                _AC.Controller.Resume();
                CommonEvents.seekInvoked = true;
                _AC.Controller.Seek(TimeSpan.FromMilliseconds(_SEEK_TIME), TimeSeekOrigin.BeginTime);
            }
        }
          
        /******************************************************************************
        * Function:          OnCurrentStateTL1
        ******************************************************************************/
        /// <summary>
        /// Checks the result of removing the Animation events. Signals the result using TestResult.
        /// </summary>
        /// <returns></returns>
        private void OnCurrentStateTL1(object sender, EventArgs e)
        {
            if (((Clock)sender).CurrentState != ClockState.Active)
            {
                GlobalLog.LogStatus("---OnCurrentStateTL1---" + ((Clock)sender).CurrentState);

                int expBegunCount        = 1;
                int expEndedCount        = 0;
                int expPausedCount       = 1;
                int expRepeatedCount     = 0;
                int expResumedCount      = 0;
                int expReversedCount     = 0;

                GlobalLog.LogEvidence("Begun Count-    Actual: " + CommonEvents.begunCount    + " / Expected: " + expBegunCount);
                GlobalLog.LogEvidence("Ended Count-    Actual: " + CommonEvents.endedCount    + " / Expected: " + expEndedCount);
                GlobalLog.LogEvidence("Paused Count-   Actual: " + CommonEvents.pausedCount   + " / Expected: " + expPausedCount);
                GlobalLog.LogEvidence("Repeated Count- Actual: " + CommonEvents.repeatedCount + " / Expected: " + expRepeatedCount);
                GlobalLog.LogEvidence("Resumed Count-  Actual: " + CommonEvents.resumedCount  + " / Expected: " + expResumedCount);
                GlobalLog.LogEvidence("Reversed Count- Actual: " + CommonEvents.reversedCount + " / Expected: " + expReversedCount);

                bool pass1 = (CommonEvents.begunCount          == expBegunCount);
                bool pass2 = (CommonEvents.endedCount          == expEndedCount);
                bool pass3 = (CommonEvents.pausedCount         == expPausedCount);
                bool pass4 = (CommonEvents.repeatedCount       == expRepeatedCount);
                bool pass5 = (CommonEvents.resumedCount        == expResumedCount);
                bool pass6 = (CommonEvents.reversedCount       == expReversedCount);

                bool testPassed = (pass1 && pass2 && pass3 && pass4 && pass5 && pass6);

                GlobalLog.LogEvidence("FINAL RESULT: " + testPassed);

                if (testPassed)
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
