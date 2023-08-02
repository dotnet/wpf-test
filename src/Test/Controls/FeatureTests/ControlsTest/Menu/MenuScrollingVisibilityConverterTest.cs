using System;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Collections;

using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using Avalon.Test.ComponentModel;
using Microsoft.Test.Logging;
using Avalon.Test.ComponentModel.Actions;
using Avalon.Test.ComponentModel.Validations;

namespace Avalon.Test.ComponentModel.UnitTests
{

    /// <summary>
    /// MenuScrollingVisibilityConverterTest - call ConvertBack method
    /// which dont have test coverage now. Right now calling this method should 
    /// always return Binding.DoNothing
    /// </summary>
    public class MenuScrollingVisibilityConverterTest : IUnitTest
    {
        public TestResult Perform(object obj, XmlElement variation)
        {
            bool testResult = false;

            TestLog.Current.LogStatus("MenuScrollingVisibilityConverterTest");

            MenuScrollingVisibilityConverter menuScrollingVisibilityConverter = new MenuScrollingVisibilityConverter();


            //This method always returns Binding.DoNothing and not really doing anything particular - see the implementation
            //of MenuScrollingVisibilityConverter for details
            object[] retVal = menuScrollingVisibilityConverter.ConvertBack(null, new Type[4],  null, System.Globalization.CultureInfo.CurrentCulture);

            if (retVal.Length == 1)
            {
                if (retVal[0] == Binding.DoNothing)
                {
                    testResult = true;
                }
                else
                {
                    TestLog.Current.LogEvidence("Expected Binding.DoNothing returned");
                    TestLog.Current.LogEvidence("Actual: " + retVal[0].ToString());
                }
            }
            else
            {
                TestLog.Current.LogEvidence("MenuScrollingVisibilityConverter.ConvertBack return value contains : " + retVal.Length + " elements");
                TestLog.Current.LogEvidence("Expected only one");
            }

            if (testResult)
            {
                TestLog.Current.LogStatus("PASS");
                return TestResult.Pass;
            }
            else
            {
                TestLog.Current.LogStatus("FAIL");
                return TestResult.Fail;
            }
        }

    }
}
