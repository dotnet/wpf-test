// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.HighLevelScenarios.Regressions</area>



    [Test(2, "Animation.HighLevelScenarios.Regressions", "SerializeKeyTimeUniformTest")]
    public class SerializeKeyTimeUniformTest : AvalonTest
    {

        #region Constructor

        /******************************************************************************
        * Function:          SerializeKeyTimeUniformTest Constructor
        ******************************************************************************/
        public SerializeKeyTimeUniformTest()
        {
            InitializeSteps += new TestStep(StartTest);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Creates a KeyFrame and starts the test.
        /// </summary>
        /// <returns>Returns success if the element was found</returns>
        TestResult StartTest()
        {
            LinearThicknessKeyFrame obj = new LinearThicknessKeyFrame();
            obj.KeyTime = KeyTime.Uniform;
            SerializeObject(obj, "temp.xaml");

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          SerializeObject
        ******************************************************************************/
        private void SerializeObject(object root, string filename)
        {
            Console.WriteLine("Serializing " + root.ToString());
            string outerXml = SerializeObjectTree(root,  XamlWriterMode.Expression);

            GlobalLog.LogStatus("***Result of Serialization:\n" + outerXml);

            if (outerXml.IndexOf("Uniform") >= 0)
            {
                Signal("TestFinished", TestResult.Pass);
            }
            else
            {
                Signal("TestFinished", TestResult.Fail);
            }
        }

        /******************************************************************************
        * Function:          SerializeObjectTree - 1
        ******************************************************************************/
        public string SerializeObjectTree(object objectTree, XamlWriterMode expressionMode)
        {

            StringBuilder sb = new StringBuilder();
            TextWriter writer = new StringWriter(sb);
            XmlTextWriter xmlWriter = null;

            try
            {
                // Create XmlTextWriter
                xmlWriter = new XmlTextWriter(writer);

                // Set serialization mode
                XamlDesignerSerializationManager manager = new XamlDesignerSerializationManager(xmlWriter);
                manager.XamlWriterMode = expressionMode;


                // Serialize
                SerializeObjectTree(objectTree, manager);
            }
            finally
            {
                if(xmlWriter != null)
                    xmlWriter.Close();
            }

            return sb.ToString();
        }

        /******************************************************************************
        * Function:          SerializeObjectTree - 2
        ******************************************************************************/
        public void SerializeObjectTree(object objectTree, XamlDesignerSerializationManager manager)
        {
            // Serialize.
            //The test case passes if this statement does not throw a NullReferenceException.
            XamlWriter.Save(objectTree, manager);
        }

        /******************************************************************************
        * Function:          Verify
        ******************************************************************************/
        /// <summary>
        /// Verifies the result of the test case, and returns a Pass/Fail result.
        /// </summary>
        /// <returns></returns>
        TestResult Verify()
        {
            TestResult result = WaitForSignal("TestFinished");

            return result;
        }

        #endregion
    }
}
