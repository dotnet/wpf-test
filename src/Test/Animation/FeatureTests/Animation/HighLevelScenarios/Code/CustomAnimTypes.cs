// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* ***************  MIL Integration Test *****************
*     Description:
*          Verify using a Custom Animation to animate different Animation Types.
*     Major Actions:
*          (a) Create a new window.
*          (b) Initiate a custom animation; the parameter passed to this application indicates which one.
*          (c) Validate the ending value of the animation.
*     Pass Conditions:
*          The test passes if the actual values after the animation match the expected values.
*     How verified:
*          (a) The AnimationClock's GetCurrentValue is checked after the animation is completed.
*          (b) The animated DP's value is also checked.
*
*     Framework:          An executable is created.
*     Area:               Animation/Timing
*     Dependencies:       TestRuntime.dll
*     Support Files:
**********************************************************/
using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
     
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.HighLevelScenarios.Extensibility</area>
    /// <priority>3</priority>
    /// <description>
    /// Verify Custom Animations for different Animation Types.
    /// </description>
    /// </summary>
    [Test(3, "Animation.HighLevelScenarios.Extensibility", "CustomAnimTypesTest")]


    /******************************************************************************
    *******************************************************************************
    * CLASS:          CustomAnimTypesTest
    *******************************************************************************
    ******************************************************************************/
    class CustomAnimTypesTest : WindowTest
    {
        #region Test case members
        
        public  static string               inputString         = "";
        private AnimationClock              _AC                  = null;
        private Canvas                      _body                = null;
        private Button                      _button1             = null;
        private SolidColorBrush             _SCB                 = null;
        private EllipseGeometry             _ellipseGeometry     = null;
        private Viewport3D                  _viewport3D          = null;
        private PointLight                  _light               = null;
        private ArcSegment                  _AS                  = null;
        private Popup                       _popupControl        = null;
        private bool                        _baseBoolean         = true;
        private Color                       _baseColor           = Colors.DodgerBlue;
        private double                      _baseDouble          = 10d;
        private int                         _baseInt32           = 5;
        private Point                       _basePoint           = new Point(0,0);
        private Point3D                     _basePoint3D         = new Point3D(1,1,1);
        private Rect                        _baseRect            = new Rect(1,1,1,1);
        private Size                        _baseSize            = new Size(50, 50);
        private Thickness                   _baseThickness       = new Thickness(0,0,0,0);
        private TimeSpan                    _ANIM_DURATION       = TimeSpan.FromMilliseconds(3000);
        private bool                        _testPassed          = false;

        #endregion


        #region Constructor
        
        [Variation("Boolean", Priority=0)]
        [Variation("Color")]
        [Variation("DoubleWidth")]
        [Variation("Int32")]
        [Variation("DoubleSCBOpacity")]
        [Variation("Rect")]
        [Variation("Point")]
        [Variation("Size")]
        [Variation("Thickness")]
        [Variation("Point3D")]                  

        /******************************************************************************
        * Function:          CustomAnimTypesTest Constructor
        ******************************************************************************/
        public CustomAnimTypesTest(string testValue)
        {
            inputString = testValue;
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(Animate);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region CustomAnimTest Steps

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

            Window.Name             = "Window";
            Window.Height           = 400d;
            Window.Width            = 400d;
            Window.Top              = 0d;
            Window.Left             = 0d;
            Window.ContentRendered  += new EventHandler(OnContentRendered);

            AddControls();
            
            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          AddControls
        ******************************************************************************/
        /// <summary>
        /// Add elements to the window.
        /// </summary>
        /// <returns></returns>
        private void AddControls()
        {
            GlobalLog.LogStatus("---Add controls to the window---");

            _body = new Canvas();
            _body.Name          = "body";
            _body.Height        = 300d;
            _body.Width         = 300d;
            _body.Background    = Brushes.MediumSlateBlue;

            _button1 = new Button();
            _body.Children.Add(_button1);
            _button1.Name        = "button1";
            _button1.Content     = "Avalon!";
            _button1.Width       = 60d;
            _button1.Height      = 30d;
            _button1.Margin      = _baseThickness;
            _button1.IsEnabled   = _baseBoolean;
            Canvas.SetLeft(_button1, 40d);
            Canvas.SetTop(_button1, 100d);

            _SCB = new SolidColorBrush();
            _SCB.Color = _baseColor;
            _button1.Background = _SCB;

            Window.Content = _body;
        }


        /******************************************************************************
        * Function:          OnContentRendered
        ******************************************************************************/
        /// <summary>
        /// Fires when the content of the window is rendered.
        /// Used to create and begin the Storyboard.
        /// </summary>
        /// <returns></returns>
        private void OnContentRendered(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("---OnContentRendered---");

            Signal("PageRendered", TestResult.Pass);
        }

        /******************************************************************************
        * Function:          Animate
        ******************************************************************************/
        /// <summary>
        /// Animate: Create a Custom Animation and start it.
        /// </summary>
        /// <returns>TestResult</returns>
        TestResult Animate()
        {
            WaitForSignal("PageRendered");

            FrameworkElement    FE           = null;
            DependencyProperty  dp           = null;
            NewCustomAnimation2 customAnim   = null;

            bool animationCreated = CreateAnimation(ref FE, ref dp, ref customAnim);

            if (animationCreated)
            {
                switch (inputString)
                {
                    case "DoubleSCBOpacity" :
                        AnimationClock clock = customAnim.CreateClock();
                        _SCB.ApplyAnimationClock(dp, clock);
                        break;

                    case "Color" :
                        _SCB.BeginAnimation(dp, customAnim);
                        break;

                    case "Point" :
                        _ellipseGeometry.BeginAnimation(dp, customAnim);
                        break;

                    case "Point3D" :
                        _light.BeginAnimation(dp, customAnim);
                        break;

                    case "Size" :
                        _AS.BeginAnimation(dp, customAnim);
                        break;

                    default :
                        FE.BeginAnimation(dp, customAnim);
                        break;
                }
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }
        
        /******************************************************************************
        * Function:          CreateAnimation
        ******************************************************************************/
        /// <summary>
        /// CreateAnimation: Establish a Storyboard containing an Animation.
        /// </summary>
        /// <returns>A boolean, indicating whether or not an Animation was created</returns>
        bool CreateAnimation(ref FrameworkElement FE, ref DependencyProperty dp, ref NewCustomAnimation2 customAnim)
        {
            GlobalLog.LogStatus("---CreateAnimation---");

            bool animCreated = true;

            customAnim = new NewCustomAnimation2();
            customAnim.Duration  = new Duration(_ANIM_DURATION);
            customAnim.CurrentStateInvalidated       += new EventHandler(OnCurrentStateInvalidated);

            switch (inputString)
            {
                case "Boolean":
                    dp = Button.IsEnabledProperty;
                    FE = (FrameworkElement)_button1;
                    break;
                case "Color":
                    dp = SolidColorBrush.ColorProperty;
                    FE = (FrameworkElement)_button1;            //Containing element.
                    break;
                case "DoubleSCBOpacity":
                    dp = SolidColorBrush.OpacityProperty;
                    FE = (FrameworkElement)_button1;            //Containing element.
                    break;
                case "DoubleWidth":
                    _button1.Width = 60d;
                    dp = Button.WidthProperty;
                    FE = (FrameworkElement)_button1;
                    break;
                case "Int32":
                    dp = Button.TabIndexProperty;
                    FE = (FrameworkElement)_button1;
                    break;
                case "Point":
                    _ellipseGeometry = new EllipseGeometry();
                    _ellipseGeometry.RadiusX               = 30d;
                    _ellipseGeometry.RadiusY               = 30d;
                    _ellipseGeometry.Center                = new Point(5,5);
                    _button1.Clip = _ellipseGeometry;

                    dp = EllipseGeometry.CenterProperty;
                    FE = (FrameworkElement)_button1;            //Containing element.
                    break;
                case "Point3D":
                    _viewport3D = new Viewport3D();
                    _body.Children.Add(_viewport3D);
                    _viewport3D.ClipToBounds = true;
                    _viewport3D.Height       = 150d;
                    _viewport3D.Width        = 150d;

                    _light = new PointLight();
                    _light.ConstantAttenuation   = 1;
                    _light.LinearAttenuation     = 0;
                    _light.QuadraticAttenuation  = 0;
                    _light.Range                 = 100;

                    MeshGeometry3D mesh1 = SpecialObjects.Sphere(20, 20, 1.2d);
                    Material mat1 = new DiffuseMaterial( Brushes.Gray );
                    GeometryModel3D primitive1 = new GeometryModel3D( mesh1, mat1 );

                    Model3DGroup mg = new Model3DGroup();
                    mg.Children.Add( _light );
                    mg.Children.Add( primitive1 );
                    ModelVisual3D visual = new ModelVisual3D();
                    visual.Content = mg;
                    _viewport3D.Children.Clear();
                    _viewport3D.Children.Add( visual );
                    PerspectiveCamera camera = new PerspectiveCamera(
                                                    new Point3D( 0, 0, 5 ),     // Position
                                                    new Vector3D( 0, 0, -1 ),   // LookDirection
                                                    new Vector3D( 0, 1, 0 ),    // UpDirection
                                                    45                          // FieldOfView
                                                    );
                    camera.NearPlaneDistance = 1;
                    camera.FarPlaneDistance = 20;
                    _viewport3D.Camera = camera;

                    dp = PointLight.PositionProperty;
                    FE = (FrameworkElement)_viewport3D;            //Containing element.
                    break;
                case "Rect":
                    _popupControl  = new Popup();
                    _popupControl.IsOpen             = true;
                    _popupControl.Placement          = PlacementMode.Top;
                    _popupControl.PlacementRectangle = _baseRect;
                    _body.Children.Remove(_button1);
                    _popupControl.Child              = _button1;
                    _body.Children.Add(_popupControl);

                    dp = Popup.PlacementRectangleProperty;
                    FE = (FrameworkElement)_popupControl;
                    break;
                case "Size":
                    Path path1 = new Path();
                    path1.Fill                = Brushes.DarkMagenta;
                    path1.StrokeThickness     = 4;
                    path1.Stroke              = Brushes.MediumSlateBlue;

                    _AS = new ArcSegment ();
                    _AS.IsLargeArc       = true;
                    _AS.Size             = _baseSize;
                    _AS.SweepDirection   = SweepDirection.Counterclockwise;
                    _AS.RotationAngle    = 50;

                    PathFigure PF1 = new PathFigure ();
                    PF1.StartPoint = new Point(30, 150);
                    PF1.Segments.Add(_AS);

                    PathGeometry PG = new PathGeometry ();
                    PG.Figures.Add(PF1);

                    path1.Data = PG;
                    _body.Children.Add(path1);

                    dp = ArcSegment.SizeProperty;
                    FE = (FrameworkElement)path1;            //Containing element.
                    break;
                case "Thickness":
                    dp = Button.MarginProperty;
                    FE = (FrameworkElement)_button1;
                    break;
                default:
                    GlobalLog.LogEvidence("ERROR!!! CreateAnimation: Incorrect argument (1).");
                    animCreated = false;
                    break;
            }
            return animCreated;
        }

        /******************************************************************************
        * Function:          OnCurrentStateInvalidated
        ******************************************************************************/
        /// <summary>
        /// OnCurrentStateInvalidated: Calls verification routines.
        /// </summary>
        /// <returns></returns>
        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {
            GlobalLog.LogEvidence("---OnCurrentStateInvalidated--- " + ((Clock)sender).CurrentState);

            if ( ((AnimationClock)sender).CurrentState != ClockState.Active )
            {
                _AC = ((AnimationClock)sender);

                Signal("EventFired", TestResult.Pass);
            }
        }

        /******************************************************************************
        * Function:          Verify
        ******************************************************************************/
        /// <summary>
        /// Verify: Verifies the Custom Animation.
        /// </summary>
        /// <returns>TestResult</returns>
        TestResult Verify()
        {
            WaitForSignal("EventFired");

            switch (inputString)
            {
                case "Boolean":
                    Boolean expectedBoolean  = false;
                    Boolean actualBooleanGCV = (Boolean)_AC.GetCurrentValue(_baseBoolean, _baseBoolean);
                    Boolean actualBoolean    = _button1.IsEnabled;

                    GlobalLog.LogEvidence("------------------------------");
                    GlobalLog.LogEvidence("--Expected               : " + expectedBoolean);
                    GlobalLog.LogEvidence("--Actual GetCurrentValue : " + actualBooleanGCV);
                    GlobalLog.LogEvidence("--Actual Value           : " + actualBoolean);
                    GlobalLog.LogEvidence("------------------------------");

                    _testPassed = ((actualBooleanGCV == expectedBoolean) && (actualBoolean == expectedBoolean));
                    break;
                case "Color":
                    Color expectedColor  = Colors.Red;
                    Color actualColorGCV = (Color)_AC.GetCurrentValue(_baseColor, _baseColor);
                    Color actualColor    = _SCB.Color;

                    GlobalLog.LogEvidence("------------------------------");
                    GlobalLog.LogEvidence("--Expected               : " + expectedColor);
                    GlobalLog.LogEvidence("--Actual GetCurrentValue : " + actualColorGCV);
                    GlobalLog.LogEvidence("--Actual Value           : " + actualColor);
                    GlobalLog.LogEvidence("------------------------------");

                    float tolerance  = 0.001f;
                    _testPassed = AnimationUtilities.CompareColors(expectedColor, actualColor, tolerance);
                    _testPassed = AnimationUtilities.CompareColors(expectedColor, actualColorGCV, tolerance) && _testPassed;
                    break;
                case "DoubleSCBOpacity":
                    double expectedOpacity  = 0.75d;
                    double actualOpacityGCV = (double)_AC.GetCurrentValue(_baseDouble, _baseDouble);
                    double actualOpacity    = _SCB.Opacity;

                    GlobalLog.LogEvidence("------------------------------");
                    GlobalLog.LogEvidence("--Expected               : " + expectedOpacity);
                    GlobalLog.LogEvidence("--Actual GetCurrentValue : " + actualOpacityGCV);
                    GlobalLog.LogEvidence("--Actual Value           : " + actualOpacity);
                    GlobalLog.LogEvidence("------------------------------");

                    _testPassed = ((actualOpacityGCV == expectedOpacity) && (actualOpacity == expectedOpacity));
                    break;
                case "DoubleWidth":
                    double expectedWidth  = 240d;
                    double actualWidthGCV = (double)_AC.GetCurrentValue(_baseDouble, _baseDouble);
                    double actualWidth    = _button1.Width;

                    GlobalLog.LogEvidence("------------------------------");
                    GlobalLog.LogEvidence("--Expected               : " + expectedWidth);
                    GlobalLog.LogEvidence("--Actual GetCurrentValue : " + actualWidthGCV);
                    GlobalLog.LogEvidence("--Actual Value           : " + actualWidth);
                    GlobalLog.LogEvidence("------------------------------");

                    _testPassed = ((actualWidthGCV == expectedWidth) && (actualWidth == expectedWidth));
                    break;
                case "Int32":
                    int expectedInt32  = 100;
                    int actualInt32GCV = (int)_AC.GetCurrentValue(_baseInt32, _baseInt32);
                    int actualInt32    = _button1.TabIndex;

                    GlobalLog.LogEvidence("------------------------------");
                    GlobalLog.LogEvidence("--Expected               : " + expectedInt32);
                    GlobalLog.LogEvidence("--Actual GetCurrentValue : " + actualInt32GCV);
                    GlobalLog.LogEvidence("--Actual Value           : " + actualInt32);
                    GlobalLog.LogEvidence("------------------------------");

                    _testPassed = ((actualInt32GCV == expectedInt32) && (actualInt32 == expectedInt32));
                    break;
                case "Point":
                    Point expectedPoint  = new Point(25,25);
                    Point actualPointGCV = (Point)_AC.GetCurrentValue(_basePoint, _basePoint);
                    Point actualPoint    = _ellipseGeometry.Center;

                    GlobalLog.LogEvidence("------------------------------");
                    GlobalLog.LogEvidence("--Expected               : " + expectedPoint);
                    GlobalLog.LogEvidence("--Actual GetCurrentValue : " + actualPointGCV);
                    GlobalLog.LogEvidence("--Actual Value           : " + actualPoint);
                    GlobalLog.LogEvidence("------------------------------");

                    _testPassed = ((actualPointGCV == expectedPoint) && (actualPoint == expectedPoint));
                    break;
                case "Point3D":
                    Point3D expectedPoint3D  = new Point3D(2.5, 0, 5);
                    Point3D actualPoint3DGCV = (Point3D)_AC.GetCurrentValue(_basePoint3D, _basePoint3D);
                    Point3D actualPoint3D    = _light.Position;

                    GlobalLog.LogEvidence("------------------------------");
                    GlobalLog.LogEvidence("--Expected               : " + expectedPoint3D);
                    GlobalLog.LogEvidence("--Actual GetCurrentValue : " + actualPoint3DGCV);
                    GlobalLog.LogEvidence("--Actual Value           : " + actualPoint3D);
                    GlobalLog.LogEvidence("------------------------------");

                    _testPassed = ((actualPoint3DGCV == expectedPoint3D) && (actualPoint3D == expectedPoint3D));
                    break;
                case "Rect":
                    Rect expectedRect  = new Rect(20,20,20,20);
                    Rect actualRectGCV = (Rect)_AC.GetCurrentValue(_baseRect, _baseRect);
                    Rect actualRect    = _popupControl.PlacementRectangle;

                    GlobalLog.LogEvidence("------------------------------");
                    GlobalLog.LogEvidence("--Expected               : " + expectedRect);
                    GlobalLog.LogEvidence("--Actual GetCurrentValue : " + actualRectGCV);
                    GlobalLog.LogEvidence("--Actual Value           : " + actualRect);
                    GlobalLog.LogEvidence("------------------------------");

                    _testPassed = ((actualRectGCV == expectedRect) && (actualRect == expectedRect));
                    break;
                case "Size":
                    Size expectedSize  = new Size(100, 100);
                    Size actualSizeGCV = (Size)_AC.GetCurrentValue(_baseSize, _baseSize);
                    Size actualSize    = _AS.Size;

                    GlobalLog.LogEvidence("------------------------------");
                    GlobalLog.LogEvidence("--Expected               : " + expectedSize);
                    GlobalLog.LogEvidence("--Actual GetCurrentValue : " + actualSizeGCV);
                    GlobalLog.LogEvidence("--Actual Value           : " + actualSize);
                    GlobalLog.LogEvidence("------------------------------");

                    _testPassed = ((actualSizeGCV == expectedSize) && (actualSize == expectedSize));
                    break;
                case "Thickness":
                    Thickness expectedThickness  = new Thickness(10,2,10,2);
                    Thickness actualThicknessGCV = (Thickness)_AC.GetCurrentValue(_baseThickness, _baseThickness);
                    Thickness actualThickness    = _button1.Margin;

                    GlobalLog.LogEvidence("------------------------------");
                    GlobalLog.LogEvidence("--Expected               : " + expectedThickness);
                    GlobalLog.LogEvidence("--Actual GetCurrentValue : " + actualThicknessGCV);
                    GlobalLog.LogEvidence("--Actual Value           : " + actualThickness);
                    GlobalLog.LogEvidence("------------------------------");

                    _testPassed = ((actualThicknessGCV == expectedThickness) && (actualThickness == expectedThickness));
                    break;
                default:
                    GlobalLog.LogEvidence("ERROR!!! CreateAnimation: Incorrect argument (4).");
                    _testPassed = false;
                    break;
            }

            if (_testPassed)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }
        #endregion
    }


    /******************************************************************************
    *******************************************************************************
    * CLASS:          NewCustomAnimation2
    *******************************************************************************
    ******************************************************************************/
    // Custom Animation Class:
    public class NewCustomAnimation2 : System.Windows.Media.Animation.AnimationTimeline
    {
        #region Custom case members

        private string      _inString;
        public double       FromDouble;
        public double       ToDouble;
        public Color        FromColor;
        public Color        ToColor;
        public Thickness    FromThickness;
        public Thickness    ToThickness;
        public Rect         FromRect;
        public Rect         ToRect;
        public Boolean      FromBoolean;
        public Boolean      ToBoolean;
        public Point        FromPoint;
        public Point        ToPoint;
        public Point        ByPoint;
        public int          FromInt32;
        public int          ToInt32;
        public int          ByInt32;
        public Point3D      FromPoint3D;
        public Point3D      ToPoint3D;
        public Size         BySize;

        #endregion


        #region Constructor
        public NewCustomAnimation2() : base()
        {
            _inString = CustomAnimTypesTest.inputString;
        }

        public new NewCustomAnimation2 Clone()
        {
            return (NewCustomAnimation2)base.Clone();
        }
        public new NewCustomAnimation2 GetAsFrozen()
        {
            return (NewCustomAnimation2)base.GetAsFrozen();
        }
        #endregion


        #region NewCustomAnimation2 Steps

        protected override void CloneCore( Freezable sourceFreezable)
        {
            NewCustomAnimation2 newCustomAnimation = (NewCustomAnimation2)sourceFreezable;
            base.CloneCore(sourceFreezable);
            SetFromTo(newCustomAnimation);
        }

        protected override void GetAsFrozenCore(Freezable sourceFreezable)
        {
            NewCustomAnimation2 newCustomAnimation = (NewCustomAnimation2)sourceFreezable;
      
            base.GetAsFrozenCore(sourceFreezable);
            SetFromTo(newCustomAnimation);
        }
        protected override void GetCurrentValueAsFrozenCore(Freezable sourceFreezable)
        {
            NewCustomAnimation2 newCustomAnimation = (NewCustomAnimation2)sourceFreezable;

            base.GetCurrentValueAsFrozenCore(sourceFreezable);
            SetFromTo(newCustomAnimation);
        }

        protected override Freezable CreateInstanceCore()
        {
            return new NewCustomAnimation2();
        }

        private void SetFromTo(NewCustomAnimation2 customAnimation)
        {
            switch (_inString)
            {
                case "Boolean":
                    this.FromBoolean            = customAnimation.FromBoolean;
                    this.ToBoolean              = customAnimation.ToBoolean;
                    break;
                case "Color":
                    this.FromColor              = customAnimation.FromColor;
                    this.ToColor                = customAnimation.ToColor;
                    break;
                case "DoubleWidth":
                    this.FromDouble             = customAnimation.FromDouble;
                    this.ToDouble               = customAnimation.ToDouble;
                    break;
                case "Int32":
                    this.FromInt32              = customAnimation.FromInt32;
                    this.ToInt32                = customAnimation.ToInt32;
                    this.ByInt32                = customAnimation.ByInt32;
                    break;
                case "DoubleSCBOpacity":
                    this.FromDouble             = customAnimation.FromDouble;
                    this.ToDouble               = customAnimation.ToDouble;
                    break;
                case "Point":
                    this.FromPoint              = customAnimation.FromPoint;
                    this.ToPoint                = customAnimation.ToPoint;
                    this.ByPoint                = customAnimation.ByPoint;
                    break;
                case "Point3D":
                    this.FromPoint3D            = customAnimation.FromPoint3D;
                    this.ToPoint3D              = customAnimation.ToPoint3D;
                    break;
                case "Rect":
                    this.FromRect               = customAnimation.FromRect;
                    this.ToRect                 = customAnimation.ToRect;
                    break;
                case "Size":
                    this.BySize                 = customAnimation.BySize;
                    break;
                case "Thickness":
                    this.FromThickness          = customAnimation.FromThickness;
                    this.ToThickness            = customAnimation.ToThickness;
                    break;
            }
        }

        /******************************************************************************
        * Function:          TargetPropertyType
        ******************************************************************************/
        /// <summary>
        /// TargetPropertyType: Override, depending on the type of animation.
        /// </summary>
        public override System.Type TargetPropertyType
        {
            get
            {
                switch (CustomAnimTypesTest.inputString)
                {
                    case "Boolean":
                        return typeof( Boolean );
                    case "Color":
                        return typeof( Color );
                    case "DoubleWidth":
                        return typeof( double );
                    case "Int32":
                        return typeof( int );
                    case "DoubleSCBOpacity":
                        return typeof( double );
                    case "Point":
                        return typeof( Point );
                    case "Point3D":
                        return typeof( Point3D );
                    case "Rect":
                        return typeof( Rect );
                    case "Size":
                        return typeof( Size );
                    case "Thickness":
                        return typeof( Thickness );
                    default:
                        return null;
                }
            }
        }

        /******************************************************************************
        * Function:          GetCurrentValueCore
        ******************************************************************************/
        /// <summary>
        /// GetCurrentValueCore: Carry out the custom animation here.
        /// </summary>
        public override object GetCurrentValue( object originValue, object destinationValue, System.Windows.Media.Animation.AnimationClock animClock )
        {
            double lowerBound = .3d;
            double upperBound = .7d;

            switch (CustomAnimTypesTest.inputString)
            {
                case "Boolean":
                    if ( (animClock.CurrentProgress > lowerBound) && (animClock.CurrentProgress < upperBound)  )
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case "Color":
                    if ( (animClock.CurrentProgress > lowerBound) && (animClock.CurrentProgress < upperBound)  )
                    {
                        return Colors.SpringGreen;
                    }
                    else
                    {
                        return Colors.Red;
                    }
                case "DoubleWidth":
                    if ( (animClock.CurrentProgress > lowerBound) && (animClock.CurrentProgress < upperBound)  )
                    {
                        return 120d;
                    }
                    else
                    {
                        return 240d;
                    }
                case "DoubleSCBOpacity":
                    if ( (animClock.CurrentProgress > lowerBound) && (animClock.CurrentProgress < upperBound)  )
                    {
                        return 0.2d;
                    }
                    else
                    {
                        return 0.75d;
                    }
                case "Int32":
                    if ( (animClock.CurrentProgress > lowerBound) && (animClock.CurrentProgress < upperBound)  )
                    {
                        return 2;
                    }
                    else
                    {
                        return 100;
                    }
                case "Point":
                    if ( (animClock.CurrentProgress > lowerBound) && (animClock.CurrentProgress < upperBound)  )
                    {
                        return new Point(250,250);
                    }
                    else
                    {
                        return new Point(25,25);
                    }
                case "Point3D":
                    if ( (animClock.CurrentProgress > lowerBound) && (animClock.CurrentProgress < upperBound)  )
                    {
                        return new Point3D(1,2,3);
                    }
                    else
                    {
                        return new Point3D(2.5, 0, 5);
                    }
                case "Rect":
                    if ( (animClock.CurrentProgress > lowerBound) && (animClock.CurrentProgress < upperBound)  )
                    {
                        return new Rect(0,50,0,50);
                    }
                    else
                    {
                        return new Rect(20,20,20,20);
                    }
                case "Size":
                    if ( (animClock.CurrentProgress > lowerBound) && (animClock.CurrentProgress < upperBound)  )
                    {
                        return new Size(0,0);
                    }
                    else
                    {
                        return new Size(100,100);
                    }
                case "Thickness":
                    if ( (animClock.CurrentProgress > lowerBound) && (animClock.CurrentProgress < upperBound)  )
                    {
                        return new Thickness(0,5,0,5);
                    }
                    else
                    {
                        return new Thickness(10,2,10,2);
                    }
                default:
                    return null;
            }
        }
        #endregion
    }
}
