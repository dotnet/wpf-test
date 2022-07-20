// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Reflection;

namespace DRT
{
    public sealed class DrtInkTestSuite : DrtTestSuite
    {
        public System.Collections.Hashtable Options;
        public DrtInkTestSuite() : base("Ink Tests")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            // initialize the suite here.  This includes loading the tree.

            // return the lists of tests to run against the tree
            return new DrtTest[]{ new DrtTest( TestInk ) };
        }

        // Testing an action that Avalon reacts to asynchronously:
        private void TestInk()
        {
            bool caseFound = false;
            foreach (Type type in TestCases)
            {
                    // run only specific test
                if (Options.Contains("case"))
                {
                    if (0 != type.ToString().Substring(type.Namespace.Length + 1).ToLower().CompareTo(((string)Options["case"]).ToLower()))
                    {
                        continue;
                    }
                    else
                    {
                        caseFound = true;
                    }
                }
                else
                {
                    caseFound = true;
                }

                    // dump test names
                if (Options.Contains("dump"))
                {
                    Console.WriteLine(type.Name);
                    continue;
                }

                    // create the test object
                ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                DrtInkTestcase myTestCase = (DrtInkTestcase)constructor.Invoke(null);

                    // write the sub-test name
                DRT.EnableTracing();
                string testExec = "   Executing Test: " + type.ToString();
                DRT.Trace(testExec);
                System.Diagnostics.Trace.WriteLine(testExec);
                DRT.DisableTracing();

                    // store any global test options
                FieldInfo field = type.GetField("Options");
                field.SetValue(myTestCase, Options);

                // enable DRT tracing
                field = type.GetField("DRT");
                field.SetValue(myTestCase, DRT);

                MethodInfo method = type.GetMethod("Run");

                string errorString = type.ToString();
                    // run the test
                try
                {
                    method.Invoke(myTestCase, null);
                }
                    // catch exceptions triggered by the test case
                catch (System.Reflection.TargetInvocationException e)
                {
                    errorString += Environment.NewLine + e.InnerException.ToString();
                }

                field = type.GetField("Success");
                bool success = (bool)field.GetValue(myTestCase);
                DRT.Assert(success, errorString);
                if (success)
                {
                    System.Diagnostics.Trace.WriteLine(testExec + " SUCCEEDED");
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine(testExec + " FAILED");
                }

                    // break on first test failure
                if (!success && Options.Contains("failfast"))
                {
                    return;
                }

                // check that Avalon reacted correctly.  Assert if it didn't:
                //DRT.Assert(condition, "message");
                //DRT.AssertEqual(expected, actual, "message");
            }
            if (!caseFound)
            {
                throw new InvalidOperationException(Options["case"].ToString() + " Testcase was not found!");
            }
        }
        // test cases 
        static Type[] testCases = null;
        public static Type[] TestCases
        {
            get
            {
                if (testCases != null)
                {
                    return testCases;
                }

                // best guess at number of tests is 20 (minimum)
                System.Collections.ArrayList cases = new System.Collections.ArrayList(20);

                foreach (Type t in typeof(DrtInkTestSuite).Assembly.GetTypes())
                {
                    if (t.IsSubclassOf(typeof(DrtInkTestcase)) && (t.Attributes & System.Reflection.TypeAttributes.Abstract) == 0)
                    {
                        cases.Add(t);
                    }
                }

                testCases = (Type[])cases.ToArray(typeof(Type));
                return testCases;
            }
        }
    }
}
