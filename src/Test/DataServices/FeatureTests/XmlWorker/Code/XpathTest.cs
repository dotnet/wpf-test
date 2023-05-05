// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Reflection;
using System.Threading; 
using System.Windows.Threading;
using System.Windows;
using System.Xml;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{

    /// <summary>
    /// <description>
    /// This test is just intersting variations used with XPath 
    /// This subclasses XPathTest() test
    /// </description>
    /// </summary>
    [Test(2, "Xml", "XPathTestVariationTest")]
    public class XPathTestVariationTest : XPathTest
    {
        [Variation("/xml/player[2]/name")]
        [Variation("/xml/player[2]/processing-instruction()")]
        [Variation("/xml/player[2]/comment()")]
        [Variation("/xml/player[2]/name")]
        [Variation("/xml/player[2]/name/text()")]
        [Variation("/xml/player[2]/@attrib")]
        [Variation("/xml/player[votes > '1669065' and votes < '1669067']/name")]
        [Variation("/xml/player[last()]/name")]
        [Variation("/xml/player[position() = '2']/name")]
        [Variation("/xml/child::player[position() = '2']/child::name")]
        [Variation("/descendant::player[votes = '1669066']/name")]
        [Variation("/xml/player[@ID = 'P1']/name")]
        [Variation("/xml/player[@ID][2]/name")]
        [Variation("descendant::player[@ID = 'P4']/name")]
        [Variation("descendant-or-self::player[position = 'OF' and (team != 'Colorado Rockies' and votes != '2315204')]/name")]

        public XPathTestVariationTest(string s) : base(s)
       {
       }

    }


    /// <summary>
    /// <description>
    /// This test is to check binding using single node xpath expressions.
    /// </description>
    /// </summary>
    [Test(0, "Xml", "XPathTest")]
    public class XPathTest : WindowTest
    {
        // Private variables
        private HappyMan _happy;
        private XmlDocument _doc;
        private string _xPathString = "";

        #region Contructors

        /// <summary>
        /// Default Contructor, for the BVT test
        /// </summary>
        public XPathTest()
            : this("/xml/player[2]/name")
        {
        }


        public XPathTest(string xpathvalue)
        {
            _xPathString = xpathvalue;
            InitializeSteps += new TestStep(CreateTree);
            RunSteps += new TestStep(TwoWay_SetBinding);
            RunSteps += new TestStep(TwoWay_ChangeSource);
            RunSteps += new TestStep(TwoWay_ChangeTarget);
        }

        #endregion

        #region Initializing Steps

        /// <summary>
        /// Create a tree, initalize the test control, set the datacontext
        /// </summary>
        /// <returns>Result, success if everything worked, failure if the document is null</returns>
        private TestResult CreateTree()
        {
            // Output the Xpath expression that is being tested
            LogComment("Testing: " + _xPathString);

            Status("Creating the Element Tree");
            _happy = new HappyMan();
            _happy.HappyName = "George";
            _happy.Position = new Point(100, 100);
            //happy.Width = new Length (200);
            //happy.Height = new Length (200);
            _happy.Width = 200;
            _happy.Height = 200;
            Window.Content = _happy;

            Status("Getting xml stream");

            // Code to get a resource is copied from MSBuild output
            string baseName = Assembly.GetAssembly(this.GetType()).GetName().Name + ".g";
            string resId = "national_allstars.xml";

            System.Resources.ResourceManager _rm;
            System.Reflection.Assembly exeAsm = System.Reflection.Assembly.GetExecutingAssembly();

            _rm = new System.Resources.ResourceManager(baseName, exeAsm);

            System.IO.Stream xmlFile = _rm.GetStream(resId);

            if (xmlFile == null)
            {
                LogComment("xmlFile was null. Unable to begin test");
                return TestResult.Fail;
            }

            _doc = new XmlDocument();
            _doc.Load(xmlFile);
            if (_doc == null)
            {
                LogComment("XmlDocument was null. Unable to begin test");
                return TestResult.Fail;
            }
            _happy.DataContext = _doc;

            return TestResult.Pass;
        }

        #endregion

        #region Run Steps

        /// <summary>
        /// Set two way databinding on the NameProperty
        /// </summary>
        /// <returns></returns>
        private TestResult TwoWay_SetBinding()
        {
            Status("Setting TwoWay binding.");
            Binding bind = new Binding();
            bind.XPath = _xPathString;
            bind.Mode = BindingMode.TwoWay;
            _happy.SetBinding(HappyMan.HappyNameProperty, bind);

            return ValidateBinding("TwoWay_SetBinding");
        }

        /// <summary>
        /// Validate that the databound property updated when the new value is
        /// set in the XmlDocument
        /// </summary>
        private TestResult TwoWay_ChangeSource()
        {
            Status("Changing source value for TwoWay binding.");

            XmlNode node = _doc.SelectSingleNode(_xPathString);
            if (node == null)
            {
                LogComment("Unable to get a reference to the XmlNode");
                return TestResult.Fail;
            }
            node.InnerText = "new Source";

            return ValidateBinding("TwoWay_ChangeSource");
        }

        /// <summary>
        /// Validate that the XmlDocument updated when the new value is
        /// set on the bound property
        /// </summary>
        /// <returns></returns>
        private TestResult TwoWay_ChangeTarget()
        {
            Status("Changing target element value for TwoWay binding.");
            _happy.HappyName = "new Target";

            return ValidateBinding("TwoWay_ChangeTarget");
        }

        #endregion

        #region Helper Functions

        /// <summary>
        /// Validation function, checks the XmlDocument and the bound property
        /// to see that they match
        /// </summary>
        /// <param name="stepName">string, step name to use for the comment</param>
        /// <returns>Result, success if they match, failure if they do not</returns>
        private TestResult ValidateBinding(string stepName)
        {
            WaitForPriority(DispatcherPriority.Background);

            XmlNode node = _doc.SelectSingleNode(_xPathString);
            if (node == null)
            {
                LogComment("Unable to determine expected value");
                return TestResult.Fail;
            }
            if (node.InnerText != _happy.HappyName)
            {
                LogComment("XmlDom had: " + node.InnerText);
                LogComment("Control had: " + _happy.HappyName);
                return TestResult.Fail;
            }

            LogComment(stepName + " XmlDom and Control had the same value, value:" + _happy.HappyName);
            return TestResult.Pass;
        }

        /// <summary>
        /// Clear the binding and set the XmlDocument and the NameProperty to
        /// source and target values respectively
        /// </summary>
        /// <param name="source">string, value to set the XmlDocument to</param>
        /// <param name="target">string, value to set the NameProperty to</param>
        void ResetBinding(string source, string target)
        {
            Status("Clearing BindingExpression and reseting source and target");
            BindingOperations.ClearBinding(_happy, HappyMan.HappyNameProperty);

            XmlNode node = _doc.SelectSingleNode(_xPathString);
            node.InnerText = source;
            _happy.HappyName = target;
        }

        #endregion

    }
}
