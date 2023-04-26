// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
* ***************  MIL Animation Test *******************************************************
*     Purpose:            This framework tests the integration of Animation and Element Services,
*                         focusing on EventTriggers in a variety of scenarios.  The Model is 
*                         stateless and consists of the following test matrix (parameters):
*                             (a) Location of the EventTrigger (Element Style / Root Style)
*                             (b) Location of the Storyboard (Trigger / Style Resources / Element Resources / Root Resources)
*                             (c) Type of Trigger (Event / Property / Data / MultiData / Multi)
*                             (c) Type of Animation (Framework Element / Animatable / KeyFrame)
*                             (d) References (Static vs. Dynamic)
*                             (e) Style (Basic vs. BasedOn)
*                         The sequence of events in this test case is:
*                             (1) the page containing a ProgressBar and a Button is rendered.
*                             (2) The Storyboard is set up, based on the Model parameters.
*                             (3) A DispatcherTime is started, used to control the timing of verification.
*                             (4) Using UIAutomation, the mouse is moved over the source element.
*                             (5) The Animation is triggered.
*                             (6) The Animation finishes and is verified.
*     Pass Conditions:    A test case passes if (a) GetValue (at the end of the animation) returns
*                         the correct value of the animated DP, and (b) Animation events fire correctly.
*     How verified:       
*     Framework:          A CLR executable is created.
*     Area:               Animation/Timing
*     Dependencies:       TestRuntime.dll, AnimationFramework.dll
*     Support Files:      EventTriggers2Model.xtc file, which specifies the test cases [must be available at run time]
*********************************************************************************************/
using System;
using System.Xml;
using System.Data;
using System.Collections;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
     
using Microsoft.Test;
using Microsoft.Test.Modeling;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Storyboard.Models.Triggers</area>
    /// <priority>2</priority>
    /// <description>
    /// Verify Animations using EventTriggers in a variety of scenarios.
    /// </description>
    /// </summary>
    [Test(2, "Storyboard.Models.Triggers", "EventTriggers1Mode2", SupportFiles=@"FeatureTests\Animation\EventTriggers2Model.xtc")]

    class EventTriggers2Model : WindowModel
    {
        #region Test case members

        private Canvas                          _body                    = null;
        private ProgressBar                     _progressBar             = null;
        private Canvas                          _canvas1                 = null;
        private ScaleTransform                  _scaleTransform          = null;
        
        private Int32AnimationUsingKeyFrames    _animZIndex              = null;
        private DoubleAnimation                 _animValue               = null;
        private DoubleAnimation                 _animScale               = null;
        
        private double                          _initialValue            = 1d;
        private double                          _initialScale            = 1d;
        private int                             _initialZIndex           = 1;
        private double                          _finalValue              = 100d;
        private double                          _finalScale              = 2d;
        private int                             _finalZIndex             = 4;
        
        //Input values from the Model.
        private string                          _locationTriggerInStyle  = "";
        private string                          _locationStoryboard      = "";
        private string                          _animationType           = "";
        private string                          _triggerType             = "";
        private string                          _styleReference          = "";
        private string                          _styleType               = "";
        
        //Used for setting up the Animation.
        private DependencyProperty              _dp                      = null;
        private BeginStoryboard                 _beginStory              = null;
        private Storyboard                      _story                   = null;
        private ResourceDictionary              _elementDictionary       = null;
        private ResourceDictionary              _rootDictionary          = null;
        private Style                           _baseStyle               = null;
        private Style                           _triggerStyle            = null;
        private FrameworkElement                _animatedElement         = null;

        public int                              actCurrentState         = 0;
        public int                              actCurrentGlobalSpeed   = 0;
        public int                              expCurrentState         = 2;
        public int                              expCurrentGlobalSpeed   = 2;

        private TimeSpan                        _BEGIN_TIME              = TimeSpan.FromMilliseconds(0);
        private Duration                        _DURATION_TIME           = new Duration(TimeSpan.FromMilliseconds(1000));
        private TimeSpan                        _TIMER_INTERVAL          = TimeSpan.FromMilliseconds(2500);
        private string                          _STYLEKEY                = "StyleKey";
        private string                          _BASESTYLEKEY            = "BaseStyleKey";

        private DispatcherTimer                 _aTimer                  = null;
        private int                             _dispatcherTickCount     = 0;
        private int                             _testNumber              = 0;
        private bool                            _testPassed              = false;
        private bool                            _cumulativeResult        = true;
        private string                          _outString               = "";

        #endregion


        #region Contructors

        [Variation("EventTriggers2Model.xtc",    1,   10)]
        [Variation("EventTriggers2Model.xtc",   11,   20)]
        [Variation("EventTriggers2Model.xtc",   21,   30, Disabled=true)]
        [Variation("EventTriggers2Model.xtc",   31,   40)]
        [Variation("EventTriggers2Model.xtc",   41,   50)]
        [Variation("EventTriggers2Model.xtc",   51,   60)]
        [Variation("EventTriggers2Model.xtc",   61,   70)]
        [Variation("EventTriggers2Model.xtc",   71,   80)]
        [Variation("EventTriggers2Model.xtc",   81,   90)]
        [Variation("EventTriggers2Model.xtc",   91,  100)]
        [Variation("EventTriggers2Model.xtc",  101,  110)]
        [Variation("EventTriggers2Model.xtc",  111,  120)]
        [Variation("EventTriggers2Model.xtc",  121,  130)]
        [Variation("EventTriggers2Model.xtc",  131,  140)]
        [Variation("EventTriggers2Model.xtc",  141,  150)]
        [Variation("EventTriggers2Model.xtc",  151,  160)]
        [Variation("EventTriggers2Model.xtc",  161,  170)]
        [Variation("EventTriggers2Model.xtc",  171,  180)]
        [Variation("EventTriggers2Model.xtc",  181,  190)]
        [Variation("EventTriggers2Model.xtc",  191,  200)]
        [Variation("EventTriggers2Model.xtc",  201,  210)]
        [Variation("EventTriggers2Model.xtc",  211,  220)]
        [Variation("EventTriggers2Model.xtc",  221,  230)]
        [Variation("EventTriggers2Model.xtc",  231,  240)]
        [Variation("EventTriggers2Model.xtc",  241,  250)]
        [Variation("EventTriggers2Model.xtc",  251,  260)]
        [Variation("EventTriggers2Model.xtc",  261,  270)]
        [Variation("EventTriggers2Model.xtc",  271,  280)]
        [Variation("EventTriggers2Model.xtc",  281,  290)]
        [Variation("EventTriggers2Model.xtc",  291,  300)]
        [Variation("EventTriggers2Model.xtc",  301,  310)]
        [Variation("EventTriggers2Model.xtc",  311,  320)]
        [Variation("EventTriggers2Model.xtc",  321,  330)]
        [Variation("EventTriggers2Model.xtc",  331,  340)]
        [Variation("EventTriggers2Model.xtc",  341,  350)]
        [Variation("EventTriggers2Model.xtc",  351,  360)]
        [Variation("EventTriggers2Model.xtc",  361,  370)]
        [Variation("EventTriggers2Model.xtc",  371,  380)]
        [Variation("EventTriggers2Model.xtc",  381,  390)]
        [Variation("EventTriggers2Model.xtc",  391,  400)]
        [Variation("EventTriggers2Model.xtc",  401,  410)]
        [Variation("EventTriggers2Model.xtc",  411,  420)]
        [Variation("EventTriggers2Model.xtc",  421,  430)]
        [Variation("EventTriggers2Model.xtc",  431,  440)]
        [Variation("EventTriggers2Model.xtc",  441,  450)]
        [Variation("EventTriggers2Model.xtc",  451,  460)]
        [Variation("EventTriggers2Model.xtc",  461,  470)]
        [Variation("EventTriggers2Model.xtc",  471,  479)]


        public EventTriggers2Model(string xtcFileName) : this(xtcFileName, -1) { }

        public EventTriggers2Model(string xtcFileName, int testCaseNumber)
            : this(xtcFileName, testCaseNumber, testCaseNumber) { }

        public EventTriggers2Model(string xtcFileName, int startTestCaseNumber, int endTestCaseNumber)
            : base(xtcFileName, startTestCaseNumber, endTestCaseNumber)
        {
            OnEndCase += new StateEventHandler(OnEndCase_Handler);

            //Add Action Handlers
            AddAction("Animate",       new ActionHandler(Animate));
        }

        #endregion


        #region Model setup and clean-up events

        /******************************************************************************
        * Function:          Animate
        ******************************************************************************/
        /// <summary>
        /// Animate: The (single) Model Action that is executed once for each test case combination.
        /// </summary>
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// <returns> Succeess status. </returns>
        /// 
        private bool Animate( State endState, State inParameters, State outParameters )
        {
            //outString += "--Model Action: Animate --\n";
            _testNumber++;



            try 
            {
                //NOTE:  the Model Action is invoked by the framework -before- SetWindowProperties and
                //OnBeginCase fire, so the window creation and content logic is moved here instead.
                //But window creation must occur only once, for all test cases.
                if (_testNumber == 1)
                {
                    SetWindowProperties();
                }

                SetWindowContent();

                _outString += "-------------------------------------------------\n";
                _outString += "BEGIN TEST #" + _testNumber + "\n";
                _outString += "-------------------------------------------------\n";
            
                //Create and start the Animation.
                SetupAnimation(inParameters, outParameters);
            }
            catch(Exception e)
            {
                _outString +=  _outString + "\n";
                _outString += "ERROR!! UNEXPECTED EXCEPTION:\n" + e.Message.ToString() + "\n";
                _testPassed = false;
            }
            return true;
        }

        /******************************************************************************
        * Function:          SetWindowProperties
        ******************************************************************************/
        /// <summary>
        /// SetWindowProperties: Apply properties to the Window object.
        /// </summary>
        /// <returns></returns>
        //private void SetWindowProperties(object sender, EventArgs e)
        private void SetWindowProperties()
        {
            _outString += "--SetWindowProperties--\n";
            
            Window.Title                = "EventTriggers2 Test";
            Window.Left                 = 10;
            Window.Top                  = 10;
            Window.Height               = 400;
            Window.Width                = 600;
            Window.WindowStyle          = WindowStyle.None;
            Window.ContentRendered     += new EventHandler(OnContentRendered);
        }
        
        /******************************************************************************
        * Function:          SetWindowContent
        ******************************************************************************/
        /// <summary>
        /// SetWindowContent: add elements to the NavigationWindow
        /// </summary>
        /// <returns>The content of the NavigationWindow</returns>
        private void SetWindowContent()
        {
            _outString += "--SetWindowContent--\n";

            NameScope.SetNameScope(Window, new NameScope());

            _body  = new Canvas();
            _body.Background    = Brushes.DarkViolet;
            _body.Height        = 400d;
            _body.Width         = 600d;

            //Create a to-be-animated element, when animationType = "KeyFrame`"
            _canvas1 = new Canvas();
            _canvas1.Name        = "CANVAS1";
            _canvas1.Background  = Brushes.Lavender;
            _canvas1.Height      = 150d;
            _canvas1.Width       = 250d;
            Canvas.SetZIndex (_canvas1, 0);
            _body.Children.Add(_canvas1);

            //Create a to-be-animated element, when animationType = "FE" or "Animatable"
            _progressBar = new ProgressBar();
            _progressBar.Background    = Brushes.MistyRose;
            _progressBar.Name          = "PROGRESS";
            _progressBar.Orientation   = Orientation.Horizontal;
            _progressBar.Value         = 0.2d;
            _progressBar.Height        = 40d;
            _progressBar.Margin        = new Thickness(25, 25, 25, 25);

            _scaleTransform = new ScaleTransform();
            _scaleTransform.ScaleX   = _initialScale;
            _scaleTransform.ScaleY   = _initialScale;
            _progressBar.RenderTransform = _scaleTransform;
            Window.RegisterName("SCALETRANS", _scaleTransform); //Provide the Animatable a Name.

            _canvas1.Children.Add(_progressBar);

            Canvas canvas2 = new Canvas();
            canvas2.Background  = Brushes.SlateBlue;
            canvas2.Height      = 150d;
            canvas2.Width       = 150d;
            Canvas.SetZIndex (canvas2, 1);
            Canvas.SetTop  (canvas2, 75d);
            Canvas.SetLeft (canvas2, 75d);
            _body.Children.Add(canvas2);
            
            Window.Content = _body;

            Window.RegisterName(_progressBar.Name, _progressBar);

            WaitForSignal("RenderPage");
        }
        
        /******************************************************************************
           * Function:          OnContentRendered
           ******************************************************************************/
        /// <summary>
        /// OnContentRendered: Invoked when the page content appears.
        /// </summary>
        private void OnContentRendered(object sender, EventArgs e)
        {
            _outString += "--OnContentRendered--\n";
            
            //Ensure the page has rendered before continuing the test.
            Signal("RenderPage", (TestResult)(_cumulativeResult ? TestResult.Pass : TestResult.Fail));
        }
        
        /******************************************************************************
           * Function:          OnTick
           ******************************************************************************/
        /// <summary>
        /// Invoked every time the DispatcherTimer ticks. Used to control the timing of verification.
        /// </summary>
        /// <returns>  </returns>
        private void OnTick(object sender, EventArgs e)
        {
            _dispatcherTickCount++;
            _outString += "----Tick #" + _dispatcherTickCount + "----\n";
            
            _aTimer.Stop();

            VerifyAnimation();

            //Signal that the Model code can finish the test case, and start the next one (if any).
            Signal("TestIsFinished", TestResult.Pass);
        }

        /******************************************************************************
        * Function:          OnEndCase_Handler
        ******************************************************************************/
        /// <summary>
        /// OnEndCase_Handler: Fires when each test case ends.
        /// </summary>
        /// <returns></returns>
        void OnEndCase_Handler(object sender, StateEventArgs e)
        {
            //outString += "-- OnEndCase\n";
            EndCase(e.State);
        }

        /******************************************************************************
        * Function:          EndCase
        ******************************************************************************/
        /// <summary>
        /// OnEndCase: Overrides the Model's EndCase, to clean up after each test case.
        /// </summary>
        /// <returns>True</returns>
        public override bool EndCase(State endCase)
        {
            //outString += "-- EndCase\n";

            _aTimer.Stop();
            
            if (_testPassed)
            {
                _outString += "END TEST #" + _testNumber + "                       RESULT: PASSED\n";
            }
            else
            {
                _outString += "END TEST #" + _testNumber + "                       RESULT: FAILED\n";
            }
            _outString += "-------------------------------------------------\n";

            _cumulativeResult = (_testPassed && _cumulativeResult);

            Reset();

            return true;
        }

        /******************************************************************************
        * Function:          CleanUp
        ******************************************************************************/
        /// <summary>
        /// Returns the final result to the frmwk for one or more test cases.
        /// </summary>
        /// <returns> true </returns>
        public override bool CleanUp()
        {
            _outString +="-------------------------------------------------\n";
            //Communicate to Model whether the tests pass or fail as a whole.
            //This allows -all- requested tests to run, even though one of them fails.
            if (_cumulativeResult)
            {
                _outString += "CUMULATIVE RESULT:  PASSED\n";
            }
            else
            {
                _outString += "CUMULATIVE RESULT:  FAILED\n";
            }
            
            //Print out all messages.
            GlobalLog.LogEvidence( _outString );

            return _cumulativeResult;
        }
        
        /******************************************************************************
        * Function:          SetupAnimation
        ******************************************************************************/
        /// <summary>
        /// SetupAnimation: create the Storyboard and Animation and begin.
        /// </summary>
        /// <returns></returns>
        private void SetupAnimation(State inParameters, State outParameters)
        {
            _outString += "--SetupAnimation--\n";

            _animationType           = inParameters["Animation"];
            _triggerType             = inParameters["TriggerType"];
            _locationStoryboard      = inParameters["StoryboardLocation"];
            _styleReference          = inParameters["Reference"];
            _styleType               = inParameters["Style"];
            _locationTriggerInStyle  = inParameters["TriggerLocation"];
            
            _outString += "------Model Input Parameters----------------\n";
            _outString += "- Animation:          " + _animationType + "\n";
            _outString += "- TriggerType:        " + _triggerType + "\n";
            _outString += "- StoryboardLocation: " + _locationStoryboard + "\n";
            _outString += "- Reference:          " + _styleReference + "\n";
            _outString += "- Style:              " + _styleType + "\n";
            _outString += "- TriggerLocation:    " + _locationTriggerInStyle + "\n";
            _outString += "--------------------------------------------\n";
            
            //Create a new Style to use for animation; it will eventually contain a Trigger.
            _triggerStyle = new Style();
            
            //The animated element can be either a Canvas or the ProgressBar inside it.
            if (_animationType == "KeyFrame")
            {
                _animatedElement = _canvas1;
                _triggerStyle.TargetType = typeof(Canvas);
            }
            else
            {
                _triggerStyle.TargetType = typeof(ProgressBar);
                _animatedElement = _progressBar;
            }
            
            //Creat a new Style and add it to the Root Dictionary; it will be used later for the
            //basedOn condition.
            _baseStyle = CreateBaseStyle();
            
            //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //(1) Create an Animation [and associated PropertyPath], and add it to a new Storyboard.
            //    A new Storyboard with Animation and an associated DependencyProperty are returned by ref.
            SetAnimation();
            
            //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //(2) Create a Trigger: Event / Property / Data / MultiData / Multi.
            //    A Style is returned containing a Trigger with a BeginStoryboard pointing to a Storyboard.
            CreateTrigger();
            
            //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //(3) Place the Storyboard, either on an element or in an element's Resources.
            SetStoryboardLocation();

            //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //(4) Create a Style containing the Trigger, either basic or BasedOn.
            SetStyle();
            
            //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //(5) Apply the Style (in either the animated element's Resources or the root's Resources).
            //    The Style has been established in the previous steps.
            ApplyStyle();
            
            //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //(6) Connect the Style to the animted DO, either as a StaticReference or a DynamicReference.
            SetReference();
            
            //----------------------------------------------------------------------------------------------------------------------------------------------------------------------

            //Use UIAutomation to start the Animation by moving the mouse over the triggering element.
            _progressBar.Focus();
            _outString += "--Starting the Animation via UIAutomation--\n";
            UserInput.MouseLeftClickCenter(_progressBar);  //Click.
 
            //Start a separate Timer to control the timing of verification.
            _outString += "----Start DispatcherTimer----\n";
            _dispatcherTickCount = 0;  //Needs to be reset for every case.
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = _TIMER_INTERVAL;
            _aTimer.Start();

            //Must wait for UIAutomation to trigger the animation and results to be verified,
            //before continuing the Model logic, which will end the test case.
            WaitForSignal("TestIsFinished");

        }
        
        /******************************************************************************
        * Function:          Reset
        ******************************************************************************/
        /// <summary>
        /// Reset: resets variables after each Test Case is verified.
        /// </summary>
        /// <returns></returns>
        private void Reset()
        {
            actCurrentState             = 0;
            actCurrentGlobalSpeed       = 0;
            _testPassed                  = false;
            
            _progressBar.Background = Brushes.Yellow;
            
            if (_story != null)
            {
                _story.Stop(_animatedElement);
                _story = null;
            }

            Window.UnregisterName(_beginStory.Name);
            Window.UnregisterName(_animatedElement.Name);
            
            _dp                  = null;
            _beginStory          = null;
            _rootDictionary      = null;
            _elementDictionary   = null;
            _triggerStyle        = null;
            _baseStyle           = null;
        }
        
        /******************************************************************************
        * Function:          CreateBaseStyle
        ******************************************************************************/
        /// <summary>
        /// CreateBaseStyle: creates a returns a new Style [later used for the BasedOn condition]
        /// </summary>
        /// <returns></returns>
        private Style CreateBaseStyle()
        {
            Style baseStyle = new Style();
            
            if (_animationType == "KeyFrame")
            {
                baseStyle.TargetType = typeof(Canvas);
                baseStyle.Setters.Add(new Setter(Canvas.HeightProperty, 200d));
            }
            else
            {
                baseStyle.TargetType = typeof(ProgressBar);
                baseStyle.Setters.Add(new Setter(ProgressBar.WidthProperty, 200d));
            }

            _animatedElement.Style = baseStyle;

            _rootDictionary = new ResourceDictionary();
            _rootDictionary.Add(_BASESTYLEKEY, baseStyle);
            ((FrameworkElement)Window).Resources = _rootDictionary;
            
            return baseStyle;
        }
        
        /******************************************************************************
        * Function:          SetAnimation
        ******************************************************************************/
        /// <summary>
        /// SetAnimation: creates an animation, adds it to a new Storyboard, and returns
        /// the Storyboard by reference.
        /// </summary>
        /// <returns></returns>
        private void SetAnimation()
        {
            _outString += "--SetAnimation--\n";
            
            PropertyPath        pp;
            
            _story = new Storyboard();
            _story.Name = "story";
            
            switch (_animationType)
            {
                case "FE" :
                    _animValue = CreateValueAnimation(_initialValue, _finalValue);
                    
                    _dp = ProgressBar.ValueProperty;
                    pp = new PropertyPath( "(0)", _dp );
                    
                    AttachEvents(_animValue);

                    Storyboard.SetTargetProperty(_animValue, pp);
                    _story.Children.Add(_animValue);
                    break;
                
                case "Animatable" :
                    _dp = ScaleTransform.ScaleXProperty;
                    _animScale = CreateScaleAnimation(_initialScale, _finalScale);
                    
                    pp = new PropertyPath("(0).(1)", new DependencyProperty[] { ProgressBar.RenderTransformProperty, _dp });
                    
                    AttachEvents(_animScale);

                    Storyboard.SetTargetProperty(_animScale, pp);
                    _story.Children.Add(_animScale);
                    break;
                
                case "KeyFrame" :
                    _dp = Panel.ZIndexProperty;
                    pp = new PropertyPath( "(0)", _dp );
                    
                    _animZIndex = CreateInt32KFAnimation();
                    
                    AttachEvents(_animZIndex);
                    
                    Storyboard.SetTargetProperty(_animZIndex, pp);
                    _story.Children.Add(_animZIndex);
                    break;

                default:
                    _outString += "ERROR!! SetAnimation: AnimationType was not found \n";
                    _testPassed = false;
                    break;
            }
            
            Window.RegisterName(_animatedElement.Name, _animatedElement);
        }

        /******************************************************************************
        * Function:          CreateValueAnimation
        ******************************************************************************/
        /// <summary>
        /// CreateValueAnimation: Creates a DoubleAnimation, for animating Value.
        /// </summary>
        /// <returns>A DoubleAnimation</returns>
        private DoubleAnimation CreateValueAnimation(double fromValue, double toValue)
        {
            _outString += "--CreateValueAnimation--\n";
            
            DoubleAnimation anim = new DoubleAnimation();
            anim.From            = fromValue;
            anim.To              = toValue;
            anim.BeginTime       = _BEGIN_TIME;
            anim.Duration        = _DURATION_TIME;
            
            return anim;
        }
        
        /******************************************************************************
        * Function:          CreateScaleAnimation
        ******************************************************************************/
        /// <summary>
        /// CreateScaleAnimation: Creates a second DoubleAnimation, for animating ScaleX.
        /// </summary>
        /// <returns>A DoubleAnimation</returns>
        private DoubleAnimation CreateScaleAnimation(double fromValue, double toValue)
        {
            _outString += "--CreateScaleAnimation--\n";
            
            DoubleAnimation anim = new DoubleAnimation();
            anim.From            = fromValue;
            anim.To              = toValue;
            anim.BeginTime       = _BEGIN_TIME;
            anim.Duration        = _DURATION_TIME;
            
            return anim;
        }
        
        /******************************************************************************
        * Function:          CreateInt32KFAnimation
        ******************************************************************************/
        /// <summary>
        /// CreateInt32KFAnimation: Creates a Int32AnimationUsingKeyFrames.
        /// </summary>
        /// <returns>A Int32UsingKeyFrames</returns>
        private Int32AnimationUsingKeyFrames CreateInt32KFAnimation()
        {
            _outString += "--CreateInt32KFAnimation--\n";

            Int32AnimationUsingKeyFrames anim = new Int32AnimationUsingKeyFrames();
            Int32KeyFrameCollection I32KFC = new Int32KeyFrameCollection();
            I32KFC.Add(new DiscreteInt32KeyFrame(_initialZIndex, KeyTime.FromPercent(0.50d)));
            I32KFC.Add(new DiscreteInt32KeyFrame(_finalZIndex,   KeyTime.FromPercent(0.99d)));
            anim.KeyFrames = I32KFC;
            anim.BeginTime              = _BEGIN_TIME;
            anim.Duration               = _DURATION_TIME;
            
            return anim;
        }
        
        /******************************************************************************
        * Function:          CreateTrigger
        ******************************************************************************/
        /// <summary>
        /// CreateTrigger: creates an BeginStoryboard, adds the Storyboard to it, then creates
        /// the appropriate Trigger and adds the BeginStoryboard to it.  The new Trigger is then
        /// added to a Style, which will later be associated with an element on the page.
        /// </summary>
        /// <returns></returns>
        private void CreateTrigger()
        {
            _outString += "--CreateTrigger--\n";
            
            _beginStory = AnimationUtilities.CreateBeginStoryboard(Window, "beginStory");
            
            switch (_triggerType)
            {
                case "Event" :
                    EventTrigger eTrigger = AnimationUtilities.CreateEventTrigger(System.Windows.Input.Mouse.PreviewMouseDownEvent, _beginStory);
                    _triggerStyle.Triggers.Add(eTrigger);
                    break;
                
                case "Property" :
                    Trigger pTrigger = AnimationUtilities.CreatePropertyTrigger(UIElement.IsMouseOverProperty, true, _beginStory);
                    _triggerStyle.Triggers.Add(pTrigger);
                    break;
                
                case "Data" :
                    DataTrigger dTrigger = AnimationUtilities.CreateDataTrigger(new PropertyPath("(0)", UIElement.IsMouseOverProperty), true, _beginStory);
                    _triggerStyle.Triggers.Add(dTrigger);
                    break;
                
                case "MultiData" :
                    MultiDataTrigger mdTrigger = AnimationUtilities.CreateMultiDataTrigger(_beginStory);

                    Binding b1 = AnimationUtilities.CreateBinding(new PropertyPath("(0)", UIElement.IsMouseOverProperty), RelativeSource.Self);
                    Condition c1 = AnimationUtilities.CreateCondition(b1, true); //Set via a mouse move using UIAutomation.
                    mdTrigger.Conditions.Add(c1);

                    Binding b2 = AnimationUtilities.CreateBinding(new PropertyPath("(0)", FrameworkElement.HeightProperty), RelativeSource.Self);
                    Condition c2 = AnimationUtilities.CreateCondition(b2, _animatedElement.Height); //Set in code.
                    mdTrigger.Conditions.Add(c2);

                    _triggerStyle.Triggers.Add(mdTrigger);
                    break;
                
                case "Multi" :
                    MultiTrigger mTrigger = AnimationUtilities.CreateMultiTrigger(_beginStory);
                    Condition c3 = AnimationUtilities.CreateCondition(FrameworkElement.HeightProperty, _animatedElement.Height);  //Set in code.
                    mTrigger.Conditions.Add(c3);

                    Condition c4 = AnimationUtilities.CreateCondition(UIElement.IsMouseOverProperty, true); //Set via a mouse move using UIAutomation.
                    mTrigger.Conditions.Add(c4);

                    _triggerStyle.Triggers.Add(mTrigger);
                    break;

                default:
                    _outString += "ERROR!! CreateTrigger: TriggerType was not found \n";
                    _testPassed = false;
                    break;
            }
        }
        
        /******************************************************************************
        * Function:          SetStoryboardLocation
        ******************************************************************************/
        /// <summary>
        /// SetStoryboardLocation: Attaches a Storyboard it to the specified object, after
        /// creating a new ResourceDictionary in some cases.
        /// Note: a non-null Style is returned only when locationStoryboard="InStyleResources".
        /// </summary>
        /// <returns></returns>
        private void SetStoryboardLocation()
        {
            _outString += "--SetStoryboardLocation--\n";
            
            switch (_locationStoryboard)
            {
                case "InTrigger" :
                    _beginStory.Storyboard = _story;   //Add the Storyboard to the BeginStoryboard.
                    break;
                
                case "InElementResources" :
                    _elementDictionary = new ResourceDictionary();
                    _elementDictionary.Add("StoryKey", _story);
                    ((FrameworkElement)_animatedElement).Resources = _elementDictionary;
                    
                    Storyboard storyInElement = (Storyboard)((FrameworkElement)_animatedElement).FindResource("StoryKey");
                    _beginStory.Storyboard = storyInElement;
                    break;
                
                case "InRootResources" :
                    _rootDictionary = new ResourceDictionary();
                    _rootDictionary.Add("StoryKey", _story);
                    ((FrameworkElement)Window).Resources = _rootDictionary;
                    
                    Storyboard storyInRoot = (Storyboard)Window.FindResource("StoryKey");
                    _beginStory.Storyboard = storyInRoot;
                    break;
                
                case "InStyleResources" :
                    _triggerStyle.Resources.BeginInit();
                    _triggerStyle.Resources.Add("StoryKey", _story);
                    _triggerStyle.Resources.EndInit();
                    
                    object o = _triggerStyle.Resources["StoryKey"];
                    Storyboard storyInStyle = (Storyboard)o;
                    _beginStory.Storyboard = storyInStyle;
                    break;

                default:
                    _outString += "ERROR!! SetStoryboardLocation: Storyboard Location was not found \n";
                    _testPassed = false;
                    break;
            }
        }
        
        /******************************************************************************
        * Function:          SetStyle
        ******************************************************************************/
        /// <summary>
        /// SetStyle: if BasedOn requested, will base the Style on another Style, and return
        /// the Style.
        /// </summary>
        /// <returns></returns>
        private void SetStyle()
        {
            _outString += "--SetStyle--\n";

            switch (_styleType)
            {
                case "Basic" :
                    //Do nothing.
                    break;
                
                case "BasedOn" :
                    _triggerStyle.BasedOn = _baseStyle;
                    break;

                default:
                    _outString += "ERROR!! SetStyle: Style Type was not found \n";
                    _testPassed = false;
                    break;
            }
        }
        
        /******************************************************************************
        * Function:          ApplyStyle
        ******************************************************************************/
        /// <summary>
        /// SetSource: Assign a Style to either the animated element or the root element.
        /// The animated element can either be a Canvas or a ProgressBar.
        /// </summary>
        /// <returns></returns>
        private void ApplyStyle()
        {
            _outString += "--ApplyStyle--\n";
            
            switch (_locationTriggerInStyle)
            {
                case "StyleElement" :
                    if (_elementDictionary == null)
                    {
                        _elementDictionary = new ResourceDictionary();
                    }

                    _elementDictionary.Add(_STYLEKEY, _triggerStyle);
                    ((FrameworkElement)_animatedElement).Resources = _elementDictionary;
                    break;

                case "StyleRoot" :
                    if (_rootDictionary == null)
                    {
                        _rootDictionary = new ResourceDictionary();
                    }
                    _rootDictionary.Add(_STYLEKEY, _triggerStyle);
                    ((FrameworkElement)Window).Resources = _rootDictionary;
                    break;

                default:
                    _outString += "ERROR!! ApplyStyle: TriggerLocation was not found \n";
                    _testPassed = false;
                    break;
            }
        }
        
        /******************************************************************************
        * Function:          SetReference
        ******************************************************************************/
        /// <summary>
        /// SetReference: Create a Dynamic or Static Reference to connect a Style to a specific FE.
        /// </summary>
        /// <returns></returns>
        private void SetReference()
        {
            _outString += "----SetReference----\n";
           
            switch (_styleReference)
            {
                case "Dynamic" :
                    _animatedElement.SetResourceReference(FrameworkElement.StyleProperty, _STYLEKEY);
                    break;

                case "Static" :
                    switch (_locationTriggerInStyle)
                    {
                        case "StyleElement" :
                            _animatedElement.Style = (Style)_animatedElement.Resources["StyleKey"];
                            break;

                        case "StyleRoot" :
                            _animatedElement.Style = (Style)Window.Resources["StyleKey"];
                            break;

                        default:
                            _outString += "ERROR!! SetReference: TriggerLocation was not found \n";
                            _testPassed = false;
                            break;
                    }
                    break;
                    
                default:
                    _outString += "ERROR!! SetReference: Style Reference was not found \n";
                    _testPassed = false;
                    break;
            }
        }
        
        /******************************************************************************
        * Function:          AttachEvents
        ******************************************************************************/
        /// <summary>
        /// AttachEvents: Associates Animation events with an Animation.
        /// </summary>
        /// <returns></returns>
        private void AttachEvents (AnimationTimeline AT)
        {
            AT.CurrentStateInvalidated          += new EventHandler(OnCurrentStateInvalidated);
            AT.CurrentGlobalSpeedInvalidated    += new EventHandler(OnCurrentGlobalSpeedInvalidated);
        }
        
        /******************************************************************************
        * Function:          DetachEvents
        ******************************************************************************/
        /// <summary>
        /// DetachEvents: Removes Animation events.
        /// </summary>
        /// <returns></returns>
        private void DetachEvents (Clock clock)
        {
            clock.CurrentStateInvalidated          -= new EventHandler(OnCurrentStateInvalidated);
            clock.CurrentGlobalSpeedInvalidated    -= new EventHandler(OnCurrentGlobalSpeedInvalidated);
        }

        /******************************************************************************
           * Function:          OnCurrentStateInvalidated
           ******************************************************************************/
        /// <summary>
        /// OnCurrentStateInvalidated: Used to validate event firing.
        /// </summary>
        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {               
            Clock storyboardClock = (Clock)sender;
            _outString += "CurrentStateInvalidated:  " + storyboardClock.CurrentState + "\n";
            
            if (storyboardClock.CurrentState == ClockState.Filling)
            {
                DetachEvents(storyboardClock);
            }
            actCurrentState++;
        }

        /******************************************************************************
           * Function:          OnCurrentGlobalSpeedInvalidated
           ******************************************************************************/
        /// <summary>
        /// OnCurrentGlobalSpeedInvalidated: Used to validate event firing.
        /// </summary>
        private void OnCurrentGlobalSpeedInvalidated(object sender, EventArgs e)
        {
            _outString += "CurrentGlobalSpeedInvalidated:  " + ((Clock)sender).CurrentGlobalSpeed + "\n";
            actCurrentGlobalSpeed++;
        }
        
        /******************************************************************************
           * Function:          VerifyAnimation
           ******************************************************************************/
        /// <summary>
        /// Verify the Animation DP value after the Animation has finished.
        /// </summary>
        /// <returns>  </returns>
        private void VerifyAnimation()
        {
            _outString += "----VerifyAnimation----\n";
            
            bool dpValueCorrect = false;
            
            _outString += "-----------------RESULTS--------------------------\n";
            switch (_animationType)
            {
                case "FE" :
                    double tolerance = 0.01;
                    dpValueCorrect = ( (_progressBar.Value <= _finalValue + tolerance) && (_progressBar.Value >= _finalValue - tolerance) );
                    _outString += "      GetValue:        Actual: " + _progressBar.Value + "  Expected: " + _finalValue + "\n";
                    break;
                case "Animatable" :
                    dpValueCorrect = (_scaleTransform.ScaleX == _finalScale);
                    _outString += "      GetValue:        Actual: " + _scaleTransform.ScaleX + "  Expected: " + _finalScale + "\n";
                    break;
                case "KeyFrame" :
                    int actualZIndex = Panel.GetZIndex(_canvas1);
                    dpValueCorrect = (actualZIndex == _finalZIndex);
                    _outString += "      GetValue:        Actual: " + actualZIndex + " Expected: " + _finalZIndex + "\n";
                    break;

                default:
                    _outString += "ERROR!! VerifyAnimation: AnimationType was not found \n";
                    _testPassed = false;
                    break;
             }
             
            _outString += "      CurrentStateFired:       Act: " + actCurrentState +        "  Exp: >= " + expCurrentState + "\n";
            _outString += "      CurrentGlobalSpeedFired: Act: " + actCurrentGlobalSpeed +  "  Exp: >=" + expCurrentGlobalSpeed + "\n";
            
            _testPassed = (dpValueCorrect && (actCurrentState >= expCurrentState) && (actCurrentGlobalSpeed >= expCurrentGlobalSpeed));
            _outString += "      RESULT: " + _testPassed + "\n";
            _outString += "--------------------------------------------------\n";
        }
        #endregion
    }
}
