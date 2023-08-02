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
    /// ComponentResourceKey ToString() test
    /// </summary>
    public class ComponentResourceKeyToStringTest : IUnitTest
    {
        public TestResult Perform(object obj, XmlElement variation)
        {
            //bool testResult = false;

            ComponentResourceKey componentResourceKey = new ComponentResourceKey();

            string componentResourceKeyToStringValue =  componentResourceKey.ToString();

            if (componentResourceKeyToStringValue.Equals("TargetType=null ID=null"))
            {
                TestLog.Current.LogEvidence("PASS");
                return TestResult.Pass;
            }
            else
            {
                TestLog.Current.LogEvidence("FAIL: Expected value:  TargetType=null ID=null, Actual value:  " + componentResourceKey.ToString());
                return TestResult.Fail;
            }
        }

    }
}
