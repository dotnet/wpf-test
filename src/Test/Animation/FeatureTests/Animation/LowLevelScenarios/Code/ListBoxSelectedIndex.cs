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
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.LowLevelScenarios.Regressions</area>

    [Test(2, "Animation.LowLevelScenarios.Regressions", "ListBoxSelectedIndexTest")]
    public class ListBoxSelectedIndexTest : WindowTest
    {
        #region Test case members

        private ListBox             _listbox1;
        private Int32Animation      _animInt32;
        private DispatcherTimer     _aTimer              = null;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          ListBoxSelectedIndexTest Constructor
        ******************************************************************************/
        public ListBoxSelectedIndexTest()
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
            Window.Width        = 450d;
            Window.Height       = 450d;
            Window.Left         = 0d;
            Window.Top          = 0d;
            Window.ContentRendered     += new EventHandler(OnContentRendered);

            Canvas body = new Canvas();
            Window.Content = body;
            body.Background = Brushes.Gainsboro;

            _listbox1 = new ListBox();
            body.Children.Add(_listbox1);
            Canvas.SetTop  (_listbox1, 120d);
            Canvas.SetLeft (_listbox1, 120d);          
            _listbox1.Width             = 150d;
            _listbox1.Height            = 250d;
            _listbox1.Background        = new SolidColorBrush(Colors.DarkKhaki);
            _listbox1.FontSize          = 18d;
            
            for (int i = 0; i < 18; i++)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = "Item " + i.ToString();
                _listbox1.Items.Add(item);
            }
            _listbox1.SelectedIndex = 2;

            _animInt32 = new Int32Animation();
            _animInt32.By                    = 4;
            _animInt32.BeginTime             = TimeSpan.FromSeconds(0);
            _animInt32.Duration              = new Duration(TimeSpan.FromMilliseconds(500));
            
            _listbox1.BeginAnimation(ListBox.SelectedIndexProperty, _animInt32);
            
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

            int actValue = (int)_listbox1.GetValue(ListBox.SelectedIndexProperty);
            int expValue = 6;
            
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
