// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// <summary>

/// Global test services (asserts, eventing, attribute inspection)

/// for the framework.

// </summary>



using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Security.Permissions;
using System.Security;

namespace DRT
{
    /// <summary>
    /// Global test services (asserts, eventing, attribute inspection)
    /// for the framework.
    /// </summary>
    /// <remarks>
    /// Implementation Note:
    /// Orginally was a static class, however, needed to bridge events
    /// accross application domains.  So it is now a MarshalByRefObject 
    /// singleton (across application domains).
    /// </remarks>
    public class TestServices : MarshalByRefObject
    {
        #region Constructors
        //----------------------------------------------------------------------
        // Constructors
        //----------------------------------------------------------------------

        /// <summary>
        /// Class is a singleton; declared to enforce this by contract.
        /// </summary>
        private TestServices() {}
        #endregion Constructors

        #region Public Methods
        //----------------------------------------------------------------------
        // Public Methods
        //----------------------------------------------------------------------

        /// <summary>
        /// Adds a performance counter to the list of counters that will be
        /// logged.
        /// </summary>
        /// <param name="counter">A performance counter to log.</param>
        public static void Add(PerformanceCounter counter)
        {
            if (IsBridged)
            {
                Warning(
                    "Performance counter {0}.{1}.{2} is from a foreign application domain and will incure marshaling costs.",
                    counter.CategoryName,
                    counter.CounterName,
                    counter.InstanceName);
            }

            Current._counters.Add(counter);
        }

        /// <summary>
        /// Will raise an Assert event if the condition is not true.
        /// </summary>
        /// <param name="condition">Expression to evaluate.</param>
        /// <param name="format">A format string used to build the message.</param>
        /// <param name="args">A list of arguments for the format string.</param>
        public static void Assert(
            bool condition, string format, params object[] args)
        {
            string message = BuildMessage(format, args);
            if (!condition)
            {
                Current.RaiseMessageSent(
                    MessageEventArgs.MessageCategory.Assert, message);

                InternalTrace("Failed Assert(\"{0}\")", message);
            }
            else
            {
                InternalTrace("Passed Assert(\"{0}\")", message);
            }
        }

        /// <summary>
        /// Will raise a Warning event.
        /// </summary>
        /// <param name="format">A format string used to build the message.
        /// </param>
        /// <param name="args">A list of arguments for the format string.
        /// </param>
        public static void Warning(string format, params object[] args)
        {
            string message = BuildMessage(format, args);
            Current.RaiseMessageSent(
                MessageEventArgs.MessageCategory.Warning, message);

            InternalTrace(message);
        }

        /// <summary>
        /// Will raise a Log event.
        /// </summary>
        /// <param name="format">A format string used to build the message.
        /// </param>
        /// <param name="args">A list of arguments for the format string.
        /// </param>
        public static void Log(string format, params object[] args)
        {
            string message = BuildMessage(format, args);
            Current.RaiseMessageSent(
                MessageEventArgs.MessageCategory.Log, message);

            InternalTrace(message);
        }

        /// <summary>
        /// Will log all performance counters in our list.
        /// </summary>
        /// <param name="label">A friendly label for the
        /// performance check point.</param>
        public static void LogPerformance(string label)
        {
            List<PerformanceCounter> invalid = null;

            foreach (PerformanceCounter counter in Current._counters)
            {
                try
                {
                    Log("Perf: {4} [{0}].[{1}].[{2}] = {3}",
                        counter.CategoryName,
                        counter.CounterName,
                        counter.InstanceName,
                        counter.RawValue,
                        label);
                }
                catch (InvalidOperationException)
                {
                    if (invalid == null)
                    {
                        invalid = new List<PerformanceCounter>();
                    }

                    invalid.Add(counter);
                }
            }

            if (invalid != null)
            {
                foreach (PerformanceCounter counter in invalid)
                {
                    Current._counters.Remove(counter);
                    Log("Perf: {4} [{0}].[{1}].[{2}] removed as it is no longer valid.",
                        counter.CategoryName,
                        counter.CounterName,
                        counter.InstanceName,
                        label);
                }
            }
        }

        /// <summary>
        /// Will raise a Trace event.
        /// </summary>
        /// <param name="format">A format string used to build the message.
        /// </param>
        /// <param name="args">A list of arguments for the format string.
        /// </param>
        public static void Trace(string format, params object[] args)
        {
            // If there are no args, the given string should be treated as literal.
            // Otherwise, if it happens to be a GUID like {001B...}, we'd get an exception from string.Format()
            // because it sees a format specifier in the string.
            string message = args.Length == 0 ? format : BuildMessage(format, args);
            Current.RaiseMessageSent(
                MessageEventArgs.MessageCategory.Trace, message);

            InternalTrace(message);
        }

        /// <summary>
        /// Will set the remote reference used for the singleton this class
        /// in the foreign application domain.
        /// </summary>
        /// <param name="foreignDomain">The foreign applicatoin domaion.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public void SetBridge(AppDomain foreignDomain)
        {
            lock (typeof(TestServices)) // Lock: Scope=Type Order=1 
            {
                foreignDomain.SetData(TestServiceDataSlotName, this);
            }
        }
        #endregion Public Methods

        #region Public Properties
        //----------------------------------------------------------------------
        // Public Properties
        //----------------------------------------------------------------------


        /// <summary>
        /// Returns true if this static type is having calls forwarded to
        /// another application domain.
        /// </summary>
        public static bool IsBridged
        {
            get
            {
                // since this lock is used on almost every call
                // exploring a less expensive way would be a good
                // optimization to consider
                lock (typeof(TestServices)) // Lock: Scope=Type Order=1 
                {
                    if (!_isBridged.HasValue)
                    {
                        _isBridged = new Nullable<bool>(
                            ((AppDomain.CurrentDomain.GetData(
                                TestServiceDataSlotName) as TestServices) != null));
                    }

                    return _isBridged.Value;
                }
            }
        }

        /// <summary>
        /// Returns the singleton.
        /// </summary>
        public static TestServices Current
        {
            get
            {
                // since this lock is used on almost every call
                // exploring a less expensive way would be a good
                // optimization to consider
                lock (typeof(TestServices)) // Lock: Scope=Type Order=1 
                {
                    if (_singleton == null)
                    {
                        // if a bridge was set use it
                        _singleton = AppDomain.CurrentDomain.GetData(
                            TestServiceDataSlotName) as TestServices;
                        
                        if (_singleton == null)
                        {
                            // otherwise create one
                            _singleton = new TestServices();
                        }
                    }

                    return _singleton;
                }
            }
        }


        /// <summary>
        /// True enables diagnostics; we will generate diagnostic information
        /// and perform self testing.
        /// </summary>
        /// <remarks>
        /// Other classes will check this value to enable diagnostic behavior.
        /// </remarks>
        public static bool Diagnose
        {
            get { return _diagnose; }
            set { _diagnose = value; }
        }
        #endregion Public Properties

        #region Public Events
        //----------------------------------------------------------------------
        // Public Events
        //----------------------------------------------------------------------

        /// <summary>
        /// An Message has been sent.
        /// </summary>
        public event EventHandler<MessageEventArgs> MessageSent;
        #endregion Public Events

        #region Internal Methods
        //----------------------------------------------------------------------
        // Internal Methods
        //----------------------------------------------------------------------

        /// <summary>
        /// Used for trace events that the test system does not want published.
        /// 
        /// Example debug only statements.
        /// </summary>
        /// <param name="format">A format string used to build the message.
        /// </param>
        /// <param name="args">A list of arguments for the format string.
        /// </param>
        internal static void InternalTrace(string format, params object[] args)
        {
            // If there are no args, the given string should be treated as literal.
            // Otherwise, if it happens to be a GUID like {001B...}, we'd get an exception from string.Format()
            // because it sees a format specifier in the string.
            string message = args.Length == 0 ? format : 
                string.Format(CultureInfo.CurrentCulture, format, args);

            System.Threading.Thread current =
                System.Threading.Thread.CurrentThread;

            message = string.Format(
                CultureInfo.CurrentCulture,
                "[D:{0}][T:{1}]{2}",
                AppDomain.CurrentDomain.IsDefaultAppDomain() ?
                    "Default" : AppDomain.CurrentDomain.FriendlyName,
                current.IsThreadPoolThread ?
                    current.ManagedThreadId.ToString() : current.Name,
                message);

            Current.RaiseMessageSent(
                MessageEventArgs.MessageCategory.Internal, message);

        }

        /// <summary>
        /// Returns the first attribute of the type specified on the class.
        /// </summary>
        /// <param name="attributeType">Attribute to look for.</param>
        /// <param name="classType">Class to inspect.</param>
        /// <returns>The first attribute found.</returns>
        internal static Attribute GetFirstAttribute(
            Type attributeType, Type classType)
        {
            Attribute result = null;

            Attribute[] attributes =
                (Attribute[])classType.GetCustomAttributes(
                    attributeType, false);

            if (attributes != null && attributes.Length != 0)
            {
                result = attributes[0];
            }

            return result;
        }

        /// <summary>
        /// Returns the first attribute of the type specified on the method.
        /// </summary>
        /// <param name="attributeType">Attribute to look for.</param>
        /// <param name="method">Method to inspect.</param>
        /// <returns>The first attribute found.</returns>
        internal static Attribute GetFirstAttribute(
            Type attributeType, MethodInfo method)
        {
            Attribute result = null;

            Attribute[] attributes =
                (Attribute[])method.GetCustomAttributes(attributeType, false);

            if (attributes != null && attributes.Length != 0)
            {
                result = attributes[0];
            }

            return result;
        }

        /// <summary>
        /// Returns true if the declaring method has an attribute of that type.
        /// </summary>
        /// <param name="attributeType">Attribute to look for.</param>
        /// <param name="method">Method to inspect.</param>
        /// <returns>Returns true if the declaring method has an attribute of
        /// that type.</returns>
        internal static bool HasAttribute(Type attributeType, MethodInfo method)
        {
            return method.GetCustomAttributes(attributeType, false).Length != 0;
        }
        #endregion Internal Methods

        #region Private Methods
        //----------------------------------------------------------------------
        // Private Methods
        //----------------------------------------------------------------------

        /// <summary>
        /// Raise the message.
        /// </summary>
        /// <param name="category">Category of message.</param>
        /// <param name="message">Message to raise.</param>
        private void RaiseMessageSent(
            MessageEventArgs.MessageCategory category, string message)
        {
            if (Current.MessageSent != null)
            {
                Current.MessageSent(this, 
                    new MessageEventArgs(category, message));
            }
        }

        /// <summary>
        /// As calling code may be under restricted priveledge and diagnosics
        /// for traces and messages may need higher permissions, this method
        /// will format the message with elevated permissions.
        /// </summary>
        /// <param name="format">A format string used to build the message.
        /// </param>
        /// <param name="args">A list of arguments for the format string.
        /// </param>
        /// <returns>The message.</returns>
        [SecurityCritical, SecurityTreatAsSafe]
        private static string BuildMessage(string format, params object[] args)
        {
            return string.Format(
                    CultureInfo.CurrentCulture, format, args);
        }
        #endregion Private Methods

        #region Private Fields
        //----------------------------------------------------------------------
        // Private Fields
        //----------------------------------------------------------------------

        private const string TestServiceDataSlotName = "TestServices";
        private static TestServices _singleton;
        private static Nullable<bool> _isBridged;
        private static bool _diagnose;

        private List<PerformanceCounter> _counters = new List<PerformanceCounter>();
        #endregion Private Fields
    }

    #region Attributes
    /// <summary>
    /// Marks a class as a group of tests for TestServices.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TestGroupAttribute : Attribute
    {
        #region Constructors
        //----------------------------------------------------------------------
        // Constructors
        //----------------------------------------------------------------------

        /// <summary>
        /// Marks a class as a group of tests for TestServices.
        /// </summary>
        /// <param name="contact">Alias for the person responsible for the
        /// TestGroup (DRT).</param>
        /// <param name="team">Alias for the team responsible for the
        /// TestGroup (DRT).</param>
        public TestGroupAttribute(string contact, string team)
        {
            this._contact = contact;
            this._team = team;
        }
        #endregion Constructors

        #region Public Properties
        //----------------------------------------------------------------------
        // Public Properties
        //----------------------------------------------------------------------

        /// <summary>
        /// Contact for the test group. (alias)
        /// </summary>
        public string Contact
        {
            get { return this._contact; }
        }

        /// <summary>
        /// Team for the test group. (alias)
        /// </summary>
        public string Team
        {
            get { return this._team; }
        }
        #endregion Public Properties

        #region Private Fields
        //----------------------------------------------------------------------
        // Private Fields
        //----------------------------------------------------------------------

        private string _contact;
        private string _team;
        #endregion Private Fields
    }

    /// <summary>
    /// Marks a class as a test for TestServices.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TestAttribute : Attribute
    {
        #region Constructors
        //----------------------------------------------------------------------
        // Constructors
        //----------------------------------------------------------------------

        /// <summary>
        /// Marks a class as a test for TestServices.
        /// </summary>
        /// <param name="name">Name of the test.</param>
        /// <param name="contact">Alias for the person responsible for the test.
        /// </param>
        public TestAttribute(string name, string contact)
        {
            this._name = name;
            this._contact = contact;
        }
        #endregion Constructors

        #region Public Properties
        //----------------------------------------------------------------------
        // Public Properties
        //----------------------------------------------------------------------

        /// <summary>
        /// Name of the test.
        /// </summary>
        public string Name
        {
            get { return this._name; }
        }

        /// <summary>
        /// Name of the person responsible for the test. (alias)
        /// </summary>
        public string Contact
        {
            get { return this._contact; }
        }
        #endregion Public Properties

        #region Private Fields
        //----------------------------------------------------------------------
        // Private Fields
        //----------------------------------------------------------------------

        private string _name;
        private string _contact;
        #endregion Private Fields
    }

    /// <summary>
    /// Marks a method as a test step for TestServices.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class TestStepAttribute : Attribute
    {
        #region Constructors
        //----------------------------------------------------------------------
        // Constructors
        //----------------------------------------------------------------------

        /// <summary>
        /// Marks a method as a test step for TestServices.
        /// </summary>
        public TestStepAttribute() {}

        /// <summary>
        /// Marks a method as a test step for TestServices.
        /// </summary>
        /// <param name="name">Name of the test step.</param>
        public TestStepAttribute(string name)
        {
            this._name = name;
        }
        #endregion Constructors

        #region Public Properties
        //----------------------------------------------------------------------
        // Public Properties
        //----------------------------------------------------------------------

        /// <summary>
        /// Indicates the required appartment state for the test step.
        /// </summary>
        public ApartmentState ApartmentState
        {
            get { return _apartment; }
            set { _apartment = value; }
        }
        /// <summary>
        /// Indicates if the step should run asynchronously.
        /// Default False.
        /// </summary>
        public bool Async
        {
            get { return _async; }
            set { _async = value; }
        }
        
        /// <summary>
        /// Amount of time in milliseconds to wait before starting the test.
        /// Default 0.
        /// </summary>
        public uint Delay
        {
            get { return _delay; }
            set { _delay = value; }
        }

        /// <summary>
        /// Name of the test step.
        /// </summary>
        public string Name
        {
            get { return this._name; }
        }

        /// <summary>
        /// The sequence the step will be executed in.
        /// Default is physical order.
        /// </summary>
        public uint Order
        {
            get { return _order; }
            set { _order = value; }
        }
	
        
        /// <summary>
        /// Amount of time in milliseconds before timeout.
        /// Default 30,000.
        /// A timeout will cause a TestServices.Assert
        /// </summary>
        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        /// <summary>
        /// The name of a TestStep to wait for before execution.
        /// </summary>
        public string WaitFor
        {
            get { return _waitFor; }
            set { _waitFor = value; }
        }
	
        #endregion Public Properties

        #region Private Fields
        //----------------------------------------------------------------------
        // Private Fields
        //----------------------------------------------------------------------

        private ApartmentState _apartment;
        private bool _async; // default: Non-Async or Blocking
        private uint _delay; // default: 0 sec
        private string _name;
        private uint _order = uint.MaxValue;
        private int _timeout = 90000; // default: 90 sec
        private string _waitFor;
        #endregion Private Fields
    }
    #endregion Attributes

    #region EventArgs
    /// <summary>
    /// When a TestServices message event occurs, this type contains the information for the event.
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        #region Constructors
        //----------------------------------------------------------------------
        // Constructors
        //----------------------------------------------------------------------

        /// <summary>
        /// Contains the information for a TestServcies MessageEvent.
        /// </summary>
        /// <param name="category">The category of Message (Assert, Log, Trace).
        /// </param>
        /// <param name="message">The text of the message.</param>
        public MessageEventArgs(MessageCategory category, string message)
        {
            _category = category;
            _message = message;
        }
        #endregion Constructors

        #region Public Properties
        //----------------------------------------------------------------------
        // Public Properties
        //----------------------------------------------------------------------

        /// <summary>
        /// The text of the message
        /// </summary>
        public string Message
        {
            get { return _message; }
        }

        /// <summary>
        /// The category of Message (Assert, Log, Trace).
        /// </summary>
        public MessageCategory Category
        {
            get { return _category; }
        }
        #endregion Public Properties

        #region Private Fields
        //----------------------------------------------------------------------
        // Private Fields
        //----------------------------------------------------------------------

        private string _message;
        private MessageCategory _category;
        #endregion Private Fields

        /// <summary>
        /// These are the categories of messages TestServices will raise.
        /// </summary>
        public enum MessageCategory
        {
            /// <summary>
            /// An Assertion failed.
            /// </summary>
            Assert,
            /// <summary>
            /// Message should be presented to user indicating a non failure
            /// issue.
            /// </summary>
            Warning,
            /// <summary>
            /// Message should be presented to user.
            /// </summary>
            Log,
            /// <summary>
            /// Message should be writen in diagnostic modes.
            /// </summary>
            Trace,
            /// <summary>
            /// Message is for diagnosing issues with TestServices only.
            /// </summary>
            Internal
        }
    }
    #endregion EventArgs
}
