//Verify setting AssemblyName using ThemeDictionaryExtension works.

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
    /// ThemeDictionaryExtension test
    /// </summary>
    public class ThemeDictionaryExtensionTest: IUnitTest
    {
        public TestResult Perform(object obj, XmlElement variation)
        {
            TestLog.Current.LogEvidence("ThemeDictionaryExtensionTest - test constructor with one parameter ");

            ThemeDictionaryExtension themeDictionaryExtension = new ThemeDictionaryExtension("testAssemblyName.dll");

            string retVal = themeDictionaryExtension.AssemblyName;

            if (retVal.Equals("testAssemblyName.dll"))
            {
                TestLog.Current.LogEvidence("PASS");
                return TestResult.Pass;
            }
            else
            {
                TestLog.Current.LogEvidence("FAIL: Expected value: testAssemblyName.dll, Actual value:  " + themeDictionaryExtension.AssemblyName );
                return TestResult.Fail;
            }
        }

    }
}
