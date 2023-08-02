// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2004
 *
 *   Program:   Test freezable Event
 
 *
 ************************************************************/

using System;
using System.Reflection;
using System.Windows;
using System.Threading;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.ElementServices.Freezables.Utils;


namespace Microsoft.Test.ElementServices.Freezables
{

    /**********************************************************************************
    * CLASS:          EventPatternsTest
    **********************************************************************************/
    internal class EventPatternsTest : FreezablesPatternsBase 
    {

        #region Constructor
        /******************************************************************************
        * Function:          Constructor
        ******************************************************************************/
        internal EventPatternsTest(Microsoft.Test.ElementServices.Freezables.Utils.Result result)
            : base(result)
        {
            testName = "EventPatternsTest";
        }
        #endregion


        #region Internal Members
        /******************************************************************************
        * Function:          Perform
        ******************************************************************************/
        /// <summary>
        /// Invoke the specific test case requested.
        /// </summary>
        /// <param name="t">The Type of the Freezable object to be tested.</param>
        /// <returns></returns>
        internal override void Perform(Type t)
        {
            if (t.IsAbstract || IsKnownIssue(t) || t.IsNotPublic)
            {
                return;
            }
            if (t.ToString() == "System.Windows.Media.Imaging.BitmapMetadata")
            {
                return;
            }

            //
            if (t.ToString() == "System.Windows.Media.MediaPlayer")
            {
                return;
            }

            GlobalLog.LogStatus("ClassName: " + t.ToString());

            // Add event handler to "Changed",
            EventPatternsTestFiring(t, "TestSingleEventFiring");

            // Call Freeze, Add event handler
            // Expected Exception raised
            TestAddOrRemoveEvent(t, "TestAddEvent");
      
            // Call Freeze, then remove event from "Changed"
            // Expected Exception raised
            TestAddOrRemoveEvent(t, "TestRemoveEvent");
            
            // Test event on Complex freezable
            // with SONU=ChangeableReference
            FreezablesPatternsBaseReferenceForECT(t);

            // Test event on Complex freezable
            // with SONU=ChangeableCopy
            FreezablesPatternsBaseCopyForECT(t);

            // Add/Remove event handler many time to Changed property
            EventPatternsTestFiring(t, "TestMultiEventFiring");
        }

        /******************************************************************************
        * Function:          EventPatternsTestFiring
        ******************************************************************************/
        /// <summary>
        /// Create a Freezable of Type t and register it.
        /// </summary>
        /// <param name="t">The Type of the Freezable object to be tested.</param>
        /// <param name="name">Test name.</param>
        /// <returns></returns>
        private void EventPatternsTestFiring(Type t, string name)
        {
            // create an instance of this type
            Freezable obj = null;

            if (FreezablesPatternsHelper.IsSpecialFreezable(t))
            {
                obj = FreezablesPatternsHelper.CreateSpecialFreezable(t);
            }
            else
            {
                obj = FreezablesPatternsHelper.CreateNewChangeable(t);
            }
            if (obj != null)
            {
                TestObj testobj = new TestObj(obj, result);
                testobj.RegisterEvent(obj, name);
            }
        }

    
        /******************************************************************************
        * Function:          TestAddOrRemoveEvent
        ******************************************************************************/
        /// <summary>
        /// Adding/Remove event on an Unchangeable object should throw.
        /// </summary>
        /// <param name="t">The Type of the Freezable object to be tested.</param>
        /// <param name="name">Test name.</param>
        /// <returns></returns>
        private void TestAddOrRemoveEvent(Type t, string testName)
        {
            Freezable obj = FreezablesPatternsHelper.CreateNewChangeable(t);

            if (FreezablesPatternsHelper.IsSpecialCase(t, obj))
            {
                return;
            }

            if (obj != null && obj.CanFreeze)
            {
                TestObj testObj = new TestObj(obj, result);
                try
                {
                    obj.Freeze();
                    // testName could be AddEvent or RemoveEvent
                    testObj.RegisterEvent(obj, testName);

                    // if no exception thrown, fail the test now
                    GlobalLog.LogEvidence("An InvalidOperationException was expected, but was not thrown");
                }
                catch (System.InvalidOperationException e)
                {
                    // Expected InvalidOperationExpection, since we call
                    // Freeze before adding/removing event
                    //if (!e.ToString().Contains("must have IsFrozen"))
                    //{
                    //    result.passed &= false;
                    //    result.failures.Add(t.ToString() + ": " + testName + " - Unexpected Exceptiion : " + e.ToString());
                    //    GlobalLog.LogEvidence("{0} - Unexpected Exception : {1}", testName, e.ToString());
                    //}
                    string s = e.ToString();
                }
            }
        }

        /******************************************************************************
        * Function:          FreezablesPatternsBaseReferenceForECT
        ******************************************************************************/
        /// <summary>
        /// ChangeableReference for Embedded Freezable Type (complex freezable).
        /// </summary>
        /// <param name="t">The Type of the Freezable object to be tested.</param>
        /// <returns></returns>
        private void FreezablesPatternsBaseReferenceForECT(Type t)
        {
            // create object2 and invoke the get properties on it
            Freezable obj1 = FreezablesPatternsHelper.CreateNewChangeable(t);
            Freezable obj2 = FreezablesPatternsHelper.CreateNewChangeable(t);

            if (FreezablesPatternsHelper.IsSpecialCase(t, obj1) || obj1 == null)
            {
                return;
            }

            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            for (int i = 0; i < pi.Length; i++)
            {
                Freezable complexChangeable = FreezablesPatternsHelper.GetComplexChangeable(pi[i], obj1);
            
                if (complexChangeable == null)
                {
                    continue;
                }
          
                if (!complexChangeable.IsFrozen)
                {
                    if (!pi[i].CanWrite)
                    {
                        return;
                    }
                    // need to attacht to obj2
                    pi[i].SetValue(obj2, complexChangeable, null);

                    TestObj testObj1 = new TestObj(obj1, result);
                    testObj1.RegisterEvent(obj1, "TestAddEventOnComplexChangeable1");

                    TestObj testObj2 = new TestObj(obj2, result);
                    testObj2.RegisterEvent(obj2, "TestAddEventOnComplexChangeable2");
                    
                    TestObj testObj3 = new TestObj(complexChangeable, result);
                    testObj3.RegisterEvent(complexChangeable, "TestAddEventOnComplexChangeable3");
                    
                    // This modification should trigger the events for all objects (obj1, complexChangeable)
                    ((Freezable)complexChangeable).Freeze();
                    if (!testObj1.IsComplexTypeEvent1Fired || !testObj2.IsComplexTypeEvent2Fired || !testObj3.IsComplexTypeEvent3Fired)
                    {
                        result.passed &= false;
                        if (!testObj1.IsComplexTypeEvent1Fired)
                        {
                            result.failures.Add(obj1.GetType().ToString() + " : Event not fired on Complex Freezable");
                            GlobalLog.LogEvidence(" {0} : Event did not fire on Complex Freezable", obj1.GetType().ToString());
                        }

                        if (!testObj2.IsComplexTypeEvent2Fired)
                        {
                            result.failures.Add(obj2.GetType().ToString() + " : Event not fired on Complex Freezable");
                            GlobalLog.LogEvidence(" {0} : Event did not fire Complex Freezable", obj2.GetType().ToString());
                        }

                        if (!testObj3.IsComplexTypeEvent3Fired)
                        {
                            result.failures.Add(complexChangeable.GetType().ToString() + " : Event not fired on Complex Freezable");
                            GlobalLog.LogEvidence(" {0} : Event did not fire Complex Freezable", complexChangeable.GetType().ToString());
                        }
                    }
                }
            }
        }

        /******************************************************************************
        * Function:          FreezablesPatternsBaseCopyForECT
        ******************************************************************************/
        /// <summary>
        /// ChangeableCopy for Embedded Freezable Type (complex freezable).
        /// </summary>
        /// <param name="t">The Type of the Freezable object to be tested.</param>
        /// <returns></returns>
        private void FreezablesPatternsBaseCopyForECT(Type t)
        {
            Freezable obj1 = FreezablesPatternsHelper.CreateNewChangeable(t);

            if (FreezablesPatternsHelper.IsSpecialCase(t, obj1) || obj1 == null)
            {
                return;
            }

            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

            for (int i = 0; i < pi.Length; i++)
            {
                Freezable complexChangeable = FreezablesPatternsHelper.GetComplexChangeable(pi[i], obj1);
                if (complexChangeable == null)
                {
                    continue;
                }
                if (!complexChangeable.IsFrozen)
                {
                    if (!pi[i].CanWrite)
                    {
                        return;
                    }

                    // need to attacht to obj1
                    pi[i].SetValue(obj1, complexChangeable, null);

                    // GetValue here
                    Freezable complexValue = (Freezable)pi[i].GetValue(obj1, null);
                    TestObj testObj1 = new TestObj(obj1, result);

                    testObj1.RegisterEvent(obj1, "TestAddEventOnComplexChangeable1");

                    TestObj testObj2 = new TestObj(complexChangeable, result);

                    testObj2.RegisterEvent(complexValue, "TestAddEventOnComplexChangeable3");

                    // This modification should trigger the events for all objects (obj1, complexChangeable)
                    ((Freezable)complexValue).Freeze();
                    if (!testObj1.IsComplexTypeEvent1Fired || !testObj2.IsComplexTypeEvent3Fired)
                    {
                        result.passed &= false;
                        if (!testObj1.IsComplexTypeEvent1Fired)
                        {
                            result.failures.Add(obj1.GetType().ToString() + " : Event not fired on Complex Freezable");
                            GlobalLog.LogEvidence(" {0} : Event did not fire on Complex Freezable", obj1.GetType().ToString());
                        }

                        if (!testObj2.IsComplexTypeEvent3Fired)
                        {
                            result.failures.Add(complexChangeable.GetType().ToString() + " : Event not fired on Complex Freezable");
                            GlobalLog.LogEvidence(" {0} : Event did not fire Complex Freezable", complexChangeable.GetType().ToString());
                        }
                    }
                }
            }
        }
        #endregion
    }


    /**********************************************************************************
    * CLASS:          EventPatternsTest
    **********************************************************************************/
    internal class TestObj
    {
        #region Private Data
        private string _className;
        private Microsoft.Test.ElementServices.Freezables.Utils.Result _result;
        internal bool IsEventFired;
        internal bool IsComplexTypeEvent1Fired;
        internal bool IsComplexTypeEvent2Fired;
        internal bool IsComplexTypeEvent3Fired;
        internal int  numEventFired;
        #endregion


        #region Constructor
        /******************************************************************************
        * Function:          Constructor
        ******************************************************************************/
        internal TestObj(Freezable obj, Microsoft.Test.ElementServices.Freezables.Utils.Result af)
        {
            _result = af;
            _className = obj.GetType().ToString();
            IsEventFired = false;
            IsComplexTypeEvent1Fired = false;
            IsComplexTypeEvent2Fired = false;
            IsComplexTypeEvent3Fired = false;
        }
        #endregion


        #region Internal Members
        /******************************************************************************
        * Function:          RegisterEvent
        ******************************************************************************/
        /// <summary>
        /// Attach an event handler for the Changed event.  In some cases, test event
        /// firing when calling Freeze().
        /// </summary>
        /// <param name="obj">The Freezable object to be tested.</param>
        /// <param name="name">Test name.</param>
        /// <returns></returns>
        internal void RegisterEvent(Freezable obj, string testName)
        {
            switch (testName)
            {
                case "TestSingleEventFiring":
                    obj.Changed += new EventHandler(EventFiringHandler);
                    // Freeze to make the even fire
                    obj.Freeze();
                    if (!IsEventFired)
                    {
                        _result.passed &= false;
                        _result.failures.Add(obj.GetType().ToString() + " : Event not fired");
                        GlobalLog.LogEvidence(" {0} : Event did not fire", obj.GetType().ToString());
                    }

                    // reset
                    IsEventFired = false;
                    break;
                case "TestMultiEventFiring":
                   //  Add change handler 10000 time
                   //  make sure it fired >= 10000 times
                    // This because some freezable may be nested
                    // and the nested freezable (children) may or may not 
                    // fired the event to its parent.
                    AddEventFiringHandler(obj, 10000);
                    numEventFired = 0;
                    obj.Freeze();

                    if (numEventFired < 10000)
                    {
                        _result.passed &= false;
                        _result.failures.Add(obj.GetType().ToString() + " : Number of event fired is : " + numEventFired + " Expected: >= " + 10000);
                        GlobalLog.LogEvidence(" {0} : Number of event fired is {1} Expected >= " + 10000, obj.GetType().ToString(), numEventFired);
                    }
                    //  Add change handler 10000 time
                    //  Remove change hander 1 time
                    //  make sure it fired >= 9999 times
                    obj = obj.Clone();
                    AddEventFiringHandler(obj, 10000);
                    AddEventFiringHandler(obj, 1);
                    numEventFired = 0;
                    obj.Freeze();

                    if (numEventFired < 9999)
                    {
                        _result.passed &= false;
                        _result.failures.Add(obj.GetType().ToString() + " : Number of event fired is : " + numEventFired + " Expected: >= " + 9999);
                        GlobalLog.LogEvidence(" {0} : Number of event fired is {1} Expected >= " + 9999, obj.GetType().ToString(), numEventFired);
                    }
                    // Add change handler 100
                    // Then remove 100 time
                    // Make sure event not fired
                    numEventFired = 0;
                    // Make unfrozen freezable
                    obj = obj.Clone(); 
                    AddEventFiringHandler(obj, 100);
                    RemoveEventFiringHandler(obj, 100);
                    obj.Freeze();
                    if (numEventFired != 0)
                    {
                        _result.passed &= false;
                        _result.failures.Add(obj.GetType().ToString() + " : Number of event fired is : " + numEventFired + " Expected: 0");
                        GlobalLog.LogEvidence(" {0} : Number of event fired is {1} Expected 0", obj.GetType().ToString(), numEventFired);
                    }
                    // Add Event 100 time,
                    // Remove 101 time
                    // make sure it throw
                    
                    obj = obj.Clone();
                    AddEventFiringHandler(obj, 100);
                    try
                    {
                        RemoveEventFiringHandler(obj, 101);
                        _result.passed &= false;
                        _result.failures.Add(obj.GetType().ToString() + " : Number of event fired is : " + numEventFired + " Expected: System.ArgumentException");
                        GlobalLog.LogEvidence(" {0} : Number of event fired is {1} Expected an Exception", obj.GetType().ToString(), numEventFired);
                    }
                    catch (ArgumentException e)
                    {
                        Type expectedErrorType = typeof(System.ArgumentException);

                        if (expectedErrorType != e.GetType() && !e.GetType().IsSubclassOf(expectedErrorType))
                        {
                            _result.passed &= false;
                            _result.failures.Add(obj.GetType().ToString() + " : Number of event fired is : " + numEventFired + "RegisterEvent - Expected System.ArgumentException");
                        }
                    }
                    break;
                case "TestAddEvent":
                    obj.Changed += new EventHandler(AddEventHandler);
                    break;

                case "TestAddEventOnComplexChangeable1":
                    obj.Changed += new EventHandler(AddComplexTypeEventHandler1);
                    break;

                case "TestAddEventOnComplexChangeable2":
                    obj.Changed += new EventHandler(AddComplexTypeEventHandler2);
                    break;

                case "TestAddEventOnComplexChangeable3":
                    obj.Changed += new EventHandler(AddComplexTypeEventHandler3);
                    break;

                case "TestRemoveEvent":
                    obj.Changed -= new EventHandler(RemoveEventHandler);
                    break;

                default:
                    throw new ApplicationException("Unknown Test Name: " + testName);
            }
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          DoNothingEventHandler
        ******************************************************************************/
        private void DoNothingEventHandler(Object sender, EventArgs args)
        {
        }

        /******************************************************************************
        * Function:          EventFiringHandler
        ******************************************************************************/
        private void EventFiringHandler(Object sender, EventArgs args)
        {
            IsEventFired = true;
            numEventFired++;
        }

        /******************************************************************************
        * Function:          AddEventHandler
        ******************************************************************************/
        private void AddEventHandler(Object sender, EventArgs args)
        {
            GlobalLog.LogEvidence("This event should not fire");
        }

        /******************************************************************************
        * Function:          AddComplexTypeEventHandler1
        ******************************************************************************/
        private void AddComplexTypeEventHandler1(Object sender, EventArgs args)
        {
            IsComplexTypeEvent1Fired = true;
        }

        /******************************************************************************
        * Function:          AddComplexTypeEventHandler2
        ******************************************************************************/
        private void AddComplexTypeEventHandler2(Object sender, EventArgs args)
        {
            IsComplexTypeEvent2Fired = true;
        }

        /******************************************************************************
        * Function:          AddComplexTypeEventHandler3
        ******************************************************************************/
        private void AddComplexTypeEventHandler3(Object sender, EventArgs args)
        {
            IsComplexTypeEvent3Fired = true;
        }

        /******************************************************************************
        * Function:          RemoveEventHandler
        ******************************************************************************/
        private void RemoveEventHandler(Object sender, EventArgs args)
        {
            ((Freezable)sender).Freeze();
            ((Freezable)sender).Changed -= new EventHandler(DoNothingEventHandler);
        }

        /******************************************************************************
        * Function:          AddEventFiringHandler
        ******************************************************************************/
        private void AddEventFiringHandler(Freezable freezable, int count)
        {
            for (int i = 0; i < count; i++)
            {
                freezable.Changed += new EventHandler(EventFiringHandler);
        
            }
        }
        /******************************************************************************
        * Function:          RemoveEventFiringHandler
        ******************************************************************************/
        private void RemoveEventFiringHandler(Freezable freezable, int count)
        {
            for (int i = 0; i < count; i++)
            {
                freezable.Changed -= new EventHandler(EventFiringHandler);
        
            }
        }
        #endregion
    }
}
