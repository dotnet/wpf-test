// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* ***************  Animation Event Test *****************
*     Description:
*          Verify that events fire when attached to a Clock.
*     Pass Conditions:
*          The test case will Pass if all events fire appropriately.
*     How verified:
*          The result of the comparisons between actual and expected values is passed to TestResult.
*
*     Framework:          A CLR executable is created.
*     Area:               Animation/Timing
*     Dependencies:       TestRuntime.dll
*     Support Files:
* *******************************************************/
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
    /// <priority>2</priority>
    /// <description>
    /// Verify Animation event firing in a variety of scenarios
    /// </description>
    /// </summary>
    [Test(2, "Animation.LowLevelScenarios.Events", "AddEvent2Test")]

    class AddEvent2Test : WindowTest
    {
        #region Test case members

        private string                  _inputString     = "";
        private Canvas                  _body;
        private Glyphs                  _GY;
        private AnimationClock          _AC;
        private Clock                   _rootClock;
        private ParallelTimeline        _root;
        private int                     _SEEK_TIME       = 100;
        private int                     _DURATION        = 500;

        #endregion


        #region Constructor

        [Variation("Color")]
        [Variation("Double", Priority=1)]
        [Variation("Double2")]
        [Variation("Byte")]
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
        [Variation("String")]
        [Variation("Thickness")]

        /******************************************************************************
        * Function:          AddEvent2Test Constructor
        ******************************************************************************/
        public AddEvent2Test(string testValue)
        {
            _inputString = testValue;
            InitializeSteps += new TestStep(Initialize);
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
        /// <returns>TestResult.Pass;</returns>
        TestResult Initialize()
        {
            GlobalLog.LogStatus("---Initialize---");
                    
            _GY = new Glyphs();
            _GY.Fill                    = Brushes.Brown;
            _GY.FontRenderingEmSize     = 20.0;
            _GY.IsSideways              = false;
            _GY.OriginX                 = 150;
            _GY.OriginY                 = 150;
            _GY.UnicodeString           = "Hello, World!";
            //GY.FontUri                = new Uri(@"c:\windows\fonts\tahoma.ttf", UriKind.RelativeOrAbsolute);
            _GY.FontUri                 = new Uri( @"file://" + Environment.GetEnvironmentVariable("windir") + @"\fonts\tahoma.ttf", UriKind.RelativeOrAbsolute);
            _GY.Indices                 = ";;;;;;;,89";
            _GY.BidiLevel               = 1;

            _body = new Canvas();
            _body.Background     = Brushes.Moccasin;
            _body.Width          = 200;
            _body.Height         = 200;

            _body.Children.Add(_GY);
            Window.Content = _body;
            
            return TestResult.Pass;
        }
        
        /******************************************************************************
        * Function:          CreateAnimation
        ******************************************************************************/
        /// <summary>
        /// CreateAnimation: create an Animation, but do not start it.
        /// </summary>
        /// <returns>TestResult</returns>
        TestResult CreateAnimation()
        {
            GlobalLog.LogStatus("---CreateAnimation---");

            bool animationCreated = true;

            _root = new ParallelTimeline();
            _root.BeginTime           = null;
            _root.Duration            = Duration.Forever;

            //This separate timeline is used to carry out verification, via OnCurrentStateTL1.
            ParallelTimeline TL1 = new ParallelTimeline();
            TL1.BeginTime                       = TimeSpan.FromMilliseconds(0);
            TL1.Duration                        = new Duration(TimeSpan.FromMilliseconds(4000));
            TL1.CurrentStateInvalidated        += new EventHandler(OnCurrentStateTL1);
            _root.Children.Add(TL1);

            //This separate timeline is used only to control invocation of the
            //Pause/Resume/Seek methods for the Animation.
            ParallelTimeline TL2 = new ParallelTimeline();
            TL2.BeginTime                       = TimeSpan.FromMilliseconds(0);
            TL2.Duration                        = new Duration(TimeSpan.FromMilliseconds(200));
            TL2.CurrentStateInvalidated        += new EventHandler(OnCurrentStateTL2);
            _root.Children.Add(TL2);
          
            switch (_inputString.ToLower(CultureInfo.InvariantCulture))
            {
                case "color":
                    //SCENARIO 1: Color Animation
                    ColorAnimation animColor = new ColorAnimation();
                    animColor.From                  = Colors.Orange;
                    animColor.To                    = Colors.Purple;
                    animColor.BeginTime             = null;
                    animColor.Duration              = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                    animColor.RepeatBehavior        = new RepeatBehavior(2);
                    animColor.AutoReverse           = true;

                    SolidColorBrush SCB1 = new SolidColorBrush();
                    SCB1.Color = Colors.Orange;
                    _AC = animColor.CreateClock();
                    SCB1.ApplyAnimationClock(SolidColorBrush.ColorProperty, _AC);
                    _GY.Fill = SCB1;

                    _rootClock = _root.CreateClock();
                    break;

                case "double":
                    //SCENARIO 2: Double Animation
                    DoubleAnimation animDouble = new DoubleAnimation();
                    animDouble.From                 = 1;
                    animDouble.To                   = 0;
                    animDouble.BeginTime            = null;
                    animDouble.Duration             = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                    animDouble.RepeatBehavior       = new RepeatBehavior(2);
                    animDouble.AutoReverse          = true;

                    _AC = animDouble.CreateClock();
                    _GY.ApplyAnimationClock(Glyphs.OpacityProperty, _AC);

                    _rootClock = _root.CreateClock();
                    break;

                case "byte":
                    //SCENARIO 3: Byte Animation
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
                    _rootClock = _root.CreateClock();
                    _AC = (AnimationClock)animByte.CreateClock();
                    break;

                case "double2":
                    //SCENARIO 4: Double Animation
                    DoubleAnimation animDouble2 = new DoubleAnimation();
                    animDouble2.From                 = 150d;
                    animDouble2.To                   = -10d;
                    animDouble2.BeginTime            = null;
                    animDouble2.Duration             = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                    animDouble2.RepeatBehavior       = new RepeatBehavior(2);
                    animDouble2.AutoReverse          = true;                              

                    _AC = animDouble2.CreateClock();
                    _GY.ApplyAnimationClock(Glyphs.OriginXProperty, _AC);

                    _rootClock = _root.CreateClock();
                    break;

                case "point":
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

                    _AC = animPoint.CreateClock();
                    LGB.ApplyAnimationClock(LinearGradientBrush.EndPointProperty, _AC);

                    _GY.Fill = LGB;

                    _rootClock = _root.CreateClock();
                    break;

                case "rect":
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

                    _body.Children.Remove(_GY);
                    _body.Children.Add(PT);

                    _rootClock = _root.CreateClock();
                    break;

                case "size":
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

                    _rootClock = _root.CreateClock();
                    break;

                case "vector":
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
                    _rootClock = _root.CreateClock();
                    _AC = (AnimationClock)animVector.CreateClock();
                    break; 

                case "point3d":
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
                    _rootClock = _root.CreateClock();
                    _AC = (AnimationClock)animPoint3D.CreateClock();
                    break;

                case "vector3d":
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
                    _rootClock = _root.CreateClock();
                    _AC = (AnimationClock)animVector3D.CreateClock();
                    break;

                case "pathpoint":
                    //SCENARIO 13: Path Animation
                    PointAnimationUsingPath animPathPoint = new PointAnimationUsingPath();
                    animPathPoint.BeginTime              = null;
                    animPathPoint.Duration               = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                    animPathPoint.RepeatBehavior         = new RepeatBehavior(2);
                    animPathPoint.AutoReverse            = true;                              

                    //hooking up the PointAnimationUsingPath to the Root.
                    _root.Children.Add(animPathPoint);
                    _rootClock = _root.CreateClock();
                    _AC = (AnimationClock)animPathPoint.CreateClock();
                    break;

                case "bool":
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
                    _GY.ApplyAnimationClock(Glyphs.IsEnabledProperty, _AC);

                    _rootClock = _root.CreateClock();
                    break;

                case "char":
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
                    _rootClock = _root.CreateClock();
                    _AC = (AnimationClock)animChar.CreateClock();
                    break;

                case "decimal":
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
                    _rootClock = _root.CreateClock();
                    _AC = (AnimationClock)animDecimal.CreateClock();
                    break;

                case "int16":
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
                    _rootClock = _root.CreateClock();
                    _AC = (AnimationClock)animInt16.CreateClock();
                    break;                                   

                case "int32":
                    //SCENARIO 18: Int32 Animation
                    Int32Animation animInt32 = new Int32Animation();
                    animInt32.From                  = 1;
                    animInt32.To                    = 2;
                    animInt32.BeginTime             = null;
                    animInt32.Duration              = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                    animInt32.RepeatBehavior        = new RepeatBehavior(2);
                    animInt32.AutoReverse           = true;                              

                    _AC = animInt32.CreateClock();
                    _GY.ApplyAnimationClock(Glyphs.BidiLevelProperty, _AC);

                    _rootClock = _root.CreateClock();
                    break;

                case "int64":
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
                    _rootClock = _root.CreateClock();
                    _AC = (AnimationClock)animInt64.CreateClock();
                    break;

                case "pathdouble":
                    //SCENARIO 20: PathDouble Animation

                    //Create a PathFigureCollection.
                    PathFigureCollection PFC1 = SpecialObjects.CreatePathFigureCollection();

                    //Create a PathGeometry: animation.
                    //Assign it a ScaleTransform and PathFigureCollection.
                    ScaleTransform scaleTransform1 = new ScaleTransform();
                    scaleTransform1.ScaleX     = 1.5;
                    scaleTransform1.ScaleY     = 1.5;
                    PathGeometry pathGeometry = new PathGeometry();
                    pathGeometry.Transform   = scaleTransform1;
                    pathGeometry.Figures     = PFC1;

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
                    //and a new TextBox to it.
                    Decorator TD2 = new Decorator();

                    _AC = animPathDouble.CreateClock();
                    TT.ApplyAnimationClock(TranslateTransform.XProperty, _AC);

                    TD2.LayoutTransform = TT;

                    TextBox GY2 = new TextBox();
                    GY2.Height    = 20;
                    GY2.Width     = 120;
                    GY2.Text      = "DoubleAnimationUsingPath";
                    TD2.Child     = GY2;
                    _body.Children.Add(TD2);

                    _body.Children.Remove(_GY);
                    TD2.Child = _GY;

                    _rootClock = _root.CreateClock();
                    break;

                case "pathdouble2":
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
                    _GY.ApplyAnimationClock(Glyphs.OriginYProperty, _AC);

                    _rootClock = _root.CreateClock();
                    break;

                case "single":
                    //SCENARIO 22: Single Animation
                    SingleAnimation animSingle = new SingleAnimation();
                    animSingle.BeginTime            = null;
                    animSingle.Duration             = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                    animSingle.RepeatBehavior       = new RepeatBehavior(2);
                    animSingle.AutoReverse          = true;                              

                    //hooking up the SingleAnimation to the Root.
                    _root.Children.Add(animSingle);
                    _rootClock = _root.CreateClock();
                    _AC = (AnimationClock)animSingle.CreateClock();

                    break;

                    case "string":
                    //SCENARIO 23: String Animation
                    StringAnimationUsingKeyFrames animString = new StringAnimationUsingKeyFrames();

                    StringKeyFrameCollection SKFC = new StringKeyFrameCollection();
                    SKFC.Add(new DiscreteStringKeyFrame("aaaaaaaaaaaa!",KeyTime.FromPercent(0f)));
                    SKFC.Add(new DiscreteStringKeyFrame("bbbbbbbbbbbb!",KeyTime.FromPercent(0.5f)));
                    SKFC.Add(new DiscreteStringKeyFrame("cccccccccccc!", KeyTime.FromPercent(0.9f)));
                    animString.KeyFrames = SKFC;

                    animString.BeginTime            = null;
                    animString.Duration             = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                    animString.RepeatBehavior       = new RepeatBehavior(2);
                    animString.AutoReverse          = true;

                    _AC = animString.CreateClock();
                    _GY.ApplyAnimationClock(Glyphs.UnicodeStringProperty, _AC);

                    _rootClock = _root.CreateClock();
                    break;

                case "thickness":
                    //SCENARIO 24: Thickness Animation
                    ThicknessAnimation animThickness = new ThicknessAnimation();

                    animThickness.BeginTime = null;
                    animThickness.Duration          = new Duration(TimeSpan.FromMilliseconds(_DURATION));
                    animThickness.By                = new Thickness(9, 9, 9, 9);
                    animThickness.RepeatBehavior    = new RepeatBehavior(2);
                    animThickness.AutoReverse       = true;

                    _GY.Margin = new Thickness(1, 1, 1, 1);

                    _AC = animThickness.CreateClock();
                    _GY.ApplyAnimationClock(Glyphs.MarginProperty, _AC);

                    _rootClock = _root.CreateClock();
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
            AttachHandlersClock(_AC);
            _rootClock.Controller.Begin();
            _AC.Controller.Begin();

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          AttachHandlers
        ******************************************************************************/
        /// <summary>
        /// AttachHandlers: Associate Animation events with the Animation.
        /// </summary>
        /// <returns>A Timeline with attached events</returns>
        private Clock AttachHandlersClock(Clock clock)
        {
            clock.CurrentStateInvalidated       += new EventHandler(OnCurrentStateInvalidated);
            clock.CurrentGlobalSpeedInvalidated += new EventHandler(OnCurrentGlobalSpeedInvalidated);
            clock.CurrentTimeInvalidated        += new EventHandler(OnCurrentTimeInvalidated);
            clock.CurrentTimeInvalidated        += new EventHandler(OnCurrentTimeInvalidatedRepeat);

            return clock;
        }

        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {
            CommonEvents.CurrentStateInvalidatedHandler(sender);
        }

        private void OnCurrentTimeInvalidated(object sender, EventArgs e)
        {
            CommonEvents.CurrentTimeInvalidatedHandler(sender);
        }

        private void OnCurrentGlobalSpeedInvalidated(object sender, EventArgs e)
        {               
            CommonEvents.CurrentGlobalSpeedInvalidatedHandler(sender);
        }

        public void OnCurrentTimeInvalidatedRepeat(object sender, EventArgs args)
        {                             
            CommonEvents.CurrentTimeInvalidatedRepeatHandler(sender);
        }
          
        /******************************************************************************
        * Function:          OnCurrentStateTL2
        ******************************************************************************/
        /// <summary>
        /// OnCurrentStateTL2: Controls invocation of Animation methods, which fire events
        /// </summary>
        /// <returns></returns>
        private void OnCurrentStateTL2(object sender, EventArgs e)
        {     
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
        /// OnCurrentStateTL1: Verifies Event firing
        /// </summary>
        /// <returns></returns>
        private void OnCurrentStateTL1(object sender, EventArgs e)
        {
            if (((Clock)sender).CurrentState != ClockState.Active)
            {
                GlobalLog.LogStatus("---OnCurrentStateTL1---");

                int expBegunCount        = 1;
                int expEndedCount        = 1;
                int expChangedCount      = 10;  //Will check for greater than this.
                int expPausedCount       = 1;
                int expRepeatedCount     = 1;
                int expResumedCount      = 1;

                int expReversedCount     = 2;

                GlobalLog.LogEvidence("Begun Count-    Actual: " + CommonEvents.begunCount    + " / Expected: " + expBegunCount);
                GlobalLog.LogEvidence("Ended Count-    Actual: " + CommonEvents.endedCount    + " / Expected: " + expEndedCount);
                GlobalLog.LogEvidence("Changed Count-  Actual: " + CommonEvents.changedCount  + " / Expected: >" + expChangedCount);
                GlobalLog.LogEvidence("Paused Count-   Actual: " + CommonEvents.pausedCount   + " / Expected: " + expPausedCount);
                GlobalLog.LogEvidence("Repeated Count- Actual: " + CommonEvents.repeatedCount + " / Expected: " + expRepeatedCount);
                GlobalLog.LogEvidence("Resumed Count-  Actual: " + CommonEvents.resumedCount  + " / Expected: " + expResumedCount);
                GlobalLog.LogEvidence("Reversed Count- Actual: " + CommonEvents.reversedCount + " / Expected: " + expReversedCount);

                bool pass1 = (CommonEvents.begunCount          == expBegunCount);
                bool pass2 = (CommonEvents.endedCount          == expEndedCount);
                bool pass3 = (CommonEvents.changedCount        > expPausedCount);
                bool pass4 = (CommonEvents.pausedCount         == expPausedCount);
                bool pass5 = (CommonEvents.repeatedCount       == expRepeatedCount);
                bool pass6 = (CommonEvents.resumedCount        == expResumedCount);
                bool pass7 = (CommonEvents.reversedCount       == expReversedCount);

                bool testPassed = (pass1 && pass2 && pass3 && pass4 && pass5 && pass6 && pass7);

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
