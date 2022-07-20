// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Driver class for selecting between and running N TestSuites.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using Microsoft.Test;
using Microsoft.Test.Annotations;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;

namespace Annotations.Test.Framework
{
    //public class Program
    //{
    //    [STAThread]
    //    public static void Main(string[] args)
    //    {
    //        new TestSuiteDriver().Run(args);
    //    }
    //}

    /// <summary>
    /// Class that contains a collection of TestSuites and knows how to run them.
    /// </summary>
    public class TestSuiteDriver : Application
    {
        #region Private Data

        string[] args;
        Assembly targetAssembly = null;

        #endregion

        #region Constructor

        public TestSuiteDriver()
        {
            // Application can only be terminated by explicit call to shutdown.
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        }

        #endregion

        #region Public Methods

        public void Run(string[] args)
        {
            this.args = (args == null) ? new string[0] : args;
            base.Run(); // Application.Run();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // If first arg is '/?' then print general driver usage.
            if (args.Length > 0 && args[0].Equals("/?"))
            {
                AnnotationsTestSettings.PrintUsage();
                return;
            }

            try
            {
                
                LogManager.BeginTest(DriverState.TestName);
                RunNextTestDefinition();
            }
            catch (Exception exception)
            {
                // Driver should always handle its exceptions because it is vastly 
                // more performent then making a JitDebugger do it.
                GlobalLog.LogEvidence("Driver Error: " + exception.ToString());

                ShutdownDriver();
            }
        }

        private void RunNextTestDefinition()
        {
            try
            {
                AnnotationsTestSettings testSettings = new AnnotationsTestSettings();

                if (targetAssembly == null)
                {
                    targetAssembly = Assembly.Load(testSettings.TargetAssemblyName);
                }
                else if (!targetAssembly.GetName().Name.Equals(testSettings.TargetAssemblyName))
                {
                    throw new NotSupportedException("Currently don't support running tests from different assemblies in the same ExecutionGroup.");
                }

                TestSuite targetSuite = null;
                Type suiteType = targetAssembly.GetType(testSettings.SuiteName);
                if (suiteType == null)
                    throw new ArgumentException(string.Format("Assembly '{0}' doesn't contain class '{1}'.", targetAssembly.FullName, testSettings.SuiteName));

                // Create instance of suite and add it to cache.
                targetSuite = (TestSuite)Activator.CreateInstance(suiteType);

                if (testSettings.Usage)
                {
                    TestSuite.PrintUsage(targetSuite);
                }
                else
                {
                    Run(targetSuite, testSettings.TestId, testSettings.CommandLine.Split(' '));
                }
            }
            catch (Exception e)
            {
                GlobalLog.LogEvidence(e);
                ShutdownDriver();
            }
        }

        private void Run(TestSuite suiteToRun, string testId, string[] args)
        {
            try
            {
                suiteToRun.Run(testId, args);
                // Note: this call may return before the test is actually done,
                // so you have to wait for the Finished event.
            }
            finally
            {
                // If error or we are done, shutdown.
                if (suiteToRun == null || suiteToRun.IsFinished)
                    OnFinished(null, null);
                else
                    suiteToRun.Finished += OnFinished;
            }
        }

        private void ShutdownDriver()
        {
            LogManager.EndTest();
            // Application Won't terminate until we tell it to.
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Will get signaled when
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFinished(object sender, EventArgs e)
        {
            ShutdownDriver();
        }

        #endregion
    }
}
