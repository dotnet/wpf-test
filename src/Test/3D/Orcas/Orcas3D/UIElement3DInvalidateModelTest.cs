// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using Microsoft.Test.Graphics.TestTypes;
using System.Threading;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// UIElement3D.InvalidateModel() and UIElement3D.OnUpdateModel() Test
    /// </summary>
    public class UIElement3DInvalidateModelTest : CoreGraphicsTest
    {
        #region Private Data

        private CustomUIElement3D _testElement;

        #endregion

        #region Override Members

        /// <summary>Run UIElement3D.InvalidateModel() / UIElement3D.OnUpdateModel() Test</summary>
        public override void RunTheTest()
        {
            //The test is run in a separate thread, to allow the test to explicitly end 
            //itself (with a call to Dispatcher.BeginInvokeShutdown) rather than
            //having the test end immediately once the RunTheTest() method completes. 
            //This is required because the test makes use of callbacks from timers.
            Thread t = new Thread(new ThreadStart(threadStart));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }

        #endregion

        #region Private Members

        private void threadStart()
        {
            _testElement = new CustomUIElement3D(this);
            _testElement.RunTheTest();
            Dispatcher.Run();
            Dispatcher.CurrentDispatcher.UnhandledException += Dispatcher_UnhandledException;
        }

        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            AddFailure("An unhandled Exception occured during the test");

            Log("Exception: " + e.Exception.ToString());
            Log("Stack Trace: " + e.Exception.StackTrace);

            Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);
        }

        #endregion

        /// <summary>
        /// This class is created for use within the UIElement3DInvalidateModelTest
        /// </summary>
        private class CustomUIElement3D : UIElement3D
        {
            #region Private Data

            //Timer to control the execution of the test
            private DispatcherTimer _testTimer;

            //Causes the test to timeout after 5 seconds
            private DispatcherTimer _timeoutTimer;

            //Set to true when InvalidateModel() is called.
            //Set back to false by OnUpdateModel()
            private bool _modelInvalidated = false;

            //Number of times OnUpdateModel() has been called.
            private int _numTimesOnUpdateModelCalled = 0;

            //Reference to test object to use logging functionality
            private UIElement3DInvalidateModelTest _test;

            #endregion

            #region Constructors

            internal CustomUIElement3D(UIElement3DInvalidateModelTest testObject)
            {
                _test = testObject;
                _test.Log("CustomUIElement3D.ctor()");
            }

            #endregion

            #region Internal Members

            internal void RunTheTest()
            {
                _test.Log("CustomUIElement3D.RunTheTest()");
                CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);

                //Call InvalidateModel() in 500 milliseconds:
                _testTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(500),
                                                DispatcherPriority.Normal,
                                                timer_InvalidateModel,
                                                Dispatcher.CurrentDispatcher);


                //Cause the test to timeout if it has not completed in 5 seconds
                _timeoutTimer = new DispatcherTimer(TimeSpan.FromSeconds(5),
                                                   DispatcherPriority.Normal,
                                                   timer_TestTimeout,
                                                   Dispatcher.CurrentDispatcher);
                _testTimer.Start();
                _timeoutTimer.Start();
            }

            #endregion

            #region Private Members

            private void timer_InvalidateModel(object sender, EventArgs e)
            {
                _testTimer.Tick -= new EventHandler(timer_InvalidateModel);
                _testTimer.Stop();

                //After calling InvalidateModel(), OnUpdateModel() should be called once before
                //the next render.
                _test.Log("Calling InvalidateModel()");
                InvalidateModel();
                _modelInvalidated = true;
            }

            //Called on every rendering
            private void CompositionTarget_Rendering(object sender, EventArgs e)
            {
                if (_modelInvalidated)
                {
                    _test.AddFailure("InvalidateModel() has been called, but rendering is happening"
                                     + "before OnUpdateModel() is called.");
                    EndTest();
                }
            }

            private void timer_TestTimeout(object sender, EventArgs e)
            {
                _test.AddFailure("Test timed out after 5 seconds");
                EndTest();
            }

            private void timer_testComplete(object sender, EventArgs e)
            {
                _test.Log("OnUpdateModel() was called once and only once after InvalidateModel() was called");
                _test.Log("Test Pass");
                EndTest();
            }

            private void EndTest()
            {
                _test.Log("End of UIElement3D.InvalidateModel() Test");
                _testTimer.Stop();
                _timeoutTimer.Stop();
                Dispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);
            }

            #endregion

            #region Override Members

            //This method should be called once and only once after InvalidateModel() has been called
            //but before CompositionTarget.Rendering event is fired.
            protected override void OnUpdateModel()
            {
                _numTimesOnUpdateModelCalled++;

                switch (_numTimesOnUpdateModelCalled)
                {
                    case 1:
                        //OnUpdateModel() is called once for the first render without InvalidateModel()
                        //having been called. 
                        _test.Log("OnUpdateModel() called for the first render.");
                        break;

                    case 2:
                        if (_modelInvalidated)
                        {
                            _test.Log("OnUpdateModel() called after InvalidateModel() and before Render");
                            _modelInvalidated = false;

                            //Wait 500 milliseconds before passing test to ensure
                            //OnUpdateModel is not called again.
                            _testTimer.Stop();
                            _testTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(500),
                                                            DispatcherPriority.Normal,
                                                            timer_testComplete,
                                                            Dispatcher.CurrentDispatcher);
                            _testTimer.Start();
                        }
                        else
                        {
                            _test.AddFailure("OnUpdateModel() was called without InvalidateModel() being called first");
                            EndTest();
                        }
                        break;

                    default:
                        _test.AddFailure("OnUpdateModel() was called more than once after InvalidateModel() is called");
                        EndTest();
                        break;
                }
            }
        }

        #endregion
    }
}