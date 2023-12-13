// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Test.Uis.TextEditing
{
    #region Namespaces.
    
    using System;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using System.Windows.Controls;
    using System.ComponentModel;
    using System.Windows.Media;
    using System.Windows;
    using Microsoft.Test.Imaging;
    using System.Drawing;
    using System.Windows.Documents;
    using System.Collections;
    
    #endregion Namespaces.

    /// <summary>
    /// Base class for forms editing test cases.
    /// </summary>
    [TestArgument("Case", "Name of entry method to invoke for case.")]
    public class CommonTestCase : Test.Uis.TestTypes.CustomTestCase
    {
        #region Protected fields.
        
        /// <summary>Logger used to report results.</summary>
        protected Logger MyLogger = Logger.Current;

        /// <summary>Whether the test case has succeeded.</summary>
        protected bool pass = true;

        /// <summary> </summary>
        protected string CurrentFunction;

        /// <summary>Counter for run more than one cases </summary>
        protected int CaseCounter = 0;

        /// <summary>CaseDelegate hold all cases delegates so that cases can be combined.</summary>
        protected ArrayList CaseDelegate;

        /// <summary>Array list to record the failed cases information </summary>
        protected ArrayList CaseStatus; 
        
        #endregion Protected fields.

        /// <summary>Initializes the test case.</summary>
        public new virtual void Init()
        {
            EnterFuction("Init");
            MyLogger.Log("You did not perfrom initialization for you test!!!");
            EndFunction();
        }

        /// <summary>
        /// Test entry function.
        /// </summary>
        public override void RunTestCase()
        {
            EnterFuction("RunTestCase");
            Init();
            QueueDelegate(InvokeCase);
            EndFunction();
        }

        /// <summary>Invokes the test case.</summary>
        public void InvokeCase()
        {
            string caseName;                        // Method name for case.
            CasePriority priority;                  // case priority
            EnterFuction("InvokeCase");
            CaseDelegate = new System.Collections.ArrayList();
            CaseStatus = new System.Collections.ArrayList();
            caseName = ConfigurationSettings.Current.GetArgument("Case");
            priority =(CasePriority)ConfigurationSettings.Current.GetArgumentAsInt("Priority") ;

            if (caseName == null || caseName == string.Empty || caseName == "RunAllCases")
            {
                System.Reflection.MethodInfo[] mInfos = this.GetType().GetMethods();
                foreach (System.Reflection.MethodInfo minfo in mInfos)
                {
                    if (minfo.Name.Contains("_CaseStart"))
                    {
                        CaseDelegate.Add(minfo);
                    }
                    else
                    {
                        object[] objs = minfo.GetCustomAttributes(typeof(TestCaseAttribute),false);
                        for (int i = 0; i < objs.Length; i++)
                        {
                            if (((TestCaseAttribute)objs[i]).TestCaseStatus == LocalCaseStatus.Ready && priority == ((TestCaseAttribute)objs[i]).TestCasePriority)
                            {
                                CaseDelegate.Add(minfo);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                System.Reflection.MethodInfo method = this.GetType().GetMethod(caseName);
                if(method!=null)
                    CaseDelegate.Add(method);
            }
            //Failed if no case found.
            Verifier.Verify(CaseDelegate.Count > 0, "Failed: No case found!!! Case Name: [" + caseName + "]");
            QueueDelegate(EndTest);
            EndFunction();
        }
      
        /// <summary> </summary>
        protected void EnterFuction(string str)
        {
            EnterFunction(str);
        }

        /// <summary> </summary>
        protected void EnterFunction(string str)
        {
            Verifier.Verify(str != null && str != string.Empty, "EnterFunction won't acept null or empty string!!!");
            CurrentFunction = str;
            MyLogger.Log("Enter function - " + this.GetType().FullName + "." + CurrentFunction + "()");
        }
        /// <summary>/// </summary>
        protected void EndFunction()
        {
            MyLogger.Log("End function - " + this.GetType().FullName +"." +  CurrentFunction + "()");
            CurrentFunction = null; 
        }

        /// <summary>EndTest will choose to report pass or run another cases</summary>
        protected void EndTest()
        {
            Sleep();
            string message = pass ? "Local case Passed: " : "Local case Failed: ";
            pass = true;
            if(CaseCounter>0)
                CaseStatus.Add(message + this.GetType().Name + "." + ((System.Reflection.MemberInfo)CaseDelegate[CaseCounter - 1]).Name);
            
            if (CaseCounter < CaseDelegate.Count)
            {
                string stars = RepeatString("*", 45);
                MyLogger.Log( "\r\n\r\n" + stars+ ((System.Reflection.MemberInfo)CaseDelegate[CaseCounter]).Name + stars);
                ((System.Reflection.MethodInfo)CaseDelegate[CaseCounter++]).Invoke(this, new object[] {});
            }
            else
            {
                if (CaseStatus.Count != 0)
                {
                    MyLogger.Log("Cases Passed list: ");
                    for (int i = 0; i < CaseStatus.Count; i++)
                    {
                        if (((string)CaseStatus[i]).Contains("Local case Passed"))
                            MyLogger.Log("             " + CaseStatus[i]);

                    }
                    MyLogger.Log("Cases Failed List: ");
                    for (int k = 0; k < CaseStatus.Count; k++)
                    {
                        if (((string)CaseStatus[k]).Contains("Local case Failed"))
                        {
                            MyLogger.Log("             " + CaseStatus[k]);
                            pass = false;
                        }
                    }
                }

                MyLogger.Quit(pass);
            }
        }

        /// <summary>Sleep for a certain time </summary>
        protected void Sleep()
        {
            Sleep(50);
        }
        /// <summary>
    /// Sleep
    /// </summary>
    /// <param name="minisecond"></param>
        protected void Sleep(int minisecond)
        {
            System.Threading.Thread.Sleep(minisecond);
        }

        /// <summary>
        /// Make repeated string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        protected string RepeatString(string str, int x)
        {
            string tempStr = string.Empty;

            while (x-- > 0)
                tempStr += str;

            return tempStr;
        }
        /// <summary>
        /// Find the freuqency of a string
        /// </summary>
        /// <param name="MainString"></param>
        /// <param name="SubString"></param>
        /// <returns></returns>
        protected int Occurency(string MainString, string SubString)
        {
            return MainString.Contains(SubString) ? Occurency(MainString.Substring(MainString.IndexOf(SubString) + SubString.Length), SubString) + 1 : 0;
        }
        
        /// <summary>
        /// Creates an instance of the specified element.
        /// </summary>
        static public FrameworkElement CreateElementWithSize(string name, int left, int top, int width, int height)
        {
            if (null == name || name == string.Empty)
            {
                throw new Exception("Can't create element without name!!!");
            }

            FrameworkElement FE = null;

            FE = ReflectionUtils.CreateInstanceOfType(name, null) as FrameworkElement;
            if (null == FE)
                throw new Exception("Not able to create FrameworkElement: " + name);

            FE.Width = width;
            FE.Height = height;
            Canvas.SetTop(FE, top);
            Canvas.SetLeft(FE, left);
            return FE;
        }
        /// <summary>
        /// FailedIf
        /// </summary>
        /// <param name="Failed"></param>
        /// <param name="message"></param>
        protected void FailedIf(bool Failed, string message)
        {
            if (Failed)
            {
                pass = false;
                MyLogger.Log(message);
            }
        }
    }
    /// <summary>
    /// the attribute for a test case. This is specified on the method.
    /// </summary>
    //[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class TestCaseAttribute : Attribute
    {
        private LocalCaseStatus _currentStatus;
        private string _caseName;
        private CasePriority _priority;

        /// <summary>
        /// constructor 
        /// </summary>
        /// <param name="status"></param>
        /// <param name="caseName"></param>
        public TestCaseAttribute(LocalCaseStatus status, string caseName):this(status, CasePriority.BVT, caseName)
        {
        }

		/// <summary>
		/// TestCaseAttribute constructor
		/// </summary>
		/// <param name="status"></param>
		/// <param name="priority"></param>
		/// <param name="caseName"></param>
        public TestCaseAttribute(LocalCaseStatus status, CasePriority priority, string caseName)
        {
            _currentStatus = status;
            _caseName = caseName;
            _priority = priority; 
        }

        /// <summary>
        /// return the name of a test case
        /// </summary>
        /// <value></value>
        public string TestCaseName
        {
            get
            {
                return _caseName;
            }
        }

        /// <summary>
        /// Return the status of a test case.
        /// </summary>
        /// <value></value>
        public LocalCaseStatus TestCaseStatus
        {
            get
            {
                return _currentStatus;
            }
        }

        /// <summary>
        /// return the priority of the case.
        /// </summary>
        /// <value></value>
        public CasePriority TestCasePriority
        {
            get
            {
                return _priority;
            }
        }
    }

    /// <summary>
    /// define the status of the case
    /// </summary>
    public enum LocalCaseStatus
    {
        /// <summary>
        /// test case is Ready
        /// </summary>
        Ready,
        /// <summary>
        /// Test case is broken. need to be fixed.
        /// </summary>
        Broken,
        /// <summary>
        /// Test case is still under development.
        /// </summary>
        UnderDevelopment,
    }

    /// <summary>
    /// Test Case priority
    /// </summary>
    public enum CasePriority
    {
        /// <summary>
        /// priority 0
        /// </summary>
        BVT, 
        /// <summary>
        /// priority 1
        /// </summary>
        p1,
        /// <summary>
        /// priority 2
        /// </summary>
        p2,
        /// <summary>
        /// priority 3 
        /// </summary>
        p3,
        /// <summary>
        /// priority 4
        /// </summary>
        p4, 
        /// <summary>
        /// priority 5
        /// </summary>
        p5
    }
}