// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: 
 *      Driver for the FuzzTests.
 *      Parses command line arguments, creates and runs FuzzTest instances.
 * Contributor: 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Test.Logging;
using Avalon.Test.CoreUI.Trusted;

namespace Avalon.Test.CoreUI.Parser.Security
{
    /// <summary>
    /// Driver for the FuzzTests. Parses command line arguments, 
    /// creates and runs FuzzTest instances.
    /// </summary>
    class FuzzTestDriver
    {

        /// <summary>
        /// Reads the current test case parameters from the current AppDomain
        /// and puts them in a dictionary.
        /// </summary>
        /// <returns>A dictionary of test case params. Value is null if there is no test case info or params.</returns>
        public static IDictionary GetCurrentParams()
        {
            SortedList args = null;

            // If TestCaseInfo was available on the AppDomain,
            // read args and set traversal constraints.
            if ( !String.IsNullOrEmpty(DriverState.DriverParameters["Params"]))
            {
                args = Utility.ParseArgs(DriverState.DriverParameters["Params"].Split(' '), false, true, "-");
            }

            return args;
        }
        /// <summary>
        /// Test case entry point.
        /// </summary>
        public void RunTest()
        {


            IDictionary args = GetCurrentParams();

            if (args.Count < 1)
            {
                _Usage();
            }
            else
            {
                if (args["drt"] != null)
                    _Drt();
                else if(args["config"] != null)
                {
                    // Post callback to execute as a dispatcher operation.
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback(_RunAllTestsInDispatcher), args["config"]);

                    // Run the dispatcher.
                    Dispatcher.Run(); 
                }
            }
        }

        // Callback that executes as a DispatcherOperation.
        // This runs all the fuzz tests in a single Dispatcher.
        private object _RunAllTestsInDispatcher(object config)
        {
            List<FuzzTest> tests = ParserFuzzTest.LoadTests((string)config);
            for (int i = 0; i < tests.Count; i++)
            {
                FuzzTest test = tests[i];
                CoreLogger.LogStatus("\r\nStarting fuzz test " + i.ToString() + "...");
                test.Run();
            }

            Dispatcher.CurrentDispatcher.InvokeShutdown();

            return null;
        }

        private static void _Usage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("    -drt");
            Console.WriteLine("        Runs the regression test");
            Console.WriteLine();
            Console.WriteLine("    -config=TestDescription.xml");
            Console.WriteLine("        Runs the fuzz test described by 'TestDescription.xml'");
            Console.WriteLine();
        }

        private static void _Drt()
        {
            // Test basic functionality of Fuzzer against regressions I may cause

            ThreadContainer thread1 = new ThreadContainer(1);
            ThreadContainer thread2 = new ThreadContainer(2);
            ThreadContainer thread3 = new ThreadContainer(3);
            ThreadContainer thread4 = new ThreadContainer(4);
            ThreadContainer thread5 = new ThreadContainer(5);

            thread1.Start();
            thread2.Start();
            thread3.Start();
            thread4.Start();
            thread5.Start();

            thread1.Join();
            thread2.Join();
            thread3.Join();
            thread4.Join();
            thread5.Join();
        }

        private class ThreadContainer
        {
            public ThreadContainer(int n)
            {
                this._fuzzer = n;
                this._thread = new Thread(new ThreadStart(ThreadWorker));
                this._thread.SetApartmentState(ApartmentState.STA);
            }

            private int _fuzzer;
            private Thread _thread;

            public void Start()
            {
                _thread.Start();
            }

            public void Join()
            {
                _thread.Join();
            }
            
            private void ThreadWorker()
            {
                CoreLogger.LogStatus("Starting work on this thread");

                ParserFuzzTest test = new BamlFuzzTest();
                Random random = new Random();

                switch (this._fuzzer)
                {
                case 1:
                    ShuffleFuzzer fuzzer1 = new ShuffleFuzzer(random);
                    fuzzer1.NumShuffles = 2;
                    test.Fuzzers.Add(fuzzer1);
                    break;

                case 2:
                    ConnectionIdFuzzer fuzzer2 = new ConnectionIdFuzzer(random);
                    fuzzer2.AllowDuplicateIds = true;
                    test.Fuzzers.Add(fuzzer2);
                    break;

                case 3:
                    RandomByteFuzzer fuzzer3 = new RandomByteFuzzer(random);
                    fuzzer3.Frequency = 100;
                    fuzzer3.Variance = 50;
                    test.Fuzzers.Add(fuzzer3);
                    break;

                case 4:
                    ShuffleFuzzer fuzzer4 = new ShuffleFuzzer(random);
                    fuzzer4.NumShuffles = 10;

                    ConnectionIdFuzzer fuzzer5 = new ConnectionIdFuzzer(random);
                    fuzzer5.AllowDuplicateIds = false;

                    RandomByteFuzzer fuzzer6 = new RandomByteFuzzer(random);
                    fuzzer6.Frequency = 100;
                    fuzzer6.Variance = 50;

                    test.Fuzzers.Add(fuzzer4);
                    test.Fuzzers.Add(fuzzer5);
                    test.Fuzzers.Add(fuzzer6);
                    break;

                case 5:
                    InsertionFuzzer fuzzer7 = new InsertionFuzzer(random);
                    fuzzer7.Frequency = 4;
                    fuzzer7.Variance = 4;
                    fuzzer7.MaxBytesToInsert = 5;
                    test.Fuzzers.Add(fuzzer7);
                    break;
                }

                test.Run();
            }
        }
    }
}
