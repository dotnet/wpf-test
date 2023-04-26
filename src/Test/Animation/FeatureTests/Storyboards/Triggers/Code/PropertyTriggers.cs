// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* ***************  Animation Integration Test *****************
*     Major Actions:
*           For each test, a Property Trigger is established for a different DP.  The Property Trigger is
*           associated with a DoubleAnimation [TranslateTransform.YProperty] on a Button's RenderTransform.
*           The value is then explicitly changed, which triggers the Animation.
*     Pass Conditions:
*           Each test case passes if the animation successfully completes.
*     How verified:
*          A VScan utility is used to get the color of two points.
*          The result of the comparisons between actual and expected values is passed to TestResult.
*     Framework:        A CLR executable is created.
*     Area:             Animation
*     Dependencies:     TestRuntime.dll
*     Support Files:    

*/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.Input;   //UserInput
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Storyboards.Triggers.PropertyTriggers</area>
    /// <priority>2</priority>
    /// <description>
    /// Verify applying Animations on a Button's properties via Property Triggers.
    /// </description>
    /// </summary>
    [Test(2, "Storyboards.Triggers.PropertyTriggers", "PropertyTriggersTest", Timeout=120)]
    public class PropertyTriggersTest : WindowTest
    {
        #region Test case members

        private string                  _inputString                 = "";
        private StringBuilder           _outputString                = new StringBuilder();
        private DispatcherTimer         _aTimer                      = null;  //Used for Timing method verification.
        private VisualVerifier          _verifier                    = null;

        private Canvas                  _body;
        private Button                  _button;
        private string                  _windowTitle                 = "PropertyTrigger Animations";
        private TimeSpan                _BEGIN_TIME                  = TimeSpan.FromMilliseconds(0);
        private Duration                _DURATION_TIME               = new Duration(TimeSpan.FromMilliseconds(1000));
        private TimeSpan                _DISPATCHER_TICK_INTERVAL    = new TimeSpan(0,0,0,0,2050);
        private int                     _dispatcherTickCount         = 0;
        private Color                   _expInitialColor             = Colors.DeepPink;
        private Color                   _expColor                    = Colors.Blue;
        private int                     _x1                          = 40;
        private int                     _y1                          = 20;
        private int                     _x2                          = 80;
        private int                     _y2                          = 80;

        private bool                    _mouseMoved                  = false;
        private bool                    _expectTriggering            = true;
        private bool                    _testPassed                  = false;
        private int                     _renderedCount               = 0;
        private int                     _treeCreationCount           = 0;
        private const int               ATTEMPTS_ALLOWED            = 3;
        
        public static bool              eventFired                  = false;

        #endregion


        #region Constructor

        // [DISABLE WHILE PORTING]
        // [Variation("Width", Priority=0)]
        // [Variation("IsDefaultTrue")]
        // [Variation("IsDefaultFalse1")]
        // [Variation("IsDefaultFalse2")]
        // [Variation("IsCancel")]
        // [Variation("IsPressed", Priority=0)]
        // [Variation("ClickMode")]
        // [Variation("Content", Disabled=true)]                       // Flakey test: disabling - 09-22-10.
        // [Variation("HasContent", Disabled=true)]                    // Flakey test: disabling - 09-22-10.
        // [Variation("Background")]
        // [Variation("BorderBrush")]
        // [Variation("BorderThickness")]
        // [Variation("FontFamily", Disabled=true)]                    // Flakey test: disabling - 09-22-10.
        // [Variation("FontSize", Disabled=true)]                      // Flakey test: disabling - 09-22-10.
        // [Variation("FontStretch", Disabled=true)]                   // Flakey test: disabling - 09-22-10.
        // [Variation("FontStyle", Disabled=true)]                     // Flakey test: disabling - 09-22-10.
        // [Variation("FontWeight", Disabled=true)]                    // Flakey test: disabling - 09-22-10.
        // [Variation("Foreground", Priority=1)]
        // [Variation("HorizontalContentAlignment", Disabled=true)]    // Flakey test: disabling - 09-22-10.
        // [Variation("IsTabStop")]
        // [Variation("Padding", Disabled=true)]                       // Flakey test: disabling - 09-22-10.
        // [Variation("TabIndex")]
        // [Variation("ActualHeight")]
        // [Variation("ContextMenu")]
        // [Variation("Cursor")]
        // [Variation("Focusable")]
        // [Variation("LayoutTransform")]
        // [Variation("Margin")]
        // [Variation("MaxWidth")]
        // [Variation("Name")]
        // [Variation("Tag")]
        // [Variation("ToolTip", Priority=0)]
        // [Variation("VerticalAlignment")]
        // [Variation("FlowDirection", Disabled=true)]                 // Flakey test: disabling - 09-22-10.
        // [Variation("IsMouseOver", Priority=1)]
        // [Variation("IsEnabled")]
        // [Variation("IsFocused")]
        // [Variation("IsMouseOver2", Priority=1)]
        // [Variation("IsVisible")]
        [Variation("Opacity0", Priority=0)]
        // [Variation("Opacity1", Priority=0)]
        // [Variation("Opacity2")]
        [Variation("RenderTransform")]
        // [Variation("SnapsToDevicePixels")]
        // [Variation("IsHitTestVisible")]
        // [Variation("OverridesDefaultStyle")]
        // [Variation("BackgroundOpacity")]

        /******************************************************************************
        * Function:          PropertyTriggersTest Constructor
        ******************************************************************************/
        public PropertyTriggersTest(string testValue)
        {
            GlobalLog.LogStatus("----------PropertyTriggersTest: " + testValue);
            _inputString = testValue;

            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(FinishTest);
        }

        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Sets up the test case.
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        TestResult Initialize()
        {
            CreateTree();

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          CreateTree
        ******************************************************************************/
        /// <summary>
        /// Creates the window content and initializes the VisualVerifier object.
        /// </summary>
        private void CreateTree()
        {
            Window.Title               = _windowTitle;
            Window.Left                = 0;
            Window.Top                 = 0;
            Window.Height              = 400;
            Window.Width               = 400;
            Window.WindowStyle         = WindowStyle.None;
            Window.ContentRendered     += new EventHandler(OnContentRendered);

            _body  = new Canvas();
            Window.Content = _body;
            _body.Width              = 400;
            _body.Height             = 400;
            _body.Background         = Brushes.DeepPink;

            _button  = new Button();
            _button.Background   = Brushes.Blue;
            _button.Width        = 120d;
            _button.Height       = 120d;

            Rectangle rect = new Rectangle();
            rect.Width        = 150d;
            rect.Height       = 150d;
            rect.Fill         = Brushes.Blue;
            _button.Content    = rect;

            TranslateTransform translateTransform = new TranslateTransform();
            translateTransform.X     = 20;
            translateTransform.Y     = 200;
            _button.RenderTransform = translateTransform;

            _body.Children.Add(_button);

            _outputString.Append("---Tree created---\n");

            _verifier = null;
            _verifier = new VisualVerifier();
            _verifier.InitRender(Window);

            _treeCreationCount++;
        }
         
        /******************************************************************************
           * Function:          OnContentRendered
           ******************************************************************************/
        /// <summary>
        /// OnContentRendered: Invoked when the .xaml page loads.
        /// Obtains references to elements defined in the .xaml page. Then starts the test.
        /// </summary>
        private void OnContentRendered(object sender, EventArgs e)
        {
            _renderedCount++;
            
            _outputString.Append("---OnContentRendered Fired---\n");

            // HACK-HACK:  need to ignore spurious firing of ContentRendered when test is restarted.
            if (_renderedCount == _treeCreationCount)
            {
                StartTest();
            }
            else
            {
                _renderedCount--;
            }
        }
        
        /******************************************************************************
           * Function:          StartTest
           ******************************************************************************/
        /// <summary>
        /// StartTest: sets up verification. Then, starts the Animation via a DispatcherTimer.
        /// </summary>
        private void StartTest()
        {
            // Verify the initial color before the Animation begins.
            Thread.Sleep(1000);
            _outputString.Append("-----------------------------------Tick #0\n");
            _testPassed = (PropertyTriggersHelper.CheckColor(_x1, _y1, _x2, _y2, _expInitialColor, _verifier, ref _outputString) && _testPassed);

            //Verify Timing Methods using OnDispatcherTick to control UIAutomation.
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnDispatcherTick);
            _aTimer.Interval = _DISPATCHER_TICK_INTERVAL;
            _aTimer.Start();
            _outputString.Append("---DispatcherTimer Started---\n");
        }
        
        /******************************************************************************
        * Function:          OnDispatcherTick
        ******************************************************************************/
        /// <summary>
        /// The OnDispatcherTick event handler of the DispatcherTimer is used solely as a
        /// means of verifying Timing Methods applied in Markup.
        /// </summary>
        /// <returns></returns>              
        private void OnDispatcherTick(object sender, EventArgs e)          
        {
            _dispatcherTickCount++;
            _outputString.Append("-----------------------------------Tick #" + _dispatcherTickCount + "\n");
            
            if (_dispatcherTickCount == 1)
            {
                _testPassed = true;

                // Start the Animation on the first Tick.
                _outputString.Append("---TriggerAnimation - Animating: " + _inputString + "---\n");
                PropertyTriggersHelper.TriggerAnimation(_inputString, _button, _BEGIN_TIME, _DURATION_TIME, ref _mouseMoved, ref _expectTriggering);
            }
            else
            {

                // Stop the Timer and verify the Animation (which has completed).
                _aTimer.Stop();

                _outputString.Append("CurrentStateInvalidated Fired --- RESULT: " + eventFired + "\n");
                
                // Verify the animation.
                if (_expectTriggering)
                {
                    _testPassed = (PropertyTriggersHelper.CheckColor(_x1, _y1, _x2, _y2, _expColor, _verifier, ref _outputString) && _testPassed);
                }
                else
                {
                    eventFired = true; //Force a "pass" for event firing when no animation expected.
                    _outputString.Append("[CurrentStateInvalidated not expected to fire]\n");
                    _testPassed = (PropertyTriggersHelper.CheckColor(_x1, _y1, _x2, _y2, _expInitialColor, _verifier, ref _outputString) && _testPassed);
                }
                
                // Also check eventFired to test if CurrentStateInvalidated fired
                _testPassed = (eventFired && _testPassed);
                ProcessTestResult();
            }
        }

        /******************************************************************************
        * Function:          ProcessTestResult
        ******************************************************************************/
        /// <summary>
        /// ProcessTestResult:  Signal that the test has finished.  Restart the test if necessary.
        /// </summary>
        /// <returns></returns>              
        private void ProcessTestResult()          
        {
            if (_mouseMoved)
            {
                // "Restore" mouse position to 0,0, to prevent a current mouse move affecting later cases.
                UserInput.MouseMove(Window,0,0);
            }
            
            if (_testPassed)
            {
                Signal("TestFinished", TestResult.Pass);
            }
            else
            {
                // Restart the test case if it fails.  Carry out the test ATTEMPTS_ALLOWED number of times in total.
                if (_treeCreationCount < ATTEMPTS_ALLOWED)
                {
                    // Try again.
                    _outputString.Append("\n****RESTARTING TEST CASE****\n");
                    eventFired = false;
                    _dispatcherTickCount = 0;
                    _mouseMoved = false;
                    _expectTriggering = true;
                    CreateTree();
                }
                else
                {
                    Signal("TestFinished", TestResult.Fail);
                }
            }
        }

        /******************************************************************************
        * Function:          FinishTest
        ******************************************************************************/
        /// <summary>
        /// FinishTest: Returns a Pass/Fail result.
        /// </summary>
        /// <returns>A TestResult object, indicating Pass or Fail</returns>
        TestResult FinishTest()
        {
            WaitForSignal("TestFinished");
            
            GlobalLog.LogStatus(_outputString.ToString());

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
