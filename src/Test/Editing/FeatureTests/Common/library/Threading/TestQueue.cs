// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides a queue to provide a stronger queuing model with input.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 8 $ $Source: //depot/vbl_wcp_avalon_dev/windowstest/client/wcptests/uis/Common/Library/Utils/Queue.cs $")]

namespace Test.Uis.Threading
{
    #region Namespaces.

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;

    using Test.Uis.Utils;

    #endregion Namespaces.

    /// <summary>
    /// This class provides a queue abstraction with stronger guarantees
    /// than the regular Avalon queue.
    /// </summary>
    class TestQueue
    {
        #region Constructors.

        /// <summary>Initializes a new Test.Uis.Threading.TestQueue instance.</summary>
        private TestQueue() : this(Dispatcher.CurrentDispatcher)
        {
        }

        /// <summary>
        /// Initializes a new Test.Uis.Threading.TestQueue instance
        /// for a specific context.
        /// </summary>
        /// <param name="dispatcher">Dispatcher to which items will be posted.</param>
        private TestQueue(Dispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            this._dispatcher = dispatcher;
            this._delayedList = new List<QueueItem>();

            // Note that DispatcherPriortiy.Send is considered the highest priority.
            this._queues = new ItemQueue[1 + (int)DispatcherPriority.Send];
            for (int i = 0; i < this._queues.Length; i++)
            {
                this._queues[i] = new ItemQueue();
            }

            MapQueueToDispatcher(dispatcher, this);
        }

        #endregion Constructors.

        #region Public properties.

        /// <summary>Dispatcher on which test queue operates.</summary>
        public Dispatcher Dispatcher
        {
            get { return this._dispatcher; }
        }

        /// <summary>Retrieves the test queue for the specified dispatcher.</summary>
        /// <remarks>
        /// If there was no existing test queue for the specified dispatcher,
        /// a new one is created on-demand.
        /// </remarks>
        public static TestQueue FromDispatcher(Dispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            lock(s_lock)
            {
                TestQueue result;

                if (s_dispatcherQueues == null)
                {
                    s_dispatcherQueues = new Dictionary<Dispatcher, TestQueue>();
                }
                if (s_dispatcherQueues.ContainsKey(dispatcher))
                {
                    result = s_dispatcherQueues[dispatcher];
                }
                else
                {
                    // Creating a test queue will automatically map it to
                    // the dispatcher.
                    result = new TestQueue(dispatcher);
                }
                return result;
            }
        }

        /// <summary>Retrieves the test queue for the current thread.</summary>
        public static TestQueue ThreadQueue
        {
            get
            {
                return FromDispatcher(Dispatcher.CurrentDispatcher);
            }
        }

        #endregion Public properties.

        #region Public methods.

        /// <summary>
        /// Adds a callback to the specified method and arguments with the given
        /// priority.
        /// </summary>
        /// <param name="method">Method to call back to.</param>
        /// <param name="args">Arguments for method.</param>
        /// <param name="priority">Priority of callback.</param>
        public void AddItem(Delegate method, object[] args, DispatcherPriority priority)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            _queues[(int)priority].Enqueue(new QueueItem(method, args, priority, DateTime.Now));
            UpdateDispatcherStrategy();
        }

        /// <summary>
        /// Adds a callback to the specified method and arguments with the given
        /// priority.
        /// </summary>
        /// <param name="method">Method to call back to.</param>
        /// <param name="args">Arguments for method.</param>
        /// <param name="holdUntil">Time before which the delegate should not be invoked.</param>
        public void AddItem(Delegate method, object[] args, DateTime holdUntil)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            _delayedList.Add(new QueueItem(method, args, DispatcherPriority.SystemIdle, holdUntil));
            UpdateDispatcherStrategy();
        }

        /// <summary>Clears all content from the test queue.</summary>
        public void Clear()
        {
            for (int i = 0; i < _queues.Length; i++)
            {
                _queues[i].Clear();
            }
            _delayedList.Clear();
        }

        /// <summary>Returns a string representation of this object.</summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString()
        {
            string result;

            result = "TestQueue [" +
                "Input delay=" + _inputDelayRequired + ";" +
                "Delayed count=" + _delayedList.Count;
            for (int i = 0; i < _queues.Length; i++)
            {
                if (_queues[i].Count > 0)
                {
                    result += ";" + ((DispatcherPriority)i).ToString() +
                        " items=" + _queues[i].Count;
                }
            }
            result += "]";
            return result;
        }

        #endregion Public methods.

        #region Internal methods.

        /// <summary>
        /// Adds a calback to the specified user-input producing method.
        /// </summary>
        /// <param name="method">Method that will send user input.</param>
        /// <param name="args">Arguments to the callback.</param>
        internal void AddInput(Delegate method, object[] args)
        {
            QueueItem item;
            DispatcherPriority priority;

            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            priority = DispatcherPriority.Send;
            item = new QueueItem(method, args, priority, DateTime.Now);
            item.IsInput = true;
            _inputDelayRequired = true;
            _queues[(int)priority].Enqueue(item);

            UpdateDispatcherStrategy();
        }

        #endregion Internal methods.

        #region Internal properties.

        /// <summary>
        /// Whether the current thread is the main application thread.
        /// </summary>
        internal static bool IsThreadApplicationCurrent
        {
            get
            {
                Application application;

                application = Application.Current;
                if (application == null) return false;
                if (application.Dispatcher == null) return false;
                return Thread.CurrentThread == application.Dispatcher.Thread;
            }
        }

        #endregion Internal properties.

        #region Private methods.

        /// <summary>Selects and dispatches an item.</summary>
        private void DispatchItem()
        {
            QueueItem item;

            if (_inputDelayRequired)
            {
                return;
            }

            item = GetDispatchCandidate(true);
            if (item != null)
            {
                item.Invoke();
            }

            UpdateDispatcherStrategy();
        }

        /// <summary>Gets the most important item to be dispatched.</summary>
        /// <param name="remove">Whether the item should be removed from it container.</param>
        /// <returns>A QueueItem that can be dispatched immediately, null if there are none.</returns>
        /// <remarks>
        /// This may return somewhat erroneous results in that lower-priority
        /// items are returned before higher-priority items that were delayed
        /// in the past. The 'dispatched sometime after' constraint still holds,
        /// however.
        /// </remarks>
        private QueueItem GetDispatchCandidate(bool remove)
        {
            DateTime now;

            for (int i = (int)DispatcherPriority.Send; i > (int)DispatcherPriority.Inactive; i--)
            {
                if (_queues[i].Count > 0)
                {
                    QueueItem item;

                    item = _queues[i].Peek();
                    if (item.HoldUntil <= DateTime.Now)
                    {
                        if (remove)
                        {
                            _queues[i].Dequeue();
                        }
                        return item;
                    }
                }
            }

            now = DateTime.Now;
            foreach(QueueItem item in _delayedList)
            {
                if (item.HoldUntil <= now)
                {
                    if (remove)
                    {
                        _delayedList.Remove(item);
                    }
                    return item;
                }
            }

            return null;
        }

        /// <summary>Maps the specified queue to the given dispatcher.</summary>
        /// <param name="dispatcher">Dispatcher with queue.</param>
        /// <param name="queue">Queue for dispatcher.</param>
        private static void MapQueueToDispatcher(Dispatcher dispatcher, TestQueue queue)
        {
            lock (s_lock)
            {
                if (s_dispatcherQueues == null)
                {
                    s_dispatcherQueues = new Dictionary<Dispatcher, TestQueue>();
                }
                if (s_dispatcherQueues.ContainsKey(dispatcher))
                {
                    throw new InvalidOperationException(
                        "TestQueue already created for dispatcher " + dispatcher +
                        ". Access through FromDispatcher or ThreadQueue to " +
                        "prevent duplicates.");
                }
                s_dispatcherQueues.Add(dispatcher, queue);
            }
        }

        /// <summary>
        /// Updates the Avalon Dispatcher to enable this queue
        /// to enforce its priorities.
        /// </summary>
        private void UpdateDispatcherStrategy()
        {
            QueueItem item;

            if (_inputDelayRequired)
            {
                DispatcherTimer timer;

                timer = new DispatcherTimer(TimeSpan.FromMilliseconds(CiceroTimerMs),
                    DispatcherPriority.SystemIdle,
                    delegate(object sender, EventArgs e)
                {
                    DispatcherTimer senderTimer;
                    senderTimer = (DispatcherTimer)sender;
                    senderTimer.Stop();
                    this._inputDelayRequired = false;
                    DispatchItem();
                }, this.Dispatcher);
                timer.Start();
                return;
            }

            item = GetDispatchCandidate(false);
            if (item != null && !item.IsInSystemQueue)
            {
                _dispatcher.BeginInvoke(item.Priority, (SimpleHandler) DispatchItem);
                item.IsInSystemQueue = true;
            }
            else if (_delayedList.Count > 0)
            {
                QueueItem soonestItem;
                DispatcherTimer timer;
                TimeSpan interval;

                // There is nothing to be dispatched immediately.
                // Search for soonest delayed item and set up a timer for it.
                _delayedList.Sort(delegate(QueueItem left, QueueItem right) {
                    return left.HoldUntil.CompareTo(right.HoldUntil);
                });
                soonestItem = _delayedList[0];

                // Interval may be negative by the time we evaluate DateTime.Now.
                // The dispatcher timer requires a positive value.
                interval = soonestItem.HoldUntil.Subtract(DateTime.Now);
                if (interval < TimeSpan.Zero)
                {
                    interval = TimeSpan.FromMilliseconds(1);
                }

                timer = new DispatcherTimer(interval, soonestItem.Priority,
                    delegate(object sender, EventArgs e) {
                        DispatcherTimer senderTimer;
                        senderTimer = (DispatcherTimer)sender;
                        senderTimer.Stop();
                        DispatchItem();
                    }, this.Dispatcher);
                timer.Start();
            }
        }

        #endregion Private methods.


        #region Private fields.

        /// <summary>Dispatcher on which test queue operates.</summary>
        private Dispatcher _dispatcher;

        /// <summary>Array of item queues, in ascending priority order.</summary>
        private ItemQueue[] _queues;

        /// <summary>List of delayed items.</summary>
        private List<QueueItem> _delayedList;

        /// <summary>Lock object for static object access.</summary>
        private static object s_lock = new object();

        /// <summary>Dictionary with Dispatcher / TestQueue pairs.</summary>
        private static Dictionary<Dispatcher, TestQueue> s_dispatcherQueues;

        /// <summary>Flag set when a delay is required before dispatching input items.</summary>
        private bool _inputDelayRequired;

        /// <summary>Time to wait for Cicero updates, in milliseconds.</summary>
        private const int CiceroTimerMs = 450;

        #endregion Private fields.


        #region Inner Types.

        /// <summary>Item being queued.</summary>
        class QueueItem
        {
            internal QueueItem(Delegate method, object[] args, DispatcherPriority priority,
                DateTime holdUntil)
            {
                this.Method = method;
                this.Args = args;
                this.Priority = priority;
                this.HoldUntil = holdUntil;
            }

            internal void Invoke()
            {
                this.Method.DynamicInvoke(this.Args);
            }

            internal void BeginInvoke(Dispatcher dispatcher)
            {
                if (dispatcher == null)
                {
                    throw new ArgumentNullException("dispatcher");
                }
                if (DateTime.Now < HoldUntil)
                {
                    throw new InvalidOperationException("QueueItem.BeginInvoke " +
                        "called at " + DateTime.Now + ", but was setup to hold " +
                        "until " + HoldUntil);
                }

                if (Args == null || Args.Length == 0)
                {
                    dispatcher.BeginInvoke(Priority, Method);
                }
                else if (Args.Length == 1)
                {
                    dispatcher.BeginInvoke(Priority, Method, Args[1]);
                }
                else
                {
                    object firstArg;
                    object[] otherArgs;

                    // To pass an arbitrary amount of arguments, they need
                    // to be sliced like so.
                    firstArg = Args[0];
                    otherArgs = new object[Args.Length - 1];
                    Array.Copy(Args, 1, otherArgs, 0, otherArgs.Length);
                    dispatcher.BeginInvoke(Priority, Method, firstArg, otherArgs);
                }
            }

            /// <summary>
            /// IsInput
            /// </summary>
            public bool IsInput
            {
                get
                {
                    return _isInput;
                }
                set
                {
                    _isInput = value; 
                }
            }

            public Delegate Method;
            public object[] Args;
            public DispatcherPriority Priority;
            public DateTime HoldUntil;
            private bool _isInput;
            public bool IsInSystemQueue;
        }

        /// <summary>Queue of items for a given priority.</summary>
        class ItemQueue : Queue<QueueItem> { }

        #endregion Inner Types.


        #region Unit Tests.

        /// <summary>Entry point for TestQueue test cases.</summary>
        [STAThread]
        private static void Main()
        {
            Application application;

            application = new Application();
            application.Startup += delegate
            {
                s_testWindow = new Window();
                s_testWindow.Show();

                TestQueueItemCtor();

                TestCtor();
                TestThreadQueue();
                TestAddItem();
            };
            application.Run();
        }

        private static void Assert(bool condition, string conditionDescription)
        {
            if (!condition) throw new ApplicationException("Assertion failed: " + conditionDescription);
        }

        private static void TestQueueItemCtor()
        {
            QueueItem item;
            DateTime now;

            now = DateTime.Now;
            item = new QueueItem(
                (SimpleHandler) TestQueueItemCtor, new string[] { "foo" }, DispatcherPriority.Normal, now);
            Assert(item.Method.Method.Name == "TestQueueItemCtor", "Method set as exected.");
            Assert(item.Args[0].ToString() == "foo", "Arguments set as exected.");
            Assert(item.Priority == DispatcherPriority.Normal, "Priority set as exected.");
            Assert(item.HoldUntil == now, "HoldUntil set as exected.");
        }

        private static void TestCtor()
        {
            TestQueue queue;

            queue = new TestQueue();
            Assert(queue.Dispatcher == Dispatcher.CurrentDispatcher, "Default is current dispatcher.");
        }

        private static void TestThreadQueue()
        {
            TestQueue queue;

            queue = ThreadQueue;
            Assert(queue != null, "Thread TestQueue is created on-demand.");
            Assert(queue == ThreadQueue, "Single TestQueue is created on the same thread.");
            Assert(queue.Dispatcher == Dispatcher.CurrentDispatcher,
                "Thread TestQueue created on same dispatcher.");
        }

        private static void TestAddItem()
        {
            TestQueue queue;

            s_number = 0;
            queue = ThreadQueue;
            queue.AddItem((SimpleHandler)delegate { s_number++; }, null, DispatcherPriority.Normal);
            Assert(s_number == 0, "Callback was invoked immediately.");
            queue.AddItem((SimpleHandler)TestAddDelay, null, DispatcherPriority.Normal);
        }

        private static void TestAddDelay()
        {
            // (verification for TestAddItem)
            Assert(s_number == 1, "Callback was invoked.");

            s_stamp = DateTime.Now;
            ThreadQueue.AddItem((SimpleHandler)TestAddDelayCheck, null, s_stamp.AddSeconds(3));
        }

        private static void TestAddDelayCheck()
        {
            // (verification for TestAddDelay)
            Assert(s_stamp.AddSeconds(2) < DateTime.Now, "Callback was invoked.");

            ThreadQueue.AddItem((SimpleHandler)TestAddInput, null, DispatcherPriority.Normal);
        }

        private static void TestAddInput()
        {
            s_stamp = DateTime.Now;
            ThreadQueue.AddItem((SimpleHandler)TestAddInputShouldBeDelayedFromBefore, null, DispatcherPriority.Normal);
            ThreadQueue.AddInput((SimpleHandler)TestAddInputSender, null);
            ThreadQueue.AddItem((SimpleHandler)TestAddInputShouldBeDelayed, null, DispatcherPriority.Normal);
        }

        private static void TestAddInputSender()
        {
            Assert(DateTime.Now > s_stamp.AddMilliseconds(CiceroTimerMs), "Sender not called immediately");
            Thread.Sleep(500);
        }

        private static void TestAddInputShouldBeDelayedFromBefore()
        {
            Assert(DateTime.Now > s_stamp.AddMilliseconds(CiceroTimerMs + 500), "Items are delayed by input items.");
            Thread.Sleep(200);
        }

        private static void TestAddInputShouldBeDelayed()
        {
            Assert(DateTime.Now > s_stamp.AddMilliseconds(CiceroTimerMs + 700), "Items are delayed by input items.");
            s_testWindow.Close();
        }

        private static Window s_testWindow;
        private static int s_number;
        private static DateTime s_stamp;

        #endregion Unit Tests.
    }
}
