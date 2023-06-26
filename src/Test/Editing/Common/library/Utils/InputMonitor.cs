// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Keyboard and mouse emulation services for test cases.

namespace Test.Uis.Utils
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Threading; using System.Windows.Threading;
    using System.Windows;
    using Microsoft.Test.Input;
    using System.Windows.Input;

    #endregion

    /// <summary>
    /// An abstract class for all InputMonitors, which do
    /// reference counting for Avalon input PostProcess events.
    /// </summary>
    /// <remarks>
    /// Children classes need to implement the abstract PostProcessInput
    /// method. This will be registered in Attach method to Avalon
    /// InputManager and will be called by it when PostProcess event happens
    /// Detach is only called by child classes since they are the ones who
    /// know when to clean up (remove the handlers from Avalon InputManager)
    /// Future elaboration may include the support of more Avalon InputManager 
    /// events (not just NotifyInput??). and more children classes can be devloped
    /// to track different kinds of input (stylus??)
    /// </remarks>
    public abstract class InputMonitor
    {
        #region Constructors

        /// <summary>
        /// Constructor (more common use)
        /// This initializes the expected input count to the number equal to count
        /// and it creates an instance of ProcessInputEventHandler
        /// </summary>
        /// <param name="count">Expected input count, must be positive</param>
        protected InputMonitor(int count)
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException("count must be positive");
            }
            this._countOfInput = count;
        }

        #endregion

        #region Public methods
        /// <summary>
        /// The caller needs to call Attach on an input monitor instance 
        /// to get things rolling. The method makes sure that Attach can only be
        /// called on an detached item (default after being created) by checking 
        /// the state of internal member _handler. In addition to that it starts 
        /// placing items at ApplicationIdle priority to occupy the dispatcher so that
        /// it won't dispatch any SystemIdle priority items.
        /// </summary>
        public void Attach()
        {
            if (this._handler != null)
            {
                throw new InvalidOperationException("InputMonitor has already been attached");
            }

            this._handler = new ProcessInputEventHandler(PostProcessInput);
            
            //
            // remember, this needs Dispatcher access
            //
             
            InputManager.Current.PostProcessInput += this._handler;
        }

        /// <summary>
        /// Detach the event handler
        /// Detach is expected to be called by children classes, not the client
        /// the reason is that the client is supposed to instantiate an instance
        /// of InputMonitor, call Attach and it is done. The client shouldn't need
        /// to worry about cleaning these up.
        /// Exception: InvalidOperationException when _handler has been initialized (this can only be
        /// initialized in Attach())
        /// </summary>
        public void Detach()
        {
            if (this._handler == null)
            {
                throw new InvalidOperationException("InputMonitor has already been detached");
            }

            InputManager.Current.PostProcessInput -= this._handler;
            this._handler = null;
        }


        /// <summary>
        /// This method is called by InputMonitorManager to consolidate
        /// the waited input events count of two InputMonitors into
        /// one that has already existed. 
        /// </summary>
        /// <param name="inputMonitor"></param>
        /// <returns></returns>
        public int ConsolidateInputCount(InputMonitor inputMonitor)
        {
            if (!IsSameKind(inputMonitor))
            {
                throw new InvalidOperationException("InputMonitor cannot be consolidated if they are not instances of the same kind");
            }

            this._countOfInput += inputMonitor.CurrentInputCount;
            return this._countOfInput;
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Exposes the internal count
        /// I don't actually know if we need to expose the setter, but 
        /// if we allow default ctor we should allow someone to set the internal
        /// count when the default intance is created
        /// </summary>
        /// <value>Current count</value>
        public int CurrentInputCount
        {
            get { return this._countOfInput; }
        }

        /// <summary>
        /// Return true if Attach on this InputMonitor is called, false otherwise
        /// </summary>
        /// <value></value>
        public bool IsAttached
        {
            get { return (this._handler != null); } 
        }



        /// <summary>
        /// Children classes call this method to decrement the internal count
        /// if supplied argument > internal count it will throw exception. Thus 
        /// children classes have the responsibility to make sure that argument
        /// count is valid before calling into this method. The method returns
        /// the decremented count so we can do something like
        /// if (DecrementInputCount() == 0)
        /// Exception: InvalidOperationException when count > _countOfInput
        /// </summary>
        /// <param name="count">The desired count to be decremented from internal input count</param>
        /// <returns>The internal input count after it is decremented</returns>
        protected int DecrementInputCount(int count)
        {
            //
            // make sure we don't mess up internal count
            //
            if (this._countOfInput < count)
            {
                throw new InvalidOperationException("Input count goes negative");
            }

            this._countOfInput -= count;
            return this._countOfInput;
        }

        /// <summary>
        /// An overload to DecrementInputCount with 1 as the argument (most common usage)
        /// </summary>
        /// <returns></returns>
        protected int DecrementInputCount()
        {
            return DecrementInputCount(1);
        }        

        /// <summary>
        /// This has to be overriden by children classes upon PostProcessInput event
        /// </summary>
        /// <param name="sender">Sender of the PostProcessInput event, usually this is InputMonitor</param>
        /// <param name="args">See sdk for this</param>
        protected abstract void PostProcessInput(object sender, ProcessInputEventArgs args);

        /// <summary>
        /// Child classes have to implement this
        /// "Same kind" means more than just the supplied inputMonitor is the 
        /// instance of the same class. For example, two MousePostProcessInputMonitor
        /// which track different mouse events are not treated as the same kind. They 
        /// will be treated the same kind only if they are instances fro the same class
        /// and they track same input event
        /// </summary>
        /// <param name="inputMonitor"></param>
        /// <returns>true if they are the same kind, false otherwise</returns>
        public abstract bool IsSameKind(InputMonitor inputMonitor);

#if DEBUG_INPUTMONITOR
        /// <summary>
        /// For debug purpose
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string message = String.Format("Type: [{0}]\nCount: [{1}]\nHandler: [{2}]", 
                   this.GetType().ToString(),
                   CurrentInputCount.ToString(),
                   this._handler.Method.ToString());

            return message;
        }
#endif

        #endregion

        #region Private members

        /// <summary>
        /// the ProcessInputEventHandler instance
        /// </summary>
        private ProcessInputEventHandler _handler = null;

        /// <summary>
        /// internal count
        /// </summary>
        private int _countOfInput = 0;

        #endregion
    }

    /// <summary>
    /// A class to override InputMonitor to track Mouse events
    /// </summary>
    internal class MousePostProcessInputMonitor : InputMonitor
    {
        #region Constructors

        /// <summary>
        /// Default constructor. for most of the time you need to use the second form
        /// MousePostProcessInputMonitor(int count)
        /// </summary>
        public MousePostProcessInputMonitor() : this(1)
        {
        }

        /// <summary>
        /// This ctor allows you to specify the count of input to be waited for
        /// This assumes that it waits for MouseLeftButtonUp event only
        /// </summary>
        /// <param name="count">Number of mouse input</param>
        public MousePostProcessInputMonitor(int count) : this(count, Microsoft.Test.Input.RawMouseActions.Button1Release)
        {
        }

        /// <summary>
        /// This ctor allows count and RawMouseActions to be specified. RawMouseActions can be bitwised
        /// together. See sdk for details
        /// </summary>
        /// <param name="count">count of input</param>
        /// <param name="rawMouseActions">See sdk for details</param>
        public MousePostProcessInputMonitor(int count, Microsoft.Test.Input.RawMouseActions rawMouseActions) : base (count)
        {
            this._rawMouseActions = rawMouseActions;
        }

        /// <summary>
        /// Return true if the supplied InputMonitor is of type MousePostProcessInputMonitor
        /// and it tracks same RawMouseActions
        /// </summary>
        /// <param name="inputMonitor"></param>
        /// <returns></returns>
        public override bool IsSameKind(InputMonitor inputMonitor)
        {
            MousePostProcessInputMonitor mousePostProcessInputMonitor = inputMonitor as MousePostProcessInputMonitor;

            if (mousePostProcessInputMonitor != null)
            {
                return (mousePostProcessInputMonitor.RawMouseActions == this._rawMouseActions); 
            }

            return false;
        }


        /// <summary>
        /// return the tracked RawMouseActions
        /// </summary>
        /// <value></value>
        public RawMouseActions RawMouseActions
        {
            get { return this._rawMouseActions; }
        }

        #endregion

        #region Protected overrides

        /// <summary>
        /// This override the parent PostProcessInput handler
        /// </summary>
        /// <param name="sender">It is usually the InputManager</param>
        /// <param name="args">ProcessInputEventArgs, so that we know what the input is</param>
        protected override void PostProcessInput(object sender, ProcessInputEventArgs args)
        {
            //
            // we are only interested if it is an InputReportEventArgs instance
            // 
            if (InputReportEventArgsWrapper.IsCorrectType(args.StagingItem.Input))
            {
                InputReportEventArgsWrapper inputReportEventArgs = new InputReportEventArgsWrapper(args.StagingItem.Input);

                //
                // We are interested only in mouse events, and in the bubbling phrase
                //
                if (inputReportEventArgs.Report != null 
                    && inputReportEventArgs.Report.Type == InputType.Mouse
                    && inputReportEventArgs.RoutedEvent.RoutingStrategy == RoutingStrategy.Bubble)
                {
                    RawMouseInputReportWrapper mouseInputReport = new RawMouseInputReportWrapper(inputReportEventArgs.Report);

                    if ((mouseInputReport.Actions & this._rawMouseActions) == mouseInputReport.Actions)
                    {
                        if (base.DecrementInputCount() == 0)
                        {
                            //
                            // the reference count goes down to zero,
                            // all expected keyboard mouse events are seen
                            // and it's time to leave
                            //
                            base.Detach();
                        }
                    }
                }
            }
        }
        #endregion

#if DEBUG_INPUTMONITOR
        /// <summary>
        /// For debug purpose
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string message = String.Format("Type: [{0}]\nCount: [{1}]\nRawMouseActions: [{2}]", 
                   this.GetType().ToString(),
                   CurrentInputCount.ToString(),
                   this._rawMouseActions.ToString());

            return message;
        }
#endif

        #region Private members
        private RawMouseActions _rawMouseActions = RawMouseActions.None;
        #endregion
    }

    /// <summary>
    /// A class to override InputMonitor to track keyboard events
    /// </summary>
    internal class KeyboardPostProcessInputMonitor : InputMonitor
    {
        /// <summary>
        /// Default is that we need to track at least 1 event
        /// </summary>
        public KeyboardPostProcessInputMonitor() : this(1)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        public KeyboardPostProcessInputMonitor(int count) : this(count, RawKeyboardActions.KeyUp)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="rawKeyboardActions"></param>
        public KeyboardPostProcessInputMonitor(int count, RawKeyboardActions rawKeyboardActions) : base (count)
        {
            this._rawKeyboardActions = rawKeyboardActions;
        }

        /// <summary>
        /// Return true if the supplied InputMonitor is of type KeyboardPostProcessInputMonitor
        /// and it tracks same RawKeyboardActions
        /// </summary>
        /// <param name="inputMonitor"></param>
        /// <returns></returns>
        public override bool IsSameKind(InputMonitor inputMonitor)
        {
            KeyboardPostProcessInputMonitor keyboardPostProcessInputMonitor = inputMonitor as KeyboardPostProcessInputMonitor;

            if (keyboardPostProcessInputMonitor != null)
            {
                return (keyboardPostProcessInputMonitor.RawKeyboardActions == this._rawKeyboardActions);
            }

            return false;
        }

        /// <summary>
        /// return the tracked RawMouseActions
        /// </summary>
        /// <value></value>
        public RawKeyboardActions RawKeyboardActions
        {
            get { return this._rawKeyboardActions; }
        }


        /// <summary>
        /// This override the parent PostProcessInput handler
        /// </summary>
        /// <param name="sender">It is usually InputManager</param>
        /// <param name="args">ProcessInputEventArgs, so that we know what the input is</param>
        protected override void PostProcessInput(object sender, ProcessInputEventArgs args)
        {
            //
            // we are only interested if it is an InputReportEventArgs instance
            // 
            if (InputReportEventArgsWrapper.IsCorrectType(args.StagingItem.Input))
            {
                InputReportEventArgsWrapper inputReportEventArgs = new InputReportEventArgsWrapper(args.StagingItem.Input);

                //
                // We are interested only in keyboard events, and in the bubbling phrase
                //
                if (inputReportEventArgs.Report != null 
                    && inputReportEventArgs.Report.Type == InputType.Keyboard 
                    && inputReportEventArgs.RoutedEvent.RoutingStrategy == RoutingStrategy.Bubble)
                {
                    RawKeyboardInputReportWrapper keyboardInputReport = new RawKeyboardInputReportWrapper(inputReportEventArgs.Report);

                    if ((keyboardInputReport.Actions & this._rawKeyboardActions) == keyboardInputReport.Actions)
                    {
                        if (base.DecrementInputCount() == 0)
                        {
                            //
                            // the reference count goes down to zero,
                            // all expected keyboard input events are seen
                            // and it's time to leave
                            //
                            base.Detach();
                        }
                    }
                }
            }
        }       

#if DEBUG_INPUTMONITOR
        /// <summary>
        /// For debug purpose
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string message = String.Format("Type: [{0}]\nCount: [{1}]\nRawKeyboardActions: [{2}]", 
                   this.GetType().ToString(),
                   CurrentInputCount.ToString(),
                   this._rawKeyboardActions.ToString());

            return message;
        }
#endif

        #region Private members

        private Microsoft.Test.Input.RawKeyboardActions _rawKeyboardActions = RawKeyboardActions.KeyUp;

        #endregion
    }
}
