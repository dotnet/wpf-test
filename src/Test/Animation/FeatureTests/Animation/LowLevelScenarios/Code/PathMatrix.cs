// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* ***************  MIL Integration Test *****************
*     Major Actions:
*          (a) Create a new window via OnStartup.
*          (b) Create a Path, and for each test, associate an animation with one or more of its properties
*          (d) Use the TimeEventHandler to carry out verification after the animation is complete.
*     Pass Conditions:
*          The test passes if the actual rendering matches the expected rendering during the 
*          course of the animation.
*     How verified:
*          A simplified rendering verification is used, consisting of Bitmap comparisons of the 
*          page captured before, during, and after the Animation.  The test case passes if each of
*          the three pair-wise comparisons show a difference in rendering.
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
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.LowLevelScenarios.Shapes</area>
    /// <priority>1</priority>
    /// <description>
    /// Verify rendering of a DoubleAnimationUsingPath under various conditions.
    /// </description>
    /// </summary>
    [Test(2, "Animation.LowLevelScenarios.Shapes", "PathMatrixTest", SupportFiles=@"FeatureTests\Animation\avalon.png")]

    class PathMatrixTest : WindowTest
    {
        #region Test case members

        private ClockManager                    _clockManager;

        private string                          _inputString        = null;
        private Canvas                          _body;
        private Canvas                          _container;
        private Path                            _path1;
        private TextBlock                       _textblock;
        private AnimationClock                  _clock               = null;
        private string                          _windowTitle         = "MatrixAnimationUsingPath";
        private TimeSpan                        _BEGIN_TIME          = TimeSpan.FromMilliseconds(2000);
        private TimeSpan                        _ANIM_DURATION_TIME  = TimeSpan.FromMilliseconds(3000);
        private int                             _tickCount           = 0;
        private bool                            _testPassed          = false;

        private ImageComparator                 _imageCompare        = new ImageComparator();
        private System.Drawing.Rectangle        _clientRect;
        private System.Drawing.Bitmap           _beforeCapture;
        private System.Drawing.Bitmap           _betweenCapture;
        private System.Drawing.Bitmap           _afterCapture;

        #endregion


        #region Constructor

        // [DISABLE WHILE PORTING] 
        // [Variation("1", Priority=0)]
        // [Variation("2", Priority=0)]
        // [Variation("3")]
        // [Variation("4", Priority=0)]
        // [Variation("5")]
        // [Variation("6")]
        // [Variation("7")]
        // [Variation("8")]
        // [Variation("9")]
        // [Variation("10")]
        // [Variation("11")]
        // [Variation("12")]
        // [Variation("13")]
        // [Variation("14")]
        // [Variation("15", Priority=0)]
        // [Variation("16")]


        /******************************************************************************
        * Function:          PathMatrixTest Constructor
        ******************************************************************************/
        public PathMatrixTest(string testValue)
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
        /// Initialize: create a new Window and add content to it.
        /// </summary>
        /// <returns>TestResult.Pass</returns>
        TestResult Initialize()
        {
            GlobalLog.LogStatus("Initialize");

            Window.Title            = _windowTitle;
            Window.Left             = 0d;
            Window.Top              = 0d;
            Window.Height           = 400d;
            Window.Width            = 400d;
            Window.WindowStyle      = WindowStyle.None;
            Window.ContentRendered  += new EventHandler(OnContentRendered);

            _body  = new Canvas();
            Window.Content = _body;
            _body.Width              = 400d;
            _body.Height             = 400d;
            _body.Background         = Brushes.Navy;
            Canvas.SetTop  (_body, 0d);
            Canvas.SetLeft (_body, 0d);

            _container  = new Canvas();
            _body.Children.Add(_container);
            _body.Width              = 125d;
            _body.Height             = 125d;
            _body.Background         = Brushes.LightSteelBlue;
            Canvas.SetTop  (_body, 120d);
            Canvas.SetLeft (_body, 120d);

            //Sometimes added later as a child element.
            _textblock = new TextBlock();
            _textblock.Text  = "A";
            _textblock.FontSize     = 16d;
            _textblock.Height       = 20d;
            _textblock.Width        = 20d;

            //Create a Path, with a PathGeometry.
            _path1  = CreatePath();
            _body.Children.Add(_path1);

            GlobalLog.LogStatus("--------Window created----");

            CreateAnimation();

            _clientRect = new System.Drawing.Rectangle((int)Window.Left, (int)Window.Top, (int)Window.Width, (int)Window.Height);              

            return TestResult.Pass;
        }

        /******************************************************************************
           * Function:          OnContentRendered
           ******************************************************************************/
        /// <summary>
        /// OnContentRendered: Invoked when the page loads.
        /// </summary>
        private void OnContentRendered(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("---OnContentRendered Fired---");

            //Set up the Clock Manager to control verification: a check is made just before and just after the animation.
            //ASSUMPTION: the animation in Markup begins at 2000 and has a duration of 3000.
            int[] times = new int[]{1000, 3500, 6000}; // before, between and after
            _clockManager = new ClockManager(times);
            ClockManager.Ticked += new TickEvent(OnTimeTicked);
            _clockManager.hostManager.Resume();
            
            GlobalLog.LogStatus("---End of OnContentRendered---");
        }
          
        /******************************************************************************
        * Function:          CreateAnimation
        ******************************************************************************/
        /// <summary>
        /// Establish a MatrixAnimationUsingPath.
        /// </summary>
        /// <returns></returns>
        public void CreateAnimation()
        {
            MatrixAnimationUsingPath    anim    = null;
            Decorator                   dec     = null;
            
            switch (_inputString)
            {
                case "1":
                    //TEST 1: MatrixTransform.MatrixProperty:
                    //              IsAdditive              = false
                    //              IsAngleCumulative       = false
                    //              DoesRotateWithTangent   = false
                    //              IsOffsetCumulative      = false
                    Button button1 = new Button();
                    button1.Height      = 20d;
                    button1.Width       = 20d;
                    button1.Content     = "A";
                    button1.FontSize    = 16d;
                    button1.Padding     = new Thickness(0,0,0,0);

                    anim = CreateMatrixAnimation(false, false, false, false);
                    dec = CreateDecorator(anim);
                    dec.Child = button1;
                    break;
                case "2":
                    //TEST 2: MatrixTransform.MatrixProperty:
                    //              IsAdditive              = false
                    //              IsAngleCumulative       = true
                    //              DoesRotateWithTangent   = false
                    //              IsOffsetCumulative      = false
                    TextBox textbox1 = new TextBox();
                    textbox1.Height      = 20d;
                    textbox1.Width       = 20d;
                    textbox1.Text        = "A";
                    textbox1.FontSize    = 16d;
                    textbox1.Padding     = new Thickness(0,0,0,0);

                    anim = CreateMatrixAnimation(false, true, false, false);
                    dec = CreateDecorator(anim);
                    dec.Child = textbox1;
                    break;
                case "3":
                    //TEST 3: MatrixTransform.MatrixProperty:
                    //              IsAdditive              = false
                    //              IsAngleCumulative       = true
                    //              DoesRotateWithTangent   = true
                    //              IsOffsetCumulative      = false
                    
                    Canvas canvas0 = new Canvas();
                    Ellipse ellipse1 = new Ellipse();
                    ellipse1.Width      = 20d;
                    ellipse1.Height     = 20d;
                    ellipse1.Fill       = Brushes.Blue;
                    
                    _textblock.Foreground   = Brushes.Yellow;
                    Canvas.SetTop  (_textblock, 3d);
                    Canvas.SetLeft (_textblock, 4d);
                    
                    canvas0.Children.Add(ellipse1);
                    canvas0.Children.Add(_textblock);

                    anim = CreateMatrixAnimation(false, true, true, false);
                    dec = CreateDecorator(anim);
                    dec.Child = canvas0;
                    break;
                case "4":
                    //TEST 4: MatrixTransform.MatrixProperty:
                    //              IsAdditive              = false
                    //              IsAngleCumulative       = true
                    //              DoesRotateWithTangent   = false
                    //              IsOffsetCumulative      = true

                    anim = CreateMatrixAnimation(false, true, false, true);
                    dec = CreateDecorator(anim);
                    dec.Child = _textblock;
                    break;
                case "5":
                    //TEST 5: MatrixTransform.MatrixProperty:
                    //              IsAdditive              = false
                    //              IsAngleCumulative       = true
                    //              DoesRotateWithTangent   = true
                    //              IsOffsetCumulative      = true
                    ComboBox combobox1 = new ComboBox();
                    combobox1.Height        = 20;
                    combobox1.Width         = 20;
                    combobox1.Background    = Brushes.BlueViolet;

                    anim = CreateMatrixAnimation(false, true, true, true);
                    dec = CreateDecorator(anim);
                    dec.Child = combobox1;
                    break;
                case "6":
                    //TEST 6: MatrixTransform.MatrixProperty:
                    //              IsAdditive              = false
                    //              IsAngleCumulative       = false
                    //              DoesRotateWithTangent   = true
                    //              IsOffsetCumulative      = false
                    Canvas canvas1 = new Canvas();
                    
                    TextBox textbox2 = new TextBox();
                    textbox2.Height      = 20;
                    textbox2.Width       = 20;
                    textbox2.Text        = "A";
                    textbox2.FontSize    = 16;
                    textbox2.Padding     = new Thickness(0,0,0,0);
                    textbox2.Background  = Brushes.Purple;
                    canvas1.Children.Add(textbox2);

                    anim = CreateMatrixAnimation(false, false, true, false);
                    dec = CreateDecorator(anim);
                    dec.Child = canvas1;
                    break;
                case "7":
                    //TEST 7: MatrixTransform.MatrixProperty:
                    //              IsAdditive              = false
                    //              IsAngleCumulative       = false
                    //              DoesRotateWithTangent   = false
                    //              IsOffsetCumulative      = true
                    CheckBox checkbox1 = new CheckBox();
                    checkbox1.Height        = 20;
                    checkbox1.Width         = 20;
                    checkbox1.Background    = Brushes.Green;

                    anim = CreateMatrixAnimation(false, false, false, true);
                    dec = CreateDecorator(anim);
                    dec.Child = checkbox1;
                    break;
                case "8":
                    //TEST 8: MatrixTransform.MatrixProperty:
                    //              IsAdditive              = false
                    //              IsAngleCumulative       = false
                    //              DoesRotateWithTangent   = true
                    //              IsOffsetCumulative      = true
                    RadioButton radiobutton1 = new RadioButton();
                    radiobutton1.Height        = 20;
                    radiobutton1.Width         = 20;
                    radiobutton1.Background    = Brushes.Yellow;

                    anim = CreateMatrixAnimation(false, false, true, true);
                    dec = CreateDecorator(anim);
                    dec.Child = radiobutton1;
                    break;
                case "9":
                    //TEST 9: MatrixTransform.MatrixProperty:
                    //              IsAdditive              = true
                    //              IsAngleCumulative       = true
                    //              DoesRotateWithTangent   = true
                    //              IsOffsetCumulative      = true
                    ListBox listbox1 = new ListBox();
                    listbox1.Height        = 20;
                    listbox1.Width         = 20;
                    listbox1.Background    = Brushes.MistyRose;
                    
                    ListBoxItem listboxitem1 = new ListBoxItem();
                    listboxitem1.Content = "A";
                    listbox1.Items.Add(listboxitem1);

                    anim = CreateMatrixAnimation(true, true, true, true);
                    dec = CreateDecorator(anim);
                    dec.Child = listbox1;
                    break;
                case "10":
                    //TEST 10: MatrixTransform.MatrixProperty:
                    //              IsAdditive              = true
                    //              IsAngleCumulative       = false
                    //              DoesRotateWithTangent   = true
                    //              IsOffsetCumulative      = true
                    InkCanvas inkcanvas1 = new InkCanvas();
                    inkcanvas1.Height        = 20;
                    inkcanvas1.Width         = 20;
                    inkcanvas1.Background    = Brushes.HotPink;

                    inkcanvas1.Children.Add(_textblock);

                    anim = CreateMatrixAnimation(true, false, true, true);
                    dec = CreateDecorator(anim);
                    dec.Child = inkcanvas1;
                    break;
                case "11":
                    //TEST 11: MatrixTransform.MatrixProperty:
                    //              IsAdditive              = true
                    //              IsAngleCumulative       = false
                    //              DoesRotateWithTangent   = false
                    //              IsOffsetCumulative      = true
                    Button button2 = new Button();
                    button2.Height      = 20;
                    button2.Width       = 20;

                    Uri bitmapUri = new Uri(@"Avalon.png", UriKind.RelativeOrAbsolute);
                    BitmapImage image = null;
                    image = new BitmapImage(bitmapUri);
                    ImageBrush imageBrush = new ImageBrush(image);
                    imageBrush.Stretch = Stretch.Fill;

                    button2.Background = imageBrush;

                    anim = CreateMatrixAnimation(true, false, false, true);
                    dec = CreateDecorator(anim);
                    dec.Child = button2;
                    break;
                case "12":
                    //TEST 12: MatrixTransform.MatrixProperty:
                    //              IsAdditive              = true
                    //              IsAngleCumulative       = false
                    //              DoesRotateWithTangent   = true
                    //              IsOffsetCumulative      = false
                    DockPanel dockpanel1 = new DockPanel();
                    dockpanel1.Height        = 20;
                    dockpanel1.Width         = 20;
                    dockpanel1.Background    = Brushes.LightSkyBlue;

                    dockpanel1.Children.Add(_textblock);

                    anim = CreateMatrixAnimation(true, false, true, false);
                    dec = CreateDecorator(anim);
                    dec.Child = dockpanel1;
                    break;
                case "13":
                    //TEST 13: MatrixTransform.MatrixProperty:
                    //              IsAdditive              = true
                    //              IsAngleCumulative       = false
                    //              DoesRotateWithTangent   = false
                    //              IsOffsetCumulative      = false
                    StackPanel stackpanel1 = new StackPanel();
                    stackpanel1.Height        = 20;
                    stackpanel1.Width         = 20;

                    Button button3 = new Button();
                    button3.Height      = 19;
                    button3.Width       = 19;
                    button3.Content     = "A";
                    stackpanel1.Children.Add(button3);

                    anim = CreateMatrixAnimation(true, false, false, false);
                    dec = CreateDecorator(anim);
                    dec.Child = stackpanel1;
                    break;
                case "14":
                    //TEST 14: MatrixTransform.MatrixProperty:
                    //              IsAdditive              = true
                    //              IsAngleCumulative       = true
                    //              DoesRotateWithTangent   = false
                    //              IsOffsetCumulative      = false
                    TabControl tabcontrol1 = new TabControl();
                    tabcontrol1.Height        = 20;
                    tabcontrol1.Width         = 20;

                    anim = CreateMatrixAnimation(true, true, false, false);
                    dec = CreateDecorator(anim);
                    dec.Child = tabcontrol1;
                    break;
                case "15":
                    //TEST 15: MatrixTransform.MatrixProperty:
                    //              IsAdditive              = true
                    //              IsAngleCumulative       = true
                    //              DoesRotateWithTangent   = true
                    //              IsOffsetCumulative      = false
                    Polygon polygon1 = new Polygon();
                    PointCollection points = new PointCollection();
                    points.Add(new Point(0, 0));
                    points.Add(new Point(30, 0));
                    points.Add(new Point(30, 20));
                    points.Add(new Point(0, 20));
                    polygon1.Points = points;
                    polygon1.Stroke             = Brushes.Navy;
                    polygon1.StrokeThickness    = 2d;
                    polygon1.Fill               = Brushes.Bisque;

                    anim = CreateMatrixAnimation(true, true, true, false);
                    dec = CreateDecorator(anim);
                    dec.Child = polygon1;
                    break;
                case "16":
                    //TEST 16: MatrixTransform.MatrixProperty:
                    //              IsAdditive              = true
                    //              IsAngleCumulative       = true
                    //              DoesRotateWithTangent   = false
                    //              IsOffsetCumulative      = true
                    Rectangle rectangle1 = new Rectangle();
                    rectangle1.Height           = 20d;
                    rectangle1.Width            = 20d;
                    rectangle1.Stroke           = Brushes.Purple;
                    rectangle1.StrokeThickness  = 2d;
                    rectangle1.Fill             = Brushes.Pink;

                    anim = CreateMatrixAnimation(true, true, false, true);
                    dec = CreateDecorator(anim);
                    dec.Child = rectangle1;
                    break;
                default:
                    GlobalLog.LogEvidence("ERROR!!! CreateAnimation: Incorrect argument.");
                    Signal("TestFinished", TestResult.Fail);
                    break;
            }

            if (dec != null)
            {
                //Add the Decorator to the Canvas container.
                _container.Children.Add(dec);
            }
        }

        /******************************************************************************
        * Function:          CreateDecorator
        ******************************************************************************/
        /// <summary>
        /// OnTimeTicked: Creates and returns a Decorator containing an animated MatrixProperty.
        /// </summary>
        /// <returns>Decorator</returns>
        private Decorator CreateDecorator(MatrixAnimationUsingPath animMatrix)
        {
            //Create a TranslateTransform.
            TranslateTransform TT = new TranslateTransform();
            TT.X    = -7.5;
            TT.Y    = -7.5;
            
            //Create a MatrixTransform and add the Animation to it.
            MatrixTransform MT = new MatrixTransform();
            _clock = animMatrix.CreateClock();
            MT.ApplyAnimationClock(MatrixTransform.MatrixProperty, _clock);
            
            //Create a TranformGroup, and add the two Transforms to it.
            TransformGroup TGRP = new TransformGroup();
            TGRP.Children.Add(TT);
            TGRP.Children.Add(MT);
            
            //Create a Decorator and add the TransformGroup to it.
            Decorator decorator = new Decorator();
            decorator.Margin          = new Thickness(15,15,15,15);
            decorator.RenderTransform = TGRP;
            
            return decorator;
        }
          
        /******************************************************************************
        * Function:          CreateMatrixAnimation
        ******************************************************************************/
        /// <summary>
        /// CreateMatrixAnimation: Creates and returns an animation.
        /// </summary>
        /// <returns>MatrixAnimationUsingPath</returns>
        private MatrixAnimationUsingPath CreateMatrixAnimation(bool parm1, bool parm2, bool parm3, bool parm4)
        {
            MatrixAnimationUsingPath animMatrix = new MatrixAnimationUsingPath();
            animMatrix.BeginTime                = _BEGIN_TIME;
            animMatrix.Duration                 = new Duration(_ANIM_DURATION_TIME);
            animMatrix.RepeatBehavior           = new RepeatBehavior(2d);
            animMatrix.IsAdditive               = parm1;
            animMatrix.IsAngleCumulative        = parm2;
            animMatrix.DoesRotateWithTangent    = parm3;
            animMatrix.IsOffsetCumulative       = parm4;

            //Add a PathGeometry to the MatrixAnimation.
            //It is also assigned to a Path element to render a path the animation will follow.
            animMatrix.PathGeometry = CreatePathGeometry();
            
            return animMatrix;
        }
          
        /******************************************************************************
        * Function:          CreatePath
        ******************************************************************************/
        /// <summary>
        /// OnTimeTicked: Creates and returns a Path with a PathGeometry.
        /// </summary>
        /// <returns>Path</returns>
        private Path CreatePath()
        {
            Path path  = new Path();
            Canvas.SetTop  (path, 15d);
            Canvas.SetLeft (path, 15d);
            path.Stroke             = Brushes.SteelBlue;
            path.StrokeThickness    = 2d;

            //Create a PathGeometry and assign it to the Path's Data property, to display the path.
            //It is also assigned to the MatrixAnimation.
            path.Data   = CreatePathGeometry();
            
            return path;
        }
        
        /******************************************************************************
        * Function:          CreatePathGeometry
        ******************************************************************************/
        /// <summary>
        /// OnTimeTicked: Creates and returns a PathGeometry.
        /// </summary>
        /// <returns>PathGeometry</returns>
        private PathGeometry CreatePathGeometry()
        {
            //Create a PathFigureCollection.
            PathFigureCollection PFC1 = SpecialObjects.CreatePathFigureCollection();

            //Create a PathGeometry: animation.
            //Assign it a ScaleTransform and PathFigureCollection.
            ScaleTransform scaleTransform1 = new ScaleTransform();
            scaleTransform1.ScaleX      = 1.5;
            scaleTransform1.ScaleY      = 1.5;

            PathGeometry pathGeometry   = new PathGeometry();
            pathGeometry.Transform      = scaleTransform1;
            pathGeometry.Figures        = PFC1;
            
            return pathGeometry;
        }        

        /******************************************************************************
           * Function:          CaptureTheScreen
          ******************************************************************************/
        /// <summary>
        /// CaptureTheScreen: gets a screen capture and checks for null;
        /// </summary>
        /// <returns>A Bitmap, used for animation verification</returns>
        private System.Drawing.Bitmap CaptureTheScreen()
        {
            System.Drawing.Bitmap tempBitmap = ImageUtility.CaptureScreen(_clientRect);

            return tempBitmap;
        }

        /******************************************************************************
           * Function:          OnTimeTicked
           ******************************************************************************/
        /// <summary>
        /// OnTimeTicked: Calls verification routines.
        /// </summary>
        private void OnTimeTicked(object sender, TimeControlArgs e)
        {
            string outputData   = "";
            
            _tickCount++;
            
            GlobalLog.LogStatus("**********Tick #" + _tickCount + " at time " + e.curTime);

            if (_tickCount == 1 )
            {
                _beforeCapture = CaptureTheScreen();
            }
            else if (_tickCount == 2)
            {
                _betweenCapture = CaptureTheScreen();
            }
            else 
            {
                _afterCapture = CaptureTheScreen();
            }

            //On the last tick, pass/fail the test case, then close the application.           
            if (e.lastTick)
            {                
                GlobalLog.LogStatus("**********Last Tick");

                outputData = "<--------Final Result-------->\n";
                bool pass1 = _imageCompare.Compare(new ImageAdapter(_beforeCapture), new ImageAdapter(_betweenCapture), true);
                bool pass2 = _imageCompare.Compare(new ImageAdapter(_betweenCapture), new ImageAdapter(_afterCapture), true);
                bool pass3 = _imageCompare.Compare(new ImageAdapter(_beforeCapture), new ImageAdapter(_afterCapture), true);      

                if ( (!pass1) && (!pass2) && (!pass3) ) 
                { 
                    _testPassed = true;
                    outputData += "  All screen captures were different\n";
                }

                if (pass1) { outputData += "  Before and Between animation captures were identical \n"; }
                if (pass2) { outputData += "  Between and After animation captures were identical \n"; }
                if (pass3) { outputData += "  Before and After animation captures were identical \n"; }

                GlobalLog.LogEvidence( outputData );
                
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
