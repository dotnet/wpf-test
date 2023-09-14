// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************************
*    Source Control Information
*    
 
  
*    Revision:         $Revision: 1 $
 
*
******************************************************************************/

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Input;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Trusted.Controls;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Modeling;
using Microsoft.Test.Win32;

namespace Avalon.Test.Hosting
{    
    /// <summary>
    /// KeyboardInterop Model class
    /// Removing this due a change on the dev code
    /// </summary>
    //[TestCaseModel("RegistrationOne.xtc","3",@"bla",TestCaseSecurityLevel.FullTrust,"")]    
    // todo: what happened to this?
    public class KeyboardInterop : Model 
    {

        /// <summary>
        /// Creates a KeyboardInterop Model instance
        /// </summary>
        public KeyboardInterop(): base()
        {
            Name = "KeyboardInterop";
            Description = "KeyboardInterop Model";
            ModelPath = "KeyboardInterop.ite";

            //Attach Event Handlers
            OnBeginCase += new StateEventHandler(OnBeginCase_Handler);
            OnEndCase += new StateEventHandler(OnEndCase_Handler);
            base.OnEndCaseOnNestedPump += new EventHandler(OnEndCaseOnNestedPump_Handler);

            //Add StateVariables
            AddStateVariable("SourceState");
            AddStateVariable("HostState");
            AddStateVariable("PositionState");
            AddStateVariable("RegisterSourceState");
            
            //Add Action Handlers
            AddAction("CreateSource", new ActionHandler(CreateSource));
            AddAction("CreateHost", new ActionHandler(CreateHost));
            AddAction("RegisterSource", new ActionHandler(RegisterSource));
            AddAction("UnRegisterSource", new ActionHandler(UnRegisterSource));
            AddAction("RegisterHost", new ActionHandler(RegisterHost));
            AddAction("UnRegisterHost", new ActionHandler(UnRegisterHost));
            
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

            _support = new KeyboardSupport(this.AsyncActions);
            _support.RunDispatcher();
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

            _support.Dispose();
        }

        
        /// <summary>
        /// Callback that is called when alls transitions for the current test case on the 
        /// XTC are completed. For example you may want to exited the Dispatcher or
        /// or close the nested pump.
        /// </summary>
        private void OnEndCaseOnNestedPump_Handler(object o,EventArgs args)
        {
            _support.Dispose();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Handler for CreateSource</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool CreateSource(State endState, State inParams, State outParams)
        {
			
			string str = (string)inParams["SourceType"];
            LogComment("SourceType value: " + str);
            _support.CreateSource(str);

			_currentState = endState;
			return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Handler for CreateHost</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool CreateHost(State endState, State inParams, State outParams)
        {

            string[] array = {inParams["HostType"], inParams["Position"], inParams["NavMode"]};
            LogComment("Position value: " + inParams["Position"] + ";HostType value: " + inParams["HostType"] 
                + "; NavMode: " + inParams["NavMode"]);

            _support.CreateHost(array);

			_currentState = endState;
			return true;
        }






        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Handler for CreateSource</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool RegisterSource(State endState, State inParams, State outParams)
        {
			string[] array = { inParams["Times"], _currentState["RegisterSourceState"] };
			LogComment("Times value: " + inParams["Times"] + "; State: " + endState.ToString());

			_support.RegisterSource(array);

			_currentState = endState;
			return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Handler for CreateHost</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool UnRegisterSource(State endState, State inParams, State outParams)
        {

			string[] array = { inParams["Times"], _currentState["RegisterSourceState"] };
			LogComment("Times value: " + inParams["Times"] + "; State: " + endState.ToString());

            _support.UnRegisterSource(array);

			_currentState = endState;
			return true;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Handler for CreateSource</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool RegisterHost(State endState, State inParams, State outParams)
        {
			string[] array = { inParams["Times"], _currentState["RegisterHostState"] };
			LogComment("Times value: " + inParams["Times"]);

            _support.RegisterHost(array);

			_currentState = endState;
			return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Handler for CreateHost</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool UnRegisterHost(State endState, State inParams, State outParams)
        {

			string[] array = { inParams["Times"], _currentState["RegisterHostState"] };
			LogComment("Times value: " + inParams["Times"]);

            _support.UnRegisterHost(array);

			_currentState = endState;
			return true;
        }


		State _currentState = null;
		KeyboardSupport _support = null;
    }


    ///<summary>
    /// This class is intent to be reusable by each different test case during the model
    ///</summary>
    public class KeyboardSupport
    {

        ///<summary>
        /// Build a Dispatcher and the dispatcher
        ///</summary>
        public KeyboardSupport(AsyncActionsManager asyncManager)
        {
            this._asyncManager = asyncManager;

            _dispatcher = Dispatcher.CurrentDispatcher;

        }

        ///<summary>
        /// Start running the dispatcher
        ///</summary>
        public void RunDispatcher()
        {
            _isDispatcherRunning = true;
            _postNextActionOnAvalonQueue();
            Dispatcher.Run();
        }


        ///<summary>
        /// UnRegister a Source from the Dispatcher, synchronous
        ///</summary>
        public void UnRegisterSource(string[] array)
        {
            unRegister(new registerSinkHelper(null, (IKeyboardInputSink)returnHwndSource(), array[0], array[1]));
			_postNextActionOnAvalonQueue();

        }

        ///<summary>
        /// UnRegister the Host from the Source, synchronous
        ///</summary>
        public void UnRegisterHost(string[] array)
        {
            unRegister(new registerSinkHelper(returnHwndSource(), _hwndHost, array[0], array[1]));
			_postNextActionOnAvalonQueue();

        }


        object unRegister(object o)
        {
            registerSinkHelper helper = (registerSinkHelper)o;

            string times = helper.Times;
			string state = helper.State;

            IKeyboardInputSink child = helper.Child;


            if (state == "UnRegistered")
            {
                if (child.KeyboardInputSite != null)
                {
                    throw new Microsoft.Test.TestValidationException("The Site must be NULL");
                }
                return null;

            }


            IKeyboardInputSite site = child.KeyboardInputSite;

            if (child.KeyboardInputSite != null && child is HwndSource)
            {
                throw new Microsoft.Test.TestValidationException("The Site on the Sink must be NULL");
            }
            else if (child.KeyboardInputSite == null && !(child is HwndSource))
            {
                throw new Microsoft.Test.TestValidationException("The Site on the Sink must be NULL");
            }


            if (site != null)
            {

                if (site.Sink != child)
                {
                    throw new Microsoft.Test.TestValidationException("Site.Sink value is not the expected");
                }

                site.Unregister();

                if (child.KeyboardInputSite != null)
                {
                    throw new Microsoft.Test.TestValidationException("The Site on the Sink Must be NULL");
                }


                //
                /*if (site.Sink != null)
                {
                   throw new Microsoft.Test.TestValidationException("Site.Sink value is not Null");
                }*/


                if (times == "Multiple")
                {
                    try
                    {
                        site.Unregister();
                    }
                    catch (InvalidOperationException) { }
                }

            }

            if (child.KeyboardInputSite != null)
            {
                throw new Microsoft.Test.TestValidationException("The Site on the Sink Must be NULL");
            }

            //
            /*
            if (site.Sink != null)
            {
               throw new Microsoft.Test.TestValidationException("Site.Sink value is not Null");
            }
			*/


            return null;
        }


        ///<summary>
        /// Register the Host from the Source, synchronous
        ///</summary>
        public void RegisterSource(string[] array)
        {
            try
            {
                register(
                    new registerSinkHelper(null, returnHwndSource(),array[0],array[1]));
            }
            finally
            {
                _postNextActionOnAvalonQueue();
            }
        }

        ///<summary>
        /// Register the Host from the Source, synchronous
        ///</summary>
        public void RegisterHost(string[] array)
        {
            try
            {
                register(new registerSinkHelper((IKeyboardInputSink)returnHwndSource(), (IKeyboardInputSink)_hwndHost,array[0],array[1]));
            }
            finally
            {
                _postNextActionOnAvalonQueue();
            }
        }

        ///<summary>
        /// this is a support class to pass information to the DoCallback on the Context
        ///</summary>
        class registerSinkHelper
        {
            ///<summary>
            /// CCTOR
            ///</summary>
            public registerSinkHelper(IKeyboardInputSink parent, IKeyboardInputSink child, string times, string state)
            {
                Parent = parent;
                Child = child;
                Times = times;
				State = state;
			}

            public IKeyboardInputSink Parent;
            public IKeyboardInputSink Child;
            public string Times;
			public string State;
		}


        object register(object o)
        {

            registerSinkHelper helper = (registerSinkHelper)o;

            string times = helper.Times;
			string state = helper.State;
            IKeyboardInputSink parent = helper.Parent;

            if (times == "null")
            {
                try
                {
                    if (parent != null)
                    {
                        parent.RegisterKeyboardInputSink(null);
                    }
                }
                catch(ArgumentNullException){}                
                return null;
            }

			IKeyboardInputSink child = helper.Child;

			if (state != "UnRegistered")
			{

				if (child is HwndSource && child.KeyboardInputSite != null)
				{
                    throw new Microsoft.Test.TestValidationException("The Site on the Sink Must Not be NULL");
				}


				if (child.KeyboardInputSite != null && child.KeyboardInputSite.Sink != child)
				{
                    throw new Microsoft.Test.TestValidationException("Site.Sink value is not the HwndSource");
				}


			
				return null;
			}


			if (child.KeyboardInputSite != null)
			{
                throw new Microsoft.Test.TestValidationException("The Site on the Sink Must be NULL");
			}


            
            return null;
        }


        ///<summary>
        /// This method create a HwndHost inside a context
        ///</summary>
        public void CreateHost(string[] array)
        {

            _createHost( array);
            _postNextActionOnAvalonQueue();
        }

        private object _createHost(object stringArray)
        {
        
            string str = ((string[])stringArray)[0];
            string position = ((string[])stringArray)[1];


            
            // This section is about which host control create
            
            if (str == "NestedAvalon")  
            {
                _hwndHost = new AvalonHwndControl();

                AvalonHwndControl c = (AvalonHwndControl)_hwndHost;
                Button b = new Button();
                b.Content = "Button Inside Avalon that it is HwndHost";
                b.Width = 100;
                b.Height = 50;

                DockPanel dp = new DockPanel();
                dp.Children.Add(b);
                c.RootVisual = dp;
                
            }
            else if (str == "SingleHwnd")
            {
                _hwndHost = new SingleHwndControl();

            }
            else if (str == "MultipleHwnd")
            {
                _hwndHost = new MultipleHwndControl();
            }
            else if (str == "NoTabAllow")
            {
                _hwndHost = new NoTabAllowControl();

                NoTabAllowControl c = (NoTabAllowControl)_hwndHost;
                Button b = new Button();
                b.Content = "Bla";
                c.RootVisual = b;
            }

            // Making the HwndHost focusable

            _hwndHost.Focusable = true;

            
            // Set the position

            if (position == "Alone")
            {
                _rootVisual.Children.Add(_hwndHost);
                return null;
            }

            Button b1 = new Button();
            b1.Content = "B1";

            Button b2 = new Button();
            b2.Content = "B2";
            
            if (position == "Begin")
            {
                _rootVisual.Children.Add(_hwndHost);
                _rootVisual.Children.Add(b1);
                _rootVisual.Children.Add(b2);
            }
            else if (position == "Middle")
            {
                _rootVisual.Children.Add(b1);
                _rootVisual.Children.Add(_hwndHost);
                _rootVisual.Children.Add(b2);
            }
            else if (position == "End")
            {
                _rootVisual.Children.Add(b1);
                _rootVisual.Children.Add(b2);
                _rootVisual.Children.Add(_hwndHost);
            }            
            else
            {
                throw new InvalidOperationException("Automation error, not expected value: " + position);
            }
            

            return null;  

        }


        ///<summary>
        /// Creates the HwndSource on a Dispatcher
        ///</summary>
        public void CreateSource( string sourceType)
        {
            _createSource( sourceType);
            _postNextActionOnAvalonQueue();
            
        }

        private object _createSource(object sourceType)
        {
            _rootVisual = new StackPanel();


            string str = (string)sourceType;
            

            if (str == "HwndSource")  
            {
                HwndSource s = SourceHelper.CreateHwndSource(400,400,0,0);
                NativeMethods.SetForegroundWindow(new HandleRef(null,s.Handle));
                _source = s;
                s.RootVisual = _rootVisual;

            }
            else
            {
                Window w = new Window();
                w.Show();
                w.Activate();
                _source = w;
                w.Content = _rootVisual;
            }
            
            return null;            
        }

        

        ///<summary>
        /// Dispose the test case, after this, this class cannot be reuse
        ///</summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                if (_isDispatcherRunning)
                {
                    Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.SystemIdle);
                }
            }
        }

        ///<summary>
        /// Utility to return a HwndSource from the source, only for convinience
        ///</summary>
        HwndSource returnHwndSource()
        {
            HwndSource source = null;
            
            if (_source is HwndSource)
            {
                source = (HwndSource)_source;
                
            }
            else if (_source is Window)
            {
                source = (HwndSource) _returnHwndSource( _source);                
            }
            else
            {
                throw new InvalidOperationException("Unexpected Source type or Null");
            }

            return source;
        }


        object _returnHwndSource(object o)
        {
            return PresentationSource.FromVisual((Window)o);
        }

        //<summary>
        //
        //  This method check if we are inside of a nested pump. if we already on a pump we need to pull the next ITE Action from the 
        //  the model.  Right now we ask to pull inside of a queue item. There is no need for this, //



        private void _postNextActionOnAvalonQueue()
        {
            if (_isDispatcherRunning)
            {
                _dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback (_postModelNextAction),null);
            }
        }

        //<summary>
        //
        //  Asking to pull the next ITE action
        //  
        //
        //</summary>
        private object _postModelNextAction(object o)
        {
           _asyncManager.ExecuteAsyncNextAction();
           return null;
        }


        AsyncActionsManager _asyncManager = null;
        Dispatcher _dispatcher = null;
        bool _isDispatcherRunning = false;

        HwndHost _hwndHost = null;
        object _source;
        Panel _rootVisual;

        bool _isDisposed = false;

    }

}

//This file was generated using ITE on: Sunday, May 23, 2004 3:08:00 PM
