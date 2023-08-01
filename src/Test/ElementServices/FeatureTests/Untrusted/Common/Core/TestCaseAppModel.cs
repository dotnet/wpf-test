// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Windows;
using System.Collections;
using System.Threading; 
using System.Windows.Threading;
using System.Windows.Interop;
using System.Collections.Generic;

using Avalon.Test.CoreUI.Common;
using Microsoft.Test.Win32;
using Microsoft.Test;
using Microsoft.Test.Discovery;

namespace Avalon.Test.CoreUI
{
    /// <summary>
    /// Type of the test case support on the TestCase class constructor
    /// </summary>  
    /// <remarks>
    /// Type of cases:
    /// <ul>
    /// <li> None </li>
    /// <li>ContextSupport</li>
    /// </ul>
    /// <para/>
    /// ContextSupport: It will create a default context.
    /// This is supportted because it can provide different hosting scenarios with a lot abstraction
    /// </remarks>
    public enum TestCaseType 
    {
        /// <summary>
        /// 

        None,
        
        /// <summary>
        /// 

        ContextSupport,

        /// <summary>
        /// 
        /// </summary>
        ContextEnteringSupport,

        /// <summary>
        /// This is Context Entering + Creating a HwndSource
        /// </summary>
        HwndSourceSupport


    }

    /// <summary>
    /// This is the abstract base class for Threading and Eventing test cases.
    /// </summary>
    /// <remarks>
    /// This class provide a several ways to create test cases on a Common way
    /// <para/>
    ///     
    /// /////

    [TestDefaults(DefaultMethodName="RunTest")]
    abstract public class TestCase :  IDisposable
    {
        /// <summary>
        /// Static constructor that creates the Array of Contexts and Dispatchers
        /// </summary>
        static TestCase()
        {
            ExceptionList = new List<Exception>();
        }

        /// <summary>
        /// Default constructor takes None as TestCaseType
        /// </summary>
        public TestCase()
            : this(TestCaseType.None)
        {
        }

        /// <summary>
        /// Constructor Class
        /// </summary>
        /// <remarks>
        /// This class provide a several ways to create test cases on a Common way
        ///     <para/>
        /// </remarks>
        public TestCase(TestCaseType type)
        {

            _testType = type;


            MainDispatcher = Dispatcher.CurrentDispatcher;
            

            if ( type==TestCaseType.HwndSourceSupport)
            {
                Source = SourceHelper.CreateHwndSource(600, 600, 25, 25);
                Source.AddHook(new HwndSourceHook(Helper));
            }
        
        }

        /// <summary>..
        /// 
        /// </summary>
        protected event EventHandler OnDestroyMainWindowEvent;
        
        /// <summary>
        /// 
        /// </summary>
        static protected List<Exception> ExceptionList = null;
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        /// <param name="message"></param>
        /// <param name="firstParam"></param>
        /// <param name="secondParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        protected IntPtr Helper(IntPtr window, int message, IntPtr firstParam,
            IntPtr secondParam, ref bool handled)
        {
            
            if(message == NativeConstants.WM_DESTROY && window == Source.Handle)
            {
                if (OnDestroyMainWindowEvent != null)
                {
                    OnDestroyMainWindowEvent(window,new EventArgs());
                }
            }

            handled = false;
            return IntPtr.Zero;
        }

        /// <summary>
        /// 
        /// </summary>
        protected HwndSource Source = null;



        /// <summary>
        ///     Return the type of the test case
        /// </summary>
        public TestCaseType TestType 
        {

            get
            {
                return _testType;
            }
        }


        #region IDisposable Members

        /// <summary>
        ///     Free all resources on the test case so there may not be leak problems
        /// </summary>
        public virtual void Dispose()
        {

            if (!_disposed)
            {
                 _disposed = true;
            }
        }
        #endregion



        /// <summary>
        ///     virtual method
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        public virtual void Run()
        {
        }



        /// <summary>
        ///  Entry point method for the test case
        /// </summary>
        public void RunTest()
        {
            Exception Excep = null;
            try
            {
                Run();
            }
            catch(Exception e)
            {
                Excep = e;
            }
            finally
            {
                if (Excep!=null)
                {
                    CoreLogger.LogStatus("**** An exception has occurred, calling Dispose before logging exception ****", ConsoleColor.Red);
                }
                
                Dispose();            
                
            }

            if (Excep!=null) throw Excep;
        }


        /// <summary>
        ///  
        /// </summary>
        protected void FinalReportFailure()
        {
            
            
            if (ExceptionList.Count > 0)
            {
                for (int i=0;i<ExceptionList.Count;i++)
                {
                    Exception e = ExceptionList[i];
                    CoreLogger.LogStatus("Exception #" + i.ToString());
                    CoreLogger.LogStatus(e.Message.ToString());
                    if (e.StackTrace != null)
                        CoreLogger.LogStatus(e.StackTrace.ToString());
                }

                this.TestCaseFailed = true;
                throw ExceptionList[0];
            }

            if (IsTestCaseFail)
                CoreLogger.LogStatus("Test case fail but there is no more information.");


            

        }


        /// <summary>
        ///  
        /// </summary>
        protected virtual bool IsTestCaseFail
        {
            get
            {
                return TestCaseFailed;
            }
               
        }

        /// <summary>
        ///  
        /// </summary>
        protected bool TestCaseFailed = false;



        /// <summary>
        ///  
        /// </summary>
        protected void ExitDispatcheronTimeout(TimeSpan span, bool failure)
        {
            DispatcherTimer dTimer = new DispatcherTimer(DispatcherPriority.Background);
            dTimer.Interval = span;
            dTimer.Tag = failure;
            dTimer.Tick += new EventHandler(forceDispatcherExit);
            dTimer.Start();

        }


        void forceDispatcherExit(object o, EventArgs args)
        {
            DispatcherTimer dTimer = (DispatcherTimer)o;
            dTimer.Stop();
            if ( (bool)dTimer.Tag == true)
                ExceptionList.Add(new Microsoft.Test.TestValidationException("The test case was exit in timeout"));
            Microsoft.Test.Threading.DispatcherHelper.ShutDown();
        }
        

        /// <summary>
        ///     This static field is support when the test cases is type ContextSupport
        /// </summary>
        public static Dispatcher MainDispatcher;

        /// <summary>
        /// Holds the reference for the type of the test case
        /// </summary>
        TestCaseType _testType;


        /// <summary>
        /// Holds the reference if the the test case is disposed
        /// </summary>
        bool _disposed = false;
    }

}

