// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// <summary>

//  Adapts a TestServices.TestGroup to a DRT.DrtBase subclass.  It bridges the

//  TestServices functionality into DRT.  The class also includes DrtTestSuite

//  factory methods to convert a TestServices.Test to a DRT.DrtTestSuite.

// </summary>



using System;
using System.Security.Permissions;
using System.Security;
using System.Threading;
using System.Diagnostics;

namespace DRT
{
    /// <summary>
    /// Adapts a TestServices.TestGroup to a DRT.DrtBase subclass.  It bridges
    /// the TestServices functionality into DRT.  The class also includes
    /// DrtTestSuite factory methods to convert a TestServices.Test to a
    /// DRT.DrtTestSuite.
    /// </summary>
    /// <remarks>
    /// This class is an adapter to a DrtBase class.  
    /// 
    /// It proxies the functionality of TestServices and the subclass's 
    /// TestGroup attribute to DrtBase.
    /// 
    /// It has factory methods that return new DrtTestSuite for any class 
    /// marked with the Test attribute.
    /// </remarks>
    public abstract class DrtBaseAdapter : DrtBase
    {
        #region Constructors
        //----------------------------------------------------------------------
        // Constructors
        //----------------------------------------------------------------------

        /// <summary>
        /// Will connect TestServices to DrtBase set Drt properties by
        /// inspect the derived classes TestGroupAttribute.
        /// </summary>
        protected DrtBaseAdapter()
        {
            Type subclass = this.GetType();

            base.DrtName = subclass.Name;
            base.WindowTitle = subclass.Name;

            TestServices.Current.MessageSent += this.MessageSentHandler;

            TestGroupAttribute testGroup =
                TestServices.GetFirstAttribute(
                    typeof(TestGroupAttribute), subclass) as TestGroupAttribute;

            if (testGroup != null)
            {
                base.Contact = testGroup.Contact;
                base.TeamContact = testGroup.Team;
            }
            else
            {
                TestServices.Assert(
                    true, "TestGroup attribute missing from derived class.");
            }
        }
        #endregion Constructors

        #region Protected Methods
        //----------------------------------------------------------------------
        // Protected Methods
        //----------------------------------------------------------------------

        /// <summary>
        /// Will create a new DrtTestSuite to run the test instance provided.
        /// </summary>
        /// <param name="test">An instance of a test.</param>
        /// <returns>A new DrtTestSuite that will run the test.</returns>
        protected static DrtTestSuite CreateTestAdapter(Type test)
        {
            return new DrtTestSuiteAdapter(
                delegate
                {
                    return MethodInvoker.Chain(
             new ContextInvoker(),
             new MethodInvoker());
                },
                test);
        }

        /// <summary>
        /// Will create a new DrtTestSuite to run the test instance provided.
        /// </summary>
        /// <param name="test">An instance of a test.</param>
        /// <param name="context">Data for the test.</param>
        /// <returns>A new DrtTestSuite that will run the test.</returns>
        protected static DrtTestSuite CreateTestAdapter(
            Type test, TestContext context)
        {
            //MethodInvoker invoker = MethodInvoker.Chain(
            //        new ContextInvoker(context),
            //        new PartialTrustInvoker(),
            //        new MethodInvoker());
            return new DrtTestSuiteAdapter(
                delegate
                {
                    return MethodInvoker.Chain(
             new ContextInvoker(context),
             new MethodInvoker());
                },
                test);
        }
        #endregion Protected Methods

        #region Protected Methods - Overrides DrtBase
        //----------------------------------------------------------------------
        // Protected Methods - Overrides DrtBase
        //----------------------------------------------------------------------

        /// <summary>
        /// Called when the DRT is starting up -- after the Dispatcher has been created,
        /// before any suites are started; derived classes should call the base class
        /// method before doing anything. We start the DebugListener here if requested.
        /// </summary>
        protected override void OnStartingUp()
        {
            base.OnStartingUp();

            // Set up the listener for trace output (if needed)
            if (_listener)
            {
                System.Diagnostics.Trace.Listeners.Add(new TraceAdapter(this));
            }
        }

        /// <summary>
        /// Override this in derived classes to handle command-line arguments 
        /// one-by-one.
        /// </summary>
        /// <param name="arg">current argument</param>
        /// <param name="option">if there was a leading "-" or "/" to arg
        /// </param>
        /// <param name="args">the array of command line arguments</param>
        /// <param name="k">current index in the argument array.  passed by ref
        /// so you can increase it to "consume" arguments to options.</param>
        /// <returns>True if handled</returns>
        protected override bool HandleCommandLineArgument(
            string arg, bool option, string[] args, ref int k)
        {
            bool handled = false;
            switch (arg)
            {
                case "diagnose":
                    TestServices.Diagnose = true;
                    handled = true;
                    break;
                case "fulltrust":
                    handled = true;
                    break;
                case "listener":
                    _listener = true;
                    handled = true;
                    break;
                case "notimeout":
                    Call.ForceNoTimeout = true;
                    AutomationHelper.Timeout = Timeout.Infinite;
                    handled = true;
                    break;
                default:
                    handled = base.HandleCommandLineArgument(
                        arg, option, args,ref k);
                    break;
            }
            return handled;
        }

        /// <summary>
        /// Print a description of command line arguments.  Derived classes
        /// should override this to describe their own arguments, and then call
        /// base.PrintOptions() to get the DrtBase description.
        /// </summary>
        protected override void PrintOptions()
        {
            Console.WriteLine("TestServices.DLL options:");
            // Spacing Marker
            //  <..-Command..........Text........................................................>
            Console.WriteLine(
                "  -diagnose         Turns on consol output of internal trace information.");
            Console.WriteLine(
                "                    use to diagnose issues with TestServices.DLL");
            Console.WriteLine(
                "  -fulltrust        Forces [PartialTrust] SecurityZone to MyComputer for all.");
            Console.WriteLine(
                "                    use to verify issue is not PartialTrust related");
            Console.WriteLine(
                "  -listener         Turns on a TraceListener which publishes to DrtBase.");
            Console.WriteLine(
                "                    use to diagnose test failures");
            Console.WriteLine(
                "  -notimeout        Forces [TestStep(Timeout=Timeout.Infinite)] for all.");
            Console.WriteLine(
                "                    use to disable timeouts when debugging");
            base.PrintOptions();
        }
        #endregion Protected Methods - Overrides DrtBase

        #region Private Methods - Handlers TestServices
        //----------------------------------------------------------------------
        // Private Methods - Handlers TestServices
        //----------------------------------------------------------------------

        /// <summary>
        /// Bridges TestServices Messages to DrtBase or Console.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="args">The message arguments.</param>
        private void MessageSentHandler(object sender, MessageEventArgs args)
        {
            switch(args.Category)
            {
                case MessageEventArgs.MessageCategory.Internal:
                    if (TestServices.Diagnose)
                    {
                        if (base.DelayOutput)
                        {
                            DRT.LogOutput(args.Message);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.WriteLine(args.Message);
                            Console.ResetColor();
                        }
                    }
                    break;

                case MessageEventArgs.MessageCategory.Trace:
                    DRT.Trace(args.Message);
                    break;

                case MessageEventArgs.MessageCategory.Log:
                    DRT.LogOutput(args.Message);
                    break;

                case MessageEventArgs.MessageCategory.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(args.Message);
                    Console.ResetColor();
                    break;

                case MessageEventArgs.MessageCategory.Assert:
                    DRT.Assert(false, args.Message);
                    break;
            }
        }
        #endregion Private Methods - Handlers TestServices

        #region Private Fields
        //----------------------------------------------------------------------
        // Private Fields
        //----------------------------------------------------------------------

        /// <summary>
        /// When true we will create a trace listener and publish it's messages
        /// to DrtBase.
        /// </summary>
        private bool _listener = false;
        #endregion Private Fields

        /// <summary>
        /// Will bridge Diagnostics.Trace events to DrtBase.
        /// </summary>
        private class TraceAdapter : TraceListener
        {
            #region Constructors
            //------------------------------------------------------------------
            // Constructors
            //------------------------------------------------------------------

            /// <summary>
            /// Constructs a new TraceListner which will publish the events
            /// to the DrtBase provided.
            /// </summary>
            /// <param name="drtBase">The DrtBase to send events to.</param>
            public TraceAdapter(DrtBase drtBase)
            {
                _drtBase = drtBase;
            }
            #endregion Constructors

            #region Public Methods - TraceListner Overrides
            //------------------------------------------------------------------
            // Public Methods - TraceListner Overrides
            //------------------------------------------------------------------

            /// <summary>
            /// Calls DrtBase.Trace with the message.
            /// </summary>
            /// <remarks>
            /// There is no DrtBase method which allows us to write a partial
            /// line so this is the best fit.
            /// </remarks>
            /// <param name="message">The message.</param>
            public override void Write(string message)
            {
                _drtBase.Trace(message);
            }

            /// <summary>
            /// Calls DrtBase.Trace with the message.
            /// </summary>
            /// <param name="message">The message.</param>
            public override void WriteLine(string message)
            {
                _drtBase.Trace(message);
            }
            #endregion Public Methods - TraceListner Overrides

            #region Private Fields
            //------------------------------------------------------------------
            //  Private Fields
            //------------------------------------------------------------------

            DrtBase _drtBase;
            #endregion Private Fields
        }
    }
}
