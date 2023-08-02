using System;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Collections;

using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Controls.Primitives;
using Avalon.Test.ComponentModel;
using Microsoft.Test.Logging;

namespace Avalon.Test.ComponentModel.UnitTests
{
    /// <summary>
    /// Test for ArgumentNullException in Set/Get methods
    ///  ArgumentNullExceptionTest.cs
    /// 
    ///  Test Set/Get static methods on Controls to make sure ArgumentNullException is thrown
    ///  when passed in DependencyObject is null.
    ///  
    ///  In Xtc file if a "Control" and "ControlBase" (base class of Control) specified then this test will 
    /// look for Set/Get defined upto the "ControlBase" otherwise it will look for those only in Control
    ///  
    /// Eg.<ExceptionTest Control="Control"/>, <ExceptionTest Control="ListBox" ControlBase="Selector"/>  
    /// </summary>
    public class ArgumentNullExceptionTest : IUnitTest
    {
        public TestResult Perform(object obj, XmlElement variation)
        {
            string controlName = (variation.ChildNodes[0]).Attributes["Control"].Value as string;

            Type controlType = GetControlType(controlName);

            string controlBaseName = null;
            if ((variation.ChildNodes[0]).Attributes.Count > 1)
            {
                controlBaseName = (variation.ChildNodes[0]).Attributes["ControlBase"].Value as string;
            }

            TestLog.Current.LogEvidence("Test " + controlType.ToString() + " for ArgumentNullException in Set/Get methods");

            ArrayList methodInfo = new ArrayList();

            methodInfo.AddRange( controlType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public) );

            SetExcludeList(variation);

            TestMethods(controlType, methodInfo);

            if (controlBaseName != null)
            {
                while ((controlType.Name != controlBaseName) && (controlType.Name != "Control"))
                {
                    controlType = controlType.BaseType;
                    methodInfo.Clear();
                    
                    methodInfo.AddRange(controlType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public));

                    TestLog.Current.LogStatus("Added base class " + controlType.ToString() + " for ArgumentNullException test in Set/Get methods");

                    TestMethods(controlType, methodInfo);
                }
            }

            if (!testResult)
            {
                return TestResult.Fail;
            }
            else
            {
                return TestResult.Pass;
            }
        }

        /// <summary>
        /// Get type for the given typeName
        /// </summary>
        /// <param name="typename"></param>
        /// <returns></returns>
        private Type GetControlType(string typename)
        {
            if (String.IsNullOrEmpty(typename))
                throw new ArgumentException("objectName cannot be null", "objectName");

            Type type = null;

            ArrayList objectNamespaces = new ArrayList();
            objectNamespaces.Add("System.Windows.Controls");
            objectNamespaces.Add("System.Windows.Controls.Primitives");

            foreach (string objectNamespace in objectNamespaces)
            {
                type = typeof(FrameworkElement).Assembly.GetType(objectNamespace + "." + typename, false);
                if (type != null)
                    break;
            }

            if (type == null)
                throw new Exception("Could not find the type " + typename + " in the object namespaces");

            return type;
        }

        /// <summary>
        /// Find and test Set/Get methods for ArgumentNullException
        /// </summary>
        /// <param name="control"></param>
        /// <param name="method"></param>
        private void TestMethods(Type controlType, ArrayList methodInfo)
        {
            foreach (MethodInfo method in methodInfo)
            {
                if (!(method.Name.StartsWith("Set") || method.Name.StartsWith("Get")))
                    continue;

                if (excludeList.Contains(method.DeclaringType + "." + method.Name))
                    continue;

                ParameterInfo[] parameterInfo = method.GetParameters();
                bool bResult = false;

                if ((parameterInfo[0].Position == 0) && (parameterInfo[0].ParameterType == typeof(DependencyObject)))
                {
                    bResult = TestForArgumentNullException(controlType, method, parameterInfo);
                }

                TestLog.Current.LogEvidence((bResult ? "PASS" : "FAIL") + " testing " + method.Name + " for ArgumentNullException");

                testResult &= bResult;
            }
        }
        /// <summary>
        /// Test for ArgumentNullException
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="parameterInfo"></param>
        public bool TestForArgumentNullException(Type controlType, MethodInfo method, ParameterInfo[] parameterInfo)
        {

            TestLog.Current.LogStatus("Test " + method.Name + " in " + method.DeclaringType.ToString() );

            ArrayList methodParams = new ArrayList(); ;

            DependencyObject dpo = null;

            methodParams.Add(dpo);

            Exception exception = null;

            for (int position = 1; position < parameterInfo.Length; position++)
            {
                TestLog.Current.LogStatus("Parameter: " + parameterInfo[position].Name);
                methodParams.Add(null);
            }
            
            try
            {
                controlType.InvokeMember(method.Name, BindingFlags.Default | BindingFlags.InvokeMethod , null, null, methodParams.ToArray());
            }

            catch (Exception exp)
            {
                exception = exp;
            }

            if (exception != null)
            {
                TestLog.Current.LogStatus("Actual exception: " + exception.InnerException.ToString());

                if (exception.InnerException.GetType().Equals(typeof(ArgumentNullException)))
                {
                        TestLog.Current.LogStatus("Exception Message: " + exception.InnerException.Message);
                        return true;
                }
            }

            return false;

        }

        /// <summary>
        /// Set methods to exlude from test - see Slider.SetTickPlacement for example
        /// </summary>
        /// <param name="variation"></param>
        private void SetExcludeList(XmlElement variation)
        {
            excludeList = new ArrayList();

            if (variation.ChildNodes.Count > 1)
            {
                foreach (XmlNode node in variation.ChildNodes[1])
                {
                    excludeList.Add(node.Name);
                }
            }
        }

        private bool testResult = true;
        private ArrayList excludeList;
    }
}
