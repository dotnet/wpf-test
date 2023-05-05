// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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
    /// <area>Animation.LowLevelScenarios.Regressions</area>
    /// <priority>0</priority>

    [Test(0, "Animation.LowLevelScenarios.Regressions", "IsCumulativeTest")]
    public class IsCumulativeTest : WindowTest
    {
        #region Test case members

        private ListBoxItem         _animatedElement         = null;
        private double              _actValue1               = 0d;
        private double              _actValue2               = 0d;
        private double              _actValue3               = 0d;
        private DispatcherTimer     _aTimer                  = null;
        private int                 _tickCount               = 0;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          IsCumulativeTest Constructor
        ******************************************************************************/
        public IsCumulativeTest()
        {
            InitializeSteps += new TestStep(CreateTree);
            RunSteps += new TestStep(StartTimer);
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
            Window.Height       = 400d;
            Window.Left         = 0d;
            Window.Top          = 0d;

            Canvas body = new Canvas();
            body.Background = Brushes.Gainsboro;

            ListBox listbox1 = new ListBox();
            body.Children.Add(listbox1);
            Canvas.SetTop  (listbox1, 100d);
            Canvas.SetLeft (listbox1, 100d);          
            listbox1.Width             = 150d;
            listbox1.Height            = 250d;
            listbox1.Background        = new SolidColorBrush(Colors.HotPink);
            
            for (int i = 0; i < 10; i++)
            {
                ListBoxItem item = new ListBoxItem();
                item.FontSize   = 12d;
                item.Content    = "Item " + i.ToString();
                listbox1.Items.Add(item);
            }
            _animatedElement = (ListBoxItem)listbox1.Items[1];
            _animatedElement.Background = Brushes.LightBlue;

            Window.Content = body;

            DoubleAnimation anim = new DoubleAnimation();
            anim.To                     = 18d;
            anim.BeginTime              = TimeSpan.FromSeconds(0);
            anim.Duration               = new Duration(TimeSpan.FromSeconds(1));
            anim.RepeatBehavior         = new RepeatBehavior(3);
            anim.IsCumulative           = true;

            AnimationClock AC = anim.CreateClock();
            _animatedElement.ApplyAnimationClock(ListBoxItem.FontSizeProperty, AC);

            _actValue1 = (double)_animatedElement.GetValue(ListBoxItem.FontSizeProperty);
            
            return TestResult.Pass;
        }
          
        /******************************************************************************
        * Function:          StartTimer
        ******************************************************************************/
        /// <summary>
        /// Fires when the page loads and the page content is finished rendering.
        /// </summary>
        /// <returns></returns>
        TestResult StartTimer()
        {
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = TimeSpan.FromSeconds(1);
            _aTimer.Start();
            GlobalLog.LogStatus("----DispatcherTimer Started----");
            
            return TestResult.Pass;
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

            switch (_tickCount)
            {
                case 1:
                    _actValue2 = (double)_animatedElement.GetValue(ListBoxItem.FontSizeProperty);
                    break;
                case 4:
                    _aTimer.Stop();
                    Signal("AnimationDone", TestResult.Pass);
                    break;
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

            double tolerance = 1d;

            _actValue3 = (double)_animatedElement.GetValue(ListBoxItem.FontSizeProperty);

            double expValue1 = 12d;
            double expValue2 = 18d;
            double expValue3 = 30d;

            bool b1 = (_actValue1 <= expValue1+tolerance && _actValue1 >= expValue1-tolerance);
            bool b2 = (_actValue2 <= expValue2+tolerance && _actValue2 >= expValue2-tolerance);
            bool b3 = (_actValue3 <= expValue3+tolerance && _actValue3 >= expValue3-tolerance);

            GlobalLog.LogEvidence("------------RESULTS------------");
            GlobalLog.LogEvidence("Act Value 1:   " + _actValue1);
            GlobalLog.LogEvidence("Exp Value 1:   " + expValue1);
            GlobalLog.LogEvidence("Act Value 2:   " + _actValue2);
            GlobalLog.LogEvidence("Exp Value 2:   " + expValue2);
            GlobalLog.LogEvidence("Act Value 3:   " + _actValue3);
            GlobalLog.LogEvidence("Exp Value 3:   " + expValue3);
            GlobalLog.LogEvidence("-------------------------------");
            GlobalLog.LogEvidence("Comparisons: " + b1 + "/" + b2 + "/" + b3);
            GlobalLog.LogEvidence("-------------------------------");

            if ( b1 && b2 && b3)
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
