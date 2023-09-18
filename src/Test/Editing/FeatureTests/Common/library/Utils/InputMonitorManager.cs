// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 11 $ $Source: //depot/vbl_wcp_avalon/windowstest/client/wcptests/uis/Common/Library/Utils/InputMonitorManager.cs $")]

namespace Test.Uis.Utils
{
    #region Namespaces.

    using System;
    using System.Threading; 
    using System.Windows.Threading;
    using System.Collections;
    using System.Windows.Input;
    using System.Windows;
    
    using System.Diagnostics;

#if DEBUG_INPUTMONITOR
    using Test.Uis.Loggers;
#endif
    #endregion

    /// <summary>
    /// This class keeps the Avalon queue spinning until
    /// pending input items have been processed.
    /// </summary>
    /// <remarks>
    /// The life of InputMonitorManager starts with InputMonitorManager.Initialize
    /// which creates the instance of InputMonitorManager. Afterwards any reference 
    /// to InputMonitorManager is done through InputMonitorManager.Current. The initialization
    /// of InputMonitorManager must be done in the main UI thread since it needs 
    /// the UI context to post items. The ctor is made private so no other InputMonitorManager
    /// can be created.
    /// 
    /// The idea is that InputMonitorManager keeps posting ContextIdle items to the Avalon queue (see
    /// sdk for description on DispatcherPriority), when it is aware of any expected input needs to
    /// be received. This is accomplished by keeping track of all InputMonitor objects (They are 
    /// "registered" thtough AddInputMonitorToList) Whenever any item in the list has input count
    /// > 0 meaning "I am still waiting for some kind of expected input to be received, and please don't dispatch
    /// the next SystemIdle item". It keeps posting ContextIdle items until all InputMonitor registered
    /// to have input count == 0. At that time it knows all expected input has been processed
    /// and it is the right time to move on. Simply speaking, InputMonitorManager keeps "padding"
    /// the Avalon queue with ContextIdle items when it needs to wait for input.
    /// </remarks>
    public class InputMonitorManager
    {

        #region Constructors

        /// <summary>
        /// An overload of ctor which allows you to specify a particular
        /// Dispatcher to post items. For most of the time the main UI thread
        /// Dispatcher is needed. See InputMonitorManager() for details
        /// </summary>
        /// <param name="dispatcher"></param>
        private InputMonitorManager(Dispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }

            this._inputMonitorList = new ArrayList();
            this._dispatcher = dispatcher;
            this._isEnabled = false;
        }

        #endregion Constructors


        #region Public methods.

        /// <summary>
        /// Given an InputMonitorManager instance (via InputMonitorManager.Current) and 
        /// an InputMonitor instance
        /// AddInputMonitorToList adds that to the internal ArrayList, followed by 
        /// calling InputMonitor.Attach if that InputMonitor instance has not already
        /// attached (See InputMonitor.Attach) for details. InputMonitor is an abstract class
        /// of the real thing you want (e.g. MousePostProcessInputMonitor)
        /// </summary>
        /// <param name="inputMonitor">Keyboard / Mouse input monitor instance</param>
        public void AddInputMonitor(InputMonitor inputMonitor)
        {
            bool inputCountConsolidated = false;

            if (this._isEnabled)
            {
                //
                // if inputMonitor has already attached we don't need to do this
                // again. We need to guarantee that inputMonitor is attached. The
                // reason is that during the course of devlopment I found that
                // if someone forgets to call Attach on inputMonitor and use it right away
                // the reference counting on InputMonitor will not work, and it loops and loops
                // in InputMonitorManager.WakeQueue. The Debug.Assert allows this to be debugged
                // easily.
                // 
                if (!inputMonitor.IsAttached)
                {
                    //
                    // We need to have thread in Dispatcher active here
                    // If you look at InputMonitor.Attach it needs access to
                    // Avalon InputManager. Remember AddInputMonitorToList
                    // can be invoked from a worker thread (e.g. when input
                    // is going to be simulated in a worker thread)
                    // 
                    this._dispatcher.Invoke(DispatcherPriority.Normal, new DispatcherOperationCallback(AttachInputMonitor), inputMonitor);
                }

                Debug.Assert(inputMonitor.IsAttached, "InputMonitor is not attached!");

                // We cannot add the list yet.
                // If there already exists InputMonitor monitoring the same
                // input events we need to consolidate the count in both
                //
                for (int i = 0; i < this._inputMonitorList.Count; i++)
                {
                    InputMonitor existingInputMonitor = this._inputMonitorList[i] as InputMonitor;
                    if (existingInputMonitor.IsSameKind(inputMonitor))
                    {
                        existingInputMonitor.ConsolidateInputCount(inputMonitor);
                        inputCountConsolidated = true;
                        break;
                    }
                }

                //
                // If we can't find any existing InputMonitor of the same kind
                // we add that to the list right away.
                // 
                if (!inputCountConsolidated)
                {
                    this._inputMonitorList.Add(inputMonitor);
                }

                //
                // WakeQueue is actually recursively posting an ContextIdle item 
                // to the Avalon queue, until it can't find any InputMonitor
                // in the internal list to have input count > 0, which actually means
                // "Now all expected input has been processed, and we are ready to
                // get to the next item."
                // 
                WakeQueue();
            }
#if DEBUG_INPUTMONITOR
            else
            {
                Logger.Current.Log("DEBUG_INPUTMONITOR: InputMontior is disabled. AddInputMonitorToList is no-op");
            }
#endif
        }

        /// <summary>
        /// This is where the life of InputMonitorManager starts. Client (now it is class TestTypes)
        /// calls this method to initialize InputMonitorManager with a static reference to an
        /// instance. If the static reference has already been initialized this will throw InvalidOperationException
        /// </summary>
        public static void Initialize(Dispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
            }

            if (s_inputMonitorManager != null)
            {
                // Throw only if an attempt was made to initialize in a different
                // dispatcher.
                if (dispatcher != s_inputMonitorManager._dispatcher)
                {
                    throw new InvalidOperationException("InputMonitorManager has already been initialized for a different Dispaacher.");
                }

                // Leave the manager as it was otherwise.
                return;
            }

            s_inputMonitorManager = new InputMonitorManager(dispatcher);
        }

        #endregion


        #region Public properties

        /// <summary>
        /// This is the access point of the static InputMonitorManager instance
        /// </summary>
        /// <value></value>
        public static InputMonitorManager Current
        {
            get
            {
                //
                // throw if it is not initialized
                //
                if (s_inputMonitorManager == null)
                {
                    throw new InvalidOperationException("InputMonitorManager is not initialized");
                }

                return s_inputMonitorManager;
            }
        }

        /// <summary>
        /// Gets or sets whether the monitor will keep the
        /// Avalon queue spinning.
        /// </summary>
        /// <remarks>
        /// You can either use InputMonitorManagerDisabled="true" in xml
        /// or in code InputMonitorManager.Current.IsEnabled = false;
        /// </remarks>
        public bool IsEnabled
        {
            get
            {
                return this._isEnabled;
            }
            set
            {
                this._isEnabled = value;
            }
        }

        #endregion


        #region Private methods.

        /// <summary>
        /// This method goes through the internal list
        /// and remove any InputMonitor items with input count
        /// == 0 (which means they are not needed anymore).
        /// Compact the list so that the memory can be reclaimed
        /// </summary>
        /// <returns>bool. true means there are still "active"
        /// InputMonitor instance, meaning that some input events 
        /// are being waited for, false otherwise.</returns>
        private bool CleanInputMonitorList()
        {
            ArrayList newList = new ArrayList();

            //
            // go through the list and add the "active" InputMonitor
            // items to newList
            // 
            for (int i = 0; i < this._inputMonitorList.Count; i++)
            {
                InputMonitor inputMonitor = this._inputMonitorList[i] as InputMonitor;

                if (inputMonitor != null)
                {
                    if (inputMonitor.CurrentInputCount > 0)
                    {
                        newList.Add(inputMonitor);
                    }
                }
            }

            //
            // Let the old one garbage collected
            //
            this._inputMonitorList = newList;

            //
            // return if there exists active InputMonitor
            //             
            return (this._inputMonitorList.Count > 0);
        }

        /// <summary>
        /// This method is a synchronise method so that
        /// inputMonitor.Attach can be called in a specified 
        /// Dispatcher
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private object AttachInputMonitor(object o)
        {
            InputMonitor inputMonitor = o as InputMonitor;

            //
            // InputMonitor.Attach needs to access Avalon InputManager,
            // requiring access to Dispatcher
            //
            if (inputMonitor != null)
            {
                inputMonitor.Attach();
            }
            else
            {
                throw new InvalidOperationException("o passed is not an InputMonitor. Most likely an Avalon bug");
            }

            return null;
        }

        /// <summary>
        /// WakeQueue recursively posts the ContextIdle item until 
        /// CleanInputMonitorList returns false (which means no 
        /// InputMonitor instance is active)
        /// WARNING WARNING WARNING:
        /// The code here is very timing sensitive. I have noticed that if it 
        /// spews to the CONSOLE everytime when this method is called it will
        /// hold up the input queue and input events are not dispatched.
        /// If you plan to change the code in this method please please please
        /// to be absolutely sure what you are doing. The code embraced in DEBUG_INPUTMONITOR
        /// should NOT be enabled UNLESS you are doing debugging on your own.
        /// NEVER enable this code when the code is going to run with Piper or in
        /// the lab.
        /// </summary>
        private void WakeQueue()
        {
            if (!this._isEnabled)
            {
#if DEBUG_INPUTMONITOR
                Logger.Current.Log("DEBUG_INPUTMONITOR: InputMontior is disabled. Clearing up InputMonitorList");
#endif
                //
                // Clean up any existing InputMonitors 
                // 
                for (int i = 0; i < this._inputMonitorList.Count; i++)
                {
                    InputMonitor inputMonitor = this._inputMonitorList[i] as InputMonitor;

                    inputMonitor.Detach();
                }

                this._inputMonitorList.Clear();
            }
            else
            {

                //
                // DON'T enable this unless you are doing debugging on your own
                //
#if DEBUG_INPUTMONITOR
                Logger.Current.Log("DEBUG_INPUTMONITOR: InputMonitorManager.WakeQueue");
#endif
                if (CleanInputMonitorList())
                {
                    //
                    // DON'T enable this unless you are doing debugging on your own
                    //
#if DEBUG_INPUTMONITOR
                    DumpInternalList();
#endif
                    //
                    // post an item to itself
                    // 
                    (new QueueHelper(this._dispatcher, DispatcherPriority.ContextIdle)).QueueDelegate(new SimpleHandler(WakeQueue));
                }
            }
        }

#if DEBUG_INPUTMONITOR
        private void DumpInternalList()
        {
            Logger.Current.Log("Dumping internal list:");
            Logger.Current.Log("");
            Logger.Current.Log("");
            for (int i = 0; i < this._inputMonitorList.Count; i++)
            {
                Logger.Current.Log("[{0}]", i.ToString());
                Logger.Current.Log("=====================");

                InputMonitor monitor = this._inputMonitorList[i] as InputMonitor;

                Logger.Current.Log(monitor.ToString());
                Logger.Current.Log("");
            }
        }
#endif
    
        #endregion Private methods.


        #region Private fields.

        /// <summary>List of items being monitored..</summary>
        private ArrayList _inputMonitorList;

        /// <summary>Context that the manager is monitoring.</summary>
        private Dispatcher _dispatcher;

        /// <summary>Whether the manager tracks input items.</summary>
        private bool _isEnabled;

        /// <summary>This is the static instance to InputMonitorManager</summary>
        private static InputMonitorManager s_inputMonitorManager = null;

        #endregion
    }
}
