// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// <summary>

//  Adapts a TestServices.Test to a DRT.TestSuite.  PrepareResources is mapped 

//  to constructor, ReleaseResources maps to IDisposable, and Run will call

//  the TestSteps in sequence.

// </summary>



using System;

namespace DRT
{
    /// <summary>
    /// Adapts a TestServices.Test to a DRT.TestSuite.  PrepareResources is 
    /// mapped to constructor, ReleaseResources maps to IDisposable, and Run 
    /// will call the TestSteps in sequence.
    /// </summary>
    internal sealed class DrtTestSuiteAdapter : DrtTestSuite
    {
        #region Constructors
        //----------------------------------------------------------------------
        //
        // Constructor
        //
        //----------------------------------------------------------------------

        /// <summary>
        /// Creates an DrtTestSuiteAdapter.
        /// </summary>
        public DrtTestSuiteAdapter(
            MethodInvoker.MethodInvokerFactoryDelegate invokerFactory,
            Type test)
            // DrtTestSuite has no other way to set name
            : base((new TestInfo(test)).Name)
        {
            this.Contact = new TestInfo(test).Contact;

            _testType = test;
            _invokerFactory = invokerFactory;
            _testFactory = delegate { 
                return Activator.CreateInstance(_testType); };
        }
        #endregion

        #region Public Methods - Overrides DrtTestSuite
        //----------------------------------------------------------------------
        //
        // Public Methods - Overrides DrtTestSuite
        //
        //----------------------------------------------------------------------

        /// <summary>
        /// Simply returns the delegate to run the the underlying test object.
        /// </summary>
        /// <returns>An array with a single DrtTest delegate.</returns>
        public override DrtTest[] PrepareTests()
        {
            // construct objects
            _invoker = _invokerFactory();
            _test = _testFactory();

            return new DrtTest[] { new DrtTest(this.Run) };
        }

        /// <summary>
        /// Will call IDisposable.Dispose on the underlying test object.
        /// </summary>
        public override void ReleaseResources()
        {
            IDisposable disposable = _test as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
        #endregion

        #region Private Methods
        //----------------------------------------------------------------------
        //
        // Private Methods
        //
        //----------------------------------------------------------------------

        /// <summary>
        /// Delegate used by DrtBase which will run the test
        /// using an invoker provided at construction.
        /// </summary>
        private void Run()
        {
            TestEngine.Run(_invoker, _test);
        }
        #endregion

        #region Private Delegates
        //----------------------------------------------------------------------
        //
        // Private Delegates
        //
        //----------------------------------------------------------------------
        private delegate object TestFactoryDelegate();
        #endregion

        #region Private Fields
        //----------------------------------------------------------------------
        //
        // Private Fields
        //
        //----------------------------------------------------------------------
        private MethodInvoker _invoker;
        private object _test;
        private MethodInvoker.MethodInvokerFactoryDelegate _invokerFactory;
        private Type _testType;
        private TestFactoryDelegate _testFactory;
        #endregion
    }
}
