// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides queue services for test cases.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Utils/Queue.cs $")]

namespace Test.Uis.Utils
{
    #region Namespaces.

    using System;
    using System.Threading;
    using System.Windows.Threading;

    using System.Windows;

    #endregion Namespaces.

    #region Delegates.

    /// <summary>Supports the simplest possible callback.</summary>
    public delegate void SimpleHandler();

    /// <summary>Supports the dispatcher callback.</summary>
    /// <remarks>In practice, this is identical to SimpleHandler.</remarks>
    public delegate void OnDispatchHandler();

    #endregion Delegates.

    /// <summary>This class provides queue helper methods to test cases.</summary>
    /// <remarks>
    /// <p>This class is tightly coupled with the Avalon queue model.
    /// To enable logging of queuing items, set the LogTestQueuing and
    /// LogTestDispatching arguments through the library configuration
    /// object.</p></remarks>
    public class QueueHelper
    {

        #region Constructors.

        /// <summary>Initializes a new Test.Uis.Queue.QueueHelper instance.</summary>
        public QueueHelper() : this(Dispatcher.CurrentDispatcher, DispatcherPriority.SystemIdle)
        {
        }

        /// <summary>
        /// Initializes a new Test.Uis.Queue.QueueHelper instance
        /// for a specific context.
        /// </summary>
        /// <param name="context">UiContext to which items will be posted.</param>
        public QueueHelper(Dispatcher context) : this(context, DispatcherPriority.SystemIdle)
        {
        }


        /// <summary>
        /// Creates a new Test.Uis.Queue.QueueHelper instance
        /// for a specific context.
        /// </summary>
        /// <param name="itemPriority">Priority of the posted item. See ItemPriority enum for details</param>
        public QueueHelper(DispatcherPriority itemPriority) : this(Dispatcher.CurrentDispatcher, itemPriority)
        {
        }

        /// <summary>
        /// Creates a new Test.Uis.Queue.QueueHelper instance
        /// for a specific context.
        /// </summary>
        /// <param name="dispatcher">UiContext to which items will be posted.</param>
        /// <param name="itemPriority">Priority of the posted item. See ItemPriority enum for details</param>
        public QueueHelper(Dispatcher dispatcher, DispatcherPriority itemPriority)
        {
            this._itemPriority = itemPriority;
            this._dispatcher = dispatcher;
        }

        #endregion Constructors.


        #region Public properties.

        /// <summary>Retrieves the current queue helper.</summary>
        /// <remarks>
        /// This actually creates an on-the-fly queue helper
        /// that can service the current UiContext.
        /// </remarks>
        public static QueueHelper Current
        {
            get { return new QueueHelper(); }
        }

        /// <summary>Priority for posted items.</summary>
        /// <remarks>
        /// <p>
        /// This property defaults to DispatcherPriority.SystemIdle. This
        /// ensures that the queue is empty before this item is processed.
        /// All Idle items are processed on a first-come, first-served basis,
        /// but only when no other items are to be processed. This means that
        /// if two items are enqueued and the first posts events like
        /// keystrokes, the keystrokes will be processed before the second
        /// item.
        /// </p><p>
        /// Use DispatcherPriority.Background to interleave the
        /// posted item with input.
        /// </p>
        /// </remarks>
        public DispatcherPriority ItemPriority
        {
            get { return this._itemPriority; }
            set { this._itemPriority = value; }
        }

        /// <summary>Argument name to enable queuing logs.</summary>
        public const string LogTestQueuingArgName = "LogTestQueuing";

        /// <summary>Argument name to enable dispatching logs.</summary>
        public const string LogTestDispatchingArgName = "LogTestDispatching";

        #endregion Public properties.


        #region Private methods.

        /// <remarks>Describes a delegate instance.</remarks>
        /// <param name="instance">Delegate to describe.</param>
        /// <returns>A description of the delegate.</returns>
        private static string DescribeDelegate(Delegate instance)
        {
            new System.Security.Permissions.ReflectionPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return String.Format(
                "{0} ({1}, {2})", instance, instance.Method, instance.Target);
        }

        #endregion Private methods.


        #region Public methods.

        /// <summary>
        /// Queues an arbitrary delegate for defered dispatching.
        /// </summary>
        /// <param name="instance">
        /// The SimpleHandler delegate to invoke on dispatch.
        /// </param>
        /// <remarks>
        /// If the setting "LogTestQueueing" is set to "true", this
        /// method will write to the current logger.
        /// </remarks>
        /// <example>The following sample shows how to use this method.<code>...
        /// private void MyMethod() {
        ///   QueueHelper.Current.QueueDelegate(new SimpleHandler(MyHandler));
        /// }
        /// ...
        /// private void MyHandler() {
        ///   System.Console.WriteLine("Hello, world!");
        /// }</code></example>
        public void QueueDelegate(SimpleHandler instance)
        {
            QueueDelegate(instance, null);
        }

        /// <summary>
        /// Queues an arbitrary delegate for defered dispatching.
        /// </summary>
        /// <param name="instance">The delegate to invoke on dispatch.</param>
        /// <param name="args">The argument list for the delegate.</param>
        /// <remarks>
        /// If the setting "LogTestQueuing" is set to "true", this
        /// method will write to the current logger.
        /// </remarks>
        /// <example>The following sample shows how to use this method.<code>...
        /// private void MyMethod() {
        ///   QueueHelper.Current.QueueDelegate(new MyCustomDelegate(MyHandler),
        ///     new object[] { "Hello, world!" });
        /// }
        /// ...
        /// private void MyHandler(string msg) {
        ///   System.Console.WriteLine(msg);
        /// }</code></example>
        public void QueueDelegate(Delegate instance, params object[] args)
        {
            bool shouldLog; // Whether the operation should be logged.

            shouldLog = ConfigurationSettings.Current.GetArgumentAsBool(
                LogTestQueuingArgName);
            if (shouldLog)
            {
                string description; // Description of delegate to post.

                description = DescribeDelegate(instance);
                Test.Uis.Loggers.Logger.Current.Log("Posting " + description);
            }

            Test.Uis.Threading.TestQueue.FromDispatcher(_dispatcher)
                .AddItem(instance, args, this.ItemPriority);
        }

        /// <summary>
        /// Queues an arbitrary delegate to be dispatched in the future.
        /// </summary>
        /// <param name="delay">The minimum time to wait before dispatching the item.</param>
        /// <param name="instance">The delegate to invoke on dispatch.</param>
        /// <param name="args">The argument list for the delegate.</param>
        /// <remarks>
        /// If the setting "LogTestQueuing" is set to "true", this
        /// method will write to the current logger.
        /// </remarks>
        /// <example>The following sample shows how to use this method.<code>...
        /// private void MyMethod() {
        ///   QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromMillisecond(1500),
        ///     new MyCustomDelegate(MyHandler),
        ///     new object[] { "Hello, world!" });
        /// }
        /// ...
        /// private void MyHandler(string msg) {
        ///   System.Console.WriteLine(msg);
        /// }</code></example>
        public void QueueDelayedDelegate(TimeSpan delay,
            Delegate instance, params object[] args)
        {
            Test.Uis.Threading.TestQueue.FromDispatcher(_dispatcher)
                .AddItem(instance, args, DateTime.Now.Add(delay));
        }

        #endregion Public methods.


        #region Private fields.

        /// <summary>Default priority for items.</summary>
        private DispatcherPriority _itemPriority;

        /// <summary>Dispatcher to which queue is bound.</summary>
        private Dispatcher _dispatcher;

        #endregion Private fields.
    }
}
