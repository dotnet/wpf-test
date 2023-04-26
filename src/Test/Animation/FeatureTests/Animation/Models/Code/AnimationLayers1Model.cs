// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
* ***************  MIL Animation Test *******************************************************
*   Purpose: 
*   Verify a sequence of animations on the same DependencyProperty: RotateTransform.AngleProperty
*   on a TextBox's LayoutTransform. The animations fall into five categories, in random order,
*   determined by the Model's state-based actions:
*       (1) EventTrigger: located inside the animated element
*       (2) EventTrigger: starting in code a Storyboard defined in Markup Resources
*       (3) EventTrigger: Storyboard in root element's Style Resources section
*       (4) Property Trigger
*       (5) DataTrigger, within animated element's ControlTemplate
*   The initial animation in the sequence is always performed in one of the markup files
*   loaded when the test case begins.  All animations have a BeginTime of 0:0:0, a Duration of
*   0:0:2, and a By value of 60d.
*
*   NOTE:  this test framework relies on loose .xaml, rather than compiled .xaml files.
*
*   Pass Conditions:    
*   How verified:       
*   Framework:          A CLR executable is created.
*   Area:               Animation/Timing

*   Dependencies:       TestRuntime.dll, AnimationFramework.dll
*   Support Files:      AnimationLayers1.xtc file, which specifies the test cases [must be available at run time]

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
using System.Windows.Input;
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
    [Test(2, "Storyboard.Models.Triggers", "AnimationLayers1Model", SupportFiles=@"FeatureTests\Animation\AnimationLayers1Model.xtc,FeatureTests\Animation\LO1.xaml,FeatureTests\Animation\LO2.xaml,FeatureTests\Animation\LO3.xaml,FeatureTests\Animation\LO4.xaml,FeatureTests\Animation\LO5.xaml")]

    class AnimationLayers1Model : WindowModel
    {
        #region Test case members

        private NavigationWindow            _navWin                  = null;
        private TextBox                     _animElement             = null;
        private Button                      _focusButton             = null;
        private RotateTransform             _RT                      = null;
        private Storyboard                  _resourceStoryboard      = null;
        private double                      _byValue                 = 60d;
        private Style                       _currentStyle            = null;
        private HandoffBehavior             _currentHandoff          = HandoffBehavior.SnapshotAndReplace;
        private double                      _currentExpAngle         = 0d;
        private string                      _currentAction           = "";
        private int                         _animationCount          = 0;
        private int                         _actEventFired           = 0;
        private int                         _expEventFired           = 0;
        private int                         _animationsStarted       = 0;
        private int                         _animationsFinished      = 0;
        private bool                        _currentComposing        = false;
        private string                      _firstLayer              = "";
        private string                      _secondLayer             = "";
        private Duration                    _DURATION_TIME           = new Duration(TimeSpan.FromMilliseconds(2000));
        private TimeSpan                    _TIMER_INTERVAL          = TimeSpan.FromMilliseconds(500);

        private DispatcherTimer             _aTimer                  = null;
        private int                         _dispatcherTickCount     = 0;
        private int                         _testNumber              = 0;
        private bool                        _testPassed              = true;
        private bool                        _cumulativeResult        = true;
        private string                      _outString               = "";

        #endregion


        #region Contructors

        [Variation("AnimationLayers1Model.xtc",  1, Priority=1)]
        [Variation("AnimationLayers1Model.xtc",  2)]
        [Variation("AnimationLayers1Model.xtc",  3)]
        [Variation("AnimationLayers1Model.xtc",  4)]
        [Variation("AnimationLayers1Model.xtc",  5)]
        [Variation("AnimationLayers1Model.xtc",  6)]
        [Variation("AnimationLayers1Model.xtc",  7)]
        [Variation("AnimationLayers1Model.xtc",  8)]
        [Variation("AnimationLayers1Model.xtc",  9)]
        [Variation("AnimationLayers1Model.xtc", 10)]
        [Variation("AnimationLayers1Model.xtc", 11)]
        [Variation("AnimationLayers1Model.xtc", 12)]
        [Variation("AnimationLayers1Model.xtc", 13)]
        [Variation("AnimationLayers1Model.xtc", 14)]
        [Variation("AnimationLayers1Model.xtc", 15)]
        [Variation("AnimationLayers1Model.xtc", 16)]
        [Variation("AnimationLayers1Model.xtc", 17)]
        [Variation("AnimationLayers1Model.xtc", 18)]
        [Variation("AnimationLayers1Model.xtc", 19)]
        [Variation("AnimationLayers1Model.xtc", 20)]
        [Variation("AnimationLayers1Model.xtc", 21)]
        [Variation("AnimationLayers1Model.xtc", 22)]
        [Variation("AnimationLayers1Model.xtc", 23)]
        [Variation("AnimationLayers1Model.xtc", 24)]
        [Variation("AnimationLayers1Model.xtc", 25)]

        public AnimationLayers1Model(string xtcFileName) : this(xtcFileName, -1) { }

        public AnimationLayers1Model(string xtcFileName, int testCaseNumber)
            : this(xtcFileName, testCaseNumber, testCaseNumber) { }

        public AnimationLayers1Model(string xtcFileName, int startTestCaseNumber, int endTestCaseNumber)
            : base(xtcFileName, startTestCaseNumber, endTestCaseNumber)
        {
            OnBeginCase += new StateEventHandler(OnBeginCase_Handler);
            OnEndCase += new StateEventHandler(OnEndCase_Handler);

            //Add Action Handlers
            AddAction("apply_animation1",   new ActionHandler(apply_animation1));
            AddAction("apply_animation2",   new ActionHandler(apply_animation2));
            AddAction("apply_animation3",   new ActionHandler(apply_animation3));
            AddAction("apply_animation4",   new ActionHandler(apply_animation4));
            AddAction("apply_animation5",   new ActionHandler(apply_animation5));
        }

        #endregion


        #region Model setup and clean-up events

        /******************************************************************************
        * Function:          OnBeginCase_Handler
        ******************************************************************************/
        /// <summary>
        /// OnBeginCase_Handler: Fires when each test case begins.
        /// </summary>
        /// <returns></returns>
        void OnBeginCase_Handler(object sender, StateEventArgs e)
        {
            //outString += " OnBeginCase\n";

            _testNumber++;
            _outString +="-------------------------------------------------\n";
            _outString +="-------------------------------------------------\n";
            _outString +="BEGIN TEST #" + _testNumber + "\n";
            _outString +="-------------------------------------------------\n";
            
            //Initialize for each test case.
            _testPassed          = true;
            _currentStyle        = null;
            _currentHandoff      = HandoffBehavior.SnapshotAndReplace;
            _currentExpAngle     = 0d;
            _currentAction       = "";
            _resourceStoryboard  = null;
            _animationCount      = 0;
            _actEventFired       = 0;
            _expEventFired       = 0;
            _animationsStarted   = 0;
            _animationsFinished  = 0;
            _currentComposing    = false;
            _firstLayer          = "";
            _secondLayer         = "";
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
            //outString += "-OnEndCase\n";
            
            EndCase(e.State);
        }

        /******************************************************************************
        * Function:          EndCase
        ******************************************************************************/
        /// <summary>
        /// OnEndCase: Overrides the Model's EndCase, to verify the test case result.
        /// </summary>
        /// <returns>True</returns>
        public override bool EndCase(State endCase)
        {
            //outString += "-EndCase\n";

            //Wait for the last Animation to finish.
            WaitForSignal("FinishTest");

            _aTimer.Stop();

            VerifyAnimation();
            
            if (_testPassed)
            {
                _outString +="END TEST #" + _testNumber + "                       RESULT: PASSED\n";
            }
            else
            {
                _outString +="END TEST #" + _testNumber + "                       RESULT: FAILED\n";
            }
            _outString += "-------------------------------------------------\n";
            
            _cumulativeResult = (_testPassed && _cumulativeResult);

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

        #endregion


        #region Action steps

        /// <summary>
        /// Handler for apply_animation1
        /// [SCENARIO: EventTrigger: Storyboard in EventTrigger on the animated element]
        /// </summary>
        /// 
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// 
        /// <returns> True </returns>
        /// 
        private bool apply_animation1( State endState, State inParameters, State outParameters )
        {
            _currentAction = "1";
            _animationCount++;

            if (_animationCount == 1)
            {
                _outString += "ACTION #" + _animationCount + ": EventTrigger - SnapshotAndReplace\n";
                _firstLayer = "Local";
                _currentHandoff = HandoffBehavior.SnapshotAndReplace; //HandoffBehavior set in Markup.
                LoadMarkup("LO1");
            }
            else
            {
                _outString += "ACTION #" + _animationCount + ": EventTrigger - " + _currentHandoff + "\n";
                _secondLayer = "Local";
                _currentHandoff = GetHandoff(inParameters["Handoff"]);
                _currentComposing = GetComposing();
                CreateEventTrigger(_currentHandoff);
                _expEventFired++;
                _animElement.Focus();
                UserInput.MouseLeftClickCenter(_animElement); //Trigger the Animation.
            }
            
            //Wait before continuing to invoke the next Action.
            WaitForSignal("ContinueToNextAction");

            return true;
        }

        /// <summary>
        /// Handler for apply_animation2
        /// [SCENARIO: EventTrigger: Storyboard in root element Resources section]
        /// </summary>
        /// 
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        private bool apply_animation2( State endState, State inParameters, State outParameters )
        {
            //NOTE: In this case, HandoffBehavior is always HandoffBehavior.SnapshotAndReplace, 
            //because it is specified that way in Markup.
            _currentHandoff = HandoffBehavior.SnapshotAndReplace;

            _currentAction = "2";
            _animationCount++;

            _outString += "ACTION #" + _animationCount + ": FindResource - " + _currentHandoff + "\n";
            
            if (_animationCount == 1)
            {
                _firstLayer = "Local";
                LoadMarkup("LO2");
            }
            else
            {
                _secondLayer = "Local";
                _currentComposing = GetComposing();
                _expEventFired++;
                //No EventTrigger or BeginStoryboard is used in thise case.  Instead, the Storyboard,
                //which is located in the Markup's Resources, is begun directly in code.  It is the
                //equivalent of EventTriggers, however, with respect to Layers.
                StartStoryboardInResource( _currentHandoff );               //Trigger the Animation.
            }
            
            //Wait before continuing to invoke the next Action.
            WaitForSignal("ContinueToNextAction");

            return true;
        }

        /// <summary>
        /// Handler for apply_animation3
        /// [SCENARIO: EventTrigger: in root element Style in its Resources section]
        /// </summary>
        /// 
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        private bool apply_animation3( State endState, State inParameters, State outParameters )
        {
            _currentAction = "3";
            _animationCount++;

            if (_animationCount == 1)
            {
                _outString += "ACTION #" + _animationCount + ": EventTrigger/Style - SnapshotAndReplace\n";
                _firstLayer = "Local";
                _currentHandoff = HandoffBehavior.SnapshotAndReplace; //HandoffBehavior set in Markup.
                LoadMarkup("LO3");
            }
            else
            {
                _outString += "ACTION #" + _animationCount + ": EventTrigger/Style - " + _currentHandoff + "\n";
                _secondLayer = "Local";
                _currentHandoff = GetHandoff(inParameters["Handoff"]);
                _currentComposing = GetComposing();
                CreateEventTriggerInStyle( _currentHandoff );
                _expEventFired++;
                //UserInput.MouseLeftClickCenter(animElement);  //Click.
                //KeyDown.
                Keyboard.Focus(_animElement);
                _animElement.Focus();
                UserInput.KeyPress("A", true);               //Trigger the Animation.
                UserInput.KeyPress("A", false);
            }
            
            //Wait before continuing to invoke the next Action.
            WaitForSignal("ContinueToNextAction");

            return true;
        }

        /// <summary>
        /// Handler for apply_animation4
        /// [SCENARIO: Property Trigger: Storyboard in Property Trigger (Opacity property)]
        /// </summary>
        /// 
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        private bool apply_animation4( State endState, State inParameters, State outParameters )
        {
            _currentAction = "4";
            _animationCount++;

            if (_animationCount == 1)
            {
                _outString += "ACTION #" + _animationCount + ": PropTrigger - SnapshotAndReplace\n";
                _firstLayer = "NotLocal";
                _currentHandoff = HandoffBehavior.SnapshotAndReplace; //HandoffBehavior set in Markup.
                LoadMarkup("LO4");

                //Move mouse to center of TextBox, to trigger the LO4 Property Trigger animation.
                UserInput.MouseMove(_animElement,75,75);     //Trigger the animation.
            }
            else
            {
                _outString += "ACTION #" + _animationCount + ": PropTrigger - " + _currentHandoff + "\n";
                _secondLayer = "NotLocal";
                _currentHandoff = GetHandoff(inParameters["Handoff"]);
                _currentComposing = GetComposing();
                CreatePropertyTrigger( _currentHandoff );
                _expEventFired++;
                _animElement.Opacity = 0.5d;                                //Trigger the animation.
            }
            
            //Wait before continuing to invoke the next Action.
            WaitForSignal("ContinueToNextAction");

            return true;
        }

        /// <summary>
        /// Handler for apply_animation5
        /// [SCENARIO: DataTrigger: Storyboard in DataTrigger, within animated element's Template]
        /// </summary>
        /// 
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        private bool apply_animation5( State endState, State inParameters, State outParameters )
        {
            _currentAction = "5";
            _animationCount++;
            
            if (_animationCount == 1)
            {
                _outString += "ACTION #" + _animationCount + ": DataTrigger - SnapshotAndReplace\n";
                _firstLayer = "NotLocal";
                _currentHandoff = HandoffBehavior.SnapshotAndReplace; //HandoffBehavior set in Markup.
                LoadMarkup("LO5");

                //Move mouse to center of TextBox, to trigger the LO5 DataTrigger animation.
                UserInput.MouseMove(_animElement,75,75);     //Trigger the animation.
            }
            else
            {
                _outString += "ACTION #" + _animationCount + ": DataTrigger - " + _currentHandoff + "\n";
                _secondLayer = "NotLocal";
                _currentHandoff = GetHandoff(inParameters["Handoff"]);
                _currentComposing = GetComposing();
                CreateDataTriggerWithHandoff( _currentHandoff );
                _expEventFired++;
                
                //Move mouse to center of TextBox.
                //UserInput.MouseMove(animElement,75,75);     //Trigger the animation.
                _animElement.Text = "Avalon!";                                //Trigger the animation.
            }
            
            //Wait before continuing to invoke the next Action.
            WaitForSignal("ContinueToNextAction");

            return true;
        }

        /******************************************************************************
           * Function:          GetHandoff
           ******************************************************************************/
        /// <summary>
        /// GetHandoff: Determines the requested HandoffBehavior.
        /// </summary>
        /// <returns>The expected HandoffBehavior</returns>
        private HandoffBehavior GetHandoff(string inputParameter)
        {
            HandoffBehavior hb;

            if (inputParameter == "Compose")
            {
                hb = HandoffBehavior.Compose;
            }
            else
            {
                hb = HandoffBehavior.SnapshotAndReplace;
            }
            
            return hb;
        }

        /******************************************************************************
           * Function:          GetComposing
           ******************************************************************************/
        /// <summary>
        /// GetComposing: Uses the currentComposing boolean to track whether or not an Animation is
        /// supposed to Compose rather than SnapshotAndReplace.  This determination is affected by
        /// a change from an Animation on a "Local" Layer to one that is on a "NonLocal" Layer, and
        /// vice versa.
        /// </summary>
        /// <returns>Whether or not the Animation should compose with the previous one</returns>
        private bool GetComposing()
        {
            bool cmp = false;
            
            if (_secondLayer == _firstLayer)
            {
                //The current Animation is in the same Layer as the previous one, so must check the
                //specified HandoffBehavior to determine whether or not to expect Composition.
                if (_currentHandoff == HandoffBehavior.Compose)
                {
                    cmp = true;
                }
                else
                {
                    cmp = false;
                }
            }
            else
            {
                if (_secondLayer == "NotLocal")
                {
                    //The current Animation is in a different Layer, so they will Compose.
                    //That is, an Animation composes when it is in a separate Layer, which in 
                    //this case means that the current Animation and the previous animation change
                    //from EventTrigger/CodeAnimation (1,2,3) to PropertyTrigger/DataTrigger (4,5), or
                    //vice versa.
                    cmp = true;
                }
            }
            
            return cmp;
        }

        /******************************************************************************
           * Function:          LoadMarkup
           ******************************************************************************/
        /// <summary>
        /// LoadMarkup: Loads a page containing the initial Animation.
        /// </summary>
        private void LoadMarkup(string fileName)
        {
            _outString += "----LoadMarkup---- " + fileName + ".xaml\n";

            //Hide the parent window, so it won't overlay the Animation window.
            Window.Visibility          = Visibility.Hidden;   
            
            _navWin = new NavigationWindow();
            _navWin.Title                = "Trigger Variations Test";
            _navWin.Left                 = 50;
            _navWin.Top                  = 50;
            _navWin.Height               = 400;
            _navWin.Width                = 600;
            _navWin.WindowStyle          = WindowStyle.None;
            _navWin.ContentRendered     += new EventHandler(OnContentRendered);

            NameScope.SetNameScope(_navWin, new NameScope());

            fileName = @"pack://siteoforigin:,,,/" + fileName + ".xaml";
            
            _navWin.Navigate(new Uri(@fileName, UriKind.RelativeOrAbsolute));
            _navWin.Show();

            WaitForSignal("RenderPage");
        }

        /******************************************************************************
           * Function:          OnContentRendered
           ******************************************************************************/
        /// <summary>
        /// OnContentRendered: Invoked after all content on the page is rendered.
        /// </summary>
        private void OnContentRendered(object sender, EventArgs e)
        {
            DependencyObject control = null;
            
            //outString += "----OnContentRendered----\n";
            
            //Immediately click on the focus button before the test case starts, to avoid accidentally
            //moving the mouse across the animated element.
            //UserInput.MouseLeftClickCenter(focusButton);
            //Retrieve the root element
            StackPanel rootElement = (StackPanel)LogicalTreeHelper.FindLogicalNode((DependencyObject)_navWin.Content,"Root");

            //Retrieve the Storyboard ("resourceStoryboard") defined in the root element's Resources.
            //It will be used in apply_animation2.
            FindStoryboardInResource(rootElement);

            //Retrieve the animated element in markup.  Additional animations will be applied to it.
            if (_currentAction == "5")
            {
                //The animated element is inside a ControlTemplate.
                control = (DependencyObject)LogicalTreeHelper.FindLogicalNode((DependencyObject)_navWin.Content,"AnimateTemplate");
                _animElement = (TextBox)((Control)control).Template.FindName("Animate", (Control)control);
            }
            else
            {
                _animElement = (TextBox)LogicalTreeHelper.FindLogicalNode((DependencyObject)_navWin.Content,"Animate");
            }

            _animElement.MouseDown           += new MouseButtonEventHandler(OnMouseDown);
            _animElement.KeyDown             += new KeyEventHandler(OnKeyDown);
            _animElement.KeyUp               += new KeyEventHandler(OnKeyUp);
            _animElement.GotKeyboardFocus    += new KeyboardFocusChangedEventHandler(OnGotKeyboardFocus);

            //Retrive a button in markup.  It will be used to shift focus after every animation.
            _focusButton = (Button)LogicalTreeHelper.FindLogicalNode((DependencyObject)_navWin.Content,"FocusButton");

            if (_animElement != null)
            {
                if (_currentAction == "5")
                {
                    Animatable animatable = (Animatable)((Control)control).Template.FindName("AnimateAnimatable", (Control)control);
                    _RT = (RotateTransform)animatable;
                }
                else
                {
                    _RT = (RotateTransform)_animElement.LayoutTransform;
                }
                //Start a separate Timer to control the timing of verification.
                _dispatcherTickCount = 0;  //Needs to be reset for every case.
                _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
                _aTimer.Tick += new EventHandler(OnTick);
                _aTimer.Interval = _TIMER_INTERVAL;
                _aTimer.Start();
                //outString += "----DispatcherTimer Started----\n";
            }
            else
            {
                GlobalLog.LogEvidence("ERROR!! OnContentRendered: The animated element was not found in Markup. \n");
                _testPassed = false;
            }

            Signal("RenderPage", (TestResult)(_cumulativeResult ? TestResult.Pass : TestResult.Fail));
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            //outString += "--- MouseDown Fired ---\n";
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            //outString += "--- KeyDown Fired ---\n";
        }
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            //outString += "--- KeyUp Fired ---\n";
        }
        private void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            //outString += "--- GotKeyboardFocus Fired ---\n";
        }

        /******************************************************************************
           * Function:          FindStoryboardInResource
           ******************************************************************************/
        /// <summary>
        /// FindStoryboardInResource: Retrieve the Storyboard that is specified in the Resources section in
        /// each Markup file.  It is available for use for the apply_animation2 action.
        /// <param name="stackPanel">      A StackPanel       </param>
        /// <returns>  </returns>
        /// </summary>
        private void FindStoryboardInResource(StackPanel stackPanel)
        {
            //outString += "----FindStoryboardInResource----\n";
            
            try
            {
                _resourceStoryboard = (Storyboard)stackPanel.FindResource("RotateAnim");
            }
            catch( Exception e )
            {
                GlobalLog.LogEvidence("ERROR!! FindStoryboardInResource: A Storyboard was not found in StackPanel Resources: " + e.Message + "\n");
                _cumulativeResult = false;
            }
        }

        /******************************************************************************
           * Function:          CreateEventTrigger  [FOR ACTION apply_animation1]
           ******************************************************************************/
        /// <summary>
        /// Create an EventTrigger containing a new Storyboard --- PREVIEWMOUSEDOWN.
        /// </summary>
        /// <param name="handoff">      The requested HandoffBehavior       </param>
        /// <returns>  </returns>
        private void CreateEventTrigger(HandoffBehavior handoff)
        {
            //outString += "--- CreateEventTrigger ---\n";

            BeginStoryboard begin1 = CreateNewBeginStoryboard(handoff);
            
            EventTrigger eventTrigger1 = AnimationUtilities.CreateEventTrigger(System.Windows.Input.Mouse.PreviewMouseDownEvent, begin1);
            _animElement.Triggers.Add(eventTrigger1);
        }

        /******************************************************************************
           * Function:          StartStoryboardInResource  [FOR ACTION apply_animation2]
           ******************************************************************************/
        /// <summary>
        /// Begin a Storyboard that was defined in a Resource --- BEGUN IN CODE.
        /// </summary>
        /// <param name="handoff">      The requested HandoffBehavior       </param>
        /// <returns>  </returns>
        private void StartStoryboardInResource(HandoffBehavior handoff)
        {
            //outString += "--- StartStoryboardInResource ---\n";

            _resourceStoryboard.CurrentStateInvalidated  += new EventHandler(OnCurrentState);
            //This animation is always SnapshotAndReplace.
            _resourceStoryboard.Begin(_animElement, handoff, true);
        }

        /******************************************************************************
           * Function:          CreateEventTriggerInStyle  [FOR ACTION apply_animation3]
           ******************************************************************************/
        /// <summary>
        /// Create an EventTrigger in the root Element's Style section -- PREVIEWKEYDOWN.
        /// </summary>
        /// <param name="handoff">      The requested HandoffBehavior       </param>
        /// <returns>  </returns>
        private void CreateEventTriggerInStyle(HandoffBehavior handoff)
        {
            //outString += "--- CreateEventTriggerInStyle ---\n";

            BeginStoryboard begin3 = CreateNewBeginStoryboard(handoff);

            EventTrigger eventTrigger3 = AnimationUtilities.CreateEventTrigger(System.Windows.Input.Keyboard.PreviewKeyDownEvent, begin3);

            Style style3 = new Style();
            style3.TargetType = typeof(TextBox);
            style3.Triggers.Add(eventTrigger3);

            if (_currentStyle != null)
            {
                style3.BasedOn = _currentStyle;
                _currentStyle = style3;
            }
            _animElement.Style = style3;
        }

        /******************************************************************************
           * Function:          CreatePropertyTrigger  [FOR ACTION apply_animation4]
           ******************************************************************************/
        /// <summary>
        /// Create a PropertyTrigger, which changes the Opacity property to trigger the animation.
        /// </summary>
        /// <param name="handoff">      The requested HandoffBehavior       </param>
        /// <returns>  </returns>
        private void CreatePropertyTrigger(HandoffBehavior handoff)
        {
            //outString += "--- CreatePropertyTrigger ---\n";

            BeginStoryboard begin4 = CreateNewBeginStoryboard(handoff);

            Trigger trigger = AnimationUtilities.CreatePropertyTrigger(TextBox.OpacityProperty, 0.5d, begin4);

            Style style4 = new Style();
            style4.TargetType = typeof(TextBox);
            style4.Triggers.Add(trigger);

            if (_currentStyle != null)
            {
                style4.BasedOn = _currentStyle;
                _currentStyle = style4;
            }
            _animElement.Style = style4;
        }

        /******************************************************************************
           * Function:      CreateDataTriggerWithHandoff  [FOR ACTION apply_animation5]
           ******************************************************************************/
        /// <summary>
        /// Create a DataTrigger --- MOUSEOVER.
        /// </summary>
        /// <param name="handoff">      The requested HandoffBehavior       </param>
        /// <returns>  </returns>
        private void CreateDataTriggerWithHandoff(HandoffBehavior handoff)
        {
            //outString += "--- CreateDataTriggerWithHandoff ---\n";
            
            BeginStoryboard begin5 = CreateNewBeginStoryboard(handoff);

            DataTrigger dataTrigger = AnimationUtilities.CreateDataTrigger(new PropertyPath("(0)", TextBox.TextProperty), "Avalon!", begin5);

            Style style5 = new Style();
            style5.TargetType = typeof(TextBox);
            style5.Triggers.Add(dataTrigger);

            if (_currentStyle != null)
            {
                style5.BasedOn = _currentStyle;
                _currentStyle = style5;
            }
            _animElement.Style = style5;
        }


        /******************************************************************************
           * Function:          CreateNewBeginStoryboard
           ******************************************************************************/
        /// <summary>
        /// Create a BeginStoryboard containing a DoubleAnimation.
        /// </summary>
        /// <param name="handoff">      The requested HandoffBehavior       </param>
        /// <returns> A BeginStoryboard </returns>
        private BeginStoryboard CreateNewBeginStoryboard(HandoffBehavior handoff)
        {
            DoubleAnimation anim1 = new DoubleAnimation();                                             
            anim1.BeginTime                 = TimeSpan.FromMilliseconds(0);
            anim1.Duration                  = _DURATION_TIME;
            anim1.By                        = _byValue;
            anim1.CurrentStateInvalidated  += new EventHandler(OnCurrentState);

            Storyboard storyboard = new Storyboard();
            storyboard.Name = "story";
            storyboard.Children.Add(anim1);
            
            PropertyPath path  = new PropertyPath("(0).(1)", new DependencyProperty[] { TextBox.LayoutTransformProperty, RotateTransform.AngleProperty });
            Storyboard.SetTargetProperty(anim1, path);
            
            string name = "B" + _animationCount.ToString(); //Must have a unique name.
            BeginStoryboard beginStory = AnimationUtilities.CreateBeginStoryboard(_navWin, name, handoff);

            beginStory.Storyboard = storyboard;
            
            return beginStory;
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
            
            if (_dispatcherTickCount  == 1)
            {
                //Do nothing at 500 msec:  the initial animation, launched in markup, is active.
            }
            else if (_dispatcherTickCount % 2 == 1)
            {
                //ODD TICKS: 
                //No Animation is started here.

                //Move the mouse away the animated object before starting the next Animation, by 
                //clicking on the -edge- of the "focus button". [This avoids an animation causing
                //the mouse to accidentally move over the TextBox.]
                _focusButton.Focus();
                UserInput.MouseLeftDown(_focusButton);
                UserInput.MouseLeftUp(_focusButton);
            }
            else
            {
                //EVEN TICKS:
                if (_dispatcherTickCount == 6)
                {
                    //Delay finishing the test, to provide time for the last Animation to finish.
                    Signal("FinishTest", TestResult.Pass);
                }
                else
                {
                    //beyond Tick 1:  start the next Animation.
                    //Continue only when the mouse move has occurred.
                    //This will allow the model code to start the next Animation.
                    Signal("ContinueToNextAction", TestResult.Pass);
                }
            }
            
            //Update the currentExpAngle, which is the expected rotation angle at this point.
            _currentExpAngle += UpdateCurrentExpAngle(_dispatcherTickCount);

            _outString += "--------------------------------------------\n";
            _outString += "Tick " + _dispatcherTickCount + "--Exp Angle: " + _currentExpAngle + "\n";
            _outString += "Tick " + _dispatcherTickCount + "--Act Angle: " + _RT.Angle + "\n";
        }
        
        /******************************************************************************
        * Function:          UpdateCurrentExpAngle
        ******************************************************************************/
        /// <summary>
        /// Calculates the expected currentExpAngle, used for verification.
        /// </summary>
        /// <returns>The calculated currentExpAngle</returns>
        private double UpdateCurrentExpAngle(int count)
        {
            double angle = 0;
            
            switch (count)
            {
                case 1  :
                    angle += .25 * _byValue;
                    break;
                case 2  :
                    angle += .25 * _byValue;
                    break;
                case 3 :
                    if (_currentComposing)
                    {
                        angle += (.25 * _byValue) + (.25 * _byValue);
                    }
                    else
                    {   
                        //SnapshotAndReplace: start over when an additional animation is applied.
                        angle += .25 * _byValue;
                    }
                    break;
                case 4 :
                    if (_currentComposing)
                    {
                        angle += (.25 * _byValue) + (.25 * _byValue);
                    }
                    else
                    {   
                        //SnapshotAndReplace: start over when an additional animation is applied.
                        angle += .25 * _byValue;
                    }
                    break;
                case 5  :
                    angle += .25 * _byValue;
                    break;
                case 6  :
                    angle += .25 * _byValue;
                    break;
            }

            return angle;
        }
        
        /******************************************************************************
        * Function:          OnCurrentState
        ******************************************************************************/
        /// <summary>
        /// Invoked when the CurrentStateInvalidated event fires on the Animation.
        /// </summary>
        /// <returns>  </returns>
        private void OnCurrentState(object sender, EventArgs e)
        {
            _outString += "-- CurrentStateInvalidated fired: " + ((Clock)sender).CurrentState + "\n";
            
            //Will verify that the CurrentStateInvalidated event fired when beginning the animation.
            if (((Clock)sender).CurrentState == ClockState.Active)
            {
                _actEventFired++;
                _animationsStarted++;
            }
            else
            {
                _animationsFinished++;
            }
        }
        
        /******************************************************************************
           * Function:          VerifyAnimation
           ******************************************************************************/
        /// <summary>
        /// Verify the animated TextBox has rotated correctly.
        /// </summary>
        /// <returns>  </returns>
        private void VerifyAnimation()
        {
            //outString += "----VerifyAnimation----\n";
            
            double tolerance = 8;

            bool angleCorrect = ( (_RT.Angle <= _currentExpAngle + tolerance) && (_RT.Angle >= _currentExpAngle - tolerance) );
            bool valueCorrect = ( (_RT.CloneCurrentValue().Angle <= _currentExpAngle + tolerance) && (_RT.CloneCurrentValue().Angle >= _currentExpAngle - tolerance) );
            bool eventCorrect = ( _actEventFired == _expEventFired );
            bool animCorrect = angleCorrect && valueCorrect;
            _testPassed = animCorrect && _testPassed && eventCorrect;
            _testPassed = animCorrect && _testPassed;
            
            _outString += "---------------FINAL RESULTS---------------\n";
            _outString += "--Angle - Expected: " + _currentExpAngle +  " / Actual: " + _RT.Angle + "\n";
            _outString += "--Value - Expected: " + _currentExpAngle +  " / Actual: " + _RT.CloneCurrentValue().Angle + "\n";
            _outString += "--Event - Expected: " + _expEventFired + " / Actual: " + _actEventFired + "\n";
            _outString += "--------------------------------------------\n";
        }
        #endregion
    }
}
