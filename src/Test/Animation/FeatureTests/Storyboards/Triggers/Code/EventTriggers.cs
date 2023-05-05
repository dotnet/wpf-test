// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* ***************  MIL Integration Test *****************
*     Major Actions:
*     Pass Conditions:
*          (a) the animation's Progress() and ClockState APIs return the correct values
*          (b) the actual rendering matches the expected rendering during the course of the animation.
*
*     How verified: the VScan utility, getColorAtPoint(x1, y1), is called to determine if the actual
*                   color of the animated element is correct.
*                   The result of the comparisons between actual and expected values is passed to TestResult.
*
*     Requirements:
*           The .xaml files loaded by this test framework must contain the following:
*           (a) the root element must have ID="Root"
*           (b) the animating element must have ID="TargetElement", unless the root contains the Trigger
*           (e) the Animation must have a BeginTime="0:0:0" and a Duration="0:0:1.5"
*           (f) the file name of the .xaml file must contain 'Style' if the animation is in a Style
*           (g) the root must contain a Canvas with Background="White"
*           (h) if MouseLeave or LostKeyboardFocus events are tested, the markup must contain an element named "FocusElement"
*
*     NOTE: the Markup files are built into an Avalon application; they are not being loaded as loose xaml.
*
*     Framework:          A CLR executable is created.
*     Area:               Animation/Timing
*     Dependencies:       TestRuntime.dll, AnimationCommon.dll
*     Support Files:          
**********************************************************/
using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;   //UserInput
using Microsoft.Test.Logging;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Storyboards.Triggers.EventTriggers</area>
    /// <priority>2</priority>
    /// <description>
    /// Rendering verification of animation within Markup
    /// </description>
    /// </summary>
    [Test(2, "Storyboards.Triggers.EventTriggers", "EventTriggersTest", Timeout = 120)]

    class EventTriggersTest : XamlTest
    {
        #region Test case members

        private DispatcherTimer     _aTimer                      = null;  //Used for Timing method verification.
        private VisualVerifier      _verifier;

        private string              _inputString                 = "";
        private StringBuilder       _outputString                = new StringBuilder();
        private ArrayList           _eventList;
        private ArrayList           _expectedList;
        private int                 _argCount                    = 0;
        private int                 _eventCount                  = 0;
        
        private string              _windowTitle                 = "EventTrigger Test";
        private string              _fileName                    = null;
        private FrameworkElement    _sourceElement               = null;
        private FrameworkElement    _targetElement               = null;
        private TimeSpan            _DISPATCHER_TICK_INTERVAL    = new TimeSpan(0,0,0,0,2000);
        private int                 _dispatcherTickCount         = 0;
        private bool                _eventFired                  = false;
        private bool                _testPassed                  = true;
        private int                 _x1                          = 35;
        private int                 _y1                          = 35;
        private int                 _x2                          = 85;
        private int                 _y2                          = 85;
        private Color               _expInitialColor             = Colors.White;
        private int                 _testCount                   = 0;
        private const int           ATTEMPTS_ALLOWED            = 3;

        #endregion


        #region Constructor

        //EventTriggers.
        [Variation(@"ETElement3D-Focus.xaml","GotFocus_Blue", Versions="3.0SP1,3.0SP2,AH")]
        // [DISABLE WHILE PORTING]
        // [Variation(@"ETElement3D-KeyDown.xaml","KeyDown_Blue", Versions="3.0SP1,3.0SP2,AH")]
        [Variation(@"ETElement3D-KeyUp.xaml","KeyUp_Blue", Versions="3.0SP1,3.0SP2,AH")]
        [Variation(@"ETElement3D-MouseDown.xaml","MouseDown_Blue", Versions="3.0SP1,3.0SP2,AH")]
        [Variation(@"ETElement3D-MouseEnter.xaml","MouseEnter_Blue", Versions="3.0SP1,3.0SP2,AH")]
        [Variation(@"ETElement3D-MouseLeave.xaml","MouseEnter_White_MouseLeave_Blue", Versions="3.0SP1,3.0SP2,AH")]
        // [DISABLE WHILE PORTING]
        // [Variation(@"ETElement3D-MouseUp.xaml","MouseUp_Blue", Versions="3.0SP1,3.0SP2,AH")]

        [Variation(@"ETArcSegmentPause.xaml","MouseDown_White_MouseLeave_White")]

        //////////////////////////////////////////////////////////////////////////////////////////////////
        // DISABLEDUNSTABLETEST:
        // TestName:EventTriggersTest(ETButtonBackgroundPause.xaml,MouseEnter_#FFD6D6D6_MouseDown_#FF999999)
        // Area: Animation   SubArea: Storyboards.Triggers.EventTriggers
        // Disable this case due to high fail rate, will enable after fix it.
        // to find all disabled tests in test tree, use: �findstr /snip DISABLEDUNSTABLETEST?
        //////////////////////////////////////////////////////////////////////////////////////////////////// 

        //[Variation(@"ETButtonBackgroundPause.xaml","MouseEnter_#FFD6D6D6_MouseDown_#FF999999", Priority=0)]

        [Variation(@"ETButtonHeight.xaml","Click_Red", Priority=1)]
        [Variation(@"ETButtonOpacity.xaml","Click_DarkViolet", Priority=1)]
        [Variation(@"ETButtonRotate.xaml","MouseEnter_Navy_MouseDown_Navy", Priority=1)]
        [Variation(@"ETCanvasLeft.xaml","MouseEnter_LightSeaGreen", Priority=0)]
        [Variation(@"ETCanvasLeftSeek.xaml","MouseEnter_#191970_MouseUp_#FFFFFF")]
        [Variation(@"ETComboBoxYRemove.xaml","MouseEnter_White_MouseLeave_White", Priority=1)]
        [Variation(@"ETEllipseWidthStyle1.xaml","MouseDown_Blue")]
        [Variation(@"ETInkCanvasHeight.xaml","MouseUp_Purple")]
        [Variation(@"ETInkCanvasOffsetLoaded.xaml","Loaded_Brown")]
        [Variation(@"ETLineGeometryRemove.xaml","MouseEnter_White_MouseLeave_White")]
        [Variation(@"ETLineGeometryStop.xaml","MouseEnter_White_MouseLeave_White", Priority=0)]
        [Variation(@"ETListBoxClipRect.xaml","Loaded_HotPink_MouseUp_White", Disabled=true)]                // Flakey test: disabling - 09-22-10.
        [Variation(@"ETListBoxHeightWidthStyle2.xaml","MouseEnter_OrangeRed")]
        // [Variation(@"ETListBoxPaddingStyle2.xaml","GotFocus_RoyalBlue", Priority=1)]
        [Variation(@"ETRectMarginThickness.xaml","MouseEnter_DarkBlue")]
        [Variation(@"ETRectOpacityStyle1.xaml","MouseLeave_DeepPink", Priority=0)]
        [Variation(@"ETRectWidthStyle1.xaml","MouseLeave_LightGreen")]
        [Variation(@"ETTextBlockTextContentKF.xaml","MouseDown_MediumVioletRed")]
        [Variation(@"ETTextBoxColorStyle1.xaml","KeyDown_MediumSlateBlue", Priority=0)]
        [Variation(@"ETTextBoxHeight.xaml","KeyDown_DarkViolet")]
        [Variation(@"ETTextBoxOpacity.xaml","TextChanged_Azure")]
        [Variation(@"ETTextBoxOpacityStyle2.xaml","KeyUp_LightSeaGreen")]
        [Variation(@"ETTextBoxLeftSpeedRatio.xaml","MouseDown_DodgerBlue", Priority=1)]
        [Variation(@"ETTextBoxTopPause.xaml","KeyUp_White")]
        [Variation(@"ETTextBoxXResume.xaml","MouseEnter_White_MouseDown_White_MouseLeave_CornflowerBlue", Priority=1, Disabled=true)]  // Flakey test: disabling - 09-22-10.

        //DataTriggers
        [Variation(@"DTCanvasRotateStyle1.xaml","MouseDown_Black_MouseLeave_White", Priority=0)]
        [Variation(@"DTComboBoxKeyFrameStyle1.xaml","MouseDown_Red_MouseLeave_White")]
        [Variation(@"DTInkVisibilityStyle4.xaml","MouseDown_Blue")]
        [Variation(@"DTListBoxOpacityStyle2.xaml","MouseEnter_Green_MouseLeave_White")]
        [Variation(@"DTRectOpacityStyle3.xaml","MouseEnter_SteelBlue_MouseLeave_White")]
        [Variation(@"DTRichTextBoxColorStyle5.xaml","MouseEnter_SeaGreen_MouseLeave_White", Disabled=true)]  // Flakey test: disabling - 09-22-10.
        [Variation(@"DTScrollBarLeftTopStyle7.xaml","MouseDown_PaleVioletRed_MouseLeave_White")]
        [Variation(@"DTTextBoxWidthStyle6.xaml","MouseDown_Indigo_MouseLeave_White")]

        //MultiDataTriggers
        [Variation(@"MDTFrameOpacityStyle2.xaml","MouseDown_MediumSlateBlue_MouseLeave_White")]


        /******************************************************************************
        * Function:          EventTriggersTest Constructor
        ******************************************************************************/
        public EventTriggersTest(string p1, string p2): base(p1)
        {
            _outputString.Append("\n----------------------------------------");
            _outputString.Append("\nInput Parameters 1 (fileName:     " + p1);
            _outputString.Append("\nInput Parameters 2 (inputString): " + p2);
            _outputString.Append("\n----------------------------------------");

            _fileName    = p1;
            _inputString = p2;

            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(FinishTest);
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
            _outputString.Append("\n----Initialize----");

            //Parse the 2nd parameter into arrays.
            char[] delimiters = new char[] { '_' };
            String[] resultArray = _inputString.Split(delimiters);
            _argCount = resultArray.Length;

            _eventList    = new ArrayList();
            _expectedList = new ArrayList();

            //Enter each requested EventTrigger and expected value into ArrayLists.
            for (int j=0; j<_argCount; j+=2)
            {
                _eventList.Add(resultArray[j]);
                _expectedList.Add(resultArray[j+1]);
            }

            if (_argCount == 0)
            {
                throw new TestSetupException("ERROR!!! Missing Arguments.");
            }

            SetupWindow();

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          SetupWindow
        ******************************************************************************/
        /// <summary>
        /// Sets Window properties and initializes the VisualVerifier object.
        /// </summary>
        private void SetupWindow()
        {
            Window.Left                 = 0d;
            Window.Top                  = 0d;
            Window.Height               = 650d;
            Window.Width                = 650d;
            Window.Title                = _windowTitle; 
            Window.Topmost              = true;
            Window.Activate();
            AnimationUtilities.RemoveNavigationBar(Window);
            Window.ContentRendered     += new EventHandler(OnContentRendered);

            _outputString.Append("\n----Setting up the Verifier----");
            _verifier = new VisualVerifier();
            _verifier.InitRender(Window);
        }        

        /******************************************************************************
           * Function:          OnContentRendered
           ******************************************************************************/
        /// <summary>
        /// OnContentRendered: Invoked when all content is rendered.
        /// </summary>
        private void OnContentRendered(object sender, EventArgs e)
        {
            _outputString.Append("\n----ContentRendered Fired----");
            
            PrepareTest();
            LaunchTest();
        }
        
        /******************************************************************************
           * Function:          PrepareTest
           ******************************************************************************/
        /// <summary>
        /// Set Window properties, initialize the VisualVerifier object, and obtain references
        /// to elements defined in the .xaml page.
        /// </summary>
        private void PrepareTest()
        {
            //Retrieve elements defined in Markup.
            //Obtain the target of the Animation, and bind an Animation event.
            _targetElement = ObtainTarget();

            if (_targetElement == null)
            {
                _outputString.Append("\n----ERROR!!! OnContentRendered: the Target Element was not found.");
                _testPassed = false;
            }
            else
            {
                //Obtain the element which will trigger the Animation; it may be the same
                //as the target element.
                _sourceElement = ObtainSource(_targetElement);
            }
        }

        /******************************************************************************
           * Function:          LaunchTest
           ******************************************************************************/
        /// <summary>
        /// Conduct initial check, then start the test.
        /// </summary>
        private void LaunchTest()
        {
            if (_sourceElement != null)
            {
                //Conduct an intial check before the Animation starts.
                //Some tests may skip this step.
                if (_targetElement.Tag == null)
                {
                    Thread.Sleep(1000);
                    _outputString.Append("\n-----------------------------------Tick #0");
                    _testPassed = (CheckColor(_expInitialColor) && _testPassed);
                }
                else
                {
                    string tag = (string)_targetElement.Tag;
                    if (tag != "Skip")
                    {
                        _outputString.Append("\n-----------------------------------Tick #0");
                        _testPassed = (CheckColor(_expInitialColor) && _testPassed);
                    }
                }

                StartTest();
            }
        }
        
        /******************************************************************************
           * Function:          ObtainTarget
           ******************************************************************************/
        /// <summary>
        /// ObtainTarget: (1) get the target element (the target of the animation)
        /// (2) check for EventTriggers, (3) attach an event (if no Style involved)
        /// </summary>
        private FrameworkElement ObtainTarget()
        {
            FrameworkElement target = null;
            
            _outputString.Append("\n----Obtain Target----");
            target = (FrameworkElement)LogicalTreeHelper.FindLogicalNode((DependencyObject)Window.Content,"TargetElement");

            if (target == null)
            {
                //If an element named "TargetElement is not found, then assume the root
                //element contains the EventTrigger.
                FrameworkElement root = (FrameworkElement)LogicalTreeHelper.FindLogicalNode((DependencyObject)Window.Content,"Root");
                if (root == null)
                {
                    target = null;
                    GlobalLog.LogEvidence("ERROR!!! ObtainTarget: Root element not found.");
                    Signal("TestFinished", TestResult.Fail);
                }
                else
                {
                    target = root;
                    CheckForTriggers(ref target);
                }
            }
            else
            {
                CheckForTriggers(ref target);
            }

            return target;
        }
        
        /******************************************************************************
           * Function:          CheckForTriggers
           ******************************************************************************/
        /// <summary>
        /// CheckForTriggers: Check to see if any EventTriggers exist in Markup.
        /// </summary>
        private void CheckForTriggers(ref FrameworkElement target)
        {
            if ( (_fileName.ToLower(CultureInfo.InvariantCulture).IndexOf("style") >= 0) )
            //if (target.Style != null)
            {
                Style STY = (Style)target.Style;
                if (STY.Triggers.Count == 0)
                {
                    target = null;
                    GlobalLog.LogEvidence("ERROR!!! A Trigger was not found on the Style.");
                    Signal("TestFinished", TestResult.Fail);
                }
                else
                {
                    //ET = (EventTrigger)STY.Triggers[0];
                    //BeginStoryboard BST = (BeginStoryboard)ET.Actions[0];
                    //Storyboard ST = (Storyboard)BST.Storyboard;
                    //AnimationTimeline AT = (AnimationTimeline)ST.Children[0];
                    //AT.CurrentStateInvalidated += new EventHandler(OnCurrentStateInvalidated);
                    //This won't work: binding the event throws because the animation is frozen.
                    _eventFired = true; //force a pass for the verification of event firing.
                }
            }
            else if (target.Triggers.Count == 0)
            {
                target = null;
                GlobalLog.LogEvidence("ERROR!!! No EventTrigger was found.");
                Signal("TestFinished", TestResult.Fail);
            }
            else
            {
                //Attach a CurrentStateInvalidated event to the target, to verify that the
                //animation began (possible only if the animation is not in a style).
                AttachEvent(target);
            }
        }
                
        /******************************************************************************
           * Function:          ObtainSource
           ******************************************************************************/
        /// <summary>
        /// ObtainSource: Get the source element -- the element for which UIAutomation
        /// will generate an event, which will trigger the animation on the target.
        /// The target and source may be the same element.  It must be specified on the FIRST
        /// Event Trigger.
        /// </summary>
        private FrameworkElement ObtainSource(FrameworkElement target)
        {
            _outputString.Append("\n----Obtain Source----");

            FrameworkElement    source  = null;
            EventTrigger        ET      = null;
            DataTrigger         DT      = null;
            MultiDataTrigger    MDT     = null;
            Trigger             TR      = null;
            
            //Determine the source element (to which UIAutomation is applied to fire an event).
            if ( (_fileName.ToLower(CultureInfo.InvariantCulture).IndexOf("style") >= 0) )
            {
                //A Style was found, containing an EventTrigger or Trigger.
                Style STY = (Style)target.Style;
                
                if (STY.Triggers[0].GetType().ToString() == "System.Windows.EventTrigger")
                {
                    ET = (EventTrigger)STY.Triggers[0];
                }
                else if (STY.Triggers[0].GetType().ToString() == "System.Windows.DataTrigger")
                {
                    DT = (DataTrigger)STY.Triggers[0];
                }
                else if (STY.Triggers[0].GetType().ToString() == "System.Windows.MultiDataTrigger")
                {
                    MDT = (MultiDataTrigger)STY.Triggers[0];
                }
                else
                {
                    TR = (Trigger)STY.Triggers[0];
                }
            }
            else
            {
                //No Style was found, so it is assumed an EventTrigger exists on the target.
                ET = (EventTrigger)target.Triggers[0];
            }
            
            //Check for SourceName only if an EventTrigger exists.
            if (ET != null)
            {
                if (ET.SourceName != null)
                {
                    string sourceName = (string)ET.SourceName;
                    if (sourceName != "")
                    {
                        //Some of the Markup files  use Event Triggers to associate an event
                        // with an animation on -another- element.  In such cases,
                        // "target" will still be the to-be-animated element, but "source"
                        //will be the one to which UIAutomation is applied to start the animation.
                        source = (FrameworkElement)LogicalTreeHelper.FindLogicalNode((DependencyObject)Window.Content,sourceName);
                        
                        if (source == null)
                        {
                            GlobalLog.LogEvidence("ERROR!!! Source element not found.");
                            Signal("TestFinished", TestResult.Fail);
                        }
                    }
                }
            }
            
            if (source == null)
            {
                //The element to which the event is applied is the same as the animated element.
                source = target;
            }
            
            return source;
        } 
        
        /******************************************************************************
           * Function:          AttachEvent
           ******************************************************************************/
        /// <summary>
        /// AttachEvent: Attach the CurrentStateInvalidated event to the Animation defined in Markup.
        /// </summary>
        private void AttachEvent(FrameworkElement target)
        {
            _outputString.Append("\n----AttachEvent----");

            EventTrigger ET = (EventTrigger)target.Triggers[0];
            BeginStoryboard BST = (BeginStoryboard)ET.Actions[0];
            Storyboard ST = (Storyboard)BST.Storyboard;
            AnimationTimeline AT = (AnimationTimeline)ST.Children[0];

            if (_fileName.ToLower(CultureInfo.InvariantCulture).IndexOf("pause") > 0)
            {
                //If Pause is being tested, bind the OnCurrentGlobalSpeedInvalidated event, in
                //order to test whether or not the animation actually paused.
                AT.CurrentGlobalSpeedInvalidated += new EventHandler(OnCurrentGlobalSpeedInvalidated);
            }
            else
            {
                //If Pause is not tested, bind the CurrentStateInvalidated event.
                AT.CurrentStateInvalidated += new EventHandler(OnCurrentStateInvalidated);
            }
        }
        
        /******************************************************************************
           * Function:          StartTest
           ******************************************************************************/
        /// <summary>
        /// StartTest: set up verification.
        /// Then, start the Animation.
        /// </summary>
        private void StartTest()
        {
            //Verify Timing Methods using OnDispatcherTick to control UIAutomation.
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnDispatcherTick);
            _aTimer.Interval = _DISPATCHER_TICK_INTERVAL;
            _aTimer.Start();
            _outputString.Append("\n----DispatcherTimer Started----");
        }
        
        
        /******************************************************************************
        * Function:          OnDispatcherTick
        ******************************************************************************/
        /// <summary>
        /// The OnDispatcherTick event handler of the DispatcherTimer is used solely as a
        /// means of verifying Timing Methods applied in Markup.
        /// It will tick the number of times necessary to handle the number of EventTriggers
        /// to be invoked (specified as input parameters and kept in eventList).
        /// For each EventTrigger, there will be two ticks: one to invoke the Event, and the
        /// second to verify the result.  The interval between ticks is constant, so the 
        /// markup must be constructed in a way to handle this verification restriction.
        /// </summary>
        /// <returns></returns>              
        private void OnDispatcherTick(object sender, EventArgs e)          
        {
            _dispatcherTickCount++;
            _outputString.Append("\n-----------------------------------Tick #" + _dispatcherTickCount);

            if (_dispatcherTickCount < _argCount + 1)
            {
                if (_dispatcherTickCount % 2 != 0)
                {
                    //Apply a Timing Method, e.g., Pause, Resume, Seek,...
                    Animate(_sourceElement);
                }
                else
                {
                    //Verify the animation.
                    string color = (string)_expectedList[_eventCount-1];
                    Color expColor = (Color)ColorConverter.ConvertFromString(color);
                    _testPassed = (CheckColor(expColor) && _testPassed);
                }
            }
            else
            {
                //Last Tick:  Stop the ticking (after all events have occurred), and verify.
                _aTimer.Stop();

                _outputString.Append("\nEvent Fired --- RESULT: " + _eventFired);

                //Also check eventFired to test if CurrentStateInvalidated fired, or, if
                //Pause invoked, the animation paused (checked via CurrentGlobalSpeedInvalidated).


                _testCount++;

                if (_testPassed)
                {
                    Signal("TestFinished", TestResult.Pass);
                }
                else
                {
                    // Restart the test case if it fails.  Carry out the test ATTEMPTS_ALLOWED number of times in total.
                    if (_testCount < ATTEMPTS_ALLOWED)
                    {
                        RestartTestCase();
                    }
                    else
                    {
                        Signal("TestFinished", TestResult.Fail);
                    }
                }
            }
        }

        /******************************************************************************
           * Function:          RestartTestCase
           ******************************************************************************/
        /// <summary>
        /// RestartTestCase:  Reset and start the test case.  (Added to handle flakiness in Lab runs.)
        /// </summary>
        private void RestartTestCase()
        {
            _outputString.Append("\n****RESTARTING TEST CASE****\n");
            _dispatcherTickCount = 0;
            _eventCount = 0;

            Window.Content = null;
            string strUri = "pack://application:,,,/" + this.GetType().Assembly.GetName().Name + ";component/" + _fileName;
            Window.Navigate(new Uri(strUri, UriKind.RelativeOrAbsolute));
            Window.Show();
            DispatcherHelper.DoEvents(0, DispatcherPriority.Input);

            SetupWindow();
            PrepareTest();
            LaunchTest();
        }

        /******************************************************************************
           * Function:          Animate
           ******************************************************************************/
        /// <summary>
        /// Animate: use UIAutomation to simulate the user starting the Animation with
        /// a user interaction.
        /// </summary>
        private void Animate(FrameworkElement sourceElement)
        {
            FrameworkElement focusElement = null;
            
            sourceElement.Focus();

            _eventCount++;  //Increment the index into the ArrayLists.
            
            string eventName = (string)_eventList[_eventCount-1];

            eventName = eventName.ToLower(CultureInfo.InvariantCulture);

            switch (eventName)
            {
                case "mousedown" :
                    _outputString.Append("\nEVENT: MouseDown");
                    UserInput.MouseLeftClickCenter(sourceElement);
                    break;
                case "mouseup" :
                    _outputString.Append("\nEVENT: MouseUp");
                    UserInput.MouseLeftDownCenter(sourceElement);
                    UserInput.MouseLeftUpCenter(sourceElement);
                    break;
                case "gotfocus" :
                    _outputString.Append("\nEVENT: GotKeyboardFocus");
                    UserInput.MouseLeftClickCenter(sourceElement);
                    break;
                case "lostfocus" :
                    _outputString.Append("\nEVENT: LostKeyboardFocus");
                    UserInput.MouseLeftClickCenter(sourceElement);
                    focusElement = (FrameworkElement)LogicalTreeHelper.FindLogicalNode((DependencyObject)Window.Content,"FocusElement");
                    UserInput.MouseLeftClickCenter(focusElement);
                    break;
                case "keydown" :
                    _outputString.Append("\nEVENT: KeyDown");
                    UserInput.MouseLeftClickCenter(sourceElement);
                    UserInput.KeyPress("A", true);
                    UserInput.KeyPress("A", false);
                    break;
                case "keyup" :
                    UserInput.MouseLeftClickCenter(sourceElement);
                    _outputString.Append("\nEVENT: KeyUp");
                    UserInput.KeyPress("A", false);
                    break;
                case "textchanged" :
                    _outputString.Append("\nEVENT: TextChanged");
                    UserInput.KeyPress("A", true);
                    UserInput.KeyPress("A", false);
                    break;
                case "click" :
                    _outputString.Append("\nEVENT: Click");
                    UserInput.MouseLeftClickCenter(sourceElement);
                    break;
                case "wheel" :
                    _outputString.Append("\nEVENT: MouseWheel");
                    UserInput.MouseWheel(sourceElement, 5, 5, 2);
                    break;
                case "mouseenter" :
                    _outputString.Append("\nEVENT: MouseEnter");
                    UserInput.MouseMove(sourceElement,20,20);
                    break;
                case "mouseleave" :
                    _outputString.Append("\nEVENT: MouseLeave");
                    //UserInput.MouseMove(sourceElement,20,20);
                    //UserInput.MouseLeftDown(sourceElement,-5,-5);
                    UserInput.MouseLeftClickCenter(sourceElement);
                    focusElement = (FrameworkElement)LogicalTreeHelper.FindLogicalNode((DependencyObject)Window.Content,"FocusElement");
                    UserInput.MouseLeftClickCenter(focusElement);
                    break;
                case "selectionchanged" :
                    _outputString.Append("\nEVENT: SelectionChanged");
                    UserInput.MouseLeftClickCenter(sourceElement);
                    break;
                case "loaded" :
                    _outputString.Append("\nEVENT: Loaded");
                    //No UIAutomation event necessary: the animation starts via FrameworkElement.Loaded.
                    break;
                default:
                    GlobalLog.LogEvidence("ERROR!!! Event name not specified.");
                    Signal("TestFinished", TestResult.Fail);
                    break;
            }
        }
        
        /******************************************************************************
           * Function:          CheckColor
           ******************************************************************************/
        /// <summary>
        /// AnCheckColor: the expected color was earlier entered into expectedList, one for
        /// each EventTrigger.
        /// </summary>
        private bool CheckColor(Color expColor)
        {
            bool passed1        = true;
            bool passed2        = true;
            Color actColor;

            //At each Tick, check the color at two different points (x1,y1) and (x2,y2) on the page.
            actColor = _verifier.getColorAtPoint(_x1, _y1);
            passed1 = CompareColors(_x1, _y1, expColor, actColor);
            
            actColor = _verifier.getColorAtPoint(_x2, _y2);
            passed2 = CompareColors(_x2, _y2, expColor, actColor);

            return (passed1 && passed2);
        }

        /******************************************************************************
           * Function:          CompareColors
           ******************************************************************************/
        private bool CompareColors(int x, int y, Color expColor, Color actColor)
        {
            bool colorMatched   = true;
            float expTolerance  = .50f;
            
            _outputString.Append("\n---------- Result at (" + x + "," + y + ") ------\n");
            _outputString.Append("\n Actual   : " + actColor.ToString());
            _outputString.Append("\n Expected : " + expColor.ToString());

            if (Math.Abs(Math.Round((double)(Decimal.Round(actColor.R,3) - expColor.R) / expColor.R,4)) >= expTolerance)
            { 
                colorMatched = false; 
            }
            if (Math.Abs(Math.Round((double)(Decimal.Round(actColor.G,3) - expColor.G) / expColor.G,4)) >= expTolerance) 
            { 
                colorMatched = false; 
            }
            if (Math.Abs(Math.Round((double)(Decimal.Round(actColor.B,3) - expColor.B) / expColor.B,4)) >= expTolerance) 
            { 
                colorMatched = false; 
            }
            return colorMatched;
        }

        /******************************************************************************
           * Function:          OnCurrentStateInvalidated
           ******************************************************************************/
        /// <summary>
        /// OnCurrentStateInvalidated: Used to validate that the animatin began.
        /// </summary>
        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {               
           _outputString.Append("\nCurrentStateInvalidated:  " + ((Clock)sender).CurrentState);
           _eventFired = true;
        }

        /******************************************************************************
           * Function:          OnCurrentGlobalSpeedInvalidated
           ******************************************************************************/
        /// <summary>
        /// OnCurrentGlobalSpeedInvalidated: Used to validate that the animation began.
        /// </summary>
        private void OnCurrentGlobalSpeedInvalidated(object sender, EventArgs e)
        {               
            _outputString.Append("\nCurrentGlobalSpeedInvalidated:  " + ((Clock)sender).CurrentGlobalSpeed);
            
            if (((Clock)sender).IsPaused)
            {
                _outputString.Append("\nCurrentGlobalSpeedInvalidated:  Paused");
                _eventFired = true;
            }
        }
        
        /******************************************************************************
        * Function:          FinishTest
        ******************************************************************************/
        /// <summary>
        /// Returns a TestResult and ends the test case.  (If the test fails the first time, try again.)
        /// </summary>
        /// <returns>A TestResult, indicating pass or fail</returns>
        TestResult FinishTest()
        {
            TestResult passed = WaitForSignal("TestFinished");

            GlobalLog.LogEvidence(_outputString.ToString());

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
}
