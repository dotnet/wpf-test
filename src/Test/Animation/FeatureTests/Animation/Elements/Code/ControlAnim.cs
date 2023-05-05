// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
* ***************  MIL Integration Test *****************
*     Major Actions:
*          (a) Create a new Navigation window that do OnStartup.
*          (b) Create Listbox, image, button, scrollbar, scrollview, textbox and listbox
*               Set up various animations such as DoubleAnimation, ColorAnimation, etc.
*               on the above controls with different properties.
*          (d) Use the TimeEventHandler to carry out verification after the animation is complete.
*     Pass Conditions:
*          The test passes if (a) the animation's CloneCurrentValue() API returns the correct value, and
*          (b) the actual rendering matches the expected rendering during the course of the animation.
*     How verified:
*          The SideBySideVerifier routine is called to (a) verify values during the animation, and
*          (b) verify rendering as well using VScan to compare the expected vs. actual bitmaps at
*          specified intervals during the animation (controlled by ClockManager).
*
*     Framework:          An executable is created.
*     Area:               Animation/Timing
*     Dependencies:       TestRuntime.dll
*     Support Files:      blue.jpg [must be available at run time]
*********************************************************************************************/
using System;
using System.Globalization;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
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
    /// <priority>1</priority>
    /// <description>
    /// Full verification of animation of Control Dependency Properties
    /// </description>
    /// </summary>
    [Test(1, "Animation.Elements.Controls", "ControlAnimTest", SupportFiles=@"FeatureTests\Animation\blue.jpg")]

    public class ControlAnimTest : WindowTest
    {
        #region Test case members
        
        private ClockManager           _clockManager;
        private SideBySideVerifier     _sideBySide;
        private string                 _inputString = "";

        private Canvas                 _body                    = null;
        private TextBlock              _textSV                  = null;
        private Border                 _borderBody              = null;
        private Color                  _colorBody               = Colors.Navy;
        //private Color                  colorImage              = Color.FromArgb(0xFF,0x00,0x00,0xFE);
        private Color                  _colorScrollViewer       = Colors.Red;
        private Color                  _colorCanvas2            = Colors.Red;
        private Color                  _colorRadioButton        = Colors.MediumVioletRed;
        private Color                  _colorInkCanvas          = Colors.White;
        private AnimationClock         _clock1                  = null;
        private AnimationClock         _clock2                  = null;
        private AnimationClock         _clock3                  = null;
        private string                 _windowTitle             = "Control Animations";
        private TimeSpan               _BEGIN_TIME              = TimeSpan.FromMilliseconds(3000);
        private Duration               _DURATION_TIME           = new Duration(TimeSpan.FromMilliseconds(3000));
        private bool                   _testPassed              = true;

        AnimationValidator _myValidator = new AnimationValidator();

        #endregion


        #region Constructor

        [Variation("Button", Priority=0)]
        [Variation("Image")]
        [Variation("ListBox", Priority=0)]
        [Variation("ScrollBar", Priority=2)]
        // [DISABLE WHILE PORTING]
        // [Variation("ScrollViewer")]
        [Variation("TextBox")]
        [Variation("RadioButton")]
        [Variation("Button2")]
        // [DISABLE WHILE PORTING]
        // [Variation("InkCanvas")]
        [Variation("CheckBox", Disabled=true)]
        [Variation("ComboBox")]
        [Variation("Popup", Disabled=true)]
        [Variation("ComboBox2")]
        [Variation("Button3", Priority=0)]

        /******************************************************************************
        * Function:          ControlAnimTest Constructor
        ******************************************************************************/
        public ControlAnimTest(string testValue)
        {
            _inputString = testValue;
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(Verify);
        }

        #endregion

        #region Test Steps

        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Initialize: create a new Window and add an Ellipse to it.
        /// </summary>
        /// <returns></returns>
        TestResult Initialize()
        {
            GlobalLog.LogEvidence("---Initialize--- [Variation=" + _inputString + "]");

            Window.Title           = _windowTitle;
            Window.Left            = 0;
            Window.Top             = 0;
            Window.Height          = 480;
            Window.Width           = 480;
            Window.WindowStyle     = WindowStyle.None;
            Window.ContentRendered  += new EventHandler(OnContentRendered);

            _borderBody = new Border();
            _borderBody.Width          = 300d;
            _borderBody.Height         = 300d;
            _borderBody.Background     = new SolidColorBrush(_colorBody);
            Window.Content = _borderBody;

            _body  = new Canvas();
            _body.Width                = 300;
            _body.Height               = 300;
            Canvas.SetTop  (_body, 0d);
            Canvas.SetLeft (_body, 0d);               
            _borderBody.Child = _body;

            GlobalLog.LogStatus("---Window created---");

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          OnContentRendered
        ******************************************************************************/
        /// <summary>
        /// Fires when the page loads and the page content is finished rendering.
        /// Starting the Animation here.
        /// </summary>
        /// <returns></returns>
        private void OnContentRendered(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("----OnContentRendered----");

            //Set up the Clock Manager to control the Clock. This pauses the Root timeline.
            int[] times = new int[]{2500,3500,4500,5500,6500};
            _clockManager = new ClockManager(times);
            ClockManager.Ticked += new TickEvent(OnTimeTicked);
            GlobalLog.LogStatus("---Clock Manager set up---");

            //Create an instance of the SideBySideVerifier, passing the Windowdow.
            _sideBySide = new SideBySideVerifier(Window);

            CreateAnimation();
        }
        
        /******************************************************************************
        * Function:          CreateAnimation
        ******************************************************************************/
        /// <summary>
        /// Establish a Timeline to control animation verification.
        /// </summary>
        /// <returns></returns>
        public void CreateAnimation()
        {

            switch (_inputString)
            {
                case "Button":
                    AnimateButton();       //RotateTransform.AngleProperty -- DoubleAnimation.
                    break;
                case "Image":
                    AnimateImage();        //FrameworkElement.HeightProperty -- DoubleAnimation.
                    break;
                case "ListBox":
                    AnimateListBox();      //FrameworkElement.WidthProperty -- DoubleAnimation.
                    break;
                case "ScrollBar":
                    AnimateScrollBar();    //RangeBase.ValueProperty  -- DoubleAnimation.
                    break;
                case "ScrollViewer":
                    AnimateScrollViewer(); //SolidColorBrush.ColorProperty -- ColorAnimation.
                    break;
                case "TextBox":
                    AnimateTextBox();      //Control.HeightProperty -- DoubleAnimation
                    break;
                case "RadioButton":
                    AnimateRadioButton();  //RadioButton.OpacityProperty -- DoubleAnimation
                    break;
                case "Button2":
                    AnimateButton2();      //Multiple properties -- Double and Double
                    break;
                case "InkCanvas":
                    AnimateInkCanvas();    //SolidColorBrush.ColorProperty -- ColorAnimation
                    break;
                case "CheckBox":
                    AnimateCheckBox();     //CheckBox.IsCheckedProperty -- BooleanAnimationUsingKeyFrames
                    break;
                case "ComboBox":
                    AnimateComboBox();     //RotateTransform.CenterProperty -- PointAnimation
                    break;
                case "Popup":
                    AnimatePopup();        //Popup.PlacementRectangleProperty -- RectAnimation
                    break;
                case "ComboBox2":
                    AnimateComboBox2();    //ScaleTransform.ScaleXProperty -- DoubleAnimation
                    break;
                case "Button3":
                    AnimateButton3();      //TranslateTransform.XProperty -- DoubleAnimation
                    break;
                default:
                    string strMessage = "ERROR!!! CreateAnimation: Incorrect variation specified: " + _inputString;
                    GlobalLog.LogEvidence( strMessage);
                    Window.Close();
                    break;
            }

            _clockManager.hostManager.Resume();
            GlobalLog.LogStatus("---Root Timeline Resumed---");
        }
          
        /******************************************************************************
        * Function:          AnimateButton
        ******************************************************************************/
        /// <summary>
        /// BUTTON --- Double animation:  AngleAnimations via a RotateTransform
        /// </summary>
        /// <returns></returns>
        private void AnimateButton()
        {
            try
            {
                _borderBody.Width          = 200d;
                _borderBody.Height         = 200d;

                GlobalLog.LogStatus("Build a button element...");
                Button button  = new Button();
                button.Content             = "Button";
                button.Width               = 100;
                button.Height              = 50;
                button.Foreground          = new SolidColorBrush(Colors.Blue);
                Canvas.SetTop  (button, 90d);
                Canvas.SetLeft (button, 90d);               

                //Rotation requires building a RotateTransform.
                DoubleAnimation animButton = new DoubleAnimation();                    
                animButton.FillBehavior     = FillBehavior.HoldEnd;
                animButton.BeginTime        = _BEGIN_TIME;
                animButton.Duration         = _DURATION_TIME;
                animButton.From             = 0d;
                animButton.To               = 90d;

                RotateTransform rotateTransform = new RotateTransform();
                rotateTransform.Angle   = 0.0f;
                rotateTransform.CenterX  = 50;
                rotateTransform.CenterY  = 50;

                Decorator TD = new Decorator();
                TD.LayoutTransform      = rotateTransform;
                TD.Child                = button;
                _borderBody.Child = TD;

                _clock1 = animButton.CreateClock();
                rotateTransform.ApplyAnimationClock(RotateTransform.AngleProperty, _clock1);

                //Register an animation for verification, passing the animated DO and DP.
                _sideBySide.RegisterAnimation(rotateTransform,RotateTransform.AngleProperty, _clock1);
            }
            catch (Exception e1)
            {
                GlobalLog.LogEvidence("Exception 1 caught: " + e1.ToString());
                throw(e1);
            }
        }
          
        /******************************************************************************
        * Function:          AnimateImage
        ******************************************************************************/
        /// <summary>
        /// IMAGE --- Double animations:  WidthProperty and HeightProperty
        /// </summary>
        /// <returns></returns>
        private void AnimateImage()
        {
            try
            {
                GlobalLog.LogStatus("Build an image element...");
/*
                Image IM = new Image();
                IM.Source = new BitmapImage(new Uri( "blue.jpg", UriKind.RelativeOrAbsolute ));
                IM.Width           = 50d;
                IM.Height          = 50d;
                IM.Stretch         = Stretch.Fill;
                borderBody.Child = IM;
*/
                Image IM = new Image();
                BitmapImage BI = new BitmapImage();
                BI.BeginInit();
                BI.UriSource = new Uri("blue.jpg", UriKind.Relative);
                BI.EndInit();
                IM.Width           = 50d;
                IM.Height          = 50d;
                IM.Stretch         = Stretch.Fill;
                IM.Source = BI;
                _borderBody.Child = IM;

                DoubleAnimation animImage = new DoubleAnimation();
                animImage.From         = 50d;
                animImage.To           = 150d;
                animImage.BeginTime    = _BEGIN_TIME;
                animImage.Duration     = _DURATION_TIME;
                animImage.FillBehavior = FillBehavior.HoldEnd;

                _clock1 = animImage.CreateClock();
                IM.ApplyAnimationClock(FrameworkElement.HeightProperty, _clock1);

                //Register an animation for verification, passing the animated DO and DP.
                _sideBySide.RegisterAnimation(IM,FrameworkElement.HeightProperty, _clock1);
            }
            catch (Exception e2)
            {
                GlobalLog.LogEvidence("Exception 2 caught: " + e2.ToString());
                throw(e2);
            }
        }

        /******************************************************************************
        * Function:          AnimateListBox
        ******************************************************************************/
        /// <summary>
        /// LISTBOX --- Double animations:  WidthProperty and HeightProperty
        /// </summary>
        /// <returns></returns>
        private void AnimateListBox()
        {
            try
            {
                GlobalLog.LogStatus("Build a ListBox element...");

                ListBox listBox = new ListBox();
                _borderBody.Child = listBox;
                listBox.ClipToBounds      = true;
                listBox.Width             = 50d;
                listBox.Height            = 50d;
                listBox.Background        = new SolidColorBrush(Colors.Green);
                listBox.Foreground        = new SolidColorBrush(Colors.Blue);
                listBox.FontSize          = 24;
                listBox.SelectionMode     = SelectionMode.Multiple;
                for (int i = 0; i < 5; i++)
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = "Item " + i.ToString();
                    listBox.Items.Add(item);
                }
                listBox.SelectedIndex     = 4;

                DoubleAnimation animListBox = new DoubleAnimation();
                animListBox.To           = 150;
                animListBox.BeginTime    = _BEGIN_TIME;
                animListBox.Duration     = _DURATION_TIME;
                animListBox.FillBehavior = FillBehavior.HoldEnd;

                _clock1 = animListBox.CreateClock();
                listBox.ApplyAnimationClock(FrameworkElement.WidthProperty, _clock1);

                _clock2 = animListBox.CreateClock();
                listBox.ApplyAnimationClock(FrameworkElement.HeightProperty, _clock2);

                //Register an animation for verification, passing the animated DO and DP.
                _sideBySide.RegisterAnimation(listBox,FrameworkElement.HeightProperty, _clock1);
                _sideBySide.RegisterAnimation(listBox,FrameworkElement.WidthProperty, _clock2);
            }
            catch (Exception e3)
            {
                GlobalLog.LogEvidence("Exception 3 caught: " + e3.ToString());
                throw(e3);
            }
        }
          
        /******************************************************************************
        * Function:          AnimateScrollBar
        ******************************************************************************/
        /// <summary>
        /// VERTICALSCROLLBAR --- Double animation:  ValueProperty
        /// </summary>
        /// <returns></returns>
        private void AnimateScrollBar()
        {
            try
            {     
                Border borderScrollBar = new Border();
                borderScrollBar.Width               = 40d;
                borderScrollBar.Height              = 300d;

                ScrollBar vScrollBar = new ScrollBar();
                //vScrollBar.ClipToBounds        = true;
                vScrollBar.Width                 = 40d;
                vScrollBar.Height                = 300d;
                vScrollBar.Opacity               = 1;
                //vScrollBar.ThumbProportion     = 0.30f;
                vScrollBar.Value                 = .5f;
                borderScrollBar.Child = vScrollBar;
                _body.Children.Add(borderScrollBar);               

                vScrollBar.ValueChanged          += new RoutedPropertyChangedEventHandler<double>(OnValueChanged);                                        

                _textSV  = new TextBlock();
                _textSV.Width               = 200;
                _textSV.Height              = 200;
                _textSV.FontSize            = 96;
                _textSV.Foreground          = new SolidColorBrush(Colors.Red);
                Canvas.SetTop  (_textSV, 0d);
                Canvas.SetLeft (_textSV, 100d);               
                _body.Children.Add(_textSV);               

                //Animation of the Scrollbar Thumb.                    
                DoubleAnimation animScrollBar = new DoubleAnimation();                                             
                animScrollBar.FillBehavior            = FillBehavior.HoldEnd;
                animScrollBar.BeginTime               = _BEGIN_TIME;
                animScrollBar.Duration                = _DURATION_TIME;
                animScrollBar.To                      = .8;

                _clock1 = animScrollBar.CreateClock();
                vScrollBar.ApplyAnimationClock(RangeBase.ValueProperty, _clock1);

                //Register an animation for verification, passing the animated DO and DP.
                _sideBySide.RegisterAnimation(vScrollBar,RangeBase.ValueProperty, _clock1);
            }
            catch (Exception e4)
            {
                GlobalLog.LogEvidence("Exception 4 caught: " + e4.ToString());
                throw(e4);
            }
        }

        private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)          
        {
            //GlobalLog.LogStatus("---ValueChanged---");
            _textSV.Text = "ZZZ";
        }

        /******************************************************************************
        * Function:          AnimateScrollViewer
        ******************************************************************************/
        /// <summary>
        /// SCROLLVIEWER --- ColorAnimation:  ForegroundProperty
        /// </summary>
        /// <returns></returns>
        private void AnimateScrollViewer()
        {
            try
            {
                ScrollViewer SV = new ScrollViewer();
                _body.Children.Add(SV);
                //SV.ClipToBounds     = true;
                SV.Content                     = "W";
                SV.Width                       = 200d;
                SV.Height                      = 200d;
                SV.FontSize                    = 96;                    
                Canvas.SetTop  (SV, 50d);
                Canvas.SetLeft (SV, 50d);

                ColorAnimation animScrollViewer = new ColorAnimation();
                animScrollViewer.From          = Colors.Blue;
                animScrollViewer.To            = _colorScrollViewer;
                animScrollViewer.BeginTime     = _BEGIN_TIME;
                animScrollViewer.Duration      = _DURATION_TIME;
                animScrollViewer.FillBehavior  = FillBehavior.HoldEnd;

                SolidColorBrush SCB = new SolidColorBrush();
                SCB.Color = Colors.Blue;
                SV.Foreground = SCB;

                _clock1 = animScrollViewer.CreateClock();
                SCB.ApplyAnimationClock(SolidColorBrush.ColorProperty, _clock1);

                //Register an animation for verification, passing the animated DO and DP.
                _sideBySide.RegisterAnimation(SCB,SolidColorBrush.ColorProperty, _clock1);
            }
            catch (Exception e5)
            {
                GlobalLog.LogEvidence("Exception 5 caught: " + e5.ToString());
                throw(e5);
            }
        }
          
        /******************************************************************************
        * Function:          AnimateTextBox
        ******************************************************************************/
        /// <summary>
        /// TEXTBOX --- Double animation:  HeightProperty
        /// </summary>
        /// <returns></returns>
        private void AnimateTextBox()
        {
            try
            {
                //Eliminating the White background of the NavigationWindow for verification.
                _borderBody.Width          = 480d;
                _borderBody.Height         = 480d;

                TextBox TX  = new TextBox();
                TX.Height          = 24d;
                TX.Width           = 72d;
                _body.Children.Add(TX);

                DoubleAnimation animTextBox = new DoubleAnimation();
                animTextBox.To                = 200d;
                animTextBox.BeginTime         = _BEGIN_TIME;
                animTextBox.Duration          = _DURATION_TIME;
                animTextBox.FillBehavior      = FillBehavior.HoldEnd;

                _clock1 = animTextBox.CreateClock();
                TX.ApplyAnimationClock(Control.HeightProperty, _clock1);

                //Register an animation for verification, passing the animated DO and DP.
                _sideBySide.RegisterAnimation(TX,Control.HeightProperty, _clock1);
            }
            catch (Exception e6)
            {
                GlobalLog.LogEvidence("Exception 6 caught: " + e6.ToString());
                throw(e6);
            }
        }
          
        /******************************************************************************
        * Function:          AnimateRadioButton
        ******************************************************************************/
        /// <summary>
        /// RADIOBUTTON --- Double animation:  OpacityProperty
        /// </summary>
        /// <returns></returns>
        private void AnimateRadioButton()
        {
            try
            {               
                RadioButton RB = new RadioButton();
                RB.Width           = 75d;
                RB.Height          = 50d;
                RB.Opacity         = 0d;
                RB.Background      = new SolidColorBrush(_colorRadioButton);
                Canvas.SetTop  (RB, 120d);
                Canvas.SetLeft (RB, 120d);               
                _body.Children.Add(RB);

                DoubleAnimation animRadio = new DoubleAnimation();                                             
                animRadio.FillBehavior            = FillBehavior.HoldEnd;
                animRadio.BeginTime               = _BEGIN_TIME;
                animRadio.Duration                = _DURATION_TIME;
                animRadio.To                      = 1d;

                _clock1 = animRadio.CreateClock();
                RB.ApplyAnimationClock(RadioButton.OpacityProperty, _clock1);

                //Register an animation for verification, passing the animated DO and DP.
                _sideBySide.RegisterAnimation(RB,RadioButton.OpacityProperty, _clock1);
            }
            catch (Exception e7)
            {
                GlobalLog.LogEvidence("Exception 7 caught: " + e7.ToString());
                throw(e7);
            }
        }
          
        /******************************************************************************
        * Function:          AnimateButton2
        ******************************************************************************/
        /// <summary>
        /// BUTTON2 --- Left (Double), Top (Double) and Opacity (Double) Animations
        /// </summary>
        /// <returns></returns>
        private void AnimateButton2()
        {
            try
            {
                //Change the size/location of the background, for verification purposes.
                _borderBody.Width          = 80d;
                _borderBody.Height         = 40d;
                Canvas.SetTop  (_borderBody, 0d);
                Canvas.SetLeft (_borderBody, 0d);

                //Add a Canvas, to be covered by the button, for verification purposes.
                Canvas CV  = new Canvas();
                _body.Children.Add(CV);
                CV.Width               = 80;
                CV.Height              = 40;
                CV.Background          = new SolidColorBrush(_colorCanvas2);
                Canvas.SetTop  (CV, 200d);
                Canvas.SetLeft (CV, 200d);               

                GlobalLog.LogStatus("Build a button element...");
                Button button2  = new Button();
                _body.Children.Add(button2);
                button2.Opacity          = 0;
                button2.Content          = "Button2";
                button2.Width            = 80;
                button2.Height           = 40;
                button2.Background       = new SolidColorBrush(Colors.Green);
                Canvas.SetTop  (button2, 0d);
                Canvas.SetLeft (button2, 0d);               

                DoubleAnimation animButton2a = new DoubleAnimation();
                animButton2a.To                = 200d;
                animButton2a.BeginTime         = _BEGIN_TIME;
                animButton2a.Duration          = _DURATION_TIME;
                animButton2a.FillBehavior      = FillBehavior.HoldEnd;

                DoubleAnimation animButton2b = new DoubleAnimation();
                animButton2b.To                = 200d;
                animButton2b.BeginTime         = _BEGIN_TIME;
                animButton2b.Duration          = _DURATION_TIME;
                animButton2b.FillBehavior      = FillBehavior.HoldEnd;

                DoubleAnimation animButton2c = new DoubleAnimation();                                             
                animButton2c.FillBehavior      = FillBehavior.HoldEnd;
                animButton2c.BeginTime         = _BEGIN_TIME;
                animButton2c.Duration          = _DURATION_TIME;
                animButton2c.To                = 1d;

                _clock1 = animButton2a.CreateClock();
                _clock2 = animButton2b.CreateClock();
                _clock3 = animButton2c.CreateClock();

                button2.ApplyAnimationClock(Canvas.LeftProperty, _clock1);
                button2.ApplyAnimationClock(Canvas.TopProperty, _clock2);
                button2.ApplyAnimationClock(Button.OpacityProperty, _clock3);

                //Register an animation for verification, passing the animated DO and DP.
                _sideBySide.RegisterAnimation(button2,Canvas.LeftProperty, _clock1);
                _sideBySide.RegisterAnimation(button2,Canvas.TopProperty, _clock2);
                _sideBySide.RegisterAnimation(button2,Button.OpacityProperty, _clock3);
            }
            catch (Exception e8)
            {
                GlobalLog.LogEvidence("Exception 8 caught: " + e8.ToString());
                throw(e8);
            }
        }
          
        /******************************************************************************
        * Function:          AnimateInkCanvas
        ******************************************************************************/
        /// <summary>
        /// INKCANVAS --- Double animation:  XAnimations via TranslateTransform
        /// </summary>
        /// <returns></returns>
        private void AnimateInkCanvas()
        {
            try
            {               
                InkCanvas HL = new InkCanvas();
                HL.Width           = 125;
                HL.Height          = 50;

                //ANIMATION #1:  XAnimations (via a TranslateTransform)
                DoubleAnimation animInkCanvas1 = new DoubleAnimation();
                animInkCanvas1.FillBehavior         = FillBehavior.HoldEnd;
                animInkCanvas1.BeginTime            = _BEGIN_TIME;
                animInkCanvas1.Duration             = _DURATION_TIME;
                animInkCanvas1.To                   = 5;

                TranslateTransform translateTransform = new TranslateTransform();
                translateTransform.X     = 120;
                translateTransform.Y     = 120;

                //TransformDecorator TD = new TransformDecorator();
                Decorator TD = new Decorator();
                Canvas.SetTop  (TD, 120d);
                Canvas.SetLeft (TD, 120d);               
                TD.LayoutTransform          = translateTransform;
                TD.Child                    = HL;
                _body.Children.Add(TD);

                //ANIMATION #2:  Color
                ColorAnimation animInkCanvas2 = new ColorAnimation();
                animInkCanvas2.From          = Colors.HotPink;
                animInkCanvas2.To            = _colorInkCanvas;
                animInkCanvas2.BeginTime     = _BEGIN_TIME;
                animInkCanvas2.Duration      = _DURATION_TIME;
                animInkCanvas2.FillBehavior  = FillBehavior.HoldEnd;

                SolidColorBrush SCB = new SolidColorBrush();
                SCB.Color = Colors.HotPink;
                HL.Background = SCB;

                _clock1 = animInkCanvas1.CreateClock();
                _clock2 = animInkCanvas2.CreateClock();

                translateTransform.ApplyAnimationClock(TranslateTransform.XProperty, _clock1);
                SCB.ApplyAnimationClock(SolidColorBrush.ColorProperty, _clock2);

                //Register an animation for verification, passing the animated DO and DP.
                _sideBySide.RegisterAnimation((TranslateTransform)TD.LayoutTransform,TranslateTransform.XProperty, _clock1);
                _sideBySide.RegisterAnimation(SCB,SolidColorBrush.ColorProperty, _clock2);
            }
            catch (Exception e9)
            {
                GlobalLog.LogEvidence("Exception 9 caught: " + e9.ToString());
                throw(e9);
            }
        }
          
        /******************************************************************************
        * Function:          AnimateCheckBox
        ******************************************************************************/
        /// <summary>
        /// CHECKBOX --- BooleanAnimationUsingKeyFrames -- IsChecked
        /// </summary>
        /// <returns></returns>
        private void AnimateCheckBox()
        {
            try
            {               
                CheckBox CB = new CheckBox();
                CB.IsChecked = true;
                Canvas.SetTop  (CB, 150d);
                Canvas.SetLeft (CB, 150d);
                _body.Children.Add(CB);
                
                BooleanAnimationUsingKeyFrames animCheckBox = new BooleanAnimationUsingKeyFrames();

                BooleanKeyFrameCollection BKFC = new BooleanKeyFrameCollection();
                BKFC.Add(new DiscreteBooleanKeyFrame(false,KeyTime.FromPercent(0f)));
                BKFC.Add(new DiscreteBooleanKeyFrame(true,KeyTime.FromPercent(0.4f)));
                BKFC.Add(new DiscreteBooleanKeyFrame(true, KeyTime.FromPercent(1.0f)));
                animCheckBox.KeyFrames = BKFC;
                
                animCheckBox.BeginTime         = _BEGIN_TIME;
                animCheckBox.Duration          = _DURATION_TIME;

                _clock1 = animCheckBox.CreateClock();

                CB.ApplyAnimationClock(CheckBox.IsCheckedProperty, _clock1);

                //Register an animation for verification, passing the animated DO and DP.
                _sideBySide.RegisterAnimation(CB,CheckBox.IsCheckedProperty, _clock1);
            }
            catch (Exception e10)
            {
                GlobalLog.LogEvidence("Exception 10 caught: " + e10.ToString());
                throw(e10);
            }
        }

        /******************************************************************************
        * Function:          AnimateComboBox
        ******************************************************************************/
        /// <summary>
        /// COMBOBOX --- PointAnimation of LinearGradientBrush.EndPoint property
        /// </summary>
        /// <returns></returns>
        private void AnimateComboBox()
        {
            try
            {
                GlobalLog.LogStatus("Build a ComboBox element...");

                ComboBox comboBox = new ComboBox();
                _body.Children.Add(comboBox);
                comboBox.ClipToBounds      = true;
                comboBox.Width             = 50d;
                comboBox.Height            = 50d;
                comboBox.FontSize          = 24;
                Canvas.SetTop  (comboBox, 150d);
                Canvas.SetLeft (comboBox, 150d);               

                LinearGradientBrush LGB = new LinearGradientBrush();
                LGB.StartPoint = new Point(0.0, 0.0);
                LGB.EndPoint = new Point(1.0, 1.0);
                LGB.MappingMode = BrushMappingMode.RelativeToBoundingBox;
                GradientStop GS1 = new GradientStop(Colors.Blue, 0.0);
                GradientStop GS2 = new GradientStop(Colors.DodgerBlue, 0.5);
                GradientStop GS3 = new GradientStop(Colors.LightBlue, 1.0);
                GradientStopCollection GSC = new GradientStopCollection();
                GSC.Add(GS1);
                GSC.Add(GS2);
                GSC.Add(GS3);
                LGB.GradientStops = GSC;
                comboBox.Background = LGB;

                for (int i = 0; i < 10; i++)
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = "Item " + i.ToString();
                    comboBox.Items.Add(item);
                }

                PointAnimation animPoint = new PointAnimation();
                animPoint.From                  = new Point(0.0, 1.0);
                animPoint.To                    = new Point(1.0, 0.0);
                animPoint.BeginTime             = _BEGIN_TIME;
                animPoint.Duration              = _DURATION_TIME;

                _clock1 = animPoint.CreateClock();
                LGB.ApplyAnimationClock(LinearGradientBrush.EndPointProperty, _clock1);

                //Register an animation for verification, passing the animated DO and DP.
                _sideBySide.RegisterAnimation(LGB, LinearGradientBrush.EndPointProperty, _clock1);
            }
            catch (Exception e11)
            {
            GlobalLog.LogEvidence("Exception 11 caught: " + e11.ToString());
                throw(e11);
            }
        }
     
          /******************************************************************************
          * Function:          AnimatePopup
          ******************************************************************************/
          /// <summary>
          /// POPUP --- Rect animation:  PlacementRectangleProperty
          /// </summary>
          /// <returns></returns>
          private void AnimatePopup()
          {
              try
              {
                  Ellipse ellipse  = new Ellipse();
                  ellipse.Width = 100;
                  ellipse.Height = 100;

                  ellipse.Fill        = Brushes.Purple;

                  Popup popupControl  = new Popup();
                  popupControl.IsOpen             = true;
                  popupControl.Placement          = PlacementMode.Bottom;
                  popupControl.PlacementRectangle = new Rect(100,100,100,100);
                  //popupControl.Height             = 24;
                  //popupControl.Width              = 72;
                  popupControl.Child              = ellipse;
                  _body.Children.Add(popupControl);

                  RectAnimation animPopup = new RectAnimation();
                  animPopup.From         = new Rect(100,100,100,100);
                  animPopup.To           = new Rect(120,120,120,120);
                  animPopup.BeginTime    = _BEGIN_TIME;
                  animPopup.Duration     = _DURATION_TIME;
                  animPopup.FillBehavior = FillBehavior.HoldEnd;

                  _clock1 = animPopup.CreateClock();
                  popupControl.ApplyAnimationClock(Popup.PlacementRectangleProperty, _clock1);

                  //Register an animation for verification, passing the animated DO and DP.
                  _sideBySide.RegisterAnimation(popupControl,Popup.PlacementRectangleProperty, _clock1);
            }
            catch (Exception e12)
            {
                GlobalLog.LogEvidence("Exception 12 caught: " + e12.ToString());
                throw(e12);
            }
        }

        /******************************************************************************
        * Function:          AnimateComboBox2
        ******************************************************************************/
        /// <summary>
        /// COMBOBOX --- DoubleAnimation of ScaleTransform.ScaleX property
        /// </summary>
        /// <returns></returns>
        private void AnimateComboBox2()
        {
            try
            {
                _borderBody.Background = Brushes.OrangeRed;
                Canvas.SetTop  (_borderBody, -150d);
                Canvas.SetLeft (_borderBody, -150d);

                GlobalLog.LogStatus("Build a ComboBox element...");

                ComboBox comboBox2 = new ComboBox();
                _body.Children.Add(comboBox2);
                comboBox2.Text              = "Avalon!";
                comboBox2.FontSize          = 18;
                comboBox2.IsEditable        = true;
                Canvas.SetTop  (comboBox2, 10d);
                Canvas.SetLeft (comboBox2, 10d);

                DoubleAnimation animScale  = new DoubleAnimation();                                             
                animScale.BeginTime           = _BEGIN_TIME;
                animScale.Duration            = _DURATION_TIME;
                animScale.By                  = -3d;

                ScaleTransform scaleTransform = new ScaleTransform();
                scaleTransform.ScaleX   = 3d;
                scaleTransform.ScaleY   = 3d;

                comboBox2.RenderTransform = scaleTransform;

                _clock1 = animScale.CreateClock();
                scaleTransform.ApplyAnimationClock(ScaleTransform.ScaleXProperty, _clock1);

                //Register an animation for verification, passing the animated DO and DP.
                _sideBySide.RegisterAnimation(scaleTransform, ScaleTransform.ScaleXProperty, _clock1);
            }
            catch (Exception e13)
            {
                GlobalLog.LogEvidence("Exception 13 caught: " + e13.ToString());
                throw(e13);
            }
        }

        /******************************************************************************
        * Function:          AnimateButton3
        ******************************************************************************/
        /// <summary>
        /// COMBOBOX --- DoubleAnimation of TranslateTransform.X property
 
        private void AnimateButton3()
        {
            try
            {
                _borderBody.Background = Brushes.Cornsilk;
                RenderOptions.SetEdgeMode (_borderBody, EdgeMode.Aliased);

                GlobalLog.LogStatus("Build a Button element...");

                Button button3 = new Button();
                _body.Children.Add(button3);
                button3.Content           = "Avalon!";
                button3.FontSize          = 18;
                button3.Background        = Brushes.Red;
                Canvas.SetTop  (button3, 50d);
                Canvas.SetLeft (button3, 200d);

                DoubleAnimation animTrans  = new DoubleAnimation();                                             
                animTrans.BeginTime           = _BEGIN_TIME;
                animTrans.Duration            = _DURATION_TIME;
                animTrans.From                = 0d;
                animTrans.To                  = -200d;

                TranslateTransform translateTransform = new TranslateTransform();
                translateTransform.X   = -150d;
                translateTransform.Y   = 0d;

                button3.RenderTransform = translateTransform;

                _clock1 = animTrans.CreateClock();
                translateTransform.ApplyAnimationClock(TranslateTransform.XProperty, _clock1);

                //Register an animation for verification, passing the animated DO and DP.
                _sideBySide.RegisterAnimation(translateTransform, TranslateTransform.XProperty, _clock1);
            }
            catch (Exception e14)
            {
                GlobalLog.LogEvidence("Exception 14 caught: " + e14.ToString());
                throw(e14);
            }
        }
          
        /******************************************************************************
           * Function:          OnTimeTicked
           ******************************************************************************/
        /// <summary>
        /// OnTimeTicked: Calls verification routines every tick.
        /// </summary>
        private void OnTimeTicked(object sender, TimeControlArgs e)
        {
            GlobalLog.LogStatus("---------------------------------------------");

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
                GlobalLog.LogEvidence("**********************************");
                GlobalLog.LogEvidence("    FINAL RESULT: " + _testPassed);
                GlobalLog.LogEvidence("**********************************");

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
