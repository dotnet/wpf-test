// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: IDictionary Interface Test
 * 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Modeling;

using Microsoft.Test.Serialization;
using Microsoft.Test.Threading;
using System.Windows.Threading;
using System.Windows;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Common;

namespace Avalon.Test.CoreUI.Resources
{
    /// <summary>
    /// IDictionaryTest Model class
    /// </summary>
    ///     
    [Model(@"FeatureTests\ElementServices\IDictionary_pairwise.xtc", 0, @"Resources\IDictionary", TestCaseSecurityLevel.FullTrust, "IDictionary Interface Test.",
    ExpandModelCases = true)]
    public class IDictionaryTest : CoreModel
    {
        /// <summary>
        /// Creates a IDictionaryTest Model instance.
        /// </summary>
        public IDictionaryTest()
            : base()
        {
            Name = "IDictionaryTest";
            Description = "Model IDictionaryTest";
            ModelPath = "MODEL_PATH_TOKEN";

            // Add Action Handlers.
            AddAction("Add", new ActionHandler(Add));
            AddAction("Contains", new ActionHandler(Contains));
            AddAction("Clear", new ActionHandler(Clear));
            AddAction("Remove", new ActionHandler(Remove));
        }

        // Create IDictionary object to be used for the test.
        IDictionary _testDict = new ResourceDictionary();

        // Create Array to keep track of add/removes and other ops (for Validation purposes).
        ArrayList _validationArray = new ArrayList();

        // This array contains a set of keys to be used for entering into the testDict.
        Hashtable _keys = new Hashtable();
        Hashtable _vals = new Hashtable();
    
        /// <summary>
        /// Initialize the model.
        /// </summary>
        /// 
        /// <returns> Success status. </returns>
        /// 
        public override bool Initialize()
        {
            CoreLogger.LogStatus("Initialize...");

            // Define possible set of Values.
            FrameworkElement buttonKey = new FrameworkElement();
            DependencyObject depObjKey = new DependencyObject();            
            object genObjKey = new Object();
            string testStrKey = "stringger";
            string testStr2Key = "stringger2";
            
            // Define possible set of Values.
            FrameworkElement button = new FrameworkElement();
            DependencyObject depObj = new DependencyObject();            
            object genObj = new Object();
            string testStr = "stringger";
            string testStr2 = "stringger2";

            // Insert Keys.
            _keys[1] = buttonKey;
            _keys[2] = depObjKey;
            _keys[3] = genObjKey;
            _keys[4] = testStrKey;
            _keys[5] = testStr2Key;

            // Insert Values.
            _vals[1] = button;
            _vals[2] = depObj;
            _vals[3] = genObj;
            _vals[4] = testStr;
            _vals[5] = testStr2;

            return true;
        }

        /// <summary>
        /// Clean up the model.
        /// </summary>
        /// 
        /// <returns> Success status. </returns>
        /// 
        public bool Cleanup()
        {
            CoreLogger.LogStatus("Cleanup");

            return true;
        }

        /// <summary>
        /// This method will be called to set the modeled system to expected state.
        /// </summary>
        /// 
        /// <param name="expectedState"> Expected state of the system-under-test. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        public bool SetCurrentState(State expectedState)
        {
            CoreLogger.LogStatus("SetCurrentState: " + expectedState);

            return true;
        }

        /// <summary>
        /// This method will be called after every action to retrieve current state of the modeled system.
        /// </summary>
        /// 
        /// <param name="currentState"> Reference to the state which should be populated. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        public bool GetCurrentState(State currentState)
        {
            CoreLogger.LogStatus("GetCurrentState: " + currentState);

            //
            // The state values set here will be compared to the expected state by the framework.
            //
            currentState["ReadOnly"] = _testDict.IsReadOnly.ToString();
            currentState["Count"] = getCount().ToString();

            return true;
        }

        /// <summary>
        /// This method will be called before each test case.
        /// </summary>
        /// 
        /// <param name="beginState"> State before test case is started. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        public override bool BeginCase(State beginState)
        {
            CoreLogger.LogStatus("BeginCase: " + beginState);

            return true;
        }

        /// <summary>
        /// This method will be called after each test case.
        /// </summary>
        /// 
        /// <param name="endState"> State after test case is started. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        public override bool EndCase(State endState)
        {
            CoreLogger.LogStatus("EndCase: " + endState);

            return true;
        }

        /// <summary>
        /// Manually determine the number of objects in the testDictionary. 
        /// </summary>
        /// <returns> Current Count. </returns>
        public int getCount()
        {
            int counter = 0;
            IEnumerator myEnumerator = _testDict.GetEnumerator();

            while (myEnumerator.MoveNext())
            {
                if (myEnumerator.Current != null)
                {
                    counter++;
                }
            }
            return counter;
        }

        /// <summary>
        /// Handler for Add.
        /// </summary>
        /// 
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        private bool Add(State endState, State inParameters, State outParameters)
        {
            CoreLogger.LogStatus("Action Add");

            CoreLogger.LogStatus(inParameters.ToString());
            CoreLogger.LogStatus(endState.ToString());
            
            bool succeed = true;
            bool validAdd = true;

            string paramKey = inParameters["Key"].ToString();
            string paramVal = inParameters["Value"].ToString();
            
            string exMessage = "";
            object myKey = "";
            object myVal = null;
            

            // Determine what key to use.
            if (paramKey == "Valid")
            {
                myKey = _keys[_testDict.Count+1];                
            }
            else if (paramKey == "Null")
            {
                validAdd = false;
                myKey = null;                
            }
            else if (paramKey == "Repeat")
            {
                validAdd = false;

                // Can only repeat if something is present.
                if (_testDict.Count > 0)
                {
                    myKey = _keys[_testDict.Count];                    
                }
                else
                {
                    // Skip repeat action if dictionary is empty.
                    return true;
                }
            }

            // Determine what value to use.
            if (paramVal == "Valid")
            {
                myVal = _vals[_testDict.Count];
            }
            else if (paramVal == "Null")
            {
                myVal = null;
            }

            //
            // Attempt to add the key/value pair.
            //
            try
            {
                _testDict.Add(myKey, myVal);
            }
            catch (Exception ex)
            {
                // Record exception message.
                exMessage = ex.Message;
            }

            //
            // Verify Exceptions.
            //

            // Check if exception was fired on null key val.
            if (myKey == null)
            {
                if (exMessage.CompareTo("Key cannot be null.\r\nParameter name: key") != 0)
                {
                    CoreLogger.LogStatus("Null key was added and no exception thrown!");
                    return false;
                }
            }

            // Check if exception was fired on Repeat Key.
            if (_validationArray.Contains(myKey))
            {
                if (exMessage.CompareTo("An item with the same key has already been added.") != 0)
                {
                    CoreLogger.LogStatus("Repeat key was added and no exception thrown!");
                    return false;
                }
            }

            // If operation was successfull update the validation array.
            if (validAdd == true)
            {
                _validationArray.Add(myKey);
                succeed = verifyEnumerator();
            }

            return succeed;
        }

        
        /// <summary>
        /// Verifies that enumerator is counting correctly.
        /// </summary>        
        /// 
        /// <returns> Succeess status. </returns>
        private bool verifyEnumerator()
        {
            if (_validationArray.Count == getCount())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Handler for Clear. Clears TestDictionary Items.
        /// </summary>
        /// 
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        private bool Clear(State endState, State inParameters, State outParameters)
        {

            CoreLogger.LogStatus("Action Clear");
            CoreLogger.LogStatus(endState.ToString());            
            
            try
            {
                _testDict.Clear();
            }
            catch (Exception ex)
            {
                CoreLogger.LogStatus("There shouldn't be an exception here!" + ex.ToString());
                return false;
            }

            _validationArray.Clear();

            return true;
        }

        /// <summary>
        /// Handler for Contains. Verifies Last key inserted is in dictionary.
        /// </summary>
        /// 
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        private bool Contains(State endState, State inParameters, State outParameters)
        {
            CoreLogger.LogStatus("Action Contains");

            CoreLogger.LogStatus(inParameters.ToString());
            CoreLogger.LogStatus(endState.ToString());

            string paramKey = inParameters["Key"].ToString();
            
            object searchKey = null;
            bool isContains = false;
            string exMessage = "";

            if (paramKey == "Valid")
            {
                // Grab key from TestDictionary.
                searchKey = _keys[_testDict.Count];
            }
            else if (paramKey == "Null")
            {
                searchKey = null;
            }
            else if (paramKey == "Invalid")
            {
                searchKey = "Non-existant key";
            }

            try
            {
                isContains = _testDict.Contains(searchKey);
                CoreLogger.LogStatus("isContains: " + isContains);
            }
            catch (Exception ex)
            {
                // Record exception.
                exMessage = ex.Message;
            }


            //
            // Check Results.
            //
            if ((paramKey == "Null") && (isContains == true))
            {   
                CoreLogger.LogStatus("Contains return true for null key");
                return false;
            }
            if ((paramKey == "Invalid") && (isContains == true))
            {
                CoreLogger.LogStatus("Contains return true for Invalid key");
                return false;
            }

            // If valid key and still not found then return false.
            if ((paramKey == "Valid") && (isContains == false))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Handler for Remove.
        /// </summary>
        /// 
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        private bool Remove(State endState, State inParameters, State outParameters)
        {
            CoreLogger.LogStatus("Action Remove");

            CoreLogger.LogStatus(inParameters.ToString());
            CoreLogger.LogStatus(endState.ToString());

            string paramKey = inParameters["Key"].ToString();

            object keyToRemove = null;
            Type t = null;

            if (paramKey == "Valid")
            {
                // If its a valid key select one from the validation array.
                keyToRemove = _keys[_testDict.Count+1];
            }
            else if (paramKey == "Null")
            {
                keyToRemove = null;
            }
            if (paramKey == "Invalid")
            {
                // If its a valid key select one from the validation array.
                keyToRemove = "RandomString";
            }

            //
            // Attempt to remove key.
            //
            try
            {
                _testDict.Remove(keyToRemove);
            }
            catch (Exception ex)
            {
                CoreLogger.LogStatus("Remove Exception: " + ex.ToString());
                
                t = ex.GetType();
            }

            //
            // Check if exceptions were thrown.
            //

            // 1. Key not present.  No Exception should be thrown.
            if (paramKey == "Invalid")
            {
                if (t != null)
                {
                    CoreLogger.LogStatus("Attempted to remove non-existent key: an unexpected Exception was thrown.");
                    return false;
                }
            }

            // 2. Null Key.
            if (paramKey == "Null")
            {
                if (t == null)
                {
                    CoreLogger.LogStatus("Attempted to remove null key: no Exception was thrown.");
                    return false;
                }
                else
                {
                    if (t != typeof(System.ArgumentNullException))
                    {
                        CoreLogger.LogStatus("Attempted to remove null key: the Exception thrown was not an ArgumentNullException.");
                        return false;
                    }
                    else
                    {
                        CoreLogger.LogStatus("Attempted to remove null key: the expected ArgumentNullException Exception was thrown.");
                    }
                }
            }

            // 3. Regular Key.
            if (paramKey == "Valid")
            {
                if (t != null)
                {
                    CoreLogger.LogStatus("Could not remove key properly: an unexpected Exception was thrown.");
                    return false;
                }
            }

            // If success then delete from validation array.
            _validationArray.Remove(keyToRemove);
            
            return true;
        }
    }
}
