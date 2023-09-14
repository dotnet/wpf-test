using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Threading;
using Avalon.Test.ComponentModel;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Microsoft.Test.Controls
{
    ////////////////////////////////////////////////////////////////////////////////////////////
    // DISABLEDUNSTABLETEST:
    // TestName: DataGridAutoColumnGenerationP1
    // Area: Controls�� SubArea: DataGrid
    // Disable this case due to high fail rate, will enable after fix it.
    // to find all disabled tests in test tree, use: �findstr /snip DISABLEDUNSTABLETEST� 
    ////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// <description>
    /// DataGrid auto-column generation tests.    
    /// </description>

    /// </summary>
    [Test(1, "DataGrid", "DataGridAutoColumnGenerationP1", SecurityLevel = TestCaseSecurityLevel.FullTrust, Timeout = 120, Disabled = true)]
    public class DataGridAutoColumnGenerationP1 : DataGridAutoColumnGenerationTest
    {
        #region Constructor

        public DataGridAutoColumnGenerationP1()
            : this(@"DataGridAutoGenOffUserSet.xaml")
        {
        }

        [Variation("DataGridAutoGenOn.xaml")]
        [Variation("DataGridAutoGenOff.xaml")]
        [Variation("DataGridAutoGenOnZeroCount.xaml")]
        [Variation("DataGridAutoGenOnNullSource.xaml")]
        [Variation("DataGridAutoGenOnUserSet.xaml")]
        [Variation("DataGridAutoGenOffUserSet.xaml")]
        [Variation("DataGridAutoGenOnZeroCountUserSet.xaml")]
        [Variation("DataGridAutoGenOnNullSourceUserSet.xaml")]
        public DataGridAutoColumnGenerationP1(string filename)
            : base(filename)
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestChangingSources);
            RunSteps += new TestStep(TestChangingSources2);
            RunSteps += new TestStep(TestChangesToColumns);
            RunSteps += new TestStep(TestChangesToColumns2);
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initial Setup  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public override TestResult Setup()
        {
            base.Setup();

            Status("Setup specific for DataGridAutoColumnGenerationP1");



            LogComment("Setup for DataGridAutoColumnGenerationP1 was successful");
            return TestResult.Pass;
        }        

        /// <summary>
        /// Action1: change the ItemsSource behavior, and verify
        /// Action2: change the AutoGenerateColumns property, and verify 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestChangingSources()
        {
            Status("TestChangingSources");

            try
            {
                foreach (DoItemsSourceBehavior ItemsSourceBehavior in doItemsSourceBehaviors)
                {
                    LogComment(string.Format("Begin ItemsSourceBehavior: {0}", ItemsSourceBehavior.Method.Name));

                    ItemsSourceBehavior();
                    VerifyOnItemsSourceChanged();

                    foreach (DoAutoGenerateColumnsBehavior AutoGenBehavior in doAutoGenerateColumnsBehaviors)
                    {
                        LogComment(string.Format("  Begin AutoGenBehavior: {0}", AutoGenBehavior.Method.Name));

                        // Capture the current state
                        StateProfile tempCurState = curState;

                        try
                        {
                            AutoGenBehavior();
                            VerifyOnAutoGenColumnsChanged();
                        }
                        finally
                        {
                            SetToInitState(tempCurState);
                        }

                        LogComment(string.Format("  End AutoGenBehavior: {0}", AutoGenBehavior.Method.Name));
                    }

                    LogComment(string.Format("End ItemsSourceBehavior: {0}", ItemsSourceBehavior.Method.Name));
                }
            }
            finally
            {
                SetToInitState(initialStateProfiles[xamlFilename]);                
            }

            LogComment("TestChangingSources was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Action1: change the ItemsSource behavior, and verify
        /// Action2: change the Columns behavior, and verify 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestChangingSources2()
        {
            Status("TestChangingSources");

            try
            {
                foreach (DoItemsSourceBehavior ItemsSourceBehavior in doItemsSourceBehaviors)
                {
                    LogComment(string.Format("Begin ItemsSourceBehavior: {0}", ItemsSourceBehavior.Method.Name));

                    ItemsSourceBehavior();
                    VerifyOnItemsSourceChanged();

                    foreach (DoColumnsBehavior DoColumnBehavior in doColumnsBehaviors)
                    {
                        LogComment(string.Format("  Begin DoColumnBehavior: {0}", DoColumnBehavior.Method.Name));

                        // Capture the current state
                        StateProfile tempCurState = curState;

                        try
                        {
                            DoColumnBehavior();
                            VerifyOnColumnsChanged();
                        }
                        finally
                        {
                            SetToInitState(tempCurState);
                        }

                        LogComment(string.Format("  End DoColumnBehavior: {0}", DoColumnBehavior.Method.Name));
                    }

                    LogComment(string.Format("End ItemsSourceBehavior: {0}", ItemsSourceBehavior.Method.Name));
                }
            }
            finally
            {
                SetToInitState(initialStateProfiles[xamlFilename]);   
            }

            LogComment("TestChangingSources was successful");
            return TestResult.Pass;
        }

        /// <summary> 
        /// Action1: change the Columns behavior, and verify 
        /// Action2: change the ItemsSource behavior, and verify
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestChangesToColumns()
        {
            Status("TestChangesToColumns");

            try
            {
                foreach (DoColumnsBehavior DoColumnBehavior in doColumnsBehaviors)
                {
                    LogComment(string.Format("Begin DoColumnBehavior: {0}", DoColumnBehavior.Method.Name));

                    DoColumnBehavior();
                    VerifyOnColumnsChanged();

                    foreach (DoItemsSourceBehavior ItemsSourceBehavior in doItemsSourceBehaviors)
                    {
                        LogComment(string.Format("  Begin ItemsSourceBehavior: {0}", ItemsSourceBehavior.Method.Name));

                        // Capture the current state
                        StateProfile tempCurState = curState;

                        try
                        {
                            ItemsSourceBehavior();
                            VerifyOnItemsSourceChanged();
                        }
                        finally
                        {
                            SetToInitState(tempCurState);
                        }                        

                        LogComment(string.Format("  End ItemsSourceBehavior: {0}", ItemsSourceBehavior.Method.Name));
                    }

                    LogComment(string.Format("End DoColumnBehavior: {0}", DoColumnBehavior.Method.Name));
                }
            }
            finally
            {
                SetToInitState(initialStateProfiles[xamlFilename]);   
            }

            LogComment("TestChangesToColumns was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Action1: change the Columns behavior, and verify 
        /// Action2: change the AutoGenerateColumns property, and verify 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestChangesToColumns2()
        {
            Status("TestChangesToColumns");

            try
            {
                foreach (DoColumnsBehavior DoColumnBehavior in doColumnsBehaviors)
                {
                    LogComment(string.Format("Begin DoColumnBehavior: {0}", DoColumnBehavior.Method.Name));

                    DoColumnBehavior();
                    VerifyOnColumnsChanged();

                    foreach (DoAutoGenerateColumnsBehavior AutoGenBehavior in doAutoGenerateColumnsBehaviors)
                    {
                        LogComment(string.Format("  Begin AutoGenBehavior: {0}", AutoGenBehavior.Method.Name));

                        // Capture the current state
                        StateProfile tempCurState = curState;

                        try
                        {
                            AutoGenBehavior();
                            VerifyOnAutoGenColumnsChanged();
                        }
                        finally
                        {
                            SetToInitState(tempCurState);
                        }

                        LogComment(string.Format("  End AutoGenBehavior: {0}", AutoGenBehavior.Method.Name));
                    }

                    LogComment(string.Format("End DoColumnBehavior: {0}", DoColumnBehavior.Method.Name));
                }
            }
            finally
            {
                SetToInitState(initialStateProfiles[xamlFilename]);   
            }

            LogComment("TestChangesToColumns was successful");
            return TestResult.Pass;
        }
        
        #endregion Test Steps        
    }
}
