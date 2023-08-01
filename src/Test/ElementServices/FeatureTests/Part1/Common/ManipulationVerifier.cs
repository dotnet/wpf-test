// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// General verifications for manipulations
    /// 
    /// Segments - 
    ///     a. inertia portions - need some review
    ///     b. combo MP modes - valid and invalid
    ///     c. event sequence - full and/or partial
    ///     d. event routing
    ///     e. hit testing
    ///     f. panning during deltas
    ///     F. MP factory – Beta 2
    /// </summary>
    public class ManipulationVerifier : MultiTouchVerifier
    {
        #region Constructor

        public ManipulationVerifier(UIElement element)
            : base()
        {
            Element = element;
            //currentState = ManipulationState.None;
        }

        #endregion 
        
        #region Properties

        /// <summary>
        /// keep track of the element that's being manipulated
        /// </summary>
        public UIElement Element { get; set; } // 

        #region manipulation events data

        public ManipulationData ManipulationInfo { get; set; }
        public ManipulationStartingData StartingEventData { get; set; }
        public ManipulationStartedData StartedEventData { get; set; }
        public ManipulationDeltaData DeltaEventData { get; set; }
        public ManipulationInertiaStartingData InertiaStartingEventData { get; set; }
        public ManipulationCompletedData CompletedEventData { get; set; }
        public ManipulationBoundaryData BoundaryFeedbackData { get; set; }
        public List<ManipulationData> manipulationDataList; // 
        
        /// <summary>
        /// set the ManipulationData
        /// </summary>
        public List<ManipulationData> ManipulationDataList
        {
            get 
            {
                return manipulationDataList;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("manipulationDataList should not be null!");
                }

                manipulationDataList = value;
            }
        }

        #endregion

        /// <summary>
        /// check if the manipulation is enabled
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool IsManipulationOn
        {
            get
            {
                return Element.IsManipulationEnabled;
            }            
        }

        /// <summary>
        /// check if there is any active manipulation for the Element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool IsManipulationActive
        {
            get
            {
                return Manipulation.IsManipulationActive(Element);
            }
        }

        /// <summary>
        /// Check for single or non-single touch manipulations
        /// </summary>
        public bool AllowsSingleMode
        {
            // 
            get
            {
                return _isSingleTouchEnabled;
            }
            set
            {
                _isSingleTouchEnabled = value; 
            }
        }

        /// <summary>
        /// keep track of the current mode
        /// </summary>
        public ManipulationModes CurrentSupportedMode
        {
            get 
            {
                return _supportedMode; 
            }
            set             
            {
                _supportedMode = value; 
            }
        }

        /// <summary>
        /// Gets the set of manipulations that are allowed
        /// </summary>
        public ManipulationModes AllowedManipulations
        {
            get
            {
                ManipulationModes allowed = ManipulationModes.None;

                if (Allows(ManipulationModes.TranslateX))
                {
                    allowed |= ManipulationModes.TranslateX;
                }
                if (Allows(ManipulationModes.TranslateY))
                {
                    allowed |= ManipulationModes.TranslateY;
                }
                if (Allows(ManipulationModes.Scale) && (this._manipulatorsSinceStart > 1))
                {
                    allowed |= ManipulationModes.Scale;
                }
                if (Allows(ManipulationModes.Rotate) && ((this._manipulatorsSinceStart > 1) || this.HadPivotSinceStart))
                {
                    allowed |= ManipulationModes.Rotate;
                }

                return allowed;
            }
        }

        /// <summary>
        /// track the container
        /// </summary>
        public UIElement Container
        {
            get 
            {
                return _container; 
            }
            set 
            {
                _container = value; 
            }
        }

        /// <summary>
        /// check if we have a pivot set
        /// </summary>
        public bool HadPivotSinceStart
        {
            get 
            {
                return (_originPivot != null);
            }
        }

        /// <summary>
        /// This value is true by default
        /// If complete() is called before all velocities reach zero, the value should be set to false.
        /// </summary>
        public bool HaveAllVelocitiesReachZero
        {
            get { return _haveAllVelocitiesReachZero; }
            set { _haveAllVelocitiesReachZero = value; }
        }

        /// <summary>
        /// starting event counter
        /// </summary>
        public int StartingEventCount 
        {
            get { return _startingEventCount; }   
        }

        /// <summary>
        /// started event counter
        /// </summary>
        public int StartedEventCount
        {
            get { return _startEventCount; }
        }

        /// <summary>
        /// delta event counter
        /// </summary>
        public int DeltaEventCount
        {
            get { return _deltaEventCount; }
        }

        /// <summary>
        /// inertia starting event counter
        /// </summary>
        public int InertiaStartingEventCount
        {
            get { return _inertiaStartingEventCount; }
        }

        /// <summary>
        /// completed event counter
        /// </summary>
        public int CompletedEventCount
        {
            get { return _completedEventCount; }
        }

        /// <summary>
        /// boundary feedback event counter
        /// </summary>
        public int BoundaryFeedbackEventCount
        {
            get { return _boundaryFeedback; }
        }

        #endregion

        #region Public Methods and Events 

        /// <summary>
        /// check if a given manipulation mode is supported by the element
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public bool IsManipulationSupported(ManipulationModes mode)
        {
            return ((ManipulationModes)(this._supportedMode & mode)) == mode;
        }

        /// <summary>
        /// starting event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void VerifyStartingEvent(object sender, ManipulationStartingEventArgs e)
        {
            _startingEventCount++;
            
            // set the data         
            StartingEventData = new ManipulationStartingData(e);

            // set variables
            _isSingleTouchEnabled = StartingEventData.IsSingleTouchEnabled;  
            _supportedMode = StartingEventData.Mode;                                    
            _container = (UIElement)StartingEventData.Container;
            _originPivot = StartingEventData.Pivot;                                        
        }

        /// <summary>
        /// started event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void VerifyStartedEvent(object sender, ManipulationStartedEventArgs e)
        {
            _startEventCount++;

            // get the data
            StartedEventData = new ManipulationStartedData(e);
                    
            // get the manipulation origin 
            _originStart = StartedEventData.Origin;         
          
            // get the container
            var localContainer = StartedEventData.Container;
            Utils.Assert(localContainer == this._container, "In started - the container is incorrect");
        }

        /// <summary>
        /// delta event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void VerifyDeltaEvent(object sender, ManipulationDeltaEventArgs e)
        {
            if (e.IsInertial)
            {
                _deltaInertiaEventCount++;
            }
            else
            {
                _deltaEventCount++;
            }

            // set the current dalta data
            DeltaEventData = new ManipulationDeltaData(e);

            // get the origin
            _originDelta = DeltaEventData.Origin; // 
            
            // get the container 
            _container = (UIElement)DeltaEventData.Container;

            //*******************
            // verifications
            //*******************

            // manipulation data
            var manipulationDelta = DeltaEventData.Delta; 
            var manipulationCumulated = DeltaEventData.Cumulative;              
            var manipulationVelocities = DeltaEventData.Velocites; 

            // cumulating delta
            _translationDeltaCumulated += manipulationDelta.Translation;
            _expansionDeltaCumulated += manipulationDelta.Expansion;
            _scaleDeltaCumulated += manipulationDelta.Scale;  // -1; 
            _rotationDeltaCumulated += manipulationDelta.Rotation;
            ManipulationDelta cumulated = new ManipulationDelta(_translationDeltaCumulated,
                _rotationDeltaCumulated, _scaleDeltaCumulated, _expansionDeltaCumulated);

            // current velocities - used later
            _linearVelocityCurrent = manipulationVelocities.LinearVelocity;
            _angularVelocityCurrent = manipulationVelocities.AngularVelocity;
            _expansionVelocityCurrent = manipulationVelocities.ExpansionVelocity;
        }

        /// <summary>
        /// inertia starting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void VerifyInertiaStartingEvent(object sender, ManipulationInertiaStartingEventArgs e)
        {
            _inertiaStartingEventCount++;

            // set the data
            InertiaStartingEventData = new ManipulationInertiaStartingData(e);

            // set variables
            _inertiaElement = (UIElement)sender;
            _originInertia = InertiaStartingEventData.Origin;
            _container = (UIElement)InertiaStartingEventData.Container;
            _inertiaInitialVelocities = InertiaStartingEventData.InitialVelocites;
        }

        /// <summary>
        /// verify completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void VerifyCompletedEvent(object sender, ManipulationCompletedEventArgs e)
        {
            if (e.IsInertial)
            {
                _completedInertiaEventCount++;
            }
            else
            {
                _completedEventCount++;
            }

            // set the completed data
            CompletedEventData = new ManipulationCompletedData(e);  
                                   
            // set the origin at the completion
            _originCompleted = CompletedEventData.Origin;
            
            ManipulationDelta manipulationTotal;
            manipulationTotal = e.TotalManipulation;

            // Verifications - the total should be close to the accumulated on the element
            Utils.Assert(VerifyVector(manipulationTotal.Translation, DeltaEventData.Cumulative.Translation), string.Format("In Completed - Translation - the total should be close to the sum of the cumulated - Expected = {0}, Actual = {1}", manipulationTotal.Translation, _translationDeltaCumulated));
            Utils.Assert(VerifyVector(manipulationTotal.Expansion, DeltaEventData.Cumulative.Expansion),
                string.Format("In Completed - Expansion - the sum of delta cumulated should be close to the total cumulated - Expected = {0}, Actual = {1}", manipulationTotal.Expansion, _expansionDeltaCumulated));

            //
            Utils.Assert(VerifyVector(manipulationTotal.Scale, DeltaEventData.Cumulative.Scale),
                string.Format("Scale - the sum of delta cumulated should be close to the total cumulated - Expected = {0}, Actual = {1}", manipulationTotal.Scale, _scaleDeltaCumulated));
            
            Utils.AssertAreClose(manipulationTotal.Rotation, DeltaEventData.Cumulative.Rotation, DoubleUtil.epsilon,
                string.Format("In Completed - Rotation - the sum of delta cumulated should be close to the total cumulated - Expected = {0}, Actual = {1}", manipulationTotal.Rotation, _rotationDeltaCumulated));

            // other verifications
            if (e.IsInertial)
            {
                //note: this won't work if the inertia was completed before the velocities reach zero 
                if (HaveAllVelocitiesReachZero)
                {
                    Utils.AssertAreClose(Math.Round(CompletedEventData.Velocites.AngularVelocity,1), 0, DoubleUtil.epsilon, "VerifyCompletedEvent: AngularVelocity is not expected!!");
                    Utils.Assert(CompletedEventData.Velocites.ExpansionVelocity == new Vector(0, 0), "VerifyCompletedEvent: ExpansionVelocity is not expected!!");
                    Utils.AssertAreClose(Math.Round(CompletedEventData.Velocites.LinearVelocity.X,1), 0, DoubleUtil.epsilon, "VerifyCompletedEvent: LinearVelocity.X is not expected!!");
                    Utils.AssertAreClose(Math.Round(CompletedEventData.Velocites.LinearVelocity.Y,1), 0, DoubleUtil.epsilon, "VerifyCompletedEvent: LinearVelocity.Y is not expected!");

                    VerifyExpansion();
                    VerifyRotation();
                }
            }
            else
            {
                // no inertia
                VerifyOrigin();
                VerifyManipulations(e, (UIElement)sender);
                VerifyVelocities();
            }
        }

        /// <summary>
        /// manipulation boundaryfeedback 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void VerifyManipulationBoundaryFeedbackEvent(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            _boundaryFeedback++;

            if (e.BoundaryFeedback != null)
            {
                BoundaryFeedbackData = new ManipulationBoundaryData(e);
            }
        }

        #endregion

        #region Private Manipulation Methods

        /// <summary>
        /// verify the sender
        /// </summary>
        /// <param name="sender"></param>
        private void VerifySender(object sender)
        {
            Utils.AssertEqual(Element, (UIElement)sender, "The sender should be the Element", null); //
        }

        /// <summary>
        /// Verify manipulations at Completed w/o inertia  
        /// 
        /// 



        private void VerifyManipulations(ManipulationCompletedEventArgs e, UIElement element)
        {          
            var totalManipulation = e.TotalManipulation;  
            
            // verify the mode
            if (totalManipulation.Translation.X != 0)
            {
                Utils.Assert(IsManipulationSupported(ManipulationModes.TranslateX), string.Format(CultureInfo.InvariantCulture, "VerifyManipulations: ManipulationModes.TranslateX should be enabled"));
            }

            if (totalManipulation.Translation.Y != 0)
            {
                Utils.Assert(IsManipulationSupported(ManipulationModes.TranslateY), string.Format(CultureInfo.InvariantCulture, "VerifyManipulations: ManipulationModes.TranslateY should be enabled"));
            }

            if (totalManipulation.Rotation != 0)
            {
                Utils.Assert(IsManipulationSupported(ManipulationModes.Rotate), string.Format(CultureInfo.InvariantCulture, "VerifyManipulations: ManipulationModes.Rotation should be enabled"));
            }

            if (totalManipulation.Scale.X != 0 || totalManipulation.Scale.Y != 0) // should be uniformed
            {
                Utils.Assert(IsManipulationSupported(ManipulationModes.Scale), string.Format(CultureInfo.InvariantCulture, "VerifyManipulations: ManipulationModes.Scale should be enabled"));
            }

            if (totalManipulation.Expansion.X != 0 || totalManipulation.Expansion.Y != 0) // should be uniformed
            {
                Utils.Assert(IsManipulationSupported(ManipulationModes.Scale), string.Format(CultureInfo.InvariantCulture, "VerifyManipulations: ManipulationModes.Scale should be enabled"));
            }

            // verify related manipulations
            if (WantsManipulation(_supportedMode))
            {
                if (_supportedMode == ManipulationModes.All)
                {
                    VerifyAllEnabled();
                }
                else if (_supportedMode == ManipulationModes.Rotate)
                {
                    VerifyOnlyRotateEnabled();
                }
                else if (_supportedMode == ManipulationModes.TranslateX)
                {
                    VerifyOnlyTranslateXEnabled();
                }
                else if (_supportedMode == ManipulationModes.TranslateY)
                {
                    VerifyOnlyTranslateYEnabled();
                }
                else if ((_supportedMode == (ManipulationModes.TranslateX | ManipulationModes.TranslateY)) ||
                            this._isSingleTouchEnabled)  
                {
                    VerifyOnlyTranslationEnabled();
                }
                else if (_supportedMode == ManipulationModes.Scale)
                {
                    VerifyOnlyScaleEnabled();
                }
            }
            else // none
            {
                VerifyNoneEnabled();
            }             
        }

        /// <summary>
        /// General verificaions for ManipulationMode == ManipulationModes.All
        /// </summary>
        private void VerifyAllEnabled()
        {
            Utils.Assert(VerifyVector(_translationDeltaCumulated, CompletedEventData.Total.Translation),
                string.Format("VerifyAllEnabled: Translation - the total should be close to the sum of the cumulated - Expected = {0}, Actual = {1}", _translationDeltaCumulated, CompletedEventData.Total.Translation));
            Utils.Assert(VerifyVector(_expansionDeltaCumulated, CompletedEventData.Total.Expansion),
                string.Format("VerifyAllEnabled: Expansion - the total should be close to the sum of the cumulated - Expected = {0}, Actual = {1}", _expansionDeltaCumulated, CompletedEventData.Total.Expansion));
            
            //


            
            Utils.AssertAreClose(_rotationDeltaCumulated, CompletedEventData.Total.Rotation, DoubleUtil.epsilon,
                string.Format("VerifyAllEnabled: Rotation - the total should be close to the sum of the cumulated - Expected = {0}, Actual = {1}", _rotationDeltaCumulated, CompletedEventData.Total.Rotation));
            Utils.Assert(VerifyVector(_linearVelocityCurrent, CompletedEventData.Velocites.LinearVelocity),
                string.Format("VerifyAllEnabled: LinearVelocity - the total should be close to the sum of the cumulated - Expected = {0}, Actual = {1}", _linearVelocityCurrent, CompletedEventData.Velocites.LinearVelocity));
            Utils.AssertAreClose(_angularVelocityCurrent, CompletedEventData.Velocites.AngularVelocity, DoubleUtil.epsilon,
                string.Format("VerifyAllEnabled: AngularVelocity - the total should be close to the sum of the cumulated - Expected = {0}, Actual = {1}", _angularVelocityCurrent, CompletedEventData.Velocites.AngularVelocity));
            Utils.Assert(VerifyVector(_expansionVelocityCurrent, CompletedEventData.Velocites.ExpansionVelocity),
                string.Format("VerifyAllEnabled: ExpansionVelocity - the total should be close to the sum of the cumulated - Expected = {0}, Actual = {1}", _expansionVelocityCurrent, CompletedEventData.Velocites.ExpansionVelocity));
        }

        /// <summary>
        /// General verificaions for ManipulationModes.None
        /// </summary>
        private void VerifyNoneEnabled()
        {
            Utils.Assert(CompletedEventData.Total == new ManipulationDelta(new Vector(0, 0), 0, new Vector(0, 0), new Vector(0, 0)), "VerifyNoneEnabled - Total Manipulation is not expected!");
            Utils.Assert(CompletedEventData.Velocites == new ManipulationVelocities(new Vector(0, 0), 0, new Vector(0, 0)), "VerifyNoneEnabled - Total Velocites is not expected!");
        }

        /// <summary>
        /// General verificaions for ManipulationModes.Scale
        /// </summary>
        private void VerifyOnlyScaleEnabled()
        {
            Utils.Assert(CompletedEventData.Total.Translation == new Vector(0, 0), "VerifyOnlyScaleEnabled - Total Translation is not expected!");
            Utils.AssertAreClose(0.0, CompletedEventData.Total.Rotation, DoubleUtil.epsilon, "VerifyOnlyScaleEnabled - Total Rotation is not expected!");
            Utils.Assert(new Vector(0, 0) == CompletedEventData.Total.Expansion, "VerifyOnlyScaleEnabled - Total Expansion is not expected!");
            Utils.Assert(CompletedEventData.Velocites == new ManipulationVelocities(new Vector(0, 0), 0, new Vector(0, 0)), "VerifyOnlyScaleEnabled - Total Velocites is not expected!");
        }

        /// <summary>
        /// general verificaitons for only Translate enable (TranslateX && TranslateY).  
        /// </summary>
        private void VerifyOnlyTranslationEnabled()
        {
            Utils.AssertAreClose(0.0, CompletedEventData.Total.Rotation, DoubleUtil.epsilon, "VerifyOnlyTranslationEnabled - Total Rotation is not expected!");
            Utils.Assert(new Vector(0, 0) == CompletedEventData.Total.Scale, "VerifyOnlyTranslationEnabled - Total Scale is not expected!");

            Utils.Assert(new Vector(0, 0) == CompletedEventData.Total.Expansion, "VerifyOnlyTranslationEnabled - Total Expansion is not expected!");
            Utils.Assert(CompletedEventData.Velocites == new ManipulationVelocities(new Vector(0, 0), 0, new Vector(0, 0)), "VerifyOnlyTranslationEnabled - Total Velocites is not expected!");
        }

        /// <summary>
        ///General verificaitions for ManipulationModes.TranslateX
        /// </summary>
        private void VerifyOnlyTranslateXEnabled()
        {
            Utils.AssertAreClose(0.0, CompletedEventData.Total.Translation.Y, DoubleUtil.epsilon, "VerifyOnlyTranslateXEnabled - Total Translation Y is not expected!");
            Utils.AssertAreClose(0.0, CompletedEventData.Total.Rotation, DoubleUtil.epsilon, "VerifyOnlyTranslateXEnabled - Total Rotation is not expected!");
            Utils.Assert(new Vector(0, 0) == CompletedEventData.Total.Scale, "VerifyOnlyTranslateXEnabled - Total Scale is not expected!");
            Utils.Assert(new Vector(0, 0) == CompletedEventData.Total.Expansion, "VerifyOnlyTranslateXEnabled - Total Expansion is not expected!");
            Utils.Assert(CompletedEventData.Velocites == new ManipulationVelocities(new Vector(0, 0), 0, new Vector(0, 0)), "VerifyOnlyTranslateXEnabled - Total Velocites is not expected!");
        }

        /// <summary>
        /// General verificaitions for ManipulationModes.TranslateY
        /// </summary>
        private void VerifyOnlyTranslateYEnabled()
        {
            Utils.AssertAreClose(0.0, CompletedEventData.Total.Translation.X, DoubleUtil.epsilon, "VerifyOnlyTranslateYEnabled - Total Translation X is not expected!");
            Utils.AssertAreClose(0.0, CompletedEventData.Total.Rotation, DoubleUtil.epsilon, "VerifyOnlyTranslateYEnabled - Total Rotation is not expected!");
            Utils.Assert(new Vector(0, 0) == CompletedEventData.Total.Scale, "VerifyOnlyTranslateYEnabled - Total Scale is not expected!");
            Utils.Assert(new Vector(0, 0) == CompletedEventData.Total.Expansion, "VerifyOnlyTranslateYEnabled - Total Expansion is not expected!");
            Utils.Assert(CompletedEventData.Velocites == new ManipulationVelocities(new Vector(0, 0), 0, new Vector(0, 0)), "VerifyOnlyTranslateYEnabled - Total Velocites is not expected!");
        }

        /// <summary>
        /// general verificaiton for ManipulationModes.Rotate 
        /// </summary>
        private void VerifyOnlyRotateEnabled()
        {
            Utils.Assert(CompletedEventData.Total.Translation == new Vector(0, 0), "VerifyOnlyRotateEnabled - Total Translation is not expected!");
            Utils.Assert(new Vector(0, 0) == CompletedEventData.Total.Scale, "VerifyOnlyRotateEnabled - Total Scale is not expected!");
            Utils.Assert(new Vector(0, 0) == CompletedEventData.Total.Expansion, "VerifyOnlyRotateEnabled - Total Expansion is not expected!");
            Utils.Assert(CompletedEventData.Velocites == new ManipulationVelocities(new Vector(0, 0), 0, new Vector(0, 0)), "VerifyOnlyRotateEnabled - Total Velocites is not expected!");
        }

        /// <summary>
        /// Check the velocities between last delta event and the completed event - all velocities should reach 0 by the last completed
        /// </summary>
        private void VerifyVelocities()
        {
            // 
            Utils.Assert(CompletedEventData.Velocites.LinearVelocity == _linearVelocityCurrent, "VerifyVelocities - Total LinearVelocity is not correct!");
            Utils.AssertAreClose(CompletedEventData.Velocites.AngularVelocity, _angularVelocityCurrent, DoubleUtil.epsilon, "VerifyVelocities - Total AngularVelocity is not correct!");
            Utils.Assert(CompletedEventData.Velocites.ExpansionVelocity == _expansionVelocityCurrent, "VerifyVelocities - Total ExpansionVelocity is not correct!");
        }
        
        /// <summary>
        /// verify the manipulation origin from started to completed
        /// </summary>
        private void VerifyOrigin() 
        {
            double actual = (double)(_originCompleted.X - _originStart.X);
            //
        }

        /// <summary>
        /// verify 
        /// </summary>
        private void VerifyBoundaryFeedback()
        {
            ManipulationDelta bf = BoundaryFeedbackData.BoundaryFeedback;
            Utils.Assert(new Vector(0, 0) == bf.Scale, "VerifyBoundaryFeedback - Scale is not expected!");
            Utils.Assert(new Vector(0,0) == bf.Expansion, "VerifyBoundaryFeedback - Expansion is not expected!");
            Utils.AssertAreClose(0.0, bf.Rotation, DoubleUtil.epsilon, "VerifyBoundaryFeedback - Rotation is not expected!");
        }

        #endregion
        
        #region Private Inertia Methods

        /// <summary>
        /// Verify that the Manipulation origin is correct
        /// </summary>
        /// <param name="expectedOrigin"></param>
        private void VerifyManipulationOrigin(Point expectedOrigin)
        {
            double actual = (double)(CompletedEventData.Origin.X - expectedOrigin.X);            
            Utils.Assert(DoubleUtil.LessThan(actual, DoubleUtil.epsilon), "VerifyManipulationOrigin: Manipulation Origin.X is not at the correct range!");

            actual = (double)(CompletedEventData.Origin.Y - expectedOrigin.Y);
            Utils.Assert(DoubleUtil.LessThan(actual, DoubleUtil.epsilon), "VerifyManipulationOrigin: Manipulation Origin.Y is not at the correct range!");
        }
        
        /// <summary>
        /// Verify the expansion against the expected value within given default epsilon 
        /// </summary>
        /// <param name="expectedExpansion"></param>
        private void VerifyExpansion(Vector expectedExpansion)
        {
            //ONLY USE X here!!!
            double expansionError = Math.Abs(expectedExpansion.X - CompletedEventData.Total.Expansion.X);
            Utils.Assert(DoubleUtil.LessThan(expansionError, DoubleUtil.epsilon), 
                string.Format("The expansion error is not within error margin, CurrentError = {0}, Epsilon = {1}", expansionError, DoubleUtil.epsilon));
        }

        /// <summary>
        /// Verify Manipulation Origin when all velocity reach to zero
        /// NOTE - given product change with Surface scnearios this is temp outdated
        /// </summary>
        private void VerifyInertiaOrigin()
        {
            double x = _originInertia.X + CompletedEventData.Total.Translation.X;
            double y = _originInertia.Y + CompletedEventData.Total.Translation.Y;
            VerifyManipulationOrigin(new Point(x, y));
        }        

        /// <summary>
        /// Verify rotation when all velocity reach to zero
        /// </summary>
        private void VerifyRotation()
        {
            double currentDiff = 0;
            double rotationEpsilon = 0.1;

            if ((InertiaStartingEventData.InitialVelocites != null) && 
                !double.IsNaN(InertiaStartingEventData.InitialVelocites.AngularVelocity))
            {
                var behavior = InertiaStartingEventData.RotationBehavior;

                if (!double.IsNaN(behavior.DesiredRotation) &&
                    DoubleUtil.IsDoubleFinite(behavior.DesiredRotation))
                {
                    currentDiff = Math.Abs(behavior.DesiredRotation - CompletedEventData.Total.Rotation);
                    Utils.Assert(DoubleUtil.LessThan(currentDiff, rotationEpsilon), 
                        string.Format("VerifyRotation: Total Rotation is not at expected range. CurrentError = {0}, Error margin = {1}", currentDiff, rotationEpsilon));
                }
                else if (!double.IsNaN(behavior.DesiredDeceleration) &&
                    DoubleUtil.IsDoubleFinite(behavior.DesiredDeceleration))
                {
                    double totalRotation = (double)(Math.Pow(InertiaStartingEventData.InitialVelocites.AngularVelocity, 2) / (2 * behavior.DesiredDeceleration));
                    currentDiff = Math.Abs(CompletedEventData.Total.Rotation - totalRotation);
                    Utils.Assert(DoubleUtil.LessThan(currentDiff, rotationEpsilon), 
                        string.Format("VerifyRotation: AngularDeceleration is not at expected range. CurrentError = {0}, Error margin = {1}", currentDiff, rotationEpsilon));
                 }
            }
        }

        /// <summary>
        /// Verify expansion after the velocities reach 0.
        /// </summary>
        private void VerifyExpansion()
        {
            if (InertiaStartingEventData.InitialVelocites != null) 
            {
                var behavior = InertiaStartingEventData.ExpansionBehavior;

                if (DoubleUtil.IsDoubleFinite(behavior.DesiredExpansion.X) &&
                    DoubleUtil.IsDoubleFinite(behavior.DesiredExpansion.Y))
                {
                    VerifyExpansion(behavior.DesiredExpansion);
                }
                else if (DoubleUtil.IsDoubleFinite(behavior.DesiredDeceleration))
                {
                    double locX = (Math.Pow(InertiaStartingEventData.InitialVelocites.ExpansionVelocity.X, 2) / (2 * behavior.DesiredDeceleration));
                    VerifyExpansion(new Vector(locX, locX));
                }
            }
        }

        private bool VerifyVector(Vector expected, Vector actual)
        {
            double currentDiffX = 0;
            double currentDiffY = 0;
            bool isClose = true; 

            currentDiffX = Math.Abs(expected.X - actual.X);
            currentDiffY = Math.Abs(expected.Y - actual.Y);
            
            if (expected.X != 0 && expected.Y != 0)
            {
                if ((currentDiffX / expected.X) < 0.15 && (currentDiffY / expected.Y) < 0.15)
                {
                    isClose = true;
                }
                else 
                {
                    isClose = false;
                }
            }

            return isClose; 
        }

        #endregion
        
        #region Local Helpers
        
        /// <summary>
        /// Get the visual parent of the element passed in
        /// </summary>
        /// <param name="element">the element</param>
        /// <returns>the visual parent</returns>
        private static UIElement GetParent(object element)
        {
            return (UIElement)VisualTreeHelper.GetParent((UIElement)element);
        }

        /// <summary>
        /// reset all counters
        /// </summary>
        private void ResetAll()
        {
            _startingEventCount = 0;
            _startEventCount = 0;
            _deltaEventCount = 0;
            _inertiaStartingEventCount = 0;
            _completedEventCount = 0;
            _boundaryFeedback = 0; 
        }

        /// <summary>
        /// Make sure UIElement tested wants manipulations
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        private static bool WantsManipulation(ManipulationModes mode)
        {
            return (mode & (ManipulationModes.Translate | ManipulationModes.Scale | ManipulationModes.Rotate)) != ManipulationModes.None;
        }

        /// <summary>
        /// check whether all the specified manipulations are allowed
        /// </summary>
        /// <param name="manipulations"></param>
        /// <returns></returns>
        private bool Allows(ManipulationModes manipulations)
        {
            return (manipulations & this._supportedMode) == manipulations;
        }

        /// <summary>
        /// Detects whether a point falls within a registered element
        /// </summary>
        private static UIElement HitTestRegistered(Visual visual)
        {
            while (visual != null)
            {
                UIElement element = visual as UIElement;

                // NOTE - THIS WOULD NOT WORK W/O AN ACTIVE MANIPULATION
                if ((element != null) && WantsManipulation(Manipulation.GetManipulationMode(element))) 
                {
                    return element;
                }
                else
                {
                    visual = VisualTreeHelper.GetParent(visual) as Visual;
                }
            }

            return null;
        }

        /// <summary>
        /// expresses rotation in the specified degrees per second
        /// </summary>
        /// <param name="degreesPerSecond"></param>
        /// <returns></returns>
        private static Func<long, double> Rotate(double degreesPerSecond)
        {
            return (timestamp) =>
            {
                double seconds = (double)timestamp / (double)oneSecond;
                return degreesPerSecond * seconds;
            };
        }

        /// <summary>
        /// Gets whether a pivot is usable
        /// </summary>
        /// <param name="pivot"></param>
        /// <returns></returns>
        private static bool IsUsablePivot(ManipulationPivot pivot)
        {
            return (pivot != null)
                && !double.IsNaN(pivot.Center.X) && !double.IsInfinity(pivot.Center.X)
                && !double.IsNaN(pivot.Center.Y) && !double.IsInfinity(pivot.Center.Y)
                && (!double.IsNaN(pivot.Radius) && (double.IsInfinity(pivot.Radius) || (pivot.Radius < 0.0D)));
        }

        #endregion

        #region Fields

        // counters
        private int _startingEventCount = 0;
        private int _startEventCount = 0;
        private int _deltaEventCount = 0;
        private int _inertiaStartingEventCount = 0;
        private int _completedEventCount = 0;
        private int _boundaryFeedback = 0;

        private int _deltaInertiaEventCount = 0;
        private int _completedInertiaEventCount = 0;

        private int _manipulatorsSinceStart = 0; 

        // cumulated manipulation
        private Vector _translationDeltaCumulated = new Vector(0, 0);
        private double _rotationDeltaCumulated = 0;
        private Vector _expansionDeltaCumulated = new Vector(0, 0);
        private Vector _scaleDeltaCumulated = new Vector(0, 0);

        // current velocities
        private Vector _linearVelocityCurrent = new Vector(0, 0);
        private double _angularVelocityCurrent = 0;
        private Vector _expansionVelocityCurrent = new Vector(0, 0);

        // manipulation origins
        private Point _originStart = new Point(0, 0);
        private Point _originDelta = new Point(0, 0);
        private Point _originCompleted = new Point(0, 0);        
        private Point _originInertia = new Point(0, 0);

        private ManipulationVelocities _inertiaInitialVelocities;
        private ManipulationPivot _originPivot; // pivot from the starting

        // indicators
        private bool _isSingleTouchEnabled = false;
        private bool _haveAllVelocitiesReachZero = true;

        // elements
        //
        private UIElement _inertiaElement; // 
        private UIElement _container;

        // mode
        private ManipulationModes _supportedMode = ManipulationModes.All; 

        // consts - manipulation processor needs
        private const long oneSecond = 10000000; // # of timestamp ticks in 1 sec     
        private const double minProcessorRadius = 20.0; // the manipulator's value
        private const double maxProcessorSmoothingRadius = 10.0 * minProcessorRadius;
        private const double spatialScale = 4.0 * maxProcessorSmoothingRadius;

        #endregion               

    }
}
