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
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.PropertyMethodEvent.Regressions</area>

    [Test(2, "Animation.PropertyMethodEvent.Regressions", "NullGetCurrentValueArgsTest")]
    public class NullGetCurrentValueArgsTest : WindowTest
    {

        #region Test case members
        
        private string                      _inputString     = "";
        private CharAnimationUsingKeyFrames _animation;
        private Clock                       _clock1;
        
        #endregion


        #region Constructor
        
        [Variation("NullDefaultOriginValue")]
        [Variation("NullDefaultDestinationValue")]
        [Variation("NullAnimationClock")]

        /******************************************************************************
        * Function:          NullGetCurrentValueArgsTest Constructor
        ******************************************************************************/
        public NullGetCurrentValueArgsTest(string testValue)
        {
            _inputString = testValue;
            InitializeSteps += new TestStep(CreateKeyFrameAnimation);
            RunSteps += new TestStep(CallGetCurrentValue);
        }

        #endregion


        #region Test Steps
        
        /******************************************************************************
        * Function:          CreateKeyFrameAnimation
        ******************************************************************************/
        /// <summary>
        /// Creates a CharAnimationUsingKeyFrames, and a corresponding Clock.
        /// </summary>
        /// <returns>A TestResult</returns>
        private TestResult CreateKeyFrameAnimation()
        {
            _animation = new CharAnimationUsingKeyFrames();
            _animation.BeginTime = TimeSpan.FromMilliseconds(0);
            _animation.Duration  = new Duration(TimeSpan.FromMilliseconds(5000));

            CharKeyFrameCollection KFC = new CharKeyFrameCollection();
            KFC.Add( new DiscreteCharKeyFrame('a', KeyTime.FromPercent(.50)) );
            KFC.Add( new DiscreteCharKeyFrame('a', KeyTime.FromPercent(.75)) );

            _animation.KeyFrames = KFC;

            _clock1 = _animation.CreateClock();
            
            return TestResult.Pass;
        }
        
        /******************************************************************************
        * Function:          CallGetCurrentValue
        ******************************************************************************/
        /// <summary>
        /// Passes null to one of the arguments to GetCurrentValue.
        /// </summary>
        /// <returns>A TestResult</returns>
        private TestResult CallGetCurrentValue()
        {
            SetExpectedErrorTypeInStep(typeof(System.ArgumentNullException), "Outer");                
            switch (_inputString)
            {
                case "NullDefaultOriginValue":
                    _animation.GetCurrentValue(null, 'a', (AnimationClock)_clock1);
                    break;
                case "NullDefaultDestinationValue":
                    _animation.GetCurrentValue('a', null, (AnimationClock)_clock1);
                    break;
                case "NullAnimationClock":
                    _animation.GetCurrentValue('a', 'a', null);
                    break;
            }
            
            return TestResult.Pass;
        }

        #endregion
    }
}
