// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
* ***************  MIL Animation Test *******************************************************
*     Purpose:            This framework tests the integration of Animation and Element Services,
*                         focusing on EventTriggers in a variety of scenarios.  The Model is 
*                         stateless and consists of the following test matrix (parameters):
*                             (a) Location of the EventTrigger
*                             (b) Location of the Storyboard
*                             (c) Type of Animation
*                             (d) SourceName presence
*                             (e) Location of the animated DP
*                         The sequence of events in this test case is:
*                             (1) the page containing a ScrollBar and a Button is rendered.
*                             (2) The Storyboard is set up, based on the Model parameters.
*                             (3) A DispatcherTime is started, used to control the timing of verification.
*                             (4) Using UIAutomation, the mouse is moved over the Source Element.
*                             (5) The Animation is triggered.
*                             (6) The Animation finishes and is verified.
*     Pass Conditions:    A test case passes if (a) GetValue (at the end of the animation) returns
*                         the correct value of the animated DP, and (b) Animation events fire correctly.
*     How verified:       
*     Framework:          A CLR executable is created.
*     Area:               Animation/Timing
*     Dependencies:       TestRuntime.dll, AnimationFramework.dll
*     Support Files:      EventTriggers1Model.xtc file, which specifies the test cases [must be available at run time].
*********************************************************************************************/
using System;
using System.Xml;
using System.Data;
using System.Collections;
using System.Threading;
using System.Windows;
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
    [Test(2, "Storyboard.Models.Triggers", "EventTriggers1Model", SupportFiles=@"FeatureTests\Animation\EventTriggers1Model.xtc")]

    class EventTriggers1Model : WindowModel
    {
        #region Test case members

        private DispatcherTimer                 _aTimer                  = null;
        private int                             _dispatcherTickCount     = 0;
        private int                             _testNumber              = 0;
        private Canvas                          _body                    = null;
        private ScrollBar                       _scrollBar               = null;
        private Button                          _button                  = null;
        private LineGeometry                    _lineGeometry            = null;
        private DrawingBrush                    _drawBrush               = null;
        private ObjectAnimationUsingKeyFrames   _animObject              = null;
        private DoubleAnimation                 _animDouble              = null;
        private PointAnimation                  _animAnimatable          = null;
        
        private double                          _initialDouble           = 0.2d;
        private Point                           _initialPoint            = new Point(0d, 0.2d);
        private Orientation                     _initialEnum             = Orientation.Horizontal;
        private double                          _finalDouble             = 0.9d;
        private Point                           _finalPoint              = new Point(0d, 5d);
        private Orientation                     _finalEnum               = Orientation.Vertical;
        
        //Input values from the Model.
        private string                          _locationTrigger         = "";
        private string                          _locationStoryboard      = "";
        private string                          _animationType           = "";
        private string                          _sourceName              = "";
        private string                          _locationDP              = "";

        public bool                             currentStateFired       = false;
        public bool                             currentGlobalSpeedFired = false;

        private TimeSpan                        _BEGIN_TIME              = TimeSpan.FromMilliseconds(0);
        private Duration                        _DURATION_TIME           = new Duration(TimeSpan.FromMilliseconds(1000));
        private TimeSpan                        _TIMER_INTERVAL          = TimeSpan.FromMilliseconds(3000);

        private bool                            _testPassed              = true;
        private bool                            _cumulativeResult        = true;
        private string                          _outString               = "";

        #endregion


        #region Contructors


        [Variation("EventTriggers1Model.xtc",    1,15)]
        [Variation("EventTriggers1Model.xtc",    16,30)]
        [Variation("EventTriggers1Model.xtc",    31,45)]
        [Variation("EventTriggers1Model.xtc",    46,60)]
        [Variation("EventTriggers1Model.xtc",    61,75)]
        [Variation("EventTriggers1Model.xtc",    76,90)]
        [Variation("EventTriggers1Model.xtc",    91,105)]
        [Variation("EventTriggers1Model.xtc",    106,120)]
        [Variation("EventTriggers1Model.xtc",    121,135)]
        [Variation("EventTriggers1Model.xtc",    136,150)]
        [Variation("EventTriggers1Model.xtc",    151,165)]
        [Variation("EventTriggers1Model.xtc",    166,180)]
        [Variation("EventTriggers1Model.xtc",    181,191)]

        public EventTriggers1Model(string xtcFileName) : this(xtcFileName, -1) { }

        public EventTriggers1Model(string xtcFileName, int testCaseNumber)
            : this(xtcFileName, testCaseNumber, testCaseNumber) { }

        public EventTriggers1Model(string xtcFileName, int startTestCaseNumber, int endTestCaseNumber)
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
            //outString += "----Model Action: Animate\n";
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
            
                //Click on the Button to ensure a mouse move to the ScrollBar takes place later.
                UserInput.MouseLeftClickCenter(_button);

                _locationTrigger     = inParameters["TriggerLocation"];
                _locationStoryboard  = inParameters["StoryboardLocation"];
                _animationType       = inParameters["Animation"];
                _sourceName          = inParameters["SourceName"];
                _locationDP          = inParameters["DPLocation"];

                _outString +="------Model Input Parameters----------------\n";
                _outString +="- Animation:          " + _animationType        + "\n";
                _outString +="- TriggerLocation:    " + _locationTrigger      + "\n";
                _outString +="- StoryboardLocation: " + _locationStoryboard   + "\n";
                _outString +="- SourceName:         " + _sourceName           + "\n";
                _outString +="- DPLocation:         " + _locationDP           + "\n";
                _outString +="--------------------------------------------\n";

                //Use the Model's input paramters to set up the Animation on the ScrollBar.
                DependencyProperty  dp          = null;
                BeginStoryboard     beginStory  = null;
                Storyboard          story       = null;
                EventTrigger        trigger     = null;
                ResourceDictionary  rootDict    = null;
                ResourceDictionary  elementDict = null;
                Style               style       = null;

                //-------------------------------------------------------------------------------------------------
                //(1) Create an Animation [and associated PropertyPath], and add it to a new Storyboard.
                SetAnimation(_animationType, ref dp, ref story);

                //-------------------------------------------------------------------------------------------------
                //(2) Create an EventTrigger and add it to either the animated element or the root.
                SetTriggerLocation(_locationTrigger, story, ref trigger, ref beginStory);

                //-------------------------------------------------------------------------------------------------
                //(3) Place the Storyboard, either on an element or in an element's Resources.
                SetStoryboardLocation(_locationStoryboard, _scrollBar, story, trigger, ref rootDict, ref elementDict, ref style, ref beginStory);

                //-------------------------------------------------------------------------------------------------
                //(4) Trigger the Animation from a different element, if requested.
                SetSource(_sourceName, ref trigger);

                //-------------------------------------------------------------------------------------------------
                //(5) Place the animated DP, either on the animated element or an element's Resources.
                SetDPLocation(_locationDP, _animationType, _scrollBar, dp, ref rootDict, ref elementDict, style);


                //Use UIAutomation to start the Animation by moving the mouse over the triggering element.
                _scrollBar.Focus();
                UserInput.MouseLeftClickCenter(_scrollBar);  //Click.

                if (_sourceName == "Absent")
                {
                    _scrollBar.Focus();
                    //UserInput.MouseMove(scrollBar,5,5);
                    UserInput.MouseLeftClickCenter(_scrollBar);  //Click.
                }
                else
                {
                    _body.Focus();
                    UserInput.MouseLeftDown(_body);   //Click 4,4 from top-left corner.
                }


                //Start a separate Timer to control the timing of verification.
                _outString += "----Start DispatcherTimer----\n";
                _dispatcherTickCount = 0;  //Needs to be reset for every case.
                _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
                _aTimer.Tick += new EventHandler(OnTick);
                _aTimer.Interval = _TIMER_INTERVAL;
                _aTimer.Start();

                //Must wait for UIAutomation to trigger the animation and results to be verified,
                //before continuing the Model logic, which will end the test case.
                WaitForSignal("VerifyResults");

                //End of test case: clean up.
                switch (_animationType)
                {
                    case "FE" :
                        DetachEvents(_animDouble);
                        break;
                    case "Animatable" :
                        DetachEvents(_animAnimatable);
                        break;
                    case "KeyFrame" :
                        DetachEvents(_animObject);
                        break;
                    default:
                        GlobalLog.LogEvidence("ERROR!! Animate: AnimationType was not found \n");
                        _testPassed = false;
                        break;
                }
                if (story != null)
                {
                    story.Remove(_scrollBar);
                }

                story.BeginAnimation(dp, null);
                story = null;

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();

                Window.UnregisterName(beginStory.Name);

                dp                      = null;
                beginStory              = null;
                trigger                 = null;
                rootDict                = null;
                elementDict             = null;
                style                   = null;
            }
            catch(Exception e)
            {
                GlobalLog.LogEvidence( _outString );
                GlobalLog.LogEvidence("ERROR!! UNEXPECTED EXCEPTION:\n" + e.Message.ToString());
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
            _outString += "-- SetWindowProperties\n";
            
            Window.Title                = "Trigger Variations Test";
            Window.Left                 = 50;
            Window.Top                  = 50;
            Window.Height               = 200;
            Window.Width                = 400;
            Window.WindowStyle          = WindowStyle.None;
            Window.ContentRendered     += new EventHandler(OnContentRendered);
        }

        /******************************************************************************
        * Function:          SetWindowContent
        ******************************************************************************/
        /// <summary>
        /// SetWindowContent: Add content to the Window.
        /// </summary>
        /// <returns></returns>
        void SetWindowContent()
        {
            _outString += "-- SetWindowContent\n";
            
            NameScope.SetNameScope(Window, new NameScope());

            _body  = new Canvas();
            _body.Background     = Brushes.OrangeRed;
            _body.Height         = 150d;
            _body.Width          = 300d;
           
            _scrollBar = new ScrollBar();
            _scrollBar.Background    = Brushes.Yellow;
            _scrollBar.Name          = "SCROLL";
            _scrollBar.Orientation   = _initialEnum;
            _scrollBar.Value         = 0.2d;
            _scrollBar.Height        = 30d;
            _scrollBar.Width         = 90d;
            _scrollBar.ViewportSize  = 0.2d;
            
            //Create a DrawingBrush with LineGeometry, which later may be set as the ScollBar's Background.
            _lineGeometry = new LineGeometry();
            _lineGeometry.StartPoint = new Point(0d, 0.2d);
            _lineGeometry.EndPoint   = new Point(1d, 0.2d);
            Window.RegisterName("LINEGEO", _lineGeometry); //Provide the Animatable a Name.

            GeometryDrawing geoDrawing = new GeometryDrawing();
            geoDrawing.Pen = new Pen( Brushes.Purple, 3d );
            geoDrawing.Geometry = _lineGeometry;

            _drawBrush = new DrawingBrush();
            _drawBrush.Drawing = geoDrawing;
            //--------------------------------------------------------------------------------
            
            Canvas.SetTop  (_scrollBar, 50d);
            Canvas.SetLeft (_scrollBar, 50d);
            _body.Children.Add(_scrollBar);

            _button = new Button();
            _button.Name     = "AlternativeAnimationSource";
            _button.Height   = 20d;
            _button.Width    = 40d;
            Canvas.SetTop  (_button, 10d);
            Canvas.SetLeft (_button, 10d);
            _body.Children.Add(_button);
            
            Window.Content = _body;

            Window.RegisterName(_scrollBar.Name, _scrollBar);

            WaitForSignal("RenderPage");
        }

        /******************************************************************************
           * Function:          OnContentRendered
           ******************************************************************************/
        /// <summary>
        /// OnContentRendered: Invoked when the page content appears.
        /// A Signal/WaitForSignal is used to ensure that Animation will not occur before
        /// the page content is rendered.
        /// </summary>
        private void OnContentRendered(object sender, EventArgs e)
        {
            _outString +="-- OnContentRendered\n";
            
            //Ensure the page has rendered before continuing the test.
            Signal("RenderPage", TestResult.Pass);
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
            
            VerifyAnimation();
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

            currentStateFired       = false;
            currentGlobalSpeedFired = false;
            
            _scrollBar.Background = Brushes.Yellow;
            
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
            
            _testPassed = false;
            
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
        * Function:          SetAnimation
        ******************************************************************************/
        /// <summary>
        /// SetAnimation: creates an animation, adds it to a new Storyboard, and returns
        /// the Storyboard by reference.
        /// </summary>
        /// <returns></returns>
        private void SetAnimation(string typeOfAnimation, ref DependencyProperty dp, ref Storyboard storyboard)
        {
            _outString += "----SetAnimation----\n";
            
            PropertyPath        pp;
            
            storyboard = new Storyboard();
            storyboard.Name = "story";
            
            switch (typeOfAnimation)
            {
                case "FE" :
                    _animDouble = CreateDoubleAnimation(_scrollBar, _initialDouble, _finalDouble);
                    
                    dp = ScrollBar.ValueProperty;
                    pp = new PropertyPath( "(0)", dp );
                    
                    AttachEvents(_animDouble);

                    Storyboard.SetTargetProperty(_animDouble, pp);
                    Storyboard.SetTargetName(_animDouble, _scrollBar.Name);
                    storyboard.Children.Add(_animDouble);
                    break;
                
                case "Animatable" :
                    dp = LineGeometry.StartPointProperty;
                    _animAnimatable = CreatePointAnimation(_lineGeometry, _initialPoint, _finalPoint);
                    
                    pp = new PropertyPath("(0)", dp);
                    
                    AttachEvents(_animAnimatable);

                    Storyboard.SetTargetProperty(_animAnimatable, pp);
                    Storyboard.SetTargetName(_animAnimatable, "LINEGEO");
                    storyboard.Children.Add(_animAnimatable);
                    break;
                
                case "KeyFrame" :
                    dp = ScrollBar.OrientationProperty;
                    pp = new PropertyPath( "(0)", dp );
                    
                    _animObject = CreateObjectKFAnimation(_scrollBar, dp);
                    
                    AttachEvents(_animObject);
                    
                    Storyboard.SetTargetProperty(_animObject, pp);
                    Storyboard.SetTargetName(_animObject, _scrollBar.Name);
                    storyboard.Children.Add(_animObject);
                    break;

                default:
                    GlobalLog.LogEvidence("ERROR!! SetAnimation: AnimationType was not found \n");
                    _testPassed = false;
                    break;
            }
        }
        
        /******************************************************************************
        * Function:          SetTriggerLocation
        ******************************************************************************/
        /// <summary>
        /// SetTriggerLocation:  Creates an EventTrigger and adds it to the appropriate element.
        /// </summary>
        /// <returns>An EventTrigger, by ref</returns>
        private void SetTriggerLocation(string locationOfTrigger, Storyboard storyboard, ref EventTrigger trigger1, ref BeginStoryboard begin)
        {
            _outString +="----SetTriggerLocation----\n";

            begin = AnimationUtilities.CreateBeginStoryboard(Window, "beginStory");
            begin.Storyboard = storyboard;         //Add the Storyboard to the BeginStoryboard.

            trigger1 = AnimationUtilities.CreateEventTrigger(System.Windows.Input.Mouse.PreviewMouseDownEvent, begin);

            switch (locationOfTrigger)
            {

                case "Element" :
                    _scrollBar.Triggers.Add(trigger1);
                    break;
                
                case "Root" :
                    Window.Triggers.Add(trigger1);
                    break;

                default:
                    GlobalLog.LogEvidence("ERROR!! SetTriggerLocation: Trigger Location was not found \n");
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
        /// Note: a non-null Style is returned only when locatonOfStoryboard="InStyleResources".
        /// </summary>
        /// <returns></returns>
        private void SetStoryboardLocation(string                  locationOfStoryboard,
                                           DependencyObject        DO,
                                           Storyboard              storyboard,
                                           EventTrigger            trigger,
                                           ref ResourceDictionary  rootDictionary,
                                           ref ResourceDictionary  elementDictionary,
                                           ref Style               style,
                                           ref BeginStoryboard     beginStoryboard)
        {
            _outString +="----SetStoryboardLocation----\n";
            
            switch (locationOfStoryboard)
            {
                case "InTrigger" :
                    //Do nothing.
                    break;
                
                case "InElementResources" :
                    elementDictionary = new ResourceDictionary();
                    elementDictionary.Add("StoryKey", storyboard);
                    ((FrameworkElement)DO).Resources = elementDictionary;
                    
                    Storyboard storyInElement = (Storyboard)((FrameworkElement)DO).FindResource("StoryKey");
                    beginStoryboard.Storyboard = storyInElement;
                    break;
                
                case "InRootResources" :
                    rootDictionary = new ResourceDictionary();
                    rootDictionary.Add("StoryKey", storyboard);
                    ((FrameworkElement)Window).Resources = rootDictionary;
                    
                    Storyboard storyInRoot = (Storyboard)Window.FindResource("StoryKey");
                    beginStoryboard.Storyboard = storyInRoot;
                    break;
                
                case "InStyleResources" :
                    style = new Style();
                    style.TargetType = DO.GetType();
                    style.Triggers.Add(trigger);
                    style.Resources.BeginInit();
                    style.Resources.Add("StoryKey", storyboard);
                    style.Resources.EndInit();
                    
                    object o = style.Resources["StoryKey"];
                    Storyboard storyInStyle = (Storyboard)o;
                    beginStoryboard.Storyboard = storyInStyle;
                    break;

                default:
                    GlobalLog.LogEvidence("ERROR!! SetStoryboardLocation: Storyboard Location was not found \n");
                    _testPassed = false;
                    break;
            }
        }
        
        /******************************************************************************
        * Function:          SetSource
        ******************************************************************************/
        /// <summary>
        /// SetSource: Depending on the Model parameter, sets a SourceName on the EventTrigger,
        /// so that the triggering element is different than the animated element.
        /// </summary>
        /// <returns></returns>
        private void SetSource(string source, ref EventTrigger trigger)
        {
            _outString +="----SetSource----\n";

            switch (_sourceName)
            {
                case "Present" :
                    trigger.SourceName = _body.Name;
                    break;
                
                case "Absent" :
                    //Do nothing: the triggering element is the same as the animated element.
                    break;

                default:
                    GlobalLog.LogEvidence("ERROR!! SetSource: Source Name was not found \n");
                    _testPassed = false;
                    break;
            }
        }
        
        /******************************************************************************
        * Function:          SetDPLocation
        ******************************************************************************/
        /// <summary>
        /// SetDPLocation: Place the to-be-animated DP directly on the element, or in an Element's
        /// Style.
        /// </summary>
        /// <returns></returns>
        private void SetDPLocation(string                   locationOfDP,
                                   string                   animationType,
                                   DependencyObject         DO,
                                   DependencyProperty       dp,
                                   ref ResourceDictionary   rootDictionary,
                                   ref ResourceDictionary   elementDictionary,
                                   Style                    storyboardStyle)
        {
            _outString +="----SetDPLocation----\n";

            Style dpStyle = new Style();
            dpStyle.TargetType = DO.GetType();
            if (storyboardStyle != null)
            {
                dpStyle.BasedOn = storyboardStyle; //Combine with the Storyboard style, if it exists.
            }
            
            DynamicResourceExtension dynamicResourceExt = new DynamicResourceExtension();

            switch (_locationDP)
            {
                case "Element" :
                    //The DP is set directly on the Animated element.
                    switch (animationType)
                    {
                        case "FE" :
                            DO.SetValue(dp, _initialDouble);
                            break;

                        case "Animatable" :
                            _lineGeometry.SetValue(dp, _initialPoint);
                            _scrollBar.Background = _drawBrush;
                            break;

                        case "KeyFrame" :
                            DO.SetValue(dp, _initialEnum);
                            break;

                        default:
                            GlobalLog.LogEvidence( "ERROR!! SetDPLocation: AnimationType was not found \n");
                            _testPassed = false;
                            break;
                    }
                    break;
                
                case "RootResources" :
                    //Place the dp in a style inside the root element's resources.
                    switch (animationType)
                    {
                        case "FE" :
                            dpStyle.Setters.Add(new Setter(dp, _initialDouble));
                            break;

                        case "Animatable" :
                            dpStyle.Setters.Add(new Setter(dp, _initialPoint));
                            _scrollBar.Background = _drawBrush;
                            break;

                        case "KeyFrame" :
                            dpStyle.Setters.Add(new Setter(dp, _initialEnum));
                            break;

                        default:
                            GlobalLog.LogEvidence("ERROR!! SetDPLocation: AnimationType was not found \n");
                            _testPassed = false;
                            break;
                    }
                    //dynamicResourceExt.ResourceKey = "StyleKeyRoot";
                    //dpStyle.Setters.Add(new Setter(dp, dynamicResourceExt));
                    if (rootDictionary == null)
                    {
                        rootDictionary = new ResourceDictionary();
                    }
                    rootDictionary.Add("DPKey", dpStyle);
                    //body.Style = dpStyle;
                    break;
                
                case "ElementResources" :
                    //Place the dp in a style inside the Animated element's resources.
                    switch (animationType)
                    {
                        case "FE" :
                            dpStyle.Setters.Add(new Setter(dp, _initialDouble));
                            break;

                        case "Animatable" :
                            dpStyle.Setters.Add(new Setter(dp, _initialPoint));
                            _scrollBar.Background = _drawBrush;
                            break;

                        case "KeyFrame" :
                            dpStyle.Setters.Add(new Setter(dp, _initialEnum));
                            break;

                        default:
                            GlobalLog.LogEvidence("ERROR!! SetDPLocation: AnimationType was not found \n");
                            _testPassed = false;
                            break;
                    }
                    if (elementDictionary == null)
                    {
                        elementDictionary = new ResourceDictionary();
                    }
                    elementDictionary.Add("DPKey", dpStyle);
                    //Associate a Key to the style, which then can be referenced by the animated DO.
                    //Then add the style to the DO.
                    //DynamicResourceExtension dynamicResourceExt2 = new DynamicResourceExtension();
                    //dynamicResourceExt.ResourceKey = "StyleKeyElement";
                    //style.Setters.Add(new Setter(dp, dynamicResourceExt));
                    //((FrameworkElement)DO).Style = style;
                    break;
                
                case "StyleResources" :
                    //The generic style being passed in is on the NavigationWindow; it may also
                    //contain the Storyboard.
                    switch (animationType)
                    {
                        case "FE" :
                            dpStyle.Setters.Add(new Setter(dp, _initialDouble));
                            break;

                        case "Animatable" :
                            dpStyle.Setters.Add(new Setter(dp, _initialPoint));
                            _scrollBar.Background = _drawBrush;
                            break;

                        case "KeyFrame" :
                            dpStyle.Setters.Add(new Setter(dp, _initialEnum));
                            break;

                        default:
                            GlobalLog.LogEvidence("ERROR!! SetDPLocationAnimationType was not found \n");
                            _testPassed = false;
                            break;
                    }
                    break;

                default:
                    GlobalLog.LogEvidence("ERROR!! SetDPLocation: DP Location was not found \n");
                    _testPassed = false;
                    break;
            }
        }
        
        /******************************************************************************
        * Function:          CreateDoubleAnimation
        ******************************************************************************/
        /// <summary>
        /// CreateDoubleAnimation: Creates a DoubleAnimation.
        /// </summary>
        /// <returns>A DoubleAnimation</returns>
        private DoubleAnimation CreateDoubleAnimation(DependencyObject DO, double fromValue, double toValue)
        {
            _outString +="----CreateDoubleAnimation----\n";
            
            DoubleAnimation anim = new DoubleAnimation();
            anim.From            = fromValue;
            anim.To              = toValue;
            anim.BeginTime       = _BEGIN_TIME;
            anim.Duration        = _DURATION_TIME;
            
            return anim;
        }
        
        /******************************************************************************
        * Function:          CreatePointAnimation
        ******************************************************************************/
        /// <summary>
        /// CreatePointAnimation: Creates a PointAnimation.
        /// </summary>
        /// <returns>A PointAnimation</returns>
        private PointAnimation CreatePointAnimation(DependencyObject DO, Point fromValue, Point toValue)
        {
            _outString +="----CreatePointAnimation----\n";
            
            PointAnimation anim = new PointAnimation();
            anim.From            = fromValue;
            anim.To              = toValue;
            anim.BeginTime       = _BEGIN_TIME;
            anim.Duration        = _DURATION_TIME;
            
            return anim;
        }
        
        /******************************************************************************
        * Function:          CreateObjectKFAnimation
        ******************************************************************************/
        /// <summary>
        /// CreateObjectKFAnimation: Creates a ObjectAnimationUsingKeyFramesAnimation.
        /// </summary>
        /// <returns>A ObjectAnimationUsingKeyFrames</returns>
        private ObjectAnimationUsingKeyFrames CreateObjectKFAnimation(DependencyObject DO, DependencyProperty DP)
        {
            _outString +="----Create KeyFramesAnimation----\n";

            ObjectAnimationUsingKeyFrames anim = new ObjectAnimationUsingKeyFrames();
            ObjectKeyFrameCollection OKFC = new ObjectKeyFrameCollection();
            OKFC.Add(new DiscreteObjectKeyFrame(_initialEnum, KeyTime.FromPercent(0.50d)));
            OKFC.Add(new DiscreteObjectKeyFrame(_finalEnum,   KeyTime.FromPercent(0.99d)));
            anim.KeyFrames = OKFC;

            anim.BeginTime              = _BEGIN_TIME;
            anim.Duration               = _DURATION_TIME;
            
            return anim;
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
        private void DetachEvents (AnimationTimeline AT)
        {
            AT.CurrentStateInvalidated          -= new EventHandler(OnCurrentStateInvalidated);
            AT.CurrentGlobalSpeedInvalidated    -= new EventHandler(OnCurrentGlobalSpeedInvalidated);
        }

        /******************************************************************************
           * Function:          OnCurrentStateInvalidated
           ******************************************************************************/
        /// <summary>
        /// OnCurrentStateInvalidated: Used to validate event firing.
        /// </summary>
        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {               
            _outString +="CurrentStateInvalidated:  " + ((Clock)sender).CurrentState + "\n";
            currentStateFired = true;
        }

        /******************************************************************************
           * Function:          OnCurrentGlobalSpeedInvalidated
           ******************************************************************************/
        /// <summary>
        /// OnCurrentGlobalSpeedInvalidated: Used to validate event firing.
        /// </summary>
        private void OnCurrentGlobalSpeedInvalidated(object sender, EventArgs e)
        {               
           _outString +="CurrentGlobalSpeedInvalidated:  " + ((Clock)sender).CurrentGlobalSpeed + "\n";
           currentGlobalSpeedFired = true;
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
            _outString +="----VerifyAnimation----\n";
            
            bool dpValueCorrect = false;
            
            _outString +="-------------------------------------------\n";
            switch (_animationType)
            {
                case "FE" :
                    double tolerance = 0.01;
                    dpValueCorrect = ( (_scrollBar.Value <= _finalDouble + tolerance) && (_scrollBar.Value >= _finalDouble - tolerance) );
                    _outString +="       GetValue - Exp: " + _finalDouble +  " / Act: " + _scrollBar.Value + "\n";
                    break;
                case "Animatable" :
                    dpValueCorrect = (_lineGeometry.StartPoint == _finalPoint);
                    _outString +="       GetValue - Exp: " + _finalPoint +  " / Act: " + _lineGeometry.StartPoint + "\n";
                    break;
                case "KeyFrame" :
                    dpValueCorrect = (_scrollBar.Orientation == _finalEnum);
                    _outString +="       GetValue - Exp: " + _finalEnum +  " / Act: " + _scrollBar.Orientation + "\n";
                    break;

                default:
                    GlobalLog.LogEvidence("ERROR!! VerifyAnimation: AnimationType was not found \n");
                    _testPassed = false;
                    break;
             }
             
            _outString +="       CurrentStateFired:       " + currentStateFired + "\n";
            _outString +="       CurrentGlobalSpeedFired: " + currentGlobalSpeedFired + "\n";
            
            _testPassed = (currentStateFired && currentGlobalSpeedFired && dpValueCorrect);
            _outString +="       RESULT: " + _testPassed + "\n";
            _outString +="-------------------------------------------\n";
            
            Signal("VerifyResults", (TestResult)(_cumulativeResult ? TestResult.Pass : TestResult.Fail));
        }
        #endregion
    }
}
