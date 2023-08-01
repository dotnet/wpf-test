// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Avalon.Test.CoreUI.Resources
{
    /// <summary>
    /// This test case is designed to test a simple Resource reference on a Framework Element like StackPanel
    /// 
    /// To run this test:
    /// coretests.exe /Class="Avalon.Test.Resources.BVT1" /Method="RunTest"
    /// Because TestCaseInfo.Method has default value of "RunTest", command can be simplified as:
    /// coretests.exe /Class="Avalon.Test.Resources.BVT1" 
    /// </summary>
    /// <remarks>
    /// <ol>
    /// <li>Create StackPanel</li>
    /// <li>Set its Resources Property to a Resource Dictionary</li>
    /// <li>Set the VerticalAlignment property to a value in the Resource</li>
    /// <li>Verify the VerticalAlignment Property of the StackPanel is set correctly</li>
    /// <li>Now Directly change the VerticalAlignment property of the StackPanel</li>
    /// <li>Verify the VerticalAlignment Property</li>
    ///</ol>
    /// </remarks>            
    [Test(0, "Resources.BVT", TestCaseSecurityLevel.FullTrust, "BVT1")]
    public class BVT1 : TestHelper
    {
        #region Constructor
        /******************************************************************************
        * Function:          BVT1 Constructor
        ******************************************************************************/
        public BVT1()
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        TestResult StartTest()
        {
            this.Init();

            GlobalLog.LogStatus("Creating a TextBox and set Height and Width");
            TextBox textbox = new TextBox();
            textbox.Width = 100.00;
            textbox.Height = 100.00;

            GlobalLog.LogStatus("Creating ResourceDictionaryHelper");
            ResourceDictionaryHelper resourceDictionayHelper = new ResourceDictionaryHelper();

            GlobalLog.LogStatus("Setting TextBox.Resources to new ResourceDictionay");
            textbox.Resources = resourceDictionayHelper.CreateCharacterCasing();

            GlobalLog.LogStatus("Assigning CharacterCasing property of TextBox to a value in the Resource");
            textbox.SetResourceReference(TextBox.CharacterCasingProperty, "CCLower");

            string str = textbox.CharacterCasing.ToString();
            CheckResults(str == "Lower", "Checking CharacterCasing Property of textbox", "Lower", str);

            GlobalLog.LogStatus("Setting CharacterCasing property directly now and see if it works");
            textbox.CharacterCasing = CharacterCasing.Upper;

            str = textbox.CharacterCasing.ToString();

            CheckResults(str == "Upper", "Checking CharacterCasing Property of textbox", "Upper", str);

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion
    }

}

