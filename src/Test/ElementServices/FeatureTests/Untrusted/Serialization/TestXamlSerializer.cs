// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: using reflection to cover the behavior of XamlSerializer.
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Windows;
using Avalon.Test.CoreUI.Common;
using System.Reflection;
using System.Diagnostics;
namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    /// A test to cover methods in XamlSerializer with reflection.
    /// </summary>
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCasePriority("1")]
    [TestCaseArea(@"Serialization\XamlSerializer\Boundary")]
    [TestCaseMethod("Run")]
    [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
    public class TestXamlSerializer
    {
        /// <summary>
        /// Entrance
        /// </summary>
        public void Run()
        {
            CoreLogger.LogStatus("Verifying XamlSerializer with reflection.");
            CoreLogger.LogStatus("Loading Assembly ...");
            Assembly assembly = Assembly.Load(_asmName);
            CoreLogger.LogStatus("Finding XamlSerializer Type ...");
            _xamlSerializerType = assembly.GetType("System.Windows.Markup.XamlSerializer", true, true);
            CoreLogger.LogStatus("Creating XamlSerializer Instance ...");
            _xamlSerializerInstance = Activator.CreateInstance(_xamlSerializerType);
            //, BindingFlags.Instance | BindingFlags.NonPublic, null, null, System.Globalization.CultureInfo.InvariantCulture);

            TestEachMethod();
        }
        private void TestEachMethod()
        {
            TestMethod("ConvertXamlToBaml", 4);
            TestMethod("ConvertXamlToObject", 5);
            TestMethod("ConvertBamlToObject", 3);
            TestMethod("ConvertStringToCustomBinary", 2);
            TestMethod("ConvertCustomBinaryToObject", 1);
        }
        private void TestMethod(string methodName,int paramtersNumber)
        {
            CoreLogger.LogStatus("Testing method " + methodName + " ...");
            object[] param = new Array[paramtersNumber];
            bool gotCorrectException = false;
            for (int i = 0; i < paramtersNumber; i++)
            {
                param[i] = null;
            }
            try
            {
                MethodInfo mInfo;
                mInfo = _xamlSerializerType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                Debug.Assert((mInfo != null), "Can't find Method with name = " + methodName + ".");
                mInfo.Invoke(_xamlSerializerInstance, param);
            }
            catch (TargetInvocationException exception)
            {
                if(exception.InnerException is InvalidOperationException)
                    gotCorrectException = true;
                else
                    CoreLogger.LogStatus("Exception : " + exception.ToString());
            }
            catch(Exception exception)
            {
                CoreLogger.LogStatus("Exception : " + exception.ToString());
            }

            if(!gotCorrectException)
            {
                throw new Microsoft.Test.TestValidationException("Not throwing InvalidOperationException.");
            }
        }

        private object _xamlSerializerInstance;
        private Type _xamlSerializerType;
        private const string _asmName = "PresentationFramework, " +
                                 "Version=" + Microsoft.Internal.BuildInfo.WCP_VERSION + "," +
                                 "Culture=neutral, " +
                                 "PublicKeyToken=" + Microsoft.Internal.BuildInfo.WCP_PUBLIC_KEY_TOKEN + "";

    }
}
