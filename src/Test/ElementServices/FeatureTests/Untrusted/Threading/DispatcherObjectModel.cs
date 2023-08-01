// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Creating DispatcherObject objects and Validating the its properties
 * 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using System.Windows.Threading;
using System.Windows;

using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Threading;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Modeling;
using Microsoft.Test.Serialization;
using Microsoft.Test.Threading;
using Microsoft.Test.Win32;


using Avalon.Test.CoreUI.Win32;

namespace Avalon.Test.CoreUI.Threading
{

    /// <summary>
    /// State based model.
    /// Creating DispatcherObject objects and Validating the its properties
    /// The model can be found at:
    /// %SDXROOT%\testsrc\windowstestdata\REDIST\client\wcptests\core\Base\Testcases\Threading\DispatcherObjectModel.mbt    
    /// </summary>
    [Model(@"FeatureTests\ElementServices\DispatcherObjectModelAllTransitions.xtc",1,@"Threading\DispatcherObject",TestCaseSecurityLevel.FullTrust,
      "Creating DispatcherObject objects and Validating the its properties", ExpandModelCases = true, Area = "ElementServices")]
    //[MultipleThreadTestCaseModel(@"FeatureTests\ElementServices\DispatcherObjectModelAllTransitions.xtc", 3, 1, @"Threading\DispatcherObject",TestCaseSecurityLevel.FullTrust,        "Creating DispatcherObject objects and Validating the its properties")]    
    public class DispatcherObjectModel : CoreModel 
    {
        /// <summary>
        /// Creates a DispatcherOperationCompleteModel Model instance
        /// </summary>
        public DispatcherObjectModel(): base()
        {
            Name = "DispatcherObjectModel";
            Description = "Model DispatcherObjectModel";
            ModelPath = "MODEL_PATH_TOKEN";

            //Attach Event Handlers
            OnBeginCase += new StateEventHandler(OnBeginCase_Handler);
            OnEndCase += new StateEventHandler(OnEndCase_Handler);
            OnEndCaseOnNestedPump += new EventHandler(OnEndCaseOnNestedPump_Handler);
            
            //Add Action Handlers
            AddAction("CreateDO", new ActionHandler(CreateDO));
            AddAction("DetachFromDispatcher", new ActionHandler(DetachFromDispatcher));
            AddAction("Setup", new ActionHandler(Setup));
            AddAction("DispatcherShutdown", new ActionHandler(DispatcherShutdown));
            AddAction("Verify", new ActionHandler(Verify));            

        }


        /// <summary>
        /// Sets the Model as necessary to begin the case with the given State
        /// </summary>
        /// <remarks>
        /// Attached to OnBeginCase event which is fired by the Traversal
        /// before a new case begins
        /// </remarks>
        /// <param name="sender">Model that fired the event</param>
        /// <param name="e">The Initial State in a StateEventArgs</param>
        private void OnBeginCase_Handler(object sender, StateEventArgs e)
        {
            _automation = DispatcherObjectModelAutomation.Current; //(this.AsyncActions);
        }


        /// <summary>
        /// Sets the Model as necessary when a case ends with the given State
        /// </summary>
        /// <remarks>
        /// Attached to OnEndCase event which is fired by the Traversal
        /// after a case ends
        /// </remarks>
        /// <param name="sender">Model that fired the event</param>
        /// <param name="e">The End State in a StateEventArgs</param>
        private void OnEndCase_Handler(object sender, StateEventArgs e)
        {
            _automation.Dispose();
        }


        /// <summary>
        /// Callback that is called when alls transitions for the current test case on the 
        /// XTC are completed. For example you may want to exited the Dispatcher or
        /// or close the nested pump.
        /// </summary>
        private void OnEndCaseOnNestedPump_Handler(object o,EventArgs args)
        {
            _automation.Dispose();
        }

        /// <summary>
        /// </summary>
        private bool CreateDO( State endState, State inParameters, State outParameters )
        {
            CoreLogger.LogStatus( "Action CreateDO" );
            CoreLogger.LogStatus( inParameters.ToString() );

            bool succeed = true;

            /*
               inParameters["Constructor"] - // 
            */

            _automation.CreateDO(inParameters);

            return succeed;
        }

        /// <summary>
        /// </summary>
        private bool DetachFromDispatcher( State endState, State inParameters, State outParameters )
        {
            CoreLogger.LogStatus( "Action DetachFromDispatcher" );

            CoreLogger.LogStatus( inParameters.ToString() );

            bool succeed = true;

            _automation.DetachFromDispatcher(inParameters);
            return succeed;
        }

        /// <summary>
        /// </summary>
        private bool DispatcherShutdown( State endState, State inParameters, State outParameters )
        {
            CoreLogger.LogStatus( "Action DispatcherShutdown" );

            //CoreLogger.LogStatus( inParameters.ToString() );

            bool succeed = true;

            
            _automation.DispatcherShutdown(inParameters);
            return succeed;
        }

        /// <summary>
        /// </summary>
        private bool Setup( State endState, State inParameters, State outParameters )
        {
            CoreLogger.LogStatus( "Action Setup" );

            CoreLogger.LogStatus( inParameters.ToString() );


            bool succeed = true;

            _automation.Setup(inParameters);
            return succeed;
        }




        /// <summary>
        /// </summary>
         private bool Verify( State endState, State inParameters, State outParameters )
        {
            CoreLogger.LogStatus( "Action Verify" );

            //CoreLogger.LogStatus( inParameters.ToString() );


            bool succeed = true;

            _automation.Verify(inParameters);
            return succeed;
        }
        DispatcherObjectModelAutomation _automation = null;
        
    }


    /// <summary>
    /// State based model.
    /// Creating DispatcherObject objects and Validating the its properties
    /// The model can be found at:
    /// %SDXROOT%\testsrc\windowstestdata\REDIST\client\wcptests\core\Base\Testcases\Threading\DispatcherObjectModel.mbt  
    /// </summary>
    public class DispatcherObjectModelAutomation : ModelAutomationBase
    {

        enum ConstructorsValues
        {
            Default = 0,
            Freezable = 1

        }

        /// <summary>
        /// </summary>
        public override void Dispose()
        {
            VerifyAll();
        }


        /// <summary>
        /// Creates a DispatcherObject
        /// </summary>
        public static DispatcherObjectModelAutomation Current
        {
            get
            {
                DispatcherObjectModelAutomation current = s_current;

                if (current != null)
                {
                    return current;
                }                    
                
                lock(s_syncRoot)
                {
                    if (s_current == null)
                    {
                        s_current = new DispatcherObjectModelAutomation(null);                        
                    }
                }

                current = s_current;
                return current;
            }
        }

        private static DispatcherObjectModelAutomation s_current = null;
        private static object s_syncRoot = new object();


        /// <summary>
        /// </summary>
        private DispatcherObjectModelAutomation(AsyncActionsManager asyncManager):base(asyncManager)
        {                        
            _dispatcher = Dispatcher.CurrentDispatcher;                       
        }


        /// <summary>
        /// Sets up the test case depending on the dispatcher to use.
        /// </summary>
        public void Setup(IDictionary dictionary)
        {
            DispatcherState dState = ParsingDispatcherState(dictionary);

            switch(dState)
            {
                case DispatcherState.NotRunning:
                    _dispatcher = Dispatcher.CurrentDispatcher;
                    break;

                case DispatcherState.Shutdown:
                    _dispatcher = Dispatcher.CurrentDispatcher;
                    DispatcherHelper.ShutDown(_dispatcher);
                    break;
                                            
            }
        }


        Dispatcher _dispatcher = null;

        /// <summary>
        /// Creates a DispatcherObject
        /// </summary>
        public void CreateDO(IDictionary dictionary)
        {
            // dictionary["Constructor"]


            ConstructorsValues ctr = ParsingConstructor(dictionary);

            DispatcherObject dObject = null;

            dObject = DispatcherObjectWrapper.CreateDO((int)ctr);
            if (dObject.Dispatcher == null)
            {
                CoreLogger.LogTestResult(false, "The object always should be bound.");
            }
            t_currentDO = (IDetachDispatcher)dObject;
            AddDOToList((IDetachDispatcher)dObject);

            
        }


        /// <summary>
        /// 
        /// </summary>
        public void DispatcherShutdown(IDictionary dictionary)
        {
            DispatcherHelper.ShutDown(Dispatcher.CurrentDispatcher);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Verify(IDictionary dictionary)
        {
            
            VerifyPrivate(t_currentDO);
        }


        /// <summary>
        /// 
        /// </summary>
        public void DetachFromDispatcher(IDictionary dictionary)
        {
            bool internalDeatch = ParsingInternalDetach(dictionary);
            DetachItems(internalDeatch);
        }


        void VerifyPrivate(IDetachDispatcher dObject)
        {

            if (dObject == null)
            {
                throw new InvalidOperationException("Test failure.");
            }
            if (dObject.IsDetach)
            {

                if (!((DispatcherObject)dObject).CheckAccess() && dObject.DetachProcessStatus == DetachProcessStatus.Completed)
                {
                    CoreLogger.LogTestResult(false, "CheckAccess on a detched Object failed. Expecing: True");
                }
                if (dObject.DetachProcessStatus == DetachProcessStatus.Completed)
                {
                    ((DispatcherObject)dObject).VerifyAccess();
                }

                if (((DispatcherObject)dObject).Dispatcher != null && dObject.DetachProcessStatus == DetachProcessStatus.Completed)
                {
                    CoreLogger.LogTestResult(false, "The Dispatcher property should be null.");
                }

                while (dObject.DetachProcessStatus != DetachProcessStatus.Completed)
                {

                }
                if (!((DispatcherObject)dObject).CheckAccess())
                {
                    CoreLogger.LogTestResult(false, "CheckAccess on a detched Object failed. Expecing: True");
                }

                ((DispatcherObject)dObject).VerifyAccess();

                if (((DispatcherObject)dObject).Dispatcher != null)
                {
                    CoreLogger.LogTestResult(false, "The Dispatcher property should be null.");
                }

            }
            else
            {
                if (dObject.Thread == Thread.CurrentThread)
                {
                    if (!((DispatcherObject)dObject).CheckAccess())
                    {
                        CoreLogger.LogTestResult(false, "CheckAccess on a Attached Object failed. Expecing: True");
                    }

                    ((DispatcherObject)dObject).VerifyAccess();

                    if (((DispatcherObject)dObject).Dispatcher.Thread != Thread.CurrentThread)
                    {
                        CoreLogger.LogTestResult(false, "Dispatcher.Thread property is not matching.");
                    }
                }
                else
                {
                    if (((DispatcherObject)dObject).CheckAccess())
                    {
                        CoreLogger.LogTestResult(false, "CheckAccess on a Attached Object failed. Expecing: False");
                    }

                    bool exceptionCaught = false;

                    try
                    {
                        ((DispatcherObject)dObject).VerifyAccess();
                    }
                    catch (InvalidOperationException)
                    {
                        exceptionCaught = true;
                    }

                    if (!exceptionCaught)
                    {
                        CoreLogger.LogTestResult(false, "Expecting an InvalidOperationException.");
                    }

                    if (((DispatcherObject)dObject).Dispatcher.Thread == Thread.CurrentThread)
                    {
                        CoreLogger.LogTestResult(false, "Dispatcher.Thread property should not match.");
                    }

                }

            }
        }


        private static DispatcherState ParsingDispatcherState(IDictionary dictionary)
        {
            string state = (string)dictionary["DispatcherState"];
            DispatcherState ds = (DispatcherState)Enum.Parse(typeof(DispatcherState),state,true /* ignore case */ );
            return ds;
        }



        static bool ParsingInternalDetach(IDictionary dictionary)
        {
            if (String.Compare("InternalDetach",(string)dictionary["Count"], true) == 0)
            {
                return true;
            }

            return false;
        }

        static ConstructorsValues ParsingConstructor(IDictionary dictionary)
        {
            string ctr = (string)dictionary["Constructor"];

            if (String.Compare("Default", ctr, true) == 0)
            {
                return (ConstructorsValues)0;
            }

            if (String.Compare("Freezable", ctr, true) == 0)
            {
                return (ConstructorsValues)1;
            }


            throw new InvalidOperationException("Constructor param is invalid.");
        }


        void VerifyAll()
        {
            int counter = 0;
            int itemCount = 0;

            Interlocked.Exchange(ref itemCount, s_doList.Count);


            IDetachDispatcher itemObject = null;
            
            while(counter < itemCount)
            {
                lock(s_doList)
                {                    
                    if (counter < itemCount)
                    {
                        itemObject = s_doList[counter];
                    }
                    else
                    {
                        break;
                    }

                    itemCount = s_doList.Count;
                }
                counter++;
                
                VerifyPrivate(itemObject);
            }           
        }



        static void DetachItems(bool internalDetach)
        {
            if (internalDetach)                
            {
                t_currentDO.Detach(internalDetach);
            }
        }

        static void AddDOToList(IDetachDispatcher d)
        {
            lock(s_doList)
            {
                s_doList.Add(d);
            }
        }

        [ThreadStatic]
        static IDetachDispatcher t_currentDO = null;
        static List<IDetachDispatcher> s_doList = new List<IDetachDispatcher>();
 
    } 


    /// <summary>
    /// Wraps some internal behavior for Dispatcherobject class
    /// </summary>    
    public static class DispatcherObjectWrapper
    {

        /// <summary>
        /// Creates a DO using the specifed constructor.
        /// </summary> 
        public static DispatcherObject CreateDO(int constructor)
        {
            switch(constructor)
            {               
                case 0:
                    return new DispatcherObjectTest();
                case 1:
                    return new DispatcherObjectFreezableTest();              
            }
            
            return null;
        }



        /// <summary>
        /// Detach the DO from the dispatcher.
        /// </summary> 
        public static void DetachFromDispatcher(DispatcherObject dObject)
        {
            Type type = dObject.GetType();
            type.InvokeMember(
                "DetachFromDispatcher", 
                BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic, 
                null,
                dObject,
                null);

        }

    }


    interface IDetachDispatcher
    {
        void Detach(bool internalDetach);

        bool IsDetach 
        {
            get;
            set;
        }

        Thread Thread 
        {
            get;
        }

        DetachProcessStatus DetachProcessStatus
        {
            get;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum DetachProcessStatus
    {
        /// <summary>
        /// 
        /// </summary>
        NoStarted,
        /// <summary>
        /// 
        /// </summary>
        Started,
        /// <summary>
        /// 
        /// </summary>
        Completed
    }

    /// <summary>
    /// </summary> 
    public class DispatcherObjectTest : DispatcherObject, IDetachDispatcher
    {
        /// <summary>
        /// </summary> 
        public DispatcherObjectTest() : base()
        {
            _thread = Thread.CurrentThread;
        }

        /// <summary>
        /// </summary> 
        void IDetachDispatcher.Detach(bool internalDetach)
        {
            _detachStatus = DetachProcessStatus.Started;
            _thread = null;
            _isDetach = true;            
            DispatcherObjectWrapper.DetachFromDispatcher(this);
            _detachStatus = DetachProcessStatus.Completed;
        }

        /// <summary>
        /// 
        /// </summary>
        DetachProcessStatus IDetachDispatcher.DetachProcessStatus
        {
            get
            {
                return _detachStatus;
            }
        }

        /// <summary>
        /// </summary> 
        bool IDetachDispatcher.IsDetach 
        {
            get
            {
                return _isDetach;
            }
            set
            {
                _isDetach = value;
            }
        }

        /// <summary>
        /// </summary> 
        Thread IDetachDispatcher.Thread 
        {
            get
            {           
                return _thread;
            }
        }

        bool _isDetach = false;
        DetachProcessStatus _detachStatus = DetachProcessStatus.NoStarted;
        Thread _thread = null;            
    }

    /// <summary>
    /// </summary> 
    public class DispatcherObjectFreezableTest : Freezable, IDetachDispatcher
    {
        /// <summary>
        /// </summary> 
        public DispatcherObjectFreezableTest() : base()
        {
            _thread = Thread.CurrentThread;
        }

        /// <summary>
        /// </summary> 
        protected override Freezable CreateInstanceCore()
        {
            return new DispatcherObjectFreezableTest();
        }

        /// <summary>
        /// 
        /// </summary>
        DetachProcessStatus IDetachDispatcher.DetachProcessStatus
        {
            get
            {
                return _detachStatus;
            }
        }

        /// <summary>
        /// </summary> 
        void IDetachDispatcher.Detach(bool internalDetach)
        {
            _detachStatus = DetachProcessStatus.Started;
            _thread = null;            
            _isDetach = true;
            if (internalDetach)
            {
                DispatcherObjectWrapper.DetachFromDispatcher(this);
            }
            else
            {
                this.Freeze();
            }
            _detachStatus = DetachProcessStatus.Completed;
        }

        /// <summary>
        /// </summary> 
        Thread IDetachDispatcher.Thread
        {
            get
            {           
                return _thread;
            }
        }

        
        /// <summary>
        /// </summary> 
        bool IDetachDispatcher.IsDetach 
        {
            get
            {
                return _isDetach;
            }
            set
            {
                _isDetach = value;
            }
        }
        DetachProcessStatus _detachStatus = DetachProcessStatus.NoStarted;
        bool _isDetach = false;
        Thread _thread = null;

    }

    
}



