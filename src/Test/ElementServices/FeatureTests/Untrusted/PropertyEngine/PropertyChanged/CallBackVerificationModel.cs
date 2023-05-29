// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: PropertyChanged Callback verification Test
 * 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Reflection;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Windows.Markup;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Modeling;
using Avalon.Test.CoreUI.Parser;
using Avalon.Test.CoreUI.Trusted.Utilities;
#endregion

namespace Avalon.Test.CoreUI.PropertyEngine.PropertyChanged
{
    /// <summary>
    /// Tests for DependencyProperty value changes.
    /// </summary>
    [TestCaseModel("PropertyChange.xtc", "0", @"PropertyEngine\PropertyChanged", "PropertyChanged Callback Verification.")]
    [Model(@"FeatureTests\ElementServices\PropertyChange.xtc", 0, @"PropertyEngine\PropertyChanged", TestCaseSecurityLevel.FullTrust, "PropertyChanged Callback Verification",
        ExpandModelCases = true)
    ]
    public class PropertyChangeCallBack : CoreModel
    {
        /// <summary>
        /// Creates a PropertyChangeTest Model instance.
        /// </summary>
        public PropertyChangeCallBack()
            : base()
        {
            Name = "PropertyChangeCallBack";
            Description = "Model PropertyChangeTest";
            ModelPath = "MODEL_PATH_TOKEN";

            // Add Action Handlers.
            AddAction("SetUp", new ActionHandler(SetUp));
            AddAction("ClearPropertyValue", new ActionHandler(ClearPropertyValue));
            AddAction("SetPropertyValue", new ActionHandler(SetPropertyValue));
        }
              

        #region Oracle Control Methods
        /// <summary>
        /// Initialize the model.
        /// </summary>
        /// 
        /// <returns> Success status. </returns>
        /// 
        public override bool Initialize()
        {
            CoreLogger.LogStatus("Initialize...");

            return true;
        }

        /// <summary>
        /// Clean up the model.
        /// </summary>
        /// <returns> Success status. </returns>        
        public bool Cleanup()
        {
            CoreLogger.LogStatus("Cleanup");

            return true;
        }

        /// <summary>
        /// This method will be called to set the modeled system to expected state.
        /// </summary>
        /// <param name="expectedState"> Expected state of the system-under-test. </param>
        /// <returns> Succeess status. </returns>
        public bool SetCurrentState(State expectedState)
        {
            CoreLogger.LogStatus("SetCurrentState: " + expectedState);

            return true;
        }

        /// <summary>
        /// This method will be called after every action to retrieve current state of the modeled system.
        /// </summary>
        /// <param name="currentState"> Reference to the state which should be populated. </param>
        /// <returns> Succeess status. </returns>
        public bool GetCurrentState(State currentState)
        {
            CoreLogger.LogStatus("GetCurrentState: " + currentState);

            //
            // The state values set here will be compared to the expected state by the framework.
            //            

            return true;
        }

        /// <summary>
        /// This method will be called before each test case.
        /// </summary>
        /// <param name="beginState"> State before test case is started. </param>
        /// <returns> Succeess status. </returns>
        public override bool BeginCase(State beginState)
        {
            CoreLogger.LogStatus("BeginCase: " + beginState);
            
            return true;
        }

        /// <summary>
        /// This method will be called after each test case.
        /// </summary>
        /// <param name="endState"> State after test case is started. </param>
        /// <returns> Succeess status. </returns>
        public override bool EndCase(State endState)
        {
            CoreLogger.LogStatus("EndCase: " + endState);

            return true;
        }
        #endregion

        #region Model Actions
        /// <summary>
        /// Handler for SetUp. This registers the property based on the parameters from the Model.
        /// </summary>
        /// 
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        private bool SetUp(State endState, State inParameters, State outParameters)
        {
            CoreLogger.LogStatus("Action: Setup");
            CoreLogger.LogStatus(inParameters.ToString());
            
            bool succeed = true;
            string DPName = "";

            _RegistrationType = inParameters["RegistrationType"].ToString();
            _ReadOnly = inParameters["ReadOnly"].ToString();
            _PropertyType = inParameters["PropertyType"].ToString();
            _MetaData = inParameters["MetaData"].ToString();
            _TargetType = inParameters["TargetType"].ToString();            

            // Initialize new CustomDO.
            _cDO = new CustomDO(inParameters);

            DPName = _RegistrationType + _ReadOnly + "Property";
            CoreLogger.LogStatus("DPNAME: " + DPName);

            FieldInfo fi = typeof(CustomDO).GetField(DPName, BindingFlags.Public | BindingFlags.Static);
            _DP_to_use = (DependencyProperty)fi.GetValue(_cDO);

            CoreLogger.LogStatus("Default Value: " + _cDO.First);
            CoreLogger.LogStatus("Default Value (for selected DP): " + _cDO.GetValue(_DP_to_use));            

            return succeed;
        }

        /// <summary>
        /// Handler for SetPropertyValue. Sets a new value for the property.
        /// </summary>
        /// 
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        private bool SetPropertyValue(State endState, State inParameters, State outParameters)
        {
            CoreLogger.LogStatus("Action: SetPropertyValue");
            CoreLogger.LogStatus(inParameters.ToString());            

            bool succeed = true;
            string exMessage = "";

            _NewVal = inParameters["NewVal"].ToString();

            // If it is a read-only property, then verify that it wasnt overridden (check exception).
            if (_ReadOnly == "True")
            {
                try
                {
                    _cDO.SetValue(_DP_to_use, "SampProp");
                }
                catch (InvalidOperationException ex)
                {
                    // Record exception message.
                    exMessage = ex.Message;
                }

                if (exMessage == "")
                {
                    throw new Microsoft.Test.TestValidationException("Attempted to set the value for a and no exception thrown.");
                }
                else
                {
                    CoreLogger.LogStatus("Cannot write to readonly DP exception successfull", ConsoleColor.Green);
                }
            }

            else
            {
                // Set the DP to the custom Value.
                if (_NewVal == "None")
                {
                    _cDO.SetValue(_DP_to_use, null);
                }
                else if (_NewVal == "Default")
                {
                    _cDO.SetValue(_DP_to_use, "SampProp");
                }
                else if (_NewVal == "Derived")
                {
                    _cDO.SetValue(_DP_to_use, "SampProp");
                }
                else if (_NewVal == "Custom")
                {
                    _cDO.SetValue(_DP_to_use, "SampProp");
                }
                else if (_NewVal == "CallsCoerce")
                {
                    _cDO.SetValue(_DP_to_use, "SampProp");
                }
                else if (_NewVal == "Repeat")
                {
                    _cDO.SetValue(_DP_to_use, _cDO.GetValue(_DP_to_use));
                }
            }
                        
            // Verify that the PropertyChangeCallBacks / CoerceValueCallBacks were correct.
            _VerifyCoerceCallBacks("Set", _ReadOnly);
            _VerifyPropertyChangeCallBacks("Set", _ReadOnly);

            return succeed;
        }

        /// <summary>
        /// Handler for ClearPropertyValue. Clear the current value for the property.
        /// </summary>
        /// 
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        private bool ClearPropertyValue(State endState, State inParameters, State outParameters)
        {
            CoreLogger.LogStatus("Action: ClearPropertyValue");
           
            bool succeed = true;
            string exMessage = "";

            // If it is a read-only property, then verify that it wasnt overridden (check exception).
            if (_ReadOnly == "True")
            {
                // Attempt to clear the value of the DP.
                try
                {
                    _cDO.ClearValue(_DP_to_use);
                }
                catch (InvalidOperationException ex)
                {
                    // Record exception message.
                    exMessage = ex.Message;
                }

                if (exMessage == "")
                {
                    throw new Microsoft.Test.TestValidationException("Attempted to clear the value for a readonly DP and no exception thrown.");
                }
                else
                {
                    CoreLogger.LogStatus("Cannot clear readonly DP exception successfull", ConsoleColor.Green);
                }
            }

            else
            {
                _cDO.ClearValue(_DP_to_use);
            }

            // Verify CallBacks (Coerce and PropertyChanged)
            _VerifyCoerceCallBacks("Clear", _ReadOnly);
            _VerifyPropertyChangeCallBacks("Clear", _ReadOnly);
            

            return succeed;
        }
        #endregion

        #region Verifying CallBacks

        // Verify that the correct PropertyChanged CallBacks are being called the correct number of times
        private void _VerifyPropertyChangeCallBacks(string action, string isReadOnly)
        {
            string argListName = "";

            argListName = _RegistrationType + _ReadOnly + "EventArgsList";
            CoreLogger.LogStatus("argListName : " + argListName);

            FieldInfo fi = typeof(CustomDO).GetField(argListName, BindingFlags.Public | BindingFlags.Static);
            _argList_to_use = (List<DependencyPropertyChangedEventArgs>)fi.GetValue(_cDO);

            List<DependencyPropertyChangedEventArgs> argList = _argList_to_use;

            // There should be no PropertyChangeCallBacks if the relevant metadata was not present.
            /*if (_MetaData == "None" || _MetaData == "Coerced")
            {                
                if (argList.Count != 0)
                {                 
                    throw new Microsoft.Test.TestValidationException("Got PropertyChangeCallBack even though NO metadata added.");
                }                
            }*/

            int expectedEvents = 0;

            if (_ReadOnly == "True")
            {
                // The property Should never ve changed if readonly.
                expectedEvents = 0;
            }
            else
            {
                if (_NewVal == "Repeat")
                {
                    // If repeat value propertychangedcallback should not be called
                    expectedEvents = 0;
                }
                else
                {
                    expectedEvents = 2;
                }
            }

            // 1. Verify Count.
            if (argList.Count != expectedEvents)
            {
                throw new Microsoft.Test.TestValidationException("One of the PropertyChanged event callbacks was called an unexpected number of times. Actual: " + argList.Count + ", Expected: " + expectedEvents);
            }

            // 2. Verify Referential Equality.
            if (_ReadOnly != "True" && _NewVal != "Repeat")
            {
                if (argList[0] != argList[1] ||
                    !(argList[0] == argList[1]) ||
                    !argList[0].Equals(argList[1]) ||
                    !argList[0].Equals((object)argList[1]))
                {
                    throw new Microsoft.Test.TestValidationException("The PropertyChanged event args from the virtual and metadata callbacks aren't equal.");
                }

                // 3. Verify Old/New Value
                if (argList[0].OldValue != argList[1].OldValue || argList[0].NewValue != argList[1].NewValue)
                {
                    throw new Microsoft.Test.TestValidationException("Tne PropertyChanged event args from the virtual and metadata callbacks don't have equal OldValue or NewValue.");
                }
            }

            argList.Clear();
            _argList_to_use.Clear();
            
        }

        // Verify that the correct CoerceValue CallBacks are being called the correct number of times
        private void _VerifyCoerceCallBacks(string action, string isReadOnly)
        {
            // Verify that CoerceValueCallBack was triggered.
            string coerceListName = "";

            coerceListName = "Coerce" + _RegistrationType + _ReadOnly + "List";
            CoreLogger.LogStatus("coerceListName : " + coerceListName);

            FieldInfo fi = typeof(CustomDO).GetField(coerceListName, BindingFlags.Public | BindingFlags.Static);
            _coerceList_to_use = (List<string>)fi.GetValue(_cDO);

            // New Behavior: Clear does not call coercevalue callback
            if (action == "Clear")
            {
                if (_coerceList_to_use.Count != _coerceCount)
                {
                    CoreLogger.LogStatus("Actual: " + _coerceList_to_use.Count);
                    CoreLogger.LogStatus("Expected: " + _coerceCount);
                    throw new Microsoft.Test.TestValidationException("Coercevalue Callback was not called for event: " + action);
                }
            }

            else
            {
                if (isReadOnly == "False")
                {
                    // Increase the expected count.
                    _coerceCount++;

                    if (_coerceList_to_use.Count != _coerceCount)
                    {
                        CoreLogger.LogStatus("Actual: " + _coerceList_to_use.Count);
                        CoreLogger.LogStatus("Expected: " + _coerceCount);
                        throw new Microsoft.Test.TestValidationException("Coercevalue Callback was not called for event: " + action);
                    }
                }
                else
                {
                    if (_coerceList_to_use.Count != _coerceCount)
                    {
                        CoreLogger.LogStatus("Actual: " + _coerceList_to_use.Count);
                        CoreLogger.LogStatus("Expected: " + _coerceCount);
                        throw new Microsoft.Test.TestValidationException("Coercevalue Callback was not called for event: " + action);
                    }
                }
            }
        }

        #endregion

        #region Helper Methods / Fields
        private CustomDO _cDO = null;
        private DependencyProperty _DP_to_use = null;
        private List<DependencyPropertyChangedEventArgs> _argList_to_use = null;
        private List<string> _coerceList_to_use = null;
        private int _coerceCount = 0;
        
        // State Defining Variables.
        private string _RegistrationType = null;
        private string _ReadOnly = null;
        private string _PropertyType = null;
        private string _MetaData = null;
        private string _TargetType = null;
        private string _NewVal = null;

        #endregion
        
    }   
}
