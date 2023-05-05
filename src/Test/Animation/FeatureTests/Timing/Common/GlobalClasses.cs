// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:   This is the common routine used by all Timing BVTs and many of the Pri 1
 * cases.  To run these cases, you must use a "dot" after the class name of the particular
 * file that is being loaded, e.g., TimeNode Acceleration .
 * To automatically produce the .txt file that contains the expected results, do the following:
 * TimeNode Acceleration 0 NameOfExpectedFile.txt
 *
 * Dependencies:        TestRuntime.dll
 */
using System;
using System.IO;
using System.Text;
using System.Globalization;
using MethodInfo = System.Reflection.MethodInfo;
using System.Threading;
using System.Reflection;
using ArrayList = System.Collections.ArrayList;

using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Text.RegularExpressions;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;


namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Timing</area>
    /// <priority>1</priority>
    /// <description>
    /// Timing test Framework.
    /// </description>
    /// </summary>
    [Test(2, "Timing", "TimingTest")]
          
    /******************************************************************************
    *******************************************************************************
    * CLASS:          TimingTest
    *******************************************************************************
    ******************************************************************************/
    public class TimingTest : AvalonTest
    {
        #region Test case members

        private bool            _result          = true;
        private string          _failMessage     = null;
        private string          _className       = null;
        private string          _testPurpose     = null;
        private string          _expFileName     = null;
        
        #endregion


        #region Constructor

        //Clock.
        [Variation(@"FeatureTests\Animation\ActiveExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ActiveExpect.txt")]
        [Variation(@"FeatureTests\Animation\ActiveInLoopExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ActiveInLoopExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\ChangedEventExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ChangedEventExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ClockEventsExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ClockEventsExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ClockEventsDetachAfterExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ClockEventsDetachAfterExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\ClockEventsDetachBeforeExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ClockEventsDetachBeforeExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\ClockPausedResumedExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ClockPausedResumedExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\CurrentGlobalSpeedExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\CurrentGlobalSpeedExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\CurrentIterationExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\CurrentIterationExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\CurrentStateExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\CurrentStateExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\CurrentTimeExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\CurrentTimeExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\CurrentTimeFillExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\CurrentTimeFillExpect.txt")]
        [Variation(@"FeatureTests\Animation\EventsPauseResumeExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\EventsPauseResumeExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\IsPausedExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\IsPausedExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\MultipleClocksExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\MultipleClocksExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\NaturalDurationExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\NaturalDurationExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ParentExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ParentExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ProgressExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ProgressExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ProgressInLoopExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ProgressInLoopExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\ProgressWithRepeatExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ProgressWithRepeatExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\RemoveClockExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\RemoveClockExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\TimelineGetExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\TimelineGetExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\TimeNodeCurrentTimeExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\TimeNodeCurrentTimeExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\TLCDefaultsExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\TLCDefaultsExpect.txt", Priority = 1)]

        //ClockCollection.
        [Variation(@"FeatureTests\Animation\ContainsTLCCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ContainsTLCCExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\CopyToTLCCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\CopyToTLCCExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\CountTLCCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\CountTLCCExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\DefaultsTLCCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\DefaultsTLCCExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\EqualsCollectionTLCCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\EqualsCollectionTLCCExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\EqualsObjectTLCCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\EqualsObjectTLCCExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\EqualsTLCCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\EqualsTLCCExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\GetHashCodeTLCCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\GetHashCodeTLCCExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\GetItemTLCCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\GetItemTLCCExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\InequalityTLCCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\InequalityTLCCExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\IsReadOnlyTLCCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\IsReadOnlyTLCCExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\MethodsTLCCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\MethodsTLCCExpect.txt", Priority = 1)]

        //ClockController.
        [Variation(@"FeatureTests\Animation\BeginExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\BeginExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\BeginInExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\BeginInExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICBeginInSpeedExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICBeginInSpeedExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\ICBeginInSpeed2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICBeginInSpeed2Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICBeginInStateExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICBeginInStateExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\ICBeginInState2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICBeginInState2Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICBeginInTimeExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICBeginInTimeExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\ICBeginInTime2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICBeginInTime2Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICClockExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICClockExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICCompleteExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICCompleteExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICDefaultsExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICDefaultsExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\ICOnChildExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICOnChildExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\ICPauseBeginResumeExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICPauseBeginResumeExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICPauseResumeBeginExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICPauseResumeBeginExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICPauseSeekAlignedResumeExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICPauseSeekAlignedResumeExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICPauseSeekResumeExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICPauseSeekResumeExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICPauseSkipToFillExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICPauseSkipToFillExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICPauseSkipToFillResumeExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICPauseSkipToFillResumeExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICPauseWhenFillingExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICPauseWhenFillingExpect.txt")]
        [Variation(@"FeatureTests\Animation\ICRemoveExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICRemoveExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSeekExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSeekAligned1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekAligned1Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSeekAligned2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekAligned2Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSeekAligned3Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekAligned3Expect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\ICSeekAlignedPastDuration1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekAlignedPastDuration1Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSeekAlignedPastDuration3Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekAlignedPastDuration3Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSeekAlignedPastDuration4Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekAlignedPastDuration4Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSeekAlignedPastDuration5Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekAlignedPastDuration5Expect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\ICSeekAlignedPastDuration6Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekAlignedPastDuration6Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSeekAlignedPastDuration7Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekAlignedPastDuration7Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSeekAlignedPastDuration8Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekAlignedPastDuration8Expect.txt", Priority = 3)]
        [Variation(@"FeatureTests\Animation\ICSeekAlignedPastDuration9Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekAlignedPastDuration9Expect.txt", Priority = 3)]
        [Variation(@"FeatureTests\Animation\ICSeekBeforeBeginExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekBeforeBeginExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSeekDurationExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekDurationExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSeekDurationMinusExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekDurationMinusExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSeekInSpeedExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekInSpeedExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSeekInStateExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekInStateExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSeekInTimeExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekInTimeExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSeekMinusExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekMinusExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSeekNoDuration1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekNoDuration1Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSeekNoDuration2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekNoDuration2Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSeekParent1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekParent1Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSeekPastDuration1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekPastDuration1Expect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\ICSeekPastDuration3Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekPastDuration3Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSeekPastDuration4Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekPastDuration4Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSeekPastDuration5Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekPastDuration5Expect.txt", Priority = 3)]
        [Variation(@"FeatureTests\Animation\ICSeekPastDuration6Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekPastDuration6Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSeekPastDuration7Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekPastDuration7Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSeekPastDuration8Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekPastDuration8Expect.txt", Priority = 3)]
        [Variation(@"FeatureTests\Animation\ICSeekPastDuration9Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekPastDuration9Expect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\ICSeekPastDuration11Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekPastDuration11Expect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\ICSeekPastDuration12Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekPastDuration12Expect.txt", Priority = 3)]
        [Variation(@"FeatureTests\Animation\ICSeekPastDuration13Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekPastDuration13Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSeekPastDuration14Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekPastDuration14Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSeekPastDuration15Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekPastDuration15Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSeekPastDuration16Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekPastDuration16Expect.txt", Priority = 3)]
        [Variation(@"FeatureTests\Animation\ICSeekPastDuration17Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekPastDuration17Expect.txt", Priority = 3)]
        [Variation(@"FeatureTests\Animation\ICSeekWhenAutomaticExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSeekWhenAutomaticExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSkipToFillExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSkipToFillExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSkipToFillInSpeedExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSkipToFillInSpeedExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSkipToFillInStateExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSkipToFillInStateExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSkipToFillInTimeExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSkipToFillInTimeExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSkipToFillStopExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSkipToFillStopExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICSkipToFillWhenAutomaticExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSkipToFillWhenAutomaticExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\ICSpeedRatioExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICSpeedRatioExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICStopExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICStopExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICStopInSpeedExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICStopInSpeedExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICStopInStateExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICStopInStateExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICStopInTimeExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICStopInTimeExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ICStopStartExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ICStopStartExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\PauseResumeExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\PauseResumeExpect.txt", Priority = 0)]

        //Code Coverage.
        [Variation(@"FeatureTests\Animation\CCContainerStopExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\CCContainerStopExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ClockGpTLExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ClockGpTLExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ContainerSkipToFillExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ContainerSkipToFillExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\DurationEqualExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\DurationEqualExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\DurationGetHashCodeExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\DurationGetHashCodeExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\DurationOperatorsExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\DurationOperatorsExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\RemoveAt_ZeroCountExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\RemoveAt_ZeroCountExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\RepeatBehaviorEqualExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\RepeatBehaviorEqualExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\RepeatBehaviorGetHashCodeExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\RepeatBehaviorGetHashCodeExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\Thrown_DurationExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\Thrown_DurationExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\Thrown_TimelineAddChildExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\Thrown_TimelineAddChildExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\Thrown_TimelineAddTextExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\Thrown_TimelineAddTextExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\Thrown_TLCRemoveAtExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\Thrown_TLCRemoveAtExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\Thrown_TLCThisExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\Thrown_TLCThisExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\TimelineAddChildExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\TimelineAddChildExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\TimelineAddTextExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\TimelineAddTextExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\TLCAddExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\TLCAddExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\TLCContainsExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\TLCContainsExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\TLCIndexOfExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\TLCIndexOfExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\TLCInsertExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\TLCInsertExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\TLCRemoveExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\TLCRemoveExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\TLCThisExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\TLCThisExpect.txt", Priority = 2)]

        //Duration.
        [Variation(@"FeatureTests\Animation\DurationDefaultsExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\DurationDefaultsExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\DurationForeverAutoExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\DurationForeverAutoExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\DurationMethodsExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\DurationMethodsExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\DurationPropertiesExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\DurationPropertiesExpect.txt", Priority = 0)]

        //FillBehavior.
        [Variation(@"FeatureTests\Animation\DeactivateExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\DeactivateExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\FBDefaultsExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\FBDefaultsExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\HoldEndExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\HoldEndExpect.txt", Priority = 0)]

        //ParallelTimeline.
        [Variation(@"FeatureTests\Animation\CloneCurrentValuePTLExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\CloneCurrentValuePTLExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ConstructorPTL1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ConstructorPTL1Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ConstructorPTL2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ConstructorPTL2Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ConstructorPTL3Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ConstructorPTL3Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ConstructorPTL4Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ConstructorPTL4Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\CopyPTLExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\CopyPTLExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\SBDefaultsExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBDefaultsExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\SBSlipBehaviorExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBSlipBehaviorExpect.txt", Priority = 0)]

        //Regressions.
        [Variation(@"FeatureTests\Animation\bug1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug1Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug2Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug3Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug3Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug4Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug4Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug5Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug5Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug6Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug6Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug7Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug7Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug8Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug8Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug9Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug9Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug10Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug10Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug11Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug11Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug12Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug12Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug14Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug14Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug15Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug15Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug16Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug16Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug17Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug17Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug18Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug18Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug19Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug19Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug20Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug20Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug21Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug21Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug22Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug22Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug23Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug23Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug24Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug24Expect.txt", Priority = 3)]
        [Variation(@"FeatureTests\Animation\bug25Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug25Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug26Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug26Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug27Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug27Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug28Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug28Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug29Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug29Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug30Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug30Expect.txt", Priority = 3)]
        [Variation(@"FeatureTests\Animation\bug31Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug31Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug32Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug32Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug33Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug33Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug34Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug34Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug35Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug35Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug36Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug36Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug37Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug37Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug38Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug38Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug39Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug39Expect.txt", Disabled = true)]
        [Variation(@"FeatureTests\Animation\bug40Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug40Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug41Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug41Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug42Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug42Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug43Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug43Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug44Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug44Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug45Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug45Expect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\bug46Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug46Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug47Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug47Expect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\bug48Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug48Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug49Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug49Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug50Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug50Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug51Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug51Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug52Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug52Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug53Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug53Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug54Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug54Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug55Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug55Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug56Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug56Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug57Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug57Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug58Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug58Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug59Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug59Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug60Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug60Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug61Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug61Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug62Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug62Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug63Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug63Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug64Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug64Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug65Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug65Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug66Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug66Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug67Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug67Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug68Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug68Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug69Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug69Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug70Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug70Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug71Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug71Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug72Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug72Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug73Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug73Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug74Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug74Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug75Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug75Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug76Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug76Expect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\bug77Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug77Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug78Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug78Expect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\bug79Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug79Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug80Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug80Expect.txt", Disabled = true)]
        [Variation(@"FeatureTests\Animation\bug81Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug81Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug82Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug82Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug83Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug83Expect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\bug84Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug84Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug85Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug85Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug86Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug86Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug87Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug87Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug88Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug88Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug89Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug89Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug90Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug90Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug91Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug91Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug92Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug92Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug93Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug93Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug94Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug94Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug95Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug95Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug96Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug96Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug97Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug97Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug98Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug98Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug99Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug99Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug100Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug100Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug101Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug101Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug102Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug102Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug103Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug103Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug104Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug104Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug105Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug105Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug106Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug106Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug107Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug107Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug108Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug108Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug109Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug109Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug110Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug110Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug111Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug111Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug112Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug112Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug113Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug113Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug114Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug114Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug115Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug115Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug116Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug116Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug117Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug117Expect.txt", Disabled = true)]
        [Variation(@"FeatureTests\Animation\bug118Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug118Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug119Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug119Expect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\bug120Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug120Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug121Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug121Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug122Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug122Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug123Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug123Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug124Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug124Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug125Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug125Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug126Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug126Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug127Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug127Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug128Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug128Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug129Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug129Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug130Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug130Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug131Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug131Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug132Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug132Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug133Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug133Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug134Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug134Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug135Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug135Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug136Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug136Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug137Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug137Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug138Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug138Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug139Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug139Expect.txt")]
        [Variation(@"FeatureTests\Animation\bug140Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug140Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\bug141Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\bug141Expect.txt", Priority = 0)]

        //RepeatBehavior.
        [Variation(@"FeatureTests\Animation\IterationCountExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\IterationCountExpect.txt")]
        [Variation(@"FeatureTests\Animation\IterationCountZeroExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\IterationCountZeroExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\RBConverterExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\RBConverterExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\RBDefaultsExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\RBDefaultsExpect.txt")]
        [Variation(@"FeatureTests\Animation\RBEqualsExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\RBEqualsExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\RBEquals2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\RBEquals2Expect.txt")]
        [Variation(@"FeatureTests\Animation\RBEqualsObjectExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\RBEqualsObjectExpect.txt")]
        [Variation(@"FeatureTests\Animation\RBException1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\RBException1Expect.txt")]
        [Variation(@"FeatureTests\Animation\RBException2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\RBException2Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\RBException3Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\RBException3Expect.txt")]
        [Variation(@"FeatureTests\Animation\RBForeverExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\RBForeverExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\RBGetHashCodeExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\RBGetHashCodeExpect.txt")]
        [Variation(@"FeatureTests\Animation\RBLessThanOneExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\RBLessThanOneExpect.txt")]
        [Variation(@"FeatureTests\Animation\RBNoDurationExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\RBNoDurationExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\RBOperatorsExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\RBOperatorsExpect.txt")]
        [Variation(@"FeatureTests\Animation\RBToStringExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\RBToStringExpect.txt")]
        [Variation(@"FeatureTests\Animation\RBToString2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\RBToString2Expect.txt")]
        [Variation(@"FeatureTests\Animation\RepeatBehaviorOneExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\RepeatBehaviorOneExpect.txt")]
        [Variation(@"FeatureTests\Animation\RepeatDurationBothExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\RepeatDurationBothExpect.txt")]
        [Variation(@"FeatureTests\Animation\RepeatDurationSmallerExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\RepeatDurationSmallerExpect.txt")]

        //SlipBehavior.
        [Variation(@"FeatureTests\Animation\SBBegin1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBBegin1Expect.txt")]
        [Variation(@"FeatureTests\Animation\SBBegin2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBBegin2Expect.txt")]
        [Variation(@"FeatureTests\Animation\SBBegin3Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBBegin3Expect.txt")]
        [Variation(@"FeatureTests\Animation\SBBeginTimeChanged1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBBeginTimeChanged1Expect.txt")]
        [Variation(@"FeatureTests\Animation\SBClip1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBClip1Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SBClip2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBClip2Expect.txt")]
        [Variation(@"FeatureTests\Animation\SBDFR1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBDFR1Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SBDurationAuto1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBDurationAuto1Expect.txt")]
        [Variation(@"FeatureTests\Animation\SBDurationAuto2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBDurationAuto2Expect.txt")]
        [Variation(@"FeatureTests\Animation\SBFillStop1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBFillStop1Expect.txt")]
        [Variation(@"FeatureTests\Animation\SBGrow1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBGrow1Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SBGrow2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBGrow2Expect.txt")]
        [Variation(@"FeatureTests\Animation\SBMethodsGrow1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBMethodsGrow1Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SBMethodsGrow2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBMethodsGrow2Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SBMethodsGrow3Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBMethodsGrow3Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SBMethodsGrow4Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBMethodsGrow4Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SBMethodsSlip1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBMethodsSlip1Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SBMethodsSlip2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBMethodsSlip2Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SBMethodsSlip3Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBMethodsSlip3Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SBMethodsSlip4Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBMethodsSlip4Expect.txt", Priority = 0)]

        //////////////////////////////////////////////////////////////////////////////////////////////////
        // DISABLEDUNSTABLETEST:
        // TestName:TimingTest(FeatureTests\\Animation\\SBNegativeBegin1Expect.txt,R,x)
        // Area: Animation   SubArea: Timing
        // Disable this case due to high fail rate, will enable after fix it.
        // to find all disabled tests in test tree, use: �findstr /snip DISABLEDUNSTABLETEST� 
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        //[Variation(@"FeatureTests\Animation\SBNegativeBegin1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBNegativeBegin1Expect.txt", Priority = 0)]
        
        [Variation(@"FeatureTests\Animation\SBNegativeBegin2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBNegativeBegin2Expect.txt")]
        [Variation(@"FeatureTests\Animation\SBPause1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBPause1Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SBPauseResume1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBPauseResume1Expect.txt")]
        [Variation(@"FeatureTests\Animation\SBPauseResume2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBPauseResume2Expect.txt")]
        [Variation(@"FeatureTests\Animation\SBPauseResume3Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBPauseResume3Expect.txt")]
        [Variation(@"FeatureTests\Animation\SBPauseResume4Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBPauseResume4Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SBPauseResume5Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBPauseResume5Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SBPauseSeekGrowExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBPauseSeekGrowExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SBPauseSeekSlipExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBPauseSeekSlipExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SBRemove1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBRemove1Expect.txt")]
        [Variation(@"FeatureTests\Animation\SBRemove2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBRemove2Expect.txt")]
        [Variation(@"FeatureTests\Animation\SBRepeatGrow1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBRepeatGrow1Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SBRepeatGrow2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBRepeatGrow2Expect.txt")]
        [Variation(@"FeatureTests\Animation\SBRepeatGrow3Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBRepeatGrow3Expect.txt")]
        [Variation(@"FeatureTests\Animation\SBRepeatGrow4Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBRepeatGrow4Expect.txt", Priority = 0)]
        //Product         [Variation(@"FeatureTests\Animation\SBRepeatGrow5Expect.txt", "R", "x", SupportFiles=@"FeatureTests\Animation\SBRepeatGrow5Expect.txt")]
        [Variation(@"FeatureTests\Animation\SBRepeatGrow6Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBRepeatGrow6Expect.txt")]
        [Variation(@"FeatureTests\Animation\SBRepeatGrow7Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBRepeatGrow7Expect.txt")]
        [Variation(@"FeatureTests\Animation\SBRepeatGrow8Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBRepeatGrow8Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SBRepeatSlip1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBRepeatSlip1Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SBRepeatSlip2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBRepeatSlip2Expect.txt", Priority = 0)]
        //Product       [Variation(@"FeatureTests\Animation\SBRepeatSlip3Expect.txt", "R", "x", SupportFiles=@"FeatureTests\Animation\SBRepeatSlip3Expect.txt")]
        [Variation(@"FeatureTests\Animation\SBRepeatSlip4Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBRepeatSlip4Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SBSeek1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBSeek1Expect.txt")]
        [Variation(@"FeatureTests\Animation\SBSeek2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBSeek2Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SBSeekAligned1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBSeekAligned1Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SBSeekAligned2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBSeekAligned2Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SBSeekBackIntoExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBSeekBackIntoExpect.txt")]
        [Variation(@"FeatureTests\Animation\SBSeekBeforeExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBSeekBeforeExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SBSeekNoDurationExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBSeekNoDurationExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SBSkipToFill1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBSkipToFill1Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SBSkipToFill2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBSkipToFill2Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SBSlip1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBSlip1Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SBSlip2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBSlip2Expect.txt")]
        [Variation(@"FeatureTests\Animation\SBSlipAtZeroExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBSlipAtZeroExpect.txt")]
        [Variation(@"FeatureTests\Animation\SBSpeedRatioGrow1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBSpeedRatioGrow1Expect.txt")]
        [Variation(@"FeatureTests\Animation\SBSpeedRatioGrow2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBSpeedRatioGrow2Expect.txt")]
        [Variation(@"FeatureTests\Animation\SBSpeedRatioSlip1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBSpeedRatioSlip1Expect.txt")]
        [Variation(@"FeatureTests\Animation\SBSpeedRatioSlip2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBSpeedRatioSlip2Expect.txt")]
        [Variation(@"FeatureTests\Animation\SBStop1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBStop1Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SBStop2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBStop2Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SBStop3Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SBStop3Expect.txt", Priority = 0)]

        //TimeContainer.
        [Variation(@"FeatureTests\Animation\BeginInTCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\BeginInTCExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ChildEventsExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ChildEventsExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ChildEventsHoldEndExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ChildEventsHoldEndExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ChildEventsStopExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ChildEventsStopExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ChildFillBehaviorExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ChildFillBehaviorExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ChildFrozenExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ChildFrozenExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\ContainerBeginDurationExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ContainerBeginDurationExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ContainerFillExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ContainerFillExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ContainerFillHoldExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ContainerFillHoldExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ContainerFillHoldChildExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ContainerFillHoldChildExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ContainerReverseRepeatExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ContainerReverseRepeatExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ContainerStopExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ContainerStopExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\PauseResumeTCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\PauseResumeTCExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\TimeContainerReverseExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\TimeContainerReverseExpect.txt", Priority = 0)]

        //Timeline.
        [Variation(@"FeatureTests\Animation\AccelerationExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\AccelerationExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\AutoReverseExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\AutoReverseExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\BeginAtExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\BeginAtExpect.txt", Priority = 3)]
        [Variation(@"FeatureTests\Animation\BeginDurationExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\BeginDurationExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\BeginIndefiniteExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\BeginIndefiniteExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\BeginInfiniteExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\BeginInfiniteExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\BeginNoDurationExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\BeginNoDurationExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\BeginNotSpecifiedExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\BeginNotSpecifiedExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ChildrenExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ChildrenExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\Constructor1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\Constructor1Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\Constructor2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\Constructor2Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\Constructor3Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\Constructor3Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\CopyTimelineExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\CopyTimelineExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\CopyTimelineChangeExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\CopyTimelineChangeExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\DecelerationExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\DecelerationExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\DefaultsTLExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\DefaultsTLExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\DFRExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\DFRExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\DFRChildExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\DFRChildExpect.txt", Priority = 0)]
        //Product [Variation(@"FeatureTests\Animation\DFRPauseExpect.txt", "R", "x", SupportFiles=@"FeatureTests\Animation\DFRPauseExpect.txt", Priority=1)]
        [Variation(@"FeatureTests\Animation\DFRPauseResumeExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\DFRPauseResumeExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\DFRReverseExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\DFRReverseExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\DFRSeekExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\DFRSeekExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\DFRSpeed1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\DFRSpeed1Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\DFRSpeed2Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\DFRSpeed2Expect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\DFRStateExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\DFRStateExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\DFRUnsetExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\DFRUnsetExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\DoesAnimateExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\DoesAnimateExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\Duration1Expect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\Duration1Expect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\DurationForeverExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\DurationForeverExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\DurationInLoopExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\DurationInLoopExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\EventsRemoveAfterExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\EventsRemoveAfterExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\EventsRemoveBeforeExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\EventsRemoveBeforeExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\FillHoldExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\FillHoldExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\IDGetExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\IDGetExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\IDInBegunInEndedExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\IDInBegunInEndedExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\RepeatDurationExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\RepeatDurationExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ReversedExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ReversedExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ReverseRepeatExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ReverseRepeatExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SeekExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SeekExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SpeedExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SpeedExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SpeedChangedExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SpeedChangedExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SpeedChangedAccelExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SpeedChangedAccelExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SpeedChangedDecelExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SpeedChangedDecelExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\TimeNodeRepeatExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\TimeNodeRepeatExpect.txt", Priority = 0)]

        //TimelineCollection.
        [Variation(@"FeatureTests\Animation\AddTLCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\AddTLCExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ClearTLCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ClearTLCExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\CloneCurrentValueTLCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\CloneCurrentValueTLCExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\CloneTLCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\CloneTLCExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\Constructor1TLCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\Constructor1TLCExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\Constructor2TLCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\Constructor2TLCExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\Constructor3TLCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\Constructor3TLCExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ContainsFrozenTLCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ContainsFrozenTLCExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\ContainsTLCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ContainsTLCExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\CopyToObjectTLCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\CopyToObjectTLCExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\CopyToTimelineTLCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\CopyToTimelineTLCExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\DefaultsTLCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\DefaultsTLCExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\IndexOfTLCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\IndexOfTLCExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\InsertTLCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\InsertTLCExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\IsFixedSizeTLCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\IsFixedSizeTLCExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\IsReadOnlyTLCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\IsReadOnlyTLCExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\IsSynchronizedTLCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\IsSynchronizedTLCExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\RemoveAtTLCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\RemoveAtTLCExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\RemoveTLCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\RemoveTLCExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SetItemTLCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SetItemTLCExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\SyncRootTLCExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\SyncRootTLCExpect.txt", Priority = 1)]

        //TimelineGroup.
        [Variation(@"FeatureTests\Animation\CloneCurrentValueTLGExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\CloneCurrentValueTLGExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\CopyTLGExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\CopyTLGExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\CreateClockTLGExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\CreateClockTLGExpect.txt", Priority = 0)]

        //TimeManager.
        [Variation(@"FeatureTests\Animation\TMEmptyConstructorExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\TMEmptyConstructorExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\TMPauseResumeExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\TMPauseResumeExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\TMRestartExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\TMRestartExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\TMSeekExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\TMSeekExpect.txt", Priority = 2)]
        [Variation(@"FeatureTests\Animation\TMStopStartExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\TMStopStartExpect.txt", Priority = 0)]

        //TimeNodeEnumerator.
        [Variation(@"FeatureTests\Animation\CurrentContainerTMExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\CurrentContainerTMExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\CurrentNodeTMExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\CurrentNodeTMExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\EqualityTMExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\EqualityTMExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\EqualsTMExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\EqualsTMExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\GetHashCodeTMExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\GetHashCodeTMExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\InequalityTMExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\InequalityTMExpect.txt", Priority = 1)]
        [Variation(@"FeatureTests\Animation\MoveNextMixedTMExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\MoveNextMixedTMExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\MoveNextTMExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\MoveNextTMExpect.txt", Priority = 0)]
        [Variation(@"FeatureTests\Animation\ResetTMExpect.txt", "R", "x", SupportFiles = @"FeatureTests\Animation\ResetTMExpect.txt", Priority = 0)]


        /******************************************************************************
        * Function:          TimingTest Constructor
        ******************************************************************************/
        public TimingTest(string p1, string p2, string p3)
        {
            _className       = p1;   // This input path will be parsed to obtain the actual class name.
            _testPurpose     = p2;   // "R" = run test case; "C" = create new expected results file.
            _expFileName     = p3;   // "x" is a placeholder.

            InitializeSteps += new TestStep(ParseInputString);
            RunSteps += new TestStep(RunTest);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          ParseInputString
        ******************************************************************************/
        /// <summary>
        /// ParseInputString: Retrieves the class name and the expected file name from the first input parameter.
        /// </summary>
        /// <returns></returns>
        TestResult ParseInputString()
        {
            int indx = _className.LastIndexOf("\\");
            if (indx >= 0)
                _className = _className.Remove(0,indx+1);     //Remove the Animation prefix.

            indx = _className.LastIndexOf(".");
            if (indx >= 0)
                _className = _className.Remove(indx);         //Remove the .txt suffix.

            indx = _className.LastIndexOf("Expect");
            if (indx >= 0)
                _className = _className.Remove(indx,6);       //Remove 'Expect'.

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          RunTest
        ******************************************************************************/
        /// <summary>
        /// RunTest: Creates an object given by the Name "className" and calls the "Test" function
        /// on the constructed object. 
        /// </summary>
        /// <returns>A TestResult, indicating Pass or Fail</returns>
        TestResult RunTest()
        {
            try
            {
                // Get the Type of the Testcase Eg: Microsoft.Test.Animation.TimelineClone
                Type typeOfTestCase = Type.GetType("Microsoft.Test.Animation." + _className);
                DEBUG.ASSERT( typeOfTestCase != null , "Error in TestCase : \n Cannot find class " + _className );

                // Create an object of the above Type  ( like new TimelineClone() )
                object instanceOfTestCase = Activator.CreateInstance( typeOfTestCase );
                DEBUG.ASSERT( instanceOfTestCase != null , "Error in TestCase : \n Cannot Create class " + _className );

                // Get Test Method Reference from the Type
                MethodInfo methodTest = typeOfTestCase.GetMethod("Test");
                DEBUG.ASSERT( methodTest != null, "Error in TestCase : \n Cannot find Method \" Test \" in " + _className);

                //-----Run the test-----------------------------------------------------------
                string actualResult = (string) methodTest.Invoke( instanceOfTestCase, null );
                //----------------------------------------------------------------------------

                if ( _testPurpose == "C")
                {
                    //Create a new expected results file.
                    GlobalLog.LogEvidence("**********Creating a new .txt file");
                    string fName = "";
                    if (_expFileName == null)
                    {
                        fName = _className + "Expect.txt";
                    }
                    else
                    {
                        fName = _expFileName;
                    }
                    FileStream outputFile = new FileStream( fName, FileMode.Create );
                    //StreamWriter outputFileWriter = new StreamWriter(outputFile);

                    //TextWriter tWriter = Console.Out;
                    //Console.SetOut( outputFileWriter );
                    //Console.Write( actualResult );
                    //Console.SetOut( tWriter );
                    //outputFileWriter.Close();

                    byte[] info = new UTF8Encoding(true).GetBytes(actualResult);
                    outputFile.Write(info, 0, info.Length);
                    GlobalLog.LogFile(fName, outputFile);

                    _result = true;  //force a pass.
                    Signal("TestFinished", TestResult.Pass);
                }
                else
                {
                    //Use the existing expected results file to verify the test results.
                    string expectedResult = GetExpectedResultFromFile( _className + "Expect.txt" );

                    // Decide Whether Avalon is Shipping !!
                    _result &= CompareOutputs( actualResult , expectedResult );

                    Signal("TestFinished", TestResult.Pass);
                }

            }
            catch (Exception e)
            {
                GlobalLog.LogEvidence(e.ToString());
                _failMessage += "\n" +  e.ToString();
                _result &= false;
                throw new Exception(e.ToString());
            }

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          GetExpectedResultFromFile
        ******************************************************************************/
        /// <summary>
        /// GetExpectedResultFromFile: Opens the file [ supposed to contain Expected Ouput. ]
        /// </summary>
        /// <returns>A string containing File Data</returns>
        private string GetExpectedResultFromFile(string fileName)
        {
            // Never Believe User Input
            try 
            {
                StreamReader fileReader = new StreamReader(fileName);
                return  fileReader.ReadToEnd();
            }
            catch( Exception e )
            {
                throw new Exception( "Error in TestCase: \n" + e.ToString());
            }
        }

        /******************************************************************************
        * Function:          LocalizeProgressValue
        ******************************************************************************/
        private string LocalizeProgressValue( string toChange )
        {
            NumberFormatInfo _numInfo = CultureInfo.CurrentCulture.NumberFormat;
            String localized = toChange.Replace("0.","0"+_numInfo.NumberDecimalSeparator);

            // for exponential progress values, we need to do a regular expression replace of the decimal separator
            localized = Regex.Replace(localized, @"([0-9])\.([0-9]+E)", "$1" + _numInfo.NumberDecimalSeparator + "$2");

            return localized;
        }

        /******************************************************************************
        * Function:          GetProgress
        ******************************************************************************/
        /// <summary>
        /// GetProgress: Parses a string to return a (double) CurrentProgress value.
        /// </summary>
        /// <returns>A double representing the CurrentProgress of the Timeline</returns>
        private double GetProgress( string progressString )
        {


            string[] vals = progressString.Split(' ');
            string progressDelim = "0";

            foreach( string word in vals )
            {
                if ( word.IndexOf( progressDelim ) >= 0 )
                {
                    string w = word.Replace(")","");
                    double progressValue = Double.Parse( w, CultureInfo.CurrentCulture );
                    return progressValue;
                }
            }

            return 1;
        }

        /******************************************************************************
        * Function:          CompareOutputs
        ******************************************************************************/
        /// <summary>
        /// CompareOutputs: Compares the actual vs. expected output.
        /// </summary>
        /// <returns>A bool, indicating whether the actual output matches the expected output</returns>
        private bool CompareOutputs( string actualOutput, string expectOutput )
        {
            string[] aOut = actualOutput.Split(new char[] {'\n'});
            string[] eOut = expectOutput.Split(new char[] {'\n'});
            string processingTime="";
            string failLog=" List of Failures : \n";

            int i = 0;
            bool isPassed = true;
            double threshold = 0.001;

            // Compare Each String
            for(i = 0; i<aOut.Length && i<eOut.Length; i++)
            {
                    if (aOut[i].IndexOf("Processing time") == 0)
                         processingTime=aOut[i];

                    if ( eOut[i].IndexOf("Progress") > 0 )
                    {
                         eOut[i] = LocalizeProgressValue( eOut[i] );

                         double expectedProgress = GetProgress( eOut[i] );
                         double actualProgress = GetProgress( aOut[i] );
                         if ( aOut[i].Trim().Equals(eOut[i].Trim()))
                         {
                              GlobalLog.LogEvidence("PASS : " + eOut[i]);
                              continue;
                         }
                         else if ( Math.Abs( actualProgress - expectedProgress ) < threshold )
                         {
                              GlobalLog.LogEvidence("PASS : " + eOut[i]);
                              continue;
                         }
                         else
                         {
                              GlobalLog.LogEvidence("FAIL :     Actual: " + aOut[i] );
                              GlobalLog.LogEvidence("          Expect: " + eOut[i] );          
                         
                              failLog += processingTime + "\n";
                              failLog += "Actual:     " + aOut[i] + "\n" ;
                              failLog += "Expect:     " + eOut[i] + "\n\n";

                              isPassed = false;
                         }
                    }
                    else if ( aOut[i].Trim().Equals(eOut[i].Trim()))
                    {
                         GlobalLog.LogEvidence("PASS : " + eOut[i]);
                         continue;
                    }
                    else 
                    {
                         GlobalLog.LogEvidence("FAIL :     Actual: " + aOut[i] );
                         GlobalLog.LogEvidence("          Expect: " + eOut[i] );          
                         
                         failLog += processingTime + "\n";
                         failLog += "Actual:     " + aOut[i] + "\n" ;
                         failLog += "Expect:     " + eOut[i] + "\n\n";

                         isPassed = false;
                    };
               }

               // Both Actual and Expected should be same. Otherwise something is wrong.
               for( ; i<aOut.Length ; i++ )
               {
                    if (aOut[i].IndexOf("Processing time") == 0)
                         processingTime=aOut[i];

                    GlobalLog.LogEvidence("FAIL : Actual:     " + aOut[i] );
                    GlobalLog.LogEvidence("       Expect:     " );

                    failLog += processingTime + "\n";
                    failLog += "Actual:     " + aOut[i] + "\n" ;
                    failLog += "Expect:     " + "\n\n";

                    isPassed = false;
               }

               for( ; i<eOut.Length; i++)
               {     
                    if (eOut[i].IndexOf("Processing time") == 0)
                         processingTime=eOut[i];
                 
                    GlobalLog.LogEvidence("FAIL : Actual:     " );
                    GlobalLog.LogEvidence("       Expect:     " + eOut[i] );

                    failLog += processingTime + "\n";
                    failLog += "Actual:     " + "\n" ;
                    failLog += "Expect:     " + eOut[i] + "\n\n";

                    isPassed = false;
               }

               if ( !isPassed )
                    GlobalLog.LogEvidence(failLog);

               return isPassed;
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
            WaitForSignal("TestFinished");
            
            if (_result)
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


    /******************************************************************************
    *******************************************************************************
    * CLASS:          DEBUG
    *******************************************************************************
    ******************************************************************************/
    class DEBUG
    {
        #region DEBUG
        /*
        *     Function:  ASSERT
        *  Arguments: Condition, ErrorMessage
        *  Description: If Condition = false then throws an Exception which is handled by GenericLoader
        *                    
        */
        public static void ASSERT(bool bCondition, string errMsg)
        {
            if (!bCondition)
                throw new Exception(errMsg);
        }

        /*
        *     Function:  ASSERT
        *  Arguments: Condition, ErrorMessage , SuccessMessage
        *  Description: If Condition = false then throws an Exception which is handled by GenericLoader
        *                     If Condition = true then Logs the SuccessMessage
        *                    
        */          
        public static void ASSERT(bool bCondition, string errMsg , string successMessage)
        {
            if (!bCondition)
                throw new Exception(errMsg);
            else
                LOGSTATUS( successMessage );
        }

        /*
        * Function:     LOGSTATUS
        * Arguments:     message
        * Description: Logs the message in the Framework
        */
        public static void LOGSTATUS( string message )
        {
            GlobalLog.LogEvidence( message );
        }
        #endregion
    }

    /******************************************************************************
    *******************************************************************************
    * CLASS:          ITimeBVT
    *******************************************************************************
    ******************************************************************************/
    //This is a global interface class for all Timing test cases.
    public abstract class ITimeBVT
    {
        #region Test case members

        public  string        outString                   = "";
        public  int           iterationCount              = 0;
        public  double        saveSpeed                   = 0;
        public  bool          savePaused                  = false;

        public  static bool   eventsVerify                = false;
        public  static int    tickNumber                  = 0;
        private ArrayList     _begunEvents                 = new ArrayList();
        private ArrayList     _endedEvents                 = new ArrayList();
        private ArrayList     _reversedEvents              = new ArrayList();
        private ArrayList     _repeatedEvents              = new ArrayList();
        private ArrayList     _pausedEvents                = new ArrayList();
        private ArrayList     _resumedEvents               = new ArrayList();
        private ArrayList     _currentTimeEvents           = new ArrayList();
        private ArrayList     _currentGlobalSpeedEvents    = new ArrayList();
        private ArrayList     _completedEvents             = new ArrayList();
        private ArrayList     _removeRequestedEvents       = new ArrayList();

        public TimeSpan CurrentTime
        {
            get
            {
                return _tManager.CurrentTime;
            }

            set
            {
                _tManager.CurrentTime = value;
            }
        }
        private TimeManagerInternal _tManager;
          
          
        public virtual TimeManagerInternal EstablishTimeManager(ITimeBVT currentTest)
        {
            _tManager = new TimeManagerInternal();

            _tManager.Stop();
            CurrentTime = TimeSpan.Zero;

            return _tManager;
        }
        #endregion
          
        #region Test Steps

        //----------------SYNC TIMELINE------------------------------------------------------
        private SyncTimeline _syncTimeline;

        public virtual SyncTimeline EstablishSyncTimeline(ITimeBVT currentTest)
        {
           _syncTimeline = new SyncTimeline();

           return _syncTimeline;
        }

        //Actual values are supplied by each individual test case.
        public static double              slipBegin;
        public static double              slipDuration;
        public static Nullable<TimeSpan>  syncDurationTime = null;
          
          
        //----------------ATTACH TIMELINE HANDLERS-------------------------------------------
        public virtual void AttachCurrentStateInvalidated(Timeline subject)
        {
            subject.CurrentStateInvalidated          += new EventHandler(OnCurrentStateInvalidated);
        }

        public virtual void AttachCurrentGlobalSpeedInvalidated(Timeline subject)
        {
            subject.CurrentGlobalSpeedInvalidated    += new EventHandler(OnCurrentGlobalSpeedInvalidated);
        }

        public virtual void AttachCurrentTimeInvalidated(Timeline subject)
        {
            subject.CurrentTimeInvalidated           += new EventHandler(OnCurrentTimeInvalidated);
        }

        public virtual void AttachCompleted(Timeline subject)
        {
            subject.Completed                        += new EventHandler(OnCompleted);
        }

        public virtual void AttachRemoveRequested(Timeline subject)
        {
            subject.RemoveRequested                  += new EventHandler(OnRemoveRequested);
        }

        public virtual void AttachAllHandlers(Timeline subject)
        {
            subject.CurrentStateInvalidated          += new EventHandler(OnCurrentStateInvalidated);
            subject.CurrentGlobalSpeedInvalidated    += new EventHandler(OnCurrentGlobalSpeedInvalidated);
            subject.CurrentTimeInvalidated           += new EventHandler(OnCurrentTimeInvalidated);
            subject.Completed                        += new EventHandler(OnCompleted);
            subject.RemoveRequested                  += new EventHandler(OnRemoveRequested);
        }

        //----------------DETACH TIMELINE HANDLERS----------------------------------------
        public virtual void DetachCurrentStateInvalidated(Timeline subject)
        {
            subject.CurrentStateInvalidated          -= new EventHandler(OnCurrentStateInvalidated);
        }

        public virtual void DetachCurrentGlobalSpeedInvalidated(Timeline subject)
        {
            subject.CurrentGlobalSpeedInvalidated    -= new EventHandler(OnCurrentGlobalSpeedInvalidated);
        }

        public virtual void DetachCurrentTimeInvalidated(Timeline subject)
        {
            subject.CurrentTimeInvalidated           -= new EventHandler(OnCurrentTimeInvalidated);
        }

        public virtual void DetachCompleted(Timeline subject)
        {
            subject.Completed                        -= new EventHandler(OnCompleted);
        }

        public virtual void DetachRemoveRequested(Timeline subject)
        {
            subject.RemoveRequested                  -= new EventHandler(OnRemoveRequested);
        }

        public virtual void DetachAllHandlers(Timeline subject)
        {
            subject.CurrentStateInvalidated          -= new EventHandler(OnCurrentStateInvalidated);
            subject.CurrentGlobalSpeedInvalidated    -= new EventHandler(OnCurrentGlobalSpeedInvalidated);
            subject.CurrentTimeInvalidated           -= new EventHandler(OnCurrentTimeInvalidated);
            subject.Completed                        -= new EventHandler(OnCompleted);
            subject.RemoveRequested                  -= new EventHandler(OnRemoveRequested);
        }

        //----------------ATTACH CLOCK HANDLERS-------------------------------------------
        public virtual void AttachCurrentStateInvalidatedTC(Clock subject)
        {
            subject.CurrentStateInvalidated          += new EventHandler(OnCurrentStateInvalidated);
        }

        public virtual void AttachCurrentGlobalSpeedInvalidatedTC(Clock subject)
        {
            subject.CurrentGlobalSpeedInvalidated    += new EventHandler(OnCurrentGlobalSpeedInvalidated);
        }

        public virtual void AttachCurrentTimeInvalidatedTC(Clock subject)
        {
            subject.CurrentTimeInvalidated           += new EventHandler(OnCurrentTimeInvalidated);
        }

        public virtual void AttachCompletedTC(Clock subject)
        {
            subject.Completed                        += new EventHandler(OnCompleted);
        }

        public virtual void AttachRemoveRequestedTC(Clock subject)
        {
            subject.RemoveRequested                  += new EventHandler(OnRemoveRequested);
        }

        public virtual void AttachRepeatTC(Clock subject)
        {
            subject.CurrentTimeInvalidated           += new EventHandler(OnCurrentTimeInvalidatedRepeat);
        }

        public virtual void AttachAllHandlersTC(Clock subject)
        {
            subject.CurrentStateInvalidated          += new EventHandler(OnCurrentStateInvalidated);
            subject.CurrentTimeInvalidated           += new EventHandler(OnCurrentTimeInvalidated);
            subject.CurrentGlobalSpeedInvalidated    += new EventHandler(OnCurrentGlobalSpeedInvalidated);
            subject.Completed                        += new EventHandler(OnCompleted);
            subject.RemoveRequested                  += new EventHandler(OnRemoveRequested);
        }

        //----------------DETACH CLOCK HANDLERS-------------------------------------------
        public virtual void DetachCurrentStateInvalidatedTC(Clock subject)
        {
            subject.CurrentStateInvalidated          -= new EventHandler(OnCurrentStateInvalidated);
        }

        public virtual void DetachCurrentTimeInvalidatedTC(Clock subject)
        {
            subject.CurrentTimeInvalidated           -= new EventHandler(OnCurrentTimeInvalidated);
        }

        public virtual void DetachCurrentGlobalSpeedInvalidatedTC(Clock subject)
        {
            subject.CurrentGlobalSpeedInvalidated    -= new EventHandler(OnCurrentGlobalSpeedInvalidated);
        }

        public virtual void DetachCompletedTC(Clock subject)
        {
            subject.Completed                        -= new EventHandler(OnCompleted);
        }

        public virtual void DetachRemoveRequestedTC(Clock subject)
        {
            subject.RemoveRequested                  -= new EventHandler(OnRemoveRequested);
        }

        public virtual void DetachAllHandlersTC(Clock subject)
        {
            subject.CurrentStateInvalidated          -= new EventHandler(OnCurrentStateInvalidated);
            subject.CurrentTimeInvalidated           -= new EventHandler(OnCurrentTimeInvalidated);
            subject.CurrentGlobalSpeedInvalidated    -= new EventHandler(OnCurrentGlobalSpeedInvalidated);
            subject.Completed                        -= new EventHandler(OnCompleted);
            subject.RemoveRequested                  -= new EventHandler(OnRemoveRequested);
        }

          
        //-----EVENT HANDLERS-----------------------------------------

        // 2-9-05: Now can verify Event firing (case-by-case basis) by accumulating notifications
        // and outputting them at the very end, thereby eliminating a dependency on order of 
        // event firing.  [Order is not guaranteed as of the 2-8-05 breaking change.]
        // to use this method, a test case must set the boolean "eventsVerify" to true and then
        // call WriteAllEvents after Ticking is done.

        /**************************************************************************************
        * 2-7-05: This function is invoked when CurrentStateInvalidated fires.
        * It fires when the Timeline changes state; possible states are: Active, Filling, Stopped.
        **************************************************************************************/
        public virtual void OnCurrentStateInvalidated(object subject, EventArgs args)
        {
            if (((Clock)subject).CurrentState == ClockState.Active)
            {
                WriteBegun(((Clock)subject));
            }
            else
            {
                WriteEnded(((Clock)subject));
            }
        }
          
        /**************************************************************************************
        *  : This function is invoked when CurrentGlobalSpeedInvalidated fires.
        * It fires when the Timeline changes speed, which can result from a number of things:
        *Acceleration/Deceleration changes, pausing, seeking, beginning, ending, etc.
        **************************************************************************************/
        public virtual void OnCurrentGlobalSpeedInvalidated(object subject, EventArgs args)
        {
               bool   addedToArray = false;
               double globalSpeed  = GetCurrentGlobalSpeed(((Clock)subject));

               if (!eventsVerify)
               {
                   outString += "  " + ((Clock)subject).Timeline.Name + ": CurrentGlobalSpeedInvalidated fired";
               }               
               
               //Using the Clock's CurrentGlobalSpeed property to determine if the Timeline reverses.
               //CurrentGlobalSpeed changes to -1 when the clock reverses.
               if (globalSpeed < 0)
               {
                   //Avoid signalling repeated "Reversed" during Acceleration/Deceleration, by
                   //writing 'Reversed' only when the sign of the CurrentGlobalSpeed property changes.
                   //LIMITATION: this won't detect multiple Timelines reversing at the same time.
                   if (Math.Sign(globalSpeed) != Math.Sign(saveSpeed))
                   {
                       WriteReversed(ref addedToArray);
                   }
               }
               
               //Using the Clock's IsPaused property to determine if the Timeline pauses/resumes.
               if (((Clock)subject).IsPaused)
               {
                   WritePaused(ref addedToArray);
               }
               else
               {
                   if (savePaused)
                   {
                       //If clock was at one time paused, but IsPaused no longer true, then
                       //the clock has resumed when CurrentGlobalSpeedInvalidated has fired.
                       WriteResumed(ref addedToArray);
                   }
               }

               savePaused = ((Clock)subject).IsPaused;
               saveSpeed = globalSpeed;

               if (eventsVerify)
               {
                   //This is a catch-all: in certain scenarios, CurrentGlobalSpeedInvalidated
                   //will fire, but not due to a Paused/Resumed/Reversed clock.
                   if (!addedToArray)
                   {
                       WriteCurrentGlobalSpeed();
                   }
               }
               else
               {
                   outString += "\n";
               }
        }

        /**************************************************************************************
        * This function is invoked when the Completed event fires.
        **************************************************************************************/
        public virtual void OnCompleted(object subject, EventArgs args)
        {
            WriteCompleted(((Clock)subject));
        }

        /**************************************************************************************
        * This function is invoked when the RemoveRequested event fires.
        **************************************************************************************/
        public virtual void OnRemoveRequested(object subject, EventArgs args)
        {
            WriteRemoveRequested(((Clock)subject));
        }

        public virtual double GetCurrentGlobalSpeed(Clock clock)
        {
            if (clock.CurrentGlobalSpeed == null)
            {
                //Handle a null CurrentGlobalSpeed.
                return 99999;
            }
            else
            {
                return (double)clock.CurrentGlobalSpeed;
            }
        }

        //-----Handlers for Clock only--------------------------------------------------
        /**************************************************************************************
        *  : This function is invoked when CurrentTimeInvalidated fires.
        * It fires when the Clock's CurrentTime property changes.
        **************************************************************************************/
        public virtual void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {               
            //Place 'Progress' in outString in order to force a call to LocalizeProgressValue().
            outString += "  " + ((Clock)subject).Timeline.Name + ": CurrentTimeInvalidated fired (Progress = " + ((Clock)subject).CurrentProgress + ")\n";
            _currentTimeEvents.Add(tickNumber);
        }
                   
        /**************************************************************************************
        * This version only outputs if the Timeline repeats. It won't take part in 
        * tracking firing of CurrentTimeInvalidated.
        **************************************************************************************/
        public virtual void OnCurrentTimeInvalidatedRepeat(object subject, EventArgs args)
        {                             
            //Using the Clock's CurrentIteration property to determine if the Timeline repeats.
            //CurrentIteration changes to 1 when the Timeline begins, and -1 when it ends.
            //It also increments when the Timeline repeats.

            if ((((Clock)subject).CurrentIteration > iterationCount))
            {
                if (iterationCount > 0)
                {
                    //-----THE TIMELINE HAS REPEATED-----
                    if (eventsVerify)
                    {
                        _repeatedEvents.Add(tickNumber);
                    }
                    else
                    {
                        outString += "  " + ((Clock)subject).Timeline.Name + ": CurrentTimeInvalidated fired  (Repeated)" + "\n";
                    }
                }
                iterationCount++;
            }
        }
          
         /**************************************************************************************
          * The following routines write the results of the event firing, either via outString
          * directly, or via a set of ArrayLists.
          **************************************************************************************/         
          public virtual void WriteBegun(Clock clock)
          {
               //-----THE TIMELINE HAS BEGUN-----
               if (eventsVerify)
               {
                   _begunEvents.Add(tickNumber);
               }
               else
               {
                   outString += "  " + clock.Timeline.Name + ": CurrentStateInvalidated fired (Begun)" + "\n";
               }
          }

          public virtual void WriteEnded(Clock clock)
          {
               //-----THE TIMELINE HAS ENDED-----
               if (eventsVerify)
               {
                   _endedEvents.Add(tickNumber);
               }
               else
               {
                   outString += "  " + clock.Timeline.Name + ": CurrentStateInvalidated fired (Ended)" + "\n" ;
               }
          }
          
          public virtual void WriteReversed(ref bool added)
          {               
               //-----THE TIMELINE HAS REVERSED-----
               if (eventsVerify)
               {
                   _reversedEvents.Add(tickNumber);
                   added = true;
               }
               else
               {
                   outString += "  (Reversed)";
               }
          }
          
          public virtual void WritePaused(ref bool added)
          {               
               //-----THE TIMELINE HAS PAUSED-----
               if (eventsVerify)
               {
                   _pausedEvents.Add(tickNumber);
                   added = true;
               }
               else
               {
                   outString += "  (Paused)";
               }
          }

          public virtual void WriteResumed(ref bool added)
          {               
               //-----THE TIMELINE HAS RESUMED-----
               if (eventsVerify)
               {
                   _resumedEvents.Add(tickNumber);
                   added = true;
               }
               else
               {
                   outString += "  (Resumed)";
               }
          }

          public virtual void WriteCompleted(Clock clock)
          {               
               //-----THE TIMELINE HAS COMPLETED-----
               if (eventsVerify)
               {
                   _completedEvents.Add(tickNumber);
               }
               else
               {
                   outString += "  " + clock.Timeline.Name + ": Completed fired" + "\n" ;
               }
          }

          public virtual void WriteRemoveRequested(Clock clock)
          {               
               //-----THE RESUMEREQUESTED EVENT FIRED-----
               if (eventsVerify)
               {
                   _removeRequestedEvents.Add(tickNumber);
               }
               else
               {
                   outString += "  " + clock.Timeline.Name + ": RemoveRequested fired" + "\n" ;
               }
          }

          public virtual void WriteCurrentGlobalSpeed()
          {               
               _currentGlobalSpeedEvents.Add(tickNumber);             
          }
          
         /*
          *  : This function outputs the contents of the events arrraylists.
          * It sorts them separately, based on Tick number, so that the actual order of event
          * firing does not affect the results.
          */
          public virtual void WriteAllEvents()
          {
              if ( _begunEvents.Count > 0 )
              {
                  _begunEvents.Sort();
                  outString += "---------- CurrentStateInvalidated: Timeline Began" + "\n";
                  foreach ( int s1 in _begunEvents )
                  {
                      outString += "---At Tick #" + s1 + "\n";
                  }
              }

              if ( _endedEvents.Count > 0 )
              {
                  _endedEvents.Sort();
                  outString += "---------- CurrentStateInvalidated: Timeline Ended" + "\n";
                  foreach ( int s2 in _endedEvents )
                  {
                      outString += "---At Tick #" + s2 + "\n";
                  }
              }

              if ( _currentGlobalSpeedEvents.Count > 0 )
              {
                  _currentGlobalSpeedEvents.Sort();
                  outString += "---------- CurrentGlobalSpeedInvalidated" + "\n";
                  foreach ( int s3 in _currentGlobalSpeedEvents )
                  {
                      outString += "---At Tick #" + s3 + "\n";
                  }
              }

              if ( _reversedEvents.Count > 0 )
              {
                  _reversedEvents.Sort();
                  outString += "---------- CurrentGlobalSpeedInvalidated: Timeline Reversed" + "\n";
                  foreach ( int s4 in _reversedEvents )
                  {
                      outString += "---At Tick #" + s4 + "\n";
                  }
              }

              if ( _repeatedEvents.Count > 0 )
              {
                  _repeatedEvents.Sort();
                  outString += "---------- CurrentTimeInvalidated: Timeline Repeated" + "\n";
                  foreach ( int s5 in _repeatedEvents )
                  {
                      outString += "---At Tick #" + s5 + "\n";
                  }
              }

              if ( _pausedEvents.Count > 0 )
              {
                  _pausedEvents.Sort();
                  outString += "---------- CurrentGlobalSpeedInvalidated: Timeline Paused" + "\n";
                  foreach ( int s6 in _pausedEvents )
                  {
                      outString += "---At Tick #" + s6 + "\n";
                  }
              }

              if ( _resumedEvents.Count > 0 )
              {
                  _resumedEvents.Sort();
                  outString += "---------- CurrentGlobalSpeedInvalidated: Timeline Resumed" + "\n";
                  foreach ( int s7 in _resumedEvents )
                  {
                      outString += "---At Tick #" + s7 + "\n";
                  }
              }

              if ( _currentTimeEvents.Count > 0 )
              {
                  _currentTimeEvents.Sort();
                  outString += "---------- CurrentTimeInvalidated" + "\n";
                  foreach ( int s8 in _currentTimeEvents )
                  {
                      outString += "---At Tick #" + s8 + "\n";
                  }
              }

              if ( _completedEvents.Count > 0 )
              {
                  _completedEvents.Sort();
                  outString += "---------- Completed" + "\n";
                  foreach ( int s9 in _completedEvents )
                  {
                      outString += "---At Tick #" + s9 + "\n";
                  }
              }

              if ( _removeRequestedEvents.Count > 0 )
              {
                  _removeRequestedEvents.Sort();
                  outString += "---------- RemoveRequested" + "\n";
                  foreach ( int s10 in _removeRequestedEvents )
                  {
                      outString += "---At Tick #" + s10 + "\n";
                  }
              }
          }
        //-------------------------------------------------------------------------------------         
          
          
        public virtual void OnProgress( Clock subject )
        {    
            if ( ((Clock)subject).CurrentState == ClockState.Active )
            {
                outString += "  " + ((Clock)subject).Timeline.Name + ": Progress = " + subject.CurrentProgress + "\n";
            }
        }

        public virtual void PreTick(int i)
        {
        }

        public virtual void PostTick(int i)
        {
        }
          
        public abstract string Test();     
     }


    /*
    *  : Added this class to reduce the impact of any Timing related dev
    * changes. This class is a place holder of all the methods/functions that are
    * common to all testcases.
    *        
    */
    class TimeGenericWrappers
    {
           /*  : This function checks for correct Exceptions.
           * Input: expected -- The expected Exception type.
           *        actual   -- The actual Exception type.
           */
          internal static void CHECKEXCEPTION( Type expected, Type actual, ref string outString )
          {        
                if (expected == actual || actual.IsSubclassOf(expected))
                {
                    outString += "----------PASS. Expected exception found." + "\n";
                }
                else
                {
                    outString += "----------FAIL. Expected exception not found." + "\n";
                    outString += "--Expected: " + expected.ToString() + "\n";
                    outString += "--Actual:   " + actual.ToString()   + "\n";
                }
          }

          /*
           *  : This function fires OnProgress on the node and its children.
           * Input:currentTest -- Handle to the currentTest so that events can be fired. 
           *           tNode -- Node on which progress needs to be fired.
           */
          internal static void FIREONPROGRESS( ITimeBVT currentTest, Clock tClockNode )
          {        
               currentTest.OnProgress( tClockNode );

               ClockGroup tClockNodeGroup = tClockNode as ClockGroup;

               if ( tClockNodeGroup != null )
               {
                   foreach ( Clock tClockChild in tClockNodeGroup.Children )
                   {
                        if ( (tClockChild.CurrentState == ClockState.Active) || (tClockChild.CurrentState == ClockState.Filling))
                        {
                             FIREONPROGRESS( currentTest, tClockChild );
                        }
                   }
               }
          }

          /*
           *  : This function Displays the state of tNode and its children.
           *             State is determined by CurrentProgress and CurrentState properties.
           * Input:currentTest -- Handle to the currentTest so that events can be fired. 
           *           tNode -- Node whose state is emitted.
           */
          internal static void DISPLAYSTATE( ITimeBVT currentTest, Clock tClock )
          {
               currentTest.outString += "State of " + tClock.Timeline.Name + " : ";
               currentTest.outString += " , Progress : "      + tClock.CurrentProgress + 
                                              " , CurrentState : "   + tClock.CurrentState + "\n";

               ClockGroup tClockGroup = tClock as ClockGroup;

               if ( tClockGroup != null )
               {
                   foreach ( Clock tClockChild in tClockGroup.Children )
                   {
                        DISPLAYSTATE( currentTest, tClockChild );                         
                   }
               }

          }

          /*
           *  : This function sends Clock ticks to the time manager.
           * Input: begin -- start tick
           *        end   -- last tick
           *        step  -- length of the interval between two ticks
           *        outString -- a string variable to send output back to the caller
           */
          internal static void EXECUTE(ITimeBVT currentTest, Clock tClock, TimeManagerInternal tMan, int begin, int end, int step, ref string outString )
          {
               currentTest.CurrentTime = TimeSpan.FromMilliseconds(begin);
               tMan.Start();

               for(int i = begin ; i <= end; i += step )
               {
                    ITimeBVT.tickNumber = i;
                    
                    outString += "Processing time " + i + " ms\n";
                    DEBUG.LOGSTATUS("Processing time " + i);

                    currentTest.PreTick(i);

                    currentTest.CurrentTime = TimeSpan.FromMilliseconds(i);
                    tMan.Tick();  
                    
                    FIREONPROGRESS( currentTest, tClock );                              

                    currentTest.PostTick(i);
               }
          }
     
          /*
           *  : This function sends Clock ticks to the time manger and emits the state of TimingEngine after each clock tick.
           * Input: begin -- start tick
           *        end   -- last tick
           *        step  -- length of the interval between two ticks
           *        outString -- a string variable to send output back to the caller
           */
          internal static void TESTSTATE(ITimeBVT currentTest, Clock tClock, TimeManagerInternal tMan, int begin, int end, int step, ref string outString )
          {
               currentTest.CurrentTime = TimeSpan.FromMilliseconds(begin);
               tMan.Start();

               for( int i = begin ; i <= end; i += step )
               {                    
                    ITimeBVT.tickNumber = i;

                    outString += "Processing time " + i + " ms\n";
                    DEBUG.LOGSTATUS("Processing time " + i);
                    
                    currentTest.PreTick(i);

                    currentTest.CurrentTime = TimeSpan.FromMilliseconds(i);
                    tMan.Tick();
              
                    DISPLAYSTATE( currentTest, tClock );                                                       

                    currentTest.PostTick(i);
               }
          }
        #endregion
    }
}
