// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* ***************  MIL Integration Test *****************
*     Description:
*          Verify using a Custom Animation to animate Enums.
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
    /// <area>Animation.HighLevelScenarios.Extensibility</area>
    /// <priority>3</priority>
    /// <description>
    /// Verify Custom Animations on selected Enumeration properties.
    /// </description>
    /// </summary>
    [Test(3, "Animation.HighLevelScenarios.Extensibility", "CustomAnimEnumTest")]


    /******************************************************************************
    *******************************************************************************
    * CLASS:          CustomAnimEnumTest
    *******************************************************************************
    ******************************************************************************/
    class CustomAnimEnumTest : WindowTest
    {
        #region Test case members

        public  static string               inputString         = "";
        private AnimationClock              _AC                  = null;
        private DockPanel                   _dockpanel           = null;
        private Button                      _button1             = null;
        private TextBlock                   _textblock1          = null;
        private TextBox                     _textbox1            = null;
        private Slider                      _slider1             = null;
        private Popup                       _popup1              = null;
        private Expander                    _expander1           = null;
        private TabControl                  _tab1                = null;
        private SolidColorBrush             _SCB                 = null;
        private double                      _fromValue           = 10d;
        private TimeSpan                    _ANIM_DURATION       = TimeSpan.FromMilliseconds(3000);
        private TimeSpan                    _ANIM_DURATION_STORY = TimeSpan.FromMilliseconds(6000);
        private bool                        _testPassed          = false;

        #endregion


        #region Constructor

        [Variation("AutoToolTipPlacement", Priority = 0, Keywords = "MicroSuite")]
        [Variation("Dock")]
        [Variation("ExpandDirection")]
        [Variation("FlowDirection")]
        [Variation("PlacementMode")]
        [Variation("ScrollBarVisibility")]
        [Variation("SizeToContent")]
        [Variation("TabStripPlacement")]
        [Variation("TextAlignment")]
        [Variation("TickPlacement")]
        [Variation("VerticalAlignment", Priority=0)]
        [Variation("WindowState")]
        [Variation("WindowStyle")]

        /******************************************************************************
        * Function:          CustomAnimEnumTest Constructor
        ******************************************************************************/
        public CustomAnimEnumTest(string testValue)
        {
            inputString = testValue;
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(Create);
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

            NameScope.SetNameScope(Window, new NameScope());

            Canvas body  = new Canvas();
            body.Background = Brushes.OrangeRed;
            Window.Content = body;

            _dockpanel = new DockPanel();
            _dockpanel.Name          = "dockpanel";
            _dockpanel.Height        = 300d;
            _dockpanel.Width         = 300d;
            _dockpanel.Background    = Brushes.Blue;

            _button1 = new Button();
            _button1.Name        = "button1";
            _button1.Content     = "Avalon!";
            _button1.Width       = 60d;
            _button1.Height      = 30d;
            _dockpanel.Children.Add(_button1);
            Window.RegisterName(_button1.Name, _button1);
            body.Children.Add(_dockpanel);

            _textbox1 = new TextBox();
            _textbox1.Name    = "textbox1";
            _textbox1.Height  = 30d;
            _textbox1.Width   = 60d;
            _dockpanel.Children.Add(_textbox1);

            _textblock1 = new TextBlock();
            _textblock1.Name         = "textblock1";
            _textblock1.Width        = 80d;
            _textblock1.Height       = 40d;
            _textblock1.Text         = "TextBlock!";
            _dockpanel.Children.Add(_textblock1);

            _slider1 = new Slider();
            _slider1.Orientation         = Orientation.Vertical;
            _slider1.Name                = "slider1";
            _slider1.Height              = 70d;
            _slider1.Value               = 50d;
            _slider1.HorizontalAlignment = HorizontalAlignment.Left;
            _slider1.IsSnapToTickEnabled = true;
            _slider1.Maximum             = 3d;
            _slider1.TickPlacement       = TickPlacement.Both;
            _slider1.AutoToolTipPlacement= AutoToolTipPlacement.None;
            _slider1.TickFrequency       = 1;
            _dockpanel.Children.Add(_slider1);

            ToolTip tt = new ToolTip();
            tt.Content = "ToolTip";
            _slider1.ToolTip = tt;

            _popup1  = new Popup();
            _popup1.Name                 = "popup1";
            _popup1.PlacementTarget      = _button1;
            _popup1.Placement            = PlacementMode.Bottom;
            //popup1.PlacementRectangle   = new Rect(20,20,20,20);
            Rectangle rr = new Rectangle();
            rr.Height  = 15d;
            rr.Width   = 30d;
            rr.Fill    = Brushes.Yellow;
            _popup1.Child                = rr;
            _dockpanel.Children.Add(_popup1);

            _popup1.IsOpen = true; //Must happen after Window.Show, in order for placement location to be correct: if not, the popup will appear, but will be placed at 0,0.

            _expander1 = new Expander();
            _expander1.Name      = "expander1";
            _expander1.Content   = new CheckBox();
            _dockpanel.Children.Add(_expander1);

            _tab1 = new TabControl();
            _tab1.Name       = "tab1";
            TabItem tabitem1 = new TabItem();
            TabItem tabitem2 = new TabItem();
            TabItem tabitem3 = new TabItem();
            _tab1.Items.Add(tabitem1);
            _tab1.Items.Add(tabitem2);
            _tab1.Items.Add(tabitem3);
            _dockpanel.Children.Add(_tab1);

            _SCB = new SolidColorBrush();
            _SCB.Color = Colors.DodgerBlue;
            _button1.Background = _SCB;
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
        * Function:          Create
        ******************************************************************************/
        /// <summary>
        /// Create: Establish a Storyboard containing an Animation.
        /// </summary>
        /// <returns>TestResult</returns>
        TestResult Create()
        {
            WaitForSignal("PageRendered");

            FrameworkElement   FE           = null;
            PropertyPath       propertyPath = null;
            NewCustomAnimation customAnim   = null;

            bool animationCreated = CreateAnimation(ref FE, ref propertyPath, ref customAnim);

            if (animationCreated)
            {
                Storyboard.SetTargetProperty(customAnim, propertyPath);
                CreateStoryboard(FE, customAnim);
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
        bool CreateAnimation(ref FrameworkElement FE, ref PropertyPath path, ref NewCustomAnimation customAnim)
        {
            GlobalLog.LogStatus("---CreateAnimation---");

            bool animCreated = true;

            customAnim = new NewCustomAnimation();
            customAnim.From      = _fromValue;
            customAnim.To        = 50d;
            customAnim.Duration  = new Duration(_ANIM_DURATION);
            customAnim.CurrentStateInvalidated       += new EventHandler(OnCurrentStateInvalidated);

            switch (inputString)
            {
                case "AutoToolTipPlacement":
                    _slider1.AutoToolTipPlacement = AutoToolTipPlacement.None;
                    path = new PropertyPath("(0)", Slider.AutoToolTipPlacementProperty);
                    FE = (FrameworkElement)_slider1;
                    break;
                case "Dock":
                    _dockpanel.LastChildFill = false;
                    Canvas.SetTop  (_button1, 10d);
                    Canvas.SetLeft (_button1, 10d);
                    DockPanel.SetDock (_button1, Dock.Right);
                    path = new PropertyPath("(0)", DockPanel.DockProperty);
                    FE = (FrameworkElement)_button1;
                    break;
                case "ExpandDirection":
                    _expander1.ExpandDirection = ExpandDirection.Down;
                    path = new PropertyPath("(0)", Expander.ExpandDirectionProperty);
                    FE = (FrameworkElement)_expander1;
                    break;
                case "FlowDirection":
                    _button1.FlowDirection = FlowDirection.LeftToRight;
                    path = new PropertyPath("(0)", FrameworkElement.FlowDirectionProperty);
                    FE = (FrameworkElement)_button1;
                    break;
                case "PlacementMode":
                    path = new PropertyPath("(0)", Popup.PlacementProperty);
                    FE = (FrameworkElement)_popup1;
                    break;
                case "PopupAnimation":
                    _popup1.PopupAnimation = PopupAnimation.None;
                    path = new PropertyPath("(0)", Popup.PopupAnimationProperty);
                    FE = (FrameworkElement)_popup1;
                    break;
                //case "Rotation":
                //    image1 = CreateImage(Rotation.Rotate0);
                //    ImageBrush imageBrush = new ImageBrush(image1);
                //    imageBrush.Stretch = Stretch.Fill;
                //    button1.Background = imageBrush;
                //    path = new PropertyPath("(0)", BitmapImage.RotationProperty);
                //    FE = (FrameworkElement)image1;
                //    break;
                case "ScrollBarVisibility":
                    _textbox1.Text                           = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                    _textbox1.TextWrapping                   = TextWrapping.Wrap;
                    _textbox1.VerticalScrollBarVisibility    = ScrollBarVisibility.Auto;
                    path = new PropertyPath("(0)", TextBox.VerticalScrollBarVisibilityProperty);
                    FE = (FrameworkElement)_textbox1;
                    break;
                case "SizeToContent":
                    Window.SizeToContent = SizeToContent.Height;
                    path = new PropertyPath("(0)", Window.SizeToContentProperty);
                    FE = (FrameworkElement)Window;
                    break;
                case "TabStripPlacement":
                    _tab1.TabStripPlacement = Dock.Bottom;
                    path = new PropertyPath("(0)", TabControl.TabStripPlacementProperty);
                    FE = (FrameworkElement)_tab1;
                    break;
                case "TextAlignment":
                    _textblock1.TextAlignment = TextAlignment.Right;
                    path = new PropertyPath("(0)", TextBlock.TextAlignmentProperty);
                    FE = (FrameworkElement)_textblock1;
                    break;
                case "TickPlacement":
                    path = new PropertyPath("(0)", Slider.TickPlacementProperty);
                    FE = (FrameworkElement)_slider1;
                    break;
                case "VerticalAlignment":
                    _button1.VerticalAlignment = VerticalAlignment.Center;
                    path = new PropertyPath("(0)", FrameworkElement.VerticalAlignmentProperty);
                    FE = (FrameworkElement)_button1;
                    break;
                case "WindowState":
                    Window.WindowState = WindowState.Maximized;
                    path = new PropertyPath("(0)", Window.WindowStateProperty);
                    FE = (FrameworkElement)Window;
                    break;
                case "WindowStyle":
                    Window.WindowStyle = WindowStyle.ThreeDBorderWindow;
                    path = new PropertyPath("(0)", Window.WindowStyleProperty);
                    FE = (FrameworkElement)Window;
                    break;
                default:
                    GlobalLog.LogEvidence("ERROR!!! CreateAnimation: Incorrect argument (1).");
                    animCreated = false;
                    break;
            }
            return animCreated;
        }

        /******************************************************************************
        * Function:          CreateStoryboard
        ******************************************************************************/
        /// <summary>
        /// Establish and begin a Storyboard containing an Animation.
        /// </summary>
        /// <returns></returns>
        public void CreateStoryboard(FrameworkElement FE, NewCustomAnimation customAnim)
        {
            GlobalLog.LogStatus("---CreateStoryboard---");
            Storyboard storyboard = new Storyboard();
            storyboard.Duration         = new Duration(_ANIM_DURATION_STORY);
            storyboard.Name             = "story";

            storyboard.Children.Add(customAnim);
            
            BeginStoryboard beginStory = AnimationUtilities.CreateBeginStoryboard(Window, "beginStory");

            beginStory.Storyboard = storyboard;

            beginStory.Storyboard.Begin(FE, HandoffBehavior.SnapshotAndReplace, true);

            GlobalLog.LogStatus("---Storyboard Begun---");
        }

/*
        private BitmapImage CreateImage(Rotation rotation)
        {
            BitmapImage im = new BitmapImage();
            Uri bitmapUri = new Uri(@"Avalon.png", UriKind.RelativeOrAbsolute);
            im.BeginInit();
            im.Rotation = rotation;
            im.UriSource = bitmapUri;
            im.EndInit();

            return im;
        }
*/


        /******************************************************************************
        * Function:          OnCurrentStateInvalidated
        ******************************************************************************/
        /// <summary>
        /// OnCurrentStateInvalidated: Calls verification routines.
        /// </summary>
        /// <returns></returns>
        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("---OnCurrentStateInvalidated--- " + ((Clock)sender).CurrentState);

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
                case "AutoToolTipPlacement":
                    AutoToolTipPlacement expectedPlacement  = AutoToolTipPlacement.BottomRight;
                    AutoToolTipPlacement actualPlacementGCV = (AutoToolTipPlacement)_AC.GetCurrentValue(_fromValue, _fromValue);
                    AutoToolTipPlacement actualPlacement    = _slider1.AutoToolTipPlacement;

                    GlobalLog.LogEvidence("------------------------------");
                    GlobalLog.LogEvidence("--Expected               : " + expectedPlacement);
                    GlobalLog.LogEvidence("--Actual GetCurrentValue : " + actualPlacementGCV);
                    GlobalLog.LogEvidence("--Actual Value           : " + actualPlacement);
                    GlobalLog.LogEvidence("------------------------------");

                    _testPassed = ((actualPlacementGCV == expectedPlacement) && (actualPlacement == expectedPlacement));
                    break;
                case "Dock":
                    Dock expectedDock   = Dock.Left;
                    Dock actualDockGCV  = (Dock)_AC.GetCurrentValue(_fromValue, _fromValue);
                    Dock actualDock     = DockPanel.GetDock (_button1);

                    GlobalLog.LogEvidence("------------------------------");
                    GlobalLog.LogEvidence("--Expected               : " + expectedDock);
                    GlobalLog.LogEvidence("--Actual GetCurrentValue : " + actualDockGCV);
                    GlobalLog.LogEvidence("--Actual Value           : " + actualDock);
                    GlobalLog.LogEvidence("------------------------------");

                    _testPassed = ((actualDockGCV == expectedDock) && (actualDock == expectedDock));
                    break;
                case "ExpandDirection":
                    ExpandDirection expectedExpand  = ExpandDirection.Right;
                    ExpandDirection actualExpandGCV = (ExpandDirection)_AC.GetCurrentValue(_fromValue, _fromValue);
                    ExpandDirection actualExpand    = _expander1.ExpandDirection;

                    GlobalLog.LogEvidence("------------------------------");
                    GlobalLog.LogEvidence("--Expected               : " + expectedExpand);
                    GlobalLog.LogEvidence("--Actual GetCurrentValue : " + actualExpandGCV);
                    GlobalLog.LogEvidence("--Actual Value           : " + actualExpand);
                    GlobalLog.LogEvidence("------------------------------");

                    _testPassed = ((actualExpandGCV == expectedExpand) && (actualExpand == expectedExpand));
                    break;
                case "FlowDirection":
                    FlowDirection expectedFlow  = FlowDirection.RightToLeft;
                    FlowDirection actualFlowGCV = (FlowDirection)_AC.GetCurrentValue(_fromValue, _fromValue);
                    FlowDirection actualFlow    = _button1.FlowDirection;

                    GlobalLog.LogEvidence("------------------------------");
                    GlobalLog.LogEvidence("--Expected               : " + expectedFlow);
                    GlobalLog.LogEvidence("--Actual GetCurrentValue : " + actualFlowGCV);
                    GlobalLog.LogEvidence("--Actual Value           : " + actualFlow);
                    GlobalLog.LogEvidence("------------------------------");

                    _testPassed = ((actualFlowGCV == expectedFlow) && (actualFlow == expectedFlow));
                    break;
                case "PlacementMode":
                    PlacementMode expectedPlace  = PlacementMode.Top;
                    PlacementMode actualPlaceGCV = (PlacementMode)_AC.GetCurrentValue(_fromValue, _fromValue);
                    PlacementMode actualPlace    = _popup1.Placement;

                    GlobalLog.LogEvidence("------------------------------");
                    GlobalLog.LogEvidence("--Expected               : " + expectedPlace);
                    GlobalLog.LogEvidence("--Actual GetCurrentValue : " + actualPlaceGCV);
                    GlobalLog.LogEvidence("--Actual Value           : " + actualPlace);
                    GlobalLog.LogEvidence("------------------------------");

                    _testPassed = ((actualPlaceGCV == expectedPlace) && (actualPlace == expectedPlace));
                    break;
                case "PopupAnimation":
                    PopupAnimation expectedPop  = PopupAnimation.Fade;
                    PopupAnimation actualPopGCV = (PopupAnimation)_AC.GetCurrentValue(_fromValue, _fromValue);
                    PopupAnimation actualPop    = _popup1.PopupAnimation;

                    GlobalLog.LogEvidence("------------------------------");
                    GlobalLog.LogEvidence("--Expected               : " + expectedPop);
                    GlobalLog.LogEvidence("--Actual GetCurrentValue : " + actualPopGCV);
                    GlobalLog.LogEvidence("--Actual Value           : " + actualPop);
                    GlobalLog.LogEvidence("------------------------------");

                    _testPassed = ((actualPopGCV == expectedPop) && (actualPop == expectedPop));
                    break;
                //case "Rotation":
                //    Rotation expectedRotation  = Rotation.Rotate180;
                //    Rotation actualRotationGCV = (Rotation)AC.GetCurrentValue(fromValue, fromValue);
                //    Rotation actualRotation    = image1.Rotation;
                //
                //    GlobalLog.LogEvidence("------------------------------");
                //    GlobalLog.LogEvidence("--Expected               : " + expectedRotation);
                //    GlobalLog.LogEvidence("--Actual GetCurrentValue : " + actualRotationGCV);
                //    GlobalLog.LogEvidence("--Actual Value           : " + actualRotation);
                //    GlobalLog.LogEvidence("------------------------------");

                //    testPassed = ((actualRotationGCV == expectedRotation) && (actualRotation == expectedRotation));
                //    break;
                case "ScrollBarVisibility":
                    ScrollBarVisibility expectedVis  = ScrollBarVisibility.Visible;
                    ScrollBarVisibility actualVisGCV = (ScrollBarVisibility)_AC.GetCurrentValue(_fromValue, _fromValue);
                    ScrollBarVisibility actualVis    = _textbox1.VerticalScrollBarVisibility;

                    GlobalLog.LogEvidence("------------------------------");
                    GlobalLog.LogEvidence("--Expected               : " + expectedVis);
                    GlobalLog.LogEvidence("--Actual GetCurrentValue : " + actualVisGCV);
                    GlobalLog.LogEvidence("--Actual Value           : " + actualVis);
                    GlobalLog.LogEvidence("------------------------------");

                    _testPassed = ((actualVisGCV == expectedVis) && (actualVis == expectedVis));
                    break;
                case "SizeToContent":
                    SizeToContent expectedSizeToContent  = SizeToContent.Manual;
                    SizeToContent actualSizeToContentGCV = (SizeToContent)_AC.GetCurrentValue(_fromValue, _fromValue);
                    SizeToContent actualSizeToContent    = Window.SizeToContent;

                    GlobalLog.LogEvidence("------------------------------");
                    GlobalLog.LogEvidence("--Expected               : " + expectedSizeToContent);
                    GlobalLog.LogEvidence("--Actual GetCurrentValue : " + actualSizeToContentGCV);
                    GlobalLog.LogEvidence("--Actual Value           : " + actualSizeToContent);
                    GlobalLog.LogEvidence("------------------------------");

                    _testPassed = ((actualSizeToContentGCV == expectedSizeToContent) && (actualSizeToContent == expectedSizeToContent));
                    break;
                case "TabStripPlacement":
                    Dock expectedTab  = Dock.Right;
                    Dock actualTabGCV = (Dock)_AC.GetCurrentValue(_fromValue, _fromValue);
                    Dock actualTab    = _tab1.TabStripPlacement;

                    GlobalLog.LogEvidence("------------------------------");
                    GlobalLog.LogEvidence("--Expected               : " + expectedTab);
                    GlobalLog.LogEvidence("--Actual GetCurrentValue : " + actualTabGCV);
                    GlobalLog.LogEvidence("--Actual Value           : " + actualTab);
                    GlobalLog.LogEvidence("------------------------------");

                    _testPassed = ((actualTabGCV == expectedTab) && (actualTab == expectedTab));
                    break;
                case "TextAlignment":
                    TextAlignment expectedAlign  = TextAlignment.Center;
                    TextAlignment actualAlignGCV = (TextAlignment)_AC.GetCurrentValue(_fromValue, _fromValue);
                    TextAlignment actualAlign    = _textblock1.TextAlignment;

                    GlobalLog.LogEvidence("------------------------------");
                    GlobalLog.LogEvidence("--Expected               : " + expectedAlign);
                    GlobalLog.LogEvidence("--Actual GetCurrentValue : " + actualAlignGCV);
                    GlobalLog.LogEvidence("--Actual Value           : " + actualAlign);
                    GlobalLog.LogEvidence("------------------------------");

                    _testPassed = ((actualAlignGCV == expectedAlign) && (actualAlign == expectedAlign));
                    break;
                case "TickPlacement":
                    TickPlacement expectedTick  = TickPlacement.TopLeft;
                    TickPlacement actualTickGCV = (TickPlacement)_AC.GetCurrentValue(_fromValue, _fromValue);
                    TickPlacement actualTick    = _slider1.TickPlacement;

                    GlobalLog.LogEvidence("------------------------------");
                    GlobalLog.LogEvidence("--Expected               : " + expectedTick);
                    GlobalLog.LogEvidence("--Actual GetCurrentValue : " + actualTickGCV);
                    GlobalLog.LogEvidence("--Actual Value           : " + actualTick);
                    GlobalLog.LogEvidence("------------------------------");

                    _testPassed = ((actualTickGCV == expectedTick) && (actualTick == expectedTick));
                    break;
                case "VerticalAlignment":
                    VerticalAlignment expectedVertAlign  = VerticalAlignment.Bottom;
                    VerticalAlignment actualVertAlignGCV = (VerticalAlignment)_AC.GetCurrentValue(_fromValue, _fromValue);
                    VerticalAlignment actualVertAlign    = _button1.VerticalAlignment;

                    GlobalLog.LogEvidence("------------------------------");
                    GlobalLog.LogEvidence("--Expected               : " + expectedVertAlign);
                    GlobalLog.LogEvidence("--Actual GetCurrentValue : " + actualVertAlignGCV);
                    GlobalLog.LogEvidence("--Actual Value           : " + actualVertAlign);
                    GlobalLog.LogEvidence("------------------------------");

                    _testPassed = ((actualVertAlignGCV == expectedVertAlign) && (actualVertAlign == expectedVertAlign));
                    break;
                case "WindowState":
                    WindowState expectedWinState  = WindowState.Normal;
                    WindowState actualWinStateGCV = (WindowState)_AC.GetCurrentValue(_fromValue, _fromValue);
                    WindowState actualWinState    = Window.WindowState;

                    GlobalLog.LogEvidence("------------------------------");
                    GlobalLog.LogEvidence("--Expected               : " + expectedWinState);
                    GlobalLog.LogEvidence("--Actual GetCurrentValue : " + actualWinStateGCV);
                    GlobalLog.LogEvidence("--Actual Value           : " + actualWinState);
                    GlobalLog.LogEvidence("------------------------------");

                    _testPassed = ((actualWinStateGCV == expectedWinState) && (actualWinState == expectedWinState));
                    break;
                case "WindowStyle":
                    WindowStyle expectedWinStyle  = WindowStyle.ToolWindow;
                    WindowStyle actualWinStyleGCV = (WindowStyle)_AC.GetCurrentValue(_fromValue, _fromValue);
                    WindowStyle actualWinStyle    = Window.WindowStyle;

                    GlobalLog.LogEvidence("------------------------------");
                    GlobalLog.LogEvidence("--Expected               : " + expectedWinStyle);
                    GlobalLog.LogEvidence("--Actual GetCurrentValue : " + actualWinStyleGCV);
                    GlobalLog.LogEvidence("--Actual Value           : " + actualWinStyle);
                    GlobalLog.LogEvidence("------------------------------");

                    _testPassed = ((actualWinStyleGCV == expectedWinStyle) && (actualWinStyle == expectedWinStyle));
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
    * CLASS:          NewCustomAnimation
    *******************************************************************************
    ******************************************************************************/
    // Custom Animation Class:
    public class NewCustomAnimation : System.Windows.Media.Animation.AnimationTimeline
    {
        #region Custom case members

        public double From;
        public double To;

        #endregion


        #region Constructor
        public NewCustomAnimation() : base()
        {
        }

        public new NewCustomAnimation Clone()
        {
            return (NewCustomAnimation)base.Clone();
        }
        public new NewCustomAnimation GetAsFrozen()
        {
            return (NewCustomAnimation)base.GetAsFrozen();
        }
        #endregion


        #region NewCustomAnimation Steps

        protected override void CloneCore( Freezable sourceFreezable)
        {
            NewCustomAnimation newCustomAnimation = (NewCustomAnimation)sourceFreezable;
            base.CloneCore(sourceFreezable);
            this.From = newCustomAnimation.From;
            this.To = newCustomAnimation.To;
        }

        protected override void GetAsFrozenCore(Freezable sourceFreezable)
        {
            NewCustomAnimation newCustomAnimation = (NewCustomAnimation)sourceFreezable;
      
            base.GetAsFrozenCore(sourceFreezable);
            this.From = newCustomAnimation.From;
            this.To = newCustomAnimation.To;
        }
        protected override void GetCurrentValueAsFrozenCore(Freezable sourceFreezable)
        {
            NewCustomAnimation newCustomAnimation = (NewCustomAnimation)sourceFreezable;

            base.GetCurrentValueAsFrozenCore(sourceFreezable);
            this.From = newCustomAnimation.From;
            this.To = newCustomAnimation.To;
        }

        protected override Freezable CreateInstanceCore()
        {
            return new NewCustomAnimation();
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
                switch (CustomAnimEnumTest.inputString)
                {
                    case "AutoToolTipPlacement":
                        return typeof( AutoToolTipPlacement );
                    case "Dock":
                        return typeof( Dock );
                    case "ExpandDirection":
                        return typeof( ExpandDirection );
                    case "FlowDirection":
                        return typeof( FlowDirection );
                    case "PlacementMode":
                        return typeof( PlacementMode );
                    case "PopupAnimation":
                        return typeof( PopupAnimation );
                    case "Rotation":
                        return typeof( Rotation );
                    case "ScrollBarVisibility":
                        return typeof( ScrollBarVisibility );
                    case "SizeToContent":
                        return typeof( SizeToContent );
                    case "TabStripPlacement":
                        return typeof( Dock );
                    case "TextAlignment":
                        return typeof( TextAlignment );
                    case "TickPlacement":
                        return typeof( TickPlacement );
                    case "VerticalAlignment":
                        return typeof( VerticalAlignment );
                    case "WindowState":
                        return typeof( WindowState );
                    case "WindowStyle":
                        return typeof( WindowStyle );
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
            double lowerBound = .4d;
            double upperBound = .6d;

            switch (CustomAnimEnumTest.inputString)
            {
                case "AutoToolTipPlacement":
                    if ( (animClock.CurrentProgress > lowerBound) && (animClock.CurrentProgress < upperBound)  )
                    {
                        return AutoToolTipPlacement.TopLeft;
                    }
                    else
                    {
                        return AutoToolTipPlacement.BottomRight;
                    }
                case "Dock":
                    if ( (animClock.CurrentProgress > lowerBound) && (animClock.CurrentProgress < upperBound)  )
                    {
                        return Dock.Right;
                    }
                    else
                    {
                        return Dock.Left;
                    }
                case "ExpandDirection":
                    if ( (animClock.CurrentProgress > lowerBound) && (animClock.CurrentProgress < upperBound)  )
                    {
                        return ExpandDirection.Left;
                    }
                    else
                    {
                        return ExpandDirection.Right;
                    }
                case "FlowDirection":
                    if ( (animClock.CurrentProgress > lowerBound) && (animClock.CurrentProgress < upperBound)  )
                    {
                        return FlowDirection.LeftToRight;
                    }
                    else
                    {
                        return FlowDirection.RightToLeft;
                    }
                case "PlacementMode":
                    if ( (animClock.CurrentProgress > lowerBound) && (animClock.CurrentProgress < upperBound)  )
                    {
                        return PlacementMode.Right;
                    }
                    else
                    {
                        return PlacementMode.Top;
                    }
                case "PopupAnimation":
                    if ( (animClock.CurrentProgress > lowerBound) && (animClock.CurrentProgress < upperBound)  )
                    {
                        return PopupAnimation.Slide;
                    }
                    else
                    {
                        return PopupAnimation.Fade;
                    }
                case "Rotation":
                    if ( (animClock.CurrentProgress > lowerBound) && (animClock.CurrentProgress < upperBound)  )
                    {
                        return Rotation.Rotate90;
                    }
                    else
                    {
                        return Rotation.Rotate180;
                    }
                case "ScrollBarVisibility":
                    if ( (animClock.CurrentProgress > lowerBound) && (animClock.CurrentProgress < upperBound)  )
                    {
                        return ScrollBarVisibility.Disabled;
                    }
                    else
                    {
                        return ScrollBarVisibility.Visible;
                    }
                case "SizeToContent":
                    if ( (animClock.CurrentProgress > lowerBound) && (animClock.CurrentProgress < upperBound)  )
                    {
                        return SizeToContent.Width;
                    }
                    else
                    {
                        return SizeToContent.Manual;
                    }
                case "TabStripPlacement":
                    if ( (animClock.CurrentProgress > lowerBound) && (animClock.CurrentProgress < upperBound)  )
                    {
                        return Dock.Left;
                    }
                    else
                    {
                        return Dock.Right;
                    }
                case "TextAlignment":
                    if ( (animClock.CurrentProgress > lowerBound) && (animClock.CurrentProgress < upperBound)  )
                    {
                        return TextAlignment.Left;
                    }
                    else
                    {
                        return TextAlignment.Center;
                    }
                case "TickPlacement":
                    if ( (animClock.CurrentProgress > lowerBound) && (animClock.CurrentProgress < upperBound)  )
                    {
                        return TickPlacement.BottomRight;
                    }
                    else
                    {
                        return TickPlacement.TopLeft;
                    }
                case "VerticalAlignment":
                    if ( (animClock.CurrentProgress > lowerBound) && (animClock.CurrentProgress < upperBound)  )
                    {
                        return VerticalAlignment.Top;
                    }
                    else
                    {
                        return VerticalAlignment.Bottom;
                    }
                case "WindowState":
                    if ( (animClock.CurrentProgress > lowerBound) && (animClock.CurrentProgress < upperBound)  )
                    {
                        return WindowState.Minimized;
                    }
                    else
                    {
                        return WindowState.Normal;
                    }
                case "WindowStyle":
                    if ( (animClock.CurrentProgress > lowerBound) && (animClock.CurrentProgress < upperBound)  )
                    {
                        return WindowStyle.None;
                    }
                    else
                    {
                        return WindowStyle.ToolWindow;
                    }
                default:
                    return null;
            }
        }
        #endregion
    }
}
