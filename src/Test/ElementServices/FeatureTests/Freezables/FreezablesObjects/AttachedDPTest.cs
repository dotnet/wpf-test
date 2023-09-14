// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2005
 *
 *   Program:   AttachedDPTest  - DP == DependencyProperty
 
 *
 ************************************************************/
//*****************************************************************************************
//This test does the following things:
//For each freezable created from the script (xtc) 
//            Perform set/get attachedDP, and verify
//            Perform copy and verify
//            Perform freeze and verify
//            Perform set on freezableDP, and verify the changed event
//            If freezable is also animatable, then perform CloneCurrentValue and verify
//******************************************************************************************
using System;
using System.Xml;
using System.Security;
using System.Windows;
using System.Windows.Media;

using Microsoft.Test.ElementServices.Freezables.Objects;
using Microsoft.Test.Logging;


namespace Microsoft.Test.ElementServices.Freezables
{
    /**********************************************************************************
    * CLASS:          AttachedDPTest
    **********************************************************************************/
    public class AttachedDPTest: FreezablesObjectsBase
    {
        #region Private Data
        private bool _isEventFired;
        private DependencyProperty _booAP;
        private DependencyProperty _referenceAP;
        private DependencyProperty _freezableAP;
        #endregion


        #region Constructor
        /******************************************************************************
        * Function:          Constructor
        ******************************************************************************/
        public AttachedDPTest(string testName, string objName) : base(testName, objName)
        {
            // setup attached properties
            
            _booAP  = DependencyProperty.RegisterAttached(
                              "Bool",
                              typeof(bool),
                              typeof(AttachedDPTest)
                              );
            _referenceAP = DependencyProperty.RegisterAttached(
                    "Reference",
                    typeof(object),
                    typeof(AttachedDPTest)
                    );

            _freezableAP = DependencyProperty.RegisterAttached(
                    "Freezable",
                    typeof(Pen),
                    typeof(AttachedDPTest)
                    );

            Perform();
        }
        #endregion


        #region Public and Protected Members
        /******************************************************************************
        * Function:          RunTest
        ******************************************************************************/
        public override void RunTest()
        {
            foreach (Freezable freezable in freezables)
            {
                // SetGet test
                freezable.SetValue(_booAP, true);
                bool b = (bool)freezable.GetValue(_booAP);

                if (!b)
                {
                    LogFailure("GetValue(booAP) should return true: ", freezable);
                }

                freezable.SetValue(_referenceAP, new object());
                object obj = freezable.GetValue(_referenceAP);

                if (obj.GetType().ToString() != "System.Object")
                {
                    LogFailure(" :GetValue(referenceAP should return System.Object", freezable);
                }

                freezable.SetValue(_freezableAP, new Pen());
                object pen = freezable.GetValue(_freezableAP);
                if (pen.GetType().ToString() != "System.Windows.Media.Pen")
                {
                    LogFailure(": GetValue should return System.Windows.Media.Pen: ", freezable);
                }
                
                // Test event
                _isEventFired = false;
                freezable.Changed += new EventHandler(FreezableEventHandler);
                ((Pen)freezable.GetValue(_freezableAP)).Thickness = 5.0;

                if (!_isEventFired)
                {
                    LogFailure(": Changed event not fired when setting attached freezable DP on a freezable: ", freezable);
                }

                // Copy test
                Freezable freezableCopy = freezable.Clone();
                if (freezableCopy.GetValue(_booAP) != freezable.GetValue(_booAP))
                {
                    LogFailure(": GetValue(booAP) should be equal to the copied object: ", freezable);
                }
                // Reference properties should be the same reference after a copy
                if (!ReferenceEquals(freezableCopy.GetValue(_referenceAP), freezable.GetValue(_referenceAP)))
                {
                    LogFailure(": GetValue(referenceAP) should be equal to the copied object: ", freezable);
                }
         
                // Freezable properties should be different instances after a copy
                if (ReferenceEquals(freezableCopy.GetValue(_freezableAP), freezable.GetValue(_freezableAP)))
                {
                    LogFailure(": GetValue(freezableAP) should NOT be equal to the copied object ", freezable);
                }
                
                // Check a sub property to verify the deep copy
                if (!ReferenceEquals(
                    ((Pen)freezableCopy.GetValue(_freezableAP)).Brush,
                    ((Pen)freezable.GetValue(_freezableAP)).Brush)
                    )
                {
                    LogFailure(": GetValue(freezableAP) on sub property should be equal to the copied object ", freezable);
                }
                // Freeze test
                freezable.Freeze();
                Freezable f = (Freezable)freezable.GetValue(_freezableAP);
                if (!f.IsFrozen)
                {
                    LogFailure(": freezableAP should be frozen after calling Freeze() ", freezable);
                }

                // Test CloneCurrentValue
                Freezable freezableCopyCurrentValue = freezableCopy.CloneCurrentValue();
                // Value properties should be the same value after a copy
                if (freezableCopyCurrentValue.GetValue(_booAP) != freezableCopy.GetValue(_booAP))
                {
                    LogFailure(": freezableCopyCurrentValue.CloneCurrentValue(booAP) should be equal to the original object ", freezable);
                }
                // Reference properties should be the same reference after copy
                if (!ReferenceEquals(freezableCopyCurrentValue.GetValue(_referenceAP), freezableCopy.GetValue(_referenceAP)))
                {
                    LogFailure(": freezableCopyCurrentValue.CloneCurrentValue(ReferencelAP) should be reference equal to the original object ", freezable);
                }
                // Freezable properties should be different instances after a copy
                if (ReferenceEquals(freezableCopyCurrentValue.GetValue(_freezableAP), freezableCopy.GetValue(_freezableAP)))
                {
                    LogFailure(": freezableCopyCurrentValue.CloneCurrentValue(freezableAP) should be reference equal to the original object ", freezable);
                }
            }
        }
 
        /******************************************************************************
        * Function:          FreezableEventHandler
        ******************************************************************************/
        public void FreezableEventHandler(Object sender, EventArgs args)
        {
            _isEventFired = true;
        }
  
        /******************************************************************************
        * Function:          LogFailure
        ******************************************************************************/
        // default value for result.Status == Pass
        private void LogFailure(string message, Freezable freezable)
        {
            GlobalLog.LogEvidence(freezable.GetType().ToString() + message);
            failures.Add(freezable.GetType().ToString() + message);
            passed = false;
        }
        #endregion
    }
}
  


