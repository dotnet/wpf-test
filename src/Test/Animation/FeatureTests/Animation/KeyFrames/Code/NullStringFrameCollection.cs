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
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;   //UserInput
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.KeyFrames.Regressions</area>

    [Test(2, "Animation.KeyFrames.Regressions", "NullStringFrameCollectionTest")]
    public class NullStringFrameCollectionTest : WindowTest
    {
        #region Test case members

        private TextBlock           _textblock;
        private DispatcherTimer     _aTimer              = null;
        private int                 _tickCount           = 0;
        
        #endregion


        #region Constructor
        /******************************************************************************
        * Function:          NullStringFrameCollectionTest Constructor
        ******************************************************************************/
        public NullStringFrameCollectionTest()
        {
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
        /// <returns>Returns success if the element was found</returns>
        TestResult CreateTree()
        {
            NameScope.SetNameScope(Window, new NameScope());

            Window.Width        = 300d;
            Window.Height       = 300d;
            Window.Left         = 0d;
            Window.Top          = 0d;
            Window.ContentRendered     += new EventHandler(OnContentRendered);

            Canvas body = new Canvas();
            Window.Content = body;
            body.Background = Brushes.CadetBlue;

            _textblock = new TextBlock();
            body.Children.Add(_textblock);
            _textblock.Width             = 100;
            _textblock.Height            = 100;
            _textblock.Background        = Brushes.CornflowerBlue;
            _textblock.Foreground        = Brushes.Azure;
            _textblock.Text              = "Avalon!";

            StringAnimationUsingKeyFrames stringAnimation = new StringAnimationUsingKeyFrames();  
            stringAnimation.RepeatBehavior  = RepeatBehavior.Forever; 
            stringAnimation.Duration        = new Duration(TimeSpan.FromSeconds(1.0));

            BeginStoryboard  beginStory = AnimationUtilities.CreateBeginStoryboard(Window, "beginStory");
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(stringAnimation);
            Storyboard.SetTargetProperty(stringAnimation, new PropertyPath(TextBlock.TextProperty));
            beginStory.Storyboard = storyboard;

            EventTrigger mouseEnterTrigger = AnimationUtilities.CreateEventTrigger(TextBlock.MouseEnterEvent, beginStory);
            
            _textblock.Triggers.Add(mouseEnterTrigger);
            
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
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,2000);
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
            GlobalLog.LogStatus("----Tick# " + _tickCount);
            
            if (_tickCount == 1)
            {
                UserInput.MouseMove(_textblock,20,20);
            }
            else
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
        /// <returns></returns>
        TestResult Verify()
        {
            WaitForSignal("AnimationDone");

            string actValue = (string)_textblock.Text;
            string expValue = "Avalon!";
            
            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("Expected Value: " + expValue);
            GlobalLog.LogEvidence("Actual Value:   " + actValue);
            
            if (actValue == expValue)
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
