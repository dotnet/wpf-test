// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Holds common reusable entry points for model-based tests.
 * Contributor: Microsoft
 *
 
  
 * Revision:         $Revision: 6 $
 
 * Filename:         $Source: //depot/vbl_wcp_avalon_dev/windowstest/client/wcptests/Core/common/CoreTestsTrusted/XamlTestCaseInfo.cs $
********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml;

using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Modeling;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;


namespace Avalon.Test.CoreUI
{
    /// <summary>
    /// Holds common reusable entry points for model-based tests.
    /// </summary>
    public class ModelingCommon : IHostedTest
    {
        ///// <summary>
        ///// This entry point is for all cases that use the MultipleThreadTestCaseModelAttribute
        ///// </summary>        
        //public void MultipleThreadGenericMdeEntryPoint()
        //{
        //    MultipleThreadModelTestCaseInfo testCase = (MultipleThreadModelTestCaseInfo)TestCaseInfo.GetCurrentInfo();

        //    CoreLogger.LogThreadId = true;

        //    string[] testCaseArray = testCase.ModelTestCases.Split(new Char[] {','});

        //    if (testCaseArray == null || testCaseArray.Length < 2)
        //    {
        //        throw new ArgumentException("The ModeTestCases parameters is null or has less than 2 items.");               
        //    }

        //    ManualResetEvent ev = new ManualResetEvent(false);
        //    ManualResetEvent evFinal = new ManualResetEvent(false);
        //    for(int i = 0; i < testCaseArray.Length; i++)
        //    {
        //        _totalThreads++;
        //        Thread t = new Thread(new ParameterizedThreadStart(WorkerThreadHandler));
        //        t.Start(new object[] {ev, int.Parse(testCaseArray[i]), testCase, evFinal});                
        //    }
            
        //    Thread.Sleep(0);
        //    ev.Set();

        //    evFinal.WaitOne();
           
        //}



        private int _totalThreads = 0;
        private int _completedThreads = 0;

        void Unlock(ManualResetEvent ev)
        {
            int completedThreads = Interlocked.Increment(ref _completedThreads);
            if (completedThreads >= _totalThreads)
            {
                ev.Set();
            }
        }


        //void WorkerThreadHandler(object o)
        //{
        //    object[] oArray = (object[])o;

        //    ManualResetEvent ev = (ManualResetEvent)oArray[0];
        //    ManualResetEvent evFinal = (ManualResetEvent)oArray[3];
        //    ev.WaitOne();

        //    int testCaseNumber = (int)oArray[1];

        //    MultipleThreadModelTestCaseInfo testCase = (MultipleThreadModelTestCaseInfo)oArray[2];


        //    try
        //    {                
        //        GenericMdeEntryPointPrivate(testCase, testCaseNumber, testCaseNumber);
        //    }
        //    catch(Exception e)
        //    {
        //        CoreLogger.LogTestResult(false,"Exception caught");
        //        CoreLogger.LogStatus(e.Message);
        //        CoreLogger.LogStatus(e.StackTrace.ToString());                
        //    }
        //    finally
        //    {
        //        Unlock(evFinal);
        //    }
        //}


        /// <summary>
        /// This entry point is for all cases that use the TestCaseModelAttribute.
        /// </summary>        
        public void GenericMdeEntryPoint()
        {
            //RangeModelTestCaseInfo testCase = (RangeModelTestCaseInfo)TestCaseInfo.GetCurrentInfo();
            ModelExecutionData testCase = new ModelExecutionData(DriverState.DriverParameters);

            int modelStart = testCase.ModelStart;
            int modelEnd = testCase.ModelEnd;

            GenericMdeEntryPointPrivate(testCase, modelStart, modelEnd);
        }

        
        /// <summary>
        /// 
        /// </summary>        
        public void GenericMdeEntryPointPrivate(ModelExecutionData testCase, int modelStart, int modelEnd)
        {           
            if (testCase == null)
            {
                throw new Microsoft.Test.TestSetupException("Could not get model execution data.");
            }

            Type classType = null;

            // 
            if (testCase.ModelClass.IndexOf('.') != -1)
            {                          
                classType = Utility.FindType(testCase.ModelClass, true);
            }
            else
            {
                classType = Utility.FindType(testCase.ModelClass, false);
            }

            Type type = classType;

            if (type == null)
            {
                throw new Microsoft.Test.TestSetupException("Could not get type '" + testCase.ModelClass + "' from default assemblies.");
            }

            object obj = Activator.CreateInstance(type);

            SetTestContainer(obj);

            int firstTraversal = modelStart;
            int lastTraversal = modelEnd;
            //string xtcFile = /*Path.Combine(
            string xtcFile = testCase.XtcFileName.Substring(testCase.XtcFileName.LastIndexOfAny(new Char[] { '/', '\\' }) + 1);//);
            
            // Load and run the model traversals.
            XtcTestCaseLoader loader = null;
            if(firstTraversal >= 0 && lastTraversal >=0)
            {
                loader = new XtcTestCaseLoader(xtcFile, firstTraversal, lastTraversal);
            }
            else
            {
                loader = new XtcTestCaseLoader(xtcFile);
            }

            // Add model to xtc loader.
            loader.AddModel((Model)obj);
            
            if (firstTraversal == lastTraversal)
            {
                loader.ShouldCreateTestLogs = false;
            }

            bool allPassed = loader.Run();

            // Log the overall test as failed if any model traversals failed.
            if (!allPassed)
            {
                throw new TestValidationException("Model cases failed: " + loader.TotalFailures.Count);                
            }
            else
            {
                CoreLogger.LogStatus("SUCCESS: All Model cases passed.");                
            }
        }

        /// <summary>
        /// Common entry point for hosted model-based tests, i.e. ones that use TestCaseHostedModelAttribute.
        /// Hosted tests implement IHostedTest, and are "hosted" by some TestContainer. 
        /// The XTC file is ----ed open, and each TEST node is passed to the RunVariation method
        /// of the ModelClassAssemblyName, which is specified in the TestCaseInfo.
        /// </summary>
        public void GenericHostedModelEntryPoint()
        {
            // HostedModelTestCaseInfo testCase = (HostedModelTestCaseInfo)TestCaseInfo.GetCurrentInfo();
            ModelExecutionData testCase = new ModelExecutionData(DriverState.DriverParameters);

            if (testCase == null)
            {
                throw new Microsoft.Test.TestSetupException("Could not get TestCaseInfo.");
            }

            _xmlDoc = new XmlDocument();
            _xmlDoc.Load(Path.Combine(/**/Environment.CurrentDirectory,testCase.XtcFileName));

            bool isActionSequenceModel = false; 

            if (_xmlDoc.DocumentElement.NamespaceURI.IndexOf("ActionSequence") != -1)
            {
                CoreLogger.LogStatus("Detecting ActionSequence xtc type.");
                isActionSequenceModel = true;
            }

            int modelStart = testCase.ModelStart;
            int modelEnd = testCase.ModelEnd;

            if (isActionSequenceModel)
            {
                CoreLogger.LogStatus("Running ActionSequence test case.");

                ActionSequenceTestEngineEntryPoint(testCase, modelStart, modelEnd);
            }
            else
            {
                CoreLogger.LogStatus("Running MDE test case.");

                GenericMdeEntryPointPrivate(testCase,modelStart, modelEnd);
            }
        }


        XmlDocument _xmlDoc = null;
        

        private void  SetTestContainer(object o)
        {
            if (o is IHostedTest)
            {
                // Forward the TestContainer reference to the test instance.
                CoreLogger.LogStatus("Setting TestContainer on nested IHostedTest...");
                ((IHostedTest)o).TestContainer = this.TestContainer;
            }

        }

        private void ActionSequenceTestEngineEntryPoint(ModelExecutionData testCase, int modelStart, int modelEnd)
        {

            //
            // Load test instance, and get entry point.
            //

            // Load Assembly.
            string testAssemblyName = "ElementServicesTest";

            if (!Path.HasExtension(testAssemblyName))
            {
                testAssemblyName += ".dll";
            }

            // Load object.
            Type type = Utility.FindType("Avalon.Test.CoreUI.ActionSequenceTestEngine", Path.GetFileNameWithoutExtension(testAssemblyName), true);

            object obj = Activator.CreateInstance(type);

            if (!(obj is IHostedTest))
            {
                throw new Microsoft.Test.TestSetupException("The type '" + testCase.ModelClass + "' doesn't implement IHostedTest.");
            }

            SetTestContainer(obj);

            // Get method.
            MethodInfo methodInfo = type.GetMethod("RunVariation", new Type[] { typeof(XmlNode), typeof(string) });

            if (methodInfo == null)
            {
                throw new InvalidOperationException("Could not find entry point 'RunVariation' on object '" + type.Name + "'.");
            }

            //
            // Run variations from ModelStart to ModelEnd.
            //

            int totalFailures = 0;
            
            
            // Construct the XmlNamespaceManager used for xpath queries later.
            NameTable ntable = new NameTable();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(ntable);
            nsmgr.AddNamespace("x", _xmlDoc.DocumentElement.NamespaceURI);

            // Query for all TEST nodes.
            XmlNodeList testNodes = _xmlDoc.SelectNodes("//x:TEST", nsmgr);

            if(modelStart > testNodes.Count)
            {
                throw new InvalidOperationException("The specified start index '" + modelStart.ToString() + "' is greater than the model test count '" + testNodes.Count + "'.");
            }
            
            if(modelEnd > testNodes.Count)
            {
                throw new InvalidOperationException("The specified end index '" + modelStart.ToString() + "' is greater than the model test count '" + testNodes.Count + "'.");
            }

            for(int i=modelStart; i <= modelEnd; i++)
            {
                TestLog testLog = new TestLog("Variation " + i.ToString());
                CoreLogger.LogStatus("* Start model case #" + i.ToString());

                XmlNode testNode = testNodes[i - 1];

                try
                {
                    methodInfo.Invoke(obj, new object[] { testNode, _xmlDoc.DocumentElement.NamespaceURI });
 
                    testLog.Result = TestResult.Pass;
                }
                catch (Exception e)
                {
                    CoreLogger.LogStatus("Exception occurred");
                    GlobalLog.LogEvidence(e.ToString());
                    testLog.Result = TestResult.Fail;
                }
                finally
                {
                    if(testLog.Result == TestResult.Fail)
                        totalFailures++;

                    CoreLogger.LogStatus("* End model case: #" + i.ToString());
                }

                testLog.Close();
            }
            

            // Log the overall test as failed if any model traversals failed.
            if (totalFailures > 0)
            {
                CoreLogger.LogTestResult(false, "Test variations failed: " + totalFailures);
            }
        }

        /// <summary>
        /// Container for the generic hosted model entry point. Value is null for MDE-based tests (GenericMdeEntryPoint).
        /// </summary>
        public ITestContainer TestContainer
        {
            get
            {
                return _testContainer;
            }
            set
            {
                _testContainer = value;
            }
        }

        ITestContainer _testContainer = null;

        #region Private Classes

        /// <summary>
        /// 
        /// </summary>
        public class ModelExecutionData
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="test"></param>
            public ModelExecutionData(ContentPropertyBag test)
            {
                _test = test;
            }

            /// <summary>
            /// 
            /// </summary>
            public int ModelStart
            {
                get { 
                    if (!String.IsNullOrEmpty(_test["TIndex"]))
                    {
                        return Int32.Parse(_test["TIndex"]);
                    }

                    return Int32.Parse(_test["ModelStart"]);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public int ModelEnd
            {
                get 
                { 
                    if (!String.IsNullOrEmpty(_test["TIndex"]))
                    {
                        return Int32.Parse(_test["TIndex"]);
                    }
                    
                    return Int32.Parse(_test["ModelEnd"]);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public int TestIndex
            {
                get 
                {
                    return Int32.Parse(_test["TIndex"]);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public string ModelClass
            {
                get
                {
                    return _test["ModelClass"];
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public string ModelAssembly
            {
                get
                {
                    return _test["ModelAssembly"];
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public string XtcFileName
            {
                get
                {
                    return _test["XtcFileName"];
                }
            }

            private ContentPropertyBag _test;
        }

        #endregion 
    }

    /// <summary>
    /// All element services models should inherit from this class which redirects
    /// the test case method to a common entry point.
    /// </summary>
    [TestDefaults(DefaultMethodName = "CommonModelingEntryPoint")]
    public class CoreModel : Model
    {
        /// <summary>
        /// Common modeling entry point.
        /// </summary>
        public void CommonModelingEntryPoint()
        {
           

            //OnBeginCase += new StateEventHandler(CoreModel_OnBeginCase);
            //OnEndCase += new StateEventHandler(CoreModel_OnEndCase);

            CoreLogger.BeginVariation("Variation: " + StateVariables.ToCommaSeparatedList());
            ModelingCommon mc = new ModelingCommon();
            mc.GenericMdeEntryPoint();
            CoreLogger.EndVariation();
        }

        void CoreModel_OnBeginCase(object sender, StateEventArgs e)
        {
            CoreLogger.BeginVariation(StateVariables.ToCommaSeparatedList());
        }

        void CoreModel_OnEndCase(object sender, StateEventArgs e)
        {
            CoreLogger.EndVariation();
        }                
    }
}

