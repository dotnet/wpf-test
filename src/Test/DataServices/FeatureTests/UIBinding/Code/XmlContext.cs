// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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
	/// TODO
	/// </description>
	/// </summary>
    [Test(0, "Binding", "XmlContext")]
    public class XmlContext : WindowTest
    {
        HappyMan _happy;
        XmlDocument _doc;
        string _xPath = "/root/dwarf/Name";

        public XmlContext() {
            InitializeSteps += new TestStep(CreateTree);
            RunSteps += new TestStep (SetBinding);
            RunSteps += new TestStep (ChangeSource);
            RunSteps += new TestStep (ChangeTarget);
        }

        private TestResult CreateTree() {
            Status("Creating the Element Tree");
            _happy = new HappyMan();
            _happy.HappyName = "George";
            Window.Content = _happy;

            _doc = new XmlDocument();
            _doc.LoadXml("<root><dwarf><Name>Sleepy</Name></dwarf></root>");

            _happy.DataContext = _doc;
            return TestResult.Pass;
        }


        // Set TwoWay binding.
		private TestResult SetBinding()
		{
            Status("Setting TwoWay binding.");
            Binding bind = new Binding();
            bind.XPath = _xPath;
            bind.Mode = BindingMode.TwoWay;
            _happy.SetBinding(HappyMan.HappyNameProperty, bind);
            return ValidateBinding("Sleepy");
        }

        // Validate behavior on TwoWay binding when source value is changed.
        private TestResult ChangeSource ()
        {
            Status("Changing source value for TwoWay binding.");
            XmlNode node = _doc.SelectSingleNode(_xPath);
            node.InnerText = "new Source";

            //Wait for binding to happen
            WaitForPriority(DispatcherPriority.DataBind);
            return ValidateBinding("new Source");
        }

        // Validate behavior on TwoWay binding when target element is changed.
        private TestResult ChangeTarget ()
        {
            Status("Changing target element value for TwoWay binding.");
            _happy.HappyName = "new Target";

            //Wait for binding to happen
            WaitForPriority(DispatcherPriority.DataBind);
            return ValidateBinding("new Target");
        }

        // Validates that the source and target are the expected values
        private TestResult ValidateBinding(string expectedValue) {
            XmlNode node = _doc.SelectSingleNode(_xPath);

            if (expectedValue != node.InnerText)
            {
                LogComment("Source XmlElement name has an unexpected value");
                LogComment("Expected: " + expectedValue);
                LogComment("Actual: " + node.InnerText);
                return TestResult.Fail;
            }

            if (expectedValue != _happy.HappyName)
            {
                LogComment("Target HappyMan name has an unexpected value");
                LogComment("Expected: " + expectedValue);
                LogComment("Actual: " + _happy.HappyName);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

    }
}
