// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// A event log class for touch, both frame based and non-frame based, and manipulations,
    /// which use TouchEventLog, FrameEventLog and ManipulationEventLog defined below
    /// </summary>
    public class MultiTouchEventLog
    {
        #region Public Lists for various logs

        public List<TouchEventLog> AddedEvents = new List<TouchEventLog>();
        public List<TouchEventLog> ChangedEvents = new List<TouchEventLog>();
        public List<TouchEventLog> RemovedEvents = new List<TouchEventLog>();
        public List<TouchEventLog> TappedEvents = new List<TouchEventLog>();
        public List<TouchEventLog> HoldEvents = new List<TouchEventLog>();        
        public List<TouchEventLog> TouchEnter = new List<TouchEventLog>();
        public List<TouchEventLog> TouchLeave = new List<TouchEventLog>();
        public List<TouchEventLog> GotTouchCapture = new List<TouchEventLog>();
        public List<TouchEventLog> LostTouchCapture = new List<TouchEventLog>();
                
        public List<FrameEventLog> FrameEvents = new List<FrameEventLog>();
        
        public List<ManipulationEventLog> ManipulationStartingEvents = new List<ManipulationEventLog>();
        public List<ManipulationEventLog> ManipulationStartedEvents = new List<ManipulationEventLog>();
        public List<ManipulationEventLog> ManipulationDeltaEvents = new List<ManipulationEventLog>();
        public List<ManipulationEventLog> ManipulationCompleteEvents = new List<ManipulationEventLog>();
        public List<ManipulationEventLog> ManipulationInertiaStartingEvents = new List<ManipulationEventLog>();
        public List<ManipulationEventLog> ManipulationBoundaryFeedbackEvents = new List<ManipulationEventLog>();

        #endregion

        #region Touch

        public void LogTouchAdded(object sender, TouchEventArgs e)
        {
            lock (AddedEvents)
            {
                TouchEventLog log = new TouchEventLog(e);
                AddedEvents.Add(log);
            }
        }

        public void LogTouchUpdated(object sender, TouchEventArgs e)
        {
            lock (ChangedEvents)
            {
                TouchEventLog log = new TouchEventLog(e);
                ChangedEvents.Add(log);
            }

        }

        public void LogTouchRemoved(object sender, TouchEventArgs e)
        {
            lock (RemovedEvents)
            {
                TouchEventLog log = new TouchEventLog(e);
                RemovedEvents.Add(log);
            }
        }

        public void LogTouchTapped(object sender, TouchEventArgs e)
        {
            lock (TappedEvents)
            {
                TouchEventLog log = new TouchEventLog(e);
                TappedEvents.Add(log);
            }
        }
        
        public void LogTouchHold(object sender, TouchEventArgs e)
        {
            lock (HoldEvents)
            {
                TouchEventLog log = new TouchEventLog(e);
                HoldEvents.Add(log);
            }
        }
        public void LogTouchEnter(object sender, TouchEventArgs e)
        {
            lock (TouchEnter)
            {
                TouchEventLog log = new TouchEventLog(e);
                TouchEnter.Add(log);
            }
        }
        
        public void LogTouchLeave(object sender, TouchEventArgs e)
        {
            lock (TouchLeave)
            {
                TouchEventLog log = new TouchEventLog(e);
                TouchLeave.Add(log);
            }
        }
        
        public void LogGotTouchCapture(object sender, TouchEventArgs e)
        {
            lock (GotTouchCapture)
            {
                TouchEventLog log = new TouchEventLog(e);
                GotTouchCapture.Add(log);
            }
        }
        
        public void LogLostTouchCapture(object sender, TouchEventArgs e)
        {
            lock (LostTouchCapture)
            {
                TouchEventLog log = new TouchEventLog(e);
                LostTouchCapture.Add(log);
            }
        }

        #endregion

        #region Framebased

        public void LogFrameReceived(object sender, TouchFrameEventArgs e)
        {
            lock (FrameEvents)
            {           
                FrameEventLog log = new FrameEventLog();
                //
                FrameEvents.Add(log);
            }
        }

        #endregion

        #region Manipulations

        public void LogManipulationStarting(object sender, ManipulationStartingEventArgs e)
        { 
            lock (ManipulationStartingEvents)
            {
                GlobalLog.LogEvidence(string.Format("--- {0}, MP Starting", DateTime.Now.ToLongTimeString()));
                ManipulationEventLog log = new ManipulationEventLog(e);
                ManipulationStartingEvents.Add(log);          
            }
        }
        
        public void LogManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            lock (ManipulationStartedEvents)
            {
                GlobalLog.LogEvidence(string.Format("--- {0}, MP Started", DateTime.Now.ToLongTimeString()));
                ManipulationEventLog log = new ManipulationEventLog(e);
                ManipulationStartedEvents.Add(log);
            }
        }
        
        public void LogManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            lock (ManipulationDeltaEvents)
            {
                ManipulationEventLog log = new ManipulationEventLog(e);
                ManipulationDeltaEvents.Add(log);
            }
        }
        
        public void LogManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            lock (ManipulationCompleteEvents)
            {
                GlobalLog.LogEvidence("--- {0}, MP Completed", DateTime.Now.ToLongTimeString());
                ManipulationEventLog log = new ManipulationEventLog(e);
                ManipulationCompleteEvents.Add(log);
            }
        }
        
        public void LogManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            lock (ManipulationInertiaStartingEvents)
            {
                ManipulationEventLog log = new ManipulationEventLog(e);
                ManipulationInertiaStartingEvents.Add(log);
            }
        }
        
        public void LogManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            lock (ManipulationBoundaryFeedbackEvents)
            {
                ManipulationEventLog log = new ManipulationEventLog(e);
                ManipulationBoundaryFeedbackEvents.Add(log);
            }
        }

        #endregion
    }

    /// <summary>
    /// a struct encapsulating the touch events
    /// </summary>
    public struct TouchEventLog
    {
        public TouchEventLog(TouchEventArgs e)
        {
            Time = DateTime.Now;
            
            this.touchDevice = e.TouchDevice; 
            Id = e.TouchDevice.Id;

            //



        }

        public DateTime Time;
        public int Id;
        public TouchDevice touchDevice;

        //



    }
    
    /// <summary>
    /// a struct encapsulating frame event
    /// </summary>
    public struct FrameEventLog
    {
        public DateTime Time;
        public long TimeStamp;
        public TouchPoint PrimaryTouchPoint;
        public TouchPointCollection TouchPoints;
        public IInputElement RelativeTo;
    }

    /// <summary>
    /// a class encapsulating the manipulation events
    /// </summary>
    public class ManipulationEventLog
    {
        #region Fields 

        //
        public bool IsSingleManipulationEnabled;
        public UIElement Container;
        public ManipulationModes ManipulationMode; 

        public double ManipulationOriginX;
        public double ManipulationOriginY;
        
        public double DeltaX;
        public double DeltaY;
        public Vector ScaleDelta;
        public double RotationDelta;
        public Vector ExpansionDelta;

        public double CumulativeTranslationX;
        public double CumulativeTranslationY;
        public Vector CumulativeScale;
        public double CumulativeRotation;
        public Vector CumulativeExpansion;
        
        public double VelocityX;
        public double VelocityY;        
        public double AngularVelocity;
        public Vector ExpansionVelocity;

        public InertiaExpansionBehavior ExpansionBehavior;
        public InertiaRotationBehavior RotationBehavior;
        public InertiaTranslationBehavior TranslationBehavior;

        public ManipulationDelta Feedback;

        #endregion

        #region Constructors

        public ManipulationEventLog(ManipulationStartingEventArgs e)
        {
            Container = (UIElement)e.ManipulationContainer;
            IsSingleManipulationEnabled = e.IsSingleTouchEnabled;
            ManipulationMode = e.Mode;
        }

        public ManipulationEventLog(ManipulationStartedEventArgs e)
        {
            ManipulationOriginX = e.ManipulationOrigin.X;
            ManipulationOriginY = e.ManipulationOrigin.Y;
        }

        public ManipulationEventLog(ManipulationDeltaEventArgs e)
        {
            ManipulationOriginX = e.ManipulationOrigin.X;
            ManipulationOriginY = e.ManipulationOrigin.Y;

            DeltaX = e.DeltaManipulation.Translation.X;
            DeltaY = e.DeltaManipulation.Translation.Y;
            ScaleDelta = e.DeltaManipulation.Scale; 
            RotationDelta = e.DeltaManipulation.Rotation;
            ExpansionDelta = e.DeltaManipulation.Expansion;

            CumulativeTranslationX = e.CumulativeManipulation.Translation.X;
            CumulativeTranslationY = e.CumulativeManipulation.Translation.Y;
            CumulativeScale = e.CumulativeManipulation.Scale;
            CumulativeExpansion = e.CumulativeManipulation.Expansion;
            CumulativeRotation = e.CumulativeManipulation.Rotation;

            VelocityX = e.Velocities.LinearVelocity.X;
            VelocityY = e.Velocities.LinearVelocity.Y;
            ExpansionVelocity = e.Velocities.ExpansionVelocity;
            AngularVelocity = e.Velocities.AngularVelocity;
        }

        public ManipulationEventLog(ManipulationInertiaStartingEventArgs e)
        { 
            Container = (UIElement)e.ManipulationContainer;

            ManipulationOriginX = e.ManipulationOrigin.X;
            ManipulationOriginY = e.ManipulationOrigin.Y;

            VelocityX = e.InitialVelocities.LinearVelocity.X;
            VelocityY = e.InitialVelocities.LinearVelocity.Y;
            ExpansionVelocity = e.InitialVelocities.ExpansionVelocity;
            AngularVelocity = e.InitialVelocities.AngularVelocity;

            ExpansionBehavior = e.ExpansionBehavior;
            TranslationBehavior = e.TranslationBehavior;
            RotationBehavior = e.RotationBehavior;            
        }

        public ManipulationEventLog(ManipulationCompletedEventArgs e)
        {
            ManipulationOriginX = e.ManipulationOrigin.X;
            ManipulationOriginY = e.ManipulationOrigin.Y;

            CumulativeTranslationX = e.TotalManipulation.Translation.X;
            CumulativeTranslationY = e.TotalManipulation.Translation.Y;
            CumulativeScale = e.TotalManipulation.Scale;
            CumulativeExpansion = e.TotalManipulation.Expansion;
            CumulativeRotation = e.TotalManipulation.Rotation;

            VelocityX = e.FinalVelocities.LinearVelocity.X;
            VelocityY = e.FinalVelocities.LinearVelocity.Y;
            ExpansionVelocity = e.FinalVelocities.ExpansionVelocity;
            AngularVelocity = e.FinalVelocities.AngularVelocity;
        }

        public ManipulationEventLog(ManipulationBoundaryFeedbackEventArgs e)
        { 
            // 
            Container = (UIElement)e.ManipulationContainer; 
            Feedback = e.BoundaryFeedback;         
        }

        #endregion
    }
}
