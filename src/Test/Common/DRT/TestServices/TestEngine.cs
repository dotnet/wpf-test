// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// <summary>

//  This class is responsible for excuting the TestSteps in a Test.

// </summary>




using System;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;

namespace DRT
{
    /// <summary>
    /// This class is responsible for excuting the TestSteps in a Test.
    /// </summary>
    public static class TestEngine
    {
        #region Public Methods
        //----------------------------------------------------------------------
        // Public Methods
        //----------------------------------------------------------------------

        /// <summary>
        /// Will invoke all public methods in the test that are marked
        /// with the TestStep attribute using the provided method invoker.
        /// </summary>
        /// <param name="invoker">A MethodInvoker</param>
        /// <param name="test">An object with test steps.</param>
        public static void Run(MethodInvoker invoker, object test)
        {
            RunOnce();

            ExecutionPlan plan = BuildDefaultExecutionPlan(invoker, test);

            LogExecutionPlan(plan);

            ExecutePlan(plan);
        }
        #endregion Public Methods

        #region Private Methods
        //----------------------------------------------------------------------
        // Private Methods
        //----------------------------------------------------------------------

        /// <summary>
        /// Will execute only once regardless of the number of times called.
        /// </summary>
        private static void RunOnce()
        {
            lock (typeof(TestEngine)) // Lock: Scope=Type Order=1
            {
                if (!_hasRunOnce)
                {
                    // only run self tests in diagnostic mode
                    if (TestServices.Diagnose)
                    {
                        SelfTest();
                    }
                    _hasRunOnce = true;
                }
            }
        }

        /// <summary>
        /// Will Test TestServices.
        /// </summary>
        private static void SelfTest()
        {
            MethodInvoker invoker = MethodInvoker.Chain(new MethodInvoker[] {
                    new ContextInvoker() });

            // Execute the test plan for every type in our assembly that is
            // attributed as a Test.
            foreach (Type type in typeof(TestEngine).Assembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(TestAttribute), false).Length > 0)
                {
                    TestServices.Log("Diagnostic Test: {0} will be executed.", type.Name);
                    ExecutePlan(BuildDefaultExecutionPlan(
                        invoker,
                        Activator.CreateInstance(type)));
                }
            }
        }

        /// <summary>
        /// Will execute test steps in a given execution plan.
        /// </summary>
        /// <param name="plan">The execution plan.</param>
        private static void ExecutePlan(ExecutionPlan plan)
        {
            DateTime started = DateTime.Now;

            foreach (Call call in plan.Calls)
            {
                if (call.MethodInvoker.HadCriticalFailure)
                {
                    TestServices.Log(
                        "Step: {0} skipped due to MethodInvoker failure.",
                        plan.TestSteps[call].Name);
                }
                else
                {
                    TestServices.Log(
                        "Step: {0} started.",
                        plan.TestSteps[call].Name); 
                    call.DoCall();
                }
            }

            foreach (Call call in plan.Calls)
            {
                call.WaitForCall();

                TestServices.Log(
                    call.PostCallSummary());
            }

            TestServices.Log(
                "Execution Time: {0}ms",
                (int)(DateTime.Now - started).TotalMilliseconds);
        }

        /// <summary>
        /// Will construct the default execution plan.  The default plan orders
        /// test steps by Order parameter, then by phsyical order in the class.
        /// </summary>
        /// <param name="invoker">The invoker to use in the plan.</param>
        /// <param name="test">The test to generate the plan for.</param>
        /// <returns>An execution plan.</returns>
        private static ExecutionPlan BuildDefaultExecutionPlan(
            MethodInvoker invoker, object test)
        {
            // all methods in the test
            MethodInfo[] methods = test.GetType().GetMethods();
            int count = methods.Length;

            // use to store all the calls in the execution plan
            // default plan will only execute a test step method once so the
            // number of calls <= the count of methods
            Dictionary<string, Call> calls =
                new Dictionary<string, Call>(count);

            // use to get the TestStep attribute for the call
            // testSteps are attributes on methods thus they are <= the count
            // of methods.
            Dictionary<Call, TestStepAttribute> testSteps =
                new Dictionary<Call, TestStepAttribute>(count);

            // use to get the orginal physical order of the call
            // default plan will only execute a test step method once so the
            // number of calls <= the count of methods
            Dictionary<Call, int> physicalOrder =
                new Dictionary<Call, int>(count);

            // for every method in the test object
            for (int i = 0; i < count; i++)
            {
                MethodInfo method = methods[i];

                TestStepAttribute attrib =
                    TestServices.GetFirstAttribute(typeof(TestStepAttribute), method)
                    as TestStepAttribute;

                // if an attribute was found, it is a TestStep
                if (attrib != null)
                {
                    // again in the default plan calls map 1:1 to methods that
                    // are test steps, so create a call for the method
                    Call call = new Call(
                        invoker,
                        method,
                        test,
                        new object[method.GetParameters().Length],
                        GetDependencies(attrib, calls));

                    if (!calls.ContainsKey(method.Name))
                    {
                        calls.Add(method.Name, call);
                        physicalOrder.Add(call, i);
                        testSteps.Add(call, attrib);
                    }
                    else
                    {
                        TestServices.Warning(
                            "Skipping TestStep \"{0}\".\n" +
                            "Overloads are not allowed for methods marked with TestStep attribute.",
                            method.Name);
                    }
                }
            }

            // goal: order calls by oder parameter, then by position
            List<Call> sorted = new List<Call>(calls.Values);
            sorted.Sort(delegate(Call left, Call right) 
                {
                    int result = left.Order.CompareTo(right.Order);
                    // if the order is the same sub sort by physical order
                    if (result == 0)
                    {
                        result = physicalOrder[left]
                            .CompareTo(physicalOrder[right]);
                    }
                    return result; 
                });

            // build the execution plan
            ExecutionPlan plan;
            plan.TestSteps = testSteps;
            plan.Calls = sorted;
            return plan;
        }

        /// <summary>
        /// Will write the execution plan to TestServices.Trace
        /// </summary>
        /// <param name="plan">The execution plan.</param>
        private static void LogExecutionPlan(ExecutionPlan plan)
        {
            TestServices.Trace("Execution Plan");
            foreach (Call c in plan.Calls)
            {
                TestServices.Trace(c.PreCallSummary());
            }
        }

        /// <summary>
        /// Will return all the dependencies (as defined by WaitFor) on a call.
        /// </summary>
        /// <remarks>
        /// Current implementation assumes a default plan where calls map 1:1 to
        /// methods marked as test steps.  This will need to change for XML
        /// plans.
        /// </remarks>
        /// <param name="attribute">The attribute for which we would like the
        /// dependent calls.</param>
        /// <param name="knownCalls">A dictionary of known calls with the name
        /// of the test step as the key.</param>
        /// <returns>A list of the dependent calls from the knownCalls
        /// parameter.</returns>
        private static Call[] GetDependencies(
            TestStepAttribute attribute, Dictionary<string, Call> knownCalls)
        {
            string waitForList = attribute.WaitFor;

            // PreCondition: if there is no list, return an empty array
            if (string.IsNullOrEmpty(waitForList))
            {
                return new Call[0];
            }

            // turn the comman seperated list into an array
            string[] callNames =
                waitForList.Split(
                    CultureInfo.InvariantCulture
                        .TextInfo.ListSeparator.ToCharArray(),
                    StringSplitOptions.RemoveEmptyEntries);

            // return array size matches the list
            Call[] dependencies = new Call[callNames.Length];

            // look up the dependent call and store it in the array
            for (int i = 0; i < callNames.Length; i++)
            {
                string callName = callNames[i].Trim();
                if (knownCalls.ContainsKey(callName))
                {
                    dependencies[i] = knownCalls[callName];
                }
                else
                {
                    // fail if there is an unknown dependency
                    TestServices.Assert(
                        false,
                        "WaitFor call named {0} not found.",
                        callName);
                }
            }

            return dependencies;
        }
        #endregion Private Methods

        #region Private Fields
        //----------------------------------------------------------------------
        // Private Fields
        //----------------------------------------------------------------------

        private static bool _hasRunOnce;

        #endregion Private Fields
    }

    /// <summary>
    /// Includes an ordered list of the calls to execute for the test and the
    /// test step information for them.
    /// </summary>
    internal struct ExecutionPlan
    {
        #region Public Fields
        //----------------------------------------------------------------------
        // Public Fields
        //----------------------------------------------------------------------

        /// <summary>
        /// TestStep information, primarily for logging / tracing
        /// </summary>
        public Dictionary<Call, TestStepAttribute> TestSteps;
        /// <summary>
        /// The ordered list of calls to execute.
        /// </summary>
        public List<Call> Calls;
        #endregion Public Fields
    }
}
