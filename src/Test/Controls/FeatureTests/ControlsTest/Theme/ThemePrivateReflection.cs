using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

using System;
using System.Reflection;
using System.Windows;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// ThemePrivateReflection
    /// Verify that private reflection into UxThemeWrapper still works
    /// </summary>
    [Test(1, "Theme", Versions="4.8+")]
    public class ThemePrivateReflection : StepsTest
    {
        public ThemePrivateReflection()
        {
            RunSteps += new TestStep(RunTest);
        }

        public TestResult RunTest()
        {
            // Some apps use private reflection into UxThemeWrapper.  This is
            // officially not supported, but in practice we should not break
            // compat by removing the private fields.
            //
            // Note that this only tests that the fields are present.  We make
            // no promises about the effect of setting or getting the field values.

            Assembly presentationFramework = typeof(FrameworkElement).Assembly;
            Type uxThemeWrapper = presentationFramework.GetType("MS.Win32.UxThemeWrapper");
            FieldInfo fiIsActive = uxThemeWrapper.GetField("_isActive", BindingFlags.Static | BindingFlags.NonPublic);
            FieldInfo fiThemeName = uxThemeWrapper.GetField("_themeName", BindingFlags.Static | BindingFlags.NonPublic);
            FieldInfo fiThemeColor = uxThemeWrapper.GetField("_themeColor", BindingFlags.Static | BindingFlags.NonPublic);

            TestResult result = TestResult.Pass;
            if (fiIsActive == null)
            {
                result = TestResult.Fail;
                Log.LogStatus("UxThemeWrapper._isActive is missing");
            }
            if (fiThemeName == null)
            {
                result = TestResult.Fail;
                Log.LogStatus("UxThemeWrapper._themeName is missing");
            }
            if (fiThemeColor == null)
            {
                result = TestResult.Fail;
                Log.LogStatus("UxThemeWrapper._themeColor is missing");
            }

            return result;
        }
    }
}

