// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//  summary: driver that sequentially sets an animation on all properties of
//  a given element.  Variations use different methods of setting animation
//  and/or filter the property set.
//
// $Id:$ $Change:$
using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text.RegularExpressions;
using System.Globalization;

using System.Threading;
using System.Windows.Threading;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Data;
using System.Windows.Navigation;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Threading;


namespace Microsoft.Test.Animation
{

    public class ComprehensiveAnimationDriver : TimeIntegrationDriver
    {
        /// <summary>
        /// count of how many samples we take of the property
        /// </summary>
        protected int numberOfMeasurements = 8;

        /// <summary>
        /// Measure for this sub-step
        /// </summary>
        protected int currentMeasurement;

        protected int indexMeasurement;
        
        /// <summary>
        /// recording of the animated property value
        /// </summary>
        protected struct Measurement
        {
            public int TickCount;               // # of times the property has been measured (for DiscreteAnimation class)
            public object PropertyValue;        // value of property when measured
            
            public override string ToString()
            {
                return "Measurement (" + PropertyValue + ", " + TickCount + ")";
            }
        }

        /// <summary>
        /// Measurement collection
        /// </summary>
        protected Measurement[] measurements;
        
        /// <summary>
        /// Internal cummulative test result
        /// </summary>
        protected bool testResult;
        
        /// <summary>
        /// Reporting value
        /// </summary>
        protected string durationValue;
        
        /// <summary>
        /// beginning of animation
        /// </summary>
        protected TimeSpan startingTime;

        /// <summary>
        /// time value at which the property change effect was first felt
        /// </summary>
        protected TimeSpan effectTime;
        
        /// <summary>
        /// end of animation
        /// </summary>
        protected TimeSpan endTime;

        /// <summary>
        /// Property list
        /// </summary>
        protected IntegrationProperty[] testProperties;
        
        /// <summary>
        /// Current active property index
        /// </summary>
        protected int currentPropertyIndex;
        
        /// <summary>
        /// Current active property from the list for this sub-scenario
        /// </summary>
        public IntegrationProperty CurrentProperty
        {
            get { return testProperties[currentPropertyIndex % testProperties.Length]; }
        }

        /// <summary>
        /// Target Element for test
        /// </summary>
        protected DependencyObject targetElement;
        
        /// <summary>
        /// Reference to original source element
        /// </summary>
        protected DependencyObject originalElement;
        
        /// <summary>
        /// Container for target Element for test
        /// </summary>
        protected UIElement parent;

        /// <summary>
        /// Data association that provides the necessary property values for the current element
        /// </summary>
        protected AnimationDataAssociation dataAssociation;

        /// <summary>
        /// reference to active animation clock
        /// </summary>
        protected AnimationClock _currentAnimationClock = null;
        protected string elementFile;

        /// <summary>
        /// Constructor creates the required test steps
        /// </summary>
        public ComprehensiveAnimationDriver(string markupFile, int timeInterval) : base(timeInterval)
        {
            // Set Base class parameters
            // this is a looping test
            looping = true;
            testResult = true;
            elementFile = markupFile;

            // add steps for test: set it up, run the test, clean it up
            // StartAnimationTest will stop the global timer, run the test, and 
            // then restart it once it's finished sampling animation values
            AddStep(new DoTimedStep(DoSetUp));
            AddStep(new DoTimedStep(StartAnimationTest));
            AddStep(new DoTimedStep(DoCleanUp));
        }

        /// <summary>
        /// Custom exception handler, logs exception and moves to next step
        /// </summary>
        override protected void HandleTopLevelException( System.Exception ex )
        {
            // do base handler
            base.HandleTopLevelException( ex );

            // save result
            if ( ex is InternalDriverException )
            {
                RegisterResult(CurrentProperty, CommonConstants.TestInternalFailure, ex.Message );
            }
            else
            {
                RegisterResult(CurrentProperty, CommonConstants.TestFail, "Fail. Exeception thrown inside the driver." );
                // only set fail on test exceptions
                testResult &= false;
            }
            // resync driver
            // begin measuring at the beginning
            currentMeasurement = 0;
            indexMeasurement = 0;
            // go to the next property
            currentPropertyIndex = currentStep / steps.Count ;
        }

        /// <summary>
        /// Start the test
        /// </summary>
        public void Test(DependencyObject target, UIElement testParent)
        {
            // test specific scenario
            TestSetup(target, testParent);

            // begin timed test
            StartTimer();
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="target">the UIElement under test</param>
        virtual public void TestSetup(DependencyObject target, UIElement testParent)
        {
            // NOTE : we're going to create a new element to do the testing
            // with anyway, this is just a consequence of how the design was done. 
            
            // save input internally
            originalElement = targetElement = target;
            parent = testParent;

            // Create a property browser to get complete property description
            PropertyExplorer pe = new PropertyExplorer( targetElement );

            // Scan animatable properties
            ArrayList tempList = new ArrayList();
            foreach ( IntegrationProperty ip in pe.IntegrationProperties )
            {
                // fill the logs for this one
              //  SetCommonPropertyLog( ip );
                // apply filter, if implemented
                if ( FilterProperty( ip ) ) continue;
                // only store real animatable properties
                tempList.Add( ip );
            }
            Log2("### Properties to test: " + tempList.Count);
            
            // Get list of interesting properties
            testProperties = new IntegrationProperty[ tempList.Count ] ;
            for( int i = 0; i < tempList.Count; i++ )
            {
                testProperties[i] = (IntegrationProperty)tempList[i];
           //     if (testProperties[i].ToString() == "IsUndoEnabled")
           //         testProperties[i] = testProperties[i - 1];

            }

            // Set property index
            currentPropertyIndex = 0;

            Log2( "### Creating data association ..." );
            // create DATA association for this element
            dataAssociation = new AnimationDataAssociation(
                    null,
                    GetAnimationDuration(),
                    target
                );

        }

        /// <summary>
        /// Determines whether a particular property must be filtered for this test
        /// </summary>
        /// <param name="ip">the property to filter</param>
        /// <returns>true if the property needs filtering, otherwise false</returns>
        virtual public bool FilterProperty( IntegrationProperty ip )
        {
            // read-only properties can't be animated
            if (FilterReadOnlyProperty(ip))
            {
                RegisterResult(ip, CommonConstants.TestIgnore, "Read-only property cannot be animated.");
                return true;
            }

            // properties that don't implement UIPropertyMetadata can't be animated
            if (FilterNonAnimatableProperty(ip))
            {
                RegisterResult(ip, CommonConstants.TestIgnore, "Property without UIPropertyMetadata cannot be animated.");
                return true;
            }

            // finally, we have a set of known "bad" properties with associated 
            if (FilterKnownBugProperty(ip))
            {
                RegisterResult(ip, CommonConstants.TestIgnore, "Property has a known animation issue - ignoring.");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if the property has a known issue and must be filtered from this test
        /// </summary>
        private bool FilterKnownBugProperty(IntegrationProperty ip)
        {
            // filter out known issues
            Regex filterExpression;
            filterExpression = new Regex("Timeline");

            // Filter out known issues
            string filterstring = PropertyFilterTable.Instance().GetFilterString(originalElement);

            //Log(PropertyFilterTable.Instance().ToString());
            Regex knownIssuesFilter = new Regex( filterstring );
            if (knownIssuesFilter.Match(ip.ToString()).Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Determines whether a particular property is read-only and must be filtered from this test
        /// </summary>
        private bool FilterReadOnlyProperty(IntegrationProperty ip)
        {
            // can't animate something if you can't set it.
            if (ip.ReadOnly)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether a particular property is not animatable and must be filtered from this test
        /// </summary>
        private bool FilterNonAnimatableProperty(IntegrationProperty ip)
        {
            // filter out properties without UIPropertyMetadata
            UIPropertyMetadata ownerMetadata = ip.OriginalProperty.DefaultMetadata as UIPropertyMetadata;
            if (ownerMetadata == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Set up for this sub-step
        /// </summary>
        virtual public void DoSetUp()
        {
            if (!CommonSetUpPreAmble()) return;

            // Clear container - DO NOT use visualoperations to clean children of Panel. Not supported - results undefined.
            ((Panel)parent).Children.Clear();

            // get the current objects
            string currentTypeName = IntegrationUtilities.GetApplicationProperty( CommonConstants.flagTestElement );

            // create the element
            DependencyObject[] testItems = IntegrationUtilities.GetStockElementAndParent(elementFile,  currentTypeName );

            if ( testItems[0] != null )
            {
                targetElement = testItems[0];

                // add the element to the tree
                if ( testItems[1] != null)
                {
                    ((System.Windows.Markup.IAddChild)parent).AddChild( testItems[1] );
                }
                else
                {
                    // default is to use canvas directly
                    ((System.Windows.Markup.IAddChild)parent).AddChild( testItems[0] );
                }
            }
            else
            {
                // Use internal exception to abort this sub-run.
                throw new InternalDriverException("Cannot create element from stock reference.");
            }

            // get animation data from factory
            AnimationTimeline animation = (AnimationTimeline)dataAssociation.GetValueForProperty(CurrentProperty.OriginalProperty);

            // add it if present
            if ( animation == null )
            {
                // Use internal exception to abort this sub-run.
                throw new InternalDriverException("Cannot provide animation for property.");
            }
            
            // add animation to the element
            // we restrict ourselves to IAnimatables
            if (targetElement is IAnimatable)
            {
                _currentAnimationClock = animation.CreateClock();
                ((IAnimatable)targetElement).ApplyAnimationClock(CurrentProperty.OriginalProperty, _currentAnimationClock);
            }
            else
            {
                throw new InternalDriverException("targetElement is not an IAnimatable.");
            }
        }

        /// <summary>
        /// Common part of setup
        /// </summary>
        virtual protected bool CommonSetUpPreAmble()
        {
            // exit condition
            if ( currentPropertyIndex >= testProperties.Length )
            {
                // report result
                ReportResult(testResult, this.Name );
                // exit
                EndTest();
                // stop the loop

                AnimationOnElementTest.testPassed = testResult;
                if (testResult)
                {
                    AnimationOnElementTest.dispatcherSignalHelper.Signal( "AnimationDone", TestResult.Pass );
                }
                else
                {
                    AnimationOnElementTest.dispatcherSignalHelper.Signal( "AnimationDone", TestResult.Fail );
                }

                return false;
            }

            //Log("### New test starting for: " + CurrentProperty.ToString() + " @" + DateTime.Now.ToString("T", DateTimeFormatInfo.InvariantInfo));
      
            // create set of measurements
            measurements = new Measurement[numberOfMeasurements] ;
            currentMeasurement = 0;
            indexMeasurement = 0;
            return true;
        }

        /// <summary>
        /// start the animation test
        /// </summary>
        virtual public void StartAnimationTest()
        {
            // stop the global TimeDriver timer (we have our own timer for the test)
            StopTimer();

            if (((IAnimatable)targetElement) == null)
            {
                throw new InternalDriverException("target element is not an IAnimatable!");
            }

            // start the animation
            if (_currentAnimationClock == null)
            {
                throw new InternalDriverException("no available animation clock!");
            }
            
            // measure values at unique events
            // we just want to make sure that it has changed somehow - higher-fidelity
            // measurements are the charter of the Animations team (as per conversation
            // with test & dev teams)
            
            // this requires a bit of explanation: the DiscreteAnimation class has the unfortunate honor of
            // being able to animate anything that it can - it does this by cycling through a list of canned
            // values for the property. However, for some properties, other pieces of Avalon may be interested 
            // in their values. The way animation works is:
            //      - timer Tick()
            //      - animated property is invalidated
            //      - measure / arrange passes for layout
            //      - everything is rendered
            //      - lather, rinse, repeat
            // In something like the measurement pass, a call to GetCurrentValueCore may or may not be made to
            // the DiscreteAnimation. If you had only two properties available (e.g. a Boolean animation), you 
            // could have a sequence like:
            //      - Tick(): measurement pass asks for property - value 1 is returned
            //      - we ask for current value - value 2 is returned
            //      - Tick(): measurement pass asks for property - value 1 is returned
            //      - we ask for current value - value 2 is returned
            // In this scenario, it appears as if the property is *not animating*. Calling GetValue on a property 
            // animated via a DiscreteAnimation is not reliable. This is mitigated by imposing the restriction 
            // that we have to listen to the PropertyAnimated event on the DiscreteAnimation class, which reports 
            // both the # of times GetCurrentValueCore has been called (TickCount) and the value of the property 
            // (PropertyValue). Between the two, we should have a unique signature that changes from call to call
            // for the animated property. It also means that we record every time the Animation is queried, which is
            // different than with other animation types (e.g. DoubleAnimation). For these other animation types,
            //  this is less of an issue because it's unlikely that we'd get back the same value every other call. 
            //  The only exception is the BooleanAnimation, but we use the DiscreteAnimation class for booleans. 
            
            AnimationTimeline animation = _currentAnimationClock.Timeline;
            if (animation is DiscreteAnimation)
            {
                _currentAnimationClock.CurrentTimeInvalidated += QueryPropertyValue;
                ((DiscreteAnimation)animation).PropertyAnimated += new DiscreteAnimation.PropertyAnimatedEventHandler(OnDiscretePropertyAnimated);
            }
            else
            {
                _currentAnimationClock.CurrentTimeInvalidated += MeasurePropertyValue;
            }

            // start the animation
            _currentAnimationClock.Controller.Begin();
        }


        /// <summary>
        /// end the animation test
        /// </summary>
        virtual public void EndAnimationTest()
        {
            // end the animation
            _currentAnimationClock.Controller.Stop();

            // stop listening - see explanation in StartAnimationTest() for what this is all about.
            AnimationTimeline animation = _currentAnimationClock.Timeline;
            if (animation is DiscreteAnimation)
            {
                _currentAnimationClock.CurrentTimeInvalidated -= QueryPropertyValue;
                ((DiscreteAnimation)animation).PropertyAnimated -= OnDiscretePropertyAnimated;
            }
            else
            {
                _currentAnimationClock.CurrentTimeInvalidated -= MeasurePropertyValue;
            }

            // restart the global TimeDriver timer
            StartTimer();
        }

        /// <summary>
        /// record the value of the animated property from the DiscreteAnimation class
        /// </summary>
        public void OnDiscretePropertyAnimated(object sender, PropertyAnimatedEventArgs args)
        {
            if (currentMeasurement < numberOfMeasurements)
            {
                measurements[currentMeasurement].TickCount = args.TickCount;
                measurements[currentMeasurement].PropertyValue = args.PropertyValue;
                currentMeasurement++;
                //Log("measurement: " + measurements[currentMeasurement - 1].ToString());
            }
        }

        /// <summary>
        /// measure current property using loose binding: calling this will invoke the callback
        /// which we can use to measure the property value.
        /// </summary>
        virtual public void QueryPropertyValue(object source, EventArgs args)
        {
            if ((((Clock)source).CurrentIteration > 1) || (((Clock)source).CurrentGlobalSpeed < 0))
            {
                if (currentMeasurement < numberOfMeasurements)
                {
                    timeManager.Pause();
                    targetElement.GetValue(CurrentProperty.OriginalProperty);
                    timeManager.Resume();
                }
                else
                {
                    EndAnimationTest();
                }
            }
        }

        /// <summary>
        /// measure current property using loose binding
        /// </summary>
        virtual public void MeasurePropertyValue(object source, EventArgs args)
        {
          if ( (((Clock)source).CurrentIteration >1) || (((Clock)source).CurrentGlobalSpeed < 0) )
          {
            if (currentMeasurement < numberOfMeasurements)
            {
                //8-15-06: Validating every other CurrentTimeInvalidated firing.
                if (currentMeasurement % 2 == 0)
                {
                    timeManager.Pause();

                    // it's unlikely that we'd get the same value for the property twice in a row
                    // (as opposed to the DiscreteAnimation), so we just rely on the PropertyValue
                    // to be different between calls to GetValue()
                    measurements[indexMeasurement].TickCount = 0;
                    measurements[indexMeasurement].PropertyValue = targetElement.GetValue(CurrentProperty.OriginalProperty);
                    //Log("measurement: " + measurements[currentMeasurement - 1].ToString());
                    //Log("CurrentTime: " + ((Clock)source).CurrentTime);
                    indexMeasurement++;
                    timeManager.Resume();
                }
                currentMeasurement++;
            }
            else
            {
                EndAnimationTest();
            }
          }
        }

        /// <summary>
        /// Clean up for this sub-step, and report!
        /// </summary>
        virtual public void DoCleanUp()
        {
            // default to failure
            bool localResult = false;

            // build sample string
            string[] samples = new string[ measurements.Length ];
            for ( int i=0; i< measurements.Length; i++)
            {
                if (measurements[i].PropertyValue != null)
                {
                    try
                    {
                        samples[i] = dataAssociation.Translator.GetMarkupRepresentation(
                        CurrentProperty.OriginalProperty.PropertyType.FullName, measurements[i].PropertyValue);
                    }
                    catch( System.Exception)
                    {
                        samples[i] = measurements[i].PropertyValue.ToString();
                    }
                }
                else
                {
                    samples[i] = "(null)" ;
                }
            }


            // measure and log result
 
            if (ValidateMeasurements(measurements))
            {
                if (ValidateControl())
                {
                    localResult = true;
                    RegisterResult(CurrentProperty, CommonConstants.TestPass, "Pass." );
                }
            }
            else
            {
                RegisterResult(CurrentProperty, CommonConstants.TestFail, "Fail." );
            }

            // accumulate results
            testResult &= localResult;
            // clean timing values
            effectTime = startingTime = TimeSpan.Zero;
            // next property
            currentPropertyIndex++;
            // extra cleaning
            measurements = null;
            _currentAnimationClock = null;
        }
 
        virtual protected bool ValidateControl()
        {
            bool controlStillValid = true;

            return controlStillValid;
        }


        /// <summary>
        /// Helper to define equality
        /// </summary>
        virtual protected bool ValidateMeasurements(Measurement[] samples)
        {
            for (int i = 0; i < (indexMeasurement - 1); i++)
            {
                // we should never have a null measurement
                if (samples[i].PropertyValue == null || samples[i + 1].PropertyValue == null)
                {
                    Log2(" validation failure ");
                    Log2("   - measurement " + (samples[i].PropertyValue == null ? i : i + 1) + " was null");
                    return false;
                }

                // our two measurements shouldn't ever be equal, because we're measuring at 
                // the beginning and end of an autoreversing animation that goes forever.
                // However, this can happen even when the animation is running due to the fact 
                // that we may be competing with other parts of Avalon to get the information we 
                // want: we ask for value 1, then they ask for value 2 and then we ask for 
                // value 3, which happens to be identical to value 1 because it's a Boolean animation.
                // This is why the discrete animation also includes a TickCount that is incremented 
                // every time the property is queried, so that we at least have some sense that
                // the animation is successfully running.
                if (samples[i].PropertyValue.Equals(samples[i+1].PropertyValue))
                {
                    Log2(" validation failure comparing PropertyValue:");
                    Log2("   - measurement: " + samples[i].ToString());
                    Log2("   - measurement: " + samples[i + 1].ToString());
                    Log2(" querying tick count:");
                    if (samples[i].TickCount.Equals(samples[i + 1].TickCount))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Heper to print data
        /// </summary>
        virtual protected string PrintSamples( string[] samples )
        {
            string s = "";
            for ( int i=0; i<indexMeasurement; i++)
            {
                s += samples[i] + " ";
            }
            return s;
        }

        /// <summary>
        /// Gets the duration of this animation, in milliseconds
        /// </summary>
        /// <returns>Millisecond duration for this animation.</returns>
        virtual protected Duration GetAnimationDuration()
        {
            // make sure the animation takes a bit less than one test cycle
            return new Duration(TimeSpan.FromMilliseconds((int)(TimeInterval * ( (float)numberOfMeasurements + 1.75F ))));
         }

        /// <summary>
        /// Gets the name of the test
        /// </summary>
        /// <returns>Name of Current Test.</returns>
        public override string Name
        {
            get
            {
                if ( targetElement != null )
                {
                    return base.Name + " on " + targetElement.GetType().Name;
                }
                else return base.Name;
            }
        }

    }



    /// <summary>
    /// This wraps the entire animation test. It uses the threading helper class
    /// to create async events in a timely, orderded manner.
    /// This variation filters a single property of the set
    /// </summary>
    public class FilteredObjectAnimationDriver : ComprehensiveAnimationDriver
    {
        
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="timeInterval">interval for tests in milliseconds</param>
        public FilteredObjectAnimationDriver(string elementFile, int timeInterval): base(elementFile, timeInterval)
        {
        }

        #region base class overrides
        /// <summary>
        /// Determines wether a particular propery must be filtered for this test
        /// </summary>
        /// <param name="ip">the property to filter</param>
        /// <returns>true if the property needs filtering</returns>
        override public bool FilterProperty(IntegrationProperty ip)
        {
            // no filter, use base class criteria
            if (filter == null) return base.FilterProperty(ip);

            // apply local filter
            Match m = filterExpression.Match(ip.ToString());
            if (m.Success)
            {
                // passes our local filter, move up the hierarchy
                return base.FilterProperty(ip);
            }
            // Default is to filter
            RegisterResult(ip, CommonConstants.TestIgnore, "Property does not match filter criteria.");
            return true;
        }
        #endregion


        #region current driver specifics
        /// <summary>
        /// Internal filter parameter
        /// </summary>
        protected string filter = null;
        /// <summary>
        /// Internal regular expression base on the filter
        /// </summary>
        protected Regex filterExpression = null;
        /// <summary>
        /// Public filter regular expression string
        /// </summary>
        public string Filter
        {
            get { return filter; }
            set
            {
                // store local value
                filter = value;
                // create regular expression based on filter string
                if ( filter != null )
                    filterExpression = new Regex(filter);
            }
        }
        #endregion
    }
}



