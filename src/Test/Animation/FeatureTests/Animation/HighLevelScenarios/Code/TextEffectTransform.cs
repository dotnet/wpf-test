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
    /// <area>Animation.HighLevelScenarios.Regressions</area>

    [Test(2, "Animation.HighLevelScenarios.Regressions", "TextEffectTransformTest")]
    public class TextEffectTransformTest : WindowTest
    {
        #region Test case members

        private double              _toValue                 = 250d;
        private TextEffect          _animatedSpecialEffect;
        private TranslateTransform  _animatedTransform;
        private DispatcherTimer     _aTimer                  = null;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          TextEffectTransformTest Constructor
        ******************************************************************************/
        public TextEffectTransformTest()
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
            Window.Width        = 300d;
            Window.Height       = 300d;
            Window.Left         = 0d;
            Window.Top          = 0d;
            Window.ContentRendered     += new EventHandler(OnContentRendered);

            NameScope.SetNameScope(Window, new NameScope());

            FlowDocument flowDocument = new FlowDocument();
            flowDocument.Name = "Flow";

            Window.Content = flowDocument;

            Run theText = new Run( 
                "Lorem ipsum dolor sit amet, consectetuer adipiscing elit.");   
                
            _animatedSpecialEffect = new TextEffect();
            _animatedSpecialEffect.Foreground    = Brushes.OrangeRed;
            _animatedSpecialEffect.PositionStart = 0;
            _animatedSpecialEffect.PositionCount = 20;
            
            _animatedTransform =  new TranslateTransform();

            Window.RegisterName("animatedTransform", _animatedTransform);             
            _animatedSpecialEffect.Transform = _animatedTransform;

            theText.TextEffects = new TextEffectCollection();
            theText.TextEffects.Add(_animatedSpecialEffect);

            Paragraph animatedParagraph = new Paragraph(theText);
            animatedParagraph.Background    = Brushes.LightGray;
            animatedParagraph.Padding       = new Thickness(20);
   
            flowDocument.Blocks.Add(animatedParagraph);                           
            Storyboard myStoryboard = new Storyboard();
            
            DoubleAnimation xAnimation = new DoubleAnimation();
            xAnimation.To       = _toValue;
            xAnimation.Duration = TimeSpan.FromSeconds(2);                        
            Storyboard.SetTargetName(xAnimation, "animatedTransform");
            Storyboard.SetTargetProperty(xAnimation, new PropertyPath(TranslateTransform.XProperty));      
            myStoryboard.Children.Add(xAnimation);
            
            DoubleAnimation yAnimation = new DoubleAnimation();
            yAnimation.To       = _toValue;
            yAnimation.Duration = TimeSpan.FromSeconds(2);           
            Storyboard.SetTargetName(yAnimation, "animatedTransform");
            Storyboard.SetTargetProperty(yAnimation, new PropertyPath(TranslateTransform.YProperty));      
            myStoryboard.Children.Add(yAnimation); 
            
            myStoryboard.Begin(flowDocument);
            
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
            _aTimer.Interval = new TimeSpan(0,0,0,0,3000);
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
            Signal("AnimationDone", TestResult.Pass);
            _aTimer.Stop();
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

            double actValue = (double)_animatedTransform.GetValue(TranslateTransform.XProperty);
            
            GlobalLog.LogEvidence("-----Verifying the Animation-----");
            GlobalLog.LogEvidence("Expected Value: " + _toValue);
            GlobalLog.LogEvidence("Actual Value:   " + actValue);
            
            if (actValue == _toValue)
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
