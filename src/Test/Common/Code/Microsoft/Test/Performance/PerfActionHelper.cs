// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Test.Performance
{
    #region PerfActionHelper

    public enum ActionBehavior
    {
        ActionIsComplete, 
        ActionWillSignalComplete
    };


    /// <summary>
    /// A TestAction is simply an Action performed on a test scenario.
    /// PerfActionHelper is who drives TestActions and logs events on
    /// behalf of these TestActions in a standard way. The hopes of this
    /// standard way of event logging is that it produces more reliable
    /// and constant measurments and allows the perf team a means
    /// to ensure this reliability.
    /// This class is meant to be overidden in order to build a TestAction
    /// that does anything. PerfActionHelper has a set of built-in
    /// shrinkwrapped actions also. Please re-use these actions wherever
    /// possible.
    /// </summary>
    /// <remarks>
    /// In order to make your own custom TestActions you can derive from this class.
    /// Overriding PreAction and Action where appropriate.
    /// </remarks>
    public abstract class TestAction
    {
        #region Private
        private string _name;   //test action name
        private PerfActionHelper _helper;
        #endregion Private


        #region Properties
        /// <summary>
        /// Name of this action. This is exposed in logs and will be used to 
        /// identify the behavior that this test action induces from the command line.
        /// </summary>
        public string Name { get { return _name; } }

        /// <summary>
        /// Gets the PerfActionHelper that is driving this test action.
        /// </summary>
        public PerfActionHelper ActionHelper { get { return _helper; } }

        #endregion Properties

        #region Constructors
        /// <summary>
        /// Constructs a new TestAction with the given ActionName
        /// </summary>
        /// <param name="ActionName"></param>
        public TestAction(string ActionName)
        {
            if (ActionName == null)
            {
                throw new ArgumentNullException("The name of a TestAction cannot be null");
            }

            if (ActionName.Length == 0)
            {
                throw new ArgumentException("Action name must be specified.", "ActionName");
            }

            _name = ActionName;
        }

        #endregion Constructors


        #region Public Methods

        /// <summary>
        /// Called by PerfAction helper to initialize the TestAction
        /// </summary>
        /// <param name="Helper">The PerfActionHelper that will be interacting with this TestAction</param>
        /// <remarks>Be sure to call base.Initialize() if you override this member.</remarks>
        public virtual void Initialize(PerfActionHelper Helper)
        {
            _helper = Helper;   
        }


        /// <summary>
        /// Called by PerfActionHelper, the purpose of the PreAction function is
        /// to prepare the scenario for measurment. Any actions conducted during
        /// the PreAction function will not be measured.
        /// </summary>
        /// <returns>ActionIsComplete means PreAction is finished. 
        /// ActionWillSignalComplete means we are operating asyncronously and will call
        /// one of the EndPreAction... function on PerfActionHelper to signal complete.</returns>
        /// <remarks>Override this function in order to perform custom PreAction work.</remarks>
        public virtual ActionBehavior PreAction()
        {
            return ActionBehavior.ActionIsComplete;
        }

        /// <summary>
        /// Called by PerfActionHelper, the purpose of the Action function is
        /// to perform operations that are to be measured.
        /// </summary>
        /// <returns>ActionIsComplete means Action is finished.
        /// ActionWillSignalComplete means we are operating asyncronously and will call
        /// one of the EndAction... family of functions on PerfActionHelper to signal complete.</returns>
        public abstract ActionBehavior Action();

        #endregion Public Methods
    }


    /// <summary>
    /// PerfActionHelper drives test actions and emits ETW events along the way.
    /// This is an attempt to instrument test scenarios in a standardized way
    /// that is simple, fast and easier to develop with.
    /// 
    /// Basically PerfActionHelper must be instantiated by the test scenario.
    /// Once Initialize is called, PerfActionHelper will analyze all command line 
    /// arguments that were specified at process creation time. It will then search
    /// for arguments that match the TestAction command line name. From there it will
    /// initialize the proper TestAction, Call PreAction, log a Begin Execution
    /// tag, Call the Action delegate, and then log an End Execution event and
    /// shutdown the app.
    /// </summary>
    /// <remarks>
    /// A /hold argument to the command line will stop PerfActionHelper from shutting down the app
    /// after its Action is completed.
    /// 
    /// Command line format should follow:
    /// /ActionName:ActionArg1;ActionArg2;ActionArg3
    /// </remarks>
    public class PerfActionHelper
    {
        public enum OperationalState 
        {
            Initializing, //PerfActionHelper is currently being constructed or initializing it's active action
            PreAction, //PerfActionHelper is in the process of calling PreAction on the current TestAction
            Action,  //PerfActionHelper is in the process of calling Action on the current TestAction
            Complete, //PerfActionHelper's Active TestAction has completed and measuring is complete.
            Holding,  //PerfActionHelper is completed, and is holding
            ShuttingDown  //PerfActionHelper will be shutting down the app soon.
        }

        #region Private
        private const int DEFAULT_RENDERING_TIMEOUT = 200;      // in milliseconds

        private delegate void _voidCallDelegate();
        

        private OperationalState _current_state; //tracks current state of the perf action helper
        private OperationalState _next_state; //tracks next state of the perf action helper

        private Hashtable _testActions = null; //contains all known types of test actions
        private EventHandler _PostTimerCallHandler = null; //caching this event handler to cut down on calls to new
        private _voidCallDelegate _TransitionDelegate = null; //caching this delegate to cut down on calls to new

        private Dispatcher _originalDispatcher = null; //original dispatcher the perfactionheler was created on
        private TestAction _action = null; //the current test action or null if there is none
        private int _renderingTimeout = DEFAULT_RENDERING_TIMEOUT;
        private bool _isMouseMovedOut = false;
        private bool _beep = false;
        private bool _hold = false;
        private string[] _action_arguments; //arguments passed to the curent action. ActionArg1, ActionArg2, ActionArg...
        #endregion Private

        #region Constructors
        /// <summary>
        /// Constructs a PerfActionHelper object and adds any 
        /// CustomActions defined by the test owner.
        /// This constructor will result in Initiailze being called on
        /// itself as well.
        /// </summary>
        /// <param name="CustomActions">A list of custom TestAction objects to enable for this scenario.</param>
        public PerfActionHelper(params TestAction[] CustomActions)
        {
            //make sure we have permissions to operate
            System.Security.Permissions.EnvironmentPermission envPermission = new System.Security.Permissions.EnvironmentPermission(System.Security.Permissions.PermissionState.Unrestricted);
            envPermission.Assert();


            //initialize local variables
            this._testActions = new Hashtable();
            _PostTimerCallHandler = new EventHandler(_PostTimerCall);
            _TransitionDelegate = new _voidCallDelegate(_Transition);
            _originalDispatcher = Dispatcher.CurrentDispatcher;
            _current_state = OperationalState.Initializing;
            
            

          
            
            //add built in test action types
            _AddTestAction(new StartupTestAction());
            _AddTestAction(new NavigationTestAction());
            _AddTestAction(new ResizeTestAction());
            _AddTestAction(new ScrollTestAction());
            _AddTestAction(new InputTestAction());
            _AddTestAction(new PageFlipTestAction());
            _AddTestAction(new GcTestAction());

            //add custom actions
            foreach(TestAction custom_action in CustomActions)
            {
                _AddTestAction(custom_action);
            }

            
            string command_type = null;
            
            if (_ParseActionParameters(System.Environment.GetCommandLineArgs(), out command_type, out _action_arguments))
            {
                if(_testActions.Contains(command_type.ToLower()))
                {
                    _action = (TestAction)_testActions[command_type.ToLower()];
                    _TransitionImmediate();
                }
            }
        }

        #endregion Constructors

        #region Properties
        /// <summary>
        /// RenderingTimeout is the amount of time that should pass
        /// after Idle status is acheived to ensure that all rendering 
        /// completes on the rendering thread.
        ///     This is because the Avalon Managed thread will post Idle
        /// before the Rendering Thread finished rendering.
        /// </summary>
        public int RenderingTimeout
        {
            get
            {
                return _renderingTimeout;
            }

            set
            {
                _renderingTimeout = value;
            }
        }

        /// <summary>
        /// Arguments that are passed to the Active Action
        /// </summary>
        public string[] ActionArguments
        {
            get
            {
                return _action_arguments;
            }
        }

        /// <summary>
        /// Get or set Beep, which will cause a Beep to sound at the beginning
        /// and end of the TestActions main action delegate.
        /// </summary>
        public bool Beep
        {
            get
            {
                return _beep;
            }


            set
            {
                _beep = value;
            }
        }

        /// <summary>
        /// Returns the Dispatcher object that the PerfActionHelper was created on.
        /// </summary>
        public Dispatcher OriginalDispatcher 
        { 
            get 
            { 
                return _originalDispatcher; 
            } 
        }

        /// <summary>
        /// Returns the current state that this PerfActionHelper is in.
        /// </summary>
        public OperationalState CurrentState
        {
            get
            {
                return _current_state;
            }
        }


        /// <summary>
        /// Returns the Active Test Action or null if no TestAction will be used.
        /// </summary>
        public TestAction ActiveTestAction
        {
            get
            {
                return _action;
            }
        }


        #endregion Properties


        #region Public Methods
        /// <summary>
        /// Signals the end of the PreAction state after rendering completes and Idle is finished.
        /// Any PreAction that returns ActionWillSignalComplete must call this when asyncronous
        /// PreAction operations are complete.
        /// </summary>
        public void EndPreActionOnRenderIdle()
        {
            if (_current_state != OperationalState.PreAction)
            {
                throw new Exception("Calling EndPreAction is only valid during the PreAction phase.");
            }

            _TransitionOnRenderIdle();
        }


        /// <summary>
        /// Signals the end of the Action state after rendering completes and Idle is finished.
        /// Any Action that returns ActionWillSignalComplete must call this when asyncronous
        /// Action operations are complete.
        /// </summary>
        public void EndActionOnRenderIdle()
        {
            if (_current_state != OperationalState.Action)
            {
                throw new Exception("Calling EndAction is only valid during the Action phase.");
            }

            _TransitionOnRenderIdle();
        }

        /// <summary>
        /// Helper function to block till all queue items are processed, also introduce time
        /// lag between subsequent test runs
        /// </summary>
        public static void WaitTillQueueItemsProcessed()
        {
            // To keep this thread busy, we'll have to push a frame.
            DispatcherFrame frame = new DispatcherFrame();

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new DispatcherOperationCallback(
                delegate(object arg)
                {
                    frame.Continue = false;
                    return null;
                }), null);

            // Keep the thread busy processing events until the timeout has expired.
            Dispatcher.PushFrame(frame);
        }
    
        #endregion Public Methods


        #region Private Methods

        //Calls transition with a very high priority. This should cause Transition to be called
        //immediately 
        private void _TransitionImmediate()
        {
            OriginalDispatcher.BeginInvoke(
                    DispatcherPriority.Send,
                    _TransitionDelegate
                    );
        }

        //Calls transition after render idle (system idle, with rendering complete)
        private void _TransitionOnRenderIdle()
        {
            _DispatchAfterDelay(
                OriginalDispatcher,
                DispatcherPriority.SystemIdle,
                System.TimeSpan.FromMilliseconds(RenderingTimeout),
                _TransitionDelegate,
                null);
        }

        //the heart of perf action helper. 
        //the idea is that everytime we call in here we are performing some
        //state transition and creating all the side-effects that goes with that.
        private void _Transition()
        {    
            if (_action == null)
            {
                 throw new Exception("Internal Error: PerfActionHelper cannot operate with a null active TestAction. This should be an unreachable state. Contact avtperf");
            }

            _current_state = _next_state;

            switch (_current_state)
            {
                case OperationalState.Initializing:
                    _action.Initialize(this);
                    _next_state = OperationalState.PreAction;
               
                    _TransitionImmediate();
                    return;


                case OperationalState.PreAction:

                    if (!_isMouseMovedOut)
                    {
                        _next_state = OperationalState.PreAction; 

                        //InputPerf.SendMouseInput(1280, 1024, 0, SendMouseInputFlags.Move);
                        _isMouseMovedOut = true;

                        _TransitionOnRenderIdle();
                    }
                    else
                    {
                        if (EventTrace.IsEnabled(EventTrace.Flags.Performance, EventTrace.Level.Normal))
                        {
                            EventTrace.EventProvider.TraceEvent(EventTrace.QUEUEIDLE_GUID, EventType.Info, _action.Name);
                        }

                        _next_state = OperationalState.Action;

                        if (_action.PreAction() == ActionBehavior.ActionIsComplete)
                        {
                            _TransitionOnRenderIdle();
                        }

                        // If we get back ActionWillSignalComplete then they must call
                        // EndPreActionOnRenderIdle which will call transition for us
                    }

                    return;

                case OperationalState.Action:

                    if (EventTrace.IsEnabled(EventTrace.Flags.Performance, EventTrace.Level.Normal))
                    {
                        EventTrace.EventProvider.TraceEvent(EventTrace.OPERATION_GUID, EventType.StartEvent, _action.Name);
                    }

                    _next_state = OperationalState.Complete;

                    if (_beep)
                    {
                        System.Console.Beep(2000, 40);
                    }

                    if (_action.Action() == ActionBehavior.ActionIsComplete)
                    {
                        _TransitionOnRenderIdle();
                    }

                    // If we get back ActionWillSignalComplete then they must call
                    // EndActionOnRenderIdle which will call transition for us

                    return;

                case OperationalState.Complete:

                    if (EventTrace.IsEnabled(EventTrace.Flags.Performance, EventTrace.Level.Normal))
                    {
                        EventTrace.EventProvider.TraceEvent(EventTrace.OPERATION_GUID, EventType.EndEvent, _action.Name);
                    }

                    if (_beep)
                    {
                        Console.Beep(4000, 40);
                    }

                    if (_hold)
                    {
                        _next_state = OperationalState.Holding;
                    }
                    else
                    {
                        _next_state = OperationalState.ShuttingDown;
                    }


                    _TransitionImmediate();
                    return;

                case OperationalState.Holding:
                    return; //do nothing we are holding

                case OperationalState.ShuttingDown:
                    //shut us down after 2 * render idle
                    _DispatchAfterDelay(
                            OriginalDispatcher,
                            DispatcherPriority.SystemIdle,
                            System.TimeSpan.FromMilliseconds(2 * RenderingTimeout),
                            new _voidCallDelegate(OriginalDispatcher.InvokeShutdown),
                            null
                    );
                    return;
                    
                default:
                    throw new Exception("Invalid Operational State Entered.");
            }
        }

        /// <summary>
        /// Schedules a delegate call after the specified Delay once the Dispatcher processes the request.
        /// </summary>
        /// <param name="TargetDispatcher">The dispatcher to schedule the call on.</param>
        /// <param name="Priority">The Priority that the call will be queued at.</param>
        /// <param name="CallDelay">The amount of time that will pass before the call is made once the queued timer is started.</param>
        /// <param name="CallToMake">Delegate to call.</param>
        /// <param name="CallArgs">Arguments to pass to the Delegate.</param>
        private void _DispatchAfterDelay(Dispatcher TargetDispatcher, DispatcherPriority Priority, TimeSpan CallDelay, Delegate CallToMake, object[] CallArgs)
        {
            DispatcherTimer timer = new DispatcherTimer(
                       CallDelay,
                       Priority,
                       _PostTimerCallHandler,
                       TargetDispatcher);

            TimerCallTag tag = new TimerCallTag(CallToMake, CallArgs);

            timer.Tag = tag;
        }


        //allows us to associated a call delegate and its arguments with a timer
        private class TimerCallTag
        {
            public Delegate Call;
            public object[] CallArgs;

            public TimerCallTag(Delegate call, object[] callArgs)
            {
                Call = call;
                CallArgs = callArgs;
            }
        }
     
        //called by the timer that _DispatchAfterDelay creates. this will unwrap the timer
        //disable it and call the delegate for us.
        private void _PostTimerCall(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.IsEnabled = false;

            TimerCallTag callwrapper = (TimerCallTag)timer.Tag;

            callwrapper.Call.DynamicInvoke(callwrapper.CallArgs);
        }

        
        // Adds a new TestAction object to our hash table
        // protects us from collisions
        private void _AddTestAction(TestAction Action)
        {
            string action_name = Action.Name.ToLower();

            if(_testActions.Contains(action_name))
            {
                TestAction collision = (TestAction)_testActions[action_name];

                throw new ArgumentException(
                    string.Format(
                    "There is more than one TestAction with the same name. Conflicting Name:\"{0}\" Input Type:\"{1}\" Existing Type: \"{2}", 
                    action_name, 
                    Action.GetType().Name,
                    collision.GetType().Name
                                )
                                        );
            }

            _testActions.Add(Action.Name.ToLower(), Action);
        }


        //parses the command line arguments and finds a viable action type
        private bool _ParseActionParameters(string[] CommandLineArgs, out string ActionType, out string[] ActionArgs)
        {
            char[] command_delimiters = new char[] { '-', '/' };

            //check if we are holding
            for (int index = 0; index < CommandLineArgs.Length; index++)
            {
                if (CommandLineArgs[index].IndexOfAny(command_delimiters, 0, 1) >= 0)
                {
                    string possible_cmd_str = CommandLineArgs[index].TrimStart(command_delimiters);

                    if (string.Compare(possible_cmd_str, "hold", true) == 0)
                    {
                        _hold = true;
                        continue;
                    }

                    if (string.Compare(possible_cmd_str, "beep", true) == 0)
                    {
                        _beep = true;
                        continue;
                    }
                }
            }

            //now find our command type, first one we can find wins
            for(int index = 0; index < CommandLineArgs.Length; index++)
            {
                if(CommandLineArgs[index].IndexOfAny(command_delimiters, 0, 1) >= 0)
                {
                    string perf_argument_str = CommandLineArgs[index].TrimStart(command_delimiters);


                    string[] perf_args = perf_argument_str.Split(':');

                    if (perf_args.Length >= 1)
                    {
                        string possible_action_type = perf_args[0].ToLower();

                        if (_testActions.Contains(possible_action_type))
                        {
                            ActionType = perf_args[0];

                            if (perf_args.Length == 2)
                            {
                                ActionArgs = perf_args[1].Split(';');
                            }
                            else
                            {
                                ActionArgs = new string[0];
                            }

                            return true;
                        }
                    }
                    else
                    {
                        continue; //perf argument wasn't proper. skip this and look for another one
                    }

                }
            }

            ActionType = null;
            ActionArgs = null;
            return false;
        }

        #endregion Private Methods
    }

    #endregion PerfActionHelper






    #region Built-In Test Actions


    /// <summary>
    /// Startup Test action simply does nothing. This results in perf action helper
    /// logging Begin and End Execution events and closing down the app once it goes idle.
    /// </summary>
    public class StartupTestAction : TestAction
    {
        public StartupTestAction()
            : base("Startup")
        { }

        public override ActionBehavior Action()
        {
            return ActionBehavior.ActionIsComplete;
        }
    }



    // ----------------------------------------------------------------------------------------------
    // Navigation
    // ----------------------------------------------------------------------------------------------
    public class NavigationTestAction : TestAction
    {
        private int _current_index;

        public NavigationTestAction()
            : base("Navigation")
        {
            _current_index = 0;
        }

        public override ActionBehavior Action()
        {
            if ((ActionHelper.ActionArguments == null) || (ActionHelper.ActionArguments.Length == 0))
            {
                throw new ArgumentException("Navigation requires semicolon-delimited list of target URIs. (Minumum 1 URI)");
            }

            ActionHelper.OriginalDispatcher.BeginInvoke(
                    DispatcherPriority.SystemIdle,
                    new DispatcherOperationCallback(NavigateWorker),
                    null);

            return ActionBehavior.ActionWillSignalComplete;
        }

    

        private object NavigateWorker(object arg)
        {
            System.Windows.Navigation.NavigationWindow navigationWindow = (System.Windows.Navigation.NavigationWindow)System.Windows.Application.Current.Windows[0];


            if (_current_index >= ActionHelper.ActionArguments.Length)
            {
                // We're done navigating
                ActionHelper.EndActionOnRenderIdle();
                return null;
            }

            if (ActionHelper.ActionArguments[_current_index] == "_b_")
            {
                navigationWindow.GoBack();
            }
            else if (ActionHelper.ActionArguments[_current_index] == "_f_")
            {
                navigationWindow.GoForward();
            }
            else
            {
                navigationWindow.Navigate(new Uri(ActionHelper.ActionArguments[_current_index], UriKind.RelativeOrAbsolute));
            }

            _current_index++;

            ActionHelper.OriginalDispatcher.BeginInvoke(
                    DispatcherPriority.SystemIdle,
                    new DispatcherOperationCallback(NavigateWorker),
                    null);

            return null;
        }
    }


    // ----------------------------------------------------------------------------------------------
    // GC
    // ----------------------------------------------------------------------------------------------
    public class GcTestAction : TestAction
    {
        public GcTestAction()
            : base("GC")
        { }

        public override ActionBehavior Action()
        {
            DispatcherTimer timer = new DispatcherTimer(
                    System.TimeSpan.FromMilliseconds(5000),
                    DispatcherPriority.SystemIdle,
                    new EventHandler(this.GcWorker),
                    ActionHelper.OriginalDispatcher);

            return ActionBehavior.ActionWillSignalComplete;
        }

        private void GcWorker(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.IsEnabled = false;

            GC.Collect(2);
            GC.WaitForPendingFinalizers();
            CLRProfilerControl.DumpHeap();
            CLRProfilerControl.LogWriteLine("APPSHUTDOWN");

            ActionHelper.EndActionOnRenderIdle();
        }

    }


    // ----------------------------------------------------------------------------------------------
    // Resize
    // ----------------------------------------------------------------------------------------------
    public class ResizeTestAction : TestAction
    {
        private int _FromX;
        private int _FromY;
        private int _ToX;
        private int _ToY;
        private int _RepeatCount = 1;

        public ResizeTestAction()
            : base("Resize")
        {}

        public override ActionBehavior PreAction()
        {
            Window window = null;

            // Parameter value should be of the format:
            // /resize:FromX;FromY;ToX;ToY(;RepeatCount)
            if (ActionHelper.ActionArguments.Length < 4)
            {
                throw new ArgumentException("Invalid resize parameter - format must be \"From X;From Y;To X; To Y(;Repeat Count)\"");
            }

            try
            {
                _FromX = Int32.Parse(ActionHelper.ActionArguments[0]);
                _FromY = Int32.Parse(ActionHelper.ActionArguments[1]);
                _ToX = Int32.Parse(ActionHelper.ActionArguments[2]);
                _ToY = Int32.Parse(ActionHelper.ActionArguments[3]);

                if (ActionHelper.ActionArguments.Length == 5)
                {
                    _RepeatCount = Int32.Parse(ActionHelper.ActionArguments[4]);
                }
            }
            catch (Exception)
            {
                throw new ArgumentException("Invalid resize parameter - format must be \"From X;From Y;To X; To Y(;Repeat Count)\"");
            }

            if ((_FromX < 100) || (_FromY < 100))
            {
                throw new ArgumentException("The starting window size is too small (must be 100 x 100 or greater)");
            }

            if ((_ToX < 100) || (_ToY < 100))
            {
                throw new ArgumentException("The ending window size is too small (must be 100 x 100 or greater)");
            }

            // Set the current window size to the starting window size
            window = System.Windows.Application.Current.MainWindow;
            window.Width = _FromX;
            window.Height = _FromY;
            window.Left = 0;
            window.Top = 0;

            InputPerf.SendMouseInput(
                    _FromX - 2,
                    _FromY - 2,
                    0,
                    SendMouseInputFlags.Move | SendMouseInputFlags.Absolute);

            InputPerf.SendMouseInput(
                    0,
                    0,
                    0,
                    SendMouseInputFlags.LeftDown | SendMouseInputFlags.Move
                    );

            return ActionBehavior.ActionIsComplete;
        }

        public override ActionBehavior Action()
        {
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ResizeWorker));
            return ActionBehavior.ActionWillSignalComplete;
        }

        private void ResizeWorker(object arg)
        {
            int i = 0;

            for (i = 0; i < _RepeatCount; i++)
            {
                InputPerf.SendMouseInput(
                    ((i % 2) == 0) ? _ToX : _FromX,
                    ((i % 2) == 0) ? _ToY : _FromY,
                    0,
                    SendMouseInputFlags.Move
                  | SendMouseInputFlags.LeftDown
                  | SendMouseInputFlags.Absolute);

                System.Threading.Thread.Sleep(0);
            }

            InputPerf.SendMouseInput(0, 0, 0,
                    SendMouseInputFlags.LeftUp);

            ActionHelper.EndActionOnRenderIdle();
        }
    }


    public class ScrollTestAction : TestAction
    {
        /// s_scrollBarPosX_Y : defines the postion of the scrollbar top left corner
        /// s_scrollFromX, s_scrollToX: defines horizontal scroll range
        /// s_scrollFromY, s_scrollToY: defines vertical scroll range
        /// s_scrollInc: defines mouse move increments when doing scrolling
        /// s_scrollCount: defines how many times we need to repeat
        private int _ScrollBarPositionX;
        private int _ScrollBarPositionY;
        private int _FromX;
        private int _FromY;
        private int _ToX;
        private int _ToY;
        private int _ScrollIncrement;
        private int _RepeatCount;
        private bool _ScrollTopToBottom;

        public ScrollTestAction()
            : base("Scroll")
        {
            _ScrollBarPositionX = 0;
            _ScrollBarPositionY = 0;
            _FromX = 0;
            _FromY = 0;
            _ToX = 0;
            _ToY = 0;
            _ScrollIncrement = 10;
            _RepeatCount = 1;
            _ScrollTopToBottom = true;
        }

        public override ActionBehavior PreAction()
        {
            if ((ActionHelper.ActionArguments.Length > 3) && (ActionHelper.ActionArguments[0].ToLower() == "id"))
            {
                // argument should be of the form:
                // /scroll:ID;scrollviewer;TB;1

                try
                {
                    Window window = null;
                    FrameworkElement scrollableElement = null;
                    DependencyObject rootNode = null;
                    System.Windows.Media.GeneralTransform generalTransform;
                    System.Windows.Media.Transform transform;
                    System.Windows.Media.Matrix matrix;
                    Rect childBounds;
                    int windowWidth = 0;

                    window = System.Windows.Application.Current.MainWindow;
                    rootNode = (DependencyObject)window.Content;
                    windowWidth = (int)window.Width;

                    FrameworkElement targetElement = (FrameworkElement)LogicalTreeHelper.FindLogicalNode(rootNode, ActionHelper.ActionArguments[1]);

                    scrollableElement = targetElement as ListView;

                    if (scrollableElement != null)
                    {
                        ArrayList children = GetVisualChildren(targetElement as Visual, typeof(ScrollViewer), new ArrayList());
                        for (int i = 0; i < children.Count; i++)
                        {
                            if (((ScrollViewer)children[i]).TemplatedParent == targetElement)
                            {
                                scrollableElement = children[i] as ScrollViewer;
                                break;
                            }
                        }
                    }
                    else
                    {
                        scrollableElement = targetElement;
                    }

                    if (scrollableElement == null)
                    {
                        throw new ArgumentException("Could not find a control with the specified ID (" + ActionHelper.ActionArguments[1] + ")");
                    }

                    generalTransform = scrollableElement.TransformToAncestor((System.Windows.Media.Visual)window);
                    transform = generalTransform as System.Windows.Media.Transform;
                    matrix = transform.Value;

                    childBounds = Rect.Transform(new Rect(0.0, 0.0, scrollableElement.RenderSize.Width, scrollableElement.RenderSize.Height), matrix);

                    if (scrollableElement.GetType().Name == "ScrollViewer")
                    {
                        if (ActionHelper.ActionArguments[2].ToLower() == "tb")
                        {
                            //((System.Windows.Controls.ScrollViewer) scrollableElement).ScrollToVerticalOffset(0);

                            if (scrollableElement.FlowDirection == FlowDirection.RightToLeft)
                            {
                                _ScrollBarPositionX = windowWidth - ((int)childBounds.TopRight.X - 5);
                                _FromX = windowWidth - _ScrollBarPositionX;
                                _ToX = windowWidth - _ScrollBarPositionX;
                            }
                            else
                            {
                                _ScrollBarPositionX = (int)childBounds.TopRight.X - 5;
                                _FromX = _ScrollBarPositionX;
                                _ToX = _ScrollBarPositionX;
                            }

                            _ScrollBarPositionY = (int)childBounds.TopRight.Y + 50;
                            _FromY = _ScrollBarPositionY;
                            _ToY = (int)childBounds.BottomRight.Y - 8;
                        }
                        else if (ActionHelper.ActionArguments[2].ToLower() == "bt")
                        {
                            _ScrollTopToBottom = false;

                            //((System.Windows.Controls.ScrollViewer) scrollableElement).ScrollToVerticalOffset(((System.Windows.Controls.ScrollViewer) scrollableElement).ExtentHeight);

                            if (scrollableElement.FlowDirection == FlowDirection.RightToLeft)
                            {
                                _ScrollBarPositionX = windowWidth - ((int)childBounds.BottomRight.X - 5);
                                _FromX = windowWidth - _ScrollBarPositionX;
                                _ToX = windowWidth - _ScrollBarPositionX;
                            }
                            else
                            {
                                _ScrollBarPositionX = (int)childBounds.BottomRight.X - 5;
                                _FromX = _ScrollBarPositionX;
                                _ToX = _ScrollBarPositionX;
                            }
                            _ScrollBarPositionY = (int)childBounds.BottomRight.Y - 8;
                            _FromY = (int)childBounds.TopRight.Y + 50;
                            _ToY = _ScrollBarPositionY;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid scroll parameter - format must be ID;Scrollable Element ID;TB|BT;Repeat Count");
                        }
                    }
                    else if (scrollableElement.GetType().Name == "FlowDocumentPageViewer")
                    {
                        if (ActionHelper.ActionArguments[2].ToLower() == "tb")
                        {
                            System.Windows.Input.NavigationCommands.GoToPage.Execute(1, ((System.Windows.Controls.FlowDocumentPageViewer)scrollableElement));

                            if (scrollableElement.FlowDirection == FlowDirection.RightToLeft)
                            {
                                _ScrollBarPositionX = windowWidth - ((int)childBounds.TopRight.X - 5);
                                _FromX = windowWidth - _ScrollBarPositionX;
                                _ToX = windowWidth - _ScrollBarPositionX;
                            }
                            else
                            {
                                _ScrollBarPositionX = (int)childBounds.TopRight.X - 5;
                                _FromX = _ScrollBarPositionX;
                                _ToX = _ScrollBarPositionX;
                            }

                            _ScrollBarPositionY = (int)childBounds.TopRight.Y + 30;
                            _FromY = _ScrollBarPositionY + 30;
                            _ToY = (int)childBounds.BottomRight.Y + 30;
                        }
                        else if (ActionHelper.ActionArguments[2].ToLower() == "bt")
                        {
                            _ScrollTopToBottom = false;
                            System.Windows.Input.NavigationCommands.GoToPage.Execute(((System.Windows.Controls.FlowDocumentPageViewer)scrollableElement).PageCount, ((System.Windows.Controls.FlowDocumentPageViewer)scrollableElement));

                            if (scrollableElement.FlowDirection == FlowDirection.RightToLeft)
                            {
                                _ScrollBarPositionX = windowWidth - ((int)childBounds.BottomRight.X - 5);
                                _FromX = windowWidth - _ScrollBarPositionX;
                                _ToX = windowWidth - _ScrollBarPositionX;
                            }
                            else
                            {
                                _ScrollBarPositionX = (int)childBounds.BottomRight.X + 5;
                                _FromX = _ScrollBarPositionX;
                                _ToX = _ScrollBarPositionX;
                            }
                            _ScrollBarPositionY = (int)childBounds.BottomRight.Y + 30;
                            _FromY = (int)childBounds.TopRight.Y + 30;
                            _ToY = _ScrollBarPositionY + 30;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid scroll parameter - format must be ID;Scrollable Element ID;TB|BT;Repeat Count");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("WRONG ARGUMENT PASSED -:Control cannot be scrolled ", "scroll");
                    }


                    if (ActionHelper.ActionArguments.Length > 3)
                    {
                        _RepeatCount = Int32.Parse(ActionHelper.ActionArguments[3]);
                    }

                    if (ActionHelper.ActionArguments.Length > 4)
                    {
                        _ScrollIncrement = Int32.Parse(ActionHelper.ActionArguments[4]);
                    }
                    else
                    {
                        _ScrollIncrement = (int)(_ToY - _FromY);
                    }

                    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ScrollPreWorker));
                    return ActionBehavior.ActionWillSignalComplete;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            return ActionBehavior.ActionIsComplete;
            /*
                        NO MORE SCROLLING BY COORDINATES ALONE!

                        else
                        {
                            s_initDone = true;
                            if (ActionHelper.ActionArguments.Length > 0)
                            {
                                s_scrollBarPosX = Int32.Parse(ActionHelper.ActionArguments[0]);
                            }

                            if (ActionHelper.ActionArguments.Length > 1)
                            {
                                s_scrollBarPosY = Int32.Parse(ActionHelper.ActionArguments[1]);
                            }

                            if (ActionHelper.ActionArguments.Length > 2)
                            {
                                s_scrollFromX = Int32.Parse(ActionHelper.ActionArguments[2]);
                            }

                            if (ActionHelper.ActionArguments.Length > 3)
                            {
                                s_scrollToX = Int32.Parse(ActionHelper.ActionArguments[3]);
                            }

                            if (ActionHelper.ActionArguments.Length > 4)
                            {
                                s_scrollFromY = Int32.Parse(ActionHelper.ActionArguments[4]);
                            }

                            if (ActionHelper.ActionArguments.Length > 5)
                            {
                                s_scrollToY = Int32.Parse(ActionHelper.ActionArguments[5]);
                            }

                            if (ActionHelper.ActionArguments.Length > 6)
                            {
                                s_scrollInc = Int32.Parse(ActionHelper.ActionArguments[6]);
                            }

                            if (ActionHelper.ActionArguments.Length > 7)
                            {
                                s_scrollCount = Int32.Parse(ActionHelper.ActionArguments[7]);
                            }

                            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(WorkerScroll));
                        }
            */
        }

        private ArrayList GetVisualChildren(DependencyObject visual, Type type, ArrayList children)
        {
            if (visual != null)
            {
                if (visual.GetType() == type)
                {
                    children.Add(visual);
                }

                int count = VisualTreeHelper.GetChildrenCount(visual);
                for (int i = 0; i < count; i++)
                {
                    DependencyObject vis = VisualTreeHelper.GetChild(visual, i);
                    children = GetVisualChildren(vis, type, children);
                }
            }

            return children;
        }

        public override ActionBehavior Action()
        {
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ScrollWorker));
            return ActionBehavior.ActionWillSignalComplete;
        }

        private void ScrollPreWorker(object arg)
        {
            InputPerf.SendMouseInput(_FromX, _FromY, 0, SendMouseInputFlags.Move | SendMouseInputFlags.Absolute);

            if ((_FromX != _ScrollBarPositionX) || (_FromY != _ScrollBarPositionY))
            {
                InputPerf.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftDown | SendMouseInputFlags.Move);
                InputPerf.SendMouseInput(_ScrollBarPositionX, _ScrollBarPositionY, 0, SendMouseInputFlags.Move | SendMouseInputFlags.Absolute);
                InputPerf.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftUp | SendMouseInputFlags.Move);
            }

            ActionHelper.EndPreActionOnRenderIdle();
        }


        internal void ScrollWorker(object state)
        {
            int ix;
            int iy;
            bool bScrolled = false;
            int i = 0;

            if (_ScrollTopToBottom == true)
            {
                iy = _FromY;
            }
            else
            {
                iy = _ToY;
            }

            //MessageBox.Show(
            //                "ScrollBarPositionX = " + _ScrollBarPositionX + "\n" +
            //                "ScrollBarPositionY = " + _ScrollBarPositionY + "\n" +
            //                "FromX = " + _FromX + "\n" +
            //                "FromY = " + _FromY + "\n" +
            //                "ToX = " + _ToX + "\n" +
            //                "ToY = " + _ToY + "\n" +
            //                "ScrollIncrement = " + _ScrollIncrement + "\n" +
            //                "RepeatCount = " + _RepeatCount + "\n" +
            //                "ScrollTopToBottom = " + _ScrollTopToBottom
            //                );

            InputPerf.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftDown | SendMouseInputFlags.Move);

            for (i = 0; i < _RepeatCount; i++)
            {
                System.Threading.Thread.Sleep(0);

                bScrolled = false;

                for (ix = _FromX; ix < _ToX; ix = ix + _ScrollIncrement)
                {
                    InputPerf.SendMouseInput(ix, _ScrollBarPositionY, 0, SendMouseInputFlags.Move | SendMouseInputFlags.LeftDown | SendMouseInputFlags.Absolute);
                    bScrolled = true;
                }

                if (true == bScrolled)
                    continue;

                for (ix = _ToX; ix > _FromX; ix = ix - _ScrollIncrement)
                {
                    InputPerf.SendMouseInput(ix, _ScrollBarPositionY, 0, SendMouseInputFlags.Move | SendMouseInputFlags.LeftDown | SendMouseInputFlags.Absolute);
                    bScrolled = true;
                }

                if (true == bScrolled)
                    continue;

                for (; (iy) < _ToY; )
                {
                    iy = iy + _ScrollIncrement;
                    InputPerf.SendMouseInput(_ScrollBarPositionX, iy, 0, SendMouseInputFlags.Move | SendMouseInputFlags.LeftDown | SendMouseInputFlags.Absolute);
                    bScrolled = true;
                }

                if (true == bScrolled)
                    continue;

                for (; (iy) > _FromY; )
                {

                    iy = iy - _ScrollIncrement;
                    InputPerf.SendMouseInput(_ScrollBarPositionX, iy, 0, SendMouseInputFlags.Move | SendMouseInputFlags.LeftDown | SendMouseInputFlags.Absolute);
                    bScrolled = true;
                }

                if (true == bScrolled)
                    continue;
            }

            InputPerf.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftUp);

            ActionHelper.EndActionOnRenderIdle();
        }
    }
    
    
    // ----------------------------------------------------------------------------------------------
    // PageFlip
    // ----------------------------------------------------------------------------------------------
    public class PageFlipTestAction : TestAction
    {
        private int _StartPage;
        private int _Pages;
        private int _CurrentIndex;
        private FlowDocumentReader _FlowDoc;
        private FlowDocumentPageViewer _FlowDocPageViewer;
        //private int RepeatCount;

        public PageFlipTestAction()
            : base("PageFlip")
        { 
            _CurrentIndex = 0;
        }

        public override ActionBehavior PreAction()
        {
            // argument should be of the form:
            // /pageFlip:FlowDocumentReaderID;StartPage;NumberOfPages
            if (ActionHelper.ActionArguments.Length != 3)
            {
                throw new ArgumentException("PageFlip action expects 3 arguments.");
            }

            Window window = null;
            FrameworkElement pageFlipableElement = null;
            DependencyObject rootNode = null;
            int windowWidth = 0;

            window = System.Windows.Application.Current.MainWindow;
            rootNode = (DependencyObject)window.Content;
            windowWidth = (int)window.Width;

            pageFlipableElement = (FrameworkElement)LogicalTreeHelper.FindLogicalNode(rootNode, ActionHelper.ActionArguments[0]);

            if (pageFlipableElement == null)
            {
                throw new ArgumentException("Could not find a control with the specified ID (" + ActionHelper.ActionArguments[0] + ")");
            }

            if (pageFlipableElement.GetType().Name == "FlowDocumentReader")
            {
                _FlowDoc = pageFlipableElement as System.Windows.Controls.FlowDocumentReader;
                _StartPage = Int32.Parse(ActionHelper.ActionArguments[1]);
                _Pages = Int32.Parse(ActionHelper.ActionArguments[2]);
                _FlowDocPageViewer = ((Decorator)FindElement(pageFlipableElement, "PART_ContentHost")).Child as FlowDocumentPageViewer;
            }
            else
            {
                throw new ArgumentException("WRONG ARGUMENT PASSED -:Control cannot be pageFliped ", "pageFlip");
            }

            _FlowDocPageViewer.GoToPage(_StartPage);

            return ActionBehavior.ActionIsComplete;
        }

        public override ActionBehavior Action()
        {
            ActionHelper.OriginalDispatcher.BeginInvoke(
                    DispatcherPriority.SystemIdle,
                    new DispatcherOperationCallback(PageFlipWorker),
                    null);

            return ActionBehavior.ActionWillSignalComplete;
        }


        private object PageFlipWorker(object arg)
        {
            if (_CurrentIndex >= _Pages)
            {
                ActionHelper.EndActionOnRenderIdle();
                return null;
            }

            _FlowDocPageViewer.NextPage();
            _CurrentIndex++;

            ActionHelper.OriginalDispatcher.BeginInvoke(
                    DispatcherPriority.SystemIdle,
                    new DispatcherOperationCallback(PageFlipWorker),
                    null);

            return null;
        }

        

        FrameworkElement FindElement(DependencyObject root, string name)
        {
            FrameworkElement target = null;
            FrameworkElement fe;

            int count = VisualTreeHelper.GetChildrenCount(root);

            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(root, i);
                fe = child as FrameworkElement;

                if (fe != null)
                {
                    if (fe.TemplatedParent != null)
                    {
                        if (fe.Name == name)
                        {
                            target = fe;
                        }
                        else
                        {
                            target = FindElement(child, name);
                        }
                    }
                }
                else
                {
                    target = FindElement(child, name);
                }

                if (target != null) { break; }
            }
            return target;
        }
    }

    // ----------------------------------------------------------------------------------------------
    // Input
    // ----------------------------------------------------------------------------------------------
    public class InputTestAction : TestAction
    {       
        private string _InputString;
        private int _ClickX;
        private int _ClickY;

        public InputTestAction()
            : base("Input")
        {
            _InputString = "look i type really fast the quick brown fox jump over lazy dog";
            _ClickX = 0;
            _ClickY = 0;
        }

        public override ActionBehavior PreAction()
        {
            // The first field is a pre action
            // The second field is the string to type

            if ((ActionHelper.ActionArguments.Length > 1) && (ActionHelper.ActionArguments[0].ToLower() == "id"))
            {
                DependencyObject rootNode = (DependencyObject)System.Windows.Application.Current.MainWindow.Content;
                DependencyObject inputElement = LogicalTreeHelper.FindLogicalNode(rootNode, ActionHelper.ActionArguments[1]);

                if (inputElement is TextBox)
                {
                    TextBox textBox = (TextBox)inputElement;

                    if (textBox == null)
                    {
                        throw new ArgumentException("WRONG ARGUMENT PASSED -: Control for the given ID not found ", "input");
                    }

                    textBox.Focus();
                }
                else
                {
                    PasswordBox passwordBox = (PasswordBox)inputElement;

                    if (passwordBox == null)
                    {
                        throw new ArgumentException("WRONG ARGUMENT PASSED -: Control for the given ID not found ", "input");
                    }

                    passwordBox.Focus();
                }

                if (ActionHelper.ActionArguments.Length > 2)
                {
                    _InputString = ActionHelper.ActionArguments[2];
                }

            }
            else if (ActionHelper.ActionArguments.Length > 0)
            {
                //
                // determine what action to take
                //
                if (ActionHelper.ActionArguments.Length > 1)
                {
                    _InputString = ActionHelper.ActionArguments[1];
                }

                if (ActionHelper.ActionArguments[0].Length > 0)
                {
                    if (ActionHelper.ActionArguments[0].ToLower().StartsWith("click_"))
                    {
                        // Finds the coordinates and click on it
                        string coordString = ActionHelper.ActionArguments[0].Remove(0, 6);
                        string[] xyStrings = coordString.Split('_');

                        //click is relative to the main window coordinates
                        //so we must compute based on the main window top and left
                        _ClickX = (int)System.Windows.Application.Current.MainWindow.Left + Convert.ToInt32(xyStrings[0]);
                        _ClickY = (int)System.Windows.Application.Current.MainWindow.Top + Convert.ToInt32(xyStrings[1]);

                        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(InputPreWorker));
                        return ActionBehavior.ActionWillSignalComplete;

                    }
                }
            }

            return ActionBehavior.ActionIsComplete;
        }

        private void InputPreWorker(object arg)
        {
            InputPerf.SendMouseInput(
                        _ClickX,
                        _ClickY,
                        0,
                        SendMouseInputFlags.Move | SendMouseInputFlags.Absolute
                        );

            InputPerf.SendMouseInput(
                        0,
                        0,
                        0,
                        SendMouseInputFlags.LeftDown
                        );

            InputPerf.SendMouseInput(
                        0,
                        0,
                        0,
                        SendMouseInputFlags.LeftUp
                        );

            ActionHelper.EndPreActionOnRenderIdle();
        }

        public override ActionBehavior Action()
        {
            System.Windows.Input.Key keyCode;
            char nextChar;
            int charIndex = 0;
            int i = 0;

            for (i = 0; i < _InputString.Length; i++)
            {
                nextChar = _InputString[i];
                charIndex = 0;

                if (char.IsUpper(nextChar))
                {
                    nextChar = char.ToLower(nextChar);

                    charIndex = Convert.ToInt32(nextChar) - INPUTKEY_TABLE_BASEINDEX;
                    keyCode = ((charIndex < _inputKeyTable.Length) && (charIndex >= 0)) ? _inputKeyTable[charIndex] : System.Windows.Input.Key.Space;

                    InputPerf.SendKeyboardInput(System.Windows.Input.Key.RightShift, true);
                    InputPerf.SendKeyboardInput(keyCode, true);
                    InputPerf.SendKeyboardInput(keyCode, false);
                    InputPerf.SendKeyboardInput(System.Windows.Input.Key.RightShift, false);
                }
                else
                {
                    charIndex = Convert.ToInt32(nextChar) - INPUTKEY_TABLE_BASEINDEX;
                    keyCode = ((charIndex < _inputKeyTable.Length) && (charIndex >= 0)) ? _inputKeyTable[charIndex] : System.Windows.Input.Key.Space;

                    InputPerf.SendKeyboardInput(keyCode, true);
                    InputPerf.SendKeyboardInput(keyCode, false);
                }
            }

            ActionHelper.EndActionOnRenderIdle();

            return ActionBehavior.ActionWillSignalComplete;
        }

        private System.Windows.Input.Key[] _inputKeyTable = new System.Windows.Input.Key[]
        {
            System.Windows.Input.Key.A,
            System.Windows.Input.Key.B,
            System.Windows.Input.Key.C,
            System.Windows.Input.Key.D,
            System.Windows.Input.Key.E,
            System.Windows.Input.Key.F,
            System.Windows.Input.Key.G,
            System.Windows.Input.Key.H,
            System.Windows.Input.Key.I,
            System.Windows.Input.Key.J,
            System.Windows.Input.Key.K,
            System.Windows.Input.Key.L,
            System.Windows.Input.Key.M,
            System.Windows.Input.Key.N,
            System.Windows.Input.Key.O,
            System.Windows.Input.Key.P,
            System.Windows.Input.Key.Q,
            System.Windows.Input.Key.R,
            System.Windows.Input.Key.S,
            System.Windows.Input.Key.T,
            System.Windows.Input.Key.U,
            System.Windows.Input.Key.V,
            System.Windows.Input.Key.W,
            System.Windows.Input.Key.X,
            System.Windows.Input.Key.Y,
            System.Windows.Input.Key.Z
        };

        private const int INPUTKEY_TABLE_BASEINDEX = 97; // value of 'a'
    }

    #endregion Built-In Test Actions
}
