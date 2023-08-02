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
    /// ComponentResourceKeyConverter test
    /// </summary>
    public class ComponentResourceKeyConverterTest : IUnitTest
    {
        public TestResult Perform(object obj, XmlElement variation)
        {
            bool testResult = true;

            ComponentResourceKeyConverter compResourceKeyConveter = new ComponentResourceKeyConverter();

            ComponentResourceKey key = new ComponentResourceKey(typeof(FrameworkElement), "MenuScrollViewer");

            TestLog.Current.LogStatus("Testing CompResourceKeyConveter.CanConvertFrom(ITypeDescriptorContext context,Type sourceType) method");

            //This method always returns false and not really doing anything particular - see the implementation
            //of ComponentResourceKeyConverter for details
            bool retVal = compResourceKeyConveter.CanConvertFrom(null, typeof(Button));

            if (retVal)
            {
                testResult = false;
                TestLog.Current.LogStatus("Expected: CompResourceKeyConveter.CanConvertFrom return false");
                TestLog.Current.LogStatus("Actual: CompResourceKeyConveter.CanConvertFrom returned true");
            }

            bool exceptionThrown = false;

            try
            {
                TestLog.Current.LogStatus("Testing CompResourceKeyConveter.ConvertFrom(ITypeDescriptorContext context,CultureInfo culture,object value) method");
                //This method always throws NotSupportedException and not really doing anything particular - see the implementation
                //of ComponentResourceKeyConverter for details
                compResourceKeyConveter.ConvertFrom(null, System.Globalization.CultureInfo.CurrentCulture, key);
            }
            catch (NotSupportedException e)
            {
                TestLog.Current.LogEvidence(e);
                exceptionThrown = true;
            }

            if (!exceptionThrown)
            {
                testResult = false;
                TestLog.Current.LogStatus("FAIL");
                TestLog.Current.LogStatus("Expected: NotSupportedException thrown");
                TestLog.Current.LogStatus("Actual: NotSupportedException  not thrown");
            }

            TestLog.Current.LogStatus("Testing CompResourceKeyConveter.ConvertTo(ITypeDescriptorContext context, CultureInfo culture,object value, Type destinationType) method");

            //This method always returns false and not really doing anything particular - see the implementation
            //of ComponentResourceKeyConverter for details            
            retVal = (bool)compResourceKeyConveter.ConvertTo(null, System.Globalization.CultureInfo.CurrentCulture, key, typeof(Button));          

            if (retVal)
            {
                testResult = false;
                TestLog.Current.LogStatus("Expected: CompResourceKeyConveter.ConvertTo return false");
                TestLog.Current.LogStatus("Actual: CompResourceKeyConveter.ConvertTo returned " + retVal.ToString());
            }
           

            if (testResult)
            {
                TestLog.Current.LogEvidence("PASS");
                return TestResult.Pass;
            }
            else
            {
                TestLog.Current.LogEvidence("FAIL");
                return TestResult.Fail;
            }
        }

    }
}
