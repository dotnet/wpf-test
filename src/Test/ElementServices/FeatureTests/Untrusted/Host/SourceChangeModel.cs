// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/****************  SourceChangeModel Model *****************
 *    Area: Avalon Core Test
 **********************************************************/

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Interop;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Trusted.Controls;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Modeling;
using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.Modeling.HwndHosting
{
    
    /// <summary>
    /// ChangingSourceModel Model class
    /// </summary>
    [Model(@"FeatureTests\ElementServices\SourceChangeTraversalMultiple.xtc", 2, @"Hosting\HwndHost\SourceChangeModel", TestCaseSecurityLevel.FullTrust, "SourceChangeModel_MultipleTraversal", ExpandModelCases=true, Area = "AppModel", Disabled = true)]
    [Model(@"FeatureTests\ElementServices\SourceChangeSingleTraversal.xtc", 2, @"Hosting\HwndHost\SourceChangeModel", TestCaseSecurityLevel.FullTrust, "SourceChangeModel_SingleTraversal", ExpandModelCases = true, Area = "AppModel", Disabled = true)]    
    public class ChangingSourceModel : CoreModel 
    {

        /// <summary>
        /// Creates a ChangingSourceModel Model instance
        /// </summary>
        public ChangingSourceModel(): base()
        {
            Name = "ChangingSourceModel";
            Description = "ChangingSourceModel Model";
            ModelPath = "FinalChangingSourceModel.ite";

            //Attach Event Handlers
            OnBeginCase += new StateEventHandler(OnBeginCase_Handler);
            OnEndCase += new StateEventHandler(OnEndCase_Handler);

            //Add StateVariables
            AddStateVariable("SourceState");
            AddStateVariable("AvalonTreeState");
            AddStateVariable("DispatcherState");
            AddStateVariable("HwndHostState");
            AddStateVariable("SourceAvalonTreeConnectionState");
            AddStateVariable("HwndHostAvalonTreeConnectionState");
            AddStateVariable("HwndHostSourceConnectionState");
            AddStateVariable("HwndHostActionState");
            base.OnEndCaseOnNestedPump += new EventHandler(OnEndCaseOnNestedPump_Handler);

            
            //Add Action Handlers
            AddAction("AddAvalonTreeToSource", new ActionHandler(AddAvalonTreeToSource));
            AddAction("AddHwndHostToAvalonTree", new ActionHandler(AddHwndHostToAvalonTree));
            AddAction("VerifyActionForHwndHost", new ActionHandler(VerifyActionForHwndHost));
            AddAction("ClickHwndHost", new ActionHandler(ClickHwndHost));
            AddAction("CreateHwndHost", new ActionHandler(CreateHwndHost));
            AddAction("ListenHwndHostMessage", new ActionHandler(ListenHwndHostMessage));
            AddAction("RemoveAvalonTreeFromSource", new ActionHandler(RemoveAvalonTreeFromSource));
            AddAction("RemoveHwndHostFromAvalonTree", new ActionHandler(RemoveHwndHostFromAvalonTree));
            AddAction("RunDispatcher", new ActionHandler(RunDispatcher));

            //

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
            
            //


            _avalon = new SourceChangeModelAutomationHelper(TestLog.Current, this.AsyncActions);
        }




        /// <summary>
        /// Sets the Model as necessary to begin the case with the given State
        /// </summary>
        private void OnEndCaseOnNestedPump_Handler(object o,EventArgs args)
        {
            _avalon.StopDispatcher();
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
            _avalon.Dispose();
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Handler for AddAvalonTreeToSource</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool AddAvalonTreeToSource(State endState, State inParams, State outParams)
        {

            _avalon.SetAvalonTreeToSource();
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Handler for AddHwndHostToAvalonTree</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool AddHwndHostToAvalonTree(State endState, State inParams, State outParams)
        {
            _avalon.SetHwndHostToAvalonTree();
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Handler for VerifyActionForHwndHost</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool VerifyActionForHwndHost(State endState, State inParams, State outParams)
        {
            _avalon.VerifyActionForHwndHost();
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Handler for ClickHwndHost</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool ClickHwndHost(State endState, State inParams, State outParams)
        {

            _avalon.ClickHwndHost();

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Handler for CreateHwndHost</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool CreateHwndHost(State endState, State inParams, State outParams)
        {           
            string controlType = (string)inParams["ControlType"];
            
            _avalon.CreateHwndHostControl(controlType);
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Handler for ListenHwndHostMessage</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool ListenHwndHostMessage(State endState, State inParams, State outParams)
        {
            string state = (string)endState["HwndHostActionState"];

            if (state == "Listened")
            {
                _avalon.ListenHwndHost();
            }
            else
            {
                _avalon.CleanListenHwndHost();
            }

            
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Handler for RemoveAvalonTreeFromSource</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool RemoveAvalonTreeFromSource(State endState, State inParams, State outParams)
        {

            _avalon.RemoveAvalonTreeFromSource();
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Handler for RemoveHwndHostFromAvalonTree</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool RemoveHwndHostFromAvalonTree(State endState, State inParams, State outParams)
        {
            _avalon.RemoveHwndHostFromAvalonTree();
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Handler for RunDispatcher</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool RunDispatcher(State endState, State inParams, State outParams)
        {
            _avalon.RunDispatcher();
    
            return true;
        }


        SourceChangeModelAutomationHelper _avalon = null;


    }

    ///<summary>
    ///</summary>
    public class SourceChangeModelAutomationHelper
    {

        ///<summary>
        ///</summary>
        public SourceChangeModelAutomationHelper(TestLog logger, AsyncActionsManager actions)
        {

            if (logger == null)
                throw new ArgumentNullException("Logger cannot be null");
            
            if (actions == null)
                throw new ArgumentNullException("AsyncActionsManager cannot be null");

            _asyncActions = actions;
            _logger = logger;

            
           

            _logger.LogEvidence("Creating a Dispatcher");

            // Creating Context
            _dispatcher = Dispatcher.CurrentDispatcher;

            _createInitialState(null);

            

        }

        object _createInitialState(object o)
        {
            //Buiding AvalonTree
            Border border = new Border();

            _rootAvalonTree = border;
            border.Background = Brushes.Blue;

            StackPanel panel = new StackPanel();

            _nodePanelTree = panel;
            border.Child = panel;

            _sourceWindow = SourceHelper.CreateHwndSource(400,400,0,0);

            return null;
        }


        ///<summary>
        ///</summary>
        public void CreateHwndHostControl(string name)
        {

            _logger.LogEvidence("CreateHwndHostControl");

            if (_hwndHost != null)
                throw new Exception("Only Support it to be call once");

            _createHwndHostControl( name);

        }

        ///<summary>
        ///</summary>
        object  _createHwndHostControl(object o)
        {
            string name = (string)o;
            
            if ("Win32Button" == name)
            {
                Win32ButtonControl hwndHostControl = new Win32ButtonControl();
                hwndHostControl.Click += new EventHandler(_clickEvent);
                hwndHostControl.Listen += new EventHandler(_listenEvent);
                
                _hwndHost = hwndHostControl;
                
            }

            _logger.LogEvidence("Creating a HwndHost control of type: " + name);
            
            
            PostNextActionOnAvalonQueue();

            return null;
        }

        ///<summary>
        ///</summary>
        public void ClickHwndHost()
        {
            if (_dispatcherrunning)
            {
                _logger.LogEvidence("Click on HwndHost");
                MouseHelper.Click((IntPtr)((IWin32Window)_hwndHost).Handle, MouseLocation.CenterLeft);          
            }
            else
            {
                _logger.LogEvidence("The dispatcher is not running: ClickHwndHost action is NOP");
            }
        }

        ///<summary>
        ///</summary>
        public void ListenHwndHost()
        {
            if (_hwndHost == null)
                throw new InvalidOperationException("Test Case is on invalid state");
            
            if (_dispatcherrunning) 
            {
                if (_hwndHost is Win32ButtonControl)
                {
                    Win32ButtonControl b = (Win32ButtonControl)_hwndHost;
                    b.PostMessageToWindow();
                    b.PostMessageToWindow();
                }
            }
            else
            {
                _logger.LogEvidence("The dispatcher is not running: ListenHwndHost action is NOP");
            }
        }


        ///<summary>
        ///</summary>
        public void CleanListenHwndHost()
        {
            if (_dispatcherrunning) 
            {
                if (_listenCount != 2)
                    throw new Microsoft.Test.TestValidationException("Expecting to listen count equal two. Value = " + _listenCount.ToString());

                _listenCount = 0;
                PostNextActionOnAvalonQueue();
            }
            else
            {
                _logger.LogEvidence("The dispatcher is not running: CleanListenHwndHost action is NOP");
            }
        }


        ///<summary>
        ///</summary>
        public void RemoveHwndHostFromAvalonTree()
        {
            _removeHwndHostFromAvalonTree(null);
        }


        object _removeHwndHostFromAvalonTree(object o)
        {
            _logger.LogEvidence("Removing HwndHost From Avalon Tree");

            if (!_nodePanelTree.Children.Contains(_hwndHost))            
                throw new Exception("There is no HwndHost on the Tree");

            _nodePanelTree.Children.Remove(_hwndHost);

            PostNextActionOnAvalonQueue();


            return null;
        }

        ///<summary>
        ///</summary>
        public void RemoveAvalonTreeFromSource()
        {
            _removeAvalonTreeFromSource(null);            
        }

        object _removeAvalonTreeFromSource(object o)
        {
            _logger.LogEvidence("Removing the Avalon Tree from the source");
            _sourceWindow.RootVisual = null;
            PostNextActionOnAvalonQueue();         
            return null;
        }



        ///<summary>
        ///</summary>
        public void RunDispatcher()
        {
            _logger.LogEvidence("Running the dispatcher");
            _dispatcherrunning = true;
            PostNextActionOnAvalonQueue();
            Dispatcher.Run();
        }


        ///<summary>
        ///</summary>
        public void SetAvalonTreeToSource()
        {
            _setAvalonTreeToSource(null);                  
        }

        object _setAvalonTreeToSource(object o)
        {
            _logger.LogEvidence("Setting Avalon Tree to the Source");
            _sourceWindow.RootVisual = (Visual)_rootAvalonTree;
            PostNextActionOnAvalonQueue();
            
            return null;
        }


        ///<summary>
        ///</summary>
        public void SetHwndHostToAvalonTree()
        {
            _setHwndHostToAvalonTree(null);           
        }


        object _setHwndHostToAvalonTree(object o)
        {
            _logger.LogEvidence("Setting the HwndHost to an Avalon Tree");
            _nodePanelTree.Children.Add(_hwndHost);

            PostNextActionOnAvalonQueue();   
            
            return null;
        }



        ///<summary>
        ///</summary>
        public void StopDispatcher()
        {
            _logger.LogEvidence("Stopping the Dispatcher");
            _dispatcher.InvokeShutdown();
            _dispatcherrunning = false;
        }


        ///<summary>
        ///</summary>
        public void VerifyActionForHwndHost()
        {
            if (_dispatcherrunning)
            {
                if (!_hwndHostLastClick)
                    throw new Microsoft.Test.TestValidationException("Expecting a click on the button");

                _hwndHostLastClick = false;
                
                _logger.LogEvidence("Validates the click on the HwndHost");
                
                PostNextActionOnAvalonQueue();
            }
            else
            {
                _logger.LogEvidence("The dispatcher is not running: VerifyActionForHwndHost action is NOP");
            }
        }
        
        ///<summary>
        ///</summary>
        public HwndHost HostControl 
        {
            get
            {
                return _hwndHost;
            }
        }

        ///<summary>
        ///</summary>
        public void Dispose()
        {
            _logger.LogEvidence("Disposing the Context");

            if (_dispatcherrunning == true)
                _dispatcher.InvokeShutdown();           
        }
        

        ///<summary>
        ///</summary>        
        public Visual RootAvalonTree
        {
            get
            {
                return _rootAvalonTree;
            }
        }


        ///<summary>
        ///</summary>
        public Panel NodePanelTree
        {
            get
            {
                return _nodePanelTree;
            }
        }

        ///<summary>
        ///</summary>
        public HwndSource SourceWindow
        {
            get
            {
                return _sourceWindow;
            }
        }

        private void _clickEvent(object sender, EventArgs args)
        {
            _logger.LogEvidence("Click received!");
            _hwndHostLastClick = true;
            PostNextActionOnAvalonQueue();  
        }

        private void _listenEvent(object sender, EventArgs args)
        {
            _listenCount++;
            
            
            if (_listenCount == 2)
            {
                PostNextActionOnAvalonQueue();
            }
            else if (_listenCount > 2)
            {
                throw new InvalidOperationException(" Listening HwndHost Bad State");
            }
        }

        private void PostNextActionOnAvalonQueue()
        {
            if (_dispatcherrunning)
                _dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback (PostModelNextAction),null);
        }

        private object PostModelNextAction(object o)
        {
           _asyncActions.ExecuteAsyncNextAction();
           return null;
        }


        HwndHost _hwndHost = null;
        HwndSource _sourceWindow = null;
        Panel _nodePanelTree = null;
        Visual _rootAvalonTree = null;
        Dispatcher _dispatcher = null;
        bool _dispatcherrunning = false;

        AsyncActionsManager _asyncActions = null;
        TestLog _logger = null;   
        bool _hwndHostLastClick = false;

        int _listenCount = 0;
        
    }
        


}

