// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Storyboards.LowLevelScenarios.Regressions</area>


    [Test(2, "Storyboards.LowLevelScenarios.Regressions", "StoryboardInTemplateTest")]
    public class StoryboardInTemplateTest : WindowTest
    {
        #region Test case members

        private string                          _inputString             = "";
        private Button                          _button1                 = null;
        private TextBlock                       _animatedElement         = null;
        private BeginStoryboard                 _beginStory              = null;
        private Storyboard                      _story                   = null;
        private ResourceDictionary              _templateDictionary      = null;
        private ThicknessAnimation              _anim                    = null;
        private ControlTemplate                 _controlTemplate         = null;
        private FrameworkElementFactory         _templateContent         = null;
        private string                          _STORYKEY                = "StoryKey";
        private double                          _toValue                 = 10d;
        private double                          _actSpeedRatioMid        = 0;
        private DispatcherTimer                 _aTimer                  = null;
        private int                             _tickCount               = 0;
        
        #endregion


        #region Constructor
        
        [Variation("Begin")]
        [Variation("Pause")]
        [Variation("PauseResume", Priority=1)]
        [Variation("Seek")]
        [Variation("SeekAlignedToLastTick", Priority=0)]
        [Variation("Stop")]
        [Variation("SkipToFill")]
        [Variation("SetSpeedRatio")]
        [Variation("Remove")]
        [Variation("Clone", Priority=0)]
        
        /******************************************************************************
        * Function:          StoryboardInTemplateTest Constructor
        ******************************************************************************/
        public StoryboardInTemplateTest(string testValue)
        {
            _inputString = testValue;
            InitializeSteps += new TestStep(CreateTree);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          CreateTree
        ******************************************************************************/
        /// <summary>
        /// Creates the window content.
        /// </summary>
        /// <returns>TestResult</returns>
        TestResult CreateTree()
        {
            Window.Width        = 300d;
            Window.Height       = 300d;
            Window.Left         = 0d;
            Window.Top          = 0d;
            Window.ContentRendered     += new EventHandler(OnContentRendered);

            NameScope.SetNameScope(Window, new NameScope());

            Canvas body = new Canvas();
            body.Background = Brushes.SlateBlue;

            //Create a Button, which will contain the ControlTemplate with the to-be-animated TextBlock.
            _button1 = new Button();
            body.Children.Add(_button1);
            Canvas.SetTop  (_button1, 75d);
            Canvas.SetLeft (_button1, 75d);
            _button1.Name          = "BUTTON1";
            Window.RegisterName(_button1.Name, _button1);

            //CREATE RESOURCE DICTIONARY FOR THE CONTROLTEMPLATE.
            _templateDictionary  = new ResourceDictionary();
            
            NameScope.SetNameScope(Window, new NameScope());
            
            _templateContent = new FrameworkElementFactory(typeof(Canvas));
            _templateContent.SetValue(Canvas.BackgroundProperty, Brushes.SpringGreen);
            _templateContent.SetValue(Canvas.HeightProperty, 70d);
            _templateContent.SetValue(Canvas.WidthProperty, 120d);

            FrameworkElementFactory textBlock = new FrameworkElementFactory(typeof(TextBlock), "textblock1");
            textBlock.SetValue(TextBlock.BackgroundProperty, Brushes.SlateBlue);
            textBlock.SetValue(TextBlock.FontFamilyProperty, new FontFamily("Trebuchet"));
            textBlock.SetValue(TextBlock.TextProperty, "Avalon!");
            textBlock.SetValue(TextBlock.FontSizeProperty, 32d);
            textBlock.SetValue(TextBlock.NameProperty, "textblock1");
            textBlock.SetValue(TextBlock.HeightProperty, 50d);
            textBlock.SetValue(TextBlock.WidthProperty, 100d);
            textBlock.SetValue(TextBlock.PaddingProperty, new Thickness(0d));

            _templateContent.AppendChild(textBlock);

            //CREATE ANIMATION AND STORYBOARD.
            _story = new Storyboard();
            _story.Name = "story";

            _anim = new ThicknessAnimation();
            _anim.To              = new Thickness(_toValue);
            _anim.BeginTime       = TimeSpan.FromMilliseconds(0);
            _anim.Duration        = new Duration(TimeSpan.FromMilliseconds(1500));
            
            DependencyProperty dp = TextBlock.PaddingProperty;
            PropertyPath pp = new PropertyPath( "(0)", dp );
            Storyboard.SetTargetProperty(_anim, pp);
            _story.Children.Add(_anim);
            
            //CREATE BEGINSTORYBOARD.
            _beginStory = AnimationUtilities.CreateBeginStoryboard(Window, "beginStory");
            _beginStory.Storyboard = _story;

            //CREATE CONTROL TEMPLATE
            _controlTemplate = new ControlTemplate(typeof(Button));
            _controlTemplate.RegisterName("textblock1", textBlock);
            
            //ADD STORYBOARD TO CONTROL TEMPLATE'S RESOURCE DICTIONARY.
            _templateDictionary.Add(_STORYKEY, _story);
            ((ControlTemplate)_controlTemplate).Resources = _templateDictionary;
            
            _controlTemplate.VisualTree = _templateContent;
            _button1.Template = _controlTemplate;

            Window.Content = body;
            
            return TestResult.Pass;
        }
          
        /******************************************************************************
        * Function:          OnContentRendered
        ******************************************************************************/
        /// <summary>
        /// Fires when the page loads and the page content is finished rendering.
        /// </summary>
        /// <returns></returns>
        private void OnContentRendered(object sender, EventArgs e)
        {
            DependencyObject dependencyObject = (DependencyObject)_button1.Template.FindName("textblock1", (FrameworkElement)_button1);
            _animatedElement = (TextBlock)dependencyObject;
            Window.RegisterName(_animatedElement.Name, _animatedElement);

            if (_inputString == "Clone")
            {
                Storyboard story2 = _story.Clone();
                _beginStory.Storyboard = story2;
                _beginStory.Storyboard.Begin(_animatedElement, _controlTemplate, true);
            }
            else
            {
//THESE WORK TOO!
//animatedElement.BeginStoryboard(story);
//beginStory.Storyboard.Begin(animatedElement);
                _beginStory.Storyboard.Begin(_animatedElement, _controlTemplate, true);

                if (_inputString == "SetSpeedRatio")
                {
                    _beginStory.Storyboard.SetSpeedRatio(_animatedElement, 2d);
                }
            }
            
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,500);
            _aTimer.Start();
            GlobalLog.LogStatus("----DispatcherTimer Started----");
        }
        
        /******************************************************************************
        * Function:          OnTick
        ******************************************************************************/
        /// <summary>
        /// Invoked every time the DispatcherTimer ticks. Used to control the timing of verification.
        /// </summary>
        /// <returns></returns>
        private void OnTick(object sender, EventArgs e)          
        {
            _tickCount++;
            
            if (_tickCount == 1)
            {
                switch (_inputString)
                {
                    case "Seek" :
                        _beginStory.Storyboard.Seek(_animatedElement, TimeSpan.FromMilliseconds(0),TimeSeekOrigin.BeginTime);
                        break;
                    case "SeekAlignedToLastTick" :
                        _beginStory.Storyboard.Seek(_animatedElement, TimeSpan.FromMilliseconds(0),TimeSeekOrigin.BeginTime);
                        break;
                    case "Pause" :
                        _beginStory.Storyboard.Pause(_animatedElement);
                        break;
                    case "PauseResume" :
                        _beginStory.Storyboard.Pause(_animatedElement);
                        break;
                    case "Stop" :
                        _beginStory.Storyboard.Stop(_animatedElement);
                        break;
                    case "SkipToFill" :
                        _beginStory.Storyboard.SkipToFill(_animatedElement);
                        break;
                    case "Remove" :
                        _beginStory.Storyboard.Remove(_animatedElement);
                        break;
                }
            }
            if (_tickCount == 2)
            {
                switch (_inputString)
                {
                    case "PauseResume" :
                        _beginStory.Storyboard.Resume(_animatedElement);
                        break;
                    case "SetSpeedRatio" :
                        //The animation should be finished at this point.
                        Thickness thickness = (Thickness)_animatedElement.GetValue(TextBlock.PaddingProperty);
                        _actSpeedRatioMid = thickness.Left;
                        break;
                }
            }
            else if (_tickCount == 3)
            {
                Signal("AnimationDone", TestResult.Pass);
                _aTimer.Stop();
            }
        }

        /******************************************************************************
        * Function:          Verify
        ******************************************************************************/
        /// <summary>
        /// Verifies the result of the test case, and returns a Pass/Fail result.
        /// </summary>
        /// <returns>TestResult</returns>
        TestResult Verify()
        {
            WaitForSignal("AnimationDone");
            
            bool    testPassed  = false;
            double  expValue    = 0d;
            double  tolerance   = 0.5d;
            
            GlobalLog.LogStatus("-----Verifying the Animation-----");

            Thickness actThickness = (Thickness)_animatedElement.GetValue(TextBlock.PaddingProperty);
            double actValue = actThickness.Left;
            
            switch (_inputString)
            {
                case "Begin" :
                    expValue = _toValue;
                    testPassed = (actValue <= expValue+tolerance && actValue >= expValue-tolerance);
                    GlobalLog.LogEvidence("Expected Value [Final]: " + expValue);
                    break;
                case "Pause" :
                    expValue = _toValue * 0.33;
                    testPassed = (actValue <= expValue+tolerance && actValue >= expValue-tolerance);
                    GlobalLog.LogEvidence("Expected Value [Final]: ~" + expValue);
                    break;
                case "PauseResume" :
                    tolerance = 2.0;
                    expValue = _toValue * 0.67;
                    testPassed = (actValue <= expValue+tolerance && actValue >= expValue-tolerance);
                    GlobalLog.LogEvidence("Expected Value [Final]: ~" + expValue);
                    break;
                case "Seek" :
                    tolerance = 2.0;
                    expValue = _toValue * 0.67;
                    testPassed = (actValue <= expValue+tolerance && actValue >= expValue-tolerance);
                    GlobalLog.LogEvidence("Expected Value [Final]: ~" + expValue);
                    break;
                case "SeekAlignedToLastTick" :
                    tolerance = 2.0;
                    expValue = _toValue * 0.67;
                    testPassed = (actValue <= expValue+tolerance && actValue >= expValue-tolerance);
                    GlobalLog.LogEvidence("Expected Value [Final]: ~" + expValue);
                    break;
                case "Stop" :
                    expValue = 0d;
                    testPassed = (actValue == expValue);
                    GlobalLog.LogEvidence("Expected Value [Final]: " + expValue);
                    break;
                case "SkipToFill" :
                    expValue = _toValue;
                    testPassed = (actValue == expValue);
                    GlobalLog.LogEvidence("Expected Value [Final]: " + expValue);
                    break;
                case "SetSpeedRatio" :
                    expValue = _toValue;
                    testPassed = (actValue == expValue && _actSpeedRatioMid == expValue);
                    GlobalLog.LogEvidence("Expected Value [Mid]  : " + expValue);
                    GlobalLog.LogEvidence("Actual Value   [Mid]  : " + _actSpeedRatioMid);
                    GlobalLog.LogEvidence("Expected Value [Final]: " + expValue);
                    break;
                case "Remove" :
                    expValue = 0d;
                    testPassed = (actValue == expValue);
                    GlobalLog.LogEvidence("Expected Value [Final]: " + expValue);
                    break;
                case "Clone" :
                    expValue = _toValue;
                    testPassed = (actValue <= expValue+tolerance && actValue >= expValue-tolerance);
                    GlobalLog.LogEvidence("Expected Value [Final]: " + expValue);
                    break;
            }
            
            GlobalLog.LogEvidence("Actual Value   [Final]: " + actValue);
            
            if (testPassed)
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
