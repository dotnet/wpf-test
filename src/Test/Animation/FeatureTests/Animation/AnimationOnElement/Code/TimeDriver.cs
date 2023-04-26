// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
using System;
using System.Collections;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{

    /// <summary>
    /// Timed driver, does custom steps on repeated intervals
    /// </summary>
    public class TimeDriver : Driver
    {
        /* Internal members */

        /// <summary>
        /// Number of discrete steps for this case
        /// </summary>
        protected int currentStep;
        /// <summary>
        /// Collection of discrete steps
        /// </summary>
        protected ArrayList steps;
        /// <summary>
        /// If set, this will have the steps loop until
        /// </summary>
        protected bool looping;
        /// <summary>
        /// Internal timer
        /// </summary>
        protected System.Windows.Threading.DispatcherTimer timer;
        /// <summary>
        /// Millisecond time interval between timer ticks
        /// </summary>
        protected int timeInterval;
        /// <summary>
        /// Time manager for the application
        /// </summary>
        protected InternalTimeManagerAOE timeManager;
        
        public Nullable<TimeSpan> CurrentTime
        {
            get
            {
                return timeManager.CurrentTime;
            }
        }
        
        public TimeSpan CurrentGlobalTime
        {
            get
            {
                return timeManager.CurrentGlobalTime;
            }
        }


        /* Internal Methods */

        /// <summary>
        /// Event handler for timing object
        /// </summary>
        private void OnTimerEvent( object sender, System.EventArgs args )
        {
            HandleTimerEvent();
        }

        /// <summary>
        /// Actual event handler implementation
        /// </summary>
        virtual protected void HandleTimerEvent()
        {
            int thisStepIndex = 0;
            try
            {
                // remember the executing step
                thisStepIndex = currentStep;
                // exit condition
                if ( !looping && currentStep >= steps.Count)
                {
                    // stop timer
                    StopTimer();
                    // end
                    EndTest();
                }
                // get this step
                DoTimedStep thisStep = (DoTimedStep)steps[ currentStep % steps.Count ] ;
                // execute
                thisStep();
                // move to the next one
                currentStep++ ;
            }
            catch( System.Exception ex )
            {
                // handle exception for this step
                HandleTopLevelException( ex );
                // move to the next, if exception handler did not fix this
                // to avoid endless loops.
                if( thisStepIndex == currentStep )
                {
                    currentStep++ ;
                }
            }
        }

        /* Overrides */

        /// <summary>
        /// Exception handler stops timer
        /// </summary>
        protected override void HandleTopLevelException( System.Exception ex )
        {
            // stop timer
            StopTimer();
            base.HandleTopLevelException( ex );
        }
        /// <summary>
        /// Test end stops timer
        /// </summary>
        protected override void EndTest()
        {
            // End the test
            base.EndTest();
            StopTimer();
        }

        /* Public API */

        /// <summary>
        /// Custom test step Delegate
        /// </summary>
        public delegate void DoTimedStep();

        /// <summary>
        /// Constructor needs time step
        /// </summary>
        public TimeDriver( int millisecondStepTime ) : base()
        {
            // create steps container
            steps = new ArrayList();
            // create timer
            timeInterval = millisecondStepTime;
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new System.TimeSpan(0,0,0,0,timeInterval);
            timer.Tick += new System.EventHandler( OnTimerEvent );
            // linear set by default
            looping = false;

            // create time manager
            timeManager = TrustedHelper.TimeManagerFromContext(Dispatcher.CurrentDispatcher);

        }

        /// <summary>
        /// Add interface for steps
        /// </summary>
        virtual public void AddStep( DoTimedStep newStep )
        {
            steps.Add( newStep );
        }

        /// <summary>
        /// Timer wrapper
        /// </summary>
        virtual public void StartTimer()
        {
            timer.Start();
        }
        /// <summary>
        /// Timer wrapper
        /// </summary>
        virtual public void StopTimer()
        {
            timer.Stop();
        }

        /// <summary>
        /// Accessor
        /// </summary>
        public int CurrentStep
        {
            get { return currentStep; }
        }
        /// <summary>
        /// Accessor
        /// </summary>
        public ArrayList Steps
        {
            get { return steps; }
        }
        /// <summary>
        /// Accessor
        /// </summary>
        public bool Looping
        {
            get { return looping; }
        }
        /// <summary>
        /// Accessor
        /// </summary>
        protected int TimeInterval
        {
            get { return timeInterval; }
        }

        /* Stock Steps */

        /// <summary>
        /// NO-OP Step
        /// </summary>
        public void DoEmptyStep()
        {
        }
        /// <summary>
        /// Global Pause
        /// </summary>
        public void DoPauseTimelineStep()
        {
            timeManager.Pause();
        }
        /// <summary>
        /// Global Resume
        /// </summary>
        public void DoResumeTimelineStep()
        {
            timeManager.Resume();
        }

        /* Helper functions */

        /// <summary>
        /// Gets the current time from the TimeManager
        /// </summary>
        /// <returns>Current global time.</returns>
        public TimeSpan? GetCurrentTime()
        {
            return timeManager.CurrentTime;
        }

    }

    /// <summary>
    /// Common Integration Test TimedDriver implementations
    /// </summary>
    public class TimeIntegrationDriver : TimeDriver
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public TimeIntegrationDriver( int millisecondStepTime ) :
            base( millisecondStepTime )
        {
            // Create reporting structure
            currentResults  = new CategoryFilter();
            currentResults.RegisterCategory(CommonConstants.TestPass);
            currentResults.RegisterCategory(CommonConstants.TestFail);
            currentResults.RegisterCategory(CommonConstants.TestIgnore);
            currentResults.RegisterCategory(CommonConstants.TestInternalFailure);
     
        }

        /// <summary>
        /// Constructor, uses predefined time by default
        /// </summary>
        public TimeIntegrationDriver() :
            this( CommonConstants.AnimationWaitTime ) {}

        /// <summary>
        /// Custom exception handler, logs exception and moves to next step
        /// </summary>
        protected override void HandleTopLevelException( System.Exception ex )
        {
   
            // just log and continue
            Log2( String.Format( "Exception Caught at step #{0}: {1}", currentStep, ex.ToString() ) );
            // loop back to the beginning of the next cycle, mi
            currentStep += ( steps.Count - (currentStep % steps.Count) ) ;
        }


        #region Context exception handling
        /// <summary>
        /// Flag and enable context exception handler
        /// </summary>
        public bool HandleDispatcherExceptions
        {
            get { return _handleDispatcherExceptions; }

            set
            {
/*
                // add if missing
                if ( handleDispatcherExceptions == false && value == true )
                {
                    System.Windows.Application.Current.DispatcherUnhandledException +=
                        new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(LocalDispatcherExceptionHandler);
                }
                // remove if existing
                if ( handleDispatcherExceptions == true && value == false )
                {
                    System.Windows.Application.Current.DispatcherUnhandledException -=
                        new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(LocalDispatcherExceptionHandler);
                }
*/
            }
        }
        private bool _handleDispatcherExceptions = false;

        /// <summary>
        /// Custom context exception handler
        /// </summary>
        protected void LocalDispatcherExceptionHandler( object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs args )
        {
            // use our handler
            HandleTopLevelException( args.Exception );
            // mark handled
            args.Handled = true;
        }
        #endregion


        /// <summary>
        /// Custom Logger
        /// </summary>
        protected override void Log2( string message )
        {
            LogHelper.LogTestMessage( String.Format( "[{0}]: {1}", this.Name, message ) );
        }

        /// <summary>
        /// Custom Reporter
        /// </summary>
        protected override void ReportResult( bool result, string comment )
        {
            LogHelper.LogTest( result, comment );
            GlobalLog.LogEvidence( comment );
//            AnimationOnElementTest.testPassed = result;
        }

        /// <summary>
        /// End test override
        /// </summary>
        protected override void EndTest()
        {
            // End the test
            base.EndTest();
            IntegrationUtilities.EndCurrentTest();
        }

        /// <summary>
        /// Registers a partial result within a category.
        /// <param name="ip">integration property under test</param>
        /// <param name="category">Category of the result, usually pass,fail,ignore or internal failure.</param>
        /// <param name="reason">Extra information about the partial result.</param>
        /// </summary>
        virtual public void RegisterResult(IntegrationProperty ip, string category, string reason)
        {
          //  Log2( String.Format("{0}: {1} ({2})",ip.ToString(),category,reason ) );
            currentResults.AddItemToCategory( category, ip.ToString() );

            // log pass and fail to final tactics
            if ( category == CommonConstants.TestPass )
            {
                LogHelper.LogSubTest(
                    true,
                    String.Format("{0} on {1}", this.Name, ip.ToString() ),
                    0 );
                Console.WriteLine("PASS: {0} on {1}", this.Name, ip.ToString());
            }
            if ( category == CommonConstants.TestFail )
            {
                Console.WriteLine("FAIL: {0} on {1}", this.Name, ip.ToString());
                LogHelper.LogSubTest(
                    false,
                    String.Format("{0} on {1}", this.Name, ip.ToString() ),
                    0 );
            }
        }

   
        /// <summary>
        /// Category filter for sorting sub test results
        /// </summary>
        protected  CategoryFilter currentResults ;

         /// <summary>
        /// Extra common step to flush the Core Queue
        /// </summary>
        public void DoFlushStep()
        {
            Queue.Flush();
        }
    }

}




