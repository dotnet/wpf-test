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
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Avalon.Test.CoreUI.Resources
{
    /// <summary>
    /// Test whether the default property is evaluated when the Resource reference on a Framework elemet is made null
    /// 
    /// To run this test:
    /// coretests.exe /Class="Avalon.Test.Resources.BVT2" /Method="RunTest"
    /// Because TestCaseInfo.Method has default value of "RunTest", command can be simplified as:
    /// coretests.exe /Class="Avalon.Test.Resources.BVT2"
    /// </summary>
    /// <remarks>
    /// <ol>
    /// <li>Create StackPanel</li>
    /// <li>Set its Resources Property to a Resource Dictionary</li>
    /// <li>Set the VerticalAlignment property to a value in the Resource</li>
    /// <li>Verify the VerticalAlignment Property of the StackPanel is set correctly</li>
    /// <li>Make the StackPanel Resource null</li>
    /// <li>Verify the VerticalAlignment Property and see that we get the default value</li>
    ///</ol>
    /// </remarks>
    [Test(0, "Resources.BVT", TestCaseSecurityLevel.FullTrust, "BVT2")]
    public class BVT2 : TestHelper
    {
        #region Constructor
        /******************************************************************************
        * Function:          BVT2 Constructor
        ******************************************************************************/
        public BVT2()
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        TestResult StartTest()
        {
            this.Init();

            GlobalLog.LogStatus("Creating a TextBox and set Height and width");
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

            GlobalLog.LogStatus("Setting TextBox.Resource to null");
            textbox.Resources = null;
            textbox.ClearValue(TextBox.CharacterCasingProperty);

            str = textbox.CharacterCasing.ToString();
            CheckResults(str == "Normal", "Checking CharacterCasing Property of textbox", "Normal", str);

            GlobalLog.LogStatus("Setting TextBox.Resources to new ResourceDictionay");
            textbox.Resources = resourceDictionayHelper.CreateCharacterCasing();
            textbox.SetResourceReference(TextBox.CharacterCasingProperty, "CCLower");

            str = textbox.CharacterCasing.ToString();
            CheckResults(str == "Lower", "Checking CharacterCasing Property of textbox", "Lower", str);


            // Create Resource Dictionary
            ResourceDictionary rd = new ResourceDictionary();

            // Add Resources
            rd.Add("r1", Brushes.Green);
            rd.Add("r2", 2.0);
            rd.Add("r3", "test");
            rd.Add("r4", Brushes.Blue);
            rd.Add("r5", Brushes.Orange);
            rd.Add("r6", Brushes.Red);

            // Iterate through keys.
            IEnumerator keyEnumerator = rd.Keys.GetEnumerator();
            int counter = 0;
            while (keyEnumerator.MoveNext())
            {
                if (keyEnumerator.Current != null)
                {
                    counter++;
                }
            }

            foreach (object key in rd)
            {
                GlobalLog.LogStatus("Key " + key.ToString());
            }

            // Iterate through Values.
            IEnumerator valueEnumerator = rd.Values.GetEnumerator();
            int counter2 = 0;
            while (valueEnumerator.MoveNext())
            {
                if (valueEnumerator.Current != null)
                {
                    counter2++;
                }
            }

            foreach (object valueKey in rd.Values)
            {
                GlobalLog.LogStatus("Value Key " + valueKey.ToString());
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion
    }
}

