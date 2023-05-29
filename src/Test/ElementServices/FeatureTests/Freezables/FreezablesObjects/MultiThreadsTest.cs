// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using System.Security.Policy;
using System.Windows;
using System.Windows.Threading;

using Microsoft.Test.ElementServices.Freezables.Objects;
using Microsoft.Test.Logging;


namespace Microsoft.Test.ElementServices.Freezables
{
    /**********************************************************************************
    * CLASS:          CopyTest
    **********************************************************************************/
    public class MultiThreadsTest : FreezablesObjectsBase
    {
        #region Private Data
        private Type _freezablePropertyType;
        private Freezable _freezable;
        private DispatcherFrame _dispatcherFrameInRunTest;
        private DispatcherFrame _dispatcherFrameInCreateNewThread;
        #endregion


        #region Constructor
        /******************************************************************************
        * Function:          Constructor
        ******************************************************************************/
        public MultiThreadsTest(string testName, string objName) : base(testName, objName)
        {
        }
        #endregion


        #region Public and Protected Members
        /******************************************************************************
        * Function:          RunTest
        ******************************************************************************/
        // This test does the following:
        // For each freezable that we get from the base class do the followng:
        // -Enumerate property of a feezable. For example Pen has Brush property
        // -Create a freezable  of that type. In this case Brush2 from another thread
        // -Then try to modify or read the value and catch the exeception.
        // -If the specific exception is caught with the correct message we pass, else we fail
        // -If no exception raise, we fail.
        public override void RunTest()
        {
            object result = null;
            
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new DispatcherOperationCallback(DoSetup),
                (object)result
            );
            _dispatcherFrameInRunTest = new DispatcherFrame();
            Dispatcher.PushFrame(_dispatcherFrameInRunTest);
 
        }
 
        /******************************************************************************
        * Function:          DoSetup
        ******************************************************************************/
        private object DoSetup(object result)
        {
            // We get freezables from the base class
            foreach (Freezable f in freezables)
            {
                TestMultiThread(f);
            }
            return result;
        }

        /******************************************************************************
        * Function:          TestMultiThread
        ******************************************************************************/
        private void TestMultiThread(Freezable f)
        {
            Type t = f.GetType();
            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in pi)
            {
                if (!TypeHelper.IsFreezable(property.PropertyType))
                {
                    continue;
                }
                if (DoesCauseStackOverflow(f, property))
                {
                    // When GetValue from some object (ex: MatrixTransform)
                    // The property value that I get is the same object as it class
                    // This will cause stack overflow, as this process is going thru
                    // Infinite loop. So I have to do this to prevent stack overflow from 
                    // happening.
                    continue;
                }
                if (f is IEnumerable)
                {
                    ModifyAndVerify(f, property);
                    // Test freeze, setvalue for each item in freezable collection
                    foreach (Freezable freezableItem in (IEnumerable)f)
                    {
                        TestMultiThread(freezableItem);
                    }

                }
                else
                {
                    Freezable subFreezable = (Freezable)property.GetValue(f, null);
                    if (subFreezable != null)
                    {
                        TestMultiThread(subFreezable);
                    }
                    ModifyAndVerify(f, property);
                }
            }
            _dispatcherFrameInRunTest.Continue = false;

        }

        /******************************************************************************
        * Function:          ModifyAndVerify
        ******************************************************************************/
        private void ModifyAndVerify(Freezable f, PropertyInfo property)
        {
      
            if (!TypeHelper.IsFreezable(property.PropertyType))
            {
               goto Exit;
            }
            if (f is IEnumerable)
            {
                _freezablePropertyType = f.GetType();
            }
            else
            {
                _freezablePropertyType = property.PropertyType;
            }
            // Make new thread to create freezable object of that property type then modify
            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(CreateNewThread));
            thread.Start();
            thread.Join();
            // This freezable is created from another thread - CreateNewThread()
            if (_freezable == null)
            {
                goto Exit;
            }
 
            try
            {
                if (!_freezable.IsFrozen)
                {
                    // if no exception, fail the test
                    GlobalLog.LogEvidence("Reading freezable property " + _freezable.GetType().ToString() + " from another thread does not cause InvalidOperationException, expecting one");
                    failures.Add("Reading freezable property " + _freezable.GetType().ToString() + " from another thread into does not cause InvalidOperationException, expecting one");
                    passed &= false;
                }
            }
            catch (System.InvalidOperationException e)
            {
                //GlobalLog.LogEvidence("Catch InvalidOperationException when Freeze()");
                LogTest(e, _freezable, "Freezing a freezable from anoter thread"); 

            }
        Exit:
            _dispatcherFrameInRunTest.Continue = false;
        }

        /******************************************************************************
        * Function:          TestFreezableCollection
        ******************************************************************************/
        private void TestFreezableCollection(Freezable sourceFreezable)
        {
            try
            {
                // Reading value from another tread should raise the exception.
                foreach (Freezable f in (IEnumerable)sourceFreezable)
                {
                    GlobalLog.LogEvidence("Reading freezable " + sourceFreezable.GetType().ToString() + " from another thread does not cause InvalidOperationException, expecting one");
                    failures.Add("Reading freezable " + sourceFreezable.GetType().ToString() + " from another thread does not cause InvalidOperationException, expecting one");
                    passed &= false;
                }
            }
            catch (InvalidOperationException e)
            {
               // GlobalLog.LogEvidence("Catch InvalidOperationException when reading value");
                LogTest(e, sourceFreezable, "Reading value");
            }
        }

        /******************************************************************************
        * Function:          CreateNewThread
        ******************************************************************************/
        private void CreateNewThread()
        {
          Dispatcher.CurrentDispatcher.BeginInvoke(
            DispatcherPriority.ApplicationIdle,
            new DispatcherOperationCallback(MakeFreezable),
            null);

          _dispatcherFrameInCreateNewThread = new DispatcherFrame();
          Dispatcher.PushFrame(_dispatcherFrameInCreateNewThread);

        }

        /******************************************************************************
        * Function:          MakeFreezable
        ******************************************************************************/
        private object MakeFreezable(object arg)
        {
            if (_freezablePropertyType.ToString().EndsWith("Collection"))
            {
                _freezable = (Freezable)PredefinedObjects.MakeCollection(_freezablePropertyType.ToString());
            }
            else
            {
                _freezable = (Freezable)PredefinedObjects.MakeValue(_freezablePropertyType);
            }
            _dispatcherFrameInCreateNewThread.Continue = false;
            return arg;
        }

        /******************************************************************************
        * Function:          LogTest
        ******************************************************************************/
        private void LogTest(InvalidOperationException e, Freezable freezable, Freezable f, string description)
        {
            
            if (e.TargetSite.Name != "VerifyAccess")
            {
                GlobalLog.LogEvidence(description + freezable.GetType().ToString() + " from another thread into " + f.GetType().ToString() + " has different exception TargetSite: " + e.TargetSite.Name);
                failures.Add(description + freezable.GetType().ToString() + " from another thread into " + f.GetType().ToString() + " has different exception TargetSite: " + e.TargetSite.Name);
                passed &= false;
            }
             
         }

        /******************************************************************************
        * Function:          LogTest
        ******************************************************************************/
         private void LogTest(InvalidOperationException e, Freezable freezable, string description)
         {

             if (e.TargetSite.Name != "VerifyAccess")
             {
                 GlobalLog.LogEvidence(description + freezable.GetType().ToString() + " has different exception TargetSite: " + e.TargetSite.Name);
                 failures.Add(description + freezable.GetType().ToString() + " has different exception TargetSite: " + e.TargetSite.Name);
                 passed &= false;
             }

         }
        /******************************************************************************
        * Function:          LogTest
        ******************************************************************************/
        private void LogTest(TargetInvocationException e, Freezable freezable, Freezable f, string description)
        {

            if (e.InnerException.TargetSite.Name != "VerifyAccess" && e.InnerException.TargetSite.Name != "EnsureConsistentDispatchers" && e.InnerException.TargetSite.Name != "SetPrologue")
            {
                GlobalLog.LogEvidence(description + freezable.GetType().ToString() + " from another thread into " + f.GetType().ToString() + " has different exception TargetSite: " + e.InnerException.TargetSite.Name);
                failures.Add(description + freezable.GetType().ToString() + " from another thread into " + f.GetType().ToString() + " has different exception TargetSite: " + e.InnerException.TargetSite.Name);
                passed &= false;
            }

        }
        #endregion
    }
}
  


