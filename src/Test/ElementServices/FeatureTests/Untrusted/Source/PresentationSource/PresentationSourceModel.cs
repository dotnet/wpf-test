// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Modeling;
using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.Hosting
{
    ///<summary>
    ///</summary>
    ///<remarks>
    ///     <filename>RegisterTwiceSameContext.cs</filename>
    ///</remarks>
    [TestDefaults]
    public class PresentationModelCurrentSources : TestCase
    {
        

        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public PresentationModelCurrentSources() :base(TestCaseType.None)
        {

        }

        /// <summary>
        /// </summary>
        public override void Run(){}


        /// <summary>
        ///  Adding 10 items (HwndSource and TestSOurceOne), Null out 2 TestSources
        ///  HwndSOurce doesn't go out of scope (


        [TestAttribute(0, @"Source\PresentationSource", TestCaseSecurityLevel.FullTrust, "PresentationSourceCleanUpDead", Area = "AppModel")]
        public void CleanUpDeadRef()
        {
            CoreLogger.BeginVariation();
            AvalonPresentationSource av = new AvalonPresentationSource();

            for (int i = 0; i<5; i++)
            {
                av.AddSource("HwndSource");
                av.AddSource("TestSourceOne");
            }

            av.VerifyList();

            //Actually nulling out position 1 and 5 from the original
            int[] indexes = {1,4};

            av.NullOutIndex(indexes);

            av.VerifyList();

            av.QueryRootVisual();

            CoreLogger.EndVariation();
        }



        /// <summary>
        ///  Creating a custom PresentationSource and calling Twice AddSource.
        ///  Validates only 1 source is on the CurrentSources
        ///  Later calling RemoveSource twice. 
        ///  Doing this process twice
        /// </summary>
        [TestAttribute(0, @"Source\PresentationSource", TestCaseSecurityLevel.FullTrust, "PresentationSourceAddSource2xRemove2x", Area = "AppModel")]
        public void AddSourceTwiceandRemoveTwice()
        {

            addTwice(null);
        }

        object addTwice(object o)
        {
            CoreLogger.BeginVariation();
            TestSourceOne s = new TestSourceOne();
            s.AddSourceAgain();

            int count = 0;
            foreach(PresentationSource source in PresentationSource.CurrentSources)
            {
                count++;
            }

            if (count != 1)
                throw new Microsoft.Test.TestValidationException("Only 1 PresentationSource should be added");
            

            s.RemoveSourceAgain();
            s.Dispose(); // I call RemoveSource during Dispose

            count = 0;

            foreach(PresentationSource source in PresentationSource.CurrentSources)
            {
                count++;
            }



            if (count != 0)
                throw new Microsoft.Test.TestValidationException("Only  PresentationSource should be added");

            TestSourceOne s1 = new TestSourceOne();

            count = 0;
            foreach(PresentationSource source in PresentationSource.CurrentSources)
            {
                count++;
            }

            if (count != 1)
                throw new Microsoft.Test.TestValidationException("Only 1 PresentationSource should be added. Step 2");


            s1.Dispose(); // I call RemoveSource during Dispose

            count = 0;

            foreach(PresentationSource source in PresentationSource.CurrentSources)
            {
                if (source != s1)
                {
                    throw new Microsoft.Test.TestValidationException("The expecting source is not on the list");
                }
                count++;
            }
            
            s.RootVisual = null;
            s1.RootVisual = null;
            CoreLogger.EndVariation();  
            return null;
             
        }


        /// <summary>
        /// </summary>
        [TestAttribute(0, @"Source\PresentationSource", TestCaseSecurityLevel.FullTrust, "PresentationSourceCleanupDeadRefAdd", Area = "AppModel")]
        public void CleanUpDeadRefAdd()
        {
            CoreLogger.BeginVariation();
            AvalonPresentationSource av = new AvalonPresentationSource();

            try
            {
                for (int i = 0; i<5; i++)
                {
                    av.AddSource("HwndSource");
                    av.AddSource("TestSourceOne");
                }

                av.VerifyList();

                //Actually nulling out position 1 and 5 from the original
                int[] indexes = {1,4};

                av.NullOutIndex(indexes);

                av.VerifyList();


                for (int i = 0; i<5; i++)
                {
                    av.AddSource("HwndSource");
                    av.AddSource("TestSourceOne");
                }

                av.VerifyList();
                av.VerifyList();

                int[] indexes2 = {17};
                av.NullOutIndex(indexes2);

                av.AddSource("TestSourceOne");
                av.VerifyList();


                av.QueryRootVisual();
            }
            finally
            {
                av.Dispose();
            }
            CoreLogger.EndVariation();

        }

    }
    
    /// <summary>
    /// PresentationSourceModel Model class
    /// </summary>
    [Model(@"FeatureTests\ElementServices\PresentationSourceAddRemove.xtc", 1, 550, 1, @"Source\PresentationSource\CurrentSources", TestCaseSecurityLevel.FullTrust, "PresentationSourceModel", Description = "Adding, Removing and Verifying PresentationSource. Using Modeling", Timeout = 300, Area="AppModel")]
    [Model(@"FeatureTests\ElementServices\PresentationSourceAddRemove.xtc", 550, 800, 1, @"Source\PresentationSource\CurrentSources", TestCaseSecurityLevel.FullTrust, "PresentationSourceModel", Description = "Adding, Removing and Verifying PresentationSource. Using Modeling", Timeout = 300, Area = "AppModel")]
    [Model(@"FeatureTests\ElementServices\PresentationSourceModel.xtc", 1, @"Source\PresentationSource\CurrentSources", TestCaseSecurityLevel.FullTrust, "PresentationSourceModel", Description = "Adding, Removing and Verifying PresentationSource. Using Modeling", Timeout = 120, Area = "AppModel")]        
    public class PresentationSourceModel : CoreModel 
    {
        /// <summary>
        /// Creates a untitled Model instance.
        /// </summary>
        public PresentationSourceModel(): base()
        {
            Name = "PresentationSourceModel";
            Description = "Presentation Source Model";
            ModelPath = "MODEL_PATH_TOKEN";
            

            //Attach Event Handlers
            OnGetCurrentState += new StateEventHandler(OnGetCurrentState_Handler);
            OnBeginCase += new StateEventHandler(OnBeginCase_Handler);
            OnEndCase += new StateEventHandler(OnEndCase_Handler);
            base.OnEndCaseOnNestedPump += new EventHandler(OnEndCaseOnNestedPump_Handler);


            //Add StateVariables
            AddStateVariable("AmountSource");
            AddStateVariable("VerifyList");

            
            //Add Action Handlers
            AddAction("AddSource", new ActionHandler(AddSource));
            AddAction("VerifyList", new ActionHandler(VerifyList));
            AddAction("Remove", new ActionHandler(Remove));
        }


        /// <summary>
        /// Gets the current State of the Model
        /// </summary>
        /// <remarks>
        /// Attached to OnGetCurrentState event which is fired after
        /// each action to validate
        /// </remarks>
        /// <param name="sender">Model that fired the event</param>
        /// <param name="e">The current State in a StateEventArgs</param>
        private void OnGetCurrentState_Handler(object sender, StateEventArgs e)
        {
            // The state values set here will be compared to the expected state by the Model's default ValidateState function.
            // Only put code here that sets the State object to represent the current State of the Model.
            
            //
            e.State["AmountSource"] = null; //
            e.State["VerifyList"] = null;   //
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
            // The state values passed in here are what the traversal expects the current/begin state to be
            // Put code here that sets the system state to match the current State object
            // Any code that needs to be run at the start of each case can also go here
            
            _av = new AvalonPresentationSource();
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
            // The state values passed in here are what the traversal expects the current/end state to be
            // Put code here that sets the system state to match the current State object
            // Any code that needs to be run at the end of each case can also go here
            
            _av.Dispose();
        }


        /// <summary>
        /// Callback that is called when alls transitions for the current test case on the 
        /// XTC are completed. For example you may want to exited the Dispatcher or
        /// or close the nested pump.
        /// </summary>
        private void OnEndCaseOnNestedPump_Handler(object o,EventArgs args){}


        

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Handler for AddSource</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool AddSource(State endState, State inParams, State outParams)
        {
            //Action Params (listed here for convienence during coding)
            //inParams["Type"] - 

           _av.AddSource(inParams["Type"]);

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Handler for VerifyList</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool VerifyList(State endState, State inParams, State outParams)
        {
            _av.VerifyList();

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Handler for VerifyList</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool Remove(State endState, State inParams, State outParams)
        {
            //Action Params (listed here for convienence during coding)
            //inParams["Position"] -             
            _av.RemoveSource(inParams["Position"]);

            return true;
        }


        AvalonPresentationSource _av = null;

    }


    /// <summary>
    /// 
    /// </summary>
    public class AvalonPresentationSource
    {

        /// <summary>
        /// 
        /// </summary>
        public AvalonPresentationSource()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            QueryRootVisual();
            LogEvidence("Dispose()");
            for (int i = 0; i<_sources.Count; i++)
            {
                ((IDisposable)_sources[i]).Dispose();
            }            _sources = null;
            _dispatcher.InvokeShutdown();

        }


        /// <summary>
        /// 
        /// </summary>
        public void RemoveSource(string type)
        {
            LogEvidence("Removing Source: " + type);

            int index = 0;
            string s = type;

            if (_sources.Count == 0)
                throw new InvalidOperationException("HOW?");

            if (s == "Initial")
            {
                index = 0;
            }
            else if (s == "Middle")
            {
                if (_sources.Count >= 2)
                {
                    index = _sources.Count/2;
                }
                else
                {
                    LogEvidence("Actually Removing the first one source");
                    index = 0;
                }
            }
            else if (s == "End")
            {
                index = _sources.Count - 1;
            }
            
            
            disposeObject(_sources[index]);



        }

        void disposeObject(object o)
        {
            PresentationSource ps = (PresentationSource)o;
            
            if (o is HwndSource)
            {
                HwndSource s = (HwndSource)o;
                s.Dispose();
                _sources.Remove(ps);
            }
            else if(o is TestSourceOne)
            {
                TestSourceOne ts1 = (TestSourceOne)o;
                ts1.Dispose();
                _sources.Remove(ps);
            }
            else if(o is TestSourceTwo)
            {
                TestSourceTwo ts2 = (TestSourceTwo)o;
                ts2.Dispose();
                _sources.Remove(ps);
            }                
        }



        /// <summary>
        /// 
        /// </summary>       
        public void AddSource(string type)
        {
            LogEvidence("Adding Source: " + type);

            string s = type;

            if (s == "TestSourceOne" || s == "TestOneSource") 
            {
                _sources.Add(new TestSourceOne());
            
            }
            else if (s == "TestSourceTwo")
            {
                _sources.Add(new TestSourceTwo());
            }
            else if (s == "HwndSource")
            {
                _sources.Add(SourceHelper.CreateHwndSource(500, 500,0,0));

            }
 
        }

        /// <summary>
        /// 
        /// </summary>
        public void VerifyList()
        {
            LogEvidence("Verify List");
 
            int count = 0;
            foreach (object source in PresentationSource.CurrentSources)
            {
                if (!_sources.Contains((PresentationSource)source))
                {
                    throw new Microsoft.Test.TestValidationException("Bizarre " + source.GetType().Name);
                }
                count++;                
            }

            if (count != _sources.Count)
            {
                throw new Microsoft.Test.TestValidationException("The count of sources doesn't match");
            }

            ArrayList list = new ArrayList();
            
            foreach (object source in PresentationSource.CurrentSources)
            {
                if (list.Contains(source))
                {
                    throw new Microsoft.Test.TestValidationException("The PresentationSOurce is duplicated");
                }

                list.Add(source);

            }
            list.Clear();
            list = null;
            
            GoThruEnumerator(_sources.Count, PresentationSource.CurrentSources.GetEnumerator());
            

        }


        void GoThruEnumerator(int amount, IEnumerator  enumerable)
        {
            bool isExceptionThrown = false;

            try
            {
                object o = enumerable.Current;
            }
            catch (InvalidOperationException)
            {
                isExceptionThrown = true;
            }

            if (!isExceptionThrown)
            {
                throw new Exception("Getting the Current before a MOveNext must throw an exception");
            }

            int count = 0;

            for (int j=0; j<amount; j++)
            {
                for (int i=0; i<count; i++)
                {
                    if (!enumerable.MoveNext())
                    {
                        throw new Microsoft.Test.TestValidationException("The Move should be possible");
                    }
                    if (enumerable.Current != _sources[--count])
                    {
                        throw new Microsoft.Test.TestValidationException("Current Enumerable should return null before MoveNext");
                    }
                }

                enumerable.Reset();
                
                isExceptionThrown = false;
                
                try
                {
                    object o = enumerable.Current;
                }
                catch (InvalidOperationException)
                {
                    isExceptionThrown = true;
                }

                if (!isExceptionThrown)
                {
                    throw new Exception("Getting the Current after Reset");
                }
                
                count++;
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        public void NullOutIndex(int[] index)
        {
            LogEvidence("Nulling out some reference");
            if (index == null || index.Length <= 0)
                throw new ArgumentException();

            int count = _sources.Count;

            for (int i = 0;i < index.Length; i++)
            {
                _sources.RemoveAt(index[i]);
            }


            GC.Collect(GC.MaxGeneration);
            GC.WaitForPendingFinalizers();
            GC.Collect(GC.MaxGeneration);

            Thread.Sleep(3000);
        }

        /// <summary>
        /// 
        /// </summary>
        public void QueryRootVisual()
        {
            ArrayList list = new ArrayList();
            
            for (int i = 0; i < _sources.Count; i++)
            {
                list.Add(_sources[i].RootVisual);
            }

            list.Clear();
        }


        void LogEvidence(string str)
        {
            TestLog.Current.LogEvidence(str);
        }

        ///<summary>
        ///</summary>
        public List<PresentationSource> Sources
        {
            get
            {
                return _sources;
            }
        }


        List<PresentationSource> _sources = new List<PresentationSource>();
        Dispatcher _dispatcher = null;
    }


    /// <summary>
    /// 
    /// </summary>        
    public class TestSourceOne : PresentationSource, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public TestSourceOne()
        {
            base.AddSource();
            
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _isDispose = true;
            base.RemoveSource();
        }


        /// <summary>
        /// 
        /// </summary>
        public void AddSourceAgain()
        {
            base.AddSource();
        }

        /// <summary>
        /// 
        /// </summary>
        public void RemoveSourceAgain()
        {
            base.RemoveSource();
        }


        /// <summary>
        ///     The root visual being presented in the source.
        /// </summary>
        public override Visual RootVisual
        {
            get
            {
                return _root;
            }
            set
            {
                _root = value;
            }
        }

        Visual _root;

        /// <summary>
        ///     The visual manager for the visuals being presented in the source.
        /// </summary>
        protected override CompositionTarget GetCompositionTargetCore() 
        {
            return null;
        }



        /// <summary>
        /// 
        /// </summary>
        public override bool IsDisposed
        {
            get
            {
                return _isDispose;
            }
        }

        bool _isDispose = false;

    }

    /// <summary>
    /// 
    /// </summary>        
    public class TestSourceTwo : PresentationSource , IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public TestSourceTwo()
        {
            base.AddSource();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _isDispose = true;
            
            base.RemoveSource();
        }


        /// <summary>
        ///     The root visual being presented in the source.
        /// </summary>
        public override Visual RootVisual
        {
            get
            {
                return _root;
            }
            set
            {
                _root = value;
            }
        }

        Visual _root;

        /// <summary>
        ///     The visual manager for the visuals being presented in the source.
        /// </summary>
        protected override CompositionTarget  GetCompositionTargetCore()
        {
            return null;
        }



        /// <summary>
        /// 
        /// </summary>
        public override bool IsDisposed
        {
            get
            {
                return _isDispose;
            }
        }

        bool _isDispose = false;

    }



}

//This file was generated using MDE on: Thursday, August 19, 2004 6:52:08 PM
