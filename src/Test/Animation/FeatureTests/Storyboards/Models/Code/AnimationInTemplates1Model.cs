// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
* ***************  MIL Animation Test *******************************************************
*     Purpose:            This framework tests the integration of Animation and Element Services,
*                         focusing on Triggers in a variety of scenarios involving a ControlTemplate.  The Model is 
*                         stateless and consists of the following test matrix (parameters):
*                             (a) Location of the Trigger (Element Style / Root Style / Element / Root)
*                             (b) Location of the Storyboard (Trigger / Style Resources / Element Resources / Root Resources)
*                             (c) Type of Trigger (Event / Property / Data / MultiData / Multi)
*                             (c) Type of Animation (Framework Element / Animatable / KeyFrame)
*                             (d) References (Static vs. Dynamic)
*                             (e) Location where the DP value is initially set (Element / Element Resources / Root Resources / ControlTemplate Resources)
*                         The sequence of events in this test case is:
*                             (1) the page is rendered containing a Button and a ControlTemplate associated with it.
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
*     Support Files:      AnimationInTemplates1Model.xtc file, which specifies the test cases [must be available at run time]
*
*     NOTE:  some scenarios in PlaceSetterAndTrigger() are not supported by the .xtc file being used.
*     These include scenarios involving placing Triggers or Storyboards inside the ControlTemplate.
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
using System.Windows.Shapes;
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
    /// Verify Animations using Triggers in a variety of scenarios with a ControlTemplate.
    /// </description>
    /// </summary>
    [Test(2, "Storyboard.Models.Triggers", "AnimationInTemplates1Model", SupportFiles=@"FeatureTests\Animation\AnimationInTemplates1Model.xtc")]

    class AnimationInTemplates1Model : WindowModel
    {
        #region Test case members

        private Canvas                          _body                    = null;
        private Button                          _button1                 = null;
        private TextBlock                       _animatedElement         = null;
        private TranslateTransform              _translateTransform      = null;
        private EventTrigger                    _eventTrigger            = null;
        private ControlTemplate                 _controlTemplate         = null;
        private FrameworkElementFactory         _templateContent         = null;
        
        private DoubleAnimationUsingKeyFrames   _animLinearDouble        = null;
        private DoubleAnimation                 _animFontSize            = null;
        private DoubleAnimation                 _animTranslate           = null;
        
        private double                          _initialFontSize         = 1d;
        private double                          _initialX                = -5d;
        private double                          _initialOpacity          = 0.50d;
        private double                          _finalFontSize           = 48d;
        private double                          _finalX                  = 45d;
        private double                          _finalOpacity            = 0d;
        private double                          _currentValue            = 0d;
        
        //Input values from the Model.
        private string                          _triggerLocation         = "";
        private string                          _locationStoryboard      = "";
        private string                          _animationType           = "";
        private string                          _triggerType             = "";
        private string                          _styleReference          = "";
        private string                          _dpLocation              = "";
        
        //Used for setting up the Animation.
        private DependencyProperty              _dp                      = null;
        private BeginStoryboard                 _beginStory              = null;
        private Storyboard                      _story                   = null;
        private ResourceDictionary              _elementDictionary       = null;
        private ResourceDictionary              _rootDictionary          = null;
        private ResourceDictionary              _templateDictionary      = null;
        private Style                           _triggerStyle            = null;
        private Style                           _setterStyle             = null;
        private Style                           _templateStyle           = null;

        public int                              actCurrentState         = 0;
        public int                              actCurrentGlobalSpeed   = 0;
        public int                              expCurrentState         = 2;
        public int                              expCurrentGlobalSpeed   = 2;

        private TimeSpan                        _BEGIN_TIME              = TimeSpan.FromMilliseconds(0);
        private Duration                        _DURATION_TIME           = new Duration(TimeSpan.FromMilliseconds(1000));
        private TimeSpan                        _TIMER_INTERVAL          = TimeSpan.FromMilliseconds(2500);
        private string                          _STYLEKEY                = "StyleKey";
        private string                          _SETTERKEY               = "SetterKey";
        private string                          _STORYKEY                = "StoryKey";

        private DispatcherTimer                 _aTimer                  = null;
        private int                             _dispatcherTickCount     = 0;
        private int                             _testNumber              = 0;
        private bool                            _testPassed              = true;
        private bool                            _cumulativeResult        = true;
        private string                          _outString               = "";

        #endregion


        #region Contructors


        [Variation("AnimationInTemplates1Model.xtc",    1,   50)]
        [Variation("AnimationInTemplates1Model.xtc",  501,  550)]
        [Variation("AnimationInTemplates1Model.xtc",  851,  900)]
        [Variation("AnimationInTemplates1Model.xtc", 1401, 1450)]


        public AnimationInTemplates1Model(string xtcFileName) : this(xtcFileName, -1) { }

        public AnimationInTemplates1Model(string xtcFileName, int testCaseNumber)
            : this(xtcFileName, testCaseNumber, testCaseNumber) { }

        public AnimationInTemplates1Model(string xtcFileName, int startTestCaseNumber, int endTestCaseNumber)
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
                //NOTE:  the Model Action is invoked by the framework -before- OnSetWindowProperties and
                //OnBeginCase fire, so the window creation and content logic is moved here instead.
                //But window creation must occur only once, for all test cases.
                if (_testNumber == 1)
                {
                    SetWindowProperties();
                }

                //For each test case, add elements to the Page and show the Page.
                SetWindowContent();

                _outString += "-------------------------------------------------\n";
                _outString += "BEGIN TEST #" + _testNumber + "\n";
                _outString += "-------------------------------------------------\n";

                _animationType           = inParameters["Animation"];
                _triggerType             = inParameters["TriggerType"];
                _locationStoryboard      = inParameters["StoryboardLocation"];
                _styleReference          = inParameters["Reference"];
                _triggerLocation         = inParameters["TriggerLocation"];
                _dpLocation              = inParameters["DPLocation"];

                _outString += "------Model Input Parameters--------------\n";
                _outString += "- Animation:          " + _animationType       + "\n";
                _outString += "- TriggerType:        " + _triggerType         + "\n";
                _outString += "- StoryboardLocation: " + _locationStoryboard  + "\n";
                _outString += "- Reference:          " + _styleReference      + "\n";
                _outString += "- TriggerLocation:    " + _triggerLocation     + "\n";
                _outString += "- DPLocation:         " + _dpLocation          + "\n";
                _outString += "------------------------------------------\n";

                //For each test case, add elements to the Page and show the Page.
                SetWindowContent();
            
                //Create and start the Animation.
                //SetupAnimation();
            }
            catch(Exception e)
            {
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
            
            Window.Title                = "AnimationInTemplates1 Test";
            Window.Left                 = 10;
            Window.Top                  = 10;
            Window.Height               = 350;
            Window.Width                = 500;
            Window.WindowStyle          = WindowStyle.None;
            Window.ContentRendered     += new EventHandler(OnContentRendered);
        }

        /******************************************************************************
        * Function:          SetWindowContent
        ******************************************************************************/
        /// <summary>
        /// SetWindowContent: add elements to the window, then Show().
        /// </summary>
        /// <returns></returns>
        private void SetWindowContent()
        {
            _outString += "--SetWindowContent--\n";
            
            //Create new Styles which will contain a Trigger and/or a Setter.
            _triggerStyle = new Style();
            _triggerStyle.TargetType = typeof(TextBlock);

            _setterStyle = new Style();
            _setterStyle.TargetType = typeof(TextBlock);

            _templateStyle = new Style();
            _templateStyle.TargetType = typeof(TextBlock);

            //Create dictionaries.
            _elementDictionary   = new ResourceDictionary();
            _rootDictionary      = new ResourceDictionary();
            _templateDictionary  = new ResourceDictionary();

            NameScope.SetNameScope(Window, new NameScope());

            _body  = new Canvas();
            _body.Background    = Brushes.DarkTurquoise;

            //Create a Button, which will contain the ControlTemplate with the to-be-animated TextBlock.
            _button1 = new Button();
            _body.Children.Add(_button1);
            _button1.Name          = "BUTTON1";
            Window.RegisterName(_button1.Name, _button1);

            Canvas.SetTop  (_button1, 75d);
            Canvas.SetLeft (_button1, 75d);

            _translateTransform = new TranslateTransform();
            _translateTransform.X   = _initialX;
            _translateTransform.Y   = _initialX;
            Window.RegisterName("TRANS", _translateTransform); //Provide the Animatable a Name.

            Window.Content = _body;

            AddControlTemplate();
            
            _controlTemplate.VisualTree = _templateContent;
            _button1.Template = _controlTemplate;

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
            
            //Retrieve the to-be-animated object from the ControlTemplate.
            DependencyObject dependencyObject = (DependencyObject)_button1.Template.FindName("textblock1", (FrameworkElement)_button1);
            _animatedElement = (TextBlock)dependencyObject;
            Window.RegisterName(_animatedElement.Name, _animatedElement);

            //Signal that the test case can proceed, now that the page is rendered.
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
            //outString += "----Tick #" + dispatcherTickCount + "----\n";
            
            if (_dispatcherTickCount == 1)
            {
                VerifyAnimation();
            }
            else
            {
                //Delay finishing the test case, to allow time for GC.
                _aTimer.Stop();

                //Signal that the next test case (if any) can start, now that this one is done.
                Signal("TestFinished", (TestResult)(_cumulativeResult ? TestResult.Pass : TestResult.Fail));
            }
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
            //outString += "--OnEndCase--\n";
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
            //outString += "--EndCase--\n";

            if (_aTimer != null) 
            {
                _aTimer.Stop();
            }
            
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

            return _cumulativeResult;
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
           * Function:          AddControlTemplate
           ******************************************************************************/
        /// <summary>
        /// AddControlTemplate: Add a ControlTemplate to the Button.
        /// </summary>
        private void AddControlTemplate()
        {
            _outString += "--AddControlTemplate--\n";

            _controlTemplate = CreateTemplate(ref _templateContent);

            if (_dpLocation == "ResourcesTemplate")
            {
                //Place the to-be-animated DP in a Setter in the ControlTemplate's Resources.
                Setter setter = CreateSetter(_animationType);

                //No Trigger is placed inside the ControlTemplate Style, so a new
                //one is created.
                _templateStyle.Setters.Add(setter);

                _templateDictionary.Add("StyleInTemplate", _templateStyle);

                ((ControlTemplate)_controlTemplate).Resources = _templateDictionary;
            }
        }
        
        /******************************************************************************
           * Function:          CreateTemplate
           ******************************************************************************/
        /// <summary>
        /// CreateTemplate: Create a ControlTemplate.
        /// </summary>
        private ControlTemplate CreateTemplate(ref FrameworkElementFactory canvas)
        {
            _outString += "--CreateTemplate--\n";

            canvas = new FrameworkElementFactory(typeof(Canvas));
            canvas.SetValue(Canvas.BackgroundProperty, Brushes.LightGreen);
            canvas.SetValue(Canvas.HeightProperty, 70d);
            canvas.SetValue(Canvas.WidthProperty, 120d);

            FrameworkElementFactory textBlock = new FrameworkElementFactory(typeof(TextBlock), "textblock1");
            textBlock.SetValue(TextBlock.BackgroundProperty, Brushes.SlateBlue);
            textBlock.SetValue(TextBlock.FontFamilyProperty, new FontFamily("Trebuchet"));
            textBlock.SetValue(TextBlock.TextProperty, "Avalon!");
            textBlock.SetValue(TextBlock.FontSizeProperty, 32d);
            textBlock.SetValue(TextBlock.NameProperty, "textblock1");
            textBlock.SetValue(TextBlock.HeightProperty, 50d);
            textBlock.SetValue(TextBlock.WidthProperty, 100d);

            canvas.AppendChild(textBlock);

            ControlTemplate template = new ControlTemplate(typeof(Button));
            
            template.RegisterName("textblock1", textBlock);
            
            return template;
        }
        
        /******************************************************************************
        * Function:          SetupAnimation
        ******************************************************************************/
        /// <summary>
        /// SetupAnimation: create the Storyboard and Animation and begin.
        /// </summary>
        /// <returns></returns>
        private void SetupAnimation()
        {
            _outString += "--SetupAnimation--\n";

            //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //(1) Create an Animation [and associated PropertyPath], and add it to a new Storyboard.
            //    A new Storyboard with Animation and an associated DependencyProperty are returned by ref.
            SetAnimation();
            
            //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //(2) Create a BeginStoryboard.
            //    A BeginStoryboard object is created.
            _beginStory = AnimationUtilities.CreateBeginStoryboard(Window, "beginStory");
            
            //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //(3) Create a Trigger: Event / Property / Data / MultiData / Multi.
            //    A Style is returned containing a Trigger with a BeginStoryboard pointing to a Storyboard.
            CreateTrigger();
            
            //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //(4) Place the Storyboard, either on an element or in an element's Resources.
            //    The Storyboard is then assigned to the BeginStoryboard's Storyboard property.
            SetStoryboardLocation();

            //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //(5) Assign the animated property using a Setter.
            //    It can be (a) directly on the animated element, (b) withing the element's Style,
            //    (c) within the element's Resources, or (d) within the root Style.
            PlaceSetterAndTrigger();


            //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //START THE ANIMATION.
            //Use UIAutomation to start the Animation by moving the mouse over the triggering element.
            _outString += "----Start the Animation via Mouse Click--\n";
            _button1.Focus();
            UserInput.MouseLeftClickCenter(_button1);
 
            //Start a separate Timer to control the timing of verification.
            _outString += "----Start DispatcherTimer----\n";
            _dispatcherTickCount = 0;  //Needs to be reset for every case.
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = _TIMER_INTERVAL;
            _aTimer.Start();

            //Must wait for UIAutomation to trigger the animation and results to be verified,
            //before continuing the Model logic, which will end the test case.
            WaitForSignal("TestFinished");
        }
        
        /******************************************************************************
        * Function:          Reset
        ******************************************************************************/
        /// <summary>
        /// Reset: resets variables.
        /// </summary>
        /// <returns></returns>
        private void Reset()
        {
            actCurrentState             = 0;
            actCurrentGlobalSpeed       = 0;
            if (_story != null)
            {
                _story.Stop(_animatedElement);
                _story = null;
            }
            if (_beginStory != null)
            {
                Window.UnregisterName(_beginStory.Name);
            }
            if (_animatedElement != null)
            {
                Window.UnregisterName(_animatedElement.Name);
            }
            
            _dp                  = null;
            _beginStory          = null;
            _rootDictionary      = null;
            _elementDictionary   = null;
            _triggerStyle        = null;
            _setterStyle         = null;
        }
        
        /******************************************************************************
        * Function:          PlaceSetterAndTrigger
        ******************************************************************************/
        /// <summary>
        /// PlaceSetterAndTrigger: (a) assigns the to-be-animated dp via a Setter (except when set
        /// directly on the element), (b) places the Setter in the appropriate Style, and (c)
        /// places the Trigger in the appropriate Style.  [Sometimes both Setter and Trigger end
        /// up inside the same Style.
        /// </summary>
        /// <returns></returns>
        private void PlaceSetterAndTrigger()
        {
            _outString += "--PlaceSetterAndTrigger--\n";
            
            Setter dpSetter;
            
            switch (_dpLocation)
            {
                case "Element" :
                    //----SCENARIO 1:
                    //Assign the to-be-animated DP directly on the DO: no Setter used in this case.
                    switch (_animationType)
                    {
                        case "FE" :
                            ((TextBlock)_animatedElement).FontSize     = 1d;
                            break;

                        case "Animatable" :
                            _animatedElement.RenderTransform = _translateTransform;
                            break;

                        case "KeyFrame" :
                            _animatedElement.Opacity     = 0d;
                            break;

                        default:
                            _outString += "ERROR!! PlaceSetterAndTrigger: AnimationType was not found \n";
                            _testPassed = false;
                            break;
                    }
                    
                    if (_triggerLocation == "StyleElement" || _triggerLocation == "Element")
                    {
                        //----SCENARIO 1:
                        //No Setter is used.
                        //The Trigger is placed inside the animated Element's Style ("triggerStyle").
                        //The triggerStyle is based on the setterStyle, so that both will apply.

                        _elementDictionary.Add(_STYLEKEY, _triggerStyle);
                        ((FrameworkElement)_animatedElement).Resources = _elementDictionary;
                        
                        //Connect the animated element to the triggerStyle via DynamicResource.
                        SetReference(_STYLEKEY);
                    }
                    else if (_triggerLocation == "StyleRoot" || _triggerLocation == "Root")
                    {
                        //----SCENARIO 2:
                        //No Setter is used.
                        //The Trigger is placed inside the root Element's Style ("triggerStyle").

                        _rootDictionary.Add(_STYLEKEY, _triggerStyle);
                        ((FrameworkElement)Window).Resources = _rootDictionary;
                        
                        //Connect the animated element to the triggerStyle via DynamicResource.
                        SetReference(_STYLEKEY);
                    }
                    else if (_triggerLocation == "ControlTemplate")
                    {
                        //----SCENARIO 3:
                        //No Setter is used.
                        //The Trigger is placed inside the ControlTemplate's Element's Style ("triggerStyle").

                        _templateDictionary.Add(_STYLEKEY, _triggerStyle);
                        ((ControlTemplate)_controlTemplate).Resources = _templateDictionary;
                        
                        //Connect the animated element to the triggerStyle via DynamicResource.
                        SetReference(_STYLEKEY);
                    }
                    else
                    {
                        _outString += "ERROR!! PlaceSetterAndTrigger 1: Trigger Location was not found \n";
                        _testPassed = false;
                    }
                    break;
                
                case "ResourcesElement" :
                    dpSetter = CreateSetter(_animationType);
                    
                    if (_triggerLocation == "StyleElement" || _triggerLocation == "Element")
                    {
                        //----SCENARIO 4:
                        //Both the dp Setter and the Trigger are placed inside the animated
                        //Element's Style ("triggerStyle").
                        _triggerStyle.Setters.Add(dpSetter);
                        
                        _elementDictionary.Add(_STYLEKEY, _triggerStyle);
                        ((FrameworkElement)_animatedElement).Resources = _elementDictionary;
                        
                        //Connect the animated element to the triggerStyle via DynamicResource.
                        SetReference(_STYLEKEY);
                    }
                    else if (_triggerLocation == "StyleRoot" || _triggerLocation == "Root")
                    {
                        //----SCENARIO 5:
                        //Two styles are involved, one for the dp Setter, one for the Trigger.
                        //The dp Setter is placed inside the animated element's Style "setterStyle")
                        //but the Trigger is placed inside a Style in the root element's Resource
                        //("triggerStyle").  The setterStyle is based on the triggerStyle, so that
                        //both will apply.
                        _setterStyle.Setters.Add(dpSetter);
                        _triggerStyle.BasedOn = _setterStyle;
                        
                        _elementDictionary.Add(_SETTERKEY, _setterStyle);
                        ((FrameworkElement)_animatedElement).Resources = _elementDictionary;
                        
                        _rootDictionary.Add(_STYLEKEY, _triggerStyle);
                        ((FrameworkElement)Window).Resources = _rootDictionary;

                        //Connect the animated element to the setterStyle via DynamicResource.
                        SetReference(_STYLEKEY);
                    }
                    else if (_triggerLocation == "ControlTemplate")
                    {
                        //----SCENARIO 6:
                        //Two styles are involved, one for the dp Setter, one for the Trigger.
                        //The dp Setter is placed inside the animated element's Style "setterStyle")
                        //but the Trigger is placed inside a Style in the ControlTemplate's Resource
                        //("triggerStyle").  The setterStyle is based on the triggerStyle, so that
                        //both will apply.
                        _setterStyle.Setters.Add(dpSetter);
                        _triggerStyle.BasedOn = _setterStyle;
                        
                        _elementDictionary.Add(_SETTERKEY, _setterStyle);
                        ((FrameworkElement)_animatedElement).Resources = _elementDictionary;
                        
                        _templateDictionary.Add(_STYLEKEY, _triggerStyle);
                        ((ControlTemplate)_controlTemplate).Resources = _templateDictionary;

                        //Connect the animated element to the setterStyle via DynamicResource.
                        SetReference(_STYLEKEY);
                    }
                    else
                    {
                        _outString += "ERROR!! PlaceSetterAndTrigger 2: Trigger Location was not found \n";
                        _testPassed = false;
                    }
                    break;
                
                case "ResourcesRoot" :
                    dpSetter = CreateSetter(_animationType);

                    if (_triggerLocation == "StyleElement" || _triggerLocation == "Element")
                    {
                        //----SCENARIO 7:
                        //Two styles are involved, one for the dp Setter, one for the Trigger.
                        //The dp Setter is placed inside the root element's Style "setterStyle")
                        //but the Trigger is placed inside a Style in the animated element's Resource
                        //("triggerStyle").  The triggerStyle is based on the setterStyle, so that
                        //both will apply.
                        _setterStyle.Setters.Add(dpSetter);
                        _triggerStyle.BasedOn = _setterStyle;
                        
                        _rootDictionary.Add(_SETTERKEY, _setterStyle);
                        ((FrameworkElement)Window).Resources = _rootDictionary;
                        
                        _elementDictionary.Add(_STYLEKEY, _triggerStyle);
                        ((FrameworkElement)_animatedElement).Resources = _elementDictionary;
                        
                        //Connect the animated element to the triggerStyle via DynamicResource.
                        SetReference(_STYLEKEY);
                    }
                    else if (_triggerLocation == "StyleRoot" || _triggerLocation == "Root")
                    {
                        //----SCENARIO 8:
                        //Both the dp Setter and the Trigger are placed inside the root
                        //Element's Style ("triggerStyle").
                        _triggerStyle.Setters.Add(dpSetter);
                        
                        _rootDictionary.Add(_STYLEKEY, _triggerStyle);
                        ((FrameworkElement)Window).Resources = _rootDictionary;
                        
                        //Connect the animated element to the triggerStyle via DynamicResource.
                        //animatedElement.SetResourceReference(FrameworkElement.StyleProperty, STYLEKEY);
                        SetReference(_STYLEKEY);
                    }
                    else if (_triggerLocation == "ControlTemplate")
                    {
                        //----SCENARIO 9:
                        //Two styles are involved, one for the dp Setter, one for the Trigger.
                        //The dp Setter is placed inside the root element's Style "setterStyle")
                        //but the Trigger is placed inside a Style in the ControlTemplate's Resource
                        //("triggerStyle").  The triggerStyle is based on the setterStyle, so that
                        //both will apply.
                        _setterStyle.Setters.Add(dpSetter);
                        _triggerStyle.BasedOn = _setterStyle;
                        
                        _rootDictionary.Add(_SETTERKEY, _setterStyle);
                        ((FrameworkElement)Window).Resources = _rootDictionary;
                        
                        _templateDictionary.Add(_STYLEKEY, _triggerStyle);
                        ((ControlTemplate)_controlTemplate).Resources = _templateDictionary;
                        
                        //Connect the animated element to the triggerStyle via DynamicResource.
                        SetReference(_STYLEKEY);
                    }
                    else
                    {
                        _outString += "ERROR!! PlaceSetterAndTrigger 3: Trigger Location was not found \n";
                        _testPassed = false;
                    }
                    break;

                case "ResourcesTemplate" :
                    //The dp Setter has already been added to the ControlTemplate, before the 
                    //page rendered.
                    
                    if (_triggerLocation == "StyleElement" || _triggerLocation == "Element")
                    {
                        //----SCENARIO 10:
                        //The dp Setter has already been placed in the ControlTemplate's Style.
                        //The Trigger is placed inside the animated Element's Style ("triggerStyle").
                        //The triggerStyle is based on the setterStyle, so that both will apply.
                        _triggerStyle.BasedOn = _templateStyle;

                        _elementDictionary.Add(_STYLEKEY, _triggerStyle);
                        ((FrameworkElement)_animatedElement).Resources = _elementDictionary;
                        
                        //Connect the animated element to the triggerStyle via DynamicResource.
                        SetReference(_STYLEKEY);
                    }
                    else if (_triggerLocation == "StyleRoot" || _triggerLocation == "Root")
                    {
                        //----SCENARIO 11:
                        //The dp Setter has already been placed in the ControlTemplate's Style.
                        //The Trigger is placed inside the root Element's Style ("triggerStyle").
                        _triggerStyle.BasedOn = _templateStyle;

                        _rootDictionary.Add(_STYLEKEY, _triggerStyle);
                        ((FrameworkElement)Window).Resources = _rootDictionary;
                        
                        //Connect the animated element to the triggerStyle via DynamicResource.
                        SetReference(_STYLEKEY);
                    }
                    else if (_triggerLocation == "ControlTemplate")
                    {
                        //----SCENARIO 12:
                        //The dp Setter has already been placed in the ControlTemplate's Style.
                        //The Trigger is placed inside the root Element's Style ("triggerStyle").
                        _triggerStyle.BasedOn = _templateStyle;

                        _rootDictionary.Add(_STYLEKEY, _triggerStyle);
                        ((FrameworkElement)Window).Resources = _rootDictionary;
                        
                        //Connect the animated element to the triggerStyle via DynamicResource.
                        SetReference(_STYLEKEY);
                    }
                    else
                    {
                        _outString += "ERROR!! PlaceSetterAndTrigger 4: Trigger Location was not found \n";
                        _testPassed = false;
                    }
                    break;

                default:
                    _outString += "ERROR!! PlaceSetterAndTrigger 1: Animation Type was not found \n";
                    _testPassed = false;
                    break;
            }
        }

        /******************************************************************************
        * Function:          CreateSetter
        ******************************************************************************/
        /// <summary>
        /// CreateSetter: creates a new Style containing a Setter for the to-be-animated
        /// property.
        /// </summary>
        /// <returns></returns>
        private Setter CreateSetter(string animationType)
        {
            _outString += "--CreateSetter--\n";
            
            Setter setter = new Setter();
            
            switch (animationType)
            {
                case "FE" :
                    setter.Property    = TextBlock.FontSizeProperty;
                    setter.Value       = (double)1;
                    break;

                case "Animatable" :
                    setter.Property    = TextBlock.RenderTransformProperty;
                    setter.Value       = (TranslateTransform)_translateTransform;
                    break;

                case "KeyFrame" :
                    setter.Property    = TextBlock.OpacityProperty;
                    setter.Value       = (double)0;
                    break;

                default:
                    _outString += "ERROR!! CreateSetter: Animation Type was not found \n";
                    _testPassed = false;
                    break;
            }
            return setter;
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
                    _animFontSize = CreateFontSizeAnimation(_initialFontSize, _finalFontSize);
                    
                    _dp = TextBlock.FontSizeProperty;
                    pp = new PropertyPath( "(0)", _dp );
                    
                    AttachEvents(_animFontSize);

                    Storyboard.SetTargetProperty(_animFontSize, pp);
                    _story.Children.Add(_animFontSize);
                    break;
                
                case "Animatable" :
                    _dp = TranslateTransform.XProperty;
                    _animTranslate = CreateTranslateAnimation(_initialX, _finalX);
                    
                    pp = new PropertyPath("(0).(1)", new DependencyProperty[] { TextBlock.RenderTransformProperty, _dp });
                    
                    AttachEvents(_animTranslate);

                    Storyboard.SetTargetProperty(_animTranslate, pp);
                    _story.Children.Add(_animTranslate);
                    break;
                
                case "KeyFrame" :
                    _dp = UIElement.OpacityProperty;
                    pp = new PropertyPath( "(0)", _dp );
                    
                    _animLinearDouble = CreateDoubleKFAnimation();
                    
                    AttachEvents(_animLinearDouble);
                    
                    Storyboard.SetTargetProperty(_animLinearDouble, pp);
                    _story.Children.Add(_animLinearDouble);
                    break;

                default:
                    _outString += "ERROR!! SetAnimation: Animation Type was not found \n";
                    _testPassed = false;
                    break;
            }
        }

        /******************************************************************************
        * Function:          CreateFontSizeAnimation
        ******************************************************************************/
        /// <summary>
        /// CreateFontSizeAnimation: Creates a DoubleAnimation, for animating Value.
        /// </summary>
        /// <returns>A DoubleAnimation</returns>
        private DoubleAnimation CreateFontSizeAnimation(double fromValue, double toValue)
        {
            _outString += "--CreateFontSizeAnimation--\n";
            
            DoubleAnimation anim = new DoubleAnimation();
            anim.From            = fromValue;
            anim.To              = toValue;
            anim.BeginTime       = _BEGIN_TIME;
            anim.Duration        = _DURATION_TIME;
            
            return anim;
        }
        
        /******************************************************************************
        * Function:          CreateTranslateAnimation
        ******************************************************************************/
        /// <summary>
        /// CreateTranslateAnimation: Creates a DoubleAnimation, for animating X and Y.
        /// </summary>
        /// <returns>A DoubleAnimation</returns>
        private DoubleAnimation CreateTranslateAnimation(double fromValue, double toValue)
        {
            _outString += "--CreateTranslateAnimation--\n";
            
            DoubleAnimation anim = new DoubleAnimation();
            anim.From            = fromValue;
            anim.To              = toValue;
            anim.BeginTime       = _BEGIN_TIME;
            anim.Duration        = _DURATION_TIME;
            
            return anim;
        }
        
        /******************************************************************************
        * Function:          CreateDoubleKFAnimation
        ******************************************************************************/
        /// <summary>
        /// CreateDoubleKFAnimation: Creates a DoubleAnimationUsingKeyFrames.
        /// </summary>
        /// <returns>A DoubleAnimationUsingKeyFrames</returns>
        private DoubleAnimationUsingKeyFrames CreateDoubleKFAnimation()
        {
            _outString += "--CreateDoubleKFAnimation--\n";

            DoubleAnimationUsingKeyFrames anim = new DoubleAnimationUsingKeyFrames();
            DoubleKeyFrameCollection KFC = new DoubleKeyFrameCollection();
            KFC.Add(new LinearDoubleKeyFrame(_initialOpacity, KeyTime.FromPercent(0.25d)));
            KFC.Add(new LinearDoubleKeyFrame( ((_finalOpacity-_initialOpacity)/2), KeyTime.FromPercent(0.50d)));
            KFC.Add(new LinearDoubleKeyFrame(_finalOpacity,   KeyTime.FromPercent(0.99d)));
            anim.KeyFrames = KFC;
            anim.BeginTime              = _BEGIN_TIME;
            anim.Duration               = _DURATION_TIME;
            
            return anim;
        }

        /******************************************************************************
        * Function:          CreateTrigger
        ******************************************************************************/
        /// <summary>
        /// CreateTrigger: creates the appropriate Trigger and adds the BeginStoryboard to it. 
        /// The new Trigger is then added to a Style, which will later be associated with
        /// an element on the page.
        /// </summary>
        /// <returns></returns>
        private void CreateTrigger()
        {
            _outString += "--CreateTrigger--\n";

            switch (_triggerType)
            {
                case "Event" :
                    _eventTrigger = AnimationUtilities.CreateEventTrigger(System.Windows.Input.Mouse.PreviewMouseDownEvent, _beginStory);
                    _triggerStyle.Triggers.Add(_eventTrigger);
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
        /// SetStoryboardLocation: Attaches a Storyboard it to the specified object.
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
                    _elementDictionary.Add(_STORYKEY, _story);
                    ((FrameworkElement)_animatedElement).Resources = _elementDictionary;
                    
                    Storyboard storyInElement = (Storyboard)((FrameworkElement)_animatedElement).FindResource(_STORYKEY);
                    _beginStory.Storyboard = storyInElement;
                    break;
                
                case "InRootResources" :
                    _rootDictionary.Add(_STORYKEY, _story);
                    ((FrameworkElement)Window).Resources = _rootDictionary;
                    
                    Storyboard storyInRoot = (Storyboard)Window.FindResource(_STORYKEY);
                    _beginStory.Storyboard = storyInRoot;
                    break;
                
                case "InStyleResources" :
                    _triggerStyle.Resources.BeginInit();
                    _triggerStyle.Resources.Add(_STORYKEY, _story);
                    _triggerStyle.Resources.EndInit();
                    
                    object o = _triggerStyle.Resources[_STORYKEY];
                    Storyboard storyInStyle = (Storyboard)o;
                    _beginStory.Storyboard = storyInStyle;
                    break;
                
                case "InTemplateResources" :
                    _templateDictionary.Add(_STORYKEY, _story);
                    ((ControlTemplate)_controlTemplate).Resources = _templateDictionary;
                    
                    Storyboard storyInTemplate = (Storyboard)_animatedElement.FindResource(_STORYKEY);
                    _beginStory.Storyboard = storyInTemplate;
                    break;

                default:
                    _outString += "ERROR!! SetStoryboardLocation: Storyboard Location was not found \n";
                    _testPassed = false;
                    break;
            }
        }
        
        /******************************************************************************
        * Function:          SetReference
        ******************************************************************************/
        /// <summary>
        /// SetReference: Create a Dynamic or Static Reference to set the Style property on the
        /// to-be-animated element.
        /// </summary>
        /// <returns></returns>
        private void SetReference(string keyName)
        {
            _outString += "--SetReference--\n";
           
            switch (_styleReference)
            {
                case "Dynamic" :
                    _animatedElement.SetResourceReference(FrameworkElement.StyleProperty, keyName);
                    break;

                case "Static" :
                    switch (_triggerLocation)
                    {
                        case "StyleElement" :
                            _animatedElement.Style = (Style)_animatedElement.Resources[keyName];
                            break;

                        case "StyleRoot" :
                            _animatedElement.Style = (Style)Window.Resources[keyName];
                            break;

                        case "Element" :
                            _animatedElement.Style = (Style)_animatedElement.Resources[keyName];
                            break;

                        case "Root" :
                            _animatedElement.Style = (Style)Window.Resources[keyName];
                            break;

                        case "ControlTemplate" :
                            _animatedElement.Style = (Style)_controlTemplate.Resources[keyName];
                            break;

                        default:
                            _outString += "ERROR!! SetReference: Trigger Location was not found \n";
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
           * Function:          OnCurrentStateInvalidated
           ******************************************************************************/
        /// <summary>
        /// OnCurrentStateInvalidated: Used to validate event firing.
        /// </summary>
        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {               
            _outString += "CurrentStateInvalidated:  " + ((Clock)sender).CurrentState + "\n";
            actCurrentState++;
            
            AnimationClock AC = (AnimationClock)((Clock)sender);
            _currentValue = (double)AC.GetCurrentValue(0d,0d);
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
            _outString += "--VerifyAnimation--\n";
            
            bool dpValueCorrect = false;
            
            _outString += "----------------RESULTS------------------\n";
            switch (_animationType)
            {
                case "FE" :
                    dpValueCorrect = ((double)_animatedElement.GetValue(_dp) == _finalFontSize);
                    _outString += "      GetValue:      Act: " + _animatedElement.FontSize + "  Exp: " + _finalFontSize + "\n";
                    break;
                case "Animatable" :
                    dpValueCorrect = (_currentValue == _finalX);  //Animated value from AnimationClock at fill time.
                    _outString += "      GetCurrentValue X: Act: " + _currentValue + "  Exp: " + _finalX + "\n";
                    break;
                case "KeyFrame" :
                    double actualOpacity = (double)_animatedElement.GetValue(_dp);
                    dpValueCorrect = (actualOpacity == _finalOpacity);
                    _outString += "      GetValue:      Act: " + actualOpacity + " Exp: " + _finalOpacity + "\n";
                    break;

                default:
                    _outString += "ERROR!! VerifyAnimation: AnimationType was not found \n";
                    _testPassed = false;
                    break;
             }
            _outString += "      dpValueCorrect: " + dpValueCorrect + "\n";
             
            _outString += "      CurrentStateFired:       Act: " + actCurrentState +        " Exp: >= " + expCurrentState + "\n";
            _outString += "      CurrentGlobalSpeedFired: Act: " + actCurrentGlobalSpeed +  " Exp: >= " + expCurrentGlobalSpeed + "\n";
            
            _testPassed = (dpValueCorrect && (actCurrentState >= expCurrentState) && (actCurrentGlobalSpeed >= expCurrentGlobalSpeed) && _testPassed);
            _outString += "      RESULT: " + _testPassed + "\n";
            _outString += "-----------------------------------------\n";
        }
        #endregion
    }
}
