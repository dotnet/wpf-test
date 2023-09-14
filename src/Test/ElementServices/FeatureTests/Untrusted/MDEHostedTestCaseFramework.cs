// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Test Case for DispatcherTimer
 *
 
  
 * Revision:         $Revision: 2 $
 
********************************************************************/
using System;
using System.Windows;
using Avalon.Test.CoreUI;
using System.Collections;
using System.Windows.Media;
using Microsoft.Test.Logging;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Threading;

namespace Avalon.Test.CoreUI
{


    /// <summary>
    /// Represents the current ITestContainer for this IHostedTest.
    /// </summary>    
    public abstract class MDEHostedTestCaseFramework : IHostedTest
    {
        /// <summary>
        /// Represents the current ITestContainer for this IHostedTest.
        /// </summary>
        public MDEHostedTestCaseFramework()
        {
        }

        /// <summary>
        /// </summary>
        public static void Run(IHostedTest testCase, CoreModelState testState, string[] contents, string[] xamlFiles)
        {
            if (testState.CompiledVersion)
            {                
                GenericCompileHostedCase.RunCase(testCase,"HostedTestEntryPoint",
                    testState.Source, 
                    null,
                    contents,
                    xamlFiles);
            }
            else
            {
                ExeStubContainerFramework exe = new ExeStubContainerFramework(testState.Source);
                exe.Run(testCase,"HostedTestEntryPoint");
            }
        }

        /// <summary>
        /// Represents the current ITestContainer for this IHostedTest.
        /// </summary>
        public ITestContainer TestContainer
        {
            get
            {
                return _iTestContainer;
            }
            set
            {
                _iTestContainer = value;
            }
        } 

        TestLog _testLog = null;

        /// <summary>
        /// </summary>
        public void HostedTestEntryPoint()
        {

            if (TestLog.Current == null)
                _testLog = new TestLog("1");
            
            MouseHelper.MoveOnVirtualScreenMonitor();

            
            TestContainer.ExceptionThrown += new EventHandler(ExceptionBeenThrown);

            // Loading the test case state or config            
            ModelState = CoreModelState.Load();  

            HostedTestEntryPointCore();
               
            bool dispatcherRan = TestContainer.RequestStartDispatcher();

            if (dispatcherRan)
            {
                RequestedDispatcherExit();
            }
            
        }


        /// <summary>
        /// </summary>
        protected virtual void RequestedDispatcherExit()
        {

        }

        /// <summary>
        /// </summary>
        protected object XamlRootObject = null;

        /// <summary>
        /// </summary>
        protected Visual RealRootVisual
        {
            get
            {
                if (XamlRootObject == null || !(XamlRootObject is Visual))
                {
                    return null;
                }

                Visual visual = (Visual)XamlRootObject ;

                while(VisualTreeHelper.GetParent(visual) != null)
                {
                    visual = (Visual)VisualTreeHelper.GetParent(visual);
                }
                
                return visual;
            }
        }



        /// <summary>
        /// </summary>
        protected CoreModelState ModelState;

        /// <summary>
        /// </summary>
        protected abstract void HostedTestEntryPointCore();


        /// <summary>
        /// </summary>
        protected void LogTest(bool v, string reason)
        {
            if (!v)
            {
                _isTestPassed = false;
            }
            
            CoreLogger.LogTestResult(v, reason);
        }

        /// <summary>
        /// </summary>
        protected void Log(string str)
        {
            CoreLogger.LogStatus(str);
        }

        /// <summary>
        /// </summary>
        protected bool IsTestPassed
        {
            get
            {
                return _isTestPassed;
                
            }
            set
            {
                
                _isTestPassed = value;
            }
        }


        /// <summary>
        /// </summary>
        private void ExceptionBeenThrown(object o, EventArgs args)
        {
            LogTest(false, "An exception has been caught");
        }

        private ITestContainer _iTestContainer = null;
        private bool _isTestPassed = true;

   }

}



