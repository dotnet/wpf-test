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
using Avalon.Test.ComponentModel.Actions;
using Avalon.Test.ComponentModel.Validations;

namespace Avalon.Test.ComponentModel.UnitTests
{

    /// <summary>
    /// Add test coverage for MenuTemplateKeys. These template keys are defined in %sdxroot%\windows\wcp\Controls\MenuItem.cs 
    /// and used in the theme XAML files under %sdxroot%\windows\wcp\Themes. 
    /// Test gets the value of a key and verify the value. 
    /// </summary>
    public class MenuItemTemplateKeyTest : IUnitTest
    {
        public TestResult Perform(object obj, XmlElement variation)
        {
            bool testResult = true;

            TestLog.Current.LogStatus("MenuItemTemplateKeyTest");

            TestLog.Current.LogStatus("Expected: MenuItem.SubmenuHeaderTemplateKey: " + submenuHeaderTemplateKey.ToString());
            TestLog.Current.LogStatus("Actual: MenuItem.SubmenuHeaderTemplateKey: " + MenuItem.SubmenuHeaderTemplateKey.ToString());      
            TestLog.Current.LogStatus("Expected: MenuItem.SubmenuItemTemplateKey: " + submenuItemTemplateKey.ToString());
            TestLog.Current.LogStatus("Actual: MenuItem.SubmenuItemTemplateKey: " + MenuItem.SubmenuItemTemplateKey.ToString());
            TestLog.Current.LogStatus("Expected: MenuItem.TopLevelHeaderTemplateKey: " + topLevelHeaderTemplateKey.ToString());
            TestLog.Current.LogStatus("Actual: MenuItem.TopLevelHeaderTemplateKey: " + MenuItem.TopLevelHeaderTemplateKey.ToString());     
            TestLog.Current.LogStatus("Expected: MenuItem.TopLevelItemTemplateKey: " + topLevelItemTemplateKey.ToString());
            TestLog.Current.LogStatus("Actual: MenuItem.TopLevelItemTemplateKey: " + MenuItem.TopLevelItemTemplateKey.ToString());

            if (!MenuItem.SubmenuHeaderTemplateKey.ToString().Equals(submenuHeaderTemplateKey))
            {
                TestLog.Current.LogStatus("FAIL : SubmenuHeaderTemplateKey");
                testResult = false;
            }

            if (!MenuItem.SubmenuItemTemplateKey.ToString().Equals(submenuItemTemplateKey))
            {
                TestLog.Current.LogStatus("FAIL : SubmenuItemTemplateKey");
                testResult = false;
            }

            if (!MenuItem.TopLevelHeaderTemplateKey.ToString().Equals(topLevelHeaderTemplateKey))
            {
                TestLog.Current.LogStatus("FAIL : TopLevelHeaderTemplateKey");
                testResult = false;
            }

            if (!MenuItem.TopLevelItemTemplateKey.ToString().Equals(topLevelItemTemplateKey))
            {
                TestLog.Current.LogStatus("FAIL : TopLevelItemTemplateKey");
                testResult = false;
            }

            if (testResult)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        private const string submenuHeaderTemplateKey = "TargetType=System.Windows.Controls.MenuItem ID=SubmenuHeaderTemplateKey";
        private const string submenuItemTemplateKey = "TargetType=System.Windows.Controls.MenuItem ID=SubmenuItemTemplateKey";
        private const string topLevelHeaderTemplateKey = "TargetType=System.Windows.Controls.MenuItem ID=TopLevelHeaderTemplateKey";
        private const string topLevelItemTemplateKey = "TargetType=System.Windows.Controls.MenuItem ID=TopLevelItemTemplateKey";
    }
}
