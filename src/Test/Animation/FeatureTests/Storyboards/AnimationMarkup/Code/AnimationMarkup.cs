// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* ***************  MIL Integration Test ***************************************************
*   Major Actions:
*   Pass Conditions:
*       (a) The actual rendering matches the expected rendering before and after the animation.
*       (b) If "Tag" is specified on the root element, the final value of the animation will be
*           verified, using GetValue.
*   How verified:
*       A VScan utility is used to get the color of two points.
*       The result of the comparisons between actual and expected values is passed to GlobalLog.LogEvidence().
*
*   Requirements:
*       The .xaml files loaded by this .exe must contain the following:
*           -- the Animation must have a BeginTime="0:0:2" and a Duration="0:0:4", in most cases.
*           -- If a Markup file has a Tag property specified on the root element, then an additional
*              test will be performed that checks the final value of the animated property.
*              (a) The Tag value must be of the format:
*                  "AnimationType space ExpectedFinalValue space DependencyPropertyName"
*              (b) The root element in Markup must have Name="Root".
*              (c) An animated FrameworkElement must have Name="AnimatedFE".
*              (d) An animated Animatable must have Name="AnimatedFE". [The associated FE need not have a Name.]
*              (e) Animating an animatable directly (via x:Name on the animatable dp) is supported; it
*                  must have Name="AnimatedAnimatable".
*              (f) Animating inside a ControlTemplate is supported; the animated element must have Name="TemplateControl"
*
*   Limitations [for final value verification]:
*       Only -one- Animation on the page can be verified.
*
*   Framework:          A CLR executable is created.
*   Area:               Animation/Timing
*   Dependencies:       TestRuntime.dll, AnimationFramework.dll
*   Support Files:      VerifyGetValue.cs.
********************************************************************************************/
using System;
using System.Security.Permissions;
using System.Globalization;
using System.Threading;
using System.Drawing;
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
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation</area>
    /// <priority>1</priority>
    /// <description>
    /// Rendering verification of animation within Markup
    /// </description>
    /// </summary>
    [Test(2, "Animation.AnimationMarkup", "AnimationMarkupTest", SupportFiles=@"FeatureTests\Animation\duck.wmv,FeatureTests\Animation\clubtrid.gif")]

    public class AnimationMarkupTest : XamlTest
    {
        #region Test case members

        private VisualVerifier                  _verifier;

        private static string                   s_inputString        = null;
        private int                             _tickCount          = 0;
        private bool                            _valueVerifyNeeded  = false;
        private bool                            _testPassed         = false;
        private string                          _windowTitle        = "Animation Markup Test";
        public  static string                   outputData         = "";
        
        private ImageComparator                 _imageCompare       = new ImageComparator();
        private System.Drawing.Rectangle        _clientRect;
        private static System.Drawing.Bitmap    s_beforeCapture;
        private static System.Drawing.Bitmap    s_betweenCapture;
        private static System.Drawing.Bitmap    s_afterCapture;

        AnimationValidator _myValidator = new AnimationValidator();

        private InternalTimeManager             _hostManager;
        private DispatcherTimer                 _aTimer              = null;
        #endregion


        #region Constructor
        
        //Storyboard Xamls

        // Element 3D integration work - in 3.5 only

        // [DISABLE WHILE PORTING]
        // [Variation(@"Element3D-animatedtransform.xaml", Priority=1, Versions="3.0SP1,3.0SP2,AH")]
        // [Variation(@"Element3D-Clip.xaml", Priority=1, Versions="3.0SP1,3.0SP2,AH")]
        // [Variation(@"Element3D-Container.xaml", Priority=1, Versions="3.0SP1,3.0SP2,AH")]
        // [Variation(@"Element3D-directtarget.xaml", Priority=1, Versions="3.0SP1,3.0SP2,AH")]
        // [Variation(@"Element3D-drilledtarget.xaml", Priority=1, Versions="3.0SP1,3.0SP2,AH")]
        // [Variation(@"Element3D-Geometry.xaml", Priority=1, Versions="3.0SP1,3.0SP2,AH")]
        // [Variation(@"Element3D-Material.xaml", Priority=1, Versions="3.0SP1,3.0SP2,AH")]
        // [Variation(@"Element3D-ModelVisual.xaml", Priority=1, Versions="3.0SP1,3.0SP2,AH")]
        // [Variation(@"Element3D-multianimatedtransforms.xaml", Priority=1, Versions="3.0SP1,3.0SP2,AH")]
        // [Variation(@"Element3D-multiple.xaml", Priority=1, Versions="3.0SP1,3.0SP2,AH")]
        // [Variation(@"Element3D-Positions.xaml", Priority=1, Versions="3.0SP1,3.0SP2,AH")]
        // [Variation(@"Element3D-styled.xaml", Priority=1, Versions="3.0SP1,3.0SP2,AH")]
        // [Variation(@"Element3D-targetcontainer.xaml", Priority=1, Versions="3.0SP1,3.0SP2,AH")]
        // [Variation(@"Element3D-uptarget.xaml", Priority=1, Versions="3.0SP1,3.0SP2,AH")]
        // [Variation(@"Element3D-viewportclip.xaml", Priority=1, Versions="3.0SP1,3.0SP2,AH")]


        [Variation(@"BlockBorderThickness.xaml", Priority=0)]          //Not in Tactics.
        // [DISABLE WHILE PORTING]
        // [Variation(@"BorderThicknessStyle.xaml", Priority=1)]
        [Variation(@"ButtonContentStyle.xaml", Priority=0)]
        [Variation(@"ButtonForegroundColor.xaml", Priority=0)]
        [Variation(@"ButtonRGBCenter.xaml", Priority=1)]
        [Variation(@"ButtonRotate.xaml", Priority=0)]
        [Variation(@"ButtonRotateStyle.xaml", Priority=0)]
        [Variation(@"ButtonSkewXY.xaml", Priority=0)]
        [Variation(@"CanvasBackgroundColorStyle.xaml", Priority=0)]
        [Variation(@"CanvasHeight.xaml", Priority=1)]
        [Variation(@"CanvasRadialClip.xaml", Priority=0)]
        [Variation(@"CheckBoxIsChecked.xaml", Priority=1, Disabled=true)]
        // [DISABLE WHILE PORTING]
        // [Variation(@"ComboBoxBackground.xaml", Priority=0)]
        // [Variation(@"ComboBoxMaxDropDownHeight.xaml", Priority=1)]
        [Variation(@"ComboBoxScaleX.xaml", Priority=1)]
        [Variation(@"ComboBoxSelectedIndex.xaml", Priority = 0, Keywords = "MicroSuite")]
        [Variation(@"DrawingBrushViewport.xaml", Priority=1)]       //Not in Tactics. 
        [Variation(@"EllipseDrawingBrushViewport.xaml", Priority=1)]
        [Variation(@"EllipseFillColor.xaml", Priority=1)]
        // [DISABLE WHILE PORTING]
        // [Variation(@"EllipseGeometryClip.xaml", Priority=1)]
        [Variation(@"EllipseGeometryClipCenter.xaml")]  
        [Variation(@"EllipseRotateTransformCenter.xaml")]
        [Variation(@"FlowDocForeground.xaml")]          
        [Variation(@"GlyphsOriginXY.xaml")]             
        [Variation(@"GridViewLoaded.xaml")]             
        [Variation(@"ImageOpacity.xaml", Priority=0)]   
        [Variation(@"KeyFramesOutOfOrder.xaml")]        
        [Variation(@"KeyTimePaced.xaml")]               
        // [DISABLE WHILE PORTING]
        // [Variation(@"LinearGradientBrushOpacity.xaml")] 
        [Variation(@"ListBoxGradientStopColor.xaml", Priority=1)]
        [Variation(@"ListBoxItemTextOpacity.xaml", Priority=0)]
        [Variation(@"ListBoxOpacityStyle.xaml", Priority=1)]
        [Variation(@"ListBoxTranslateTransformYStyle.xaml", Priority=1)]
        [Variation(@"MenuBackgroundStyle.xaml", Priority=1)]
        [Variation(@"MenuItemForegroundStyle.xaml", Priority=1)]
        [Variation(@"MenuItemHeaderStyle.xaml", Priority=1)]
        [Variation(@"MenuItemIsCheckedStyle.xaml", Priority=1, Disabled=true)]
        // [DISABLE WHILE PORTING]
        // [Variation(@"PathCombinedGeometry.xaml", Priority=1)]
        [Variation(@"PathCombinedGeometryStyle.xaml", Priority=1)]
        [Variation(@"PathGeometryMatrix.xaml", Priority = 0, Keywords = "MicroSuite")]         
        [Variation(@"PathLineGeometryStyle.xaml", Priority=1)]
        // [DISABLE WHILE PORTING]
        // [Variation(@"PathPathGeometry.xaml", Priority=1)]
        // [Variation(@"PathPathGeometryStyle.xaml", Priority=1)]
        [Variation(@"PolygonOpacityStyle.xaml", Priority=1)]
        [Variation(@"RadialGradientBrushOpacity.xaml", Priority=0)]  
        [Variation(@"RectangleMarginStyle.xaml", Priority=1)]
        [Variation(@"ScrollViewerHeight.xaml", Priority=1)]
        [Variation(@"ScrollViewerRadial.xaml", Priority=1)]
        [Variation(@"SpeedRatioVideoRender1.xaml", Disabled=true)]     
        [Variation(@"SpeedRatioVideoRender2.xaml", Disabled=true)]    
        // [DISABLE WHILE PORTING] 
        // [Variation(@"SVTranslateTransformXY.xaml")]
        [Variation(@"TextBlockTranslateTransformX.xaml", Priority=0)]

        //Misc.
        [Variation(@"ColumnSpanInt32.xaml", Priority=1)]
        // [DISABLE WHILE PORTING]
        // [Variation(@"ControlTemplateKF1.xaml", Priority=0)]
        // [Variation(@"ControlTemplateKF2.xaml", Priority=0)]
        // [Variation(@"DashStyleOffset1.xaml", Priority=0)]
        [Variation(@"DashStyleOffset2.xaml")]
        [Variation(@"DataBindColorAnim1.xaml", Priority=1)]
        [Variation(@"DataBindColorAnim2.xaml", Priority=0)]
        [Variation(@"DataBindColorAnim3.xaml")]
        [Variation(@"DataBindColorAnim5.xaml")]
        [Variation(@"DataBindColorAnim6.xaml")]
        [Variation(@"DataBindColorAnim7.xaml")]
        [Variation(@"DataBindDoubleAnim1.xaml", Priority=0)]
        [Variation(@"DataBindDoubleAnim2.xaml")]
        [Variation(@"DataBindDoubleAnim7.xaml")]
        // [DISABLE WHILE PORTING]
        // [Variation(@"DataBindMultiple1.xaml", Priority=0)]
        [Variation(@"DataBindMultiple2.xaml")]
        // [DISABLE WHILE PORTING]
        // [Variation(@"DataBindMultiple3.xaml", Priority=0)]
        [Variation(@"DataBindMultiple4.xaml", Priority=0)]
        [Variation(@"DataBindPointAnim1.xaml")]
        // [DISABLE WHILE PORTING]
        // [Variation(@"DataBindPointAnim5.xaml")]
        [Variation(@"DataBindRectAnim1.xaml", Priority=0)]
        [Variation(@"DataBindThicknessAnim1.xaml")]
        [Variation(@"DesiredFrameRate1.xaml", Priority=0)]
        [Variation(@"DirectTargeting1.xaml", Priority=1)]
        [Variation(@"DirectTargeting2.xaml", Priority=1)]
        [Variation(@"DirectTargeting3.xaml", Priority=0)]
        [Variation(@"DirectTargeting4.xaml", Priority=1)]
        [Variation(@"DirectTargeting5.xaml", Priority=0)]
        // [DISABLE WHILE PORTING]
        // [Variation(@"EventAndPropertyTriggers1.xaml", Priority=0)]
        // [Variation(@"EventAndPropertyTriggers2.xaml", Priority=1)]
        [Variation(@"GeometryGroup.xaml", Priority=1)]
        // [Variation(@"Layout1.xaml", Priority=0)]
        [Variation(@"LayoutTransform1.xaml", Priority=0)]
        [Variation(@"LayoutTransform2.xaml", Disabled=true)]
        // [Variation(@"MultipleBegins.xaml", Priority=0)]
        [Variation(@"ObjectAnimation1.xaml", Priority=1)]
        [Variation(@"ObjectAnimation2.xaml", Priority=0)]
        // [Variation(@"ObjectAnimation3.xaml", Priority=1)]
        [Variation(@"ObjectAnimation4.xaml", Priority=0)]
        [Variation(@"ObjectAnimation5.xaml", Priority=1)]
        // [Variation(@"ObjectAnimation7.xaml", Priority=1)]
        [Variation(@"ResourceDynamicStoryboard1.xaml", Priority=0)]
        [Variation(@"ResourceDynamicStoryboard2.xaml")]
        [Variation(@"ResourceDynamicStoryboard3.xaml")]
        // [Variation(@"ResourceDynamicStoryboard4.xaml", Priority=2)]
        [Variation(@"ResourceDynamicStoryboard5.xaml", Priority=0)]
        [Variation(@"ResourceDynamicStoryboard6.xaml")]
        [Variation(@"ResourceDynamicStoryboard7.xaml")]
        [Variation(@"ResourceDynamicStoryboard8.xaml", Priority=0)]
        [Variation(@"ResourceDynamicStoryboard9.xaml")]
        [Variation(@"ResourceDynamicStyle1.xaml")]
        [Variation(@"ResourceStaticStoryboard1.xaml", Priority=1)]
        [Variation(@"ResourceStaticStoryboard2.xaml")]
        [Variation(@"ResourceStaticStoryboard3.xaml")]
        // [Variation(@"ResourceStaticStoryboard4.xaml")]
        [Variation(@"ResourceStaticStoryboard5.xaml")]
        [Variation(@"ResourceStaticStoryboard6.xaml")]
        [Variation(@"ResourceStaticStoryboard7.xaml")]
        [Variation(@"ResourceStaticStoryboard8.xaml")]
        [Variation(@"ResourceStaticStoryboard9.xaml")]
        [Variation(@"ResourceStyle1.xaml", Priority=0)]
        [Variation(@"ResourceStyle2.xaml")]
        [Variation(@"ResourceStyle3.xaml", Priority=0)]
        [Variation(@"ResourceStyle4.xaml", Priority=0)]
        [Variation(@"ResourceStyle5.xaml", Priority=1)]
        [Variation(@"SizeAnim.xaml", Priority=1)]
        // [Variation(@"ToAnimation.xaml")]                               
        [Variation(@"TabControlSelection.xaml")]                       

        //DataBinding.
        // [Variation(@"bind1.xaml", Priority=0)]
        [Variation(@"bind2.xaml")]
        [Variation(@"bind3.xaml", Priority=0)]
        [Variation(@"bind4.xaml")]
        [Variation(@"bind5.xaml")]
        [Variation(@"bind6.xaml")]
        [Variation(@"bind7.xaml", Priority=1)]
        // [Variation(@"bind8.xaml", Priority=0)]
        [Variation(@"bind9.xaml", Priority=0)]
        [Variation(@"bind10.xaml", Priority=0)]
        [Variation(@"bind11.xaml", Priority=2)]
        // [Variation(@"bind12.xaml", Priority=0)]
        [Variation(@"bind13.xaml", Disabled=true)]

        //Editing.
        [Variation(@"EditingAnimations.xaml", Priority=0)]
        [Variation(@"EditingColorAnim.xaml")]
        // [DISABLE WHILE PORTING]
        // [Variation(@"EditingFlyEffectPW.xaml")]
        // [Variation(@"EditingFlyEffectRTB.xaml")]
        // [Variation(@"EditingFlyEffectTB.xaml", Priority=0)]
        // [Variation(@"EditingFontSizeAnimPW.xaml", Priority=0)]
        // [Variation(@"EditingFontSizeAnimRTB.xaml")]
        // [Variation(@"EditingFontSizeAnimTB.xaml")]
        [Variation(@"EditingGradientStopAnimPW.xaml")]
        [Variation(@"EditingGradientStopAnimRTB.xaml")]
        [Variation(@"EditingGradientStopAnimTB.xaml")]
        // [DISABLE WHILE PORTING]
        // [Variation(@"EditingMaxLinesAnim.xaml")]
        // [Variation(@"EditingMinLinesAnim.xaml", Priority=0)]
        // [Variation(@"EditingOpacityAnimPW.xaml", Priority=0)]
        // [Variation(@"EditingOpacityAnimRTB.xaml")]
        // [Variation(@"EditingOpacityAnimTB.xaml")]
        // [Variation(@"EditingRotateAnimPW.xaml")]
        // [Variation(@"EditingRotateAnimRTB.xaml")]
        // [Variation(@"EditingRotateAnimTB.xaml")]
        // [Variation(@"EditingSizeAnimPW.xaml")]
        // [Variation(@"EditingSizeAnimRTB.xaml")]
        // [Variation(@"EditingSizeAnimTB.xaml")]

        //Layout.
        [Variation(@"Background.xaml", Priority=0)]
        [Variation(@"Border_Padding_BorderThickness.xaml")]
        // [DISABLE WHILE PORTING]
        // [Variation(@"Canvas_Position.xaml")]
        // [Variation(@"Grid_Column_Row.xaml")]
        // [Variation(@"Grid_Column_Row_Span.xaml")]
        [Variation(@"ItemWidthHeight_inWrapPanel.xaml")]
        [Variation(@"LayoutTransform.xaml", Priority=0)]
        [Variation(@"LayoutTransform_Rotate_inGrid.xaml")]
        [Variation(@"LayoutTransform_Scale_inStackPanel.xaml")]
        [Variation(@"LayoutTransform_Scaling_Trigger.xaml")]
        [Variation(@"LayoutTransform_Skew_inScrollViewer.xaml")]
        [Variation(@"Resizing_Viewbox.xaml")]
        [Variation(@"Width_Height.xaml", Priority=0)]
        
        //Tablet.
        [Variation(@"AnimatedInkCanvasChild.xaml")]
        [Variation(@"InkCanvasBackgroundAnimation.xaml")]
        // [Variation(@"MultipleInkPresenters.xaml")]
        [Variation(@"RotatedInkCanvas.xaml")]
        // [Variation(@"SkewedInkPresenter.xaml")]
        [Variation(@"ZoomedInkPresenter.xaml")]
        
        //TextEffect.
        [Variation(@"TERotateTrigger.xaml")]
        [Variation(@"TETriggerClip.xaml")]
        // [Variation(@"TETriggerColor.xaml", Priority=0)]
        [Variation(@"TETriggerKeyframeRotate.xaml")]
        [Variation(@"TETriggerRotate.xaml")]
        [Variation(@"TETriggerScale.xaml", Priority = 0, Keywords = "MicroSuite")]
        [Variation(@"TETriggerSkew.xaml")]
        [Variation(@"TETriggerTranslate.xaml")]
        [Variation(@"TextEffectBlinds.xaml", Priority=0)]
        [Variation(@"TextEffectClip.xaml")]
        [Variation(@"TextEffectColor.xaml")]
        [Variation(@"TextEffectElegant.xaml")]
        [Variation(@"TextEffectMixed.xaml")]
        // [Variation(@"TextEffectTransform.xaml")]
        
        //BVTs.
        // [Variation(@"ArcSegment.xaml", Priority=0)]
        [Variation(@"BezierSegment.xaml", Priority=0)]
        [Variation(@"ColorAccelBVT.xaml", Priority=0)]
        [Variation(@"ColorDecelBVT.xaml", Priority=0)]      //Not in Tactics.
        [Variation(@"ColorKeySplineBVT.xaml", Priority=0)]
        [Variation(@"ColorSpeedBVT.xaml", Priority=0)]
        [Variation(@"DoubleAccelBVT.xaml", Priority=0)]
        [Variation(@"DoubleDecelBVT.xaml", Priority=0)]
        [Variation(@"DoubleKeySplineBVT.xaml", Priority=0)]
        [Variation(@"DoubleSpeedBVT.xaml", Priority=0)]
        [Variation(@"EllipseGeometry.xaml", Priority=0)]
        [Variation(@"GradientStop.xaml", Priority=0)]       //Not in Tactics.
        [Variation(@"LengthDecelBVT.xaml", Priority=0)]
        [Variation(@"LinearGradientBrush.xaml", Priority=0)]
        [Variation(@"LineGeometry.xaml", Priority=1)]
        [Variation(@"LineSegment.xaml", Priority=0)]
        [Variation(@"PointAccelBVT.xaml", Priority=0)]
        [Variation(@"PointDecelBVT.xaml", Priority=0)]
        [Variation(@"PointKeySplineBVT.xaml", Priority=0)]
        [Variation(@"PointSpeedBVT.xaml", Priority=0)]
        [Variation(@"QuadraticBezierSegment.xaml", Priority=0)]
        [Variation(@"RadialGradientBrush.xaml", Priority=0)]
        [Variation(@"RectAccelBVT.xaml", Priority=0)]
        [Variation(@"RectDecelBVT.xaml", Priority=0)]
        [Variation(@"RectangleGeometry.xaml", Priority=0)]
        [Variation(@"RectKeySplineBVT.xaml", Priority = 0, Keywords = "MicroSuite")]
        [Variation(@"RectSpeedBVT.xaml", Priority=0)]
        [Variation(@"RotateTransform.xaml", Priority=0)]
        // [Variation(@"ScaleTransform.xaml", Priority=0)]
        [Variation(@"SizeAccelBVT.xaml", Priority=0)]
        [Variation(@"SizeDecelBVT.xaml", Priority=0)]
        [Variation(@"SizeKeySplineBVT.xaml", Priority = 0, Keywords = "MicroSuite")]
        [Variation(@"SizeSpeedBVT.xaml", Priority=0)]
        [Variation(@"SkewTransform.xaml", Priority=0)]
        [Variation(@"SolidColorBrush.xaml", Priority=0)]
        [Variation(@"StartSegment.xaml", Priority=0)]

        //Style.
        [Variation(@"StyleColorDef.xaml", Priority=0)]
        // [Variation(@"StyleColorTypeOf.xaml", Priority=1)]
        [Variation(@"StyleColorTree.xaml", Priority=1)]
        [Variation(@"StyleColorBasedOn.xaml", Priority=1)]
        [Variation(@"StyleColorSimple.xaml", Priority=1)]
        [Variation(@"StyleDoubleDef.xaml", Priority=1)]
        [Variation(@"StyleDoubleTypeOf.xaml", Priority=1)]
        [Variation(@"StyleDoubleTree.xaml", Priority=0)]
        [Variation(@"StyleDoubleBasedOn.xaml", Priority=1)]
        [Variation(@"StyleDoubleBasedOn2.xaml", Priority=0)]
        [Variation(@"StyleDoubleSimple.xaml", Priority=1)]
        [Variation(@"StyleRectDef.xaml", Priority=1)]
        [Variation(@"StyleRectTypeOf.xaml", Priority=1)]
        // [Variation(@"StyleRectTree.xaml", Priority=1)]
        [Variation(@"StyleRectBasedOn.xaml", Priority=1)]
        [Variation(@"StyleRectSimple.xaml", Priority=1)]
        [Variation(@"StyleLengthTree.xaml", Priority=1)]

        //TextBlock.
        [Variation(@"TextWidthMarkup.xaml")]
        [Variation(@"TextColorMarkup.xaml")]
        [Variation(@"TextHeightMarkup.xaml")]
        [Variation(@"TextLeftMarkup.xaml")]
        [Variation(@"TextOpacityMarkup1.xaml")]
        [Variation(@"TextOpacityMarkup2.xaml")]
        [Variation(@"TextRotationMarkup.xaml", Priority=0)]
        [Variation(@"TextSkewMarkup.xaml")]
        [Variation(@"TextWidthMarkup.xaml")]
        [Variation(@"TextGradientStop.xaml", Priority=0)]
        
        
        /******************************************************************************
        * Function:          AnimationMarkupTest Constructor
        ******************************************************************************/
        public AnimationMarkupTest(string testValue): base(testValue)
        {
            s_inputString = testValue;
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(CreateWindow);
            RunSteps += new TestStep(StartClock);
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
            GlobalLog.LogStatus("---Initializing---");
            
            WaitForPriority(DispatcherPriority.SystemIdle);

            return TestResult.Pass;
        }
          
        /******************************************************************************
        * Function:          CreateWindow
        ******************************************************************************/
        /// <summary>
        /// Create a NavigationWindow and initialize the Rendering verifier.
        /// </summary>
        /// <returns></returns>
        TestResult CreateWindow()
        {
            GlobalLog.LogStatus("---CreateWindow---");
            
            Window.Title        = _windowTitle;
            Window.Height       = 450d;
            Window.Width        = 700d;
            Window.Left         = 0d;
            Window.Top          = 0d;
            Window.WindowStyle  = WindowStyle.None;
            AnimationUtilities.RemoveNavigationBar(Window);

            _clientRect = new System.Drawing.Rectangle((int)Window.Left, (int)Window.Top, (int)Window.Width, (int)Window.Height);              

            _verifier = new VisualVerifier();
            _verifier.InitRender(Window);
            
            return TestResult.Pass;
        }

        /******************************************************************************
           * Function:          StartClock
           ******************************************************************************/
        /// <summary>
        /// StartClock: Start the ClockManager, used for Animation Verification.
        /// </summary>
        TestResult StartClock()
        {
            GlobalLog.LogStatus("---StartClock---");

            //Retrieve the value of the Tag property on the root element, if any.
            //If one is found, verification of the final value of the animated DP will be
            //verified later.
            string errMessage = VerifyGetValue.FindTag(Window, ref _valueVerifyNeeded);

            if (errMessage == "")
            {


                this._hostManager = new InternalTimeManager();
                _hostManager.Seek(0);
                _hostManager.Pause();

                GlobalLog.LogStatus("---StartTimer---");

                _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
                _aTimer.Tick += new EventHandler(OnTick);
                _aTimer.Interval = new TimeSpan(0,0,0,0,2000);
                _aTimer.Start();


                WaitForSignal("TestFinished");

                //Pass/Fail determined by verification initiated via the ClockManager.
                if (_testPassed)
                {
                    return TestResult.Pass;
                }
                else
                {
                    return TestResult.Fail;
                }
            }
            else
            {
                GlobalLog.LogEvidence(errMessage);
                return TestResult.Fail;
            }
        }


         /******************************************************************************
           * Function:          CaptureTheScreen
           ******************************************************************************/
        /// <summary>
        /// CaptureTheScreen: gets a screen capture and checks for null;
        /// </summary>
        private System.Drawing.Bitmap CaptureTheScreen()
        {
            Bitmap tempBitmap = ImageUtility.CaptureScreen(_clientRect);
            Signal("CaptureDone", TestResult.Pass);

            return tempBitmap;
        }

        /******************************************************************************
           * Function:          OnTimeTicked
           ******************************************************************************/
        /// <summary>
        /// OnTimeTicked: Calls verification routines.
        /// </summary>
        private void OnTick(object sender, EventArgs e)          
        {
            _tickCount++;
            
            GlobalLog.LogStatus("**********Tick #" + _tickCount + " at time " + (_hostManager.CurrentTime.HasValue ? (int)_hostManager.CurrentTime.Value.TotalMilliseconds : 0));

            if (_tickCount == 1 )
            {
                s_beforeCapture = CaptureTheScreen();
                WaitForSignal("CaptureDone");
                _hostManager.Resume();
                _hostManager.Seek(3000);
                _hostManager.Pause();
            }
            else if (_tickCount == 2)
            {
                s_betweenCapture = CaptureTheScreen();
                WaitForSignal("CaptureDone");
                _hostManager.Resume();
                _hostManager.Seek(7000);
                _hostManager.Pause();
            }
            else 
            {

                // the third tick should be the final tick. After capturing the screen, validate and pass/fail and close the application

                s_afterCapture = CaptureTheScreen();
                WaitForSignal("CaptureDone");
              
                GlobalLog.LogStatus("**********Last Tick");

                outputData = "<--------Final Result-------->\n";
                bool pass1 = _imageCompare.Compare(new ImageAdapter(s_beforeCapture), new ImageAdapter(s_betweenCapture), true);
                bool pass2 = _imageCompare.Compare(new ImageAdapter(s_betweenCapture), new ImageAdapter(s_afterCapture), true);
                bool pass3 = _imageCompare.Compare(new ImageAdapter(s_beforeCapture), new ImageAdapter(s_afterCapture), true);      

                if ( (!pass1) && (!pass2) && (!pass3) ) 
                { 
                    _testPassed = true;
                    outputData += "  All screen captures were different\n";
                }

                if (pass1) { outputData += "  Before and Between animation captures were identical \n"; }
                if (pass2) { outputData += "  Between and After animation captures were identical \n"; }
                if (pass3) { outputData += "  Before and After animation captures were identical \n"; }

                if (_valueVerifyNeeded)
                {
                    //Verify that the final animated DP value is correct.
                    bool pass4 = VerifyGetValue.VerifyValue(Window, ref outputData);
                    _testPassed = (_testPassed && pass4);
                }

                GlobalLog.LogEvidence( outputData );


                Signal("TestFinished", TestResult.Pass);
            }
        }
        #endregion
    }
}
